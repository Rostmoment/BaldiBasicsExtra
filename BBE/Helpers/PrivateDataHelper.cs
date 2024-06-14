using AlmostEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BBE.Helpers
{
    class PrivateDataHelper
    {
        public static T UseMethod<T>(object instance, string methodName, params object[] parameters)
        {
            return Traverse.Create(instance).Method(methodName, parameters).GetValue<T>();
        }
        public static void UseMethod(object instance, string methodName, params object[] parameters)
        {
            Traverse.Create(instance).Method(methodName, parameters).GetValue();
        }
        public static T GetVariable<T>(object instance, string fieldName)
        {
            return Traverse.Create(instance).Field(fieldName).GetValue<T>();
        }
        public static void SetValue(object instance, string fieldName, object setVal)
        {
            Traverse.Create(instance).Field(fieldName).SetValue(setVal);
        }
    }
}
