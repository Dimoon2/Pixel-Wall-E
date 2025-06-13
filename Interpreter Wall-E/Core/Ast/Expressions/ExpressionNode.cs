using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Expressions
{
    // Base class for all AST nodes that represent expressions.
    // Expressions are parts of the code that evaluate to a value (e.g., 5, "Red", n + 1, GetActualX()).
    public abstract class ExpressionNode : AstNode
    {
        public abstract object Evaluate(Interprete interpreter);
    }
}