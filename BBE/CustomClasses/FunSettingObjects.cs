using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MTM101BaldAPI.Registers;
using BBE.Creators;
using System.Collections;
using TMPro;
using BBE.API;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.XR;
using BBE.Extensions;
using MTM101BaldAPI.UI;

namespace BBE.CustomClasses
{
    public enum Operator
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }
    public class YCTP : MonoBehaviour
    {
        public static Dictionary<int, UnityAction<YCTP>> yctpCheatCodes = new Dictionary<int, UnityAction<YCTP>>();
        private GameObject canvas;
        private int firstNum = 0;
        private int secondNum = 0;
        private int answer = 0;
        protected int currentProblem = 1;
        public GameObject objectToDestroy;
        public TMP_Text playerAnswer;
        public TMP_Text problemText;
        public TMP_Text solveText;
        public bool isActive = true;
        private Operator operatorProblem;
        protected int wrongTotal = 0;
        private EnvironmentController ec;
        protected List<string> wrongAnswers = new List<string>() { "", "-", " " };
        protected List<GameObject> marks = new List<GameObject> { };

        void Start()
        {
            Setup();
            OpenPad();
            Time.timeScale = 0;
        }
        void Update()
        {
            try
            {
                playerAnswer.ChangeizeTextContainerState(true);
                problemText.ChangeizeTextContainerState(true);
                solveText.ChangeizeTextContainerState(true);
            }
            catch (NullReferenceException)
            {

            }
            if (!isActive)
            {
                return;
            }
            if (FunSettingsType.CSSEAFS.IsActive())
            {
                playerAnswer.text = "";
                CheckCheatCode();
                CheckAnswer();
                if (currentProblem > 3)
                {
                    DestroyText();
                    isActive = false;
                    ClosePad();
                }
                else
                {
                    GenerateProblem();
                }
            }
            if (Input.anyKeyDown)
            {
                if (Input.inputString.Length > 0 && char.IsNumber(Input.inputString[0]))
                {
                    string tmp = playerAnswer.text;
                    if (tmp.StartsWith("-"))
                    {
                        tmp = tmp.Remove(0, 1);
                    }
                    if (tmp.Length < 9)
                    {
                        playerAnswer.text += Input.inputString[0];
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
                {
                    if (playerAnswer.text.StartsWith("-"))
                    {
                        playerAnswer.text = playerAnswer.text.Remove(0, 1);
                    }
                    else
                    {
                        playerAnswer.text = "-" + playerAnswer.text;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Backspace) && playerAnswer.text.Length > 0)
                {
                    playerAnswer.text = playerAnswer.text.Substring(0, playerAnswer.text.Length - 1);
                }
                else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    CheckCheatCode();
                    CheckAnswer();
                    if (currentProblem > 3)
                    {
                        DestroyText();
                        isActive = false;
                        ClosePad();
                    }
                    else
                    {
                        GenerateProblem();
                    }
                }
            }
        }
        public void CheckCheatCode()
        {
            if (wrongAnswers.Contains(playerAnswer.text))
            {
                return;
            }
            int answer = int.Parse(playerAnswer.text);
            if (yctpCheatCodes.ContainsKey(answer))
            {
                yctpCheatCodes[answer](this);
            }
        }
        public void HideHUD(bool hide)
        {
            if (hide)
            {
                SubtitleManager.Instance.DestroyAll();
            }
            CoreGameManager.Instance.GetHud(0).Hide(hide);
        }
        public virtual void Setup()
        {
            HideHUD(true);
            currentProblem = 1;
            wrongTotal = 0;
            ec = BaseGameManager.Instance.Ec;
            canvas = CreateObjects.CreateCanvas("YCTP_Canvas_ExtraMod", sprite: BasePlugin.Asset.Get<Sprite>("YCTPCanvas"));
            playerAnswer = CreateObjects.CreateText("PlayerAnswer_Text", "", true, new Vector3(-54, -77.4f, -36), new Vector3(10, 10, 10), canvas.transform, BaldiFonts.ComicSans24);
            playerAnswer.isOrthographic = false;
            problemText = CreateObjects.CreateText("ProblemText_Text", "", true, new Vector3(-90, -5, -36), new Vector3(10, 10, 10), canvas.transform, BaldiFonts.ComicSans18);
            problemText.isOrthographic = false;
            solveText = CreateObjects.CreateText("SolveText", "Test", true, new Vector3(-90, 50.4f, -36), new Vector3(10, 10, 10), canvas.transform, BaldiFonts.ComicSans18);
            solveText.isOrthographic = false;
            for (int i = 0; i < 3; i++)
            {
                GameObject game = new GameObject("Checkmark");
                Image res = game.AddComponent<Image>();
                res.sprite = BasePlugin.Asset.Get<Sprite>("CheckMark");
                game.transform.SetParent(canvas.transform);
                game.SetActive(false);
                game.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                float y = 0;
                switch (i)
                {
                    case 0:
                        y = 1.04f;
                        break;
                    case 1:
                        y = 0.04f;
                        break;
                    case 2:
                        y = -1.04f;
                        break;
                }
                game.transform.position = new Vector3(-3.7f, y, 1);
                marks.Add(game);
            }
        }
        public virtual void GenerateProblem()
        {
            solveText.text = "BBE_SolveMath".Localize() + currentProblem;
            operatorProblem = new Operator[] { Operator.Addition, Operator.Subtraction }.ChooseRandom();
            playerAnswer.text = "";
            firstNum = UnityEngine.Random.Range(0, 10);
            secondNum = UnityEngine.Random.Range(0, 10);
            string strOperator = "";
            string end = "=?";
            switch (operatorProblem)
            {
                case Operator.Addition:
                    answer = firstNum + secondNum;
                    strOperator = "+";
                    break;
                case Operator.Subtraction:
                    answer = firstNum - secondNum;
                    strOperator = "-";
                    break;
                case Operator.Multiplication:
                    answer = firstNum * secondNum;
                    strOperator = "×";
                    break;
                case Operator.Division:
                    GenerateDivision();
                    answer = firstNum / secondNum;
                    strOperator = "÷";
                    break;
                default:
                    throw new Exception("BRO WHAT THE HECK ARE YOU TRY TO DO!?!?!?!??!");

            }
            if (secondNum < 0)
            {
                strOperator += "(";
                end = ")=?";
            }
            problemText.text = firstNum + strOperator + secondNum + end;
        }
        public IEnumerator FadeOff()
        {
            float x = 10;
            while (x > 0)
            {
                x -= 0.1f;
                yield return null;
            }
            foreach (GameObject data in marks)
            {
                Destroy(data);
            }
            Image canvasImage = canvas.GetComponentInChildren<Image>();
            while (canvasImage.color.a > 0)
            {
                float alpha = canvasImage.color.a;
                alpha -= 0.05f;
                if (alpha < 0)
                {
                    alpha = 0;
                }
                canvasImage.color = canvasImage.color.Change(a: alpha);
                yield return null;
            }
            DestroyCanvas();
            Destroy(solveText);
            Destroy(solveText.gameObject);
            HideHUD(false);
            Time.timeScale = 1;
            CoreGameManager.Instance.disablePause = false;
            if (wrongTotal == 3)
            {
                ec.MakeNoise(CoreGameManager.Instance.GetPlayer(0).transform.position, 79);
            }
            yield break;
        }
        public void GenerateDivision()
        {
            List<List<int>> possible = new List<List<int>>() { };
            for (int x = -9; x <= 9; x++)
            {
                for (int y = -9; y <= 9; y++)
                {
                    if (x != 0 && y != 0 && x % y == 0)
                    {
                        possible.Add(new List<int> { x, y });
                    }
                }
            }
            List<int> res = possible.ChooseRandom();
            firstNum = res[0];
            secondNum = res[1];
        }
        public virtual void CheckAnswer()
        {
            GameObject game = marks[currentProblem - 1];
            if (wrongAnswers.Contains(playerAnswer.text) || int.Parse(playerAnswer.text) != answer)
            {
                game.GetComponent<Image>().sprite = BasePlugin.Asset.Get<Sprite>("CrossMark");
                wrongTotal++;
            }
            game.SetActive(true);
            currentProblem++;
        }
        public void DestroyText()
        {
            Destroy(playerAnswer);
            Destroy(playerAnswer.gameObject);
            Destroy(problemText);
            Destroy(problemText.gameObject);
        }
        public void DestroyCanvas()
        {
            Destroy(canvas);
            Destroy(objectToDestroy);
            Destroy(gameObject);
        }
        public void OpenPad()
        {
            InputManager.Instance.ActivateActionSet("Interface");
            CoreGameManager.Instance.disablePause = true;
            GenerateProblem();
        }
        private IEnumerator Wait(float t, bool quit = false)
        {
            float left = t;
            while (left > 0)
            {
                left -= Time.unscaledDeltaTime;
                yield return null;
            }
            ClosePad(quit);
        }
        public void ClosePad(float time, bool quit = false) => StartCoroutine(Wait(time, quit));
        public void ClosePad(bool quit = false)
        {
            if (FunSettingsType.HardMode.IsActive() && ec.GetBaldi() != null)
            {
                ec.GetBaldi().GetAngry(wrongTotal * 0.333f);
            }
            else if (ec.GetBaldi() != null)
            {
                ec.GetBaldi().GetExtraAnger(wrongTotal * 0.333f);
            }
            string textToSet = "";
            if (wrongTotal == 0)
            {
                if (BaseGameManager.Instance is EndlessGameManager)
                {
                    textToSet = LocalizationManager.Instance.GetLocalizedText("BBE_YCTP_Recover" + UnityEngine.Random.Range(1, 3));
                }
                else
                {
                    textToSet = "BBE_YCTP_Victory1".Localize();
                }
            }
            else
            {
                textToSet = LocalizationManager.Instance.GetLocalizedText("BBE_YCTP_Fail" + UnityEngine.Random.Range(1, 3));
            }
            solveText.text = textToSet;
            InputManager.Instance.ActivateActionSet("InGame");
            if (!quit)
                StartCoroutine(FadeOff());
            else
                CoreGameManager.Instance.Quit();
        }
    }
}
