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
#endregion


namespace MultiJoin
{
    public partial class FormMultiJoin : Form
    {
        public FormMultiJoin()
        {
            InitializeComponent();

            //List<Category> selectedElementCategories = new List<Category>();
            List<JoinableElement> selectedElements = new List<JoinableElement>();
            List<Category> selectedCategories = new List<Category>();
            //List<Element> selectionToJoin = new List<Element>();

            #region Present unique category list to user
            //foreach (ElementId e in selectedElementIds.ToList())
            //{
            //    Category cat = doc.GetElement(e).Category;

            //    selectedElementCategories.Add(cat);
            //}

            foreach (ElementId eId in selectedElementIds)               
            {
                selectedElements.Add(new JoinableElement(eId));
            }

            //clbCategories.DataSource = selectedElementCategories.GroupBy(x => x.Name).Select(g => g.First()).ToList();
            clbCategories.Sorted = true;
            clbCategories.DataSource = JoinableElement.uniqueCategories;
            clbCategories.DisplayMember = "Name";
            #endregion

            #region Select elements based on category
            foreach (Category itemChecked in clbCategories.CheckedItems)
            {
                selectedCategories.Add(itemChecked);
            }

            #endregion

            /* -----
            * 1. Let the list loop over the elements (n!)
            * 2. Check if already joined 
            * 3. Check if boundingbox intersects
            * 3. Check if material matches
            * 4. Try catch any other problems and report amount that couldnt be joined?
            */

            foreach (JoinableElement e in selectedElements)
            {
                foreach (JoinableElement f in selectedElements)
                {
                    if (e.elementId != f.elementId && !JoinGeometryUtils.AreElementsJoined(doc, e.element, f.element))
                    {
                        //get only the instance that intersect the current line of the path
                        foreach (JoinableElement e in selectedElements)
                        {
                            selectedElements;
                        }


                    }
                }
            }

        }

        private void btn_OK_Click(object sender, EventArgs e)
        {

            using (Transaction tx = new Transaction(doc, "Joined multiple elements"))
            {

                tx.Start();

                tx.Commit();
            }

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

}
