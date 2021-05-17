using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Threading;
using System.Diagnostics;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace DiscordVROverlay
{
    public class DiscordInterface : MonoBehaviour
    {
        public string url;

        private Thread callThread;
        private bool callThreadEnabled = false;
        // private Thread serversThread;
        // private bool serversThreadEnabled = false;
        private ChromeDriver callDriver;
        // private ChromeDriver serversDriver;

        [SerializeField]
        float refreshRate = .3f;

        private string driverPath;

        private bool started = false;
        private bool canRefresh = false;
        private bool doRefresh = true;

        [SerializeField]
        private bool headless = true;

        public static DiscordInterface instance;

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
            Application.targetFrameRate = -1;
        }

        void Start()
        {
            driverPath = Application.dataPath + "/StreamingAssets";
            StartCoroutine(GetPageContents());
        }

        public void ChangeURL(string newURL)
        {
            url = newURL;
            RestartCall();
        }

        void RestartCall()
        {
            started = true;
            canRefresh = false;
            StopCallThread();
            DiscordUserManager.instance.Flush();
            callThread = new Thread(SeleniumCallThread);
            callThread.Start();
            canRefresh = true;
        }

        // public void StartServersThread()
        // {
        //     // serversThread = new Thread(SeleniumServersThread);
        //     // serversThread.Start();
        // }

        IEnumerator GetPageContents()
        {
            while (true)
            {
                if (!started)
                {
                    yield return new WaitForSeconds(.1f);
                    continue;
                }
                while (canRefresh)
                {
                    yield return new WaitForSeconds(refreshRate);
                    doRefresh = true;
                }
            }
        }

        void SeleniumCallThread()
        {
            ChromeOptions options = new ChromeOptions();
            if (headless)
            {
                options.AddArguments(new List<string>(){ "headless" });
            }

            string error = "";
            try
            {
                ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverPath);
                service.HideCommandPromptWindow = headless;
                callDriver = new ChromeDriver(service, options);
            }
            catch(System.InvalidOperationException e)
            {
                print(e.Message);
                string[] splitError = e.Message.Split(' ');
                string chromeVersion = splitError[15].Substring(0,2);
                string chromeDriverVersion = splitError[11].Substring(0,2);
                error = "Currently you have chromedriver version " +
                    chromeDriverVersion +
                    ", while your Google Chrome is version " +
                    chromeVersion +
                    ". Please get the chromedriver that matches the version of your installed Google Chrome. Go to \n" +
                    "https://chromedriver.chromium.org/downloads" +
                    "\n to download the correct version. Unzip the file and drag chromedriver.exe into\n" +
                    "SteamVRDiscordOverlay\\DiscordVROverlay_Data\\StreamingAssets" +
                    "\n in the SteamVRDiscordOverlay folder.";
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            callDriver.Url = url;
            callThreadEnabled = true;
            
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.Log(error);
                ErrorManager.instance.AddError(error.ToString());
                StopCallThread();
                return;
            }

            while (callThreadEnabled)
            {
                if (doRefresh)
                {
                    IWebElement usersXML = callDriver.FindElement(By.XPath("//ul[@class='voice-states']"));
                    var userXMLList = usersXML.FindElements(By.XPath(".//li"));
                    HashSet<string> disconnectedUsers = new HashSet<string>(new List<string>(DiscordUserManager.instance.users.Keys));
                    HashSet<string> currentUsers = new HashSet<string>();
                    foreach (var user in userXMLList)
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
                    doRefresh = false;
                }
            }
        }

        // void SeleniumServersThread()
        // {
        //     ChromeOptions options = new ChromeOptions();
        //     if (headless)
        //     {
        //         options.AddArguments(new List<string>(){ "headless" });
        //     }
        //     ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverPath);
        //     service.HideCommandPromptWindow = headless;
        //     serversDriver = new ChromeDriver(service, options);
        //     serversDriver.Url = "https://streamkit.discord.com/overlay";
        //     serversThreadEnabled = true;

        //     IWebElement obsButton = serversDriver.FindElement(By.XPath("//button[text()='Install for OBS']"));
        //     obsButton.Click();
        //     Thread.Sleep(1000);
        //     IWebElement voiceButton = serversDriver.FindElement(By.XPath("//button[@value='voice']"));
        //     voiceButton.Click();
        //     Thread.Sleep(1000);
        //     IWebElement buttonServerDropdown = serversDriver.FindElement(By.XPath("//div[@data-reactid[contains(.,'selected-guild')]]"));
        //     buttonServerDropdown.Click();
        //     IWebElement serverDropdown = serversDriver.FindElement(By.XPath("//div[@class='Select-menu-outer']"));
        //     print(serverDropdown.Text);




        //     // while (callThreadEnabled)
        //     // {
        //     //     if (doRefresh)
        //     //     {
        //     //     }
        //     // }
        // }

        void StopCallThread()
        {
            callThreadEnabled = false;
            callDriver?.Close();
            callDriver?.Quit();
            callDriver = null;
            callThread?.Abort();

            Process[] chromeDriverProcesses =  Process.GetProcessesByName("chromedriver");

            foreach(var chromeDriverProcess in chromeDriverProcesses)
            {
                UnityEngine.Debug.Log("Destroying Chrome process.");
                chromeDriverProcess.Kill();
            }
        }

        // void StopServersThread()
        // {
        //     serversThreadEnabled = false;
        //     serversDriver?.Quit();
        //     serversThread?.Abort();
        // }

        void OnDestroy()
        {
            StopCallThread();
            // StopServersThread();
        }
    }
}
