namespace Theaterverdeling
{
    partial class PreviewForm
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
            if (disposing && Pagebox != null && Pagebox.Image != null)
            {
                Pagebox.Image.Dispose();
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
            this.Sidepanel = new System.Windows.Forms.Panel();
            this.Showingslist = new System.Windows.Forms.ListBox();
            this.Exportbutton = new System.Windows.Forms.Button();
            this.Pagebox = new System.Windows.Forms.PictureBox();
            this.Sidepanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pagebox)).BeginInit();
            this.SuspendLayout();
            // 
            // Sidepanel
            // 
            this.Sidepanel.Controls.Add(this.Showingslist);
            this.Sidepanel.Controls.Add(this.Exportbutton);
            this.Sidepanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.Sidepanel.Location = new System.Drawing.Point(0, 0);
            this.Sidepanel.Name = "Sidepanel";
            this.Sidepanel.Padding = new System.Windows.Forms.Padding(8);
            this.Sidepanel.Size = new System.Drawing.Size(210, 820);
            this.Sidepanel.TabIndex = 0;
            // 
            // Showingslist
            // 
            this.Showingslist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Showingslist.FormattingEnabled = true;
            this.Showingslist.IntegralHeight = false;
            this.Showingslist.Location = new System.Drawing.Point(8, 8);
            this.Showingslist.Name = "Showingslist";
            this.Showingslist.Size = new System.Drawing.Size(194, 774);
            this.Showingslist.TabIndex = 0;
            this.Showingslist.SelectedIndexChanged += new System.EventHandler(this.ShowingslistSelectedIndexChanged);
            // 
            // Exportbutton
            // 
            this.Exportbutton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Exportbutton.Location = new System.Drawing.Point(8, 782);
            this.Exportbutton.Name = "Exportbutton";
            this.Exportbutton.Size = new System.Drawing.Size(194, 30);
            this.Exportbutton.TabIndex = 1;
            this.Exportbutton.Text = "Exporteer PDF";
            this.Exportbutton.UseVisualStyleBackColor = true;
            this.Exportbutton.Click += new System.EventHandler(this.ExportbuttonClick);
            // 
            // Pagebox
            // 
            this.Pagebox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Pagebox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Pagebox.Location = new System.Drawing.Point(210, 0);
            this.Pagebox.Name = "Pagebox";
            this.Pagebox.Size = new System.Drawing.Size(590, 820);
            this.Pagebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Pagebox.TabIndex = 1;
            this.Pagebox.TabStop = false;
            // 
            // PreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 820);
            this.Controls.Add(this.Pagebox);
            this.Controls.Add(this.Sidepanel);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "PreviewForm";
            this.Text = "Voorbeeld";
            this.Sidepanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Pagebox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Sidepanel;
        private System.Windows.Forms.ListBox Showingslist;
        private System.Windows.Forms.Button Exportbutton;
        private System.Windows.Forms.PictureBox Pagebox;
    }
}
