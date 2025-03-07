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
        private AudioManager audioMan;
        public override Texture2D MaterialTexture => BasePlugin.Asset.Get<Texture2D>("NotebookDoorMaterial");
        public int notebookToCollect = 2;
        public override void OnStart()
        {
            base.OnStart();
            audioMan = gameObject.AddAudioManager();
            SwingDoor.Lock(true);
        }
        public void Unlock()
        {
            // WHY THERE IS NULLREFERENCEEXCPETION
            SwingDoor.Unlock();
            Destroy(this);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                audioMan.PlaySingle("CollectTwoNotebooks");
        }
    }
}
