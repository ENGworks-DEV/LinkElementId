using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;




namespace LinkElementID
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string ExecutingAssemblyPath = Assembly.GetExecutingAssembly().Location;
            RibbonPanel m_projectPanel = application.CreateRibbonPanel("LinkElementID");
            PushButton pushButton = m_projectPanel.AddItem(new PushButtonData("LinkElementID",
                "LinkElementID", ExecutingAssemblyPath,
                "LinkElementID.CommandLink")) as PushButton;
            pushButton.ToolTip = "LinkElementID";
            pushButton.LongDescription = "This addin allows you to get the ID of a linked element";
            pushButton.LargeImage = PngImageSource("Resources.LinkElementIDLogo.png");
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string newpath = Path.GetFullPath(Path.Combine(path, "..\\"));
            ContextualHelp contextHelp = new ContextualHelp(ContextualHelpType.Url, "https://engworks.com/LinkElementID/");
            pushButton.SetContextualHelp(contextHelp);
            return Result.Succeeded;
        }

        private ImageSource PngImageSource(string embeddedPath)
        {
            Stream stream = base.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class CommandLink : IExternalCommand
    {
        Application _app;
        // Declare Need variables.
        //
        private ExternalCommandData cre_cmddata;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // objects for the top level access
            //
            this.cre_cmddata = commandData;
            UIApplication uiApp = cre_cmddata.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            _app = uiApp.Application;
            UIDocument ui_doc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = ui_doc.Document;

            ElmIdForm homewin = new ElmIdForm(commandData, doc);
            homewin.ShowDialog();
            return Result.Succeeded;
        }
    }
}
