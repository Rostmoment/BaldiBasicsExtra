using BBE.Helpers;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.OptionsAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BBE.CustomClasses
{
    public class ModOptions : MonoBehaviour
    {
        public GameObject category;
        public OptionsMenu menu;
        public StandardMenuButton folderButton;
        public StandardMenuButton gamebananaButton;
        public StandardMenuButton githubButton;
        public StandardMenuButton checkVersion;
        public TextLocalizer textLocalizer;
        private static string version = "0.0.0.0";
        public void BuildMenu()
        {
            if (textLocalizer.IsNull())
            {
                textLocalizer = CustomOptionsCore.CreateText(menu, new Vector2(0, 0), "WelcomeOption");
                textLocalizer.textBox.alignment = TextAlignmentOptions.Center;
                textLocalizer.textBox.autoSizeTextContainer = true;
                textLocalizer.transform.SetParent(category.transform, false);
            }
            if (gamebananaButton.IsNull())
            {
                gamebananaButton = CreateObjects.CreateButtonWithSprite("GamebananaPage", BasePlugin.Instance.asset.Get<Sprite>("GamebananaIcon"), parent: category.transform, positon: new Vector3(-100, -150));
                gamebananaButton.OnPress = new UnityEngine.Events.UnityEvent();
                gamebananaButton.OnPress.AddListener(() => { Process.Start("https://gamebanana.com/mods/482309"); });
            }
            if (githubButton.IsNull())
            {
                githubButton = CreateObjects.CreateButtonWithSprite("GithubPage", BasePlugin.Instance.asset.Get<Sprite>("GithubIcon"), parent: category.transform, positon: new Vector3(100, -150));
                githubButton.OnPress = new UnityEngine.Events.UnityEvent();
                githubButton.OnPress.AddListener(() => { Process.Start("https://github.com/Rostmoment/BaldiBasicsExtra"); });
            }
            /*if (folderButton.IsNull())
            {
                folderButton = CreateObjects.CreateButtonWithSprite("OpenFolder", BasePlugin.Instance.asset.Get<Sprite>("FolderIcon"), parent: category.transform, positon: new Vector3(0, -150));
                folderButton.OnPress = new UnityEngine.Events.UnityEvent();
                folderButton.OnPress.AddListener(() => {
                    
                });
            }*/
            if (checkVersion.IsNull())
            {
                checkVersion = CustomOptionsCore.CreateTextButton(menu, new Vector2(0, -100), "CheckVersion", "CheckVersionDesk", async () => 
                {
                    await GetVersion();
                    if (version != "0.0.0.0")
                    {
                        textLocalizer.SetText(string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("ModVersion"), BasePlugin.Instance.Info.Metadata.Version, version));
                    }
                    else
                    {
                        textLocalizer.SetText(Singleton<LocalizationManager>.Instance.GetLocalizedText("WentWrong"));
                    }
                });
                checkVersion.transform.SetParent(category.transform, false);
            }
        }
        void Update()
        {
            if (!textLocalizer.IsNull())
            {
                textLocalizer.textBox.ChangeizeTextContainerState(true);
            }
            if (!checkVersion.IsNull()) 
            {
                checkVersion.text.ChangeizeTextContainerState(true);
            }
        }
        public static async Task<string> GetVersion()
        {
            await ReadVersion(new string[] { });
            return version;
        }
        public static void SetVersion(string data)
        {
            string pattern = "\"([^\"]*)\"";
            MatchCollection matches = Regex.Matches(data, pattern);
            if (matches.Count > 0)
            {
                version = matches[2].Value.Substring(1, matches[2].Value.Length - 2);
            }
        }
        public static async Task ReadVersion(string[] args)
        {
            string fileUrl = "https://raw.githubusercontent.com/Rostmoment/BaldiBasicsExtra/master/BBE/BasePlugin.cs";

            try
            {
                using HttpClient client = new HttpClient();
                string fileContent = await client.GetStringAsync(fileUrl);
                SetVersion(fileContent);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e.Message}");
            }
        }
    }
}