using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace AutoJudge
{
    public partial class Form1 : Form
    {
        int timeLimit = 2000; // TLEまでの時間,プログラム実行中,最大この時間UIがフリーズする

        System.Diagnostics.Process p; // コマンドラインで実行
        string result, command; // 実行結果, 入力
        Thread rThread;

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
            //startExeThread();
            StartThread();
            Console.WriteLine("start excute");
            // cmd.exe が終了するのを2000msec待つ
            p.WaitForExit(2000);
            // 終了していなければTLEとする
            if (!p.HasExited)
            {
                Console.WriteLine("TLE");
                p.Kill(); // 終了させる
                p.WaitForExit(timeLimit); // 終了するまで待つ(数msecかかる)
                Console.WriteLine(filename);
                errorFlag = TLE;
            }
            else
            {
                Console.WriteLine("no error");
                errorFlag = "";
            }
            Console.WriteLine("end excute");
            return result;
        }

        public void StartThread()
        {
            ThreadStart readThread = new ThreadStart(ReadThread);
            ThreadStart writeThread = new ThreadStart(WriteThread);
            rThread = new Thread(readThread);
            Thread wThread = new Thread(writeThread);
            rThread.Name = "ReadThread";
            wThread.Name = "WriteThread";
            rThread.Start();       //starting the read thread
            wThread.Start();       //starting the write thread
            wThread.Join();

            // for long process
            Console.WriteLine("Start joinThread");
            Thread t = new Thread(new ThreadStart(joinThread));
            t.IsBackground = true;
            t.Start();
            Console.WriteLine("end StartThread");
        }

        private void joinThread()
        {
            Console.WriteLine("enter join thread");
            rThread.Join();
            Console.WriteLine("end join thread");
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
