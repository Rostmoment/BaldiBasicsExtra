using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MTM101BaldAPI.Registers;
using BBE.Creators;
using System.Collections;
using TMPro;
using BBE.API;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.XR;
using BBE.Extensions;
using MTM101BaldAPI.UI;
using System;

namespace BBE.CustomClasses.FunSettings
{
    public class DVDMode : MonoBehaviour
    {
        private const int WINDOW_HEIGHT = 768;
        private const int WINDOW_WIDTH = 1024;

        private bool white;
        private int currentY = 1;
        private int currentX = 1;
        private int xModifer = 2;
        private int yModifer = 2;
        public List<int> xLimit = new List<int>() { };
        public List<int> yLimit = new List<int>() { };
        private int height;
        private int width;
        private List<int> originalResolution = new List<int> { };
        private bool fullScreen = false;
        public IntPtr gameWindow = IntPtr.Zero;

        public void ShowRandomTextWindow()
        {

        }
        private void SetWindowSize()
        {
            if (originalResolution.Count == 0)
            {
                originalResolution.Add(PlayerFileManager.Instance.resolutionX);
                originalResolution.Add(PlayerFileManager.Instance.resolutionY);
                fullScreen = PlayerFileManager.Instance.fullscreen;
            }
            PlayerFileManager.Instance.resolutionX = WINDOW_WIDTH;
            PlayerFileManager.Instance.resolutionY = WINDOW_HEIGHT;
            PlayerFileManager.Instance.fullscreen = false;
            PlayerFileManager.Instance.UpdateResolution();
        }
        private void Start()
        {
            WindowsAPI.GetScreenSize(out width, out height);
            gameWindow = WindowsAPI.GetWindowByTitleName("Baldi's Basics Plus");
            if (width <= 0 || height <= 0 || gameWindow == IntPtr.Zero)
            {
                WindowsAPI.ShowDialogWindow("Oops", "BBE_DVDSomethingWentWrong".Localize());
                FunSetting.Get(FunSettingsType.DVDMode).Set(false);
                Destroy(this);
            }
            SetWindowSize();
            yModifer = new int[] { -2, 2 }.ChooseRandom();
            xModifer = new int[] { -2, 2 }.ChooseRandom();
            currentX = 0;
            currentY = 0;
            xLimit.AddRange(new int[] { 0, width });
            yLimit.AddRange(new int[] { 0, height });
            white = true;
            WindowsAPI.ChangeTitleBarToWhite(gameWindow);
        }
        private void OnDestroy()
        {
            PlayerFileManager.Instance.resolutionX = originalResolution[0];
            PlayerFileManager.Instance.resolutionY = originalResolution[1];
            PlayerFileManager.Instance.fullscreen = fullScreen;
            PlayerFileManager.Instance.UpdateResolution();
            if (gameWindow != IntPtr.Zero)
            {
                WindowsAPI.SetWindowPosition(gameWindow, 0, 0);
                WindowsAPI.SetWindowTransparent(gameWindow, 255);
            }
        }
        private IEnumerator ShakeCamera()
        {
            GameCamera gameCamera = CoreGameManager.Instance.GetCamera(0);
            List<CameraModifier> modifiers = new List<CameraModifier>();
            for (int i = 0; i<250; i++)
            {
                CameraModifier cameraModifier = new CameraModifier(new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f)), Vector3.zero);
                gameCamera.cameraModifiers.Add(cameraModifier);
                modifiers.Add(cameraModifier);
                yield return null;
            }
            foreach (CameraModifier modifier in modifiers)
            {
                gameCamera.cameraModifiers.Remove(modifier);
                yield return null;
            }
            yield break;
        }
        private void Bump()
        {
            StartCoroutine(ShakeCamera());
            CoreGameManager.Instance?.audMan?.PlaySingle("Bang");
            if (white)
                WindowsAPI.ChangeTitleBarToBlack(gameWindow);
            else
                WindowsAPI.ChangeTitleBarToWhite(gameWindow);
            white = !white;
        }
        private void Update()
        {
            if (PlayerFileManager.Instance.resolutionX != WINDOW_WIDTH || PlayerFileManager.Instance.resolutionY != WINDOW_HEIGHT || PlayerFileManager.Instance.fullscreen)
            {
                WindowsAPI.ShowDialogWindow("WOW!", "BBE_TheSmartest".Localize(), MessageBoxButtons.YesNo);
                SetWindowSize();
            }
            if (gameWindow != IntPtr.Zero)
            {
                if (currentY < yLimit[0] || currentY + WINDOW_HEIGHT > yLimit[1])
                {
                    yModifer *= -1;
                    Bump();
                }
                if (currentX < xLimit[0] || currentX + WINDOW_WIDTH > xLimit[1])
                {
                    xModifer *= -1;
                    Bump();
                }
                currentY += yModifer * (int)Time.timeScale;
                currentX += xModifer * (int)Time.timeScale;
                WindowsAPI.SetWindowPosition(gameWindow, currentX, currentY);
            }
        }
    }
}
