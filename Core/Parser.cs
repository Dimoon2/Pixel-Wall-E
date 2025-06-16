using System;
using System.Collections.Generic;
using Interpreter.Core.Ast; // For AstNode
using Interpreter.Core.Ast.Statements; // For ProgramNode, StatementNode, SpawnNode
using Interpreter.Core.Ast.Expressions;
using System.Linq.Expressions;
using System.Linq;
using System.Diagnostics; // For ExpressionNode, NumberLiteralNode

namespace Interpreter.Core
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int currentTokenIndex;
        public List<string> errors;

        public Parser(List<Token> token)
        {
            tokens = token;
            currentTokenIndex = 0;
            errors = new List<string>();
        }
        private Token currentToken
        {
            get
            {
                if (tokens is null)
                {
                    return null;
                }
                if (currentTokenIndex >= 0 && currentTokenIndex < tokens.Count)
                {
                    return tokens[currentTokenIndex];
                }

                return new Token(TokenType.EndOfFile, "\\0", "Synthetic EOF from CurrentToken OOB");
            }
        }
        private TokenType currentTokenType => currentToken.Type;

        private void Advance()
        {
            if (currentTokenIndex < tokens.Count - 1) // Only advance if not already at the last token
            {
                currentTokenIndex++;
            }
            else
            {
                // Console.Error.WriteLine($"!!!! DEBUG: Advance: AT LAST TOKEN (EOF or other). Not advancing. Current: {currentToken}, Index: {currentTokenIndex}");
            }
        }

        private Token Match(TokenType expectedType)
        {
            if (currentToken.Type == expectedType)
            {
                Token token = currentToken;
                Advance();
                return token;
            }
            errors.Add($"Parser Error: Expected token {expectedType} but got {currentTokenType} ('{currentToken.Value}') at position {currentTokenIndex}.");

            return null!;
        }
        public TokenType PeekType()
        {
            int PeekIndex = currentTokenIndex + 1;
            if (PeekIndex >= tokens.Count) return TokenType.EndOfFile;
            return tokens[PeekIndex].Type;
        }

        // Public method to get any parsing errors collected
        public IReadOnlyList<string> Errors => errors;


        //PARSE PROGRAM
        public ProgramNode ParseProgram()
        {
            if (tokens == null || tokens.Count == 0 || tokens.Last().Type != TokenType.EndOfFile) // Ensure EOF is last if not empty
            {
                // if (tokens == null) tokens = new List<Token>(); // Prevent null
                if (tokens!.Count == 0 || tokens.Last().Type != TokenType.EndOfFile)
                {
                    tokens.Add(new Token(TokenType.EndOfFile, "\\0", "Forced EOF"));
                }
                if (tokens.Count == 1 && tokens[0].Type == TokenType.EndOfFile)
                { // Only EOF
                    return new ProgramNode(new List<StatementNode>());
                }
            }

            var statements = new List<StatementNode>();
            while (currentTokenType != TokenType.EndOfFile)
            {
                // 1. Skip any leading newlines before a statement
                while (currentTokenType == TokenType.Newline)
                {
                    Advance();
                    // Safety for skipping newlines if Advance gets stuck (unlikely but for paranoia)
                    if (currentTokenType == TokenType.Newline)// && loopSafetyCounter % 10 == 0 && loopSafetyCounter > tokens.Count)
                    {
                        // Console.Error.WriteLine($"!!!! DEBUG: Potentially stuck skipping Newlines. Token: {currentToken}");
                    }
                }
                // 2. If, after skipping newlines, we are at EOF, then break the main loop.
                if (currentTokenType == TokenType.EndOfFile)
                {
                    break;
                }

                // 3. Parse a statement
                StatementNode statement = ParseStatement(); // ParseStatement now advances on its own default error

                if (statement != null)
                {
                    statements.Add(statement);
                }
                else // ParseStatement returned null (error)
                {
                    if (currentTokenType != TokenType.Newline && currentTokenType != TokenType.EndOfFile)
                    {
                        Advance();
                    }
                }

                // 4. After a statement or error recovery, expect a Newline or EndOfFile.
                if (currentTokenType == TokenType.Newline)
                {
                    Advance(); // Match the (single) newline
                }
                else if (currentTokenType != TokenType.EndOfFile)
                {
                    // This means a statement was parsed (or error occurred) and it wasn't followed by a newline or EOF.
                    // Example: Spawn(0,0) Spawn(1,1) <- no newline
                    if (statement != null) // Only if the statement itself was considered "parsed"
                    {
                        errors.Add($"Parser Error: Expected Newline or EndOfFile after statement, but got {currentTokenType} ('{currentToken.Value}').");
                        // To prevent potential loop if this non-newline/EOF token isn't handled by next iteration's ParseStatement
                        Advance();
                    }
                }
            }
            return new ProgramNode(statements);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private StatementNode ParseStatement()
        {
            // StatementNode result = null!;
            switch (currentTokenType)
            {
                case TokenType.KeywordSpawn:
                    return ParseSpawnStatement();

                case TokenType.KeywordColor:
                    return ParseColorStatement();
                case TokenType.KeywordSize:
                    return ParseSizeStatement();

                case TokenType.KeywordDrawLine:
                    return ParseDrawLineStatement();
                case TokenType.KeywordDrawCircle:
                    return ParseDrawCircleStatement();
                case TokenType.KeywordDrawRectangle:
                    return ParseDrawRectangleStatement();
                case TokenType.KeywordFill:
                    return ParseFillStatement();

                case TokenType.Identifier:
                    if (PeekType() == TokenType.Arrow)
                    {
                        return ParseAssignmentStatement();
                    }
                    else if (PeekType() == TokenType.Newline || PeekType() == TokenType.EndOfFile)// FOR LABELS
                    {
                        return ParseLabelStatement();
                    }
                    else
                    {
                        Debug.WriteLine("Entre a identifier con algo q no es variable ni labe");
                        errors.Add($"Parser Error: Identifier '{currentToken.Value}' at start of statement is not followed by '<-' or '_' for assigments and label.");
                        return null!;
                    }
                case TokenType.KeywordGoTo:
                    return ParseGoToStatement();

                // ... other cases ...
                default:
                    errors.Add($"Parser Error: Unexpected token {currentTokenType} ('{currentToken.Value}') at start of a statement. Skipping this token.");
                    Advance(); // Match the problematic token //not in new prom
                    return null!; // Error
            }
        }
        // --- Specific Statement Parsers ---
        private SpawnNode ParseSpawnStatement()
        {
            Match(TokenType.KeywordSpawn); // Match 'Spawn'

            if (Match(TokenType.LParen) == null)
            {
                errors.Add("Null argument");
                return null!;
            }

            ExpressionNode xCoord = ParseExpression();
            if (xCoord == null)
            {
                errors.Add("Null argument");
                return null!;
            }

            if (Match(TokenType.Comma) == null)
            {
                errors.Add("Null argument");
                return null!;
            }

            ExpressionNode yCoord = ParseExpression();
            if (yCoord == null)
            {
                errors.Add("Null argument");
                return null!;
            }

            if (Match(TokenType.RParen) == null)
            {
                errors.Add("Null argument");
                return null!;
            }

            return new SpawnNode(xCoord, yCoord);
        }

        private ColorNode ParseColorStatement()
        {
            Match(TokenType.KeywordColor);
            if (Match(TokenType.LParen) == null)
            {
                errors.Add("Null argument");
                return null!;
            }
            ExpressionNode color = ParseExpression();
            Match(TokenType.RParen);

            if (color is null)
            {
                errors.Add("Null argument");
                return null!;
            }
            return new ColorNode(color);
        }

        private SizeNode ParseSizeStatement()
        {
            Match(TokenType.KeywordSize);
            Match(TokenType.LParen);
            ExpressionNode size = ParseExpression();
            Match(TokenType.RParen);
            if (size is null)
            {
                errors.Add("Null argument");
                return null!;
            }
            return new SizeNode(size);
        }

        private DrawLineNode ParseDrawLineStatement()
        {
            if (Match(TokenType.KeywordDrawLine) == null) return null!;
            if (Match(TokenType.LParen) == null) return null!;
            ExpressionNode dirX = ParseExpression();
            if (dirX == null) return null!;
            if (Match(TokenType.Comma) == null) return null!;
            ExpressionNode dirY = ParseExpression();
            if (dirY == null) return null!;
            if (Match(TokenType.Comma) == null) return null!;
            ExpressionNode dist = ParseExpression();
            if (dist == null) return null!;
            if (Match(TokenType.RParen) == null) return null!;

            return new DrawLineNode(dirX, dirY, dist);
        }

        private DrawCircleNode ParseDrawCircleStatement()
        {
            if (Match(TokenType.KeywordDrawCircle) == null) return null!;
            if (Match(TokenType.LParen) == null) return null!;
            ExpressionNode dirX = ParseExpression();
            if (dirX == null) return null!;
            if (Match(TokenType.Comma) == null) return null!;
            ExpressionNode dirY = ParseExpression();
            if (dirY == null) return null!;
            if (Match(TokenType.Comma) == null) return null!;
            ExpressionNode radius = ParseExpression();
            if (radius == null) return null!;
            if (Match(TokenType.RParen) == null) return null!;

            return new DrawCircleNode(dirX, dirY, radius);
        }

        private DrawRectangleNode ParseDrawRectangleStatement()
        {
            if (Match(TokenType.KeywordDrawRectangle) == null) return null!;
            if (Match(TokenType.LParen) == null) return null!;

            ExpressionNode dirX = ParseExpression();
            if (dirX == null) return null!;
            if (Match(TokenType.Comma) == null) return null!;

            ExpressionNode dirY = ParseExpression();
            if (dirY == null) return null!;
            if (Match(TokenType.Comma) == null) return null!;

            ExpressionNode dist = ParseExpression();
            if (dist == null) return null!;
            if (Match(TokenType.Comma) == null) return null!;

            ExpressionNode width = ParseExpression();
            if (width == null) return null!;
            if (Match(TokenType.Comma) == null) return null!;

            ExpressionNode height = ParseExpression();
            if (height == null) return null!;
            if (Match(TokenType.RParen) == null) return null!;

            return new DrawRectangleNode(dirX, dirY, dist, width, height);
        }

        private FillNode ParseFillStatement()
        {
            Match(TokenType.KeywordFill);
            if (Match(TokenType.LParen) == null) return null!;
            if (Match(TokenType.RParen) == null) return null!;
            return new FillNode();
        }

        private AssignmentNode ParseAssignmentStatement()
        {
            Token identifier = Match(TokenType.Identifier);
            if (identifier == null) return null!;
            if (Match(TokenType.Arrow) == null) return null!;

            ExpressionNode value = ParseExpression();
            if (value == null) return null!;

            return new AssignmentNode(identifier, value);
        }

        private LabelNode ParseLabelStatement()
        {
            Token identifier = Match(TokenType.Identifier);
            if (identifier == null) return null!;

            return new LabelNode(identifier);
        }

        private GoToNode ParseGoToStatement()
        {
            Match(TokenType.KeywordGoTo);
            if (Match(TokenType.LBracket) == null) return null!;

            Token labelIdentifier = Match(TokenType.Identifier);
            if (labelIdentifier == null)
            {
                errors.Add($"Parser Error: Expected label name (Identifier) after 'GoTo ['.");
                return null!;
            }

            if (Match(TokenType.RBracket) is null)
            {
                errors.Add("Null argument");
                return null!;
            }
            if (Match(TokenType.LParen) is null)
            {
                errors.Add("Null argument");
                return null!;
            }
            ExpressionNode condition = ParseExpression();
            if (Match(TokenType.RParen) is null)
            {
                errors.Add("Null argument");
                return null!;
            }
            return new GoToNode(labelIdentifier, condition);
        }
        
        // --- Expression Parsers ---
        private ExpressionNode ParseExpression()
        {
            return ParseLogicalAndExpression();
        }

        private ExpressionNode ParseLogicalAndExpression() // &&
        {
            ExpressionNode left = ParseLogicalOrExpression();
            while (currentTokenType == TokenType.And)
            {
                Token operatorToken = Match(TokenType.And);
                ExpressionNode right = ParseLogicalOrExpression();
                if (right == null) return null!;
                left = new BinaryOpNode(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionNode ParseLogicalOrExpression() // ||
        {
            ExpressionNode left = ParseComparisonExpression();
            while (currentTokenType == TokenType.Or)
            {
                Token operatorToken = Match(TokenType.Or);
                ExpressionNode right = ParseComparisonExpression();
                if (right == null) return null!;
                left = new BinaryOpNode(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionNode ParseComparisonExpression()
        {
            ExpressionNode left = ParseAdictiveExpression();
            while (currentTokenType == TokenType.EqualEqual || currentTokenType == TokenType.LessEqual || currentTokenType == TokenType.Less
            || currentTokenType == TokenType.Greater || currentTokenType == TokenType.GreaterEqual)
            {
                Token operatorToken = currentToken;
                Advance();
                ExpressionNode right = ParseAdictiveExpression();
                if (right == null) return null!;
                left = new BinaryOpNode(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionNode ParseAdictiveExpression()
        {
            ExpressionNode left = ParseMultiplicativeExpression();
            while (currentTokenType == TokenType.Plus || currentTokenType == TokenType.Minus)
            {
                Token operatorToken = currentToken;
                Advance();
                ExpressionNode right = ParseMultiplicativeExpression();
                if (right == null) return null!;
                left = new BinaryOpNode(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionNode ParseMultiplicativeExpression()
        {
            ExpressionNode left = ParsePowerExpression();
            while (currentTokenType == TokenType.Multiply || currentTokenType == TokenType.Divide || currentTokenType == TokenType.Modulo)
            {
                Token operatorToken = currentToken;
                Advance();
                ExpressionNode right = ParsePowerExpression();
                if (right == null) return null!;
                left = new BinaryOpNode(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionNode ParsePowerExpression()
        {
            ExpressionNode left = ParseUnaryOpExpression();
            while (currentTokenType == TokenType.Power)
            {
                Token operatorToken = Match(TokenType.Power);
                ExpressionNode right = ParseUnaryOpExpression();
                if (right == null) return null!;
                left = new BinaryOpNode(left, operatorToken, right);
            }
            return left;
        }
        private ExpressionNode ParseUnaryOpExpression()
        {
            if (currentToken.Type == TokenType.Minus)
            {
                Token minusToken = Match(TokenType.Minus);
                ExpressionNode node = ParsePrimaryExpression();
                if (node == null) return null!;
                return new UnaryOpNode(minusToken, node);
            }
            return ParsePrimaryExpression();
        }

        public ExpressionNode ParsePrimaryExpression()
        {
            Token currentTokenSnapshot = currentToken;
            switch (currentTokenType)
            {
                case TokenType.Number: return ParseNumberLiteral();

                case TokenType.String: return ParseStringLiteral();


                case TokenType.Identifier: return ParseVariable();

                // --- Cases for built-in no-argument functions ---
                case TokenType.KeywordGetActualX:
                case TokenType.KeywordGetActualY:
                case TokenType.KeywordGetCanvasSize:
                    return ParseSimpleFunctionCall(currentTokenSnapshot);
                // --- Cases for built-in argument functions ---
                case TokenType.KeywordIsBrushColor:
                case TokenType.KeywordIsBrushSize:
                case TokenType.KeywordIsCanvasColor:
                case TokenType.KeywordGetColorCount:
                    return ParseArgumentedFunction(currentTokenSnapshot);
                case TokenType.LParen:
                    return ParseParenthesizedExpression();

                default:
                    errors.Add($"Parser Error: Unexpected token {currentTokenType} ('{currentToken.Value}') where a primary expression was expected.");
                    return null!;
            }
        }

        //NUMBER
        private NumberLiteralNode ParseNumberLiteral()
        {
            Token numberToken = Match(TokenType.Number); // Match will advance
            NumberLiteralNode node = null!;
            if (numberToken != null)
            {
                node = new NumberLiteralNode(numberToken);
            }
            return node!;
        }

        private ExpressionNode ParseParenthesizedExpression()
        {
            if (Match(TokenType.LParen) == null) return null!;
            ExpressionNode expressionInside = ParseExpression();
            if (expressionInside == null) return null!;

            if (Match(TokenType.RParen) == null)
            {
                errors.Add("Parser Error: Missing ')' after parenthesized expression.");
                return null!;
            }
            return expressionInside;
        }

        //STRING
        private StringLiteralNode ParseStringLiteral()
        {
            Token stringToken = Match(TokenType.String);
            if (stringToken != null)
            {
                return new StringLiteralNode(stringToken);
            }
            return null!;
        }

        //VARIABLE
        private VariableNode ParseVariable()
        {
            Token identifierToken = Match(TokenType.Identifier);
            if (identifierToken != null)
            {
                return new VariableNode(identifierToken);
            }
            return null!;
        }

        private FunctionCallNode ParseSimpleFunctionCall(Token functionKeywordToken)
        {
            Advance();
            if (Match(TokenType.LParen) == null)
            {
                errors.Add($"Parser Error: Expected '(' after function keyword '{functionKeywordToken.Value}'.");
                return null!;
            }

            // Expect and Match RParen (since these are no-argument functions)
            if (Match(TokenType.RParen) == null)
            {
                // Error: Expected ')' to close function call
                errors.Add($"Parser Error: Expected ')' to close function call for '{functionKeywordToken.Value}'.");
                return null!;
            }
            return new FunctionCallNode(functionKeywordToken, new List<ExpressionNode>());
        }

        private FunctionCallNode ParseArgumentedFunction(Token functionKeywordToken)
        {
            Advance();
            if (Match(TokenType.LParen) == null)
            {
                errors.Add($"Parser Error: Expected '(' after function keyword '{functionKeywordToken.Value}'.");
                return null!;
            }

            List<ExpressionNode> expressions = ParseArgumentList();
            if (expressions == null) return null!;

            if (Match(TokenType.RParen) == null)
            {
                errors.Add($"Parser Error: Expected ')' to close argument list for function '{functionKeywordToken.Value}'.");
                return null!;
            }
            return new FunctionCallNode(functionKeywordToken, expressions);
        }

        private List<ExpressionNode> ParseArgumentList()
        {
            var arguments = new List<ExpressionNode>();

            if (currentTokenType == TokenType.RParen)
            {
                return arguments; // Empty argument list
            }

            ExpressionNode firstArgument = ParseExpression();
            if (firstArgument == null)
            {
                return null!; // Error
            }
            arguments.Add(firstArgument);

            while (currentTokenType == TokenType.Comma)
            {
                Match(TokenType.Comma);
                ExpressionNode nextArgument = ParseExpression();
                if (nextArgument == null)
                {
                    return null!;
                }
                arguments.Add(nextArgument);
            }

            return arguments;
        }
    }
}