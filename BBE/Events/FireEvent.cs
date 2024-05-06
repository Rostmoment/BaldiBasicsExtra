using BBE.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.WebRequestMethods;

namespace BBE.Events
{
    public class FireEvent : RandomEvent
    {
        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            if (FireFog.IsNull())
            {
                CreateFog();
            }
            base.Initialize(controller, rng);
        }
        public override void Begin()
        {
            base.Begin();
            List<Cell> cells = ec.AllTilesNoGarbage(false, false);
            for (int i = 0; i < cells.Count; i++)
            {
                if (i % 5 == 0)
                {
                    SpawnFire(cells[i]);
                }
            }
            ec.AddFog(FireFog);
        }
        public override void End()
        {
            ec.RemoveFog(FireFog);
            foreach (GameObject fire in Fires)
            {
                Destroy(fire);
            }
            base.End();
        }
        public void CreateFog()
        {
            FireFog = new Fog();
            FireFog.startDist = 5f;
            FireFog.maxDist = 250f;
            FireFog.strength = 25f;
            FireFog.color = Color.red;
        }
        public void SpawnFire(Cell cell)
        {
            GameObject Fire = new GameObject("FireExtraMod");
            FireObject currentFire = Fire.AddComponent<FireObject>();
            currentFire.UpdatePosition(cell);
            Fires.Add(Fire);
        }
        public List<GameObject> Fires = new List<GameObject>();
        public Fog FireFog;
    }
    public class FireObject : MonoBehaviour
    {
        void Update()
        {
            gameObject.transform.LookAt(new Vector3(Camera.main.transform.position.x, 5, Camera.main.transform.position.z));
        }
        void Start()
        {
            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.sprite = AssetsHelper.SpriteFromFile(Path.Combine("Textures", "Events", "Fire.png"), 20);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 5, gameObject.transform.position.z);
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }
        public void UpdatePosition(Cell cell)
        {
            gameObject.transform.position = cell.TileTransform.position;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 5, gameObject.transform.position.z);
        }
        private void OnTriggerEnter(Collider other)
        {

        }
        public BoxCollider collider;
        public SpriteRenderer sprite;
    }
}
