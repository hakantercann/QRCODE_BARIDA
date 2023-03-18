using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QRCODE_BARIDA.PopUps
{
    public partial class NumPad : Form
    {
        TextBox textBox;
        public NumPad()
        {
            InitializeComponent();
            textBox = new TextBox();
        }
        public void assignToTextBox(TextBox textBox) 
        {
            this.textBox = textBox;
        }
        public string getValue()
        {
            return textBox.Text;
        }


        private void button8_Click(object sender, EventArgs e)
        {
            textBox.Text += "7";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox.Text += "6";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox.Text += "8";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox.Text += "9";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox.Text += "1";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox.Text += "2";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox.Text += "3";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox.Text += "4";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox.Text += "5";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox.Text += "0";
        }
    }
}
