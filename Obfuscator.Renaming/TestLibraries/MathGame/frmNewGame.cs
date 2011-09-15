#region License & Copyright
/*
 * frmNewGame.cs
 * Copyright (C)2007 John Sedlak (http://jsedlak.org)
 * 
 * Purpose: Allows the user to start a new game by
 * selecting options.
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
    public partial class frmNewGame : Form
    {
        private bool m_spGame = true;

        public frmNewGame()
        {
            InitializeComponent();
        }

        public bool SinglePlayerGame
        {
            get
            {
                return m_spGame;
            }
        }

        public string PlayerOneName
        {
            get
            {
                return "Player One";
            }
        }

        public string PlayerTwoName
        {
            get
            {
                return "Player Two";
            }
        }

        private void btnSingle_Click(object sender, EventArgs e)
        {
            m_spGame = true;

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void btnMulti_Click(object sender, EventArgs e)
        {
            m_spGame = false;

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            //Application.Exit();
            this.Close();
        }
    }
}