using BBE.Extensions;
using BBE.Creators;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace BBE.Helpers
{
    // I copied this class from AnyModInEditor
    class Letters
    {
        public static Dictionary<char, Texture2D> letters = new Dictionary<char, Texture2D>() { };
        public static void CreateTexture(char c, params List<int>[] positions)
        {
            Texture2D res = AssetsHelper.CreateTexture(10, 10, Color.clear);
            foreach (List<int> position in positions)
            {
                res.SetPixel(position[0], 9 - position[1], Color.black);
            }
            res.Apply();
            letters.Add(c, res);
        }
        public static Sprite CreateSprite(string name) => Sprite.Create(CreateTexture(new Texture2D(32, 32), name), new Rect(), Vector2.one);
        public static Texture2D CreateTexture(string name) => CreateTexture(null, name);
        public static Texture2D CreateTexture(Texture2D prefab, string name)
        {
            if (name.Length > 9)
                name = name.Substring(0, 9);
            Texture2D textureResult = null;
            if (prefab == null) textureResult = AssetsHelper.CreateTexture(32, 32, Color.clear);
            else
            {
                textureResult = prefab.CopyTexture();
            }
            int xAdd = 0;
            int yAdd = 20;
            foreach (char c in name)
            {
                if (!letters.ContainsKey(char.ToUpper(c)))
                {
                    continue;
                }
                Texture2D texture = letters[char.ToUpper(c)];
                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        Color color = texture.GetPixel(x, y);
                        if (color.a > 0)
                        {
                            textureResult.SetPixel(x + xAdd, y + yAdd, color);
                        }
                    }
                }
                xAdd += 10;
                if (xAdd >= 30)
                {
                    xAdd = 0;
                    yAdd -= 10;
                }
            }
            textureResult.Apply();
            return textureResult;
        }
        public static void Create()
        {
            // Thanks to Crashy
            CreateTexture('A', new List<int> { 5, 0 }, new List<int> { 5, 1 }, new List<int> { 5, 2 }, new List<int> { 4, 3 }, new List<int> { 6, 3 }, new List<int> { 3, 4 }, new List<int> { 6, 4 }, new List<int> { 3, 5 }, new List<int> { 6, 5 }, new List<int> { 2, 6 }, new List<int> { 3, 6 }, new List<int> { 4, 6 }, new List<int> { 5, 6 }, new List<int> { 6, 6 }, new List<int> { 2, 7 }, new List<int> { 7, 7 }, new List<int> { 1, 8 }, new List<int> { 7, 8 }, new List<int> { 1, 9 }, new List<int> { 7, 9 });
            CreateTexture('B', new List<int> { 2, 0 }, new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 2, 1 }, new List<int> { 6, 1 }, new List<int> { 2, 2 }, new List<int> { 6, 2 }, new List<int> { 2, 3 }, new List<int> { 6, 3 }, new List<int> { 2, 4 }, new List<int> { 5, 4 }, new List<int> { 2, 5 }, new List<int> { 3, 5 }, new List<int> { 4, 5 }, new List<int> { 5, 5 }, new List<int> { 6, 5 }, new List<int> { 2, 6 }, new List<int> { 7, 6 }, new List<int> { 2, 7 }, new List<int> { 7, 7 }, new List<int> { 2, 8 }, new List<int> { 6, 8 }, new List<int> { 2, 9 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 });
            CreateTexture('C', new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 7, 0 }, new List<int> { 3, 1 }, new List<int> { 7, 1 }, new List<int> { 2, 2 }, new List<int> { 2, 3 }, new List<int> { 1, 4 }, new List<int> { 1, 5 }, new List<int> { 1, 6 }, new List<int> { 1, 7 }, new List<int> { 1, 8 }, new List<int> { 7, 8 }, new List<int> { 2, 9 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 }, new List<int> { 6, 9 });
            CreateTexture('D', new List<int> { 1, 0 }, new List<int> { 2, 0 }, new List<int> { 1, 1 }, new List<int> { 3, 1 }, new List<int> { 4, 1 }, new List<int> { 1, 2 }, new List<int> { 5, 2 }, new List<int> { 1, 3 }, new List<int> { 6, 3 }, new List<int> { 1, 4 }, new List<int> { 7, 4 }, new List<int> { 1, 5 }, new List<int> { 7, 5 }, new List<int> { 1, 6 }, new List<int> { 7, 6 }, new List<int> { 1, 7 }, new List<int> { 7, 7 }, new List<int> { 1, 8 }, new List<int> { 6, 8 }, new List<int> { 2, 9 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 });
            CreateTexture('E', new List<int> { 2, 0 }, new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 7, 0 }, new List<int> { 2, 1 }, new List<int> { 2, 2 }, new List<int> { 2, 3 }, new List<int> { 2, 4 }, new List<int> { 3, 4 }, new List<int> { 4, 4 }, new List<int> { 5, 4 }, new List<int> { 6, 4 }, new List<int> { 7, 4 }, new List<int> { 2, 5 }, new List<int> { 2, 6 }, new List<int> { 2, 7 }, new List<int> { 2, 8 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 }, new List<int> { 6, 9 }, new List<int> { 7, 9 });
            CreateTexture('F', new List<int> { 2, 0 }, new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 7, 0 }, new List<int> { 2, 1 }, new List<int> { 2, 2 }, new List<int> { 2, 3 }, new List<int> { 2, 4 }, new List<int> { 3, 4 }, new List<int> { 4, 4 }, new List<int> { 5, 4 }, new List<int> { 6, 4 }, new List<int> { 2, 5 }, new List<int> { 2, 6 }, new List<int> { 2, 7 }, new List<int> { 2, 8 }, new List<int> { 2, 9 });
            CreateTexture('G', new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 3, 1 }, new List<int> { 7, 1 }, new List<int> { 2, 2 }, new List<int> { 2, 3 }, new List<int> { 1, 4 }, new List<int> { 1, 5 }, new List<int> { 4, 5 }, new List<int> { 5, 5 }, new List<int> { 6, 5 }, new List<int> { 7, 5 }, new List<int> { 8, 5 }, new List<int> { 1, 6 }, new List<int> { 8, 6 }, new List<int> { 1, 7 }, new List<int> { 7, 7 }, new List<int> { 1, 8 }, new List<int> { 6, 8 }, new List<int> { 2, 9 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 });
            CreateTexture('H', new List<int> { 1, 0 }, new List<int> { 8, 0 }, new List<int> { 1, 1 }, new List<int> { 8, 1 }, new List<int> { 1, 2 }, new List<int> { 8, 2 }, new List<int> { 1, 3 }, new List<int> { 8, 3 }, new List<int> { 1, 4 }, new List<int> { 8, 4 }, new List<int> { 1, 5 }, new List<int> { 2, 5 }, new List<int> { 3, 5 }, new List<int> { 4, 5 }, new List<int> { 5, 5 }, new List<int> { 6, 5 }, new List<int> { 7, 5 }, new List<int> { 8, 5 }, new List<int> { 1, 6 }, new List<int> { 8, 6 }, new List<int> { 1, 7 }, new List<int> { 8, 7 }, new List<int> { 1, 8 }, new List<int> { 8, 8 }, new List<int> { 1, 9 }, new List<int> { 8, 9 });
            CreateTexture('I', new List<int> { 2, 0 }, new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 4, 1 }, new List<int> { 4, 2 }, new List<int> { 4, 3 }, new List<int> { 4, 4 }, new List<int> { 4, 5 }, new List<int> { 4, 6 }, new List<int> { 4, 7 }, new List<int> { 4, 8 }, new List<int> { 2, 9 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 }, new List<int> { 6, 9 });
            CreateTexture('J', new List<int> { 2, 0 }, new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 7, 0 }, new List<int> { 5, 1 }, new List<int> { 5, 2 }, new List<int> { 5, 3 }, new List<int> { 5, 4 }, new List<int> { 5, 5 }, new List<int> { 5, 6 }, new List<int> { 1, 7 }, new List<int> { 5, 7 }, new List<int> { 1, 8 }, new List<int> { 5, 8 }, new List<int> { 2, 9 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 });
            CreateTexture('K', new List<int> { 2, 0 }, new List<int> { 7, 0 }, new List<int> { 2, 1 }, new List<int> { 6, 1 }, new List<int> { 2, 2 }, new List<int> { 5, 2 }, new List<int> { 2, 3 }, new List<int> { 4, 3 }, new List<int> { 2, 4 }, new List<int> { 3, 4 }, new List<int> { 2, 5 }, new List<int> { 3, 5 }, new List<int> { 2, 6 }, new List<int> { 4, 6 }, new List<int> { 2, 7 }, new List<int> { 5, 7 }, new List<int> { 2, 8 }, new List<int> { 6, 8 }, new List<int> { 2, 9 }, new List<int> { 7, 9 });
            CreateTexture('L', new List<int> { 2, 0 }, new List<int> { 2, 1 }, new List<int> { 2, 2 }, new List<int> { 2, 3 }, new List<int> { 2, 4 }, new List<int> { 2, 5 }, new List<int> { 2, 6 }, new List<int> { 2, 7 }, new List<int> { 2, 8 }, new List<int> { 2, 9 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 }, new List<int> { 6, 9 });
            CreateTexture('M', new List<int> { 2, 0 }, new List<int> { 6, 0 }, new List<int> { 2, 1 }, new List<int> { 6, 1 }, new List<int> { 2, 2 }, new List<int> { 6, 2 }, new List<int> { 2, 3 }, new List<int> { 5, 3 }, new List<int> { 6, 3 }, new List<int> { 1, 4 }, new List<int> { 3, 4 }, new List<int> { 5, 4 }, new List<int> { 7, 4 }, new List<int> { 1, 5 }, new List<int> { 3, 5 }, new List<int> { 5, 5 }, new List<int> { 7, 5 }, new List<int> { 1, 6 }, new List<int> { 3, 6 }, new List<int> { 5, 6 }, new List<int> { 7, 6 }, new List<int> { 0, 7 }, new List<int> { 3, 7 }, new List<int> { 5, 7 }, new List<int> { 7, 7 }, new List<int> { 0, 8 }, new List<int> { 4, 8 }, new List<int> { 8, 8 }, new List<int> { 0, 9 }, new List<int> { 4, 9 }, new List<int> { 8, 9 });
            CreateTexture('N', new List<int> { 0, 0 }, new List<int> { 8, 0 }, new List<int> { 0, 1 }, new List<int> { 1, 1 }, new List<int> { 8, 1 }, new List<int> { 0, 2 }, new List<int> { 2, 2 }, new List<int> { 8, 2 }, new List<int> { 0, 3 }, new List<int> { 2, 3 }, new List<int> { 8, 3 }, new List<int> { 0, 4 }, new List<int> { 3, 4 }, new List<int> { 8, 4 }, new List<int> { 0, 5 }, new List<int> { 4, 5 }, new List<int> { 8, 5 }, new List<int> { 0, 6 }, new List<int> { 5, 6 }, new List<int> { 8, 6 }, new List<int> { 0, 7 }, new List<int> { 6, 7 }, new List<int> { 8, 7 }, new List<int> { 0, 8 }, new List<int> { 7, 8 }, new List<int> { 8, 8 }, new List<int> { 0, 9 }, new List<int> { 8, 9 });
            CreateTexture('O', new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 2, 1 }, new List<int> { 7, 1 }, new List<int> { 1, 2 }, new List<int> { 8, 2 }, new List<int> { 0, 3 }, new List<int> { 8, 3 }, new List<int> { 0, 4 }, new List<int> { 8, 4 }, new List<int> { 0, 5 }, new List<int> { 8, 5 }, new List<int> { 0, 6 }, new List<int> { 8, 6 }, new List<int> { 0, 7 }, new List<int> { 7, 7 }, new List<int> { 1, 8 }, new List<int> { 6, 8 }, new List<int> { 2, 9 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 });
            CreateTexture('P', new List<int> { 2, 0 }, new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 2, 1 }, new List<int> { 6, 1 }, new List<int> { 2, 2 }, new List<int> { 6, 2 }, new List<int> { 2, 3 }, new List<int> { 6, 3 }, new List<int> { 2, 4 }, new List<int> { 6, 4 }, new List<int> { 2, 5 }, new List<int> { 3, 5 }, new List<int> { 4, 5 }, new List<int> { 5, 5 }, new List<int> { 2, 6 }, new List<int> { 2, 7 }, new List<int> { 2, 8 }, new List<int> { 2, 9 });
            CreateTexture('Q', new List<int> { 2, 0 }, new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 6, 1 }, new List<int> { 0, 2 }, new List<int> { 7, 2 }, new List<int> { 0, 3 }, new List<int> { 7, 3 }, new List<int> { 0, 4 }, new List<int> { 7, 4 }, new List<int> { 0, 5 }, new List<int> { 7, 5 }, new List<int> { 1, 6 }, new List<int> { 4, 6 }, new List<int> { 5, 6 }, new List<int> { 7, 6 }, new List<int> { 2, 7 }, new List<int> { 3, 7 }, new List<int> { 4, 7 }, new List<int> { 5, 7 }, new List<int> { 6, 7 }, new List<int> { 7, 8 }, new List<int> { 7, 9 }, new List<int> { 8, 9 });
            CreateTexture('R', new List<int> { 2, 0 }, new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 2, 1 }, new List<int> { 6, 1 }, new List<int> { 2, 2 }, new List<int> { 7, 2 }, new List<int> { 2, 3 }, new List<int> { 7, 3 }, new List<int> { 2, 4 }, new List<int> { 7, 4 }, new List<int> { 2, 5 }, new List<int> { 6, 5 }, new List<int> { 2, 6 }, new List<int> { 3, 6 }, new List<int> { 4, 6 }, new List<int> { 5, 6 }, new List<int> { 2, 7 }, new List<int> { 5, 7 }, new List<int> { 2, 8 }, new List<int> { 6, 8 }, new List<int> { 2, 9 }, new List<int> { 7, 9 });
            CreateTexture('S', new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 7, 0 }, new List<int> { 8, 0 }, new List<int> { 3, 1 }, new List<int> { 2, 2 }, new List<int> { 2, 3 }, new List<int> { 3, 4 }, new List<int> { 4, 4 }, new List<int> { 5, 4 }, new List<int> { 6, 4 }, new List<int> { 7, 4 }, new List<int> { 8, 5 }, new List<int> { 8, 6 }, new List<int> { 8, 7 }, new List<int> { 1, 8 }, new List<int> { 7, 8 }, new List<int> { 2, 9 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 }, new List<int> { 6, 9 });
            CreateTexture('T', new List<int> { 1, 0 }, new List<int> { 2, 0 }, new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 7, 0 }, new List<int> { 4, 1 }, new List<int> { 4, 2 }, new List<int> { 4, 3 }, new List<int> { 4, 4 }, new List<int> { 4, 5 }, new List<int> { 4, 6 }, new List<int> { 4, 7 }, new List<int> { 4, 8 }, new List<int> { 4, 9 });
            CreateTexture('U', new List<int> { 1, 0 }, new List<int> { 8, 0 }, new List<int> { 1, 1 }, new List<int> { 8, 1 }, new List<int> { 1, 2 }, new List<int> { 8, 2 }, new List<int> { 1, 3 }, new List<int> { 8, 3 }, new List<int> { 1, 4 }, new List<int> { 8, 4 }, new List<int> { 1, 5 }, new List<int> { 8, 5 }, new List<int> { 1, 6 }, new List<int> { 8, 6 }, new List<int> { 1, 7 }, new List<int> { 8, 7 }, new List<int> { 2, 8 }, new List<int> { 7, 8 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 }, new List<int> { 6, 9 });
            CreateTexture('V', new List<int> { 1, 0 }, new List<int> { 8, 0 }, new List<int> { 1, 1 }, new List<int> { 8, 1 }, new List<int> { 1, 2 }, new List<int> { 7, 2 }, new List<int> { 2, 3 }, new List<int> { 7, 3 }, new List<int> { 2, 4 }, new List<int> { 6, 4 }, new List<int> { 2, 5 }, new List<int> { 6, 5 }, new List<int> { 3, 6 }, new List<int> { 5, 6 }, new List<int> { 3, 7 }, new List<int> { 5, 7 }, new List<int> { 4, 8 }, new List<int> { 4, 9 });
            CreateTexture('W', new List<int> { 0, 1 }, new List<int> { 9, 1 }, new List<int> { 0, 2 }, new List<int> { 9, 2 }, new List<int> { 0, 3 }, new List<int> { 8, 3 }, new List<int> { 1, 4 }, new List<int> { 4, 4 }, new List<int> { 5, 4 }, new List<int> { 8, 4 }, new List<int> { 1, 5 }, new List<int> { 3, 5 }, new List<int> { 5, 5 }, new List<int> { 7, 5 }, new List<int> { 1, 6 }, new List<int> { 3, 6 }, new List<int> { 5, 6 }, new List<int> { 7, 6 }, new List<int> { 1, 7 }, new List<int> { 3, 7 }, new List<int> { 5, 7 }, new List<int> { 7, 7 }, new List<int> { 2, 8 }, new List<int> { 6, 8 }, new List<int> { 2, 9 }, new List<int> { 6, 9 });
            CreateTexture('X', new List<int> { 0, 0 }, new List<int> { 8, 0 }, new List<int> { 1, 1 }, new List<int> { 7, 1 }, new List<int> { 2, 2 }, new List<int> { 6, 2 }, new List<int> { 3, 3 }, new List<int> { 5, 3 }, new List<int> { 4, 4 }, new List<int> { 4, 5 }, new List<int> { 3, 6 }, new List<int> { 5, 6 }, new List<int> { 2, 7 }, new List<int> { 6, 7 }, new List<int> { 1, 8 }, new List<int> { 7, 8 }, new List<int> { 0, 9 }, new List<int> { 8, 9 });
            CreateTexture('Y', new List<int> { 1, 0 }, new List<int> { 8, 0 }, new List<int> { 2, 1 }, new List<int> { 7, 1 }, new List<int> { 3, 2 }, new List<int> { 7, 2 }, new List<int> { 3, 3 }, new List<int> { 6, 3 }, new List<int> { 4, 4 }, new List<int> { 6, 4 }, new List<int> { 4, 5 }, new List<int> { 5, 5 }, new List<int> { 5, 6 }, new List<int> { 4, 7 }, new List<int> { 4, 8 }, new List<int> { 4, 9 });
            CreateTexture('Z', new List<int> { 1, 0 }, new List<int> { 2, 0 }, new List<int> { 3, 0 }, new List<int> { 4, 0 }, new List<int> { 5, 0 }, new List<int> { 6, 0 }, new List<int> { 7, 0 }, new List<int> { 8, 0 }, new List<int> { 7, 1 }, new List<int> { 6, 2 }, new List<int> { 5, 3 }, new List<int> { 4, 4 }, new List<int> { 4, 5 }, new List<int> { 3, 6 }, new List<int> { 2, 7 }, new List<int> { 1, 8 }, new List<int> { 1, 9 }, new List<int> { 2, 9 }, new List<int> { 3, 9 }, new List<int> { 4, 9 }, new List<int> { 5, 9 }, new List<int> { 6, 9 }, new List<int> { 7, 9 }, new List<int> { 8, 9 });
            CreateTexture(' ');
        }
    }
}