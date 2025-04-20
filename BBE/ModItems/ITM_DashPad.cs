using BBE.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BBE.ModItems
{
    class DashPad : MonoBehaviour
    {
        private static int[] angles = new int[]
        {
            0,
            90,
            180,
            270,
            360
        };
        private const float TIME = 5;
        private const float COOLDOWN_TIME = 10;
        private SpriteRenderer sprite;
        private float time;
        private float cooldown;
        private AudioManager audMan;
        private List<Entity> entities;
        private BoxCollider collider;
        private EnvironmentController ec;
        public bool AnyoneOnPad => entities.Count > 0;

        public void SetPosition(PlayerManager player)
        {
            Cell cell = player.ec.CellFromPosition(player.gameObject.transform.position);
            Vector3 pos = cell.CenterWorldPosition.Change(y: 0.1f);
            gameObject.transform.position = pos;
            gameObject.transform.rotation = CorrectRotation(CoreGameManager.Instance.GetCamera(0).transform.rotation);

        }
        public Quaternion CorrectRotation(Quaternion rotation)
        {
            rotation.eulerAngles = CorrectRotation(rotation.eulerAngles);
            return rotation;
        }
        private Vector3 CorrectRotation(Vector3 rotation)
        {
            return rotation;
        }
        private void Push()
        {
            entities.Do(x => Push(x));
        }
        private void Push(Entity entity)
        {
            entity.AddForce(new Force(transform.forward, 500, -150f));
        }

        private void Start()
        {
            time = float.NaN;
            cooldown = float.NaN;
            entities = new List<Entity>();
            sprite = gameObject.AddSpriteRender("DashPadPlaced");
            audMan = gameObject.AddAudioManager();
            collider = gameObject.AddCollider(new Vector3(5f, 15f, 5f));
            ec = BaseGameManager.Instance.ec;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                entities.Add(other.GetComponent<PlayerManager>().plm.entity);
            }
            if (other.CompareTag("NPC"))
            {
                entities.Add(other.GetComponent<NPC>().GetComponent<Entity>());
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                entities.Remove(other.GetComponent<PlayerManager>().plm.entity);
            }
            if (other.CompareTag("NPC"))
            {
                entities.Remove(other.GetComponent<NPC>().GetComponent<Entity>());
            }
        }
        private void Update()
        {
            if (AnyoneOnPad && float.IsNaN(cooldown))
            {
                if (float.IsNaN(time))
                {
                    time = TIME;
                }
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
            }
            else
            {
                time = float.NaN;
            }
            if (time <= 0 && float.IsNaN(cooldown))
            {
                Push();
                time = float.NaN;
                cooldown = COOLDOWN_TIME;
                audMan.PlaySingle("DashPadPush");
            }
            if (!float.IsNaN(cooldown))
            {
                cooldown -= Time.deltaTime * ec.EnvironmentTimeScale;
                if (cooldown <= 0)
                {
                    cooldown = float.NaN;
                }
            }
        }
    }
    class ITM_DashPad : Item
    {
        public override bool Use(PlayerManager pm)
        {
            new GameObject("BBE_DashPad").AddComponent<DashPad>().SetPosition(pm);
            return true;
        }
    }
}
