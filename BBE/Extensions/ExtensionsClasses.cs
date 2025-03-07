using BBE.CustomClasses;
using BBE.Creators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using BBE.ModItems;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System.Diagnostics;
using BBE.Patches;
using BBE.Helpers;

namespace BBE.Extensions
{
    public static class ExtensionsClasses
    {
        public static T ConvertTo<T>(this MonoBehaviour original) where T : MonoBehaviour
        {
            if (original == null)
            {
                BasePlugin.Logger.LogError("ConvertTo failed: original is null");
                return null;
            }

            Type originalType = original.GetType();
            Type targetType = typeof(T);

            if (!targetType.IsSubclassOf(originalType) && targetType != originalType)
            {
                BasePlugin.Logger.LogError($"ConvertTo failed: {targetType.Name} is not {originalType.Name}");
                return null;
            }

            GameObject obj = original.gameObject;
            T newComponent = obj.AddComponent<T>();

            Type type = original.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (FieldInfo field in type.GetFields(flags))
            {
                if (field.IsDefined(typeof(SerializeField), true) || field.IsPublic)
                {
                    field.SetValue(newComponent, field.GetValue(original));
                }
            }
            UnityEngine.Object.Destroy(original);
            return newComponent;
        }

        public static char ToUpper(this char c) => c.ToString().ToUpper().ToCharArray().First();
        public static char ToLower(this char c) => c.ToString().ToLower().ToCharArray().First();
        public static int Count(this string str, char c) => str.Count(x => x == c);

        public static void PatchAllInheritingClasses(this Harmony harmony, Type originalType, string methodName, Type patchClass, string patchMethod, PatchesType type) =>
            harmony.PatchAllInheritingClasses(originalType, methodName, new HarmonyMethod(patchClass, patchMethod), type);
        public static void PatchAllInheritingClasses(this Harmony harmony, Type originalType, string methodName, HarmonyMethod method, PatchesType type)
        {
            Type[] derivedTypes = AssetsHelper.GetDerivedTypes(originalType);
            foreach (Type derivedType in derivedTypes)
            {
                MethodInfo methodInfo = AccessTools.Method(derivedType, methodName);
                if (methodInfo != null)
                {
                    switch (type)
                    {
                        case PatchesType.Prefix:
                            harmony.Patch(original: methodInfo, prefix: method);
                            break;
                        case PatchesType.Transpiler:
                            harmony.Patch(original: methodInfo, transpiler: method);
                            break;
                        case PatchesType.Postfix:
                            harmony.Patch(original: methodInfo, postfix: method);
                            break;
                    }
                }
            }
        }

        public static bool IsOverride(this object obj, string methodName) => obj.GetType().IsOverride(methodName);
        public static bool IsOverride(this Type type, string methodName)
        {
            MethodInfo m = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (m == null) return false;
            return m.IsOverride();
        }
        public static bool IsOverride(this MethodInfo m)
        {
            if (m.DeclaringType == typeof(object) || m.DeclaringType.BaseType == null)
            {
                return false;
            }
            return m.GetBaseDefinition().DeclaringType != m.DeclaringType;
        }

        public static Vector3 FindNearest(this Vector3 start, List<Vector3> vectors)
        {
            if (vectors.EmptyOrNull())
            {
                throw new ArgumentException("The list of vectors cannot be null or empty.");
            }

            Vector3 nearest = vectors[0];
            float minDistance = Vector3.Distance(start, nearest);

            foreach (Vector3 vector in vectors)
            {
                float distance = Vector3.Distance(start, vector);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = vector;
                }
            }

