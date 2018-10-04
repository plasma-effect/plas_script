using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasScript
{
    static class Utility
    {
        static public IEnumerable<int> Range(int start,int end)
        {
            for(var i = start; i < end; ++i)
            {
                yield return i;
            }
        }
    }
}
