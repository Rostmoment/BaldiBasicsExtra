using BaldisBasicsPlusAdvanced.Game.Spawning;
using BBE.Extensions;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BBE.CustomClasses
{
    public class FloorData
    {
        private static List<FloorData> floors = new List<FloorData>();
        public static List<FloorData> All => floors;
        public FloorData(string n)
        {
            name = n;
            if (floors.EmptyOrNull())
                floors = new List<FloorData>();

            shopItems = new List<WeightedItemObject>();
            potentialItems = new List<WeightedItemObject>();
            partyEventItems = new List<WeightedItemObject>();
            forcedItems = new List<ItemObject>();

            potentialNPCs = new List<WeightedNPC>();
            forcedNPCs = new List<NPC>();

            randomEvent = new List<WeightedRandomEvent>();

            specialRooms = new List<WeightedRoomAsset>();
            roomGroups = new List<RoomGroup>();

            customSwingDoors = new List<WeightedGameObject>();

            forcedStructures = new List<StructureWithParameters>();

            posters = new List<WeightedPosterObject>();

            floors.Add(this);
        }
        public void RemoveDisabled()
        {
            forcedNPCs.RemoveAll(x => BBEConfigs.DisabledNPC.Contains(x.GetMeta().character.ToStringExtended()));
            potentialNPCs.RemoveAll(x => BBEConfigs.DisabledNPC.Contains(x.selection.GetMeta().character.ToStringExtended()));
            forcedItems.RemoveAll(x => BBEConfigs.DisabledItems.Contains(x.nameKey));
            potentialItems.RemoveAll(x => BBEConfigs.DisabledItems.Contains(x.selection.nameKey));
        }

        public string name;
        public static FloorData Get(string name) => floors.Find(x => x.name == name);

        public List<WeightedItemObject> shopItems;
        public List<WeightedItemObject> potentialItems;
        public List<WeightedItemObject> partyEventItems;
        public List<ItemObject> forcedItems;

        public List<WeightedNPC> potentialNPCs;
        public List<NPC> forcedNPCs;

        public List<WeightedRandomEvent> randomEvent;

        public List<WeightedRoomAsset> specialRooms;
        public List<RoomGroup> roomGroups;

        public List<WeightedGameObject> customSwingDoors;

        public List<StructureWithParameters> forcedStructures;

        public List<WeightedPosterObject> posters;
    }
}
