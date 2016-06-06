using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class EBrackets:ExpressionPart
    {
        public EBrackets(String text)
            : base(text)
        {
            switch (text) { 
                case "(":
                    this.priority = 1;
                    break;
                case ")":
                    this.priority = 7;
                    break;
            }
        }

        public override byte compare(ExpressionPart x)
        {
            if (this.priority == 1) return 1;
            if (this.priority == 7) {
                if (x.priority == 1) return 3;
                if (x.priority == 0) throw new FormatException("Brackets position is incorrect");
            }
            return 2;
        }

        public static bool isSuitablePart(String x)
        {
            if (x == "(" || x == ")") return true;
            else return false;
        }
    }
}
