using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace AutoJudge
{
    public partial class Form1 : Form
    {
        System.Diagnostics.Process p; // コマンドラインで実行
        string result, command; // 実行結果, 入力

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
    }
}
