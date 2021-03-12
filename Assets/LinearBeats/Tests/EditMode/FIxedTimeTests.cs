#pragma warning disable IDE0090

using System;
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;

[TestFixture]
public class FixedTImeTests
{
    private const float SamplesPerSecond = 500f;
    private const int PulsesPerQuarterNote = 100;
    private const int FirstPulse = 0;
    private const int ThirdPulse = 800;
    private const int SecondPulse = 400;
    private const float FirstBpm = 60f;
    private const float SecondBpm = 120f;
    private const float ThirdBpm = 30f;
    private readonly Pulse pulseA = 0;
    private readonly Pulse pulseB = 600;
    private readonly Pulse pulseC = 1200;
    private readonly Pulse pulseD = 400;
    private readonly Sample sampleA = 0;
    private readonly Sample sampleB = 2500;
    private readonly Sample sampleC = 7000;
    private readonly Sample sampleD = 2000;
    private readonly Second secondA = 0;
    private readonly Second secondB = 5;
    private readonly Second secondC = 14;
    private readonly Second secondD = 4;

    private Timing timingDisorder;
    private Timing timingSingle;
    private TimingConverter converterDisorder = null;
    private TimingConverter converterSingle = null;

    [SetUp]
    public void SetUp()
    {
        timingDisorder = new Timing()
        {
            PulsesPerQuarterNote = PulsesPerQuarterNote,
            BpmEvents = new BpmEvent[]
            {
                new BpmEvent() { Pulse = ThirdPulse, Bpm = ThirdBpm },
                new BpmEvent() { Pulse = FirstPulse, Bpm = FirstBpm },
                new BpmEvent() { Pulse = SecondPulse, Bpm = SecondBpm },
            }
        };

        timingSingle = new Timing()
        {
            PulsesPerQuarterNote = PulsesPerQuarterNote,
            BpmEvents = new BpmEvent[]
            {
                new BpmEvent() { Pulse = FirstPulse, Bpm = FirstBpm },
            }
        };

        converterDisorder = new TimingConverter(timingDisorder, SamplesPerSecond);
        converterSingle = new TimingConverter(timingSingle, SamplesPerSecond);
    }

    [Test]
    public void Init_BpmEvents_Cannot_Be_Null()
    {
        timingSingle.BpmEvents = null;

        Assert.Catch<ArgumentNullException>(() => new TimingConverter(timingSingle, SamplesPerSecond));
    }

    [Test]
    public void Init_BpmEvents_Cannot_Be_Empty()
    {
        timingSingle.BpmEvents = new BpmEvent[] { };

        Assert.Catch<ArgumentNullException>(() => new TimingConverter(timingSingle, SamplesPerSecond));
    }

    [Test]
    public void Init_Any_BpmEvents_Bpm_Must_Be_Non_Zero_Positive()
    {
        timingSingle.BpmEvents = new BpmEvent[]
        {
            new BpmEvent() { Pulse = FirstPulse, Bpm = 0 },
        };

        Assert.Catch<ArgumentException>(() => new TimingConverter(timingSingle, SamplesPerSecond));
    }

    [Test]
    public void Init_At_Least_One_BpmEvent_Pulse_Must_Be_Zero()
    {
        timingSingle.BpmEvents = new BpmEvent[]
        {
            new BpmEvent() { Pulse = 400, Bpm = FirstBpm },
        };

        Assert.Catch<ArgumentException>(() => new TimingConverter(timingSingle, SamplesPerSecond));
    }

    [Test]
    public void Init_PulsesPerQuarterNote_Must_Be_Non_Zero_Positive()
    {
        timingSingle.PulsesPerQuarterNote = 0;

        Assert.Catch<ArgumentException>(() => new TimingConverter(timingSingle, SamplesPerSecond));
    }

    [Test]
    public void Init_SamplesPerSecond_Must_Be_Non_Zero_Positive()
    {
        Assert.Catch<ArgumentException>(() => new TimingConverter(timingSingle, 0));
        Assert.Catch<ArgumentException>(() => new TimingConverter(timingSingle, -500));
    }

