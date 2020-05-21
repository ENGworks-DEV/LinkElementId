using Autodesk.Revit.DB;

namespace LinkElementID.Core
{
    class LinkSystem
    {
        public static string get(Document _doc, Reference refElemLinked)
        {
            string result = "";
            RevitLinkInstance elem = _doc.GetElement(refElemLinked.ElementId) as RevitLinkInstance;
            Document docLinked = elem.GetLinkDocument();
            Element linkedelement = docLinked.GetElement(refElemLinked.LinkedElementId);
            Parameter abbreviationParam = linkedelement.LookupParameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM.ToString());
            if (abbreviationParam != null)
            {
                result = abbreviationParam.AsValueString();//RBS_SYSTEM_ABBREVIATION_PARAM
            }
            return result;         
        }
    }
}
