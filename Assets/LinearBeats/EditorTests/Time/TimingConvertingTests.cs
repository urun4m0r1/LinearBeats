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

        protected static readonly BpmEvent[] BpmEvents =
        {
            new BpmEvent {Ppqn = Ppqn, Pulse = ThirdPulse, Bpm = ThirdBpm},
            new BpmEvent {Ppqn = Ppqn, Pulse = FirstPulse, Bpm = FirstBpm},
            new BpmEvent {Ppqn = Ppqn, Pulse = SecondPulse, Bpm = SecondBpm},
        };

        protected static readonly BpmEvent[] SingleBpmEvents =
        {
            new BpmEvent {Ppqn = Ppqn, Pulse = FirstPulse, Bpm = FirstBpm},
        };

        protected static readonly TimingConverter Converter =
            new TimingConverter(BpmEvents, StandardBpm, SamplesPerSecond);

        protected static readonly TimingConverter ConverterSingle =
            new TimingConverter(SingleBpmEvents, StandardBpm, SamplesPerSecond);

        protected static readonly FixedTimeFactory Factory =
            new FixedTimeFactory(Converter);


        protected static Sample ToSample(Pulse v) => Converter.ToSample(v, Converter.GetTimingIndex(v));
        protected static Pulse ToPulse(Sample v) => Converter.ToPulse(v, Converter.GetTimingIndex(v));
        protected static Second ToSecond(Sample v) => Converter.ToSecond(v);
        protected static Sample ToSample(Second v) => Converter.ToSample(v);

        protected static Second ToSecond(Pulse v) =>
            Converter.ToSecond(Converter.ToSample(v, Converter.GetTimingIndex(v)));

        protected static Pulse ToPulse(Second v)
        {
            var sample = Converter.ToSample(v);
            return Converter.ToPulse(sample, Converter.GetTimingIndex(sample));
        }
    }
}
