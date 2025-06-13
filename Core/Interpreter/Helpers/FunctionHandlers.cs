using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpreter.Core.Interpreter.Helpers
{
    public delegate object BuiltInFunctionHandler(List<object> evaluatedArgs, WallEContext wallE, Canvas canvas, SymbolTable symbolTable);
    public static class FunctionHandlers
    {
        private static Dictionary<TokenType, BuiltInFunctionHandler> handlers = new Dictionary<TokenType, BuiltInFunctionHandler>
        {
            { TokenType.KeywordGetActualX, HandleGetActualX },
            { TokenType.KeywordGetActualY, HandleGetActualY },
            { TokenType.KeywordGetCanvasSize, HandleGetCanvasSize },
            { TokenType.KeywordIsBrushColor, HandleIsBrushColor },
            { TokenType.KeywordIsBrushSize, HandleIsBrushSize },
            { TokenType.KeywordIsCanvasColor, HandleIsCanvasColor },
        //  { TokenType.KeywordGetColorCount, HandleGetColorCount },
            
            // Add True/False here if they are ever lexed as function keywords
            { TokenType.KeywordTrue, (args, w, c, st) => { if (args.Count != 0) throw new RuntimeException("true() takes no arguments."); return true; } },
            { TokenType.KeywordFalse, (args, w, c, st) => { if (args.Count != 0) throw new RuntimeException("false() takes no arguments."); return false; } }
        };

        public static bool TryGetHandler(TokenType functionKeywordType, out BuiltInFunctionHandler handler)
        {
            return handlers.TryGetValue(functionKeywordType, out handler!);
        }

        public static object HandleGetActualX(List<object> evaluatedArgs, WallEContext wallE, Canvas canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 0)
            {
                throw new RuntimeException("GetActualX() takes no arguments.");
            }
            return (double)wallE.CurrentX;
        }

        public static object HandleGetActualY(List<object> evaluatedArgs, WallEContext wallE, Canvas canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 0)
            {
                throw new RuntimeException("GetActualY() takes no arguments.");
            }
            return (double)wallE.CurrentY;
        }

        public static object HandleGetCanvasSize(List<object> evaluatedArgs, WallEContext wallE, Canvas canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 0) throw new RuntimeException("GetCanvasSize() takes no arguments.");
            return (double)canvas.Width;
        }

        public static object HandleIsBrushColor(List<object> evaluatedArgs, WallEContext wallE, Canvas canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 1 || !(evaluatedArgs[0] is string colorNameArg))
            {
                throw new RuntimeException("IsBrushColor() requires one argument (color name).");
            }
            return wallE.CurrentBrushColor.Name.Equals(colorNameArg.ToLower(), StringComparison.OrdinalIgnoreCase);
        }

        public static object HandleIsBrushSize(List<object> evaluatedArgs, WallEContext wallE, Canvas canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 1 || !(evaluatedArgs[0] is double sizeArg)) // Expecting double from expression eval
            {
                throw new RuntimeException("IsBrushSize() requires one numeric argument (size).");
            }
            if (wallE.CurrentBrushSize == sizeArg) return 1;
            return 0; // Brush size is int
        }

        public static object HandleIsCanvasColor(List<object> evaluatedArgs, WallEContext wallE, Canvas canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 3 ||
                !(evaluatedArgs[1] is int yArg) ||
                !(evaluatedArgs[2] is int xArg)
                || !(evaluatedArgs[0] is string colorNameArg))
            {
                throw new RuntimeException("IsCanvasColor() requires three arguments (string colorName, numeric y, numeric x).");
            }
            int x = wallE.CurrentX;
            int y = wallE.CurrentY;

            int wishx = x + xArg;
            int wishy = y + yArg;

            if (canvas.Width < wishx || canvas.Width < wishy) return false;
            PixelColor pixelColor = canvas.GetPixel(wishx, wishy);

            if (pixelColor != null! && pixelColor.Name == colorNameArg)
            { return 1; }
            return 0;
        }

        // public static object HandleGetColorCount(List<object> evaluatedArgs, WallEContext wallE, Canvas canvas, SymbolTable symbolTable)
        // {

        // }
    }
}