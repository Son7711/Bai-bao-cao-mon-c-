using Quanlyview;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace Quanlyview
{
    public partial class Quanly : Form
    {
        private string strCon = @"Data Source=SONDAICA\SQLEXPRESS;Initial Catalog=quanli;User ID=sa;Password=123456;Encrypt=False;";
        private SqlConnection sqlCon; // Khai báo SqlConnection

        public List<Employee> lstEmp = new List<Employee>();
        private BindingSource bs = new BindingSource();
        public bool isThoat = true;
        public event EventHandler DangXuat;

        private string employeeImagePath = string.Empty; // Store the image path

        public Quanly()
        {
            InitializeComponent();
            SetupImageList();

            //ngày sinh
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd MMMM yyyy";
            dateTimePicker1.ShowUpDown = true; // Hiển thị theo dạng lên/xuống
        }

        private void Quanly_Load(object sender, EventArgs e)
        {
            lstEmp = GetData();
            bs.DataSource = lstEmp; // Khởi tạo DataSource ở đây
            dgvEmployee.DataSource = bs; // Đặt DataSource cho DataGridView
            SetupDataGridView();
            dgvEmployee.CellBeginEdit += dgvEmployee_CellBeginEdit; // Đăng ký sự kiện

            dateTimePicker1.Value = DateTime.Now; // Set the default date to now

            cbSortOptions.Items.Add("Sắp xếp theo tên");
            cbSortOptions.Items.Add("Sắp xếp theo ID");
            cbSortOptions.SelectedIndex = 0; // Mặc định chọn "Sắp xếp theo tên"
        }



        public List<Employee> GetData()
        {
            List<Employee> employee = new List<Employee>();

            using (sqlCon = new SqlConnection(strCon)) // Sử dụng từ khóa using để quản lý tài nguyên
            {
                sqlCon.Open(); // Mở kết nối

                // Câu truy vấn để lấy dữ liệu
                string query = "SELECT Id, Name, BirthDate, Gender, Address, SDT, Email, Maduan, Maphongban, ImagePath FROM Quanlinv";

                using (SqlCommand cmd = new SqlCommand(query, sqlCon)) // Tạo SqlCommand
                {
                    using (SqlDataReader reader = cmd.ExecuteReader()) // Sử dụng using cho SqlDataReader
                    {
                        while (reader.Read()) // Đọc dữ liệu
                        {
                            Employee emp = new Employee
                            {
                                Id = reader.GetInt32(0), // ID
                                Name = reader.GetString(1), // Tên
                                BirthDate = reader.GetDateTime(2), // Ngày sinh
                                Gender = reader.GetBoolean(3), // Giới tính
                                Address = reader.GetString(4), // Địa chỉ
                                SDT = reader.GetString(5),
                                Email = reader.GetString(6),
                                Maduan = reader.GetString(7),
                                Maphongban = reader.GetString(8),
                                ImagePath = reader.IsDBNull(9) ? null : reader.GetString(9) // Ảnh
                            };
                            employee.Add(emp); // Thêm vào danh sách
                        }
                    }
                }
            }
            return employee; // Trả về danh sách nhân viên
        }

        private void AddEmployee(Employee newEmp)
        {
            using (sqlCon = new SqlConnection(strCon))
            {
                sqlCon.Open();
                string query = "INSERT INTO Quanlinv (Id, Name, BirthDate, Gender, Address, SDT, Email, Maduan, Maphongban, ImagePath) VALUES (@Id, @Name, @BirthDate, @Gender, @Address, @SDT, @Email, @Maduan, @Maphongban, @ImagePath)";

                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@Id", newEmp.Id);
                    cmd.Parameters.AddWithValue("@Name", newEmp.Name);
                    cmd.Parameters.AddWithValue("@BirthDate", newEmp.BirthDate);
                    cmd.Parameters.AddWithValue("@Gender", newEmp.Gender);
                    cmd.Parameters.AddWithValue("@Address", newEmp.Address);
                    cmd.Parameters.AddWithValue("@SDT", newEmp.SDT);
                    cmd.Parameters.AddWithValue("@Email", newEmp.Email);
                    cmd.Parameters.AddWithValue("@Maduan", newEmp.Maduan);
                    cmd.Parameters.AddWithValue("@Maphongban", newEmp.Maphongban);
                    cmd.Parameters.AddWithValue("@ImagePath", newEmp.ImagePath);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void UpdateEmployee(Employee emp)
        {
            using (sqlCon = new SqlConnection(strCon))
            {
                sqlCon.Open();
                string query = "UPDATE Quanlinv SET Name=@Name, BirthDate=@BirthDate, Gender=@Gender, Address=@Address, SDT=@SDT, Email=@Email, Maduan=@Maduan, Maphongban=@Maphongban, ImagePath=@ImagePath WHERE Id=@Id";

                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@Id", emp.Id);
                    cmd.Parameters.AddWithValue("@Name", emp.Name);
                    cmd.Parameters.AddWithValue("@BirthDate", emp.BirthDate);
                    cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                    cmd.Parameters.AddWithValue("@Address", emp.Address);
                    cmd.Parameters.AddWithValue("@SDT", emp.SDT);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@Maduan", emp.Maduan);
                    cmd.Parameters.AddWithValue("@Maphongban", emp.Maphongban);
                    cmd.Parameters.AddWithValue("@ImagePath", emp.ImagePath);


                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void DeleteEmployee(int empId)
        {
            using (sqlCon = new SqlConnection(strCon))
            {
                sqlCon.Open();
                string query = "DELETE FROM Quanlinv WHERE Id=@Id";

                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@Id", empId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SetupDataGridView()
        {
            dgvEmployee.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEmployee.Columns[0].HeaderText = "Mã";
            dgvEmployee.Columns[1].HeaderText = "Tên";
            dgvEmployee.Columns[2].HeaderText = "Ngày Sinh";
            dgvEmployee.Columns[3].HeaderText = "SĐT";
            dgvEmployee.Columns[4].HeaderText = "Email";
            dgvEmployee.Columns[5].HeaderText = "Giới Tính";
            dgvEmployee.Columns[6].HeaderText = "Địa Chỉ";
            dgvEmployee.Columns[7].HeaderText = "Mã Dự Án";
            dgvEmployee.Columns[8].HeaderText = "Mã Phòng Ban";
            dgvEmployee.Columns[9].HeaderText = "Ảnh";

            // Thiết lập DataGridView là chỉ đọc
            dgvEmployee.ReadOnly = true;
        }

        private void dgvEmployee_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Hiển thị thông báo lỗi khi người dùng cố gắng sửa đổi ô
            MessageBox.Show("Lỗi: Không được thay đổi thông tin dưới bảng.");
            e.Cancel = true; // Hủy bỏ việc chỉnh sửa
        }



        private void Quanly_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isThoat) Application.Exit();
        }



        private void dgvEmployee_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            // Lấy nhân viên từ danh sách dựa trên chỉ số hàng
            Employee em = lstEmp[e.RowIndex];
            if (em.Id != 0) // Giả sử ID là int và 0 là giá trị không hợp lệ
            {
                // Điền thông tin nhân viên vào các TextBox
                tbId.Text = em.Id.ToString();
                tbName.Text = em.Name;
                ckGender.Checked = em.Gender;
                tbSDT.Text = em.SDT;
                tbEmail.Text = em.Email;
                tbAddress.Text = em.Address;
                tbMaduan.Text = em.Maduan;
                cbMaphongban.Text = em.Maphongban;

                // Kiểm tra trước khi gán giá trị cho DateTimePicker
                if (em.BirthDate != DateTime.MinValue)
                {
                    dateTimePicker1.Value = em.BirthDate;
                }
                else
                {
                    dateTimePicker1.Value = DateTime.Now; // Giá trị mặc định
                }

                // Load ảnh nhân viên nếu có
                if (!string.IsNullOrEmpty(em.ImagePath) && System.IO.File.Exists(em.ImagePath))
                {
                    pbEmployeeImage.Image = Image.FromFile(em.ImagePath);
                }
                else
                {
                    pbEmployeeImage.Image = null; // Không có ảnh
                }
            }
            else
            {
                // Nếu ID không có giá trị, làm trống các ô nhập liệu
                ClearInputFields();
            }
        }



        private void dgvEmployee_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Lấy tên cột mà người dùng nhấn vào
            string columnName = dgvEmployee.Columns[e.ColumnIndex].DataPropertyName;

            // Sắp xếp theo cột được nhấn (có thể đổi lại thành ASC hoặc DESC)
            var sortedList = lstEmp.OrderBy(emp => emp.GetType().GetProperty(columnName).GetValue(emp, null)).ToList();

            // Cập nhật lại DataSource
            bs.DataSource = sortedList;
            bs.ResetBindings(false);
        }



        private void ClearInputFields()
        {
            tbId.Text = "";
            tbName.Text = "";
            tbAddress.Text = "";
            tbSDT.Text = "";
            tbEmail.Text = "";
            tbAddress.Text = "";
            tbMaduan.Text = "";
            cbMaphongban.Text = "";
            ckGender.Checked = false;
            pbEmployeeImage.Image = null;
            dateTimePicker1.Value = DateTime.Now;
        }

        private void SetupImageList()
        {
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(24, 24);

            imageList.Images.Add(Image.FromFile("Images/addnv.png"));
            imageList.Images.Add(Image.FromFile("Images/editnv.png"));
            imageList.Images.Add(Image.FromFile("Images/deletenv.png"));
            imageList.Images.Add(Image.FromFile("Images/clear.png"));

            btAddNew.ImageList = imageList;
            btAddNew.ImageIndex = 0;

            btEdit.ImageList = imageList;
            btEdit.ImageIndex = 1;

            btDelete.ImageList = imageList;
            btDelete.ImageIndex = 2;

            btClear.ImageList = imageList;
            btClear.ImageIndex = 3;
        }



        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            this.Text = dateTimePicker1.Value.ToString("dd MMMM yyyy");
        }








        private void btDangXuat_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DangXuat?.Invoke(this, EventArgs.Empty); // Gọi sự kiện đăng xuất nếu người dùng chọn Yes
            }
        }


        private void btThoat_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btAddNew_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem các trường có hợp lệ không
                if (string.IsNullOrWhiteSpace(tbId.Text) || string.IsNullOrWhiteSpace(tbName.Text) ||
                    string.IsNullOrWhiteSpace(tbSDT.Text) || string.IsNullOrWhiteSpace(tbEmail.Text) ||
                    string.IsNullOrWhiteSpace(tbAddress.Text) || string.IsNullOrWhiteSpace(tbMaduan.Text) ||
                    string.IsNullOrWhiteSpace(cbMaphongban.Text))
                {
                    MessageBox.Show("Lỗi: Vui lòng điền đầy đủ thông tin ID, Tên, SĐT, Email, Địa chỉ, Mã dự án và Mã phòng ban.");
                    return;
                }

                // Kiểm tra độ dài của ID, Tên, Email, Mã dự án và Mã phòng ban
                if (tbId.Text.Length > 10)
                {
                    MessageBox.Show("Lỗi: Mã nhân viên không được vượt quá 10 ký tự.");
                    return;
                }
                if (tbName.Text.Length > 50)
                {
                    MessageBox.Show("Lỗi: Tên không được vượt quá 50 ký tự.");
                    return;
                }
                if (tbEmail.Text.Length > 50)
                {
                    MessageBox.Show("Lỗi: Email không được vượt quá 50 ký tự.");
                    return;
                }
                if (tbMaduan.Text.Length > 15)
                {
                    MessageBox.Show("Lỗi: Mã dự án không được vượt quá 15 ký tự.");
                    return;
                }
                if (cbMaphongban.Text.Length > 15)
                {
                    MessageBox.Show("Lỗi: Mã phòng ban không được vượt quá 15 ký tự.");
                    return;
                }

                int newId = int.Parse(tbId.Text);
                if (lstEmp.Any(emp => emp.Id == newId))
                {
                    MessageBox.Show("Lỗi: ID đã tồn tại. Vui lòng nhập ID khác.");
                    return;
                }

                // Kiểm tra tên có chứa ký tự không hợp lệ
                if (!System.Text.RegularExpressions.Regex.IsMatch(tbName.Text, @"^[\p{L} .'-]+$"))
                {
                    MessageBox.Show("Lỗi: Tên không được chứa số hoặc ký tự đặc biệt.");
                    return;
                }
                // Kiểm tra độ tuổi phải đủ 18
                DateTime birthDate = dateTimePicker1.Value;
                int age = DateTime.Now.Year - birthDate.Year;
                if (birthDate > DateTime.Now.AddYears(-age)) age--;

                if (age < 18)
                {
                    MessageBox.Show("Lỗi: Nhân viên phải đủ 18 tuổi trở lên.");
                    return;
                }

                // Kiểm tra SĐT có 10 số và bắt đầu bằng số 0
                if (!System.Text.RegularExpressions.Regex.IsMatch(tbSDT.Text, @"^0\d{9}$"))
                {
                    MessageBox.Show("Lỗi: SĐT phải là 10 số và bắt đầu bằng số 0.");
                    return;
                }

                // Kiểm tra định dạng email
                if (!System.Text.RegularExpressions.Regex.IsMatch(tbEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Lỗi: Vui lòng nhập địa chỉ email hợp lệ.");
                    return;
                }

                // Thêm nhân viên mới
                Employee newEmp = new Employee
                {
                    Id = newId,
                    Name = tbName.Text,
                    Gender = ckGender.Checked,
                    SDT = tbSDT.Text,
                    Email = tbEmail.Text,
                    Address = tbAddress.Text,
                    Maduan = tbMaduan.Text,
                    Maphongban = cbMaphongban.Text,
                    ImagePath = employeeImagePath,
                    BirthDate = dateTimePicker1.Value.Date
                };

                lstEmp.Add(newEmp);
                AddEmployee(newEmp);

                bs.DataSource = lstEmp;  // Cập nhật lại DataSource
                bs.ResetBindings(false);  // Làm mới BindingSource để hiển thị

                ClearInputFields();
                MessageBox.Show("Thêm nhân viên thành công!");
            }
            catch (FormatException)
            {
                MessageBox.Show("Lỗi: Vui lòng nhập số nguyên hợp lệ cho ID.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void btEdit_Click_1(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow == null) return;

            int idx = dgvEmployee.CurrentRow.Index;
            Employee em = lstEmp[idx];

            try
            {
                int newId = int.Parse(tbId.Text);

                // Kiểm tra nếu người dùng cố gắng thay đổi ID
                if (newId != em.Id)
                {
                    MessageBox.Show("Không được phép sửa ID!");
                    tbId.Text = em.Id.ToString(); // Đặt lại ID ban đầu trong textbox
                    return;
                }

                // Kiểm tra độ dài của Tên, Email, Mã dự án và Mã phòng ban
                if (tbName.Text.Length > 50)
                {
                    MessageBox.Show("Lỗi: Tên không được vượt quá 50 ký tự.");
                    return;
                }
                // Kiểm tra độ tuổi phải đủ 18
                DateTime birthDate = dateTimePicker1.Value;
                int age = DateTime.Now.Year - birthDate.Year;
                if (birthDate > DateTime.Now.AddYears(-age)) age--;

                if (age < 18)
                {
                    MessageBox.Show("Lỗi: Nhân viên phải đủ 18 tuổi trở lên.");
                    return;
                }
                if (tbEmail.Text.Length > 50)
                {
                    MessageBox.Show("Lỗi: Email không được vượt quá 50 ký tự.");
                    return;
                }
                if (tbMaduan.Text.Length > 15)
                {
                    MessageBox.Show("Lỗi: Mã dự án không được vượt quá 15 ký tự.");
                    return;
                }
                if (cbMaphongban.Text.Length > 15)
                {
                    MessageBox.Show("Lỗi: Mã phòng ban không được vượt quá 15 ký tự.");
                    return;
                }

                // Kiểm tra xem các trường có hợp lệ không
                if (string.IsNullOrWhiteSpace(tbName.Text) || string.IsNullOrWhiteSpace(tbSDT.Text) || string.IsNullOrWhiteSpace(tbEmail.Text))
                {
                    MessageBox.Show("Lỗi: Vui lòng điền đầy đủ thông tin Tên, SĐT và Email.");
                    return;
                }

                // Kiểm tra SĐT có 10 số và bắt đầu bằng số 0
                if (!System.Text.RegularExpressions.Regex.IsMatch(tbSDT.Text, @"^0\d{9}$"))
                {
                    MessageBox.Show("Lỗi: SĐT phải là 10 số và bắt đầu bằng số 0.");
                    return;
                }

                // Kiểm tra định dạng email
                if (!System.Text.RegularExpressions.Regex.IsMatch(tbEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Lỗi: Vui lòng nhập địa chỉ email hợp lệ.");
                    return;
                }

                // Cập nhật thông tin nhân viên
                em.Name = tbName.Text;
                em.Gender = ckGender.Checked;
                em.SDT = tbSDT.Text;
                em.Email = tbEmail.Text;
                em.Address = tbAddress.Text;
                em.Maduan = tbMaduan.Text;
                em.Maphongban = cbMaphongban.Text;
                em.ImagePath = employeeImagePath;
                em.BirthDate = dateTimePicker1.Value.Date;

                bs.ResetBindings(false);
                UpdateEmployee(em);

                ClearInputFields();

                MessageBox.Show("Cập nhật nhân viên thành công!"); // Thông báo cập nhật thành công
            }
            catch (FormatException)
            {
                MessageBox.Show("Lỗi: Vui lòng nhập số nguyên hợp lệ cho ID.");
            }
        }




        private void btDelete_Click_1(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow == null) return;

            // Hộp thoại xác nhận
            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa nhân viên này?",
                                                  "Xác nhận xóa",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            // Nếu người dùng chọn Yes
            if (result == DialogResult.Yes)
            {
                int idx = dgvEmployee.CurrentRow.Index;

                var empId = lstEmp[idx].Id;


                lstEmp.RemoveAt(idx);
                DeleteEmployee(empId);
                bs.ResetBindings(false);
                ClearInputFields();

                MessageBox.Show("Xóa nhân viên thành công!");  // Thông báo thành công
            }
        }


        private void btSelectImage_Click_1(object sender, EventArgs e)
        {

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    employeeImagePath = ofd.FileName;
                    pbEmployeeImage.Image = Image.FromFile(employeeImagePath);
                }
            }
        }

        private void btSearch_Click(object sender, EventArgs e)
        {
            string searchText = tbSearchName.Text.ToLower(); // Lấy giá trị từ TextBox và chuyển về chữ thường

            // Lọc danh sách nhân viên theo điều kiện gần đúng với ID hoặc Name
            var filteredList = lstEmp.Where(emp =>
                emp.Id.ToString().Contains(searchText) || // Tìm gần đúng theo Id
                emp.Name.ToLower().Contains(searchText) // Tìm gần đúng theo Name
            ).ToList();

            // Cập nhật lại DataSource của DataGridView
            bs.DataSource = filteredList;
            bs.ResetBindings(false);

            // Nếu không có kết quả nào
            if (filteredList.Count == 0)
            {
                MessageBox.Show("Không tìm thấy nhân viên nào với thông tin đã nhập.");
            }
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btSort_Click(object sender, EventArgs e)
        {

            List<Employee> sortedList;

            if (cbSortOptions.SelectedItem.ToString() == "Sắp xếp theo tên")
            {
                // Sắp xếp danh sách nhân viên theo tên
                sortedList = lstEmp.OrderBy(emp => emp.Name).ToList();
            }
            else
            {
                // Sắp xếp danh sách nhân viên theo ID
                sortedList = lstEmp.OrderBy(emp => emp.Id).ToList();
            }

            // Cập nhật lại DataSource của DataGridView
            bs.DataSource = sortedList; // Cập nhật DataSource sau khi sắp xếp
            bs.ResetBindings(false); // Đảm bảo DataGridView được làm mới

            MessageBox.Show("Danh sách đã được sắp xếp.");
        }



        private void btClear_Click_Click(object sender, EventArgs e)
        {
            ClearInputFields(); // Gọi hàm đã có để làm trống các ô nhập liệu

        }



        private void btRefresh_Click(object sender, EventArgs e)
        {
            // Đặt lại DataSource về danh sách gốc lstEmp
            bs.DataSource = lstEmp;
            bs.ResetBindings(false);


            tbSearchName.Clear();
        }


    }
}

