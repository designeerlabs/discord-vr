using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using SimpleJSON;

namespace DiscordVROverlay.SeleniumInterface
{
    public class ServerList : MonoBehaviour
    {
        // url of streamkit //
        [SerializeField]
        private string url;

        private Thread thread;
        private ChromeDriver driver;

        public List<Server> servers = new List<Server>();

        // if the thread should restart //
        private bool restartThread = false;
        // if the thread has started //
        private bool threadEnabled = false;
        // if selenium has opened //
        private bool seleniumOpen = false;
        // if there is a new channel to join //
        public bool joinChannel = false;
        // if the list has been updated //
        private bool updated = false;
        // whether it's possible to get voice data //
        private bool canGetVoice = false;
        // whether it's possible to connect to the discord client //
        public bool connectionError = false;
        // whether it was previously possible to connect to the discord client //
        public bool connectionErrorPrevious = false;

        // refresh the list of servers (set from selenium thread) //
        private bool refreshServerData = false;
        // refresh the list of servers and channels //
        private bool refreshServers = false;
        // refresh the list of users talking //
        private bool refreshVoice = false;
        // toggle show only speaking //
        private bool toggleSpeakingOnly = false;
        // import server data //
        private bool importServerData = false;

        // where chromedriver.exe is stored //
        private string driverPath;

        // the name of the server/channel cache file //
        [SerializeField]
        private string cacheFile = "server-cache.json";
        
        [SerializeField]
        private bool headless = true;

        // web page elements//
        // server elements //
        IWebElement buttonServerDropdown;
        IWebElement buttonServerDropdownInput;
        // channel elements //
        IWebElement buttonChannelDropdown;
        IWebElement buttonChannelDropdownInput;
        // voice settings elements //
        IWebElement buttonShowSpeaking;
        // iframe that contains who is talking //
        IWebElement voiceContainer;

        // the instance of the singleton //
        public static ServerList Instance;

        /// <summary>
        /// create the singleton
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        /// <summary>
        /// called on first frame
        /// </summary>
        private void Start()
        {
            // chromedriver.exe should be in the StreamingAssets folder //
            driverPath = Application.dataPath + "/StreamingAssets";

            // start selenium //
            RestartThread();

            // start listening for server/channel refresh //
            IEnumerator callbackCoroutine = EventCallback();
            StartCoroutine(callbackCoroutine);
        }

        /// <summary>
        /// stop the thread and start it again
        /// </summary>
        public void RestartThread(bool killAll = false)
        {
            restartThread = false;
            print("restarting chromedriver thread");
            StopThread(killAll);
            thread = new Thread(SeleniumThread);
            thread.Start();
        }

        /// <summary>
        /// restart the thread with an event callback
        /// </summary>
        public void RestartThread(UnityEvent callbackEvent)
        {
            RestartThread();
        }

        private IEnumerator EventCallback()
        {
            while (true)
            {
                if (threadEnabled && updated)
                {
                    updated = false;
                    ServerManager.Instance.GetFromCache();
                }
                if (threadEnabled)
                {
                    refreshVoice = true;
                }
                if (refreshServerData)
                {
                    RefreshServerData();
                }
                if (importServerData)
                {
                    ImportServerData();
                }
                if (restartThread)
                {
                    RestartThread(true);
                }
                yield return new WaitForSeconds(.3f);
            }
        }