            return nearest;
        }
        public static void SetMainTexture(this Material material, string name)
        {
            material.SetMainTexture(BasePlugin.Asset.Get<Texture2D>(name));
        }
        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public static string ToHex(this Color color)
        {
            string res = "#";
            res += $"{((int)(color.r * 255)).ToString("X")}";
            res += $"{((int)(color.g * 255)).ToString("X")}";
            res += $"{((int)(color.b * 255)).ToString("X")}";
            if (color.a != 0)
                res += $"{((int)(color.a * 255)).ToString("X")}";
            return res;
        }
        public static Color Opposite(this Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            h = (h + 0.5f) % 1.0f;
            return Color.HSVToRGB(h, s, v);
        }
        public static Color GetAverageColor(this Image image, bool useAlpha = false, bool ignoreZeroAlpha = true) => image.sprite.GetAverageColor(useAlpha, ignoreZeroAlpha);
        public static Color GetAverageColor(this Sprite spr, bool useAlpha = false, bool ignoreZeroAlpha = true) => spr.texture.GetAverageColor(useAlpha, ignoreZeroAlpha);
        public static Color GetAverageColor(this Texture2D texture, bool useAlpha = false, bool ignoreZeroAlpha = true)
        {
            try
            {
                Color[] pixels = texture.GetPixels();
                float r = 0, g = 0, b = 0, a = 0;
                int count = 0;
                foreach (Color color in pixels)
                {
                    if (ignoreZeroAlpha && color.a <= 0) continue;

                    r += color.r;
                    g += color.g;
                    b += color.b;
                    a += color.a;
                    count++;
                }
                if (count == 0) return Color.clear;
                r /= count;
                g /= count;
                b /= count;
                a = useAlpha ? a / count : 1;
                return new Color(r, g, b, a);
            }
            catch
            {
                return Color.clear;
            }
        }

        public static void TryPatchAll(this Harmony harmony, Assembly assembly)
        {
            foreach (Type type in AccessTools.GetTypesFromAssembly(assembly))
            {
                if (type == null) continue;
                try
                {
                    harmony.CreateClassProcessor(type).Patch();
                }
                catch
                {
                }
            };
        }

        public static void TryPatchAll(this Harmony _harmony)
        {
            MethodBase method = new StackTrace().GetFrame(1).GetMethod();
            Assembly assembly = method.ReflectedType.Assembly;
            _harmony.TryPatchAll(assembly);
        }

