namespace Theaterverdeling
{
    partial class EditorForm
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
            this.Toppanel = new System.Windows.Forms.Panel();
            this.Pickerlabel = new System.Windows.Forms.Label();
            this.Hallpicker = new System.Windows.Forms.ComboBox();
            this.Namelabel = new System.Windows.Forms.Label();
            this.Hallname = new System.Windows.Forms.TextBox();
            this.Savebutton = new System.Windows.Forms.Button();
            this.Statuslabel = new System.Windows.Forms.Label();
            this.Leftpanel = new System.Windows.Forms.Panel();
            this.Rowsgrid = new System.Windows.Forms.DataGridView();
            this.Buttonpanel = new System.Windows.Forms.FlowLayoutPanel();
            this.Addrowbutton = new System.Windows.Forms.Button();
            this.Addwalkwaybutton = new System.Windows.Forms.Button();
            this.Removerowbutton = new System.Windows.Forms.Button();
            this.Upbutton = new System.Windows.Forms.Button();
            this.Downbutton = new System.Windows.Forms.Button();
            this.Mappanel = new System.Windows.Forms.Panel();
            this.Seatmap = new SeatMapControl();
            this.Hintlabel = new System.Windows.Forms.Label();
            this.Countlabel = new System.Windows.Forms.Label();
            this.Toppanel.SuspendLayout();
            this.Leftpanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Rowsgrid)).BeginInit();
            this.Buttonpanel.SuspendLayout();
            this.Mappanel.SuspendLayout();
            this.SuspendLayout();
            //
            // Toppanel
            //
            this.Toppanel.Controls.Add(this.Pickerlabel);
            this.Toppanel.Controls.Add(this.Hallpicker);
            this.Toppanel.Controls.Add(this.Namelabel);
            this.Toppanel.Controls.Add(this.Hallname);
            this.Toppanel.Controls.Add(this.Savebutton);
            this.Toppanel.Controls.Add(this.Statuslabel);
            this.Toppanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Toppanel.Location = new System.Drawing.Point(0, 0);
            this.Toppanel.Name = "Toppanel";
            this.Toppanel.Size = new System.Drawing.Size(1100, 42);
            this.Toppanel.TabIndex = 0;
            //
            // Pickerlabel
            //
            this.Pickerlabel.AutoSize = true;
            this.Pickerlabel.Location = new System.Drawing.Point(10, 14);
            this.Pickerlabel.Name = "Pickerlabel";
            this.Pickerlabel.Size = new System.Drawing.Size(40, 13);
            this.Pickerlabel.TabIndex = 0;
            this.Pickerlabel.Text = "Open";
            //
            // Hallpicker
            //
            this.Hallpicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Hallpicker.FormattingEnabled = true;
            this.Hallpicker.Location = new System.Drawing.Point(52, 10);
            this.Hallpicker.Name = "Hallpicker";
            this.Hallpicker.Size = new System.Drawing.Size(170, 21);
            this.Hallpicker.TabIndex = 1;
            this.Hallpicker.SelectedIndexChanged += new System.EventHandler(this.HallpickerSelectedIndexChanged);
            //
            // Namelabel
            //
            this.Namelabel.AutoSize = true;
            this.Namelabel.Location = new System.Drawing.Point(240, 14);
            this.Namelabel.Name = "Namelabel";
            this.Namelabel.Size = new System.Drawing.Size(40, 13);
            this.Namelabel.TabIndex = 2;
            this.Namelabel.Text = "Naam";
            //
            // Hallname
            //
            this.Hallname.Location = new System.Drawing.Point(284, 10);
            this.Hallname.Name = "Hallname";
            this.Hallname.Size = new System.Drawing.Size(170, 20);
            this.Hallname.TabIndex = 3;
            this.Hallname.TextChanged += new System.EventHandler(this.HallnameTextChanged);
            //
            // Savebutton
            //
            this.Savebutton.Location = new System.Drawing.Point(464, 8);
            this.Savebutton.Name = "Savebutton";
            this.Savebutton.Size = new System.Drawing.Size(80, 25);
            this.Savebutton.TabIndex = 4;
            this.Savebutton.Text = "Opslaan";
            this.Savebutton.UseVisualStyleBackColor = true;
            this.Savebutton.Click += new System.EventHandler(this.SavebuttonClick);
            //
            // Statuslabel
            //
            this.Statuslabel.AutoSize = true;
            this.Statuslabel.Location = new System.Drawing.Point(556, 14);
            this.Statuslabel.Name = "Statuslabel";
            this.Statuslabel.Size = new System.Drawing.Size(0, 13);
            this.Statuslabel.TabIndex = 5;
            //
            // Leftpanel
            //
            this.Leftpanel.Controls.Add(this.Rowsgrid);
            this.Leftpanel.Controls.Add(this.Buttonpanel);
            this.Leftpanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.Leftpanel.Location = new System.Drawing.Point(0, 42);
            this.Leftpanel.Name = "Leftpanel";
            this.Leftpanel.Padding = new System.Windows.Forms.Padding(8, 0, 4, 8);
            this.Leftpanel.Size = new System.Drawing.Size(500, 658);
            this.Leftpanel.TabIndex = 1;
            //
            // Rowsgrid
            //
            this.Rowsgrid.AllowUserToAddRows = false;
            this.Rowsgrid.AllowUserToDeleteRows = false;
            this.Rowsgrid.AllowUserToResizeRows = false;
            this.Rowsgrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Rowsgrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Rowsgrid.Location = new System.Drawing.Point(8, 0);
            this.Rowsgrid.MultiSelect = false;
            this.Rowsgrid.Name = "Rowsgrid";
            this.Rowsgrid.RowHeadersWidth = 25;
            this.Rowsgrid.Size = new System.Drawing.Size(488, 610);
            this.Rowsgrid.TabIndex = 0;
            this.Rowsgrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.RowsgridCellValueChanged);
            this.Rowsgrid.CurrentCellChanged += new System.EventHandler(this.RowsgridCurrentCellChanged);
            //
            // Buttonpanel
            //
            this.Buttonpanel.Controls.Add(this.Addrowbutton);
            this.Buttonpanel.Controls.Add(this.Addwalkwaybutton);
            this.Buttonpanel.Controls.Add(this.Removerowbutton);
            this.Buttonpanel.Controls.Add(this.Upbutton);
            this.Buttonpanel.Controls.Add(this.Downbutton);
            this.Buttonpanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Buttonpanel.Location = new System.Drawing.Point(8, 610);
            this.Buttonpanel.Name = "Buttonpanel";
            this.Buttonpanel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.Buttonpanel.Size = new System.Drawing.Size(488, 40);
            this.Buttonpanel.TabIndex = 1;
            //
            // Addrowbutton
            //
            this.Addrowbutton.Name = "Addrowbutton";
            this.Addrowbutton.Size = new System.Drawing.Size(100, 25);
            this.Addrowbutton.TabIndex = 0;
            this.Addrowbutton.Text = "Rij toevoegen";
            this.Addrowbutton.UseVisualStyleBackColor = true;
            this.Addrowbutton.Click += new System.EventHandler(this.AddrowbuttonClick);
            //
            // Addwalkwaybutton
            //
            this.Addwalkwaybutton.Name = "Addwalkwaybutton";
            this.Addwalkwaybutton.Size = new System.Drawing.Size(125, 25);
            this.Addwalkwaybutton.TabIndex = 1;
            this.Addwalkwaybutton.Text = "Looppad toevoegen";
            this.Addwalkwaybutton.UseVisualStyleBackColor = true;
            this.Addwalkwaybutton.Click += new System.EventHandler(this.AddwalkwaybuttonClick);
            //
            // Removerowbutton
            //
            this.Removerowbutton.Name = "Removerowbutton";
            this.Removerowbutton.Size = new System.Drawing.Size(85, 25);
            this.Removerowbutton.TabIndex = 2;
            this.Removerowbutton.Text = "Verwijderen";
            this.Removerowbutton.UseVisualStyleBackColor = true;
            this.Removerowbutton.Click += new System.EventHandler(this.RemoverowbuttonClick);
            //
            // Upbutton
            //
            this.Upbutton.Name = "Upbutton";
            this.Upbutton.Size = new System.Drawing.Size(65, 25);
            this.Upbutton.TabIndex = 3;
            this.Upbutton.Text = "Omhoog";
            this.Upbutton.UseVisualStyleBackColor = true;
            this.Upbutton.Click += new System.EventHandler(this.UpbuttonClick);
            //
            // Downbutton
            //
            this.Downbutton.Name = "Downbutton";
            this.Downbutton.Size = new System.Drawing.Size(65, 25);
            this.Downbutton.TabIndex = 4;
            this.Downbutton.Text = "Omlaag";
            this.Downbutton.UseVisualStyleBackColor = true;
            this.Downbutton.Click += new System.EventHandler(this.DownbuttonClick);
            //
            // Mappanel
            //
            this.Mappanel.Controls.Add(this.Seatmap);
            this.Mappanel.Controls.Add(this.Hintlabel);
            this.Mappanel.Controls.Add(this.Countlabel);
            this.Mappanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Mappanel.Location = new System.Drawing.Point(500, 42);
            this.Mappanel.Name = "Mappanel";
            this.Mappanel.Padding = new System.Windows.Forms.Padding(4, 0, 8, 8);
            this.Mappanel.Size = new System.Drawing.Size(600, 658);
            this.Mappanel.TabIndex = 2;
            //
            // Seatmap
            //
            this.Seatmap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Seatmap.Location = new System.Drawing.Point(4, 34);
            this.Seatmap.Name = "Seatmap";
            this.Seatmap.Size = new System.Drawing.Size(588, 596);
            this.Seatmap.TabIndex = 0;
            this.Seatmap.SeatMapClick += new System.EventHandler<SeatMapClickEventArgs>(this.SeatmapSeatMapClick);
            //
            // Hintlabel
            //
            this.Hintlabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Hintlabel.Location = new System.Drawing.Point(4, 0);
            this.Hintlabel.Name = "Hintlabel";
            this.Hintlabel.Size = new System.Drawing.Size(588, 34);
            this.Hintlabel.TabIndex = 1;
            this.Hintlabel.Text = "Het podium is boven. Klik op een stoel om hem wel of niet te gebruiken; grijze stoelen worden niet gebruikt. Klik naast de stoelen om een rij te kiezen.";
            //
            // Countlabel
            //
            this.Countlabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Countlabel.Location = new System.Drawing.Point(4, 630);
            this.Countlabel.Name = "Countlabel";
            this.Countlabel.Size = new System.Drawing.Size(588, 20);
            this.Countlabel.TabIndex = 2;
            this.Countlabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // EditorForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.Mappanel);
            this.Controls.Add(this.Leftpanel);
            this.Controls.Add(this.Toppanel);
            this.MinimumSize = new System.Drawing.Size(800, 450);
            this.Name = "EditorForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Text = "Zalen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorFormFormClosing);
            this.Toppanel.ResumeLayout(false);
            this.Toppanel.PerformLayout();
            this.Leftpanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Rowsgrid)).EndInit();
            this.Buttonpanel.ResumeLayout(false);
            this.Mappanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Toppanel;
        private System.Windows.Forms.Label Pickerlabel;
        private System.Windows.Forms.ComboBox Hallpicker;
        private System.Windows.Forms.Label Namelabel;
        private System.Windows.Forms.TextBox Hallname;
        private System.Windows.Forms.Button Savebutton;
        private System.Windows.Forms.Label Statuslabel;
        private System.Windows.Forms.Panel Leftpanel;
        private System.Windows.Forms.DataGridView Rowsgrid;
        private System.Windows.Forms.FlowLayoutPanel Buttonpanel;
        private System.Windows.Forms.Button Addrowbutton;
        private System.Windows.Forms.Button Addwalkwaybutton;
        private System.Windows.Forms.Button Removerowbutton;
        private System.Windows.Forms.Button Upbutton;
        private System.Windows.Forms.Button Downbutton;
        private System.Windows.Forms.Panel Mappanel;
        private SeatMapControl Seatmap;
        private System.Windows.Forms.Label Hintlabel;
        private System.Windows.Forms.Label Countlabel;
    }
}
