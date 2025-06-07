using System;
using System.Collections.Generic;
using System.Linq; // For checking if Errors list has any items
using System.Text;
using Interpreter.Core; // Your namespace for Lexer, Token, Parser, etc.
using Interpreter.Core.Ast.Statements; // For ProgramNode

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Pixel Wall-E Parser Test");
            Console.WriteLine("Enter Pixel Wall-E code. Type 'EOF' on a new line to finish input, or 'exit' to quit.");
            Console.WriteLine("Supported for now: Spawn(NUMBER, NUMBER) on separate lines.");
            Console.WriteLine("------------------------------------");

            while (true)
            {
                Console.WriteLine("\nEnter code (type 'EOF' on a new line to process, or 'exit' to quit):");
                StringBuilder inputBuilder = new StringBuilder();
                string line;

                // Loop to read multi-line input from the user
                while (true)
                {
                    line = Console.ReadLine() ?? String.Empty;
                    if (line == null) // End of stream, e.g., if input is redirected
                    {
                        Console.WriteLine("Debug: Program.cs - Console.ReadLine() returned null.");
                        break;
                    }
                    if (line.Trim().Equals("EOF", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Debug: Program.cs - EOF detected.");
                        break;
                    }
                    if (line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Debug: Program.cs - Exit command detected. Exiting parser test.");
                        return; // Exit the program
                    }
                    inputBuilder.AppendLine(line);
                }

                string inputText = inputBuilder.ToString();

                // Check if only EOF was typed without any preceding actual code
                if (string.IsNullOrWhiteSpace(inputText) && line != null && line.Trim().Equals("EOF", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("No actual code input provided before EOF.");
                    // continue; // Decide if you want to loop again or process empty input
                }


                Console.WriteLine("\n--- Input to be Processed ---");
                Console.WriteLine($"'{inputText.TrimEnd('\r', '\n')}'"); // Print the input being lexed/parsed
                Console.WriteLine("------------------------------------");

                // --- 1. Lexing ---
                Console.WriteLine("Debug: Program.cs - Creating Lexer instance.");
                Lexer lexer = new Lexer(inputText);
                List<Token> tokens = new List<Token>();
                Token currentLexToken;
                Console.WriteLine("Debug: Program.cs - Starting lexing loop.");
                int lexerLoopCount = 0;
                int maxLexerLoops = string.IsNullOrEmpty(inputText) ? 10 : inputText.Length + 10; // Safety limit

                do
                {
                    lexerLoopCount++;
                    if (lexerLoopCount > maxLexerLoops)
                    {
                        Console.WriteLine($"Debug: Program.cs - Lexer safety break! Loops: {lexerLoopCount}, Input Length: {inputText.Length}");
                        // Add the last known token if possible, then a forceful EOF
                        if (tokens.Count > 0 && tokens.Last().Type != TokenType.EndOfFile)
                            tokens.Add(new Token(TokenType.EndOfFile, "\\0", "Lexer Safety EOF"));
                        else if (tokens.Count == 0)
                            tokens.Add(new Token(TokenType.EndOfFile, "\\0", "Lexer Safety EOF"));
                        break;
                    }

                    currentLexToken = lexer.GetNextToken();
                    tokens.Add(currentLexToken);
                    // Optional: Very verbose logging of each token as it's lexed
                    // Console.WriteLine($"Debug: Program.cs - Lexed token: {currentLexToken}");
                } while (currentLexToken.Type != TokenType.EndOfFile);
                Console.WriteLine($"Debug: Program.cs - Finished lexing loop. Tokens count: {tokens.Count}, Lexer loops: {lexerLoopCount}");

                Console.WriteLine("\n--- Tokens ---");
                bool hasLexerErrors = false;
                if (!tokens.Any())
                {
                    Console.WriteLine("No tokens were produced by the lexer.");
                }
                else
                {
                    foreach (var token in tokens)
                    {
                        Console.WriteLine(token.ToString());
                        if (token.Type == TokenType.Illegal)
                        {
                            hasLexerErrors = true;
                        }
                    }
                }


                if (hasLexerErrors)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\nLEXER ERRORS DETECTED. Parsing might produce further errors or fail.");
                    Console.ResetColor();
                }

                // --- 2. Parsing ---
                Console.WriteLine("\n--- Parsing ---");
                Console.WriteLine("Debug: Program.cs - About to create Parser instance.");
                Console.WriteLine($"Debug: Program.cs - Finished lexing loop. Tokens count: {tokens.Count}, Lexer loops: {lexerLoopCount}");

                if (tokens == null) // Explicit null check
                {
                    Console.WriteLine("Debug: Program.cs - FATAL: 'tokens' list IS NULL before creating Parser!");
                }
                else if (!tokens.Any())
                {
                    Console.WriteLine("Debug: Program.cs - 'tokens' list IS EMPTY before creating Parser.");
                    // It's valid for the lexer to return just an EOF token if the input is empty.
                    // So, an empty list before adding EOF would mean truly no tokens from non-empty input.
                    // If tokens list contains only EOF, tokens.Any() is true.
                    if (tokens.Count == 1 && tokens[0].Type == TokenType.EndOfFile)
                    {
                        Console.WriteLine("Debug: Program.cs - 'tokens' list contains only EOF.");
                    }
                }
                else
                {
                    Console.WriteLine($"Debug: Program.cs - 'tokens' list is NOT NULL and NOT EMPTY. Count: {tokens.Count}. First: {tokens[0]}, Last: {tokens.Last()}");
                }

                Console.WriteLine("Debug: Program.cs - About to create Parser instance.");
                Parser parser = new Parser(tokens!);
                Console.WriteLine("Debug: Program.cs - Parser instance created. About to call ParseProgram().");
                ProgramNode astRoot = parser.ParseProgram();
                Console.WriteLine("Debug: Program.cs - ParseProgram() returned.");



                // --- Display Parsing Errors ---
                if (parser.Errors.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nPARSING ERRORS / DEBUG LOG:"); // Changed title to include debug
                    foreach (string error in parser.Errors) // parser.Errors now contains our debug messages too
                    {
                        Console.WriteLine(error);
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nParsing successful! No errors or debug messages reported by the parser.");
                    Console.ResetColor();
                }

                // --- Display AST ---
                Console.WriteLine("\n--- Abstract Syntax Tree (AST) ---");
                if (astRoot != null)
                {
                    // Check if there are any statements to print, to avoid large empty ProgramNode output
                    if (astRoot.Statements.Any() || parser.Errors.Any(e => !e.StartsWith("Debug:"))) // Print AST if statements or real errors
                    {
                        Console.WriteLine(astRoot.ToString());
                    }
                    else if (!parser.Errors.Any(e => !e.StartsWith("Debug:")))
                    {
                        Console.WriteLine("AST is empty and no parsing errors (likely empty or only whitespace/newline input).");
                    }
                    else
                    {
                        Console.WriteLine("AST generated but might be incomplete due to errors.");
                        Console.WriteLine(astRoot.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("AST Root is null (major parsing failure or no input).");
                }
                Console.WriteLine("------------------------------------");
            }
        }
    }
}