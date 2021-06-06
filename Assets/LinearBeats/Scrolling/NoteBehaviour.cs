using JetBrains.Annotations;
using LinearBeats.Media;
using LinearBeats.Judgement;
using LinearBeats.Script;
using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Scrolling
{
    public sealed class NoteBehaviour : RailBehaviour
    {
        [CanBeNull] public IMediaPlayer MediaPlayer { get; set; }
        [CanBeNull] public NoteJudgement Judgement { get; set; }
        public FixedTime AudioLength { get; set; }
        public Shape NoteShape { get; set; }
        protected override Vector3 Position => new Vector3(PosX, PosY, RailObject?.StartPosition ?? 0f);
        protected override Vector3 Scale => new Vector3(Width, Height, Length);
        private float PosX => NoteShape.PosCol - 6f;
        private float PosY => NoteShape.PosRow * 2f;
        private float Width => NoteShape.SizeCol ?? 1;
        private float Height => (NoteShape.SizeRow ?? 1) == 1 ? 1f : 20f;

        private float Length
        {
            get
            {
                var positionLength = RailObject?.EndPosition - RailObject?.StartPosition ?? 1f;
                return positionLength <= 1f ? 1f : positionLength * 10f;
            }
        }

        protected override bool UpdateRailDisabled
        {
            get
            {
                if (RailObject == null || Judgement == null || MediaPlayer == null) return false;

                var (judge, elapsed) = Judgement.JudgeNote(RailObject, NoteShape, new Vector3(PosX, PosY, 0f));

                if (judge == Judge.Miss)
                    MediaPlayer.Pause();
                else if (judge != Judge.Null)
                    MediaPlayer.Play(RailObject.StartTime, AudioLength);
                else return false;

                return true;
            }
        }
    }
}
