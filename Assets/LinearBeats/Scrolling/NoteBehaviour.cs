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
        [SerializeField] [CanBeNull] private Transform effectAnchor;
        [CanBeNull] public IMediaPlayer MediaPlayer { get; set; }
        [CanBeNull] public NoteJudgement Judgement { get; set; }
        public FixedTime AudioLength { get; set; }
        public Note Note { get; set; }
        protected override Vector3 Position => new Vector3(Pos, 0f, RailObject?.StartPosition ?? 0f);
        protected override Vector3 Scale => new Vector3(Note.Shape.Size ?? 1f, 1f, Length);
        private float Pos => (float) Note.Shape.Pos - 6f;

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

                //TODO: 해당 레인 앞에 노트 있을시 판정 안되게 하기
                var elapsedTime = RailObject.CurrentTime - RailObject.StartTime;
                var judge = Judgement.JudgeNote(Note, elapsedTime, effectAnchor);

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
