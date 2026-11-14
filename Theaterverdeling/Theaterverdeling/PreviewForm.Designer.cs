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
            this.Showingslabel = new System.Windows.Forms.Label();
            this.Showingslist = new System.Windows.Forms.ListBox();
            this.Grouplabel = new System.Windows.Forms.Label();
            this.Grouplist = new System.Windows.Forms.ListBox();
            this.Actionpanel = new System.Windows.Forms.Panel();
            this.Seatslabel = new System.Windows.Forms.Label();
            this.Movelabel = new System.Windows.Forms.Label();
            this.Movechoice = new System.Windows.Forms.ComboBox();
            this.Movebutton = new System.Windows.Forms.Button();
            this.Pinbutton = new System.Windows.Forms.Button();
            this.Messagelabel = new System.Windows.Forms.Label();
            this.Exportbutton = new System.Windows.Forms.Button();
            this.Mappanel = new System.Windows.Forms.Panel();
            this.Seatmap = new SeatMapControl();
            this.Stagelabel = new System.Windows.Forms.Label();
            this.Freelabel = new System.Windows.Forms.Label();
            this.Sidepanel.SuspendLayout();
            this.Actionpanel.SuspendLayout();
            this.Mappanel.SuspendLayout();
            this.SuspendLayout();
            //
            // Sidepanel
            //
            this.Sidepanel.Controls.Add(this.Grouplist);
            this.Sidepanel.Controls.Add(this.Actionpanel);
            this.Sidepanel.Controls.Add(this.Grouplabel);
            this.Sidepanel.Controls.Add(this.Showingslist);
            this.Sidepanel.Controls.Add(this.Showingslabel);
            this.Sidepanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.Sidepanel.Location = new System.Drawing.Point(0, 0);
            this.Sidepanel.Name = "Sidepanel";
            this.Sidepanel.Padding = new System.Windows.Forms.Padding(8);
            this.Sidepanel.Size = new System.Drawing.Size(280, 700);
            this.Sidepanel.TabIndex = 0;
            //
            // Showingslabel
            //
            this.Showingslabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Showingslabel.Location = new System.Drawing.Point(8, 8);
            this.Showingslabel.Name = "Showingslabel";
            this.Showingslabel.Size = new System.Drawing.Size(264, 18);
            this.Showingslabel.TabIndex = 0;
            this.Showingslabel.Text = "Voorstellingen";
            //
            // Showingslist
            //
            this.Showingslist.Dock = System.Windows.Forms.DockStyle.Top;
            this.Showingslist.FormattingEnabled = true;
            this.Showingslist.IntegralHeight = false;
            this.Showingslist.Location = new System.Drawing.Point(8, 26);
            this.Showingslist.Name = "Showingslist";
            this.Showingslist.Size = new System.Drawing.Size(264, 130);
            this.Showingslist.TabIndex = 1;
            this.Showingslist.SelectedIndexChanged += new System.EventHandler(this.ShowingslistSelectedIndexChanged);
            //
            // Grouplabel
            //
            this.Grouplabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Grouplabel.Location = new System.Drawing.Point(8, 156);
            this.Grouplabel.Name = "Grouplabel";
            this.Grouplabel.Size = new System.Drawing.Size(264, 24);
            this.Grouplabel.TabIndex = 2;
            this.Grouplabel.Text = "Groepen in deze voorstelling";
            this.Grouplabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            //
            // Grouplist
            //
            this.Grouplist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grouplist.FormattingEnabled = true;
            this.Grouplist.IntegralHeight = false;
            this.Grouplist.Location = new System.Drawing.Point(8, 180);
            this.Grouplist.Name = "Grouplist";
            this.Grouplist.Size = new System.Drawing.Size(264, 292);
            this.Grouplist.TabIndex = 3;
            this.Grouplist.SelectedIndexChanged += new System.EventHandler(this.GrouplistSelectedIndexChanged);
            //
            // Actionpanel
            //
            this.Actionpanel.Controls.Add(this.Seatslabel);
            this.Actionpanel.Controls.Add(this.Movelabel);
            this.Actionpanel.Controls.Add(this.Movechoice);
            this.Actionpanel.Controls.Add(this.Movebutton);
            this.Actionpanel.Controls.Add(this.Pinbutton);
            this.Actionpanel.Controls.Add(this.Messagelabel);
            this.Actionpanel.Controls.Add(this.Exportbutton);
            this.Actionpanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Actionpanel.Location = new System.Drawing.Point(8, 472);
            this.Actionpanel.Name = "Actionpanel";
            this.Actionpanel.Size = new System.Drawing.Size(264, 220);
            this.Actionpanel.TabIndex = 4;
            //
            // Seatslabel
            //
            this.Seatslabel.Location = new System.Drawing.Point(0, 6);
            this.Seatslabel.Name = "Seatslabel";
            this.Seatslabel.Size = new System.Drawing.Size(264, 44);
            this.Seatslabel.TabIndex = 0;
            //
            // Movelabel
            //
            this.Movelabel.AutoSize = true;
            this.Movelabel.Location = new System.Drawing.Point(0, 56);
            this.Movelabel.Name = "Movelabel";
            this.Movelabel.Size = new System.Drawing.Size(80, 13);
            this.Movelabel.TabIndex = 1;
            this.Movelabel.Text = "Verplaats naar";
            //
            // Movechoice
            //
            this.Movechoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Movechoice.FormattingEnabled = true;
            this.Movechoice.Location = new System.Drawing.Point(0, 73);
            this.Movechoice.Name = "Movechoice";
            this.Movechoice.Size = new System.Drawing.Size(264, 21);
            this.Movechoice.TabIndex = 2;
            //
            // Movebutton
            //
            this.Movebutton.Location = new System.Drawing.Point(0, 100);
            this.Movebutton.Name = "Movebutton";
            this.Movebutton.Size = new System.Drawing.Size(128, 25);
            this.Movebutton.TabIndex = 3;
            this.Movebutton.Text = "Verplaats";
            this.Movebutton.UseVisualStyleBackColor = true;
            this.Movebutton.Click += new System.EventHandler(this.MovebuttonClick);
            //
            // Pinbutton
            //
            this.Pinbutton.Location = new System.Drawing.Point(136, 100);
            this.Pinbutton.Name = "Pinbutton";
            this.Pinbutton.Size = new System.Drawing.Size(128, 25);
            this.Pinbutton.TabIndex = 4;
            this.Pinbutton.Text = "Vastzetten";
            this.Pinbutton.UseVisualStyleBackColor = true;
            this.Pinbutton.Click += new System.EventHandler(this.PinbuttonClick);
            //
            // Messagelabel
            //
            this.Messagelabel.Location = new System.Drawing.Point(0, 132);
            this.Messagelabel.Name = "Messagelabel";
            this.Messagelabel.Size = new System.Drawing.Size(264, 46);
            this.Messagelabel.TabIndex = 5;
            //
            // Exportbutton
            //
            this.Exportbutton.Location = new System.Drawing.Point(0, 186);
            this.Exportbutton.Name = "Exportbutton";
            this.Exportbutton.Size = new System.Drawing.Size(264, 30);
            this.Exportbutton.TabIndex = 6;
            this.Exportbutton.Text = "Exporteer PDF";
            this.Exportbutton.UseVisualStyleBackColor = true;
            this.Exportbutton.Click += new System.EventHandler(this.ExportbuttonClick);
            //
            // Mappanel
            //
            this.Mappanel.Controls.Add(this.Seatmap);
            this.Mappanel.Controls.Add(this.Stagelabel);
            this.Mappanel.Controls.Add(this.Freelabel);
            this.Mappanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Mappanel.Location = new System.Drawing.Point(280, 0);
            this.Mappanel.Name = "Mappanel";
            this.Mappanel.Padding = new System.Windows.Forms.Padding(0, 8, 8, 8);
            this.Mappanel.Size = new System.Drawing.Size(820, 700);
            this.Mappanel.TabIndex = 1;
            //
            // Seatmap
            //
            this.Seatmap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Seatmap.Location = new System.Drawing.Point(0, 32);
            this.Seatmap.Name = "Seatmap";
            this.Seatmap.Size = new System.Drawing.Size(812, 640);
            this.Seatmap.TabIndex = 0;
            this.Seatmap.SeatMapClick += new System.EventHandler<SeatMapClickEventArgs>(this.SeatmapSeatMapClick);
            //
            // Stagelabel
            //
            this.Stagelabel.BackColor = System.Drawing.Color.White;
            this.Stagelabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Stagelabel.Location = new System.Drawing.Point(0, 8);
            this.Stagelabel.Name = "Stagelabel";
            this.Stagelabel.Size = new System.Drawing.Size(812, 24);
            this.Stagelabel.TabIndex = 1;
            this.Stagelabel.Text = "Podium";
            this.Stagelabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // Freelabel
            //
            this.Freelabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Freelabel.Location = new System.Drawing.Point(0, 672);
            this.Freelabel.Name = "Freelabel";
            this.Freelabel.Size = new System.Drawing.Size(812, 20);
            this.Freelabel.TabIndex = 2;
            this.Freelabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // PreviewForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.Mappanel);
            this.Controls.Add(this.Sidepanel);
            this.MinimumSize = new System.Drawing.Size(700, 520);
            this.Name = "PreviewForm";
            this.Text = "Voorbeeld";
            this.Sidepanel.ResumeLayout(false);
            this.Actionpanel.ResumeLayout(false);
            this.Actionpanel.PerformLayout();
            this.Mappanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Sidepanel;
        private System.Windows.Forms.Label Showingslabel;
        private System.Windows.Forms.ListBox Showingslist;
        private System.Windows.Forms.Label Grouplabel;
        private System.Windows.Forms.ListBox Grouplist;
        private System.Windows.Forms.Panel Actionpanel;
        private System.Windows.Forms.Label Seatslabel;
        private System.Windows.Forms.Label Movelabel;
        private System.Windows.Forms.ComboBox Movechoice;
        private System.Windows.Forms.Button Movebutton;
        private System.Windows.Forms.Button Pinbutton;
        private System.Windows.Forms.Label Messagelabel;
        private System.Windows.Forms.Button Exportbutton;
        private System.Windows.Forms.Panel Mappanel;
        private SeatMapControl Seatmap;
        private System.Windows.Forms.Label Stagelabel;
        private System.Windows.Forms.Label Freelabel;
    }
}
