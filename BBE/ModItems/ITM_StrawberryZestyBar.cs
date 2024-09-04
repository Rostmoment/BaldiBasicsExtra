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
        Sprite previousSprite = null;
        ValueModifier staminaRaiseModifier = new ValueModifier(0f, -3f);
        SoundObject audEat = ((ITM_ZestyBar)ItemMetaStorage.Instance.FindByEnum(Items.ZestyBar).value.item).audEat;
        public override bool Use(PlayerManager pm)
        {
            HudManager hudMan = Singleton<CoreGameManager>.Instance.GetHud(0);
            Image stamImage = hudMan.transform.Find("Staminometer").Find("Overlay").GetComponent<Image>();
            previousSprite = stamImage.sprite;
            stamImage.sprite = BasePlugin.Instance.asset.Get<Sprite>("StrawberryStamina");
            pm.plm.stamina = pm.plm.staminaMax * 3f;
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(audEat);
            StartCoroutine(Timer(pm));
            return true;
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
            HudManager hudMan = Singleton<CoreGameManager>.Instance.GetHud(0);
            Image stamImage = hudMan.transform.Find("Staminometer").Find("Overlay").GetComponent<Image>();
            stamImage.sprite = previousSprite;
            previousSprite = null;
            Destroy(base.gameObject);
            yield break;
        }
    }
}
