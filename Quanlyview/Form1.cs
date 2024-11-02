using System;
using System.Windows.Forms;

namespace Quanlyview
{
    public partial class Form1 : Form
    {
        public List<TaiKhoan> lstTaiKhoan = new List<TaiKhoan>();

        public Form1()
        {
            InitializeComponent();
            KhoiTaoTaiKhoanMacDinh();  // Gọi hàm khởi tạo tài khoản mặc định
        }

        private void KhoiTaoTaiKhoanMacDinh()
        {
            // Thêm tài khoản mặc định vào danh sách
            lstTaiKhoan.Add(new TaiKhoan { TenTaiKhoan = "admin", MatKhau = "123456" });
            lstTaiKhoan.Add(new TaiKhoan { TenTaiKhoan = "user1", MatKhau = "password1" });
            // Có thể thêm nhiều tài khoản khác nếu cần
        }

        private void btDangNhap_Click(object sender, EventArgs e)
        {
            if (KiemTraDangNhap(tbTaiKhoan.Text, tbMatKhau.Text))
            {
                Quanly f = new Quanly(); // Tạo một đối tượng Quanly
                f.DangXuat += F_DangXuat; // Đăng ký sự kiện Đăng Xuất
                f.Show(); // Hiển thị form Quanly
                this.Hide(); // Ẩn form đăng nhập
                tbTaiKhoan.Text = ""; // Xóa ô nhập tên tài khoản
                tbMatKhau.Text = ""; // Xóa ô nhập mật khẩu
            }
            else
            {
                MessageBox.Show("Sai tên tài khoản hoặc mật khẩu", "Error");
                tbTaiKhoan.Focus(); // Đưa con trỏ chuột về ô nhập tên tài khoản
            }
        }

        private void F_DangXuat(object? sender, EventArgs e)
        {
            // Xử lý khi đăng xuất từ form Quanly
            (sender as Quanly).isThoat = false; // Đặt isThoat thành false
            (sender as Quanly).Close(); // Đóng form Quanly
            this.Show(); // Hiển thị lại form đăng nhập
        }

        private void tbMatKhau_TextChanged(object sender, EventArgs e)
        {
            // Nếu cần, có thể thêm xử lý cho sự kiện thay đổi mật khẩu
        }

        private void tbTaiKhoan_TextChanged(object sender, EventArgs e)
        {
            // Nếu cần, có thể thêm xử lý cho sự kiện thay đổi tên tài khoản
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Nếu cần, có thể thêm xử lý cho sự kiện load form
        }

        private bool KiemTraDangNhap(string tentaikhoan, string matkhau)
        {
            // Kiểm tra xem tài khoản có tồn tại trong danh sách không
            return lstTaiKhoan.Any(tk => tk.TenTaiKhoan == tentaikhoan && tk.MatKhau == matkhau);
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Hiển thị hoặc ẩn mật khẩu khi checkbox được chọn
            tbMatKhau.UseSystemPasswordChar = !checkBox1.Checked;
        }
    }
}
