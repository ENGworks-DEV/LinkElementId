using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Drawing;
using Color = System.Drawing.Color;
using System.Timers;

namespace LinkElementID
{

  

    public static class tools
    {
        public static Transaction Transaction { get; set; }
        private static string currentNumber;

        public static UIApplication uiapp { get; set; }

        public static UIDocument uidoc { get; set; }
        /// <summary>
        /// current document
        /// </summary>
        public static Document doc { get; set; }



        /// <summary>
        /// Copy and embedded resource to a directory(txt file in this case)
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="outDirectory"></param>
        /// <param name="internalFilePath"></param>
        /// <param name="resourceName"></param>
        private static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
                w.Write(r.ReadBytes((int)s.Length));
        }


        /// <summary>
        /// Create a project parameter
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="visible"></param>
        /// <param name="cats"></param>
        /// <param name="group"></param>
        /// <param name="inst"></param>
        public static void RawCreateProjectParameter(Autodesk.Revit.ApplicationServices.Application app, string name, ParameterType type, bool visible, CategorySet cats, BuiltInParameterGroup group, bool inst)
        {


            string oriFile = app.SharedParametersFilename;
            string tempFile = Path.GetTempFileName() + ".txt";
            using (File.Create(tempFile)) { }
            app.SharedParametersFilename = tempFile;

            ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(name, type);

            ExternalDefinition def = app.OpenSharedParameterFile().Groups.Create("TemporaryDefintionGroup").Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;

            app.SharedParametersFilename = oriFile;
            File.Delete(tempFile);

            Autodesk.Revit.DB.Binding binding = app.Create.NewTypeBinding(cats);
            if (inst) binding = app.Create.NewInstanceBinding(cats);

            BindingMap map = (new UIApplication(app)).ActiveUIDocument.Document.ParameterBindings;
            map.Insert(def, binding, group);
        }


        /// <summary>
        /// Categories used when creating shared parameters / project parameters
        /// </summary>
        /// <returns></returns>
        public static CategorySet CategorySetList()
        {
            CategorySet categorySet = new CategorySet();
            foreach (Category c in doc.Settings.Categories)
            {
                if (c.CategoryType == CategoryType.Model
                    && c.AllowsBoundParameters
                    )
                { categorySet.Insert(c); }

            }

            return categorySet;
        }

        public static List<Element> ListOfElements { get; set; } = new List<Element>();

        public static Element selectedElement { get; set; }

        public static List<Element> selectedElements { get; set; } = new List<Element>();


       

        public static Element NewSelection()
        {
            tools.selectedElements.Clear();
            tools.SelectionFilter filterS = new tools.SelectionFilter();
            try
            {
                Reference refElement = tools.uidoc.Selection.PickObject(ObjectType.LinkedElement, new tools.SelectionFilter());
                return tools.doc.GetElement(refElement.ElementId);
            }
            catch
            {
                return null;
            }
            
        }



