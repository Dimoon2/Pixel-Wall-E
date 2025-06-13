using System;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
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

        public override void Execute(Interprete interpreter)
        {
            try
            {
                object X = XCoordinate.Evaluate(interpreter);
                object Y = YCoordinate.Evaluate(interpreter);
                if (!(X is int xVal))
                    throw new RuntimeException($"Spawn X coordinate must be a number. Got {X?.GetType().Name}.");
                if (!(Y is int yVal))
                    throw new RuntimeException($"Spawn Y coordinate must be a number. Got {Y?.GetType().Name}.");

                if (xVal > interpreter.canvas.Width || yVal > interpreter.canvas.Width)
                {
                    throw new RuntimeException("Coordinate X or Y out of the bounds of the canvas");
                }

                interpreter.wallEContext.CurrentX = xVal;
                interpreter.wallEContext.CurrentY = yVal;
                interpreter.OutputLog.Add($"Executing: Spawn({XCoordinate}, {YCoordinate})");
            }
            catch (RuntimeException ex)
            {
                throw new RuntimeException($"Error in Spawn statement: {ex.Message}");
            }
        }
    }
}