using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordVROverlay.SeleniumInterface
{
    public class Server
    {
        public string name;
        public string[] channels;

        public Server(string _name, string[] _channels)
        {
            name = _name;
            channels = _channels;
            ServerList.Instance.servers.Add(this);
        }
    }
}
