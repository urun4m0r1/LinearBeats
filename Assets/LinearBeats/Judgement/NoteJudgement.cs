using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using LinearBeats.Keyboard;
using LinearBeats.Script;
using LinearBeats.Time;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Judgement
{
    public sealed class NoteJudgement
    {
        [ShowInInspector, ReadOnly] [NotNull] private readonly InputReceiver _pressedReceiver;
        [ShowInInspector, ReadOnly] [NotNull] private readonly JudgeRange _judgeRange;
        [ShowInInspector, ReadOnly] [NotNull] private readonly JudgeEffectSpawner _judgeEffectSpawner;

        public NoteJudgement(
            [NotNull] InputReceiver pressedReceiver,
            [NotNull] JudgeRange judgeRange,
            [NotNull] JudgeEffectSpawner judgeEffectSpawner)
        {
            _pressedReceiver = pressedReceiver;
            _judgeRange = judgeRange;
            _judgeEffectSpawner = judgeEffectSpawner;
        }

        //TODO: 롱노트 판정추가
        public Judge JudgeNote(Note note, FixedTime elapsedTime, Transform effectAnchor)
        {
            var noteProgress = elapsedTime / note.Trigger.Duration;
            var pressedKey = _pressedReceiver.GetFirstKeyInvokedInNote(note.Shape, noteProgress);
            var isKeyPressed = pressedKey != KeyType.None;
            var judge = GetJudge(elapsedTime, isKeyPressed);

            if (judge != Judge.Null) _judgeEffectSpawner.Spawn(judge, new Vector3(effectAnchor.position.x, 0f, 0f));

            return judge;
        }

        private Judge GetJudge(Second elapsedTime, bool isKeyPressed)
        {
            if (elapsedTime > _judgeRange.GetRange(Judge.Bad)) return Judge.Miss;

            return isKeyPressed ? JudgeOffset(Mathf.Abs(elapsedTime)) : Judge.Null;
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        private Judge JudgeOffset(float offsetTime)
        {
            if (offsetTime <= _judgeRange.GetRange(Judge.Perfect)) return Judge.Perfect;
            if (offsetTime <= _judgeRange.GetRange(Judge.Great)) return Judge.Great;
            if (offsetTime <= _judgeRange.GetRange(Judge.Good)) return Judge.Good;
            if (offsetTime <= _judgeRange.GetRange(Judge.Bad)) return Judge.Bad;

            return Judge.Null;
        }
    }
}
