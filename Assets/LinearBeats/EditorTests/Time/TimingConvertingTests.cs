#pragma warning disable IDE0090

using LinearBeats.Script;
using LinearBeats.Time;
using LinearBeats.Visuals;
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
        protected readonly Pulse pulseA = 0;
        protected readonly Pulse pulseB = 600;
        protected readonly Pulse pulseC = 1200;
        protected readonly Pulse pulseD = 400;
        protected readonly Sample sampleA = 0;
        protected readonly Sample sampleB = 2500;
        protected readonly Sample sampleC = 7000;
        protected readonly Sample sampleD = 2000;
        protected readonly Second secondA = 0;
        protected readonly Second secondB = 5;
        protected readonly Second secondC = 14;
        protected readonly Second secondD = 4;

        protected BpmEvent[] disorderedBpmEvents;
        protected BpmEvent[] singleBpmEvents;
        protected TimingConverter converterDisorder = null;
        protected TimingConverter converterSingle = null;
        protected FixedTimeFactory fixedTimeFactory = null;

        [SetUp]
        public void SetUp()
        {
            disorderedBpmEvents = new BpmEvent[]
            {
                new BpmEvent() { Ppqn = Ppqn, Pulse = ThirdPulse, Bpm = ThirdBpm },
                new BpmEvent() { Ppqn = Ppqn, Pulse = FirstPulse, Bpm = FirstBpm },
                new BpmEvent() { Ppqn = Ppqn, Pulse = SecondPulse, Bpm = SecondBpm },
            };

            singleBpmEvents = new BpmEvent[]
            {
                new BpmEvent() { Ppqn = Ppqn, Pulse = FirstPulse, Bpm = FirstBpm },
            };

            converterDisorder = new TimingConverter(disorderedBpmEvents, StandardBpm, SamplesPerSecond);
            converterSingle = new TimingConverter(singleBpmEvents, StandardBpm, SamplesPerSecond);

            var positionConverter = new PositionConverter.Builder(converterDisorder).Build();

            fixedTimeFactory = new FixedTimeFactory(positionConverter);
        }

        [TearDown]
        public void TearDown()
        {
            converterDisorder = null;
            converterSingle = null;

            fixedTimeFactory = null;
        }
    }
}
