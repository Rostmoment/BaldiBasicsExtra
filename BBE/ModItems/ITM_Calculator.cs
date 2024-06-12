using BBE.Helpers;
using UnityEngine;
using TMPro;
using HarmonyLib;
using MTM101BaldAPI.Registers;

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
                int totalProblems = PrivateDataHelper.GetVariable<int>(mathMachine, "totalProblems");
                int answeredProblems = PrivateDataHelper.GetVariable<int>(mathMachine, "answeredProblems");
                if (totalProblems > 1 && totalProblems != answeredProblems)
                {
                    answeredProblems = totalProblems - 1;
                    PrivateDataHelper.SetValue(mathMachine, "answeredProblems", answeredProblems);
                    TMP_Text totalTmp = PrivateDataHelper.GetVariable<TMP_Text>(mathMachine, "totalTmp");
                    totalTmp.text = totalTmp.text = $"<sprite={answeredProblems}><sprite=10><sprite={totalProblems}>";
                    PrivateDataHelper.SetValue(mathMachine, "totalTmp", totalTmp);
                }
                TMP_Text answerText = PrivateDataHelper.GetVariable<TMP_Text>(mathMachine, "answerText");
                answerText.text = PrivateDataHelper.GetVariable<int>(mathMachine, "answer").ToString();
                answerText.autoSizeTextContainer = false;
                answerText.autoSizeTextContainer = true;
                PrivateDataHelper.SetValue(mathMachine, "answerText", answerText);
            }
            Destroy(gameObject);
            return true;
        }
    }
}
