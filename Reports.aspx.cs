using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Web;
using System.Globalization;

public partial class Reports : System.Web.UI.Page
{

    DAL d1 = new DAL();
    DAL d = new DAL();
    public string rem_emp_count = "0";//vikas add 
    ReportDocument crystalReport = new ReportDocument();
    string curr_date = "";

    public void ButtonColor()
    {

        enable_disable();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        curr_date = Session["system_curr_date"].ToString();
        if (Session["comp_code"] == null || Session["comp_code"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }
        if (d.getaccess(Session["ROLE"].ToString(), "Month Wise", Session["COMP_CODE"].ToString()) == "I")
        {
            Response.Redirect("unauthorised_access.aspx");
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Month Wise", Session["COMP_CODE"].ToString()) == "R")
        {
            btnsendemail.Visible = false;
            btn_pf_challan_dwnld.Visible = false;
            btn_kyc_Download.Visible = false;
            btn_pf_challan_Xmls.Visible = false;
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Month Wise", Session["COMP_CODE"].ToString()) == "U")
        {
            btnsendemail.Visible = false;
            btn_pf_challan_dwnld.Visible = false;
            btn_kyc_Download.Visible = false;
            btn_pf_challan_Xmls.Visible = false;
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Month Wise", Session["COMP_CODE"].ToString()) == "C")
        {
            btn_pf_challan_dwnld.Visible = false;
            btn_kyc_Download.Visible = false;
            btn_pf_challan_Xmls.Visible = false;
        }
        //generate_excel();
        if (!IsPostBack)
        {
            ViewState["rem_emp_count"] = 0;
            rem_emp_count = ViewState["rem_emp_count"].ToString();
            d1.con1.Open();
            MySqlCommand cmd = new MySqlCommand("select concat(UNIT_CODE,'-',UNIT_NAME) from pay_unit_master WHERE comp_code='" + Session["comp_code"].ToString() + "' ORDER BY UNIT_CODE", d1.con1);
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ddlunitselect.Items.Add(dr[0].ToString());//ddl_banklist0.Items.Add(dr_banks[0].ToString());
            }
            ddlunitselect.Items.Insert(0, "ALL");
            ddlunitselect.SelectedIndex = 0;
            d1.con1.Close();
            Session["ReportMonthNo"] = "";

            ddl_client.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', client_code from pay_client_master where comp_code='" + Session["comp_code"] + "' and client_active_close='0' ORDER BY client_code", d.con);
            d.con.Open();
            try
            {
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_client.DataSource = dt_item;
                    ddl_client.DataTextField = dt_item.Columns[0].ToString();
                    ddl_client.DataValueField = dt_item.Columns[1].ToString();
                    ddl_client.DataBind();
                }
                dt_item.Dispose();
                hide_controls();
                d.con.Close();
                ddl_client.Items.Insert(0, "Select");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
            employee_status();
            //Designation

            ddl_designation.Items.Clear();
            System.Data.DataTable dt_item_grade = new System.Data.DataTable();
            MySqlDataAdapter cmd_item_grade = new MySqlDataAdapter("SELECT GRADE_DESC,GRADE_CODE FROM pay_grade_master  WHERE comp_code='" + Session["comp_code"].ToString() + "'", d.con);
            d.con.Open();
            try
            {
                cmd_item_grade.Fill(dt_item_grade);
                if (dt_item_grade.Rows.Count > 0)
                {
                    ddl_designation.DataSource = dt_item_grade;
                    ddl_designation.DataTextField = dt_item_grade.Columns[0].ToString();
                    ddl_designation.DataValueField = dt_item_grade.Columns[1].ToString();
                    ddl_designation.DataBind();
                }
                dt_item_grade.Dispose();
                d.con.Close();
                ddl_designation.Items.Insert(0, "ALL");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
            ddl_act_SelectedIndexChanged(null, null);
        }

        btnsendemail.Visible = false;
        UnitGridView.Visible = false;
        btn_kyc_Download.Visible = false;
        btn_pf_challan_dwnld.Visible = false;
        //UnitGrid_PF.Visible = false;
        UnitGrid_PF.Visible = true;
        btn_pf_challan_Xmls.Visible = false;
        panel_esic_statement.Visible = false;
        panel_ptax.Visible = false;
        panel_bankexcel.Visible = false;



    }



    private void ReportLoad(string query, string downloadfilename)
    {
        d.con.Close();
        d1.con.Close();
        try
        {
            //btnsendemail.Visible = true;
            string downloadname = downloadfilename;
            System.Data.DataTable dt = new System.Data.DataTable();
            MySqlCommand cmd = new MySqlCommand(query);
            MySqlDataReader sda = null;
            cmd.Connection = d.con;
            d.con.Open();
            sda = cmd.ExecuteReader();
            dt.Load(sda);
            d.con.Close();
            crystalReport.SetDataSource(dt);
            crystalReport.Refresh();
            //  crystalReport.SetDatabaseLogon(@"Tanvi-Tej\Tanvi","");
            //Response.End();
            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, downloadname);


        }
        catch (Exception ex)
        {
            Response.Write("<script language='javascript'>alert('" + Server.HtmlEncode(ex.Message) + "')</script>");
            //var message = new JavaScriptSerializer().Serialize(ex.Message.ToString());
            //var script = string.Format("alert({0});", message);
            //ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "", script, true);
        }
        finally
        {
            Response.End();
        }
    }

    private void Reportformat(string query, string downloadfilename)
    {
        try
        {
            string downloadname = downloadfilename;
            btnsendemail.Visible = true;
            System.Data.DataTable dt = new System.Data.DataTable();
            MySqlCommand cmd = new MySqlCommand(query);
            MySqlDataReader sda = null;
            cmd.Connection = d.con;
            d.con.Open();
            sda = cmd.ExecuteReader();
            dt.Load(sda);
            d.con.Close();
            crystalReport.SetDataSource(dt);
            //crystalReport.Refresh();
            //  crystalReport.SetDatabaseLogon(@"Tanvi-Tej\Tanvi","");

            crystalReport.ExportToHttpResponse(ExportFormatType.Excel, Response, false, downloadname);
            Response.End();
        }
        catch (Exception ex)
        {
            Response.Write("<script language='javascript'>alert('" + Server.HtmlEncode(ex.Message) + "')</script>");

        }
        finally
        {
            Response.End();
        }
    }



    protected void btnclose_Click(object sender, EventArgs e)
    {
        //CrystalReportViewer1.ReportSource = null;
        //CrystalReportViewer1.RefreshReport();
        //Session["ReportMonthNo"] = "";
        //crystalReport.Dispose();
        UnitGridView.Visible = false;
        btn_kyc_Download.Visible = false;

        Response.Redirect("Home.aspx");
    }
    protected void lbempmusterroll_Click(object sender, EventArgs e)
    {

    }
    protected void lbtsalarystatementbyunit_Click(object sender, EventArgs e)
    {

    }
    //protected void btn_send_email_Click(object sender, EventArgs e)
    //{
    //    MySqlDataReader drmax = null;
    //    string toemailid = "";
    //    ButtonColor();
    //    if (ddlunitselect.Text != "ALL")
    //    {
    //        d.con1.Open();
    //        MySqlCommand cmdmax = new MySqlCommand("select unit_email_id from pay_unit_master where UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' and comp_code='" + Session["comp_code"].ToString() + "'", d.con1);
    //        drmax = cmdmax.ExecuteReader();
    //        if (!drmax.HasRows)
    //        {
    //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please add email id for unit to send email!!')", true);
    //            return;
    //        }
    //        else if (drmax.Read())
    //        {
    //            toemailid = drmax.GetValue(0).ToString();
    //        }
    //        toemailid = toemailid.Replace(" ", "");
    //        d.con1.Close();
    //        d.con1.Open();
    //        String Fromemailid = "", frompasswd = "";
    //        MySqlCommand cmdmax1 = new MySqlCommand("select email_id, email_password from pay_company_master where comp_code='" + Session["comp_code"].ToString() + "'", d.con1);
    //        drmax = cmdmax1.ExecuteReader();


    //        if (drmax.Read())
    //        {
    //            if (drmax.GetValue(0).ToString().Equals(DBNull.Value))
    //            {
    //                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please add email id for this Company!!')", true);
    //                return;
    //            }
    //            Fromemailid = drmax.GetValue(0).ToString();
    //            frompasswd = StringCipher.Decrypt(drmax.GetValue(1).ToString(), "1234567890");
    //        }
    //        if (!object.Equals(toemailid, ""))
    //        {
    //            //try
    //            //{
    //            ExportOptions CrExportOptions;
    //            DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
    //            PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
    //            CrDiskFileDestinationOptions.DiskFileName = "c:\\Report.pdf";
    //            CrExportOptions = crystalReport.ExportOptions;
    //            {
    //                CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
    //                CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
    //                CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
    //                CrExportOptions.FormatOptions = CrFormatTypeOptions;
    //            }
    //            crystalReport.Export();
    //            CrDiskFileDestinationOptions = null;
    //            CrExportOptions = null;
    //            CrFormatTypeOptions = null;
    //            CrystalReportViewer1.ReportSource = null;
    //            CrystalReportViewer1.RefreshReport();
    //            Session["ReportMonthNo"] = "";
    //            crystalReport.Dispose();
    //            //}
    //            //catch (Exception ex)
    //            //{
    //            //    MessageBox.Show(ex.ToString());
    //            //}
    //            MailMessage mail = new MailMessage();
    //            SmtpClient SmtpServer;
    //            if (Fromemailid.Contains("monalient"))
    //            {
    //                SmtpServer = new SmtpClient("smtp.net4india.com");
    //                SmtpServer.Port = 25;
    //                SmtpServer.EnableSsl = false;
    //            }
    //            else
    //            {
    //                SmtpServer = new SmtpClient("smtp.mail.yahoo.com");
    //                SmtpServer.Port = 587;
    //                SmtpServer.EnableSsl = true;
    //            }
    //            //try
    //            //{
    //            mail.From = new MailAddress(Fromemailid);
    //            mail.To.Add(toemailid);
    //            mail.Subject = "Report from " + Session["COMP_NAME"].ToString();
    //            mail.Body = "Please find the attached report.";

    //            System.Net.Mail.Attachment attachment;
    //            attachment = new System.Net.Mail.Attachment("c:\\Report.pdf");
    //            mail.Attachments.Add(attachment);


    //            SmtpServer.Credentials = new System.Net.NetworkCredential(Fromemailid, frompasswd);
    //            // SmtpServer.EnableSsl = true;

    //            SmtpServer.Send(mail);
    //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Email Sent Successfully !!')", true);
    //            //MessageBox.Show("");

    //            //}
    //            //catch (Exception ex)
    //            //{
    //            //    MessageBox.Show(ex.ToString());
    //            //}
    //            //finally
    //            //{
    //            mail.Dispose();
    //            SmtpServer.Dispose();
    //            //}
    //        }
    //        else
    //        {
    //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please add email id for unit to send email!!')", true);
    //        }
    //    }
    //}

    protected void btn_send_email_Click(object sender, EventArgs e)
    {
        Response.Redirect("Email_format.aspx");


    }

    protected void btnclear_Click(object sender, EventArgs e)
    {
        ButtonColor();
        //CrystalReportViewer1.ReportSource = null;
        //CrystalReportViewer1.RefreshReport();
        //Session["ReportMonthNo"] = "";
        //crystalReport.Dispose();
        btnsendemail.Visible = false;
        UnitGridView.Visible = false;
        btn_kyc_Download.Visible = false;
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }


    protected void lbtattendance_Click(object sender, EventArgs e)
    {

    }

    protected void btnpfstatement_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }

        ButtonColor();
        btnpfstatement.BackColor = System.Drawing.Color.GreenYellow;

        string dowmloadname = "PF_Statement";
        string query = null;
        string UnitList = "";
        crystalReport.Load(Server.MapPath("~/Rpt_Monthlypf.rpt"));
        //string a = txtfromdate.Text;
        DateTime dt = Convert.ToDateTime(txttodate.Text);
        string thisMonth = dt.ToString("MMMM");
        crystalReport.DataDefinition.FormulaFields["current_month"].Text = @"'" + thisMonth + "'";
        crystalReport.DataDefinition.FormulaFields["current_year"].Text = @"'" + txttodate.Text.Substring(3) + "'";

        if (ddlunitselect.Text == "ALL")
        {
            //        SELECT  pay_company_master.COMPANY_NAME, pay_company_master.PF_REG_NO, pay_company_master.CURRENT_MONTH,  pay_company_master.CURRENT_YEAR, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_company_master.comp_code, pay_employee_master.JOINING_DATE, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.PF_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.PF_GROSS, pay_attendance.PF, pay_attendance.COMP_PF, pay_attendance.COMP_PF_PEN, pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PIN, pay_company_master.PF_REG_OFFICE, pay_company_master.COMPANY_AUTHORISED_NAME, pay_company_master.COMPANY_AUTHORISED_DESIGNATION, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_employee_master.BIRTH_DATE FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code 
            query = " SELECT  pay_company_master.COMPANY_NAME, pay_company_master.PF_REG_NO, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_company_master.comp_code, pay_employee_master.JOINING_DATE, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.PF_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.PF_GROSS, pay_attendance.PF, pay_attendance.COMP_PF, pay_attendance.COMP_PF_PEN, pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PF_REG_OFFICE,  pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_employee_master.BIRTH_DATE,pay_attendance.CPF_SHEET,pay_attendance.C_HEAD01,pay_attendance.C_HEAD02,pay_attendance.C_HEAD03,pay_attendance.C_HEAD04,pay_attendance.C_HEAD05,pay_attendance.C_HEAD06,pay_attendance.C_HEAD07,pay_attendance.C_HEAD08,pay_attendance.C_HEAD09,pay_attendance.L_HEAD01,pay_attendance.L_HEAD02,pay_attendance.L_HEAD03,pay_attendance.L_HEAD04,pay_attendance.L_HEAD05,pay_attendance.OT_GROSS,pay_attendance.OT_HRS,pay_employee_master.PAN_NUMBER FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code   WHERE pay_attendance.PF>0 and pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY  pay_employee_master.PF_NUMBER, pay_employee_master.EMP_CODE asc";
        }
        else
        {
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);
            query = "SELECT  pay_company_master.COMPANY_NAME, pay_company_master.PF_REG_NO, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_company_master.comp_code, pay_employee_master.JOINING_DATE, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.PF_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.PF_GROSS, pay_attendance.PF, pay_attendance.COMP_PF, pay_attendance.COMP_PF_PEN, pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PF_REG_OFFICE,  pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_employee_master.BIRTH_DATE,pay_attendance.CPF_SHEET,pay_attendance.C_HEAD01,pay_attendance.C_HEAD02,pay_attendance.C_HEAD03,pay_attendance.C_HEAD04,pay_attendance.C_HEAD05,pay_attendance.C_HEAD06,pay_attendance.C_HEAD07,pay_attendance.C_HEAD08,pay_attendance.C_HEAD09,pay_attendance.L_HEAD01,pay_attendance.L_HEAD02,pay_attendance.L_HEAD03,pay_attendance.L_HEAD04,pay_attendance.L_HEAD05,pay_attendance.OT_GROSS,pay_attendance.OT_HRS,pay_employee_master.PAN_NUMBER FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code   WHERE pay_attendance.PF>0 and pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.UNIT_CODE in (" + UnitList + ") AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY  pay_employee_master.PF_NUMBER, pay_employee_master.EMP_CODE asc";
        }
        Session["ReportMonthNo"] = "01";
        ReportLoad(query, dowmloadname);
        //enable_disable();


    }


    private void enable_disable()
    {
        UnitGridView.Visible = false;
        btn_kyc_Download.Visible = false;

    }
    protected void btnesicstatement_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnesicstatement.BackColor = System.Drawing.Color.GreenYellow;
        //string downloadname = "ESIC_Statement";
        //string query = null;
        //crystalReport.Load(Server.MapPath("~/Rpt_Monthlyesic.rpt"));
        //DateTime dt = Convert.ToDateTime(txttodate.Text);
        //string thisMonth = dt.ToString("MMMM");
        //crystalReport.DataDefinition.FormulaFields["current_month"].Text = @"'" + thisMonth + "'";
        //crystalReport.DataDefinition.FormulaFields["current_year"].Text = @"'" + txttodate.Text.Substring(3) + "'";

        //if (ddlunitselect.Text == "ALL")
        //{
        //    //SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ESIC_REG_NO, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS, pay_attendance.ESIC_GROSS, pay_attendance.OT_ESIC_GROSS, pay_attendance.ESIC, pay_attendance.ESIC_OT, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_unit_master.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.PIN, pay_company_master.COMPANY_AUTHORISED_NAME, pay_company_master.CURRENT_MONTH, pay_company_master.CURRENT_YEAR, pay_employee_master.PF_SHEET, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI FROM            pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE 
        //    query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ESIC_REG_NO, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS, pay_attendance.ESIC_GROSS, pay_attendance.OT_ESIC_GROSS, pay_attendance.ESIC, pay_attendance.ESIC_OT, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_unit_master.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_employee_master.PF_SHEET, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI,pay_attendance.CPF_SHEET FROM            pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE   WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.ESIC_TOT>0 AND  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY pay_employee_master.ESIC_NUMBER ASC";
        //}
        //else
        //{
        //    // query = "SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ESIC_REG_NO, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS, pay_attendance.OT_ESIC_GROSS, pay_attendance.ESIC, pay_attendance.ESIC_OT, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_unit_master.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.PIN, pay_company_master.COMPANY_AUTHORISED_NAME, pay_company_master.CURRENT_MONTH, pay_company_master.CURRENT_YEAR, pay_employee_master.PF_SHEET, pay_attendance.ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI,pay_attendance.CPF_SHEET FROM            pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.ESIC_TOT>0 AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_unit_master.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' ORDER BY  pay_employee_master.ESIC_NUMBER ASC";
        //    query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ESIC_REG_NO, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS, pay_attendance.ESIC_GROSS, pay_attendance.OT_ESIC_GROSS, pay_attendance.ESIC, pay_attendance.ESIC_OT, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_unit_master.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY,  pay_employee_master.PF_SHEET, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI,pay_attendance.CPF_SHEET FROM            pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.ESIC_TOT>0 AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_unit_master.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY pay_employee_master.ESIC_NUMBER ASC";
        //}
        //Session["ReportMonthNo"] = "02";
        //ReportLoad(query, downloadname);
        //enable_disable();


        d.con1.Open();
        string query = "";
        string squery = "";
        //int length = ddl_unitcode.SelectedValue.Length;
        string strallexcelout = Session["AllExcelOut"].ToString();

        //  query = "SELECT  pay_attendance.EMP_CODE, pay_employee_master.EMP_NAME, pay_attendance.CGRADE_CODE AS  GRADE_CODE , pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS,  pay_attendance.C_HEAD01 AS '" + ehead01.ToString() + "', pay_attendance.C_HEAD02 AS '" + ehead02.ToString() + "', pay_attendance.C_HEAD03 AS '" + ehead03.ToString() + "', pay_attendance.C_HEAD04 AS '" + ehead04.ToString() + "', pay_attendance.C_HEAD05 AS '" + ehead05.ToString() + "', pay_attendance.C_HEAD06 AS '" + ehead06.ToString() + "', pay_attendance.C_HEAD07 AS '" + ehead07.ToString() + "',pay_attendance.C_HEAD08 AS '" + ehead08.ToString() + "',pay_attendance.C_HEAD09 AS '" + ehead09.ToString() + "',pay_attendance.C_HEAD10 AS '" + ehead10.ToString() + "' , pay_attendance.C_HEAD11 AS '" + ehead11.ToString() + "' , pay_attendance.C_HEAD12 AS '" + ehead12.ToString() + "' ,pay_attendance.OT_GROSS,pay_attendance.L_HEAD01 AS  '" + lhead01 + "',pay_attendance.L_HEAD02 AS  '" + lhead02 + "' ,pay_attendance.L_HEAD03 AS  '" + lhead03 + "',pay_attendance.L_HEAD04 AS  '" + lhead04 + "',(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.OT_GROSS+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04) AS GROSS, pay_attendance.PF, pay_attendance.PTAX,pay_attendance.ESIC_TOT,MLWF AS LWF,(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX) AS DEDUCT,round(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS-pay_attendance.PF- pay_attendance.PTAX-pay_attendance.ESIC_TOT-pay_attendance.MLWF-(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX),0) AS NETTAMOUNT ,EMP_FATHER_NAME AS FATHER,(CASE WHEN (EMP_FATHER_NAME='' OR EMP_FATHER_NAME IS NULL) THEN '****' ELSE '' END )AS SIGNATURE, pay_employee_master.PF_BANK_NAME,pay_employee_master.PF_IFSC_CODE,pay_employee_master.BANK_EMP_AC_CODE FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN  pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE( pay_attendance.PRESENT_DAYS>0  OR  pay_attendance.OT_HRS>0) AND  (pay_attendance.comp_code = '" + Session["comp_code"].ToString() + "') AND (pay_attendance.UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "')  AND pay_attendance.MONTH= '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR='" + txttodate.Text.Substring(3, 4) + "' ORDER BY pay_employee_master.EMP_CODE";

        //query = "SELECT  pay_attendance.EMP_CODE, pay_employee_master.EMP_NAME,case pay_employee_master.GENDER when 'M' then 'MALE' when 'F' then 'FEMALE' end as 'GENDER' , pay_attendance.CGRADE_CODE AS  GRADE_CODE ,pay_department_master.DEPT_NAME , pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS,  pay_attendance.C_HEAD01 AS '" + ehead01.ToString() + "', pay_attendance.C_HEAD02 AS '" + ehead02.ToString() + "', pay_attendance.C_HEAD03 AS '" + ehead03.ToString() + "', pay_attendance.C_HEAD04 AS '" + ehead04.ToString() + "', pay_attendance.C_HEAD05 AS '" + ehead05.ToString() + "', pay_attendance.C_HEAD06 AS '" + ehead06.ToString() + "', pay_attendance.C_HEAD07 AS '" + ehead07.ToString() + "',pay_attendance.C_HEAD08 AS '" + ehead08.ToString() + "',pay_attendance.C_HEAD09 AS '" + ehead09.ToString() + "',pay_attendance.C_HEAD10 AS '" + ehead10.ToString() + "' , pay_attendance.C_HEAD11 AS '" + ehead11.ToString() + "' , pay_attendance.C_HEAD12 AS '" + ehead12.ToString() + "' ,pay_attendance.OT_GROSS,pay_attendance.L_HEAD01 AS  '" + lhead01 + "',pay_attendance.L_HEAD02 AS  '" + lhead02 + "' ,pay_attendance.L_HEAD03 AS  '" + lhead03 + "',pay_attendance.L_HEAD04 AS  '" + lhead04 + "',(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.OT_GROSS+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04) AS GROSS, pay_attendance.PF, pay_attendance.PTAX,pay_attendance.ESIC_TOT,MLWF AS LWF,(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX) AS DEDUCT,round(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS-pay_attendance.PF- pay_attendance.PTAX-pay_attendance.ESIC_TOT-pay_attendance.MLWF-(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX),0) AS NETTAMOUNT ,EMP_FATHER_NAME AS FATHER,(CASE WHEN (EMP_FATHER_NAME='' OR EMP_FATHER_NAME IS NULL) THEN '****' ELSE '' END )AS SIGNATURE, pay_employee_master.PF_BANK_NAME,pay_employee_master.PF_IFSC_CODE,pay_employee_master.BANK_EMP_AC_CODE FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN  pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code inner join pay_department_master on pay_employee_master.DEPT_CODE=pay_department_master.DEPT_CODE AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE( pay_attendance.PRESENT_DAYS>0  OR  pay_attendance.OT_HRS>0) AND  (pay_attendance.comp_code = '" + Session["comp_code"].ToString() + "') AND (pay_attendance.UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "')  AND pay_attendance.MONTH= '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR='" + txttodate.Text.Substring(3, 4) + "' ORDER BY pay_employee_master.EMP_CODE";
        query = "SELECT pay_client_master.CLIENT_NAME, pay_unit_master.state_name,concat (pay_client_master.CLIENT_NAME, '-' ,pay_unit_master.state_name ) as ZONE,pay_employee_master.GRADE_CODE,pay_company_master.ESIC_REG_NO,pay_employee_master.EMP_NAME, pay_attendance.TOT_ESIC_GROSS,pay_attendance.PRESENT_DAYS,pay_attendance.PAYABLE_DAYS,pay_attendance.ESIC_TOT,pay_attendance.ESIC_COMP_CONTRI, ( pay_attendance.ESIC_TOT +pay_attendance.ESIC_COMP_CONTRI) as ESIC FROM pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_client_master ON pay_attendance.comp_code = pay_client_master.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.ESIC_TOT>0 AND  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY pay_employee_master.ESIC_NUMBER ASC";
        //query = "SELECT pay_client_master.CLIENT_NAME, pay_unit_master.state_name, pay_unit_master.ZONE,pay_employee_master.GRADE_CODE,pay_company_master.ESIC_REG_NO,pay_employee_master.EMP_NAME, pay_attendance.TOT_ESIC_GROSS,pay_attendance.PRESENT_DAYS as WORKING_DAYS,pay_attendance.PAYABLE_DAYS AS CALCULATION_OF_WOEKING_DAYS,pay_attendance.ESIC_TOT AS EMPLOYRR_E_CONTRIBUTION,pay_attendance.ESIC_COMP_CONTRI AS EMPLOYEE_R_CONTRIBUTION, ( pay_attendance.ESIC_TOT +pay_attendance.ESIC_COMP_CONTRI) as TOTAL FROM pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_client_master ON pay_attendance.comp_code = pay_client_master.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.ESIC_TOT>0 AND  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY pay_employee_master.ESIC_NUMBER ASC";

        MySqlCommand cmd = new MySqlCommand(query, d.con1);
        DataSet ds = new DataSet();
        MySqlDataAdapter adp = new MySqlDataAdapter(query, d.con1);
        adp.Fill(ds);
        gv_esic_statement.DataSource = ds.Tables[0];
        gv_esic_statement.DataBind();
        d.con1.Close();
        panel_esic_statement.Visible = true;
        panel_employee_information_status.Visible = false;
        panel_employee_pf_esic_no.Visible = false;
        panel_esic_summary_utwise.Visible = false;
        panel_esic_summary_utwise.Visible = false;


