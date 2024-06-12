using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BBE.Helpers;
using System.IO;
using System;
using HarmonyLib;

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
        private List<string> HiddenHUD = new List<string> { };
        public List<SpriteRenderer> Indicators = new List<SpriteRenderer>() { };
        private Operator operatorProblem;
        public int WrongTotal = 0;
        private EnvironmentController ec;
        public List<GameObject> CheckMarks = new List<GameObject> { };
        public List<GameObject> CrossMarks = new List<GameObject> { };

        void Start()
        {
            Setup();
            OpenPad();
            Time.timeScale = 0;
        }
        void Update () 
        {
            try
            {
                PlayerAnswer.ChangeizeTextContainerState(true);
                ProblemText.ChangeizeTextContainerState(true);
            }
            catch (NullReferenceException)
            {
                
            }
            if (Input.anyKeyDown)
            {
                if (Input.inputString.Length > 0 && char.IsNumber(Input.inputString[0])) 
                {
                    if (PlayerAnswer.text.Length < 5)
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
            if (hide)
            {
                for (int i = 0; i < Singleton<CoreGameManager>.Instance.GetHud(0).transform.childCount; i++)
                {
                    Transform child = Singleton<CoreGameManager>.Instance.GetHud(0).transform.GetChild(i);
                    if (child.name != "PlayerAnswer_Text" && child.name != "ProblemText_Text" && child.gameObject.activeSelf) 
                    {
                        child.gameObject.SetActive(false);
                        HiddenHUD.Add(child.name);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Singleton<CoreGameManager>.Instance.GetHud(0).transform.childCount; i++)
                {
                    Transform child = Singleton<CoreGameManager>.Instance.GetHud(0).transform.GetChild(i);
                    if (HiddenHUD.Contains(child.name) && !child.gameObject.activeSelf)
                    {
                        child.gameObject.SetActive(true);
                        HiddenHUD.Remove(child.name);
                    } 
                }
            }
        }
        public void Setup()
        {
            HideHUD(true);
            currentProblem = 1;
            WrongTotal++;
            ec = Singleton<BaseGameManager>.Instance.Ec;
            canvas = UnityEngine.Object.Instantiate(Prefabs.Canvas);
            canvas.name = "YCTP_Canvas_ExtraMod";
            UnityEngine.UI.Image canvasImage = canvas.GetComponentInChildren<UnityEngine.UI.Image>();
            canvasImage.sprite = BasePlugin.Instance.asset.Get<Sprite>("YCTPCanvas");
            Canvas yctpCanvas = canvas.GetComponent<Canvas>();
            canvas.SetActive(true);
            yctpCanvas.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
            PlayerAnswer = Instantiate(PrivateDataHelper.GetVariable<TMP_Text[]>(Singleton<CoreGameManager>.Instance.GetHud(0), "textBox")[0]);
            PlayerAnswer.gameObject.transform.position = new Vector3(-1.5f, -1.7f, 1);
            PlayerAnswer.gameObject.transform.localScale = new Vector3(0.0277f, 0.0277f, 0.0277f);
            PlayerAnswer.isOrthographic = false;
            PlayerAnswer.gameObject.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).transform);
            PlayerAnswer.gameObject.name = "PlayerAnswer_Text";
            PlayerAnswer.fontSize = 48f;
            PlayerAnswer.text = "";
            ProblemText = Instantiate(PrivateDataHelper.GetVariable<TMP_Text[]>(Singleton<CoreGameManager>.Instance.GetHud(0), "textBox")[0]);
            ProblemText.gameObject.transform.position = new Vector3(-2.5f, 0, 1);
            ProblemText.gameObject.transform.localScale = new Vector3(0.0277f, 0.0277f, 0.0277f);
            ProblemText.isOrthographic = false;
            ProblemText.text = "";
            ProblemText.gameObject.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).transform);
            ProblemText.gameObject.name = "ProblemText_Text";
            ProblemText.gameObject.SetActive(true);
            PlayerAnswer.gameObject.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                GameObject game = new GameObject("Checkmark");
                SpriteRenderer res = game.AddComponent<SpriteRenderer>();
                res.sprite = BasePlugin.Instance.asset.Get<Sprite>("CheckMark");
                res.sortingOrder = 1;
                game.transform.SetParent(canvas.transform);
                CheckMarks.Add(game);
            }
            for (int i = 0; i < 3; i++)
            {
                GameObject game = new GameObject("CrossMark");
                SpriteRenderer res = game.AddComponent<SpriteRenderer>();
                res.sprite = BasePlugin.Instance.asset.Get<Sprite>("CrossMark");
                res.sortingOrder = 1;
                game.transform.SetParent(canvas.transform);
                CheckMarks.Add(game);
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
            currentProblem++;
        }
        public void GenerateDivision()
        {
            List<List<int>> possible = new List<List<int>>() { };
            for (int x = -9; x <= 9 ; x++)
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
            if (PlayerAnswer.text == "" || int.Parse(PlayerAnswer.text) != answer)
            {
                WrongTotal++;
            }
        }
        public void DestroyAll()
        {
            foreach (GameObject data in CheckMarks)
            {
                Destroy(data);
            }
            foreach (GameObject data in CrossMarks)
            {
                Destroy(data);
            }
            Destroy(canvas);
            Destroy(PlayerAnswer);
            Destroy(PlayerAnswer.gameObject);
            Destroy(ProblemText);
            Destroy(ProblemText.gameObject);
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
                ec.GetBaldi().GetAngry((WrongTotal - 1) * 0.333f);
            }
            else if (!ec.GetBaldi().IsNull())
            {
                ec.GetBaldi().GetExtraAnger((WrongTotal-1) * 0.333f);
            }
            HideHUD(false);
            if (WrongTotal == 4)
            {
                ec.MakeNoise(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position, 79);
            }
            Singleton<InputManager>.Instance.ActivateActionSet("InGame");
            Singleton<CoreGameManager>.Instance.disablePause = false;
            Time.timeScale = 1;
            DestroyAll();
        }
    }
}
