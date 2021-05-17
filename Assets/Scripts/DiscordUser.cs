using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

namespace DiscordVROverlay
{
    public class DiscordUser : MonoBehaviour
    {
        public string userId;
        public string userName = "";
        public bool isTalking = false;
        public Texture2D profilePicture;
        public bool isUpdated = true;

        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        // private TextMesh text;
        private UnityEngine.UI.Text text;
        [SerializeField]
        private RectTransform usernameContainer;
        [SerializeField]
        private GameObject isTalkingPlane;

        public void InitUser(string _userId, string _userName, string profileUrl)
        {
            userId = _userId;
            userName = _userName;
            StartCoroutine(GetProfilePicture(profileUrl));
        }

        void Update()
        {
            if (!isUpdated)
            {
                OnUserUpdate();
                isUpdated = true;
            }
        }

        public void OnUserUpdate()
        {
            isTalkingPlane.SetActive(isTalking);
            usernameContainer.gameObject.SetActive(DiscordUserManager.instance.showNames);
            usernameContainer.anchorMax = DiscordUserManager.instance.usernameAnchor;
            usernameContainer.anchorMin = DiscordUserManager.instance.usernameAnchor;
            usernameContainer.pivot = DiscordUserManager.instance.usernameAnchor;
            usernameContainer.anchoredPosition = DiscordUserManager.instance.usernameAlignment;
            if (text.text != userName)
            {
                // string paragraphText = Regex.Replace(userName, ".{8}", "$0\n");
                // if (userName.Length % 8 == 0)
                // {
                //     paragraphText = paragraphText.Remove(paragraphText.Length - 1, 1);
                // }
                text.text = userName;
            }
        }

        IEnumerator GetProfilePicture(string profileUrl)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(profileUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    profilePicture = DownloadHandlerTexture.GetContent(uwr);
                    spriteRenderer.sprite = Sprite.Create(
                        profilePicture,
                        new Rect(
                            0f,
                            0f,
                            profilePicture.width,
                            profilePicture.height
                        ),
                        new Vector2(
                            0.5f,
                            0.5f
                        ),
                        100.0f
                    );
                }
            }
        }

        void OnDestroy()
        {
            // DiscordUserManager.instance.users.Remove(userId);
        }
    }
}
