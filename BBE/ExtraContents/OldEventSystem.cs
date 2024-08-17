using BBE.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace BBE.ExtraContents
{
    // NOT USED
    class OldEventSystem : MonoBehaviour
    {
        public static OldEventSystem Instance;
        private static GameObject canvas;
        private static TMP_Text TMPText;
        void Start()
        {
           CreateInstance();
        }
        public static void CreateInstance()
        {
            Instance = new GameObject("OldEventSystem").AddComponent<OldEventSystem>();
        }
        public static void ShowTextStatic(string text, float time = 5)
        {
            try { Instance.ShowText(text, time); }
            catch { CreateInstance(); Instance.ShowText(text, time); }
        }
        public void ShowText(string text, float time = 5)
        {
            if (TMPText.IsNull() || canvas.IsNull()) CreateCanvas();
            TMPText.text = text;
            canvas.SetActive(true);
            StartCoroutine(Timer(time));
        }
        public static IEnumerator Timer(float time)
        {
            float timeLeft = time;
            while (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                yield return null;
            }
            canvas.SetActive(false);
        }
        public static void CreateCanvas()
        {
            if (canvas.IsNull()) canvas = CreateObjects.CreateCanvas("OldNotificationSystem", false, color: new Color(0, 0, 0, 0.5f));
            if (TMPText.IsNull()) TMPText = CreateObjects.CreateText("EventText", "", false, new Vector3(-10, -130), Vector3.one, canvas.transform, 24);
        }
    }
}
