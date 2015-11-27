using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace AutoJudge
{
    // http://7ujm.net/CSharp/17.html

    public partial class Form1 : Form
    {
        private string fileName = "TestCases.txt";
        private int saveIdxProblemNum = 0, saveIdxTestCaseNum = 1, saveIdxProblemNowN = 2, saveIdxTestCaseStartPoint = 3;

        // ファイルへ保存
        private void saveFile()
        {
            ArrayList ar = new ArrayList();
            for (int i = 0; i < saveIdxTestCaseStartPoint; i++)
                ar.Add("");

            ar[saveIdxProblemNum] = problemNum.ToString();
            ar[saveIdxTestCaseNum] = testCaseNum.ToString();
            ar[saveIdxProblemNowN] = problemNowN.ToString();
            for (int i = 0; i < problemNum; i++)
            {
                for (int j = 0; j < testCaseNum; j++)
                {
                    ar.Add(inputs[i, j]);
                    ar.Add(groundTruth[i, j]);
                    ar.Add(samples[i, j]);
                    ar.Add(checks[i, j]);
                }
            }
            SaveFile(ar, fileName);
        }

        // ファイルから読み出し
        private void loadFile()
        {
            if (!File.Exists(fileName))
                return;

            ArrayList ar = LoadFile(fileName);

            problemNowN = Math.Min(int.Parse((string)ar[saveIdxProblemNowN]), problemNum - 1);
            problemNowS = problemStrs[problemNowN];
            int arIdx = saveIdxTestCaseStartPoint;
            for (int i = 0; i < problemNum; i++)
            {
                for (int j = 0; j < testCaseNum; j++)
                {
                    inputs[i, j] = replaceReturnCode((string)ar[arIdx++]);
                    groundTruth[i, j] = replaceReturnCode((string)ar[arIdx++]);
                    samples[i, j] = replaceReturnCode((string)ar[arIdx++]);
                    checks[i, j] = (string)ar[arIdx++];
                }
            }
            loadDataToTab();
        }

        private string replaceReturnCode(string s)
        {
            if (s != null)
                return s.Replace(((char)10).ToString(), "\r\n");
            return "";
        }

        private void SaveFile(ArrayList ar, System.String filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            XmlSerializer sr = new XmlSerializer(typeof(ArrayList), new Type[] { typeof(string) });
            sr.Serialize(fs, ar);
            fs.Close();
        }

        private ArrayList LoadFile(System.String filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlSerializer sr = new XmlSerializer(typeof(ArrayList), new Type[] { typeof(string) });
            ArrayList ar = (ArrayList)sr.Deserialize(fs);
            fs.Close();
            return ar;
        }
    }
}
