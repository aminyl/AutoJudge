using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoJudge
{
    public partial class Form1 : Form
    {
        // 1つのディレクトリにすべての実行ファイルを置いておく
        string answerFilePath = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\GoogleDrive\ProgrammingContest\ruby\vimcodes\";
        string fileType = ".rb";

        string[] problemStrs = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" }; // 実行ファイル名
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

        bool onStart; // 起動したときにジャッジされないように

        public Form1()
        {
            InitializeComponent();
            onStart = true;

            if (style == 1) // タイトルバーをなくす
            {
                this.FormBorderStyle = FormBorderStyle.None;
            }
            else if (style == 2) // サイズ変更できるように
            {
                this.ControlBox = false;
                this.Text = "";
            }

            init();
            loadFile();

            // このForm1()が終わった後に1度Form1_Activated()が呼ばれる
            if (!updateOnActivated) onStart = false;
        }

        private void init()
        {
            for (int i = 0; i < problemStrs.Length; i++)
                problemNums[problemStrs[i]] = i;
            problemNowS = problemStrs[problemNowN];

            inputs = new string[problemNum, testCaseNum];
            groundTruth = new string[problemNum, testCaseNum];
            samples = new string[problemNum, testCaseNum];
            checks = new string[problemNum, testCaseNum];
            errors = new string[problemNum, testCaseNum];

            makeControlls();
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
                this.Left += e.X - mousePoint.X;
                this.Top += e.Y - mousePoint.Y;
            }
        }

        private void btnTabs_Click(object sender, EventArgs e)
        {
            Console.WriteLine("tab clicked");

            string btnName = ((Button)sender).Name;
            string btnIDS = btnName.Substring(btnName.Length - 1);
            int btnIDN = problemNums[btnIDS];

            if (problemNowN == btnIDN)
                return;

            Console.WriteLine("tab changed");

            changeProblemNowTo(btnIDN);
            // タブが変わった時にジャッジされないように
            tabChanged = true;
            loadDataToTab();
            tabChanged = false;
        }

        private void loadDataToTab()
        {
            for (int i = 0; i < testCaseNum; i++)
            {
                txtbxInputs[i].Text = inputs[problemNowN, i];
                txtbxAnswers[i].Text = groundTruth[problemNowN, i];
                labelSample[i].Text = samples[problemNowN, i];
                labelCheck[i].Text = checks[problemNowN, i];
            }
            checkAll(); // 色情報は保持していないので再チェック

            // 選択されているタブの色を白色に
            for (int i = 0; i < problemNum; i++)
                btnTabs[i].BackColor = Color.Transparent;
            btnTabs[problemNowN].BackColor = Color.White;
        }

        private void changeProblemNowTo(int i)
        {
            problemNowN = i;
            problemNowS = problemStrs[i];
        }

        private void changeProblemNowTo(string s)
        {
            problemNowN = problemNums[s];
            problemNowS = s;
        }

        private void btnExeAll_Click(object sender, EventArgs e)
        {
            exeAllcheckAll();
        }

        private void btnUpdate_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!updateOnActivated) {
                    exeAllcheckAll();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                updateOnActivated = !updateOnActivated;
                setUpdateBtn(updateOnActivated ? 1 : 0);
                //loadFile();
            }
        }

        // プログラム終了
        private void btnClose_Click(object sender, EventArgs e)
        {
            saveFile();
            Close();
        }

        // フォームを最小化
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void exeAll()
        {
            for (int i = 0; i < testCaseNum; i++)
                samples[problemNowN, i] = (labelSample[i].Text = excute_byIDn(i));
        }

        private void checkAll()
        {
            for (int i = 0; i < testCaseNum; i++)
                checkAns(i);
        }

        private void exeAllcheckAll()
        {
            exeAll();
            checkAll();
        }

        private void txtbxInputs_Changed(object sender, EventArgs e)
        {
            if (tabChanged || onStart)
                return;

            Console.WriteLine("inputs changed");
            string txtboxText = ((TextBox)sender).Text;
            string txtboxName = ((TextBox)sender).Name;
            string txtboxIDs = txtboxName.Substring(txtboxName.Length - 1);
            int txtboxIDn = problemNums[txtboxIDs];
            saveInputToArray(txtboxIDn);

            // 入力された文字列を渡して実行
            Console.WriteLine("excute txtboxId : " + txtboxIDn);
            string tmpres = excute_byIDn(txtboxIDn);
            // 表示と保存
            samples[problemNowN, txtboxIDn] = (labelSample[txtboxIDn].Text = tmpres);

            // 答え合わせ
            checkAns(txtboxIDn);
        }

        private void txtbxAnswers_Changed(object sender, EventArgs e)
        {
            if (tabChanged || onStart)
                return;

            Console.WriteLine("answers changed");
            string txtboxText = ((TextBox)sender).Text;
            string txtboxName = ((TextBox)sender).Name;
            string txtboxIDs = txtboxName.Substring(txtboxName.Length - 1);
            int txtboxIDn = problemNums[txtboxIDs];
            saveGTToArray(txtboxIDn);

            // 答え合わせ
            checkAns(txtboxIDn);
        }

        private void saveInputToArray(int id)
        {
            inputs[problemNowN, id] = txtbxInputs[id].Text;
        }

        private void saveGTToArray(int id)
        {
            groundTruth[problemNowN, id] = txtbxAnswers[id].Text;
        }

        // Ctr + A で全選択
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                ((TextBox)sender).SelectAll();
        }

        // 出力が正しいかを判定して表示
        private void checkAns(int id)
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
        private string excute_byIDn(int idn)
        {
            string ID = problemNowS;
            string input = txtbxInputs[idn].Text;
            string gt = txtbxAnswers[idn].Text;
            if (input == "" && gt == "") return ""; // 何も入力されていない場合は実行しない
            string res = excute(ID, input);
            if (errorFlag != "")
                setError(idn);
            return res;
        }

        private void setError(int id)
        {
            Console.WriteLine("setting error " + problemNowN + " " + id + " " + id + " ");
            errors[problemNowN, id] = errorFlag;
            errorFlag = "";
        }

        // ダブルクリックした時にジャッジ
        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            exeAllcheckAll();
        }

        // 何かキーを押した時にジャッジ, 動かない
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            exeAllcheckAll();
        }

        // フォームをウィンドウの最前面に持ってくるたびにジャッジ
        private void Form1_Activated(object sender, EventArgs e)
        {
            if (updateOnActivated)
            {
                if (onStart)
                    onStart = false;
                else
                    exeAllcheckAll();
            }
        }
    }
}
