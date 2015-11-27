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

        int style = 1; // 0:タイトルバーあり 1:タイトルバーなし 2:タイトルバーなし + サイズ変更可能

        int problemNowN = 0; // 現在開いているタブ
        string problemNowS; // 現在開いているタブ

        bool tabChanged = false; // タブが変わった時にジャッジされないように
        bool updateOnActivated = true; // フォームをウィンドウの最前面に持ってくるたびにジャッジするかどうか、チェックボックスで変更

        public Form1()
        {
            InitializeComponent();

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
        }

        private void init()
        {
            System.Console.WriteLine(problemStrs.Length);
            for (int i = 0; i < problemStrs.Length; i++)
                problemNums[problemStrs[i]] = i;
            problemNowS = problemStrs[problemNowN];

            inputs = new string[problemNum, testCaseNum];
            groundTruth = new string[problemNum, testCaseNum];
            samples = new string[problemNum, testCaseNum];
            checks = new string[problemNum, testCaseNum];

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
            string btnName = ((Button)sender).Name;
            string btnID = btnName.Substring(btnName.Length - 1);
            problemNowN = problemNums[btnID];
            problemNowS = btnID;

            loadDataToTab();
        }

        private void loadDataToTab()
        {
            for (int i = 0; i < testCaseNum; i++)
            {
                // タブが変わった時にジャッジされないように TODO:他の方法で
                tabChanged = true;
                txtbxInputs[i].Text = inputs[problemNowN, i];
                tabChanged = true;
                txtbxAnswers[i].Text = groundTruth[problemNowN, i];
                tabChanged = true;
                labelSample[i].Text = samples[problemNowN, i];
                tabChanged = true;
                labelCheck[i].Text = checks[problemNowN, i];
            }
            checkAll(); // 色情報は保持していないので再チェック

            // 選択されているタブの色を白色に
            for (int i = 0; i < problemNum; i++)
                btnTabs[i].BackColor = Color.Transparent;
            btnTabs[problemNowN].BackColor = Color.White;
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
            if (tabChanged)
                tabChanged = false;
            else
            {
                string txtboxText = ((TextBox)sender).Text;
                string txtboxName = ((TextBox)sender).Name;
                string txtboxIDs = txtboxName.Substring(txtboxName.Length - 1);
                int txtboxIDn = problemNums[txtboxIDs];
                inputs[problemNowN, txtboxIDn] = txtboxText;

                // 入力された文字列を渡して実行
                string tmpres = excute_byIDn(txtboxIDn);
                // 表示と保存
                samples[problemNowN, txtboxIDn] = (labelSample[txtboxIDn].Text = tmpres);

                // 答え合わせ
                checkAns(txtboxIDn);
            }
        }

        private void txtbxAnswers_Changed(object sender, EventArgs e)
        {
            if (tabChanged)
                tabChanged = false;
            else
            {
                string txtboxText = ((TextBox)sender).Text;
                string txtboxName = ((TextBox)sender).Name;
                string txtboxIDs = txtboxName.Substring(txtboxName.Length - 1);
                int txtboxIDn = problemNums[txtboxIDs];
                groundTruth[problemNowN, txtboxIDn] = txtboxText;

                // 答え合わせ
                checkAns(txtboxIDn);
            }
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
            else if (gt == smp)
            {
                res = "AC";
                labelCheck[id].BackColor = Color.Green;
            }
            else
            {
                res = "WA";
                labelCheck[id].BackColor = Color.Orange;
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
            return excute(ID, input);
        }

        // filename:プログラムファイル名, input:入力
        private string excute(string filename, string input)
        {
            p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "ruby.exe";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.Arguments = answerFilePath + filename + fileType;
            // cmd.exe を起動
            p.Start();
            // cmd.exe で実行するコマンドを取得
            command = input;
            // 標準入出力からリードライトするためのスレッドを起動
            StartThread();
            // cmd.exe が終了するのを待つ
            p.WaitForExit();
            return result;
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
                exeAllcheckAll();
        }
    }
}
