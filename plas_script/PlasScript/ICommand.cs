using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlasScript.Expression;

namespace PlasScript
{
    public interface ICommand
    {
        dynamic Run(ListView<dynamic> local, ScriptEngine engine);
        void OutPut(TextWriter writer);
    }

    public class ExpressionCommand : ICommand
    {
        IExpr expr;

        public ExpressionCommand(IExpr expr)
        {
            this.expr = expr;
        }

        public void OutPut(TextWriter writer)
        {
            writer.WriteLine($"{this.expr};");
        }

        public dynamic Run(ListView<dynamic> local, ScriptEngine engine)
        {
            this.expr.Eval(local, engine);
            return null;
        }
    }
    public class ValueDefineCommand : ICommand
    {
        AssignOperator expr;

        public ValueDefineCommand(int index, IExpr expr)
        {
            this.expr = new AssignOperator(new Value(index), expr);
        }

        public void OutPut(TextWriter writer)
        {
            writer.WriteLine($"var {this.expr};");
        }

        public dynamic Run(ListView<dynamic> local, ScriptEngine engine)
        {
            this.expr.Eval(local, engine);
            return null;
        }
    }
    public class ReturnCommand : ICommand
    {
        IExpr expr;

        public ReturnCommand(IExpr expr)
        {
            this.expr = expr;
        }

        public void OutPut(TextWriter writer)
        {
            writer.WriteLine($"return {this.expr};");
        }

        public dynamic Run(ListView<dynamic> local, ScriptEngine engine)
        {
            return this.expr.Eval(local, engine);
        }
    }
    public class IfCommand : ICommand
    {
        List<Tuple<IExpr, List<ICommand>>> cond;
        List<ICommand> list;

        public IfCommand(List<Tuple<IExpr, List<ICommand>>> cond, List<ICommand> list)
        {
            this.cond = cond;
            this.list = list;
        }

        public void OutPut(TextWriter writer)
        {
            foreach (var (c, lis) in this.cond)
            {
                writer.WriteLine($"if({c}){{");
                foreach(var a in lis)
                {
                    a.OutPut(writer);
                }
                writer.Write($"}}else ");
            }
            writer.WriteLine("{");
            foreach(var a in this.list)
            {
                a.OutPut(writer);
            }
            writer.WriteLine("}");
        }

        public dynamic Run(ListView<dynamic> local, ScriptEngine engine)
        {
            foreach(var (c, lis) in this.cond)
            {
                if (c.Eval(local, engine))
                {
                    foreach(var u in lis)
                    {
                        var ret = u.Run(local, engine);
                        if (ret != null)
                        {
                            return ret;
                        }
                    }
                    return null;
                }
            }
            foreach (var u in this.list)
            {
                var ret = u.Run(local, engine);
                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }
    }
}
