using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Logic
{
    public class LogicExpression
    {
        public Queue<ExpressionPart> body; 
        public Stack<ExpressionPart> California;
        private Stack<ExpressionPart> Texas;

        public List<EOperand> operands = new List<EOperand>();
        public TruthTable truthTable;

        public EOperand solution;

        private bool isRPNcreated = false;
        private bool isCalculated = false;

        public LogicExpression(String strExpression) 
        {
            //Parsing expression from string to set of ExpressionPart objects
            body = new Queue<ExpressionPart>();
            String delimiters = LogicSymbols.OR + LogicSymbols.AND + LogicSymbols.IMP + LogicSymbols.NOT;
            String[] parts = Regex.Split(strExpression, @"(?=[" + delimiters + ")(]|)|(?<=[" + delimiters + ")(])");
            body.Enqueue(new ESeparateSymbol(true));

            ExpressionPart x;
            foreach (String i in parts){
               if (EBrackets.isSuitablePart(i)) body.Enqueue(new EBrackets(i));
               else if (EOperation.isSuitablePart(i)){
                   x = new EOperation(i);
                   body.Enqueue(x);
               }
               else if (EOperand.isSuitablePart(i)){
                   x = new EOperand(i);
                   if (operands.Contains(x) && !((EOperand)x).isConstant)
                   {
                       x = operands.First<EOperand>(p => p.Equals(x));
                   }
                   if (!operands.Contains(x) && !((EOperand)x).isConstant) operands.Add((EOperand)x);
                   body.Enqueue((EOperand)x);
               }
            }
            body.Enqueue(new ESeparateSymbol(false));
            this.operands = this.operands.OrderBy<EOperand, String>(p => p.text).ToList<EOperand>();
        }

        public LogicExpression(List<ExpressionPart> body) {
            for (int i = 0; i < body.Count; i++)
                if (body[i] is EOperand)
                    if (((EOperand)body[i]).isPartOfPattern) throw new ArgumentException("There are pattern elements in body array. Delete all pattern operands.");
                    else if (operands.Contains(body[i]) && !((EOperand)body[i]).isConstant)
                        body[i] = operands.First<EOperand>(p => p.Equals(body[i]));
                    else if (!operands.Contains(body[i]) && !((EOperand)body[i]).isConstant) operands.Add((EOperand)body[i]);

            this.body = new Queue<ExpressionPart>(body);
            this.operands = this.operands.OrderBy<EOperand, String>(p => p.text).ToList<EOperand>();
        }

        public LogicExpression(LogicExpression leftExpr, LogicExpression rightExpr, EOperation op) {
            //Creating new "body" sequence with two existing sequences separated with EQU operation
            List<ExpressionPart> inputLeft = new List<ExpressionPart>(leftExpr.body);
            List<EOperand> newOperands = new List<EOperand>(leftExpr.operands);
            inputLeft[inputLeft.Count - 1] = new EBrackets(")");
            inputLeft.Insert(1, new EBrackets("("));

            inputLeft.Add(op);

            List<ExpressionPart> inputRight = new List<ExpressionPart>(rightExpr.body);
            List<EOperand> rightOperands = new List<EOperand>(rightExpr.operands);
            inputRight[0] = new EBrackets("(");
            inputRight.Insert(inputRight.Count - 1, new EBrackets(")"));

            inputLeft.AddRange(inputRight);
            newOperands.AddRange(rightOperands);

            //Removing vars repeating
            for (int i = 0; i < newOperands.Count; i++)
                for (int j = i + 1; j < newOperands.Count; j++)
                    if (newOperands[i].Equals(newOperands[j])) newOperands.RemoveAt(j);

            //Recovering reference connection between .body and .operands
            for (int i = 0; i < inputLeft.Count; i++)
            {
                if (!(inputLeft[i] is EOperand)) continue;
                for (int j = 0; j < newOperands.Count; j++)
                    if (inputLeft[i].Equals(newOperands[j]) && !Object.ReferenceEquals(inputLeft, newOperands)) inputLeft[i] = newOperands[j];
            }

            this.body = new Queue<ExpressionPart>(inputLeft);
            this.operands = newOperands;
        }

        //Method generates reversed polish record using existing set of ExpressionPart objects
        public void makeRPolishRecord() {
            bool repeatChoice = false;
            //Stack with final result 
            California = new Stack<ExpressionPart>();

            //Buffer stack for storing intermediate data
            Texas = new Stack<ExpressionPart>();

            foreach (ExpressionPart i in body) { 
                if (Texas.Count == 0) {
                    Texas.Push(i);
                    continue;
                }

                do
                {
                    /* 
                     * Put part of the initial expression in either California or Texas stack
                     * considering top element in Texas Stack
                     */
                    repeatChoice = false;
                    switch (i.compare(Texas.Peek()))
                    {
                        case 0:
                            California.Push(i);
                            break;

                        case 1:
                            Texas.Push(i);
                            break;

                        case 2:
                            while (i.compare(Texas.Peek()) == 2)
                                California.Push(Texas.Pop());
                            repeatChoice = true;
                            break;

                        case 3:
                            Texas.Pop();
                            break;

                        case 4:
                            break;
                    }
                } while (repeatChoice);
            }
            isRPNcreated = true;
        }

        //Method finds solution of logic expression using RPN
        public EOperand calculate() {
            if (!isRPNcreated) throw new InvalidOperationException("Reversed polish notation hasn't been created yet");

            Stack<ExpressionPart> calcField = new Stack<ExpressionPart>();
            Stack<ExpressionPart> polishNotation = new Stack<ExpressionPart>(California);
            while (polishNotation.Count > 0)
            {
                //Selecting operands
                while ((polishNotation.Count > 0) && (polishNotation.Peek() is EOperand))
                    calcField.Push(polishNotation.Pop());

                //Looking for operators and computing
                while ((polishNotation.Count > 0) && (polishNotation.Peek() is EOperation))
                {
                    EOperation op = null;
                    try
                    {
                        op = (EOperation)polishNotation.Pop();
                        op.commit(calcField);
                    }
                    catch (InvalidOperationException) {
                        throw new FormatException("Math expression isn't correct");
                    }
                }
            }

            this.isCalculated = true;
            this.solution = (EOperand)calcField.Pop();
            return this.solution;
        }

        //Overloaded method for setting bool values for variables 
        public EOperand calculate(params bool[] x){
            if (x.Length != operands.Count) throw new InvalidOperationException("Some of operands doesn't have its value(value property == null)");
            
            //Set input values for existing operands
            for (int i = 0; i < x.Length; i++)
                operands[i].value = x[i];
            return calculate();
        }

        public void generateTruthTable() {
            if (!isRPNcreated) throw new InvalidOperationException("Reversed polish notation hasn't been created yet");
            truthTable = new TruthTable(operands.Count);

            //Using for generating different sets of bool values to fill table with simple vars 
            int topVariantVal = (int)Math.Pow(2, operands.Count);
            String currentVariantBin;

            for (int i = 0; i < topVariantVal; i++) {
                currentVariantBin = Convert.ToString(i, 2);
                currentVariantBin = new String('0', operands.Count - currentVariantBin.Length) + currentVariantBin;
                truthTable.truthTableSimple[i] = currentVariantBin.Select<char, bool>(chr => chr == '1').ToArray();
                truthTable.truthTableExpression[i] = (bool)(this.calculate(truthTable.truthTableSimple[i])).value;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is LogicExpression)) return false;
            //Declare LogicExpression
            LogicExpression x = new LogicExpression(this, (LogicExpression)obj, new EOperation(LogicSymbols.EQU));
            x.makeRPolishRecord();
            x.generateTruthTable();
            return x.truthTable.isTautology();
        }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            foreach (ExpressionPart i in this.body)
                if (!(i is ESeparateSymbol)) res.Append(i.ToString());
            return res.ToString();
        }
    }
}
