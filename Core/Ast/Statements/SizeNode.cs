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
            SizeExpression = numberExpression;
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

                if (!(sizeValue is int size))
                {
                    throw new RuntimeException($"Size command expects a numeric argument. Got {sizeValue?.GetType().Name}.");
                }
                
                if (size < 1)
                {
                    throw new RuntimeException($"Brush size must be a positive integer. Got {size}.");
                }
                if (size % 2 == 0) { size = size - 1;}
                
                interpreter.wallEContext.BrushSize = size;
                interpreter.OutputLog.Add($"Wall-E brush size set to: {interpreter.wallEContext.BrushSize}.");
            }
            catch (RuntimeException rex)
            {
                throw new RuntimeException($"Error in Size statement: {rex.Message}");
            }
        }
    }
}