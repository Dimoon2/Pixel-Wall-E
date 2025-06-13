using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interpreter.Core; // Your namespace for Lexer, Token, Parser, etc.
using Interpreter.Core.Ast.Statements;
using Interpreter.Core.Interpreter; // For Interpreter, Canvas, WallEContext, etc.
using Interpreter.Core.Interpreter.Helpers; // If you have helpers like BinaryOperations in a sub-namespace


namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Pixel Wall-E Interpreter Test");
            Console.WriteLine("Enter Pixel Wall-E code. Type 'EOF' on a new line to finish input, or 'exit' to quit.");
            Console.WriteLine("------------------------------------");

            while (true)
            {
                Console.WriteLine("\nEnter code (type 'EOF' on a new line to process, or 'exit' to quit):");
                StringBuilder inputBuilder = new StringBuilder();
                string line;

                while (true)
                {
                    line = Console.ReadLine() ?? string.Empty;
                    if (line.Trim().Equals("EOF", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                    if (line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Exiting interpreter test.");
                        return;
                    }
                    inputBuilder.AppendLine(line);
                }

                string inputText = inputBuilder.ToString();

                if (string.IsNullOrWhiteSpace(inputText) && line.Trim().Equals("EOF", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("No actual code input provided before EOF.");
                    continue;
                }

                Console.WriteLine("\n--- Input to be Processed ---");
                Console.WriteLine($"'{inputText.TrimEnd('\r', '\n')}'");
                Console.WriteLine("------------------------------------");

                // --- 1. Lexing ---
                Console.WriteLine("\n--- Lexing ---");
                Lexer lexer = new Lexer(inputText);
                List<Token> tokens = new List<Token>();
                Token currentLexToken;
                int lexerSafetyCounter = 0;
                int maxLexerLoops = string.IsNullOrEmpty(inputText) ? 10 : inputText.Length + 100; // Increased safety margin

                do
                {
                    lexerSafetyCounter++;
                    if (lexerSafetyCounter > maxLexerLoops)
                    {
                        Console.Error.WriteLine($"LEXER SAFETY BREAK! Loop count: {lexerSafetyCounter}");
                        tokens.Add(new Token(TokenType.Illegal, "LEXER_SAFETY_BREAK", "Lexer possibly stuck."));
                        if (tokens.Last().Type != TokenType.EndOfFile)
                           tokens.Add(new Token(TokenType.EndOfFile, "\\0", "Forced EOF after lexer safety break"));
                        break;
                    }
                    currentLexToken = lexer.GetNextToken();
                    tokens.Add(currentLexToken);
                } while (currentLexToken.Type != TokenType.EndOfFile);

                Console.WriteLine("\n--- Tokens ---");
                bool hasLexerErrors = false;
                foreach (var token in tokens)
                {
                    Console.WriteLine(token.ToString());
                    if (token.Type == TokenType.Illegal)
                    {
                        hasLexerErrors = true;
                    }
                }

                if (hasLexerErrors)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\nLEXER ERRORS DETECTED. Parsing and interpretation might be affected.");
                    Console.ResetColor();
                    // Optionally, you might want to stop here if lexer errors are critical
                    // continue;
                }

                // --- 2. Parsing ---
                Console.WriteLine("\n--- Parsing ---");
                Parser parser = new Parser(tokens);
                ProgramNode astRoot = parser.ParseProgram();

                if (parser.Errors.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nPARSING ERRORS / DEBUG LOG:");
                    foreach (string error in parser.Errors)
                    {
                        Console.WriteLine(error);
                    }
                    Console.ResetColor();
                    // Optionally, stop if parsing errors are critical
                    // continue;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nParsing successful! No errors reported by the parser.");
                    Console.ResetColor();
                }

                Console.WriteLine("\n--- Abstract Syntax Tree (AST) ---");
                if (astRoot != null)
                {
                    if (astRoot.Statements.Any() || parser.Errors.Any(e => !e.StartsWith("Debug:", StringComparison.OrdinalIgnoreCase)))
                    {
                        // You might want a more concise AST print method for complex trees
                        Console.WriteLine(astRoot.ToString()); // This can be very verbose
                    }
                    else if (!parser.Errors.Any(e => !e.StartsWith("Debug:", StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine("AST is empty (likely empty or only whitespace/newline input).");
                    }
                }
                else
                {
                    Console.WriteLine("AST Root is null (major parsing failure or no input).");
                }

                // --- 3. Interpretation ---
                if (astRoot != null && !parser.Errors.Any(e => !e.StartsWith("Debug:", StringComparison.OrdinalIgnoreCase)) && !hasLexerErrors) // Proceed if AST is valid and no critical parser/lexer errors
                {
                    Console.WriteLine("\n--- Interpretation ---");
                    // Setup Interpreter components
                    // You might want to get canvas dimensions from user or a command later
                    Canvas simCanvas = new Canvas(20, 10); // Example canvas size
                    WallEContext simWallE = new WallEContext();
                    SymbolTable simSymbolTable = new SymbolTable();
                    RuntimeEnvironment simRuntimeEnv = new RuntimeEnvironment();

                    Core.Interpreter.Interpreter interpreter =
                        new Core.Interpreter.Interpreter(simCanvas, simWallE, simSymbolTable, simRuntimeEnv);

                    Console.WriteLine("Executing program...");
                    interpreter.ExecuteProgram(astRoot);

                    Console.WriteLine("\n--- Interpreter Output Log ---");
                    interpreter.OutputLog.ForEach(Console.WriteLine);

                    if (interpreter.ErrorLog.Any())
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("\n--- Interpreter Runtime Errors ---");
                        interpreter.ErrorLog.ForEach(Console.Error.WriteLine);
                        Console.ResetColor();
                    }

                    Console.WriteLine($"\n--- Final Wall-E State --- \n{simWallE}");

                    // Optional: Add a way to dump the symbol table
                    // Console.WriteLine("\n--- Final Symbol Table ---");
                    // simSymbolTable.DumpToConsole(); // You'd need to implement this method

                    Console.WriteLine("\n--- Final Canvas ---");
                    simCanvas.PrintToConsole();
                }
                else if (astRoot != null) // AST exists but there were parser/lexer errors
                {
                    Console.WriteLine("\nInterpretation skipped due to lexer or parser errors.");
                }


                Console.WriteLine("------------------------------------");
            } // End of main while loop
        } // End of Main
    } // End of class Program
}