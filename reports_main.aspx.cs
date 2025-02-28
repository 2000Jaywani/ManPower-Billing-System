using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Globalization;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Web;
using System.IO.Compression;
using OctaBillsApi;
using System.Net;
using Newtonsoft.Json;
using iTextSharp.text.pdf;
using System.Collections;
using iTextSharp.text.xml.xmp;
using org.bouncycastle.pkcs;
using org.bouncycastle.crypto;
using System.Collections.Generic;
using System.Web.Script.Serialization;

public partial class Employee_salary_details : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();
    BillingSalary bs = new BillingSalary();
    string KeyId = "";//"k637452784807367055";
    string KeySecret = "";// "Dhxs60WQBnnmn52xd9Kw";
    int e_invoice = 0;
    public int arrears_invoice = 0, ot_invoice = 0, counter = 0;
    CrystalDecisions.CrystalReports.Engine.ReportDocument crystalReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["COMP_CODE"] == null || Session["COMP_CODE"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }
        if (!IsPostBack)
        {
            ddl_client.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select client_name, client_code from pay_report_gst where comp_code='" + Session["comp_code"] + "' GROUP BY client_code ORDER BY client_code", d.con);
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
                d.con.Close();
                ddl_client.Items.Insert(0, "ALL");
                ddl_state.Items.Insert(0, "ALL");
                ddl_unitcode.Items.Insert(0, "ALL");
                ddl_employee.Items.Insert(0, "ALL");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
    }

    protected void ddl_client_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_client.SelectedValue != "ALL")
        {

            d1.con1.Open();
            ddl_state.Items.Clear();
            try
            {
                MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter("SELECT distinct state FROM pay_designation_count where CLIENT_CODE = '" + ddl_client.SelectedValue + "' and state in (select state_name from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE in(" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_client.SelectedValue + "')  ORDER BY STATE", d1.con1);
                DataSet DS = new DataSet();
                MySqlDataAdapter.Fill(DS);
                ddl_state.DataSource = DS;
                ddl_state.DataBind();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                ddl_state.Items.Insert(0, "ALL");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d1.con1.Close();
            }


            ddl_unitcode.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' AND state_name ='" + ddl_state.SelectedValue + "' and UNIT_CODE in(select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE='" + Session["LOGIN_ID"].ToString() + "' AND client_code='" + ddl_client.SelectedValue + "' AND state_name='" + ddl_state.SelectedValue + "') ORDER BY UNIT_CODE", d.con);
            d.con.Open();
            try
            {
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_unitcode.DataSource = dt_item;
                    ddl_unitcode.DataTextField = dt_item.Columns[0].ToString();
                    ddl_unitcode.DataValueField = dt_item.Columns[1].ToString();
                    ddl_unitcode.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_unitcode.Items.Insert(0, "ALL");
                ddl_unitcode.SelectedIndex = 0;
                ddl_state_SelectedIndexChanged(null, null);
                ddlunitselect_SelectedIndexChanged(null, null);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }

        }
    }
    protected void ddlunitselect_SelectedIndexChanged(object sender, EventArgs e)
    {

        ddl_employee.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item;
        string left = " employee_type = '" + ddl_employee_type.SelectedValue + "' and  (left_date = '' or left_date is null)";
        if (ddl_employee_type.SelectedValue == "Left")
        {
            left = " left_date is not null";
        }
        string where = " where  comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and unit_code='" + ddl_unitcode.SelectedValue + "' and " + left + " ORDER BY emp_name";
        if (ddl_unitcode.SelectedValue == "ALL")
        {
            where = " where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and EMP_CURRENT_STATE='" + ddl_state.SelectedValue + "' and " + left + " ORDER BY emp_name";
        }
        //vikas 08-01-19
        cmd_item = new MySqlDataAdapter("Select (SELECT CASE Employee_type WHEN 'Reliever' THEN CONCAT(emp_name, '-', 'Reliever') ELSE emp_name END) AS 'EMP_NAME',EMP_CODE from pay_employee_master " + where, d.con);
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_employee.DataSource = dt_item;
                ddl_employee.DataTextField = dt_item.Columns[0].ToString();
                ddl_employee.DataValueField = dt_item.Columns[1].ToString();
                ddl_employee.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
            ddl_employee.Items.Insert(0, "ALL");
            ddl_employee.SelectedIndex = 0;
            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }

    protected void btnclose_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }

    protected void ddl_state_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_client.SelectedValue != "ALL")
        {
            ddl_unitcode.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and pay_unit_master.state_name = '" + ddl_state.SelectedValue + "' and  pay_unit_master.UNIT_CODE  in ( select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE in(" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_client.SelectedValue + "' AND state_name='" + ddl_state.SelectedValue + "')   ORDER BY pay_unit_master.state_name", d.con);
            d.con.Open();
            try
            {
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_unitcode.DataSource = dt_item;
                    ddl_unitcode.DataTextField = dt_item.Columns[0].ToString();
                    ddl_unitcode.DataValueField = dt_item.Columns[1].ToString();
                    ddl_unitcode.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_unitcode.Items.Insert(0, "ALL");
                ddl_unitcode.SelectedIndex = 0;
                ddlunitselect_SelectedIndexChanged(null, null);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
    }

    private void send_file(int count)
    {
        string sql = "";
        d.con.Open();
        if (count == 1)
        {
            string where = "";
            if (ddl_emp_diff.SelectedValue.Equals("0"))
            {
                where = " where pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' and employee_type = 'Permanent' and left_date is null order by 1,2,4,6";
            }
            else if (ddl_emp_diff.SelectedValue.Equals("1"))
            {
                d.operation("create table pay_number_update (num_ber varchar(100));insert into pay_number_update SELECT " + ddl_emp_type.SelectedValue + " FROM pay_employee_master where employee_type = 'Permanent' and " + ddl_emp_type.SelectedValue + " is not null and " + ddl_emp_type.SelectedValue + " != '' GROUP BY " + ddl_emp_type.SelectedValue + " HAVING COUNT(*) > 1;");
                where = " inner join pay_number_update on num_ber =" + ddl_emp_type.SelectedValue + " where pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' and employee_type = 'Permanent' order by 7";
            }
            else if (ddl_emp_diff.SelectedValue.Equals("2"))
            {
                where = " where pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' and employee_type = 'Permanent' and (" + ddl_emp_type.SelectedValue + " is null or " + ddl_emp_type.SelectedValue + " = '') order by 1,2,4,6";
            }
            sql = "select client_name as 'CLIENT NAME',state_name as STATE, unit_name as 'BRANCH NAME',unit_city as 'BRANCH CITY',EMP_NAME,EMP_FATHER_NAME, " + ddl_emp_type.SelectedValue.ToUpper() + " as Number, date_format(left_date,'%d/%m/%Y') as 'LEFT DATE',date_format(joining_date,'%d/%m/%Y') as 'JOINING DATE' from pay_employee_master inner join pay_unit_master on pay_unit_master.comp_code = pay_employee_master.comp_code and pay_unit_master.unit_code = pay_employee_master.unit_code inner join pay_client_master on pay_client_master.comp_code = pay_employee_master.comp_code and pay_unit_master.client_code = pay_client_master.client_code " + where;
        }
        else if (count == 2)
        {
            string where = " where pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "'";
            if (!ddl_client.SelectedValue.Equals("ALL"))
            {
                where = where + " and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "'";
            }
            if (!ddl_state.SelectedValue.Equals("ALL"))
            {
                where = where + " and pay_unit_master.state_name = '" + ddl_state.SelectedValue + "'";
            }
            if (!ddl_unitcode.SelectedValue.Equals("ALL"))
            {
                where = where + " and pay_unit_master.unit_code = '" + ddl_unitcode.SelectedValue + "'";
            }
            if (!ddl_employee_type.SelectedValue.Equals("ALL"))
            {
                where = where + " and pay_employee_master.employee_type = '" + ddl_employee_type.SelectedValue + "'";
            }
            if (!ddl_employee.SelectedValue.Equals("ALL"))
            {
                where = where + " and pay_employee_master.emp_code = '" + ddl_employee.SelectedValue + "'";
            }
            sql = "SELECT client_name AS 'CLIENT NAME', pay_billing_unit_Rate_history. state_name AS 'STATE', pay_billing_unit_Rate_history.unit_name AS 'BRANCH', pay_employee_master.emp_name AS 'EMPLOYEE NAME', pay_employee_master.employee_type AS 'EMPLOYEE TYPE', pay_employee_master.p_tax_number as 'AADHAR NUMBER', pay_employee_master.pan_number as 'UAN NUMBER', pay_employee_master.pf_number as 'PF NUMBER', pay_employee_master.esic_number as 'ESIC NUMBER', pay_billing_unit_Rate_history.tot_days_present as 'DAYS PRESENT', round(pay_billing_unit_Rate_history.Amount,2) as 'BILLING AMOUNT', round(pay_pro_master.payment,2) as 'PAYMENT AMOUNT', MONTHNAME(STR_TO_DATE(pay_billing_unit_Rate_history.month, '%m')) as 'MONTH', pay_billing_unit_Rate_history.year as 'YEAR' FROM pay_employee_master INNER JOIN pay_unit_master ON pay_employee_master.unit_code = pay_unit_master.unit_code AND pay_employee_master.comp_code = pay_unit_master.comp_code INNER JOIN pay_client_master ON pay_client_master.client_code = pay_unit_master.client_code AND pay_client_master.comp_code = pay_unit_master.comp_code inner join pay_billing_unit_Rate_history on pay_employee_master.emp_code = pay_billing_unit_Rate_history.emp_code inner join pay_pro_master on pay_employee_master.emp_code = pay_pro_master.emp_code AND pay_pro_master.month = pay_billing_unit_Rate_history.month and pay_pro_master.year = pay_billing_unit_Rate_history.year " + where + " AND pay_billing_unit_Rate_history.month = " + txt_month_year.Text.Substring(0, 2) + " and pay_billing_unit_Rate_history.year = " + txt_month_year.Text.Substring(3) + " GROUP BY client_name, pay_billing_unit_Rate_history.state_name, pay_billing_unit_Rate_history.unit_name, pay_employee_master.emp_name ORDER BY 1, 2, 3,4";
        }
        MySqlDataAdapter dscmd = new MySqlDataAdapter(sql, d.con);
        DataSet ds = new DataSet();
        dscmd.Fill(ds);
        d.operation("DROP TABLE IF EXISTS pay_number_update;");
        if (ds.Tables[0].Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            if (count == 1)
            {
                Response.AddHeader("content-disposition", "attachment;filename=Employees_Documents.xls");
            }
            else if (count == 2)
            {
                Response.AddHeader("content-disposition", "attachment;filename=Employees_Details.xls");
            }
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Repeater Repeater1 = new Repeater();
            Repeater1.DataSource = ds;
            Repeater1.HeaderTemplate = new MyTemplate(ListItemType.Header, ds, count, ddl_emp_type.SelectedItem.ToString());
            Repeater1.ItemTemplate = new MyTemplate(ListItemType.Item, ds, count, ddl_emp_type.SelectedItem.ToString());
            Repeater1.FooterTemplate = new MyTemplate(ListItemType.Footer, null, count, ddl_emp_type.SelectedItem.ToString());
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
    }
    public class MyTemplate : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;
        int i;
        string emp_type;
        public MyTemplate(ListItemType type, DataSet ds, int i, string emp_type)
        {
            this.type = type;
            this.ds = ds;
            this.i = i;
            this.emp_type = emp_type;
            ctr = 0;
        }
        public void InstantiateIn(Control container)
        {
            switch (type)
            { //Original Bank A/C Number ,PF_IFSC_CODE,BANK_HOLDER_NAME
                case ListItemType.Header:
                    if (i == 1)
                    {
                        lc = new LiteralControl("<table border=1><tr><th>SR. NO.</th><th>Client Name</th><th>State Name</th><th>Branch Name</th><th>Branch City</th><th>EMPLOYEE NAME</th><th>EMPLOYEE FATHER NAME</th><th>" + emp_type + " NUMBER</th><th>LEFT DATE</th><th>JOINING DATE</th></tr>");
                    }
                    else if (i == 2)
                    {
                        lc = new LiteralControl("<TABLE BORDER=1><TR><TH>SR. NO.</TH><TH>CLIENT NAME</TH><TH>STATE NAME</TH><TH>BRANCH NAME</TH><TH>EMPLOYEE NAME</TH><TH>EMPLOYEE TYPE</TH><TH>AADHAR NUMBER</TH><TH>UAN NUMBER</TH><TH>PF NUMBER</TH><TH>ESIC NUMBER</TH><TH>DAYS PRESENT</TH><TH>BILLING AMOUNT</TH><TH>PAYMENT AMOUNT</TH><th>MONTH</th><th>YEAR</th></TR>");
                    }
                    break;
                case ListItemType.Item:
                    if (i == 1)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["CLIENT NAME"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["STATE"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["BRANCH NAME"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["BRANCH CITY"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["EMP_NAME"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["EMP_FATHER_NAME"].ToString().ToUpper() + "</td><td>'" + ds.Tables[0].Rows[ctr]["Number"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["LEFT DATE"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["JOINING DATE"].ToString().ToUpper() + "</td></tr>");
                    }
                    else if (i == 2)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["CLIENT NAME"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["STATE"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["BRANCH"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["EMPLOYEE NAME"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["EMPLOYEE TYPE"].ToString().ToUpper() + "</td><td>'" + ds.Tables[0].Rows[ctr]["AADHAR NUMBER"].ToString().ToUpper() + "</td><td>'" + ds.Tables[0].Rows[ctr]["UAN NUMBER"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["PF NUMBER"].ToString().ToUpper() + "</td><td>'" + ds.Tables[0].Rows[ctr]["ESIC NUMBER"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["DAYS PRESENT"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["BILLING AMOUNT"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["PAYMENT AMOUNT"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["MONTH"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["YEAR"].ToString().ToUpper() + "</td></tr>");
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
    protected void btn_emp_report_Click(object sender, EventArgs e)
    {
        send_file(1);
    }
    protected void btn_employee_report_Click(object sender, EventArgs e)
    {
        send_file(2);
    }
    protected void btn_getxl_report_Click(object sender, EventArgs e)
    {
        hidtab.Value = "4";
        if (ddl_report.SelectedValue == "PF XL")
        {
            export_xl(1);
        }
        if (ddl_report.SelectedValue == "LWF XL")
        {
            export_xl(2);
        }
        if (ddl_report.SelectedValue == "PT XL")
        {
            export_xl(3);
        }
        if (ddl_report.SelectedValue == "ESIC XL")
        {
            export_xl(4);
        }
        if (ddl_report.SelectedValue == "GST XL")
        {
            export_xl(5);
        }
        if (ddl_report.SelectedValue == "Branch Head Contact Details")
        {
            export_xl(6);
        }
        if (ddl_report.SelectedValue == "Salary Slip Sending Details")
        {
            export_xl(7);
        }
        if (ddl_report.SelectedValue == "Joining Letter Sending Details")
        {
            export_xl(8);
        }
        if (ddl_report.SelectedValue == "Monthwise Billing Details")
        {
            export_xl(9);
        }



    }
    private void export_xl(int i)
    {
        string t = ddl_bill_type.SelectedValue;
        string sql = null;
        string where_head = "";
        string where_salary = "";
        string where_joining = "";
        string where_billing = "";
        string client = "";

        where_head = "where pay_unit_master.comp_code='" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + ddl_unitcode.SelectedValue + "'  and branch_status != 1 ";
        where_salary = "where pay_pro_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_pro_master.client_code = '" + ddl_client.SelectedValue + "' AND pay_pro_master.unit_code = '" + ddl_unitcode.SelectedValue + "'  ";
        where_joining = " WHERE pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_employee_master.client_code = '" + ddl_client.SelectedValue + "' and pay_employee_master.unit_code = '" + ddl_unitcode.SelectedValue + "' ";
        where_billing = " WHERE comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and unit_code = '" + ddl_unitcode.SelectedValue + "' ";
        if (ddl_state.SelectedValue == "ALL")
        {
            where_head = "where pay_unit_master.comp_code='" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code= '" + ddl_client.SelectedValue + "' and branch_status != 1  ";
            where_salary = " where pay_pro_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_pro_master.client_code = '" + ddl_client.SelectedValue + "' ";
            where_joining = " WHERE pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_employee_master.client_code = '" + ddl_client.SelectedValue + "' ";
            where_billing = " WHERE comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' ";
        }
        else if (ddl_unitcode.SelectedValue == "ALL")
        {
            where_head = "where pay_unit_master.comp_code='" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code= '" + ddl_client.SelectedValue + "' and state_name='" + ddl_state.SelectedValue + "' and branch_status != 1  ";
            where_salary = " where pay_pro_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_pro_master.client_code = '" + ddl_client.SelectedValue + "' AND pay_pro_master.state_name = '" + ddl_state.SelectedValue + "' ";
            where_joining = " WHERE pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_employee_master.client_code = '" + ddl_client.SelectedValue + "' and client_wise_state = '" + ddl_state.SelectedValue + "' ";
            where_billing = " WHERE comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_state.SelectedValue + "' ";
        }

        if (ddl_client.SelectedValue == "ALL")
        {
            where_billing = " WHERE comp_code = '" + Session["comp_code"].ToString() + "' ";
        }
        if (i == 1)
        {
            sql = "SELECT pay_billing_unit_rate_history.client, pay_billing_unit_rate_history.state_name, pay_billing_unit_rate_history.unit_name, pay_billing_unit_rate_history.emp_name, pay_billing_unit_rate_history.grade_desc, pay_billing_unit_rate_history.month, pay_billing_unit_rate_history.year, ROUND(pf, 2) AS 'PF_EmployerAmount', ROUND(sal_pf, 2) AS 'PF_EmployeeAmount' FROM pay_employee_master INNER JOIN pay_billing_unit_rate_history ON pay_billing_unit_rate_history.emp_code = pay_employee_master.emp_code AND pay_billing_unit_rate_history.emp_type IN ('Permanent', 'Temporary') INNER JOIN pay_pro_master ON pay_pro_master.emp_code = pay_employee_master.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year AND payment_status = 1 WHERE pay_billing_unit_rate_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_unit_rate_history.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' ORDER BY 1, 2, 3, 4 ";
            //sql = "SELECT pay_billing_unit_rate_history.client, pay_billing_unit_rate_history.state_name, pay_billing_unit_rate_history.unit_name, pay_billing_unit_rate_history.emp_name, pay_billing_unit_rate_history.grade_desc, pay_billing_unit_rate_history.month, ROUND(pf, 2) AS 'PF_EmployerAmount', ROUND(sal_pf, 2) AS 'PF_EmployeeAmount' FROM pay_billing_unit_rate_history INNER JOIN pay_pro_master ON pay_billing_unit_rate_history.comp_code = pay_pro_master.comp_code   AND pay_pro_master.state_name = pay_billing_unit_rate_history.state_name   AND pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code   AND invoice_no IS NOT NULL INNER JOIN pay_employee_master ON pay_pro_master.emp_code = pay_employee_master.emp_code  WHERE pay_billing_unit_rate_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_unit_rate_history.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' ORDER BY pay_billing_unit_rate_history.state_name";
            //sql = "SELECT client, state_name, unit_name, emp_name, grade_desc, month, ROUND(pf,2) as 'PF_Amount' FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND client_code = '" + ddl_client.SelectedValue + "' AND month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "' ORDER BY state_name";

        }
        if (i == 2)
        {
            sql = "SELECT pay_billing_unit_rate_history.client, pay_billing_unit_rate_history.state_name, pay_billing_unit_rate_history.unit_name, pay_billing_unit_rate_history.emp_name, pay_billing_unit_rate_history.grade_desc, pay_billing_unit_rate_history.month, pay_billing_unit_rate_history.year, ROUND(lwf, 2) AS 'LWF_EmployerAmount', ROUND(lwf_salary, 2) AS 'LWF_EmployeeAmount' FROM pay_employee_master INNER JOIN pay_billing_unit_rate_history ON pay_billing_unit_rate_history.emp_code = pay_employee_master.emp_code AND pay_billing_unit_rate_history.emp_type IN ('Permanent', 'Temporary') INNER JOIN pay_pro_master ON pay_pro_master.emp_code = pay_employee_master.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year AND payment_status = 1 WHERE pay_billing_unit_rate_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_unit_rate_history.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' ORDER BY 1, 2, 3, 4 ";
            // sql = "SELECT client, state_name, unit_name, emp_name, grade_desc, month, ROUND(lwf,2) as 'LWF_Amount' FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND client_code = '" + ddl_client.SelectedValue + "' AND month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "' ORDER BY state_name";

        }
        if (i == 3)
        {

            sql = "SELECT client, state_name, unit_name, emp_name, grade, month,year,ROUND(PT_AMOUNT,2) as 'PT_Amount' FROM pay_pro_master WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND client_code = '" + ddl_client.SelectedValue + "' AND month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "'  AND payment_status = 1 ORDER BY 1, 2, 3, 4";

        }
        if (i == 4)
        {

            sql = "SELECT CONCAT(pay_pro_master.client, '-', IFNULL(pay_pro_master.designation, '')) AS 'client', pay_pro_master.state_name, pay_pro_master.unit_name, CASE grade WHEN 'OFFICE BOY' THEN CASE pay_employee_master.gender WHEN 'M' THEN 'OFFICE BOY' WHEN 'F' THEN 'OFFICE LADY' ELSE 'OFFICE BOY' END ELSE grade END AS 'grade', pay_pro_master.emp_name, ROUND(((pay_pro_master.gross * 1.75) / 100), 2) AS 'sal_esic', ROUND(((pay_pro_master.gross * 4.75) / 100), 2) AS 'bill_esic' FROM pay_pro_master INNER JOIN pay_billing_unit_rate_history ON pay_pro_master.comp_code = pay_billing_unit_rate_history.comp_code AND pay_pro_master.state_name = pay_billing_unit_rate_history.state_name AND pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code AND invoice_no IS NOT NULL INNER JOIN pay_employee_master ON pay_pro_master.emp_code = pay_employee_master.emp_code WHERE pay_pro_master.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_pro_master.year = '" + txt_date.Text.Substring(3) + "' AND pay_pro_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_pro_master.state_name = '" + ddl_state.SelectedValue + "' AND (pay_pro_master.Employee_type = 'Temporary' OR pay_pro_master.Employee_type = 'Permanent') AND (PAN_No IS NOT NULL AND PAN_No != '') AND (ESI_No IS NOT NULL AND ESI_No != '') GROUP BY pay_pro_master.client, pay_pro_master.state_name, pay_pro_master.unit_name, pay_pro_master.emp_name";

        }
        if (i == 5)
        {

            sql = "SELECT client, state_name, IFNULL( auto_invoice_no ,  invoice_no ) AS 'invoice_no',month,CGST9,SGST9,IGST18,(CGST9+SGST9+IGST18) as 'TOTAL GST' FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND client_code = '" + ddl_client.SelectedValue + "' AND month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "' AND ( emp_code  != '' OR  emp_code  IS NOT NULL) ORDER BY 1,2,3";

        }
        if (i == 6)
        {

            sql = "SELECT client_name, state_name, unit_name, LocationHead_Name, LocationHead_mobileno, LocationHead_Emailid, OperationHead_Name, OperationHead_Mobileno, OperationHead_EmailId, FinanceHead_Name, FinanceHead_Mobileno, FinanceHead_EmailId, adminhead_name, adminhead_mobile, adminhead_email FROM pay_unit_master inner join pay_client_master on pay_unit_master.comp_code = pay_client_master.comp_code and pay_unit_master.client_code = pay_client_master.client_code " + where_head;

        }
        if (i == 7)
        {

            sql = "SELECT client_name, state_name, unit_name, emp_name, grade,month, year, (CASE WHEN branch_email = 0 THEN 'Not Send' WHEN branch_email = 2 THEN 'Send' ELSE 'Not Send' END) AS 'status' FROM pay_pro_master INNER JOIN pay_client_master ON pay_pro_master.comp_code = pay_client_master.comp_code AND pay_pro_master.client_code = pay_client_master.client_code " + where_salary + " and  pay_pro_master.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_pro_master.year = '" + txt_date.Text.Substring(3) + "' and employee_type != 'Reliever' ";

        }
        if (i == 8)
        {

            sql = "  SELECT client_name, client_wise_state AS 'state_name', unit_name, emp_name, GRADE_DESC as 'designation', date_format(joining_date, '%d/%m/%y') AS 'joining_date',  (CASE WHEN joining_letter_email = 0 THEN 'Not Send' WHEN joining_letter_email = 1 THEN 'Send' ELSE 'Not Send' END) AS 'status' FROM pay_employee_master INNER JOIN pay_client_master ON pay_employee_master.comp_code = pay_client_master.comp_code AND pay_employee_master.client_code = pay_client_master.client_code INNER JOIN pay_unit_master ON pay_employee_master.comp_code = pay_unit_master.comp_code AND pay_unit_master.client_code = pay_employee_master.client_code AND pay_unit_master.unit_code = pay_employee_master.unit_code INNER JOIN  pay_grade_master  ON  pay_employee_master . comp_code  =  pay_grade_master . comp_code  and  pay_employee_master . GRADE_CODE  =  pay_grade_master . GRADE_CODE " + where_joining + " and left_date is null  ORDER BY 3";

        }
        if (i == 9)
        {
            if (ddl_bill_type.SelectedValue == "1")
            {
                client = ",client";
            }

            if ((ddl_bill_type.SelectedValue == "1") || (ddl_bill_type.SelectedValue == "2"))
            {
                sql = "SELECT client, state_name,month,year, sum(Amount + uniform + operational_cost + conveyance_rate + group_insurance_billing + ot_amount + service_charge) AS 'Amount', sum((CGST9) + (SGST9) + (IGST18)) AS 'GST', sum((Amount) + (uniform) + (operational_cost) + (Service_charge) + (ot_amount) + (group_insurance_billing) + (conveyance_rate) + (CGST9) + (SGST9) + (IGST18)) AS 'Grand Total'  FROM pay_billing_unit_rate_history " + where_billing + "and month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "' and invoice_flag = 1 and  EMP_CODE != '' GROUP BY pay_billing_unit_rate_history.state_name " + client + " ORDER BY 1, 2, 3, 4";
            }
            if (ddl_bill_type.SelectedValue == "3")
            {
                sql = "SELECT client, state_name, unit_name,month,year,  sum(Amount + uniform + operational_cost + conveyance_rate + group_insurance_billing + ot_amount + service_charge) AS 'Amount', sum((CGST9) + (SGST9) + (IGST18)) AS 'GST', sum((Amount) + (uniform) + (operational_cost) + (Service_charge) + (ot_amount) + (group_insurance_billing) + (conveyance_rate) + (CGST9) + (SGST9) + (IGST18)) AS 'Grand Total' FROM pay_billing_unit_rate_history " + where_billing + "and month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "' and invoice_flag = 1 and  EMP_CODE != '' GROUP BY pay_billing_unit_rate_history.unit_name ORDER BY 1, 2, 3, 4";
            }
        }

        MySqlCommand cmd = new MySqlCommand(sql, d.con);

        MySqlDataAdapter dscmd = new MySqlDataAdapter(cmd);

        DataSet ds = new DataSet();
        dscmd.Fill(ds);

        if (ds.Tables[0].Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            if (i == 1)
            {
                Response.AddHeader("content-disposition", "attachment;filename=PF_REPORT.xls");
            }
            else if (i == 2)
            {
                Response.AddHeader("content-disposition", "attachment;filename=LWF_REPORT.xls");
            }

            else if (i == 3)
            {
                Response.AddHeader("content-disposition", "attachment;filename=PT_REPORT.xls");
            }
            else if (i == 4)
            {
                Response.AddHeader("content-disposition", "attachment;filename=ESIC_REPORT.xls");
            }
            else if (i == 5)
            {
                Response.AddHeader("content-disposition", "attachment;filename=GST_REPORT.xls");
            }
            else if (i == 6)
            {
                Response.AddHeader("content-disposition", "attachment;filename= HEAD_CONTACT_DETAILS.xls");
            }
            else if (i == 7)
            {
                Response.AddHeader("content-disposition", "attachment;filename=SALARY_SLIP_SENDING_DETAILS.xls");
            }
            else if (i == 8)
            {
                Response.AddHeader("content-disposition", "attachment;filename=JOINING_LETTER_DETAILS.xls");
            }
            else if (i == 9)
            {
                if (t == "1")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=BILLING_DETAILS_CLIENTWISE.xls");
                }
                if (t == "2")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=BILLING_DETAILS_STATEWISE.xls");
                }
                if (t == "3")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=BILLING_DETAILS_BRANCHWISE.xls");
                }
            }
            string date1 = txt_date.Text;

            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Repeater Repeater1 = new Repeater();
            Repeater1.DataSource = ds;
            Repeater1.HeaderTemplate = new MyTemplate12(ListItemType.Header, ds, i, date1, t, 1);
            Repeater1.ItemTemplate = new MyTemplate12(ListItemType.Item, ds, i, date1, t, 1);
            Repeater1.FooterTemplate = new MyTemplate12(ListItemType.Footer, null, i, date1, t, 1);
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
    //ada 
    public class MyTemplate12 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;
        static int ctr1;
        int i;
        string emp_type;
        string date1;
        string t;
        double emp_esic = 0, empr_esic = 0, total = 0;
        string client_name = "";
        int i3 = 1;
        private ListItemType listItemType;
        double amount = 0, gst = 0, grand_total = 0, amount1 = 0, gst1 = 0, grand_total1 = 0;

        private string getmonth(string month)
        {
            month = (int.Parse(month)).ToString();
            if (month == "1")
            {
                return "JAN";
            }
            else if (month == "2")
            { return "FEB"; }
            else if (month == "3")
            { return "MAR"; }
            else if (month == "4")
            { return "APR"; }
            else if (month == "5")
            { return "MAY"; }
            else if (month == "6")
            { return "JUN"; }
            else if (month == "7")
            { return "JUL"; }
            else if (month == "8")
            { return "AUG"; }
            else if (month == "9")
            { return "SEP"; }
            else if (month == "10")
            { return "OCT"; }
            else if (month == "11")
            { return "NOV"; }
            else if (month == "12")
            { return "DEC"; }
            return "";

        }
        public MyTemplate12(ListItemType type, DataSet ds, int i, string date1, string t, int i3)
        {
            // TODO: Complete member initialization
            this.type = type;
            this.ds = ds;
            this.i = i;
            this.date1 = date1;
            this.t = t;

            this.i3 = i3;

        }
        public void InstantiateIn(Control container)
        {
            switch (type)
            { //Original Bank A/C Number ,PF_IFSC_CODE,BANK_HOLDER_NAME
                case ListItemType.Header:


                    // var today = DateTime.Now;
                    var current_date = date1;

                    if (i == 1)
                    {
                        lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=11 align=center> PF REPORT MONTH " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><table border=1><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>BRANCH NAME</th><th>DESIGNATION</th><th>EMPLOYEE NAME</th><th>MONTH</th><th>YEAR</th><th>EMPLOYER CONTRIBUTION</th><th>EMPLOYEE CONTRIBUTION</th><th>TOTAL AMOUNT</th></tr>");
                    }
                    else if (i == 2)
                    {
                        lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=8 align=center> LWF REPORT MONTH " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><table border=1><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>BRANCH NAME</th><th>DESIGNATION</th><th>EMPLOYEE NAME</th><th>MONTH</th><th>YEAR</th><th>EMPLOYER CONTRIBUTION</th><th>EMPLOYEE CONTRIBUTION</th><th>TOTAL AMOUNT</th></tr>");
                    }
                    else if (i == 3)
                    {
                        lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=9 align=center> PT REPORT MONTH " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><table border=1><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>BRANCH NAME</th><th>DESIGNATION</th><th>EMPLOYEE NAME</th><th>MONTH</th><th>YEAR</th><th>AMOUNT</th></tr>");
                    }
                    else if (i == 4)
                    {
                        lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=9 align=center> ESIC REPORT MONTH " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><table border=1><tr><th>SR. NO.</th><th>STATE NAME</th><th>CLIENT NAME</th><th>BRANCH NAME</th><th>DESIGNATION</th><th>EMPLOYEE NAME</th><th>EMPLOYEE CONTRIBUTION</th><th>EMPLOYER CONTRIBUTION</th><th>TOTAL AMOUNT</th></tr>");

                    }
                    else if (i == 5)
                    {
                        lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=9 align=center> GST REPORT MONTH " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><table border=1><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>INVOICE NO</th><th>MONTH</th><th>CGST</th><th>SGST</th><th>IGST</th><th>TOTAL GST</th></tr>");
                    }
                    else if (i == 6)
                    {
                        lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=16 align=center> HEAD CONTACT DETAILS</th></tr><table border=1><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>BRANCH NAME</th><th>LOCATION HEAD NAME</th><th>LOCATION HEAD MOBILE NO</th><th>LOCATION HEAD E-MAIL</th><th>OPERTION HEAD NAME</th><th>OPERTION HEAD MOBILE NO</th><th>OPERTION HEAD E-MAIL</th><th>FINANCE HEAD NAME</th><th>FINANCE HEAD MOBILE NO</th><th>FINANCE HEAD E-MAIL</th><th>ADMIN HEAD NAME</th><th>ADMIN HEAD MOBILE NO</th><th>ADMIN HEAD E-MAIL</th></tr>");
                    }
                    else if (i == 7)
                    {
                        lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=9 align=center> SALARY SLIP SENDING DETAILS MONTH " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><table border=1><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>BRANCH NAME</th><th>EMPLOYEE NAME</th><th>DESIGNATION</th><th>MONTH</th><th>YEAR</th><th>STATUS</th></tr>");
                    }
                    else if (i == 8)
                    {
                        lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=8 align=center> JOINING LETTER DETAILS </th></tr><table border=1><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>BRANCH NAME</th><th>EMPLOYEE NAME</th><th>DESIGNATION</th><th>JOINING DATE</th><th>STATUS</th></tr>");
                    }
                    else if (i == 9)
                    {
                        if ((t == "1") || (t == "2"))
                        {
                            lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=8 align=center> BILLING DETAILS MONTH " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><table border=1><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>MONTH</th><th>YEAR</th><th>AMOUNT</th><th>GST</th><th>GRAND TOTAL</th></tr>");
                        }
                        if (t == "3")
                        {
                            lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=9 align=center> BILLING DETAILS MONTH " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><table border=1><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>BRANCH NAME</th><th>MONTH</th><th>YEAR</th><th>AMOUNT</th><th>GST</th><th>GRAND TOTAL</th></tr>");
                        }

                    }
                    break;
                case ListItemType.Item:
                    if (i == 1)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["year"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["PF_EmployerAmount"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["PF_EmployeeAmount"].ToString().ToUpper() + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["PF_EmployerAmount"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["PF_EmployeeAmount"].ToString()), 2) + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 10>Total</td><td>=ROUND(SUM(k3:k" + (ctr + 3) + "),2)</td></b></tr>";
                        }
                    }
                    else if (i == 2)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["year"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["LWF_EmployerAmount"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["LWF_EmployeeAmount"].ToString().ToUpper() + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["LWF_EmployerAmount"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["LWF_EmployeeAmount"].ToString()), 2) + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 10>Total</td><td>=ROUND(SUM(k3:k" + (ctr + 3) + "),2)</td></b></tr>";
                        }
                    }
                    else if (i == 3)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["year"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["PT_Amount"].ToString().ToUpper() + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 8>Total</td><td>=ROUND(SUM(I3:I" + (ctr + 3) + "),2)</td></b></tr>";
                        }
                    }
                    else if (i == 4)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["client"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grade"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + Math.Round((double.Parse(ds.Tables[0].Rows[ctr]["sal_esic"].ToString())), 2) + "</td><td>" + Math.Round((double.Parse(ds.Tables[0].Rows[ctr]["bill_esic"].ToString())), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sal_esic"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["bill_esic"].ToString()), 2) + "</td></tr>");
                        emp_esic = emp_esic + Math.Round((double.Parse(ds.Tables[0].Rows[ctr]["sal_esic"].ToString())), 2);
                        empr_esic = empr_esic + (double.Parse(ds.Tables[0].Rows[ctr]["bill_esic"].ToString()));
                        total = total + (double.Parse(ds.Tables[0].Rows[ctr]["sal_esic"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["bill_esic"].ToString()));

                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 8>Total</td><td>=ROUND(SUM(I3:I" + (ctr + 3) + "),0)</td></b></tr>";
                        }

                    }
                    else if (i == 5)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["invoice_no"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["CGST9"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["SGST9"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["IGST18"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["TOTAL GST"].ToString().ToUpper() + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 8>Total</td><td>=ROUND(SUM(I3:I" + (ctr + 3) + "),2)</td></b></tr>";
                        }
                    }
                    else if (i == 6)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_NAME"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["LocationHead_Name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["LocationHead_mobileno"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["LocationHead_Emailid"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["OperationHead_Name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["OperationHead_Mobileno"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["OperationHead_EmailId"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["FinanceHead_Name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["FinanceHead_Mobileno"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["FinanceHead_EmailId"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["adminhead_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["adminhead_mobile"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["adminhead_email"].ToString().ToUpper() + "</td></tr>");

                    }
                    else if (i == 7)
                    {
                        string bg = "";
                        if (ds.Tables[0].Rows[ctr]["status"].ToString() == "Send")
                        {
                            bg = "bgcolor=green";
                        }
                        else
                        {
                            bg = "bgcolor=red";
                        }
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["GRADE"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["year"].ToString().ToUpper() + "</td><td " + bg + ">" + ds.Tables[0].Rows[ctr]["status"].ToString().ToUpper() + "</td></tr>");
                    }
                    else if (i == 8)
                    {
                        string bg = "";
                        if (ds.Tables[0].Rows[ctr]["status"].ToString() == "Send")
                        {
                            bg = "bgcolor=green";
                        }
                        else
                        {
                            bg = "bgcolor=red";
                        }
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["designation"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["joining_date"].ToString().ToUpper() + "</td><td " + bg + ">" + ds.Tables[0].Rows[ctr]["status"].ToString().ToUpper() + "</td></tr>");
                    }
                    else if (i == 9)
                    {
                        int set_start_row = 3;
                        if (t == "2")
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["year"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Amount"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["GST"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Grand Total"].ToString().ToUpper() + "</td></tr>");
                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                lc.Text = lc.Text + "<tr><b><td align=center colspan = 5>Total</td><td>=ROUND(SUM(F3:F" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(H3:H" + (ctr + 3) + "),2)</td></b></tr>";
                            }
                        }

                        if (t == "1")
                        {
                            if (client_name != ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper())
                            {
                                if (client_name != "")
                                {
                                    i3 = i3 + 1;

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan = 5>Total</td><td>" + amount + "</td><td>" + gst + "</td><td>" + grand_total + "</td></b></tr>";

                                    ctr1 = ctr + i3 + 1;
                                    amount = 0; gst = 0; grand_total = 0;

                                }
                            }
                            amount = amount + double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString());
                            gst = gst + double.Parse(ds.Tables[0].Rows[ctr]["GST"].ToString());
                            grand_total = grand_total + double.Parse(ds.Tables[0].Rows[ctr]["Grand Total"].ToString());

                            amount1 = amount1 + double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString());
                            gst1 = gst1 + double.Parse(ds.Tables[0].Rows[ctr]["GST"].ToString());
                            grand_total1 = grand_total1 + double.Parse(ds.Tables[0].Rows[ctr]["Grand Total"].ToString());

                            client_name = ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper();
                            // lc = new LiteralControl( "<tr><b><td align=center colspan = 5>Total</td><td>=ROUND(SUM(F3:F" + (ctr + i) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + i) + "),2)</td><td>=ROUND(SUM(H3:H" + (ctr + i) + "),2)</td></b></tr>");
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["year"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Amount"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["GST"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Grand Total"].ToString().ToUpper() + "</td></tr>");
                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                lc.Text = lc.Text + "<tr><b><td align=center colspan = 5>Total</td><td>" + amount + "</td><td>" + gst + "</td><td>" + grand_total + "</td></b></tr>";

                                lc.Text = lc.Text + "<tr><b><td align=center colspan = 5>GRAND TOTAL</td><td>" + amount1 + "</td><td>" + gst1 + "</td><td>" + grand_total1 + "</td></b></tr>";
                            }
                        }


                        if (t == "3")
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["year"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Amount"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["GST"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Grand Total"].ToString().ToUpper() + "</td></tr>");
                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                lc.Text = lc.Text + "<tr><b><td align=center colspan = 6>Total</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(H3:H" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(I3:I" + (ctr + 3) + "),2)</td></b></tr>";
                            }
                        }

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
    protected void gst_report_Click(object sender, EventArgs e)
    {
        counter = 1;
        if (ddl_gst_type.SelectedValue == "ALL")
        {
            all_gst_report("ALL", 1);
        }
        else if (ddl_gst_type.SelectedValue == "1")
        {
            all_gst_report("manpower", 1);
        }
        else if (ddl_gst_type.SelectedValue == "2")
        {
            all_gst_report("conveyance", 1);
        }
        else if (ddl_gst_type.SelectedValue == "3")
        {
            all_gst_report("driver_conveyance", 1);
        }
        else if (ddl_gst_type.SelectedValue == "4")
        {
            all_gst_report("material", 1);
        }
        else if (ddl_gst_type.SelectedValue == "5")
        {
            all_gst_report("deepclean", 1);
        }
        else if (ddl_gst_type.SelectedValue == "6")
        {
            all_gst_report("machine_rental", 1);
        }
        else if (ddl_gst_type.SelectedValue == "7")
        {
            all_gst_report("arrears_manpower", 1);
        }
        else if (ddl_gst_type.SelectedValue == "8")
        {
            all_gst_report("manual", 1);
        }
        else if (ddl_gst_type.SelectedValue == "9")
        {
            all_gst_report("r_and_m_bill", 1);
        }
        else if (ddl_gst_type.SelectedValue == "10")
        {
            all_gst_report("administrative_bill", 1);
        }
        else if (ddl_gst_type.SelectedValue == "11")
        {
            all_gst_report("shiftwise_bill", 1);
        }
        else if (ddl_gst_type.SelectedValue == "12")
        {
            all_gst_report("incentive_bill", 1);
        }
        else if (ddl_gst_type.SelectedValue == "13")
        {
            all_gst_report("office_rent_bill", 1);
        }
        else if (ddl_gst_type.SelectedValue == "14")
        {
            all_gst_report("manpower_ot", 1);
        }

    }
    protected void all_gst_report(string type, int counter)
    {
        hidtab.Value = "5";
        string query = "";
        try
        {

            string where = "";
            string invoice_flag = "";
            string billing_type = "";
            string order_by = "order by client_name";
            if (type != "ALL")
            {
                billing_type = " and G.type = '" + type + "'";
                // billing_type = " and type = '" + type + "'";
            }
            if (ddl_client.SelectedValue != "ALL")
            {
                order_by = "order by invoice_no";
                //where = "  and pay_report_gst.client_code='" + ddl_client.SelectedValue + "' ";
                where = "  and G.client_code='" + ddl_client.SelectedValue + "' ";

            }
            else if (ddl_state.SelectedValue != "ALL")
            {
                //where = where + " and pay_report_gst.state_name ='" + ddl_state.SelectedValue + "'";
                where = where + " and G.state_name ='" + ddl_state.SelectedValue + "'";

            }
            if (type == "manual")
            {
                invoice_flag = " and final_invoice !='0'";

                //if (ddl_client.SelectedValue != "ALL")
                //{
                //    where = " and client_name='" + ddl_client.SelectedItem + "'";

                //}
                //else if (ddl_state.SelectedValue != "ALL")
                //{
                //    where = where + " and state_name ='" + ddl_state.SelectedValue + "'";

                //}
            }
            if (counter == 1)
            {
                query = "SELECT DATE_FORMAT(invoice_date, '%d/%m/%Y') AS 'billing_date',  IF(G.type = 'manual', CASE WHEN manual_invoice_bill_type = 'Manpower Billing' THEN 'manual(manpower)'   WHEN manual_invoice_bill_type = 'Conveyance Billing' THEN 'manual(conveyance)'  WHEN manual_invoice_bill_type = 'Driver Convenyance' THEN 'manual(driver_conveyance)' WHEN manual_invoice_bill_type = 'Machine Rental' THEN 'manual(machine_rental)' WHEN manual_invoice_bill_type = 'Material Billing' THEN 'manual(material)' WHEN manual_invoice_bill_type = 'Deep Clean Billing' THEN 'manual(deepclean)' WHEN manual_invoice_bill_type = 'OT Billing' THEN 'manual(manpower_ot)' WHEN manual_invoice_bill_type = 'Office Rent Billing' THEN 'manual(office_rent_bill)' WHEN manual_invoice_bill_type = 'Shiftwise Billing' THEN 'manual(shiftwise_bill)' WHEN manual_invoice_bill_type = 'R And M Service' THEN 'manual(r_and_m_bill)' WHEN manual_invoice_bill_type = 'Administrative Expenses' THEN 'manual(administrative_bill)' else 'manual'  END, G.type) AS 'type',  month,  year, invoice_no, G.client_name, replace(IF(G.state_name='Pondicherry','Puducherry',G.state_name),'2','') as state_name, Z.Field3, TRIM(Field1) AS Field1, gst_no, ROUND(amount, 2) AS 'amount',ROUND(cgst, 2) AS 'cgst',ROUND(sgst, 2) AS 'sgst',ROUND(igst, 2) AS 'igst',ROUND(cgst + igst + sgst, 2) AS 'gst',ROUND(cgst + igst + sgst + amount, 2) AS 'Total_BILL', sac_code, G.creditnote_against_invoice_no,IF(G.e_invoice_status=0,'Pending','Done') as e_invoice_status  FROM pay_report_gst G LEFT join pay_zone_master Z on G.state_name = Z.region and Z.type='GST' and G.client_code = Z.client_code AND G.comp_code = Z.COMP_CODE WHERE (invoice_no IS NOT NULL  AND invoice_no !='') and G.comp_code = '" + Session["comp_code"].ToString() + "' and invoice_date between str_to_date('" + gst_from_date.Text + "','%d/%m/%Y') and str_to_date('" + gst_to_date.Text + "','%d/%m/%Y')  " + billing_type + "" + where + invoice_flag + " and (amount is not null ||amount != 0)  order by client_name,G.type";
                //query = "SELECT DATE_FORMAT(invoice_date, '%d/%m/%Y') AS 'billing_date',type, month, year, invoice_no, client_name, state_name, gst_no,ROUND(amount, 2) AS 'amount', ROUND(cgst, 2) AS 'cgst', ROUND(sgst, 2) AS 'sgst', ROUND(igst, 2)AS 'igst', ROUND(cgst + igst + sgst, 2) AS 'gst', ROUND(cgst + igst + sgst + amount, 2) AS 'Total_BILL',sac_code FROM pay_report_gst WHERE  (invoice_no IS NOT NULL  AND invoice_no !='') and comp_code = '" + Session["comp_code"].ToString() + "' and invoice_date between str_to_date('" + gst_from_date.Text + "','%d/%m/%Y') and str_to_date('" + gst_to_date.Text + "','%d/%m/%Y')  " + billing_type + "" + where + invoice_flag + " and (amount is not null ||amount != 0)  order by client_name,type";
            }
            //new button for sac wise gst report
            else if (counter == 2)
            {
                query = "SELECT  DATE_FORMAT(invoice_date, '%d/%m/%Y') AS 'billing_date', IF(G.type = 'manual', CASE     WHEN manual_invoice_bill_type = 'Manpower Billing' THEN 'manpower'     WHEN manual_invoice_bill_type = 'Conveyance Billing' THEN 'conveyance'     WHEN manual_invoice_bill_type = 'Driver Convenyance' THEN 'driver_conveyance'     WHEN manual_invoice_bill_type = 'Machine Rental' THEN 'machine_rental'     WHEN manual_invoice_bill_type = 'Material Billing' THEN 'material'     WHEN manual_invoice_bill_type = 'Deep Clean Billing' THEN 'deepclean'     WHEN manual_invoice_bill_type = 'OT Billing' THEN 'manpower_ot'     WHEN manual_invoice_bill_type = 'Office Rent Billing' THEN 'office_rent_bill'     WHEN manual_invoice_bill_type = 'Shiftwise Billing' THEN 'shiftwise_bill'     WHEN manual_invoice_bill_type = 'R And M Service' THEN 'r_and_m_bill'     WHEN manual_invoice_bill_type = 'Administrative Expenses' THEN 'administrative_bill'     else         'manual' END, G.type) AS 'type', G.month, G.year, G.invoice_no, G.client_name,case WHEN  G.state_name = 'Maharashtra-Mumbai' then  REPLACE(IF(G.state_name = 'Maharashtra-Mumbai', 'Maharashtra', G.state_name), '2', '')  else REPLACE(IF(G.state_name = 'Pondicherry', 'Puducherry', G.state_name), '2', '') end AS state_name, gst_no, IF(length(Field1)>100, REVERSE(SUBSTRING(REVERSE(SUBSTRING(Field1, 1, 100)), INSTR(REVERSE(SUBSTRING(Field1, 1, 100)), ','))),Field1) as GST_addr_100char,IF(length(Field1)>100, CONCAT(substring_index(SUBSTRING(Field1, 1, 100), ',', -1),'', SUBSTRING(Field1, 101, 200)),'') as GST_add_aftr_100_char, Field3, ROUND(G.amount, 2) AS 'amount', ROUND(cgst, 2) AS 'cgst', ROUND(sgst, 2) AS 'sgst', ROUND(igst, 2) AS 'igst', ROUND(cgst + igst + sgst, 2) AS 'gst', ROUND(cgst + igst + sgst + G.amount, 2) AS 'Total_BILL', sac_code, SUM(tot_days_present) AS 'no_of_paid_days', G.creditnote_against_invoice_no,IF(G.e_invoice_status=0,'Pending','Done') as e_invoice_status    FROM pay_report_gst G LEFT OUTER JOIN pay_billing_unit_rate_history ON G.comp_code = pay_billing_unit_rate_history.comp_code  AND G.invoice_no = pay_billing_unit_rate_history.invoice_no  LEFT JOIN pay_zone_master Z ON G.state_name = Z.region AND Z.type = 'GST' AND G.client_code = Z.client_code  AND G.comp_code = Z.COMP_CODE WHERE (G.invoice_no IS NOT NULL  AND G.invoice_no != '')     AND G.comp_code = '" + Session["comp_code"].ToString() + "'  AND invoice_date BETWEEN STR_TO_DATE('" + gst_from_date.Text + "','%d/%m/%Y') and str_to_date('" + gst_to_date.Text + "','%d/%m/%Y')  " + billing_type + "" + where + invoice_flag + " AND (G.amount IS NOT NULL || G.amount != 0) GROUP BY invoice_no " + order_by + ", G.type";
                //query = "SELECT DATE_FORMAT(invoice_date, '%d/%m/%Y') AS 'billing_date',type,pay_report_gst. month,pay_report_gst. year, pay_report_gst.invoice_no, client_name,pay_report_gst. state_name, gst_no,ROUND(pay_report_gst.amount, 2) AS 'amount', ROUND(cgst, 2) AS 'cgst', ROUND(sgst, 2) AS 'sgst', ROUND(igst, 2)AS 'igst', ROUND(cgst + igst + sgst, 2) AS 'gst', ROUND(cgst + igst + sgst + pay_report_gst.amount, 2) AS 'Total_BILL', sac_code,sum(tot_days_present) as 'no_of_paid_days' FROM pay_report_gst  left outer join pay_billing_unit_rate_history on pay_report_gst.comp_code=pay_billing_unit_rate_history.comp_code and pay_report_gst.invoice_no=pay_billing_unit_rate_history.invoice_no WHERE  (pay_report_gst.invoice_no IS NOT NULL  AND pay_report_gst.invoice_no !='') and pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' and invoice_date between str_to_date('" + gst_from_date.Text + "','%d/%m/%Y') and str_to_date('" + gst_to_date.Text + "','%d/%m/%Y')  " + billing_type + "" + where + invoice_flag + " and (pay_report_gst.amount is not null ||pay_report_gst.amount != 0) group by invoice_no  " + order_by + ",type";
            }
            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();

            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);


            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                //existing gst report
                if (counter == 1)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=GST_Report" + ddl_client.SelectedItem.Text.Replace(" ", "_") + ".xls");
                }
                //sac wise gst report
                else if (counter == 2)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=SAC_WISE_GST_Report" + ddl_client.SelectedItem.Text.Replace(" ", "_") + ".xls");
                }
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate1(ListItemType.Header, ds,counter);
                Repeater1.ItemTemplate = new MyTemplate1(ListItemType.Item, ds,counter);
                Repeater1.FooterTemplate = new MyTemplate1(ListItemType.Footer, null,counter);
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
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    public class MyTemplate1 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        int counter;
        static int ctr;



        public MyTemplate1(ListItemType type, DataSet ds,int counter)
        {
            this.type = type;
            this.ds = ds;
            ctr = 0;
            this.counter = counter;

        }

        public void InstantiateIn(Control container)
        {


            switch (type)
            {
                case ListItemType.Header:
                    if (counter == 1)
                    {
                        lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=20>GST Reports</th></tr><tr><th>SR NO.</th><th>Billing Date</th><th>Billing Type</th><th>Month</th><th>Year</th><th>Invoice No</th><th>Client</th><th>State Name</th><th>GST NO.</th><th>Bill Amount</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Total GST</th><th>Total Bill</th><th>SAC CODE</th><th>GST ADDRESS</th><th>GST PINCODE</th><th>REFERENCE NUMBER</th><th>E-Invoice Status</th></tr> ");
                    }
                    else if (counter == 2)
                    {
                        lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=22>SAC WISE GST Reports</th></tr><tr><th>SR NO.</th><th>Billing Date</th><th>Billing Type</th><th>Month</th><th>Year</th><th>Invoice No</th><th>Client</th><th>State Name</th><th>GST NO.</th><th>Bill Amount</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Total GST</th><th>Total Bill</th><th>SAC Code</th><th>NO of Days</th><th>GST ADDRESS1 100 CHAR</th><th>GST ADDRESS2 AFTER 100 CHAR</th><th>GST PINCODE</th><th>REFERENCE NUMBER</th><th>E-Invoice Status</th></tr> ");
                    }
                    break;
                case ListItemType.Item:
                    if (counter == 1)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["billing_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["month"] + "</td><td>" + ds.Tables[0].Rows[ctr]["year"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["igst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Total_BILL"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sac_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Field1"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Field3"] + "</td><td>" + ds.Tables[0].Rows[ctr]["creditnote_against_invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["e_invoice_status"] + "</td></tr>");
                    }
                    else if (counter == 2)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["billing_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["month"] + "</td><td>" + ds.Tables[0].Rows[ctr]["year"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["igst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Total_BILL"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sac_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["no_of_paid_days"] + "</td><td>" + ds.Tables[0].Rows[ctr]["GST_addr_100char"] + "</td><td>" + ds.Tables[0].Rows[ctr]["GST_add_aftr_100_char"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Field3"] + "</td><td>" + ds.Tables[0].Rows[ctr]["creditnote_against_invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["e_invoice_status"] + "</td></tr>");
                    }
                    if (counter == 1)
                    {
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 9>Total</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td></b></tr>";
                        }
                    }
                    else if (counter == 2)
                    {
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 9>Total</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td></td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td></b></tr>";
                        }

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
    protected void ddl_get_report_Click(object sender, EventArgs e)
    {
        hidtab.Value = "6";
        if (dll_type.SelectedValue == "1")
        {
            export_account_xl(1);
        }
        if (dll_type.SelectedValue == "2")
        {
            export_account_xl(2);
        }
        if (dll_type.SelectedValue == "3")
        {
            export_account_xl(3);
        }
        if (dll_type.SelectedValue == "4")
        {
            export_account_xl(4);
        }
        if (dll_type.SelectedValue == "5")
        {
            export_account_xl(5);
        }
    }
    protected void export_account_xl(int i)
    {
        hidtab.Value = "6";
        try
        {
            string sql = "";
            string where = "";
            string where1 = "";
            if (ddl_client.SelectedValue != "ALL")
            {
                where = "  and client_code='" + ddl_client.SelectedValue + "' ";
                where1 = " and pay_pro_master.client_code='" + ddl_client.SelectedValue + "' ";
            }
            if (ddl_state.SelectedValue != "ALL")
            {
                where = where + " and state_name ='" + ddl_state.SelectedValue + "'";
                where1 = where1 + " and pay_pro_master.state_name ='" + ddl_state.SelectedValue + "'";
            }
            if (i == 1)
            {
                if (ddl_client.SelectedValue.Equals("RCPL"))
                {
                    sql = "SELECT pay_billing_unit_rate_history.client, pay_billing_unit_rate_history.state_name, CASE WHEN invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'invoice_no', Amount AS 'Billing_Amount', ROUND(CGST9 + igst18 + sgst9, 2) AS 'gst', ROUND(((SUM(FLOOR(payment - (fine) + (emp_advance_payment) + (emp_advance) + (reliver_advances)))) * pay_billing_unit_rate_history.bill_amount / 100), 2) AS 'payment', ROUND(((pay_billing_unit_rate_history.pf + (SUM(pay_pro_master.sal_pf)) * pay_billing_unit_rate_history.bill_amount / 100)), 2) AS 'PF', ROUND(((pay_billing_unit_rate_history.esic + (SUM(pay_pro_master.sal_esic)) * pay_billing_unit_rate_history.bill_amount / 100)), 2) AS 'ESIC', ROUND(((pay_billing_unit_rate_history.lwf + (SUM(pay_pro_master.lwf_salary)) * pay_billing_unit_rate_history.bill_amount / 100)), 2) AS 'LWF', ROUND((SUM(pay_pro_master.pt_amount) * pay_billing_unit_rate_history.bill_amount / 100), 2) AS 'PT', ROUND(((SUM(pay_billing_unit_rate_history.bonus_after_gross) + SUM(pay_billing_unit_rate_history.leave_after_gross) + SUM(pay_billing_unit_rate_history.gratuity_after_gross)) * pay_billing_unit_rate_history.bill_amount / 100), 2) AS 'Others' FROM pay_billing_unit_rate_history INNER JOIN pay_pro_master ON pay_billing_unit_rate_history.client_code = pay_pro_master.client_code AND pay_billing_unit_rate_history.month = pay_pro_master.month AND pay_billing_unit_rate_history.year = pay_pro_master.year AND payment_status = 1 WHERE pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND billing_date BETWEEN STR_TO_DATE('" + acc_from_date.Text + "', '%d/%m/%Y') AND STR_TO_DATE('" + acc_to_date.Text + "', '%d/%m/%Y') AND (auto_invoice_no IS NOT NULL OR invoice_no IS NOT NULL) GROUP BY invoice_no, auto_invoice_no order by billing_date";
                }
                else
                {
                    sql = "SELECT pay_pro_master.client,  pay_pro_master.state_name,CASE WHEN invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'invoice_no', floor((sum(pay_billing_unit_rate_history.amount) + IF(bill_ser_uniform = 0, ((sum(pay_billing_unit_rate.uniform) / sum(pay_billing_unit_rate.month_days)) * sum(pay_billing_unit_rate_history.tot_days_present)), 0) + IF(bill_ser_operations = 0, ((sum(pay_billing_unit_rate.operational_cost) / sum(pay_billing_unit_rate.month_days)) * sum(pay_billing_unit_rate_history.tot_days_present)), 0) + sum(Service_charge) + SUM(IFNULL(pay_conveyance_amount_history.conveyance_rate, pay_billing_unit_rate_history.conveyance_amount) * tot_days_present / pay_billing_unit_rate_history.month_days) + sum(pay_billing_unit_rate_history.ot_amount))) AS 'Billing_Amount', (case when  pay_client_master.gst_applicable = 1 then ROUND((SUM(CGST9) + SUM(igst18) + SUM(sgst9)), 2) else 0 end) AS 'gst', (SUM(FLOOR(payment - (fine)  + (emp_advance_payment) + (emp_advance) + (reliver_advances)))) AS 'payment', round((sum(pay_billing_unit_rate_history.pf) + sum(pay_pro_master.sal_pf)),2) as PF, round((sum(pay_billing_unit_rate_history.esic) + sum(pay_pro_master.sal_esic)),2) as ESIC, round((sum(pay_billing_unit_rate_history.lwf) + sum(pay_pro_master.lwf_salary)),2) as lwf, round(sum(pay_pro_master.pt_amount),2) as PT, round((sum(pay_billing_unit_rate_history.bonus_after_gross)+sum(pay_billing_unit_rate_history.leave_after_gross)+sum(pay_billing_unit_rate_history.gratuity_after_gross)),2) as Others FROM pay_pro_master INNER JOIN pay_billing_unit_rate_history ON pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year INNER JOIN pay_billing_unit_rate ON pay_billing_unit_rate_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_unit_rate_history.unit_code = pay_billing_unit_rate.unit_code AND pay_billing_unit_rate_history.month = pay_billing_unit_rate.month AND pay_billing_unit_rate_history.year = pay_billing_unit_rate.year AND pay_billing_unit_rate_history.grade_code = pay_billing_unit_rate.designation INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate_history.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate_history.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate_history.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate_history.month AND pay_billing_master_history.year = pay_billing_unit_rate_history.year AND pay_billing_master_history.designation = pay_billing_unit_rate_history.grade_code AND pay_billing_master_history.hours = pay_billing_unit_rate_history.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_client_master ON pay_client_master.comp_code = pay_billing_unit_rate_history.comp_code AND pay_client_master.client_code = pay_billing_unit_rate_history.client_code LEFT OUTER JOIN pay_conveyance_amount_history ON pay_conveyance_amount_history.emp_code = pay_billing_unit_rate_history.emp_code AND pay_conveyance_amount_history.comp_code = pay_billing_unit_rate_history.comp_code AND pay_conveyance_amount_history.unit_code = pay_billing_unit_rate_history.unit_code AND pay_conveyance_amount_history.month = pay_billing_unit_rate_history.month AND pay_conveyance_amount_history.year = pay_billing_unit_rate_history.year WHERE  pay_pro_master.comp_code  = '" + Session["comp_code"].ToString() + "' AND billing_date BETWEEN STR_TO_DATE('" + acc_from_date.Text + "', '%d/%m/%Y') AND STR_TO_DATE('" + acc_to_date .Text+ "', '%d/%m/%Y') " + where1 + " AND (pay_pro_master.start_date = 0 AND pay_pro_master.end_date = 0) AND (pay_billing_unit_rate_history.start_date = 0 AND pay_billing_unit_rate_history.end_date = 0) group by auto_invoice_no order by billing_date ";
                    if (ddl_client.SelectedValue.Equals("ALL"))
                    {
                        sql = sql + " union SELECT pay_billing_unit_rate_history.client, pay_billing_unit_rate_history.state_name, CASE WHEN invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'invoice_no', Amount AS 'Billing_Amount', ROUND(CGST9 + igst18 + sgst9, 2) AS 'gst', ROUND(((SUM(FLOOR(payment - (fine) + (emp_advance_payment) + (emp_advance) + (reliver_advances)))) * pay_billing_unit_rate_history.bill_amount / 100), 2) AS 'payment', ROUND(((pay_billing_unit_rate_history.pf + (SUM(pay_pro_master.sal_pf)) * pay_billing_unit_rate_history.bill_amount / 100)), 2) AS 'PF', ROUND(((pay_billing_unit_rate_history.esic + (SUM(pay_pro_master.sal_esic)) * pay_billing_unit_rate_history.bill_amount / 100)), 2) AS 'ESIC', ROUND(((pay_billing_unit_rate_history.lwf + (SUM(pay_pro_master.lwf_salary)) * pay_billing_unit_rate_history.bill_amount / 100)), 2) AS 'LWF', ROUND((SUM(pay_pro_master.pt_amount) * pay_billing_unit_rate_history.bill_amount / 100), 2) AS 'PT', ROUND(((SUM(pay_billing_unit_rate_history.bonus_after_gross) + SUM(pay_billing_unit_rate_history.leave_after_gross) + SUM(pay_billing_unit_rate_history.gratuity_after_gross)) * pay_billing_unit_rate_history.bill_amount / 100), 2) AS 'Others' FROM pay_billing_unit_rate_history INNER JOIN pay_pro_master ON pay_billing_unit_rate_history.client_code = pay_pro_master.client_code AND pay_billing_unit_rate_history.month = pay_pro_master.month AND pay_billing_unit_rate_history.year = pay_pro_master.year AND payment_status = 1 WHERE pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND billing_date BETWEEN STR_TO_DATE('" + acc_from_date.Text + "', '%d/%m/%Y') AND STR_TO_DATE('" + acc_to_date.Text + "', '%d/%m/%Y') AND (auto_invoice_no IS NOT NULL OR invoice_no IS NOT NULL) GROUP BY invoice_no, auto_invoice_no order by billing_date";
                    }

                }
            }
            else if (i == 2)
            {
                sql = "SELECT client_name, state_name, invoice_no, amount, gst,(amount + gst) AS 'grand_total', payment FROM pay_report_gst WHERE  comp_code  = '" + Session["comp_code"].ToString() + "' and month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "'  AND  type = 'conveyance' " + where;
            }
            else if (i == 3)
            {
                sql = "SELECT client_name, state_name, invoice_no, amount, gst,(amount + gst) AS 'grand_total', payment FROM pay_report_gst WHERE   comp_code  = '" + Session["comp_code"].ToString() + "' and month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "'  AND  type  = 'driver_conveyance' " + where;
            }
            else if (i == 4)
            {
                sql = "SELECT client_name, state_name, invoice_no, amount, gst,(amount + gst) AS 'grand_total', payment FROM pay_report_gst WHERE  comp_code  = '" + Session["comp_code"].ToString() + "' and month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "' AND  type  = 'material'  " + where;
            }
            else if (i == 5)
            {
                sql = "SELECT client_name, state_name, invoice_no, amount, gst,(amount + gst) AS 'grand_total', payment FROM pay_report_gst WHERE  comp_code  = '" + Session["comp_code"].ToString() + "' and month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "' AND  type  = 'deep clean'  " + where;
            }

            //double Payment = 0, pf = 0, esic = 0, lwf = 0,pt = 0,Others = 0;

            //      MySqlDataAdapter dscmd1 = new MySqlDataAdapter("SELECT pay_billing_unit_rate_history.client, pay_billing_unit_rate_history.state_name, CASE WHEN invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'invoice_no',  SUM(Amount + uniform + operational_cost + conveyance_rate + group_insurance_billing + ot_amount + service_charge) AS 'Billing_Amount', ROUND((SUM(CGST9) + SUM(igst18) + SUM(sgst9)), 2) AS 'gst' FROM pay_billing_unit_rate_history WHERE comp_code  = '" + Session["comp_code"].ToString() + "' and month = '" + txt_date.Text.Substring(0, 2) + "' AND year = '" + txt_date.Text.Substring(3) + "'  AND auto_invoice_no IS NOT NULL " + where + " GROUP BY auto_invoice_no ORDER BY auto_invoice_no", d.con1);

            //DataTable dt = new DataTable();
            //dscmd1.Fill(dt);
            //dt.Columns.Add("payment");
            //dt.Columns.Add("pf");
            //dt.Columns.Add("ESIC");
            //dt.Columns.Add("LWF");
            //dt.Columns.Add("PT");
            //dt.Columns.Add("others");

            //d.con1.Open();

            //    string payment = d.getsinglestring("SELECT (SUM(FLOOR(payment - (fine) + (emp_advance_payment) + (emp_advance) + (reliver_advances)))) AS 'payment' FROM pay_pro_master INNER JOIN pay_billing_unit_rate_history ON pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year WHERE  pay_pro_master.comp_code  = '" + Session["comp_code"].ToString() + "' and pay_pro_master.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_pro_master.year = '" + txt_date.Text.Substring(3) + "'  " + where1 + " AND (pay_pro_master.start_date = 0 AND pay_pro_master.end_date = 0) AND (pay_billing_unit_rate_history.start_date = 0 AND pay_billing_unit_rate_history.end_date = 0) GROUP BY auto_invoice_no ORDER BY auto_invoice_no");
            //    string PF = d.getsinglestring("SELECT ROUND((SUM(pay_billing_unit_rate_history.pf) + SUM(pay_pro_master.sal_pf)), 2) AS 'PF' FROM pay_pro_master INNER JOIN pay_billing_unit_rate_history ON pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year WHERE  pay_pro_master.comp_code  = '" + Session["comp_code"].ToString() + "' and pay_pro_master.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_pro_master.year = '" + txt_date.Text.Substring(3) + "'  " + where1 + " AND (pay_pro_master.start_date = 0 AND pay_pro_master.end_date = 0) AND (pay_billing_unit_rate_history.start_date = 0 AND pay_billing_unit_rate_history.end_date = 0) GROUP BY auto_invoice_no ORDER BY auto_invoice_no");
            //    string ESIC = d.getsinglestring("SELECT ROUND((SUM(pay_billing_unit_rate_history.esic) + SUM(pay_pro_master.sal_esic)), 2) AS 'ESIC' FROM pay_pro_master INNER JOIN pay_billing_unit_rate_history ON pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year WHERE  pay_pro_master.comp_code  = '" + Session["comp_code"].ToString() + "' and pay_pro_master.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_pro_master.year = '" + txt_date.Text.Substring(3) + "'  " + where1 + " AND (pay_pro_master.start_date = 0 AND pay_pro_master.end_date = 0) AND (pay_billing_unit_rate_history.start_date = 0 AND pay_billing_unit_rate_history.end_date = 0) GROUP BY auto_invoice_no ORDER BY auto_invoice_no");
            //    string LWF = d.getsinglestring("SELECT  ROUND((SUM(pay_billing_unit_rate_history.lwf) + SUM(pay_pro_master.lwf_salary)), 2) AS 'lwf' FROM pay_pro_master INNER JOIN pay_billing_unit_rate_history ON pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year WHERE  pay_pro_master.comp_code  = '" + Session["comp_code"].ToString() + "' and pay_pro_master.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_pro_master.year = '" + txt_date.Text.Substring(3) + "'  " + where1 + " AND (pay_pro_master.start_date = 0 AND pay_pro_master.end_date = 0) AND (pay_billing_unit_rate_history.start_date = 0 AND pay_billing_unit_rate_history.end_date = 0) GROUP BY auto_invoice_no ORDER BY auto_invoice_no");
            //    string PT = d.getsinglestring("SELECT  ROUND(SUM(pay_pro_master.pt_amount), 2) AS 'PT' FROM pay_pro_master INNER JOIN pay_billing_unit_rate_history ON pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year WHERE  pay_pro_master.comp_code  = '" + Session["comp_code"].ToString() + "' and pay_pro_master.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_pro_master.year = '" + txt_date.Text.Substring(3) + "'  " + where1 + " AND (pay_pro_master.start_date = 0 AND pay_pro_master.end_date = 0) AND (pay_billing_unit_rate_history.start_date = 0 AND pay_billing_unit_rate_history.end_date = 0) GROUP BY auto_invoice_no ORDER BY auto_invoice_no");
            //    string others = d.getsinglestring("SELECT ROUND((SUM(pay_billing_unit_rate_history.bonus_after_gross) + SUM(pay_billing_unit_rate_history.leave_after_gross) + SUM(pay_billing_unit_rate_history.gratuity_after_gross)), 2) AS 'Others' FROM pay_pro_master INNER JOIN pay_billing_unit_rate_history ON pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year WHERE  pay_pro_master.comp_code  = '" + Session["comp_code"].ToString() + "' and pay_pro_master.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_pro_master.year = '" + txt_date.Text.Substring(3) + "'  " + where1 + " AND (pay_pro_master.start_date = 0 AND pay_pro_master.end_date = 0) AND (pay_billing_unit_rate_history.start_date = 0 AND pay_billing_unit_rate_history.end_date = 0) GROUP BY auto_invoice_no ORDER BY auto_invoice_no");

            //    //data will not found then return
            //    if (payment.Equals(""))
            //    {
            //        d.con1.Close();
            //        return;
            //    }
            //    MySqlCommand cmd_cg = new MySqlCommand("Select comp_name,IFNULL(percent,0) as percent,Companyname_gst_no,gst_address from pay_company_group where comp_code ='" + Session["COMP_CODE"].ToString() + "' " + where, d1.con1);
            //    d1.con1.Open();
            //    MySqlDataReader dr_cg = cmd_cg.ExecuteReader();
            //    while (dr_cg.Read())
            //    {
            //        Payment = (double.Parse(payment) * double.Parse(dr_cg.GetValue(1).ToString())) / 100;
            //        pf = (double.Parse(PF) * double.Parse(dr_cg.GetValue(1).ToString())) / 100;
            //        esic = (double.Parse(ESIC) * double.Parse(dr_cg.GetValue(1).ToString())) / 100;
            //        lwf = (double.Parse(LWF) * double.Parse(dr_cg.GetValue(1).ToString())) / 100;
            //        pt = (double.Parse(PT) * double.Parse(dr_cg.GetValue(1).ToString())) / 100;
            //        Others = (double.Parse(others) * double.Parse(dr_cg.GetValue(1).ToString())) / 100;

            //        // dt.Tables[0].Rows.Add(dscmd1);
            //        //DataRow dr = callsTable.NewRow(); //Create New Row
            //        //dr["Call"] = "Legs";              // Set Column Value
            //        //callsTable.Rows.InsertAt(dr, 11)
            //        DataRow _pay = dt.NewRow();
            //        _pay["payment"] = Payment;
            //        _pay["pf"] = pf;
            //        _pay["ESIC"] = esic;
            //        _pay["LWF"] = lwf;
            //        _pay["PT"] = pt;
            //        _pay["others"] = Others;
            //      //  dt.[0].Rows.Add(_pay);
            //        dt.Rows.InsertAt(_pay, 0);
            //    }
            //    dr_cg.Dispose();
            //    dr_cg.Close();
            //    d1.con1.Close();

            MySqlCommand cmd_cg = new MySqlCommand(sql, d.con);
            cmd_cg.CommandTimeout = 300;
            MySqlDataAdapter dscmd = new MySqlDataAdapter(cmd_cg);
            DataSet ds = new DataSet();
            dscmd.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;

                if (i == 1)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=MANPOWER_BALANCE_SHEET.xls");
                }
                else if (i == 2)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=CONVEYANCE_BALANCE_SHEET.xls");
                }

                else if (i == 3)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=DRIVER_CONVEYANCE_BALANCE_SHEET.xls");
                }
                else if (i == 4)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=MATERIAL_BALANCE_SHEET.xls");
                }
                else if (i == 5)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=DEEP_CLEAN_BALANCE_SHEET.xls");
                }
                string date1 = txt_date.Text;
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate2(ListItemType.Header, ds, i, date1);
                Repeater1.ItemTemplate = new MyTemplate2(ListItemType.Item, ds, i, date1);
                Repeater1.FooterTemplate = new MyTemplate2(ListItemType.Footer, ds, i, date1);
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
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    public class MyTemplate2 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;
        int i;
        string date1;


        public MyTemplate2(ListItemType type, DataSet ds, int i, string date1)
        {
            this.type = type;
            this.ds = ds;
            this.i = i;
            this.date1 = date1;
            ctr = 0;
            //paid_days = 0;
            //rate = 0;
        }
        private string getmonth(string month)
        {
            month = (int.Parse(month)).ToString();
            if (month == "1")
            {
                return "JAN";
            }
            else if (month == "2")
            { return "FEB"; }
            else if (month == "3")
            { return "MAR"; }
            else if (month == "4")
            { return "APR"; }
            else if (month == "5")
            { return "MAY"; }
            else if (month == "6")
            { return "JUN"; }
            else if (month == "7")
            { return "JUL"; }
            else if (month == "8")
            { return "AUG"; }
            else if (month == "9")
            { return "SEP"; }
            else if (month == "10")
            { return "OCT"; }
            else if (month == "11")
            { return "NOV"; }
            else if (month == "12")
            { return "DEC"; }
            return "";

        }
        public void InstantiateIn(Control container)
        {
            switch (type)
            {

                case ListItemType.Header:

                    var current_date = date1;
                    if (i == 1)
                    {
                        lc = new LiteralControl("<table border=1><tr ><th  bgcolor=yellow colspan=13>MANPOWER BALANCE SHEET</th></tr><tr><th>SR NO.</th><th>Client Name</th><th>State Name</th><th>Invoice No</th><th>BASE AMOUNT</th><th>GST</th><th>BILLING AMOUNT </th><th>EMPLOYEE Payment</th><th>PF (EMPLOYER + EMPLOYEE)</th><th>ESIC (EMPLOYER + EMPLOYEE)</th><th>LWF</th><th>PT</th><th>OTHER(BONUS + LEAVE + GRADUITY)</th></tr> ");

                    }
                    else if (i == 2)
                    {
                        lc = new LiteralControl("<table border=1><tr ><th  bgcolor=yellow colspan=8> CONVEYANCE BALANCE SHEET  " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><tr><th>SR NO.</th><th>Client Name</th><th>State Name</th><th>Invoice No</th><th>SUB TOTAL</th><th>Total GST</th><th>GRAND TOTAL</th><th>Total Payment</th></tr> ");

                    }
                    else if (i == 3)
                    {
                        lc = new LiteralControl("<table border=1><tr ><th  bgcolor=yellow colspan=8>DRIVER CONVEYANCE BALANCE SHEET  " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><tr><th>SR NO.</th><th>Client Name</th><th>State Name</th><th>Invoice No</th><th>SUB TOTAL</th><th>Total GST</th><th>GRAND TOTAL</th><th>Total Payment</th></tr> ");

                    }
                    else if (i == 4)
                    {
                        lc = new LiteralControl("<table border=1><tr ><th  bgcolor=yellow colspan=8>MATERIAL BALANCE SHEET  " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><tr><th>SR NO.</th><th>Client Name</th><th>State Name</th><th>Invoice No</th><th>SUB TOTAL</th><th>Total GST</th><th>GRAND TOTAL</th><th>Total Payment</th></tr> ");

                    }
                    else if (i == 5)
                    {
                        lc = new LiteralControl("<table border=1><tr ><th  bgcolor=yellow colspan=8>DEEP CLEAN BALANCE SHEET  " + getmonth(current_date.Substring(0, 2)) + " " + current_date.Substring(3) + "</th></tr><tr><th>SR NO.</th><th>Client Name</th><th>State Name</th><th>Invoice No</th><th>SUB TOTAL</th><th>Total GST</th><th>GRAND TOTAL</th><th>Total Payment</th></tr> ");

                    }
                    break;
                case ListItemType.Item:
                    if (i == 1)
                    {

                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr][0].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][1].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][2].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][3].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][4].ToString().ToUpper() + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr][3].ToString()) + double.Parse(ds.Tables[0].Rows[ctr][4].ToString()), 2) + "</td><td>" + ds.Tables[0].Rows[ctr][5].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][6].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][7].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][8].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][9].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][10].ToString().ToUpper() + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 4>Total</td><td>=ROUND(SUM(E3:E" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(F3:F" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(H3:H" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(I3:I" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td></b></tr>";
                        }
                    }
                    else if (i == 2)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grand_total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["payment"] + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 4>Total</td><td>=ROUND(SUM(E3:E" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(F3:F" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(H3:H" + (ctr + 3) + "),2)</td></b></tr>";
                        }
                    }
                    else if (i == 3)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grand_total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["payment"] + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 4>Total</td><td>=ROUND(SUM(E3:E" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(F3:F" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(H3:H" + (ctr + 3) + "),2)</td></b></tr>";
                        }
                    }
                    else if (i == 4)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grand_total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["payment"] + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 4>Total</td><td>=ROUND(SUM(E3:E" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(F3:F" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(H3:H" + (ctr + 3) + "),2)</td></b></tr>";
                        }
                    }
                    else if (i == 5)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grand_total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["payment"] + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 4>Total</td><td>=ROUND(SUM(E3:E" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(F3:F" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(H3:H" + (ctr + 3) + "),2)</td></b></tr>";
                        }
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
    protected void btn_report_Click(object sender, EventArgs e)
    {
        try
        {
            string query = "";
            string from_date="'"+txt_payment_date_from.Text+"'";
            string to_date = "'" + txt_payment_date_to.Text + "'";
            string type1 = "" + ddl_type.SelectedValue + "";

            string from_day = "" + txt_payment_date_from.Text.Substring(0, 2) + "";
            string from_month = "" + txt_payment_date_from.Text.Substring(3, 2) + "";
            string from_year = "" + txt_payment_date_from.Text.Substring(6) + "";

            string to_day = "" + txt_payment_date_to.Text.Substring(0, 2) + "";
            string to_month = "" + txt_payment_date_to.Text.Substring(3, 2) + "";
            string to_year = "" + txt_payment_date_to.Text.Substring(6) + "";
            string where = "";
            string where1 = "";
            string where2 = "";
            string where3 = "";
            string where4 = "";

            if (ddl_type.SelectedValue == "2")
            {
                if (ddl_type.SelectedValue == "ALL")
                {
                    where = "AND vendor_code = '" + ddl_type_client.SelectedValue + "'";
                }
                query = "SELECT vendor_id,purch_invoice_no,vendor_invoice_no,ROUND(grand_total) as 'Amount',date_format(date,'%d/%m/%Y') as 'DATE'," + from_date + " as from_date," + to_date + " as 'to_date',pay_emp_paypro.pay_pro_no,paypro_batch_id,bank FROM pay_pro_vendor INNER JOIN  pay_transactionp ON pay_transactionp.comp_code = pay_pro_vendor.comp_code AND pay_transactionp.DOC_NO = pay_pro_vendor.purch_invoice_no    INNER JOIN pay_emp_paypro ON pay_pro_vendor.purch_invoice_no = pay_emp_paypro.emp_code  AND pay_pro_vendor.comp_code = pay_emp_paypro.comp_code WHERE pay_pro_vendor.comp_code = 'C01' AND date BETWEEN ('" + txt_payment_date_from.Text + "') AND ('" + txt_payment_date_to.Text + "') AND pay_pro_vendor.payment_status = 1  AND paypro_batch_id is not null "+where+" GROUP BY purch_invoice_no";
            }
            else if (ddl_type.SelectedValue == "1")
            {
                if (ddl_type_client.SelectedValue != "ALL")
                {
                     where1 = "AND pay_pro_master.client_code = '" + ddl_type_client.SelectedValue + "'";
                     where2 = " AND pay_pro_material_history.client_code = '" + ddl_type_client.SelectedValue + "'";
                     where3 = " AND pay_pro_material_history.client_code = '" + ddl_type_client.SelectedValue + "'";
                     where4 = " AND pay_pro_material_history.client_code = '" + ddl_type_client.SelectedValue + "'";
                }
                query = "SELECT  pay_report_gst.client_name, pay_report_gst.type, pay_report_gst.invoice_no, count(pay_billing_unit_rate_history.emp_code)AS 'billing_emp_count', COUNT(pay_pro_master.emp_code) AS 'paid_emp_count', ((count(pay_billing_unit_rate_history.emp_code)) - (COUNT(pay_pro_master.emp_code))) AS 'unpaid_emp_count', round(pay_report_gst.amount,2) AS 'bill_total', round( (pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst),2) AS 'gst', round((pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst),2) AS 'grand_total',ROUND( (sum(pay_pro_master.Payment-(fine+EMP_ADVANCE_PAYMENT+emp_advance+reliver_advances + absent_attendance_total)))) AS 'Paid_Amount',date_format(salary_date,'%d-%m-%Y') as 'Paid_date'," + from_date + " as from_date," + to_date + " as 'to_date',paypro_batch_id ,(SELECT DISTINCT bank FROM pay_client_master INNER JOIN  pay_pro_master ON pay_client_master.comp_code = pay_pro_master.comp_code AND pay_pro_master.client_code = pay_client_master.client_code where pay_pro_master.client_code = '" + ddl_type_client.SelectedValue + "') as 'bank' FROM pay_report_gst INNER JOIN  pay_billing_unit_rate_history ON pay_report_gst.client_code = pay_billing_unit_rate_history.client_code AND pay_billing_unit_rate_history.comp_code = pay_report_gst.comp_code  AND pay_billing_unit_rate_history.invoice_no = pay_report_gst.invoice_no INNER JOIN pay_pro_master ON pay_billing_unit_rate_history.client_code = pay_pro_master.client_code  AND pay_pro_master.comp_code = pay_billing_unit_rate_history.comp_code  AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year   AND pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code  WHERE payment_status = 1 AND pay_pro_master.comp_code = '" + Session["COMP_CODE"].ToString() + "'" + where1 + " AND pay_report_gst.type = 'manpower' and salary_date BETWEEN ('" + from_year + "-" + from_month + "-" + from_day + "')  AND ('" + to_year + "-" + to_month + "-" + to_day + "')  GROUP BY pay_report_gst.invoice_no,paypro_batch_id UNION SELECT  pay_report_gst.client_name, pay_report_gst.type, pay_report_gst.invoice_no, count(pay_billing_material_history.emp_code)AS 'billing_emp_count', COUNT(pay_pro_material_history.emp_code) AS 'paid_emp_count', ((count(pay_billing_material_history.emp_code)) - (COUNT(pay_pro_material_history.emp_code))) AS 'unpaid_emp_count', round(pay_report_gst.amount,2) AS 'bill_total', round( (pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst),2) AS 'gst', round((pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst),2) AS 'grand_total',ROUND(sum(conveyance_amount-emp_con_deduction)) AS 'Paid_Amount', DATE_FORMAT(pay_pro_material_history.payment_date, '%d-%m-%Y') AS 'Paid_date'," + from_date + " as from_date," + to_date + " as 'to_date',paypro_batch_id,(SELECT DISTINCT bank FROM pay_client_master INNER JOIN  pay_pro_material_history ON pay_client_master.comp_code = pay_pro_material_history.comp_code AND pay_pro_material_history.client_code = pay_client_master.client_code where pay_pro_material_history.client_code = '" + ddl_type_client.SelectedValue + "') as 'bank' FROM pay_report_gst INNER JOIN pay_billing_material_history ON pay_report_gst.client_code = pay_report_gst.client_code  AND pay_billing_material_history.comp_code = pay_report_gst.comp_code AND pay_billing_material_history.month = pay_report_gst.month AND pay_billing_material_history.year = pay_report_gst.year AND pay_billing_material_history.invoice_no = pay_report_gst.invoice_no INNER JOIN   pay_pro_material_history ON pay_billing_material_history.client_code = pay_pro_material_history.client_code AND pay_pro_material_history.comp_code = pay_billing_material_history.comp_code AND pay_pro_material_history.month = pay_billing_material_history.month AND pay_pro_material_history.year = pay_billing_material_history.year AND pay_pro_material_history.emp_code = pay_billing_material_history.emp_code  WHERE payment_status = 1 AND pay_pro_material_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' " + where2 + "  AND pay_pro_material_history.type = 'Conveyance' AND pay_report_gst.type = 'conveyance' AND pay_pro_material_history.conveyance_type != 100  and pay_pro_material_history.payment_date BETWEEN str_to_date('" + txt_payment_date_from.Text + "','%d/%m/%Y')  AND str_to_date('" + txt_payment_date_to.Text + "','%d/%m/%Y') GROUP BY pay_report_gst.invoice_no,paypro_batch_id UNION SELECT  pay_report_gst.client_name, pay_report_gst.type, pay_report_gst.invoice_no, count(pay_billing_material_history.emp_code)AS 'billing_emp_count', COUNT(pay_pro_material_history.emp_code) AS 'paid_emp_count', ((count(pay_billing_material_history.emp_code)) - (COUNT(pay_pro_material_history.emp_code))) AS 'unpaid_emp_count', round(pay_report_gst.amount,2) AS 'bill_total', round( (pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst),2) AS 'gst', round((pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst),2) AS 'grand_total', ROUND(SUM(conveyance_amount-driver_con_deduction)) AS 'Paid Amount', DATE_FORMAT(pay_pro_material_history.payment_date, '%d-%m-%Y') AS 'Paid_date'," + from_date + " as from_date," + to_date + " as 'to_date', paypro_batch_id,(SELECT DISTINCT bank FROM pay_client_master INNER JOIN  pay_pro_material_history ON pay_client_master.comp_code = pay_pro_material_history.comp_code AND pay_pro_material_history.client_code = pay_client_master.client_code where pay_pro_material_history.client_code = '" + ddl_type_client.SelectedValue + "') as 'bank' FROM pay_report_gst INNER JOIN pay_billing_material_history ON pay_report_gst.client_code = pay_report_gst.client_code AND pay_billing_material_history.comp_code = pay_report_gst.comp_code AND pay_billing_material_history.month = pay_report_gst.month AND pay_billing_material_history.year = pay_report_gst.year AND pay_billing_material_history.invoice_no = pay_report_gst.invoice_no INNER JOIN   pay_pro_material_history ON pay_billing_material_history.client_code = pay_pro_material_history.client_code  AND pay_pro_material_history.comp_code = pay_billing_material_history.comp_code AND pay_pro_material_history.month = pay_billing_material_history.month AND pay_pro_material_history.year = pay_billing_material_history.year  AND pay_pro_material_history.emp_code = pay_billing_material_history.emp_code  WHERE payment_status = 1 AND pay_pro_material_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' " + where3 + "  AND pay_pro_material_history.type = 'Conveyance' AND pay_report_gst.type = 'driver_conveyance' AND pay_pro_material_history.conveyance_type = 100  and pay_pro_material_history.payment_date BETWEEN str_to_date('" + txt_payment_date_from.Text + "','%d/%m/%Y') AND str_to_date('" + txt_payment_date_to.Text + "','%d/%m/%Y')  GROUP BY pay_report_gst.invoice_no,paypro_batch_id UNION SELECT  pay_report_gst.client_name, pay_report_gst.type, pay_report_gst.invoice_no, count(pay_billing_material_history.emp_code)AS 'billing_emp_count', COUNT(pay_pro_material_history.emp_code) AS 'paid_emp_count', ((count(pay_billing_material_history.emp_code)) - (COUNT(pay_pro_material_history.emp_code))) AS 'unpaid_emp_count', round(pay_report_gst.amount,2) AS 'bill_total', round( (pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst),2) AS 'gst', round((pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst),2) AS 'grand_total',(sum(material_amount- material_deduction)) as 'Paid Amount', DATE_FORMAT(pay_pro_material_history.payment_date, '%d-%m-%Y') AS 'Paid_date'," + from_date + " as from_date," + to_date + " as 'to_date',paypro_batch_id,(SELECT DISTINCT bank FROM pay_client_master INNER JOIN  pay_pro_material_history ON pay_client_master.comp_code = pay_pro_material_history.comp_code AND pay_pro_material_history.client_code = pay_client_master.client_code where pay_pro_material_history.client_code = '" + ddl_type_client.SelectedValue + "') as 'bank' FROM pay_report_gst INNER JOIN pay_billing_material_history ON pay_report_gst.client_code = pay_report_gst.client_code AND pay_billing_material_history.comp_code = pay_report_gst.comp_code AND pay_billing_material_history.month = pay_report_gst.month AND pay_billing_material_history.year = pay_report_gst.year AND pay_billing_material_history.invoice_no = pay_report_gst.invoice_no INNER JOIN  pay_pro_material_history ON pay_billing_material_history.client_code = pay_pro_material_history.client_code AND pay_pro_material_history.comp_code = pay_billing_material_history.comp_code AND pay_pro_material_history.month = pay_billing_material_history.month AND pay_pro_material_history.year = pay_billing_material_history.year AND pay_pro_material_history.emp_code = pay_billing_material_history.emp_code  WHERE payment_status = 1 AND pay_pro_material_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' " + where4 + " AND pay_billing_material_history.type = 'Material' AND pay_report_gst.type = 'material' AND pay_pro_material_history.conveyance_type != 100 and pay_pro_material_history.payment_date BETWEEN str_to_date('" + txt_payment_date_from.Text + "','%d/%m/%Y') AND str_to_date('" + txt_payment_date_to.Text + "','%d/%m/%Y') GROUP BY pay_report_gst.invoice_no,paypro_batch_id UNION SELECT  pay_report_gst.client_name, pay_report_gst.type, pay_report_gst.invoice_no, count(pay_billing_material_history.emp_code)AS 'billing_emp_count', COUNT(pay_pro_material_history.emp_code) AS 'paid_emp_count', ((count(pay_billing_material_history.emp_code)) - (COUNT(pay_pro_material_history.emp_code))) AS 'unpaid_emp_count', round(pay_report_gst.amount,2) AS 'bill_total', round( (pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst),2) AS 'gst', round((pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst),2) AS 'grand_total',round(SUM(conveyance_amount)) AS 'Paid Amount', DATE_FORMAT(pay_pro_material_history.payment_date, '%d-%m-%Y') AS 'Paid_date'," + from_date + " as from_date," + to_date + " as 'to_date',paypro_batch_id,(SELECT DISTINCT bank FROM pay_client_master INNER JOIN  pay_pro_material_history ON pay_client_master.comp_code = pay_pro_material_history.comp_code AND pay_pro_material_history.client_code = pay_client_master.client_code where pay_pro_material_history.client_code = '" + ddl_type_client.SelectedValue + "') as 'bank' FROM pay_report_gst         INNER JOIN pay_billing_material_history ON pay_report_gst.client_code = pay_report_gst.client_code AND pay_billing_material_history.comp_code = pay_report_gst.comp_code AND pay_billing_material_history.month = pay_report_gst.month AND pay_billing_material_history.year = pay_report_gst.year AND pay_billing_material_history.invoice_no = pay_report_gst.invoice_no INNER JOIN pay_pro_material_history ON pay_billing_material_history.client_code = pay_pro_material_history.client_code AND pay_pro_material_history.comp_code = pay_billing_material_history.comp_code AND pay_pro_material_history.month = pay_billing_material_history.month AND pay_pro_material_history.year = pay_billing_material_history.year AND pay_pro_material_history.emp_code = pay_billing_material_history.emp_code  WHERE payment_status = 1 AND pay_pro_material_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' " + where4 + " AND pay_report_gst.type = 'deepclean' AND pay_pro_material_history.conveyance_type = 100 GROUP BY pay_report_gst.invoice_no,paypro_batch_id";
            }
            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();

            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);


            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                if (ddl_type.SelectedValue == "1")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=Client_Paid_Report" + ".xls");
                }
                else
                {
                    Response.AddHeader("content-disposition", "attachment;filename=Vendor_Paid_Report" + ".xls");
                }
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate10(ListItemType.Header, ds, type1);
                Repeater1.ItemTemplate = new MyTemplate10(ListItemType.Item, ds, type1);
                Repeater1.FooterTemplate = new MyTemplate10(ListItemType.Footer, null, type1);
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
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    public class MyTemplate10 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;
        string type1;

        public MyTemplate10(ListItemType type, DataSet ds, string type1)
        {
            this.type = type;
            this.ds = ds;
            ctr = 0;
            this.type1 = type1;

        }

        public void InstantiateIn(Control container)
        {


            switch (type)
            {
                case ListItemType.Header:
                    if (type1 == "2")
                    {
                        lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=11>VENDOR PAID REPORT</th></tr><tr><th>SR NO.</th><th>Vendor Name</th><th>Invoice No</th><th>Vendor Invoice No</th><th>Paid Amount</th><th>Paid Date</th><th>From Date</th><th>To Date</th><th>CRN Number</th><th>Batch No</th><th>Bank Name</th></tr> "); 
                    }
                    else if (type1 == "1")
                    {
                        lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=16>CLIENT PAID REPORT</th></tr><tr><th>SR NO.</th><th>Client Name</th><th>Type Of Bill</th><th>Invoice No</th><th>Billing Employee Count</th><th>Paid Employee Count</th><th>Unpaid Employee Count</th><th>Bill Total</th><th>GST</th><th>Grand Total</th><th>Paid Amount</th><th>Paid Date</th><th>From Date</th><th>To Date</th><th>Batch No</th><th>Bank Name</th></tr> "); 
                    }
                    break;
                case ListItemType.Item:
                    if (type1 == "2")
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["vendor_id"] + "</td><td>" + ds.Tables[0].Rows[ctr]["purch_invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["vendor_invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["DATE"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["from_date"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["to_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["pay_pro_no"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["paypro_batch_id"] + "</td><td>" + ds.Tables[0].Rows[ctr]["bank"] + "</td></tr>");
                    }
                    else if (type1 == "1")
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["type"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["billing_emp_count"] + "</td><td>" + ds.Tables[0].Rows[ctr]["paid_emp_count"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unpaid_emp_count"] + "</td><td>" + ds.Tables[0].Rows[ctr]["bill_total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grand_total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Paid_Amount"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["Paid_date"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["from_date"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["to_date"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["paypro_batch_id"] + "</td><td>" + ds.Tables[0].Rows[ctr]["bank"] + "</td></tr>");
                    }
                         //if (ds.Tables[0].Rows.Count == ctr + 1)
                    //{
                    //    lc.Text = lc.Text + "<tr><b><td align=center colspan = 9>Total</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td></b></tr>";
                    //}

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
    protected void ddl_type_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "7";
        ddl_client.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = null;
        if (ddl_type.SelectedValue == "1")
        {
            cmd_item = new MySqlDataAdapter("Select CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', client_code from pay_client_master where comp_code='" + Session["comp_code"].ToString() + "' and client_active_close='0'  ORDER BY client_code", d.con);
        }
        else if (ddl_type.SelectedValue == "2")
        {
            cmd_item = new MySqlDataAdapter("SELECT cust_name,cust_code FROM pay_transactionp where comp_code='" + Session["comp_code"].ToString() + "'  group by cust_code ORDER BY cust_code", d.con);
        }
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_type_client.DataSource = dt_item;
                ddl_type_client.DataTextField = dt_item.Columns[0].ToString();
                ddl_type_client.DataValueField = dt_item.Columns[1].ToString();
                ddl_type_client.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
           
            ddl_type_client.Items.Insert(0, "Select");
            ddl_type_client.Items.Insert(1, "ALL");
            d.con.Close();
        }
    }
    protected void ddl_type_tally_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "8";
        //ddl_client.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = null;
        if (ddl_type_tally.SelectedValue == "1")
        {
            cmd_item = new MySqlDataAdapter("Select CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', client_code from pay_client_master where comp_code='" + Session["comp_code"].ToString() + "' and client_active_close='0'  ORDER BY client_code", d.con);
        }
        else if (ddl_type_tally.SelectedValue == "2")
        {
            cmd_item = new MySqlDataAdapter("SELECT cust_name,cust_code FROM pay_transactionp where comp_code='" + Session["comp_code"].ToString() + "'  group by cust_code ORDER BY cust_code", d.con);
        }
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_select_type.DataSource = dt_item;
                ddl_select_type.DataTextField = dt_item.Columns[0].ToString();
                ddl_select_type.DataValueField = dt_item.Columns[1].ToString();
                ddl_select_type.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
        }
        catch (Exception ex) { throw ex; }
        finally
        {

            ddl_select_type.Items.Insert(0, "Select");
            ddl_select_type.Items.Insert(1, "ALL");
            d.con.Close();
        }
    }
    protected void btn_get_report_Click(object sender, EventArgs e)
    {
        try
        {
            string query = "";
            string type1 = "" + ddl_type_tally.SelectedValue + "";
            string where = "";
            if (ddl_select_type.SelectedValue == "ALL")
            {
                where = "";
            }
            else
            {
                where = "and pay_transactionp.CUST_CODE='" + ddl_select_type.SelectedValue + "'";
            }
            if (type1 == "1")
            {
               // query = "select  date_format(invoice_date,'%d-%m-%Y') as invoice_date,'' as Voucher_Type,'' as Standard_Narration,'' as voucher_no,client_name,(pay_report_gst.amount+pay_report_gst.cgst+pay_report_gst.sgst+pay_report_gst.igst) as 'Amount_gst' ,pay_report_gst.invoice_no,'',pay_report_gst.type,pay_report_gst.amount,'','','','','',pay_report_gst.cgst,'',pay_report_gst.sgst,'',pay_report_gst.igst,pay_report_gst.state_name,pay_report_gst.month,pay_report_gst.year, (equmental_rental_rate+chemical_consumables_rate+dustbin_liners_rate+femina_hygiene_rate+aerosol_dispenser_rate) as 'material_amount'  from pay_report_gst left join  pay_billing_unit_rate_history on pay_report_gst.invoice_no=pay_billing_unit_rate_history.auto_invoice_no and pay_report_gst.comp_code = pay_billing_unit_rate_history.comp_code where pay_report_gst.client_code='" + ddl_select_type.SelectedValue + "'  and invoice_date  BETWEEN STR_TO_DATE('" + txt_tally_from_date.Text + "', '%d/%m/%Y') AND STR_TO_DATE('" + txt_tally_to_date.Text + "', '%d/%m/%Y')  group by pay_report_gst.invoice_no";
                query = "select  date_format(invoice_date,'%d-%m-%Y') as invoice_date,'' as Voucher_Type,'' as Standard_Narration,'' as voucher_no,client_name,(pay_report_gst.amount+pay_report_gst.cgst+pay_report_gst.sgst+pay_report_gst.igst) as 'Amount_gst' ,pay_report_gst.invoice_no,'',pay_report_gst.type,pay_report_gst.amount,'','','','','',pay_report_gst.cgst,'',pay_report_gst.sgst,'',pay_report_gst.igst,pay_report_gst.state_name,pay_report_gst.month,pay_report_gst.year, '' as 'material_amount'  from pay_report_gst where pay_report_gst.client_code='" + ddl_select_type.SelectedValue + "'  and invoice_date  BETWEEN STR_TO_DATE('" + txt_tally_from_date.Text + "', '%d/%m/%Y') AND STR_TO_DATE('" + txt_tally_to_date.Text + "', '%d/%m/%Y')  group by pay_report_gst.invoice_no";
            }
            else if (type1 == "2")
            {
                query = "select distinct DOC_NO,pur_order_no,date_format(booking_date,'%d-%m-%Y') as 'booking_date',date_format(DOC_DATE,'%d-%m-%Y') as 'DOC_DATE',vendor_invoice_no,pay_transactionp.CUST_CODE,CUST_NAME,ROUND(payable_amount, 2) as 'payable_amount' ,ROUND(FINAL_PRICE, 2) as 'FINAL_PRICE',ROUND(GROSS_AMOUNT, 2) as 'GROSS_AMOUNT',round(igst,2) as 'igst',round(cgst,2) as cgst,round(sgst,2) as 'sgst',tax_code,round(tds_amount,2) as tds_amount,NARRATION,item_type from pay_transactionp inner join pay_transaction_po_details on pay_transactionp.comp_code=pay_transaction_po_details.COMP_CODE and pay_transactionp.CUST_CODE=pay_transaction_po_details.CUST_CODE where pay_transactionp.COMP_CODE='" + Session["comp_code"].ToString() + "' " + where + " and final_invoice_flag = '2' and booking_date  BETWEEN STR_TO_DATE('" + txt_tally_from_date.Text + "', '%d/%m/%Y') AND STR_TO_DATE('" + txt_tally_to_date.Text + "', '%d/%m/%Y') "; 
            }
            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();
            dscmd.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                if (ddl_type_tally.SelectedValue == "1")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=Client_Tally_Report" + ".xls");
                }
                else
                {
                    Response.AddHeader("content-disposition", "attachment;filename=Vendor_Tally_Report" + ".xls");
                }
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate01(ListItemType.Header, ds, type1);
                Repeater1.ItemTemplate = new MyTemplate01(ListItemType.Item, ds, type1);
                Repeater1.FooterTemplate = new MyTemplate01(ListItemType.Footer, null, type1);
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
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    public class MyTemplate01 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;
        string type1;

        public MyTemplate01(ListItemType type, DataSet ds, string type1)
        {
            this.type = type;
            this.ds = ds;
            ctr = 0;
            this.type1 = type1;

        }

        public void InstantiateIn(Control container)
        {


            switch (type)
            {
                case ListItemType.Header:
                    //for vendor
                    if (type1 == "2")
                    {
                        lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=18>VENDOR TALLY REPORT</th></tr><tr><th>SR NO.</th><th>Bill No</th><th>PO No</th><th>Booked Date</th><th>Vendor Invoice Date</th><th>Vendor Invoice No</th><th>Vendor Code</th><th>Name Of Vendor</th><th>Purchase/Exp Type</th><th>Final PO Amount</th><th>Total Amount(Inc GST)</th><th>Gross Amount</th><th>IGST Amount</th><th>CGST Amount</th><th>SGST Amount</th><th>TDS Code</th><th>TDS Amount Deducted</th><th>Narration</th></tr> ");
                    }
                    //client
                    else if (type1 == "1")
                    {
                        lc = new LiteralControl("<table border=1><tr ></tr><tr><th>SR NO.</th><th bgcolor=DeepSkyBlue>DATE</th><th bgcolor=LightBlue>VOUCHER TYPE</th><th bgcolor=DeepSkyBlue>STANDARD NARRATION</th><th bgcolor=LightBlue>VOUCHER NO</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-1</th><th bgcolor=IndianRed>AMOUNT-1</th><th bgcolor=DeepSkyBlue>REFERANCE NUMBER</th><th bgcolor=DeepSkyBlue>REFERANCE DUE DAYS</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-2</th><th bgcolor=IndianRed>AMOUNT-2</th><th bgcolor=DeepSkyBlue>STOCK ITEM NAME</th><th bgcolor=SkyBlue>STOCK ITEM QTY</th><th bgcolor=DeepSkyBlue>STOCK ITEM RATE</th><th bgcolor=DeepSkyBlue>STOCK ITEM TOTAL AMT</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-3</th><th bgcolor=IndianRed>AMOUNT-3</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-4</th><th bgcolor=LightCoral>AMOUNT-4</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-5</th><th bgcolor=IndianRed>AMOUNT-5</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-6</th><th bgcolor=LightCoral>AMOUNT-6</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-7</th><th bgcolor=IndianRed>AMOUNT-7</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-8</th><th bgcolor=LightCoral>AMOUNT-8</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-9</th><th bgcolor=IndianRed>AMOUNT-9</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-10</th><th bgcolor=LightCoral>AMOUNT-10</th><th bgcolor=LightSkyBlue>LEDGER NAME  CR/CR-11</th><th bgcolor=LightSkyBlue>AMOUNT-11</th><th bgcolor=LightSkyBlue>LEDGER NAME  CR/CR-12</th><th bgcolor=LightSkyBlue>AMOUNT-12</th><th bgcolor=YellowGreen>BRANCH/STATE</th></tr> ");
                    }
                    break;
                case ListItemType.Item:
                    if (type1 == "2")
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["DOC_NO"] + "</td><td>" + ds.Tables[0].Rows[ctr]["pur_order_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["booking_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["DOC_DATE"] + "</td><td>" + ds.Tables[0].Rows[ctr]["vendor_invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["CUST_CODE"] + "</td><td>" + ds.Tables[0].Rows[ctr]["CUST_NAME"] + "</td><td>" + ds.Tables[0].Rows[ctr]["item_type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["payable_amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["FINAL_PRICE"] + "</td><td>" + ds.Tables[0].Rows[ctr]["GROSS_AMOUNT"] + "</td><td>" + ds.Tables[0].Rows[ctr]["igst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["tax_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["tds_amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["NARRATION"] + "</td></tr>");
                    }
                    else if (type1 == "1")
                    {
                        DateTimeFormatInfo mfi = new DateTimeFormatInfo();
                        string month ="" + ds.Tables[0].Rows[ctr]["month"] + "";
                        string year = "" + ds.Tables[0].Rows[ctr]["year"] + "";
                        string month_name = mfi.GetMonthName(int.Parse("" + month + "")).ToString();
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_date"] + "</td><td>Sales</td><td>Being Sale of Services for the Month of " + month_name + " " + year + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Amount_gst"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td></td><td></td><td></td><td></td><td>CGST@9%</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>SGST@9%</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td><td>IGST@18%</td><td>" + ds.Tables[0].Rows[ctr]["igst"] + "</td><td>Material</td><td>" + ds.Tables[0].Rows[ctr]["material_amount"] + "</td><td>convenyence</td><td>0</td><td>Deep Cleaning</td><td>0</td><td>Rental</td><td>0</td><td>Arreas</td><td>0</td><td>R&M</td><td>0</td><td>Administrative</td><td>0</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td></tr>");
                    }
                    //if (ds.Tables[0].Rows.Count == ctr + 1)
                    //{
                    //    lc.Text = lc.Text + "<tr><b><td align=center colspan = 9>Total</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td></b></tr>";
                    //}

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
    protected void btn_sac_wise_gst_report_Click(object sender, EventArgs e)
    {
        //counter = 2;
        if (ddl_gst_type.SelectedValue == "ALL")
        {
            all_gst_report("ALL", 2);
        }
        else if (ddl_gst_type.SelectedValue == "1")
        {
            all_gst_report("manpower", 2);
        }
        else if (ddl_gst_type.SelectedValue == "2")
        {
            all_gst_report("conveyance", 2);
        }
        else if (ddl_gst_type.SelectedValue == "3")
        {
            all_gst_report("driver_conveyance", 2);
        }
        else if (ddl_gst_type.SelectedValue == "4")
        {
            all_gst_report("material", 2);
        }
        else if (ddl_gst_type.SelectedValue == "5")
        {
            all_gst_report("deepclean", 2);
        }
        else if (ddl_gst_type.SelectedValue == "6")
        {
            all_gst_report("machine_rental", 2);
        }
        else if (ddl_gst_type.SelectedValue == "7")
        {
            all_gst_report("arrears_manpower", 2);
        }
        else if (ddl_gst_type.SelectedValue == "8")
        {
            all_gst_report("manual", 2);
        }
    }
    protected void btn_r_m_tally_report_Click(object sender, EventArgs e)
    {
        try
        {
        string where = "";
        string query = "";
        if (ddl_client.SelectedValue != "ALL")
        {
            where = "  and  client_code='" + ddl_client.SelectedValue + "' ";

        }
        else if (ddl_state.SelectedValue != "ALL")
        {
            where = where + " and state_name ='" + ddl_state.SelectedValue + "'";

        }
        query = "SELECT  date_format(vendor_invoice_date,'%d/%m/%Y') as 'vendor_invoice_date',discription,gst_no,party_name,gross_amount,vendor_invoice_no,amount, CASE WHEN vendor_igst != 0 THEN -vendor_igst else 0 END AS 'vendor_igst',CASE WHEN vendor_cgst != 0 THEN -vendor_cgst else 0 END AS 'vendor_cgst',CASE WHEN vendor_sgst != 0 THEN -vendor_sgst else 0 END AS 'vendor_sgst'  FROM pay_r_and_m_service where vendor_invoice_date between STR_TO_DATE('" + gst_from_date.Text + "', '%d/%m/%Y') AND STR_TO_DATE('" + gst_to_date.Text + "', '%d/%m/%Y') " + where + "";
        MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();
            dscmd.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                
                    Response.AddHeader("content-disposition", "attachment;filename=R&M_Tally_Report" + ".xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate02(ListItemType.Header, ds);
                Repeater1.ItemTemplate = new MyTemplate02(ListItemType.Item, ds);
                Repeater1.FooterTemplate = new MyTemplate02(ListItemType.Footer, null);
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
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('This Report is only for R&M');", true);
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    public class MyTemplate02 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;
        string type1;

        public MyTemplate02(ListItemType type, DataSet ds)
        {
            this.type = type;
            this.ds = ds;
            ctr = 0;
            

        }

        public void InstantiateIn(Control container)
        {


            switch (type)
            {
                case ListItemType.Header:
                   
                    //client
                   
                        lc = new LiteralControl("<table border=1><tr ></tr><tr><th>SR NO.</th><th bgcolor=DeepSkyBlue>DATE</th><th bgcolor=LightBlue>VOUCHER TYPE</th><th bgcolor=DeepSkyBlue>STANDARD NARRATION</th><th bgcolor=LightBlue>VOUCHER NO</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-1</th><th bgcolor=IndianRed>AMOUNT-1</th><th bgcolor=DeepSkyBlue>REFERANCE NUMBER</th><th bgcolor=DeepSkyBlue>REFERANCE DUE DAYS</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-2</th><th bgcolor=IndianRed>AMOUNT-2</th><th bgcolor=DeepSkyBlue>STOCK ITEM NAME</th><th bgcolor=SkyBlue>STOCK ITEM QTY</th><th bgcolor=DeepSkyBlue>STOCK ITEM RATE</th><th bgcolor=DeepSkyBlue>STOCK ITEM TOTAL AMT</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-3</th><th bgcolor=IndianRed>AMOUNT-3</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-4</th><th bgcolor=LightCoral>AMOUNT-4</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-5</th><th bgcolor=IndianRed>AMOUNT-5</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-6</th><th bgcolor=LightCoral>AMOUNT-6</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-7</th><th bgcolor=IndianRed>AMOUNT-7</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-8</th><th bgcolor=LightCoral>AMOUNT-8</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-9</th><th bgcolor=IndianRed>AMOUNT-9</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-10</th><th bgcolor=LightCoral>AMOUNT-10</th></tr> ");
                    
                    break;
                case ListItemType.Item:
                    DateTimeFormatInfo mfi = new DateTimeFormatInfo();
                    //string month = "" + ds.Tables[0].Rows[ctr]["month"] + "";
                    //string year = "" + ds.Tables[0].Rows[ctr]["year"] + "";
                    //string month_name = mfi.GetMonthName(int.Parse("" + month + "")).ToString();
                    lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["vendor_invoice_date"] + "</td><td>Journel</td><td>" + ds.Tables[0].Rows[ctr]["discription"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["party_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["vendor_invoice_no"] + "</td><td></td><td>R&M Expenses_Reimbersment</td><td>-" + ds.Tables[0].Rows[ctr]["gross_amount"] + "</td><td></td><td></td><td></td><td></td><td>CGST</td><td>" + ds.Tables[0].Rows[ctr]["vendor_cgst"] + "</td><td>SGST</td><td>" + ds.Tables[0].Rows[ctr]["vendor_sgst"] + "</td><td>IGST</td><td>" + ds.Tables[0].Rows[ctr]["vendor_igst"] + "</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>");

                    //if (ds.Tables[0].Rows.Count == ctr + 1)
                    //{
                    //    lc.Text = lc.Text + "<tr><b><td align=center colspan = 9>Total</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td></b></tr>";
                    //}

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

    //Start Vendor Purchase GST Report

    protected void ddl_ven_type_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "9";

        string query = "";
        ddl_ven_name.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();

        if (ddl_ven_type.SelectedValue == "1")
        {
            query = "select distinct party_name from pay_r_and_m_service where comp_code='" + Session["COMP_CODE"] + "' and GST_applicable_rm=1";
        }

        if (ddl_ven_type.SelectedValue == "2")
        {
            query = "select distinct CUST_NAME from pay_transactionp where COMP_CODE='" + Session["COMP_CODE"] + "'";
        }

        MySqlDataAdapter cmd_item = new MySqlDataAdapter(query, d.con);
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_ven_name.DataSource = dt_item;
                ddl_ven_name.DataTextField = dt_item.Columns[0].ToString();
                ddl_ven_name.DataValueField = dt_item.Columns[0].ToString();
                ddl_ven_name.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
            ddl_ven_name.Items.Insert(0, "Select");
            ddl_ven_name.Items.Insert(1, "ALL");
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }

    protected void btn_ven_report_Click(object sender, EventArgs e)
    {
        hidtab.Value = "9";

        try
        {
            string query = "", ven_type = "" + ddl_ven_type.SelectedValue + "", where = "";

            if (ddl_ven_type.SelectedValue == "1")
            {
                if (ddl_ven_name.SelectedValue != "ALL" && ddl_ven_name.SelectedValue != "Select")
                {
                    where = "and party_name='" + ddl_ven_name.SelectedValue + "'";
                }

                query = "SELECT DATE_FORMAT(vendor_invoice_date, '%d/%m/%Y') AS Billing_Date,  discription AS Type,  month,	year, vendor_invoice_no, party_name, state_name, gst_no AS GST_NO, gross_amount AS Bill_Amount, vendor_cgst AS CGST, vendor_sgst AS SGST, vendor_igst AS IGST, (vendor_cgst + vendor_sgst + vendor_igst) AS Total_GST, amount AS Total_Bill, hsn_no  from pay_r_and_m_service where approve_flag=2 and vendor_invoice_date between STR_TO_DATE('" + txt_ven_from_date.Text + "', '%d/%m/%Y') and STR_TO_DATE('" + txt_ven_to_date.Text + "', '%d/%m/%Y') and gst_no is not null and GST_applicable_rm=1 " + where + " order by month,year,vendor_invoice_date";
            }

            if (ddl_ven_type.SelectedValue == "2")
            {
                if (ddl_ven_name.SelectedValue == "ALL" && ddl_ven_name.SelectedValue == "Select")
                {
                    where = "and CUST_NAME='" + ddl_ven_type.SelectedValue + "'";
                }

                query = "SELECT DATE_FORMAT(DOC_DATE, '%d/%m/%Y') AS Billing_Date, NARRATION AS Type, month, year, vendor_invoice_no , CUST_NAME AS party_name, '' as state_name, customer_gst_no AS GST_NO, GROSS_AMOUNT as Bill_Amount, cgst AS CGST, sgst AS SGST, igst AS IGST, (cgst + sgst + igst) AS Total_gst, NET_TOTAL AS Total_Bill, '' as hsn_no from pay_transactionp where DOC_DATE between STR_TO_DATE('" + txt_ven_from_date.Text + "', '%d/%m/%Y') and STR_TO_DATE('" + txt_ven_to_date.Text + "', '%d/%m/%Y') and customer_gst_no is not null " + where + " order by year,month,DOC_DATE";
            }

            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();

            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;

                Response.AddHeader("content-disposition", "attachment;filename=Vendor Purchase GST Report.xls");

                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate5(ListItemType.Header, ds);
                Repeater1.ItemTemplate = new MyTemplate5(ListItemType.Item, ds);
                Repeater1.FooterTemplate = new MyTemplate5(ListItemType.Footer, null);
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

        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }

    public class MyTemplate5 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;

        public MyTemplate5(ListItemType type, DataSet ds)
        {
            this.type = type;
            this.ds = ds;
            ctr = 0;
        }

        public void InstantiateIn(Control container)
        {
            switch (type)
            {
                case ListItemType.Header:

                    lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=16>Vendor Purchase GST Report</th></tr><tr><th>SR NO.</th><th>Invoice Date</th><th>Type</th><th>Month</th><th>Year</th><th>Invoice No</th><th>Vendor Name</th><th>State Name</th><th>GST NO</th><th>Bill Amount</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Total GST</th><th>Bill Amount</th><th>HSN NO</th></tr> ");

                    break;

                case ListItemType.Item:

                    lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["Billing_Date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["month"] + "</td><td>" + ds.Tables[0].Rows[ctr]["year"] + "</td><td>" + ds.Tables[0].Rows[ctr]["vendor_invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["party_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["GST_NO"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Bill_Amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["CGST"] + "</td><td>" + ds.Tables[0].Rows[ctr]["SGST"] + "</td><td>" + ds.Tables[0].Rows[ctr]["IGST"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Total_GST"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Total_Bill"] + "</td><td>" + ds.Tables[0].Rows[ctr]["hsn_no"] + "</td></tr>");

                    if (ds.Tables[0].Rows.Count == ctr + 1)
                    {
                        lc.Text = lc.Text + "<tr><b><td align=center colspan = 9>Total</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td></b></tr>";
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

    //END
    //Sachin Start All Invoice Download
    private bool btn_arrears_invoiceClicked = false;

    protected void btn_all_inv_download_Click(object sender, EventArgs e)
    {

        hidtab.Value = "5";
        counter = 1;
        e_invoice = 0;
        if (ddl_gst_type.SelectedValue == "ALL")
        {
            download_all_invoice("ALL", 1);
        }
        else if (ddl_gst_type.SelectedValue == "1")
        {
            download_all_invoice("manpower", 1);
        }
        else if (ddl_gst_type.SelectedValue == "2")
        {
            download_all_invoice("conveyance", 1);
        }
        else if (ddl_gst_type.SelectedValue == "3")
        {
            download_all_invoice("driver_conveyance", 1);
        }
        else if (ddl_gst_type.SelectedValue == "4")
        {
            download_all_invoice("material", 1);
        }
        else if (ddl_gst_type.SelectedValue == "5")
        {
            download_all_invoice("deepclean", 1);
        }
        else if (ddl_gst_type.SelectedValue == "6")
        {
            download_all_invoice("machine_rental", 1);
        }
        else if (ddl_gst_type.SelectedValue == "7")
        {
            download_all_invoice("arrears_manpower", 1);
        }
        else if (ddl_gst_type.SelectedValue == "8")
        {
            download_all_invoice("manual", 1);
        }
        else if (ddl_gst_type.SelectedValue == "9")
        {
            download_all_invoice("r_and_m_bill", 1);
        }
        else if (ddl_gst_type.SelectedValue == "10")
        {
            download_all_invoice("administrative_bill", 1);
        }
        else if (ddl_gst_type.SelectedValue == "11")
        {
            download_all_invoice("shiftwise_bill", 1);
        }
        else if (ddl_gst_type.SelectedValue == "12")
        {
            download_all_invoice("incentive_bill", 1);
        }
        else if (ddl_gst_type.SelectedValue == "13")
        {
            download_all_invoice("office_rent_bill", 1);
        }
    }

    protected void btn_e_invoice_Click(object sender, EventArgs e)
    {
        try
        {
            e_invoice = 1;

            hidtab.Value = "5";
            counter = 1;
            if (ddl_gst_type.SelectedValue == "ALL")
            {
                download_all_invoice("ALL", 1);
            }
            else if (ddl_gst_type.SelectedValue == "1")
            {
                download_all_invoice("manpower", 1);
            }
            else if (ddl_gst_type.SelectedValue == "2")
            {
                download_all_invoice("conveyance", 1);
            }
            else if (ddl_gst_type.SelectedValue == "3")
            {
                download_all_invoice("driver_conveyance", 1);
            }
            else if (ddl_gst_type.SelectedValue == "4")
            {
                download_all_invoice("material", 1);
            }
            else if (ddl_gst_type.SelectedValue == "5")
            {
                download_all_invoice("deepclean", 1);
            }
            else if (ddl_gst_type.SelectedValue == "6")
            {
                download_all_invoice("machine_rental", 1);
            }
            else if (ddl_gst_type.SelectedValue == "7")
            {
                download_all_invoice("arrears_manpower", 1);
            }
            else if (ddl_gst_type.SelectedValue == "8")
            {
                download_all_invoice("manual", 1);
            }
            else if (ddl_gst_type.SelectedValue == "9")
            {
                download_all_invoice("r_and_m_bill", 1);
            }
            else if (ddl_gst_type.SelectedValue == "10")
            {
                download_all_invoice("administrative_bill", 1);
            }
            else if (ddl_gst_type.SelectedValue == "11")
            {
                download_all_invoice("shiftwise_bill", 1);
            }
            else if (ddl_gst_type.SelectedValue == "12")
            {
                download_all_invoice("incentive_bill", 1);
            }
            else if (ddl_gst_type.SelectedValue == "13")
            {
                download_all_invoice("office_rent_bill", 1);
            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    string Source, Target, Certificate, Password, Author, Title, Subject, Keywords, Creator, Producer, Reason, Contact, Location;

    public void DigitalSign_invoice_print(string downloadname, string filename, string Invpath)
    {
        Certificate = @"" + Server.MapPath("~/Logs/IHMS DSC.pfx") + "";
        Password = "12345678";
        Source = downloadname;
        // string pdf_path = Server.MapPath("~/Invoice_copy/Digital_invoice/" + filename + "");
        string pdf_path = Invpath + "\\DG_" + filename;
        //string pdf_path = Invpath + "Invoice_" + filename;
        if (File.Exists(pdf_path))
        {
            File.Delete(pdf_path);
        }
        // Target = Server.MapPath("~/Invoice_copy/Digital_invoice/" + filename + "");
        Target = pdf_path;
        Reason = "To AuthenticateDocument";
        Contact = "International Housekeeping And Maintenance Services";
        Location = "Pune";
        Cert myCert = null;
        try
        {
            myCert = new Cert(Certificate, Password);
            //debug("Certificate OK");
        }
        catch (Exception ex)
        {
            //    debug("Error : please make sure you entered a valid certificate file and password");
            //    debug("Exception : "+ex.ToString());
            return;
        }
        //debug("Creating new MetaData ... ");

        //Adding Meta Datas
        MetaData MyMD = new MetaData();
        MyMD.Author = "Mr. Arun Kumar Singh";
        MyMD.Title = "Authorised Signatory";
        MyMD.Subject = "Digital Signing  Document";
        MyMD.Keywords = "International Housekeeping And Maintenance Services";
        MyMD.Creator = "Rave (http://www.nevrona.com/rave)";
        MyMD.Producer = "Nevrona Designs";


        //debug("Signing document ... ");
        PDFSigner pdfs = new PDFSigner(Source, Target, myCert, MyMD);
        string pdf_path1 = Source;

        PdfReader pdfReader = new PdfReader(pdf_path1);
        int numberOfPages = pdfReader.NumberOfPages;

        bool chk_sign = true;//not in use
        pdfs.Sign(Reason, Contact, Location, chk_sign, numberOfPages);
        // Open the result for demonstration purposes.
        //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Target)
        //{
        //    UseShellExecute = true
        //}
        //);

        // Response.ContentType = ContentType;
        //Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(Target));
        //  Response.WriteFile(Target);
        //Response.End();




        if (File.Exists(Source))
        {
            File.Delete(Source);
        }
    }

    //Cert DS
    class Cert
    {
        #region Attributes

        private string path = "";
        private string password = "";
        private AsymmetricKeyParameter akp;
        private org.bouncycastle.x509.X509Certificate[] chain;

        #endregion

        #region Accessors
        public org.bouncycastle.x509.X509Certificate[] Chain
        {
            get { return chain; }
        }
        public AsymmetricKeyParameter Akp
        {
            get { return akp; }
        }

        public string Path
        {
            get { return path; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        #endregion

        #region Helpers

        private void processCert()
        {
            string alias = null;
            PKCS12Store pk12;

            //First we'll read the certificate file
            pk12 = new PKCS12Store(new FileStream(this.Path, FileMode.Open, FileAccess.Read), this.password.ToCharArray());

            //then Iterate throught certificate entries to find the private key entry
            IEnumerator i = pk12.aliases();
            while (i.MoveNext())
            {
                alias = ((string)i.Current);
                if (pk12.isKeyEntry(alias))
                    break;
            }

            this.akp = pk12.getKey(alias).getKey();
            X509CertificateEntry[] ce = pk12.getCertificateChain(alias);
            this.chain = new org.bouncycastle.x509.X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
                chain[k] = ce[k].getCertificate();

        }
        #endregion

        #region Constructors
        public Cert()
        { }
        public Cert(string cpath)
        {
            this.path = cpath;
            this.processCert();
        }
        public Cert(string cpath, string cpassword)
        {
            this.path = cpath;
            this.Password = cpassword;
            this.processCert();
        }
        #endregion

    }
    //End Cert DS
    //Meta Data DS
    class MetaData
    {
        private Hashtable info = new Hashtable();

        public Hashtable Info
        {
            get { return info; }
            set { info = value; }
        }

        public string Author
        {
            get { return (string)info["Author"]; }
            set { info.Add("Author", value); }
        }
        public string Title
        {
            get { return (string)info["Title"]; }
            set { info.Add("Title", value); }
        }
        public string Subject
        {
            get { return (string)info["Subject"]; }
            set { info.Add("Subject", value); }
        }
        public string Keywords
        {
            get { return (string)info["Keywords"]; }
            set { info.Add("Keywords", value); }
        }
        public string Producer
        {
            get { return (string)info["Producer"]; }
            set { info.Add("Producer", value); }
        }
        public string Creator
        {
            get { return (string)info["Creator"]; }
            set { info.Add("Creator", value); }
        }
        public Hashtable getMetaData()
        {
            return this.info;
        }
        public byte[] getStreamedMetaData()
        {
            MemoryStream os = new System.IO.MemoryStream();
            XmpWriter xmp = new XmpWriter(os, this.info);
            xmp.Close();
            return os.ToArray();
        }

    }
    //END Meta Data DS
    //Digital Signer
    class PDFSigner
    {
        private string inputPDF = "";
        private string outputPDF = "";
        private Cert myCert;
        private MetaData metadata;

        public PDFSigner(string input, string output)
        {
            this.inputPDF = input;
            this.outputPDF = output;
        }

        public PDFSigner(string input, string output, Cert cert)
        {
            this.inputPDF = input;
            this.outputPDF = output;
            this.myCert = cert;
        }
        public PDFSigner(string input, string output, MetaData md)
        {
            this.inputPDF = input;
            this.outputPDF = output;
            this.metadata = md;
        }
        public PDFSigner(string input, string output, Cert cert, MetaData md)
        {
            this.inputPDF = input;
            this.outputPDF = output;
            this.myCert = cert;
            this.metadata = md;
        }

        public void Verify()
        {

        }
        public void Sign(string SigReason, string SigContact, string SigLocation, bool visible, int numberOfPages)
        {
            PdfReader reader = new PdfReader(this.inputPDF);
            //Activate MultiSignatures
            PdfStamper st = PdfStamper.CreateSignature(reader, new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write), '\0', null, true);
            //To disable Multi signatures uncomment this line : every new signature will invalidate older ones !
            //PdfStamper st = PdfStamper.CreateSignature(reader, new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write), '\0'); 

            st.MoreInfo = this.metadata.getMetaData();
            st.XmpMetadata = this.metadata.getStreamedMetaData();
            PdfSignatureAppearance sap = st.SignatureAppearance;

            sap.SetCrypto(this.myCert.Akp, this.myCert.Chain, null, PdfSignatureAppearance.WINCER_SIGNED);
            sap.Reason = SigReason;
            sap.Contact = SigContact;
            sap.Location = SigLocation;
            //if (visible)
            //sap.SetVisibleSignature(new iTextSharp.text.Rectangle(100, 100, 250, 150), 1, null);
            sap.SetVisibleSignature(new iTextSharp.text.Rectangle(80, 60, 250, 120), numberOfPages, null);
            //sap.SetAbsolutePosition(new iTextSharp.text.Rectangle(1000F,1000F));
            //  Document document = new Document(PageSize.A4, 188f, 88f, 5f, 10f);

            st.Close();
        }

    }
    // End Digital Signer




    protected void download_all_invoice(string type, int counter)
    {
        hidtab.Value = "5";
        int invoice_fl_man = 0, invoice_arrear = 0;
        string query1 = "", query = "", query2 = "";
        string invoice = "", bill_date = "", billing_name = "", grade_code = "", material_type = "", ddl_start_date_common = "", Billing_wise = "", ddl_end_date_common = "", client_code = "", region = "", ddl_arrears_type = "Select", bill_type = "", txt_arrear_monthend = "", designation = "", txt_arrear_month_year = "", month = "", year = "", client_name = "", state_name, unit_code = "", billing_process = "";
        string dowmloadname = "";
        try
        {
            string where = "";
            string invoice_flag = "";
            string billing_type = "";
            string order_by = "order by client_name";
            if (type != "ALL")
            {
                billing_type = " and G.type = '" + type + "'";
            }
            if (ddl_client.SelectedValue != "ALL")
            {
                order_by = "order by invoice_no";
                where = "  and G.client_code='" + ddl_client.SelectedValue + "' ";
            }
            else if (ddl_state.SelectedValue != "ALL")
            {
                where = where + " and G.state_name ='" + ddl_state.SelectedValue + "'";
            }
            if (type == "manual")
            {
                invoice_flag = " and final_invoice !='0'";
            }
            if (counter == 1)
            {
                query1 = "SELECT DATE_FORMAT(invoice_date, '%d/%m/%Y') AS 'billing_date',region,start_date,end_date,material_type, UNIT_NAME, U.unit_code, G.client_code, month, year, invoice_no, G.client_name, G.state_name, G.type,    CASE WHEN G.type = 'manpower' THEN 'Manpower Billing' WHEN G.type = 'conveyance' THEN 'Conveyance Billing' WHEN G.type = 'driver_conveyance' THEN 'Conveyance Billing' WHEN G.type = 'machine_rental' THEN 'Machine Rental'        WHEN G.type = 'material' THEN 'Material Billing'        WHEN G.type = 'deepclean' THEN 'Deep Clean Billing'        WHEN G.type = 'manpower_ot' THEN 'OT Billing'        WHEN G.type = 'r_and_m_bill' THEN 'R And M Service'  WHEN G.type = 'administrative_bill' THEN 'Administrative Expenses'  WHEN G.type = 'shiftwise_bill' THEN 'Shiftwise Billing'  WHEN G.type = 'office_rent_bill' THEN 'Office Rent Billing'    END AS 'billing_name' FROM pay_report_gst G LEFT JOIN pay_unit_master U ON G.unit_code = U.unit_code AND G.comp_code = U.COMP_CODE  WHERE (invoice_no IS NOT NULL  AND invoice_no !='') and G.comp_code = '" + Session["comp_code"].ToString() + "' and invoice_date between str_to_date('" + gst_from_date.Text + "','%d/%m/%Y') and str_to_date('" + gst_to_date.Text + "','%d/%m/%Y')  " + billing_type + "" + where + invoice_flag + " and (amount is not null ||amount != 0) ORDER BY billing_date , G.type";
            }
            if (ddl_gst_type.SelectedValue == "3")
            {
                query1 = "SELECT DATE_FORMAT(invoice_date, '%d/%m/%Y') AS 'billing_date',  M.zone, G.start_date, G.end_date, U.UNIT_NAME, G.material_type, U.unit_code, G.client_code, G.month, G.year, G.invoice_no, G.client_name, G.state_name, G.type,     CASE WHEN G.type = 'manpower' THEN 'Manpower Billing' WHEN G.type = 'conveyance' THEN 'Conveyance Billing' WHEN G.type = 'driver_conveyance' THEN 'Conveyance Billing' WHEN G.type = 'machine_rental' THEN 'Machine Rental'  WHEN G.type = 'material' THEN 'Material Billing'  WHEN G.type = 'deepclean' THEN 'Deep Clean Billing'        WHEN G.type = 'manpower_ot' THEN 'OT Billing'        WHEN G.type = 'r_and_m_bill' THEN 'R And M Service'        WHEN G.type = 'administrative_bill' THEN 'Administrative Expenses'        WHEN G.type = 'shiftwise_bill' THEN 'Shiftwise Billing'  WHEN G.type = 'office_rent_bill' THEN 'Office Rent Billing'    END AS 'billing_name' FROM pay_report_gst G LEFT JOIN pay_unit_master U ON G.unit_code = U.unit_code AND G.comp_code = U.COMP_CODE LEFT JOIN pay_billing_material_history M ON G.comp_code = M.comp_code AND G.state_name = M.state_name  AND G.month=M.month AND G.client_code = M.client_code WHERE (G.invoice_no IS NOT NULL  AND G.invoice_no != '') and G.comp_code = '" + Session["comp_code"].ToString() + "'  and G.invoice_date between str_to_date('" + gst_from_date.Text + "','%d/%m/%Y') and str_to_date('" + gst_to_date.Text + "','%d/%m/%Y')  " + billing_type + "  " + where + invoice_flag + "  AND M.zone is not null  AND (G.amount IS NOT NULL || G.amount != 0) group by G.invoice_no,M.zone ORDER BY billing_date , G.type";
            }
            MySqlDataAdapter dscmd = new MySqlDataAdapter(query1, d.con);
            DataSet ds = new DataSet();

            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);

            //Clear Folder INV_ZIP
            System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/INV_ZIP"));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            //END

            string folderPath = Server.MapPath("~/INV_ZIP\\" + ddl_client.SelectedItem.Text + "_" + ddl_gst_type.SelectedItem.Text);

            if (!Directory.Exists(folderPath))
            {
                //If Directory (Folder) does not exists. Create it.
                Directory.CreateDirectory(folderPath);
            }

            if (ds.Tables[0].Rows.Count > 0)
            {

                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {

                    state_name = ds.Tables[0].Rows[j]["state_name"].ToString();
                    bill_date = ds.Tables[0].Rows[j]["billing_date"].ToString();
                    client_name = ds.Tables[0].Rows[j]["client_name"].ToString();

                    invoice = ds.Tables[0].Rows[j]["invoice_no"].ToString();

                    client_code = ds.Tables[0].Rows[j]["client_code"].ToString();

                    bill_type = ds.Tables[0].Rows[j]["type"].ToString();

                    month = ds.Tables[0].Rows[j]["month"].ToString();

                    year = ds.Tables[0].Rows[j]["year"].ToString();

                    ddl_start_date_common = ds.Tables[0].Rows[j]["start_date"].ToString();

                    ddl_end_date_common = ds.Tables[0].Rows[j]["end_date"].ToString();

                    material_type = ds.Tables[0].Rows[j]["material_type"].ToString();

                    billing_name = ds.Tables[0].Rows[j]["billing_name"].ToString();

                    try
                    {

                        grade_code = d.getsinglestring("SELECT GRADE_CODE FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + client_code + "' and state_name='" + state_name + "' and invoice_no='" + invoice + "'");

                        Billing_wise = d.getsinglestring("SELECT DISTINCT B.billing_wise FROM pay_client_billing_details B WHERE B.comp_code = '" + Session["comp_code"].ToString() + "' AND B.client_code = '" + client_code + "' AND B.state ='" + state_name + "' AND billing_name='" + billing_name + "'");

                    }
                    catch { }


                    if (bill_type == "conveyance" || bill_type == "driver_conveyance")
                    {
                        region = d.getsinglestring("SELECT distinct zone FROM pay_billing_material_history where zone is not null and comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + client_code + "' and state_name='" + state_name + "' and invoice_no='" + invoice + "'"); ;
                    }
                    else
                    {
                        region = d.getsinglestring("SELECT distinct zone FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + client_code + "' and state_name='" + state_name + "' and invoice_no='" + invoice + "'");
                    }
                    unit_code = ds.Tables[0].Rows[j]["unit_code"].ToString();

                    designation = grade_code;

                    //1) Manpower Type and  7) Arrears Manpower
                    if (ddl_gst_type.SelectedValue == "1" || ddl_gst_type.SelectedValue == "7" || bill_type == "manpower" || bill_type == "arrears_manpower" || ddl_gst_type.SelectedValue == "14" || bill_type == "manpower_ot")
                    {
                        #region
                        int s_d = 0;
                        int e_d = 0; string str_date = "", end_date = "";

                        if (ddl_gst_type.SelectedValue == "7")
                        {
                            btn_arrears_invoiceClicked = true;
                            arrears_invoice = 1;
                        }
                        else
                        {
                            btn_arrears_invoiceClicked = false;
                            arrears_invoice = 0;
                        }


                        if (ddl_gst_type.SelectedValue == "14" || bill_type == "manpower_ot")
                        {
                            ot_invoice = 1;
                        }
                        else
                        {
                            ot_invoice = 0;
                        }

                        if (unit_code == "" || Billing_wise == "Statewise")
                        {
                            unit_code = "ALL";
                        }

                        if (ddl_client.SelectedValue == "4")
                        {
                            billing_process = "Non Metro";
                        }
                        else
                        {
                            billing_process = "Regular";
                        }

                        if (region == "")
                        {
                            region = "Select";
                        }

                        int month1 = Convert.ToInt32(month);
                        try
                        {
                            s_d = Convert.ToInt32(ddl_start_date_common);

                            e_d = Convert.ToInt32(ddl_end_date_common);

                        }
                        catch { }

                        string strMessage = string.Format(String.Format("{0:D2}", month1));

                        if (s_d == 0)
                        {
                            str_date = "0";
                            end_date = "0";
                        }
                        else
                        {
                            str_date = string.Format(String.Format("{0:D2}", s_d));

                            end_date = string.Format(String.Format("{0:D2}", e_d));
                        }
                        string txt_month_year2 = "" + strMessage + "/" + year + "";

                        dowmloadname = invoice;

                        if (ddl_start_date_common == "0" && ddl_end_date_common == "0")
                        {
                            ddl_arrears_type = "month";
                        }
                        else
                        {
                            ddl_arrears_type = "policy";
                        }

                        string billing_type1 = "And (bill_type is null || bill_type ='')";
                        string invoice_type = "";
                        string start_date = get_start_date(month, year);
                        string txt_month_year1 = "";
                        if (arrears_invoice == 1)
                        {
                            if (ddl_arrears_type.Equals("month"))
                            {
                                txt_month_year1 = txt_month_year2;
                            }
                            else
                            {
                                txt_month_year1 = txt_month_year2;

                                txt_arrear_month_year = str_date + "/" + txt_month_year2;
                                txt_arrear_monthend = end_date + "/" + txt_month_year2;
                            }
                            billing_type1 = "And bill_type = 'Arrears_bill'";
                            invoice_type = "3";
                        }
                        else
                        {
                            if (client_code == "HDFC" && state_name == "Madhya Pradesh")
                            {
                                invoice_type = "2";//UNCLUB
                            }
                            else
                            {
                                invoice_type = "1";//CLUB
                            }

                            txt_month_year1 = txt_month_year2;
                            ddl_arrears_type = "Select";
                        }

                        //Invoice clubing
                        string invoice_club = d.getsinglestring("select invoice_club from pay_client_master where comp_code = '" + Session["COMP_CODE"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'");

                        #region
                        if (ot_invoice == 1 && client_code != "HDFC")
                        {
                            crystalReport.Load(Server.MapPath("~/client_bill_invoice_ot_Tail.rpt"));
                        }
                        else if (client_code == "BAGICTM" && state_name == "Maharashtra")
                        {
                            crystalReport.Load(Server.MapPath("~/client_bill_invoice_unclub_bajaj.rpt"));
                        }
                        else if ((ddl_client.SelectedItem.Text.Contains("BAJAJ") && client_code != "4") || client_code == "DHFL")
                        {
                            crystalReport.Load(Server.MapPath("~/client_bill_invoice_club_bajaj.rpt"));
                        }
                        else if (client_code == "4")
                        {
                            crystalReport.Load(Server.MapPath("~/client_bill_invoice_bfl.rpt"));

                        }
                        else if (client_code == "LNT")
                        {
                            crystalReport.Load(Server.MapPath("~/shiftwise_invoice.rpt"));
                        }
                        else if (ot_invoice == 1 && client_code != "HDFC")
                        {
                            crystalReport.Load(Server.MapPath("~/client_bill_invoice_ot_hdfc.rpt"));
                        }

                        else if (client_code == "HDFC")
                        {
                            crystalReport.Load(Server.MapPath("~/client_bill_invoice_hdfc.rpt"));
                        }
                        else if (client_code == "Credence")
                        {

                            crystalReport.Load(Server.MapPath("~/client_bill_invoice_credence.rpt"));
                        }

                        else if (client_code == "SUN")
                        {
                            crystalReport.Load(Server.MapPath("~/client_bill_invoice_sungard.rpt"));
                        }
                        else if (client_code == "8")
                        {
                            crystalReport.Load(Server.MapPath("~/kotak_client_bill_invoice_club.rpt"));
                        }
                        else if (client_code == "RCPL")
                        {

                            if (client_code == "ALL")
                            {
                                crystalReport.Load(Server.MapPath("~/client_bill_invoice_RG.rpt"));
                            }
                            else
                            {

                                crystalReport.Load(Server.MapPath("~/client_bill_invoice_RG_unit.rpt"));
                            }
                        }
                        else
                        {
                            if (client_code.Equals("ESFB") || client_code.Equals("EquitasRes"))
                            {
                                crystalReport.Load(Server.MapPath("~/client_bill_invoice_equitas.rpt"));
                            }
                            else if (client_code.Equals("7"))
                            {
                                crystalReport.Load(Server.MapPath("~/client_bill_invoice_club_7.rpt"));
                            }
                            else if (client_code.Equals("RLIC HK"))
                            {
                                crystalReport.Load(Server.MapPath("~/client_bill_invoice_club_RLIC.rpt"));
                            }
                            else if (client_code.Equals("TECHM"))
                            {
                                crystalReport.Load(Server.MapPath("~/TECHM.rpt"));
                            }
                            else if (client_code.Equals("TAIL") && invoice_club == "manpower_material")
                            {
                                if (btn_arrears_invoiceClicked == true)
                                {
                                    //  crystalReport.Load(Server.MapPath("~/manpower_material_arrear.rpt"));
                                    crystalReport.Load(Server.MapPath("~/client_bill_invoice_arrear.rpt"));
                                }
                                else
                                {
                                    crystalReport.Load(Server.MapPath("~/manpower_material_club_bill.rpt"));
                                }

                            }
                            else if (invoice_club == "manpower_material_pestcontrol")   //ddl_client.SelectedValue.Equals("BALI") && 
                            {
                                if (btn_arrears_invoiceClicked == true)
                                {
                                    crystalReport.Load(Server.MapPath("~/client_bill_invoice_arrear.rpt"));
                                }
                                else
                                {
                                    crystalReport.Load(Server.MapPath("~/manpower_material_pestcontrol_bill.rpt"));
                                }
                            }
                            else if (client_code.Equals("BIRLA"))
                            {
                                crystalReport.Load(Server.MapPath("~/client_bill_invoice_BIRLA.rpt"));
                            }
                            else if (client_code.Equals("BEL") && state_name == "Maharashtra")
                            {
                                crystalReport.Load(Server.MapPath("~/client_bill_invoice_manpower_machine_rental.rpt"));
                            }
                            else { crystalReport.Load(Server.MapPath("~/client_bill_invoice_club.rpt")); }

                        }
                        #endregion

                        query = bs.get_invoice_query(Session["COMP_CODE"].ToString(), ddl_client.SelectedItem.Text, ddl_client.SelectedValue, state_name, unit_code, invoice_type, designation, txt_month_year2, int.Parse(str_date), int.Parse(end_date), billing_type1, region, txt_arrear_month_year, txt_arrear_monthend, ddl_arrears_type, invoice_fl_man, invoice_arrear, ot_invoice, billing_process);


                        Session["ReportMonthNo"] = "01";
                        #endregion
                        if (e_invoice == 1)
                        {
                            ReportLoad_e_invoice(query, dowmloadname, invoice, bill_date, state_name, strMessage, year, bill_type, folderPath);
                        }
                        else
                        {
                            ReportLoad_All_Invoice(query, dowmloadname, invoice, bill_date, state_name, strMessage, year, bill_type, folderPath);
                        }
                    }

                    //2) Convaynce Bill type
                    else if (ddl_gst_type.SelectedValue == "2" || bill_type == "conveyance")
                    {
                        conveyance_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, Billing_wise);
                    }

                    //3) Driver Convaynace Bill Type
                    else if (ddl_gst_type.SelectedValue == "3" || bill_type == "driver_conveyance")
                    {
                        Driver_conveyance_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, Billing_wise);
                    }

                    //4) Material Bill type
                    else if (ddl_gst_type.SelectedValue == "4" || bill_type == "material")
                    {
                        Material_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, Billing_wise, material_type);
                    }

                    //5) DeepClean
                    else if (ddl_gst_type.SelectedValue == "5" || bill_type == "deepclean")
                    {
                        DeepClean_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, Billing_wise); ;
                    }

                    //6) Machine Rental
                    else if (ddl_gst_type.SelectedValue == "6" || bill_type == "machine_rental")
                    {
                        Machine_rental_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, bill_type, Billing_wise, material_type);
                    }

                    //9) R_&_M
                    else if (ddl_gst_type.SelectedValue == "9" || bill_type == "r_and_m_bill")
                    {
                        R_and_M_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, bill_type, Billing_wise);
                    }

                    //10) Administrative
                    else if (ddl_gst_type.SelectedValue == "10" || bill_type == "administrative_bill")
                    {
                        Administrative_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, bill_type, Billing_wise);
                    }

                    //11) shiftwise_bill
                    else if (ddl_gst_type.SelectedValue == "11" || bill_type == "shiftwise_bill")
                    {
                        shiftwise_bill_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, bill_type, Billing_wise);
                    }

                    //12) incentive_bill
                    //else if (ddl_gst_type.SelectedValue == "12")
                    //{
                    //    incentive_bill_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, bill_type);
                    //}

                    //13) office_rent
                    else if (ddl_gst_type.SelectedValue == "13" || bill_type == "office_rent_bill")
                    {
                        office_rent_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, bill_type, Billing_wise);
                    }

                }
                //END

                //ZIP create
                create_zip();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Matching Records Found.');", true);
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    private void ReportLoad_All_Invoice(string query, string downloadfilename, string invoice, string bill_date, string state_name1, string month, string year, string bill_types, string directory_path)
    {

        string ot_applicable = "", machine_rental = "", handaling_amount = "", state_name = "";
        string headerpath = null;
        string footerpath = null;
        //Material Invoice
        string INV_bill_date = bill_date;
        try
        {
            //btnsendemail.Visible = true;
            double total_amount = 0, gst = 0;
            string downloadname = downloadfilename;
            System.Data.DataTable dt = new System.Data.DataTable();
            MySqlCommand cmd = new MySqlCommand(query, d.con);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            d.con.Open();
            sda.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('NO RECORD FOUND FOR THIS MONTH');", true);
                return;
            }
            else { }
            if (ddl_client.SelectedValue == "RCPL" && downloadfilename == "Invoice")
            {
                ot_applicable = d.getsinglestring("SELECT round((sum(pay_billing_unit_rate_history.Amount) + sum(pay_billing_unit_rate_history.uniform) + sum(pay_billing_unit_rate_history.operational_cost) + sum(pay_billing_unit_rate_history.Service_charge)),0) as Total FROM pay_billing_unit_rate_history where pay_billing_unit_rate_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_unit_rate_history.state_name = '" + state_name1 + "' AND pay_billing_unit_rate_history.month = '" + month + "' AND pay_billing_unit_rate_history.Year = '" + year + "' AND (emp_code != '' OR emp_code IS NOT NULL) AND start_date = '0' AND end_date = '0' AND (bill_type IS NULL || bill_type = '') group by pay_billing_unit_rate_history.client_code ");
                bill_date = dt.Rows[0][0].ToString();
            }
            else if (ddl_client.SelectedValue == "RCPL" && downloadfilename == "Material Invoice")
            {

                bill_date = dt.Rows[0][0].ToString();

            }
            //state_name = dt.Columns[14].ToString();
            d.con.Close();
            crystalReport.DataDefinition.FormulaFields["invoice_no"].Text = @"'" + invoice + "'";
            crystalReport.DataDefinition.FormulaFields["bill_date"].Text = @"'" + bill_date + "'";
            if (ddl_client.SelectedValue == "RCPL" && downloadfilename == "Invoice")
            {
                crystalReport.DataDefinition.FormulaFields["Unit_total_amount"].Text = @"'" + ot_applicable + "'";

            }
            if (Session["COMP_CODE"].ToString() == "C02")
            {
                headerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C02_header.png");
                footerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C02_footer.png");
                crystalReport.DataDefinition.FormulaFields["headerimagepath"].Text = @"'" + headerpath + "'";
                crystalReport.DataDefinition.FormulaFields["footerimagepath"].Text = @"'" + footerpath + "'";
            }
            else
            {
                headerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C01_header.png");
                footerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C01_footer.png");
                crystalReport.DataDefinition.FormulaFields["headerimagepath"].Text = @"'" + headerpath + "'";
                crystalReport.DataDefinition.FormulaFields["footerimagepath"].Text = @"'" + footerpath + "'";
            }
            
            PageMargins margins;
            // Get the PageMargins structure and set the 
            // margins for the report.
            margins = crystalReport.PrintOptions.PageMargins;
            margins.bottomMargin = 0;
            margins.leftMargin = 350;
            margins.rightMargin = 0;
            margins.topMargin = 0;
            // Apply the page margins.
            crystalReport.PrintOptions.ApplyPageMargins(margins);
            crystalReport.SetDataSource(dt);
            crystalReport.Refresh();
            string file_name = "";

            file_name = invoice + "_" + state_name1 + "_" + bill_types + ".pdf";

            string filepath = directory_path + "\\" + file_name;

            string Invpath = directory_path;


            if (INV_bill_date != "")
            {
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, filepath);
                ViewState["zip_path"] = directory_path;
                //DigitalSign_invoice_print(filepath, file_name, Invpath);
            }
            else
            {
                //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, downloadname);
            }


            //}

            ViewState["ALL_STATE"] = "0";
        }
        catch
        {
            // throw ex;
        }
        finally
        {

            d.con.Close();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }


    private void ReportLoad_e_invoice(string query, string downloadfilename, string invoice, string bill_date, string state_name1, string month, string year, string bill_types, string directory_path)
    {

        string ot_applicable = "", machine_rental = "", handaling_amount = "", state_name = "";
        string headerpath = null;
        string footerpath = null;
        string irn_no = "", irn_gstin = "", json_result = "", ack_no = "", ack_time = "", qr_img = "";

        //Material Invoice
        string INV_bill_date = bill_date;
        try
        {
            //btnsendemail.Visible = true;
            double total_amount = 0, gst = 0;
            string downloadname = downloadfilename;
            System.Data.DataTable dt = new System.Data.DataTable();
            MySqlCommand cmd = new MySqlCommand(query, d.con);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            d.con.Open();
            sda.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('NO RECORD FOUND FOR THIS MONTH');", true);
                return;
            }
            else { }
            // e-inv Start TM

            DataTable dt2 = new DataTable();
            MySqlCommand cmd2 = new MySqlCommand("select g.comp_code,g.client_code,g.client_name,g.gst_no as client_gstno,substring(g.gst_no,1,2) as client_statecode,z.field1 as client_gstaddress,z.field3 as client_pincode,g.sac_code,g.state_name,g.invoice_no,date_format(g.invoice_date,'%d/%m/%Y') as invoice_date,g.type, cmp.COMPANY_NAME, cmp.ADDRESS1 as cmp_address,  cmp.CITY as cmp_location, cmp.STATE as cmp_state,  cmp.SERVICE_TAX_REG_NO as cmp_gstin,cmp.pin as cmp_pin, substring(cmp.SERVICE_TAX_REG_NO,1,2) as cmp_state_code,ROUND(g.amount,2) as taxable_amt,ROUND(g.cgst,2) as cgst,ROUND(g.sgst,2) as sgst,ROUND(g.igst,2) as igst, ROUND((g.amount+g.cgst+g.sgst+g.igst),2) as billing_amt,( ROUND((g.amount+g.cgst+g.sgst+g.igst),2)- ROUND((g.amount+g.cgst+g.sgst+g.igst),0)) as rounding_amt,ROUND((g.amount+g.cgst+g.sgst+g.igst),0) as roundoff_billing_amt  from pay_report_gst g   inner join pay_company_master cmp on g.comp_code=cmp.COMP_CODE   left join pay_zone_master z on g.comp_code=z.comp_code and g.client_code=z.client_code and g.state_name=z.REGION and z.type='GST'  where g.month='" + month + "' and g.year='" + year + "' and g.invoice_no='" + invoice + "'", d.con1);
            MySqlDataAdapter dt_item2 = new MySqlDataAdapter(cmd2);
            dt_item2.Fill(dt2);

            string client_code = "", client_name = "", client_gstno = "", client_statecode = "", client_gstaddress = "", client_pincode = "0", sac_code = "", statename = "", invoice_no = "", invoice_date = "", billing_type = "", COMPANY_NAME = "", cmp_address = "", cmp_location = "", cmp_state = "", cmp_gstin = "", cmp_pin = "0", cmp_state_code = "", taxable_amt = "0", cgst = "0", sgst = "0", igst = "0", billing_amt = "0", rounding_amt = "0", roundoff_billing_amt = "0";
            if (dt2.Rows.Count > 0)
            {
                client_code = dt2.Rows[0]["client_code"].ToString();
                client_name = dt2.Rows[0]["client_name"].ToString();
                client_gstno = dt2.Rows[0]["client_gstno"].ToString();
                client_statecode = dt2.Rows[0]["client_statecode"].ToString();
                client_gstaddress = dt2.Rows[0]["client_gstaddress"].ToString();
                client_pincode = dt2.Rows[0]["client_pincode"].ToString();

                COMPANY_NAME = dt2.Rows[0]["COMPANY_NAME"].ToString();
                cmp_address = dt2.Rows[0]["cmp_address"].ToString();
                cmp_location = dt2.Rows[0]["cmp_location"].ToString();
                cmp_state = dt2.Rows[0]["cmp_state"].ToString();
                cmp_gstin = dt2.Rows[0]["cmp_gstin"].ToString();
                cmp_pin = (dt2.Rows[0]["cmp_pin"].ToString()).Trim();
                cmp_state_code = dt2.Rows[0]["cmp_state_code"].ToString();


                sac_code = dt2.Rows[0]["sac_code"].ToString();
                statename = dt2.Rows[0]["state_name"].ToString();
                invoice_no = dt2.Rows[0]["invoice_no"].ToString();
                invoice_date = dt2.Rows[0]["invoice_date"].ToString();
                billing_type = dt2.Rows[0]["type"].ToString();
                // eg, taxable_amt= 488798.24, cgst=43991.86, sgst=43991.86, igst=0.00, billing_amt=576781.96, rounding_amt=-0.04, roundoff_billing_amt=576782    
                taxable_amt = dt2.Rows[0]["taxable_amt"].ToString();
                cgst = dt2.Rows[0]["cgst"].ToString();
                sgst = dt2.Rows[0]["sgst"].ToString();
                igst = dt2.Rows[0]["igst"].ToString();
                billing_amt = dt2.Rows[0]["billing_amt"].ToString();
                rounding_amt = dt2.Rows[0]["rounding_amt"].ToString();
                roundoff_billing_amt = dt2.Rows[0]["roundoff_billing_amt"].ToString();

            }
            DataTable dt1 = new DataTable();
            MySqlCommand cmd1 = new MySqlCommand("select id, comp_code, client_code, invoice_no, invoice_date, irnno, irn_gstin, ack_no, ack_date, qr_code_image, state, billtype, status from pay_einvoice_detail where month='" + month + "' and year='" + year + "' and invoice_no='" + invoice + "'", d.con1);
            MySqlDataAdapter dt_item = new MySqlDataAdapter(cmd1);
            dt_item.Fill(dt1);


            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow row in dt1.Rows)
                {

                    irn_gstin = row["irn_gstin"].ToString();
                    irn_no = row["irnno"].ToString();
                    ack_no = row["ack_no"].ToString();
                    ack_time = row["ack_date"].ToString();
                    // qr_img = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "" + row["qr_code_image"].ToString()+ "");
                    qr_img = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "E_Invoice_code\\" + row["qr_code_image"].ToString() + "");
                }
            }
            else
            {
                // Generate E-Invoice IRN & QR Code
                /* OctaBills Cloud API */
                var client = new OctaBillsApiClient(KeyId, KeySecret);
                /* Use for OctaBills Server API  */
                //var client = new OctaBillsApiClient(ServerAddress, ServerPort, Username, Password);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                var doc = new JDocument
                {
                    TxnInfo = new JTxnInfo
                    {
                        TaxScheme = "GST",
                        SupplyType = "B2B",
                        IsRcmApplied = "N"
                    },

                    DocInfo = new JDocInfo
                    {
                        // DocType INV--Regular Invoice    CRN-- Credit Note     DBN-- Debit NOte

                        //  DocType = "INV",
                        // DocType = "CRN",
                        DocType = "DBN",
                        DocNo = "API-" + (DateTime.Now.Ticks / TimeSpan.TicksPerSecond),
                        DocDate = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)

                        //// Invoice detail
                        //DocType = "INV",  // regularinvoice=INV   Credit Note =CRN    Debit Note=DBN
                        //DocNo = invoice_no,//"API-" + (DateTime.Now.Ticks / TimeSpan.TicksPerSecond),
                        //DocDate = invoice_date//DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                    },

                    Seller = new JContactInfo
                    {//company detail -- testing
                        Gstin = "08AASFB9647G1ZU",
                        LegalName = "Blowbits Solution LLP",
                        TradeName = "Blowbits Solution LLP",
                        Addr1 = "146 Ashok Nagar",
                        Addr2 = "Road No 8",
                        Location = "Jaipur",
                        PinCode = 313001,
                        StateCode = "08"

                        ////ihms
                        //Gstin = cmp_gstin,
                        //LegalName = COMPANY_NAME,
                        //TradeName = COMPANY_NAME,
                        //Addr1 = cmp_address,
                        //Addr2 = "NA",
                        //Location = cmp_location,
                        //PinCode = Convert.ToInt32(cmp_pin),
                        //StateCode = cmp_state_code


                    },

                    Buyer = new JContactInfo
                    {
                        // Client detail--testing
                        Gstin = "33AABCN5735F1ZP",
                        LegalName = "Star Colourpark India Private Limited",
                        TradeName = "Star Colourpark India Private Limited",
                        PlaceOfSupply = "33",
                        Addr1 = "110, Asoka Plaza,",
                        Addr2 = "Dr. Nanjappa Road, Gandhipuram",
                        Location = "Coimbatore",
                        PinCode = 641018,
                        StateCode = "33"

                        //// IHMS
                        //Gstin = client_gstno,
                        //LegalName = client_name,
                        //TradeName = client_name,
                        //PlaceOfSupply = statename,
                        //Addr1 = client_gstaddress,
                        //Addr2 = "NA",
                        //Location =statename,
                        //PinCode = Convert.ToInt32(client_pincode),
                        //StateCode = client_statecode
                    },

                    Items = new List<JLineItem>
                {
                    new JLineItem
                    { // testing
                          SrNo ="1",
                        ProductDescription = "Manpower",
                         Hsn= "1001",
                        Qty= 1,
                        Uqc= "OTH",
                        UnitPrice= 5000,
                        ItemGrossValue=  5000,
                        Discount= 0,
                        TaxableValue= 5000,
                        GstRate= 18,
                        Igst= 900
                        //// ihms
                        //SrNo ="1",
                        //ProductDescription = billing_type,
                        //Hsn= sac_code,
                        //Qty= 1,
                        //Uqc= "OTH",
                        //UnitPrice= Convert.ToDecimal(taxable_amt),
                        //ItemGrossValue= Convert.ToDecimal(taxable_amt),
                        //Discount= 0,
                        //TaxableValue= Convert.ToDecimal(taxable_amt),
                        //GstRate= 18,
                        //Igst= Convert.ToDecimal(igst),
                        //Cgst=Convert.ToDecimal(cgst),                        
                        //Sgst=Convert.ToDecimal(sgst)
                    }
                },

                    DocSummary = new JDocSummary
                    { // testing
                        RoundingOff = 0,
                        DocValue = 5900
                        //// Ihms
                        //RoundingOff =Convert.ToDecimal(rounding_amt),
                        //DocValue = Convert.ToDecimal(roundoff_billing_amt)
                    }

                };

                try
                {
                    var result = client.GenerateIrn(doc, true, false);
                    json_result = JsonConvert.SerializeObject(result, Formatting.Indented);
                    //--IRN cancel btn
                    // buttonCancelIrn.Enabled = result.Success;
                    string Qr_codename = invoice + ".png";
                    if (result.Success)
                    {
                        irn_gstin = doc.Seller.Gstin;
                        irn_no = result.Irn;
                        ack_no = result.AckNo;
                        ack_time = result.AckTime;
                        if (result.QRCodeImagePng != null)
                        {
                            var imagedata = Convert.FromBase64String(result.QRCodeImagePng);//view image code
                            //   QrCodeImage.Image = Image.FromStream(new MemoryStream(imagedata));     //--Windows
                            //   QrCodeImage.ImageUrl = "data:image;base64," + Convert.ToBase64String(imagedata);  //--Asp

                            string qt_code = System.IO.Path.Combine(Convert.ToBase64String(imagedata));
                            byte[] bytes = Convert.FromBase64String(qt_code);

                            System.Drawing.Image image;
                            using (MemoryStream ms2 = new MemoryStream(bytes))
                            {
                                image = System.Drawing.Image.FromStream(ms2);
                                image.Save(Server.MapPath("E_Invoice_code/") + Qr_codename, System.Drawing.Imaging.ImageFormat.Jpeg);

                            }
                            qr_img = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "E_Invoice_code\\" + Qr_codename + "");

                        }

                        DateTime theDate = Convert.ToDateTime(ack_time);
                        string ack_date = theDate.ToString("yyyy-MM-dd H:mm:ss");

                        d.operation("INSERT INTO pay_einvoice_detail (comp_code, client_code,client_name, invoice_no, invoice_date, irnno, irn_gstin, ack_no, ack_date, qr_code_image, state, billtype,month,year,client_gstin) values ('" + Session["COMP_CODE"].ToString() + "','" + client_code + "','" + client_name + "','" + invoice + "',str_to_date('" + bill_date + "','%d/%m/%Y'),'" + irn_no + "','" + irn_gstin + "','" + ack_no + "', '" + ack_date + "' ,'" + Qr_codename + "','" + state_name1 + "','" + bill_types + "','" + month + "','" + year + "','" + client_gstno + "') ");
                        d.operation("  Update pay_report_gst set softcopy_sendmail_status=0,e_invoice_status=1 where invoice_no='" + invoice + "' and month='" + month + "' and year='" + year + "' ");

                    }

                }
                catch (Exception ex)
                {
                    json_result = ex.Message;
                }
                // e-inv end

            }

            #region


            if (ddl_client.SelectedValue == "RCPL" && downloadfilename == "Invoice")
            {
                ot_applicable = d.getsinglestring("SELECT round((sum(pay_billing_unit_rate_history.Amount) + sum(pay_billing_unit_rate_history.uniform) + sum(pay_billing_unit_rate_history.operational_cost) + sum(pay_billing_unit_rate_history.Service_charge)),0) as Total FROM pay_billing_unit_rate_history where pay_billing_unit_rate_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_unit_rate_history.state_name = '" + state_name1 + "' AND pay_billing_unit_rate_history.month = '" + month + "' AND pay_billing_unit_rate_history.Year = '" + year + "' AND (emp_code != '' OR emp_code IS NOT NULL) AND start_date = '0' AND end_date = '0' AND (bill_type IS NULL || bill_type = '') group by pay_billing_unit_rate_history.client_code ");
                bill_date = dt.Rows[0][0].ToString();
            }
            else if (ddl_client.SelectedValue == "RCPL" && downloadfilename == "Material Invoice")
            {

                bill_date = dt.Rows[0][0].ToString();

            }
            //state_name = dt.Columns[14].ToString();
            d.con.Close();
            //crystalReport.DataDefinition.FormulaFields["invoice_no"].Text = @"'" + invoice + "'";
            //crystalReport.DataDefinition.FormulaFields["bill_date"].Text = @"'" + bill_date + "'";


            crystalReport.DataDefinition.FormulaFields["invoice_no"].Text = @"'" + invoice + "'";
            crystalReport.DataDefinition.FormulaFields["bill_date"].Text = @"'" + bill_date + "'";


            crystalReport.DataDefinition.FormulaFields["irn_no"].Text = @"'" + "IRN.: " + irn_no + "'";
            crystalReport.DataDefinition.FormulaFields["qr_code"].Text = @"'" + qr_img + "'";
            crystalReport.DataDefinition.FormulaFields["ack_no"].Text = @"'" + "Ack No : " + ack_no + "'";
            crystalReport.DataDefinition.FormulaFields["ack_time"].Text = @"'" + "Ack Date : " + ack_time + "'";



            if (ddl_client.SelectedValue == "RCPL" && downloadfilename == "Invoice")
            {
                crystalReport.DataDefinition.FormulaFields["Unit_total_amount"].Text = @"'" + ot_applicable + "'";

            }
            if (Session["COMP_CODE"].ToString() == "C02")
            {
                headerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C02_header.png");
                footerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C02_footer.png");
                crystalReport.DataDefinition.FormulaFields["headerimagepath"].Text = @"'" + headerpath + "'";
                crystalReport.DataDefinition.FormulaFields["footerimagepath"].Text = @"'" + footerpath + "'";
            }
            else
            {
                headerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C01_header.png");
                footerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C01_footer.png");
                crystalReport.DataDefinition.FormulaFields["headerimagepath"].Text = @"'" + headerpath + "'";
                crystalReport.DataDefinition.FormulaFields["footerimagepath"].Text = @"'" + footerpath + "'";
            }
            if (ddl_client.SelectedValue == "RCPL" && downloadfilename == "Material Invoice")
            {


            }
            PageMargins margins;
            // Get the PageMargins structure and set the 
            // margins for the report.
            margins = crystalReport.PrintOptions.PageMargins;
            margins.bottomMargin = 0;
            margins.leftMargin = 350;
            margins.rightMargin = 0;
            margins.topMargin = 0;
            // Apply the page margins.
            crystalReport.PrintOptions.ApplyPageMargins(margins);
            crystalReport.SetDataSource(dt);
            crystalReport.Refresh();
            string file_name = "";

            file_name = invoice + "_" + bill_types + ".pdf";

            string filepath = directory_path + "\\" + file_name;

            string Invpath = directory_path;


            if (INV_bill_date != "")
            {
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, filepath);
                ViewState["zip_path"] = directory_path;
                DigitalSign_invoice_print(filepath, file_name, Invpath);
            }
            else
            {
                //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, downloadname);
            }

            #endregion
            //}

            ViewState["ALL_STATE"] = "0";
        }
        catch
        {
            // throw ex;
        }
        finally
        {

            d.con.Close();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    protected void create_zip()
    {
        string inv_path = ViewState["zip_path"].ToString();
        string zip_filename = inv_path + ".zip";

        //Delete previous ZIP 
        FileInfo ZIP = new FileInfo(zip_filename);

        if (ZIP.Exists)
        {
            ZIP.Delete();
        }

        //ZIP create 

        ZipFile.CreateFromDirectory(inv_path, zip_filename);

        try
        {
            //Download ZIP
            System.IO.FileInfo _file = new System.IO.FileInfo(zip_filename);
            if (_file.Exists)
            {
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + _file.Name);
                Response.AddHeader("Content-Length", _file.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.WriteFile(_file.FullName);
                Response.End();
            }
            else
            {
                ClientScript.RegisterStartupScript(Type.GetType("System.String"), "messagebox", "&lt;script type=\"text/javascript\"&gt;alert('File not Found');</script>");
            }

        }
        catch { }
    }

    protected string get_start_date(string month, string year)
    {
        return d1.getsinglestring("SELECT IFNULL((SELECT start_date_common FROM pay_billing_master_history INNER JOIN pay_unit_master ON pay_billing_master_history.billing_unit_code = pay_unit_master.unit_code AND pay_billing_master_history.comp_code = pay_unit_master.comp_code WHERE pay_billing_master_history.billing_client_code = '" + ddl_client.SelectedValue + "' AND month = '" + month + "' and year = '" + year + "' and  pay_billing_master_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' limit 1),(SELECT start_date_common FROM pay_billing_master INNER JOIN pay_unit_master ON pay_billing_master.billing_unit_code = pay_unit_master.unit_code AND pay_billing_master.comp_code = pay_unit_master.comp_code WHERE pay_billing_master.billing_client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' limit 1))");
    }

    protected string getmonth(string month)
    {
        month = (int.Parse(month)).ToString();
        if (month == "1")
        {
            return "JAN";
        }
        else if (month == "2")
        { return "FEB"; }
        else if (month == "3")
        { return "MAR"; }
        else if (month == "4")
        { return "APR"; }
        else if (month == "5")
        { return "MAY"; }
        else if (month == "6")
        { return "JUN"; }
        else if (month == "7")
        { return "JUL"; }
        else if (month == "8")
        { return "AUG"; }
        else if (month == "9")
        { return "SEP"; }
        else if (month == "10")
        { return "OCT"; }
        else if (month == "11")
        { return "NOV"; }
        else if (month == "12")
        { return "DEC"; }
        return "";

    }

    protected void conveyance_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month, string year, string folderPath, string bill_wise)
    {
        #region
        if (unit_code == "" || bill_wise == "Statewise")
        {
            unit_code = "ALL";
        }
        if (state_name == "")
        {
            state_name = "ALL";
        }
        string where = "", delete_where = "";
        string sql = "";
        string query_con = "";

        int month2 = Convert.ToInt32(month);
        int year3 = Convert.ToInt32(year);

        string month_m = string.Format(String.Format("{0:D2}", month2));

        int month_i = Convert.ToInt32(month_m);

        string start_date_common = get_start_date(month, year);
        DateTimeFormatInfo mfi = new DateTimeFormatInfo();
        string month_name = mfi.GetMonthName(month_i).ToString();

        where = " pay_conveyance_amount_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_conveyance_amount_history.client_code= '" + ddl_client.SelectedValue + "' and pay_conveyance_amount_history.unit_code='" + unit_code + "' and pay_conveyance_amount_history.month='" + month_m + "' and pay_conveyance_amount_history.year = '" + year3 + "'  and conveyance='emp_conveyance'  ";

        int month1 = month_i - 1;
        int year_1 = year3;
        if (month1 == 0)
        {
            month1 = 12;
            year_1 = year_1 - 1;
        }

        string daterange = "concat(upper(DATE_FORMAT(str_to_date('" + year_1 + "-" + month + "-01','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(LAST_DAY('" + year_1 + "-" + month_i + "-01'), '%d %b %Y'))) as start_end_date";

        if (start_date_common != "" && start_date_common != "1")
        {
            daterange = "concat(upper(DATE_FORMAT(str_to_date('" + year_1 + "-" + ((month_i == 1) ? 12 : (month_i - 1)) + "-" + start_date_common + "','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(str_to_date('" + year3 + "-" + month_i + "-" + (int.Parse(start_date_common) - 1) + "','%Y-%m-%d'), '%d %b %Y'))) as start_end_date";
        }

        string where1 = "", where_state = "";

        if (state_name.Equals("Maharashtra") && client_code.Equals("BAGIC") && int.Parse("" + year3 + "" + month_m + "") > 20204) { where_state = " and state='" + state_name + "' and billingwise_id = 5"; }

        if (d.getsinglestring("select billingwise_id from pay_client_billing_details where  client_code = '" + ddl_client.SelectedValue + "' " + where_state).Equals("5"))
        {
            where_state = " and zone = '" + region + "'";
        }
        else
        { where_state = ""; }
        delete_where = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + unit_code + "' and month='" + month_m + "' and year = '" + year3 + "'   and Type = 'Conveyance' ";
        where = "where pay_billing_material_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + unit_code + "' and month='" + month_m + "' and year = '" + year3 + "'   and Type = 'Conveyance' ";

        if (state_name == "ALL")
        {
            where = "where pay_billing_material_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code= '" + ddl_client.SelectedValue + "'  and month='" + month_m + "' and year = '" + year3 + "'   and Type = 'Conveyance' ";
        }
        else if (unit_code == "ALL")
        {
            where = "where pay_billing_material_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + state_name + "'  and month='" + month_m + "' and year = '" + year3 + "'   and Type = 'Conveyance' ";
        }

        if (!d.getsinglestring("select max(conveyance_type) FROM pay_billing_material_history " + where + " and conveyance_type=3 limit 1").Equals("3"))
        {
            crystalReport.Load(Server.MapPath("~/client_bill_invoice_conveyance.rpt"));
            where1 = where;
            where = where + where_state + " AND Conveyance_PerKmRate > 0 and conveyance_type != '100' GROUP by state_name ";
            query_con = "SELECT  pay_billing_material_history.comp_code ,  COMPANY_NAME ,  COMP_ADDRESS1 ,  COMP_ADDRESS2 ,  COMP_CITY ,  COMP_STATE as 'STATE',  PF_REG_NO ,  COMPANY_PAN_NO ,  COMPANY_TAN_NO ,  COMPANY_CIN_NO ,  SERVICE_TAX_REG_NO ,  ESIC_REG_NO ,  STATE_NAME ,  UNIT_full_ADD1 as  'UNIT_ADD1' ,  invoice_shipping_address AS 'UNIT_ADD2' ,  UNIT_CITY ,  UNIT_NAME ,  client  AS 'other',  unit_gst_no ,  grade_desc  AS 'designation',  " + daterange + ",   concat('" + month_name + "',' ' ,'" + year3 + "') AS 'month' ,  housekeeiing_sac_code ,  Security_sac_code , IF( conveyance_applicable  = 1,  Conveyance_PerKmRate , 0) AS 'grand_total', SUM(IF( conveyance_applicable  = 1 AND  conveyance_type  = 2, ( Conveyance_PerKmRate  *  conveyance_km ),  conveyance_rate )) AS 'total', IF( conveyance_service_charge  = 1,  conveyance_service_charge_per , 0) AS 'Expr1', IF( conveyance_service_charge  = 1,  conveyance_service_amount , 0) AS 'hrs_12_ot', SUM(IF( conveyance_type  = 1, ( conveyance_rate  /  Conveyance_PerKmRate ),  conveyance_km )) AS 'month_days',  unit_code ,if(pay_billing_material_history.comp_code = 'C02','Ranchi','Pune') as 'EMP_NAME',po_no as 'type'  FROM pay_billing_material_history LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_material_history.comp_code AND pay_client_billing_details.client_code = pay_billing_material_history.client_code AND pay_client_billing_details.STATE = pay_billing_material_history.state_name AND billing_name = 'Conveyance Billing' " + where;

        }
        else
        {
            crystalReport.Load(Server.MapPath("~/client_bill_invoice_conveyance_empwise.rpt"));
            where1 = where;
            where = where + where_state + " and conveyance_type != '100' GROUP by state_name ";
            query_con = "SELECT  pay_billing_material_history.comp_code ,  COMPANY_NAME ,  COMP_ADDRESS1 ,  COMP_ADDRESS2 ,  COMP_CITY ,  COMP_STATE as 'STATE',  PF_REG_NO ,  COMPANY_PAN_NO ,  COMPANY_TAN_NO ,  COMPANY_CIN_NO ,  SERVICE_TAX_REG_NO ,  ESIC_REG_NO ,  STATE_NAME ,  UNIT_full_ADD1 as  'UNIT_ADD1' ,  invoice_shipping_address AS 'UNIT_ADD2' ,  UNIT_CITY ,  UNIT_NAME ,  client  AS 'other',  unit_gst_no ,  grade_desc  AS 'designation',  " + daterange + ",   concat('" + month_name + "',' ' ,'" + year3 + "') AS 'month' ,  housekeeiing_sac_code ,  Security_sac_code , count(emp_code) AS 'grand_total', SUM(IF( conveyance_applicable  = 1 AND  conveyance_type  = 2, ( Conveyance_PerKmRate  *  conveyance_km ),  conveyance_rate )) AS 'total', IF( conveyance_service_charge  = 1,  conveyance_service_charge_per , 0) AS 'Expr1', IF( conveyance_service_charge  = 1,  conveyance_service_amount , 0) AS 'hrs_12_ot', SUM(IF( conveyance_type  = 1, ( conveyance_rate  /  Conveyance_PerKmRate ),  conveyance_km )) AS 'month_days',  unit_code,if(pay_billing_material_history.comp_code = 'C02','Ranchi','Pune') as 'EMP_NAME',po_no as 'type'  FROM pay_billing_material_history LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_material_history.comp_code AND pay_client_billing_details.client_code = pay_billing_material_history.client_code AND pay_client_billing_details.STATE = pay_billing_material_history.state_name AND billing_name = 'Conveyance Billing'" + where;

        }
        #endregion
        if (e_invoice == 1)
        {
            ReportLoad_e_invoice(query_con, dowmloadname, invoice, bill_date, state_name, month, year, "Conveyance", folderPath);
        }
        else
        {
            ReportLoad_All_Invoice(query_con, dowmloadname, invoice, bill_date, state_name, month, year, "Conveyance", folderPath);
        }

    }

    protected void Driver_conveyance_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month, string year, string folderPath, string bill_wise)
    {
        #region
        if (unit_code == "" || bill_wise == "Statewise")
        {
            unit_code = "ALL";
        }
        if (state_name == "")
        {
            state_name = "ALL";
        }

        string where = "";
        string query_con = "";
        int month2 = Convert.ToInt32(month);
        int year3 = Convert.ToInt32(year);
        string month_m = string.Format(String.Format("{0:D2}", month2));
        int month_i = Convert.ToInt32(month_m);
        string start_date_common = get_start_date(month, year);
        DateTimeFormatInfo mfi = new DateTimeFormatInfo();
        string month_name = mfi.GetMonthName(month_i).ToString();

        where = " pay_conveyance_amount_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_conveyance_amount_history.client_code= '" + client_code + "' and pay_conveyance_amount_history.unit_code='" + unit_code + "' and pay_conveyance_amount_history.month='" + month_m + "' and pay_conveyance_amount_history.year = '" + year3 + "'  ";
        if (state_name == "ALL")
        {
            where = " pay_conveyance_amount_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_conveyance_amount_history.client_code= '" + client_code + "'  and pay_conveyance_amount_history.month='" + month_m + "' and pay_conveyance_amount_history.year = '" + year3 + "' ";
        }
        else if (unit_code == "ALL")
        {
            where = " pay_conveyance_amount_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_conveyance_amount_history.client_code= '" + client_code + "' and pay_conveyance_amount_history.state = '" + state_name + "'  and pay_conveyance_amount_history.month='" + month_m + "' and pay_conveyance_amount_history.year = '" + year3 + "' ";

        }

        int month1 = month_i - 1;
        int year_1 = year3;
        if (month1 == 0)
        {
            month1 = 12;
            year_1 = year_1 - 1;
        }


        string sql = "";
        string where1 = "", where_state = "";

        if (state_name.Equals("Maharashtra") && client_code.Equals("BAGIC") && int.Parse("" + year3 + "" + month_m + "") > 20204) { where_state = " and state='" + state_name + "' and billingwise_id = 5"; }


        if (d.getsinglestring("select billingwise_id from pay_client_billing_details where  client_code = '" + client_code + "' " + where_state).Equals("5"))
        {
            if (region != "")
            {
                where_state = " and pay_billing_material_history.zone = '" + region + "'";
            }

        }
        else
        { where_state = ""; }

        string daterange = "concat(upper(DATE_FORMAT(str_to_date('" + year + "-" + month1 + "-01','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(LAST_DAY('" + year + "-" + month1 + "-01'), '%d %b %Y'))) as start_end_date";

        if (start_date_common != "" && start_date_common != "1")
        {
            daterange = "concat(upper(DATE_FORMAT(str_to_date('" + year + "-" + ((month2 == 1) ? 12 : (month_i - 1)) + "-" + start_date_common + "','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(str_to_date('" + year3 + "-" + month_m + "-" + (int.Parse(start_date_common) - 1) + "','%Y-%m-%d'), '%d %b %Y'))) as start_end_date";
        }

        where = "where pay_billing_material_history.comp_code='" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_material_history.client_code= '" + client_code + "' and pay_billing_material_history.unit_code='" + unit_code + "' and pay_billing_material_history.month='" + month_m + "' and pay_billing_material_history.year = '" + year3 + "'  AND pay_conveyance_amount_history.conveyance = 'driver_conveyance' AND pay_billing_material_history.conveyance_type = '100' ";
        where1 = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + client_code + "' and month='" + month_m + "' and year = '" + year3 + "' ";

        if (state_name == "ALL")
        {
            where = "where pay_billing_material_history.comp_code='" + Session["comp_code"].ToString() + "' " + where_state + "  and pay_billing_material_history.client_code= '" + client_code + "'  and pay_billing_material_history.month='" + month_m + "' and pay_billing_material_history.year = '" + year3 + "'  AND pay_billing_material_history.conveyance_type = '100' AND pay_conveyance_amount_history.conveyance = 'driver_conveyance'";
            where1 = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + client_code + "'  and month='" + month_m + "' and year = '" + year3 + "' ";
        }
        else if (unit_code == "ALL")
        {
            where = "where pay_billing_material_history.comp_code='" + Session["comp_code"].ToString() + "' " + where_state + "  and pay_billing_material_history.client_code= '" + client_code + "' and pay_billing_material_history.state_name = '" + state_name + "'  and pay_billing_material_history.month='" + month_m + "' and pay_billing_material_history.year = '" + year3 + "'  AND pay_billing_material_history.conveyance_type = '100' AND pay_conveyance_amount_history.conveyance = 'driver_conveyance'";
            where1 = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + client_code + "' and state_name = '" + state_name + "'  and month='" + month_m + "' and year = '" + year3 + "'";
        }


        crystalReport.Load(Server.MapPath("~/driver_conveyance.rpt"));

        where = where + "  GROUP by state_name ";

        query_con = "SELECT pay_billing_material_history.comp_code, COMPANY_NAME, COMP_ADDRESS1 AS 'ADDRESS1', COMP_ADDRESS2 AS 'ADDRESS2', COMP_CITY AS 'CITY', COMP_STATE AS 'STATE', PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, STATE_NAME, UNIT_full_ADD1 AS 'UNIT_ADD1', invoice_shipping_address AS 'UNIT_ADD2', UNIT_CITY, UNIT_NAME, client AS 'other', unit_gst_no, grade_desc AS 'designation',  " + daterange + ",   concat('" + month_name + "',' ' ,'" + year3 + "') AS 'month', housekeeiing_sac_code, SUM(((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) + (((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) * 5 / 100)) AS 'total', SUM(IF(LOCATE(COMP_STATE, state_name), ((((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) + (((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) * 5 / 100)) * 9 / 100), 0)) AS 'SGST', SUM(IF(LOCATE(COMP_STATE, state_name), ((((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) + (((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) * 5 / 100)) * 9 / 100), 0)) AS 'CGST', SUM(IF(LOCATE(COMP_STATE, state_name), 0, ((((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) + (((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + ( total_km)) * 5 / 100)) * 18 / 100))) AS 'IGST', pay_billing_material_history.unit_code,if(pay_billing_material_history.comp_code = 'C02','Ranchi','Pune') as 'month_days',po_no as 'type'   FROM pay_billing_material_history INNER JOIN pay_conveyance_amount_history ON pay_conveyance_amount_history.emp_code = pay_billing_material_history.emp_code AND pay_conveyance_amount_history.comp_code = pay_billing_material_history.comp_code  AND pay_conveyance_amount_history.month = '" + month_m + "' AND pay_conveyance_amount_history.year = '" + year3 + "' AND pay_conveyance_amount_history.conveyance = 'driver_conveyance' INNER JOIN pay_billing_master ON pay_billing_master.billing_unit_code = pay_billing_material_history.unit_code AND pay_billing_master.comp_code = pay_billing_material_history.comp_code AND pay_billing_master.designation = pay_billing_material_history.GRADE_CODE LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_material_history.comp_code AND pay_client_billing_details.client_code = pay_billing_material_history.client_code AND pay_client_billing_details.STATE = pay_billing_material_history.state_name AND billing_name = 'Conveyance Billing'    " + where;

        string gst_to_be = d.getsinglestring("  select  DISTINCT (Gst_to_be) as 'Gst_to_be' from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + client_code + "'");

        #endregion
        if (e_invoice == 1)
        {
            ReportLoad_e_invoice(query_con, dowmloadname, invoice, bill_date, state_name, month, year, "driver_conveyance", folderPath);
        }
        else
        {
            ReportLoad_All_Invoice(query_con, dowmloadname, invoice, bill_date, state_name, month, year, "driver_conveyance", folderPath);
        }
    }

    protected void Material_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month_m, string year_y, string folderPath, string bill_wise, string material_type_tissue)
    {
        #region
        if (unit_code == "" || bill_wise == "Statewise")
        {
            unit_code = "ALL";
        }
        if (state_name == "")
        {
            state_name = "ALL";
        }
        int month2 = Convert.ToInt32(month_m);
        int year = Convert.ToInt32(year_y);
        string month = string.Format(String.Format("{0:D2}", month2));
        int month_i = Convert.ToInt32(month);
        string where = "";

        string query_con = "", material_type = "";

        if (material_type_tissue == "Tissue")
        {
            material_type = "2";
        }
        else
        {
            material_type = "1";
        }

        where = " pay_material_details.comp_code='" + Session["comp_code"].ToString() + "' and pay_material_details.client_code= '" + ddl_client.SelectedValue + "' and pay_material_details.unit_code='" + unit_code + "' and pay_material_details.month='" + month + "' and pay_material_details.year = '" + year + "'  ";
        if (state_name == "ALL")
        {
            where = " pay_material_details.comp_code='" + Session["comp_code"].ToString() + "' and pay_material_details.client_code= '" + ddl_client.SelectedValue + "'  and pay_material_details.month='" + month + "' and pay_material_details.year = '" + year + "' ";
        }
        else if (unit_code == "ALL")
        {
            where = " pay_material_details.comp_code='" + Session["comp_code"].ToString() + "' and pay_material_details.client_code= '" + ddl_client.SelectedValue + "' and pay_material_details.state_name = '" + state_name + "'  and pay_material_details.month='" + month + "' and pay_material_details.year = '" + year + "' ";

        }

        string designation = "";
        if (material_type == "2")//2-TissueBill
        {
            designation = "Tissue";
        }

        string start_date_common = get_start_date(month_m, year_y), where_fix = "", where_clause = "";

        string daterange = "concat(upper(DATE_FORMAT(str_to_date('" + year + "-" + month + "-01','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(LAST_DAY('" + year + "-" + month + "-01'), '%d %b %Y'))) as start_end_date";


        if (start_date_common != "" && start_date_common != "1")
        {
            daterange = "concat(upper(DATE_FORMAT(str_to_date('" + (month_i == 1 ? year - 1 : year) + "-" + (month_i == 1 ? 12 : month_i - 1) + "-" + start_date_common + "','%Y-%m-%d'), '%D %b %Y')),' TO ',upper(DATE_FORMAT(str_to_date('" + year_y + "-" + month + "-" + (int.Parse(start_date_common) - 1) + "','%Y-%m-%d'), '%D %b %Y'))) as start_end_date";
        }

        string where1 = "", emp = "";
        DateTimeFormatInfo mfi = new DateTimeFormatInfo();
        string month_name = mfi.GetMonthName(month_i).ToString();

        where_clause = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + unit_code + "' and month='" + month + "' and year = '" + year + "'  ";
        if (d.getsinglestring("select max(material_contract) from pay_billing_material_history   " + where_clause + " limit  1").Equals("4"))
        { emp = ",emp_code"; }
        where_fix = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_material_history.unit_code = '" + unit_code + "' AND pay_billing_material_history.month = '" + month + "' AND pay_billing_material_history.Year = '" + year + "' ";
        where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and unit_code = '" + unit_code + "' and month = '" + month + "' and Year = '" + year + "' and tot_days_present > 0  and material_contract != 0 AND grade_code in ('HK','HKSR','CT') GROUP BY unit_code,designation " + emp + "  ORDER BY STATE_NAME, UNIT_NAME";
        where1 = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and unit_code = '" + unit_code + "' and month = '" + month + "' and Year = '" + year + "' and tot_days_present > 0  and material_contract != 0 and grade_code in ('HK','HKSR','CT') ";

        string gst_to_be = d.getsinglestring("  select  DISTINCT (Gst_to_be) as 'Gst_to_be' from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + ddl_client.SelectedValue + "'");

        if (state_name == "ALL")
        {
            where_clause = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "'  and month='" + month + "' and year = '" + year + "'  ";
            if (d.getsinglestring("select max(material_contract) from pay_billing_material_history   " + where_clause + " limit  1").Equals("4"))
            { emp = ",emp_code"; }
            where_fix = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "'  AND pay_billing_material_history.month = '" + month + "' AND pay_billing_material_history.Year = '" + year + "' ";
            where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and month = '" + month + "' and Year = '" + year + "' and tot_days_present > 0 and material_contract != 0 AND grade_code in ('HK','HKSR','CT') GROUP BY unit_code,designation" + emp + " ORDER BY STATE_NAME, UNIT_NAME";
            where1 = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and month = '" + month + "' and Year = '" + year + "' and tot_days_present > 0 and material_contract != 0 and grade_code in ('HK','HKSR','CT') ";
        }
        else if (unit_code == "ALL")
        {
            where_clause = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name='" + state_name + "' and month='" + month + "' and year = '" + year + "'  ";
            if (d.getsinglestring("select max(material_contract) from pay_billing_material_history   " + where_clause + " limit  1").Equals("4"))
            { emp = ",emp_code"; }
            where_fix = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_material_history.state_name = '" + state_name + "' AND pay_billing_material_history.month = '" + month + "' AND pay_billing_material_history.Year = '" + year + "' ";
            where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + state_name + "' and month = '" + month + "' and Year = '" + year + "' and tot_days_present > 0 and material_contract != 0 AND grade_code in ('HK','HKSR','CT') GROUP BY unit_code,designation" + emp + "  ORDER BY STATE_NAME, UNIT_NAME ";
            where1 = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + state_name + "' and month = '" + month + "' and Year = '" + year + "' and tot_days_present > 0 and material_contract != 0 and grade_code in ('HK','HKSR','CT') ";
        }
        if (ddl_client.SelectedValue == "SUN")
        {
            crystalReport.Load(Server.MapPath("~/client_bill_invoice_sungard.rpt"));
            query_con = "SELECT comp_code, COMPANY_NAME, ADDRESS1, ADDRESS2, CITY, STATE, PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, STATE_NAME, UNIT_ADD1, UNIT_ADD2, UNIT_CITY, UNIT_NAME, other, unit_gst_no, start_end_date, month, housekeeiing_sac_code, Security_sac_code, unit_code, bill_amount as 'total', month_days, year,	equmental_unit, equmental_rental_rate, chemical_unit, chemical_consumables_rate, dustbin_unit, dustbin_liners_rate, femina_unit, femina_hygiene_rate, aerosol_unit, aerosol_dispenser_rate FROM (SELECT pay_company_master.comp_code, COMPANY_NAME, pay_company_master.ADDRESS1, pay_company_master.ADDRESS2, pay_company_master.CITY, pay_company_master.STATE, PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, UNIT_NAME, STATE_NAME, (SELECT field1 FROM pay_zone_master WHERE pay_zone_master.client_code = '" + ddl_client.SelectedValue + "' AND pay_zone_master.region = '" + state_name + "' AND comp_code = '" + Session["COMP_CODE"].ToString() + "' AND type = 'GST') AS 'UNIT_ADD1', UNIT_ADD2, UNIT_CITY, CLIENT_NAME AS 'other',(SELECT field2 FROM pay_zone_master WHERE pay_zone_master.client_code = '" + ddl_client.SelectedValue + "' and pay_zone_master.region = '" + state_name + "' and type = 'GST') AS 'unit_gst_no', " + daterange + ",  concat('" + month_name + "',' ' ,'" + year + "') AS 'month', housekeeiing_sac_code, Security_sac_code, pay_unit_master.unit_code, bill_amount ,(pay_unit_master.emp_count * month_days) AS 'month_days',(SELECT SUM(tot_days_present) FROM pay_attendance_muster WHERE pay_attendance_muster.comp_code = pay_billing_master_history.comp_code AND pay_attendance_muster.unit_code = pay_billing_master_history.billing_unit_code AND pay_billing_master_history.month = pay_attendance_muster.month AND pay_billing_master_history.year = pay_attendance_muster.year) AS 'year',equmental_unit, equmental_rental_rate, chemical_unit, chemical_consumables_rate, dustbin_unit, dustbin_liners_rate, femina_unit, femina_hygiene_rate, aerosol_unit, aerosol_dispenser_rate FROM pay_employee_master INNER JOIN pay_attendance_muster ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_muster.unit_code = pay_unit_master.unit_code AND pay_attendance_muster.comp_code = pay_unit_master.comp_code INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code INNER JOIN pay_billing_unit_rate ON pay_attendance_muster.comp_code = pay_billing_unit_rate.comp_code AND pay_attendance_muster.unit_code = pay_billing_unit_rate.unit_code AND pay_employee_master.grade_code = pay_billing_unit_rate.designation AND pay_attendance_muster.month = pay_billing_unit_rate.month AND pay_attendance_muster.year = pay_billing_unit_rate.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_attendance_muster.unit_code AND pay_billing_master_history.month = pay_attendance_muster.month AND pay_billing_master_history.year = pay_attendance_muster.year AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.designation = pay_billing_unit_rate.designation INNER JOIN pay_client_master ON pay_client_master.comp_code = pay_company_master.comp_code AND pay_client_master.client_code = pay_unit_master.client_code where " + where;

        }
        if (ddl_client.SelectedValue == "RCPL")
        {

            if (!d.getsinglestring("select sum(percent) from pay_company_group where comp_code = '" + Session["COMP_CODE"].ToString() + "' AND client_code = '" + ddl_client.SelectedValue + "'").Equals(100))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Total percent is not 100%');", true);
                // return;
            }

            if (state_name == "ALL")
            {
                crystalReport.Load(Server.MapPath("~/client_bill_invoice_RG.rpt"));

            }
            else
            {
                crystalReport.Load(Server.MapPath("~/client_material_invoice_RG_unit.rpt"));
            }
            query_con = "SELECT IF(invoice_flag != 0, DATE_FORMAT(billing_date, '%d/%m/%Y'), '') AS 'bill_date', pay_billing_unit_rate_history.comp_code, client AS 'other', COMPANY_NAME, COMP_ADDRESS1 AS 'ADDRESS1', COMP_ADDRESS2 AS 'ADDRESS2', COMP_CITY AS 'CITY', COMP_STATE AS 'STATE', PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, state_name AS 'STATE_NAME', fromtodate AS 'start_end_date', grade_desc AS 'designation',  Amount AS 'total', bill_amount AS 'equmental_handling_percent', CONCAT('" + month_name + "', ' ', '" + year + "') AS 'month', '998519' AS 'housekeeiing_sac_code', Security_sac_code, state_per AS 'tool_unit', companyname_gst_no AS 'unit_gst_no', IF(invoice_flag != 0, auto_invoice_no, '') AS 'Expr1', gst_address AS 'UNIT_ADD1', handling_per_amount as tool_handling_percent  , machine_rental_amount  as equmental_rental_rate,handling_percent as hrs_12_ot,unit_name,invoice_shipping_address AS 'UNIT_ADD2' FROM pay_billing_material_history AS pay_billing_unit_rate_history  LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_unit_rate_history.comp_code AND pay_client_billing_details.client_code = pay_billing_unit_rate_history.client_code AND pay_client_billing_details.STATE = pay_billing_unit_rate_history.state_name AND billing_name = 'Manpower Billing' WHERE pay_billing_unit_rate_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_unit_rate_history.state_name = '" + state_name + "' AND pay_billing_unit_rate_history.unit_code = '" + unit_code + "' AND pay_billing_unit_rate_history.month = '" + month + "' AND pay_billing_unit_rate_history.Year = '" + year + "' AND (emp_code = '' OR emp_code IS NULL)  GROUP BY pay_billing_unit_rate_history.auto_invoice_no ORDER BY pay_billing_unit_rate_history.auto_invoice_no";

        }
        else
        {
            if (d.getsinglestring("select max(material_contract) from pay_billing_material_history   " + where_clause + " limit  1").Equals("3"))
            {
                crystalReport.Load(Server.MapPath("~/material_fix_bill_invoice.rpt"));
                query_con = "SELECT pay_billing_material_history.comp_code,  pay_billing_material_history.unit_code  , COMPANY_NAME, COMP_ADDRESS1 AS 'ADDRESS1', COMP_ADDRESS2 AS 'ADDRESS2', COMP_CITY AS 'CITY', COMP_STATE AS 'STATE', PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, pay_billing_material_history.STATE_NAME, UNIT_full_ADD1 AS 'UNIT_ADD1', UNIT_ADD2, UNIT_CITY, UNIT_NAME, client AS 'other', unit_gst_no, grade_desc AS 'designation',  fromtodate AS 'start_end_date', concat('" + month_name + "',' ' ,'" + year + "') AS 'month', housekeeiing_sac_code, Security_sac_code, material_name as 'tool_unit',  rate as 'hrs_12_ot', quantity as 'grand_total', ROUND(rate * quantity, 2) AS 'total', CASE WHEN pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0 THEN ROUND((((rate * quantity) * pay_material_billing_details.handling_percent) / 100),2) WHEN pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_charges_amount > 0 THEN handling_charges_amount ELSE 0 END AS 'tool_handling_percent', pay_material_billing_details.handling_percent as 'Expr1', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME), IF(material_contract = 3, ROUND(((rate * quantity) + IF(material_contract = 3 AND pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0, ROUND(((rate * quantity) * pay_material_billing_details.handling_percent) / 100), handling_charges_amount)) * 9, 2) / 100, 0), 0) AS 'SGST', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME), IF(material_contract = 3, ROUND(((rate * quantity) + IF(material_contract = 3 AND pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0, ROUND(((rate * quantity) * pay_material_billing_details.handling_percent) / 100), handling_charges_amount)) * 9, 2) / 100, 0), 0) AS 'CGST', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME) != 1, IF(material_contract = 3, ROUND(((rate * quantity) + IF(material_contract = 3 AND pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0, ROUND(((rate * quantity) * pay_material_billing_details.handling_percent) / 100), handling_charges_amount)) * 18, 2) / 100, 0), 0) AS 'IGST',machine_rental_applicable as tool_applicable,machine_rental_amount as tool_unit FROM pay_billing_material_history INNER JOIN pay_material_billing_details ON pay_billing_material_history.comp_code = pay_material_billing_details.comp_Code AND pay_billing_material_history.client_code = pay_material_billing_details.client_code AND pay_billing_material_history.state_name = pay_material_billing_details.state AND pay_billing_material_history.unit_code = pay_material_billing_details.unit_code1 AND pay_billing_material_history.month = pay_material_billing_details.month AND pay_billing_material_history.year = pay_material_billing_details.year WHERE " + where_fix + " AND pay_billing_material_history.tot_days_present > 0 AND pay_billing_material_history.material_contract = 3 AND grade_code = 'HK' GROUP BY pay_billing_material_history.unit_code, Id_material ORDER BY 2, 3  ";
            }
            else
            {
                if (material_type == "2")//2-Tissuebill
                {
                    where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_material_history.state_name = '" + state_name + "' AND pay_billing_material_history.month = '" + month + "' AND pay_billing_material_history.Year = '" + year + "' and  pay_billing_material_history.Type='Material' and pay_billing_material_history.material_type='Tissue' ";
                    crystalReport.Load(Server.MapPath("~/material_fix_bill_Tissue.rpt"));
                    query_con = "SELECT pay_billing_material_history.comp_code, COMPANY_NAME, COMP_ADDRESS1 AS 'ADDRESS1', COMP_ADDRESS2 AS 'ADDRESS2', COMP_CITY AS 'CITY', COMP_STATE AS 'STATE', PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, STATE_NAME,UNIT_full_ADD1 as  'UNIT_ADD1', invoice_shipping_address AS 'UNIT_ADD2', UNIT_CITY, UNIT_NAME, client AS 'other', unit_gst_no, grade_desc AS 'designation',  fromtodate AS 'start_end_date', concat('" + month_name + "',' ' ,'" + year + "') AS 'month', housekeeiing_sac_code, Security_sac_code, tissue_rate AS 'grand_total', ROUND((tissue_qty * tissue_rate), 2) AS 'total', IF(handling_applicable = 1, handling_percent, 0) AS 'Expr1', tissue_qty AS 'hrs_12_ot', unit_code, handling_charges_amount as equmental_rental_rate,machine_rental_applicable as tool_applicable,machine_rental_amount as tool_unit,if(pay_billing_material_history.comp_code = 'C02','Ranchi','Pune') as 'aerosol_unit',po_no as 'type' FROM pay_billing_material_history LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_material_history.comp_code AND pay_client_billing_details.client_code = pay_billing_material_history.client_code AND pay_client_billing_details.STATE = pay_billing_material_history.state_name AND billing_name = 'Material Billing' WHERE " + where;

                }
                else
                {
                    crystalReport.Load(Server.MapPath("~/material_fix_bill.rpt"));
                    query_con = "SELECT pay_billing_material_history.comp_code, COMPANY_NAME, COMP_ADDRESS1 AS 'ADDRESS1', COMP_ADDRESS2 AS 'ADDRESS2', COMP_CITY AS 'CITY', COMP_STATE AS 'STATE', PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, STATE_NAME,UNIT_full_ADD1 as  'UNIT_ADD1', invoice_shipping_address AS 'UNIT_ADD2', UNIT_CITY, UNIT_NAME, client AS 'other', unit_gst_no, grade_desc AS 'designation',  fromtodate AS 'start_end_date', concat('" + month_name + "',' ' ,'" + year + "') AS 'month', housekeeiing_sac_code, Security_sac_code, IF(material_contract != 0, contract_amount, 0) AS 'grand_total', IF(material_contract != 0 AND contract_type = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) AS 'total', IF(handling_applicable = 1, handling_percent, 0) AS 'Expr1', material_area AS 'hrs_12_ot', unit_code, handling_charges_amount as equmental_rental_rate,machine_rental_applicable as tool_applicable,machine_rental_amount as tool_unit,if(pay_billing_material_history.comp_code = 'C02','Ranchi','Pune') as 'aerosol_unit',po_no as 'type' FROM pay_billing_material_history LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_material_history.comp_code AND pay_client_billing_details.client_code = pay_billing_material_history.client_code AND pay_client_billing_details.STATE = pay_billing_material_history.state_name AND billing_name = 'Material Billing' WHERE " + where + "  and pay_billing_material_history.material_type is null";

                }
            }
        }
        #endregion
        if (e_invoice == 1)
        {
            ReportLoad_e_invoice(query_con, dowmloadname, invoice, bill_date, state_name, month_m, year_y, "Material Invoice", folderPath);
        }
        else
        {
            ReportLoad_All_Invoice(query_con, dowmloadname, invoice, bill_date, state_name, month_m, year_y, "Material Invoice", folderPath);

        }
    }

    protected void DeepClean_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month, string year, string folderPath, string bill_wise)
    {
        #region
        if (unit_code == "" || bill_wise == "Statewise")
        {
            unit_code = "ALL";
        }
        if (state_name == "")
        {
            state_name = "ALL";
        }
        string start_date_common = get_start_date(month, year);
        int month2 = Convert.ToInt32(month);
        DateTimeFormatInfo mfi = new DateTimeFormatInfo();
        string month_name = mfi.GetMonthName(month2).ToString();

        string gst_to_be = d.getsinglestring("  select  DISTINCT (Gst_to_be) as 'Gst_to_be' from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'");

        string where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and unit_code = '" + unit_code + "' and month = '" + month + "' and Year = '" + year + "' and tot_days_present > 0 AND dc_contract = 1  GROUP BY unit_code, designation  ORDER BY UNIT_NAME";
        if (state_name == "ALL")
        {
            where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and month = '" + month + "' and Year = '" + year + "' and tot_days_present > 0 AND dc_contract = 1 GROUP BY unit_code, designation ORDER BY UNIT_NAME";
        }
        else if (unit_code == "ALL")
        {
            where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + state_name + "' and month = '" + month + "' and Year = '" + year + "' and tot_days_present > 0 AND dc_contract = 1 GROUP BY unit_code, designation ORDER BY UNIT_NAME";
        }

        crystalReport.Load(Server.MapPath("~/client_bill_invoice_dc.rpt"));
        string query = "SELECT  pay_billing_material_history.comp_code ,  COMPANY_NAME ,  COMP_ADDRESS1  AS 'ADDRESS1',  COMP_ADDRESS2  AS 'ADDRESS2',  COMP_CITY  AS 'CITY',  COMP_STATE  AS 'STATE',  PF_REG_NO ,  COMPANY_PAN_NO ,  COMPANY_TAN_NO ,  COMPANY_CIN_NO ,  SERVICE_TAX_REG_NO ,  ESIC_REG_NO ,  STATE_NAME ,  UNIT_full_ADD1 as  'UNIT_ADD1' ,  invoice_shipping_address AS 'UNIT_ADD2' ,  UNIT_CITY ,  UNIT_NAME ,  CLIENT  AS 'other',  unit_gst_no ,  grade_desc  AS 'designation',  fromtodate AS 'start_end_date',  concat('" + month_name + "' ,'  '," + year + ") as 'month' ,  housekeeiing_sac_code ,  Security_sac_code ,  dc_area  AS 'month_days', IF( dc_contract  = 1,  dc_rate , 0) AS 'grand_total', IF( dc_contract  = 1 AND  dc_type  = 2, ( dc_rate  *  dc_area ),  dc_rate ) AS 'total', IF( dc_handling_charge  = 1,  dc_handling_percent , 0) AS 'Expr1',  unit_code ,if(pay_billing_material_history.comp_code = 'C02','Ranchi','Pune') as 'EMP_NAME' FROM  pay_billing_material_history LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_material_history.comp_code AND pay_client_billing_details.client_code = pay_billing_material_history.client_code AND pay_client_billing_details.STATE = pay_billing_material_history.state_name AND billing_name = 'Deep Clean Billing' WHERE " + where;

        #endregion
        if (e_invoice == 1)
        {
            ReportLoad_e_invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, "Deep Clean", folderPath);
        }
        else
        {
            ReportLoad_All_Invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, "Deep Clean", folderPath);
        }

    }

    protected void Machine_rental_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month, string year, string folderPath, string type, string bill_wise, string material_tissue)
    {

        #region
        if (unit_code == "" || bill_wise == "Statewise")
        {
            unit_code = "ALL";
        }
        if (state_name == "")
        {
            state_name = "ALL";
        }

        string INV_bill_date = bill_date, directory_path = folderPath, bill_types = type;

        int month2 = Convert.ToInt32(month);

        int year3 = Convert.ToInt32(year);

        string month_m = string.Format(String.Format("{0:D2}", month2));

        int month_i = Convert.ToInt32(month_m);

        string headerpath = null;
        string footerpath = null;

        if (ddl_client.SelectedValue.Equals("RCPL"))
        {
            Material_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, bill_wise, material_tissue);
            return;
        }
        try
        {
            string daterange = "concat(upper(DATE_FORMAT(str_to_date('" + year + "-" + month_i + "-01','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(LAST_DAY('" + year + "-" + month_i + "-01'), '%d %b %Y'))) as start_end_date";

            string start_date_common = get_start_date(month, year);

            if (start_date_common != "" && start_date_common != "1")
            {
                daterange = "concat(upper(DATE_FORMAT(str_to_date('" + year + "-" + (month_i == 1 ? 12 : (month_i - 1)) + "-" + start_date_common + "','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(str_to_date('" + year + "-" + month_i + "-" + (int.Parse(start_date_common) - 1) + "','%Y-%m-%d'), '%d %b %Y'))) as start_end_date";
            }

            ReportDocument crystalReport = new ReportDocument();
            System.Data.DataTable dt = new System.Data.DataTable();
            DateTimeFormatInfo mfi = new DateTimeFormatInfo();
            string month_name = mfi.GetMonthName(month_i).ToString();

            string gst_to_be = d.getsinglestring("  select  DISTINCT (Gst_to_be) as 'Gst_to_be' from pay_unit_master where comp_code='" + Session["COMP_CODE"].ToString() + "' and client_code='" + ddl_client.SelectedValue + "'");

            string query1 = "";
            if (ddl_client.SelectedValue == "RCPL")
            {

                query1 = "SELECT IF(pay_billing_unit_rate_history.invoice_flag != 0, DATE_FORMAT(pay_billing_unit_rate_history.billing_date, '%d/%m/%Y'), '') AS 'bill_date', pay_billing_unit_rate_history.comp_code, client AS 'other', pay_billing_unit_rate_history.COMPANY_NAME, pay_billing_unit_rate_history.COMP_ADDRESS1 AS 'ADDRESS1', pay_billing_unit_rate_history.COMP_ADDRESS2 AS 'ADDRESS2', pay_billing_unit_rate_history.COMP_CITY AS 'CITY', pay_billing_unit_rate_history.COMP_STATE AS 'STATE', pay_billing_unit_rate_history.PF_REG_NO, pay_billing_unit_rate_history.COMPANY_PAN_NO, pay_billing_unit_rate_history.COMPANY_TAN_NO, pay_billing_unit_rate_history.COMPANY_CIN_NO, pay_billing_unit_rate_history.SERVICE_TAX_REG_NO, pay_billing_unit_rate_history.ESIC_REG_NO, state_name AS 'STATE_NAME', fromtodate AS 'start_end_date', grade_desc AS 'designation', Amount AS 'total', bill_amount AS 'equmental_handling_percent', CONCAT('" + month_name + "', ' ', '" + year + "') AS 'month', '998519' AS 'housekeeiing_sac_code', Security_sac_code, state_per AS 'tool_unit', pay_billing_unit_rate_history.companyname_gst_no AS 'unit_gst_no', IF(pay_billing_unit_rate_history.invoice_flag != 0, pay_billing_unit_rate_history.auto_invoice_no, '') AS 'Expr1', pay_billing_unit_rate_history.gst_address AS 'UNIT_ADD1', handling_per_amount AS 'tool_handling_percent', (pay_billing_rental_machine.total / 100) * pay_company_group.percent AS 'equmental_rental_rate', handling_percent AS 'hrs_12_ot', pay_billing_unit_rate_history.unit_name FROM pay_billing_material_history AS pay_billing_unit_rate_history INNer JOIN pay_billing_rental_machine ON pay_billing_rental_machine.client_code = pay_billing_unit_rate_history.client_code  AND pay_billing_rental_machine.comp_code = pay_billing_unit_rate_history.comp_code  AND pay_billing_rental_machine.unit_code = pay_billing_unit_rate_history.unit_code AND pay_billing_rental_machine.month = pay_billing_unit_rate_history.month AND pay_billing_rental_machine.year = pay_billing_unit_rate_history.year INNER JOIN pay_company_group  ON pay_billing_rental_machine.client_code = pay_billing_rental_machine.client_code  AND pay_billing_rental_machine.comp_code = pay_billing_rental_machine.comp_code  aND pay_billing_rental_machine.unit_code = pay_billing_rental_machine.unit_code WHERE pay_billing_unit_rate_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_unit_rate_history.state_name = '" + state_name + "' AND pay_billing_unit_rate_history.unit_code = '" + unit_code + "' AND pay_billing_unit_rate_history.month = '" + month_i + "' AND pay_billing_unit_rate_history.Year = '" + year + "' AND (emp_code = '' OR emp_code IS NULL) GROUP BY pay_company_group.comp_name ORDER BY pay_billing_unit_rate_history.auto_invoice_no ";
            }
            else if (ddl_client.SelectedValue == "MEDLINE")
            {

                query1 = "SELECT date_format(billing_date,'%d/%m/%Y') as 'month_days',auto_invoice_no as 'expr1',machine_name as 'type',CONCAT('Total',' ', rent_type) AS 'femina_unit',pay_billing_rental_machine.comp_code,COMPANY_NAME,COMP_ADDRESS1 AS 'ADDRESS1',COMP_ADDRESS2 AS 'ADDRESS2',COMP_CITY AS 'CITY',COMP_STATE AS 'STATE',PF_REG_NO,COMPANY_PAN_NO,COMPANY_TAN_NO,COMPANY_CIN_NO,SERVICE_TAX_REG_NO,ESIC_REG_NO, state as STATE_NAME,UNIT_full_ADD1 AS 'UNIT_ADD1',UNIT_ADD2,UNIT_CITY,UNIT_NAME,client_name AS 'other',unit_gst_no," + daterange + ",concat('" + month_name + "',' ' ,'" + year + "') AS 'month','' as housekeeiing_sac_code,'' as  Security_sac_code,'' AS 'grand_total','' AS 'Expr1','' AS 'hrs_12_ot'  ,unit_code,CASE WHEN handling_per > '0' THEN CONCAT(handling_per, '%') WHEN handling_amount > '0' THEN CONCAT(handling_amount, ' Amount') ELSE '' END AS 'chemical_unit', rent * qty AS 'total', CASE WHEN handling_per > '0' THEN (rent * qty) * (handling_per) / 100 WHEN handling_amount > '0' THEN handling_amount ELSE '0' END AS 'equmental_rental_rate',total as 'tool_unit', qty  AS 'emp_name', rent  AS 'equmental_unit',hsn_number as 'dustbin_unit' FROM pay_billing_rental_machine INNER JOIN  pay_item_master  ON  pay_billing_rental_machine . machine_code  =  pay_item_master . ITEM_CODE  where month='" + month_i + "' and year='" + year + "' and client_code='" + ddl_client.SelectedValue + "' and unit_code='" + unit_code + "' and  pay_billing_rental_machine.comp_code='" + Session["COMP_CODE"].ToString() + "' ";
            }
            else
            {
                query1 = "SELECT date_format(billing_date,'%d/%m/%Y') as 'month_days',auto_invoice_no as 'expr1',machine_name as 'type',CONCAT('Total',' ', rent_type) AS 'femina_unit',pay_billing_rental_machine.comp_code,COMPANY_NAME,COMP_ADDRESS1 AS 'ADDRESS1',COMP_ADDRESS2 AS 'ADDRESS2',COMP_CITY AS 'CITY',COMP_STATE AS 'STATE',PF_REG_NO,COMPANY_PAN_NO,COMPANY_TAN_NO,COMPANY_CIN_NO,SERVICE_TAX_REG_NO,ESIC_REG_NO, pay_billing_rental_machine.state as STATE_NAME,UNIT_full_ADD1 AS 'UNIT_ADD1', invoice_shipping_address as 'UNIT_ADD2',UNIT_CITY,UNIT_NAME,client_name AS 'other',unit_gst_no," + daterange + ",concat('" + month_name + "',' ' ,'" + year + "') AS 'month','' as housekeeiing_sac_code,'' as  Security_sac_code,'' AS 'grand_total','' AS 'Expr1','' AS 'hrs_12_ot'  ,unit_code,CASE WHEN handling_per > '0' THEN CONCAT(handling_per, '%') WHEN handling_amount > '0' THEN CONCAT(handling_amount, ' Amount') ELSE '' END AS 'chemical_unit', rent * qty AS 'total', CASE WHEN handling_per > '0' THEN (rent * qty) * (handling_per) / 100 WHEN handling_amount > '0' THEN handling_amount ELSE '0' END AS 'equmental_rental_rate',total as 'tool_unit', qty  AS 'emp_name', rent  AS 'equmental_unit',if(pay_billing_rental_machine.comp_code = 'C02','Ranchi','Pune') as 'Emp_code' FROM pay_billing_rental_machine LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_rental_machine.comp_code AND pay_client_billing_details.client_code = pay_billing_rental_machine.client_code AND pay_client_billing_details.STATE = pay_billing_rental_machine.state AND billing_name = 'Machine Rental' where month='" + month_i + "' and year='" + year + "' and pay_billing_rental_machine.client_code='" + ddl_client.SelectedValue + "' and unit_code='" + unit_code + "' and  pay_billing_rental_machine.comp_code='" + Session["COMP_CODE"].ToString() + "' group by type";

            }
            MySqlCommand cmd = new MySqlCommand(query1, d.con);
            MySqlDataReader sda = null;
            d.con.Open();
            try
            {
                sda = cmd.ExecuteReader();
                dt.Load(sda);

            }
            catch (Exception ex) { throw ex; }
            if (ddl_client.SelectedValue == "RCPL")
            {
                crystalReport.Load(Server.MapPath("~/client_material_invoice_RG_unit.rpt"));
            }
            else
            {
                if (ddl_client.SelectedValue == "MEDLINE")
                {
                    crystalReport.Load(Server.MapPath("~/machine_fix_bill_medline.rpt"));
                }
                else if (ddl_client.SelectedValue == "BAGICTM")
                {
                    crystalReport.Load(Server.MapPath("~/machine_fix_bill_laptop.rpt"));
                }
                else
                {
                    crystalReport.Load(Server.MapPath("~/machine_fix_bill.rpt"));
                }

                if (Session["COMP_CODE"].ToString() == "C02")
                {
                    headerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C02_header.png");
                    footerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C02_footer.png");
                    crystalReport.DataDefinition.FormulaFields["headerimagepath1"].Text = @"'" + headerpath + "'";
                    crystalReport.DataDefinition.FormulaFields["footerimagepath"].Text = @"'" + footerpath + "'";
                }
                else
                {
                    headerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C01_header.png");
                    footerpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C01_footer.png");
                    crystalReport.DataDefinition.FormulaFields["headerimagepath1"].Text = @"'" + headerpath + "'";
                    crystalReport.DataDefinition.FormulaFields["footerimagepath"].Text = @"'" + footerpath + "'";
                }
            }

            PageMargins margins;
            margins = crystalReport.PrintOptions.PageMargins;
            margins.bottomMargin = 0;
            margins.leftMargin = 350;
            margins.rightMargin = 0;
            margins.topMargin = 0;
            crystalReport.PrintOptions.ApplyPageMargins(margins);
            crystalReport.SetDataSource(dt);
            crystalReport.Refresh();
            string file_name = "";

            file_name = invoice + "_" + bill_types + ".pdf";

            string filepath = directory_path + "\\" + file_name;

            string Invpath = directory_path;


            if (INV_bill_date != "")
            {
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, filepath);
                ViewState["zip_path"] = directory_path;
            }
            else
            {
                //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, downloadname);
            }

            ViewState["ALL_STATE"] = "0";
        }
        catch { }
        finally
        {
            d.con.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        #endregion
    }

    protected void R_and_M_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month, string year, string folderPath, string type, string bill_wise)
    {

        #region
        if (unit_code == "" || bill_wise == "Statewise")
        {
            unit_code = "ALL";
        }
        if (state_name == "" )
        {
            state_name = "ALL";
        }

        string start_date = get_start_date(month, year);

        string txt_month_year1 = "";

        string invoice_type = "CLUB";

        string ddl_invoice_slot = d1.getsinglestring("select distinct invoice_slot from pay_billing_r_m where auto_invoice_no='" + invoice + "'");

        txt_month_year1 = month + "/" + year;

        string query = null, sql = null, where = "", delete_where = "", group_by = "";

        month = "" + month + "";

        string firstday = "01/" + txt_month_year1;

        string month_name = getmonth(month);

        string invoice_month_name = "concat('" + month_name + "',' ' ,'" + year + "')";

        where = "  pay_billing_r_m.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_r_m.client_code = '" + ddl_client.SelectedValue + "'  and pay_billing_r_m.month = '" + month + "' and pay_billing_r_m.Year = '" + year + "' and pay_billing_r_m.invoice_slot = '" + ddl_invoice_slot + "' ";

        delete_where = "  comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'  and month = '" + month + "' and Year = '" + year + "' ";

        group_by = " group by pay_billing_r_m.client_code";

        string gst_to_be = d.getsinglestring("select  DISTINCT (Gst_to_be) as 'Gst_to_be' from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + ddl_client.SelectedValue + "'");

        crystalReport.Load(Server.MapPath("~/client_bill_invoice_r_and_m_RLIC.rpt"));

        query = "SELECT  pay_billing_r_m.client_code, pay_billing_r_m.comp_code, case when pay_billing_r_m.client_code = 'RNLIC RM'  then 'RELIANCE NIPPON LIFE INSURANCE CO. LTD.' else client end AS 'other', COMPANY_NAME, COMP_ADDRESS1 AS 'ADDRESS1', COMP_ADDRESS2 AS 'ADDRESS2', COMP_CITY AS 'CITY', COMP_STATE AS 'STATE', PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, state_name AS 'STATE_NAME', UNIT_full_ADD1 AS 'UNIT_ADD1', invoice_shipping_address AS 'UNIT_ADD2', unit_city AS 'UNIT_CITY', unit_gst_no, fromtodate AS 'start_end_date', (SUM(ROUND((amount) + (Service_charge), 2))) AS 'total', IFNULL(SUM(SGST9), 0) AS 'SGST', IFNULL(SUM(CGST9), 0) AS 'CGST', IFNULL(SUM(IGST18), 0) AS 'IGST', " + invoice_month_name + " AS 'month', housekeeiing_sac_code, Security_sac_code, unit_code, CAST(gst_applicable AS CHAR) AS 'ZONE' FROM pay_billing_r_m LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_r_m.comp_code AND pay_client_billing_details.client_code = pay_billing_r_m.client_code AND pay_client_billing_details.STATE = pay_billing_r_m.state_name AND billing_name = 'R And M Service' where " + where + group_by;

        #endregion
        if (e_invoice == 1)
        {
            ReportLoad_e_invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, type, folderPath);
        }
        else
        {
            ReportLoad_All_Invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, type, folderPath);
        }


    }

    protected void Administrative_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month_m, string year_y, string folderPath, string type, string bill_wise)
    {

        #region
        if (unit_code == "" || bill_wise == "Statewise")
        {
            unit_code = "ALL";
        }
        if (state_name == "")
        {
            state_name = "ALL";
        }

        string invoice_type = type;

        string start_date = get_start_date(month_m, year_y);

        string txt_month_year1 = month_m + "/" + year_y;

        string query = null, where = "", delete_where = "", group_by = "";

        string a = txt_month_year.Text;

        string firstday = "01/" + txt_month_year1;

        string month = "" + month_m + "";

        string year = "" + year_y + "";

        string month_name = getmonth(month);

        string invoice_month_name = "concat('" + month_name + "',' ' ,'" + year + "')";


        where = "  pay_billing_admini_expense.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_admini_expense.client_code = '" + ddl_client.SelectedValue + "'  and pay_billing_admini_expense.month = '" + month_m + "' and pay_billing_admini_expense.Year = '" + year_y + "' ";

        delete_where = "  comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'  and month = '" + month_m + "' and Year = '" + year_y + "' ";

        group_by = " group by pay_billing_admini_expense.client_code";

        string gst_to_be = d.getsinglestring("  select  DISTINCT (Gst_to_be) as 'Gst_to_be' from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + ddl_client.SelectedValue + "'");

        crystalReport.Load(Server.MapPath("~/client_bill_invoice_administrative_RLIC.rpt"));

        query = "SELECT  pay_billing_admini_expense.client_code, pay_billing_admini_expense.comp_code,  case when pay_billing_admini_expense.client_code = 'RNLIC RM'  then 'RELIANCE NIPPON LIFE INSURANCE CO. LTD.' else client end AS 'other', COMPANY_NAME, COMP_ADDRESS1 AS 'ADDRESS1', COMP_ADDRESS2 AS 'ADDRESS2', COMP_CITY AS 'CITY', COMP_STATE AS 'STATE', PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, state_name AS 'STATE_NAME', UNIT_full_ADD1 AS 'UNIT_ADD1', invoice_shipping_address AS 'UNIT_ADD2', unit_city AS 'UNIT_CITY', unit_gst_no, fromtodate AS 'start_end_date', (SUM(ROUND((amount) + (Service_charge), 2))) AS 'total', IFNULL(SUM(SGST9), 0) AS 'SGST', IFNULL(SUM(CGST9), 0) AS 'CGST', IFNULL(SUM(IGST18), 0) AS 'IGST', " + invoice_month_name + " AS 'month', housekeeiing_sac_code, Security_sac_code, unit_code, CAST(gst_applicable AS CHAR) AS 'ZONE' FROM pay_billing_admini_expense LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_admini_expense.comp_code AND pay_client_billing_details.client_code = pay_billing_admini_expense.client_code AND pay_client_billing_details.STATE = pay_billing_admini_expense.state_name AND billing_name = 'Administrative Expenses' where " + where + group_by;


        #endregion
        if (e_invoice == 1)
        {
            ReportLoad_e_invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, type, folderPath);
        }
        else
        {
            ReportLoad_All_Invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, type, folderPath);

        }

    }

    protected void shiftwise_bill_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month_m, string year_y, string folderPath, string type, string bill_wise)
    {

        #region
        if (unit_code == "" || bill_wise == "Statewise")
        {
            unit_code = "ALL";
        }
        if (state_name == "")
        {
            state_name = "ALL";
        }

        string invoice_type = type;

        string start_date = get_start_date(month_m, year_y);

        string txt_month_year1 = month_m + "/" + year_y;

        string query = null, where = "", where1 = "", delete_where = "";
        string a = txt_month_year1;
        string firstday = "01/" + txt_month_year1;
        string month = "" + month_m + "";
        string year = "" + year_y + "";
        string month_name = getmonth(month);
        string invoice_month_name = "concat('" + month_name + "',' ' ,'" + year + "')";

        delete_where = "  comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'  and month = '" + month + "' and Year = '" + year + "' and state_name = '" + state_name + "'";

        where = "pay_billing_shiftwise.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.state_name = '" + state_name + "'  and pay_billing_shiftwise.unit_code = '" + ddl_unitcode.SelectedValue + "'   and pay_billing_shiftwise.month = '" + month + "' and pay_billing_shiftwise.Year = '" + year + "' and pay_billing_shiftwise.shift_days > 0      GROUP BY pay_billing_shiftwise.state_name,pay_billing_shiftwise.unit_code";
        where1 = "pay_billing_shiftwise.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.state_name = '" + state_name + "'  and pay_billing_shiftwise.unit_code = '" + ddl_unitcode.SelectedValue + "' and pay_billing_shiftwise.month = '" + month + "' and pay_billing_shiftwise.Year = '" + year + "' and pay_billing_shiftwise.shift_days > 0      GROUP BY pay_billing_shiftwise.state_name";
        if (state_name == "ALL")
        {
            where = "pay_billing_shiftwise.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.month = '" + month + "' and pay_billing_shiftwise.Year = '" + year + "'  and pay_billing_shiftwise.shift_days > 0      GROUP BY pay_billing_shiftwise.state_name,pay_billing_shiftwise.unit_code";
            where1 = "pay_billing_shiftwise.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.month = '" + month + "' and pay_billing_shiftwise.Year = '" + year + "'   and pay_billing_shiftwise.shift_days > 0      GROUP BY pay_billing_shiftwise.state_name";

        }
        else if (unit_code == "ALL")
        {
            where = "pay_billing_shiftwise.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.state_name = '" + state_name + "' and pay_billing_shiftwise.month = '" + month + "'  and pay_billing_shiftwise.Year = '" + year + "' and pay_billing_shiftwise.shift_days > 0   GROUP BY pay_billing_shiftwise.state_name,pay_billing_shiftwise.unit_code";
            where1 = "pay_billing_shiftwise.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.state_name = '" + state_name + "' and pay_billing_shiftwise.month = '" + month + "' and pay_billing_shiftwise.Year = '" + year + "' and pay_billing_shiftwise.shift_days > 0   GROUP BY pay_billing_shiftwise.state_name";
        }
        string gst_to_be = d.getsinglestring("  select  DISTINCT (`Gst_to_be`) as 'Gst_to_be' from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + ddl_client.SelectedValue + "'");

        crystalReport.Load(Server.MapPath("~/shiftwise_invoice.rpt"));

        query = "SELECT  pay_billing_shiftwise.comp_code, client AS 'other', COMPANY_NAME, COMP_ADDRESS1 AS 'ADDRESS1', COMP_ADDRESS2 AS 'ADDRESS2', COMP_CITY AS 'CITY', COMP_STATE AS 'STATE', PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, state_name AS 'STATE_NAME', UNIT_full_ADD1 AS 'UNIT_ADD1', invoice_shipping_address AS UNIT_ADD2, unit_city AS 'UNIT_CITY', unit_gst_no AS 'unit_gst_no', grade_desc AS 'designation', fromtodate AS 'start_end_date', SUM(shift_days) AS 'TOT_DAYS_PRESENT', month_days AS 'month_days', (SUM(ROUND(amount))) AS 'grand_total', (SUM(ROUND((amount) + (Service_charge), 2))) AS 'total', ROUND(SUM(CGST9), 2) AS CGST, ROUND(SUM(SGST9), 2) AS SGST, ROUND(SUM(IGST18), 2) AS IGST,  " + invoice_month_name + " AS 'month', housekeeiing_sac_code, Security_sac_code, unit_name, unit_code, IF(billing_gst_applicable = 1, (SUM(Amount) + SUM(Service_charge)), 0) AS 'hrs_12_ot','Shift Count' as 'zone' FROM pay_billing_shiftwise LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_shiftwise.comp_code AND pay_client_billing_details.client_code = pay_billing_shiftwise.client_code AND pay_client_billing_details.STATE = pay_billing_shiftwise.state_name AND billing_name = 'Shiftwise Billing' WHERE  " + where;

        #endregion
        if (e_invoice == 1)
        {
            ReportLoad_e_invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, type, folderPath);
        }
        else
        {
            ReportLoad_All_Invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, type, folderPath);

        }

    }

    protected void office_rent_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month_m, string year_y, string folderPath, string type, string bill_wise)
    {

        #region
        if (unit_code == "" || bill_wise == "Statewise")
        {
            unit_code = "ALL";
        }
        if (state_name == "")
        {
            state_name = "ALL";
        }

        string invoice_type = type;

        string start_date = get_start_date(month_m, year_y);

        string txt_month_year1 = month_m + "/" + year_y;

        string query = null, where = "", where1 = "", delete_where = "";
        string a = txt_month_year.Text;
        string firstday = "01/" + txt_month_year.Text;
        string month = "" + month_m + "";
        string year = "" + year_y + "";
        string month_name = getmonth(month);
        string invoice_month_name = "concat('" + month_name + "',' ' ,'" + year + "')";


        delete_where = "  comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'  and month = '" + month + "' and Year = '" + year + "' and state_name = '" + state_name + "' and unit_code = '" + unit_code + "'";

        where = "pay_billing_office_rent.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_office_rent.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_office_rent.state_name = '" + state_name + "'  and pay_billing_office_rent.unit_code = '" + unit_code + "'   and pay_billing_office_rent.month = '" + month + "' and pay_billing_office_rent.Year = '" + year + "'      GROUP BY pay_billing_office_rent.unit_code";
        where1 = "pay_billing_office_rent.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_office_rent.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_office_rent.state_name = '" + state_name + "'  and pay_billing_office_rent.unit_code = '" + unit_code + "' and pay_billing_office_rent.month = '" + month + "' and pay_billing_office_rent.Year = '" + year + "'      GROUP BY pay_billing_office_rent.unit_code";

        if (state_name == "ALL")
        {
            delete_where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and month = '" + month + "' and Year = '" + year + "'";
            where = "pay_billing_office_rent.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_office_rent.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_office_rent.month = '" + month + "' and pay_billing_office_rent.Year = '" + year + "'       GROUP BY pay_billing_office_rent.unit_code";
            where1 = "pay_billing_office_rent.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_office_rent.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_office_rent.month = '" + month + "' and pay_billing_office_rent.Year = '" + year + "'        GROUP BY pay_billing_office_rent.unit_code";

        }

        else if (unit_code == "ALL")
        {
            delete_where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + state_name + "' and month = '" + month + "'  and Year = '" + year + "'";
            where = "pay_billing_office_rent.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_office_rent.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_office_rent.state_name = '" + state_name + "' and pay_billing_office_rent.month = '" + month + "'  and pay_billing_office_rent.Year = '" + year + "'    GROUP BY pay_billing_office_rent.unit_code";
            where1 = "pay_billing_office_rent.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_office_rent.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_office_rent.state_name = '" + state_name + "' and pay_billing_office_rent.month = '" + month + "' and pay_billing_office_rent.Year = '" + year + "'   GROUP BY pay_billing_office_rent.unit_code";
        }
        string gst_to_be = d.getsinglestring("  select  DISTINCT (Gst_to_be) as 'Gst_to_be' from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + ddl_client.SelectedValue + "'");

        crystalReport.Load(Server.MapPath("~/office_rent_invoice.rpt"));

        query = "SELECT  pay_billing_office_rent.comp_code, client AS 'other', COMPANY_NAME, COMP_ADDRESS1 AS 'ADDRESS1', COMP_ADDRESS2 AS 'ADDRESS2', COMP_CITY AS 'CITY', COMP_STATE AS 'STATE', PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, state_name AS 'STATE_NAME', UNIT_full_ADD1 AS 'UNIT_ADD1', invoice_shipping_address AS UNIT_ADD2, unit_city AS 'UNIT_CITY', unit_gst_no AS 'unit_gst_no', grade_desc AS 'designation', fromtodate AS 'start_end_date', (SUM(ROUND(amount))) AS 'grand_total', (SUM(ROUND((amount) + (Service_charge), 2))) AS 'total', ROUND(SUM(CGST9), 2) AS CGST, ROUND(SUM(SGST9), 2) AS SGST, ROUND(SUM(IGST18), 2) AS IGST,  " + invoice_month_name + " AS 'month', housekeeiing_sac_code, Security_sac_code, unit_name, unit_code, IF(billing_gst_applicable = 1, (SUM(Amount) + SUM(Service_charge)), 0) AS 'hrs_12_ot' FROM pay_billing_office_rent LEFT JOIN pay_client_billing_details ON pay_client_billing_details.comp_code = pay_billing_office_rent.comp_code AND pay_client_billing_details.client_code = pay_billing_office_rent.client_code AND pay_client_billing_details.STATE = pay_billing_office_rent.state_name AND billing_name = 'Office Rent Billing' WHERE  " + where;

        #endregion
        if (e_invoice == 1)
        {
            ReportLoad_e_invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, type, folderPath);
        }
        else
        {
            ReportLoad_All_Invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, type, folderPath);

        }

    }

    protected void incentive_bill_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month_m, string year_y, string folderPath, string type)
    {
        #region
        if (unit_code == "")
        {
            unit_code = "ALL";
        }
        if (state_name == "")
        {
            state_name = "ALL";
        }


        //ReportLoad_All_Invoice(query, dowmloadname, invoice, bill_date, state_name, month, year, type, folderPath);
        #endregion
    }
    //END
    protected void btn_view_einv_Click(object sender, EventArgs e)
    {
        hidtab.Value = "5";
        Check_for_e_inv();
    }

    private void Check_for_e_inv()
    {
        string query = "";
       
        string where_client = "";
        try
        {

            if (gst_to_date.Text!="" && gst_from_date.Text!="")
            {
            string to_date = gst_to_date.Text;
            DateTime toDate = DateTime.ParseExact(to_date, "dd/MM/yyyy",System.Globalization.CultureInfo.InvariantCulture);
            string from_date = gst_from_date.Text;
            DateTime fromDate = DateTime.ParseExact(from_date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                if (fromDate < DateTime.Today.AddDays(-40))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Selected From Date is not applicable E-invoice..!')", true);
                    return;
                }

            if (toDate > DateTime.Today)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Selected date is not applicable E-invoice.');", true);
                    return;
                }
               else
                {
            if (ddl_client.SelectedValue != "Select")
            {
                if (ddl_client.SelectedValue != "ALL")
                {
                    where_client = " and client_code='" + ddl_client.SelectedValue + "'";
                }
                query = "select client_name,state_name,month,year,invoice_no,date_format(invoice_date,'%d/%m/%Y') as invoice_date,ROUND(amount,2) as amount,ROUND(cgst,2) as cgst,ROUND(sgst,2) as sgst,ROUND(igst,2) as igst,ROUND(amount+cgst+sgst+igst,2) as billing_amount,type from pay_report_gst where comp_code='" + Session["comp_code"] + "' and  e_invoice_status=0  " + where_client + " AND invoice_date BETWEEN STR_TO_DATE('" + gst_from_date.Text + "','%d/%m/%Y') and str_to_date('" + gst_to_date.Text + "','%d/%m/%Y')";//and invoice_date='" + current_date + "'

                MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
                DataSet ds = new DataSet();
                dscmd.SelectCommand.CommandTimeout = 400;
                dscmd.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    gv_check_einv.DataSource = ds.Tables[0];
                    gv_check_einv.DataBind();
                    btn_einv_process.Visible = true;
                }
                else
                {
                    gv_check_einv.DataSource = null;
                    gv_check_einv.DataBind();
                    btn_einv_process.Visible = false;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('This dated invoice detail not found for E-INVOICE...!!');", true);
                    return;
                }

                //string invoice_no="", month="",  year="";
                //einvoice_process(invoice_no,month,year);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select client name.');", true);
                return;
            }
            }


            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select Correct Date for E-Invoice.');", true);
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    protected void btn_einv_process_Click(object sender, EventArgs e)
    {
        try
        {
            foreach (GridViewRow row in gv_check_einv.Rows)
            {
                var checkbox = row.FindControl("chk_record_material") as System.Web.UI.WebControls.CheckBox;
                string inv_no = "";
                inv_no = row.Cells[6].Text;

                if (checkbox.Checked == true)
                {
                    #region E-Invoice OCTA Portal ID & Password
                    try
                    {
                        System.Data.DataTable dt_einv = new System.Data.DataTable();
                        MySqlDataAdapter cmd_einv = new MySqlDataAdapter("select comp_code,E_Invoice_applicable,e_portal_id,e_portal_password from pay_company_master where comp_code='" + Session["comp_code"] + "' AND E_Invoice_applicable='YES'", d.con);
                        d.con.Open();
                        cmd_einv.Fill(dt_einv);
                        if (dt_einv.Rows.Count > 0)
                        {
                            KeyId = dt_einv.Rows[0]["e_portal_id"].ToString();
                            KeySecret = dt_einv.Rows[0]["e_portal_password"].ToString();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('E-Invoicing is not applicable for thise company('" + Session["comp_code"].ToString() + "').');", true);
                            return;
                        }
                        d.con.Close();
                    }
                    catch { }
                    #endregion
                    #region
                    hidtab.Value = "5";
                    string query = "", inv_list = "";
                    bool ok = false;
                    string current_date = DateTime.Now.ToString("yyyy-MM-dd");
                    string where_client = "";
                    try
                    {
                        if (ddl_client.SelectedValue != "Select")
                        {
                            if (ddl_client.SelectedValue != "ALL")
                            {
                                where_client = " and client_code='" + ddl_client.SelectedValue + "'";
                            }
                            query = "select client_name,state_name,month,year,invoice_no,date_format(invoice_date,'%d/%m/%Y') as invoice_date,ROUND(amount,2) as amount,ROUND(cgst,2) as cgst,ROUND(sgst,2) as sgst,ROUND(igst,2) as igst,ROUND(amount+cgst+sgst+igst,2) as billing_amount, IF(type = 'manual', CASE WHEN manual_invoice_bill_type = 'Manpower Billing' THEN 'manpower' WHEN manual_invoice_bill_type = 'Employee Conveyance' THEN 'conveyance' WHEN manual_invoice_bill_type = 'Driver Convenyance' THEN 'driver_conveyance' WHEN manual_invoice_bill_type = 'Machine Rental' THEN 'machine_rental' WHEN manual_invoice_bill_type = 'Material Billing' THEN 'material' WHEN manual_invoice_bill_type = 'Deep Clean Billing' THEN 'deepclean' WHEN manual_invoice_bill_type = 'OT Billing' THEN 'manpower_ot' WHEN manual_invoice_bill_type = 'Office Rent Billing' THEN 'office_rent_bill' WHEN manual_invoice_bill_type = 'Shiftwise Billing' THEN 'shiftwise_bill' WHEN manual_invoice_bill_type = 'R And M Service' THEN 'r_and_m_bill' WHEN manual_invoice_bill_type = 'Administrative Expenses' THEN 'administrative_bill' WHEN manual_invoice_bill_type='' or manual_invoice_bill_type is null then 'manual'  END, type) AS 'type' from pay_report_gst where comp_code='" + Session["comp_code"] + "' and  e_invoice_status=0 AND invoice_no='" + inv_no + "'";

                MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
                DataTable dt = new DataTable();
                dscmd.SelectCommand.CommandTimeout = 400;
                dscmd.Fill(dt);

                string month = "", year = "", invoice_no = "",invoice_list="",msg="";
                foreach (DataRow dr in dt.Rows)
                {
                    month = dr["month"].ToString();
                    year = dr["year"].ToString();
                    invoice_no = dr["invoice_no"].ToString();

                    if (KeyId!="" && KeySecret!="")
                    {
                        //Live E- invoice Code IHMS
                       einvoice_process(invoice_no, month, year, out invoice_list, out ok,out msg);

                        //Testing E-invoice Code IHMS--Using OCTA GST Secrete Key & Password
                        //einvoice_process_testing(invoice_no, month, year, out invoice_list, out ok, out msg);

                        if (inv_list == "") { inv_list = invoice_list; } else { inv_list = inv_list + "," + invoice_list; };                       
                        //ok = true;
                    }
                                     
                }

                if (ok==true)
                {
                    check_einv_bind();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + msg + " \n " + inv_list + "');", true);
                }
                else
                {
                    check_einv_bind();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + msg + "');", true);
                }

                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select client name.');", true);
                        }
                    }
                    catch (Exception ex) { throw ex; }
                    finally
                    {
                        d.con.Close();
                    }
                    #endregion
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    private void check_einv_bind()
    {
        string where_client ="", query = "";
        if (ddl_client.SelectedValue != "Select")
        {
            if (ddl_client.SelectedValue != "ALL")
            {
                where_client = " and client_code='" + ddl_client.SelectedValue + "'";
            }
            query = "select client_name,state_name,month,year,invoice_no,date_format(invoice_date,'%d/%m/%Y') as invoice_date,ROUND(amount,2) as amount,ROUND(cgst,2) as cgst,ROUND(sgst,2) as sgst,ROUND(igst,2) as igst,ROUND(amount+cgst+sgst+igst,2) as billing_amount,type from pay_report_gst where comp_code='" + Session["comp_code"] + "' and  e_invoice_status=0  " + where_client + " AND invoice_date BETWEEN STR_TO_DATE('" + gst_from_date.Text + "','%d/%m/%Y') and str_to_date('" + gst_to_date.Text + "','%d/%m/%Y')";//and invoice_date='" + current_date + "'

            MySqlDataAdapter ds_cmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();
            ds_cmd.SelectCommand.CommandTimeout = 400;
            ds_cmd.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                gv_check_einv.DataSource = ds.Tables[0];
                gv_check_einv.DataBind();
                btn_einv_process.Visible = true;
            }
            else
            {
                gv_check_einv.DataSource = null;
                gv_check_einv.DataBind();
                btn_einv_process.Visible = false;

            }
        }
    }
    protected void gv_check_einv_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_check_einv.UseAccessibleHeader = false;
            gv_check_einv.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
    private void einvoice_process(string invoice_no, string month, string year, out string  invoice_no_list,out Boolean ok,out string msg)
    {

        try
        {
            // string ot_applicable = "", machine_rental = "", handaling_amount = "", state_name = "";
            string irn_no = "", irn_gstin = "", json_result = "", ack_no = "", ack_time = "", qr_img = "", doc_type = "", invoice_no_list1="";
            // e-inv Start TM
            #region
            DataTable dt2 = new DataTable();
            MySqlCommand cmd2 = new MySqlCommand("select g.comp_code,g.client_code,g.client_name,g.gst_no as client_gstno,substring(g.gst_no,1,2) as client_statecode,IF(length(z.Field1)>100, REVERSE(SUBSTRING(REVERSE(SUBSTRING(z.Field1, 1, 100)), INSTR(REVERSE(SUBSTRING(z.Field1, 1, 100)), ','))),z.Field1) as client_gstaddress_1, IF(length(z.Field1)>100, CONCAT(substring_index(SUBSTRING(z.Field1, 1, 100), ',', -1),'', SUBSTRING(z.Field1, 101, 200)),'') as client_gstaddress_2,z.field3 as client_pincode,g.sac_code,case WHEN  G.state_name = 'Maharashtra-Mumbai' then  REPLACE(IF(G.state_name = 'Maharashtra-Mumbai', 'Maharashtra', G.state_name), '2', '')  else REPLACE(IF(G.state_name = 'Pondicherry', 'Puducherry', G.state_name), '2', '') end AS 'state_name',g.invoice_no,date_format(g.invoice_date,'%d/%m/%Y') as invoice_date,IF(g.type = 'manual', CASE WHEN manual_invoice_bill_type = 'Manpower Billing' THEN 'manpower' WHEN manual_invoice_bill_type = 'Employee Conveyance' THEN 'conveyance' WHEN manual_invoice_bill_type = 'Driver Convenyance' THEN 'driver_conveyance' WHEN manual_invoice_bill_type = 'Machine Rental' THEN 'machine_rental' WHEN manual_invoice_bill_type = 'Material Billing' THEN 'material' WHEN manual_invoice_bill_type = 'Deep Clean Billing' THEN 'deepclean' WHEN manual_invoice_bill_type = 'OT Billing' THEN 'manpower_ot' WHEN manual_invoice_bill_type = 'Office Rent Billing' THEN 'office_rent_bill' WHEN manual_invoice_bill_type = 'Shiftwise Billing' THEN 'shiftwise_bill' WHEN manual_invoice_bill_type = 'R And M Service' THEN 'r_and_m_bill' WHEN manual_invoice_bill_type = 'Administrative Expenses' THEN 'administrative_bill' WHEN manual_invoice_bill_type='' or manual_invoice_bill_type is null then 'manual'  END, g.type) AS 'type', cmp.COMPANY_NAME, IF(length(cmp.ADDRESS1)>100, REVERSE(SUBSTRING(REVERSE(SUBSTRING(cmp.ADDRESS1, 1, 100)), INSTR(REVERSE(SUBSTRING(cmp.ADDRESS1, 1, 100)), ','))),cmp.ADDRESS1) as cmp_address_1, IF(length(cmp.ADDRESS1)>100, CONCAT(substring_index(SUBSTRING(cmp.ADDRESS1, 1, 100), ',', -1),'', SUBSTRING(cmp.ADDRESS1, 101, 200)),'') as cmp_address_2,  cmp.CITY as cmp_location, cmp.STATE as cmp_state,  cmp.SERVICE_TAX_REG_NO as cmp_gstin,cmp.pin as cmp_pin, substring(cmp.SERVICE_TAX_REG_NO,1,2) as cmp_state_code,ROUND(g.amount,2) as taxable_amt,ROUND(g.cgst,2) as cgst,ROUND(g.sgst,2) as sgst,ROUND(g.igst,2) as igst, ROUND((g.amount+g.cgst+g.sgst+g.igst),2) as billing_amt,( ROUND((g.amount+g.cgst+g.sgst+g.igst),2)- ROUND((g.amount+g.cgst+g.sgst+g.igst),0)) as rounding_amt,ROUND((g.amount+g.cgst+g.sgst+g.igst),0) as roundoff_billing_amt  from pay_report_gst g   inner join pay_company_master cmp on g.comp_code=cmp.COMP_CODE   left join pay_zone_master z on g.comp_code=z.comp_code and g.client_code=z.client_code and g.state_name=z.REGION and z.type='GST'  where g.month='" + month + "' and g.year='" + year + "' and g.invoice_no='" + invoice_no + "'", d.con1);
            MySqlDataAdapter dt_item2 = new MySqlDataAdapter(cmd2);
            dt_item2.Fill(dt2);

            string client_code = "", client_name = "", client_gstno = "", client_statecode = "", client_gstaddress_1 = "", client_gstaddress_2 = "", client_pincode = "0", sac_code = "", statename = "", invoice_date = "", billing_type = "", COMPANY_NAME = "", cmp_address_1="",cmp_address_2 = "", cmp_location = "", cmp_state = "", cmp_gstin = "", cmp_pin = "0", cmp_state_code = "", taxable_amt = "0", cgst = "0", sgst = "0", igst = "0", billing_amt = "0", rounding_amt = "0", roundoff_billing_amt = "0";
            if (dt2.Rows.Count > 0)
            {
                client_code = dt2.Rows[0]["client_code"].ToString();
                client_name = dt2.Rows[0]["client_name"].ToString();
                client_gstno = dt2.Rows[0]["client_gstno"].ToString();
                client_statecode = dt2.Rows[0]["client_statecode"].ToString();
                client_gstaddress_1 = dt2.Rows[0]["client_gstaddress_1"].ToString();
                client_gstaddress_2 = dt2.Rows[0]["client_gstaddress_2"].ToString();
                client_pincode = dt2.Rows[0]["client_pincode"].ToString();

                COMPANY_NAME = dt2.Rows[0]["COMPANY_NAME"].ToString();
                cmp_address_1 = dt2.Rows[0]["cmp_address_1"].ToString();
                cmp_address_2 = dt2.Rows[0]["cmp_address_2"].ToString();
                cmp_location = dt2.Rows[0]["cmp_location"].ToString();
                cmp_state = dt2.Rows[0]["cmp_state"].ToString();
                cmp_gstin = dt2.Rows[0]["cmp_gstin"].ToString();
                cmp_pin = (dt2.Rows[0]["cmp_pin"].ToString()).Trim();
                cmp_state_code = dt2.Rows[0]["cmp_state_code"].ToString();

                sac_code = dt2.Rows[0]["sac_code"].ToString();
                statename = dt2.Rows[0]["state_name"].ToString();
                invoice_no = dt2.Rows[0]["invoice_no"].ToString();
                invoice_date = dt2.Rows[0]["invoice_date"].ToString();
                billing_type = dt2.Rows[0]["type"].ToString();
                // eg, taxable_amt= 488798.24, cgst=43991.86, sgst=43991.86, igst=0.00, billing_amt=576781.96, rounding_amt=-0.04, roundoff_billing_amt=576782    
                taxable_amt = dt2.Rows[0]["taxable_amt"].ToString();
                cgst = dt2.Rows[0]["cgst"].ToString();
                sgst = dt2.Rows[0]["sgst"].ToString();
                igst = dt2.Rows[0]["igst"].ToString();
                billing_amt = dt2.Rows[0]["billing_amt"].ToString();
                rounding_amt = dt2.Rows[0]["rounding_amt"].ToString();
                roundoff_billing_amt = dt2.Rows[0]["roundoff_billing_amt"].ToString();

            }
            DataTable dt1 = new DataTable();
            MySqlCommand cmd1 = new MySqlCommand("select id, comp_code, client_code, invoice_no, invoice_date, irnno, irn_gstin, ack_no, ack_date, qr_code_image, state, billtype, status from pay_einvoice_detail where month='" + month + "' and year='" + year + "' and invoice_no='" + invoice_no + "'", d.con1);
            MySqlDataAdapter dt_item = new MySqlDataAdapter(cmd1);
            dt_item.Fill(dt1);
#endregion
            if (cmp_gstin != "" && COMPANY_NAME != "" && cmp_address_1 != "" && cmp_pin != "" && cmp_state_code != "" && client_gstno!="" && client_gstaddress_1!="" && client_name!="" && client_statecode !="" && client_pincode!="" && billing_type!="" && taxable_amt!="" && sac_code!="")
            {
              #region OCTA API 
            
            if (billing_type == "credit")
            {
                doc_type = "CRN";
            }
            else if (billing_type == "debit")
            {
                doc_type = "DBN";
            }
            else
            {
                doc_type = "INV";
            }

            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow row in dt1.Rows)
                {

                    irn_gstin = row["irn_gstin"].ToString();
                    irn_no = row["irnno"].ToString();
                    ack_no = row["ack_no"].ToString();
                    ack_time = row["ack_date"].ToString();
                    // qr_img = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "" + row["qr_code_image"].ToString()+ "");
                    qr_img = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "E_Invoice_code\\" + row["qr_code_image"].ToString() + "");


                }
            }
            else
            {
                // Generate E-Invoice IRN & QR Code
                /* OctaBills Cloud API */
                var client = new OctaBillsApiClient(KeyId, KeySecret);
                /* Use for OctaBills Server API  */
                //var client = new OctaBillsApiClient(ServerAddress, ServerPort, Username, Password);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                var doc = new JDocument
                {
                    TxnInfo = new JTxnInfo
                    {
                        TaxScheme = "GST",
                        SupplyType = "B2B",
                        IsRcmApplied = "N"
                    },

                    DocInfo = new JDocInfo
                    {
                        // DocType INV--Regular Invoice    CRN-- Credit Note     DBN-- Debit NOte

                        //  DocType = "INV",//INV-invoice no
                        // DocType = "CRN", //CRN-Credit note
                        // DocType = "DBN", //DBN-Debit note

                        DocType = doc_type,
                        DocNo = invoice_no,//"API-" + (DateTime.Now.Ticks / TimeSpan.TicksPerSecond),//invoice_no,//
                        DocDate = invoice_date//DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)

                        //// Invoice details
                        //DocType = "INV",  // regularinvoice=INV   Credit Note =CRN    Debit Note=DBN
                        //DocNo = invoice_no,//"API-" + (DateTime.Now.Ticks / TimeSpan.TicksPerSecond),
                        //DocDate = invoice_date//DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                    },

                    Seller = new JContactInfo
                    {
                        ////company detail -- testing
                        //Gstin = "08AASFB9647G1ZU",
                        //LegalName = "Blowbits Solution LLP",
                        //TradeName = "Blowbits Solution LLP",
                        //Addr1 = "146 Ashok Nagar",
                        //Addr2 = "Road No 8",
                        //Location = "Jaipur",
                        //PinCode = 313001,
                        //StateCode = "08"

                        //ihms
                        Gstin = cmp_gstin,
                        LegalName = COMPANY_NAME,
                        TradeName = COMPANY_NAME,
                        Addr1 = cmp_address_1,
                        Addr2 = cmp_address_2,
                        Location = cmp_location,
                        PinCode = Convert.ToInt32(cmp_pin),
                        StateCode = cmp_state_code


                    },

                    Buyer = new JContactInfo
                    {
                        //// Client detail--testing
                        //Gstin = "33AABCN5735F1ZP",
                        //LegalName = "Star Colourpark India Private Limited",
                        //TradeName = "Star Colourpark India Private Limited",
                        //PlaceOfSupply = "33",
                        //Addr1 = "110, Asoka Plaza,",
                        //Addr2 = "Dr. Nanjappa Road, Gandhipuram",
                        //Location = "Coimbatore",
                        //PinCode = 641018,
                        //StateCode = "33"

                        // IHMS
                        Gstin = client_gstno,
                        LegalName = client_name,
                        TradeName = client_name,
                        PlaceOfSupply = statename,
                        Addr1 = client_gstaddress_1,
                        Addr2 = client_gstaddress_1,
                        Location = statename,
                        PinCode = Convert.ToInt32(client_pincode),
                        StateCode = client_statecode
                    },

                    Items = new List<JLineItem>
                {
                    new JLineItem
                    { 
                        //// testing
                        //  SrNo ="1",
                        //ProductDescription = "Manpower",
                        // Hsn= "1001",
                        //Qty= 1,
                        //Uqc= "OTH",
                        //UnitPrice= 5000,
                        //ItemGrossValue=  5000,
                        //Discount= 0,
                        //TaxableValue= 5000,
                        //GstRate= 18,
                        //Igst= 900



                        // ihms
                        SrNo ="1",
                        ProductDescription = billing_type,
                        Hsn= sac_code,                       
                        UnitPrice= Convert.ToDecimal(taxable_amt),
                        ItemGrossValue= Convert.ToDecimal(taxable_amt),
                        Discount= 0,
                        TaxableValue= Convert.ToDecimal(taxable_amt),
                        GstRate= 18,
                        Igst= Convert.ToDecimal(igst),
                        Cgst=Convert.ToDecimal(cgst),                        
                        Sgst=Convert.ToDecimal(sgst)
                    }
                },

                    DocSummary = new JDocSummary
                    { 
                        
                        //// testing
                        //RoundingOff = 0,
                        //DocValue = 5900
                        
                        // Ihms
                        RoundingOff = Convert.ToDecimal(rounding_amt),
                        DocValue = Convert.ToDecimal(roundoff_billing_amt)
                    }

                };

                try
                {
                    var result = client.GenerateIrn(doc, true, false);
                    json_result = JsonConvert.SerializeObject(result, Formatting.Indented);
                    //--IRN cancel btn
                    // buttonCancelIrn.Enabled = result.Success;
                    string Qr_codename = invoice_no + ".png";
                    if (result.Success)
                    {
                        irn_gstin = doc.Seller.Gstin;
                        irn_no = result.Irn;
                        ack_no = result.AckNo;
                        ack_time = result.AckTime;
                        if (result.QRCodeImagePng != null)
                        {
                            var imagedata = Convert.FromBase64String(result.QRCodeImagePng);//view image code
                            //   QrCodeImage.Image = Image.FromStream(new MemoryStream(imagedata));     //--Windows
                            //   QrCodeImage.ImageUrl = "data:image;base64," + Convert.ToBase64String(imagedata);  //--Asp

                            string qt_code = System.IO.Path.Combine(Convert.ToBase64String(imagedata));
                            byte[] bytes = Convert.FromBase64String(qt_code);

                            System.Drawing.Image image;
                            try
                            {
                                using (MemoryStream ms2 = new MemoryStream(bytes))
                                {
                                    image = System.Drawing.Image.FromStream(ms2);
                                    image.Save(Server.MapPath("E_Invoice_code/") + Qr_codename, System.Drawing.Imaging.ImageFormat.Jpeg);

                                }
                                qr_img = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "E_Invoice_code\\" + Qr_codename + "");
                            }
                            catch  {  }
                          

                        }

                        DateTime theDate = Convert.ToDateTime(ack_time);
                        string ack_date = theDate.ToString("yyyy-MM-dd H:mm:ss");
                        try
                        {
                            if (irn_no != "" && invoice_no != "" && Qr_codename != "" && month != "" && year != "" && ack_no!="" )
                            {
                                d.operation("INSERT INTO pay_einvoice_detail (comp_code, client_code,client_name, invoice_no, invoice_date, irnno, irn_gstin, ack_no, ack_date, qr_code_image, state, billtype,month,year,client_gstin,upload_by,upload_date) values ('" + Session["COMP_CODE"].ToString() + "','" + client_code + "','" + client_name + "','" + invoice_no + "',str_to_date('" + invoice_date + "','%d/%m/%Y'),'" + irn_no + "','" + irn_gstin + "','" + ack_no + "', '" + ack_date + "' ,'" + Qr_codename + "','" + statename + "','" + billing_type + "','" + month + "','" + year + "','" + client_gstno + "','" + Session["LOGIN_ID"].ToString() + "',now()) ");
                                d.operation("Update pay_report_gst set softcopy_sendmail_status=0,e_invoice_status=1 where invoice_no='" + invoice_no + "' and month='" + month + "' and year='" + year + "' ");
                                invoice_no_list1 = invoice_no;
                                ok = true;
                            }
                            
                        }
                        catch     {  }
                       
                    }
                    else
                    {
                        string json = new JavaScriptSerializer().Serialize(json_result);
                        //string error_code = result.Code;
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert( " + json + ")", true);
                        invoice_no_list = invoice_no_list1;
                        msg = json;
                        ok = false;
                        return ;
                      

                    }

                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    invoice_no_list = invoice_no_list1;
                    ok = false;
                }
            }
            #endregion
            }
            // e-inv end
            string json1 = new JavaScriptSerializer().Serialize(json_result);
            msg = json1;
            ok = true;
            invoice_no_list = invoice_no_list1;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void einvoice_process_testing(string invoice_no, string month, string year, out string invoice_no_list, out Boolean ok, out string msg)
    {

        try
        {
            // string ot_applicable = "", machine_rental = "", handaling_amount = "", state_name = "";
            string irn_no = "", irn_gstin = "", json_result = "", ack_no = "", ack_time = "", qr_img = "", doc_type = "", invoice_no_list1 = "";
            // e-inv Start TM
            #region
            DataTable dt2 = new DataTable();
            MySqlCommand cmd2 = new MySqlCommand("select g.comp_code,g.client_code,g.client_name,g.gst_no as client_gstno,substring(g.gst_no,1,2) as client_statecode,IF(length(z.Field1)>100, REVERSE(SUBSTRING(REVERSE(SUBSTRING(z.Field1, 1, 100)), INSTR(REVERSE(SUBSTRING(z.Field1, 1, 100)), ','))),z.Field1) as client_gstaddress_1, IF(length(z.Field1)>100, CONCAT(substring_index(SUBSTRING(z.Field1, 1, 100), ',', -1),'', SUBSTRING(z.Field1, 101, 200)),'') as client_gstaddress_2,z.field3 as client_pincode,g.sac_code,case WHEN  G.state_name = 'Maharashtra-Mumbai' then  REPLACE(IF(G.state_name = 'Maharashtra-Mumbai', 'Maharashtra', G.state_name), '2', '')  else REPLACE(IF(G.state_name = 'Pondicherry', 'Puducherry', G.state_name), '2', '') end AS 'state_name',g.invoice_no,date_format(g.invoice_date,'%d/%m/%Y') as invoice_date,IF(g.type = 'manual', CASE WHEN manual_invoice_bill_type = 'Manpower Billing' THEN 'manpower' WHEN manual_invoice_bill_type = 'Employee Conveyance' THEN 'conveyance' WHEN manual_invoice_bill_type = 'Driver Convenyance' THEN 'driver_conveyance' WHEN manual_invoice_bill_type = 'Machine Rental' THEN 'machine_rental' WHEN manual_invoice_bill_type = 'Material Billing' THEN 'material' WHEN manual_invoice_bill_type = 'Deep Clean Billing' THEN 'deepclean' WHEN manual_invoice_bill_type = 'OT Billing' THEN 'manpower_ot' WHEN manual_invoice_bill_type = 'Office Rent Billing' THEN 'office_rent_bill' WHEN manual_invoice_bill_type = 'Shiftwise Billing' THEN 'shiftwise_bill' WHEN manual_invoice_bill_type = 'R And M Service' THEN 'r_and_m_bill' WHEN manual_invoice_bill_type = 'Administrative Expenses' THEN 'administrative_bill' WHEN manual_invoice_bill_type='' or manual_invoice_bill_type is null then 'manual'  END, g.type) AS 'type', cmp.COMPANY_NAME, IF(length(cmp.ADDRESS1)>100, REVERSE(SUBSTRING(REVERSE(SUBSTRING(cmp.ADDRESS1, 1, 100)), INSTR(REVERSE(SUBSTRING(cmp.ADDRESS1, 1, 100)), ','))),cmp.ADDRESS1) as cmp_address_1, IF(length(cmp.ADDRESS1)>100, CONCAT(substring_index(SUBSTRING(cmp.ADDRESS1, 1, 100), ',', -1),'', SUBSTRING(cmp.ADDRESS1, 101, 200)),'') as cmp_address_2,  cmp.CITY as cmp_location, cmp.STATE as cmp_state,  cmp.SERVICE_TAX_REG_NO as cmp_gstin,cmp.pin as cmp_pin, substring(cmp.SERVICE_TAX_REG_NO,1,2) as cmp_state_code,ROUND(g.amount,2) as taxable_amt,ROUND(g.cgst,2) as cgst,ROUND(g.sgst,2) as sgst,ROUND(g.igst,2) as igst, ROUND((g.amount+g.cgst+g.sgst+g.igst),2) as billing_amt,( ROUND((g.amount+g.cgst+g.sgst+g.igst),2)- ROUND((g.amount+g.cgst+g.sgst+g.igst),0)) as rounding_amt,ROUND((g.amount+g.cgst+g.sgst+g.igst),0) as roundoff_billing_amt  from pay_report_gst g   inner join pay_company_master cmp on g.comp_code=cmp.COMP_CODE   left join pay_zone_master z on g.comp_code=z.comp_code and g.client_code=z.client_code and g.state_name=z.REGION and z.type='GST'  where g.month='" + month + "' and g.year='" + year + "' and g.invoice_no='" + invoice_no + "'", d.con1);
            MySqlDataAdapter dt_item2 = new MySqlDataAdapter(cmd2);
            dt_item2.Fill(dt2);

            string client_code = "", client_name = "", client_gstno = "", client_statecode = "", client_gstaddress_1 = "", client_gstaddress_2 = "", client_pincode = "0", sac_code = "", statename = "", invoice_date = "", billing_type = "", COMPANY_NAME = "", cmp_address_1 = "", cmp_address_2 = "", cmp_location = "", cmp_state = "", cmp_gstin = "", cmp_pin = "0", cmp_state_code = "", taxable_amt = "0", cgst = "0", sgst = "0", igst = "0", billing_amt = "0", rounding_amt = "0", roundoff_billing_amt = "0";
            if (dt2.Rows.Count > 0)
            {
                client_code = dt2.Rows[0]["client_code"].ToString();
                client_name = dt2.Rows[0]["client_name"].ToString();
                client_gstno = dt2.Rows[0]["client_gstno"].ToString();
                client_statecode = dt2.Rows[0]["client_statecode"].ToString();
                client_gstaddress_1 = dt2.Rows[0]["client_gstaddress_1"].ToString();
                client_gstaddress_2 = dt2.Rows[0]["client_gstaddress_2"].ToString();
                client_pincode = dt2.Rows[0]["client_pincode"].ToString();

                COMPANY_NAME = dt2.Rows[0]["COMPANY_NAME"].ToString();
                cmp_address_1 = dt2.Rows[0]["cmp_address_1"].ToString();
                cmp_address_2 = dt2.Rows[0]["cmp_address_2"].ToString();
                cmp_location = dt2.Rows[0]["cmp_location"].ToString();
                cmp_state = dt2.Rows[0]["cmp_state"].ToString();
                cmp_gstin = dt2.Rows[0]["cmp_gstin"].ToString();
                cmp_pin = (dt2.Rows[0]["cmp_pin"].ToString()).Trim();
                cmp_state_code = dt2.Rows[0]["cmp_state_code"].ToString();

                sac_code = dt2.Rows[0]["sac_code"].ToString();
                statename = dt2.Rows[0]["state_name"].ToString();
                invoice_no = dt2.Rows[0]["invoice_no"].ToString();
                invoice_date = dt2.Rows[0]["invoice_date"].ToString();
                billing_type = dt2.Rows[0]["type"].ToString();
                // eg, taxable_amt= 488798.24, cgst=43991.86, sgst=43991.86, igst=0.00, billing_amt=576781.96, rounding_amt=-0.04, roundoff_billing_amt=576782    
                taxable_amt = dt2.Rows[0]["taxable_amt"].ToString();
                cgst = dt2.Rows[0]["cgst"].ToString();
                sgst = dt2.Rows[0]["sgst"].ToString();
                igst = dt2.Rows[0]["igst"].ToString();
                billing_amt = dt2.Rows[0]["billing_amt"].ToString();
                rounding_amt = dt2.Rows[0]["rounding_amt"].ToString();
                roundoff_billing_amt = dt2.Rows[0]["roundoff_billing_amt"].ToString();

            }
            DataTable dt1 = new DataTable();
            MySqlCommand cmd1 = new MySqlCommand("select id, comp_code, client_code, invoice_no, invoice_date, irnno, irn_gstin, ack_no, ack_date, qr_code_image, state, billtype, status from pay_einvoice_detail where month='" + month + "' and year='" + year + "' and invoice_no='" + invoice_no + "'", d.con1);
            MySqlDataAdapter dt_item = new MySqlDataAdapter(cmd1);
            dt_item.Fill(dt1);
            #endregion
            if (cmp_gstin != "" && COMPANY_NAME != "" && cmp_address_1 != "" && cmp_pin != "" && cmp_state_code != "" && client_gstno != "" && client_gstaddress_1 != "" && client_name != "" && client_statecode != "" && client_pincode != "" && billing_type != "" && taxable_amt != "" && sac_code != "")
            {
                #region OCTA API

                if (billing_type == "credit")
                {
                    doc_type = "CRN";
                }
                else if (billing_type == "debit")
                {
                    doc_type = "DBN";
                }
                else
                {
                    doc_type = "INV";
                }

                if (dt1.Rows.Count > 0)
                {
                    foreach (DataRow row in dt1.Rows)
                    {

                        irn_gstin = row["irn_gstin"].ToString();
                        irn_no = row["irnno"].ToString();
                        ack_no = row["ack_no"].ToString();
                        ack_time = row["ack_date"].ToString();
                        // qr_img = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "" + row["qr_code_image"].ToString()+ "");
                        qr_img = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "E_Invoice_code\\" + row["qr_code_image"].ToString() + "");


                    }
                }
                else
                {
                    // Generate E-Invoice IRN & QR Code
                    /* OctaBills Cloud API */

                    KeyId = "k637452784807367055";
                    KeySecret = "Dhxs60WQBnnmn52xd9Kw";

                    var client = new OctaBillsApiClient(KeyId, KeySecret);
                    /* Use for OctaBills Server API  */
                    //var client = new OctaBillsApiClient(ServerAddress, ServerPort, Username, Password);
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                    var doc = new JDocument
                    {
                        TxnInfo = new JTxnInfo
                        {
                            TaxScheme = "GST",
                            SupplyType = "B2B",
                            IsRcmApplied = "N"
                        },

                        DocInfo = new JDocInfo
                        {
                            // DocType INV--Regular Invoice    CRN-- Credit Note     DBN-- Debit NOte

                            //  DocType = "INV",//INV-invoice no
                            // DocType = "CRN", //CRN-Credit note
                            // DocType = "DBN", //DBN-Debit note

                            DocType = doc_type,
                            DocNo = invoice_no,//"API-" + (DateTime.Now.Ticks / TimeSpan.TicksPerSecond),//invoice_no,//
                            DocDate = invoice_date//DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)

                            //// Invoice details
                            //DocType = "INV",  // regularinvoice=INV   Credit Note =CRN    Debit Note=DBN
                            //DocNo = invoice_no,//"API-" + (DateTime.Now.Ticks / TimeSpan.TicksPerSecond),
                            //DocDate = invoice_date//DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                        },

                        Seller = new JContactInfo
                        {
                            //company detail -- testing
                            Gstin = "08AASFB9647G1ZU",
                            LegalName = "Blowbits Solution LLP",
                            TradeName = "Blowbits Solution LLP",
                            Addr1 = "146 Ashok Nagar",
                            Addr2 = "Road No 8",
                            Location = "Jaipur",
                            PinCode = 313001,
                            StateCode = "08"

                            ////ihms
                            //Gstin = cmp_gstin,
                            //LegalName = COMPANY_NAME,
                            //TradeName = COMPANY_NAME,
                            //Addr1 = cmp_address_1,
                            //Addr2 = cmp_address_2,
                            //Location = cmp_location,
                            //PinCode = Convert.ToInt32(cmp_pin),
                            //StateCode = cmp_state_code


                        },

                        Buyer = new JContactInfo
                        {
                            // Client detail--testing
                            Gstin = "33AABCN5735F1ZP",
                            LegalName = "Star Colourpark India Private Limited",
                            TradeName = "Star Colourpark India Private Limited",
                            PlaceOfSupply = "33",
                            Addr1 = "110, Asoka Plaza,",
                            Addr2 = "Dr. Nanjappa Road, Gandhipuram",
                            Location = "Coimbatore",
                            PinCode = 641018,
                            StateCode = "33"

                            //// IHMS
                            //Gstin = client_gstno,
                            //LegalName = client_name,
                            //TradeName = client_name,
                            //PlaceOfSupply = statename,
                            //Addr1 = client_gstaddress_1,
                            //Addr2 = client_gstaddress_1,
                            //Location = statename,
                            //PinCode = Convert.ToInt32(client_pincode),
                            //StateCode = client_statecode
                        },

                        Items = new List<JLineItem>
                {
                    new JLineItem
                    { 
                        //// testing
                        //  SrNo ="1",
                        //ProductDescription = "Manpower",
                        // Hsn= "1001",
                        //Qty= 1,
                        //Uqc= "OTH",
                        //UnitPrice= 5000,
                        //ItemGrossValue=  5000,
                        //Discount= 0,
                        //TaxableValue= 5000,
                        //GstRate= 18,
                        //Igst= 900



                        // ihms
                        SrNo ="1",
                        ProductDescription = billing_type,
                        Hsn= sac_code,
                        UnitPrice= Convert.ToDecimal(taxable_amt),
                        ItemGrossValue= Convert.ToDecimal(taxable_amt),
                        Discount= 0,
                        TaxableValue= Convert.ToDecimal(taxable_amt),
                        GstRate= 18,
                        Igst= Convert.ToDecimal(igst),
                        Cgst=Convert.ToDecimal(cgst),                        
                        Sgst=Convert.ToDecimal(sgst)
                    }
                },

                        DocSummary = new JDocSummary
                        {

                            //// testing
                            //RoundingOff = 0,
                            //DocValue = 5900

                            // Ihms
                            RoundingOff = Convert.ToDecimal(rounding_amt),
                            DocValue = Convert.ToDecimal(roundoff_billing_amt)
                        }

                    };

                    try
                    {
                        var result = client.GenerateIrn(doc, true, false);
                        json_result = JsonConvert.SerializeObject(result, Formatting.Indented);
                        //--IRN cancel btn
                        // buttonCancelIrn.Enabled = result.Success;

                        string Qr_codename = invoice_no + ".png";
                        if (result.Success)
                        {
                            string qt_code = "";
                            irn_gstin = doc.Seller.Gstin;
                            irn_no = result.Irn;
                            ack_no = result.AckNo;
                            ack_time = result.AckTime;
                            if (result.QRCodeImagePng != null)
                            {
                                var imagedata = Convert.FromBase64String(result.QRCodeImagePng);//view image code
                                //   QrCodeImage.Image = Image.FromStream(new MemoryStream(imagedata));     //--Windows
                                //   QrCodeImage.ImageUrl = "data:image;base64," + Convert.ToBase64String(imagedata);  //--Asp

                                 qt_code = System.IO.Path.Combine(Convert.ToBase64String(imagedata));
                                byte[] bytes = Convert.FromBase64String(qt_code);

                                System.Drawing.Image image;
                                try
                                {
                                    using (MemoryStream ms2 = new MemoryStream(bytes))
                                    {
                                        image = System.Drawing.Image.FromStream(ms2);
                                        image.Save(Server.MapPath("E_Invoice_code/") + Qr_codename, System.Drawing.Imaging.ImageFormat.Jpeg);

                                    }
                                    qr_img = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "E_Invoice_code\\" + Qr_codename + "");
                                }
                                catch { }


                            }

                            DateTime theDate = Convert.ToDateTime(ack_time);
                            string ack_date = theDate.ToString("yyyy-MM-dd H:mm:ss");
                            try
                            {
                                if (irn_no != "" && invoice_no != "" && Qr_codename != "" && month != "" && year != "" && ack_no != "")
                                {
                                    d.operation("INSERT INTO pay_einvoice_detail (comp_code, client_code,client_name, invoice_no, invoice_date, irnno, irn_gstin, ack_no, ack_date, qr_code_image, state, billtype,month,year,client_gstin,upload_by,upload_date,qr_code_inbyte) values ('" + Session["COMP_CODE"].ToString() + "','" + client_code + "','" + client_name + "','" + invoice_no + "',str_to_date('" + invoice_date + "','%d/%m/%Y'),'" + irn_no + "','" + irn_gstin + "','" + ack_no + "', '" + ack_date + "' ,'" + Qr_codename + "','" + statename + "','" + billing_type + "','" + month + "','" + year + "','" + client_gstno + "','" + Session["LOGIN_ID"].ToString() + "',now(),'" + qt_code + "') ");
                                    d.operation("  Update pay_report_gst set softcopy_sendmail_status=0,e_invoice_status=1 where invoice_no='" + invoice_no + "' and month='" + month + "' and year='" + year + "' ");
                                    invoice_no_list1 = invoice_no;
                                    ok = true;
                                }

                            }
                            catch { }

                        }
                        else
                        {
                            string json = new JavaScriptSerializer().Serialize(json_result);
                            //string error_code = result.Code;
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert( " + json + ")", true);
                            invoice_no_list = invoice_no_list1;
                            msg = json;
                            ok = false;
                            return;


                        }

                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                        invoice_no_list = invoice_no_list1;
                        ok = false;
                    }
                }
                #endregion
            }

            // e-inv end
            string json1 = new JavaScriptSerializer().Serialize(json_result);
            msg = json1;
            ok = true;
            invoice_no_list = invoice_no_list1;
        }
        catch (Exception)
        {
            throw;
        }
    }

 
}