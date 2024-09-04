using UnityEngine;
using System.Collections;
using BBE.Helpers;
using System.IO;
using System.Collections.Generic;

namespace BBE.ModItems
{
    // Create residue that slows down player and NPCs
    public class GlueResidue : MonoBehaviour
    {
        private int PlayerEnterTimes = 0;
        private AudioManager audMan;
        private List<NPC> npcs = new List<NPC>() { };
        private List<PlayerManager> players = new List<PlayerManager>() { };
        public MovementModifier moveMod = new MovementModifier(default(Vector3), 0.3f);
        private SpriteRenderer sprite;
        private BoxCollider collider;
        void Start()
        {
            audMan = gameObject.AddAudioManager();
            PlayerEnterTimes = 0;
            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.sprite = BasePlugin.Instance.asset.Get<Sprite>("GlueResidue");
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(4.5f, 15f, 4.5f);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerManager player = other.GetComponent<PlayerManager>();
                players.Add(player);
                if (PlayerEnterTimes > 2)
                {
                    if (!player.plm.am.moveMods.Contains(moveMod))
                    {
                        audMan.PlaySingle("Ben_Splat", false);
                        player.plm.am.moveMods.Add(moveMod);
                    }
                }
                else
                {
                    PlayerEnterTimes += 1;
                }
            }
            if (other.CompareTag("NPC"))
            {
                NPC npc = other.GetComponent<NPC>();
                npcs.Add(npc);
                ActivityModifier activityModifier;
                if (npc.TryGetComponent<ActivityModifier>(out activityModifier))
                {
                    audMan.PlaySingle("Ben_Splat", false);
                    activityModifier.moveMods.Add(moveMod);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerManager player = other.GetComponent<PlayerManager>();
                if (players.Contains(player))
                {
                    players.Remove(player);
                }
                if (player.plm.am.moveMods.Contains(moveMod))
                {
                    player.plm.am.moveMods.Remove(moveMod);
                }
            }
            if (other.CompareTag("NPC"))
            {
                NPC npc = other.GetComponent<NPC>();
                if (npcs.Contains(npc))
                {
                    npcs.Remove(npc);
                }
                ActivityModifier activityModifier;
                if (npc.TryGetComponent<ActivityModifier>(out activityModifier) && activityModifier.moveMods.Contains(moveMod))
                {
                    activityModifier.moveMods.Remove(moveMod);
                }
            }
        }
        public void SetPosition(PlayerManager player)
        {
            Vector3 pos = new Vector3(player.gameObject.transform.position.x, 0.1f, player.gameObject.transform.position.z);
            Vector3 lookAt = new Vector3(player.gameObject.transform.position.x, 180, player.gameObject.transform.position.z);
            gameObject.transform.position = pos;
            gameObject.transform.LookAt(lookAt);
        }
        public void RemoveMoveMods()
        {
            foreach (NPC npc in npcs)
            {
                ActivityModifier activityModifier;
                if (npc.TryGetComponent<ActivityModifier>(out activityModifier) && activityModifier.moveMods.Contains(moveMod))
                {
                    activityModifier.moveMods.Remove(moveMod);
                }
            }
            foreach (PlayerManager player in players)
            {
                if (player.Am.moveMods.Contains(moveMod))
                {
                    player.Am.moveMods.Remove(moveMod);
                }
            }
        }
    }

    public class ITM_Glue : Item
    {
        public GameObject glueObject;
        public GlueResidue glue;
        public override bool Use(PlayerManager pm)
        {
            glueObject = new GameObject("Glue_ExtraMod");
            glue = glueObject.AddComponent<GlueResidue>();
            glue.SetPosition(pm);
            StartCoroutine(Timer(15));
            return true;
        }
        public IEnumerator Timer(float timeLeft)
        {
            float time = timeLeft;
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            glue.RemoveMoveMods();
            Destroy(glue);
            Destroy(glueObject);
            Destroy(gameObject);
            yield break;
        }
    }
}
