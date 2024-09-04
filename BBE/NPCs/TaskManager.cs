using MTM101BaldAPI.Components;
using System.IO;
using System;
using System.Collections.Generic;

namespace BBE.NPCs
{
    public class TaskManager : NPC
    {
        public AudioManager audMan;
        public CustomSpriteAnimator animator;
        public override void Initialize()
        {
            base.Initialize();
            audMan = this.GetComponent<AudioManager>();
        }
        public static string[] GetDesktop()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            FileInfo[] files = directoryInfo.GetFiles();
            List<string> res = new List<string>();
            foreach (FileInfo file in files)
            {
                res.Add(file.Name);
            }
            return res.ToArray();
        }
    }
}
