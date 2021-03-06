﻿using Autodesk.Revit.DB;

namespace LinkElementID.Core
{
    public static class LinkCat
    {
        public static string get(Document _doc, Reference refElemLinked)
        {
            string result = "";
            RevitLinkInstance elem = _doc.GetElement(refElemLinked.ElementId) as RevitLinkInstance;
            Document docLinked = elem.GetLinkDocument();
            Element linkedelement = docLinked.GetElement(refElemLinked.LinkedElementId);
            result = linkedelement.Category.Name.ToString();
            return result;
        }
    }
}
