using BBE.Events;
using BBE.Extensions;
using BBE.Creators;
using MTM101BaldAPI;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using BBE.Helpers;

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
            spriteObject = new GameObject("FireSprite");
            sprite = spriteObject.AddSpriteRender(BasePlugin.Asset.Get<Sprite>("FireSprite"));
            spriteObject.transform.SetParent(this.gameObject.transform, false);
            collider = gameObject.AddCollider(new Vector3(4.5f, 15f, 4.5f));
            spriteObject.layer = LayerMask.NameToLayer("Billboard");
            StartCoroutine(LightAnimation());
        }
        public void DestroyFire()
        {
            StartCoroutine(UnlightAnimation());
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
            Destroy(spriteObject);
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
            Destroy(spriteObject);
            Destroy(gameObject);
        }
        public void UpdatePosition(Cell cell)
        {
            gameObject.transform.position = cell.TileTransform.position;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 5, gameObject.transform.position.z);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.HasComponent<ITM_BSODA>())
            {
                DestroyFire();
                fireEvent.fires.Remove(this);
            }
            if (other.CompareTag("Player"))
            {
                if (FireEvent.RedEffect == null)
                {
                    GameObject game = Instantiate(Prefabs.Canvas);
                    game.name = "RedFireEffect";
                    FireEvent.RedEffect = game;
                }
                Image image = FireEvent.RedEffect.GetComponentInChildren<Image>();
                image.sprite = null;
                image.color = new Color(1, 0, 0, FireEvent.AlphaChannel);
                image.transform.localScale = new Vector2(100, 100);
                image.transform.localPosition = Vector2.zero;
                Canvas canvas = FireEvent.RedEffect.GetComponent<Canvas>();
                canvas.gameObject.SetActive(true);
                canvas.worldCamera = CoreGameManager.Instance.GetCamera(0).canvasCam;
                FireEvent.AlphaChannel += 0.1f;
            }
            if (other.HasComponent<NPC>())
            {
                NPC npc = other.GetComponent<NPC>();
                if (npc.TryGetComponent<Looker>(out Looker loooker))
                {
                    if (!FireEvent.npcs.ContainsKey(npc))
                        FireEvent.npcs.Add(npc, loooker.distance);
                    if (loooker.distance > 0)
                    {
                        if (loooker.distance - LookerSubsctraction < 0)
                        {
                            loooker.distance = 0;
                        }
                        else
                        {
                            loooker.distance -= LookerSubsctraction;
                        }
                    }
                };
            }
        }
        private static float LookerSubsctraction => 1000;
        private GameObject spriteObject;
        public FireEvent fireEvent;
        public BoxCollider collider;
        public SpriteRenderer sprite;
    }
}
