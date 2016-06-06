using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class EOperand:ExpressionPart
    {
        public bool? value;

        public bool isConstant = false;

        //Using for creating pattern operands where name and other properties aren't important, this kind of operands used in searching by List 
        public bool isPartOfPattern = false;

        public EOperand(String text)
            : base(text)
        {

            this.priority = null;
            if (!this.isPartOfPattern)
            try
            {
                this.value = bool.Parse(text);
                isConstant = true;
            }
            catch (FormatException)
            {
                if (text == "T")
                {
                    this.value = true;
                    isConstant = true;
                }
                else if (text == "F")
                {
                    this.value = false;
                    isConstant = true;
                }
                else this.value = null;
            }
        }

        public EOperand(bool? value)
            : base(value.ToString())
        {
            this.priority = null;
            this.value = value;
        }

        public EOperand(String text, bool? value)
            : base(text)
        {
            this.priority = null;
            this.value = value;
        }

        public override byte compare(ExpressionPart x) 
        {
            return 0;
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is EOperand)) return false;
            if ((!this.isPartOfPattern && ((EOperand)obj).isPartOfPattern) || (this.isPartOfPattern && !((EOperand)obj).isPartOfPattern)) return true;
            return base.Equals(obj);
        }

        public static bool isSuitablePart(String x)
        {
            return x.All<char>(c => Char.IsLetter(c)) && x.Length != 0;
        }
    }
}
