using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    class LabelNode : StatementNode
    {
        public Token IdentifierToken { get; }
        public string Name => IdentifierToken.Value;

        public LabelNode(Token identifierToken)
        {
            if (identifierToken.Type != TokenType.Identifier)
            {
                throw new ArgumentException("LabelNode requires an IDENTIFIER token for the label name.", nameof(identifierToken));
            }
            IdentifierToken = identifierToken;
        }

        public override string ToString()
        {
            return $"Label(Name: {Name})";
        }

        public override void Execute(Interprete interpreter)
        {
             // Labels are processed during the ScanForLabels phase.
            interpreter.OutputLog.Add($"Encountered Label: {IdentifierToken.Value}");
        }
    }
}