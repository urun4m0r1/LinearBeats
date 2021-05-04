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

        private float PosX => NoteShape.PosCol - 6f;
        private float PosY => NoteShape.PosRow * 2f;
        private float Width => NoteShape.SizeCol;
        private float Height => NoteShape.SizeRow == 1 ? 1f : 20f;

        //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)
        protected override Vector3 GetPosition(RailObject railObject) =>
            new Vector3(PosX, PosY, railObject.StartPosition);

        protected override Vector3 GetScale(RailObject railObject) =>
            new Vector3(Width, Height, 1f);

        protected override void UpdateLifecycle()
        {
            base.UpdateLifecycle();

            if (RailObject == null || Judgement == null) return;

            var noteJudged = Judgement.JudgeNote(RailObject, NoteShape, new Vector3(PosX, PosY, 0f));

            if (noteJudged) LeanPool.Despawn(this);
        }
    }
}
