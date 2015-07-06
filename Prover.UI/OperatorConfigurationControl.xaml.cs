using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Prover.Engine.Types;
using Xceed.Wpf.Toolkit;

namespace Prover.UI
{
    /// <summary>
    /// Interaction logic for OperatorConfiguration.xaml
    /// </summary>
    public partial class OperatorConfigurationControl : UserControl
    {


        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(OperatorConfigurationControl), new PropertyMetadata(""));

        Regex _operatorRegex = new Regex(@"^[!@#$%^&*-+=|<>/*~]*$");

        public OperatorConfig Configuration
        {
            get { return (OperatorConfig)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("Configuration", typeof(OperatorConfig), typeof(OperatorConfigurationControl), new PropertyMetadata(null));
        
        public OperatorConfigurationControl()
        {
            InitializeComponent();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!_operatorRegex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }
    }
}
