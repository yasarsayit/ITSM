using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class CategoryManagement : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CheckTools chk = new CheckTools();
        TranslationTools trn = new TranslationTools();
        CookieTools cookie = new CookieTools();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null && string.IsNullOrEmpty(cookie.Oku("User"))) { Response.Redirect("Login.aspx"); return; }


            btnAddType.Text = gettext("btn_add", "Add");
            btnAddSubType.Text = gettext("btn_add", "Add");

            if (!IsPostBack)
            {
                this.Title = gettext("ctgmng_title", "Category Management");

                this.DataBind();

                BindMainTypes();
            }
        }
        public string gettext(string tag, string fallback)
        {
            if (Session["Lang"] == null)
            {
                Session["Lang"] = trn.GetLangCode();
            }
            return trn.GetText(tag, fallback, Session["Lang"].ToString()).ToString();
        }


        private int SelectedMainTypeId
        {
            get { return ViewState["SelectedMainTypeId"] != null ? (int)ViewState["SelectedMainTypeId"] : 0; }
            set { ViewState["SelectedMainTypeId"] = value; }
        }

        private int SelectedSubTypeId
        {
            get { return ViewState["SelectedSubTypeId"] != null ? (int)ViewState["SelectedSubTypeId"] : 0; }
            set { ViewState["SelectedSubTypeId"] = value; }
        }

        protected void gvTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedMainTypeId = Convert.ToInt32(gvTypes.SelectedDataKey.Value);
            DataTable dt = db.SqlToDt($"SELECT Description, DescriptionEN FROM RequestTypes WHERE TypeID = {SelectedMainTypeId}");

            if (dt != null && dt.Rows.Count > 0)
            {
                txtNewType.Text = dt.Rows[0]["Description"].ToString();
                txtNewTypeEN.Text = dt.Rows[0]["DescriptionEN"].ToString();
                btnAddType.Text = gettext("btn_update", "Update");
            }
        }


        protected void gvSubTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedSubTypeId = Convert.ToInt32(gvSubTypes.SelectedDataKey.Value);
            DataTable dt = db.SqlToDt($"SELECT Description, DescriptionEN FROM RequestSubTypes WHERE SubTypeID = {SelectedSubTypeId}");

            if (dt != null && dt.Rows.Count > 0)
            {
                txtNewSubType.Text = dt.Rows[0]["Description"].ToString();
                txtNewSubTypeEN.Text = dt.Rows[0]["DescriptionEN"].ToString();
                btnAddSubType.Text = gettext("btn_update", "Update");
            }
        }

        protected void btnAddType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNewType.Text.Trim()) || string.IsNullOrEmpty(txtNewTypeEN.Text.Trim()))
            {
                ShowAlert(gettext("msg_subtype_validation", "Please select Main Type for Sub-Request and fill in both language isolation fields."), "warning");
                return;
            }
            string val = chk.temizle(txtNewType.Text.Trim());
            string valEN = chk.temizle(txtNewTypeEN.Text.Trim());

            if (SelectedMainTypeId == 0)
            {
                db.SqlToDt($"INSERT INTO RequestTypes (Description, DescriptionEN) VALUES ('{val}', '{valEN}')");
                ShowAlert(gettext("msg_add_success", "Main Type added successfully."), "success");
            }
            else
            {
                db.SqlToDt($"UPDATE RequestTypes SET Description = '{val}', DescriptionEN = '{valEN}' WHERE TypeID = {SelectedMainTypeId}");
                ShowAlert(gettext("msg_update_success", "Main Type updated successfully."), "success");
                SelectedMainTypeId = 0;
                btnAddType.Text = gettext("btn_add", "Add");
            }

            txtNewType.Text = "";
            txtNewTypeEN.Text = "";
            BindMainTypes();
        }



        protected void btnAddSubType_Click(object sender, EventArgs e)
        {
            if (ddlMainTypes.SelectedValue == "0" || string.IsNullOrEmpty(txtNewSubType.Text.Trim()) || string.IsNullOrEmpty(txtNewSubTypeEN.Text.Trim()))
            {
                ShowAlert(gettext("msg_subtype_validation", "Please select Main Type for Sub-Request and fill in both language isolation fields."), "warning");
                return;
            }

            string val = chk.temizle(txtNewSubType.Text.Trim());
            string valEN = chk.temizle(txtNewSubTypeEN.Text.Trim());
            string pId = ddlMainTypes.SelectedValue;

            if (SelectedSubTypeId == 0)
            {
                db.SqlToDt($"INSERT INTO RequestSubTypes (Description, DescriptionEN, ReqTypeId) VALUES ('{val}', '{valEN}', {pId})");
                ShowAlert(gettext("msg_add_sub_success", "Sub Type added successfully."), "success");
            }
            else
            {
                db.SqlToDt($"UPDATE RequestSubTypes SET Description = '{val}', DescriptionEN = '{valEN}' WHERE SubTypeID = {SelectedSubTypeId}");
                ShowAlert(gettext("msg_update_sub_success", "Sub Type updated successfully."), "success");
                SelectedSubTypeId = 0;
                btnAddSubType.Text = gettext("btn_add", "Add");
            }

            txtNewSubType.Text = "";
            txtNewSubTypeEN.Text = "";
            BindSubTypes(pId);
        }


        private void BindMainTypes()
        {
            DataTable dt = db.SqlToDt("SELECT TypeID AS Id, Description AS TypeName, DescriptionEN AS TypeNameEN FROM RequestTypes ORDER BY Description ASC");
            gvTypes.DataSource = dt;
            gvTypes.DataBind();

            ddlMainTypes.DataSource = dt;
            ddlMainTypes.DataTextField = "TypeName";
            ddlMainTypes.DataValueField = "Id";
            ddlMainTypes.DataBind();
            ddlMainTypes.Items.Insert(0, new ListItem(gettext("select_main_type", "-- Select Main Type --"), "0"));
        }

        private void BindSubTypes(string typeId)
        {
            if (string.IsNullOrEmpty(typeId) || typeId == "0")
            {
                gvSubTypes.DataSource = null;
                gvSubTypes.DataBind();
                return;
            }

            DataTable dt = db.SqlToDt("SELECT SubTypeID AS Id, Description AS SubTypeName, DescriptionEN AS SubTypeNameEN FROM RequestSubTypes WHERE ReqTypeId = " + chk.temizle(typeId) + " ORDER BY Description ASC");

            if (dt != null && dt.Rows.Count > 0)
            {
                gvSubTypes.DataSource = dt;
                gvSubTypes.DataBind();
            }
            else
            {
                gvSubTypes.DataSource = null;
                gvSubTypes.DataBind();
            }
        }

        protected void ddlMainTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubTypes(ddlMainTypes.SelectedValue);
        }

        private void ShowAlert(string message, string type)
        {
            string script = $"showAlert('{message}', '{type}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
        }
    }
}