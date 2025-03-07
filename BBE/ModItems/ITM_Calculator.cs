using BBE.Extensions;
using BBE.Patches;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace BBE.ModItems
{
    public class ITM_Calculator : Item
    {
        // Shows the correct answer on all math machines, and if there are multiple questions, answers all but one of them
        public override bool Use(PlayerManager pm)
        {
            pm.RuleBreak("usingCalculator", 1.6f);

            foreach (MathMachine mathMachine in FindObjectsOfType<MathMachine>())
            {
                HandleMathMachine(mathMachine);
            }

            Destroy(gameObject);
            return true;
        }

        private void HandleMathMachine(MathMachine mathMachine)
        {
            int totalProblems = mathMachine.totalProblems;
            int answeredProblems = mathMachine.answeredProblems;
            if (totalProblems > 1 && totalProblems != answeredProblems)
            {
                mathMachine.SetProblemsCount(totalProblems, totalProblems-1);
            }
            if (NotEqualMathMachines.machines.Contains(mathMachine))
            {
                mathMachine.answerText.text = mathMachine.currentNumbers.EmptyOrNull()
                    ? mathMachine.answer.ToString()
                    : mathMachine.currentNumbers.Where(x => x.value != mathMachine.answer).ChooseRandom().Value.ToString();
                SetMachineNumballs(mathMachine, true);
            }
            else
            {
                mathMachine.answerText.text = mathMachine.answer.ToString();
                SetMachineNumballs(mathMachine, false);
            }

            mathMachine.answerText.autoSizeTextContainer = true;
        }

        private void SetMachineNumballs(MathMachine mathMachine, bool isNotEqualMachine)
        {
            foreach (MathMachineNumber numball in mathMachine.currentNumbers)
            {
                bool isCorrectAnswer = numball.Value == mathMachine.answer;
                bool useGreenBalloon = isNotEqualMachine ? !isCorrectAnswer : isCorrectAnswer;
                SetNumballSprite(numball, mathMachine.corrupted, useGreenBalloon);
            }
        }

        private void SetNumballSprite(MathMachineNumber numball, bool isCorrupted, bool useGreenBalloon)
        {
            string balloonColor = isCorrupted || !useGreenBalloon ? "BalloonRed" : "BalloonGreen";
            string spriteName = $"{balloonColor}{numball.Value}";

            if (BasePlugin.Asset.Exists<Sprite>(spriteName, out Sprite sprite))
                numball.sprite.GetComponent<SpriteRenderer>().sprite = sprite;
            else
                BasePlugin.Logger.LogWarning($"Unknown NumBall {numball.Value}");
        }
    }
}
