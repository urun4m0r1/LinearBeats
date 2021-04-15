using System;
using System.Diagnostics.CodeAnalysis;
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;
using static LinearBeats.EditorTests.FloatTests;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public class TimingConverterTests
    {
        private const float SamplesPerSecond = 500f;
        private const int Ppqn = 100;
        private const int FirstPulse = 0;
        private const int SecondPulse = 400;
        private const int ThirdPulse = 800;
        private const float StandardBpm = 60f;
        private const float FirstBpm = 60f;
        private const float SecondBpm = 120f;
        private const float ThirdBpm = 30f;
        private readonly Pulse _pulseA = 0;
        private readonly Pulse _pulseB = 600;
        private readonly Pulse _pulseC = 1200;
        private readonly Pulse _pulseD = 400;
        private readonly Sample _sampleA = 0;
        private readonly Sample _sampleB = 2500;
        private readonly Sample _sampleC = 7000;
        private readonly Sample _sampleD = 2000;
        private readonly Second _secondA = 0;
        private readonly Second _secondB = 5;
        private readonly Second _secondC = 14;
        private readonly Second _secondD = 4;

        private static readonly BpmEvent[] BpmEvents =
        {
            new BpmEvent {Ppqn = Ppqn, Pulse = ThirdPulse, Bpm = ThirdBpm},
            new BpmEvent {Ppqn = Ppqn, Pulse = FirstPulse, Bpm = FirstBpm},
            new BpmEvent {Ppqn = Ppqn, Pulse = SecondPulse, Bpm = SecondBpm},
        };

        private static readonly BpmEvent[] SingleBpmEvents =
        {
            new BpmEvent {Ppqn = Ppqn, Pulse = FirstPulse, Bpm = FirstBpm},
        };

        private readonly TimingConverter _converter =
            new TimingConverter(BpmEvents, StandardBpm, SamplesPerSecond);

        private readonly TimingConverter _converterSingle =
            new TimingConverter(SingleBpmEvents, StandardBpm, SamplesPerSecond);

        [Test]
        public void Init_BpmEvents_Cannot_Be_Null() =>
            Assert.Catch<NullReferenceException>(() => new TimingConverter(null!, StandardBpm, SamplesPerSecond));

        [Test]
        public void Init_BpmEvents_Cannot_Be_Empty() =>
            Assert.Catch<ArgumentException>(() => new TimingConverter(new BpmEvent[] { }, StandardBpm, SamplesPerSecond));

        [Test]
        public void Init_Any_BpmEvents_Bpm_Must_Be_Non_Zero_Positive()
        {
            BpmEvent[] bpmEvents = {new BpmEvent {Ppqn = Ppqn, Pulse = FirstPulse, Bpm = 0}};
            Assert.Catch<ArgumentException>(() => new TimingConverter(bpmEvents, StandardBpm, SamplesPerSecond));
        }

        [Test]
        public void Init_At_Least_One_BpmEvent_Pulse_Must_Be_Zero()
        {
            BpmEvent[] bpmEvents = {new BpmEvent {Ppqn = Ppqn, Pulse = 400, Bpm = FirstBpm}};

            Assert.Catch<ArgumentException>(() => new TimingConverter(bpmEvents, StandardBpm, SamplesPerSecond));
        }

        [Test]
        public void Init_Ppqn_Must_Be_Non_Zero_Positive()
        {
            BpmEvent[] bpmEvents = {new BpmEvent {Ppqn = 0, Pulse = FirstPulse, Bpm = FirstBpm}};

            Assert.Catch<ArgumentException>(() => new TimingConverter(bpmEvents, StandardBpm, SamplesPerSecond));
        }

        [Test]
        public void Init_SamplesPerSecond_Must_Be_Non_Zero_Positive()
        {
            Assert.Catch<ArgumentException>(() => new TimingConverter(SingleBpmEvents, StandardBpm, 0));
            Assert.Catch<ArgumentException>(() => new TimingConverter(SingleBpmEvents, StandardBpm, -500));
        }

        [Test]
        public void Should_Get_Index_From_Pulse()
        {
            Assert.AreEqual(0, _converter.GetTimingIndex(_pulseA), Delta);
            Assert.AreEqual(1, _converter.GetTimingIndex(_pulseB), Delta);
            Assert.AreEqual(2, _converter.GetTimingIndex(_pulseC), Delta);

            Assert.AreEqual(0, _converterSingle.GetTimingIndex(_pulseA), Delta);
            Assert.AreEqual(0, _converterSingle.GetTimingIndex(_pulseD), Delta);
        }

        [Test]
        public void Should_Get_Bpm_From_Index()
        {
            Assert.AreEqual(FirstBpm, _converter.GetBpm(0), Delta);
            Assert.AreEqual(SecondBpm, _converter.GetBpm(1), Delta);
            Assert.AreEqual(ThirdBpm, _converter.GetBpm(2), Delta);

            Assert.AreEqual(FirstBpm, _converterSingle.GetBpm(0), Delta);
        }

        [Test]
        public void Should_Convert_Pulse_To_Sample()
        {
            Assert.AreEqual(_sampleA, _converter.ToSample(_pulseA, 0), Delta);
            Assert.AreEqual(_sampleB, _converter.ToSample(_pulseB, 1), Delta);
            Assert.AreEqual(_sampleC, _converter.ToSample(_pulseC, 2), Delta);

            Assert.AreEqual(_sampleA, _converterSingle.ToSample(_pulseA, 0), Delta);
            Assert.AreEqual(_sampleD, _converterSingle.ToSample(_pulseD, 0), Delta);
        }

        [Test]
        public void Should_Convert_Sample_To_Pulse()
        {
            Assert.AreEqual(_pulseA, _converter.ToPulse(_sampleA, 0), Delta);
            Assert.AreEqual(_pulseB, _converter.ToPulse(_sampleB, 1), Delta);
            Assert.AreEqual(_pulseC, _converter.ToPulse(_sampleC, 2), Delta);

            Assert.AreEqual(_pulseA, _converterSingle.ToPulse(_sampleA, 0), Delta);
            Assert.AreEqual(_pulseD, _converterSingle.ToPulse(_sampleD, 0), Delta);
        }

        [Test]
        public void Should_Convert_Sample_To_Second()
        {
            Assert.AreEqual(_secondA, _converter.ToSecond(_sampleA), Delta);
            Assert.AreEqual(_secondB, _converter.ToSecond(_sampleB), Delta);
            Assert.AreEqual(_secondC, _converter.ToSecond(_sampleC), Delta);

            Assert.AreEqual(_secondA, _converterSingle.ToSecond(_sampleA), Delta);
            Assert.AreEqual(_secondD, _converterSingle.ToSecond(_sampleD), Delta);
        }

        [Test]
        public void Should_Convert_Second_To_Sample()
        {
            Assert.AreEqual(_sampleA, _converter.ToSample(_secondA), Delta);
            Assert.AreEqual(_sampleB, _converter.ToSample(_secondB), Delta);
            Assert.AreEqual(_sampleC, _converter.ToSample(_secondC), Delta);

            Assert.AreEqual(_sampleA, _converterSingle.ToSample(_secondA), Delta);
            Assert.AreEqual(_sampleD, _converterSingle.ToSample(_secondD), Delta);
        }
    }
}
