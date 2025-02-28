using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Patches
{
    [HarmonyPatch(typeof(PlayerMovement), "MouseMove")]
    internal class CameraPatches
    {/*
        private static bool Prefix(PlayerMovement __instance)
        {
            Quaternion rotation = __instance.transform.rotation;
            float horizontal = 0f;
            float vertical = 0f;
            int num2 = 1;

            if (__instance.pm.reversed)
            {
                num2 = -1;
            }

            if (!Singleton<PlayerFileManager>.Instance.authenticMode)
            {
                Singleton<InputManager>.Instance.GetAnalogInput(__instance.cameraAnalogData, out __instance._absoluteVector, out __instance._deltaVector, 0.1f);
            }
            else
            {
                Singleton<InputManager>.Instance.GetAnalogInput(__instance.movementAnalogData, out __instance._absoluteVector, out __instance._deltaVector, 0.1f);
                __instance._deltaVector.x = 0f;
                __instance._deltaVector.y = 0f;
            }

            // Горизонтальное вращение (вокруг оси Y)
            horizontal = __instance._deltaVector.x * num2 * Singleton<PlayerFileManager>.Instance.mouseCameraSensitivity
                         + __instance._absoluteVector.x * Time.deltaTime * Singleton<PlayerFileManager>.Instance.controllerCameraSensitivity * num2;

            // Вертикальное вращение (вокруг оси X)
            vertical = -__instance._deltaVector.y * Singleton<PlayerFileManager>.Instance.mouseCameraSensitivity
                       - __instance._absoluteVector.y * Time.deltaTime * Singleton<PlayerFileManager>.Instance.controllerCameraSensitivity;

            // Применение вращения
            __instance._rotation = Vector3.zero;
            __instance._rotation.y = horizontal * Time.timeScale * __instance.pm.PlayerTimeScale;
            __instance._rotation.x = vertical * Time.timeScale * __instance.pm.PlayerTimeScale;

            rotation.eulerAngles += __instance._rotation;
            __instance.transform.rotation = rotation;

            return false;
        }
        private static Quaternion ClampRotationAroundXAxis(Quaternion q, float min, float max)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1f;
            float num = 114.59156f * Mathf.Atan(q.x);
            num = Mathf.Clamp(num, min, max);
            q.x = Mathf.Tan(0.008726646f * num);
            return q;
        }*/
    }
}
