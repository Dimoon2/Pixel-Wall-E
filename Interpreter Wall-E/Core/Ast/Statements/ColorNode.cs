using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
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
                interpreter.wallEContext.CurrentBrushColor = new PixelColor(colorName);
                interpreter.OutputLog.Add($"Wall-E brush color set to: {interpreter.wallEContext.CurrentBrushColor.Name}.");
            }
            catch (RuntimeException rex)
            {
                throw new RuntimeException($"Error in Color statement: {rex.Message}");
            }
        }
    }
}