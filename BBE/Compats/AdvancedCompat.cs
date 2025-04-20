using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using BBE.Extensions;
using BBE.Creators;
using MTM101BaldAPI;
using UnityEngine;
using HarmonyLib;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BBE.Helpers;

namespace BBE.Compats
{
    class BSODPlate : BasePlate
    {
    }
    class CorruptedSymbolMachine : MonoBehaviour
    {
        public static List<SymbolMachine> allCorrupted;
        string possibleSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        SymbolMachine machine;
        void Update()
        {
            if (machine != null)
            {
                machine.texts.Do(x => x.text = possibleSymbols.ChooseRandom().ToString());
            }
        }
        public CorruptedSymbolMachine Set(SymbolMachine machine)
        {
            if (allCorrupted == null) allCorrupted = new List<SymbolMachine>();
            this.machine = machine;
            allCorrupted.Add(machine);
            return this;
        }
    }
    [HarmonyPatch]
    class AdvancedPatches
    {
        [HarmonyPatch(typeof(PresentPlate), "SpawnRandomItem")]
        [HarmonyPostfix]
        private static void NoDSODAInPlate(PresentPlate __instance)
        {
            foreach (Pickup pickup in __instance.spawnedPickups)
            {
                if (pickup != null && pickup.item.itemType.Is(ModdedItems.DSODA))
                {
                    ItemObject itm = ItemMetaStorage.Instance.FindAll((ItemMetaData x) => !x.value.itemType.Is(ModdedItems.DSODA) && x.id != 0 && x.flags != ItemFlags.InstantUse).ChooseRandom().value;
                    pickup.item = itm;
                }
            }
        }
        [HarmonyPatch(typeof(SymbolMachine), "Clicked")]
        [HarmonyPrefix]
        private static bool WrongCorruptedSymbolMachine(SymbolMachine __instance, int player)
        {
            if (!__instance.playerIsHolding || __instance.completed || !__instance.spelloons[__instance.playerHolding].trackingPlayer || CorruptedSymbolMachine.allCorrupted.EmptyOrNull())
            {
                return true;
            }
            if (CorruptedSymbolMachine.allCorrupted.Contains(__instance))
            {
                __instance.OnCompleted(false);
                return false;
            }
            return true;
        }
    }
    class AdvancedCompat : BaseCompat
    {
        public override string GUID => "mrsasha5.baldi.basics.plus.advanced";

        public override void Postfix()
        {
            base.Prefix();/*
            AssetsStorage.sprites.Add("adv_balloon_!", AssetsHelper.CreateTexture("Textures", "Other", "BBE_ADV_ExclamtaionMark.png").ToSprite(30f));
            MathMachineNumber mathMachineNumber = GameObject.Instantiate<MathMachineNumber>(AssetsHelper.LoadAsset<MathMachineNumber>("MathNum_0"));
            GameObject.Destroy(mathMachineNumber);
            Spelloon spelloon = mathMachineNumber.gameObject.AddComponent<Spelloon>();
            spelloon.name = "Spelloon_!";
            spelloon.InitializePrefab();
            spelloon.InitializePrefabPost("!");
            spelloon.gameObject.ConvertToPrefab(true);
            ObjectsStorage.Spelloons.Add("spelloon_!", spelloon);*/
            ApiManager.CreateNewSpelloon("!", AssetsHelper.CreateTexture("Textures", "Other", "BBE_ADV_ExclamtaionMark.png").ToSprite(30f));
            ApiManager.AddNewSymbolMachineWords(BasePlugin.Instance.Info, "Extra", "Kulak", "Rost!", "Math!", "OhNo!", "RTMT!");
            for (int i = 1; i<int.MaxValue; i++)
            {
                string key = "BBE_adv_tip_" + i.ToString();
                if (!LocalizationManager.Instance.HasKey(key))
                    break;
                ApiManager.AddNewTips(BasePlugin.Instance.Info, key);
            }

            new FoodRecipeData(BasePlugin.Instance.Info)
                .SetRawFood()
                .RegisterRecipe();
        }  
    }
}
