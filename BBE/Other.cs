using BBE.CustomClasses;
using BBE.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.Events;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MonoMod.RuntimeDetour;

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
        public static ObjectPlacer AddObjectPlacer(this GameObject prefab, CellCoverage requiredCoverages, params TileShape[] eligibleShapes)
        {
            ObjectPlacer placer = new ObjectPlacer
            {
                eligibleShapes = eligibleShapes.ToList(),
                coverage = requiredCoverages
            };
            if (prefab.GetComponent<EnvironmentObject>().IsNull())
                prefab.AddComponent<EnvironmentObject>();

            placer.prefab = prefab;

            return placer;
        }
        public static List<List<T>> SplitList<T>(this List<T> values, int chunkSize)
        {
            List<List<T>> res = new List<List<T>>();
            for (int i = 0; i < values.Count; i += chunkSize)
            {
                res.Add(values.GetRange(i, Math.Min(chunkSize, values.Count - i)));
            }
            return res;
        }
        public static bool IsActive(this FunSettingsType setting)
        {
            FunSetting s = FunSetting.Get(setting);
            if (s.IsNull())
            {
                return false;
            }
            return s.Value;
        }
        public static void Teleport(this NPC npc)
        {
            if (npc.Character == Character.Cumulo)
            {
                Cumulo cumulo = npc.GetComponent<Cumulo>();
                cumulo.StopBlowing();
                cumulo.windManager.gameObject.SetActive(false);
                cumulo.audMan.FlushQueue(endCurrent: true);
            }
            npc.gameObject.transform.position = npc.ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true).TileTransform.position + Vector3.up * 5f;
        }
        public static bool SomethingIsNull(this object obj, params string[] exceptions)
        {
            bool res = false;
            foreach (FieldInfo data in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (!exceptions.Contains(data.Name) && data.GetValue(obj).IsNull())
                {
                    res = true;
                    Debug.LogWarning(data.Name + " is null!");
                }
            }
            return res;
        }
        public static void SetText(this TextLocalizer textLocalizer, string text)
        {
            textLocalizer.textBox.text = text;
        }
        public static Sprite ToSprite(this Texture2D texture, float x = 1)
        {
            return AssetLoader.SpriteFromTexture2D(texture, x);
        }
        public static void AddFromResources<T>(this AssetManager asset, string resourseName, string name = null) where T : UnityEngine.Object
        {
            if (name.IsNull()) name = resourseName;
            asset.Add<T>(name, AssetsHelper.LoadAsset<T>(resourseName));
        }
        public static T ChooseRandom<T>(this T[] value)
        {
            return value.ToList().ChooseRandom();
        }
        public static T ChooseRandom<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                return default(T);
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        private static byte[] ToCodeInstruction(Action action)
        {
            MethodBody methodBody = action.GetMethodInfo().GetMethodBody();
            return methodBody.GetILAsByteArray();
        }
        public static void DebugAllInstructionsInfo(this IEnumerable<CodeInstruction> instructions)
        {
            for (int x = 0; x<instructions.Count(); x++)
            {
                Debug.Log(x + "/" + instructions.ToList()[x].opcode + "/" + instructions.ToList()[x].operand + "/" + instructions.ToList()[x]);
            }
        }
        public static IEnumerable<CodeInstruction> RemoveByOperandAsString(this IEnumerable<CodeInstruction> instructions, params string[] values) 
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (!values.Contains(instruction.operand.ToString()))
                {
                    yield return instruction;
                }
            }
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
