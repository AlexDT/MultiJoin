#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RHDRevitLib;
using RHDRevitLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using static MultiJoin.GroupUtil;

#endregion

namespace MultiJoin
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static UIApplication uiapp;
        public static UIDocument uidoc;
        public static Application app;
        public static Document doc;

        internal static List<ElementId> selectedElementIds;
        public List<ElementId> groupedElements;

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            #region  Security Check
            Security security = new Security();
            if (security.RHDHVemployeeCheck() != true)
            {
                TaskDialog.Show("Failed", "System is not recognized as a RHDHV System" + "\n" + "\n"
                                    + "Addin will not Load");
                return Result.Failed;
            }
            #endregion

            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;

            using (var form = new FormMultiJoin())
            {
                selectedElementIds = uidoc.Selection.GetElementIds().ToList();

                //Finds all the grouped elements and adds 
                foreach (ElementId id in selectedElementIds.Where(g => g.GetType() == typeof(Group)))
                {
                    groupedElements.AddRange(GetMembersRecursive(doc, (Group)doc.GetElement(id)));
                }

                //Removes Group from the list
                foreach (ElementId gId in selectedElementIds.Where(g => g.GetType() == typeof(Group)))
                {
                    selectedElementIds.Remove(gId);
                }

                //if group exists add its elementids to the list.
                if (groupedElements != null)
                {
                    selectedElementIds.AddRange(groupedElements);
                }

                if (selectedElementIds.Count < 2)
                {
                    TaskDialog.Show("Selection", "Please select 2 or more elements to be joined.");

                    return Result.Failed;
                }
                else if (selectedElementIds.Count > 10)
                {
                    TaskDialog td = new TaskDialog("MultiJoin - " + selectedElementIds.Count + " items selected.")
                    {
                        MainContent = "You have selected more than 10 elements. Processing time may be long." +
                        Environment.NewLine + Environment.NewLine +
                        "Are you sure you want to proceed?"
                    };

                    td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "No", "Stop action and make a smaller selection.");
                    td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Yes", "Execute tool, I have time.");
                    td.DefaultButton = TaskDialogResult.CommandLink1;
                    td.AllowCancellation = true;

                    switch (td.Show())
                    {
                        case TaskDialogResult.CommandLink1:

                            return Result.Cancelled;

                        case TaskDialogResult.CommandLink2:

                            form.ShowDialog();

                            // Cancel when user hits escape
                            if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                                form.Close();

                            break;

                        case 0:

                            return Result.Failed;
                    }
                }
            }

            return Result.Succeeded;
        }
    }
}
