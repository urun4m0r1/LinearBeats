using JetBrains.Annotations;
using Lean.Pool;
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
        protected override Vector3 Scale => new Vector3(Width, Height, 1f);
        private float PosX => NoteShape.PosCol - 6f;
        private float PosY => NoteShape.PosRow * 2f;
        private float Width => NoteShape.SizeCol;
        private float Height => NoteShape.SizeRow == 1 ? 1f : 20f;

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
