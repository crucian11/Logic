# Logic
Provides a convenient way of representing logic expressions. It also allows you to perform basic operations with logic expressions(generate a truth table, find the solution with a given tuple of values).
##Usage
####LogicExpression class
First of all, you should instantiate a LogicExpression class by passing string representation into its constructor:
```c#
LogicExpression expression = new LogicExpression("p" + LogicSymbols.AND + "q" + LogicSymbols.EQU + "r");
```
Note that you can use either a set of LogicSymbols constants or raw unicode symbols, while creating an object:
```c#
LogicExpression expression2 = new LogicExpression("p∧q→r");
```
There are four operations supported by this library:
- conjunction (`LogicSymbols.AND` or \u2227 unicode symbol);
- disjunction (`LogicSymbols.OR` or \u2227 unicode symbol);
- implication (`LogicSymbols.IMP` or \u2227 unicode symbol);
- equivalence (`LogicSymbols.EQU` or \u2227 unicode symbol).

Before performing some operation, a reversed polish notation should be created, you can do it by calling makeRPolishRecord method:
```c#
expression.makeRPolishRecord();
```
This method creates a notation and store it in a California property. After that, this property is being used in other methods of the class.
Let's see all the rest of the methods:
- `EOperand calculate()` method performs a computation with existing values stored in a operands property;
- `EOperand calculate(params bool[] x)` method performs a computation with given tuple of boolean values;
- `TruthTable generateTruthTable()` method generates a truth table.

#### LEGenerator class
First of all, you should create an object-generator:
```c#
LEGenerator g = new LEGenerator();
```
LEGenerator class has an interface that's similar to a Random class. There are few methods in the class:
- `LogicExpression next()` generates a random logic expression;
- `LogicExpression next(int maxVarsNumber)` does as same as the `next` method, but it allows you to limit a max amount of variables;
- `LogicExpression nextEqual(LogicExpression exp)` alters the expression using propositional logic transformation rules;
- `LogicExpression nextNotEqual(LogicExpression exp)` generates a not equal expression to the passed one;
- `LogicExpression nextNotEqual(LogicExpression exp, int maxVarsNumber)` does as same as the `nextNotEqual` method with limitation of a max amount of variables.

Also, there are few additional classes in the library:
- ExpressionPart represents a single part of logic expression(bracket, operation, operand ect.);
- EOperand is one of the ExpressionPart derived classes, represents a particular operand in logic expression, there's a boolean "value" property that's used in calculate methods;
- TruthTable represents truth table for a particular logic expression, contains method isTautology() for its further analyzying
