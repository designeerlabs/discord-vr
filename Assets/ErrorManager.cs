using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordVROverlay.SeleniumInterface;

namespace DiscordVROverlay
{
    public class ErrorManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject errorPrefab;
        [SerializeField]
        private GameObject connectionError;

        private Queue<string> errors = new Queue<string>();

        public static ErrorManager instance;

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

        public void AddError(string e)
        {
            errors.Enqueue(e);
        }

        void Update()
        {
            if (errors.Count > 0)
            {
                NewError(errors.Dequeue());
            }

            connectionError.SetActive(ServerList.Instance.connectionError);
        }

        public void NewError(string e)
        {
            Error err = Instantiate(errorPrefab, transform).GetComponent<Error>();
            err.SetError(e);
        }
    }
}
