using BBE.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.CustomClasses
{
    public class FloorData
    {
        public static List<FloorData> floors = new List<FloorData>();
        public static List<Floor> allFloors = new List<Floor>() { Floor.Floor1, Floor.Floor2, Floor.Floor3, Floor.Endless, Floor.Challenge, Floor.Mixed, Floor.None };
        public Floor floor;
        public List<CustomNPCData> NPCs = new List<CustomNPCData>();
        public List<CustomBuilderData> Builders = new List<CustomBuilderData>();
        public List<CustomItemData> Items = new List<CustomItemData>();
        public List<CustomRoomData> Rooms = new List<CustomRoomData>();
        public List<WeightedRandomEvent> Events = new List<WeightedRandomEvent> { };
        public List<WeightedPosterObject> Posters = new List<WeightedPosterObject>() { };
        public static FloorData Mixed => Get(Floor.Mixed);

        public static void Create()
        {
            allFloors.Do(floor => Create(floor));
        }
        public static FloorData Create(Floor floor)
        {
            FloorData data = new FloorData();
            data.floor = floor;
            floors.Add(data);
            return data;
        }
        public static FloorData Get(Floor floor)
        {
            return floors.Find(x => x.floor == floor);
        }
        public void AddBuilder(CustomBuilderData data)
        {
            Builders.Add(data);
        }
        public static void AddBuilder(CustomBuilderData data, Floor floor)
        {
            Get(floor).AddBuilder(data);
        }
        public void AddNPC(CustomNPCData data)
        {
            NPCs.Add(data);
        }
        public static void AddNPC(CustomNPCData data, Floor floor)
        {
            Get(floor).AddNPC(data);
        }
        public void AddItem(CustomItemData data)
        {
            Items.Add(data);
        }
        public static void AddItem(CustomItemData data, Floor floor)
        {
            Get(floor).AddItem(data);
        }
        public void AddEvent(WeightedRandomEvent data)
        {
            Events.Add(data);
        }
        public static void AddEvent(WeightedRandomEvent data, Floor floor)
        {
            Get(floor).AddEvent(data);
        }
        public void AddRoom(CustomRoomData asset)
        {
            Rooms.Add(asset);
        }
        public static void AddRoom(CustomRoomData asset, Floor floor)
        {
            Get(floor).AddRoom(asset);
        }
        public void AddPoster(WeightedPosterObject obj)
        {
            Posters.Add(obj);
        }
        public static void AddPoster(WeightedPosterObject asset, Floor floor)
        {
            Get(floor).AddPoster(asset);
        }
    }
}
