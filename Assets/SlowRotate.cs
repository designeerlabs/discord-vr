using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordVROverlay
{
    public class SlowRotate : MonoBehaviour
    {
        public Transform target;
        public float timeCount;


        void Update()
        {
            if (target)
            {
                transform.position = target.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, timeCount);
            }
        }
    }
}
