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
        // Keywords for potential future LOAD/SAVE/EVAL if handled by the language itself
        // KeywordLoad,
        // KeywordSave,
        // KeywordEval,

        // Keywords - Built-in Functions 
        KeywordGetActualX,
        KeywordGetActualY,
        KeywordGetCanvasSize, // Assuming a corresponding height function, or GetCanvasSize returns a pair/object
        KeywordGetColorCount,
        KeywordIsBrushColor,
        KeywordIsBrushSize,
        KeywordIsCanvasColor,
        KeywordTrue, // For boolean literals 'true'
        KeywordFalse, // For boolean literals 'false'
                      // The spec for GoTo (condition) implies expressions evaluate to numbers (0 for false, non-0 for true)
                      // So, explicit true/false keywords might not be strictly needed yet if we follow that.

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
        Illegal         // Optional: for tokens that are not valid
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }    // The raw lexeme, e.g., "Spawn", "<-", "\"Red\""
        public object Literal { get; }  // The actual value, e.g., int 5, string "Red" (no quotes)

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