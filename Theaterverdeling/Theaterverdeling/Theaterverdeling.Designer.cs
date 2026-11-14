using System.Drawing;

namespace Theaterverdeling
{
    partial class Theaterverdeling
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
            this.Addshow = new System.Windows.Forms.Button();
            this.Addgroupbutton = new System.Windows.Forms.Button();
            this.Voorstellingviewer = new System.Windows.Forms.ListView();
            this.Groupviewer = new System.Windows.Forms.ListView();
            this.Seatlabel = new System.Windows.Forms.Label();
            this.Dividebutton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.voorstellingnaambox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ResetButton = new System.Windows.Forms.Button();
            this.RemoveGroupbut = new System.Windows.Forms.Button();
            this.Removeshowbutton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.LoadButton = new System.Windows.Forms.Button();
            this.Editshowbutton = new System.Windows.Forms.Button();
            this.Editgroupbutton = new System.Windows.Forms.Button();
            this.Bulkgroupbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Addshow
            // 
            this.Addshow.Location = new System.Drawing.Point(178, 92);
            this.Addshow.Name = "Addshow";
            this.Addshow.Size = new System.Drawing.Size(140, 25);
            this.Addshow.TabIndex = 2;
            this.Addshow.Text = "Voorstelling toevoegen";
            this.Addshow.UseVisualStyleBackColor = true;
            this.Addshow.Click += new System.EventHandler(this.AddshowClick);
            // 
            // Addgroupbutton
            // 
            this.Addgroupbutton.Location = new System.Drawing.Point(616, 92);
            this.Addgroupbutton.Name = "Addgroupbutton";
            this.Addgroupbutton.Size = new System.Drawing.Size(152, 25);
            this.Addgroupbutton.TabIndex = 6;
            this.Addgroupbutton.Text = "Groep toevoegen";
            this.Addgroupbutton.UseVisualStyleBackColor = true;
            this.Addgroupbutton.Click += new System.EventHandler(this.AddgroupClick);
            // 
            // Voorstellingviewer
            // 
            this.Voorstellingviewer.Location = new System.Drawing.Point(12, 92);
            this.Voorstellingviewer.Name = "Voorstellingviewer";
            this.Voorstellingviewer.HideSelection = false;
            this.Voorstellingviewer.ShowItemToolTips = true;
            this.Voorstellingviewer.ItemActivate += new System.EventHandler(this.EditshowClick);
            this.Voorstellingviewer.Size = new System.Drawing.Size(160, 380);
            this.Voorstellingviewer.TabIndex = 1;
            this.Voorstellingviewer.TileSize = new System.Drawing.Size(110, 40);
            this.Voorstellingviewer.UseCompatibleStateImageBehavior = false;
            this.Voorstellingviewer.View = System.Windows.Forms.View.Tile;
            // 
            // Groupviewer
            // 
            this.Groupviewer.Location = new System.Drawing.Point(335, 92);
            this.Groupviewer.Name = "Groupviewer";
            this.Groupviewer.ItemActivate += new System.EventHandler(this.EditgroupClick);
            this.Groupviewer.Size = new System.Drawing.Size(275, 380);
            this.Groupviewer.TabIndex = 5;
            this.Groupviewer.Columns.Add("Naam", 140);
            this.Groupviewer.Columns.Add("Aantal", 48, System.Windows.Forms.HorizontalAlignment.Right);
            this.Groupviewer.Columns.Add("Vooraan", 56);
            this.Groupviewer.Columns.Add("Ochtend", 56);
            this.Groupviewer.FullRowSelect = true;
            this.Groupviewer.HideSelection = false;
            this.Groupviewer.UseCompatibleStateImageBehavior = false;
            this.Groupviewer.View = System.Windows.Forms.View.Details;
            // 
            // Seatlabel
            // 
            this.Seatlabel.AutoSize = true;
            this.Seatlabel.Location = new System.Drawing.Point(420, 70);
            this.Seatlabel.Name = "Seatlabel";
            this.Seatlabel.Size = new System.Drawing.Size(63, 13);
            this.Seatlabel.TabIndex = 5;
            this.Seatlabel.Text = "0 personen, 0 stoelen";
            // 
            // Dividebutton
            // 
            this.Dividebutton.Location = new System.Drawing.Point(12, 524);
            this.Dividebutton.Name = "Dividebutton";
            this.Dividebutton.Size = new System.Drawing.Size(170, 36);
            this.Dividebutton.TabIndex = 10;
            this.Dividebutton.Font = new System.Drawing.Font(System.Windows.Forms.Control.DefaultFont, System.Drawing.FontStyle.Bold);
            this.Dividebutton.Text = "Verdeel groepen";
            this.Dividebutton.UseVisualStyleBackColor = true;
            this.Dividebutton.Click += new System.EventHandler(this.Divideshows);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Titel van de voorstelling";
            // 
            // voorstellingnaambox
            // 
            this.voorstellingnaambox.Location = new System.Drawing.Point(12, 30);
            this.voorstellingnaambox.Name = "voorstellingnaambox";
            this.voorstellingnaambox.TextChanged += new System.EventHandler(this.ShowNameChanged);
            this.voorstellingnaambox.Size = new System.Drawing.Size(300, 20);
            this.voorstellingnaambox.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 9;
            this.label2.Font = new System.Drawing.Font(System.Windows.Forms.Control.DefaultFont, System.Drawing.FontStyle.Bold);
            this.label2.Text = "1. Voorstellingen (datum en tijd)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(335, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 10;
            this.label3.Font = new System.Drawing.Font(System.Windows.Forms.Control.DefaultFont, System.Drawing.FontStyle.Bold);
            this.label3.Text = "2. Groepen";
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(668, 580);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(100, 25);
            this.ResetButton.TabIndex = 14;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButtonClick);
            // 
            // RemoveGroupbut
            // 
            this.RemoveGroupbut.Location = new System.Drawing.Point(616, 185);
            this.RemoveGroupbut.Name = "RemoveGroupbut";
            this.RemoveGroupbut.Size = new System.Drawing.Size(152, 25);
            this.RemoveGroupbut.TabIndex = 9;
            this.RemoveGroupbut.Text = "Verwijder groep";
            this.RemoveGroupbut.UseVisualStyleBackColor = true;
            this.RemoveGroupbut.Click += new System.EventHandler(this.RemoveGroupbut_Click);
            // 
            // Removeshowbutton
            // 
            this.Removeshowbutton.Location = new System.Drawing.Point(178, 154);
            this.Removeshowbutton.Name = "Removeshowbutton";
            this.Removeshowbutton.Size = new System.Drawing.Size(140, 25);
            this.Removeshowbutton.TabIndex = 4;
            this.Removeshowbutton.Text = "Verwijder voorstelling";
            this.Removeshowbutton.UseVisualStyleBackColor = true;
            this.Removeshowbutton.Click += new System.EventHandler(this.RemoveshowbuttonClick);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(12, 580);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(100, 25);
            this.SaveButton.TabIndex = 11;
            this.SaveButton.Text = "Opslaan";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(118, 580);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(100, 25);
            this.LoadButton.TabIndex = 12;
            this.LoadButton.Text = "Laden";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // Editshowbutton
            // 
            this.Editshowbutton.Location = new System.Drawing.Point(178, 123);
            this.Editshowbutton.Name = "Editshowbutton";
            this.Editshowbutton.Size = new System.Drawing.Size(140, 25);
            this.Editshowbutton.TabIndex = 3;
            this.Editshowbutton.Text = "Wijzig voorstelling";
            this.Editshowbutton.UseVisualStyleBackColor = true;
            this.Editshowbutton.Click += new System.EventHandler(this.EditshowClick);
            // 
            // Editgroupbutton
            // 
            this.Editgroupbutton.Location = new System.Drawing.Point(616, 154);
            this.Editgroupbutton.Name = "Editgroupbutton";
            this.Editgroupbutton.Size = new System.Drawing.Size(152, 25);
            this.Editgroupbutton.TabIndex = 8;
            this.Editgroupbutton.Text = "Wijzig groep";
            this.Editgroupbutton.UseVisualStyleBackColor = true;
            this.Editgroupbutton.Click += new System.EventHandler(this.EditgroupClick);
            // 
            // Bulkgroupbutton
            // 
            this.Bulkgroupbutton.Location = new System.Drawing.Point(616, 123);
            this.Bulkgroupbutton.Name = "Bulkgroupbutton";
            this.Bulkgroupbutton.Size = new System.Drawing.Size(152, 25);
            this.Bulkgroupbutton.TabIndex = 7;
            this.Bulkgroupbutton.Text = "Meerdere groepen";
            this.Bulkgroupbutton.UseVisualStyleBackColor = true;
            this.Bulkgroupbutton.Click += new System.EventHandler(this.BulkgroupClick);
            // 
            // Hallsbutton
            // 
            this.Hallsbutton = new System.Windows.Forms.Button();
            this.Hallsbutton.Location = new System.Drawing.Point(224, 580);
            this.Hallsbutton.Name = "Hallsbutton";
            this.Hallsbutton.Size = new System.Drawing.Size(100, 25);
            this.Hallsbutton.TabIndex = 13;
            this.Hallsbutton.Text = "Zalen";
            this.Hallsbutton.UseVisualStyleBackColor = true;
            this.Hallsbutton.Click += new System.EventHandler(this.HallsbuttonClick);
            // 
            // Hintlabel
            // 
            this.Hintlabel = new System.Windows.Forms.Label();
            this.Hintlabel.Location = new System.Drawing.Point(12, 478);
            this.Hintlabel.Name = "Hintlabel";
            this.Hintlabel.Size = new System.Drawing.Size(756, 16);
            this.Hintlabel.TabIndex = 21;
            // 
            // Steplabel3
            // 
            this.Steplabel3 = new System.Windows.Forms.Label();
            this.Steplabel3.AutoSize = true;
            this.Steplabel3.Font = new System.Drawing.Font(System.Windows.Forms.Control.DefaultFont, System.Drawing.FontStyle.Bold);
            this.Steplabel3.Location = new System.Drawing.Point(12, 504);
            this.Steplabel3.Name = "Steplabel3";
            this.Steplabel3.Size = new System.Drawing.Size(70, 13);
            this.Steplabel3.TabIndex = 22;
            this.Steplabel3.Text = "3. Indeling";
            // 
            // Stephint
            // 
            this.Stephint = new System.Windows.Forms.Label();
            this.Stephint.AutoSize = true;
            this.Stephint.Location = new System.Drawing.Point(190, 536);
            this.Stephint.Name = "Stephint";
            this.Stephint.Size = new System.Drawing.Size(300, 13);
            this.Stephint.TabIndex = 23;
            this.Stephint.Text = "Bekijk daarna de indeling, pas aan waar nodig en maak de PDF.";
            // 
            // Theaterverdeling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 617);
            this.Controls.Add(this.Steplabel3);
            this.Controls.Add(this.Stephint);
            this.Controls.Add(this.Hintlabel);
            this.Controls.Add(this.Hallsbutton);
            this.Controls.Add(this.Bulkgroupbutton);
            this.Controls.Add(this.Editgroupbutton);
            this.Controls.Add(this.Editshowbutton);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.Removeshowbutton);
            this.Controls.Add(this.RemoveGroupbut);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.voorstellingnaambox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Dividebutton);
            this.Controls.Add(this.Seatlabel);
            this.Controls.Add(this.Groupviewer);
            this.Controls.Add(this.Voorstellingviewer);
            this.Controls.Add(this.Addgroupbutton);
            this.Controls.Add(this.Addshow);
            this.MinimumSize = new System.Drawing.Size(796, 656);
            this.Name = "Theaterverdeling";
            this.Text = "Groepsindeling";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Addshow;
        private System.Windows.Forms.Button Addgroupbutton;
        private System.Windows.Forms.ListView Voorstellingviewer;
        private System.Windows.Forms.ListView Groupviewer;
        private System.Windows.Forms.Label Seatlabel;
        private System.Windows.Forms.Button Dividebutton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox voorstellingnaambox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Button RemoveGroupbut;
        private System.Windows.Forms.Button Removeshowbutton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.Button Editshowbutton;
        private System.Windows.Forms.Button Editgroupbutton;
        private System.Windows.Forms.Button Bulkgroupbutton;
        private System.Windows.Forms.Button Hallsbutton;
        private System.Windows.Forms.Label Hintlabel;
        private System.Windows.Forms.Label Steplabel3;
        private System.Windows.Forms.Label Stephint;
    }
}

