using HarmonyLib;
using MidiPlayerTK;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;

namespace BBE.Helpers
{
    internal class Math
    {
        private static float GetValueFromFloatOrInt(float value)
        {
            return value;
        }
        public static float[] FromFloat(params float[] value) {
        float[]           res                           = 
        new                                             float
        [value                                          .Length];
        for (                                        int index
        = 0;                                         index < 
        value                             .          Length;
        index++) {                       res.           AddItem(
        GetValueFromFloatOrInt(value[index]));} return res;
        }
    }
}
