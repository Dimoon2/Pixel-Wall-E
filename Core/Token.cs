namespace Interpreter.Core
{
    public enum TokenType
    {
        //Literals
        Number,
        String,
        Identifier,

        // Keywords - Commands
        KeywordSpawn,
        KeywordColor,
        KeywordSize,
        KeywordDrawLine,
        KeywordDrawCircle,
        KeywordDrawRectangle,
        KeywordFill,
        KeywordGoTo,

        // Keywords - Built-in Functions 
        KeywordGetActualX,
        KeywordGetActualY,
        KeywordGetCanvasSize, 
        KeywordGetColorCount,
        KeywordIsBrushColor,
        KeywordIsBrushSize,
        KeywordIsCanvasColor,
        KeywordTrue, 
        KeywordFalse, 

        //Operators
        Plus, //+
        Minus, //-
        Multiply, //*
        Divide, // /
        Power, // **
        Modulo, // %
        Underscore, // _

        Arrow,          // <- (Assignment)
        EqualEqual,     // == (Comparison)
        GreaterEqual,   // >=
        LessEqual,      // <=
        Greater,        // >
        Less,           // <
        Comment,        // //
        And, // &
        Or, // ||
        Not, // !

        // Punctuation
        LParen,         // (
        RParen,         // )
        LBracket,       // [
        RBracket,       // ]
        Comma,          // ,
        Colon,

        // Control & Whitespace
        Newline,        // Represents one or more new line characters \n, \r, \r\n
        EndOfFile,      // EOF

        // For errors or unrecognized tokens
        Illegal         //for tokens that are not valid
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }    // The raw lexeme
        public object Literal { get; }  // The actual value

        public Token(TokenType type, string value, object literal = null!)
        {
            Type = type;
            Value = value;
            Literal = literal;
        }

        public override string ToString()
        {
            return $"Type: {Type}, Value: '{Value}'{(Literal != null ? $", Literal: {Literal}" : "")}";
        }


    }
}