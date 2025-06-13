using System;
using System.Diagnostics;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
using Interpreter.Core.Interpreter.Helpers;
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
            return $"GoTo[{LabelName}] ({Condition})";
        }

        public override void Execute(Interprete interpreter)
        {
            try
            {
                string targetLabelName = LabelToken.Value;
                object condition =Condition.Evaluate(interpreter);
                bool conditionResult = BinaryOperations.ConvertToBooleanStatic(condition);

                interpreter.OutputLog.Add($"Executing GoTo [{targetLabelName}] with condition ({Condition}) evaluated to {conditionResult}.");

                if (conditionResult)
                {
                    interpreter.runtimeEnvironment.RequestGoTo(targetLabelName);
                    interpreter.OutputLog.Add($"GoTo [{targetLabelName}] requested.");
                }
                else
                {
                    interpreter.OutputLog.Add($"GoTo [{targetLabelName}] condition false. No jump.");
                }
            }
            catch (RuntimeException rex)
            {
                throw new RuntimeException($"Error in GoTo statement targeting '{LabelToken.Value}': {rex.Message}");
            }
        }
    }
}