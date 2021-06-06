using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using static MultiJoin.Command;
using static MultiJoin.JoinableElement;

namespace MultiJoin
{
    //This script will recursively find all the elements in a group. 
    class GroupUtil
    {
        public static List<ElementId> GetMembersRecursive(Document doc, Group g, List<ElementId> r = null)
        {
            if (r == null)
            {
                r = new List<ElementId>();
            }

            List<ElementId> elems = g.GetMemberIds()/*.Select(q => doc.GetElement(q))*/.ToList();

            //It iterates over the elements inside the first group and recognizes the groups that are inside that group. 
            foreach (ElementId el in elems)
            {
                //When it finds a group it will calls itself to run again on that group (recursive) 
                if (el.GetType() == typeof(Group))
                {
                    GetMembersRecursive(doc, (Group)doc.GetElement(el), r);
                    continue;
                }

                r.Add(el);
            }

            return r;
        }
    }
}
