using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MTM101BaldAPI.AssetManager;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI.Table;

namespace BBE.Helpers
{
    class AssetsHelper
    {
        public static Sprite SpriteFromFile(string path, float pixelsPerUnit = 1f)
        {
            Texture2D texture = AssetManager.TextureFromFile(ModPath + path);
            Sprite sprite = AssetManager.SpriteFromTexture2D(texture, Vector2.one/2f, pixelsPerUnit);
            return sprite;
        }
        private static string ModPath = "BALDI_Data/StreamingAssets/Modded/rost.moment.baldiplus.extramod/";
    }
}
