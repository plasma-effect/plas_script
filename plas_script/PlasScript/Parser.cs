using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlasScript.Utility;

namespace PlasScript
{
    public class Parser
    {
        public abstract class Expression
        {
            public abstract IPResult Parse(List<ILexToken> line, int index, out int next, Parser parser);
            public static Expression operator +(Expression lhs, Expression rhs)
            {
                if(lhs is Sequence seq)
                {
                    seq.Add(rhs);
                    return seq;
                }
                return new Sequence(lhs, rhs);
            }
            public static Expression operator /(Expression lhs, Expression rhs)
            {
                if (lhs is Select sel)
                {
                    sel.Add(rhs);
                    return sel;
                }
                return new Select(lhs, rhs);
            }
        }
        public class AnyTerm<T> : Expression
            where T : ILexToken
        {
            public override IPResult Parse(List<ILexToken> line, int index, out int next, Parser parser)
            {
                if (index != line.Count && line[index] is T v) 
                {
                    next = index + 1;
                    return new TermResult<T>(v);
                }
                next = -1;
                return null;
            }
        }
        public class SpecTerm<T> : Expression
            where T : ILexToken
        {
            T[] exprs;

            public SpecTerm(params T[] exprs)
            {
                this.exprs = exprs;
            }

            public override IPResult Parse(List<ILexToken> line, int index, out int next, Parser parser)
            {
                if (index != line.Count && line[index] is T)
                {
                    foreach(var v in this.exprs)
                    {
                        if (v.Equals(line[index]))
                        {
                            next = index + 1;
                            return new TermResult<T>(v);
                        }
                    }
                }
                next = -1;
                return null;
            }
        }
        public class Sequence : Expression
        {
            List<Expression> list;

            public Sequence(params Expression[] list)
            {
                this.list = new List<Expression>(list);
            }

            public override IPResult Parse(List<ILexToken> line, int index, out int next, Parser parser)
            {
                var ret = new List<IPResult>();
                foreach(var v in this.list)
                {
                    ret.Add(v.Parse(line, index, out index, parser));
                    if (index == -1)
                    {
                        next = -1;
                        return null;
                    }
                }
                next = index;
                return new SequenceResult(ret);
            }

            public void Add(Expression expr)
            {
                this.list.Add(expr);
            }
        }
        public class Select : Expression
        {
            List<Expression> list;

            public Select(params Expression[] list)
            {
                this.list = new List<Expression>(list);
            }

            public override IPResult Parse(List<ILexToken> line, int index, out int next, Parser parser)
            {
                foreach (var i in Range(0, this.list.Count)) 
                {
                    var ret = this.list[i].Parse(line, index, out next, parser);
                    if (next != -1)
                    {
                        return new SelectedResult(i, ret);
                    }
                }
                next = -1;
                return null;
            }

            public void Add(Expression expr)
            {
                this.list.Add(expr);
            }
        }
        public class Repeat : Expression
        {
            Expression expr;

            public Repeat(Expression expr)
            {
                this.expr = expr;
            }

            public override IPResult Parse(List<ILexToken> line, int index, out int next, Parser parser)
            {
                var ret = new List<IPResult>();
                foreach(var i in Range(0, 0x1000000))
                {
                    var v = this.expr.Parse(line, index, out next, parser);
                    if (next == -1)
                    {
                        next = index;
                        return new RepeatResult(ret);
                    }
                    index = next;
                    ret.Add(v);
                }
                next = -1;
                return null;
            }
        }
        public class Optional : Expression
        {
            Expression expr;

            public Optional(Expression expr)
            {
                this.expr = expr;
            }

            public override IPResult Parse(List<ILexToken> line, int index, out int next, Parser parser)
            {
                var ret = this.expr.Parse(line, index, out next, parser);
                if (next == -1)
                {
                    next = index;
                }
                return ret;
            }
        }
        public class Value : Expression
        {
            int index;

            public Value(int index)
            {
                this.index = index;
            }

            public override IPResult Parse(List<ILexToken> line, int index, out int next, Parser parser)
            {
                return parser.Parse(line, index, out next, this.index);
            }
        }

        Expression[] exprs;

        IPResult Parse(List<ILexToken> line, int index, out int next, int val)
        {
            return this.exprs[val].Parse(line, index, out next, this);
        }

        public bool Parse(List<ILexToken> line, out IPResult result, int index = 0)
        {
            result = Parse(line, index, out var n, 0);
            return n != -1;
        }

        public Parser(params Expression[] exprs)
        {
            this.exprs = exprs;
        }


        static public AnyTerm<T> NewAnyTerm<T>()
            where T : ILexToken
        {
            return new AnyTerm<T>();
        }
        static public SpecTerm<T> NewSpec<T>(T expr)
            where T : ILexToken
        {
            return new SpecTerm<T>(expr);
        }
        static public SpecTerm<Operator> NewOperatorSpec(params string[] ops)
        {
            var ret = new Operator[ops.Length];
            foreach(var i in Range(0, ops.Length))
            {
                ret[i] = LexToken.NewOperator(ops[i]);
            }
            return new SpecTerm<Operator>(ret);
        }
        static public SpecTerm<Name> NewNameSpec(params string[] ops)
        {
            var ret = new Name[ops.Length];
            foreach (var i in Range(0, ops.Length))
            {
                ret[i] = LexToken.NewName(ops[i]);
            }
            return new SpecTerm<Name>(ret);
        }
        static public SpecTerm<Parentheses> NewParentSpec(string p)
        {
            return NewSpec(new Parentheses(p));
        }
        static public Repeat NewRepeat(Expression expr)
        {
            return new Repeat(expr);
        }
        static public Optional NewOptional(Expression expr)
        {
            return new Optional(expr);
        }
        static public Value NewValue(int index)
        {
            return new Value(index);
        }
    }
}