        // Some methods didn't work in game, so I was forced recreate it myself
        public static bool TrueContains(this string str, char c)
        {
            return str.ToCharArray().Contains(c);
        }
        public static bool Between(this int i, int min, int max, bool includeMinAndMax = true)
        {
            if (includeMinAndMax)
                return min<=i && max>=i;
            return min<i && max>i;
        }
        public static bool Between(this float i, float min, float max, bool includeMinAndMax = true)
        {
            if (includeMinAndMax)
                return min <= i && max >= i;
            return min < i && max > i;
        }
        public static bool Exists<T>(this IEnumerable<T> values, Func<T, bool> func) => values.Exists(func, out _);
        public static bool Exists<T>(this IEnumerable<T> values, Func<T, bool> func, out T result)
        {
            if (values.Count(func) > 0)
            {
                result = values.Where(func).First();
                return true;
            }
            result = default(T);
            return false;
        }
        public static int FindNearest(this IEnumerable<int> ints, float value)
        {
            int nearest = ints.ElementAt(0);
            float minDifference = Math.Abs(ints.ElementAt(0) - value);

            foreach (int x in ints)
            {
                float difference = Math.Abs(x - value);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    nearest = x;
                }
            }
            return nearest;
        }
        public static float Clamp(this float f, float min, float max)
        {
            if (f > max) f = max;
            if (f < min) f = min;
            return f;
        }
        public static int Clamp(this int f, int min, int max)
        {
            if (f > max) f = max;
            if (f < min) f = min;
            return f;
        }
        public static string[] TrueSplit(this string input, string delimiter)
        {
            List<string> substrings = new List<string>();
            int currentIndex = 0;
            int delimiterIndex;
            while ((delimiterIndex = input.IndexOf(delimiter, currentIndex)) != -1)
            {
                substrings.Add(input.Substring(currentIndex, delimiterIndex - currentIndex));
                currentIndex = delimiterIndex + delimiter.Length;
            }
            if (currentIndex < input.Length)
            {
                substrings.Add(input.Substring(currentIndex));
            }
            return substrings.ToArray();
        }
        public static string[] TrueSplit(this string input, char delimiter)
        {
            List<string> substrings = new List<string>();
            string current = "";
            foreach (char c in input)
            {
                if (c == delimiter)
                {
                    substrings.Add(current);
                    current = "";
                }
                else current += c;
            }
            if (current != "") substrings.Add(current);
            return substrings.ToArray();
        }
        public static int AddToSiblingIndex(this Transform transform, int value)
        {
            transform.SetSiblingIndex(transform.GetSiblingIndex() + value);
            return transform.GetSiblingIndex();
        }
        public static Transform[] GetChilds(this Transform transform)
        {
            List<Transform> childs = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++) childs.Add(transform.GetChild(i));
            return childs.ToArray();
        }
        public static Vector3 ToVector3(this IntVector2 intVector2, float y = 5)
        {
            return new Vector3(intVector2.x, y, intVector2.z);
        }
        public static Texture2D FlipByY(this Texture2D original)
        {
            Texture2D flipped = new Texture2D(original.width, original.height);
            for (int y = 0; y < original.height; y++)
            {
                for (int x = 0; x < original.width; x++)
                {
                    flipped.SetPixel(x, y, original.GetPixel(x, original.height - 1 - y));
                }
            }
            flipped.Apply();
            return flipped;
        }
        public static Texture2D FlipByX(this Texture2D original)
        {
            Texture2D flipped = new Texture2D(original.width, original.height);
            for (int y = 0; y < original.height; y++)
            {
                for (int x = 0; x < original.width; x++)
                {
                    flipped.SetPixel(x, y, original.GetPixel(original.width - 1 - x, y));
                }
            }
            flipped.Apply();
            return flipped;
        }
        public static Texture2D CopyTexture(this Texture2D original)
        {
            Texture2D copy = new Texture2D(original.width, original.height, original.format, false);
            if (original.isReadable)
            {
                copy.SetPixels(original.GetPixels());
                return copy;
            }
            RenderTexture rt = new RenderTexture(original.width, original.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(original, rt);
            RenderTexture.active = rt;
            copy.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            copy.Apply();
            RenderTexture.active = null;
            rt.Release();
            return copy;
        }
        public static Entity CreateEntity(this GameObject target, Transform rendererBase = null) => target.CreateEntity(0, 0, rendererBase);
        public static Entity CreateEntity(this GameObject target, Collider collider, Collider triggerCollider, Transform rendererBase = null)
        {
            LayerMask mask = target.layer;
            Entity entity = target.AddComponent<Entity>();
            entity.SetActive(false);
            target.layer = mask;
            entity.rendererBase = rendererBase;
            entity.collider = collider;
            entity.externalActivity = target.AddComponent<ActivityModifier>();
            entity.trigger = triggerCollider;
            return entity;
        }
        public static void SetSpeed(this Navigator navigator, float speed, float maxSpeed)
        {
            navigator.maxSpeed = maxSpeed;
            navigator.SetSpeed(speed);
        }
        public static Vector3 Change(this Vector3 vector, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            if (float.IsNaN(x)) x = vector.x;
            if (float.IsNaN(y)) y = vector.y;
            if (float.IsNaN(z)) z = vector.z;
            return new Vector3(x, y, z);
        }
        public static Color Change(this Color color, float r = -1, float g = -1, float b = -1, float a = -1)
        {
            if (r == -1) r = color.r;
            if (g == -1) g = color.g;
            if (b == -1) b = color.b;
            if (a == -1) a = color.a;
            r = r.Clamp(0f, 1f);
            g = g.Clamp(0f, 1f);
            b = b.Clamp(0f, 1f);
            a = a.Clamp(0f, 1f);
            return new Color(r, g, b, a);
        }
        public static BoxCollider AddCollider(this GameObject obj, Vector3 size, bool isTrigger = true) => obj.AddCollider(size, Vector3.zero, isTrigger);
        public static BoxCollider AddCollider(this GameObject obj, Vector3 size, Vector3 center, bool isTrigger = true)
        {
            BoxCollider res = obj.AddComponent<BoxCollider>();
            res.size = size;
            res.center = center;
            res.isTrigger = isTrigger;
            return res;
        }
        public static Sprite ReplaceColor(this Sprite originalSprite, Color referenceColor, Color colorToReplace)
        {
            Texture2D texture = UnityEngine.Object.Instantiate(originalSprite.texture);
            texture.filterMode = FilterMode.Point;
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color pixelColor = texture.GetPixel(x, y);
                    if (pixelColor.Equals(referenceColor)) texture.SetPixel(x, y, colorToReplace);
                }
            }
            texture.Apply();
            return Sprite.Create(texture, originalSprite.rect, new Vector2(0.5f, 0.5f), originalSprite.pixelsPerUnit);
        }
        public static string Localize(this string key, bool encrypted = false)
        {
            return LocalizationManager.Instance.GetLocalizedText(key, encrypted);
        }
        public static Sprite ResizeSprite(this Sprite sprite, float pixelsPerUnit = float.NaN, Vector2? center = null) => sprite.ResizeSprite(sprite.texture.width, sprite.texture.height, pixelsPerUnit, center);
        public static Sprite ResizeSprite(this Sprite sprite, int newWidth, int newHeight, float pixelsPerUnit = float.NaN, Vector2? center = null)
        {
            if (center == null)
                center = sprite.pivot;
            if (float.IsNaN(pixelsPerUnit))
                pixelsPerUnit = sprite.pixelsPerUnit;
            if (center == sprite.pivot && pixelsPerUnit == sprite.pixelsPerUnit && sprite.texture.width == newWidth && sprite.texture.height == newHeight)
                return sprite;
            if (float.IsNaN(pixelsPerUnit))
                pixelsPerUnit = sprite.pixelsPerUnit;
            Vector2 vector2;
            if (center == null)
                vector2 = Vector2.one / 2f;
            else
                vector2 = new Vector2(center.Value.x, center.Value.y);
            return Sprite.Create(sprite.texture.ResizeTexture(newWidth, newHeight), new Rect(0, 0, newWidth, newHeight), vector2, pixelsPerUnit);
        }
        public static Texture2D ResizeTexture(this Texture2D sourceTexture, int newWidth, int newHeight)
        {
            if (sourceTexture == null)
                return AssetsHelper.CreateTexture(newWidth, newHeight, Color.clear);
            if (sourceTexture.width == newWidth && sourceTexture.height == newHeight)
                return sourceTexture;
            Texture2D reference = sourceTexture.CopyTexture();
            Texture2D resizedTexture = new Texture2D(newWidth, newHeight, sourceTexture.format, false);
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    float xRatio = (float)x / newWidth;
                    float yRatio = (float)y / newHeight;
                    Color color = reference.GetPixelBilinear(xRatio, yRatio);
                    resizedTexture.SetPixel(x, y, color);
                }
            }
            resizedTexture.Apply();
            return resizedTexture;
        }
        public static SoundObject AddAdditionalKey(this SoundObject soundObject, string key, float time, bool encrypted = false)
        {
            if (soundObject.additionalKeys == null) soundObject.additionalKeys = new SubtitleTimedKey[] { };
            soundObject.additionalKeys = soundObject.additionalKeys.AddToArray(new SubtitleTimedKey() { encrypted = encrypted, key = key, time = time });
            return soundObject;
        }
        public static T Duplicate<T>(this T obj) where T : Component
        {
            T res = UnityEngine.Object.Instantiate(obj);
            res.gameObject.ConvertToPrefab(res.gameObject.activeSelf);
            res.name = obj.name;
            return res;
        }
        public static Color Copy(this Color color)
        {
            return new Color(color.r, color.g, color.b, color.a);
        }
        public static Texture2D ReadTexture(this BinaryReader reader)
        {
            if (!reader.ReadBoolean()) return null;
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            TextureFormat format = (TextureFormat)reader.ReadInt32();
            int dataLength = reader.ReadInt32();
            byte[] textureData = reader.ReadBytes(dataLength);
            Texture2D texture = new Texture2D(width, height, format, false);
            texture.LoadRawTextureData(textureData);
            texture.Apply();

            return texture;
        }
        public static void Write(this BinaryWriter writer, Texture2D texture)
        {
            if (texture == null)
            {
                writer.Write(false);
                return;
            }
            writer.Write(true);
            writer.Write(texture.width);
            writer.Write(texture.height);
            writer.Write((int)texture.format);
            byte[] textureData = texture.GetRawTextureData();
            writer.Write(textureData.Length);
            writer.Write(textureData);
        }
        public static AudioClip Reverse(this AudioClip audioClip)
        {
            float[] samples = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(samples, 0);

            samples = samples.Reverse().ToArray();

            AudioClip reversedClip = AudioClip.Create(
                audioClip.name + "_Reversed",
                audioClip.samples,
                audioClip.channels,
                audioClip.frequency,
                false
            );

            reversedClip.SetData(samples, 0);

            return reversedClip;
        }
        public static bool HasComponent<T>(this GameObject obj) where T : Component
        {
            return obj.TryGetComponent<T>(out _);
        }
        public static bool HasComponent<T>(this Component obj) where T : Component
        {
            return obj.TryGetComponent<T>(out _);
        }
        public static bool DeleteComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null)
            {
                return false;
            }
            if (!obj.HasComponent<T>())
            {
                return false;
            }
            UnityEngine.Object.Destroy(obj.GetComponent<T>());
            return true;
        }
        public static bool SomethingIsNull(this object obj, params string[] exceptions)
        {
            bool res = false;
            foreach (FieldInfo data in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (exceptions.Contains(data.Name) && data.GetValue(obj) != null)
                {
                    res = true;
                    BasePlugin.Logger.LogWarning(data.Name + " is null!");
                }
            }
            return res;
        }
        public static void SetText(this TextLocalizer textLocalizer, string text)
        {
            textLocalizer.textBox.text = text;
        }
        public static Sprite ToSprite(this Texture2D texture, float x, float y, float pixelsPerUnit = 1f) => texture.ToSprite(new Vector2(x, y), pixelsPerUnit);
        public static Sprite ToSprite(this Texture2D texture, float x = 1) => AssetLoader.SpriteFromTexture2D(texture, x);
        public static Sprite ToSprite(this Texture2D texture, float x, Vector2 center) => texture.ToSprite(center, x);
        public static Sprite ToSprite(this Texture2D texture, Vector2 center, float x = 1f) => AssetLoader.SpriteFromTexture2D(texture, center, x);
        public static void SetToCanvas(this Canvas canvas, Transform transform, Vector2 position, Vector2 size)
        {
            transform.SetParent(canvas.transform);
            transform.localPosition = position;
            transform.localScale = size;
            transform.gameObject.layer = LayerMask.NameToLayer("UI");
        }
        public static Image CreateImage(this Canvas canvas, Vector2 position, Vector2 size, bool enabled = true, Color? color = null, Sprite sprite = null)
        {
            var img = new GameObject("Image").AddComponent<Image>();
            img.sprite = sprite;
            img.color = color ?? Color.white;
            canvas.SetToCanvas(img.transform, position, size);
            img.enabled = enabled;

            return img;
        }
        public static void ChangeizeTextContainerState(this TMP_Text text, bool state)
        {
            text.autoSizeTextContainer = !state;
            text.autoSizeTextContainer = state;
            text.autoSizeTextContainer = !state;
            text.autoSizeTextContainer = state;
        }
    }
}
