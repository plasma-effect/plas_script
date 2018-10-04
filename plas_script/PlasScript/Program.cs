using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlasScript.Parser;
using static System.Console;

namespace PlasScript
{
    class Program
    {
        static long Eval0(SequenceResult result)
        {
            var ret = Eval1(result[0] as SequenceResult);
            var r1 = result[1] as RepeatResult;
            foreach (var i in Utility.Range(0, r1.Count))
            {
                var u = r1[i] as SequenceResult;
                var u0 = u[0] as TermResult<Operator>;
                var u1 = u[1] as SequenceResult;
                var v = Eval1(u1);
                if (u0.Item.Item == "+")
                {
                    ret += v;
                }
                else
                {
                    ret -= v;
                }
            }
            return ret;
        }
        static long Eval1(SequenceResult result)
        {
            var ret = Eval2(result[0] as SelectedResult);
            var r1 = result[1] as RepeatResult;
            foreach (var i in Utility.Range(0, r1.Count))
            {
                var u = r1[i] as SequenceResult;
                var u0 = u[0] as TermResult<Operator>;
                var u1 = u[1] as SelectedResult;
                var v = Eval2(u1);
                if (u0.Item.Item == "*")
                {
                    ret *= v;
                }
                else
                {
                    ret /= v;
                }
            }
            return ret;
        }
        static long Eval2(SelectedResult result)
        {
            if (result.Index == 0)
            {
                var r = result.Result as SequenceResult;
                return Eval0(r[1] as SequenceResult);
            }
            else
            {
                var r = result.Result as TermResult<NumberLiteral>;
                return r.Item.Item;
            }
        }

        static void Main(string[] args)
        {
            var lexer = new Lexer();
            var parser = new Parser(
                NewValue(1) + NewRepeat(NewOperatorSpec("+", "-") + NewValue(1)),
                NewValue(2) + NewRepeat(NewOperatorSpec("*", "/") + NewValue(2)),
                    (NewParentSpec("(") + NewValue(0) + NewParentSpec(")"))
                    / NewAnyTerm<NumberLiteral>());
            var lex = lexer.Analize(ReadLine());
            foreach(var v in lex)
            {
                WriteLine(v);
            }
            WriteLine(parser.Parse(lex, out var ret) ? ret.ToString() : "パースに失敗しました");
            WriteLine(Eval0(ret as SequenceResult));
        }
    }
}
