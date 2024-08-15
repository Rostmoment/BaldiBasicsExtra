using BBE.Events;
using BepInEx.Bootstrap;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System.Text;
using System.Linq;
using System.IO;
using UnityEngine;
using BBE.ModItems;
using HarmonyLib;
using TMPro;
using NPOI.HPSF;
using System.Collections.Generic;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore;
using System.Diagnostics;
using UnityEngine.Analytics;
using BBE.Patches;

namespace BBE.Helpers
{
    class AssetsHelper
    {
        public static void ClearData()
        {
            SoundEvent.ActiveEventsCount = 0;
            ElectricityEvent.ActiveEventsCount = 0;
            AudioListener.volume = 1f;
            ITM_Shield.Used = false;
            ITM_TimeReverser.Used = false;
            /*for (int i = 0; i < Singleton<CoreGameManager>.Instance.GetHud(0).transform.childCount; i++)
            {
                if (names.Contains(Singleton<CoreGameManager>.Instance.GetHud(0).transform.GetChild(i).gameObject.name))
                {
                    Object.Destroy(Singleton<CoreGameManager>.Instance.GetHud(0).transform.GetChild(i).gameObject);
                }
            }*/
        }

        // Thanks to PRBF
        public static TMP_FontAsset FontFromFile(string path, int size = 24, int atlasWidth = 1024, int atlasHeight = 1024, GlyphRenderMode renderMode = GlyphRenderMode.RASTER_HINTED)
        {
            Font font = new Font(ModPath + path);
            TMP_FontAsset tmp_FontAsset = TMP_FontAsset.CreateFontAsset(font, size, 2, renderMode, atlasWidth, atlasHeight, AtlasPopulationMode.Dynamic, true);
            tmp_FontAsset.name = Path.GetFileNameWithoutExtension(path);
            tmp_FontAsset.material.shader = Shader.Find("TextMeshPro/Bitmap");
            tmp_FontAsset.atlasTexture.filterMode = FilterMode.Point;
            return tmp_FontAsset;
        }
        public static Sprite SpriteFromFile(string path, float pixelsPerUnit = 1f)
        {
            Sprite sprite = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(ModPath + path), Vector2.one/2f, pixelsPerUnit);
            return sprite;
        }
        public static bool FileIsExists(string path)
        {
            return File.Exists(ModPath + path);
        }
        public static Texture2D TextureFromFile(params string[] path)
        {
            return AssetLoader.TextureFromFile(ModPath + Path.Combine(path));
        }
        public static T[] FindAllOfType<T>() where T : Object
        {
            return Resources.FindObjectsOfTypeAll<T>();
        }
        public static T Find<T>(int index = 0) where T : Object
        {
            try { return Resources.FindObjectsOfTypeAll<T>()[index]; }
            catch { return null; }

        }
        public static bool AssetsAreInstalled()
        {
            return Directory.Exists(ModPath);
        }
        public static T LoadAsset<T>(string name) where T : Object
        {
            return (from x in Resources.FindObjectsOfTypeAll<T>()
                    where x.name.ToLower() == name.ToLower()
                    select x).First();
        }
        public static bool ModInstalled(string mod)
        {
            return Chainloader.PluginInfos.ContainsKey(mod);
        }
        public static SoundObject CreateSoundObject(object audio, SoundType type, Color? color = null, bool Subtitle = true, float sublength = 1f, string SubtitleKey = "Rost moment", SubtitleTimedKey[] timedKeys = null)
        {
            AudioClip clip = null;
            if (audio is AudioClip)
            {
                clip = (AudioClip)audio;
            }
            else if (audio is string)
            {
                clip = AssetLoader.AudioClipFromMod(BasePlugin.Instance, (string)audio);
            }
            if (clip.IsNull())
            {
                throw new System.NullReferenceException("BRO WHAT THE HECK ARE YOU DOING!?!?!?!??!");
            }
            SoundObject result = ObjectCreators.CreateSoundObject(clip, SubtitleKey, type, color ?? Color.white, -1f);
            if (!timedKeys.IsNull() && timedKeys.Length > 0)
            {
                result.additionalKeys = timedKeys;
            }
            result.subtitle = Subtitle;
            return result;
        }
        public static Color ColorFromHex(string hex)
        {
            if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1);
            }
            List<List<char>> charList = hex.ToList().SplitList(2);
            if (charList.Count != 4 && charList.Count != 3)
            {
                BasePlugin.logger.LogWarning("HexCode " + hex + " is invalid!");
                return Color.clear;
            }
            List<int> values = new List<int>() { };
            foreach (List<char> chars in charList)
            {
                if (chars.Count != 2)
                {
                    BasePlugin.logger.LogWarning("HexCode " + hex + " is invalid!");
                    return Color.clear;
                }
                string temp = chars[0].ToString() + chars[1].ToString();
                try
                {
                    values.Add(System.Convert.ToInt16(temp, 16));
                }
                catch
                {
                    BasePlugin.logger.LogWarning("HexCode " + hex + " is invalid!");
                }
            }
            if (values.Count == 4)
            {
                return new Color(values[0] / 255f, values[1] / 255f, values[2] / 255f, values[3] / 255f);
            }
            return new Color(values[0] / 255f, values[1] / 255f, values[2] / 255f);
        }
        public static string MidiFromFile(string path, string name)
        {
            return AssetLoader.MidiFromMod(name, BasePlugin.Instance, path);
        }
        public static List<Sprite> CreateSpriteSheet(string path, int tilesByX, int tilesByY, float pixelsPerUnit = 1f)
        {
            return AssetsHelper.CreateSpriteSheet(AssetsHelper.TextureFromFile(path), tilesByX, tilesByY, pixelsPerUnit);
        }
        public static List<Sprite> CreateSpriteSheet(Texture2D texture, int tilesByX, int tilesByY, float pixelsPerUnit = 1f)
        {
            List<Sprite> result = new List<Sprite>();
            int XSize = texture.width / tilesByX;
            int YSize = texture.height / tilesByY;
            for (int y = 0; y < tilesByY; y++)
            {
                for (int x = 0; x < tilesByX; x++)
                {
                    result.Add(Sprite.Create(texture, new Rect(x * XSize, y * YSize, XSize, YSize), Vector2.one / 2f, pixelsPerUnit, 0, SpriteMeshType.FullRect));
                }
            }
            return result;
        }
        public static Sprite CreateColoredSprite(Color c, float pixelsPerUnit = 1)
        {
            Texture2D texture = AssetsHelper.TextureFromFile("Textures", "Other", "Placeholder.png");
            for (int x = 0; x < texture.width; x++)
            {
                for (int y  = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, c);
                }
            }
            texture.Apply();
            return AssetLoader.SpriteFromTexture2D(texture, pixelsPerUnit);
        }
        public static AudioClip AudioFromFile(string path)
        {
            return AssetLoader.AudioClipFromMod(BasePlugin.Instance, path);
        }
        public static void UseCmd(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            Process process = Process.Start(processInfo);
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                { };
            process.BeginOutputReadLine();
            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                { };
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Close();
        }
        public static string ModPathToOpen
        {
            get
            {
                string res = Application.dataPath + "/StreamingAssets/Modded/rost.moment.baldiplus.extramod";
                return res.Replace("/", "\\" + "\\");
            }
        }
        public static string ModPath => "BALDI_Data/StreamingAssets/Modded/rost.moment.baldiplus.extramod/";
    }
}
