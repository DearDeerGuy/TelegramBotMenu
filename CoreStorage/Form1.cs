using CoreTelegramLibrary;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text.Json;
using TelegramLibrary.Models;

namespace CoreStorage
{
    public partial class Form1 : Form
    {
        TeleClass teleClass;
        public Form1()
        {
            InitializeComponent();
        }   
        private async void addButton_Click(object sender, EventArgs e)
        {
            using (StorageContext context = new StorageContext())
            {
                Product product = new Product();
                AddForm addProduct = new AddForm(product);
                if (addProduct.ShowDialog() == DialogResult.OK)
                {
                    var name = context.Products.FirstOrDefault(t => t.Name == product.Name);
                    if (name != null)
                    {
                        MessageBox.Show("The product was already exists");
                    }
                    else
                    {
                        context.Products.Add(product);
                        await context.SaveChangesAsync();
                        teleClass.UpdateProduct();
                    }
                }
            }
        }
        private async void editButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (int.TryParse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                {
                    using (StorageContext context = new StorageContext())
                    {
                        Product? product = await context.Products.FindAsync(id);
                        if (product != null)
                        {
                            AddForm addProduct = new AddForm(product);
                            if (addProduct.ShowDialog() == DialogResult.OK)
                            {
                                await context.SaveChangesAsync();
                                teleClass.UpdateProduct();
                            }
                        }
                    }
                }
            }
        }
        private async void deleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (int.TryParse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                {
                    using (StorageContext context = new StorageContext())
                    {
                        Product? product = await context.Products.FindAsync(id);
                        if (product != null)
                        {
                            var res = MessageBox.Show($"Do you want to delete product: {product.Name}?", "Delete product", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (res == DialogResult.OK)
                            {
                                context.Products.Remove(product);
                                await context.SaveChangesAsync();
                                teleClass.UpdateProduct();
                            }
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            teleClass = new TeleClass(textBox1, dataGridView1);
            teleClass.UpdateProduct();
        }
    }
}