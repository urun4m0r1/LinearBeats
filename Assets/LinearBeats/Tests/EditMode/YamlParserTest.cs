#pragma warning disable IDE0090

using System.IO;
using LinearBeats.Time;
using NUnit.Framework;
using Utils.Extensions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class YamlParserTest
{
    [Test]
    public void Can_Convert_Time_To_Sample()
    {
        #region Given
        //TODO Pulse, Time, Sample를 각각 구조체를 만들어 사용하게 만들기
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






