using System.Data.Common;
using Interpreter.Core.Ast.Expressions;

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
                // Internal consistency check: Parser should ensure this.
                throw new ArgumentException("AssignmentNode requires an IDENTIFIER token for the variable name.", nameof(identifier));
            }
            Identifier = identifier;
            ValueExpression = valueExpression ?? throw new ArgumentNullException(nameof(valueExpression));
        }

        public override string ToString()
        {
            return $"Assign(Var: {Name}, Value: ({ValueExpression}))";
        }
    }
}