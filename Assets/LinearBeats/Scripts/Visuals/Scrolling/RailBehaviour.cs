using UnityEngine;

namespace LinearBeats.Visuals
{
    public class RailBehaviour : MonoBehaviour
    {
        public float PositionMultiplyer { get; set; } = 0f;
        public ulong Pulse { get; set; } = 0;

        public void SetZPosition(float zPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        }
    }
}
