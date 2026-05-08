using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.UI;

namespace ITSM
{
    public partial class _Default : Page
    {

        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();
        CheckTools chk = new CheckTools();

        protected void Page_Load(object sender, EventArgs e)
        {
            string userSession = Session["User"]?.ToString() ?? cookie.Oku("User");
            if (string.IsNullOrEmpty(userSession))
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string uType = cookie.Oku("UserType")?.Trim().ToLower();

            if (uType == "admin")
            {
                phAdminDashboard.Visible = true;

                if (!IsPostBack)
                {
                    LoadDashboardData();
                }
            }
            else
            {

                phAdminDashboard.Visible = false;
                Response.Redirect("Requests.aspx");
            }
        }

        public void LoadDashboardData()
        {
            try
            {
                string userSession = Session["User"]?.ToString() ?? cookie.Oku("User");
                userSession = chk.temizle(userSession);

                string sqlUserId = $"SELECT UserID FROM Users WHERE (UserName = '{userSession}' OR Email = '{userSession}') AND isDeleted IS NULL";
                DataTable dtU = db.SqlToDt(sqlUserId);

                if (dtU != null && dtU.Rows.Count > 0)
                {
                    string currentUserId = dtU.Rows[0]["UserID"].ToString();

                    string sqlSlaCheck = @"SELECT COUNT(*) FROM Requests 
                          WHERE (isDeleted IS NULL OR isDeleted != 'X') 
                          AND (IsConfirmed IS NULL OR IsConfirmed = 0) 
                          AND DATEDIFF(MINUTE, RequestDate, GETDATE()) > 30";
                    DataTable dtSla = db.SqlToDt(sqlSlaCheck);
                    int slaCount = (dtSla != null && dtSla.Rows.Count > 0) ? Convert.ToInt32(dtSla.Rows[0][0]) : 0;
                    pnlSlaAlert.Visible = (slaCount > 0);

                    string sqlAvg = @"SELECT AVG(DATEDIFF(MINUTE, RequestDate, GETDATE())) 
                     FROM Requests 
                     WHERE IsConfirmed > 0 AND (isDeleted IS NULL OR isDeleted != 'X')";
                    DataTable dtAvg = db.SqlToDt(sqlAvg);
                    if (dtAvg != null && dtAvg.Rows.Count > 0 && dtAvg.Rows[0][0] != DBNull.Value)
                    {
                        int totalMinutes = Convert.ToInt32(dtAvg.Rows[0][0]);
                        litAvgTime.Text = totalMinutes < 60 ? totalMinutes + " Mins" : (totalMinutes / 60) + " Hours " + (totalMinutes % 60) + " Mins";
                    }
                    else { litAvgTime.Text = "No Data"; }

                    string sqlOpen = $@"SELECT TOP 20 R.RequestId, R.Topic, R.RecordUser, R.IsConfirmed, R.RequestDate, 
                      COALESCE(NULLIF(LTRIM(RTRIM(U1.NameSurname)), ''), NULLIF(LTRIM(RTRIM(U2.NameSurname)), ''), R.RecordUser) AS RequesterName 
                      FROM Requests R 
                      LEFT JOIN Users U1 ON R.UserID = U1.UserID AND (U1.isDeleted IS NULL OR U1.isDeleted != 'X') 
                      LEFT JOIN Users U2 ON (U2.UserName = R.RecordUser OR U2.NameSurname = R.RecordUser OR U2.Email = R.RecordUser OR U2.EmployeeID = R.RecordUser) 
                          AND (U2.isDeleted IS NULL OR U2.isDeleted != 'X') 
                      WHERE (R.isDeleted IS NULL OR R.isDeleted != 'X') 
                      AND (R.IsConfirmed IS NULL OR R.IsConfirmed = 0 OR R.IsConfirmed = '0') 
                      ORDER BY R.RequestDate ASC, R.RequestId DESC";

                    DataTable dtOpen = db.SqlToDt(sqlOpen);
                    rptOpenRequests.DataSource = dtOpen;
                    rptOpenRequests.DataBind();
                    litOpenCount.Text = dtOpen != null ? dtOpen.Rows.Count.ToString() : "0";

                    string sqlMy = $@"SELECT RequestId, Topic, RequestDate, IsImportant, IsUrgent, IsConfirmed 
                      FROM Requests 
                      WHERE (isDeleted IS NULL OR isDeleted != 'X') 
                      AND (IsConfirmed != 3 )
                      AND (
                           EXISTS (SELECT 1 FROM Tasks WHERE Tasks.RequestID = Requests.RequestId AND Tasks.EmployeeID = '{currentUserId}')
                      )
                      ORDER BY RequestId DESC";

                    DataTable dtMy = db.SqlToDt(sqlMy);
                    if (dtMy != null)
                    {
                        rptMyRequests.DataSource = dtMy;
                        rptMyRequests.DataBind();
                        litMyCount.Text = dtMy.Rows.Count.ToString();
                    }
                    else
                    {
                        litMyCount.Text = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<div style='color:red; font-weight:bold;'>Dashboard Error: " + ex.Message + "</div>");
            }
        }
        protected string GetSlaTextColor(object isConfirmed, object isSlaExceeded)
        {
            if (isSlaExceeded != DBNull.Value && isSlaExceeded.ToString() == "1")
            {
                string status = isConfirmed != DBNull.Value ? isConfirmed.ToString() : "0";
                if (status == "0") return "sla-danger";
            }
            return "";
        }
        protected string GetRowClass(object isConfirmed, object isSlaExceeded)
        {
            string status = isConfirmed != DBNull.Value ? isConfirmed.ToString() : "0";

            switch (status)
            {
                case "1": return "status-processing";
                case "2": return "status-rejected";
                case "3": return "status-completed";
                default: return "status-waiting";
            }
        }
        protected string GetStatusBadge(object isConfirmed)
        {
            if (isConfirmed == null || isConfirmed == DBNull.Value)
                return "<span class=\"badge bg-light text-dark border\">Pending</span>";

            string status = isConfirmed.ToString().Trim();
            switch (status)
            {
                case "1":
                    return "<span class=\"badge bg-warning text-dark\">Approved</span>";
                case "2":
                    return "<span class=\"badge bg-danger\">Rejected</span>";
                case "3":
                    return "<span class=\"badge bg-success\">Completed</span>";
                case "0":
                default:
                    return "<span class=\"badge bg-light text-dark border\">Pending</span>";
            }
        }

        protected string GetPriorityBadges(object isImportant, object isUrgent)
        {
            bool urgent = isUrgent != DBNull.Value && (Convert.ToBoolean(isUrgent) || isUrgent.ToString() == "1");
            bool important = isImportant != DBNull.Value && (Convert.ToBoolean(isImportant) || isImportant.ToString() == "1");
            string html = "";

            if (urgent) html += "<span class='badge bg-danger me-1' title='ACİL'><i class='far fa-exclamation'></i></span>";
            if (important) html += "<span class='badge bg-warning text-dark me-1' title='ÖNEMLİ'><i class='fal fa-user-crown'></i></span>";

            return string.IsNullOrEmpty(html) ? "<span class='text-muted small'>Standart</span>" : html;
        }




    }
}