using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using Prover.Engine.Decomposition;
using Prover.Engine.Parser;
using Prover.Engine.Types;
using Prover.Engine.Types.Decomposition;
using Prover.Engine.Types.Expression;
using Prover.UI.Graph;
using Prover.UI.Observable;

namespace Prover.UI.ViewModel
{
    class MainViewModel : ObservableBase
    {
        private readonly ProverGraphLayout _graphControl;
        private readonly MainWindow _window;

        readonly ObservableValue<string> _formula = new ObservableValue<string>();

        public ObservableValue<string> Formula { get { return _formula; } }

        private IAlgorithm Algorithm
        {
            get
            {
                return _algorithms.ContainsKey(AlgorithmType) ? _algorithms[AlgorithmType] : _algorithms.Values.First();
            }
        }

        private readonly Dictionary<AlgorithmType, IAlgorithm> _algorithms = new Dictionary<AlgorithmType, IAlgorithm>
        {
            {AlgorithmType.SimpleContradiction, AlgorithmFactory.GetAlgorithm(AlgorithmType.SimpleContradiction)},
            {AlgorithmType.OptimizedContradiction, AlgorithmFactory.GetAlgorithm(AlgorithmType.OptimizedContradiction)},
            {AlgorithmType.Optimized, AlgorithmFactory.GetAlgorithm(AlgorithmType.Optimized)}
        };

        protected AlgorithmType AlgorithmType
        {
            get { return (AlgorithmType) _window.CbAlgorithmType.SelectedIndex; }
        }

        public long NodesCount
        {
            get { return _nodesCount; }
            set
            {
                if (value == _nodesCount) return;
                _nodesCount = value;
                OnPropertyChanged();
            }
        }

        public bool IsFullGraphRendered { get { return NodesCount <= _nodesLimit; } }

        public bool? IsTautology
        {
            get { return _isTautology; }
            set
            {
                if (value == _isTautology) return;
                _isTautology = value;
                OnPropertyChanged();
            }
        }

        public bool? IsTrueable
        {
            get { return _isTrueable; }
            set
            {
                if (value == _isTrueable) return;
                _isTrueable = value;
                OnPropertyChanged();
            }
        }

        public bool IsInProgress
        {
            get { return _isInProgress; }
            set
            {
                if (value == _isInProgress) return;
                _isInProgress = value;
                OnPropertyChanged();
            }
        }

        private IParser _parser;

        private ProverGraph _treeGraph = new ProverGraph();

        private OperatorsConfig _operatorsConfig;

        readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(OperatorsConfig));
        readonly string _operatorsConfigPath = "operator-settings.xml";
        private long _solvingTime;
        private long _nodesCount;
        private bool? _isTautology;
        private readonly ObservableCollection<string> _contextExpressions = new ObservableCollection<string>();

        private readonly int _nodesLimit = 200;
        private INode _rootNode;

