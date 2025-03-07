using BBE.Compats;
using BBE.Extensions;
using CarnivalPack;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.PlusExtensions;
using MTM101BaldAPI.Registers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BBE.ModItems
{
    class ITM_StrawberryZestyBar : Item
    {
        private static Sprite previousSprite = null;
        private ValueModifier staminaRaiseModifier;
        private SoundObject AudEat => ((ITM_ZestyBar)ItemMetaStorage.Instance.FindByEnum(Items.ZestyBar).value.item).audEat;
        public override bool Use(PlayerManager pm)
        {
            if (previousSprite != null) return false;
            staminaRaiseModifier = new ValueModifier(0f, -3f);
            HudManager hudMan = CoreGameManager.Instance.GetHud(0);
            Image stamImage = hudMan.transform.Find("Staminometer").Find("Overlay").GetComponent<Image>();
            previousSprite = stamImage.sprite;
            stamImage.sprite = BasePlugin.Asset.Get<Sprite>("StrawberryStamina");
            if (ModIntegration.CarnivalIsInstalled)
                SetCarnivalPackCompat(pm, stamImage);
            pm.plm.stamina = pm.plm.staminaMax * 4.5f;
            CoreGameManager.Instance.audMan.PlaySingle(AudEat);
            StartCoroutine(Timer(pm));
            return true;
        }
        private void SetCarnivalPackCompat(PlayerManager pm, Image stamImage)
        {
            if (pm.gameObject.HasComponent<CottonCandyManager>())
            {
                stamImage.sprite = BasePlugin.Asset.Get<Sprite>("StrawberryStaminaAndCottonCandy");
            }
        }
        private void SetOldSprite()
        {
            if (previousSprite == null) return;
            HudManager hudMan = CoreGameManager.Instance.GetHud(0);
            Image stamImage = hudMan.transform.Find("Staminometer").Find("Overlay").GetComponent<Image>();
            stamImage.sprite = previousSprite;
            previousSprite = null;
        }
        public IEnumerator Timer(PlayerManager pm)
        {
            pm.GetMovementStatModifier().AddModifier("staminaRise", staminaRaiseModifier);
            while (pm.plm.stamina > 0)
            {
                pm.plm.stamina = Mathf.Max(pm.plm.stamina - pm.plm.staminaDrop * Time.deltaTime * pm.PlayerTimeScale, 0f);
                yield return null;
            }
            pm.GetMovementStatModifier().RemoveModifier(staminaRaiseModifier);
            SetOldSprite();
            Destroy(base.gameObject);
            yield break;
        }
        void OnDestroy() => SetOldSprite();
    }
}
