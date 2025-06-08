using Interpreter.Core;
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
    }
}
