﻿using LinearBeats.EditorTests.Time;
using LinearBeats.Judgement;
using LinearBeats.Scrolling;
using NUnit.Framework;

namespace LinearBeats.EditorTests.Judgement
{
    [TestFixture]
    public class NoteJudgementTests
    {
        private static readonly NoteJudgement Judgement = new NoteJudgement(null!, null!);

        [Test]
        public void Should_Judge()
        {
            NoteBehaviour a = null;
            // var judge = Judgement.JudgeNote(a, FixedTimeTests.V0);
        }
    }
}
