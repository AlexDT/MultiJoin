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

#endregion

/* SUMMARY ADD-IN
 
> . . . . .
> . . . . .

*/


/* TODO:
 
> . . . . .
> . . . . .

*/

/* VERSIONS 

Version 0.0.1
+ . . . . . .

Version 0.0.2
+ . . . . . .

*/

namespace MultiJoin
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static UIApplication uiapp;
        public static UIDocument uidoc;
        public static Application app;
        public static Document doc;

        internal static ICollection<ElementId> selectedElementIds;
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

            selectedElementIds = uidoc.Selection.GetElementIds().ToList();

            using (var form = new FormMultiJoin())
            {
                if (selectedElementIds.Count < 2)
                {
                    TaskDialog.Show("Selection", "Please select 2 or more elements to be joined.");

                    return Result.Failed;
                }
                else
                {
                    form.ShowDialog();

                    // Cancel when user hits escape
                    if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                        form.Close();
                }
            }

            return Result.Succeeded;
        }
    }
}
