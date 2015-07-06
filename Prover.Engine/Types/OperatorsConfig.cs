using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Engine.Types
{
    public class OperatorsConfig
    {
        public OperatorConfig Negation { get; set; }

        public OperatorConfig Conjunction { get; set; }

        public OperatorConfig Disjunction { get; set; }

        public OperatorConfig NegatedConjunction { get; set; }

        public OperatorConfig NegatedDisjunction { get; set; }

        public OperatorConfig Implication { get; set; }

        public OperatorConfig Equivalence { get; set; }

        public OperatorConfig Always { get; set; }

        public OperatorConfig Sometime { get; set; }

        public OperatorConfig ExclusiveOr { get; set; }

        public OperatorsConfig(OperatorsConfig config)
        {
            SetProperties(config);
        }

        private void SetProperties(OperatorsConfig config)
        {
            Negation = new OperatorConfig(config.Negation);
            Conjunction = new OperatorConfig(config.Conjunction);
            Disjunction = new OperatorConfig(config.Disjunction);
            NegatedConjunction = new OperatorConfig(config.NegatedConjunction);
            NegatedDisjunction = new OperatorConfig(config.NegatedDisjunction);
            Implication = new OperatorConfig(config.Implication);
            Equivalence = new OperatorConfig(config.Equivalence);
            Always = new OperatorConfig(config.Always);
            Sometime = new OperatorConfig(config.Sometime);
            ExclusiveOr = new OperatorConfig(config.ExclusiveOr);
        }

        public OperatorsConfig()
        {
            Negation = new OperatorConfig();
            Conjunction = new OperatorConfig();
            Disjunction = new OperatorConfig();
            NegatedConjunction = new OperatorConfig();
            NegatedDisjunction = new OperatorConfig();
            Implication = new OperatorConfig();
            Equivalence = new OperatorConfig();
            Always = new OperatorConfig();
            Sometime = new OperatorConfig();
            ExclusiveOr = new OperatorConfig();
        }
    }
}
