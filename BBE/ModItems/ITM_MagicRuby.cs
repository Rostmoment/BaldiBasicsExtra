using BBE.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.ModItems
{
    class Portal : MonoBehaviour
    {
        private SpriteRenderer sprite;
        void Start()
        {
            sprite = gameObject.AddComponent<SpriteRenderer>(); 
            sprite.sprite = BasePlugin.Instance.asset.Get<Sprite>("MagicRubyPortal");
            gameObject.layer = LayerMask.NameToLayer("Billboard");
            gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }
        void Update()
        {
            try
            {
                gameObject.transform.LookAt(new Vector3(Camera.main.transform.position.x, 5, Camera.main.transform.position.z));
            }
            catch (NullReferenceException) { }
        }
        public void TeleportPlayer(PlayerManager pm)
        {
            pm.Teleport(gameObject.transform.position);
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle("Teleport", false);
            ITM_MagicRuby.portal = null;
            Destroy(sprite);
            Destroy(gameObject);
            Destroy(this);
        }
    }
    class ITM_MagicRuby : Item
    {
        public static Portal portal = null;

        private void CreatePortal(PlayerManager pm)
        {
            portal = this.gameObject.AddComponent<Portal>();
            portal.transform.position = pm.transform.position;
            portal.name = "BaldiBasicsExtraMagicRubyPortal";
        }
        public override bool Use(PlayerManager pm)
        {
            if (portal.IsNull())
            {
                CreatePortal(pm);
                return false;
            }
            try
            {
                portal.TeleportPlayer(pm);
                Destroy(gameObject);
                return true;
            }
            catch (NullReferenceException)
            {
                CreatePortal(pm);
                return false;
            }
        }
    }
}
