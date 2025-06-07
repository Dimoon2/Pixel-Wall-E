using System.Collections.Generic;
using System.Text;

namespace Interpreter.Core
{
    class Lexer
    {
        private readonly string text;
        private int position;

        // The Keyword Map
        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            // Commands
            {"Spawn", TokenType.KeywordSpawn},
            {"Color", TokenType.KeywordColor},
            {"Size", TokenType.KeywordSize},
            {"DrawLine", TokenType.KeywordDrawLine},
            {"DrawCircle", TokenType.KeywordDrawCircle},
            {"DrawRectangle", TokenType.KeywordDrawRectangle},
            {"Fill", TokenType.KeywordFill},
            {"GoTo", TokenType.KeywordGoTo},

            // Built-in Functions
            {"GetActualX", TokenType.KeywordGetActualX},
            {"GetActualY", TokenType.KeywordGetActualY},
            {"GetCanvasSize", TokenType.KeywordGetCanvasSize},
            {"GetColorCount", TokenType.KeywordGetColorCount},
            {"IsBrushColor", TokenType.KeywordIsBrushColor},
            {"IsBrushSize", TokenType.KeywordIsBrushSize},
            {"IsCanvasColor", TokenType.KeywordIsCanvasColor},
            // Add "true" and "false" here if we decide to support them as keywords
            {"true", TokenType.KeywordTrue},
            {"false", TokenType.KeywordFalse}
        };
        public Lexer(string Text)
        {
            text = Text;
            position = 0;
        }
        private char CurrentChar
        {
            get
            {
                if (position >= text.Length)
                {
                    return '\0'; //(EOF)
                }
                return text[position];
            }
        }
        public void Advance()
        {
            position++;
        }

        private char Peek()
        {
            if (position + 1 >= text.Length)
            {
                return '\0';
            }
            return text[position + 1];
        }

        private bool IsIdentifierStart(char c)
        {
            return char.IsLetter(c);
            // return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == 'ñ' || c == 'Ñ' ... etc.;
        }

        private bool IsIdentifierPart(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '-';
        }

