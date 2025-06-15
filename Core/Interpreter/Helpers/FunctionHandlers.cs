using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
            if (!colors.TryGetValue(s.ToLower(), out Color value))
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
            { TokenType.KeywordGetColorCount, HandleGetColorCount },
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

        public static object HandleGetColorCount(List<object> evaluatedArgs, WallEState wallE, CanvasState canvas, SymbolTable symbolTable)
        {
            if (evaluatedArgs.Count != 5 ||
                           !(evaluatedArgs[0] is string color) ||
                           !(evaluatedArgs[1] is int x1) ||
                           !(evaluatedArgs[2] is int y1) ||
                           !(evaluatedArgs[3] is int x2) ||
                           !(evaluatedArgs[2] is int y2))
            {
                throw new RuntimeException("GetColorCount() requires five arguments (string color, numeric x1, numeric y1, numeric x2 and numeric y2).");
            }

            if (!canvas.IsValidPosition(x1) || !canvas.IsValidPosition(x2) || !canvas.IsValidPosition(y1) || !canvas.IsValidPosition(y2))
            {
                throw new RuntimeException("Not valid coordinates in GetColorCount arguments.");
            }

            int counter = 0;
            Color convertedColor = GetColor(color);
            if (x1 <= x2 && y1 <= y2)
            {
                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if (canvas.GetPixel(i, j) == convertedColor)
                        {
                            counter++;

                        }
                    }
                }
                return counter;
            }
            if (x1 >= x2 && y1 >= y2)
            {
                for (int i = x2; i <= x1; i++)
                {
                    for (int j = y2; j <= y1; j++)
                    {
                        if (canvas.GetPixel(i, j) == convertedColor)
                        {
                            counter++;
                        }
                    }
                }
                return counter;
            }
            if (x1 >= x2 && y1 <= y2)
            {
                for (int i = y1; i <= y2; i++)
                {
                    for (int j = x2; j <= x1; j++)
                    {
                        if (canvas.GetPixel(i, j) == convertedColor)
                        {
                            counter++;

                        }
                    }
                }
                return counter;
            }
            else
            {
                for (int i = y2; i <= y1; i++)
                {
                    for (int j = x1; j <= x2; j++)
                    {
                        if (canvas.GetPixel(i, j) == convertedColor)
                        {
                            counter++;

                        }
                    }
                }
                return counter;
            }
        }
    }
}