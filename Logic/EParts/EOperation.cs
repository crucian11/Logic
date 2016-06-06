using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class EOperation:ExpressionPart
    {
        public EOperation(String text)
            : base(text)
        {
            if (text == LogicSymbols.NOT) this.priority = 6;
            if (text == LogicSymbols.AND) this.priority = 5;
            if (text == LogicSymbols.OR) this.priority = 4;
            if (text == LogicSymbols.IMP) this.priority = 3;
            if (text == LogicSymbols.EQU) this.priority = 2;
        }

        public override byte compare(ExpressionPart x)
        {
            if (x.priority >= this.priority) return 2;
            else return 1;
        }

        public void commit(Stack<ExpressionPart> values) {
            EOperand left = null;
            EOperand right =  null;

            if (this.priority != 6) right = (EOperand)values.Pop();
            left = (EOperand)values.Pop();

            try
            {
                switch (this.priority)
                {
                    case 6:
                        values.Push(new EOperand((!left.value).ToString()));
                        break;

                    case 5:
                        values.Push(new EOperand((left.value & right.value).ToString()));
                        break;

                    case 4:
                        values.Push(new EOperand((left.value | right.value).ToString()));
                        break;

                    case 3:
                        values.Push(new EOperand(((!left.value) | right.value).ToString()));
                        break;

                    case 2:
                        values.Push(new EOperand((left.value == right.value).ToString()));
                        break;
                }
            }
            catch (InvalidOperationException ex) {
                throw new InvalidOperationException("Some of operands doesn't have it's value(value property == null)", ex);
            }
        }

        public static bool isSuitablePart(String x)
        {
            if (x == LogicSymbols.AND || x == LogicSymbols.OR || x == LogicSymbols.NOT || x == LogicSymbols.IMP || x == LogicSymbols.EQU) return true;
            else return false;
        }
    }
}
