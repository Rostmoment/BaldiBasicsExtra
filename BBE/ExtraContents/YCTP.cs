using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BBE.Helpers;
using System.IO;
using System;
using HarmonyLib;
using System.Collections;
using System.Threading;

namespace BBE.ExtraContents
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
        private GameObject canvas;
        private int FirstNum = 0;
        private int SecondNum = 0;
        private int answer = 0;
        private int currentProblem = 1;
        public GameObject ObjectToDestroy;
        private TMP_Text PlayerAnswer;
        private TMP_Text ProblemText;
        private bool IsActive = true;
        private Operator operatorProblem;
        private int WrongTotal = 0;
        private EnvironmentController ec;
        private List<string> WrongAnswers = new List<string>() { "", "-", " " };
        private List<GameObject> Marks = new List<GameObject> { };

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
                PlayerAnswer.ChangeizeTextContainerState(true);
                ProblemText.ChangeizeTextContainerState(true);
            }
            catch (NullReferenceException)
            {

            }
            if (!IsActive)
            {
                return;
            }
            if (Input.anyKeyDown)
            {
                if (Input.inputString.Length > 0 && char.IsNumber(Input.inputString[0]))
                {
                    string tmp = PlayerAnswer.text;
                    if (tmp.StartsWith("-"))
                    {
                        tmp = tmp.Remove(0, 1);
                    }
                    if (tmp.Length < 10)
                    {
                        PlayerAnswer.text += Input.inputString[0];
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
                {
                    if (PlayerAnswer.text.StartsWith("-"))
                    {
                        PlayerAnswer.text = PlayerAnswer.text.Remove(0, 1);
                    }
                    else
                    {
                        PlayerAnswer.text = "-" + PlayerAnswer.text;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Backspace) && PlayerAnswer.text.Length > 0)
                {
                    PlayerAnswer.text = PlayerAnswer.text.Substring(0, PlayerAnswer.text.Length - 1);
                }
                else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    CheckAnswer();
                    if (currentProblem > 3)
                    {
                        IsActive = false;
                        ClosePad();
                    }
                    else
                    {
                        GenerateProblem();
                    }
                }
            }
        }
        public void HideHUD(bool hide)
        {
            Singleton<CoreGameManager>.Instance.GetHud(0).Hide(hide);
        }
        public void Setup()
        {
            HideHUD(true);
            currentProblem = 1;
            WrongTotal = 0;
            ec = Singleton<BaseGameManager>.Instance.Ec;
            canvas = UnityEngine.Object.Instantiate(Prefabs.Canvas);
            canvas.name = "YCTP_Canvas_ExtraMod";
            Image canvasImage = canvas.GetComponentInChildren<Image>();
            canvasImage.sprite = BasePlugin.Instance.asset.Get<Sprite>("YCTPCanvas");
            Canvas yctpCanvas = canvas.GetComponent<Canvas>();
            canvas.SetActive(true);
            yctpCanvas.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
            PlayerAnswer = CreateObjects.CreateText("PlayerAnswer_Text", "", true, new Vector3(-1.5f, -2.15f, 1), new Vector3(0.277f, 0.277f, 0.277f), canvas.transform, 24f);
            PlayerAnswer.isOrthographic = false;
            ProblemText = CreateObjects.CreateText("ProblemText_Text", "", true, new Vector3(-2.5f, 0, 1), new Vector3(0.277f, 0.277f, 0.277f), canvas.transform, 24f);
            ProblemText.isOrthographic = false;
            for (int i = 0; i < 3; i++)
            {
                GameObject game = new GameObject("Checkmark");
                Image res = game.AddComponent<Image>();
                res.sprite = BasePlugin.Instance.asset.Get<Sprite>("CheckMark");
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
                Marks.Add(game);

            }
        }
        public void GenerateProblem()
        {
            operatorProblem = Operator.Addition;
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                operatorProblem = Operator.Subtraction;
            }
            if (FunSettingsManager.HardMode)
            {
                operatorProblem = Operator.Multiplication;
                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    operatorProblem = Operator.Division;
                }
            }
            PlayerAnswer.text = "";
            FirstNum = UnityEngine.Random.Range(-9, 9);
            SecondNum = UnityEngine.Random.Range(-9, 9);
            string strOperator = "";
            string end = "=?";
            switch (operatorProblem)
            {
                case Operator.Addition:
                    answer = FirstNum + SecondNum;
                    strOperator = "+";
                    break;
                case Operator.Subtraction:
                    answer = FirstNum - SecondNum;
                    strOperator = "-";
                    break;
                case Operator.Multiplication:
                    answer = FirstNum * SecondNum;
                    strOperator = "×";
                    break;
                case Operator.Division:
                    GenerateDivision();
                    answer = FirstNum / SecondNum;
                    strOperator = "÷";
                    break;
                default:
                    throw new Exception("BRO WHAT THE HECK ARE YOU TRY TO DO!?!?!?!??!");

            }
            if (SecondNum < 0)
            {
                strOperator += "(";
                end = ")=?";
            }
            ProblemText.text = FirstNum + strOperator + SecondNum + end;
        }
        public IEnumerator FadePad()
        {
            yield return new WaitForSeconds(1);
            DestroyText();
            Image canvasImage = canvas.GetComponentInChildren<Image>();
            while (canvasImage.color.a > 0)
            {
                float alpha = canvasImage.color.a;
                alpha -= 0.05f;
                if (alpha < 0)
                {
                    alpha = 0;
                }
                canvasImage.color = new Color(canvasImage.color.r, canvasImage.color.g, canvasImage.color.b, alpha);
                yield return null;
            }
            DestroyCanvas();
            HideHUD(false);
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
            FirstNum = res[0];
            SecondNum = res[1];
        }
        public void CheckAnswer()
        {
            GameObject game = Marks[currentProblem - 1];
            if (WrongAnswers.Contains(PlayerAnswer.text) || int.Parse(PlayerAnswer.text) != answer)
            {
                game.GetComponent<Image>().sprite = BasePlugin.Instance.asset.Get<Sprite>("CrossMark");
                WrongTotal++;
            }
            game.SetActive(true);
            currentProblem++;
        }
        public void DestroyText()
        {
            Destroy(PlayerAnswer);
            Destroy(PlayerAnswer.gameObject);
            Destroy(ProblemText);
            Destroy(ProblemText.gameObject);
        }
        public void DestroyCanvas()
        {
            foreach (GameObject data in Marks)
            {
                Destroy(data);
            }
            Destroy(canvas);
            Destroy(ObjectToDestroy);
            Destroy(gameObject);
        }
        public void OpenPad()
        {
            Singleton<InputManager>.Instance.ActivateActionSet("Interface");
            Singleton<CoreGameManager>.Instance.disablePause = true;
            GenerateProblem();
        }
        public void ClosePad()
        {
            if (FunSettingsManager.HardMode && !ec.GetBaldi().IsNull())
            {
                ec.GetBaldi().GetAngry(WrongTotal * 0.333f);
            }
            else if (!ec.GetBaldi().IsNull())
            {
                ec.GetBaldi().GetExtraAnger(WrongTotal * 0.333f);
            }
            Singleton<InputManager>.Instance.ActivateActionSet("InGame");
            Singleton<CoreGameManager>.Instance.disablePause = false;
            StartCoroutine(FadePad());
            Time.timeScale = 1;
            if (WrongTotal == 3)
            {
                ec.MakeNoise(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position, 79);
            }
        }
    }
}
