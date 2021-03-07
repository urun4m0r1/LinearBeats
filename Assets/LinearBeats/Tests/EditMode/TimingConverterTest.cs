#pragma warning disable IDE0090
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;

[TestFixture]
public class TimingConverterTest
{
    //TODO: 예외처리들이 잘 작동하는지 확인
    //TODO: 변속이 없는 경우 예외 처리하기

    Timing timing;
    TimingConverter converter = null;

    [SetUp]
    public void SetUp()
    {
        timing = new Timing()
        {
            PulsesPerQuarterNote = 100,
            BpmEvents = new BpmEvent[]
            {
                new BpmEvent() { Pulse = 800, Bpm = 30f },
                new BpmEvent() { Pulse = 0, Bpm = 60f },
                new BpmEvent() { Pulse = 400, Bpm = 120f },
            }
        };
        converter = new TimingConverter(timing, 500f);
    }

    [Test]
    public void Should_Get_Bpm_From_Pulse()
    {
        AreEqual(60f, converter.GetBpm((Pulse)0));
        AreEqual(120f, converter.GetBpm((Pulse)600));
        AreEqual(30f, converter.GetBpm((Pulse)1200));
    }

    [Test]
    public void Should_Get_Bpm_From_Sample()
    {
        AreEqual(60f, converter.GetBpm((Sample)0));
        AreEqual(120f, converter.GetBpm((Sample)2500));
        AreEqual(30f, converter.GetBpm((Sample)7000));
    }

    [Test]
    public void Should_Get_Bpm_From_Second()
    {
        AreEqual(60f, converter.GetBpm((Second)0));
        AreEqual(120f, converter.GetBpm((Second)5));
        AreEqual(30f, converter.GetBpm((Second)14));
    }

    [Test]
    public void Should_Get_Bpm_From_Index()
    {
        AreEqual(60f, converter.GetBpm(0));
        AreEqual(120f, converter.GetBpm(1));
        AreEqual(30f, converter.GetBpm(2));
    }

    [Test]
    public void Should_Convert_Pulse_To_Sample()
    {
        AreEqual((Sample)0, converter.ToSample((Pulse)0));
        AreEqual((Sample)2500, converter.ToSample((Pulse)600));
        AreEqual((Sample)7000, converter.ToSample((Pulse)1200));
    }

    [Test]
    public void Should_Convert_Sample_To_Pulse()
    {
        AreEqual((Pulse)0, converter.ToPulse((Sample)0));
        AreEqual((Pulse)600, converter.ToPulse((Sample)2500));
        AreEqual((Pulse)1200, converter.ToPulse((Sample)7000));
    }

    [Test]
    public void Should_Convert_Sample_To_Second()
    {
        AreEqual((Second)0, converter.ToSecond((Sample)0));
        AreEqual((Second)5, converter.ToSecond((Sample)2500));
        AreEqual((Second)14, converter.ToSecond((Sample)7000));
    }

    [Test]
    public void Should_Convert_Second_To_Sample()
    {
        AreEqual((Sample)0, converter.ToSample((Second)0));
        AreEqual((Sample)2500, converter.ToSample((Second)5));
        AreEqual((Sample)7000, converter.ToSample((Second)14));
    }

    [Test]
    public void Should_Convert_Second_To_Pulse()
    {
        AreEqual((Pulse)0, converter.ToPulse((Second)0));
        AreEqual((Pulse)600, converter.ToPulse((Second)5));
        AreEqual((Pulse)1200, converter.ToPulse((Second)14));
    }

    [Test]
    public void Should_Convert_Pulse_To_Second()
    {
        AreEqual((Second)0, converter.ToSecond((Pulse)0));
        AreEqual((Second)5, converter.ToSecond((Pulse)600));
        AreEqual((Second)14, converter.ToSecond((Pulse)1200));
    }

    [Test]
    public void Should_Get_Index_From_Pulse()
    {
        AreEqual(0, converter.GetTimingIndex((Pulse)0));
        AreEqual(1, converter.GetTimingIndex((Pulse)600));
        AreEqual(2, converter.GetTimingIndex((Pulse)1200));
    }

    [Test]
    public void Should_Get_Index_From_Sample()
    {
        AreEqual(0, converter.GetTimingIndex((Sample)0));
        AreEqual(1, converter.GetTimingIndex((Sample)2500));
        AreEqual(2, converter.GetTimingIndex((Sample)7000));
    }

    [Test]
    public void Should_Get_Index_From_Second()
    {
        AreEqual(0, converter.GetTimingIndex((Second)0));
        AreEqual(1, converter.GetTimingIndex((Second)5));
        AreEqual(2, converter.GetTimingIndex((Second)14));
    }

    private void AreEqual(float expected, float actual)
    {
        Assert.AreEqual(expected, actual, 0.0001);
    }
}


