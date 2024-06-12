using BBE.Events;
using BepInEx.Bootstrap;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using System.IO;
using UnityEngine;
using BBE.ModItems;
using HarmonyLib;
using System.Collections.Generic;

namespace BBE.Helpers
{
    class AssetsHelper
    {
        public static void ClearData()
        {
            List<string> names = new List<string>() { "Typerex_Text", "Typerex_Text_Word", "PlayerAnswer_Text", "EquationText_Text" };
            SoundEvent.ActiveEventsCount = 0;
            ElectricityEvent.ActiveEventsCount = 0;
            AudioListener.volume = 1f;
            ITM_Shield.TimeLeft = 0;
            /*for (int i = 0; i < Singleton<CoreGameManager>.Instance.GetHud(0).transform.childCount; i++)
            {
                if (names.Contains(Singleton<CoreGameManager>.Instance.GetHud(0).transform.GetChild(i).gameObject.name))
                {
                    Object.Destroy(Singleton<CoreGameManager>.Instance.GetHud(0).transform.GetChild(i).gameObject);
                }
            }*/
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
        public static T Find<T>() where T : Object
        {
            try { return Resources.FindObjectsOfTypeAll<T>()[0]; }
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
        public static SoundObject CreateSoundObject(object audio, SoundType type, Color? color = null, bool Subtitle = true, float sublength = 1f, string SubtitleKey = "Rost moment")
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
                return LoadAsset<SoundObject>("Teleport");
            }
            SoundObject result = ObjectCreators.CreateSoundObject(clip, SubtitleKey, type, color ?? Color.white, -1f);
            result.subtitle = Subtitle;
            return result;
        }
        public static string MidiFromFile(string path, string name)
        {
            return AssetLoader.MidiFromFile(ModPath + path, name);
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
        public static string ModPath => "BALDI_Data/StreamingAssets/Modded/rost.moment.baldiplus.extramod/";
    }
}
