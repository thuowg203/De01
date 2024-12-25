using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KT_ST4
{
    public partial class Form1 : Form
    {
        Model1 db = new Model1(); 
        private bool isAdding = false;
        private void LoadSinhVien()
        {
            dtgSinhVien.DataSource = (from Sinhvien in db.Sinhviens
                                      join Lop in db.Lops on Sinhvien.MaLop equals Lop.MaLop
                                      select new
                                      {
                                          Sinhvien.MaSV,
                                          Sinhvien.HotenSV,
                                          Sinhvien.Ngaysinh,
                                          TenLop= Lop.TenLop
                                      }).ToList();
           
            dtgSinhVien.Columns["MaSV"].HeaderText = "Mã SV";
            dtgSinhVien.Columns["HotenSV"].HeaderText = "Họ và Tên";
            dtgSinhVien.Columns["Ngaysinh"].HeaderText = "Ngày Sinh";
            dtgSinhVien.Columns["TenLop"].HeaderText = "Lớp";
            dtgSinhVien.AutoGenerateColumns = true;
            dtgSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtgSinhVien.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }
        private void LoadLop()
        {
            var Lop = db.Lops.ToList();
            cboLop.DataSource = Lop;
            cboLop.DisplayMember = "TenLop";
            cboLop.ValueMember = "MaLop";
            cboLop.SelectedIndex = -1;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            isAdding = true;
            txtMaSV.Enabled = true;
            btLuu.Enabled = true;
            btKhong.Enabled = true;
        }

        private void ClearDuLieu()
        {
            txtMaSV.Text = "";
            txtHotenSV.Text = "";
            cboLop.SelectedIndex = -1;
            dtgNgaysinh.Text = "";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSinhVien();
            LoadLop();
            btLuu.Enabled = false;
            btKhong.Enabled = false;
        }

        private void dtgSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgSinhVien.SelectedRows.Count > 0)
            {
                var row = dtgSinhVien.SelectedRows[0];
                txtMaSV.Text = row.Cells[0].Value.ToString();
                txtHotenSV.Text = row.Cells[1].Value.ToString();
                dtgNgaysinh.Text = row.Cells[2].Value.ToString();
                string tenlop = row.Cells[3].Value.ToString();
                var faculty = db.Lops.FirstOrDefault(f => f.TenLop == tenlop);
                if (faculty != null)
                {
                    cboLop.SelectedValue = faculty.MaLop;
                }
                else
                {
                    cboLop.SelectedIndex = -1;
                }
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaSV.Text))
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                

                string maSV = txtMaSV.Text.Trim();

                var student = db.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                if (student == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult result = MessageBox.Show(
            "Bạn có chắc chắn muốn xóa sinh viên này?",
            "Xác nhận xóa",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
                db.Sinhviens.Remove(student);
                db.SaveChanges();

                LoadSinhVien();
                ClearDuLieu();

                MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            isAdding = false;
            txtMaSV.Enabled = false;
            btKhong.Enabled = true;
            btLuu.Enabled = true;
        }

        private void EnableControls(bool enable)
        {
            btLuu.Enabled = enable;
            btKhong.Enabled = enable;
            btThem.Enabled = !enable;
            btSua.Enabled = !enable;
            btXoa.Enabled = !enable;
        }

        private void btLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaSV.Text))
                {
                    MessageBox.Show("Vui lòng nhập mã sinh viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtHotenSV.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên sinh viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboLop.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng chọn lớp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (isAdding)
                {
                    string maSV = txtMaSV.Text.Trim();
                    string hoTenSV = txtHotenSV.Text.Trim();
                    DateTime ngaySinh = dtgNgaysinh.Value;
                    string maLop = cboLop.SelectedValue.ToString();

                    var existingStudent = db.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                    if (existingStudent != null)
                    {
                        MessageBox.Show("Mã sinh viên đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    Sinhvien newStudent = new Sinhvien
                    {
                        MaSV = maSV,
                        HotenSV = hoTenSV,
                        Ngaysinh = ngaySinh,
                        MaLop = maLop
                    };

                    db.Sinhviens.Add(newStudent);
                }
                else
                {
                    string maSV = txtMaSV.Text.Trim();
                    var student = db.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                    if (student == null)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    student.HotenSV = txtHotenSV.Text.Trim();
                    student.Ngaysinh = dtgNgaysinh.Value;
                    student.MaLop = cboLop.SelectedValue.ToString();
                }

                db.SaveChanges();
                LoadSinhVien();
                ClearDuLieu();
                EnableControls(false);
                txtMaSV.Enabled = true;
                MessageBox.Show(isAdding ? "Thêm sinh viên thành công!" : "Sửa thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu thông tin sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btKhong_Click(object sender, EventArgs e)
        {
            ClearDuLieu();
            EnableControls(false);
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Bạn có chắc chắn muốn thoát chương trình?",
        "Xác nhận thoát",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btTim_Click(object sender, EventArgs e)
        {
            try
            {
                string searchKeyword = txtTim.Text.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(searchKeyword))
                {
                    MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var filteredList = from Sinhvien in db.Sinhviens
                                   join Lop in db.Lops on Sinhvien.MaLop equals Lop.MaLop
                                   where Sinhvien.HotenSV.ToLower().Contains(searchKeyword) 
                                   select new
                                   {

                                       Sinhvien.MaSV,
                                       Sinhvien.HotenSV,
                                       Sinhvien.Ngaysinh,
                                       TenLop = Lop.TenLop
                                   };

                dtgSinhVien.DataSource = filteredList.ToList();
                btHuy.Visible = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btHuy_Click(object sender, EventArgs e)
        {
            try
            {
                txtTim.Text = "";
                LoadSinhVien();
                btHuy.Visible = false;
                btTim.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hủy tìm kiếm người dùng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
