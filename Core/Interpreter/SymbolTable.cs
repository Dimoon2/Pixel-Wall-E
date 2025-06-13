using System;
using System.Collections.Generic;

namespace Interpreter.Core.Interpreter
{
    public class SymbolTable
    {
        private Dictionary<string, object> symbols;
        private SymbolTable parent;

        public SymbolTable() : this(null!) { }
        public SymbolTable(SymbolTable parentScope)
        {
            symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase); // Case-insensitive for variable names
            parent = parentScope;
        }

        public bool Define(string Name, object value)
        {
            symbols[Name] = value;
            return true;
        }

        public bool Assign(string Name, object value)
        {
            if (symbols.ContainsKey(Name))
            {
                symbols[Name] = value;
                return true;
            }
            if (parent != null)
            {
                return parent.Assign(Name, value);
            }
            // Let's make Assign also define if not found in any scope for simplicity matching 'n <- expr'
            symbols[Name] = value; // Define it in the current scope
            return true;
        }

        public object Get(string name)
        {
            if (symbols.TryGetValue(name, out object value))
            {
                return value;
            }

            if (parent != null)
            {
                return parent.Get(name); // Check in parent scope
            }
            throw new RuntimeException($"Runtime Error: Variable '{name}' is not defined.");
        }
        public bool IsDefined(string name)
        {
            if (symbols.ContainsKey(name))
            {
                return true;
            }
            if (parent != null)
            {
                return parent.IsDefined(name);
            }
            return false;
        }
        public void Clear()
        {
            symbols.Clear();
            // Parent scope remains, only current scope is cleared.
        }
    }
    public class RuntimeException : Exception
    {
        public RuntimeException(string message) : base(message) { }
    }
}