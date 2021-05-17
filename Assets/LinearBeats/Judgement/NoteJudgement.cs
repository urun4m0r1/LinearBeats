using JetBrains.Annotations;
using LinearBeats.Input;
using LinearBeats.Script;
using LinearBeats.Scrolling;
using LinearBeats.Time;
using LinearBeats.Visuals;
using UnityEngine;

namespace LinearBeats.Judgement
{
    public sealed class NoteJudgement
    {
        [NotNull] private readonly JudgeRange _judgeRange;
        [NotNull] private readonly LaneEffect _laneEffect;

        public NoteJudgement(
            [NotNull] JudgeRange judgeRange,
            [NotNull] LaneEffect laneEffect)
        {
            _judgeRange = judgeRange;
            _laneEffect = laneEffect;
        }

        //TODO: 롱노트, 슬라이드노트 판정추가
        public (Judge, FixedTime) JudgeNote([NotNull] RailObject railObject, Shape noteShape, Vector3 effectPosition)
        {
            var elapsedTime = railObject.CurrentTime - railObject.StartTime;
            var judge = GetJudge(elapsedTime, noteShape);

            _laneEffect.OnJudge(effectPosition, judge);

            return (judge, elapsedTime);
        }

        private Judge GetJudge(Second elapsedTime, Shape noteShape)
        {
            var offsetTime = Mathf.Abs(elapsedTime);

            if (InputHandler.IsNotePressed(noteShape))
            {
                if (offsetTime <= _judgeRange.Range(Judge.Perfect)) return Judge.Perfect;
                if (offsetTime <= _judgeRange.Range(Judge.Great)) return Judge.Great;
                if (offsetTime <= _judgeRange.Range(Judge.Good)) return Judge.Good;
                if (offsetTime <= _judgeRange.Range(Judge.Bad)) return Judge.Bad;
            }

            return elapsedTime > _judgeRange.Range(Judge.Bad) ? Judge.Miss : Judge.Null;
        }
    }
}
