using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiscordVROverlay
{
    public class FavoriteChannel : MonoBehaviour
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private ToolTip tip;

        private Server server;
        private Channel channel;

        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Join);
        }

        public void SetChannel(Server _server, Channel _channel)
        {
            server = _server;
            channel = _channel;

            text.text = server.serverNameClean[0] +"|"+ channel.channelNameClean[0];
            tip.tip = server.serverNameClean +" | "+ channel.channelNameClean;
        }

        private void Join()
        {
            ServerManager.Instance.Select(server, channel);
        }
    }
}
