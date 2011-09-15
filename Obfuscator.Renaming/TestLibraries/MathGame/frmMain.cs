#region License & Copyright
/*
 * frmMain.cs
 * Copyright (C)2007 John Sedlak (http://jsedlak.org)
 * 
 * Purpose: frmMain drives the core of the game, contains
 * the playing field and maintains the players.
 */
#endregion

#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MathGame.Directions;
#endregion

namespace MathGame
{
    public partial class frmMain : Form
    {
        #region Members
        #region Menu Item Members
        private bool m_hasSaved = false;
        private string m_savedSource = String.Empty;
        #endregion

        private int m_lastScore = 0, m_lastTurn = 0;
        private int[] m_lastButtons = new int[3];
        private string m_lastEquation = String.Empty;

        private SoundPlayer m_soundPlayer = new SoundPlayer();

        private Random m_random = new Random();         // Random Number Generator
        private Styles m_curStyle = Styles.StartGame;   // Current Style determines how the GUI looks.
        private Pen m_pen;                              // The Pen needed to draw lines.

        private int m_p1BestLeft = 2;                   // Player 1's Best Button Amount
        private int m_p1CanI = 2;                       // P1's Can I? Button Amount

        private int m_p2BestLeft = 2;                   // Player 2's Best Button Amount
        private int m_p2CanI = 2;                       // P2's Can I? Button Amount

        private bool m_spGame = true;                   // True if the game is single player

        private int m_p1Score = 0, m_p2Score = 0;       // P1 and P2's current score.
        private string m_p1Name = "Player One";         // P1's Name
        private string m_p2Name = "Player Two";         // P2's Name

        private Label[] m_labels = new Label[61];       // The Labels for each space on the board.

        private SoundPlayer m_correctAns;
        private SoundPlayer m_wrongAns;
        #endregion

        #region Possible Equations
        // Every possible Equation the User could make...
        string[] m_equations = {"({0}x{1})+{2}", "({0}x{1})-{2}", "({0}x{1})/{2}",
                            "({0}+{1})x{2}", "({0}+{1})-{2}", "({0}+{1})/{2}",
                            "({0}-{1})+{2}", "({0}-{1})x{2}", "({0}-{1})/{2}",
                            "({0}/{1})x{2}", "({0}/{1})+{2}", "({0}/{1})-{2}",
                            "{0}+({1}-{2})", "{0}+({1}x{2})", "{0}+({1}/{2})",
                            "{0}-({1}x{2})", "{0}-({1}/{2})", "{0}-({1}+{2})",
                            "{0}x({1}-{2})", "{0}x({1}+{2})", "{0}x({1}/{2})",
                            "{0}/({1}+{2})", "{0}/({1}-{2})", "{0}/({1}x{2})",
                            "{0}+{1}-{2}", "{0}+{1}/{2}", "{0}+{1}x{2}",
                            "{0}-{1}+{2}", "{0}-{1}x{2}", "{0}-{1}/{2}",
                            "{0}x{1}+{2}", "{0}x{1}/{2}","{0}x{1}-{2}",
                            "{0}/{1}-{2}", "{0}/{1}x{2}", "{0}/{1}+{2}"};
        #endregion

        #region Constructors and New Games
        public frmMain()
        {
            InitializeComponent();

            // Setup the Equation text box.
            txtEquation.BackColor = Color.White;
            txtEquation.ForeColor = Color.Black;
            txtEquation.Font = new Font(txtEquation.Font, FontStyle.Bold);

            // The pen to draw with.
            m_pen = new Pen(Color.Black, 1.5f);

            #region Load Labels
            // Create 61 labels...
            for (int i = 0; i < 61; i++)
            {
                // If it is on an odd numbered line (starting from 0)
                bool _isOdd = ((i / 10) % 2 != 0);

                // Create the label and set the size
                m_labels[i] = new Label();
                m_labels[i].Size = new Size(25, 25);

                // If it is on an odd numbered line...
                if (_isOdd)
                {
                    // Set it from the Right
                    m_labels[i].Left = 600 - ((i % 10) * 60);
                }
                else
                {
                    // If not, set it from the left
                    m_labels[i].Left = 60 + ((i % 10) * 60);
                }

                // Set the Y position of the label
                m_labels[i].Top = 140 + ((i / 10) * 75);

                // Set it's text (a number)
                m_labels[i].Text = String.Format("{0}", i + 1);

                // Set the label's font
                m_labels[i].Font = new Font(this.Font, FontStyle.Bold);

                // If it is a multiple of 10, set the forecolor to red
                if ((i + 1) % 10 == 0)
                    m_labels[i].ForeColor = Color.Red;
                // If it is a special skip space, set the forecolor to blue
                else if (i == 2 || i == 17 || i == 26 || i == 34 || i == 46 || i == 53)
                    m_labels[i].ForeColor = Color.Blue;

                // Add the label to the form
                this.Controls.Add(m_labels[i]);
            }

            // Setup the initial "0" label
            m_labels[60].Left = 20;
            m_labels[60].Top = 140;
            m_labels[60].Text = "0";
            m_labels[60].ForeColor = Color.Red;
            #endregion

            // Set the current style
            SetStyle(Styles.None);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Show();
            btnNextTurn.Enabled = false;
            btnCompare.Enabled = false;

            try
            {
                m_correctAns = new SoundPlayer(@".\sounds\ding.wav");
                m_wrongAns = new SoundPlayer(@".\sounds\error.wav");
            }
            catch
            {
                MessageBox.Show("Could not load the audio files.");
            }

            // Ask to start a new game
            NewGame();
        }

        public void NewGame()
        {
            #region Load A Game
            frmNewGame newGame = new frmNewGame();

            DialogResult _result;
            do
            {
                _result = newGame.ShowDialog();
            } while (_result != DialogResult.OK && _result != DialogResult.Cancel);

            if (_result == DialogResult.Cancel)
                return;

            // Reset Player 1 Stats
            m_p1Score = 0;      // Reset P1 Score
            m_p1CanI = 2;       // Reset P1 Can I Button
            m_p1BestLeft = 2;   // Reset P1 Best Button

            // Reset Player 2 / Computer Stats
            m_p2Score = 0;      // P2 Score
            m_p2CanI = 2;       // P2 Can I Button
            m_p2BestLeft = 2;   // P2 Best Button

            // Set Player One's Name
            m_p1Name = (!String.IsNullOrEmpty(newGame.PlayerOneName)) ? newGame.PlayerOneName : "Player One";

            // Set Player Two's Name
            m_p2Name = (!String.IsNullOrEmpty(newGame.PlayerTwoName)) ? newGame.PlayerTwoName : "Player Two";

            // Set if the game is single player or not.
            m_spGame = newGame.SinglePlayerGame;

            // Set the style.
            SetStyle(Styles.StartGame);

            // Reset the Next Turn Button!
            btnNextTurn.Text = "Start Game";

            // Repaint the form
            this.Invalidate();
            #endregion
        }
        #endregion

