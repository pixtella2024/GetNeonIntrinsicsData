namespace GetNeonIntrinsicsData
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblOutputFile = new Label();
            txtOutputFileName = new TextBox();
            btnSelectOutputFile = new Button();
            chkShowBrowser = new CheckBox();
            statusStrip1 = new StatusStrip();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            btnExec = new Button();
            btnCancel = new Button();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // lblOutputFile
            // 
            lblOutputFile.AutoSize = true;
            lblOutputFile.Location = new Point(12, 15);
            lblOutputFile.Name = "lblOutputFile";
            lblOutputFile.Size = new Size(65, 15);
            lblOutputFile.TabIndex = 0;
            lblOutputFile.Text = "出力ファイル";
            // 
            // txtOutputFileName
            // 
            txtOutputFileName.Enabled = false;
            txtOutputFileName.Location = new Point(83, 12);
            txtOutputFileName.Name = "txtOutputFileName";
            txtOutputFileName.Size = new Size(491, 23);
            txtOutputFileName.TabIndex = 1;
            // 
            // btnSelectOutputFile
            // 
            btnSelectOutputFile.Location = new Point(580, 12);
            btnSelectOutputFile.Name = "btnSelectOutputFile";
            btnSelectOutputFile.Size = new Size(32, 23);
            btnSelectOutputFile.TabIndex = 2;
            btnSelectOutputFile.Text = "...";
            btnSelectOutputFile.UseVisualStyleBackColor = true;
            btnSelectOutputFile.Click += btnSelectOutputFile_Click;
            // 
            // chkShowBrowser
            // 
            chkShowBrowser.AutoSize = true;
            chkShowBrowser.Location = new Point(12, 41);
            chkShowBrowser.Name = "chkShowBrowser";
            chkShowBrowser.Size = new Size(114, 19);
            chkShowBrowser.TabIndex = 3;
            chkShowBrowser.Text = "ブラウザを表示する";
            chkShowBrowser.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBar1, toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 73);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(628, 22);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(450, 16);
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(43, 17);
            toolStripStatusLabel1.Text = "待機中";
            // 
            // btnExec
            // 
            btnExec.Location = new Point(537, 41);
            btnExec.Name = "btnExec";
            btnExec.Size = new Size(75, 23);
            btnExec.TabIndex = 5;
            btnExec.Text = "実行";
            btnExec.UseVisualStyleBackColor = true;
            btnExec.Click += btnExec_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(456, 41);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "中断";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(628, 95);
            Controls.Add(btnCancel);
            Controls.Add(btnExec);
            Controls.Add(statusStrip1);
            Controls.Add(chkShowBrowser);
            Controls.Add(btnSelectOutputFile);
            Controls.Add(txtOutputFileName);
            Controls.Add(lblOutputFile);
            Enabled = false;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "NEON Intrinsic データ取得";
            FormClosing += MainForm_FormClosing;
            Shown += MainForm_Shown;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblOutputFile;
        private TextBox txtOutputFileName;
        private Button btnSelectOutputFile;
        private CheckBox chkShowBrowser;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Button btnExec;
        private Button btnCancel;
    }
}
