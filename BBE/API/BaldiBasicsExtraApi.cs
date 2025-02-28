using BBE.CustomClasses;
using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.Events;

namespace BBE.API
{
    class BaldiBasicsExtraApi
    {
        /// <summary>
        /// Add cheat code to YCTP
        /// </summary>
        /// <param name="value">Code to active</param>
        /// <param name="toDo">Action when player use cheat code</param>
        public static void AddCheatCodeToYCTP(int value, UnityAction toDo)
        {
            AddCheatCodeToYCTP(value, (x) =>
            {
                toDo();
            });
        }
        /// <summary>
        /// Add cheat code to YCTP
        /// </summary>
        /// <param name="value">Code to active</param>
        /// <param name="toDo">Action when player use cheat code</param>
        public static void AddCheatCodeToYCTP(int value, UnityAction<YCTP> toDo)
        {
            if (!YCTP.yctpCheatCodes.ContainsKey(value)) YCTP.yctpCheatCodes.Add(value, toDo);
        }
    }
}
