﻿using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MTM101BaldAPI.Registers;
using BBE.Helpers;
using System.Collections;
using TMPro;
using MTM101BaldAPI.Reflection;
using UnityEngine.SceneManagement;
using System.Linq;
using NPOI.SS.Formula.Functions;

namespace BBE.CustomClasses
{
    public enum Operator
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }
    public class TimeAttack : MonoBehaviour
    {
        private float time;
        private EnvironmentController ec;
        private TMP_Text canvasText;
        private GameObject canvas;
        void Start () 
        {
            time = 90f;
            ec = Singleton<BaseGameManager>.Instance.ec;
            canvas = CreateObjects.CreateCanvas("TimeAttackCanvas", color: Color.clear);
            canvasText = CreateObjects.CreateText("TimeLeftText", "", true, new Vector3(-310, 130, 0), Vector3.one, canvas.transform, 24);
        }
        void OnDestooy()
        {
            canvas.DeleteComponent<TMP_Text>();
            Destroy(canvas);
        }
        void Update ()
        {
            if (time <= 0)
            {
                EndGame();
                return;
            }
            if (!canvasText.IsNull()) canvasText.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("TimeLeftText") + ConvertTime(time);
            if (ec.Active && !ec.IsNull()) time -= Time.deltaTime;
        }
        private string ConvertTime(float totalSeconds)
        {
            int roundedSeconds = Mathf.RoundToInt(totalSeconds);
            int minutes = roundedSeconds / 60;
            int seconds = roundedSeconds % 60;
            return $"{minutes}:{seconds:D2}";
        }
        private void EndGame()
        {
            try
            {
                ec.GetBaldi().Teleport(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position);
            }
            catch { }
        }
        public void AddTime(float time)
        {
            this.time += time;
        }
    }
    public class ExtendedYCTP : YCTP
    {
        private int FirstAnswer;
        private int SecondAnswer;
        private List<int> answers = new List<int>();
        public override void Setup()
        {
            base.Setup();
            GenerateProblem();
        }
        public override void GenerateProblem()
        {
            SolveText.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("SolveEquation") + currentProblem + "\n" + Singleton<LocalizationManager>.Instance.GetLocalizedText("OneOfTwoSolutions");
            FirstAnswer = UnityEngine.Random.Range(1, 10);
            SecondAnswer = UnityEngine.Random.Range(1, 10);
            answers.Clear();
            answers.Add(FirstAnswer);
            answers.Add(SecondAnswer);
            int a = UnityEngine.Random.Range(1, 3);
            string res = "";
            if (a != 1)
            {
                res += a;
            }
            res += "X²-";
            res += ((FirstAnswer + SecondAnswer) * a);
            res += "X+";
            res += a * SecondAnswer * FirstAnswer;
            res += "=0";
            ProblemText.text = res;
        }
        public override void CheckAnswer()
        {
            GameObject game = Marks[currentProblem - 1];
            if (WrongAnswers.Contains(PlayerAnswer.text) || !answers.Contains(int.Parse(PlayerAnswer.text)))
            {
                game.GetComponent<Image>().sprite = BasePlugin.Instance.asset.Get<Sprite>("CrossMark");
                WrongTotal++;
            }
            game.SetActive(true);
            currentProblem++;
            PlayerAnswer.text = "";
        }
    }
    public class YCTP : MonoBehaviour
    {
        private GameObject canvas;
        private int FirstNum = 0;
        private int SecondNum = 0;
        private int answer = 0;
        protected int currentProblem = 1;
        public GameObject ObjectToDestroy;
        protected TMP_Text PlayerAnswer;
        protected TMP_Text ProblemText;
        protected TMP_Text SolveText;
        private bool IsActive = true;
        private Operator operatorProblem;
        protected int WrongTotal = 0;
        private EnvironmentController ec;
        protected List<string> WrongAnswers = new List<string>() { "", "-", " " };
        protected List<GameObject> Marks = new List<GameObject> { };

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
                SolveText.ChangeizeTextContainerState(true);
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
                    if (tmp.Length < 9)
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
                    CheckCheatCode();
                    CheckAnswer();
                    if (currentProblem > 3)
                    {
                        DestroyText();
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
        public void CheckCheatCode()
        {
            if (WrongAnswers.Contains(PlayerAnswer.text))
            {
                return;
            }
            if (int.Parse(PlayerAnswer.text) == 31718)
            {
                SolveText.text = "";
                ProblemText.text = "No. This is not classic version";
                return;
            }
            if (int.Parse(PlayerAnswer.text) == 777)
            {
                Singleton<BaseGameManager>.Instance.StopAllCoroutines();
                Singleton<BaseGameManager>.Instance.Ec.ResetEvents();
                Time.timeScale = 0f;
                Singleton<CoreGameManager>.Instance.readyToStart = false;
                Singleton<CoreGameManager>.Instance.disablePause = true;
                PropagatedAudioManager.paused = true;
                Singleton<BaseGameManager>.Instance.elevatorScreen = Instantiate(Singleton<BaseGameManager>.Instance.elevatorScreenPre);
                ElevatorScreen elevatorScreen = Singleton<BaseGameManager>.Instance.elevatorScreen;
                ElevatorScreen.OnLoadReadyHandler value;
                Singleton<BaseGameManager>.Instance.StopAllCoroutines();
                Singleton<BaseGameManager>.Instance.Ec.ResetEvents();
                Time.timeScale = 0f;
                value = () =>
                {
                    Singleton<CoreGameManager>.Instance.readyToStart = false;
                    Singleton<CoreGameManager>.Instance.disablePause = true;
                    PropagatedAudioManager.paused = true;
                    Singleton<CoreGameManager>.Instance.PrepareForReload();
                    Singleton<CoreGameManager>.Instance.SetLives(3);
                    Singleton<CoreGameManager>.Instance.tripPlayed = false;
                    Singleton<SubtitleManager>.Instance.DestroyAll();
                    Singleton<CoreGameManager>.Instance.sceneObject = AssetsHelper.FindAllOfType<SceneObject>().Where(x => x.name == "MainLevel_3").First();
                    SceneManager.LoadScene("Game");
                    HideHUD(false);
                };
                elevatorScreen.OnLoadReady += value;
                elevatorScreen.Initialize();
                return;
            }
        }
        public void HideHUD(bool hide)
        {
            if (hide)
            {
                Singleton<SubtitleManager>.Instance.DestroyAll();
            }
            Singleton<CoreGameManager>.Instance.GetHud(0).Hide(hide);
        }
        public virtual void Setup()
        {
            HideHUD(true);
            currentProblem = 1;
            WrongTotal = 0;
            ec = Singleton<BaseGameManager>.Instance.Ec;
            canvas = CreateObjects.CreateCanvas("YCTP_Canvas_ExtraMod", sprite: BasePlugin.Instance.asset.Get<Sprite>("YCTPCanvas"));
            PlayerAnswer = CreateObjects.CreateText("PlayerAnswer_Text", "", true, new Vector3(-54, -77.4f, -36), new Vector3(10, 10, 10), canvas.transform, 24f);
            PlayerAnswer.isOrthographic = false;
            ProblemText = CreateObjects.CreateText("ProblemText_Text", "", true, new Vector3(-90, 0, -36), new Vector3(10, 10, 10), canvas.transform, 24f);
            ProblemText.isOrthographic = false;
            SolveText = CreateObjects.CreateText("SolveText", "Test", true, new Vector3(-90, 50.4f, -36), new Vector3(10, 10, 10), canvas.transform, 20f);
            SolveText.isOrthographic = false;
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
        public virtual void GenerateProblem()
        {
            SolveText.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("SolveMath") + currentProblem;
            operatorProblem = new Operator[] { Operator.Addition, Operator.Subtraction }.ChooseRandom();

            if (FunSettingsType.HardMode.IsActive())
            {
                operatorProblem = new Operator[] { Operator.Multiplication, Operator.Division }.ChooseRandom();
            }
            PlayerAnswer.text = "";
            FirstNum = UnityEngine.Random.Range(-9, 10);
            SecondNum = UnityEngine.Random.Range(-9, 10);
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
        public IEnumerator FadeOff()
        {
            float x = 10;
            while (x > 0)
            {
                x -= 0.1f;
                yield return null;
            }
            foreach (GameObject data in Marks)
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
                canvasImage.color = new Color(canvasImage.color.r, canvasImage.color.g, canvasImage.color.b, alpha);
                yield return null;
            }
            DestroyCanvas();
            Destroy(SolveText);
            Destroy(SolveText.gameObject);
            HideHUD(false);
            Time.timeScale = 1;
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
        public virtual void CheckAnswer()
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
            if (FunSettingsType.HardMode.IsActive() && !ec.GetBaldi().IsNull())
            {
                ec.GetBaldi().GetAngry(WrongTotal * 0.333f);
            }
            else if (!ec.GetBaldi().IsNull())
            {
                ec.GetBaldi().GetExtraAnger(WrongTotal * 0.333f);
            }
            string textToSet = "";
            if (WrongTotal == 0)
            {
                if (Singleton<BaseGameManager>.Instance is EndlessGameManager)
                {
                    textToSet = Singleton<LocalizationManager>.Instance.GetLocalizedText("YCTP_Recover" + UnityEngine.Random.Range(1, 3));
                }
                else
                {
                    textToSet = Singleton<LocalizationManager>.Instance.GetLocalizedText("YCTP_Victory1");
                }
            }
            else
            {
                textToSet = Singleton<LocalizationManager>.Instance.GetLocalizedText("YCTP_Fail" + UnityEngine.Random.Range(1, 3));
            }
            SolveText.text = textToSet;
            Singleton<InputManager>.Instance.ActivateActionSet("InGame");
            Singleton<CoreGameManager>.Instance.disablePause = false;
            StartCoroutine(FadeOff());
            if (WrongTotal == 3)
            {
                ec.MakeNoise(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position, 79);
            }
        }
    }
    class HookFunSetting : MonoBehaviour
    {
        private static MovementModifier moveModifer = new MovementModifier(default, 0f);

        void Start ()
        {
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.am.moveMods.Add(moveModifer);
        }
        void Update ()
        {
            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(0, true);
            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.SetItem(ItemMetaStorage.Instance.FindByEnum(Items.GrapplingHook).value, 0);
        }
        void OnDestroy()
        {
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.am.moveMods.Remove(moveModifer);
            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(0, false);
            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.SetItem(ItemMetaStorage.Instance.FindByEnum(Items.None).value, 0);
        }
    }
    class DVDMode : MonoBehaviour
    {
        [DllImport("user32.dll")]
        static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [StructLayout(LayoutKind.Sequential)]
        struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
        }

        private int currentY = 1;
        private int currentX = 1;
        private int xModifer = 2;
        private int yModifer = 2;
        private List<int> xLimit = new List<int>() { };
        private List<int> yLimit = new List<int>() { };
        private bool fullScreen = false;
        private List<int> originalResolution = new List<int> { };
        private IntPtr gameWindow = IntPtr.Zero;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;

        void Start()
        {
            DEVMODE devMode = new DEVMODE();
            devMode.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            if (EnumDisplaySettings(null, -1, ref devMode))
            {
                int screenWidth = devMode.dmPelsWidth;
                int screenHeight = devMode.dmPelsHeight;
                xLimit.AddRange(new int[] { 0, screenWidth });
                yLimit.AddRange(new int[] { 0, screenHeight });
            }
            yModifer = new int[] { -2, 2 }.ChooseRandom();
            xModifer = new int[] { -2, 2 }.ChooseRandom();
            currentX = 0;
            currentY = 0;
            gameWindow = FindWindow(null, "Baldi's Basics Plus");
            originalResolution.Add(Singleton<PlayerFileManager>.Instance.resolutionX);
            originalResolution.Add(Singleton<PlayerFileManager>.Instance.resolutionY);
            fullScreen = Singleton<PlayerFileManager>.Instance.fullscreen;
            Singleton<PlayerFileManager>.Instance.resolutionX = 1024;
            Singleton<PlayerFileManager>.Instance.resolutionY = 768;
            Singleton<PlayerFileManager>.Instance.fullscreen = false;
            Singleton<PlayerFileManager>.Instance.UpdateResolution();
        }
        void Update()
        {
            if (gameWindow != IntPtr.Zero)
            {
                if (currentY < yLimit[0] || currentY+768 > yLimit[1])
                {
                    yModifer *= -1;
                }
                if (currentX < xLimit[0] || currentX+1024 > xLimit[1])
                {
                    xModifer *= -1;
                }
                currentY += yModifer * (int)Singleton<BaseGameManager>.Instance.Ec.NpcTimeScale * (int)Time.timeScale;
                currentX += xModifer * (int)Singleton<BaseGameManager>.Instance.Ec.NpcTimeScale * (int)Time.timeScale;
                SetWindowPos(gameWindow, IntPtr.Zero, currentX, currentY, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
            }
        }
        void OnDestroy()
        {
            if (gameWindow != IntPtr.Zero)
            {
                SetWindowPos(gameWindow, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                Singleton<PlayerFileManager>.Instance.resolutionX = originalResolution[0];
                Singleton<PlayerFileManager>.Instance.resolutionY = originalResolution[1];
                Singleton<PlayerFileManager>.Instance.fullscreen = fullScreen;
                Singleton<PlayerFileManager>.Instance.UpdateResolution();
            }
        }
    }
}
