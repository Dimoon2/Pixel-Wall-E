// In Core/Interpreter/Helpers/BinaryOperation.cs
using System;
using System.Collections.Generic;

namespace Interpreter.Core.Interpreter.Helpers
{
    public delegate object BinaryOperationHandler(object left, object right);

    public static class BinaryOperations 
    {
        private static readonly Dictionary<TokenType, BinaryOperationHandler> handlers =
             new Dictionary<TokenType, BinaryOperationHandler>
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
        private static object HandlePlus(object leftValue, object rightValue)
        {
            if (leftValue is double lDouble && rightValue is double rDouble)
                return lDouble + rDouble;
            throw new RuntimeException($"Operator '+' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        private static object HandleMinus(object leftValue, object rightValue)
        {
            if (leftValue is double lDblMin && rightValue is double rDblMin)
                return lDblMin - rDblMin;
            throw new RuntimeException($"Operator '-' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        private static object HandleMultiply(object leftValue, object rightValue)
        {
            if (leftValue is double lDblMul && rightValue is double rDblMul)
                return lDblMul * rDblMul;
            throw new RuntimeException($"Operator '*' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        private static object HandleDivide(object leftValue, object rightValue)
        {
            if (leftValue is double lDblDiv && rightValue is double rDblDiv)
            {
                if (rDblDiv == 0) throw new RuntimeException("Division by zero.");
                return lDblDiv / rDblDiv;
            }
            throw new RuntimeException($"Operator '/' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        private static object HandleModulo(object leftValue, object rightValue)
        {
            if (leftValue is double lDblMod && rightValue is double rDblMod)
            {
                if (rDblMod == 0) throw new RuntimeException("Modulo by zero.");
                return lDblMod % rDblMod;
            }
            throw new RuntimeException($"Operator '%' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        private static object HandlePower(object leftValue, object rightValue)
        {
            if (leftValue is double lDblPow && rightValue is double rDblPow)
                return Math.Pow(lDblPow, rDblPow);
            throw new RuntimeException($"Operator '**' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        private static object HandleEqualEqual(object leftValue, object rightValue)
        {
            if (leftValue is double lNumEq && rightValue is double rNumEq) return lNumEq == rNumEq;
            if (leftValue is string lStrEq && rightValue is string rStrEq) return lStrEq.Equals(rStrEq, StringComparison.OrdinalIgnoreCase);
            if (leftValue is PixelColor lColorEq && rightValue is string rStrColorEq) return lColorEq.Name.Equals(rStrColorEq.ToLower(), StringComparison.OrdinalIgnoreCase);
            if (leftValue is string lStrColorEq2 && rightValue is PixelColor rColorEq) return rColorEq.Name.Equals(lStrColorEq2.ToLower(), StringComparison.OrdinalIgnoreCase);
            if (leftValue is PixelColor lc1 && rightValue is PixelColor lc2) return lc1 == lc2;
            if (leftValue is bool lBoolEq && rightValue is bool rBoolEq) return lBoolEq == rBoolEq;
            if (leftValue is bool lBEq) return lBEq == ConvertToBooleanStatic(rightValue); 
            if (rightValue is bool rBEq) return ConvertToBooleanStatic(leftValue) == rBEq;
            throw new RuntimeException($"Operator '==' cannot compare types {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        private static object HandleLess(object leftValue, object rightValue)
        {
            if (leftValue is double lNumLt && rightValue is double rNumLt) return lNumLt < rNumLt;
            throw new RuntimeException($"Operator '<' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }
        private static object HandleLessEqual(object leftValue, object rightValue)
        {
            if (leftValue is double lNumLte && rightValue is double rNumLte) return lNumLte <= rNumLte;
            throw new RuntimeException($"Operator '<=' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }
        private static object HandleGreater(object leftValue, object rightValue)
        {
            if (leftValue is double lNumGt && rightValue is double rNumGt) return lNumGt > rNumGt;
            throw new RuntimeException($"Operator '>' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }
        private static object HandleGreaterEqual(object leftValue, object rightValue)
        {
            if (leftValue is double lNumGte && rightValue is double rNumGte) return lNumGte >= rNumGte;
            throw new RuntimeException($"Operator '>=' requires numeric operands. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.");
        }

        public static bool ConvertToBooleanStatic(object value)
        {
            if (value is bool b) return b;
            if (value is double d) return d != 0;
            if (value is int i) return i != 0; 
            if (value is string s) return !string.IsNullOrEmpty(s);
            throw new RuntimeException($"Cannot convert type {value?.GetType().Name} to Boolean for logical operation.");
        }
    }
}