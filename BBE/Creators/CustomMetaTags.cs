using BBE.Extensions;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Creators
{
    class CustomMetaTags
    {
        public static void AddTags()
        {
            AddItemsMetaTags();
            AddCharactersTags();
        }
        private static void AddCharactersTags()
        {
            NPCMetaStorage.Instance.Get(Character.Principal).tags.Add("BBE_KulakIgnoreCharacter");   
        }
        private static void AddItemsMetaTags()
        {
            ItemMetaStorage.Instance.FindByEnum(Items.Wd40).tags.Add("BBE_RNGLibraryItem");
            ItemMetaStorage.Instance.FindByEnum(Items.Scissors).tags.Add("BBE_RNGLibraryItem");
            ItemMetaStorage.Instance.FindByEnum(Items.Nametag).tags.Add("BBE_RNGLibraryItem");

            ItemMetaStorage.Instance.FindByEnum(Items.Bsoda).tags.Add("BBE_StockfishReward3");
            ItemMetaStorage.Instance.FindByEnum(Items.DietBsoda).tags.Add("BBE_StockfishReward1");
            ItemMetaStorage.Instance.FindByEnum(Items.ZestyBar).tags.Add("BBE_StockfishReward1");
        }
    }
}
