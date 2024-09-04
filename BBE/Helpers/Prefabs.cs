using BBE.CustomClasses;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Helpers
{
    internal class Prefabs
    {
        public static GenericHallBuilder HallBuilder
        {
            get
            {
                if (TrueHallBuilder.IsNull()) TrueHallBuilder = AssetsHelper.LoadAsset<GenericHallBuilder>("ZestyHallBuilder");
                return TrueHallBuilder;
            }
            set
            {
                TrueHallBuilder = value;
            }
        }
        public static SodaMachine SodaMachine
        {
            get
            {
                if (TrueSodaMachine.IsNull()) TrueSodaMachine = AssetsHelper.LoadAsset<SodaMachine>("ZestyMachine");
                return TrueSodaMachine;
            }
            set
            {
                TrueSodaMachine = value;
            }
        }
        public static TrueGlitchMode GlitchMode
        {
            get
            {
                if (TrueGlitchMode.IsNull())
                {
                    GameObject game = new GameObject("BBE_Glitch");
                    game.ConvertToPrefab(false);
                    TrueGlitchMode = game.AddComponent<TrueGlitchMode>();
                    TrueGlitchMode.destroyOnLoad = true;
                    TrueGlitchMode.elevatorScreenPre = AssetsHelper.Find<ElevatorScreen>();
                }
                return TrueGlitchMode;
            }
            set
            {
                TrueGlitchMode = value;
            }
        }
        public static MenuToggle MenuToggle
        {
            get
            {
                if (TrueMenuToggle.IsNull()) TrueMenuToggle = AssetsHelper.Find<MenuToggle>();
                return TrueMenuToggle;
            }
            set
            {
                TrueMenuToggle = value;
            }
        }
        public static MapIcon MapIcon
        {
            get
            {
                if (TrueMapIcon.IsNull()) TrueMapIcon = AssetsHelper.Find<Notebook>().iconPre;
                return TrueMapIcon;
            }
            set
            {
                TrueMapIcon = value;
            }
        }
        public static Material MapMaterial
        {
            get
            {
                if (TrueMapMaterial.IsNull()) return null;
                return TrueMapMaterial;
            }
            set
            {
                TrueMapMaterial = value;
            }
        }
        public static GameObject Fire
        {
            get
            {
                if (TrueFire.IsNull())
                {
                    TrueFire = new GameObject("FirePrefab");
                    UnityEngine.Object.DontDestroyOnLoad(TrueFire);
                }
                return TrueFire;
            }
            set
            {
                TrueFire = value;
            }
        }
        public static GameObject Canvas
        {
            get
            {
                if (TrueCanvas.IsNull()) TrueCanvas = AssetsHelper.LoadAsset<GameObject>("GumOverlay");
                return TrueCanvas;
            }
            set
            {
                TrueCanvas = value;
            }
        }
        private static GenericHallBuilder TrueHallBuilder;
        private static SodaMachine TrueSodaMachine;
        private static TrueGlitchMode TrueGlitchMode;
        private static MenuToggle TrueMenuToggle;
        private static MapIcon TrueMapIcon;
        private static Material TrueMapMaterial;
        private static GameObject TrueFire;
        private static GameObject TrueCanvas;
    }
}
