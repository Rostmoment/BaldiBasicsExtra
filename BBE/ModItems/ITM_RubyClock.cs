using BBE.Extensions;
using BBE.Creators;
using MTM101BaldAPI;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using System;
using System.Collections;
using UnityEngine;
using BBE.Helpers;

namespace BBE.ModItems
{
    class ITM_RubyClock : Item, IClickable<int>, IPrefab
    {

        public void SetupAssets()
        {
        }
        // Just for interface
        public bool ClickableHidden() => false;
        public bool ClickableRequiresNormalHeight() => true;
        public void ClickableSighted(int player) { }
        public void ClickableUnsighted(int player) { }

        // Values
        private float[] setTime = new float[]
        {
            5f,
            10f,
            15f,
            20f,
            25f,
            30f,
            35f,
            40f,
            45f,
            50f,
            55f,
            60f
        };
        private GameObject spriteObject;
        private EnvironmentController ec;
        private AudioManager audMan;
        private Entity entity;
        private SoundObject audRing = ((ITM_AlarmClock)ItemMetaStorage.Instance.FindByEnum(Items.AlarmClock).value.item).audRing;
        private SoundObject audWind = ((ITM_AlarmClock)ItemMetaStorage.Instance.FindByEnum(Items.AlarmClock).value.item).audWind;
        private SpriteRenderer spriteRenderer;
        private Sprite[] sprites = new Sprite[12];
        private float time;
        private int initSetTime = 5;
        private int noiseVal = 126;
        private NoLateIcon mapIcon;
        public static ItemObject disablerPrefab;
        public bool finished;

        public override bool Use(PlayerManager pm)
        {
            spriteObject = new GameObject("SpriteObject");
            for (int i = 5; i <= 60; i += 5) 
            {
                sprites[(i / 5) - 1] = AssetsHelper.CreateTexture("Textures", "Items", "RubyClock", "BBE_RubyClock" + i + ".png").ToSprite(15);
            }
            spriteRenderer = spriteObject.AddSpriteRender(sprites[initSetTime]);
            spriteObject.transform.SetParent(this.gameObject.transform, false);
            spriteObject.layer = LayerMask.NameToLayer("Billboard");
            spriteRenderer.material = new Material(ObjectCreators.SpriteMaterial);
            finished = false;
            ec = pm.ec;
            mapIcon = Instantiate(AssetsHelper.LoadAsset<NoLateIcon>(x => x.timeText != null));
            mapIcon.spriteRenderer.sprite = BasePlugin.Asset.Get<Sprite>("RubyClockIcon");
            mapIcon.gameObject.SetActive(true);
            mapIcon = (NoLateIcon)ec.map.AddIcon(mapIcon, transform, Color.white);
            mapIcon.timeText.color = Color.red;
            transform.position = pm.transform.position;
            audMan = gameObject.AddAudioManager();
            gameObject.transform.position = gameObject.transform.position.Change(y: 4);
            entity = gameObject.CreateEntity(2, 2, spriteObject.transform);
            entity.collider.enabled = false;
            entity.Initialize(ec, transform.position);
            StartCoroutine(Timer(setTime[initSetTime]));
            return true;
        }
        void Update()
        {
            // Stupid, but it works
            if (mapIcon != null)
            {
                mapIcon.UpdatePosition(ec.map);
                mapIcon.timeText.SetText(time.ToString("F2"));
            }
            if (entity != null && entity.collider.enabled) entity.collider.enabled = false;
        }
        private IEnumerator Timer(float timeInit)
        {
            time = timeInit;
            while (time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                if (time <= setTime[0]) spriteRenderer.sprite = sprites[0];
                else if (time <= setTime[1]) spriteRenderer.sprite = sprites[1];
                else if (time <= setTime[2]) spriteRenderer.sprite = sprites[2];
                else if (time <= setTime[3]) spriteRenderer.sprite = sprites[3];
                else if (time <= setTime[4]) spriteRenderer.sprite = sprites[4];
                else if (time <= setTime[5]) spriteRenderer.sprite = sprites[5];
                else if (time <= setTime[6]) spriteRenderer.sprite = sprites[6];
                else if (time <= setTime[7]) spriteRenderer.sprite = sprites[7];
                else if (time <= setTime[8]) spriteRenderer.sprite = sprites[8];
                else if (time <= setTime[9]) spriteRenderer.sprite = sprites[9];
                else if (time <= setTime[10]) spriteRenderer.sprite = sprites[10];
                else spriteRenderer.sprite = sprites[11];
                yield return null;
            }
            FinishTimer();
        }
        private IEnumerator Finisher()
        {
            ec.MakeNoise(base.transform.position, noiseVal);
            audMan.FlushQueue(true);
            audMan.PlaySingle(audRing);
            while (audMan.QueuedAudioIsPlaying)
            {
                yield return null;
            }
            mapIcon.gameObject.SetActive(false); 
            Destroy(spriteObject);
            
            Destroy(base.gameObject);
        }
        public void FinishTimer()
        {
            time = 0f;
            finished = true;
            spriteRenderer.sprite = sprites[11];
            StartCoroutine(Finisher());
        }
        public void Clicked(int player)
        {
            if (!finished)
            {
                audMan.PlaySingle(audWind);
                if (time <= setTime[0]) time = setTime[1];
                else if (time <= setTime[1]) time = setTime[2];
                else if (time <= setTime[2]) time = setTime[3];
                else if (time <= setTime[3]) time = setTime[4];
                else if (time <= setTime[4]) time = setTime[5];
                else if (time <= setTime[5]) time = setTime[6];
                else if (time <= setTime[6]) time = setTime[7];
                else if (time <= setTime[7]) time = setTime[8];
                else if (time <= setTime[8]) time = setTime[9];
                else if (time <= setTime[9]) time = setTime[10];
                else if (time <= setTime[10]) time = setTime[11];
                else time = setTime[0];
            }
        }
    }
}
