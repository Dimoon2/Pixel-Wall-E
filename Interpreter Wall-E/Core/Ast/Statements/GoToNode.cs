using System.Diagnostics;
using Interpreter.Core.Ast.Expressions;

namespace Interpreter.Core.Ast.Statements
{
    class GoToNode : StatementNode
    {
        public Token LabelToken { get; }
        public string LabelName => LabelToken.Value;
        public ExpressionNode Condition { get; }

        //Uncotiditional GoTo
        public GoToNode(Token labelToken) : this(labelToken, null!) { }
        //Cotiditional GoTo
        public GoToNode(Token labelToken, ExpressionNode condition)
        {
            if (labelToken.Type != TokenType.Identifier)
            {
                throw new ArgumentException("GoToNode's labelToken must be an IDENTIFIER token.", nameof(labelToken));
            }
            LabelToken = labelToken;
            Condition = condition;
        }
        public override string ToString()
        {
            //   //  GoTo [label] (condition)
            if (Condition == null) return $"GoTo[{LabelName}]";
            return $"GoTo[{LabelName}] ({Condition}) )";
        }
    }
}