﻿using System;
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
using LinkElementID.Core;

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

        public string projectVersion = CommonAssemblyInfo.Number;
        public string ProjectVersion
        {
            get { return projectVersion; }
            set { projectVersion = value; }
        }

        public ElmIdForm(ExternalCommandData cmddata_p, Document IntDocument)
        {

            p_commanddata = cmddata_p;
            UIApplication uiapp = cmddata_p.Application;
            uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            _doc = IntDocument;
            InitializeComponent();
            this.DataContext = this;
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

            IdBox.Text = LinkId.Get(_doc, refElemLinked);
            NameBox.Text = LinkName.get(_doc, refElemLinked);
            CategoryBox.Text = LinkCat.get(_doc, refElemLinked);
            AbbreviationBox.Text = LinkSystem.get(_doc, refElemLinked);                   
            }

            base.ShowDialog();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(IdBox.Text);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Title_Link(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://engworks.com/LinkElmInfo/");
        }
    }

}
