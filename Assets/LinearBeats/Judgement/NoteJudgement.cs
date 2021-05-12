using JetBrains.Annotations;
using LinearBeats.Input;
using LinearBeats.Script;
using LinearBeats.Scrolling;
using LinearBeats.Time;
using LinearBeats.Visuals;
using UnityEngine;
using UnityEngine.Assertions;

namespace LinearBeats.Judgement
{
    public sealed class NoteJudgement
    {
        [NotNull] private readonly JudgeRange _judgeRange;
        [NotNull] private readonly LaneEffect _laneEffect;

        public NoteJudgement([NotNull] JudgeRange judgeRange, [NotNull] LaneEffect laneEffect)
        {
            _judgeRange = judgeRange;
            _laneEffect = laneEffect;
        }

        //TODO: 늦게치면 무조건 miss인 현상 해결
        //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)
        public bool JudgeNote([NotNull] RailObject railObject, Shape noteShape, Vector3 effectPosition)
        {
            var noteJudgement = GetJudge(railObject.CurrentTime, railObject.StartTime, noteShape);
            if (noteJudgement == null) return false;

            _laneEffect.OnJudge(effectPosition, (Judge) noteJudgement);
            return true;
        }

        private Judge? GetJudge(FixedTime currentTime, FixedTime targetTime, Shape noteShape)
        {
            Second elapsedTime;
            Second remainingTime;
            if (currentTime >= targetTime)
            {
                elapsedTime = currentTime - targetTime;
                remainingTime = float.MaxValue;
            }
            else
            {
                elapsedTime = 0f;
                remainingTime = targetTime - currentTime;
            }

            if (InputHandler.IsNotePressed(noteShape))
            {
                if (WithinJudge(_judgeRange.Range(Judge.Perfect))) return Judge.Perfect;
                if (WithinJudge(_judgeRange.Range(Judge.Great))) return Judge.Great;
                if (WithinJudge(_judgeRange.Range(Judge.Good))) return Judge.Good;
                if (WithinJudgeRange(_judgeRange.Range(Judge.Bad), _judgeRange.Range(Judge.Good))) return Judge.Bad;

                return null;
            }

            if (MissedJudge(_judgeRange.Range(Judge.Good))) return Judge.Miss;

            return null;

            bool WithinJudge(Second judgeOffset) => !(MissedJudge(judgeOffset) || PreJudge(judgeOffset));

            bool WithinJudgeRange(Second judgeOffsetStart, Second judgeOffsetEnd)
            {
                Assert.IsTrue(judgeOffsetStart >= judgeOffsetEnd);

                return MissedJudge(judgeOffsetStart) && PreJudge(judgeOffsetEnd);
            }

            bool PreJudge(Second judgeOffset) => remainingTime >= judgeOffset;

            bool MissedJudge(Second judgeOffset) => elapsedTime >= judgeOffset;
        }
    }
}
