﻿using Sirenix.Utilities;

namespace LinearBeats.Script
{
    public enum NamingConventionStyle
    {
        None,
        CamelCase,
        PascalCase,
        Underscored,
        Hyphenated,
    }

    public enum ScriptValidatorMode
    {
        None,
        Standard,
    }

    public interface IScriptValidator
    {
        void Validate(LinearBeatsScript script);
    }

    public sealed class NullScriptValidator : IScriptValidator
    {
        public void Validate(LinearBeatsScript _) { }
    }

    public sealed class ScriptValidator : IScriptValidator
    {
        //TODO: Implement ScriptValidator
        public void Validate(LinearBeatsScript script)
        {
            if (script.VersionCode == 0 || script.VersionName.IsNullOrWhitespace()) throw new InvalidScriptException();
        }
    }
}
