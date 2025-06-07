namespace Interpreter.Core.Ast.Expressions
{
    // Base class for all AST nodes that represent expressions.
    // Expressions are parts of the code that evaluate to a value (e.g., 5, "Red", n + 1, GetActualX()).
    public abstract class ExpressionNode : AstNode
    {
        // Inherits from AstNode.
        // Specific expression types (like NumberLiteralNode, BinaryOpNode) will derive from this.
    }
}