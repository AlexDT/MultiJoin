#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RHDRevitLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MultiJoin.Command;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Form = System.Windows.Forms.Form;
using View = Autodesk.Revit.DB.View;
using System.Diagnostics;
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

                foreach (JoinableElement je in selectedElements/*.Where(ec => selectedCategories.Contains(ec.category))*/)
                {
                    if (je.canBeJoined &&
                        selectedCategories.Exists(c => c.Id == je.category.Id))
                    {
                        foreach (Element f in je.canJoinWith)
                        {
                            if (selectedCategories.Exists(c => c.Id == f.Category.Id) &&
                                !JoinGeometryUtils.AreElementsJoined(doc, je.element, f))
                            {
                                try
                                {
                                    JoinGeometryUtils.JoinGeometry(doc, je.element, f);
                                }
                                catch
                                {
                                    resultMessage += (je.name + "." + " <=> " + f.Name + "." + "\n");
                                }
                            }
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
