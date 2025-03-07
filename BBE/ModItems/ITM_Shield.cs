using UnityEngine;
using System.Collections;
using BBE.Creators;
using UnityEngine.UI;
using BBE.Extensions;

namespace BBE.ModItems
{
    public class ITM_Shield : Item
    {
        public GameObject canvas;
        // Makes the player invincible to Baldi
        public override bool Use(PlayerManager pm)
        {
            if (used)
            {
                return false;
            }
            used = true;
            CoreGameManager.Instance.GetPlayer(0).invincible = true;
            StartCoroutine(CanvasEffect(true));
            StartCoroutine(EndShield());
            return true;
        }
        private IEnumerator CanvasEffect(bool state)
        {
            if (canvas == null)
            {
                canvas = CreateObjects.CreateCanvas("ShieldCanvas", color: new Color(0.039215f, 0.898039f, 0.9960784f, 0f));
            }
            if (state)
            {
                float a = 0;
                while (a != 0.25f)
                {
                    a = canvas.GetComponentInChildren<Image>().color.a;
                    a += 0.0125f;
                    canvas.GetComponentInChildren<Image>().color = canvas.GetComponentInChildren<Image>().color.Change(a: a);
                    if (a > 0.25f)
                    {
                        a = 0.25f;
                        canvas.GetComponentInChildren<Image>().color = canvas.GetComponentInChildren<Image>().color.Change(a: a);
                        yield break;
                    }
                    yield return null;
                }
            }
            else
            {
                float a = 0.25f;
                while (a != 0)
                {
                    a = canvas.GetComponentInChildren<Image>().color.a;
                    a -= 0.0125f;
                    if (a < 0f)
                    {
                        a = 0f;
                        canvas.GetComponentInChildren<Image>().color = canvas.GetComponentInChildren<Image>().color.Change(a: a);
                        used = false;
                        Destroy(canvas);
                        Destroy(gameObject);
                        yield break;
                    }
                    canvas.GetComponentInChildren<Image>().color = canvas.GetComponentInChildren<Image>().color.Change(a: a);
                    yield return null;
                }
            }
        }
        private IEnumerator EndShield()
        {
            float TimeLeft = 20f;
            while (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                yield return null;
            }
            CoreGameManager.Instance.GetPlayer(0).invincible = false;
            StartCoroutine(CanvasEffect(false));
            yield break;
        }
        void OnDestroy()
        {
            used = false;
        }
        public static bool used = false;
    }
}