        panel_ptax.Visible = false;
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }

    }
    protected void btnptaxstatement_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnptaxstatement.BackColor = System.Drawing.Color.GreenYellow;
        string downloadname = "PTAX_Statement";
        string query = null;
        crystalReport.Load(Server.MapPath("~/Rpt_Monthlyptax.rpt"));
        DateTime dt = Convert.ToDateTime(txttodate.Text);
        string thisMonth = dt.ToString("MMMM");
        crystalReport.DataDefinition.FormulaFields["current_month"].Text = @"'" + thisMonth + "'";
        crystalReport.DataDefinition.FormulaFields["current_year"].Text = @"'" + txttodate.Text.Substring(3) + "'";

        if (ddlunitselect.Text == "ALL")
        {
            //SELECT pay_company_master.COMPANY_NAME, pay_company_master.comp_code, pay_attendance.PT_GROSS, pay_attendance.PTAX, pay_unit_master.UNIT_NAME, pay_unit_master.STATE_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.PIN, pay_company_master.STATE, pay_company_master.CURRENT_MONTH, pay_company_master.CURRENT_YEAR, pay_attendance.EMP_CODE, pay_employee_master.PF_SHEET, pay_attendance.UNIT_CODE FROM  pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE 
            query = "  SELECT pay_company_master.COMPANY_NAME, pay_company_master.comp_code, pay_attendance.PT_GROSS, pay_attendance.PTAX, pay_unit_master.UNIT_NAME, pay_unit_master.STATE_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY,  pay_company_master.STATE,  pay_attendance.EMP_CODE, pay_employee_master.PF_SHEET, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM  pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";
        }
        else
        {

            query = "  SELECT pay_company_master.COMPANY_NAME, pay_company_master.comp_code, pay_attendance.PT_GROSS, pay_attendance.PTAX, pay_unit_master.UNIT_NAME, pay_unit_master.STATE_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY,  pay_company_master.STATE,  pay_attendance.EMP_CODE, pay_employee_master.PF_SHEET, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM  pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";
        }
        Session["ReportMonthNo"] = "03";
        ReportLoad(query, downloadname);
        enable_disable();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnpfchallan_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        ButtonColor();
        btnpfchallan.BackColor = System.Drawing.Color.GreenYellow;
        string downloadname = "PF_Challan";
        string query = null;
        String currMonth = Session["CURRENT_MONTH"].ToString();
        String currYear = Session["CURRENT_YEAR"].ToString();

        if (ddlunitselect.Text == "ALL")
        {
            crystalReport.Load(Server.MapPath("~/Rpt_Mon_Pfchallan_all.rpt"));
            query = "  SELECT  pay_company_master.COMPANY_NAME, pay_company_master.PF_REG_NO, '" + txttodate.Text.Substring(0, 2) + "' as 'CURRENT_MONTH',  '" + txttodate.Text.Substring(3) + "' as 'CURRENT_YEAR',  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_company_master.comp_code, pay_employee_master.JOINING_DATE, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.PF_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.PF_GROSS, pay_attendance.PF, pay_attendance.COMP_PF, pay_attendance.COMP_PF_PEN, pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_OFFICE,  pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_employee_master.BIRTH_DATE,pay_attendance.CPF_SHEET FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code   WHERE pay_attendance.PF>0 and pay_attendance.CPF_SHEET='Yes' AND   pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";
        }
        else
        {
            crystalReport.Load(Server.MapPath("~/Rpt_Mon_Pfchallan.rpt"));
            // query = "  SELECT  pay_company_master.COMPANY_NAME, pay_company_master.PF_REG_NO, pay_company_master.CURRENT_MONTH,  pay_company_master.CURRENT_YEAR, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_company_master.comp_code, pay_employee_master.JOINING_DATE, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.PF_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.PF_GROSS, pay_attendance.PF, pay_attendance.COMP_PF, pay_attendance.COMP_PF_PEN, pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PIN, pay_company_master.PF_REG_OFFICE, pay_company_master.COMPANY_AUTHORISED_NAME, pay_company_master.COMPANY_AUTHORISED_DESIGNATION, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_employee_master.BIRTH_DATE,pay_attendance.CPF_SHEET FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code      WHERE pay_attendance.PF>0 and pay_attendance.CPF_SHEET='Yes' AND  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";

            query = "  SELECT  pay_company_master.COMPANY_NAME, pay_company_master.PF_REG_NO, '" + txttodate.Text.Substring(0, 2) + "' as 'CURRENT_MONTH',  '" + txttodate.Text.Substring(3) + "' as 'CURRENT_YEAR',  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_company_master.comp_code, pay_employee_master.JOINING_DATE, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.PF_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.PF_GROSS, pay_attendance.PF, pay_attendance.COMP_PF, pay_attendance.COMP_PF_PEN, pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_OFFICE,pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_employee_master.BIRTH_DATE,pay_attendance.CPF_SHEET FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code  WHERE pay_attendance.PF>0 and pay_attendance.CPF_SHEET='Yes' AND  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";
        }
        Session["ReportMonthNo"] = "04";
        ReportLoad(query, downloadname);
        enable_disable();
    }

    protected void btnesicchallan_Click(object sender, EventArgs e)
    {

        ButtonColor();
        btnesicchallan.BackColor = System.Drawing.Color.GreenYellow;
        string downloadname = "ESIC_Challan";
        string query = null;
        crystalReport.Load(Server.MapPath("~/Rpt_Mon_Esicchallan.rpt"));
        if (ddlunitselect.Text == "ALL")
        {

            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ESIC_REG_NO, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS, pay_attendance.ESIC_GROSS, pay_attendance.OT_ESIC_GROSS, pay_attendance.ESIC, pay_attendance.ESIC_OT, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_unit_master.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY,  pay_employee_master.PF_SHEET, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI,pay_attendance.CPF_SHEET FROM            pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE   WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.ESIC_TOT>0 AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";
        }
        else
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ESIC_REG_NO, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS, pay_attendance.ESIC_GROSS, pay_attendance.OT_ESIC_GROSS, pay_attendance.ESIC, pay_attendance.ESIC_OT, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_unit_master.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_employee_master.PF_SHEET, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI,pay_attendance.CPF_SHEET FROM            pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE   WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.ESIC_TOT>0 AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_unit_master.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";
        }
        Session["ReportMonthNo"] = "06";
        ReportLoad(query, downloadname);
        enable_disable();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnformno10_Click(object sender, EventArgs e)
    {

        if (txttodate.Text != "")
        {

            ButtonColor();
            btnformno10.BackColor = System.Drawing.Color.GreenYellow;
            string downloadname = "Form_No.10";
            string query = null;
            crystalReport.Load(Server.MapPath("~/Rpt_month_Form10.rpt"));

            //crystalReport.SummaryInfo.ReportTitle = "For Period from date '" + DateTime.Parse(txtfromdate.Text).ToString("dd/MM/yyyy") + "' to date '" + DateTime.Parse(txttodate.Text).ToString("dd/MM/yyyy") + "'";

            if (ddlunitselect.Text == "ALL")
            {
                query = "SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.BIRTH_DATE, pay_employee_master.JOINING_DATE, pay_employee_master.CONFIRMATION_DATE, pay_employee_master.LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME,  '' As BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE,  pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 , pay_company_master.ADDRESS1,pay_company_master.ADDRESS2,pay_company_master.CITY,pay_company_master.STATE FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'";// AND  MONTH(pay_employee_master.LEFT_DATE) BETWEEN '" + txttodate.Text.Substring(0, 2) + "' AND '" + txttodate.Text.Substring(0, 2) + "' ";
            }
            else
            {
                query = "SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.BIRTH_DATE, pay_employee_master.JOINING_DATE, pay_employee_master.CONFIRMATION_DATE, pay_employee_master.LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, '' AS BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 , pay_company_master.ADDRESS1,pay_company_master.ADDRESS2,pay_company_master.CITY,pay_company_master.STATE  FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE  WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_employee_master.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "'  ";

            }

            Session["ReportMonthNo"] = "08";
            ReportLoad(query, downloadname);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please enter from date and  to date period for this report ')", true);
            ButtonColor();
            //CrystalReportViewer1.ReportSource = null;
            //CrystalReportViewer1.RefreshReport();
            //Session["ReportMonthNo"] = "";
            //crystalReport.Dispose();

            // Chk_date_enable.Checked = true;
            //Lblfromdate.Visible = true;
            Lbltodate.Visible = true;
            //txtfromdate.Visible = true;
            txttodate.Visible = true;
        }
        enable_disable();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnformno5_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }

        if (txttodate.Text != "")
        {

            ButtonColor();
            btnformno5.BackColor = System.Drawing.Color.GreenYellow;
            string downloadname = "Form_No.5";
            string query = null;
            crystalReport.Load(Server.MapPath("~/Rpt_month_Form5.rpt"));
            //crystalReport.SummaryInfo.ReportTitle = "For Period from date '" + DateTime.Parse(txtfromdate.Text).ToString("dd/MM/yyyy") + "' to date '" + DateTime.Parse(txttodate.Text).ToString("dd/MM/yyyy") + "'";
            int from_month = Convert.ToInt16(txttodate.Text.Substring(0, 2)) - 1;

            if (ddlunitselect.Text == "ALL")
            {
                query = "SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.BIRTH_DATE, pay_employee_master.JOINING_DATE, pay_employee_master.CONFIRMATION_DATE, pay_employee_master.LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME,  '' As BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2,pay_company_master.ADDRESS1,pay_company_master.ADDRESS2,pay_company_master.CITY,pay_company_master.STATE FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE  WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND  MONTH(pay_employee_master.JOINING_DATE) BETWEEN '" + from_month + "' AND '" + txttodate.Text.Substring(0, 2) + "' AND YEAR(pay_employee_master.JOINING_DATE) = '" + txttodate.Text.Substring(3) + "'";
            }
            else
            {
                query = "SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.BIRTH_DATE, pay_employee_master.JOINING_DATE, pay_employee_master.CONFIRMATION_DATE, pay_employee_master.LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, '' As BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2, pay_company_master.ADDRESS1,pay_company_master.ADDRESS2,pay_company_master.CITY,pay_company_master.STATE FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_employee_master.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "'  AND  MONTH(pay_employee_master.JOINING_DATE) BETWEEN '" + from_month + "' AND '" + txttodate.Text.Substring(0, 2) + "' AND YEAR(pay_employee_master.JOINING_DATE) = '" + txttodate.Text.Substring(3) + "'";

            }

            Session["ReportMonthNo"] = "09";
            ReportLoad(query, downloadname);
        }
        else
        {

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please enter from date and  to date period for this report ')", true);
            ButtonColor();
            //CrystalReportViewer1.ReportSource = null;
            //CrystalReportViewer1.RefreshReport();
            //Session["ReportMonthNo"] = "";
            //crystalReport.Dispose();

            //Chk_date_enable.Checked = true;
            //Lblfromdate.Visible = true;
            Lbltodate.Visible = true;
            //txtfromdate.Visible = true;
            txttodate.Visible = true;
        }
        enable_disable();
    }
    protected void btnformno12_Click(object sender, EventArgs e)
    {
        ButtonColor();
    }
    protected void btnmlwfsttement_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnmlwfsttement.BackColor = System.Drawing.Color.GreenYellow;
        string query = null;
        string downloadname = " MLWF_Statement";
        crystalReport.Load(Server.MapPath("~/MonthlyMLWF_unitsummary.rpt"));
        DateTime dt = Convert.ToDateTime(txttodate.Text);
        string thisMonth = dt.ToString("MMMM");
        crystalReport.DataDefinition.FormulaFields["current_month"].Text = @"'" + thisMonth + "'";
        crystalReport.DataDefinition.FormulaFields["current_year"].Text = @"'" + txttodate.Text.Substring(3) + "'";


        if (ddlunitselect.Text == "ALL")
        {
            //            SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PIN, pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05, pay_company_master.CURRENT_MONTH, pay_company_master.CURRENT_YEAR, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code 
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.MLWF>0 AND   pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'  ORDER BY pay_unit_master.UNIT_NAME ";
        }
        else
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.MLWF>0 AND     pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'  AND pay_attendance.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY UNIT_NAME ";

        }
        Session["ReportMonthNo"] = "10";
        ReportLoad(query, downloadname);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }

    }
    protected void btnmlwfstatementdetails_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnmlwfstatementdetails.BackColor = System.Drawing.Color.GreenYellow;
        string query = null;
        string downloadname = "MLWF_Statement_Details";
        crystalReport.Load(Server.MapPath("~/MonthlyMLWF.rpt"));
        DateTime dt = Convert.ToDateTime(txttodate.Text);
        string thisMonth = dt.ToString("MMMM");
        crystalReport.DataDefinition.FormulaFields["current_month"].Text = @"'" + thisMonth + "'";
        crystalReport.DataDefinition.FormulaFields["current_year"].Text = @"'" + txttodate.Text.Substring(3) + "'";


        if (ddlunitselect.Text == "ALL")
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.MLWF>0 AND   pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'  ORDER BY pay_unit_master.UNIT_NAME,EMP_NAME ";
        }
        else
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.CPF_SHEET='Yes'  AND pay_attendance.MLWF>0 AND pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'  AND pay_attendance.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY UNIT_NAME,EMP_NAME ";

        }
        Session["ReportMonthNo"] = "11";
        ReportLoad(query, downloadname);

        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    protected void btnsalarystaeemntbyunit_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnsalarystaeemntbyunit.BackColor = System.Drawing.Color.GreenYellow;
        string downloadname = "Salary_Statement_Unit_wise";
        string query = null;
        crystalReport.Load(Server.MapPath("~/MonthlySalaryUnitSummary.rpt"));
        DateTime dt = Convert.ToDateTime(txttodate.Text);
        string thisMonth = dt.ToString("MMMM");
        crystalReport.DataDefinition.FormulaFields["current_month"].Text = @"'" + thisMonth + "'";
        crystalReport.DataDefinition.FormulaFields["current_year"].Text = @"'" + txttodate.Text.Substring(3) + "'";

        if (ddlunitselect.Text == "ALL")
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') AS JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE ,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE  pay_attendance.PRESENT_DAYS>0  AND  pay_attendance.CPF_SHEET='Yes' AND  pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ";

        }
        else
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') AS JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE ,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE  pay_attendance.PRESENT_DAYS>0 AND   pay_attendance.CPF_SHEET='Yes' AND pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'  AND pay_attendance.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";

        }
        Session["ReportMonthNo"] = "13";
        ReportLoad(query, downloadname);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }




    //protected void btnunitsummary_Click(object sender, EventArgs e)
    //{


    //}
    protected void btnbankwisesalarystamt_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnbankwisesalarystamt.BackColor = System.Drawing.Color.Lavender;
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnotstatement_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnotstatement.BackColor = System.Drawing.Color.Lavender;
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnempgratutystatement_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnempgratutystatement.BackColor = System.Drawing.Color.Lavender;
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnleavestatus_Click(object sender, EventArgs e)
    {
        ButtonColor();
        // btnleavestatus.BackColor = System.Drawing.Color.Lavender;
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnemployeeinfostatust_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        ButtonColor();
        btnemployeeinfostatust.BackColor = System.Drawing.Color.GreenYellow;

        d.con1.Open();
        string query = "";
        string squery = "";
        //int length = ddl_unitcode.SelectedValue.Length;
        string strallexcelout = Session["AllExcelOut"].ToString();
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            query = "SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.BIRTH_DATE,'%d/%m/%Y') As BIRTH_DATE, Case When DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') else '' END As JOINING_DATE , Case When DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') else '' END As CONFIRMATION_DATE, Case When DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') else '' END As LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, pay_bank_master.BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE INNER JOIN pay_bank_master ON pay_employee_master.comp_code = pay_bank_master.comp_code   WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'";
        }
        else
        {
            query = "SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.BIRTH_DATE,'%d/%m/%Y') As BIRTH_DATE, Case When DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') else '' END As JOINING_DATE , Case When DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') else '' END As CONFIRMATION_DATE, Case When DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') else '' END As LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, pay_bank_master.BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE INNER JOIN pay_bank_master ON pay_employee_master.comp_code = pay_bank_master.comp_code     WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_employee_master.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' ";

        }
        MySqlCommand cmd = new MySqlCommand(query, d.con1);
        DataSet ds = new DataSet();
        MySqlDataAdapter adp = new MySqlDataAdapter(query, d.con1);
        adp.Fill(ds);
        gv_employee_information_status.DataSource = ds.Tables[0];
        gv_employee_information_status.DataBind();
        d.con1.Close();
        panel_employee_information_status.Visible = true;
        panel_esic_statement.Visible = false;
        panel_employee_pf_esic_no.Visible = false;
        panel_ptax.Visible = false;
        panel_esic_summary_utwise.Visible = false;


        //ButtonColor();
        //btnemployeeinfostatust.BackColor = System.Drawing.Color.GreenYellow;
        //string downloadname = "Employee_Information_Status";
        //string query = null;
        //crystalReport.Load(Server.MapPath("~/Rpt_EmployeeInfo.rpt"));

        //if (ddlunitselect.Text == "ALL")
        //{
        //    query = "SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.BIRTH_DATE,'%d/%m/%Y') As BIRTH_DATE, Case When DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') else '' END As JOINING_DATE , Case When DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') else '' END As CONFIRMATION_DATE, Case When DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') else '' END As LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, pay_bank_master.BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE INNER JOIN pay_bank_master ON pay_employee_master.comp_code = pay_bank_master.comp_code   WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'";
        //}
        //else
        //{
        //    query = "SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.BIRTH_DATE,'%d/%m/%Y') As BIRTH_DATE, Case When DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') else '' END As JOINING_DATE , Case When DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') else '' END As CONFIRMATION_DATE, Case When DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') else '' END As LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, pay_bank_master.BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE INNER JOIN pay_bank_master ON pay_employee_master.comp_code = pay_bank_master.comp_code     WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_employee_master.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' ";

        //}

        //Session["ReportMonthNo"] = "19";
        //ReportLoad(query, downloadname);




    }
    protected void btnemployeeinfostatus2_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        ButtonColor();
        btnemployeeinfostatus2.BackColor = System.Drawing.Color.GreenYellow;

        d.con1.Open();
        string query = "";
        string squery = "";
        //int length = ddl_unitcode.SelectedValue.Length;
        string strallexcelout = Session["AllExcelOut"].ToString();
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            query = "    SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.BIRTH_DATE,'%d/%m/%Y') As BIRTH_DATE, Case When DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') else '' END As JOINING_DATE , Case When DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') else '' END As CONFIRMATION_DATE, Case When DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') else '' END As LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, pay_bank_master.BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE INNER JOIN pay_bank_master ON pay_employee_master.comp_code = pay_bank_master.comp_code WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'";
        }
        else
        {
            query = "    SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.BIRTH_DATE,'%d/%m/%Y') As BIRTH_DATE, Case When DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') else '' END As JOINING_DATE , Case When DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') else '' END As CONFIRMATION_DATE, Case When DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') else '' END As LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, pay_bank_master.BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE INNER JOIN pay_bank_master ON pay_employee_master.comp_code = pay_bank_master.comp_code  WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_employee_master.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' ";

        }
        MySqlCommand cmd = new MySqlCommand(query, d.con1);
        DataSet ds = new DataSet();
        MySqlDataAdapter adp = new MySqlDataAdapter(query, d.con1);
        adp.Fill(ds);
        gv_employee_pf_esic_no.DataSource = ds.Tables[0];
        gv_employee_pf_esic_no.DataBind();
        d.con1.Close();
        panel_employee_pf_esic_no.Visible = true;
        panel_esic_statement.Visible = false;
        panel_employee_information_status.Visible = false;
        panel_ptax.Visible = false;
        panel_esic_summary_utwise.Visible = false;



        //ButtonColor();
        //btnemployeeinfostatus2.BackColor = System.Drawing.Color.GreenYellow;
        //string downloadname = "Employee_PF_and_ESIC_Number";
        //string query = null;
        //crystalReport.Load(Server.MapPath("~/Rpt_EmployeeInfopfesic.rpt"));
        //if (ddlunitselect.Text == "ALL")
        //{
        //    query = "    SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.BIRTH_DATE,'%d/%m/%Y') As BIRTH_DATE, Case When DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') else '' END As JOINING_DATE , Case When DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') else '' END As CONFIRMATION_DATE, Case When DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') else '' END As LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, pay_bank_master.BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE INNER JOIN pay_bank_master ON pay_employee_master.comp_code = pay_bank_master.comp_code WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'";
        //}
        //else
        //{
        //    query = "    SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.BIRTH_DATE,'%d/%m/%Y') As BIRTH_DATE, Case When DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') else '' END As JOINING_DATE , Case When DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.CONFIRMATION_DATE,'%d/%m/%Y') else '' END As CONFIRMATION_DATE, Case When DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') <> '' then DATE_FORMAT(pay_employee_master.LEFT_DATE,'%d/%m/%Y') else '' END As LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, pay_bank_master.BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE INNER JOIN pay_bank_master ON pay_employee_master.comp_code = pay_bank_master.comp_code  WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_employee_master.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' ";

        //}
        //Session["ReportMonthNo"] = "20";
        //ReportLoad(query, downloadname);


    }
    protected void btnempinfostatus3_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        ButtonColor();
        btnempinfostatus3.BackColor = System.Drawing.Color.Lavender;
        string downloadname = "DEDUCTION_REPORT";
        String query = "";
        crystalReport.Load(Server.MapPath("~/Rpt_EmployeeInfopfesic.rpt"));
        if (ddlunitselect.Text == "ALL")
        {
            query = "SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.BIRTH_DATE, pay_employee_master.JOINING_DATE, pay_employee_master.CONFIRMATION_DATE, pay_employee_master.LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, pay_bank_master.BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE INNER JOIN pay_bank_master ON pay_employee_master.comp_code = pay_bank_master.comp_code WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'";
        }
        else
        {
            query = "SELECT  pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_employee_master.EMP_CODE,  pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.BIRTH_DATE, pay_employee_master.JOINING_DATE, pay_employee_master.CONFIRMATION_DATE, pay_employee_master.LEFT_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.EMP_MOBILE_NO, pay_employee_master.EMP_RELIGION, pay_employee_master.PAN_NUMBER, pay_employee_master.PF_DEDUCTION_FLAG, pay_employee_master.PF_NUMBER, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_employee_master.ADHARNO, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.E_HEAD13, pay_employee_master.E_HEAD14, pay_employee_master.E_HEAD15, pay_employee_master.LEFT_REASON, pay_employee_master.EARN_TOTAL, pay_employee_master.PF_SHEET, pay_unit_master.UNIT_NAME, pay_grade_master.GRADE_DESC, pay_department_master.DEPT_NAME, pay_bank_master.BANK_NAME, pay_company_master.E_HEAD01 AS HEAD1, pay_company_master.E_HEAD02 AS HEAD2, pay_company_master.E_HEAD03 AS HEAD3, pay_company_master.E_HEAD04 AS HEAD4, pay_company_master.E_HEAD05 AS HEAD5, pay_company_master.E_HEAD06 AS HEAD6, pay_company_master.E_HEAD07 AS HEAD7, pay_company_master.E_HEAD08 AS HEAD8, pay_company_master.E_HEAD09 AS HEAD9, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.E_HEAD13 AS HEAD13, pay_company_master.E_HEAD14 AS HEAD14, pay_company_master.E_HEAD15 AS HEAD15, pay_employee_master.EMP_CURRENT_ADDRESS, pay_employee_master.EMP_CURRENT_CITY, pay_employee_master.EMP_CURRENT_STATE, pay_employee_master.EMP_CURRENT_PIN, pay_employee_master.REFNAME1, pay_employee_master.REFMOBILE1, pay_employee_master.REFNAME2, pay_employee_master.REFMOBILE2 FROM pay_company_master INNER JOIN pay_employee_master ON pay_company_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND  pay_employee_master.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND  pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_department_master ON pay_employee_master.comp_code = pay_department_master.comp_code AND  pay_employee_master.DEPT_CODE = pay_department_master.DEPT_CODE INNER JOIN pay_bank_master ON pay_employee_master.comp_code = pay_bank_master.comp_code WHERE pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_employee_master.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' ";

        }
        Session["ReportMonthNo"] = "21";
        ReportLoad(query, downloadname);




    }
    protected void btnemployeemusterroll2_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnemployeemusterroll2.BackColor = System.Drawing.Color.Lavender;
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnpayslipyr_Click(object sender, EventArgs e)
    {
        ButtonColor();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnsalarystmtyr_Click(object sender, EventArgs e)
    {
        ButtonColor();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnunitsummaryyr_Click(object sender, EventArgs e)
    {
        ButtonColor();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnbankwissalstmtyr_Click(object sender, EventArgs e)
    {
        ButtonColor();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }

    }
    protected void btnotstmtyr_Click(object sender, EventArgs e)
    {
        ButtonColor();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnpfstmtyr_Click(object sender, EventArgs e)
    {
        ButtonColor();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnesicstmtyr_Click(object sender, EventArgs e)
    {
        ButtonColor();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnpatxstmtyr_Click(object sender, EventArgs e)
    {
        ButtonColor();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnpfchallanyr_Click(object sender, EventArgs e)
    {
        ButtonColor();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnunitwisesalarysummary_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btncustomerwisesalarysummary_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    protected void btnsalarystatement_Click(object sender, EventArgs e)
    {
        string curr_date = Session["system_curr_date"].ToString();
        ButtonColor();
        btnsalarystatement.BackColor = System.Drawing.Color.GreenYellow;
        string downloadname = "Salary_Statement";
        string query = null;
        int length = ddlunitselect.SelectedValue.Length;

        crystalReport.Load(Server.MapPath("~/MonthlySalary_all_GTS.rpt"));
        crystalReport.DataDefinition.FormulaFields["current_month"].Text = @"'" + txttodate.Text.Substring(0, 2) + "'";
        crystalReport.DataDefinition.FormulaFields["current_year"].Text = @"'" + txttodate.Text.Substring(3) + "'";
        if (ddlunitselect.Text == "ALL")
        {
            query = "SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') AS JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX,pay_attendance.RESOURCE_SALARY, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.PAYABLE_DAYS>0 and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'  AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";
        }
        else
        {
            query = "  select pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, DATE_FORMAT(pay_employee_master.JOINING_DATE,'%d/%m/%Y') AS JOINING_DATE, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX,pay_attendance.RESOURCE_SALARY, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.PAYABLE_DAYS>0 AND pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";

        }
        Session["ReportMonthNo"] = "12";
        // Reportformat(query);
        ReportLoad(query, downloadname);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btnformno10_Click1(object sender, EventArgs e)
    {
        ButtonColor();
        btnformno10.BackColor = System.Drawing.Color.Lavender;
    }
    protected void btnformno5_Click1(object sender, EventArgs e)
    {
        ButtonColor();
        btnformno5.BackColor = System.Drawing.Color.Lavender;
    }
    protected void btnmlwfstatementdetails_Click1(object sender, EventArgs e)
    {
        ButtonColor();
        btnmlwfstatementdetails.BackColor = System.Drawing.Color.Lavender;
    }

    protected void Un_Load(object sender, EventArgs e)
    {
        crystalReport.Dispose();
        crystalReport.Close();
    }
    protected void btnpfchallanunitsumm_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnpfchallanunitsumm.BackColor = System.Drawing.Color.GreenYellow;
        string query = null;
        string downloadname = "PF_Challan_Unitwise";
        crystalReport.Load(Server.MapPath("~/Rpt_Mon_Pfchallan_unitsumm.rpt"));
        query = "   SELECT  pay_company_master.COMPANY_NAME, pay_company_master.PF_REG_NO,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_company_master.comp_code, pay_employee_master.JOINING_DATE, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.PF_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.PF_GROSS, pay_attendance.PF, pay_attendance.COMP_PF, pay_attendance.COMP_PF_PEN, pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_OFFICE,  pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_employee_master.BIRTH_DATE,pay_attendance.CPF_SHEET FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code        WHERE pay_attendance.PF>0 AND pay_attendance.CPF_SHEET='Yes' AND  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY pay_unit_master.UNIT_CODE ";

        Session["ReportMonthNo"] = "05";
        ReportLoad(query, downloadname);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    protected void btnesicchallanunit_Click(object sender, EventArgs e)
    {

        ButtonColor();
        btnesicchallanunit.BackColor = System.Drawing.Color.GreenYellow;
        //string downloadname = "Esic_Summary_Unitwise";
        string query = null;
        //crystalReport.Load(Server.MapPath("~/Rpt_monthesicchallan_unitwise.rpt"));
        //query = " SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ESIC_REG_NO, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.ESIC_DEDUCTION_FLAG, pay_employee_master.ESIC_NUMBER, pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS, pay_attendance.ESIC_GROSS, pay_attendance.OT_ESIC_GROSS, pay_attendance.ESIC, pay_attendance.ESIC_OT, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_unit_master.UNIT_CODE, pay_unit_master.UNIT_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY,  pay_employee_master.PF_SHEET, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI,pay_attendance.CPF_SHEET FROM            pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE   WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.ESIC<>0 AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";
        //Session["ReportMonthNo"] = "07";
        //ReportLoad(query, downloadname);
        d.con1.Open();
        query = "(SELECT pay_unit_master.UNIT_CODE as 'Unit Code', pay_unit_master.UNIT_NAME as 'Unit Name' , pay_employee_master.EMP_NAME 'Emp Name', pay_attendance.PRESENT_DAYS as 'Present Days' , pay_attendance.TOT_ESIC_GROSS  as 'ESIC Gross' , pay_attendance.ESIC_TOT as 'Employee_E_Contribution', pay_attendance.ESIC_COMP_CONTRI 'Employee_R_Contribution', (pay_attendance.ESIC_TOT + pay_attendance.ESIC_COMP_CONTRI) AS 'ESIC Total' FROM pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE pay_attendance.CPF_SHEET = 'Yes' AND pay_attendance.ESIC_TOT > 0 AND pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY pay_employee_master.ESIC_NUMBER ASC) UNION (SELECT  '' as 'Unit Code', '' as 'Unit Name', 'Total' as 'Emp Name' , SUM(pay_attendance.PRESENT_DAYS) as 'Present Days', SUM(pay_attendance.TOT_ESIC_GROSS)  as 'ESIC Gross' , SUM(pay_attendance.ESIC_TOT) as 'Employee_E_Contribution', SUM(pay_attendance.ESIC_COMP_CONTRI)'Employee_R_Contribution', SUM(pay_attendance.ESIC_TOT + pay_attendance.ESIC_COMP_CONTRI) AS 'ESIC Total' FROM pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE pay_attendance.CPF_SHEET = 'Yes' AND pay_attendance.ESIC_TOT > 0 AND pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY pay_employee_master.ESIC_NUMBER ASC)";

        DataSet ds = new DataSet();

        MySqlDataAdapter adp = new MySqlDataAdapter(query, d.con1);
        adp.Fill(ds);
        gv_esic_summary_utwise.DataSource = ds.Tables[0];
        gv_esic_summary_utwise.DataBind();
        panel_esic_summary_utwise.Visible = true;
        d.con1.Close();
        panel_esic_statement.Visible = false;
        panel_employee_information_status.Visible = false;
        panel_employee_pf_esic_no.Visible = false;
        panel_esic_summary_utwise.Visible = true;
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }


    //protected void Chk_date_enable_CheckedChanged(object sender, EventArgs e)
    //{
    //    if (Chk_date_enable.Checked == true)
    //    {
    //        Lblfromdate.Visible = true;
    //        Lbltodate.Visible = true;
    //        txtfromdate.Visible = true;
    //        txttodate.Visible = true;
    //    }
    //    else
    //    {
    //        Lblfromdate.Visible = false;
    //        Lbltodate.Visible = false;
    //        txtfromdate.Visible = false;
    //        txttodate.Visible = false;

    //    }

    //}
    protected void btn_ptax_summary_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btn_pf_summary_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btn_pf_summary.BackColor = System.Drawing.Color.GreenYellow;
        string query = null;
        string downloadname = "PF_Summary";
        int length = ddlunitselect.SelectedValue.Length;
        if (ddlunitselect.Text == "ALL")
        {
            crystalReport.Load(Server.MapPath("~/All_PF_UntiWise.rpt"));
        }
        else
        {
            crystalReport.Load(Server.MapPath("~/All_PF_UntiWise.rpt"));
        }

        if (ddlunitselect.Text == "ALL")
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.PRESENT_DAYS>0 AND   pay_attendance.CPF_SHEET='Yes'   AND pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";
        }
        else
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.PRESENT_DAYS>0 AND   pay_attendance.CPF_SHEET='Yes' AND  pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";


        }
        Session["ReportMonthNo"] = "26";
        ReportLoad(query, downloadname);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btn_esic_summary_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btn_esic_summary.BackColor = System.Drawing.Color.GreenYellow;
        string downloadname = "ESIC_Summary";
        string query = null;
        int length = ddlunitselect.SelectedValue.Length;

        crystalReport.Load(Server.MapPath("~/All_ESIC_UnitWise.rpt"));


        if (ddlunitselect.Text == "ALL")
        {
            query = "   SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.PRESENT_DAYS>0 AND   pay_attendance.CPF_SHEET='Yes' and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'   ";
        }
        else
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_BRANCH As BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.PRESENT_DAYS>0 AND   pay_attendance.CPF_SHEET='Yes' AND pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "'";


        }
        Session["ReportMonthNo"] = "27";
        ReportLoad(query, downloadname);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    protected void btn_pf_challan_Click(object sender, EventArgs e)
    {
        // btn_UAN.Visible = false;
        btn_uan_csv.Visible = false;
        btn_uan_xml.Visible = false;
        d1.con1.Open();
        String currMonth = Session["CURRENT_MONTH"].ToString();
        String currYear = Session["CURRENT_YEAR"].ToString();
        String UnitList = "";
        string strQuery = "";
        string downloadname = "PF_Challan_Download";
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            // strQuery = "SELECT  pay_employee_master.PAN_NUMBER AS 'UAN Number',pay_employee_master.EMP_NAME As 'Member Name',((pay_attendance.C_HEAD01+pay_attendance.C_HEAD02+pay_attendance.C_HEAD03+pay_attendance.C_HEAD04+pay_attendance.C_HEAD05+pay_attendance.C_HEAD06+pay_attendance.C_HEAD07+pay_attendance.C_HEAD08+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS)+(pay_attendance.OT_HRS*pay_employee_master.E_HEAD09)) As 'Gross Wages' ,pay_attendance.PF_GROSS As 'EPF Wages', pay_attendance.PF_GROSS As 'EPS Wages',pay_attendance.PF_GROSS As 'EDLI Wages',pay_attendance.PF As 'EPF Contribution', pay_attendance.COMP_PF As 'EPS Contribution', pay_attendance.COMP_PF_PEN As 'EPF_EPS_DIFF','0' AS 'NCP Days' , '0' As 'Refund Of Advances' FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE WHERE pay_attendance.CPF_SHEET='Yes' AND  (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER";// Don't Delete -> pay_employee_master.JOINING_DATE < dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and
            strQuery = "(SELECT  pay_employee_master.PAN_NUMBER AS 'UAN Number',pay_employee_master.EMP_NAME As 'Member Name',((pay_attendance.C_HEAD01+pay_attendance.C_HEAD02+pay_attendance.C_HEAD03+pay_attendance.C_HEAD04+pay_attendance.C_HEAD05+pay_attendance.C_HEAD06+pay_attendance.C_HEAD07+pay_attendance.C_HEAD08+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS)+(pay_attendance.OT_HRS*pay_employee_master.E_HEAD09)) As 'Gross Wages' ,pay_attendance.PF_GROSS As 'EPF Wages', pay_attendance.PF_GROSS As 'EPS Wages',pay_attendance.PF_GROSS As 'EDLI Wages',pay_attendance.PF As 'EPF Contribution', pay_attendance.COMP_PF As 'EPS Contribution', pay_attendance.COMP_PF_PEN As 'EPF_EPS_DIFF','0' AS 'NCP Days' , '0' As 'Refund Of Advances' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.PF_NUMBER <> 'A' and pay_employee_master.JOINING_DATE < LAST_DAY(str_to_date('01/" + txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3) + "', '%d/%m/%Y') - interval 1 month ) and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH='" + txttodate.Text.Substring(0, 2) + "'  and pay_attendance.YEAR='" + txttodate.Text.Substring(3) + "' order by pay_employee_master.PAN_NUMBER desc) union (SELECT 'Total' AS 'UAN Number',count(pay_employee_master.EMP_NAME) As 'Member Name',sum(((pay_attendance.C_HEAD01+pay_attendance.C_HEAD02+pay_attendance.C_HEAD03+pay_attendance.C_HEAD04+pay_attendance.C_HEAD05+pay_attendance.C_HEAD06+pay_attendance.C_HEAD07+pay_attendance.C_HEAD08+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS)+(pay_attendance.OT_HRS*pay_employee_master.E_HEAD09))) As 'Gross Wages' ,sum(pay_attendance.PF_GROSS) As 'EPF Wages', sum(pay_attendance.PF_GROSS) As 'EPS Wages',sum(pay_attendance.PF_GROSS) As 'EDLI Wages',sum(pay_attendance.PF) As 'EPF Contribution', sum(pay_attendance.COMP_PF) As 'EPS Contribution', sum(pay_attendance.COMP_PF_PEN) As 'EPF_EPS_DIFF','0' AS 'NCP Days' , '0' As 'Refund Of Advances' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.PF_NUMBER <> 'A' and pay_employee_master.JOINING_DATE < LAST_DAY(str_to_date('01/" + txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3) + "', '%d/%m/%Y') - interval 1 month ) and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH='" + txttodate.Text.Substring(0, 2) + "'  and pay_attendance.YEAR='" + txttodate.Text.Substring(3) + "' order by pay_employee_master.PAN_NUMBER desc)";
        }
        else
        {
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);

            //strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  trim(CAST(IFNULL(pay_attendance.PF_GROSS,0) as char)) as 'PF GROSS',  trim(cast(IFNULL(pay_attendance.PF,0) as char)) as 'PF',   ltrim(cast(IFNULL(pay_attendance.COMP_PF,0) as char)) as 'COMP PF',  ltrim(cast(IFNULL(pay_attendance.COMP_PF_PEN,0) as char)) as 'COMP PF PEN',  pay_employee_master.EMP_FATHER_NAME as 'FATHER NAME',  date_format(pay_employee_master.BIRTH_DATE, '%d-%m-%Y') as 'BIRTH DATE',  pay_employee_master.GENDER as 'GENDER',  date_format(pay_employee_master.JOINING_DATE,'%d-%m-%Y') as 'JOINING DATE'  from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code  INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE  INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE   WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE > LAST_DAY(str_to_date('01/" + txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3) + "', '%d/%m/%Y') - interval 1 month )  and (pay_employee_master.PF_NUMBER <> null || pay_employee_master.PF_NUMBER <> '') and (pay_attendance.UNIT_CODE in (" + UnitList + ")) AND  (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '')  and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' order by pay_employee_master.PF_NUMBER";
            strQuery = "(SELECT  pay_employee_master.PAN_NUMBER AS 'UAN Number',pay_employee_master.EMP_NAME As 'Member Name',((pay_attendance.C_HEAD01+pay_attendance.C_HEAD02+pay_attendance.C_HEAD03+pay_attendance.C_HEAD04+pay_attendance.C_HEAD05+pay_attendance.C_HEAD06+pay_attendance.C_HEAD07+pay_attendance.C_HEAD08+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS)+(pay_attendance.OT_HRS*pay_employee_master.E_HEAD09)) As 'Gross Wages' ,pay_attendance.PF_GROSS As 'EPF Wages', pay_attendance.PF_GROSS As 'EPS Wages',pay_attendance.PF_GROSS As 'EDLI Wages',pay_attendance.PF As 'EPF Contribution', pay_attendance.COMP_PF As 'EPS Contribution', pay_attendance.COMP_PF_PEN As 'EPF_EPS_DIFF','0' AS 'NCP Days' , '0' As 'Refund Of Advances' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE < LAST_DAY(str_to_date('01/" + txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3) + "', '%d/%m/%Y') - interval 1 month ) and pay_employee_master.PF_NUMBER <> 'A' and (pay_attendance.UNIT_CODE in (" + UnitList + ")) AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH='" + txttodate.Text.Substring(0, 2) + "'  and pay_attendance.YEAR='" + txttodate.Text.Substring(3) + "' order by pay_employee_master.PAN_NUMBER desc) union (SELECT 'Total' AS 'UAN Number',count(pay_employee_master.EMP_NAME)  As 'Member Name',sum(((pay_attendance.C_HEAD01+pay_attendance.C_HEAD02+pay_attendance.C_HEAD03+pay_attendance.C_HEAD04+pay_attendance.C_HEAD05+pay_attendance.C_HEAD06+pay_attendance.C_HEAD07+pay_attendance.C_HEAD08+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS)+(pay_attendance.OT_HRS*pay_employee_master.E_HEAD09))) As 'Gross Wages' ,sum(pay_attendance.PF_GROSS) As 'EPF Wages', sum(pay_attendance.PF_GROSS) As 'EPS Wages',sum(pay_attendance.PF_GROSS) As 'EDLI Wages',sum(pay_attendance.PF) As 'EPF Contribution', sum(pay_attendance.COMP_PF) As 'EPS Contribution', sum(pay_attendance.COMP_PF_PEN) As 'EPF_EPS_DIFF','0' AS 'NCP Days' , '0' As 'Refund Of Advances' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE < LAST_DAY(str_to_date('01/" + txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3) + "', '%d/%m/%Y') - interval 1 month ) and pay_employee_master.PF_NUMBER <> 'A' and (pay_attendance.UNIT_CODE in (" + UnitList + ")) AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH='" + txttodate.Text.Substring(0, 2) + "'  and pay_attendance.YEAR='" + txttodate.Text.Substring(3) + "' order by pay_employee_master.PAN_NUMBER desc)";
        }

        MySqlDataAdapter adapter = new MySqlDataAdapter(strQuery, d1.con1);
        DataSet ds = new DataSet();
        adapter.Fill(ds);

        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  trim(CAST(IFNULL(pay_attendance.PF_GROSS,0) as char)) as 'PF GROSS',  trim(cast(IFNULL(pay_attendance.PF,0) as char)) as 'PF',   ltrim(cast(IFNULL(pay_attendance.COMP_PF,0) as char)) as 'COMP PF',  ltrim(cast(IFNULL(pay_attendance.COMP_PF_PEN,0) as char)) as 'COMP PF PEN',  '' as 'FATHER NAME', '' as 'BIRTH DATE','' as 'GENDER', '' as 'JOINING DATE'  from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code  INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code  AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE  INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code  AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE   WHERE pay_attendance.CPF_SHEET='Yes' AND  pay_employee_master.JOINING_DATE < LAST_DAY(str_to_date('" + getmonthint(currMonth) + ",01," + currYear + "', '%m,%d,%Y') - interval 1 month )  and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '')  and (pay_employee_master.PF_NUMBER <> null || pay_employee_master.PF_NUMBER <> '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'";
        }
        else
        {
            UnitList = "";
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);
            strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  trim(CAST(IFNULL(pay_attendance.PF_GROSS,0) as char)) as 'PF GROSS',  trim(cast(IFNULL(pay_attendance.PF,0) as char)) as 'PF',   ltrim(cast(IFNULL(pay_attendance.COMP_PF,0) as char)) as 'COMP PF',  ltrim(cast(IFNULL(pay_attendance.COMP_PF_PEN,0) as char)) as 'COMP PF PEN',  '' as 'FATHER NAME', '' as 'BIRTH DATE','' as 'GENDER', '' as 'JOINING DATE' from pay_company_master  INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code  INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE  INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND   pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND  (pay_attendance.UNIT_CODE in (" + UnitList + "))   AND pay_employee_master.JOINING_DATE < LAST_DAY(str_to_date('" + getmonthint(currMonth) + ",01," + currYear + "', '%m,%d,%Y') - interval 1 month )  and (pay_employee_master.PF_NUMBER <> null || pay_employee_master.PF_NUMBER <> '')  and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '')  and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'";
        }

        adapter = new MySqlDataAdapter(strQuery, d1.con1);
        DataSet ds1 = new DataSet();
        adapter.Fill(ds1);
        //ds.Merge(ds1);
        UnitGrid_PF.AutoGenerateColumns = true;
        UnitGrid_PF.DataSource = ds;

        UnitGrid_PF.DataBind();

        UnitGrid_PF.Visible = true;
        btn_pf_challan_dwnld.Visible = true;
        btn_pf_challan_Xmls.Visible = true;
        d1.con1.Close();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    protected void btn_kyc_upload_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        ButtonColor();
        btn_kyc_Download.BackColor = System.Drawing.Color.GreenYellow;
        //ButtonColor();
        //UnitGridView.Visible = true;
        //btn_kyc_Download.Visible = true;
        d1.con1.Open();
        String currMonth = Session["CURRENT_MONTH"].ToString();
        String currYear = Session["CURRENT_YEAR"].ToString();
        // btn_kyc_Download.Visible = true;

        String UnitList = "";
        string strQuery = "";
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            //strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN', pay_employee_master.EMP_FATHER_NAME as 'FATHER NAME', CONVERT(VARCHAR, pay_employee_master.BIRTH_DATE, 103) as 'BIRTH DATE', pay_employee_master.GENDER as 'GENDER', CONVERT(VARCHAR, pay_employee_master.JOINING_DATE,103) as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.PF_NUMBER <> 'A' and pay_employee_master.JOINING_DATE > dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PF_NUMBER";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN ,'B' as DOCUMENT_TYPE, pay_employee_master.BANK_EMP_AC_CODE as DOCUMENT_NUMBER , pay_employee_master.PF_IFSC_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION, pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and (comp_code = '" + Session["comp_code"].ToString() + "')";
            //strQuery = "SELECT pay_employee_master.PAN_NUMBER as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') order by DOCUMENT_TYPE desc";
            strQuery = "SELECT pay_employee_master.PAN_NUMBER AS 'UAN',pay_employee_master.P_TAX_NUMBER AS 'P_TAX_NUMBER',pay_employee_master.EMP_NEW_PAN_NO AS 'EMP_NEW_PAN_NO',pay_employee_master.BANK_EMP_AC_CODE AS 'BANK_EMP_AC_CODE','A' AS 'DOCUMENT_TYPE', pay_employee_master.P_TAX_NUMBER AS 'DOCUMENT_NUMBER', '' AS 'PF_IFSC_CODE',pay_employee_master.EMP_NAME,pay_employee_master.EMP_QUALIFICATION, case when 'Gender' ='M' then 'Male' else 'Female'  END as 'Gender',pay_employee_master.EMP_MARRITAL_STATUS FROM pay_employee_master WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM IS NULL AND (comp_code = '" + Session["comp_code"].ToString() + "') ORDER BY DOCUMENT_TYPE DESC";

        }
        else
        {
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);

            //strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN', pay_employee_master.EMP_FATHER_NAME as 'FATHER NAME', CONVERT(VARCHAR, pay_employee_master.BIRTH_DATE, 103) as 'BIRTH DATE', pay_employee_master.GENDER as 'GENDER', CONVERT(VARCHAR, pay_employee_master.JOINING_DATE,103) as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE > dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and pay_employee_master.PF_NUMBER <> 'A' and (pay_attendance.UNIT_CODE in (" + UnitList + ")) AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PF_NUMBER";
            //   strQuery = "SELECT pay_employee_master.PAN_NUMBER as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "' and (pay_employee_master.UNIT_CODE in (" + UnitList + "))) order by DOCUMENT_TYPE desc";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER))  as UAN,'P' as DOCUMENT_TYPE , pay_employee_master.EMP_NEW_PAN_NO as DOCUMENT_NUMBER,'',pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN ,'B' as DOCUMENT_TYPE, pay_employee_master.BANK_EMP_AC_CODE as DOCUMENT_NUMBER , pay_employee_master.PF_IFSC_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION, pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
            strQuery = "SELECT pay_employee_master.PAN_NUMBER AS 'UAN',pay_employee_master.P_TAX_NUMBER AS 'P_TAX_NUMBER',pay_employee_master.EMP_NEW_PAN_NO AS 'EMP_NEW_PAN_NO',pay_employee_master.BANK_EMP_AC_CODE AS 'BANK_EMP_AC_CODE','A' AS 'DOCUMENT_TYPE', pay_employee_master.P_TAX_NUMBER AS 'DOCUMENT_NUMBER', '' AS 'PF_IFSC_CODE',pay_employee_master.EMP_NAME,pay_employee_master.EMP_QUALIFICATION, case when 'Gender' ='M' then 'Male' else 'Female' END  as 'Gender',pay_employee_master.EMP_MARRITAL_STATUS FROM pay_employee_master WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM IS NULL AND (comp_code = '" + Session["comp_code"].ToString() + "' and (pay_employee_master.UNIT_CODE in (" + UnitList + "))) order by DOCUMENT_TYPE desc";

        }

        MySqlDataAdapter adapter = new MySqlDataAdapter(strQuery, d1.con1);
        DataSet ds = new DataSet();
        adapter.Fill(ds);


        UnitGridView.DataSource = ds;

        UnitGridView.DataBind();

        UnitGridView.Visible = true;
        //    btn_kyc_Download.Visible = true;
        panel_employee_pf_esic_no.Visible = false;
        panel_esic_statement.Visible = false;
        panel_employee_information_status.Visible = false;
        panel_ptax.Visible = false;
        panel_esic_summary_utwise.Visible = false;

        d1.con1.Close();

    }


    protected void btn_form_no9_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        ButtonColor();
        btnpfstatement.BackColor = System.Drawing.Color.GreenYellow;
        string query = null;
        string downloadname = "FORM_No.9";
        crystalReport.Load(Server.MapPath("~/Form9.rpt"));
        //if (ddlunitselect.SelectedValue.ToString() == "ALL")
        //{
        String currMonth = Session["CURRENT_MONTH"].ToString();
        String currYear = Session["CURRENT_YEAR"].ToString();
        query = "select pay_company_master.COMPANY_NAME,pay_company_master.PF_REG_NO, pay_employee_master.PF_NUMBER,pay_employee_master.EMP_NAME,pay_employee_master.EMP_FATHER_NAME,pay_employee_master.BIRTH_DATE,pay_employee_master.JOINING_DATE FROM pay_company_master  INNER JOIN  pay_attendance  ON pay_company_master.comp_code = pay_attendance.comp_code  INNER JOIN pay_unit_master  ON pay_attendance.comp_code = pay_unit_master.comp_code  AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE  INNER JOIN pay_employee_master  ON pay_attendance.comp_code = pay_employee_master.comp_code  AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE   WHERE pay_attendance.CPF_SHEET='Yes'  and (pay_employee_master.LEFT_REASON is null  or rtrim(ltrim(pay_employee_master.LEFT_REASON)) = '')  AND JOINING_DATE > LAST_DAY(str_to_date('" + txttodate.Text.Substring(0, 2) + "/01/" + txttodate.Text.Substring(3) + "','%m/%d/%Y') - interval 1 month) and pay_employee_master.PF_NUMBER <> 'A' and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' order by pay_employee_master.PF_NUMBER";
        //}
        //else
        //{
        //    query = "select pay_company_master.COMPANY_NAME,pay_company_master.PF_REG_NO, pay_employee_master.PF_NUMBER,pay_employee_master.EMP_NAME,pay_employee_master.EMP_FATHER_NAME,pay_employee_master.BIRTH_DATE,pay_employee_master.JOINING_DATE FROM pay_company_master  INNER JOIN  pay_attendance  ON pay_company_master.comp_code = pay_attendance.comp_code  INNER JOIN pay_unit_master  ON pay_attendance.comp_code = pay_unit_master.comp_code  AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE  INNER JOIN pay_employee_master  ON pay_attendance.comp_code = pay_employee_master.comp_code  AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE   WHERE pay_attendance.CPF_SHEET='Yes'  and (pay_employee_master.LEFT_REASON is null  or rtrim(ltrim(pay_employee_master.LEFT_REASON)) = '') AND (pay_attendance.UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "') AND pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PF_NUMBER";
        //}
        Session["ReportMonthNo"] = "30";
        ReportLoad(query, downloadname);
    }


    protected void SqlDataSource_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
    {

    }
    protected void UnitGridView_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void UnitGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == " ")
            {
                e.Row.Cells[i].Text = "";
            }
        }
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
        //    e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
        //    e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.UnitGridView, "Select$" + e.Row.RowIndex);

        //}
    }
    protected void btnkycdownload_Click(object sender, EventArgs e)
    {
        d1.con1.Open();
        //MySqlCommand cmd = new MySqlCommand(); 

        System.Data.DataTable dt1 = new System.Data.DataTable();
        MySqlDataAdapter adp1 = new MySqlDataAdapter("select PF_REG_NO from pay_company_master WHERE comp_code='" + Session["comp_code"].ToString() + "'", d1.con1);
        adp1.Fill(dt1);
        String PFNO = "";
        foreach (DataRow row1 in dt1.Rows)
        {
            foreach (DataColumn column in dt1.Columns)
            {
                //Add the Data rows.
                PFNO = row1[column.ColumnName].ToString();

            }
        }
        PFNO = PFNO.Replace("/", "");

        System.Data.DataTable dt = new System.Data.DataTable();
        MySqlDataAdapter adp;

        //if (ddlunitselect.SelectedValue.ToString() == "ALL")
        //{
        //adp = new MySqlDataAdapter("SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + ',''A'',' + pay_employee_master.P_TAX_NUMBER + ',,' + pay_employee_master.EMP_NAME+ ',' + ' '+ ',' + pay_employee_master.EMP_QUALIFICATION+ ',,,' +  pay_employee_master.GENDER + ',,,' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER desc", d1.con1);
        adp = new MySqlDataAdapter("(SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#A#~#' + pay_employee_master.P_TAX_NUMBER + '#~##~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "') union (SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#P#~#' + pay_employee_master.EMP_NEW_PAN_NO + '#~##~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "') union (SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#B#~#' + pay_employee_master.BANK_EMP_AC_CODE + '#~#' + pay_employee_master.PF_IFSC_CODE + '#~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.EMP_NEW_PAN_NO is null or pay_employee_master.EMP_NEW_PAN_NO = '') and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "')", d1.con1);
        //}
        //else
        //{
        //adp = new MySqlDataAdapter("SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + ',' + substring(pay_employee_master.bank_code,1,1)+ ',' + pay_employee_master.bank_emp_ac_code + ',' + pay_employee_master.PF_IFSC_CODE+ ',' + pay_employee_master.EMP_NAME+ ',' + ' '+ ',' + pay_employee_master.EMP_QUALIFICATION+ ', , ,' +  pay_employee_master.GENDER + ', ,' +pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where KYC_CONFIRM is null and (UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "') AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER desc", d1.con1);
        //        }
        adp.Fill(dt);
        string csv = string.Empty;

        //  csv = "UAN,DOCUMENT TYPE,DOCUMENT NUMBER,IFSC CODE,NAME,EXPIRY DATE,EDUCATIONAL QUALIFICATIONS,PHYSICALLY HANDICAP,PHYSICALLY HANDICAP CATEGORY,GENDER,INTERNATIONAL WORKER,MARITAL STATUS,EST ID";

        //  foreach (DataColumn column in dt.Columns)
        // {
        //Add the Header row for CSV file.
        //     csv += column.ColumnName + ',';
        //}

        //Add new line.
        //csv += "\r\n";

        foreach (DataRow row in dt.Rows)
        {
            foreach (DataColumn column in dt.Columns)
            {
                //Add the Data rows.
                csv += row[column.ColumnName].ToString();
                csv += PFNO;
            }

            //Add new line.
            csv += "\r\n";
        }

        //if (ddlunitselect.SelectedValue.ToString() == "ALL")
        //{

        int res = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = now() WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        int res1 = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = now() WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        int res2 = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = now() WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.EMP_NEW_PAN_NO is null or pay_employee_master.EMP_NEW_PAN_NO = '') and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        //}
        //else
        //{
        //    int res = 0;
        //    res = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = 'Y' WHERE KYC_CONFIRM is null and (UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "') AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        //}

        String Company_name = Session["COMP_NAME"].ToString();
        Company_name = Company_name.Replace(' ', '_');
        //Download the CSV file.
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=KYC_Adhar_Download_" + Company_name + ".csv");
        Response.Charset = "";
        Response.ContentType = "application/text";
        Response.Output.Write(csv);
        Response.Flush();
        Response.End();
    }

    private String getmonthint(String Month)
    {
        if (Month.Equals("JAN") || Month.Equals("jan"))
        {
            return "01";
        }
        else if (Month.Equals("FEB") || Month.Equals("feb"))
        {
            return "02";
        }
        else if (Month.Equals("MAR") || Month.Equals("mar"))
        {
            return "03";
        }
        else if (Month.Equals("APR") || Month.Equals("apr"))
        {
            return "04";
        }
        else if (Month.Equals("MAY") || Month.Equals("may"))
        {
            return "05";
        }
        else if (Month.Equals("JUN") || Month.Equals("jun"))
        {
            return "06";
        }
        else if (Month.Equals("JUL") || Month.Equals("jul"))
        {
            return "07";
        }
        else if (Month.Equals("AUG") || Month.Equals("aug"))
        {
            return "08";
        }
        else if (Month.Equals("SEP") || Month.Equals("sep"))
        {
            return "09";
        }
        else if (Month.Equals("OCT") || Month.Equals("oct"))
        {
            return "10";
        }
        else if (Month.Equals("NOV") || Month.Equals("nov"))
        {
            return "11";
        }
        else if (Month.Equals("DEC") || Month.Equals("dec"))
        {
            return "12";
        }
        else
            return "00";

    }
    protected void btn_pfchallanD_Click(object sender, EventArgs e)
    {
        ButtonColor();
        d1.con1.Open();
        String UnitList = "";

        String currMonth = Session["CURRENT_MONTH"].ToString();
        String currYear = Session["CURRENT_YEAR"].ToString();

        //MySqlCommand cmd = new MySqlCommand();
        System.Data.DataTable dt = new System.Data.DataTable();
        MySqlDataAdapter adp;
        string query = "";
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            // adp = new MySqlDataAdapter("select concat(pay_employee_master.PF_NUMBER,'#~#',pay_employee_master.EMP_NAME, '#~#', ltrim(IFNULL(pay_attendance.PF_GROSS,0)), '#~#', ltrim(IFNULL(pay_attendance.PF_GROSS,0)), '#~#', ltrim(IFNULL(pay_attendance.PF,0)), '#~#', ltrim(IFNULL(pay_attendance.PF,0)), '#~#', ltrim(IFNULL(pay_attendance.COMP_PF,0)), '#~#',ltrim(IFNULL(pay_attendance.COMP_PF,0)), '#~#',ltrim(IFNULL(pay_attendance.COMP_PF_PEN,0)), '#~#', ltrim(IFNULL(pay_attendance.COMP_PF_PEN,0)), '#~#','', '#~#','', '#~#','', '#~#','', '#~#','', '#~#','', '#~#','','#~#','','#~#','','#~#','','#~#','','#~#','','#~#','','#~#','','#~#',''),PF_NUMBER from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE < LAST_DAY(str_to_date('" + txttodate.Text.Substring(0, 2) + "/01/" + txttodate.Text.Substring(3) + "','%m/%d/%Y') - interval 1 month) and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.PF_NUMBER <> 'A' and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'", d1.con1);

            adp = new MySqlDataAdapter("SELECT  concat_ws('#~#',pay_employee_master.PAN_NUMBER,pay_employee_master.EMP_NAME,cast(((pay_attendance.C_HEAD01+pay_attendance.C_HEAD02+pay_attendance.C_HEAD03+pay_attendance.C_HEAD04+pay_attendance.C_HEAD05+pay_attendance.C_HEAD06+pay_attendance.C_HEAD07+pay_attendance.C_HEAD08+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS)+(pay_attendance.OT_HRS*pay_employee_master.E_HEAD09)) As char),cast(pay_attendance.PF_GROSS As char),cast(pay_attendance.PF_GROSS AS char),cast(pay_attendance.PF As char),cast(pay_attendance.COMP_PF As char),cast(pay_attendance.COMP_PF_PEN As char),'0' , '0') as 'PF_CSV' FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE < LAST_DAY(str_to_date('" + txttodate.Text.Substring(0, 2) + "/01/" + txttodate.Text.Substring(3) + "','%m/%d/%Y') - interval 1 month) and pay_employee_master.PF_NUMBER <> 'A' AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER", d1.con1);


        }
        else
        {
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);

            // adp = new MySqlDataAdapter("select concat(pay_employee_master.PF_NUMBER,'#~#',pay_employee_master.EMP_NAME, '#~#', ltrim(IFNULL(pay_attendance.PF_GROSS,0)), '#~#', ltrim(IFNULL(pay_attendance.PF_GROSS,0)), '#~#', ltrim(IFNULL(pay_attendance.PF,0)), '#~#', ltrim(IFNULL(pay_attendance.PF,0)), '#~#', ltrim(IFNULL(pay_attendance.COMP_PF,0)), '#~#',ltrim(IFNULL(pay_attendance.COMP_PF,0)), '#~#',ltrim(IFNULL(pay_attendance.COMP_PF_PEN,0)), '#~#', ltrim(IFNULL(pay_attendance.COMP_PF_PEN,0)), '#~#','', '#~#','', '#~#','', '#~#','', '#~#','', '#~#','', '#~#','','#~#','','#~#','','#~#','','#~#','','#~#','','#~#','','#~#','','#~#',''),PF_NUMBER from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_attendance.UNIT_CODE in (" + UnitList + "))  AND pay_employee_master.JOINING_DATE < LAST_DAY(str_to_date('" + txttodate.Text.Substring(0, 2) + "/01/" + txttodate.Text.Substring(3) + "','%m/%d/%Y') - interval 1 month) and pay_employee_master.PF_NUMBER <> 'A' and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'", d1.con1);
            adp = new MySqlDataAdapter("SELECT  concat_ws('#~#',pay_employee_master.PAN_NUMBER,pay_employee_master.EMP_NAME,cast(((pay_attendance.C_HEAD01+pay_attendance.C_HEAD02+pay_attendance.C_HEAD03+pay_attendance.C_HEAD04+pay_attendance.C_HEAD05+pay_attendance.C_HEAD06+pay_attendance.C_HEAD07+pay_attendance.C_HEAD08+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS)+(pay_attendance.OT_HRS*pay_employee_master.E_HEAD09)) As char),cast(pay_attendance.PF_GROSS As char),cast(pay_attendance.PF_GROSS AS char),cast(pay_attendance.PF As char),cast(pay_attendance.COMP_PF As char),cast(pay_attendance.COMP_PF_PEN As char),'0' , '0') as 'PF_CSV' FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_attendance.UNIT_CODE in (" + UnitList + "))  AND pay_employee_master.JOINING_DATE < LAST_DAY(str_to_date('" + txttodate.Text.Substring(0, 2) + "/01/" + txttodate.Text.Substring(3) + "','%m/%d/%Y') - interval 1 month) and pay_employee_master.PF_NUMBER <> 'A' and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'", d1.con1);

        }
        adp.Fill(dt);

        System.Data.DataTable dt1 = new System.Data.DataTable();
        MySqlDataAdapter adp1;

        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            adp1 = new MySqlDataAdapter("select concat(pay_employee_master.PF_NUMBER,'#~#',pay_employee_master.EMP_NAME, '#~#', ltrim(IFNULL(pay_attendance.PF_GROSS,0)), '#~#', ltrim(IFNULL(pay_attendance.PF_GROSS,0)), '#~#', ltrim(IFNULL(pay_attendance.PF,0)), '#~#', ltrim(IFNULL(pay_attendance.PF,0)), '#~#', ltrim(IFNULL(pay_attendance.COMP_PF,0)), '#~#',ltrim(IFNULL(pay_attendance.COMP_PF,0)), '#~#',ltrim(IFNULL(pay_attendance.COMP_PF_PEN,0)), '#~#', ltrim(IFNULL(pay_attendance.COMP_PF_PEN,0)), '#~#','', '#~#','', '#~#','', '#~#','', '#~#','', '#~#','', '#~#',pay_employee_master.EMP_FATHER_NAME, '#~#',substring(pay_employee_master.FATHER_RELATION,1,1), '#~#',pay_employee_master.BIRTH_DATE, '#~#',pay_employee_master.GENDER, '#~#',pay_employee_master.JOINING_DATE, '#~#',pay_employee_master.JOINING_DATE,'#~#','','#~#','','#~#',''), PF_NUMBER from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.PF_NUMBER <> 'A' and pay_employee_master.JOINING_DATE > LAST_DAY(str_to_date('" + txttodate.Text.Substring(3, 2) + "/01/" + txttodate.Text.Substring(3) + "','%m/%d/%Y') - interval 1 month) and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'", d1.con1);
        }
        else
        {
            UnitList = "";
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);
            adp1 = new MySqlDataAdapter("select concat(pay_employee_master.PF_NUMBER,'#~#',pay_employee_master.EMP_NAME, '#~#', ltrim(IFNULL(pay_attendance.PF_GROSS,0)), '#~#', ltrim(IFNULL(pay_attendance.PF_GROSS,0)), '#~#', ltrim(IFNULL(pay_attendance.PF,0)), '#~#', ltrim(IFNULL(pay_attendance.PF,0)), '#~#', ltrim(IFNULL(pay_attendance.COMP_PF,0)), '#~#',ltrim(IFNULL(pay_attendance.COMP_PF,0)), '#~#',ltrim(IFNULL(pay_attendance.COMP_PF_PEN,0)), '#~#', ltrim(IFNULL(pay_attendance.COMP_PF_PEN,0)), '#~#','', '#~#','', '#~#','', '#~#','', '#~#','', '#~#','', '#~#',pay_employee_master.EMP_FATHER_NAME, '#~#',substring(pay_employee_master.FATHER_RELATION,1,1), '#~#',pay_employee_master.BIRTH_DATE, '#~#',pay_employee_master.GENDER, '#~#',pay_employee_master.JOINING_DATE, '#~#',pay_employee_master.JOINING_DATE,'#~#','','#~#','','#~#',''), PF_NUMBER from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE > LAST_DAY(str_to_date('" + txttodate.Text.Substring(3, 2) + "/01/" + txttodate.Text.Substring(6) + "','%m/%d/%Y') - interval 1 month) and pay_employee_master.PF_NUMBER <> 'A' and (pay_attendance.UNIT_CODE in (" + UnitList + ")) AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH ='" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'", d1.con1);
        }
        adp1.Fill(dt1);


        string csv = string.Empty;

        // csv = "Member Id,Member Name,EPF Wages,Eps Wages,EPF Contribution,EPF Contribution being remitted,EPS Contribution due,EPS Contribution being remitted,Diff. EPF & EPS Contribution due,Diff EPF & EPS,NCP Days,Refund of Advances,Arrear EPF Wages,Arrear EPF EE Share,Arrear EPF ER Share,Arrear EPS Share,Father Name,Relationship,Date Of Birth,Gender,Joining EPF,Joining EPS,Date of Exit EPF,Date of Exit from EPS,Reason Of Leaving";

        //Add new line.
        //csv += "\r\n";
        // dt.Merge(dt1);
        dt.DefaultView.Sort = "PF_CSV";
        dt = dt.DefaultView.ToTable();
        // dt.DefaultView.Sort= "pay_employee_master.PF_NUMBER";
        int a = 0;
        foreach (DataRow row in dt.Rows)
        {
            a = 0;
            foreach (DataColumn column in dt.Columns)
            {
                //Add the Data rows.
                if (a == 0)
                {
                    csv += row[column.ColumnName].ToString();
                    a = 1;
                }
            }

            //Add new line.
            csv += "\r\n";
        }

        //foreach (DataRow row1 in dt1.Rows)
        //{
        //    foreach (DataColumn column1 in dt1.Columns)
        //    {
        //        //Add the Data rows.
        //        csv += row1[column1.ColumnName].ToString();
        //    }

        //    //Add new line.
        //    csv += "\r\n";
        //}

        String Company_name = Session["COMP_NAME"].ToString();
        Company_name = Company_name.Replace(' ', '_');
        //Download the CSV file.
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=PF_Challan_Download_" + Company_name + ".csv");
        Response.Charset = "";
        Response.ContentType = "application/text";
        Response.Output.Write(csv);
        Response.Flush();
        Response.End();
        //UnitGrid_PF.Visible = false;
        UnitGrid_PF.Visible = true;
        btn_pf_challan_dwnld.Visible = false;
        panel_bankexcel.Visible = false;
        panel_esic_statement.Visible = false;
        panel_ptax.Visible = false;

    }

    public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
    {
        /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
           server control at run time. */
    }



    protected void btn_pfchallan_xmls(object sender, EventArgs args)
    {


        if (UnitGrid_PF.Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=PFChallanDownload.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                UnitGrid_PF.AllowPaging = false;
                UnitBAL ubl1 = new UnitBAL();
                foreach (TableCell cell in UnitGrid_PF.HeaderRow.Cells)
                {
                    cell.BackColor = UnitGrid_PF.HeaderStyle.BackColor;
                }
                foreach (GridViewRow row in UnitGrid_PF.Rows)
                {

                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = UnitGrid_PF.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = UnitGrid_PF.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                UnitGrid_PF.RenderControl(hw);

                //style to format numbers to string
                string style = @"<style> .textmode { } </style>";
                Response.Write(style);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }


    }

    protected void btn_kyc_upoad_pan(object sender, EventArgs args)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        d1.con1.Open();
        String currMonth = Session["CURRENT_MONTH"].ToString();
        String currYear = Session["CURRENT_YEAR"].ToString();
        btn_kyc_pandownload.Visible = true;

        String UnitList = "";
        string strQuery = "";
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            //strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN', pay_employee_master.EMP_FATHER_NAME as 'FATHER NAME', CONVERT(VARCHAR, pay_employee_master.BIRTH_DATE, 103) as 'BIRTH DATE', pay_employee_master.GENDER as 'GENDER', CONVERT(VARCHAR, pay_employee_master.JOINING_DATE,103) as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.PF_NUMBER <> 'A' and pay_employee_master.JOINING_DATE > dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PF_NUMBER";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') order by DOCUMENT_TYPE desc";
            //strQuery = "SELECT pay_employee_master.PAN_NUMBER  as UAN,'P' as DOCUMENT_TYPE , pay_employee_master.EMP_NEW_PAN_NO as DOCUMENT_NUMBER,pay_employee_master.PF_IFSC_CODE,pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "')";
            strQuery = "SELECT pay_employee_master.PAN_NUMBER AS 'UAN',pay_employee_master.P_TAX_NUMBER AS 'ADHARCARD NO',pay_employee_master.EMP_NEW_PAN_NO AS 'PAN NO',pay_employee_master.BANK_EMP_AC_CODE AS 'BANK ACCOUNT NO','A' AS 'DOCUMENT_TYPE', pay_employee_master.P_TAX_NUMBER AS 'DOCUMENT_NUMBER', '' AS 'PF_IFSC_CODE',pay_employee_master.EMP_NAME,pay_employee_master.EMP_QUALIFICATION, pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS FROM pay_employee_master WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM IS NULL AND (comp_code = '" + Session["comp_code"].ToString() + "') ORDER BY DOCUMENT_TYPE DESC";

        }
        else
        {
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);

            //strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN', pay_employee_master.EMP_FATHER_NAME as 'FATHER NAME', CONVERT(VARCHAR, pay_employee_master.BIRTH_DATE, 103) as 'BIRTH DATE', pay_employee_master.GENDER as 'GENDER', CONVERT(VARCHAR, pay_employee_master.JOINING_DATE,103) as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE > dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and pay_employee_master.PF_NUMBER <> 'A' and (pay_attendance.UNIT_CODE in (" + UnitList + ")) AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PF_NUMBER";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
            //strQuery = "SELECT pay_employee_master.PAN_NUMBER  as UAN,'P' as DOCUMENT_TYPE , pay_employee_master.EMP_NEW_PAN_NO as DOCUMENT_NUMBER,pay_employee_master.PF_IFSC_CODE,pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
            strQuery = "SELECT pay_employee_master.PAN_NUMBER AS 'UAN',pay_employee_master.P_TAX_NUMBER AS 'ADHARCARD NO',pay_employee_master.EMP_NEW_PAN_NO AS 'PAN NO',pay_employee_master.BANK_EMP_AC_CODE AS 'BANK ACCOUNT NO','A' AS 'DOCUMENT_TYPE', pay_employee_master.P_TAX_NUMBER AS 'DOCUMENT_NUMBER', '' AS 'PF_IFSC_CODE',pay_employee_master.EMP_NAME,pay_employee_master.EMP_QUALIFICATION, pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS FROM pay_employee_master WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM IS NULL AND (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
        }

        MySqlDataAdapter adapter = new MySqlDataAdapter(strQuery, d1.con1);
        DataSet ds = new DataSet();
        adapter.Fill(ds);


        UnitGridView.DataSource = ds;

        UnitGridView.DataBind();

        UnitGridView.Visible = true;
        //btn_pf_challan_dwnld.Visible = true;
        //btn_pf_challan_Xmls.Visible = true;
        //btn_kyc_Download.Visible = true;
        d1.con1.Close();

    }

    protected void btn_kyc_upload_bank(object sender, EventArgs args)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        d1.con1.Open();
        String currMonth = Session["CURRENT_MONTH"].ToString();
        String currYear = Session["CURRENT_YEAR"].ToString();
        btn_kyc_bankdownload.Visible = true;
        String UnitList = "";
        string strQuery = "";
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            //strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN', pay_employee_master.EMP_FATHER_NAME as 'FATHER NAME', CONVERT(VARCHAR, pay_employee_master.BIRTH_DATE, 103) as 'BIRTH DATE', pay_employee_master.GENDER as 'GENDER', CONVERT(VARCHAR, pay_employee_master.JOINING_DATE,103) as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.PF_NUMBER <> 'A' and pay_employee_master.JOINING_DATE > dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PF_NUMBER";
            // strQuery = "SELECT pay_employee_master.PAN_NUMBER as UAN ,'B' as DOCUMENT_TYPE, pay_employee_master.BANK_EMP_AC_CODE as DOCUMENT_NUMBER , pay_employee_master.PF_IFSC_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION, pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and (comp_code = '" + Session["comp_code"].ToString() + "')";

            strQuery = "SELECT pay_employee_master.PAN_NUMBER AS 'UAN',pay_employee_master.P_TAX_NUMBER AS 'ADHARCARD NO',pay_employee_master.EMP_NEW_PAN_NO AS 'PAN NO',pay_employee_master.BANK_EMP_AC_CODE AS 'BANK ACCOUNT NO','A' AS 'DOCUMENT_TYPE', pay_employee_master.P_TAX_NUMBER AS 'DOCUMENT_NUMBER', '' AS 'PF_IFSC_CODE',pay_employee_master.EMP_NAME,pay_employee_master.EMP_QUALIFICATION, pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS FROM pay_employee_master WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM IS NULL AND (comp_code = '" + Session["comp_code"].ToString() + "')  order by DOCUMENT_TYPE desc";
        }
        else
        {
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);

            //strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN', pay_employee_master.EMP_FATHER_NAME as 'FATHER NAME', CONVERT(VARCHAR, pay_employee_master.BIRTH_DATE, 103) as 'BIRTH DATE', pay_employee_master.GENDER as 'GENDER', CONVERT(VARCHAR, pay_employee_master.JOINING_DATE,103) as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE > dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and pay_employee_master.PF_NUMBER <> 'A' and (pay_attendance.UNIT_CODE in (" + UnitList + ")) AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PF_NUMBER";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER))  as UAN,'P' as DOCUMENT_TYPE , pay_employee_master.EMP_NEW_PAN_NO as DOCUMENT_NUMBER,'',pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
            strQuery = "SELECT pay_employee_master.PAN_NUMBER AS 'UAN',pay_employee_master.P_TAX_NUMBER AS 'ADHARCARD NO',pay_employee_master.EMP_NEW_PAN_NO AS 'PAN NO',pay_employee_master.BANK_EMP_AC_CODE AS 'BANK ACCOUNT NO','A' AS 'DOCUMENT_TYPE', pay_employee_master.P_TAX_NUMBER AS 'DOCUMENT_NUMBER', '' AS 'PF_IFSC_CODE',pay_employee_master.EMP_NAME,pay_employee_master.EMP_QUALIFICATION, pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS FROM pay_employee_master WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM IS NULL AND (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
        }

        MySqlDataAdapter adapter = new MySqlDataAdapter(strQuery, d1.con1);
        DataSet ds = new DataSet();
        adapter.Fill(ds);

        //if (ddlunitselect.SelectedValue.ToString() == "ALL")
        //{
        //    // strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN','' as 'FATHER NAME', '' as 'BIRTH DATE','' as 'GENDER', '' as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE < dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.PF_NUMBER <> 'A' and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'";
        //    //strQuery="SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') order by DOCUMENT_TYPE desc";
        //   // strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER))  as UAN,'P' as DOCUMENT_TYPE , pay_employee_master.EMP_NEW_PAN_NO as DOCUMENT_NUMBER,'',pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "')";
        //    strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN ,'B' as DOCUMENT_TYPE, pay_employee_master.BANK_EMP_AC_CODE as DOCUMENT_NUMBER , pay_employee_master.PF_IFSC_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION, pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and (comp_code = '" + Session["comp_code"].ToString() + "')";
        //}
        //else
        //{
        //    UnitList = "";
        //    foreach (ListItem listItem in ddlunitselect.Items)
        //    {
        //        if (listItem.Selected == true)
        //        {
        //            UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
        //        }
        //    }
        //    UnitList = UnitList.Substring(0, UnitList.Length - 1);
        //    //strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN', '' as 'FATHER NAME', '' as 'BIRTH DATE','' as 'GENDER', '' as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_attendance.UNIT_CODE in (" + UnitList + "))  AND pay_employee_master.JOINING_DATE < dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and pay_employee_master.PF_NUMBER <> 'A' and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'";
        //    //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
        //    //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER))  as UAN,'P' as DOCUMENT_TYPE , pay_employee_master.EMP_NEW_PAN_NO as DOCUMENT_NUMBER,'',pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
        //    strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN ,'B' as DOCUMENT_TYPE, pay_employee_master.BANK_EMP_AC_CODE as DOCUMENT_NUMBER , pay_employee_master.PF_IFSC_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION, pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
        //}

        //adapter = new MySqlDataAdapter(strQuery, d1.con1);
        //DataSet ds1 = new DataSet();
        //adapter.Fill(ds1);
        //ds.Merge(ds1);
        //UnitGridView.AutoGenerateColumns = true;
        UnitGridView.DataSource = ds;

        UnitGridView.DataBind();

        UnitGridView.Visible = true;
        //btn_pf_challan_dwnld.Visible = true;
        //btn_pf_challan_Xmls.Visible = true;
        //btn_kyc_Download.Visible = true;
        d1.con1.Close();
    }


    protected void btn_kyc_bank_down(object sender, EventArgs e)
    {

        d1.con1.Open();
        //MySqlCommand cmd = new MySqlCommand(); 

        System.Data.DataTable dt1 = new System.Data.DataTable();
        MySqlDataAdapter adp1 = new MySqlDataAdapter("select PF_REG_NO from pay_company_master WHERE comp_code='" + Session["comp_code"].ToString() + "'", d1.con1);
        adp1.Fill(dt1);
        String PFNO = "";
        foreach (DataRow row1 in dt1.Rows)
        {
            foreach (DataColumn column in dt1.Columns)
            {
                //Add the Data rows.
                PFNO = row1[column.ColumnName].ToString();

            }
        }
        PFNO = PFNO.Replace("/", "");

        System.Data.DataTable dt = new System.Data.DataTable();
        MySqlDataAdapter adp;


        String UnitList = "";
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            // strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN','' as 'FATHER NAME', '' as 'BIRTH DATE','' as 'GENDER', '' as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE < dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.PF_NUMBER <> 'A' and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'";
            //strQuery="SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') order by DOCUMENT_TYPE desc";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER))  as UAN,'P' as DOCUMENT_TYPE , pay_employee_master.EMP_NEW_PAN_NO as DOCUMENT_NUMBER,'',pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "')";
            adp = new MySqlDataAdapter("(SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#B#~#' + pay_employee_master.BANK_EMP_AC_CODE + '#~#' + pay_employee_master.PF_IFSC_CODE + '#~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "')", d1.con1);
        }
        else
        {

            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);
            //strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN', '' as 'FATHER NAME', '' as 'BIRTH DATE','' as 'GENDER', '' as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_attendance.UNIT_CODE in (" + UnitList + "))  AND pay_employee_master.JOINING_DATE < dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and pay_employee_master.PF_NUMBER <> 'A' and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER))  as UAN,'P' as DOCUMENT_TYPE , pay_employee_master.EMP_NEW_PAN_NO as DOCUMENT_NUMBER,'',pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
            //adp = new MySqlDataAdapter("(SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#P#~#' + pay_employee_master.EMP_NEW_PAN_NO + '#~##~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) ", d1.con1);
            adp = new MySqlDataAdapter("(SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#B#~#' + pay_employee_master.BANK_EMP_AC_CODE + '#~#' + pay_employee_master.PF_IFSC_CODE + '#~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' and (pay_employee_master.UNIT_CODE in (" + UnitList + ")))", d1.con1);
        }





        //if (ddlunitselect.SelectedValue.ToString() == "ALL")
        //{
        //adp = new MySqlDataAdapter("SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + ',''A'',' + pay_employee_master.P_TAX_NUMBER + ',,' + pay_employee_master.EMP_NAME+ ',' + ' '+ ',' + pay_employee_master.EMP_QUALIFICATION+ ',,,' +  pay_employee_master.GENDER + ',,,' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER desc", d1.con1);
        // adp = new MySqlDataAdapter("(SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#A#~#' + pay_employee_master.P_TAX_NUMBER + '#~##~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "') union (SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#P#~#' + pay_employee_master.EMP_NEW_PAN_NO + '#~##~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "') union (SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#B#~#' + pay_employee_master.BANK_EMP_AC_CODE + '#~#' + pay_employee_master.PF_IFSC_CODE + '#~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.EMP_NEW_PAN_NO is null or pay_employee_master.EMP_NEW_PAN_NO = '') and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "')", d1.con1);

        //new code
        //adp = new MySqlDataAdapter("(SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#B#~#' + pay_employee_master.BANK_EMP_AC_CODE + '#~#' + pay_employee_master.PF_IFSC_CODE + '#~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "')",d1.con1);

        //}
        //else
        //{
        //adp = new MySqlDataAdapter("SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + ',' + substring(pay_employee_master.bank_code,1,1)+ ',' + pay_employee_master.bank_emp_ac_code + ',' + pay_employee_master.PF_IFSC_CODE+ ',' + pay_employee_master.EMP_NAME+ ',' + ' '+ ',' + pay_employee_master.EMP_QUALIFICATION+ ', , ,' +  pay_employee_master.GENDER + ', ,' +pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where KYC_CONFIRM is null and (UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "') AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER desc", d1.con1);
        //        }
        adp.Fill(dt);
        string csv = string.Empty;

        //  csv = "UAN,DOCUMENT TYPE,DOCUMENT NUMBER,IFSC CODE,NAME,EXPIRY DATE,EDUCATIONAL QUALIFICATIONS,PHYSICALLY HANDICAP,PHYSICALLY HANDICAP CATEGORY,GENDER,INTERNATIONAL WORKER,MARITAL STATUS,EST ID";

        //  foreach (DataColumn column in dt.Columns)
        // {
        //Add the Header row for CSV file.
        //     csv += column.ColumnName + ',';
        //}

        //Add new line.
        //csv += "\r\n";

        foreach (DataRow row in dt.Rows)
        {
            foreach (DataColumn column in dt.Columns)
            {
                //Add the Data rows.
                csv += row[column.ColumnName].ToString();
                csv += PFNO;
            }

            //Add new line.
            csv += "\r\n";
        }

        //if (ddlunitselect.SelectedValue.ToString() == "ALL")
        //{

        //int res = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = getdate() WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        //int res1 = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = getdate() WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        //int res2 = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = getdate() WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.EMP_NEW_PAN_NO is null or pay_employee_master.EMP_NEW_PAN_NO = '') and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        ////}
        ////else
        //{
        //    int res = 0;
        //    res = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = 'Y' WHERE KYC_CONFIRM is null and (UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "') AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        //}

        String Company_name = Session["COMP_NAME"].ToString();
        Company_name = Company_name.Replace(' ', '_');
        //Download the CSV file.
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=KYC_Bank_Account_Download_" + Company_name + ".csv");
        Response.Charset = "";
        Response.ContentType = "application/text";
        Response.Output.Write(csv);
        Response.Flush();
        Response.End();

    }


    protected void btn_kyc_pan_down(Object sender, EventArgs e)
    {

        d1.con1.Open();
        //MySqlCommand cmd = new MySqlCommand(); 

        System.Data.DataTable dt1 = new System.Data.DataTable();
        MySqlDataAdapter adp1 = new MySqlDataAdapter("select PF_REG_NO from pay_company_master WHERE comp_code='" + Session["comp_code"].ToString() + "'", d1.con1);
        adp1.Fill(dt1);
        String PFNO = "";
        foreach (DataRow row1 in dt1.Rows)
        {
            foreach (DataColumn column in dt1.Columns)
            {
                //Add the Data rows.
                PFNO = row1[column.ColumnName].ToString();

            }
        }
        PFNO = PFNO.Replace("/", "");

        System.Data.DataTable dt = new System.Data.DataTable();
        MySqlDataAdapter adp;

        String UnitList = "";
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            // strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN','' as 'FATHER NAME', '' as 'BIRTH DATE','' as 'GENDER', '' as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND pay_employee_master.JOINING_DATE < dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.PF_NUMBER <> 'A' and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'";
            //strQuery="SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') order by DOCUMENT_TYPE desc";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER))  as UAN,'P' as DOCUMENT_TYPE , pay_employee_master.EMP_NEW_PAN_NO as DOCUMENT_NUMBER,'',pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "')";
            adp = new MySqlDataAdapter("(SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#P#~#' + pay_employee_master.EMP_NEW_PAN_NO + '#~##~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "'))", d1.con1);
        }
        else
        {

            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);
            //strQuery = "select pay_employee_master.PF_NUMBER as 'PF NUMBER', pay_employee_master.EMP_NAME as 'EMPLOYEE NAME',  ltrim(str(ISNULL(pay_attendance.PF_GROSS,0),25,0)) as 'PF GROSS',  ltrim(str(ISNULL(pay_attendance.PF,0),25,0)) as 'PF',  ltrim(str(ISNULL(pay_attendance.COMP_PF,0),25,0)) as 'COMP PF', ltrim(str(ISNULL(pay_attendance.COMP_PF_PEN,0),25,0)) as 'COMP PF PEN', '' as 'FATHER NAME', '' as 'BIRTH DATE','' as 'GENDER', '' as 'JOINING DATE' from pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND  pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND  pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE  WHERE pay_attendance.CPF_SHEET='Yes' AND (pay_attendance.UNIT_CODE in (" + UnitList + "))  AND pay_employee_master.JOINING_DATE < dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) and pay_employee_master.PF_NUMBER <> 'A' and (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER)) as UAN,'A'as DOCUMENT_TYPE , pay_employee_master.P_TAX_NUMBER as DOCUMENT_NUMBER ,'' as PF_IFSC_CODE,pay_employee_master.EMP_NAME , pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
            //strQuery = "SELECT top 10000 rtrim(ltrim(pay_employee_master.PAN_NUMBER))  as UAN,'P' as DOCUMENT_TYPE , pay_employee_master.EMP_NEW_PAN_NO as DOCUMENT_NUMBER,'',pay_employee_master.EMP_NAME, pay_employee_master.EMP_QUALIFICATION,pay_employee_master.GENDER ,pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + ")) order by DOCUMENT_TYPE desc";
            adp = new MySqlDataAdapter("(SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#P#~#' + pay_employee_master.EMP_NEW_PAN_NO + '#~##~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (comp_code = '" + Session["comp_code"].ToString() + "') and (pay_employee_master.UNIT_CODE in (" + UnitList + "))) ", d1.con1);
        }


        //if (ddlunitselect.SelectedValue.ToString() == "ALL")
        //{
        //adp = new MySqlDataAdapter("SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + ',''A'',' + pay_employee_master.P_TAX_NUMBER + ',,' + pay_employee_master.EMP_NAME+ ',' + ' '+ ',' + pay_employee_master.EMP_QUALIFICATION+ ',,,' +  pay_employee_master.GENDER + ',,,' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER desc", d1.con1);
        // adp = new MySqlDataAdapter("(SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#A#~#' + pay_employee_master.P_TAX_NUMBER + '#~##~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "') union (SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#P#~#' + pay_employee_master.EMP_NEW_PAN_NO + '#~##~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "') union (SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#B#~#' + pay_employee_master.BANK_EMP_AC_CODE + '#~#' + pay_employee_master.PF_IFSC_CODE + '#~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.EMP_NEW_PAN_NO is null or pay_employee_master.EMP_NEW_PAN_NO = '') and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "')", d1.con1);

        // new query

        // adp = new MySqlDataAdapter("(SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + '#~#P#~#' + pay_employee_master.EMP_NEW_PAN_NO + '#~##~#' + pay_employee_master.EMP_NAME+ '#~##~##~##~##~##~##~##~#' from pay_employee_master where pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "')", d1.con1);

        //}
        //else
        //{
        //adp = new MySqlDataAdapter("SELECT rtrim(ltrim(pay_employee_master.PAN_NUMBER)) + ',' + substring(pay_employee_master.bank_code,1,1)+ ',' + pay_employee_master.bank_emp_ac_code + ',' + pay_employee_master.PF_IFSC_CODE+ ',' + pay_employee_master.EMP_NAME+ ',' + ' '+ ',' + pay_employee_master.EMP_QUALIFICATION+ ', , ,' +  pay_employee_master.GENDER + ', ,' +pay_employee_master.EMP_MARRITAL_STATUS from pay_employee_master where KYC_CONFIRM is null and (UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "') AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER desc", d1.con1);
        //        }
        adp.Fill(dt);
        string csv = string.Empty;

        //  csv = "UAN,DOCUMENT TYPE,DOCUMENT NUMBER,IFSC CODE,NAME,EXPIRY DATE,EDUCATIONAL QUALIFICATIONS,PHYSICALLY HANDICAP,PHYSICALLY HANDICAP CATEGORY,GENDER,INTERNATIONAL WORKER,MARITAL STATUS,EST ID";

        //  foreach (DataColumn column in dt.Columns)
        // {
        //Add the Header row for CSV file.
        //     csv += column.ColumnName + ',';
        //}

        //Add new line.
        //csv += "\r\n";

        foreach (DataRow row in dt.Rows)
        {
            foreach (DataColumn column in dt.Columns)
            {
                //Add the Data rows.
                csv += row[column.ColumnName].ToString();
                csv += PFNO;
            }

            //Add new line.
            csv += "\r\n";
        }

        //if (ddlunitselect.SelectedValue.ToString() == "ALL")
        //{

        //int res = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = getdate() WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.P_TAX_NUMBER is not null and pay_employee_master.P_TAX_NUMBER <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        //int res1 = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = getdate() WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.EMP_NEW_PAN_NO is not null and pay_employee_master.EMP_NEW_PAN_NO <> '' and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        //int res2 = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = getdate() WHERE pay_employee_master.PF_NUMBER <> 'A' AND KYC_CONFIRM is null and pay_employee_master.BANK_EMP_AC_CODE is not null and pay_employee_master.BANK_EMP_AC_CODE <> '' and pay_employee_master.PAN_NUMBER is not null and pay_employee_master.PAN_NUMBER <> '' and (pay_employee_master.EMP_NEW_PAN_NO is null or pay_employee_master.EMP_NEW_PAN_NO = '') and (pay_employee_master.P_TAX_NUMBER = '' or pay_employee_master.P_TAX_NUMBER is null) and (pay_employee_master.PF_IFSC_CODE is not null or pay_employee_master.PF_IFSC_CODE <> '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        ////}
        //else
        //{
        //    int res = 0;
        //    res = d1.operation("UPDATE pay_employee_master SET KYC_CONFIRM = 'Y' WHERE KYC_CONFIRM is null and (UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "') AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "'");
        //}

        String Company_name = Session["COMP_NAME"].ToString();
        Company_name = Company_name.Replace(' ', '_');
        //Download the CSV file.
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=KYC_Pan_Account_Download_" + Company_name + ".csv");
        Response.Charset = "";
        Response.ContentType = "application/text";
        Response.Output.Write(csv);
        Response.Flush();
        Response.End();



    }
    protected void btn_uan_csv_Click(object sender, EventArgs e)
    {
        ButtonColor();
        d1.con1.Open();
        String UnitList = "";

        String currMonth = Session["CURRENT_MONTH"].ToString();
        String currYear = Session["CURRENT_YEAR"].ToString();

        //MySqlCommand cmd = new MySqlCommand();
        System.Data.DataTable dt = new System.Data.DataTable();
        MySqlDataAdapter adp;
        string query = "";
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            adp = new MySqlDataAdapter("select  isnull (pay_employee_master.PAN_NUMBER,'') + '#~#' + '#~#'+isnull(EMP_NAME,'')+'#~#' + isnull(CONVERT(VARCHAR, BIRTH_DATE, 103),'')+ '#~#' + isnull(CONVERT(VARCHAR, JOINING_DATE, 103),'')+ '#~#' + isnull(pay_employee_master.GENDER,'')+ '#~#' +  isnull(EMP_FATHER_NAME,'')+ '#~#' + CASE WHEN FATHER_RELATION='Father' then 'F' else 'H' END + '#~#' + isnull(EMP_MOBILE_NO,'')+ '#~#' +  ''+ '#~#' + 'INDIAN'+ '#~#' +  ''+ '#~#' +  ''+'#~#'+Case When EMP_MARRITAL_STATUS ='Married' then 'M' When EMP_MARRITAL_STATUS = 'Single' then 'U' When EMP_MARRITAL_STATUS='Divorced' then 'D' When EMP_MARRITAL_STATUS='Widowed' then 'W' Else '' END+'#~#'+'N'+'#~#'+''+'#~#'+''+'#~#'+''+'#~#'+''+'#~#'+'N'+'#~#'+''+'#~#'+''+'#~#'+''+'#~#'+isnull(BANK_EMP_AC_CODE,'')+'#~#'+isnull(PF_IFSC_CODE,'')+'#~#'+ Case When BANK_EMP_AC_CODE != NULL then EMP_NAME When BANK_EMP_AC_CODE != '' then EMP_NAME else '' END+'#~#'+isnull(EMP_NEW_PAN_NO,'')+'#~#'+CASE When EMP_NEW_PAN_NO != NULL then EMP_NAME When EMP_NEW_PAN_NO != '' then EMP_NAME ELSE '' END+'#~#'+isnull(ADHARNO,'')+'#~#'+Case When ADHARNO != NULL then EMP_NAME When ADHARNO != '' then EMP_NAME Else '' END , PAN_NUMBER As 'UAN NUMBER' FROM pay_employee_master WHERE pay_employee_master.JOINING_DATE > dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER", d1.con1);
            query = "select  isnull (pay_employee_master.PAN_NUMBER,'') + '#~#' + '#~#'+isnull(EMP_NAME,'')+'#~#' + isnull(DATE_FORMAT(BIRTH_DATE,'%d/%m/%Y'),'')+ '#~#' + isnull(DATE_FORMAT(JOINING_DATE,'%d/%m/%Y'),'')+ '#~#' + isnull(pay_employee_master.GENDER,'')+ '#~#' +  isnull(EMP_FATHER_NAME,'')+ '#~#' + CASE WHEN FATHER_RELATION='Father' then 'F' else 'H' END + '#~#' + isnull(EMP_MOBILE_NO,'')+ '#~#' +  ''+ '#~#' + 'INDIAN'+ '#~#' +  ''+ '#~#' +  ''+'#~#'+Case When EMP_MARRITAL_STATUS ='Married' then 'M' When EMP_MARRITAL_STATUS = 'Single' then 'U' When EMP_MARRITAL_STATUS='Divorced' then 'D' When EMP_MARRITAL_STATUS='Widowed' then 'W' Else '' END+'#~#'+'N'+'#~#'+''+'#~#'+''+'#~#'+''+'#~#'+''+'#~#'+'N'+'#~#'+''+'#~#'+''+'#~#'+''+'#~#'+isnull(BANK_EMP_AC_CODE,'')+'#~#'+isnull(PF_IFSC_CODE,'')+'#~#'+ Case When BANK_EMP_AC_CODE != NULL then EMP_NAME When BANK_EMP_AC_CODE != '' then EMP_NAME else '' END+'#~#'+isnull(EMP_NEW_PAN_NO,'')+'#~#'+CASE When EMP_NEW_PAN_NO != NULL then EMP_NAME When EMP_NEW_PAN_NO != '' then EMP_NAME ELSE '' END+'#~#'+isnull(ADHARNO,'')+'#~#'+Case When ADHARNO != NULL then EMP_NAME When ADHARNO != '' then EMP_NAME Else '' END FROM pay_employee_master WHERE (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER";
        }
        else
        {
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);

            adp = new MySqlDataAdapter("select  ifnull (pay_employee_master.PAN_NUMBER,'') + '#~#' + '#~#'+ifnull(EMP_NAME,'')+'#~#' + ifnull(DATE_FORMAT(BIRTH_DATE,'%d/%m/%Y'),'')+ '#~#' + ifnull(DATE_FORMAT(JOINING_DATE,'%d/%m/%Y'),'')+ '#~#' + ifnull(pay_employee_master.GENDER,'')+ '#~#' +  ifnull(EMP_FATHER_NAME,'')+ '#~#' + CASE WHEN FATHER_RELATION='Father' then 'F' else 'H' END + '#~#' + ifnull(EMP_MOBILE_NO,'')+ '#~#' +  ''+ '#~#' + 'INDIAN'+ '#~#' +  ''+ '#~#' +  ''+'#~#'+Case When EMP_MARRITAL_STATUS ='Married' then 'M' When EMP_MARRITAL_STATUS = 'Single' then 'U' When EMP_MARRITAL_STATUS='Divorced' then 'D' When EMP_MARRITAL_STATUS='Widowed' then 'W' Else '' END+'#~#'+'N'+'#~#'+''+'#~#'+''+'#~#'+''+'#~#'+''+'#~#'+'N'+'#~#'+''+'#~#'+''+'#~#'+''+'#~#'+ifnull(BANK_EMP_AC_CODE,'')+'#~#'+ifnull(PF_IFSC_CODE,'')+'#~#'+ Case When BANK_EMP_AC_CODE != NULL then EMP_NAME When BANK_EMP_AC_CODE != '' then EMP_NAME else '' END+'#~#'+ifnull(EMP_NEW_PAN_NO,'')+'#~#'+CASE When EMP_NEW_PAN_NO != NULL then EMP_NAME When EMP_NEW_PAN_NO != '' then EMP_NAME ELSE '' END+'#~#'+ifnull(ADHARNO,'')+'#~#'+Case When ADHARNO != NULL then EMP_NAME When ADHARNO != '' then EMP_NAME Else '' END ,PAN_NUMBER As 'UAN NUMBER' FROM pay_employee_master WHERE (pay_employee_master.UNIT_CODE in (" + UnitList + "))  AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER", d1.con1);
        }
        adp.Fill(dt);

        System.Data.DataTable dt1 = new System.Data.DataTable();
        MySqlDataAdapter adp1;

        string csv = string.Empty;

        // csv = "Member Id,Member Name,EPF Wages,Eps Wages,EPF Contribution,EPF Contribution being remitted,EPS Contribution due,EPS Contribution being remitted,Diff. EPF & EPS Contribution due,Diff EPF & EPS,NCP Days,Refund of Advances,Arrear EPF Wages,Arrear EPF EE Share,Arrear EPF ER Share,Arrear EPS Share,Father Name,Relationship,Date Of Birth,Gender,Joining EPF,Joining EPS,Date of Exit EPF,Date of Exit from EPS,Reason Of Leaving";

        //Add new line.
        //csv += "\r\n";
        // dt.Merge(dt1);
        dt.DefaultView.Sort = "UAN NUMBER";
        dt = dt.DefaultView.ToTable();
        // dt.DefaultView.Sort= "pay_employee_master.PF_NUMBER";
        int a = 0;
        foreach (DataRow row in dt.Rows)
        {
            a = 0;
            foreach (DataColumn column in dt.Columns)
            {
                //Add the Data rows.
                if (a == 0)
                {
                    csv += row[column.ColumnName].ToString();
                    a = 1;
                }
            }

            //Add new line.
            csv += "\r\n";
        }

        //foreach (DataRow row1 in dt1.Rows)
        //{
        //    foreach (DataColumn column1 in dt1.Columns)
        //    {
        //        //Add the Data rows.
        //        csv += row1[column1.ColumnName].ToString();
        //    }

        //    //Add new line.
        //    csv += "\r\n";
        //}

        String Company_name = Session["COMP_NAME"].ToString();
        Company_name = Company_name.Replace(' ', '_');
        //Download the CSV file.
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=UAN_CSV_Download_" + Company_name + ".csv");
        Response.Charset = "";
        Response.ContentType = "application/text";
        Response.Output.Write(csv);
        Response.Flush();
        Response.End();
        //UnitGrid_PF.Visible = false;
        UnitGrid_PF.Visible = true;
        btn_uan_csv.Visible = false;

    }


    //public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
    //{
    //    /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
    //       server control at run time. */
    //}

    protected void btn_uan_xml_Click(object sender, EventArgs e)
    {
        if (UnitGrid_PF.Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=UANDownload.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                UnitGrid_PF.AllowPaging = false;
                UnitBAL ubl1 = new UnitBAL();
                foreach (TableCell cell in UnitGrid_PF.HeaderRow.Cells)
                {
                    cell.BackColor = UnitGrid_PF.HeaderStyle.BackColor;
                }
                foreach (GridViewRow row in UnitGrid_PF.Rows)
                {

                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = UnitGrid_PF.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = UnitGrid_PF.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                UnitGrid_PF.RenderControl(hw);

                //style to format numbers to string
                string style = @"<style> .textmode { } </style>";
                Response.Write(style);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }
    }
    protected void btn_UAN_Click(object sender, EventArgs e)
    {
        d1.con1.Open();
        String currMonth = Session["CURRENT_MONTH"].ToString();
        String currYear = Session["CURRENT_YEAR"].ToString();
        String UnitList = "";
        string strQuery = "";
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            strQuery = "SELECT  pay_employee_master.PAN_NUMBER AS 'UAN Number', '' as 'Previous Member Id' , EMP_NAME as 'Member Name',DATE_FORMAT(BIRTH_DATE,'%d/%m/%Y') as 'BIRTH DATE' , DATE_FORMAT(JOINING_DATE,'%d/%m/%Y') as 'Date of Joining', GENDER as 'Gender' , EMP_FATHER_NAME as 'Father/Husband Name' , CASE  When FATHER_RELATION='Father' then 'F' else 'H' END as 'Relationship with the member',isnull(EMP_MOBILE_NO,'') as 'Mobile Number' , '' as 'Emailid', 'INDIAN' as 'Nationality' , '' as  'Wages as on Joining', EMP_QUALIFICATION as 'Qualification',Case When EMP_MARRITAL_STATUS='Married' then 'M' else ''END as 'Marital Status' , 'N' as 'Is International Worker','' as 'Country Of Origin','' as 'Passport Number','' as 'Passport Valid From Date','' as 'Passport Valid Till Date', 'N' as 'Is Physical Handicap', '' as 'Locomotive', '' as 'Hearing' , '' as 'Visual',  BANK_EMP_AC_CODE as 'Bank Account Number', PF_IFSC_CODE as 'Bank IFSC',case When BANK_EMP_AC_CODE != '' then EMP_NAME When BANK_EMP_AC_CODE != null then EMP_NAME else '' END as 'Name as Per Bank Details',EMP_NEW_PAN_NO as 'PAN' , case When EMP_NEW_PAN_NO != null then EMP_NAME When EMP_NEW_PAN_NO !='' then EMP_NAME else '' END as 'Name as on PAN',ADHARNO as 'AADHAAR Number', CASE WHEN ADHARNO != NULL then EMP_NAME When ADHARNO != '' then EMP_NAME else ''  END as 'Name as on AADHAAR'  from  pay_employee_master WHERE pay_employee_master.JOINING_DATE > dateadd(day,datediff(day,1,(Cast('" + getmonthint(currMonth) + "/01/" + currYear + "' as date))),0) AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER";

        }
        else
        {
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);

            strQuery = "SELECT  pay_employee_master.PAN_NUMBER AS 'UAN Number', '' as 'Previous Member Id' , EMP_NAME as 'Member Name',DATE_FORMAT(BIRTH_DATE,'%d/%m/%Y') as 'BIRTH DATE' , DATE_FORMAT(JOINING_DATE,'%d/%m/%Y') as 'Date of Joining' , GENDER as 'Gender' , EMP_FATHER_NAME as 'Father/Husband Name' , CASE  When FATHER_RELATION='Father' then 'F' else 'H' END as 'Relationship with the member',case when EMP_MOBILE_NO <> '' then EMP_MOBILE_NO  when EMP_MOBILE_NO <> null then EMP_MOBILE_NO else '' END as 'Mobile Number' , '' as 'Emailid', 'INDIAN' as 'Nationality' , '' as  'Wages as on Joining', EMP_QUALIFICATION as 'Qualification',Case When EMP_MARRITAL_STATUS='Married' then 'M' else ''END as 'Marital Status' , 'N' as 'Is International Worker','' as 'Country Of Origin','' as 'Passport Number','' as 'Passport Valid From Date','' as 'Passport Valid Till Date', 'N' as 'Is Physical Handicap', '' as 'Locomotive', '' as 'Hearing' , '' as 'Visual',  BANK_EMP_AC_CODE as 'Bank Account Number', PF_IFSC_CODE as 'Bank IFSC',case When BANK_EMP_AC_CODE != '' then EMP_NAME When BANK_EMP_AC_CODE != null then EMP_NAME else '' END as 'Name as Per Bank Details',EMP_NEW_PAN_NO as 'PAN' , case When EMP_NEW_PAN_NO != null then EMP_NAME When EMP_NEW_PAN_NO !='' then EMP_NAME else '' END as 'Name as on PAN',ADHARNO as 'AADHAAR Number', CASE WHEN ADHARNO != NULL then EMP_NAME When ADHARNO != '' then EMP_NAME else ''  END as 'Name as on AADHAAR'  FROM pay_employee_master WHERE (pay_employee_master.UNIT_CODE in (" + UnitList + "))  AND (pay_employee_master.LEFT_REASON is null or pay_employee_master.LEFT_REASON = '') and pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' order by pay_employee_master.PAN_NUMBER";
        }

        MySqlDataAdapter adapter = new MySqlDataAdapter(strQuery, d1.con1);
        DataSet ds = new DataSet();
        adapter.Fill(ds);
        UnitGrid_PF.AutoGenerateColumns = true;
        UnitGrid_PF.DataSource = ds;

        UnitGrid_PF.DataBind();

        UnitGrid_PF.Visible = true;
        btn_uan_csv.Visible = true;
        btn_uan_xml.Visible = true;
        d1.con1.Close();

    }
    protected void btn_monthly_mlwf_Click(object sender, EventArgs e)
    {

        string query = null;
        //  int length = ddlunitselect.SelectedValue.Length;
        string downloadname = "LWF_Chalan";
        crystalReport.Load(Server.MapPath("~/form_A-1_return.rpt"));

        if (ddlunitselect.Text == "ALL")
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.PRESENT_DAYS>0 AND   pay_attendance.CPF_SHEET='Yes' and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'   ";
        }
        else
        {
            query = "  SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE,  pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05,  pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance.PRESENT_DAYS, pay_attendance.LEAVE_DAYS, pay_attendance.PAYABLE_DAYS, pay_attendance.OT_HRS, pay_attendance.L_HEAD01, pay_attendance.L_HEAD02, pay_attendance.L_HEAD03, pay_attendance.L_HEAD04, pay_attendance.D_HEAD01, pay_attendance.D_LOAN, pay_attendance.D_HEAD02, pay_attendance.D_HEAD03, pay_attendance.D_HEAD04, pay_attendance.D_HEAD05, pay_attendance.INCOMETAX, pay_attendance.C_HEAD01, pay_attendance.C_HEAD02, pay_attendance.C_HEAD03, pay_attendance.C_HEAD04, pay_attendance.C_HEAD05, pay_attendance.C_HEAD06, pay_attendance.C_HEAD07, pay_attendance.C_HEAD08, pay_attendance.C_HEAD09, pay_attendance.C_HEAD10, pay_attendance.C_HEAD11, pay_attendance.C_HEAD12, pay_attendance.PF, pay_attendance.ESIC_TOT, pay_attendance.OT_GROSS, pay_attendance.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance.TOT_ESIC_GROSS, pay_attendance.ESIC_COMP_CONTRI, pay_attendance.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance.D_HEAD06, pay_attendance.D_HEAD07, pay_attendance.D_HEAD08, pay_attendance.D_HEAD09, pay_attendance.CGRADE_CODE, pay_attendance.EXTRA_DAYS, pay_attendance.EXTRA_GROSS, pay_attendance.UNIT_CODE,pay_attendance.CPF_SHEET FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance.comp_code = pay_grade_master.comp_code  WHERE pay_attendance.PRESENT_DAYS>0 AND   pay_attendance.CPF_SHEET='Yes' and pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'   ";
            //query = " SELECT pay_company_master.comp_code, pay_company_master.COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, pay_company_master.PIN, pay_company_master.PF_REG_NO, pay_company_master.ESIC_REG_NO, pay_company_master.E_HEAD01 AS HEAD01, pay_company_master.E_HEAD02 AS HEAD02, pay_company_master.E_HEAD03 AS HEAD03, pay_company_master.E_HEAD04 AS HEAD04, pay_company_master.E_HEAD05 AS HEAD05, pay_company_master.E_HEAD06 AS HEAD06, pay_company_master.E_HEAD07 AS HEAD07, pay_company_master.E_HEAD08 AS HEAD08, pay_company_master.E_HEAD09 AS HEAD09, pay_company_master.E_HEAD10 AS HEAD10, pay_company_master.E_HEAD11 AS HEAD11, pay_company_master.E_HEAD12 AS HEAD12, pay_company_master.L_HEAD01 AS LHEAD01, pay_company_master.L_HEAD02 AS LHEAD02, pay_company_master.L_HEAD03 AS LHEAD03, pay_company_master.L_HEAD04 AS LHEAD04, pay_company_master.D_HEAD01 AS DHEAD01, pay_company_master.D_HEAD02 AS DHEAD02, pay_company_master.D_HEAD03 AS DHEAD03, pay_company_master.D_HEAD04 AS DHEAD04, pay_company_master.D_HEAD05 AS DHEAD05, pay_company_master.CURRENT_MONTH, pay_company_master.CURRENT_YEAR, pay_employee_master.EMP_CODE, pay_employee_master.EMP_NAME, pay_employee_master.EMP_FATHER_NAME, pay_employee_master.JOINING_DATE, pay_employee_master.GENDER, pay_employee_master.BANK_EMP_AC_CODE, pay_employee_master.BANK_CODE, pay_employee_master.STATUS, pay_employee_master.PF_NUMBER, pay_employee_master.E_HEAD01, pay_employee_master.E_HEAD02, pay_employee_master.E_HEAD03, pay_employee_master.E_HEAD04, pay_employee_master.E_HEAD05, pay_employee_master.E_HEAD06, pay_employee_master.E_HEAD07, pay_employee_master.E_HEAD08, pay_employee_master.E_HEAD09, pay_employee_master.E_HEAD10, pay_employee_master.E_HEAD11, pay_employee_master.E_HEAD12, pay_employee_master.PF_SHEET, pay_employee_master.EARN_TOTAL, pay_employee_master.LEFT_REASON, pay_unit_master.UNIT_NAME, pay_unit_master.UNIT_ADD1, pay_unit_master.UNIT_ADD2, pay_unit_master.UNIT_CITY, pay_attendance_history.PRESENT_DAYS, pay_attendance_history.LEAVE_DAYS, pay_attendance_history.PAYABLE_DAYS, pay_attendance_history.OT_HRS, pay_attendance_history.L_HEAD01, pay_attendance_history.L_HEAD02, pay_attendance_history.L_HEAD03, pay_attendance_history.L_HEAD04, pay_attendance_history.D_HEAD01, pay_attendance_history.D_LOAN, pay_attendance_history.D_HEAD02, pay_attendance_history.D_HEAD03, pay_attendance_history.D_HEAD04, pay_attendance_history.D_HEAD05, pay_attendance_history.INCOMETAX, pay_attendance_history.C_HEAD01, pay_attendance_history.C_HEAD02, pay_attendance_history.C_HEAD03, pay_attendance_history.C_HEAD04, pay_attendance_history.C_HEAD05, pay_attendance_history.C_HEAD06, pay_attendance_history.C_HEAD07, pay_attendance_history.C_HEAD08, pay_attendance_history.C_HEAD09, pay_attendance_history.C_HEAD10, pay_attendance_history.C_HEAD11, pay_attendance_history.C_HEAD12, pay_attendance_history.PF, pay_attendance_history.ESIC_TOT, pay_attendance_history.OT_GROSS, pay_attendance_history.PTAX, pay_grade_master.GRADE_DESC, pay_employee_master.ESIC_NUMBER, pay_attendance_history.TOT_ESIC_GROSS, pay_attendance_history.ESIC_COMP_CONTRI, pay_attendance_history.MLWF, pay_company_master.D_HEAD06 AS DHEAD06, pay_company_master.D_HEAD07 AS DHEAD07, pay_company_master.D_HEAD08 AS DHEAD08, pay_company_master.D_HEAD09 AS DHEAD09, pay_attendance_history.D_HEAD06, pay_attendance_history.D_HEAD07, pay_attendance_history.D_HEAD08, pay_attendance_history.D_HEAD09, pay_attendance_history.CGRADE_CODE, pay_attendance_history.EXTRA_DAYS, pay_attendance_history.EXTRA_GROSS, pay_attendance_history.UNIT_CODE,pay_attendance_history.CPF_SHEET FROM pay_attendance_history INNER JOIN pay_employee_master ON pay_attendance_history.comp_code = pay_employee_master.comp_code AND pay_attendance_history.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_company_master ON pay_attendance_history.comp_code = pay_company_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_history.comp_code = pay_unit_master.comp_code AND pay_attendance_history.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_grade_master ON pay_attendance_history.CGRADE_CODE = pay_grade_master.GRADE_CODE AND pay_attendance_history.comp_code = pay_grade_master.comp_code  WHERE pay_attendance_history.PRESENT_DAYS>0 AND  pay_attendance_history.CPF_SHEET='Yes' AND pay_company_master.comp_code='" + Session["comp_code"].ToString() + "'  AND pay_attendance_history.UNIT_CODE='" + ddlunitselect.SelectedValue.ToString().Substring(0, 4) + "'  AND BETWEEN (CAL_MONTH='APR',CAL_YEAR='2014' AND CAL_MONTH='JUL',CAL_YEAR='2015')";


        }
        Session["ReportMonthNo"] = "33";
        ReportLoad(query, downloadname);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btn_pf_return_Click(object sender, EventArgs e)
    {

        d1.con1.Open();

        String currMonth = Session["CURRENT_MONTH"].ToString();
        String currYear = Session["CURRENT_YEAR"].ToString();
        string downloadname = "PF_Return";
        string query = null;
        string UnitList = "";
        crystalReport.Load(Server.MapPath("~/PFReturn_Form.rpt"));
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            // query = "Select pay_company_master.COMPANY_NAME,pay_company_master.COMPANY_TAN_NO,pay_company_master.CURRENT_YEAR, pay_company_master.ADDRESS1,pay_company_master.ADDRESS2,pay_company_master.CITY,pay_company_master.PIN,day(DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1,-1)),month(DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1,-1)),year(DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1,-1)),pay_attendance.PTAX,pay_employee_master.GENDER,pay_employee_master.PF_BANK_NAME,pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code   WHERE  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' ORDER BY pay_employee_master.EMP_CODE asc";
            query = "Select pay_company_master.COMPANY_NAME, pay_company_master.COMPANY_TAN_NO,year(DATE_ADD(now(), interval 0 year)) as 'CURRENT_YEAR',pay_unit_master.UNIT_ADD1 AS ADDRESS1 ,pay_unit_master.UNIT_ADD2 AS ADDRESS2 ,pay_unit_master.UNIT_CITY AS CITY, DAYOFMONTH(LAST_DAY(DATE_ADD(now(), interval -1 month))) as 'day',month(date_add(now(),interval -1 month)) as 'month',year(DATE_ADD(now(), interval 0 year)) as 'year',pay_attendance.PTAX,pay_employee_master.GENDER,pay_employee_master.PF_BANK_NAME,pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME FROM  pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code WHERE  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' ORDER BY pay_employee_master.EMP_CODE asc";
        }
        else
        {
            foreach (ListItem listItem in ddlunitselect.Items)
            {
                if (listItem.Selected == true)
                {
                    UnitList += "'" + listItem.Text.Substring(0, 4) + "',";
                }
            }
            UnitList = UnitList.Substring(0, UnitList.Length - 1);
            //query = "Select pay_company_master.COMPANY_NAME, pay_company_master.COMPANY_TAN_NO,pay_company_master.CURRENT_YEAR, pay_unit_master.UNIT_ADD1 AS ADDRESS1 ,pay_unit_master.UNIT_ADD2 AS ADDRESS2 ,pay_unit_master.UNIT_CITY AS CITY ,pay_company_master.PIN,day(DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1,-1)),month(DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1,-1)),year(DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1,-1)),pay_attendance.PTAX,pay_employee_master.GENDER,pay_employee_master.PF_BANK_NAME,pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME FROM            pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code   WHERE  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.UNIT_CODE in (" + UnitList + ") ORDER BY pay_employee_master.EMP_CODE asc";
            query = "Select pay_company_master.COMPANY_NAME, pay_company_master.COMPANY_TAN_NO,year(DATE_ADD(now(), interval 0 year)) as 'CURRENT_YEAR',pay_unit_master.UNIT_ADD1 AS ADDRESS1 ,pay_unit_master.UNIT_ADD2 AS ADDRESS2 ,pay_unit_master.UNIT_CITY AS CITY, DAYOFMONTH(LAST_DAY(DATE_ADD(now(), interval -1 month))) as 'day',month(date_add(now(),interval -1 month)) as 'month',year(DATE_ADD(now(), interval 0 year)) as 'year',pay_attendance.PTAX,pay_employee_master.GENDER,pay_employee_master.PF_BANK_NAME,pay_attendance.UNIT_CODE, pay_unit_master.UNIT_NAME FROM  pay_attendance INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE INNER JOIN pay_company_master ON pay_attendance.comp_code = pay_company_master.comp_code  WHERE  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.UNIT_CODE in (" + UnitList + ") ORDER BY pay_employee_master.EMP_CODE asc";
        }
        Session["ReportMonthNo"] = "34";
        ReportLoad(query, downloadname);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    protected void btn_bank_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btn_bank.BackColor = System.Drawing.Color.GreenYellow;


        d.con1.Open();
        string query = "";
        string squery = "";
        //int length = ddl_unitcode.SelectedValue.Length;
        string strallexcelout = Session["AllExcelOut"].ToString();
        if (ddlunitselect.SelectedValue.ToString() == "ALL")
        {
            query = "Select  ( Case when NFD_CODE='NFD' then 'N' else 'Y' end ) AS NFD_DETAILS  , EARN_TOTAL AS AMOUNT,sysdate() AS DATE,EMP_NAME AS EMPLOYEE_NAME,BANK_EMP_AC_CODE AS EMPLOYEE_ACCOUNT_NO, '' as Blank_Field,'' as Blank_Field,BANK_ACCCOUNT_NO AS BANK_ACCOUNT_NO,'' as Blank_Field,PF_IFSC_CODE AS IFCS_CODE,'' as Blank_Field,'11' as 'Blank_Field' from pay_employee_master   INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code WHERE pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' and  NFD_CODE='NFD'  ";
            //query = "SELECT pay_client_master.CLIENT_NAME, pay_unit_master.state_name, pay_unit_master.ZONE,pay_employee_master.GRADE_CODE,pay_company_master.ESIC_REG_NO,pay_employee_master.EMP_NAME, pay_attendance.TOT_ESIC_GROSS,pay_attendance.PRESENT_DAYS as WORKING_DAYS,pay_attendance.PAYABLE_DAYS AS CALCULATION_OF_WOEKING_DAYS,pay_attendance.ESIC_TOT AS EMPLOYRR_E_CONTRIBUTION,pay_attendance.ESIC_COMP_CONTRI AS EMPLOYEE_R_CONTRIBUTION, ( pay_attendance.ESIC_TOT +pay_attendance.ESIC_COMP_CONTRI) as TOTAL FROM pay_company_master INNER JOIN pay_attendance ON pay_company_master.comp_code = pay_attendance.comp_code INNER JOIN pay_client_master ON pay_attendance.comp_code = pay_client_master.comp_code INNER JOIN pay_employee_master ON pay_attendance.comp_code = pay_employee_master.comp_code AND pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE pay_attendance.CPF_SHEET='Yes' AND pay_attendance.ESIC_TOT>0 AND  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "' ORDER BY pay_employee_master.ESIC_NUMBER ASC";
        }
        else
        {
            query = "Select  ( Case when NFD_CODE='NFD' then 'N' else 'Y' end ) AS NFD_DETAILS  , EARN_TOTAL AS AMOUNT,sysdate() AS DATE,EMP_NAME AS EMPLOYEE_NAME,BANK_EMP_AC_CODE AS EMPLOYEE_ACCOUNT_NO, '' as Blank_Field,'' as Blank_Field,BANK_ACCCOUNT_NO AS BANK_ACCOUNT_NO,'' as Blank_Field,PF_IFSC_CODE AS IFCS_CODE,'' as Blank_Field,'11' as 'Blank_Field' from pay_employee_master   INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code WHERE pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' and  NFD_CODE='NFD'  and pay_employee_master.UNIT_CODE ='" + ddlunitselect.SelectedItem.Text.Substring(0, 4) + "' ";
        }
        MySqlCommand cmd = new MySqlCommand(query, d.con1);
        DataSet ds = new DataSet();
        MySqlDataAdapter adp = new MySqlDataAdapter(query, d.con1);
        adp.Fill(ds);
        gv_bankexcel.DataSource = ds.Tables[0];
        gv_bankexcel.DataBind();
        d.con1.Close();
        panel_bankexcel.Visible = true;
        panel_esic_statement.Visible = false;
        panel_employee_pf_esic_no.Visible = false;
        panel_employee_information_status.Visible = false;
        panel_esic_summary_utwise.Visible = false;
        panel_ptax.Visible = false;



        //Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
        //Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
        //Worksheet ws = (Worksheet)xla.ActiveSheet;
        //xla.Columns.ColumnWidth = 20;



        //ws.Cells[1, 1] = "INFT Yes /No";
        //ws.Cells[1, 2] = "Salary Amount";
        //ws.Cells[1, 3] = "System Current Date";
        //ws.Cells[1, 4] = "Employee Holder Name";
        //ws.Cells[1, 5] = "Employee Account no";
        //ws.Cells[1, 6] = "Blank Field";
        //ws.Cells[1, 7] = "Blank Field";
        //ws.Cells[1, 8] = "company account no";
        //ws.Cells[1, 9] = "Blank Field";
        //ws.Cells[1, 10] = "Employee ifsc code";
        //ws.Cells[1, 11] = "Blank Field";

        //try
        //{
        //    d.con1.Open();
        //    MySqlDataAdapter adp2 = new MySqlDataAdapter("Select  ( Case when NFD_CODE='NFD' then 'N' else 'Y' end )  , Amount,sysdate(),EMP_NAME,BANK_EMP_AC_CODE, '' as Blank_Field,'' as Blank_Field,BANK_ACCCOUNT_NO,'' as Blank_Field,PF_IFSC_CODE,'' as Blank_Field,'11' as '11' from pay_employee_master   INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code WHERE pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' and  NFD_CODE='NFD'  ", d.con1);
        //       System.Data.DataTable dt = new System.Data.DataTable();
        //          adp2.Fill(dt);
        //    int j = 2;
        //    foreach (System.Data.DataRow row in dt.Rows)
        //    {
        //        for (int i = 0; i < dt.Columns.Count; i++)
        //        {
        //            ws.Cells[j, i + 1] = row[i].ToString();
        //        }
        //        j++;
        //    }
        //    xla.Visible = true;
        //}
        //catch (Exception ee)
        //{
        //    Response.Write(ee.Message);
        //}
        //finally
        //{
        //    d.con1.Close();
        //}
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }

    }

    protected void btnesicexcel_Click(object sender, EventArgs e)
    {
        string query = null;
        string downloadname = " ESIC_Statement_Excel";
        crystalReport.Load(Server.MapPath("~/ESIC_format.rpt"));
        if (ddlunitselect.Text == "ALL")
        {
            query = "Select pay_employee_master.ESIC_NUMBER,pay_employee_master.EMP_NAME,pay_attendance.PRESENT_DAYS,pay_attendance.ESIC_GROSS,'' As 'Zero Working days',case when pay_employee_master.LEFT_DATE <= pay_employee_master.JOINING_DATE then null else pay_employee_master.LEFT_DATE END As 'Left Date' from pay_attendance INNER JOIN pay_employee_master ON pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE AND pay_attendance.UNIT_CODE = pay_employee_master.UNIT_CODE AND pay_attendance.comp_code = pay_employee_master.comp_code Where pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";
        }
        else
        {
            query = "Select pay_employee_master.ESIC_NUMBER,pay_employee_master.EMP_NAME,pay_attendance.PRESENT_DAYS,pay_attendance.ESIC_GROSS,'' As 'Zero Working days',case when pay_employee_master.LEFT_DATE <= pay_employee_master.JOINING_DATE then null else pay_employee_master.LEFT_DATE END As 'Left Date' from pay_attendance INNER JOIN pay_employee_master ON pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE AND pay_attendance.UNIT_CODE = pay_employee_master.UNIT_CODE AND pay_attendance.comp_code = pay_employee_master.comp_code Where pay_employee_master.UNIT_CODE ='" + ddlunitselect.SelectedItem.Text.Substring(0, 4) + "' AND pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_attendance.MONTH = '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR = '" + txttodate.Text.Substring(3) + "'";

        }
        Reportformat(query, downloadname);

        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }

    }

    protected void btnsalaryexcel_Click(object sender, EventArgs e)
    {
        ButtonColor();
        btnsalaryexcel.BackColor = System.Drawing.Color.GreenYellow;
        try
        {
            string ehead01 = "-", ehead02 = "-", ehead03 = "-", ehead04 = "-", ehead05 = "-", ehead06 = "-", ehead07 = "-", ehead08 = "-", ehead09 = "-", ehead10 = "-", ehead11 = "-", ehead12 = "-", ehead13 = "-", ehead14 = "-", ehead15 = "-";
            string lhead01 = "-", lhead02 = "-", lhead03 = "-", lhead04 = "-", lhead05 = "-";
            string dhead01 = "-", dhead02 = "-", dhead03 = "-", dhead04 = "-", dhead05 = "-", dhead06 = "-", dhead07 = "-", dhead08 = "-", dhead09 = "-", dhead10 = "-";

            MySqlDataAdapter da = null;
            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                ehead01 = dr["E_HEAD01"].ToString();
                ehead02 = dr["E_HEAD02"].ToString();
                ehead03 = dr["E_HEAD03"].ToString();
                ehead04 = dr["E_HEAD04"].ToString();
                ehead05 = dr["E_HEAD05"].ToString();
                ehead06 = dr["E_HEAD06"].ToString();
                ehead07 = dr["E_HEAD07"].ToString();
                ehead08 = dr["E_HEAD08"].ToString();
                ehead09 = dr["E_HEAD09"].ToString();
                ehead10 = dr["E_HEAD10"].ToString();
                ehead11 = dr["E_HEAD11"].ToString();
                ehead12 = dr["E_HEAD12"].ToString();
                ehead13 = dr["E_HEAD13"].ToString();
                ehead14 = dr["E_HEAD14"].ToString();
                ehead15 = dr["E_HEAD15"].ToString();

                lhead01 = dr["L_HEAD01"].ToString();
                lhead02 = dr["L_HEAD02"].ToString();
                lhead03 = dr["L_HEAD03"].ToString();
                lhead04 = dr["L_HEAD04"].ToString();
                lhead05 = dr["L_HEAD05"].ToString();

                dhead01 = dr["D_HEAD01"].ToString();
                dhead02 = dr["D_HEAD02"].ToString();
                dhead03 = dr["D_HEAD03"].ToString();
                dhead04 = dr["D_HEAD04"].ToString();
                dhead05 = dr["D_HEAD05"].ToString();
                dhead06 = dr["D_HEAD06"].ToString();
                dhead07 = dr["D_HEAD07"].ToString();
                dhead08 = dr["D_HEAD08"].ToString();
                dhead09 = dr["D_HEAD09"].ToString();
                dhead10 = dr["D_HEAD010"].ToString();


                d.con1.Open();
                string query = "";
                string squery = "";
                //int length = ddl_unitcode.SelectedValue.Length;
                string strallexcelout = Session["AllExcelOut"].ToString();

                //  query = "SELECT  pay_attendance.EMP_CODE, pay_employee_master.EMP_NAME, pay_attendance.CGRADE_CODE AS  GRADE_CODE , pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS,  pay_attendance.C_HEAD01 AS '" + ehead01.ToString() + "', pay_attendance.C_HEAD02 AS '" + ehead02.ToString() + "', pay_attendance.C_HEAD03 AS '" + ehead03.ToString() + "', pay_attendance.C_HEAD04 AS '" + ehead04.ToString() + "', pay_attendance.C_HEAD05 AS '" + ehead05.ToString() + "', pay_attendance.C_HEAD06 AS '" + ehead06.ToString() + "', pay_attendance.C_HEAD07 AS '" + ehead07.ToString() + "',pay_attendance.C_HEAD08 AS '" + ehead08.ToString() + "',pay_attendance.C_HEAD09 AS '" + ehead09.ToString() + "',pay_attendance.C_HEAD10 AS '" + ehead10.ToString() + "' , pay_attendance.C_HEAD11 AS '" + ehead11.ToString() + "' , pay_attendance.C_HEAD12 AS '" + ehead12.ToString() + "' ,pay_attendance.OT_GROSS,pay_attendance.L_HEAD01 AS  '" + lhead01 + "',pay_attendance.L_HEAD02 AS  '" + lhead02 + "' ,pay_attendance.L_HEAD03 AS  '" + lhead03 + "',pay_attendance.L_HEAD04 AS  '" + lhead04 + "',(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.OT_GROSS+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04) AS GROSS, pay_attendance.PF, pay_attendance.PTAX,pay_attendance.ESIC_TOT,MLWF AS LWF,(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX) AS DEDUCT,round(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS-pay_attendance.PF- pay_attendance.PTAX-pay_attendance.ESIC_TOT-pay_attendance.MLWF-(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX),0) AS NETTAMOUNT ,EMP_FATHER_NAME AS FATHER,(CASE WHEN (EMP_FATHER_NAME='' OR EMP_FATHER_NAME IS NULL) THEN '****' ELSE '' END )AS SIGNATURE, pay_employee_master.PF_BANK_NAME,pay_employee_master.PF_IFSC_CODE,pay_employee_master.BANK_EMP_AC_CODE FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN  pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE( pay_attendance.PRESENT_DAYS>0  OR  pay_attendance.OT_HRS>0) AND  (pay_attendance.comp_code = '" + Session["comp_code"].ToString() + "') AND (pay_attendance.UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "')  AND pay_attendance.MONTH= '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR='" + txttodate.Text.Substring(3, 4) + "' ORDER BY pay_employee_master.EMP_CODE";
                if (ddlunitselect.SelectedItem.Text == "ALL")
                {
                    query = "SELECT  pay_attendance.EMP_CODE, pay_employee_master.EMP_NAME,case pay_employee_master.GENDER when 'M' then 'MALE' when 'F' then 'FEMALE' end as 'GENDER' , pay_attendance.CGRADE_CODE AS  GRADE_CODE ,pay_department_master.DEPT_NAME , pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS,  pay_attendance.C_HEAD01 AS '" + ehead01.ToString() + "', pay_attendance.C_HEAD02 AS '" + ehead02.ToString() + "', pay_attendance.C_HEAD03 AS '" + ehead03.ToString() + "', pay_attendance.C_HEAD04 AS '" + ehead04.ToString() + "', pay_attendance.C_HEAD05 AS '" + ehead05.ToString() + "', pay_attendance.C_HEAD06 AS '" + ehead06.ToString() + "', pay_attendance.C_HEAD07 AS '" + ehead07.ToString() + "',pay_attendance.C_HEAD08 AS '" + ehead08.ToString() + "',pay_attendance.C_HEAD09 AS '" + ehead09.ToString() + "',pay_attendance.C_HEAD10 AS '" + ehead10.ToString() + "' , pay_attendance.C_HEAD11 AS '" + ehead11.ToString() + "' , pay_attendance.C_HEAD12 AS '" + ehead12.ToString() + "' ,pay_attendance.OT_GROSS,pay_attendance.L_HEAD01 AS  '" + lhead01 + "',pay_attendance.L_HEAD02 AS  '" + lhead02 + "' ,pay_attendance.L_HEAD03 AS  '" + lhead03 + "',pay_attendance.L_HEAD04 AS  '" + lhead04 + "',(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.OT_GROSS+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04) AS GROSS, pay_attendance.PF, pay_attendance.PTAX,pay_attendance.ESIC_TOT,MLWF AS LWF,(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX) AS DEDUCT,round(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS-pay_attendance.PF- pay_attendance.PTAX-pay_attendance.ESIC_TOT-pay_attendance.MLWF-(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX),0) AS NETTAMOUNT ,EMP_FATHER_NAME AS FATHER,(CASE WHEN (EMP_FATHER_NAME='' OR EMP_FATHER_NAME IS NULL) THEN '****' ELSE '' END )AS SIGNATURE, pay_employee_master.PF_BANK_NAME,pay_employee_master.PF_IFSC_CODE,pay_employee_master.BANK_EMP_AC_CODE FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN  pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code inner join pay_department_master on pay_employee_master.DEPT_CODE=pay_department_master.DEPT_CODE AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE( pay_attendance.PRESENT_DAYS>0  OR  pay_attendance.OT_HRS>0) AND  (pay_attendance.comp_code = '" + Session["comp_code"].ToString() + "')   AND pay_attendance.MONTH= '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR='" + txttodate.Text.Substring(3, 4) + "' ORDER BY pay_employee_master.EMP_CODE";
                    //  squery = "SELECT  pay_attendance.CGRADE_CODE AS  GRADE_CODE , SUM(pay_attendance.PRESENT_DAYS) AS PRESENT_DAYS , SUM(pay_attendance.OT_HRS) AS OT_HRS,  SUM(pay_attendance.C_HEAD01) AS '" + ehead01.ToString() + "', SUM(pay_attendance.C_HEAD02) AS '" + ehead02.ToString() + "', SUM(pay_attendance.C_HEAD03) AS '" + ehead03.ToString() + "', SUM(pay_attendance.C_HEAD04) AS '" + ehead04.ToString() + "', SUM(pay_attendance.C_HEAD05) AS '" + ehead05.ToString() + "', SUM(pay_attendance.C_HEAD06) AS '" + ehead06.ToString() + "', SUM(pay_attendance.C_HEAD07) AS '" + ehead07.ToString() + "',SUM(pay_attendance.C_HEAD08) AS '" + ehead08.ToString() + "',SUM(pay_attendance.C_HEAD09) AS '" + ehead09.ToString() + "',SUM(pay_attendance.C_HEAD10) AS '" + ehead10.ToString() + "',  SUM(pay_attendance.C_HEAD11) AS '" + ehead11.ToString() + "',SUM(pay_attendance.C_HEAD12) AS '" + ehead12.ToString() + "' ,SUM(pay_attendance.OT_GROSS) AS OT_GROSS,SUM(pay_attendance.L_HEAD01) AS '" + lhead01 + "',SUM(pay_attendance.L_HEAD02) AS '" + lhead02 + "',SUM(pay_attendance.L_HEAD03) AS '" + lhead03 + "',SUM(pay_attendance.L_HEAD04) AS '" + lhead04 + "', SUM(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.OT_GROSS+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04) AS GROSS, SUM(pay_attendance.PF) AS PF , SUM(pay_attendance.PTAX) AS PTAX,SUM(pay_attendance.ESIC_TOT) AS ESIC ,SUM(MLWF) AS LWF,SUM(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX) AS DEDUCT,SUM(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS-pay_attendance.PF- pay_attendance.PTAX-pay_attendance.ESIC_TOT-pay_attendance.MLWF-(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX)) AS NETTAMOUNT  FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN  pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE( pay_attendance.PRESENT_DAYS>0  OR  pay_attendance.OT_HRS>0) AND (pay_attendance.comp_code = '" + Session["comp_code"].ToString() + "') AND (pay_attendance.UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "')  AND pay_attendance.MONTH= '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR='" + txttodate.Text.Substring(3, 4) + "' GROUP BY pay_attendance.CGRADE_CODE";
                }
                else
                {
                    query = "SELECT  pay_attendance.EMP_CODE, pay_employee_master.EMP_NAME,case pay_employee_master.GENDER when 'M' then 'MALE' when 'F' then 'FEMALE' end as 'GENDER' , pay_attendance.CGRADE_CODE AS  GRADE_CODE ,pay_department_master.DEPT_NAME , pay_attendance.PRESENT_DAYS, pay_attendance.OT_HRS,  pay_attendance.C_HEAD01 AS '" + ehead01.ToString() + "', pay_attendance.C_HEAD02 AS '" + ehead02.ToString() + "', pay_attendance.C_HEAD03 AS '" + ehead03.ToString() + "', pay_attendance.C_HEAD04 AS '" + ehead04.ToString() + "', pay_attendance.C_HEAD05 AS '" + ehead05.ToString() + "', pay_attendance.C_HEAD06 AS '" + ehead06.ToString() + "', pay_attendance.C_HEAD07 AS '" + ehead07.ToString() + "',pay_attendance.C_HEAD08 AS '" + ehead08.ToString() + "',pay_attendance.C_HEAD09 AS '" + ehead09.ToString() + "',pay_attendance.C_HEAD10 AS '" + ehead10.ToString() + "' , pay_attendance.C_HEAD11 AS '" + ehead11.ToString() + "' , pay_attendance.C_HEAD12 AS '" + ehead12.ToString() + "' ,pay_attendance.OT_GROSS,pay_attendance.L_HEAD01 AS  '" + lhead01 + "',pay_attendance.L_HEAD02 AS  '" + lhead02 + "' ,pay_attendance.L_HEAD03 AS  '" + lhead03 + "',pay_attendance.L_HEAD04 AS  '" + lhead04 + "',(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.OT_GROSS+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04) AS GROSS, pay_attendance.PF, pay_attendance.PTAX,pay_attendance.ESIC_TOT,MLWF AS LWF,(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX) AS DEDUCT,round(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS-pay_attendance.PF- pay_attendance.PTAX-pay_attendance.ESIC_TOT-pay_attendance.MLWF-(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX),0) AS NETTAMOUNT ,EMP_FATHER_NAME AS FATHER,(CASE WHEN (EMP_FATHER_NAME='' OR EMP_FATHER_NAME IS NULL) THEN '****' ELSE '' END )AS SIGNATURE, pay_employee_master.PF_BANK_NAME,pay_employee_master.PF_IFSC_CODE,pay_employee_master.BANK_EMP_AC_CODE FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN  pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code inner join pay_department_master on pay_employee_master.DEPT_CODE=pay_department_master.DEPT_CODE AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE( pay_attendance.PRESENT_DAYS>0  OR  pay_attendance.OT_HRS>0) AND  (pay_attendance.comp_code = '" + Session["comp_code"].ToString() + "') AND (pay_attendance.UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "')  AND pay_attendance.MONTH= '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR='" + txttodate.Text.Substring(3, 4) + "' ORDER BY pay_employee_master.EMP_CODE";
                    //   squery = "SELECT  pay_attendance.CGRADE_CODE AS  GRADE_CODE , SUM(pay_attendance.PRESENT_DAYS) AS PRESENT_DAYS , SUM(pay_attendance.OT_HRS) AS OT_HRS,  SUM(pay_attendance.C_HEAD01) AS '" + ehead01.ToString() + "', SUM(pay_attendance.C_HEAD02) AS '" + ehead02.ToString() + "', SUM(pay_attendance.C_HEAD03) AS '" + ehead03.ToString() + "', SUM(pay_attendance.C_HEAD04) AS '" + ehead04.ToString() + "', SUM(pay_attendance.C_HEAD05) AS '" + ehead05.ToString() + "', SUM(pay_attendance.C_HEAD06) AS '" + ehead06.ToString() + "', SUM(pay_attendance.C_HEAD07) AS '" + ehead07.ToString() + "',SUM(pay_attendance.C_HEAD08) AS '" + ehead08.ToString() + "',SUM(pay_attendance.C_HEAD09) AS '" + ehead09.ToString() + "',SUM(pay_attendance.C_HEAD10) AS '" + ehead10.ToString() + "',  SUM(pay_attendance.C_HEAD11) AS '" + ehead11.ToString() + "',SUM(pay_attendance.C_HEAD12) AS '" + ehead12.ToString() + "' ,SUM(pay_attendance.OT_GROSS) AS OT_GROSS,SUM(pay_attendance.L_HEAD01) AS '" + lhead01 + "',SUM(pay_attendance.L_HEAD02) AS '" + lhead02 + "',SUM(pay_attendance.L_HEAD03) AS '" + lhead03 + "',SUM(pay_attendance.L_HEAD04) AS '" + lhead04 + "', SUM(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.OT_GROSS+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04) AS GROSS, SUM(pay_attendance.PF) AS PF , SUM(pay_attendance.PTAX) AS PTAX,SUM(pay_attendance.ESIC_TOT) AS ESIC ,SUM(MLWF) AS LWF,SUM(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX) AS DEDUCT,SUM(pay_attendance.C_HEAD01+ pay_attendance.C_HEAD02+ pay_attendance.C_HEAD03+ pay_attendance.C_HEAD04+ pay_attendance.C_HEAD05+ pay_attendance.C_HEAD06+ pay_attendance.C_HEAD07+ pay_attendance.C_HEAD08+ pay_attendance.C_HEAD09+ pay_attendance.C_HEAD10+pay_attendance.C_HEAD11+pay_attendance.C_HEAD12+pay_attendance.L_HEAD01+pay_attendance.L_HEAD02+pay_attendance.L_HEAD03+pay_attendance.L_HEAD04+pay_attendance.OT_GROSS-pay_attendance.PF- pay_attendance.PTAX-pay_attendance.ESIC_TOT-pay_attendance.MLWF-(pay_attendance.D_HEAD01+ pay_attendance.D_HEAD02+pay_attendance.D_HEAD03+pay_attendance.D_HEAD04+pay_attendance.D_HEAD05+pay_attendance.D_HEAD06+pay_attendance.D_HEAD07+ pay_attendance.D_HEAD08+pay_attendance.D_HEAD09+pay_attendance.INCOMETAX)) AS NETTAMOUNT  FROM pay_attendance INNER JOIN pay_employee_master ON pay_attendance.EMP_CODE = pay_employee_master.EMP_CODE INNER JOIN  pay_unit_master ON pay_attendance.comp_code = pay_unit_master.comp_code AND pay_attendance.UNIT_CODE = pay_unit_master.UNIT_CODE WHERE( pay_attendance.PRESENT_DAYS>0  OR  pay_attendance.OT_HRS>0) AND (pay_attendance.comp_code = '" + Session["comp_code"].ToString() + "') AND (pay_attendance.UNIT_CODE = '" + ddlunitselect.SelectedValue.Substring(0, 4).ToString() + "')  AND pay_attendance.MONTH= '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance.YEAR='" + txttodate.Text.Substring(3, 4) + "' GROUP BY pay_attendance.CGRADE_CODE";

                }
                MySqlCommand cmd = new MySqlCommand(query, d.con1);
                DataSet ds = new DataSet();
                MySqlDataAdapter adp = new MySqlDataAdapter(query, d.con1);
                adp.Fill(ds);
                gv_ptax.DataSource = ds.Tables[0];
                gv_ptax.DataBind();
                d.con1.Close();
                panel_ptax.Visible = true;


                d1.con1.Close();
                panel_bankexcel.Visible = false;
                panel_employee_pf_esic_no.Visible = false;
                panel_employee_information_status.Visible = false;
                panel_esic_summary_utwise.Visible = false;
                // panel_ptax.Visible = false;
                panel_esic_statement.Visible = false;
                //    for (int i = 0; i < (UnitGrid_PF.Rows.Count); i++)
                //    {
                //        col_1 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[6].Text.ToString());
                //        col_1_tot = col_1_tot + col_1;

                //        col_2 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[7].Text.ToString());
                //        col_2_tot = col_2_tot + col_2;

                //        col_3 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[8].Text.ToString());
                //        col_3_tot = col_3_tot + col_3;

                //        col_4 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[9].Text.ToString());
                //        col_4_tot = col_4_tot + col_4;

                //        col_5 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[10].Text.ToString());
                //        col_5_tot = col_5_tot + col_5;

                //        col_6 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[11].Text.ToString());
                //        col_6_tot = col_6_tot + col_6;

                //        col_7 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[12].Text.ToString());
                //        col_7_tot = col_7_tot + col_7;

                //        col_8 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[13].Text.ToString());
                //        col_8_tot = col_8_tot + col_8;

                //        col_9 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[14].Text.ToString());
                //        col_9_tot = col_9_tot + col_9;

                //        col_10 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[15].Text.ToString());
                //        col_10_tot = col_10_tot + col_10;

                //        col_11 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[16].Text.ToString());
                //        col_11_tot = col_11_tot + col_11;

                //        col_12 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[17].Text.ToString());
                //        col_12_tot = col_12_tot + col_12;

                //        col_13 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[18].Text.ToString());
                //        col_13_tot = col_13_tot + col_13;

                //        col_14 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[19].Text.ToString());
                //        col_14_tot = col_14_tot + col_14;

                //        col_15 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[20].Text.ToString());
                //        col_15_tot = col_15_tot + col_15;

                //        col_16 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[21].Text.ToString());
                //        col_16_tot = col_16_tot + col_16;

                //        col_17 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[22].Text.ToString());
                //        col_17_tot = col_17_tot + col_17;

                //        col_18 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[23].Text.ToString());
                //        col_18_tot = col_18_tot + col_18;



                //        col_19 = Convert.ToDouble(UnitGrid_PF.Rows[i].Cells[24].Text.ToString());
                //        col_19_tot = col_19_tot + col_19;

                //    }

                //    UnitGrid_PF.FooterRow.Cells[3].Text = "Total:";
                //    UnitGrid_PF.FooterRow.Cells[4].Text = col_1_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[4].Visible = true;
                //    UnitGrid_PF.FooterRow.Cells[5].Text = col_2_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[6].Text = col_3_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[7].Text = col_4_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[8].Text = col_5_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[9].Text = col_6_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[10].Text = col_7_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[11].Text = col_8_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[12].Text = col_9_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[13].Text = col_10_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[14].Text = col_11_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[15].Text = col_12_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[16].Text = col_13_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[17].Text = col_14_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[18].Text = col_15_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[19].Text = col_16_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[20].Text = col_17_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[21].Text = col_18_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[22].Text = col_19_tot.ToString();
                //    UnitGrid_PF.FooterRow.Cells[23].Text = col_20_tot.ToString();



                //}

                //if (UnitGrid_PF.Rows.Count > 0)
                //{
                //    Response.Clear();
                //    Response.Buffer = true;
                //    Response.AddHeader("content-disposition", "attachment;filename=Salary_excel_data.xls");
                //    Response.Charset = "";
                //    Response.ContentType = "application/vnd.ms-excel";
                //    using (StringWriter sw = new StringWriter())
                //    {
                //        HtmlTextWriter hw = new HtmlTextWriter(sw);

                //        //To Export all pages
                //        UnitGrid_PF.AllowPaging = false;
                //        // UnitBAL ubl1 = new UnitBAL();
                //        //DataSet ds = new DataSet();
                //        //ds = ubl1.UnitSelect();
                //        //UnitGridView.DataSource = ds.Tables["pay_unit_master"];

                //        //Session["ExlColCnt"] = GridView2.Columns.Count.ToString();
                //        foreach (TableCell cell in UnitGrid_PF.HeaderRow.Cells)
                //        {
                //            cell.BackColor = UnitGrid_PF.HeaderStyle.BackColor;
                //        }
                //        foreach (GridViewRow row in UnitGrid_PF.Rows)
                //        {

                //            foreach (TableCell cell in row.Cells)
                //            {
                //                if (row.RowIndex % 2 == 0)
                //                {
                //                    cell.BackColor = UnitGrid_PF.AlternatingRowStyle.BackColor;
                //                }
                //                else
                //                {
                //                    cell.BackColor = UnitGrid_PF.RowStyle.BackColor;
                //                }
                //                cell.CssClass = "textmode";
                //            }
                //        }

                //        UnitGrid_PF.RenderControl(hw);

                //        //style to format numbers to string
                //        string style = @"<style> .textmode { } </style>";
                //        Response.Write(style);
                //        Response.Output.Write(sw.ToString());
                //        Response.Flush();
                //        Response.End();
                //        UnitGrid_PF.Visible = true;
                //    }
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        }
        catch (Exception ee)
        {

        }
    }

    protected void btn_form_16_Click(object sender, EventArgs e)
    {
        get_form16();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    public void get_form16()
    {
        ReportDocument crystalReport = new ReportDocument();
        try
        {
            crystalReport.Load(Server.MapPath("~/Form16.rpt"));

            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "Form16");

            Response.End();
        }
        catch (Exception ee)
        {


        }
    }




    protected void btnformno11_Click(object sender, EventArgs e)
    {
        //  d.con1.Open();
        // System.Web.UI.WebControls.Label lbl_EMP_code = (System.Web.UI.WebControls.Label)gv_experianceletter.SelectedRow.FindControl("lbl_realiving_id");
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }

        // string l_EMP_code1 = lbl_EMP_code.Text;
        d.con1.Close();
        string downloadname = "Form No 11";
        string query = null;



        crystalReport.Load(Server.MapPath("~/rpt_emp_fund.rpt"));
        query = "SELECT pay_employee_master.comp_code,pay_employee_master.EMP_NAME as EMP_NAME,date_format(pay_employee_master.BIRTH_DATE,'%d/%m/%Y') AS BIRTH_DATE , pay_employee_master.EMP_FATHER_NAME AS EMP_FATHER_NAME, case when pay_employee_master.GENDER = 'm' then 'Male' else 'Female' END as 'GENDER',pay_employee_master. EMP_MOBILE_NO AS EMP_MOBILE_NO, pay_employee_master. EMP_EMAIL_ID as EMP_EMAIL_ID FROM pay_employee_master   WHERE  pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' AND EMP_CODE='" + Session["EMP_CODE"].ToString() + "' ";

        //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "Offerletter");
        ReportLoad2(query, downloadname);
        Response.End();
    }

    private void ReportLoad2(string query, string downloadfilename2)
    {
        d.con.Close();
        // d1.con.Close();
        try
        {
            //btnsendemail.Visible = true;
            string downloadname = downloadfilename2;
            System.Data.DataTable dt = new System.Data.DataTable();
            MySqlCommand cmd = new MySqlCommand(query);
            MySqlDataReader sda = null;
            cmd.Connection = d.con;
            d.con.Open();
            sda = cmd.ExecuteReader();
            dt.Load(sda);
            d.con.Close();

            //MySqlCommand cmd_item1 = new MySqlCommand("SELECT COMP_LOGO from pay_company_master where comp_code='" + Session["comp_code"].ToString() + "' ", d.con);
            //d.con.Open();
            //MySqlDataReader dr_item1 = cmd_item1.ExecuteReader();
            //if (dr_item1.Read())
            //{
            //    string path = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\" + dr_item1.GetValue(0).ToString());
            //    crystalReport.DataDefinition.FormulaFields["image_path"].Text = "'" + path + "'";
            //    PictureObject TAddress1 = (PictureObject)crystalReport.ReportDefinition.Sections[0].ReportObjects["Picture1"];
            //    TAddress1.Width = 850;
            //    TAddress1.Height = 400;
            //    TAddress1.Left = 4000;
            //}
            //else
            //{
            //    string path = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\logo.png");
            //    crystalReport.DataDefinition.FormulaFields["image_path"].Text = "'" + path + "'";
            //}
            //dr_item1.Close();
            //d.con.Close();

            crystalReport.SetDataSource(dt);
            crystalReport.Refresh();
            //  crystalReport.SetDatabaseLogon(@"Tanvi-Tej\Tanvi","");
            //Response.End();
            // updateAmount();
            // text_Clear();
            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, true, downloadname);
            // updateAmount();

        }
        catch (Exception ex)
        {
            Response.Write("<script language='javascript'>alert('" + Server.HtmlEncode(ex.Message) + "')</script>");
            //var message = new JavaScriptSerializer().Serialize(ex.Message.ToString());
            //var script = string.Format("alert({0});", message);
            //ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "", script, true);
        }
        finally
        {

        }
    }
    protected void btnformno2_Click(object sender, EventArgs e)
    {
        Panel3.Visible = false;
        Panel2.Visible = false;
        string downloadname = "FORM_NO_2";
        string query = null;



        crystalReport.Load(Server.MapPath("~/rpt_nomination_form.rpt"));
        // query = "SELECT pay_employee_master.comp_code,pay_employee_master.EMP_NAME as EMP_NAME,pay_employee_master.BIRTH_DATE AS BIRTH_DATE , pay_employee_master.EMP_FATHER_NAME AS EMP_FATHER_NAME,pay_employee_master. GENDER AS GENDER,pay_employee_master. EMP_MOBILE_NO AS EMP_MOBILE_NO, pay_employee_master. EMP_EMAIL_ID as EMP_EMAIL_ID FROM pay_employee_master   WHERE pay_employee_master.EMP_CODE ='" + l_EMP_code1 + "'  AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' ";

        crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "FORM_NO_2");
        ReportLoad1(downloadname);
        Response.End();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    private void ReportLoad1(string downloadfilename2)
    {
        d.con.Close();
        // d1.con.Close();
        try
        {
            //btnsendemail.Visible = true;
            string downloadname = downloadfilename2;
            System.Data.DataTable dt = new System.Data.DataTable();


            crystalReport.SetDataSource(dt);
            crystalReport.Refresh();

            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, true, downloadname);
            // updateAmount();

        }
        catch (Exception ex)
        {
            Response.Write("<script language='javascript'>alert('" + Server.HtmlEncode(ex.Message) + "')</script>");

        }
        finally
        {

        }
    }
    private void ReportLoad2(string downloadfilename2)
    {
        d.con.Close();
        // d1.con.Close();
        try
        {
            ////btnsendemail.Visible = true;
            string downloadname = downloadfilename2;
            // System.Data.DataTable dt = new System.Data.DataTable();


            // crystalReport.SetDataSource(dt);
            crystalReport.Refresh();

            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, true, downloadname);
            // updateAmount();

        }
        catch (Exception ex)
        {
            Response.Write("<script language='javascript'>alert('" + Server.HtmlEncode(ex.Message) + "')</script>");

        }
        finally
        {

        }
    }
    protected void btnnoobjecioncerificae_Click(object sender, EventArgs e)
    {
        Panel3.Visible = false;
        Panel2.Visible = false;
        string downloadname = "No_Objection_Certificate";
        string query = null;



        crystalReport.Load(Server.MapPath("~/rpt_emp_objection.rpt"));
        // query = "SELECT pay_employee_master.comp_code,pay_employee_master.EMP_NAME as EMP_NAME,pay_employee_master.BIRTH_DATE AS BIRTH_DATE , pay_employee_master.EMP_FATHER_NAME AS EMP_FATHER_NAME,pay_employee_master. GENDER AS GENDER,pay_employee_master. EMP_MOBILE_NO AS EMP_MOBILE_NO, pay_employee_master. EMP_EMAIL_ID as EMP_EMAIL_ID FROM pay_employee_master   WHERE pay_employee_master.EMP_CODE ='" + l_EMP_code1 + "'  AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' ";

        crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "No_Objection_Certificate");
        ReportLoad1(downloadname);
        Response.End();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    protected void btn_lwf_chalan_Click(object sender, EventArgs e)
    {
        Panel3.Visible = false;
        Panel2.Visible = false;
        string downloadname = "No_Objection_Certificate";
        string query = null;



        crystalReport.Load(Server.MapPath("~/form_A-1_return.rpt"));
        // query = "SELECT pay_employee_master.comp_code,pay_employee_master.EMP_NAME as EMP_NAME,pay_employee_master.BIRTH_DATE AS BIRTH_DATE , pay_employee_master.EMP_FATHER_NAME AS EMP_FATHER_NAME,pay_employee_master. GENDER AS GENDER,pay_employee_master. EMP_MOBILE_NO AS EMP_MOBILE_NO, pay_employee_master. EMP_EMAIL_ID as EMP_EMAIL_ID FROM pay_employee_master   WHERE pay_employee_master.EMP_CODE ='" + l_EMP_code1 + "'  AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' ";

        crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "LWF_Chalan");
        ReportLoad1(downloadname);
        Response.End();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }



    protected void gv_esic_statement_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_esic_statement.UseAccessibleHeader = false;
            gv_esic_statement.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch
        {

        } //vinod
    }
    protected void UnitGridView_PreRender(object sender, EventArgs e)
    {


        try
        {
            UnitGridView.UseAccessibleHeader = false;
            UnitGridView.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch
        {

        } //vinod
    }


    protected void employee_information_status_PreRender(object sender, EventArgs e)
    {


        try
        {
            gv_employee_information_status.UseAccessibleHeader = false;
            gv_employee_information_status.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch
        {

        } //vinod
    }


    protected void employee_pf_esic_no_PreRender(object sender, EventArgs e)
    {


        try
        {
            gv_employee_pf_esic_no.UseAccessibleHeader = false;
            gv_employee_pf_esic_no.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch
        {

        } //vinod
    }


    protected void gv_ptax_PreRender(object sender, EventArgs e)
    {

        try
        {
            gv_ptax.UseAccessibleHeader = false;
            gv_ptax.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch
        {

        } //vinod

    }
    protected void panel_bankexcel_PreRender(object sender, EventArgs e)
    {

        try
        {
            gv_bankexcel.UseAccessibleHeader = false;
            gv_bankexcel.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch
        {

        } //vinod

    }

    protected void UnitGrid_PF_PreRender(object sender, EventArgs e)
    {

        try
        {
            UnitGrid_PF.UseAccessibleHeader = false;
            UnitGrid_PF.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch
        {

        } //vinod

    }

    protected void btn_upload_Click(object sender, EventArgs e)
    {

        upload_documents(document1_file, ddl_clra_act.SelectedValue);

    }
    private void upload_documents(FileUpload document_file, string file_name)
    {
        if (document_file.HasFile)
        {
            string fileExt = System.IO.Path.GetExtension(document_file.FileName);
            if (fileExt == ".jpg" || fileExt == ".png" || fileExt == ".pdf")
            {
                string fileName = Path.GetFileName(document_file.PostedFile.FileName);
                document_file.PostedFile.SaveAs(Server.MapPath("~/compliance/") + fileName);

                File.Copy(Server.MapPath("~/compliance/") + fileName, Server.MapPath("~/compliance/") + Session["COMP_CODE"].ToString() + "_" + ddl_client.SelectedValue + "_" + ddl_clra_act.SelectedValue.Replace(" ", "_") + fileExt, true);
                File.Delete(Server.MapPath("~/compliance/") + fileName);

                d.operation("insert into pay_compliance_master (comp_code, client_code, description, file_name, act_name, uploaded_by, uploaded_datetime) values ('" + Session["COMP_CODE"].ToString() + "','" + ddl_client.SelectedValue + "','" + ddl_clra_act.SelectedValue + "','" + Session["COMP_CODE"].ToString() + "_" + ddl_client.SelectedValue + "_" + ddl_clra_act.SelectedValue.Replace(" ", "_") + fileExt + "','" + ddl_clra_act.SelectedValue + "','" + Session["LOGIN_ID"].ToString() + "',now())");
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please select only JPG, PNG and PDF Files !!!')", true);
            }
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Select Document !!')", true);
        }
        load_grdview();
    }

    protected void gv_esic_summary_utwise_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_esic_summary_utwise.UseAccessibleHeader = false;
            gv_esic_summary_utwise.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch
        {

        }
    }


    protected void ddl_client_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (ddl_client.SelectedValue != "Select")
        {
            ddlunitselect.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
           //comment 30/09 MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_NAME,'_',UNIT_ADD1) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "'  AND pay_unit_master.branch_status = 0  ORDER BY UNIT_CODE", d.con);
           MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_NAME,'_',UNIT_ADD1) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "'  AND (branch_close_date is null || branch_close_date = '' ||branch_close_date  >= STR_TO_DATE('01/" + txttodate.Text + "', '%d/%m/%Y'))  ORDER BY UNIT_CODE", d.con);
		    d.con.Open();
            try
            {
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddlunitselect.DataSource = dt_item;
                    ddlunitselect.DataTextField = dt_item.Columns[0].ToString();
                    ddlunitselect.DataValueField = dt_item.Columns[1].ToString();
                    ddlunitselect.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddlunitselect.Items.Insert(0, "ALL");
                ddlunitselect.SelectedIndex = 0;
                show_controls();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }


            //State
            ddl_state.Items.Clear();
            dt_item = new System.Data.DataTable();
          //comment 30/09  cmd_item = new MySqlDataAdapter("Select distinct(state_name) from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' ORDER BY state_name", d.con);
           cmd_item = new MySqlDataAdapter("Select distinct(state_name) from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' AND (branch_close_date is null || branch_close_date = '' ||branch_close_date  >= STR_TO_DATE('01/" + txttodate.Text + "', '%d/%m/%Y')) ORDER BY state_name", d.con);
		    d.con.Open();
            try
            {
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_state.DataSource = dt_item;
                    ddl_state.DataTextField = dt_item.Columns[0].ToString();
                    ddl_state.DataValueField = dt_item.Columns[0].ToString();
                    ddl_state.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_state.Items.Insert(0, "ALL");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
                ddl_state_SelectedIndexChanged(null, null);
            }
        }
        else
        {
            ddlunitselect.Items.Clear();
            ddl_state.Items.Clear();
            hide_controls();
        }
    }
    private void show_controls()
    {
        unit_panel.Visible = true;
        //btn_Export.Visible = true;
        //  btn_process.Visible = true;
        //btn_save.Visible = true;
        //FileUpload1.Visible = true;
        //btn_upload.Visible = true;
    }

    private void hide_controls()
    {
        unit_panel.Visible = false;
        // btn_Export.Visible = false;
        //  btn_process.Visible = false;
        //btn_save.Visible = false;
        //FileUpload1.Visible = false;
        //btn_upload.Visible = false;
    }



    protected void ddlunitselect_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlunitselect.SelectedValue == "ALL")
        {
            ddl_designation.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select DISTINCT (designation), grade_code FROM pay_designation_count INNER JOIN pay_grade_master ON pay_designation_count.designation = pay_grade_master.grade_desc AND pay_designation_count.comp_code = pay_grade_master.comp_code WHERE pay_designation_count.comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and state = '" + ddl_state.SelectedValue + "' ORDER BY designation", d.con);
            d.con.Open();
            try
            {
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_designation.DataSource = dt_item;
                    ddl_designation.DataTextField = dt_item.Columns[0].ToString();
                    ddl_designation.DataValueField = dt_item.Columns[1].ToString();
                    ddl_designation.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_designation.Items.Insert(0, "ALL");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
        else
        {
            ddl_designation.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select DISTINCT (designation), grade_code FROM pay_designation_count INNER JOIN pay_grade_master ON pay_designation_count.designation = pay_grade_master.grade_desc AND pay_designation_count.comp_code = pay_grade_master.comp_code WHERE pay_designation_count.comp_code = '" + Session["comp_code"] + "' and unit_code = '" + ddlunitselect.SelectedValue + "' ORDER BY designation", d.con);
            d.con.Open();
            try
            {
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_designation.DataSource = dt_item;
                    ddl_designation.DataTextField = dt_item.Columns[0].ToString();
                    ddl_designation.DataValueField = dt_item.Columns[1].ToString();
                    ddl_designation.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_designation.Items.Insert(0, "ALL");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
    }

    protected void btn_formno_A_Click(object sender, EventArgs e)
    {
        generate_letter(8);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    protected void btn_formno_B_Click(object sender, EventArgs e)
    {
        generate_letter(9);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }


    protected void btn_enquiry_form_Click(object sender, EventArgs e)
    {
        Panel3.Visible = false;
        Panel2.Visible = false;
        string downloadname = "FORM_NO_2";

        crystalReport.Load(Server.MapPath("~/rpt_police_verify_form.rpt"));
        // query = "SELECT pay_employee_master.comp_code,pay_employee_master.EMP_NAME as EMP_NAME,pay_employee_master.BIRTH_DATE AS BIRTH_DATE , pay_employee_master.EMP_FATHER_NAME AS EMP_FATHER_NAME,pay_employee_master. GENDER AS GENDER,pay_employee_master. EMP_MOBILE_NO AS EMP_MOBILE_NO, pay_employee_master. EMP_EMAIL_ID as EMP_EMAIL_ID FROM pay_employee_master   WHERE pay_employee_master.EMP_CODE ='" + l_EMP_code1 + "'  AND pay_employee_master.comp_code='" + Session["comp_code"].ToString() + "' ";

        crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "FORM_NO_2");
        ReportLoad1(downloadname);
        Response.End();
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    private void generate_letter(int counter)
    {
        string sql = null;

        d.con.Open();
        try
        {
            string where = "";
            if (counter != 14 && counter != 15 && counter != 16 && counter != 17)
            {
                where = " pay_unit_master.comp_code= '" + Session["COMP_CODE"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_employee_master.joining_date < str_to_date('31/" + txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3) + "','%d/%m/%Y')";
                if (!ddl_state.SelectedValue.ToUpper().Equals("ALL"))
                { where = where + " and pay_unit_master.state_name= '" + ddl_state.SelectedValue + "'"; }
                if (!ddlunitselect.SelectedValue.ToUpper().Equals("ALL"))
                {
                    where = where + " and pay_unit_master.unit_code= '" + ddl_state.SelectedValue + "'";
                }
                if (!ddl_designation.SelectedValue.ToUpper().Equals("ALL"))
                { where = where + " and pay_employee_master.grade_code = '" + ddl_designation.SelectedValue + "'"; }

                if (ddl_client.SelectedValue == "BAGIC TM")
                {
                    where = where + " and (pay_employee_master.employee_type='Permanent' OR pay_employee_master.employee_type='Temporary')";
                }
                else
                {
                    where = where + " and pay_employee_master.employee_type='Permanent'";
                }
            }
            if (counter.Equals(1) || counter.Equals(2) || counter.Equals(10))
            {
                if (counter == 1) { where = where + " and 18 > YEAR(DATE_SUB(NOW(), INTERVAL TO_DAYS(birth_date) DAY)) "; }
                else { where = where + " and 18 <= YEAR(DATE_SUB(NOW(), INTERVAL TO_DAYS(birth_date) DAY)) "; }

                sql = "select emp_name,case when gender='M' then 'MALE' else ' FEMALE' END as Gender,emp_father_name,  concat(EMP_PERM_ADDRESS,' ',EMP_PERM_CITY,' ',EMP_PERM_STATE,' ',EMP_PERM_PIN) as a ,concat(EMP_CURRENT_ADDRESS,' ',EMP_CURRENT_CITY,' ',EMP_CURRENT_STATE,' ',EMP_CURRENT_PIN) as b, date_format(joining_date,'%d/%m/%Y') as joining_date, case when Left_date is null then 'CONTINUE' else date_format(left_date,'%d/%m/%Y') END as left_date, case when Left_date is null then 'CONTINUE' else 'LEFT' END as left_reason ,(select concat(company_name,' ',ADDRESS1,' ',CITY,' ',STATE) from pay_company_master where pay_company_master.comp_code = pay_employee_master.comp_code) as company, (select grade_desc from pay_grade_master where pay_grade_master.grade_code = pay_employee_master.grade_code and pay_grade_master.comp_code = pay_employee_master.comp_code) as designation, pay_employee_master.id_as_per_dob FROM pay_employee_master inner join pay_unit_master on pay_employee_master.comp_code = pay_unit_master.comp_code and pay_employee_master.unit_code = pay_unit_master.unit_code WHERE " + where + " and joining_date between str_to_date('1/" + txttodate.Text + "','%d/%m/%Y') and str_to_date('31/" + txttodate.Text + "','%d/%m/%Y') order by pay_employee_master.joining_date";

            }
            else if (counter == 3)
            {
                sql = "select emp_name,case when gender='M' then 'MALE' else ' FEMALE' END as Gender,emp_father_name,  concat(EMP_PERM_ADDRESS,' ',EMP_PERM_CITY,' ',EMP_PERM_STATE,' ',EMP_PERM_PIN) as a ,concat(EMP_CURRENT_ADDRESS,' ',EMP_CURRENT_CITY,' ',EMP_CURRENT_STATE,' ',EMP_CURRENT_PIN) as b, date_format(joining_date,'%d/%m/%Y') as joining_date, case when Left_date is null then 'CONTINUE' else date_format(left_date,'%d/%m/%Y') END as left_date, Left_reason ,(select concat(company_name,' ',ADDRESS1,' ',CITY,' ',STATE) from pay_company_master where pay_company_master.comp_code = pay_employee_master.comp_code) as company, id_as_per_dob from pay_employee_master inner join pay_unit_master on pay_employee_master.comp_code = pay_unit_master.comp_code and pay_employee_master.unit_code = pay_unit_master.unit_code WHERE " + where + " and joining_date between str_to_date('1/" + txttodate.Text + "','%d/%m/%Y') and str_to_date('31/" + txttodate.Text + "','%d/%m/%Y') order by pay_employee_master.joining_date";
            }
            else if (counter == 4)
            {
                sql = "select emp_name,case when gender='M' then 'MALE' else ' FEMALE' END as Gender,emp_father_name,  concat(EMP_PERM_ADDRESS,' ',EMP_PERM_CITY,' ',EMP_PERM_STATE,' ',EMP_PERM_PIN) as a ,concat(EMP_CURRENT_ADDRESS,' ',EMP_CURRENT_CITY,' ',EMP_CURRENT_STATE,' ',EMP_CURRENT_PIN) as b, date_format(joining_date,'%d/%m/%Y') as joining_date, case when Left_date is null then 'CONTINUE' else date_format(left_date,'%d/%m/%Y') END as left_date, Left_reason ,(select concat(company_name,' ',ADDRESS1,' ',CITY,' ',STATE) from pay_company_master where pay_company_master.comp_code = pay_employee_master.comp_code) as company, id_as_per_dob from pay_employee_master inner join pay_unit_master on pay_employee_master.comp_code = pay_unit_master.comp_code and pay_employee_master.unit_code = pay_unit_master.unit_code WHERE " + where + " and joining_date between str_to_date('1/" + txttodate.Text + "','%d/%m/%Y') and str_to_date('31/" + txttodate.Text + "','%d/%m/%Y') order by pay_employee_master.joining_date";
            }
            else if (counter == 5)
            {
                sql = "select emp_name,case when gender='M' then 'MALE' else ' FEMALE' END as Gender,emp_father_name,  concat(EMP_PERM_ADDRESS,' ',EMP_PERM_CITY,' ',EMP_PERM_STATE,' ',EMP_PERM_PIN) as a ,concat(EMP_CURRENT_ADDRESS,' ',EMP_CURRENT_CITY,' ',EMP_CURRENT_STATE,' ',EMP_CURRENT_PIN) as b, date_format(joining_date,'%d/%m/%Y') as joining_date, case when Left_date is null then 'CONTINUE' else date_format(left_date,'%d/%m/%Y') END as left_date, Left_reason ,(select concat(company_name,' ',ADDRESS1,' ',CITY,' ',STATE) from pay_company_master where pay_company_master.comp_code = pay_employee_master.comp_code) as company, id_as_per_dob from pay_employee_master inner join pay_unit_master on pay_employee_master.comp_code = pay_unit_master.comp_code and pay_employee_master.unit_code = pay_unit_master.unit_code WHERE " + where + " and joining_date between str_to_date('1/" + txttodate.Text + "','%d/%m/%Y') and str_to_date('31/" + txttodate.Text + "','%d/%m/%Y')  order by pay_employee_master.joining_date";
            }
            else if (counter == 6)
            {
                sql = "SELECT upper(pay_pro_master.state_name) as state_name, upper(pay_pro_master.unit_city) as unit_city, pay_employee_master.emp_name, CASE WHEN pay_pro_master.gender = 'M' THEN 'MALE' ELSE ' FEMALE' END AS 'Gender', emp_father_name, CONCAT(EMP_PERM_ADDRESS, ' ', EMP_PERM_CITY, ' ', EMP_PERM_STATE, ' ', EMP_PERM_PIN) AS 'a', CONCAT(EMP_CURRENT_ADDRESS, ' ', EMP_CURRENT_CITY, ' ', EMP_CURRENT_STATE, ' ', EMP_CURRENT_PIN) AS 'b', DATE_FORMAT(pay_pro_master.joining_date, '%d/%m/%Y') AS 'joining_date', CASE WHEN Left_date IS NULL THEN 'CONTINUE' ELSE DATE_FORMAT(left_date, '%d/%m/%Y') END AS 'left_date', if(LEFT_REASON !='','LEFT SERVICE',LEFT_REASON) AS 'LEFT_REASON', (SELECT CONCAT(company_name, ' ', ADDRESS1, ' ', CITY, ' ', STATE) FROM pay_company_master WHERE pay_company_master.comp_code = pay_employee_master.comp_code) AS 'company', id_as_per_dob, ROUND((emp_basic_vda / total_days_present), 2) AS 'wage', UPPER(CONCAT(IF(pay_employee_master.grade_code = 'SG', 'SG', 'HK'), ' Services ', pay_pro_master.unit_city)) AS 'grade_code', (SELECT grade_desc FROM pay_grade_master WHERE pay_grade_master.grade_code = pay_employee_master.grade_code LIMIT 1) AS 'designation' FROM pay_employee_master INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND pay_employee_master.unit_code = pay_unit_master.unit_code INNER JOIN pay_pro_master ON pay_pro_master.comp_code = pay_unit_master.comp_code AND pay_pro_master.client_code = pay_unit_master.client_code AND pay_pro_master.unit_code = pay_unit_master.unit_code AND pay_pro_master.emp_code = pay_employee_master.emp_code WHERE " + where + " AND pay_employee_master.employee_type = 'Permanent' GROUP BY pay_employee_master.emp_code";
            }
            else if (counter == 7)
            {
                string start_date_common = d.getsinglestring("select start_date_common from pay_billing_master_history where billing_client_code = '" + ddl_client.SelectedValue + "' and month =" + txttodate.Text.Substring(0, 2) + " and year=" + txttodate.Text.Substring(3) + " limit 1");
                if (start_date_common == "1" || start_date_common == "")
                {
                    string daterange = "concat(upper(DATE_FORMAT(str_to_date('" + txttodate.Text.Substring(3) + "-" + txttodate.Text.Substring(0, 2) + "-01','%Y-%m-%d'), '%D %b %Y')),' TO ',upper(DATE_FORMAT(LAST_DAY('" + txttodate.Text.Substring(3) + "-" + txttodate.Text.Substring(0, 2) + "-01'), '%D %b %Y'))) as fromtodate";
                    sql = "select pay_unit_master.state_name," + daterange + ",day(last_day(str_to_date('01/" + txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3) + "','%d/%m/%Y'))) as 'total days',  emp_name, pay_employee_master.id_as_per_dob as 'emp_code',unit_name,case when DAY01 ='P' then 'P' else 'A' END as  DAY01,case when DAY02 ='P' then 'P' else 'A' END as  DAY02,case when DAY03 ='P' then 'P' else 'A' END as  DAY03,case when DAY04 ='P' then 'P' else 'A' END as  DAY04,case when DAY05 ='P' then 'P' else 'A' END as  DAY05,case when DAY06 ='P' then 'P' else 'A' END as  DAY06,case when DAY07 ='P' then 'P' else 'A' END as  DAY07,case when DAY08 ='P' then 'P' else 'A' END as  DAY08,case when DAY09 ='P' then 'P' else 'A' END as  DAY09,case when DAY10 ='P' then 'P' else 'A' END as  DAY10,case when DAY11 ='P' then 'P' else 'A' END as  DAY11,case when DAY12 ='P' then 'P' else 'A' END as  DAY12,case when DAY13 ='P' then 'P' else 'A' END as  DAY13,case when DAY14 ='P' then 'P' else 'A' END as  DAY14,case when DAY15 ='P' then 'P' else 'A' END as  DAY15,case when DAY16 ='P' then 'P' else 'A' END as  DAY16,case when DAY17 ='P' then 'P' else 'A' END as  DAY17,case when DAY18 ='P' then 'P' else 'A' END as  DAY18,case when DAY19 ='P' then 'P' else 'A' END as  DAY19,case when DAY20 ='P' then 'P' else 'A' END as  DAY20,case when DAY21 ='P' then 'P' else 'A' END as  DAY21,case when DAY22 ='P' then 'P' else 'A' END as  DAY22,case when DAY23 ='P' then 'P' else 'A' END as  DAY23,case when DAY24 ='P' then 'P' else 'A' END as  DAY24,case when DAY25 ='P' then 'P' else 'A' END as  DAY25,case when DAY26 ='P' then 'P' else 'A' END as  DAY26,case when DAY27 ='P' then 'P' else 'A' END as  DAY27,case when DAY28 ='P' then 'P' else 'A' END as  DAY28,case when DAY29 ='P' then 'P' else 'A' END as  DAY29,case when DAY30 ='P' then 'P' else 'A' END as  DAY30,case when DAY31 ='P' then 'P' else 'A' END as  DAY31,TOT_DAYS_PRESENT,(select company_name from pay_company_master where pay_company_master.comp_code = pay_unit_master.comp_code) as company,(Select LICENSE_NO From pay_client_master where client_code =pay_unit_master.client_code) as 'LICENSE_NO' from pay_employee_master inner join pay_attendance_muster on pay_attendance_muster.emp_code = pay_employee_master.emp_code inner join pay_unit_master on pay_unit_master.unit_code = pay_employee_master.unit_code where pay_attendance_muster.month = " + txttodate.Text.Substring(0, 2) + " and pay_attendance_muster.year = " + txttodate.Text.Substring(3) + "  and " + where;
                }
                else
                {
                    int month = int.Parse(txttodate.Text.Substring(0, 2)) - 1;
                    int year = int.Parse(txttodate.Text.Substring(3));
                    if (month == 0) { month = 12; year = year - 1; }
                    string daterange = "concat(upper(DATE_FORMAT(str_to_date('" + ((int.Parse(txttodate.Text.Substring(0, 2)) - 1) == 0 ? (int.Parse(txttodate.Text.Substring(3)) - 1).ToString() : txttodate.Text.Substring(3)) + "-" + ((int.Parse(txttodate.Text.Substring(0, 2)) - 1) == 0 ? 12 : (int.Parse(txttodate.Text.Substring(0, 2)) - 1)) + "-" + start_date_common + "','%Y-%m-%d'), '%D %b %Y')),' TO ',upper(DATE_FORMAT(str_to_date('" + txttodate.Text.Substring(3) + "-" + txttodate.Text.Substring(0, 2) + "-" + (int.Parse(start_date_common) - 1) + "','%Y-%m-%d'), '%D %b %Y'))) as fromtodate";

                    sql = "select pay_unit_master.state_name, " + daterange + ", day(last_day(str_to_date('01/" + month + "/" + year + "','%d/%m/%Y'))) as 'total days', pay_employee_master.emp_name, pay_employee_master.id_as_per_dob as 'emp_code',unit_name, " + d.get_calendar_days(int.Parse(start_date_common), txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3), 1, 1) + "" + d.get_calendar_days(int.Parse(start_date_common), txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3), 3, 1) + " pay_attendance_muster.tot_days_present,(select company_name from pay_company_master where pay_company_master.comp_code = pay_unit_master.comp_code) as company,(Select LICENSE_NO From pay_client_master where client_code =pay_unit_master.client_code) as 'LICENSE_NO' from pay_employee_master LEFT JOIN pay_attendance_muster ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code AND pay_attendance_muster.month =  '" + txttodate.Text.Substring(0, 2) + "'   AND pay_attendance_muster.Year = '" + txttodate.Text.Substring(3) + "'   AND pay_attendance_muster.tot_days_present > 0  INNER JOIN pay_unit_master ON pay_employee_master.unit_code = pay_unit_master.unit_code AND pay_employee_master.comp_code = pay_unit_master.comp_code   left JOIN pay_grade_master ON pay_unit_master.comp_code = pay_grade_master.comp_code AND pay_employee_master.GRADE_CODE = pay_grade_master.GRADE_CODE  INNER JOIN pay_company_master ON pay_unit_master.comp_code = pay_company_master.comp_code  LEFT JOIN pay_ot_muster ON  pay_attendance_muster.emp_code = pay_ot_muster.emp_code AND pay_attendance_muster.comp_code = pay_ot_muster.comp_code  AND pay_attendance_muster.UNIT_CODE = pay_ot_muster.UNIT_CODE  AND pay_attendance_muster.MONTH = pay_ot_muster.MONTH  AND pay_attendance_muster.YEAR = pay_ot_muster.YEAR  AND pay_ot_muster.month =  '" + txttodate.Text.Substring(0, 2) + "'  LEFT JOIN pay_attendance_muster t2   ON " + year + "= t2.year AND pay_company_master.COMP_CODE = t2.COMP_CODE  AND pay_unit_master.UNIT_CODE = t2.UNIT_CODE AND pay_employee_master.EMP_CODE = t2.EMP_CODE  AND t2.month ='" + month + "'  AND t2.tot_days_present > 0 LEFT OUTER JOIN pay_ot_muster t3   ON " + year + " = t3.YEAR  AND pay_unit_master.UNIT_CODE = t3.UNIT_CODE AND pay_employee_master.EMP_CODE = t3.EMP_CODE AND pay_company_master.COMP_CODE = t3.COMP_CODE   AND t3.month = '" + month + "' WHERE pay_attendance_muster.month =  '" + txttodate.Text.Substring(0, 2) + "' AND pay_attendance_muster.Year = '" + txttodate.Text.Substring(3) + "'   AND pay_attendance_muster.tot_days_present > 0 and " + where;
                }
            }
            else if (counter == 8)
            {
                if (ddl_act.SelectedValue == "MH SG ACT")
                {
                    where = where + " and pay_employee_master.grade_code = 'SG'";
                }
                
                sql = "SELECT pay_employee_master.id_as_per_dob, pay_employee_master.EMP_NAME, CASE WHEN GENDER = 'M' THEN 'Male' ELSE 'Female' END AS 'Gender', EMP_FATHER_NAME AS 'FatherName', DATE_FORMAT(BIRTH_DATE, '%d/%m/%Y') AS 'Birth_date', EMP_NATIONALITY AS 'Nationality', QUALIFICATION_1 AS 'Education_Level', DATE_FORMAT(JOINING_DATE, '%d/%m/%Y') AS 'Date_Of_Joining', (SELECT grade_desc FROM pay_grade_master WHERE pay_grade_master.Grade_code = pay_employee_master.GRADE_CODE AND pay_grade_master.comp_code = pay_employee_master.comp_code) AS 'Designation', (SELECT grade_desc FROM pay_grade_master WHERE pay_grade_master.Grade_code = pay_employee_master.GRADE_CODE AND pay_grade_master.comp_code = pay_employee_master.comp_code) AS 'Type_Of_Employee', EMP_MOBILE_NO AS 'Employee_No', PAN_NUMBER AS 'UAN_NO', PF_NUMBER AS 'PF_NO', ESIC_NUMBER, P_TAX_NUMBER AS 'Adhar_No', original_bank_account_no AS 'Bank_Account_No', PF_BANK_NAME AS 'BankName', PF_IFSC_CODE AS 'IFSC_Code', CONCAT(EMP_CURRENT_ADDRESS, ' ', EMP_CURRENT_CITY, ' ', EMP_CURRENT_STATE, ' ', EMP_CURRENT_PIN) AS 'present', CONCAT(EMP_PERM_ADDRESS, ' ', EMP_PERM_CITY, ' ', EMP_PERM_STATE, ' ', EMP_PERM_PIN) AS 'permanent', date_format(Left_date,'%d/%m/%Y') AS 'left_date', if(LEFT_REASON !='','LEFT SERVICE',LEFT_REASON) AS 'Reason_Of_Exit', EMP_IDENTITYMARK, (SELECT company_name FROM pay_company_master WHERE pay_company_master.comp_code = pay_employee_master.comp_code) AS 'company', EMP_NEW_PAN_NO, employee_type, QUALIFICATION_1,if((select state_name from pay_master_lwf where pay_master_lwf.state_name = pay_unit_master.state_name)!='','APPLICABLE','N/A') as lwf FROM pay_employee_master INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND pay_employee_master.unit_code = pay_unit_master.unit_code where " + where + " and emp_code not in (select emp_code FROM pay_employee_master INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND pay_employee_master.unit_code = pay_unit_master.unit_code where " + where + " and pay_employee_master.left_date < str_to_date('1/" + txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3) + "','%d/%m/%Y') ) order by JOINING_DATE";
            }
            else if (counter == 9)
            {
                sql = "SELECT (SELECT company_name FROM pay_company_master WHERE pay_company_master.comp_code = pay_employee_master.comp_code) AS 'company', ihms AS 'ihms_code', pay_pro_master.emp_name, grade, Basic, ROUND((emp_basic_vda / Total_Days_Present), 2) AS 'Rate_of_Wage', ROUND(emp_basic_vda, 2) AS 'Special_Basic', ROUND(hra_amount_salary, 2) AS 'hra', ROUND(sal_bonus_gross, 2) AS 'Bonus', leave_sal_gross, ROUND(leave_sal_gross, 2) AS 'leave_gross', ROUND((emp_basic_vda + hra_amount_salary + sal_bonus_gross + leave_sal_gross + pay_pro_master.special_allow+ot_amount), 2) AS 'total_earnings', ROUND(PAYMENT, 2) AS 'net_payment', Total_Days_Present, client, pay_pro_master.state_name, COMPANY_NAME, LICENSE_NO, pay_pro_master.EMP_CODE, id_as_per_dob, ROUND(sal_pf, 2) AS 'PF', ROUND(sal_esic, 2) AS 'ESIC', ROUND(lwf_salary, 2) AS 'lwf', ROUND(pt_amount, 2) AS 'pt', ROUND((sal_pf + sal_esic + lwf_salary + pt_amount + pay_pro_master.fine + pay_pro_master.EMP_ADVANCE_PAYMENT), 2) AS 'total_deduction', ROUND(pay_pro_master.special_allow, 2) AS 'allow', ROUND(pay_pro_master.fine, 2) AS 'fine', ROUND(pay_pro_master.EMP_ADVANCE_PAYMENT, 2) AS 'advance', round(ot_amount,2) as ot_amount, ot_hours,sal_bonus_after_gross,leave_sal_after_gross FROM pay_pro_master INNER JOIN pay_employee_master ON pay_pro_master.emp_code = pay_employee_master.emp_code AND pay_pro_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_unit_master.unit_code = pay_pro_master.unit_code AND pay_unit_master.comp_code = pay_pro_master.comp_code WHERE pay_pro_master.month = '" + txttodate.Text.Substring(0, 2) + "' AND pay_pro_master.year = '" + txttodate.Text.Substring(3) + "' AND " + where + " order by pay_employee_master.joining_date";
            }
            else if (counter == 11)
            {
                sql = "SELECT pay_employee_master.emp_name, id_as_per_dob, pay_grade_master.grade_desc, if(emp_blood_group='Select','',emp_blood_group) as emp_blood_group, DATE_FORMAT(joining_date, '%d/%m/%Y') AS 'DOJ', client_name, emp_photo FROM pay_employee_master INNER JOIN pay_grade_master ON pay_grade_master.grade_code = pay_employee_master.grade_code AND pay_grade_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_client_master ON pay_client_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_unit_master.unit_code = pay_employee_master.unit_code AND pay_unit_master.client_code = pay_client_master.client_code AND pay_unit_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_images_master ON pay_employee_master.emp_code = pay_images_master.emp_code AND pay_employee_master.comp_code = pay_images_master.comp_code WHERE pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_client_master.client_code = '" + ddl_client.SelectedValue + "' and pay_employee_master.employee_type='Permanent' and left_date is null";
            }
            else if (counter == 12)
            {
                sql = "select client, state_name,unit_gst_no, round(sum(CGST9),2) as cgst,round(sum(IGST18),2) as igst,round(sum(SGST9),2) as sgst from pay_billing_unit_rate_history where client_code = '" + ddl_client.SelectedValue + "' and month=" + txttodate.Text.Substring(0, 2) + " and year=" + txttodate.Text.Substring(3) + " and comp_code = '" + Session["COMP_CODE"].ToString() + "' group by state_name order by state_name";
            }
            else if (counter == 13)
            {
                sql = "select client, state_name, unit_city, emp_name, grade, round(gross,2) as gross, pt_amount from pay_pro_master where month=" + txttodate.Text.Substring(0, 2) + " and year=" + txttodate.Text.Substring(3) + " and client_code = '" + ddl_client.SelectedValue + "' and comp_code = '" + Session["COMP_CODE"].ToString() + "' and pt_amount > 0 order by 1,2,3,4,5";
            }
            else if (counter == 14)
            {
                sql = "SELECT billing_date, invoice_no, client, CONCAT(month, '/', year) AS 'monthyear', state_name, unit_gst_no, CAST(GROUP_CONCAT(DISTINCT grade_desc) AS char) AS 'Service', ROUND((SUM(service_charge) + SUM(amount) + SUM(uniform) + SUM(operational_cost) ), 2) AS 'Basic', ROUND(SUM(CGST9), 2) AS 'cgst', ROUND(SUM(IGST18), 2) as igst, ROUND(SUM(SGST9), 2) as sgst FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' and billing_date IS NOT NULL AND invoice_no IS NOT NULL GROUP BY invoice_no ORDER BY 3,5";
            }
            else if (counter == 15)
            {
                sql = "SELECT REPLACE(client_name, 'NEW', '') AS 'client_name', UPPER(concat(unit_add2,' ',pay_unit_master.state_name,' ',pay_unit_master.pincode)) AS 'unit_add2', UPPER(emp_name) AS 'emp_name', replace(group_concat(UPPER(ifnull(document_type,''))),',','/') AS 'document_type', replace(group_concat(ifnull(size,'')),',','/') as size, emp_mobile_no from pay_employee_master left join pay_document_details on pay_document_details.emp_code = pay_employee_master.emp_code and document_type != 'ID_Card' inner join pay_unit_master on pay_unit_master.unit_code = pay_employee_master.unit_code and pay_unit_master.comp_code = pay_employee_master.comp_code inner join pay_client_master on pay_unit_master.client_code = pay_client_master.client_code where pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' and id_card_dispatch_date = str_to_date('" + txt_print_list.Text + "','%d/%m/%Y') and pay_client_master.client_code = '" + ddl_client.SelectedValue + "' group by emp_name";
            }
            else if (counter == 16)
            {
                sql = "SELECT pay_pro_master.state_name, pay_pro_master.unit_city, pay_pro_master.emp_name, pay_billing_master_history.basic, pay_salary_unit_rate.month_days, total_days_present, ROUND(((pay_billing_master_history.basic / pay_salary_unit_rate.month_days) * total_days_present), 2) AS 'spl_bsc', ROUND(((((pay_billing_master_history.basic / pay_salary_unit_rate.month_days) * total_days_present) * pay_billing_master_history.sal_bonus)/100), 2) AS 'bonus', pay_billing_master_history.sal_bonus FROM pay_pro_master INNER JOIN pay_salary_unit_rate ON pay_salary_unit_rate.comp_code = pay_pro_master.comp_code AND pay_salary_unit_rate.unit_code = pay_pro_master.unit_code AND pay_salary_unit_rate.month = pay_pro_master.month AND pay_salary_unit_rate.year = pay_pro_master.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_pro_master.comp_code AND pay_billing_master_history.billing_unit_code = pay_pro_master.unit_code AND pay_billing_master_history.month = pay_pro_master.month AND pay_billing_master_history.year = pay_pro_master.year AND pay_salary_unit_rate.designation = pay_billing_master_history.designation INNER JOIN pay_billing_unit_rate_history ON pay_billing_unit_rate_history.emp_code = pay_pro_master.emp_code AND pay_billing_master_history.month = pay_pro_master.month AND pay_billing_master_history.year = pay_pro_master.year WHERE pay_pro_master.client_code = '" + ddl_client.SelectedValue + "' AND pay_pro_master.month = " + txttodate.Text.Substring(0, 2) + " AND pay_pro_master.year = " + txttodate.Text.Substring(3) + " and pay_billing_unit_rate_history.emp_type = 'Permanent'";
            }
            else if (counter == 17)
            {
                sql = "select client,upper(state_name) as state_name,upper(unit_city) as unit_city,emp_name,grade_desc,(month_days-tot_days_present) as absent from pay_billing_unit_rate_history WHERE client_code = '" + ddl_client.SelectedValue + "' AND month = " + txttodate.Text.Substring(0, 2) + " AND year = " + txttodate.Text.Substring(3) + " and emp_type = 'Permanent'";
            }
            else if (counter == 18)
            {
                sql = "SELECT pay_employee_master.EMP_NAME, EMP_FATHER_NAME AS 'FatherName', DATE_FORMAT(BIRTH_DATE, '%d/%m/%Y') AS 'Birth_date', DATE_FORMAT(JOINING_DATE, '%d/%m/%Y') AS 'Date_Of_Joining', PAN_NUMBER AS 'UAN_NO', ESIC_NUMBER, DATE_FORMAT(Left_date, '%d/%m/%Y') AS 'left_date', IF(LEFT_REASON != '', 'LEFT SERVICE', LEFT_REASON) AS 'Reason_Of_Exit', pay_unit_master.unit_city FROM pay_employee_master INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND pay_employee_master.unit_code = pay_unit_master.unit_code where " + where + " and emp_code not in (select emp_code FROM pay_employee_master INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND pay_employee_master.unit_code = pay_unit_master.unit_code where " + where + " and pay_employee_master.left_date < str_to_date('1/" + txttodate.Text.Substring(0, 2) + "/" + txttodate.Text.Substring(3) + "','%d/%m/%Y') ) order by JOINING_DATE";
            }
            MySqlDataAdapter dscmd = new MySqlDataAdapter(sql, d.con);
            DataSet ds = new DataSet();
            dscmd.Fill(ds);
            // int days = 0;


            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                if (counter == 1)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=FORM_XIII_REG_WORKMEN_EMPLOYED_BELOW_18.xls");
                }
                else if (counter == 2) { Response.AddHeader("content-disposition", "attachment;filename=FORM_XVI-Muster_Roll.xls"); }
                else if (counter == 3) { Response.AddHeader("content-disposition", "attachment;filename=FORM_XXIII-Register_of_overtime.xls"); }
                else if (counter == 4) { Response.AddHeader("content-disposition", "attachment;filename=FORM_XXII-Register_of_advances.xls"); }
                else if (counter == 5) { Response.AddHeader("content-disposition", "attachment;filename=FORM_XXI-Register_of_fines.xls"); }
                else if (counter == 6) { Response.AddHeader("content-disposition", "attachment;filename=FORM_XX-Reg-deduction.xls"); }
                else if (counter == 7) { Response.AddHeader("content-disposition", "attachment;filename=FORM_D_" + ddl_state.SelectedValue + ".xls"); }
                else if (counter == 8) { Response.AddHeader("content-disposition", "attachment;filename=FORM_A.xls"); }
                else if (counter == 9) { Response.AddHeader("content-disposition", "attachment;filename=FORM_B.xls"); }
                else if (counter == 10) { Response.AddHeader("content-disposition", "attachment;filename=FORM_XIII_REG_WORKMEN_EMPLOYED_ABOVE_18.xls"); }
                else if (counter == 11) { Response.AddHeader("content-disposition", "attachment;filename=ID_CARD_PRINT.xls"); }
                else if (counter == 12) { Response.AddHeader("content-disposition", "attachment;filename=GST.xls"); }
                else if (counter == 13) { Response.AddHeader("content-disposition", "attachment;filename=PROFESSIONAL_TAX.xls"); }
                else if (counter == 14) { Response.AddHeader("content-disposition", "attachment;filename=SALES_REGISTER.xls"); }
                else if (counter == 15) { Response.AddHeader("content-disposition", "attachment;filename=DISPATCH_ADDRESS_LIST.xls"); }
                else if (counter == 16) { Response.AddHeader("content-disposition", "attachment;filename=BONUS_REGISTER.xls"); }
                else if (counter == 17) { Response.AddHeader("content-disposition", "attachment;filename=LEAVE_REGISTER.xls"); }
                else if (counter == 18) { Response.AddHeader("content-disposition", "attachment;filename=EMPLOYEE_DATA.xls"); }

                string path = Server.MapPath("~/EMP_Images");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                string start_date_common = "1";
                if (counter != 14 && counter != 15 && counter != 16 && counter != 17 && counter != 18)
                { start_date_common = get_start_date(); }
                //if (txttodate.Text != "")
                //{
                //    days = DateTime.DaysInMonth(int.Parse(txttodate.Text.Substring(3)), int.Parse(txttodate.Text.Substring(0, 2)));
                //}
                Repeater1.HeaderTemplate = new MyTemplate(ListItemType.Header, ds, txttodate.Text, ddl_designation.SelectedItem.Text, ddl_client.SelectedItem.Text, counter, path, txttodate.Text, d.get_calendar_days(int.Parse(start_date_common), txttodate.Text, 0, 1), start_date_common,ddl_state.SelectedValue);
                Repeater1.ItemTemplate = new MyTemplate(ListItemType.Item, ds, "", ddl_designation.SelectedItem.Text, "", counter, path, txttodate.Text, "", "","");
                Repeater1.FooterTemplate = new MyTemplate(ListItemType.Footer, ds, "", "", "", 0, "", "", "", "a","");
                Repeater1.DataBind();

                System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                Repeater1.RenderControl(htmlWrite);

                string style = @"<style> .textmode { } </style>";
                Response.Write(style);
                Response.Output.Write(stringWrite.ToString());
                Response.Flush();
                Response.End();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Matching Records Found.');", true);
            }
            d.con.Close();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    protected void btn_form_13_Click(object sender, EventArgs e)
    {
        generate_letter(1);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    public class MyTemplate : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        string year;
        string designation;
        string client,state;
        int counter;
        int daysadd = 0, month_days = 0;

        static int ctr;
        string path = "", header1 = "", header = "", start_date_common = "";
        string payment_date;

        public MyTemplate(ListItemType type, DataSet ds, string year, string designation, string client, int counter, string path, string payment_date, string header1, string start_date_common,string state)
        {
            this.counter = counter;
            this.type = type;
            this.ds = ds;
            this.year = year;
            this.designation = designation;
            this.client = client;
            this.path = path;
            this.state = state;
            ctr = 0;
            this.payment_date = payment_date;
            this.header1 = header1;
            this.start_date_common = start_date_common;
            if (counter == 7)
            {
                header = "<th>1</th><th>2</th><th>3</th><th>4</th><th>5</th><th>6</th><th>7</th><th>8</th><th>9</th><th>10</th><th>11</th><th>12</th><th>13</th><th>14</th><th>15</th><th>16</th><th>17</th><th>18</th><th>19</th><th>20</th><th>21</th><th>22</th><th>23</th><th>24</th><th>25</th><th>26</th><th>27</th><th>28</th>";
                month_days = int.Parse(ds.Tables[0].Rows[ctr]["total days"].ToString());
                if (month_days == 29)
                { header = header + "<th>29</th>"; daysadd = 1; }
                else if (month_days == 30)
                {
                    header = header + "<th>29</th><th>30</th>"; daysadd = 2;
                }
                else if (month_days == 31)
                {
                    header = header + "<th>29</th><th>30</th><th>31</th>"; daysadd = 3;
                }
            }
        }
        public void InstantiateIn(Control container)
        {
            switch (type)
            {
                case ListItemType.Header:
                    if (counter == 1 || counter == 10)
                    {
                        lc = new LiteralControl("<table border=1><tr><td colspan=12 align=center><b>FORM XIII</b></td></tr><tr><td colspan=12 align=center>REGISTER OF WORKMEN EMPLOYED BY CONTRACTOR</td></tr><tr><td colspan=12 align=center>[Rule 75]</td></tr><tr><td colspan=12> </td></tr><tr><td colspan=6 align=left>Name and address of contractor <b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td><td colspan=6 align=left>Name and address of establishment in/under <b>which contract is carried on " + year.Substring(3) + "</b></td></tr><tr><td colspan=12> </td></tr><tr><td colspan=6 align=left>Nature and location of work <b>" + designation + " Services</b></td><td colspan=6 align=left>Name and address of Principal Employer <b>" + client + "</b></td></tr><tr><td colspan=12> </td></tr><tr><th>Sl. No.</th><th>Name and surname of workman</th><th>Age and Sex</th><th>Father's/ Husband's name</th><th>Nature of employment/ Designation</th><th>Permanent Home Address of workman (Village and Tahsil/Taluk and District</th><th>Local Address</th><th>Date of commencement of employment</th><th>Signature or thumb impression of workman</th><th>Date of termination of employment</th><th>Reasons for termination</th><th>Remarks</th></tr>");
                        break;
                    }
                    else if (counter == 2)
                    {
                        lc = new LiteralControl("<table border=1><tr><td colspan=6 align=center><b>FORM XVI</b></td></tr><tr><td colspan=6 align=center><b>MUSTER ROLL</b></td></tr><tr><td colspan=6 align=center>[Rule 78(1)(a)(i)]</td></tr><tr><td colspan=6> </td></tr><tr><td colspan=3 align=left>Name and address of contractor <b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td><td colspan=3 align=left>Name and address of establishment in/under which contract is carried on " + year.Substring(3) + "</td></tr><tr><td colspan=6> </td></tr><tr><td colspan=3 align=left>Nature and location of work <b>" + designation + " Services</b></td><td colspan=3 align=left>Name and address of Principal Employer <b>" + client + "</b></td></tr><tr><td colspan=6> </td></tr><tr><th>Sl. No.</th><th>Name of workman</th><th>Father's/ Husband's name</th><th>Sex</th><th>Dates</th><th>Remarks</th></tr>");
                        break;
                    }
                    else if (counter == 3)
                    {
                        lc = new LiteralControl("<table border=1><tr><td colspan=12 align=center><b>FORM XXIII</b></td></tr><tr><td colspan=12 align=center>REGISTER OF OVERTIME</td></tr><tr><td colspan=12 align=center>[Rule 78(1)(a)(iii)]</td></tr><tr><td colspan=12> </td></tr><tr><td colspan=6 align=left>Name and address of contractor <b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td><td colspan=6 align=left>Name and address of establishment in/under <b>which contract is carried on " + year.Substring(3) + "</b></td></tr><tr><td colspan=12> </td></tr><tr><td colspan=6 align=left>Nature and location of work <b>" + designation + " Services</b></td><td colspan=6 align=left>Name and address of Principal Employer <b>" + client + "</b></td></tr><tr><td colspan=12> </td></tr><tr><th>Sl. No.</th><th>Name of workman</th><th>Father's/ Husband's name</th><th>Sex</th><th>Nature of employment / Designation</th><th>Date on which overtime worked</th><th>Total overtime worked or production in case of piece rated</th><th>Normal rates of wages</th><th>Overtime rate of wages</th><th>Overtime earnings</th><th>Date on which overtime wages paid</th><th>Remarks</th></tr>");
                        break;
                    }
                    else if (counter == 4)
                    {
                        lc = new LiteralControl("<table border=1><tr><td colspan=11 align=center><b>FORM XXII</b></td></tr><tr><td colspan=11 align=center>REGISTER OF ADVANCES</td></tr><tr><td colspan=11 align=center>[Rule 78(1)(a)(iii)]</td></tr><tr><td colspan=11> </td></tr><tr><td colspan=6 align=left>Name and address of contractor <b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td><td colspan=5 align=left>Name and address of establishment in/under <b>which contract is carried on " + year.Substring(3) + "</b></td></tr><tr><td colspan=11> </td></tr><tr><td colspan=6 align=left>Nature and location of work <b>" + designation + " Services</b></td><td colspan=5 align=left>Name and address of Principal Employer <b>" + client + "</b></td></tr><tr><td colspan=11> </td></tr><tr><th>Sl. No.</th><th>Name of workman</th><th>Father's/ Husband's name</th><th>Nature of employment / Designation</th><th>Wage period and wages payable</th><th>Date and amount of advance given</th><th>Purpose (s) for which advance made</th><th>No. of installments by which advance to be repaid</th><th>Date and amount of each installment repaid</th><th>Date on which last installment was repaid</th><th>Remarks</th></tr>");
                        break;
                    }
                    else if (counter == 5)
                    {
                        lc = new LiteralControl("<table border=1><tr><td colspan=12 align=center><b>FORM XXI</b></td></tr><tr><td colspan=12 align=center>REGISTER OF FINES</td></tr><tr><td colspan=12 align=center>[Rule 78(1)(a)(ii)]</td></tr><tr><td colspan=12> </td></tr><tr><td colspan=6 align=left>Name and address of contractor <b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td><td colspan=6 align=left>Name and address of establishment in/under <b>which contract is carried on " + year.Substring(3) + "</b></td></tr><tr><td colspan=12> </td></tr><tr><td colspan=6 align=left>Nature and location of work <b>" + designation + " Services</b></td><td colspan=6 align=left>Name and address of Principal Employer <b>" + client + "</b></td></tr><tr><td colspan=12> </td></tr><tr><th>Sl. No.</th><th>Name of workman</th><th>Father's/ Husband's name</th><th>Nature of employment / Designation</th><th>Act/Omission for which fine imposed</th><th>Date of offence</th><th>Whether workman showned cause against fine</th><th>Name of person in whose presence employee's explaination was heard</th><th>Wage periods and wages payable</th><th>Amount of fine imposed</th><th>Date on which fine realised</th><th>Remarks</th></tr>");
                        break;
                    }
                    else if (counter == 6)
                    {
                        lc = new LiteralControl("<table border=1><tr><td colspan=15 align=center><b>FORM XX</b></td></tr><tr><td colspan=15 align=center>REGISTER OF DEDUCTIONS FOR DAMAGE OR LOSS</td></tr><tr><td colspan=15 align=center>[Rule 78(1)(a)(ii)]</td></tr><tr><td colspan=15> </td></tr><tr><td colspan=9 align=left>Name and address of contractor <b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td><td colspan=6 align=left>Name and address of establishment in/under <b>which contract is carried on " + year.Substring(3) + "</b></td></tr><tr><td colspan=15> </td></tr><tr><td colspan=9 align=left>Nature and location of work <b>" + ds.Tables[0].Rows[ctr]["grade_code"] + " Services</b></td><td colspan=6 align=left>Name and address of Principal Employer <b>" + client + "</b></td></tr><tr><td colspan=15> </td></tr><tr><th>Sl. No.</th><th>Name of workman</th><th>State</th><th>Location</th><th>Father's/ Husband's name</th><th>Nature of employment / Designation</th><th>Particulars of damage or loss</th><th>Date of damage or loss</th><th>Whether workman showned cause against deduction</th><th>Name of person in whose presence employee's explaination was heard</th><th>Amount of deduction imposed</th><th>No. of installments</th><th colspan=2>Date of recovery</th><th>Remarks</th></tr><tr><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th>First Installment</th><th>Last Installment</th><th></th></tr>");
                        break;
                    }
                    else if (counter == 7)
                    {
                        if (start_date_common != "" && start_date_common != "1")
                        {
                            header = header1;
                        }

                        lc = new LiteralControl("<table border=1><tr><td colspan=" + (month_days + 8) + " align=center><b>FORM D</b></td></tr><tr><td colspan=<td colspan=" + (month_days + 8) + " align=center>" + state + "</td></tr><tr><td colspan=" + (month_days + 8) + "></td></tr><tr><td colspan=12>Name of establishment - <b>" + client + "</b></td><td colspan=13>Name of Owner - <b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td><td colspan=" + ((month_days + 9) - 26) + ">LIN - " + ds.Tables[0].Rows[ctr]["LICENSE_NO"] + " </td></tr><tr><td colspan=" + (month_days + 8) + "></td></tr><tr><td colspan=" + (month_days + 8) + " align=left>For the Period From <b>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</b></td></tr><tr><td colspan=" + (month_days + 8) + "></td></tr><tr><th>Sl. No.</th><th>Sl. No. in Emp. Register</th><th>Name</th><th>Relay # or set work</th><th>Place of Work</th><th colspan=" + month_days + ">Dates</th><th>Summary of days</th><th>Remarks No. of hours</th><th>Signature of Register keeper</th></tr><tr><th></th><th></th><th></th><th></th><th></th>" + header + "</th><th></th><th></th></tr>");
                        break;
                    }
                    else if (counter == 8)
                    {
                        lc = new LiteralControl("<table border=1><tr><td colspan=31 align=center><b>SCHEDULE</b></td></tr><tr><td colspan=<td colspan=31 align=center>[ See rule 2(1) ]</td></tr><tr><td colspan=31 align=center>FORM A</td></tr><tr><td colspan=31 align=center>[PART-A]</td></tr><tr><td colspan=15>Name of establishment - <b>" + client + "</b></td><td colspan=16>Name of Owner - <b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td></tr><tr><td colspan=31> </td></tr><tr><th>Sr.No.</th><th>Employee Code</th><th>Name and Surname</th><th>Gender</th><th>Father's/Spouse Name</th><th>Date of Birth</th><th>Nationality</th><th>Education Level</th><th>Date of Joining</th><th>Designation</th><th>Category Address *(HS/S/SS/US)</th><th>Type of Employment</th><th>Mobile</th><th>UAN</th><th>PAN</th><th>PF</th><th>ESI IP</th><th>LWF</th><th>AADHAR</th><th>Bank A/c No.</th><th>Bank</th><th>Branch (IFSC)</th><th>Present Address</th><th>Permanent</th><th>Service Book No.</th><th>Date of Exit</th><th>Reason for Exit</th><th>Mark of Identification</th><th>Employee Photo</th><th>Specimen Signature/Thumb Impression</th><th>Remarks</th></tr>");
                        break;
                    }
                    else if (counter == 9)
                    {
                        lc = new LiteralControl("<table border=1><tr><td colspan=37 align=center><b>FORM B</b></td></tr><tr><td colspan=37 align=center><b>" + ds.Tables[0].Rows[ctr]["state_name"] + "</b></td></tr><tr><td colspan=37 align=center><b>Rate of Minimum Wages and since the date</b></td></tr><tr><td colspan=5></td><td colspan=8 align=center><b>Highly Skilled</b></td><td colspan=10 align=center><b>Skilled</b></td><td colspan=8 align=center><b>Semi Skilled</b></td><td colspan=6 align=center><b>Un Skilled</b></td></tr><tr><td colspan=5 aign=left><b>Minimum Basic</b></td><td colspan=8 align=center>N/A</td><td colspan=10 align=center>N/A</td><td colspan=8 align=center>" + ds.Tables[0].Rows[ctr]["Basic"] + "</td><td colspan=6 align=center>N/A</td></tr><tr><td colspan=5 aign=left><b>DA</b></td><td colspan=8></td><td colspan=10</td><td colspan=8></td><td colspan=6></td></tr><tr><td colspan=5 aign=left><b>Overtime</b></td><td colspan=8></td><td colspan=10></td><td colspan=8></td><td colspan=6></td></tr><tr><td colspan=14>Name of establishment - <b>" + client + "</b></td><td colspan=12>Name of Owner - <b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td><td colspan=11>LIN - <b>" + ds.Tables[0].Rows[ctr]["LICENSE_NO"] + "</td></tr><tr><td colspan=37><b>Wage period From 1/" + year + " To " + month_days + "/" + year + " (Monthly/Fortnightly/Weekly/Daily/Piece Rated)</b></td></tr><tr><td colspan=37></td></tr><tr><td colspan=7></td><td colspan=13 align=center><b>Earned Wages</b></td><td align=center colspan=12><b>Deduction</b></td><td colspan=5></td></tr><tr><th>Employee Code</th><th>Sr.No</th><th>Sl. No. in Employee register</th><th>Name and Surname</th><th>Rate of Wage</th><th>No. of Days worked</th><th>Overtime hours worked</th><th>Basic</th><th>Special Basic</th><th>DA</th><th>SA</th><th>Payments Overtime</th><th>HRA</th><th colspan=6>Others</th><th>Total</th><th>PF</th><th>ESIC</th><th>LWF</th><th>PT</th><th>Society</th><th>Income Tax</th><th>Insurance</th><th colspan=3>Others</th><th>Recoveries</th><th>Total</th><th>Net payment</th><th>Employer Share PF Welfare Fund</th><th>Receipt by Employee/Bank Transaction ID</th><th>Date of Payment</th><th>Remarks</th></tr><tr><th>1</th><th>2</th><th>3</th><th>4</th><th>5</th><th>6</th><th>7</th><th>8</th><th>9</th><th>10</th><th>11</th><th>12</th><th>13</th><th colspan=6>14</th><th>15</th><th>16</th><th>17</th><th>18</th><th>19</th><th>20</th><th>21</th><th>22</th><th colspan=3>23</th><th>24</th><th>25</th><th>26</th><th>27</th><th>28</th><th>29</th><th>30</th></tr><tr><th colspan=13></th><th>Bonus</th><th>Advance Bonus</th><th>Leave</th><th>Advance Leave</th><th>Allowance</th><th>Total Others</th><th colspan=8></th><th>Fine</th><th>Advance</th><th>Total Others</th><th colspan=7></th></tr>");
                        break;
                    }
                    else if (counter == 12)
                    {
                        lc = new LiteralControl("<table border=1><tr><th>SR NO</th><th>CLIENT NAME</th><th>STATE</th><th>CLIENT GST</th><th>CGST</th><th>IGST</th><th>SGST</th></tr>");
                        break;
                    }
                    else if (counter == 13)
                    {
                        lc = new LiteralControl("<table border=1><tr><th>SR NO</th><th>CLIENT NAME</th><th>STATE</th><th>BRANCH CITY</th><th>EMPLOYEE NAME</th><th>DESIGNATION</th><th>GROSS</th><th>PT AMOUNT</th></tr>");
                        break;
                    }
                    else if (counter == 14)
                    {
                        lc = new LiteralControl("<table border=1><tr><th>SR NO</th><th>BILLING DATE</th><th>INVOICE NUMBER</th><th>CLIENT NAME</th><th>MONTH/YEAR</th><th>STATE</th><th>CLIENT GST NO.</th><th>SERVICE</th><th>BASIC</th><th>CGST</th><th>IGST</th><th>SGST</th></tr>");
                        break;
                    }
                    else if (counter == 15)
                    {
                        lc = new LiteralControl("<table border=1>");
                        break;
                    }
                    else if (counter == 16)
                    {
                        DateTimeFormatInfo mfi = new DateTimeFormatInfo();
                        lc = new LiteralControl("<table border=1><tr><th colspan=9>" + client + " BONUS RETURN - FORM D</th></tr><tr><th colspan=9>MONTH - "+mfi.GetMonthName(int.Parse(year.Substring(0,2))).ToString()+"-"+year.Substring(3)+"</th></tr><tr><th>SR NO</th><th>STATE</th><th>LOCATON</th><th>NAME</th><th>BASIC</th><th>TOTAL PRESENT DAYS</th><th>NO. OF DAYS WORKED</th><th>SPECIAL BONUS</th><th>BONUS @" + ds.Tables[0].Rows[ctr]["sal_bonus"] + "%</th></tr>");
                        break;
                    }
                    else if (counter == 17)
                    {
                        DateTimeFormatInfo mfi = new DateTimeFormatInfo();
                        lc = new LiteralControl("<table border=1><tr><th colspan=7>LEAVE RECORD</th></tr><tr><th colspan=7>MONTH - " + mfi.GetMonthName(int.Parse(year.Substring(0, 2))).ToString() + "-" + year.Substring(3) + "</th></tr><tr><th>SR NO</th><th>CLIENT</th><th>STATE</th><th>LOCATON</th><th>NAME OF THE EMPLOYEE</th><th>DESIGNATION</th><th>ABSENT/LEAVE</th></tr>");
                        break;
                    }
                    else if (counter == 18)
                    {
                        lc = new LiteralControl("<table border=1><tr><th>SR. NO</th><th>EMPLOYEE NAME</th><th>FATHER NAME</th><th>DOB</th><th>DOJ</th><th>LOCATION DEPUTED</th><th>UAN NO</th><th>ESIC NO</th><th>DOL</th><th>REMARK</th></tr>");
                        break;
                    }
                    else
                    {
                        lc = new LiteralControl("<table border=1><tr><th>SR NO</th><th>EMPLOYEE NAME</th><th>EMPLOYEE ID</th><th>DEG</th><th>BLOOD GROUP</th><th>DOJ</th><th>CLIENT NAME</th><th>EMPLOYEE PHOTO</th></tr>");
                        break;
                    }
                case ListItemType.Item:
                    if (counter == 1 || counter == 10)
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr]["id_as_per_dob"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Gender"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_father_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["designation"] + "</td><td>" + ds.Tables[0].Rows[ctr]["a"] + "</td><td>" + ds.Tables[0].Rows[ctr]["b"] + "</td><td>" + ds.Tables[0].Rows[ctr]["joining_date"] + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["left_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["left_reason"] + "</td><td></td></tr>");
                    }
                    else if (counter == 2)
                    {
                        lc = new LiteralControl("<tr align=center><td>" + ds.Tables[0].Rows[ctr]["id_as_per_dob"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_father_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Gender"] + "</td><td>" + ds.Tables[0].Rows[ctr]["joining_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["left_reason"] + "</td></tr>");
                    }
                    else if (counter == 3)
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr]["id_as_per_dob"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_father_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Gender"] + "</td><td>" + designation + "</td><td>NO OVERTIME</td><td>N/A</td><td>N/A</td><td>N/A</td><td>N/A</td><td>N/A</td><td></td></tr>");
                    }
                    else if (counter == 4)
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr]["id_as_per_dob"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_father_name"] + "</td><td>" + designation + "</td><td>N/A</td><td>NOT GIVEN</td><td>N/A</td><td>N/A</td><td>N/A</td><td>N/A</td><td></td></tr>");
                    }
                    else if (counter == 5)
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr]["id_as_per_dob"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_father_name"] + "</td><td>" + designation + "</td><td>NO FINE</td><td>N/A</td><td>NO</td><td>N/A</td><td>N/A</td><td>N/A</td><td>N/A</td><td></td></tr>");
                    }
                    else if (counter == 6)
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr]["id_as_per_dob"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_father_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["designation"] + "</td><td>N/A</td><td>N/A</td><td>NO</td><td>N/A</td><td>NO</td><td>N/A</td><td>N/A</td><td>N/A</td><td></td></tr>");
                    }
                    else if (counter == 7)
                    {
                        string days = ""; if (month_days == 29) { days = "<td " + (ds.Tables[0].Rows[ctr]["DAY29"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY29"] + "</td>"; } else if (month_days == 30) { days = "<td " + (ds.Tables[0].Rows[ctr]["DAY29"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY29"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY30"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY30"] + "</td>"; } else if (month_days == 31) { days = "<td " + (ds.Tables[0].Rows[ctr]["DAY29"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY29"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY30"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY30"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY31"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY31"] + "</td>"; }
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["unit_name"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY01"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY01"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY02"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY02"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY03"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY03"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY04"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY04"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY05"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY05"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY06"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY06"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY07"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY07"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY08"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY08"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY09"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY09"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY10"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY10"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY11"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY11"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY12"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY12"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY13"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY13"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY14"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY14"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY15"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY15"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY16"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY16"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY17"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY17"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY18"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY18"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY19"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY19"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY20"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY20"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY21"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY21"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY22"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY22"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY23"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY23"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY24"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY24"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY25"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY25"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY26"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY26"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY27"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY27"] + "</td><td " + (ds.Tables[0].Rows[ctr]["DAY28"].ToString() != "P" ? "bgcolor=red" : "") + ">" + ds.Tables[0].Rows[ctr]["DAY28"] + "</td>" + days + "<td>" + ds.Tables[0].Rows[ctr]["TOT_DAYS_PRESENT"] + "</td><td></td><td></td></tr>");
                    }
                    else if (counter == 8)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["id_as_per_dob"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Gender"] + "</td><td>" + ds.Tables[0].Rows[ctr]["FatherName"] + "</td><td>" + ds.Tables[0].Rows[ctr]["birth_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["nationality"] + "</td><td>" + ds.Tables[0].Rows[ctr]["QUALIFICATION_1"] + "</td><td>" + ds.Tables[0].Rows[ctr]["date_of_joining"] + "</td><td>" + ds.Tables[0].Rows[ctr]["designation"] + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["employee_type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["employee_no"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["uan_no"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["EMP_NEW_PAN_NO"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["pf_no"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["esic_number"] + "</td><td>" + ds.Tables[0].Rows[ctr]["lwf"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["adhar_no"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["bank_account_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["bankname"] + "</td><td>" + ds.Tables[0].Rows[ctr]["IFSC_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["present"] + "</td><td>" + (ds.Tables[0].Rows[ctr]["permanent"].ToString() == ds.Tables[0].Rows[ctr]["present"].ToString()?"DO":ds.Tables[0].Rows[ctr]["permanent"]) + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["left_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["reason_of_exit"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_identitymark"] + "</td><td></td><td></td><td></td></tr>");
                    }
                    else if (counter == 9)
                    {
                        lc = new LiteralControl("<tr><td align=center>" + ds.Tables[0].Rows[ctr]["id_as_per_dob"] + "</td><td align=center>" + (ctr + 1) + "</td><td align=center></td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["rate_of_wage"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Total_Days_Present"] + "</td><td>" + ds.Tables[0].Rows[ctr]["ot_hours"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["Basic"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["Special_Basic"] + "</td><td></td><td></td><td>'" + ds.Tables[0].Rows[ctr]["ot_amount"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["HRA"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["Bonus"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["sal_bonus_after_gross"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["leave_gross"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["leave_sal_after_gross"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["allow"] + "</td><td>'" + (double.Parse(ds.Tables[0].Rows[ctr]["Bonus"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["leave_gross"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["allow"].ToString())) + "</td><td>'" + ds.Tables[0].Rows[ctr]["total_earnings"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["PF"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["ESIC"] + "</td><td>" + ds.Tables[0].Rows[ctr]["LWF"] + "</td><td>" + ds.Tables[0].Rows[ctr]["PT"] + "</td><td></td><td></td><td></td><td>" + ds.Tables[0].Rows[ctr]["fine"] + "</td><td>" + ds.Tables[0].Rows[ctr]["advance"] + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["fine"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["advance"].ToString())) + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["total_deduction"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["net_payment"] + "</td><td></td><td></td><td>07/" + payment_date + "</td><td></td></tr>");
                    }
                    else if (counter == 12)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_gst_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Igst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan=4>Total</td><td>=ROUND(SUM(E2:E" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(F2:F" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(G2:G" + (ctr + 2) + "),2)</td></tr>";
                        }
                    }
                    else if (counter == 13)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grade"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gross"] + "</td><td>" + ds.Tables[0].Rows[ctr]["pt_amount"] + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan=7>Total</td><td>=ROUND(SUM(H2:H" + (ctr + 2) + "),2)</td></tr>";
                        }
                    }
                    else if (counter == 14)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["billing_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["client"] + "</td><td>" + ds.Tables[0].Rows[ctr]["monthyear"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_gst_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["service"] + "</td><td>" + ds.Tables[0].Rows[ctr]["basic"] + "</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["igst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan=8>Total</td><td>=ROUND(SUM(I2:I" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(J2:J" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(K2:K" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(L2:L" + (ctr + 2) + "),2)</td></tr>";
                        }
                    }
                    else if (counter == 15)
                    {
                        lc = new LiteralControl("<tr><td style=font-weight:Calibri;font-size:26px;width:700px;><b>M/S. " + ds.Tables[0].Rows[ctr]["client_name"] + "</b></td></tr><tr><td style=font-weight:Calibri;font-size:18px;width:650px;><b>ADDRESS : " + ds.Tables[0].Rows[ctr]["unit_add2"] + "</b></td></tr><tr><td style=font-weight:Calibri;font-size:18px;width:650px;><b>NAME : " + ds.Tables[0].Rows[ctr]["emp_name"] + "</b></td></tr><tr><td style=font-weight:Calibri;font-size:18px;width:650px;><b>" + ds.Tables[0].Rows[ctr]["document_type"] + " - " + ds.Tables[0].Rows[ctr]["size"] + "</b></td></tr><tr><td  style=font-weight:Calibri;font-size:18px;width:650px;><b>CONTACT NO. : " + ds.Tables[0].Rows[ctr]["emp_mobile_no"] + "</b></td></tr><tr><td></td></tr><tr><td></td></tr><tr><td></td></tr>");
                    }
                    else if (counter == 16)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["basic"] + "</td><td>" + ds.Tables[0].Rows[ctr]["total_days_present"] + "</td><td>" + ds.Tables[0].Rows[ctr]["spl_bsc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["bonus"] + "</td></tr>");

                    }
                    else if (counter == 17)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["absent"] + "</td></tr>");

                    }
                    else if (counter == 18)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["fathername"] + "</td><td>" + ds.Tables[0].Rows[ctr]["birth_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["date_of_joining"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["uan_no"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["esic_number"] + "</td><td>" + ds.Tables[0].Rows[ctr]["left_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["reason_of_exit"] + "</td></tr>");

                    }
                    else
                    {
                        string image_path = "<td align='center' height=130 width=175><img src= '" + path + "\\" + ds.Tables[0].Rows[ctr]["EMP_PHOTO"].ToString() + "' id='myimage' height='125' width='160' align='middle' />";
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["id_as_per_dob"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_blood_group"] + "</td><td>" + ds.Tables[0].Rows[ctr]["DOJ"] + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td>" + (ds.Tables[0].Rows[ctr]["EMP_PHOTO"].ToString() == "" ? "<td>" : image_path) + "</td></tr>");
                    }
                    ctr++;
                    break;
                case ListItemType.Footer:
                    lc = new LiteralControl("</table>");
                    ctr = 0;
                    break;
            }
            container.Controls.Add(lc);
        }
    }
    protected void btn_form_14_Click(object sender, EventArgs e)
    {
        generate_recu_report(1);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    private void generate_recu_report(int counter)
    {
        string sql = null;
        int month = int.Parse(txttodate.Text.Substring(0, 2));

        d.con.Open();
        string where = " pay_unit_master.comp_code= '" + Session["COMP_CODE"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "'";
        if (!ddl_state.SelectedValue.ToUpper().Equals("ALL"))
        { where = where + " and pay_unit_master.state_name= '" + ddl_state.SelectedValue + "'"; }
        if (!ddlunitselect.SelectedValue.ToUpper().Equals("ALL"))
        {
            where = where + " and pay_unit_master.unit_code= '" + ddl_state.SelectedValue + "'";
        }
        if (!ddl_designation.SelectedValue.ToUpper().Equals("ALL"))
        { where = where + " and pay_employee_master.grade_code = '" + ddl_designation.SelectedValue + "'"; }

        if (ddl_act.SelectedValue == "MH SG ACT")
        {
            where = where + " and pay_employee_master.grade_code = 'SG'";
        }

        //sql = "SELECT Left_date, emp_name, joining_date, left_date1, leftreason, company, wage, address, BIRTH_DATE, EMP_IDENTITYMARK, emp_father_name, (emp_basic_vda + hra_amount_salary + sal_bonus_gross + leave_sal_gross + washing_salary + travelling_salary + education_salary + allowances_salary + cca_salary + other_allow + gratuity_gross + sal_ot) AS 'gross' FROM (SELECT Left_date, emp_name, DATE_FORMAT(joining_date, '%d/%m/%Y') AS 'joining_date', CASE WHEN Left_date IS NULL THEN 'TILL DATE' ELSE Left_date END AS 'left_date1', CASE WHEN Left_date IS NULL THEN 'CONTINUE' ELSE 'LEFT' END AS 'leftreason', (SELECT CONCAT(company_name, ' ', ADDRESS1, ' ', CITY, ' ', STATE) FROM pay_company_master WHERE pay_company_master.comp_code = pay_employee_master.comp_code) AS 'company', (pay_billing_master_history.basic_salary + pay_billing_master_history.vda_salary) AS 'wage', CONCAT(EMP_CURRENT_ADDRESS, ' ', EMP_CURRENT_CITY, ' ', EMP_CURRENT_STATE, ' ', EMP_CURRENT_PIN) AS 'address', DATE_FORMAT(BIRTH_DATE, '%d/%m/%Y') AS 'BIRTH_DATE', EMP_IDENTITYMARK, emp_father_name, (((pay_billing_master_history.basic_salary + pay_billing_master_history.vda_salary) / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'emp_basic_vda', ((pay_salary_unit_rate.hra_amount_salary / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'hra_amount_salary', ((pay_salary_unit_rate.sal_bonus / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'sal_bonus_gross', ((pay_salary_unit_rate.leave_days / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'leave_sal_gross', ((pay_salary_unit_rate.washing_salary / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'washing_salary', ((pay_salary_unit_rate.travelling_salary / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'travelling_salary', ((pay_salary_unit_rate.education_salary / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'education_salary', ((pay_salary_unit_rate.allowances_salary / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'allowances_salary', CASE WHEN pay_employee_master.cca = 0 THEN ((pay_salary_unit_rate.cca_salary / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE pay_employee_master.cca END AS 'cca_salary', CASE WHEN pay_employee_master.special_allow = 0 THEN ((pay_billing_master_history.other_allow / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE pay_employee_master.special_allow END AS 'other_allow', CASE WHEN pay_employee_master.gratuity = 0 THEN CASE WHEN gratuity_taxable_salary = '1' THEN ((pay_salary_unit_rate.gratuity_salary / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END ELSE pay_employee_master.gratuity END AS 'gratuity_gross', CASE WHEN pay_employee_master.gratuity = 0 THEN CASE WHEN gratuity_taxable_salary = '0' THEN ((pay_salary_unit_rate.gratuity_salary / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END ELSE pay_employee_master.gratuity END AS 'gratuity_after_gross', ((pay_salary_unit_rate.ot_amount / pay_salary_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'sal_ot' FROM pay_employee_master INNER JOIN pay_attendance_muster ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_muster.unit_code = pay_unit_master.unit_code AND pay_attendance_muster.comp_code = pay_unit_master.comp_code INNER JOIN pay_salary_unit_rate ON pay_attendance_muster.unit_code = pay_salary_unit_rate.unit_code AND pay_attendance_muster.month = pay_salary_unit_rate.month AND pay_attendance_muster.year = pay_salary_unit_rate.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_salary_unit_rate.comp_code AND pay_billing_master_history.billing_client_code = pay_salary_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_salary_unit_rate.unit_code AND pay_billing_master_history.month = pay_salary_unit_rate.month AND pay_billing_master_history.year = pay_salary_unit_rate.year AND pay_employee_master.grade_code = pay_billing_master_history.designation AND pay_billing_master_history.designation = pay_salary_unit_rate.designation AND pay_billing_master_history.hours = pay_salary_unit_rate.hours INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code WHERE pay_attendance_muster.month = '" + month + "' AND pay_attendance_muster.year = '" + txttodate.Text.Substring(3) + "' AND pay_employee_master.Employee_type = 'Permanent' AND pay_attendance_muster.tot_days_present > 0 and "+where+") AS Form15";

        sql = "SELECT date_format(Left_date,'%d/%m/%Y') as left_date, pay_pro_master.emp_name, DATE_FORMAT(pay_employee_master.joining_date, '%d/%m/%Y') AS 'joining_date', CASE WHEN Left_date IS NULL THEN 'TILL DATE' ELSE DATE_FORMAT(Left_date, '%d/%m/%Y') END AS 'left_date1', CASE WHEN Left_date IS NULL THEN 'CONTINUE' ELSE 'LEFT SERVICE' END AS 'leftreason', (SELECT CONCAT(company_name, ' ', ADDRESS1, ' ', CITY, ' ', STATE) FROM pay_company_master WHERE pay_company_master.comp_code = pay_employee_master.comp_code) AS 'company', round((emp_basic_vda/total_days_present),2) AS 'wage', CONCAT(EMP_CURRENT_ADDRESS, ' ', EMP_CURRENT_CITY, ' ', EMP_CURRENT_STATE, ' ', EMP_CURRENT_PIN) AS 'address', DATE_FORMAT(BIRTH_DATE, '%d/%m/%Y') AS 'BIRTH_DATE', EMP_IDENTITYMARK, emp_father_name, id_as_per_dob, upper(concat(if(pay_employee_master.grade_code='SG','SG','HK'),' Services ', pay_pro_master.unit_city)) as grade_code, (select grade_desc from pay_grade_master where pay_grade_master.grade_code = pay_employee_master.grade_code limit 1) as designation FROM pay_employee_master INNER JOIN pay_unit_master ON pay_employee_master.unit_code = pay_unit_master.unit_code AND pay_employee_master.comp_code = pay_unit_master.comp_code INNER JOIN pay_pro_master ON pay_pro_master.comp_code = pay_unit_master.comp_code AND pay_pro_master.client_code = pay_unit_master.client_code AND pay_pro_master.unit_code = pay_unit_master.unit_code AND pay_pro_master.emp_code = pay_employee_master.emp_code INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code WHERE pay_pro_master.month = '" + month + "' AND pay_pro_master.year = '" + txttodate.Text.Substring(3) + "' AND pay_employee_master.Employee_type = 'Permanent' AND " + where + " group by pay_employee_master.emp_code order by pay_employee_master.joining_date";

        MySqlDataAdapter dscmd = new MySqlDataAdapter(sql, d.con);
        DataSet ds = new DataSet();
        dscmd.Fill(ds);

        if (ds.Tables[0].Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            if (counter == 1)
            {
                Response.AddHeader("content-disposition", "attachment;filename=FORM_XIV-Employment_card.xls");
            }
            else
            { Response.AddHeader("content-disposition", "attachment;filename=FORM_XV-Service_Certificate.xls"); }
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Repeater Repeater1 = new Repeater();
            Repeater1.DataSource = ds;
            Repeater1.HeaderTemplate = new MyTemplate1(ListItemType.Header, null, "", "", "", 0);
            Repeater1.ItemTemplate = new MyTemplate1(ListItemType.Item, ds, txttodate.Text.Substring(3), ddl_designation.SelectedItem.Text, ddl_client.SelectedItem.Text, counter);
            Repeater1.FooterTemplate = new MyTemplate1(ListItemType.Footer, null, "", "", "", 0);
            Repeater1.DataBind();

            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            Repeater1.RenderControl(htmlWrite);

            string style = @"<style> .textmode { } </style>";
            Response.Write(style);
            Response.Output.Write(stringWrite.ToString());
            Response.Flush();
            Response.End();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Matching Records Found.');", true);
        }
        d.con.Close();
    }

    public class MyTemplate1 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        string year;
        string designation;
        string client;
        static int ctr;
        int counter;
        public MyTemplate1(ListItemType type, DataSet ds, string year, string designation, string client, int counter)
        {
            this.type = type;
            this.ds = ds;
            this.year = year;
            this.designation = designation;
            this.client = client;
            this.counter = counter;
            ctr = 0;
        }
        public void InstantiateIn(Control container)
        {
            switch (type)
            {
                case ListItemType.Header:
                    lc = new LiteralControl("<table border=1>");
                    break;
                case ListItemType.Item:
                    if (counter == 1)
                    {
                        lc = new LiteralControl("<tr><td colspan=2 align=center><b>FORM XIV</b></td></tr><tr><td colspan=2 align=center><b>EMPLOYMENT CARD</b></td></tr><tr><td colspan=2 align=center>[Rule 76]</td></tr><tr><td align=left>Name and address of contractor</td><td><b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td></tr><tr><td align=left>Name and address of establishment in/under which contract is carried on </td><td><b>" + year + "</b></td></tr><tr><td align=left>Nature and location of work</td><td><b>" + ds.Tables[0].Rows[ctr]["grade_code"] + "</b></td></tr><tr><td align=left>Name and address of Principal Employer</td><td> <b>" + client + "</b></td></tr></tr><tr><td>1. Name of the workman</td><td><b>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</b></td></tr><tr><td>2. Sl. No. In the register of workmen employed </td><td><b>" + ds.Tables[0].Rows[ctr]["id_as_per_dob"].ToString() + "</b></td></tr><tr><td>3. Nature of Employment/Designation </td><td><b>" + ds.Tables[0].Rows[ctr]["designation"] + "</b></td></tr><tr><td align = center>4. Wage rate (with particulars of unit, in case of piece - work) </td><td><b>" + ds.Tables[0].Rows[ctr]["wage"] + "</b></td></tr><tr><td>5. Wage period </td><td><b>Month</b></td></tr><tr><td>6. Tenure of employment </td><td><b>" + ds.Tables[0].Rows[ctr]["joining_date"].ToString() + " to " + ds.Tables[0].Rows[ctr]["left_date1"].ToString() + "</b></td></tr><tr><td>7. Remarks </td><td><b>" + ds.Tables[0].Rows[ctr]["leftreason"] + "</b></td></tr><tr><td></td><td align=right>Signature of Contractor</td></tr><tr><td colspan=2> </td></tr><tr rowspan=2><td colspan=2> </td></tr><tr rowspan=2><td colspan=2> </td></tr>");
                    }
                    else
                    {
                        lc = new LiteralControl("<tr><td colspan=6 align=center><b>FORM XV</b></td></tr><tr><td colspan=6 align=center><b>SERVICE CERTIFICATE</b></td></tr><tr><td colspan=6 align=center>[Rule 77]</td></tr><tr><td colspan=6> </td></tr><tr><td colspan=3 align=left>Name and address of contractor <b>" + ds.Tables[0].Rows[ctr]["company"] + "</b></td><td colspan=3 align=left>Name and address of establishment in/under <b>which contract is carried on... " + year + "</b></td></tr><tr><td colspan=6> </td></tr><tr><td colspan=3 align=left>Nature and location of work... <b>" + ds.Tables[0].Rows[ctr]["grade_code"] + "</b></td><td colspan=3 align=left>Name and address of Principal Employer... <b>" + client + "</b></td></tr><tr><td colspan=6> </td></tr><tr><td colspan=6>Name and address of the workman... <b>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString() + "  " + ds.Tables[0].Rows[ctr]["address"].ToString() + "</b></td></tr><tr><td colspan=6>Age or Date of Birth... <b>" + ds.Tables[0].Rows[ctr]["BIRTH_DATE"] + "</b></td></tr><tr><td colspan=6> </td></tr><tr><td colspan=6>Identification Marks... <b>" + (ds.Tables[0].Rows[ctr]["EMP_IDENTITYMARK"] == "" ? "NA" : ds.Tables[0].Rows[ctr]["EMP_IDENTITYMARK"]) + "</b></td></tr><tr><td colspan=6></td></tr><tr><td colspan=6>Father's/Husband's name... <b>" + ds.Tables[0].Rows[ctr]["emp_father_name"] + "</b></td></tr><tr><td><b>Sl. No.</b></td><td colspan=2><b>Total period for which employed</b></td><td><b>Nature of work done</b></td><td><b>Rate of wages (with particulars of unit in case of piece-work)</b></td><td><b>Remark</b></td></tr><tr><td> </td><td align = center>From</td><td align = center>To</td><td> </td><td> </td><td> </td></tr><tr><td> " + ds.Tables[0].Rows[ctr]["id_as_per_dob"].ToString() + "</td><td align = center>" + ds.Tables[0].Rows[ctr]["joining_date"] + "</td><td align = center>" + ds.Tables[0].Rows[ctr]["left_date1"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["designation"] + "</td><td align = center>" + ds.Tables[0].Rows[ctr]["wage"] + "</td><td>" + ds.Tables[0].Rows[ctr]["leftreason"] + "</td></tr><tr><td colspan=6></td></tr><tr><td colspan=6></td></tr><tr><td colspan=6 align=right>Signature of Contractor</td></tr><tr><td colspan=6> </td></tr><tr rowspan=2><td colspan=6> </td></tr><tr rowspan=2><td colspan=6> </td></tr>");
                    }
                    ctr++;
                    break;
                case ListItemType.Footer:
                    lc = new LiteralControl("</table>");
                    ctr = 0;
                    break;
            }
            container.Controls.Add(lc);
        }
    }

    protected void btn_form_16_Click1(object sender, EventArgs e)
    {
        generate_letter(2);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btn_form_15_Click(object sender, EventArgs e)
    {
        generate_recu_report(2);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btn_form_23_Click(object sender, EventArgs e)
    {
        generate_letter(3);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btn_form_22_Click(object sender, EventArgs e)
    {
        generate_letter(4);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btn_form_21_Click(object sender, EventArgs e)
    {
        generate_letter(5);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btn_form_20_Click(object sender, EventArgs e)
    {
        generate_letter(6);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void btn_formd_Click(object sender, EventArgs e)
    {
        generate_letter(7);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void ddl_state_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlunitselect.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
      //comment 30/09  MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name='" + ddl_state.SelectedValue + "' AND branch_status = 0 ORDER BY UNIT_CODE", d.con);
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name='" + ddl_state.SelectedValue + "' AND (branch_close_date is null || branch_close_date = '' ||branch_close_date  >= STR_TO_DATE('01/" + txttodate.Text + "', '%d/%m/%Y')) ORDER BY UNIT_CODE", d.con);
		d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddlunitselect.DataSource = dt_item;
                ddlunitselect.DataTextField = dt_item.Columns[0].ToString();
                ddlunitselect.DataValueField = dt_item.Columns[1].ToString();
                ddlunitselect.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
            ddlunitselect.Items.Insert(0, "ALL");
            show_controls();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    protected void ddl_act_SelectedIndexChanged(object sender, EventArgs e)
    {
        btn_form_13.Visible = false;
        btn_form_14.Visible = false;
        Button1.Visible = false;
        btn_form_15.Visible = false;
        btn_form_23.Visible = false;
        btn_form_22.Visible = false;
        btn_form_21.Visible = false;
        btn_form_20.Visible = false;
        btn_formno_A.Visible = false;
        btn_formno_B.Visible = false;
        btn_formd.Visible = false;
        btn_form_13_above_18.Visible = false;
        document_upload.Visible = false;
        btn_bonus_register.Visible = false;
        btn_leave_record.Visible = false;
        btn_employee_data1.Visible = false;
        ddl_clra_act.Items.Clear();

        if (ddl_act.SelectedValue == "CHILD LABOUR ACT")
        {
            btn_form_13_above_18.Visible = true;
            btn_form_13.Visible = true;
        }
        else if (ddl_act.SelectedValue == "CLRA ACT")
        {
            document_upload.Visible = true;
            btn_formno_B.Visible = true;
            btn_formno_A.Visible = true;
            btn_form_13.Visible = true;
            btn_form_13_above_18.Visible = true;
            btn_form_14.Visible = true;
            btn_formd.Visible = true;
            btn_form_15.Visible = true;
            Button1.Visible = true;
            btn_form_23.Visible = true;
            btn_form_22.Visible = true;
            btn_form_21.Visible = true;
            btn_form_20.Visible = true;
            btn_bonus_register.Visible = true;
            btn_leave_record.Visible = true;
            btn_employee_data1.Visible = true;
            ddl_clra_act.Items.Insert(0, "AGREEMENT COPY");
            ddl_clra_act.Items.Insert(1, "RC COPY");
            ddl_clra_act.Items.Insert(2, "FORM V ISSUED BY PE");
            ddl_clra_act.Items.Insert(3, "LICENSE COPY");
            ddl_clra_act.Items.Insert(4, "ACK OF PE");
            ddl_clra_act.Items.Insert(5, "BANK TRANSFER LETTER");
            load_grdview();
        }
        else if (ddl_act.SelectedValue == "S&E ACT")
        {
            btn_formno_A.Visible = true;
            btn_form_23.Visible = true;
        }
        else if (ddl_act.SelectedValue == "MB ACT") { }
        else if (ddl_act.SelectedValue == "MW ACT")
        {
            btn_formno_B.Visible = true;
            btn_form_23.Visible = true;
            btn_form_22.Visible = true;
            btn_form_21.Visible = true;
            btn_form_20.Visible = true;
            btn_formd.Visible = true;
        }
        else if (ddl_act.SelectedValue == "PAYMENT WAGES ACT")
        {
            btn_formno_B.Visible = true;
            document_upload.Visible = true;
            ddl_clra_act.Items.Insert(0, "BANK TRANSFER PROOF");
            load_grdview();
        }
        else if (ddl_act.SelectedValue == "N & FH ACT")
        {
            btn_formno_A.Visible = true;

        }
        else if (ddl_act.SelectedValue == "EPF ACT")
        {
            //FORM2
            document_upload.Visible = true;
            ddl_clra_act.Items.Insert(0, "COMPANY PF CODE");
            ddl_clra_act.Items.Insert(1, "ECR COPY");
            ddl_clra_act.Items.Insert(2, "INSPECTION REPORT");
            load_grdview();
        }
        else if (ddl_act.SelectedValue == "ESI ACT")
        {
            document_upload.Visible = true;
            ddl_clra_act.Items.Insert(0, "ESI REG. CODE LETTER");
            ddl_clra_act.Items.Insert(1, "ECR COPY");
            ddl_clra_act.Items.Insert(2, "TIC FORM OF EMPLOYEES");
            load_grdview();
        }
        else if (ddl_act.SelectedValue == "EC ACT")
        {
            btn_formno_B.Visible = true; // Employees having salary greater than 21000
            document_upload.Visible = true;
            ddl_clra_act.Items.Insert(0, "RC-RETURN OF CONTRIBUTION");
            load_grdview();
        }
        else if (ddl_act.SelectedValue == "LWF ACT")
        {
            document_upload.Visible = true;
            ddl_clra_act.Items.Insert(0, "LWF REMITTANCES CHALLAN");
            ddl_clra_act.Items.Insert(1, "UNPAID ACCUMULATION REGISTER");
            ddl_clra_act.Items.Insert(2, "LWF REGISTER COPY");
            load_grdview();
        }
        else if (ddl_act.SelectedValue == "MH SG ACT")
        {
            //Only for Maharashtra State and for only Security Guard.
            document_upload.Visible = true;
            ddl_clra_act.Items.Insert(0, "RC OR EXEMPTION FROM GUARD BOARD");
            ddl_clra_act.Items.Insert(1, "REGISTER PRINCIPAL EMPLOYER");
            btn_formno_A.Visible = true;
            btn_form_14.Visible = true;
            load_grdview();
        }
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

    protected void btn_form_13_above_18_Click(object sender, EventArgs e)
    {
        generate_letter(10);
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }
    protected void DownloadFile(object sender, EventArgs e)
    {
        string filePath = (sender as LinkButton).CommandArgument;
        Response.ContentType = ContentType;
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
        Response.WriteFile(filePath);
        Response.End();
    }
    protected void grd_company_files_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
        e.Row.Cells[1].Visible = false;
    }
    protected void grd_company_files_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int item = (int)grd_company_files.DataKeys[e.RowIndex].Value;
        string temp = d.getsinglestring("SELECT file_name FROM pay_compliance_master WHERE id=" + item);
        if (temp != "")
        {
            File.Delete(Server.MapPath("~/compliance/") + temp);
        }
        d.operation("delete from pay_compliance_master WHERE id=" + item);
        load_grdview();

    }
    private void load_grdview()
    {
        MySqlDataAdapter MySqlDataAdapter1 = new MySqlDataAdapter("SELECT id,client_code, description,concat('~/compliance/',file_name) as Value FROM pay_compliance_master where comp_Code = '" + Session["COMP_CODE"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and act_name = '" + ddl_clra_act.SelectedItem.Text + "'", d.con1);
        DataSet DS1 = new DataSet();
        MySqlDataAdapter1.Fill(DS1);
        grd_company_files.DataSource = DS1;
        grd_company_files.DataBind();
    }
    protected void grd_company_files_files_PreRender(object sender, EventArgs e)
    {
        try
        {
            grd_company_files.UseAccessibleHeader = false;
            grd_company_files.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }
    protected string get_start_date()
    {
        return d.getsinglestring("SELECT IFNULL((SELECT start_date_common FROM pay_billing_master_history INNER JOIN pay_unit_master ON pay_billing_master_history.billing_unit_code = pay_unit_master.unit_code AND pay_billing_master_history.comp_code = pay_unit_master.comp_code WHERE pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' AND pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND month = '" + txttodate.Text.Substring(0, 2) + "' AND year = '" + txttodate.Text.Substring(3) + "' limit 1),(SELECT start_date_common  FROM pay_billing_master  INNER JOIN  pay_unit_master  ON  pay_billing_master.billing_unit_code  =  pay_unit_master.unit_code  AND  pay_billing_master.comp_code  =  pay_unit_master.comp_code  WHERE pay_unit_master.client_code  = '" + ddl_client.SelectedValue + "' AND  pay_unit_master.comp_code  = '" + Session["COMP_CODE"].ToString() + "' limit 1))");
    }
    protected void btn_generate_id_card_Click(object sender, EventArgs e)
    {
        if (d.getsinglestring("select id_card_dispatch from pay_employee_master where id_card_dispatch_date = str_to_date('"+txt_print_list.Text+"','%d/%m/%Y')").Equals(txt_lot.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Lot Number already Used !!')", true);
            txt_lot.Focus();
            return;
        }
        string zip_file = "c:/id_card/id_cards.zip";
        string folder_name = "c:/id_card/id_sub_idcard";
        if (File.Exists(zip_file))
        {
            File.Delete(zip_file);
        }
        if (!Directory.Exists("c:/id_card"))
        {
            Directory.CreateDirectory("c:/id_card");
            Directory.CreateDirectory(folder_name);
        }
        else
        {
            Directory.Delete("c:/id_card",recursive:true);
            Directory.CreateDirectory("c:/id_card");
            Directory.CreateDirectory(folder_name);
        }
        generate_excel(folder_name);
        //ZipFile.CreateFromDirectory(folder_name, zip_file);
        //Download_File(zip_file);
        //Directory.Delete("c:/id_card",recursive:true);
    }
    private void Download_File(string FilePath)
    {
        Response.ContentType = ContentType;
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
        Response.WriteFile(FilePath);
        Response.End();
    }
    protected void btn_gst_Click(object sender, EventArgs e)
    {
        generate_letter(12);
    }
    protected void btn_pt_Click(object sender, EventArgs e)
    {
        generate_letter(13);
    }
    protected void btn_sales_register_Click(object sender, EventArgs e)
    {
        generate_letter(14);
    }

    private void generate_excel(string folder_name)
    {
        Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
        Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
        Worksheet ws = (Worksheet)xla.ActiveSheet;
        try
        {
            //add some text 
            ws.Cells[1, 1] = "EMPLOYEE NAME";
            ws.Cells[1, 2] = "ID";
            ws.Cells[1, 3] = "DEG";
            ws.Cells[1, 4] = "DOJ";
            ws.Cells[1, 5] = "CLIENT NAME";
            ws.Cells[1, 6] = "EMP PHOTO";

            // MySqlCommand cmd = new MySqlCommand("SELECT pay_employee_master.emp_name, id_as_per_dob, pay_grade_master.grade_desc, if(emp_blood_group='Select','',emp_blood_group) as emp_blood_group, DATE_FORMAT(joining_date, '%d/%m/%Y') AS 'DOJ', client_name, emp_photo FROM pay_employee_master INNER JOIN pay_grade_master ON pay_grade_master.grade_code = pay_employee_master.grade_code AND pay_grade_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_client_master ON pay_client_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_unit_master.unit_code = pay_employee_master.unit_code AND pay_unit_master.client_code = pay_client_master.client_code AND pay_unit_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_images_master ON pay_employee_master.emp_code = pay_images_master.emp_code AND pay_employee_master.comp_code = pay_images_master.comp_code WHERE pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_client_master.client_code = '" + ddl_client.SelectedValue + "' and pay_employee_master.employee_type='Permanent' and left_date is null", d.con1);
            MySqlCommand cmd = new MySqlCommand("SELECT pay_employee_master.emp_name, id_as_per_dob, pay_grade_master.grade_desc, upper(DATE_FORMAT(joining_date, '%d-%b-%Y')) AS 'DOJ', upper(concat(pay_client_master.client_code,'-',pay_unit_master.unit_city)), original_photo FROM pay_employee_master INNER JOIN pay_grade_master ON pay_grade_master.grade_code = pay_employee_master.grade_code AND pay_grade_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_client_master ON pay_client_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_unit_master.unit_code = pay_employee_master.unit_code AND pay_unit_master.client_code = pay_client_master.client_code AND pay_unit_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_images_master ON pay_employee_master.emp_code = pay_images_master.emp_code AND pay_employee_master.comp_code = pay_images_master.comp_code WHERE pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_client_master.client_code = '" + ddl_client.SelectedValue + "' and pay_employee_master.employee_type='Permanent' and left_date is null and pay_employee_master.legal_flag=2 and pay_employee_master.id_card_dispatch_date is null", d.con1);

            d.con1.Open();
            try
            {
                MySqlDataReader dr = cmd.ExecuteReader();
                int i = 2;
                while (dr.Read())
                {
                    if (!dr.GetValue(5).ToString().Equals(""))
                    {
                        string path = Server.MapPath("~/EMP_Images");
                        try
                        {
                            File.Copy(path + "\\" + dr.GetValue(5).ToString(), folder_name + "/" + (i-1) + ".jpg");
                            ws.Cells[i, 1] = dr.GetValue(0).ToString();
                            ws.Cells[i, 2] = "ID : " + dr.GetValue(1).ToString();
                            ws.Cells[i, 3] = dr.GetValue(2).ToString();
                            ws.Cells[i, 4] = "DOJ : " + dr.GetValue(3).ToString();
                            ws.Cells[i, 5] = dr.GetValue(4).ToString();
                            ws.Cells[i, 6] = (i-1);
                            ++i;
                            d.operation("update pay_employee_master set id_card_dispatch_date=now(), id_card_dispatch=" + txt_lot.Text + " WHERE id_as_per_dob = '" + dr.GetValue(1).ToString() + "'");
                        }
                        catch { }
                        //Microsoft.Office.Interop.Excel.Range oRange = (Microsoft.Office.Interop.Excel.Range)ws.Cells[i, 7];
                        //float Left = (float)((double)oRange.Left);
                        //float Top = (float)((double)oRange.Top);
                        //const float ImageSize = 100;
                        //string path = Server.MapPath("~/EMP_Images");
                        //try
                        //{
                        //    ws.Shapes.AddPicture(path + "\\" + dr.GetValue(6).ToString(), Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                        //}
                        //catch { }
                    }
                }
                d.con1.Close();
            }
            catch (Exception ex) { throw ex; }
            finally { d.con1.Close(); }

            //xla.Visible = true;

            wb.SaveAs(folder_name.Replace("/","\\") + "\\Sheet1", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            wb.Close();
            xla.Quit();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('COMPLETED, Please check folder " + folder_name + " !!')", true);
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            ReleaseComObject(ws);
            ReleaseComObject(wb);
            ReleaseComObject(xla);
        }
    }
    private void ReleaseComObject(object obj)
    {
        try
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
            obj = null;
        }
        catch (Exception ex)
        {
            obj = null;
        }
    }

    protected void btn_dispatch_print_Click(object sender, EventArgs e)
    {
        generate_letter(15);
    }
    protected void btn_clientwiseAllEmployeeID(object sender,EventArgs ee) {
        string comp_logo = null;
        string comp_stamppath = null;
       // string rightfooterpath = null;
       // string stamppath = null;
        d.con1.Open();
        try
        {
            string unit = ddl_state.SelectedValue.ToString();
            string brancn = ddlunitselect.SelectedValue.ToString();
            string grae = ddl_designation.SelectedValue.ToString();
            string downloadname = "I_Card";
            string query = null;
            crystalReport.Load(Server.MapPath("~/I_Card_client.rpt"));
            string filepath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "EMP_Images\\");
            filepath = filepath.Replace("\\", "\\\\");
            if (Session["COMP_CODE"].ToString() == "C02")
            {
                comp_logo = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C02_1.png");
                comp_stamppath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C02_stamp.png");
            }
            else
            {
                comp_logo = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C01_1.png");
                 comp_stamppath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C01_stamp.jpg");
            }
            // headerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\");
            comp_logo = comp_logo.Replace("\\", "\\\\");
            comp_stamppath = comp_stamppath.Replace("\\", "\\\\");
            query = "SELECT pay_employee_master.EMP_CODE as 'celtcode', pay_employee_master.EMP_NAME, EMP_FATHER_NAME, BIRTH_DATE, GRADE_DESC, EMP_MOBILE_NO, LOCATION AS 'ADDRESS2', EMP_CURRENT_ADDRESS AS 'ADDRESS1', EMP_CURRENT_CITY AS 'EMP_CITY', concat('DOJ : ',upper(date_format(joining_date,'%d-%M-%Y'))) as 'joining_date',concat('ID : ',id_as_per_dob) as 'EMP_CODE',upper(concat(pay_client_master.client_code,' - ',pay_unit_master.unit_city)) as 'city', EMP_CURRENT_STATE AS 'STATE', concat('" + filepath + "', pay_images_master.original_photo) AS 'PF_SHEET', pay_images_master.emp_signature, department_type AS 'DEPT_NAME', (SELECT CASE WHEN EMP_BLOOD_GROUP IS NOT NULL THEN EMP_BLOOD_GROUP ELSE '' END) AS 'GENDER', (SELECT client_name FROM pay_client_master WHERE client_code = pay_employee_master.client_code) AS 'BANK_CODE','" + comp_logo + "' as COMPANY_NAME,'" + comp_stamppath + "' as COMPANY_PAN_NO FROM pay_employee_master INNER JOIN pay_grade_master ON pay_grade_master.grade_code = pay_employee_master.grade_code AND pay_grade_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_client_master ON pay_client_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_unit_master.unit_code = pay_employee_master.unit_code AND pay_unit_master.client_code = pay_client_master.client_code AND pay_unit_master.comp_code = pay_employee_master.comp_code INNER JOIN pay_images_master ON pay_employee_master.emp_code = pay_images_master.emp_code AND pay_employee_master.comp_code = pay_images_master.comp_code WHERE pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_client_master.client_code = '" + ddl_client.SelectedValue + "' and pay_employee_master.employee_type='Permanent' and left_date is null and pay_employee_master.legal_flag=2 and pay_employee_master.emp_code not in (select emp_code from pay_document_details where dispatch_flag=1)  and pay_employee_master.id_card_dispatch='0'";
            //string query123 = " SELECT `pay_employee_master`.`EMP_CODE` as 'celtcode', `pay_employee_master`.`EMP_NAME`, `EMP_FATHER_NAME`, `BIRTH_DATE`, `GRADE_DESC`, `EMP_MOBILE_NO`, `LOCATION` AS 'ADDRESS2', `EMP_CURRENT_ADDRESS` AS 'ADDRESS1', `EMP_CURRENT_CITY` AS 'EMP_CITY', date_format(joining_date,'%d-%M-%Y') as 'joining_date',ihmscode as 'EMP_CODE',concat(pay_client_master.client_code,' - ',pay_unit_master.unit_city) as 'city', `EMP_CURRENT_STATE` AS 'STATE', concat('" + filepath + "', `pay_images_master`.`original_photo`) AS 'PF_SHEET', `pay_images_master`.`emp_signature`, `department_type` AS 'DEPT_NAME', (SELECT CASE WHEN `EMP_BLOOD_GROUP` IS NOT NULL THEN `EMP_BLOOD_GROUP` ELSE '' END) AS 'GENDER', (SELECT `client_name` FROM `pay_client_master` WHERE `client_code` = `pay_employee_master`.`client_code`) AS 'BANK_CODE' FROM `pay_employee_master` INNER JOIN `pay_grade_master` ON `pay_grade_master`.`grade_code` = `pay_employee_master`.`grade_code` AND `pay_grade_master`.`comp_code` = `pay_employee_master`.`comp_code` INNER JOIN `pay_client_master` ON `pay_client_master`.`comp_code` = `pay_employee_master`.`comp_code` INNER JOIN `pay_unit_master` ON `pay_unit_master`.`unit_code` = `pay_employee_master`.`unit_code` AND `pay_unit_master`.`client_code` = `pay_client_master`.`client_code` AND `pay_unit_master`.`comp_code` = `pay_employee_master`.`comp_code` INNER JOIN `pay_images_master` ON `pay_employee_master`.`emp_code` = `pay_images_master`.`emp_code` AND `pay_employee_master`.`comp_code` = `pay_images_master`.`comp_code` WHERE pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_client_master.client_code = '" + ddl_client.SelectedValue + "' and pay_employee_master.employee_type='Permanent' and left_date is null and pay_employee_master.legal_flag=2 and pay_employee_master.emp_code not in (select emp_code from pay_document_details where dispatch_flag=1 ) and pay_unit_master.state_name='" + ddl_state.SelectedValue.ToString() + "' and pay_unit_master.unit_code='" + ddlunitselect.SelectedValue.ToString() + "' and pay_employee_master.grade_code='" + ddl_designation.SelectedValue.ToString() + "'";
            ReportLoad(query, downloadname);
            Response.End();
            d.con1.Close();
        }
        catch (Exception ee2)
        {
            throw ee2;
        }
        finally
        {
            d.con1.Close();
            d.operation("update pay_employee_master set id_card_dispatch_date=now(), id_card_dispatch='1' WHERE pay_employee_master.comp_code = '" + Session["COMP_CODE"].ToString() + "'   AND pay_employee_master.client_code = '" + ddl_client.SelectedValue + "' and pay_employee_master.employee_type='Permanent'  and left_date is null and pay_employee_master.legal_flag=2");
        }
    }
    //ddl_client_form5
    //ddl_state_form5

    protected void btn_generateform5_Click(object sender, EventArgs e)
    {
            try
            {

                //SELECT GROUP_CONCAT(unit_code),GROUP_CONCAT(unit_name) as 'location',labour_office,SUM(`Emp_count`) AS 'a', GROUP_CONCAT( Emp_count),`client_name` AS 'client_code',`state_name`,CASE WHEN SUM(`Emp_count`) >= 20 THEN 'Generated Form 5  ' ELSE 'No Need' END AS 'Status' FROM  `pay_unit_master` INNER JOIN `pay_client_master` ON `pay_unit_master`.`comp_code` = `pay_client_master`.`comp_code` AND `pay_unit_master`.`client_code` = `pay_client_master`.`client_code` WHERE `pay_unit_master`.`comp_code` = '" + Session["COMP_CODE"].ToString() + "' AND `labour_office` IS NOT NULL and pay_unit_master.client_code='" + ddl_client.SelectedValue + "' and state_name='" + ddl_state.SelectedValue + "' GROUP BY `labour_office` HAVING  `a` >= 20
                //string sql = "SELECT `client_name`  AS 'client_code',`state_name`,`labour_office`,`Emp_count` AS 'count', CASE WHEN `Emp_count` >= 20 THEN 'Need ' ELSE 'No Need' END AS 'Status' FROM   `pay_unit_master` INNER JOIN `pay_client_master` ON `pay_unit_master`.`comp_code` = `pay_client_master`.`comp_code` AND `pay_unit_master`.`client_code` = `pay_client_master`.`client_code` WHERE  pay_unit_master.`state_name` = '" + ddl_state.SelectedValue + "'  AND pay_unit_master.`comp_code` = '" + Session["COMP_CODE"].ToString() + "' AND pay_unit_master.`client_code` = '" + ddl_client.SelectedValue + "' AND `Emp_count` >= 20";
                string sql = "SELECT GROUP_CONCAT(unit_code),GROUP_CONCAT(unit_name) as 'location',labour_office,SUM(`Emp_count`) AS 'a', GROUP_CONCAT( Emp_count) as 'Emp_count',`client_name` AS 'client_code',`state_name`,CASE WHEN SUM(`Emp_count`) >= 20 THEN 'Generated Form 5  ' ELSE 'No Need' END AS 'Status' FROM  `pay_unit_master` INNER JOIN `pay_client_master` ON `pay_unit_master`.`comp_code` = `pay_client_master`.`comp_code` AND `pay_unit_master`.`client_code` = `pay_client_master`.`client_code` WHERE `pay_unit_master`.`comp_code` = '" + Session["COMP_CODE"].ToString() + "' AND `labour_office` IS NOT NULL and pay_unit_master.client_code='" + ddl_client.SelectedValue + "' and state_name='" + ddl_state.SelectedValue + "' GROUP BY `labour_office` HAVING  `a` >= 20";
                MySqlDataAdapter dscmd = new MySqlDataAdapter(sql, d.con);
                DataSet ds = new DataSet();
                dscmd.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment;filename=Form5" + ddl_state.SelectedItem.Text.Replace(" ", "_") + ".xls");

                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";
                    Repeater Repeater1 = new Repeater();
                    Repeater1.DataSource = ds;
                    Repeater1.HeaderTemplate = new MyTemplate2(ListItemType.Header, ds);
                    Repeater1.ItemTemplate = new MyTemplate2(ListItemType.Item, ds);
                    Repeater1.FooterTemplate = new MyTemplate2(ListItemType.Footer, null);
                    Repeater1.DataBind();

                    System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                    System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                    Repeater1.RenderControl(htmlWrite);

                    string style = @"<style> .textmode { } </style>";
                    Response.Write(style);
                    Response.Output.Write(stringWrite.ToString());
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Matching Records Found.');", true);
                    rem_emp_count = ViewState["rem_emp_count"].ToString();
                }
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        
    }
    public class MyTemplate2 : ITemplate
    {
        ListItemType type1;
        LiteralControl lc1;
        DataSet ds1;

        public MyTemplate2(ListItemType type1, DataSet ds)
        {
            this.type = type1;
            this.ds1 = ds;
        }

        public void InstantiateIn(Control container)
        {
            switch (type)
            {
                case ListItemType.Header:
                    lc1 = new LiteralControl("<table border=1><tr><th>Sr.No</th><th> CLIENT NAME</th><th> STATE NAME</th><th> LOCATION NAME</th><th>EMP COUNT</th><th>LABOUR REGIONAL OFFICE</th> <th> TOTAL EMP COUNT</th><th>FORM 5 STATUS</th></tr> ");
                    break;
                case ListItemType.Item://
                    lc1 = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds1.Tables[0].Rows[ctr]["client_code"] + "</td><td>" + ds1.Tables[0].Rows[ctr]["STATE_NAME"] + "</td><td>" + ds1.Tables[0].Rows[ctr]["location"] + "</td><td>" + ds1.Tables[0].Rows[ctr]["Emp_count"] + "</td><td>" + ds1.Tables[0].Rows[ctr]["labour_office"] + "</td><td>" + ds1.Tables[0].Rows[ctr]["a"] + "</td><td>" + ds1.Tables[0].Rows[ctr]["Status"] + "</td></tr>");
                    ctr++;
                    break;
                case ListItemType.Footer:
                    lc1 = new LiteralControl("</table>");
                    ctr = 0;
                    break;
            }
            container.Controls.Add(lc1);
        }



        public ListItemType type { get; set; }

        public int ctr { get; set; }

        public LiteralControl lc { get; set; }
    }
    protected void employee_status()
    {

        try
        {
            gv_genrateform5.DataSource = null;
            gv_genrateform5.DataBind();

            System.Data.DataTable dt_item = new System.Data.DataTable();
            //MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT client_name AS 'client_code',unit_code,state_name,labour_office,Emp_count AS 'count',CASE WHEN Emp_count >= 20 THEN 'Need' ELSE 'No Need' END AS 'Status' FROM pay_unit_master INNER JOIN pay_client_master ON pay_unit_master.comp_code = pay_client_master.comp_code AND pay_unit_master.client_code = pay_client_master.client_code WHERE pay_unit_master.comp_code ='" + Session["comp_code"].ToString() + "' AND Emp_count >= 20", d.con);
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT GROUP_CONCAT(unit_code),GROUP_CONCAT(unit_name),labour_office, SUM(Emp_count) AS 'a',client_name AS 'client_code',state_name FROM pay_unit_master INNER JOIN pay_client_master ON pay_unit_master.comp_code = pay_client_master.comp_code AND pay_unit_master.client_code = pay_client_master.client_code WHERE pay_unit_master.comp_code = '" + Session["comp_code"].ToString() + "' and client_active_close='0' AND labour_office IS NOT NULL GROUP BY labour_office,pay_unit_master.client_code HAVING a >= 20", d.con);
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ViewState["rem_emp_count"] = dt_item.Rows.Count.ToString();
                rem_emp_count = ViewState["rem_emp_count"].ToString();
                gv_genrateform5.DataSource = dt_item;
                gv_genrateform5.DataBind();

            }
        }
        catch (Exception ex) { throw ex; }
        finally { }

    }
    protected void btn_bank_images_Click(object sender, EventArgs e)
    {
        string folder_name = "c:/bank_passbook";

        if (!Directory.Exists(folder_name))
        {
            Directory.CreateDirectory(folder_name);
        }
        else
        {
            Directory.Delete(folder_name, recursive: true);
            Directory.CreateDirectory(folder_name);
        }
        generate_passbook(folder_name);
    }

    private void generate_passbook(string folder_name)
    {
      
        try
        {
            string where = " pay_employee_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_employee_master.client_code = '" + ddl_client.SelectedValue + "'";
          
            if (ddl_state.SelectedValue != "ALL") { where = where + "AND pay_employee_master.client_wise_state = '" + ddl_state.SelectedValue + "'"; }
            if (ddlunitselect.SelectedValue != "ALL") { where = where + "AND pay_employee_master.unit_code = '" + ddlunitselect.SelectedValue + "'"; }

            MySqlCommand cmd = new MySqlCommand("SELECT  CONCAT((SELECT DISTINCT state_code FROM pay_state_master WHERE pay_state_master.state_name = pay_unit_master.state_name), '_', unit_name, '_', replace(pay_employee_master.emp_name,' ','_'),SUBSTRING(bank_passbook,LOCATE('.',bank_passbook))) AS 'file_name', bank_passbook FROM pay_employee_master INNER JOIN pay_images_master ON pay_employee_master.emp_code = pay_images_master.emp_code AND pay_employee_master.comp_code = pay_images_master.comp_code INNER JOIN pay_unit_master ON pay_unit_master.comp_code = pay_employee_master.comp_code AND pay_unit_master.unit_code = pay_employee_master.unit_code  WHERE " + where + " and left_date is null and bank_passbook is not null  order by file_name", d.con1);

            d.con1.Open();
            try
            {
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.GetValue(1).ToString().Equals(""))
                    {
                        string path = Server.MapPath("~/EMP_Images");
                        try
                        {
                            //string ext = dr.GetValue(1).ToString().Substring(dr.GetValue(1).ToString().IndexOf("."), (dr.GetValue(1).ToString().Length-(dr.GetValue(1).ToString().IndexOf("."))));
                            
                            File.Copy(path + "\\" + dr.GetValue(1).ToString(), folder_name + "/" + dr.GetValue(0).ToString());
                           
                        }
                        catch { }
                    }
                }
                d.con1.Close();
            }
            catch (Exception ex) { throw ex; }
            finally { d.con1.Close(); }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('COMPLETED, Please check folder " + folder_name + " !!')", true);
        }
        catch (Exception ex) { throw ex; }
        finally
        {
        }
    }
 protected void btn_bonus_register_Click(object sender, EventArgs e)
    {
        generate_letter(16);
    }
  protected void btn_leave_record_Click(object sender, EventArgs e)
    {
        generate_letter(17);
    }
    protected void btn_employee_data_Click(object sender, EventArgs e)
    {
        generate_letter(18);
    }

}