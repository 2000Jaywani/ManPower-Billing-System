using System;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Threading;
using System.Net.Mail;
using System.Collections.Generic;
using System.Globalization;


public partial class account_department : System.Web.UI.Page
{
    DAL d = new DAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            cash_gv_code();
        }
    }
    protected void btn_approve_Command(object sender, CommandEventArgs e)
    {

        string id = e.CommandArgument.ToString();
        DateTime now = DateTime.Now;
        string accounts_approval_date = now.ToString();
        string Rejection_Reason = "";
        //string debited_to = (gvrow.FindControl("ddl_debt_to") as System.Web.UI.WebControls.DropDownList).SelectedValue;
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        string debited_to = (grdrow.FindControl("ddl_debt_to") as System.Web.UI.WebControls.DropDownList).SelectedValue;
        string narration = (grdrow.FindControl("narration") as System.Web.UI.WebControls.TextBox).Text;
        int cash_v = d.operation("Update pay_cash_voucher set status=2,narration='" + narration + "',debited_to='" + debited_to + " ',Rejection_Reason='" + Rejection_Reason + "', cash_login_id = '" + Session["login_id"].ToString() + "', accounts_approval_date = '" + accounts_approval_date + "' where id=" + id + "");

        if (cash_v > 0)
        {
            cash_gv_code();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Request Approved Successfully... !!!');", true);
        }

    }
    protected void btn_reject_Command(object sender, CommandEventArgs e)
    {
        string id = e.CommandArgument.ToString();
        GridViewRow row = (GridViewRow)(((LinkButton)sender)).NamingContainer;
        string Rejection_Reason = (row.FindControl("rejected_reason") as System.Web.UI.WebControls.TextBox).Text;

        if (Rejection_Reason != "")
        {

            int cash_v = d.operation("Update pay_cash_voucher set status=3,Rejection_Reason='" + Rejection_Reason + "' where id=" + id + "");
            if (cash_v > 0)
            {
                cash_gv_code();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Request Reject Successfully... !!!');", true);
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Rejection Reason....!!!');", true);
            return;
        }



    }

    public void cash_gv_code()
    {

        try
        {
            gv_cash_voucher.DataSource = null;
            gv_cash_voucher.DataBind();
            d.con.Open();
            MySqlDataAdapter adp_grid = new MySqlDataAdapter("select id, case when status = '0' then 'Waiting For Approval' when status = '1' then 'Self Approved' when status ='2' then 'Approved By Accounts' when status ='3' then 'Rejected By Accounts' when status ='4' then 'Approved By HOD'  when status ='5' then 'Rejected By HOD' when status ='6' then 'Approved By Authorised Signatory' when status=7 then 'Rejected By Authorised Signatory' when status=8 then 'Paid' end as 'status', cash_rs,receiver_name,debited_to,DATE_FORMAT(request_date, '%d/%m/%Y') AS 'request_date',narration,soft_copy_file, Rejection_Reason ,status from pay_cash_voucher where comp_code= '" + Session["comp_code"].ToString() + "' AND status in (1,2,3)", d.con);
            DataSet ds = new DataSet();
            adp_grid.Fill(ds);
            gv_cash_voucher.DataSource = ds;
            gv_cash_voucher.DataBind();
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

    protected void gv_cash_voucher_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
        try
        {
            ((LinkButton)e.Row.Cells[9].FindControl("btn_approve")).Visible = true;
            ((LinkButton)e.Row.Cells[10].FindControl("btn_reject")).Visible = true;

        }
        catch { }
        if (e.Row.Cells[8].Text == "Approved By Accounts")
        {
            try
            {
                ((LinkButton)e.Row.Cells[9].FindControl("btn_approve")).Visible = false;
                ((LinkButton)e.Row.Cells[10].FindControl("btn_reject")).Visible = false;
            }
            catch { }
        }
        if (e.Row.Cells[8].Text == "Rejected By Accounts")
        {
            try
            {
                ((LinkButton)e.Row.Cells[9].FindControl("btn_approve")).Visible = false;
                ((LinkButton)e.Row.Cells[10].FindControl("btn_reject")).Visible = false;
            }
            catch { }
        }
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
            e.Row.Cells[1].Visible = false;
           

        }

        
        var drop_down = (DropDownList)e.Row.FindControl("ddl_debt_to");
        if (drop_down != null)
        {
            drop_down.SelectedValue = e.Row.Cells[4].Text;
        }
        string imageUrl;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (DataRowView)e.Row.DataItem;

            if (dr["soft_copy_file"].ToString() != "")
            {
                imageUrl = "~/cash_voucher_images/" + dr["soft_copy_file"];
                (e.Row.FindControl("soft_copy_file") as System.Web.UI.WebControls.Image).ImageUrl = imageUrl;
                if (dr["soft_copy_file"].ToString().ToUpper().Contains(".PDF"))
                {
                    imageUrl = "~/Images/pdf_format.jpg";
                    (e.Row.FindControl("soft_copy_file") as System.Web.UI.WebControls.Image).ImageUrl = imageUrl;
                }
                if (dr["soft_copy_file"].ToString().ToUpper().Contains(".ZIP"))
                {
                    imageUrl = "~/Images/winzip.png";
                    (e.Row.FindControl("soft_copy_file") as System.Web.UI.WebControls.Image).ImageUrl = imageUrl;
                }
            }
        }
        //e.Row.Cells[2].Visible = false;
    }
    protected void Downlaodfile(object sender, EventArgs e)
    {

        try
        {
            LinkButton lnkbtn = sender as LinkButton;
            GridViewRow gvrow = lnkbtn.NamingContainer as GridViewRow;
            GridViewRow row = lnkbtn.NamingContainer as GridViewRow;
            string FilePath = "~/cash_voucher_images/pdf_format.jpg";
            Response.ContentType = "~/cash_voucher_images";
            Response.AddHeader("Content-Disposition", "attachment;filename=\"" + FilePath + "\"");
            Response.TransmitFile(Server.MapPath(FilePath));
            Response.End();

        }
        catch { }
    }


    protected void link_soft_copy_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow1.Cells[1].Text;
        string original_photo = d.getsinglestring("select soft_copy_file from pay_cash_voucher where Id = '" + id + "'  ");
        downloadfile(original_photo);
        //showImage(object sender ,E);
    }
    protected void downloadfile(string filename)
    {
        try
        {
            string path2 = Server.MapPath("~\\cash_voucher_images\\" + filename);

            bool code = File.Exists(path2);
            if (code == true)
            {
                Response.Clear();
                Response.ContentType = "Application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(path2));

                Response.TransmitFile("~\\cash_voucher_images\\" + filename);
                Response.WriteFile(path2);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                Response.End();
                Response.Close();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('File Can not Found!!.');", true);
            }

        }
        catch (Exception ex) { throw ex; }




    }
    protected void gv_cash_voucher_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_cash_voucher.UseAccessibleHeader = false;
            gv_cash_voucher.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }


    protected void btn_tally_report_Click(object sender, EventArgs e)
    {
        //hidtab.Value = "3";
        try
        {
            string where = "";
            string query = "";

            query = "SELECT cash_rs,receiver_name, debited_to, date_format(request_date,'%d/%m/%Y') as 'request_date',narration,payment_method,date_format(payment_date,'%d/%m/%Y') as 'payment_date',rejection_reason,case when status = '0' then 'Waiting For Approval' when status = '1' then 'Self Approved' when status ='2' then 'Approved By Accounts_Department' when status ='3' then 'Rejected By Account_Department' when status ='4' then 'Approved By HOD'  when status ='5' then 'Rejected By HOD' when status ='6' then 'Approved By Authorised Signatory' when status=7 then 'Rejected By Authorised Signatory' when status=8 then 'Paid' end as 'status' FROM pay_cash_voucher where status in(1,2,3)  ";
            MySqlDataAdapter dscmd = new MySqlDataAdapter(query, d.con);
            DataSet ds = new DataSet();
            dscmd.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;

                Response.AddHeader("content-disposition", "attachment;filename=Accounts_Report" + ".xls");
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
                    //lc = new LiteralControl("<table border=1><tr ></tr><tr><th>SR NO.</th><td>Journel</td><th bgcolor=DeepSkyBlue>DATE</th><th bgcolor=DeepSkyBlue>STANDARD NARRATION</th><th bgcolor=IndianRed>Receiver Name </th><th bgcolor=IndianRed>AMOUNT-1</th><th bgcolor=DeepSkyBlue>REFERANCE DUE DAYS</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-2</th><th bgcolor=IndianRed>AMOUNT-2</th><th bgcolor=DeepSkyBlue>STOCK ITEM NAME</th><th bgcolor=SkyBlue>STOCK ITEM QTY</th><th bgcolor=DeepSkyBlue>STOCK ITEM RATE</th><th bgcolor=DeepSkyBlue>STOCK ITEM TOTAL AMT</th><th bgcolor=IndianRed>LEDGER NAME DR/CR-3</th><th bgcolor=IndianRed>AMOUNT-3</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-4</th><th bgcolor=LightCoral>AMOUNT-4</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-5</th><th bgcolor=IndianRed>AMOUNT-5</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-6</th><th bgcolor=LightCoral>AMOUNT-6</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-7</th><th bgcolor=IndianRed>AMOUNT-7</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-8</th><th bgcolor=LightCoral>AMOUNT-8</th><th bgcolor=IndianRed>LEDGER NAME  CR/CR-9</th><th bgcolor=IndianRed>AMOUNT-9</th><th bgcolor=LightCoral>LEDGER NAME  CR/CR-10</th><th bgcolor=LightCoral>AMOUNT-10</th></tr> ");
                    lc = new LiteralControl("<table border=1><tr ></tr><tr><th>SR NO.</th><th bgcolor=LightBlue>Request Date</th><th bgcolor=IndianRed>narration</th><th bgcolor=DeepSkyBlue>Receiver Name</th><th bgcolor=LightBlue>Amount</th><th bgcolor=IndianRed>Debited TO</th><th bgcolor=IndianRed>Status</th><th bgcolor=IndianRed>Rejection Reason</th>");

                    break;
                case ListItemType.Item:
                    DateTimeFormatInfo mfi = new DateTimeFormatInfo();

                    lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["request_date"] + "</td><td> " + ds.Tables[0].Rows[ctr]["narration"] + "</td><td>" + ds.Tables[0].Rows[ctr]["receiver_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["cash_rs"] + "</td><td>" + ds.Tables[0].Rows[ctr]["debited_to"] + "</td><td>" + ds.Tables[0].Rows[ctr]["status"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Rejection_Reason"] + "</td></tr>");
                    // lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["request_date"] + "</td><td>" + ds.Tables[0].Rows[ctr][""] + "</td><td>" + ds.Tables[0].Rows[ctr]["gst_no"] + "</td><td>" + ds.Tables[0].Rows[ctr]["party_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["amount"] + "</td><td>'" + ds.Tables[0].Rows[ctr]["vendor_invoice_no"] + "</td><td></td><td>R&M Expenses_Reimbersment</td><td>-" + ds.Tables[0].Rows[ctr]["gross_amount"] + "</td><td></td><td></td><td></td><td></td><td>CGST</td><td>" + ds.Tables[0].Rows[ctr]["vendor_cgst"] + "</td><td>SGST</td><td>" + ds.Tables[0].Rows[ctr]["vendor_sgst"] + "</td><td>IGST</td><td>" + ds.Tables[0].Rows[ctr]["vendor_igst"] + "</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>");

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




