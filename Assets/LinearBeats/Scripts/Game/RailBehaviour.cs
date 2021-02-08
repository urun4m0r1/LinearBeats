using UnityEngine;

namespace LinearBeats.Game
{
    public class RailBehaviour : MonoBehaviour
    {
        public float PositionMultiplyer = 0f;
        public ulong Pulse = 0;

        public void SetZPosition(float zPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        }
    }
}
