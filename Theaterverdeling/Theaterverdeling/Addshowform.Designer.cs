namespace Theaterverdeling
{
    partial class Addshowform
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
            this.Datumkeuze = new System.Windows.Forms.DateTimePicker();
            this.Addshow = new System.Windows.Forms.Button();
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.Timelabel = new System.Windows.Forms.Label();
            this.Datumlabel = new System.Windows.Forms.Label();
            this.Time = new System.Windows.Forms.DateTimePicker();
            this.Sectionsoff = new System.Windows.Forms.CheckedListBox();
            this.Sectionslabel = new System.Windows.Forms.Label();
            this.Errorlabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Datumkeuze
            // 
            this.Datumkeuze.Location = new System.Drawing.Point(4, 25);
            this.Datumkeuze.Name = "Datumkeuze";
            this.Datumkeuze.Size = new System.Drawing.Size(200, 20);
            this.Datumkeuze.TabIndex = 4;
            // 
            // Addshow
            // 
            this.Addshow.Location = new System.Drawing.Point(148, 152);
            this.Addshow.Name = "Addshow";
            this.Addshow.Size = new System.Drawing.Size(75, 23);
            this.Addshow.TabIndex = 5;
            this.Addshow.Text = "Toevoegen";
            this.Addshow.UseVisualStyleBackColor = true;
            this.Addshow.Click += new System.EventHandler(this.AddshowClick);
            // 
            // Cancelbutton
            // 
            this.Cancelbutton.Location = new System.Drawing.Point(229, 152);
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.Size = new System.Drawing.Size(75, 23);
            this.Cancelbutton.TabIndex = 6;
            this.Cancelbutton.Text = "Annuleren";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            this.Cancelbutton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // Timelabel
            // 
            this.Timelabel.AutoSize = true;
            this.Timelabel.Location = new System.Drawing.Point(226, 9);
            this.Timelabel.Name = "Timelabel";
            this.Timelabel.Size = new System.Drawing.Size(40, 13);
            this.Timelabel.TabIndex = 7;
            this.Timelabel.Text = "Tijdstip";
            // 
            // Datumlabel
            // 
            this.Datumlabel.AutoSize = true;
            this.Datumlabel.Location = new System.Drawing.Point(12, 9);
            this.Datumlabel.Name = "Datumlabel";
            this.Datumlabel.Size = new System.Drawing.Size(38, 13);
            this.Datumlabel.TabIndex = 9;
            this.Datumlabel.Text = "Datum";
            // 
            // Time
            // 
            this.Time.CustomFormat = "HH:mm";
            this.Time.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Time.Location = new System.Drawing.Point(229, 25);
            this.Time.Name = "Time";
            this.Time.ShowUpDown = true;
            this.Time.Size = new System.Drawing.Size(87, 20);
            this.Time.TabIndex = 10;
            // 
            // Sectionslabel
            // 
            this.Sectionslabel.AutoSize = true;
            this.Sectionslabel.Location = new System.Drawing.Point(12, 56);
            this.Sectionslabel.Name = "Sectionslabel";
            this.Sectionslabel.Size = new System.Drawing.Size(52, 13);
            this.Sectionslabel.TabIndex = 12;
            this.Sectionslabel.Text = "Niet gebruiken in deze voorstelling";
            // 
            // Sectionsoff
            // 
            this.Sectionsoff.CheckOnClick = true;
            this.Sectionsoff.FormattingEnabled = true;
            this.Sectionsoff.Location = new System.Drawing.Point(15, 72);
            this.Sectionsoff.Name = "Sectionsoff";
            this.Sectionsoff.Size = new System.Drawing.Size(189, 49);
            this.Sectionsoff.TabIndex = 11;
            // 
            // Halllabel
            // 
            this.Halllabel = new System.Windows.Forms.Label();
            this.Halllabel.AutoSize = true;
            this.Halllabel.Location = new System.Drawing.Point(226, 56);
            this.Halllabel.Name = "Halllabel";
            this.Halllabel.Size = new System.Drawing.Size(28, 13);
            this.Halllabel.TabIndex = 13;
            this.Halllabel.Text = "Zaal";
            // 
            // Hallchoice
            // 
            this.Hallchoice = new System.Windows.Forms.ComboBox();
            this.Hallchoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Hallchoice.FormattingEnabled = true;
            this.Hallchoice.Location = new System.Drawing.Point(229, 72);
            this.Hallchoice.Name = "Hallchoice";
            this.Hallchoice.Size = new System.Drawing.Size(100, 21);
            this.Hallchoice.TabIndex = 14;
            this.Hallchoice.SelectedIndexChanged += new System.EventHandler(this.HallchoiceSelectedIndexChanged);
            // 
            // Errorlabel
            // 
            this.Errorlabel.AutoSize = true;
            this.Errorlabel.ForeColor = System.Drawing.Color.Firebrick;
            this.Errorlabel.Location = new System.Drawing.Point(12, 130);
            this.Errorlabel.Name = "Errorlabel";
            this.Errorlabel.Size = new System.Drawing.Size(0, 13);
            this.Errorlabel.TabIndex = 20;
            // 
            // Addshowform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 187);
            this.Controls.Add(this.Hallchoice);
            this.Controls.Add(this.Halllabel);
            this.Controls.Add(this.Errorlabel);
            this.Controls.Add(this.Sectionslabel);
            this.Controls.Add(this.Sectionsoff);
            this.Controls.Add(this.Time);
            this.Controls.Add(this.Datumlabel);
            this.Controls.Add(this.Timelabel);
            this.Controls.Add(this.Cancelbutton);
            this.Controls.Add(this.Addshow);
            this.Controls.Add(this.Datumkeuze);
            this.Name = "Addshowform";
            this.Text = "Voorstelling toevoegen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker Datumkeuze;
        private System.Windows.Forms.Button Addshow;
        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.Label Timelabel;
        private System.Windows.Forms.Label Datumlabel;
        private System.Windows.Forms.DateTimePicker Time;
        public Voorstelling show;
        private System.Windows.Forms.CheckedListBox Sectionsoff;
        private System.Windows.Forms.Label Sectionslabel;
        private System.Windows.Forms.Label Errorlabel;
        private System.Windows.Forms.Label Halllabel;
        private System.Windows.Forms.ComboBox Hallchoice;
    }
}