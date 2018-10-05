using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using PlasScript.Expression;

namespace PlasScript
{
    class Program
    {
        static dynamic Plus(dynamic lhs, dynamic rhs)
        {
            return lhs + rhs;
        }

        static dynamic Equal(dynamic lhs, dynamic rhs)
        {
            return lhs.Equals(rhs);
        }
        
        static dynamic InternalWriteLine(dynamic[] d)
        {
            foreach(var v in d)
            {
                if (v is long[] ar)
                {
                    foreach(var u in ar)
                    {
                        Write($"{u} ");
                    }
                }
                else
                {
                    Write($"{v} ");
                }
            }
            WriteLine();
            return new VoidType();
        }

        static dynamic InternalReadLine(dynamic[] d)
        {
            return ReadLine().Split(' ').Select(s => long.Parse(s)).ToArray();
        }

        static dynamic[] ConSum(dynamic[] d)
        {
            foreach(var i in Utility.Range(1, d[0].Length))
            {
                d[0][i] += d[0][i - 1];
            }
            return d[0];
        }

        static ExpressionCommand MakeWriteLine(IExpr expr)
        {
            return new ExpressionCommand(
                new EmbeddedFunction("WriteLine", InternalWriteLine,
                new IExpr[] { expr }));
        }

        static ValueDefineCommand MakeReadLine(int index)
        {
            return
                new ValueDefineCommand(index,
                new EmbeddedFunction("ReadLine", InternalReadLine,
                new IExpr[] {}));
        }


        static void Main(string[] args)
        {
            var command = new List<ICommand>
            {
                MakeReadLine(0),
                MakeWriteLine(new Value(0)),
                new ExpressionCommand(new AssignOperator(
                    new IndexCall(new Value(0),new Expression.NumberLiteral(0)),
                    new IndexCall(new Value(0),new Expression.NumberLiteral(1)))),
                MakeWriteLine(new Value(0))
            };

            var engine = new ScriptEngine(
                new Dictionary<string, Tuple<List<string>, int, string, List<ICommand>>>
                {
                    {"Main",Tuple.Create(new List<string>{},2,"void",command) }
                });
            engine.Run("Main");
            engine.OutPut(Out);
#if DEBUG
            WriteLine("終了を待機しています。Enterを押してください");
            ReadLine();
#endif
        }
    }
}
