using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasScript
{
    public interface ILexToken
    {
       
    }

    public class Name : ILexToken
    {
        public Name(string item)
        {
            this.Item = item;
        }

        public override string ToString()
        {
            return $"Name {this.Item}";
        }
        
        public override int GetHashCode()
        {
            return -979861770 + EqualityComparer<string>.Default.GetHashCode(this.Item);
        }

        public override bool Equals(object obj)
        {
            return obj is Name name &&
                   this.Item == name.Item;
        }

        public string Item { get; }
    }
    public class NumberLiteral : ILexToken
    {
        public NumberLiteral(long item)
        {
            this.Item = item;
        }

        public override string ToString()
        {
            return $"Number {this.Item}";
        }

        public override bool Equals(object obj)
        {
            return obj is NumberLiteral literal &&
                   this.Item == literal.Item;
        }

        public override int GetHashCode()
        {
            return -979861770 + this.Item.GetHashCode();
        }

        public long Item { get; }
    }
    public class DoubleLiteral : ILexToken
    {
        public DoubleLiteral(double item)
        {
            this.Item = item;
        }

        public override string ToString()
        {
            return $"Double {this.Item}";
        }

        public override bool Equals(object obj)
        {
            return obj is DoubleLiteral literal &&
                   this.Item == literal.Item;
        }

        public override int GetHashCode()
        {
            return -979861770 + this.Item.GetHashCode();
        }

        public double Item { get; }
    }
    public class StringLiteral :ILexToken
    {
        public StringLiteral(string item)
        {
            this.Item = item;
        }

        public override string ToString()
        {
            return $"String {this.Item}";
        }

        public override bool Equals(object obj)
        {
            return obj is StringLiteral literal &&
                   this.Item == literal.Item;
        }

        public override int GetHashCode()
        {
            return -979861770 + EqualityComparer<string>.Default.GetHashCode(this.Item);
        }

        public string Item { get; }
    }
    public class CharLiteral : ILexToken
    {
        public CharLiteral(char item)
        {
            this.Item = item;
        }

        public override string ToString()
        {
            return $"Char '{this.Item}'";
        }

        public override bool Equals(object obj)
        {
            return obj is CharLiteral literal &&
                   this.Item == literal.Item;
        }

        public override int GetHashCode()
        {
            return -979861770 + this.Item.GetHashCode();
        }

        public char Item { get; }
    }
    public class Parentheses : ILexToken
    {
        public Parentheses(string item)
        {
            this.Item = item;
        }

        public override string ToString()
        {
            return $"Parentheses {this.Item}";
        }

        public override bool Equals(object obj)
        {
            return obj is Parentheses parentheses &&
                   this.Item == parentheses.Item;
        }

        public override int GetHashCode()
        {
            return -979861770 + EqualityComparer<string>.Default.GetHashCode(this.Item);
        }

        public string Item { get; }
    }
    public class Operator : ILexToken
    {
        public Operator(string item)
        {
            this.Item = item;
        }

        public override string ToString()
        {
            return $"Operator {this.Item}";
        }

        public override bool Equals(object obj)
        {
            return obj is Operator @operator &&
                   this.Item == @operator.Item;
        }

        public override int GetHashCode()
        {
            return -979861770 + EqualityComparer<string>.Default.GetHashCode(this.Item);
        }

        public string Item { get; }
    }

    public static class LexToken
    {
        static public Name NewName(string item)
        {
            return new Name(item);
        }
        static public NumberLiteral NewNumberLiteral(long item)
        {
            return new NumberLiteral(item);
        }
        static public DoubleLiteral NewDoubleLiteral(double item)
        {
            return new DoubleLiteral(item);
        }
        static public StringLiteral NewStringLiteral(string item)
        {
            return new StringLiteral(item);
        }
        static public CharLiteral NewCharLiteral(char item)
        {
            return new CharLiteral(item);
        }
        static public Parentheses NewParentheses(string item)
        {
            return new Parentheses(item);
        }
        static public Operator NewOperator(string item)
        {
            return new Operator(item);
        }
    }
}
