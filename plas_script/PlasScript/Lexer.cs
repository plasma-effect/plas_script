using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static PlasScript.LexToken;

namespace PlasScript
{
    public class Lexer
    {
        Regex strreg, charreg, namereg, doublereg, numreg;
        Regex twopreg, oneopreg, parareg;
        public Lexer()
        {
            this.strreg = new Regex(@"""([^""\\]|\\.)*""");
            this.charreg = new Regex(@"\'.\'");
            this.namereg = new Regex(@"[a-zA-Z_]\w*");
            this.doublereg = new Regex(@"\d+\.\d*");
            this.numreg = new Regex(@"\d+");
            this.twopreg = new Regex(@"(\+\=)|(\-\=)|(\*\=)|(\/\=)|(\%\=)|(\<\<\=)|(\>\>\=)|(\&\=)|(\|\=)|(\^\=)|(\+\+)|(\-\-)|(\:\:)|(\<\<)|(\>\>)|(\<\=)|(\>\=)|(\=\=)|(\!\=)|(\&\&)|(\|\|)");
            this.oneopreg = new Regex(@"\.|\,|\?|\+|\-|\~|\!|\*|\/|\%|\<|\>|\&|\||\^|\=|\:");
            this.parareg = new Regex(@"\[|\]|\(|\)|\{|\}");
        }
        
        public List<ILexToken> Analize(string line)
        {
            return Analize(line, 0, line.Length, new List<ILexToken>());
        }

        private bool RegexSearch(Regex regex, string line,int start,int end,out Match match)
        {
            match = regex.Match(line, start, end - start);
            return match.Success;
        }

        private List<ILexToken> Analize(string line, int start, int end, List<ILexToken> ret)
        {
            if (RegexSearch(this.strreg, line, start, end, out var match))
            {
                return Next(NewStringLiteral(match.Value), match, line, start, end, ret);
            }
            else if (RegexSearch(this.charreg, line, start, end, out match))
            {
                return Next(NewCharLiteral(match.Value[1]), match, line, start, end, ret);
            }
            else if (RegexSearch(this.namereg, line, start, end, out match))
            {
                return Next(NewName(match.Value), match, line, start, end, ret);
            }
            else if (RegexSearch(this.doublereg, line, start, end, out match))
            {
                return Next(NewDoubleLiteral(double.Parse(match.Value)), match, line, start, end, ret);
            }
            else if (RegexSearch(this.numreg, line, start, end, out match))
            {
                return Next(NewNumberLiteral(long.Parse(match.Value)), match, line, start, end, ret);
            }
            else if (RegexSearch(this.twopreg, line, start, end, out match))
            {
                return Next(NewOperator(match.Value), match, line, start, end, ret);
            }
            else if (RegexSearch(this.oneopreg, line, start, end, out match))
            {
                return Next(NewOperator(match.Value), match, line, start, end, ret);
            }
            else if (RegexSearch(this.parareg, line, start, end, out match))
            {
                return Next(NewParentheses(match.Value), match, line, start, end, ret);
            }
            else if (line.Substring(start, end - start).All(c => c == ' ')) 
            {
                return ret;
            }
            else
            {
                throw new ArgumentException("不明なトークンが含まれています");
            }
        }

        private List<ILexToken> Next(ILexToken token, Match match, string line,int start,int end,List<ILexToken> ret)
        {
            Analize(line, start, match.Index, ret);
            ret.Add(token);
            return Analize(line, match.Index + match.Length, end, ret);
        }
    }
}