    [Test]
    public void Should_Get_Bpm_From_Pulse()
    {
        AreEqual(FirstBpm, converterDisorder.GetBpm(pulseA));
        AreEqual(SecondBpm, converterDisorder.GetBpm(pulseB));
        AreEqual(ThirdBpm, converterDisorder.GetBpm(pulseC));

        AreEqual(FirstBpm, converterSingle.GetBpm(pulseA));
        AreEqual(FirstBpm, converterSingle.GetBpm(pulseD));
    }

    [Test]
    public void Should_Get_Bpm_From_Sample()
    {
        AreEqual(FirstBpm, converterDisorder.GetBpm(sampleA));
        AreEqual(SecondBpm, converterDisorder.GetBpm(sampleB));
        AreEqual(ThirdBpm, converterDisorder.GetBpm(sampleC));

        AreEqual(FirstBpm, converterSingle.GetBpm(sampleA));
        AreEqual(FirstBpm, converterSingle.GetBpm(sampleD));
    }

    [Test]
    public void Should_Get_Bpm_From_Second()
    {
        AreEqual(FirstBpm, converterDisorder.GetBpm(secondA));
        AreEqual(SecondBpm, converterDisorder.GetBpm(secondB));
        AreEqual(ThirdBpm, converterDisorder.GetBpm(secondC));

        AreEqual(FirstBpm, converterSingle.GetBpm(secondA));
        AreEqual(FirstBpm, converterSingle.GetBpm(secondD));
    }

    [Test]
    public void Should_Convert_Pulse_To_Sample()
    {
        AreEqual(sampleA, converterDisorder.ToSample(pulseA));
        AreEqual(sampleB, converterDisorder.ToSample(pulseB));
        AreEqual(sampleC, converterDisorder.ToSample(pulseC));

        AreEqual(sampleA, converterSingle.ToSample(pulseA));
        AreEqual(sampleD, converterSingle.ToSample(pulseD));
    }

    [Test]
    public void Should_Convert_Sample_To_Pulse()
    {
        AreEqual(pulseA, converterDisorder.ToPulse(sampleA));
        AreEqual(pulseB, converterDisorder.ToPulse(sampleB));
        AreEqual(pulseC, converterDisorder.ToPulse(sampleC));

        AreEqual(pulseA, converterSingle.ToPulse(sampleA));
        AreEqual(pulseD, converterSingle.ToPulse(sampleD));
    }

    [Test]
    public void Should_Convert_Sample_To_Second()
    {
        AreEqual(secondA, converterDisorder.ToSecond(sampleA));
        AreEqual(secondB, converterDisorder.ToSecond(sampleB));
        AreEqual(secondC, converterDisorder.ToSecond(sampleC));

        AreEqual(secondA, converterSingle.ToSecond(sampleA));
        AreEqual(secondD, converterSingle.ToSecond(sampleD));
    }

    [Test]
    public void Should_Convert_Second_To_Sample()
    {
        AreEqual(sampleA, converterDisorder.ToSample(secondA));
        AreEqual(sampleB, converterDisorder.ToSample(secondB));
        AreEqual(sampleC, converterDisorder.ToSample(secondC));

        AreEqual(sampleA, converterSingle.ToSample(secondA));
        AreEqual(sampleD, converterSingle.ToSample(secondD));
    }

    [Test]
    public void Should_Convert_Second_To_Pulse()
    {
        AreEqual(pulseA, converterDisorder.ToPulse(secondA));
        AreEqual(pulseB, converterDisorder.ToPulse(secondB));
        AreEqual(pulseC, converterDisorder.ToPulse(secondC));

        AreEqual(pulseA, converterSingle.ToPulse(secondA));
        AreEqual(pulseD, converterSingle.ToPulse(secondD));
    }

    [Test]
    public void Should_Convert_Pulse_To_Second()
    {
        AreEqual(secondA, converterDisorder.ToSecond(pulseA));
        AreEqual(secondB, converterDisorder.ToSecond(pulseB));
        AreEqual(secondC, converterDisorder.ToSecond(pulseC));

        AreEqual(secondA, converterSingle.ToSecond(pulseA));
        AreEqual(secondD, converterSingle.ToSecond(pulseD));
    }

    private void AreEqual(float expected, float actual)
    {
        Assert.AreEqual(expected, actual, 0.0001);
    }
}


