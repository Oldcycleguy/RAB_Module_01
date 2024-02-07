#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace RAB_Module_01
{
    [Transaction(TransactionMode.Manual)]
    public class Module01Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here
            int number = 250;
            int elevation = 0;
            int floorHeight = 15;

            FilteredElementCollector collector1 = new FilteredElementCollector(doc);
            collector1.OfClass(typeof(ViewFamilyType));

            //Get titleblock
            FilteredElementCollector collector2 = new FilteredElementCollector(doc);
            collector2.OfCategory(BuiltInCategory.OST_TitleBlocks);
            //had to refer to the old Module 01 challenge review for this bit as 
            //it just threw up an error about not being a Titleblock type without it
            collector2.WhereElementIsElementType();
            ElementId tbId = collector2.FirstElementId();


            //Find the floorplan view type
            ViewFamilyType floorplanVFT = null;
            foreach (ViewFamilyType curVFT in collector1)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    floorplanVFT = curVFT;
                    break;
                }
            }

            //Find the ceilingplan view type
            ViewFamilyType ceilingPlanVFT = null;
            foreach (ViewFamilyType curVFT in collector1)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    ceilingPlanVFT = curVFT;
                    break;
                }
            }


            //loop through the levels
            for (int i = 1; i <= number; i++)
            {
                double by3 = i % 3;
                double by5 = i % 5;

                Transaction t = new Transaction(doc);
                t.Start("Create new level");
                Level newLevel = Level.Create(doc, elevation);
                newLevel.Name = i.ToString();

                if (by3 == 0 && by5 == 0)
                {
                    // create a sheet called FIZZBUZZ_# 
                    ViewSheet newSheet = ViewSheet.Create(doc, tbId);
                    newSheet.Name = "FIZZBUZZ_" + i.ToString();
                    newSheet.SheetNumber = i.ToString();

                    //create a floorplan called FIZZBUZZ_#
                    ViewPlan newPlan = ViewPlan.Create(doc, floorplanVFT.Id, newLevel.Id);
                    newPlan.Name = "FIZZBUZZ_" + i.ToString();

                    // add a view to the sheet
                    XYZ insPoint = new XYZ(1, 0.5, 0);
                    Viewport newViewport = Viewport.Create(doc, newSheet.Id, newPlan.Id, insPoint);

                }

                else if (by3 == 0 && by5 > 0)
                {
                    //create a floorplan called FIZZ_#
                    ViewPlan newFPlan = ViewPlan.Create(doc, floorplanVFT.Id, newLevel.Id);
                    newFPlan.Name = "FIZZ_" + i.ToString();

                }
                else if (by5 == 0 && by3 > 0)
                {
                    //create a ceiling plan called BUZZ_#
                    ViewPlan newCPlan = ViewPlan.Create(doc, ceilingPlanVFT.Id, newLevel.Id);
                    newCPlan.Name = "BUZZ_" + i.ToString();
                }

                elevation = elevation + floorHeight;

                t.Commit();
                t.Dispose();
            
            }

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }
}
