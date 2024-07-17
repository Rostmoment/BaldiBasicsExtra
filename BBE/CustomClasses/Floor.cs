using BBE.Helpers;
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
        public List<CustomItemData> Items = new List<CustomItemData>();
        public List<CustomEventData> Events = new List<CustomEventData> { };
        public static FloorData Mixed => Get(Floor.Mixed);
        public static void Create()
        {
            foreach (Floor floor in allFloors)
            {
                Create(floor);
            }
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
        public void AddNPC(CustomNPCData data)
        {
            NPCs.Add(data);
        }
        public static void AddNPC(CustomNPCData data, Floor floor)
        {
            foreach (FloorData floorData in floors)
            {
                if (floorData.floor == floor)
                {
                    floorData.AddNPC(data); 
                }
            }
        }
        public void AddItem(CustomItemData data)
        {
            Items.Add(data);
        }
        public static void AddItem(CustomItemData data, Floor floor)
        {
            foreach (FloorData floorData in floors)
            {
                if (floorData.floor == floor)
                {
                    floorData.AddItem(data);
                }
            }
        }
        public void AddEvent(CustomEventData data)
        {
            Events.Add(data);
        }
        public static void AddEvent(CustomEventData data, Floor floor)
        {
            foreach (FloorData floorData in floors)
            {
                if (floorData.floor == floor)
                {
                    floorData.AddEvent(data);
                }
            }
        }
    }
}
