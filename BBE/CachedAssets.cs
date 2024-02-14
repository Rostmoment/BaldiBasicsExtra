using BBE.Helpers;
using Rewired;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BBE
{
    public static class CachedAssets
    {
        public static Sprite hookSprite;
        public static Sprite cracksSprite;
        public static SoundObject teleportSound;
        public static Material lineMaterial;
        public static SoundObject grappleLaunchSound;
        public static SoundObject grappleClangSound;
        public static AudioClip grappleLoopSound;
        public static SoundObject baldiBreak;
        public static Dictionary<string, MapIcon> MapIcons = new Dictionary<string, MapIcon>();
        public readonly static Dictionary<string, ItemObject> items = new Dictionary<string, ItemObject>();
        public static SoundObject TeleportationChaosBegin;
        public static void CacheGameAssets()
        {
            teleportSound = AssetsHelper.LoadAsset<SoundObject>("Teleport");
            hookSprite = AssetsHelper.LoadAsset<Sprite>("GrapplingHookSprite");
            cracksSprite = AssetsHelper.LoadAsset<Sprite>("GrappleCracks");
            lineMaterial = AssetsHelper.LoadAsset<Material>("BlackBehind");
            grappleLaunchSound = AssetsHelper.LoadAsset<SoundObject>("GrappleLaunch");
            grappleClangSound = AssetsHelper.LoadAsset<SoundObject>("GrappleClang");
            grappleLoopSound = AssetsHelper.LoadAsset<AudioClip>("GrappleLoop");
            baldiBreak = AssetsHelper.LoadAsset<SoundObject>("BAL_Break");
            TeleportationChaosBegin = AssetsHelper.CreateSoundObject("Audio/Events/EventTCStarted.ogg", SoundType.Effect, Subtitle: false);
        }
    }
}
