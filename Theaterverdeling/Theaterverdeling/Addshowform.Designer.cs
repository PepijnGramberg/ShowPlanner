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
            this.Balconyoff = new System.Windows.Forms.CheckBox();
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
            this.Addshow.Location = new System.Drawing.Point(148, 101);
            this.Addshow.Name = "Addshow";
            this.Addshow.Size = new System.Drawing.Size(75, 23);
            this.Addshow.TabIndex = 5;
            this.Addshow.Text = "Toevoegen";
            this.Addshow.UseVisualStyleBackColor = true;
            this.Addshow.Click += new System.EventHandler(this.AddshowClick);
            // 
            // Cancelbutton
            // 
            this.Cancelbutton.Location = new System.Drawing.Point(229, 101);
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
            this.Time.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.Time.Location = new System.Drawing.Point(229, 25);
            this.Time.Name = "Time";
            this.Time.ShowUpDown = true;
            this.Time.Size = new System.Drawing.Size(87, 20);
            this.Time.TabIndex = 10;
            // 
            // Balconyoff
            // 
            this.Balconyoff.AutoSize = true;
            this.Balconyoff.Location = new System.Drawing.Point(15, 62);
            this.Balconyoff.Name = "Balconyoff";
            this.Balconyoff.Size = new System.Drawing.Size(104, 17);
            this.Balconyoff.TabIndex = 11;
            this.Balconyoff.Text = "Balkon Uitzetten";
            this.Balconyoff.UseVisualStyleBackColor = true;
            // 
            // Addshowform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 136);
            this.Controls.Add(this.Balconyoff);
            this.Controls.Add(this.Time);
            this.Controls.Add(this.Datumlabel);
            this.Controls.Add(this.Timelabel);
            this.Controls.Add(this.Cancelbutton);
            this.Controls.Add(this.Addshow);
            this.Controls.Add(this.Datumkeuze);
            this.Name = "Addshowform";
            this.Text = "Voorstelling Toevoegen";
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
        private System.Windows.Forms.CheckBox Balconyoff;
    }
}