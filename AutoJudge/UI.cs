using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace AutoJudge
{
    public class DoubleClickButton : Button
    {
        public DoubleClickButton() : base()
        {
            SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
        }
    }

    public partial class Form1 : Form
    {
        private Button[] btnTabs;
        private Button btnUpdate;
        private DoubleClickButton btnRefresh;
        private Button btnMinimize;
        private Button btnClose;
        private TextBox[] txtbxInputs;
        private TextBox[] txtbxGroundTruth;
        private Label[] labelSample;
        private Label[] labelCheck;
        private Label labelBack;

        // ウィンドウ移動のためにマウスのクリック位置を記憶
        private Point mousePoint;

        private void MakeControlls()
        {
            int formSizeX = 640, formSizeY = 480;
            // フォームのサイズ変更
            ClientSize = new Size(formSizeX, formSizeY);
            MaximizeBox = false;

            // サイズ変更の検知
            SizeChanged += new EventHandler(Form1_SizeChanged);

            int tabHeight = 23, tabWidth = 48, tabOffset = 1, tabMergLeft = 0, tabMergUp = 0;
            int updateBtnHeight = tabHeight, updateBtnWidth = tabHeight;
            int updateBtnX = formSizeX - 4 * tabHeight, updateBtnY = tabMergUp;
            MakeTabBtn(tabHeight, tabWidth, tabOffset, tabMergLeft, tabMergUp);
            MakeUpdateBtn(updateBtnHeight, updateBtnWidth, updateBtnX, updateBtnY);
            MakeRefreshBtn(updateBtnHeight, updateBtnWidth, updateBtnX - updateBtnWidth, updateBtnY);

            if (style != 0)
            {
                MakeCloseBtn(tabHeight, tabHeight, formSizeX - tabHeight, 0);
                MakeMinimizeBtn(tabHeight, tabHeight, formSizeX - 2 * tabHeight, 0);
            }

            int txtbxHeight = 85, txtbxWidth = (int)(txtbxHeight * 1.618), txtbxOffset = 1, txtbxMergLeft = 10, txtbxMergUp = 50;
            int inputGtMerge = 10;
            int sampleLblX = txtbxMergLeft + 2 * txtbxWidth + inputGtMerge;
            int checkLblX = txtbxMergLeft + 3 * txtbxWidth + inputGtMerge;
            MakeInputTxtBox(txtbxHeight, txtbxWidth, txtbxOffset, txtbxMergLeft, txtbxMergUp);
            MakeGroundTruthTxtBox(txtbxHeight, txtbxWidth, txtbxOffset, txtbxMergLeft, txtbxMergUp, inputGtMerge);
            MakeSampleLable(txtbxHeight, sampleLblX, txtbxOffset, txtbxMergUp);
            MakeCheckLable(txtbxHeight, checkLblX, txtbxOffset, txtbxMergUp);

            // 背景の描画, コントロールの重なり順の制御が分からないので最後に書く
            SetBackgroundImage();
        }

        private void MakeBtn(Button btn, string name, int height, int width, int x, int y)
        {
            btn.Name = name;
            btn.Size = new Size(width, height);
            btn.Location = new Point(x, y);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.Transparent;
            btn.FlatAppearance.MouseOverBackColor = Color.White;
            btn.FlatAppearance.BorderSize = 0;
        }

        // 色指定必要
        private void MakeBtn(Button btn, string name, string text, int height, int width, int x, int y, EventHandler e)
        {
            SuspendLayout();
            MakeBtn(btn, name, height, width, x, y);
            // プロパティ設定
            btn.Text = text;
            // イベントハンドラに関連付け
            btn.Click += e;
            // フォームにコントロールを追加
            Controls.Add(btn);
            ResumeLayout(false);
        }

        private void MakeTabBtn(int height, int width, int offset, int mergLeft, int mergUp)
        {
            btnTabs = new Button[problemNum];
            for (int i = 0; i < btnTabs.Length; i++)
            {
                btnTabs[i] = new Button();
                int x = i * width - i * offset + mergLeft;
                int y = mergUp;
                MakeBtn(btnTabs[i], "btnTab" + problemStrs[i], problemStrs[i], height, width, x, y, new EventHandler(BtnTabs_Click));
            }
            btnTabs[problemNowN].BackColor = Color.White;
        }

        private void MakeCloseBtn(int height, int width, int x, int y)
        {
            btnClose = new Button();
            MakeBtn(btnClose, "btnCloase", "X", height, width, x, y, new EventHandler(BtnClose_Click));
        }

        private void MakeMinimizeBtn(int height, int width, int x, int y)
        {
            btnMinimize = new Button();
            MakeBtn(btnMinimize, "btnMinimize", "-", height, width, x, y, new EventHandler(BtnMinimize_Click));
        }

        private void MakeUpdateBtn(int height, int width, int x, int y)
        {
            SuspendLayout();
            btnUpdate = new Button();
            MakeBtn(btnUpdate, "btnUpdate", height, width, x, y);
            btnUpdate.BackgroundImageLayout = ImageLayout.Zoom;
            btnUpdate.MouseUp += new MouseEventHandler(BtnUpdate_MouseUp);
            Controls.Add(btnUpdate);
            SetUpdateBtn(1);
            ResumeLayout(false);
        }

        // アップデートボタンの二つのモード
        private void SetUpdateBtn(int mode)
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

        private void MakeRefreshBtn(int height, int width, int x, int y)
        {
            SuspendLayout();
            btnRefresh = new DoubleClickButton();
            MakeBtn(btnRefresh, "btnClear", height, width, x, y);
            btnRefresh.BackgroundImageLayout = ImageLayout.Zoom;
            btnRefresh.BackgroundImage = Properties.Resources.refresh; // http://icooon-mono.com/
            // イベントハンドラに関連付け
            btnRefresh.DoubleClick += new EventHandler(BtnRefresh_DoubleClick);
            // フォームにコントロールを追加
            Controls.Add(btnRefresh);
            ResumeLayout(false);
        }

        private void MakeInputTxtBox(int height, int width, int offset, int mergLeft, int mergUp)
        {
            txtbxInputs = new TextBox[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < txtbxInputs.Length; i++)
            {
                // インスタンス作成
                txtbxInputs[i] = new TextBox();
                // プロパティ設定
                txtbxInputs[i].Name = "Input" + problemStrs[i];
                txtbxInputs[i].Text = "";
                txtbxInputs[i].Size = new Size(width, height);
                txtbxInputs[i].Location = new Point(mergLeft, i * height - i * offset + mergUp);
                txtbxInputs[i].AcceptsReturn = true;
                txtbxInputs[i].Multiline = true;
                // イベントハンドラに関連付け
                txtbxInputs[i].TextChanged += new EventHandler(TxtbxInputs_Changed);
                txtbxInputs[i].KeyDown += new KeyEventHandler(TextBox_KeyDown);

            }
            // フォームにコントロールを追加
            Controls.AddRange(txtbxInputs);
            ResumeLayout(false);
        }

        private void MakeGroundTruthTxtBox(int height, int width, int offset, int mergLeft, int mergUp, int inputGtMerge)
        {
            txtbxGroundTruth = new TextBox[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < txtbxGroundTruth.Length; i++)
            {
                // インスタンス作成
                txtbxGroundTruth[i] = new TextBox();
                // プロパティ設定
                txtbxGroundTruth[i].Name = "GroundTruth" + problemStrs[i];
                txtbxGroundTruth[i].Text = "";
                txtbxGroundTruth[i].Size = new Size(width, height);
                txtbxGroundTruth[i].Location = new Point(mergLeft + width + inputGtMerge, i * height - i * offset + mergUp);
                txtbxGroundTruth[i].AcceptsReturn = true;
                txtbxGroundTruth[i].Multiline = true;
                // イベントハンドラに関連付け
                txtbxGroundTruth[i].TextChanged += new EventHandler(TxtbxGroundTruth_Changed);
                txtbxGroundTruth[i].KeyDown += new KeyEventHandler(TextBox_KeyDown);
            }
            //フォームにコントロールを追加
            Controls.AddRange(txtbxGroundTruth);
            ResumeLayout(false);
        }

        private void MakeSampleLable(int height, int x, int offset, int mergUp)
        {
            labelSample = new Label[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < labelSample.Length; i++)
            {
                // インスタンス作成
                labelSample[i] = new Label();
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

        private void MakeCheckLable(int height, int x, int offset, int mergUp)
        {
            labelCheck = new Label[testCaseNum];
            SuspendLayout();
            for (int i = 0; i < labelCheck.Length; i++)
            {
                // インスタンス作成
                labelCheck[i] = new Label();
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
        private Image SetBackgroundImage()
        {
            // http://dobon.net/vb/dotnet/graphics/hadeinimage.html
            // 画像を読み込む
            if (!System.IO.File.Exists(backgroundImagePath))
                return null;
            Image img = System.Drawing.Image.FromFile(backgroundImagePath);
            int bgImgWith = img.Width, bgImgHeight = img.Height;
            // 描画先とするImageオブジェクトを作成する
            Bitmap canvas = new Bitmap(bgImgWith, bgImgHeight);
            // ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = Graphics.FromImage(canvas);
            // ColorMatrixオブジェクトの作成
            ColorMatrix cm = new ColorMatrix();
            // ColorMatrixの行列の値を変更し，好きな色にする
            cm.Matrix00 = backgroundMatrixV[0];
            cm.Matrix11 = backgroundMatrixV[1];
            cm.Matrix22 = backgroundMatrixV[2];
            cm.Matrix33 = backgroundMatrixV[3];
            cm.Matrix44 = backgroundMatrixV[4];
            // ImageAttributesオブジェクトの作成
            ImageAttributes ia = new ImageAttributes();
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
        private void SetBackgroundImage(int x, int y)
        {
            Image backImage = SetBackgroundImage();
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