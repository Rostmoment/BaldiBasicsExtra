using BBE.Events.HookChaos;
using BBE.Events;
using BBE.Helpers;
using BBE.ModItems;
using HarmonyLib;

namespace BBE.ExtraContents
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class InitializeDataInBaseGameManager
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        private static void Initialize()
        {
            AssetsHelper.InitializeData();
        }
    }

    [HarmonyPatch(typeof(LevelGenerator))]
    internal class InitializeDataInLevelGenerator
    {
        [HarmonyPatch("StartGenerate")]
        [HarmonyPrefix]
        private static void Initialize()
        {
            AssetsHelper.InitializeData();
        }
    }

    [HarmonyPatch(typeof(MainMenu))]
    internal class InitializeMod
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void Initialize()
        {
            AssetsHelper.InitializeData();
            if (Prefabs.MapIcon == null)
            {
                Prefabs.MapIcon = AssetsHelper.Find<Notebook>().iconPre;
            }
            CreateObjects.CreateItem<ITM_Calculator>("Item_Calculator_Desk", "Item_Calculator", "Calculator", 60, "Calculator.png", 60, 50, pixelsPerUnitLargeTexture: 400f, ItemCanSpawnInFieldTrip: true);
            // CreateObjects.CreateItem<ITM_GravityDevice>("Gravity Device\nIdea by @Kostya_Karuselkin2735", "Gravity Device", "GravityDevice", 60, "Placeholder.jpg", 60, 50);
            CreateObjects.CreateItem<ITM_SpeedPotions>("Item_Potion_Desk", "Item_Potion", "ReductionPotions", 60, "Potion.png", 60, 50, ItemCanSpawnInFieldTrip: true, pixelsPerUnitLargeTexture: 400f);
            CreateObjects.CreateItem<ITM_Shield>("Item_Shield_Desk", "Item_Shield", "Shield", 60, "Shield.png", 60, 150, ItemCanSpawnInMysteryRoom: true, ItemCanSpawnInRooms: false, pixelsPerUnitLargeTexture: 150f);
            CreateObjects.CreateEvent<TeleportationChaosEvent>("TeleportationChaos", 60, Floor.Floor2, Floor.Floor3);
            CreateObjects.CreateEvent<SoundEvent>("SoundEvent", 45, Floor.Floor1, Floor.Floor2);
            CreateObjects.CreateEvent<HookChaosEvent>("HookChaos", 60, Floor.Floor2, Floor.Floor3, Floor.Endless);
            CreateObjects.CreateEvent<ElectricityEvent>("ElectricityEvent", 30, Floor.Floor2, Floor.Floor3, Floor.Endless);
        }
    }
}
