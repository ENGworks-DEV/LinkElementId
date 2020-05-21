using Autodesk.Revit.DB;

namespace LinkElementID.Core
{
    public static class LinkId
    {
        public static string Get(Document _doc, Reference refElemLinked)
        {
            string result = "";
            RevitLinkInstance elem = _doc.GetElement(refElemLinked.ElementId) as RevitLinkInstance;
            Document docLinked = elem.GetLinkDocument();
            Element linkedelement = docLinked.GetElement(refElemLinked.LinkedElementId);
            result = linkedelement.Id.ToString();
            return result;
        }
    }
}
