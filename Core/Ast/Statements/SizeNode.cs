using System;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    class SizeNode : StatementNode
    {
        public ExpressionNode SizeExpression { get; }

        public SizeNode(ExpressionNode numberExpression)
        {
            SizeExpression = numberExpression ?? throw new ArgumentNullException(nameof(numberExpression));
        }

        public override string ToString()
        {
            return $"Size(Value: {SizeExpression})";
        }

        public override void Execute(Interprete interpreter)
        {
            try
            {
                object sizeValue = SizeExpression.Evaluate(interpreter);

                if (!(sizeValue is double sizeDouble))
                {
                    throw new RuntimeException($"Size command expects a numeric argument. Got {sizeValue?.GetType().Name}.");
                }
                int newSize = (int)Math.Round(sizeDouble);

                // Add validation for brush size as per spec (e.g., must be positive)
                if (newSize < 1)
                {
                    throw new RuntimeException($"Brush size must be a positive integer. Got {newSize}.");
                }
                interpreter.wallEContext.CurrentBrushSize = newSize;
                interpreter.OutputLog.Add($"Wall-E brush size set to: {interpreter.wallEContext.CurrentBrushSize}.");
            }
            catch (RuntimeException rex)
            {
                throw new RuntimeException($"Error in Size statement: {rex.Message}");
            }
        }
    }
}