        public long SolvingTime
        {
            get { return _solvingTime; }
            set
            {
                if (value == _solvingTime) return;
                _solvingTime = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(ProverGraphLayout graphControl, MainWindow window)
        {
            LoadConfig();
            _graphControl = graphControl;
            _window = window;
            _graphControl.DataContext = _treeGraph;
            _parser = new MultipleCharsParser(_operatorsConfig);
        }

        void LoadConfig()
        {
            try
            {
                using (FileStream stream = new FileStream(_operatorsConfigPath, FileMode.Open))
                {
                    _operatorsConfig = (OperatorsConfig)_xmlSerializer.Deserialize(stream);
                }
            }
            catch
            {
                _operatorsConfig = new OperatorsConfig
                {
                    Negation = new OperatorConfig("~", 1),
                    Conjunction = new OperatorConfig("&", 2),
                    Disjunction = new OperatorConfig("|", 2),
                    ExclusiveOr = new OperatorConfig("+", 2),
                    Implication = new OperatorConfig(">", 3),
                    Equivalence = new OperatorConfig("=", 3),
                    Always = new OperatorConfig("!", 2),
                    Sometime = new OperatorConfig("?", 2),
                    NegatedConjunction = new OperatorConfig("~&", 2),
                    NegatedDisjunction = new OperatorConfig("~|", 2)
                };
            }
        }

        public void SolveCancel()
        {
            if (IsInProgress)
            {
                _cancellationTokenSource.Cancel();
            }
            else
            {
                try
                {
                    IExpression expression = _parser.Parse(Formula.Value);
                    SolveExpression(expression);
                }
                catch (ParserException exception)
                {
                    MessageBox.Show(
                        string.Format("Wystąpił błąd podczas parsowania\nZnak: {0}\nWiadomość: {1}",
                            exception.CharacterNumber, exception.Message), "Błąd", MessageBoxButton.OK);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Nieoczekiwany błąd podczas rozwiązywania\n\n" + exception, "Błąd",
                        MessageBoxButton.OK);
                }
            }
        }

        private void SolveExpression(IExpression expression)
        {
            Stopwatch watch = new Stopwatch();
            IsInProgress = true;

            IAlgorithm algorithm = Algorithm;

            _cancellationTokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                watch.Start();
                AlgorithmResult result = algorithm.Solve(expression, _cancellationTokenSource.Token);
                watch.Stop();
                return result;
            }, _cancellationTokenSource.Token).ContinueWith(task =>
            {
                AlgorithmResult result = task.Result;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {

                    IsInProgress = false; 
                    SolvingTime = watch.ElapsedMilliseconds;
                    NodesCount = result.Nodes.Count();
                    IsTautology = result.IsTautology;
                    IsTrueable = result.IsTrueable;

                    SetContext(null);

                    RedrawGraph(result);

                    if (result.Nodes.Count() >= _nodesLimit)
                    {
                        MessageBox.Show(
                            string.Format("Liczba węzłów w grafie przekracza {0}, narysowany został częściowy graf", _nodesLimit),
                            "Uwaga", MessageBoxButton.OK);
                    }
                }));
            }, _cancellationTokenSource.Token).ContinueWith(task =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    IsInProgress = false; 
                }));
            });
        }

        private void RedrawGraph(AlgorithmResult result)
        {
            _currentRootNode = _rootNode = result.Nodes.First();
            _renderedStack.Clear();
            DrawGraphDescendantsFromNode(_rootNode);
        }

        private void DrawGraphDescendantsFromNode(INode root)
        {
            int i = 0;
            List<IConnection> connections = new List<IConnection>();
            List<INode> nodes = new List<INode>();
            Queue<INode> nodesQueue = new Queue<INode>();

            nodesQueue.Enqueue(root);
            nodes.Add(root);

            ClearGraph();
            while (nodesQueue.Any() && i < _nodesLimit)
            {
                INode node = nodesQueue.Dequeue();

                foreach (IConnection childConnection in node.Children)
                {
                    nodes.Add(childConnection.EndNode);
                    nodesQueue.Enqueue(childConnection.EndNode);
                    connections.Add(childConnection);
                    i++;
                }
            }

            foreach (INode node in nodes)
            {
                _treeGraph.AddVertex(node);
            }

            foreach (IConnection connection in connections)
            {
                ProverEdge edge = new ProverEdge(connection.StartNode, connection.EndNode);
                _treeGraph.AddEdge(edge);
            }
        }

        private void ClearGraph()
        {
            _graphControl.DataContext = _treeGraph = new ProverGraph();
        }

        public void OpenFormulaFromFile(string fileName)
        {
            IExpression expression;

            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                expression = _parser.Parse(stream);
            }

            SolveExpression(expression);
        }

        public void OpenOperatorsConfigWindow()
        {
            OperatorsConfig config = OperatorSettingsWindow.GetConfig(_operatorsConfig);

            if (config != null)
            {
                _operatorsConfig = config;
                _parser = new MultipleCharsParser(_operatorsConfig);
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            using (FileStream stream = new FileStream(_operatorsConfigPath, FileMode.Create))
            {
                _xmlSerializer.Serialize(stream, _operatorsConfig);
            }
        }

        public ObservableCollection<string> ContextExpressions
        {
            get { return _contextExpressions; }
        }

        public bool IsRootRendered { get { return _rootNode != null && !_renderedStack.Any(); } }

        public void SetContext(INode node)
        {
            ContextExpressions.Clear();

            if (node != null)
            {
                foreach (string expression in node.GetAllExpressions().Select(x => x.ToString()))
                {
                    ContextExpressions.Add(expression);
                }
            }
        }

        public void LoadChildren(INode node)
        {
            _renderedStack.Push(_currentRootNode);
            _currentRootNode = node;
            DrawGraphDescendantsFromNode(node);
        }

        public void LoadAncestors(INode node)
        {
            // unused
            throw new NotImplementedException();
        }

        readonly Stack<INode> _renderedStack = new Stack<INode>();
        private INode _currentRootNode;
        private bool _isInProgress;
        private CancellationTokenSource _cancellationTokenSource;
        private bool? _isTrueable;

        public void LoadRoot()
        {
            if (_rootNode != null)
            {
                _currentRootNode = _renderedStack.Pop();
                DrawGraphDescendantsFromNode(_currentRootNode);
            }
        }
    }
}
