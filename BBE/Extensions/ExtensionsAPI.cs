using BBE.Compats;
using BBE.Creators;
using BBE.Helpers;
using BBE.ModItems;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.UI;
using PlusLevelLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace BBE.Extensions
{
    public static class ExtensionsAPI
    {
        // Builder extensions
        public static ItemBuilder SetMeta(this ItemBuilder itm, ItemFlags flags, params string[] tags)
        {
            itm.flags = flags;
            itm.tags = tags;
            return itm;
        }
        public static ItemBuilder SetLargeSprite(this ItemBuilder itm, Sprite sprite)
        {
            itm.largeSprite = sprite;
            return itm;
        }
        public static ItemBuilder SetSmallSprite(this ItemBuilder itm, Sprite sprite)
        {
            itm.smallSprite = sprite;
            return itm;
        }
        public static ItemBuilder CopyFromItem(this ItemBuilder itm, ItemObject itemObject)
        {
            ItemBuilder res = itm.SetPickupSound(itemObject.audPickupOverride)
                .SetItemComponent(itemObject.item)
                .SetNameAndDescription(itm.localizedText, itm.localizedDescription)
                .SetSprites(itemObject.itemSpriteSmall, itemObject.itemSpriteLarge)
                .SetEnum(itemObject.itemType)
                .SetShopPrice(itemObject.price)
                .SetGeneratorCost(itemObject.value);
            res.instantUse = !itemObject.addToInventory;
            return res;
        }
        public static ItemBuilder SetSprites(this ItemBuilder itm, Sprite toSet) => itm.SetSprites(toSet, toSet);
        public static ItemBuilder SetShopPrice(this ItemBuilder itm) => itm.SetShopPrice(int.MaxValue);
        public static ItemBuilder SetEnum(this ItemBuilder itm, ModdedItems modded) => itm.SetEnum(modded.ToItemsEnum());
        public static ItemObject BuildAndSetup(this ItemBuilder itm)
        {
            ItemObject item = itm.Build();
            if (item.item.gameObject.TryGetComponent<IItemPrefab>(out IItemPrefab prefab))
                prefab.SetupAssets();
            if (item.item is BaseMultipleUseItem converted)
            {
                ItemMetaData meta = item.GetMeta();
                item.nameKey = itm.localizedText + "_" + converted.UsesTotal;
                meta.value.nameKey = itm.localizedText + "_" + converted.UsesTotal;
                ItemObject current = item;
                for (int i = 0; i < converted.UsesTotal; i++)
                {
                    if (i >= converted.UsesTotal - 1)
                        break;
                    ItemObject duplicate = current.Copy(itm.localizedText + "_" + (converted.UsesTotal-1-i).ToString(), meta);
                    ((BaseMultipleUseItem)current.item).next = duplicate;
                    current = duplicate;
                }
                item.GetMeta().itemObjects = item.GetMeta().itemObjects.Reverse().ToArray();
            }
            PlusLevelLoaderPlugin.Instance.itemObjects.Add(item.itemType.ToStringExtended(), item);
            return item;
        }

        public static RandomEventBuilder<E> SetEnum<E>(this RandomEventBuilder<E> reb, ModdedRandomEvent modded) where E : RandomEvent => 
            reb.SetEnum(modded.ToRandomEventEnum());
        public static RandomEventBuilder<E> SetEnumAndName<E>(this RandomEventBuilder<E> reb, ModdedRandomEvent modded) where E : RandomEvent =>
            reb.SetEnum(modded).SetName("ExtraModCustomEvent_" + modded.ToString());

        public static N BuildAndSetup<N>(this NPCBuilder<N> npc) where N : NPC
        {
            N res = npc.Build();
            if (res.gameObject.TryGetComponent<INPCPrefab>(out INPCPrefab prefab))
                prefab.SetupAssets();
            PlusLevelLoaderPlugin.Instance.npcAliases.Add(npc.characterEnum.ToStringExtended(), res);
            return res;
        }
        public static NPCBuilder<N> SetTags<N>(this NPCBuilder<N> npc, params string[] tags) where N : NPC =>
            npc.SetMetaTags(tags);
        public static NPCBuilder<N> SetEnum<N>(this NPCBuilder<N> npc, ModdedCharacters modded) where N : NPC =>
            npc.SetEnum(modded.ToCharacterEnum());
        public static NPCBuilder<N> SetNameAndEnum<N>(this NPCBuilder<N> npc, ModdedCharacters modded) where N : NPC =>
            npc.SetEnum(modded).SetName(modded.ToString());

        // Meta storage extensions
        public static RandomEventMetadata Get(this RandomEventMetaStorage storage, ModdedRandomEvent modded) => storage.Get(modded.ToRandomEventEnum());
        public static NPCMetadata Get(this NPCMetaStorage storage, ModdedCharacters character) => storage.Get(character.ToCharacterEnum());
        public static ItemMetaData FindByEnum(this ItemMetaStorage itm, ModdedItems item) => itm.FindByEnum(item.ToItemsEnum());
        public static ItemMetaData FindByEnum(this ItemMetaStorage itm, string name) => itm.FindByEnum(name.ToEnum<Items>());
        public static void AddRooms(this Character character, params WeightedRoomAsset[] rooms) =>
            NPCMetaStorage.Instance.FindAll(x => x.value.character == character).Do(x => x.prefabs.Do(y => y.Value.potentialRoomAssets = y.Value.potentialRoomAssets.AddRangeToArray(rooms)));
        // Assets extensions
        public static T AddAndReturn<T>(this AssetManager asset, string key, T value)
        {
            asset.Add<T>(key, value);
            return value;
        }
        public static void AddRange<T>(this AssetManager asset, List<T> values, string baseName, int startIndex = 0) =>
            asset.AddRange<T>(values.ToArray(), baseName, startIndex);
        public static void AddRange<T>(this AssetManager asset, T[] values, string baseName, int startIndex = 0)
        {
            for (int i = startIndex; i<values.Length+startIndex; i++)
            {
                asset.Add<T>(baseName+i.ToString(), values[i-startIndex]);
            }
        }
        public static T GetOrAddFromResources<T>(this AssetManager asset, string key, string name = null) where T : UnityEngine.Object
        {
            if (name == null) name = key;
            if (asset.Exists<T>(key)) asset.AddFromResources<T>(key, name);
            return asset.Get<T>(key);
        }
        public static T GetOrAdd<T>(this AssetManager asset, string key, T value)
        {
            if (!asset.Exists<T>(key)) asset.Add(key, value);
            return asset.Get<T>(key);
        }
        public static bool Exists<T>(this AssetManager asset, string name, out T res)
        {
            if (asset.Exists<T>(name)) {
                res = asset.Get<T>(name);
                return true;
            }
            res = default(T);
            return false;
        }
        public static bool Exists(this AssetManager asset, Type type) => asset.data.ContainsKey(type);
        public static bool Exists(this AssetManager asset, Type type, string key)
        {
            if (!asset.Exists(type)) return false;
            return asset.data[type].ContainsKey(key);
        }
        public static bool Exists<T>(this AssetManager asset) => asset.Exists(typeof(T));
        public static bool Exists<T>(this AssetManager asset, string key) => asset.Exists(typeof(T), key);
        public static void AddFromResources<T>(this AssetManager asset, string name, Func<T, bool> predicate) where T : UnityEngine.Object =>
            asset.Add(name, AssetsHelper.LoadAsset(predicate));
        public static void AddFromResources<T>(this AssetManager asset, string resourseName, string name = null) where T : UnityEngine.Object
        {
            if (name == null) name = resourseName;
            asset.Add(name, AssetsHelper.LoadAsset<T>(resourseName));
        }
        // Other
        // I am too lazy to type StandardMenuButton every time
        public static StandardMenuButton ConvertToButton(this GameObject obj, bool autoAssign = true) => obj.ConvertToButton<StandardMenuButton>(autoAssign);
    }
}
