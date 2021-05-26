using System;
using System.IO;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LinearBeats.Script
{
    [InlineProperty]
    public sealed class ScriptParser
    {
        [ShowInInspector, ReadOnly, HideLabel, MultiLineProperty(10)] [NotNull] private readonly string _rawScript;
        [NotNull] private INamingConvention _namingConvention = new NullNamingConvention();
        [NotNull] private IScriptValidator _validator = new NullScriptValidator();

        private ScriptParser([NotNull] string rawScript) => _rawScript = rawScript;

        public sealed class Builder
        {
            [NotNull] private readonly ScriptParser _base;

            public Builder([NotNull] string rawScript) => _base = new ScriptParser(rawScript);

            [NotNull] public ScriptParser Build() => _base;

            [NotNull] public Builder SetNamingConvention(NamingConventionStyle mode)
            {
                _base._namingConvention = mode switch
                {
                    NamingConventionStyle.None => new NullNamingConvention(),
                    NamingConventionStyle.CamelCase => new CamelCaseNamingConvention(),
                    NamingConventionStyle.PascalCase => new PascalCaseNamingConvention(),
                    NamingConventionStyle.Underscored => new UnderscoredNamingConvention(),
                    NamingConventionStyle.Hyphenated => new HyphenatedNamingConvention(),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };

                return this;
            }

            [NotNull] public Builder SetScriptValidator(ScriptValidatorMode mode)
            {
                _base._validator = mode switch
                {
                    ScriptValidatorMode.None => new NullScriptValidator(),
                    ScriptValidatorMode.Standard => new ScriptValidator(),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
                };

                return this;
            }
        }

        public bool TryParse(out LinearBeatsScript script)
        {
            try
            {
                script = Parse();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                script = default;
                return false;
            }
        }

        public LinearBeatsScript Parse()
        {
            var stringReader = new StringReader(_rawScript);
            var deserializer = new DeserializerBuilder().WithNamingConvention(_namingConvention).Build();

            var script = deserializer.Deserialize<LinearBeatsScript>(stringReader);
            _validator.Validate(ref script);

            return script;
        }
    }
}
