using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace DiscordVROverlay
{
    public class Channel : MonoBehaviour
    {
        private string channelName_;
        public string channelName
        {
            get
            {
                return channelName_;
            }
            set
            {
                channelName_ = value;
                text.text = channelName_;
            }
        }
        public string channelNameClean
        {
            get
            {
                return Regex.Replace(channelName, @"\p{Cs}", "");
            }
        }

        [SerializeField]
        private Text text;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(delegate{
                ServerManager.Instance.Select(this);
            });
        }

        public void Initialize(string name, ChannelList channelList)
        {
            channelName = name;
            channelList.AddChannel(this);
        }
    }
}
