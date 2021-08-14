using JetBrains.Annotations;
using LinearBeats.Judgement;
using LinearBeats.Media;
using LinearBeats.Script;
using LinearBeats.Time;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Rail
{
    public sealed class NoteBehaviour : RailBehaviour
    {
        [SerializeField] [CanBeNull] private Transform effectAnchor;
        [ShowInInspector, ReadOnly] [CanBeNull] public IMediaPlayer MediaPlayer { get; set; }
        [ShowInInspector, ReadOnly] [CanBeNull] public NoteJudgement Judgement { get; set; }
        [ShowInInspector, ReadOnly] public FixedTime AudioLength { get; set; }
        [ShowInInspector, ReadOnly] public Note Note { get; set; }
        [ShowInInspector, ReadOnly] protected override Vector3 Position => new Vector3(Pos, 0f, RailObject?.StartPosition ?? 0f);
        [ShowInInspector, ReadOnly] protected override Vector3 Scale => new Vector3(Note.Shape.Size ?? 1f, 1f, Length);
        [ShowInInspector, ReadOnly] private float Pos => (float) Note.Shape.Pos - 6f;

        [ShowInInspector, ReadOnly] private float Length
        {
            get
            {
                var positionLength = RailObject?.EndPosition - RailObject?.StartPosition ?? 1f;
                return positionLength <= 1f ? 1f : positionLength * 10f;
            }
        }

        [ShowInInspector, ReadOnly] protected override bool RailDisabled
        {
            get
            {
                if (RailObject == null || Judgement == null || MediaPlayer == null) return false;

                //FIXME: 해당 레인 앞에 노트 있을시 판정 안되게 하기
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
