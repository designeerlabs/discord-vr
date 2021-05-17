using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Valve.VR;
using System.IO;

public class Installer : MonoBehaviour
{
    public static Installer instance;

    public Unity_SteamVR_Handler svrh;

    [SerializeField]
    private string manifestFile = "manifest.vrmanifest";
    [SerializeField]
    private string appKey = "3472673";

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
        Debug.Log("Started at "+ Time.time);
    }

    public void Startup()
    {
        CVRApplications app = svrh.ovrHandler.Applications;
        string manifestPath = Application.dataPath +"/../"+ manifestFile;
        app.RemoveApplicationManifest(manifestPath);
        app.RemoveApplicationManifest(Application.persistentDataPath+"/"+manifestFile);

        bool manifestExists = File.Exists(manifestPath);
        if (!manifestExists)
        {
            JSONNode json = new JSONObject();
            json["source"] = "builtin";
            JSONNode application = new JSONObject();
            application["app_key"] = "steam.overlay."+ appKey;
            application["launch_type"] = "binary";
            application["binary_path_windows"] = "DiscordVROverlay.exe";
            application["is_dashboard_overlay"] = false;
            application["strings"]["en_us"]["name"] = "Discord VR Overlay";
            application["strings"]["en_us"]["description"] = "Overlays Discord voice chat into SteamVR";
            json["applications"][0] = application;

            StreamWriter file = File.CreateText(manifestPath);
            file.Write(json.ToString(4));
            file.Close();
        }

        bool installed = app.IsApplicationInstalled("steam.overlay."+ appKey);
        Debug.Log("App installed state is "+ installed +".");
        if (!installed)
        {
            EVRApplicationError evrae;
            Debug.Log("Adding manifest.");
            evrae = app.AddApplicationManifest(manifestPath, false);
            Debug.Log("Manifest file added with "+ evrae);
            evrae = app.SetApplicationAutoLaunch("steam.overlay."+ appKey, true);
            Debug.Log("Aplication auto launched with "+ evrae);
        }
    }
}

/*
{
	"source" : "builtin",
	"applications": [{
		"app_key": "steam.overlay.3472673",
		"launch_type": "binary",
		"binary_path_windows": "AdvancedSettings.exe",
		"is_dashboard_overlay": true,

		"strings": {
			"en_us": {
				"name": "Discord VR Overlay",
				"description": "Overlays Discord voice chat into SteamVR"
			}
		}
	}]
}
*/