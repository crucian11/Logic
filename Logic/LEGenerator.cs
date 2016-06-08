using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Logic
{
    public class LEGenerator
    {
        //Properties are used in next() method
        Random gen = new Random();
        private static EOperand[] operandsSet = 
            new EOperand[] {new EOperand("p"), new EOperand("q"), new EOperand("r"), new EOperand("s"), new EOperand("t"), new EOperand("w"), new EOperand("z")};

        private static EOperation[] operationsSet = 
            new EOperation[] { new EOperation(LogicSymbols.AND), new EOperation(LogicSymbols.OR), new EOperation(LogicSymbols.IMP), new EOperation(LogicSymbols.EQU) };

        private int[] operationsCount;
        private int operandsRepeats = 2;
        private int operationsRepeats = 2;
        private int[] operandsCount;
        private int maxVarsNumber = operandsSet.Length + 3;

        //Expressing basic and derived argument forms with regex patterns for using in nextEqual()
        private static String[] wrapAND = new String[2] { @"(?<=\(|^|[" + LogicSymbols.lessOrEqualPrioritySymbols(LogicSymbols.AND) + @"])", @"(?=[" + LogicSymbols.lessOrEqualPrioritySymbols(LogicSymbols.AND) + @"]|\)|$)" };
        private static String[] wrapOR = new String[2] { @"(?<=\(|^|[" + LogicSymbols.lessOrEqualPrioritySymbols(LogicSymbols.OR) + @"])", @"(?=[" + LogicSymbols.lessOrEqualPrioritySymbols(LogicSymbols.OR) + @"]|\)|$)" };
        private static String[] wrapIMP = new String[2] { @"(?<=\(|^|[" + LogicSymbols.lessOrEqualPrioritySymbols(LogicSymbols.IMP) + @"])", @"(?=[" + LogicSymbols.lessOrEqualPrioritySymbols(LogicSymbols.IMP) + @"]|\)|$)" };
        private static String[] wrapEQU = new String[2] { @"(?<=\(|^|[" + LogicSymbols.lessOrEqualPrioritySymbols(LogicSymbols.EQU) + @"])", @"(?=[" + LogicSymbols.lessOrEqualPrioritySymbols(LogicSymbols.EQU) + @"]|\)|$)" };
        private static String operand = @"(" + LogicSymbols.NOT + @"?(?:\w+|\([\w" + LogicSymbols.allSymbols() + @"]+\)))";

        //Used when expressing implication by disjunction
        private static String[] wrapIMPSpecial = new String[2] { @"(?<=\(|^|[" + LogicSymbols.lessPrioritySymbols(LogicSymbols.IMP) + @"])", @"(?=[" + LogicSymbols.lessPrioritySymbols(LogicSymbols.IMP) + @"]|\)|$)" };

        private static String[][] laws = new String[][]
        {
            // ((p OR r) OR q) == (p OR (r OR q)) 
            new String[2]{wrapOR[0] + @"\(" + operand + LogicSymbols.OR + operand+@"\)" + LogicSymbols.OR + operand + wrapOR[1],  @"$1"+LogicSymbols.OR+@"($2"+LogicSymbols.OR+@"$3)"},

            // ((p AND r) AND q) == (p AND (r AND q)) 
            new String[2]{wrapAND[0] + @"\(" + operand + LogicSymbols.AND + operand + @"\)" + LogicSymbols.AND + operand + wrapAND[1],  @"$1" + LogicSymbols.AND + @"($2"+LogicSymbols.AND+@"$3)"},
            
            // ((p OR r) AND q) == ((p AND q) OR (r AND q)) 
            new String[2]{wrapOR[0] + @"\(" + operand + LogicSymbols.OR + operand + @"\)" + LogicSymbols.AND + operand + wrapOR[1],  @"($1" + LogicSymbols.AND + @"$3)" + LogicSymbols.OR + @"($2" + LogicSymbols.AND + @"$3)"},
            
            // ((p AND r) OR q) == ((p OR q) AND (r OR q)) 
            new String[2]{wrapOR[0] + @"\(" + operand + LogicSymbols.AND + operand + @"\)" + LogicSymbols.OR + operand + wrapOR[1],  @"($1" + LogicSymbols.OR + @"$3\)" + LogicSymbols.AND + @"($2" + LogicSymbols.OR + @"$3)"},

            // NOT(p OR q) == (NOT p AND NOT q)
            new String[2]{LogicSymbols.NOT + @"\(" + operand + LogicSymbols.OR + operand + @"\)",  @"(" + LogicSymbols.NOT + @"$1" + LogicSymbols.AND + LogicSymbols.NOT + @"$2)"},

            // NOT(p AND q) == (NOT p OR NOT q)
            new String[2]{LogicSymbols.NOT+@"\(" + operand + LogicSymbols.AND + operand + @"\)",  @"(" + LogicSymbols.NOT + @"$1" + LogicSymbols.OR + LogicSymbols.NOT + @"$2)"},

            // NOT(NOT p) == p
            new String[2]{LogicSymbols.NOT + @"\(" + LogicSymbols.NOT + operand + @"\)",  @"$1"},

            //(p AND p) == p 
            new String[2]{wrapAND[0] + operand + LogicSymbols.AND + @"\1" + wrapAND[1],  "$1"},

            //(p AND NOT p) == F 
            new String[2]{wrapAND[0] + operand + LogicSymbols.AND + LogicSymbols.NOT + @"\1" + wrapAND[1],  "F"},

            //(p AND F) == F 
            new String[2]{wrapAND[0] + operand + LogicSymbols.AND + @"F" + wrapAND[1],  "F"},
            
            //(p AND T) == p 
            new String[2]{wrapAND[0] + operand + LogicSymbols.AND + @"T" + wrapAND[1],  "$1"},

            //(p OR p) == p
            new String[2]{wrapOR[0] + operand + LogicSymbols.OR + @"\1" + wrapOR[1],  "$1"},

            //(p OR NOT p) == T
            new String[2]{wrapOR[0] + operand + LogicSymbols.OR + LogicSymbols.NOT + @"\1" + wrapOR[1],  "T"},

            //(p OR F) == p 
            new String[2]{wrapOR[0] + operand + LogicSymbols.OR + @"F" + wrapOR[1],  "$1"},

            //(p OR T) == T 
            new String[2]{wrapOR[0] + operand + LogicSymbols.OR + @"T" + wrapOR[1],  "T"},

            // p IMP q == NOT p OR q
            new String[2]{wrapIMPSpecial[0] + operand + LogicSymbols.IMP + operand + wrapIMPSpecial[1],  LogicSymbols.NOT + @"$1" + LogicSymbols.OR + @"$2"},

            //p AND q == q AND p 
            new String[2]{wrapAND[0] + operand + LogicSymbols.AND + operand + wrapAND[1],  "$2"+LogicSymbols.AND+"$1"},

            //p OR q == q OR p
            new String[2]{wrapOR[0] + operand + LogicSymbols.OR + operand +wrapOR[1],  "$2"+LogicSymbols.OR+"$1"}
        };

        public LEGenerator() {
            Regex.CacheSize = laws.Length + 2;
        }

        public LogicExpression next() {
            operationsCount = new int[operationsSet.Length];
            operandsCount = new int[operandsSet.Length];
            int operationsCountControl = 0, operandsCountControl = 0;

            List<ExpressionPart> lexp = new List<ExpressionPart>();
            bool generateAgain = false;

            //Generating vars
            int varsNumber = gen.Next(2, maxVarsNumber + 1);
            int currentOperandIndex, maxIndex = 2;
            for (int i = 0; i < varsNumber; i++)
                do
                {
                    generateAgain = false;
                    currentOperandIndex = gen.Next(0, maxIndex + 1);
                    if (operandsCount.All<int>(p => p == operandsRepeats)) throw new ApplicationException("Configuration of the class is invalid");
                    if (operandsCount[currentOperandIndex] == operandsRepeats)
                    {
                        generateAgain = true;
                        continue;
                    }
                    else
                    {
                        lexp.Add(operandsSet[currentOperandIndex]);
                        operandsCount[currentOperandIndex]++;
                        if (maxIndex < operandsSet.Length - 1) maxIndex++;
                    }
                } while (generateAgain);

            //Generating brackets
            if (lexp.Count >= 3) {
                int bracketsCoupleNumber = gen.Next(0, varsNumber / 2 + 1);
                int elementsInBrackets;
                int startPosition, endPosition;
                int[] scope;
                List<int[]> freeBracketsAreas = new List<int[]>();
                List<int[]> debug = new List<int[]>();
                freeBracketsAreas.Add(new int[2] { 0, lexp.Count - 1 });
                int i = 0;
                generateAgain = true;
                while (i <= bracketsCoupleNumber)
                {
                    generateAgain = false;

                    //Choose unoccupied area
                    scope = freeBracketsAreas[gen.Next(freeBracketsAreas.Count)];

                    foreach (int[] j in freeBracketsAreas)
                        if (scope[0] < j[0])
                        {
                            j[0] += 2;
                            j[1] += 2;
                        }

                    elementsInBrackets = gen.Next(2, scope[1] - scope[0] + 1);

                    //Adding one to consider behavior of insert method and another one to include max value when calculating random value
                    startPosition = gen.Next(scope[0], scope[1] - elementsInBrackets + 1 + 1);

                    //Calculating position of second bracket considering insertion of first bracket
                    endPosition = startPosition + elementsInBrackets + 1;

                    freeBracketsAreas.Remove(scope);

                    if (freeBracketsAreas.Count == 0) generateAgain = true;
                    lexp.Insert(startPosition, new EBrackets("("));
                    lexp.Insert(endPosition, new EBrackets(")"));

                    if (generateAgain)
                    {
                        //Finding out areas are not occupied 
                        int s = -1;
                        for (int j = 0; j < lexp.Count; j++)
                        {
                            //Saving position of beginning of free part of expression
                            if ((j == 0) && lexp[j].text != "(") s = j;
                            else if ((j > 0) && lexp[j - 1].text == ")") s = j;
                            
                            //Position of the end
                            if ((j > 0) && lexp[j].text == "(" && s != -1)
                            {
                                if (j == s || j - 1 == s)
                                {
                                    s = -1;
                                    continue;
                                }
                                freeBracketsAreas.Add(new int[2] { s, j - 1});
                                s = -1;
                            }
                            else if ((j == lexp.Count - 1) && lexp[j].text != ")" && (j != s && j != s - 1))
                            {
                                freeBracketsAreas.Add(new int[2] { s, j });
                                s = -1;
                            }
                                
                        }
                        if (freeBracketsAreas.Count() == 0) break;
                    }
                    i++;
                }
            }

            //Generating operations
            int currentOpIndex;
            bool isInverted;
            bool haveAdded = false;
            for (int i = 0; i < lexp.Count; i++) {
                if (haveAdded)
                {
                    haveAdded = false;
                    continue;
                }
                isInverted = (gen.Next() % 5) == 0;
                if (isInverted) {
                    if (i > 0) {
                        if (lexp[i - 1].text != LogicSymbols.NOT && lexp[i].text != ")" && lexp[i - 1].text != ")") lexp.Insert(i, new EOperation(LogicSymbols.NOT));
                    }
                    else
                    {
                        lexp.Insert(i, new EOperation(LogicSymbols.NOT));
                        continue;
                    }
                }
                
                if (i > 0){
                    bool isLastNOT = false;
                    int putIndex;

                    //Filter conditions
                    if (lexp[i - 1].text == "(" || lexp[i].text == ")") continue;
                    if (lexp[i - 1] is EOperation)
                        if ((lexp[i - 1].text == LogicSymbols.NOT))
                            if (i > 1 && ((lexp[i - 2] is EOperand) || (lexp[i - 2].text == ")")))
                                isLastNOT = true;
                            else continue;
                    do
                    {
                        generateAgain = false;
                        currentOpIndex = gen.Next(0, operationsSet.Length);
                        if (isLastNOT) putIndex = i - 1;
                        else putIndex = i;
                        if (operationsCount.All<int>(p => p == operationsRepeats)) operationsRepeats *= 2;
                        if (operationsCount[currentOpIndex] == operationsRepeats)
                        {
                            generateAgain = true;
                            continue;
                        }
                        else
                        {
                            operationsCount[currentOpIndex]++;
                            lexp.Insert(putIndex, operationsSet[currentOpIndex]);
                        }
                    } while (generateAgain);
                    haveAdded = true;
                }
            }
            lexp.Insert(0, new ESeparateSymbol(true));
            lexp.Add(new ESeparateSymbol(false));
            return new LogicExpression(lexp);
        }

        /*
         *<summary>
         *  Method generates LogicExpression object with specified number of vars.
         *  When method receives zero value, it will behave like the next() method
         *</summary>
         */
        public LogicExpression next(int maxVarsNumber) {
            LogicExpression res;
            if (maxVarsNumber == -1)
            {
                res = next();
                return res;
            }
            int initialV = this.maxVarsNumber;
            if (maxVarsNumber < 2 || this.maxVarsNumber < maxVarsNumber)
                throw new ArgumentOutOfRangeException("maxVarsNumber should be less than" + this.maxVarsNumber + " and more than 2");
            this.maxVarsNumber = (int)maxVarsNumber;
            res = next();
            this.maxVarsNumber = initialV;
            return res;
        }

        /*
         *<summary>
         *  Method generates LogicExpression object equal to object passed into
         *  using equivalent transforms.
         *</summary>
         */
        public LogicExpression nextEqual(LogicExpression exp) {
            String sourceString = exp.ToString();
            LogicExpression res = null;
            int cLimit = laws.Length;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < cLimit; j++)
                {
                    sourceString = Regex.Replace(sourceString, laws[j][0], laws[j][1], RegexOptions.Compiled);
                    if (j >= 7 && j <= 14)
                        sourceString = Regex.Replace(sourceString, @"\(" + operand + @"\)", @"$1", RegexOptions.Compiled);
                    while ((j==5 || j==6 || j==15) && sourceString.IndexOf(LogicSymbols.NOT + LogicSymbols.NOT) > -1)
                        sourceString = Regex.Replace(sourceString, LogicSymbols.NOT + @"(" + LogicSymbols.NOT + @"(?:\w+|\([\w" + LogicSymbols.allSymbols() + @"]+\)))", LogicSymbols.NOT + @"($1)", RegexOptions.Compiled);

                }
                if (i == 0) cLimit -= 3;
            }
            res = new LogicExpression(sourceString);
            //if (!exp.Equals(res)) throw new ArgumentException("Transforms are invalid");
            return res;
        }

        public LogicExpression nextNotEqual(LogicExpression exp) {
            LogicExpression result;
            do
            {
                result = next();
            } while (result.Equals(exp));
            return result;
        }

        public LogicExpression nextNotEqual(LogicExpression exp, int maxVarsNumber)
        {
            LogicExpression result;
            do
            {
                result = next(maxVarsNumber);
            } while (result.Equals(exp));
            return result;
        }
    }
}
