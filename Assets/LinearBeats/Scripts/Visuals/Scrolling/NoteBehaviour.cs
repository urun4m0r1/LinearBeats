using LinearBeats.Judgement;
using LinearBeats.Script;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public class NoteBehaviour : RailBehaviour
    {
        public Note Note { get; set; }

        public void OnJudge(Judge judge, ulong currentPulse)
        {
            if (judge != Judge.Miss)
            {
                Debug.Log($"{judge}Row:{Note.PositionRow}, Col:{Note.PositionCol} " +
                $"/ Note: {Note.Pulse}, At: {currentPulse}");
            }
        }
    }
}
