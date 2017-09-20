using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Permissions;

namespace AutoJudge
{
    public partial class Form1 : Form
    {
        // 1つのディレクトリにすべての実行ファイルを置いておく
        string fileType = ".rb";

        string[] problemStrs = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" }; // 実行ファイル名 e.g. A.rb, A.exe
        Dictionary<string, int> problemNums = new Dictionary<string, int>();

        int problemNum = 8; // 表示するタブ数, 11以上は problemStrs に追加が必要
        int testCaseNum = 5; // テストケースの数，変更可能
        string[,] inputs, groundTruth, samples, checks; // テキストを保持, [問題数, テストケース数]
        string[,] errors; // エラーを保持, 今のところTLEのみ
        string errorFlag; // エラーを保持, 今のところTLEのみ
        string NONE = "", AC = "AC", WA = "WA", TLE = "TLE";

        int style = 1; // 0:タイトルバーあり 1:タイトルバーなし 2:タイトルバーなし + サイズ変更可能

        int problemNowN = 0; // 現在開いているタブ
        string problemNowS; // 現在開いているタブ

        bool tabChanged = false; // タブが変わった時にジャッジされないように
        bool updateOnActivated = true; // フォームをウィンドウの最前面に持ってくるたびにジャッジするかどうか、チェックボックスで変更
        bool refreshing = false; // // テストケースをクリアしているときにジャッジされないように
        bool onStart; // 起動したときにジャッジされないように

        // 設定
        string settingFilePath = "settings.txt";
        string codeFilePath = string.Empty;
        string testCasePath = "TestCases.txt";
        string backgroundImagePath = string.Empty;
        float[] backgroundMatrixV = new float[5];

        public Form1()
        {
            InitializeComponent();
            onStart = true;

            if (style == 1) // タイトルバーをなくす
            {
                FormBorderStyle = FormBorderStyle.None;
            }
            else if (style == 2) // サイズ変更できるように
            {
                ControlBox = false;
                Text = "";
            }
            else if (style == 3) // style 1 で最小化の挙動が気になるとき
            {
                FormBorderStyle = FormBorderStyle.FixedSingle;
                ControlBox = false;
                Text = "";
            }

            Init();
            LoadFile();

            // このForm1()が終わった後に1度Form1_Activated()が呼ばれる
            if (!updateOnActivated) onStart = false;
        }

        private void Init()
        {
            if (System.IO.File.Exists(settingFilePath))
                ReadSettings();

            for (int i = 0; i < problemStrs.Length; i++)
                problemNums[problemStrs[i]] = i;
            problemNowS = problemStrs[problemNowN];

            inputs = new string[problemNum, testCaseNum];
            groundTruth = new string[problemNum, testCaseNum];
            samples = new string[problemNum, testCaseNum];
            checks = new string[problemNum, testCaseNum];
            errors = new string[problemNum, testCaseNum];

            MakeControlls();
        }

        private void ReadSettings()
        {
            string tag_;
            System.IO.StreamReader cReader = (
                new System.IO.StreamReader(settingFilePath, System.Text.Encoding.Default)
            );

            tag_ = cReader.ReadLine();
            codeFilePath = cReader.ReadLine();
            tag_ = cReader.ReadLine();
            testCasePath = cReader.ReadLine();
            tag_ = cReader.ReadLine();
            backgroundImagePath = cReader.ReadLine();
            for (int i = 0; i < 5; i++)
                backgroundMatrixV[i] = float.Parse(cReader.ReadLine());
            cReader.Close();
        }

