using System;
using Avalonia.Media;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
using Interpreter.Core.Interpreter.Helpers;
namespace Interpreter.Core.Ast.Statements
{
    class ColorNode : StatementNode
    {
        public ExpressionNode ColorExpression { get; }

        public ColorNode(ExpressionNode colorExpression)
        {
            ColorExpression = colorExpression ?? throw new ArgumentNullException(nameof(colorExpression));
        }

        public override string ToString()
        {
            return $"Color(Value: {ColorExpression})";
        }

        public override void Execute(Interprete interpreter)
        {
            try
            {
                object colorValue = ColorExpression.Evaluate(interpreter);
                if (!(colorValue is string colorName))
                {
                    throw new RuntimeException($"Color command expects a string argument (color name). Got {colorValue?.GetType().Name}.");
                }
                string _color = colorName.ToLower();
                Color color = FunctionHandlers.GetColor(_color);
                interpreter.wallEContext.BrushColor = color;
                interpreter.OutputLog.Add($"Wall-E brush color set to: {interpreter.wallEContext.BrushColor}.");
            }
            catch (RuntimeException rex)
            {
                throw new RuntimeException($"Error in Color statement: {rex.Message}");
            }
        }
    }
}