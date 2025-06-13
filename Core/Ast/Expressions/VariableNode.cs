using System;
using Interpreter.Core;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Expressions
{
    class VariableNode : ExpressionNode
    {
        public Token IdentifierToken { get; }
        public string Name => IdentifierToken.Value;
        public VariableNode(Token identifierToken)
        {

            if (identifierToken == null)
            {
                throw new ArgumentNullException(nameof(identifierToken), "CRITICAL: identifierToken parameter was null in VariableNode constructor.");
            }
            IdentifierToken = identifierToken;
        }

        public override string ToString()
        {
            return $"Variable: {Name}";
        }

        public override object Evaluate(Interprete interpreter)
        {
            return interpreter.symbolTable.Get(Name);
        }
    }
}
