using UnityEngine;
using System.Linq;
using System.Collections.Generic;
namespace BBE.Helpers
{
    class Variables
    {
        //Variables for generator
        public static WeightedItemObject[] MysteryRoomItems = { };
        public static WeightedItemObject[] RoomItems = { };
        public static WeightedItemObject[] ShopItems = { };
        public static WeightedItemObject[] PartyItems = { };
        public static List<WeightedItem> FieldTripItems = new List<WeightedItem>();
        //Other
        public static bool AngryBaldi = false;
        public static Floor CurrentFloor;
    }
}
