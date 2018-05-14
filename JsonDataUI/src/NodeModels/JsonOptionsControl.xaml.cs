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
using System.Windows.Shapes;

namespace JsonDataUI.Views
{
    /// <summary>
    /// Interaction logic for JsonOptionsControl.xaml
    /// </summary>
    public partial class JsonOptionsControl : UserControl
    {
        public JsonOptionsControl()
        {
            InitializeComponent();
            foreach(string option in Enum.GetNames(typeof(JsonData.JsonOption)))
            {
                cBox_JsonOptions.Items.Add(option);
            }
            cBox_JsonOptions.SelectedIndex = 0;
        }

        private void cBox_JsonOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
