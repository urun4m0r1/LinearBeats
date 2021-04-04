#pragma warning disable IDE0090

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

            Assert.Catch<ArgumentNullException>(() => new TimingConverter(bpmEvents, StandardBpm, SamplesPerSecond));
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
            Assert.Catch<ArgumentException>(() => new TimingConverter(singleBpmEvents, StandardBpm, 0));
            Assert.Catch<ArgumentException>(() => new TimingConverter(singleBpmEvents, StandardBpm, -500));
        }

        [Test]
        public void Should_Get_Bpm_From_Pulse()
        {
            TestUtils.AreEqual(FirstBpm, converterDisorder.GetBpm(pulseA));
            TestUtils.AreEqual(SecondBpm, converterDisorder.GetBpm(pulseB));
            TestUtils.AreEqual(ThirdBpm, converterDisorder.GetBpm(pulseC));

            TestUtils.AreEqual(FirstBpm, converterSingle.GetBpm(pulseA));
            TestUtils.AreEqual(FirstBpm, converterSingle.GetBpm(pulseD));
        }

        [Test]
        public void Should_Get_Bpm_From_Sample()
        {
            TestUtils.AreEqual(FirstBpm, converterDisorder.GetBpm(sampleA));
            TestUtils.AreEqual(SecondBpm, converterDisorder.GetBpm(sampleB));
            TestUtils.AreEqual(ThirdBpm, converterDisorder.GetBpm(sampleC));

            TestUtils.AreEqual(FirstBpm, converterSingle.GetBpm(sampleA));
            TestUtils.AreEqual(FirstBpm, converterSingle.GetBpm(sampleD));
        }

        [Test]
        public void Should_Get_Bpm_From_Second()
        {
            TestUtils.AreEqual(FirstBpm, converterDisorder.GetBpm(secondA));
            TestUtils.AreEqual(SecondBpm, converterDisorder.GetBpm(secondB));
            TestUtils.AreEqual(ThirdBpm, converterDisorder.GetBpm(secondC));

            TestUtils.AreEqual(FirstBpm, converterSingle.GetBpm(secondA));
            TestUtils.AreEqual(FirstBpm, converterSingle.GetBpm(secondD));
        }

        [Test]
        public void Should_Convert_Pulse_To_Sample()
        {
            TestUtils.AreEqual(sampleA, converterDisorder.ToSample(pulseA));
            TestUtils.AreEqual(sampleB, converterDisorder.ToSample(pulseB));
            TestUtils.AreEqual(sampleC, converterDisorder.ToSample(pulseC));

            TestUtils.AreEqual(sampleA, converterSingle.ToSample(pulseA));
            TestUtils.AreEqual(sampleD, converterSingle.ToSample(pulseD));
        }

        [Test]
        public void Should_Convert_Sample_To_Pulse()
        {
            TestUtils.AreEqual(pulseA, converterDisorder.ToPulse(sampleA));
            TestUtils.AreEqual(pulseB, converterDisorder.ToPulse(sampleB));
            TestUtils.AreEqual(pulseC, converterDisorder.ToPulse(sampleC));

            TestUtils.AreEqual(pulseA, converterSingle.ToPulse(sampleA));
            TestUtils.AreEqual(pulseD, converterSingle.ToPulse(sampleD));
        }

        [Test]
        public void Should_Convert_Sample_To_Second()
        {
            TestUtils.AreEqual(secondA, converterDisorder.ToSecond(sampleA));
            TestUtils.AreEqual(secondB, converterDisorder.ToSecond(sampleB));
            TestUtils.AreEqual(secondC, converterDisorder.ToSecond(sampleC));

            TestUtils.AreEqual(secondA, converterSingle.ToSecond(sampleA));
            TestUtils.AreEqual(secondD, converterSingle.ToSecond(sampleD));
        }

        [Test]
        public void Should_Convert_Second_To_Sample()
        {
            TestUtils.AreEqual(sampleA, converterDisorder.ToSample(secondA));
            TestUtils.AreEqual(sampleB, converterDisorder.ToSample(secondB));
            TestUtils.AreEqual(sampleC, converterDisorder.ToSample(secondC));

            TestUtils.AreEqual(sampleA, converterSingle.ToSample(secondA));
            TestUtils.AreEqual(sampleD, converterSingle.ToSample(secondD));
        }

        [Test]
        public void Should_Convert_Second_To_Pulse()
        {
            TestUtils.AreEqual(pulseA, converterDisorder.ToPulse(secondA));
            TestUtils.AreEqual(pulseB, converterDisorder.ToPulse(secondB));
            TestUtils.AreEqual(pulseC, converterDisorder.ToPulse(secondC));

            TestUtils.AreEqual(pulseA, converterSingle.ToPulse(secondA));
            TestUtils.AreEqual(pulseD, converterSingle.ToPulse(secondD));
        }

        [Test]
        public void Should_Convert_Pulse_To_Second()
        {
            TestUtils.AreEqual(secondA, converterDisorder.ToSecond(pulseA));
            TestUtils.AreEqual(secondB, converterDisorder.ToSecond(pulseB));
            TestUtils.AreEqual(secondC, converterDisorder.ToSecond(pulseC));

            TestUtils.AreEqual(secondA, converterSingle.ToSecond(pulseA));
            TestUtils.AreEqual(secondD, converterSingle.ToSecond(pulseD));
        }
    }
}
