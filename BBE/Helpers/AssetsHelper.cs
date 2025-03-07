using BBE.Events;
using BepInEx.Bootstrap;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System.Linq;
using System.IO;
using UnityEngine;
using BBE.ModItems;
using TMPro;
using System.Collections.Generic;
using UnityEngine.TextCore.LowLevel;
using System.Diagnostics;
using BBE.Extensions;
using System.Reflection;

namespace BBE.Helpers
{
    class AssetsHelper
    {
        public static string[] GetFiles(string path) 
        {
            return Directory.GetFiles(ModPath + path);
        }
        public static T[] All<T>() where T : System.Enum => (T[])System.Enum.GetValues(typeof(T));
        // Thanks to PRBF
        public static TMP_FontAsset FontFromFile(params string[] path)
        {
            return FontFromFile(Path.Combine(path), 24);
        }
        public static TMP_FontAsset FontFromFile(string path, int size = 24, int atlasWidth = 1024, int atlasHeight = 1024, GlyphRenderMode renderMode = GlyphRenderMode.RASTER_HINTED)
        {
            if (FontEngine.LoadFontFace(ModPath + path, size) > FontEngineError.Success)
                MTM101BaldiDevAPI.CauseCrash(BasePlugin.Instance.Info, new System.Exception("Something wrong with " + path));
            Font font = new Font(ModPath + path);
            TMP_FontAsset tmp_FontAsset = TMP_FontAsset.CreateFontAsset(font, size, 2, renderMode, atlasWidth, atlasHeight, AtlasPopulationMode.Dynamic, true);
            tmp_FontAsset.name = Path.GetFileNameWithoutExtension(path);
            tmp_FontAsset.material.shader = Shader.Find("TextMeshPro/Bitmap");
            tmp_FontAsset.atlasTexture.filterMode = FilterMode.Point;
            return tmp_FontAsset;
        }
        public static bool FileIsExists(string path)
        {
            return File.Exists(ModPath + path);
        }
        public static Texture2D CreateTexture(int size, bool includeAlpha = false) =>
            CreateTexture(size, GenerateRandomColor(includeAlpha));
        public static Texture2D CreateTexture(int size, Color color) =>
            CreateTexture(size, size, color);
        public static Texture2D CreateTexture(int width, int height, bool includeAlpha = false)
        {
            return CreateTexture(width, height, GenerateRandomColor(includeAlpha));
        }
        public static Texture2D CreateTexture(int width, int height, string color) =>
            CreateTexture(width, height, ColorFromHex(color));
        public static Texture2D CreateTexture(int width, int height, Color color)
        {
            Texture2D texture2D = new Texture2D(width, height);
            for (int x = 0; x < texture2D.width; x++)
            {
                for (int y = 0; y < texture2D.height; y++)
                {
                    texture2D.SetPixel(x, y, color);
                }
            }
            texture2D.Apply();
            return texture2D;
        }
        public static Texture2D CreateTexture(List<string> path, TextureFormat format) =>
            CreateTexture(format, path);
        public static Texture2D CreateTexture(TextureFormat format, List<string> path) =>
            CreateTexture(format, path.ToArray());
        public static Texture2D CreateTexture(List<string> path) =>
            CreateTexture(TextureFormat.RGBA32, path.ToArray());
        public static Texture2D CreateTexture(params string[] path) =>
            CreateTexture(TextureFormat.RGBA32, path);
        public static Texture2D CreateTexture(TextureFormat format, params string[] path) =>
            AssetLoader.TextureFromFile(ModPath + Path.Combine(path), format);
        public static T[] FindAllOfType<T>() where T : Object
        {
            return Resources.FindObjectsOfTypeAll<T>();
        }
        public static bool AssetsAreInstalled()
        {
            return Directory.Exists(ModPath);
        }
        public static T LoadAsset<T>(bool includeNull) where T : Object
        {
            try
            {
                T[] t = FindAllOfType<T>().Where(x => x.GetType() == typeof(T)).ToArray();
                if (!includeNull)
                    t = t.Where(x => x != null).ToArray();
                return t.First();
            }
            catch
            {
                return null;
            }
        }
        public static T LoadAsset<T>(int index = 0) where T : Object
        {
            try { return Resources.FindObjectsOfTypeAll<T>().Where(x => x.GetType() == typeof(T)).ToList()[index]; }
            catch { return null; }
        }
        public static T LoadAsset<T>(System.Func<T, bool> predicate) where T : Object
        {
            return (from x in Resources.FindObjectsOfTypeAll<T>() where predicate(x) select x).Where(x => x.GetType() == typeof(T)).First();
        }
        public static T LoadAsset<T>(string name) where T : Object
        {
            return (from x in Resources.FindObjectsOfTypeAll<T>()
                    where x.name.ToLower() == name.ToLower()
                    select x).Where(x => x.GetType() == typeof(T)).First();
        }
        public static bool ModInstalled(string mod)
        {
            return Chainloader.PluginInfos.ContainsKey(mod);
        }
        public static System.Type[] GetDerivedTypes(System.Type baseType)
        {
            return Assembly.GetAssembly(baseType)
                           .GetTypes()
                           .Where(t => t.IsSubclassOf(baseType))
                           .ToArray();
        }
        public static SoundObject CreateSoundObject(AudioClip audio, SoundType type, string color = null, float sublength = -1f, string subtitleKey = "") =>
            CreateSoundObject(audio, type, ColorFromHex(color), sublength, subtitleKey);
        public static SoundObject CreateSoundObject(AudioClip audio, SoundType type, Color? color = null, float sublength = -1f, string subtitleKey = "")
        {
            SoundObject result = ObjectCreators.CreateSoundObject(audio, subtitleKey, type, color ?? Color.white, sublength);
            if (subtitleKey.EmptyOrNull())
                result.subtitle = false;
            if (result.color.a == 0)
                result.subtitle = false;
            return result;
        }
        public static Color GenerateRandomColor(bool includeAlpha = false) 
        {
            float r = Random.Range(0, 256) / 256f;
            float g = Random.Range(0, 256) / 256f;
            float b = Random.Range(0, 256) / 256f;
            float a = 1f;
            if (includeAlpha)
                a = Random.Range(0, 256) / 256f;
            return new Color(r, g, b, a);
        }
        public static Color ColorFromHex(string hex)
        {
            if (hex.EmptyOrNull())
                return new Color(0, 0, 0, 0);
            while (hex.StartsWith("#"))
            {
                hex = hex.Substring(1);
            }
            List<List<char>> charList = hex.ToList().SplitList(2);
            if (charList.Count != 4 && charList.Count != 3)
            {
                BasePlugin.Logger.LogWarning("HexCode " + hex + " is invalid!");
                return new Color(0, 0, 0, 0);
            }
            List<int> values = new List<int>() { };
            foreach (List<char> chars in charList)
            {
                if (chars.Count != 2)
                {
                    BasePlugin.Logger.LogWarning("HexCode " + hex + " is invalid!");
                    return new Color(0, 0, 0, 0);
                }
                string temp = chars[0].ToString() + chars[1].ToString();
                try
                {
                    values.Add(System.Convert.ToInt16(temp, 16));
                }
                catch
                {
                    BasePlugin.Logger.LogWarning("HexCode " + hex + " is invalid!");
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
        public static Sprite[] CreateSpriteSheet(int tilesByX, int tilesByY, float pixelsPerUnit = 1f, params string[] path)
        {
            return AssetsHelper.CreateSpriteSheet(AssetsHelper.CreateTexture(path), tilesByX, tilesByY, pixelsPerUnit);
        }
        public static Sprite[] CreateSpriteSheet(Texture2D texture, int tilesByX, int tilesByY, float pixelsPerUnit = 1f)
        {
            return AssetLoader.SpritesFromSpritesheet(tilesByX, tilesByY, pixelsPerUnit, Vector2.one / 2f, texture);
        }
        public static Sprite CreateColoredSprite(Color c, int xSize, int ySize, float pixelsPerUnit = 1)
        {
            Texture2D texture = new Texture2D(xSize, ySize);
            for (int x = 0; x < texture.width; x++)
            {
                for (int y  = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, c);
                }
            }
            texture.Apply();
            return texture.ToSprite(pixelsPerUnit);
        }
        public static AudioClip AudioFromFile(params string[] path)
        {
            return AssetLoader.AudioClipFromMod(BasePlugin.Instance, path);
        }
        public static string ModPathToOpen
        {
            get
            {
                string res = Application.dataPath + "/StreamingAssets/Modded/rost.moment.baldiplus.extramod";
                return res.Replace("/", "\\" + "\\");
            }
        }
        public static string ModPath 
        {
            get
            { 
                return AssetLoader.GetModPath(BasePlugin.Instance)+"/";
            }
        }
    }
}
