using BBE.Events;
using BepInEx.Bootstrap;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System.Linq;
using System.IO;
using UnityEngine;
using BBE.ExtraContents;
using BBE.ExtraContents.FunSettings;
using BBE.ModItems;

namespace BBE.Helpers
{
    class AssetsHelper
    {
        public static void ClearData()
        {
            SoundEvent.ActiveEventsCount = 0;
            ElectricityEvent.ActiveEventsCount = 0;
            AudioListener.volume = 1f;
            ITM_Shield.TimeLeft = 0;
        }

        public static Sprite SpriteFromFile(string path, float pixelsPerUnit = 1f)
        {
            Texture2D texture = AssetLoader.TextureFromFile(ModPath + path);
            Sprite sprite = AssetLoader.SpriteFromTexture2D(texture, Vector2.one/2f, pixelsPerUnit);
            return sprite;
        }
        public static T Find<T>() where T : Object
        {
            try { return Resources.FindObjectsOfTypeAll<T>()[0]; }
            catch { return null; }
            
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
                clip = AssetLoader.AudioClipFromMod(new LoadMod(), (string)audio);
            }
            if (clip == null)
            {
                return null;
            }
            SoundObject result = ObjectCreators.CreateSoundObject(clip, SubtitleKey, type, color ?? Color.white, -1f);
            result.subtitle = Subtitle;
            return result;
        }
        public static AudioClip AudioFromFile(string path)
        {
            return AssetLoader.AudioClipFromMod(new LoadMod(), path);
        }
        private static string ModPath = "BALDI_Data/StreamingAssets/Modded/rost.moment.baldiplus.extramod/";
    }
}
