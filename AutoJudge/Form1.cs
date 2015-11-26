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

        private System.Windows.Forms.Button[] btnTabs;
        private System.Windows.Forms.Button btnExeAll;
        private System.Windows.Forms.CheckBox chBoxUpdateOnActivated;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox[] txtbxInputs;
        private System.Windows.Forms.TextBox[] txtbxAnswers;
        private System.Windows.Forms.Label[] labelSample;
        private System.Windows.Forms.Label[] labelCheck;

        public Form1()
        {
            InitializeComponent();
            // フォームのサイズ変更
            SuspendLayout();
            ClientSize = new System.Drawing.Size(600, 500);

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

        // マウスのクリック位置を記憶
        private Point mousePoint;

        // マウスのボタンが押されたとき : Form1のタイトルバーをなくした時に，フォームを移動できるように
        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (style == 0) return;
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                //位置を記憶する
                mousePoint = new Point(e.X, e.Y);
            }
        }

        // マウスが動いたとき : Form1のタイトルバーをなくした時に，フォームを移動できるように
        private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
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

        private void makeControlls()
        {
            int tabHeight = 23, tabWidth = 48, tabOffset = 1, tabMergLeft = 0, tabMergUp = 0;
            int exeAllBtnHeight = tabHeight, exeAllBtnWidth = tabWidth;
            int exeAllBtnX = 500;
            int exeAllBtnY = tabMergUp;

            makeTabBtn(tabHeight, tabWidth, tabOffset, tabMergLeft, tabMergUp);
            makeExeAllBtn(exeAllBtnHeight, exeAllBtnWidth, exeAllBtnX, exeAllBtnY);
            if (style == 1 || style == 2)
            {
                makeCloseBtn(tabHeight, tabHeight, 600 - tabHeight, 0);
                makeMinimizeBtn(tabHeight, tabHeight, 600 - 2 * tabHeight, 0);
            }

            int updateOnActivatedCheckBoxX = 450, updateOnActivatedCheckBoxY = 0;
            int updateOnActivatedCheckBoxHeight = 20, updateOnActivatedCheckBoxWidth = 20;
            makeUpdateOnActivatedCheckBox(updateOnActivatedCheckBoxWidth, updateOnActivatedCheckBoxHeight, updateOnActivatedCheckBoxX, updateOnActivatedCheckBoxY);

            int txtbxHeight = 80, txtbxWidth = 100, txtbxOffset = 1, txtbxMergLeft = 5, txtbxMergUp = 50;
            int inputAnsMerge = 10;
            int sampleLblX = txtbxMergLeft + 2 * txtbxWidth + inputAnsMerge;
            int checkLblX = txtbxMergLeft + 3 * txtbxWidth + inputAnsMerge;
            makeInputTxtBox(txtbxHeight, txtbxWidth, txtbxOffset, txtbxMergLeft, txtbxMergUp);
            makeAnswerTxtBox(txtbxHeight, txtbxWidth, txtbxOffset, txtbxMergLeft, txtbxMergUp, inputAnsMerge);
            makeSampleLable(txtbxHeight, sampleLblX, txtbxOffset, txtbxMergUp);
            makeCheckLable(txtbxHeight, checkLblX, txtbxOffset, txtbxMergUp);
        }

        private void makeTabBtn(int height, int width, int offset, int mergLeft, int mergUp)
        {
            btnTabs = new System.Windows.Forms.Button[problemNum];
            for (int i = 0; i < btnTabs.Length; i++)
            {
                btnTabs[i] = new System.Windows.Forms.Button();
                int x = i * width - i * offset + mergLeft;
                int y = mergUp;
                makeBtn(btnTabs[i], "btnTab" + problemStrs[i], problemStrs[i], height, width, x, y, new EventHandler(btnTabs_Click));
            }
            btnTabs[0].BackColor = Color.White;
        }

        private void btnTabs_Click(object sender, EventArgs e)
        {
            string btnName = ((System.Windows.Forms.Button)sender).Name;
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
            ((System.Windows.Forms.Button)sender).BackColor = Color.White;
        }
        private void makeExeAllBtn(int height, int width, int x, int y)
        {
            btnExeAll = new System.Windows.Forms.Button();
            makeBtn(btnExeAll, "btnExeAll", "update", height, width, x, y, new EventHandler(btnExeAll_Click));
        }
        private void btnExeAll_Click(object sender, EventArgs e)
        {
            exeAllcheckAll();
        }
        private void makeCloseBtn(int height, int width, int x, int y)
        {
            btnClose = new System.Windows.Forms.Button();
            makeBtn(btnClose, "btnCloase", "X", height, width, x, y, new EventHandler(btnClose_Click));
        }
        private void makeMinimizeBtn(int height, int width, int x, int y)
        {
            btnClose = new System.Windows.Forms.Button();
            makeBtn(btnClose, "btnMinimize", "-", height, width, x, y, new EventHandler(btnClose_Minimized));
        }
        private void makeUpdateOnActivatedCheckBox(int height, int width, int x, int y)
        {
            chBoxUpdateOnActivated = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            //プロパティ設定
            chBoxUpdateOnActivated.Name = "chBoxUpdateOnActivated";
            chBoxUpdateOnActivated.AutoSize = false;
            chBoxUpdateOnActivated.Size = new Size(width, height);
            chBoxUpdateOnActivated.Location = new Point(x, y);
            chBoxUpdateOnActivated.FlatStyle = FlatStyle.Flat;
            chBoxUpdateOnActivated.BackColor = Color.Transparent;
            chBoxUpdateOnActivated.FlatAppearance.CheckedBackColor = Color.Gray;
            chBoxUpdateOnActivated.FlatAppearance.BorderSize = 0;
            chBoxUpdateOnActivated.FlatAppearance.BorderColor = Color.Gray;
            chBoxUpdateOnActivated.Appearance = Appearance.Button;
            chBoxUpdateOnActivated.CheckState = CheckState.Checked;
            //イベントハンドラに関連付け
            chBoxUpdateOnActivated.Click += new EventHandler(chBoxUpdateOnActivated_Click);
            //フォームにコントロールを追加
            Controls.Add(chBoxUpdateOnActivated);
            ResumeLayout(false);

        }
        private void chBoxUpdateOnActivated_Click(object sender, EventArgs e)
        {
            updateOnActivated = ((System.Windows.Forms.CheckBox)sender).Checked;
        }

        // 色指定必要
        private void makeBtn(Button btn, string name, string text, int height, int width, int x, int y, EventHandler e)
        {
            SuspendLayout();
            //プロパティ設定
            btn.Name = name;
            btn.Text = text;
            btn.Size = new Size(width, height);
            btn.Location = new Point(x, y);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.MouseOverBackColor = Color.White;
            btn.FlatAppearance.BorderSize = 0;
            //イベントハンドラに関連付け
            btn.Click += e;
            //フォームにコントロールを追加
            Controls.Add(btn);
            ResumeLayout(false);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnClose_Minimized(object sender, EventArgs e)
        {
            // フォームを最小化
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

        private void makeInputTxtBox(int height, int width, int offset, int mergLeft, int mergUp)
        {
            txtbxInputs = new System.Windows.Forms.TextBox[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < txtbxInputs.Length; i++)
            {
                //インスタンス作成
                txtbxInputs[i] = new System.Windows.Forms.TextBox();
                //プロパティ設定
                txtbxInputs[i].Name = "Input" + problemStrs[i];
                txtbxInputs[i].Text = "";
                txtbxInputs[i].Size = new Size(width, height);
                txtbxInputs[i].Location = new Point(mergLeft, i * height - i * offset + mergUp);
                txtbxInputs[i].AcceptsReturn = true;
                txtbxInputs[i].Multiline = true;
                //イベントハンドラに関連付け
                txtbxInputs[i].TextChanged += new EventHandler(txtbxInputs_Changed);
                txtbxInputs[i].KeyDown += new KeyEventHandler(textBox_KeyDown);
                
            }
            //フォームにコントロールを追加
            Controls.AddRange(txtbxInputs);
            ResumeLayout(false);
        }

        private void makeAnswerTxtBox(int height, int width, int offset, int mergLeft, int mergUp, int inputAnsMerge)
        {
            txtbxAnswers = new System.Windows.Forms.TextBox[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < txtbxAnswers.Length; i++)
            {
                //インスタンス作成
                txtbxAnswers[i] = new System.Windows.Forms.TextBox();
                //プロパティ設定
                txtbxAnswers[i].Name = "GroundTruth" + problemStrs[i];
                txtbxAnswers[i].Text = "";
                txtbxAnswers[i].Size = new Size(width, height);
                txtbxAnswers[i].Location = new Point(mergLeft + width + inputAnsMerge, i * height - i * offset + mergUp);
                txtbxAnswers[i].AcceptsReturn = true;
                txtbxAnswers[i].Multiline = true;
                //イベントハンドラに関連付け
                txtbxAnswers[i].TextChanged += new EventHandler(txtbxAnswers_Changed);
                txtbxAnswers[i].KeyDown += new KeyEventHandler(textBox_KeyDown);
            }
            //フォームにコントロールを追加
            Controls.AddRange(txtbxAnswers);
            ResumeLayout(false);
        }
        private void makeSampleLable(int height, int x, int offset, int mergUp)
        {
            labelSample = new System.Windows.Forms.Label[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < labelSample.Length; i++)
            {
                //インスタンス作成
                labelSample[i] = new System.Windows.Forms.Label();
                //プロパティ設定
                labelSample[i].Name = "Sample" + problemStrs[i];
                labelSample[i].Text = "";
                labelSample[i].Location = new Point(x, i * height - i * offset + mergUp);
                labelSample[i].AutoSize = true;
            }
            //フォームにコントロールを追加
            Controls.AddRange(labelSample);
            ResumeLayout(false);
        }
        private void makeCheckLable(int height, int x, int offset, int mergUp)
        {
            labelCheck = new System.Windows.Forms.Label[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < labelCheck.Length; i++)
            {
                //インスタンス作成
                labelCheck[i] = new System.Windows.Forms.Label();
                //プロパティ設定
                labelCheck[i].Name = "Check" + problemStrs[i];
                labelCheck[i].Text = "";
                labelCheck[i].Location = new Point(x, i * height - i * offset + mergUp);
                labelCheck[i].AutoSize = true;
                labelCheck[i].Font = new Font("Arial", 12, FontStyle.Bold);
                labelCheck[i].ForeColor = Color.White;
            }
            //フォームにコントロールを追加
            Controls.AddRange(labelCheck);
            ResumeLayout(false);
        }
        private void txtbxInputs_Changed(object sender, EventArgs e)
        {
            if (tabChanged)
                tabChanged = false;
            else
            {
                string txtboxText = ((System.Windows.Forms.TextBox)sender).Text;
                string txtboxName = ((System.Windows.Forms.TextBox)sender).Name;
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
                string txtboxText = ((System.Windows.Forms.TextBox)sender).Text;
                string txtboxName = ((System.Windows.Forms.TextBox)sender).Name;
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
                ((System.Windows.Forms.TextBox)sender).SelectAll();
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
