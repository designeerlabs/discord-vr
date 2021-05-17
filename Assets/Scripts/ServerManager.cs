using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DiscordVROverlay
{
    public class ServerManager : MonoBehaviour
    {
        // parents of lists //
        public Transform serverListParent;
        public Transform channelListParent;

        // prefabs //
        public GameObject serverPrefab;
        public GameObject channelListPrefab;
        public GameObject channelPrefab;

        // current selection //
        [HideInInspector]
        public Server currentServer;
        [HideInInspector]
        public Channel currentChannel;

        // dictionary for server to channel lookup //
        public Dictionary<Server, ChannelList> servers = new Dictionary<Server, ChannelList>();

        // event that is fired when server list is refreshed //
        private UnityEvent onRefresh = new UnityEvent();

        // the instance of the singleton //
        public static ServerManager Instance;

        /// <summary>
        /// create the singleton
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

            onRefresh.AddListener(Populate);
        }

        /// <summary>
        /// tell the selenium interface to gather servers/channels
        /// </summary>
        public void Refresh()
        {
            SeleniumInterface.ServerList.Instance.RestartThread(onRefresh);
        }

        /// <summary>
        /// tell the selenium interface to gather servers/channels
        /// </summary>
        public void GetFromCache()
        {
            onRefresh.Invoke();
        }

        /// <summary>
        /// populate server/channel lists
        /// </summary>
        private void Populate()
        {
            ClearServers();
            foreach (SeleniumInterface.Server server in SeleniumInterface.ServerList.Instance.servers)
            {
                AddServer(server);
            }
            Loading.instance?.RemoveLoading("Refreshing server data");
        }

        /// <summary>
        /// add a server and it's channels
        /// </summary>
        private void AddServer(SeleniumInterface.Server _server)
        {
            Server server = Instantiate(
                serverPrefab,
                serverListParent
            ).GetComponent<Server>();

            KeyValuePair<Server, ChannelList> newServer = server.Initialize(_server.name, _server.channels);
            servers.Add(
                newServer.Key,
                newServer.Value
            );
        }

        /// <summary>
        /// clear all servers from the gui
        /// </summary>
        public void ClearServers()
        {
            foreach (Transform server in serverListParent)
            {
                Destroy(server.gameObject);
            }
        }

        /// <summary>
        /// set the current server
        /// </summary>
        public void Select(Server server)
        {
            if (currentServer)
            {
                currentServer.channelList.gameObject.SetActive(false);
            }
            currentServer = server;
            if (currentServer)
            {
                currentServer.channelList.gameObject.SetActive(true);
            }
            RectTransform channelList = channelListParent.GetComponent<RectTransform>();
            channelList.anchoredPosition = new Vector2(channelList.anchoredPosition.x, 0f);
        }

        /// <summary>
        /// set the current channel and join it
        /// </summary>
        public void Select(Channel channel)
        {
            currentChannel = channel;
            DiscordVROverlay.SeleniumInterface.ServerList.Instance.joinChannel = true;
            FavoritesBar.instance.AddFavorite(currentServer, currentChannel);
        }

        /// <summary>
        /// set the current server and channel and join it
        /// </summary>
        public void Select(Server server, Channel channel)
        {
            currentServer = server;
            currentChannel = channel;
            DiscordVROverlay.SeleniumInterface.ServerList.Instance.joinChannel = true;
        }
    }
}
