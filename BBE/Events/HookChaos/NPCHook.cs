using BBE.Helpers;
using BBE.Patches;
using MTM101BaldAPI.AssetTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace BBE.Events.HookChaos
{
    public class NPCHook : MonoBehaviour
    {
        //consts

        private static Material LineMaterial = AssetsHelper.LoadAsset<Material>("BlackBehind");

        private static Sprite HookSprite = BasePlugin.Instance.asset.Get<Sprite>("GrapplingHookSprite");

        private static Sprite CracksSprite = BasePlugin.Instance.asset.Get<Sprite>("GrappleCracks");

        private float speed = 300f;

        private float maxPressure = 300f;

        private float initialForce = 25f;

        private float forceIncrease = 20f;

        private float stopDistance = 15f;

        //end

        private MovementModifier moveMod;

        private Rigidbody rigidbody;

        private LineRenderer lineRenderer;

        private NPC character;

        private EnvironmentController ec;

        private LayerMaskObject layerMask;

        private AudioSource motorAudio;

        private AudioManager audMan;


        private Vector3[] positions = new Vector3[2];

        public float force;

        public float pressure;

        public float initialDistance;

        public float time;

        private bool locked;

        private bool snapped;

        private Transform cracks; //sprite which draws on wall

        private void preInitialize() //initialize all components
        {
            moveMod = new MovementModifier(Vector3.zero, 1f);

            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.angularDrag = 0;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.constraints = (RigidbodyConstraints)116;
            rigidbody.freezeRotation = true;
            rigidbody.inertiaTensor = Vector3.zero;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidbody.mass = 0;
            rigidbody.useGravity = false;

            gameObject.tag = "GrapplingHook";
            gameObject.layer = 13; //"Entities" layer

            lineRenderer = new GameObject("LineRenderer").AddComponent<LineRenderer>();
            lineRenderer.transform.parent = transform;
            lineRenderer.startWidth = 0.1977f;
            lineRenderer.endWidth = 0.1977f;
            lineRenderer.allowOcclusionWhenDynamic = false;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.material = LineMaterial;
            lineRenderer.gameObject.layer = 13; //"Entities" layer

            layerMask = ScriptableObject.CreateInstance<LayerMaskObject>();
            layerMask.mask = 2113537;

            SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
            sphere.radius = 0.5f;
            sphere.isTrigger = true;

            SphereCollider _sphere = gameObject.AddComponent<SphereCollider>();
            _sphere.radius = 0.25f;
            _sphere.isTrigger = false;

            SpriteRenderer spriteRenderer = new GameObject("Sprite").AddComponent<SpriteRenderer>();
            spriteRenderer.transform.parent = transform;
            spriteRenderer.sprite = HookSprite;
            spriteRenderer.size = new Vector2(0.64f, 0.64f);
            spriteRenderer.gameObject.layer = 9; //"Billboard"

            SpriteRenderer cracksRenderer = new GameObject("Cracks").AddComponent<SpriteRenderer>();
            cracks = cracksRenderer.gameObject.transform;
            cracks.parent = this.transform;
            cracksRenderer.sprite = CracksSprite;
            cracksRenderer.gameObject.layer = 9; //"Billboard"
            cracks.gameObject.SetActive(false);

            motorAudio = new GameObject("motorAudio").AddComponent<AudioSource>();
            motorAudio.loop = true;
            motorAudio.clip = BasePlugin.Instance.asset.Get<AudioClip>("GrappleLoopSound");
            motorAudio.transform.parent = transform;
            motorAudio.spatialBlend = 1f;
            motorAudio.maxDistance = 500f;
            motorAudio.rolloffMode = AudioRolloffMode.Custom;

            audMan = gameObject.AddComponent<AudioManager>();
            audMan.audioDevice = gameObject.AddComponent<AudioSource>();
            audMan.audioDevice.spatialBlend = 1f;
            audMan.audioDevice.maxDistance = 100f;
            audMan.audioDevice.rolloffMode = AudioRolloffMode.Custom;
        }

        public void initialize(NPC character, EnvironmentController ec)
        {
            preInitialize();
            this.character = character;
            this.ec = ec;

            transform.position = character.transform.position;
            transform.LookAt(ec.Players.First().transform);
            //base.transform.rotation = Quaternion.Euler(new Vector3(0, character.transform.rotation.eulerAngles.y, 0));
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
            character.GetComponent<ActivityModifier>().moveMods.Add(moveMod);
            audMan.PlaySingle("GrappleLaunch");
        }

        private void Update()
        {
            Vector3 characterPosition = new Vector3(character.transform.position.x, 5, character.transform.position.z); //на всякий
            if (!locked)
            {
                rigidbody.velocity = transform.forward * speed * ec.EnvironmentTimeScale;
                time += Time.deltaTime * ec.EnvironmentTimeScale;
                if (time > 60f)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if ((transform.position - characterPosition).magnitude <= stopDistance)
                {
                    StartCoroutine(EndDelay());
                }

                moveMod.movementAddend = (transform.position - characterPosition).normalized * force;
                if (!snapped)
                {
                    motorAudio.gameObject.transform.position = characterPosition;
                    motorAudio.pitch = (force - initialForce) / 100f + 1f;
                }

                force += forceIncrease * Time.deltaTime;
                pressure = (transform.position - characterPosition).magnitude - (initialDistance - force);
                if (pressure > maxPressure && !snapped)
                {
                    Break();
                }
            }

            positions[0] = transform.position;
            positions[1] = characterPosition - Vector3.up * 1f;
            lineRenderer.SetPositions(positions);
        }

        private void OnCollisionEnter(Collision collision)
        {     
            if (layerMask.Contains(collision.gameObject.layer) && !locked && collision.gameObject.tag != "Player" && collision.gameObject.tag != "NPC")
            {
                locked = true;
                force = initialForce;
                initialDistance = (transform.position - character.transform.position).magnitude;
                rigidbody.velocity = Vector3.zero;
                audMan.PlaySingle("GrappleClang");
                motorAudio.Play();
                cracks.rotation = Quaternion.LookRotation(collision.contacts[0].normal * -1f, Vector3.up);
                cracks.gameObject.SetActive(value: true);
            }
        }

        public void Break()
        {
            snapped = true;
            audMan.FlushQueue(endCurrent: true);
            audMan.QueueAudio("BAL_Break");
            motorAudio.Stop();
            lineRenderer.enabled = false;
            character.GetComponent<ActivityModifier>().moveMods.Remove(moveMod);
            StartCoroutine(WaitForAudio());
        }

        private void End()
        {
            character.GetComponent<ActivityModifier>().moveMods.Remove(moveMod);
            Destroy(gameObject);
        }

        private IEnumerator EndDelay()
        {
            float time = 0.25f;
            while (time > 0f)
            {
                time -= Time.deltaTime;
                yield return null;
            }

            End();
        }

        private IEnumerator WaitForAudio()
        {
            while (audMan.audioDevice.isPlaying)
            {
                yield return null;
            }

            End();
            yield break;
        }


    }

}
