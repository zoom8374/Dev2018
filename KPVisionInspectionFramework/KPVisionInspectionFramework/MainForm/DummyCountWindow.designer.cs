namespace KPVisionInspectionFramework
{
    partial class DummyCountWindow
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
            this.btnConfirm = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelTrainImage = new System.Windows.Forms.Label();
            this.numUpDownDummyCount = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDummyCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConfirm
            // 
            this.btnConfirm.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfirm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfirm.Location = new System.Drawing.Point(57, 98);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(131, 38);
            this.btnConfirm.TabIndex = 23;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.Goldenrod;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(250, 30);
            this.labelTitle.TabIndex = 20;
            this.labelTitle.Text = " Dummy Count";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseDown);
            this.labelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseMove);
            // 
            // labelTrainImage
            // 
            this.labelTrainImage.BackColor = System.Drawing.Color.Black;
            this.labelTrainImage.Font = new System.Drawing.Font("나눔바른고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTrainImage.ForeColor = System.Drawing.Color.White;
            this.labelTrainImage.Location = new System.Drawing.Point(6, 38);
            this.labelTrainImage.Name = "labelTrainImage";
            this.labelTrainImage.Size = new System.Drawing.Size(104, 53);
            this.labelTrainImage.TabIndex = 24;
            this.labelTrainImage.Text = "Count";
            this.labelTrainImage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numUpDownDummyCount
            // 
            this.numUpDownDummyCount.Font = new System.Drawing.Font("나눔바른고딕", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.numUpDownDummyCount.Location = new System.Drawing.Point(116, 38);
            this.numUpDownDummyCount.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numUpDownDummyCount.Name = "numUpDownDummyCount";
            this.numUpDownDummyCount.Size = new System.Drawing.Size(127, 53);
            this.numUpDownDummyCount.TabIndex = 25;
            this.numUpDownDummyCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DummyCountWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(94)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(250, 142);
            this.Controls.Add(this.numUpDownDummyCount);
            this.Controls.Add(this.labelTrainImage);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.labelTitle);
            this.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DummyCountWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RecipeNewName";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDummyCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelTrainImage;
        private System.Windows.Forms.NumericUpDown numUpDownDummyCount;
    }
}