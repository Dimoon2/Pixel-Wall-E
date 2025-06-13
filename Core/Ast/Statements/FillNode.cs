using System;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    class FillNode : StatementNode
    {
        public FillNode(){}

        public override string ToString()
        {
            return $"Fill()";
        }

        public override void Execute(Interprete interpreter)
        {
            throw new NotImplementedException();
        }
    }
}