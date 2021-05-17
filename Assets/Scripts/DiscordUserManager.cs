using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordVROverlay
{
    public class DiscordUserManager : MonoBehaviour
    {
        public static DiscordUserManager instance;
        
        public Dictionary<string, DiscordUser> users = new Dictionary<string, DiscordUser>();
        public Dictionary<string, DiscordUser> oldUsers = new Dictionary<string, DiscordUser>();

        public GameObject userPrefab;

        [SerializeField]
        private Transform oldUserContainer;

        [HideInInspector]
        public Vector3 usernameAlignment;
        [HideInInspector]
        public Vector2 usernameAnchor;

        [SerializeField]
        private float spacing = 2f;
        // [SerializeField]
        // private bool showOnlySpeaking = false;

        [SerializeField]
        private UnityEngine.UI.Slider scaleSlider;

        public float scale = 1f;
        public bool showNames = true;

        private Queue<NewUser> newUserQueue = new Queue<NewUser>();


        public HashSet<string> disconnectedUsers = new HashSet<string>();

        class NewUser
        {
            public string userId;
            public string userName;
            public string profileUrl;
            public bool isTalking;
            public NewUser(string _userId, string _userName, string _profileURL, bool _isTalking)
            {
                userId = _userId;
                userName = _userName;
                profileUrl = _profileURL;
                isTalking = _isTalking;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        void Update()
        {
            DeleteOldUsers();

            for (int i=0; i<newUserQueue.Count; i++)
            {
                NewUser newUser = newUserQueue.Dequeue();
                if (oldUsers.ContainsKey(newUser.userId))
                {
                    DiscordUser user = oldUsers[newUser.userId];
                    users.Add(user.userId, user);
                    user.transform.parent = transform;
                    oldUsers.Remove(user.userId);
                }
                else
                {
                    DiscordUser user = Instantiate(userPrefab, transform).GetComponent<DiscordUser>();
                    // user.transform.localPosition = new Vector3((transform.childCount-1)*spacing, 0f, 0f);
                    user.InitUser(newUser.userId, newUser.userName, newUser.profileUrl);
                    users[newUser.userId] = user;
                }
            }
        }

        public void InspectUser(string userId, string userName, string profileUrl, bool isTalking = false)
        {
            if (!users.ContainsKey(userId))
            {
                newUserQueue.Enqueue(new NewUser(userId, userName, profileUrl, isTalking));
            }
            else
            {
                DiscordUser user = users[userId];
                user.isTalking = isTalking;
                user.userName = userName;
                user.isUpdated = false;
            }
        }

        void DeleteOldUsers(bool fullDelete = false)
        {
            foreach (string i in disconnectedUsers)
            {
                if (users.ContainsKey(i))
                {
                    DiscordUser user = users[i];
                    if (!fullDelete)
                    {
                        oldUsers.Add(i, user);
                        user.transform.parent = oldUserContainer;
                    }
                    users.Remove(i);
                    if (fullDelete)
                    {
                        Destroy(user.gameObject);
                    }
                }
            }
        }

        public void Flush()
        {
            foreach (KeyValuePair<string, DiscordUser> user in users)
            {
                disconnectedUsers.Add(user.Key);
            }
            DeleteOldUsers(true);
            users = new Dictionary<string, DiscordUser>();
            oldUsers = new Dictionary<string, DiscordUser>();
            newUserQueue = new Queue<NewUser>();
            disconnectedUsers = new HashSet<string>();
        }

        public void ShowNames()
        {
            showNames = !showNames;
        }

        public void ChangeScale()
        {
            scale = scaleSlider.value;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
