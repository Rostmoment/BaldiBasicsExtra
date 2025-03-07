using BBE.API;
using BBE.CustomClasses;
using BBE.Extensions;
using BBE.Creators;
using MTM101BaldAPI;
using MTM101BaldAPI.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Helpers;

namespace BBE.NPCs
{
    public class RostPlayable
    {
        // For playable character mod
    }
    public class Rost : NPC
    {
        public readonly static string[] links = new string[]
        {
            "https://youtu.be/oavMtUWDBTM?si=zL1j3npiyOOKnH3R", // Trolololo
            "https://youtu.be/dQw4w9WgXcQ?si=UhWPe-R3airdThzd", // Rick roll
            "https://youtu.be/o755NMLMDXk?si=JxoeToQlKMshlYuE", // Get XD
            "https://youtu.be/fC7oUOUEEi4?si=iGkc02dqMiOqQae7", // Get stick buggged LOL
            "https://youtu.be/PXqcHi2fkXI?si=rNZqNc9Ba6X-OFew", // you've been trolled
            "https://youtu.be/IlouAA8mRZo?si=VpfH3aFY3z7sfKlO", // your mine
            "https://youtu.be/jNQXAC9IVRw?si=wHT3qNmX5eS_tRtD", // Me at the zoo (world first YouTube video)
            "https://youtu.be/YTC75cKzuNk?si=TzT3KKp-3ufW6wP0", // Roblox death sound origin
            "https://youtu.be/PwJL04MRVyA?si=B2E9Mn30sNJiGLkQ" // Roblox news

        };
        public void EndGame()
        {/*
            if (ModConfig.ShutdownPcOnLose)
            {
                BasePlugin.Logger.LogError("Goodbye World!");
                WindowsAPI.ShutdownPC();
            }   
            else
            {
                WindowsAPI.OpenLink(links.ChooseRandom());
            }*/
        }
        private AudioManager audMan;
        private static IntPtr _window;
        public static IntPtr GameWindow 
        {
            get
            {
                // if (_window.IsNull() || _window == IntPtr.Zero) _window = WindowsAPI.GetWindowByTitleName("Baldi's Basics Plus");
                return _window;
            }
            set
            {
                _window = value;
            }
        }
        public override void Initialize()
        {
            base.Initialize();
            audMan = this.GetComponent<AudioManager>();
            audMan.overrideSubtitleColor = false;
            spriteRenderer[0].sprite = AssetsHelper.CreateColoredSprite(new Color(0, 0, 0, 0), 10, 10);
            Navigator.passableObstacles.Add(PassableObstacle.Window);
            Navigator.passableObstacles.Add(PassableObstacle.Bully);
            Navigator.passableObstacles.Add(PassableObstacle.LockedDoor);
        }
    }
}
