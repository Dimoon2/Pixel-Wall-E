using System;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    class DrawLineNode : StatementNode
    {
        public ExpressionNode ExpresDirX { get; }
        public ExpressionNode ExpresDirY { get; }
        public ExpressionNode ExpresDistance { get; }

        public DrawLineNode(ExpressionNode expres1, ExpressionNode expres2, ExpressionNode expres3)
        {
            ExpresDirX= expres1 ?? throw new ArgumentNullException(nameof(expres1));
            ExpresDirY= expres2 ?? throw new ArgumentNullException(nameof(expres2));
            ExpresDistance = expres3 ?? throw new ArgumentNullException(nameof(expres3));
        }

        public override string ToString()
        {
            return $"DrawLine(DirX: {ExpresDirX}, DirY: {ExpresDirY}, Distance: {ExpresDistance})";
        }

        public override void Execute(Interprete interpreter)
        {
            throw new NotImplementedException();
        }
    }
}