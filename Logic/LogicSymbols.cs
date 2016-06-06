using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public static class LogicSymbols
    {
        public static String NOT = '\u00AC'.ToString();
        public static String AND = '\u2227'.ToString();
        public static String OR = '\u2228'.ToString();
        public static String IMP = '\u2192'.ToString();
        public static String EQU = '\u2194'.ToString();

        public static String allSymbols() {
            return LogicSymbols.NOT + LogicSymbols.AND + LogicSymbols.OR + LogicSymbols.IMP + LogicSymbols.EQU;
        }

        public static String lessOrEqualPrioritySymbols(String sym) {
            if (sym == LogicSymbols.NOT) return LogicSymbols.NOT + LogicSymbols.AND + LogicSymbols.OR + LogicSymbols.IMP + LogicSymbols.EQU;
            if (sym == LogicSymbols.AND) return LogicSymbols.AND + LogicSymbols.OR + LogicSymbols.IMP + LogicSymbols.EQU;
            if (sym == LogicSymbols.OR) return LogicSymbols.OR + LogicSymbols.IMP + LogicSymbols.EQU;
            if (sym == LogicSymbols.IMP) return LogicSymbols.IMP + LogicSymbols.EQU;
            if (sym == LogicSymbols.EQU) return LogicSymbols.EQU;
            return "";
        }

        public static String lessPrioritySymbols(String sym)
        {
            if (sym == LogicSymbols.NOT) return LogicSymbols.AND + LogicSymbols.OR + LogicSymbols.IMP + LogicSymbols.EQU;
            if (sym == LogicSymbols.AND) return LogicSymbols.OR + LogicSymbols.IMP + LogicSymbols.EQU;
            if (sym == LogicSymbols.OR) return LogicSymbols.IMP + LogicSymbols.EQU;
            if (sym == LogicSymbols.IMP) return LogicSymbols.EQU;
            if (sym == LogicSymbols.EQU) return "";
            return "";
        } 
    }
}
