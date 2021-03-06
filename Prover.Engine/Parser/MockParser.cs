﻿using System.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using Prover.Engine.Types.Expression;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Prover.Engine.Types;

namespace Prover.Engine.Parser
{
    public class MockParser : IParser
    {
        private readonly OperatorsConfig _operatorsConfig;
        private Hashtable precidence = new Hashtable();
        public List<Tok> calc_list = new List<Tok>();
        private Dictionary<string, string> data_store = new Dictionary<string, string>();

        private enum type { num = 0, var, op, neg };

        public MockParser(OperatorsConfig operatorsConfig)
        {
            _operatorsConfig = operatorsConfig;
            precidence.Add(operatorsConfig.Negation.Symbol, operatorsConfig.Negation.Priority); //zaprzeczenie
            precidence.Add(operatorsConfig.Conjunction.Symbol, operatorsConfig.Conjunction.Priority); //koniunkcja
            precidence.Add(operatorsConfig.Disjunction.Symbol, operatorsConfig.Disjunction.Priority); //alternatywa
            precidence.Add(operatorsConfig.ExclusiveOr.Symbol, operatorsConfig.ExclusiveOr.Priority); //alternatywa wykluczająca
            precidence.Add(operatorsConfig.Implication.Symbol, operatorsConfig.Implication.Priority); //implikacja
            precidence.Add(operatorsConfig.Equivalence.Symbol, operatorsConfig.Equivalence.Priority); //rownowaznosc
            precidence.Add(operatorsConfig.Always.Symbol, operatorsConfig.Always.Priority); //zawsze 
            precidence.Add(operatorsConfig.Sometime.Symbol, operatorsConfig.Sometime.Priority); //czasami
        }

        public IExpression Parse(string expression)
        {
            bool rtn = RPN(expression) == true ? true : false;
            return Compute();
            //return rtn == true ? "success" : "failed";
        }

        private IExpression Compute()
        {
            Stack<TokenExpr> compu_stack = new Stack<TokenExpr>();
            try
            {
                foreach (Tok tok in calc_list)
                {
                    TokenExpr res = new TokenExpr();

                    if (tok._class == (int)type.num || tok._class == (int)type.var)
                    {
                        Console.WriteLine(tok.repr);
                        TokenExpr tokexpr = new TokenExpr(tok.repr);
                        compu_stack.Push(tokexpr);
                    }
                    else if (tok._class == (int)type.op)
                    {
                        TokenExpr rhs = compu_stack.Pop();
                        
                        if (tok.repr == _operatorsConfig.Conjunction.Symbol)
                        {
                            res.repr = new Conjunction(compu_stack.Pop().repr, rhs.repr);
                        }
                        else if (tok.repr == _operatorsConfig.Disjunction.Symbol)
                        {
                            res.repr = new Disjunction(compu_stack.Pop().repr, rhs.repr);
                        }
                        else if (tok.repr == _operatorsConfig.Implication.Symbol)
                        {
                            res.repr = new Implication(compu_stack.Pop().repr, rhs.repr);
                        }
                        else if (tok.repr == _operatorsConfig.Equivalence.Symbol)
                        {
                            res.repr = new Equivalence(compu_stack.Pop().repr, rhs.repr);
                        }
                        else if (tok.repr == _operatorsConfig.ExclusiveOr.Symbol)
                        {
                            res.repr = new ExclusiveOr(compu_stack.Pop().repr, rhs.repr);
                        }
                        else if (tok.repr == _operatorsConfig.Always.Symbol)
                        {
                            res.repr = new Always(rhs.repr);
                        }
                        else if (tok.repr == _operatorsConfig.Sometime.Symbol)
                        {
                            res.repr = new Sometime(rhs.repr);
                        }
                        //else if (tok.repr == _operatorsConfig.Negation.Symbol)
                        //{
                        //    res.repr = new Negation(rhs.repr);
                        //}
                        else
                        {
                            calc_list.Clear();
                            MessageBox.Show("Nieznany operator");
                            return new Conjunction(new Literal("0"), new Literal("0"));
                        }

                        compu_stack.Push(res);
                    }
                    else if (tok._class == (int)type.neg)
                    {
                        TokenExpr expr = compu_stack.Pop();
                        res.repr = new Negation(expr.repr);
                        compu_stack.Push(res);

                    }
                }
                //return double.Parse(GetValue(compu_stack.Pop()));
                IExpression result = compu_stack.Pop().repr;
                calc_list.Clear();
                return result;
            }
            catch (Exception e)
            {
                calc_list.Clear();
                MessageBox.Show("Problem z analizą wejścia." + e);
                return new Conjunction(new Literal("0"), new Literal("0"));
            }
        }

