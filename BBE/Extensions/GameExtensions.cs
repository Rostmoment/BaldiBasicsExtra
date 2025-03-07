using BBE.CustomClasses;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BBE.Extensions
{
    public static class GameExtensions
    {

        public static T AddFunction<T>(this RoomFunctionContainer room) where T : RoomFunction
        {
            if (room.functions == null)
                room.functions = new List<RoomFunction>();
            T res = room.gameObject.GetOrAddComponent<T>();
            if (!room.functions.Contains(res))
                room.AddFunction(res);
            return res;
        }
        public static T AddFunction<T>(this RoomAsset room) where T : RoomFunction =>
            room.roomFunctionContainer.AddFunction<T>();

        public static MovementModifier Copy(this MovementModifier movementModifier)
        {
            MovementModifier result = new MovementModifier(movementModifier.movementAddend, movementModifier.movementMultiplier, movementModifier.priority);
            result.ignoreAirborne = movementModifier.ignoreAirborne;
            result.ignoreGrounded = movementModifier.ignoreGrounded;
            result.forceTrigger = movementModifier.forceTrigger;
            return result;
        }
        public static void AddKey(this LocalizationManager localization, string key, string value, bool overrideKey = false)
        {
            if (!localization.HasKey(key) || overrideKey)
            {
                localization.localizedText[key] = value;
            }
        }
        public static bool HasKey(this LocalizationManager localization, string key) => localization.localizedText.ContainsKey(key);

        public static void SetProblemsCount(this MathMachine math, int total) => math.SetProblemsCount(total, math.answeredProblems);
        public static void SetProblemsCount(this MathMachine math, int total, int solved)
        {
            math.totalProblems = total;
            if (total > math.currentNumbers.Where(x => x.Available).Count())
                math.totalProblems = math.currentNumbers.Where(x => x.Available).Count();
            math.answeredProblems = solved;
            if (solved >= math.totalProblems)
                math.answeredProblems = math.totalProblems - 1;
            if (math.totalProblems > 1)
            {
                string text = "";
                foreach (char c in math.answeredProblems.ToString())
                {
                    text += $"<sprite={c}>";
                }
                text += "<sprite=10>";
                foreach (char c in math.totalProblems.ToString())
                {
                    text += $"<sprite={c}>";
                }

                math.totalTmp.text = text;
                math.totalTmp.enableAutoSizing = true;
                math.totalTmp.gameObject.SetActive(value: true);
            }
        }

        public static void Destroy(this MapIcon icon, Map map = null)
        {
            icon.gameObject.SetActive(false);
            map?.icons.RemoveIfContains(icon);
            Destroy(icon);
        }
        public static void Unfound(this MapTile tile)
        {
            tile.gameObject.SetActive(false);
            tile.found = false;
        }

        public static void SetSelectedSlot(this ItemManager itm, ItemObject itemObject) => itm.SetItem(itemObject, itm.selectedItem);
        public static void RemoveSelectedItem(this ItemManager itm) => itm.RemoveItem(itm.selectedItem);
        public static ItemObject GetSelectedItem(this ItemManager itm) => itm.items[itm.selectedItem];


        public static Entity CreateEntity(this GameObject target, float colliderRadius, float triggerColliderRadius = 0f, Transform rendererBase = null) =>
            target.CreateEntity(colliderRadius, triggerColliderRadius, out _, out _, rendererBase);
        public static Entity CreateEntity(this GameObject target, float colliderRadius, float triggerColliderRadius, out CapsuleCollider nonTriggerCollider, out CapsuleCollider triggerCollider, Transform rendererBase = null)
        {
            CapsuleCollider collider = target.AddComponent<CapsuleCollider>();
            collider.radius = colliderRadius;
            nonTriggerCollider = collider;
            CapsuleCollider trigger = target.AddComponent<CapsuleCollider>();
            trigger.isTrigger = true;
            if (triggerColliderRadius <= 0f) trigger.enabled = false;
            else trigger.radius = triggerColliderRadius;
            triggerCollider = trigger;
            return target.CreateEntity(collider, trigger, rendererBase);
        }

        public static Pickup ToPickup(this ItemObject item) => item.ToPickup(CoreGameManager.Instance.GetPlayer(0).transform.position);
        public static Pickup ToPickup(this ItemObject item, Vector3 position)
        {
            Pickup pickup = UnityEngine.Object.Instantiate(BaseGameManager.Instance.ec.pickupPre);
            pickup.item = item;
            pickup.transform.position = position;
            return pickup;
        }

        public static SpriteRenderer AddSpriteRender(this GameObject obj, string sprite) => obj.AddSpriteRender(BasePlugin.Asset.Get<Sprite>(sprite));
        public static SpriteRenderer AddSpriteRender(this GameObject obj, Sprite sprite)
        {
            SpriteRenderer res = obj.AddComponent<SpriteRenderer>();
            res.sprite = sprite;
            return res;
        }


        public static void RemoveItems(this ItemManager itm, ModdedItems toRemove) => itm.RemoveItems(toRemove.ToItemsEnum());
        public static void RemoveItems(this ItemManager itm, Items toRemove)
        {
            for (int i = 0; i < itm.items.Length; i++)
            {
                if (itm.items[i].itemType == toRemove)
                {
                    CoreGameManager.Instance.GetHud(0).inventory.LoseItem(i, itm.items[i]);
                    itm.items[i] = itm.nothing;
                    CoreGameManager.Instance.GetHud(0).UpdateItemIcon(i, itm.nothing.itemSpriteSmall);
                }
            }
        }
        public static void EndGame(this CoreGameManager instance, Transform player, Transform targerPosition) =>
            instance.EndGame(player, targerPosition, ((Baldi)NPCMetaStorage.Instance.Get(Character.Baldi).value).loseSounds);
        public static void EndGame(this CoreGameManager instance, Transform player, Transform targetPosition, WeightedSoundObject[] loseSounds)
        {
            Time.timeScale = 0f;
            MusicManager.Instance.StopMidi();
            instance.disablePause = true;
            instance.GetCamera(0).UpdateTargets(targetPosition, 0);
            instance.GetCamera(0).offestPos = (player.position - targetPosition.position).normalized * 2f + Vector3.up;
            instance.GetCamera(0).SetControllable(value: false);
            instance.GetCamera(0).matchTargetRotation = false;
            instance.audMan.volumeModifier = 0.6f;
            if (!loseSounds.EmptyOrNull()) instance.audMan.PlaySingle(WeightedSelection<SoundObject>.RandomSelection(loseSounds));
            instance.StartCoroutine(instance.EndSequence());
            InputManager.Instance.Rumble(1f, 2f);

        }
        public static ItemObject Copy(this ItemObject item, ItemMetaData meta) => item.Copy(item.nameKey, meta);
        public static ItemObject Copy(this ItemObject item) => item.Copy(item.nameKey);
        public static ItemObject Copy(this ItemObject item, string nameKey)
        {
            ItemObject res = UnityEngine.Object.Instantiate(item);
            res.nameKey = nameKey;
            res.name = "ItmObj_" + nameKey;
            Item duplicate = item.item.Duplicate();
            res.item = duplicate;
            return res;
        }
        public static ItemObject Copy(this ItemObject item, string nameKey, ItemMetaData meta)
        {
            ItemObject res = item.Copy(nameKey);
            if (meta.itemObjects.EmptyOrNull())
                meta.itemObjects = new ItemObject[] { };
            meta.itemObjects = meta.itemObjects.AddToArray(res);
            res.AddMeta(meta);
            return res;
        }

        public static AudioManager AddAudioManager(this GameObject obj)
        {
            AudioManager audman = obj.GetOrAddComponent<AudioManager>();
            audman.audioDevice = obj.GetOrAddComponent<AudioSource>();
            return audman;
        }
        public static PropagatedAudioManager AddPropagatedAudioManager(this GameObject obj, float min = 25f, float max = 50f)
        {
            PropagatedAudioManager res = obj.AddComponent<PropagatedAudioManager>();
            res.minDistance = min;
            res.maxDistance = max;
            return res;
        }

        public static void PlaySingle(this AudioManager audMan, string key, bool subtitle = true)
        {
            SoundObject sound = BasePlugin.Asset.Get<SoundObject>(key);
            sound.subtitle = subtitle;
            audMan.PlaySingle(sound);
        }
        public static void QueueAudio(this AudioManager audMan, string key, bool subtitle = true)
        {
            SoundObject sound = BasePlugin.Asset.Get<SoundObject>(key);
            sound.subtitle = subtitle;
            audMan.QueueAudio(sound);
        }
    }
}
