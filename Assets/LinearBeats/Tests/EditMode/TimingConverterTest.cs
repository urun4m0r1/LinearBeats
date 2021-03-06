#pragma warning disable IDE0090
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;

[TestFixture]
public class TimingConverterTest
{
    Timing timing;
    TimingConverter converter;

    [SetUp]
    public void SetUp()
    {
        #region Given
        timing = new Timing()
        {
            PulsesPerQuarterNote = 100,
            BpmEvents = new BpmEvent[]
            {
                new BpmEvent() { Pulse = 0, Bpm = 100f },
                new BpmEvent() { Pulse = 300, Bpm = 150f },
                new BpmEvent() { Pulse = 700, Bpm = 500f },
                new BpmEvent() { Pulse = 1000, Bpm = 200f },
            }
        };
        converter = new TimingConverter(timing, 500);
        #endregion
    }

    [Test]
    public void Can_Convert_Time_To_Sample()
    {
        #region When
        Second currentSecond = 10;
        #endregion

        #region Then
        Assert.AreEqual((Sample)5000, converter.ToSample(currentSecond));
        #endregion
    }
}


