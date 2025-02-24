using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System.Diagnostics;

namespace PipeSystemExporter
{
  class Util
  {
    /// <summary>
    /// Get the connector set of a given element
    /// </summary>
    /// <param name="e">the owner of the connector</param>
    /// <returns>if found, return all the connectors found, or else return null</returns>
    public static ConnectorSet GetConnectors(Element e)
    {
      if (e == null) return null;
      FamilyInstance fi = e as FamilyInstance;
      if (fi != null && fi.MEPModel != null)
      {
        return fi.MEPModel.ConnectorManager.Connectors;
      }
      MEPSystem mepsystem = e as MEPSystem;
      if (mepsystem != null)
      {
        return mepsystem.ConnectorManager.Connectors;
      }
      MEPCurve mepcurve = e as MEPCurve;
      if (mepcurve != null)
      {
        return mepcurve.ConnectorManager.Connectors;
      }
      return null;
    }

    /// <summary>
    /// Get a list of connector points of a given element
    /// </summary>
    public static List<XYZ> GetConnectorPoints(Element e)
    {
      ConnectorSet cons = GetConnectors(e);
      int n = cons.Size;
      List<XYZ> pts = new List<XYZ>(n);
      foreach (Connector con in cons)
      {
        pts.Add(con.Origin);
      }
      return pts;
    }

    /// <summary>
    ///     Return an English plural suffix for the given
    ///     number of items, i.e. 's' for zero or more
    ///     than one, and nothing for exactly one.
    /// </summary>
    public static string PluralSuffix(int n)
    {
      return 1 == n ? "" : "s";
    }

    /// <summary>
    ///     Return a dot (full stop) for zero
    ///     or a colon for more than zero.
    /// </summary>
    public static string DotOrColon(int n)
    {
      return 0 < n ? ":" : ".";
    }

    /// <summary>
    ///     Return a string for a real number
    ///     formatted to two decimal places.
    /// </summary>
    public static string RealString(double a)
    {
      return a.ToString("0.####");
    }

    /// <summary>
    ///     Return a string for an XYZ point
    ///     or vector with its coordinates
    ///     formatted to two decimal places.
    /// </summary>
    public static string PointString(
        XYZ p,
        bool onlySpaceSeparator = false)
    {
      var format_string = onlySpaceSeparator
          ? "{0} {1} {2}"
          : "({0},{1},{2})";

      return string.Format(format_string,
          RealString(p.X),
          RealString(p.Y),
          RealString(p.Z));
    }
  }
}
