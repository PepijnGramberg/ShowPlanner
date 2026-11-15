namespace Theaterverdeling
{
    partial class Bulkgroupform
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
            this.Groupgrid = new GroupGrid();
            this.Bottompanel = new System.Windows.Forms.Panel();
            this.Toppanel = new System.Windows.Forms.Panel();
            this.Hintlabel = new System.Windows.Forms.Label();
            this.Pastebutton = new System.Windows.Forms.Button();
            this.Addbutton = new System.Windows.Forms.Button();
            this.Closebutton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Groupgrid)).BeginInit();
            this.Bottompanel.SuspendLayout();
            this.SuspendLayout();
            //
            // Groupgrid
            //
            this.Groupgrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Groupgrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Groupgrid.Location = new System.Drawing.Point(0, 0);
            this.Groupgrid.Name = "Groupgrid";
            this.Groupgrid.Size = new System.Drawing.Size(860, 374);
            this.Groupgrid.TabIndex = 0;
            this.Groupgrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GroupgridKeyDown);
            //
            // Toppanel
            //
            this.Toppanel.Controls.Add(this.Hintlabel);
            this.Toppanel.Controls.Add(this.Pastebutton);
            this.Toppanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Toppanel.Location = new System.Drawing.Point(0, 0);
            this.Toppanel.Name = "Toppanel";
            this.Toppanel.Size = new System.Drawing.Size(860, 56);
            this.Toppanel.TabIndex = 2;
            //
            // Bottompanel
            //
            this.Bottompanel.Controls.Add(this.Addbutton);
            this.Bottompanel.Controls.Add(this.Closebutton);
            this.Bottompanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Bottompanel.Location = new System.Drawing.Point(0, 374);
            this.Bottompanel.Name = "Bottompanel";
            this.Bottompanel.Size = new System.Drawing.Size(860, 46);
            this.Bottompanel.TabIndex = 1;
            //
            // Hintlabel
            //
            this.Hintlabel.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.Hintlabel.Location = new System.Drawing.Point(9, 7);
            this.Hintlabel.Name = "Hintlabel";
            this.Hintlabel.Size = new System.Drawing.Size(690, 44);
            this.Hintlabel.TabIndex = 0;
            this.Hintlabel.Text = "Plak hier de groepen uit Excel: selecteer daar de kolommen met de naam en het aantal, kopieer ze, en klik hier op \"Plakken uit Excel\" of druk op Ctrl+V. Typen kan ook. " +
                "Vink per groep de datums aan die de voorkeur hebben; niets aangevinkt betekent dat elke datum goed is.";
            //
            // Pastebutton
            //
            this.Pastebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Pastebutton.Location = new System.Drawing.Point(718, 12);
            this.Pastebutton.Name = "Pastebutton";
            this.Pastebutton.Size = new System.Drawing.Size(130, 28);
            this.Pastebutton.TabIndex = 1;
            this.Pastebutton.Text = "Plakken uit Excel";
            this.Pastebutton.UseVisualStyleBackColor = true;
            this.Pastebutton.Click += new System.EventHandler(this.PastebuttonClick);
            //
            // Addbutton
            //
            this.Addbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Addbutton.Location = new System.Drawing.Point(677, 11);
            this.Addbutton.Name = "Addbutton";
            this.Addbutton.Size = new System.Drawing.Size(85, 24);
            this.Addbutton.TabIndex = 2;
            this.Addbutton.Text = "Toevoegen";
            this.Addbutton.UseVisualStyleBackColor = true;
            this.Addbutton.Click += new System.EventHandler(this.AddbuttonClick);
            //
            // Closebutton
            //
            this.Closebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Closebutton.Location = new System.Drawing.Point(768, 11);
            this.Closebutton.Name = "Closebutton";
            this.Closebutton.Size = new System.Drawing.Size(80, 24);
            this.Closebutton.TabIndex = 3;
            this.Closebutton.Text = "Sluiten";
            this.Closebutton.UseVisualStyleBackColor = true;
            this.Closebutton.Click += new System.EventHandler(this.ClosebuttonClick);
            //
            // Bulkgroupform
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 420);
            this.Controls.Add(this.Groupgrid);
            this.Controls.Add(this.Bottompanel);
            this.Controls.Add(this.Toppanel);
            this.MinimumSize = new System.Drawing.Size(600, 250);
            this.CancelButton = this.Closebutton;
            this.Name = "Bulkgroupform";
            this.Text = "Meerdere groepen toevoegen";
            ((System.ComponentModel.ISupportInitialize)(this.Groupgrid)).EndInit();
            this.Bottompanel.ResumeLayout(false);
            this.Bottompanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupGrid Groupgrid;
        private System.Windows.Forms.Panel Bottompanel;
        private System.Windows.Forms.Panel Toppanel;
        private System.Windows.Forms.Label Hintlabel;
        private System.Windows.Forms.Button Pastebutton;
        private System.Windows.Forms.Button Addbutton;
        private System.Windows.Forms.Button Closebutton;
    }
}
