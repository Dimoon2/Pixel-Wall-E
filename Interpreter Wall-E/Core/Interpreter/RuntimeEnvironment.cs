using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Interpreter.Core.Ast.Statements; // For ProgramNode

namespace Interpreter.Core.Interpreter
{
    public class RuntimeEnvironment
    {
        public int ProgramCounter { get; set; } //index of the current statement
        private Dictionary<string, int> labels;
        public bool GoToPending { get; set; }
        public string TargetLabel { get; set; }

        public RuntimeEnvironment()
        {
            labels = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            ProgramCounter = 0;
            GoToPending = false;
        }

        public void Reset()
        {
            ProgramCounter = 0;
            labels.Clear();
            GoToPending = false;
            labels = null!;
        }

        public void ScanLabels(ProgramNode programNode) //once before execution
        {
            labels.Clear();
            if (programNode == null || programNode.Statements == null) return;
            for (int i = 0; i < programNode.Statements.Count; i++)
            {
                if (programNode.Statements[i] is LabelNode labelNode)
                {
                    string labelName = labelNode.Name;
                    if (labels.ContainsKey(labelName))
                    {
                        throw new RuntimeException($"Runtime Error: Duplicate label definition for '{labelName}'.");
                    }
                    labels.Add(labelName, i);
                }
            }
        }

        public bool TryGetLabelIndex(string labelName, out int index)
        {
            return labels.TryGetValue(labelName, out index);
        }

        public void RequestGoTo(string labelName)
        {
            if (TryGetLabelIndex(labelName, out _)) // Check if label exists
            {
                GoToPending = true;
                TargetLabel = labelName;
            }
            else
            {
                throw new RuntimeException($"Runtime Error: GoTo target label '{labelName}' not found.");
            }
        }

        // Call after each statement execution cycle to see if we need to jump
        public void ProcessPendingGoTo()
        {
            if (GoToPending)
            {
                if (TryGetLabelIndex(TargetLabel, out int targetIndex))
                {
                    ProgramCounter = targetIndex; // Set PC to the statement *at* the label
                }
                else
                {
                    // This should have been caught by RequestGoTo, but as a safeguard:
                    throw new RuntimeException($"Runtime Error: Target label '{TargetLabel}' for GoTo disappeared.");
                }
                GoToPending = false;
                TargetLabel = null!;
            }
        }
    }
}