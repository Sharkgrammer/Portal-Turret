namespace Portal_Turret
{
    partial class mainForm
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
            this.components = new System.ComponentModel.Container();
            this.tBody = new System.Windows.Forms.PictureBox();
            this.movement = new System.Windows.Forms.Timer(this.components);
            this.camera = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tBody)).BeginInit();
            this.SuspendLayout();
            // 
            // tBody
            // 
            this.tBody.BackColor = System.Drawing.Color.Transparent;
            this.tBody.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tBody.Image = global::Portal_Turret.Properties.Resources.Turret_Body__2_;
            this.tBody.Location = new System.Drawing.Point(267, 0);
            this.tBody.Name = "tBody";
            this.tBody.Size = new System.Drawing.Size(481, 693);
            this.tBody.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.tBody.TabIndex = 2;
            this.tBody.TabStop = false;
            // 
            // movement
            // 
            this.movement.Interval = 10;
            this.movement.Tick += new System.EventHandler(this.movement_Tick);
            // 
            // camera
            // 
            this.camera.Tick += new System.EventHandler(this.camera_Tick);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 762);
            this.Controls.Add(this.tBody);
            this.Name = "mainForm";
            this.Text = ":(";
            ((System.ComponentModel.ISupportInitialize)(this.tBody)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox tBody;
        private System.Windows.Forms.Timer movement;
        private System.Windows.Forms.Timer camera;
    }
}

