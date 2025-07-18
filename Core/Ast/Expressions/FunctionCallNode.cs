using System.Collections.Generic; 
using System.Linq; 
using Interpreter.Core;
using System;
using Interpreter.Core.Interpreter;
using Interpreter.Core.Interpreter.Helpers;
namespace Interpreter.Core.Ast.Expressions
{
    class FunctionCallNode : ExpressionNode
    {
        public Token FunctionNameToken { get; } 
        public string FunctionName => FunctionNameToken.Value;
        public List<ExpressionNode> Arguments { get; }

        public FunctionCallNode(Token functionNameToken, List<ExpressionNode> arguments)
        {
            FunctionNameToken = functionNameToken;
            Arguments = arguments;
        }

        public override string ToString()
        {
            string argsString = Arguments.Any() ? string.Join(", ", Arguments.Select(a => a.ToString())) : "";
            return $"{FunctionName}({argsString})";
        }

        public override object Evaluate(Interprete interpreter)
        {
            var evaluatedArgs = new List<object>();
            foreach (var argExpr in Arguments)
            {
                evaluatedArgs.Add(argExpr.Evaluate(interpreter));
            }

            // Use the FunctionHandlers helper class
            if (FunctionHandlers.TryGetHandler(FunctionNameToken.Type, out BuiltInFunctionHandler handler))
            {
                try
                {
                    return handler(evaluatedArgs, interpreter.wallEContext, interpreter.canvas, interpreter.symbolTable);
                }
                catch (RuntimeException) { throw; } // Re-throw our specific exceptions
                catch (Exception ex) // Catch other unexpected errors from handlers
                {
                    throw new RuntimeException($"Error during function call '{FunctionNameToken.Value}': {ex.Message}");
                }
            }
            throw new RuntimeException($"Unknown built-in function keyword: {FunctionNameToken.Type} ('{FunctionNameToken.Value}')");
        }
    }
}
