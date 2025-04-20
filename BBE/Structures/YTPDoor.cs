using BBE.Extensions;
using BBE.Creators;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Helpers;

namespace BBE.Structures
{
    class YTPDoor : CustomSwingDoor<YTPDoor>, IClickable<int>
    {
        public int ytp = 250;

        public override Texture2D MaterialTexture => BasePlugin.Asset.Get<Texture2D>("YTPDoorMaterial");

        public override void VirtualStart()
        {
            base.VirtualStart();
            SwingDoor.Lock(true);
        }
        public void Unlock()
        {
            SwingDoor.Unlock();
            Destroy(this);
        }
        public void Clicked(int player)
        {
            if (CoreGameManager.Instance.GetPoints(player) >= ytp)
            {
                Unlock();
                CoreGameManager.Instance.AddPoints(-ytp, player, true);
            }
        }

        public void ClickableSighted(int player) { }
        public void ClickableUnsighted(int player) { }
        public bool ClickableHidden() => false;
        public bool ClickableRequiresNormalHeight() => false;
    }
}