        // 背景をドラッグしてウィンドウを移動できるように
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (style == 0) return;
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                mousePoint = new Point(e.X, e.Y);
            }
        }

        // 背景をドラッグしてウィンドウを移動できるように
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (style == 0) return;
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // 最大化されたときにドラッグで元に戻す
                if (WindowState == FormWindowState.Maximized && mousePoint.Y < 20)
                    WindowState = FormWindowState.Normal;
                Left += e.X - mousePoint.X;
                Top += e.Y - mousePoint.Y;
            }
        }

        private void BtnTabs_Click(object sender, EventArgs e)
        {
            Console.WriteLine("tab clicked");

            string btnName = ((Button)sender).Name;
            string btnIDS = btnName.Substring(btnName.Length - 1);
            int btnIDN = problemNums[btnIDS];

            if (problemNowN == btnIDN)
                return;

            Console.WriteLine("tab changed");

            ChangeProblemNowTo(btnIDN);
            // タブが変わった時にジャッジされないように
            tabChanged = true;
            LoadDataToTab();
            tabChanged = false;
        }

        private void LoadDataToTab()
        {
            for (int i = 0; i < testCaseNum; i++)
            {
                txtbxInputs[i].Text = inputs[problemNowN, i];
                txtbxAnswers[i].Text = groundTruth[problemNowN, i];
                labelSample[i].Text = samples[problemNowN, i];
                labelCheck[i].Text = checks[problemNowN, i];
            }
            CheckAll(); // 色情報は保持していないので再チェック

            // 選択されているタブの色を白色に
            for (int i = 0; i < problemNum; i++)
                btnTabs[i].BackColor = Color.Transparent;
            btnTabs[problemNowN].BackColor = Color.White;
        }

        private void ChangeProblemNowTo(int i)
        {
            problemNowN = i;
            problemNowS = problemStrs[i];
        }

        private void ChangeProblemNowTo(string s)
        {
            problemNowN = problemNums[s];
            problemNowS = s;
        }

        private void BtnExeAll_Click(object sender, EventArgs e)
        {
            ExeAllcheckAll();
        }

        private void BtnUpdate_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!updateOnActivated)
                {
                    ExeAllcheckAll();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                updateOnActivated = !updateOnActivated;
                SetUpdateBtn(updateOnActivated ? 1 : 0);
                //loadFile();
            }
        }

        // 開いている問題のテストケースをクリア
        private void BtnRefresh_DoubleClick(object sender, EventArgs e)
        {
            RefreshTestCase();
        }

        // プログラム終了
        private void BtnClose_Click(object sender, EventArgs e)
        {
            SaveFile();
            Close();
        }

        // フォームを最小化
        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Maximized:
                    WindowState = FormWindowState.Normal;
                    break;
                default:
                    WindowState = FormWindowState.Minimized;
                    break;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (!onStart)
            {
                switch (WindowState)
                {
                    case FormWindowState.Maximized:
                        // フォームが最小化、最大化された時は、元の状態に戻す
                        WindowState = FormWindowState.Normal;
                        // btnMinimize.Text = "□";
                        break;
                    default:
                        btnMinimize.Text = "-";
                        break;
                }
            }
        }

        private void ExeAll()
        {
            for (int i = 0; i < testCaseNum; i++)
                samples[problemNowN, i] = (labelSample[i].Text = Excute_byIDn(i));
        }

        private void CheckAll()
        {
            for (int i = 0; i < testCaseNum; i++)
                CheckAns(i);
        }

        private void ExeAllcheckAll()
        {
            ExeAll();
            CheckAll();
        }

        private void RefreshTestCase()
        {
            refreshing = true;
            for (int i = 0; i < testCaseNum; i++)
            {
                txtbxInputs[i].Text = "";
                txtbxAnswers[i].Text = "";
                inputs[problemNowN, i] = "";
                groundTruth[problemNowN, i] = "";
            }
            refreshing = true;
            ExeAllcheckAll();
        }

        private void TxtbxInputs_Changed(object sender, EventArgs e)
        {
            if (tabChanged || onStart || refreshing)
                return;

            Console.WriteLine("inputs changed");
            string txtboxText = ((TextBox)sender).Text;
            string txtboxName = ((TextBox)sender).Name;
            string txtboxIDs = txtboxName.Substring(txtboxName.Length - 1);
            int txtboxIDn = problemNums[txtboxIDs];
            SaveInputToArray(txtboxIDn);

            // 入力された文字列を渡して実行
            Console.WriteLine("excute txtboxId : " + txtboxIDn);
            string tmpres = Excute_byIDn(txtboxIDn);
            // 表示と保存
            samples[problemNowN, txtboxIDn] = (labelSample[txtboxIDn].Text = tmpres);

            // 答え合わせ
            CheckAns(txtboxIDn);
        }

        private void TxtbxAnswers_Changed(object sender, EventArgs e)
        {
            if (tabChanged || onStart || refreshing)
                return;

            Console.WriteLine("answers changed");
            string txtboxText = ((TextBox)sender).Text;
            string txtboxName = ((TextBox)sender).Name;
            string txtboxIDs = txtboxName.Substring(txtboxName.Length - 1);
            int txtboxIDn = problemNums[txtboxIDs];
            SaveGTToArray(txtboxIDn);

            // 答え合わせ
            CheckAns(txtboxIDn);
        }

        private void SaveInputToArray(int id)
        {
            inputs[problemNowN, id] = txtbxInputs[id].Text;
        }

        private void SaveGTToArray(int id)
        {
            groundTruth[problemNowN, id] = txtbxAnswers[id].Text;
        }

        // Ctr + A で全選択
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                ((TextBox)sender).SelectAll();
        }

        // 出力が正しいかを判定して表示
        private void CheckAns(int id)
        {
            string gt = txtbxAnswers[id].Text;
            string smp = labelSample[id].Text;
            string res;
            if (gt == "")
            {
                res = "";
                labelCheck[id].BackColor = Color.Transparent; // 無くてもいい
            }
            else if (!(errors[problemNowN, id] == NONE || errors[problemNowN, id] == null))
            {
                Console.WriteLine("error name : " + errors[problemNowN, id]);
                res = errors[problemNowN, id];
                labelCheck[id].BackColor = Color.Orange;
            }
            else
            {
                if (gt == smp)
                {
                    res = AC;
                    labelCheck[id].BackColor = Color.Green;
                }
                else
                {
                    res = WA;
                    labelCheck[id].BackColor = Color.Orange;
                }
            }
            checks[problemNowN, id] = (labelCheck[id].Text = res);
        }

        // テストケースをIDで指定して実行
        private string Excute_byIDn(int idn)
        {
            string ID = problemNowS;
            string input = txtbxInputs[idn].Text;
            string gt = txtbxAnswers[idn].Text;
            if (input == "" && gt == "") return ""; // 何も入力されていない場合は実行しない
            string res = Excute(ID, input);
            SetError(idn);
            return res;
        }

        private void SetError(int id)
        {
            Console.WriteLine("setting error " + problemNowN + " " + id + " " + id + " ");
            errors[problemNowN, id] = errorFlag;
            errorFlag = "";
        }

        // ダブルクリックした時にジャッジ
        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ExeAllcheckAll();
        }

        // 何かキーを押した時にジャッジ, 動かない
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            ExeAllcheckAll();
        }

        // フォームをウィンドウの最前面に持ってくるたびにジャッジ
        private void Form1_Activated(object sender, EventArgs e)
        {
            if (updateOnActivated)
            {
                if (onStart)
                    onStart = false;
                else
                    ExeAllcheckAll();
            }
        }
    }
}
