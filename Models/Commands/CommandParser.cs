// using System;
// using System.Collections.Generic;
// using System.Text.RegularExpressions;

// namespace PixelWallEApp.Models.Commands
// {
//     public static class CommandParser
//     {
//         // Extremely basic parser focusing only on Spawn and DrawLine for now
//         // Returns list of commands or throws FormatException on error
//         public static List<ICommandDefinition> Parse(string code)
//         {
//             var commands = new List<ICommandDefinition>();
//             var lines = code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

//             for (int i = 0; i < lines.Length; i++)
//             {
//                 string line = lines[i].Trim();
//                 if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//") || line.StartsWith("#")) // Skip empty lines/comments
//                     continue;

//                 // Match Spawn(x, y)
//                 var spawnMatch = Regex.Match(line, @"^\s*Spawn\s*\(\s*(-?\d+)\s*,\s*(-?\d+)\s*\)\s*$", RegexOptions.IgnoreCase);
//                 if (spawnMatch.Success)
//                 {
//                     if (int.TryParse(spawnMatch.Groups[1].Value, out int x) &&
//                         int.TryParse(spawnMatch.Groups[2].Value, out int y))
//                     {
//                         commands.Add(new SpawnCommand(x, y));
//                         continue;
//                     }
//                     throw new FormatException($"Syntax Error (Line {i + 1}): Invalid arguments for Spawn. Expected Spawn(int, int). Found: '{line}'");
//                 }

//                 // Match DrawLine(dirX, dirY, distance)
//                 var drawLineMatch = Regex.Match(line, @"^\s*DrawLine\s*\(\s*(-?\d+)\s*,\s*(-?\d+)\s*,\s*(-?\d+)\s*\)\s*$", RegexOptions.IgnoreCase);
//                 if (drawLineMatch.Success)
//                 {
//                     if (int.TryParse(drawLineMatch.Groups[1].Value, out int dirX) &&
//                         int.TryParse(drawLineMatch.Groups[2].Value, out int dirY) &&
//                         int.TryParse(drawLineMatch.Groups[3].Value, out int dist))
//                     {
//                         // Basic validation for direction values
//                          if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1 || (dirX == 0 && dirY == 0))
//                          {
//                              throw new FormatException($"Syntax Error (Line {i + 1}): Invalid direction ({dirX},{dirY}) for DrawLine. Components must be -1, 0, or 1, and not both zero.");
//                          }
//                          if (dist < 0)
//                          {
//                              throw new FormatException($"Syntax Error (Line {i + 1}): Distance ({dist}) for DrawLine cannot be negative.");
//                          }

//                         commands.Add(new DrawLineCommand(dirX, dirY, dist));
//                         continue;
//                     }
//                     throw new FormatException($"Syntax Error (Line {i + 1}): Invalid arguments for DrawLine. Expected DrawLine(int, int, int). Found: '{line}'");
//                 }

//                 // Add other command parsing logic here (Color, Size, Fill, etc.)


//                 // If no match
//                 throw new FormatException($"Syntax Error (Line {i + 1}): Unknown command or syntax: '{line}'");
//             }

//             return commands;
//         }
//     }
// }