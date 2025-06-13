using System;
using System.Collections.Generic; // For List used by some AST nodes
using Interpreter.Core.Ast;
using Interpreter.Core.Ast.Statements;
using Interpreter.Core.Ast.Expressions;
using System.Linq.Expressions;
using Interpreter.Core.Interpreter.Helpers;
namespace Interpreter.Core.Interpreter
{
    public class Interprete
    {

        public Canvas canvas;
        public WallEContext wallEContext;
        public RuntimeEnvironment runtimeEnvironment;
        public SymbolTable symbolTable;

        //collecting errors:
        public List<string> OutputLog { get; }
        public List<string> ErrorLog { get; }

        public Interprete(Canvas canvas, WallEContext wallEContext, SymbolTable symbolTable, RuntimeEnvironment runtimeEnv)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.wallEContext = wallEContext ?? throw new ArgumentNullException(nameof(wallEContext));
            this.symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
            this.runtimeEnvironment = runtimeEnv ?? throw new ArgumentNullException(nameof(runtimeEnv));

            OutputLog = new List<string>();
            ErrorLog = new List<string>();
        }

        public void ExecuteProgram(ProgramNode programNode)
        {
            if (programNode == null || programNode.Statements == null)
            {
                ErrorLog.Add("Interpreter: Cannot execute a null program or program with no statements list.");
                return;
            }

            //reseting
            runtimeEnvironment.Reset();

            //scanning for labels
            try
            { runtimeEnvironment.ScanLabels(programNode); }
            catch (RuntimeException run)
            {
                ErrorLog.Add($"Preprocessing Error: {run.Message}");
                return;
            }

            //execution loop:
            runtimeEnvironment.ProgramCounter = 0;
            while (runtimeEnvironment.ProgramCounter < programNode.Statements.Count)
            {
                if (ErrorLog.Count > 100) //safety break :"(
                {
                    ErrorLog.Add("Interpreter: Too many runtime errors. Halting execution.");
                    break;
                }
                //main method
                StatementNode currentStatement = programNode.Statements[runtimeEnvironment.ProgramCounter];

                try
                {
                    currentStatement.Execute(this);
                }
                catch (RuntimeException run)
                {
                    ErrorLog.Add($"Runtime Error (Statement {runtimeEnvironment.ProgramCounter + 1}): {run.Message}");
                    break;
                }
                catch (Exception ex) //unexpected bugs
                {
                    ErrorLog.Add($"Interpreter Internal Error (Statement {runtimeEnvironment.ProgramCounter + 1}): {ex.Message} - {ex.StackTrace}");
                    break;
                }

                //check GoTo
                if (runtimeEnvironment.GoToPending)
                {
                    runtimeEnvironment.ProcessPendingGoTo();
                }
                else
                {
                    runtimeEnvironment.ProgramCounter++;
                }
            }
            if (ErrorLog.Count == 0)
            {
                OutputLog.Add("Interpreter: Program execution completed.");
            }
            else
            {
                OutputLog.Add($"Interpreter: Program execution finished with {ErrorLog.Count} error(s).");
            }
        }
    }
}