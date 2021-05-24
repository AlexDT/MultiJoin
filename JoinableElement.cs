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
        #region public properties
        public static List<Category> uniqueCategories = new List<Category>();
        public static int counter;
        #endregion

        #region Internal properties
        internal ElementId elementId { get; }
        internal Element element { get; }
        internal string name { get; }
        internal Category category { get; }
        internal BoundingBoxXYZ boundingBox { get; }
        internal XYZ boundingBoxMin { get; }
        internal XYZ boundingBoxMax { get; }
        internal Outline boundingBoxOutline { get; }
        internal BoundingBoxIntersectsFilter boundingBoxFilter {get;}
        internal bool canBeJoined { get; set; }
        #endregion

        public JoinableElement(ElementId eId)
        {
            elementId = eId;
            element = doc.GetElement(elementId);
            name = element.Name;
            category = element.Category;
            canBeJoined = false;
            boundingBox = element.get_BoundingBox(doc.ActiveView);
            XYZ bbMin = boundingBox.Min;
            XYZ bbMax = boundingBox.Max;
            boundingBoxOutline = new Outline(boundingBoxMin, boundingBoxMax);

            addCategory(element.Category);
            counter += 1;

            Debug.WriteLine(name + ": " + elementId + " - " + category.Name);

        }
        private void addCategory(Category cat)
        {
            if (!uniqueCategories.Exists(c => c.Id == cat.Id))
            {
                uniqueCategories.Add(cat);
            }
        }
    }
}
