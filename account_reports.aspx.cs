using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Configuration;
using System.Data.OleDb;
using System.Globalization;
using System.Collections.Generic;
using System.Net.Mail;


public partial class account_reports : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();
    DAL d3 = new DAL();
    DAL d4 = new DAL();
    BillingSalary bs = new BillingSalary();
    string aa = "";
    CrystalDecisions.CrystalReports.Engine.ReportDocument crystalReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
    protected void Page_Load(object sender, EventArgs e)
    {

        if (d.getaccess(Session["ROLE"].ToString(), "Account", Session["COMP_CODE"].ToString()) == "I")
        {
            Response.Redirect("unauthorised_access.aspx");
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Account", Session["COMP_CODE"].ToString()) == "R")
        {

        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Account", Session["COMP_CODE"].ToString()) == "U")
        {

        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Account", Session["COMP_CODE"].ToString()) == "C")
        {

        }
        if (Session["comp_code"] == null || Session["comp_code"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }
        if (!IsPostBack)
        {
            client_code();
            // comp_data();
          //  txt_comp_name.Text = d.getsinglestring("Select Company_name from pay_company_master where comp_code= '" + Session["COMP_CODE"].ToString() + "' ");
           // seles();
            pnl_bank_details.Visible = false;
            btn_update_receipt.Visible = false;
            btn_row.Visible = false;
            client_name();
            payment_type_selection();
            btn_add_others.Visible = false;
            txt_description.Visible = false;
            Panel6.Visible = false;
            Panel_gv_pmt.Visible = false;
            head_transction.Visible = false;
            btn_update.Visible = false;
            panel_add_other.Visible = false;
            Panel_other_desc.Visible = false;
            panel_mode.Visible = false;
            //panel2.Visible = true;
            //   load_gv_payment("");
            // load_gv_debit_pmt_details("1");
            ddl_batch_no.Items.Insert(0, new ListItem("Select"));
            submit_btn.Visible = false;
            cheque.Visible = false;
            ddl_mode_transfer.SelectedValue = "Select";
            for_other.Visible = false;
            for_client.Visible = false;
            for_other1.Visible = false;
            desc.Visible = false;
            div_invoice_list.Visible = false;
           // account_link_details.Visible = false;
            txt_recived_am.Visible = false;
            pnl_utr_report.Visible = false;



        }

    }
    //vikas
    protected void client_code()
    {

        // insert();
        ddl_minibank_client.Items.Clear();
        try
        {
            System.Data.DataTable dt_item1 = new System.Data.DataTable();//reciept entry Client List       
            MySqlDataAdapter cmd_item1 = new MySqlDataAdapter("select pay_minibank_master.client_name,pay_minibank_master.client_code from pay_minibank_master where pay_minibank_master.comp_code = '" + Session["comp_code"] + "'  AND `receipt_approve` != '0'  GROUP BY pay_minibank_master.client_name   ", d.con);
            d.con.Close();
            d.con.Open();
            cmd_item1.Fill(dt_item1);
            if (dt_item1.Rows.Count > 0)
            {
                ddl_client.DataSource = dt_item1;
                ddl_client.DataTextField = dt_item1.Columns[0].ToString();
                ddl_client.DataValueField = dt_item1.Columns[0].ToString();// dt_item1.Columns[1].ToString();
                ddl_client.DataBind();
            }
        }
        catch { }


        System.Data.DataTable dt_item = new System.Data.DataTable();
       // MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', client_code from pay_client_master where comp_code='" + Session["comp_code"] + "' ORDER BY client_code", d.con);

        MySqlDataAdapter cmd_item = new MySqlDataAdapter("select CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', client_code  from pay_report_gst  where  comp_code='" + Session["comp_code"] + "' and client_code is not NULL and client_name is not NULL and client_code NOT like 'OM%'  group by client_name order by client_name", d.con);
        d.con.Close();
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                //ddl_client.DataSource = dt_item1;
                //ddl_client.DataTextField = dt_item1.Columns[0].ToString();
                //ddl_client.DataValueField = dt_item1.Columns[1].ToString();
                //ddl_client.DataBind();

                ddl_minibank_client.DataSource = dt_item;
                ddl_minibank_client.DataTextField = dt_item.Columns[0].ToString();
                ddl_minibank_client.DataValueField = dt_item.Columns[0].ToString(); //dt_item.Columns[1].ToString();
                ddl_minibank_client.DataBind();

                ddl_pmt_client.DataSource = dt_item;
                ddl_pmt_client.DataTextField = dt_item.Columns[0].ToString();
                ddl_pmt_client.DataValueField = dt_item.Columns[1].ToString();
                ddl_pmt_client.DataBind();

	        
				
                ddl_upload_lg_client.DataSource = dt_item;
                ddl_upload_lg_client.DataTextField = dt_item.Columns[0].ToString();
                ddl_upload_lg_client.DataValueField = dt_item.Columns[1].ToString();
                ddl_upload_lg_client.DataBind();
				
			    ddl_client_outstanding.DataSource = dt_item;
                ddl_client_outstanding.DataTextField = dt_item.Columns[0].ToString();
                ddl_client_outstanding.DataValueField = dt_item.Columns[0].ToString();
                //ddl_client_outstanding.DataValueField = dt_item.Columns[1].ToString();
                ddl_client_outstanding.DataBind();

            }
            dt_item.Dispose();
            // hide_controls();
            d.con.Close();
            ddl_client.Items.Insert(0, "Select");
            ddl_minibank_client.Items.Insert(0, "Select");
            ddl_pmt_client.Items.Insert(0, "Select");
            ddl_upload_lg_client.Items.Insert(0, "Select");
			   ddl_client_outstanding.Items.Insert(0, "Select");
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }

    }



    protected void bntclose_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }


    protected void gv_fullmonthot_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[3].Text == "1")
            {
                //Find the TextBox control.
                CheckBox txtName = (e.Row.FindControl("chk_client") as CheckBox);
                txtName.Checked = true;
                txtName.Enabled = false;
            }
        }
        //e.Row.Cells[7].Visible = false;
    }
    //vikas
    //private void load_grdview()
    //{
    //    gv_fullmonthot.Visible = false;
    //    d.con1.Open();
    //    try
    //    {
    //        MySqlDataAdapter MySqlDataAdapter1 = new MySqlDataAdapter("SELECT id,comp_code,client_code,unit_code,month,year,(SELECT emp_name FROM pay_employee_master WHERE uploaded_by = pay_employee_master.emp_code) AS uploaded_by, uploaded_date,description,concat('~/Attendance_Images/',file_name) as Value FROM pay_files_timesheet where comp_Code = '" + Session["COMP_CODE"].ToString() + "' ", d.con1);

    //        DataSet DS1 = new DataSet();
    //        MySqlDataAdapter1.Fill(DS1);
    //        grd_company_files.DataSource = null;
    //        grd_company_files.DataBind();
    //        grd_company_files.DataSource = DS1;
    //        grd_company_files.DataBind();
    //        txt_document1.Text = "";
    //        grd_company_files.Visible = true;
    //        d.con1.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        d.con1.Close();

    //    }

    //}

    //vikas
    protected void upload_Click(object sender, EventArgs e)
    {
        //upload_documents(document1_file);


    }
    //private void upload_documents(FileUpload document_file)
    //{

    //    if (document_file.HasFile)
    //    {
    //        string fileExt = System.IO.Path.GetExtension(document_file.FileName);
    //        if (fileExt == ".jpg" || fileExt == ".JPG" || fileExt == ".png" || fileExt == ".PNG" || fileExt == ".pdf" || fileExt == ".PDF" || fileExt == ".JPEG" || fileExt == ".jpeg")
    //        {
    //            string fileName = Path.GetFileName(document_file.PostedFile.FileName);
    //            document_file.PostedFile.SaveAs(Server.MapPath("~/Attendance_Images/") + fileName);

    //            string new_file_name = txt_month.Text.Replace("/", "_") + fileExt;

    //            File.Copy(Server.MapPath("~/Attendance_Images/") + fileName, Server.MapPath("~/Attendance_Images/") + new_file_name, true);
    //            File.Delete(Server.MapPath("~/Attendance_Images/") + fileName);
    //            d.operation("insert into pay_files_timesheet (comp_code, file_name, description, month, year, uploaded_by, uploaded_date) values ('" + Session["COMP_CODE"].ToString() + "','" + new_file_name + "','" + txt_document1.Text + "','" + int.Parse(txt_month.Text.Substring(0, 2)) + "','" + int.Parse(txt_month.Text.Substring(3)) + "','" + Session["LOGIN_ID"].ToString() + "',now())");
    //        }
    //        else
    //        {
    //            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please select only JPG, PNG and PDF Files !!!')", true);
    //        }

    //    }
    //    //load_grdview();
    //}

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
        e.Row.Cells[0].Visible = false;

        if (e.Row.Cells[1].Text == "1")
        {
            e.Row.Cells[1].Text = "JAN";
        }
        else if (e.Row.Cells[1].Text == "2")
        {
            e.Row.Cells[1].Text = "FEB";
        }
        else if (e.Row.Cells[1].Text == "3")
        {
            e.Row.Cells[1].Text = "MAR";
        }
        else if (e.Row.Cells[1].Text == "4")
        {
            e.Row.Cells[1].Text = "APR";
        }
        else if (e.Row.Cells[1].Text == "5")
        {
            e.Row.Cells[1].Text = "MAY";
        }
        else if (e.Row.Cells[1].Text == "6")
        {
            e.Row.Cells[1].Text = "JUN";
        }
        else if (e.Row.Cells[1].Text == "7")
        {
            e.Row.Cells[1].Text = "JUL";
        }
        else if (e.Row.Cells[1].Text == "8")
        {
            e.Row.Cells[1].Text = "AUG";
        }
        else if (e.Row.Cells[1].Text == "9")
        {
            e.Row.Cells[1].Text = "SEP";
        }
        else if (e.Row.Cells[1].Text == "10")
        {
            e.Row.Cells[1].Text = "OCT";
        }
        else if (e.Row.Cells[1].Text == "11")
        {
            e.Row.Cells[1].Text = "NOV";
        }
        else if (e.Row.Cells[1].Text == "12")
        {
            e.Row.Cells[1].Text = "DEC";
        }
    }
    //mahendra payment_history

    //DAL d = new DAL();
    // double rec_amount;
    // double balance_amount;
    double final_double_amount;
    protected int result = 0;
    protected double billing_amount = 0, recived_amt = 0;


    protected void btn_close_click(object sender, object e)
    {
        Response.Redirect("Home.aspx");
    }
    //Receipt details

    protected void btn_submit_Click(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        //receving_amount.Visible = false;
        //load_client_amount();
        try
        {
            process_operation();
            Check_setteled_amt();
        }
        catch    {  }

    }

    private void process_operation()
    {
       
        try
        {
            div_invoice_list.Visible = true;
            txt_recived_am.Text = "0";

            txt_total_invoice.Text = "";
            txt_deducted.Text = "";
            DataSet ds = new DataSet();
            gv_invoice_list.DataSource = null;
            gv_invoice_list.DataBind();

            //ds = d.select_data("SELECT payment_history.Invoice_No AS 'Invoice_no', CONCAT(payment_history.month, '/', payment_history.year) AS 'bill_month', ROUND(payment_history.billing_amt) as 'billing_amt',(ROUND(payment_history.billing_amt) - IFNULL(ROUND(SUM(pay_report_gst.received_amt + tds_amount)), 0)) AS 'Balanced Amount',payment_history.Id FROM payment_history  LEFT JOIN pay_report_gst ON payment_history.Invoice_No = pay_report_gst.Invoice_No WHERE payment_history.comp_code='" + Session["COMP_CODE"].ToString() + "' and  payment_history.client_code = '" + ddl_client.SelectedValue + "' AND payment_history.invoice_flag = 2 and payment_history.invoice_no  not in (SELECT Invoice_No FROM (SELECT payment_history.Invoice_No, CASE adjustment_sign != '' WHEN adjustment_sign = 1 THEN ROUND(IFNULL((payment_history.billing_amt - SUM(pay_report_gst.received_amt + tds_amount + adjustment_amt)), 0), 2) WHEN adjustment_sign = 2 THEN ROUND(IFNULL((payment_history.billing_amt - SUM(pay_report_gst.received_amt + tds_amount - adjustment_amt)), 0), 2) ELSE payment_history.billing_amt END AS 'Balanced_Amount' FROM payment_history LEFT JOIN pay_report_gst ON payment_history.Invoice_No = pay_report_gst.Invoice_No AND payment_history.client_code = pay_report_gst.client_code AND payment_history.month = pay_report_gst.month AND payment_history.year = pay_report_gst.year WHERE payment_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' and payment_history.client_code = '" + ddl_client.SelectedValue + "' AND payment_history.invoice_flag = 2 GROUP BY payment_history.Invoice_No) AS t1 WHERE t1.Balanced_Amount <= 0) ");
            //chaitali commented query for all type invoice
            //ds = d.select_data("SELECT Invoice_No,amount ,month, year,Balanced_Amount FROM((SELECT pay_report_gst.Invoice_No,pay_report_gst.amount,pay_report_gst.month, pay_report_gst.year,CASE adjustment_sign != '' WHEN adjustment_sign = 1 THEN ROUND(IFNULL((pay_report_gst.amount - SUM(pay_report_gst.received_amt + tds_amount + adjustment_amt)), 0), 2) WHEN adjustment_sign = 2 THEN ROUND(IFNULL((pay_report_gst.amount - SUM(pay_report_gst.received_amt + tds_amount - adjustment_amt)), 0), 2) ELSE pay_report_gst.amount END AS 'Balanced_Amount' FROM pay_report_gst  LEFT JOIN pay_report_gst ON pay_report_gst.Invoice_No = pay_report_gst.Invoice_No AND pay_report_gst.client_code = pay_report_gst.client_code AND pay_report_gst.month = pay_report_gst.month AND pay_report_gst.year = pay_report_gst.year INNER JOIN `pay_billing_unit_rate_history` ON `pay_report_gst`.`Invoice_No` = `pay_billing_unit_rate_history`.`Invoice_No` AND `pay_report_gst`.`client_code` = `pay_billing_unit_rate_history`.`client_code` WHERE pay_report_gst.comp_code='" + Session["comp_code"] + "' and pay_report_gst.client_code = '" + ddl_client.SelectedValue + "' AND invoice_flag = 2  GROUP BY pay_report_gst.Invoice_No) AS t1) WHERE t1.Balanced_Amount != 0 &&  `t1`.`Balanced_Amount` > 0");
            d1.con1.Open();


            string where_client_code = "";
            string client_code_rd = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_client.SelectedValue + "' limit 1");


            //if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
            //{
            //    where_client = " pay_minibank_master.client_code = '7'  ";
            //}
            if (client_code_rd == "ESFB" || client_code_rd == "EquitasRes")
            {
                where_client_code = "  pay_report_gst.client_code IN ('ESFB','EquitasRes') ";
            }
            else if (client_code_rd == "7")
            {
                where_client_code = "  pay_report_gst.client_code IN ('7') ";
            }
            else if (client_code_rd == "TAIL" || client_code_rd == "TAILTEMP")
            {
                where_client_code = "  pay_report_gst.client_code IN ('TAIL','TAILTEMP') ";
            }  
            else if (client_code_rd == "RLIC HK" || client_code_rd == "RCFL" || client_code_rd == "RNLIC RM" || client_code_rd == "RCPL")
            {
                where_client_code = " pay_report_gst.client_code IN ('RLIC HK','RCFL','RNLIC RM')  ";
            }
            else
            {
                where_client_code = "  pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "'  and pay_report_gst.client_code='" + client_code_rd + "' and  pay_report_gst.client_name = '" + ddl_client.SelectedValue + "' ";
            }




            //  ds = d.select_data("SELECT Invoice_No,ROUND(amount, 2) AS 'amount',month,year,ROUND(Balanced_Amount, 2) AS 'Balanced_Amount', type FROM((SELECT pay_report_gst . Invoice_No ,( amount +  cgst +  sgst +  igst ) AS 'amount',pay_report_gst . month ,pay_report_gst . year , CASE  pay_report_gst.adjustment_sign  = '0' WHEN  pay_report_gst.adjustment_sign  = 1  THEN ROUND(IFNULL((( amount +  cgst +  sgst +  igst )  - SUM( pay_report_gst . received_amt  +  pay_report_gst.tds_amount  +  pay_report_gst.adjustment_amt )), 0), 2) WHEN  pay_report_gst.adjustment_sign  = 2 THEN ROUND(IFNULL((( amount +  cgst +  sgst +  igst ) - SUM( pay_report_gst . received_amt  +  pay_report_gst.tds_amount  -  pay_report_gst.adjustment_amt )), 0), 2)   ELSE round((sum( amount +  cgst +  sgst +  igst ) - sum(pay_report_gst.received_amt + pay_report_gst.tds_amount)),2) END AS 'Balanced_Amount', type  FROM pay_report_gst  WHERE pay_report_gst . comp_code  = '" + Session["COMP_CODE"].ToString() + "' AND  pay_report_gst . client_code  = '" + ddl_client.SelectedValue + "' AND  pay_report_gst . flag_invoice  = 2 GROUP BY pay_report_gst . Invoice_No ) AS t1)WHERE t1 . Balanced_Amount  != 0 &&  t1 . Balanced_Amount  > 0.99 ");
            //ds = d.select_data("SELECT Invoice_No,ROUND(amount, 2) AS 'amount',month,year,ROUND(Balanced_Amount, 2) AS 'Balanced_Amount',ROUND(tds_amt, 2) as tds_amt, type FROM((SELECT pay_report_gst . Invoice_No ,( amount +  cgst +  sgst +  igst ) AS 'amount',pay_report_gst . month ,pay_report_gst . year ,  if (pay_report_gst . received_amt>0,0,(CASE WHEN  pay_report_gst . tds_amount  != '' THEN  pay_report_gst . tds_amount  = '0' WHEN  tds_applicable  = 1 AND  pay_client_master . tds_on  = 1 THEN ROUND(((( amount  + cgst + sgst+ igst ) *  tds_percentage ) / 100), 2)WHEN  tds_applicable  = 1 AND  pay_client_master . tds_on  = 2 THEN ROUND((((amount) *  tds_percentage ) / 100), 2)   ELSE 0 END))               AS 'tds_amt',   CASE  pay_report_gst.adjustment_sign  = '0' WHEN  pay_report_gst.adjustment_sign  = 1  THEN ROUND(IFNULL((( amount +  cgst +  sgst +  igst )  - SUM( pay_report_gst . received_amt  +  pay_report_gst.tds_amount  +  pay_report_gst.adjustment_amt )), 0), 2) WHEN  pay_report_gst.adjustment_sign  = 2 THEN ROUND(IFNULL((( amount +  cgst +  sgst +  igst ) - SUM( pay_report_gst . received_amt  +  pay_report_gst.tds_amount  -   pay_report_gst.adjustment_amt )), 0), 2)   ELSE round((sum( amount +  cgst +  sgst +  igst ) - sum(pay_report_gst.received_amt + pay_report_gst.tds_amount)),2) END AS 'Balanced_Amount',   type FROM pay_report_gst  INNER JOIN  pay_client_master  ON  pay_report_gst . comp_code  =    pay_client_master . comp_Code  AND  pay_report_gst . client_code  =  pay_client_master . client_code    WHERE pay_report_gst . comp_code  = '" + Session["COMP_CODE"].ToString() + "' AND  pay_report_gst . client_code  = '" + ddl_client.SelectedValue + "' AND  pay_report_gst . flag_invoice  = 2 GROUP BY pay_report_gst . Invoice_No ) AS t1)WHERE t1 . Balanced_Amount  != 0 &&  (t1 . Balanced_Amount) > 0.99");
            ds = d.select_data("SELECT Invoice_No,ROUND(amount, 2) AS 'amount',month,year,ROUND(tds_amt,2) as tds_amt,ROUND(Received_Amount,2) as Received_Amount,deduction_amt,ROUND((Balance_Amount), 2) AS 'Balance_Amount', type FROM (( SELECT pay_report_gst . Invoice_No ,ROUND( amount +  cgst +  sgst +  igst ,2) AS 'amount',pay_report_gst . month ,pay_report_gst . year ,   (CASE WHEN  (pay_report_gst . tds_amount  != '' OR  pay_report_gst . tds_amount  != '0') THEN  pay_report_gst . tds_amount  WHEN  tds_applicable  = 1 AND  pay_client_master . tds_on  = 1 THEN ROUND(((( amount  + cgst + sgst+ igst ) *  tds_percentage ) / 100), 2)WHEN  tds_applicable  = 1 AND  pay_client_master . tds_on  = 2 THEN ROUND((((amount) *  tds_percentage ) / 100), 2)   ELSE 0 END)  AS 'tds_amt',  ROUND((pay_report_gst.received_amt+pay_report_gst.received_amt2 + pay_report_gst.received_amt3),2) AS 'Received_Amount', (ROUND( amount +  cgst +  sgst +  igst ,2)-(CASE WHEN  (pay_report_gst . tds_amount  != '' OR pay_report_gst . tds_amount  != '0') THEN  pay_report_gst . tds_amount  WHEN  tds_applicable  = 1 AND  pay_client_master . tds_on  = 1 THEN ROUND(((( amount  + cgst + sgst+ igst ) *  tds_percentage ) / 100), 2)WHEN  tds_applicable  = 1 AND  pay_client_master . tds_on  = 2 THEN ROUND((((amount) *  tds_percentage ) / 100), 2)   ELSE 0 END)-ROUND((pay_report_gst.received_amt+pay_report_gst.received_amt2 + pay_report_gst.received_amt3),2)-ROUND(IFNULL(pay_report_gst.deduction_amt,0),2)) as Balance_Amount, ROUND(IFNULL(pay_report_gst.deduction_amt,0),2) as deduction_amt ,   type FROM pay_report_gst  INNER JOIN  pay_client_master  ON  pay_report_gst . comp_code  =    pay_client_master . comp_Code  AND  pay_report_gst . client_code  =  pay_client_master . client_code    WHERE " + where_client_code + " AND  pay_report_gst . flag_invoice  = 2 GROUP BY pay_report_gst . Invoice_No ) AS t1)  WHERE t1 . Balance_Amount > 0.99");
            if (ds.Tables[0].Rows.Count > 0)
            {
                gv_invoice_list.DataSource = ds;
                gv_invoice_list.DataBind();
                Panel6.Visible = true;
                // panel2.Visible = false;
              

            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('No Matching Records Found !!!')", true);
                gv_invoice_list.DataSource = null;
                gv_invoice_list.DataBind();
                btn_process.Visible = false;
            }
            ds.Dispose();
           
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            hidtab.Value = "1";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            Panel_gv_pmt.Visible = false;
        }
    }

    private void Check_setteled_amt()
    {
        try
        {
            string where_client = "";

            if (ddl_client.SelectedValue == "TATA STEEL LTD" || ddl_client.SelectedValue == "TATA STEELS PVT LTD")
            {
                where_client = " pay_minibank_master.client_code = '7'  ";
            }

            else if (ddl_client.SelectedValue == "EQUITAS SMALL FINANCE BANK" || ddl_client.SelectedValue == "Equitas Small Finance Bank Limited")
            {
                where_client = " pay_minibank_master.client_code IN ('ESFB','EquitasRes' ) ";
            }
            else if (ddl_client.SelectedValue == "TATA AIA Life Insurance Company Limited" || ddl_client.SelectedValue == "TATA AIA Life Insurance Company Limited Tem")
            {
                where_client = " pay_minibank_master.client_code  IN ('TAIL','TAILTEMP' ) ";
            }
            else if (ddl_client.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD." || ddl_client.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD. RM")
            {
                where_client = " pay_minibank_master.client_code IN ('RLIC HK','RNLIC RM' ) ";
            }
            else 
            {
                where_client = " pay_minibank_master.client_name = '" + ddl_client.SelectedValue + "'  ";
            }










          //  MySqlCommand cmd = new MySqlCommand("SELECT `pay_minibank_master`.`ID`, ROUND(IFNULL(SUM(`pay_report_gst`.`received_amt`), 0), 2) AS ' SETTLED_AMOUNT', ROUND(pay_minibank_master.`Amount` - (IFNULL(SUM(`pay_report_gst`.`received_amt`), 0)), 2) AS 'REMANING_AMOUNT',ROUND(pay_minibank_master.`Amount`,2) as utr_amt,pay_minibank_master.description as payment_type FROM `pay_minibank_master`  LEFT JOIN `pay_report_gst` ON `pay_report_gst`.`payment_id` = `pay_minibank_master`.`id` WHERE `pay_minibank_master`.`comp_code` = '" + Session["COMP_CODE"].ToString() + "' and pay_minibank_master.receive_date=str_to_date('" + txt_date.Text + "','%d-%m-%Y') and pay_minibank_master.client_name='" + ddl_client.SelectedValue + "' and pay_minibank_master.amount='" + ddl_client_resive_amt.SelectedItem + "'", d1.con1);
            MySqlCommand cmd = new MySqlCommand("SELECT `pay_minibank_master`.`ID`, ROUND(IFNULL(IFNULL( (select SUM(`pay_report_gst`.`received_amt`) from pay_report_gst where pay_report_gst.payment_id= pay_minibank_master.Id),0) + IFNULL( (select SUM(b.received_amt2) from pay_report_gst b where b.payment_id2= pay_minibank_master.Id),0) +   IFNULL((select SUM(c.received_amt3) from pay_report_gst c where   c.payment_id3= pay_minibank_master.Id),0) , 0), 2) AS ' SETTLED_AMOUNT',  ROUND(pay_minibank_master.`Amount` - (IFNULL(  IFNULL((select SUM(`pay_report_gst`.`received_amt`) from pay_report_gst where pay_report_gst.payment_id= pay_minibank_master.id),0)+    IFNULL((select SUM(b.received_amt2) from pay_report_gst b where b.payment_id2= pay_minibank_master.id),0) +    IFNULL((select SUM(c.received_amt3) from pay_report_gst c where c.payment_id3= pay_minibank_master.id),0), 0)), 2) AS 'REMANING_AMOUNT', ROUND(pay_minibank_master.`Amount`, 2) AS utr_amt, pay_minibank_master.description AS payment_type,pay_minibank_master.Utr_no FROM `pay_minibank_master` WHERE `pay_minibank_master`.`comp_code` = '" + Session["COMP_CODE"].ToString() + "' and pay_minibank_master.receive_date=str_to_date('" + txt_date.Text + "','%d-%m-%Y') and " + where_client + " and pay_minibank_master.id='" + ddl_client_resive_amt.SelectedValue + "'", d1.con1);
            d1.con1.Close();
            d1.con1.Open();
            MySqlDataReader dr = cmd.ExecuteReader();
            string setteled_amt1 = "0", setteled_amt2 = "0";
            double utr_amt = 0;
            if (dr.Read())
            {
                // txt_total_invoice.Text = dr.GetValue(1).ToString();
                setteled_amt1 = dr.GetValue(1).ToString();
                lit_payment_type.Text = dr["payment_type"].ToString();
                lit_utr_no.Text = dr["Utr_no"].ToString();

                // txt_deducted.Text = dr.GetValue(2).ToString();
                // utr_amt = Convert.ToSingle(dr.GetValue(2).ToString());
                d1.con1.Close();
            }
            else
            {
                lit_payment_type.Text = "";
                lit_utr_no.Text = "";
            }

            //MySqlCommand cmd2 = new MySqlCommand("SELECT `pay_minibank_master`.`ID`, ROUND(IFNULL(SUM(`pay_report_gst`.`received_amt2`), 0), 2) AS ' SETTLED_AMOUNT', ROUND(pay_minibank_master.`Amount` - (IFNULL(SUM(`pay_report_gst`.`received_amt2`), 0)), 2) AS 'REMANING_AMOUNT' FROM `pay_minibank_master`  LEFT JOIN `pay_report_gst` ON `pay_report_gst`.`payment_id2` = `pay_minibank_master`.`id` WHERE `pay_minibank_master`.`comp_code` = '" + Session["COMP_CODE"].ToString() + "' and pay_minibank_master.receive_date=str_to_date('" + txt_date.Text + "','%d-%m-%Y') and pay_minibank_master.client_code='" + ddl_client.SelectedValue + "' and pay_minibank_master.amount='" + ddl_client_resive_amt.SelectedItem + "'", d1.con1);
            //d1.con1.Close();
            //d1.con1.Open();
            //MySqlDataReader dr2 = cmd2.ExecuteReader();
            //if (dr2.Read())
            //{
            //    setteled_amt2 = dr2.GetValue(1).ToString();
            //    d1.con1.Close();
            //}
            try
            {
                utr_amt = Convert.ToDouble(ddl_client_resive_amt.SelectedItem.Text);
            }
            catch { }

            double total_setteled = Convert.ToDouble(setteled_amt1);// +Convert.ToDouble(setteled_amt2);

            txt_total_invoice.Text = (total_setteled).ToString();
            txt_deducted.Text = (utr_amt - total_setteled).ToString();

        }
        catch { }

    }
    protected void ddl_client_resive_amt_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
           // Check_setteled_amt();
            process_operation();
            Check_setteled_amt();
        }
        catch  { }
    
    }


    protected void gv_payment_SelectedIndexChanged(object sender, EventArgs e)
    {
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        string invoice_no = gv_payment.SelectedRow.Cells[7].Text;


        try
        {
            payment_details(invoice_no);
            // panel_payment_detail.Visible = true;
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d1.con1.Close();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        }

    }

    protected void gv_payment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (DataRowView)e.Row.DataItem;
            if (dr["receipt_de_approve"].ToString() != "0" && dr["receipt_de_approve"].ToString() != "3")
            {
                //LinkButton lb1 = e.Row.FindControl("unit_name") as LinkButton;
                //lb1.Visible = false;


                //  e.Row.Cells[14].Visible = false;
                // e.Row.Cells[15].Visible = false;

                LinkButton lb1 = e.Row.FindControl("lnk_remove_manual_other") as LinkButton;
                lb1.Visible = false;
                //LinkButton lbtn_approve1 = e.Row.FindControl("lbtn_approve") as LinkButton;
                //lbtn_approve1.Visible = false;


            }
        }



        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr1 = (DataRowView)e.Row.DataItem;


            e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.gv_payment, "Select$" + e.Row.RowIndex);

        }
        // e.Row.Cells[0].Visible = false;

        e.Row.Cells[1].Visible = false;
        e.Row.Cells[2].Visible = false;
        e.Row.Cells[3].Visible = false;
        // e.Row.Cells[4].Visible = false;
        e.Row.Cells[5].Visible = false;
        //e.Row.Cells[4].Visible = false;
        //e.Row.Cells[6].Visible = false;
        //e.Row.Cells[12].Visible = false;
    }
    protected void ddl_client_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        Bind_UTR_date();

    }

    private void Bind_UTR_date()
    {
        try
        {
            hidtab.Value = "1";
            string where_client = "";
            //if (ddl_client.SelectedValue == "TATA STEEL LTD" || ddl_client.SelectedValue == "TATA STEELS PVT LTD")
            //{
            //    where_client = "pay_minibank_master.client_code='7' ";
            //}
            //else
            //{
            //    where_client = " pay_minibank_master.client_name='" + ddl_client.SelectedValue + "' ";
            //}

            if (ddl_client.SelectedValue == "TATA STEEL LTD" || ddl_client.SelectedValue == "TATA STEELS PVT LTD")
            {
                where_client = " pay_minibank_master.client_code = '7'  ";
            }

            else if (ddl_client.SelectedValue == "EQUITAS SMALL FINANCE BANK" || ddl_client.SelectedValue == "Equitas Small Finance Bank Limited")
            {
                where_client = " pay_minibank_master.client_code IN ('ESFB','EquitasRes' ) ";
            }
            else if (ddl_client.SelectedValue == "TATA AIA Life Insurance Company Limited" || ddl_client.SelectedValue == "TATA AIA Life Insurance Company Limited Tem")
            {
                where_client = " pay_minibank_master.client_code  IN ('TAIL','TAILTEMP' ) ";
            }
            else if (ddl_client.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD." || ddl_client.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD. RM")
            {
                where_client = " pay_minibank_master.client_code IN ('RLIC HK','RNLIC RM' ) ";
            }
            else
            {
                where_client = " pay_minibank_master.client_name = '" + ddl_client.SelectedValue + "'  ";
            }


            string client_code_rd = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_client.SelectedValue + "' limit 1");

            txt_date.Items.Clear();
            txt_date.DataSource = null;
            txt_date.DataBind();
            //gv_links.DataSource = null;
            //gv_links.DataBind();
            if (ddl_client.SelectedValue != "Select")
            {
                //account_link_details.Visible = true;
                //gv_links.DataSource = null;
                //gv_links.DataBind();
                //  load_gv_payment("and payment_history.client_code = '" + ddl_client.SelectedValue + "'");
                DataTable dt_item = new DataTable();
                string where_client_code = "";
                //if (client_code_rd == "ESFB" || client_code_rd == "EquitasRes")
                //{
                //    where_client_code = "  pay_minibank_master.client_code IN ('ESFB','EquitasRes') ";
                //}
                //else if (client_code_rd == "RLIC HK" || client_code_rd == "RCFL" || client_code_rd == "RNLIC RM" || client_code_rd == "RCPL")
                //{
                //    where_client_code = " pay_minibank_master.client_code IN ('RLIC HK','RCFL','RCPL','RNLIC RM')  ";
                //}
                //else
                //{
                where_client_code = "  pay_minibank_master.comp_code = '" + Session["COMP_CODE"].ToString() + "'  and  "+where_client+" ";//pay_minibank_master.client_name='" + ddl_client.SelectedValue + "'
	           // }

                MySqlDataAdapter cmd_item = new MySqlDataAdapter("select   DATE_FORMAT(`receive_date`, '%d-%m-%Y') from( SELECT pay_minibank_master.ID,receive_date, ROUND(pay_minibank_master.Amount - ((IFNULL((select SUM(a.received_amt) from pay_report_gst a where a.payment_id=pay_minibank_master.id) , 0))+  (IFNULL((select SUM(b.received_amt2) from pay_report_gst b where b.payment_id2=pay_minibank_master.id) , 0))+  (IFNULL((select SUM(a.received_amt3) from pay_report_gst a where a.payment_id3=pay_minibank_master.id) , 0))), 2) AS 'REMANING_AMOUNT'  FROM pay_minibank_master LEFT JOIN  pay_report_gst ON pay_report_gst.payment_id = pay_minibank_master.id       WHERE  "+where_client_code+" AND `receipt_approve` != '0' and  ROUND(pay_minibank_master.Amount - (IFNULL((select SUM(a.received_amt) from pay_report_gst a where a.payment_id=pay_minibank_master.id) , 0)), 2) >0.99  GROUP BY pay_minibank_master.receive_date   ) as t1 where REMANING_AMOUNT >0.99 ", d.con);

                d.con.Open();

                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    txt_date.DataSource = dt_item;

                    txt_date.DataValueField = dt_item.Columns[0].ToString();

                    txt_date.DataBind();
                }
                txt_date.Items.Insert(0, "Select");
                dt_item.Dispose();
                d.con.Close();
                //display_close_date();
                // load client date wise amount
                ddl_client_resive_amt.DataSource = null;
                ddl_client_resive_amt.DataBind();
                txt_recived_am.Text = "0";
                txt_total_invoice.Text = "0";
                txt_deducted.Text = "0";


            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            hidtab.Value = "1";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);

        }

    }
    protected void load_client_amount()
    {

        try
        {
            DataTable dt_item = new DataTable();
            ddl_client_resive_amt.Items.Clear();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("select Id,amount from ( SELECT pay_minibank_master.id AS 'Id', ROUND((Amount - IFNULL(SUM(received_amt), 0)), 2) AS 'amount'   FROM pay_minibank_master LEFT JOIN pay_report_gst ON pay_minibank_master.id = pay_report_gst.payment_id AND pay_minibank_master.CLIENT_CODE = pay_report_gst.CLIENT_CODE WHERE pay_minibank_master.receive_date = date_format('" + txt_date.Text + "', '%Y-%m-%d') AND pay_minibank_master.client_code = '" + ddl_client.SelectedValue + "' GROUP BY pay_minibank_master.id, pay_report_gst.payment_id)  as t1 where  amount > 0 ORDER BY amount  ", d.con);
            d.con.Open();

            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_client_resive_amt.DataSource = dt_item;

                ddl_client_resive_amt.DataValueField = dt_item.Columns[0].ToString();
                ddl_client_resive_amt.DataTextField = dt_item.Columns[1].ToString();
                ddl_client_resive_amt.DataBind();
            }
            ddl_client_resive_amt.Items.Insert(0, "Select");
            dt_item.Dispose();
            d.con.Close();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            d.con.Close();
        }
    }
    //protected void ddl_state_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (ddl_state.SelectedValue != "ALL")
    //    {
    //        ddl_branch.Items.Clear();
    //        System.Data.DataTable dt_item = new System.Data.DataTable();
    //        MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT((SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_NAME,'_',UNIT_ADD1) as UNIT_NAME, unit_code,flag from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_state.SelectedValue + "' ORDER BY UNIT_CODE", d.con);
    //        d.con.Open();
    //        try
    //        {
    //            cmd_item.Fill(dt_item);
    //            if (dt_item.Rows.Count > 0)
    //            {
    //                ddl_branch.DataSource = dt_item;
    //                ddl_branch.DataTextField = dt_item.Columns[0].ToString();
    //                ddl_branch.DataValueField = dt_item.Columns[1].ToString();
    //                ddl_branch.DataBind();
    //            }
    //            ddl_branch.Items.Insert(0, "ALL");
    //            dt_item.Dispose();
    //            d.con.Close();

    //        }
    //        catch (Exception ex) { throw ex; }
    //        finally
    //        {
    //            d.con.Close();
    //        }
    //    }
    //}

    protected void payment_details(string invoice_no)
    {
        head_transction.Visible = true;
        d.con1.Open();
        try
        {
            gv_payment_detail.DataSource = null;
            gv_payment_detail.DataBind();
            DataSet ds1 = new DataSet();
            MySqlDataAdapter adp1 = new MySqlDataAdapter("select ID,Invoice_No as 'Invoice No',   `receipt_de_approve`,CASE  WHEN `receipt_de_approve` = '0' THEN 'Pending'  WHEN `receipt_de_approve` = '1' THEN 'Approve By Jr Acc'   WHEN `receipt_de_approve` = '2' THEN 'Approve By Sr Acc'  WHEN `receipt_de_approve` = '3' THEN 'Rejected By Sr Acc'  END AS 'Status' ,(SELECT CLIENT_NAME FROM pay_client_master WHERE pay_client_master.client_code = pay_report_gst.client_code AND pay_client_master.comp_code = pay_report_gst.comp_code) AS 'Client Name',Round(billing_amt,2) as 'Bill Amount',Round(received_amt,2) as 'Received Amount',Round(tds_amount,2) as 'TDS Amount' , CASE  WHEN (adjustment_sign = 1 && adjustment_amt > 0) THEN CONCAT('+', ROUND(adjustment_amt, 2)) WHEN (adjustment_sign = 2 && adjustment_amt > 0) THEN CONCAT('-', ROUND(adjustment_amt, 2)) ELSE ROUND(adjustment_amt, 2) END AS 'Adj Amt',date_format(received_date,'%d/%m/%Y') as 'Received Date',receipt_de_reasons as 'Reject_Reason' from pay_report_gst where comp_code = '" + Session["COMP_CODE"].ToString() + "' and Invoice_No='" + invoice_no + "'  order by Id", d.con1);
            adp1.Fill(ds1);
            gv_payment_detail.DataSource = ds1.Tables[0];
            gv_payment_detail.DataBind();
            d.con1.Close();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con1.Close();
        }
    }
    protected void gv_payment_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_payment.UseAccessibleHeader = false;
            gv_payment.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
    protected void btn_close_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }
    public void text_clear()
    {
        //txt_invoice_no.Text = "";
        //ViewState["client_code"] = "";
        //ViewState["state_name"] = "";
        //ViewState["unit_code"] = "";
        //ViewState["taxable_amount"] = 0;
        //gv_payment_detail.DataSource = null;
        //gv_payment_detail.DataBind();

        //txt_bill_amount.Text = "";
        //txt_receive_amount.Text = "0";
        //txt_receive_date.Text = "";
        //ddl_tds_amount.SelectedIndex = 0;
        //txt_tds_amt.Text = "0";
        //ddl_adjustment.SelectedIndex = 0;
        //txt_adment_amt.Text = "0";
    }
    protected void btn_process_Click(object sender, EventArgs e)
    {
        string invoice_list = "";
        string b_amt = "";
        div_invoice_list.Visible = false;
        try
        {
            double sum = 0;
            //int ck_count = 0;
            //double bal_amt = 0, tds_amt = 0, Tot_amt = 0, adj_count_amt = 0;
            //double received_amt = 0;
            //double received_amt2 = 0;
            //double total_received = 0;

            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            string client_code_rd = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_client.SelectedValue + "' limit 1");

            foreach (GridViewRow gvrow in gv_invoice_list.Rows)
            {
                string Invoice_no = (string)gv_invoice_list.DataKeys[gvrow.RowIndex].Value;
                //string Balanced_amount = gv_invoice_list.Rows[gvrow.RowIndex].Cells[6].Text;
                var checkbox = gvrow.FindControl("chk_invoice") as CheckBox;

                if (checkbox.Checked == true)
                {
                    invoice_list = invoice_list + "'" + Invoice_no + "',";
                }
            }
           

            if (invoice_list == "") { ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please Select Invoice !!!')", true); return; }
            if (invoice_list.Length > 0)
            {
                invoice_list = invoice_list.Substring(0, invoice_list.Length - 1);
            }
            else { invoice_list = "''"; }

            string where_client_code = "";
            if (client_code_rd == "ESFB" || client_code_rd == "EquitasRes")
            {
                where_client_code = "  pay_minibank_master.client_code IN ('ESFB','EquitasRes') ";
            }
            else if (client_code_rd == "7")
            {
                where_client_code = "  pay_minibank_master.client_code IN ('7') ";
            }
            else if (client_code_rd == "TAIL" || client_code_rd == "TAILTEMP")
            {
                where_client_code = "  pay_minibank_master.client_code IN ('TAIL','TAILTEMP') ";
            }  
            else if (client_code_rd == "RLIC HK" || client_code_rd == "RCFL" || client_code_rd == "RNLIC RM" || client_code_rd == "RCPL")
            {
                where_client_code = " pay_minibank_master.client_code IN ('RLIC HK','RCFL','RCPL','RNLIC RM')  ";
            }
            else
            {
                where_client_code = "  pay_minibank_master.comp_code = '" + Session["COMP_CODE"].ToString() + "'  and   pay_minibank_master.client_code='" + client_code_rd + "' and pay_minibank_master.client_name='" + ddl_client.SelectedValue + "'  ";
            }





            //`pay_minibank_master`.`comp_code` = '" + Session["COMP_CODE"].ToString() + "' and pay_minibank_master.client_code='" + ddl_client.SelectedValue + "'

            //d1.con1.Open();
            //MySqlCommand cmd = new MySqlCommand("SELECT `pay_minibank_master`.`ID`, ROUND(IFNULL(SUM(`pay_report_gst`.`received_amt`), 0), 2) AS ' SETTLED_AMOUNT', ROUND(pay_minibank_master.`Amount` - (IFNULL(SUM(`pay_report_gst`.`received_amt`), 0)), 2) AS 'REMANING_AMOUNT' FROM `pay_minibank_master`  LEFT JOIN `pay_report_gst` ON `pay_report_gst`.`payment_id` = `pay_minibank_master`.`id` WHERE  pay_minibank_master.receive_date=str_to_date('" + txt_date.Text + "','%d-%m-%Y')  and "+where_client_code+" and pay_minibank_master.amount='" + ddl_client_resive_amt.SelectedItem + "'", d1.con1);

            //MySqlDataReader dr = cmd.ExecuteReader();

            //if (dr.Read())
            //{
            //    txt_total_invoice.Text = dr.GetValue(1).ToString();
            //    txt_deducted.Text = dr.GetValue(2).ToString();
            //    d1.con1.Close();
            //}

            string where_client_code_gst = "";
            if (client_code_rd == "ESFB" || client_code_rd == "EquitasRes")
            {
                where_client_code_gst = "  pay_report_gst.client_code IN ('ESFB','EquitasRes') ";
            }
            else if (client_code_rd == "7")
            {
                where_client_code_gst = "  pay_report_gst.client_code IN ('7') ";
            }
            else if (client_code_rd == "TAIL" || client_code_rd == "TAILTEMP")
            {
                where_client_code_gst = "  pay_report_gst.client_code IN ('TAIL','TAILTEMP') ";
            }  
            else if (client_code_rd == "RLIC HK" || client_code_rd == "RCFL" || client_code_rd == "RNLIC RM" || client_code_rd == "RCPL")
            {
                where_client_code_gst = " pay_report_gst.client_code IN ('RLIC HK','RCFL','RCPL','RNLIC RM')  ";
            }
            else
            {
                where_client_code_gst = "  pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "'  and pay_report_gst.client_code='" + client_code_rd + "' and pay_report_gst.client_name='" + ddl_client.SelectedValue + "'  ";
            }
           // pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "'AND pay_report_gst.client_code = '" + ddl_client.SelectedValue + "'

            DataSet ds = new DataSet();
            gv_invoice_pmt.DataSource = null;
            gv_invoice_pmt.DataBind();

            gv_invoice_pmt.Columns[3].HeaderText = d.getsinglestring("SELECT CASE WHEN tds_applicable = 1 AND tds_percentage = 1 AND tds_on = 1 THEN 'TDS 1% ON BILLING AMOUNT'  WHEN tds_applicable = 1 AND tds_percentage = 1 AND tds_on = 2 THEN 'TDS 1% ON TAXABLE AMOUNT' WHEN tds_applicable = 1 AND tds_percentage = 2 AND tds_on = 1 THEN 'TDS 2% ON BILLING AMOUNT' WHEN tds_applicable = 1 AND tds_percentage = 2 AND tds_on = 2 THEN 'TDS 2% ON TAXABLE AMOUNT'  WHEN `tds_applicable` = 1 AND `tds_percentage` = 0.75 AND `tds_on` = 1 THEN 'TDS 0.75% ON BILLING AMOUNT'  WHEN `tds_applicable` = 1 AND `tds_percentage` = 0.75 AND `tds_on` = 2 THEN 'TDS 0.75% ON TAXABLE AMOUNT' WHEN `tds_applicable` = 1 AND `tds_percentage` = 1.5 AND `tds_on` = 1 THEN 'TDS 1.5% ON  BILLING AMOUNT' WHEN `tds_applicable` = 1 AND `tds_percentage` = 1.5 AND `tds_on` = 2 THEN 'TDS 1.5% ON TAXABLE AMOUNT'  ELSE 'TDS NOT APPLICABLE' END AS 'TDS STATUS' FROM pay_client_master WHERE comp_Code = '" + Session["COMP_CODE"].ToString() + "' AND client_code = '" + client_code_rd + "'");
          // 2- time payment received code
            // ds = d.select_data("SELECT Invoice_no,ROUND(amount, 2) AS 'billing_amt',ROUND(`tds_amt`, 2) AS 'tds_amt',ROUND((amount - `tds_amt`), 2) AS 'receviable_amt',`receving_date`,received_amt1 ,received_amt2, ROUND((received_amt1 + received_amt2), 2) AS total_received,ROUND(((`amount` - `tds_amt`) - (received_amt1 + received_amt2)), 2) AS balance,ROUND(`adj_amt`,2) AS 'adj_amt',tds,tds_on,adjustment_sign,deduct_amt,remark FROM (SELECT pay_report_gst.Invoice_No AS 'Invoice_no',(amount + cgst + sgst + igst) AS 'amount',ROUND(received_amt, 2) AS received_amt1,'" + txt_date.Text + "' AS 'receving_date',ROUND(received_amt2, 2) AS received_amt2,CASE WHEN pay_report_gst.tds_amount != '' and pay_report_gst.tds_amount != 0 THEN pay_report_gst.tds_amount  WHEN tds_applicable = 1 AND pay_client_master.tds_on = 1 THEN ROUND((((amount + cgst + sgst + igst) * tds_percentage) / 100), 2)WHEN tds_applicable = 1 AND pay_client_master.tds_on = 2 THEN ROUND((((amount) * tds_percentage) / 100), 2)ELSE 0 END AS 'tds_amt', 0 AS 'adj_amt',`amount` AS 'tds',0 AS 'tds_on',0 AS 'adjustment_sign',0 AS deduct_amt,'' AS remark FROM pay_report_gst INNER JOIN pay_client_master ON pay_report_gst.comp_code = pay_client_master.comp_Code AND pay_report_gst.client_code = pay_client_master.client_code where  " + where_client_code_gst + "  AND pay_report_gst.Invoice_No IN (" + invoice_list + ")AND pay_report_gst.flag_invoice = 2 GROUP BY pay_report_gst.Invoice_No , pay_report_gst.client_code ORDER BY pay_report_gst.Id) AS t1");
            // 3- time payment received code
            ds = d.select_data("SELECT Invoice_no,ROUND(amount, 2) AS 'billing_amt',ROUND(`tds_amt`, 2) AS 'tds_amt',ROUND((amount - `tds_amt`), 2) AS 'receviable_amt',`receving_date`,received_amt1 ,received_amt2,received_amt3, ROUND((received_amt1 + received_amt2+received_amt3), 2) AS total_received,ROUND(((`amount` - `tds_amt`) - (received_amt1 + received_amt2+received_amt3)-deduct_amt), 2) AS balance,ROUND(`adj_amt`,2) AS 'adj_amt',tds,tds_on,adjustment_sign,deduct_amt,remark FROM (SELECT pay_report_gst.Invoice_No AS 'Invoice_no',(amount + cgst + sgst + igst) AS 'amount',ROUND(received_amt, 2) AS received_amt1,'" + txt_date.Text + "' AS 'receving_date',ROUND(received_amt2, 2) AS received_amt2,ROUND(received_amt3, 2) AS received_amt3,CASE WHEN pay_report_gst.tds_amount != '' and pay_report_gst.tds_amount != 0 THEN pay_report_gst.tds_amount  WHEN tds_applicable = 1 AND pay_client_master.tds_on = 1 THEN ROUND((((amount + cgst + sgst + igst) * tds_percentage) / 100), 2)WHEN tds_applicable = 1 AND pay_client_master.tds_on = 2 THEN ROUND((((amount) * tds_percentage) / 100), 2)ELSE 0 END AS 'tds_amt', 0 AS 'adj_amt',`amount` AS 'tds',0 AS 'tds_on',0 AS 'adjustment_sign',ROUND(IFNULL(pay_report_gst.deduction_amt,0),2) AS deduct_amt,'' AS remark FROM pay_report_gst INNER JOIN pay_client_master ON pay_report_gst.comp_code = pay_client_master.comp_Code AND pay_report_gst.client_code = pay_client_master.client_code where  " + where_client_code_gst + "  AND pay_report_gst.Invoice_No IN (" + invoice_list + ")AND pay_report_gst.flag_invoice = 2 GROUP BY pay_report_gst.Invoice_No , pay_report_gst.client_code ORDER BY pay_report_gst.Id) AS t1");
            gv_invoice_pmt.DataSource = ds;
            gv_invoice_pmt.DataBind();

            Panel6.Visible = false;
            Panel_gv_pmt.Visible = true;
            btn_save.Visible = true;
            btn_approve_receipt_de.Visible = false;
            txt_recived_am.Visible = true;

            // }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);

        }

    }
    double t_tot_rece = 0;
    protected void gv_invoice_pmt_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        gv_invoice_pmt.Columns[11].Visible = false;

        //double sum = 0;
        //for (int i = 0; i < gv_invoice_pmt.Rows.Count; ++i)
        //{
        //    sum += Convert.ToDouble(gv_invoice_pmt.Rows[i].Cells[8].Text);
        //}
        //txt_recived_am.Text = sum.ToString();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txt1 = (TextBox)e.Row.FindControl("txt_received_amt1");
            TextBox txt2 = (TextBox)e.Row.FindControl("txt_received_amt2");
            TextBox txt3 = (TextBox)e.Row.FindControl("txt_received_amt3");

            TextBox txt_total_rec = (TextBox)e.Row.FindControl("txt_total_received");
            try
            {
                if (txt_total_rec.Text != "")
                {
                    t_tot_rece = t_tot_rece + Convert.ToDouble(txt_total_rec.Text);
                }
            }
            catch { }

            //int rowNum = gv_invoice_pmt.row.Index;

            DataRowView dr = (DataRowView)e.Row.DataItem;
            double received_amt1 = Convert.ToDouble(dr["received_amt1"].ToString());
            double received_amt2 = Convert.ToDouble(dr["received_amt2"].ToString());
            double received_amt3 = Convert.ToDouble(dr["received_amt3"].ToString());

            if (received_amt1 > 0 && received_amt2 == 0)
            {
                txt1.Enabled = false;
                txt2.Enabled = true;
                txt3.Enabled = false;
            }
            else if (received_amt2 > 0 && received_amt3 == 0)
            {
                txt1.Enabled = false;
                txt2.Enabled = false;
                txt3.Enabled = true;
            }
            else if (received_amt2 == 0 && received_amt1 == 0)
            {
                txt1.Enabled = true;
                txt2.Enabled = false;
                txt3.Enabled = false;
            }
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            ViewState["tot_rece_amt"] = t_tot_rece.ToString();

        }

        // ViewState["tot_rece_amt"] = t_tot_rece.ToString();
        //
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DropDownList ddl_tds_amount = e.Row.FindControl("ddl_tds_amount") as DropDownList;
            DropDownList ddl_tds_on = e.Row.FindControl("ddl_tds_on") as DropDownList;
            DropDownList ddl_adjustment = e.Row.FindControl("ddl_adjustment") as DropDownList;

        }
    }
    protected void btn_save_Click(object sender, EventArgs e)
    {
        //try { ScriptManager.RegisterStartupScript(this, this.GetType(), "callmyfunction", "unblock()", true); }
        //catch { }
        hidtab.Value = "1";
        string invoice_list = null;
        if (check_validation("Insert"))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "callmyfunction", "unblock()", true);

            return;
        }
        d.con1.Open();
        MySqlTransaction mtran = d.con1.BeginTransaction();
        try
        {

            int result = 0;
            //double t_amt = 0, 
            double bill_count_adj = 0;
            double recive_amt = Math.Round(Convert.ToDouble(txt_recived_am.Text),2);
            string invo_list = "", inv_list_pay_status = "", deduct_inv_list = "", deduct_amt_inv_list="";
            string date_list = "";
            string date1 = txt_date.SelectedItem.ToString();
            string payment_type = d.getsinglestring("select payment_type from pay_minibank_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_name = '" + ddl_client.SelectedValue + "' and id='" + ddl_client_resive_amt.SelectedValue+ "' and receive_date = str_to_date('" + txt_date.Text + "','%d-%m-%Y') limit 1");

            #region Payment Remark Validation
            foreach (GridViewRow row in gv_invoice_pmt.Rows)
            {

                if (row != null)
                {
                    bill_count_adj += 0.99;

                    string invoice_no1 = row.Cells[1].Text;
                    double txt_balance1 = double.Parse(((TextBox)row.FindControl("txt_balance")).Text);
                    string txt_remark1 = ((TextBox)row.FindControl("txt_remark")).Text;
                    string ddl_pay_status = ((DropDownList)row.FindControl("ddl_remark_head")).SelectedItem.Text;
                    string txt_deduct_amt = ((TextBox)row.FindControl("txt_deduct_amt")).Text;
                    if (txt_balance1 > 1 && (txt_remark1 == "" || ddl_pay_status=="Select"))
                    {
                        if (invo_list == "") { invo_list = invoice_no1; }
                        else
                        {
                            invo_list = invo_list + "," + invoice_no1;
                        }
                    }
                     if (txt_balance1 > 1 &&  ddl_pay_status=="Payment Done")
                    {
                        if (inv_list_pay_status == "") { inv_list_pay_status = invoice_no1; }
                        else
                        {
                            inv_list_pay_status = inv_list_pay_status + "," + invoice_no1;
                        }
                    }
                     if (double.Parse(txt_deduct_amt) > 0 && txt_remark1 == "")
                     {
                          if (deduct_inv_list == "") { deduct_inv_list = invoice_no1; }
                        else
                        {
                            deduct_inv_list = deduct_inv_list + "," + invoice_no1;
                        }
                     }

                     if ( ddl_pay_status=="Credit Note" && txt_deduct_amt=="0")
                     {
                         if (deduct_amt_inv_list == "") { deduct_amt_inv_list = invoice_no1; }
                        else
                        {
                            deduct_amt_inv_list = deduct_amt_inv_list + "," + invoice_no1;
                        } 
                     }

                }
            }
            if (invo_list != "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please Enter Remark OR Status for Invoice No : " + invo_list + "!!!')", true);
                return;
            }
            if (inv_list_pay_status != "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please select correct payment status for Invoice No : " + inv_list_pay_status + "!!!')", true);
                return; 
            }
            if (deduct_inv_list!="")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please Enter Remark  for Invoice No : " + deduct_inv_list + "!!!')", true);
                return;
            }
            if (deduct_amt_inv_list != "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please Enter deduction amount against Credit Note for Invoice No : " + deduct_amt_inv_list + "!!!')", true);
                return;
            }
           
            if (payment_type == "1")// Invoice Against
            {
                foreach (GridViewRow row in gv_invoice_pmt.Rows)
                {
                    if (row != null)
                    {
                        string invoice_no = row.Cells[1].Text;
                        string date2 = d.getsinglestring("select invoice_date from pay_report_gst where comp_code = '" + Session["comp_code"].ToString() + "'  and invoice_no ='" + invoice_no + "'"); //and client_name = 'Equitas Small Finance Bank Limited'
                        DateTime dt1 = Convert.ToDateTime(date1);
                        DateTime dt2 = Convert.ToDateTime(date2);
                        int value = DateTime.Compare(dt1, dt2);
                        if (value < 0)
                        {
                            if (invo_list == "") { invo_list = invoice_no; }
                            if (date_list == "") { date_list = dt2.ToString("dd/MM/yyyy"); }
                            else
                            {
                                date_list = date_list + "," + dt2.ToString("dd/MM/yyyy");
                                invo_list = invo_list + "," + invoice_no;
                            }
                        }
                    }
                }
                if (date_list != "")
                {
                  //  ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Receipt Date " + date1 + " Should Be Greater Than Invoice Date Of: " + invo_list + "!!!')", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Receipt Date " + date1 + " Should Be Greater Than Invoice Date Of: " + invo_list + "!!!')", true);
                          
                    return;
                }
            }
            #endregion
            //else
            //{
            double utr_amt_check = double.Parse(d.getsinglestring("select ROUND((received_amt1 + received_amt2 + received_amt3),2) as UTR_received_amt  from (select (select IFNULL(SUM(g.received_amt),0) from pay_report_gst g where g.payment_id='" + ddl_client_resive_amt.SelectedValue + "') as received_amt1, (select IFNULL(SUM(g2.received_amt2),0) from pay_report_gst g2 where g2.payment_id2='" + ddl_client_resive_amt.SelectedValue + "') as received_amt2,(select IFNULL(SUM(g3.received_amt3),0) from pay_report_gst g3 where g3.payment_id3='" + ddl_client_resive_amt.SelectedValue + "') as received_amt3) as t1 "));
                if (utr_amt_check > 0 && payment_type == "1")//payment_type == "1"----Invoice Against
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Same UTR against  " + utr_amt_check + "/- amount already received, You canot add more...!!! ')", true);
                    return;
                }
                else
                {
                    #region Receipt_detail Entry_code

                    if (payment_type == "1" && Convert.ToDouble(ddl_client_resive_amt.SelectedItem.Text) > (recive_amt) || Convert.ToDouble(ddl_client_resive_amt.SelectedItem.Text) <(recive_amt))
                    {

                        if (Convert.ToDouble(ddl_client_resive_amt.SelectedItem.Text) > (recive_amt))
                        {
                           // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Selected invoice amount  not equal to received amount')", true);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Selected invoice amount  not equal to received amount')", true);
                       
                            return;
                        }
                        else
                        {
                          //  ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Selected invoice amount is greter than recieved amount')", true);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Selected invoice amount is greter than recieved amount')", true);
                       
                            return;
                        }
                    }
                    else
                    {
                        double utr_rem_amt=0,inv_current_received_amt=0;
                        try{	        
		                     utr_rem_amt=Math.Round(Convert.ToDouble(txt_deducted.Text),2);
                             inv_current_received_amt = Math.Round(Convert.ToDouble(txt_recived_am.Text), 2);
	                        }
	                    catch {}



                        if (payment_type != "1" && utr_rem_amt < (inv_current_received_amt))
                        {

                            double extra_amt = Math.Round((inv_current_received_amt - utr_rem_amt), 2);
                          //  ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your Received amount is greater than remaining amount')", true);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Your Received amount is greater than remaining  UTR Amount.( " + extra_amt + "- Extra Received)')", true);
                       
                            return;
                        }
                        else
                        {

                        foreach (GridViewRow row in gv_invoice_pmt.Rows)
                        {
                            if (row != null)
                            {

                                //d.con.Open();
                                string invoice_no = row.Cells[1].Text;
                                double txt_balance = double.Parse(((TextBox)row.FindControl("txt_balance")).Text);
                                string txt_remark = ((TextBox)row.FindControl("txt_remark")).Text;
                                string ddl_pay_status = ((DropDownList)row.FindControl("ddl_remark_head")).SelectedItem.Text;

                                if (txt_balance == 0 && ddl_pay_status != "Credit Note")
                                {
                                    ddl_pay_status = "Payment Done";
                                }

                                //if (txt_balance > 1 && txt_remark == "")
                                //{
                                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please Enter Remark for " + invoice_no + "!!!')", true);

                                //}
                                //else
                                //{
                                result = 0;
                                //string invoice_no = row.Cells[1].Text;

                                double txt_bill_amount = double.Parse(row.Cells[2].Text);
                                double txt_tds_amt = double.Parse(((TextBox)row.FindControl("txt_tds_amt")).Text);

                                double txt_receive_amount = double.Parse(((TextBox)row.FindControl("txt_recive_amt")).Text);

                                string txt_reciving_date = ((TextBox)row.FindControl("txt_reciving_date")).Text;

                                double txt_received_amt1 = double.Parse(((TextBox)row.FindControl("txt_received_amt1")).Text);
                                double txt_received_amt2 = double.Parse(((TextBox)row.FindControl("txt_received_amt2")).Text);
                                double txt_received_amt3 = double.Parse(((TextBox)row.FindControl("txt_received_amt3")).Text);
                               // double txt_total_received = double.Parse(((TextBox)row.FindControl("txt_total_received")).Text);
                                double tds_amount = 0;
                                int adj_selectedvalue = 0;
                                double txt_adjustment_amt = 0;//double.Parse(((TextBox)row.FindControl("txt_adjustment_amt")).Text);
                                double txt_deduct_amt = double.Parse(((TextBox)row.FindControl("txt_deduct_amt")).Text);
                                int month = int.Parse(return_fields1("month", invoice_no));
                                int year = int.Parse(return_fields1("year", invoice_no));
                                tds_amount = txt_tds_amt;
                                double received_amt1 = double.Parse(d.getsinglestring("select ifnull(sum(received_amt + tds_amount), 0) from pay_report_gst where comp_code='" + Session["comp_code"].ToString() + "' and invoice_no='" + invoice_no + "' and client_name='" + ddl_client.SelectedValue + "'  order by id"));
                                //adjustment amount
                                string adj_sign = "";
                                try
                                {
                                    if (adj_selectedvalue == 1) { received_amt1 = received_amt1 + txt_adjustment_amt; }
                                    if (adj_selectedvalue == 2) { received_amt1 = received_amt1 - txt_adjustment_amt; }
                                    string matching = String.Format("{0:0.00}", (received_amt1 + txt_receive_amount + tds_amount));
                                    Double balance_amt = Convert.ToDouble(matching);
                                }
                                catch { }
                                try
                                {
                                    double received_amt2 = Convert.ToDouble(txt_received_amt2);
                                    double received_amt = Convert.ToDouble(txt_received_amt1);
                                    double received_amt3 = Convert.ToDouble(txt_received_amt3);
                                    long payment_id = Convert.ToInt64(ddl_client_resive_amt.SelectedValue);
                                    string query = "";
                                    long payment_id1 = 0, payment_id2 = 0, payment_id3 = 0;
                                    if (received_amt2 > 0 && received_amt > 0 && received_amt3==0)
                                    {
                                        payment_id2 = payment_id;
                                        received_amt2 = Convert.ToDouble(txt_received_amt2);
                                        query = " update pay_report_gst set `received_original_amount`='" + ddl_client_resive_amt.SelectedItem + "',  received_amt2 ='" + txt_received_amt2 + "', adjustment_amt ='0',adjustment_sign='0',received_date2= str_to_date('" + txt_reciving_date + "','%d-%m-%Y'),payment_id2='" + ddl_client_resive_amt.SelectedValue + "',uploaded_by2='" + Session["login_id"].ToString() + "',uploaded_date2=now(),deduction_amt='" + txt_deduct_amt + "',remark2='" + txt_remark + "',payment_status='" + ddl_pay_status + "' where invoice_no = '" + invoice_no + "' ";
                                        // query = " update pay_report_gst  set `received_original_amount`='" + ddl_client_resive_amt.SelectedItem + "',  billing_amt='" + txt_bill_amount + "',received_amt2 ='" + txt_received_amt2 + "',received_date2= str_to_date('" + txt_reciving_date + "','%d-%m-%Y'),total_received_amt='" + (received_amt + txt_receive_amount + tds_amount) + "',payment_id2='" + ddl_client_resive_amt.SelectedValue + "',uploaded_by2='" + Session["login_id"].ToString() + "',uploaded_date2=now(),deduction_amt='" + txt_deduct_amt + "',remark =  '" + txt_remark + "',received_amt2='" + txt_received_amt2 + "',uploaded_by2='" + Session["login_id"].ToString() + "',uploaded_date2=now(),total_received='" + txt_total_received + "',balance='" + txt_balance + "' where invoice_no = '" + invoice_no + "' ";

                                    }
                                    else if (received_amt2 > 0 && received_amt > 0 && received_amt3 > 0)
                                    {
                                        payment_id3 = payment_id;
                                        received_amt3 = Convert.ToDouble(txt_received_amt3);
                                        query = " update pay_report_gst set `received_original_amount`='" + ddl_client_resive_amt.SelectedItem + "',  received_amt3 ='" + txt_received_amt3 + "', adjustment_amt ='0',adjustment_sign='0',received_date3= str_to_date('" + txt_reciving_date + "','%d-%m-%Y'),payment_id3='" + ddl_client_resive_amt.SelectedValue + "',uploaded_by3='" + Session["login_id"].ToString() + "',uploaded_date3=now(),deduction_amt='" + txt_deduct_amt + "',remark3='" + txt_remark + "' ,payment_status='" + ddl_pay_status + "'  where invoice_no = '" + invoice_no + "' ";
                                        // query = " update pay_report_gst  set `received_original_amount`='" + ddl_client_resive_amt.SelectedItem + "',  billing_amt='" + txt_bill_amount + "',received_amt2 ='" + txt_received_amt2 + "',received_date2= str_to_date('" + txt_reciving_date + "','%d-%m-%Y'),total_received_amt='" + (received_amt + txt_receive_amount + tds_amount) + "',payment_id2='" + ddl_client_resive_amt.SelectedValue + "',uploaded_by2='" + Session["login_id"].ToString() + "',uploaded_date2=now(),deduction_amt='" + txt_deduct_amt + "',remark =  '" + txt_remark + "',received_amt2='" + txt_received_amt2 + "',uploaded_by2='" + Session["login_id"].ToString() + "',uploaded_date2=now(),total_received='" + txt_total_received + "',balance='" + txt_balance + "' where invoice_no = '" + invoice_no + "' ";

                                    }
                                    else
                                    {
                                        payment_id1 = payment_id;
                                        received_amt = Convert.ToDouble(txt_received_amt1);
                                        query = " update pay_report_gst set `received_original_amount`='" + ddl_client_resive_amt.SelectedItem + "', billing_amt='" + txt_bill_amount + "', received_amt ='" + txt_received_amt1 + "',tds_amount ='" + tds_amount + "', adjustment_amt ='0',adjustment_sign='0',received_date= str_to_date('" + txt_reciving_date + "','%d-%m-%Y'),payment_id='" + ddl_client_resive_amt.SelectedValue + "',uploaded_by='" + Session["login_id"].ToString() + "',uploaded_date=now(),deduction_amt='" + txt_deduct_amt + "',remark='" + txt_remark + "',payment_status='" + ddl_pay_status + "'  where invoice_no = '" + invoice_no + "' ";

                                    }
                                    string query1 = null, temp = "";
                                    d.con1.Close();
                                    d.con1.Open();
                                    MySqlCommand cmd = new MySqlCommand(query, d.con1);
                                    result = 1;
                                    cmd.ExecuteNonQuery();

                                    invoice_list = invoice_list + "'" + invoice_no + "',";

                                }
                                catch (Exception ex)
                                {
                                    mtran.Rollback();
                                    throw ex;
                                }
                                finally
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "callmyfunction", "unblock()", true);

                                }
                                // }
                            }

                        }
                        if (result > 0)
                        {
                            btn_save.Visible = false;
                            btn_approve_receipt_de.Visible = true;
                            string utr_id = ddl_client_resive_amt.SelectedValue;
                          
                            upload_file_ledger(utr_id);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Transaction Saved Successfully Please Approve Record !!!')", true);
                            mtran.Commit();
                            d.con1.Close();

                        }
                    }

                    //string id = d.getsinglestring("select max(id) from pay_minibank_master ");


                    }
                    #endregion
                }

           //}

        }
        catch
        {
            //hidtab.Value = "1";
            //mtran.Rollback();
            //throw ex;
        }
        finally
        {
            hidtab.Value = "1";
            d.con1.Close();
          //  ScriptManager.RegisterStartupScript(this, this.GetType(), "callmyfunction", "unblock()", true);

        }
    }

    protected void upload_file_ledger(string id)
    {
        string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_client.SelectedValue + "' limit 1");

       
        if (file_ledger_upload.HasFile)
        {
            try
            {

            string fileExt = System.IO.Path.GetExtension(file_ledger_upload.FileName);
            if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".RAR" || fileExt.ToUpper() == ".ZIP" || fileExt.ToUpper() == ".XLSX" || fileExt.ToUpper() == ".XLS" || fileExt.ToUpper() == ".DOC" || fileExt.ToUpper() == ".DOCX")
            {
                string fileName = Path.GetFileName(file_ledger_upload.PostedFile.FileName);
                photo_upload.PostedFile.SaveAs(Server.MapPath("~/Account_images/") + fileName);
                // string id = d.getsinglestring("select ifnull(max(id),0) from pay_debit_master ");

                //   string file_name = ddl_minibank_client.SelectedValue + id + fileExt;
                string file_name = "Ledger_" + client_code + id + fileExt;

                File.Copy(Server.MapPath("~/Account_images/") + fileName, Server.MapPath("~/Account_images/") + file_name, true);
                File.Delete(Server.MapPath("~/Account_images/") + fileName);

                d.operation("update pay_minibank_master set  ledger_copy='" + file_name + "'  where comp_code='" + Session["COMP_CODE"].ToString() + "' and id='" + id + "' ");
                // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Payment Uploaded Succsefully!!');", true);


            }

            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select only JPG, PNG , XLSX, XLS and PDF  Files  !!');", true);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please select only JPG, PNG and PDF Files !!!')", true);
                return;
            }

            }
            catch    {  }


        }
    }
   


    protected void btn_update_Click(object sender, EventArgs e)
    {
        //try { ScriptManager.RegisterStartupScript(this, this.GetType(), "callmyfunction", "unblock()", true); }
        //catch { }
        hidtab.Value = "1";
        string invoice_list = null;
        if (check_validation("Update"))
        {

            return;
        }
        d.con1.Open();
        MySqlTransaction mtran = d.con1.BeginTransaction();
        try
        {

            int result = 0;

            foreach (GridViewRow row in gv_invoice_pmt.Rows)
            {
                if (row != null)
                {

                    result = 0;
                    string invoice_no = row.Cells[1].Text;

                    double txt_bill_amount = double.Parse(row.Cells[2].Text);

                    double txt_receive_amount = double.Parse(((TextBox)row.FindControl("txt_recive_amt")).Text);

                    string txt_reciving_date = ((TextBox)row.FindControl("txt_reciving_date")).Text;

                    DropDownList ddl_tds_amount = (DropDownList)row.FindControl("ddl_tds_amount");

                    DropDownList ddl_tds_on = (DropDownList)row.FindControl("ddl_tds_on");

                    double txt_tds_amt = double.Parse(((TextBox)row.FindControl("txt_tds_amt")).Text);

                    int adj_selectedvalue = int.Parse(((DropDownList)row.FindControl("ddl_adjustment")).SelectedValue);

                    double txt_adjustment_amt = double.Parse(((TextBox)row.FindControl("txt_adjustment_amt")).Text);



                    int month = int.Parse(return_fields1("month", invoice_no));
                    int year = int.Parse(return_fields1("year", invoice_no));

                    double tds_amount = 0;


                    //tds amount
                    if (ddl_tds_amount.SelectedValue != "Amount")
                    {
                        int tds_persent = int.Parse(ddl_tds_amount.SelectedValue);
                        tds_amount = ddl_tds_on.SelectedValue == "0" ? (double.Parse(return_fields1("taxable_amount", invoice_no)) * tds_persent) / 100 : (txt_bill_amount * tds_persent) / 100;
                    }
                    else
                    {
                        tds_amount = txt_tds_amt;
                    }

                    //RD
                    double received_amt1 = double.Parse(d.getsinglestring("select ifnull(Round(sum(total_received_amt),2), 0) from pay_report_gst where comp_code='" + Session["comp_code"].ToString() + "' and invoice_no='" + invoice_no + "' and client_code='" + ddl_client.SelectedValue + "' and id != '" + ViewState["row_id"] + "'  order by id"));

                    //adjustment amount
                    string adj_sign = "";

                    if (adj_selectedvalue == 1) { received_amt1 = received_amt1 + txt_adjustment_amt; }

                    if (adj_selectedvalue == 2) { received_amt1 = received_amt1 - txt_adjustment_amt; }

                    if (txt_bill_amount >= (received_amt1 + txt_receive_amount + tds_amount))
                    {
                        try
                        {
                            string query = null;
                            query = "update pay_report_gst set received_amt1 = '" + txt_receive_amount + "', tds = '" + ddl_tds_amount.SelectedValue + "',tds_on =  '" + ddl_tds_on.SelectedValue + "',tds_amount = '" + tds_amount + "',adjustment_amt = '" + txt_adjustment_amt + "',total_received_amt = '" + (txt_receive_amount + tds_amount) + "',adjustment_sign = '" + adj_selectedvalue + "' ,uploaded_by = '" + Session["login_id"].ToString() + "',uploaded_date = now() where comp_code =  '" + Session["comp_code"].ToString() + "' and Id = '" + ViewState["row_id"] + "'";
                            MySqlCommand cmd = new MySqlCommand(query, d.con1);
                            result = 1;
                            cmd.ExecuteNonQuery();

                            invoice_list = invoice_list + "'" + invoice_no + "',";

                        }
                        catch (Exception ex)
                        {
                            mtran.Rollback();
                            throw ex;
                        }
                        finally
                        { }

                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Received Amount exceed Billing Amount !!!')", true);
                        result = 0;
                        mtran.Rollback();
                        Panel_gv_pmt.Visible = true;
                        return;
                    }

                }
            }
            if (result > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Transaction Updated Successfully !!!')", true);
                mtran.Commit();
                d.con1.Close();
                tran_clear();
                //panel2.Visible = true;
                Panel_gv_pmt.Visible = false;

                invoice_list = invoice_list.Length > 0 ? invoice_list.Substring(0, invoice_list.Length - 1) : "''";
                load_gv_payment("and payment_history.invoice_no in (" + invoice_list + ")");

            }

        }
        catch (Exception ex)
        {
            mtran.Rollback();
            throw ex;
        }
        finally
        {
            hidtab.Value = "1";
            d.con1.Close();
        }
    }
    protected string return_fields(string str, string invoice_no)
    {

        return d.getsinglestring("select ifnull(" + str + ",0) from payment_history where comp_code='" + Session["comp_code"].ToString() + "' and invoice_no='" + invoice_no + "'");
    }
    // from pay_report_gst all type invoice
    protected string return_fields1(string str, string invoice_no)
    {

        return d.getsinglestring("select ifnull(" + str + ",0) from pay_report_gst where comp_code='" + Session["comp_code"].ToString() + "' and invoice_no='" + invoice_no + "'");
    }

    protected void btn_pmt_close_click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }

    protected void tran_clear()
    {

        foreach (GridViewRow row in gv_invoice_pmt.Rows)
        {
            if (row != null)
            {


                row.Cells[1].Text = "";

                row.Cells[2].Text = "";

                ((TextBox)row.FindControl("txt_recive_amt")).Text = "0";
                ((TextBox)row.FindControl("txt_reciving_date")).Text = "";
                ((TextBox)row.FindControl("txt_received_amt1")).Text = "0";
                ((TextBox)row.FindControl("txt_received_amt2")).Text = "0";

                //((DropDownList)row.FindControl("ddl_tds_amount")).SelectedValue = "Amount";
                //((DropDownList)row.FindControl("ddl_tds_on")).SelectedValue = "0";

                ((TextBox)row.FindControl("txt_tds_amt")).Text = "0";

                ((DropDownList)row.FindControl("ddl_adjustment")).SelectedValue = "0";

                ((TextBox)row.FindControl("txt_adjustment_amt")).Text = "0";
            }
        }

    }

    protected void gv_payment_detail_SelectedIndexChanged(object sender, EventArgs e)
    {
        //  ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);

        ViewState["row_id"] = gv_payment_detail.SelectedRow.Cells[0].Text;


        try
        {
            DataSet ds = new DataSet();
            gv_invoice_pmt.DataSource = null;
            gv_invoice_pmt.DataBind();
            ds = d.select_data("SELECT pay_report_gst.Invoice_No AS 'Invoice_no', ROUND(pay_report_gst.billing_amt, 2) AS 'billing_amt',DATE_FORMAT(received_date, '%d/%m/%Y') AS 'receving_date',  tds,tds_on,tds_amount as 'tds_amt',adjustment_sign,adjustment_amt as 'adj_amt',payment_id FROM pay_report_gst WHERE pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "' and id = '" + ViewState["row_id"] + "'");
            //ds = d.select_data("SELECT Invoice_no,ROUND(amount, 2) AS 'billing_amt',ROUND(`tds_amt`, 2) AS 'tds_amt',ROUND((`amount` - `tds_amt`), 2) AS 'receviable_amt',`receving_date`,received_amt ,received_amt2, ROUND(((`amount` - `tds_amt`) - total_received), 2) AS balance,total_received,ROUND(`adj_amt`,2) AS 'adj_amt',tds,tds_on,adjustment_sign,deduct_amt,remark FROM(SELECT pay_report_gst.Invoice_No AS 'Invoice_no',(amount + cgst + sgst + igst) AS 'amount',ROUND(received_amt, 2) AS received_amt1,'" + txt_date.Text + "' AS 'receving_date',ROUND(received_amt2, 2) AS received_amt2,ROUND(SUM(received_amt + received_amt2), 2) AS total_received,CASE WHEN pay_report_gst.tds_amount != '' THEN pay_report_gst.tds_amount = '0' WHEN tds_applicable = 1 AND pay_client_master.tds_on = 1 THEN ROUND((((amount + cgst + sgst + igst) * tds_percentage) / 100), 2)WHEN tds_applicable = 1 AND pay_client_master.tds_on = 2 THEN ROUND((((amount) * tds_percentage) / 100), 2)ELSE 0 END AS 'tds_amt', 0 AS 'adj_amt',`amount` AS 'tds',0 AS 'tds_on',0 AS 'adjustment_sign',0 AS deduct_amt,'' AS remark FROM pay_report_gst INNER JOIN pay_client_master ON pay_report_gst.comp_code = pay_client_master.comp_Code AND pay_report_gst.client_code = pay_client_master.client_code  where pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "'AND pay_report_gst.client_code = '" + ddl_client.SelectedValue + "'AND pay_report_gst.Invoice_No IN (" + invoice_list + ")AND pay_report_gst.flag_invoice = 2 GROUP BY pay_report_gst.Invoice_No , pay_report_gst.client_code ORDER BY pay_report_gst.Id) AS t1");

            gv_invoice_pmt.DataSource = ds;
            gv_invoice_pmt.DataBind();
            Panel6.Visible = false;
            Panel_gv_pmt.Visible = true;
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d1.con1.Close();
            load_client_details();
            btn_update.Visible = true;
            btn_save.Visible = false;
        }

    }
    protected void load_client_details()
    {
        try
        {
            d1.con1.Open();
            MySqlCommand cmd = new MySqlCommand("select date_format(received_date,'%d/%m/%Y'),client_code,payment_id from pay_report_gst where comp_code='" + Session["COMP_CODE"].ToString() + "' and Id = '" + ViewState["row_id"] + "'", d1.con1);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                txt_date.Text = dr.GetValue(0).ToString();
                ddl_client.SelectedValue = dr.GetValue(1).ToString();
                ddl_client_SelectedIndexChanged(null, null);
                ddl_client_resive_amt.SelectedValue = dr.GetValue(2).ToString();
                d1.con1.Close();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally { d1.con1.Close(); }

    }
    protected void gv_payment_detail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (DataRowView)e.Row.DataItem;
            if (dr["receipt_de_approve"].ToString() != "0" && dr["receipt_de_approve"].ToString() != "3")
            {
                //LinkButton lb1 = e.Row.FindControl("unit_name") as LinkButton;
                //lb1.Visible = false;


                //  e.Row.Cells[14].Visible = false;
                // e.Row.Cells[15].Visible = false;

                LinkButton lb1 = e.Row.FindControl("lnk_remove_product") as LinkButton;
                lb1.Visible = false;


            }
        }


        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
            //e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            //e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.gv_payment_detail, "Select$" + e.Row.RowIndex);

        }
        //e.Row.Cells[0].Visible = false;
        e.Row.Cells[1].Visible = false;
        e.Row.Cells[2].Visible = false;
        e.Row.Cells[4].Visible = false;
        //e.Row.Cells[2].Visible = false;
    }
    protected void load_gv_payment(string where)
    {
        string gv_where = " ";
        d.con1.Open();
        try
        {
            DataSet ds1 = new DataSet();
            MySqlDataAdapter adp2 = null;
            //MySqlDataAdapter adp1 = new MySqlDataAdapter("SELECT payment_history.Id, payment_history.client_code, payment_history.comp_code, DATE_FORMAT(payment_history.billing_date, '%d/%m/%Y') AS 'Bill Date', payment_history.Invoice_No AS 'Invoice No', payment_history.client_name AS 'Client Name', payment_history.state_name AS 'State', payment_history.unit_name AS 'Branch', CONCAT(payment_history.month, '/', payment_history.year) AS 'MONTH', ROUND(payment_history.taxable_amount, 2) AS 'Taxable Amount', ROUND(payment_history.GST_Amount, 2) AS 'GST', ROUND(payment_history.billing_amt) AS 'Bill Amount', IFNULL(SUM(ROUND(pay_report_gst.received_amt + tds_amount)), 0) AS 'Received Amount', pay_report_gst.tds_amount,(ROUND(payment_history.billing_amt) - IFNULL(ROUND(SUM(pay_report_gst.received_amt + tds_amount)), 0)) AS 'Balanced Amount',DATE_FORMAT( `pay_report_gst`.`received_date`, '%d/%m/%Y') AS 'Received date'  FROM payment_history  LEFT JOIN pay_report_gst ON payment_history.Invoice_No = pay_report_gst.Invoice_No  WHERE payment_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' " + where + " AND payment_history.invoice_flag = 2 GROUP BY payment_history.Invoice_No, payment_history.client_code ORDER BY Id", d.con1);
            if (ddl_client_gv.SelectedValue == "ALL")
            {
                if (ddl_type.SelectedValue == "1")
                { 
                    gv_where = "  where  Balanced_Amount <= 0.99 ";
                }
                else if (ddl_type.SelectedValue == "2")
                {
                    gv_where = "  where  Balanced_Amount > 0.99 ";     
                }
                else if (ddl_type.SelectedValue == "ALL")
                {
                    gv_where = " ";
                }
               // adp2 = new MySqlDataAdapter("SELECT Id,client_code,Bill_Date,receipt_de_approve,Status,Invoice_No,Client_Name,State,Branch,MONTH,Taxable_Amount,GST,Bill_Amount,tds_amount,Receivable_Amount,received_amt as Received_amt1,received_amt2,received_amt3,ROUND((received_amt+received_amt2+received_amt3),2) as total_received,Balanced_Amount,Received_date as Received_date1,Received_date2,Received_date3,Utr_no as Utr_no1,Utr_Amount as Utr_Amount1 ,Utr_no2, Utr_Amount2,Utr_no3, Utr_Amount3,Entry_by_user1,entry_By_date1,Entry_by_user2,entry_By_date2 ,deduction_amt,remark, CASE WHEN Balanced_Amount > 0 AND (ROUND((received_amt + received_amt2), 2)) = 0 THEN 'Not Received' WHEN Balanced_Amount BETWEEN - 1 AND 1 THEN 'Received Done' WHEN Balanced_Amount >= - 5 AND Balanced_Amount <= - 1 THEN '1 to 5 (-) Extra Done Payment' WHEN Balanced_Amount >= 1 AND Balanced_Amount <= 5 THEN '1 to 5 Less Done Payment'  WHEN Balanced_Amount >= - 100 AND Balanced_Amount < - 5 THEN '5 to 100 (-) Extra Payment' WHEN Balanced_Amount > 5 AND Balanced_Amount <= 100 THEN '5 to 100 Less  Payment'   WHEN Balanced_Amount < - 100 THEN 'Extra Payment Received' WHEN Balanced_Amount > 100 THEN 'Less Payment Received' END AS Balance_Status   FROM ( SELECT pay_report_gst.deduction_amt,    case   WHEN pay_report_gst.received_amt3 >0 then pay_report_gst.remark3  WHEN pay_report_gst.received_amt2 >0 then pay_report_gst.remark2  else pay_report_gst.remark  END as remark, pay_report_gst.Id,pay_report_gst.client_code,pay_minibank_master.Utr_no,pay_minibank_master.Amount  as 'Utr_Amount',receipt_de_reasons AS 'Reject_Reason',  `receipt_de_approve`,  CASE WHEN `receipt_de_approve` = '0' THEN 'Pending' WHEN `receipt_de_approve` = '1'  THEN 'Approve By Jr Acc' WHEN `receipt_de_approve` = '2' THEN 'Approve By Sr Acc' WHEN `receipt_de_approve` = '3' THEN 'Rejected By Sr Acc' END AS 'Status',    DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Bill_Date',pay_report_gst.Invoice_No AS 'Invoice_No',pay_report_gst.client_name AS 'Client_Name',     pay_report_gst.state_name AS 'State',pay_report_gst.unit_code AS 'Branch',DATE_FORMAT(pay_report_gst.invoice_date,'%m-%Y') AS 'MONTH', ROUND((pay_report_gst.amount), 2)     AS 'Taxable_Amount',ROUND((`cgst` + `sgst` + `igst`), 2) AS 'GST',ROUND((pay_report_gst.amount + `cgst` + `sgst` + `igst`), 2) AS 'Bill_Amount',     IFNULL(ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`) - pay_report_gst.tds_amount),2), 0) AS 'Receivable_Amount',pay_report_gst.tds_amount, pay_report_gst.received_amt, pay_report_gst.received_amt2, pay_report_gst.received_amt3, DATE_FORMAT(pay_report_gst.received_date3, '%d/%m/%Y') AS 'Received_date3', mb3.Utr_no as Utr_no3,mb3.Amount  as 'Utr_amount3',  DATE_FORMAT(pay_report_gst.received_date2, '%d/%m/%Y') AS 'Received_date2',pay_minibank_master.Amount as UTR_amt1,  mb2.Utr_no as Utr_no2,mb2.Amount  as 'Utr_amount2',       ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`)-pay_report_gst.`tds_amount`) - IFNULL(ROUND((`pay_report_gst`.`received_amt` + `pay_report_gst`.`received_amt2`+ `pay_report_gst`.`received_amt3`),2),0), 2)      AS 'Balanced_Amount',    pay_report_gst.payment,DATE_FORMAT(pay_report_gst.received_date, '%d/%m/%Y') AS 'Received_date',CONCAT(pay_report_gst.uploaded_by, '-', pay_employee_master.EMP_NAME)      AS Entry_by_user1, DATE_FORMAT(pay_report_gst.uploaded_date, '%d/%m/%Y %H:%i:%s') AS entry_By_date1,  CONCAT(pay_report_gst.uploaded_by2, '-', e2.EMP_NAME) AS Entry_by_user2,      DATE_FORMAT(pay_report_gst.uploaded_date2, '%d/%m/%Y %H:%i:%s') AS entry_By_date2    FROM pay_report_gst     LEFT JOIN pay_employee_master ON pay_report_gst.uploaded_by = pay_employee_master.emp_code          LEFT JOIN pay_employee_master e2 ON pay_report_gst.uploaded_by2 = e2.emp_code    LEFT JOIN pay_minibank_master ON pay_report_gst.payment_id = pay_minibank_master.id AND pay_report_gst.comp_code = pay_minibank_master.comp_code  LEFT JOIN pay_minibank_master mb2 ON pay_report_gst.payment_id2 = mb2.id and pay_report_gst.comp_code = mb2.comp_code    LEFT JOIN pay_minibank_master mb3 ON pay_report_gst.payment_id3 = mb3.id and pay_report_gst.comp_code = mb3.comp_code  WHERE pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_report_gst.flag_invoice = 2  GROUP BY pay_report_gst.Invoice_No ,pay_report_gst.client_code ORDER BY Id) as t1  " + gv_where + "", d.con1);
                adp2 = new MySqlDataAdapter("SELECT Id,client_code,Bill_Date,receipt_de_approve,Status,Invoice_No,Client_Name,State,Branch,MONTH,Taxable_Amount,GST,Bill_Amount,tds_amount,Receivable_Amount,received_amt as Received_amt1,received_amt2,received_amt3,ROUND((received_amt+received_amt2+received_amt3),2) as total_received,deduction_amt,Balanced_Amount,Received_date as Received_date1,Received_date2,Received_date3,Utr_no as Utr_no1,Utr_Amount as Utr_Amount1 ,Utr_no2, Utr_Amount2,Utr_no3, Utr_Amount3,Entry_by_user1,entry_By_date1,Entry_by_user2,entry_By_date2 ,remark, CASE WHEN Balanced_Amount > 0 AND (ROUND((received_amt + received_amt2+received_amt3), 2)) = 0 THEN 'Not Received' WHEN Balanced_Amount BETWEEN - 1 AND 1 THEN 'Received Done' WHEN Balanced_Amount >= - 5 AND Balanced_Amount <= - 1 THEN '1 to 5 (-) Extra Done Payment' WHEN Balanced_Amount >= 1 AND Balanced_Amount <= 5 THEN '1 to 5 Less Done Payment'  WHEN Balanced_Amount >= - 100 AND Balanced_Amount < - 5 THEN '5 to 100 (-) Extra Payment' WHEN Balanced_Amount > 5 AND Balanced_Amount <= 100 THEN '5 to 100 Less  Payment'   WHEN Balanced_Amount < - 100 THEN 'Extra Payment Received' WHEN Balanced_Amount > 100 THEN 'Less Payment Received' END AS Balance_Status ,payment_status,invoice_days as InvoiceDays   FROM  ( SELECT ROUND(IFNULL(pay_report_gst.deduction_amt,0),2) as deduction_amt,    case   WHEN pay_report_gst.received_amt3 >0 then pay_report_gst.remark3  WHEN pay_report_gst.received_amt2 >0 then pay_report_gst.remark2  else pay_report_gst.remark  END as remark, pay_report_gst.Id,pay_report_gst.client_code,pay_minibank_master.Utr_no,pay_minibank_master.Amount  as 'Utr_Amount',receipt_de_reasons AS 'Reject_Reason',  `receipt_de_approve`,  CASE WHEN `receipt_de_approve` = '0' THEN 'Pending' WHEN `receipt_de_approve` = '1'  THEN 'Approve By Jr Acc' WHEN `receipt_de_approve` = '2' THEN 'Approve By Sr Acc' WHEN `receipt_de_approve` = '3' THEN 'Rejected By Sr Acc' END AS 'Status',    DATE_FORMAT(pay_report_gst.invoice_date, '%d-%m-%Y') AS 'Bill_Date',pay_report_gst.Invoice_No AS 'Invoice_No',pay_report_gst.client_name AS 'Client_Name',     pay_report_gst.state_name AS 'State',pay_report_gst.unit_code AS 'Branch',DATE_FORMAT(pay_report_gst.invoice_date,'%b-%Y') AS 'MONTH', ROUND((pay_report_gst.amount), 2)     AS 'Taxable_Amount',ROUND((`cgst` + `sgst` + `igst`), 2) AS 'GST',ROUND((pay_report_gst.amount + `cgst` + `sgst` + `igst`), 2) AS 'Bill_Amount',     IFNULL(ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`) - pay_report_gst.tds_amount),2), 0) AS 'Receivable_Amount',pay_report_gst.tds_amount, pay_report_gst.received_amt, pay_report_gst.received_amt2, pay_report_gst.received_amt3, DATE_FORMAT(pay_report_gst.received_date3, '%d/%m/%Y') AS 'Received_date3', mb3.Utr_no as Utr_no3,mb3.Amount  as 'Utr_amount3',  DATE_FORMAT(pay_report_gst.received_date2, '%d/%m/%Y') AS 'Received_date2',pay_minibank_master.Amount as UTR_amt1,  mb2.Utr_no as Utr_no2,mb2.Amount  as 'Utr_amount2',       ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`)-pay_report_gst.`tds_amount`) - IFNULL(ROUND((`pay_report_gst`.`received_amt` + `pay_report_gst`.`received_amt2`+ `pay_report_gst`.`received_amt3`),2),0)-IFNULL(`pay_report_gst`.`deduction_amt`,0), 2)      AS 'Balanced_Amount',    pay_report_gst.payment,DATE_FORMAT(pay_report_gst.received_date, '%d/%m/%Y') AS 'Received_date',CONCAT(pay_report_gst.uploaded_by, '-', pay_employee_master.EMP_NAME)      AS Entry_by_user1, DATE_FORMAT(pay_report_gst.uploaded_date, '%d/%m/%Y %H:%i:%s') AS entry_By_date1,  CONCAT(pay_report_gst.uploaded_by2, '-', e2.EMP_NAME) AS Entry_by_user2,      DATE_FORMAT(pay_report_gst.uploaded_date2, '%d/%m/%Y %H:%i:%s') AS entry_By_date2,pay_report_gst.payment_status as payment_status,datediff(now(),invoice_date) as invoice_days   FROM pay_report_gst     LEFT JOIN pay_employee_master ON pay_report_gst.uploaded_by = pay_employee_master.emp_code          LEFT JOIN pay_employee_master e2 ON pay_report_gst.uploaded_by2 = e2.emp_code    LEFT JOIN pay_minibank_master ON pay_report_gst.payment_id = pay_minibank_master.id AND pay_report_gst.comp_code = pay_minibank_master.comp_code  LEFT JOIN pay_minibank_master mb2 ON pay_report_gst.payment_id2 = mb2.id and pay_report_gst.comp_code = mb2.comp_code    LEFT JOIN pay_minibank_master mb3 ON pay_report_gst.payment_id3 = mb3.id and pay_report_gst.comp_code = mb3.comp_code  WHERE pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_report_gst.flag_invoice = 2  GROUP BY pay_report_gst.Invoice_No ,pay_report_gst.client_code ORDER BY Id) as t1  " + gv_where + "", d.con1);
            }
            else if (ddl_client_gv.SelectedValue != "ALL")
            {
                string where_client = "";

                //if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
                //{
                //    where_client = " and pay_report_gst.client_code = '7'  ";
                //}
                //else //if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
                //{
                //    where_client = " and pay_report_gst.client_name = '" + ddl_client_gv.SelectedValue + "'  ";
                //}


                if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
                {
                    where_client = " and pay_report_gst.client_code = '7'  ";
                }
                else if (ddl_client_gv.SelectedValue == "EQUITAS SMALL FINANCE BANK" || ddl_client_gv.SelectedValue == "Equitas Small Finance Bank Limited")
                {
                    where_client = " and pay_report_gst.client_code  IN ('ESFB','EquitasRes' ) ";
                }
                else if (ddl_client_gv.SelectedValue == "TATA AIA Life Insurance Company Limited" || ddl_client_gv.SelectedValue == "TATA AIA Life Insurance Company Limited Tem")
                {
                    where_client = " and pay_report_gst.client_code  IN ('TAIL','TAILTEMP' ) ";
                }
                else if (ddl_client_gv.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD." || ddl_client_gv.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD. RM")
                {
                    where_client = " and pay_report_gst.client_code  IN ('RLIC HK','RNLIC RM' ) ";
                }
                else
                {
                    where_client = " and pay_report_gst.client_name = '" + ddl_client_gv.SelectedValue + "'  ";
                }


                if (ddl_type.SelectedValue == "1")
                {
                    gv_where = "  where  Balanced_Amount <= 0.99 ";
                }
                else if (ddl_type.SelectedValue == "2")
                {
                    gv_where = "  where  Balanced_Amount > 0.99 ";
                }
                else if (ddl_type.SelectedValue == "ALL")
                {
                    gv_where = " ";
                }
              // adp2 = new MySqlDataAdapter("SELECT Id,client_code,Bill_Date,receipt_de_approve,Status,Invoice_No,Client_Name,State,Branch,MONTH,Taxable_Amount,GST,Bill_Amount,tds_amount,Receivable_Amount,received_amt as Received_amt1,received_amt2,received_amt3,ROUND((received_amt+received_amt2+received_amt3),2) as total_received,Balanced_Amount,Received_date as Received_date1,Received_date2,Received_date3,Utr_no as Utr_no1,Utr_Amount as Utr_Amount1 ,Utr_no2, Utr_Amount2,Utr_no3, Utr_Amount3,Entry_by_user1,entry_By_date1,Entry_by_user2,entry_By_date2 ,deduction_amt,remark, CASE WHEN Balanced_Amount > 0 AND (ROUND((received_amt + received_amt2), 2)) = 0 THEN 'Not Received' WHEN Balanced_Amount BETWEEN - 1 AND 1 THEN 'Received Done' WHEN Balanced_Amount >= - 5 AND Balanced_Amount <= - 1 THEN '1 to 5 (-) Extra Done Payment' WHEN Balanced_Amount >= 1 AND Balanced_Amount <= 5 THEN '1 to 5 Less Done Payment'  WHEN Balanced_Amount >= - 100 AND Balanced_Amount < - 5 THEN '5 to 100 (-) Extra Payment' WHEN Balanced_Amount > 5 AND Balanced_Amount <= 100 THEN '5 to 100 Less  Payment'   WHEN Balanced_Amount < - 100 THEN 'Extra Payment Received' WHEN Balanced_Amount > 100 THEN 'Less Payment Received' END AS Balance_Status  FROM ( SELECT pay_report_gst.deduction_amt,    case   WHEN pay_report_gst.received_amt3 >0 then pay_report_gst.remark3  WHEN pay_report_gst.received_amt2 >0 then pay_report_gst.remark2  else pay_report_gst.remark  END as remark, pay_report_gst.Id,pay_report_gst.client_code,pay_minibank_master.Utr_no,pay_minibank_master.Amount  as 'Utr_Amount',receipt_de_reasons AS 'Reject_Reason',  `receipt_de_approve`,  CASE WHEN `receipt_de_approve` = '0' THEN 'Pending' WHEN `receipt_de_approve` = '1'  THEN 'Approve By Jr Acc' WHEN `receipt_de_approve` = '2' THEN 'Approve By Sr Acc' WHEN `receipt_de_approve` = '3' THEN 'Rejected By Sr Acc' END AS 'Status',    DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Bill_Date',pay_report_gst.Invoice_No AS 'Invoice_No',pay_report_gst.client_name AS 'Client_Name',     pay_report_gst.state_name AS 'State',pay_report_gst.unit_code AS 'Branch',DATE_FORMAT(pay_report_gst.invoice_date,'%m-%Y') AS 'MONTH', ROUND((pay_report_gst.amount), 2)     AS 'Taxable_Amount',ROUND((`cgst` + `sgst` + `igst`), 2) AS 'GST',ROUND((pay_report_gst.amount + `cgst` + `sgst` + `igst`), 2) AS 'Bill_Amount',     IFNULL(ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`) - pay_report_gst.tds_amount),2), 0) AS 'Receivable_Amount',pay_report_gst.tds_amount, pay_report_gst.received_amt, pay_report_gst.received_amt2, pay_report_gst.received_amt3, DATE_FORMAT(pay_report_gst.received_date3, '%d/%m/%Y') AS 'Received_date3', mb3.Utr_no as Utr_no3,mb3.Amount  as 'Utr_amount3',  DATE_FORMAT(pay_report_gst.received_date2, '%d/%m/%Y') AS 'Received_date2',pay_minibank_master.Amount as UTR_amt1,  mb2.Utr_no as Utr_no2,mb2.Amount  as 'Utr_amount2',       ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`)-pay_report_gst.`tds_amount`) - IFNULL(ROUND((`pay_report_gst`.`received_amt` + `pay_report_gst`.`received_amt2`+ `pay_report_gst`.`received_amt3`),2),0), 2)      AS 'Balanced_Amount',    pay_report_gst.payment,DATE_FORMAT(pay_report_gst.received_date, '%d/%m/%Y') AS 'Received_date',CONCAT(pay_report_gst.uploaded_by, '-', pay_employee_master.EMP_NAME)      AS Entry_by_user1, DATE_FORMAT(pay_report_gst.uploaded_date, '%d/%m/%Y %H:%i:%s') AS entry_By_date1,  CONCAT(pay_report_gst.uploaded_by2, '-', e2.EMP_NAME) AS Entry_by_user2,      DATE_FORMAT(pay_report_gst.uploaded_date2, '%d/%m/%Y %H:%i:%s') AS entry_By_date2    FROM pay_report_gst     LEFT JOIN pay_employee_master ON pay_report_gst.uploaded_by = pay_employee_master.emp_code          LEFT JOIN pay_employee_master e2 ON pay_report_gst.uploaded_by2 = e2.emp_code    LEFT JOIN pay_minibank_master ON pay_report_gst.payment_id = pay_minibank_master.id AND pay_report_gst.comp_code = pay_minibank_master.comp_code  LEFT JOIN pay_minibank_master mb2 ON pay_report_gst.payment_id2 = mb2.id and pay_report_gst.comp_code = mb2.comp_code    LEFT JOIN pay_minibank_master mb3 ON pay_report_gst.payment_id3 = mb3.id and pay_report_gst.comp_code = mb3.comp_code      WHERE pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "'  and pay_report_gst.client_code = '" + ddl_client_gv.SelectedValue + "'  and pay_report_gst.client_name = '" + ddl_client_gv.SelectedItem.Text + "'   AND pay_report_gst.flag_invoice = 2  GROUP BY pay_report_gst.Invoice_No ,pay_report_gst.client_code ORDER BY Id) as t1  " + gv_where + "", d.con1);
                adp2 = new MySqlDataAdapter("SELECT Id,client_code,Bill_Date,receipt_de_approve,Status,Invoice_No,Client_Name,State,Branch,MONTH,Taxable_Amount,GST,Bill_Amount,tds_amount,Receivable_Amount,received_amt as Received_amt1,received_amt2,received_amt3,ROUND((received_amt+received_amt2+received_amt3),2) as total_received,deduction_amt,Balanced_Amount,Received_date as Received_date1,Received_date2,Received_date3,Utr_no as Utr_no1,Utr_Amount as Utr_Amount1 ,Utr_no2, Utr_Amount2,Utr_no3, Utr_Amount3,Entry_by_user1,entry_By_date1,Entry_by_user2,entry_By_date2,remark, CASE WHEN Balanced_Amount > 0 AND (ROUND((received_amt + received_amt2+received_amt3), 2)) = 0 THEN 'Not Received' WHEN Balanced_Amount BETWEEN - 1 AND 1 THEN 'Received Done' WHEN Balanced_Amount >= - 5 AND Balanced_Amount <= - 1 THEN '1 to 5 (-) Extra Done Payment' WHEN Balanced_Amount >= 1 AND Balanced_Amount <= 5 THEN '1 to 5 Less Done Payment'  WHEN Balanced_Amount >= - 100 AND Balanced_Amount < - 5 THEN '5 to 100 (-) Extra Payment' WHEN Balanced_Amount > 5 AND Balanced_Amount <= 100 THEN '5 to 100 Less  Payment'   WHEN Balanced_Amount < - 100 THEN 'Extra Payment Received' WHEN Balanced_Amount > 100 THEN 'Less Payment Received' END AS Balance_Status ,payment_status,invoice_days as InvoiceDays   FROM  ( SELECT ROUND(IFNULL(pay_report_gst.deduction_amt,0),2) as deduction_amt,    case   WHEN pay_report_gst.received_amt3 >0 then pay_report_gst.remark3  WHEN pay_report_gst.received_amt2 >0 then pay_report_gst.remark2  else pay_report_gst.remark  END as remark, pay_report_gst.Id,pay_report_gst.client_code,pay_minibank_master.Utr_no,pay_minibank_master.Amount  as 'Utr_Amount',receipt_de_reasons AS 'Reject_Reason',  `receipt_de_approve`,  CASE WHEN `receipt_de_approve` = '0' THEN 'Pending' WHEN `receipt_de_approve` = '1'  THEN 'Approve By Jr Acc' WHEN `receipt_de_approve` = '2' THEN 'Approve By Sr Acc' WHEN `receipt_de_approve` = '3' THEN 'Rejected By Sr Acc' END AS 'Status',    DATE_FORMAT(pay_report_gst.invoice_date, '%d-%m-%Y') AS 'Bill_Date',pay_report_gst.Invoice_No AS 'Invoice_No',pay_report_gst.client_name AS 'Client_Name',     pay_report_gst.state_name AS 'State',pay_report_gst.unit_code AS 'Branch',DATE_FORMAT(pay_report_gst.invoice_date,'%b-%Y') AS 'MONTH', ROUND((pay_report_gst.amount), 2)     AS 'Taxable_Amount',ROUND((`cgst` + `sgst` + `igst`), 2) AS 'GST',ROUND((pay_report_gst.amount + `cgst` + `sgst` + `igst`), 2) AS 'Bill_Amount',     IFNULL(ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`) - pay_report_gst.tds_amount),2), 0) AS 'Receivable_Amount',pay_report_gst.tds_amount, pay_report_gst.received_amt, pay_report_gst.received_amt2, pay_report_gst.received_amt3, DATE_FORMAT(pay_report_gst.received_date3, '%d/%m/%Y') AS 'Received_date3', mb3.Utr_no as Utr_no3,mb3.Amount  as 'Utr_amount3',  DATE_FORMAT(pay_report_gst.received_date2, '%d/%m/%Y') AS 'Received_date2',pay_minibank_master.Amount as UTR_amt1,  mb2.Utr_no as Utr_no2,mb2.Amount  as 'Utr_amount2',       ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`)-pay_report_gst.`tds_amount`) - IFNULL(ROUND((`pay_report_gst`.`received_amt` + `pay_report_gst`.`received_amt2`+ `pay_report_gst`.`received_amt3`),2),0)-IFNULL(`pay_report_gst`.`deduction_amt`,0), 2)      AS 'Balanced_Amount',    pay_report_gst.payment,DATE_FORMAT(pay_report_gst.received_date, '%d/%m/%Y') AS 'Received_date',CONCAT(pay_report_gst.uploaded_by, '-', pay_employee_master.EMP_NAME)      AS Entry_by_user1, DATE_FORMAT(pay_report_gst.uploaded_date, '%d/%m/%Y %H:%i:%s') AS entry_By_date1,  CONCAT(pay_report_gst.uploaded_by2, '-', e2.EMP_NAME) AS Entry_by_user2,      DATE_FORMAT(pay_report_gst.uploaded_date2, '%d/%m/%Y %H:%i:%s') AS entry_By_date2,pay_report_gst.payment_status as payment_status,datediff(now(),invoice_date) as invoice_days   FROM pay_report_gst     LEFT JOIN pay_employee_master ON pay_report_gst.uploaded_by = pay_employee_master.emp_code          LEFT JOIN pay_employee_master e2 ON pay_report_gst.uploaded_by2 = e2.emp_code    LEFT JOIN pay_minibank_master ON pay_report_gst.payment_id = pay_minibank_master.id AND pay_report_gst.comp_code = pay_minibank_master.comp_code  LEFT JOIN pay_minibank_master mb2 ON pay_report_gst.payment_id2 = mb2.id and pay_report_gst.comp_code = mb2.comp_code    LEFT JOIN pay_minibank_master mb3 ON pay_report_gst.payment_id3 = mb3.id and pay_report_gst.comp_code = mb3.comp_code    WHERE pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "' " + where_client + "   AND pay_report_gst.flag_invoice = 2  GROUP BY pay_report_gst.Invoice_No ,pay_report_gst.client_code ORDER BY Id) as t1  " + gv_where + "", d.con1);
            }
            adp2.SelectCommand.CommandTimeout = 200;
            adp2.Fill(ds1);
            if (ds1.Tables[0].Rows.Count > 0)
            {
                gv_payment.DataSource = ds1.Tables[0];
                gv_payment.DataBind();
            }
            else
            {

                // ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('No Matching Records Found !!!')", true);
                gv_payment.DataSource = null;
                gv_payment.DataBind();
            }
            d.con1.Close();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con1.Close();
        }
    }

    protected void lnkDelete_Command(object sender, CommandEventArgs e)
    {
        int result = 0;
        string sql = "";
        try
        {
            string row_id = e.CommandArgument.ToString();
            sql = "SELECT comp_code, Invoice_No, CLIENT_CODE, state_name, unit_code, billing_amt, received_amt, received_date, tds, tds_on, tds_amount, adjustment_sign, adjustment_amt, total_received_amt, month, year, '" + Session["LOGIN_ID"].ToString() + "', now(), payment_id FROM pay_report_gst WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND Id = '" + row_id + "'";

            result = d.operation("insert into payment_delete_history_details (comp_code, Invoice_No, CLIENT_CODE, state, unit_code, billing_amt, received_amt, received_date, tds, tds_on, tds_amount, adjustment_sign, adjustment_amt, total_received_amt, month, year,deleted_by,deleted_date, payment_id ) " + sql);
            if (result > 0)
            {
                result = d.operation("UPDATE pay_report_gst SET billing_amt = 0, received_amt = 0, tds_amount = 0, adjustment_amt = 0, `receipt_de_reasons`='',`receipt_de_approve`='0' ,adjustment_sign = 0, received_date = NULL, total_received_amt = 0, payment_id = 0, uploaded_by = NULL, uploaded_date = NULL WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND Id = '" + row_id + "'");
                d.operation("delete  from pay_report_gst WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND Id = '" + row_id + "' and amount = 0 and (igst= 0 || cgst=0 || igst= 0 )");

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Deleted Succsefully !!!')", true);
                payment_details(d.getsinglestring("select Invoice_No  FROM pay_report_gst WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND Id = '" + row_id + "'"));
                load_gv_payment("");
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Deletion Failed !!!')", true);

            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {

        }
    }
    protected void seles()
    {
        DataSet ds1 = new DataSet();
        MySqlDataAdapter adp1 = new MySqlDataAdapter("SELECT pay_minibank_master.ID,(SELECT COMPANY_NAME FROM pay_company_master WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "') AS 'COMPANY NAME', IFNULL((SELECT client_name FROM pay_client_master WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND client_code = pay_minibank_master.client_code), (SELECT client_name FROM pay_other_client_master WHERE client_code = pay_minibank_master.client_code)) AS 'CLIENT',pay_minibank_master.description as 'Payment Description', pay_minibank_master.Amount as 'Received Amount', DATE_FORMAT(receive_date, '%D - %M - %Y') AS 'Received Date', ROUND(IFNULL(SUM(pay_report_gst.received_amt), 0), 2) AS ' SETTLED AMOUNT', Round(pay_minibank_master.Amount - (IFNULL(SUM(pay_report_gst.received_amt), 0)) ,2) as 'REMANING AMOUNT',`Bank_name` as 'Debit' ,pay_minibank_master.Amount as 'Debit Amount',IFNULL((SELECT `client_name` FROM `pay_client_master` WHERE `comp_code` = 'C01' AND `client_code` = `pay_minibank_master`.`client_code`), (SELECT `client_name` FROM `pay_other_client_master` WHERE `client_code` = `pay_minibank_master`.`client_code`)) as 'Credit ' ,  pay_minibank_master.Amount as 'Credit Amount' FROM pay_minibank_master LEFT JOIN pay_report_gst ON pay_report_gst.payment_id = pay_minibank_master.id WHERE pay_minibank_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' GROUP BY pay_minibank_master.id ", d.con1);
        adp1.Fill(ds1);
        grv_saleentery.DataSource = ds1.Tables[0];
        grv_saleentery.DataBind();
        d.con1.Close();

    }
    //MiniBank Receipt
    public void comp_data()
    {
        try
        {
           // txt_comp_name.Text = d.getsinglestring("Select Company_name from pay_company_master where comp_code= '" + Session["COMP_CODE"].ToString() + "' ");

            //DataSet ds1 = new DataSet();
            //string client_code = "";
            //if (ddl_minibank_client.SelectedIndex > 0)
            //{
            //   // client_code = "  and pay_minibank_master.client_code='" + ddl_minibank_client.SelectedValue + "' ";
            //    client_code = "  and pay_minibank_master.client_name='" + ddl_minibank_client.SelectedItem.Text + "' ";

            //}
            //MySqlDataAdapter adp1 = new MySqlDataAdapter("SELECT pay_minibank_master.ID,receipt_approve, case when receipt_approve = '0' then 'Pending' when receipt_approve ='1' then 'Approve By Jr Acc' when receipt_approve ='2' then 'Approve By Sr Acc' when receipt_approve = '3' then 'Rejected By Sr Acc' end as 'Status', (SELECT COMPANY_NAME FROM pay_company_master WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "') AS 'COMPANY NAME',pay_minibank_master. client_name as 'client Name/Other',description as 'Payment Description', DATE_FORMAT(receive_date, '%D - %M - %Y') AS 'Received Date',DATE_FORMAT(payment_hit_date,'%d/%m/%Y') as 'Payment Hit Date',  pay_minibank_master.Amount as 'Credit Amount', Mode_of_transfer as 'Mode of Transfer',Utr_no as 'UTR_NO',Cheque as 'Cheque NO',Upload_file ,ROUND(IFNULL(SUM(`payment_history_details`.`received_amt`), 0), 2) AS ' SETTLED AMOUNT',  ROUND(pay_minibank_master.Amount - (IFNULL(SUM(`payment_history_details`.`received_amt`), 0)), 2) AS 'REMANING AMOUNT',`receipt_reasons` as 'Rejected Reason'  , CONCAT(pay_minibank_master.uploaded_by,'-',pay_employee_master.EMP_NAME) as Entry_by_user, Date_Format(pay_minibank_master.uploaded_date,'%d/%m/%Y %H:%m:%s') as entry_By_date,remark   FROM pay_minibank_master LEFT JOIN payment_history_details ON payment_history_details.payment_id = pay_minibank_master.id    left join pay_employee_master on pay_minibank_master.uploaded_by=pay_employee_master.emp_code  WHERE pay_minibank_master.comp_code = '" + Session["COMP_CODE"].ToString() + "'  " + client_code + "  GROUP BY pay_minibank_master.id ", d.con1);
            //  adp1.Fill(ds1);
            //gv_minibank.DataSource = ds1.Tables[0];
            //gv_minibank.DataBind();
            //d.con1.Close();

        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }
    }
    protected void lbtn_addutr_Click(object sender, EventArgs e)
    {
    
        pnl_bank_details.Visible = true;

        //Sachin changes for comma replace in amount txt box  (28-06-2022)
        txt_minibank_amount.Text = txt_minibank_amount.Text.Replace(",", "");
        //END

        try
        {
            string utrno_allready = "";
            if (lit_bank_name.Text == "")
            {

                get_bank_details();

                if (lit_bank_name.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Bank Details Not Found, Please add bank detail for this client!!!')", true);
                    return;
                }
            }

            if (txt_minibank_utr_no.Text.Trim() == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter UTR Number')", true);
                return;
            }
            else
            {
                utrno_allready = d.getsinglestring("select utr_no from pay_minibank_master where  utr_no='" + txt_minibank_utr_no.Text.Trim() + "'");
                if (utrno_allready != "")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Same UTR Number is  Already in used')", true);
                    return;
                }
            }
            if (txt_minibank_received_date.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Receipt Date')", true);
                return;
            }
            if (txt_minibank_amount.Text == "" || txt_minibank_amount.Text == "0")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter receipt amount')", true);
                return;
            }
            if (ddl_mode_transfer.SelectedValue == "Select")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Select Mode Of Transfer')", true);
                return;
            }
            if (ddl_payment_type.SelectedValue == "Select")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Select Payment Against')", true);
                return;
            }



            if (txt_minibank_received_date.Text != "" && txt_minibank_amount.Text != "0" && txt_minibank_amount.Text != "" && ddl_mode_transfer.SelectedValue != "Select" && txt_minibank_utr_no.Text.Trim() != "" && ddl_payment_type.SelectedValue!="Select")
            {
                gv_add_utr.Visible = true;
                Button1.Visible = true;
                btn_approve_minibank.Visible = false;
                DataTable dt = new DataTable();
                DataRow dr;
                dt.Columns.Add("Receipt_Date");
                dt.Columns.Add("Amount");
                dt.Columns.Add("UTR_no");
                dt.Columns.Add("Payment_mode");
                dt.Columns.Add("Payment_against");
                dt.Columns.Add("Remark");

                foreach (GridViewRow row in gv_add_utr.Rows)
                {
                   
                    for ( int i = 0; i < gv_add_utr.Rows.Count; i++)
                    {
                         string utr_no_chk =  gv_add_utr.Rows[i].Cells[4].Text;

                         if (txt_minibank_utr_no.Text.Trim().Equals(utr_no_chk))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Same UTR Allready added, you canot add one more.!!')", true);
                        return;
                    }
                    }
                   
                }




                int rownum = 0;
                for (rownum = 0; rownum < gv_add_utr.Rows.Count; rownum++)
                {
                    if (gv_add_utr.Rows[rownum].RowType == DataControlRowType.DataRow)
                    {
                        dr = dt.NewRow();
                        dr["Receipt_Date"] = gv_add_utr.Rows[rownum].Cells[2].Text;
                        dr["Amount"] = gv_add_utr.Rows[rownum].Cells[3].Text;
                        dr["UTR_no"] = gv_add_utr.Rows[rownum].Cells[4].Text;
                        dr["Payment_mode"] = gv_add_utr.Rows[rownum].Cells[5].Text;
                        dr["Payment_against"] = gv_add_utr.Rows[rownum].Cells[6].Text;
                        dr["Remark"] = gv_add_utr.Rows[rownum].Cells[7].Text;
                        dt.Rows.Add(dr);
                    }
                }



                // Receipt_Date,Amount,UTR_no,Payment_mode,Payment_against,Remark
                dr = dt.NewRow();

                dr["Receipt_Date"] = txt_minibank_received_date.Text;
                dr["Amount"] = txt_minibank_amount.Text;
                dr["UTR_no"] = txt_minibank_utr_no.Text.Trim();
                dr["Payment_mode"] = ddl_mode_transfer.SelectedValue;
                dr["Payment_against"] = ddl_payment_type.SelectedItem.Text;
                dr["Remark"] = txt_utr_remark.Text;
                dt.Rows.Add(dr);
                gv_add_utr.DataSource = dt;
                gv_add_utr.DataBind();
                ViewState["CurrentTable"] = dt;

                btn_row.Visible = true;
                txt_minibank_received_date.Text = "";
                txt_minibank_amount.Text = "0";
                txt_utr_remark.Text = "";
                ddl_mode_transfer.SelectedIndex = 0;
                ddl_payment_type.SelectedIndex = 0;
                txt_minibank_utr_no.Text = "";
            }

        }
        catch { }
    }

    protected void gv_add_utr_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
            if (e.Row.Cells[i].Text == "&amp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
    }
    protected void gv_add_utr_PreRender(object sender, EventArgs e)
    {
        try
        {
            // grid_esic.UseAccessibleHeader = false;
            gv_add_utr.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }
    protected void linkbtn_removeitem_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        try
        {
            int rowID = ((GridViewRow)((LinkButton)sender).NamingContainer).RowIndex;
            if (ViewState["CurrentTable"] != null)
            {
                System.Data.DataTable dt = (System.Data.DataTable)ViewState["CurrentTable"];
                if (dt.Rows.Count >= 1)
                {
                    if (rowID < dt.Rows.Count)
                    {
                        dt.Rows.Remove(dt.Rows[rowID]);
                    }
                }
                ViewState["CurrentTable"] = dt;
                gv_add_utr.DataSource = dt;
                gv_add_utr.DataBind();
            }
        }
        catch { }
    }
    protected void btn_minibank_submit_Click(object sender, EventArgs e)
    {
        try
        {
            string record_save = null;
            string utrno_allready = null;
            string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");
            #region
            //if (txt_minibank_utr_no.Text != "")
            //{
                //utrno_allready = d.getsinglestring("select utr_no from pay_minibank_master where  utr_no='" + txt_minibank_utr_no.Text + "'");
                //if (utrno_allready != "")
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Same UTR Number is  Already in used')", true);
                //    return;
                //}
                //else
                //{
                    //if (ddl_pmt_recived.SelectedValue == "0")
                    //{ record_save = d.getsinglestring("select client_code,`Bank_name`,`Account_number`,`Amount`,client_bank_name,client_ac_number,Mode_of_transfer,Utr_no,uploaded_by,payment_type from pay_minibank_master where comp_code = '" + Session["comp_code"].ToString() + "' and `Utr_no`='" + txt_minibank_utr_no.Text + "' and `received_from` = '0' ");
                    // if (record_save != "")
                    //    {ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('This Record Already Added')", true);
                    //        return; }
                    //}
                    //else
                    //    if (ddl_pmt_recived.SelectedValue == "1")
                    //    {  record_save = d.getsinglestring("select `Account_number`,`Amount`,`receive_date`,`description`,`uploaded_by`,client_name,Mode_of_transfer  from pay_minibank_master where comp_code = '" + Session["comp_code"].ToString() + "'  and `Utr_no`='" + txt_minibank_utr_no.Text + "' and`received_from` = '1'  ");
                    //        if (record_save != "")
                    //        {
                    //            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('This Record Already Added')", true);
                    //            return;
                    //        }
                    //    }

            //  ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            #endregion
            int res = 0;
            if (photo_upload.HasFile)
            {
                 string fileExt = System.IO.Path.GetExtension(photo_upload.FileName);
                 if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".RAR" || fileExt.ToUpper() == ".ZIP" || fileExt.ToUpper() == ".XLSX" || fileExt.ToUpper() == ".XLS" || fileExt.ToUpper() == ".DOCX" || fileExt.ToUpper() == ".DOC")
                 {


                    if (ddl_pmt_recived.SelectedValue == "1")
                    {
                        foreach (GridViewRow row in gv_add_utr.Rows)
                        {
                            int sr_number = int.Parse(((Label)row.FindControl("lbl_srnumber")).Text);
                            string receipt_date_gv = row.Cells[2].Text;
                            string amount_gv = row.Cells[3].Text;
                            string utr_no_gv = row.Cells[4].Text;
                            string payment_mode_gv = row.Cells[5].Text;
                            string payment_against_gv = row.Cells[6].Text;
                            string remark_gv = row.Cells[7].Text;
                            string cheque_no = "";
                            if (payment_mode_gv=="Cheque")
                            {
                                cheque_no = utr_no_gv;
                                utr_no_gv = "";
                            }
                            utrno_allready = d.getsinglestring("select utr_no from pay_minibank_master where  utr_no='" + utr_no_gv + "'");
                            if (utrno_allready != "")
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('UTR Number " + utr_no_gv + " is  Already in used')", true);
                                return;
                            }
                            //if (txt_description.Text == "") { ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Enter description to Payment Related !!!')", true); txt_description.Focus(); return; }

                           
                                string fileName = Path.GetFileName(photo_upload.PostedFile.FileName);
                                photo_upload.PostedFile.SaveAs(Server.MapPath("~/Account_images/") + fileName);
                                string id = d.getsinglestring("select (max(id)+1) from pay_minibank_master ");
                                string file_name = client_code + id + fileExt;
                                File.Copy(Server.MapPath("~/Account_images/") + fileName, Server.MapPath("~/Account_images/") + file_name, true);
                                File.Delete(Server.MapPath("~/Account_images/") + fileName);
                             // Upload_file='" + file_name + "'
                              string str_chk = d.getsinglestring("select utr_no from pay_minibank_master where  utr_no='" + utr_no_gv + "'");
                                if (str_chk == "")
                                {
                                res = d.operation("Insert Into pay_minibank_master(comp_code,client_name,Bank_name,Account_number,receive_date,description,Amount,uploaded_by,uploaded_date,Mode_of_transfer,Cheque,Utr_no,received_from,payment_hit_date,remark,Upload_file) values ('" + Session["COMP_CODE"].ToString() + "','" + ddl_other.SelectedValue + "','" + lit_bank_name.Text + "','" + lit_comp_ac_number.Text + "',str_to_date('" + receipt_date_gv + "','%d/%m/%Y'),'" + payment_against_gv + "','" + amount_gv + "','" + Session["LOGIN_ID"].ToString() + "', now(),'" + payment_mode_gv + "','" + cheque_no + "','" + utr_no_gv + "','" + ddl_pmt_recived.SelectedValue + "',str_to_date('" + receipt_date_gv + "','%d/%m/%Y'),'" + remark_gv + "','" + file_name + "'");
                           // string id = d.getsinglestring("select max(id) from pay_minibank_master ");
                           // upload_file(id);
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('UTR Number " + utr_no_gv + " is  Already in used')", true);
                                    return;
                                }
                           
                        }
                    }
                    else if (ddl_pmt_recived.SelectedValue == "0")
                    {

                        //if (ddl_payment_type.SelectedValue == "Select") { ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Select Payment Against  !!!')", true); ddl_payment_type.Focus(); return; }
                        foreach (GridViewRow row in gv_add_utr.Rows)
                        {
                            int sr_number = int.Parse(((Label)row.FindControl("lbl_srnumber")).Text);
                            string receipt_date_gv = row.Cells[2].Text;
                            string amount_gv = row.Cells[3].Text;
                            string utr_no_gv = row.Cells[4].Text;
                            string payment_mode_gv = row.Cells[5].Text;
                            string payment_against_gv = row.Cells[6].Text;
                            string remark_gv = row.Cells[7].Text;
                            string cheque_no = "";
                            if (payment_mode_gv == "Cheque")
                            {
                                cheque_no = utr_no_gv;
                                utr_no_gv = "";
                            }
                            string payment_type="0";
                            if (payment_against_gv=="Invoice Against") {payment_type="1";} else if (payment_against_gv=="Advance Payment") {payment_type="2";} else if (payment_against_gv=="Payment + Advance") {payment_type="3";} else if (payment_against_gv=="Payment - Advance") {payment_type="4";} else if (payment_against_gv=="- Advance") {payment_type="5";}

                           
                                string fileName = Path.GetFileName(photo_upload.PostedFile.FileName);
                                photo_upload.PostedFile.SaveAs(Server.MapPath("~/Account_images/") + fileName);
                                string id = d.getsinglestring("select (max(id)+1) from pay_minibank_master ");
                                string file_name = client_code + id + fileExt;
                                File.Copy(Server.MapPath("~/Account_images/") + fileName, Server.MapPath("~/Account_images/") + file_name, true);
                                File.Delete(Server.MapPath("~/Account_images/") + fileName);

                                string str_chk = d.getsinglestring("select utr_no from pay_minibank_master where  utr_no='" + utr_no_gv + "'");
                                if (str_chk == "")
                                {
                                    res = d.operation("Insert Into pay_minibank_master(comp_code,client_name,Bank_name,Account_number,client_bank_name,client_ac_number,receive_date,payment_type,description,Amount,uploaded_by,uploaded_date,Mode_of_transfer,Cheque,Utr_no,client_code,received_from,payment_hit_date,remark,Upload_file) values ('" + Session["COMP_CODE"].ToString() + "','" + ddl_minibank_client.SelectedItem.Text + "','" + lit_bank_name.Text + "','" + lit_comp_ac_number.Text + "','" + lit_client_bank.Text + "','" + lit_client_ac_number.Text + "',str_to_date('" + receipt_date_gv + "','%d/%m/%Y'),'" + payment_type + "','" + payment_against_gv + "','" + amount_gv + "','" + Session["LOGIN_ID"].ToString() + "', now(),'" + payment_mode_gv + "','" + cheque_no + "','" + utr_no_gv + "','" + client_code + "','" + ddl_pmt_recived.SelectedValue + "',str_to_date('" + receipt_date_gv + "','%d/%m/%Y'),'" + remark_gv + "','" + file_name + "')");
                                    //string id = d.getsinglestring("select max(id) from pay_minibank_master ");
                                    //upload_file(id);
                                }
                                else
                                {
                                    Button1.Visible = false;
                                    btn_approve_minibank.Visible = true;
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('UTR Number " + utr_no_gv + " is  Already in used')", true);
                                    return;
                                }

                               
                          
                        }
                    }
           
                    if (res > 0)
                    {

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Transaction Saved Successfully Approve This Record !!!')", true);
                        //mini_text_clear();

                        Button1.Visible = false;
                        btn_approve_minibank.Visible = true;
                      //  for_client.Visible = false;
                      //  for_other.Visible = false;
                        //  ddl_bank_name.Items.Clear();
                        //  ddl_other_bank.Items.Clear();
                    }
                    else { ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Transaction Failed !!!')", true); }
                 }
                 else
                 {
                     ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select only JPG, PNG , XLSX, XLS and PDF  Files  !!');", true);
                     return;
                 }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please attach payment advice copy.!!!')", true); 
            }
        }
        catch (Exception ex)
        {

            throw ex;

        }
        finally
        {
          //  comp_data();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);

        }
    }

    protected void gv_minibank_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (DataRowView)e.Row.DataItem;
            if (dr["receipt_approve"].ToString() != "0" && dr["receipt_approve"].ToString() != "3")
            {
                //LinkButton lb1 = e.Row.FindControl("unit_name") as LinkButton;
                //lb1.Visible = false;


                //  e.Row.Cells[14].Visible = false;
                // e.Row.Cells[15].Visible = false;

                LinkButton lb1 = e.Row.FindControl("LinkButton2") as LinkButton;
                lb1.Visible = false;

                LinkButton lb2 = e.Row.FindControl("btn_edit_other1") as LinkButton;
                lb2.Visible = false;


            }
            else
            {
                LinkButton lb2 = e.Row.FindControl("btn_edit_other1") as LinkButton;
                lb2.Visible = true;
            }
        }




        e.Row.Cells[3].Visible = false;
        e.Row.Cells[4].Visible = false;
        e.Row.Cells[5].Visible = false;
        e.Row.Cells[6].Visible = false;
        e.Row.Cells[7].Visible = false;
       
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
            //e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            //e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.gv_minibank, "Select$" + e.Row.RowIndex);

        }

    }
    protected void gv_minibank_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    //protected void ddl_bank_name_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    ddl_comp_ac_number.Items.Clear();
    //    System.Data.DataTable dt_item = new System.Data.DataTable();
    //    MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select Field2 from pay_zone_master where comp_code='" + Session["comp_code"].ToString() + "' and Type = 'bank_details' and Field1 = '" + ddl_bank_name.SelectedValue + "'", d.con);
    //    d.con.Open();
    //    try
    //    {
    //        cmd_item.Fill(dt_item);
    //        if (dt_item.Rows.Count > 0)
    //        {
    //            ddl_comp_ac_number.DataSource = dt_item;
    //            ddl_comp_ac_number.DataTextField = dt_item.Columns[0].ToString();
    //            ddl_comp_ac_number.DataValueField = dt_item.Columns[0].ToString();
    //            ddl_comp_ac_number.DataBind();

    //        }
    //        //ddl_comp_ac_number.Items.Insert(0, "Select");
    //        dt_item.Dispose();
    //    }
    //    catch (Exception ex) { }
    //    finally { d.con.Close(); }
    //}
    // minibank client


    //protected void ddl_minibank_client_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    get_bank_details();

    //}

    private void get_bank_details()
    {
        try
        {
            hidtab.Value = "0";
            string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");
            try
            {
                d4.con.Open();
                MySqlCommand cmd = null;
                if (ddl_pmt_recived.SelectedValue == "0")
                {
                  //  str_to_date('" + txt_cheque_receive_date.Text + "','%d/%m/%Y')
                  //  cmd = new MySqlCommand("Select comp_bank_name,comp_acc_no  from pay_client_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code = '" + client_code + "'  limit 1", d4.con);
                    cmd = new MySqlCommand("select payment_ag_bank,company_ac_no from pay_company_bank_details where comp_code='" + Session["comp_code"].ToString() + "' and client_code = '" + client_code + "' and bank_period_from<=str_to_date('" + txt_minibank_received_date.Text + "','%d/%m/%Y') and bank_period_to>=str_to_date('" + txt_minibank_received_date.Text + "','%d/%m/%Y') limit 1", d4.con);

                }
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    //ddl_bank_name.Text = dr.GetValue(0).ToString();
                    lit_bank_name.Text = dr.GetValue(0).ToString();
                    lit_comp_ac_number.Text = dr.GetValue(1).ToString();
                }
                d4.con.Close();
            }
            catch
            { d4.con.Close(); }
            string client_bank = "", client_ac_no = "";
            if (ddl_pmt_recived.SelectedValue == "1")
            {
                client_bank = d.getsinglestring("select client_bank_name from pay_other_client_master where client_code='" + client_code + "' limit 1");
                client_ac_no = d.getsinglestring("select client_ac_no from pay_other_client_master where client_code='" + client_code + "' and client_bank_name = '" + client_bank + "'  limit 1");
            }
            else
            {
                client_bank = d.getsinglestring("Select Field1 from pay_zone_master where comp_code='" + Session["comp_code"].ToString() + "' and Type = 'bank_details' and CLIENT_CODE ='" + client_code + "' limit 1");
                client_ac_no = d.getsinglestring("Select Field2 from pay_zone_master where comp_code='" + Session["comp_code"].ToString() + "' and Type = 'bank_details' and Field1 = '" + client_bank + "' and client_code = '" + client_code + "'  limit 1");
            }
            lit_client_bank.Text = client_bank.ToString();
            lit_client_ac_number.Text = client_ac_no.ToString();
        }
        catch { }
    }

   

  
    //private void client_bank_ac_no()
    //{
    //    string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");

    //    System.Data.DataTable dt_item = new System.Data.DataTable();
    //    MySqlDataAdapter cmd_item = null;
    //    if (ddl_pmt_recived.SelectedValue == "1")
    //    {
    //        cmd_item = new MySqlDataAdapter("select client_ac_no from pay_other_client_master where client_code='" + client_code + "' and client_bank_name = '" + ddl_client_bank.SelectedValue + "' ", d.con);
    //    }
    //    else
    //    {
    //        cmd_item = new MySqlDataAdapter("Select Field2 from pay_zone_master where comp_code='" + Session["comp_code"].ToString() + "' and Type = 'bank_details' and Field1 = '" + ddl_client_bank.SelectedValue + "' and client_code = '" + client_code + "'", d.con);

    //    }
    //    d.con.Open();
    //    try
    //    {
    //        // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
    //        cmd_item.Fill(dt_item);
    //        if (dt_item.Rows.Count > 0)
    //        {
    //            ddl_client_ac_number.DataSource = dt_item;
    //            ddl_client_ac_number.DataTextField = dt_item.Columns[0].ToString();
    //            ddl_client_ac_number.DataValueField = dt_item.Columns[0].ToString();
    //            ddl_client_ac_number.DataBind();

    //        }
    //        //ddl_client_ac_number.Items.Insert(0, "Select");
    //        dt_item.Dispose();
    //        /// bank_name_ac_no();
    //    }
    //    catch (Exception ex) { throw ex; }
    //    finally
    //    {
    //        d.con.Close();
    //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);

    //    }
    //}
    //protected void bank_name_ac_no()
    //{
    //    try
    //    {

          
           
    //        ddl_bank_name.DataSource = null;
    //        string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");

    //        //d.con.Open();
    //        //ddl_bank_name.SelectedValue = "";
    //        ddl_comp_ac_number.Text = "";

    //        d4.con.Open();
    //        MySqlCommand cmd = null;
    //        if (ddl_pmt_recived.SelectedValue == "0")
    //        {
    //            cmd = new MySqlCommand("Select comp_bank_name,comp_acc_no  from pay_client_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code = '" + client_code + "'", d4.con);
    //            //cmd = new MySqlCommand("Select comp_bank_name,comp_ac_no  from pay_other_client_master where  client_code = '" + ddl_minibank_client.SelectedValue + "'", d.con);
    //        }
    //        //else
    //        //{
    //        //    cmd = new MySqlCommand("Select comp_bank_name,comp_acc_no  from pay_client_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_minibank_client.SelectedValue + "'", d.con);
    //        //}
    //        MySqlDataReader dr = cmd.ExecuteReader();
    //        if (dr.Read())
    //        {
    //            //ddl_bank_name.Text = dr.GetValue(0).ToString();
    //            ddl_bank_name.SelectedValue = dr.GetValue(0).ToString();
    //            ddl_comp_ac_number.Text = dr.GetValue(1).ToString();
    //        }

    //    }
    //    catch (Exception ex) { throw ex; }
    //    finally
    //    { //d.con.Close(); 
    //        d4.con.Close();

    //    }

    //}
   


    protected void gv_minibank_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_minibank.UseAccessibleHeader = false;
            gv_minibank.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }

    protected void gv_minibank_menu4(object sender, EventArgs e)
    {
        try
        {
            grv_saleentery.UseAccessibleHeader = false;
            grv_saleentery.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }

    protected void lnkminiDelete_Command(object sender, CommandEventArgs e)
    {
        int result = 0;
        string sql = "";
        try
        {
            string row_id = e.CommandArgument.ToString();
            sql = "SELECT comp_code,client_code,Bank_name,Account_number,Account_balance,client_bank_name,client_ac_number,month,year,receive_date,payment_type,Amount,description,'" + Session["LOGIN_ID"].ToString() + "', now() FROM pay_minibank_master WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND Id = '" + row_id + "'";
            try
            {
            string filename = d.getsinglestring("select upload_file FROM pay_minibank_master WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND Id = '" + row_id + "' ");
            File.Delete(Server.MapPath("~/Account_images/") + filename);
            }
            catch  { }
            d.operation("delete FROM pay_minibank_master WHERE  Id = '" + row_id + "'");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Deleted Succsefully !!!')", true);
          //  comp_data();
            result = d.operation("insert into pay_delete_minibank_history (comp_code,client_code,Bank_name,Account_number,Account_balance,client_bank_name,client_ac_number,month,year,receive_date,payment_type,Amount,description,deleted_by,deleted_date ) " + sql);
            if (result > 0)
            {
                View_utr_detail_grid();
                // d.operation("delete FROM pay_minibank_master WHERE  Id = '" + row_id + "'");

                // ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Deleted Succsefully !!!')", true);
                // comp_data();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Deletion Failed !!!')", true);

            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {

        }
    }

    protected void gv_payment_detail_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_payment_detail.UseAccessibleHeader = false;
            gv_payment_detail.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
    protected void mini_text_clear()
    {
        //    ddl_minibank_client.SelectedValue = "Select";
        //ddl_bank_name.SelectedValue = "";
        //ddl_comp_ac_number.Text = "";
        //ddl_client_bank.Items.Clear();
        //ddl_client_ac_number.Items.Clear();
        ddl_mode_transfer.SelectedValue = "Select";
        txt_minibank_received_date.Text = "";
        txt_description.Text = "";

        txt_minibank_amount.Text = "";
        txt_cheque.Text = "";
        txt_minibank_utr_no.Text = "";
        //ddl_other.SelectedValue = "Select";
        // ddl_other_bank.SelectedValue=""
        txt_payment_hit_date.Text = "";

    }

    protected void ddl_pmt_recived_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
           mini_text_clear();
            client_bank.Visible = true;
            client_ac_no.Visible = true;
            bank_name.Visible = true;
            desc.Visible = false;
            if (ddl_pmt_recived.SelectedValue == "1")
            {
             //   ddl_bank_name.SelectedValue = "";
                lit_comp_ac_number.Text = "";
                for_client.Visible = false;
                bank_name.Visible = false;
                for_other.Visible = true;
                for_other1.Visible = true;
                client_bank.Visible = false;
                client_ac_no.Visible = false;
                other_client_code();
               // btn_add_others.Visible = true;
                ddl_payment_type.Visible = false;
                lbl_payment_type.Visible = false;
                txt_description.Visible = true;
                Panel1.Visible = false;
                pnl_desc.Visible = true;
                desc.Visible = true;
            }
            else
            {
              //  ddl_bank_name.SelectedValue = "";
                lit_comp_ac_number.Text = "";
                bank_name.Visible = true;
                for_client.Visible = true;
                for_other.Visible = false;
                for_other1.Visible = false;
                client_bank.Visible = true;
                client_ac_no.Visible = true;
                client_code();
                btn_add_others.Visible = false;
                ddl_payment_type.Visible = true;
                lbl_payment_type.Visible = true;
                txt_description.Visible = false;
                Panel1.Visible = true;
                pnl_desc.Visible = false;
                desc.Visible = false;
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        { //mini_text_clear(); 
        }
    }

    //protected void btn_add_others_Click(object sender, EventArgs e)
    //{

    //    try
    //    {
    //        if (ddl_pmt_recived.SelectedValue == "1")
    //        {
    //            string client_code = create_client_code();

    //            d.operation("insert into pay_other_client_master(client_code,client_name,created_by,created_date)values('"+client_code+"','"+txt_client_name.Text+"','"+Session["LOGIN_ID"].ToString()+"',now()) ");
    //            other_client_code();
    //            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Added successfully !!!')", true);
    //        }

    //    }
    //    catch (Exception ex) { throw ex; }
    //    finally { }
    //}

    protected string create_client_code()
    {
        string client_code_series = "C";
        try
        {
            string other_client = d.getsinglestring("select Max(substring(client_code,2)+1) from pay_other_client_master");
            if (other_client == "")
            {
                client_code_series = client_code_series + "001";
            }

            else
            {
                int number = int.Parse(other_client);

                if (number < 10)
                {
                    client_code_series = client_code_series + "00" + other_client;
                }
                else if (number > 9 && number < 100)
                {
                    client_code_series = client_code_series + "0" + other_client;
                }
                else if (number > 99)
                {
                    client_code_series = client_code_series + other_client;
                }
            }
            return client_code_series;
        }
        catch (Exception ex) { throw ex; }
        finally { }
    }

    protected void other_client_code()
    {

        ddl_minibank_client.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select Field1 from pay_zone_master where comp_code='" + Session["COMP_CODE"].ToString() + "' and type='minibank' ORDER BY Field1", d.con);
        //MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select client_code,client_name from pay_other_client_master  ORDER BY client_name", d.con);
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {

                ddl_other.DataSource = dt_item;

                ddl_other.DataValueField = dt_item.Columns[0].ToString();
                //ddl_minibank_client.DataTextField = dt_item.Columns[1].ToString();
                ddl_other.DataBind();


            }
            dt_item.Dispose();
            // hide_controls();
            d.con.Close();

            ddl_other.Items.Insert(0, "Select");

        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
        //for bank name
        System.Data.DataTable dt_item1 = new System.Data.DataTable();
        MySqlDataAdapter cmd_item1 = new MySqlDataAdapter("Select Field1 from pay_zone_master where comp_code='" + Session["COMP_CODE"].ToString() + "' and type='bank_details'  and client_code is null ORDER BY Field1", d.con);
        //MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select client_code,client_name from pay_other_client_master  ORDER BY client_name", d.con);
        d.con.Open();
        try
        {
            cmd_item1.Fill(dt_item1);
            if (dt_item1.Rows.Count > 0)
            {

                ddl_other_bank.DataSource = dt_item1;

                ddl_other_bank.DataValueField = dt_item1.Columns[0].ToString();
                //ddl_minibank_client.DataTextField = dt_item.Columns[1].ToString();
                ddl_other_bank.DataBind();
            }
            dt_item1.Dispose();
            // hide_controls();
            d.con.Close();

           // ddl_other_bank.Items.Insert(0, "Select");

        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
        //Bank_name();
    }

    protected bool check_validation(string process)
    {

        double total_amount = 0;
        double received_amt = 0;
        double update_total_amount = 0;
       
        foreach (GridViewRow row in gv_invoice_pmt.Rows)
        {
            if (row != null)
            {

                result = 0;
                string invoice_no = row.Cells[1].Text;

                double txt_bill_amount = double.Parse(row.Cells[2].Text);

                double txt_receive_amount = double.Parse(((TextBox)row.FindControl("txt_recive_amt")).Text);

                string txt_reciving_date = ((TextBox)row.FindControl("txt_reciving_date")).Text;

                //DropDownList ddl_tds_amount = (DropDownList)row.FindControl("ddl_tds_amount");

                //DropDownList ddl_tds_on = (DropDownList)row.FindControl("ddl_tds_on");

                double txt_tds_amt = double.Parse(((TextBox)row.FindControl("txt_tds_amt")).Text);

                int adj_selectedvalue = int.Parse(((DropDownList)row.FindControl("ddl_adjustment")).SelectedValue);

                double txt_adjustment_amt = double.Parse(((TextBox)row.FindControl("txt_adjustment_amt")).Text);



                int month = int.Parse(return_fields1("month", invoice_no));
                int year = int.Parse(return_fields1("year", invoice_no));

                double tds_amount = 0;

                //check receving amount not zero
                if (txt_receive_amount.Equals(0) || txt_receive_amount.Equals(""))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Received Amount Must be Greater Than Zero!!!')", true);
                    row.FindControl("txt_recive_amt").Focus();
                    return true;
                }
                //tds amount
                //if (ddl_tds_amount.SelectedValue != "Amount")
                //{
                //    int tds_persent = int.Parse(ddl_tds_amount.SelectedValue);
                //    tds_amount = ddl_tds_on.SelectedValue == "0" ? (double.Parse(return_fields("taxable_amount", invoice_no)) * tds_persent) / 100 : (txt_bill_amount * tds_persent) / 100;
                //}
                //else
                //{
                tds_amount = txt_tds_amt;
                //}
                if (process == "Insert")
                {
                    update_total_amount = double.Parse(d.getsinglestring("select ifnull(Round(sum(total_received_amt),2), 0) from pay_report_gst where comp_code='" + Session["comp_code"].ToString() + "' and invoice_no='" + invoice_no + "' and client_code='" + ddl_client.SelectedValue + "' and id = '" + ViewState["row_id"] + "'  order by id"));
                    received_amt = double.Parse(d.getsinglestring("select ifnull(Round(sum(total_received_amt),2), 0) from pay_report_gst where comp_code='" + Session["comp_code"].ToString() + "' and invoice_no='" + invoice_no + "' and client_code='" + ddl_client.SelectedValue + "'  order by id"));
                }
                else if (process == "Update")
                {
                    received_amt = double.Parse(d.getsinglestring("select ifnull(Round(sum(total_received_amt),2), 0) from pay_report_gst where comp_code='" + Session["comp_code"].ToString() + "' and invoice_no='" + invoice_no + "' and client_code='" + ddl_client.SelectedValue + "' and id != '" + ViewState["row_id"] + "'  order by id"));
                    update_total_amount = double.Parse(d.getsinglestring("select ifnull(Round(sum(total_received_amt),2), 0) from pay_report_gst where comp_code='" + Session["comp_code"].ToString() + "' and invoice_no='" + invoice_no + "' and client_code='" + ddl_client.SelectedValue + "' and id = '" + ViewState["row_id"] + "'  order by id"));
                }
                //adjustment amount
                string adj_sign = "";

                if (adj_selectedvalue == 1)
                {

                    if ((txt_bill_amount - (received_amt + txt_adjustment_amt + txt_receive_amount + tds_amount)) != 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Your Adjustment Amount is : " + (txt_bill_amount - (received_amt + txt_adjustment_amt + txt_receive_amount + tds_amount)) + " You Can Enter Wrong Adjustment Amount !!!')", true);
                        row.FindControl("txt_adjustment_amt").Focus();
                        return true;
                    }
                    received_amt = received_amt + txt_adjustment_amt;

                }

                if (adj_selectedvalue == 2)
                {

                    if ((txt_bill_amount - ((received_amt - txt_adjustment_amt) + txt_receive_amount + tds_amount)) != 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Your Adjustment Amount is : " + (txt_bill_amount - ((received_amt - txt_adjustment_amt) + txt_receive_amount + tds_amount)) + " You Can Enter Wrong Adjustment Amount !!!')", true);
                        row.FindControl("txt_adjustment_amt").Focus();
                        return true;
                    }
                    received_amt = received_amt - txt_adjustment_amt;

                }
                string matching = String.Format("{0:0.00}", (received_amt + txt_receive_amount + tds_amount));
                Double balance_amt = Convert.ToDouble(matching);

                if (txt_bill_amount >= (balance_amt))
                {
                    try
                    {

                        total_amount = total_amount + (received_amt + txt_receive_amount + tds_amount);
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    { }

                }
                else
                {
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Received Amount Exceed Client Billing Amount!!!')", true);
                    //row.FindControl("txt_recive_amt").Focus();
                    //return true;
                }

            }
        }

        //temprory comment 29/07/2019
        if (total_amount > (double.Parse(ddl_client_resive_amt.SelectedItem.Text) + update_total_amount))
        {
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Received Amount Exceed Client Payment Amount !!!')", true);
            //ddl_client_resive_amt.Focus();
           // return true;////temprory comment 29/07/2019
            return false;
        }
        else { return false; }
    }



    //payment
    public void company_bank_load()
    {

        try
        {
            ddl_company_bank.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = null;
            if (ddl_pmt_paid.SelectedValue == "2")
            {
                cmd_item = new MySqlDataAdapter("Select Field1 , Field2 from pay_zone_master where comp_code='" + Session["comp_code"].ToString() + "' and Type = 'bank_details' and CLIENT_CODE is null", d.con);
            }
            else if (ddl_pmt_paid.SelectedValue == "3")
            {
                cmd_item = new MySqlDataAdapter("Select Field1 , Field2 from pay_zone_master where Field2 != '" + ddl_pmt_client.SelectedValue + "' and Type = 'bank_details' and CLIENT_CODE is null", d.con);
            }
            d.con.Open();



            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_company_bank.DataSource = dt_item;
                ddl_company_bank.DataTextField = dt_item.Columns[0].ToString();
                ddl_company_bank.DataValueField = dt_item.Columns[1].ToString();
                ddl_company_bank.DataBind();
                d.con.Close();
            }

            dt_item.Dispose();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            ddl_company_bank.Items.Insert(0, new ListItem("Select"));
            ddl_batch_no.Items.Clear();
            ddl_batch_no.Items.Insert(0, new ListItem("Select"));
            d.con.Close();
        }


    }

    protected void ddl_pmt_paid_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "2";
        pmt_text_clear(0);
        payment_type_selection();
        load_gv_debit_pmt_details(ddl_pmt_paid.SelectedValue);
        //try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        //catch { }
    }

    protected void ddl_company_bank_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "2";

        try
        {
            if (ddl_pmt_paid.SelectedValue == "3")
            {
                txt_pmt_desc.Text = "";

                txt_pmt_desc.Text = ddl_company_bank.SelectedValue;
                txt_pmt_desc.ReadOnly = true;
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

    protected void ddl_pmt_client_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "2";
        try
        {
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            //client payment
            if (ddl_pmt_paid.SelectedValue == "1")
            {
                DataSet ds = new DataSet();
                ddl_batch_no.Items.Clear();
                ds = d.select_data("SELECT batch_no FROM (SELECT batch_no, (amount_payable - paid_Amount) AS 'remaining_amount', amount_payable FROM (SELECT batch_no, SUM(REPLACE(amount_payable, ',', '')) AS 'amount_payable', SUM(CAST(IFNULL(Amount, 0) AS signed)) AS 'paid_Amount' FROM paypro_uploaded_data INNER JOIN pay_pro_master ON paypro_uploaded_data.batch_no = pay_pro_master.paypro_batch_id LEFT JOIN pay_debit_master ON pay_debit_master.annuxure_no = paypro_uploaded_data.batch_no WHERE pay_pro_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_pro_master.client_code = '" + ddl_pmt_client.SelectedValue + "' AND transaction_status = 'Paid' GROUP BY batch_no) AS t1) AS t2 WHERE amount_payable > 0 AND remaining_amount > 0");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ddl_batch_no.DataSource = ds.Tables[0];
                    ddl_batch_no.DataValueField = ds.Tables[0].Columns[0].ToString();
                    ddl_batch_no.DataTextField = ds.Tables[0].Columns[0].ToString();
                    ddl_batch_no.DataBind();


                }

                ds.Dispose();
                ddl_batch_no.Items.Insert(0, "Select");
                load_gv_debit_pmt_details(1);
            }
            //vendor payment
            else if (ddl_pmt_paid.SelectedValue == "2")
            {
                DataSet ds = new DataSet();
                ddl_batch_no.Items.Clear();
                ds = d.select_data("SELECT DOC_NO FROM (SELECT doc_no, (FINAL_PRICE - ifnull(amount,0)) AS 'amount' FROM pay_transactionp LEFT JOIN pay_debit_master ON pay_debit_master.client_code = pay_transactionp.CUST_CODE AND pay_debit_master.annuxure_no = pay_transactionp.DOC_NO WHERE CUST_CODE = '" + ddl_pmt_client.SelectedValue + "') AS t1 WHERE amount > 0   ");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ddl_batch_no.DataSource = ds.Tables[0];
                    ddl_batch_no.DataValueField = ds.Tables[0].Columns[0].ToString();
                    ddl_batch_no.DataTextField = ds.Tables[0].Columns[0].ToString();
                    ddl_batch_no.DataBind();


                }

                ds.Dispose();
                ddl_batch_no.Items.Insert(0, "Select");
                load_gv_debit_pmt_details(2);
            }
            //Internal transfer
            else if (ddl_pmt_paid.SelectedValue == "3")
            {
                txt_pmt_ac_no.Text = "";
                txt_pmt_ac_no.Text = ddl_pmt_client.SelectedValue;
                company_bank_load();
                load_gv_debit_pmt_details(3);
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

    protected void ddl_batch_no_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "2";
        try
        {
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            //Employee payemnt
            if (ddl_pmt_paid.SelectedValue == "1")
            {
                txt_pmt_amount.Text = "";
                txt_pmt_amount.Text = d.getsinglestring("SELECT SUM(REPLACE(amount_payable, ',', '')) AS 'amount_payable' FROM paypro_uploaded_data INNER JOIN pay_pro_master ON paypro_uploaded_data.batch_no = pay_pro_master.paypro_batch_id WHERE pay_pro_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND client_code = '" + ddl_pmt_client.SelectedValue + "' AND transaction_status = 'Paid' and batch_no = '" + ddl_batch_no.SelectedValue + "' GROUP BY batch_no");
                txt_pmt_amount.ReadOnly = true;

                txt_comp_bank_name.Text = d.getsinglestring("select  Field1  from pay_zone_master inner join paypro_uploaded_data ON pay_zone_master.comp_code =  paypro_uploaded_data.comp_code and pay_zone_master.Field2 =  paypro_uploaded_data.debit_ac_no  where  paypro_uploaded_data.comp_code = '" + Session["COMP_CODE"].ToString() + "' and paypro_uploaded_data.batch_no =  '" + ddl_batch_no.SelectedValue + "' limit 1");
                //ddl_company_bank.ReadOnly = true;
            }

            //vendor payment
            else if (ddl_pmt_paid.SelectedValue == "2")
            {
                txt_pmt_amount.Text = d.getsinglestring("select FINAL_PRICE from pay_transactionp where DOC_NO = '" + ddl_batch_no.SelectedValue + "'");
                txt_pmt_amount.ReadOnly = true;
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

    protected void btn_pmt_submit_Click(object sender, EventArgs e)
    {
        hidtab.Value = "2";
        int result = 0;
        string insert_field = null, select_field = null, txt_bank_no = null;

        try
        {
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            check_pmt_validation();

            if (ddl_pmt_mode.SelectedValue != "Select")
            {
                if (ddl_pmt_mode.SelectedValue == "Cheque")
                {
                    insert_field = ",pmt_mode_transfer,ChequeNo,ChequeReceivedDate,DipositeDate";
                    select_field = ",'" + ddl_pmt_mode.SelectedValue + "','" + txt_cheque_no.Text + "' ,str_to_date('" + txt_cheque_receive_date.Text + "','%d/%m/%Y'),str_to_date('" + txt_cheque_deposite_date.Text + "','%d/%m/%Y') ";
                }
                else
                {
                    insert_field = ",pmt_mode_transfer,UTR_No";
                    select_field = ",'" + ddl_pmt_mode.SelectedValue + "','" + txt_utr_no.Text + "'";

                }
            }
            txt_bank_no = d.getsinglestring("select  debit_ac_no  from  paypro_uploaded_data  where  paypro_uploaded_data.comp_code = '" + Session["COMP_CODE"].ToString() + "' and paypro_uploaded_data.batch_no =  '" + ddl_batch_no.SelectedValue + "' limit 1");
            //client payment insert 
            if (ddl_pmt_paid.SelectedValue == "1")
            {
                result = d.operation(" insert into pay_debit_master (comp_code,client_code,annuxure_no,Comp_Bank_name,Comp_Account_number,payment_type,description,Amount,payment_date,uploaded_by,uploaded_date " + insert_field + ") values ('" + Session["COMP_CODE"].ToString() + "' , '" + ddl_pmt_client.SelectedValue + "' , '" + ddl_batch_no.SelectedValue + "' , '" + txt_comp_bank_name.Text + "' ,'" + txt_bank_no + "', '" + ddl_pmt_paid.SelectedValue + "','" + ddl_pmt_desc.SelectedItem.Text + "' , '" + txt_pmt_amount.Text + "' , str_to_date('" + txt_pmt_date.Text + "','%d/%m/%Y'),'" + Session["LOGIN_ID"].ToString() + "',now() " + select_field + ") ");
                string id = d.getsinglestring("select max(id) from pay_debit_master ");
                upload_Click(id);
                if (result > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Employee Payment Succsefully!!');", true);
                    load_gv_debit_pmt_details(1);
                    pmt_text_clear(1);
                }
            }

            else if (ddl_pmt_paid.SelectedValue == "2")
            {
                result = d.operation(" insert into pay_debit_master (comp_code,client_code,annuxure_no,Comp_Bank_name,payment_type,description,Amount,payment_date,uploaded_by,uploaded_date " + insert_field + ") values ('" + Session["COMP_CODE"].ToString() + "','" + ddl_pmt_client.SelectedValue + "' , '" + ddl_batch_no.SelectedValue + "' , '" + ddl_company_bank.SelectedValue + "' , '" + ddl_pmt_paid.SelectedValue + "', '" + txt_pmt_desc.Text + "', '" + txt_pmt_amount.Text + "' , str_to_date('" + txt_pmt_date.Text + "','%d/%m/%Y')'" + Session["LOGIN_ID"].ToString() + "',now()  " + select_field + ") ");
                string id = d.getsinglestring("select max(id) from pay_debit_master ");
                upload_Click(id);
                if (result > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Vendor Payment  Succsefully!!');", true);
                    load_gv_debit_pmt_details(2);
                    pmt_text_clear(1);
                }

            }
            else if (ddl_pmt_paid.SelectedValue == "3")
            {
                result = d.operation(" insert into pay_debit_master (payment_type ,transfer_to_bank_name,transfer_to_ac_no ,transfer_from_bank_name,transfer_from_ac_no ,Amount,payment_date,uploaded_by,uploaded_date " + insert_field + ") values ('" + ddl_pmt_paid.SelectedValue + "', '" + ddl_pmt_client.SelectedItem.Text + "' , '" + txt_pmt_ac_no.Text + "' , '" + ddl_company_bank.SelectedItem.Text + "' ,'" + txt_pmt_desc.Text + "', '" + txt_pmt_amount.Text + "' , str_to_date('" + txt_pmt_date.Text + "','%d/%m/%Y'),'" + Session["LOGIN_ID"].ToString() + "',now() " + select_field + ")");

                if (result > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Internal Transfer Succsefully!!');", true);
                    load_gv_debit_pmt_details(3);
                    pmt_text_clear(1);
                }

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

    protected void ddl_pmt_mode_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "2";

        try
        {
            if (ddl_pmt_mode.SelectedValue == "Cheque")
            {
                panel_mode.Visible = false;
                panel_mode_cheque.Visible = true;
                txt_utr_no.Text = "";
            }
            else
            {
                panel_mode.Visible = true;
                panel_mode_cheque.Visible = false;
                txt_cheque_no.Text = "";
                txt_cheque_receive_date.Text = "";
                txt_cheque_deposite_date.Text = "";
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

    protected void upload_Click(string id)
    {


        if (document1_file.HasFile)
        {

            string fileExt = System.IO.Path.GetExtension(document1_file.FileName);
            if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".RAR" || fileExt.ToUpper() == ".ZIP" || fileExt.ToUpper() == ".XLSX" || fileExt.ToUpper() == ".XLS")
            {
                string fileName = Path.GetFileName(document1_file.PostedFile.FileName);
                document1_file.PostedFile.SaveAs(Server.MapPath("~/Annuxure_upload/") + fileName);
               // string id = d.getsinglestring("select ifnull(max(id),0) from pay_debit_master ");

                string file_name = ddl_pmt_client.SelectedValue + ddl_batch_no.SelectedValue + id + fileExt;

                File.Copy(Server.MapPath("~/Annuxure_upload/") + fileName, Server.MapPath("~/Annuxure_upload/") + file_name, true);
                File.Delete(Server.MapPath("~/Annuxure_upload/") + fileName);



                d.operation("update   pay_debit_master set  annuxure_file='" + file_name + "', uploaded_by='" + Session["LOGIN_ID"].ToString() + "', uploaded_date=now()  where comp_code='" + Session["COMP_CODE"].ToString() + "' and annuxure_no = '" + ddl_batch_no.SelectedValue + "'");
                // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Payment Uploaded Succsefully!!');", true);


            }

            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select only JPG, PNG , XLSX, XLS and PDF  Files  !!');", true);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please select only JPG, PNG and PDF Files !!!')", true);
                return;
            }


        }
    }

    protected void check_pmt_validation()
    {
        try
        {
            double payble_amount = double.Parse(txt_pmt_amount.Text);

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
    }

    protected void payment_type_selection()
    {
        try
        {
            //employee payment
            if (ddl_pmt_paid.SelectedValue == "1")
            {
                client_code();
                panel_add_other.Visible = false;
                ddl_payment_type.Visible = true;
                lbl_payment_type.Visible = true;
                panel_annexure_id.Visible = true;
                Panel_client_desc.Visible = true;
                Panel_other_desc.Visible = false;
                btn_add_others.Visible = false;
                panel_ddl_bank_name.Visible = false;
                panel_txt_bank_name.Visible = true;
                panel_ac_no.Visible = false;

                lable_client.Text = "Client :";
                lable_bank.Text = " Company Bank Name :";
                label_ac_no.Text = "";
            }

             //vendor payment
            else if (ddl_pmt_paid.SelectedValue == "2")
            {

                vendor_load();
                company_bank_load();
                ddl_payment_type.Visible = true;
                lbl_payment_type.Visible = true;
                panel_annexure_id.Visible = true;
                Panel_client_desc.Visible = false;
                Panel_other_desc.Visible = true;
                panel_add_other.Visible = false;
                panel_ddl_bank_name.Visible = true;
                panel_txt_bank_name.Visible = false;
                panel_ac_no.Visible = false;

                lable_client.Text = "Vendor :";
                lable_bank.Text = " Company Bank Name :";
                label_ac_no.Text = "Description";
            }

            //Internal transfer
            else
            {

                internal_transfer();
                panel_add_other.Visible = false;
                ddl_payment_type.Visible = true;
                lbl_payment_type.Visible = true;
                panel_annexure_id.Visible = false;
                Panel_client_desc.Visible = false;
                Panel_other_desc.Visible = true;
                panel_ddl_bank_name.Visible = true;
                btn_add_others.Visible = false;
                panel_txt_bank_name.Visible = false;
                panel_ac_no.Visible = true;
                att_upload_panel.Visible = false;
                lable_client.Text = "Transfer To :";
                lable_bank.Text = "Transfer From :";
                label_ac_no.Text = " A/C No :";
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally { }
    }

    protected void vendor_load()
    {

        try
        {
            DataSet ds = new DataSet();
            ddl_pmt_client.Items.Clear();
            ds = d.select_data("select VEND_ID,VEND_NAME from pay_vendor_master where comp_code = '" + Session["COMP_CODE"].ToString() + "'");

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddl_pmt_client.DataSource = ds.Tables[0];
                ddl_pmt_client.DataValueField = ds.Tables[0].Columns[0].ToString();
                ddl_pmt_client.DataTextField = ds.Tables[0].Columns[1].ToString();
                ddl_pmt_client.DataBind();

                ddl_pmt_client.Items.Insert(0, "Select");
            }

            ds.Dispose();



        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
    }

    protected void internal_transfer()
    {

        try
        {
            DataSet ds = new DataSet();
            ddl_pmt_client.Items.Clear();
            ds = d.select_data("Select field2,Field1 from pay_zone_master where Type = 'bank_details' and CLIENT_CODE is null");

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddl_pmt_client.DataSource = ds.Tables[0];
                ddl_pmt_client.DataValueField = ds.Tables[0].Columns[0].ToString();
                ddl_pmt_client.DataTextField = ds.Tables[0].Columns[1].ToString();
                ddl_pmt_client.DataBind();

                ddl_pmt_client.Items.Insert(0, "Select");
            }

            ds.Dispose();



        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            load_gv_debit_pmt_details(3);
        }
    }

    protected void load_gv_debit_pmt_details(int i)
    {
        try
        {
            DataSet ds = new DataSet();
            gv_debit_pmt_details.DataSource = null;
            gv_debit_pmt_details.DataBind();

            //Employee payment
            if (i == 1)
            {
                ds = d.select_data("SELECT distinct pay_debit_master.Id,pay_debit_master.Comp_Bank_name, Comp_Account_number, pay_client_master.client_name, annuxure_no, Amount, description, date_format(payment_date,'%d/%m/%Y') as 'payment_date', annuxure_file FROM pay_debit_master INNER JOIN pay_client_master ON pay_debit_master.comp_code = pay_client_master.comp_code AND pay_debit_master.client_code = pay_client_master.client_code where pay_debit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' and pay_debit_master.client_Code = '" + ddl_pmt_client.SelectedValue + "' ");
            }
            //Vendor payment
            else if (i == 2)
            {
                ds = d.select_data("SELECT  distinct pay_debit_master.Id, pay_debit_master.Comp_Bank_name, Comp_Account_number, VEND_NAME, annuxure_no, Amount, description,  date_format(payment_date,'%d/%m/%Y') as 'payment_date', annuxure_file FROM pay_debit_master INNER JOIN pay_vendor_master ON pay_debit_master.client_code = pay_vendor_master.VEND_ID Where pay_debit_master.client_Code = '" + ddl_pmt_client.SelectedValue + "' ");
            }
            //Internal Transfer
            else if (i == 3)
            {
                ds = d.select_data("SELECT pay_debit_master.Comp_Bank_name, Comp_Account_number, pay_client_master.client_name, annuxure_no, Amount, description,  date_format(payment_date,'%d/%m/%Y') as 'payment_date', annuxure_file FROM pay_debit_master INNER JOIN pay_client_master ON pay_debit_master.comp_code = pay_client_master.comp_code AND pay_debit_master.client_code = pay_client_master.client_code where pay_debit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' and pay_debit_master.client_Code = '" + ddl_pmt_client.SelectedValue + "' ");

            }


            gv_debit_pmt_details.DataSource = ds;
            gv_debit_pmt_details.DataBind();
            ds.Dispose();


        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }

    }
    protected void load_gv_debit_pmt_details(string i)
    {
        try
        {
            DataSet ds = new DataSet();
            gv_debit_pmt_details.DataSource = null;
            gv_debit_pmt_details.DataBind();

            //Employee payment
            if (i == "1")
            {
                ds = d.select_data("SELECT distinct pay_debit_master.Id,pay_debit_master.Comp_Bank_name, Comp_Account_number, pay_client_master.client_name, annuxure_no, Amount, description, date_format(payment_date,'%d/%m/%Y') as 'payment_date', annuxure_file FROM pay_debit_master INNER JOIN pay_client_master ON pay_debit_master.comp_code = pay_client_master.comp_code AND pay_debit_master.client_code = pay_client_master.client_code where pay_debit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' and pay_debit_master.payment_type = '" + ddl_pmt_paid.SelectedValue + "' ");
            }
            //Vendor payment
            else if (i == "2")
            {
                ds = d.select_data("SELECT  distinct pay_debit_master.Id, pay_debit_master.Comp_Bank_name, Comp_Account_number, VEND_NAME, annuxure_no, Amount, description,  date_format(payment_date,'%d/%m/%Y') as 'payment_date', annuxure_file FROM pay_debit_master INNER JOIN pay_vendor_master ON pay_debit_master.client_code = pay_vendor_master.VEND_ID Where pay_debit_master.payment_type = '" + ddl_pmt_paid.SelectedValue + "' ");
            }
            //Internal Transfer
            else if (i == "3")
            {
                ds = d.select_data("SELECT pay_debit_master.Comp_Bank_name, Comp_Account_number, pay_client_master.client_name, annuxure_no, Amount, description,  date_format(payment_date,'%d/%m/%Y') as 'payment_date', annuxure_file FROM pay_debit_master INNER JOIN pay_client_master ON pay_debit_master.comp_code = pay_client_master.comp_code AND pay_debit_master.client_code = pay_client_master.client_code where pay_debit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' and pay_debit_master.client_Code = '" + ddl_pmt_client.SelectedValue + "' ");

            }


            gv_debit_pmt_details.DataSource = ds;
            gv_debit_pmt_details.DataBind();
            ds.Dispose();


        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }

    }
    protected void gv_debit_pmt_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
        //Employee payment
        if (ddl_pmt_paid.SelectedValue == "1")
        {
            e.Row.Cells[2].Visible = false;
            e.Row.Cells[10].Visible = false;
        }
        //Vendor payment
        else if (ddl_pmt_paid.SelectedValue == "2")
        {
            e.Row.Cells[2].Visible = false;
            e.Row.Cells[10].Visible = false;
        }
        //Internal Transfer
        else if (ddl_pmt_paid.SelectedValue == "3")
        {

        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
            //e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            //e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.gv_minibank, "Select$" + e.Row.RowIndex);

        }
    }
    protected void pmt_text_clear(int text_clear_mode)
    {
        if (text_clear_mode.Equals(1))
        {
            ddl_pmt_client.SelectedValue = "Select";
        }
        else
        {
            ddl_pmt_client.Items.Clear();
        }
        ddl_batch_no.Items.Clear();
        ddl_company_bank.Items.Clear();
        txt_pmt_desc.Text = "";
        txt_pmt_amount.Text = "";
        txt_pmt_date.Text = "";
        txt_pmt_ac_no.Text = "";
        ddl_pmt_mode.SelectedIndex = 0;
        txt_utr_no.Text = "";
        txt_cheque_no.Text = "";
        txt_cheque_receive_date.Text = "";
        txt_cheque_deposite_date.Text = "";
        txt_comp_bank_name.Text = "";
        ddl_pmt_desc.SelectedValue = "Select";

    }

    protected void lnkpmtDownload_Command(object sender, CommandEventArgs e)
    {
        string filename = e.CommandArgument.ToString();


        if (filename != "")
        {
            downloadfile(filename);
        }

        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attachment File Cannot Be Uploaded !!!')", true);
        }
    }

    protected void downloadfile(string filename)
    {

        //var result = filename.Substring(filename.Length - 4);
        //if (result.Contains("jpeg"))
        //{
        //    result = ".jpeg";
        //}
        try
        {


            string path2 = Server.MapPath("~\\Annuxure_upload\\" + filename);

            Response.Clear();
            Response.ContentType = "Application/pdf/jpg/jpeg/png/zip/xls/xlsx";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename);
            Response.TransmitFile("~\\Annuxure_upload\\" + filename);
            Response.WriteFile(path2);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Response.End();
            Response.Close();





        }
        catch (Exception ex) { }


    }
    protected void gv_invoice_list_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_invoice_list.UseAccessibleHeader = false;
            gv_invoice_list.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }
    protected void lnkpmtDelete_Command(object sender, CommandEventArgs e)
    {
        int result = 0;

        try
        {
            string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });

            string Id = commandArgs[0];

            string filename = commandArgs[1];

            if (!filename.Equals(""))
            {
                string delete_file = System.IO.Path.Combine(@"~/Annuxure_upload/" + filename);
                if (File.Exists(delete_file))
                {
                    File.Delete(delete_file);
                }
            }
            result = d.operation("delete FROM pay_debit_master WHERE  Id = '" + Id + "'");

            if (result > 0)
            {

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Deleted Succsefully !!!')", true);
                load_gv_debit_pmt_details(ddl_pmt_paid.SelectedValue);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Deletion Failed !!!')", true);

            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {

        }
    }
    protected void gv_debit_pmt_details_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_debit_pmt_details.UseAccessibleHeader = false;
            gv_debit_pmt_details.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }
    protected void btn_upload_Click(object sender, EventArgs e)
    {
        //try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        //catch { }
        string FilePath = "";
        if (FileUpload1.HasFile)
        {
            try
            {
                string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                if (Extension.ToUpper() == ".XLS" || Extension.ToUpper() == ".XLSX")
                {
                    string FolderPath = "~/Temp_images/";
                    FilePath = Server.MapPath(FolderPath + FileName);
                    FileUpload1.SaveAs(FilePath);
                    btn_Import_Click(FilePath, Extension, "Yes", FileName);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Ledger File Uploaded Successfully...');", true);
                    File.Delete(FilePath);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please upload a valid excel file.');", true);
                }
            }
            catch (Exception ee)
            {
                throw ee;
                // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('System Error - Please Try again....');", true);
            }
            finally
            {
                File.Delete(FilePath);
            }
        }
    }
    public void btn_Import_Click(string FilePath, string Extension, string IsHDR, string filename)
    {
        string conStr = "";
        switch (Extension.ToUpper())
        {
            case ".XLS":
                conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                break;
            case ".XLSX":
                conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                break;
        }
        conStr = String.Format(conStr, FilePath, IsHDR);
        OleDbConnection connExcel = new OleDbConnection(conStr);
        OleDbCommand cmdExcel = new OleDbCommand();
        //   OleDbCommand cmdExcel1 = new OleDbCommand();
        OleDbDataAdapter oda = new OleDbDataAdapter();
        // OleDbDataAdapter oda1 = new OleDbDataAdapter();
        System.Data.DataTable dt = new System.Data.DataTable();
        //System.Data.DataTable dt1 = new System.Data.DataTable();
        cmdExcel.Connection = connExcel;
        //cmdExcel1.Connection = connExcel;

        // Get The Name of First Sheet
        connExcel.Open();
        System.Data.DataTable dtExcelSchema;
        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
        connExcel.Close();

        //Read Data from First Sheet

        connExcel.Open();
        cmdExcel.CommandText = "SELECT * FROM [" + sheetName + "]";
        oda.SelectCommand = cmdExcel;
        oda.Fill(dt);

        connExcel.Close();

        //check file format

        //Push Datatable to database
        DataTable table2 = new DataTable("ledger");
        if (ddl_upload_lg_client.SelectedValue == "8")//Kotak
        {
            table2.Columns.Add("Invoice_Number");
            table2.Columns.Add("Invoice_Date");
            table2.Columns.Add("GL_Date");
            table2.Columns.Add("Batch_No");
            table2.Columns.Add("Invoice_Type");
            table2.Columns.Add("Payment_Voucher");
            table2.Columns.Add("Check_No");
            table2.Columns.Add("Description");
            table2.Columns.Add("Account");
            table2.Columns.Add("Amount_Dr");
            table2.Columns.Add("Amount_Cr");
            table2.Columns.Add("Comments");
        }
        else if (ddl_upload_lg_client.SelectedValue == "4")//BFL
        {
            table2.Columns.Add("Account");
            table2.Columns.Add("Business_Area");
            table2.Columns.Add("Offsetting_acct_no");
            table2.Columns.Add("Assignment");
            table2.Columns.Add("Reference");
            table2.Columns.Add("Document_Number");
            table2.Columns.Add("Document_Type");
            table2.Columns.Add("Tax_code");
            table2.Columns.Add("Posting_Date");
            table2.Columns.Add("Document_Date");
            table2.Columns.Add("Clearing_Document");
            table2.Columns.Add("Special_GL_ind");
            table2.Columns.Add("Amount_in_local_currency");
            table2.Columns.Add("Text");
            table2.Columns.Add("Withholding_tax_amnt");
            table2.Columns.Add("Withhldg_tax_base_amount");
            table2.Columns.Add("Payment_reference");
            table2.Columns.Add("Comments");
            //cmdExcel1.CommandText = "SELECT * FROM [" + sheetName + "]";
            //oda1.SelectCommand = cmdExcel1;
            //oda1.Fill(dt1);
        }
        else if (ddl_upload_lg_client.SelectedValue == "RLIC HK")
        {
            table2.Columns.Add("Clearing_Date");
            table2.Columns.Add("Clearing_Document");
            table2.Columns.Add("Name_of_posting_key");
            table2.Columns.Add("Doc_No");
            table2.Columns.Add("Document_Date");
            table2.Columns.Add("Due_On");
            table2.Columns.Add("Discount");
            table2.Columns.Add("Amount");
            table2.Columns.Add("Reference_Number");
            table2.Columns.Add("Assignment_Number");
            table2.Columns.Add("Comments");
        }
        else if (ddl_upload_lg_client.SelectedValue == "HDFC")
        {
            table2.Columns.Add("Assignment");
            table2.Columns.Add("Document Number");
            table2.Columns.Add("Document Type");
            table2.Columns.Add("Document Date");
            table2.Columns.Add("Special GL ind");
            table2.Columns.Add("Posting Date");
            table2.Columns.Add("Withholding tax amnt");
            table2.Columns.Add("Withhldg tax base amount");
            table2.Columns.Add("Amount in local currency");
            table2.Columns.Add("Clearing Document");
            table2.Columns.Add("Text");
            table2.Columns.Add("Reference");
            table2.Columns.Add("Parked by");
            table2.Columns.Add("Account");
            table2.Columns.Add("Comments");
        }
        else if (ddl_upload_lg_client.SelectedValue == "RBL")
        {
            table2.Columns.Add("BAZ_CLAIM_NO");
            table2.Columns.Add("CLAIM_TYPE");
            table2.Columns.Add("STATUS_DATE");
            table2.Columns.Add("STATUS");
            table2.Columns.Add("APPROVED_AMOUNT");
            table2.Columns.Add("ADJUSTED_AMOUNT");
            table2.Columns.Add("NET_PAYABLE_AMOUNT");
            table2.Columns.Add("CLAIM_FOR_USER_NAME_VENDOR_NAME");
            table2.Columns.Add("PAYREFNO_PAYREFDATE");
            table2.Columns.Add("PAYMENT_SEQUENCE_NO_DATE");
            table2.Columns.Add("TDS_AMOUNT");
            table2.Columns.Add("INVOICE_NO");
            table2.Columns.Add("INVOICE_DATE");
            table2.Columns.Add("COMMENTS");
        }
        else if (ddl_upload_lg_client.SelectedValue.Contains("BAG") || ddl_upload_lg_client.SelectedValue == "BG")
        {
            table2.Columns.Add("Account_Code");
            table2.Columns.Add("Accounting_Period");
            table2.Columns.Add("Base_Amount");
            table2.Columns.Add("Debit_Credit_marker");
            table2.Columns.Add("Transaction_Date");
            table2.Columns.Add("Journal_No");
            table2.Columns.Add("Journal_Line");
            table2.Columns.Add("Journal_Type");
            table2.Columns.Add("Journal_Source");
            table2.Columns.Add("Transaction_Reference");
            table2.Columns.Add("Description");
            table2.Columns.Add("COMMENTS");
        }
        else if (ddl_upload_lg_client.SelectedValue == "DHFL")
        {
            table2.Columns.Add("cluster_ref_no");
            table2.Columns.Add("circle");
            table2.Columns.Add("cluster");
            table2.Columns.Add("branch");
            table2.Columns.Add("service_centre");
            table2.Columns.Add("invoice_no");
            table2.Columns.Add("invoice_dt");
            table2.Columns.Add("vendor_name");
            table2.Columns.Add("head");
            table2.Columns.Add("gross_amt");
            table2.Columns.Add("period_for_which_payment_is_due");
            table2.Columns.Add("pan_no");
            table2.Columns.Add("service_tax_no");
            table2.Columns.Add("inward_date");
            table2.Columns.Add("received_by");
            table2.Columns.Add("paid_on");
            table2.Columns.Add("mode_of_payment");
            table2.Columns.Add("dispatch_date");
            table2.Columns.Add("dispatched_to");
            table2.Columns.Add("COMMENTS");
        }
        else if (ddl_upload_lg_client.SelectedValue == "7")
        {
            table2.Columns.Add("Doc_Chq_Date");
            table2.Columns.Add("Amount_in_Local_Currency");
            table2.Columns.Add("Text");
            table2.Columns.Add("UTR_No");
            table2.Columns.Add("Clearing_Date");
            table2.Columns.Add("Hdr_Text_Bank");
            table2.Columns.Add("COMMENTS");
        }
        else if (ddl_upload_lg_client.SelectedValue == "BRLI")
        {
            table2.Columns.Add("No");
            table2.Columns.Add("Transaction_Number");
            table2.Columns.Add("Vendor_Invoice_Date");
            table2.Columns.Add("Date");
            table2.Columns.Add("Due_Date");
            table2.Columns.Add("Age");
            table2.Columns.Add("Amount_Gross");
            table2.Columns.Add("CGST");
            table2.Columns.Add("SGST");
            table2.Columns.Add("IGST");
            table2.Columns.Add("Tds");
            table2.Columns.Add("Net_Payable");
            table2.Columns.Add("COMMENTS");
        }
        try
        {
            foreach (DataRow r in dt.Rows)
            {
                try
                {
                    int res = 0;

                    if (ddl_upload_lg_client.SelectedValue == "8")//Kotak
                    {
                        if (r[0].ToString().Trim() != "" && !r[0].ToString().ToUpper().Contains("REF") && !r[0].ToString().ToUpper().Contains("DATE") && !r[0].ToString().ToUpper().Contains("VENDOR") && !r[0].ToString().ToUpper().Contains("INVOICE") && !r[0].ToString().ToUpper().Contains("FROM") && !r[0].ToString().ToUpper().Contains("LEDGER"))
                        {
                            try
                            {
                                if (r[0].ToString().ToUpper().Contains("TDS"))
                                {
                                    res = d.operation("update pay_report_gst set tds_deducted=" + r[9].ToString().Trim() + " where INSTR('" + r[0].ToString().Trim() + "', invoice_no) > 0");
                                }
                                else
                                {
                                    string payment_date = r[2].ToString().Trim();
                                    if (payment_date.Length > 10) { payment_date = payment_date.Substring(0, 10); }
                                    
                                    //CHANGES BY VINOD TO CHECK SERVER DATETIME FORMAT
                                    //res = d.operation("update pay_report_gst set payment=" + r[9].ToString().Trim() + ",payment_date=str_to_date('" + payment_date + "','%m/%d/%Y'), flag=1 where invoice_no = '" + r[0].ToString().Trim() + "'");
                                    res = d.operation("update pay_report_gst set payment=" + r[9].ToString().Trim() + ",payment_date='" + payment_date + "', flag=1 where invoice_no = '" + r[0].ToString().Trim() + "'");
                                }
                                if (res == 0)
                                {
                                    //if (!d.getsinglestring("select payment_status from pay_pro_master where BANK_EMP_AC_CODE = '" + bank_code.Trim() + "' and floor(Payment) = " + amount + " and comp_code='" + comp_code + "' and MONTH = " + txt_month_year.Text.Substring(0, 2) + " AND YEAR=" + txt_month_year.Text.Substring(3) + "").Equals("1"))
                                    //  {
                                    table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), "Invoice number not Matching");
                                    //}
                                }

                            }
                            catch (Exception ex)
                            {
                                table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), ex.Message.ToString());
                            }
                        }
                    }
                    else if (ddl_upload_lg_client.SelectedValue == "RLIC HK")//Reliance
                    {
                        try
                        {
                            if (r[8].ToString().Trim() != "" && !r[8].ToString().ToUpper().Contains("REFER"))   
                            {
                                string payment_date = r[5].ToString().Trim();
                                double amount = double.Parse(r[7].ToString().Trim()) * -1;
                                foreach (DataRow m in dt.Rows)
                                {
                                    if (m[1].ToString().Trim() == r[1].ToString().Trim() && m[7].ToString().Trim() == amount.ToString())
                                    {
                                        payment_date = m[5].ToString().Trim();
                                        break;
                                    }
                                }
                                if (payment_date.Length > 10) { payment_date = payment_date.Substring(0, 10); }
                               // res = d.operation("update pay_report_gst set payment=" + amount + ",payment_date=str_to_date('" + payment_date + "','%d/%m/%Y'), flag=1, tds_deducted = 0  where invoice_no = '" + r[8].ToString().Trim() + "'");
                                res = d.operation("update pay_report_gst set payment=" + amount + ",payment_date='" + payment_date + "', flag=1, tds_deducted = 0  where invoice_no = '" + r[8].ToString().Trim() + "'");

                                if (res == 0)
                                {
                                    table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), "Invoice number not Matching");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), ex.Message.ToString());
                        }

                    }
                    else if (ddl_upload_lg_client.SelectedValue == "4")//BFL
                    {
                        if (r[6].ToString().Trim() != "" && !r[6].ToString().ToUpper().Contains("REF"))
                        {
                            try
                            {
                                string payment_date = "";
                                double amount = double.Parse(r[14].ToString().Trim()) * -1;//converting to positive number
                                double tds = double.Parse(r[16].ToString().Trim()) * -1;//converting to positive number
                                foreach (DataRow m in dt.Rows)
                                {
                                    if (m[12].ToString().Trim() == r[12].ToString().Trim() && m[14].ToString().Trim() == amount.ToString())
                                    {
                                        payment_date = m[11].ToString().Trim();
                                        break;
                                    }
                                }

                                if (payment_date.Length > 10) { payment_date = payment_date.Substring(0, 10); }

                                if (!payment_date.Equals(""))
                                {
                                    //res = d.operation("update pay_report_gst set transaction_ref='" + r[18].ToString().Trim() + "', batchid = '" + r[12].ToString().Trim() + "', tds_deducted=" + tds + ", payment=" + amount + ",payment_date=str_to_date('" + payment_date + "','%d/%m/%Y'), flag=1 where invoice_no like '%" + r[6].ToString().Trim() + "%'");
                                    res = d.operation("update pay_report_gst set transaction_ref='" + r[18].ToString().Trim() + "', batchid = '" + r[12].ToString().Trim() + "', tds_deducted=" + tds + ", payment=" + amount + ",payment_date='" + payment_date + "', flag=1 where invoice_no like '%" + r[6].ToString().Trim() + "%'");
                                }

                                if (res == 0)
                                {
                                    table2.Rows.Add(r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), r[13].ToString(), r[14].ToString(), r[15].ToString(), r[16].ToString(), r[17].ToString(), "Invoice number not Matching");
                                }
                            }
                            catch (Exception ex)
                            {
                                table2.Rows.Add(r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), r[13].ToString(), r[14].ToString(), r[15].ToString(), r[16].ToString(), r[17].ToString(), ex.Message.ToString());
                            }
                        }
                    }
                    else if (ddl_upload_lg_client.SelectedValue == "HDFC")//HDFC
                    {
                        if (r[12].ToString().Trim() != "" && !r[12].ToString().ToUpper().Contains("REF"))
                        {
                            try
                            {
                                string payment_date = "";
                                double amount = double.Parse(r[9].ToString().Trim()) * -1;//converting to positive number
                                double tds = double.Parse(r[7].ToString().Trim()) * -1;//converting to positive number
                                foreach (DataRow m in dt.Rows)
                                {
                                    if (m[10].ToString().Trim() == r[10].ToString().Trim() && m[9].ToString().Trim() == amount.ToString())
                                    {
                                        payment_date = m[6].ToString().Trim();
                                        break;
                                    }
                                }

                                if (payment_date.Length > 10) { payment_date = payment_date.Substring(0, 10); }

                                if (!payment_date.Equals(""))
                                {
                                    //res = d.operation("update pay_report_gst set batchid = '" + r[10].ToString().Trim() + "', tds_deducted=" + tds + ", payment=" + amount + ",payment_date=str_to_date('" + payment_date + "','%d/%m/%Y'), flag=1 where invoice_no like '%" + r[12].ToString().Trim() + "%'");
                                    res = d.operation("update pay_report_gst set batchid = '" + r[10].ToString().Trim() + "', tds_deducted=" + tds + ", payment=" + amount + ",payment_date='" + payment_date + "', flag=1 where invoice_no like '%" + r[12].ToString().Trim() + "%'");
                                }

                                if (res == 0)
                                {
                                    table2.Rows.Add(r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), r[13].ToString(), r[14].ToString(), "Invoice number not Matching");
                                }
                            }
                            catch (Exception ex)
                            {
                                table2.Rows.Add(r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), r[13].ToString(), r[14].ToString(), ex.Message.ToString());
                            }
                        }
                    }
                    else if (ddl_upload_lg_client.SelectedValue.Contains("BAG") || ddl_upload_lg_client.SelectedValue == "BG")//BAG
                    {
                        if (r[12].ToString().Trim() != "" && !r[12].ToString().ToUpper().Contains("REF"))
                        {
                            try
                            {
                                string payment_date = "";
                                double amount = double.Parse(r[5].ToString().Trim()) * -1;//converting to positive number
                                foreach (DataRow m in dt.Rows)
                                {
                                    if (m[13].ToString().Trim().Contains(r[13].ToString().Trim()) && m[5].ToString().Trim() == amount.ToString())
                                    {
                                        payment_date = m[7].ToString().Trim();
                                        break;
                                    }
                                }

                                if (payment_date.Length > 10) { payment_date = payment_date.Substring(0, 10); }

                                if (!payment_date.Equals(""))
                                {
                                    //res = d.operation("update pay_report_gst set batchid = '" + r[13].ToString().Trim() + "', payment=" + amount + ",payment_date=str_to_date('" + payment_date + "','%d/%m/%Y'), flag=1 where invoice_no like '%" + r[12].ToString().Trim() + "%'");
                                    res = d.operation("update pay_report_gst set batchid = '" + r[13].ToString().Trim() + "', payment=" + amount + ",payment_date='" + payment_date + "', flag=1 where invoice_no like '%" + r[12].ToString().Trim() + "%'");
                                }

                                if (res == 0)
                                {
                                    table2.Rows.Add(r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), r[13].ToString(), "Invoice number not Matching");
                                }
                            }
                            catch (Exception ex)
                            {
                                table2.Rows.Add(r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), r[13].ToString(), ex.Message.ToString());
                            }
                        }
                    }
                    else if (ddl_upload_lg_client.SelectedValue == "RBL")//RBL BANK Ltd.s
                    {
                        try
                        {
                            if (r[11].ToString().Trim() != "" && !r[11].ToString().ToUpper().Contains("INVOI"))
                            {
                                string payment_date = r[2].ToString().Trim();
                                if (payment_date.Length > 10) { payment_date = payment_date.Substring(0, 10); }
                                double amount = double.Parse(r[6].ToString().Trim());
                                //res = d.operation("update pay_report_gst set payment=" + amount + ",payment_date=str_to_date('" + payment_date + "','%d/%m/%Y'), flag=1, tds_deducted = " + double.Parse(r[10].ToString().Trim()) + " where invoice_no = '" + r[11].ToString().Trim() + "'");
                                res = d.operation("update pay_report_gst set payment=" + amount + ",payment_date='" + payment_date + "', flag=1, tds_deducted = " + double.Parse(r[10].ToString().Trim()) + " where invoice_no = '" + r[11].ToString().Trim() + "'");

                                if (res == 0)
                                {
                                    table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), "Invoice number not Matching");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), ex.Message.ToString());
                        }

                    }
                    else if (ddl_upload_lg_client.SelectedValue == "DHFL")
                    {
                        try
                        {
                            if (r[5].ToString().Trim() != "" && !r[5].ToString().ToUpper().Contains("INVOI"))
                            {
                                string payment_date = r[15].ToString().Trim();
                                if (payment_date.Length > 10) { payment_date = payment_date.Substring(0, 10); }
                                double amount = double.Parse(r[9].ToString().Trim());
                                //res = d.operation("update pay_report_gst set payment=" + amount + ",payment_date=str_to_date('" + payment_date + "','%d/%m/%Y'), flag=1 where invoice_no = '" + r[5].ToString().Trim() + "'");
                                res = d.operation("update pay_report_gst set payment=" + amount + ",payment_date='" + payment_date + "', flag=1 where invoice_no = '" + r[5].ToString().Trim() + "'");

                                if (res == 0)
                                {
                                    table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), r[13].ToString(), r[14].ToString(), r[15].ToString(), r[16].ToString(), r[17].ToString(), r[18].ToString(), "Invoice number not Matching");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), r[13].ToString(), r[14].ToString(), r[15].ToString(), r[16].ToString(), r[17].ToString(), r[18].ToString(), ex.Message.ToString());
                        }

                    }
                    else if (ddl_upload_lg_client.SelectedValue == "7")
                    {
                        try
                        {
                            if (r[1].ToString().Trim() != "" && !r[1].ToString().ToUpper().Contains("DOCUM"))
                            {

                                string payment_date = "";
                                double amount = double.Parse(r[3].ToString().Trim()) * -1;//converting to positive number
                                foreach (DataRow m in dt.Rows)
                                {
                                    if (m[5].ToString().Trim().Contains(r[5].ToString().Trim()) && m[3].ToString().Trim() == amount.ToString())
                                    {
                                        payment_date = m[6].ToString().Trim();
                                        break;
                                    }
                                }

                                if (!payment_date.Equals(""))
                                {
                                    if (payment_date.Length > 10) { payment_date = payment_date.Substring(0, 10); }
                                    res = d.operation("update pay_report_gst set payment=" + amount + ",payment_date= '" + payment_date + "', flag=1, transaction_ref = '" + r[5].ToString().Trim() + "' where invoice_no = '" + r[7].ToString().Replace("BN:", "").Trim() + "'");
                                }
                                if (res == 0)
                                {
                                    table2.Rows.Add(r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), "Invoice number not Matching");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            table2.Rows.Add(r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), ex.Message.ToString());
                        }

                    }
                    else if (ddl_upload_lg_client.SelectedValue == "BRLI")
                    {
                        try
                        {
                            if (r[2].ToString().Trim() != "" && !r[2].ToString().ToUpper().Contains("NO"))
                            {
                                string payment_date = r[6].ToString().Trim();
                                if (payment_date.Length > 10) { payment_date = payment_date.Substring(0, 10); }
                                double amount = double.Parse(r[14].ToString().Trim());
                                res = d.operation("update pay_report_gst set payment=" + amount + ",transaction_ref = '" + r[3].ToString().Trim() + "', payment_date='" + payment_date + "', flag=1 where invoice_no = '" + r[2].ToString().Trim() + "'");

                                if (res == 0)
                                {
                                    table2.Rows.Add(r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[14].ToString(), "Invoice number not Matching");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            table2.Rows.Add(r[2].ToString(), r[3].ToString(), r[4].ToString(), r[5].ToString(), r[6].ToString(), r[7].ToString(), r[8].ToString(), r[9].ToString(), r[10].ToString(), r[11].ToString(), r[12].ToString(), r[14].ToString(), ex.Message.ToString());
                        }

                    }
                }
                catch (Exception ex)
                {
                    // comments = ex.Message;
                    throw ex;
                }
            }
            if (table2.Rows.Count > 0)
            {
                DataSet ds = new DataSet("ledger");
                ds.Tables.Add(table2);
                send_file(ds, ddl_upload_lg_client.SelectedValue);
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('File Uploaded Successfully !!!');", true);

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            d.con.Close();
            connExcel.Close();
        }
    }
    private void send_file(DataSet ds, string client_code)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Ledger_issue.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Repeater Repeater1 = new Repeater();
            Repeater1.DataSource = ds;
            Repeater1.HeaderTemplate = new MyTemplate(ListItemType.Header, ds, client_code);
            Repeater1.ItemTemplate = new MyTemplate(ListItemType.Item, ds, client_code);
            Repeater1.FooterTemplate = new MyTemplate(ListItemType.Footer, null, null);
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
        string client_code = "";
        public MyTemplate(ListItemType type, DataSet ds, string client_code)
        {
            this.type = type;
            this.ds = ds;
            this.client_code = client_code;
            ctr = 0;
        }
        public void InstantiateIn(Control container)
        {

            switch (type)
            {
                case ListItemType.Header:
                    if (client_code == "8")
                    {
                        lc = new LiteralControl("<table border=1><tr><th>Invoice Date</th><th>GL Date</th><th>Batch No</th><th>Invoice Type</th><th>Payment Voucher</th><th>Check No</th><th>Description</th><th>Account</th><th>Amount Dr</th><th>Amount Cr</th></tr>");
                    }
                    else if (client_code == "4")
                    {
                        lc = new LiteralControl("<table border=1><tr><th>Account</th><th>Business Area</th><th>Offsetting acct no</th><th>Assignment</th><th>Reference</th><th>Document Number</th><th>Document Type</th><th>Tax code</th><th>Posting Date</th><th>Document Date</th><th>Clearing Document</th><th>Special GL ind</th><th>Amount in local currency</th><th>Text</th><th>Withholding tax amnt</th><th>Withhldg tax base amount</th><th>Payment reference</th></tr>");
                    }
                    else if (client_code == "RLIC HK")
                    {
                        lc = new LiteralControl("<table border=1><tr><th>Clearing Date</th><th>Clearing Document</th><th>Name of posting key</th><th>Doc. No.</th><th>Document Date</th><th>Due On</th><th>Discount</th><th>Amount</th><th>Reference Number</th><th>Assignment Number</th><th>Comments</th></tr>");
                    }
                    else if (client_code == "HDFC")
                    {
                        lc = new LiteralControl("<table border=1><tr><th>ASSIGNMENT</th><th>DOCUMENT NUMBER</th><th>DOCUMENT TYPE</th><th>DOCUMENT DATE</th><th>SPECIAL GL INDA</th><th>POSTING DATE</th><th>WITHHOLDING TAX AMNT</th><th>WITHHLDG TAX BASE AMOUNT</th><th>AMOUNT IN LOCAL CURRENCY</th><th>CLEARING DOCUMENT</th><th>TEXT</th><th>REFERENCE</th><th>PARKED BY</th><th>ACCOUNT</th><th>COMMENTS</th></tr>");
                    }
                    else if (client_code == "RBL")
                    {
                        lc = new LiteralControl("<table border=1><tr><th>BAZ_CLAIM_NO</TH><TH>CLAIM_TYPE</TH><TH>STATUS_DATE</TH><TH>STATUS</TH><TH>APPROVED_AMOUNT</TH><TH>ADJUSTED_AMOUNT</TH><TH>NET_PAYABLE_AMOUNT</TH><TH>CLAIM_FOR_USER_NAME_VENDOR_NAME</TH><TH>PAYREFNO_PAYREFDATE</TH><TH>PAYMENT_SEQUENCE_NO_DATE</TH><TH>TDS_AMOUNT</TH><TH>INVOICE_NO</TH><TH>INVOICE_DATE</TH><TH>COMMENTS</th></tr>");
                    }
                    else if (client_code.Contains("BAG") || client_code == "BG")//BAGIC
                    {
                        lc = new LiteralControl("<table border=1><tr><th>ACCOUNT_CODE</TH><TH>ACCOUNTING_PERIOD</TH><TH>BASE_AMOUNT</TH><TH>DEBIT_CREDIT_MARKER</TH><TH>TRANSACTION_DATE</TH><TH>JOURNAL_NO</TH><TH>JOURNAL_LINE</TH><TH>JOURNAL_TYPE</TH><TH>JOURNAL_SOURCE</TH><TH>TRANSACTION_REFERENCE</TH><TH>DESCRIPTION</TH><TH>COMMENTS</th></tr>");
                    }
                    else if (client_code == "DHFL")
                    {
                        lc = new LiteralControl("<table border=1><tr><th>cluster_ref_no</TH><TH>circle</TH><TH>cluster</TH><TH>branch</TH><TH>service_centre</TH><TH>invoice_no</TH><TH>invoice_dt</TH><TH>vendor_name</TH><TH>head</TH><TH>gross_amt</TH><TH>period_for_which_payment_is_due</TH><TH>pan_no</TH><TH>service_tax_no</TH><TH>inward_date</TH><TH>received_by</TH><TH>paid_on</TH><TH>mode_of_payment</TH><TH>dispatch_date</TH><TH>dispatched_to</TH><TH>comments</TH></tr>");
                    }
                    else if (client_code == "7")
                    {
                        lc = new LiteralControl("<table border=1><tr><th>Doc_Chq_Date</th><th>Amount_in_Local_Currency</th><th>Text</th><th>UTR_No</th><th>Clearing_Date</th><th>Hdr_Text_Bank</th><TH>comments</TH></tr>");
                    }
                    else if (client_code == "BRLI")
                    {
                        lc = new LiteralControl("<table border=1><tr><th>No</th><th>Transaction_Number</th><th>Vendor_Invoice_Date</th><th>Date</th><th>Due_Date</th><th>Age</th><th>Amount_Gross</th><th>CGST</th><th>SGST</th><th>IGST</th><th>Tds</th><th>Net_Payable</th><TH>comments</TH></tr>");
                    }
                    break;
                case ListItemType.Item:
                    if (client_code == "8")
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr][0] + " </td><td>" + ds.Tables[0].Rows[ctr][1] + " </td><td>" + ds.Tables[0].Rows[ctr][2] + " </td><td>" + ds.Tables[0].Rows[ctr][3] + " </td><td>" + ds.Tables[0].Rows[ctr][4] + " </td><td>" + ds.Tables[0].Rows[ctr][5] + " </td><td>" + ds.Tables[0].Rows[ctr][6] + " </td><td>" + ds.Tables[0].Rows[ctr][7] + " </td><td>" + ds.Tables[0].Rows[ctr][8] + " </td><td>" + ds.Tables[0].Rows[ctr][9] + " </td><td>" + ds.Tables[0].Rows[ctr][10] + " </td><td>" + ds.Tables[0].Rows[ctr][11] + " </td></tr>");
                    }
                    else if (client_code == "4")
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr][0] + " </td><td>" + ds.Tables[0].Rows[ctr][1] + " </td><td>" + ds.Tables[0].Rows[ctr][2] + " </td><td>" + ds.Tables[0].Rows[ctr][3] + " </td><td>" + ds.Tables[0].Rows[ctr][4] + " </td><td>" + ds.Tables[0].Rows[ctr][5] + " </td><td>" + ds.Tables[0].Rows[ctr][6] + " </td><td>" + ds.Tables[0].Rows[ctr][7] + " </td><td>" + ds.Tables[0].Rows[ctr][8] + " </td><td>" + ds.Tables[0].Rows[ctr][9] + " </td><td>" + ds.Tables[0].Rows[ctr][10] + " </td><td>" + ds.Tables[0].Rows[ctr][11] + " </td><td>" + ds.Tables[0].Rows[ctr][12] + " </td><td>" + ds.Tables[0].Rows[ctr][13] + " </td><td>" + ds.Tables[0].Rows[ctr][14] + " </td><td>" + ds.Tables[0].Rows[ctr][15] + " </td><td>" + ds.Tables[0].Rows[ctr][16] + " </td></tr>");
                    }
                    else if (client_code == "RLIC HK")
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr][0] + " </td><td>" + ds.Tables[0].Rows[ctr][1] + " </td><td>" + ds.Tables[0].Rows[ctr][2] + " </td><td>" + ds.Tables[0].Rows[ctr][3] + " </td><td>" + ds.Tables[0].Rows[ctr][4] + " </td><td>" + ds.Tables[0].Rows[ctr][5] + " </td><td>" + ds.Tables[0].Rows[ctr][6] + " </td><td>" + ds.Tables[0].Rows[ctr][7] + " </td><td>" + ds.Tables[0].Rows[ctr][8] + " </td><td>" + ds.Tables[0].Rows[ctr][9] + " </td><td>" + ds.Tables[0].Rows[ctr][10] + " </td></tr>");
                    }
                    else if (client_code == "HDFC")
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr][0] + " </td><td>" + ds.Tables[0].Rows[ctr][1] + " </td><td>" + ds.Tables[0].Rows[ctr][2] + " </td><td>" + ds.Tables[0].Rows[ctr][3] + " </td><td>" + ds.Tables[0].Rows[ctr][4] + " </td><td>" + ds.Tables[0].Rows[ctr][5] + " </td><td>" + ds.Tables[0].Rows[ctr][6] + " </td><td>" + ds.Tables[0].Rows[ctr][7] + " </td><td>" + ds.Tables[0].Rows[ctr][8] + " </td><td>" + ds.Tables[0].Rows[ctr][9] + " </td><td>" + ds.Tables[0].Rows[ctr][10] + " </td><td>" + ds.Tables[0].Rows[ctr][11] + " </td><td>" + ds.Tables[0].Rows[ctr][12] + " </td><td>" + ds.Tables[0].Rows[ctr][13] + " </td><td>" + ds.Tables[0].Rows[ctr][14] + " </td></tr>");
                    }
                    else if (client_code == "RBL")
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr][0] + " </td><td>" + ds.Tables[0].Rows[ctr][1] + " </td><td>" + ds.Tables[0].Rows[ctr][2] + " </td><td>" + ds.Tables[0].Rows[ctr][3] + " </td><td>" + ds.Tables[0].Rows[ctr][4] + " </td><td>" + ds.Tables[0].Rows[ctr][5] + " </td><td>" + ds.Tables[0].Rows[ctr][6] + " </td><td>" + ds.Tables[0].Rows[ctr][7] + " </td><td>" + ds.Tables[0].Rows[ctr][8] + " </td><td>" + ds.Tables[0].Rows[ctr][9] + " </td><td>" + ds.Tables[0].Rows[ctr][10] + " </td><td>" + ds.Tables[0].Rows[ctr][11] + " </td><td>" + ds.Tables[0].Rows[ctr][12] + " </td><td>" + ds.Tables[0].Rows[ctr][13] + " </td></tr>");
                    }
                    else if (client_code.Contains("BAG") || client_code == "BG")//BAGIC
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr][0] + " </td><td>" + ds.Tables[0].Rows[ctr][1] + " </td><td>" + ds.Tables[0].Rows[ctr][2] + " </td><td>" + ds.Tables[0].Rows[ctr][3] + " </td><td>" + ds.Tables[0].Rows[ctr][4] + " </td><td>" + ds.Tables[0].Rows[ctr][5] + " </td><td>" + ds.Tables[0].Rows[ctr][6] + " </td><td>" + ds.Tables[0].Rows[ctr][7] + " </td><td>" + ds.Tables[0].Rows[ctr][8] + " </td><td>" + ds.Tables[0].Rows[ctr][9] + " </td><td>" + ds.Tables[0].Rows[ctr][10] + " </td><td>" + ds.Tables[0].Rows[ctr][11] + " </td></tr>");
                    }
                    else if (client_code == "DHFL")
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr][0] + " </td><td>" + ds.Tables[0].Rows[ctr][1] + " </td><td>" + ds.Tables[0].Rows[ctr][2] + " </td><td>" + ds.Tables[0].Rows[ctr][3] + " </td><td>" + ds.Tables[0].Rows[ctr][4] + " </td><td>" + ds.Tables[0].Rows[ctr][5] + " </td><td>" + ds.Tables[0].Rows[ctr][6] + " </td><td>" + ds.Tables[0].Rows[ctr][7] + " </td><td>" + ds.Tables[0].Rows[ctr][8] + " </td><td>" + ds.Tables[0].Rows[ctr][9] + " </td><td>" + ds.Tables[0].Rows[ctr][10] + " </td><td>" + ds.Tables[0].Rows[ctr][11] + " </td><td>" + ds.Tables[0].Rows[ctr][12] + " </td><td>" + ds.Tables[0].Rows[ctr][13] + " </td><td>" + ds.Tables[0].Rows[ctr][14] + " </td><td>" + ds.Tables[0].Rows[ctr][15] + " </td><td>" + ds.Tables[0].Rows[ctr][16] + " </td><td>" + ds.Tables[0].Rows[ctr][17] + " </td><td>" + ds.Tables[0].Rows[ctr][18] + " </td><td>" + ds.Tables[0].Rows[ctr][19] + " </td></tr>");
                    }
                    else if (client_code == "7")
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr][0] + " </td><td>" + ds.Tables[0].Rows[ctr][1] + " </td><td>" + ds.Tables[0].Rows[ctr][2] + " </td><td>" + ds.Tables[0].Rows[ctr][3] + " </td><td>" + ds.Tables[0].Rows[ctr][4] + " </td><td>" + ds.Tables[0].Rows[ctr][5] + " </td><td>" + ds.Tables[0].Rows[ctr][6] + " </td></tr>");
                    }
                    else if (client_code == "BRLI")
                    {
                        lc = new LiteralControl("<tr><td>" + ds.Tables[0].Rows[ctr][0] + " </td><td>" + ds.Tables[0].Rows[ctr][1] + " </td><td>" + ds.Tables[0].Rows[ctr][2] + " </td><td>" + ds.Tables[0].Rows[ctr][3] + " </td><td>" + ds.Tables[0].Rows[ctr][4] + " </td><td>" + ds.Tables[0].Rows[ctr][5] + " </td><td>" + ds.Tables[0].Rows[ctr][6] + " </td><td>" + ds.Tables[0].Rows[ctr][7] + " </td><td>" + ds.Tables[0].Rows[ctr][8] + " </td><td>" + ds.Tables[0].Rows[ctr][9] + " </td><td>" + ds.Tables[0].Rows[ctr][10] + " </td><td>" + ds.Tables[0].Rows[ctr][11] + " </td><td>" + ds.Tables[0].Rows[ctr][12] + " </td></tr>");
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
        export_xl(1);
    }

    private void export_xl(int i)
    {

        string sql = null;

        if (i == 1)
        {
            string where = " WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND client_code = '" + ddl_upload_lg_client.SelectedValue + "' ";
            if (ddl_upload_lg_client.SelectedValue.Contains("BAG") || ddl_upload_lg_client.SelectedValue == "BG")//BAGIC
            {
                where = " WHERE comp_code = '" + Session["COMP_CODE"].ToString() + "' AND client_code like '%BAG%' or client_code = 'BG'";
            }
            if (ddl_upload_lg_client.SelectedValue.Equals("4"))
            {
                sql = "SELECT client_name, state_name, month, year, invoice_no, DATE_FORMAT(invoice_date, '%d/%m/%Y'), '', type, ROUND(amount, 2) AS 'amount', ROUND(cgst, 2) AS 'cgst', ROUND(sgst, 2) AS 'sgst', ROUND(igst, 2) AS 'igst', ROUND((cgst + sgst + igst), 2) AS 'total_gst', ROUND((cgst + sgst + igst + amount), 2) AS 'total_amount', ROUND((cgst + sgst + igst + amount)-(amount * (if(comp_code='C02',0.01,0.02))), 2) AS 'total_payment', DATE_FORMAT(payment_date, '%d/%m/%Y') AS 'Payment_date', ROUND(payment, 2) AS 'payment', ROUND(IF(payment = 0, 0, ((cgst + sgst + igst + amount) - payment-(amount * (if(comp_code='C02',0.01,0.02))))), 2) AS 'query_amount', ROUND(IF(payment != 0, 0, ((cgst + sgst + igst + amount) - payment)), 2) AS 'outstanding_amount', ROUND(tds_deduction, 2) AS 'tds_deduction', ROUND(tds_deducted, 2) AS 'tds_deducted', ROUND((tds_deduction - tds_deducted), 2) AS 'tds_differance' FROM pay_report_gst " + where + " ORDER BY 5,3 ";
            }
            else
            {
                sql = "SELECT client_name, state_name, month, year, invoice_no, DATE_FORMAT(invoice_date, '%d/%m/%Y'), '', type, ROUND(amount, 2) AS 'amount', ROUND(cgst, 2) AS 'cgst', ROUND(sgst, 2) AS 'sgst', ROUND(igst, 2) AS 'igst', ROUND((cgst + sgst + igst), 2) AS 'total_gst', ROUND((cgst + sgst + igst + amount), 2) AS 'total_amount', ROUND((cgst + sgst + igst + amount) - (amount * (if(comp_code='C02',0.01,0.02))), 2) AS 'total_payment', DATE_FORMAT(payment_date, '%d/%m/%Y') AS 'Payment_date', ROUND(payment, 2) AS 'payment', ROUND(IF(payment = 0, 0, ((cgst + sgst + igst + amount) - payment - (amount * (if(comp_code='C02',0.01,0.02))))), 2) AS 'query_amount', ROUND(IF(payment != 0, 0, ((cgst + sgst + igst + amount) - payment- (amount * (if(comp_code='C02',0.01,0.02))))), 2) AS 'outstanding_amount', ROUND(tds_deduction, 2) AS 'tds_deduction', ROUND(tds_deducted, 2) AS 'tds_deducted', ROUND((tds_deduction - tds_deducted), 2) AS 'tds_differance' FROM pay_report_gst " + where + " ORDER BY 5,3 ";
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
                Response.AddHeader("content-disposition", "attachment;filename=LEDGER_REPORT.xls");
            }

            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Repeater Repeater1 = new Repeater();
            Repeater1.DataSource = ds;
            Repeater1.HeaderTemplate = new MyTemplate12(ListItemType.Header, ds, i);
            Repeater1.ItemTemplate = new MyTemplate12(ListItemType.Item, ds, i);
            Repeater1.FooterTemplate = new MyTemplate12(ListItemType.Footer, ds, i);
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

    public class MyTemplate12 : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;
        static int ctr1;
        int i;
        string emp_type;
        int i3 = 1;
        private ListItemType listItemType;

        public MyTemplate12(ListItemType type, DataSet ds, int i)
        {
            // TODO: Complete member initialization
            this.type = type;
            this.ds = ds;
            this.i = i;

        }
        public void InstantiateIn(Control container)
        {
            switch (type)
            {
                case ListItemType.Header:

                    if (i == 1)
                    {
                        lc = new LiteralControl("<table border=1><tr><th bgcolor=yellow colspan=23 align=center> LEDGER REPORT </th></tr><table border=1><tr><th>SR. NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>MONTH</th><th>YEAR</th><th>INVOICE NO</th><th>INVOICE DATE</th><th>INVOICE PERIOD</th><th>SERVICE CATEGORY</th><th>SUBTOTAL</th><th>CGST</th><th>SGST</th><th>IGST</th><th>TOTAL GST</th><th>GRAND TOTAL AMOUNT</th><th>TOTAL PAYMENT</th><th>PAYMENT DATE</th><th>PAYMENT</th><th>QUERY AMOUNT</th><th>OUTSTANDING AMOUNT</th><th>TDS DEDUCTION</th><th>TDS DEDUCTED</th><th>TDS DIFFERANCE</th></tr>");
                    }

                    break;
                case ListItemType.Item:
                    if (i == 1)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr][0].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][1].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][2].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][3].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][4].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][5].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][6].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][7].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][8].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][9].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][10].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][11].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][12].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][13].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][14].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][15].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][16].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][17].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][18].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][19].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][20].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr][21].ToString().ToUpper() + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 9>Total</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(R3:R" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(S3:S" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(T3:T" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(U3:U" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(V3:V" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(W3:W" + (ctr + 3) + "),2)</td></b></tr>";
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
    protected void payment_gv()
    {
        hidtab.Value = "5";
        d.con.Open();
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        try
        {
            DataSet ds_status = new DataSet();
            MySqlDataAdapter dt_status = new MySqlDataAdapter("select ID,batchid,state_name,invoice_no,invoice_date,Amount,payment,payment_date,transaction_ref,tds_deducted,remarks from pay_report_gst where client_code='" + ddl_upload_lg_client.SelectedValue + "' ORDER BY id DESC ", d.con);
            // MySqlDataAdapter dt_status = new MySqlDataAdapter("select ID,pay_employee_master.emp_name, CONCAT('Washroom & Pantry Cleaning Condition ? :- ', answer1) AS 'que_1_ans',CONCAT('Training & Grooming including uniform ? :-', answer2) AS 'que_2_ans',CONCAT('Status of /cleaning/dusting of service & store room ? :-', answer3) AS 'que_3_ans', CONCAT('Maintain HK Staff job card and Supervisor visit ? :-', answer4) AS 'que_4_ans',CONCAT('Check for 5S Store Setup ? :-', answer5) AS 'que_5_ans', CONCAT('Deep Cleaning og office on every Saturday ? :-', answer6) AS 'que_6_ans',CONCAT('Reporting of office hygiene & pest Control ? :-', answer7) AS 'que_7_ans',CONCAT('Meeting with Client ? :-', answer8) AS 'que_8_ans',CONCAT('Compliance Management ? :-', answer9) AS 'que_9_ans',CONCAT('Dusting cleaning of workstation,windows,doors etc./Dusy Bins Condition/Cleaning Material supply ? :-', answer10) AS 'que_10_ans',pay_service_rating.remark,(CASE flag   WHEN 0 THEN 'Pending' WHEN 1 THEN 'Approved' WHEN 2 THEN 'Reject' ELSE '' END) AS 'Status' from pay_service_rating  inner join pay_employee_master on pay_service_rating.comp_code=pay_employee_master.comp_code and pay_service_rating.emp_code=pay_employee_master.emp_code  WHERE pay_service_rating.emp_code = '" + dd1_super.SelectedValue + "' AND pay_service_rating.client_code = '" + ddl_client.SelectedValue + "' AND pay_service_rating.unit_code = '" + ddl_unit.SelectedValue + "'  ORDER BY id DESC", d.con);
            dt_status.Fill(ds_status);
            if (ds_status.Tables[0].Rows.Count > 0)
            {
                payment_gridview.DataSource = ds_status;
                payment_gridview.DataBind();
            }
        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }
    }

    protected void payment_gridview_PreRender(object sender, EventArgs e)
    {
        try

        {
            payment_gridview.UseAccessibleHeader = false;
            payment_gridview.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
    protected void btn_show_Click(object sender, EventArgs e)
    {
       // payment_gv();
    }
    protected void ddl_upload_lg_client_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "5";
        payment_gv();
    }
    protected void payment_gridview_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "5";
        submit_btn.Visible = true;
        d.con.Open();
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        try
        {
            ViewState["invoice_no"] = payment_gridview.SelectedRow.Cells[3].Text;
            string batchid = payment_gridview.SelectedRow.Cells[1].Text;
            string state = payment_gridview.SelectedRow.Cells[2].Text;
           // string invoice = payment_gridview.SelectedRow.Cells[3].Text;
            string date_invoice = payment_gridview.SelectedRow.Cells[4].Text;
            string payment = payment_gridview.SelectedRow.Cells[5].Text;
            string date_payment = payment_gridview.SelectedRow.Cells[6].Text;
            string ref_no = payment_gridview.SelectedRow.Cells[7].Text;
            string deduction = payment_gridview.SelectedRow.Cells[8].Text;
            string remark = payment_gridview.SelectedRow.Cells[9].Text;

            MySqlCommand cmd = new MySqlCommand("select batchid,state_name,invoice_no,invoice_date,payment,payment_date,transaction_ref,tds_deducted,remarks from pay_report_gst where invoice_no= '" + ViewState["invoice_no"].ToString() + "'", d.con);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                txt_batchid.Text = dr.GetValue(0).ToString();
                txt_state.Text = dr.GetValue(1).ToString();
                txt_invoice.Text = dr.GetValue(2).ToString();
                txt_date_invoice.Text = dr.GetValue(3).ToString();
                txt_payment.Text = dr.GetValue(4).ToString();
                  txt_date_payment.Text = dr.GetValue(5).ToString();
                  txt_ref.Text = dr.GetValue(6).ToString();
                  txt_deduction.Text = dr.GetValue(7).ToString();
                  txt_remark.Text = dr.GetValue(8).ToString();
            }
            dr.Dispose();
            d.con.Close();

        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }
    }
    protected void payment_gridview_RowDataBound1(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.payment_gridview, "Select$" + e.Row.RowIndex);

        }
        e.Row.Cells[0].Visible = false;
    }
    protected void payment_gridview_PreRender1(object sender, EventArgs e)
    {

        try
        {
            payment_gridview.UseAccessibleHeader = false;
            payment_gridview.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
    protected void submit_btn_Click(object sender, EventArgs e)
    {
        try
        {
           // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            int result = 0;
            // string invoice = payment_gridview.SelectedRow.Cells[3].Text;
            result = d.operation("update pay_report_gst set payment='" + txt_payment.Text + "' , remarks='" + txt_remark.Text + "', batchid='" + txt_batchid.Text + "', transaction_ref='" + txt_ref.Text + "' where invoice_no='" + ViewState["invoice_no"].ToString() + "'");
            if (result > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record update successfully');", true);
            }
            payment_gv();
            txt_batchid.Text = "";
            txt_state.Text = "";
            txt_invoice.Text = "";
            txt_date_invoice.Text = "";
            txt_payment.Text = "";
            txt_date_payment.Text = "";
            txt_ref.Text = "";
            txt_deduction.Text = "";
            txt_remark.Text = "";
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            
        }
    }
    protected void close_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }

    protected void txt_date_SelectedIndexChanged(object sender, EventArgs e)
    {
        //load_client_amount();
        txt_date_changes();
    }

    private void txt_date_changes()
    {
        ddl_client_resive_amt.Items.Clear();
        try
        {
            string where_client = "";

            //if (ddl_client.SelectedValue == "TATA STEEL LTD" || ddl_client.SelectedValue == "TATA STEELS PVT LTD")
            //{
            //    where_client = " pay_minibank_master.client_code = '7'  ";
            //}
            //else //if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
            //{
            //    where_client = " pay_minibank_master.client_name = '" + ddl_client.SelectedValue + "'  ";
            //}

            if (ddl_client.SelectedValue == "TATA STEEL LTD" || ddl_client.SelectedValue == "TATA STEELS PVT LTD")
            {
                where_client = "  pay_minibank_master.client_code = '7'  ";
            }
            else if (ddl_client.SelectedValue == "EQUITAS SMALL FINANCE BANK" || ddl_client.SelectedValue == "Equitas Small Finance Bank Limited")
            {
                where_client = "  pay_minibank_master.client_code  IN ('ESFB','EquitasRes' ) ";
            }
            else if (ddl_client.SelectedValue == "TATA AIA Life Insurance Company Limited" || ddl_client.SelectedValue == "TATA AIA Life Insurance Company Limited Tem")
            {
                where_client = "  pay_minibank_master.client_code  IN ('TAIL','TAILTEMP' ) ";
            }
            else if (ddl_client.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD." || ddl_client.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD. RM")
            {
                where_client = "  pay_minibank_master.client_code  IN ('RLIC HK','RNLIC RM' ) ";
            }
            else
            {
                where_client = "  pay_minibank_master.client_name = '" + ddl_client.SelectedValue + "'  ";
            }


            
            
            string client_code_rd = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_client.SelectedValue + "' limit 1");


            DataTable dt_item = new DataTable();
            ddl_client_resive_amt.Items.Clear();
         //   MySqlDataAdapter cmd_item = new MySqlDataAdapter("select Id,amount from ( SELECT pay_minibank_master.id AS 'Id',pay_minibank_master.amount   FROM pay_minibank_master LEFT JOIN pay_report_gst ON pay_minibank_master.id = pay_report_gst.payment_id AND pay_minibank_master.CLIENT_CODE = pay_report_gst.CLIENT_CODE WHERE pay_minibank_master.receive_date = str_to_date('" + txt_date.Text + "', '%d-%m-%Y') AND pay_minibank_master.client_code = '" + client_code_rd + "' AND pay_minibank_master.client_name = '" + ddl_client.SelectedValue + "' AND `receipt_approve` != '0' and   ROUND(pay_minibank_master.Amount -  ((IFNULL((select SUM(a.received_amt) from pay_report_gst a where a.payment_id=pay_minibank_master.id) , 0)   +  (IFNULL((select SUM(b.received_amt2) from pay_report_gst b where b.payment_id2=pay_minibank_master.id) , 0)) +   (IFNULL((select SUM(c.received_amt3) from pay_report_gst c where c.payment_id3=pay_minibank_master.id) , 0) ))  ), 2) >0.99  GROUP BY pay_minibank_master.id, pay_report_gst.payment_id)  as t1 where  amount > 0 ORDER BY amount  ", d.con);
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("select Id,amount from ( SELECT pay_minibank_master.id AS 'Id',pay_minibank_master.amount   FROM pay_minibank_master LEFT JOIN pay_report_gst ON pay_minibank_master.id = pay_report_gst.payment_id AND pay_minibank_master.CLIENT_CODE = pay_report_gst.CLIENT_CODE WHERE pay_minibank_master.receive_date = str_to_date('" + txt_date.Text + "', '%d-%m-%Y') AND pay_minibank_master.client_code = '" + client_code_rd + "' AND   " + where_client + "   AND `receipt_approve` != '0' and   ROUND(pay_minibank_master.Amount -  ((IFNULL((select SUM(a.received_amt) from pay_report_gst a where a.payment_id=pay_minibank_master.id) , 0)   +  (IFNULL((select SUM(b.received_amt2) from pay_report_gst b where b.payment_id2=pay_minibank_master.id) , 0)) +   (IFNULL((select SUM(c.received_amt3) from pay_report_gst c where c.payment_id3=pay_minibank_master.id) , 0) ))  ), 2) >0.99  GROUP BY pay_minibank_master.id, pay_report_gst.payment_id)  as t1 where  amount > 0 ORDER BY amount  ", d.con);
            d.con.Open();

            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_client_resive_amt.DataSource = dt_item;
                ddl_client_resive_amt.DataValueField = dt_item.Columns[0].ToString();
                ddl_client_resive_amt.DataTextField = dt_item.Columns[1].ToString();
                ddl_client_resive_amt.DataBind();
            }
            //ddl_client_resive_amt.Items.Insert(0, "Select");
            dt_item.Dispose();
            d.con.Close();


            try
            {
                process_operation();
                Check_setteled_amt();
            }
            catch { }



        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            d.con.Close();
        }
    }
    protected void ddl_mode_transfer_SelectedIndexChanged(object sender, EventArgs e)
    {
       
        d.con.Open();
        try
        {
            if (ddl_mode_transfer.SelectedValue == "Cheque")
            {
                cheque.Visible = true;
                utr_no.Visible = false;
            }
            else
            {
                cheque.Visible = false;
                utr_no.Visible = true;
            }
        }
        catch (Exception ex)
        {
            d.con.Close();
            throw ex;
        }
        finally
        {

        }

    }
    protected void upload_file(string id)
    {
        string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");


        if (photo_upload.HasFile)
        {

            string fileExt = System.IO.Path.GetExtension(photo_upload.FileName);
            if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".RAR" || fileExt.ToUpper() == ".ZIP" || fileExt.ToUpper() == ".XLSX" || fileExt.ToUpper() == ".XLS" || fileExt.ToUpper() == ".DOCX" || fileExt.ToUpper() == ".DOC")
            {
                string fileName = Path.GetFileName(photo_upload.PostedFile.FileName);
                photo_upload.PostedFile.SaveAs(Server.MapPath("~/Account_images/") + fileName);
                // string id = d.getsinglestring("select ifnull(max(id),0) from pay_debit_master ");

             //   string file_name = ddl_minibank_client.SelectedValue + id + fileExt;
                string file_name = client_code + id + fileExt;

                File.Copy(Server.MapPath("~/Account_images/") + fileName, Server.MapPath("~/Account_images/") + file_name, true);
                File.Delete(Server.MapPath("~/Account_images/") + fileName);



                d.operation("update pay_minibank_master set  Upload_file='" + file_name + "', uploaded_by='" + Session["LOGIN_ID"].ToString() + "', uploaded_date=now()  where comp_code='" + Session["COMP_CODE"].ToString() + "' and id='"+id+"' ");
                // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Payment Uploaded Succsefully!!');", true);


            }

            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select only JPG, PNG , XLSX, XLS and PDF  Files  !!');", true);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please select only JPG, PNG and PDF Files !!!')", true);
                return;
            }


        }
    }
    protected void lnk_download_Command(object sender, CommandEventArgs e)
    {
        //string filePath = (sender as LinkButton).CommandArgument;
        //Response.ContentType = ContentType;
        //Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
        //Response.WriteFile(filePath);
        //Response.End();

        string filename = e.CommandArgument.ToString();


        if (filename != "")
        {
            string path2 = Server.MapPath("~\\Account_images\\" + filename);

            Response.Clear();
            Response.ContentType = "Application/pdf/jpg/jpeg/png/zip/xls/xlsx";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename);
            Response.TransmitFile("~\\Account_images\\" + filename);
            Response.WriteFile(path2);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Response.End();
            Response.Close();
        }

    }
    protected void ddl_payment_type_SelectedIndexChanged(object sender, EventArgs e)
    {
        d.con.Close();
        d.con.Open();
        try
        {
            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
          //  bank_name_ac_no();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);

        }
       
    }
    //protected void Bank_name()
    //{
    //    try
    //    {
    //    d.con.Open();
    //    MySqlCommand cmd_item1 = new MySqlCommand("Select Field1, Field2 from pay_zone_master where comp_code='" + Session["COMP_CODE"].ToString() + "' and Type = 'bank_details' and Field1='" + ddl_other_bank .SelectedValue+ "'", d.con);
    //   // MySqlCommand cmd_item1 = new MySqlCommand("Select comp_bank_name, comp_acc_no from pay_client_master where comp_code='" + Session["COMP_CODE"].ToString() + "' and comp_bank_name='" + ddl_other_bank.SelectedValue + "'", d.con);
    //    MySqlDataReader dr = cmd_item1.ExecuteReader();
    //    while (dr.Read())
    //    { 
    //        ddl_bank_name.SelectedValue=dr.GetValue(0).ToString();
    //        ddl_comp_ac_number.Text = dr.GetValue(1).ToString();
        
    //    }
    //        d.con.Close();
    //        //ddl_other_bank.Items.Insert(0, "Select");
           
    //    }
    //    catch (Exception ex) { throw ex; }
    //    finally
    //    {
    //        d.con.Close();
    //    }
    //}
    //protected void ddl_other_bank_SelectedIndexChanged(object sender, EventArgs e)
    //{
        // ddl_bank_name.Text = "";
        // lit_comp_ac_number.Text = "";
        // Bank_name();

    //}

    // company bank details komal 23-04-2020
    //protected void comp_bank_details(string client_code)
    //{

    //    MySqlDataAdapter comp_bank = null;
    //    System.Data.DataTable dt_item = new System.Data.DataTable();

    //    comp_bank = new MySqlDataAdapter("select distinct payment_ag_bank from pay_company_bank_details where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "'", d.con);
    //    d.con.Open();
    //    try
    //    {
    //        comp_bank.Fill(dt_item);
    //        if (dt_item.Rows.Count > 0)
    //        {
    //            ddl_bank_name.DataSource = dt_item;
    //            ddl_bank_name.DataTextField = dt_item.Columns[0].ToString();
    //            ddl_bank_name.DataValueField = dt_item.Columns[0].ToString();
    //            ddl_bank_name.DataBind();
    //        }
    //    }
    //    catch (Exception ex) { throw ex; }
    //    finally { d.con.Close(); }
    //    account_no(client_code);
    //}
    //protected void ddl_bank_name_SelectedIndexChanged(object sender, EventArgs e)
    //{

    //    //  ddl_bank_name.SelectedValue = "";
    //   string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");

    //    ddl_comp_ac_number.Text = "";
    //    account_no(client_code);
    //}

    //protected void account_no(string client_code)
    //{
    //    try
    //    {
    //        d.con.Open();
    //        //ddl_bank_name.Text = "";
    //        ddl_comp_ac_number.Text = "";
    //        MySqlCommand cmd = null;
    //        if (ddl_pmt_recived.SelectedValue == "0")
    //        {
    //            cmd = new MySqlCommand("Select payment_ag_bank, company_ac_no from pay_company_bank_details where comp_code='" + Session["comp_code"].ToString() + "' and client_code = '" + client_code + "' and payment_ag_bank = '" + ddl_bank_name.SelectedValue + "'", d.con);
    //        }
    //        MySqlDataReader dr = cmd.ExecuteReader();
    //        if (dr.Read())
    //        {
    //            ddl_bank_name.SelectedValue = dr.GetValue(0).ToString();
    //            ddl_comp_ac_number.Text = dr.GetValue(1).ToString();
    //        }

    //    }
    //    catch (Exception ex) { throw ex; }
    //    finally { d.con.Close(); }

    //}
    //protected void gv_minibank_SelectedIndexChanged1(object sender, EventArgs e)
    //  {

    // }
    protected void lnk_remove_manual_other_Click(object sender, EventArgs e)
    {
        int result = 0, result1 = 0;
        try
        {
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;

            result = d.operation("UPDATE pay_report_gst SET billing_amt = 0, received_amt = 0, `receipt_de_reasons`='',`receipt_de_approve`='0', tds_amount = 0, adjustment_amt = 0, adjustment_sign = 0, received_date = NULL, total_received_amt = 0, payment_id = 0, uploaded_by = NULL, uploaded_date = NULL,received_original_amount= 0 WHERE  Invoice_No = '" + grdrow.Cells[7].Text + "'");
            result1 = d.operation("delete  from pay_report_gst where Invoice_No = '" + grdrow.Cells[7].Text + "' and id= '" + grdrow.Cells[2].Text + "' and amount = 0 and (igst= 0 || cgst=0 || igst= 0 )");
            if (result > 0 || result1 > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Deleted Succsefully !!!')", true);
                load_gv_payment("");
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Deletion Failed !!!')", true);
                load_gv_payment("");
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {

        }
    }

    protected void client_name()
    {

        ddl_client_gv.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
      //  MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', client_code from pay_client_master where comp_code='" + Session["comp_code"] + "' and client_active_close='0' ORDER BY client_code", d.con);
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("select CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', client_code  from pay_report_gst  where  comp_code='" + Session["comp_code"] + "' and client_code is not NULL and client_name is not NULL and client_code NOT like 'OM%'  group by client_name order by client_name", d.con);
     
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_client_gv.DataSource = dt_item;
                ddl_client_gv.DataTextField = dt_item.Columns[0].ToString();//dt_item.Columns[0].ToString();
                ddl_client_gv.DataValueField = dt_item.Columns[0].ToString();
                ddl_client_gv.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
            ddl_client_gv.Items.Insert(0, "ALL");
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    //protected void ddl_client_gv_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
    //    load_gv_payment("");
    //}
    //protected void ddl_type_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
    //    load_gv_payment("");
    //}
    protected void btn_view_Click(object sender, EventArgs e)
    {
        try
        {
            hidtab.Value = "1";
            load_gv_payment("");

        }
        catch (Exception ex)
        {

            throw ex;
        }

    }
    protected void btn_download_utr_inv_click(object sender, EventArgs e)
    {
        try
        {
            hidtab.Value = "1";
            string gv_where = " ";
            d.con1.Open();

            DataSet ds = new DataSet();
            MySqlDataAdapter adp2 = null;
            //MySqlDataAdapter adp1 = new MySqlDataAdapter("SELECT payment_history.Id, payment_history.client_code, payment_history.comp_code, DATE_FORMAT(payment_history.billing_date, '%d/%m/%Y') AS 'Bill Date', payment_history.Invoice_No AS 'Invoice No', payment_history.client_name AS 'Client Name', payment_history.state_name AS 'State', payment_history.unit_name AS 'Branch', CONCAT(payment_history.month, '/', payment_history.year) AS 'MONTH', ROUND(payment_history.taxable_amount, 2) AS 'Taxable Amount', ROUND(payment_history.GST_Amount, 2) AS 'GST', ROUND(payment_history.billing_amt) AS 'Bill Amount', IFNULL(SUM(ROUND(pay_report_gst.received_amt + tds_amount)), 0) AS 'Received Amount', pay_report_gst.tds_amount,(ROUND(payment_history.billing_amt) - IFNULL(ROUND(SUM(pay_report_gst.received_amt + tds_amount)), 0)) AS 'Balanced Amount',DATE_FORMAT( `pay_report_gst`.`received_date`, '%d/%m/%Y') AS 'Received date'  FROM payment_history  LEFT JOIN pay_report_gst ON payment_history.Invoice_No = pay_report_gst.Invoice_No  WHERE payment_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' " + where + " AND payment_history.invoice_flag = 2 GROUP BY payment_history.Invoice_No, payment_history.client_code ORDER BY Id", d.con1);
            if (ddl_client_gv.SelectedValue == "ALL")
            {
                if (ddl_type.SelectedValue == "1")
                {
                    gv_where = "  where  Balanced_Amount <= 0.99 ";
                }
                else if (ddl_type.SelectedValue == "2")
                {
                    gv_where = "  where  Balanced_Amount > 0.99 ";
                }
                else if (ddl_type.SelectedValue == "ALL")
                {
                    gv_where = " ";
                }
             //CONCAT(LEFT(MONTHNAME(STR_TO_DATE(month,'%m')),3),'-',year)
              //  adp2 = new MySqlDataAdapter("SELECT Id,client_code,Bill_Date,receipt_de_approve,Status,Invoice_No,Client_Name,State,Branch,MONTH,Taxable_Amount,GST,Bill_Amount,tds_amount,Receivable_Amount,received_amt as Received_amt1,received_amt2,received_amt3,ROUND((received_amt+received_amt2+received_amt3),2) as total_received,deduction_amt,Balanced_Amount,Received_date as Received_date1,Received_date2,Received_date3,Utr_no as Utr_no1,Utr_Amount as Utr_Amount1 ,Utr_no2, Utr_Amount2,Utr_no3, Utr_Amount3,Entry_by_user1,entry_By_date1,Entry_by_user2,entry_By_date2 ,remark, CASE WHEN Balanced_Amount > 0 AND (ROUND((received_amt + received_amt2), 2)) = 0 THEN 'Not Received' WHEN Balanced_Amount BETWEEN - 1 AND 1 THEN 'Received Done' WHEN Balanced_Amount >= - 5 AND Balanced_Amount <= - 1 THEN '1 to 5 (-) Extra Done Payment' WHEN Balanced_Amount >= 1 AND Balanced_Amount <= 5 THEN '1 to 5 Less Done Payment'  WHEN Balanced_Amount >= - 100 AND Balanced_Amount < - 5 THEN '5 to 100 (-) Extra Payment' WHEN Balanced_Amount > 5 AND Balanced_Amount <= 100 THEN '5 to 100 Less  Payment'   WHEN Balanced_Amount < - 100 THEN 'Extra Payment Received' WHEN Balanced_Amount > 100 THEN 'Less Payment Received' END AS Balance_Status ,payment_status,invoice_days as InvoiceDays   FROM  ( SELECT ROUND(IFNULL(pay_report_gst.deduction_amt,0),2) as deduction_amt,    case   WHEN pay_report_gst.received_amt3 >0 then pay_report_gst.remark3  WHEN pay_report_gst.received_amt2 >0 then pay_report_gst.remark2  else pay_report_gst.remark  END as remark, pay_report_gst.Id,pay_report_gst.client_code,pay_minibank_master.Utr_no,pay_minibank_master.Amount  as 'Utr_Amount',receipt_de_reasons AS 'Reject_Reason',  `receipt_de_approve`,  CASE WHEN `receipt_de_approve` = '0' THEN 'Pending' WHEN `receipt_de_approve` = '1'  THEN 'Approve By Jr Acc' WHEN `receipt_de_approve` = '2' THEN 'Approve By Sr Acc' WHEN `receipt_de_approve` = '3' THEN 'Rejected By Sr Acc' END AS 'Status',    DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Bill_Date',pay_report_gst.Invoice_No AS 'Invoice_No',pay_report_gst.client_name AS 'Client_Name',     pay_report_gst.state_name AS 'State',pay_report_gst.unit_code AS 'Branch',DATE_FORMAT(pay_report_gst.invoice_date,'%m-%Y') AS 'MONTH', ROUND((pay_report_gst.amount), 2)     AS 'Taxable_Amount',ROUND((`cgst` + `sgst` + `igst`), 2) AS 'GST',ROUND((pay_report_gst.amount + `cgst` + `sgst` + `igst`), 2) AS 'Bill_Amount',     IFNULL(ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`) - pay_report_gst.tds_amount),2), 0) AS 'Receivable_Amount',pay_report_gst.tds_amount, pay_report_gst.received_amt, pay_report_gst.received_amt2, pay_report_gst.received_amt3, DATE_FORMAT(pay_report_gst.received_date3, '%d/%m/%Y') AS 'Received_date3', mb3.Utr_no as Utr_no3,mb3.Amount  as 'Utr_amount3',  DATE_FORMAT(pay_report_gst.received_date2, '%d/%m/%Y') AS 'Received_date2',pay_minibank_master.Amount as UTR_amt1,  mb2.Utr_no as Utr_no2,mb2.Amount  as 'Utr_amount2',       ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`)-pay_report_gst.`tds_amount`) - IFNULL(ROUND((`pay_report_gst`.`received_amt` + `pay_report_gst`.`received_amt2`+ `pay_report_gst`.`received_amt3`),2),0)-IFNULL(`pay_report_gst`.`deduction_amt`,0), 2)      AS 'Balanced_Amount',    pay_report_gst.payment,DATE_FORMAT(pay_report_gst.received_date, '%d/%m/%Y') AS 'Received_date',CONCAT(pay_report_gst.uploaded_by, '-', pay_employee_master.EMP_NAME)      AS Entry_by_user1, DATE_FORMAT(pay_report_gst.uploaded_date, '%d/%m/%Y %H:%i:%s') AS entry_By_date1,  CONCAT(pay_report_gst.uploaded_by2, '-', e2.EMP_NAME) AS Entry_by_user2,      DATE_FORMAT(pay_report_gst.uploaded_date2, '%d/%m/%Y %H:%i:%s') AS entry_By_date2,pay_report_gst.payment_status as payment_status,datediff(now(),invoice_date) as invoice_days   FROM pay_report_gst     LEFT JOIN pay_employee_master ON pay_report_gst.uploaded_by = pay_employee_master.emp_code          LEFT JOIN pay_employee_master e2 ON pay_report_gst.uploaded_by2 = e2.emp_code    LEFT JOIN pay_minibank_master ON pay_report_gst.payment_id = pay_minibank_master.id AND pay_report_gst.comp_code = pay_minibank_master.comp_code  LEFT JOIN pay_minibank_master mb2 ON pay_report_gst.payment_id2 = mb2.id and pay_report_gst.comp_code = mb2.comp_code    LEFT JOIN pay_minibank_master mb3 ON pay_report_gst.payment_id3 = mb3.id and pay_report_gst.comp_code = mb3.comp_code  WHERE pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_report_gst.flag_invoice = 2  GROUP BY pay_report_gst.Invoice_No ,pay_report_gst.client_code ORDER BY Id) as t1  " + gv_where + "", d.con1);
                adp2 = new MySqlDataAdapter("SELECT Id,client_code,Bill_Date,receipt_de_approve,Status,Invoice_No,Client_Name,State,Branch,MONTH,Taxable_Amount,GST,Bill_Amount,tds_amount,Receivable_Amount,received_amt as Received_amt1,received_amt2,received_amt3,ROUND((received_amt+received_amt2+received_amt3),2) as total_received,deduction_amt,Balanced_Amount,Received_date as Received_date1,Received_date2,Received_date3,Utr_no as Utr_no1,Utr_Amount as Utr_Amount1 ,Utr_no2, Utr_Amount2,Utr_no3, Utr_Amount3,Entry_by_user1,entry_By_date1,Entry_by_user2,entry_By_date2 ,remark, CASE WHEN Balanced_Amount > 0 AND (ROUND((received_amt + received_amt2), 2)) = 0 THEN 'Not Received' WHEN Balanced_Amount BETWEEN - 1 AND 1 THEN 'Received Done' WHEN Balanced_Amount >= - 5 AND Balanced_Amount <= - 1 THEN '1 to 5 (-) Extra Done Payment' WHEN Balanced_Amount >= 1 AND Balanced_Amount <= 5 THEN '1 to 5 Less Done Payment'  WHEN Balanced_Amount >= - 100 AND Balanced_Amount < - 5 THEN '5 to 100 (-) Extra Payment' WHEN Balanced_Amount > 5 AND Balanced_Amount <= 100 THEN '5 to 100 Less  Payment'   WHEN Balanced_Amount < - 100 THEN 'Extra Payment Received' WHEN Balanced_Amount > 100 THEN 'Less Payment Received' END AS Balance_Status ,payment_status,invoice_days as InvoiceDays   FROM  ( SELECT ROUND(IFNULL(pay_report_gst.deduction_amt,0),2) as deduction_amt,    case   WHEN pay_report_gst.received_amt3 >0 then pay_report_gst.remark3  WHEN pay_report_gst.received_amt2 >0 then pay_report_gst.remark2  else pay_report_gst.remark  END as remark, pay_report_gst.Id,pay_report_gst.client_code,pay_minibank_master.Utr_no,pay_minibank_master.Amount  as 'Utr_Amount',receipt_de_reasons AS 'Reject_Reason',  `receipt_de_approve`,  CASE WHEN `receipt_de_approve` = '0' THEN 'Pending' WHEN `receipt_de_approve` = '1'  THEN 'Approve By Jr Acc' WHEN `receipt_de_approve` = '2' THEN 'Approve By Sr Acc' WHEN `receipt_de_approve` = '3' THEN 'Rejected By Sr Acc' END AS 'Status',    DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Bill_Date',pay_report_gst.Invoice_No AS 'Invoice_No',pay_report_gst.client_name AS 'Client_Name',     pay_report_gst.state_name AS 'State',pay_report_gst.unit_code AS 'Branch',DATE_FORMAT(pay_report_gst.invoice_date,'%b-%Y') AS 'MONTH', ROUND((pay_report_gst.amount), 2)     AS 'Taxable_Amount',ROUND((`cgst` + `sgst` + `igst`), 2) AS 'GST',ROUND((pay_report_gst.amount + `cgst` + `sgst` + `igst`), 2) AS 'Bill_Amount',     IFNULL(ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`) - pay_report_gst.tds_amount),2), 0) AS 'Receivable_Amount',pay_report_gst.tds_amount, pay_report_gst.received_amt, pay_report_gst.received_amt2, pay_report_gst.received_amt3, DATE_FORMAT(pay_report_gst.received_date3, '%d/%m/%Y') AS 'Received_date3', mb3.Utr_no as Utr_no3,mb3.Amount  as 'Utr_amount3',  DATE_FORMAT(pay_report_gst.received_date2, '%d/%m/%Y') AS 'Received_date2',pay_minibank_master.Amount as UTR_amt1,  mb2.Utr_no as Utr_no2,mb2.Amount  as 'Utr_amount2',       ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`)-pay_report_gst.`tds_amount`) - IFNULL(ROUND((`pay_report_gst`.`received_amt` + `pay_report_gst`.`received_amt2`+ `pay_report_gst`.`received_amt3`),2),0)-IFNULL(`pay_report_gst`.`deduction_amt`,0), 2)      AS 'Balanced_Amount',    pay_report_gst.payment,DATE_FORMAT(pay_report_gst.received_date, '%d/%m/%Y') AS 'Received_date',CONCAT(pay_report_gst.uploaded_by, '-', pay_employee_master.EMP_NAME)      AS Entry_by_user1, DATE_FORMAT(pay_report_gst.uploaded_date, '%d/%m/%Y %H:%i:%s') AS entry_By_date1,  CONCAT(pay_report_gst.uploaded_by2, '-', e2.EMP_NAME) AS Entry_by_user2,      DATE_FORMAT(pay_report_gst.uploaded_date2, '%d/%m/%Y %H:%i:%s') AS entry_By_date2,pay_report_gst.payment_status as payment_status,datediff(now(),invoice_date) as invoice_days   FROM pay_report_gst     LEFT JOIN pay_employee_master ON pay_report_gst.uploaded_by = pay_employee_master.emp_code          LEFT JOIN pay_employee_master e2 ON pay_report_gst.uploaded_by2 = e2.emp_code    LEFT JOIN pay_minibank_master ON pay_report_gst.payment_id = pay_minibank_master.id AND pay_report_gst.comp_code = pay_minibank_master.comp_code  LEFT JOIN pay_minibank_master mb2 ON pay_report_gst.payment_id2 = mb2.id and pay_report_gst.comp_code = mb2.comp_code    LEFT JOIN pay_minibank_master mb3 ON pay_report_gst.payment_id3 = mb3.id and pay_report_gst.comp_code = mb3.comp_code  WHERE pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_report_gst.flag_invoice = 2  GROUP BY pay_report_gst.Invoice_No ,pay_report_gst.client_code ORDER BY Id) as t1  " + gv_where + "", d.con1);
            }
            else if (ddl_client_gv.SelectedValue != "ALL")
            {
                string where_client = "";

                //if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
                //{
                //    where_client = " and pay_report_gst.client_code = '7'  ";
                //}
                //else //if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
                //{
                //    where_client = " and pay_report_gst.client_name = '" + ddl_client_gv.SelectedValue + "'  ";
                //}



                if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
                {
                    where_client = " and pay_report_gst.client_code = '7'  ";
                }
                else if (ddl_client_gv.SelectedValue == "EQUITAS SMALL FINANCE BANK" || ddl_client_gv.SelectedValue == "Equitas Small Finance Bank Limited")
                {
                    where_client = " and pay_report_gst.client_code  IN ('ESFB','EquitasRes' ) ";
                }
                else if (ddl_client_gv.SelectedValue == "TATA AIA Life Insurance Company Limited" || ddl_client_gv.SelectedValue == "TATA AIA Life Insurance Company Limited Tem")
                {
                    where_client = " and pay_report_gst.client_code  IN ('TAIL','TAILTEMP' ) ";
                }
                else if (ddl_client_gv.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD." || ddl_client_gv.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD. RM")
                {
                    where_client = " and pay_report_gst.client_code  IN ('RLIC HK','RNLIC RM' ) ";
                }
                else
                {
                    where_client = " and pay_report_gst.client_name = '" + ddl_client_gv.SelectedValue + "'  ";
                }





                if (ddl_type.SelectedValue == "1")
                {
                    gv_where = "  where  Balanced_Amount <= 0.99 ";
                }
                else if (ddl_type.SelectedValue == "2")
                {
                    gv_where = "  where  Balanced_Amount > 0.99 ";
                }
                else if (ddl_type.SelectedValue == "ALL")
                {
                    gv_where = " ";
                }
              // adp2 = new MySqlDataAdapter("SELECT Id,client_code,Bill_Date,receipt_de_approve,Status,Invoice_No,Client_Name,State,Branch,MONTH,Taxable_Amount,GST,Bill_Amount,tds_amount,Receivable_Amount,received_amt as Received_amt1,received_amt2,received_amt3,ROUND((received_amt+received_amt2+received_amt3),2) as total_received,deduction_amt,Balanced_Amount,Received_date as Received_date1,Received_date2,Received_date3,Utr_no as Utr_no1,Utr_Amount as Utr_Amount1 ,Utr_no2, Utr_Amount2,Utr_no3, Utr_Amount3,Entry_by_user1,entry_By_date1,Entry_by_user2,entry_By_date2 ,remark, CASE WHEN Balanced_Amount > 0 AND (ROUND((received_amt + received_amt2), 2)) = 0 THEN 'Not Received' WHEN Balanced_Amount BETWEEN - 1 AND 1 THEN 'Received Done' WHEN Balanced_Amount >= - 5 AND Balanced_Amount <= - 1 THEN '1 to 5 (-) Extra Done Payment' WHEN Balanced_Amount >= 1 AND Balanced_Amount <= 5 THEN '1 to 5 Less Done Payment'  WHEN Balanced_Amount >= - 100 AND Balanced_Amount < - 5 THEN '5 to 100 (-) Extra Payment' WHEN Balanced_Amount > 5 AND Balanced_Amount <= 100 THEN '5 to 100 Less  Payment'   WHEN Balanced_Amount < - 100 THEN 'Extra Payment Received' WHEN Balanced_Amount > 100 THEN 'Less Payment Received' END AS Balance_Status ,payment_status,invoice_days as InvoiceDays   FROM  ( SELECT ROUND(IFNULL(pay_report_gst.deduction_amt,0),2) as deduction_amt,    case   WHEN pay_report_gst.received_amt3 >0 then pay_report_gst.remark3  WHEN pay_report_gst.received_amt2 >0 then pay_report_gst.remark2  else pay_report_gst.remark  END as remark, pay_report_gst.Id,pay_report_gst.client_code,pay_minibank_master.Utr_no,pay_minibank_master.Amount  as 'Utr_Amount',receipt_de_reasons AS 'Reject_Reason',  `receipt_de_approve`,  CASE WHEN `receipt_de_approve` = '0' THEN 'Pending' WHEN `receipt_de_approve` = '1'  THEN 'Approve By Jr Acc' WHEN `receipt_de_approve` = '2' THEN 'Approve By Sr Acc' WHEN `receipt_de_approve` = '3' THEN 'Rejected By Sr Acc' END AS 'Status',    DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Bill_Date',pay_report_gst.Invoice_No AS 'Invoice_No',pay_report_gst.client_name AS 'Client_Name',     pay_report_gst.state_name AS 'State',pay_report_gst.unit_code AS 'Branch',DATE_FORMAT(pay_report_gst.invoice_date,'%m-%Y') AS 'MONTH', ROUND((pay_report_gst.amount), 2)     AS 'Taxable_Amount',ROUND((`cgst` + `sgst` + `igst`), 2) AS 'GST',ROUND((pay_report_gst.amount + `cgst` + `sgst` + `igst`), 2) AS 'Bill_Amount',     IFNULL(ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`) - pay_report_gst.tds_amount),2), 0) AS 'Receivable_Amount',pay_report_gst.tds_amount, pay_report_gst.received_amt, pay_report_gst.received_amt2, pay_report_gst.received_amt3, DATE_FORMAT(pay_report_gst.received_date3, '%d/%m/%Y') AS 'Received_date3', mb3.Utr_no as Utr_no3,mb3.Amount  as 'Utr_amount3',  DATE_FORMAT(pay_report_gst.received_date2, '%d/%m/%Y') AS 'Received_date2',pay_minibank_master.Amount as UTR_amt1,  mb2.Utr_no as Utr_no2,mb2.Amount  as 'Utr_amount2',       ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`)-pay_report_gst.`tds_amount`) - IFNULL(ROUND((`pay_report_gst`.`received_amt` + `pay_report_gst`.`received_amt2`+ `pay_report_gst`.`received_amt3`),2),0)-IFNULL(`pay_report_gst`.`deduction_amt`,0), 2)      AS 'Balanced_Amount',    pay_report_gst.payment,DATE_FORMAT(pay_report_gst.received_date, '%d/%m/%Y') AS 'Received_date',CONCAT(pay_report_gst.uploaded_by, '-', pay_employee_master.EMP_NAME)      AS Entry_by_user1, DATE_FORMAT(pay_report_gst.uploaded_date, '%d/%m/%Y %H:%i:%s') AS entry_By_date1,  CONCAT(pay_report_gst.uploaded_by2, '-', e2.EMP_NAME) AS Entry_by_user2,      DATE_FORMAT(pay_report_gst.uploaded_date2, '%d/%m/%Y %H:%i:%s') AS entry_By_date2,pay_report_gst.payment_status as payment_status,datediff(now(),invoice_date) as invoice_days   FROM pay_report_gst     LEFT JOIN pay_employee_master ON pay_report_gst.uploaded_by = pay_employee_master.emp_code          LEFT JOIN pay_employee_master e2 ON pay_report_gst.uploaded_by2 = e2.emp_code    LEFT JOIN pay_minibank_master ON pay_report_gst.payment_id = pay_minibank_master.id AND pay_report_gst.comp_code = pay_minibank_master.comp_code  LEFT JOIN pay_minibank_master mb2 ON pay_report_gst.payment_id2 = mb2.id and pay_report_gst.comp_code = mb2.comp_code    LEFT JOIN pay_minibank_master mb3 ON pay_report_gst.payment_id3 = mb3.id and pay_report_gst.comp_code = mb3.comp_code     WHERE pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "'  " + where_client + "    AND pay_report_gst.flag_invoice = 2  GROUP BY pay_report_gst.Invoice_No ,pay_report_gst.client_code ORDER BY Id) as t1  " + gv_where + "", d.con1);
                adp2 = new MySqlDataAdapter("SELECT Id,client_code,Bill_Date,receipt_de_approve,Status,Invoice_No,Client_Name,State,Branch,MONTH,Taxable_Amount,GST,Bill_Amount,tds_amount,Receivable_Amount,received_amt as Received_amt1,received_amt2,received_amt3,ROUND((received_amt+received_amt2+received_amt3),2) as total_received,deduction_amt,Balanced_Amount,Received_date as Received_date1,Received_date2,Received_date3,Utr_no as Utr_no1,Utr_Amount as Utr_Amount1 ,Utr_no2, Utr_Amount2,Utr_no3, Utr_Amount3,Entry_by_user1,entry_By_date1,Entry_by_user2,entry_By_date2 ,remark, CASE WHEN Balanced_Amount > 0 AND (ROUND((received_amt + received_amt2), 2)) = 0 THEN 'Not Received' WHEN Balanced_Amount BETWEEN - 1 AND 1 THEN 'Received Done' WHEN Balanced_Amount >= - 5 AND Balanced_Amount <= - 1 THEN '1 to 5 (-) Extra Done Payment' WHEN Balanced_Amount >= 1 AND Balanced_Amount <= 5 THEN '1 to 5 Less Done Payment'  WHEN Balanced_Amount >= - 100 AND Balanced_Amount < - 5 THEN '5 to 100 (-) Extra Payment' WHEN Balanced_Amount > 5 AND Balanced_Amount <= 100 THEN '5 to 100 Less  Payment'   WHEN Balanced_Amount < - 100 THEN 'Extra Payment Received' WHEN Balanced_Amount > 100 THEN 'Less Payment Received' END AS Balance_Status ,payment_status,invoice_days as InvoiceDays   FROM  ( SELECT ROUND(IFNULL(pay_report_gst.deduction_amt,0),2) as deduction_amt,    case   WHEN pay_report_gst.received_amt3 >0 then pay_report_gst.remark3  WHEN pay_report_gst.received_amt2 >0 then pay_report_gst.remark2  else pay_report_gst.remark  END as remark, pay_report_gst.Id,pay_report_gst.client_code,pay_minibank_master.Utr_no,pay_minibank_master.Amount  as 'Utr_Amount',receipt_de_reasons AS 'Reject_Reason',  `receipt_de_approve`,  CASE WHEN `receipt_de_approve` = '0' THEN 'Pending' WHEN `receipt_de_approve` = '1'  THEN 'Approve By Jr Acc' WHEN `receipt_de_approve` = '2' THEN 'Approve By Sr Acc' WHEN `receipt_de_approve` = '3' THEN 'Rejected By Sr Acc' END AS 'Status',    DATE_FORMAT(pay_report_gst.invoice_date, '%d/%m/%Y') AS 'Bill_Date',pay_report_gst.Invoice_No AS 'Invoice_No',pay_report_gst.client_name AS 'Client_Name',     pay_report_gst.state_name AS 'State',pay_report_gst.unit_code AS 'Branch',DATE_FORMAT(pay_report_gst.invoice_date,'%b-%Y') AS 'MONTH', ROUND((pay_report_gst.amount), 2)     AS 'Taxable_Amount',ROUND((`cgst` + `sgst` + `igst`), 2) AS 'GST',ROUND((pay_report_gst.amount + `cgst` + `sgst` + `igst`), 2) AS 'Bill_Amount',     IFNULL(ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`) - pay_report_gst.tds_amount),2), 0) AS 'Receivable_Amount',pay_report_gst.tds_amount, pay_report_gst.received_amt, pay_report_gst.received_amt2, pay_report_gst.received_amt3, DATE_FORMAT(pay_report_gst.received_date3, '%d/%m/%Y') AS 'Received_date3', mb3.Utr_no as Utr_no3,mb3.Amount  as 'Utr_amount3',  DATE_FORMAT(pay_report_gst.received_date2, '%d/%m/%Y') AS 'Received_date2',pay_minibank_master.Amount as UTR_amt1,  mb2.Utr_no as Utr_no2,mb2.Amount  as 'Utr_amount2',       ROUND(((pay_report_gst.amount + `cgst` + `sgst` + `igst`)-pay_report_gst.`tds_amount`) - IFNULL(ROUND((`pay_report_gst`.`received_amt` + `pay_report_gst`.`received_amt2`+ `pay_report_gst`.`received_amt3`),2),0)-IFNULL(`pay_report_gst`.`deduction_amt`,0), 2)      AS 'Balanced_Amount',    pay_report_gst.payment,DATE_FORMAT(pay_report_gst.received_date, '%d/%m/%Y') AS 'Received_date',CONCAT(pay_report_gst.uploaded_by, '-', pay_employee_master.EMP_NAME)      AS Entry_by_user1, DATE_FORMAT(pay_report_gst.uploaded_date, '%d/%m/%Y %H:%i:%s') AS entry_By_date1,  CONCAT(pay_report_gst.uploaded_by2, '-', e2.EMP_NAME) AS Entry_by_user2,      DATE_FORMAT(pay_report_gst.uploaded_date2, '%d/%m/%Y %H:%i:%s') AS entry_By_date2,pay_report_gst.payment_status as payment_status,datediff(now(),invoice_date) as invoice_days   FROM pay_report_gst     LEFT JOIN pay_employee_master ON pay_report_gst.uploaded_by = pay_employee_master.emp_code          LEFT JOIN pay_employee_master e2 ON pay_report_gst.uploaded_by2 = e2.emp_code    LEFT JOIN pay_minibank_master ON pay_report_gst.payment_id = pay_minibank_master.id AND pay_report_gst.comp_code = pay_minibank_master.comp_code  LEFT JOIN pay_minibank_master mb2 ON pay_report_gst.payment_id2 = mb2.id and pay_report_gst.comp_code = mb2.comp_code    LEFT JOIN pay_minibank_master mb3 ON pay_report_gst.payment_id3 = mb3.id and pay_report_gst.comp_code = mb3.comp_code     WHERE pay_report_gst.comp_code = '" + Session["COMP_CODE"].ToString() + "'  " + where_client + "    AND pay_report_gst.flag_invoice = 2  GROUP BY pay_report_gst.Invoice_No ,pay_report_gst.client_code ORDER BY Id) as t1  " + gv_where + "", d.con1);

            }
            adp2.SelectCommand.CommandTimeout = 200;
            adp2.Fill(ds);

           
            if (ds.Tables[0].Rows.Count > 0)
            {
                try
                {

                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment;filename=Receipt_DetailReport1.xls");
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";
                    Repeater Repeater1 = new Repeater();
                    Repeater1.DataSource = ds;
                    Repeater1.HeaderTemplate = new MyTemplate_utrdetail(ListItemType.Header, ds);
                    Repeater1.ItemTemplate = new MyTemplate_utrdetail(ListItemType.Item, ds);
                    Repeater1.FooterTemplate = new MyTemplate_utrdetail(ListItemType.Footer, ds);
                    Repeater1.DataBind();
                    System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                    System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                    Repeater1.RenderControl(htmlWrite);
                    string style = @"<style> .textmode { } </style>";
                    Response.Write(style);
                    Response.Output.Write(stringWrite.ToString());
                    Response.Flush();
                    Response.End();
                   // HttpContext.Current.ApplicationInstance.CompleteRequest();

                }
                catch (System.Threading.ThreadAbortException ex)
                {
                   // throw ex;
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Matching Records Found.');", true);
            }
        }
        catch
        {
            //  throw ex;
        }
        hidtab.Value = "1";

    }

    public class MyTemplate_utrdetail : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;

        public MyTemplate_utrdetail(ListItemType type, DataSet ds)
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

                    lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=33>Receipt Detail Report</th></tr><tr><th>SR NO.</th><th>Bill Date</th><th>Status</th><th>Invoice No</th><th>Client Name</th><th>State</th><th>Month</th><th>Taxable Amount</th><th>GST</th><th>Bill Amount</th><th>TDS Amount</th> <th>Receivable Amount</th><th>Received Amt 1</th><th>Received Amt 2</th><th>Received Amt 3</th><th>Total Received</th><th>Deduction Amt</th><th>Balanced Amount</th><th>Received Date1</th> <th>Received Date2</th> <th>Received Date3</th><th>UTR No 1</th><th>UTR Amt 1</th><th>UTR No 2</th><th>UTR Amt 2</th><th>UTR No 3</th><th>UTR Amt 3</th><th>Entry By User1</th><th>Entry By Date1</th><th>Entry By User2</th><th>Entry By Date2</th><th>Remark</th>  <th> Balance_Status </th><th>Payment Status</th><th> InvoiceDays </th></tr> ");  // <th>Deduction Amt</th>

                    break;
                case ListItemType.Item:
                    //                                                                                                                                               invoice_no, Client_Name,                                                                                                                State, Branch,                                                      MONTH, Taxable_Amount,                                                                                  GST,                                             Bill_Amount,                                        tds_amount,                                          Receivable_Amount,                                          Received_amt1,                                          received_amt2,                                          total_received,                                          Balanced_Amount,                                          Received_date1,                                          Received_date2,                                           utr_no1,                                          utr_amount1,                                          utr_no2,                                          utr_Amount2,                                          Entry_by_user1,                                          entry_By_date1,                                          Entry_by_user2,                                          entry_By_date2,                                          deduction_amt,                                          remark
                    lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["Bill_Date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Status"] + "</td><td>" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_Name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["State"] + "</td><td>" + ds.Tables[0].Rows[ctr]["MONTH"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Taxable_Amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["GST"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Bill_Amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["tds_amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Receivable_Amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Received_amt1"] + "</td><td>" + ds.Tables[0].Rows[ctr]["received_amt2"] + "</td><td>" + ds.Tables[0].Rows[ctr]["received_amt3"] + "</td><td>" + ds.Tables[0].Rows[ctr]["total_received"] + "</td><td>" + ds.Tables[0].Rows[ctr]["deduction_amt"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Balanced_Amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Received_date1"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Received_date2"] + "</td> <td>" + ds.Tables[0].Rows[ctr]["Received_date3"] + "</td> <td>" + ds.Tables[0].Rows[ctr]["utr_no1"] + "</td><td>" + ds.Tables[0].Rows[ctr]["utr_amount1"] + "</td><td>" + ds.Tables[0].Rows[ctr]["utr_no2"] + "</td><td>" + ds.Tables[0].Rows[ctr]["utr_Amount2"] + "</td><td>" + ds.Tables[0].Rows[ctr]["utr_no3"] + "</td><td>" + ds.Tables[0].Rows[ctr]["utr_Amount3"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Entry_by_user1"] + "</td><td>" + ds.Tables[0].Rows[ctr]["entry_By_date1"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Entry_by_user2"] + "</td><td>" + ds.Tables[0].Rows[ctr]["entry_By_date2"] + "</td><td>" + ds.Tables[0].Rows[ctr]["remark"] + "</td> <td>" + ds.Tables[0].Rows[ctr]["Balance_Status"] + "</td> <td>" + ds.Tables[0].Rows[ctr]["payment_status"] + "</td><td>" + ds.Tables[0].Rows[ctr]["InvoiceDays"] + "</td> </tr>"); //<td>" + ds.Tables[0].Rows[ctr]["deduction_amt"] + "</td>payment_status
                    //}
                    //if (counter == 1)
                    //{
                        //if (ds.Tables[0].Rows.Count == ctr + 1)
                        //{
                        //    lc.Text = lc.Text + "<tr><b><td align=center colspan = 27>Total</td></tr>";//<td>=ROUND(SUM(F3:F" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td></b>
                        //}
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




    //protected void display_close_date()
    //{
    //    d.con.Open();
    //    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
    //    try
    //    {
    //        DataSet ds_status = new DataSet();
    //        // MySqlDataAdapter dt_status = new MySqlDataAdapter("SELECT client_name  ,receive_date,   REMANING_AMOUNT  FROM (SELECT pay_minibank_master  .  ID  , pay_minibank_master  .  client_name   AS 'client_Name', DATE_FORMAT(  receive_date  , '%d-%m-%Y') AS 'receive_date', Amount   AS 'Credit Amount', ROUND(IFNULL(SUM(  pay_report_gst  .  received_amt  ), 0), 2) AS ' SETTLED_AMOUNT', ROUND(  Amount   - (IFNULL(SUM(  pay_report_gst  .  received_amt  ), 0)), 2) AS 'REMANING_AMOUNT' FROM pay_minibank_master   LEFT JOIN   pay_report_gst  ON  pay_report_gst . payment_id  =  pay_minibank_master . id  WHERE pay_minibank_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' and `pay_minibank_master`.`client_code`='" + ddl_client.SelectedValue + "' GROUP BY pay_minibank_master . id ) AS t1 WHERE REMANING_AMOUNT  < 0.99", d.con);
    //        MySqlDataAdapter dt_status = new MySqlDataAdapter("select  client_name, DATE_FORMAT(`receive_date`, '%d-%m-%Y') as 'receive_date',REMANING_AMOUNT from(SELECT pay_minibank_master.ID,pay_minibank_master.client_name,receive_date, ROUND(pay_minibank_master.Amount - (IFNULL(SUM(pay_report_gst.received_amt), 0)), 2) AS 'REMANING_AMOUNT' FROM pay_minibank_master LEFT JOIN pay_report_gst ON pay_report_gst.payment_id = pay_minibank_master.id WHERE pay_minibank_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' and `pay_minibank_master`.`client_name`='" + ddl_client.SelectedValue + "' GROUP BY pay_minibank_master.receive_date) as t1 where REMANING_AMOUNT <0.99 ", d.con);
    //        dt_status.Fill(ds_status);
    //        if (ds_status.Tables[0].Rows.Count > 0)
    //        {
    //            gv_links.DataSource = ds_status;
    //            gv_links.DataBind();
    //        }
    //    }
    //    catch (Exception ex) { throw ex; }
    //    finally { d.con.Close(); }
    //}
    protected void btn_approve_minibank_Click(object sender, EventArgs e)
    {
        try
        {
            string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");


            string record_save = null;


            //if (ddl_pmt_recived.SelectedValue == "0")
            //{
            //    record_save = d.getsinglestring("select client_code,`Bank_name`,`Account_number`,`Amount` from pay_minibank_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + client_code + "' and bank_name = '" + lit_bank_name.Text + "' and `Account_number` = '" + lit_comp_ac_number.Text + "' and `client_bank_name` = '" + lit_client_bank.Text + "' and `client_ac_number` ='" + lit_client_ac_number.Text + "' and `Amount` ='" + txt_minibank_amount.Text + "' and `receive_date`= str_to_date('" + txt_minibank_received_date.Text + "','%d/%m/%Y') and `Mode_of_transfer` ='" + ddl_mode_transfer.SelectedValue + "' and `Utr_no`='" + txt_minibank_utr_no.Text + "' and uploaded_by = '" + Session["LOGIN_ID"].ToString() + "' and `payment_type` ='" + ddl_payment_type.SelectedValue + "' and `received_from` = '0' ");

            //    if (record_save == "")
            //    {
            //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Submit Record First')", true);
            //        return;
            //    }
            //}
            //else

            //    if (ddl_pmt_recived.SelectedValue == "1")
            //    {
            //        record_save = d.getsinglestring("select `Account_number`,`Amount`,`receive_date`,`description`,`uploaded_by`  from pay_minibank_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_name = '" + ddl_other.SelectedValue + "'  and `Account_number` = '" + lit_comp_ac_number.Text + "'  and `Amount` ='" + txt_minibank_amount.Text + "' and `receive_date`=str_to_date('" + txt_minibank_received_date.Text + "','%d/%m/%Y') and `Mode_of_transfer` ='" + ddl_mode_transfer.SelectedValue + "' and `Utr_no`='" + txt_minibank_utr_no.Text + "' and uploaded_by = '" + Session["LOGIN_ID"].ToString() + "' and `received_from` = '1' ");

            //        if (record_save == "")
            //        {
            //            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Submit Record First')", true);
            //            return;
            //        }

            //    }



            int result = 0;

            if (ddl_pmt_recived.SelectedValue == "0")
            {
                foreach (GridViewRow row in gv_add_utr.Rows)
                {
                    int sr_number = int.Parse(((Label)row.FindControl("lbl_srnumber")).Text);
                    string receipt_date_gv = row.Cells[2].Text;
                    string amount_gv = row.Cells[3].Text;
                    string utr_no_gv = row.Cells[4].Text;
                    string payment_mode_gv = row.Cells[5].Text;
                    string payment_against_gv = row.Cells[6].Text;
                    string remark_gv = row.Cells[7].Text;
                    //string cheque_no = "";

                    result = d.operation("update pay_minibank_master set receipt_approve = '1' , receipt_reasons ='' where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + client_code + "' and bank_name = '" + lit_bank_name.Text + "' and Account_number = '" + lit_comp_ac_number.Text + "' and client_bank_name = '" + lit_client_bank.Text + "' and client_ac_number ='" + lit_client_ac_number.Text + "' and `Amount` ='" + amount_gv + "' and `receive_date`= str_to_date('" + receipt_date_gv + "','%d/%m/%Y') and `Mode_of_transfer` ='" + payment_mode_gv + "' and `Utr_no`='" + utr_no_gv + "' and uploaded_by = '" + Session["LOGIN_ID"].ToString() + "' ");//and payment_type ='" + ddl_payment_type.SelectedValue + "' 
                }
                 }

            else if (ddl_pmt_recived.SelectedValue == "1")
            {
                //comp_code,client_name,Bank_name,Account_number,receive_date,description,Amount,uploaded_by,uploaded_date,Mode_of_transfer,Cheque,Utr_no
                foreach (GridViewRow row in gv_add_utr.Rows)
                {
                    int sr_number = int.Parse(((Label)row.FindControl("lbl_srnumber")).Text);
                    string receipt_date_gv = row.Cells[2].Text;
                    string amount_gv = row.Cells[3].Text;
                    string utr_no_gv = row.Cells[4].Text;
                    string payment_mode_gv = row.Cells[5].Text;
                    string payment_against_gv = row.Cells[6].Text;
                    string remark_gv = row.Cells[7].Text;
                    //string cheque_no = "";
                    result = d.operation("update pay_minibank_master set receipt_approve = '1' ,receipt_reasons = '' where comp_code = '" + Session["comp_code"].ToString() + "' and client_name = '" + ddl_other.SelectedValue + "'  and Account_number = '" + lit_comp_ac_number.Text + "'  and Amount ='" + amount_gv + "' and receive_date =str_to_date('" + receipt_date_gv + "','%d/%m/%Y') and Mode_of_transfer ='" + payment_mode_gv + "' and Utr_no ='" + utr_no_gv + "' and uploaded_by = '" + Session["LOGIN_ID"].ToString() + "'  ");
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Approved Succesfully !!');", true);

           // comp_data();
            mini_text_clear();

          //  ddl_pmt_recived.SelectedValue = "Select";
           // ddl_minibank_client.SelectedValue = "Select";
            ddl_other_bank.Items.Clear();
            ddl_payment_type.SelectedValue = "Select";
            gv_add_utr.DataSource = null;
            gv_add_utr.DataBind();
            pnl_bank_details.Visible = false;
            lit_bank_name.Text = "";
            lit_client_ac_number.Text = "";
            lit_client_bank.Text = "";
            lit_comp_ac_number.Text = "";
            btn_approve_minibank.Visible = false;
            Button1.Visible = true;
            btn_row.Visible = false;

        }
        catch (Exception ex) { throw ex; }
        finally { }
    }
    protected void btn_approve_receipt_de_Click(object sender, EventArgs e)
    {
        try
        {
            hidtab.Value = "1";

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            string inlist = "";

            foreach (GridViewRow gvrow in gv_invoice_pmt.Rows)
            {

                // string emp_code = (string)gv_checklist_uniform.DataKeys[gvrow.RowIndex].Value;
                string invoice_no = gv_invoice_pmt.Rows[gvrow.RowIndex].Cells[1].Text;

                TextBox txt_returnqty = (TextBox)gvrow.FindControl("txt_recive_amt");
                string receive_amt = (txt_returnqty.Text);

                TextBox receive_date1 = (TextBox)gvrow.FindControl("txt_reciving_date");
                string receive_date = (receive_date1.Text);

                double txt_received_amt1 = double.Parse(((TextBox)gvrow.FindControl("txt_received_amt1")).Text);
                double txt_received_amt2 = double.Parse(((TextBox)gvrow.FindControl("txt_received_amt2")).Text);

                

                //var checkbox = gvrow.FindControl("chk_client") as System.Web.UI.WebControls.CheckBox;
                //if (checkbox.Checked == true)
                //{

                //    inlist = "" + invoice_no+ "";
                //}

                if (txt_received_amt1 > 0 && txt_received_amt2 == 0)
                {
                    string receipt_details = d.getsinglestring("select distinct invoice_no from pay_report_gst where comp_code = '" + Session["comp_code"].ToString() + "' and `Invoice_No`='" + invoice_no + "' and `received_amt` = '" + txt_received_amt1 + "'");
                    if (receipt_details == "")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Submit Record First')", true);
                        return;
                    }
                    int result = 0;
                    result = d.operation("update pay_report_gst set receipt_de_approve = '1',`receipt_de_reasons`='' where comp_code='" + Session["comp_code"].ToString() + "' and Invoice_No ='" + invoice_no + "' and received_amt = '" + txt_received_amt1 + "' and `received_date` = str_to_date('" + txt_date.SelectedValue + "','%d-%m-%Y') ");
                }
                else
                {
                    string receipt_details = d.getsinglestring("select distinct invoice_no from pay_report_gst where comp_code = '" + Session["comp_code"].ToString() + "' and `Invoice_No`='" + invoice_no + "' and `received_amt2` = '" + txt_received_amt2 + "'");
                    if (receipt_details == "")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Submit Record First')", true);
                        return;
                    }
                    int result = 0;
                    result = d.operation("update pay_report_gst set receipt_de_approve2 = '1',`receipt_de_reasons`='' where comp_code='" + Session["comp_code"].ToString() + "' and Invoice_No ='" + invoice_no + "' and received_amt2 = '" + txt_received_amt2 + "' and `received_date2` = str_to_date('" + txt_date.SelectedValue + "','%d-%m-%Y') ");
               }
            }


            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Record Approve  Successfully !!!')", true);
        
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            tran_clear();
            //panel2.Visible = true;
            Panel_gv_pmt.Visible = false;

            Bind_UTR_date();

            txt_date_changes();
            hidtab.Value = "1";


        }


    }

    //protected void client_bank_name()
    //{
    //    string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");

    //    System.Data.DataTable dt_item = new System.Data.DataTable();
    //    MySqlDataAdapter cmd_item = null;

    //    if (ddl_pmt_recived.SelectedValue == "1")
    //    {
    //        cmd_item = new MySqlDataAdapter("select client_bank_name from pay_other_client_master where client_code='" + client_code + "' ", d.con);
    //    }
    //    else
    //    {
    //        cmd_item = new MySqlDataAdapter("Select Field1 from pay_zone_master where comp_code='" + Session["comp_code"].ToString() + "' and Type = 'bank_details' and CLIENT_CODE ='" + client_code + "' ", d.con);
    //    }
    //    d.con.Open();
    //    try
    //    {
    //        cmd_item.Fill(dt_item);
    //        if (dt_item.Rows.Count > 0)
    //        {
    //            ddl_client_bank.DataSource = dt_item;
    //            ddl_client_bank.DataTextField = dt_item.Columns[0].ToString();
    //            ddl_client_bank.DataValueField = dt_item.Columns[0].ToString();
    //            ddl_client_bank.DataBind();
    //        }

    //        dt_item.Dispose();
    //        //    ddl_bank_name.Readonly=true;
    //        ddl_comp_ac_number.ReadOnly = true;
    //    }
    //    catch (Exception ex) { throw ex; }
    //    finally { d.con.Close(); }

    //    comp_bank_details(client_code);






    //}

    protected void btn_edit_other1_Click(object sender, EventArgs e)
    {
        MySqlDataReader dr2 = null;
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow.Cells[4].Text;

        btn_update_receipt.Visible = true;
        Button1.Visible = false;

        int result = 0;
        if (id!="")
      	{
            result = d.operation("update pay_minibank_master set receipt_approve = '1' where id = '" + id + "' ");
	    }

        if (result > 0)
        {
            View_utr_detail_grid();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Approved Succesfully !!');", true);
            //comp_data();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record Not Approved !!');", true);
            return;
        }



        //// for other edit 
        //if (received_from_to == "1")
        //{
        //    MySqlCommand cmd2 = new MySqlCommand("SELECT `received_from`, `client_name`, `description`,`amount`, DATE_FORMAT( receive_date , '%d/%m/%Y') as 'receive_date', `Mode_of_transfer`, `Utr_no`, id,DATE_FORMAT( payment_hit_date, '%d/%m/%Y') as 'payment_hit_date' FROM `pay_minibank_master` WHERE  `id` = '" + id + "'", d3.con);

        //    d3.con.Open();
        //    try
        //    {
        //        dr2 = cmd2.ExecuteReader();
        //        if (dr2.Read())
        //        {


        //            ddl_pmt_recived.SelectedValue = dr2.GetValue(0).ToString();
        //            ddl_pmt_recived_SelectedIndexChanged(null, null);

        //            ddl_other.SelectedValue = dr2.GetValue(1).ToString();
        //            txt_description.Text = dr2.GetValue(2).ToString();
        //            txt_minibank_amount.Text = dr2.GetValue(3).ToString();
        //            txt_minibank_received_date.Text = dr2.GetValue(4).ToString();
        //            ddl_mode_transfer.SelectedValue = dr2.GetValue(5).ToString();
        //            txt_minibank_utr_no.Text = dr2.GetValue(6).ToString();
        //            txt_id.Text = dr2.GetValue(7).ToString();
        //            //  txt_payment_hit_date.Text = dr2.GetValue(8).ToString();

        //        }
        //        dr2.Close();



        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        //    }
        //    catch (Exception ex) { throw ex; }
        //    finally { }
        //}
        //// for client edit
        //else if (received_from_to == "0")
        //{

        //    MySqlCommand cmd2 = new MySqlCommand("select distinct `received_from` ,client_code,`payment_type`,`client_bank_name`,`client_ac_number`,amount, DATE_FORMAT( receive_date , '%d/%m/%Y') as 'receive_date',`Mode_of_transfer`,`Utr_no`,id,DATE_FORMAT( payment_hit_date, '%d/%m/%Y') as 'payment_hit_date',client_name from pay_minibank_master  WHERE  `id` = '" + id + "'", d3.con);

        //    d3.con.Open();
        //    try
        //    {
        //        dr2 = cmd2.ExecuteReader();
        //        if (dr2.Read())
        //        {


        //            ddl_pmt_recived.SelectedValue = dr2.GetValue(0).ToString();
        //            ddl_pmt_recived_SelectedIndexChanged(null, null);

        //            ddl_minibank_client.SelectedValue = dr2.GetValue(11).ToString();
        //            // ddl_minibank_client_SelectedIndexChanged(null,null);

        //            // comp_bank_details();

        //            ddl_payment_type.SelectedValue = dr2.GetValue(2).ToString();
        //            ddl_payment_type_SelectedIndexChanged(null, null);
        //           // client_bank_name();
        //            //ddl_client_bank_SelectedIndexChanged(null, null);
        //            //ddl_client_bank.SelectedValue = dr2.GetValue(3).ToString();

        //            //ddl_client_ac_number.SelectedValue= dr2.GetValue(4).ToString();
        //            // bank_name_ac_no();
        //            txt_minibank_amount.Text = dr2.GetValue(5).ToString();
        //            txt_minibank_received_date.Text = dr2.GetValue(6).ToString();

        //            ddl_mode_transfer.SelectedValue = dr2.GetValue(7).ToString();
        //            txt_minibank_utr_no.Text = dr2.GetValue(8).ToString();
        //            txt_id.Text = dr2.GetValue(9).ToString();
        //            // txt_payment_hit_date.Text = dr2.GetValue(10).ToString();
        //        }
        //        dr2.Close();


        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        //    }
        //    catch (Exception ex) { throw ex; }
        //    finally { }
        //}
        ////Edit Code end

    }
    protected void btn_update_receipt_Click(object sender, EventArgs e)
    {
        try
        {
            string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");


            int result = 0;

            if (ddl_pmt_recived.SelectedValue == "0")
            {
                result = d.operation("update pay_minibank_master set received_from = '" + ddl_pmt_recived.SelectedValue + "', client_code='" + client_code + "' , `Bank_name` = '" + lit_bank_name.Text + "', `Account_number`='" + lit_comp_ac_number.Text + "',`payment_type` = '" + ddl_payment_type.SelectedValue + "',client_bank_name ='" + lit_client_bank.Text + "',`client_ac_number`='" + lit_client_ac_number.Text + "', amount = '" + txt_minibank_amount.Text + "',`receive_date` = str_to_date('" + txt_minibank_received_date.Text + "','%d/%m/%Y') , `Mode_of_transfer` ='" + ddl_mode_transfer.SelectedValue + "', `Utr_no`='" + txt_minibank_utr_no.Text.Trim() + "', uploaded_by = '" + Session["LOGIN_ID"].ToString() + "',payment_hit_date=str_to_date('" + txt_minibank_received_date.Text + "','%d/%m/%Y')  where id = '" + txt_id.Text + "' ");
            }

            else if (ddl_pmt_recived.SelectedValue == "1")
            {
                //comp_code,client_name,Bank_name,Account_number,receive_date,description,Amount,uploaded_by,uploaded_date,Mode_of_transfer,Cheque,Utr_no

                result = d.operation("update pay_minibank_master set client_name = '" + ddl_other.SelectedValue + "' , `Account_number` = '" + lit_comp_ac_number.Text + "' , `Amount` ='" + txt_minibank_amount.Text + "', `receive_date`=str_to_date('" + txt_minibank_received_date.Text + "','%d/%m/%Y'), `Mode_of_transfer` ='" + ddl_mode_transfer.SelectedValue + "' ,`Utr_no`='" + txt_minibank_utr_no.Text.Trim() + "', uploaded_by = '" + Session["LOGIN_ID"].ToString() + "',payment_hit_date=str_to_date('" + txt_minibank_received_date.Text + "','%d/%m/%Y')  where id = '" + txt_id.Text + "' ");
            }
            if (result > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record Updated Succesfully !!');", true);
                //comp_data();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record Not Updated !!');", true);
                return;
            }

            // mini_text_clear();


        }
        catch (Exception ex) { throw ex; }
        finally { }
    }

    protected void btn_utr_view_Click(object sender, EventArgs e)
    {
        View_utr_detail_grid();

    }

    private void View_utr_detail_grid()
    {
        hidtab.Value = "0";
        string query = "";
        string sql_where = "", where_status = "";
        try
        {
            string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");


            if (ddl_nockoff_status.SelectedValue != "All")
            {
                if (ddl_nockoff_status.SelectedValue == "Yes")
                {
                    where_status = "  where (received1+received2)>0 ";
                }
                else
                {
                    where_status = "  where (received1+received2)<1 ";
                }
            }

            if (txt_from_date.Text != "" && txt_to_date.Text != "")
            {
                if (ddl_minibank_client.SelectedIndex > 0)
                {
                    sql_where = "and m.client_code='" + client_code + "' and  m.client_name='" + ddl_minibank_client.SelectedValue + "'  AND  m.receive_date between str_to_date('" + txt_from_date.Text + "','%d/%m/%Y') and str_to_date('" + txt_to_date.Text + "','%d/%m/%Y')";
                }
                else
                {
                    sql_where = "and  m.receive_date between str_to_date('" + txt_from_date.Text + "','%d/%m/%Y') and str_to_date('" + txt_to_date.Text + "','%d/%m/%Y')";
                }
            }
            else
            {
                if (ddl_minibank_client.SelectedIndex > 0)
                {
                    sql_where = "and m.client_code='" + client_code + "' and  m.client_name='" + ddl_minibank_client.SelectedValue + "'    ";
                }
            }//utr_type,utr_balance
           //2 time receivedpayment balanc query
            //query = "select ID,Upload_file,receipt_approve, month,case when receipt_approve = '0' then 'Pending' when receipt_approve ='1' then 'Approve By Jr Acc' when receipt_approve ='2' then 'Approve By Sr Acc' when receipt_approve = '3' then 'Rejected By Sr Acc' end as 'Status', client_name, Receive_DATE, Utr_no, amount,(received1+received2) as setteled_amt,(amount-(received1+received2)) as utr_balance, IF((amount-(received1+received2))>0.99,'NO','Yes') as  Nockup_status, Comp_Bank_Name, Entry_Date, Entry_By,remark,utr_type  from (select DATE_FORMAT(m.receive_date,'%m-%Y') as month,m.client_name,DATE_FORMAT(m.receive_date,'%d/%m/%Y') as Receive_DATE,m.Utr_no,m.amount ,  Round(IFNULL(sum(g.received_amt),0),2) as received1,  IFNULL((select sum(received_amt2) from pay_report_gst where payment_id2=m.id),0) as received2,  m.Bank_name as Comp_Bank_Name, DATE_FORMAT(m.Uploaded_date,'%d/%m/%Y %h:%i') as Entry_Date,e.EMP_NAME as Entry_By ,m.remark ,m.description as utr_type,m.Upload_file , m.ID,receipt_approve    from pay_minibank_master m    left join pay_report_gst g on m.id=g.payment_id   left join pay_employee_master e on m.uploaded_by=e.EMP_CODE   where m.comp_code= '" + Session["COMP_CODE"].ToString() + "' " + sql_where + "group by m.id order by m.client_name,m.receive_date ) as t1  " + where_status + "";
            query = "select ID,Upload_file,receipt_approve, month,case when receipt_approve = '0' then 'Pending' when receipt_approve ='1' then 'Approve By Jr Acc' when receipt_approve ='2' then 'Approve By Sr Acc' when receipt_approve = '3' then 'Rejected By Sr Acc' end as 'Status', client_name, Receive_DATE, Utr_no, amount, (received1+received2+received3) as setteled_amt, (amount-(received1+received2+received3)) as utr_balance, IF((amount-(received1+received2+received3))>0.99,'NO','Yes') as  Nockoff_Status, Comp_Bank_Name, Entry_Date, Entry_By,remark,utr_type  from (select DATE_FORMAT(m.receive_date,'%m-%Y') as month,m.client_name,DATE_FORMAT(m.receive_date,'%d/%m/%Y') as Receive_DATE,m.Utr_no,m.amount ,  Round(IFNULL(sum(g.received_amt),0),2) as received1,  IFNULL((select sum(received_amt2) from pay_report_gst where payment_id2=m.id),0) as received2, IFNULL((select sum(received_amt3) from pay_report_gst where payment_id3=m.id),0) as received3,   m.Bank_name as Comp_Bank_Name, DATE_FORMAT(m.Uploaded_date,'%d/%m/%Y %h:%i') as Entry_Date,e.EMP_NAME as Entry_By ,m.remark ,m.description as utr_type,m.Upload_file , m.ID,receipt_approve    from pay_minibank_master m    left join pay_report_gst g on m.id=g.payment_id   left join pay_employee_master e on m.uploaded_by=e.EMP_CODE   where m.comp_code= '" + Session["COMP_CODE"].ToString() + "' " + sql_where + "group by m.id order by m.client_name,m.receive_date ) as t1  " + where_status + "";
            

            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();

            dscmd.SelectCommand.CommandTimeout = 400;
            dscmd.Fill(ds);

            gv_minibank.DataSource = ds.Tables[0];
            gv_minibank.DataBind();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }


    protected void btn_UTR_report_Click(object sender, EventArgs e)
    {
        all_utr();

    }
    protected void all_utr()
    {
        hidtab.Value = "0";
        string query = "";
        int counter = 1;
        string sql_where = "", where_status = "";
        try
        {
            string client_code = d.getsinglestring("select client_code from pay_report_gst where client_name = '" + ddl_minibank_client.SelectedValue + "' limit 1");


            if (ddl_nockoff_status.SelectedValue != "All")
            {
                if (ddl_nockoff_status.SelectedValue == "Yes")
                {
                    where_status = "  where (received1+received2)>0 order by client_name,month, Receive_DATE";
                }
                else
                {
                    where_status = "  where (received1+received2)<1 order by client_name,month, Receive_DATE ";
                }
            }

            if (txt_from_date.Text != "" && txt_to_date.Text != "")
            {
                if (ddl_minibank_client.SelectedIndex > 0)
                {
                    sql_where = "and m.client_code='" + client_code + "' and  m.client_name='" + ddl_minibank_client.SelectedValue + "'  AND  m.receive_date between str_to_date('" + txt_from_date.Text + "','%d/%m/%Y') and str_to_date('" + txt_to_date.Text + "','%d/%m/%Y')";
                }
                else
                {
                    sql_where = "and  m.receive_date between str_to_date('" + txt_from_date.Text + "','%d/%m/%Y') and str_to_date('" + txt_to_date.Text + "','%d/%m/%Y')";
                }
            }
            else
            {
                if (ddl_minibank_client.SelectedIndex > 0)
                {
                    sql_where = "and m.client_code='" + client_code + "' and  m.client_name='" + ddl_minibank_client.SelectedValue + "'    ";
                }
            }//utr_type,utr_balance
            //vishal-2-time payment received balance report query
           // query = "select month, client_name, Receive_DATE, Utr_no, amount,(received1+received2) as setteled_amt,(amount-(received1+received2)) as utr_balance, IF((amount-(received1+received2))>0.99,'NO','Yes') as  Nockup_status, Comp_Bank_Name, Entry_Date, Entry_By,remark,utr_type  from ( select DATE_FORMAT(m.receive_date,'%m-%Y') as month,m.client_name,DATE_FORMAT(m.receive_date,'%d/%m/%Y') as Receive_DATE,m.Utr_no,m.amount ,  Round(IFNULL(sum(g.received_amt),0),2) as received1,  IFNULL((select sum(received_amt2) from pay_report_gst where payment_id2=m.id),0) as received2,  m.Bank_name as Comp_Bank_Name, DATE_FORMAT(m.Uploaded_date,'%d/%m/%Y %h:%i') as Entry_Date,e.EMP_NAME as Entry_By ,m.remark ,m.description as utr_type  from pay_minibank_master m    left join pay_report_gst g on m.id=g.payment_id   left join pay_employee_master e on m.uploaded_by=e.EMP_CODE   where m.comp_code= '" + Session["COMP_CODE"].ToString() + "' " + sql_where + "group by m.id order by m.client_name,m.receive_date ) as t1  " + where_status + "";

            //vishal-3-time payment received balance report query
         //   query = "select month, client_name, Receive_DATE, Utr_no, amount,(received1+received2+received3) as setteled_amt,(amount-(received1+received2+received3)) as utr_balance,  IF((amount-(received1+received2+received3))>0.99,'NO','Yes') as  Nockup_status, Comp_Bank_Name, Entry_Date, Entry_By,remark,utr_type  from (  select DATE_FORMAT(m.receive_date,'%m-%Y') as month,m.client_name,DATE_FORMAT(m.receive_date,'%d/%m/%Y') as Receive_DATE,m.Utr_no,m.amount ,    Round(IFNULL(sum(g.received_amt),0),2) as received1,   IFNULL((select sum(received_amt2) from pay_report_gst where payment_id2=m.id),0) as received2,   IFNULL((select sum(g3.received_amt3) from pay_report_gst g3 where g3.payment_id3=m.id),0) as received3,  m.Bank_name as Comp_Bank_Name, DATE_FORMAT(m.Uploaded_date,'%d/%m/%Y %h:%i') as Entry_Date,e.EMP_NAME as Entry_By ,m.remark ,m.description as utr_type  from pay_minibank_master m    left join pay_report_gst g on m.id=g.payment_id   left join pay_employee_master e on m.uploaded_by=e.EMP_CODE   where m.comp_code= '" + Session["COMP_CODE"].ToString() + "' " + sql_where + "group by m.id order by m.client_name,m.receive_date ) as t1  " + where_status + "";


            query = "select month, client_name, Receive_DATE, Utr_no, amount,(received1+received2+received3) as setteled_amt,(amount-(received1+received2+received3)) as utr_balance,  IF((amount-(received1+received2+received3))>0.99,'NO','Yes') as  Nockup_status, Comp_Bank_Name, Entry_Date, Entry_By,remark,utr_type  from (  select DATE_FORMAT(m.receive_date,'%m-%Y') as month,m.client_name,DATE_FORMAT(m.receive_date,'%d/%m/%Y') as Receive_DATE,m.Utr_no,ROUND(m.amount,2) as amount,    Round(IFNULL(sum(ROUND(g.received_amt,2)),0),2) as received1,   IFNULL((select sum(ROUND(received_amt2,2)) from pay_report_gst where payment_id2=m.id),0) as received2,    IFNULL((select sum(ROUND(g3.received_amt3,2)) from pay_report_gst g3 where g3.payment_id3=m.id),0) as received3,   m.Bank_name as Comp_Bank_Name, DATE_FORMAT(m.Uploaded_date,'%d/%m/%Y %h:%i') as Entry_Date,e.EMP_NAME as Entry_By ,m.remark ,m.description as utr_type  from pay_minibank_master m    left join pay_report_gst g on m.id=g.payment_id   left join pay_employee_master e on m.uploaded_by=e.EMP_CODE   where m.comp_code= '" + Session["COMP_CODE"].ToString() + "' " + sql_where + "group by m.id order by m.client_name,m.receive_date ) as t1  " + where_status + "";

            //new button for sac wise gst report

            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();

            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);


            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                //existing gst report

                Response.AddHeader("content-disposition", "attachment;filename=UTR_Report" + ddl_client.SelectedItem.Text.Replace(" ", "_") + ".xls");
                //}
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
            {//utr_type,utr_balance
                case ListItemType.Header:
                    if (counter == 1)
                    {
                        lc = new LiteralControl("<table border=1><tr ><th bgcolor=yellow colspan=14>UTR Reports</th></tr><tr><th>SR NO.</th><th>Month</th><th>Client Name</th><th>Receive DATE</th><th>UTR NO</th><th>UTR Amount</th><th>Setteled Amount</th> <th>UTR Balance</th> <th>Nockoff Status</th><th>Company Bank Name</th><th>Entry Date</th><th>Entry By</th><th>Remark</th> <th>UTR Type</th></tr> ");
                    }

                    break;
                case ListItemType.Item:
                    if (counter == 1)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["month"] + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Receive_DATE"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Utr_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["setteled_amt"] + "</td> <td>" + ds.Tables[0].Rows[ctr]["utr_balance"] + "</td> <td>" + ds.Tables[0].Rows[ctr]["Nockup_status"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Comp_Bank_Name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Entry_Date"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Entry_By"] + "</td><td>" + ds.Tables[0].Rows[ctr]["remark"] + "</td><td>" + ds.Tables[0].Rows[ctr]["utr_type"] + "</td></tr>");
                    }
                    if (counter == 1)
                    {
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 5>Total</td><td>=ROUND(SUM(F3:F" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td> <td>=ROUND(SUM(H3:H" + (ctr + 3) + "),2)</td> </b> </tr>";
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



    protected void lbtn_utr_Click(object sender, EventArgs e)
    {
        if (pnl_utr_report.Visible == true)
        {
            pnl_utr_report.Visible = false;
        }
        else
        {
            pnl_utr_report.Visible = true;
        }
    }


    protected void txt_total_received_TextChanged(object sender, EventArgs e)
    {
        try
        {



            // d.con.Open();

            GridViewRow row = (GridViewRow)(((TextBox)sender)).NamingContainer;




            string txt_received = (row.FindControl("txt_received_amt1") as System.Web.UI.WebControls.TextBox).Text;
            Double txt_received1 = Convert.ToDouble(txt_received);

            string txt_received_am2 = (row.FindControl("txt_received_amt2") as System.Web.UI.WebControls.TextBox).Text;
            Double txt_received2 = Convert.ToDouble(txt_received_am2);




            Double Total = ((txt_received1) + (txt_received2));


            string txt_received_amt1 = (row.FindControl("txt_received_amt1") as System.Web.UI.WebControls.TextBox).Text;
            txt_received_amt1 = txt_received_amt1.ToString();

            string txt_received_amt2 = (row.FindControl("txt_received_amt2") as System.Web.UI.WebControls.TextBox).Text;
            txt_received_amt2 = txt_received_amt2.ToString();

            //if (txt_received1 > 0 && txt_received2 == 0)
            //{

            //    Total = txt_received1 - Total;
            //}

            //else
            TextBox txt_total_received = (TextBox)row.FindControl("txt_total_received");
            txt_total_received.Text = Total.ToString();

            //}

            //TextBox txt_rec1 = (TextBox)row.FindControl("txt_received_amt1");
            //TextBox txt_rec2 = (TextBox)row.FindControl("txt_received_amt2");

            //if (txt_received1 > 0 && txt_received1 == 0)
            //{
            //    txt_rec1.Enabled = true;
            //    txt_rec2.Enabled = false;
            //}
            //else
            //{
            //    txt_rec1.Enabled = false;
            //    txt_rec2.Enabled = true;
            //}




        }






        catch (Exception ex) { throw ex; }
        finally
        {
            gv_invoice_pmt.Visible = true;
            //button.Visible = true;
            d.con.Close();
        }
    }



    protected void txt_total_received_TextChanged1(object sender, EventArgs e)
    {

        try
        {
            //for showing sum of cell in textbox
            double total = 0;
            double tds_amt = 0, billing_amt = 0, receivable = 0, received_amt1 = 0, received_amt2 = 0, total_received = 0, deduction_amt = 0; ;

            GridViewRow row = (GridViewRow)(((TextBox)sender)).NamingContainer;

            try
            {
                string cell_value_t = row.Cells[2].Text;
                billing_amt = Convert.ToDouble(row.Cells[2].Text.ToString());
            }
            catch { }
            try
            {
                TextBox txt_tds_amt_1 = (TextBox)row.FindControl("txt_tds_amt");
                if (txt_tds_amt_1.Text == "")
                {
                    txt_tds_amt_1.Text = "0";
                }

                    string txt_tds_amt = txt_tds_amt_1.Text;//(row.FindControl("txt_tds_amt") as System.Web.UI.WebControls.TextBox).Text;
                //if (txt_tds_amt=="")
                //{
                //    txt_tds_amt = "0";
                //}
                    tds_amt = Convert.ToDouble(txt_tds_amt);
            }
            catch { }

            TextBox txt_receivable_amt = (TextBox)row.FindControl("txt_recive_amt");
            receivable = billing_amt - tds_amt;
            txt_receivable_amt.Text = receivable.ToString("0.00");

            TextBox txt_received_amt1 = (TextBox)row.FindControl("txt_received_amt1");
            TextBox txt_received_amt2 = (TextBox)row.FindControl("txt_received_amt2");
            TextBox txt_received_amt3 = (TextBox)row.FindControl("txt_received_amt3");
            TextBox txt_deduct_amt = (TextBox)row.FindControl("txt_deduct_amt");
            DropDownList ddl_remark_head = (DropDownList)row.FindControl("ddl_remark_head");
            if (txt_received_amt1.Text=="")
            { txt_received_amt1.Text = "0";  }
            if (txt_received_amt2.Text == "")
            { txt_received_amt2.Text = "0"; }
            if (txt_received_amt3.Text == "")
            { txt_received_amt3.Text = "0"; }
           
            if (txt_deduct_amt.Text == "")
            { txt_deduct_amt.Text = "0"; }

            total_received = (Convert.ToDouble(txt_received_amt1.Text) + Convert.ToDouble(txt_received_amt2.Text) + Convert.ToDouble(txt_received_amt3.Text));
            TextBox txt_total_received1 = (TextBox)row.FindControl("txt_total_received");
            txt_total_received1.Text = total_received.ToString("0.00");
            Double BALANCE_RS = Math.Round((Convert.ToDouble(txt_receivable_amt.Text)) - (Convert.ToDouble(txt_total_received1.Text)), 2);
            TextBox txt_balance = (TextBox)row.FindControl("txt_balance");
            txt_balance.Text = BALANCE_RS.ToString();

            if (ddl_remark_head.SelectedValue=="3")// Credit note --selected value
            {
                double bal_amt1=BALANCE_RS-Math.Round((Convert.ToDouble(txt_deduct_amt.Text)),2);
                txt_balance.Text = bal_amt1.ToString();
            }


            foreach (GridViewRow gvr in gv_invoice_pmt.Rows)
            {
                TextBox tb_rec1 = (TextBox)gvr.Cells[9].FindControl("txt_received_amt1");
                TextBox tb_rec2 = (TextBox)gvr.Cells[10].FindControl("txt_received_amt2");
                TextBox tb_rec3 = (TextBox)gvr.Cells[11].FindControl("txt_received_amt3");
                TextBox tb_tot_rec = (TextBox)gvr.Cells[12].FindControl("txt_total_received");
                string rec1 = tb_rec1.Text;
                string rec2 = tb_rec2.Text;
                string rec3 = tb_rec3.Text;
                tb_tot_rec.Text = (Convert.ToDouble(tb_rec1.Text) + Convert.ToDouble(tb_rec2.Text) + Convert.ToDouble(tb_rec3.Text)).ToString("0.00");


                //TextBox tb = (TextBox)gvr.Cells[8].FindControl("txt_received_amt1");
                TextBox tb = (TextBox)gvr.Cells[12].FindControl("txt_total_received");
                txt_recived_am.Text = "0";
                //Add column name insted of Column1 and Enter column cell index correctly.
                double sum;
                if (double.TryParse(tb.Text, out sum))
                {
                    total = total + sum;
                }


            }

            string tot_rece_amt = (row.FindControl("txt_total_received") as System.Web.UI.WebControls.TextBox).Text;
            //  float tot_rece_amt = Convert.ToSingle("tot_rece_amt").ToString();
            double pre_tot_rec = Convert.ToDouble(ViewState["tot_rece_amt"].ToString());
            double act_rece = Convert.ToDouble(total) - pre_tot_rec;
            txt_recived_am.Text = act_rece.ToString("0.00");





        }
        catch (Exception ex) { throw ex; }
        finally
        {
           // gv_invoice_pmt.Visible = true;
            //button.Visible = true;
            d.con.Close();
        }
    }
    protected void ddl_remark_head_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow row = (GridViewRow)(((DropDownList)sender)).NamingContainer;
            //GridViewRow row = (GridViewRow)(((TextBox)sender)).NamingContainer;
            TextBox txt_deduct_amt = (TextBox)row.FindControl("txt_deduct_amt");
            DropDownList ddl_remark_head = (DropDownList)row.FindControl("ddl_remark_head");
            if (ddl_remark_head.SelectedValue == "3")// Credit note --selected value
            {
                txt_deduct_amt.Enabled = true;
            }
            else
            {
                txt_deduct_amt.Enabled = false;
               // txt_deduct_amt.Text = "0";
              //  txt_total_received_TextChanged1(sender,  e);
            }


        }
        catch (Exception)
        {
            
            throw;
        }
    }

    protected void gv_invoice_pmt_SelectedIndexChanged(object sender, EventArgs e)
    {

        int sum = 0;

        for (int i = 0; i < gv_invoice_pmt.Rows.Count; i++)
        {
            if (gv_invoice_pmt != null)
            {
                //if (int.TryParse(gv_invoice_pmt.Rows[i].Cells[8].Text.ToString(), out sum))
                //{
                //   // int total = sum + Convert.ToInt32(gv_invoice_pmt.Rows[i].Cells[8].Text);



                //    // sum += *Convert.ToInt32(saleDataGridView.Rows[i].Cells[6].Value);



                //   // txt_recived_am.Text = total.ToString();
                //}


            }

        }
    }
    protected void ddl_client_gv_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        if (ddl_client_gv.SelectedValue=="ALL" && ddl_type.SelectedValue=="ALL")
        {
            btn_view.Visible = false;
        }
        else
        {
            btn_view.Visible = true;  
        }
    }

    protected void btn_view_outstanding_Click(object sender, EventArgs e)
    {
        hidtab.Value = "5";
        string query = "";
        //  string sql_where = "", where_status = "";
        try
        {
            if (ddl_client_outstanding.SelectedValue != "Select")
            {

                string where_client = "";

                if (ddl_client_outstanding.SelectedValue == "TATA STEEL LTD" || ddl_client_outstanding.SelectedValue == "TATA STEELS PVT LTD")
                {
                    where_client = " and g.client_code = '7'  ";
                }
                else if (ddl_client_outstanding.SelectedValue == "EQUITAS SMALL FINANCE BANK" || ddl_client_outstanding.SelectedValue == "Equitas Small Finance Bank Limited")
                {
                    where_client = " and g.client_code  IN ('ESFB','EquitasRes' ) ";
                }
                else if (ddl_client_outstanding.SelectedValue == "TATA AIA Life Insurance Company Limited" || ddl_client_outstanding.SelectedValue == "TATA AIA Life Insurance Company Limited Tem")
                {
                    where_client = " and g.client_code  IN ('TAIL','TAILTEMP' ) ";
                }
                else if (ddl_client_outstanding.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD." || ddl_client_outstanding.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD. RM")
                {
                    where_client = " and g.client_code  IN ('RLIC HK','RNLIC RM' ) ";
                }
                else 
                {
                    where_client = " and g.client_name = '" + ddl_client_outstanding.SelectedValue + "'  ";
                }

               // query = "select invoice_date,invoice_no,CONCAT(LEFT(MONTHNAME(STR_TO_DATE(month,'%m')),3),'-',year) as Month,client_name,state_name as state,  ucase(type) as billtype,amount, gst, billing_amt,tds_amount,(billing_amt-tds_amount) as receivable_amt ,received_amt1,received_amt2,received_amt3, total_received,deduction_amt, Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Less Received','Not Received') as status from  ( select client_name,state_name,month,year,invoice_no,Date_format(invoice_date,'%d/%m/%Y') as invoice_date,type,Round(amount,2) as amount,ROUND((cgst+sgst+igst),2) as gst, Round((amount+cgst+sgst+igst),2) as billing_amt,Round(tds_amount,2) as tds_amount, Round(received_amt,2) as received_amt1,Round(received_amt2,2) as received_amt2 ,Round(received_amt3,2) as received_amt3,Round((received_amt+received_amt2+received_amt3),2) as total_received,Round(IFNULL(deduction_amt,0),2) as deduction_amt from pay_report_gst where flag_invoice=2 " + where_client + " order by year,month asc ) as t1 where  ((( billing_amt-total_received-tds_amount-deduction_amt)>100 && total_received>0)  || (total_received=0)) and year>2019 AND type !='credit'";
              //  query = "select invoice_date,invoice_no, month,client_name,state_name as state,  ucase(type) as billtype,amount, gst, billing_amt,tds_amount,(billing_amt-tds_amount) as receivable_amt ,received_amt1,received_amt2,received_amt3, total_received,deduction_amt, Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Less Received','Not Received') as status from  ( select g.client_name,  If((i.region!='Select' && i.region!='ALL'  && i.region!=''),concat(g.state_name,' (',i.region,')'),g.state_name) as state_name ,DATE_FORMAT(g.invoice_date,'%b-%Y') as month,g.year,g.invoice_no,Date_format(g.invoice_date,'%d/%m/%Y') as invoice_date,g.type,Round(g.amount,2) as amount,ROUND((g.cgst+g.sgst+g.igst),2) as gst, Round((g.amount+g.cgst+g.sgst+g.igst),2) as billing_amt,Round(g.tds_amount,2) as tds_amount, Round(g.received_amt,2) as received_amt1,Round(g.received_amt2,2) as received_amt2 ,Round(g.received_amt3,2) as received_amt3,Round((g.received_amt+g.received_amt2+g.received_amt3),2) as total_received,Round(IFNULL(g.deduction_amt,0),2) as deduction_amt from  pay_report_gst g  left join pay_billing_invoice_history i on g.comp_code=i.comp_code and g.client_code=i.client_code and g.invoice_no=i.invoice_no where g.flag_invoice=2 and " + where_client + " order by g.year,g.month asc ) as t1 where  ((( billing_amt-total_received-tds_amount-deduction_amt)>100 && total_received>0)  || (total_received=0)) and year>2019 AND type !='credit'";

                query = "select invoice_date,invoice_no, month,client_name,state_name as state,  ucase(type) as billtype,amount, gst, billing_amt,tds_amount,(billing_amt-tds_amount) as receivable_amt ,received_amt1,received_amt2,received_amt3, total_received,deduction_amt, Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Less Received','Not Received') as status from  ( select g.client_name,  If((i.region!='Select' && i.region!='ALL'  && i.region!=''),concat(g.state_name,' (',i.region,')'),g.state_name) as state_name ,DATE_FORMAT(g.invoice_date,'%b-%Y') as month,g.year,g.invoice_no,Date_format(g.invoice_date,'%d/%m/%Y') as invoice_date,g.type,Round(g.amount,2) as amount,ROUND((g.cgst+g.sgst+g.igst),2) as gst, Round((g.amount+g.cgst+g.sgst+g.igst),2) as billing_amt,Round(g.tds_amount,2) as tds_amount, Round(g.received_amt,2) as received_amt1,Round(g.received_amt2,2) as received_amt2 ,Round(g.received_amt3,2) as received_amt3,Round((g.received_amt+g.received_amt2+g.received_amt3),2) as total_received,Round(IFNULL(g.deduction_amt,0),2) as deduction_amt from  pay_report_gst g  left join pay_billing_invoice_history i on g.comp_code=i.comp_code and g.client_code=i.client_code and g.invoice_no=i.invoice_no where g.flag_invoice=2 and   (g.invoice_no is not null && g.invoice_no!='' ) and  (g.invoice_date is not null && g.invoice_date!='') " + where_client + " group by g.invoice_no order by g.year,g.month asc ) as t1 where  ((( billing_amt-total_received-tds_amount-deduction_amt)>100 && total_received>=0)  || (total_received=0 && deduction_amt=0)) and year>2019 AND type !='credit'";
                    
                MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
                DataSet ds = new DataSet();
                dscmd.SelectCommand.CommandTimeout = 400;
                dscmd.Fill(ds);
                //gv_outstanding.DataSource = ds.Tables[0];
                //gv_outstanding.DataBind();
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Outstanding_" + ddl_client_outstanding.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    Repeater Repeater1 = new Repeater();
                    Repeater1.DataSource = ds;
                    Repeater1.HeaderTemplate = new MyTemplate_outstanding(ListItemType.Header, ds);
                    Repeater1.ItemTemplate = new MyTemplate_outstanding(ListItemType.Item, ds);
                    Repeater1.FooterTemplate = new MyTemplate_outstanding(ListItemType.Footer, ds);
                    Repeater1.DataBind();
                    System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                    System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                    Repeater1.RenderControl(htmlWrite);
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
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select client name.');", true);
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }

    protected void gv_outstanding_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_outstanding.UseAccessibleHeader = false;
            gv_outstanding.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
    protected void btn_send_mail_Click(object sender, EventArgs e)
    {
        try
        {
            hidtab.Value = "5";
            if (ddl_client_outstanding.SelectedValue != "Select")
            {

                //string where_client = "";
                //if (ddl_client_outstanding.SelectedValue == "TATA STEEL LTD" || ddl_client_outstanding.SelectedValue == "TATA STEELS PVT LTD")
                //{
                //    where_client = " and pay_report_gst.client_code = '7'  ";
                //}
                //else 
                //{
                //    where_client = " and pay_report_gst.client_name = '" + ddl_client_outstanding.SelectedValue + "'  ";
                //}
                //query = "select invoice_date,invoice_no,CONCAT(month,'-',year) as Month,client_name,state_name as state,  ucase(type) as billtype,amount, gst, billing_amt,tds_amount,(billing_amt-tds_amount) as receivable_amt ,received_amt1,received_amt2,received_amt3, total_received, Round(( billing_amt-total_received-tds_amount),2) as balance_amt,IF(total_received>0,'Less Received','Not Received') as status from  ( select client_name,state_name,month,year,invoice_no,Date_format(invoice_date,'%d/%m/%Y') as invoice_date,type,Round(amount,2) as amount,ROUND((cgst+sgst+igst),2) as gst, Round((amount+cgst+sgst+igst),2) as billing_amt,Round(tds_amount,2) as tds_amount, Round(received_amt,2) as received_amt1,Round(received_amt2,2) as received_amt2 ,Round(received_amt3,2) as received_amt3,Round((received_amt+received_amt2+received_amt3),2) as total_received from pay_report_gst where flag_invoice=2  " + where_client + " order by year,month asc ) as t1 where  ((( billing_amt-total_received-tds_amount)>100 && total_received>0)  || (total_received=0)) and year>2019 AND type !='credit'";
                //MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
                //DataSet ds = new DataSet();
                //dscmd.SelectCommand.CommandTimeout = 400;
                //dscmd.Fill(ds);

                //if (ds.Tables[0].Rows.Count > 0)
                //{
                   

                  //  string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                    string strPath = Server.MapPath("~/final_invoice\\");
                    string fileName = "Outstanding_" + ddl_client_outstanding.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls";
                  //  File.WriteAllText(strPath + fileName, stringWrite.ToString());
                    multisheet_outstanding(strPath, fileName);

                   send_zip_mail(strPath + fileName, strPath);


               // }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select client name.');", true);
            }

        }
        catch { }
    }

    private void multisheet_outstanding(string folder_path, string fileName)
    {

        string body = string.Empty;
        using (StreamReader reader = new StreamReader(System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Multisheet_outstanding.mht")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{Not_Received}", generate_report_softcopy(1, "NotReceived").ToString());
        body = body.Replace("{Lesss_received}", generate_report_softcopy(2, "LessReceived").ToString());
            body = body.Replace("colspan=", "class=3Dxl65 colspan=3D");
            body = body.Replace("=ROUND", "=3DROUND");
            body = body.Replace("=SUM", "=3DSUM");
            body = body.Replace("border =", "border=3D");
            body = body.Replace("border=", "border=3D");
            body = body.Replace("<th>", "<th class=3Dxl66>");

            System.IO.File.WriteAllText(folder_path + "\\" + fileName , body);


        
    }


    private StringWriter generate_report_softcopy(int i,string bal_status)
    {

        #region finance Copy
        string sql = ""; string where_client = ""; 
        System.IO.StringWriter stringWrite = new System.IO.StringWriter();
        if (ddl_client_outstanding.SelectedValue != "Select")
        {           
            //if (ddl_client_outstanding.SelectedValue == "TATA STEEL LTD" || ddl_client_outstanding.SelectedValue == "TATA STEELS PVT LTD")
            //{
            //    where_client = " and pay_report_gst.client_code = '7'  ";
            //}
            //else
            //{
            //    where_client = " and pay_report_gst.client_name = '" + ddl_client_outstanding.SelectedValue + "'  ";
            //}
            if (ddl_client_outstanding.SelectedValue == "TATA STEEL LTD" || ddl_client_outstanding.SelectedValue == "TATA STEELS PVT LTD")
            {
                where_client = " and g.client_code = '7'  ";
            }
            else if (ddl_client_outstanding.SelectedValue == "EQUITAS SMALL FINANCE BANK" || ddl_client_outstanding.SelectedValue == "Equitas Small Finance Bank Limited")
            {
                where_client = " and g.client_code  IN ('ESFB','EquitasRes' ) ";
            }
            else if (ddl_client_outstanding.SelectedValue == "TATA AIA Life Insurance Company Limited" || ddl_client_outstanding.SelectedValue == "TATA AIA Life Insurance Company Limited Tem")
            {
                where_client = " and g.client_code  IN ('TAIL','TAILTEMP' ) ";
            }
            else if (ddl_client_outstanding.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD." || ddl_client_outstanding.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD. RM")
            {
                where_client = " and g.client_code  IN ('RLIC HK','RNLIC RM' ) ";
            }
            else //if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
            {
                where_client = " and g.client_name = '" + ddl_client_outstanding.SelectedValue + "'  ";
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select client name.');", true);
        }

        d.con.Open();
        try
        {
            if (i == 1)//Not Received
            {              
               // sql = "select invoice_date,invoice_no,Month,client_name,state_name as state,  ucase(type) as billtype,amount, gst, billing_amt,tds_amount,(billing_amt-tds_amount) as receivable_amt ,received_amt1,received_amt2,received_amt3, total_received,deduction_amt, Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Less Received','Not Received') as status from  ( select client_name,state_name,DATE_FORMAT(invoice_date,'%b-%Y') as Month,year,invoice_no,Date_format(invoice_date,'%d-%m-%Y') as invoice_date,type,Round(amount,2) as amount,ROUND((cgst+sgst+igst),2) as gst, Round((amount+cgst+sgst+igst),2) as billing_amt,Round(tds_amount,2) as tds_amount, Round(received_amt,2) as received_amt1,Round(received_amt2,2) as received_amt2 ,Round(received_amt3,2) as received_amt3,Round((received_amt+received_amt2+received_amt3),2) as total_received, Round(IFNULL(deduction_amt,0),2) as deduction_amt from pay_report_gst where flag_invoice=2  " + where_client + " order by year,month asc ) as t1 where  total_received=0 and year>2019 AND type !='credit'";                              


                sql = "select invoice_date,invoice_no, month,client_name,state_name as state,  ucase(type) as billtype,amount, gst, billing_amt,tds_amount,(billing_amt-tds_amount) as receivable_amt ,received_amt1,received_amt2,received_amt3, total_received,deduction_amt, Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,CASE WHEN (total_received > 0 AND (payment_status = 'Short Amount Received' OR payment_status IS NULL)) THEN 'Less Received ' WHEN (total_received = 0 AND chk_invoice_date < CURDATE() - INTERVAL 30 DAY) THEN 'Not Received (Old)' WHEN (total_received = 0 AND chk_invoice_date >= CURDATE() - INTERVAL 30 DAY) THEN ' Not Received (Current)' END AS status   from  ( select g.client_name,  If((i.region!='Select' && i.region!='ALL'  && i.region!=''),concat(g.state_name,' (',i.region,')'),g.state_name) as state_name ,DATE_FORMAT(g.invoice_date,'%b-%Y') as month,g.year,g.invoice_no,g.invoice_date AS chk_invoice_date,Date_format(g.invoice_date,'%d/%m/%Y') as invoice_date,g.type,Round(g.amount,2) as amount,ROUND((g.cgst+g.sgst+g.igst),2) as gst, Round((g.amount+g.cgst+g.sgst+g.igst),2) as billing_amt,Round(g.tds_amount,2) as tds_amount, Round(g.received_amt,2) as received_amt1,Round(g.received_amt2,2) as received_amt2 ,Round(g.received_amt3,2) as received_amt3,Round((g.received_amt+g.received_amt2+g.received_amt3),2) as total_received,Round(IFNULL(g.deduction_amt,0),2) as deduction_amt,g.payment_status from  pay_report_gst g  left join pay_billing_invoice_history i on g.comp_code=i.comp_code and g.client_code=i.client_code and g.invoice_no=i.invoice_no where g.flag_invoice=2 and   (g.invoice_no is not null && g.invoice_no!='' ) and  (g.invoice_date is not null && g.invoice_date!='') " + where_client + " group by g.invoice_no order by g.year,g.month asc ) as t1 where  (total_received=0 && deduction_amt=0) and year>2019 AND type !='credit'";
                 
            
            }
            else if (i == 2)//Less Received
            {
             //   sql = "select invoice_date,invoice_no,Month,client_name,state_name as state,  ucase(type) as billtype,amount, gst, billing_amt,tds_amount,(billing_amt-tds_amount) as receivable_amt ,received_amt1,received_amt2,received_amt3, total_received,deduction_amt, Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Less Received','Not Received') as status from  ( select client_name,state_name,DATE_FORMAT(invoice_date,'%b-%Y') as Month,year,invoice_no,Date_format(invoice_date,'%d-%m-%Y') as invoice_date,type,Round(amount,2) as amount,ROUND((cgst+sgst+igst),2) as gst, Round((amount+cgst+sgst+igst),2) as billing_amt,Round(tds_amount,2) as tds_amount, Round(received_amt,2) as received_amt1,Round(received_amt2,2) as received_amt2 ,Round(received_amt3,2) as received_amt3,Round((received_amt+received_amt2+received_amt3),2) as total_received, Round(IFNULL(deduction_amt,0),2) as deduction_amt from pay_report_gst where flag_invoice=2  " + where_client + " order by year,month asc ) as t1 where  (( billing_amt-total_received-tds_amount-deduction_amt)>100 && total_received>0) and year>2019 AND type !='credit'";
                sql = "select invoice_date,invoice_no, month,client_name,state_name as state,  ucase(type) as billtype,amount, gst, billing_amt,tds_amount,(billing_amt-tds_amount) as receivable_amt ,received_amt1,received_amt2,received_amt3, total_received,deduction_amt, Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Less Received','Not Received') as status from  ( select g.client_name,  If((i.region!='Select' && i.region!='ALL'  && i.region!=''),concat(g.state_name,' (',i.region,')'),g.state_name) as state_name ,DATE_FORMAT(g.invoice_date,'%b-%Y') as month,g.year,g.invoice_no,Date_format(g.invoice_date,'%d/%m/%Y') as invoice_date,g.type,Round(g.amount,2) as amount,ROUND((g.cgst+g.sgst+g.igst),2) as gst, Round((g.amount+g.cgst+g.sgst+g.igst),2) as billing_amt,Round(g.tds_amount,2) as tds_amount, Round(g.received_amt,2) as received_amt1,Round(g.received_amt2,2) as received_amt2 ,Round(g.received_amt3,2) as received_amt3,Round((g.received_amt+g.received_amt2+g.received_amt3),2) as total_received,Round(IFNULL(g.deduction_amt,0),2) as deduction_amt from  pay_report_gst g  left join pay_billing_invoice_history i on g.comp_code=i.comp_code and g.client_code=i.client_code and g.invoice_no=i.invoice_no where g.flag_invoice=2 and   (g.invoice_no is not null && g.invoice_no!='' ) and  (g.invoice_date is not null && g.invoice_date!='') " + where_client + " group by g.invoice_no order by g.year,g.month asc ) as t1 where  ((( billing_amt-total_received-tds_amount-deduction_amt)>100 && total_received>0)) and year>2019 AND type !='credit'"; 
            }
           
            DataSet ds = new DataSet();
            MySqlDataAdapter dscmd = new MySqlDataAdapter(sql, d.con);
            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                    //Response.Clear();
                    //Response.Buffer = true;
                    //if (i == 1)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=NotReceived_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}
                    //else if (i == 2)
                    //{
                    //    Response.AddHeader("content-disposition", "attachment;filename=LessReceived_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    //}                  
                
        #endregion
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate_outstanding(ListItemType.Header, ds);
                Repeater1.ItemTemplate = new MyTemplate_outstanding(ListItemType.Item, ds);
                Repeater1.FooterTemplate = new MyTemplate_outstanding(ListItemType.Footer, ds);             
                
                Repeater1.DataBind();              
                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                Repeater1.RenderControl(htmlWrite);

                return stringWrite;




              
                string style = @"<style> .textmode { mso-number-format:\@; } </style>";
               
                string strPath = Server.MapPath("~/final_invoice\\");
                string fileName = "Outstanding_" + ddl_client_outstanding.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls";
                string fileName1 = "new_" + fileName ;
                File.WriteAllText(strPath + fileName, stringWrite.ToString());
                //Response.Clear();
                //  Response.Write("{\"success\":true,\"isSuccess\":true,\"fileName\":\"" + fileName + "\"}");

                //// Convert Excel to PDF in memory  
                //SautinSoft.ExcelToPdf x = new SautinSoft.ExcelToPdf();

                //// Set PDF as output format.  
                //x.OutputFormat = SautinSoft.ExcelToPdf.eOutputFormat.Pdf;

                //string excelFile = strPath + fileName;
                //string newExcelPath = strPath + fileName1;
                //string pdfFile = Path.ChangeExtension(newExcelPath, ".pdf");
                //byte[] pdfBytes = null;

                //try
                //{
                //    Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                //    Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Open(excelFile);
                //    app.DisplayAlerts = false;
                //    workbook.CheckCompatibility = false;
                //    workbook.DoNotPromptForConvert = true;
                //    workbook.SaveAs(newExcelPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8,
                //    Type.Missing, Type.Missing, false, false,
                //    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing,
                //    Type.Missing, Type.Missing, Type.Missing);
                //    workbook.Close();
                //    app.Quit();
                //    app = null;

                //    //DataTable dt = ConvertExcelToDataTable(newExcelPath);
                //    //GeneratePDF(dt, pdfFile, page_name);

                //    if (File.Exists(excelFile))
                //    {
                //        File.Delete(excelFile);
                //    }
                //    if (File.Exists(newExcelPath))
                //    {
                //        File.Delete(newExcelPath);
                //    }
                //    //string stamp_filename = page_name + ddl_client.SelectedValue.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + "_" + ddl_billing_state.SelectedValue + "_" + invoice + ".pdf";
                //    //add_stamp_on_pdf(pdfFile, strPath, stamp_filename, billing_flag);

                //    if (File.Exists(pdfFile))
                //    {
                //        File.Delete(pdfFile);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //    Console.ReadLine();
                //}
            }
            

            d.con.Close();


            System.Text.StringBuilder sb = new System.Text.StringBuilder("", 1);
            return new System.IO.StringWriter(sb);
            
            
            //if (i != 0)
            //{
                //System.Text.StringBuilder sb = new System.Text.StringBuilder("", 1);
                //return new System.IO.StringWriter(sb);
              //  return stringWrite;
            //}
            //else
            //{
            //    return null;
            //}
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }


    }
   
    
     protected void ddl_client_outstanding_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            hidtab.Value = "5";

            string to_mailid = "", cc_mailid = "", bcc_mailid = "";

            if (ddl_client_outstanding.SelectedValue!="Select")
            {
                show_outstanding();

                string client_code_gst = d.getsinglestring("select distinct client_code from pay_report_gst where comp_code='" + Session["comp_code"].ToString() + "' and client_name ='" + ddl_client_outstanding.SelectedValue + "' ");

        DataSet ds = new DataSet();
        ds = d.select_data("select comp_code, client_code, client_name, from_mail, from_mail_pw, to_mail, cc_mail, bcc_mail, thanks_n_reguards, contact, designation, comp_name, billing_cycle, billing_tag_line, attendance_tag_line, active from  pay_outstanding_email_details where comp_code='" + Session["comp_code"].ToString() + "' and client_code='"+client_code_gst+"' and active=1");

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            //from_mail = dr["from_mail"].ToString();
            //from_mail_password = dr["from_mail_pw"].ToString();
            to_mailid = dr["to_mail"].ToString();
            cc_mailid = dr["cc_mail"].ToString();
            bcc_mailid = dr["bcc_mail"].ToString();
            //thanks_regards = dr["thanks_n_reguards"].ToString();
            //contact = dr["contact"].ToString();
            //designation = dr["designation"].ToString();
            //company = dr["comp_name"].ToString();
        }
            }
            else
            {
                to_mailid = "";
                cc_mailid = "";
                bcc_mailid = "";
                lit_outstanding_amt.Text = "0";
                if (lit_outstanding_amt.Text == "0")
                {
                    btn_send_mail.Visible = false;
                }
                else
                {
                    btn_send_mail.Visible = true;
                }

            }

            txt_to_mail.Text = to_mailid.ToString();
            txt_cc_mail.Text = cc_mailid.ToString();
            if (to_mailid=="")
            {
                btn_send_mail.Visible = false;
            } else
                {
                    btn_send_mail.Visible = true;
                }


        }
        catch  { }
    
    }

     private void show_outstanding()
     {
         string where_client = "";

         if (ddl_client_outstanding.SelectedValue == "TATA STEEL LTD" || ddl_client_outstanding.SelectedValue == "TATA STEELS PVT LTD")
         {
             where_client = " and pay_report_gst.client_code = '7'  ";
         }
         else if (ddl_client_outstanding.SelectedValue == "EQUITAS SMALL FINANCE BANK" || ddl_client_outstanding.SelectedValue == "Equitas Small Finance Bank Limited")
	     {
             where_client = " and pay_report_gst.client_code  IN ('ESFB','EquitasRes' ) ";
	     }
         else if (ddl_client_outstanding.SelectedValue == "TATA AIA Life Insurance Company Limited" || ddl_client_outstanding.SelectedValue == "TATA AIA Life Insurance Company Limited Tem")
         {
             where_client = " and pay_report_gst.client_code  IN ('TAIL','TAILTEMP' ) ";
         }
         else if (ddl_client_outstanding.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD." || ddl_client_outstanding.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD. RM")
         {
             where_client = " and pay_report_gst.client_code  IN ('RLIC HK','RNLIC RM' ) ";
         }
         else //if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
         {
             where_client = " and pay_report_gst.client_name = '" + ddl_client_outstanding.SelectedValue + "'  ";
         }
         lit_outstanding_amt.Text = d.getsinglestring(" select IFNUll(SUM(balance_amt),0) as balance_amt from (select Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Short Amount Received Against Invoices','Amount Not Received Against Invoices') as status from  ( select client_name,state_name,month,year,invoice_no,Date_format(invoice_date,'%d-%m-%Y') as invoice_date,type,Round(amount,2) as amount,ROUND((cgst+sgst+igst),2) as gst, Round((amount+cgst+sgst+igst),2) as billing_amt,Round(tds_amount,2) as tds_amount, Round(received_amt,2) as received_amt1,Round(received_amt2,2) as received_amt2 ,Round(received_amt3,2) as received_amt3,Round((received_amt+received_amt2+received_amt3),2) as total_received, round(IFNULL(deduction_amt,0),2) as deduction_amt from pay_report_gst where flag_invoice=2  " + where_client + " order by year,month asc ) as t1 where  year>2019 AND type !='credit' and  ((( billing_amt-total_received-tds_amount-deduction_amt)>100 && total_received>0)  || (total_received=0))) as t1");
                                                   
         if ( lit_outstanding_amt.Text=="0")
         {
             btn_send_mail.Visible = false;
         }
         else
         {
             btn_send_mail.Visible = true;
         }

     }

    protected void send_zip_mail(string zip_filename, string file_path)
    {
        string body = "";

        string from_mail = "", from_mail_password = "", to_mailid = "", cc_mailid = "", bcc_mailid = "", thanks_regards = "", contact = "", company = "", designation = "";

        string client_code_gst = d.getsinglestring("select distinct client_code from pay_report_gst where comp_code='" + Session["comp_code"].ToString() + "' and client_name ='" + ddl_client_outstanding.SelectedValue + "' ");

        DataSet ds = new DataSet();
        ds = d.select_data("select comp_code, client_code, client_name, from_mail, from_mail_pw, to_mail, cc_mail, bcc_mail, thanks_n_reguards, contact, designation, comp_name, billing_cycle, billing_tag_line, attendance_tag_line, active from  pay_outstanding_email_details where comp_code='" + Session["comp_code"].ToString() + "' and client_code='"+client_code_gst+"' and active=1");

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            from_mail = dr["from_mail"].ToString();
            from_mail_password = dr["from_mail_pw"].ToString();
            to_mailid = dr["to_mail"].ToString();
            cc_mailid = dr["cc_mail"].ToString();
            bcc_mailid = dr["bcc_mail"].ToString();
            thanks_regards = dr["thanks_n_reguards"].ToString();
            contact = dr["contact"].ToString();
            designation = dr["designation"].ToString();
            company = dr["comp_name"].ToString();
        }





        string from_emailid = from_mail;
        string password = from_mail_password;
        string current_date = DateTime.Now.ToString("dd/MMM/yyyy");
        string month_year = DateTime.Now.ToString("MMMM-yyyy");
        string to_emailid = to_mailid.Trim();
        string cc_emailid = cc_mailid.Trim();


        string where_client = "";

        if (ddl_client_outstanding.SelectedValue == "TATA STEEL LTD" || ddl_client_outstanding.SelectedValue == "TATA STEELS PVT LTD")
        {
            where_client = " and g.client_code = '7'  ";
        }
        else if (ddl_client_outstanding.SelectedValue == "EQUITAS SMALL FINANCE BANK" || ddl_client_outstanding.SelectedValue == "Equitas Small Finance Bank Limited")
        {
            where_client = " and g.client_code  IN ('ESFB','EquitasRes' ) ";
        }
        else if (ddl_client_outstanding.SelectedValue == "TATA AIA Life Insurance Company Limited" || ddl_client_outstanding.SelectedValue == "TATA AIA Life Insurance Company Limited Tem")
        {
            where_client = " and g.client_code  IN ('TAIL','TAILTEMP' ) ";
        }
        else if (ddl_client_outstanding.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD." || ddl_client_outstanding.SelectedValue == "RELIANCE NIPPON LIFE INSURANCE CO. LTD. RM")
        {
            where_client = " and g.client_code  IN ('RLIC HK','RNLIC RM' ) ";
        }
        else //if (ddl_client_gv.SelectedValue == "TATA STEEL LTD" || ddl_client_gv.SelectedValue == "TATA STEELS PVT LTD")
        {
            where_client = " and g.client_name = '" + ddl_client_outstanding.SelectedValue + "'  ";
        }


        string last_invoice_date = d.getsinglestring("SELECT DATE_FORMAT(  MAx(g.invoice_date), '%d-%m-%Y') AS invoice_date   FROM   pay_report_gst g  WHERE   g.flag_invoice = 2  " + where_client + "");

        string mail_body = "We request you to please check & clear the outstanding at the earliest for the smooth operational process. \n If you have any payment advice for the mentioned invoices, kindly share with us so that we will update the same in our records. If you have any query in the mentioned outstanding, please feel free to contact us. ";


        List<string> temp_list = new List<string>();
        try
        {
           
            body = "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title></title></head><body><P><tr><td><b>Dear Sir / Madam, </b></td></tr></P><P><tr><td><b>Greetings from IH&MS...!!!</b></td></tr> </P>   <tr><td><b>Please find the below outstanding details from date 01-01-2020 to till " + last_invoice_date + " Date.</b></td></tr>";//<tr><td><b>" + mail_body + "</b></td></tr>
            string[] main_body = mail_body.Split('.');
            foreach (string new_body in main_body)
            {
                temp_list.Add(new_body);
            }
            for (int i = 0; i < temp_list.Count - 1; i++)
            {
                body += "<tr><td>" + temp_list[i] + "." + "</td></tr>";
            }
           
            //Summary
            System.Data.DataTable dt = new System.Data.DataTable();
          //  string query = " select SUM(balance_amt) as balance_amt, status from (select Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Short Amount Received Against Invoices','Amount Not Received Against Invoices') as status from  ( select client_name,state_name,month,year,invoice_no,Date_format(invoice_date,'%d-%m-%Y') as invoice_date,type,Round(amount,2) as amount,ROUND((cgst+sgst+igst),2) as gst, Round((amount+cgst+sgst+igst),2) as billing_amt,Round(tds_amount,2) as tds_amount, Round(received_amt,2) as received_amt1,Round(received_amt2,2) as received_amt2 ,Round(received_amt3,2) as received_amt3,Round((received_amt+received_amt2+received_amt3),2) as total_received, Round(IFNULL(deduction_amt,0),2) as deduction_amt from pay_report_gst g where flag_invoice=2 " + where_client + " order by year,month asc ) as t1 where  year>2019 AND type !='credit' and  ((( billing_amt-total_received-tds_amount-deduction_amt)>100 && total_received>0)  || (total_received=0))) as t1 group by status order by status Asc";
            string query = " select SUM(balance_amt) as balance_amt, status from (select  balance_amt,total_received, payment_status, invoice_date, CASE  WHEN  (total_received > 0 AND (payment_status IS NULL OR payment_status != 'Payment Done'))  THEN  'Short Amount Received Against Invoices'  WHEN  (total_received = 0 AND invoice_date < CURDATE() - INTERVAL 30 DAY)  THEN  'Amount Not Received Against Invoices(Old)'  WHEN  (total_received = 0 AND invoice_date >= CURDATE() - INTERVAL 30 DAY)  THEN  'Amount Not Received Against Invoices(Current)' END AS status  from (SELECT  ROUND((billing_amt - total_received - tds_amount - deduction_amt), 2) AS balance_amt, total_received, payment_status, invoice_date FROM ( select client_name,state_name,month,year,invoice_no, payment_status, invoice_date, type,Round(amount,2) as amount,        ROUND((cgst+sgst+igst),2) as gst, Round((amount+cgst+sgst+igst),2) as billing_amt,Round(tds_amount,2) as tds_amount, Round(received_amt,2) as received_amt1,Round(received_amt2,2) as received_amt2 ,Round(received_amt3,2) as received_amt3,Round((received_amt+received_amt2+received_amt3),2) as total_received, Round(IFNULL(deduction_amt,0),2) as deduction_amt from pay_report_gst g where flag_invoice=2 " + where_client + " order by year,month asc ) as t1 where  year>2019 AND type !='credit' AND (((billing_amt - total_received - tds_amount - deduction_amt) > 100 && total_received >= 0) || (total_received = 0 && deduction_amt=0))) tt1) AS tt GROUP BY status";
          
            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            dscmd.SelectCommand.CommandTimeout = 400;
            dscmd.Fill(dt);

            //Amount Not Received Against Invoice(Old)
            string query2 = " SELECT invoice_date, invoice_no, Month, client_name, state_name AS state, UCASE(type) AS billtype, amount, gst, billing_amt, tds_amount, (billing_amt - tds_amount) AS receivable_amt, received_amt1, received_amt2, received_amt3, total_received, deduction_amt, ROUND((billing_amt - total_received - tds_amount - deduction_amt),2) AS balance_amt,IF(total_received > 0, 'Short Amount Received',  'Amount Not Received') AS status FROM (SELECT client_name,state_name,DATE_FORMAT(invoice_date, '%b-%Y') AS Month,year,invoice_no,DATE_FORMAT(invoice_date, '%d-%m-%Y') AS invoice_date,type,ROUND(amount, 2) AS amount,ROUND((cgst + sgst + igst), 2) AS gst,ROUND((amount + cgst + sgst + igst), 2) AS billing_amt,ROUND(tds_amount, 2) AS tds_amount,ROUND(received_amt, 2) AS received_amt1,ROUND(received_amt2, 2) AS received_amt2,ROUND(received_amt3, 2) AS received_amt3,ROUND((received_amt + received_amt2 + received_amt3), 2) AS total_received,ROUND(IFNULL(deduction_amt, 0), 2) AS deduction_amt  FROM  pay_report_gst g WHERE flag_invoice = 2 and total_received = 0 AND invoice_date < CURDATE() - INTERVAL 30 DAY  " + where_client + " ORDER BY year , month ASC) AS t1 WHERE   ((ROUND((billing_amt - total_received - tds_amount - deduction_amt),2) >100  && total_received >= 0)  || (total_received = 0 && deduction_amt=0)) and  total_received=0  AND invoice_date < CURDATE() - INTERVAL 30 DAY and  year > 2019  AND type != 'credit'";
            DataTable dt2 = new DataTable();
            MySqlDataAdapter dt2cmd = new MySqlDataAdapter(query2, d.con);
            dt2cmd.SelectCommand.CommandTimeout = 400;
            dt2cmd.Fill(dt2);

            //Amount Not Received Against Invoices(Current)
            string query3 = "SELECT  invoice_date, invoice_no, Month, client_name, state_name AS state, UCASE(type) AS billtype, amount, gst, billing_amt, tds_amount, (billing_amt - tds_amount) AS receivable_amt, received_amt1, received_amt2, received_amt3, total_received, deduction_amt, ROUND((billing_amt - total_received - tds_amount - deduction_amt), 2) AS balance_amt, IF(total_received > 0,'Short Amount Received','Amount Not Received') AS status FROM (SELECT client_name, state_name, DATE_FORMAT(invoice_date, '%b-%Y') AS Month, year, invoice_no, DATE_FORMAT(invoice_date, '%d-%m-%Y') AS invoice_date, type, ROUND(amount, 2) AS amount, ROUND((cgst + sgst + igst), 2) AS gst, ROUND((amount + cgst + sgst + igst), 2) AS billing_amt, ROUND(tds_amount, 2) AS tds_amount, ROUND(received_amt, 2) AS received_amt1, ROUND(received_amt2, 2) AS received_amt2, ROUND(received_amt3, 2) AS received_amt3, ROUND((received_amt + received_amt2 + received_amt3), 2) AS total_received, ROUND(IFNULL(deduction_amt, 0), 2) AS deduction_amt FROM pay_report_gst g WHERE flag_invoice = 2 AND total_received = 0 AND invoice_date >= CURDATE() - INTERVAL 30 DAY  " + where_client + "  ORDER BY year , month ASC) AS t1 WHERE  ((ROUND((billing_amt - total_received - tds_amount - deduction_amt),2) >100  && total_received >= 0)  || (total_received = 0 && deduction_amt=0))  and  year > 2019 AND type != 'credit'";
            DataTable dt3 = new DataTable();
            MySqlDataAdapter dt3cmd = new MySqlDataAdapter(query3, d.con);
            dt3cmd.SelectCommand.CommandTimeout = 400;
            dt3cmd.Fill(dt3);

            //Short Amount Received Against Invoices
            string query4 = " SELECT  invoice_date, invoice_no, Month, client_name, state_name AS state, UCASE(type) AS billtype, amount, gst, billing_amt, tds_amount, (billing_amt - tds_amount) AS receivable_amt, received_amt1, received_amt2, received_amt3, total_received, deduction_amt, ROUND((billing_amt - total_received - tds_amount - deduction_amt),2) AS balance_amt, IF(total_received > 0, 'Short Amount Received', 'Amount Not Received') AS status FROM (SELECT  client_name,state_name,DATE_FORMAT(invoice_date, '%b-%Y') AS Month,year,invoice_no,DATE_FORMAT(invoice_date, '%d-%m-%Y') AS invoice_date,type,ROUND(amount, 2) AS amount,ROUND((cgst + sgst + igst), 2) AS gst,ROUND((amount + cgst + sgst + igst), 2) AS billing_amt,ROUND(tds_amount, 2) AS tds_amount,ROUND(received_amt, 2) AS received_amt1,ROUND(received_amt2, 2) AS received_amt2,ROUND(received_amt3, 2) AS received_amt3,ROUND((received_amt + received_amt2 + received_amt3), 2) AS total_received,ROUND(IFNULL(deduction_amt, 0), 2) AS deduction_amt,payment_status FROM pay_report_gst g WHERE flag_invoice = 2  " + where_client + "  ORDER BY year , month ASC) AS t1 WHERE (((billing_amt - total_received - tds_amount - deduction_amt) > 100 && total_received > 0)) AND year > 2019 AND type != 'credit' AND  (total_received > 0 AND  (payment_status IS NULL OR payment_status != 'Payment Done'))"; // (payment_status = 'Short Amount Received' OR payment_status IS NULL OR payment_status='GST Hold'))
            DataTable dt4 = new DataTable();
            MySqlDataAdapter dt4cmd = new MySqlDataAdapter(query4, d.con);
            dt4cmd.SelectCommand.CommandTimeout = 400;
            dt4cmd.Fill(dt4);

            // Short Amount In Process            
            //string query5 = " select invoice_date,invoice_no,Month,client_name,state_name as state,ucase(type) as billtype,amount, gst, billing_amt,tds_amount, (billing_amt-tds_amount) as receivable_amt, received_amt1,received_amt2,received_amt3, total_received,deduction_amt, Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Short Amount Received','Amount Not Received') as status from  ( select client_name,state_name,DATE_FORMAT(invoice_date,'%b-%Y') as Month,year,invoice_no,Date_format(invoice_date,'%d-%m-%Y') as invoice_date,type,Round(amount,2) as amount,ROUND((cgst+sgst+igst),2) as gst, Round((amount+cgst+sgst+igst),2) as billing_amt,Round(tds_amount,2) as tds_amount, Round(received_amt,2) as received_amt1,Round(received_amt2,2) as received_amt2 ,Round(received_amt3,2) as received_amt3,Round((received_amt+received_amt2+received_amt3),2) as total_received, Round(IFNULL(deduction_amt,0),2) as deduction_amt from pay_report_gst where flag_invoice=2 AND (total_received > 0 AND payment_status = 'Inprocess') " + where_client + " order by year,month asc ) as t1 where  ((( billing_amt-total_received-tds_amount-deduction_amt)>100 && total_received>0)  || (total_received=0)) and year>2019 AND type !='credit'";
            //DataTable dt5 = new DataTable();
            //MySqlDataAdapter dt5cmd = new MySqlDataAdapter(query5, d.con);
            //dt5cmd.SelectCommand.CommandTimeout = 400;
            //dt5cmd.Fill(dt5);


            //Not Received & Less Received Detail
            DataTable dt1 = new DataTable();
            //string query1 = " select invoice_date,invoice_no,Month,client_name,state_name as state,  ucase(type) as billtype,amount, gst, billing_amt,tds_amount, (billing_amt-tds_amount) as receivable_amt, received_amt1,received_amt2,received_amt3, total_received,deduction_amt, Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Short Amount Received','Amount Not Received') as status from  ( select client_name,state_name,DATE_FORMAT(invoice_date,'%b-%Y') as Month,year,invoice_no,Date_format(invoice_date,'%d-%m-%Y') as invoice_date,type,Round(amount,2) as amount,ROUND((cgst+sgst+igst),2) as gst, Round((amount+cgst+sgst+igst),2) as billing_amt,Round(tds_amount,2) as tds_amount, Round(received_amt,2) as received_amt1,Round(received_amt2,2) as received_amt2 ,Round(received_amt3,2) as received_amt3,Round((received_amt+received_amt2+received_amt3),2) as total_received, Round(IFNULL(deduction_amt,0),2) as deduction_amt from pay_report_gst where flag_invoice=2  " + where_client + " order by year,month asc ) as t1 where  ((( billing_amt-total_received-tds_amount-deduction_amt)>100 && total_received>0)  || (total_received=0)) and year>2019 AND type !='credit'";
            string query1 = " select invoice_date,invoice_no, month,client_name,state_name as state,  ucase(type) as billtype,amount, gst, billing_amt,tds_amount,(billing_amt-tds_amount) as receivable_amt ,received_amt1,received_amt2,received_amt3, total_received,deduction_amt, Round(( billing_amt-total_received-tds_amount-deduction_amt),2) as balance_amt,IF(total_received>0,'Less Received','Not Received') as status from  ( select g.client_name,  If((i.region!='Select' && i.region!='ALL'  && i.region!=''),concat(g.state_name,' (',i.region,')'),g.state_name) as state_name ,DATE_FORMAT(g.invoice_date,'%b-%Y') as month,g.year,g.invoice_no,Date_format(g.invoice_date,'%d/%m/%Y') as invoice_date,g.type,Round(g.amount,2) as amount,ROUND((g.cgst+g.sgst+g.igst),2) as gst, Round((g.amount+g.cgst+g.sgst+g.igst),2) as billing_amt,Round(g.tds_amount,2) as tds_amount, Round(g.received_amt,2) as received_amt1,Round(g.received_amt2,2) as received_amt2 ,Round(g.received_amt3,2) as received_amt3,Round((g.received_amt+g.received_amt2+g.received_amt3),2) as total_received,Round(IFNULL(g.deduction_amt,0),2) as deduction_amt from  pay_report_gst g  left join pay_billing_invoice_history i on g.comp_code=i.comp_code and g.client_code=i.client_code and g.invoice_no=i.invoice_no where g.flag_invoice=2 and   (g.invoice_no is not null && g.invoice_no!='' ) and  (g.invoice_date is not null && g.invoice_date!='') " + where_client + " group by g.invoice_no order by g.year,g.month asc ) as t1 where  ((( billing_amt-total_received-tds_amount-deduction_amt)>100 && total_received>0)  || (total_received=0)) and year>2019 AND type !='credit';";
           
            MySqlDataAdapter dt1cmd = new MySqlDataAdapter(query1, d.con);
            dt1cmd.SelectCommand.CommandTimeout = 400;
            dt1cmd.Fill(dt1);

            double t_bal_amt = 0;
            int Sr_No = 0;
            #region Summary
            body += "<br/><tr><td color=blue><b>Outstanding Summary</b></td></tr>   <br />";
            body += "<table border=1><tr><th bgcolor=yellow>SR NO.</th><th bgcolor=yellow>DESCRIPTION</th><th bgcolor=yellow>OUTSTANDING AMOUNT</th></tr>";
            for (int loopCount = 0; loopCount < dt.Rows.Count; loopCount++)
            {                                                                                                                                                                                                                                                                                                                                                                                  // </td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td></b></tr>";
                Sr_No = Sr_No + 1;
                body += "<tr><td>" + Sr_No + "</td><td>" + dt.Rows[loopCount]["status"] + "</td><td align =right>" + dt.Rows[loopCount]["balance_amt"] + "</td></tr>";
                t_bal_amt += Convert.ToDouble(dt.Rows[loopCount]["balance_amt"]);
            }
            body += "<tr><b> <td align=center ><b>TOTAL OUTSTANDING</b></td><td align =right><b>" + String.Format("{0:n}", t_bal_amt) + "</b></td></tr>";
            body += "</table>  <br /> ";
            #endregion
            #region (A) Amount Not Received Against Invoices(Current)
            int Sr_No1 = 0;
            double one_month_billing_amt = 0;
            if (dt3.Rows.Count > 0)
            {
                body += "<tr><td color=blue><b>(A) Amount Not Received Against Invoices(Current)</b></td></tr>   <br />";
                body += "<table border=1><tr><th bgcolor=yellow>SR NO.</th><th bgcolor=yellow>INVOICE DATE</th><th bgcolor=yellow>INVOICE NO</th><th bgcolor=yellow>MONTH</th><th bgcolor=yellow>CLIENT NAME</th><th bgcolor=yellow>STATE</th><th bgcolor=yellow>INVOICE TYPE</th><th bgcolor=yellow>TAXABLE AMOUNT</th><th bgcolor=yellow>GST</th><th bgcolor=yellow>INVOICE AMOUNT</th></tr>";
                for (int loopCount = 0; loopCount < dt3.Rows.Count; loopCount++)
                {
                    Sr_No1 = Sr_No1 + 1;
                    body += "<tr><td>" + Sr_No1 + "</td><td>" + dt3.Rows[loopCount]["invoice_date"] + "</td><td>" + dt3.Rows[loopCount]["invoice_no"] + "</td><td>" + dt3.Rows[loopCount]["Month"] + "</td><td>" + dt3.Rows[loopCount]["client_name"] + "</td><td>" + dt3.Rows[loopCount]["state"] + "</td><td>" + dt3.Rows[loopCount]["billtype"] + "</td><td align =right>" + dt3.Rows[loopCount]["amount"] + "</td><td align =right>" + dt3.Rows[loopCount]["gst"] + "</td><td align =right>" + dt3.Rows[loopCount]["billing_amt"] + "</td></tr>";
                    one_month_billing_amt += Convert.ToDouble(dt3.Rows[loopCount]["balance_amt"]);
                }
                body += "<tr><b><td align=center colspan= 8><b>TOTAL OUTSTANDING</b></td><td align =right><b>" + String.Format("{0:n}", one_month_billing_amt) + "</b></td></tr>";
                body += "</table>  <br /> ";
            }

            #endregion

            #region (B) Amount Not Received Against Invoice(Old)
            int Sr_No2 = 0;
            double total_billing_amt = 0;
            if (dt2.Rows.Count > 0)
            {
                body += "<tr><td color=blue><b>(B) Amount Not Received Against Invoices(Old)</b></td></tr>   <br />";
                body += "<table border=1><tr><th bgcolor=yellow>SR NO.</th><th bgcolor=yellow>INVOICE DATE</th><th bgcolor=yellow>INVOICE NO</th><th bgcolor=yellow>MONTH</th><th bgcolor=yellow>CLIENT NAME</th><th bgcolor=yellow>STATE</th><th bgcolor=yellow>INVOICE TYPE</th><th bgcolor=yellow>TAXABLE AMOUNT</th><th bgcolor=yellow>GST</th><th bgcolor=yellow>INVOICE AMOUNT</th></tr>";
                for (int loopCount = 0; loopCount < dt2.Rows.Count; loopCount++)
                {
                    //invoice_date, invoice_no, Month, client_name, state, billtype, amount, gst,  billing_amt, tds_amount, received_amt1, received_amt2, received_amt3,  total_received, balance_amt, status
                    Sr_No2 = Sr_No2 + 1;
                    body += "<tr><td>" + Sr_No2 + "</td><td>" + dt2.Rows[loopCount]["invoice_date"] + "</td><td>" + dt2.Rows[loopCount]["invoice_no"] + "</td><td>" + dt2.Rows[loopCount]["Month"] + "</td><td>" + dt2.Rows[loopCount]["client_name"] + "</td><td>" + dt2.Rows[loopCount]["state"] + "</td><td>" + dt2.Rows[loopCount]["billtype"] + "</td><td align =right>" + dt2.Rows[loopCount]["amount"] + "</td><td align =right>" + dt2.Rows[loopCount]["gst"] + "</td><td align =right>" + dt2.Rows[loopCount]["billing_amt"] + "</td></tr>";
                    total_billing_amt += Convert.ToDouble(dt2.Rows[loopCount]["balance_amt"]);

                }
                body += "<tr><b><td align=center colspan= 8><b>TOTAL OUTSTANDING</b></td><td align =right><b>" + String.Format("{0:n}", total_billing_amt) + "</b></td></tr>";
                body += "</table>  <br /> ";
            }
            #endregion

            #region (C) Short Amount Received Against Invoices
            int Sr_No3 = 0;
            double total_balance_amt = 0;
            if (dt4.Rows.Count > 0)
            {
                body += "<tr><td color=blue><b>(C) Short Amount Received Against Invoices</b></td></tr>   <br />";
                body += "<table border=1><tr><th bgcolor=yellow>SR NO.</th><th bgcolor=yellow width=100px>INVOICE DATE</th><th bgcolor=yellow>INVOICE NO</th><th bgcolor=yellow width=80px>MONTH</th><th bgcolor=yellow width=350px>CLIENT NAME</th><th bgcolor=yellow width=100px>STATE</th><th bgcolor=yellow>INVOICE TYPE</th><th bgcolor=yellow>TAXABLE AMOUNT</th><th bgcolor=yellow>GST</th><th bgcolor=yellow>INVOICE AMOUNT</th><th bgcolor=yellow>TDS AMT</th><th bgcolor=yellow>RECEIVABLE AMT</th><th bgcolor=yellow>RECEIVED AMT1</th><th bgcolor=yellow>RECEIVED AMT2</th><th bgcolor=yellow>RECEIVED AMT3</th><th bgcolor=yellow>TOTAL RECEIVED</th><th bgcolor=yellow>BALANCE AMT</th></tr>";
                for (int loopCount = 0; loopCount < dt4.Rows.Count; loopCount++)
                {

                    //invoice_date, invoice_no, Month, client_name, state, billtype, amount, gst,  billing_amt, tds_amount, received_amt1, received_amt2, received_amt3,  total_received, balance_amt, status
                    Sr_No3 = Sr_No3 + 1;
                    body += "<tr><td>" + Sr_No3 + "</td><td>" + dt4.Rows[loopCount]["invoice_date"] + "</td><td>" + dt4.Rows[loopCount]["invoice_no"] + "</td><td>" + dt4.Rows[loopCount]["Month"] + "</td><td>" + dt4.Rows[loopCount]["client_name"] + "</td><td>" + dt4.Rows[loopCount]["state"] + "</td><td>" + dt4.Rows[loopCount]["billtype"] + "</td><td align =right>" + dt4.Rows[loopCount]["amount"] + "</td><td align =right>" + dt4.Rows[loopCount]["gst"] + "</td><td align =right>" + dt4.Rows[loopCount]["billing_amt"] + "</td><td align =right>" + dt4.Rows[loopCount]["tds_amount"] + "</td><td align =right>" + dt4.Rows[loopCount]["receivable_amt"] + "</td><td align =right>" + dt4.Rows[loopCount]["received_amt1"] + "</td><td align =right>" + dt4.Rows[loopCount]["received_amt2"] + "</td><td align =right>" + dt4.Rows[loopCount]["received_amt3"] + "</td><td align =right>" + dt4.Rows[loopCount]["total_received"] + "</td><td align =right>" + dt4.Rows[loopCount]["balance_amt"] + "</td></tr>";
                    total_balance_amt += Convert.ToDouble(dt4.Rows[loopCount]["balance_amt"]);

                }
                body += "<tr><b><td align=center colspan= 15><b>TOTAL OUTSTANDING</b></td><td align =right><b>" + String.Format("{0:n}", total_balance_amt) + "</b></td></tr>";
            } body += "</table>  <br /> ";

            #endregion

            body += "<br/><P> <tr><td  color=red> Note : This is an system generated email.</td></tr> <br/> <tr><td><b>Thanking You,</b></td></tr><tr><td><b>Regards,</b></td></tr> <br/><tr><td>" + designation + "</td></tr><tr><td>" + company + "</td></tr><tr><td>Mobile. " + contact + "</td></tr></P>";//<tr><td><b>" + thanks_regards + "</b></td></tr>
           

            body = "<font color='0a1b63'>" + body + "</font>";
            //string body = "<tr><td style = \"font-family:Georgia;font-size:12pt;\">Dear Sir / Madam, </td></tr><tr><td style = \"font-family:Georgia;font-size:12pt;\">Greetings from IH&MS...!!!</td></tr><tr><td><a href=\"http://localhost:62148/code/'\"+ link_path1 + \"'>Download</a></td></tr>";


            string subject = "Outstanding Bill Details (₹ :  "+String.Format("{0:n}", t_bal_amt)  +"/-)-" + ddl_client_outstanding.SelectedItem.Text + "";
            string txt_toMail = txt_to_mail.Text; string txt_ccmail = txt_cc_mail.Text;
            using (MailMessage mailMessage = new MailMessage())
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mailMessage.From = new MailAddress(from_emailid);
                mailMessage.To.Add(txt_toMail);
                mailMessage.CC.Add(txt_ccmail);
               // mailMessage.bcc.Add(cc_emailid);
                mailMessage.Bcc.Add(bcc_mailid);
                mailMessage.Attachments.Add(new Attachment(zip_filename));

                mailMessage.Subject = subject;//"OUTSTANDING PAYMENT DETAILS OF  " + ddl_client_outstanding.SelectedItem.Text.ToString().ToUpper() + "  ";
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(from_emailid, password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mailMessage);
               
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Mail Sent Successfully...!!!');", true);
                ddl_client_outstanding.SelectedIndex = 0;
                txt_cc_mail.Text = "";
                txt_to_mail.Text = "";
                lit_outstanding_amt.Text = "0";
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Error in Sending Email...!!');", true);
            throw ex;
        }
        finally
        {
            d.con.Close();
            // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Mail Send Successfully.');", true);
        }
    }


    public class MyTemplate_outstanding : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;

        public MyTemplate_outstanding(ListItemType type, DataSet ds)
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

                    lc = new LiteralControl("<table border=1 width=100%><tr><th bgcolor=yellow colspan=19>OUTSTANDING REPORT</th></tr> " +
                        "<tr><th>SR NO.</th><th>INVOICE DATE</th><th>INVOICE NO</th><th>MONTH</th><th>CLIENT NAME</th><th>STATE</th><th>INVOICE TYPE</th>"+
                        "<th>TAXABLE AMOUNT</th><th>GST</th><th>INVOICE AMOUNT</th><th>TDS AMOUNT</th><th>RECEIVABLE AMT</th><th>RECEIVED AMT1</th><th>RECEIVED AMT2</th><th>RECEIVED AMT3</th>"+
                        "<th>TOTAL RECEIVED AMT</th><th>DEDUCTION AMT</th><th>BALANCE AMOUNT</th><th>STATUS</th></tr>");
                
                    break;
                case ListItemType.Item:
                    lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["invoice_no"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["Month"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["client_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["state"] + "</td><td>" + ds.Tables[0].Rows[ctr]["billtype"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst"] + "</td><td>" + ds.Tables[0].Rows[ctr]["billing_amt"] + "</td><td>" + ds.Tables[0].Rows[ctr]["tds_amount"] + "</td><td>" + ds.Tables[0].Rows[ctr]["receivable_amt"] + "</td><td>" + ds.Tables[0].Rows[ctr]["received_amt1"] + "</td> <td>" + ds.Tables[0].Rows[ctr]["received_amt2"] + "</td><td>" + ds.Tables[0].Rows[ctr]["received_amt3"] + "</td><td>" + ds.Tables[0].Rows[ctr]["total_received"] + "</td><td>" + ds.Tables[0].Rows[ctr]["deduction_amt"] + "</td><td>" + ds.Tables[0].Rows[ctr]["balance_amt"] + "</td><td>" + ds.Tables[0].Rows[ctr]["status"] + "</td></tr>");

                    if (ds.Tables[0].Rows.Count == ctr + 1)
                    {
                        lc.Text = lc.Text + "<tr><td align=center colspan=17>TOTAL</td><td>=ROUND(SUM(R3:R" + (ctr + 3) + "),2)</td></tr>";//<td>=ROUND(SUM(F3:F" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(G3:G" + (ctr + 3) + "),2)</td></b>
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
