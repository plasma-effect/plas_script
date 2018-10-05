using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlasScript.Utility;

namespace PlasScript
{
    public interface IExpr
    {
        dynamic Eval(ListView<dynamic> local, ScriptEngine engine);
    }
    
    namespace Expression
    {
        public class NumberLiteral : IExpr
        {
            long val;

            public NumberLiteral(long val)
            {
                this.val = val;
            }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                return this.val;
            }
            public override string ToString()
            {
                return this.val.ToString();
            }
        }
        public class StringLiteral : IExpr
        {
            string str;

            public StringLiteral(string str)
            {
                this.str = str;
            }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                return this.str.Substring(1, this.str.Length - 2);
            }
            public override string ToString()
            {
                return this.str;
            }
        }
        public class DoubleLiteral : IExpr
        {
            double db;

            public DoubleLiteral(double db)
            {
                this.db = db;
            }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                return this.db;
            }
            public override string ToString()
            {
                return this.db.ToString();
            }
        }
        public class CharLiteral : IExpr
        {
            char c;

            public CharLiteral(char c)
            {
                this.c = c;
            }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                return this.c;
            }
            public override string ToString()
            {
                return $"'{this.c}'";
            }
        }
        public class Value : IExpr
        {
            public Value(int index)
            {
                this.Index = index;
            }

            public int Index { get; set; }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                return local[this.Index];
            }
            public override string ToString()
            {
                return $"_{this.Index}";
            }
        }
        public class AssignOperator : IExpr
        {
            IExpr lhs;
            IExpr rhs;

            public AssignOperator(IExpr val, IExpr rhs)
            {
                this.lhs = val;
                this.rhs = rhs;
            }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                if (this.lhs is Value index)
                {
                    local[index.Index] = this.rhs.Eval(local, engine);
                }
                else if (this.lhs is IndexCall call && call.Val is Value index2) 
                {
                    local[index2.Index][call.Expr.Eval(local, engine)] = this.rhs.Eval(local, engine);
                }
                return new VoidType();
            }
            public override string ToString()
            {
                return $"{this.lhs} = {this.rhs}";
            }
        }
        public class EmbeddedFunction : IExpr
        {
            string name;
            Func<dynamic[], dynamic> func;
            IExpr[] exprs;

            public EmbeddedFunction(string name, Func<dynamic[], dynamic> func, IExpr[] exprs)
            {
                this.name = name;
                this.func = func;
                this.exprs = exprs;
            }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                return this.func(this.exprs.Select(e => e.Eval(local, engine)).ToArray());
            }

            public override string ToString()
            {
                var str = new StringBuilder();
                str.Append($"{this.name}(");
                if (this.exprs.Length != 0)
                {
                    str.Append(this.exprs[0].ToString());
                }
                foreach(var i in Range(1, this.exprs.Length))
                {
                    str.Append($", {this.exprs[i]}");
                }
                str.Append(")");
                return str.ToString();
            }
        }
        public class EmbeddedBiOperator : IExpr
        {
            string name;
            Func<dynamic, dynamic, dynamic> func;
            IExpr lhs, rhs;

            public EmbeddedBiOperator(string name, Func<dynamic, dynamic, dynamic> func, IExpr lhs, IExpr rhs)
            {
                this.name = name;
                this.func = func;
                this.lhs = lhs;
                this.rhs = rhs;
            }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                return this.func(this.lhs.Eval(local, engine), this.rhs.Eval(local, engine));
            }

            public override string ToString()
            {
                return $"({this.lhs} {this.name} {this.rhs})";
            }
        }
        public class EmbeddedMonoOperator : IExpr
        {
            string name;
            Func<dynamic, dynamic> func;
            IExpr expr;

            public EmbeddedMonoOperator(string name, Func<dynamic, dynamic> func, IExpr expr, IExpr rhs)
            {
                this.name = name;
                this.func = func;
                this.expr = expr;
            }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                return this.func(this.expr.Eval(local, engine));
            }

            public override string ToString()
            {
                return $"{this.name}{this.expr}";
            }
        }
        public class TriOperator : IExpr
        {
            IExpr cond, @true, @false;

            public TriOperator(IExpr cond, IExpr @true, IExpr @false)
            {
                this.cond = cond;
                this.@true = @true;
                this.@false = @false;
            }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                if (this.cond.Eval(local, engine))
                {
                    return this.@true.Eval(local, engine);
                }
                else
                {
                    return this.@false.Eval(local, engine);
                }
            }

            public override string ToString()
            {
                return $"({this.cond} ? {this.@true} : {this.@false})";
            }
        }
        public class UserDefinedFunction : IExpr
        {
            string name;
            IExpr[] args;

            public UserDefinedFunction(string name, IExpr[] args)
            {
                this.name = name;
                this.args = args;
            }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                var view = local.MakeListView(local.Count);
                view.Add(this.args.Length);
                foreach (var i in Range(0, this.args.Length))
                {
                    view[i] = this.args[i].Eval(local, engine);
                }
                var ret = engine.Run(this.name, view);
                view.Remove(this.args.Length);
                return ret;
            }

            public override string ToString()
            {
                var str = new StringBuilder();
                str.Append($"{this.name}(");
                if (this.args.Length != 0)
                {
                    str.Append(this.args[0]);
                    foreach(var i in Range(1, this.args.Length))
                    {
                        str.Append($", {this.args[i]}");
                    }
                }
                str.Append(")");
                return str.ToString();
            }
        }
        public class IndexCall : IExpr
        {
            public IndexCall(IExpr val, IExpr expr)
            {
                this.Val = val;
                this.Expr = expr;
            }

            public IExpr Val { get; set; }
            public IExpr Expr { get; set; }

            public dynamic Eval(ListView<dynamic> local, ScriptEngine engine)
            {
                return this.Val.Eval(local, engine)[this.Expr.Eval(local, engine)];
            }

            public override string ToString()
            {
                return $"{this.Val}[{this.Expr}]";
            }
        }  
    }
}
