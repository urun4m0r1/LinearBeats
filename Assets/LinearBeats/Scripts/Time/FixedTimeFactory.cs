#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public class FixedTimeFactory
    {
        public TimingConverter Converter { get; }

        public FixedTimeFactory(TimingConverter converter)
        {
            Converter = converter ?? throw new ArgumentNullException();
        }

        public FixedTime Create(Pulse value) => new FixedTime(Converter, value);
        public FixedTime Create(Sample value) => new FixedTime(Converter, value);
        public FixedTime Create(Second value) => new FixedTime(Converter, value);
    }
}
