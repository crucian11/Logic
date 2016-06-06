using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class ESeparateSymbol:ExpressionPart
    {
        private static string symString = "$"; 
        public ESeparateSymbol(bool isStartSymbol)
            : base(symString)
        {
            if (isStartSymbol) this.priority = 0;
            else this.priority = 6;
        }

        public override byte compare(ExpressionPart x)
        {
            if (x.priority == 1) throw new FormatException("Brackets position is incorrect");
            if (x.priority != 0) return 2;
            return 4;
        }

        public static bool isSuitablePart(String x)
        {
            if (x == symString) return true;
            else return false;
        }
    }
}