        /// <summary>
        /// the function that is called on a seperate thread to launch chromedriver
        /// </summary>
        private void SeleniumThread()
        {
            print("started chromedriver thread");
            threadEnabled = true;
            ChromeOptions options = new ChromeOptions();
            // headless stops the selenium windows from popping up //
            if (headless)
            {
                options.AddArguments(new List<string>(){ "headless" });
            }

            // where the error will be put if there is one //
            string error = "";
            try
            {
                // create a service from the path of chromedriver.exe //
                ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverPath);
                service.HideCommandPromptWindow = headless;
                driver = new ChromeDriver(service, options);
                JSONNode capabilities = JSON.Parse(driver.Capabilities.ToString());
                // string chromeVersion = capabilities["browserVersion"].ToString().Split('"')[1];
                // string chromedriverVersion = capabilities["chrome"]["chromedriverVersion"].ToString().Split(' ')[0].Split('"')[1];
                // ChromedriverDowloader.Start(chromeVersion, driverPath, chromedriverVersion);
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            // check if the error string has anything //
            if (!string.IsNullOrEmpty(error))
            {
                // create the error popup //
                UnityEngine.Debug.Log(error);
                string[] splitError = error.Split(' ');
                if (splitError[0] + splitError[1] + splitError[2] == "sessionnotcreated:")
                {
                    string chromeVersion = splitError[15];
                    string chromedriverVersion = splitError[11];
                    driver?.Close();
                    driver?.Quit();
                    ChromedriverDowloader.Start(chromeVersion, driverPath, chromedriverVersion);
                    UnityEngine.Debug.Log("downloading correct version of chromedriver");
                    restartThread = true;
                }
                else
                {
                    ErrorManager.instance.AddError(error.ToString());
                }
                StopThread();
            }

            driver.Url = url;
            seleniumOpen = true;

            IWebElement obsButton = driver.FindElement(By.XPath("//button[text()='Install for OBS']"));
            obsButton.Click();
            Thread.Sleep(1000);
            IWebElement voiceButton = driver.FindElement(By.XPath("//button[@value='voice']"));
            voiceButton.Click();
            Thread.Sleep(2000);
            // server elements
            buttonServerDropdown = driver.FindElement(By.XPath("//div[@data-reactid[contains(.,'selected-guild')]]"));
            buttonServerDropdownInput = driver.FindElement(By.XPath("//input[@data-reactid[contains(.,'selected-guild')]]"));
            // channel elements
            buttonChannelDropdown = driver.FindElement(By.XPath("//div[@data-reactid[contains(.,'selected-voice-channel')]]"));
            buttonChannelDropdownInput = driver.FindElement(By.XPath("//input[@data-reactid[contains(.,'selected-voice-channel')]]"));
            // voice settings elements
            buttonShowSpeaking = driver.FindElement(By.XPath("//div[@class='switch-toggle']"));

            // load servers and channels from cache //
            importServerData = true;

            while (seleniumOpen)
            {
                try
                {
                    int errorBannerCount = driver.FindElements(By.XPath("//div[@class='connect-notice']")).Count;
                    bool prev = connectionError;
                    connectionError = errorBannerCount > 0;
                    if (prev && !connectionError)
                    {
                        refreshServerData = true;
                    }
                }
                catch { }

                // see if there is a connection error
                if (connectionError)
                {
                    continue;
                }
                // join a channel
                if (joinChannel)
                {
                    joinChannel = false;
                    // enter the server name
                    buttonServerDropdown.Click();
                    buttonServerDropdownInput.SendKeys(ServerManager.Instance.currentServer.serverName);
                    buttonServerDropdownInput.SendKeys(Keys.Return);
                    // enter the channel name
                    buttonChannelDropdown.Click();
                    string server = ServerManager.Instance.currentChannel.channelNameClean;
                    buttonChannelDropdownInput.SendKeys(server);
                    buttonChannelDropdownInput.SendKeys(Keys.Return);

                    canGetVoice = true;
                }
                // refresh the list of servers/channels
                if (refreshServers)
                {
                    RefreshServerDataSeleniumThread();
                }
                // toggle show only speaking
                if (toggleSpeakingOnly)
                {
                    toggleSpeakingOnly = false;
                    buttonShowSpeaking.Click();
                }
                // show who is talking
                if (canGetVoice && refreshVoice)
                {
                    RefreshVoiceData();
                }
            }
        }

        /// <summary>
        /// enable the refresh variable so that the selenium thread refreshes
        /// </summary>
        public void RefreshServerData()
        {
            refreshServerData = false;
            refreshServers = true;
            Loading.instance?.AddLoading("Refreshing server data");
        }

        /// <summary>
        /// gets a list of servers and channels from the webpage
        /// </summary>
        private void RefreshServerDataSeleniumThread()
        {
            canGetVoice = false;
            refreshServers = false;
            servers = new List<Server>();
            // click on the server list and get all servers
            buttonServerDropdown.Click();
            IWebElement serverDropdown = driver.FindElement(By.XPath("//div[@class='Select-menu-outer']"));
            string[] serversSplit = serverDropdown.Text.Split('\n');

            // go through the servers and get their channels
            foreach (string serverName in serversSplit)
            {
                // get rid of all non BMP characters (emojis and others)
                string server = Regex.Replace(serverName, @"\p{Cs}", "");
                buttonServerDropdown.Click();
                buttonServerDropdownInput.SendKeys(server);
                buttonServerDropdownInput.SendKeys(Keys.Return);
                buttonChannelDropdown.Click();
                // Thread.Sleep(10);
                IWebElement channelDropdown = driver.FindElement(By.XPath("//div[@class='Select-menu-outer']"));
                string[] channels = channelDropdown.Text.Split('\n');
                new Server(server, channels);
            }
            UnityEngine.Debug.Log("Updated server/channel list.");
            updated = true;

            SaveServerData();
        }

