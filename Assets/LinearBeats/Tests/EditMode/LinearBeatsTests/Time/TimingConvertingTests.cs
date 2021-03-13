#pragma warning disable IDE0090

using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;

namespace LinearBeatsTests.Time
{
    public abstract class TimingConvertingTests
    {
        protected const float SamplesPerSecond = 500f;
        protected const int PulsesPerQuarterNote = 100;
        protected const int FirstPulse = 0;
        protected const int ThirdPulse = 800;
        protected const int SecondPulse = 400;
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

        protected Timing timingDisorder;
        protected Timing timingSingle;
        protected TimingConverter converterDisorder = null;
        protected TimingConverter converterSingle = null;

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

            FixedTime.Converter = converterDisorder;
        }

        [TearDown]
        public void TearDown()
        {
            converterDisorder = null;
            converterSingle = null;

            FixedTime.Converter = null;
        }
    }
}