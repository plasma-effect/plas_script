using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasScript
{
    public static class Utility
    {
        static public IEnumerable<int> Range(int start,int end)
        {
            for(var i = start; i < end; ++i)
            {
                yield return i;
            }
        }
        static public ListView<T> MakeListView<T>(List<T> list, int shift = 0)
        {
            return new ListView<T>(list, shift);
        }
    }

    public class ListView<T> : IEnumerable<T>
    {
        int shift;

        public ListView(List<T> list, int shift)
        {
            this.Full = list;
            this.shift = shift;
        }

        public T this[int index]
        {
            get
            {
                return this.Full[index + this.shift];
            }
            set
            {
                this.Full[index + this.shift] = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Full.Skip(this.shift).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Full.Skip(this.shift).GetEnumerator();
        }

        public ListView<T> MakeListView(int shift)
        {
            return new ListView<T>(this.Full, shift + this.shift);
        }

        /// <summary>
        /// 後ろからcount要素削除
        /// </summary>
        /// <param name="count">削除する個数</param>
        public void Remove(int count)
        {
            this.Full.RemoveRange(this.Full.Count - count, count);
        }

        /// <summary>
        /// 後ろにcount要素追加
        /// </summary>
        /// <param name="count">追加する個数</param>
        public void Add(int count)
        {
            this.Full.AddRange(Enumerable.Repeat(default(T), count));
        }

        /// <summary>
        /// shift後の残り要素数を見る
        /// </summary>
        public int Count
        {
            get
            {
                return this.Full.Count - this.shift;
            }
        }

        /// <summary>
        /// 素のリスト
        /// </summary>
        public List<T> Full { get; }
    }
    

}
