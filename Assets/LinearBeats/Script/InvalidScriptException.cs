using System;
using YamlDotNet.Core;

namespace LinearBeats.Script
{
    public sealed class InvalidScriptException : YamlException
    {
        public InvalidScriptException() { }

        public InvalidScriptException(string message) : base(message) { }

        public InvalidScriptException(string message, Exception inner) : base(message, inner) { }
    }
}
