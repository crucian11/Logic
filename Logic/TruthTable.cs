using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class TruthTable
    {
        //Truth table only for simple variables
        public bool[][] truthTableSimple;

        //Truth table only for whole expression
        public bool[] truthTableExpression;

        public TruthTable(int varCount) {
            truthTableSimple = new bool[(int)Math.Pow(2, varCount)][];

            truthTableExpression = new bool[truthTableSimple.Length];
        }

        public bool isTautology() {
            //Method determines LogicExpression is tautology
            foreach (bool x in truthTableExpression)
                if (!x) return false;
            return true;
        }
    }
}
