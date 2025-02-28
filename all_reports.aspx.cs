using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using CrystalDecisions.Shared;
using System.Linq;
using System.Net.Mail;
using CrystalDecisions.CrystalReports.Engine;

using System.IO.Compression;

public partial class all_reports : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();
    DAL d_cg = new DAL();
    int counter = 0;
    BillingSalary bs = new BillingSalary();
    public int arrears_invoice = 0, ot_invoice = 0;
    public static string month_name = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["COMP_CODE"] == null || Session["COMP_CODE"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }
        if (!IsPostBack)
        {
            ddl_billtype_financecopy_Bind("");
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
                    ddl_payment_client_vendor_name.DataSource = dt_item;
                    ddl_payment_client_vendor_name.DataTextField = dt_item.Columns[0].ToString();
                    ddl_payment_client_vendor_name.DataValueField = dt_item.Columns[1].ToString();
                    ddl_payment_client_vendor_name.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_payment_client_vendor_name.Items.Insert(0, "Select");
                ddl_payment_client_vendor_name.Items.Insert(1, "ALL");
                ddl_client.Items.Insert(0, "ALL");
                ddl_client.Items.Insert(0, "ALL");
                ddl_state.Items.Insert(0, "ALL");
                ddl_unitcode.Items.Insert(0, "ALL");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
    }

    protected void ddl_billtype_financecopy_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "6";
        if (ddl_billtype_financecopy.SelectedValue=="3")
        {
            conveyance_type.Visible = true; 
        }
        else
        {
            conveyance_type.Visible = false; 
        }
        hidtab.Value = "6";
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

            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }


            // ddl_billtype_financecopy_Bind(ddl_client.SelectedValue);
        }
    }

    private void ddl_billtype_financecopy_Bind(string client_code)
    {
        ddl_billtype_financecopy.Items.Clear();
        string str = "";
        System.Data.DataTable dt_item = new System.Data.DataTable();
        if (client_code == "")
        {
            str = "select distinct billing_id,billing_name from pay_client_billing_details ";
        }
        else
        {
            str = "select distinct billing_id,billing_name from pay_client_billing_details  where pay_client_billing_details.client_code='" + client_code + "'";
        }

        MySqlDataAdapter cmd_item = new MySqlDataAdapter(str, d.con);


        d.con.Open();
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_billtype_financecopy.DataSource = dt_item;
                ddl_billtype_financecopy.DataValueField = dt_item.Columns[0].ToString();
                ddl_billtype_financecopy.DataTextField = dt_item.Columns[1].ToString();
                ddl_billtype_financecopy.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
            ddl_billtype_financecopy.Items.Insert(0, "Select");

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

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
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
                billing_type = " and type = '" + type + "'";
            }
            if (ddl_client.SelectedValue != "ALL")
            {
                order_by = "order by invoice_no";
                where = "  and pay_report_gst.client_code='" + ddl_client.SelectedValue + "' ";

            }
            else if (ddl_state.SelectedValue != "ALL")
            {
                where = where + " and pay_report_gst.state_name ='" + ddl_state.SelectedValue + "'";

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
                query = "SELECT DATE_FORMAT(invoice_date, '%d/%m/%Y') AS 'billing_date',type, month, year, invoice_no, client_name, state_name, gst_no,ROUND(amount, 2) AS 'amount', ROUND(cgst, 2) AS 'cgst', ROUND(sgst, 2) AS 'sgst', ROUND(igst, 2)AS 'igst', ROUND(cgst + igst + sgst, 2) AS 'gst', ROUND(cgst + igst + sgst + amount, 2) AS 'Total_BILL',sac_code FROM pay_report_gst WHERE  (invoice_no IS NOT NULL  AND invoice_no !='') and comp_code = '" + Session["comp_code"].ToString() + "' and invoice_date between str_to_date('" + gst_from_date.Text + "','%d/%m/%Y') and str_to_date('" + gst_to_date.Text + "','%d/%m/%Y')  " + billing_type + "" + where + invoice_flag + " and (amount is not null ||amount != 0)  order by client_name,type";
            }
            //new button for sac wise gst report
            else if (counter == 2)
            {
                query = "SELECT DATE_FORMAT(invoice_date, '%d/%m/%Y') AS 'billing_date',type,pay_report_gst. month,pay_report_gst. year, pay_report_gst.invoice_no, client_name,pay_report_gst. state_name, gst_no,ROUND(pay_report_gst.amount, 2) AS 'amount', ROUND(cgst, 2) AS 'cgst', ROUND(sgst, 2) AS 'sgst', ROUND(igst, 2)AS 'igst', ROUND(cgst + igst + sgst, 2) AS 'gst', ROUND(cgst + igst + sgst + pay_report_gst.amount, 2) AS 'Total_BILL', sac_code,sum(tot_days_present) as 'no_of_paid_days' FROM pay_report_gst  left outer join pay_billing_unit_rate_history on pay_report_gst.comp_code=pay_billing_unit_rate_history.comp_code and pay_report_gst.invoice_no=pay_billing_unit_rate_history.invoice_no WHERE  (pay_report_gst.invoice_no IS NOT NULL  AND pay_report_gst.invoice_no !='') and pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' and invoice_date between str_to_date('" + gst_from_date.Text + "','%d/%m/%Y') and str_to_date('" + gst_to_date.Text + "','%d/%m/%Y')  " + billing_type + "" + where + invoice_flag + " and (pay_report_gst.amount is not null ||pay_report_gst.amount != 0) group by invoice_no  " + order_by + ",type";
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
                Repeater1.HeaderTemplate = new MyTemplate1(ListItemType.Header, ds, counter);
                Repeater1.ItemTemplate = new MyTemplate1(ListItemType.Item, ds, counter);
                Repeater1.FooterTemplate = new MyTemplate1(ListItemType.Footer, null, counter);
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



        public MyTemplate1(ListItemType type, DataSet ds, int counter)
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
                        lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=16>GST Reports</th></tr><tr><th>SR NO.</th><th>Billing Date</th><th>Billing Type</th><th>Month</th><th>Year</th><th>Invoice No</th><th>Client</th><th>State Name</th><th>GST NO.</th><th>Bill Amount</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Total GST</th><th>Total Bill</th><th>SAC CODE</th></tr> ");
                    }
                    else if (counter == 2)
                    {
                        lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=17>SAC WISE GST Reports</th></tr><tr><th>SR NO.</th><th>Billing Date</th><th>Billing Type</th><th>Month</th><th>Year</th><th>Invoice No</th><th>Client</th><th>State Name</th><th>GST NO.</th><th>Bill Amount</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Total GST</th><th>Total Bill</th><th>SAC Code</th><th>NO of Days</th></tr> ");
                    }
                    break;
                case ListItemType.Item:
                    if (counter == 1)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["billing_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["month"] + "</td><td>" + ds.Tables[0].Rows[ctr]["year"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["igst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Total_BILL"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sac_code"] + "</td></tr>");
                    }
                    else if (counter == 2)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["billing_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["month"] + "</td><td>" + ds.Tables[0].Rows[ctr]["year"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["igst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Total_BILL"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sac_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["no_of_paid_days"] + "</td></tr>");
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

    protected void btn_report_Click(object sender, EventArgs e)
    {
        try
        {
            string query = "";
            string from_date = "'" + txt_payment_date_from.Text + "'";
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
                query = "SELECT vendor_id,purch_invoice_no,vendor_invoice_no,ROUND(grand_total) as 'Amount',date_format(date,'%d/%m/%Y') as 'DATE'," + from_date + " as from_date," + to_date + " as 'to_date',pay_emp_paypro.pay_pro_no,paypro_batch_id,bank FROM pay_pro_vendor INNER JOIN  pay_transactionp ON pay_transactionp.comp_code = pay_pro_vendor.comp_code AND pay_transactionp.DOC_NO = pay_pro_vendor.purch_invoice_no    INNER JOIN pay_emp_paypro ON pay_pro_vendor.purch_invoice_no = pay_emp_paypro.emp_code  AND pay_pro_vendor.comp_code = pay_emp_paypro.comp_code WHERE pay_pro_vendor.comp_code = 'C01' AND date BETWEEN ('" + txt_payment_date_from.Text + "') AND ('" + txt_payment_date_to.Text + "') AND pay_pro_vendor.payment_status = 1  AND paypro_batch_id is not null " + where + " GROUP BY purch_invoice_no";
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
        hidtab.Value = "1";
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
        hidtab.Value = "2";
        //ddl_client.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = null;
        if (ddl_type_tally.SelectedValue == "1" || ddl_type_tally.SelectedValue == "3")
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
            else if (type1 == "3")
            {
                query = "SELECT  date_format(vendor_invoice_date,'%d/%m/%Y') as 'vendor_invoice_date',discription,gst_no,party_name,gross_amount,vendor_invoice_no,amount, CASE WHEN vendor_igst != 0 THEN -vendor_igst else 0 END AS 'vendor_igst',CASE WHEN vendor_cgst != 0 THEN -vendor_cgst else 0 END AS 'vendor_cgst',CASE WHEN vendor_sgst != 0 THEN -vendor_sgst else 0 END AS 'vendor_sgst'  FROM pay_r_and_m_service where vendor_invoice_date between STR_TO_DATE('" + txt_tally_from_date.Text + "', '%d/%m/%Y') AND STR_TO_DATE('" + txt_tally_to_date.Text + "', '%d/%m/%Y') and client_code='" + ddl_select_type.SelectedValue + "'";
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
                    //R&M
                    else if (type1 == "3")
                    {
                        lc = new LiteralControl("<table border=1><tr ></tr><tr><th>SR NO.</th><th bgcolor=DeepSkyBlue>DATE</th><th bgcolor=LightBlue>VOUCHER TYPE</th><th bgcolor=DeepSkyBlue>STANDARD NARRATION</th><th bgcolor=LightBlue>VOUCHER NO</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-1</th><th bgcolor=IndianRed>AMOUNT-1</th><th bgcolor=DeepSkyBlue>REFERANCE NUMBER</th><th bgcolor=DeepSkyBlue>REFERANCE DUE DAYS</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-2</th><th bgcolor=IndianRed>AMOUNT-2</th><th bgcolor=DeepSkyBlue>STOCK ITEM NAME</th><th bgcolor=SkyBlue>STOCK ITEM QTY</th><th bgcolor=DeepSkyBlue>STOCK ITEM RATE</th><th bgcolor=DeepSkyBlue>STOCK ITEM TOTAL AMT</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-3</th><th bgcolor=IndianRed>AMOUNT-3</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-4</th><th bgcolor=LightCoral>AMOUNT-4</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-5</th><th bgcolor=IndianRed>AMOUNT-5</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-6</th><th bgcolor=LightCoral>AMOUNT-6</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-7</th><th bgcolor=IndianRed>AMOUNT-7</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-8</th><th bgcolor=LightCoral>AMOUNT-8</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-9</th><th bgcolor=IndianRed>AMOUNT-9</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-10</th><th bgcolor=LightCoral>AMOUNT-10</th></tr> ");

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
                        string month = "" + ds.Tables[0].Rows[ctr]["month"] + "";
                        string year = "" + ds.Tables[0].Rows[ctr]["year"] + "";
                        string month_name = mfi.GetMonthName(int.Parse("" + month + "")).ToString();
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_date"] + "</td><td>Sales</td><td>Being Sale of Services for the Month of " + month_name + " " + year + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Amount_gst"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td></td><td></td><td></td><td></td><td>CGST@9%</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>SGST@9%</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td><td>IGST@18%</td><td>" + ds.Tables[0].Rows[ctr]["igst"] + "</td><td>Material</td><td>" + ds.Tables[0].Rows[ctr]["material_amount"] + "</td><td>convenyence</td><td>0</td><td>Deep Cleaning</td><td>0</td><td>Rental</td><td>0</td><td>Arreas</td><td>0</td><td>R&M</td><td>0</td><td>Administrative</td><td>0</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td></tr>");
                    }
                    else if (type1 == "3")
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["vendor_invoice_date"] + "</td><td>Journel</td><td>" + ds.Tables[0].Rows[ctr]["discription"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["party_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["vendor_invoice_no"] + "</td><td></td><td>R&M Expenses_Reimbersment</td><td>-" + ds.Tables[0].Rows[ctr]["gross_amount"] + "</td><td></td><td></td><td></td><td></td><td>CGST</td><td>" + ds.Tables[0].Rows[ctr]["vendor_cgst"] + "</td><td>SGST</td><td>" + ds.Tables[0].Rows[ctr]["vendor_sgst"] + "</td><td>IGST</td><td>" + ds.Tables[0].Rows[ctr]["vendor_igst"] + "</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>");

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

    protected void btn_get_payment_Click(object sender, EventArgs e)
    {
        hidtab.Value = "3";
        try
        {
            int i = 0;
            string From_month = "";
            string To_month = "";
            string query = "";
            string where = "";

            if (ddl_payment_client_vendor_name.SelectedValue != "ALL")
            {
                where = " and pay_report_gst.client_code = '" + ddl_payment_client_vendor_name.SelectedValue + "' ";
            }

            if (gst_from_month.Text.Substring(3) != gst_to_month.Text.Substring(3))
            {
                int month = int.Parse(gst_from_month.Text.Substring(0, 2));
                int month1 = int.Parse(gst_to_month.Text.Substring(0, 2));
                for (int j = month; j <= 12; j++)
                {
                    From_month = From_month + j + ",";

                }
                From_month = From_month.Substring(0, From_month.Length - 1);
                for (int j = 1; j <= month1; j++)
                {
                    To_month = To_month + j + ",";

                }
                To_month = To_month.Substring(0, To_month.Length - 1);
            }
            else
            {
                int month = int.Parse(gst_from_month.Text.Substring(0, 2));
                int month1 = int.Parse(gst_to_month.Text.Substring(0, 2));
                for (int j = month; j <= month1; j++)
                {
                    From_month = From_month + j + ",";

                }
                From_month = From_month.Substring(0, From_month.Length - 1);
            }
            //Invoice Payment Report
            if (ddl_payment_report_type.SelectedValue == "1")
            {
                //Manpower summary
                if (ddl_type_payment.SelectedValue == "1")
                {
                    query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst, (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type, SUM(pay_pro_master.payment- (fine + EMP_ADVANCE_PAYMENT + emp_advance + reliver_advances + absent_attendance_total)) as 'payment' FROM pay_report_gst INNER JOIN pay_billing_unit_rate_history ON pay_report_gst.Invoice_no = pay_billing_unit_rate_history.auto_invoice_no INNER JOIN pay_pro_master ON pay_billing_unit_rate_history.month = pay_pro_master.month AND pay_billing_unit_rate_history.year = pay_pro_master.year AND pay_billing_unit_rate_history.emp_code = pay_pro_master.emp_code AND pay_billing_unit_rate_history.start_date = pay_pro_master.start_date AND (pay_billing_unit_rate_history.hdfc_type != 'ot_bill' || pay_billing_unit_rate_history.hdfc_type is null) AND (pay_pro_master.hdfc_type != 'ot_bill' || pay_pro_master.hdfc_type is null) WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND type = 'manpower' " + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY invoice_no ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no";
                    }
                }
                //Conveyance summary
                if (ddl_type_payment.SelectedValue == "2")
                {
                    query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst, (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', (SUM(conveyance_amount - emp_con_deduction)) AS 'payment', pay_report_gst.type, pay_report_gst.month, pay_report_gst.year FROM pay_report_gst INNER JOIN pay_billing_material_history ON pay_report_gst.Invoice_no = pay_billing_material_history.auto_invoice_no INNER JOIN pay_pro_material_history ON pay_billing_material_history.month = pay_pro_material_history.month AND pay_billing_material_history.year = pay_pro_material_history.year AND pay_billing_material_history.emp_code = pay_pro_material_history.emp_code WHERE  pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'conveyance' AND pay_pro_material_history.type = 'Conveyance' AND pay_pro_material_history.conveyance_type != 100 " + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY invoice_no ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no";
                    }
                }
                //Driver Conveyance summary
                if (ddl_type_payment.SelectedValue == "3")
                {
                    query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst, (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', (SUM(conveyance_amount - driver_con_deduction)) AS 'payment', pay_report_gst.type, pay_report_gst.month, pay_report_gst.year FROM pay_report_gst INNER JOIN pay_billing_material_history ON pay_report_gst.Invoice_no = pay_billing_material_history.auto_invoice_no INNER JOIN pay_pro_material_history ON pay_billing_material_history.month = pay_pro_material_history.month AND pay_billing_material_history.year = pay_pro_material_history.year AND pay_billing_material_history.emp_code = pay_pro_material_history.emp_code WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'driver_conveyance' AND pay_pro_material_history.type = 'Conveyance' AND pay_pro_material_history.conveyance_type = 100" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY invoice_no ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no";
                    }
                }
                //Material summary
                if (ddl_type_payment.SelectedValue == "4")
                {
                    query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst, (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type, (SUM(material_amount - material_deduction)) AS 'payment' FROM pay_report_gst INNER JOIN pay_billing_material_history ON pay_report_gst.Invoice_no = pay_billing_material_history.auto_invoice_no INNER JOIN pay_pro_material_history ON pay_billing_material_history.month = pay_pro_material_history.month AND pay_billing_material_history.year = pay_pro_material_history.year AND pay_billing_material_history.emp_code = pay_pro_material_history.emp_code WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'material' AND pay_billing_material_history.type = 'Material'" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY invoice_no ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no";
                    }
                }
                //Arrears summary
                if (ddl_type_payment.SelectedValue == "5")
                {
                    query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst, (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type, SUM(pay_pro_master_arrears.payment) as 'payment' FROM pay_report_gst INNER JOIN pay_billing_unit_rate_history_arrears ON pay_report_gst.Invoice_no = pay_billing_unit_rate_history_arrears.auto_invoice_no INNER JOIN pay_pro_master_arrears ON pay_billing_unit_rate_history_arrears.month = pay_pro_master_arrears.month AND pay_billing_unit_rate_history_arrears.year = pay_pro_master_arrears.year AND pay_billing_unit_rate_history_arrears.emp_code = pay_pro_master_arrears.emp_code WHERE  pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'arrears_manpower'" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY invoice_no ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no";
                    }
                }
                //R&M summary
                if (ddl_type_payment.SelectedValue == "6")
                {
                    query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst, (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type, SUM(pay_pro_r_m.amount) AS 'payment' FROM pay_report_gst INNER JOIN pay_billing_r_m ON pay_report_gst.Invoice_no = pay_billing_r_m.auto_invoice_no INNER JOIN pay_pro_r_m ON pay_pro_r_m.month = pay_billing_r_m.month AND pay_pro_r_m.year = pay_billing_r_m.year AND pay_pro_r_m.emp_code = pay_billing_r_m.emp_code WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND type = 'r_and_m_bill'" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY invoice_no ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no";
                    }
                }
                //Admnistrative summary
                if (ddl_type_payment.SelectedValue == "7")
                {
                    query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst, (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type, SUM(pay_pro_admini_expense.amount) AS 'payment' FROM pay_report_gst INNER JOIN pay_billing_admini_expense ON pay_report_gst.Invoice_no = pay_billing_admini_expense.auto_invoice_no INNER JOIN pay_pro_admini_expense ON pay_pro_admini_expense.month = pay_billing_admini_expense.month AND pay_pro_admini_expense.year = pay_billing_admini_expense.year AND pay_pro_admini_expense.emp_code = pay_billing_admini_expense.emp_code WHERE  pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND type = 'administrative_bill'" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY invoice_no ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no";
                    }
                }
                //Shiftwise summary
                if (ddl_type_payment.SelectedValue == "8")
                {
                    query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst, (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type, SUM(pay_pro_shiftwise.amount) as 'payment' FROM pay_report_gst INNER JOIN pay_billing_shiftwise ON pay_report_gst.Invoice_no = pay_billing_shiftwise.auto_invoice_no INNER JOIN pay_pro_shiftwise ON pay_billing_shiftwise.month = pay_pro_shiftwise.month AND pay_billing_shiftwise.year = pay_pro_shiftwise.year AND pay_billing_shiftwise.emp_code = pay_pro_shiftwise.emp_code WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND type = 'shiftwise_bill'" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY invoice_no ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no";
                    }
                }
                //Vendor summary
                if (ddl_type_payment.SelectedValue == "9")
                {
                    query = "SELECT  vendor_id, purch_invoice_no, vendor_invoice_no, pay_transactionp.TAXABLE_AMT AS 'gross_amount', igst, cgst, sgst, round(pay_transactionp.TAXABLE_AMT + igst + cgst + sgst) AS 'total_invoice_value', ROUND(grand_total) AS 'Payment', Bank_holder_name, BANK_EMP_AC_CODE as 'BANK_EMP_NO', PF_IFSC_CODE as 'IFSC_CODE', pay_emp_paypro.pay_pro_no, paypro_batch_id, month_year FROM pay_pro_vendor INNER JOIN pay_transactionp ON pay_transactionp.comp_code = pay_pro_vendor.comp_code AND pay_transactionp.DOC_NO = pay_pro_vendor.purch_invoice_no INNER JOIN pay_emp_paypro ON pay_pro_vendor.purch_invoice_no = pay_emp_paypro.emp_code AND pay_pro_vendor.comp_code = pay_emp_paypro.comp_code WHERE pay_transactionp.comp_code = '" + Session["comp_code"].ToString() + "'";

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_transactionp.month IN (" + From_month + ") and pay_transactionp.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY purch_invoice_no union " + query + " and pay_transactionp.month IN (" + To_month + ") and pay_transactionp.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY purch_invoice_no ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_transactionp.month IN (" + From_month + ") and pay_transactionp.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY purch_invoice_no";
                    }
                }
                //OT Manpower summary
                if (ddl_type_payment.SelectedValue == "10")
                {
                    query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst, (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type, SUM(pay_pro_master.payment- (fine + EMP_ADVANCE_PAYMENT + emp_advance + reliver_advances + absent_attendance_total)) as 'payment' FROM pay_report_gst INNER JOIN pay_billing_unit_rate_history ON pay_report_gst.Invoice_no = pay_billing_unit_rate_history.auto_invoice_no INNER JOIN pay_pro_master ON pay_billing_unit_rate_history.month = pay_pro_master.month AND pay_billing_unit_rate_history.year = pay_pro_master.year AND pay_billing_unit_rate_history.emp_code = pay_pro_master.emp_code AND pay_billing_unit_rate_history.start_date = pay_pro_master.start_date AND pay_billing_unit_rate_history.hdfc_type = pay_pro_master.hdfc_type WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND type = 'manpower_ot' " + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY invoice_no ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY invoice_no";
                    }
                }
            }
            else if (ddl_payment_report_type.SelectedValue == "2")
            {

                //Manpower Employeewise
                if (ddl_type_payment.SelectedValue == "1")
                {
                    query = "SELECT  pay_pro_master.client, pay_pro_master.state_name, pay_pro_master.unit_name, CASE pay_pro_master.employee_type WHEN 'Reliever' THEN CONCAT(pay_pro_master.emp_name, '-', 'Reliever') ELSE pay_pro_master.emp_name END AS 'emp_name', CASE designation WHEN 'OB' THEN CASE pay_pro_master.gender WHEN 'M' THEN 'OFFICE BOY' WHEN 'F' THEN 'OFFICE LADY' ELSE '' END ELSE grade END AS 'grade', actual_basic_vda, pay_pro_master.emp_basic_vda, hra_amount_salary, sal_bonus_gross, leave_sal_gross, washing_salary, travelling_salary, education_salary, allowances_salary, cca_salary, pay_pro_master.other_allow, pay_pro_master.gratuity_gross, sal_ot, pay_pro_master.gross, pay_pro_master.ot_rate, pay_pro_master.ot_hours, pay_pro_master.ot_amount, sal_pf, sal_esic, lwf_salary, sal_uniform_rate, PT_AMOUNT, sal_bonus_after_gross, leave_sal_after_gross, pay_pro_master.gratuity_after_gross, common_allow, esic_allowances_salary, (pay_billing_unit_rate_history.conveyance_amount / pay_billing_unit_rate_history.month_days * tot_days_present) AS 'conveyance_rate', absent_attendance_total, emp_advance, reliver_advances, pay_pro_master.deduction AS 'uni_deduct', pay_pro_master.fine, Total_Days_Present, ((pay_pro_master.gross + common_allow + sal_bonus_after_gross + leave_sal_after_gross + pay_pro_master.gratuity_after_gross + esic_allowances_salary + pay_pro_master.ot_amount + (IF(pay_pro_master.client_code = 'BAGICTM', (pay_billing_unit_rate_history.conveyance_amount / pay_billing_unit_rate_history.month_days * tot_days_present), 0))) - (sal_pf + sal_esic + lwf_salary + sal_uniform_rate + PT_AMOUNT + absent_attendance_total + pay_pro_master.fine + emp_advance + advance_deduction + reliver_advances + pay_pro_master.deduction)) AS 'payment', Bank_holder_name, BANK_EMP_AC_CODE AS 'BANK_AC_NO', PF_IFSC_CODE AS 'IFSC_CODE', pay_pro_master.salary_status, pay_report_gst.invoice_no, paypro_batch_id, pay_pro_no, pay_pro_master.month, pay_pro_master.year,date_format(salary_date,'%d/%m/%Y') as 'paid_date' FROM pay_pro_master INNER JOIN pay_billing_unit_rate_history ON pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year AND pay_pro_master.start_date = pay_billing_unit_rate_history.start_date AND (pay_billing_unit_rate_history.hdfc_type != 'ot_bill' || pay_billing_unit_rate_history.hdfc_type is null) AND (pay_pro_master.hdfc_type != 'ot_bill' || pay_pro_master.hdfc_type is null) LEFT OUTER JOIN pay_emp_paypro ON pay_emp_paypro.comp_code = pay_pro_master.comp_code AND pay_emp_paypro.emp_code = pay_pro_master.emp_code AND pay_emp_paypro.month = pay_pro_master.month AND pay_emp_paypro.year = pay_pro_master.year AND bank = 'AXIS BANK' AND type = 0 INNER JOIN pay_report_gst ON pay_report_gst.Invoice_no = pay_billing_unit_rate_history.auto_invoice_no WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'manpower' " + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_master.emp_code , pay_pro_master.month , pay_pro_master.year union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY pay_pro_master.emp_code , pay_pro_master.month , pay_pro_master.year ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_master.emp_code , pay_pro_master.month , pay_pro_master.year";
                    }
                }
                //Conveyance Employeewise
                if (ddl_type_payment.SelectedValue == "2")
                {
                    query = "SELECT  pay_pro_material_history.client, pay_pro_material_history.state_name, pay_pro_material_history.unit_name, pay_pro_material_history.emp_name, pay_pro_material_history.grade_desc,  conveyance_amount, emp_con_deduction, (conveyance_amount - emp_con_deduction) as 'payment', pay_pro_material_history.BANK_HOLDER_NAME , pay_pro_material_history.BANK_EMP_AC_CODE AS 'BANK_EMP_AC_NO', pay_pro_material_history.PF_IFSC_CODE AS 'IFSC_CODE', pay_report_gst.invoice_no, pay_emp_paypro.pay_pro_no, pay_pro_material_history.month, pay_pro_material_history.year FROM pay_pro_material_history INNER JOIN pay_employee_master ON pay_pro_material_history.emp_code = pay_employee_master.emp_code AND pay_pro_material_history.comp_code = pay_employee_master.comp_code AND pay_pro_material_history.unit_code = pay_employee_master.unit_code INNER JOIN pay_billing_material_history ON pay_pro_material_history.emp_code = pay_billing_material_history.emp_code AND pay_pro_material_history.month = pay_billing_material_history.month AND pay_pro_material_history.year = pay_billing_material_history.year AND pay_pro_material_history.type = pay_billing_material_history.type LEFT OUTER JOIN pay_emp_paypro ON pay_pro_material_history.emp_code = pay_emp_paypro.emp_code AND pay_pro_material_history.month = pay_emp_paypro.month AND pay_pro_material_history.year = pay_emp_paypro.year AND bank = 'AXIS BANK' AND pay_emp_paypro.type = 1 INNER JOIN pay_report_gst ON pay_report_gst.Invoice_no = pay_billing_material_history.auto_invoice_no WHERE  pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'conveyance' AND pay_pro_material_history.type = 'Conveyance' AND pay_pro_material_history.conveyance_type != 100 " + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_material_history.emp_code , pay_pro_material_history.month , pay_pro_material_history.year union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY pay_pro_material_history.emp_code , pay_pro_material_history.month , pay_pro_material_history.year ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_material_history.emp_code , pay_pro_material_history.month , pay_pro_material_history.year";
                    }
                }
                //Driver Conveyance Employeewise
                if (ddl_type_payment.SelectedValue == "3")
                {
                    query = "SELECT  pay_pro_material_history.client, pay_pro_material_history.state_name, pay_pro_material_history.unit_name, pay_pro_material_history.emp_name,  pay_pro_material_history.grade_desc, conveyance_amount, driver_con_deduction, (conveyance_amount - driver_con_deduction) as 'payment', pay_pro_material_history.BANK_HOLDER_NAME , pay_pro_material_history.BANK_EMP_AC_CODE AS 'BANK_EMP_AC_NO', pay_pro_material_history.PF_IFSC_CODE AS 'IFSC_CODE', pay_report_gst.invoice_no, pay_emp_paypro.pay_pro_no, pay_pro_material_history.month, pay_pro_material_history.year FROM pay_pro_material_history INNER JOIN pay_employee_master ON pay_pro_material_history.emp_code = pay_employee_master.emp_code AND pay_pro_material_history.comp_code = pay_employee_master.comp_code AND pay_pro_material_history.unit_code = pay_employee_master.unit_code INNER JOIN pay_billing_material_history ON pay_pro_material_history.emp_code = pay_billing_material_history.emp_code AND pay_pro_material_history.month = pay_billing_material_history.month AND pay_pro_material_history.year = pay_billing_material_history.year AND pay_pro_material_history.type = pay_billing_material_history.type LEFT OUTER JOIN pay_emp_paypro ON pay_pro_material_history.emp_code = pay_emp_paypro.emp_code AND pay_pro_material_history.month = pay_emp_paypro.month AND pay_pro_material_history.year = pay_emp_paypro.year AND bank = 'AXIS BANK' AND pay_emp_paypro.type = 1 INNER JOIN pay_report_gst ON pay_report_gst.Invoice_no = pay_billing_material_history.auto_invoice_no WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'driver_conveyance' AND pay_pro_material_history.type = 'Conveyance' AND pay_pro_material_history.conveyance_type = 100" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_material_history.emp_code , pay_pro_material_history.month , pay_pro_material_history.year union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY pay_pro_material_history.emp_code , pay_pro_material_history.month , pay_pro_material_history.year ";

                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_material_history.emp_code , pay_pro_material_history.month , pay_pro_material_history.year";

                    }
                }
                //Material Employeewise
                if (ddl_type_payment.SelectedValue == "4")
                {
                    query = "SELECT  pay_pro_material_history.client, pay_pro_material_history.state_name, pay_pro_material_history.unit_name, pay_pro_material_history.emp_name, pay_pro_material_history.grade_desc, material_amount, material_deduction, (material_amount - material_deduction) AS 'payment', Bank_holder_name, BANK_EMP_AC_CODE AS 'BANK_EMP_AC_NO', PF_IFSC_CODE AS 'IFSC_CODE', pay_report_gst.invoice_no, pay_emp_paypro.pay_pro_no, pay_pro_material_history.month, pay_pro_material_history.year FROM pay_pro_material_history INNER JOIN pay_billing_material_history ON pay_pro_material_history.emp_code = pay_billing_material_history.emp_code AND pay_pro_material_history.month = pay_billing_material_history.month AND pay_pro_material_history.year = pay_billing_material_history.year AND pay_pro_material_history.type = pay_billing_material_history.type INNER JOIN pay_report_gst ON pay_report_gst.Invoice_no = pay_billing_material_history.auto_invoice_no LEFT OUTER JOIN pay_emp_paypro ON pay_pro_material_history.emp_code = pay_emp_paypro.emp_code AND pay_pro_material_history.month = pay_emp_paypro.month AND pay_pro_material_history.year = pay_emp_paypro.year AND bank = 'AXIS BANK' AND pay_emp_paypro.type = 2 WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'material' AND pay_billing_material_history.type = 'Material'" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_material_history.emp_code , pay_pro_material_history.month , pay_pro_material_history.year union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY pay_pro_material_history.emp_code , pay_pro_material_history.month , pay_pro_material_history.year ";

                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_material_history.emp_code , pay_pro_material_history.month , pay_pro_material_history.year";

                    }
                }
                //Arrears Employeewise
                if (ddl_type_payment.SelectedValue == "5")
                {
                    query = "SELECT  pay_pro_master.client, pay_pro_master.state_name, pay_pro_master.unit_name, CASE pay_pro_master.employee_type WHEN 'Reliever' THEN CONCAT(pay_pro_master.emp_name, '-', 'Reliever') ELSE pay_pro_master.emp_name END AS 'emp_name', CASE designation WHEN 'OB' THEN CASE pay_pro_master.gender WHEN 'M' THEN 'OFFICE BOY' WHEN 'F' THEN 'OFFICE LADY' ELSE '' END ELSE grade END AS 'grade', actual_basic_vda, pay_pro_master.emp_basic_vda, hra_amount_salary, sal_bonus_gross, leave_sal_gross, washing_salary, travelling_salary, education_salary, allowances_salary, cca_salary, pay_pro_master.other_allow, pay_pro_master.gratuity_gross, sal_ot, pay_pro_master.gross, pay_pro_master.ot_rate, pay_pro_master.ot_hours, pay_pro_master.ot_amount, pay_pro_master.gross, sal_pf, sal_esic, lwf_salary, sal_uniform_rate, PT_AMOUNT, sal_bonus_after_gross, leave_sal_after_gross, pay_pro_master.gratuity_after_gross, common_allow, esic_allowances_salary, absent_attendance_total, emp_advance, reliver_advances, pay_pro_master.deduction AS 'uni_deduct', pay_pro_master.fine, Total_Days_Present, ((pay_pro_master.gross + common_allow + sal_bonus_after_gross + leave_sal_after_gross + pay_pro_master.gratuity_after_gross + esic_allowances_salary + pay_pro_master.ot_amount + (IF(pay_pro_master.client_code = 'BAGICTM', (pay_billing_unit_rate_history.conveyance_amount / pay_billing_unit_rate_history.month_days * tot_days_present), 0))) - (sal_pf + sal_esic + lwf_salary + sal_uniform_rate + PT_AMOUNT + absent_attendance_total + pay_pro_master.fine + emp_advance + advance_deduction + reliver_advances + pay_pro_master.deduction)) AS 'payment', BANK_HOLDER_NAME, BANK_EMP_AC_CODE AS 'BANK_EMP_AC_NO', PF_IFSC_CODE AS 'IFSC_CODE', pay_pro_master.salary_status, pay_report_gst.invoice_no, paypro_batch_id, pay_pro_no, pay_pro_master.month, pay_pro_master.year,date_format(salary_date,'%d/%m/%Y') as 'paid_date' FROM pay_pro_master_arrears AS pay_pro_master INNER JOIN pay_billing_unit_rate_history_arrears AS pay_billing_unit_rate_history ON pay_pro_master.emp_code = pay_billing_unit_rate_history.emp_code AND pay_pro_master.month = pay_billing_unit_rate_history.month AND pay_pro_master.year = pay_billing_unit_rate_history.year LEFT OUTER JOIN pay_emp_paypro ON pay_emp_paypro.comp_code = pay_pro_master.comp_code AND pay_emp_paypro.emp_code = pay_pro_master.emp_code AND pay_emp_paypro.month = pay_pro_master.month AND pay_emp_paypro.year = pay_pro_master.year AND bank = 'AXIS BANK' AND type = 4 INNER JOIN pay_report_gst ON pay_report_gst.Invoice_no = pay_billing_unit_rate_history.auto_invoice_no WHERE  pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'arrears_manpower'" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_master.emp_code , pay_pro_master.month , pay_pro_master.year union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY pay_pro_master.emp_code , pay_pro_master.month , pay_pro_master.year ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_master.emp_code , pay_pro_master.month , pay_pro_master.year";
                    }
                }
                //R&M Employeewise
                if (ddl_type_payment.SelectedValue == "6")
                {
                    query = "SELECT   pay_pro_r_m.client, pay_pro_r_m.state_name, pay_pro_r_m.unit_name, pay_pro_r_m.emp_name, pay_pro_r_m.amount as 'Payment', Bank_holder_name, BANK_EMP_AC_CODE AS 'BANK_EMP_AC_NO', PF_IFSC_CODE AS 'IFSC_CODE', pay_report_gst.invoice_no, pay_pro_no,   pay_pro_r_m.month, pay_pro_r_m.year FROM pay_pro_r_m INNER JOIN pay_billing_r_m ON pay_pro_r_m.comp_code = pay_billing_r_m.comp_code AND pay_pro_r_m.client_code = pay_billing_r_m.client_code AND pay_pro_r_m.unit_code = pay_billing_r_m.unit_code AND pay_pro_r_m.emp_name = pay_billing_r_m.emp_name AND pay_pro_r_m.month = pay_billing_r_m.month AND pay_pro_r_m.year = pay_billing_r_m.year AND pay_pro_r_m.invoice_slot = pay_billing_r_m.invoice_slot LEFT OUTER JOIN pay_emp_paypro ON pay_pro_r_m.emp_code = pay_emp_paypro.emp_code AND pay_pro_r_m.month = pay_emp_paypro.month AND pay_pro_r_m.year = pay_emp_paypro.year AND bank = 'AXIS BANK' AND type = 5 INNER JOIN pay_report_gst ON pay_report_gst.Invoice_no = pay_billing_r_m.auto_invoice_no  WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'r_and_m_bill'" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_r_m.EMP_CODE,pay_pro_r_m.month,pay_pro_r_m.year union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY pay_pro_r_m.EMP_CODE,pay_pro_r_m.month,pay_pro_r_m.year ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_r_m.EMP_CODE,pay_pro_r_m.month,pay_pro_r_m.year";
                    }
                }
                //Admnistrative Employeewise
                if (ddl_type_payment.SelectedValue == "7")
                {
                    query = "SELECT   pay_pro_admini_expense.client, pay_pro_admini_expense.state_name, pay_pro_admini_expense.unit_name, pay_pro_admini_expense.emp_name, pay_pro_admini_expense.amount AS 'Payment', Bank_holder_name, BANK_EMP_AC_CODE AS 'BANK_EMP_AC_NO', PF_IFSC_CODE AS 'IFSC_CODE', pay_report_gst.invoice_no, pay_pro_no, pay_pro_admini_expense.month, pay_pro_admini_expense.year FROM pay_pro_admini_expense INNER JOIN pay_billing_admini_expense ON pay_billing_admini_expense.comp_code = pay_pro_admini_expense.comp_code AND pay_billing_admini_expense.client_code = pay_pro_admini_expense.client_code AND pay_billing_admini_expense.emp_name = pay_pro_admini_expense.emp_name AND pay_billing_admini_expense.month = pay_pro_admini_expense.month AND pay_billing_admini_expense.year = pay_pro_admini_expense.year LEFT OUTER JOIN pay_emp_paypro ON pay_pro_admini_expense.emp_code = pay_emp_paypro.emp_code AND pay_pro_admini_expense.month = pay_emp_paypro.month AND pay_pro_admini_expense.year = pay_emp_paypro.year AND bank = 'AXIS BANK' AND type = 6 INNER JOIN pay_report_gst ON pay_report_gst.Invoice_no = pay_billing_admini_expense.auto_invoice_no WHERE  pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'administrative_bill'" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_admini_expense.EMP_CODE,pay_pro_admini_expense.month,pay_pro_admini_expense.year union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY pay_pro_admini_expense.EMP_CODE,pay_pro_admini_expense.month,pay_pro_admini_expense.year ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_admini_expense.EMP_CODE,pay_pro_admini_expense.month,pay_pro_admini_expense.year";
                    }
                }
                //Shiftwise Employeewise
                if (ddl_type_payment.SelectedValue == "8")
                {
                    query = "SELECT  pay_pro_shiftwise.client, pay_pro_shiftwise.state_name, pay_pro_shiftwise.unit_name, pay_pro_shiftwise.emp_name,pay_pro_shiftwise.grade_desc, pay_pro_shiftwise.shift_salary_rate, pay_pro_shiftwise.shift_days, pay_pro_shiftwise.amount AS 'Payment', Bank_holder_name, BANK_EMP_AC_CODE AS 'BANK_EMP_AC_NO', PF_IFSC_CODE AS 'IFSC_CODE', pay_report_gst.invoice_no, pay_emp_paypro.pay_pro_no, pay_pro_shiftwise.month, pay_pro_shiftwise.year FROM pay_pro_shiftwise INNER JOIN pay_billing_shiftwise ON `pay_billing_shiftwise`.`emp_code` = `pay_pro_shiftwise`.`emp_code` AND `pay_billing_shiftwise`.`month` = `pay_pro_shiftwise`.`month` AND `pay_billing_shiftwise`.`year` = `pay_pro_shiftwise`.`year` LEFT OUTER JOIN pay_emp_paypro ON pay_pro_shiftwise.emp_code = pay_emp_paypro.emp_code AND pay_pro_shiftwise.month = pay_emp_paypro.month AND pay_pro_shiftwise.year = pay_emp_paypro.year AND bank = 'AXIS BANK' AND type = 9 INNER JOIN pay_report_gst ON pay_report_gst.Invoice_no = pay_billing_shiftwise.auto_invoice_no WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'shiftwise_bill'" + where;

                    if (To_month != "")
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_shiftwise.emp_code , pay_pro_shiftwise.month , pay_pro_shiftwise.year union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY pay_pro_shiftwise.emp_code , pay_pro_shiftwise.month , pay_pro_shiftwise.year ";
                    }
                    else
                    {
                        query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY pay_pro_shiftwise.emp_code , pay_pro_shiftwise.month , pay_pro_shiftwise.year";
                    }
                }



            }
            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();

            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);


            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                if (ddl_payment_report_type.SelectedValue == "1")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=" + ddl_type_payment.SelectedItem + "_SUMMARY_Report_" + ddl_payment_client_vendor_name.SelectedItem.Text.Replace(" ", "_") + ".xls");
                }
                else if (ddl_payment_report_type.SelectedValue == "2")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=" + ddl_type_payment.SelectedItem + "_EMPLOYEEWISE_Report" + ddl_payment_client_vendor_name.SelectedItem.Text.Replace(" ", "_") + ".xls");
                }
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate3(ListItemType.Header, ds, ddl_type_payment.SelectedValue, ddl_payment_report_type.SelectedValue);
                Repeater1.ItemTemplate = new MyTemplate3(ListItemType.Item, ds, ddl_type_payment.SelectedValue, ddl_payment_report_type.SelectedValue);
                Repeater1.FooterTemplate = new MyTemplate3(ListItemType.Footer, null, null, null);
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
    protected void btn_rejectbill_Click(object sender, EventArgs e)
    {
        hidtab.Value = "5";
        try
        {
            int i = 0;
            string From_month = "";
            string To_month = "";
            string query = "";
            string where = "";

            if (ddl_client.SelectedValue != "ALL")
            {
                where += " and pay_report_gst.Client_name = '" + ddl_client.SelectedItem + "' ";
            }
            if (ddl_state.SelectedValue != "ALL")
            {
                where += " and pay_report_gst.state_name = '" + ddl_state.SelectedItem + "' ";
            }
            if (ddl_unitcode.SelectedValue != "ALL")
            {
                where += " and pay_report_gst.unit_code = '" + ddl_unitcode.SelectedValue + "' ";
            }


            if (txt_fromdate.Text.Substring(3) != txt_todate.Text.Substring(3))
            {
                int month = int.Parse(txt_fromdate.Text.Substring(0, 2));
                int month1 = int.Parse(txt_todate.Text.Substring(0, 2));
                for (int j = month; j <= 12; j++)
                {
                    From_month = From_month + j + ",";

                }
                From_month = From_month.Substring(0, From_month.Length - 1);
                for (int j = 1; j <= month1; j++)
                {
                    To_month = To_month + j + ",";

                }
                To_month = To_month.Substring(0, To_month.Length - 1);
            }
            else
            {
                int month = int.Parse(txt_fromdate.Text.Substring(0, 2));
                int month1 = int.Parse(txt_todate.Text.Substring(0, 2));
                for (int j = month; j <= month1; j++)
                {
                    From_month = From_month + j + ",";

                }
                From_month = From_month.Substring(0, From_month.Length - 1);
            }
            //Invoice Reject Report

            //Manpower 
            if (ddl_billtypes.SelectedValue == "1")
            {
                 query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', 'manpower' as billtype, pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.emp_count,  pay_report_gst.amount AS 'grandTotal', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst,(pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'Totalgst', pay_report_gst.type , SUM(pay_pro_master.payment- (fine + EMP_ADVANCE_PAYMENT + emp_advance + reliver_advances + absent_attendance_total)) as 'Total_CTC'  FROM pay_report_gst INNER JOIN pay_billing_unit_rate_history ON pay_report_gst.Invoice_no = pay_billing_unit_rate_history.auto_invoice_no INNER JOIN pay_pro_master ON pay_billing_unit_rate_history.month = pay_pro_master.month AND pay_billing_unit_rate_history.year = pay_pro_master.year AND pay_billing_unit_rate_history.emp_code = pay_pro_master.emp_code AND pay_billing_unit_rate_history.start_date = pay_pro_master.start_date  WHERE pay_report_gst.comp_code = 'C01' AND  pay_billing_unit_rate_history.invoice_flag=0 and pay_billing_unit_rate_history.invoice_no !=''";                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
                
                if (To_month != "")
                {
                    query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_unit_rate_history.invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + txt_todate.Text.Substring(3) + "' GROUP BY pay_billing_unit_rate_history.invoice_no ";
                }
                else
                {
                    query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_unit_rate_history.invoice_no";
                }
            }
            //Conveyance 
            if (ddl_billtypes.SelectedValue == "2")
            {
                query = "SELECT  IF(pay_billing_material_history.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company','Conveyance' AS billtype, client as client_name,CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'Invoice date', STATE_NAME as state_name, UNIT_NAME as unit_name, unit_gst_no,COUNT(emp_name) AS emp_count,SUM(round(IF( conveyance_applicable  = 1 AND  conveyance_type  = 2, ( Conveyance_PerKmRate  *  conveyance_km ),  conveyance_rate ) ,2))AS grandTotal,CGST ,  SGST , IGST , (SGST+ CGST+ IGST) as Totalgst,(IF( conveyance_applicable  = 1 AND  conveyance_type  = 2, ( Conveyance_PerKmRate  *  conveyance_km ),  conveyance_rate ) +SGST+ CGST+ IGST) as Total_CTC FROM pay_billing_material_history WHERE comp_code= '" + Session["comp_code"].ToString() + "' AND status_flag = '2' AND invoice_flag = 0 AND invoice_no != ''";
                //query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date',  pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst,   (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount',  pay_report_gst.type,(SUM(conveyance_amount - emp_con_deduction)) AS 'payment'   FROM pay_report_gst INNER JOIN pay_billing_material_history ON pay_report_gst.Invoice_no = pay_billing_material_history.auto_invoice_no INNER JOIN pay_pro_material_history    ON pay_billing_material_history.month = pay_pro_material_history.month AND pay_billing_material_history.year = pay_pro_material_history.year AND pay_billing_material_history.emp_code = pay_pro_material_history.emp_code   WHERE   pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.type = 'Conveyance'    AND  pay_billing_material_history.invoice_flag=0 and pay_billing_material_history.invoice_no !='' " + where;

                if (To_month != "")
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                    //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + txt_todate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no ";
                }
                else
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                    //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no";
                }
            }
            //Driver Conveyance 
            if (ddl_billtypes.SelectedValue == "3")
            {
                query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date',  pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst,   (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount',  pay_report_gst.type,(SUM(conveyance_amount - emp_con_deduction)) AS 'payment'   FROM pay_report_gst INNER JOIN pay_billing_material_history ON pay_report_gst.Invoice_no = pay_billing_material_history.auto_invoice_no INNER JOIN pay_pro_material_history    ON pay_billing_material_history.month = pay_pro_material_history.month AND pay_billing_material_history.year = pay_pro_material_history.year AND pay_billing_material_history.emp_code = pay_pro_material_history.emp_code   WHERE   pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.type = 'driver_conveyance'    AND  pay_billing_material_history.invoice_flag=0 and pay_billing_material_history.invoice_no !=''" + where;

                if (To_month != "")
                {
                    query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + txt_todate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no ";
                }
                else
                {
                    query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no";
                }
            }
            //Material 
            if (ddl_billtypes.SelectedValue == "4")
            {
                //query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date',  pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst,   (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount',  pay_report_gst.type,(SUM(conveyance_amount - emp_con_deduction)) AS 'payment'   FROM pay_report_gst INNER JOIN pay_billing_material_history ON pay_report_gst.Invoice_no = pay_billing_material_history.auto_invoice_no INNER JOIN pay_pro_material_history    ON pay_billing_material_history.month = pay_pro_material_history.month AND pay_billing_material_history.year = pay_pro_material_history.year AND pay_billing_material_history.emp_code = pay_pro_material_history.emp_code   WHERE   pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.type = 'Material'    AND  pay_billing_material_history.invoice_flag=0 and pay_billing_material_history.invoice_no !='' " + where;
                query = "SELECT  IF(pay_billing_material_history.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', 'Material' AS billtype,client as client_name,invoice_no, DATE_FORMAT(billing_date, '%d/%m/%Y') AS billing_date,state_name,unit_name,unit_gst_no,COUNT(emp_name) AS emp_count,SUM(IF(material_contract != 0 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) ) AS grandTotal, ROUND(SUM(IFNULL(CGST, 0)), 2) AS CGST,ROUND(SUM(IFNULL(sgst, 0)), 2) AS SGST,ROUND(SUM(IFNULL(igst, 0)), 2) AS igst,SUM(ROUND((IFNULL(CGST, 0) + IFNULL(sgst, 0) + IFNULL(igst, 0)),2)) AS Totalgst,SUM(IF(material_contract != 0 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + CGST+sgst+igst) AS Total_CTC FROM pay_billing_material_history WHERE comp_code= '" + Session["comp_code"].ToString() + "' AND status_flag = '2' AND invoice_flag = 0 AND invoice_no != ''";
                if (To_month != "")
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                    //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + txt_todate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no ";
                }
                else
                {
                   query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                    //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no";
                }
            }

            //DeepClean 
            if (ddl_billtypes.SelectedValue == "5")
            {

                query = "SELECT  IF(pay_billing_material_history.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company','deep_clean' AS billtype,client as client_name,invoice_no, DATE_FORMAT(billing_date, '%d/%m/%Y') AS 'Invoice Date',STATE_NAME as state_name, UNIT_NAME as unit_name, unit_gst_no,COUNT(emp_name) AS emp_count, IF(dc_contract = 1 AND dc_type = 2, (dc_rate * dc_area), dc_rate) AS 'grandTotal',CGST ,  SGST, IGST, ROUND((CGST+SGST+IGST),2) as Totalgst,ROUND( IF(dc_contract = 1 AND dc_type = 2,(dc_rate * dc_area),  dc_rate) +CGST+SGST+IGST,2) as Total_CTC FROM pay_billing_material_history where comp_code = '" + Session["comp_code"].ToString() + "'  AND invoice_flag = 0 AND invoice_no != '' ";
                //query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date',  pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst,   (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount',  pay_report_gst.type,(SUM(conveyance_amount - emp_con_deduction)) AS 'payment'   FROM pay_report_gst INNER JOIN pay_billing_material_history ON pay_report_gst.Invoice_no = pay_billing_material_history.auto_invoice_no INNER JOIN pay_pro_material_history    ON pay_billing_material_history.month = pay_pro_material_history.month AND pay_billing_material_history.year = pay_pro_material_history.year AND pay_billing_material_history.emp_code = pay_pro_material_history.emp_code   WHERE   pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.type = 'DeepClean'    AND  pay_billing_material_history.invoice_flag=0 and pay_billing_material_history.invoice_no !='' " + where;

                if (To_month != "")
                {
                       query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                        //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + txt_todate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no ";
                }
                else
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                   // query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_material_history.invoice_no";
                }
            }


            //  

            //Machine_Rental 
            if (ddl_billtypes.SelectedValue == "6")
            {

                query = "SELECT  IF(pay_billing_rental_machine.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company',  'Machine Rental' AS billtype,client_code as client_name,invoice_no,DATE_FORMAT(billing_date, '%d/%m/%Y') AS Invoice Date,state as state_name,unit_name,unit_gst_no,COUNT(machine_name) AS machine_count  ,SUM(ROUND((handling_amount+ rent + service_charge ),2)) AS grandTotal,ROUND(SUM(IFNULL(CGST, 0)), 2) AS CGST,ROUND(SUM(IFNULL(sgst, 0)), 2) AS SGST,ROUND(SUM(IFNULL(igst, 0)), 2) AS igst,SUM(ROUND((IFNULL(CGST, 0) + IFNULL(sgst, 0) + IFNULL(igst, 0)),2)) AS Totalgst,SUM(ROUND(handling_amount + service_charge + rent + IFNULL(CGST, 0) + IFNULL(sgst, 0) + IFNULL(igst, 0),2)) AS Total_CTC FROM pay_billing_rental_machine where comp_code = '" + Session["comp_code"].ToString() + "'  AND invoice_flag = 0 AND invoice_no != '' ";
                //query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date',    pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst,     (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type    FROM pay_report_gst INNER JOIN pay_billing_rental_machine ON pay_report_gst.Invoice_no = pay_billing_rental_machine.auto_invoice_no     WHERE pay_report_gst.comp_code = 'C01' AND type = 'machine_rental'     and   pay_billing_rental_machine.invoice_flag=0 and pay_billing_rental_machine.invoice_no !=''  " + where;

                if (To_month != "")
                {
                     query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                   // query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_rental_machine.invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + txt_todate.Text.Substring(3) + "' GROUP BY pay_billing_rental_machine.invoice_no ";
                }
                else
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                    //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_rental_machine.invoice_no";
                }
            }


            //Arrears 
            if (ddl_billtypes.SelectedValue == "7")
            {

                query = "SELECT  IF(pay_billing_unit_rate_history_arrears.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', 'arrears' AS billtype,client as client_name,invoice_no,DATE_FORMAT(billing_date, '%d/%m/%Y') AS 'Invoice Date',state_name,unit_name,unit_gst_no,COUNT(emp_name) AS emp_count,SUM(ROUND((Amount + service_charge + operational_cost + uniform + ot_amount + group_insurance_billing),2)) AS grandTotal,ROUND(SUM(IFNULL(CGST9, 0)), 2) AS CGST,ROUND(SUM(IFNULL(sgst9, 0)), 2) AS SGST,ROUND(SUM(IFNULL(igst18, 0)), 2) AS igst,SUM(ROUND((IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0)),2)) AS Totalgst,SUM(ROUND(Amount + service_charge + operational_cost + uniform + ot_amount + group_insurance_billing + IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0),2)) AS Total_CTC FROM pay_billing_unit_rate_history_arrears where comp_code = '" + Session["comp_code"].ToString() + "'  AND invoice_flag = 0 AND invoice_no != '' ";
                   // query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date',  pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst,   (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type, SUM(pay_pro_master_arrears.payment) as 'payment'    FROM pay_report_gst   INNER JOIN pay_billing_unit_rate_history_arrears ON pay_report_gst.Invoice_no = pay_billing_unit_rate_history_arrears.auto_invoice_no    INNER JOIN pay_pro_master_arrears ON pay_billing_unit_rate_history_arrears.month = pay_pro_master_arrears.month AND pay_billing_unit_rate_history_arrears.year = pay_pro_master_arrears.year    AND pay_billing_unit_rate_history_arrears.emp_code = pay_pro_master_arrears.emp_code    WHERE  pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_report_gst.type = 'arrears_manpower'    AND  pay_billing_unit_rate_history_arrears.invoice_flag=0 and pay_billing_unit_rate_history_arrears.invoice_no !=''    AND pay_report_gst.type = 'arrears_manpower'" + where;

                if (To_month != "")
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                    //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_unit_rate_history_arrears.invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + txt_todate.Text.Substring(3) + "' GROUP BY pay_billing_unit_rate_history_arrears.invoice_no ";
                }
                else
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                    //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_unit_rate_history_arrears.invoice_no";
                }
            }
            //R&M 
            if (ddl_billtypes.SelectedValue == "9")
            {
                
                query="SELECT  IF(pay_billing_r_m.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', 'R&M' AS billtype,client as client_name,invoice_no,DATE_FORMAT(billing_date, '%d/%m/%Y') AS 'Invoice Date',state_name,unit_name,unit_gst_no,COUNT(emp_name) AS emp_count,(SUM(ROUND((amount) + (Service_charge), 2)) ) AS grandTotal,ROUND(SUM(IFNULL(CGST9, 0)), 2) AS CGST,ROUND(SUM(IFNULL(sgst9, 0)), 2) AS SGST,ROUND(SUM(IFNULL(igst18, 0)), 2) AS igst,SUM(ROUND((IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0)),2)) AS Totalgst,SUM(ROUND(Amount + service_charge + IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0),2)) AS Total_CTC FROM pay_billing_r_m WHERE   '" + Session["comp_code"].ToString() + "'  AND invoice_flag = 0 AND invoice_no != ''";
                //query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date',  pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst,  (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type, SUM(pay_pro_r_m.amount) AS 'payment'   FROM pay_report_gst INNER JOIN pay_billing_r_m ON pay_report_gst.Invoice_no = pay_billing_r_m.auto_invoice_no   INNER JOIN pay_pro_r_m ON pay_pro_r_m.month = pay_billing_r_m.month AND pay_pro_r_m.year = pay_billing_r_m.year   AND pay_pro_r_m.emp_code = pay_billing_r_m.emp_code    WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'r_and_m_bill' AND  pay_billing_r_m.invoice_flag=0 and pay_billing_r_m.invoice_no !=''" + where;

                if (To_month != "")
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                    //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_r_m.invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + txt_todate.Text.Substring(3) + "' GROUP BY pay_billing_r_m.invoice_no ";
                }
                else
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                    //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_r_m.invoice_no";
                }
            }
            //Admnistrative 
            if (ddl_billtypes.SelectedValue == "10")
            {
                query = "SELECT  IF(pay_billing_admini_expense.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', 'Administrative_Expense' AS billtype, client as client_name,invoice_no,DATE_FORMAT(billing_date, '%d/%m/%Y') AS 'Invoice date',state_name,unit_name,unit_gst_no,COUNT(emp_name) AS emp_count, SUM(ROUND((Amount + service_charge + bill_service_charge ),2)) AS grandTotal,ROUND(SUM(IFNULL(CGST9, 0)), 2) AS CGST,ROUND(SUM(IFNULL(sgst9, 0)), 2) AS SGST,ROUND(SUM(IFNULL(igst18, 0)), 2) AS igst,SUM(ROUND((IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0)),2)) AS Totalgst,SUM(ROUND(Amount + service_charge + bill_service_charge + IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0), 2)) AS Total_CTC FROM pay_billing_admini_expense where comp_code = '" + Session["comp_code"].ToString() + "'  AND invoice_flag = 0 AND invoice_no != '' ";
                 //query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', pay_report_gst.Client_name, pay_report_gst.State_name, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date',  pay_report_gst.gst_no, pay_report_gst.sac_code, pay_report_gst.emp_count, pay_report_gst.month, pay_report_gst.year, pay_report_gst.amount AS 'bill_amount', pay_report_gst.cgst, pay_report_gst.sgst, pay_report_gst.igst,  (pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst) AS 'total_billing_amount', pay_report_gst.type, SUM(pay_pro_admini_expense.amount) AS 'payment'   FROM pay_report_gst INNER JOIN pay_billing_admini_expense ON pay_report_gst.Invoice_no = pay_billing_admini_expense.auto_invoice_no   INNER JOIN pay_pro_admini_expense ON pay_pro_admini_expense.month = pay_billing_admini_expense.month AND pay_pro_admini_expense.year = pay_billing_admini_expense.year   AND pay_pro_admini_expense.emp_code = pay_billing_admini_expense.emp_code WHERE  pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_report_gst.type = 'administrative_bill'    and   pay_billing_admini_expense.invoice_flag=0 and pay_billing_admini_expense.invoice_no !=''" + where;

                if (To_month != "")
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                    //query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + txt_fromdate.Text.Substring(3) + "' GROUP BY pay_billing_admini_expense.invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + txt_todate.Text.Substring(3) + "' GROUP BY pay_billing_admini_expense.invoice_no ";
                }
                else
                {
                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                }
            }
            //Shiftwise 
            if (ddl_billtypes.SelectedValue == "11")
            {

                query = "SELECT  IF(pay_billing_shiftwise.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company','Shiftwise Billing' AS billtype, client as client_name,invoice_no,DATE_FORMAT(billing_date, '%d/%m/%Y') AS 'Invoice Date',state_name,unit_name,unit_gst_no,COUNT(emp_name) AS emp_count,SUM(ROUND((pay_billing_shiftwise.amount+Service_charge), 2)) as grandTotal,ROUND(SUM(IFNULL(CGST9, 0)), 2) AS CGST,ROUND(SUM(IFNULL(sgst9, 0)), 2) AS SGST,ROUND(SUM(IFNULL(igst18, 0)), 2) AS igst,SUM(ROUND((IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0)),2)) AS Totalgst,SUM(ROUND((pay_billing_shiftwise.amount+Service_charge+CGST9+SGST9+IGST18),2)) as  Total_CTC FROM pay_billing_shiftwise where comp_code = '" + Session["comp_code"].ToString() + "'  AND invoice_flag = 0 AND invoice_no != '' ";
                
                if (To_month != "")
                {

                    query = "" + query + " AND  month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                }
                else
                {
                    query = "" + query + "AND month IN(" + From_month + ") and year = '" + txt_fromdate.Text.Substring(3) + "' group by invoice_no";
                }
            }
            ////Vendor summary
            //if (ddl_type_payment.SelectedValue == "9")
            //{
            //    query = "SELECT  vendor_id, purch_invoice_no, vendor_invoice_no, pay_transactionp.TAXABLE_AMT AS 'gross_amount', igst, cgst, sgst, round(pay_transactionp.TAXABLE_AMT + igst + cgst + sgst) AS 'total_invoice_value', ROUND(grand_total) AS 'Payment', Bank_holder_name, BANK_EMP_AC_CODE as 'BANK_EMP_NO', PF_IFSC_CODE as 'IFSC_CODE', pay_emp_paypro.pay_pro_no, paypro_batch_id, month_year FROM pay_pro_vendor INNER JOIN pay_transactionp ON pay_transactionp.comp_code = pay_pro_vendor.comp_code AND pay_transactionp.DOC_NO = pay_pro_vendor.purch_invoice_no INNER JOIN pay_emp_paypro ON pay_pro_vendor.purch_invoice_no = pay_emp_paypro.emp_code AND pay_pro_vendor.comp_code = pay_emp_paypro.comp_code WHERE pay_transactionp.comp_code = '" + Session["comp_code"].ToString() + "'";

            //    if (To_month != "")
            //    {
            //        query = "" + query + " AND pay_transactionp.month IN (" + From_month + ") and pay_transactionp.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY purch_invoice_no union " + query + " and pay_transactionp.month IN (" + To_month + ") and pay_transactionp.year='" + gst_to_month.Text.Substring(3) + "' GROUP BY purch_invoice_no ";
            //    }
            //    else
            //    {
            //        query = "" + query + " AND pay_transactionp.month IN (" + From_month + ") and pay_transactionp.year='" + gst_from_month.Text.Substring(3) + "' GROUP BY purch_invoice_no";
            //    }
            //}

            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();

            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);


            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                //if (ddl_payment_report_type.SelectedValue == "1")
                //{
                Response.AddHeader("content-disposition", "attachment;filename=" + ddl_billtypes.SelectedItem + "_RejectBill_Report.xls");
                //}
                //else if (ddl_payment_report_type.SelectedValue == "2")
                //{
                //    Response.AddHeader("content-disposition", "attachment;filename=" + ddl_billtypes.SelectedItem + "_EMPLOYEEWISE_Report" + ddl_payment_client_vendor_name.SelectedItem.Text.Replace(" ", "_") + ".xls");
                //}
                string type1 = "1";//4-Reject Bill report
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate3(ListItemType.Header, ds, ddl_billtypes.SelectedValue, type1);
                Repeater1.ItemTemplate = new MyTemplate3(ListItemType.Item, ds, ddl_billtypes.SelectedValue, type1);
                Repeater1.FooterTemplate = new MyTemplate3(ListItemType.Footer, null, null, null);
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

    protected void btn_financecopy_Click(object sender, EventArgs e)
    {
        hidtab.Value = "7";
        int count = 0;
        if (ddl_billtype_financecopy.SelectedValue == "1")//manpower
        { count = 1; }
        if (ddl_billtype_financecopy.SelectedValue == "11")//Shiftwise 
        { count = 11; }
        if (ddl_billtype_financecopy.SelectedValue == "2")//Material
        { count = 2; }
        if (ddl_billtype_financecopy.SelectedValue == "4")//Deep Cleaning
        { count = 4; }
        if (ddl_billtype_financecopy.SelectedValue == "3")//Conveyance
        { count = 3; }
        if (ddl_billtype_financecopy.SelectedValue == "7")//Machine Rental
        { machinRental_FC(); }
        if (ddl_billtype_financecopy.SelectedValue == "8")//R&M services
        { count = 8; }
        if (ddl_billtype_financecopy.SelectedValue == "9")//administrative Expenses
        { count = 9; }




        if (count > 0)
        {
            generate_report(count, 0, ddl_billtype_financecopy.SelectedValue);
        }

       


        hidtab.Value = "7";
    }
    #region Machin Rental FC
    private void machinRental_FC()
    {
        try
        {
            string where = "";
            DateTimeFormatInfo mfi = new DateTimeFormatInfo();
            month_name = mfi.GetMonthName(int.Parse(txt_date.Text.Substring(0, 2))).ToString();
            month_name = month_name + " " + txt_date.Text.Substring(3).ToUpper();

            string daterange = "concat(upper(DATE_FORMAT(str_to_date('" + txt_date.Text.Substring(3) + "-" + txt_date.Text.Substring(0, 2) + "-01','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(LAST_DAY('" + txt_date.Text.Substring(3) + "-" + txt_date.Text.Substring(0, 2) + "-01'), '%d %b %Y'))) as start_end_date";

            string start_date_common = get_start_date();
            if (start_date_common != "" && start_date_common != "1")
            {
                daterange = "concat(upper(DATE_FORMAT(str_to_date('" + txt_date.Text.Substring(3) + "-" + (int.Parse(txt_date.Text.Substring(0, 2)) == 1 ? 12 : (int.Parse(txt_date.Text.Substring(0, 2)) - 1)) + "-" + start_date_common + "','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(str_to_date('" + txt_date.Text.Substring(3) + "-" + txt_date.Text.Substring(0, 2) + "-" + (int.Parse(start_date_common) - 1) + "','%Y-%m-%d'), '%d %b %Y'))) as start_end_date";
            }
            if (ddl_client.SelectedValue!="ALL")
            {
                where = " AND client_code = '" + ddl_client.SelectedValue + "' ";
            }
            if (ddl_state.SelectedValue!="ALL")
            {
                 where = " AND client_code = '" + ddl_client.SelectedValue + "'  and  STATE='"+ddl_state.SelectedItem.Text+"' ";
            }
            if (ddl_unitcode.SelectedValue!="ALL")
            {
                where = " AND client_code = '" + ddl_client.SelectedValue + "'  and  STATE='" + ddl_state.SelectedItem.Text + "'  AND unit_code = '" + ddl_unitcode.SelectedValue + "' ";
            }
            string sql = "SELECT  cast(CASE WHEN handling_per > '0' THEN handling_per else '0' end as char)as 'h_per',auto_invoice_no,DATE_FORMAT(billing_date, '%d-%m-%Y') as 'billing_date'," + daterange + ",COMP_STATE AS 'STATE1',STATE,CLIENT_CODE,client_name,UNIT_NAME,machine_name,cast((CASE WHEN  handling_per  > '0' THEN (( rent  *  handling_per  *  qty ) / 100) WHEN  handling_amount  > 0 THEN  handling_amount  ELSE '0' END) as char) AS 'handling',total, rent as 'qty', qty as 'rent',rent*qty as 'rent_qty',cast(CASE WHEN SUBSTRING(service_tax_reg_no,1, 2) = SUBSTRING(unit_gst_no,1, 2) THEN ((total) * 9 / 100) ELSE '0' END as char) AS 'SGCT',cast(CASE WHEN SUBSTRING(service_tax_reg_no,1, 2) = SUBSTRING(unit_gst_no,1, 2) THEN ((total) * 9 / 100) ELSE '0' END as char) AS 'CGCT',cast(CASE WHEN SUBSTRING(service_tax_reg_no,1, 2) != SUBSTRING(unit_gst_no,1, 2) THEN ((total) * 18 / 100) ELSE '0' END as char) AS 'IGCT', rent_type  FROM pay_billing_rental_machine WHERE  comp_code = '" + Session["COMP_CODE"].ToString() + "'  and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' "+ where +" ";

            MySqlDataAdapter dscmd = new MySqlDataAdapter(sql, d.con);
            DataSet ds = new DataSet();
            dscmd.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Machine_Rental_Finance_copy_" + ddl_client.SelectedItem.Text.Replace(" ", "_") + ".xls");

                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate11(ListItemType.Header, ds, 1);
                Repeater1.ItemTemplate = new MyTemplate11(ListItemType.Item, ds, 1);
                Repeater1.FooterTemplate = new MyTemplate11(ListItemType.Footer, null, 1);
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
    public class MyTemplate11 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;
        int i;


        public MyTemplate11(ListItemType type, DataSet ds, int i)
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

                    lc = new LiteralControl("<table border=1><tr><th  colspan =18 bgcolor=yellow align=center >MACHINE RENTAL FINANCE COPY </th></tr><tr ><th>SR No.</th><th>BILL NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>CLIENT</th><th>STATE</th><th>Location</th><th>Machine Name</th><th>Rent Type</th><th>QTY</th><th>Rate</th><th>Total</th><th>Handling Charges " + ds.Tables[0].Rows[ctr]["h_per"] + "%</th><th>Total</th><th>SGST 9%</th><th>CGST 9%</th><th>IGST 18%</th><th>Total GST</th><th>GRAND TOTAL</th></tr>");
                    break;
                case ListItemType.Item:
                    //3                                                 //location                                     //handling     
                    lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td> <td>" + ds.Tables[0].Rows[ctr]["auto_invoice_no"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["start_end_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["STATE"] + "</td><td>" + ds.Tables[0].Rows[ctr]["UNIT_NAME"] + "</td><td>" + ds.Tables[0].Rows[ctr]["machine_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["rent_type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["qty"] + "</td><td>" + ds.Tables[0].Rows[ctr]["rent"] + "</td><td>" + ds.Tables[0].Rows[ctr]["rent_qty"] + "</td><td>" + ds.Tables[0].Rows[ctr]["handling"] + "</td><td>" + ds.Tables[0].Rows[ctr]["total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["SGCT"] + "</td><td>" + ds.Tables[0].Rows[ctr]["CGCT"] + "</td><td>" + ds.Tables[0].Rows[ctr]["IGCT"] + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGCT"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["CGCT"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGCT"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGCT"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["CGCT"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGCT"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()), 2) + "</td></tr>");//double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()
                    if (ds.Tables[0].Rows.Count == ctr + 1)
                    {
                        lc.Text = lc.Text + "<tr><b><td align=center colspan = 11>Total</td><td>=ROUND(SUM(L2:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M2:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N2:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O2:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P2:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q2:Q" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(R2:R" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(S2:S" + (ctr + 3) + "),2)</td> </b></tr>";
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
    #endregion

    public class MyTemplate_finc : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        int counter;
        static int ctr;

        public MyTemplate_finc(ListItemType type, DataSet ds, int counter)
        {
            this.type = type;
            this.ds = ds;
            ctr = 0;
            this.counter = counter;

        }

        public void InstantiateIn(Control container)
        {

            string header = "";
            switch (type)
            {
                case ListItemType.Header:

                    if (counter == 11)//shiftwise
                    {
                        lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=16>Finance Copy Reports</th></tr><tr><th>SR NO.</th><th>Client</th><th>Invoice No</th><th>Billing Date</th><th>StateName</th><th>Unit Name</th><th>Unit GST No</th><th>Emp Name</th><th>Designation</th><th>Shift Count</th><th>Grand Total</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Total GST</th><th>Total CTC</th> ");

                    }

                    //else if (counter == 8)// R & M Services
                    //{
                    //    int colspan = 19;

                    //    if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                    //    {
                    //        header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE <br style=\"mso-data-placement:same-cell;\">@" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                    //        colspan = 21;
                    //    }
                    //    else
                    //    {
                    //        header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                    //        colspan = 20;
                    //    }

                    //    lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan + " bgcolor=yellow align=center>FINANCE COPY  FOR R&M Services</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>BILL <br style=\"mso-data-placement:same-cell;\">NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th><th>BRANCH NAME</th><th>ZONE</th><th>REGION</th><th>STATE</th><th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "DISTRICT" : "CITY") + "</th><th>PARTY NAME</th><th>IMG  <br style=\"mso-data-placement:same-cell;\">TICKET NO.</th><th>UTR<br style=\"mso-data-placement:same-cell;\">NUMBER</th><th>TOTAL  <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");

                    //}
                    else if (counter == 9)// Administrative Expences
                    {
                        int colspan = 20;

                        if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE <br style=\"mso-data-placement:same-cell;\">@" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                            colspan = 21;
                        }
                        else
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                            colspan = 20;
                        }

                        lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan + " bgcolor=yellow align=center>FINANCE COPY  FOR ADMINISTRATIVE EXPENCES</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>BILL <br style=\"mso-data-placement:same-cell;\">NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th><th>BRANCH NAME</th><th>ZONE</th><th>REGION</th><th>STATE</th><th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "DISTRICT" : "CITY") + "</th><th>PARTY NAME</th><th>UTR<br style=\"mso-data-placement:same-cell;\">NUMBER</th><th>DAYS</th><th>TOTAL  <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");

                    }

                    else
                    {
                        lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=16>Finance Copy Reports</th></tr><tr><th>SR NO.</th><th>Client</th><th>Invoice No</th><th>Billing Date</th><th>StateName</th><th>Unit Name</th><th>Unit GST No</th><th>Emp Name</th><th>Designation</th><th>Present Days</th><th>Grand Total</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Total GST</th><th>Total CTC</th> ");

                    }

                    break;
                case ListItemType.Item:
                   
                    //  if (counter == 8)
                    //{
                       

                    //    lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["help_req_number"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["utr_number"].ToString().ToUpper() + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td></tr>");

                    //    if (ds.Tables[0].Rows.Count == ctr + 1)
                    //    {
                    //        lc.Text = lc.Text + "<tr><b><td align=center colspan = 13>Total</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(R3:R" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(S3:S" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(T3:T" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(U3:U" + (ctr + 3) + "),2)</td></b></tr>";

                    //    }


                    //}
                       if (counter == 9)
                      {

                          lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["utr_number"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["days"].ToString().ToUpper() + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td></tr>");

                          if (ds.Tables[0].Rows.Count == ctr + 1)
                          {
                              lc.Text = lc.Text + "<tr><b><td align=center colspan = 12>Total</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(R3:R" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(S3:S" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(T3:T" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(U3:U" + (ctr + 3) + "),2)</td></b></tr>";

                          }

                      }
                      else
                      {
                          lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"] + "</td><td>" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["billing_date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["unit_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_gst_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["tot_days_present"] + "</td><td>" + ds.Tables[0].Rows[ctr]["grandTotal"] + "</td><td>" + ds.Tables[0].Rows[ctr]["CGST9"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sgst9"] + "</td><td>" + ds.Tables[0].Rows[ctr]["igst18"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Totalgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Total_CTC"] + "</td></tr>");

                          if (ds.Tables[0].Rows.Count == ctr + 1)
                          {
                              lc.Text = lc.Text + "<tr><b><td align=center colspan = 10>Total</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td></b></tr>";
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


    protected string get_start_date()
    {
        return d1.getsinglestring("SELECT IFNULL((SELECT start_date_common FROM pay_billing_master_history INNER JOIN pay_unit_master ON pay_billing_master_history.billing_unit_code = pay_unit_master.unit_code AND pay_billing_master_history.comp_code = pay_unit_master.comp_code WHERE pay_billing_master_history.billing_client_code = '" + ddl_client.SelectedValue + "' AND month = '" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' and  pay_billing_master_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' limit 1),(SELECT start_date_common FROM pay_billing_master INNER JOIN pay_unit_master ON pay_billing_master.billing_unit_code = pay_unit_master.unit_code AND pay_billing_master.comp_code = pay_unit_master.comp_code WHERE pay_billing_master.billing_client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' limit 1))");
    }

    private StringWriter generate_report(int i, int type_cl, string billing_type1)
    {

        string month_name = "";
        string where_state = "", region_order = "";
        if (ddl_state.SelectedValue.Equals("Maharashtra") && type_cl.Equals(0) && ddl_client.SelectedValue.Equals("BAGIC") && int.Parse(("" + txt_date.Text.Substring(3) + "" + txt_date.Text.Substring(0, 2) + "")) > 20204 && billing_type1.Equals("1")) { where_state = " and state='" + ddl_state.SelectedValue + "' and billingwise_id = 5"; }
        string billing_bfl = "";
        DateTimeFormatInfo mfi = new DateTimeFormatInfo();
        month_name = mfi.GetMonthName(int.Parse(txt_date.Text.Substring(0, 2))).ToString();
        month_name = month_name + " " + txt_date.Text.Substring(3).ToUpper();
        string where = "";
        string sql = null;
        string invoice = "";
        string bill_date = "", billing_type = "And (bill_type is null || bill_type ='')";
        int month_days = 0;
        string start_date_common = get_start_date();
        string grade = "";
        string where_clause = "", where_fix = "", where_emp = "";


        d.con.Open();
        try
        {
            //if (ddl_billtype_financecopy.SelectedValue == "2")
            //{
            //    grade = " and pay_billing_unit_rate_history.grade_code = '" + ddl_designation.SelectedValue + "'";

            //}
            //if (ddl_billtype_financecopy.SelectedValue == "2" && ddl_arrears_type.SelectedValue != "Select")
            //{
            //    grade = " and pay_billing_unit_rate_history_arrears.grade_code = '" + ddl_designation.SelectedValue + "'";

            //}


            if (i == 1)//finance copy--Manpower
            {
                #region Manpower Finance_Copy
                string rg_terms = "";
                if (ddl_client.SelectedValue == "RCPL")
                {
                    rg_terms = "AND (emp_code != '' OR emp_code IS NOT NULL)";
                }
                string start_end_date = "AND (start_date = 0 AND end_date = 0) " + billing_type;
                //if (ddl_start_date_common.SelectedValue != "0" && ddl_end_date_common.SelectedValue != "0")
                //{
                //    start_end_date = "AND (start_date = " + ddl_start_date_common.SelectedValue + " AND end_date = " + ddl_end_date_common.SelectedValue + ") " + billing_type;
                //}

                if (ddl_client.SelectedValue == "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "'   and month='" + txt_date.Text.Substring(0, 2) + "'  " + billing_bfl + "  and year = '" + txt_date.Text.Substring(3) + "'  and flag != 0 " + where_state + rg_terms + " Group by Id order by invoice_no";
                }
                else
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "'  and month='" + txt_date.Text.Substring(0, 2) + "'  " + billing_bfl + "  and year = '" + txt_date.Text.Substring(3) + "'   and flag != 0 " + where_state + rg_terms + " Group by Id  order by invoice_no";
                }

                if (ddl_state.SelectedValue == "ALL" && ddl_client.SelectedValue != "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "'  and month='" + txt_date.Text.Substring(0, 2) + "'  " + billing_bfl + "  and year = '" + txt_date.Text.Substring(3) + "'   and flag != 0 " + where_state + rg_terms + "  Group by Id  order by invoice_no";
                }
                else if (ddl_state.SelectedValue != "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name= '" + ddl_state.SelectedItem.Text + "'  and month='" + txt_date.Text.Substring(0, 2) + "'  " + billing_bfl + "  and year = '" + txt_date.Text.Substring(3) + "'   and flag != 0 " + where_state + rg_terms + "  Group by Id  order by invoice_no";
                }
                if (ddl_unitcode.SelectedValue == "ALL" && ddl_client.SelectedValue != "ALL" && ddl_state.SelectedValue != "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name= '" + ddl_state.SelectedItem.Text + "'  and month='" + txt_date.Text.Substring(0, 2) + "'  " + billing_bfl + "  and year = '" + txt_date.Text.Substring(3) + "'   and flag != 0 " + where_state + rg_terms + " Group by Id  order by invoice_no";
                }
                else if (ddl_unitcode.SelectedValue != "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name= '" + ddl_state.SelectedItem.Text + "' and unit_name='" + ddl_unitcode.SelectedItem.Text + "'  and month='" + txt_date.Text.Substring(0, 2) + "'  " + billing_bfl + "  and year = '" + txt_date.Text.Substring(3) + "'   and flag != 0 " + where_state + rg_terms + "  Group by Id  order by invoice_no";
                }

                if (ddl_client.SelectedValue == "HDFC")
                {
                    where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.unit_code='" + ddl_unitcode.SelectedValue + "' and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' and hdfc_type='manpower_bill' " + grade + " and pay_billing_unit_rate_history.flag != 0 " + start_end_date + "  group by pay_billing_unit_rate_history.unit_code,grade_desc  order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
                    if (ddl_state.SelectedValue == "ALL")
                    {
                        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "'  and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' and hdfc_type='manpower_bill' " + grade + " and pay_billing_unit_rate_history.flag != 0 " + start_end_date + " group by pay_billing_unit_rate_history.unit_code,grade_desc order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
                    }
                    else if (ddl_unitcode.SelectedValue == "ALL")
                    {
                        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.state_name = '" + ddl_state.SelectedValue + "'  and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' and hdfc_type='manpower_bill' " + grade + "  and pay_billing_unit_rate_history.flag != 0 " + start_end_date + " group by pay_billing_unit_rate_history.unit_code,grade_desc order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
                    }
                    // sql = "SELECT CASE WHEN invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', pay_billing_unit_rate_history.client_code, client, pay_billing_unit_rate_history.state_name, pay_billing_unit_rate_history.branch_type, pay_billing_unit_rate_history.unit_name, pay_billing_unit_rate_history.unit_city, pay_billing_unit_rate_history.client_branch_code, emp_name, grade_desc, emp_basic_vda, SUM(hra) AS 'hra', SUM(bonus_gross) AS 'bonus_gross', SUM(leave_gross) AS 'leave_gross', SUM(gratuity_gross) AS 'gratuity_gross', SUM(washing) AS 'washing', SUM(travelling) AS 'travelling', SUM(education) AS 'education', SUM(allowances) AS 'allowances', SUM(cca_billing) AS 'cca_billing', SUM(other_allow) AS 'other_allow', SUM(gross) AS 'gross', SUM(bonus_after_gross) AS 'bonus_after_gross', SUM(leave_after_gross) AS 'leave_after_gross', SUM(gratuity_after_gross) AS 'gratuity_after_gross', SUM(pf) AS 'pf', SUM(esic) AS 'esic', SUM(hrs_12_ot) AS 'hrs_12_ot', SUM(esic_ot) AS 'esic_ot', SUM(lwf) AS 'lwf', SUM(uniform) AS 'uniform', SUM(relieving_charg) AS 'relieving_charg', SUM(operational_cost) AS 'operational_cost', SUM(tot_days_present) AS 'tot_days_present', SUM(Amount) AS 'Amount', SUM(Service_charge) AS 'Service_charge', SUM(CGST9) AS 'CGST9', SUM(IGST18) AS 'IGST18', SUM(SGST9) AS 'SGST9', bill_service_charge, NH, hours, fromtodate, (amount * month_days/tot_days_present) as 'sub_total_c', MAX(ot_rate) AS 'ot_rate', SUM(ot_hours) AS 'ot_hours', SUM(ot_amount) AS 'ot_amount', group_insurance_billing, bill_service_charge_amount, pay_billing_unit_rate_history.txt_zone, pay_billing_unit_rate_history.adminhead_name, ihms, pay_billing_unit_rate_history.location_type, pay_billing_unit_rate_history.unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, pay_billing_unit_rate_history.branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) AS 'total_emp_count', SUM(no_of_duties) AS 'no_of_duties', pay_billing_unit_rate_history.zone, TOT_WORKING_DAYS, GRADE_CODE, month_days, material_area,(SELECT  field2 FROM pay_zone_master WHERE pay_zone_master.comp_code = pay_billing_unit_rate_history.comp_code AND pay_zone_master.CLIENT_CODE = pay_billing_unit_rate_history.CLIENT_CODE AND pay_zone_master.ZONE = pay_unit_master.txt_zone AND type = 'ZONE' AND field1 = 'admin') AS 'zonal_name' FROM pay_billing_unit_rate_history INNER JOIN pay_unit_master ON pay_billing_unit_rate_history.comp_code = pay_unit_master.comp_code AND pay_billing_unit_rate_history.unit_code = pay_unit_master.unit_code " + where;
                }

                if (ddl_client.SelectedValue == "BAGICTM")
                {
                    where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.unit_code='" + ddl_unitcode.SelectedValue + "' and pay_billing_unit_rate_history.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' and hdfc_type is null  " + grade + "  and flag != 0  " + rg_terms + " " + start_end_date;
                    if (ddl_state.SelectedValue == "ALL")
                    {
                        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_unit_rate_history.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' " + grade + " and hdfc_type is null  and flag != 0 " + rg_terms + " " + start_end_date;
                    }
                    else if (ddl_unitcode.SelectedValue == "ALL")
                    {
                        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.state_name = '" + ddl_state.SelectedValue + "'  and pay_billing_unit_rate_history.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' " + grade + " and hdfc_type is null  and flag != 0  " + rg_terms + " " + start_end_date;
                    }

                    //sql = "SELECT txt_zone,zone,CASE WHEN invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_unit_rate_history.client_code, CASE WHEN pay_billing_unit_rate_history.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE client END AS 'client', state_name, unit_name, unit_city, client_branch_code, pay_billing_unit_rate_history.emp_name, grade_desc, emp_basic_vda, hra, bonus_gross, leave_gross, gratuity_gross, washing, travelling, education, allowances, cca_billing, other_allow, gross, bonus_after_gross, leave_after_gross, gratuity_after_gross, pf, esic, hrs_12_ot, esic_ot, lwf, uniform, relieving_charg, operational_cost, tot_days_present, amount AS 'Amount', Service_charge,CGST9, SGST9, IGST18, bill_service_charge, NH, hours, fromtodate, (amount * month_days / tot_days_present) AS 'sub_total_c', ot_rate, ot_hours, ot_amount, group_insurance_billing, bill_service_charge_amount, bill_service_charge_amount, branch_type, month_days, gst_applicable, OPus_NO, pay_billing_unit_rate_history.unit_code, conveyance_amount AS 'conveyance_rate'  FROM  pay_billing_unit_rate_history " + where + " order by 7,8,11";
                }
                //else
                //{
                sql = "SELECT client, invoice_no, IF(invoice_flag = 1, DATE_FORMAT(billing_date, '%d/%m/%Y'), '') billing_date, state_name, unit_name, unit_gst_no, IF(client_code = 'HDFC', '',emp_name) as emp_name, grade_desc, IF(client_code = 'HDFC', SUM(tot_days_present), tot_days_present) AS tot_days_present, IF(client_code = 'HDFC', SUM(ROUND((Amount + service_charge + operational_cost + uniform + ot_amount + group_insurance_billing),   2)), ROUND((Amount + service_charge + operational_cost + uniform + ot_amount + group_insurance_billing),   2)) AS grandTotal, ROUND(IF(client_code = 'HDFC',   SUM(IFNULL(CGST9, 0)),   IFNULL(CGST9, 0)),   2) AS CGST9, ROUND(IF(client_code = 'HDFC',   SUM(IFNULL(sgst9, 0)),   IFNULL(sgst9, 0)),   2) AS sgst9, ROUND(IF(client_code = 'HDFC',   SUM(IFNULL(igst18, 0)),   IFNULL(igst18, 0)),   2) AS igst18,IF(client_code = 'HDFC', SUM(ROUND((IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0)),   2)), ROUND((IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0)),   2)) AS Totalgst,     IF(client_code = 'HDFC', SUM(ROUND(Amount + service_charge + operational_cost + uniform + ot_amount + group_insurance_billing + IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0),2)), ROUND(Amount + service_charge + operational_cost + uniform + ot_amount + group_insurance_billing + IFNULL(CGST9, 0) + IFNULL(sgst9, 0) + IFNULL(igst18, 0), 2)) AS Total_CTC FROM pay_billing_unit_rate_history  " + where + "  ";
                //}
                #endregion
            }
            else if (i == 11)// Shiftwise finance copy
            {
                #region shiftwise
                string start_end_date = "AND (start_date = 0 AND end_date = 0) ";
                //if (ddl_start_date_common.SelectedValue != "0" && ddl_end_date_common.SelectedValue != "0")
                //{
                //    start_end_date = "AND (start_date = " + ddl_start_date_common.SelectedValue + " AND end_date = " + ddl_end_date_common.SelectedValue + ") ";
                //}
                if (ddl_client.SelectedValue == "ALL")
                {
                    where = "where pay_billing_shiftwise.comp_code='" + Session["comp_code"].ToString() + "'   and pay_billing_shiftwise.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_shiftwise.year = '" + txt_date.Text.Substring(3) + "' " + start_end_date;
                }
                else
                {
                    where = "where pay_billing_shiftwise.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_shiftwise.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_shiftwise.year = '" + txt_date.Text.Substring(3) + "' " + start_end_date;

                    if (ddl_state.SelectedValue != "ALL")
                    {
                        where = "where pay_billing_shiftwise.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.state_name = '" + ddl_state.SelectedValue + "'  and pay_billing_shiftwise.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_shiftwise.year = '" + txt_date.Text.Substring(3) + "'   " + start_end_date;
                    }
                    if (ddl_unitcode.SelectedValue != "ALL")
                    {
                        where = "where pay_billing_shiftwise.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_shiftwise.state_name = '" + ddl_state.SelectedValue + "' and  pay_billing_shiftwise.unit_code='" + ddl_unitcode.SelectedValue + "'  and pay_billing_shiftwise.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_shiftwise.year = '" + txt_date.Text.Substring(3) + "'  " + start_end_date;
                    }
                }

                sql = "SELECT  CASE  WHEN pay_billing_shiftwise.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD'         ELSE pay_billing_shiftwise.client     END AS 'client',  auto_invoice_no AS 'invoice_no',   CASE         WHEN pay_billing_shiftwise.invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y')         ELSE ''     END AS 'billing_date',   pay_billing_shiftwise.state_name,     pay_billing_shiftwise.unit_name,     unit_gst_no,      pay_billing_shiftwise.emp_name,     grade_desc,     shift_days as tot_days_present,     ROUND((pay_billing_shiftwise.amount+Service_charge),2) as grandTotal,     CGST9,     SGST9,     IGST18, 	ROUND((CGST9+SGST9+IGST18),2) as Totalgst, 	ROUND((pay_billing_shiftwise.amount+Service_charge+CGST9+SGST9+IGST18)) as  Total_CTC   FROM pay_billing_shiftwise INNER JOIN pay_shift_details ON pay_shift_details.comp_code = pay_billing_shiftwise.comp_code AND pay_shift_details.client_code = pay_billing_shiftwise.client_code AND pay_shift_details.unit_code = pay_billing_shiftwise.unit_code AND pay_shift_details.month = pay_billing_shiftwise.month AND pay_shift_details.year = pay_billing_shiftwise.year AND pay_shift_details.EMP_CODE = pay_billing_shiftwise.EMP_CODE  " + where + " group by auto_invoice_no,pay_shift_details.EMP_CODE";

                #endregion
            }
            else if (i == 2)//material finance copy
            {
                #region material finance copy
                where_fix = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_material_history.unit_code = '" + ddl_unitcode.SelectedValue + "' AND pay_billing_material_history.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_billing_material_history.Year = '" + txt_date.Text.Substring(3) + "' ";
                where_clause = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + ddl_unitcode.SelectedValue + "' and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "'  ";
                where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and unit_code = '" + ddl_unitcode.SelectedValue + "' and month = '" + txt_date.Text.Substring(0, 2) + "' and Year = '" + txt_date.Text.Substring(3) + "' and tot_days_present > 0  and material_contract != 0 AND grade_code in ('HK','HKSR') GROUP BY unit_code  ORDER BY STATE_NAME, UNIT_NAME ";
                where_emp = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_material_history.unit_code = '" + ddl_unitcode.SelectedValue + "' and pay_billing_material_history.month = '" + txt_date.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_date.Text.Substring(3) + "' and tot_days_present > 0  and material_contract != 0 and grade_code = 'HK' AND pay_material_details.material_flag = '2' GROUP BY unit_code,pay_billing_material_history.emp_code  ORDER BY STATE_NAME, UNIT_NAME ";
                if (ddl_state.SelectedValue == "ALL")
                {
                    where_fix = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_material_history.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_billing_material_history.Year = '" + txt_date.Text.Substring(3) + "' ";
                    where_clause = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "'  and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' ";
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and month = '" + txt_date.Text.Substring(0, 2) + "' and Year = '" + txt_date.Text.Substring(3) + "' and tot_days_present > 0 and material_contract != 0 AND grade_code in ('HK','HKSR') GROUP BY unit_code ORDER BY STATE_NAME, UNIT_NAME ";
                    where_emp = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "'  and pay_billing_material_history.month = '" + txt_date.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_date.Text.Substring(3) + "' and tot_days_present > 0  and material_contract != 0 and grade_code = 'HK' AND pay_material_details.material_flag = '2' GROUP BY unit_code,pay_billing_material_history.emp_code  ORDER BY STATE_NAME, UNIT_NAME ";
                }
                else if (ddl_unitcode.SelectedValue == "ALL")
                {
                    where_fix = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "'  AND pay_billing_material_history.state_name = '" + ddl_state.SelectedValue + "' AND pay_billing_material_history.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_billing_material_history.Year = '" + txt_date.Text.Substring(3) + "' ";
                    where_clause = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_state.SelectedValue + "'  and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' ";
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_state.SelectedValue + "' and month = '" + txt_date.Text.Substring(0, 2) + "' and Year = '" + txt_date.Text.Substring(3) + "' and tot_days_present > 0 and material_contract != 0 AND grade_code in ('HK','HKSR') GROUP BY unit_code ORDER BY STATE_NAME, UNIT_NAME ";
                    where_emp = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_material_history.state_name = '" + ddl_state.SelectedValue + "' and pay_billing_material_history.month = '" + txt_date.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_date.Text.Substring(3) + "' and tot_days_present > 0  and material_contract != 0 and grade_code = 'HK' AND pay_material_details.material_flag = '2' GROUP BY unit_code,pay_billing_material_history.emp_code  ORDER BY STATE_NAME, UNIT_NAME ";
                }

                if (d.getsinglestring("select max(material_contract) from pay_billing_material_history   " + where_clause + " limit  1").Equals("3"))
                {
                    //query = "SELECT CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND pay_billing_material_history.month >= 4 AND pay_billing_material_history.year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE '' END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', material_contract, contract_type, COMP_STATE AS 'STATE', pay_billing_material_history.unit_code, pay_billing_material_history.fromtodate, pay_billing_material_history.STATE_NAME, pay_billing_material_history.CLIENT_CODE, UNIT_NAME, Client_branch_code, grade_desc AS 'designation', material_name, rate, quantity, ROUND(rate * quantity, 2) AS 'total', CASE WHEN pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0 THEN ROUND((((rate * quantity) * pay_material_billing_details.handling_percent) / 100), 2) WHEN pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_charges_amount > 0 THEN pay_billing_material_history.handling_charges_amount ELSE 0 END AS 'handling_charge', pay_material_billing_details.handling_percent, round(IF(gst_applicable = 1 AND LOCATE(COMP_STATE, STATE_NAME), IF(material_contract = 3, (((rate * quantity) + IF(material_contract = 3 AND pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0, (((rate * quantity) * pay_material_billing_details.handling_percent) / 100), pay_billing_material_history.handling_charges_amount)) * 9) / 100, 0), 0),2) AS 'SGST', round(IF(gst_applicable = 1 AND LOCATE(COMP_STATE, STATE_NAME), IF(material_contract = 3, (((rate * quantity) + IF(material_contract = 3 AND pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0, (((rate * quantity) * pay_material_billing_details.handling_percent) / 100), pay_billing_material_history.handling_charges_amount)) * 9) / 100, 0), 0),2) AS 'CGST', round(IF(gst_applicable = 1 AND LOCATE(COMP_STATE, STATE_NAME) != 1, IF(material_contract = 3, (((rate * quantity) + IF(material_contract = 3 AND pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0, (((rate * quantity) * pay_material_billing_details.handling_percent) / 100), pay_billing_material_history.handling_charges_amount)) * 18) / 100, 0), 0),2) AS 'IGST' from  pay_billing_material_history INNER JOIN pay_material_billing_details ON pay_billing_material_history.comp_code = pay_material_billing_details.comp_Code AND pay_billing_material_history.client_code = pay_material_billing_details.client_code AND pay_billing_material_history.state_name = pay_material_billing_details.state AND pay_billing_material_history.unit_code = pay_material_billing_details.unit_code1 AND pay_billing_material_history.month = pay_material_billing_details.month AND pay_billing_material_history.year = pay_material_billing_details.year WHERE " + where_fix + " AND pay_billing_material_history.tot_days_present > 0 AND pay_billing_material_history.material_contract = 3 AND grade_code = 'HK' GROUP BY pay_billing_material_history.unit_code, Id_material ORDER BY UNIT_NAME  ";
                    sql = " SELECT CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND pay_billing_material_history.month >= 4 AND pay_billing_material_history.year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE '' END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', material_contract, contract_type, COMP_STATE AS 'STATE', pay_billing_material_history.unit_code, pay_billing_material_history.fromtodate, pay_billing_material_history.STATE_NAME, pay_billing_material_history.CLIENT_CODE, UNIT_NAME, Client_branch_code, grade_desc AS 'designation', material_name, rate, quantity, ROUND(rate * quantity, 2) AS 'total', CASE WHEN pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0 THEN ROUND((((rate * quantity) * pay_material_billing_details.handling_percent) / 100), 2) WHEN pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_charges_amount > 0 THEN pay_billing_material_history.handling_charges_amount ELSE 0 END AS 'handling_charge', pay_material_billing_details.handling_percent, SGST, CGST, IGST from  pay_billing_material_history INNER JOIN pay_material_billing_details ON pay_billing_material_history.comp_code = pay_material_billing_details.comp_Code AND pay_billing_material_history.client_code = pay_material_billing_details.client_code AND pay_billing_material_history.state_name = pay_material_billing_details.state AND pay_billing_material_history.unit_code = pay_material_billing_details.unit_code1 AND pay_billing_material_history.month = pay_material_billing_details.month AND pay_billing_material_history.year = pay_material_billing_details.year WHERE " + where_fix + " AND pay_billing_material_history.tot_days_present > 0 AND pay_billing_material_history.material_contract = 3 AND grade_code = 'HK' GROUP BY pay_billing_material_history.unit_code, Id_material ORDER BY UNIT_NAME  ";
                }
                else if (d.getsinglestring("select max(material_contract) from pay_billing_material_history   " + where_clause + " limit  1").Equals("4"))
                {
                    sql = "SELECT  client, CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'invoice_no',  CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date',STATE_NAME as state_name,  UNIT_NAME as unit_name, unit_gst_no,emp_name,    grade_desc ,    tot_days_present,    IF(material_contract != 0 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) AS 'grandTotal',  ROUND(CGST,2) as CGST9, ROUND(SGST,2) as sgst9, ROUND(IGST,2) as igst18, ROUND((CGST+SGST+IGST),2) as Totalgst, ROUND( IF(material_contract != 0 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2))+CGST+SGST+IGST , 2) as Total_CTC  FROM pay_billing_material_history WHERE " + where_emp;
                }
                else
                {
                    sql = "SELECT  client, CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'invoice_no',  CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date',STATE_NAME as state_name,  UNIT_NAME as unit_name, unit_gst_no,emp_name,    grade_desc ,    tot_days_present,    IF(material_contract != 0 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) AS 'grandTotal',  ROUND(CGST,2) as CGST9, ROUND(SGST,2) as sgst9, ROUND(IGST,2) as igst18, ROUND((CGST+SGST+IGST),2) as Totalgst, ROUND( IF(material_contract != 0 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2))+CGST+SGST+IGST , 2) as Total_CTC  FROM pay_billing_material_history WHERE " + where;
                    //query = "SELECT material_contract, CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND pay_billing_material_history.month >= 4 AND pay_billing_material_history.year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE '' END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', contract_type, COMP_STATE AS 'STATE', unit_code, STATE_NAME, CLIENT_CODE, UNIT_NAME, Client_branch_code, grade_desc AS 'designation', handling_percent, material_area, IF(material_contract != 0, contract_amount, 0) AS 'rate', IF(material_contract = 2, ROUND(contract_amount * material_area, 2), ROUND(contract_amount, 2)) AS 'sub_total', IF(material_contract = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) AS 'total', IF(handling_applicable = 1, (IF(material_contract = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0) AS 'handling_charge', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME), ((IF(material_contract = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 9) / 100, 0) AS 'SGST', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME), ((IF(material_contract = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 9) / 100, 0) AS 'CGST', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME) != 1, ((IF(material_contract = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 18) / 100, 0) AS 'IGST', machine_rental_amount, machine_rental_applicable, fromtodate FROM pay_billing_material_history WHERE " + where;
                    //  query = "SELECT material_contract,CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND pay_billing_material_history.month >= 4 AND pay_billing_material_history.year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE '' END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', contract_type,COMP_STATE AS 'STATE',unit_code, STATE_NAME, CLIENT_CODE, UNIT_NAME, Client_branch_code, grade_desc AS 'designation', handling_percent, material_area, IF(material_contract = 2, contract_amount, 0) AS 'rate', IF(material_contract = 2 AND contract_type = 2, ROUND(contract_amount * material_area, 2), ROUND(contract_amount, 2)) AS 'sub_total', IF(material_contract = 2 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) AS 'total', IF(handling_applicable = 2, (IF(material_contract = 1 AND contract_type = 2, ROUND(((contract_amount * material_area) / month_days), 2), (ROUND(contract_amount, 2) * handling_percent) / 100)), 0) AS 'handling_charge', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME), ((IF(material_contract = 2 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 1 AND contract_type = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 9) / 100, 0) AS 'SGST', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME), ((IF(material_contract = 2 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 1 AND contract_type = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 9) / 100, 0) AS 'CGST', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME) != 1, ((IF(material_contract = 2 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 1 AND contract_type = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 18) / 100, 0) AS 'IGST', machine_rental_amount, machine_rental_applicable, fromtodate FROM pay_billing_material_history WHERE " + where;
                }
                #endregion
            }
            else if (i==4)//Deep Cleaning F.C
            {
                #region Deep Cleaning
                if (ddl_client.SelectedValue != "ALL")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and unit_code = '" + ddl_unitcode.SelectedValue + "' and month = '" + txt_date.Text.Substring(0, 2) + "' and Year = '" + txt_date.Text.Substring(3) + "' and tot_days_present > 0 AND dc_contract = 1  GROUP BY unit_code  ORDER BY STATE_NAME, UNIT_NAME";
                }
                if (ddl_state.SelectedValue == "ALL")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and month = '" + txt_date.Text.Substring(0, 2) + "' and Year = '" + txt_date.Text.Substring(3) + "' and tot_days_present > 0 AND dc_contract = 1 GROUP BY unit_code ORDER BY STATE_NAME, UNIT_NAME ";
                }
                else if (ddl_unitcode.SelectedValue == "ALL")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_state.SelectedValue + "' and month = '" + txt_date.Text.Substring(0, 2) + "' and Year = '" + txt_date.Text.Substring(3) + "' and tot_days_present > 0 AND dc_contract = 1 GROUP BY unit_code ORDER BY STATE_NAME, UNIT_NAME ";
                }
                if (ddl_client.SelectedValue == "ALL")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "'  and month = '" + txt_date.Text.Substring(0, 2) + "' and Year = '" + txt_date.Text.Substring(3) + "' and tot_days_present > 0 AND dc_contract = 1  GROUP BY unit_code  ORDER BY client,STATE_NAME, UNIT_NAME";
                }
                sql = "SELECT  client, CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', STATE_NAME as state_name,    UNIT_NAME as unit_name,    unit_gst_no,emp_name,    grade_desc ,tot_days_present, IF(dc_contract = 1 AND dc_type = 2, (dc_rate * dc_area), dc_rate) AS 'grandTotal',    CGST as CGST9,  SGST as sgst9, IGST as igst18, ROUND((CGST+SGST+IGST),2) as Totalgst,    ROUND( IF(dc_contract = 1 AND dc_type = 2, (dc_rate * dc_area),  dc_rate) +CGST+SGST+IGST,2) as Total_CTC FROM pay_billing_material_history  WHERE " + where;

                #endregion
            }
            else if (i == 3)//Conveyance
            {
                #region Conveyance
                if (ddl_conveyance_type.SelectedValue=="2")//Driver Conveyance
                {
                    if (ddl_client.SelectedValue != "ALL")
                    {
                        where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_material_history.unit_code = '" + ddl_unitcode.SelectedValue + "' and pay_billing_material_history.month = '" + txt_date.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_date.Text.Substring(3) + "'  and pay_billing_material_history.conveyance_type = 100  ORDER BY state_name,unit_name,emp_name ";
                    }
                    if (ddl_state.SelectedValue == "ALL")
                    {
                        where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_material_history.month = '" + txt_date.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_date.Text.Substring(3) + "'  and pay_billing_material_history.conveyance_type = 100 ORDER BY STATE_NAME, UNIT_NAME  ";
                    }
                    else if (ddl_unitcode.SelectedValue == "ALL")
                    {
                        where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_material_history.state_name = '" + ddl_state.SelectedValue + "' and pay_billing_material_history.month = '" + txt_date.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_date.Text.Substring(3) + "'  and pay_billing_material_history.conveyance_type = 100 ORDER BY state_name,unit_name,emp_name ";
                    }
                    if (ddl_client.SelectedValue == "ALL")
                    {
                        where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'  " + where_state + " and pay_billing_material_history.month = '" + txt_date.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_date.Text.Substring(3) + "'  and pay_billing_material_history.conveyance_type = 100 ORDER BY state_name,unit_name,emp_name ";
                    }
                    sql = "SELECT client, CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'invoice_no',CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date',STATE_NAME as state_name, UNIT_NAME as unit_name, unit_gst_no,  pay_billing_material_history.EMP_NAME as emp_name, grade_desc, tot_days_present, (((food_allowance_rate * food_allowance_days) + (outstation_allowance_rate * outstation_allowance_days) + (outstation_food_allowance_rate * outstation_food_allowance_days) + (night_halt_rate * night_halt_days) + (total_km)) + (((food_allowance_rate * food_allowance_days) + (outstation_allowance_rate * outstation_allowance_days) + (outstation_food_allowance_rate * outstation_food_allowance_days) + (night_halt_rate * night_halt_days) + (total_km)) * 5 / 100)) AS 'grandTotal', CGST as CGST9,SGST as sgst9, IGST as igst18,  Round(SGST+CGST+IGST ) as Totalgst   FROM pay_billing_material_history  INNER JOIN pay_conveyance_amount_history ON pay_conveyance_amount_history.emp_code = pay_billing_material_history.emp_code AND pay_conveyance_amount_history.comp_code = pay_billing_material_history.comp_code  AND pay_conveyance_amount_history.month = '" + txt_date.Text.Substring(0, 2) + "' AND pay_conveyance_amount_history.year = '" + txt_date.Text.Substring(3) + "' and  pay_conveyance_amount_history.conveyance = 'driver_conveyance' INNER JOIN pay_billing_master ON pay_billing_master.billing_unit_code = pay_billing_material_history.unit_code AND pay_billing_master.comp_code = pay_billing_material_history.comp_code AND pay_billing_master.designation = pay_billing_material_history.GRADE_CODE  WHERE " + where; 
                }
                else if (ddl_conveyance_type.SelectedValue == "1")//Employee Conveyance
                {
                    if (ddl_client.SelectedValue != "ALL")
                    {
                        where = "where comp_code='" + Session["comp_code"].ToString() + "' " + where_state + " and client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + ddl_unitcode.SelectedValue + "' and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "'  ";
                    }
                    if (ddl_state.SelectedValue == "ALL")
                    {
                        where = "where comp_code='" + Session["comp_code"].ToString() + "' " + where_state + " and client_code= '" + ddl_client.SelectedValue + "'  and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' ";
                    }
                    else if (ddl_unitcode.SelectedValue == "ALL")
                    {
                        where = "where comp_code='" + Session["comp_code"].ToString() + "' " + where_state + " and client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_state.SelectedValue + "'  and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' ";
                    }
                    if (ddl_client.SelectedValue == "ALL")
                    {
                        where = "where comp_code='" + Session["comp_code"].ToString() + "' " + where_state + "  and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' ";
                    }
                    //employeewies
                    if (d.getsinglestring("select max(conveyance_type) FROM pay_billing_material_history " + where + " and conveyance_type=3 limit 1").Equals("3"))
                    {
                        where = where + "  and Conveyance_Rate > 0 and conveyance_type !=0 and  conveyance_type != '100' group by emp_code ORDER BY CLIENT,state_name,unit_name,emp_name";
                    }
                    else
                    {
                        where = where + "  and Conveyance_PerKmRate > 0 and conveyance_type !=0 and conveyance_type != '100' group by emp_code ORDER BY client,state_name,unit_name,emp_name";
                    }
                    sql = "SELECT client,CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', STATE_NAME as state_name, UNIT_NAME as unit_name, unit_gst_no, EMP_NAME as emp_name , grade_desc,tot_days_present,  IF( conveyance_applicable  = 1 AND  conveyance_type  = 2, ( Conveyance_PerKmRate  *  conveyance_km ),  conveyance_rate ) AS grandTotal,   CGST as CGST9,  SGST as sgst9, IGST as igst18, (SGST+ CGST+ IGST) as Totalgst,   (IF( conveyance_applicable  = 1 AND  conveyance_type  = 2, ( Conveyance_PerKmRate  *  conveyance_km ),  conveyance_rate ) +SGST+ CGST+ IGST) as Total_CTC,  Conveyance_PerKmRate,IF(conveyance_type = 1, (conveyance_rate / Conveyance_PerKmRate), conveyance_km) AS 'conveyance_km'   FROM pay_billing_material_history " + where;
                }
                #endregion

            }

            else if (i == 8)//R&M finance copy
            {

                string start_end_date = "AND (start_date = 0 AND end_date = 0) ";
                //if (ddl_start_date_common.SelectedValue != "0" && ddl_end_date_common.SelectedValue != "0")
                //{
                //    start_end_date = "AND (start_date = " + ddl_start_date_common.SelectedValue + " AND end_date = " + ddl_end_date_common.SelectedValue + ") ";
                //}
                if (ddl_client.SelectedValue!="ALL")
                {
                    where = "where pay_billing_r_m.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_r_m.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_r_m.unit_code='" + ddl_unitcode.SelectedValue + "' and pay_billing_r_m.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_r_m.year = '" + txt_date.Text.Substring(3) + "'  and (approve_flag =1 || approve_flag =2)" + start_end_date;
              
                }
                if (ddl_state.SelectedValue == "ALL")
                {
                    where = "where pay_billing_r_m.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_r_m.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_r_m.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_r_m.year = '" + txt_date.Text.Substring(3) + "'  and (approve_flag =1 || approve_flag =2) " + start_end_date;
                }
                else if (ddl_unitcode.SelectedValue == "ALL")
                {
                    where = "where pay_billing_r_m.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_r_m.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_r_m.state_name = '" + ddl_state.SelectedValue + "'  and pay_billing_r_m.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_r_m.year = '" + txt_date.Text.Substring(3) + "'  and (approve_flag =1 || approve_flag =2) " + start_end_date;
                }

                sql = "SELECT  CASE WHEN pay_billing_r_m.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE pay_billing_r_m.client END AS 'client',  auto_invoice_no AS 'invoice_no',invoice_no in123, CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date',    pay_billing_r_m.state_name, pay_billing_r_m.unit_name, unit_gst_no,  pay_billing_r_m.emp_name,grade_desc, '' as tot_days_present,     ROUND(amount + Service_charge,2) as grandTotal, CGST9,SGST9 as sgst9, IGST18 as igst18,ROUND(CGST9+SGST9+IGST18,2) as Totalgst,     ROUND((amount + Service_charge+CGST9+SGST9+IGST18+bill_service_charge),2) as Total_CTC " + where + " group by pay_billing_r_m.id "; 
            }
            else if (i == 9)//Administrative Expense finance copy
            {

                string start_end_date = "AND (start_date = 0 AND end_date = 0) ";
                //if (ddl_start_date_common.SelectedValue != "0" && ddl_end_date_common.SelectedValue != "0")
                //{
                //    start_end_date = "AND (start_date = " + ddl_start_date_common.SelectedValue + " AND end_date = " + ddl_end_date_common.SelectedValue + ") ";
                //}
                if (ddl_client.SelectedValue != "ALL")
                {
                    where = "where pay_billing_admini_expense.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_admini_expense.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_admini_expense.unit_code='" + ddl_unitcode.SelectedValue + "' and pay_billing_admini_expense.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_admini_expense.year = '" + txt_date.Text.Substring(3) + "' AND (approve_flag = 1 || approve_flag = 2) " + start_end_date;
                }
                     if (ddl_state.SelectedValue == "ALL")
                {
                    where = "where pay_billing_admini_expense.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_admini_expense.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_admini_expense.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_admini_expense.year = '" + txt_date.Text.Substring(3) + "' AND (approve_flag = 1 || approve_flag = 2)  " + start_end_date;
                }
                else if (ddl_unitcode.SelectedValue == "ALL")
                {
                    where = "where pay_billing_admini_expense.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_admini_expense.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_admini_expense.state_name = '" + ddl_state.SelectedValue + "'  and pay_billing_admini_expense.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_admini_expense.year = '" + txt_date.Text.Substring(3) + "' AND (approve_flag = 1 || approve_flag = 2) " + start_end_date;
                }

                sql = "SELECT  txt_zone, zone, auto_invoice_no AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_admini_expense.client_code, CASE WHEN pay_billing_admini_expense.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE pay_billing_admini_expense.client END AS 'client', pay_billing_admini_expense.state_name, pay_billing_admini_expense.unit_name, pay_billing_admini_expense.unit_city, pay_billing_admini_expense.client_branch_code, pay_billing_admini_expense.emp_name, pay_billing_admini_expense.amount AS 'Amount', Service_charge, CGST9, IGST18, SGST9, bill_service_charge, fromtodate, bill_service_charge_amount, branch_type, gst_applicable, pay_billing_admini_expense.unit_code, pay_billing_admini_expense.days, utr_number FROM pay_billing_admini_expense INNER JOIN pay_administrative_expense ON pay_administrative_expense.comp_code = pay_billing_admini_expense.comp_code AND pay_administrative_expense.client_code = pay_billing_admini_expense.client_code AND pay_administrative_expense.unit_code = pay_billing_admini_expense.unit_code AND pay_administrative_expense.month = pay_billing_admini_expense.month AND pay_administrative_expense.year = pay_billing_admini_expense.year AND pay_administrative_expense.party_name = pay_billing_admini_expense.emp_name LEFT OUTER JOIN pay_pro_admini_expense ON pay_pro_admini_expense.comp_code = pay_billing_admini_expense.comp_code AND pay_pro_admini_expense.client_code = pay_billing_admini_expense.client_code AND pay_pro_admini_expense.unit_code = pay_billing_admini_expense.unit_code AND pay_pro_admini_expense.month = pay_billing_admini_expense.month AND pay_pro_admini_expense.year = pay_billing_admini_expense.year AND pay_pro_admini_expense.emp_code = pay_billing_admini_expense.emp_code  " + where + " group by pay_administrative_expense.id ";

            }

            #region
            //Arrears finance copy
            // else if (i == 7)
            //{
            //    string rg_terms = "";
            //    string where1 = "", month_list = "", year = "", new_yera = "", new_month = "", old_month_year = "", new_month_year = "";
            //    string order_by_clause1 = "   ORDER BY state_name,unit_name,emp_name";
            //    if (arrear_type == "policy")
            //    {
            //        new_month_year = "  month in (" + txt_date.Text.Substring(3, 2) + ") and year in (" + txt_date.Text.Substring(6) + ") ";
            //        old_month_year = "  month in (" + txt_date.Text.Substring(3, 2) + ") and year in (" + txt_date.Text.Substring(6) + ") ";
            //    }
            //    else
            //    {
            //        new_month_year = "  month in (" + txt_date.Text.Substring(0, 2) + ") and year in (" + txt_date.Text.Substring(3) + ") ";
            //        old_month_year = "  month in (" + txt_date.Text.Substring(0, 2) + ") and year in (" + txt_date.Text.Substring(3) + ") ";
            //    }
            //    if (ddl_client.SelectedValue == "RCPL")
            //    {
            //        rg_terms = "AND (emp_code != '' OR emp_code IS NOT NULL)";
            //    }
            //    string start_end_date = "AND (start_date = 0 AND end_date = 0) " + billing_type;
            //    if (ddl_start_date_common.SelectedValue != "0" && ddl_end_date_common.SelectedValue != "0")
            //    {
            //        start_end_date = "AND (start_date = " + ddl_start_date_common.SelectedValue + " AND end_date = " + ddl_end_date_common.SelectedValue + ") " + billing_type;
            //    }
            //    where1 = " and comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' " + where_state_arrears + " and unit_code='" + ddl_unitcode.SelectedValue + "'  " + grade + "  ";
            //    if (ddl_state.SelectedValue == "ALL")
            //    {
            //        where1 = " and comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "'  " + where_state_arrears + " " + grade + " ";
            //    }
            //    else if (ddl_unitcode.SelectedValue == "ALL")
            //    {
            //        where1 = " and  comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_state.SelectedValue + "' " + where_state_arrears + "  " + grade + "   ";
            //    }
            //    string multi = "";
            //    if (type_cl == 1)
            //    {
            //        multi = " and pay_billing_unit_rate_history_arrears.invoice_flag!=0 ";
            //        //if (ddl_state.SelectedValue.Equals("ALL") && state_name_arrear_state != "" && type_cl == 1)
            //        //{
            //        //    multi = multi + " and pay_billing_unit_rate_history_arrears.state_name in (" + state_name_arrear_state.Substring(0, state_name_arrear_state.Length - 1) + ") ";
            //        //}
            //    }
            //    if (ddl_client.SelectedValue == "HDFC")
            //    {
            //        where1 = multi + " and pay_billing_unit_rate_history_arrears.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history_arrears.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history_arrears.unit_code='" + ddl_unitcode.SelectedValue + "'  " + grade + "   group by pay_billing_unit_rate_history_arrears.unit_code,pay_billing_unit_rate_history_arrears.GRADE_CODE  order by pay_billing_unit_rate_history_arrears.state_name,pay_billing_unit_rate_history_arrears.unit_name";
            //        if (ddl_state.SelectedValue == "ALL")
            //        {
            //            where1 = multi + " and pay_billing_unit_rate_history_arrears.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history_arrears.client_code= '" + ddl_client.SelectedValue + "'   " + grade + "  group by pay_billing_unit_rate_history_arrears.unit_code,pay_billing_unit_rate_history_arrears.GRADE_CODE  order by pay_billing_unit_rate_history_arrears.state_name,pay_billing_unit_rate_history_arrears.unit_name";
            //        }
            //        else if (ddl_unitcode.SelectedValue == "ALL")
            //        {
            //            where1 = multi + " and pay_billing_unit_rate_history_arrears.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history_arrears.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history_arrears.state_name = '" + ddl_state.SelectedValue + "'   " + grade + "   group by pay_billing_unit_rate_history_arrears.unit_code,pay_billing_unit_rate_history_arrears.GRADE_CODE order by pay_billing_unit_rate_history_arrears.state_name,pay_billing_unit_rate_history_arrears.unit_name";
            //        }

            //        // sql = "SELECT CASE WHEN invoice_flag != 0 AND month <= 3 AND year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND month >= 4 AND year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE '' END AS 'bill_invoice_no', client_code, client, state_name,branch_type, unit_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, SUM(hra) as 'hra', SUM(bonus_gross) as 'bonus_gross', SUM(leave_gross) as 'leave_gross', SUM(gratuity_gross) as 'gratuity_gross', SUM(washing) as 'washing', SUM(travelling) as 'travelling', SUM(education) as 'education', SUM(allowances) as 'allowances', SUM(cca_billing) as 'cca_billing', SUM(other_allow) as 'other_allow', SUM(gross) as 'gross', SUM(bonus_after_gross) as 'bonus_after_gross', SUM(leave_after_gross) as 'leave_after_gross', SUM(gratuity_after_gross) as 'gratuity_after_gross', SUM(pf) as 'pf', SUM(esic) as 'esic', SUM(hrs_12_ot) as 'hrs_12_ot' , SUM(esic_ot) as 'esic_ot', SUM(lwf) as 'lwf', SUM(uniform) as 'uniform', SUM(relieving_charg) as 'relieving_charg', SUM(operational_cost) as 'operational_cost', SUM(tot_days_present) as 'tot_days_present',sum(Amount) as 'Amount', SUM(Service_charge) as 'Service_charge', SUM(CGST9) as 'CGST9', SUM(IGST18) as 'IGST18', SUM(SGST9) as 'SGST9', bill_service_charge , NH, hours, fromtodate,sub_total_c, max(ot_rate) as 'ot_rate', SUM(ot_hours) as 'ot_hours', SUM(ot_amount) as 'ot_amount', group_insurance_billing, bill_service_charge_amount, txt_zone, adminhead_name, ihms, location_type, unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) as 'total_emp_count', sum(no_of_duties) as 'no_of_duties', zone, TOT_WORKING_DAYS, GRADE_CODE, month_days,material_area FROM pay_billing_unit_rate_history_arrears where  " + old_month_year + "" + where1 + "  ";
            //        sql = "SELECT CASE WHEN invoice_flag != 0 AND month <= 3 AND year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND month >= 4 AND year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE auto_invoice_no END AS 'bill_invoice_no', pay_billing_unit_rate_history_arrears.client_code, client, pay_billing_unit_rate_history_arrears.state_name, pay_billing_unit_rate_history_arrears.branch_type, pay_billing_unit_rate_history_arrears.unit_name, pay_billing_unit_rate_history_arrears.unit_city, pay_billing_unit_rate_history_arrears.client_branch_code, emp_name, grade_desc, emp_basic_vda, SUM(hra) AS 'hra', SUM(bonus_gross) AS 'bonus_gross', SUM(leave_gross) AS 'leave_gross', SUM(gratuity_gross) AS 'gratuity_gross', SUM(washing) AS 'washing', SUM(travelling) AS 'travelling', SUM(education) AS 'education', SUM(allowances) AS 'allowances', SUM(cca_billing) AS 'cca_billing', SUM(other_allow) AS 'other_allow', SUM(gross) AS 'gross', SUM(bonus_after_gross) AS 'bonus_after_gross', SUM(leave_after_gross) AS 'leave_after_gross', SUM(gratuity_after_gross) AS 'gratuity_after_gross', SUM(pf) AS 'pf', SUM(esic) AS 'esic', SUM(hrs_12_ot) AS 'hrs_12_ot', SUM(esic_ot) AS 'esic_ot', SUM(lwf) AS 'lwf', SUM(uniform) AS 'uniform', SUM(relieving_charg) AS 'relieving_charg', SUM(operational_cost) AS 'operational_cost', SUM(tot_days_present) AS 'tot_days_present', SUM(Amount) AS 'Amount', SUM(Service_charge) AS 'Service_charge', SUM(CGST9) AS 'CGST9', SUM(IGST18) AS 'IGST18', SUM(SGST9) AS 'SGST9', bill_service_charge, NH, hours, fromtodate, (amount * month_days/tot_days_present) as 'sub_total_c', MAX(ot_rate) AS 'ot_rate', SUM(ot_hours) AS 'ot_hours', SUM(ot_amount) AS 'ot_amount', group_insurance_billing, bill_service_charge_amount, pay_billing_unit_rate_history_arrears.txt_zone, pay_billing_unit_rate_history_arrears.adminhead_name, ihms, pay_billing_unit_rate_history_arrears.location_type, pay_billing_unit_rate_history_arrears.unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, pay_billing_unit_rate_history_arrears.branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) AS 'total_emp_count', SUM(no_of_duties) AS 'no_of_duties', pay_billing_unit_rate_history_arrears.zone, TOT_WORKING_DAYS, GRADE_CODE, month_days,material_area,(SELECT field2 FROM pay_zone_master WHERE pay_zone_master.comp_code = pay_billing_unit_rate_history_arrears.comp_code AND pay_zone_master.CLIENT_CODE = pay_billing_unit_rate_history_arrears.CLIENT_CODE AND pay_zone_master.ZONE = pay_unit_master.txt_zone AND type = 'ZONE' AND field1 = 'admin') AS 'zonal_name' FROM pay_billing_unit_rate_history_arrears INNER JOIN pay_unit_master ON pay_billing_unit_rate_history_arrears.comp_code = pay_unit_master.comp_code AND pay_billing_unit_rate_history_arrears.unit_code = pay_unit_master.unit_code  where  " + old_month_year + "" + where1 + "  ";

            //    }
            //    else if (ddl_client.SelectedValue == "BAGICTM")
            //    {
            //        where1 = multi + " and comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + ddl_unitcode.SelectedValue + "'  " + grade + " ";
            //        if (ddl_state.SelectedValue == "ALL")
            //        {
            //            where1 = multi + " and comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "'   " + grade + "  ";
            //        }
            //        else if (ddl_unitcode.SelectedValue == "ALL")
            //        {
            //            where1 = multi + " and  comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_state.SelectedValue + "'   " + grade + "   ";
            //        }

            //        sql = "SELECT '' as 'txt_zone','' as 'zone',CASE WHEN invoice_flag != 0 AND month <= 3 AND year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND month >= 4 AND year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE auto_invoice_no END AS 'bill_invoice_no', client_code, client, state_name,branch_type, unit_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, SUM(hra) as 'hra', SUM(bonus_gross) as 'bonus_gross', SUM(leave_gross) as 'leave_gross', SUM(gratuity_gross) as 'gratuity_gross', SUM(washing) as 'washing', SUM(travelling) as 'travelling', SUM(education) as 'education', SUM(allowances) as 'allowances', SUM(cca_billing) as 'cca_billing', SUM(other_allow) as 'other_allow', SUM(gross) as 'gross', SUM(bonus_after_gross) as 'bonus_after_gross', SUM(leave_after_gross) as 'leave_after_gross', SUM(gratuity_after_gross) as 'gratuity_after_gross', SUM(pf) as 'pf', SUM(esic) as 'esic', SUM(hrs_12_ot) as 'hrs_12_ot' , SUM(esic_ot) as 'esic_ot', SUM(lwf) as 'lwf', SUM(uniform) as 'uniform', SUM(relieving_charg) as 'relieving_charg', SUM(operational_cost) as 'operational_cost', SUM(tot_days_present) as 'tot_days_present',ifnull(sum(Amount),0) as 'Amount', SUM(Service_charge) as 'Service_charge', ifnull(SUM(CGST9),0) as 'CGST9', ifnull(SUM(IGST18),0) as 'IGST18', ifnull(SUM(SGST9),0) as 'SGST9', bill_service_charge , NH, hours, fromtodate,(amount * month_days/tot_days_present) as 'sub_total_c', max(ot_rate) as 'ot_rate', SUM(ot_hours) as 'ot_hours', SUM(ot_amount) as 'ot_amount', group_insurance_billing, bill_service_charge_amount, txt_zone, adminhead_name, ihms, location_type, unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) as 'total_emp_count', sum(no_of_duties) as 'no_of_duties', zone, TOT_WORKING_DAYS, GRADE_CODE, month_days FROM pay_billing_unit_rate_history_arrears where " + old_month_year + "" + where1 + " group by unit_code  order by pay_billing_unit_rate_history_arrears.state_name,pay_billing_unit_rate_history_arrears.unit_name";


            //    }
            //    else
            //    {
            //        sql = "SELECT '' as 'txt_zone','' as 'zone',CASE WHEN invoice_flag != 0 AND month <= 3 AND year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND  year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', client_code,case when client_code = 'BAGIC TM' then 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' else client end AS 'client',state_name,unit_name,unit_city,client_branch_code,emp_name,grade_desc,emp_basic_vda,hra,bonus_gross,leave_gross,gratuity_gross,washing,travelling,education,allowances,cca_billing,other_allow,gross,bonus_after_gross,leave_after_gross,gratuity_after_gross,ifnull( pf ,0) as 'pf',esic,hrs_12_ot,esic_ot,lwf,uniform,relieving_charg,operational_cost,tot_days_present,ifnull( amount ,0) as 'Amount', Service_charge as 'Service_charge',ifnull(CGST9,0) as 'CGST9',ifnull(IGST18,0) as 'IGST18',ifnull(SGST9,0) as 'SGST9',bill_service_charge,NH,hours,fromtodate,(amount * month_days/tot_days_present) as 'sub_total_c',ot_rate,ot_hours,ot_amount,group_insurance_billing,bill_service_charge_amount,bill_service_charge_amount,branch_type,month_days,gst_applicable,OPus_NO,unit_code from pay_billing_unit_rate_history_arrears where " + old_month_year + " " + where1 + multi + " " + order_by_clause1;
            //    }
            //}

            ////OT Finance Copy
            //else if (i == 9)
            //{
            //    string start_end_date = "AND (start_date = 0 AND end_date = 0) " + billing_type;
            //    if (ddl_start_date_common.SelectedValue != "0" && ddl_end_date_common.SelectedValue != "0")
            //    {
            //        start_end_date = "AND (start_date = " + ddl_start_date_common.SelectedValue + " AND end_date = " + ddl_end_date_common.SelectedValue + ") " + billing_type;
            //    }

            //    where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.unit_code='" + ddl_unitcode.SelectedValue + "' " + billing_bfl + "  and pay_billing_unit_rate_history.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' AND hdfc_type = 'ot_bill' " + grade + "     AND approve = 2  " + where_state + " " + start_end_date;
            //    if (ddl_state.SelectedValue == "ALL")
            //    {
            //        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_unit_rate_history.month='" + txt_date.Text.Substring(0, 2) + "'  " + billing_bfl + "  and pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' AND hdfc_type = 'ot_bill'   AND approve = 2 " + where_state + " " + start_end_date;
            //    }
            //    else if (ddl_unitcode.SelectedValue == "ALL")
            //    {
            //        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.state_name = '" + ddl_state.SelectedValue + "'  " + billing_bfl + "   and pay_billing_unit_rate_history.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' AND hdfc_type = 'ot_bill' " + grade + "   AND approve = 2  " + where_state + " " + start_end_date;
            //    }
            //    if (ddl_client.SelectedValue == "HDFC")
            //    {
            //        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.unit_code='" + ddl_unitcode.SelectedValue + "' and pay_billing_unit_rate_history.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' " + grade + " AND hdfc_type = 'ot_bill' AND approve = 2 " + start_end_date + "  group by pay_billing_unit_rate_history.unit_code,grade_desc  order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
            //        if (ddl_state.SelectedValue == "ALL")
            //        {
            //            where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_unit_rate_history.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' " + grade + " AND hdfc_type = 'ot_bill' AND approve = 2 " + start_end_date + " group by pay_billing_unit_rate_history.unit_code,grade_desc order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
            //        }
            //        else if (ddl_unitcode.SelectedValue == "ALL")
            //        {
            //            where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.state_name = '" + ddl_state.SelectedValue + "'  and pay_billing_unit_rate_history.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_date.Text.Substring(3) + "' " + grade + " AND hdfc_type = 'ot_bill'  AND approve = 2 " + start_end_date + " group by pay_billing_unit_rate_history.unit_code,grade_desc order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
            //        }

            //        sql = "SELECT  CASE WHEN pay_billing_unit_rate_history.invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', pay_billing_unit_rate_history.client_code, client, pay_billing_unit_rate_history.state_name, pay_billing_unit_rate_history.branch_type, pay_billing_unit_rate_history.unit_name, pay_billing_unit_rate_history.unit_city, pay_billing_unit_rate_history.client_branch_code, emp_name, grade_desc, SUM(tot_days_present) AS 'tot_days_present', SUM(Amount) AS 'Amount', SUM(Service_charge) AS 'Service_charge', SUM(CGST9) AS 'CGST9', SUM(IGST18) AS 'IGST18', SUM(SGST9) AS 'SGST9', bill_service_charge, hours, fromtodate, MAX(ot_rate) AS 'ot_rate', SUM(ot_hours) AS 'ot_hours', SUM(ot_amount) AS 'ot_amount', bill_service_charge_amount, pay_billing_unit_rate_history.txt_zone, pay_billing_unit_rate_history.adminhead_name, ihms, pay_billing_unit_rate_history.location_type, pay_billing_unit_rate_history.unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, pay_billing_unit_rate_history.branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) AS 'total_emp_count', SUM(no_of_duties) AS 'no_of_duties', pay_billing_unit_rate_history.zone, TOT_WORKING_DAYS, GRADE_CODE, month_days, material_area,(SELECT  field2 FROM pay_zone_master WHERE pay_zone_master.comp_code = pay_billing_unit_rate_history.comp_code AND pay_zone_master.CLIENT_CODE = pay_billing_unit_rate_history.CLIENT_CODE AND pay_zone_master.ZONE = pay_unit_master.txt_zone AND type = 'ZONE' AND field1 = 'admin') AS 'zonal_name' FROM pay_billing_unit_rate_history INNER JOIN pay_unit_master ON pay_billing_unit_rate_history.comp_code = pay_unit_master.comp_code AND pay_billing_unit_rate_history.unit_code = pay_unit_master.unit_code INNER JOIN pay_ot_upload ON pay_ot_upload.comp_code = pay_billing_unit_rate_history.comp_code AND pay_ot_upload.unit_code = pay_billing_unit_rate_history.unit_code AND pay_ot_upload.month = pay_billing_unit_rate_history.month AND pay_ot_upload.year = pay_billing_unit_rate_history.year " + where;
            //    }
            //    else
            //    {
            //        sql = "SELECT txt_zone,zone,CASE WHEN pay_billing_unit_rate_history.invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN pay_billing_unit_rate_history.invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_unit_rate_history.client_code,case when pay_billing_unit_rate_history.client_code = 'BAGIC TM' then 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' else client end AS 'client',state_name,unit_name,unit_city, if(pay_billing_unit_rate_history.client_code ='4' ,branch_cost_centre_code,client_branch_code) as 'client_branch_code',emp_name,grade_desc,emp_basic_vda,hra,bonus_gross,leave_gross,gratuity_gross,washing,travelling,education,allowances,cca_billing,other_allow,gross,bonus_after_gross,leave_after_gross,gratuity_after_gross,pf,esic,hrs_12_ot,esic_ot,lwf,uniform,relieving_charg,operational_cost,tot_days_present,amount as 'Amount',Service_charge,CGST9,IGST18,SGST9,bill_service_charge,NH,hours,fromtodate,(amount * month_days/tot_days_present) as 'sub_total_c',round(ot_rate,2) as 'ot_rate',ot_hours,round(ot_amount,2) as 'ot_amount',group_insurance_billing,bill_service_charge_amount,bill_service_charge_amount,branch_type,month_days,gst_applicable,OPus_NO,pay_billing_unit_rate_history.unit_code,yearly_bonus,yearly_gratuity from pay_billing_unit_rate_history INNER JOIN pay_ot_upload ON pay_ot_upload.comp_code = pay_billing_unit_rate_history.comp_code AND pay_ot_upload.unit_code = pay_billing_unit_rate_history.unit_code AND pay_ot_upload.month = pay_billing_unit_rate_history.month AND pay_ot_upload.year = pay_billing_unit_rate_history.year " + where + " " + order_by_clause;
            //    }

            //}

            ////R&M finance copy
            //else if (i == 11)
            //{

            //    //string start_end_date = "AND (start_date = 0 AND end_date = 0) ";
            //    //if (ddl_start_date_common.SelectedValue != "0" && ddl_end_date_common.SelectedValue != "0")
            //    //{
            //    //    start_end_date = "AND (start_date = " + ddl_start_date_common.SelectedValue + " AND end_date = " + ddl_end_date_common.SelectedValue + ") ";
            //    //}

            //    //where = "where pay_billing_r_m.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_r_m.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_r_m.unit_code='" + ddl_unitcode.SelectedValue + "' and pay_billing_r_m.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_r_m.year = '" + txt_date.Text.Substring(3) + "' and pay_billing_r_m.invoice_slot = '" + ddl_invoice_slot.SelectedValue + "' and (approve_flag =1 || approve_flag =2)" + start_end_date;
            //    //if (ddl_state.SelectedValue == "ALL")
            //    //{
            //    //    where = "where pay_billing_r_m.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_r_m.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_r_m.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_r_m.year = '" + txt_date.Text.Substring(3) + "' and pay_billing_r_m.invoice_slot = '" + ddl_invoice_slot.SelectedValue + "' and (approve_flag =1 || approve_flag =2) " + start_end_date;
            //    //}
            //    //else if (ddl_unitcode.SelectedValue == "ALL")
            //    //{
            //    //    where = "where pay_billing_r_m.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_r_m.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_r_m.state_name = '" + ddl_state.SelectedValue + "'  and pay_billing_r_m.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_r_m.year = '" + txt_date.Text.Substring(3) + "' and pay_billing_r_m.invoice_slot = '" + ddl_invoice_slot.SelectedValue + "' and (approve_flag =1 || approve_flag =2) " + start_end_date;
            //    //}

            //    //sql = "SELECT  txt_zone, zone, auto_invoice_no AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_r_m.client_code, CASE WHEN pay_billing_r_m.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE pay_billing_r_m.client END AS 'client', pay_billing_r_m.state_name, pay_billing_r_m.unit_name, pay_billing_r_m.unit_city, pay_billing_r_m.client_branch_code, pay_billing_r_m.emp_name, help_req_number, pay_billing_r_m.amount AS 'Amount', Service_charge, CGST9, IGST18, SGST9, bill_service_charge, fromtodate, bill_service_charge_amount, branch_type, gst_applicable, pay_billing_r_m.unit_code, utr_number  FROM pay_billing_r_m INNER JOIN pay_r_and_m_service ON pay_r_and_m_service.comp_code = pay_billing_r_m.comp_code AND pay_r_and_m_service.client_code = pay_billing_r_m.client_code AND pay_r_and_m_service.unit_code = pay_billing_r_m.unit_code AND pay_r_and_m_service.month = pay_billing_r_m.month AND pay_r_and_m_service.year = pay_billing_r_m.year AND pay_r_and_m_service.EMP_CODE = pay_billing_r_m.EMP_CODE left outer JOIN pay_pro_r_m ON pay_pro_r_m.comp_code = pay_billing_r_m.comp_code AND pay_pro_r_m.client_code = pay_billing_r_m.client_code AND pay_pro_r_m.unit_code = pay_billing_r_m.unit_code AND pay_pro_r_m.month = pay_billing_r_m.month AND pay_pro_r_m.year = pay_billing_r_m.year AND pay_pro_r_m.EMP_CODE = pay_billing_r_m.EMP_CODE " + where + " group by pay_billing_r_m.id " + R_M_order_by_clause + "";

            //}
            ////Administrative Expense finance copy
            //else if (i == 12)
            //{

            //    string start_end_date = "AND (start_date = 0 AND end_date = 0) ";
            //    if (ddl_start_date_common.SelectedValue != "0" && ddl_end_date_common.SelectedValue != "0")
            //    {
            //        start_end_date = "AND (start_date = " + ddl_start_date_common.SelectedValue + " AND end_date = " + ddl_end_date_common.SelectedValue + ") ";
            //    }

            //    where = "where pay_billing_admini_expense.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_admini_expense.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_admini_expense.unit_code='" + ddl_unitcode.SelectedValue + "' and pay_billing_admini_expense.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_admini_expense.year = '" + txt_date.Text.Substring(3) + "' AND (approve_flag = 1 || approve_flag = 2) " + start_end_date;
            //    if (ddl_state.SelectedValue == "ALL")
            //    {
            //        where = "where pay_billing_admini_expense.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_admini_expense.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_admini_expense.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_admini_expense.year = '" + txt_date.Text.Substring(3) + "' AND (approve_flag = 1 || approve_flag = 2)  " + start_end_date;
            //    }
            //    else if (ddl_unitcode.SelectedValue == "ALL")
            //    {
            //        where = "where pay_billing_admini_expense.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_admini_expense.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_admini_expense.state_name = '" + ddl_state.SelectedValue + "'  and pay_billing_admini_expense.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_admini_expense.year = '" + txt_date.Text.Substring(3) + "' AND (approve_flag = 1 || approve_flag = 2) " + start_end_date;
            //    }

            //    sql = "SELECT  txt_zone, zone, auto_invoice_no AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_admini_expense.client_code, CASE WHEN pay_billing_admini_expense.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE pay_billing_admini_expense.client END AS 'client', pay_billing_admini_expense.state_name, pay_billing_admini_expense.unit_name, pay_billing_admini_expense.unit_city, pay_billing_admini_expense.client_branch_code, pay_billing_admini_expense.emp_name, pay_billing_admini_expense.amount AS 'Amount', Service_charge, CGST9, IGST18, SGST9, bill_service_charge, fromtodate, bill_service_charge_amount, branch_type, gst_applicable, pay_billing_admini_expense.unit_code, pay_billing_admini_expense.days, utr_number FROM pay_billing_admini_expense INNER JOIN pay_administrative_expense ON pay_administrative_expense.comp_code = pay_billing_admini_expense.comp_code AND pay_administrative_expense.client_code = pay_billing_admini_expense.client_code AND pay_administrative_expense.unit_code = pay_billing_admini_expense.unit_code AND pay_administrative_expense.month = pay_billing_admini_expense.month AND pay_administrative_expense.year = pay_billing_admini_expense.year AND pay_administrative_expense.party_name = pay_billing_admini_expense.emp_name LEFT OUTER JOIN pay_pro_admini_expense ON pay_pro_admini_expense.comp_code = pay_billing_admini_expense.comp_code AND pay_pro_admini_expense.client_code = pay_billing_admini_expense.client_code AND pay_pro_admini_expense.unit_code = pay_billing_admini_expense.unit_code AND pay_pro_admini_expense.month = pay_billing_admini_expense.month AND pay_pro_admini_expense.year = pay_billing_admini_expense.year AND pay_pro_admini_expense.emp_code = pay_billing_admini_expense.emp_code  " + where + " group by pay_administrative_expense.id " + R_M_order_by_clause + "";

            //}

            // Shiftwise finance copy
            //else if (i == 14)
            //{

            //    string start_end_date = "AND (start_date = 0 AND end_date = 0) ";
            //    if (ddl_start_date_common.SelectedValue != "0" && ddl_end_date_common.SelectedValue != "0")
            //    {
            //        start_end_date = "AND (start_date = " + ddl_start_date_common.SelectedValue + " AND end_date = " + ddl_end_date_common.SelectedValue + ") ";
            //    }

            //    where = "where pay_billing_shiftwise.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.unit_code='" + ddl_unitcode.SelectedValue + "' and pay_billing_shiftwise.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_shiftwise.year = '" + txt_date.Text.Substring(3) + "' " + start_end_date;
            //    if (ddl_state.SelectedValue == "ALL")
            //    {
            //        where = "where pay_billing_shiftwise.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_shiftwise.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_shiftwise.year = '" + txt_date.Text.Substring(3) + "'   " + start_end_date;
            //    }
            //    else if (ddl_unitcode.SelectedValue == "ALL")
            //    {
            //        where = "where pay_billing_shiftwise.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.state_name = '" + ddl_state.SelectedValue + "'  and pay_billing_shiftwise.month='" + txt_date.Text.Substring(0, 2) + "' and pay_billing_shiftwise.year = '" + txt_date.Text.Substring(3) + "'  " + start_end_date;
            //    }

            //    sql = "SELECT  txt_zone, zone, auto_invoice_no AS 'bill_invoice_no', CASE WHEN pay_billing_shiftwise.invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_shiftwise.client_code, CASE WHEN pay_billing_shiftwise.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE pay_billing_shiftwise.client END AS 'client', pay_billing_shiftwise.state_name, pay_billing_shiftwise.unit_name, pay_billing_shiftwise.unit_city, pay_billing_shiftwise.client_branch_code, pay_billing_shiftwise.emp_name,shiftwise_rate ,pay_billing_shiftwise.amount AS 'Amount', Service_charge, CGST9, IGST18, SGST9, bill_service_charge, fromtodate, bill_service_charge_amount, branch_type, gst_applicable, pay_billing_shiftwise.unit_code, pay_billing_shiftwise.shift_days  FROM pay_billing_shiftwise INNER JOIN pay_shift_details ON pay_shift_details.comp_code = pay_billing_shiftwise.comp_code AND pay_shift_details.client_code = pay_billing_shiftwise.client_code AND pay_shift_details.unit_code = pay_billing_shiftwise.unit_code AND pay_shift_details.month = pay_billing_shiftwise.month AND pay_shift_details.year = pay_billing_shiftwise.year AND pay_shift_details.EMP_CODE = pay_billing_shiftwise.EMP_CODE  " + where + " group by pay_shift_details.EMP_CODE " + R_M_order_by_clause + "";

            //}
            #endregion


            DataSet ds = new DataSet();

            MySqlDataAdapter dscmd = new MySqlDataAdapter(sql, d.con);

            dscmd.SelectCommand.CommandTimeout = 200;

            dscmd.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (type_cl == 0)
                {
                    Response.Clear();

                    //if (i == 2)
                    //{
                    Response.AddHeader("content-disposition", "attachment;filename=FINANCE_COPY_" + ddl_billtype_financecopy.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}

                    #region
                    //else if (i == 3)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=ATTENDANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //else if (i == 4)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=SUPPORT_FORMAT_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //if (i == 5)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=STATE_WISE_RATE_BREAKUP_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //if (i == 6)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=ARREARS_RATE_BREAKUP_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //else if (i == 7)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=ARREARS_FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //else if (i == 8)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=ARREARS_ATTENDANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //else if (i == 9)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //else if (i == 10)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=OT_RATE_BREAKUP_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //else if (i == 11)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=R&M_FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //else if (i == 12)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=ADMINISTRATIVE_EXPENSE_FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //else if (i == 13)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=OT_SHEET_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //else if (i == 14)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=SHIFTWISE_FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    #endregion

                }
                if (ddl_client.SelectedValue == "RCPL" && i == 1) { invoice = ""; }
                if (ddl_client.SelectedValue == "ALL")
                {
                    start_date_common = "1";
                }


                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                //Repeater1.HeaderTemplate = new MyTemplate_finance(ListItemType.Header, ds, i, invoice, bill_date, start_date_common, "", month_days, type_cl, ddl_state.SelectedValue);
                //Repeater1.ItemTemplate = new MyTemplate_finance(ListItemType.Item, ds, i, invoice, bill_date, start_date_common, "", month_days, type_cl, ddl_state.SelectedValue);
                // Repeater1.FooterTemplate = new MyTemplate_finance(ListItemType.Footer, null, i, invoice, bill_date, start_date_common, "", month_days, type_cl, ddl_state.SelectedValue);

                Repeater1.HeaderTemplate = new MyTemplate_finc(ListItemType.Header, ds, i);
                Repeater1.ItemTemplate = new MyTemplate_finc(ListItemType.Item, ds, i);
                Repeater1.HeaderTemplate = new MyTemplate_finc(ListItemType.Header, ds, i);



                Repeater1.DataBind();

                System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                Repeater1.RenderControl(htmlWrite);
                //Akshay 23-04-2019
                //if (ddl_client.SelectedValue == "RCPL" && i == 2) { stringWrite = update_grp_companies(stringWrite, ds); }
                //if (type_cl == 1)
                //{
                //    return stringWrite;
                //}

                string style = @"<style> .textmode { } </style>";
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
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
            if (type_cl == 1)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder("", 1);
                return new System.IO.StringWriter(sb);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }

    }

    private StringWriter update_grp_companies(StringWriter stringwrite, DataSet ds)
    {

        string grp_comp = "", where = "", tr = "", td = "", branch_name = "", cell = "";
        double ctc = 0, percentage = 0;
        int row_count = ds.Tables[0].Rows.Count;

        if (ds.Tables[0].Rows[row_count - 1]["branch_type"].ToString() == "0" || ds.Tables[0].Rows[row_count - 1]["branch_type"].ToString() == "")
        {
            //cell = "Y" + (row_count + 4) + "";
            cell = "AA" + (row_count + 3) + "";
        }
        else
        { //cell = "Z" + (row_count + 4) + "";
            cell = "AB" + (row_count + 3) + "";
        }
        where = "comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'  and unit_code = '" + ddl_unitcode.SelectedValue + "' and month = '" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "'";
        if (ddl_state.SelectedValue == "ALL")
        {
            where = "comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and month = '" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "'";
        }
        else if (ddl_unitcode.SelectedValue == "ALL")
        {
            where = "comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'  and unit_code in (Select unit_code from pay_unit_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_state.SelectedValue + "') and month = '" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "'";
        }


        MySqlCommand cmd = new MySqlCommand("Select state_per from pay_billing_unit_rate_history where client_code = '" + ddl_client.SelectedValue + "' and comp_code = '" + Session["COMP_CODE"].ToString() + "' and unit_code = '" + ddl_unitcode.SelectedValue + "' and month = '" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' AND Emp_code IS NULL group by state_per", d_cg.con);
        d_cg.con.Open();
        MySqlDataReader dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            string invoice_no = d.getsinglestring("select auto_invoice_no from pay_billing_unit_rate_history where " + where + " and month = '" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' and state_per = '" + dr.GetValue(0).ToString() + "' and invoice_flag != 0 ");
            d1.con1.Open();

            MySqlCommand cmd_cg = new MySqlCommand("Select unit_code,state_per,Companyname_gst_no,gst_address,bill_amount from pay_billing_unit_rate_history where " + where + "  and state_per= '" + dr.GetValue(0).ToString() + "' AND Emp_code IS NULL group by state_per", d1.con1);
            MySqlDataReader dr_cg = cmd_cg.ExecuteReader();
            while (dr_cg.Read())
            {
                percentage = percentage + double.Parse(dr_cg.GetValue(4).ToString());
                td = td + "<td>" + dr_cg.GetValue(4).ToString() + "</td><td>= ROUND(" + cell + " * " + dr_cg.GetValue(4).ToString() + ",2)%</td>";
            }
            dr_cg.Dispose();
            d1.con1.Close();

            tr = tr + "<tr><th colspan=2>'" + invoice_no + "</th><th colspan=2>" + dr.GetValue(0).ToString() + "</th>" + td + "</tr>";
            td = "";
        }
        tr = tr + "<tr><th colspan=4>Total</th><td>=Round(" + percentage + ",2)</td><td>=Round(" + cell + ",2)</td></tr>";
        d_cg.con.Close();



        var ValuetoReturn = (from Rows in ds.Tables[0].AsEnumerable() orderby Rows["unit_code"] select Rows["unit_name"]).Distinct().ToList();

        for (int i = 0; i < ValuetoReturn.Count; i++)
        {
            branch_name = branch_name + "<th>PERCENTAGE</th><th>AMOUNT</th>";
        }

        grp_comp = "<table BORDER=1><tr><th colspan=2>INVOICE NO</th><th colspan=2>COMPANY NAME</th>" + branch_name + "</tr>" + tr + "</table>";

        stringwrite.WriteLine("<br/><br/>");
        stringwrite.WriteLine(grp_comp);

        return stringwrite;
    }



    //protected void ddl_invoice_type_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (ddl_invoice_type.SelectedValue == "1")
    //    {
    //        ddl_designation.Items.Clear();
    //        desigpanel.Visible = false;
    //    }
    //    else if (ddl_invoice_type.SelectedValue == "2")
    //    {
    //        if (txt_date.Text != "")
    //        {
    //            ddl_designation.Items.Clear();
    //            desigpanel.Visible = true; int i = 0; string temp = "";
    //            if (ddl_state.SelectedValue == "ALL")
    //            {
    //                temp = d1.getsinglestring("select group_concat(distinct(designation)) from pay_billing_unit_rate where client_code='" + ddl_client.SelectedValue + "'  and year='" + txt_date.Text.Substring(3) + "'and month='" + txt_date.Text.Substring(0, 2) + "' and unit_code in (select unit_code from pay_unit_master where comp_code='" + Session["COMP_CODE"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "')");
    //            }
    //            else if (ddl_unitcode.SelectedValue == "ALL")
    //            {
    //                temp = d1.getsinglestring("select group_concat(distinct(designation)) from pay_billing_unit_rate where client_code='" + ddl_client.SelectedValue + "'  and year='" + txt_date.Text.Substring(3) + "'and month='" + txt_date.Text.Substring(0, 2) + "' and unit_code in (select unit_code from pay_unit_master where comp_code='" + Session["COMP_CODE"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_state.SelectedValue + "')");
    //            }
    //            else
    //            {
    //                temp = d1.getsinglestring("select group_concat(distinct(designation)) from pay_billing_unit_rate where client_code='" + ddl_client.SelectedValue + "'  and year='" + txt_date.Text.Substring(3) + "'and month='" + txt_date.Text.Substring(0, 2) + "' and unit_code = '" + ddl_unitcode.SelectedValue + "'");
    //            }
    //            var designationlist = temp.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
    //            foreach (string designation in designationlist)
    //            {
    //                ddl_designation.Items.Insert(i++, designation);
    //            }
    //        }
    //        else
    //        { ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select Month and try again.');", true); }
    //    }
    //}



    public class MyTemplate3 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        string counter, invoice, bill_date, start_date_common, header1, state_name_ddl;
        string report_type;
        static int ctr;
        int i, type_cl, month_days;



        public MyTemplate3(ListItemType type, DataSet ds, string counter, string report_type)
        {
            this.type = type;
            this.ds = ds;
            ctr = 0;
            this.counter = counter;
            this.report_type = report_type;

        }
        public MyTemplate3(ListItemType type, DataSet ds, int i, string invoice, string bill_date, string start_date_common, string header1, int month_days, int type_cl, string state_name_ddl)
        {
            this.type = type;
            this.ds = ds;
            this.i = i;
            this.invoice = invoice;
            this.bill_date = bill_date;
            this.start_date_common = start_date_common;
            this.header1 = header1;
            this.month_days = month_days;
            this.type_cl = type_cl;
            this.state_name_ddl = state_name_ddl;
            ctr = 0;

        }

        public void InstantiateIn(Control container)
        {


            switch (type)
            {
                case ListItemType.Header:
                    if (report_type == "1")
                    {
                        if (counter != "9")
                        {
                            lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=18>Reject Bill Report</th></tr><tr><th>SR NO.</th><th>Comapany Name</th><th>Client Name</th><th>State Name</th><th>Invoice No</th><th>Invoice Date</th><th>Employee Count</th><th>grand total</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Totalgst</th><th>Total_CTC</th></tr> ");
                            //lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=18>Salary Summary Report</th></tr><tr><th>SR NO.</th><th>Comapany Name</th><th>Client Name</th><th>State Name</th><th>Invoice No</th><th>Invoice Date</th><th>GST No</th><th>SAC Code</th><th>Employee Count</th><th>Month</th><th>Year</th><th>Billing Amount</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Total Billing Amount</th><th>Payment Amount</th><th>Type</th></tr> ");
                        }
                        else if (counter == "9")
                        {
                            lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=16>Salary Summary Report</th></tr><tr><th>SR NO.</th><th>Vendor Id</th><th>Purchase Invoice No</th><th>Vendor Invoice No</th><th>Month Year</th><th>Gross Amount</th><th>IGST</th><th>CGST</th><th>SGST</th><th>Total Invoice Amount</th><th>Payment Amount</th><th>Bank Holder Name</th><th>Bank Account No</th><th>IFSC Code</th><th>CRN No</th><th>Batch No</th></tr> ");
                        }
                    }

                    else if (report_type == "4")
                    {
                        if (counter != "9")
                        {
                            lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=18>Reject Bill Report</th></tr><tr><th>SR NO.</th><th>Comapany Name</th><th>Client Name</th><th>State Name</th><th>Invoice No</th><th>Invoice Date</th><th>Employee Count</th><th>grand total</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Totalgst</th><th>Total_CTC</th></tr> ");
                        }
                        else if (counter == "9")
                        {
                            lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=16>Reject Bill Report</th></tr><tr><th>SR NO.</th><th>Vendor Id</th><th>Purchase Invoice No</th><th>Vendor Invoice No</th><th>Month Year</th><th>Gross Amount</th><th>IGST</th><th>CGST</th><th>SGST</th><th>Total Invoice Amount</th><th>Payment Amount</th><th>Bank Holder Name</th><th>Bank Account No</th><th>IFSC Code</th><th>CRN No</th><th>Batch No</th></tr> ");
                        }
                    }

                    else if (report_type == "2")
                    {
                        if (counter == "1" || counter == "5")
                        {
                            lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=51>Employeewise Salary Report</th></tr><tr><th>SR NO.</th><th>Client Name</th><th>State Name</th><th>Branch Name</th><th>Employee Name</th><th>Designation</th><th>Actual Basic VDA</th><th>Emplyee Basic VDA</th><th>HRA</th><th>Bonus</th><th>Leave</th><th>Washing</th><th>Travelling</th><th>Education</th><th>Allowance</th><th>CCA</th><th>Other Allowance</th><th>Gratuity</th><th>OT</th><th>Gross</th><th>OT Rate</th><th>OT Hours</th><th>OT Amount</th><th>PF</th><th>ESIC</th><th>LWF</th><th>Uniform</th><th>PT</th><th>Bonus</th><th>Leave</th><th>Gratuity</th><th>Comman Allowance</th><th>ESIC Allowance</th><th>Conveyance</th><th>Absent Attendance</th><th>Emplyee Advance</th><th>Reliver Advance</th><th>Unit Deduction</th><th>Fine</th><th>Present Days</th><th>Payment</th><th>Bank Holder Name</th><th>Bank Account No</th><th>IFSC Code</th><th>Salary Status</th><th>Invoice No</th><th>Batch No</th><th>Paypro No</th><th>Month</th><th>Year</th><th>PAID DATE</th></tr> ");
                        }
                        else if (counter == "2" || counter == "3" || counter == "4")
                        {
                            lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=16>Employeewise Salary Report</th></tr><tr><th>SR NO.</th><th>Client Name</th><th>State Name</th><th>Branch Name</th><th>Employee Name</th><th>Designation</th><th>Conveyance Amount</th><th>Emplyee Deduction</th><th>Payment</th><th>Bank Holder Name</th><th>Bank Account No</th><th>IFSC Code</th><th>Invoice No</th><th>Paypro No</th><th>Month</th><th>Year</th></tr> ");

                        }
                        else if (counter == "6" || counter == "7")
                        {
                            lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=13>Employeewise Salary Report</th></tr><tr><th>SR NO.</th><th>Client Name</th><th>State Name</th><th>Branch Name</th><th>Party Name</th><th>Payment</th><th>Bank Holder Name</th><th>Bank Account No</th><th>IFSC Code</th><th>Invoice No</th><th>Paypro No</th><th>Month</th><th>Year</th></tr> ");

                        }
                        else if (counter == "8")
                        {
                            lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=16>Employeewise Salary Report</th></tr><tr><th>SR NO.</th><th>Client Name</th><th>State Name</th><th>Branch Name</th><th>Employee Name</th><th>Designation</th><th>Rate</th><th>Shift Count</th><th>Payment</th><th>Bank Holder Name</th><th>Bank Account No</th><th>IFSC Code</th><th>Invoice No</th><th>Paypro No</th><th>Month</th><th>Year</th></tr> ");

                        }
                    }
                    break;
                case ListItemType.Item:
                    if (report_type == "1")
                    {
                        if (counter != "9")
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["company"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["State_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Invoice_no"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["Invoice Date"] + "</td><td>" +   ds.Tables[0].Rows[ctr]["emp_count"] + "</td><td>" +  ds.Tables[0].Rows[ctr]["grandTotal"] + "</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["igst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Totalgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Total_CTC"] + "</td></tr>");
                        }
                        else if (counter == "9")
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["vendor_id"] + "</td><td>" + ds.Tables[0].Rows[ctr]["purch_invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["vendor_invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["month_year"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gross_amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["igst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["cgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["sgst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["total_invoice_value"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Payment"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Bank_holder_name"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["BANK_EMP_NO"] + "</td><td>" + ds.Tables[0].Rows[ctr]["IFSC_CODE"] + "</td><td>" + ds.Tables[0].Rows[ctr]["pay_pro_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["paypro_batch_id"] + "</td></tr>");
                        }
                    }
                    else if (report_type == "2")
                    {
                        if (counter == "1" || counter == "5")
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr][0].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][1].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][2].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][3].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][4].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][5].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][6].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][7].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][8].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][9].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][10].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][11].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][12].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][13].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][14].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][15].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][16].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][17].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][18].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][19].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][20].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][21].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][22].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][23].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][24].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][25].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][26].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][27].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][28].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][29].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][30].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][31].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][32].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][33].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][34].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][35].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][36].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][37].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][38].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][39].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][40].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr][41].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][42].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][43].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][44].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr][45].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][46].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][47].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][48].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][49].ToString() + "</td></tr>");
                        }
                        else if (counter == "2" || counter == "3" || counter == "4")
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr][0].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][1].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][2].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][3].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][4].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][5].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][6].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][7].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][8].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr][9].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][10].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][11].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][12].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][13].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][14].ToString() + "</td></tr>");

                        }
                        else if (counter == "6" || counter == "7")
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr][0].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][1].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][2].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][3].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][4].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][5].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr][6].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][7].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][8].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr][9].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][10].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][11].ToString() + "</td></td></tr>");

                        }
                        else if (counter == "8")
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr][0].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][1].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][2].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][3].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][4].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][5].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][6].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][7].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][8].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr][9].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][10].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][11].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][12].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][13].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr][14].ToString() + "</td></tr>");

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
    protected void btn_get_mis_Click(object sender, EventArgs e)
    {
        hidtab.Value = "4";
        try
        {
            int i = 0;
            string From_month = "";
            string To_month = "";
            string query = "";
            string where = "";

            if (ddl_client.SelectedValue != "ALL")
            {
                where = " and pay_report_gst.client_code = '" + ddl_client.SelectedValue + "' ";
            }

            if (mis_from_month.Text.Substring(3) != mis_to_month.Text.Substring(3))
            {
                int month = int.Parse(mis_from_month.Text.Substring(0, 2));
                int month1 = int.Parse(mis_to_month.Text.Substring(0, 2));
                for (int j = month; j <= 12; j++)
                {
                    From_month = From_month + j + ",";

                }
                From_month = From_month.Substring(0, From_month.Length - 1);
                for (int j = 1; j <= month1; j++)
                {
                    To_month = To_month + j + ",";

                }
                To_month = To_month.Substring(0, To_month.Length - 1);
            }
            else
            {
                int month = int.Parse(mis_from_month.Text.Substring(0, 2));
                int month1 = int.Parse(mis_to_month.Text.Substring(0, 2));
                for (int j = month; j <= month1; j++)
                {
                    From_month = From_month + j + ",";

                }
                From_month = From_month.Substring(0, From_month.Length - 1);
            }

            query = "SELECT  IF(pay_report_gst.comp_code = 'C01', 'INTEGRATED', 'IHMS') AS 'company', CONCAT(pay_report_gst.month, '/', pay_report_gst.year) AS 'month_year', pay_report_gst.Client_name, pay_report_gst.type, pay_report_gst.Invoice_no, DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Invoice Date', pay_report_gst.State_name, pay_report_gst.gst_no, pay_report_gst.emp_count, SUM(tot_days_present) AS 'working_days', ROUND(SUM(pay_billing_unit_rate_history.amount), 2) AS 'gross_amount', ROUND(SUM(Service_charge), 2) AS 'Service_charge', ROUND((pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst), 2) AS 'total_gst', ROUND((pay_report_gst.amount + pay_report_gst.cgst + pay_report_gst.sgst + pay_report_gst.igst), 2) AS 'total_billing_amount', ROUND(SUM(pay_pro_master.payment - (fine + EMP_ADVANCE_PAYMENT + emp_advance + reliver_advances + absent_attendance_total)), 2) AS 'take_home', ROUND((SUM(pay_billing_unit_rate_history.pf) + SUM(pay_pro_master.sal_pf)), 2) AS 'pf_payable', ROUND((SUM(pay_billing_unit_rate_history.esic) + SUM(pay_pro_master.sal_esic)), 2) AS 'esic_payable', SUM(pay_billing_unit_rate_history.group_insurance_billing + pay_billing_unit_rate_history.medical_insurance_amount) AS 'group_medical_insurance', IF(pay_pro_master.payment_status = 1, ROUND(SUM(pay_pro_master.payment - (fine + EMP_ADVANCE_PAYMENT + emp_advance + reliver_advances + absent_attendance_total)),2),0) AS 'paid_payment', IF(pay_pro_master.payment_status = 0, ROUND(SUM(pay_pro_master.payment - (fine + EMP_ADVANCE_PAYMENT + emp_advance + reliver_advances + absent_attendance_total)),2),0) AS 'unpaid_payment' FROM pay_report_gst INNER JOIN pay_billing_unit_rate_history ON pay_report_gst.Invoice_no = pay_billing_unit_rate_history.auto_invoice_no INNER JOIN pay_pro_master ON pay_billing_unit_rate_history.month = pay_pro_master.month AND pay_billing_unit_rate_history.year = pay_pro_master.year AND pay_billing_unit_rate_history.emp_code = pay_pro_master.emp_code AND pay_billing_unit_rate_history.start_date = pay_pro_master.start_date WHERE pay_report_gst.comp_code = '" + Session["comp_code"].ToString() + "' AND type = 'manpower' " + where;

            if (To_month != "")
            {
                query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + mis_from_month.Text.Substring(3) + "' GROUP BY invoice_no union " + query + " and pay_report_gst.month IN (" + To_month + ") and pay_report_gst.year='" + mis_to_month.Text.Substring(3) + "' GROUP BY invoice_no ";
            }
            else
            {
                query = "" + query + " AND pay_report_gst.month IN (" + From_month + ") and pay_report_gst.year='" + mis_from_month.Text.Substring(3) + "' GROUP BY invoice_no";
            }


            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();

            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);


            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;

                Response.AddHeader("content-disposition", "attachment;filename=MIS_Report.xls");

                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate4(ListItemType.Header, ds);
                Repeater1.ItemTemplate = new MyTemplate4(ListItemType.Item, ds);
                Repeater1.FooterTemplate = new MyTemplate4(ListItemType.Footer, null);
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

    public class MyTemplate4 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
      
        static int ctr;



        public MyTemplate4(ListItemType type, DataSet ds)
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

                    lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=42>" + ds.Tables[0].Rows[ctr]["company"] + "</th></tr><tr><th style=background-color:skyblue; colspan=18>Basic Details</th><th style=background-color:aqua; colspan=10>Billing Details</th><th style=background-color:orange; colspan=10>At Difference</th><th style=background-color:pink; colspan=4>Payment Status</th></tr><tr><th>SR NO.</th><th>Month/Year</th><th>Name of Client</th><th>Billing Type</th><th>Invoice No</th><th>Invoice Date</th><th>State Name</th><th>Client GST No</th><th>Approved Strenght</th><th>Approved Working Days</th><th>Gross Billed</th><th>Service Charges</th><th>Total GST</th><th>TotaL Billing</th><th>Take Home</th><th>PF(EMPLOYER + EMPLOYEE) Payable</th><th>ESIC(EMPLOYER + EMPLOYEE) Payable</th><th>GPA/HI</th><th>Actual Strenght</th><th>Actual Working Days</th><th>Gross Billed</th><th>Service Charges</th><th>Total GST</th><th>TotaL Billing</th><th>Take Home</th><th>PF(EMPLOYER + EMPLOYEE) Payable</th><th>ESIC(EMPLOYER + EMPLOYEE) Payable</th><th>GPA/HI</th><th>Actual Strenght</th><th>Actual Working Days</th><th>Gross Billed</th><th>Service Charges</th><th>Total GST</th><th>TotaL Billing</th><th>Take Home</th><th>PF(EMPLOYER + EMPLOYEE) Payable</th><th>ESIC(EMPLOYER + EMPLOYEE) Payable</th><th>GPA/HI</th><th>Paid Salary</th><th>Unpaid Salary</th><th>Total Amount</th><th>Difference payment</th></tr> ");

                    break;
                case ListItemType.Item:

                    lc = new LiteralControl("<tr><td>" + (ctr + 2) + "</td><td>" + ds.Tables[0].Rows[ctr]["month_year"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Invoice Date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["State_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst_no"] + "</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>" + ds.Tables[0].Rows[ctr]["emp_count"] + "</td><td>" + ds.Tables[0].Rows[ctr]["working_days"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gross_amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Service_charge"] + "</td><td>" + ds.Tables[0].Rows[ctr]["total_gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["total_billing_amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["take_home"] + "</td><td>" + ds.Tables[0].Rows[ctr]["pf_payable"] + "</td><td>" + ds.Tables[0].Rows[ctr]["esic_payable"] + "</td><td>" + ds.Tables[0].Rows[ctr]["group_medical_insurance"] + "</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>0</td><td>" + ds.Tables[0].Rows[ctr]["paid_payment"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unpaid_payment"] + "</td><td>" + (Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["paid_payment"].ToString()), 2) + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["unpaid_payment"].ToString()), 2)) + "</td><td>" + (Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["take_home"].ToString()), 2) - Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["paid_payment"].ToString()), 2)) + "</td></tr>");

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

    ///soft copy send status
   




    ///soft copy send status

    protected void btn_softcopy_Click(object sender, EventArgs e)
    {
        soft_copy_mail_send();
    }


    protected void soft_copy_mail_send()
    {
         hidtab.Value = "4";
         try
         {
             int i = 0;
           
        
             int month = int.Parse(txt_date.Text.Substring(0, 2));
             int year = int.Parse(txt_date.Text.Substring(3));
             int prev_month = month - 1;
             if (prev_month == 0)
             {
                 prev_month = 12;
                 year = year - 1;
             }
             string prev_month1 = prev_month.ToString();
             string year1 = year.ToString();
             string query = "";
             string where = "";
             if (ddl_client.SelectedValue != "ALL")
             {
                 where += " and a.Client_name = '" + ddl_client.SelectedItem + "' ";
             }
             if (ddl_state.SelectedValue != "ALL")
             {
                 where += " and a.state_name = '" + ddl_state.SelectedItem + "' ";
             }
             if (ddl_unitcode.SelectedValue != "ALL")
             {
                 where += " and a.unit_code = '" + ddl_unitcode.SelectedValue + "' ";
             }

            //Sachin 
            string billing_month = txt_date.Text;

           // query = " SELECT a.comp_code, a.client_code, a.client_name, CONCAT(c.start_date_billing,'  TO  ',c.end_date_billing) AS Billing_Period, a.month,a.year,a.type,COUNT(invoice_no) AS Final_bill, IFNULL (ROUND(SUM(amount + cgst + sgst + igst), 2),0)  AS Final_bill_total, SUM(IF(softcopy_sendmail_status = '1', 1, 0)) AS Sent_bill,SUM(IF(a.softcopy_sendmail_status = '0',1,0)) AS pending_bill,(SELECT  COUNT(invoice_no) AS Final_bill FROM pay_report_gst b WHERE b.client_code = a.client_code AND b.comp_code = a.comp_code AND b.month = (IF(a.month = 1, 12, a.month))AND b.year = (IF(a.month = 1, a.year - 1, a.year))AND a.type = b.type GROUP BY b.client_code , b.Type) AS previous_months_bill, (SELECT IFNULL (ROUND(SUM(amount + cgst + sgst + igst), 2),0)  AS Final_bill_total FROM pay_report_gst b WHERE b.client_code = a.client_code AND b.month = a.month - 1 AND b.year = (IF(a.month = 1, b.year - 1, a.year))AND a.type = b.type) AS previous_months_total, '" + billing_month + "' as billing_month  FROM pay_report_gst a INNER JOIN pay_client_master c ON a.client_code = c.CLIENT_CODE and a.CLIENT_NAME = c.CLIENT_NAME WHERE a.comp_code = '" + Session["comp_code"].ToString() + "' and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' " + where + "  group by a.client_code,Type ORDER BY a.CLIENT_NAME asc  ";


            query = " SELECT a.comp_code, a.client_code, a.client_name, CONCAT(c.start_date_billing,'  TO  ',c.end_date_billing) AS Billing_Period, a.month,a.year,UCASE(a.type) as type,COUNT(invoice_no) AS Final_bill, IFNULL (ROUND(SUM(amount + cgst + sgst + igst), 2),0)  AS Final_bill_total, SUM(IF(softcopy_sendmail_status = '1', 1, 0)) AS Sent_bill,SUM(IF(a.softcopy_sendmail_status = '0',1,0)) AS pending_bill, (SELECT  COUNT(invoice_no) AS Final_bill FROM pay_report_gst b WHERE b.client_code = a.client_code AND b.comp_code = a.comp_code AND b.month = (IF(a.month = 1, 12, a.month-1)) AND b.year = (IF(a.month = 1, a.year - 1, a.year))AND   b.type=a.type ) AS previous_months_bill, (SELECT IFNULL (ROUND(SUM(amount + cgst + sgst + igst), 2),0)  AS Final_bill_total FROM pay_report_gst b WHERE b.client_code = a.client_code AND b.month = (IF(a.month = 1, 12, a.month-1)) AND b.year = (IF(a.month = 1, a.year - 1, a.year)) AND b.type=a.type ) AS previous_months_total, '" + billing_month + "' as billing_month  FROM pay_report_gst a INNER JOIN pay_client_master c ON a.client_code = c.CLIENT_CODE and a.CLIENT_NAME = c.CLIENT_NAME WHERE a.comp_code = '" + Session["comp_code"].ToString() + "' and month='" + txt_date.Text.Substring(0, 2) + "' and year = '" + txt_date.Text.Substring(3) + "' " + where + "  group by a.client_code,Type ORDER BY a.CLIENT_NAME asc  ";

             //    query = "SELECT comp_code,    client_code,    client_name,type  ,  count(invoice_no) as Total_bill,    SUM(IF(softcopy_sendmail_status = '1', 1, 0)) AS Sent_bill,    SUM(IF(softcopy_sendmail_status = '0', 1, 0)) AS pending_bill FROM    pay_report_gst WHERE    comp_code = '" + Session["comp_code"].ToString() + "'  and client_code = '" + ddl_client.SelectedValue + "' and month='" + txt_date.Text.Substring(2)+ "' and year = '"+txt_date.Text.Substring(3)+"'  group by client_code,Type order by client_code";

                 MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
                 DataSet ds = new DataSet();

                 dscmd.SelectCommand.CommandTimeout = 200;
                 dscmd.Fill(ds);


                 if (ds.Tables[0].Rows.Count > 0)
                 {
                     Response.Clear();
                     Response.Buffer = true;

                     Response.AddHeader("content-disposition", "attachment;filename=Softcopy_Mail_send_Report.xls");

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
                    string heading = "Soft Copy Mail Send Report For The Month Of  " + Convert.ToDateTime(ds.Tables[0].Rows[ctr]["billing_month"]).ToString("MMM-yyyy").ToUpper() + "";

                    // lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=11>Soft Copy Mail Send Report</th></tr><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>MONTH</th><th>YEAR</th><th>TYPE</th><th>PREVIOUS MONTH BILL</th><th>PREVIOUS MONTH BILLING TOTAL</th><th>CURRENT MONTH BILL</th><th>CURRENT MONTH BILLING TOTAL</th><th>SENT BILL</th><th>NOT SENT BILL</th></tr>");

                    lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=12>" + heading + "</th></tr><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>BILLING DATE</th><th>MONTH</th><th>YEAR</th><th>TYPE</th><th>PREVIOUS MONTH BILL</th><th>PREVIOUS MONTH BILLING TOTAL</th><th>CURRENT MONTH BILL</th><th>CURRENT MONTH BILLING TOTAL</th><th>SENT BILL</th><th>NOT SENT BILL</th></tr>");


                    break;
                case ListItemType.Item:
                    //lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["month"] + "</td><td>" + ds.Tables[0].Rows[ctr]["year"] + "</td><td>" + ds.Tables[0].Rows[ctr]["type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["previous_months_bill"] + "</td><td>" + ds.Tables[0].Rows[ctr]["previous_months_total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Final_bill"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Final_bill_total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Sent_bill"] + "</td><td>" + ds.Tables[0].Rows[ctr]["pending_bill"] + "</td></tr>");
                    lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Billing_Period"] + "</td><td>" + ds.Tables[0].Rows[ctr]["month"] + "</td><td>" + ds.Tables[0].Rows[ctr]["year"] + "</td><td>" + ds.Tables[0].Rows[ctr]["type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["previous_months_bill"] + "</td><td>" + ds.Tables[0].Rows[ctr]["previous_months_total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Final_bill"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Final_bill_total"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Sent_bill"] + "</td><td>" + ds.Tables[0].Rows[ctr]["pending_bill"] + "</td></tr>");
                    if (ds.Tables[0].Rows.Count == ctr + 1)
                    {
                        lc.Text = lc.Text + "<tr><b><td align=center colspan = 6>Total</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(H3:H" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(I3:I" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td></tr>";
                        //lc.Text = lc.Text + "<tr><b><td align=center colspan = 5>Total</td><td>=ROUND(SUM(F3:F" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(H3:H" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(I3:I" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td></tr>";
                    }
                    ctr++;

                   // lc.Text = lc.Text + "<tr><b><td align=center colspan=3>Total</td><td>=SUM(J" + (ctc1 + set_start_row) + ":D" + (ctr + i3) + ")</td><td>=SUM(K" + (ctc1 + set_start_row) + ":E" + (ctr + i3) + ")</td><td>=SUM(L" + (ctc1 + set_start_row) + ":F" + (ctr + i3) + ")</td></tr>";

                    break;

                case ListItemType.Footer:
                    lc = new LiteralControl("</table>");
                    ctr = 0;
                    break;
            }
            container.Controls.Add(lc);
        }


    }

    protected void btn_attendancere_Click(object sender, EventArgs e)
    {
        hidtab.Value = "9";
        gv_attndshow();


    }

    protected void gv_attndshow()
    {
        hidtab.Value = "9";
        try
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            gv_attendance.DataSource = null;
            gv_attendance.DataBind();

            string where1 = "";

            if (ddl_client.SelectedValue != "ALL")
            {
                where1 += "AND C.CLIENT_CODE  = '" + ddl_client.SelectedValue + "' ";
            }

            d.con.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT C.CLIENT_NAME, C.CLIENT_CODE, A.MONTH, A.YEAR, COUNT(DISTINCT (A.UNIT_CODE)) AS 'Current_month_attendance', (SELECT COUNT(DISTINCT (A1.UNIT_CODE)) FROM pay_attendance_muster A1 LEFT JOIN pay_unit_master B1 ON A1.UNIT_CODE = B1.UNIT_CODE AND A1.COMP_CODE = B1.COMP_CODE LEFT JOIN pay_client_master C1 ON B1.CLIENT_CODE = C1.CLIENT_CODE WHERE A1.MONTH = IF(A.MONTH = 1, 12, A.MONTH - 1)AND A1.year = IF(A.MONTH = 1, A.year - 1, A.year) AND A1.COMP_CODE = A.COMP_CODE AND B1.CLIENT_CODE = B.CLIENT_CODE AND B1.branch_status=B.branch_status) AS 'pre_month_cnt' FROM pay_attendance_muster A INNER JOIN pay_unit_master B ON A.UNIT_CODE = B.UNIT_CODE AND A.COMP_CODE = B.COMP_CODE INNER JOIN pay_client_master C ON B.CLIENT_CODE = C.CLIENT_CODE WHERE A.COMP_CODE = '" + Session["comp_code"].ToString() + "' AND A.MONTH = '" + txt_date.Text.Substring(0, 2) + "' AND A.YEAR = '" + txt_date.Text.Substring(3) + "' " + where1 + "  AND B.branch_status='0' GROUP BY C.CLIENT_CODE, A.YEAR, A.MONTH", d.con);
            MySqlDataAdapter dt_item = new MySqlDataAdapter(cmd);
            cmd.CommandTimeout = 200;
            dt_item.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                gv_attendance.DataSource = dt;
                gv_attendance.DataBind();
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }

    }
    protected void gv_attendance_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
    protected void gv_attendance_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_attendance.UseAccessibleHeader = false;
            gv_attendance.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//
    }
    protected void btn_view_Click(object sender, EventArgs e)
    {
        hidtab.Value = "9";
        try
        {

            System.Data.DataTable dt = new System.Data.DataTable();
            gv_state_attendance.DataSource = null;
            gv_state_attendance.DataBind();

            LinkButton btn = (LinkButton)sender;
            GridViewRow row = btn.NamingContainer as GridViewRow;
            string client_code = gv_attendance.DataKeys[row.RowIndex].Values[0].ToString();
            System.Diagnostics.Debug.WriteLine(client_code);

            d.con.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT C.CLIENT_NAME, B.STATE_NAME, A.MONTH, A.YEAR,(SELECT COUNT(DISTINCT (A2.UNIT_CODE)) FROM pay_attendance_muster A2 LEFT JOIN pay_unit_master B2 ON A2.UNIT_CODE = B2.UNIT_CODE AND A2.COMP_CODE = B2.COMP_CODE LEFT JOIN pay_client_master C2 ON B2.CLIENT_CODE = C2.CLIENT_CODE where A2.MONTH = A.MONTH AND A2.YEAR = A.YEAR AND A2.COMP_CODE = A.COMP_CODE AND B2.CLIENT_CODE = B.CLIENT_CODE AND A2.flag=4 AND B2.STATE_NAME = B.STATE_NAME AND B2.branch_status=B.branch_status) AS 'approve_by_admin_manager', (SELECT COUNT(DISTINCT (A3.UNIT_CODE)) FROM pay_attendance_muster A3 LEFT JOIN pay_unit_master B3 ON A3.UNIT_CODE = B3.UNIT_CODE AND A3.COMP_CODE = B3.COMP_CODE LEFT JOIN pay_client_master C3 ON B3.CLIENT_CODE = C3.CLIENT_CODE where A3.MONTH = A.MONTH AND A3.YEAR = A.YEAR AND A3.COMP_CODE = A.COMP_CODE AND B3.CLIENT_CODE = B.CLIENT_CODE AND A3.flag=2 AND B3.STATE_NAME = B.STATE_NAME AND B3.branch_status=B.branch_status) AS 'approve_by_finance', (SELECT COUNT(DISTINCT (A4.UNIT_CODE)) FROM pay_attendance_muster A4 LEFT JOIN pay_unit_master B4 ON A4.UNIT_CODE = B4.UNIT_CODE AND A4.COMP_CODE = B4.COMP_CODE LEFT JOIN pay_client_master C4 ON B4.CLIENT_CODE = C4.CLIENT_CODE where A4.MONTH = A.MONTH AND A4.YEAR = A.YEAR AND A4.COMP_CODE = A.COMP_CODE AND B4.CLIENT_CODE = B.CLIENT_CODE AND A4.flag=1 AND B4.STATE_NAME = B.STATE_NAME AND B4.branch_status=B.branch_status) AS 'Approve_by_admin', (SELECT COUNT(DISTINCT (A5.UNIT_CODE)) FROM pay_attendance_muster A5 LEFT JOIN pay_unit_master B5 ON A5.UNIT_CODE = B5.UNIT_CODE AND A5.COMP_CODE = B5.COMP_CODE  LEFT JOIN pay_client_master C5 ON B5.CLIENT_CODE = C5.CLIENT_CODE  WHERE A5.MONTH = A.MONTH AND A5.YEAR = A.YEAR  AND A5.COMP_CODE = A.COMP_CODE  AND B5.CLIENT_CODE = B.CLIENT_CODE AND( A5.flag=0 or A5.flag=3) AND B5.STATE_NAME = B.STATE_NAME AND B5.branch_status=B.branch_status) AS 'Pending', COUNT(DISTINCT (A.UNIT_CODE)) AS 'Current_month_attendance', (SELECT COUNT(DISTINCT (A1.UNIT_CODE)) FROM pay_attendance_muster A1 LEFT JOIN pay_unit_master B1 ON A1.UNIT_CODE = B1.UNIT_CODE AND A1.COMP_CODE = B1.COMP_CODE LEFT JOIN pay_client_master C1 ON B1.CLIENT_CODE = C1.CLIENT_CODE WHERE A1.MONTH = IF(A.MONTH = 1, 12, A.MONTH - 1)AND A1.year = IF(A.MONTH = 1, A.year - 1, A.year)AND A1.COMP_CODE = A.COMP_CODE AND B1.CLIENT_CODE = B.CLIENT_CODE AND B1.STATE_NAME = B.STATE_NAME  AND B1.branch_status=B.branch_status) AS 'pre_month_cnt'FROM pay_attendance_muster A INNER JOIN pay_unit_master B ON A.UNIT_CODE = B.UNIT_CODE AND A.COMP_CODE = B.COMP_CODE INNER JOIN pay_client_master C ON B.CLIENT_CODE = C.CLIENT_CODE WHERE A.COMP_CODE = '" + Session["comp_code"].ToString() + "' AND A.MONTH = '" + txt_date.Text.Substring(0, 2) + "' AND A.YEAR = '" + txt_date.Text.Substring(3) + "' AND B.CLIENT_CODE = '" + client_code + "' AND B.branch_status='0' GROUP BY B.STATE_NAME , A.YEAR , A.MONTH", d.con);
            // MySqlCommand cmd = new MySqlCommand("select c.client_name,pt.state,a.MONTH,a.YEAR,If(pt.flag='4','approve by admin manager','Approve by admin')as status,if(pt.flag='2','approve by finance','approve by admin manager')as status1,count(distinct(pt.status='approve by admin'))as 'approve by admin',count(distinct(pt.status='Approve By Admin Manager'))as 'Approve by admin Manager',count(distinct(pt.status='Approve By Finance'))as 'Approve by Finance',COUNT(distinct(a.UNIT_CODE))as Current_month_attendance ,(select COUNT(distinct(a.UNIT_CODE))from pay_unit_master  u1 INNER JOIN pay_client_master c1 ON u1.CLIENT_CODE = c1.CLIENT_CODE and u1.comp_code = c1.comp_code INNER JOIN pay_attendance_muster a1 ON u1.unit_code = a1.unit_code  and u1.COMP_CODE = a1.COMP_CODE where  a1.MONTH=if(a.MONTH=1,12,a.MONTH-1) and a1.year=IF(a.MONTH=1,a.year-1,a.year) and  c1.CLIENT_CODE=c.CLIENT_CODE group by  c1.CLIENT_CODE) as pre_month_cnt from pay_unit_master  u INNER JOIN pay_client_master c ON u.CLIENT_CODE = c.CLIENT_CODE and u.comp_code = c.comp_code INNER JOIN pay_attendance_muster a ON u.unit_code = a.unit_code  and u.COMP_CODE = a.COMP_CODE INNER JOIN pay_files_timesheet pt ON  u.CLIENT_CODE = pt.CLIENT_CODE and u.unit_code = pt.unit_code  and u.COMP_CODE = pt.COMP_CODE  WHERE    u.comp_code = '" + Session["comp_code"].ToString() + "' and a.month='" + txt_date.Text.Substring(0, 2) + "' and a.year = '" + txt_date.Text.Substring(3) + "' AND c.CLIENT_CODE='" + client_code + "' group by u.unit_code ", d.con);
            MySqlDataAdapter dt_item = new MySqlDataAdapter(cmd);
            cmd.CommandTimeout = 200;
            dt_item.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                gv_state_attendance.DataSource = dt;
                gv_state_attendance.DataBind();
                //btn_view.Visible = true;
            }
        }

        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
    }
    protected void gv_state_attendance_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_state_attendance.UseAccessibleHeader = false;
            gv_state_attendance.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }

    //MD Approval Attendace Gridview
    protected void gv_md_approval()
    {
        hidtab.Value = "9";
        try
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            gv_Md_Approve.DataSource = null;
            gv_Md_Approve.DataBind();
            string where1 = "", Month_Year = "", Approve_Type = "";

            if (ddl_client.SelectedValue != "ALL")
            {
                where1 = "and client_code='" + ddl_client.SelectedValue + "'";
            }
            if (ddl_Select.SelectedValue == "2")
            {
                Month_Year = "and month='" + txt_report_month_year.Text.Substring(0, 2) + "' and year = '" + txt_report_month_year.Text.Substring(3) + "'";
            }
            if (ddl_Select.SelectedValue == "3")
            {
                Month_Year = "and year='" + txt_report_month_year.Text.Substring(3) + "'";
            }
            if (ddl_approved_or_not.SelectedValue == "2")
            {
                Approve_Type = "where NOTApprove = 0";
            }
            if (ddl_approved_or_not.SelectedValue == "3")
            {
                Approve_Type = "where NOTApprove > 0";
            }
            d.con.Open();
            MySqlCommand cmd = new MySqlCommand("select * from (select client_name,month,year,type,count(invoice_no) as 'Total_Invoice', SUM(IF((flag_invoice=2),1,0)) as Approve,SUM(IF((flag_invoice!=2),1,0)) as NOTApprove from pay_report_gst where comp_code='" + Session["comp_code"] + "' " + Month_Year + " " + where1 + " and type not in('Credit','Debit')  and type is not null group by client_code,type,month,year ) as t1 " + Approve_Type + " ORDER BY year,month,client_name", d.con);
            MySqlDataAdapter dt_item = new MySqlDataAdapter(cmd);
            cmd.CommandTimeout = 200;
            dt_item.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                gv_Md_Approve.DataSource = dt;
                gv_Md_Approve.DataBind();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Matching Records Found.');", true);
            }
        }
        catch { }
    }
    protected void btn_mdApprove_Click(object sender, EventArgs e)
    {
        gv_md_approval();
    }
    //END
    protected void gv_Md_Approve_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
    protected void gv_Md_Approve_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_Md_Approve.UseAccessibleHeader = false;
            gv_Md_Approve.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//
    }
    protected void btn_excle_report_Click(object sender, EventArgs e)
    {
        hidtab.Value = "10";
        int i = 4;
        string where = "";
        try
        {
            int From = int.Parse(txt_out_from_month.Text.Substring(0, 2));
            int TO = int.Parse(txt_out_to_month.Text.Substring(0, 2));

            if (txt_out_from_month.Text.Substring(3) == txt_out_to_month.Text.Substring(3) && From <= TO)
            {
                string sql = null;

                if (ddl_client.SelectedItem.Text == "ALL")
                {
                    where = "and month between '" + txt_out_from_month.Text.Substring(0, 2) + "' and '" + txt_out_to_month.Text.Substring(0, 2) + "' and year= '" + txt_out_from_month.Text.Substring(3) + "'";
                }
                else
                {
                    where = "and month between '" + txt_out_from_month.Text.Substring(0, 2) + "' and '" + txt_out_to_month.Text.Substring(0, 2) + "' and year= '" + txt_out_from_month.Text.Substring(3) + "' and client_name='" + ddl_client.SelectedValue + "'";
                }

                if (i == 4)
                {
                    sql = "select client_code, client_name, month, year,SUM(IF(month = 1, out_bal, 0)) AS pt_1,    SUM(IF(month = 2, out_bal, 0)) AS pt_2,    SUM(IF(month = 3, out_bal, 0)) AS pt_3,    SUM(IF(month = 4, out_bal, 0)) AS pt_4,    SUM(IF(month = 5, out_bal, 0)) AS pt_5,    SUM(IF(month = 6, out_bal, 0)) AS pt_6,    SUM(IF(month = 7, out_bal, 0)) AS pt_7,    SUM(IF(month = 8, out_bal, 0)) AS pt_8,    SUM(IF(month = 9, out_bal, 0)) AS pt_9,    SUM(IF(month = 10, out_bal, 0)) AS pt_10,    SUM(IF(month = 11, out_bal, 0)) AS pt_11,    SUM(IF(month = 12, out_bal, 0)) AS pt_12  from (select client_code, client_name, month, year, billing_amt, tds_amt, received,round((billing_amt-tds_amt-received),2) as outstanding,Round((If((billing_amt - tds_amt - received) <10,0,billing_amt-tds_amt-received)),2) as out_bal from (select client_code,client_name,month,year, SUM(amount+cgst+sgst+igst) as billing_amt,sum(tds_amount) as tds_amt,SUM( received_amt + received_amt2 + received_amt3) as received from pay_report_gst where comp_code='" + Session["COMP_CODE"].ToString() + "' " + where + " AND type!='credit' AND (billing_amt - total_received - tds_amount) > 100 group by client_code,month ) as t1 ) t2  group by t2.client_code;";
                }

                d.con.Open();
                MySqlDataAdapter dscmd = new MySqlDataAdapter(sql, d.con);
                DataSet ds = new DataSet();
                dscmd.SelectCommand.CommandTimeout = 800;

                dscmd.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    Response.Clear();
                    Response.Buffer = true;

                    if (i == 4)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=Outstanding_report.xls");
                    }

                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";
                    Repeater Repeater1 = new Repeater();
                    Repeater1.DataSource = ds;
                    if (i == 4)
                    {
                        int from_month = From;
                        int to_month = TO;

                        Repeater1.HeaderTemplate = new MyTemplate_pt(ListItemType.Header, ds, i, from_month, to_month);
                        Repeater1.ItemTemplate = new MyTemplate_pt(ListItemType.Item, ds, i, from_month, to_month);
                        Repeater1.FooterTemplate = new MyTemplate_pt(ListItemType.Footer, null, i, from_month, to_month);
                    }

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
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select To date month is equal to or greater than from date month and year is equal ');", true);
                return;
            }
        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }
    }
    public class MyTemplate_pt : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;
        int i;
        int k = 3;
        int srno;
        int cnt = 2;
        double sum_total_pt = 0;
        string month_year = "";
        string header = "";
        string bodystr = "";
        int total_emp_count;
        int from_month;
        int to_month;
        private ListItemType listItemType;


        public MyTemplate_pt(ListItemType type, DataSet ds, int i, int from_month, int to_month)
        {
            this.type = type;
            this.ds = ds;
            this.i = i;
            ctr = 0;
            this.from_month = from_month;
            this.to_month = to_month;

        }

        public MyTemplate_pt(ListItemType listItemType, DataSet ds, int i)
        {
            // TODO: Complete member initialization
            this.listItemType = listItemType;
            this.ds = ds;
            this.i = i;
        }
        public void InstantiateIn(Control container)
        {
            switch (type)
            {
                case ListItemType.Header:
                    if (i == 4)
                    {
                        string html_head = "", start_month = "";

                        for (int j = from_month; j <= to_month; j++)
                        {
                            string Year = ds.Tables[0].Rows[0]["Year"].ToString();
                            //  start_month = j.ToString();
                            if (j == 1) { start_month = "January"; }
                            if (j == 2) { start_month = "February"; }
                            if (j == 3) { start_month = "March"; }
                            if (j == 4) { start_month = "April"; }
                            if (j == 5) { start_month = "May"; }
                            if (j == 6) { start_month = "June"; }
                            if (j == 7) { start_month = "July"; }
                            if (j == 8) { start_month = "August"; }
                            if (j == 9) { start_month = "September"; }
                            if (j == 10) { start_month = "October"; }
                            if (j == 11) { start_month = "November"; }
                            if (j == 12) { start_month = "December"; }

                            html_head = html_head + "<th bgcolor=yellow colspan=1>" + start_month + " - " + Year + " </th>";
                            k = k + 1;
                        }

                        lc = new LiteralControl("<table  border=1 ><tr><th bgcolor=yellow colspan=" + k + " align=center>Outstanding Report</th></tr><tr><th>SR. NO.</th><th>CLIENT NAME</th>" + html_head + "<th bgcolor=yellow >TOTAL AMOUNT</th></tr>");
                    }

                    break;
                case ListItemType.Item:

                    if (i == 4)
                    {
                        string html_body = "";

                        double Pt_month = 0, total_pt = 0;

                        for (int j = from_month; j <= to_month; j++)
                        {

                            Pt_month = Convert.ToDouble(ds.Tables[0].Rows[ctr]["pt_" + j].ToString());

                            total_pt = total_pt + Pt_month;

                            html_body = html_body + "<td>" + Pt_month + "</td>";
                        }
                        if (total_pt > 0)
                        {
                            srno = srno + 1;

                            lc = new LiteralControl("<tr><td>" + (srno) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td>" + html_body + " <td>" + total_pt + "</td></tr>");

                            sum_total_pt = sum_total_pt + total_pt;
                        }

                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            for (int j = from_month; j <= to_month; j++)
                            {
                                cnt = cnt + 1;
                            }

                            lc.Text = lc.Text + "<tr><b><td align=center colspan = " + cnt + ">Total</td><td>" + sum_total_pt + "</td></tr>";
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
}