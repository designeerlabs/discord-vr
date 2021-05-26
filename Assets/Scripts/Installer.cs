using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Valve.VR;
using System.IO;
using System;

namespace DiscordVROverlay
{
    public class Installer : MonoBehaviour
    {
        public static Installer instance;

        public Unity_SteamVR_Handler svrh;

        [SerializeField]
        private string manifestFile = "manifest.vrmanifest";
        [SerializeField]
        private string appKey = "designeerlabs.overlay.discord";

        private string manifestPath;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }
        }

        void Start()
        {
            manifestPath = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\")) + manifestFile;
            ErrorManager.instance.AddError("path: "+ manifestPath);
            #if !UNITY_EDITOR
            CreateManifestFile();
            #endif
        }

        private void CreateManifestFile()
        {
            bool manifestExists = File.Exists(manifestPath);
            if (!manifestExists)
            {
                JSONNode json = new JSONObject();
                json["source"] = "builtin";
                JSONNode application = new JSONObject();
                application["app_key"] = appKey;
                application["launch_type"] = "binary";
                application["binary_path_windows"] = "DiscordVR.exe";
                application["is_dashboard_overlay"] = false;
                application["strings"]["en_us"]["name"] = "Discord VR";
                application["strings"]["en_us"]["description"] = "Overlays Discord voice chat into SteamVR";
                json["applications"][0] = application;

                StreamWriter file = File.CreateText(manifestPath);
                file.Write(json.ToString(4));
                file.Close();
            }
        }

        public void RemoveAsSteamVRPlugin()
        {
            try
            {
                CVRApplications app = svrh.ovrHandler.Applications;
                app.RemoveApplicationManifest(manifestPath);
            }
            catch (Exception e)
            {
                ErrorManager.instance.AddError("ERROR: "+ e.Message);
            }
        }

        public void AddAsSteamVRPlugin()
        {
            try
            {
                ErrorManager.instance.AddError("started add");
                CVRApplications app = svrh.ovrHandler.Applications;
                bool installed = app.IsApplicationInstalled(appKey);
                ErrorManager.instance.AddError("installed: "+ installed);
                if (!installed)
                {
                    EVRApplicationError evrae;
                    evrae = app.AddApplicationManifest(manifestPath, false);
                    ErrorManager.instance.AddError("add: "+ evrae);
                    evrae = app.SetApplicationAutoLaunch(appKey, true);
                    ErrorManager.instance.AddError("autolaunch: "+ evrae);
                }
            }
            catch (Exception e)
            {
                ErrorManager.instance.AddError("ERROR: "+ e.Message);
            }
        }
    }
}

/*
{
	"source" : "builtin",
	"applications": [{
		"app_key": "designeerlabs.overlay.discord",
		"launch_type": "binary",
		"binary_path_windows": "DiscordVR.exe",
		"is_dashboard_overlay": true,

		"strings": {
			"en_us": {
				"name": "Discord VR",
				"description": "Overlays Discord voice chat into SteamVR"
			}
		}
	}]
}
*/