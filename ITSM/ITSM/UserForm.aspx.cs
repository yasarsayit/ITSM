using System;
using System.Data;

namespace ITSM
{
    public partial class UserForm : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string id = Request.QueryString["id"];
                    if (!string.IsNullOrEmpty(id))
                    {
                        LoadData(id);
                        lblPageTitle.Text = "Edit User (ID: " + id + ")";
                    }
                    else
                    {
                        lblPageTitle.Text = "Add New User";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Error", "Page load error: " + ex.Message);
            }
        }

        private void LoadData(string id)
        {
            try
            {
                string sql = "SELECT * FROM Users WHERE UserID = " + id;
                DataTable dt = db.SqlToDt(sql);

                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    hfID.Value = id;

                    txtEmployeeID.Text = dr["EmployeeID"].ToString();
                    txtNameSurname.Text = dr["NameSurname"].ToString();
                    txtUserName.Text = dr["UserName"].ToString();
                    txtEmail.Text = dr["Email"].ToString();
                    txtDepartment.Text = dr["Department"].ToString();
                    txtTitle.Text = dr["Title"].ToString();
                    txtUserType.Text = dr["UserType"].ToString();
                    txtPass.Text = dr["Pass"].ToString();

                    if (ddlStatus.Items.FindByValue(dr["Status"].ToString()) != null)
                        ddlStatus.SelectedValue = dr["Status"].ToString();

                    if (dr["StartDate"] != DBNull.Value)
                        txtStartDate.Text = Convert.ToDateTime(dr["StartDate"]).ToString("yyyy-MM-dd");

                    if (dr["EndDate"] != DBNull.Value)
                        txtEndDate.Text = Convert.ToDateTime(dr["EndDate"]).ToString("yyyy-MM-dd");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Error", "Data load error: " + ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string id = hfID.Value;
                string currentUser = cookie.Oku("User") ?? "Admin";

                string empID = txtEmployeeID.Text.Trim().Replace("'", "''");
                string name = txtNameSurname.Text.Trim().Replace("'", "''");
                string user = txtUserName.Text.Trim().Replace("'", "''");
                string email = txtEmail.Text.Trim().Replace("'", "''");
                string dept = txtDepartment.Text.Trim().Replace("'", "''");
                string title = txtTitle.Text.Trim().Replace("'", "''");
                string userType = txtUserType.Text.Trim().Replace("'", "''");
                string pass = txtPass.Text.Trim().Replace("'", "''");
                string status = ddlStatus.SelectedValue;

                string startDate = string.IsNullOrEmpty(txtStartDate.Text) ? "NULL" : "'" + txtStartDate.Text + "'";
                string endDate = string.IsNullOrEmpty(txtEndDate.Text) ? "NULL" : "'" + txtEndDate.Text + "'";

                string isDeletedValue = string.IsNullOrEmpty(txtEndDate.Text) ? "NULL" : "'X'";

                string sql = "";
                if (string.IsNullOrEmpty(id))
                {
                    sql = "INSERT INTO Users (EmployeeID, NameSurname, UserName, Email, Department, Title, UserType, Pass, Status, StartDate, EndDate, IsDeleted) VALUES (" +
                          $"'{empID}', '{name}', '{user}', '{email}', '{dept}', '{title}', '{userType}', '{pass}', {status}, {startDate}, {endDate}, {isDeletedValue})";
                }
                else
                {
                    sql = "UPDATE Users SET " +
                          $"EmployeeID = '{empID}', NameSurname = '{name}', UserName = '{user}', Email = '{email}', " +
                          $"Department = '{dept}', Title = '{title}', UserType = '{userType}', Pass = '{pass}', " +
                          $"Status = {status}, StartDate = {startDate}, EndDate = {endDate}, IsDeleted = {isDeletedValue} " +
                          $"WHERE UserID = {id}";
                }

                int result = db.ExecStr(sql);
                if (result > 0)
                {
                    ShowAlert("success", "Success", "User saved successfully.", "Users.aspx");
                }
                else
                {
                    ShowAlert("error", "Error", "Save error: " + db.Hata);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Error", "System error: " + ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Users.aspx");
        }

        private void ShowAlert(string type, string title, string message, string redirectUrl = "")
        {
            hfAlertType.Value = type;
            hfAlertTitle.Value = title;
            hfAlertMessage.Value = message;
            hfRedirectUrl.Value = redirectUrl;
        }
    }
}