        #region Painting
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            #region Draw Box
            g.DrawLine(m_pen, 13, 84, 664, 84);
            g.DrawLine(m_pen, 13, 84, 13, 558);
            g.DrawLine(m_pen, 13, 558, 664, 558);
            g.DrawLine(m_pen, 664, 84, 664, 558);
            #endregion

            #region Draw Short-Cut Lines
            g.DrawLine(m_pen, new Point(185, 140), new Point(190, 200));
            g.DrawLine(m_pen, new Point(435, 296), new Point(375, 351));
            g.DrawLine(m_pen, new Point(430, 456), new Point(430, 496));
            #endregion

            #region Draw Lines
            g.DrawLine(m_pen, new Point(20, 134), new Point(615, 134));
            g.DrawLine(m_pen, new Point(60, 209), new Point(615, 209));
            g.DrawLine(m_pen, new Point(60, 284), new Point(615, 284));
            g.DrawLine(m_pen, new Point(60, 359), new Point(615, 359));
            g.DrawLine(m_pen, new Point(60, 434), new Point(615, 434));
            g.DrawLine(m_pen, new Point(60, 509), new Point(615, 509));
            #endregion

            #region Draw Player Two
            int _left;
            bool _isOdd = (((m_p2Score - 1) / 10) % 2 != 0);

            if (_isOdd)
                _left = 600 - (((m_p2Score - 1) % 10) * 60);
            else
                _left = 60 + (((m_p2Score - 1) % 10) * 60);

            if (m_p2Score == 0)
                _left = 20;

            g.FillEllipse(m_pen.Brush, new Rectangle(_left, 100 + (((m_p2Score - 1) / 10) * 75), 20, 20));
            //g.FillRectangle(m_pen.Brush, new Rectangle(_left, 100 + (((m_p1Score - 1) / 10) * 75), 20, 20));
            #endregion

            #region Draw Player One
            int top = 100 + (((m_p1Score - 1)/ 10) * 75);
            _isOdd = (((m_p1Score - 1) / 10) % 2 != 0);

            if(_isOdd)
                _left = 600 - (((m_p1Score - 1) % 10) * 60);
            else
                _left = 60 + (((m_p1Score - 1) % 10) * 60);

            if (m_p1Score == 0)
                _left = 20;

            Point[] _points = new Point[3];
            _points[0].X = _left + 10; _points[0].Y = top;
            _points[1].X = _left; _points[1].Y = top + 20;
            _points[2].X = _left + 20; _points[2].Y = top + 20;

            g.FillPolygon(m_pen.Brush, _points);
            #endregion
        }
        #endregion

