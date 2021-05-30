﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using System.Diagnostics;

using static MultiJoin.Command;

namespace MultiJoin
{
    public class JoinableElement
    {
        #region public static properties
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
        internal BoundingBoxIntersectsFilter boundingBoxFilter { get; set; }
        internal bool canBeJoined { get; set; }
        internal List<Element> canJoinWith { get; set; }
        //internal FilteredElementCollector collector { get; set; }
        #endregion

        public JoinableElement(ElementId eId)
        {
            elementId = eId;
            element = doc.GetElement(elementId);
            name = element.Name;
            category = element.Category;
            canBeJoined = true;
            boundingBox = element.get_BoundingBox(doc.ActiveView);
            XYZ boundingBoxMin = boundingBox.Min;
            XYZ boundingBoxMax = boundingBox.Max;
            boundingBoxOutline = new Outline(boundingBoxMin, boundingBoxMax);
            boundingBoxFilter = new BoundingBoxIntersectsFilter(boundingBoxOutline, 100);
            canJoinWith = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(boundingBoxFilter).ToList();

            addCategory(element.Category);
            addJoinable(element);
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
        private void addJoinable(Element ele)
        {
            if (canBeJoined)
            {
                canJoinWith.Add(ele);
            }
        }
    }
}
