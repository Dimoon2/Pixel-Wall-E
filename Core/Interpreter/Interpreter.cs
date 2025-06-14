using System;
using System.Collections.Generic; // For List used by some AST nodes
using Interpreter.Core.Ast;
using Interpreter.Core.Ast.Statements;
using Interpreter.Core.Ast.Expressions;
using System.Linq.Expressions;
using Interpreter.Core.Interpreter.Helpers;
using PixelWallEApp.Models;
using PixelWallEApp.Models.Canvas;

namespace Interpreter.Core.Interpreter
{
    public class Interprete
    {

        public CanvasState canvas;
        public WallEState wallEContext;
        public RuntimeEnvironment runtimeEnvironment;
        public SymbolTable symbolTable;

        //collecting errors:
        public List<string> OutputLog { get; }
        public List<string> ErrorLog { get; }

        public Interprete(CanvasState canvas, WallEState wallEContext)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.wallEContext = wallEContext ?? throw new ArgumentNullException(nameof(wallEContext));
            this.symbolTable = new SymbolTable();
            this.runtimeEnvironment = new RuntimeEnvironment();
            symbolTable = new SymbolTable();

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
            {
                runtimeEnvironment.ScanLabels(programNode);
                StatementNode firstStatement = programNode.Statements[0];
                if (firstStatement is not SpawnNode)
                {
                    throw new RuntimeException("Spawn must be the fist instruction in wallE language");
                }
            }
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
                    //  if(currentStatement is SpawnNode && wallEContext.IsSpawned){throw new RuntimeException("Spawn must be the fist instruction in wallE language");}
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
                OutputLog.Add($"Interpreter: Program execution completed. Walle current position:({wallEContext.X} , {wallEContext.Y})");
            }
            else
            {
                OutputLog.Add($"Interpreter: Program execution finished with {ErrorLog.Count} error(s), Walle current position:({wallEContext.X} , {wallEContext.Y})");
            }
        }
    }
}