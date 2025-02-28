using BBE.CustomClasses;
using BBE.Creators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using BBE.Helpers;

namespace BBE.Extensions
{
    public static class EnumExtensions
    {
        // Dictionarys for enums
        public static Dictionary<ModdedRandomEvent, RandomEventType> events;
        private static Dictionary<ModdedItems, Items> items;
        private static Dictionary<ModdedCharacters, Character> characters;

        // equals "operator"
        public static bool IsModded(this Character c, out ModdedCharacters res)
        {
            if (c.IsModded())
            {
                res = characters.Where(x => x.Value == c).First().Key;
                return true;
            }
            res = ModdedCharacters.None;
            return false;
        }
        public static bool IsModded(this Character c) => characters.ContainsValue(c);
        public static bool IsModded(this Items itm, out ModdedItems res)
        {
            if (itm.IsModded())
            {
                res = items.Where(x => x.Value == itm).First().Key;
                return true;
            }
            res = ModdedItems.None;
            return false;
        }
        public static bool IsModded(this Items itm) => items.ContainsValue(itm);
        public static bool IsModded(this RandomEventType randomEventType, out ModdedRandomEvent res)
        {
            if (randomEventType.IsModded())
            {
                res = events.Where(x => x.Value == randomEventType).First().Key;
                return true;
            }
            res = ModdedRandomEvent.None;
            return false;
        }
        public static bool IsModded(this RandomEventType randomEventType) => events.ContainsValue(randomEventType);
        public static bool Is(this RandomEventType type, ModdedRandomEvent modded) => modded.ToRandomEventEnum() == type;
        public static bool Is(this ModdedRandomEvent modded, RandomEventType type) => modded.ToRandomEventEnum() == type;
        public static bool Is(this Items item, ModdedItems modded) => modded.ToItemsEnum() == item;
        public static bool Is(this ModdedItems modded, Items item) => modded.ToItemsEnum() == item;
        public static bool Is(this Character character, ModdedCharacters modded) => modded.ToCharacterEnum() == character;
        public static bool Is(this ModdedCharacters modded, Character character) => modded.ToCharacterEnum() == character;

        // To enum methods
        public static T ToEnum<T>(this string text) where T : Enum
        {
            return MTM101BaldAPI.EnumExtensions.ExtendEnum<T>(text);
        }
        public static Character ToCharacterEnum(this ModdedCharacters moddedCharacters)
        {
            if (characters.EmptyOrNull()) characters = new Dictionary<ModdedCharacters, Character>();
            if (!characters.ContainsKey(moddedCharacters))
            {
                characters.Add(moddedCharacters, moddedCharacters.ToString().ToEnum<Character>());
            }
            return characters[moddedCharacters];
        }
        public static Items ToItemsEnum(this ModdedItems moddedItems)
        {
            if (items.EmptyOrNull()) items = new Dictionary<ModdedItems, Items>();
            if (!items.ContainsKey(moddedItems))
            {
                items.Add(moddedItems, moddedItems.ToString().ToEnum<Items>());
            }
            return items[moddedItems];
        }
        public static RandomEventType ToRandomEventEnum(this ModdedRandomEvent modded)
        {
            if (events.EmptyOrNull()) events = new Dictionary<ModdedRandomEvent, RandomEventType>();
            if (!events.ContainsKey(modded))
            {
                events.Add(modded, modded.ToString().ToEnum<RandomEventType>());
            }
            return events[modded];
        }
        public static string ToLower<T>(this T value) where T : Enum => value.ToString().ToLower();
        public static string ToUpper<T>(this T value) where T : Enum => value.ToString().ToUpper();

        // Is none methods
        public static bool IsNone(this RandomEventType r) => r.Is(ModdedRandomEvent.None);
        public static bool IsNone(this ModdedRandomEvent r) => r == ModdedRandomEvent.None;
        public static bool IsNone(this Items i) => i == Items.None;
        public static bool IsNone(this ModdedItems i) => i == ModdedItems.None;
        public static bool IsNone(this Character c) => c == ModdedCharacters.None.ToCharacterEnum();
        public static bool IsNone(this ModdedCharacters c) => c == ModdedCharacters.None;

        // Other
        public static T ParseOrConvertToEnum<T>(this string text, bool ignoreCase = true) where T : Enum
        {
            if (!text.TryParseToEnum<T>(ignoreCase, out T res))
                res = text.ToEnum<T>();
            return res;
        }
        public static T ParseToEnum<T>(this string text, bool ignoreCase = true) where T : Enum
        {
            if (text.TryParseToEnum<T>(ignoreCase, out T res)) return res;
            throw new ArgumentException("No enum with type " + typeof(T).ToString() + " and name " + text);
        }

        public static bool TryParseToEnum<T>(this string text, out T res) where T : Enum => text.TryParseToEnum(true, out res);
        public static bool TryParseToEnum<T>(this string text, bool ignoreCase, out T res) where T : Enum
        {
            if (ignoreCase)
            {
                if (AssetsHelper.All<T>().Where(x => x.ToString().ToLower() == text.ToLower()).Count() > 0)
                {
                    res = AssetsHelper.All<T>().Where(x => x.ToString().ToLower() == text.ToLower()).First();
                    return true;
                }
            }
            else
            {
                if (AssetsHelper.All<T>().Where(x => x.ToString() == text).Count() > 0)
                {
                    res = AssetsHelper.All<T>().Where(x => x.ToString() == text).First();
                    return true;
                }
            }
            res = default(T);
            return false;
        }
        public static T ReadEnum<T>(this BinaryReader reader, bool canBeExtended = false) where T : Enum
        {
            if (!canBeExtended)
                return reader.ReadString().ParseToEnum<T>();   
            string s = reader.ReadString();
            if (s.TryParseToEnum<T>(out T res))
                return res;
            return s.ToEnum<T>();
        }
        public static void Write<T>(this BinaryWriter writer, T t) where T : Enum
        {
            writer.Write(t.ToString());
        }
        public static bool IsActive(this FunSettingsType setting)
        {
            FunSetting s = FunSetting.Get(setting);
            if (s == null) return false;
            return s.Value;
        }
        public static bool IsExtended<T>(this T value) where T : Enum =>
            !IsOriginial(value);
        public static bool IsOriginial<T>(this T value) where T : Enum =>
            AssetsHelper.All<T>().Contains(value);

        public static string ToName(this Layer layer) => LayerMask.LayerToName(layer.ToLayer());
        public static LayerMask ToLayer(this Layer layer)
        {
            return layer switch
            {
                Layer.Window => LayerMask.NameToLayer("Window"),
                Layer.Billboard => LayerMask.NameToLayer("Billboard"),
                Layer.IClickable => LayerMask.NameToLayer("ClickableCollidableEntities"),
                Layer.IgnoreRaycast => LayerMask.NameToLayer("Ignore Raycast"),
                Layer.BlockRaycast => LayerMask.NameToLayer("Block Raycast"),
                Layer.StandardEntities => LayerMask.NameToLayer("StandardEntities"),
                Layer.Map => LayerMask.NameToLayer("Map"),
                Layer.UI => LayerMask.NameToLayer("UI"),
                Layer.GumCollision => 2113537,
                Layer.EntityCollision => 2113541,
                Layer.PrincipalLooker => 2326529,
                _ => Layer.Window.ToLayer(),
            };
        }
    }
}
