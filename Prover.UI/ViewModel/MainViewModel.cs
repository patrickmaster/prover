using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphSharp.Controls;
using Prover.Engine.Decomposition;
using Prover.Engine.Parser;
using Prover.Engine.Types.Decomposition;
using Prover.Engine.Types.Expression;
using Prover.UI.Observable;
using QuickGraph;

namespace Prover.UI.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private readonly GraphLayout _graphControl;
        public readonly ObservableValue<string> Formula = new ObservableValue<string>();

        private readonly IAlgorithm _algorithm = new SimpleAlgorithm();

        private readonly IParser _parser = new MockParser();

        private BidirectionalGraph<object, IEdge<object>> _treeGraph =
            new BidirectionalGraph<object, IEdge<object>>();
        
        public MainViewModel(GraphLayout graphControl)
        {
            _graphControl = graphControl;
            _graphControl.DataContext = _treeGraph;
        }

        public void SolveInline()
        {
            IExpression expression = _parser.Parse(Formula.Value);
            SolveExpression(expression);
        }

        private void SolveExpression(IExpression expression)
        {
            AlgorithmResult result = _algorithm.Solve(expression);

            ClearGraph();

            foreach (INode node in result.Nodes)
            {
                _treeGraph.AddVertex(node);
            }

            foreach (IConnection connection in result.Connections)
            {
                IEdge<object> edge = new Edge<object>(connection.StartNode, connection.EndNode);
                _treeGraph.AddEdge(edge);
            }
        }

        private void ClearGraph()
        {
            _graphControl.DataContext = _treeGraph = new BidirectionalGraph<object, IEdge<object>>();
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
    }
}
