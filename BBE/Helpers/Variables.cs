using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Helpers
{
    internal class Variables
    {
        public static WeightedItemObject[] MysteryRoomItems = { };
        public static WeightedItemObject[] RoomItems = { };
        public static WeightedItemObject[] ShopItems = { };
        public static WeightedItemObject[] PartyItems = { };
        public static List<WeightedItem> FieldTripItems = new List<WeightedItem>();
        public static Floor CurrentFloor;
    }
}
