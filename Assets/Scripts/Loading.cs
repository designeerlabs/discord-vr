using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace DiscordVROverlay
{
    public class Loading : MonoBehaviour
    {
        public static Loading instance;

        private string _currentLoading;
        private string currentLoading
        {
            get
            {
                return _currentLoading;
            }
            set
            {
                _currentLoading = value;
                text.text = _currentLoading;
            }
        }
        private List<string> loadingScreens = new List<string>();

        private GameObject loadingChild;

        public Text text;

        private void Awake()
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

        private void Start()
        {
            loadingChild = transform.GetChild(0).gameObject;
        }

        public void AddLoading(string s)
        {
            loadingScreens.Add(s);
            UpdateLoading();
        }

        public void RemoveLoading(string s)
        {
            loadingScreens.Remove(s);
            UpdateLoading();
        }

        private void UpdateLoading()
        {
            if (loadingScreens.Count > 0)
            {
                Enable(true);
                currentLoading = loadingScreens[loadingScreens.Count-1];
            }
            else
            {
                Enable(false);
            }
        }

        private void Enable(bool b)
        {
            try
            {
                loadingChild.SetActive(b);
            }
            catch (Exception e)
            {
                print("ERROR "+ e.Message);
            }
        }
    }
}
