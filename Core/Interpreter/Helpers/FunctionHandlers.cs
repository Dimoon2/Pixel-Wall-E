using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using PixelWallEApp.Models;
using PixelWallEApp.Models.Canvas;

using Tmds.DBus.Protocol;

namespace Interpreter.Core.Interpreter.Helpers
{
    public delegate object BuiltInFunctionHandler(List<object> evaluatedArgs, WallEState wallE, CanvasState canvas, SymbolTable symbolTable);
    public static class FunctionHandlers
    {
        private static Dictionary<string, Avalonia.Media.Color> colors = new Dictionary<string, Avalonia.Media.Color>
        {
         {"red",Colors.Red},
         {"blue",Colors.Blue},
         {"green",Colors.Green},
         {"yellow",Colors.Yellow},
         {"orange", Colors.Orange},
         {"purple",Colors.Purple},
         {"black",Colors.Black},
         {"white",Colors.White},
         {"transparent",Colors.Transparent},
        };
        public static Color GetColor(string s)
        {
            if (!colors.TryGetValue(s, out Color value))
                throw new RuntimeException("Expected a string representing a color!");
            return value;

        }
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

        public static object HandleGetActualX(List<object> evaluatedArgs, WallEState wallE, CanvasState canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 0)
            {
                throw new RuntimeException("GetActualX() takes no arguments.");
            }
            return (int)wallE.X;
        }

        public static object HandleGetActualY(List<object> evaluatedArgs, WallEState wallE, CanvasState canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 0)
            {
                throw new RuntimeException("GetActualY() takes no arguments.");
            }
            return (int)wallE.Y;
        }

        public static object HandleGetCanvasSize(List<object> evaluatedArgs, WallEState wallE, CanvasState canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 0) throw new RuntimeException("GetCanvasSize() takes no arguments.");
            return (int)canvas.Size;
        }

        public static object HandleIsBrushColor(List<object> evaluatedArgs, WallEState wallE, CanvasState canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 1 || !(evaluatedArgs[0] is string colorNameArg))
            {
                throw new RuntimeException("IsBrushColor() requires one argument (color name).");
            }
            string color = colorNameArg.ToLower();
            Color actualColor = wallE.BrushColor;
            Color ExpectedColor = GetColor(color);
            return ExpectedColor == actualColor ? 1 : 0;
        }

        public static object HandleIsBrushSize(List<object> evaluatedArgs, WallEState wallE, CanvasState canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 1 || !(evaluatedArgs[0] is double sizeArg)) // Expecting double from expression eval
            {
                throw new RuntimeException("IsBrushSize() requires one numeric argument (size).");
            }
            if (wallE.BrushSize == sizeArg) return 1;
            return 0; // Brush size is int
        }

        public static object HandleIsCanvasColor(List<object> evaluatedArgs, WallEState wallE, CanvasState canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 3 ||
                !(evaluatedArgs[1] is int yArg) ||
                !(evaluatedArgs[2] is int xArg)
                || !(evaluatedArgs[0] is string colorNameArg))
            {
                throw new RuntimeException("IsCanvasColor() requires three arguments (string colorName, numeric y, numeric x).");
            }
            int x = wallE.X;
            int y = wallE.Y;
            Color color = GetColor(colorNameArg);

            int wishx = x + xArg;
            int wishy = y + yArg;

            if (canvas.Size < wishx || canvas.Size < wishy || wishx < 0 || wishy < 0) return false;
            Color pixelColor = canvas.GetPixel(wishx, wishy);

            if (pixelColor == color)
            { return 1; }
            return 0;
        }

        // public static object HandleGetColorCount(List<object> evaluatedArgs, WallEState wallE, CanvasState canvas, SymbolTable symbolTable)
        // {

        // }
    }
}