namespace Theaterverdeling
{
    partial class Addgroupform
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
            this.Addgroupbutton = new System.Windows.Forms.Button();
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.Datumvoorkeurenbox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Groepnaam = new System.Windows.Forms.TextBox();
            this.Groepsgrootte = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Firstrowcheck = new System.Windows.Forms.CheckBox();
            this.Morningcheck = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Errorlabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Groepsgrootte)).BeginInit();
            this.SuspendLayout();
            // 
            // Addgroupbutton
            // 
            this.Addgroupbutton.Location = new System.Drawing.Point(285, 185);
            this.Addgroupbutton.Name = "Addgroupbutton";
            this.Addgroupbutton.Size = new System.Drawing.Size(131, 23);
            this.Addgroupbutton.TabIndex = 5;
            this.Addgroupbutton.Text = "Groep toevoegen";
            this.Addgroupbutton.UseVisualStyleBackColor = true;
            this.Addgroupbutton.Click += new System.EventHandler(this.AddgroupbuttonClick);
            // 
            // Cancelbutton
            // 
            this.Cancelbutton.Location = new System.Drawing.Point(422, 185);
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.Size = new System.Drawing.Size(75, 23);
            this.Cancelbutton.TabIndex = 6;
            this.Cancelbutton.Text = "Annuleren";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            this.Cancelbutton.Click += new System.EventHandler(this.CancelbuttonClick);
            // 
            // Datumvoorkeurenbox
            // 
            this.Datumvoorkeurenbox.FormattingEnabled = true;
            this.Datumvoorkeurenbox.Location = new System.Drawing.Point(320, 34);
            this.Datumvoorkeurenbox.Name = "Datumvoorkeurenbox";
            this.Datumvoorkeurenbox.Size = new System.Drawing.Size(175, 94);
            this.Datumvoorkeurenbox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(320, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Voorkeur voor datum";
            // 
            // Groepnaam
            // 
            this.Groepnaam.Location = new System.Drawing.Point(120, 12);
            this.Groepnaam.Name = "Groepnaam";
            this.Groepnaam.Size = new System.Drawing.Size(180, 20);
            this.Groepnaam.TabIndex = 0;
            // 
            // Groepsgrootte
            // 
            this.Groepsgrootte.Location = new System.Drawing.Point(120, 39);
            this.Groepsgrootte.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.Groepsgrootte.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Groepsgrootte.Name = "Groepsgrootte";
            this.Groepsgrootte.Size = new System.Drawing.Size(70, 20);
            this.Groepsgrootte.TabIndex = 1;
            this.Groepsgrootte.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Naam";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Aantal personen";
            // 
            // Firstrowcheck
            // 
            this.Firstrowcheck.AutoSize = true;
            this.Firstrowcheck.Location = new System.Drawing.Point(18, 92);
            this.Firstrowcheck.Name = "Firstrowcheck";
            this.Firstrowcheck.Size = new System.Drawing.Size(66, 17);
            this.Firstrowcheck.TabIndex = 2;
            this.Firstrowcheck.Text = "Wil vooraan zitten";
            this.Firstrowcheck.UseVisualStyleBackColor = true;
            // 
            // Morningcheck
            // 
            this.Morningcheck.AutoSize = true;
            this.Morningcheck.Location = new System.Drawing.Point(18, 116);
            this.Morningcheck.Name = "Morningcheck";
            this.Morningcheck.Size = new System.Drawing.Size(123, 17);
            this.Morningcheck.TabIndex = 3;
            this.Morningcheck.Text = "Alleen een ochtendvoorstelling (voor 12:00)";
            this.Morningcheck.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Wensen";
            // 
            // Errorlabel
            // 
            this.Errorlabel.AutoSize = true;
            this.Errorlabel.ForeColor = System.Drawing.Color.Firebrick;
            this.Errorlabel.Location = new System.Drawing.Point(15, 160);
            this.Errorlabel.Name = "Errorlabel";
            this.Errorlabel.Size = new System.Drawing.Size(0, 13);
            this.Errorlabel.TabIndex = 20;
            // 
            // Datehint
            // 
            this.Datehint = new System.Windows.Forms.Label();
            this.Datehint.AutoSize = true;
            this.Datehint.Location = new System.Drawing.Point(320, 134);
            this.Datehint.Name = "Datehint";
            this.Datehint.Size = new System.Drawing.Size(180, 13);
            this.Datehint.TabIndex = 21;
            this.Datehint.Text = "Niets aangevinkt: elke datum is goed.";
            // 
            // Addgroupform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 222);
            this.Controls.Add(this.Datehint);
            this.Controls.Add(this.Errorlabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Morningcheck);
            this.Controls.Add(this.Firstrowcheck);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Groepsgrootte);
            this.Controls.Add(this.Groepnaam);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Datumvoorkeurenbox);
            this.Controls.Add(this.Cancelbutton);
            this.Controls.Add(this.Addgroupbutton);
            this.AcceptButton = this.Addgroupbutton;
            this.CancelButton = this.Cancelbutton;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Name = "Addgroupform";
            this.Text = "Groep toevoegen";
            ((System.ComponentModel.ISupportInitialize)(this.Groepsgrootte)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Addgroupbutton;
        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.CheckedListBox Datumvoorkeurenbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Groepnaam;
        private System.Windows.Forms.NumericUpDown Groepsgrootte;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox Firstrowcheck;
        private System.Windows.Forms.CheckBox Morningcheck;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label Errorlabel;
        private System.Windows.Forms.Label Datehint;
    }
}