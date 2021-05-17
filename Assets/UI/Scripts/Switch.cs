using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DiscordVROverlay
{
    public class SwitchEvent : UnityEvent<int> { }

    public class Switch : MonoBehaviour
    {
        [SerializeField]
        private Transform slider;
        [SerializeField]
        private int slideFrames;
        [SerializeField]
        private Transform options;

        private Vector3 startPosition;
        private Vector3 targetPosition;
        private int elapsedFrames = 0;

        private IEnumerator moveCoroutine;

        public SwitchEvent onSwitch = new SwitchEvent();
        
        public int currentIndex = 0;

        private void Awake()
        {
            foreach (Transform child in options)
            {
                Button button = child.GetComponent<Button>();
                if (button)
                {
                    button.onClick.AddListener(delegate
                        {
                            Select(button);
                        }
                    );
                }
            }
            SelectForce(currentIndex);
        }

        public void SelectForce(int i)
        {
            Select(options.GetChild(i).GetComponent<Button>());
        }

        public void Select(Button button)
        {

            if (!slider) return;
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = Move(slider.position, button.transform.position);
            StartCoroutine(moveCoroutine);
            currentIndex = button.transform.GetSiblingIndex();
            onSwitch.Invoke(currentIndex);
        }

        private IEnumerator Move(Vector3 oldPos, Vector3 newPos)
        {
            elapsedFrames = 0;
            while (slider.position != newPos)
            {
                elapsedFrames++;
                slider.position = Vector3.Lerp(oldPos, newPos, Ease((float)elapsedFrames/(float)slideFrames));
                yield return null;
            }
        }

        private float Ease(float x)
        {
            return (x == 1) ? 1 : 1 - Mathf.Pow(2f, -10f * x);
        }
    }
}