        #region Methods for Taking Turns
        protected bool AddToEquation(string str, bool number)
        {
            bool _endsWithInt = false;
            for (int i = 0; i < 10; i++)
                if (txtEquation.Text.EndsWith(i.ToString()))
                    _endsWithInt = true;

            if (number && m_curStyle != Styles.Solve)
            {
                if (_endsWithInt)
                {
                    MessageBox.Show("You cannot have two digit numbers in your equation.", "Two Digit Numbers",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }
            }
            else if (number && m_curStyle == Styles.Solve)
            {
                txtEquation.Text += str;

                return false;
            }
            else if (txtEquation.Text == "" && (str != "(" && str != ")"))
            {
                MessageBox.Show("You must begin with an equation or a parenthesis!", "Begin an Equation",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
            else if ((!number) && (!_endsWithInt) && !txtEquation.Text.EndsWith(")") && (str != "(" && str != ")"))
            {
                MessageBox.Show("You cannot have two mathematical operations in a row.", "Two Mathematical Operations",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            txtEquation.Text += str;

            return true;
        }

        private void btnNextTurn_Click(object sender, EventArgs e)
        {
            if (btnNextTurn.Text.Equals("Start Game"))
                btnNextTurn.Text = "Next Turn";
            else if (btnNextTurn.Text.Equals("Done"))
            {
                string _equation = "";
                double _correct = 0;
                double _answer = 0;
                try
                {
                    int _index = txtEquation.Text.IndexOf("=");

                    _equation = txtEquation.Text.Substring(0, _index);
                    m_lastEquation = _equation;

                    _correct = EquationSolver.SolvePar(_equation);
                    _answer = Double.Parse(txtEquation.Text.Substring(_index + 1));
                }
                catch
                {
                    MessageBox.Show("Your equation has not been completed correctly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    #region Move to Next Turn
                    SetStyle(Styles.None);
                    btnNextTurn.Text = "Next Turn";

                    if (lblTurn.Text.Equals("Player One:"))
                    {
                        lblTurn.Text = "Player Two:";

                        this.Text = "Math Game: " + m_p2Name + "'s turn!";
                    }
                    else
                    {
                        lblTurn.Text = "Player One:";

                        this.Text = "Math Game: " + m_p1Name + "'s turn!";
                    }

                    NextTurn();
                    #endregion

                    return;
                }

                if (_correct == _answer)
                {
                    if (lblTurn.Text.Equals("Player One:"))
                    {
                        // If the sound is on, play it!
                        #region Play Sound
                        try
                        {
                            if (mnuSound.Checked && m_correctAns != null)
                                m_correctAns.Play();
                        }
                        catch (FileNotFoundException fnf)
                        {
                            MessageBox.Show("The audio file was not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            mnuSound.Checked = false;
                        }
                        catch (Exception ex)
                        {
                            mnuSound.Checked = false;
                        }
                        #endregion

                        MessageBox.Show("Player One was correct and can move " + ((int)_correct).ToString() + " spaces!", "Correct!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (m_p1Score + (int)_answer > 60)
                        {
                            MessageBox.Show("Must land directly on 60 to win!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            m_lastScore = m_p1Score;
                            m_lastTurn = (int)_answer;

                            m_p1Score += (int)_answer;
                        }
                    }
                    else
                    {
                        // If the sound is on, play it!
                        #region Play Sound
                        try
                        {
                            if (mnuSound.Checked && m_correctAns != null)
                                m_correctAns.Play();
                        }
                        catch (FileNotFoundException fnf)
                        {
                            MessageBox.Show("The audio file was not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            mnuSound.Checked = false;
                        }
                        catch (Exception ex)
                        {
                            mnuSound.Checked = false;
                        }
                        #endregion

                        MessageBox.Show("Player Two was correct and can move " + ((int)_correct).ToString() + " spaces!", "Correct!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (m_p2Score + (int)_answer > 60)
                        {
                            MessageBox.Show("Must land directly on 60 to win!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {

                            m_lastScore = m_p2Score;
                            m_lastTurn = (int)_answer;

                            m_p2Score += (int)_answer;
                        }
                    }

                    bool _gameOver = false;
                    CheckScores(out _gameOver);

                    if (_gameOver)
                        return;

                    SetStyle(Styles.None);
                    btnNextTurn.Text = "Next Turn";

                    this.Invalidate();
                }
                else
                {
                    // If the sound is on, play it!
                    #region Play Sound
                    try
                    {
                        if (mnuSound.Checked && m_wrongAns != null)
                            m_wrongAns.Play();
                    }
                    catch (FileNotFoundException fnf)
                    {
                        MessageBox.Show("The audio file was not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        mnuSound.Checked = false;
                    }
                    catch (Exception ex)
                    {

                        mnuSound.Checked = false;
                    }
                    #endregion

                    string _str = String.Format("I am sorry but you are incorrect and lose your turn. The correct answer to {0} was {1}.", _equation, _correct);
                    MessageBox.Show(_str, "Incorrect Answer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    SetStyle(Styles.None);
                    btnNextTurn.Text = "Next Turn";
                }

                if (lblTurn.Text.Equals("Player One:"))
                {
                    lblTurn.Text = "Player Two:";

                    this.Text = "Math Game: " + m_p2Name + "'s turn!";
                }
                else
                {
                    lblTurn.Text = "Player One:";

                    this.Text = "Math Game: " + m_p1Name + "'s turn!";
                }

                NextTurn();
            }
            else
            {
                SetStyle(Styles.NextTurn);
                NextTurn();

                do
                {
                    btnNum0.Text = m_random.Next(1, 3).ToString();
                    btnNum1.Text = m_random.Next(0, 4).ToString();
                    btnNum2.Text = m_random.Next(1, 7).ToString();
                } while (btnNum0.Text.Equals(btnNum1.Text) && btnNum1.Text.Equals(btnNum2.Text));

                m_lastButtons[0] = int.Parse(btnNum0.Text);
                m_lastButtons[1] = int.Parse(btnNum1.Text);
                m_lastButtons[2] = int.Parse(btnNum2.Text);

                // Take Computer Turn
                if (m_spGame && lblTurn.Text.Equals("Player Two:"))
                {
                    int _turn;

                    // Make sure the Computer Player moves.
                    do
                    {
                        _turn = TakeComputerTurn();
                    } while (_turn <= 0);

                    // Add his score up.
                    m_p2Score += _turn;

                    // Display how much he has moved.
                    MessageBox.Show(String.Format("{0} gets to move {1} spaces!", m_p2Name, _turn), "Player Two!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Check Scores
                    bool _gameOver = false;
                    CheckScores(out _gameOver);

                    if (_gameOver)
                        return;

                    // Set the current style...
                    SetStyle(Styles.None);
                    btnNextTurn.Text = "Next Turn";

                    // Set the window and label text
                    if (lblTurn.Text.Equals("Player One:"))
                    {
                        lblTurn.Text = "Player Two:";

                        this.Text = "Math Game: " + m_p2Name + "'s turn!";
                    }
                    else
                    {
                        lblTurn.Text = "Player One:";

                        this.Text = "Math Game: " + m_p1Name + "'s turn!";
                    }

                    // Setup for the next turn
                    NextTurn();

                    // Invalidate the form so it repaints.
                    this.Invalidate();
                }
            }
        }

        private void NextTurn()
        {
            // If it is player one's turn...
            if (lblTurn.Text.Equals("Player One:"))
            {
                // Setup the Can I button
                btnCanI.Text = "Can I? (" + m_p1CanI.ToString() + ")";

                // Disable it if need-be
                if (m_p1CanI == 0)
                    btnCanI.Enabled = false;

                // Set up the Best button
                btnBest.Text = String.Format("Best ({0})", m_p1BestLeft);

                // Disable it if need be
                if (m_p1BestLeft == 0)
                    btnBest.Enabled = false;
            }
            // If it is Player two's turn
            else
            {
                // Setup his Can I button
                btnCanI.Text = "Can I? (" + m_p2CanI.ToString() + ")";

                // Disable it if need-be
                if (m_p2CanI == 0)
                    btnCanI.Enabled = false;

                // Set up his Best button
                btnBest.Text = String.Format("Best ({0})", m_p2BestLeft);

                // Disable it if need-be
                if (m_p2BestLeft == 0)
                    btnBest.Enabled = false;
            }

            btnNextTurn.Enabled = true;
        }

        private int TakeComputerTurn()
        {
            try
            {
                List<int> _possibleValues = new List<int>();

                foreach (string _eq in m_equations)
                    _possibleValues.Add((int)EquationSolver.SolvePar(String.Format(_eq, btnNum0.Text, btnNum1.Text, btnNum2.Text)));

                int _index = m_random.Next(0, _possibleValues.Count - 1);

                return _possibleValues[_index];
            }
            catch
            {
                return 0;
            }
        }

        private void CheckScores(out bool gameOver)
        {
            gameOver = false;
            if (lblTurn.Text.Equals("Player One:"))
            {
                #region P1 Score
                // Check for jump spots, on the tens
                if (m_p1Score == 10 || m_p1Score == 20 || m_p1Score == 30 || m_p1Score == 40 || m_p1Score == 50)
                    m_p1Score += 10;

                // Check for the shortcut routes
                if (m_p1Score == 3)
                    m_p1Score = 18;
                else if (m_p1Score == 27)
                    m_p1Score = 35;
                else if (m_p1Score == 47)
                    m_p1Score = 54;

                // Check if player one has won
                if (m_p1Score == 60)
                {
                    // Make sure the form gets painted.
                    this.Invalidate();

                    // Show that he/she has won
                    MessageBox.Show("You have won the game, congratulations! Use the file menu to start a new game.", "You Win!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Set no style
                    SetStyle(Styles.None);

                    // Disable the rest of the buttons
                    btnNextTurn.Enabled = false;
                    btnCompare.Enabled = false;

                    // Set GameOver to true
                    gameOver = true;

                    // And return
                    return;
                }

                // Check Player 1's score against Player 2
                if (m_p1Score == m_p2Score)
                {
                    m_p2Score = 0;

                    // Show that the players have matched up
                    MessageBox.Show(String.Format("You have landed on the same space as {0}! {0}'s score is now reset to Zero!", m_p2Name), "Player One has landed on Player Two!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Make sure the player is never below 0.
                if (m_p1Score < 0)
                    m_p1Score = 0;
                #endregion
            }
            else
            {
                #region P2 Score
                // Check Player 2 for a jump position, on the tens
                if (m_p2Score == 10 || m_p2Score == 20 || m_p2Score == 30 || m_p2Score == 40 || m_p2Score == 50)
                    m_p2Score += 10;

                // Check Player 2 for the skip positions
                if (m_p2Score == 3)
                    m_p2Score = 18;
                else if (m_p2Score == 27)
                    m_p2Score = 35;
                else if (m_p2Score == 47)
                    m_p2Score = 54;

                // Check if Player 2 has won.
                if (m_p2Score == 60)
                {
                    // Make sure the form gets painted.
                    this.Invalidate();

                    // If it was a two player game,
                    // show the message box for Player 2.
                    // Otherwise show that the player lost.
                    if (!m_spGame)
                        MessageBox.Show("You have won the game, congratulations! Use the file menu to start a new game.", "You Win!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("The computer has won the game! Use the file menu to start a new game.", "You Lose!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Set no style
                    SetStyle(Styles.None);

                    // Disable the buttons
                    btnNextTurn.Enabled = false;
                    btnCompare.Enabled = false;

                    // Make sure the game is over
                    gameOver = true;

                    return;
                }

                // Check if the two players landed on each other
                if (m_p1Score == m_p2Score)
                {
                    // Set player 1's score
                    m_p1Score = 0;

                    // Show that they matched up.
                    MessageBox.Show(String.Format("{0} has landed on the same space as you! Your score is now reset to Zero!", m_p2Name), "Player Two has landed on you!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Make sure that player 2 never draws w/ a negative score
                if (m_p2Score < 0)
                    m_p2Score = 0;
                #endregion
            }
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            double _sol = EquationSolver.SolvePar(txtEquation.Text);

            if (_sol % 1 != 0)
            {
                MessageBox.Show("Your equation results in a fraction. Division is only allowed if the result is an integer such as 1, 2, 3, et cetera.",
                    "Invalid Division", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            if (btnNum0.Enabled || btnNum1.Enabled || btnNum2.Enabled)
            {
                MessageBox.Show("You have not used all the numbers, please go back and use all the numbers by clicking on the buttons to the left.",
                    "Invalid Equation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            if (AddToEquation(btnEquals.Text, false))
            {
                // Set the style to solve mode.
                SetStyle(Styles.Solve);
                btnNextTurn.Text = "Done";
            }
        }
        #endregion

        #region Style Settings
        private void ResetButtonText(int player)
        {
            switch (player)
            {
                case 1:
                    btnCanI.Text = "Can I? (" + m_p1CanI.ToString() + ")";
                    btnBest.Text = String.Format("Best ({0})", m_p1BestLeft);
                    break;
                case 2:
                    btnCanI.Text = "Can I? (" + m_p2CanI.ToString() + ")";
                    btnBest.Text = String.Format("Best ({0})", m_p2BestLeft);
                    break;
            }
        }

        private void SetStyle(Styles style)
        {
            m_curStyle = style;

            switch (style)
            {
                case Styles.None:
                    #region No Style
                    btnNum0.Enabled = false;
                    btnNum0.Visible = false;
                    btnNum1.Enabled = false;
                    btnNum1.Visible = false;
                    btnNum2.Enabled = false;
                    btnNum2.Visible = false;

                    btnNum3.Enabled = false;
                    btnNum3.Visible = false;
                    btnNum4.Enabled = false;
                    btnNum4.Visible = false;
                    btnNum5.Enabled = false;
                    btnNum5.Visible = false;
                    btnNum6.Enabled = false;
                    btnNum6.Visible = false;
                    btnNum7.Enabled = false;
                    btnNum7.Visible = false;
                    btnNum8.Enabled = false;
                    btnNum8.Visible = false;
                    btnNum9.Enabled = false;
                    btnNum9.Visible = false;

                    btnAdd.Enabled = false;
                    btnAdd.Visible = false;
                    btnDivide.Enabled = false;
                    btnDivide.Visible = false;
                    btnMultiply.Enabled = false;
                    btnMultiply.Visible = false;
                    btnEquals.Enabled = false;
                    btnEquals.Visible = false;
                    btnParLeft.Enabled = false;
                    btnParLeft.Visible = false;
                    btnParRight.Enabled = false;
                    btnParRight.Visible = false;

                    btnSubtract.Enabled = false;
                    btnSubtract.Visible = false;

                    btnUndo.Enabled = false;

                    txtEquation.Text = "";

                    btnNum0.Text = "0";
                    btnNum1.Text = "1";
                    btnNum2.Text = "2";

                    btnCanI.Enabled = false;
                    //MessageBox.Show(lblTurn.Text);
                    if(lblTurn.Text == "Player One:" || !m_spGame)
                        btnCompare.Enabled = true;
                    btnBest.Enabled = false;

                    btnNextTurn.Enabled = true;
                    #endregion
                    break;
                case Styles.NextTurn:
                    #region Next Turn Style
                    btnNum0.Enabled = true;
                    btnNum0.Visible = true;
                    btnNum1.Enabled = true;
                    btnNum1.Visible = true;
                    btnNum2.Enabled = true;
                    btnNum2.Visible = true;

                    btnNum3.Enabled = false;
                    btnNum3.Visible = false;
                    btnNum4.Enabled = false;
                    btnNum4.Visible = false;
                    btnNum5.Enabled = false;
                    btnNum5.Visible = false;
                    btnNum6.Enabled = false;
                    btnNum6.Visible = false;
                    btnNum7.Enabled = false;
                    btnNum7.Visible = false;
                    btnNum8.Enabled = false;
                    btnNum8.Visible = false;
                    btnNum9.Enabled = false;
                    btnNum9.Visible = false;

                    btnSubtract.Enabled = true;

                    btnAdd.Enabled = true;
                    btnAdd.Visible = true;
                    btnDivide.Enabled = true;
                    btnDivide.Visible = true;
                    btnMultiply.Enabled = true;
                    btnMultiply.Visible = true;
                    btnEquals.Enabled = true;
                    btnEquals.Visible = true;
                    btnParLeft.Enabled = true;
                    btnParLeft.Visible = true;
                    btnParRight.Enabled = true;
                    btnParRight.Visible = true;
                    btnSubtract.Enabled = true;
                    btnSubtract.Visible = true;

                    btnUndo.Enabled = true;

                    btnCanI.Enabled = true;
                    btnCompare.Enabled = false;
                    btnBest.Enabled = true;

                    txtEquation.Text = "";

                    btnNextTurn.Enabled = false;
                    #endregion
                    break;
                case Styles.Solve:
                    #region Solve Style
                    btnNum0.Text = "0";
                    btnNum1.Text = "1";
                    btnNum2.Text = "2";

                    btnNum0.Enabled = true;
                    btnNum0.Visible = true;
                    btnNum1.Enabled = true;
                    btnNum1.Visible = true;
                    btnNum2.Enabled = true;
                    btnNum2.Visible = true;
                    btnNum3.Enabled = true;
                    btnNum3.Visible = true;
                    btnNum4.Enabled = true;
                    btnNum4.Visible = true;
                    btnNum5.Enabled = true;
                    btnNum5.Visible = true;
                    btnNum6.Enabled = true;
                    btnNum6.Visible = true;
                    btnNum7.Enabled = true;
                    btnNum7.Visible = true;
                    btnNum8.Enabled = true;
                    btnNum8.Visible = true;
                    btnNum9.Enabled = true;
                    btnNum9.Visible = true;

                    btnSubtract.Enabled = true;
                    btnSubtract.Visible = true;

                    btnAdd.Enabled = false;
                    btnAdd.Visible = false;
                    btnDivide.Enabled = false;
                    btnDivide.Visible = false;
                    btnMultiply.Enabled = false;
                    btnMultiply.Visible = false;
                    btnEquals.Enabled = false;
                    btnEquals.Visible = false;
                    btnParLeft.Enabled = false;
                    btnParLeft.Visible = false;
                    btnParRight.Enabled = false;
                    btnParRight.Visible = false;

                    btnUndo.Enabled = true;

                    btnCanI.Enabled = false;
                    btnCompare.Enabled = false;
                    btnBest.Enabled = false;

                    btnNextTurn.Enabled = true;
                    #endregion
                    break;
                case Styles.StartGame:
                    #region Start Game Style
                    btnNum0.Enabled = false;
                    btnNum1.Enabled = false;
                    btnNum2.Enabled = false;
                    btnNum3.Enabled = false;
                    btnNum4.Enabled = false;
                    btnNum5.Enabled = false;
                    btnNum6.Enabled = false;
                    btnNum7.Enabled = false;
                    btnNum8.Enabled = false;
                    btnNum9.Enabled = false;

                    btnSubtract.Enabled = false;
                    btnAdd.Enabled = false;
                    btnDivide.Enabled = false;
                    btnMultiply.Enabled = false;
                    btnEquals.Enabled = false;
                    btnParLeft.Enabled = false;
                    btnParRight.Enabled = false;

                    btnUndo.Enabled = false;

                    btnCanI.Enabled = false;
                    btnCompare.Enabled = false;
                    btnBest.Enabled = false;

                    btnNextTurn.Enabled = true;
                    #endregion
                    break;
            }
        }
        #endregion

        #region Number Button Clicks
        private void btnNum0_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnNum0.Text, true))
                btnNum0.Enabled = false;
        }

        private void btnNum1_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnNum1.Text, true))
                btnNum1.Enabled = false;
        }

        private void btnNum2_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnNum2.Text, true))
                btnNum2.Enabled = false;
        }

        private void btnNum3_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnNum3.Text, true))
                btnNum3.Enabled = false;
        }

        private void btnNum4_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnNum4.Text, true))
                btnNum4.Enabled = false;
        }

        private void btnNum5_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnNum5.Text, true))
                btnNum5.Enabled = false;
        }

        private void btnNum6_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnNum6.Text, true))
                btnNum6.Enabled = false;
        }

        private void btnNum7_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnNum7.Text, true))
                btnNum7.Enabled = false;
        }

        private void btnNum8_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnNum8.Text, true))
                btnNum8.Enabled = false;
        }

        private void btnNum9_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnNum9.Text, true))
                btnNum9.Enabled = false;
        }
        #endregion

        #region Operation Button Clicks
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnAdd.Text, false))
                btnAdd.Enabled = false;
        }

        private void btnSubtract_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnSubtract.Text, false))
                btnSubtract.Enabled = false;
        }

        private void btnMultiply_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnMultiply.Text, false))
                btnMultiply.Enabled = false;
        }

        private void btnDivide_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnDivide.Text, false))
                btnDivide.Enabled = false;
        }

        private void btnParLeft_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnParLeft.Text, false))
                btnParLeft.Enabled = false;
        }

        private void btnParRight_Click(object sender, EventArgs e)
        {
            if (AddToEquation(btnParRight.Text, false))
                btnParRight.Enabled = false;
        }
        #endregion

        #region Menu Operations
        private void mnuNewGame_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void mnuQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mnuAboutMathGame_Click(object sender, EventArgs e)
        {
            frmAbout _about = new frmAbout();

            _about.ShowDialog();
        }

        private void mnuSound_Click(object sender, EventArgs e)
        {
            // Update the Icon in the menu
            if (!mnuSound.Checked)
                this.mnuSound.Image = global::MathGame.Properties.Resources.sound_off;
            else
                this.mnuSound.Image = global::MathGame.Properties.Resources.sound;
        }
        #endregion

        #region Best, Compare and CanI Buttons
        private void btnBest_Click(object sender, EventArgs e)
        {
            double max = 0;
            string maxEq = String.Empty;
            string maxEq2 = String.Empty;
            #region Solve Every Equation
            foreach (string _eq in m_equations)
            {
                //MessageBox.Show("Trying equation (pre): " + _eq);

                string _tempEq = String.Format(_eq, btnNum0.Text, btnNum1.Text, btnNum2.Text);

                //MessageBox.Show("Trying equation (post): " + _tempEq);

                double _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }

                _tempEq = String.Format(_eq, btnNum0.Text, btnNum2.Text, btnNum1.Text);
                _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }

                _tempEq = String.Format(_eq, btnNum1.Text, btnNum0.Text, btnNum2.Text);
                _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }

                _tempEq = String.Format(_eq, btnNum2.Text, btnNum0.Text, btnNum1.Text);
                _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }

                _tempEq = String.Format(_eq, btnNum1.Text, btnNum2.Text, btnNum0.Text);
                _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }

                _tempEq = String.Format(_eq, btnNum2.Text, btnNum1.Text, btnNum0.Text);
                _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }
            }
            #endregion

            MessageBox.Show("Try using the following formula: " + String.Format(maxEq2, "#", "#", "#") + "=" + max.ToString());

            if (lblTurn.Text.Equals("Player One:"))
            {
                m_p1BestLeft--;

                ResetButtonText(1);

                if (m_p1BestLeft == 0)
                    btnBest.Enabled = false;
            }
            else
            {
                m_p2BestLeft--;

                ResetButtonText(2);

                if (m_p2BestLeft == 0)
                    btnBest.Enabled = false;
            }
        }

        private void btnCanI_Click(object sender, EventArgs e)
        {
            frmCanI _frmCan = new frmCanI();

            if (_frmCan.ShowDialog() == DialogResult.OK)
            {
                string maxEq = String.Empty;
                string maxEq2 = String.Empty;
                int _value = _frmCan.Value;
                bool _can = false;

                #region Solve Every Equation
                foreach (string _eq in m_equations)
                {
                    //MessageBox.Show("Trying equation (pre): " + _eq);

                    string _tempEq = String.Format(_eq, btnNum0.Text, btnNum1.Text, btnNum2.Text);

                    //MessageBox.Show("Trying equation (post): " + _tempEq);

                    double _tempMax = EquationSolver.SolvePar(_tempEq);
                    if (_value == _tempMax)
                    {
                        _can = true;
                        maxEq = _tempEq;
                        maxEq2 = _eq;
                    }

                    _tempEq = String.Format(_eq, btnNum0.Text, btnNum2.Text, btnNum1.Text);
                    _tempMax = EquationSolver.SolvePar(_tempEq);
                    if (_value == _tempMax)
                    {
                        _can = true;
                        maxEq = _tempEq;
                        maxEq2 = _eq;
                    }

                    _tempEq = String.Format(_eq, btnNum1.Text, btnNum0.Text, btnNum2.Text);
                    _tempMax = EquationSolver.SolvePar(_tempEq);
                    if (_value == _tempMax)
                    {
                        _can = true;
                        maxEq = _tempEq;
                        maxEq2 = _eq;
                    }

                    _tempEq = String.Format(_eq, btnNum2.Text, btnNum0.Text, btnNum1.Text);
                    _tempMax = EquationSolver.SolvePar(_tempEq);
                    if (_value == _tempMax)
                    {
                        _can = true;
                        maxEq = _tempEq;
                        maxEq2 = _eq;
                    }

                    _tempEq = String.Format(_eq, btnNum1.Text, btnNum2.Text, btnNum0.Text);
                    _tempMax = EquationSolver.SolvePar(_tempEq);
                    if (_value == _tempMax)
                    {
                        _can = true;
                        maxEq = _tempEq;
                        maxEq2 = _eq;
                    }

                    _tempEq = String.Format(_eq, btnNum2.Text, btnNum1.Text, btnNum0.Text);
                    _tempMax = EquationSolver.SolvePar(_tempEq);
                    if (_value == _tempMax)
                    {
                        _can = true;
                        maxEq = _tempEq;
                        maxEq2 = _eq;
                    }
                }
                #endregion

                if (lblTurn.Text.Equals("Player One:"))
                {
                    m_p1CanI--;
                    ResetButtonText(1);

                    if (m_p1CanI == 0)
                        btnCanI.Enabled = false;
                }
                else
                {
                    m_p2CanI--;
                    ResetButtonText(2);

                    if (m_p2CanI == 0)
                        btnCanI.Enabled = false;
                }

                if (_can)
                    MessageBox.Show("Yes, you can! Try using the following formula: " + String.Format(maxEq2, "#", "#", "#"));
                else
                    MessageBox.Show("No, you cannot!");
            }
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            if (m_lastTurn == 0)
                return;

            int _max = 0;
            int _spaces = 0;
            string _eqn = "";
            string _output = "";

            _max = FindMax(out _eqn, out _spaces);

            if (_max <= m_lastScore + m_lastTurn)
                _output = String.Format("I would not have done anything different! Good job!");
            else
                _output = String.Format("I would have used the equation {0} to move {1} spaces resulting in a total score of {2}.", _eqn, _spaces, _max);

            #region Old
            /*int _score = m_lastScore;
            int _toNextSkip = 0, _toNextJump = 0, _toNextBump = 0;

            // Figure out where the next jump is. Jumps = 10, 20, 30, 40, and 50.
            _toNextJump = (((_score % 10) + 1) * 10) - _score;

            // Get the next bump, a bump is done by landing on the next player
            if (lblTurn.Text == "Player One:")
                _toNextBump = m_p2Score - _score;
            else
                _toNextBump = m_p1Score - _score;

            // Get the next skip spot and calc. the distance.
            if (_score < 3)
                _toNextSkip = 3 - _score;
            else if (_score < 27)
                _toNextSkip = 27 - _score;
            else if (_score < 47)
                _toNextSkip = 47 - _score;

            string _output = String.Empty;
            string _equation = String.Empty;
            if (_toNextJump != m_lastTurn && CheckSolution(_toNextJump, out _equation))
            {
                _output = String.Format("I would have gone to the next jump position with this equation: {0}", _equation);
            }
            else if (_toNextBump != m_lastTurn && CheckSolution(_toNextBump, out _equation))
            {
                _output = String.Format("I would have bumped the other player with this equation: {0}", _equation);
            }
            else if (_toNextSkip != m_lastTurn && CheckSolution(_toNextSkip, out _equation))
            {
                _output = String.Format("I would have gone to the next skip position with this equation: {0}", _equation);
            }
            else
            {
                int max = FindMax(out _equation);

                if (max > m_lastTurn)
                    _output = String.Format("I would have used the following equation to get the maximum number of steps: {0}", String.Format("{0}={1}", _equation, max));
                else
                    _output = String.Format("I would not have done anything different! Good job!");
            }*/
            #endregion

            MessageBox.Show(_output, "Compare Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int FindMax(out string eqn, out int spaces)
        {
            double max = 0;
            string maxEq = String.Empty;
            string maxEq2 = String.Empty;

            spaces = 0;

            #region Solve Every Equation
            foreach (string _eq in m_equations)
            {
                //MessageBox.Show("Trying equation (pre): " + _eq);

                string _tempEq = String.Format(_eq, m_lastButtons[0], m_lastButtons[1], m_lastButtons[2]);

                //MessageBox.Show("Trying equation (post): " + _tempEq);

                double _spaces = EquationSolver.SolvePar(_tempEq);
                double _tempMax = ApplyToBoard(m_lastScore, _spaces);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                    spaces = (int)_spaces;
                }

                _tempEq = String.Format(_eq, m_lastButtons[0], m_lastButtons[2], m_lastButtons[1]);
                _spaces = EquationSolver.SolvePar(_tempEq);
                _tempMax = ApplyToBoard(m_lastScore, _spaces);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                    spaces = (int)_spaces;
                }

                _tempEq = String.Format(_eq, m_lastButtons[1], m_lastButtons[0], m_lastButtons[2]);
                _spaces = EquationSolver.SolvePar(_tempEq);
                _tempMax = ApplyToBoard(m_lastScore, _spaces);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                    spaces = (int)_spaces;
                }

                _tempEq = String.Format(_eq, m_lastButtons[2], m_lastButtons[0], m_lastButtons[1]);
                _spaces = EquationSolver.SolvePar(_tempEq);
                _tempMax = ApplyToBoard(m_lastScore, _spaces);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                    spaces = (int)_spaces;
                }

                _tempEq = String.Format(_eq, m_lastButtons[1], m_lastButtons[2], m_lastButtons[0]);
                _spaces = EquationSolver.SolvePar(_tempEq);
                _tempMax = ApplyToBoard(m_lastScore, _spaces);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                    spaces = (int)_spaces;
                }

                _tempEq = String.Format(_eq, m_lastButtons[2], m_lastButtons[1], m_lastButtons[0]);
                _spaces = EquationSolver.SolvePar(_tempEq);
                _tempMax = ApplyToBoard(m_lastScore, _spaces);
                if (_tempMax > max)
                {
                    max = _tempMax;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                    spaces = (int)_spaces;
                }
            }
            #endregion

            eqn = maxEq;

            return (int)max;
        }

        private int ApplyToBoard(int score, double spaces)
        {
            int _total = score + (int)spaces;

            // Check for jump spots, on the tens
            if (_total == 10 || _total == 20 || _total == 30 || _total == 40 || _total == 50)
                _total += 10;

            // Check for the shortcut routes
            if (_total == 3)
                _total = 18;
            else if (m_p1Score == 27)
                _total = 35;
            else if (m_p1Score == 47)
                _total = 54;

            return _total;
        }

        private bool CheckSolution(int solution, out string eqn)
        {
            string maxEq = String.Empty;
            string maxEq2 = String.Empty;
            int _value = solution;
            bool _can = false;

            #region Solve Every Equation
            foreach (string _eq in m_equations)
            {
                //MessageBox.Show("Trying equation (pre): " + _eq);

                string _tempEq = String.Format(_eq, m_lastButtons[0], m_lastButtons[1], m_lastButtons[2]);

                //MessageBox.Show("Trying equation (post): " + _tempEq);

                double _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_value == _tempMax)
                {
                    _can = true;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }

                _tempEq = String.Format(_eq, m_lastButtons[0], m_lastButtons[2], m_lastButtons[1]);
                _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_value == _tempMax)
                {
                    _can = true;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }

                _tempEq = String.Format(_eq, m_lastButtons[1], m_lastButtons[0], m_lastButtons[2]);
                _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_value == _tempMax)
                {
                    _can = true;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }

                _tempEq = String.Format(_eq, m_lastButtons[2], m_lastButtons[0], m_lastButtons[1]);
                _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_value == _tempMax)
                {
                    _can = true;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }

                _tempEq = String.Format(_eq, m_lastButtons[1], m_lastButtons[2], m_lastButtons[0]);
                _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_value == _tempMax)
                {
                    _can = true;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }

                _tempEq = String.Format(_eq, m_lastButtons[2], m_lastButtons[1], m_lastButtons[0]);
                _tempMax = EquationSolver.SolvePar(_tempEq);
                if (_value == _tempMax)
                {
                    _can = true;
                    maxEq = _tempEq;
                    maxEq2 = _eq;
                }
            }
            #endregion

            if (_can)
            {
                eqn = maxEq;

                return true;
            }

            eqn = String.Empty;

            return false;
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtEquation.Text))
                return;

            string _last = txtEquation.Text.Substring(txtEquation.Text.Length - 1);

            if (btnNextTurn.Text == "Done" && _last == "=")
                return;

            #region Check Buttons
            if (btnNum0.Text.Equals(_last))
                btnNum0.Enabled = true;
            else if (btnNum1.Text.Equals(_last))
                btnNum1.Enabled = true;
            else if (btnNum2.Text.Equals(_last))
                btnNum2.Enabled = true;
            else if (btnNum3.Text.Equals(_last))
                btnNum3.Enabled = true;
            else if (btnNum4.Text.Equals(_last))
                btnNum4.Enabled = true;
            else if (btnNum5.Text.Equals(_last))
                btnNum5.Enabled = true;
            else if (btnNum6.Text.Equals(_last))
                btnNum6.Enabled = true;
            else if (btnNum7.Text.Equals(_last))
                btnNum7.Enabled = true;
            else if (btnNum8.Text.Equals(_last))
                btnNum8.Enabled = true;
            else if (btnNum9.Text.Equals(_last))
                btnNum9.Enabled = true;
            else if (btnAdd.Text.Equals(_last))
                btnAdd.Enabled = true;
            else if (btnSubtract.Text.Equals(_last))
                btnSubtract.Enabled = true;
            else if (btnDivide.Text.Equals(_last))
                btnDivide.Enabled = true;
            else if (btnMultiply.Text.Equals(_last))
                btnMultiply.Enabled = true;
            else if (btnEquals.Text.Equals(_last))
                btnEquals.Enabled = true;
            else if (btnParLeft.Text.Equals(_last))
                btnParLeft.Enabled = true;
            else if (btnParRight.Text.Equals(_last))
                btnParRight.Enabled = true;
            #endregion

            txtEquation.Text = txtEquation.Text.Substring(0, txtEquation.Text.Length - 1);
        }
        #endregion

        #region Saving
        private void mnuSave_Click(object sender, EventArgs e)
        {
            if (!m_hasSaved)
            {
                // Open the Save Dialog Window
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Math Game (*.xmg)|*.xmg";

                // If the user wants to save...
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Save the file.
                    m_savedSource = sfd.FileName;
                    Save();
                }
            }
            else
            {
                // Save over the existing file.
                Save();
            }
        }

        private void Save()
        {
            if (String.IsNullOrEmpty(m_savedSource))
                return;
            XmlWriterSettings _settings = new XmlWriterSettings();
            _settings.NewLineHandling = NewLineHandling.Entitize;
            _settings.Indent = true;

            // Open up the XML file
            XmlWriter xw = XmlWriter.Create(m_savedSource, _settings);

            // Start MathGame
            xw.WriteStartElement("MathGame");

            #region Write First Player
            // Write the first player
            xw.WriteStartElement("Player1");

            // Write P1's name
            xw.WriteStartElement("Name");
            xw.WriteString(m_p1Name);
            xw.WriteEndElement();

            // Write P1's score
            xw.WriteStartElement("Score");
            xw.WriteString(m_p1Score.ToString());
            xw.WriteEndElement();

            // Write P1's CanI
            xw.WriteStartElement("CanI");
            xw.WriteString(m_p1CanI.ToString());
            xw.WriteEndElement();

            // Write P1's Best
            xw.WriteStartElement("Best");
            xw.WriteString(m_p1BestLeft.ToString());
            xw.WriteEndElement();

            // End P1
            xw.WriteEndElement();
            #endregion

            #region Write Second Player
            // Write the first player
            xw.WriteStartElement("Player2");

            // Write P1's name
            xw.WriteStartElement("Name");
            xw.WriteString(m_p2Name);
            xw.WriteEndElement();

            // Write P1's score
            xw.WriteStartElement("Score");
            xw.WriteString(m_p2Score.ToString());
            xw.WriteEndElement();

            // Write P1's CanI
            xw.WriteStartElement("CanI");
            xw.WriteString(m_p2CanI.ToString());
            xw.WriteEndElement();

            // Write P1's Best
            xw.WriteStartElement("Best");
            xw.WriteString(m_p2BestLeft.ToString());
            xw.WriteEndElement();

            // End P1
            xw.WriteEndElement();
            #endregion

            // End MathGame
            xw.WriteEndElement();

            xw.Close();
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            // Open the Save Dialog Window
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Math Game (*.xmg)|*.xmg";

            // If the user wants to save...
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Save the file.
                m_savedSource = sfd.FileName;
                Save();
            }
        }
        #endregion

        #region Loading
        private void mnuOpen_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(m_savedSource))
            {
                if (MessageBox.Show("Are you sure you want to open a game? You will lose your current game if you have not saved.",
                                    "Open Saved Game", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Math Game (*.xmg)|*.xmg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                m_savedSource = ofd.FileName;
                LoadGame();
            }
        }

        private void LoadGame()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(m_savedSource);

            XmlNode _curNode = doc.FirstChild;
            while (_curNode != null && !_curNode.Name.Equals("MathGame"))
                _curNode = _curNode.NextSibling;

            foreach (XmlNode _node in _curNode.ChildNodes)
            {
                if (_node.Name.Equals("Player1"))
                    LoadPlayer1(_node);
                else if (_node.Name.Equals("Player2"))
                    LoadPlayer2(_node);
            }

            ResetButtonText(1);
            //ResetButtonText(2);

            this.Invalidate();
        }

        private void LoadPlayer1(XmlNode node)
        {
            foreach (XmlNode _node in node.ChildNodes)
            {
                if (_node.Name.Equals("Name"))
                    m_p1Name = _node.FirstChild.Value.ToString();
                else if (_node.Name.Equals("Score"))
                {
                    int _score = 0;
                    if (int.TryParse(_node.FirstChild.Value, out _score))
                        m_p1Score = _score;
                }
                else if (_node.Name.Equals("CanI"))
                {
                    int _canI = 0;
                    if (int.TryParse(_node.FirstChild.Value, out _canI))
                        m_p1CanI = _canI;
                }
                else if (_node.Name.Equals("Best"))
                {
                    int _best = 0;
                    if (int.TryParse(_node.FirstChild.Value, out _best))
                        m_p1BestLeft = _best;
                }
            }
        }

        private void LoadPlayer2(XmlNode node)
        {
            foreach (XmlNode _node in node.ChildNodes)
            {
                if (_node.Name.Equals("Name"))
                    m_p2Name = _node.FirstChild.Value.ToString();
                else if (_node.Name.Equals("Score"))
                {
                    int _score = 0;
                    if (int.TryParse(_node.FirstChild.Value, out _score))
                        m_p2Score = _score;
                }
                else if (_node.Name.Equals("CanI"))
                {
                    int _canI = 0;
                    if (int.TryParse(_node.FirstChild.Value, out _canI))
                        m_p2CanI = _canI;
                }
                else if (_node.Name.Equals("Best"))
                {
                    int _best = 0;
                    if (int.TryParse(_node.FirstChild.Value, out _best))
                        m_p2BestLeft = _best;
                }
            }
        }
        #endregion

        #region Directions Menu
        private void objectiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmObjective _objective = new frmObjective();
            if (_objective.ShowDialog() == DialogResult.OK)
            {
                int x = _objective.Left, y = _objective.Top;
                _objective = null;

                frmHowToMove _htm = new frmHowToMove();
                _htm.ShowDialog();
                _htm.Left = x;
                _htm.Top = y;
            }
        }

        private void howToMoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmHowToMove _htm = new frmHowToMove();

            _htm.ShowDialog();
        }

        private void mnuOrder_Click(object sender, EventArgs e)
        {
            frmOrder _order = new frmOrder();
            _order.ShowDialog();
        }

        private void mnuSpecial_Click(object sender, EventArgs e)
        {
            frmSpecialMoves _fsm = new frmSpecialMoves();
            _fsm.ShowDialog();
        }

        private void mnuHints_Click(object sender, EventArgs e)
        {
            frmHints _fhints = new frmHints();
            _fhints.ShowDialog();
        }
        #endregion
    }

    public enum Styles
    {
        NextTurn,
        Solve,
        StartGame,
        None
    }
}