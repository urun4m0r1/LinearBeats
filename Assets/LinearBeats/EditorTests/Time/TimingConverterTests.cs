using System;
using System.Collections.Generic;
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
        public const int SamplesPerSecond = 500;
        private const int Ppqn = 100;
        private const int FirstPulse = 0;
        private const int SecondPulse = 400;
        private const int ThirdPulse = 800;
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
            new BpmEvent(ThirdPulse, ThirdBpm, Ppqn),
            new BpmEvent(FirstPulse, FirstBpm, Ppqn),
            new BpmEvent(SecondPulse, SecondBpm, Ppqn),
        };

        public static readonly TimingConverter Converter =
            new TimingConverter(FirstBpm, Ppqn, SamplesPerSecond, BpmEvents);

        private static readonly BpmEvent[] SingleBpmEvents = {new BpmEvent(FirstPulse, FirstBpm, Ppqn)};

        private readonly TimingConverter _converterSingle =
            new TimingConverter(FirstBpm, Ppqn, SamplesPerSecond, SingleBpmEvents);

        [Test]
        public void Init_BpmEvents_Cannot_Be_Empty() =>
            Assert.Catch<InvalidScriptException>(() =>
                new TimingConverter(FirstBpm, Ppqn, SamplesPerSecond, new BpmEvent[] { }));

        [Test]
        public void Init_Any_BpmEvents_Bpm_Must_Be_Non_Zero_Positive()
        {
            BpmEvent[] bpmEvents = {new BpmEvent(FirstPulse, 0, Ppqn)};
            Assert.Catch<InvalidScriptException>(() => InitTimingConverter(bpmEvents));
        }

        private static void InitTimingConverter(IReadOnlyCollection<BpmEvent> bpmEvents) =>
            new TimingConverter(FirstBpm, Ppqn, SamplesPerSecond, bpmEvents);

        [Test]
        public void Init_At_Least_One_BpmEvent_Pulse_Must_Be_Zero()
        {
            BpmEvent[] bpmEvents = {new BpmEvent(400, FirstBpm, Ppqn)};
            Assert.Catch<InvalidScriptException>(() => InitTimingConverter(bpmEvents));
        }

        [Test]
        public void Init_All_BpmEvent_Pulse_Must_Be_Positive()
        {
            BpmEvent[] bpmEvents = {new BpmEvent(-400, FirstBpm, Ppqn)};
            Assert.Catch<InvalidScriptException>(() => InitTimingConverter(bpmEvents));
        }

        [Test]
        public void Init_Ppqn_Must_Be_Non_Zero_Positive()
        {
            BpmEvent[] bpmEvents = {new BpmEvent(FirstPulse, FirstBpm, 0)};
            Assert.Catch<InvalidScriptException>(() => InitTimingConverter(bpmEvents));
        }

        [Test]
        public void Init_SamplesPerSecond_Must_Be_Non_Zero_Positive()
        {
            Assert.Catch<ArgumentException>(() => new TimingConverter(FirstBpm, Ppqn, 0, SingleBpmEvents));
            Assert.Catch<ArgumentException>(() => new TimingConverter(FirstBpm, Ppqn, -500, SingleBpmEvents));
        }

        [Test]
        public void Should_Get_Index_From_Pulse()
        {
            Assert.AreEqual(0, Converter.GetTimingIndex(_pulseA), Delta);
            Assert.AreEqual(1, Converter.GetTimingIndex(_pulseB), Delta);
            Assert.AreEqual(2, Converter.GetTimingIndex(_pulseC), Delta);

            Assert.AreEqual(0, _converterSingle.GetTimingIndex(_pulseA), Delta);
            Assert.AreEqual(0, _converterSingle.GetTimingIndex(_pulseD), Delta);
        }

        [Test]
        public void Should_Get_Bpm_From_Index()
        {
            Assert.AreEqual(FirstBpm, Converter.GetBpm(0), Delta);
            Assert.AreEqual(SecondBpm, Converter.GetBpm(1), Delta);
            Assert.AreEqual(ThirdBpm, Converter.GetBpm(2), Delta);

            Assert.AreEqual(FirstBpm, _converterSingle.GetBpm(0), Delta);
        }

        [Test]
        public void Should_Convert_Pulse_To_Sample()
        {
            Assert.AreEqual(_sampleA, Converter.ToSample(_pulseA, 0), Delta);
            Assert.AreEqual(_sampleB, Converter.ToSample(_pulseB, 1), Delta);
            Assert.AreEqual(_sampleC, Converter.ToSample(_pulseC, 2), Delta);

            Assert.AreEqual(_sampleA, _converterSingle.ToSample(_pulseA, 0), Delta);
            Assert.AreEqual(_sampleD, _converterSingle.ToSample(_pulseD, 0), Delta);
        }

        [Test]
        public void Should_Convert_Sample_To_Pulse()
        {
            Assert.AreEqual(_pulseA, Converter.ToPulse(_sampleA, 0), Delta);
            Assert.AreEqual(_pulseB, Converter.ToPulse(_sampleB, 1), Delta);
            Assert.AreEqual(_pulseC, Converter.ToPulse(_sampleC, 2), Delta);

            Assert.AreEqual(_pulseA, _converterSingle.ToPulse(_sampleA, 0), Delta);
            Assert.AreEqual(_pulseD, _converterSingle.ToPulse(_sampleD, 0), Delta);
        }

        [Test]
        public void Should_Convert_Sample_To_Second()
        {
            Assert.AreEqual(_secondA, Converter.ToSecond(_sampleA), Delta);
            Assert.AreEqual(_secondB, Converter.ToSecond(_sampleB), Delta);
            Assert.AreEqual(_secondC, Converter.ToSecond(_sampleC), Delta);

            Assert.AreEqual(_secondA, _converterSingle.ToSecond(_sampleA), Delta);
            Assert.AreEqual(_secondD, _converterSingle.ToSecond(_sampleD), Delta);
        }

        [Test]
        public void Should_Convert_Second_To_Sample()
        {
            Assert.AreEqual(_sampleA, Converter.ToSample(_secondA), Delta);
            Assert.AreEqual(_sampleB, Converter.ToSample(_secondB), Delta);
            Assert.AreEqual(_sampleC, Converter.ToSample(_secondC), Delta);

            Assert.AreEqual(_sampleA, _converterSingle.ToSample(_secondA), Delta);
            Assert.AreEqual(_sampleD, _converterSingle.ToSample(_secondD), Delta);
        }
    }
}
