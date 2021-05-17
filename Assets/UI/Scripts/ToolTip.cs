using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DiscordVROverlay
{
    public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private string _tip;
        public string tip
        {
            get
            {
                return _tip;
            }
            set
            {
                _tip = value;
                text.text = _tip;
            }
        }

        [SerializeField]
        private Text text;
        [SerializeField]
        private GameObject box;

        public void OnPointerEnter(PointerEventData eventData)
        {
            box.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            box.SetActive(false);
        }
    }
}
