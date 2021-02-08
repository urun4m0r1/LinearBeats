using System.IO;
using Sirenix.Utilities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LinearBeats.Script
{
    public sealed class ScriptParser
    {
        private string _scriptText = null;

        public string ScriptText
        {
            private get => _scriptText;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new System.ArgumentNullException();
                }

                _scriptText = value;
            }
        }

        public ScriptParser(string scriptText)
        {
            ScriptText = scriptText;
        }

        public LinearBeatsScript Parse()
        {
            var scriptTextReader = new StringReader(ScriptText);
            var deserializerBuilder = new DeserializerBuilder().WithNamingConvention(new PascalCaseNamingConvention());
            IDeserializer deserializer = deserializerBuilder.Build();
            LinearBeatsScript deserializedScript = deserializer.Deserialize<LinearBeatsScript>(scriptTextReader);
            return Validate(deserializedScript);
        }

        private LinearBeatsScript Validate(LinearBeatsScript script)
        {
            if (
                script.VersionCode == 0
                || script.VersionName.IsNullOrWhitespace()
            ) throw new InvalidDataException();

            return script;
        }
    }
}
