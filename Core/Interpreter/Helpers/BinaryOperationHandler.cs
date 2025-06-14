// In Core/Interpreter/Helpers/BinaryOperation.cs
using System;
using System.Collections.Generic;
using Avalonia.Media; //forcolors

namespace Interpreter.Core.Interpreter.Helpers
{
    public delegate object BinaryOperationHandler(object left, object right);

    public static class BinaryOperations
    {
        private static Dictionary<TokenType, BinaryOperationHandler> handlers = new Dictionary<TokenType, BinaryOperationHandler>
             {
                // Arithmetic
                { TokenType.Plus, HandlePlus },
                { TokenType.Minus, HandleMinus },
                { TokenType.Multiply, HandleMultiply },
                { TokenType.Divide, HandleDivide },
                { TokenType.Modulo, HandleModulo },
                { TokenType.Power, HandlePower },

                // Comparison
                { TokenType.EqualEqual, HandleEqualEqual },
                { TokenType.Less, HandleLess },
                { TokenType.LessEqual, HandleLessEqual },
                { TokenType.Greater, HandleGreater },
                { TokenType.GreaterEqual, HandleGreaterEqual },

                 // Logical 
                 { TokenType.And, (l, r) => ConvertToBooleanStatic(l) && ConvertToBooleanStatic(r) },
                 { TokenType.Or,  (l, r) => ConvertToBooleanStatic(l) || ConvertToBooleanStatic(r) }
             };

        //To get a handler
        public static bool TryGetHandler(TokenType operatorType, out BinaryOperationHandler handler)
        {
            return handlers.TryGetValue(operatorType, out handler!);
        }

        // --- Handler methods for each operation --- :)
        public static object HandlePlus(object leftValue, object rightValue)
        {
            if (leftValue is int lint && rightValue is int rint)
                return lint + rint;
            throw new RuntimeException($"Operator '+' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        public static object HandleMinus(object leftValue, object rightValue)
        {
            if (leftValue is int lDblMin && rightValue is int rDblMin)
                return lDblMin - rDblMin;
            throw new RuntimeException($"Operator '-' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        public static object HandleMultiply(object leftValue, object rightValue)
        {
            if (leftValue is int lDblMul && rightValue is int rDblMul)
                return lDblMul * rDblMul;
            throw new RuntimeException($"Operator '*' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        public static object HandleDivide(object leftValue, object rightValue)
        {
            if (leftValue is int lDblDiv && rightValue is int rDblDiv)
            {
                if (rDblDiv == 0) throw new RuntimeException("Division by zero.");
                return lDblDiv / rDblDiv;
            }
            throw new RuntimeException($"Operator '/' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        public static object HandleModulo(object leftValue, object rightValue)
        {
            if (leftValue is int lDblMod && rightValue is int rDblMod)
            {
                if (rDblMod == 0) throw new RuntimeException("Modulo by zero.");
                return lDblMod % rDblMod;
            }
            throw new RuntimeException($"Operator '%' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        public static object HandlePower(object leftValue, object rightValue)
        {
            if (leftValue is int lDblPow && rightValue is int rDblPow)
                return (int)Math.Pow(lDblPow, rDblPow);
            throw new RuntimeException($"Operator '**' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        public static object HandleEqualEqual(object leftValue, object rightValue)
        {
            if (leftValue is int lNumEq && rightValue is int rNumEq) return lNumEq == rNumEq;
            if (leftValue is string lStrEq && rightValue is string rStrEq) return lStrEq.Equals(rStrEq, StringComparison.OrdinalIgnoreCase);
            if (leftValue is Color lColorEq && rightValue is string rStrColorEq) return lColorEq == FunctionHandlers.GetColor(rStrColorEq) ? true: false;
            if (leftValue is string lStrColorEq2 && rightValue is Color rColorEq) return  FunctionHandlers.GetColor(lStrColorEq2) == rColorEq ? true : false;
            if (leftValue is Color lc1 && rightValue is Color lc2) return lc1 == lc2;
            if (leftValue is bool lBoolEq && rightValue is bool rBoolEq) return lBoolEq == rBoolEq;
            if (leftValue is bool lBEq) return lBEq == ConvertToBooleanStatic(rightValue);
            if (rightValue is bool rBEq) return ConvertToBooleanStatic(leftValue) == rBEq;
            throw new RuntimeException($"Operator '==' cannot compare types {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        public static object HandleLess(object leftValue, object rightValue)
        {
            if (leftValue is int lNumLt && rightValue is int rNumLt) return lNumLt < rNumLt;
            throw new RuntimeException($"Operator '<' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }
        public static object HandleLessEqual(object leftValue, object rightValue)
        {
            if (leftValue is int lNumLte && rightValue is int rNumLte) return lNumLte <= rNumLte;
            throw new RuntimeException($"Operator '<=' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }
        public static object HandleGreater(object leftValue, object rightValue)
        {
            if (leftValue is int lNumGt && rightValue is int rNumGt) return lNumGt > rNumGt;
            throw new RuntimeException($"Operator '>' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }
        public static object HandleGreaterEqual(object leftValue, object rightValue)
        {
            if (leftValue is int lNumGte && rightValue is int rNumGte) return lNumGte >= rNumGte;
            throw new RuntimeException($"Operator '>=' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        public static bool ConvertToBooleanStatic(object value)
        {
            if (value is bool b) return b;
            if (value is int d) return d != 0;
            if (value is int i) return i != 0;
            if (value is string s) return !string.IsNullOrEmpty(s);
            throw new RuntimeException($"Cannot convert type {value?.GetType().Name} to Boolean for logical operation.");
        }
    }
}