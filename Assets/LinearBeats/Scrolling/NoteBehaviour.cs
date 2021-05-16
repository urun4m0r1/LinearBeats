using JetBrains.Annotations;
using LinearBeats.Judgement;
using LinearBeats.Script;
using UnityEngine;

namespace LinearBeats.Scrolling
{
    public sealed class NoteBehaviour : RailBehaviour
    {
        [CanBeNull] public NoteJudgement Judgement { get; set; }
        public Shape NoteShape { get; set; }
        protected override Vector3 Position => new Vector3(PosX, PosY, RailObject?.StartPosition ?? 0f);
        protected override Vector3 Scale => new Vector3(Width, Height, Length);
        private float PosX => NoteShape.PosCol - 6f;
        private float PosY => NoteShape.PosRow * 2f;
        private float Width => NoteShape.SizeCol;
        private float Height => NoteShape.SizeRow == 1 ? 1f : 20f;

        private float Length
        {
            get
            {
                var positionLength = RailObject?.EndPosition - RailObject?.StartPosition ?? 1f;
                return positionLength <= 1f ? 1f : positionLength * 10f;
            }
        }

        protected override bool RailDisabled
        {
            get
            {
                if (RailObject == null || Judgement == null) return false;

                return Judgement.JudgeNote(RailObject, NoteShape, new Vector3(PosX, PosY, 0f));
            }
        }
    }
}
