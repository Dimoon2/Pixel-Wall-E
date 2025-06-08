using Interpreter.Core.Ast.Expressions;

namespace Interpreter.Core.Ast.Statements
{
    class FillNode : StatementNode
    {
        public FillNode(){}

        public override string ToString()
        {
            return $"Fill()";
        }
    }
}