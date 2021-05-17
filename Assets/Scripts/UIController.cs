using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace DiscordVROverlay
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private Unity_Overlay overlay;
        [SerializeField]
        private InputField urlInput;
        [SerializeField]
        private Slider opacitySlider;

        [SerializeField]
        private string[] servers, channels;

        [SerializeField]
        private Transform serverList, channelList;

        public void ChangeOpacity()
        {
            overlay.opacity = opacitySlider.value;
        }

        public void ChangeServers(string[] s)
        {
            servers = s;
            DestroyChildren(serverList);
            // for (int i=0; i<)
        }

        public void ChangeChannels(string[] c)
        {
            channels = c;
            DestroyChildren(channelList);
        }

        void DestroyChildren(Transform t)
        {
            foreach (Transform child in t)
            {
                Destroy(child.gameObject);
            }
        }

        public void ParentTo(Transform t)
        {

        }
    }
}
