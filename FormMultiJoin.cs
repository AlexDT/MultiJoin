#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using static MultiJoin.Command;

using Form = System.Windows.Forms.Form;
#endregion


namespace MultiJoin
{
    public partial class FormMultiJoin : Form
    {
        internal List<JoinableElement> selectedElements = new List<JoinableElement>();
        internal List<Category> selectedCategories = new List<Category>();

        public FormMultiJoin()
        {
            InitializeComponent();

            #region Present unique category list to user

            foreach (ElementId eId in selectedElementIds)
            {
                selectedElements.Add(new JoinableElement(eId));

                Debug.WriteLine(selectedElements.Where(e => e.elementId == eId).First().canBeJoined.ToString());
            }

            clbCategories.Sorted = true;
            clbCategories.DataSource = JoinableElement.uniqueCategories;
            clbCategories.DisplayMember = "Name";

            for (int i = 0; i < clbCategories.Items.Count; i++)
            {
                clbCategories.SetItemChecked(i, true);
            }
            #endregion
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            #region Select elements based on category
            foreach (Category itemChecked in clbCategories.CheckedItems)
            {
                selectedCategories.Add(itemChecked);
            }
            #endregion

            using (Transaction tx = new Transaction(doc, "Joined multiple elements"))
            {
                tx.Start();

                /* -----
                * v1. Let the list loop over the elements
                * v2. Check if already joined 
                * 3. Check if boundingbox intersects
                * 3. Check if material matches
                * 4. Try catch any other problems and report amount that couldn't be joined?
                */

                string resultMessage = "";

                foreach (JoinableElement joinableElement in selectedElements/*.Where(ec => selectedCategories.Contains(ec.category))*/)
                {
                    if (!joinableElement.canBeJoined ||
                        !selectedCategories.Exists(c => c.Id == joinableElement.category.Id))
                    { continue; }

                    foreach (Element element in joinableElement.canJoinWith)
                    {
                        if (!selectedCategories.Exists(c => c.Id == element.Category.Id) ||
                            !JoinGeometryUtils.AreElementsJoined(doc, joinableElement.element, element))
                        { continue; }

                        try
                        {
                            JoinGeometryUtils.JoinGeometry(doc, joinableElement.element, element);
                        }
                        catch
                        {
                            resultMessage += (joinableElement.name + "." + " <=> " + element.Name + "." + "\n");
                        }
                    }
                }

                TaskDialog.Show("Results", resultMessage);

                tx.Commit();

                ClearMemory();
                Close();
            }

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void ClearMemory()
        {
            selectedElements.Clear();
            selectedCategories.Clear();
            selectedElementIds.Clear();
        }
    }
}
