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
using System.IO;
using MTM101BaldAPI.ObjectCreation;

namespace BBE
{
    public enum Floor
    {
        Floor1 = 1,
        Floor2 = 2,
        Floor3 = 3,
        Endless = 4,
        Challenge = 5,
        Mixed = 6,
        None = 0
    }
    public static class ExtensionsClasses
    {
        public static bool IsNot<T>(this object obj) => !(obj is T);
        public static bool InList<T>(this T obj, List<T> list) => list.Contains(obj);
        public static T Duplicate<T>(this T obj) where T : Component
        {
            T res = UnityEngine.Object.Instantiate(obj);
            res.gameObject.ConvertToPrefab(res.gameObject.activeSelf);
            res.name = obj.name;
            return res;
        }
        public static ItemObject Copy(this ItemObject item, string nameKey)
        {
            ItemObject res = GameObject.Instantiate(item);
            res.nameKey = nameKey;
            res.name = "ItmObj_" + nameKey;
            Item duplicate = item.item.Duplicate();
            res.item = duplicate;
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
            if (texture.IsNull())
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

            samples = samples.ReverseAsArray();

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
        public static T[] RemoveLastN<T>(this T[] array, int n)
        {
            return array.ToList().RemoveLastN(n).ToArray();
        }
        public static List<T> RemoveLastN<T>(this List<T> list, int n)
        {
            if (n <= list.Count)
            {
                list.RemoveRange(list.Count - n, n);
                return list;
            }
            else
            {
                return list;
            }
        }
        public static T[] ReverseAsArray<T>(this T[] array)
        {
            return array.Reverse().ToArray();
        }
        public static void PlayMidiFromData(this MusicManager musicManager, string song, bool loop)
        {
            musicManager.PlayMidi(CachedAssets.Midis[song], loop);
        }
        public static AudioManager AddAudioManager(this GameObject obj)
        {
            AudioManager audman = obj.AddComponent<AudioManager>();
            audman.audioDevice = obj.AddComponent<AudioSource>();
            return audman;
        }
        public static bool DeleteComponent<T>(this GameObject obj) where T : Component
        {
            if (obj.IsNull())
            {
                return false;
            }
            if (obj.GetComponent<T>().IsNull())
            {
                return false;
            }
            UnityEngine.Object.Destroy(obj.GetComponent<T>());
            return true;
        }
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
            if (s.IsNull()) return false;
            return s.Value;
        }
        public static void Teleport(this NPC npc, Vector3 pos)
        {
            if (npc.Character == Character.Cumulo)
            {
                Cumulo cumulo = npc.GetComponent<Cumulo>();
                cumulo.StopBlowing();
                cumulo.windManager.gameObject.SetActive(false);
                cumulo.audMan.FlushQueue(endCurrent: true);
            }
            npc.gameObject.transform.position = pos;
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
        public static void PlaySingle(this AudioManager audMan, string key, bool subtitle = true)
        {
            SoundObject sound = BasePlugin.Instance.asset.Get<SoundObject>(key);
            sound.subtitle = subtitle;
            audMan.PlaySingle(sound);
        }
        public static void QueueAudio(this AudioManager audMan, string key, bool subtitle = true)
        {
            SoundObject sound = BasePlugin.Instance.asset.Get<SoundObject>(key);
            sound.subtitle = subtitle;
            audMan.QueueAudio   (sound);
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
