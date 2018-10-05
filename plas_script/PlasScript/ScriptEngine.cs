using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasScript
{
    public class ScriptEngine
    {
        Dictionary<string, Tuple<List<string>, int, string, List<ICommand>>> userdefined;

        public ScriptEngine(Dictionary<string, Tuple<List<string>, int, string, List<ICommand>>> userdefined)
        {
            this.userdefined = userdefined;
        }

        public dynamic Run(string index, ListView<dynamic> local)
        {
            var (_, s, _, list) = this.userdefined[index];
            local.Add(s);
            dynamic ret = new VoidType();
            foreach(var v in list)
            {
                var re = v.Run(local, this);
#if DEBUG
                foreach(var value in local)
                {
                    Console.Write($"{value} ");
                }
                Console.WriteLine();
#endif
                if (re != null)
                {
                    ret = re;
                    break;
                }
            }
            local.Remove(s);
            return ret;
        }

        public dynamic Run(string index)
        {
            return Run(index, new ListView<dynamic>(new List<dynamic>(), 0));
        }

        public void OutPut(TextWriter writer)
        {
            writer.WriteLine("class Program{");
            foreach(var key in this.userdefined.Keys)
            {
                var (atype, _, rtype, list) = this.userdefined[key];
                writer.Write($"static {rtype} {key}(");
                if (atype.Count != 0)
                {
                    writer.Write($"{atype[0]} _local0");
                    foreach (var i in Utility.Range(1, atype.Count)) 
                    {
                        writer.Write($", {atype[i]} _local{i}");
                    }
                }
                writer.WriteLine("){");
                foreach(var v in list)
                {
                    v.OutPut(writer);
                }
                writer.WriteLine("}");
            }
            writer.WriteLine("}");
        }
    }
}
