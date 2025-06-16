using System;
using System.Diagnostics;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    public class SpawnNode : StatementNode
    {
        public ExpressionNode XCoordinate { get; }
        public ExpressionNode YCoordinate { get; }
        public SpawnNode(ExpressionNode xCoordinate, ExpressionNode yCoordinate)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
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

                if (xVal > interpreter.canvas.Size || yVal > interpreter.canvas.Size || xVal < 0 || yVal < 0)
                {
                    throw new RuntimeException("Coordinate X or Y out of the bounds of the canvas");
                }
                if (interpreter.wallEContext.IsSpawned)
                {
                    throw new RuntimeException("Wall-E can only be spawned once"); 
                }
                interpreter.wallEContext.X = xVal;
                interpreter.wallEContext.Y = yVal;
                interpreter.wallEContext.IsSpawned = true;
                Debug.WriteLine($"WAlle spawned at {xVal} , {yVal}");
                interpreter.OutputLog.Add($"Executing: Spawn({XCoordinate}, {YCoordinate})");
            }
            catch (RuntimeException ex)
            {
                throw new RuntimeException($"Error in Spawn statement: {ex.Message}");
            }
        }
    }
}