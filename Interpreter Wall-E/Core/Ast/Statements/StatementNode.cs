using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    public abstract class StatementNode : AstNode
    {
        public abstract void Execute(Interprete interpreter);
        
    }
}