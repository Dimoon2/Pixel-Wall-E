using Interpreter.Core;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast
{
        public abstract class AstNode
    {
        // We might add common properties or methods here later,
        // for example, for a Visitor pattern or source code location.
        // For now, a simple ToString() can be helpful for debugging.
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}