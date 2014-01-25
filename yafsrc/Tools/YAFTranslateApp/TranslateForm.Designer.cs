namespace YAF.TranslateApp
{
    partial class TranslateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TranslateForm));
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblSourceTranslationFile = new System.Windows.Forms.Label();
            this.lblDestinationTranslationFile = new System.Windows.Forms.Label();
            this.tbxSourceTranslationFile = new System.Windows.Forms.TextBox();
            this.tbxDestinationTranslationFile = new System.Windows.Forms.TextBox();
            this.btnLoadSourceTranslation = new System.Windows.Forms.Button();
            this.btnLoadDestinationTranslation = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grid1 = new SourceGrid.Grid();
            this.btnPopulateTranslations = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.checkPendingOnly = new System.Windows.Forms.CheckBox();
            this.btnAutoTranslate = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.Location = new System.Drawing.Point(793, 622);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(75, 23);
            this.btnQuit.TabIndex = 0;
            this.btnQuit.Text = "&Quit";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.BtnQuitClick);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(672, 622);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(115, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save Language";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSaveClick);
            // 
            // lblSourceTranslationFile
            // 
            this.lblSourceTranslationFile.AutoSize = true;
            this.lblSourceTranslationFile.Location = new System.Drawing.Point(9, 9);
            this.lblSourceTranslationFile.Name = "lblSourceTranslationFile";
            this.lblSourceTranslationFile.Size = new System.Drawing.Size(171, 13);
            this.lblSourceTranslationFile.TabIndex = 4;
            this.lblSourceTranslationFile.Text = "Source translation file (english.xml):";
            // 
            // lblDestinationTranslationFile
            // 
            this.lblDestinationTranslationFile.AutoSize = true;
            this.lblDestinationTranslationFile.Location = new System.Drawing.Point(9, 48);
            this.lblDestinationTranslationFile.Name = "lblDestinationTranslationFile";
            this.lblDestinationTranslationFile.Size = new System.Drawing.Size(130, 13);
            this.lblDestinationTranslationFile.TabIndex = 5;
            this.lblDestinationTranslationFile.Text = "Destination translation file:";
            // 
            // tbxSourceTranslationFile
            // 
            this.tbxSourceTranslationFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxSourceTranslationFile.Location = new System.Drawing.Point(12, 25);
            this.tbxSourceTranslationFile.Name = "tbxSourceTranslationFile";
            this.tbxSourceTranslationFile.ReadOnly = true;
            this.tbxSourceTranslationFile.Size = new System.Drawing.Size(544, 20);
            this.tbxSourceTranslationFile.TabIndex = 6;
            // 
            // tbxDestinationTranslationFile
            // 
            this.tbxDestinationTranslationFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxDestinationTranslationFile.Location = new System.Drawing.Point(12, 64);
            this.tbxDestinationTranslationFile.Name = "tbxDestinationTranslationFile";
            this.tbxDestinationTranslationFile.ReadOnly = true;
            this.tbxDestinationTranslationFile.Size = new System.Drawing.Size(544, 20);
            this.tbxDestinationTranslationFile.TabIndex = 7;
            // 
            // btnLoadSourceTranslation
            // 
            this.btnLoadSourceTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadSourceTranslation.Location = new System.Drawing.Point(562, 23);
            this.btnLoadSourceTranslation.Name = "btnLoadSourceTranslation";
            this.btnLoadSourceTranslation.Size = new System.Drawing.Size(165, 23);
            this.btnLoadSourceTranslation.TabIndex = 8;
            this.btnLoadSourceTranslation.Text = "Select source translation...";
            this.btnLoadSourceTranslation.UseVisualStyleBackColor = true;
            this.btnLoadSourceTranslation.Click += new System.EventHandler(this.BtnLoadSourceTranslationClick);
            // 
            // btnLoadDestinationTranslation
            // 
            this.btnLoadDestinationTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadDestinationTranslation.Location = new System.Drawing.Point(562, 62);
            this.btnLoadDestinationTranslation.Name = "btnLoadDestinationTranslation";
            this.btnLoadDestinationTranslation.Size = new System.Drawing.Size(165, 23);
            this.btnLoadDestinationTranslation.TabIndex = 9;
            this.btnLoadDestinationTranslation.Text = "Select destination translation...";
            this.btnLoadDestinationTranslation.UseVisualStyleBackColor = true;
            this.btnLoadDestinationTranslation.Click += new System.EventHandler(this.BtnLoadDestinationTranslationClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(9, 625);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Red text = untranslated (equal to source translation)";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.grid1);
            this.panel1.Location = new System.Drawing.Point(12, 109);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(856, 507);
            this.panel1.TabIndex = 12;
            // 
            // grid1
            // 
            this.grid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grid1.EnableSort = true;
            this.grid1.Location = new System.Drawing.Point(3, 3);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.Size = new System.Drawing.Size(848, 499);
            this.grid1.TabIndex = 5;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            // 
            // btnPopulateTranslations
            // 
            this.btnPopulateTranslations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPopulateTranslations.Location = new System.Drawing.Point(733, 61);
            this.btnPopulateTranslations.Name = "btnPopulateTranslations";
            this.btnPopulateTranslations.Size = new System.Drawing.Size(143, 23);
            this.btnPopulateTranslations.TabIndex = 13;
            this.btnPopulateTranslations.Text = "Populate translation";
            this.btnPopulateTranslations.UseVisualStyleBackColor = true;
            this.btnPopulateTranslations.Click += new System.EventHandler(this.BtnPopulateTranslationsClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 651);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(880, 22);
            this.statusStrip1.TabIndex = 14;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(400, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(37, 17);
            this.toolStripStatusLabel1.Text = "          ";
            // 
            // checkPendingOnly
            // 
            this.checkPendingOnly.AutoSize = true;
            this.checkPendingOnly.Location = new System.Drawing.Point(12, 86);
            this.checkPendingOnly.Name = "checkPendingOnly";
            this.checkPendingOnly.Size = new System.Drawing.Size(177, 17);
            this.checkPendingOnly.TabIndex = 15;
            this.checkPendingOnly.Text = "Show only Pending Translations";
            this.checkPendingOnly.UseVisualStyleBackColor = true;
            this.checkPendingOnly.CheckedChanged += new System.EventHandler(this.CheckPendingOnlyCheckedChanged);
            // 
            // btnAutoTranslate
            // 
            this.btnAutoTranslate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoTranslate.Enabled = false;
            this.btnAutoTranslate.Location = new System.Drawing.Point(527, 622);
            this.btnAutoTranslate.Name = "btnAutoTranslate";
            this.btnAutoTranslate.Size = new System.Drawing.Size(139, 23);
            this.btnAutoTranslate.TabIndex = 16;
            this.btnAutoTranslate.Text = "Auto Translate Pending ";
            this.btnAutoTranslate.UseVisualStyleBackColor = true;
            this.btnAutoTranslate.Click += new System.EventHandler(this.AutoTranslateAll);
            // 
            // TranslateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 673);
            this.Controls.Add(this.btnAutoTranslate);
            this.Controls.Add(this.checkPendingOnly);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnPopulateTranslations);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLoadDestinationTranslation);
            this.Controls.Add(this.btnLoadSourceTranslation);
            this.Controls.Add(this.tbxDestinationTranslationFile);
            this.Controls.Add(this.tbxSourceTranslationFile);
            this.Controls.Add(this.lblDestinationTranslationFile);
            this.Controls.Add(this.lblSourceTranslationFile);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnQuit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(896, 711);
            this.Name = "TranslateForm";
            this.Text = "YAF Translation 1.10";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TranslateFormFormClosing);
            this.Load += new System.EventHandler(this.TranslateForm_Load);
            this.panel1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblSourceTranslationFile;
        private System.Windows.Forms.Label lblDestinationTranslationFile;
        private System.Windows.Forms.TextBox tbxSourceTranslationFile;
        private System.Windows.Forms.TextBox tbxDestinationTranslationFile;
        private System.Windows.Forms.Button btnLoadSourceTranslation;
        private System.Windows.Forms.Button btnLoadDestinationTranslation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPopulateTranslations;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.CheckBox checkPendingOnly;
        private System.Windows.Forms.Button btnAutoTranslate;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private SourceGrid.Grid grid1;
    }
}

