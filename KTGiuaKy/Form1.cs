using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KTGiuaKy.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.Entity;


namespace KTGiuaKy
{
    public partial class frmSanpham : Form
    {
        private Model1 context;

        public frmSanpham()
        {
            InitializeComponent();
            context = new Model1();
        }
        private void LoadLoaiSPComboBox()
        {
      
            cboLoaiSP.DataSource = context.LoaiSPs.ToList();
            cboLoaiSP.DisplayMember = "TenLoai";  
            cboLoaiSP.ValueMember = "MaLoai";    
        }
        private void LoadData()
        {

            var products = context.Sanphams
                .Include(sp => sp.LoaiSP)
                .ToList(); 

            var formattedProducts = products.Select(sp => new
            {
                sp.MaSP,
                sp.TenSP,
                Ngaynhap = sp.Ngaynhap.HasValue ? sp.Ngaynhap.Value.ToString("dd/MM/yyyy") : "",
                LoaiSP = sp.LoaiSP.TenLoai
            }).ToList();

            dgvSanpham.DataSource = formattedProducts;
        }



        private void btnSua_Click(object sender, EventArgs e)
        {
            var product = context.Sanphams.FirstOrDefault(sp => sp.MaSP == txtMaSP.Text);

            if (product != null)
            {
                product.TenSP = txtTenSP.Text;
                product.Ngaynhap = dtNgayNhap.Value;
                product.MaLoai = cboLoaiSP.SelectedValue.ToString();

                context.SaveChanges();

                LoadData();
            }
            else
            {
                MessageBox.Show("Không tìm thấy sản phẩm để sửa.");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            var product = context.Sanphams.FirstOrDefault(sp => sp.MaSP == txtMaSP.Text);

            if (product != null)
            {
                context.Sanphams.Remove(product);
                context.SaveChanges();

                LoadData();
            }
            else
            {
                MessageBox.Show("Không tìm thấy sản phẩm để xóa.");
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSP.Text) || string.IsNullOrEmpty(txtTenSP.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sản phẩm.");
                return;
            }

            var product = context.Sanphams.FirstOrDefault(sp => sp.MaSP == txtMaSP.Text);

            if (product == null)
            {
                product = new Sanpham
                {
                    MaSP = txtMaSP.Text,
                    TenSP = txtTenSP.Text,
                    Ngaynhap = dtNgayNhap.Value,
                    MaLoai = cboLoaiSP.SelectedValue.ToString()
                };

                context.Sanphams.Add(product);
            }
            else
            {
                product.TenSP = txtTenSP.Text;
                product.Ngaynhap = dtNgayNhap.Value;
                product.MaLoai = cboLoaiSP.SelectedValue.ToString();
            }

            context.SaveChanges();
            LoadData(); 
            MessageBox.Show("Thông tin sản phẩm đã được lưu.");
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSP.Text) || string.IsNullOrEmpty(txtTenSP.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sản phẩm.");
                return;
            }

            var product = new Sanpham
            {
                MaSP = txtMaSP.Text,
                TenSP = txtTenSP.Text,
                Ngaynhap = dtNgayNhap.Value,
                MaLoai = cboLoaiSP.SelectedValue.ToString() 
            };

            context.Sanphams.Add(product);
            context.SaveChanges();

            LoadData();
        }

        private void dgvSanpham_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSanpham.SelectedRows.Count > 0)
            {
                var selectedRow = dgvSanpham.SelectedRows[0];

                if (selectedRow.Cells["MaSP"].Value != null)
                {
                    txtMaSP.Text = selectedRow.Cells["MaSP"].Value.ToString();
                    txtTenSP.Text = selectedRow.Cells["TenSP"].Value.ToString();
                    dtNgayNhap.Value = Convert.ToDateTime(selectedRow.Cells["Ngaynhap"].Value);
                    cboLoaiSP.SelectedValue = selectedRow.Cells["LoaiSP"].Value.ToString();
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim().ToLower();

        
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var filteredProducts = context.Sanphams
                    .Where(sp => sp.TenSP.ToLower().Contains(searchTerm)) 
                    .Include(sp => sp.LoaiSP)
                    .ToList();

                dgvSanpham.DataSource = filteredProducts.Select(sp => new
                {
                    sp.MaSP,
                    sp.TenSP,
                    Ngaynhap = sp.Ngaynhap.HasValue ? sp.Ngaynhap.Value.ToString("dd/MM/yyyy") : "",
                    LoaiSP = sp.LoaiSP.TenLoai
                }).ToList();
            }
            else
            {
                LoadData();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
        }

        private void frmSanpham_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadLoaiSPComboBox();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();  
            }
        }

        private void frmSanpham_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
    }

