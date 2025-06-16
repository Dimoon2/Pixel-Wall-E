using System;
using System.Data.Common;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;

namespace Interpreter.Core.Ast.Statements
{
    class AssignmentNode : StatementNode
    {
        public Token Identifier { get; }
        public string Name => Identifier.Value;
        public ExpressionNode ValueExpression { get; }

        public AssignmentNode(Token identifier, ExpressionNode valueExpression)
        {
            if (identifier.Type != TokenType.Identifier)
            {
               return;
               // throw new ArgumentException("AssignmentNode requires an IDENTIFIER token for the variable name.", nameof(identifier));
            }
            Identifier = identifier;
            ValueExpression = valueExpression;
        }

        public override string ToString()
        {
            return $"Assign(Var: {Name}, Value: ({ValueExpression}))";
        }

        public override void Execute(Interprete interpreter)
        {
             try
            {
                string variableName = Identifier.Value;
                object valueToAssign = ValueExpression.Evaluate(interpreter);
                interpreter.symbolTable.Assign(variableName, valueToAssign);
                interpreter.OutputLog.Add($"Assigned to '{variableName}': {valueToAssign ?? "null"}.");
            }
            catch (RuntimeException rex)
            {
                throw new RuntimeException($"Error in Assignment statement: {rex.Message}");
            }
        }
    }
}