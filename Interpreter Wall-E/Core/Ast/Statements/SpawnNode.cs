using Interpreter.Core.Ast.Expressions; 

namespace Interpreter.Core.Ast.Statements
{
    public class SpawnNode : StatementNode
    {
        // The Spawn command takes two arguments, which are expressions that should evaluate to numbers.
        public ExpressionNode XCoordinate { get; }
        public ExpressionNode YCoordinate { get; }

        public SpawnNode(ExpressionNode xCoordinate, ExpressionNode yCoordinate)
        {
            XCoordinate = xCoordinate ?? throw new ArgumentNullException(nameof(xCoordinate));
            YCoordinate = yCoordinate ?? throw new ArgumentNullException(nameof(yCoordinate));
        }

        public override string ToString()
        {
            return $"Spawn(X: {XCoordinate}, Y: {YCoordinate})";
        }
    }
}