using BBE.Extensions;
using BBE.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace BBE.ModItems
{
    class ITM_BaldiTeleporter : Item, IEntityTrigger, IItemPrefab
    {
        private float speed = 120;
        private PlayerManager player;
        private float angle = 0;
        [SerializeField]
        private PropagatedAudioManager audMan;
        [SerializeField]
        private Entity entity;
        [SerializeField]
        private Transform renderer;

        public void EntityTriggerEnter(Collider other) { }

        public void EntityTriggerExit(Collider other) { }

        public void EntityTriggerStay(Collider other)
        {

        }
        private IEnumerator Timer(float time)
        {
            float left = time;
            while (left > 0)
            {
                left -= Time.deltaTime * player.ec.EnvironmentTimeScale;
                yield return null;
            }
            Destroy(gameObject);
            yield break;
        }
        private void Update()
        {
            angle += 120 * Time.deltaTime;
            if (angle > 360)
                angle = 0;
            renderer.Find("Sprite_Flying").GetComponent<SpriteRenderer>().SetSpriteRotation(angle);
            entity.UpdateInternalMovement(transform.forward * speed * player.ec.EnvironmentTimeScale);
        }
        public override bool Use(PlayerManager pm)
        {
            audMan.SetLoop(false);
            angle = 0;
            gameObject.SetActive(true);
            player = pm;
            StartCoroutine(Timer(15));
            renderer.Find("Sprite_Flying").GetComponent<SpriteRenderer>();
            transform.position = pm.transform.position;
            transform.rotation = CoreGameManager.Instance.GetCamera(pm.playerNumber).transform.rotation;
            entity.Initialize(pm.ec, transform.position);
            entity.OnEntityMoveInitialCollision += OnWallHit;
            return true;
        }

        private void OnWallHit(RaycastHit hit)
        {
            if (speed > 0 && hit.transform.gameObject.layer != 2)
            {
                player.ec.GetBaldi()?.Navigator.Entity.Teleport(transform.position);
                entity.SetFrozen(true);
                speed = 0;
                renderer.gameObject.SetActive(false);
                audMan.PlaySingle("Teleport");
                StartCoroutine(Timer(0));
            }
        }

        public void SetupAssets()
        {
            renderer = Instantiate(AssetsHelper.LoadAsset<Gum>().transform.Find("RendererBase"));
            renderer.SetParent(transform);
            renderer.localPosition = Vector3.zero;
            renderer.Find("Sprite_Flying").GetComponent<SpriteRenderer>().sprite = AssetsHelper.CreateTexture("Textures", "Items", "BBE_BaldiTeleporterLarge.png").ToSprite(20);
            //Destroy(renderer.Find("Sprite_Grounded"));
            gameObject.layer = Layer.StandardEntities.ToLayer();
            audMan = gameObject.AddPropagatedAudioManager();
            entity = gameObject.CreateEntity(1f, 1f, out CapsuleCollider collider, out _, renderer);
            entity.collisionLayerMask = Layer.GumCollision.ToLayer();
            entity.SetGrounded(false);
            collider.height = 4;
        }
    }
}
