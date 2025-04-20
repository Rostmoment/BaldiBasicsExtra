using BBE.Creators;
using BBE.Helpers;
using BBE.Structures;
using MTM101BaldAPI;
using PlusLevelLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Compats
{
    class LevelLoaderCompat : BaseCompat
    {
        public LevelLoaderCompat(bool forced) : base(forced) { }
        public override string GUID => "mtm101.rulerp.baldiplus.levelloader";
        private static List<string> markers = new List<string>
            {
                "nonSafeCellMarker",
                "potentialDoorMarker",
                "forcedDoorMarker",
                "lightSpotMarker",
                "itemSpawnMarker"
            };
        private static void AddMissingMarker(string name)
        {
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add(name, new GameObject(name));
            PlusLevelLoaderPlugin.Instance.prefabAliases[name].ConvertToPrefab(true);
        }
        public override void Prefix()
        {
            foreach (string marker in markers)
            {
                if (!PlusLevelLoaderPlugin.Instance.prefabAliases.ContainsKey(marker))
                    AddMissingMarker(marker);
            }
            if (!PlusLevelLoaderPlugin.Instance.textureAliases.ContainsKey("SaloonWallEditor"))
                PlusLevelLoaderPlugin.Instance.textureAliases.Add("SaloonWallEditor", AssetsHelper.LoadAsset<Texture2D>("SaloonWall"));
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("JohnMusclesGymFloor", AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_JohnMusclesGymFloor.png"));
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("JohnMusclesGymWall", AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_JohnMusclesGymWall.png"));
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("BBEChessClassFloor", AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_StockfishRoomFloor.png"));
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("BBEGreenSaloon", AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_GreenSaloon.png"));
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("BBEOldLibraryWall", AssetsHelper.CreateTexture("Textures", "Rooms", "BBE_OldLibraryWall.png"));
            base.Prefix();
        }
        public override void Postfix()
        {
            base.Postfix();
            PlusLevelLoaderPlugin.Instance.doorPrefabs.Add("YTPDoor", YTPDoor.Create().GetComponent<YTPDoor>().SwingDoor);
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("StrawberryZestyBarMachine", CreateObjects.CreateVendingMachine("StrawberryZestyBarMachine", null, Items.Map).gameObject);
        }
    }
}
