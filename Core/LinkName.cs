using Autodesk.Revit.DB;

namespace LinkElementID.Core
{
    public static class LinkName
    {
        public static string get(Document _doc, Reference refElemLinked)
        {
            string result = "";
            RevitLinkInstance elem = _doc.GetElement(refElemLinked.ElementId) as RevitLinkInstance;
            Document docLinked = elem.GetLinkDocument();
            result = docLinked.Title.ToString();
            return result;
        }
    }
}
