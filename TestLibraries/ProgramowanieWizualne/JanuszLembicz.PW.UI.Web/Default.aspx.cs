#region

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace JanuszLembicz.PW.UI.Web
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e) {}

        protected void ProductsObjectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["filter"] = GetFilter();
        }

        private ProductFilter GetFilter()
        {
            ProductFilter filter = new ProductFilter
                                       {ProductName = ProductNameTextBox.Text, ProducerName = ProducerNameTextBox.Text,};

            if(MinMemoryCapacityCheckBox.Checked)
            {
                MinMemoryCapacityTextBox.Text = MinMemoryCapacityTextBox.Text == string.Empty
                                                    ? "0" : MinMemoryCapacityTextBox.Text;
                filter.MemoryCapacityFrom = Int32.Parse(MinMemoryCapacityTextBox.Text);
            }
            if(MaxMemoryCapacityCheckBox.Checked)
            {
                MaxMemoryCapacityTextBox.Text = MaxMemoryCapacityTextBox.Text == string.Empty
                                                    ? "0" : MaxMemoryCapacityTextBox.Text;
                filter.MemoryCapacityTo = Int32.Parse(MaxMemoryCapacityTextBox.Text);
            }

            return filter;
        }

        protected void FilterButton_Click(object sender, EventArgs e)
        {
            ProductsGridView.DataBind();
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            ProductNameTextBox.Text = string.Empty;
            ProducerNameTextBox.Text = string.Empty;
            MinMemoryCapacityTextBox.Text = string.Empty;
            MaxMemoryCapacityTextBox.Text = string.Empty;
            MinMemoryCapacityCheckBox.Checked = false;
            MaxMemoryCapacityCheckBox.Checked = false;
            ProductsGridView.DataBind();
        }

        protected void SmalestProductsDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            SmalestTextBox.Text = SmalestTextBox.Text == string.Empty ? "0" : SmalestTextBox.Text;
            e.InputParameters["n"] = Int32.Parse(SmalestTextBox.Text);
        }

        protected void SmallestButton_Click(object sender, EventArgs e)
        {
            SmalestProductsGridView.DataBind();
        }
    }
}