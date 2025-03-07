using BaldiLevelEditor;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Compats.EditorCompat
{
    class YTPDoorVisual : DoorEditorVisual
    {
        public override void SetupMaterials(MeshRenderer renderer, bool outside)
        {
            base.SetupMaterials(renderer, outside);
            renderer.materials[0].SetMaskTexture(BaldiLevelEditorPlugin.Instance.assetMan.Get<Texture2D>("SwingDoorMask"));
            renderer.materials[1].SetMainTexture(BasePlugin.Asset.Get<Texture2D>("YTPDoorMaterial"));
        }
    }
}
