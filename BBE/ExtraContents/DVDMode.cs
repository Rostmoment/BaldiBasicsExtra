using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace BBE.ExtraContents
{
    class DVDMode : MonoBehaviour
    {
        public int currentY = 1;
        public int currentX = 1;
        public int xModifer = 2;
        public int yModifer = 2;
        public int[] xLimit = new int[] { -500, 1000 };
        public int[] yLimit = new int[] { -200, 400 };
        public IntPtr gameWindow = IntPtr.Zero;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;

        void Start()
        {
            yModifer = new int[]{ -2, 2 }[UnityEngine.Random.Range(0, 1)];
            xModifer = new int[]{ -2, 2 }[UnityEngine.Random.Range(0, 1)];
            currentX = 0;
            currentY = 0;
            gameWindow = FindWindow(null, "Baldi's Basics Plus");
            Singleton<PlayerFileManager>.Instance.resolutionX = 1024;
            Singleton<PlayerFileManager>.Instance.resolutionY = 768;
            Singleton<PlayerFileManager>.Instance.fullscreen = false;
            Singleton<PlayerFileManager>.Instance.UpdateResolution();
        }
        void Update()
        {
            if (gameWindow != IntPtr.Zero)
            {
                if (currentY < yLimit[0] || currentY > yLimit[1])
                {
                    yModifer *= -1;
                }
                if (currentX < xLimit[0] || currentX > xLimit[1])
                {
                    xModifer *= -1;
                }
                currentY += yModifer;
                currentX += xModifer;
                
                SetWindowPos(gameWindow, IntPtr.Zero, currentX, currentY, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
            }
        }
    }
}
