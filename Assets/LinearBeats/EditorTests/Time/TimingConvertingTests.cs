#pragma warning disable IDE0090

using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;

namespace LinearBeats.EditorTests.Time
{
    public abstract class TimingConvertingTests
    {
        protected const float SamplesPerSecond = 500f;
        protected const int Ppqn = 100;
        protected const int FirstPulse = 0;
        protected const int SecondPulse = 400;
        protected const int ThirdPulse = 800;
        protected const float StandardBpm = 60f;
        protected const float FirstBpm = 60f;
        protected const float SecondBpm = 120f;
        protected const float ThirdBpm = 30f;
        protected readonly Pulse PulseA = 0;
        protected readonly Pulse PulseB = 600;
        protected readonly Pulse PulseC = 1200;
        protected readonly Pulse PulseD = 400;
        protected readonly Sample SampleA = 0;
        protected readonly Sample SampleB = 2500;
        protected readonly Sample SampleC = 7000;
        protected readonly Sample SampleD = 2000;
        protected readonly Second SecondA = 0;
        protected readonly Second SecondB = 5;
        protected readonly Second SecondC = 14;
        protected readonly Second SecondD = 4;

        protected BpmEvent[] DisorderedBpmEvents;
        protected BpmEvent[] SingleBpmEvents;
        protected TimingConverter ConverterDisorder;
        protected TimingConverter ConverterSingle;
        protected FixedTimeFactory FixedTimeFactory;

        [SetUp]
        public void SetUp()
        {
            DisorderedBpmEvents = new[]
            {
                new BpmEvent { Ppqn = Ppqn, Pulse = ThirdPulse, Bpm = ThirdBpm },
                new BpmEvent { Ppqn = Ppqn, Pulse = FirstPulse, Bpm = FirstBpm },
                new BpmEvent { Ppqn = Ppqn, Pulse = SecondPulse, Bpm = SecondBpm },
            };

            SingleBpmEvents = new[]
            {
                new BpmEvent { Ppqn = Ppqn, Pulse = FirstPulse, Bpm = FirstBpm },
            };

            ConverterDisorder = new TimingConverter(DisorderedBpmEvents, StandardBpm, SamplesPerSecond);
            ConverterSingle = new TimingConverter(SingleBpmEvents, StandardBpm, SamplesPerSecond);

            FixedTimeFactory = new FixedTimeFactory(ConverterDisorder);
        }

        [TearDown]
        public void TearDown()
        {
            ConverterDisorder = null;
            ConverterSingle = null;

            FixedTimeFactory = null;
        }
    }
}
