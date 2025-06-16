using System;
using System.Collections.Generic;
using System.Linq; 
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    public class ProgramNode : AstNode
    {
        public List<StatementNode> Statements { get; }

        public ProgramNode(List<StatementNode> statements)
        {
            Statements = statements ?? new List<StatementNode>();
        }

        public override string ToString()
        {
            if (Statements.Count == 0 )
            {
                return "ProgramNode(No Statements)";
            }
            
            var statementStrings = Statements.Select(s =>
                s.ToString().Replace("\n", "\n  ")); 
            return $"ProgramNode([\n  {string.Join(",\n  ", statementStrings)}\n])";
        }
    }
}