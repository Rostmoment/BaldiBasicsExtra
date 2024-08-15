using BBE.CustomClasses;
using BBE.Helpers;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBE.Events
{
    public class FireEvent : RandomEvent
    {
        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            base.Initialize(controller, rng);
        }
        public override void Begin()
        {
            base.Begin();
            ec.MaxRaycast = float.NegativeInfinity;
            ec.AddFog(CreateFog());
            ec.audMan.PlaySingle("FireEventStart");
            for (int x = 0; x<ec.AllTilesNoGarbage(false, false).Count; x++)
            {
                if (x % 5 == index)
                {
                    SpawnFire(ec.AllTilesNoGarbage(false, false)[x]);
                }
            }
            foreach (GameObject gameObject in FindObjectsOfType<GameObject>())
            {
                if (gameObject.name.ToLower().Contains("tree"))
                {
                    Destroy(gameObject);
                }
            }

            index++;
        }
        public IEnumerator DestroyCanvas()
        {
            float a = RedEffect.GetComponentInChildren<Image>().color.a;
            while (a > 0)
            {
                a -= Time.deltaTime;
                if (a < 0)
                {
                    a = 0;
                }
                RedEffect.GetComponentInChildren<Image>().color = new Color(1, 0, 0, a);
                yield return null;
            }
            FireEvent.AlphaChannel = 0;
            yield break;
        }
        public override void End()
        {
            base.End();
            StartCoroutine(DestroyCanvas());
            ec.MaxRaycast = float.PositiveInfinity;
            ec.RemoveFog(FireFog);
            foreach (FireObject fire in Fires)
            {
                fire.DestoyWithNoAnimation();
            }
            index--;
        }
        public Fog CreateFog()
        {
            FireFog = new Fog
            {
                color = Color.red,
                maxDist = 100,
                priority = 0,
                startDist = 5,
                strength = 2
            };
            return FireFog;
        }
        public void SpawnFire(Cell cell)
        {
            GameObject Fire = Instantiate(Prefabs.Fire);
            FireObject currentFire = Fire.AddComponent<FireObject>();
            Fire.name = "Fire_ExtraMod";
            currentFire.fireEvent = this;
            currentFire.UpdatePosition(cell);
            Fires.Add(currentFire);
        }
        public static float AlphaChannel = 0;
        public static GameObject RedEffect = null;
        public List<FireObject> Fires = new List<FireObject>();
        public Fog FireFog;
        public static int index = 0;
    }
}
