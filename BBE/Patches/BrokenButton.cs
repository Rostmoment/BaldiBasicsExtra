using BBE.Extensions;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace BBE.Patches
{
    class BrokenButton : GameButton
    {
        private ParticleSystem sparks;
        private int left = -1;
        public override void Pressed(int playerNumber)
        {
            if (left == 0)
                return;
            if (left < 0)
                left = new System.Random(CoreGameManager.Instance.seed).Next(1, 4);
            left--;
            base.Pressed(playerNumber);
            if (left == 0)
            {
                audMan.PlaySingle("BrokenButtonExplosion");
                SetupSparks();
            }
        }
        private void SetupSparks()
        {
            GameObject sparksObject = new GameObject("SparksEffect");
            sparksObject.transform.position = transform.position;
            sparks = sparksObject.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = sparks.main;
            main.startColor = Color.yellow;
            main.startSpeed = 5f;
            main.startSize = 0.1f;
            main.startLifetime = 0.2f;

            ParticleSystem.EmissionModule emission = sparks.emission;
            emission.rateOverTime = 0;
            emission.burstCount = 1;
            emission.SetBurst(0, new ParticleSystem.Burst(0f, 10));

            ParticleSystem.ShapeModule shape = sparks.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            ParticleSystemRenderer renderer = sparks.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Sprites/Default"));
            sparks.Play();
        }
    }
}
