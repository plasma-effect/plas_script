using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlasScript.Utility;

namespace PlasScript
{
    public interface IPResult
    {
    }
    public class SequenceResult : IPResult
    {
        List<IPResult> list;

        public SequenceResult(List<IPResult> list)
        {
            this.list = list;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("Sequence(");
            if (this.Length != 0)
            {
                str.Append(this.list[0]);
                foreach(var i in Range(1, this.Length))
                {
                    str.Append($", {this.list[i]}");
                }
            }
            str.Append(")");
            return str.ToString();
        }

        public int Length
        {
            get
            {
                return this.list.Count;
            }
        }

        public IPResult this[int index]
        {
            get
            {
                return this.list[index];
            }
        }
    }
    public class SelectedResult : IPResult
    {
        public SelectedResult(int index, IPResult result)
        {
            this.Index = index;
            this.Result = result;
        }

        public override string ToString()
        {
            return $"Select({this.Index}, {this.Result})";
        }

        public int Index { get; }
        public IPResult Result { get; }
    }
    public class RepeatResult : IPResult
    {
        List<IPResult> list;

        public RepeatResult(List<IPResult> list)
        {
            this.list = list;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("Repeat(");
            if (this.Count != 0)
            {
                str.Append(this.list[0]);
                foreach (var i in Range(1, this.Count))
                {
                    str.Append($", {this.list[i]}");
                }
            }
            str.Append(")");
            return str.ToString();
        }

        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        public IPResult this[int index]
        {
            get
            {
                return this.list[index];
            }
        }
    }
    public class TermResult<T> : IPResult
        where T : ILexToken
    {
        public TermResult(T item)
        {
            this.Item = item;
        }

        public override string ToString()
        {
            return $"Term({this.Item})";
        }

        public T Item { get; }
    }
}
