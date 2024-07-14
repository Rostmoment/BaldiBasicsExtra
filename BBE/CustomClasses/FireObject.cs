using BBE.Events;
using BBE.Helpers;
using MTM101BaldAPI;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace BBE.CustomClasses
{
    public class FireObject : MonoBehaviour
    {
        void Update()
        {
            try
            {
                gameObject.transform.LookAt(new Vector3(Camera.main.transform.position.x, 5, Camera.main.transform.position.z));

            }
            catch (NullReferenceException) { }
        }
        void Start()
        {
            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.sprite = BasePlugin.Instance.asset.Get<Sprite>("FireSprite");
            collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(4.5f, 15f, 4.5f);
            collider.isTrigger = true;
            StartCoroutine(LightAnimation());
        }
        public void DestroyFire()
        {
            StartCoroutine(UnlightAnimation());
            fireEvent.Fires.Remove(this);
        }
        public IEnumerator UnlightAnimation()
        {
            float[] sizes = new float[] { 1, 1, 1 };
            float speed = 0.05f;
            while (sizes[1] > 0)
            {
                gameObject.transform.localScale = new Vector3(sizes[0], sizes[1], sizes[2]);
                sizes[0] -= speed;
                sizes[1] -= speed;
                sizes[2] -= speed;
                yield return null;
            }
            Destroy(gameObject);
            yield break;
        }
        private IEnumerator LightAnimation()
        {
            float[] sizes = new float[] { 0, 0, 0 };
            float speed = 0.05f;
            while (sizes[1] < 1)
            {
                gameObject.transform.localScale = new Vector3(sizes[0], sizes[1], sizes[2]);
                sizes[0] += speed;
                sizes[1] += speed;
                sizes[2] += speed;
                yield return null;
            }
            yield break;
        }
        public void DestoyWithNoAnimation()
        {
            Destroy(gameObject);
        }
        public void UpdatePosition(Cell cell)
        {
            gameObject.transform.position = cell.TileTransform.position;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 5, gameObject.transform.position.z);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<ITM_BSODA>().IsNull())
            {
                DestroyFire();
            }
            if (other.CompareTag("Player"))
            {
                if (FireEvent.RedEffect.IsNull())
                {
                    GameObject game = Instantiate(Prefabs.Canvas);
                    game.name = "RedFireEffect";
                    FireEvent.RedEffect = game;
                }
                Image image = FireEvent.RedEffect.GetComponentInChildren<Image>();
                image.sprite = null;
                image.color = new Color(1, 0, 0, FireEvent.AlphaChannel);
                image.transform.localScale = new Vector2(100, 100);
                image.transform.localPosition = new Vector2(0, 0);
                Canvas canvas = FireEvent.RedEffect.GetComponent<Canvas>();
                canvas.gameObject.SetActive(true);
                canvas.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
                FireEvent.AlphaChannel += 0.1f;
            }
        }
        public FireEvent fireEvent;
        public BoxCollider collider;
        public SpriteRenderer sprite;
    }
}
