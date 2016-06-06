using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public abstract class ExpressionPart
    {
        /* 
         * Class represents separate part of math expression
         * using for Reverse Polish notation
         */

        public sbyte? priority;
        public String text;

        public ExpressionPart(String text) {
            this.text = text;
        }

        /*Function returns a code of comparing another part of expression
         * 0 -- put argument value into the solve stack
         * 1 -- put argument value into the buffer stack
         * 2 -- put this value into the solve stack
         * 3 -- take away this and argument value
         * 4 -- start symbol of expression and end symbol have met
         */
        public abstract byte compare(ExpressionPart x);

        public override bool Equals(Object obj)
        {
            if (!(obj is ExpressionPart)) return false;
            return this.text == ((ExpressionPart)obj).text;
        }

        public override string ToString()
        {
            return text;
        }
    }
}
