using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using BBE.Helpers;
using TMPro;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System.Linq;

namespace BBE.CustomClasses
{
    class CorruptedSymbolMachine : MonoBehaviour
    {
        string possibleSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        SymbolMachine machine;
        void Update()
        {
            if (!machine.IsNull()) machine.texts.Do(x => x.text = possibleSymbols.ToCharArray().ChooseRandom().ToString());
        }
        public CorruptedSymbolMachine Set(SymbolMachine machine)
        {
            this.machine = machine;
            return this;
        }
    }
    class TrueGlitchMode : BaseGameManager
    {
        public static LevelObject newLevelObject;
        public static SceneObject newSceneObject;
        public static void CreateLevel()
        {
            LevelObject levelObjectRef = AssetsHelper.LoadAsset<LevelObject>(x => x.name == "Main3");
            GameObject newManager = new GameObject("BBE_TrueGlitch");
            newManager.ConvertToPrefab(true);
            Prefabs.GlitchMode = newManager.AddComponent<TrueGlitchMode>();
            Prefabs.GlitchMode.destroyOnLoad = true;
            Prefabs.GlitchMode.elevatorScreenPre = AssetsHelper.Find<ElevatorScreen>();
            newLevelObject = Instantiate(levelObjectRef);
            newLevelObject.name = "Rost";
            newLevelObject.maxClassRooms = 11;
            newLevelObject.minClassRooms = 11;
            newLevelObject.exitCount = 7;
            newLevelObject.randomEvents = new List<WeightedRandomEvent> { };
            newLevelObject.minEvents = 0;
            newLevelObject.maxEvents = 0;
            newSceneObject = Instantiate(AssetsHelper.LoadAsset<SceneObject>(x => x.name == "MainLevel_3"));
            newSceneObject.manager = Prefabs.GlitchMode;
            newSceneObject.levelObject = newLevelObject;
            newSceneObject.levelTitle = "Rost";
            newSceneObject.name = "Rost";
        }
        public static TrueGlitchMode instance;
        void OnDestroy()
        {
            instance = null;
        }
        public override void Initialize()
        {
            base.Initialize();
            instance = this;
        }
        public override void ExitedSpawn()
        {
            base.ExitedSpawn();
            BeginSpoopMode();
        }
        public override void BeginPlay()
        {
            base.BeginPlay();
            ec.activities.Do(x => x.corrupted = true); // Every math machine is impossible to solve
            if (ModIntegration.AdvancedInstalled) CorruptSymbolMachines();
        }
        public override void Update()
        {
            base.Update();
            //ec.map.tiles.ConvertTo1d(ec.map.size.x, ec.map.size.z).Do(x => x.gameObject.SetActive(false)); // Disable map
        }
        public void CorruptSymbolMachines()
        { // All symbol machines are corrupted
            foreach (SymbolMachine symbolMachine in Object.FindObjectsOfType<SymbolMachine>())
            {
                symbolMachine.gameObject.AddComponent<CorruptedSymbolMachine>().Set(symbolMachine);
                symbolMachine.SetFaceTex("CanNotHide");
                StartCoroutine(symbolMachine.SpelloonPopper());
            }
        }
    }
}
