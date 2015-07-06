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
using System.Windows.Shapes;
using Microsoft.Win32;
using Prover.Engine.Types;
using Prover.UI.ViewModel;

namespace Prover.UI
{
    /// <summary>
    /// Interaction logic for OperatorSettings.xaml
    /// </summary>
    public partial class OperatorSettingsWindow : Window
    {
        internal readonly SettingsViewModel ViewModel;

        public OperatorSettingsWindow(OperatorsConfig config)
        {
            InitializeComponent();
            DataContext = ViewModel = new SettingsViewModel(config); 
        }

        private void Confirm_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public static OperatorsConfig GetConfig(OperatorsConfig config)
        {
            OperatorSettingsWindow window = new OperatorSettingsWindow(config);

            if (window.ShowDialog() == true)
            {
                return window.ViewModel.Config;
            }
            else
            {
                return null;
            }
        }

        private void Open_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Pliki konfiguracyjne XML|*.xml";
            
            if (dialog.ShowDialog() == true)
            {
                ViewModel.LoadFromFile(dialog.FileName);
            }
        }
        
        private void Save_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Pliki konfiguracyjne XML|*.xml";

            if (dialog.ShowDialog() == true)
            {
                ViewModel.SaveToFile(dialog.FileName);
            }
        }

        private void Confirm_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =
                ViewModel != null && 
                !string.IsNullOrEmpty(ViewModel.Config.Negation.Symbol) &&
                !string.IsNullOrEmpty(ViewModel.Config.Conjunction.Symbol) &&
                !string.IsNullOrEmpty(ViewModel.Config.Disjunction.Symbol) &&
                !string.IsNullOrEmpty(ViewModel.Config.Equivalence.Symbol) &&
                !string.IsNullOrEmpty(ViewModel.Config.ExclusiveOr.Symbol) &&
                !string.IsNullOrEmpty(ViewModel.Config.Implication.Symbol) &&
                !string.IsNullOrEmpty(ViewModel.Config.NegatedConjunction.Symbol) &&
                !string.IsNullOrEmpty(ViewModel.Config.NegatedDisjunction.Symbol) &&
                !string.IsNullOrEmpty(ViewModel.Config.Sometime.Symbol) &&
                !string.IsNullOrEmpty(ViewModel.Config.Always.Symbol);
        }
    }
}
