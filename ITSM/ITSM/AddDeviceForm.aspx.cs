using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace ITSM
{

    [Serializable]
    public class SelectedDevice
    {
        public string DeviceID { get; set; }
        public string DeviceInfo { get; set; }
    }

    public partial class AddDeviceForm : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();


        private List<SelectedDevice> CurrentSelectedDevices
        {
            get
            {
                if (ViewState["SelectedDevices"] == null)
                    ViewState["SelectedDevices"] = new List<SelectedDevice>();
                return (List<SelectedDevice>)ViewState["SelectedDevices"];
            }
            set { ViewState["SelectedDevices"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMainDevices();
                string id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                {
                    hfID.Value = id;
                    LoadData(id);
                    lblPageTitle.Text = "Edit Additional Device (ID: " + id + ")";
                }
            }
        }

        private void LoadMainDevices()
        {

            string sql = "SELECT DeviceID, (Brand + ' ' + Model + ' [' + SerialNumber + ']') as DeviceInfo FROM device WHERE IsDelete IS NULL ORDER BY Brand";
            DataTable dt = db.SqlToDt(sql);

            ddlMainDevices.DataSource = dt;
            ddlMainDevices.DataTextField = "DeviceInfo";
            ddlMainDevices.DataValueField = "DeviceID";
            ddlMainDevices.DataBind();
            ddlMainDevices.Items.Insert(0, new ListItem("-- Select Device --", "0"));
        }





        private void LoadData(string id)
        {
            string sql = "SELECT * FROM AdditionalDevices WHERE AD_ID = " + id;
            DataTable dt = db.SqlToDt(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                txtDeviceType.Text = dr["DeviceType"].ToString();
                txtDescription.Text = dr["Description"].ToString();

                txtScrapDescription.Text = dr["ScrapDescription"].ToString();
                txtAccountingCode.Text = dr["AccountingCode"].ToString();
                txtPurchaseNumber.Text = dr["PurchaseNumber"].ToString();
                ddlStatus.SelectedValue = dr["Status"].ToString();

                if (dr["ScrapDescription"] != DBNull.Value)
                {
                    txtScrapDescription.Text = dr["ScrapDescription"].ToString();
                }

                if (dr["PurchaseDate"] != DBNull.Value)
                    txtPurchaseDate.Text = Convert.ToDateTime(dr["PurchaseDate"]).ToString("yyyy-MM-dd");

                if (dr["WarrantyEnd"] != DBNull.Value)
                    txtWarrantyEnd.Text = Convert.ToDateTime(dr["WarrantyEnd"]).ToString("yyyy-MM-dd");

                if (dr["ScrapDate"] != DBNull.Value)
                    txtScrapDate.Text = Convert.ToDateTime(dr["ScrapDate"]).ToString("yyyy-MM-dd");

                string relSql = "SELECT D.DeviceID, " + Environment.NewLine;
                relSql += " (D.Brand + ' ' + D.Model + ' [' + D.SerialNumber + ']') as DeviceInfo " + Environment.NewLine;
                relSql += " FROM DeviceRel DR " + Environment.NewLine;
                relSql += " INNER JOIN device D ON DR.PrimaryDevice = D.DeviceID " + Environment.NewLine;
                relSql += " WHERE DR.SecondaryDevice = " + id + " " + Environment.NewLine;
                relSql += " AND DR.Status = 1";

                DataTable dtRel = db.SqlToDt(relSql);
                List<SelectedDevice> list = new List<SelectedDevice>();
                foreach (DataRow row in dtRel.Rows)
                {
                    list.Add(new SelectedDevice
                    {
                        DeviceID = row["DeviceID"].ToString(),
                        DeviceInfo = row["DeviceInfo"].ToString()
                    });
                }


                CurrentSelectedDevices = list;
                BindSelectedRepeater();
            }
        }


        private void LoadSelectedDevices(int adId)
        {
            
            string sql = $@"SELECT dr.PrimaryDevice as DeviceID, d.DeviceName + ' (' + d.SerialNo + ')' as DeviceInfo 
                    FROM DeviceRel dr
                    JOIN Devices d ON dr.PrimaryDevice = d.DeviceID
                    WHERE dr.SecondaryDevice = {adId} AND dr.Status = 1";

            DataTable dt = db.SqlToDt(sql);
            List<SelectedDevice> list = new List<SelectedDevice>();

            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new SelectedDevice
                {
                    DeviceID = dr["DeviceID"].ToString(),
                    DeviceInfo = dr["DeviceInfo"].ToString()
                });
            }

            CurrentSelectedDevices = list;
            rptSelectedDevices.DataSource = list;
            rptSelectedDevices.DataBind();
        }


        protected void btnAddDevice_Click(object sender, EventArgs e)
        {
            if (ddlMainDevices.SelectedValue != "0")
            {
                var list = CurrentSelectedDevices;

                if (!list.Any(x => x.DeviceID == ddlMainDevices.SelectedValue))
                {
                    list.Add(new SelectedDevice
                    {
                        DeviceID = ddlMainDevices.SelectedValue,
                        DeviceInfo = ddlMainDevices.SelectedItem.Text
                    });
                    CurrentSelectedDevices = list;
                    BindSelectedRepeater();
                }
                ddlMainDevices.SelectedIndex = 0;
            }
        }


        protected void rptSelectedDevices_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                var list = CurrentSelectedDevices;
                list.RemoveAll(x => x.DeviceID == e.CommandArgument.ToString());
                CurrentSelectedDevices = list;
                BindSelectedRepeater();
            }
        }

        private void BindSelectedRepeater()
        {
            rptSelectedDevices.DataSource = CurrentSelectedDevices;
            rptSelectedDevices.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string currentUser = cookie.Oku("User") ?? "System";
            int adId;

            string pDate = string.IsNullOrEmpty(txtPurchaseDate.Text) ? "NULL" : "'" + txtPurchaseDate.Text + "'";
            string wDate = string.IsNullOrEmpty(txtWarrantyEnd.Text) ? "NULL" : "'" + txtWarrantyEnd.Text + "'";
            string sDate = string.IsNullOrEmpty(txtScrapDate.Text) ? "NULL" : "'" + txtScrapDate.Text + "'";

            string isDeleteValue = string.IsNullOrEmpty(txtScrapDate.Text) ? "NULL" : "'X'";

            if (string.IsNullOrEmpty(hfID.Value))
            {

                string sql = $@"INSERT INTO AdditionalDevices 
            (DeviceType, Description, Status, AccountingCode, PurchaseNumber, PurchaseDate, WarrantyEnd, ScrapDate, ScrapDescription, IsDelete, RecordDate, RecordUser) 
            VALUES 
            ('{txtDeviceType.Text.Replace("'", "''")}', '{txtDescription.Text.Replace("'", "''")}', {ddlStatus.SelectedValue}, 
             '{txtAccountingCode.Text.Replace("'", "''")}', '{txtPurchaseNumber.Text.Replace("'", "''")}', 
             {pDate}, {wDate}, {sDate}, '{txtScrapDescription.Text.Replace("'", "''")}', {isDeleteValue}, GETDATE(), '{currentUser}'); SELECT SCOPE_IDENTITY();";

                adId = Convert.ToInt32(db.SqlToDt(sql).Rows[0][0]);
            }
            else
            {

                adId = Convert.ToInt32(hfID.Value);
                string sql = $@"UPDATE AdditionalDevices SET 
            DeviceType = '{txtDeviceType.Text.Replace("'", "''")}', 
            Description = '{txtDescription.Text.Replace("'", "''")}', 
            Status = {ddlStatus.SelectedValue}, 
            AccountingCode = '{txtAccountingCode.Text.Replace("'", "''")}', 
            PurchaseNumber = '{txtPurchaseNumber.Text.Replace("'", "''")}', 
            PurchaseDate = {pDate}, 
            WarrantyEnd = {wDate}, 
            ScrapDate = {sDate}, 
            ScrapDescription = '{txtScrapDescription.Text.Replace("'", "''")}',
            IsDelete = {isDeleteValue},
            UpdateDate = GETDATE(), 
            UpdateUser = '{currentUser}' 
            WHERE AD_ID = {adId}";

                db.ExecStr(sql);


                db.ExecStr($"UPDATE DeviceRel SET Status = 0, EndDate = GETDATE() WHERE SecondaryDevice = {adId} AND Status = 1");
            }

  
            foreach (var item in CurrentSelectedDevices)
            {
                string checkSql = $"SELECT RelID FROM DeviceRel WHERE PrimaryDevice = {item.DeviceID} AND SecondaryDevice = {adId}";
                if (db.SqlToDt(checkSql).Rows.Count > 0)
                    db.ExecStr($"UPDATE DeviceRel SET Status = 1, EndDate = NULL WHERE PrimaryDevice = {item.DeviceID} AND SecondaryDevice = {adId}");
                else
                    db.ExecStr($"INSERT INTO DeviceRel (PrimaryDevice, SecondaryDevice, Status, StartDate, RecordDate, RecordUser) VALUES ({item.DeviceID}, {adId}, 1, GETDATE(), GETDATE(), '{currentUser}')");
            }

            ShowAlert("success", "Success", "Changes saved successfully.", "AddDevices.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e) => Response.Redirect("AddDevices.aspx");

        private void ShowAlert(string type, string title, string message, string url = "")
        {
            hfAlertType.Value = type;
            hfAlertTitle.Value = title;
            hfAlertMessage.Value = message;
            hfRedirectUrl.Value = url;
        }
    }
}