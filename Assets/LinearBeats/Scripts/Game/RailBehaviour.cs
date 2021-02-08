using UnityEngine;

namespace LinearBeats.Game
{
    public class RailBehaviour : MonoBehaviour
    {
        public ulong Pulse = 0;

        public void SetZPosition(float zPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        }
    }
}
