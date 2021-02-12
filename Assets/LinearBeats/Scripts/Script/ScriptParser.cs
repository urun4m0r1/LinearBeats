using System.IO;
using Sirenix.Utilities;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LinearBeats.Script
{
    public sealed class ScriptParser
    {
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
        private string _scriptText = null;

        public ScriptParser(string scriptText)
        {
            ScriptText = scriptText;
        }

        public static LinearBeatsScript ParseFromResources(string scriptPath)
        {
            var scriptAsset = Resources.Load(scriptPath) as TextAsset;
            return new ScriptParser(scriptAsset.text).Parse();
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