        public Token GetNextToken()
        {
            while (CurrentChar != '\0')
            {
                // 1.Newlines
                if (CurrentChar == '\r')
                {
                    Advance();
                    if (CurrentChar == '\n') Advance(); // CR+LF
                    return new Token(TokenType.Newline, "\\n");
                }
                if (CurrentChar == '\n')
                {
                    Advance(); // LF
                    return new Token(TokenType.Newline, "\\n");
                }

                // 2. Skip other whitespace (spaces, tabs)
                if (CurrentChar == ' ' || CurrentChar == '\t')
                {
                    Advance();
                    continue;
                }

                // 3. Handle Numbers 
                if (char.IsDigit(CurrentChar))
                {
                    var startPos = position;
                    while (char.IsDigit(CurrentChar))
                    {
                        Advance();
                    }
                    string numberStr = text.Substring(startPos, position - startPos);
                    if (int.TryParse(numberStr, out int intValue))
                    {
                        return new Token(TokenType.Number, numberStr, intValue);
                    }
                    else
                    {
                        return new Token(TokenType.Illegal, numberStr, null);
                    }
                }

                // 4. Handle Identifiers and keywords
                if (IsIdentifierStart(CurrentChar))
                {
                    var startPos = position;
                    var sb = new StringBuilder();
                    while (CurrentChar != '\0' && IsIdentifierPart(CurrentChar))
                    {
                        sb.Append(CurrentChar);
                        Advance();
                    }
                    string identifierStr = sb.ToString();
                    string lexeme = text.Substring(startPos, position - startPos);

                    // Check if it's a keyword
                    if (Keywords.TryGetValue(identifierStr, out TokenType keywordType))
                    {
                        return new Token(keywordType, lexeme); // Keywords don't have a separate 'literal' different from their text
                    }
                    // If not a keyword, it's a user-defined identifier
                    return new Token(TokenType.Identifier, lexeme, identifierStr); // Literal is the identifier name
                }

                // 5. Handle String Literals
                if (CurrentChar == '"')
                {
                    Advance(); // Consume opening quote
                    var sbString = new StringBuilder();
                    var startPosStringContent = position;
                    while (CurrentChar != '"' && CurrentChar != '\0')
                    {
                        sbString.Append(CurrentChar);
                        Advance();
                    }
                    string stringContent = sbString.ToString();
                    string rawStringLexeme;

                    if (CurrentChar == '"')
                    {
                        Advance();
                        int stringLexemeStart = startPosStringContent - 1; // Position of the opening quote
                        rawStringLexeme = text.Substring(stringLexemeStart, position - stringLexemeStart);
                        return new Token(TokenType.String, rawStringLexeme, stringContent);
                    }
                    else
                    {
                        // Unterminated string
                        rawStringLexeme = text.Substring(startPosStringContent - 1, position - (startPosStringContent - 1));
                        return new Token(TokenType.Illegal, rawStringLexeme, "Unterminated string");
                    }
                }

                // 6. Handle Operators and Punctuation (using switch)
                char charToSwitch = CurrentChar; // Store before advancing in some cases
                switch (charToSwitch)
                {
                    case '+': Advance(); return new Token(TokenType.Plus, "+");
                    case '-': Advance(); return new Token(TokenType.Minus, "-"); // Note: '-' in identifiers handled by IsIdentifierPart
                    case '*':
                        if (Peek() == '*') { Advance(); Advance(); return new Token(TokenType.Power, "**"); }
                        Advance(); return new Token(TokenType.Multiply, "*");
                    case '/': Advance(); return new Token(TokenType.Divide, "/");
                    case '%': Advance(); return new Token(TokenType.Modulo, "%");
                    case '<':
                        if (Peek() == '-') { Advance(); Advance(); return new Token(TokenType.Arrow, "<-"); }
                        if (Peek() == '=') { Advance(); Advance(); return new Token(TokenType.LessEqual, "<="); }
                        Advance(); return new Token(TokenType.Less, "<");
                    case '>':
                        if (Peek() == '=') { Advance(); Advance(); return new Token(TokenType.GreaterEqual, ">="); }
                        Advance(); return new Token(TokenType.Greater, ">");
                    case '=': // Note: Assignment is '<-', so '==' is only for comparison
                        if (Peek() == '=') { Advance(); Advance(); return new Token(TokenType.EqualEqual, "=="); }
                        Advance(); return new Token(TokenType.Illegal, "=", "Single '=' is not a valid operator. Did you mean '==' or '<-'?");
                    case '&':
                        if (Peek() == '&') { Advance(); Advance(); return new Token(TokenType.And, "&&"); }
                        Advance(); return new Token(TokenType.Illegal, "&", "Single '&' is not valid. Did you mean '&&'?");
                    case '|':
                        if (Peek() == '|') { Advance(); Advance(); return new Token(TokenType.Or, "||"); }
                        Advance(); return new Token(TokenType.Illegal, "|", "Single '|' is not valid. Did you mean '||'?");
                    case '(': Advance(); return new Token(TokenType.LParen, "(");
                    case ')': Advance(); return new Token(TokenType.RParen, ")");
                    case '[': Advance(); return new Token(TokenType.LBracket, "[");
                    case ']': Advance(); return new Token(TokenType.RBracket, "]");
                    case ',': Advance(); return new Token(TokenType.Comma, ",");
                    case ':': Advance(); return new Token(TokenType.Colon, ":");
                }

                // 7. If no token matched above, it's an illegal character
                char illegalChar = CurrentChar;
                Advance();
                return new Token(TokenType.Illegal, illegalChar.ToString(), $"Unexpected character: '{illegalChar}'");
            }
            return new Token(TokenType.EndOfFile, "\0");
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            Token token;
            do
            {
                token = GetNextToken();
                tokens.Add(token);
                if (token.Type == TokenType.Illegal)
                {
                    // Optional: Stop tokenizing on first error or collect all errors
                    Console.WriteLine($"Lexer Error: {token.Literal} (Value: '{token.Value}')");
                    // For now, let's stop if we want to be strict. Or just add and let parser deal.
                }
            } while (token.Type != TokenType.EndOfFile);
            return tokens;
        }
    }
}