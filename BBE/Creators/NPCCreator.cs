using BBE.CustomClasses;
using BBE.Extensions;
using BBE.Helpers;
using BBE.NPCs;
using HarmonyLib;
using MTM101BaldAPI.ObjectCreation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Creators
{
    class NPCCreator
    {
        private static void MakeForced(NPC npc, params string[] floors) =>
            floors.Do(x => FloorData.Get(x)?.forcedNPCs.Add(npc));
        private static void AddToFloors(NPC npc, int F1, int F2, int F3, int END)
        {
            if (F1 > 0)
                FloorData.Get("F1").potentialNPCs.Add(new WeightedNPC() { selection = npc, weight = F1 });
            if (F2 > 0)
                FloorData.Get("F2").potentialNPCs.Add(new WeightedNPC() { selection = npc, weight = F1 });
            if (F3 > 0)
                FloorData.Get("F3").potentialNPCs.Add(new WeightedNPC() { selection = npc, weight = F1 });
            if (END > 0)
                FloorData.Get("END").potentialNPCs.Add(new WeightedNPC() { selection = npc, weight = F1 });
        }

        public static void CreateNPCs()
        {
            NPC npc = new NPCBuilder<Kulak>(BasePlugin.Instance.Info)
                .SetNameAndEnum(ModdedCharacters.Kulak)
                .SetMetaName("BBE_Kulak")
                .SetPoster(AssetsHelper.CreateTexture("Textures", "NPCs", "Kulak", "BBE_KulakPoster.png"), "BBE_Kulak", "BBE_Kulak_Desc")
                .AddTrigger()
                .AddHeatmap()
                .BuildAndSetup();
            AddToFloors(npc, 0, 50, 100, 200);

            new NPCBuilder<FuckingSnail>(BasePlugin.Instance.Info)
                .SetNameAndEnum(ModdedCharacters.Snail)
                .SetMetaName("BBE_Snail")
                .SetPoster(AssetsHelper.CreateTexture("Textures", "NPCs", "Snail", "BBE_PosterSnail.png"), "BBE_Snail", "BBE_Snail_Desc")
                .AddTrigger()
                .AddHeatmap()
                .BuildAndSetup();

            npc = new NPCBuilder<Andrey>(BasePlugin.Instance.Info)
                .SetNameAndEnum(ModdedCharacters.Andrey)
                .SetMetaName("BBE_Andrey")
                .SetPoster(AssetsHelper.CreateTexture("Textures", "NPCs", "Andrey", "BBE_AndreyPoster.png"), "BBE_Andrey", "BBE_Andrey_Desc")
                .AddLooker()
                .AddTrigger()
                .AddHeatmap()
                .BuildAndSetup();
            AddToFloors(npc, 0, 0, 150, 250);

            npc = new NPCBuilder<MrPaint>(BasePlugin.Instance.Info)
                .SetNameAndEnum(ModdedCharacters.MrPaint)
                .SetMetaName("BBE_MrPaint")
                .SetPoster(AssetsHelper.CreateTexture("Textures", "NPCs", "MrPaint", "BBE_MrPaintPoster.png"), "BBE_MrPaint", "BBE_MrPaint_Desc")
                .AddLooker()
                .AddTrigger()
                .AddHeatmap()
                .AddSpawnableRoomCategories(RoomCategory.Special)
                .SetTags("faculty")
                .BuildAndSetup();
            AddToFloors(npc, 0, 150, 175, 200);

            npc = new NPCBuilder<Stockfish>(BasePlugin.Instance.Info)
                .SetNameAndEnum(ModdedCharacters.Stockfish)
                .SetMetaName("BBE_Stockfish")
                .SetPoster(AssetsHelper.CreateTexture("Textures", "NPCs", "Stockfish", "BBE_StockfishPoster.png"), "BBE_Stockfish", "BBE_Stockfish_Desc")
                .AddSpawnableRoomCategories(RoomCategory.Special)
                .AddLooker()
                .AddTrigger()
                .AddHeatmap()
                .SetTags("adv_exclusion_hammer_immunity", "faculty")
                .BuildAndSetup();

            AddToFloors(npc, 0, 109, 159, 250);
            
            npc = new NPCBuilder<Tesseract>(BasePlugin.Instance.Info)
                .SetNameAndEnum(ModdedCharacters.Tesseract)
                .SetMetaName("BBE_Tesseract")
                .SetPoster(AssetsHelper.CreateTexture("Textures", "NPCs", "Tesseract", "BBE_TesseractPosterBase.png"), "BBE_Tesseract", "BBE_Tesseract_Desc")
                .AddSpawnableRoomCategories(RoomCategory.Special)
                .AddLooker()
                .AddTrigger()
                .AddHeatmap()
                .SetTags("BBE_NPCGravityDeviceIgnore", "BBE_TesseractIgnoreNPC")
                .BuildAndSetup();

            AddToFloors(npc, 0, 0, 150, 155);
        }
    }
}