        /// <summary>
        /// let the driver get elements from the voice iframe
        /// </summary>
        private void FocusDriverVoice()
        {
            driver.SwitchTo().Frame(0);
            voiceContainer = driver.FindElement(By.XPath("//ul[@class='voice-states']"));
        }

        /// <summary>
        /// let the driver get elements from the main page
        /// </summary>
        private void FocusDriverMain()
        {
            driver.SwitchTo().DefaultContent();
        }

        /// <summary>
        /// saves the servers and channels to a file in JSON format
        /// </summary>
        private void SaveServerData()
        {
            JSONNode json = new JSONObject();

            foreach (Server server in servers)
            {
                JSONNode serverJson = new JSONObject();
                serverJson["name"] = server.name;
                foreach (string channel in server.channels)
                {
                    serverJson["channels"][-1] = channel;
                }
                json["servers"][-1] = serverJson;
            }

            StreamWriter file = File.CreateText(Application.streamingAssetsPath +"/"+ cacheFile);
            file.Write(json.ToString(4));
            file.Close();
        }

        /// <summary>
        /// load servers and channels from a JSON file
        /// </summary>
        private void ImportServerData()
        {
            importServerData = false;
            if (File.Exists(Application.streamingAssetsPath +"/"+ cacheFile))
            {
                StreamReader file = File.OpenText(Application.streamingAssetsPath +"/"+ cacheFile);
                JSONNode json = JSON.Parse(file.ReadToEnd());
                foreach (JSONNode server in json["servers"])
                {
                    List<string> channels = new List<string>();
                    for (int i=0; i<server["channels"].Count; i++)
                    {
                        channels.Add(server["channels"][i]);
                    }
                    new Server(server["name"], channels.ToArray());
                }
                updated = true;
            }
            else
            {
                UnityEngine.Debug.Log("Cache file doesn't exist, refreshing server/channel list.");
                RefreshServerData();
            }
        }

        /// <summary>
        /// get the list of users talking in the voice chat
        /// </summary>
        private void RefreshVoiceData()
        {
            FocusDriverVoice();
            var voiceList = voiceContainer.FindElements(By.XPath(".//li"));
            HashSet<string> disconnectedUsers = new HashSet<string>(new List<string>(DiscordUserManager.instance.users.Keys));
            HashSet<string> currentUsers = new HashSet<string>();
            foreach (var user in voiceList)
            {
                try
                {
                    string id = user.GetAttribute("data-reactid");
                    string[] userIdAttribute = id.Split('$');
                    string userId = userIdAttribute[userIdAttribute.Length-1];
                    currentUsers.Add(userId);
                    string userName = user.FindElement(By.XPath(".//span")).Text;
                    IWebElement image = user.FindElement(By.XPath(".//img"));
                    string profileUrl = image.GetAttribute("src");
                    string speakingClass = image.GetAttribute("class");
                    bool isTalking = speakingClass.Split(' ').Length > 1;
                    DiscordUserManager.instance.InspectUser(userId, userName, profileUrl, isTalking);
                }
                catch (StaleElementReferenceException)
                {
                    continue;
                }
            }

            disconnectedUsers.ExceptWith(currentUsers);
            DiscordUserManager.instance.disconnectedUsers = disconnectedUsers;
            refreshVoice = false;

            FocusDriverMain();
        }

        /// <summary>
        /// toggle show only speaking
        /// </summary>
        public void ToggleSpeakingOnly()
        {
            toggleSpeakingOnly = true;
        }

        /// <summary>
        /// stop the selenium thread
        /// </summary>
        private void StopThread(bool killAll = false)
        {
            updated = false;
            canGetVoice = false;
            refreshVoice = false;
            refreshServers = false;
            threadEnabled = false;
            seleniumOpen = false;
            driver?.Close();
            driver?.Quit();
            driver = null;
            thread?.Abort();

            // this will kill all selenium processes //
            if (killAll)
            {
                Process[] chromeDriverProcesses =  Process.GetProcessesByName("chromedriver");

                foreach(var chromeDriverProcess in chromeDriverProcesses)
                {
                    chromeDriverProcess.Kill();
                }

                // Process[] chromiumHosts =  Process.GetProcessesByName("Chromium host executable (32 bit)");

                // foreach(var chromeHost in chromiumHosts)
                // {
                //     chromeHost.Kill();
                // }
            }
        }

        /// <summary>
        /// kill the thread when the app stops
        /// </summary>
        void OnDestroy()
        {
            StopThread(true);
        }
    }
}
