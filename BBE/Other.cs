using BBE.Helpers;
using BBE.Patches;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBE
{
    public enum Floor
    {
        Floor1 = 1,
        Floor2 = 2,
        Floor3 = 3,
        Endless = 4,
        Challenge = 5,
        None = 0
    }
    static class ExtensionsClasses
    {
        public static Sprite ToSprite(this Texture2D texture, float x = 1)
        {
            return AssetLoader.SpriteFromTexture2D(texture, x);
        }
        public static Vector3 FindNearestPoint(this Vector3 targetPoint, Vector3[] points)
        {
            if (points == null || points.Length == 0)
            {
                Debug.LogError("The points array is empty or null!");
                return Vector3.zero;
            }
            float minDistance = Mathf.Infinity;
            Vector3 nearestPoint = Vector3.zero;
            foreach (Vector3 point in points)
            {
                float distance = Vector3.Distance(targetPoint, point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPoint = point;
                }
            }
            return nearestPoint;
        }
        public static void AddFromResources<T>(this AssetManager asset, string resourseName, string name = null) where T : UnityEngine.Object
        {
            if (name.IsNull()) name = resourseName;
            asset.Add<T>(name, AssetsHelper.LoadAsset<T>(resourseName));
        }
        public static T ChooseRandom<T>(this T value) where T : Enum
        {
            Array array = Enum.GetValues(typeof(T));
            List<T> values = new List<T>();
            foreach (T en in array)
            {
                values.Add(en);
            }
            return values.ChooseRandom();
        }
        public static T ChooseRandom<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                return default(T);
            }
            return list[UnityEngine.Random.Range(0, list.Count-1)];
        }
        public static IEnumerable<CodeInstruction> RemoveAtIndexes(this IEnumerable<CodeInstruction> instructions, params int[] indexes)
        {
            int x = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                if (!indexes.Contains(x)) 
                {
                    yield return instruction;
                }
                x++;
            }
        }
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
        public static T ToEnum<T>(this string text) where T : Enum
        {
            return EnumExtensions.ExtendEnum<T>(text);
        }
        public static void PlaySingle(this AudioManager audMan, string key)
        {
            audMan.PlaySingle(BasePlugin.Instance.asset.Get<SoundObject>(key));
        }
        public static void QueueAudio(this AudioManager audMan, string key)
        {
            audMan.QueueAudio(BasePlugin.Instance.asset.Get<SoundObject>(key));
        }
        public static void ChangeizeTextContainerState(this TMP_Text text, bool state)
        {
            text.autoSizeTextContainer = !state;
            text.autoSizeTextContainer = state;
            text.autoSizeTextContainer = !state;
            text.autoSizeTextContainer = state;
        }
        public static bool IsNull(this object  obj)
        {
            return obj == null;
        }
        public static Floor ToFloor(this string value)
        {
            if (value == "Main1" || value == "F1")
            {
                return Floor.Floor1;
            }
            if (value == "Main2" || value == "F2")
            {
                return Floor.Floor2;
            }
            if (value == "Main3" || value == "F3")
            {
                return Floor.Floor3;
            }
            if (value == "Endless1" || value == "END")
            {
                return Floor.Endless;
            }
            return Floor.None;
        }
    }
}
