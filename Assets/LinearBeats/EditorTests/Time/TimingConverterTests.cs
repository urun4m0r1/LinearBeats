using System;
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class TimingConverterTests : TimingConvertingTests
    {
        [Test]
        public void Init_BpmEvents_Cannot_Be_Null()
        {
            BpmEvent[] bpmEvents = null;

            Assert.Catch<NullReferenceException>(() => new TimingConverter(bpmEvents, StandardBpm, SamplesPerSecond));
        }

        [Test]
        public void Init_BpmEvents_Cannot_Be_Empty()
        {
            BpmEvent[] bpmEvents = new BpmEvent[] { };

            Assert.Catch<ArgumentNullException>(() => new TimingConverter(bpmEvents, StandardBpm, SamplesPerSecond));
        }

        [Test]
        public void Init_Any_BpmEvents_Bpm_Must_Be_Non_Zero_Positive()
        {
            BpmEvent[] bpmEvents = new BpmEvent[]
            {
                new BpmEvent() { Ppqn = Ppqn, Pulse = FirstPulse, Bpm = 0 },
            };

            Assert.Catch<ArgumentException>(() => new TimingConverter(bpmEvents, StandardBpm, SamplesPerSecond));
        }

        [Test]
        public void Init_At_Least_One_BpmEvent_Pulse_Must_Be_Zero()
        {
            BpmEvent[] bpmEvents = new BpmEvent[]
            {
                new BpmEvent() { Ppqn = Ppqn, Pulse = 400, Bpm = FirstBpm },
            };

            Assert.Catch<ArgumentException>(() => new TimingConverter(bpmEvents, StandardBpm, SamplesPerSecond));
        }

        [Test]
        public void Init_Ppqn_Must_Be_Non_Zero_Positive()
        {
            BpmEvent[] bpmEvents = new BpmEvent[]
            {
                new BpmEvent() { Ppqn = 0, Pulse = FirstPulse, Bpm = FirstBpm },
            };

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
            TestUtils.AreEqual(0, ConverterDisorder.GetTimingIndex(PulseA));
            TestUtils.AreEqual(1, ConverterDisorder.GetTimingIndex(PulseB));
            TestUtils.AreEqual(2, ConverterDisorder.GetTimingIndex(PulseC));

            TestUtils.AreEqual(0, ConverterSingle.GetTimingIndex(PulseA));
            TestUtils.AreEqual(0, ConverterSingle.GetTimingIndex(PulseD));
        }

        [Test]
        public void Should_Get_Bpm_From_Index()
        {
            TestUtils.AreEqual(FirstBpm, ConverterDisorder.GetBpm(0));
            TestUtils.AreEqual(SecondBpm, ConverterDisorder.GetBpm(1));
            TestUtils.AreEqual(ThirdBpm, ConverterDisorder.GetBpm(2));

            TestUtils.AreEqual(FirstBpm, ConverterSingle.GetBpm(0));
        }

        [Test]
        public void Should_Convert_Pulse_To_Sample()
        {
            TestUtils.AreEqual(SampleA, ConverterDisorder.ToSample(PulseA, 0));
            TestUtils.AreEqual(SampleB, ConverterDisorder.ToSample(PulseB, 1));
            TestUtils.AreEqual(SampleC, ConverterDisorder.ToSample(PulseC, 2));

            TestUtils.AreEqual(SampleA, ConverterSingle.ToSample(PulseA, 0));
            TestUtils.AreEqual(SampleD, ConverterSingle.ToSample(PulseD, 0));
        }

        [Test]
        public void Should_Convert_Sample_To_Pulse()
        {
            TestUtils.AreEqual(PulseA, ConverterDisorder.ToPulse(SampleA, 0));
            TestUtils.AreEqual(PulseB, ConverterDisorder.ToPulse(SampleB, 1));
            TestUtils.AreEqual(PulseC, ConverterDisorder.ToPulse(SampleC, 2));

            TestUtils.AreEqual(PulseA, ConverterSingle.ToPulse(SampleA, 0));
            TestUtils.AreEqual(PulseD, ConverterSingle.ToPulse(SampleD, 0));
        }

        [Test]
        public void Should_Convert_Sample_To_Second()
        {
            TestUtils.AreEqual(SecondA, ConverterDisorder.ToSecond(SampleA));
            TestUtils.AreEqual(SecondB, ConverterDisorder.ToSecond(SampleB));
            TestUtils.AreEqual(SecondC, ConverterDisorder.ToSecond(SampleC));

            TestUtils.AreEqual(SecondA, ConverterSingle.ToSecond(SampleA));
            TestUtils.AreEqual(SecondD, ConverterSingle.ToSecond(SampleD));
        }

        [Test]
        public void Should_Convert_Second_To_Sample()
        {
            TestUtils.AreEqual(SampleA, ConverterDisorder.ToSample(SecondA));
            TestUtils.AreEqual(SampleB, ConverterDisorder.ToSample(SecondB));
            TestUtils.AreEqual(SampleC, ConverterDisorder.ToSample(SecondC));

            TestUtils.AreEqual(SampleA, ConverterSingle.ToSample(SecondA));
            TestUtils.AreEqual(SampleD, ConverterSingle.ToSample(SecondD));
        }
    }
}
