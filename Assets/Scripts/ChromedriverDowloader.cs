using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using Ionic.Zip;

namespace DiscordVROverlay
{
    public static class ChromedriverDowloader
    {
        const string baseQueryUrl = "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_";
        const string baseDownloadUrl = "https://chromedriver.storage.googleapis.com/";
        const string baseDownloadUrlSuffix = "/chromedriver_win32.zip";

        public static void Start(string chromeVersion, string driverPath, string chromedriverVersion = null)
        {
            Debug.Log(chromeVersion);
            Debug.Log(chromedriverVersion);

            string chromeVersionShort = "";
            string[] versionChunks = chromeVersion.Split('.');
            for (int i=0; i<versionChunks.Length-1; i++)
            {
                if (i != 0)
                {
                    chromeVersionShort += ".";
                }
                chromeVersionShort += versionChunks[i];
            }
            Debug.Log(baseQueryUrl+chromeVersionShort);

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(baseQueryUrl+chromeVersionShort);

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);
            string newestChromedriverVersion = objReader.ReadLine();
            Debug.Log(newestChromedriverVersion);
            string chromedriverDownloadUrl = baseDownloadUrl + newestChromedriverVersion + baseDownloadUrlSuffix;
            Debug.Log(chromedriverDownloadUrl);
            string zipPath = driverPath +@"\chromedriver.zip";

            WebClient mywebClient = new WebClient();
            mywebClient.DownloadFile(chromedriverDownloadUrl, zipPath);

            string chromedriverExePath = driverPath + @"\chromedriver.exe";
            string oldDriverPath = driverPath +@"\OldDrivers";
            if (File.Exists(chromedriverExePath))
            {
                if (!Directory.Exists(oldDriverPath))
                {
                    Directory.CreateDirectory(oldDriverPath);
                }
                File.Move(chromedriverExePath, oldDriverPath +@"\chromedriverOLD"+ Directory.GetFiles(oldDriverPath).Length +".exe");
            }

            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                zip.ExtractAll(driverPath);
            }
        }
    }
}
