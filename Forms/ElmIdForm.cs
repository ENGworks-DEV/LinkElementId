using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Forms;
using System.IO;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace LinkElementID
{
    /// <summary>
    /// Interaction logic for ManInputForm.xaml
    /// </summary>
    public partial class ElmIdForm : Window
    {
        private ExternalCommandData p_commanddata;
        public Document _doc;
        public UIDocument uidoc;
        private const string Caption = "Interval elapsed.  Continue running?";
        private static readonly Timer _Timer = new Timer();


        public ElmIdForm(ExternalCommandData cmddata_p, Document IntDocument)
        {

            p_commanddata = cmddata_p;
            UIApplication uiapp = cmddata_p.Application;
            uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            _doc = IntDocument;

            InitializeComponent();



        }

        /// <summary>
        /// Button close form when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GetIdLinkElm(object sender, RoutedEventArgs el)
        {
            List<Element> RunElements = new List<Element>();
            List<ElementId> RunElementsId = new List<ElementId>();

            ObjectType obt = ObjectType.LinkedElement;
            Reference refElemLinked;
            Element linkedelement = null;

            base.Hide();

            try
            {
                refElemLinked = uidoc.Selection.PickObject(obt, "Please pick an element in the linked model");
            }
            catch
            {
                refElemLinked = null;
            }

            

            if(refElemLinked != null)
            {
            RevitLinkInstance elem = _doc.GetElement(refElemLinked.ElementId) as RevitLinkInstance;
            Document docLinked = elem.GetLinkDocument();

            linkedelement = docLinked.GetElement(refElemLinked.LinkedElementId);

                IdBox.Text = linkedelement.Id.ToString();
                NameBox.Text = docLinked.Title.ToString();
                CategoryBox.Text = linkedelement.Category.Name.ToString();

                Parameter abbreviationParam = linkedelement.LookupParameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM.ToString());
                if (abbreviationParam != null)
                    AbbreviationBox.Text = abbreviationParam.AsValueString();//RBS_SYSTEM_ABBREVIATION_PARAM
            }

            base.ShowDialog();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(IdBox.Text);
        }
    }

}
