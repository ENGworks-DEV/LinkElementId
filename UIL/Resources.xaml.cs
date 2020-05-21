using System;
using System.Windows;


namespace LinkElementID
{
    public partial class Resources : ResourceDictionary
    {

        private void EngworksLink(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/document/d/1KpfvO6rYD7nN1HMmemU-iUHEzY4denRZfHgL_N6JVYA/edit?usp=sharing");
        }

        private void AddInLink(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://facebook.engworks.com/#/");
        }
    }
}
