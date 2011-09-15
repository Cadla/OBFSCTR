#region License & Copyright
/*
 * frmCanI.cs
 * Copyright (C)2007 John Sedlak (http://jsedlak.org)
 * 
 * Purpose: Obtains integer input from the user.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MathGame
{
    public partial class frmCanI : Form
    {
        protected int m_intValue;

        public frmCanI()
        {
            InitializeComponent();

            txtNumber.Focus();
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtNumber.Text, out m_intValue))
            {
                this.DialogResult = DialogResult.OK;

                this.Close();
            }
            else
                MessageBox.Show("Input is not an integer.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }

        public int Value
        {
            get
            {
                return m_intValue;
            }
        }
    }
}