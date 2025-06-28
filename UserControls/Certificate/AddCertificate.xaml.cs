using Registers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Registers.UserControls
{
    /// <summary>
    /// Interaction logic for AddCertificate.xaml
    /// </summary>
    public partial class AddCertificate : System.Windows.Controls.UserControl
    {
        public AddCertificate()
        {
            InitializeComponent();
        }

        private void DateTimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Xceed.Wpf.Toolkit.DateTimePicker picker &&
                picker.Template.FindName("PART_TextBox", picker) is System.Windows.Controls.TextBox textBox)
            {
                textBox.TextAlignment = TextAlignment.Left;
            }
        }

        private void MyListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is CertificateFormViewModel vm)
            {
                vm.HeaderColumnWidth = e.NewSize.Width - 80; 
            }
        }

    }
}
