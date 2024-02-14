using BepInEx.Bootstrap;
using MTM101BaldAPI.AssetManager;
using MTM101BaldAPI.SaveSystem;
using MTM101BaldAPI;
using System.Linq;
using UnityEngine;
using BBE.Events;
using BBE.ExtraContents;
namespace BBE.Helpers
{
    class AssetsHelper
    {
        public static void InitializeData()
        {
            LibraryPatches.PlayerInLibrary = false;
            SoundEvent.IsActive = false;
            ElectricityEvent.activeEventsCount = 0;
            AudioListener.volume = 1f;
            Variables.AngryBaldi = false;
        }
        public static Sprite SpriteFromFile(string path, float pixelsPerUnit = 1f)
        {
            Texture2D texture = AssetManager.TextureFromFile(ModPath + path);
            Sprite sprite = AssetManager.SpriteFromTexture2D(texture, Vector2.one/2f, pixelsPerUnit);
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
        public static void PlayMidi(string MidiPatch, bool loop = false, string MidiName = "RostMoment")
        {
            string midi = AssetManager.MidiFromFile(ModPath + MidiPatch, MidiName);
            Singleton<MusicManager>.Instance.PlayMidi(midi, loop);
        }
        public static SoundObject CreateSoundObject(object audio, SoundType type, Color? color = null, bool Subtitle = true, float sublength = -1f, string SubtitleKey = "Rost moment")
        {
            AudioClip clip = null;
            if (audio is AudioClip)
            {
                clip = (AudioClip)audio;
            }
            else if (audio is string)
            {
                clip = AudioFromFile((string)audio);
            }
            if (clip == null)
            {
                return null;
            }
            SoundObject result = ObjectCreatorHandlers.CreateSoundObject(clip, SubtitleKey, type, color ?? Color.white, sublength);
            result.subtitle = Subtitle;
            return result;
        }
        public static AudioClip AudioFromFile(string path)
        {
            AudioClip clip = AssetManager.AudioClipFromFile(ModPath + path);
            return clip;
        }
        public static Texture2D TextureFromFile(string path)
        {
            return AssetManager.TextureFromFile(ModPath + path);
        }
        public static AudioClip LoadAudioFromOtherMod(string pluginName, string path)
        {
            AudioClip clip = AssetManager.AudioClipFromFile("BALDI_Data/StreamingAssets/Modded/" + pluginName + "/" + path);
            return clip;
        }
        public static string SaveModPath;
        private static string ModPath = "BALDI_Data/StreamingAssets/Modded/rost.moment.baldiplus.extramod/";
    }
}
