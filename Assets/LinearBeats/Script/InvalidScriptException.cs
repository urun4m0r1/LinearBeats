using System;

namespace LinearBeats.Script
{
    [Serializable]
    public sealed class InvalidScriptException : Exception
    {
        public InvalidScriptException() { }

        public InvalidScriptException(string message) : base(message) { }

        public InvalidScriptException(string message, Exception inner) : base(message, inner) { }
    }
}
