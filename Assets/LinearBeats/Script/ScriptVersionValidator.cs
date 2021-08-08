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
        VersionValidator,
    }

    public interface IScriptValidator
    {
        void Validate(ref LinearBeatsScript script);
    }

    public sealed class NullScriptValidator : IScriptValidator
    {
        public void Validate(ref LinearBeatsScript _) { }
    }

    public sealed class ScriptVersionValidator : IScriptValidator
    {
        //TODO: 버전 호환성 체크 기능 추가
        public void Validate(ref LinearBeatsScript script)
        {
            if (script.VersionCode == 0) throw new InvalidScriptException("VersionCode must be non-zero positive");
        }
    }
}
