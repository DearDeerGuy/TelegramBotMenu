using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramLibrary.Models;

namespace CoreStorage
{
    public partial class AddForm : Form
    {
        Product product;
        public AddForm(Product product)
        {
            InitializeComponent();
            this.product = product;
        }
        private void SetInitialValue()
        {
            comboBox1.SelectedValue = product.CategoryId;
            textBoxName.Text = product.Name;
            textBoxUnits.Text = product.Units;
            textBoxQuantity.Text = product.Quantity.ToString();
            textBoxPrice.Text = product.Price.ToString();
            textDescription.Text = product.Description;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                if (int.TryParse(comboBox1.SelectedValue.ToString(), out int id_ctagory))
                {
                    product.CategoryId = id_ctagory;
                }
                product.Name = textBoxName.Text;
                product.Units = textBoxUnits.Text;
                product.Quantity = int.Parse(textBoxQuantity.Text);
                product.Price = double.Parse(textBoxPrice.Text);
                product.Description = textDescription.Text;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void AddForm_Load(object sender, EventArgs e)
        {
            using (StorageContext context = new StorageContext())
            {
                comboBox1.DataSource = null;
                comboBox1.DisplayMember = nameof(Category.CategoryName);
                comboBox1.ValueMember = nameof(Category.Id);
                comboBox1.DataSource = context.Categories.ToList();
            }
            SetInitialValue();
        }
    }
}
