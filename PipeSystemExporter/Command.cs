#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

#endregion

namespace PipeSystemExporter
{
  [Transaction(TransactionMode.ReadOnly)]
  public class Command : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements)
    {
      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Document doc = uidoc.Document;

      FilteredElementCollector col
        = new FilteredElementCollector(doc)
          .WhereElementIsNotElementType()
          .OfCategory(BuiltInCategory.OST_PipeCurves)
          .OfClass(typeof(Pipe));

      int n = col.GetElementCount();
      Debug.Print($"{n} pipe{Util.PluralSuffix(n)}{Util.DotOrColon(n)}");
      
      foreach (Pipe pipe in col)
      {
        double diameter = pipe.Diameter;
        double inch = 25.4;
        double foot = 12 * inch;
        int diam_mm = Convert.ToInt32(diameter * foot);
        List<XYZ> pts = Util.GetConnectorPoints(pipe);
        Debug.Assert(2 == pts.Count, "expected two endpoints on pipe");
        Debug.Print($"  pipe '{pipe.Name}' {diam_mm}mm {Util.RealString(diameter)} {Util.PointString(pts[0])} {Util.PointString(pts[1])}");
      }

      col = new FilteredElementCollector(doc)
        .WhereElementIsNotElementType()
        .OfCategory(BuiltInCategory.OST_PipeFitting)
        .OfClass(typeof(FamilyInstance));

      n = col.GetElementCount();
      Debug.Print($"{n} fitting{Util.PluralSuffix(n)}{Util.DotOrColon(n)}");

      foreach (FamilyInstance fitting in col)
      {
        List<XYZ> pts = Util.GetConnectorPoints(fitting);
        n = pts.Count;
        Debug.Assert(1 == n || 2 == n || 3 == n, "expected one, two or three endpoints on fitting");
        string s = (1 == n) ? "plug" : ((2 == n) ? "elbow" : "tee");
        string t = string.Join(" ", pts.Select( p => Util.PointString(p)));
        Debug.Print($"  {s} '{fitting.Symbol.FamilyName}' '{fitting.Name}' {t}");
      }

      return Result.Succeeded;
    }
  }
}
