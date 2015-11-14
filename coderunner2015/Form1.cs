using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

namespace coderunner2015
{
    public partial class Form1 : Form
    {
        System.Diagnostics.Process p;
        string result;
        string command;
        string[] problemStrs = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
        string answerFilePath = @"C:\Users\blackhat\GoogleDrive\ProgrammingContest\ruby\vimcodes\";
        string fileType = ".rb";

        public Form1()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            makeBtn();
            makeTxtBox();
        }

        private void makeBtn()
        {
            int tabHeight = 23, tabWidth = 48, tabOffset = 1, tabMergLeft = 5, tabMergUp = 10, tabNum = 8;
            this.btnTabs = new System.Windows.Forms.Button[tabNum];
            //ボタンコントロールのインスタンス作成し、プロパティを設定する
            this.SuspendLayout();
            for (int i = 0; i < this.btnTabs.Length; i++)
            {
                //インスタンス作成
                this.btnTabs[i] = new System.Windows.Forms.Button();
                //プロパティ設定
                this.btnTabs[i].Name = "btn" + problemStrs[i];
                this.btnTabs[i].Text = problemStrs[i];
                this.btnTabs[i].Size = new Size(tabWidth, tabHeight);
                this.btnTabs[i].Location = new Point(i * tabWidth - i * tabOffset + tabMergLeft, tabMergUp);
                this.btnTabs[i].FlatStyle = FlatStyle.Popup;
                //イベントハンドラに関連付け
                this.btnTabs[i].Click +=
                    new EventHandler(this.btnTabs_Click);
            }
            //フォームにコントロールを追加
            this.Controls.AddRange(this.btnTabs);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.Button[] btnTabs;
        private void btnTabs_Click(object sender, EventArgs e)
        {
            MessageBox.Show(((System.Windows.Forms.Button)sender).Name);
        }

        private void makeTxtBox()
        {
            int txtbxHeight = 50, txtbxWidth = 48, txtbxOffset = 1, txtbxMergLeft = 5, txtbxMergUp = 50, txtbxNum = 5;
            this.txtbxAnswers = new System.Windows.Forms.TextBox[txtbxNum];
            this.SuspendLayout();
            for (int i = 0; i < this.txtbxAnswers.Length; i++)
            {
                //インスタンス作成
                this.txtbxAnswers[i] = new System.Windows.Forms.TextBox();
                //プロパティ設定
                this.txtbxAnswers[i].Name = "GroundTruth" + problemStrs[i];
                this.txtbxAnswers[i].Text = "";
                this.txtbxAnswers[i].Size = new Size(txtbxWidth, txtbxHeight);
                this.txtbxAnswers[i].Location = new Point(txtbxMergLeft, i *txtbxHeight - i * txtbxOffset + txtbxMergUp);
                this.txtbxAnswers[i].AcceptsReturn = true;
                this.txtbxAnswers[i].Multiline = true;
                //イベントハンドラに関連付け
                this.txtbxAnswers[i].Click +=
                    new EventHandler(this.txtbxAnswers_Click);
            }
            //フォームにコントロールを追加
            this.Controls.AddRange(this.txtbxAnswers);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.TextBox[] txtbxAnswers;
        private void txtbxAnswers_Click(object sender, EventArgs e)
        {
            MessageBox.Show(((System.Windows.Forms.Button)sender).Name);
        }

        private void send_Click(object sender, EventArgs e)
        {
            label_SAA1.Text = excuse("A");
            if (label_SAA1.Text == textBox_GTA1.Text)
                label_CHA1.Text = "AC";
            else
                label_CHA1.Text = "WA";
        }

        private string excuse(string s)
        {
            p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "ruby.exe";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.Arguments = answerFilePath + s + fileType;
            // cmd.exe を起動します。
            p.Start();
            // cmd.exe で実行するコマンドを取得します。
            command = textBox_QA1.Text;
            // 標準入出力からリードライトするためのスレッドを起動します。
            this.StartThread();
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


        private string getArgument_result()
        {
            //return @"C:\Users\blackhat\GoogleDrive\ProgrammingContest\ruby\vimcodes\A.rb" + " < " + @"C:\Users\blackhat\Desktop\testcase.txt";
            return "C:\\Users\\blackhat\\GoogleDrive\\ProgrammingContest\\ruby\\vimcodes\\A.rb < C:\\Users\\blackhat\\Desktop\\testcase.txt";
        }

        private string startPSI(ProcessStartInfo psi)
        {
            Process p = Process.Start(psi); // アプリの実行開始
            string output = p.StandardOutput.ReadToEnd(); // 標準出力の読み取り
            return output.Replace("\r\r\n", "\n"); // 改行コードの修正
        }

        private void No_Click(object sender, EventArgs e)
        {
        }
    }
}
