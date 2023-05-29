namespace TerrariaClassSwitcher
{
    partial class MainScreen
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.updateTick = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // updateTick
            // 
            this.updateTick.Enabled = true;
            this.updateTick.Interval = 1;
            this.updateTick.Tick += new System.EventHandler(this.updateTick_Tick);
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lime;
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Red;
            this.Name = "MainScreen";
            this.Size = new System.Drawing.Size(500, 500);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainScreen_Paint);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainScreen_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer updateTick;
    }
}
