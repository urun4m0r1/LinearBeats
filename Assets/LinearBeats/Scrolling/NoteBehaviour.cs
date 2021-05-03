using System;
using LinearBeats.Script;
using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Scrolling
{
    public class NoteBehaviour : RailBehaviour
    {
        public Note Note { get; set; }
        public Transform JudgeEffectAnchor;


        //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)
        protected override Vector3 GetPosition(RailObject railObject, FixedTime currentTime)
        {
            var startPosition = railObject.GetStartPosition(currentTime);
            return new Vector3(GetNotePosX(), GetNotePosY(), startPosition);
        }

        protected override Vector3 GetScale(RailObject railObject, FixedTime currentTime)
        {
            var startPosition = railObject.GetStartPosition(currentTime);
            var endPosition = railObject.GetEndPosition(currentTime);

            var distance = endPosition - startPosition;
            var isLengthShort = Math.Abs(distance) < 1f;
            var length = isLengthShort ? 1f : railObject.ScaleLength(distance);
            return new Vector3(GetNoteWidth(), GetNoteHeight(), length);
        }

        private float GetNotePosX() => Note.Shape.PosCol - 6f;
        private float GetNotePosY() => Note.Shape.PosRow * 2f;
        private float GetNoteWidth() => Note.Shape.SizeCol;
        private float GetNoteHeight() => Note.Shape.SizeRow == 1 ? 1 : 20;
    }
}
