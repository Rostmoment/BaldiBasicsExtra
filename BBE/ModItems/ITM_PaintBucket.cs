using BBE.Compats;
using BBE.Extensions;
using BBE.Creators;
using BBE.NPCs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Helpers;

namespace BBE.ModItems
{
    public class ITM_PaintBucket : Item
    {
        private PaintBase paint;
        public override bool Use(PlayerManager pm)
        {
            if (ModIntegration.TimesIsInstalled) pm.RuleBreak("littering", 5f, 0.8f);
            Color color = Color.white;
            GameObject game = new GameObject("PaintItem");
            switch (UnityEngine.Random.Range(0, 4))
            {
                case 0:
                    paint = game.AddComponent<PinkPaint>();
                    color = Color.magenta;
                    break;
                case 1:
                    paint = game.AddComponent<YellowPaint>();
                    color = Color.yellow;
                    break;
                case 2:
                    paint = game.AddComponent<BluePaint>();
                    color = Color.blue;
                    break;
                case 3:
                    paint = game.AddComponent<PurplePaint>();
                    color = AssetsHelper.ColorFromHex("761594");
                    break;
            }
            paint.Initialize(BasePlugin.Asset.Get<Sprite>("PaintBase").ReplaceColor(Color.white, color), pm.gameObject);
            paint.item = gameObject;
            paint.paintObject = game;
            return true;
        }
    }
}
