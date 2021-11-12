using LinearBeats.Script;
using NUnit.Framework;
using Unity.VisualScripting.YamlDotNet.Core;

namespace LinearBeats.EditorTests.Script
{
    [TestFixture]
    public class YamlDotNetTests
    {
        [Test]
        public void Throw_Exception_With_Invalid_Script()
        {
            const string invalidScript = @"
AudioChannels: []
VideoChannels:
- { Channel: 1, FileName: '', Offset: 0, Layer: 1 }
";

            var scriptParser = new ScriptParser.Builder(invalidScript).Build();
            Assert.Catch<YamlException>(() => scriptParser.Parse());
        }
    }
}