        private string GetValue(Tok input)
        {
            if (input._class == (int)type.num)
            {
                return input.repr;
            }
            else
            {
                return data_store[input.repr].ToString();
            }
        }

        private bool RPN(string expression)
        {
            //Regex reg = new Regex(@"\[\b[A-Za-z]+\b(?![\d])\]|[\~\+\-/\*]|[\d]+(\.[\d]+)?|[()]");
            Regex reg = new Regex(@"[\&\|\>\=\+FG]|\b[A-Za-z]+|[()]");
            Stack<Tok> op_stack = new Stack<Tok>();

            try
            {
                foreach (Match token in reg.Matches(expression))
                {
                    Tok tok = new Tok();
                    string value = token.Captures[0].Value;
                    Console.WriteLine("foricz" + value);

                    //if (IsNumeric(value))
                    //{
                    //   tok._class = (int)type.num;
                    //   tok.repr = value;
                    //  calc_list.Add(tok);

                    //}
                    if (IsOperator(value))
                    {
                        Console.WriteLine("operator" + value);

                        while (op_stack.Count != 0 && op_stack.Peek().repr != "(")
                        {
                            if (HasPrecidenceOrEqual(op_stack.Peek().repr, value))
                            {
                                calc_list.Add(op_stack.Pop());
                            }
                            else break;
                        }
                        tok._class = (int)type.op;
                        tok.repr = value;

                        op_stack.Push(tok);
                    }
                    else if (IsVariable(value))
                    {
                        Console.WriteLine("variable" + value);

                        tok._class = (int)type.var;
                        tok.repr = value;
                        calc_list.Add(tok);

                    }
                    else if (IsNegation(value))
                    {
                        Console.WriteLine("variable" + value);

                        tok._class = (int)type.neg;
                        tok.repr = value;
                        calc_list.Add(tok);

                    }
                    else if (value == "(")
                    {
                        tok._class = (int)type.var;
                        tok.repr = value;

                        op_stack.Push(tok);
                    }
                    else if (value == ")")
                    {
                        while (op_stack.Count != 0 && op_stack.Peek().repr != "(")
                        {
                            calc_list.Add(op_stack.Pop());
                        }
                        if (op_stack.Count != 0) op_stack.Pop();
                    }
                }
            }
            catch (System.ArgumentNullException)
            {
                MessageBox.Show("Wejście jest puste lub zawiera niedozwolone znaki.");
                return false;
            }

            while (op_stack.Count != 0)
            {
                calc_list.Add(op_stack.Pop());
            }

            return true;
        }


        private bool IsOperator(string arg)
        {
            return new Regex(@"[\&\|\>\=\+FG]").Match(arg).Success;
        }

        private bool IsNegation(string arg)
        {
            return new Regex(@"[\~]").Match(arg).Success;
        }

        private bool IsVariable(string arg)
        {
            return new Regex(@"\b[A-Za-z]+").Match(arg).Success;
        }

        private bool HasPrecidenceOrEqual(string lhs, string rhs)
        {
            return ((int)precidence[lhs] >= (int)precidence[rhs]);
        }

        // from file
        public IExpression Parse(Stream stream)
        {
            return new Implication(
                new Implication(new Conjunction(new Literal("p"), new Negation(new Literal("r"))), new Literal("q")),
                new Disjunction(new Negation(new Literal("p")), new Literal("q")));
        }
    }

    public struct Tok
    {
        public int _class;
        public string repr;
    }

    public struct TokenExpr
    {
        public IExpression repr;
        
        public TokenExpr(string expr)
        {
            repr = new Literal(expr);
        }
    }
}