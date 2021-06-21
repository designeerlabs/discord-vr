using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DiscordVROverlay.SeleniumInterface;
using SimpleJSON;
using System.IO;

namespace DiscordVROverlay
{
    public class UIManager : MonoBehaviour
    {
        public Unity_Overlay overlay;
        public ServerList serverList;
        public SlowRotate drag;

        public Switch attachSwitch;
        public Switch alignSwitch;

        public Slider opacitySlider;
        public Slider scaleSlider;
        public Slider dragSlider;

        public GridLayoutGroup usersContainer;

        public Text textShowSpeaking;
        public Text textShowNames;

        public Transform head;
        public Transform handL;
        public Transform handR;

        public GameObject placeOutline;
        public Button placeOverlayButton;
        private bool placingOverlay;

        public string configFileName;
        public string version;

        public Color colorInput = Color.white;
        public Color colorHighlight = Color.white;

        private bool speakingOnly = false;

        public static UIManager instance;

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

            attachSwitch.onSwitch.AddListener(ChangeAttach);
            alignSwitch.onSwitch.AddListener(ChangeAlign);

            // ChangeOpacity();
            // ChangeScale();
            // ChangeDrag();
        }

        void Start()
        {
            LoadConfig(Application.streamingAssetsPath +"/"+ configFileName);
        }

        public void ChangeAttachForce(int i)
        {
            attachSwitch.SelectForce(i);
        }

        public void ChangeAttach(int i)
        {
            switch (i)
            {
                case 0:
                    drag.target = head;
                    ChangeScaleForce(1f);
                    ChangeAlignForce(1);
                    break;
                case 1:
                    drag.target = handL;
                    ChangeScaleForce(.7f);
                    ChangeAlignForce(0);
                    break;
                case 2:
                    drag.target = handR;
                    ChangeScaleForce(1f);
                    ChangeAlignForce(2);
                    break;
                case 3:
                    drag.target = null;
                    drag.transform.localPosition = Vector3.zero;
                    drag.transform.localEulerAngles = Vector3.zero;
                    ChangeScaleForce(.7f);
                    ChangeAlignForce(1);
                    break;
            }
        }

        public void ChangeAlignForce(int i)
        {
            alignSwitch.SelectForce(i);
        }

        public void ChangeAlign(int i)
        {
            Vector3 newPosition = Vector3.zero;
            Vector3 newRotation = Vector3.zero;
            switch (i)
            {
                case 0: // left
                    // usersContainer.childAlignment = TextAnchor.UpperLeft;
                    usersContainer.startAxis = GridLayoutGroup.Axis.Vertical;
                    switch (attachSwitch.currentIndex)
                    {
                        case 0: // head
                            newPosition = new Vector3(0f, -.1f, .36f);
                            newRotation = new Vector3(.377f, .418f, -.297f);
                            ChangeScaleForce(.5f);
                            break;
                        case 1: // left
                            newPosition = new Vector3(-.045f, 0f, 0f);
                            newRotation = new Vector3(90f, 0f, 0f);
                            break;
                        case 2: // right
                            newPosition = new Vector3(-.045f, 0f, 0f);
                            newRotation = new Vector3(90f, 0f, 0f);
                            break;
                        case 3: // world
                            newPosition = new Vector3(0f, 0f, .0f);
                            newRotation = new Vector3( 90f, 0f, 0f);
                            break;
                    }
                    DiscordUserManager.instance.usernameAnchor = new Vector2(.5f, .5f);
                    DiscordUserManager.instance.usernameAlignment = new Vector3(-95f, 0f, 0f);
                    break;
                case 1: // center
                    // usersContainer.childAlignment = TextAnchor.MiddleCenter;
                    usersContainer.startAxis = GridLayoutGroup.Axis.Horizontal;
                    switch (attachSwitch.currentIndex)
                    {
                        case 0: // head
                            newPosition = new Vector3(0f, -.2f, .4f);
                            newRotation = new Vector3(30f, 0f, 0f);
                            break;
                        case 1: // left
                            newPosition = new Vector3(0f, 0f, .06f);
                            newRotation = new Vector3(61.7f, 0f, 0f);
                            break;
                        case 2: // right
                            newPosition = new Vector3(0f, 0f, .06f);
                            newRotation = new Vector3(61.7f, 0f, 0f);
                            break;
                        case 3: // world
                            newPosition = new Vector3(0f, 0f, .0f);
                            newRotation = new Vector3( 90f, 0f, 0f);
                            break;
                    }
                    DiscordUserManager.instance.usernameAnchor = new Vector2(.5f, 1f);
                    DiscordUserManager.instance.usernameAlignment = new Vector3(0f, -95f, 0f);
                    break;
                case 2: // right
                    // usersContainer.childAlignment = TextAnchor.UpperRight;
                    usersContainer.startAxis = GridLayoutGroup.Axis.Vertical;
                    switch (attachSwitch.currentIndex)
                    {
                        case 0: // head
                            newPosition = new Vector3(0f, -.1f, .36f);
                            newRotation = new Vector3(.377f, .418f, -.297f);
                            ChangeScaleForce(.5f);
                            break;
                        case 1: // left
                            newPosition = new Vector3(.045f, 0f, 0f);
                            newRotation = new Vector3(90f, 0f, 0f);
                            break;
                        case 2: // right
                            newPosition = new Vector3(.045f, 0f, 0f);
                            newRotation = new Vector3(90f, 0f, 0f);
                            break;
                        case 3: // world
                            newPosition = new Vector3(0f, 0f, .0f);
                            newRotation = new Vector3( 90f, 0f, 0f);
                            break;
                    }
                    DiscordUserManager.instance.usernameAnchor = new Vector2(.5f, .5f);
                    DiscordUserManager.instance.usernameAlignment = new Vector3(95f, 0f, 0f);
                    break;
            }
            overlay.transform.parent.localPosition = newPosition;
            overlay.transform.parent.localEulerAngles = newRotation;
        }

