using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Win32;
using Prover.Engine.Types;
using Prover.Engine.Types.Decomposition;
using Prover.UI.ViewModel;

namespace Prover.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel(Graph, this);
            DataContext = _viewModel;
            CbAlgorithmType.SelectedIndex = 0;
        }

        private void SolveCancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _viewModel != null && !string.IsNullOrEmpty(_viewModel.Formula.Value);
        }

        private void SolveCancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.SolveCancel();
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                _viewModel.OpenFormulaFromFile(dialog.FileName);
            }
        }

        private void OperatorsConfiguration_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.OpenOperatorsConfigWindow();
        }

        private void INode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Debug.WriteLine("node clicked");

            INode node = ((StackPanel)sender).DataContext as INode;

            if (node != null)
            {
                _viewModel.SetContext(node);
            }
        }

        private void Close_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void INode_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void LoadNodeChildren_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.LoadChildren(e.Parameter as INode);
        }

        private void LoadNodeNeighbours_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !_viewModel.IsFullGraphRendered;
        }

        private void LoadNodeAncestors_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.LoadAncestors(e.Parameter as INode);
        }

        private void LoadRoot_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.LoadRoot();
        }

        private void LoadRoot_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !_viewModel.IsFullGraphRendered && !_viewModel.IsRootRendered;
        }
    }
}
