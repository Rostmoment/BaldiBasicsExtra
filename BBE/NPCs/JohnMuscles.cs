using BBE.Extensions;
using BBE.Creators;
using BBE.ModItems;
using HarmonyLib;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Helpers;

namespace BBE.NPCs
{
    public class JohnMuscles : NPC, IPrefab
    {
        [SerializeField]
        private Sprite talking, angry, happy; // 40
        [SerializeField]
        private Sprite[] walking, walkingAngry, showing;
        public void SetupAssets()
        {
            talking = AssetsHelper.CreateTexture("Textures", "NPCs", "JohnMuscles", "BBE_JohnMusclesTalking.png").ToSprite(40);
            angry = AssetsHelper.CreateTexture("Textures", "NPCs", "JohnMuscles", "BBE_JohnMusclesAngry.png").ToSprite(40);
            happy = AssetsHelper.CreateTexture("Textures", "NPCs", "JohnMuscles", "BBE_JohnMusclesHappy.png").ToSprite(40);
            walking = AssetsHelper.CreateSpriteSheet(2, 1, 40, "Textures", "NPCs", "JohnMuscles", "BBE_JohnMusclesWalking.png");
            walkingAngry = AssetsHelper.CreateSpriteSheet(2, 1, 40, "Textures", "NPCs", "JohnMuscles", "BBE_JohnMusclesWalkingAngry.png");
            showing = AssetsHelper.CreateSpriteSheet(2, 1, 40, "Textures", "NPCs", "JohnMuscles", "BBE_JohnMusclesShowing.png");
        }
        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
