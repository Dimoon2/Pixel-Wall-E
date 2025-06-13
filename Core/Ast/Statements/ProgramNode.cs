using System;
using System.Collections.Generic;
using System.Linq; // For Linq's Select and Join, used in ToString()
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    // Represents the entire program or script as a sequence of statements.
    // This will typically be the root node of the AST.
    public class ProgramNode : AstNode // It's a high-level structure, AstNode is a fitting base.
    {
        public List<StatementNode> Statements { get; }

        public ProgramNode(List<StatementNode> statements)
        {
            Statements = statements ?? new List<StatementNode>();
        }

        // Override ToString for a more readable representation of the program structure.
        public override string ToString()
        {
            if (Statements.Count == 0 )
            {
                return "ProgramNode(No Statements)";
            }
            // Indent statements for better readability
            var statementStrings = Statements.Select(s =>
                s.ToString().Replace("\n", "\n  ")); // Indent multi-line statement strings
            return $"ProgramNode([\n  {string.Join(",\n  ", statementStrings)}\n])";
        }
    }
}