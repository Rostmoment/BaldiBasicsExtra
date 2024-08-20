using HarmonyLib;
using System.Linq;
using UnityEngine;
namespace BBE.ModItems
{
    public class ITM_Calculator : Item
    {
        // Shows the correct answer on all math machines, and if there are more than one question, answers all but one of them
        public override bool Use(PlayerManager pm)
        {
            pm.RuleBreak("usingCalculator", 1.6f);
            foreach (MathMachine mathMachine in FindObjectsOfType<MathMachine>())
            {
                int totalProblems = mathMachine.totalProblems;
                int answeredProblems = mathMachine.answeredProblems;
                if (totalProblems > 1 && totalProblems != answeredProblems)
                {
                    answeredProblems = totalProblems - 1;
                    mathMachine.answeredProblems = answeredProblems;
                    mathMachine.totalTmp.text = $"<sprite={answeredProblems}><sprite=10><sprite={totalProblems}>";
                }
                mathMachine.answerText.text = mathMachine.answer.ToString();
                mathMachine.answerText.autoSizeTextContainer = false;
                mathMachine.answerText.autoSizeTextContainer = true;
                try
                {
                    mathMachine.currentNumbers.Find(x => x.Value == mathMachine.answer).sprite.GetComponent<SpriteRenderer>().sprite = BasePlugin.Instance.asset.Get<Sprite>("BalloonGreen" + mathMachine.answer);
                }
                catch
                {
                    Debug.LogWarning("Unknown NumBall " + mathMachine.answer);
                }
                mathMachine.currentNumbers.Where(x => x.Value != mathMachine.answer).Do(x => 
                {
                    try
                    {
                        x.sprite.GetComponent<SpriteRenderer>().sprite = BasePlugin.Instance.asset.Get<Sprite>("BalloonRed" + x.Value);
                    }
                    catch
                    {
                        Debug.LogWarning("Unknown NumBall " + x.Value);
                    }
                });
            }
            Destroy(gameObject);
            return true;
        }
    }
}
