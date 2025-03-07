using BBE.CustomClasses;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Creators
{
    class PostersCreator
    {
        public static void AddPosterToFloors(PosterObject poster, int F1, int F2, int F3, int END)
        {

            if (F1 > 0)
                FloorData.Get("F1").posters.Add(new WeightedPosterObject() { selection = poster, weight = F1 });
            if (F2 > 0)
                FloorData.Get("F2").posters.Add(new WeightedPosterObject() { selection = poster, weight = F1 });
            if (F3 > 0)
                FloorData.Get("F3").posters.Add(new WeightedPosterObject() { selection = poster, weight = F1 });
            if (END > 0)
                FloorData.Get("END").posters.Add(new WeightedPosterObject() { selection = poster, weight = F1 });
        }

        public static void Create()
        {
        }
    }
}
