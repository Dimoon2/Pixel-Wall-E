using System.Collections.Generic; // For List
using System.Linq; // For Linq operations in ToString
using Interpreter.Core;

namespace Interpreter.Core.Ast.Expressions
{
    class FunctionCallNode : ExpressionNode
    {
        public Token FunctionNameToken { get; } // The IDENTIFIER or KEYWORD_XXX token for the function name
        public string FunctionName => FunctionNameToken.Value;
        public List<ExpressionNode> Arguments { get; }

        public FunctionCallNode(Token functionNameToken, List<ExpressionNode> arguments)
        {
            FunctionNameToken = functionNameToken ?? throw new ArgumentNullException(nameof(functionNameToken));
            Arguments = arguments ?? new List<ExpressionNode>();
        }

        public override string ToString()
        {
            string argsString = Arguments.Any()? string.Join(", ", Arguments.Select(a => a.ToString())) : "";
            return $"{FunctionName}({argsString})";
        }
    }
}