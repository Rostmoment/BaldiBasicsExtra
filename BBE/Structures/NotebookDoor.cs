using BBE.Extensions;
using BBE.Creators;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Structures
{
    class NotebookDoor : CustomSwingDoor<NotebookDoor>
    {
        public override Texture2D MaterialTexture => BasePlugin.Asset.Get<Texture2D>("NotebookDoorMaterial");
        public int notebookToCollect = 2;
        public override void VirtualStart()
        {
            base.VirtualStart();
            SwingDoor.Lock(true);
        }
        public void Unlock()
        {
            // WHY THERE IS NULLREFERENCEEXCPETION
            SwingDoor.Unlock();
            Destroy(this);
        }
    }
}
