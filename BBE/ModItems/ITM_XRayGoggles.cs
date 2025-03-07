using BBE.Extensions;
using BBE.Creators;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BBE.ModItems
{
    class ITM_XRayGoggles : Item
    {
        private EnvironmentController ec;
        private Dictionary<Fog, float> fogs = new Dictionary<Fog, float>();
        private static GameObject canvas;
        private static float WorkingTime => 30f;
        private static float CanvasAlphaMax => 0.25f;
        public override bool Use(PlayerManager pm)
        {
            if (canvas != null) return false;
            fogs.Clear();
            ec = pm.ec;
            ec.fogs.Do(x => fogs.Add(x, x.strength));
            StartCoroutine(Timer(WorkingTime));
            return true;
        }
        private IEnumerator Timer(float time)
        {
            yield return StartCoroutine(Use());
            float timeLeft = time;
            while (timeLeft > 0) 
            {
                timeLeft -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }
            yield return StartCoroutine(Disable());
            DestroyCanvas();
            Destroy(gameObject);
            yield break;
        }
        private IEnumerator Use()
        {
            canvas = CreateObjects.CreateCanvas("XrayGoggles", color: "#044a0400");
            float a = canvas.GetComponentInChildren<Image>().color.a;
            while (a < CanvasAlphaMax)
            {
                if (a > CanvasAlphaMax) a = CanvasAlphaMax;
                a += 0.0125f;
                canvas.GetComponentInChildren<Image>().color = canvas.GetComponentInChildren<Image>().color.Change(a: a);
                yield return null;
            }
            fogs.Do(x => x.Key.strength = 0);
            ec.UpdateFog();
            yield break;
        }
        private IEnumerator Disable()
        {
            float a = canvas.GetComponentInChildren<Image>().color.a;
            while (a > 0)
            {
                if (a < 0) a = 0;
                a -= 0.0125f;
                canvas.GetComponentInChildren<Image>().color = canvas.GetComponentInChildren<Image>().color.Change(a: a);
                yield return null;
            }
            fogs.Do(x => x.Key.strength = x.Value);
            ec.UpdateFog();
        }
        private void DestroyCanvas()
        {
            Destroy(canvas);
            canvas = null;
        }
        void OnDestroy()
        {
            DestroyCanvas();
        }
    }
}
