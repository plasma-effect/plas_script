using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasScript
{
    public interface IExpr
    {
        dynamic Eval(List<dynamic> local);
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

            public dynamic Eval(List<dynamic> local)
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

            public dynamic Eval(List<dynamic> local)
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

            public dynamic Eval(List<dynamic> local)
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

            public dynamic Eval(List<dynamic> local)
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
            int index;

            public Value(int index)
            {
                this.index = index;
            }

            public dynamic Eval(List<dynamic> local)
            {
                return local[this.index];
            }
            public override string ToString()
            {
                return $"_local{this.index}";
            }
        }
        public class AssignOperator : IExpr
        {
            int index;
            IExpr rhs;

            public AssignOperator(int index, IExpr rhs)
            {
                this.index = index;
                this.rhs = rhs;
            }

            public dynamic Eval(List<dynamic> local)
            {
                local[this.index] = this.rhs.Eval(local);
                return new VoidType();
            }
            public override string ToString()
            {
                return $"_local{this.index} = {this.rhs}";
            }
        }
    }
}
