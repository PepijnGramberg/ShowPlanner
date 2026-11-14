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
            this.Addshow.Location = new System.Drawing.Point(164, 112);
            this.Addshow.Name = "Addshow";
            this.Addshow.Size = new System.Drawing.Size(146, 23);
            this.Addshow.TabIndex = 1;
            this.Addshow.Text = "Maak voorstelling";
            this.Addshow.UseVisualStyleBackColor = true;
            this.Addshow.Click += new System.EventHandler(this.AddshowClick);
            // 
            // Addgroupbutton
            // 
            this.Addgroupbutton.Location = new System.Drawing.Point(567, 112);
            this.Addgroupbutton.Name = "Addgroupbutton";
            this.Addgroupbutton.Size = new System.Drawing.Size(114, 23);
            this.Addgroupbutton.TabIndex = 2;
            this.Addgroupbutton.Text = "Maak groep";
            this.Addgroupbutton.UseVisualStyleBackColor = true;
            this.Addgroupbutton.Click += new System.EventHandler(this.AddgroupClick);
            // 
            // Voorstellingviewer
            // 
            this.Voorstellingviewer.Location = new System.Drawing.Point(12, 112);
            this.Voorstellingviewer.Name = "Voorstellingviewer";
            this.Voorstellingviewer.ShowItemToolTips = true;
            this.Voorstellingviewer.ItemActivate += new System.EventHandler(this.EditshowClick);
            this.Voorstellingviewer.Size = new System.Drawing.Size(146, 351);
            this.Voorstellingviewer.TabIndex = 3;
            this.Voorstellingviewer.TileSize = new System.Drawing.Size(110, 40);
            this.Voorstellingviewer.UseCompatibleStateImageBehavior = false;
            this.Voorstellingviewer.View = System.Windows.Forms.View.Tile;
            // 
            // Groupviewer
            // 
            this.Groupviewer.Location = new System.Drawing.Point(337, 112);
            this.Groupviewer.Name = "Groupviewer";
            this.Groupviewer.ItemActivate += new System.EventHandler(this.EditgroupClick);
            this.Groupviewer.Size = new System.Drawing.Size(225, 351);
            this.Groupviewer.TabIndex = 4;
            this.Groupviewer.TileSize = new System.Drawing.Size(220, 30);
            this.Groupviewer.UseCompatibleStateImageBehavior = false;
            this.Groupviewer.View = System.Windows.Forms.View.Tile;
            // 
            // Seatlabel
            // 
            this.Seatlabel.AutoSize = true;
            this.Seatlabel.Location = new System.Drawing.Point(19, 64);
            this.Seatlabel.Name = "Seatlabel";
            this.Seatlabel.Size = new System.Drawing.Size(63, 13);
            this.Seatlabel.TabIndex = 5;
            this.Seatlabel.Text = "0/0 Stoelen";
            // 
            // Dividebutton
            // 
            this.Dividebutton.Location = new System.Drawing.Point(489, 484);
            this.Dividebutton.Name = "Dividebutton";
            this.Dividebutton.Size = new System.Drawing.Size(110, 23);
            this.Dividebutton.TabIndex = 6;
            this.Dividebutton.Text = "Verdeel groepen";
            this.Dividebutton.UseVisualStyleBackColor = true;
            this.Dividebutton.Click += new System.EventHandler(this.Divideshows);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Voorstelling naam";
            // 
            // voorstellingnaambox
            // 
            this.voorstellingnaambox.Location = new System.Drawing.Point(11, 32);
            this.voorstellingnaambox.Name = "voorstellingnaambox";
            this.voorstellingnaambox.TextChanged += new System.EventHandler(this.ShowNameChanged);
            this.voorstellingnaambox.Size = new System.Drawing.Size(234, 20);
            this.voorstellingnaambox.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Voorstellingen";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(334, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Groepen";
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(606, 483);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 11;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButtonClick);
            // 
            // RemoveGroupbut
            // 
            this.RemoveGroupbut.Location = new System.Drawing.Point(568, 142);
            this.RemoveGroupbut.Name = "RemoveGroupbut";
            this.RemoveGroupbut.Size = new System.Drawing.Size(113, 23);
            this.RemoveGroupbut.TabIndex = 13;
            this.RemoveGroupbut.Text = "Verwijder groep";
            this.RemoveGroupbut.UseVisualStyleBackColor = true;
            this.RemoveGroupbut.Click += new System.EventHandler(this.RemoveGroupbut_Click);
            // 
            // Removeshowbutton
            // 
            this.Removeshowbutton.Location = new System.Drawing.Point(165, 142);
            this.Removeshowbutton.Name = "Removeshowbutton";
            this.Removeshowbutton.Size = new System.Drawing.Size(145, 23);
            this.Removeshowbutton.TabIndex = 14;
            this.Removeshowbutton.Text = "Verwijder voorstelling";
            this.Removeshowbutton.UseVisualStyleBackColor = true;
            this.Removeshowbutton.Click += new System.EventHandler(this.RemoveshowbuttonClick);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(35, 482);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 15;
            this.SaveButton.Text = "Opslaan";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(117, 484);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(75, 23);
            this.LoadButton.TabIndex = 16;
            this.LoadButton.Text = "Laden";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // Editshowbutton
            // 
            this.Editshowbutton.Location = new System.Drawing.Point(165, 172);
            this.Editshowbutton.Name = "Editshowbutton";
            this.Editshowbutton.Size = new System.Drawing.Size(145, 23);
            this.Editshowbutton.TabIndex = 16;
            this.Editshowbutton.Text = "Wijzig voorstelling";
            this.Editshowbutton.UseVisualStyleBackColor = true;
            this.Editshowbutton.Click += new System.EventHandler(this.EditshowClick);
            // 
            // Editgroupbutton
            // 
            this.Editgroupbutton.Location = new System.Drawing.Point(568, 172);
            this.Editgroupbutton.Name = "Editgroupbutton";
            this.Editgroupbutton.Size = new System.Drawing.Size(113, 23);
            this.Editgroupbutton.TabIndex = 17;
            this.Editgroupbutton.Text = "Wijzig groep";
            this.Editgroupbutton.UseVisualStyleBackColor = true;
            this.Editgroupbutton.Click += new System.EventHandler(this.EditgroupClick);
            // 
            // Bulkgroupbutton
            // 
            this.Bulkgroupbutton.Location = new System.Drawing.Point(568, 202);
            this.Bulkgroupbutton.Name = "Bulkgroupbutton";
            this.Bulkgroupbutton.Size = new System.Drawing.Size(113, 23);
            this.Bulkgroupbutton.TabIndex = 18;
            this.Bulkgroupbutton.Text = "Meerdere groepen";
            this.Bulkgroupbutton.UseVisualStyleBackColor = true;
            this.Bulkgroupbutton.Click += new System.EventHandler(this.BulkgroupClick);
            // 
            // Theaterverdeling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 520);
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
            this.MinimumSize = new System.Drawing.Size(700, 550);
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
    }
}