        public void ChangeOpacityForce(float opacity)
        {
            opacitySlider.value = opacity;
        }

        public void ChangeOpacity()
        {
            overlay.opacity = opacitySlider.value;
        }

        public void ChangeScaleForce(float scale)
        {
            scaleSlider.value = scale / 2f;
        }

        public void ChangeScale()
        {
            overlay.widthInMeters = scaleSlider.value * 2f;
            overlay.transform.localPosition = new Vector3(0f, -scaleSlider.value, 0f);
        }

        public void ChangeDragForce(float drag)
        {
            dragSlider.value = drag;
        }

        public void ChangeDrag()
        {
            // .05 - .3
            drag.timeCount = (dragSlider.value * .295f) + .05f;
        }

        public void StartStopPlaceOverlay()
        {
            if (placingOverlay)
            {
                StopPlaceOverlay();
            }
            else
            {
                StartPlaceOverlay();
            }
        }

        public void StartPlaceOverlay()
        {
            ChangeAttachForce(3);
            drag.target = handR;
            overlay.transform.parent.localPosition = new Vector3(0f, -.05f, .2f);
            placeOutline.SetActive(true);
            placeOverlayButton.GetComponent<Image>().color = colorHighlight;
            placeOverlayButton.GetComponentInChildren<Text>().text = "Press grip to stop placing";
            placingOverlay = true;
        }

        public void StopPlaceOverlay()
        {
            if (!placingOverlay) return;
            drag.target = null;
            placeOutline.SetActive(false);
            placeOverlayButton.GetComponent<Image>().color = colorInput;
            placeOverlayButton.GetComponentInChildren<Text>().text = "Place in VR";
            placingOverlay = false;
        }

        public void ToggleSpeakingOnly()
        {
            serverList.ToggleSpeakingOnly();
            speakingOnly = !speakingOnly;
            if (speakingOnly)
            {
                textShowSpeaking.text = "Show Everyone";
            }
            else
            {
                textShowSpeaking.text = "Show Only Who Is Talking";
            }
        }

        public void ToggleNames()
        {
            DiscordUserManager.instance.showNames = !DiscordUserManager.instance.showNames;
            if (DiscordUserManager.instance.showNames)
            {
                textShowNames.text = "Hide Names";
            }
            else
            {
                textShowNames.text = "Show Names";
            }
        }

        public void LoadConfig(string configPath)
        {
            if (configPath == null || !File.Exists(configPath)) return;
            JSONNode config = JSON.Parse(File.ReadAllText(configPath));
            if (config["version"] != "1.0.1") return;
            JSONNode configSettings = config["settings"];
            print("asdfasdf");
            // ChangeAttachForce(configSettings["attach-to"]);
            ChangeAlignForce(configSettings["align-to"]);
            ChangeOpacityForce(configSettings["opacity"]);
            ChangeDragForce(configSettings["drag"]);
        }

        public void SetConfig(string configPath)
        {
            JSONNode config = new JSONObject();
            config["version"] = version;
            config["auto-launch"] = false;
            config["join-on-launch"] = false;
            config["settings"] = new JSONObject();
            JSONNode configSettings = config["settings"];
            configSettings["attach-to"] = attachSwitch.currentIndex;
            configSettings["align-to"] = alignSwitch.currentIndex;
            configSettings["opacity"] = opacitySlider.value;
            configSettings["scale"] = scaleSlider.value;
            configSettings["drag"] = dragSlider.value;

            config["favorite-channels"] = new JSONArray();

            File.WriteAllText(configPath, config.ToString(4));
        }

        void OnDestroy()
        {
            SetConfig(Application.streamingAssetsPath +"/"+ configFileName);
        }
    }
}
