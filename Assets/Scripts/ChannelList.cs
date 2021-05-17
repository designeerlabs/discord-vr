using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordVROverlay
{
    public class ChannelList : MonoBehaviour
    {
        public List<Channel> channels = new List<Channel>();

        /// <summary>
        /// add a channel to the list
        /// </summary>
        public void AddChannel(Channel channel)
        {
            channels.Add(channel);
            channel.transform.SetParent(transform, false);
        }
    }
}
