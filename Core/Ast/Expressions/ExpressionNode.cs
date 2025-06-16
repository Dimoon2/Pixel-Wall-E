using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Expressions
{
    public abstract class ExpressionNode : AstNode
    {
        public abstract object Evaluate(Interprete interpreter);
    }
}