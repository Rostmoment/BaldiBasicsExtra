using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using BBE.Creators;
using TMPro;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System.Linq;
using BBE.Extensions;

namespace BBE.CustomClasses
{
    class TrueGlitchMode : BaseGameManager
    {
        private Fog fog;
        public static TrueGlitchMode instance;
        void OnDestroy()
        {
            instance = null;
        }
        public override void Initialize()
        {
            base.Initialize();
            instance = this;
            fog = new Fog()
            {
                startDist = 2,
                priority = 0,
                strength = 3,
                color = Color.black,
                maxDist = 50
            };
            ec.AddFog(fog);
            ec.npcsToSpawn.Add(NPCMetaStorage.Instance.Get(Character.Baldi).value);
            ec.npcSpawnTiles.Add(ec.allTiles.ChooseRandom());
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
        }
        public override void VirtualUpdate()
        {
            base.VirtualUpdate();
            for (int i = 0; i < ec.map.size.x; i++)
            {
                for (int j = 0; j < ec.map.size.z; j++)
                {
                    if (!ec.cells[i, j].Null)
                    {
                        ec.map.tiles[i, j].Unfound();
                    }
                }
            }
        }
    }
}