        public static bool filterParam(Element elementEx, Element elementExP, BuiltInParameter param01, BuiltInParameter param02,
            BuiltInParameter param03, BuiltInParameter param04, BuiltInParameter param05, BuiltInParameter param06)
        {
            bool result = false;

            string elemParam06 = "n";

            string elemParam01 = elementEx.get_Parameter(param01).AsValueString();
            string elemParam02 = elementEx.get_Parameter(param02).AsValueString();
            string elemParam03 = elementEx.get_Parameter(param03).AsValueString();
            string elemParam04 = elementEx.get_Parameter(param04).AsValueString();
            string elemParam05 = elementEx.get_Parameter(param05).AsValueString();
            try
            { 
                elemParam06 = elementEx.get_Parameter(param06).AsValueString();
            }
            catch
            {

            }
            string elemParam06P = "";

            string elemParam01P = elementExP.get_Parameter(param01).AsValueString();
            string elemParam02P = elementExP.get_Parameter(param02).AsValueString();
            string elemParam03P = elementExP.get_Parameter(param03).AsValueString();
            string elemParam04P = elementExP.get_Parameter(param04).AsValueString();
            string elemParam05P = elementExP.get_Parameter(param05).AsValueString();
            try
            {
                 elemParam06P = elementExP.get_Parameter(param06).AsValueString();
            }
            catch
            {

            }


            if (elemParam01 == elemParam01P)
            {
                if (elemParam02 == elemParam02P)
                {
                    if (elemParam03 == elemParam03P)
                    {
                        if (elemParam04 == elemParam04P)
                        {
                            if (elemParam05 == elemParam05P)
                            {
                                if (elemParam06 == elemParam06P)
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Get number from element as string
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string getNumber(Element element)
        {
            currentNumber = "";
 
            if(element.get_Parameter(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER).AsString() != null)
              {
                 var value = element.get_Parameter(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER).AsString();
                 currentNumber = value;
              }             
            
            return currentNumber;
        }

        /// <summary>
        /// Creates a new number from a prefix, separator, a number and the amount of digits the number should have
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="separator">Separator string</param>
        /// <param name="number">Current number</param>
        /// <param name="chars">Amount of digits the number should have</param>
        /// <returns></returns>
        public static string createNumbering(string prefix, string separator, int number, int chars)
        {

            var num = number.ToString().PadLeft(chars, '0');
            return prefix + separator + num;

        }

        public static string ActualCounter { get; set; }

        public static int count { get; set; }

        public class SelectionFilter : ISelectionFilter
        {
            #region ISelectionFilter Members

            public bool AllowElement(Element elem)
            {
                if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_FabricationDuctwork) return true;

                return false;
            }

            public bool AllowReference(Reference refer, XYZ pos)
            {
                return false;
            }

            #endregion
        }

        /// <summary>
        /// Resets override on all elements in current view
        /// </summary>
        public static void resetView()
        {
            using (Transaction ResetView = new Transaction(tools.uidoc.Document, "Reset view"))
            {
                ResetView.Start();
                OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();

                LogicalOrFilter logicalOrFilter = new LogicalOrFilter(filters());

                var collector = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(
                    logicalOrFilter).WhereElementIsNotElementType();

                foreach (var item in collector.ToElements())
                {
                    if (item.IsValidObject && doc.GetElement(item.Id) != null)
                    {
                        doc.ActiveView.SetElementOverrides(item.Id, overrideGraphicSettings);
                    }

                }

                ResetView.Commit();
            }
        }


        /// <summary>
        /// Reset prefix values on all elements visible on current view
        /// </summary>
        public static void resetValues()
        {
            List<BuiltInCategory> allBuiltinCategories = FabricationCategories();

            LogicalOrFilter logicalOrFilter = new LogicalOrFilter(filters());

            var collector = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(
                logicalOrFilter).WhereElementIsNotElementType();
            using (Transaction ResetView = new Transaction(tools.uidoc.Document, "Reset view"))
            {
                ResetView.Start();
                OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
                Guid guid = new Guid("460e0a79-a970-4b03-95f1-ac395c070beb");
                string blankPrtnmbr = "";
                foreach (var item in collector.ToElements())
                {
                    item.get_Parameter(guid).Set(blankPrtnmbr);
                    Category category = item.Category;
                    BuiltInCategory enumCategory = (BuiltInCategory)category.Id.IntegerValue;
                    if (allBuiltinCategories.Contains(enumCategory))
                    {
                        item.get_Parameter(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER).Set(blankPrtnmbr);
                    }
                }

                ResetView.Commit();

            }
        }

        /// <summary>
        /// Write configuration file saved on user document 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="number"></param>
        public static void writeConfig(string prefix, string separator, string number)
        {

            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\renumberConf";

            using (StreamWriter st = new StreamWriter(path))
            {
                st.WriteLine(prefix);
                st.WriteLine(separator);
                st.WriteLine(number);

            }
        }


        /// <summary>
        /// Read configuration file saved on user document 
        /// </summary>
        /// <returns></returns>
        public static List<string> readConfig()
        {
            var output = new List<string>();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\renumberConf";
            if (File.Exists(path))
            {
                using (StreamReader st = new StreamReader(path))
                {
                    int counter = 0;
                    string line;
                    while ((line = st.ReadLine()) != null)
                    {
                        output.Add(line);
                        counter++;
                    }


                }
            }
            return output;
        }


        /// <summary>
        /// Assing number to element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="partNumber"></param>
        public static void AssingPartNumber(Element element, string partNumber)
        {

            Category category = element.Category;
            BuiltInCategory enumCategory = (BuiltInCategory)category.Id.IntegerValue;
            List<BuiltInCategory> allBuiltinCategories = FabricationCategories();

            if (allBuiltinCategories.Contains(enumCategory))
            {
                //try
                //{
                    element.get_Parameter(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER).Set(partNumber);
                    ListOfElements.Add(element);
                    ActualCounter = partNumber;
                    //uidoc.RefreshActiveView();
                    //uidoc.Selection.SetElementIds(new List<ElementId>() { element.Id });
                //}
                //catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            //else
            //{
            //    Guid guid = new Guid("460e0a79-a970-4b03-95f1-ac395c070beb");
            //    element.get_Parameter(guid).Set(partNumber);
            //    ListOfElements.Add(element);
            //    uidoc.Selection.SetElementIds(new List<ElementId>() { element.Id });
            //}

        }

        /// <summary>
        /// Hard coded list of categories to be used as a filter, only pipes, ducts and FabParts should be included when selecting and filter
        /// </summary>
        /// <returns></returns>
        public static List<ElementFilter> filters()
        {
            var output = new List<ElementFilter>();
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_FabricationDuctwork));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_FabricationPipework));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_FabricationContainment));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_PipeAccessory));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_PipeFitting));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_DuctAccessory));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_DuctFitting));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_Conduit));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_ConduitFitting));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_CableTray));
            output.Add(new ElementCategoryFilter(BuiltInCategory.OST_CableTrayFitting));
            return output;
        }

        /// <summary>
        /// Hard coded list of categories to be used as a filter on selection, only pipes, ducts and FabParts should be included when selecting and filter
        /// </summary>
        /// <returns></returns>
        public static List<BuiltInCategory> SelectionFilters()
        {
            var output = new List<BuiltInCategory>();
            output.Add((BuiltInCategory.OST_FabricationDuctwork));
            output.Add(BuiltInCategory.OST_FabricationPipework);
            output.Add(BuiltInCategory.OST_FabricationContainment);
            output.Add((BuiltInCategory.OST_PipeAccessory));
            output.Add((BuiltInCategory.OST_PipeCurves));
            output.Add((BuiltInCategory.OST_PipeFitting));
            output.Add((BuiltInCategory.OST_DuctAccessory));
            output.Add((BuiltInCategory.OST_DuctCurves));
            output.Add((BuiltInCategory.OST_DuctFitting));
            output.Add((BuiltInCategory.OST_Conduit));
            output.Add((BuiltInCategory.OST_ConduitFitting));
            output.Add((BuiltInCategory.OST_CableTray));
            output.Add((BuiltInCategory.OST_CableTrayFitting));
            return output;
        }

        /// <summary>
        /// Hard coded list of Fabricationparts categories to be used as a filter between fabrication and non fabrication parts
        /// </summary>
        /// <returns></returns>
        public static List<BuiltInCategory> FabricationCategories()
        {
            var output = new List<BuiltInCategory>();
            output.Add((BuiltInCategory.OST_FabricationDuctwork));
            output.Add(BuiltInCategory.OST_FabricationPipework);
            output.Add(BuiltInCategory.OST_FabricationContainment);
            return output;
        }



        /// <summary>
        /// Get number and prefix from string using the separator
        /// </summary>
        /// <param name="CNumber"></param>
        /// <returns></returns>
        
    }
}