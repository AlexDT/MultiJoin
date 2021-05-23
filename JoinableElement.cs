using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using System.Diagnostics;

using static MultiJoin.Command;

namespace MultiJoin
{
    internal class JoinableElement
    {
        public static List<Category> uniqueCategories = new List<Category>();

        private static int counter;

        #region Internal properties
        public ElementId elementId;
        internal Element element;
        internal string name;
        internal Category category;
        internal bool canBeJoined { get; set; }
        #endregion

        public JoinableElement(ElementId eId)
        {
            elementId = eId;
            element = doc.GetElement(elementId);
            name = element.Name;
            category = element.Category;
            canBeJoined = true;

            addCategory(element.Category);
            counter += 1;

            Debug.WriteLine(name + ": " + elementId + " - " + category.Name);

        }
        public JoinableElement(Element ele)
        {
            element = ele;
            elementId = element.Id;
            name = element.Name;
            category = element.Category;
            canBeJoined = true;

            addCategory(element.Category);
            counter += 1;

            Debug.WriteLine(name, elementId, category.Name);

        }

        private void addCategory(Category cat)
        {
            if (!uniqueCategories.Any(c => c.Name == cat.Name))
            {
                uniqueCategories.Add(cat);
            }
        }
    }
}
