using System;
using System.Windows.Forms;

namespace Theaterverdeling
{
    public partial class Addshowform : Form
    {
        public delegate void AddingshowEventHandler(object sender, EventArgs e);
        public Addshowform()
        {
            InitializeComponent();
        }

        private void AddshowClick(object sender, EventArgs e)
        {

                var time = Time.Value;
                var zaal = new Lampegiet(false,Balconyoff.Checked);
                show = new Voorstelling(time, zaal, Datumkeuze.Value.Date);
                Addingshow(this, new EventArgs());
                Close();
        }
        public event AddingshowEventHandler Addingshow;

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }


    }
}
