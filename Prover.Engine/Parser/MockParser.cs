using System.IO;
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

namespace Prover.Engine.Parser
{
    public class MockParser : IParser
    {
        private Hashtable precidence = new Hashtable();
        public  List<Token> calc_list = new List<Token>();
        private Dictionary<string, string> data_store = new Dictionary<string, string>();
        
        private enum type { num = 0, var, op, neg };

        public MockParser()
        {        
            precidence.Add("~", 1); //zaprzeczenie
            precidence.Add("&", 2); //koniunkcja
            precidence.Add("|", 2); //alternatywa
            precidence.Add("+", 2); //alternatywa wykluczająca
            precidence.Add(">", 3); //implikacja
            precidence.Add("=", 3); //rownowaznosc
            precidence.Add("!", 2); //zawsze 
            precidence.Add("?", 2); //czasami
        }

        public string Parse(string expression){
            bool rtn = RPN(expression) == true ? true : false;
            return rtn == true ? "success" : "failed";
        }
        // from text
        //public IExpression Parse(string expression)
        //{
        //    expression = "dupa";
        //    // we expect IExpression object to be returned

        //    return new Implication(
        //        new Implication(new Literal("p"), new Literal("q")),
        //        new Disjunction(new Negation(new Literal("p")), new Literal("q")));
        //}

        public IExpression Compute(){
            Stack<TokenExpr> compu_stack = new Stack<TokenExpr>();
            try{
                foreach(Token tok in calc_list){
                    TokenExpr res = new TokenExpr();
                    
                    if(tok._class == (int)type.num || tok._class == (int)type.var){
                        Console.WriteLine(tok.repr);
                        TokenExpr tokexpr = new TokenExpr(tok._class, tok.repr);
                        compu_stack.Push(tokexpr);
                    }
                    else if(tok._class == (int)type.op){

                        TokenExpr rhs = compu_stack.Pop();
                        TokenExpr lhs = compu_stack.Pop();

                        switch (tok.repr)
                        {
                            case "&":
                                //res.repr = (double.Parse(GetValue(lhs)) + double.Parse(GetValue(rhs))).ToString();
                                //compu_stack.Push(res);
                                res.repr = new Conjunction(lhs.repr, rhs.repr);
                                compu_stack.Push(res);
                                break;
                            case "|":
                                //res.repr = (double.Parse(GetValue(lhs)) + double.Parse(GetValue(rhs))).ToString();
                                //compu_stack.Push(res);
                                res.repr = new Disjunction(lhs.repr, rhs.repr);
                                compu_stack.Push(res);
                                break;
                            case ">":
                                //res.repr = (double.Parse(GetValue(lhs)) + double.Parse(GetValue(rhs))).ToString();
                                //compu_stack.Push(res);
                                res.repr = new Implication(lhs.repr, rhs.repr);
                                compu_stack.Push(res);
                                break;
                            case "=":
                                //res.repr = (double.Parse(GetValue(lhs)) + double.Parse(GetValue(rhs))).ToString();
                                //compu_stack.Push(res);
                                res.repr = new Equivalence(lhs.repr, rhs.repr);
                                compu_stack.Push(res);
                                break;
                            case "+":
                                //res.repr = (double.Parse(GetValue(lhs)) + double.Parse(GetValue(rhs))).ToString();
                                //compu_stack.Push(res);
                                res.repr = new ExclusiveOr(lhs.repr, rhs.repr);
                                compu_stack.Push(res);
                                break;
                            default:
                                //return double.Parse(compu_stack.Pop().repr);
                                //compu_stack.Pop();
                                calc_list.Clear();
                                MessageBox.Show("Nieznany operator");
                                return new Conjunction(new Literal("0"), new Literal("0"));
                        }
                    }
                    else if (tok._class == (int)type.neg){
                        TokenExpr expr = compu_stack.Pop();
                        res.repr = new Negation(expr.repr);
                        compu_stack.Push(res);

                    }
                }                     
                //return double.Parse(GetValue(compu_stack.Pop()));
                IExpression result = compu_stack.Pop().repr;
                calc_list.Clear();
                return result;
            }catch(Exception e){
                calc_list.Clear();
                MessageBox.Show("Problem z analizą wejścia." + e);
                return new Conjunction(new Literal("0"), new Literal("0"));
            }
        }

        private string GetValue(Token input){
            if(input._class == (int)type.num){
                return input.repr;
            }else{
                return  data_store[input.repr].ToString();
            }
        }

        private bool RPN(string expression){
            //Regex reg = new Regex(@"\[\b[A-Za-z]+\b(?![\d])\]|[\~\+\-/\*]|[\d]+(\.[\d]+)?|[()]");
            Regex reg = new Regex(@"[\&\|\>\=\+FG]|\b[A-Za-z]+|[()]");
            Stack<Token> op_stack = new Stack<Token>();

            try
            {
                foreach (Match token in reg.Matches(expression))
                {
                    Token tok = new Token();
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
            catch (System.ArgumentNullException){
                MessageBox.Show("Wejście jest puste lub zawiera niedozwolone znaki.");
                return false;
            }

            while(op_stack.Count != 0){
                calc_list.Add(op_stack.Pop());
            }

            return true;
        }


        private bool IsOperator(string arg){
            return new Regex(@"[\&\|\>\=\+FG]").Match(arg).Success;
        }

        private bool IsNegation(string arg)
        {
            return new Regex(@"[\~]").Match(arg).Success;
        }

        private bool IsVariable(string arg){
            return new Regex(@"\b[A-Za-z]+").Match(arg).Success;
        }

        private bool HasPrecidenceOrEqual(string lhs, string rhs){
            return ((int)precidence[lhs] >= (int)precidence[rhs]);
        }           




        // from file
        public IExpression Parse(Stream stream)
        {
            return new Implication(
                new Implication(new Conjunction( new Literal("p"), new Negation(new Literal("r"))), new Literal("q")),
                new Disjunction(new Negation(new Literal("p")), new Literal("q")));
        }
    }

    public struct Token
    {
        public int _class;
        public string repr;
    }
    public struct TokenExpr
    {
        //public int _class;
        public IExpression repr;
        public TokenExpr(int cls, string expr)
        {
            //_class = cls;
            repr = new Literal(expr);
        }


    }
}