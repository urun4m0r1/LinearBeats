#pragma warning disable IDE0090

using System.IO;
using LinearBeats;
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class YamlParserTests
{
    [Test]
    public void Should_Parse_Pulse()
    {
        #region Given
        var scriptTextReader = new StringReader("Pulse: 1");
        var deserializerBuilder = new DeserializerBuilder().WithNamingConvention(new PascalCaseNamingConvention());
        IDeserializer deserializer = deserializerBuilder.Build();
        var deserializedScript = deserializer.Deserialize<MockScript>(scriptTextReader);
        #endregion

        #region When
        var number = deserializedScript.Pulse;
        #endregion

        #region Then
        Assert.AreEqual((Pulse)1, number);
        #endregion
    }

    public struct MockScript
    {
        public Pulse Pulse;
    }
}
