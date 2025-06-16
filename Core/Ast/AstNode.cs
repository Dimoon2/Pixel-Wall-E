using Interpreter.Core;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast
{
        public abstract class AstNode
    {
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}