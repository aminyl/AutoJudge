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
using System.Xml.Serialization;

namespace AutoJudge
{
    public partial class Form1 : Form
    {
        // 問題のファイル名はA～J (A.rb, B.rb...)
        string answerFilePath = @"C:\Users\blackhat\GoogleDrive\ProgrammingContest\ruby\vimcodes\";
        string fileType = ".rb";

        string[] problemStrs = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
        Hashtable ht = new Hashtable();
        Dictionary<string, int> problemTable = new Dictionary<string, int>(){
            {"A", 0},{"B", 1},{"C", 2},{"D", 3},{"E", 4},{"F", 5},{"G", 6},{"H", 7},{"I", 8},{"J", 9}};

        int problemNum = 8; // 問題がABCDだけならproblemNum = 4, 11以上はKLM...と追加が必要
        int testCaseNum = 5; // テストケースの数，変更可能
        string[,] inputs, groundTruth, samples, checks; // テキストを保持, [問題数, テストケース数]

        int style = 1; // 0:タイトルバーあり 1:タイトルバーなし 2:タイトルバーなし，サイズ変更可能

        int problemNowN = 0; // 現在解いている問題
        string problemNowS = "A"; // 現在解いている問題
        bool tabChanged = false; // タブが変わった時にジャッジされないように

        bool updateOnActivated = true; // フォームをウィンドウの最前面に持ってくるたびにジャッジするかどうか、チェックボックスで変更

        System.Diagnostics.Process p; // コマンドラインで実行
        string result, command; // 実行結果, 入力

        public Form1()
        {
            InitializeComponent();

            if (style == 1) // タイトルバーをなくす
            {
                this.FormBorderStyle = FormBorderStyle.None;
            }
            else if (style == 2)
            {
                this.ControlBox = false;
                this.Text = "";
            }
            // BackColor = Color.Green;


            init();
        }

        // マウスのボタンが押されたとき : Form1のタイトルバーをなくした時に，フォームを移動できるように
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (style == 0) return;
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                //位置を記憶する
                mousePoint = new Point(e.X, e.Y);
            }
        }

        // マウスが動いたとき : Form1のタイトルバーをなくした時に，フォームを移動できるように
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (style == 0) return;
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this.Left += e.X - mousePoint.X;
                this.Top += e.Y - mousePoint.Y;
            }
        }

        private void init()
        {

            inputs = new string[10, testCaseNum];
            groundTruth = new string[problemNum, testCaseNum];
            samples = new string[problemNum, testCaseNum];
            checks = new string[problemNum, testCaseNum];

            makeControlls();
        }

        private void btnTabs_Click(object sender, EventArgs e)
        {
            string btnName = ((Button)sender).Name;
            string btnID = btnName.Substring(btnName.Length - 1);
            problemNowN = problemTable[btnID];
            problemNowS = btnID;

            for (int i = 0; i < testCaseNum; i++)
            {
                // タブが変わった時にジャッジされないように
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
            ((Button)sender).BackColor = Color.White;
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
            }
        }

        //private void chBoxUpdateOnActivated_Click(object sender, EventArgs e)
        //{
        //    updateOnActivated = ((CheckBox)sender).Checked;
        //}

        // プログラム終了
        private void btnClose_Click(object sender, EventArgs e)
        {
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
                int txtboxIDn = problemTable[txtboxIDs];
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
                int txtboxIDn = problemTable[txtboxIDs];
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
            // cmd.exe を起動します。
            p.Start();
            // cmd.exe で実行するコマンドを取得します。
            command = input;
            // 標準入出力からリードライトするためのスレッドを起動します。
            StartThread();
            // cmd.exe が終了するのを待ちます。
            p.WaitForExit();
            // 結果を　textBox2 へ書き出します。
            return result;
        }

        public void StartThread()
        {
            System.Threading.ThreadStart readThread = new System.Threading.ThreadStart(ReadThread);
            System.Threading.ThreadStart writeThread = new System.Threading.ThreadStart(WriteThread);
            System.Threading.Thread rThread = new System.Threading.Thread(readThread);
            System.Threading.Thread wThread = new System.Threading.Thread(writeThread);
            rThread.Name = "ReadThread";
            wThread.Name = "WriteThread";
            rThread.Start();       //starting the read thread
            wThread.Start();       //starting the write thread
            wThread.Join();
            rThread.Join();
        }

        private void ReadThread()
        {
            result = p.StandardOutput.ReadToEnd();
        }

        private void WriteThread()
        {
            p.StandardInput.WriteLine(command);
            p.StandardInput.WriteLine("exit");
        }


        private string startPSI(ProcessStartInfo psi)
        {
            Process p = Process.Start(psi); // アプリの実行開始
            string output = p.StandardOutput.ReadToEnd(); // 標準出力の読み取り
            return output.Replace("\r\r\n", "\n"); // 改行コードの修正
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
