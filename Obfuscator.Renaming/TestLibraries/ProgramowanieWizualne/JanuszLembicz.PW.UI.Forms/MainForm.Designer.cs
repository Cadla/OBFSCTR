namespace JanuszLembicz.PW
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer _components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            this._productDataGridView = new System.Windows.Forms.DataGridView();
            this.producerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.memoryCapacityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.warrantyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hasDisplayDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.interfaceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.productListElementBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._producerDataGridView = new System.Windows.Forms.DataGridView();
            this.producerIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iProducerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._productNameTextBox = new System.Windows.Forms.TextBox();
            this._producerNameTextBox = new System.Windows.Forms.TextBox();
            this._minMemoryCapacityCheckBox = new System.Windows.Forms.CheckBox();
            this._minMemoryCapacityNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._maxMemoryCapacityNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._maxMemoryCapacityCheckBox = new System.Windows.Forms.CheckBox();
            this._maxWarrantyCheckBox = new System.Windows.Forms.CheckBox();
            this._maxWarrantyNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._minWarrantyNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._minWarrantyCheckBox = new System.Windows.Forms.CheckBox();
            this._hasDisplayCheckBox = new System.Windows.Forms.CheckBox();
            this._interfaceComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this._filterButton = new System.Windows.Forms.Button();
            this._clearButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this._smalestNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this._smalestButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._productDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productListElementBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._producerDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iProducerBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._minMemoryCapacityNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._maxMemoryCapacityNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._maxWarrantyNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._minWarrantyNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._smalestNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // _productDataGridView
            // 
            this._productDataGridView.AllowUserToAddRows = false;
            this._productDataGridView.AllowUserToDeleteRows = false;
            this._productDataGridView.AllowUserToResizeColumns = false;
            this._productDataGridView.AllowUserToResizeRows = false;
            this._productDataGridView.AutoGenerateColumns = false;
            this._productDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this._productDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this._productDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._productDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._productDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.producerDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.memoryCapacityDataGridViewTextBoxColumn,
            this.warrantyDataGridViewTextBoxColumn,
            this.hasDisplayDataGridViewCheckBoxColumn,
            this.interfaceDataGridViewTextBoxColumn});
            this._productDataGridView.DataSource = this.productListElementBindingSource;
            this._productDataGridView.Location = new System.Drawing.Point(274, 11);
            this._productDataGridView.Name = "_productDataGridView";
            this._productDataGridView.ReadOnly = true;
            this._productDataGridView.Size = new System.Drawing.Size(515, 314);
            this._productDataGridView.TabIndex = 0;
            // 
            // producerDataGridViewTextBoxColumn
            // 
            this.producerDataGridViewTextBoxColumn.DataPropertyName = "Producer";
            this.producerDataGridViewTextBoxColumn.HeaderText = "Producer";
            this.producerDataGridViewTextBoxColumn.Name = "producerDataGridViewTextBoxColumn";
            this.producerDataGridViewTextBoxColumn.ReadOnly = true;
            this.producerDataGridViewTextBoxColumn.Width = 75;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.Width = 60;
            // 
            // memoryCapacityDataGridViewTextBoxColumn
            // 
            this.memoryCapacityDataGridViewTextBoxColumn.DataPropertyName = "MemoryCapacity";
            this.memoryCapacityDataGridViewTextBoxColumn.HeaderText = "MemoryCapacity";
            this.memoryCapacityDataGridViewTextBoxColumn.Name = "memoryCapacityDataGridViewTextBoxColumn";
            this.memoryCapacityDataGridViewTextBoxColumn.ReadOnly = true;
            this.memoryCapacityDataGridViewTextBoxColumn.Width = 110;
            // 
            // warrantyDataGridViewTextBoxColumn
            // 
            this.warrantyDataGridViewTextBoxColumn.DataPropertyName = "Warranty";
            this.warrantyDataGridViewTextBoxColumn.HeaderText = "Warranty";
            this.warrantyDataGridViewTextBoxColumn.Name = "warrantyDataGridViewTextBoxColumn";
            this.warrantyDataGridViewTextBoxColumn.ReadOnly = true;
            this.warrantyDataGridViewTextBoxColumn.Width = 75;
            // 
            // hasDisplayDataGridViewCheckBoxColumn
            // 
            this.hasDisplayDataGridViewCheckBoxColumn.DataPropertyName = "HasDisplay";
            this.hasDisplayDataGridViewCheckBoxColumn.HeaderText = "HasDisplay";
            this.hasDisplayDataGridViewCheckBoxColumn.Name = "hasDisplayDataGridViewCheckBoxColumn";
            this.hasDisplayDataGridViewCheckBoxColumn.ReadOnly = true;
            this.hasDisplayDataGridViewCheckBoxColumn.Width = 66;
            // 
            // interfaceDataGridViewTextBoxColumn
            // 
            this.interfaceDataGridViewTextBoxColumn.DataPropertyName = "Interface";
            this.interfaceDataGridViewTextBoxColumn.HeaderText = "Interface";
            this.interfaceDataGridViewTextBoxColumn.Name = "interfaceDataGridViewTextBoxColumn";
            this.interfaceDataGridViewTextBoxColumn.ReadOnly = true;
            this.interfaceDataGridViewTextBoxColumn.Width = 74;
            // 
            // productListElementBindingSource
            // 
            this.productListElementBindingSource.DataSource = typeof(JanuszLembicz.PW.ProductListElement);
            // 
            // _producerDataGridView
            // 
            this._producerDataGridView.AllowUserToAddRows = false;
            this._producerDataGridView.AllowUserToDeleteRows = false;
            this._producerDataGridView.AllowUserToResizeColumns = false;
            this._producerDataGridView.AllowUserToResizeRows = false;
            this._producerDataGridView.AutoGenerateColumns = false;
            this._producerDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this._producerDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this._producerDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._producerDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._producerDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.producerIDDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn1});
            this._producerDataGridView.DataSource = this.iProducerBindingSource;
            this._producerDataGridView.Location = new System.Drawing.Point(795, 11);
            this._producerDataGridView.Name = "_producerDataGridView";
            this._producerDataGridView.ReadOnly = true;
            this._producerDataGridView.Size = new System.Drawing.Size(192, 314);
            this._producerDataGridView.TabIndex = 1;
            // 
            // producerIDDataGridViewTextBoxColumn
            // 
            this.producerIDDataGridViewTextBoxColumn.DataPropertyName = "ProducerID";
            this.producerIDDataGridViewTextBoxColumn.HeaderText = "ProducerID";
            this.producerIDDataGridViewTextBoxColumn.Name = "producerIDDataGridViewTextBoxColumn";
            this.producerIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.producerIDDataGridViewTextBoxColumn.Width = 86;
            // 
            // nameDataGridViewTextBoxColumn1
            // 
            this.nameDataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn1.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn1.Name = "nameDataGridViewTextBoxColumn1";
            this.nameDataGridViewTextBoxColumn1.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn1.Width = 60;
            // 
            // iProducerBindingSource
            // 
            this.iProducerBindingSource.DataSource = typeof(JanuszLembicz.PW.BO.IProducer);
            // 
            // _productNameTextBox
            // 
            this._productNameTextBox.Location = new System.Drawing.Point(148, 12);
            this._productNameTextBox.Name = "_productNameTextBox";
            this._productNameTextBox.Size = new System.Drawing.Size(114, 20);
            this._productNameTextBox.TabIndex = 2;
            // 
            // _producerNameTextBox
            // 
            this._producerNameTextBox.Location = new System.Drawing.Point(148, 42);
            this._producerNameTextBox.Name = "_producerNameTextBox";
            this._producerNameTextBox.Size = new System.Drawing.Size(114, 20);
            this._producerNameTextBox.TabIndex = 3;
            // 
            // _minMemoryCapacityCheckBox
            // 
            this._minMemoryCapacityCheckBox.AutoSize = true;
            this._minMemoryCapacityCheckBox.Location = new System.Drawing.Point(147, 80);
            this._minMemoryCapacityCheckBox.Name = "_minMemoryCapacityCheckBox";
            this._minMemoryCapacityCheckBox.Size = new System.Drawing.Size(15, 14);
            this._minMemoryCapacityCheckBox.TabIndex = 4;
            this._minMemoryCapacityCheckBox.UseVisualStyleBackColor = true;
            // 
            // _minMemoryCapacityNumericUpDown
            // 
            this._minMemoryCapacityNumericUpDown.Increment = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this._minMemoryCapacityNumericUpDown.Location = new System.Drawing.Point(210, 74);
            this._minMemoryCapacityNumericUpDown.Maximum = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            this._minMemoryCapacityNumericUpDown.Minimum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this._minMemoryCapacityNumericUpDown.Name = "_minMemoryCapacityNumericUpDown";
            this._minMemoryCapacityNumericUpDown.Size = new System.Drawing.Size(51, 20);
            this._minMemoryCapacityNumericUpDown.TabIndex = 5;
            this._minMemoryCapacityNumericUpDown.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // _maxMemoryCapacityNumericUpDown
            // 
            this._maxMemoryCapacityNumericUpDown.Increment = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this._maxMemoryCapacityNumericUpDown.Location = new System.Drawing.Point(210, 100);
            this._maxMemoryCapacityNumericUpDown.Maximum = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            this._maxMemoryCapacityNumericUpDown.Minimum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this._maxMemoryCapacityNumericUpDown.Name = "_maxMemoryCapacityNumericUpDown";
            this._maxMemoryCapacityNumericUpDown.Size = new System.Drawing.Size(51, 20);
            this._maxMemoryCapacityNumericUpDown.TabIndex = 6;
            this._maxMemoryCapacityNumericUpDown.Value = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            // 
            // _maxMemoryCapacityCheckBox
            // 
            this._maxMemoryCapacityCheckBox.AutoSize = true;
            this._maxMemoryCapacityCheckBox.Location = new System.Drawing.Point(147, 106);
            this._maxMemoryCapacityCheckBox.Name = "_maxMemoryCapacityCheckBox";
            this._maxMemoryCapacityCheckBox.Size = new System.Drawing.Size(15, 14);
            this._maxMemoryCapacityCheckBox.TabIndex = 7;
            this._maxMemoryCapacityCheckBox.UseVisualStyleBackColor = true;
            // 
            // _maxWarrantyCheckBox
            // 
            this._maxWarrantyCheckBox.AutoSize = true;
            this._maxWarrantyCheckBox.Location = new System.Drawing.Point(147, 198);
            this._maxWarrantyCheckBox.Name = "_maxWarrantyCheckBox";
            this._maxWarrantyCheckBox.Size = new System.Drawing.Size(15, 14);
            this._maxWarrantyCheckBox.TabIndex = 11;
            this._maxWarrantyCheckBox.UseVisualStyleBackColor = true;
            // 
            // _maxWarrantyNumericUpDown
            // 
            this._maxWarrantyNumericUpDown.Location = new System.Drawing.Point(211, 192);
            this._maxWarrantyNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this._maxWarrantyNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._maxWarrantyNumericUpDown.Name = "_maxWarrantyNumericUpDown";
            this._maxWarrantyNumericUpDown.Size = new System.Drawing.Size(51, 20);
            this._maxWarrantyNumericUpDown.TabIndex = 10;
            this._maxWarrantyNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // _minWarrantyNumericUpDown
            // 
            this._minWarrantyNumericUpDown.Location = new System.Drawing.Point(211, 166);
            this._minWarrantyNumericUpDown.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this._minWarrantyNumericUpDown.Name = "_minWarrantyNumericUpDown";
            this._minWarrantyNumericUpDown.Size = new System.Drawing.Size(51, 20);
            this._minWarrantyNumericUpDown.TabIndex = 9;
            // 
            // _minWarrantyCheckBox
            // 
            this._minWarrantyCheckBox.AutoSize = true;
            this._minWarrantyCheckBox.Location = new System.Drawing.Point(147, 172);
            this._minWarrantyCheckBox.Name = "_minWarrantyCheckBox";
            this._minWarrantyCheckBox.Size = new System.Drawing.Size(15, 14);
            this._minWarrantyCheckBox.TabIndex = 8;
            this._minWarrantyCheckBox.UseVisualStyleBackColor = true;
            // 
            // _hasDisplayCheckBox
            // 
            this._hasDisplayCheckBox.AutoSize = true;
            this._hasDisplayCheckBox.Checked = true;
            this._hasDisplayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._hasDisplayCheckBox.Location = new System.Drawing.Point(147, 139);
            this._hasDisplayCheckBox.Name = "_hasDisplayCheckBox";
            this._hasDisplayCheckBox.Size = new System.Drawing.Size(15, 14);
            this._hasDisplayCheckBox.TabIndex = 12;
            this._hasDisplayCheckBox.UseVisualStyleBackColor = true;
            // 
            // _interfaceComboBox
            // 
            this._interfaceComboBox.FormattingEnabled = true;
            this._interfaceComboBox.Location = new System.Drawing.Point(147, 224);
            this._interfaceComboBox.Name = "_interfaceComboBox";
            this._interfaceComboBox.Size = new System.Drawing.Size(121, 21);
            this._interfaceComboBox.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Product name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Producer name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Min memory capacity:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Max memory capacity:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Has display:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 173);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Min warranty:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 199);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Max warranty:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 232);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Interface:";
            // 
            // _filterButton
            // 
            this._filterButton.Location = new System.Drawing.Point(112, 260);
            this._filterButton.Name = "_filterButton";
            this._filterButton.Size = new System.Drawing.Size(75, 23);
            this._filterButton.TabIndex = 25;
            this._filterButton.Text = "Filter";
            this._filterButton.UseVisualStyleBackColor = true;
            this._filterButton.Click += new System.EventHandler(this.FilterButtonClick);
            // 
            // _clearButton
            // 
            this._clearButton.Location = new System.Drawing.Point(193, 260);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(75, 23);
            this._clearButton.TabIndex = 26;
            this._clearButton.Text = "Clear";
            this._clearButton.UseVisualStyleBackColor = true;
            this._clearButton.Click += new System.EventHandler(this.ClearButtonClick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 297);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(24, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Get";
            // 
            // _smalestNumericUpDown
            // 
            this._smalestNumericUpDown.Location = new System.Drawing.Point(45, 293);
            this._smalestNumericUpDown.Name = "_smalestNumericUpDown";
            this._smalestNumericUpDown.Size = new System.Drawing.Size(43, 20);
            this._smalestNumericUpDown.TabIndex = 28;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(95, 297);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(132, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "sorted by memory capacity";
            // 
            // _smalestButton
            // 
            this._smalestButton.Location = new System.Drawing.Point(233, 292);
            this._smalestButton.Name = "_smalestButton";
            this._smalestButton.Size = new System.Drawing.Size(35, 23);
            this._smalestButton.TabIndex = 30;
            this._smalestButton.Text = "go";
            this._smalestButton.UseVisualStyleBackColor = true;
            this._smalestButton.Click += new System.EventHandler(this.SmallestButtonClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 334);
            this.Controls.Add(this._smalestButton);
            this.Controls.Add(this.label10);
            this.Controls.Add(this._smalestNumericUpDown);
            this.Controls.Add(this.label9);
            this.Controls.Add(this._clearButton);
            this.Controls.Add(this._filterButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._interfaceComboBox);
            this.Controls.Add(this._hasDisplayCheckBox);
            this.Controls.Add(this._maxWarrantyCheckBox);
            this.Controls.Add(this._maxWarrantyNumericUpDown);
            this.Controls.Add(this._minWarrantyNumericUpDown);
            this.Controls.Add(this._minWarrantyCheckBox);
            this.Controls.Add(this._maxMemoryCapacityCheckBox);
            this.Controls.Add(this._maxMemoryCapacityNumericUpDown);
            this.Controls.Add(this._minMemoryCapacityNumericUpDown);
            this.Controls.Add(this._minMemoryCapacityCheckBox);
            this.Controls.Add(this._producerNameTextBox);
            this.Controls.Add(this._productNameTextBox);
            this.Controls.Add(this._producerDataGridView);
            this.Controls.Add(this._productDataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainForm";
            this.Text = "Przyrost1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._productDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productListElementBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._producerDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iProducerBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._minMemoryCapacityNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._maxMemoryCapacityNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._maxWarrantyNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._minWarrantyNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._smalestNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView _productDataGridView;
        private System.Windows.Forms.DataGridView _producerDataGridView;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.DataGridViewTextBoxColumn producerIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.BindingSource iProducerBindingSource;
        private System.Windows.Forms.TextBox _productNameTextBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn producerDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn memoryCapacityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn warrantyDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn hasDisplayDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn interfaceDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource productListElementBindingSource;
        private System.Windows.Forms.TextBox _producerNameTextBox;
        private System.Windows.Forms.CheckBox _minMemoryCapacityCheckBox;
        private System.Windows.Forms.NumericUpDown _minMemoryCapacityNumericUpDown;
        private System.Windows.Forms.NumericUpDown _maxMemoryCapacityNumericUpDown;
        private System.Windows.Forms.CheckBox _maxMemoryCapacityCheckBox;
        private System.Windows.Forms.CheckBox _maxWarrantyCheckBox;
        private System.Windows.Forms.NumericUpDown _maxWarrantyNumericUpDown;
        private System.Windows.Forms.NumericUpDown _minWarrantyNumericUpDown;
        private System.Windows.Forms.CheckBox _minWarrantyCheckBox;
        private System.Windows.Forms.CheckBox _hasDisplayCheckBox;
        private System.Windows.Forms.ComboBox _interfaceComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button _filterButton;
        private System.Windows.Forms.Button _clearButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown _smalestNumericUpDown;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button _smalestButton;
    }
}

