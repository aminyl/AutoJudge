using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoJudge
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Button[] btnTabs;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox[] txtbxInputs;
        private System.Windows.Forms.TextBox[] txtbxAnswers;
        private System.Windows.Forms.Label[] labelSample;
        private System.Windows.Forms.Label[] labelCheck;
        private System.Windows.Forms.Label labelBack;

        // ウィンドウ移動のためにマウスのクリック位置を記憶
        private Point mousePoint;

        private void makeControlls()
        {
            int formSizeX = 560, formSizeY = 480;
            // フォームのサイズ変更
            ClientSize = new System.Drawing.Size(formSizeX, formSizeY);
            // サイズ変更の検知
            this.SizeChanged += new EventHandler(Form1_SizeChanged);

            int tabHeight = 23, tabWidth = 48, tabOffset = 1, tabMergLeft = 0, tabMergUp = 0;
            int updateBtnHeight = tabHeight, updateBtnWidth = tabHeight;
            int updateBtnX = formSizeX - 4 * tabHeight, updateBtnY = tabMergUp;
            makeTabBtn(tabHeight, tabWidth, tabOffset, tabMergLeft, tabMergUp);
            makeUpdateBtn(updateBtnHeight, updateBtnWidth, updateBtnX, updateBtnY);

            if (style != 0)
            {
                makeCloseBtn(tabHeight, tabHeight, formSizeX - tabHeight, 0);
                makeMinimizeBtn(tabHeight, tabHeight, formSizeX - 2 * tabHeight, 0);
            }

            int txtbxHeight = 80, txtbxWidth = (int)(txtbxHeight * 1.618), txtbxOffset = 1, txtbxMergLeft = 5, txtbxMergUp = 50;
            int inputAnsMerge = 10;
            int sampleLblX = txtbxMergLeft + 2 * txtbxWidth + inputAnsMerge;
            int checkLblX = txtbxMergLeft + 3 * txtbxWidth + inputAnsMerge;
            makeInputTxtBox(txtbxHeight, txtbxWidth, txtbxOffset, txtbxMergLeft, txtbxMergUp);
            makeAnswerTxtBox(txtbxHeight, txtbxWidth, txtbxOffset, txtbxMergLeft, txtbxMergUp, inputAnsMerge);
            makeSampleLable(txtbxHeight, sampleLblX, txtbxOffset, txtbxMergUp);
            makeCheckLable(txtbxHeight, checkLblX, txtbxOffset, txtbxMergUp);

            // 背景の描画, コントロールの重なり順の制御が分からないので最後に書く
            //setBackgroundImage(Properties.Resources.backgroundImage);
        }

        // 色指定必要
        private void makeBtn(Button btn, string name, string text, int height, int width, int x, int y, EventHandler e)
        {
            SuspendLayout();
            // プロパティ設定
            btn.Name = name;
            btn.Text = text;
            btn.Size = new Size(width, height);
            btn.Location = new Point(x, y);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.Transparent;
            btn.FlatAppearance.MouseOverBackColor = Color.White;
            btn.FlatAppearance.BorderSize = 0;
            // イベントハンドラに関連付け
            btn.Click += e;
            // フォームにコントロールを追加
            Controls.Add(btn);
            ResumeLayout(false);
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
            btnTabs[problemNowN].BackColor = Color.White;
        }

        private void makeCloseBtn(int height, int width, int x, int y)
        {
            btnClose = new System.Windows.Forms.Button();
            makeBtn(btnClose, "btnCloase", "X", height, width, x, y, new EventHandler(btnClose_Click));
        }

        private void makeMinimizeBtn(int height, int width, int x, int y)
        {
            btnMinimize = new System.Windows.Forms.Button();
            makeBtn(btnMinimize, "btnMinimize", "-", height, width, x, y, new EventHandler(btnMinimize_Click));
        }

        private void makeUpdateBtn(int height, int width, int x, int y)
        {
            btnUpdate = new System.Windows.Forms.Button();
            SuspendLayout();
            // プロパティ設定
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(width, height);
            btnUpdate.Location = new Point(x, y);
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.FlatAppearance.MouseDownBackColor = Color.Gray;
            btnUpdate.BackgroundImageLayout = ImageLayout.Zoom;
            // イベントハンドラに関連付け
            btnUpdate.MouseUp += new MouseEventHandler(btnUpdate_MouseUp);
            // フォームにコントロールを追加
            Controls.Add(btnUpdate);
            setUpdateBtn(1);
            ResumeLayout(false);
        }

        // アップデートボタンの二つのモード
        private void setUpdateBtn(int mode)
        {
            switch (mode)
            {
                case 0: // ボタンを押した時に更新
                    btnUpdate.FlatAppearance.MouseOverBackColor = Color.White;
                    btnUpdate.BackColor = Color.Transparent;
                    btnUpdate.BackgroundImage = Properties.Resources.update;
                    break;
                case 1: // アクティブになるたびに更新
                    btnUpdate.FlatAppearance.MouseOverBackColor = Color.Gray;
                    btnUpdate.BackColor = Color.Gray;
                    btnUpdate.BackgroundImage = null;
                    break;
                default:
                    break;
            }
        }

        private void makeInputTxtBox(int height, int width, int offset, int mergLeft, int mergUp)
        {
            txtbxInputs = new System.Windows.Forms.TextBox[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < txtbxInputs.Length; i++)
            {
                // インスタンス作成
                txtbxInputs[i] = new System.Windows.Forms.TextBox();
                // プロパティ設定
                txtbxInputs[i].Name = "Input" + problemStrs[i];
                txtbxInputs[i].Text = "";
                txtbxInputs[i].Size = new Size(width, height);
                txtbxInputs[i].Location = new Point(mergLeft, i * height - i * offset + mergUp);
                txtbxInputs[i].AcceptsReturn = true;
                txtbxInputs[i].Multiline = true;
                // イベントハンドラに関連付け
                txtbxInputs[i].TextChanged += new EventHandler(txtbxInputs_Changed);
                txtbxInputs[i].KeyDown += new KeyEventHandler(textBox_KeyDown);

            }
            // フォームにコントロールを追加
            Controls.AddRange(txtbxInputs);
            ResumeLayout(false);
        }

        private void makeAnswerTxtBox(int height, int width, int offset, int mergLeft, int mergUp, int inputAnsMerge)
        {
            txtbxAnswers = new System.Windows.Forms.TextBox[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < txtbxAnswers.Length; i++)
            {
                // インスタンス作成
                txtbxAnswers[i] = new System.Windows.Forms.TextBox();
                // プロパティ設定
                txtbxAnswers[i].Name = "GroundTruth" + problemStrs[i];
                txtbxAnswers[i].Text = "";
                txtbxAnswers[i].Size = new Size(width, height);
                txtbxAnswers[i].Location = new Point(mergLeft + width + inputAnsMerge, i * height - i * offset + mergUp);
                txtbxAnswers[i].AcceptsReturn = true;
                txtbxAnswers[i].Multiline = true;
                // イベントハンドラに関連付け
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
                // インスタンス作成
                labelSample[i] = new System.Windows.Forms.Label();
                // プロパティ設定
                labelSample[i].Name = "Sample" + problemStrs[i];
                labelSample[i].Text = "";
                labelSample[i].Location = new Point(x, i * height - i * offset + mergUp);
                labelSample[i].AutoSize = true;
                labelSample[i].BackColor = Color.Transparent;
            }
            // フォームにコントロールを追加
            Controls.AddRange(labelSample);
            ResumeLayout(false);
        }

        private void makeCheckLable(int height, int x, int offset, int mergUp)
        {
            labelCheck = new System.Windows.Forms.Label[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < labelCheck.Length; i++)
            {
                // インスタンス作成
                labelCheck[i] = new System.Windows.Forms.Label();
                // プロパティ設定
                labelCheck[i].Name = "Check" + problemStrs[i];
                labelCheck[i].Text = "";
                labelCheck[i].Location = new Point(x, i * height - i * offset + mergUp);
                labelCheck[i].AutoSize = true;
                labelCheck[i].Font = new Font("Arial", 12, FontStyle.Bold);
                labelCheck[i].ForeColor = Color.White;
                // labelCheck[i].BackColor = Color.Transparent;
            }
            // フォームにコントロールを追加
            Controls.AddRange(labelCheck);
            ResumeLayout(false);
        }

        // 背景の描画
        private Image setBackgroundImage(Image img)
        {
            // http://dobon.net/vb/dotnet/graphics/hadeinimage.html
            // 画像を読み込む
            int bgImgWith = img.Width, bgImgHeight = img.Height;
            // 描画先とするImageオブジェクトを作成する
            Bitmap canvas = new Bitmap(bgImgWith, bgImgHeight);
            // ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = Graphics.FromImage(canvas);
            // ColorMatrixオブジェクトの作成
            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
            // ColorMatrixの行列の値を変更し，好きな色にする
            cm.Matrix00 = 1;
            cm.Matrix11 = 0.9f;
            cm.Matrix22 = 1;
            cm.Matrix33 = 0.5F;
            cm.Matrix44 = 1;
            // ImageAttributesオブジェクトの作成
            System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
            // ColorMatrixを設定する
            ia.SetColorMatrix(cm);
            // ImageAttributesを使用して画像を描画
            g.DrawImage(img, new Rectangle(0, 0, bgImgWith, bgImgHeight), 0, 0, bgImgWith, bgImgHeight, GraphicsUnit.Pixel, ia);
            // リソースを解放する
            img.Dispose();
            g.Dispose();
            return BackgroundImage = canvas;
        }

        // 特定の範囲への背景の描画
        private void setBackgroundImage(Image img, int x, int y)
        {
            Image backImage = setBackgroundImage(img);
            SuspendLayout();
            // インスタンス作成
            labelBack = new Label();
            // プロパティ設定
            labelBack.Name = "backgroundLabel";
            labelBack.Location = new Point(x, y);
            labelBack.Size = new Size(backImage.Width, backImage.Height);
            labelBack.AutoSize = false;
            // フォームにコントロールを追加
            Controls.Add(labelBack);
            ResumeLayout(false);

            labelBack.BackgroundImage = backImage;
            labelBack.MouseDown += new MouseEventHandler(Form1_MouseDown);
            labelBack.MouseMove += new MouseEventHandler(Form1_MouseMove);
            BackgroundImage = null;
            // labelBack.SendToBack();
            Controls.Add(labelBack);
            // BackgroundImageLayout = ImageLayout.Zoom;
        }

    }
}