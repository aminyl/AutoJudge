namespace coderunner2015
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.label_SAA1 = new System.Windows.Forms.Label();
            this.send = new System.Windows.Forms.Button();
            this.textBox_QA1 = new System.Windows.Forms.TextBox();
            this.textBox_GTA1 = new System.Windows.Forms.TextBox();
            this.label_CHA1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_A = new System.Windows.Forms.TabPage();
            this.tabPage_B = new System.Windows.Forms.TabPage();
            this.tabPage_C = new System.Windows.Forms.TabPage();
            this.tabPage_D = new System.Windows.Forms.TabPage();
            this.tabPage_E = new System.Windows.Forms.TabPage();
            this.tabPage_F = new System.Windows.Forms.TabPage();
            this.tabPage_G = new System.Windows.Forms.TabPage();
            this.tabPage_H = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage_A.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_SAA1
            // 
            this.label_SAA1.AutoSize = true;
            this.label_SAA1.Location = new System.Drawing.Point(496, 143);
            this.label_SAA1.Name = "label_SAA1";
            this.label_SAA1.Size = new System.Drawing.Size(34, 12);
            this.label_SAA1.TabIndex = 10;
            this.label_SAA1.Text = "string";
            this.label_SAA1.Click += new System.EventHandler(this.No_Click);
            // 
            // send
            // 
            this.send.Location = new System.Drawing.Point(523, 6);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(103, 79);
            this.send.TabIndex = 4;
            this.send.Text = "send";
            this.send.UseVisualStyleBackColor = true;
            this.send.Click += new System.EventHandler(this.send_Click);
            // 
            // textBox_QA1
            // 
            this.textBox_QA1.AcceptsReturn = true;
            this.textBox_QA1.AcceptsTab = true;
            this.textBox_QA1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.textBox_QA1.Location = new System.Drawing.Point(20, 140);
            this.textBox_QA1.Multiline = true;
            this.textBox_QA1.Name = "textBox_QA1";
            this.textBox_QA1.Size = new System.Drawing.Size(231, 112);
            this.textBox_QA1.TabIndex = 11;
            // 
            // textBox_GTA1
            // 
            this.textBox_GTA1.AcceptsReturn = true;
            this.textBox_GTA1.AcceptsTab = true;
            this.textBox_GTA1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.textBox_GTA1.Location = new System.Drawing.Point(257, 140);
            this.textBox_GTA1.Multiline = true;
            this.textBox_GTA1.Name = "textBox_GTA1";
            this.textBox_GTA1.Size = new System.Drawing.Size(231, 86);
            this.textBox_GTA1.TabIndex = 11;
            // 
            // label_CHA1
            // 
            this.label_CHA1.AutoSize = true;
            this.label_CHA1.Location = new System.Drawing.Point(568, 143);
            this.label_CHA1.Name = "label_CHA1";
            this.label_CHA1.Size = new System.Drawing.Size(34, 12);
            this.label_CHA1.TabIndex = 10;
            this.label_CHA1.Text = "string";
            this.label_CHA1.Click += new System.EventHandler(this.No_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_A);
            this.tabControl1.Controls.Add(this.tabPage_B);
            this.tabControl1.Controls.Add(this.tabPage_C);
            this.tabControl1.Controls.Add(this.tabPage_D);
            this.tabControl1.Controls.Add(this.tabPage_E);
            this.tabControl1.Controls.Add(this.tabPage_F);
            this.tabControl1.Controls.Add(this.tabPage_G);
            this.tabControl1.Controls.Add(this.tabPage_H);
            this.tabControl1.Location = new System.Drawing.Point(21, 250);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(650, 462);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage_A
            // 
            this.tabPage_A.Controls.Add(this.textBox_GTA1);
            this.tabPage_A.Controls.Add(this.send);
            this.tabPage_A.Controls.Add(this.label_SAA1);
            this.tabPage_A.Controls.Add(this.textBox_QA1);
            this.tabPage_A.Controls.Add(this.label_CHA1);
            this.tabPage_A.Location = new System.Drawing.Point(4, 22);
            this.tabPage_A.Name = "tabPage_A";
            this.tabPage_A.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_A.Size = new System.Drawing.Size(642, 436);
            this.tabPage_A.TabIndex = 0;
            this.tabPage_A.Text = "A";
            this.tabPage_A.UseVisualStyleBackColor = true;
            // 
            // tabPage_B
            // 
            this.tabPage_B.Location = new System.Drawing.Point(4, 22);
            this.tabPage_B.Name = "tabPage_B";
            this.tabPage_B.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_B.Size = new System.Drawing.Size(515, 332);
            this.tabPage_B.TabIndex = 1;
            this.tabPage_B.Text = "B";
            this.tabPage_B.UseVisualStyleBackColor = true;
            // 
            // tabPage_C
            // 
            this.tabPage_C.Location = new System.Drawing.Point(4, 22);
            this.tabPage_C.Name = "tabPage_C";
            this.tabPage_C.Size = new System.Drawing.Size(515, 332);
            this.tabPage_C.TabIndex = 2;
            this.tabPage_C.Text = "C";
            this.tabPage_C.UseVisualStyleBackColor = true;
            // 
            // tabPage_D
            // 
            this.tabPage_D.Location = new System.Drawing.Point(4, 22);
            this.tabPage_D.Name = "tabPage_D";
            this.tabPage_D.Size = new System.Drawing.Size(515, 332);
            this.tabPage_D.TabIndex = 3;
            this.tabPage_D.Text = "D";
            this.tabPage_D.UseVisualStyleBackColor = true;
            // 
            // tabPage_E
            // 
            this.tabPage_E.Location = new System.Drawing.Point(4, 22);
            this.tabPage_E.Name = "tabPage_E";
            this.tabPage_E.Size = new System.Drawing.Size(515, 332);
            this.tabPage_E.TabIndex = 4;
            this.tabPage_E.Text = "E";
            this.tabPage_E.UseVisualStyleBackColor = true;
            // 
            // tabPage_F
            // 
            this.tabPage_F.Location = new System.Drawing.Point(4, 22);
            this.tabPage_F.Name = "tabPage_F";
            this.tabPage_F.Size = new System.Drawing.Size(515, 332);
            this.tabPage_F.TabIndex = 5;
            this.tabPage_F.Text = "F";
            this.tabPage_F.UseVisualStyleBackColor = true;
            // 
            // tabPage_G
            // 
            this.tabPage_G.Location = new System.Drawing.Point(4, 22);
            this.tabPage_G.Name = "tabPage_G";
            this.tabPage_G.Size = new System.Drawing.Size(515, 332);
            this.tabPage_G.TabIndex = 6;
            this.tabPage_G.Text = "G";
            this.tabPage_G.UseVisualStyleBackColor = true;
            // 
            // tabPage_H
            // 
            this.tabPage_H.Location = new System.Drawing.Point(4, 22);
            this.tabPage_H.Name = "tabPage_H";
            this.tabPage_H.Size = new System.Drawing.Size(515, 332);
            this.tabPage_H.TabIndex = 7;
            this.tabPage_H.Text = "H";
            this.tabPage_H.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 703);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage_A.ResumeLayout(false);
            this.tabPage_A.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label_SAA1;
        private System.Windows.Forms.Button send;
        private System.Windows.Forms.TextBox textBox_QA1;
        private System.Windows.Forms.TextBox textBox_GTA1;
        private System.Windows.Forms.Label label_CHA1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_A;
        private System.Windows.Forms.TabPage tabPage_B;
        private System.Windows.Forms.TabPage tabPage_C;
        private System.Windows.Forms.TabPage tabPage_D;
        private System.Windows.Forms.TabPage tabPage_E;
        private System.Windows.Forms.TabPage tabPage_F;
        private System.Windows.Forms.TabPage tabPage_G;
        private System.Windows.Forms.TabPage tabPage_H;
    }
}

