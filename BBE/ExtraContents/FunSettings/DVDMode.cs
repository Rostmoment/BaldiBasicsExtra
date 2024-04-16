using BBE.Helpers;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using UnityEngine;
using HarmonyLib;
namespace BBE.ExtraContents.FunSettings
{
    internal class DVDMode
    {
        [HarmonyPatch(typeof(BaseGameManager))]
        internal class AddDVDEffect
        {
            [HarmonyPatch("Update")]
            [HarmonyPrefix]
            private static void AddEffect(BaseGameManager __instance)
            {
                if (!IsActive && FunSettingsManager.DVDMode)
                {
                    AudioManager audioManager = PrivateDataHelper.GetVariable<AudioManager>(__instance.Ec, "audMan");
                    __instance.StartCoroutine(MoveWindow(audioManager));
                }
            }
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        private static IEnumerator MoveWindow(AudioManager audMan)
        {
            IsActive = true;
            Singleton<PlayerFileManager>.Instance.resolutionX = 1024;
            Singleton<PlayerFileManager>.Instance.resolutionY = 768;
            Singleton<PlayerFileManager>.Instance.fullscreen = false;
            Singleton<PlayerFileManager>.Instance.UpdateResolution();
            while (true)
            {
                int width = Screen.width;
                int height = Screen.height;
                IntPtr window = FindWindow(null, "Baldi's Basics Plus");
                if (window != IntPtr.Zero)
                {
                    if (y == height || y == -height)
                    {
                        Singleton<PlayerFileManager>.Instance.resolutionX = 1024;
                        Singleton<PlayerFileManager>.Instance.resolutionY = 768;
                        Singleton<PlayerFileManager>.Instance.fullscreen = false;
                        Singleton<PlayerFileManager>.Instance.UpdateResolution();
                        audMan.PlaySingle(AssetsHelper.CreateSoundObject(Bang, SoundType.Effect, Subtitle: false));
                        yModifer *= -1;
                    }
                    if (x == width || x == -width)
                    {
                        Singleton<PlayerFileManager>.Instance.resolutionX = 1024;
                        Singleton<PlayerFileManager>.Instance.resolutionY = 768;
                        Singleton<PlayerFileManager>.Instance.fullscreen = false;
                        Singleton<PlayerFileManager>.Instance.UpdateResolution();
                        audMan.PlaySingle(AssetsHelper.CreateSoundObject(Bang, SoundType.Effect, Subtitle: false));
                        xModifer *= -1;
                    }
                    x += speed * xModifer;
                    y += speed * yModifer;
                    SetWindowPos(window, IntPtr.Zero, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                }
                yield return null;
            }
        }
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOZORDER = 0x0004;
        private static AudioClip Bang = AssetsHelper.LoadAsset<AudioClip>("Bang");
        public static bool IsActive;
        private static int y = 0;
        private static int x = 0;
        private static int yTemp = -1;
        private static int xTemp = 1;
        private static int speed = 2;
        private static int yModifer = 1;
        private static int xModifer = 1;
    }
}
