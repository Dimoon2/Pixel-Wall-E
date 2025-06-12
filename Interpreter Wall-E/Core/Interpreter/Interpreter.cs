using System;
using System.Collections.Generic; // For List used by some AST nodes
using Interpreter.Core.Ast;
using Interpreter.Core.Ast.Statements;
using Interpreter.Core.Ast.Expressions;
using System.Linq.Expressions;
using Interpreter.Core.Interpreter.Helpers;
namespace Interpreter.Core.Interpreter
{
    public class Interpreter
    {
        private Canvas canvas;
        private WallEContext wallEContext;
        private RuntimeEnvironment runtimeEnvironment;
        private SymbolTable symbolTable;

        //collecting errors:
        public List<string> OutputLog { get; }
        public List<string> ErrorLog { get; }

        public Interpreter(Canvas canvas, WallEContext wallEContext, SymbolTable symbolTable, RuntimeEnvironment runtimeEnv)
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

                StatementNode currentStatement = programNode.Statements[runtimeEnvironment.ProgramCounter];

                try
                {
                    ExecuteStatement(currentStatement);
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

        public void ExecuteStatement(StatementNode statement)
        {
            switch (statement)
            {
                case SpawnNode spawnNode:
                    VisitSpawnNode(spawnNode);
                    break;
                case ColorNode colorNode:
                    VisitColorNode(colorNode);
                    break;
                case SizeNode sizeNode:
                    VisitSizeNode(sizeNode);
                    break;
                case DrawLineNode drawLineNode:
                    VisitDrawLineNode(drawLineNode);
                    break;
                case DrawCircleNode drawCircleNode:
                    VisitDrawCircleNode(drawCircleNode);
                    break;
                case DrawRectangleNode drawRectangleNode:
                    VisitDrawRectangleNode(drawRectangleNode);
                    break;
                case FillNode fillNode:
                    VisitFillNode(fillNode);
                    break;
                case AssignmentNode assignmentNode:
                    VisitAssignmentNode(assignmentNode);
                    break;
                // case LabelNode labelNode:
                //     VisitLabelNode(labelNode); // Labels are mostly for scanning, execution is a no-op
                //     break;
                // case GoToNode goToNode:
                //     VisitGoToNode(goToNode);
                //    break;

                default: throw new RuntimeException($"Unsupported statement type: {statement.GetType().Name}");
            }
        }
        private void VisitSpawnNode(SpawnNode node)
        {
            OutputLog.Add($"Executing: Spawn({node.XCoordinate}, {node.YCoordinate})");
            // TODO: Evaluate expressions node.XCoordinate and node.YCoordinate
            // TODO: Update wallEContext.CurrentX and wallEContext.CurrentY
            // TODO: Check canvas bounds (optional, Spawn itself might not draw)
        }

        private void VisitColorNode(ColorNode node)
        {
            OutputLog.Add($"Executing: Color({node.ColorExpression})");
            // TODO: Evaluate node.ColorExpression (should result in a string or PixelColor)
            // TODO: Update wallEContext.CurrentBrushColor
        }

        private void VisitSizeNode(SizeNode node)
        {
            OutputLog.Add($"Executing: Size({node.SizeExpression})");
            // TODO: Evaluate node.SizeExpression (should result in an int)
            // TODO: Update wallEContext.CurrentBrushSize
        }

        private void VisitDrawLineNode(DrawLineNode node)
        {
            OutputLog.Add($"Executing: DrawLine(...)");
            // TODO: Evaluate expressions
            // TODO: Call canvas drawing logic using wallEContext
        }
        private void VisitDrawCircleNode(DrawCircleNode node) { OutputLog.Add($"Executing: DrawCircle(...)"); /* TODO */ }
        private void VisitDrawRectangleNode(DrawRectangleNode node) { OutputLog.Add($"Executing: DrawRectangle(...)"); /* TODO */ }
        private void VisitFillNode(FillNode node) { OutputLog.Add($"Executing: Fill()"); /* TODO */ }

        private void VisitAssignmentNode(AssignmentNode node)
        {
            OutputLog.Add($"Executing: {node.Identifier.Value} <- {node.ValueExpression}");
            // TODO: Evaluate node.ValueExpression
            // TODO: Store result in symbolTable.Define(node.Identifier.Value, result)
        }

        // private void VisitLabelNode(LabelNode node)
        // {
        //     // Execution of a label statement is a no-op.
        //     // Labels are processed during the ScanForLabels phase.
        //     OutputLog.Add($"Encountered Label: {node.LabelToken.Value}");
        // }

        // private void VisitGoToNode(GoToNode node)
        // {
        //     OutputLog.Add($"Executing: GoTo [{node.TargetLabel.Value}] ({node.Condition})");
        //     // TODO: Evaluate node.Condition (should result in a boolean or number treated as boolean)
        //     // TODO: If condition is true, call runtimeEnvironment.RequestGoTo(node.TargetLabel.Value)
        // }

        ////////////\\\\\\\\\\\\\\Expression Evaluation\\\\\\\\\\\\/////////////////
        private Object EvaluateExpression(ExpressionNode expression)
        {
            if (expression == null)
            {
                throw new RuntimeException("Cannot evaluate a null expression.");
            }
            switch (expression)
            {
                case NumberLiteralNode numberNode:
                    return VisitNumberNode(numberNode);

                case StringLiteralNode stringNode:
                    return VisitStringLiteralNode(stringNode);

                case VariableNode variableNode:
                    return VisitVariableNode(variableNode);

                case BinaryOpNode binaryOpNode:
                    return VisitBinaryOpNode(binaryOpNode);

                    case FunctionCallNode functionCallNode:
                  //  return VisitFunctionCallNode(functionCallNode);
                default:
                    throw new RuntimeException($"Unsupported expression type: {expression.GetType().Name}");
            }
        }

        private object VisitNumberNode(NumberLiteralNode node)
        {
            if (node.NumberToken.Literal is int intVal)
            {
                return (double)intVal;
            }
            throw new RuntimeException($"Invalid literal for NumberToken: {node.NumberToken.Literal}");
        }

        private object VisitStringLiteralNode(StringLiteralNode node)
        {
            return node.StringToken.Literal?.ToString() ?? ""; //?
        }

        private object VisitVariableNode(VariableNode node)
        {
            return symbolTable.Get(node.IdentifierToken.Value);
        }


        private object VisitBinaryOpNode(BinaryOpNode node)
        {
            object leftValue = EvaluateExpression(node.Left);

            // Handle && and || here
            if (node.Operator.Type == TokenType.And)
            {
                if (!BinaryOperations.ConvertToBooleanStatic(leftValue)) return false;
                object rightValueAnd = EvaluateExpression(node.Right);
                return BinaryOperations.ConvertToBooleanStatic(rightValueAnd);
            }
            if (node.Operator.Type == TokenType.Or)
            {
                if (BinaryOperations.ConvertToBooleanStatic(leftValue)) return true;
                object rightValueOr = EvaluateExpression(node.Right);
                return BinaryOperations.ConvertToBooleanStatic(rightValueOr);
            }

            // For other operators, evaluate right operand and use the handler
            object rightValue = EvaluateExpression(node.Right);

            if (BinaryOperations.TryGetHandler(node.Operator.Type, out BinaryOperationHandler handler))
            {
                try
                {
                    return handler(leftValue, rightValue);
                }
                catch (RuntimeException) { throw; } // Re-throw exceptions
                catch (Exception) // Catch other unexpected errors from handlers
                {
                    throw; // new RuntimeException($"Error during binary operation '{node.Operator.Value}': {ex.Message}", ex);
                }
            }

            throw new RuntimeException($"Unsupported binary operator: {node.Operator.Type}");
        }

        // private object VisitFunctionCallNode(FunctionCallNode node)
        // {

        // }
    }
}