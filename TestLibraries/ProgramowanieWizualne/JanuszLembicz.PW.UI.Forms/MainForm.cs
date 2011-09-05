#region

using System;
using System.Windows.Forms;
using JanuszLembicz.PW.UI.Form.Utils;

#endregion

namespace JanuszLembicz.PW
{
    public partial class MainForm : Form
    {
        private BusinessLogic _businessLogic = new BusinessLogic();

        public MainForm()
        {
            InitializeComponent();
            _interfaceComboBox.Initialize<IOInterface>();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _productDataGridView.DataSource = _businessLogic.GetAllProducts();
            _producerDataGridView.DataSource = _businessLogic.GetAllProducers();
        }


        private ProductFilter GetFilter()
        {
            ProductFilter filter = new ProductFilter
                                       {
                                           ProductName = _productNameTextBox.Text,
                                           ProducerName = _producerNameTextBox.Text,
                                           HasDisplay = _hasDisplayCheckBox.Checked,
                                           Interface = _interfaceComboBox.SelectedValue as IOInterface?
                                       };

            if(_minMemoryCapacityCheckBox.Checked)
            {
                filter.MemoryCapacityFrom = (int?)_minMemoryCapacityNumericUpDown.Value;
            }
            if(_maxMemoryCapacityCheckBox.Checked)
            {
                filter.MemoryCapacityTo = (int?)_maxMemoryCapacityNumericUpDown.Value;
            }
            if(_minWarrantyCheckBox.Checked)
            {
                filter.WarrantyFrom = (int?)_minWarrantyNumericUpDown.Value;
            }
            if(_maxWarrantyCheckBox.Checked)
            {
                filter.WarrantyTo = (int?)_maxWarrantyNumericUpDown.Value;
            }

            return filter;
        }

        private void FilterButtonClick(object sender, EventArgs e)
        {
            _productDataGridView.DataSource = _businessLogic.GetFilteredProducts(GetFilter());
        }

        private void ClearButtonClick(object sender, EventArgs e)
        {
            _productDataGridView.DataSource = _businessLogic.GetAllProducts();
        }

        private void SmallestButtonClick(object sender, EventArgs e)
        {
            _productDataGridView.DataSource = _businessLogic.GetSmallestProducts((int)_smalestNumericUpDown.Value);
        }
    }
}