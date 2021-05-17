using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace DiscordVROverlay
{
    [RequireComponent(typeof(Button))]
    public class Server : MonoBehaviour
    {
        private string serverName_;
        public string serverName
        {
            get
            {
                return serverName_;
            }
            set
            {
                serverName_ = value;
                text.text = serverName_;
            }
        }
        public string serverNameClean
        {
            get
            {
                return Regex.Replace(serverName, @"\p{Cs}", "");
            }
        }

        public ChannelList channelList;

        [SerializeField]
        private UnityEngine.UI.Text text;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(delegate{
                ServerManager.Instance.Select(this);
            });
        }

        public KeyValuePair<Server, ChannelList> Initialize(string _serverName, string[] _channels)
        {
            serverName = _serverName;

            channelList = Instantiate(
                ServerManager.Instance.channelListPrefab,
                ServerManager.Instance.channelListParent
            ).GetComponent<ChannelList>();

            foreach (string channelName in _channels)
            {
                Channel channel = Instantiate(
                    ServerManager.Instance.channelPrefab,
                    channelList.transform
                ).GetComponent<Channel>();
                channel.Initialize(channelName, channelList);
            }

            return new KeyValuePair<Server, ChannelList>(this, channelList);
        }

        private void OnDestroy()
        {
            Destroy(channelList.gameObject);
        }
    }
}
