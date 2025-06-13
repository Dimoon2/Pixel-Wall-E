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
                case LabelNode labelNode:
                    VisitLabelNode(labelNode);
                    break;
                case GoToNode goToNode:
                    VisitGoToNode(goToNode);
                    break;

                default: throw new RuntimeException($"Unsupported statement type: {statement.GetType().Name}");
            }
        }
        private void VisitSpawnNode(SpawnNode node)
        {
            try
            {
                object XCoordinate = EvaluateExpression(node.XCoordinate);
                object YCoordinate = EvaluateExpression(node.YCoordinate);
                if (!(XCoordinate is double xVal))
                    throw new RuntimeException($"Spawn X coordinate must be a number. Got {XCoordinate?.GetType().Name}.");
                if (!(YCoordinate is double yVal))
                    throw new RuntimeException($"Spawn Y coordinate must be a number. Got {YCoordinate?.GetType().Name}.");

                if ((int)Math.Round(xVal) > canvas.Width || (int)Math.Round(yVal) > canvas.Width)
                {
                    throw new RuntimeException("Coordinate X or Y out of the bounds of the canvas");
                }

                wallEContext.CurrentX = (int)Math.Round(xVal);
                wallEContext.CurrentY = (int)Math.Round(yVal);
                OutputLog.Add($"Executing: Spawn({node.XCoordinate}, {node.YCoordinate})");
            }
            catch (RuntimeException ex)
            {
                throw new RuntimeException($"Error in Spawn statement: {ex.Message}");
            }
        }

        private void VisitColorNode(ColorNode node)
        {
            try
            {
                object colorValue = EvaluateExpression(node.ColorExpression);
                if (!(colorValue is string colorName))
                {
                    throw new RuntimeException($"Color command expects a string argument (color name). Got {colorValue?.GetType().Name}.");
                }
                wallEContext.CurrentBrushColor = new PixelColor(colorName);
                OutputLog.Add($"Wall-E brush color set to: {wallEContext.CurrentBrushColor.Name}.");
            }
            catch (RuntimeException rex)
            {
                throw new RuntimeException($"Error in Color statement: {rex.Message}");
            }
        }

        private void VisitSizeNode(SizeNode node)
        {
            try
            {
                object sizeValue = EvaluateExpression(node.SizeExpression);

                if (!(sizeValue is double sizeDouble))
                {
                    throw new RuntimeException($"Size command expects a numeric argument. Got {sizeValue?.GetType().Name}.");
                }
                int newSize = (int)Math.Round(sizeDouble);

                // Add validation for brush size as per spec (e.g., must be positive)
                if (newSize < 1)
                {
                    throw new RuntimeException($"Brush size must be a positive integer. Got {newSize}.");
                }
                wallEContext.CurrentBrushSize = newSize;
                OutputLog.Add($"Wall-E brush size set to: {wallEContext.CurrentBrushSize}.");
            }
            catch (RuntimeException rex)
            {
                throw new RuntimeException($"Error in Size statement: {rex.Message}");
            }
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
            try
            {
                string variableName = node.Identifier.Value;
                object valueToAssign = EvaluateExpression(node.ValueExpression);
                symbolTable.Assign(variableName, valueToAssign);
                OutputLog.Add($"Assigned to '{variableName}': {valueToAssign ?? "null"}.");
            }
            catch (RuntimeException rex)
            {
                throw new RuntimeException($"Error in Assignment statement: {rex.Message}");
            }
        }

        private void VisitLabelNode(LabelNode node)
        {
            // Labels are processed during the ScanForLabels phase.
            OutputLog.Add($"Encountered Label: {node.IdentifierToken.Value}");
        }

        private void VisitGoToNode(GoToNode node)
        {
            try
            {
                string targetLabelName = node.LabelToken.Value;
                object condition = EvaluateExpression(node.Condition);
                bool conditionResult = BinaryOperations.ConvertToBooleanStatic(condition);

                OutputLog.Add($"Executing GoTo [{targetLabelName}] with condition ({node.Condition}) evaluated to {conditionResult}.");

                if (conditionResult)
                {
                    runtimeEnvironment.RequestGoTo(targetLabelName);
                    OutputLog.Add($"GoTo [{targetLabelName}] requested.");
                }
                else
                {
                    OutputLog.Add($"GoTo [{targetLabelName}] condition false. No jump.");
                }
            }
            catch (RuntimeException rex)
            {
                throw new RuntimeException($"Error in GoTo statement targeting '{node.LabelToken.Value}': {rex.Message}");
            }
        }

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
                  return VisitFunctionCallNode(functionCallNode);
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

        private object VisitFunctionCallNode(FunctionCallNode node)
        {
            var evaluatedArgs = new List<object>();
            foreach (var argExpr in node.Arguments)
            {
                evaluatedArgs.Add(EvaluateExpression(argExpr));
            }

            // Use the FunctionHandlers helper class
            if (FunctionHandlers.TryGetHandler(node.FunctionNameToken.Type, out BuiltInFunctionHandler handler))
            {
                try
                {
                    // Pass the necessary context to the handler
                    return handler(evaluatedArgs, this.wallEContext, this.canvas, this.symbolTable);
                }
                catch (RuntimeException) { throw; } // Re-throw our specific exceptions
                catch (Exception ex) // Catch other unexpected errors from handlers
                {
                    throw new RuntimeException($"Error during function call '{node.FunctionNameToken.Value}': {ex.Message}");
                }
            }
            throw new RuntimeException($"Unknown built-in function keyword: {node.FunctionNameToken.Type} ('{node.FunctionNameToken.Value}')");
        }
    }
}