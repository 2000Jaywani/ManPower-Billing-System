using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using iTextSharp.text.pdf;
using iTextSharp.text.xml.xmp;
using MySql.Data.MySqlClient;
using org.bouncycastle.crypto;
using org.bouncycastle.pkcs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class reprint_invoice : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();
    DAL d3 = new DAL();
    DAL d4 = new DAL();
    DAL d_cg = new DAL();
    BillingSalary bs = new BillingSalary();
    public int arrears_invoice = 0, ot_invoice = 0;
    CrystalDecisions.CrystalReports.Engine.ReportDocument crystalReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
    protected static Queue reportQueue = new Queue();
    public static string month_name = "";
    public string ddl_invoice_slot = "", state_name_arrear_state = "", year2 = "";
    public System.Data.DataTable dt = new System.Data.DataTable();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["comp_code"] == null || Session["comp_code"].ToString() == "")
        {
            Response.Redirect("Home.aspx");
        }

        if (!IsPostBack)
        {
            ddl_client.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CASE WHEN  pay_client_master.client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  pay_client_master.client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  pay_client_master.client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  pay_client_master.client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', pay_client_master.client_code from pay_client_master INNER JOIN pay_client_state_role_grade ON pay_client_master.COMP_CODE = pay_client_state_role_grade.COMP_CODE and pay_client_master.client_code = pay_client_state_role_grade.client_code WHERE pay_client_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_client_state_role_grade.emp_code IN ('" + Session["LOGIN_ID"] + "') and client_active_close='0' group by pay_client_master.client_code ORDER BY client_code", d.con);//AND client_code in(select distinct(client_code) from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE='" + Session["LOGIN_ID"].ToString() + "')
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
                ddl_client.Items.Insert(0, "Select");
                ddl_billing_state.Items.Insert(0, "Select");
                ddl_unitcode.Items.Insert(0, "Select");
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
        ddl_billing_state.Items.Clear();
        ddl_unitcode.Items.Clear();
        ddl_unitcode.Items.Insert(0, "Select");
        ddl_billing_state.Items.Insert(0, "Select");
        if (ddl_client.SelectedValue != "Select")
        {
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = null;
            if (ddl_client.SelectedValue == "DHFL" || ddl_client.SelectedValue == "BAGIC")
            {
                cmd_item = new MySqlDataAdapter("SELECT DISTINCT( STATE_NAME ) FROM  pay_unit_master  INNER JOIN  pay_zone_master  ON  pay_unit_master . comp_code  =  pay_zone_master . comp_code  AND  pay_unit_master . client_code  =  pay_zone_master . client_code  WHERE pay_zone_master. comp_code  = '" + Session["comp_code"].ToString() + "' AND pay_zone_master. client_code  = '" + ddl_client.SelectedValue + "' AND pay_zone_master. type  = 'region' ORDER BY 1", d.con);
                div_region.Visible = true;
            }
            else
            {

                cmd_item = new MySqlDataAdapter("SELECT DISTINCT ( STATE_NAME ) FROM  pay_unit_master  WHERE  comp_code  = '" + Session["comp_code"].ToString() + "' AND  client_code  = '" + ddl_client.SelectedValue + "' ORDER BY 1", d.con);
                div_region.Visible = false;
            }
            d.con.Open();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_billing_state.DataSource = dt_item;
                    ddl_billing_state.DataTextField = dt_item.Columns[0].ToString();
                    ddl_billing_state.DataValueField = dt_item.Columns[0].ToString();
                    ddl_billing_state.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_billing_state.Items.Insert(0, "ALL");
                ddl_unitcode.Items.Insert(0, "Select");
                ddl_state_SelectedIndexChanged(null, null);
                region();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }

    }
    protected void ddl_state_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddl_unitcode.Items.Clear();
        if (ddl_billing_state.SelectedValue != "Select")
        {
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = null;
            if (ddl_client.SelectedValue == "DHFL" || (ddl_client.SelectedValue == "BAGIC" && ddl_billing_state.SelectedValue == "Maharashtra" && int.Parse(txt_month_year.Text.Replace("/", "")) > 42020))
            {
                if (ddl_billing_state.SelectedValue != "ALL")
                {
                    cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name='" + ddl_billing_state.SelectedValue + "' AND Zone = '" + ddlregion.SelectedValue + "' AND pay_unit_master.branch_status = 0  ORDER BY 1", d.con);
                }
                else
                {
                    cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' AND branch_status = 0 AND Zone = '" + ddlregion.SelectedValue + "' ORDER BY 1", d.con);
                }
            }
            else
            {
                if (ddl_billing_state.SelectedValue != "ALL")
                {
                    cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name='" + ddl_billing_state.SelectedValue + "' AND pay_unit_master.branch_status = 0  ORDER BY 1", d.con);
                }
                else
                {
                    cmd_item = new MySqlDataAdapter("SELECT CONCAT((SELECT DISTINCT ( STATE_CODE ) FROM  pay_state_master  WHERE  STATE_NAME  =  pay_unit_master . STATE_NAME ), '_',  UNIT_CITY , '_',  UNIT_ADD1 , '_',  UNIT_NAME ) AS 'UNIT_NAME',  unit_code  FROM  pay_unit_master  WHERE  comp_code  = '" + Session["comp_code"] + "' AND  client_code  = '" + ddl_client.SelectedValue + "' ORDER BY 1", d.con);
                }
            }
            d.con.Open();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
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
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
    }
    protected void region()
    {
        if (ddl_client.SelectedValue != "Select")
        {
            ddlregion.Items.Clear();
            System.Data.DataTable dt_item2 = new System.Data.DataTable();
            MySqlDataAdapter cmd_item2 = new MySqlDataAdapter("SELECT DISTINCT  pay_zone_master.region FROM pay_client_billing_details INNER JOIN pay_zone_master  ON  pay_client_billing_details . comp_code  =  pay_zone_master . comp_code  AND  pay_client_billing_details . client_code  =  pay_zone_master . client_code  WHERE  pay_client_billing_details . client_code  = '" + ddl_client.SelectedValue + "' and type = 'Region' ", d.con);
            d.con.Open();
            try
            {
                cmd_item2.Fill(dt_item2);
                if (dt_item2.Rows.Count > 0)
                {
                    ddlregion.DataSource = dt_item2;
                    ddlregion.DataTextField = dt_item2.Columns[0].ToString();
                    ddlregion.DataValueField = dt_item2.Columns[0].ToString();
                    ddlregion.DataBind();
                }
                dt_item2.Dispose();
                d.con.Close();
                ddlregion.Items.Insert(0, "Select");
                ddlregion.Items.Insert(1, "ALL");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }

        }
    }

    protected void ddl_unitcode_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void ddlregion_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlregion.SelectedValue != "Select")
        {
            ddl_billing_state.Items.Clear();
            ddl_unitcode.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = null;
            if (ddlregion.SelectedValue != "ALL")
            {
                cmd_item = new MySqlDataAdapter("SELECT DISTINCT (STATE_NAME) FROM pay_unit_master WHERE comp_code = '" + Session["comp_code"] + "' AND client_code = '" + ddl_client.SelectedValue + "' AND ZONE = '" + ddlregion.SelectedValue + "' ORDER BY 1", d.con);
            }
            else
            {
                cmd_item = new MySqlDataAdapter("SELECT DISTINCT (STATE_NAME) FROM pay_unit_master WHERE comp_code = '" + Session["comp_code"] + "' AND client_code = '" + ddl_client.SelectedValue + "' ORDER BY 1", d.con);
            }
            d.con.Open();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_billing_state.DataSource = dt_item;
                    ddl_billing_state.DataTextField = dt_item.Columns[0].ToString();
                    ddl_billing_state.DataValueField = dt_item.Columns[0].ToString();
                    ddl_billing_state.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                //ddl_billing_state.Items.Insert(0, "Select");
                ddl_billing_state.Items.Insert(0, "ALL");
                ddl_unitcode.Items.Insert(0, "ALL");
                //ddl_unitcode.Items.Clear();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                d.con.Close();
            }

        }
    }
    protected void btn_show_Click(object sender, EventArgs e)
    {
        try
        {
            gv_invoice.DataSource = null;
            gv_invoice.DataBind();

            string where = "";

            if (ddl_client.SelectedValue == "DHFL" || ddl_client.SelectedValue == "BAGIC")
            {
                if (ddl_client.SelectedValue != "Select")
                {
                    where = "comp_code='" + Session["comp_code"].ToString() + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and client_code='" + ddl_client.SelectedValue + "'";
                    if (ddlregion.SelectedValue != "Select" && ddlregion.SelectedValue != "ALL")
                    {
                        where = "comp_code='" + Session["comp_code"].ToString() + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and client_code='" + ddl_client.SelectedValue + "' and region='" + ddlregion.SelectedValue + "'";
                        if (ddl_billing_state.SelectedValue != "Select" && ddl_billing_state.SelectedValue != "ALL")
                        {
                            where = "comp_code='" + Session["comp_code"].ToString() + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and client_code='" + ddl_client.SelectedValue + "' and region='" + ddlregion.SelectedValue + "' and state_name='" + ddl_billing_state.SelectedValue + "'";
                            if (ddl_unitcode.SelectedValue != "Select" && ddl_unitcode.SelectedValue != "ALL" && ddl_unitcode.SelectedValue != "")
                            {
                                where = "comp_code='" + Session["comp_code"].ToString() + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and client_code='" + ddl_client.SelectedValue + "'and region='" + ddlregion.SelectedValue + "' and state_name='" + ddl_billing_state.SelectedValue + "' and unit_code='" + ddl_unitcode.SelectedValue + "'";
                            }
                        }
                    }
                }
            }
            else
            {
                if (ddl_client.SelectedValue != "Select")
                {
                    where = "comp_code='" + Session["comp_code"].ToString() + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and client_code='" + ddl_client.SelectedValue + "'";
                    if (ddl_billing_state.SelectedValue != "Select" && ddl_billing_state.SelectedValue != "ALL")
                    {
                        where = "comp_code='" + Session["comp_code"].ToString() + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and client_code='" + ddl_client.SelectedValue + "' and state_name='" + ddl_billing_state.SelectedValue + "'";
                        if (ddl_unitcode.SelectedValue != "Select" && ddl_unitcode.SelectedValue != "ALL" && ddl_unitcode.SelectedValue != "")
                        {
                            where = "comp_code='" + Session["comp_code"].ToString() + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and client_code='" + ddl_client.SelectedValue + "' and state_name='" + ddl_billing_state.SelectedValue + "' and unit_code='" + ddl_unitcode.SelectedValue + "'";
                        }
                    }
                }
            }

            d.con.Open(); MySqlDataAdapter adp_grid = new MySqlDataAdapter("select id,type,region,date_format(invoice_date,'%d-%m-%Y') as invoice_date ,invoice_no,region,concat(month,'-',year) as 'month_year',(cgst+igst+sgst+amount) as 'billing_amt',client_code,client_name,state_name,(select unit_name from pay_unit_master where pay_unit_master.comp_code=pay_report_gst.comp_code and pay_unit_master.client_code=pay_report_gst.client_code and pay_unit_master.unit_code=pay_report_gst.unit_code) as 'unit_name',unit_code from pay_report_gst where " + where + " and type!='manual' group by invoice_no order by invoice_no", d.con);

            DataSet ds = new DataSet();
            adp_grid.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                gv_invoice.DataSource = ds;
                gv_invoice.DataBind();
                d.con.Close();
                panel_gv.Visible = true;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Matching Records Found for this month.');", true);
                panel_gv.Visible = false;
            }
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
    protected void gv_invoice_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    string servicename = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "region"));
        //    if (servicename == "" || servicename == null)
        //    {
        //        e.Row.Cells[2].Visible = false;
        //    }
        //    else
        //    {
        //        e.Row.Cells[2].Visible = false;
        //    }
        //}
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    string servicename = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "unit_name"));
        //    if (servicename == "" || servicename == null)
        //    {
        //        e.Row.Cells[2].Visible = false;
        //    }
        //    else
        //    {
        //        e.Row.Cells[2].Visible = false;
        //    }
        //}
    }
    protected void gv_invoice_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_invoice.UseAccessibleHeader = false;
            gv_invoice.HeaderRow.TableSection = TableRowSection.TableHeader;
            panel_gv.Visible = gv_invoice.Rows.Count > 0;
        }
        catch { }
    }
    protected void gv_invoice_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        e.Row.Cells[0].Visible = false;
    }

    protected void lnk_download_invoices_Command(object sender, CommandEventArgs e)
    {
        try
        {
            GridViewRow rwo = (GridViewRow)((LinkButton)sender).NamingContainer;
            string invoice = rwo.Cells[2].Text;
            string bill_date = rwo.Cells[3].Text;
            string bill_type = rwo.Cells[6].Text;
            int i = 0;
            if (invoice != "" && bill_date != "" && bill_type != "")
            {
                download_all_invoice(invoice, bill_type, bill_date, 2, i);
            }
        }
        catch { }

    }
    protected void lnk_download_financecopy_Command(object sender, CommandEventArgs e)
    {
        try
        {
            GridViewRow rwo = (GridViewRow)((LinkButton)sender).NamingContainer;
            string invoice = rwo.Cells[2].Text;
            string bill_date = rwo.Cells[3].Text;
            string bill_type = rwo.Cells[7].Text;
            int i = 0;
            if (invoice != "" && bill_date != "" && bill_type != "")
            {
                if (bill_type == "manpower")
                {
                    i = 2;
                }
                else if (bill_type == "arrears_manpower")
                {
                    i = 7;
                }
                else if (bill_type == "shiftwise_bill")
                {
                    i = 14;
                }
                else if (bill_type == "r_and_m_bill")
                {
                    i = 11;
                }
                else if (bill_type == "administrative_bill")
                {
                    i = 12;
                }
                else if (bill_type == "manpower_ot")
                {
                    i = 9;
                }
                download_all_invoice(invoice, bill_type, bill_date, 1, i);
            }
        }
        catch { }
    }
    protected void lnk_download_breakup_Command(object sender, CommandEventArgs e)
    {
        try
        {
            GridViewRow rwo = (GridViewRow)((LinkButton)sender).NamingContainer;
            string invoice = rwo.Cells[2].Text;
            string bill_date = rwo.Cells[3].Text;
            string bill_type = rwo.Cells[7].Text;
            int i = 0;
            if (invoice != "" && bill_date != "" && bill_type != "")
            {
                if (bill_type == "manpower")
                {
                    i = 5;
                }
                else if (bill_type == "arrears_manpower")
                {
                    i = 6;
                }
                else if (bill_type == "manpower_ot")
                {
                    i = 10;
                }

                download_all_invoice(invoice, bill_type, bill_date, 1, i);
            }
        }
        catch { }
    }
    protected void lnk_download_attendance_Command(object sender, CommandEventArgs e)
    {
        try
        {
            GridViewRow rwo = (GridViewRow)((LinkButton)sender).NamingContainer;
            string invoice = rwo.Cells[2].Text;
            string bill_date = rwo.Cells[3].Text;
            string bill_type = rwo.Cells[7].Text;
            int i = 0;
            if (invoice != "" && bill_date != "" && bill_type != "")
            {
                if (bill_type == "manpower")
                {
                    i = 3;
                }
                else if (bill_type == "manpower_ot")
                {
                    i = 13;
                }
                download_all_invoice(invoice, bill_type, bill_date, 1, i);
            }
        }
        catch { }
    }

    #region Invoice
    //Sachin Start All Invoice Download
    private bool btn_arrears_invoiceClicked = false;

    #region Digital_signature_data
    //DigitalSign
    string Source, Target, Certificate, Password, Author, Title, Subject, Keywords, Creator, Producer, Reason, Contact, Location;
    public void DigitalSign(string downloadname, string filename, string Invpath, string billing_flag)
    {
        try
        {

            Certificate = @"" + Server.MapPath("~/Logs/IHMS DSC.pfx") + "";
            Password = "12345678";
            Source = downloadname;
            string pdf_path = "";
            // string pdf_path = Server.MapPath("~/Invoice_copy/Digital_invoice/" + filename + "");
            if (billing_flag == "2")
            {

                pdf_path = Invpath + "\\DG_" + filename;
                //pdf_path = Invpath + "DG_" + filename;
            }
            else
            {
                pdf_path = Invpath + "Invoice_" + filename;
            }
            //string pdf_path = Invpath + filename;
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
                throw ex;
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
            int numberOfPages = 0;
            if (billing_flag == "2")
            { numberOfPages = 1; }
            else
            {
                numberOfPages = pdfReader.NumberOfPages;
            }


            bool chk_sign = true;//not in use
            pdfs.Sign(Reason, Contact, Location, chk_sign, numberOfPages);
            //// Open the result for demonstration purposes.
            //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Target)
            //{
            //    UseShellExecute = true
            //}
            //);
            if (File.Exists(Source))
            {
                File.Delete(Source);
            }

        }
        catch { }
    }

    public void DigitalSign_invoice_print(string downloadname, string filename, string Invpath)
    {
        Certificate = @"" + Server.MapPath("~/Logs/IHMS DSC.pfx") + "";
        Password = "12345678";
        Source = downloadname;
        // string pdf_path = Server.MapPath("~/Invoice_copy/Digital_invoice/" + filename + "");
        string pdf_path = Invpath + "DG_" + filename;
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

        Response.ContentType = ContentType;
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(Target));
        Response.WriteFile(Target);
        Response.End();




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
    #endregion

    protected void download_all_invoice(string invoice, string type, string bill_date, int counter, int g_report)
    {
        int invoice_fl_man = 0, invoice_arrear = 0;
        string query1 = "", query = "", query2 = "";
        string billing_name = "", grade_code = "", material_type = "", ddl_start_date_common = "", Billing_wise = "", ddl_end_date_common = "", client_code = "", region = "", ddl_arrears_type = "Select", bill_type = "", txt_arrear_monthend = "", designation = "", txt_arrear_month_year = "", month = "", year = "", client_name = "", state_name, unit_code = "", billing_process = "";
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
            else if (ddl_billing_state.SelectedValue != "ALL")
            {
                where = where + " and G.state_name ='" + ddl_billing_state.SelectedValue + "'";
            }
            if (type == "manual")
            {
                invoice_flag = " and final_invoice !='0'";
            }

            if (type == "3")
            {
                query1 = "SELECT DATE_FORMAT(invoice_date, '%d/%m/%Y') AS 'billing_date',  M.zone, G.start_date, G.end_date, U.UNIT_NAME, G.material_type, U.unit_code, G.client_code, G.month, G.year, G.invoice_no, G.client_name, G.state_name, G.type,     CASE WHEN G.type = 'manpower' THEN 'Manpower Billing' WHEN G.type = 'conveyance' THEN 'Conveyance Billing' WHEN G.type = 'driver_conveyance' THEN 'Conveyance Billing' WHEN G.type = 'machine_rental' THEN 'Machine Rental'  WHEN G.type = 'material' THEN 'Material Billing'  WHEN G.type = 'deepclean' THEN 'Deep Clean Billing'        WHEN G.type = 'manpower_ot' THEN 'OT Billing'        WHEN G.type = 'r_and_m_bill' THEN 'R And M Service'        WHEN G.type = 'administrative_bill' THEN 'Administrative Expenses'        WHEN G.type = 'shiftwise_bill' THEN 'Shiftwise Billing'  WHEN G.type = 'office_rent_bill' THEN 'Office Rent Billing'    END AS 'billing_name' FROM pay_report_gst G LEFT JOIN pay_unit_master U ON G.unit_code = U.unit_code AND G.comp_code = U.COMP_CODE LEFT JOIN pay_billing_material_history M ON G.comp_code = M.comp_code AND G.state_name = M.state_name  AND G.month=M.month AND G.client_code = M.client_code WHERE G.invoice_no='" + invoice + "' group by G.invoice_no,M.zone ORDER BY billing_date , G.type";
            }
            else
            {
                query1 = "SELECT DATE_FORMAT(invoice_date, '%d/%m/%Y') AS 'billing_date',region,start_date,end_date,material_type, UNIT_NAME, U.unit_code, G.client_code, month, year, invoice_no, G.client_name, G.state_name, G.type,    CASE WHEN G.type = 'manpower' THEN 'Manpower Billing' WHEN G.type = 'conveyance' THEN 'Conveyance Billing' WHEN G.type = 'driver_conveyance' THEN 'Conveyance Billing' WHEN G.type = 'machine_rental' THEN 'Machine Rental'        WHEN G.type = 'material' THEN 'Material Billing'        WHEN G.type = 'deepclean' THEN 'Deep Clean Billing'        WHEN G.type = 'manpower_ot' THEN 'OT Billing'        WHEN G.type = 'r_and_m_bill' THEN 'R And M Service'  WHEN G.type = 'administrative_bill' THEN 'Administrative Expenses'  WHEN G.type = 'shiftwise_bill' THEN 'Shiftwise Billing'  WHEN G.type = 'office_rent_bill' THEN 'Office Rent Billing'    END AS 'billing_name' FROM pay_report_gst G LEFT JOIN pay_unit_master U ON G.unit_code = U.unit_code AND G.comp_code = U.COMP_CODE  WHERE invoice_no='" + invoice + "'  ORDER BY billing_date , G.type";
            }

            MySqlDataAdapter dscmd = new MySqlDataAdapter(query1, d.con);
            DataSet ds = new DataSet();

            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);

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

                        ddl_invoice_slot = d1.getsinglestring("select distinct invoice_slot from pay_billing_r_m where auto_invoice_no='" + invoice + "'");

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

                    if (unit_code == "" || Billing_wise == "Statewise")
                    {
                        unit_code = "ALL";
                    }

                    int s_d = 0;
                    int e_d = 0; string str_date = "", end_date = "";

                    if (bill_type == "arrears_manpower")
                    {
                        btn_arrears_invoiceClicked = true;
                        arrears_invoice = 1;
                    }
                    else
                    {
                        btn_arrears_invoiceClicked = false;
                        arrears_invoice = 0;
                    }


                    if (bill_type == "manpower_ot")
                    {
                        ot_invoice = 1;
                    }
                    else
                    {
                        ot_invoice = 0;
                    }
                    if (ddl_client.SelectedValue == "4")
                    {
                        billing_process = "Non Metro";
                    }
                    else
                    {
                        billing_process = "Regular";
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
                    string start_date = get_start_date();
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
                    if (counter == 1)
                    {
                        if (bill_type == "material" || bill_type == "driver_conveyance" || bill_type == "deepclean" || bill_type == "material")
                        {
                            material(g_report, designation, material_type, bill_type, invoice_type);
                        }
                        else
                        {
                            generate_report(g_report, 0, bill_type, str_date, end_date, billing_process, ddl_arrears_type, designation, invoice_type, txt_arrear_month_year, txt_arrear_monthend, unit_code, state_name, invoice);
                        }
                    }
                    else
                    {
                        //1) Manpower Type and  7) Arrears Manpower
                        if (bill_type == "manpower" || bill_type == "arrears_manpower" || bill_type == "manpower_ot")
                        {
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
                            ReportLoad_DigitalInvoice(query, dowmloadname, invoice, bill_date);

                        }

                         //2) Convaynce Bill type
                        else if (bill_type == "2" || bill_type == "conveyance")
                        {
                            conveyance_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, Billing_wise);
                        }

                        //3) Driver Convaynace Bill Type
                        else if (bill_type == "3" || bill_type == "driver_conveyance")
                        {
                            Driver_conveyance_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, Billing_wise);
                        }

                        //4) Material Bill type
                        else if (bill_type == "4" || bill_type == "material")
                        {
                            Material_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, Billing_wise, material_type);
                        }

                        //5) DeepClean
                        else if (bill_type == "5" || bill_type == "deepclean")
                        {
                            DeepClean_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, Billing_wise); ;
                        }

                        //6) Machine Rental
                        else if (bill_type == "6" || bill_type == "machine_rental")
                        {
                            Machine_rental_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, bill_type, Billing_wise, material_type);
                        }

                        //9) R_&_M
                        else if (bill_type == "9" || bill_type == "r_and_m_bill")
                        {
                            R_and_M_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, bill_type, Billing_wise);
                        }

                        //10) Administrative
                        else if (bill_type == "10" || bill_type == "administrative_bill")
                        {
                            Administrative_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, bill_type, Billing_wise);
                        }

                        //11) shiftwise_bill
                        else if (bill_type == "11" || bill_type == "shiftwise_bill")
                        {
                            shiftwise_bill_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, bill_type, Billing_wise);
                        }

                        //12) incentive_bill
                        //else if (bill_type == "12")
                        //{
                        //    incentive_bill_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, folderPath, bill_type);
                        //}

                        //13) office_rent
                        else if (bill_type == "13" || bill_type == "office_rent_bill")
                        {
                            office_rent_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, bill_type, Billing_wise);
                        }

                    }
                }
                //END
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

    private void ReportLoad_DigitalInvoice(string query, string downloadfilename, string invoice, string bill_date)
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
                ot_applicable = d.getsinglestring("SELECT round((sum(pay_billing_unit_rate_history.Amount) + sum(pay_billing_unit_rate_history.uniform) + sum(pay_billing_unit_rate_history.operational_cost) + sum(pay_billing_unit_rate_history.Service_charge)),0) as Total FROM pay_billing_unit_rate_history where pay_billing_unit_rate_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_unit_rate_history.state_name = '" + ddl_billing_state.SelectedItem + "' AND pay_billing_unit_rate_history.month = '" + txt_month_year.Text.ToString().Substring(0, 2) + "' AND pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.ToString().Substring(3, 4) + "' AND (emp_code != '' OR emp_code IS NOT NULL) AND start_date = '0' AND end_date = '0' AND (bill_type IS NULL || bill_type = '') group by pay_billing_unit_rate_history.client_code ");
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
            if (ddl_client.SelectedValue == "RNLIC RM")
            {
                file_name = invoice + "_" + ddl_billing_state.SelectedValue + " " + ddl_invoice_slot + ".pdf";
            }
            else
            {
                file_name = invoice + "_" + ddl_billing_state.SelectedValue + ".pdf";
            }
            string filepath = Server.MapPath("~/Invoice_copy\\" + file_name);
            string Invpath = Server.MapPath("~/Invoice_copy\\");

            //if (ViewState["ALL_STATE"].ToString().Equals("1"))
            //{
            //    crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "final_invoice\\" + ddl_client.SelectedValue.Replace(" ", "_")) + "\\" + ddl_billing_state.SelectedValue + ".pdf");
            //}
            //else
            //{
            //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, downloadname);
            //crystalReport.Close();
            //crystalReport.Clone();
            //crystalReport.Dispose();
            //Response.End();


            if (Session["COMP_CODE"].ToString() == "C01")
            {
                if (INV_bill_date != "")
                {
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }
                    crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, filepath);
                    DigitalSign_invoice_print(filepath, file_name, Invpath);
                }
                else
                {
                    //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, downloadname);
                }
            }
            else
            {

                // crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, downloadname);
                if (Session["COMP_CODE"].ToString() == "C02")
                {
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }
                    crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, filepath);
                    string stamp_pdf = "Invoice" + "_" + file_name;
                    add_stamp_on_invoice(filepath, Invpath, stamp_pdf);
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }
                    Response.ContentType = ContentType;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(Invpath + stamp_pdf));
                    Response.WriteFile(Invpath + stamp_pdf);
                    Response.End();
                }
            }

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

    private void ReportLoad(string query, string downloadfilename, string invoice, string bill_date)
    {

        string ot_applicable = "", machine_rental = "", handaling_amount = "";
        string headerpath = null;
        string footerpath = null;
        //Material Invoice
        try
        {

            reportQueue.Enqueue(crystalReport);
            if (reportQueue.Count > 5) ((ReportDocument)reportQueue.Dequeue()).Dispose();
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
                ot_applicable = d.getsinglestring("SELECT round((sum(pay_billing_unit_rate_history.Amount) + sum(pay_billing_unit_rate_history.uniform) + sum(pay_billing_unit_rate_history.operational_cost) + sum(pay_billing_unit_rate_history.Service_charge)),0) as Total FROM pay_billing_unit_rate_history where pay_billing_unit_rate_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_unit_rate_history.state_name = '" + ddl_billing_state.SelectedItem + "' AND pay_billing_unit_rate_history.month = '" + txt_month_year.Text.ToString().Substring(0, 2) + "' AND pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.ToString().Substring(3, 4) + "' AND (emp_code != '' OR emp_code IS NOT NULL) AND start_date = '0' AND end_date = '0' AND (bill_type IS NULL || bill_type = '') group by pay_billing_unit_rate_history.client_code ");
                bill_date = dt.Rows[0][0].ToString();

            }
            else if (ddl_client.SelectedValue == "RCPL" && downloadfilename == "Material Invoice")
            {

                bill_date = dt.Rows[0][0].ToString();

            }
            d.con.Close();


            // Vishal Einvoice code start
            string irn_no = "", qr_img = "", ack_no = "", ack_time = "";
            try
            {
                string e_invoice_status = d1.getsinglestring("select e_invoice_status from pay_report_gst where  invoice_no='" + invoice + "'");
                if (e_invoice_status == "1")
                {
                    System.Data.DataTable dt1 = new System.Data.DataTable();
                    MySqlCommand cmd1 = new MySqlCommand("Select   client_code, client_name, invoice_no,invoice_date,  irnno, irn_gstin, ack_no,DATE_FORMAT(ack_date,'%d/%m/%Y %H:%m:%s') as  ack_date, qr_code_image from pay_einvoice_detail where status=1 and invoice_no='" + invoice + "'", d.con);
                    MySqlDataAdapter sda1 = new MySqlDataAdapter(cmd1);

                    d.con.Open();
                    sda1.Fill(dt1);
                    d.con.Close();

                    if (dt1.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt1.Rows)
                        {
                            crystalReport.DataDefinition.FormulaFields["irn_no"].Text = @"'" + "IRN.: " + dr["irnno"] + "'";
                            crystalReport.DataDefinition.FormulaFields["ack_no"].Text = @"'" + "Ack No : " + dr["ack_no"] + "'";
                            crystalReport.DataDefinition.FormulaFields["ack_time"].Text = @"'" + "Ack Date : " + dr["ack_date"] + "'";

                            qr_img = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "E_Invoice_code\\" + dr["qr_code_image"].ToString() + "");
                            crystalReport.DataDefinition.FormulaFields["qr_code"].Text = @"'" + qr_img + "'";


                        }
                    }


                }
            }
            catch { }
            //Vishal  Einvoice code end


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
            //if (ViewState["ALL_STATE"].ToString().Equals("1"))
            //{
            //    crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "final_invoice\\" + ddl_client.SelectedValue.Replace(" ", "_")) + "\\" + ddl_billing_state.SelectedValue + ".pdf");
            //}
            //else
            //{
            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, downloadname);
            //  crystalReport.Close();
            //  crystalReport.Clone();
            //  crystalReport.Dispose();
            // //  Response.End();
            //}

            //ViewState["ALL_STATE"] = "0";
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            //Sachin Changes for maximum report error 06-07-2022
            reportQueue.Enqueue(crystalReport);
            if (reportQueue.Count > 5) ((ReportDocument)reportQueue.Dequeue()).Dispose();
            if (crystalReport != null)
            {
                crystalReport.Close();
                crystalReport.Clone();
                crystalReport.Dispose();
                d.con.Close();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            //END
        }
    }

    private void add_stamp_on_invoice(string pdfFile, string path, string stamp)
    {
        PdfReader pdfReader = null;
        PdfStamper pdfStamper = null;

        // Open the PDF file to be signed
        pdfReader = new PdfReader(pdfFile);

        // Output stream to write the stamped PDF to
        using (FileStream outStream = new FileStream(path + stamp, FileMode.Create))
        {
            try
            {
                // Stamper to stamp the PDF with a signature
                pdfStamper = new PdfStamper(pdfReader, outStream);

                // Load signature image
                string stamp_image = "";// System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C01_stamp.jpg");
                if (Session["comp_code"].ToString() == "C02")
                {
                    stamp_image = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\C02_stamp_softcopy.png");
                }

                iTextSharp.text.Image sigImg = iTextSharp.text.Image.GetInstance(stamp_image);

                // Scale image to fit
                sigImg.ScaleToFit(95f, 95f);

                // Set signature position on page
                sigImg.SetAbsolutePosition(60, 90);

                // Add signatures to desired page
                PdfReader pdfReader1 = new PdfReader(pdfFile);
                int numberOfPages = pdfReader1.NumberOfPages;

                PdfContentByte over = pdfStamper.GetOverContent(numberOfPages);
                over.AddImage(sigImg);
            }
            finally
            {
                // Clean up
                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }

    }

    protected string get_start_date()
    {
        return d1.getsinglestring("SELECT IFNULL((SELECT start_date_common FROM pay_billing_master_history INNER JOIN pay_unit_master ON pay_billing_master_history.billing_unit_code = pay_unit_master.unit_code AND pay_billing_master_history.comp_code = pay_unit_master.comp_code WHERE pay_billing_master_history.billing_client_code = '" + ddl_client.SelectedValue + "' AND month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' and  pay_billing_master_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' limit 1),(SELECT start_date_common FROM pay_billing_master INNER JOIN pay_unit_master ON pay_billing_master.billing_unit_code = pay_unit_master.unit_code AND pay_billing_master.comp_code = pay_unit_master.comp_code WHERE pay_billing_master.billing_client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' limit 1))");
    }

    protected string get_end_date()
    {
        return d1.getsinglestring("SELECT IFNULL((SELECT end_date_common FROM pay_billing_master_history INNER JOIN pay_unit_master ON pay_billing_master_history.billing_unit_code = pay_unit_master.unit_code AND pay_billing_master_history.comp_code = pay_unit_master.comp_code WHERE pay_billing_master_history.billing_client_code = '" + ddl_client.SelectedValue + "' AND month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' and  pay_billing_master_history.comp_code = '" + Session["COMP_CODE"].ToString() + "' limit 1),(SELECT end_date_common FROM pay_billing_master INNER JOIN pay_unit_master ON pay_billing_master.billing_unit_code = pay_unit_master.unit_code AND pay_billing_master.comp_code = pay_unit_master.comp_code WHERE pay_billing_master.billing_client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' limit 1))");
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

    protected void conveyance_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month, string year, string bill_wise)
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

        string start_date_common = get_start_date();
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
        ReportLoad_DigitalInvoice(query_con, "Conveyance", invoice, bill_date);
    }

    protected void Driver_conveyance_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month, string year, string bill_wise)
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
        string start_date_common = get_start_date();
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

        ReportLoad(query_con, "driver_conveyance", invoice, bill_date);
    }

    protected void Material_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month_m, string year_y, string bill_wise, string material_type_tissue)
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

        string start_date_common = get_start_date(), where_fix = "", where_clause = "";

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

        ReportLoad_DigitalInvoice(query_con, "Material Invoice", invoice, bill_date);
    }

    protected void DeepClean_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month, string year, string bill_wise)
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
        string start_date_common = get_start_date();
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

        ReportLoad_DigitalInvoice(query, "Deep Clean", invoice, bill_date);

    }

    protected void Machine_rental_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month, string year, string type, string bill_wise, string material_tissue)
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

        string INV_bill_date = bill_date, bill_types = type;

        int month2 = Convert.ToInt32(month);

        int year3 = Convert.ToInt32(year);

        string month_m = string.Format(String.Format("{0:D2}", month2));

        int month_i = Convert.ToInt32(month_m);

        string headerpath = null;
        string footerpath = null;

        if (ddl_client.SelectedValue.Equals("RCPL"))
        {
            Material_type(client_code, unit_code, state_name, region, bill_date, client_name, invoice, dowmloadname, month, year, bill_wise, material_tissue);
            return;
        }
        try
        {
            string daterange = "concat(upper(DATE_FORMAT(str_to_date('" + year + "-" + month_i + "-01','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(LAST_DAY('" + year + "-" + month_i + "-01'), '%d %b %Y'))) as start_end_date";

            string start_date_common = get_start_date();

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
            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, "TaxInvoice");
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

    protected void R_and_M_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month, string year, string type, string bill_wise)
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

        string start_date = get_start_date();

        string txt_month_year1 = "";

        string invoice_type = "CLUB";

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

        ReportLoad_DigitalInvoice(query, dowmloadname, invoice, bill_date);

    }

    protected void Administrative_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month_m, string year_y, string type, string bill_wise)
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

        string start_date = get_start_date();

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

        ReportLoad(query, dowmloadname, invoice, bill_date);
    }

    protected void shiftwise_bill_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month_m, string year_y, string type, string bill_wise)
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

        string start_date = get_start_date();

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

        ReportLoad_DigitalInvoice(query, dowmloadname, invoice, bill_date);
    }

    protected void office_rent_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month_m, string year_y, string type, string bill_wise)
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

        string start_date = get_start_date();

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

        ReportLoad_DigitalInvoice(query, dowmloadname, invoice, bill_date);
    }

    protected void incentive_bill_type(string client_code, string unit_code, string state_name, string region, string bill_date, string client_name, string invoice, string dowmloadname, string month_m, string year_y, string type)
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
    #endregion

    #region FINANCECOPY,Attendance,Breakup Manpower etc
    private StringWriter generate_report(int i, int type_cl, string billing_type1, string ddl_start_date_common, string ddl_end_date_common, string ddl_billing_process, string ddl_arrears_type, string ddl_designatione, string ddl_invoice_type, string txt_arrear_month_year, string txt_arrear_monthend, string ddl_unitcode, string ddl_billing_state, string auto_inv_no)
    {
        //for region changes vinod pol
        #region
        if (billing_type1 == "r_and_m")
        {
        }
        string where_state = "", where_state_arrears = "", region_order = "";
        if (ddl_billing_state.Equals("Maharashtra") && type_cl.Equals(0) && ddl_client.SelectedValue.Equals("BAGIC") && int.Parse(("" + txt_month_year.Text.Substring(3) + "" + txt_month_year.Text.Substring(0, 2) + "")) > 20204 && billing_type1.Equals("1")) { where_state = " and state='" + ddl_billing_state + "' and billingwise_id = 5"; }
        if (d.getsinglestring("select billingwise_id from pay_client_billing_details where client_code = '" + ddl_client.SelectedValue + "' " + where_state).Equals("5"))
        {
            if (!ddlregion.SelectedValue.Equals("ALL") && !ddlregion.SelectedValue.Equals("Select"))
            {
                if (!ddl_billing_state.Equals("ALL")) { where_state = " and state='" + ddl_billing_state + "'"; }

                where_state = " and pay_billing_unit_rate_history.zone = '" + ddlregion.SelectedValue + "'";
                where_state_arrears = " and pay_billing_unit_rate_history_arrears.zone = '" + ddlregion.SelectedValue + "'";

            }
            else
            {
                ddl_billing_state = "ALL";
                ddl_unitcode = "ALL";
                region_order = " pay_billing_unit_rate_history.txt_zone, pay_billing_unit_rate_history.Zone, ";
            }
        }
        else
        { where_state = ""; }
        string billing_bfl = "";
        if (ddl_billing_process != "Regular")
        {
            billing_bfl = " and branch_type ='" + ddl_billing_process + "'";
        }

        double cell = 0;
        DateTimeFormatInfo mfi = new DateTimeFormatInfo();
        month_name = mfi.GetMonthName(int.Parse(txt_month_year.Text.Substring(0, 2))).ToString();
        month_name = month_name + " " + txt_month_year.Text.Substring(3).ToUpper();

        string where = "", from_to_date = " and pay_billing_unit_rate_history.start_date = '" + ddl_start_date_common + "' and pay_billing_unit_rate_history.end_date  = '" + ddl_end_date_common + "'  ";
        string order_by_clause = "   ORDER BY " + region_order + " pay_billing_unit_rate_history.client,pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name,pay_billing_unit_rate_history.emp_name";
        string R_M_order_by_clause = "   ORDER BY client,state_name,unit_name,emp_name";
        string grade = "";
        string pay_attendance_muster = " pay_attendance_muster ", pay_billing_master_history = "pay_billing_master_history", pay_billing_unit_rate = "pay_billing_unit_rate";

        string sql = null, flag = "and pay_attendance_muster.flag != 0 ";

        string invoice = "";
        string bill_date = "", billing_type = "And (bill_type is null || bill_type ='')";
        int month_days = 0;
        if (ddl_start_date_common != "0" && ddl_end_date_common != "0")
        {

            from_to_date = " and pay_billing_unit_rate_history.start_date  = '" + ddl_start_date_common + "' and pay_billing_unit_rate_history.end_date  = '" + ddl_end_date_common + "'  ";
            pay_billing_master_history = "pay_billing_from_to_history as pay_billing_master_history";
            pay_billing_unit_rate = "pay_billing_from_to_unit_rate as pay_billing_unit_rate";
            flag = "";
        }


        //invoice and bill date 
        string invoice_bill_date = bs.get_invoice_bill_date(Session["COMP_CODE"].ToString(), ddl_client.SelectedValue, ddl_billing_state, ddl_unitcode, ddl_invoice_type, ddl_designatione, txt_month_year.Text, int.Parse(ddl_start_date_common), int.Parse(ddl_end_date_common), billing_type, ddlregion.SelectedValue, arrears_invoice, txt_month_year.Text, ddl_arrears_type, ot_invoice, ddl_billing_process, "");
        if (i == 11) { invoice_bill_date = bs.get_invoice_bill_date(Session["COMP_CODE"].ToString(), ddl_client.SelectedValue, ddl_billing_state, ddl_unitcode, "4", ddl_designatione, txt_month_year.Text, int.Parse(ddl_start_date_common), int.Parse(ddl_end_date_common), "r_m_bill", ddlregion.SelectedValue, arrears_invoice, txt_month_year.Text, ddl_arrears_type, ot_invoice, ddl_billing_process, ddl_invoice_slot); }
        if (i == 12) { invoice_bill_date = bs.get_invoice_bill_date(Session["COMP_CODE"].ToString(), ddl_client.SelectedValue, ddl_billing_state, ddl_unitcode, "5", ddl_designatione, txt_month_year.Text, int.Parse(ddl_start_date_common), int.Parse(ddl_end_date_common), "administrative_bill", ddlregion.SelectedValue, arrears_invoice, txt_month_year.Text, ddl_arrears_type, ot_invoice, ddl_billing_process, ""); }
        if (invoice_bill_date.Equals(""))
        {
            invoice = "";
            bill_date = "";
        }
        else
        {
            var invoice_bill = invoice_bill_date.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            invoice = invoice_bill[0].ToString();
            bill_date = invoice_bill[1].ToString();
        }

        string start_date_common = get_start_date();

        if (ddl_invoice_type == "2")
        {
            grade = " and pay_billing_unit_rate_history.grade_code = '" + ddl_designatione + "'";

        }
        if (ddl_invoice_type == "2" && ddl_arrears_type != "Select")
        {
            grade = " and pay_billing_unit_rate_history_arrears.grade_code = '" + ddl_designatione + "'";

        }


        d.con.Open();
        try
        {
            if (i == 1)
            {
                string multi = "";
                if (type_cl == 1)
                {
                    multi = " and pay_billing_unit_rate_history.invoice_flag!=0 ";
                }

                grade = grade + " " + from_to_date;
                string hdfc = "", ot_type = "";
                ot_type = d.getsinglestring("SELECT billing_ot FROM pay_client_master WHERE comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' ");

                //if (ddl_client.SelectedValue == "HDFC")
                //{ hdfc = "AND pay_billing_unit_rate_history.hdfc_type='manpower_bill'"; }
                if (ot_type == "Without OT")
                { hdfc = "AND (pay_billing_unit_rate_history.hdfc_type is null || pay_billing_unit_rate_history.hdfc_type='manpower_bill')"; }

                if (ddl_client.SelectedValue != "ALL")
                {
                    where = " WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history . state_name  = '" + ddl_billing_state + "' and pay_billing_unit_rate_history.unit_code = '" + ddl_unitcode + "' " + billing_bfl + " and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' " + hdfc + " and pay_billing_unit_rate_history.tot_days_present > 0  " + where_state + grade;
                }
                if (ddl_billing_state == "ALL")
                {
                    where = " WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "'  and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "'  " + billing_bfl + "  " + hdfc + "  and pay_billing_unit_rate_history.tot_days_present > 0 " + where_state + grade;
                }
                else if (ddl_unitcode == "ALL")
                {
                    where = " WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history . state_name  = '" + ddl_billing_state + "'  and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "'  " + billing_bfl + " and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' " + hdfc + " and pay_billing_unit_rate_history.tot_days_present > 0 " + where_state + grade;
                }
                if (ddl_client.SelectedValue == "ALL")
                {
                    where = " WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "'  " + billing_bfl + "  " + hdfc + " and pay_billing_unit_rate_history.tot_days_present > 0 " + where_state + grade;
                }
                //sql = "SELECT  state_name ,  unit_name ,  unit_city ,  emp_name ,  grade_desc ,DUTYHRS ,  tot_days_present ,  basic ,  vda ,  emp_basic_vda ,  bonus_rate ,  washing ,  travelling ,  education ,  allowances_esic ,  cca_billing ,  other_allow ,  bonus_gross ,  leave_gross ,  gratuity_gross ,  hra ,  special_allowance ,  gross ,  bonus_after_gross ,  leave_after_gross ,  gratuity_after_gross ,  NH ,  pf ,  esic ,  uniform_ser ,  group_insurance_billing ,  lwf ,  operational_cost ,  allowances_no_esic , ( gross  +  bonus_after_gross  +  leave_after_gross  +  gratuity_after_gross  +  NH  +  pf  +  esic  +  lwf  +  uniform_ser  +  operational_cost  +  allowances_no_esic ) AS 'sub_total_a',  ot_pr_hr_rate ,  esi_on_ot_amount ,  ot_hours , ( ot_pr_hr_rate  +  esi_on_ot_amount ) AS 'sub_total_b', ( gross  +  bonus_after_gross  +  leave_after_gross  +  gratuity_after_gross  +  NH  +  pf  +  esic  +  lwf  +  uniform_ser  +  operational_cost  +  allowances_no_esic  +  ot_pr_hr_rate  +  esi_on_ot_amount ) AS 'sub_total_ab',  relieving_charg , CASE WHEN  emp_cca  = 0 AND  branch_cca  != 0 THEN ((baseamount-bill_ot_rate)) WHEN  emp_cca  != 0 AND  branch_cca  != 0 THEN ((baseamount-bill_ot_rate)) WHEN  emp_cca  = 0 AND  branch_cca  = 0 THEN ((baseamount-bill_ot_rate)) ELSE ( bill_gross  + (( bill_gross  *  esic_percent ) / 100) +  bill_pf +lwf +  bill_uniform  +  group_insurance_billing_ser  +  bonus_after_gross  +  leave_after_gross  +  gratuity_after_gross ) END AS 'sub_total_c',  uniform_no_ser ,  operational_cost_no_ser , IF(((( Total  +  pf  +  esic  + ( ot_rate  *  ot_hours ) +  group_insurance_billing_ser ) *  bill_service_charge ) / 100) = 0,  bill_service_charge_amount , ((( Total  +  pf  +  esic  + ( ot_rate  *  ot_hours ) +  group_insurance_billing_ser ) *  bill_service_charge ) / 100)) AS 'Service_charge', (( Total  + ( ot_rate  *  ot_hours ) +  pf  +  esic  +  group_insurance_billing_ser  +  uniform_no_ser  +  operational_cost_no_ser  +  group_insurance_billing ) + IF(((( Total  +  pf  +  esic  + ( ot_rate  *  ot_hours ) +  group_insurance_billing_ser ) *  bill_service_charge ) / 100) = 0,  bill_service_charge_amount , ((( Total  +  pf  +  esic  + ( ot_rate  *  ot_hours ) +  group_insurance_billing_ser ) *  bill_service_charge ) / 100))) AS 'Amount',  pf_percent  AS 'bill_pf_percent',  esic_percent  AS 'bill_esic_percent',  gratuity_percent ,  hra_percent ,  bill_bonus_percent ,  leave_days ,  bill_service_charge,group_insurance_billing_ser,(ot_rate * ot_hours) as 'ot_amount'  FROM (SELECT  client ,  company_state ,  unit_name ,  state_name ,  unit_city ,  client_branch_code ,  emp_name ,  grade_desc ,  emp_basic_vda ,  hra ,  bonus_gross ,  leave_gross ,  gratuity_gross ,  washing ,  travelling ,  education ,  cca_billing ,  other_allow , ( emp_basic_vda  +  hra  +  bonus_gross  +  leave_gross  +  washing  +  travelling  +  education  +  allowances  +  cca_billing  +  other_allow  +  gratuity_gross  +  hrs_12_ot ) AS 'gross',  bonus_after_gross ,  leave_after_gross ,  gratuity_after_gross , ((( emp_basic_vda ) / 100) *  pf_percent ) AS 'pf', ((( emp_basic_vda  +  hra  +  bonus_gross  +  leave_gross  +  washing  +  travelling  +  education  + IF( esic_oa_billing  = 1,  allowances , 0) +  cca_billing  +  other_allow  +  gratuity_gross  +  hrs_12_ot ) / 100) *  esic_percent ) AS 'esic',  hrs_12_ot  AS 'special_allowance', ((( hrs_12_ot ) *  esic_percent ) / 100) AS 'esic_ot',  lwf , CASE WHEN  bill_ser_uniform  = 1 THEN  uniform  ELSE 0 END AS 'uniform_ser', CASE WHEN  bill_ser_uniform  = 0 THEN  uniform  ELSE 0 END AS 'uniform_no_ser',  relieving_charg , CASE WHEN  bill_ser_operations  = 1 THEN  operational_cost  ELSE 0 END AS 'operational_cost', CASE WHEN  bill_ser_operations  = 0 THEN  operational_cost  ELSE 0 END AS 'operational_cost_no_ser',  tot_days_present , ( emp_basic_vda  +  hra  +  bonus_gross  +  leave_gross  +  washing  +  travelling  +  education  +  allowances  +  cca_billing  +  other_allow  +  gratuity_gross  +  bonus_after_gross  +  leave_after_gross  +  gratuity_after_gross  +  lwf + CASE WHEN  bill_ser_uniform  = 0 THEN 0 ELSE  uniform  END +  relieving_charg  + CASE WHEN  bill_ser_operations  = 0 THEN 0 ELSE  operational_cost  END +  NH  +  hrs_12_ot  + IF( esic_common_allow  = 0,  common_allow , 0)) AS 'Total',  bill_service_charge ,  NH ,  hours , ( bill_gross ) AS 'bill_gross',  sub_total_c ,  bill_ser_uniform ,  bill_ser_operations , (IF(ot_hours > 0,ot_rate,0) + IF(ot_hours > 0 and ot_rate > 0,esi_on_ot_amount,0)) AS 'ot_rate',(ot_rate+esi_on_ot_amount) as 'bill_ot_rate',  ot_hours ,  esic_amount ,  IF(ot_hours > 0,ot_rate,0) AS 'ot_pr_hr_rate',IF(ot_hours > 0 and ot_rate > 0,esi_on_ot_amount,0) as 'esi_on_ot_amount',  emp_cca ,  branch_cca ,  bill_pf ,  bill_uniform , CASE WHEN  service_group_insurance_billing  = 0 THEN  group_insurance_billing  ELSE 0 END AS 'group_insurance_billing', CASE WHEN  service_group_insurance_billing  = 1 THEN  group_insurance_billing  ELSE 0 END AS 'group_insurance_billing_ser',  bill_service_charge_amount ,  branch_type ,  DUTYHRS ,  basic ,  vda ,  bonus_rate , IF( esic_oa_billing  = 1,  allowances , 0) AS 'allowances_esic', IF( esic_oa_billing  = 0,  allowances , 0) AS 'allowances_no_esic',  baseamount ,  pf_percent ,  esic_percent ,  gratuity_percent ,  hra_percent ,  bill_bonus_percent ,  leave_days  FROM (SELECT (SELECT  client_name  FROM  pay_client_master  WHERE  client_code  =  pay_unit_master . client_code  AND  comp_code  =  pay_unit_master . comp_code ) AS 'client',  pay_company_master . state  AS 'company_state',  pay_unit_master . unit_name ,  pay_unit_master . state_name ,  pay_unit_master . unit_city ,  pay_unit_master . client_branch_code ,  pay_employee_master . emp_name ,  pay_grade_master . grade_desc ,  pay_billing_unit_rate . basic ,  pay_billing_unit_rate . vda ,  pay_billing_unit_rate . bonus_rate , CAST(CONCAT( pay_billing_master_history . hours , 'HRS ',  pay_billing_unit_rate . month_days , ' DAYS ') AS char) AS 'DUTYHRS', ((( pay_billing_master_history . basic  +  pay_billing_master_history . vda ) /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'emp_basic_vda', (( pay_billing_unit_rate . hra  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'hra', CASE WHEN  bonus_taxable  = '1' THEN (( pay_billing_unit_rate . bonus_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'bonus_gross', CASE WHEN  bonus_taxable  = '0' THEN (( pay_billing_unit_rate . bonus_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'bonus_after_gross', CASE WHEN  leave_taxable  = '1' THEN (( pay_billing_unit_rate . leave_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'leave_gross', CASE WHEN  leave_taxable  = '0' THEN (( pay_billing_unit_rate . leave_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'leave_after_gross', CASE WHEN  gratuity_taxable  = '1' THEN (( pay_billing_unit_rate . grauity_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'gratuity_gross', CASE WHEN  gratuity_taxable  = '0' THEN (( pay_billing_unit_rate . grauity_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'gratuity_after_gross', (( pay_billing_unit_rate . washing  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'washing', (( pay_billing_unit_rate . traveling  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'travelling', (( pay_billing_unit_rate . education  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'education', (( pay_billing_unit_rate . national_holiday_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'NH', (( pay_billing_unit_rate . allowances  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'allowances', CASE WHEN  pay_employee_master . cca  = 0 THEN (( pay_billing_unit_rate . cca  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE (( pay_employee_master . cca  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) END AS 'cca_billing', CASE WHEN  pay_employee_master . special_allow  = 0 THEN (( pay_billing_unit_rate . otherallowance  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE (( pay_employee_master . special_allow  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) END AS 'other_allow', CASE WHEN  pay_billing_master_history . ot_policy_billing  = '1' THEN (( pay_billing_master_history . ot_amount_billing  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'hrs_12_ot',  pay_billing_master_history . bill_esic_percent  AS 'esic_percent',  pay_billing_master_history . bill_pf_percent  AS 'pf_percent',  gratuity_percent ,  pay_billing_master_history . hra_percent ,  pay_billing_master_history . bill_bonus_percent ,  pay_billing_master_history . leave_days , (( pay_billing_unit_rate . lwf  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'lwf', (( pay_billing_unit_rate . uniform  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'uniform', (( pay_billing_unit_rate . relieving_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'relieving_charg', (( pay_billing_unit_rate . operational_cost  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'operational_cost',  pay_attendance_muster . tot_days_present , ROUND((( pay_billing_unit_rate . sub_total_c  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ), 2) AS 'baseamount',  bill_service_charge ,  pay_billing_master_history . hours ,  pay_billing_unit_rate . sub_total_c ,  pay_billing_master_history . bill_ser_operations ,  pay_billing_master_history . bill_ser_uniform , pay_billing_unit_rate.ot_1_hr_amount AS 'ot_rate',  pay_attendance_muster . ot_hours ,  pay_billing_unit_rate . esic_amount ,  pay_billing_unit_rate.esi_on_ot_amount as 'esi_on_ot_amount',  pay_employee_master . cca  AS 'emp_cca',  pay_billing_unit_rate . cca  AS 'branch_cca', ( pay_billing_unit_rate . gross  + (( pay_employee_master . cca  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'bill_gross',  pay_billing_unit_rate . pf_amount  AS 'bill_pf',  pay_billing_unit_rate . uniform  AS 'bill_uniform', (( pay_billing_master_history . group_insurance_billing  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'group_insurance_billing',  service_group_insurance_billing ,  pay_employee_master . Employee_type , (( bill_service_charge_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'bill_service_charge_amount',  pay_billing_master_history . esic_common_allow , CASE WHEN  pay_employee_master . special_allow  = 0 THEN (( pay_billing_unit_rate . common_allowance  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE (( pay_employee_master . special_allow  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) END AS 'common_allow', IFNULL( branch_type , 0) AS 'branch_type',  pay_billing_master_history . esic_oa_billing  FROM pay_employee_master INNER JOIN " + pay_attendance_muster + " ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_muster.unit_code = pay_unit_master.unit_code AND pay_attendance_muster.comp_code = pay_unit_master.comp_code INNER JOIN pay_billing_unit_rate ON pay_attendance_muster.unit_code = pay_billing_unit_rate.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate.month AND pay_attendance_muster.year = pay_billing_unit_rate.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.comp_code = pay_employee_master.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_employee_master.grade_code = pay_billing_master_history.designation AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code INNER JOIN pay_grade_master ON pay_billing_master_history.comp_code = pay_grade_master.comp_code AND pay_billing_master_history.designation = pay_grade_master.GRADE_CODE WHERE  " + where;
                sql = "SELECT  distinct(emp_code),pay_billing_unit_rate_history.zone, pay_billing_unit_rate_history.txt_zone,client ,  state_name , branch_type, unit_name ,  pay_billing_unit_rate_history . comp_code ,  emp_name ,  grade_desc , cast(CONCAT( pay_billing_unit_rate_history . hours , ' HRS ',  pay_billing_unit_rate_history . month_days , ' DAYS') as char) AS 'DUTYHRS',  tot_days_present ,  emp_basic_vda ,  bonus_amount_billing ,  pay_billing_unit_rate_history . washing ,  pay_billing_unit_rate_history . travelling ,  pay_billing_unit_rate_history . education , IF( esic_oa_billing  = 1,  pay_billing_unit_rate_history . allowances , 0) AS 'allowances_esic',  cca_billing ,  pay_billing_unit_rate_history . other_allow ,  bonus_gross ,  leave_gross ,  gratuity_gross ,  pay_billing_unit_rate_history . hra , CASE WHEN  pay_billing_master_history . ot_policy_billing  = '1' THEN (( pay_billing_master_history . ot_amount_billing  /  pay_billing_unit_rate_history . month_days ) *  pay_billing_unit_rate_history . tot_days_present ) ELSE 0 END AS 'special_allowance',  pay_billing_unit_rate_history . gross ,  bonus_after_gross ,  leave_after_gross ,  gratuity_after_gross ,  NH ,  pf ,  esic , IF( bill_ser_uniform  = 1, (( pay_billing_unit_rate . uniform  /  pay_billing_unit_rate . month_days ) *  pay_billing_unit_rate_history . tot_days_present ), 0) AS 'uniform_ser',  pay_billing_unit_rate_history . group_insurance_billing ,pay_billing_unit_rate_history.medical_insurance_amount,  pay_billing_unit_rate_history . lwf , IF( bill_ser_operations  = 1, (( pay_billing_unit_rate . operational_cost  /  pay_billing_unit_rate . month_days ) *  pay_billing_unit_rate_history . tot_days_present ), 0) AS 'operational_cost',  pay_billing_unit_rate_history . allowances_no_esic  AS allowances_no_esic, amount AS 'sub_total_a', IF((ot_rate - esi_on_ot_amount) > 0, (ot_rate - esi_on_ot_amount), 0) AS 'ot_rate' ,  pay_billing_unit_rate_history . esic_ot ,  ot_hours , ( ot_rate ) AS 'sub_total_b', amount AS 'sub_total_ab',  relieving_charg , amount as sub_total_c , IF( bill_ser_uniform  = 0, (( pay_billing_unit_rate . uniform  /  pay_billing_unit_rate . month_days ) *  pay_billing_unit_rate_history . tot_days_present ), 0) AS 'uniform_no_ser', IF( bill_ser_operations  = 0, (( pay_billing_unit_rate . operational_cost  /  pay_billing_unit_rate . month_days ) *  pay_billing_unit_rate_history . tot_days_present ), 0) AS 'operational_cost_no_ser',  Service_charge ,(amount + IF(bill_ser_uniform = 0, ((pay_billing_unit_rate.uniform / pay_billing_unit_rate.month_days) * pay_billing_unit_rate_history.tot_days_present), 0)+ IF(bill_ser_operations = 0, ((pay_billing_unit_rate.operational_cost / pay_billing_unit_rate.month_days) * pay_billing_unit_rate_history.tot_days_present), 0)+Service_charge+(ot_rate * ot_hours)) as Amount,pay_billing_master_history.bill_bonus_percent ,pay_billing_master_history.leave_days,pay_billing_master_history.gratuity_percent , pay_billing_master_history.hra_percent, pay_billing_master_history.bill_pf_percent, pay_billing_master_history.bill_esic_percent,pay_billing_master_history.bill_service_charge,pay_billing_master_history.basic, pay_billing_master_history.vda ,pay_billing_unit_rate.bonus_rate,   ((pay_billing_master_history.group_insurance_billing/ pay_billing_unit_rate.month_days) * pay_billing_unit_rate_history.tot_days_present) AS 'group_insurance_billing_ser',IF(ot_hours > 0, ( ot_rate - esi_on_ot_amount), 0) AS 'ot_pr_hr_rate' , IF(ot_hours > 0 AND ot_rate > 0, esi_on_ot_amount, 0) AS 'esi_on_ot_amount',(ot_rate * ot_hours) AS 'ot_amount', pay_billing_unit_rate_history.conveyance_amount FROM  pay_billing_unit_rate_history  INNER JOIN  " + pay_billing_unit_rate + "   ON  pay_billing_unit_rate_history . comp_code  =  pay_billing_unit_rate . comp_code  AND  pay_billing_unit_rate_history . unit_code  =  pay_billing_unit_rate . unit_code  AND  pay_billing_unit_rate_history . month  =  pay_billing_unit_rate . month  AND  pay_billing_unit_rate_history . year  =  pay_billing_unit_rate . year  AND  pay_billing_unit_rate_history . grade_code  =  pay_billing_unit_rate.designation  INNER JOIN  " + pay_billing_master_history + "  ON  pay_billing_master_history . comp_code  =  pay_billing_unit_rate_history . comp_code   AND  pay_billing_master_history . billing_client_code  =  pay_billing_unit_rate_history . client_code  AND  pay_billing_master_history . billing_unit_code  =  pay_billing_unit_rate_history . unit_code  AND  pay_billing_master_history . month  =  pay_billing_unit_rate_history . month  AND  pay_billing_master_history . year  =  pay_billing_unit_rate_history . year  AND  pay_billing_master_history . designation  =  pay_billing_unit_rate_history . grade_code  AND  pay_billing_master_history . hours  =  pay_billing_unit_rate_history . hours  AND  pay_billing_master_history . type  = 'billing'   " + where + multi + "  group by emp_code " + order_by_clause;
            }
            //finance copy
            else if (i == 2)
            {
                string rg_terms = "";

                //Akshay 23-04-2019
                if (ddl_client.SelectedValue == "RCPL")
                {
                    rg_terms = "AND (emp_code != '' OR emp_code IS NOT NULL)";
                }
                string start_end_date = "AND (start_date = 0 AND end_date = 0) " + billing_type;
                if (ddl_start_date_common != "0" && ddl_end_date_common != "0")
                {
                    start_end_date = "AND (start_date = " + ddl_start_date_common + " AND end_date = " + ddl_end_date_common + ") " + billing_type;
                }

                where = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + ddl_unitcode + "' " + billing_bfl + "  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' and hdfc_type is null " + grade + "  and flag != 0  " + where_state + rg_terms + " " + start_end_date;
                if (ddl_billing_state == "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "'  " + billing_bfl + "  and year = '" + txt_month_year.Text.Substring(3) + "' and hdfc_type is null " + grade + " and flag != 0 " + where_state + rg_terms + " " + start_end_date;
                }
                else if (ddl_unitcode == "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state + "'  " + billing_bfl + "   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' and hdfc_type is null " + grade + " and flag != 0  " + where_state + rg_terms + " " + start_end_date;
                }
                if (ddl_client.SelectedValue == "HDFC")
                {
                    where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.unit_code='" + ddl_unitcode + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' and hdfc_type='manpower_bill' " + grade + " and pay_billing_unit_rate_history.flag != 0 " + start_end_date + "  group by pay_billing_unit_rate_history.unit_code,grade_desc  order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
                    if (ddl_billing_state == "ALL")
                    {
                        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' and hdfc_type='manpower_bill' " + grade + " and pay_billing_unit_rate_history.flag != 0 " + start_end_date + " group by pay_billing_unit_rate_history.unit_code,grade_desc order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
                    }
                    else if (ddl_unitcode == "ALL")
                    {
                        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.state_name = '" + ddl_billing_state + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' and hdfc_type='manpower_bill' " + grade + "  and pay_billing_unit_rate_history.flag != 0 " + start_end_date + " group by pay_billing_unit_rate_history.unit_code,grade_desc order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
                    }
                    //sql = "SELECT client, state_name, unit_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, hra, bonus_gross, leave_gross, gratuity_gross, washing, travelling, education, allowances, cca_billing, other_allow, gross, bonus_after_gross, leave_after_gross, gratuity_after_gross, pf, esic, hrs_12_ot, esic_ot, lwf, uniform, relieving_charg, operational_cost, tot_days_present, (Total + pf + esic + group_insurance_billing_ser) AS 'Amount', IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, ((((Total + pf + esic + group_insurance_billing_ser)  + (ot_rate * ot_hours)) * bill_service_charge) / 100)) AS 'Service_charge', CASE WHEN LOCATE(company_state, state_name) THEN ROUND(((((Total + pf + esic + operational_cost + uniform + group_insurance_billing_ser + group_insurance_billing)  + (ot_rate * ot_hours) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, ((((Total + pf + esic + group_insurance_billing_ser)  + (ot_rate * ot_hours)) * bill_service_charge) / 100))) * 9) / 100), 2) ELSE 0 END AS 'CGST9', CASE WHEN LOCATE(company_state, state_name) THEN 0 ELSE ROUND(((((Total + pf + esic + operational_cost + uniform + group_insurance_billing_ser + group_insurance_billing)  + (ot_rate * ot_hours) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, ((((Total + pf + esic + group_insurance_billing_ser) + (ot_rate * ot_hours)) * bill_service_charge) / 100))) * 18) / 100), 2) END AS 'IGST18', CASE WHEN LOCATE(company_state, state_name) THEN ROUND(((((Total + pf + esic + operational_cost + uniform + group_insurance_billing_ser + group_insurance_billing) + (ot_rate * ot_hours) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, ((((Total + pf + esic + group_insurance_billing_ser)  + (ot_rate * ot_hours)) * bill_service_charge) / 100))) * 9) / 100), 2) ELSE 0 END AS 'SGST9', bill_service_charge, NH, hours, " + daterange + ", CASE WHEN emp_cca = 0 THEN (sub_total_c - ot_rate) ELSE (bill_gross + ((bill_gross * esic_percent) / 100) + bill_pf + bill_uniform + group_insurance_billing_ser + bonus_after_gross + leave_after_gross + gratuity_after_gross) END AS 'sub_total_c', IF(ot_hours > 0, ot_rate, 0) AS 'ot_rate', ot_hours, (ot_rate * ot_hours) AS 'ot_amount', group_insurance_billing, bill_service_charge_amount,txt_zone,adminhead_name,ihms,location_type,unit_add1,emp_count,emp_count1,state_per,branch_cost_centre_code,total_emp_count,(tot_days_present) as 'no_of_duties',zone,TOT_WORKING_DAYS,GRADE_CODE,month_days FROM (SELECT client, company_state, unit_name, state_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, hra, bonus_gross, leave_gross, gratuity_gross, washing, travelling, education, allowances, cca_billing, other_allow, (emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross + hrs_12_ot) AS 'gross', bonus_after_gross, leave_after_gross, gratuity_after_gross, (((emp_basic_vda) / 100) * pf_percent) AS 'pf', (((emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + IF(esic_oa_billing =1, allowances,0) + cca_billing + other_allow + gratuity_gross + hrs_12_ot) / 100) * esic_percent) AS 'esic', hrs_12_ot, (((hrs_12_ot) * esic_percent) / 100) AS 'esic_ot', lwf, CASE WHEN bill_ser_uniform = 1 THEN 0 ELSE uniform END AS 'uniform', relieving_charg, CASE WHEN bill_ser_operations = 1 THEN 0 ELSE operational_cost END AS 'operational_cost', tot_days_present, (emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross + bonus_after_gross + leave_after_gross + gratuity_after_gross + lwf + CASE WHEN bill_ser_uniform = 0 THEN 0 ELSE uniform END + relieving_charg + CASE WHEN bill_ser_operations = 0 THEN 0 ELSE operational_cost END + NH + hrs_12_ot) AS 'Total', bill_service_charge, NH, hours, (bill_gross + emp_cca) AS 'bill_gross', sub_total_c, bill_ser_uniform, bill_ser_operations, (ot_rate + esi_on_ot_amount) AS 'ot_rate', ot_hours, esic_amount, esi_on_ot_amount, emp_cca, bill_pf, bill_uniform, esic_percent,  CASE WHEN service_group_insurance_billing = 0 THEN group_insurance_billing ELSE 0 END AS 'group_insurance_billing',  CASE WHEN service_group_insurance_billing = 1 THEN group_insurance_billing ELSE 0 END AS 'group_insurance_billing_ser', bill_service_charge_amount,txt_zone,adminhead_name,ihms,location_type,unit_add1,emp_count,emp_count1,state_per,branch_cost_centre_code,total_emp_count,zone,TOT_WORKING_DAYS,GRADE_CODE,month_days FROM (SELECT (SELECT client_name FROM pay_client_master WHERE client_code = pay_unit_master.client_code AND comp_code = '" + Session["COMP_CODE"].ToString() + "') AS 'client', pay_company_master.state AS 'company_state', pay_unit_master.unit_name, pay_unit_master.state_name, pay_unit_master.unit_city, pay_unit_master.client_branch_code, pay_employee_master.emp_name, pay_grade_master.grade_desc, SUM(((( pay_billing_master_history . basic  +  pay_billing_master_history . vda ) /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'emp_basic_vda', SUM((( pay_billing_unit_rate . hra  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'hra', CASE WHEN  bonus_taxable  = '1' THEN SUM((( pay_billing_unit_rate . bonus_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'bonus_gross', CASE WHEN  bonus_taxable  = '0' THEN SUM((( pay_billing_unit_rate . bonus_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'bonus_after_gross', CASE WHEN  leave_taxable  = '1' THEN SUM((( pay_billing_unit_rate . leave_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'leave_gross', CASE WHEN  leave_taxable  = '0' THEN SUM((( pay_billing_unit_rate . leave_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'leave_after_gross', CASE WHEN  gratuity_taxable  = '1' THEN SUM((( pay_billing_unit_rate . grauity_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'gratuity_gross', CASE WHEN  gratuity_taxable  = '0' THEN SUM((( pay_billing_unit_rate . grauity_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'gratuity_after_gross', SUM((( pay_billing_unit_rate . washing  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'washing', SUM((( pay_billing_unit_rate . traveling  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'travelling', SUM((( pay_billing_unit_rate . education  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'education', SUM((( pay_billing_unit_rate . national_holiday_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'NH', SUM((( pay_billing_unit_rate . allowances  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'allowances', SUM(CASE WHEN  pay_employee_master . cca  = 0 THEN ((( pay_billing_unit_rate . cca  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE ((( pay_employee_master . cca  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) END) AS 'cca_billing', SUM(CASE WHEN  pay_employee_master . special_allow  = 0 THEN ((( pay_billing_master_history . other_allow  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE ( pay_employee_master . special_allow ) END) AS 'other_allow', CASE WHEN  pay_billing_master_history . ot_policy_billing  = '1' THEN SUM((( pay_billing_master_history . ot_amount_billing  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'hrs_12_ot',  pay_billing_master_history . bill_esic_percent  AS 'esic_percent',  pay_billing_master_history . bill_pf_percent  AS 'pf_percent', SUM((((pay_billing_unit_rate.lwf) / (pay_billing_unit_rate.month_days)) * (pay_attendance_muster.tot_days_present))) AS 'lwf', SUM((( pay_billing_unit_rate . uniform  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'uniform', SUM((( pay_billing_unit_rate . relieving_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'relieving_charg', SUM((( pay_billing_unit_rate . operational_cost  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'operational_cost', SUM(pay_attendance_muster.tot_days_present) as tot_days_present, ROUND(((pay_billing_unit_rate.sub_total_c / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present), 2) AS 'baseamount', bill_service_charge, pay_billing_master_history.hours, pay_billing_unit_rate.sub_total_c, pay_billing_master_history.bill_ser_operations, pay_billing_master_history.bill_ser_uniform, pay_billing_unit_rate.ot_1_hr_amount AS 'ot_rate',  SUM(pay_attendance_muster.ot_hours) as 'ot_hours', pay_billing_unit_rate.esic_amount, pay_billing_unit_rate.esi_on_ot_amount, pay_employee_master.cca AS 'emp_cca', pay_billing_unit_rate.gross AS 'bill_gross', pay_billing_unit_rate.pf_amount AS 'bill_pf', pay_billing_unit_rate.uniform AS 'bill_uniform', sum(IF(pay_employee_master.Employee_type = 'Permanent' OR pay_employee_master.Employee_type = 'Reliever' OR pay_employee_master.Employee_type ='PermanentReliever',((pay_billing_master_history.group_insurance_billing / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present),0)) AS 'group_insurance_billing', service_group_insurance_billing, pay_employee_master.Employee_type, bill_service_charge_amount,pay_unit_master.txt_zone, branch_cost_centre_code, adminhead_name, 'IH&MS'as ihms	, pay_unit_master.location_type, pay_unit_master.	unit_add1, case emp_count when '1' then 'A' when '2' then 'B' else 'C' end as emp_count , case emp_count when '1' then 'Single - 8 Hrs. Shift (1 SG)' when '2' then 'Double - 16 Hrs. Shift (2 SG)' ELSE concat('Triple - 24 Hrs. Shift (', emp_count ,'SG)') end as emp_count1 , 'STATE ' as 'state_per',emp_count as 'total_emp_count',zone,TOT_WORKING_DAYS,pay_grade_master.GRADE_CODE,pay_billing_master_history.esic_oa_billing,pay_billing_unit_rate.month_days FROM pay_employee_master INNER JOIN pay_attendance_muster ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_muster.unit_code = pay_unit_master.unit_code AND pay_attendance_muster.comp_code = pay_unit_master.comp_code INNER JOIN pay_billing_unit_rate ON pay_attendance_muster.unit_code = pay_billing_unit_rate.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate.month AND pay_attendance_muster.year = pay_billing_unit_rate.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.comp_code = pay_employee_master.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_employee_master.grade_code = pay_billing_master_history.designation AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code INNER JOIN pay_grade_master ON pay_billing_master_history.comp_code = pay_grade_master.comp_code AND pay_billing_master_history.designation = pay_grade_master.GRADE_CODE WHERE " + where;
                    // sql = "SELECT CASE WHEN invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', client_code, client, state_name,branch_type, unit_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, SUM(hra) as 'hra', SUM(bonus_gross) as 'bonus_gross', SUM(leave_gross) as 'leave_gross', SUM(gratuity_gross) as 'gratuity_gross', SUM(washing) as 'washing', SUM(travelling) as 'travelling', SUM(education) as 'education', SUM(allowances) as 'allowances', SUM(cca_billing) as 'cca_billing', SUM(other_allow) as 'other_allow', SUM(gross) as 'gross', SUM(bonus_after_gross) as 'bonus_after_gross', SUM(leave_after_gross) as 'leave_after_gross', SUM(gratuity_after_gross) as 'gratuity_after_gross', SUM(pf) as 'pf', SUM(esic) as 'esic', SUM(hrs_12_ot) as 'hrs_12_ot' , SUM(esic_ot) as 'esic_ot', SUM(lwf) as 'lwf', SUM(uniform) as 'uniform', SUM(relieving_charg) as 'relieving_charg', SUM(operational_cost) as 'operational_cost', SUM(tot_days_present) as 'tot_days_present',sum(Amount) as 'Amount', SUM(Service_charge) as 'Service_charge', SUM(CGST9) as 'CGST9', SUM(IGST18) as 'IGST18', SUM(SGST9) as 'SGST9', bill_service_charge , NH, hours, fromtodate,sub_total_c, max(ot_rate) as 'ot_rate', SUM(ot_hours) as 'ot_hours', SUM(ot_amount) as 'ot_amount', group_insurance_billing, bill_service_charge_amount, txt_zone, adminhead_name, ihms, location_type, unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) as 'total_emp_count', sum(no_of_duties) as 'no_of_duties', zone, TOT_WORKING_DAYS, GRADE_CODE, month_days FROM pay_billing_unit_rate_history " + where;
                    sql = "SELECT CASE WHEN invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', pay_billing_unit_rate_history.client_code, client, pay_billing_unit_rate_history.state_name, pay_billing_unit_rate_history.branch_type, pay_billing_unit_rate_history.unit_name, pay_billing_unit_rate_history.unit_city, pay_billing_unit_rate_history.client_branch_code, emp_name, grade_desc, emp_basic_vda, SUM(hra) AS 'hra', SUM(bonus_gross) AS 'bonus_gross', SUM(leave_gross) AS 'leave_gross', SUM(gratuity_gross) AS 'gratuity_gross', SUM(washing) AS 'washing', SUM(travelling) AS 'travelling', SUM(education) AS 'education', SUM(allowances) AS 'allowances', SUM(cca_billing) AS 'cca_billing', SUM(other_allow) AS 'other_allow', SUM(gross) AS 'gross', SUM(bonus_after_gross) AS 'bonus_after_gross', SUM(leave_after_gross) AS 'leave_after_gross', SUM(gratuity_after_gross) AS 'gratuity_after_gross', SUM(pf) AS 'pf', SUM(esic) AS 'esic', SUM(hrs_12_ot) AS 'hrs_12_ot', SUM(esic_ot) AS 'esic_ot', SUM(lwf) AS 'lwf', SUM(uniform) AS 'uniform', SUM(relieving_charg) AS 'relieving_charg', SUM(operational_cost) AS 'operational_cost', SUM(tot_days_present) AS 'tot_days_present', SUM(Amount) AS 'Amount', SUM(Service_charge) AS 'Service_charge', SUM(CGST9) AS 'CGST9', SUM(IGST18) AS 'IGST18', SUM(SGST9) AS 'SGST9', bill_service_charge, NH, hours, fromtodate, (amount * month_days/tot_days_present) as 'sub_total_c', MAX(ot_rate) AS 'ot_rate', SUM(ot_hours) AS 'ot_hours', SUM(ot_amount) AS 'ot_amount', group_insurance_billing, bill_service_charge_amount, pay_billing_unit_rate_history.txt_zone, pay_billing_unit_rate_history.adminhead_name, ihms, pay_billing_unit_rate_history.location_type, pay_billing_unit_rate_history.unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, pay_billing_unit_rate_history.branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) AS 'total_emp_count', SUM(no_of_duties) AS 'no_of_duties', pay_billing_unit_rate_history.zone, TOT_WORKING_DAYS, GRADE_CODE, month_days, material_area,(SELECT  field2 FROM pay_zone_master WHERE pay_zone_master.comp_code = pay_billing_unit_rate_history.comp_code AND pay_zone_master.CLIENT_CODE = pay_billing_unit_rate_history.CLIENT_CODE AND pay_zone_master.ZONE = pay_unit_master.txt_zone AND type = 'ZONE' AND field1 = 'admin') AS 'zonal_name' FROM pay_billing_unit_rate_history INNER JOIN pay_unit_master ON pay_billing_unit_rate_history.comp_code = pay_unit_master.comp_code AND pay_billing_unit_rate_history.unit_code = pay_unit_master.unit_code " + where;
                }
                //Changes 02-10-2019 BAGICTM FC
                else if (ddl_client.SelectedValue == "BAGICTM")
                {
                    where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.unit_code='" + ddl_unitcode + "' and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "' and hdfc_type is null  " + grade + "  and flag != 0  " + rg_terms + " " + start_end_date;
                    if (ddl_billing_state == "ALL")
                    {
                        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "' " + grade + " and hdfc_type is null  and flag != 0 " + rg_terms + " " + start_end_date;
                    }
                    else if (ddl_unitcode == "ALL")
                    {
                        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.state_name = '" + ddl_billing_state + "'  and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "' " + grade + " and hdfc_type is null  and flag != 0  " + rg_terms + " " + start_end_date;
                    }
                    //sql = "SELECT client, state_name, unit_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, hra, bonus_gross, leave_gross, gratuity_gross, washing, travelling, education, allowances, cca_billing, other_allow, gross, bonus_after_gross, leave_after_gross, gratuity_after_gross, pf, esic, hrs_12_ot, esic_ot, lwf, uniform, relieving_charg, operational_cost, tot_days_present, (Total + pf + esic + group_insurance_billing_ser) AS 'Amount', IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, ((((Total + pf + esic + group_insurance_billing_ser)  + (ot_rate * ot_hours)) * bill_service_charge) / 100)) AS 'Service_charge', CASE WHEN LOCATE(company_state, state_name) THEN ROUND(((((Total + pf + esic + operational_cost + uniform + group_insurance_billing_ser + group_insurance_billing)  + (ot_rate * ot_hours) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, ((((Total + pf + esic + group_insurance_billing_ser)  + (ot_rate * ot_hours)) * bill_service_charge) / 100))) * 9) / 100), 2) ELSE 0 END AS 'CGST9', CASE WHEN LOCATE(company_state, state_name) THEN 0 ELSE ROUND(((((Total + pf + esic + operational_cost + uniform + group_insurance_billing_ser + group_insurance_billing)  + (ot_rate * ot_hours) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, ((((Total + pf + esic + group_insurance_billing_ser) + (ot_rate * ot_hours)) * bill_service_charge) / 100))) * 18) / 100), 2) END AS 'IGST18', CASE WHEN LOCATE(company_state, state_name) THEN ROUND(((((Total + pf + esic + operational_cost + uniform + group_insurance_billing_ser + group_insurance_billing) + (ot_rate * ot_hours) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, ((((Total + pf + esic + group_insurance_billing_ser)  + (ot_rate * ot_hours)) * bill_service_charge) / 100))) * 9) / 100), 2) ELSE 0 END AS 'SGST9', bill_service_charge, NH, hours, " + daterange + ", CASE WHEN emp_cca = 0 THEN (sub_total_c - ot_rate) ELSE (bill_gross + ((bill_gross * esic_percent) / 100) + bill_pf + bill_uniform + group_insurance_billing_ser + bonus_after_gross + leave_after_gross + gratuity_after_gross) END AS 'sub_total_c', IF(ot_hours > 0, ot_rate, 0) AS 'ot_rate', ot_hours, (ot_rate * ot_hours) AS 'ot_amount', group_insurance_billing, bill_service_charge_amount,txt_zone,adminhead_name,ihms,location_type,unit_add1,emp_count,emp_count1,state_per,branch_cost_centre_code,total_emp_count,(tot_days_present) as 'no_of_duties',zone,TOT_WORKING_DAYS,GRADE_CODE,month_days FROM (SELECT client, company_state, unit_name, state_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, hra, bonus_gross, leave_gross, gratuity_gross, washing, travelling, education, allowances, cca_billing, other_allow, (emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross + hrs_12_ot) AS 'gross', bonus_after_gross, leave_after_gross, gratuity_after_gross, (((emp_basic_vda) / 100) * pf_percent) AS 'pf', (((emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + IF(esic_oa_billing =1, allowances,0) + cca_billing + other_allow + gratuity_gross + hrs_12_ot) / 100) * esic_percent) AS 'esic', hrs_12_ot, (((hrs_12_ot) * esic_percent) / 100) AS 'esic_ot', lwf, CASE WHEN bill_ser_uniform = 1 THEN 0 ELSE uniform END AS 'uniform', relieving_charg, CASE WHEN bill_ser_operations = 1 THEN 0 ELSE operational_cost END AS 'operational_cost', tot_days_present, (emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross + bonus_after_gross + leave_after_gross + gratuity_after_gross + lwf + CASE WHEN bill_ser_uniform = 0 THEN 0 ELSE uniform END + relieving_charg + CASE WHEN bill_ser_operations = 0 THEN 0 ELSE operational_cost END + NH + hrs_12_ot) AS 'Total', bill_service_charge, NH, hours, (bill_gross + emp_cca) AS 'bill_gross', sub_total_c, bill_ser_uniform, bill_ser_operations, (ot_rate + esi_on_ot_amount) AS 'ot_rate', ot_hours, esic_amount, esi_on_ot_amount, emp_cca, bill_pf, bill_uniform, esic_percent,  CASE WHEN service_group_insurance_billing = 0 THEN group_insurance_billing ELSE 0 END AS 'group_insurance_billing',  CASE WHEN service_group_insurance_billing = 1 THEN group_insurance_billing ELSE 0 END AS 'group_insurance_billing_ser', bill_service_charge_amount,txt_zone,adminhead_name,ihms,location_type,unit_add1,emp_count,emp_count1,state_per,branch_cost_centre_code,total_emp_count,zone,TOT_WORKING_DAYS,GRADE_CODE,month_days FROM (SELECT (SELECT client_name FROM pay_client_master WHERE client_code = pay_unit_master.client_code AND comp_code = '" + Session["COMP_CODE"].ToString() + "') AS 'client', pay_company_master.state AS 'company_state', pay_unit_master.unit_name, pay_unit_master.state_name, pay_unit_master.unit_city, pay_unit_master.client_branch_code, pay_employee_master.emp_name, pay_grade_master.grade_desc, SUM(((( pay_billing_master_history . basic  +  pay_billing_master_history . vda ) /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'emp_basic_vda', SUM((( pay_billing_unit_rate . hra  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'hra', CASE WHEN  bonus_taxable  = '1' THEN SUM((( pay_billing_unit_rate . bonus_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'bonus_gross', CASE WHEN  bonus_taxable  = '0' THEN SUM((( pay_billing_unit_rate . bonus_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'bonus_after_gross', CASE WHEN  leave_taxable  = '1' THEN SUM((( pay_billing_unit_rate . leave_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'leave_gross', CASE WHEN  leave_taxable  = '0' THEN SUM((( pay_billing_unit_rate . leave_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'leave_after_gross', CASE WHEN  gratuity_taxable  = '1' THEN SUM((( pay_billing_unit_rate . grauity_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'gratuity_gross', CASE WHEN  gratuity_taxable  = '0' THEN SUM((( pay_billing_unit_rate . grauity_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'gratuity_after_gross', SUM((( pay_billing_unit_rate . washing  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'washing', SUM((( pay_billing_unit_rate . traveling  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'travelling', SUM((( pay_billing_unit_rate . education  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'education', SUM((( pay_billing_unit_rate . national_holiday_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'NH', SUM((( pay_billing_unit_rate . allowances  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'allowances', SUM(CASE WHEN  pay_employee_master . cca  = 0 THEN ((( pay_billing_unit_rate . cca  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE ((( pay_employee_master . cca  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) END) AS 'cca_billing', SUM(CASE WHEN  pay_employee_master . special_allow  = 0 THEN ((( pay_billing_master_history . other_allow  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE ( pay_employee_master . special_allow ) END) AS 'other_allow', CASE WHEN  pay_billing_master_history . ot_policy_billing  = '1' THEN SUM((( pay_billing_master_history . ot_amount_billing  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) ELSE 0 END AS 'hrs_12_ot',  pay_billing_master_history . bill_esic_percent  AS 'esic_percent',  pay_billing_master_history . bill_pf_percent  AS 'pf_percent', SUM((((pay_billing_unit_rate.lwf) / (pay_billing_unit_rate.month_days)) * (pay_attendance_muster.tot_days_present))) AS 'lwf', SUM((( pay_billing_unit_rate . uniform  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'uniform', SUM((( pay_billing_unit_rate . relieving_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'relieving_charg', SUM((( pay_billing_unit_rate . operational_cost  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'operational_cost', SUM(pay_attendance_muster.tot_days_present) as tot_days_present, ROUND(((pay_billing_unit_rate.sub_total_c / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present), 2) AS 'baseamount', bill_service_charge, pay_billing_master_history.hours, pay_billing_unit_rate.sub_total_c, pay_billing_master_history.bill_ser_operations, pay_billing_master_history.bill_ser_uniform, pay_billing_unit_rate.ot_1_hr_amount AS 'ot_rate',  SUM(pay_attendance_muster.ot_hours) as 'ot_hours', pay_billing_unit_rate.esic_amount, pay_billing_unit_rate.esi_on_ot_amount, pay_employee_master.cca AS 'emp_cca', pay_billing_unit_rate.gross AS 'bill_gross', pay_billing_unit_rate.pf_amount AS 'bill_pf', pay_billing_unit_rate.uniform AS 'bill_uniform', sum(IF(pay_employee_master.Employee_type = 'Permanent' OR pay_employee_master.Employee_type = 'Reliever' OR pay_employee_master.Employee_type ='PermanentReliever',((pay_billing_master_history.group_insurance_billing / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present),0)) AS 'group_insurance_billing', service_group_insurance_billing, pay_employee_master.Employee_type, bill_service_charge_amount,pay_unit_master.txt_zone, branch_cost_centre_code, adminhead_name, 'IH&MS'as ihms	, pay_unit_master.location_type, pay_unit_master.	unit_add1, case emp_count when '1' then 'A' when '2' then 'B' else 'C' end as emp_count , case emp_count when '1' then 'Single - 8 Hrs. Shift (1 SG)' when '2' then 'Double - 16 Hrs. Shift (2 SG)' ELSE concat('Triple - 24 Hrs. Shift (', emp_count ,'SG)') end as emp_count1 , 'STATE ' as 'state_per',emp_count as 'total_emp_count',zone,TOT_WORKING_DAYS,pay_grade_master.GRADE_CODE,pay_billing_master_history.esic_oa_billing,pay_billing_unit_rate.month_days FROM pay_employee_master INNER JOIN pay_attendance_muster ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_muster.unit_code = pay_unit_master.unit_code AND pay_attendance_muster.comp_code = pay_unit_master.comp_code INNER JOIN pay_billing_unit_rate ON pay_attendance_muster.unit_code = pay_billing_unit_rate.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate.month AND pay_attendance_muster.year = pay_billing_unit_rate.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.comp_code = pay_employee_master.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_employee_master.grade_code = pay_billing_master_history.designation AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code INNER JOIN pay_grade_master ON pay_billing_master_history.comp_code = pay_grade_master.comp_code AND pay_billing_master_history.designation = pay_grade_master.GRADE_CODE WHERE " + where;

                    sql = "SELECT txt_zone,zone,CASE WHEN invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_unit_rate_history.client_code, CASE WHEN pay_billing_unit_rate_history.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE client END AS 'client', state_name, unit_name, unit_city, client_branch_code, pay_billing_unit_rate_history.emp_name, grade_desc, emp_basic_vda, hra, bonus_gross, leave_gross, gratuity_gross, washing, travelling, education, allowances, cca_billing, other_allow, gross, bonus_after_gross, leave_after_gross, gratuity_after_gross, pf, esic, hrs_12_ot, esic_ot, lwf, uniform, relieving_charg, operational_cost, tot_days_present, amount AS 'Amount', Service_charge,CGST9, SGST9, IGST18, bill_service_charge, NH, hours, fromtodate, (amount * month_days / tot_days_present) AS 'sub_total_c', ot_rate, ot_hours, ot_amount, group_insurance_billing, bill_service_charge_amount, bill_service_charge_amount, branch_type, month_days, gst_applicable, OPus_NO, pay_billing_unit_rate_history.unit_code, conveyance_amount AS 'conveyance_rate'  FROM  pay_billing_unit_rate_history " + where + " order by 7,8,11";
                }
                else
                {
                    //sql = "SELECT client, state_name, unit_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, hra, bonus_gross, leave_gross, gratuity_gross, washing, travelling, education, allowances, cca_billing, other_allow, gross, bonus_after_gross, leave_after_gross, gratuity_after_gross, pf, esic, hrs_12_ot, esic_ot, lwf, uniform, relieving_charg, operational_cost, tot_days_present, (Total + pf + esic + group_insurance_billing_ser) AS 'Amount', IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, (((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100)) AS 'Service_charge', CASE WHEN LOCATE(company_state, state_name) THEN ROUND(((((Total + pf + esic + operational_cost + uniform + (ot_rate * ot_hours) + group_insurance_billing_ser + group_insurance_billing) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, (((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100))) * 9) / 100), 2) ELSE 0 END AS 'CGST9', CASE WHEN LOCATE(company_state, state_name) THEN 0 ELSE ROUND(((((Total + pf + esic + operational_cost + uniform + (ot_rate * ot_hours) + group_insurance_billing_ser + group_insurance_billing) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, (((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100))) * 18) / 100), 2) END AS 'IGST18', CASE WHEN LOCATE(company_state, state_name) THEN ROUND(((((Total + pf + esic + operational_cost + uniform + (ot_rate * ot_hours) + group_insurance_billing_ser + group_insurance_billing) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, (((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100))) * 9) / 100), 2) ELSE 0 END AS 'SGST9', bill_service_charge, NH, hours," + daterange + ", CASE WHEN emp_cca = 0 THEN (sub_total_c - ot_rate) ELSE (bill_gross + ((bill_gross * esic_percent) / 100) + bill_pf + bill_uniform + group_insurance_billing_ser + bonus_after_gross + leave_after_gross + gratuity_after_gross) END AS 'sub_total_c', IF(ot_hours > 0, ot_rate, 0) AS 'ot_rate', ot_hours, (ot_rate * ot_hours) AS 'ot_amount', group_insurance_billing, bill_service_charge_amount,branch_type,month_days,gst_applicable,OPus_NO FROM (SELECT client, company_state, unit_name, state_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, hra, bonus_gross, leave_gross, gratuity_gross, washing, travelling, education, allowances, cca_billing, other_allow, (emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross + hrs_12_ot) AS 'gross', bonus_after_gross, leave_after_gross, gratuity_after_gross, (((emp_basic_vda) / 100) * pf_percent) AS 'pf', (((emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + IF(esic_oa_billing =1, allowances,0) + cca_billing + other_allow + gratuity_gross + hrs_12_ot) / 100) * esic_percent) AS 'esic', hrs_12_ot, (((hrs_12_ot) * esic_percent) / 100) AS 'esic_ot', lwf, CASE WHEN bill_ser_uniform = 1 THEN 0 ELSE uniform END AS 'uniform', relieving_charg, CASE WHEN bill_ser_operations = 1 THEN 0 ELSE operational_cost END AS 'operational_cost', tot_days_present, (emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross + bonus_after_gross + leave_after_gross + gratuity_after_gross + lwf + CASE WHEN bill_ser_uniform = 0 THEN 0 ELSE uniform END + relieving_charg + CASE WHEN bill_ser_operations = 0 THEN 0 ELSE operational_cost END + NH + hrs_12_ot+IF(esic_common_allow = 0, common_allow, 0)) AS 'Total', bill_service_charge, NH, hours, (bill_gross + emp_cca) AS 'bill_gross', sub_total_c, bill_ser_uniform, bill_ser_operations, (ot_rate + esi_on_ot_amount) AS 'ot_rate', ot_hours, esic_amount, esi_on_ot_amount, emp_cca, bill_pf, bill_uniform, esic_percent, IF(Employee_type = 'Permanent', CASE WHEN service_group_insurance_billing = 0 THEN group_insurance_billing ELSE 0 END, 0) AS 'group_insurance_billing', IF(Employee_type = 'Permanent', CASE WHEN service_group_insurance_billing = 1 THEN group_insurance_billing ELSE 0 END, 0) AS 'group_insurance_billing_ser', bill_service_charge_amount,branch_type,month_days,gst_applicable,OPus_NO FROM (SELECT client_name  AS 'client', pay_company_master.state AS 'company_state', pay_unit_master.unit_name, pay_unit_master.state_name, pay_unit_master.unit_city, pay_unit_master.client_branch_code, pay_employee_master.emp_name, pay_grade_master.grade_desc, (((pay_billing_master_history.basic + pay_billing_master_history.vda) / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'emp_basic_vda', ((pay_billing_unit_rate.hra / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'hra', CASE WHEN bonus_taxable = '1' THEN ((pay_billing_unit_rate.bonus_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'bonus_gross', CASE WHEN bonus_taxable = '0' THEN ((pay_billing_unit_rate.bonus_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'bonus_after_gross', CASE WHEN leave_taxable = '1' THEN ((pay_billing_unit_rate.leave_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'leave_gross', CASE WHEN leave_taxable = '0' THEN ((pay_billing_unit_rate.leave_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'leave_after_gross', CASE WHEN gratuity_taxable = '1' THEN ((pay_billing_unit_rate.grauity_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'gratuity_gross', CASE WHEN gratuity_taxable = '0' THEN ((pay_billing_unit_rate.grauity_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'gratuity_after_gross', ((pay_billing_unit_rate.washing / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'washing', ((pay_billing_unit_rate.traveling / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'travelling', ((pay_billing_unit_rate.education / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'education', ((pay_billing_unit_rate.national_holiday_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'NH', ((pay_billing_unit_rate.allowances / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'allowances', CASE WHEN pay_employee_master.cca = 0 THEN ((pay_billing_unit_rate.cca / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE ((pay_employee_master.cca / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) END AS 'cca_billing', CASE WHEN pay_employee_master.special_allow = 0 THEN ((pay_billing_unit_rate.otherallowance / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE pay_employee_master.special_allow END AS 'other_allow', CASE WHEN pay_billing_master_history.ot_policy_billing = '1' THEN ((pay_billing_master_history.ot_amount_billing / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'hrs_12_ot', pay_billing_master_history.bill_esic_percent AS 'esic_percent', pay_billing_master_history.bill_pf_percent AS 'pf_percent', ((pay_billing_unit_rate.lwf / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'lwf', ((pay_billing_unit_rate.uniform / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'uniform', ((pay_billing_unit_rate.relieving_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'relieving_charg', ((pay_billing_unit_rate.operational_cost / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'operational_cost', pay_attendance_muster.tot_days_present, ROUND(((pay_billing_unit_rate.sub_total_c / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present), 2) AS 'baseamount', bill_service_charge, pay_billing_master_history.hours, pay_billing_unit_rate.sub_total_c, pay_billing_master_history.bill_ser_operations, pay_billing_master_history.bill_ser_uniform, pay_billing_unit_rate.ot_1_hr_amount AS 'ot_rate', pay_attendance_muster.ot_hours, pay_billing_unit_rate.esic_amount, pay_billing_unit_rate.esi_on_ot_amount, pay_employee_master.cca AS 'emp_cca', pay_billing_unit_rate.gross AS 'bill_gross', pay_billing_unit_rate.pf_amount AS 'bill_pf', pay_billing_unit_rate.uniform AS 'bill_uniform', ((pay_billing_master_history.group_insurance_billing / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'group_insurance_billing', service_group_insurance_billing, pay_employee_master.Employee_type, ((bill_service_charge_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) as bill_service_charge_amount, pay_billing_master_history.esic_common_allow,CASE WHEN pay_employee_master.special_allow = 0 THEN ((pay_billing_unit_rate.common_allowance / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE (( pay_employee_master . special_allow/  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present) END AS 'common_allow',IFNULL(branch_type,0) as 'branch_type',pay_billing_master_history.esic_oa_billing,pay_billing_unit_rate.month_days,gst_applicable,OPus_NO FROM pay_employee_master INNER JOIN " + pay_attendance_muster + " ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_muster.unit_code = pay_unit_master.unit_code AND pay_attendance_muster.comp_code = pay_unit_master.comp_code INNER JOIN pay_billing_unit_rate ON pay_attendance_muster.unit_code = pay_billing_unit_rate.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate.month AND pay_attendance_muster.year = pay_billing_unit_rate.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.comp_code = pay_employee_master.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_employee_master.grade_code = pay_billing_master_history.designation AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code INNER JOIN pay_grade_master ON pay_billing_master_history.comp_code = pay_grade_master.comp_code AND pay_billing_master_history.designation = pay_grade_master.GRADE_CODE INNER JOIN pay_client_master ON pay_unit_master.comp_code = pay_client_master.comp_code AND pay_unit_master.client_code = pay_client_master.client_code WHERE  " + where;
                    sql = "SELECT txt_zone,zone,CASE WHEN invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', client_code,case when client_code = 'BAGIC TM' then 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' else client end AS 'client',state_name,unit_name,unit_city, if(client_code ='4' ,branch_cost_centre_code,client_branch_code) as 'client_branch_code',emp_name,grade_desc,emp_basic_vda,hra,bonus_gross,leave_gross,gratuity_gross,washing,travelling,education,allowances,cca_billing,other_allow,gross,bonus_after_gross,leave_after_gross,gratuity_after_gross,pf,esic,hrs_12_ot,esic_ot,lwf,uniform,relieving_charg,operational_cost,tot_days_present,amount as 'Amount',Service_charge,CGST9,IGST18,SGST9,bill_service_charge,NH,hours,fromtodate,(amount * month_days/tot_days_present) as 'sub_total_c',ot_rate,ot_hours,ot_amount,group_insurance_billing,bill_service_charge_amount,bill_service_charge_amount,branch_type,month_days,gst_applicable,OPus_NO,unit_code,yearly_bonus,yearly_gratuity from pay_billing_unit_rate_history  " + where + " " + order_by_clause;
                }
            }
            //client attendance
            else if (i == 3)
            {
                string hdfc_type = "";
                if (ot_invoice == 0)
                { hdfc_type = "  and (hdfc_type='manpower_bill' || hdfc_type is null)"; }

                if (ddl_invoice_type == "2" && ddl_arrears_type != "Select")
                {
                    grade = " and pay_billing_unit_rate_history.grade_code = '" + ddl_designatione + "'";

                }
                from_to_date = from_to_date + " " + billing_type;

                if (ddl_billing_state.Equals("ALL") && state_name_arrear_state != "" && type_cl == 1 && ot_invoice == 1)
                {
                    grade = grade + " and pay_billing_unit_rate_history.hdfc_type = 'ot_bill'";
                }
                else if (ddl_billing_state.Equals("ALL") && state_name_arrear_state != "" && type_cl == 1)
                {
                    grade = grade + " and pay_billing_unit_rate_history.state_name in (" + state_name_arrear_state.Substring(0, state_name_arrear_state.Length - 1) + ") ";
                }

                if (start_date_common != "" && start_date_common != "1")
                {
                    //d.update_attendance(Session["COMP_CODE"].ToString(), ddl_client.SelectedValue, ddl_unitcode, txt_month_year.Text, int.Parse(start_date_common));
                    where = " pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_unit_rate_history.unit_code = '" + ddl_unitcode + "'  " + billing_bfl + "  and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_billing_unit_rate_history.tot_days_present > 0  " + flag + " " + grade + hdfc_type + "  " + from_to_date;
                    if (ddl_billing_state == "ALL")
                    {
                        where = " pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' " + billing_bfl + " and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_billing_unit_rate_history.tot_days_present > 0 " + flag + " " + grade + hdfc_type + "  " + from_to_date;
                    }
                    else if (ddl_unitcode == "ALL")
                    {
                        where = " pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' " + billing_bfl + " and pay_billing_unit_rate_history.state_name = '" + ddl_billing_state + "' and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "'  and pay_billing_unit_rate_history.tot_days_present > 0  " + flag + " " + grade + hdfc_type + "  " + from_to_date;
                    }

                    if (ddl_invoice_type == "2") { where = " pay_billing_unit_rate_history.grade_code = '" + ddl_designatione + "' and " + where; }
                    string getdays = "";
                    if (ddl_start_date_common != "0" && ddl_end_date_common != "0")
                    {
                        getdays = get_selected_days(ddl_start_date_common, ddl_end_date_common);
                        month_days = DateTime.DaysInMonth(int.Parse(txt_month_year.Text.Substring(3)), int.Parse(txt_month_year.Text.Substring(0, 2)));
                    }
                    else
                    {
                        getdays = d.get_calendar_days(int.Parse(start_date_common), txt_month_year.Text, 1, 2);
                    }
                    if (!getdays.Contains("DAY31"))
                    {
                        getdays = getdays + " 0 as 'DAY31',";
                    }
                    if (!getdays.Contains("DAY30"))
                    {
                        getdays = getdays + " 0 as 'DAY30',";
                    }
                    if (!getdays.Contains("DAY29"))
                    {
                        getdays = getdays + " 0 as 'DAY29',";
                    }
                    if (!getdays.Contains("DAY28"))
                    {
                        getdays = getdays + " 0 as 'DAY28',";
                    }
                    sql = "select pay_billing_unit_rate_history.client_code,pay_billing_unit_rate_history.zone, pay_billing_unit_rate_history.txt_zone,pay_billing_unit_rate_history.state_name,branch_type, pay_billing_unit_rate_history.unit_city,pay_billing_unit_rate_history.unit_name, pay_billing_unit_rate_history.client_branch_code, pay_billing_unit_rate_history.emp_name, pay_billing_unit_rate_history.grade_desc,(IF((SELECT  billing_ot FROM pay_client_master WHERE pay_client_master.comp_code = pay_billing_unit_rate_history.COMP_CODE AND pay_client_master.client_code = pay_billing_unit_rate_history.client_code) = 'With OT', pay_attendance_muster.ot_hours, 0)) AS 'ot_hours'," + getdays + " pay_attendance_muster.tot_days_present, pay_attendance_muster.tot_days_absent as absent, pay_attendance_muster.tot_working_days as 'total days',IF(pay_employee_master.LEFT_DATE IS NULL, 'CONTINUE', 'LEFT') AS 'STATUS' from pay_billing_unit_rate_history INNER JOIN " + pay_attendance_muster + " ON pay_attendance_muster.emp_code = pay_billing_unit_rate_history.emp_code and pay_attendance_muster.comp_code = pay_billing_unit_rate_history.comp_code AND   pay_attendance_muster.unit_code = pay_billing_unit_rate_history.unit_code   AND  pay_attendance_muster . month  =  pay_billing_unit_rate_history . month  AND  pay_attendance_muster . year  =  pay_billing_unit_rate_history . year INNER JOIN pay_employee_master ON pay_employee_master.COMP_CODE = pay_attendance_muster.COMP_CODE AND pay_employee_master.UNIT_CODE = pay_attendance_muster.UNIT_CODE AND pay_employee_master.EMP_CODE = pay_attendance_muster.EMP_CODE  left join pay_attendance_muster t2 on  t2.year = " + (int.Parse(txt_month_year.Text.Substring(0, 2)) == 1 ? int.Parse(txt_month_year.Text.Substring(3)) - 1 : int.Parse(txt_month_year.Text.Substring(3))) + " and pay_attendance_muster.COMP_CODE = t2.COMP_CODE and pay_attendance_muster.UNIT_CODE = t2.UNIT_CODE and pay_attendance_muster.EMP_CODE = t2.EMP_CODE and t2.month = " + (int.Parse(txt_month_year.Text.Substring(0, 2)) == 1 ? 12 : int.Parse(txt_month_year.Text.Substring(0, 2)) - 1) + " where " + where + " group by pay_billing_unit_rate_history.EMP_CODE " + order_by_clause;

                }
                else
                {

                    if (ddl_client.SelectedValue == "HDFC" && ot_invoice == 0)
                    { hdfc_type = "  and (hdfc_type='manpower_bill' || hdfc_type is null)"; }
                    where = " pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_unit_rate_history.unit_code = '" + ddl_unitcode + "' and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_billing_unit_rate_history.tot_days_present > 0  " + flag + "  " + grade + hdfc_type + "  " + from_to_date;
                    if (ddl_billing_state == "ALL")
                    {
                        where = " pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_billing_unit_rate_history.tot_days_present > 0  " + flag + "  " + grade + hdfc_type + "  " + from_to_date;
                    }
                    else if (ddl_unitcode == "ALL")
                    {
                        where = "pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.state_name = '" + ddl_billing_state + "' and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_billing_unit_rate_history.tot_days_present > 0 " + flag + "  " + grade + hdfc_type + "  " + from_to_date;
                    }
                    if (ddl_invoice_type == "2") { where = " pay_billing_unit_rate_history.grade_code = '" + ddl_designatione + "' and " + where; }
                    sql = "select pay_billing_unit_rate_history.client_code, pay_billing_unit_rate_history.zone, pay_billing_unit_rate_history.txt_zone, pay_billing_unit_rate_history.state_name, branch_type, pay_billing_unit_rate_history.unit_city,pay_billing_unit_rate_history.unit_name, pay_billing_unit_rate_history.client_branch_code, pay_billing_unit_rate_history.emp_name, pay_billing_unit_rate_history.grade_desc,(IF((SELECT  billing_ot FROM pay_client_master WHERE pay_client_master.comp_code = pay_billing_unit_rate_history.COMP_CODE AND pay_client_master.client_code = pay_billing_unit_rate_history.client_code) = 'With OT', pay_attendance_muster.ot_hours, 0)) AS 'ot_hours' , case when DAY01 = '0' then 'A' else DAY01 end as DAY01, case when DAY02 = '0' then 'A' else DAY02 end as DAY02, case when DAY03 = '0' then 'A' else DAY03 end as DAY03, case when DAY04 = '0' then 'A' else DAY04 end as DAY04, case when DAY05 = '0' then 'A' else DAY05 end as DAY05, case when DAY06 = '0' then 'A' else DAY06 end as DAY06, case when DAY07 = '0' then 'A' else DAY07 end as DAY07, case when DAY08 = '0' then 'A' else DAY08 end as DAY08, case when DAY09 = '0' then 'A' else DAY09 end as DAY09, case when DAY10 = '0' then 'A' else DAY10 end as DAY10, case when DAY11 = '0' then 'A' else DAY11 end as DAY11, case when DAY12 = '0' then 'A' else DAY12 end as DAY12, case when DAY13 = '0' then 'A' else DAY13 end as DAY13, case when DAY14 = '0' then 'A' else DAY14 end as DAY14, case when DAY15 = '0' then 'A' else DAY15 end as DAY15, case when DAY16 = '0' then 'A' else DAY16 end as DAY16, case when DAY17 = '0' then 'A' else DAY17 end as DAY17, case when DAY18 = '0' then 'A' else DAY18 end as DAY18, case when DAY19 = '0' then 'A' else DAY19 end as DAY19, case when DAY20 = '0' then 'A' else DAY20 end as DAY20, case when DAY21 = '0' then 'A' else DAY21 end as DAY21, case when DAY22 = '0' then 'A' else DAY22 end as DAY22, case when DAY23 = '0' then 'A' else DAY23 end as DAY23, case when DAY24 = '0' then 'A' else DAY24 end as DAY24, case when DAY25 = '0' then 'A' else DAY25 end as DAY25, case when DAY26 = '0' then 'A' else DAY26 end as DAY26, case when DAY27 = '0' then 'A' else DAY27 end as DAY27, case when DAY28 = '0' then 'A' else DAY28 end as DAY28, case when DAY29 = '0' then 'A' else DAY29 end as DAY29, case when DAY30 = '0' then 'A' else DAY30 end as DAY30, case when DAY31 = '0' then 'A' else DAY31 end as DAY31, pay_attendance_muster.tot_days_present, CASE WHEN (pay_attendance_muster.tot_working_days - pay_attendance_muster.tot_days_present) < 0 THEN 0 ELSE pay_attendance_muster.tot_working_days - pay_attendance_muster.tot_days_present END AS 'absent',DAY(LAST_DAY('" + txt_month_year.Text.Substring(3) + "-" + txt_month_year.Text.Substring(0, 2) + "-1')) AS 'total days', IF(pay_employee_master.LEFT_DATE IS NULL, 'CONTINUE', 'LEFT') AS 'STATUS' from pay_billing_unit_rate_history INNER JOIN " + pay_attendance_muster + " ON pay_attendance_muster.emp_code = pay_billing_unit_rate_history.emp_code and pay_attendance_muster.comp_code = pay_billing_unit_rate_history.comp_code  and pay_attendance_muster.unit_code = pay_billing_unit_rate_history.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate_history.month AND pay_attendance_muster.year = pay_billing_unit_rate_history.year  INNER JOIN pay_employee_master ON pay_employee_master.COMP_CODE = pay_attendance_muster.COMP_CODE AND pay_employee_master.UNIT_CODE = pay_attendance_muster.UNIT_CODE AND pay_employee_master.EMP_CODE = pay_attendance_muster.EMP_CODE where " + where + " group by pay_billing_unit_rate_history.EMP_CODE" + order_by_clause;

                }
            }
            else if (i == 4)
            {
                if (ddl_client.SelectedValue == "UTKARSH")
                {
                    where = "pay_company_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_unit_master.unit_code = '" + ddl_unitcode + "' and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_attendance_muster.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_attendance_muster.tot_days_present > 0 group BY pay_unit_master.state_name order by 4,3) AS billing_table) as Final_billing";
                    if (ddl_billing_state == "ALL")
                    {
                        where = "pay_company_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_attendance_muster.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_attendance_muster.tot_days_present > 0 group BY pay_unit_master.state_name order by 4,3) AS billing_table) as Final_billing";
                    }
                    else if (ddl_unitcode == "ALL")
                    {
                        where = "pay_company_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_unit_master.state_name = '" + ddl_billing_state + "' and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_attendance_muster.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_attendance_muster.tot_days_present > 0  order by 4,3) AS billing_table) as Final_billing";
                    }
                    sql = "SELECT client, state_name, unit_name, unit_city,uniform, relieving_charg, operational_cost,client_branch_code, (Total + pf + esic + group_insurance_billing_ser) AS 'Amount', IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, (((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100)) AS 'Service_charge', CASE WHEN LOCATE(company_state, state_name) THEN ROUND(((((Total + pf + esic + operational_cost + uniform + (ot_rate * ot_hours) + group_insurance_billing_ser + group_insurance_billing) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, (((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100))) * 9) / 100), 2) ELSE 0 END AS 'CGST9', CASE WHEN LOCATE(company_state, state_name) THEN 0 ELSE ROUND(((((Total + pf + esic + operational_cost + uniform + (ot_rate * ot_hours) + group_insurance_billing_ser + group_insurance_billing) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, (((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100))) * 18) / 100), 2) END AS 'IGST18', CASE WHEN LOCATE(company_state, state_name) THEN ROUND(((((Total + pf + esic + operational_cost + uniform + (ot_rate * ot_hours) + group_insurance_billing_ser + group_insurance_billing) + IF((((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100) = 0, bill_service_charge_amount, (((Total + pf + esic + (ot_rate * ot_hours) + group_insurance_billing_ser) * bill_service_charge) / 100))) * 9) / 100), 2) ELSE 0 END AS 'SGST9', bill_service_charge,IF(ot_hours > 0, ot_rate, 0) AS 'ot_rate', ot_hours, (ot_rate * ot_hours) AS 'ot_amount', group_insurance_billing, bill_service_charge_amount,branch_type,state_gst,client_code FROM (SELECT client, company_state, unit_name, state_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, hra, bonus_gross, leave_gross, gratuity_gross, washing, travelling, education, allowances, cca_billing, other_allow, (emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross + hrs_12_ot) AS 'gross', bonus_after_gross, leave_after_gross, gratuity_after_gross, (((emp_basic_vda) / 100) * pf_percent) AS 'pf', (((emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross + hrs_12_ot) / 100) * esic_percent) AS 'esic', hrs_12_ot, (((hrs_12_ot) * esic_percent) / 100) AS 'esic_ot', lwf, CASE WHEN bill_ser_uniform = 1 THEN 0 ELSE uniform END AS 'uniform', relieving_charg, CASE WHEN bill_ser_operations = 1 THEN 0 ELSE operational_cost END AS 'operational_cost', tot_days_present, (emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross + bonus_after_gross + leave_after_gross + gratuity_after_gross + lwf + CASE WHEN bill_ser_uniform = 0 THEN 0 ELSE uniform END + relieving_charg + CASE WHEN bill_ser_operations = 0 THEN 0 ELSE operational_cost END + NH + hrs_12_ot) AS 'Total', bill_service_charge, NH, hours, (bill_gross + emp_cca) AS 'bill_gross', sub_total_c, bill_ser_uniform, bill_ser_operations, (ot_rate + esi_on_ot_amount) AS 'ot_rate', ot_hours, esic_amount, esi_on_ot_amount, emp_cca, bill_pf, bill_uniform, esic_percent, IF(Employee_type = 'Permanent', CASE WHEN service_group_insurance_billing = 0 THEN group_insurance_billing ELSE 0 END, 0) AS 'group_insurance_billing', CASE WHEN service_group_insurance_billing = 1 THEN group_insurance_billing ELSE 0 END AS 'group_insurance_billing_ser', bill_service_charge_amount,branch_type,state_gst,client_code FROM (SELECT (SELECT client_name FROM pay_client_master WHERE client_code = pay_unit_master.client_code AND comp_code = '" + Session["COMP_CODE"].ToString() + "') AS 'client', pay_company_master.state AS 'company_state', pay_unit_master.unit_name, pay_unit_master.state_name, pay_unit_master.unit_city, pay_unit_master.client_branch_code, pay_employee_master.emp_name, pay_grade_master.grade_desc, (((pay_billing_master_history.basic + pay_billing_master_history.vda) / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'emp_basic_vda', ((pay_billing_unit_rate.hra / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'hra', CASE WHEN bonus_taxable = '1' THEN ((pay_billing_unit_rate.bonus_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'bonus_gross', CASE WHEN bonus_taxable = '0' THEN ((pay_billing_unit_rate.bonus_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'bonus_after_gross', CASE WHEN leave_taxable = '1' THEN ((pay_billing_unit_rate.leave_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'leave_gross', CASE WHEN leave_taxable = '0' THEN ((pay_billing_unit_rate.leave_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'leave_after_gross', CASE WHEN gratuity_taxable = '1' THEN ((pay_billing_unit_rate.grauity_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'gratuity_gross', CASE WHEN gratuity_taxable = '0' THEN ((pay_billing_unit_rate.grauity_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'gratuity_after_gross', ((pay_billing_unit_rate.washing / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'washing', ((pay_billing_unit_rate.traveling / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'travelling', ((pay_billing_unit_rate.education / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'education', ((pay_billing_unit_rate.national_holiday_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'NH', ((pay_billing_unit_rate.allowances / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'allowances', CASE WHEN pay_employee_master.cca = 0 THEN ((pay_billing_unit_rate.cca / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE ((pay_employee_master.cca / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) END AS 'cca_billing', CASE WHEN pay_employee_master.special_allow = 0 THEN ((pay_billing_master_history.other_allow / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE pay_employee_master.special_allow END AS 'other_allow', CASE WHEN pay_billing_master_history.ot_policy_billing = '1' THEN ((pay_billing_master_history.ot_amount_billing / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'hrs_12_ot', pay_billing_master_history.bill_esic_percent AS 'esic_percent', pay_billing_master_history.bill_pf_percent AS 'pf_percent', ((pay_billing_unit_rate.lwf / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'lwf', ((pay_billing_unit_rate.uniform / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'uniform', ((pay_billing_unit_rate.relieving_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'relieving_charg', ((pay_billing_unit_rate.operational_cost / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'operational_cost', pay_attendance_muster.tot_days_present, ROUND(((pay_billing_unit_rate.sub_total_c / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present), 2) AS 'baseamount', bill_service_charge, pay_billing_master_history.hours, pay_billing_unit_rate.sub_total_c, pay_billing_master_history.bill_ser_operations, pay_billing_master_history.bill_ser_uniform, pay_billing_unit_rate.ot_1_hr_amount AS 'ot_rate', pay_attendance_muster.ot_hours, pay_billing_unit_rate.esic_amount, pay_billing_unit_rate.esi_on_ot_amount, pay_employee_master.cca AS 'emp_cca', pay_billing_unit_rate.gross AS 'bill_gross', pay_billing_unit_rate.pf_amount AS 'bill_pf', pay_billing_unit_rate.uniform AS 'bill_uniform', ((pay_billing_master_history.group_insurance_billing / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'group_insurance_billing', service_group_insurance_billing, pay_employee_master.Employee_type,((bill_service_charge_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) as bill_service_charge_amount,IFNULL(branch_type,0) as 'branch_type',(SELECT  Field2  FROM  pay_zone_master  WHERE  comp_code  = '" + Session["COMP_CODE"].ToString() + "' AND  client_code  = '" + ddl_client.SelectedValue + "' AND  Type  = 'GST' AND  REGION  = '" + ddl_billing_state + "') AS 'state_gst',pay_unit_master.client_code FROM pay_employee_master INNER JOIN pay_attendance_muster ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_muster.unit_code = pay_unit_master.unit_code AND pay_attendance_muster.comp_code = pay_unit_master.comp_code INNER JOIN pay_billing_unit_rate ON pay_attendance_muster.unit_code = pay_billing_unit_rate.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate.month AND pay_attendance_muster.year = pay_billing_unit_rate.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.comp_code = pay_employee_master.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_employee_master.grade_code = pay_billing_master_history.designation AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code INNER JOIN pay_grade_master ON pay_billing_master_history.comp_code = pay_grade_master.comp_code AND pay_billing_master_history.designation = pay_grade_master.GRADE_CODE WHERE " + where;
                }
                else if (ddl_client.SelectedValue == "MAX")
                {
                    if (ddl_invoice_type == "1")
                    {
                        where = "pay_company_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_unit_master.unit_code = '" + ddl_unitcode + "' and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_attendance_muster.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_attendance_muster.tot_days_present > 0 group by pay_unit_master.unit_code, pay_grade_master.GRADE_CODE order by pay_unit_master.state_name,pay_unit_master.unit_name";
                        if (ddl_billing_state == "ALL")
                        {
                            where = "pay_company_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_attendance_muster.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_attendance_muster.tot_days_present > 0 group by pay_unit_master.unit_code, pay_grade_master.GRADE_CODE order by pay_unit_master.state_name,pay_unit_master.unit_name";
                        }
                        else if (ddl_unitcode == "ALL")
                        {
                            where = "pay_company_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_unit_master.state_name = '" + ddl_billing_state + "' and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_attendance_muster.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_attendance_muster.tot_days_present > 0 group by pay_unit_master.unit_code, pay_grade_master.GRADE_CODE order by pay_unit_master.state_name,pay_unit_master.unit_name";
                        }

                    }
                    else
                    {

                        where = "pay_company_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_unit_master.unit_code = '" + ddl_unitcode + "' and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_attendance_muster.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_attendance_muster.tot_days_present > 0 and pay_grade_master.grade_code = '" + ddl_designatione + "' group by pay_unit_master.unit_code, pay_grade_master.GRADE_CODE order by pay_unit_master.state_name,pay_unit_master.unit_name";
                        if (ddl_billing_state == "ALL")
                        {
                            where = "pay_company_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_attendance_muster.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_attendance_muster.tot_days_present > 0 and pay_grade_master.grade_code = '" + ddl_designatione + "' group by pay_unit_master.unit_code, pay_grade_master.GRADE_CODE order by pay_unit_master.state_name,pay_unit_master.unit_name";
                        }
                        else if (ddl_unitcode == "ALL")
                        {
                            where = "pay_company_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_unit_master.state_name = '" + ddl_billing_state + "' and pay_attendance_muster.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_attendance_muster.Year = '" + txt_month_year.Text.Substring(3) + "' and pay_attendance_muster.tot_days_present > 0 and pay_grade_master.grade_code = '" + ddl_designatione + "' group by pay_unit_master.unit_code, pay_grade_master.GRADE_CODE order by pay_unit_master.state_name,pay_unit_master.unit_name";
                        }
                    }
                    sql = "SELECT  pay_unit_master.client_code,pay_unit_master . unit_name ,  pay_unit_master . state_name ,  pay_grade_master . grade_desc , (SELECT COUNT( pay_employee_master . emp_code ) FROM  pay_employee_master  WHERE  comp_code  =  pay_company_master . comp_code  AND  unit_code  =  pay_unit_master . unit_code  AND  grade_code  =  pay_grade_master . grade_code  AND  employee_type  = 'Permanent') AS 'emp_count', SUM( TOT_DAYS_PRESENT ) AS 'Present_Days',  pay_billing_unit_rate . grand_total , (( pay_billing_unit_rate . grand_total  /  pay_billing_unit_rate . month_days ) * SUM( TOT_DAYS_PRESENT )) AS 'Amount',date_format('" + txt_month_year.Text.Substring(3) + "-" + txt_month_year.Text.Substring(0, 2) + "-01' ,'%b-%Y') AS 'month' FROM pay_employee_master INNER JOIN pay_attendance_muster ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_muster.unit_code = pay_unit_master.unit_code AND pay_attendance_muster.comp_code = pay_unit_master.comp_code INNER JOIN pay_billing_unit_rate ON pay_attendance_muster.unit_code = pay_billing_unit_rate.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate.month AND pay_attendance_muster.year = pay_billing_unit_rate.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.comp_code = pay_employee_master.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_employee_master.grade_code = pay_billing_master_history.designation AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code INNER JOIN pay_grade_master ON pay_billing_master_history.comp_code = pay_grade_master.comp_code AND pay_billing_master_history.designation = pay_grade_master.GRADE_CODE WHERE " + where;
                }

                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Support Format Only for UTKARSH SMALL FINANCE BANK LTD.');", true);
                    return null;
                }
            }

            if (i == 5)
            {
                where = " WHERE pay_billing_unit_rate.month = " + txt_month_year.Text.Substring(0, 2) + " AND pay_billing_unit_rate.Year = " + txt_month_year.Text.Substring(3) + " and pay_billing_master_history.billing_state = '" + ddl_billing_state + "' and  pay_billing_unit_rate.comp_code='" + Session["COMP_CODE"].ToString() + "' and pay_billing_unit_rate.client_code='" + ddl_client.SelectedValue + "' and branch_status = 0";

                if (ddl_billing_state == "ALL")
                {

                    where = " WHERE pay_billing_unit_rate.month = " + txt_month_year.Text.Substring(0, 2) + " AND pay_billing_unit_rate.Year = " + txt_month_year.Text.Substring(3) + " and  pay_billing_unit_rate.comp_code='" + Session["COMP_CODE"].ToString() + "' and pay_billing_unit_rate.client_code='" + ddl_client.SelectedValue + "'";
                }

                // vinod sir query
                // sql = "SELECT client_name AS 'client', pay_billing_master_history.billing_state, pay_billing_master_history.designation, (pay_billing_master_history.basic + pay_billing_master_history.vda) AS 'emp_basic_vda', pay_billing_master_history.basic AS 'actual_basic', pay_billing_master_history.vda AS 'actual_vda', pay_billing_unit_rate.hra AS 'hra', CASE WHEN bonus_taxable = '1' THEN pay_billing_unit_rate.bonus_amount ELSE 0 END AS 'bonus_gross', CASE WHEN bonus_taxable = '0' THEN pay_billing_unit_rate.bonus_amount ELSE 0 END AS 'bonus_after_gross', CASE WHEN leave_taxable = '1' THEN pay_billing_unit_rate.leave_amount ELSE 0 END AS 'leave_gross', CASE WHEN leave_taxable = '0' THEN pay_billing_unit_rate.leave_amount ELSE 0 END AS 'leave_after_gross', CASE WHEN gratuity_taxable = '1' THEN pay_billing_unit_rate.grauity_amount ELSE 0 END AS 'gratuity_gross', CASE WHEN gratuity_taxable = '0' THEN pay_billing_unit_rate.grauity_amount ELSE 0 END AS 'gratuity_after_gross', pay_billing_unit_rate.washing AS 'washing', pay_billing_unit_rate.traveling AS 'travelling', pay_billing_unit_rate.education AS 'education', pay_billing_unit_rate.allowances AS 'allowances', pay_billing_unit_rate.cca AS 'cca_billing', pay_billing_unit_rate.otherallowance AS 'other_allow', CASE WHEN pay_billing_master_history.ot_policy_billing = '1' THEN pay_billing_master_history.ot_amount_billing ELSE 0 END AS 'hrs_12_ot', pay_billing_master_history.bill_esic_percent AS 'esic_percent', pay_billing_master_history.bill_pf_percent AS 'pf_percent', pay_billing_unit_rate.lwf AS 'monthlwf', CASE WHEN pay_billing_master_history.pf_cmn_on = 0 THEN pay_billing_unit_rate.lwf ELSE 0 END AS 'lwf', pay_billing_unit_rate.uniform AS 'uniform', pay_billing_unit_rate.relieving_amount AS 'relieving_charg', pay_billing_unit_rate.operational_cost AS 'operational_cost', ROUND(pay_billing_unit_rate.sub_total_c, 2) AS 'baseamount', bill_service_charge, pay_billing_master_history.hours, pay_billing_unit_rate.sub_total_c, pay_billing_master_history.bill_ser_operations, pay_billing_master_history.bill_ser_uniform, pay_billing_unit_rate.ot_1_hr_amount AS 'ot_rate', pay_billing_unit_rate.esic_amount, pay_billing_unit_rate.esi_on_ot_amount, pay_billing_unit_rate.gross AS 'bill_gross', pay_billing_unit_rate.pf_amount AS 'bill_pf', pay_billing_unit_rate.uniform AS 'bill_uniform', pay_billing_master_history.group_insurance_billing AS 'group_insurance_billing', service_group_insurance_billing, bill_service_charge_amount AS 'bill_service_charge_amount', pay_billing_master_history.esic_common_allow, pay_billing_unit_rate.common_allowance AS 'common_allow', pay_billing_master_history.esic_oa_billing, pay_billing_master_history.pf_cmn_on FROM pay_billing_unit_rate INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_client_master ON pay_billing_master_history.billing_client_code = pay_client_master.client_code " + where + " group by pay_billing_master_history.billing_state, pay_billing_master_history.designation ORDER BY 1,2,3";
                //sql = "SELECT pay_billing_unit_rate . comp_code , client_name AS 'client', pay_billing_master_history . billing_state , grade_desc , cast(CONCAT( pay_billing_master_history . hours , ' HRS ', pay_billing_unit_rate . month_days , ' DAYS') as char) AS 'DUTYHRS', ( pay_billing_master_history . basic + pay_billing_master_history . vda ) AS 'emp_basic_vda', pay_billing_master_history . basic AS 'actual_basic', pay_billing_master_history . vda AS 'actual_vda', pay_billing_unit_rate . hra , CASE WHEN bonus_taxable = '1' THEN pay_billing_unit_rate . bonus_amount ELSE 0 END AS 'bonus_gross', CASE WHEN bonus_taxable = '0' THEN pay_billing_unit_rate . bonus_amount ELSE 0 END AS 'bonus_after_gross', CASE WHEN leave_taxable = '1' THEN pay_billing_unit_rate . leave_amount ELSE 0 END AS 'leave_gross', CASE WHEN leave_taxable = '0' THEN pay_billing_unit_rate . leave_amount ELSE 0 END AS 'leave_after_gross', CASE WHEN gratuity_taxable = '1' THEN pay_billing_unit_rate . grauity_amount ELSE 0 END AS 'gratuity_gross', CASE WHEN gratuity_taxable = '0' THEN pay_billing_unit_rate . grauity_amount ELSE 0 END AS 'gratuity_after_gross', if(esic_oa_billing=0,0, pay_billing_unit_rate . allowances ) as 'allowances', if(esic_oa_billing=1,0, pay_billing_unit_rate . allowances ) as 'allowances_after_gross', pay_billing_unit_rate . washing , pay_billing_unit_rate . traveling AS 'travelling', pay_billing_unit_rate . education , pay_billing_unit_rate . cca AS 'cca_billing', pay_billing_unit_rate . otherallowance AS 'other_allow', CASE WHEN pay_billing_master_history . ot_policy_billing = '1' THEN pay_billing_master_history . ot_amount_billing ELSE 0 END AS 'hrs_12_ot', pay_billing_master_history . bill_esic_percent AS 'esic_percent', pay_billing_master_history . bill_pf_percent AS 'pf_percent', pay_billing_unit_rate . lwf AS 'monthlwf', CASE WHEN pay_billing_master_history . pf_cmn_on = 0 THEN pay_billing_unit_rate . lwf ELSE 0 END AS 'lwf', pay_billing_unit_rate . uniform , pay_billing_unit_rate . relieving_amount AS 'relieving_charg', pay_billing_unit_rate . operational_cost , ROUND( pay_billing_unit_rate . sub_total_c , 2) AS 'baseamount', bill_service_charge , pay_billing_master_history . hours , pay_billing_unit_rate . sub_total_c , pay_billing_master_history . bill_ser_operations , pay_billing_master_history . bill_ser_uniform , pay_billing_unit_rate . ot_1_hr_amount AS 'ot_rate', pay_billing_unit_rate . esic_amount , pay_billing_unit_rate . esi_on_ot_amount , pay_billing_unit_rate . gross AS 'bill_gross', pay_billing_unit_rate . pf_amount AS 'bill_pf', pay_billing_unit_rate . uniform AS 'bill_uniform', pay_billing_master_history . group_insurance_billing , service_group_insurance_billing , bill_service_charge_amount , pay_billing_master_history . esic_common_allow , pay_billing_unit_rate . common_allowance AS 'common_allow', pay_billing_master_history . esic_oa_billing , pay_billing_master_history . pf_cmn_on , pay_billing_master_history . bill_bonus_percent , pay_billing_master_history . leave_days , gratuity_percent , hra_percent , bonus_rate , CASE WHEN bill_ser_uniform = '1' THEN pay_billing_master_history . bill_uniform_rate ELSE 0 END AS 'uniform_gross', CASE WHEN bill_ser_uniform = '0' THEN pay_billing_master_history . bill_uniform_rate ELSE 0 END AS 'uniform_after_gross', CASE WHEN pay_billing_master_history . bill_ser_operations = '1' THEN pay_billing_master_history . bill_oper_cost_amt ELSE 0 END AS 'operational_gross', CASE WHEN pay_billing_master_history . bill_ser_operations = '0' THEN pay_billing_master_history . bill_uniform_rate ELSE 0 END AS 'operational_after_gross', 0 AS 'special_allowance', 0 AS 'NH', 0 AS 'rate', 0 AS 'grandtotoal' FROM pay_billing_unit_rate INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_client_master ON pay_billing_master_history.billing_client_code = pay_client_master.client_code  inner join pay_grade_master on  pay_billing_unit_rate.comp_code = pay_grade_master.comp_code and pay_billing_unit_rate.designation = pay_grade_master.grade_code " + where + " group by pay_billing_master_history.billing_state, pay_billing_master_history.designation ORDER BY 1,2,3";

                sql = "SELECT pay_billing_unit_rate.comp_code, client_name AS 'client', pay_billing_master_history.billing_state, unit_name, grade_desc, CAST(CONCAT(pay_billing_master_history.hours, ' HRS ', pay_billing_unit_rate.month_days, ' DAYS') AS char) AS 'DUTYHRS', (pay_billing_master_history.basic + pay_billing_master_history.vda) AS 'emp_basic_vda', pay_billing_master_history.basic AS 'actual_basic', pay_billing_master_history.vda AS 'actual_vda', pay_billing_unit_rate.hra, CASE WHEN bonus_taxable = '1' THEN pay_billing_unit_rate.bonus_amount ELSE 0 END AS 'bonus_gross', CASE WHEN bonus_taxable = '0' THEN pay_billing_unit_rate.bonus_amount ELSE 0 END AS 'bonus_after_gross', CASE WHEN leave_taxable = '1' THEN pay_billing_unit_rate.leave_amount ELSE 0 END AS 'leave_gross', CASE WHEN leave_taxable = '0' THEN pay_billing_unit_rate.leave_amount ELSE 0 END AS 'leave_after_gross', CASE WHEN gratuity_taxable = '1' THEN pay_billing_unit_rate.grauity_amount ELSE 0 END AS 'gratuity_gross', CASE WHEN gratuity_taxable = '0' THEN pay_billing_unit_rate.grauity_amount ELSE 0 END AS 'gratuity_after_gross', IF(esic_oa_billing = 0, 0, pay_billing_unit_rate.allowances) AS 'allowances', IF(esic_oa_billing = 1, 0, pay_billing_unit_rate.allowances) AS 'allowances_after_gross', pay_billing_unit_rate.washing, pay_billing_unit_rate.traveling AS 'travelling', pay_billing_unit_rate.education, pay_billing_unit_rate.cca AS 'cca_billing', pay_billing_unit_rate.otherallowance AS 'other_allow', CASE WHEN pay_billing_master_history.ot_policy_billing = '1' THEN pay_billing_master_history.ot_amount_billing ELSE 0 END AS 'hrs_12_ot', pay_billing_master_history.bill_esic_percent AS 'esic_percent', pay_billing_master_history.bill_pf_percent AS 'pf_percent', pay_billing_unit_rate.lwf AS 'monthlwf', CASE WHEN pay_billing_master_history.pf_cmn_on = 0 THEN pay_billing_unit_rate.lwf ELSE 0 END AS 'lwf', pay_billing_unit_rate.uniform, pay_billing_unit_rate.relieving_amount AS 'relieving_charg', pay_billing_unit_rate.operational_cost, ROUND(pay_billing_unit_rate.sub_total_c, 2) AS 'baseamount', bill_service_charge, 0 AS 'hours', pay_billing_unit_rate.sub_total_c, pay_billing_master_history.bill_ser_operations, pay_billing_master_history.bill_ser_uniform, pay_billing_unit_rate.ot_1_hr_amount AS 'ot_rate', pay_billing_unit_rate.esic_amount, pay_billing_unit_rate.esi_on_ot_amount, pay_billing_unit_rate.gross AS 'bill_gross', pay_billing_unit_rate.pf_amount AS 'bill_pf', pay_billing_unit_rate.uniform AS 'bill_uniform', pay_billing_master_history.group_insurance_billing, service_group_insurance_billing, bill_service_charge_amount, pay_billing_master_history.esic_common_allow, pay_billing_unit_rate.common_allowance AS 'common_allow', pay_billing_master_history.esic_oa_billing, pay_billing_master_history.pf_cmn_on, pay_billing_master_history.bill_bonus_percent, pay_billing_master_history.leave_days, gratuity_percent, hra_percent, bonus_rate, CASE WHEN bill_ser_uniform = '1' THEN pay_billing_master_history.bill_uniform_rate ELSE 0 END AS 'uniform_gross', CASE WHEN bill_ser_uniform = '0' THEN pay_billing_master_history.bill_uniform_rate ELSE 0 END AS 'uniform_after_gross', CASE WHEN pay_billing_master_history.bill_ser_operations = '1' THEN pay_billing_master_history.bill_oper_cost_amt ELSE 0 END AS 'operational_gross', CASE WHEN pay_billing_master_history.bill_ser_operations = '0' THEN pay_billing_master_history.bill_uniform_rate ELSE 0 END AS 'operational_after_gross', 0 AS 'special_allowance', 0 AS 'NH', 0 AS 'rate', 0 AS 'grandtotoal' FROM pay_billing_unit_rate INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_client_master ON pay_billing_master_history.billing_client_code = pay_client_master.client_code INNER JOIN pay_unit_master ON pay_billing_master_history.billing_client_code = pay_unit_master.client_code AND pay_billing_master_history.billing_unit_code = pay_unit_master.unit_code INNER JOIN pay_grade_master ON pay_billing_unit_rate.comp_code = pay_grade_master.comp_code AND pay_billing_unit_rate.designation = pay_grade_master.grade_code " + where + " group by pay_billing_master_history.billing_state,pay_billing_master_history.billing_unit_code,pay_billing_master_history.designation ORDER BY 1,2,3";
            }
            // vikas add arrears rate breakup
            else if (i == 6)
            {
                string multi = "";
                if (type_cl == 1)
                {
                    multi = " and pay_billing_unit_rate_history_arrears.invoice_flag!=0 ";
                    if (ddl_billing_state.Equals("ALL") && state_name_arrear_state != "" && type_cl == 1)
                    {
                        multi = multi + " and pay_billing_unit_rate_history_arrears.state_name in (" + state_name_arrear_state.Substring(0, state_name_arrear_state.Length - 1) + ") ";
                    }
                }
                string old_month = "", new_month = "", new_yera = "", old_month_year = "", new_month_year = "", year = "";
                string start_date = "1";
                if (ddl_arrears_type.Equals("month"))
                {

                    old_month = txt_month_year.Text.Substring(0, 2);
                    old_month_year = " and pay_billing_unit_rate_history_arrears.month in (" + txt_month_year.Text.Substring(0, 2) + ") and pay_billing_unit_rate_history_arrears.year in (" + txt_month_year.Text.Substring(3) + ") ";
                    new_month_year = " and pay_billing_unit_rate_history_arrears.month in (" + txt_month_year.Text.Substring(0, 2) + ") and pay_billing_unit_rate_history_arrears.year in (" + txt_month_year.Text.Substring(3) + ") ";

                }
                else
                {
                    old_month = txt_arrear_month_year.Substring(3, 2);
                    old_month_year = " and pay_billing_unit_rate_history_arrears.month in (" + txt_arrear_month_year.Substring(3, 2) + ") and pay_billing_unit_rate_history_arrears.year in (" + txt_arrear_month_year.Substring(6) + ") ";
                    new_month_year = " and pay_billing_unit_rate_history_arrears.month in (" + txt_arrear_month_year.Substring(3, 2) + ") and pay_billing_unit_rate_history_arrears.year in (" + txt_arrear_month_year.Substring(6) + ") ";


                }
                if (start_date != "1")
                {
                    month_name = d.getmont1(old_month);
                    month_name = month_name.Substring(0, month_name.Length - 1);
                }
                else { month_name = d.getmont1(old_month); }
                month_name = month_name + " /" + year2.ToUpper();
                month_name = month_name + " " + year.ToUpper();
                // grade = grade + " " + from_to_date;
                where = " WHERE pay_billing_unit_rate_history_arrears.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history_arrears.client_code = '" + ddl_client.SelectedValue + "' " + where_state_arrears + " and state_name  = '" + ddl_billing_state + "' and pay_billing_unit_rate_history_arrears.unit_code = '" + ddl_unitcode + "' " + grade;

                if (ddl_billing_state == "ALL")
                {

                    where = " WHERE pay_billing_unit_rate_history_arrears.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history_arrears.client_code = '" + ddl_client.SelectedValue + "' " + where_state_arrears + " " + grade;
                }
                else if (ddl_unitcode == "ALL")
                {

                    where = " WHERE pay_billing_unit_rate_history_arrears.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history_arrears.client_code = '" + ddl_client.SelectedValue + "' " + where_state_arrears + " and pay_billing_unit_rate_history_arrears.state_name  = '" + ddl_billing_state + "' " + grade;
                }
                if (new_month != "")
                {
                    sql = "SELECT DISTINCT emp_code, 'old' as 'aa',pay_billing_unit_rate_history_arrears.month,pay_billing_unit_rate_history_arrears.year, client,  state_name,  branch_type,  unit_name,  pay_billing_unit_rate_history_arrears.comp_code,  emp_name,  grade_desc,  cast(CONCAT(pay_billing_unit_rate_history_arrears.hours, ' HRS ', pay_billing_unit_rate_history_arrears.month_days, ' DAYS') as char) AS 'DUTYHRS',  tot_days_present,  emp_basic_vda,  bonus_amount_billing,  pay_billing_unit_rate_history_arrears.washing,  pay_billing_unit_rate_history_arrears.travelling,  pay_billing_unit_rate_history_arrears.education,  IF(esic_oa_billing = 1, pay_billing_unit_rate_history_arrears.allowances, 0) AS 'allowances_esic',  cca_billing,  pay_billing_unit_rate_history_arrears.other_allow,  bonus_gross,  leave_gross,  gratuity_gross,  pay_billing_unit_rate_history_arrears.hra,  CASE WHEN pay_billing_master_history_arrears.ot_policy_billing = '1' THEN ((pay_billing_master_history_arrears.ot_amount_billing / pay_billing_unit_rate_history_arrears.month_days) * pay_billing_unit_rate_history_arrears.tot_days_present) ELSE 0 END AS 'special_allowance',  pay_billing_unit_rate_history_arrears.gross,  bonus_after_gross,  leave_after_gross,  gratuity_after_gross,  NH,  ifnull(pf,0) as 'pf',  ifnull(esic,0) as 'esic',   '0' AS 'uniform_ser',  pay_billing_unit_rate_history_arrears.group_insurance_billing,  pay_billing_unit_rate_history_arrears.lwf,  '0' AS 'operational_cost',  IF(esic_oa_billing = 0, pay_billing_unit_rate_history_arrears.allowances, 0) AS 'allowances_no_esic',  ifnull(amount,0) AS 'sub_total_a',  IF((ot_rate - esi_on_ot_amount) > 0, (ot_rate - esi_on_ot_amount), 0) AS 'ot_rate',  pay_billing_unit_rate_history_arrears.esic_ot,  ot_hours,  ifnull(ot_rate,0) AS 'sub_total_b',  ifnull(amount,0) AS 'sub_total_ab',  ifnull(relieving_charg,0) as 'relieving_charg',  ifnull(amount,0) AS 'sub_total_c',  '0' AS 'uniform_no_ser',   '0' AS 'operational_cost_no_ser', ifnull(Service_charge,0) as 'Service_charge',  ((ifnull(amount,0))  + (ifnull(Service_charge,0))  + (ot_rate * ot_hours)) AS 'Amount',  pay_billing_master_history_arrears.bill_bonus_percent,  pay_billing_master_history_arrears.leave_days,  pay_billing_master_history_arrears.gratuity_percent,  pay_billing_master_history_arrears.hra_percent,  pay_billing_master_history_arrears.bill_pf_percent,  pay_billing_master_history_arrears.bill_esic_percent,  pay_billing_master_history_arrears.bill_service_charge,  pay_billing_master_history_arrears.basic,  pay_billing_master_history_arrears.vda,  pay_billing_unit_rate_arrears.bonus_rate,  '0' AS 'group_insurance_billing_ser',  IF(ot_hours > 0, (ot_rate - esi_on_ot_amount), 0) AS 'ot_pr_hr_rate',  IF(ot_hours > 0 AND ot_rate > 0, esi_on_ot_amount, 0) AS 'esi_on_ot_amount',  (ot_rate * ot_hours) AS 'ot_amount'  FROM  pay_billing_unit_rate_history_arrears  INNER JOIN pay_billing_unit_rate_arrears  ON  pay_billing_unit_rate_history_arrears.comp_code = pay_billing_unit_rate_arrears.comp_code  AND pay_billing_unit_rate_history_arrears.unit_code = pay_billing_unit_rate_arrears.unit_code  AND pay_billing_unit_rate_history_arrears.month = pay_billing_unit_rate_arrears.month  AND pay_billing_unit_rate_history_arrears.year = pay_billing_unit_rate_arrears.year  AND pay_billing_unit_rate_history_arrears.grade_code = pay_billing_unit_rate_arrears.designation  INNER JOIN pay_billing_master_history_arrears  ON  pay_billing_master_history_arrears.comp_code = pay_billing_unit_rate_history_arrears.comp_code  AND pay_billing_master_history_arrears.billing_client_code = pay_billing_unit_rate_history_arrears.client_code  AND pay_billing_master_history_arrears.billing_unit_code = pay_billing_unit_rate_history_arrears.unit_code  AND pay_billing_master_history_arrears.month = pay_billing_unit_rate_history_arrears.month  AND pay_billing_master_history_arrears.year = pay_billing_unit_rate_history_arrears.year  AND pay_billing_master_history_arrears.designation = pay_billing_unit_rate_history_arrears.grade_code  AND pay_billing_master_history_arrears.hours = pay_billing_unit_rate_history_arrears.hours  AND pay_billing_master_history_arrears.type = 'billing'  " + where + " " + old_month_year + " union SELECT DISTINCT emp_code, 'old' as 'aa',pay_billing_unit_rate_history_arrears.month,pay_billing_unit_rate_history_arrears.year, client,  state_name,  branch_type,  unit_name,  pay_billing_unit_rate_history_arrears.comp_code,  emp_name,  grade_desc,  CONCAT(pay_billing_unit_rate_history_arrears.hours, ' HRS ', pay_billing_unit_rate_history_arrears.month_days, ' DAYS') AS 'DUTYHRS',  tot_days_present,  emp_basic_vda,  bonus_amount_billing,  pay_billing_unit_rate_history_arrears.washing,  pay_billing_unit_rate_history_arrears.travelling,  pay_billing_unit_rate_history_arrears.education,  IF(esic_oa_billing = 1, pay_billing_unit_rate_history_arrears.allowances, 0) AS 'allowances_esic',  cca_billing,  pay_billing_unit_rate_history_arrears.other_allow,  bonus_gross,  leave_gross,  gratuity_gross,  pay_billing_unit_rate_history_arrears.hra,  CASE WHEN pay_billing_master_history_arrears.ot_policy_billing = '1' THEN ((pay_billing_master_history_arrears.ot_amount_billing / pay_billing_unit_rate_history_arrears.month_days) * pay_billing_unit_rate_history_arrears.tot_days_present) ELSE 0 END AS 'special_allowance',  pay_billing_unit_rate_history_arrears.gross,  bonus_after_gross,  leave_after_gross,  gratuity_after_gross,  NH,  ifnull(pf,0) as 'pf',  ifnull(esic,0) as 'esic',  IF(bill_ser_uniform = 1, ((pay_billing_unit_rate_arrears.uniform / pay_billing_unit_rate_arrears.month_days) * pay_billing_unit_rate_history_arrears.tot_days_present), 0) AS 'uniform_ser',  pay_billing_unit_rate_history_arrears.group_insurance_billing,  pay_billing_unit_rate_history_arrears.lwf,  IF(bill_ser_operations = 1, ((pay_billing_unit_rate_arrears.operational_cost / pay_billing_unit_rate_arrears.month_days) * pay_billing_unit_rate_history_arrears.tot_days_present), 0) AS 'operational_cost',  IF(esic_oa_billing = 0, pay_billing_unit_rate_history_arrears.allowances, 0) AS 'allowances_no_esic',  ifnull(amount,0) AS 'sub_total_a',  IF((ot_rate - esi_on_ot_amount) > 0, (ot_rate - esi_on_ot_amount), 0) AS 'ot_rate',  pay_billing_unit_rate_history_arrears.esic_ot,  ot_hours,  ifnull(ot_rate,0) AS 'sub_total_b',  ifnull(amount,0) AS 'sub_total_ab',  ifnull(relieving_charg,0) as 'relieving_charg',  ifnull(amount,0) AS 'sub_total_c',  IF(bill_ser_uniform = 0, ((pay_billing_unit_rate_arrears.uniform / pay_billing_unit_rate_arrears.month_days) * pay_billing_unit_rate_history_arrears.tot_days_present), 0) AS 'uniform_no_ser',  IF(bill_ser_operations = 0, ((pay_billing_unit_rate_arrears.operational_cost / pay_billing_unit_rate_arrears.month_days) * pay_billing_unit_rate_history_arrears.tot_days_present), 0) AS 'operational_cost_no_ser', ifnull(Service_charge,0) as 'Service_charge',  ((ifnull(amount,0))  + IF(bill_ser_uniform = 0, ((pay_billing_unit_rate_arrears.uniform / pay_billing_unit_rate_arrears.month_days) * pay_billing_unit_rate_history_arrears.tot_days_present), 0)  + IF(bill_ser_operations = 0, ((pay_billing_unit_rate_arrears.operational_cost / pay_billing_unit_rate_arrears.month_days) * pay_billing_unit_rate_history_arrears.tot_days_present), 0)  + (ifnull(Service_charge,0))  + (ot_rate * ot_hours)) AS 'Amount',  pay_billing_master_history_arrears.bill_bonus_percent,  pay_billing_master_history_arrears.leave_days,  pay_billing_master_history_arrears.gratuity_percent,  pay_billing_master_history_arrears.hra_percent,  pay_billing_master_history_arrears.bill_pf_percent,  pay_billing_master_history_arrears.bill_esic_percent,  pay_billing_master_history_arrears.bill_service_charge,  pay_billing_master_history_arrears.basic,  pay_billing_master_history_arrears.vda,  pay_billing_unit_rate_arrears.bonus_rate,  pay_billing_master_history_arrears.group_insurance_billing AS 'group_insurance_billing_ser',  IF(ot_hours > 0, (ot_rate - esi_on_ot_amount), 0) AS 'ot_pr_hr_rate',  IF(ot_hours > 0 AND ot_rate > 0, esi_on_ot_amount, 0) AS 'esi_on_ot_amount',  (ot_rate * ot_hours) AS 'ot_amount', pay_billing_unit_rate_history_arrears.conveyance_amount  FROM  pay_billing_unit_rate_history_arrears  INNER JOIN pay_billing_unit_rate_arrears  ON  pay_billing_unit_rate_history_arrears.comp_code = pay_billing_unit_rate_arrears.comp_code  AND pay_billing_unit_rate_history_arrears.unit_code = pay_billing_unit_rate_arrears.unit_code  AND pay_billing_unit_rate_history_arrears.month = pay_billing_unit_rate_arrears.month  AND pay_billing_unit_rate_history_arrears.year = pay_billing_unit_rate_arrears.year  AND pay_billing_unit_rate_history_arrears.grade_code = pay_billing_unit_rate_arrears.designation  INNER JOIN pay_billing_master_history_arrears  ON  pay_billing_master_history_arrears.comp_code = pay_billing_unit_rate_history_arrears.comp_code  AND pay_billing_master_history_arrears.billing_client_code = pay_billing_unit_rate_history_arrears.client_code  AND pay_billing_master_history_arrears.billing_unit_code = pay_billing_unit_rate_history_arrears.unit_code  AND pay_billing_master_history_arrears.month = pay_billing_unit_rate_history_arrears.month  AND pay_billing_master_history_arrears.year = pay_billing_unit_rate_history_arrears.year  AND pay_billing_master_history_arrears.designation = pay_billing_unit_rate_history_arrears.grade_code  AND pay_billing_master_history_arrears.hours = pay_billing_unit_rate_history_arrears.hours  AND pay_billing_master_history_arrears.type = 'billing'  " + where + " " + new_month_year + multi + "  ORDER BY month,state_name, unit_name, emp_name";
                }
                else
                {
                    sql = "SELECT DISTINCT emp_code, 'old' as 'aa',pay_billing_unit_rate_history_arrears.month,pay_billing_unit_rate_history_arrears.year, client,  state_name,  branch_type,  unit_name,  pay_billing_unit_rate_history_arrears.comp_code,  emp_name,  grade_desc,  cast(CONCAT(pay_billing_unit_rate_history_arrears.hours, ' HRS ', pay_billing_unit_rate_history_arrears.month_days, ' DAYS') as char) AS 'DUTYHRS',  tot_days_present,  emp_basic_vda,  bonus_amount_billing,  pay_billing_unit_rate_history_arrears.washing,  pay_billing_unit_rate_history_arrears.travelling,  pay_billing_unit_rate_history_arrears.education,  IF(esic_oa_billing = 1, pay_billing_unit_rate_history_arrears.allowances, 0) AS 'allowances_esic',  cca_billing,  pay_billing_unit_rate_history_arrears.other_allow,  bonus_gross,  leave_gross,  gratuity_gross,  pay_billing_unit_rate_history_arrears.hra,  CASE WHEN pay_billing_master_history_arrears.ot_policy_billing = '1' THEN ((pay_billing_master_history_arrears.ot_amount_billing / pay_billing_unit_rate_history_arrears.month_days) * pay_billing_unit_rate_history_arrears.tot_days_present) ELSE 0 END AS 'special_allowance',  pay_billing_unit_rate_history_arrears.gross,  bonus_after_gross,  leave_after_gross,  gratuity_after_gross,  NH,  ifnull(pf,0) as 'pf',  ifnull(esic,0) as 'esic',   '0' AS 'uniform_ser',  pay_billing_unit_rate_history_arrears.group_insurance_billing,  pay_billing_unit_rate_history_arrears.lwf,   '0' AS 'operational_cost',  IF(esic_oa_billing = 0, pay_billing_unit_rate_history_arrears.allowances, 0) AS 'allowances_no_esic',  ifnull(amount,0) AS 'sub_total_a',  IF((ot_rate - esi_on_ot_amount) > 0, (ot_rate - esi_on_ot_amount), 0) AS 'ot_rate',  pay_billing_unit_rate_history_arrears.esic_ot,  ot_hours,  ifnull(ot_rate,0) AS 'sub_total_b',  ifnull(amount,0) AS 'sub_total_ab',  ifnull(relieving_charg,0) as 'relieving_charg',  ifnull(amount,0) AS 'sub_total_c',  '0' AS 'uniform_no_ser',  '0' AS 'operational_cost_no_ser', ifnull(Service_charge,0) as 'Service_charge',  ((ifnull(amount,0))  + (ifnull(Service_charge,0))  + (ot_rate * ot_hours)) AS 'Amount',  pay_billing_master_history_arrears.bill_bonus_percent,  pay_billing_master_history_arrears.leave_days,  pay_billing_master_history_arrears.gratuity_percent,  pay_billing_master_history_arrears.hra_percent,  pay_billing_master_history_arrears.bill_pf_percent,  pay_billing_master_history_arrears.bill_esic_percent,  pay_billing_master_history_arrears.bill_service_charge,  pay_billing_master_history_arrears.basic,  pay_billing_master_history_arrears.vda,  pay_billing_unit_rate_arrears.bonus_rate,  '0' AS 'group_insurance_billing_ser',  IF(ot_hours > 0, (ot_rate - esi_on_ot_amount), 0) AS 'ot_pr_hr_rate',  IF(ot_hours > 0 AND ot_rate > 0, esi_on_ot_amount, 0) AS 'esi_on_ot_amount',  (ot_rate * ot_hours) AS 'ot_amount', pay_billing_unit_rate_history_arrears.conveyance_amount  FROM  pay_billing_unit_rate_history_arrears  INNER JOIN pay_billing_unit_rate_arrears  ON  pay_billing_unit_rate_history_arrears.comp_code = pay_billing_unit_rate_arrears.comp_code  AND pay_billing_unit_rate_history_arrears.unit_code = pay_billing_unit_rate_arrears.unit_code  AND pay_billing_unit_rate_history_arrears.month = pay_billing_unit_rate_arrears.month  AND pay_billing_unit_rate_history_arrears.year = pay_billing_unit_rate_arrears.year  AND pay_billing_unit_rate_history_arrears.grade_code = pay_billing_unit_rate_arrears.designation  INNER JOIN pay_billing_master_history_arrears  ON  pay_billing_master_history_arrears.comp_code = pay_billing_unit_rate_history_arrears.comp_code  AND pay_billing_master_history_arrears.billing_client_code = pay_billing_unit_rate_history_arrears.client_code  AND pay_billing_master_history_arrears.billing_unit_code = pay_billing_unit_rate_history_arrears.unit_code  AND pay_billing_master_history_arrears.month = pay_billing_unit_rate_history_arrears.month  AND pay_billing_master_history_arrears.year = pay_billing_unit_rate_history_arrears.year  AND pay_billing_master_history_arrears.designation = pay_billing_unit_rate_history_arrears.grade_code  AND pay_billing_master_history_arrears.hours = pay_billing_unit_rate_history_arrears.hours  AND pay_billing_master_history_arrears.type = 'billing'  " + where + " " + old_month_year + multi + "   ORDER BY pay_billing_unit_rate_history_arrears.month,pay_billing_unit_rate_history_arrears.state_name, pay_billing_unit_rate_history_arrears.unit_name, pay_billing_unit_rate_history_arrears.emp_name";
                }

            }
            //Arrears finance copy
            else if (i == 7)
            {
                string rg_terms = "";
                string where1 = "", month_list = "", year = "", new_yera = "", new_month = "", old_month_year = "", new_month_year = "";
                string order_by_clause1 = "   ORDER BY state_name,unit_name,emp_name";
                if (ddl_arrears_type == "policy")
                {
                    new_month_year = "  month in (" + txt_arrear_month_year.Substring(3, 2) + ") and year in (" + txt_arrear_month_year.Substring(6) + ") ";
                    old_month_year = "  month in (" + txt_arrear_month_year.Substring(3, 2) + ") and year in (" + txt_arrear_month_year.Substring(6) + ") ";
                }
                else
                {
                    new_month_year = "  month in (" + txt_month_year.Text.Substring(0, 2) + ") and year in (" + txt_month_year.Text.Substring(3) + ") ";
                    old_month_year = "  month in (" + txt_month_year.Text.Substring(0, 2) + ") and year in (" + txt_month_year.Text.Substring(3) + ") ";
                }
                if (ddl_client.SelectedValue == "RCPL")
                {
                    rg_terms = "AND (emp_code != '' OR emp_code IS NOT NULL)";
                }
                string start_end_date = "AND (start_date = 0 AND end_date = 0) " + billing_type;
                if (ddl_start_date_common != "0" && ddl_end_date_common != "0")
                {
                    start_end_date = "AND (start_date = " + ddl_start_date_common + " AND end_date = " + ddl_end_date_common + ") " + billing_type;
                }
                where1 = " and comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' " + where_state_arrears + " and unit_code='" + ddl_unitcode + "'  " + grade + "  ";
                if (ddl_billing_state == "ALL")
                {
                    where1 = " and comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "'  " + where_state_arrears + " " + grade + " ";
                }
                else if (ddl_unitcode == "ALL")
                {
                    where1 = " and  comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state + "' " + where_state_arrears + "  " + grade + "   ";
                }
                string multi = "";
                if (type_cl == 1)
                {
                    multi = " and pay_billing_unit_rate_history_arrears.invoice_flag!=0 ";
                    if (ddl_billing_state.Equals("ALL") && state_name_arrear_state != "" && type_cl == 1)
                    {
                        multi = multi + " and pay_billing_unit_rate_history_arrears.state_name in (" + state_name_arrear_state.Substring(0, state_name_arrear_state.Length - 1) + ") ";
                    }
                }
                if (ddl_client.SelectedValue == "HDFC")
                {
                    where1 = multi + " and pay_billing_unit_rate_history_arrears.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history_arrears.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history_arrears.unit_code='" + ddl_unitcode + "'  " + grade + "   group by pay_billing_unit_rate_history_arrears.unit_code,pay_billing_unit_rate_history_arrears.GRADE_CODE  order by pay_billing_unit_rate_history_arrears.state_name,pay_billing_unit_rate_history_arrears.unit_name";
                    if (ddl_billing_state == "ALL")
                    {
                        where1 = multi + " and pay_billing_unit_rate_history_arrears.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history_arrears.client_code= '" + ddl_client.SelectedValue + "'   " + grade + "  group by pay_billing_unit_rate_history_arrears.unit_code,pay_billing_unit_rate_history_arrears.GRADE_CODE  order by pay_billing_unit_rate_history_arrears.state_name,pay_billing_unit_rate_history_arrears.unit_name";
                    }
                    else if (ddl_unitcode == "ALL")
                    {
                        where1 = multi + " and pay_billing_unit_rate_history_arrears.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history_arrears.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history_arrears.state_name = '" + ddl_billing_state + "'   " + grade + "   group by pay_billing_unit_rate_history_arrears.unit_code,pay_billing_unit_rate_history_arrears.GRADE_CODE order by pay_billing_unit_rate_history_arrears.state_name,pay_billing_unit_rate_history_arrears.unit_name";
                    }

                    // sql = "SELECT CASE WHEN invoice_flag != 0 AND month <= 3 AND year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND month >= 4 AND year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE '' END AS 'bill_invoice_no', client_code, client, state_name,branch_type, unit_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, SUM(hra) as 'hra', SUM(bonus_gross) as 'bonus_gross', SUM(leave_gross) as 'leave_gross', SUM(gratuity_gross) as 'gratuity_gross', SUM(washing) as 'washing', SUM(travelling) as 'travelling', SUM(education) as 'education', SUM(allowances) as 'allowances', SUM(cca_billing) as 'cca_billing', SUM(other_allow) as 'other_allow', SUM(gross) as 'gross', SUM(bonus_after_gross) as 'bonus_after_gross', SUM(leave_after_gross) as 'leave_after_gross', SUM(gratuity_after_gross) as 'gratuity_after_gross', SUM(pf) as 'pf', SUM(esic) as 'esic', SUM(hrs_12_ot) as 'hrs_12_ot' , SUM(esic_ot) as 'esic_ot', SUM(lwf) as 'lwf', SUM(uniform) as 'uniform', SUM(relieving_charg) as 'relieving_charg', SUM(operational_cost) as 'operational_cost', SUM(tot_days_present) as 'tot_days_present',sum(Amount) as 'Amount', SUM(Service_charge) as 'Service_charge', SUM(CGST9) as 'CGST9', SUM(IGST18) as 'IGST18', SUM(SGST9) as 'SGST9', bill_service_charge , NH, hours, fromtodate,sub_total_c, max(ot_rate) as 'ot_rate', SUM(ot_hours) as 'ot_hours', SUM(ot_amount) as 'ot_amount', group_insurance_billing, bill_service_charge_amount, txt_zone, adminhead_name, ihms, location_type, unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) as 'total_emp_count', sum(no_of_duties) as 'no_of_duties', zone, TOT_WORKING_DAYS, GRADE_CODE, month_days,material_area FROM pay_billing_unit_rate_history_arrears where  " + old_month_year + "" + where1 + "  ";
                    sql = "SELECT CASE WHEN invoice_flag != 0 AND month <= 3 AND year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND month >= 4 AND year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE auto_invoice_no END AS 'bill_invoice_no', pay_billing_unit_rate_history_arrears.client_code, client, pay_billing_unit_rate_history_arrears.state_name, pay_billing_unit_rate_history_arrears.branch_type, pay_billing_unit_rate_history_arrears.unit_name, pay_billing_unit_rate_history_arrears.unit_city, pay_billing_unit_rate_history_arrears.client_branch_code, emp_name, grade_desc, emp_basic_vda, SUM(hra) AS 'hra', SUM(bonus_gross) AS 'bonus_gross', SUM(leave_gross) AS 'leave_gross', SUM(gratuity_gross) AS 'gratuity_gross', SUM(washing) AS 'washing', SUM(travelling) AS 'travelling', SUM(education) AS 'education', SUM(allowances) AS 'allowances', SUM(cca_billing) AS 'cca_billing', SUM(other_allow) AS 'other_allow', SUM(gross) AS 'gross', SUM(bonus_after_gross) AS 'bonus_after_gross', SUM(leave_after_gross) AS 'leave_after_gross', SUM(gratuity_after_gross) AS 'gratuity_after_gross', SUM(pf) AS 'pf', SUM(esic) AS 'esic', SUM(hrs_12_ot) AS 'hrs_12_ot', SUM(esic_ot) AS 'esic_ot', SUM(lwf) AS 'lwf', SUM(uniform) AS 'uniform', SUM(relieving_charg) AS 'relieving_charg', SUM(operational_cost) AS 'operational_cost', SUM(tot_days_present) AS 'tot_days_present', SUM(Amount) AS 'Amount', SUM(Service_charge) AS 'Service_charge', SUM(CGST9) AS 'CGST9', SUM(IGST18) AS 'IGST18', SUM(SGST9) AS 'SGST9', bill_service_charge, NH, hours, fromtodate, (amount * month_days/tot_days_present) as 'sub_total_c', MAX(ot_rate) AS 'ot_rate', SUM(ot_hours) AS 'ot_hours', SUM(ot_amount) AS 'ot_amount', group_insurance_billing, bill_service_charge_amount, pay_billing_unit_rate_history_arrears.txt_zone, pay_billing_unit_rate_history_arrears.adminhead_name, ihms, pay_billing_unit_rate_history_arrears.location_type, pay_billing_unit_rate_history_arrears.unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, pay_billing_unit_rate_history_arrears.branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) AS 'total_emp_count', SUM(no_of_duties) AS 'no_of_duties', pay_billing_unit_rate_history_arrears.zone, TOT_WORKING_DAYS, GRADE_CODE, month_days,material_area,(SELECT field2 FROM pay_zone_master WHERE pay_zone_master.comp_code = pay_billing_unit_rate_history_arrears.comp_code AND pay_zone_master.CLIENT_CODE = pay_billing_unit_rate_history_arrears.CLIENT_CODE AND pay_zone_master.ZONE = pay_unit_master.txt_zone AND type = 'ZONE' AND field1 = 'admin') AS 'zonal_name' FROM pay_billing_unit_rate_history_arrears INNER JOIN pay_unit_master ON pay_billing_unit_rate_history_arrears.comp_code = pay_unit_master.comp_code AND pay_billing_unit_rate_history_arrears.unit_code = pay_unit_master.unit_code  where  " + old_month_year + "" + where1 + "  ";

                }
                else if (ddl_client.SelectedValue == "BAGICTM")
                {
                    where1 = multi + " and comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + ddl_unitcode + "'  " + grade + " ";
                    if (ddl_billing_state == "ALL")
                    {
                        where1 = multi + " and comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "'   " + grade + "  ";
                    }
                    else if (ddl_unitcode == "ALL")
                    {
                        where1 = multi + " and  comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state + "'   " + grade + "   ";
                    }

                    sql = "SELECT '' as 'txt_zone','' as 'zone',CASE WHEN invoice_flag != 0 AND month <= 3 AND year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND month >= 4 AND year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE auto_invoice_no END AS 'bill_invoice_no', client_code, client, state_name,branch_type, unit_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, SUM(hra) as 'hra', SUM(bonus_gross) as 'bonus_gross', SUM(leave_gross) as 'leave_gross', SUM(gratuity_gross) as 'gratuity_gross', SUM(washing) as 'washing', SUM(travelling) as 'travelling', SUM(education) as 'education', SUM(allowances) as 'allowances', SUM(cca_billing) as 'cca_billing', SUM(other_allow) as 'other_allow', SUM(gross) as 'gross', SUM(bonus_after_gross) as 'bonus_after_gross', SUM(leave_after_gross) as 'leave_after_gross', SUM(gratuity_after_gross) as 'gratuity_after_gross', SUM(pf) as 'pf', SUM(esic) as 'esic', SUM(hrs_12_ot) as 'hrs_12_ot' , SUM(esic_ot) as 'esic_ot', SUM(lwf) as 'lwf', SUM(uniform) as 'uniform', SUM(relieving_charg) as 'relieving_charg', SUM(operational_cost) as 'operational_cost', SUM(tot_days_present) as 'tot_days_present',ifnull(sum(Amount),0) as 'Amount', SUM(Service_charge) as 'Service_charge', ifnull(SUM(CGST9),0) as 'CGST9', ifnull(SUM(IGST18),0) as 'IGST18', ifnull(SUM(SGST9),0) as 'SGST9', bill_service_charge , NH, hours, fromtodate,(amount * month_days/tot_days_present) as 'sub_total_c', max(ot_rate) as 'ot_rate', SUM(ot_hours) as 'ot_hours', SUM(ot_amount) as 'ot_amount', group_insurance_billing, bill_service_charge_amount, txt_zone, adminhead_name, ihms, location_type, unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) as 'total_emp_count', sum(no_of_duties) as 'no_of_duties', zone, TOT_WORKING_DAYS, GRADE_CODE, month_days FROM pay_billing_unit_rate_history_arrears where " + old_month_year + "" + where1 + " group by unit_code  order by pay_billing_unit_rate_history_arrears.state_name,pay_billing_unit_rate_history_arrears.unit_name";


                }
                else
                {
                    sql = "SELECT '' as 'txt_zone','' as 'zone',CASE WHEN invoice_flag != 0 AND month <= 3 AND year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND  year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', client_code,case when client_code = 'BAGIC TM' then 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' else client end AS 'client',state_name,unit_name,unit_city,client_branch_code,emp_name,grade_desc,emp_basic_vda,hra,bonus_gross,leave_gross,gratuity_gross,washing,travelling,education,allowances,cca_billing,other_allow,gross,bonus_after_gross,leave_after_gross,gratuity_after_gross,ifnull( pf ,0) as 'pf',esic,hrs_12_ot,esic_ot,lwf,uniform,relieving_charg,operational_cost,tot_days_present,ifnull( amount ,0) as 'Amount', Service_charge as 'Service_charge',ifnull(CGST9,0) as 'CGST9',ifnull(IGST18,0) as 'IGST18',ifnull(SGST9,0) as 'SGST9',bill_service_charge,NH,hours,fromtodate,(amount * month_days/tot_days_present) as 'sub_total_c',ot_rate,ot_hours,ot_amount,group_insurance_billing,bill_service_charge_amount,bill_service_charge_amount,branch_type,month_days,gst_applicable,OPus_NO,unit_code,0 as 'yearly_bonus',0 as 'yearly_gratuity' from pay_billing_unit_rate_history_arrears where " + old_month_year + " " + where1 + multi + " " + order_by_clause1;
                }
            }
            else if (i == 8)
            {
                if (ddl_billing_state.Equals("ALL") && state_name_arrear_state != "" && type_cl == 1)
                {
                    grade = grade + " and pay_billing_unit_rate_history.state_name in (" + state_name_arrear_state.Substring(0, state_name_arrear_state.Length - 1) + ") ";
                }
                start_date_common = "1";
                month_days = DateTime.DaysInMonth(int.Parse(txt_arrear_month_year.Substring(6)), int.Parse(txt_arrear_month_year.Substring(3, 2)));
                order_by_clause = "   ORDER BY pay_billing_unit_rate_history_arrears.client,pay_billing_unit_rate_history_arrears.state_name,pay_billing_unit_rate_history_arrears.unit_name,pay_billing_unit_rate_history_arrears.emp_name";
                pay_attendance_muster = " pay_attendance_muster_arrears as pay_attendance_muster ";

                where = " pay_billing_unit_rate_history_arrears.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state_arrears + " and pay_billing_unit_rate_history_arrears.unit_code = '" + ddl_unitcode + "' and pay_billing_unit_rate_history_arrears.month = '" + txt_arrear_month_year.Substring(3, 2) + "' and pay_billing_unit_rate_history_arrears.Year = '" + txt_arrear_month_year.Substring(6) + "' and pay_billing_unit_rate_history_arrears.tot_days_present > 0  " + flag + "  " + grade;
                if (ddl_billing_state == "ALL")
                {
                    where = " pay_billing_unit_rate_history_arrears.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state_arrears + " and pay_billing_unit_rate_history_arrears.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history_arrears.month = '" + txt_arrear_month_year.Substring(3, 2) + "' and pay_billing_unit_rate_history_arrears.Year = '" + txt_arrear_month_year.Substring(6) + "' and pay_billing_unit_rate_history_arrears.tot_days_present > 0  " + flag + "  " + grade;
                }
                else if (ddl_unitcode == "ALL")
                {
                    where = "pay_billing_unit_rate_history_arrears.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state_arrears + " and pay_billing_unit_rate_history_arrears.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history_arrears.state_name = '" + ddl_billing_state + "' and pay_billing_unit_rate_history_arrears.month = '" + txt_arrear_month_year.Substring(3, 2) + "' and pay_billing_unit_rate_history_arrears.Year = '" + txt_arrear_month_year.Substring(6) + "' and pay_billing_unit_rate_history_arrears.tot_days_present > 0 " + flag + "  " + grade;
                }

                sql = "select pay_billing_unit_rate_history_arrears.client_code, pay_billing_unit_rate_history_arrears.zone, pay_billing_unit_rate_history_arrears.txt_zone, pay_billing_unit_rate_history_arrears.state_name, branch_type, pay_billing_unit_rate_history_arrears.unit_city,pay_billing_unit_rate_history_arrears.unit_name, pay_billing_unit_rate_history_arrears.client_branch_code, pay_billing_unit_rate_history_arrears.emp_name, pay_billing_unit_rate_history_arrears.grade_desc,pay_attendance_muster.ot_hours , case when DAY01 = '0' then 'A' else DAY01 end as DAY01, case when DAY02 = '0' then 'A' else DAY02 end as DAY02, case when DAY03 = '0' then 'A' else DAY03 end as DAY03, case when DAY04 = '0' then 'A' else DAY04 end as DAY04, case when DAY05 = '0' then 'A' else DAY05 end as DAY05, case when DAY06 = '0' then 'A' else DAY06 end as DAY06, case when DAY07 = '0' then 'A' else DAY07 end as DAY07, case when DAY08 = '0' then 'A' else DAY08 end as DAY08, case when DAY09 = '0' then 'A' else DAY09 end as DAY09, case when DAY10 = '0' then 'A' else DAY10 end as DAY10, case when DAY11 = '0' then 'A' else DAY11 end as DAY11, case when DAY12 = '0' then 'A' else DAY12 end as DAY12, case when DAY13 = '0' then 'A' else DAY13 end as DAY13, case when DAY14 = '0' then 'A' else DAY14 end as DAY14, case when DAY15 = '0' then 'A' else DAY15 end as DAY15, case when DAY16 = '0' then 'A' else DAY16 end as DAY16, case when DAY17 = '0' then 'A' else DAY17 end as DAY17, case when DAY18 = '0' then 'A' else DAY18 end as DAY18, case when DAY19 = '0' then 'A' else DAY19 end as DAY19, case when DAY20 = '0' then 'A' else DAY20 end as DAY20, case when DAY21 = '0' then 'A' else DAY21 end as DAY21, case when DAY22 = '0' then 'A' else DAY22 end as DAY22, case when DAY23 = '0' then 'A' else DAY23 end as DAY23, case when DAY24 = '0' then 'A' else DAY24 end as DAY24, case when DAY25 = '0' then 'A' else DAY25 end as DAY25, case when DAY26 = '0' then 'A' else DAY26 end as DAY26, case when DAY27 = '0' then 'A' else DAY27 end as DAY27, case when DAY28 = '0' then 'A' else DAY28 end as DAY28, case when DAY29 = '0' then 'A' else DAY29 end as DAY29, case when DAY30 = '0' then 'A' else DAY30 end as DAY30, case when DAY31 = '0' then 'A' else DAY31 end as DAY31, pay_attendance_muster.tot_days_present, CASE WHEN (pay_attendance_muster.tot_working_days - pay_attendance_muster.tot_days_present) < 0 THEN 0 ELSE pay_attendance_muster.tot_working_days - pay_attendance_muster.tot_days_present END AS 'absent',DAY(LAST_DAY('" + txt_month_year.Text.Substring(3) + "-" + txt_month_year.Text.Substring(0, 2) + "-1')) AS 'total days', IF(pay_employee_master.LEFT_DATE IS NULL, 'CONTINUE', 'LEFT') AS 'STATUS' from pay_billing_unit_rate_history_arrears INNER JOIN " + pay_attendance_muster + " ON pay_attendance_muster.emp_code = pay_billing_unit_rate_history_arrears.emp_code and pay_attendance_muster.comp_code = pay_billing_unit_rate_history_arrears.comp_code  and pay_attendance_muster.unit_code = pay_billing_unit_rate_history_arrears.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate_history_arrears.month AND pay_attendance_muster.year = pay_billing_unit_rate_history_arrears.year  INNER JOIN pay_employee_master ON pay_employee_master.COMP_CODE = pay_attendance_muster.COMP_CODE AND pay_employee_master.UNIT_CODE = pay_attendance_muster.UNIT_CODE AND pay_employee_master.EMP_CODE = pay_attendance_muster.EMP_CODE where " + where + " " + order_by_clause;
            }
            //OT Finance Copy
            else if (i == 9)
            {
                string start_end_date = "AND (start_date = 0 AND end_date = 0) " + billing_type;
                if (ddl_start_date_common != "0" && ddl_end_date_common != "0")
                {
                    start_end_date = "AND (start_date = " + ddl_start_date_common + " AND end_date = " + ddl_end_date_common + ") " + billing_type;
                }

                where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.unit_code='" + ddl_unitcode + "' " + billing_bfl + "  and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "' AND hdfc_type = 'ot_bill' " + grade + "     AND approve = 2  " + where_state + " " + start_end_date;
                if (ddl_billing_state == "ALL")
                {
                    where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "'  " + billing_bfl + "  and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "' AND hdfc_type = 'ot_bill'   AND approve = 2 " + where_state + " " + start_end_date;
                }
                else if (ddl_unitcode == "ALL")
                {
                    where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.state_name = '" + ddl_billing_state + "'  " + billing_bfl + "   and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "' AND hdfc_type = 'ot_bill' " + grade + "   AND approve = 2  " + where_state + " " + start_end_date;
                }
                if (ddl_client.SelectedValue == "HDFC")
                {
                    where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.unit_code='" + ddl_unitcode + "' and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "' " + grade + " AND hdfc_type = 'ot_bill' AND approve = 2 " + start_end_date + "  group by pay_billing_unit_rate_history.unit_code,grade_desc  order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
                    if (ddl_billing_state == "ALL")
                    {
                        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "' " + grade + " AND hdfc_type = 'ot_bill' AND approve = 2 " + start_end_date + " group by pay_billing_unit_rate_history.unit_code,grade_desc order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
                    }
                    else if (ddl_unitcode == "ALL")
                    {
                        where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.state_name = '" + ddl_billing_state + "'  and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "' " + grade + " AND hdfc_type = 'ot_bill'  AND approve = 2 " + start_end_date + " group by pay_billing_unit_rate_history.unit_code,grade_desc order by pay_billing_unit_rate_history.state_name,pay_billing_unit_rate_history.unit_name";
                    }

                    sql = "SELECT  CASE WHEN pay_billing_unit_rate_history.invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', pay_billing_unit_rate_history.client_code, client, pay_billing_unit_rate_history.state_name, pay_billing_unit_rate_history.branch_type, pay_billing_unit_rate_history.unit_name, pay_billing_unit_rate_history.unit_city, pay_billing_unit_rate_history.client_branch_code, emp_name, grade_desc, SUM(tot_days_present) AS 'tot_days_present', SUM(Amount) AS 'Amount', SUM(Service_charge) AS 'Service_charge', SUM(CGST9) AS 'CGST9', SUM(IGST18) AS 'IGST18', SUM(SGST9) AS 'SGST9', bill_service_charge, hours, fromtodate, MAX(ot_rate) AS 'ot_rate', SUM(ot_hours) AS 'ot_hours', SUM(ot_amount) AS 'ot_amount', bill_service_charge_amount, pay_billing_unit_rate_history.txt_zone, pay_billing_unit_rate_history.adminhead_name, ihms, pay_billing_unit_rate_history.location_type, pay_billing_unit_rate_history.unit_add1, emp_count2 AS 'emp_count', emp_count1, state_per, pay_billing_unit_rate_history.branch_cost_centre_code, SUM(IF(EMP_TYPE = 'Permanent', 1, 0)) AS 'total_emp_count', SUM(no_of_duties) AS 'no_of_duties', pay_billing_unit_rate_history.zone, TOT_WORKING_DAYS, GRADE_CODE, month_days, material_area,(SELECT  field2 FROM pay_zone_master WHERE pay_zone_master.comp_code = pay_billing_unit_rate_history.comp_code AND pay_zone_master.CLIENT_CODE = pay_billing_unit_rate_history.CLIENT_CODE AND pay_zone_master.ZONE = pay_unit_master.txt_zone AND type = 'ZONE' AND field1 = 'admin') AS 'zonal_name' FROM pay_billing_unit_rate_history INNER JOIN pay_unit_master ON pay_billing_unit_rate_history.comp_code = pay_unit_master.comp_code AND pay_billing_unit_rate_history.unit_code = pay_unit_master.unit_code INNER JOIN pay_ot_upload ON pay_ot_upload.comp_code = pay_billing_unit_rate_history.comp_code AND pay_ot_upload.unit_code = pay_billing_unit_rate_history.unit_code AND pay_ot_upload.month = pay_billing_unit_rate_history.month AND pay_ot_upload.year = pay_billing_unit_rate_history.year " + where;
                }
                else
                {
                    sql = "SELECT txt_zone,zone,CASE WHEN pay_billing_unit_rate_history.invoice_flag != 0 AND pay_billing_unit_rate_history.month <= 3 AND pay_billing_unit_rate_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN pay_billing_unit_rate_history.invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_unit_rate_history.client_code,case when pay_billing_unit_rate_history.client_code = 'BAGIC TM' then 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' else client end AS 'client',state_name,unit_name,unit_city, if(pay_billing_unit_rate_history.client_code ='4' ,branch_cost_centre_code,client_branch_code) as 'client_branch_code',emp_name,grade_desc,emp_basic_vda,hra,bonus_gross,leave_gross,gratuity_gross,washing,travelling,education,allowances,cca_billing,other_allow,gross,bonus_after_gross,leave_after_gross,gratuity_after_gross,pf,esic,hrs_12_ot,esic_ot,lwf,uniform,relieving_charg,operational_cost,tot_days_present,amount as 'Amount',Service_charge,CGST9,IGST18,SGST9,bill_service_charge,NH,hours,fromtodate,(amount * month_days/tot_days_present) as 'sub_total_c',round(ot_rate,2) as 'ot_rate',ot_hours,round(ot_amount,2) as 'ot_amount',group_insurance_billing,bill_service_charge_amount,bill_service_charge_amount,branch_type,month_days,gst_applicable,OPus_NO,pay_billing_unit_rate_history.unit_code,yearly_bonus,yearly_gratuity from pay_billing_unit_rate_history INNER JOIN pay_ot_upload ON pay_ot_upload.comp_code = pay_billing_unit_rate_history.comp_code AND pay_ot_upload.unit_code = pay_billing_unit_rate_history.unit_code AND pay_ot_upload.month = pay_billing_unit_rate_history.month AND pay_ot_upload.year = pay_billing_unit_rate_history.year " + where + " " + order_by_clause;
                }

            }
            //OT Rate breakup
            else if (i == 10)
            {
                grade = grade + " " + from_to_date;
                if (ddl_client.SelectedValue != "ALL")
                {
                    where = " WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history . state_name  = '" + ddl_billing_state + "' and pay_billing_unit_rate_history.unit_code = '" + ddl_unitcode + "' and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' AND pay_billing_unit_rate_history.ot_hours != 0 AND hdfc_type = 'ot_bill' AND approve != 0 " + where_state + grade;
                }
                if (ddl_billing_state == "ALL")
                {
                    where = " WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "'  and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' AND pay_billing_unit_rate_history.ot_hours != 0 AND hdfc_type = 'ot_bill' AND approve != 0 " + where_state + grade;
                }
                else if (ddl_unitcode == "ALL")
                {
                    where = " WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history . state_name  = '" + ddl_billing_state + "'  and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' AND pay_billing_unit_rate_history.ot_hours != 0 AND hdfc_type = 'ot_bill' AND approve != 0 " + where_state + grade;
                }
                if (ddl_client.SelectedValue == "ALL")
                {
                    where = " WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  and pay_billing_unit_rate_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.Year = '" + txt_month_year.Text.Substring(3) + "' AND pay_billing_unit_rate_history.ot_hours != 0 AND hdfc_type = 'ot_bill' AND approve = !=0 " + where_state + grade;
                }
                //sql = "SELECT  state_name ,  unit_name ,  unit_city ,  emp_name ,  grade_desc ,DUTYHRS ,  tot_days_present ,  basic ,  vda ,  emp_basic_vda ,  bonus_rate ,  washing ,  travelling ,  education ,  allowances_esic ,  cca_billing ,  other_allow ,  bonus_gross ,  leave_gross ,  gratuity_gross ,  hra ,  special_allowance ,  gross ,  bonus_after_gross ,  leave_after_gross ,  gratuity_after_gross ,  NH ,  pf ,  esic ,  uniform_ser ,  group_insurance_billing ,  lwf ,  operational_cost ,  allowances_no_esic , ( gross  +  bonus_after_gross  +  leave_after_gross  +  gratuity_after_gross  +  NH  +  pf  +  esic  +  lwf  +  uniform_ser  +  operational_cost  +  allowances_no_esic ) AS 'sub_total_a',  ot_pr_hr_rate ,  esi_on_ot_amount ,  ot_hours , ( ot_pr_hr_rate  +  esi_on_ot_amount ) AS 'sub_total_b', ( gross  +  bonus_after_gross  +  leave_after_gross  +  gratuity_after_gross  +  NH  +  pf  +  esic  +  lwf  +  uniform_ser  +  operational_cost  +  allowances_no_esic  +  ot_pr_hr_rate  +  esi_on_ot_amount ) AS 'sub_total_ab',  relieving_charg , CASE WHEN  emp_cca  = 0 AND  branch_cca  != 0 THEN ((baseamount-bill_ot_rate)) WHEN  emp_cca  != 0 AND  branch_cca  != 0 THEN ((baseamount-bill_ot_rate)) WHEN  emp_cca  = 0 AND  branch_cca  = 0 THEN ((baseamount-bill_ot_rate)) ELSE ( bill_gross  + (( bill_gross  *  esic_percent ) / 100) +  bill_pf +lwf +  bill_uniform  +  group_insurance_billing_ser  +  bonus_after_gross  +  leave_after_gross  +  gratuity_after_gross ) END AS 'sub_total_c',  uniform_no_ser ,  operational_cost_no_ser , IF(((( Total  +  pf  +  esic  + ( ot_rate  *  ot_hours ) +  group_insurance_billing_ser ) *  bill_service_charge ) / 100) = 0,  bill_service_charge_amount , ((( Total  +  pf  +  esic  + ( ot_rate  *  ot_hours ) +  group_insurance_billing_ser ) *  bill_service_charge ) / 100)) AS 'Service_charge', (( Total  + ( ot_rate  *  ot_hours ) +  pf  +  esic  +  group_insurance_billing_ser  +  uniform_no_ser  +  operational_cost_no_ser  +  group_insurance_billing ) + IF(((( Total  +  pf  +  esic  + ( ot_rate  *  ot_hours ) +  group_insurance_billing_ser ) *  bill_service_charge ) / 100) = 0,  bill_service_charge_amount , ((( Total  +  pf  +  esic  + ( ot_rate  *  ot_hours ) +  group_insurance_billing_ser ) *  bill_service_charge ) / 100))) AS 'Amount',  pf_percent  AS 'bill_pf_percent',  esic_percent  AS 'bill_esic_percent',  gratuity_percent ,  hra_percent ,  bill_bonus_percent ,  leave_days ,  bill_service_charge,group_insurance_billing_ser,(ot_rate * ot_hours) as 'ot_amount'  FROM (SELECT  client ,  company_state ,  unit_name ,  state_name ,  unit_city ,  client_branch_code ,  emp_name ,  grade_desc ,  emp_basic_vda ,  hra ,  bonus_gross ,  leave_gross ,  gratuity_gross ,  washing ,  travelling ,  education ,  cca_billing ,  other_allow , ( emp_basic_vda  +  hra  +  bonus_gross  +  leave_gross  +  washing  +  travelling  +  education  +  allowances  +  cca_billing  +  other_allow  +  gratuity_gross  +  hrs_12_ot ) AS 'gross',  bonus_after_gross ,  leave_after_gross ,  gratuity_after_gross , ((( emp_basic_vda ) / 100) *  pf_percent ) AS 'pf', ((( emp_basic_vda  +  hra  +  bonus_gross  +  leave_gross  +  washing  +  travelling  +  education  + IF( esic_oa_billing  = 1,  allowances , 0) +  cca_billing  +  other_allow  +  gratuity_gross  +  hrs_12_ot ) / 100) *  esic_percent ) AS 'esic',  hrs_12_ot  AS 'special_allowance', ((( hrs_12_ot ) *  esic_percent ) / 100) AS 'esic_ot',  lwf , CASE WHEN  bill_ser_uniform  = 1 THEN  uniform  ELSE 0 END AS 'uniform_ser', CASE WHEN  bill_ser_uniform  = 0 THEN  uniform  ELSE 0 END AS 'uniform_no_ser',  relieving_charg , CASE WHEN  bill_ser_operations  = 1 THEN  operational_cost  ELSE 0 END AS 'operational_cost', CASE WHEN  bill_ser_operations  = 0 THEN  operational_cost  ELSE 0 END AS 'operational_cost_no_ser',  tot_days_present , ( emp_basic_vda  +  hra  +  bonus_gross  +  leave_gross  +  washing  +  travelling  +  education  +  allowances  +  cca_billing  +  other_allow  +  gratuity_gross  +  bonus_after_gross  +  leave_after_gross  +  gratuity_after_gross  +  lwf + CASE WHEN  bill_ser_uniform  = 0 THEN 0 ELSE  uniform  END +  relieving_charg  + CASE WHEN  bill_ser_operations  = 0 THEN 0 ELSE  operational_cost  END +  NH  +  hrs_12_ot  + IF( esic_common_allow  = 0,  common_allow , 0)) AS 'Total',  bill_service_charge ,  NH ,  hours , ( bill_gross ) AS 'bill_gross',  sub_total_c ,  bill_ser_uniform ,  bill_ser_operations , (IF(ot_hours > 0,ot_rate,0) + IF(ot_hours > 0 and ot_rate > 0,esi_on_ot_amount,0)) AS 'ot_rate',(ot_rate+esi_on_ot_amount) as 'bill_ot_rate',  ot_hours ,  esic_amount ,  IF(ot_hours > 0,ot_rate,0) AS 'ot_pr_hr_rate',IF(ot_hours > 0 and ot_rate > 0,esi_on_ot_amount,0) as 'esi_on_ot_amount',  emp_cca ,  branch_cca ,  bill_pf ,  bill_uniform , CASE WHEN  service_group_insurance_billing  = 0 THEN  group_insurance_billing  ELSE 0 END AS 'group_insurance_billing', CASE WHEN  service_group_insurance_billing  = 1 THEN  group_insurance_billing  ELSE 0 END AS 'group_insurance_billing_ser',  bill_service_charge_amount ,  branch_type ,  DUTYHRS ,  basic ,  vda ,  bonus_rate , IF( esic_oa_billing  = 1,  allowances , 0) AS 'allowances_esic', IF( esic_oa_billing  = 0,  allowances , 0) AS 'allowances_no_esic',  baseamount ,  pf_percent ,  esic_percent ,  gratuity_percent ,  hra_percent ,  bill_bonus_percent ,  leave_days  FROM (SELECT (SELECT  client_name  FROM  pay_client_master  WHERE  client_code  =  pay_unit_master . client_code  AND  comp_code  =  pay_unit_master . comp_code ) AS 'client',  pay_company_master . state  AS 'company_state',  pay_unit_master . unit_name ,  pay_unit_master . state_name ,  pay_unit_master . unit_city ,  pay_unit_master . client_branch_code ,  pay_employee_master . emp_name ,  pay_grade_master . grade_desc ,  pay_billing_unit_rate . basic ,  pay_billing_unit_rate . vda ,  pay_billing_unit_rate . bonus_rate , CAST(CONCAT( pay_billing_master_history . hours , 'HRS ',  pay_billing_unit_rate . month_days , ' DAYS ') AS char) AS 'DUTYHRS', ((( pay_billing_master_history . basic  +  pay_billing_master_history . vda ) /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'emp_basic_vda', (( pay_billing_unit_rate . hra  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'hra', CASE WHEN  bonus_taxable  = '1' THEN (( pay_billing_unit_rate . bonus_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'bonus_gross', CASE WHEN  bonus_taxable  = '0' THEN (( pay_billing_unit_rate . bonus_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'bonus_after_gross', CASE WHEN  leave_taxable  = '1' THEN (( pay_billing_unit_rate . leave_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'leave_gross', CASE WHEN  leave_taxable  = '0' THEN (( pay_billing_unit_rate . leave_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'leave_after_gross', CASE WHEN  gratuity_taxable  = '1' THEN (( pay_billing_unit_rate . grauity_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'gratuity_gross', CASE WHEN  gratuity_taxable  = '0' THEN (( pay_billing_unit_rate . grauity_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'gratuity_after_gross', (( pay_billing_unit_rate . washing  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'washing', (( pay_billing_unit_rate . traveling  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'travelling', (( pay_billing_unit_rate . education  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'education', (( pay_billing_unit_rate . national_holiday_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'NH', (( pay_billing_unit_rate . allowances  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'allowances', CASE WHEN  pay_employee_master . cca  = 0 THEN (( pay_billing_unit_rate . cca  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE (( pay_employee_master . cca  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) END AS 'cca_billing', CASE WHEN  pay_employee_master . special_allow  = 0 THEN (( pay_billing_unit_rate . otherallowance  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE (( pay_employee_master . special_allow  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) END AS 'other_allow', CASE WHEN  pay_billing_master_history . ot_policy_billing  = '1' THEN (( pay_billing_master_history . ot_amount_billing  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE 0 END AS 'hrs_12_ot',  pay_billing_master_history . bill_esic_percent  AS 'esic_percent',  pay_billing_master_history . bill_pf_percent  AS 'pf_percent',  gratuity_percent ,  pay_billing_master_history . hra_percent ,  pay_billing_master_history . bill_bonus_percent ,  pay_billing_master_history . leave_days , (( pay_billing_unit_rate . lwf  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'lwf', (( pay_billing_unit_rate . uniform  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'uniform', (( pay_billing_unit_rate . relieving_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'relieving_charg', (( pay_billing_unit_rate . operational_cost  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'operational_cost',  pay_attendance_muster . tot_days_present , ROUND((( pay_billing_unit_rate . sub_total_c  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ), 2) AS 'baseamount',  bill_service_charge ,  pay_billing_master_history . hours ,  pay_billing_unit_rate . sub_total_c ,  pay_billing_master_history . bill_ser_operations ,  pay_billing_master_history . bill_ser_uniform , pay_billing_unit_rate.ot_1_hr_amount AS 'ot_rate',  pay_attendance_muster . ot_hours ,  pay_billing_unit_rate . esic_amount ,  pay_billing_unit_rate.esi_on_ot_amount as 'esi_on_ot_amount',  pay_employee_master . cca  AS 'emp_cca',  pay_billing_unit_rate . cca  AS 'branch_cca', ( pay_billing_unit_rate . gross  + (( pay_employee_master . cca  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present )) AS 'bill_gross',  pay_billing_unit_rate . pf_amount  AS 'bill_pf',  pay_billing_unit_rate . uniform  AS 'bill_uniform', (( pay_billing_master_history . group_insurance_billing  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'group_insurance_billing',  service_group_insurance_billing ,  pay_employee_master . Employee_type , (( bill_service_charge_amount  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) AS 'bill_service_charge_amount',  pay_billing_master_history . esic_common_allow , CASE WHEN  pay_employee_master . special_allow  = 0 THEN (( pay_billing_unit_rate . common_allowance  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) ELSE (( pay_employee_master . special_allow  /  pay_billing_unit_rate . month_days ) *  pay_attendance_muster . tot_days_present ) END AS 'common_allow', IFNULL( branch_type , 0) AS 'branch_type',  pay_billing_master_history . esic_oa_billing  FROM pay_employee_master INNER JOIN " + pay_attendance_muster + " ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_muster.unit_code = pay_unit_master.unit_code AND pay_attendance_muster.comp_code = pay_unit_master.comp_code INNER JOIN pay_billing_unit_rate ON pay_attendance_muster.unit_code = pay_billing_unit_rate.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate.month AND pay_attendance_muster.year = pay_billing_unit_rate.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_master_history.comp_code = pay_employee_master.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_employee_master.grade_code = pay_billing_master_history.designation AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code INNER JOIN pay_grade_master ON pay_billing_master_history.comp_code = pay_grade_master.comp_code AND pay_billing_master_history.designation = pay_grade_master.GRADE_CODE WHERE  " + where;
                sql = "SELECT DISTINCT (emp_code), pay_billing_unit_rate_history.zone, pay_billing_unit_rate_history.txt_zone, client, state_name, branch_type, unit_name, pay_billing_unit_rate_history.comp_code, emp_name, grade_desc, CAST(CONCAT(pay_billing_unit_rate_history.hours, ' HRS ', pay_billing_unit_rate_history.month_days, ' DAYS') AS CHAR) AS 'DUTYHRS', tot_days_present, IF((ot_rate - esi_on_ot_amount) > 0, (ot_rate - esi_on_ot_amount), 0) AS 'ot_rate', IF(ot_hours > 0 AND ot_rate > 0, esi_on_ot_amount, 0) AS 'esi_on_ot_amount', pay_billing_unit_rate_history.esic_ot, ot_hours, Service_charge, Amount, pay_billing_master_history.bill_service_charge, round(ot_rate * ot_hours,2) AS 'ot_amount' FROM pay_billing_unit_rate_history INNER JOIN pay_billing_unit_rate ON pay_billing_unit_rate_history.comp_code = pay_billing_unit_rate.comp_code AND pay_billing_unit_rate_history.unit_code = pay_billing_unit_rate.unit_code AND pay_billing_unit_rate_history.month = pay_billing_unit_rate.month AND pay_billing_unit_rate_history.year = pay_billing_unit_rate.year AND pay_billing_unit_rate_history.grade_code = pay_billing_unit_rate.designation INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate_history.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate_history.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate_history.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate_history.month AND pay_billing_master_history.year = pay_billing_unit_rate_history.year AND pay_billing_master_history.designation = pay_billing_unit_rate_history.grade_code AND pay_billing_master_history.hours = pay_billing_unit_rate_history.hours AND pay_billing_master_history.type = 'billing' INNER JOIN pay_ot_upload ON pay_ot_upload.comp_code = pay_billing_unit_rate_history.comp_code AND pay_ot_upload.unit_code = pay_billing_unit_rate_history.unit_code AND pay_ot_upload.month = pay_billing_unit_rate_history.month AND pay_ot_upload.year = pay_billing_unit_rate_history.year  " + where + "  group by emp_code " + order_by_clause;
            }
            //R&M finance copy
            else if (i == 11)
            {

                string start_end_date = "AND (start_date = 0 AND end_date = 0) ";
                if (ddl_start_date_common != "0" && ddl_end_date_common != "0")
                {
                    start_end_date = "AND (start_date = " + ddl_start_date_common + " AND end_date = " + ddl_end_date_common + ") ";
                }

                where = "where pay_billing_r_m.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_r_m.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_r_m.unit_code='" + ddl_unitcode + "' and pay_billing_r_m.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_r_m.year = '" + txt_month_year.Text.Substring(3) + "' and pay_billing_r_m.invoice_slot = '" + ddl_invoice_slot + "' and (approve_flag =1 || approve_flag =2)" + start_end_date;
                if (ddl_billing_state == "ALL")
                {
                    where = "where pay_billing_r_m.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_r_m.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_r_m.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_r_m.year = '" + txt_month_year.Text.Substring(3) + "' and pay_billing_r_m.invoice_slot = '" + ddl_invoice_slot + "' and (approve_flag =1 || approve_flag =2) " + start_end_date;
                }
                else if (ddl_unitcode == "ALL")
                {
                    where = "where pay_billing_r_m.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_r_m.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_r_m.state_name = '" + ddl_billing_state + "'  and pay_billing_r_m.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_r_m.year = '" + txt_month_year.Text.Substring(3) + "' and pay_billing_r_m.invoice_slot = '" + ddl_invoice_slot + "' and (approve_flag =1 || approve_flag =2) " + start_end_date;
                }

                where = "where auto_invoice_no='" + auto_inv_no + "' and (approve_flag =1 || approve_flag =2) " + start_end_date;

                sql = "SELECT  txt_zone, zone, auto_invoice_no AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_r_m.client_code, CASE WHEN pay_billing_r_m.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE pay_billing_r_m.client END AS 'client', pay_billing_r_m.state_name, pay_billing_r_m.unit_name, pay_billing_r_m.unit_city, pay_billing_r_m.client_branch_code, pay_billing_r_m.emp_name, help_req_number, pay_billing_r_m.amount AS 'Amount', Service_charge, CGST9, IGST18, SGST9, bill_service_charge, fromtodate, bill_service_charge_amount, branch_type, gst_applicable, pay_billing_r_m.unit_code, utr_number,DATE_FORMAT(payment_date,'%d/%m/%Y') as 'payment_date'FROM pay_billing_r_m INNER JOIN pay_r_and_m_service ON pay_r_and_m_service.comp_code = pay_billing_r_m.comp_code AND pay_r_and_m_service.client_code = pay_billing_r_m.client_code AND pay_r_and_m_service.unit_code = pay_billing_r_m.unit_code AND pay_r_and_m_service.month = pay_billing_r_m.month AND pay_r_and_m_service.year = pay_billing_r_m.year AND pay_r_and_m_service.EMP_CODE = pay_billing_r_m.EMP_CODE left outer JOIN pay_pro_r_m ON pay_pro_r_m.comp_code = pay_billing_r_m.comp_code AND pay_pro_r_m.client_code = pay_billing_r_m.client_code AND pay_pro_r_m.unit_code = pay_billing_r_m.unit_code AND pay_pro_r_m.month = pay_billing_r_m.month AND pay_pro_r_m.year = pay_billing_r_m.year AND pay_pro_r_m.EMP_CODE = pay_billing_r_m.EMP_CODE " + where + " group by pay_billing_r_m.id " + R_M_order_by_clause + "";

            }
            //Administrative Expense finance copy
            else if (i == 12)
            {

                string start_end_date = "AND (start_date = 0 AND end_date = 0) ";
                if (ddl_start_date_common != "0" && ddl_end_date_common != "0")
                {
                    start_end_date = "AND (start_date = " + ddl_start_date_common + " AND end_date = " + ddl_end_date_common + ") ";
                }

                where = "where pay_billing_admini_expense.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_admini_expense.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_admini_expense.unit_code='" + ddl_unitcode + "' and pay_billing_admini_expense.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_admini_expense.year = '" + txt_month_year.Text.Substring(3) + "' AND (approve_flag = 1 || approve_flag = 2) " + start_end_date;

                if (ddl_billing_state == "ALL")
                {
                    where = "where pay_billing_admini_expense.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_admini_expense.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_admini_expense.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_admini_expense.year = '" + txt_month_year.Text.Substring(3) + "' AND (approve_flag = 1 || approve_flag = 2)  " + start_end_date;
                }
                else if (ddl_unitcode == "ALL" || ddl_billing_state != "ALL")
                {
                    where = "where pay_billing_admini_expense.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_admini_expense.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_admini_expense.state_name = '" + ddl_billing_state + "'  and pay_billing_admini_expense.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_admini_expense.year = '" + txt_month_year.Text.Substring(3) + "' AND (approve_flag = 1 || approve_flag = 2) " + start_end_date;
                }

                where = "where auto_invoice_no='" + auto_inv_no + "'  AND (approve_flag = 1 || approve_flag = 2) " + start_end_date;

                sql = "SELECT  txt_zone, zone, auto_invoice_no AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_admini_expense.client_code, CASE WHEN pay_billing_admini_expense.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE pay_billing_admini_expense.client END AS 'client', pay_billing_admini_expense.state_name, pay_billing_admini_expense.unit_name, pay_billing_admini_expense.unit_city, pay_billing_admini_expense.client_branch_code, pay_billing_admini_expense.emp_name, pay_billing_admini_expense.amount AS 'Amount', Service_charge, CGST9, IGST18, SGST9, bill_service_charge, fromtodate, bill_service_charge_amount, branch_type, gst_applicable, pay_billing_admini_expense.unit_code, pay_billing_admini_expense.days, utr_number FROM pay_billing_admini_expense INNER JOIN pay_administrative_expense ON pay_administrative_expense.comp_code = pay_billing_admini_expense.comp_code AND pay_administrative_expense.client_code = pay_billing_admini_expense.client_code AND pay_administrative_expense.unit_code = pay_billing_admini_expense.unit_code AND pay_administrative_expense.month = pay_billing_admini_expense.month AND pay_administrative_expense.year = pay_billing_admini_expense.year AND pay_administrative_expense.party_name = pay_billing_admini_expense.emp_name LEFT OUTER JOIN pay_pro_admini_expense ON pay_pro_admini_expense.comp_code = pay_billing_admini_expense.comp_code AND pay_pro_admini_expense.client_code = pay_billing_admini_expense.client_code AND pay_pro_admini_expense.unit_code = pay_billing_admini_expense.unit_code AND pay_pro_admini_expense.month = pay_billing_admini_expense.month AND pay_pro_admini_expense.year = pay_billing_admini_expense.year AND pay_pro_admini_expense.emp_code = pay_billing_admini_expense.emp_code " + where + " group by pay_administrative_expense.id " + R_M_order_by_clause + "";

            }
            //OT Attendance Sheet
            else if (i == 13)
            {

                where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_unit_rate_history.unit_code='" + ddl_unitcode + "'  and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "'   and approve !=0  and pay_billing_unit_rate_history.hdfc_type = 'ot_bill'";
                string where_region = "";
                if (ddl_billing_state == "ALL")
                {
                    if (ddlregion.SelectedValue != "Select" && ddlregion.SelectedValue != "ALL")
                    {
                        where_region = " AND pay_billing_unit_rate_history.zone='" + ddlregion.SelectedValue + "' ";
                    }

                    where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "'  and approve !=0 and pay_billing_unit_rate_history.hdfc_type = 'ot_bill' " + where_region + "";
                }
                else if (ddl_unitcode == "ALL")
                {
                    if (ddlregion.SelectedValue != "Select" && ddlregion.SelectedValue != "ALL")
                    {
                        where_region = " AND pay_billing_unit_rate_history.zone='" + ddlregion.SelectedValue + "' ";
                    }
                    where = "where pay_billing_unit_rate_history.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state + "'   and pay_billing_unit_rate_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_unit_rate_history.year = '" + txt_month_year.Text.Substring(3) + "'  and approve !=0  and pay_billing_unit_rate_history.hdfc_type = 'ot_bill'   " + where_region + "";
                }

                sql = "SELECT  pay_billing_unit_rate_history.client_code, pay_billing_unit_rate_history.zone, pay_billing_unit_rate_history.txt_zone, pay_billing_unit_rate_history.state_name, branch_type, pay_billing_unit_rate_history.unit_city, pay_billing_unit_rate_history.unit_name, pay_billing_unit_rate_history.client_branch_code, pay_billing_unit_rate_history.emp_name, pay_billing_unit_rate_history.grade_desc, pay_attendance_muster.ot_hours, IF(pay_employee_master.LEFT_DATE IS NULL, 'CONTINUE', 'LEFT') AS 'STATUS' FROM pay_billing_unit_rate_history INNER JOIN pay_attendance_muster ON pay_attendance_muster.emp_code = pay_billing_unit_rate_history.emp_code AND pay_attendance_muster.comp_code = pay_billing_unit_rate_history.comp_code AND pay_attendance_muster.unit_code = pay_billing_unit_rate_history.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate_history.month AND pay_attendance_muster.year = pay_billing_unit_rate_history.year INNER JOIN pay_ot_upload ON pay_ot_upload.comp_code = pay_attendance_muster.comp_code AND pay_ot_upload.unit_code = pay_attendance_muster.unit_code AND pay_ot_upload.month = pay_attendance_muster.month AND pay_ot_upload.year = pay_attendance_muster.year INNER JOIN pay_employee_master ON pay_employee_master.COMP_CODE = pay_attendance_muster.COMP_CODE AND pay_employee_master.UNIT_CODE = pay_attendance_muster.UNIT_CODE AND pay_employee_master.EMP_CODE = pay_attendance_muster.EMP_CODE " + where + " group by pay_employee_master.emp_code" + order_by_clause;


            }
            // Shiftwise finance copy
            else if (i == 14)
            {

                string start_end_date = "AND (start_date = 0 AND end_date = 0) ";
                if (ddl_start_date_common != "0" && ddl_end_date_common != "0")
                {
                    start_end_date = "AND (start_date = " + ddl_start_date_common + " AND end_date = " + ddl_end_date_common + ") ";
                }

                where = "where pay_billing_shiftwise.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.unit_code='" + ddl_unitcode + "' and pay_billing_shiftwise.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_shiftwise.year = '" + txt_month_year.Text.Substring(3) + "' " + start_end_date;
                if (ddl_billing_state == "ALL")
                {
                    where = "where pay_billing_shiftwise.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_shiftwise.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_shiftwise.year = '" + txt_month_year.Text.Substring(3) + "'   " + start_end_date;
                }
                else if (ddl_unitcode == "ALL")
                {
                    where = "where pay_billing_shiftwise.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_shiftwise.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_shiftwise.state_name = '" + ddl_billing_state + "'  and pay_billing_shiftwise.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_shiftwise.year = '" + txt_month_year.Text.Substring(3) + "'  " + start_end_date;
                }

                where = "where auto_invoice_no='" + auto_inv_no + "'  " + start_end_date;

                sql = "SELECT  txt_zone, zone, auto_invoice_no AS 'bill_invoice_no', CASE WHEN pay_billing_shiftwise.invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_shiftwise.client_code, CASE WHEN pay_billing_shiftwise.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE pay_billing_shiftwise.client END AS 'client', pay_billing_shiftwise.state_name, pay_billing_shiftwise.unit_name, pay_billing_shiftwise.unit_city, pay_billing_shiftwise.client_branch_code, pay_billing_shiftwise.emp_name,shiftwise_rate ,pay_billing_shiftwise.amount AS 'Amount', Service_charge, CGST9, IGST18, SGST9, bill_service_charge, fromtodate, bill_service_charge_amount, branch_type, gst_applicable, pay_billing_shiftwise.unit_code, pay_billing_shiftwise.shift_days  FROM pay_billing_shiftwise INNER JOIN pay_shift_details ON pay_shift_details.comp_code = pay_billing_shiftwise.comp_code AND pay_shift_details.client_code = pay_billing_shiftwise.client_code AND pay_shift_details.unit_code = pay_billing_shiftwise.unit_code AND pay_shift_details.month = pay_billing_shiftwise.month AND pay_shift_details.year = pay_billing_shiftwise.year AND pay_shift_details.EMP_CODE = pay_billing_shiftwise.EMP_CODE  " + where + " group by pay_shift_details.EMP_CODE " + R_M_order_by_clause + "";

            }
            else if (i == 15)
            {

                string start_end_date = "AND (start_date = 0 AND end_date = 0) ";
                if (ddl_start_date_common != "0" && ddl_end_date_common != "0")
                {
                    start_end_date = "AND (start_date = " + ddl_start_date_common + " AND end_date = " + ddl_end_date_common + ") ";
                }

                where = "where pay_billing_incentive.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_incentive.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_incentive.unit_code='" + ddl_unitcode + "' and pay_billing_incentive.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_incentive.year = '" + txt_month_year.Text.Substring(3) + "' " + start_end_date;
                if (ddl_billing_state == "ALL")
                {
                    where = "where pay_billing_incentive.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_incentive.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_incentive.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_incentive.year = '" + txt_month_year.Text.Substring(3) + "'   " + start_end_date;
                }
                else if (ddl_unitcode == "ALL")
                {
                    where = "where pay_billing_incentive.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_incentive.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_incentive.state_name = '" + ddl_billing_state + "'  and pay_billing_incentive.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_incentive.year = '" + txt_month_year.Text.Substring(3) + "'  " + start_end_date;
                }

                sql = "SELECT  txt_zone, zone, auto_invoice_no AS 'bill_invoice_no', CASE WHEN pay_billing_incentive.invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_incentive.client_code, CASE WHEN pay_billing_incentive.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE pay_billing_incentive.client END AS 'client', pay_billing_incentive.state_name, pay_billing_incentive.unit_name, pay_billing_incentive.unit_city, pay_billing_incentive.client_branch_code, pay_billing_incentive.emp_name,pay_billing_incentive.amount AS 'Amount', Service_charge, CGST9, IGST18, SGST9, bill_service_charge, fromtodate, bill_service_charge_amount, branch_type, gst_applicable, pay_billing_incentive.unit_code  FROM pay_billing_incentive INNER JOIN pay_incentive_details ON pay_incentive_details.comp_code = pay_billing_incentive.comp_code AND pay_incentive_details.client_code = pay_billing_incentive.client_code AND pay_incentive_details.unit_code = pay_billing_incentive.unit_code AND pay_incentive_details.month = pay_billing_incentive.month AND pay_incentive_details.year = pay_billing_incentive.year AND pay_incentive_details.EMP_CODE = pay_billing_incentive.EMP_CODE  " + where + " group by pay_incentive_details.EMP_CODE " + R_M_order_by_clause + "";

            }
            // Office Rent finance copy
            else if (i == 16)
            {

                where = "where pay_billing_office_rent.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_office_rent.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_office_rent.unit_code='" + ddl_unitcode + "' and pay_billing_office_rent.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_office_rent.year = '" + txt_month_year.Text.Substring(3) + "'";
                if (ddl_billing_state == "ALL")
                {
                    where = "where pay_billing_office_rent.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_office_rent.client_code= '" + ddl_client.SelectedValue + "'  and pay_billing_office_rent.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_office_rent.year = '" + txt_month_year.Text.Substring(3) + "'";
                }
                else if (ddl_unitcode == "ALL")
                {
                    where = "where pay_billing_office_rent.comp_code='" + Session["comp_code"].ToString() + "' and pay_billing_office_rent.client_code= '" + ddl_client.SelectedValue + "' and pay_billing_office_rent.state_name = '" + ddl_billing_state + "'  and pay_billing_office_rent.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_office_rent.year = '" + txt_month_year.Text.Substring(3) + "'";
                }

                where = "where auto_invoice_no='" + auto_inv_no + "'";

                sql = "SELECT  txt_zone, zone, auto_invoice_no AS 'bill_invoice_no', CASE WHEN pay_billing_office_rent.invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', pay_billing_office_rent.client_code, CASE WHEN pay_billing_office_rent.client_code = 'BAGIC TM' THEN 'BAJAJ ALLIANZ GENERAL INSURANCE CO. LTD' ELSE pay_billing_office_rent.client END AS 'client', pay_billing_office_rent.state_name, pay_billing_office_rent.unit_name, pay_billing_office_rent.unit_city, pay_billing_office_rent.client_branch_code,pay_billing_office_rent.amount AS 'Amount', Service_charge, CGST9, IGST18, SGST9, bill_service_charge, fromtodate, bill_service_charge_amount, branch_type, gst_applicable, pay_billing_office_rent.unit_code  FROM pay_billing_office_rent   " + where + "";

            }

            DataSet ds = new DataSet();

            MySqlDataAdapter dscmd = new MySqlDataAdapter(sql, d.con);

            dscmd.SelectCommand.CommandTimeout = 200;

            dscmd.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (type_cl == 0)
                {
                    Response.Clear();
                    Response.Buffer = true;
                    if (i == 1)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=RATE_BREAKUP_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 2)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 3)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=ATTENDANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 4)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=SUPPORT_FORMAT_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    if (i == 5)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=STATE_WISE_RATE_BREAKUP_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    if (i == 6)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=ARREARS_RATE_BREAKUP_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 7)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=ARREARS_FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 8)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=ARREARS_ATTENDANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 9)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 10)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=OT_RATE_BREAKUP_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 11)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=R&M_FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 12)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=ADMINISTRATIVE_EXPENSE_FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 13)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=OT_SHEET_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 14)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=SHIFTWISE_FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 15)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=INCENTIVE_FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }
                    else if (i == 16)
                    {
                        Response.AddHeader("content-disposition", "attachment;filename=OFFICE_RENT_FINANCE_COPY_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                    }

                }
                if (ddl_client.SelectedValue == "RCPL" && i == 2) { invoice = ""; }
                if (ddl_client.SelectedValue == "ALL")
                {
                    start_date_common = "1";
                }
                string cal_days = "";
                if (i == 8)
                {
                    cal_days = d.get_calendar_days(int.Parse(start_date_common), txt_arrear_month_year.Substring(3), 0, 2);
                }
                else
                {
                    cal_days = d.get_calendar_days(int.Parse(start_date_common), txt_month_year.Text, 0, 2);
                }

        #endregion

                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate(ListItemType.Header, ds, i, invoice, bill_date, start_date_common, cal_days, month_days, type_cl, ddl_billing_state);
                Repeater1.ItemTemplate = new MyTemplate(ListItemType.Item, ds, i, invoice, bill_date, start_date_common, "", month_days, type_cl, ddl_billing_state);
                Repeater1.FooterTemplate = new MyTemplate(ListItemType.Footer, null, i, invoice, bill_date, start_date_common, "", month_days, type_cl, ddl_billing_state);
                Repeater1.DataBind();

                System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                Repeater1.RenderControl(htmlWrite);
                //Akshay 23-04-2019
                if (ddl_client.SelectedValue == "RCPL" && i == 2)
                {
                    stringWrite = update_grp_companies(stringWrite, ds);
                }
                if (type_cl == 1)
                {
                    return stringWrite;
                }

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
    public class MyTemplate : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr = 0;
        int start_cnt = 0, end_cnt = 0, row_cnt = 0, ctr1 = 0;
        //i3  is use to row data
        int i, i3 = 1, state_change = 0, client_change = 0, month_days = 0, type_cl = 0, atten = 3;
        string invoice = "";
        string bill_date = "";
        double rate = 0, paid_days = 0, service_charge = 0, grand_tot = 0, cgst = 0, sgst = 0, igst = 0, gst = 0, ctc = 0, present_days = 0, absent_days = 0, total_days = 0, ot_hrs = 0, ot_rate = 0, ot_amount = 0, sub_total = 0, total_emp_count = 0, no_of_duties = 0;

        double rate1 = 0, paid_days1 = 0, service_charge1 = 0, grand_tot1 = 0, cgst1 = 0, sgst1 = 0, igst1 = 0, gst1 = 0, ctc1 = 2, present_days1 = 0, absent_days1 = 0, total_days1 = 0, ot_hrs1 = 0, ot_rate1 = 0, ot_amount1 = 0, sub_total1 = 0, total_emp_count1 = 0, no_of_duties1 = 0;

        double basic = 0, vda = 0, emp_basic_vda = 0, bonus_rate = 0, washing = 0, travelling = 0, education = 0, allowances_esic = 0, cca_billing = 0, other_allow = 0, bonus_gross = 0, leave_gross = 0, gratuity_gross = 0, hra = 0, special_allowance = 0, bonus_after_gross = 0, leave_after_gross = 0, gratuity_after_gross = 0, NH = 0, pf = 0, esic = 0, uniform_ser = 0, group_insurance_billing_ser = 0, lwf = 0, operational_cost = 0, allowances_no_esic = 0, sub_total_a = 0, ot_pr_hr_rate = 0, esi_on_ot_amount = 0, ot_hours = 0, sub_total_b = 0, sub_total_ab = 0, relieving_charg = 0, sub_total_c = 0, uniform_no_ser = 0, operational_cost_no_ser = 0, Service_charge = 0, group_insurance_billing = 0, Amount = 0;
        //ADD MD 
        string washing1 = null, hra1 = null, s_allowance = null, allow = null, oth_allow = null, uniform1 = null, grp_insurance = null, medical_insurance = null, op_cost = null, DUTY_HOURS = null, RATE = null, NO_OF_PAID_DAYS = null, BASE_AMOUNT = null, OT_HOURS = null, OT_RATE = null, OT_AMOUNT = null, TOTAL_BASE_AMT_OT_AMT = null, SERVICE_CHARGE = null, GRAND_TOTAL = null, GRAND_TOTAL1 = null, CGST = null, SGST = null, IGST = null, TOTAL_GST = null, TOTAL_CTC = null, CONVEYANCE_RATE = null, CONVEYANCE_BASE_RATE = null, YEARLY_BONUS = null, YEARLY_GRATUITY = null;
        string header = "", header1 = "", state_name = "", client = "";
        string bodystr = "", start_date_common = "", branch_type = "", state_name_ddl = "", client_name = "", days_t = "", BASIC = "", VDA = "", BASIC_VDA = "", PF = "", ESIC = "", UNIFORM = "", GROUP_INSURANCE = "", LWF = "", OPERATIONAL_COST = "", ALLOWANCE = "", SUB_TOTAL_A = "", OT_1_HR_AMOUNT = "", ESIC_ON_OT = "", SUB_TOTAL_AMOUNT_B = "", SUB_TOTAL_AB = "", RELIEVING_AMOUNT = "", SUB_TOTAL_C = "";


        public MyTemplate(ListItemType type, DataSet ds, int i, string invoice, string bill_date, string start_date_common, string header1, int month_days, int type_cl, string state_name_ddl)
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
                    #region
                    if (i == 1)
                    {
                        header = "";
                        header = header + "<th>BASIC</th>";
                        header = header + "<th>VDA</th>";
                        header = header + "<th>BASIC <br style=\"mso-data-placement:same-cell;\">+ VDA</th>";
                        header = header + "<th>BONUS<br style=\"mso-data-placement:same-cell;\"> RATE</th>";
                        header = header + "<th>WASHING</th>";
                        header = header + "<th>TRAVELLING</th>";
                        header = header + "<th>" + (ds.Tables[0].Rows[ctr]["client"].ToString() == "Go Digit General Insurance" ? "FIXED HRA" : (ds.Tables[0].Rows[ctr]["client"].ToString() == "Go Digit Info Works" ? "FIXED HRA" : "EDUCATION")) + "</th>";
                        header = header + "<th>OTHER <br style=\"mso-data-placement:same-cell;\">ALLOWANCES</th>";
                        header = header + "<th>CCA</th>";
                        header = header + "<th>ALLOWANCE</th>";
                        header = header + "<th>BONUS <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["bill_bonus_percent"] + "% ON SALARY</th>";
                        header = header + "<th>EARNED LEAVES <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["leave_days"] + " DAYS ON SALARY</th>";
                        header = header + "<th>GRATUITY <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["gratuity_percent"] + " % ON SALARY</th>";

                        header = header + "<th>HRA @ " + ds.Tables[0].Rows[ctr]["hra_percent"] + "%</th>";
                        header = header + "<th>SPECIAL <br style=\"mso-data-placement:same-cell;\">ALLOWANCES</th>";
                        header = header + "<th>GROSS</th>";
                        header = header + "<th>BONUS " + ds.Tables[0].Rows[ctr]["bill_bonus_percent"] + "% <br style=\"mso-data-placement:same-cell;\">ON SALARY</th>";

                        header = header + "<th>EARNED LEAVES <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["leave_days"] + " DAYS<br style=\"mso-data-placement:same-cell;\"> ON SALARY</th>";

                        header = header + "<th>GRATUITY " + ds.Tables[0].Rows[ctr]["gratuity_percent"] + " % ON SALARY</th>";


                        header = header + "<th>NATIONAL<br style=\"mso-data-placement:same-cell;\"> HOLIDAY <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>";
                        header = header + "<th>PF " + ds.Tables[0].Rows[ctr]["bill_pf_percent"] + "% <br style=\"mso-data-placement:same-cell;\">Salary</th>";
                        header = header + "<th>ESIC " + ds.Tables[0].Rows[ctr]["bill_esic_percent"] + "% <br style=\"mso-data-placement:same-cell;\">on Gross</th>";
                        header = header + "<th>UNIFORM</th>";
                        header = header + "<th>GROUP <br style=\"mso-data-placement:same-cell;\">INSURANCE</th>";
                        header = header + "<th>MEDICAL <br style=\"mso-data-placement:same-cell;\">INSURANCE</th>";
                        header = header + "<th>LWF</th>";
                        header = header + "<th>OPERATIONAL <br style=\"mso-data-placement:same-cell;\">COST</th>";

                        header = header + "<th>ALLOWANCE</th>";
                        header = header + "<th>SUB <br style=\"mso-data-placement:same-cell;\">TOTAL A</th>";
                        header = header + "<th>OT 1 <br style=\"mso-data-placement:same-cell;\">HR AMOUNT</th>";
                        header = header + "<th>ESIC ON <br style=\"mso-data-placement:same-cell;\">OT AMOUNT</th>";
                        header = header + "<th>OT <br style=\"mso-data-placement:same-cell;\">HOURS</th>";

                        header = header + "<th>SUB TOTAL <br style=\"mso-data-placement:same-cell;\">AMOUNT B</th>";
                        header = header + "<th>SUB <br style=\"mso-data-placement:same-cell;\">TOTAL AB</th>";
                        header = header + "<th>RELIEVING<br style=\"mso-data-placement:same-cell;\"> AMOUNT</th>";
                        header = header + "<th>SUB <br style=\"mso-data-placement:same-cell;\">TOTAL C</th>";
                        header = header + "<th>UNIFORM</th>";
                        header = header + "<th>OPERATIONAL<br style=\"mso-data-placement:same-cell;\"> COST</th>";
                        header = header + "<th>OT <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>";
                        header = header + "<th>TRAVEL <br style=\"mso-data-placement:same-cell;\">ALLOWANCE</th>";
                        header = header + "<th>RATE</th>";
                        if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                        {
                            header = header + "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE @" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                        }
                        else
                        {
                            header = header + "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                        }
                        header = header + "<th>GROUP <br style=\"mso-data-placement:same-cell;\">INSURANCE</th>";
                        header = header + "<th>GRAND TOTAL</th>";
                        lc = new LiteralControl("<table border=1><tr><th colspan=54 bgcolor=yellow align=center> RATE BREAKUP " + reprint_invoice.month_name.ToUpper().ToUpper() + " FOR 8/12 HRS DUTY</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>ZONE</th><th>REGION</th><th>CLIENT NAME</th><th>STATE</th><th>LOCATION</th><th>EMPLOYEE NAME</th><th>DEG.</th><th>DUTY<br style=\"mso-data-placement:same-cell;\"> HRS</th><th>NO. OF <br style=\"mso-data-placement:same-cell;\">PAID DAYS</th>" + header + "</tr>");
                    }
                    else if (i == 2 || i == 7)
                    {
                        int colspan = 27, colspan2 = 27;
                        string branch = "";
                        string opus_code = "";
                        if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE <br style=\"mso-data-placement:same-cell;\">@" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                            colspan = 26;
                        }
                        else
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                            colspan = 24;
                        }


                        if (ds.Tables[0].Rows[ctr]["client"].ToString().Contains("HDFC"))
                        {
                            lc = new LiteralControl("<table border=1><tr><th colspan=34 bgcolor=yellow align=center>FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR.<br style=\"mso-data-placement:same-cell;\"> NO.</th><th>BILL<br style=\"mso-data-placement:same-cell;\"> NO</th><th>ZONE AS<br style=\"mso-data-placement:same-cell;\"> PER HDFC<br style=\"mso-data-placement:same-cell;\"> LIFE</th><th>ZONAL<br style=\"mso-data-placement:same-cell;\">NAME</th><th>BRANCH <br style=\"mso-data-placement:same-cell;\">COST <br style=\"mso-data-placement:same-cell;\">CENTER <br style=\"mso-data-placement:same-cell;\">CODE</th><th>REGION AS<br style=\"mso-data-placement:same-cell;\"> PER HDFC<br style=\"mso-data-placement:same-cell;\"> LIFE</th><th>CONCERN ADMIN</th><th>SECURITY <br style=\"mso-data-placement:same-cell;\">COMPANY <br style=\"mso-data-placement:same-cell;\">NAME</th><th>COST <br style=\"mso-data-placement:same-cell;\">CENTER</th><th>AREA IN<br style=\"mso-data-placement:same-cell;\">Sqr.Ft</th><th>BRANCH <br style=\"mso-data-placement:same-cell;\">CODE</th><th>LOCATION TYPE<br style=\"mso-data-placement:same-cell;\"> (BRANCH / REGIONAL OFFICE<br style=\"mso-data-placement:same-cell;\"> / ZONAL OFFICE / <br style=\"mso-data-placement:same-cell;\">HEAD OFFICE)</th><th>BRANCH NAME</th><th>" + ds.Tables[0].Rows[ctr]["GRADE_CODE"].ToString() + " <br style=\"mso-data-placement:same-cell;\">SHIFT TYPE</th><th>DUTY HOURS<br style=\"mso-data-placement:same-cell;\">(EACH GUARD)</th><th>APPLICABLE <br style=\"mso-data-placement:same-cell;\">GAZETTE <br style=\"mso-data-placement:same-cell;\">NOTIFCATION</th><th>CATEGORY <br style=\"mso-data-placement:same-cell;\">(SG / SO / SS / <br style=\"mso-data-placement:same-cell;\">GUNMAN)</th><th>STATE</th><th>RATE</th><th>NO.OF " + ds.Tables[0].Rows[ctr]["GRADE_CODE"].ToString() + " <br style=\"mso-data-placement:same-cell;\">IN BRANCH</th><th>NO. OF <br style=\"mso-data-placement:same-cell;\">DUTIES BY <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["GRADE_CODE"].ToString() + "</th><th>DAYS IN<br style=\"mso-data-placement:same-cell;\"> MONTH</th><th>BASE AMOUNT</th><th>OT <br style=\"mso-data-placement:same-cell;\">HOURS</th><th>OT <br style=\"mso-data-placement:same-cell;\">RATE</th><th>OT <br style=\"mso-data-placement:same-cell;\">AMOUNT</th><th>TOTAL BASE AMT & <br style=\"mso-data-placement:same-cell;\">OT AMT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");

                        }
                        else if (ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BAG") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BG") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIK HK") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC SG") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC") && !ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("4"))
                        {
                            if (ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIK HK") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC SG") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC"))
                            {
                                opus_code = "<th>OPUS CODE</th>";
                            }
                            if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "0")
                            {
                                branch = "<th>BRANCH <br style=\"mso-data-placement:same-cell;\">TYPE</th>";
                            }
                            lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan + " bgcolor=yellow align=center>FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR. NO.</th><th>BILL<br style=\"mso-data-placement:same-cell;\"> NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th>" + opus_code + "<th>BRANCH NAME</th>" + branch + "<th>ZONE</th><th>REGION</th><th>STATE</th><th>EMPLOYEE NAME</th><th>DEG.</th><th>RATE</th><th>NO. OF <br style=\"mso-data-placement:same-cell;\">PAID DAYS</th><th>BASE<br style=\"mso-data-placement:same-cell;\"> AMOUNT</th><th>OT <br style=\"mso-data-placement:same-cell;\">HOURS</th><th>OT <br style=\"mso-data-placement:same-cell;\">RATE</th><th>OT <br style=\"mso-data-placement:same-cell;\">AMOUNT</th><th>TOTAL BASE<br style=\"mso-data-placement:same-cell;\"> AMT & <br style=\"mso-data-placement:same-cell;\">OT AMT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");

                        }
                        else if (ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("4"))
                        {
                            colspan = 33; colspan2 = 33;
                            if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "0")
                            {
                                branch = "<th>BRANCH <br style=\"mso-data-placement:same-cell;\">TYPE</th>";
                                colspan2 = 34;
                            }

                            lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan2 + " bgcolor=yellow align=center>FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>BILL <br style=\"mso-data-placement:same-cell;\">NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>COST<br style=\"mso-data-placement:same-cell;\"> CENTER</th>" + opus_code + "<th>BRANCH NAME</th><th>BRANCH ADDRESS</th>" + branch + "<th>ZONE</th><th>REGION</th><th>STATE</th><th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "DISTRICT" : "CITY") + "</th><th>EMPLOYEE NAME</th><th>DEG.</th><th>DUTY <br style=\"mso-data-placement:same-cell;\">HOURS</th><th>IN HAND <br style=\"mso-data-placement:same-cell;\">SALARY</th><th>RATE</th><th>NO. OF<br style=\"mso-data-placement:same-cell;\"> PAID<br style=\"mso-data-placement:same-cell;\"> DAYS</th><th>BASE <br style=\"mso-data-placement:same-cell;\">AMOUNT</th><th>OT <br style=\"mso-data-placement:same-cell;\">HOURS</th><th>OT <br style=\"mso-data-placement:same-cell;\">RATE</th><th>OT <br style=\"mso-data-placement:same-cell;\">AMOUNT</th><th>TOTAL BASE <br style=\"mso-data-placement:same-cell;\">AMT & <br style=\"mso-data-placement:same-cell;\">OT AMT</th>" + header + "<th>UNIFORM</th><th>OPERTIONAL<br style=\"mso-data-placement:same-cell;\">COST</th><th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th><th>YEARLY BONUS</th><th>YEARLY GRATUITY</th></tr>");

                        }
                        else
                        {
                            // string conveyance_rate = "";

                            if (ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BAGICTM"))
                            {
                                header = "<th>TRAVEL <br style=\"mso-data-placement:same-cell;\"> ALLOWANCE</th><th>TOTAL BASE AMT <br style=\"mso-data-placement:same-cell;\">& TRAVEL ALLOWANCE</th>" + header;
                                colspan2 = colspan2 + 2;
                            }

                            if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "0")
                            {
                                branch = "<th>BRANCH <br style=\"mso-data-placement:same-cell;\">TYPE</th>";
                                colspan2 = 28;
                            }

                            lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan2 + " bgcolor=yellow align=center>FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>BILL <br style=\"mso-data-placement:same-cell;\">NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th>" + opus_code + "<th>BRANCH NAME</th>" + branch + "<th>ZONE</th><th>REGION</th><th>STATE</th><th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "DISTRICT" : "CITY") + "</th><th>EMPLOYEE NAME</th><th>DEG.</th><th>DUTY <br style=\"mso-data-placement:same-cell;\">HOURS</th><th>RATE</th><th>NO. OF<br style=\"mso-data-placement:same-cell;\"> PAID<br style=\"mso-data-placement:same-cell;\"> DAYS</th><th>BASE <br style=\"mso-data-placement:same-cell;\">AMOUNT</th><th>OT <br style=\"mso-data-placement:same-cell;\">HOURS</th><th>OT <br style=\"mso-data-placement:same-cell;\">RATE</th><th>OT <br style=\"mso-data-placement:same-cell;\">AMOUNT</th><th>TOTAL BASE <br style=\"mso-data-placement:same-cell;\">AMT & <br style=\"mso-data-placement:same-cell;\">OT AMT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");
                        }
                    }
                    #endregion
                    else if (i == 3 || i == 8)
                    {
                        header = "<th>1</th><th>2</th><th>3</th><th>4</th><th>5</th><th>6</th><th>7</th><th>8</th><th>9</th><th>10</th><th>11</th><th>12</th><th>13</th><th>14</th><th>15</th><th>16</th><th>17</th><th>18</th><th>19</th><th>20</th><th>21</th><th>22</th><th>23</th><th>24</th><th>25</th><th>26</th><th>27</th><th>28</th>";
                        int daysadd = 0;
                        int colspan = 41;
                        int days = int.Parse(ds.Tables[0].Rows[ctr]["total days"].ToString());
                        if (month_days > 0)
                        {
                            days = month_days;
                        }
                        if (days == 29)
                        { header = header + "<th>29</th>"; daysadd = 1; colspan = 42; }
                        else if (days == 30)
                        {
                            header = header + "<th>29</th><th>30</th>"; daysadd = 2;
                            colspan = 43;
                        }
                        else if (days == 31)
                        {
                            header = header + "<th>29</th><th>30</th><th>31</th>";
                            daysadd = 3;
                            colspan = 44;
                        }
                        if (start_date_common != "" && start_date_common != "1")
                        {
                            if (month_days == 0)
                            {
                                header = header1;
                            }
                        }
                        if (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM")
                        {
                            colspan = colspan + 1;
                        }
                        if (type_cl == 1) { colspan = colspan - 1; }

                        lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan + " bgcolor=yellow align=center>ATTENDANCE COPY FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SL. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>ZONE</th><th>REGION</th><th>STATE</th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "<th>DISTRICT</th>" : "") + "<th>LOCATION</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th><th>NAME</th><th>DEG.</th><th>OT <br style=\"mso-data-placement:same-cell;\">HRS.</th>" + header + "<th>TOTAL <br style=\"mso-data-placement:same-cell;\">P/DAY</th><th>ABSENT<br style=\"mso-data-placement:same-cell;\"> DAY</th><th>TOTAL <br style=\"mso-data-placement:same-cell;\">DAYS</th>" + (type_cl == 1 || type_cl == 0 ? "<th>STATUS</th>" : "") + "</tr>");
                        header = "";

                    }
                    #region
                    else if (i == 4)
                    {
                        if (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "UTKARSH")
                        {
                            lc = new LiteralControl("<table border=1><tr><th>SL. NO.</th><th>INVOICE NO</th><th>INVOICE DATE</th><th>BRANCH CODE</th><th>BRANCH NAME</th><th>STATE GST NO.</th><th>SHIP TO PARTY NAME</th><th>CITY</th><th>BASE VALUE</th><th>CGST 9%</th><th>SGST 9%</th><th>IGST 18%</th><th>TOTAL</th><th>STATE</th></tr>");
                        }
                        else if (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "MAX")
                        {
                            lc = new LiteralControl("<table border=1><tr><th>SR.NO.</th><th>LOCATION</th><th>STATE</th><th>RANK</th><th>STRENGTH</th><th>DUTY</th><th>RATE</th><th>AMOUNT</th><th>REMARKS</th><th>MONTH</th></tr>");
                        }
                    }
                    else if (i == 5)
                    {
                        header = "";
                        header = header + "<th>BASIC</th>";
                        header = header + "<th>VDA</th>";
                        header = header + "<th>BASIC <br style=\"mso-data-placement:same-cell;\">+ VDA</th>";
                        header = header + "<th>BONUS<br style=\"mso-data-placement:same-cell;\"> RATE</th>";
                        header = header + "<th>WASHING</th>";
                        header = header + "<th>TRAVELLING</th>";
                        header = header + "<th>EDUCATION</th>";
                        header = header + "<th>OTHER <br style=\"mso-data-placement:same-cell;\">ALLOWANCES</th>";
                        header = header + "<th>CCA</th>";
                        header = header + "<th>ALLOWANCE</th>";
                        header = header + "<th>BONUS <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["bill_bonus_percent"] + "% ON SALARY</th>";
                        header = header + "<th>EARNED LEAVES <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["leave_days"] + " DAYS ON SALARY</th>";
                        header = header + "<th>GRATUITY <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["gratuity_percent"] + " % ON SALARY</th>";

                        header = header + "<th>HRA @ " + ds.Tables[0].Rows[ctr]["hra_percent"] + "%</th>";
                        header = header + "<th>SPECIAL <br style=\"mso-data-placement:same-cell;\">ALLOWANCES</th>";
                        header = header + "<th>GROSS</th>";
                        header = header + "<th>BONUS " + ds.Tables[0].Rows[ctr]["bill_bonus_percent"] + "% <br style=\"mso-data-placement:same-cell;\">ON SALARY</th>";

                        header = header + "<th>EARNED LEAVES <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["leave_days"] + " DAYS<br style=\"mso-data-placement:same-cell;\"> ON SALARY</th>";

                        header = header + "<th>GRATUITY " + ds.Tables[0].Rows[ctr]["gratuity_percent"] + " % ON SALARY</th>";


                        header = header + "<th>NATIONAL<br style=\"mso-data-placement:same-cell;\"> HOLIDAY <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>";
                        header = header + "<th>PF " + ds.Tables[0].Rows[ctr]["pf_percent"] + "% <br style=\"mso-data-placement:same-cell;\">Salary</th>";
                        header = header + "<th>ESIC " + ds.Tables[0].Rows[ctr]["esic_percent"] + "% <br style=\"mso-data-placement:same-cell;\">on Gross</th>";
                        header = header + "<th>UNIFORM</th>";
                        header = header + "<th>GROUP <br style=\"mso-data-placement:same-cell;\">INSURANCE</th>";

                        header = header + "<th>LWF</th>";
                        header = header + "<th>OPERATIONAL <br style=\"mso-data-placement:same-cell;\">COST</th>";


                        header = header + "<th>ALLOWANCE</th>";
                        header = header + "<th>SUB <br style=\"mso-data-placement:same-cell;\">TOTAL A</th>";
                        header = header + "<th>OT 1 <br style=\"mso-data-placement:same-cell;\">HR AMOUNT</th>";
                        header = header + "<th>ESIC ON <br style=\"mso-data-placement:same-cell;\">OT AMOUNT</th>";
                        header = header + "<th>OT <br style=\"mso-data-placement:same-cell;\">HOURS</th>";

                        header = header + "<th>SUB TOTAL <br style=\"mso-data-placement:same-cell;\">AMOUNT B</th>";
                        header = header + "<th>SUB <br style=\"mso-data-placement:same-cell;\">TOTAL AB</th>";
                        header = header + "<th>RELIEVING<br style=\"mso-data-placement:same-cell;\"> AMOUNT</th>";
                        header = header + "<th>SUB <br style=\"mso-data-placement:same-cell;\">TOTAL C</th>";
                        header = header + "<th>UNIFORM</th>";
                        header = header + "<th>OPERATIONAL<br style=\"mso-data-placement:same-cell;\"> COST</th>";
                        header = header + "<th>OT <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>";
                        header = header + "<th>RATE</th>";
                        if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                        {
                            header = header + "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE @" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                        }
                        else
                        {
                            header = header + "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                        }
                        header = header + "<th>GROUP <br style=\"mso-data-placement:same-cell;\">INSURANCE</th>";
                        header = header + "<th>GRAND TOTAL</th>";

                        lc = new LiteralControl("<table border=1><tr><th colspan=48 bgcolor=yellow align=center>STATEWISE RATE BREAKUP  " + reprint_invoice.month_name.ToUpper().ToUpper() + " FOR 8/12 HRS DUTY</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>CLIENT NAME</th><th>STATE</th><th>BRANCH</th><th>DEG.</th><th>DUTY<br style=\"mso-data-placement:same-cell;\"> HRS</th>" + header + "</tr>");

                    }
                    //arrears
                    if (i == 6)
                    {
                        header = "";
                        header = header + "<th>BASIC</th>";
                        header = header + "<th>VDA</th>";
                        header = header + "<th>BASIC <br style=\"mso-data-placement:same-cell;\">+ VDA</th>";
                        header = header + "<th>BONUS<br style=\"mso-data-placement:same-cell;\"> RATE</th>";
                        header = header + "<th>WASHING</th>";
                        header = header + "<th>TRAVELLING</th>";
                        //  header = header + "<th>EDUCATION</th>";
                        header = header + "<th>" + (ds.Tables[0].Rows[ctr]["client"].ToString() == "Go Digit General Insurance" ? "FIXED HRA" : (ds.Tables[0].Rows[ctr]["client"].ToString() == "Go Digit Info Works" ? "FIXED HRA" : "EDUCATION")) + "</th>";

                        header = header + "<th>OTHER <br style=\"mso-data-placement:same-cell;\">ALLOWANCES</th>";
                        header = header + "<th>CCA</th>";
                        header = header + "<th>ALLOWANCE</th>";
                        header = header + "<th>BONUS <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["bill_bonus_percent"] + "% ON SALARY</th>";
                        header = header + "<th>EARNED LEAVES <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["leave_days"] + " DAYS ON SALARY</th>";
                        header = header + "<th>GRATUITY <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["gratuity_percent"] + " % ON SALARY</th>";

                        header = header + "<th>HRA @ " + ds.Tables[0].Rows[ctr]["hra_percent"] + "%</th>";
                        header = header + "<th>SPECIAL <br style=\"mso-data-placement:same-cell;\">ALLOWANCES</th>";
                        header = header + "<th>GROSS</th>";
                        header = header + "<th>BONUS " + ds.Tables[0].Rows[ctr]["bill_bonus_percent"] + "% <br style=\"mso-data-placement:same-cell;\">ON SALARY</th>";

                        header = header + "<th>EARNED LEAVES <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["leave_days"] + " DAYS<br style=\"mso-data-placement:same-cell;\"> ON SALARY</th>";

                        header = header + "<th>GRATUITY " + ds.Tables[0].Rows[ctr]["gratuity_percent"] + " % ON SALARY</th>";


                        header = header + "<th>NATIONAL<br style=\"mso-data-placement:same-cell;\"> HOLIDAY <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>";
                        header = header + "<th>PF " + ds.Tables[0].Rows[ctr]["bill_pf_percent"] + "% <br style=\"mso-data-placement:same-cell;\">Salary</th>";
                        header = header + "<th>ESIC " + ds.Tables[0].Rows[ctr]["bill_esic_percent"] + "% <br style=\"mso-data-placement:same-cell;\">on Gross</th>";
                        header = header + "<th>UNIFORM</th>";
                        header = header + "<th>GROUP <br style=\"mso-data-placement:same-cell;\">INSURANCE</th>";

                        header = header + "<th>LWF</th>";
                        header = header + "<th>OPERATIONAL <br style=\"mso-data-placement:same-cell;\">COST</th>";


                        header = header + "<th>ALLOWANCE</th>";
                        header = header + "<th>SUB <br style=\"mso-data-placement:same-cell;\">TOTAL A</th>";
                        header = header + "<th>OT 1 <br style=\"mso-data-placement:same-cell;\">HR AMOUNT</th>";
                        header = header + "<th>ESIC ON <br style=\"mso-data-placement:same-cell;\">OT AMOUNT</th>";
                        header = header + "<th>OT <br style=\"mso-data-placement:same-cell;\">HOURS</th>";

                        header = header + "<th>SUB TOTAL <br style=\"mso-data-placement:same-cell;\">AMOUNT B</th>";
                        header = header + "<th>SUB <br style=\"mso-data-placement:same-cell;\">TOTAL AB</th>";
                        header = header + "<th>RELIEVING<br style=\"mso-data-placement:same-cell;\"> AMOUNT</th>";
                        header = header + "<th>SUB <br style=\"mso-data-placement:same-cell;\">TOTAL C</th>";
                        header = header + "<th>UNIFORM</th>";
                        header = header + "<th>OPERATIONAL<br style=\"mso-data-placement:same-cell;\"> COST</th>";
                        header = header + "<th>OT <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>";
                        header = header + "<th>RATE</th>";
                        if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                        {
                            header = header + "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE @" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                        }
                        else
                        {
                            header = header + "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                        }
                        header = header + "<th>GROUP <br style=\"mso-data-placement:same-cell;\">INSURANCE</th>";
                        header = header + "<th>GRAND TOTAL</th>";
                        lc = new LiteralControl("<table border=1><tr><th colspan=49 bgcolor=yellow align=center> RATE BREAKUP " + reprint_invoice.month_name.ToUpper().ToUpper() + " FOR 8/12 HRS DUTY</th></tr><tr></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>STATE</th><th>LOCATION</th><th>EMPLOYEE NAME</th><th>DEG.</th><th>DUTY<br style=\"mso-data-placement:same-cell;\"> HRS</th><th>NO. OF <br style=\"mso-data-placement:same-cell;\">PAID DAYS</th>" + header + "<th> Policy</th><th> Month</th><th> Year</th></tr>");
                    }
                    else if (i == 9)
                    {
                        int colspan = 27, colspan2 = 27;
                        if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE <br style=\"mso-data-placement:same-cell;\">@" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                            colspan = 26;
                        }
                        else
                        {
                            header = header + "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                        }
                        if (ds.Tables[0].Rows[ctr]["client"].ToString().Contains("HDFC"))
                        {
                            lc = new LiteralControl("<table border=1><tr><th colspan=31 bgcolor=yellow align=center> OT FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR.<br style=\"mso-data-placement:same-cell;\"> NO.</th><th>BILL<br style=\"mso-data-placement:same-cell;\"> NO</th><th>ZONE AS<br style=\"mso-data-placement:same-cell;\"> PER HDFC<br style=\"mso-data-placement:same-cell;\"> LIFE</th><th>ZONE AS<br style=\"mso-data-placement:same-cell;\">NAME</th><th>BRANCH <br style=\"mso-data-placement:same-cell;\">COST <br style=\"mso-data-placement:same-cell;\">CENTER <br style=\"mso-data-placement:same-cell;\">CODE</th><th>REGION AS<br style=\"mso-data-placement:same-cell;\"> PER HDFC<br style=\"mso-data-placement:same-cell;\"> LIFE</th><th>CONCERN ADMIN</th><th>SECURITY <br style=\"mso-data-placement:same-cell;\">COMPANY <br style=\"mso-data-placement:same-cell;\">NAME</th><th>COST <br style=\"mso-data-placement:same-cell;\">CENTER</th><th>AREA IN<br style=\"mso-data-placement:same-cell;\">Sqr.Ft</th><th>BRANCH <br style=\"mso-data-placement:same-cell;\">CODE</th><th>LOCATION TYPE<br style=\"mso-data-placement:same-cell;\"> (BRANCH / REGIONAL OFFICE<br style=\"mso-data-placement:same-cell;\"> / ZONAL OFFICE / <br style=\"mso-data-placement:same-cell;\">HEAD OFFICE)</th><th>BRANCH NAME</th><th>" + ds.Tables[0].Rows[ctr]["GRADE_CODE"].ToString() + " <br style=\"mso-data-placement:same-cell;\">SHIFT TYPE</th><th>DUTY HOURS<br style=\"mso-data-placement:same-cell;\">(EACH GUARD)</th><th>APPLICABLE <br style=\"mso-data-placement:same-cell;\">GAZETTE <br style=\"mso-data-placement:same-cell;\">NOTIFCATION</th><th>CATEGORY <br style=\"mso-data-placement:same-cell;\">(SG / SO / SS / <br style=\"mso-data-placement:same-cell;\">GUNMAN)</th><th>STATE</th><th>NO.OF " + ds.Tables[0].Rows[ctr]["GRADE_CODE"].ToString() + " <br style=\"mso-data-placement:same-cell;\">IN BRANCH</th><th>NO. OF <br style=\"mso-data-placement:same-cell;\">DUTIES BY <br style=\"mso-data-placement:same-cell;\">" + ds.Tables[0].Rows[ctr]["GRADE_CODE"].ToString() + "</th><th>DAYS IN<br style=\"mso-data-placement:same-cell;\"> MONTH</th><th>OT <br style=\"mso-data-placement:same-cell;\">HOURS</th><th>OT <br style=\"mso-data-placement:same-cell;\">RATE</th><th>OT <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");

                        }
                        else
                        {
                            lc = new LiteralControl("<table border=1><tr><th colspan=23 bgcolor=yellow align=center>FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>BILL <br style=\"mso-data-placement:same-cell;\">NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th><th>BRANCH NAME</th><th>ZONE</th><th>REGION</th><th>STATE</th><th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "DISTRICT" : "CITY") + "</th><th>EMPLOYEE NAME</th><th>DEG.</th><th>NO. OF<br style=\"mso-data-placement:same-cell;\">PAID DAYS</th><th>OT <br style=\"mso-data-placement:same-cell;\">HOURS</th><th>OT <br style=\"mso-data-placement:same-cell;\">RATE</th><th>OT <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");

                        }
                    }
                    else if (i == 10)
                    {
                        header = "";
                        header = header + "<th>OT 1 <br style=\"mso-data-placement:same-cell;\">HR AMOUNT</th>";
                        header = header + "<th>ESIC ON <br style=\"mso-data-placement:same-cell;\">OT HOUR RATE</th>";
                        header = header + "<th>1 HOUR <br style=\"mso-data-placement:same-cell;\">OT RATE</th>";
                        header = header + "<th>OT <br style=\"mso-data-placement:same-cell;\">HOURS</th>";


                        header = header + "<th>OT <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>";
                        if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                        {
                            header = header + "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE @" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                        }
                        else
                        {
                            header = header + "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                        }
                        header = header + "<th>GRAND TOTAL</th>";
                        lc = new LiteralControl("<table border=1><tr><th colspan=17 bgcolor=yellow align=center> OT RATE BREAKUP " + reprint_invoice.month_name.ToUpper().ToUpper() + " FOR 8/12 HRS DUTY</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>ZONE</th><th>REGION</th><th>CLIENT NAME</th><th>STATE</th><th>LOCATION</th><th>EMPLOYEE NAME</th><th>DEG.</th><th>DUTY<br style=\"mso-data-placement:same-cell;\"> HRS</th><th>NO. OF <br style=\"mso-data-placement:same-cell;\">PAID DAYS</th>" + header + "</tr>");
                    }
                    else if (i == 11)
                    {
                        int colspan = 20;

                        if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE <br style=\"mso-data-placement:same-cell;\">@" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                            colspan = 22;
                        }
                        else
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                            colspan = 21;
                        }

                        lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan + " bgcolor=yellow align=center>FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>BILL <br style=\"mso-data-placement:same-cell;\">NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th><th>BRANCH NAME</th><th>ZONE</th><th>REGION</th><th>STATE</th><th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "DISTRICT" : "CITY") + "</th><th>PARTY NAME</th><th>IMG  <br style=\"mso-data-placement:same-cell;\">TICKET NO.</th><th>UTR<br style=\"mso-data-placement:same-cell;\">NUMBER</th><th>TOTAL  <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th><th>PAYMENT DATE</th></tr>");

                    }
                    else if (i == 12)
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

                        lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan + " bgcolor=yellow align=center>FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>BILL <br style=\"mso-data-placement:same-cell;\">NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th><th>BRANCH NAME</th><th>ZONE</th><th>REGION</th><th>STATE</th><th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "DISTRICT" : "CITY") + "</th><th>PARTY NAME</th><th>UTR<br style=\"mso-data-placement:same-cell;\">NUMBER</th><th>DAYS</th><th>TOTAL  <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");

                    }
                    else if (i == 13)
                    {

                        lc = new LiteralControl("<table border=1><tr><th colspan=10 bgcolor=yellow align=center>OT COPY FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SL. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>ZONE</th><th>REGION</th><th>STATE</th><th>LOCATION</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th><th>NAME</th><th>DEG.</th><th>OT <br style=\"mso-data-placement:same-cell;\">HRS.</th><th>STATUS</th></tr>");

                    }
                    else if (i == 14)
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

                        lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan + " bgcolor=yellow align=center>FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>BILL <br style=\"mso-data-placement:same-cell;\">NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th><th>BRANCH NAME</th><th>ZONE</th><th>REGION</th><th>STATE</th><th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "DISTRICT" : "CITY") + "</th><th>EMPLOYEE NAME</th><th>SHIFT<br style=\"mso-data-placement:same-cell;\">COUNT</th><th>SHIFT<br style=\"mso-data-placement:same-cell;\">RATE</th><th>TOTAL  <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");

                    }
                    else if (i == 15)
                    {
                        int colspan = 19;

                        if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE <br style=\"mso-data-placement:same-cell;\">@" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                            colspan = 20;
                        }
                        else
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                            colspan = 19;
                        }

                        lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan + " bgcolor=yellow align=center>FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>BILL <br style=\"mso-data-placement:same-cell;\">NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th><th>BRANCH NAME</th><th>ZONE</th><th>REGION</th><th>STATE</th><th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "DISTRICT" : "CITY") + "</th><th>EMPLOYEE NAME</th><th>TOTAL  <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");

                    }
                    else if (i == 16)
                    {
                        int colspan = 17;

                        if (double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()) > 0)
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE <br style=\"mso-data-placement:same-cell;\">@" + ds.Tables[0].Rows[ctr]["bill_service_charge"] + "%</th>";
                            colspan = 18;
                        }
                        else
                        {
                            header = "<th>SERVICE <br style=\"mso-data-placement:same-cell;\">CHARGE</th>";
                            colspan = 17;
                        }

                        lc = new LiteralControl("<table border=1><tr><th colspan=" + colspan + " bgcolor=yellow align=center>FINANCE COPY  FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr><th>SR. <br style=\"mso-data-placement:same-cell;\">NO.</th><th>BILL <br style=\"mso-data-placement:same-cell;\">NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>BRANCH<br style=\"mso-data-placement:same-cell;\"> CODE</th><th>BRANCH NAME</th><th>ZONE</th><th>REGION</th><th>STATE</th><th>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "DISTRICT" : "CITY") + "</th><th>TOTAL  <br style=\"mso-data-placement:same-cell;\">AMOUNT</th>" + header + "<th>GRAND <br style=\"mso-data-placement:same-cell;\">TOTAL</th><th>CGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>SGST <br style=\"mso-data-placement:same-cell;\">@9%</th><th>IGST <br style=\"mso-data-placement:same-cell;\">@18%</th><th>TOTAL GST</th><th>TOTAL CTC</th></tr>");

                    }
                    #endregion
                    break;
                case ListItemType.Item:
                    #region
                    if (i == 1)
                    {

                        //SAme like finance copy -vinod start
                        int set_start_row = 1;
                        int start_first_row = 3;
                        int colsize = 9;

                        if (client != ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper())
                        {
                            if (client != "")
                            {
                                //code here 
                                i3 = i3 + 1;

                                lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(J" + (ctc1 + set_start_row) + ":J" + (ctr + i3) + ")</td><td>=SUM(K" + (ctc1 + set_start_row) + ":K" + (ctr + i3) + ")</td><td>=SUM(L" + (ctc1 + set_start_row) + ":L" + (ctr + i3) + ")</td><td>=SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + ")</td><td>=ROUND(SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AB" + (ctc1 + set_start_row) + ":AB" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AC" + (ctc1 + set_start_row) + ":AC" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AD" + (ctc1 + set_start_row) + ":AD" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AE" + (ctc1 + set_start_row) + ":AE" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AF" + (ctc1 + set_start_row) + ":AF" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AG" + (ctc1 + set_start_row) + ":AG" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AH" + (ctc1 + set_start_row) + ":AH" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AI" + (ctc1 + set_start_row) + ":AI" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AJ" + (ctc1 + set_start_row) + ":AJ" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AK" + (ctc1 + set_start_row) + ":AK" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AL" + (ctc1 + set_start_row) + ":AL" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AM" + (ctc1 + set_start_row) + ":AM" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AN" + (ctc1 + set_start_row) + ":AN" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AO" + (ctc1 + set_start_row) + ":AO" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AP" + (ctc1 + set_start_row) + ":AP" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AQ" + (ctc1 + set_start_row) + ":AQ" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AR" + (ctc1 + set_start_row) + ":AR" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AS" + (ctc1 + set_start_row) + ":AS" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AT" + (ctc1 + set_start_row) + ":AT" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AU" + (ctc1 + set_start_row) + ":AU" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AV" + (ctc1 + set_start_row) + ":AV" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AW" + (ctc1 + set_start_row) + ":AW" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AX" + (ctc1 + set_start_row) + ":AX" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AY" + (ctc1 + set_start_row) + ":AY" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AZ" + (ctc1 + set_start_row) + ":AZ" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(BA" + (ctc1 + set_start_row) + ":BA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(BB" + (ctc1 + set_start_row) + ":BB" + (ctr + i3) + "),2)</td></tr>";

                                days_t = days_t + "," + "J" + (ctr + i3 + 1);
                                BASIC = BASIC + "," + "K" + (ctr + i3 + 1);
                                VDA = VDA + "," + "L" + (ctr + i3 + 1);
                                BASIC_VDA = BASIC_VDA + "," + "M" + (ctr + i3 + 1);
                                DUTY_HOURS = DUTY_HOURS + "," + "N" + (ctr + i3 + 1);
                                washing1 = washing1 + "," + "O" + (ctr + i3 + 1);
                                NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "P" + (ctr + i3 + 1);
                                BASE_AMOUNT = BASE_AMOUNT + "," + "Q" + (ctr + i3 + 1);
                                oth_allow = oth_allow + "," + "R" + (ctr + i3 + 1);
                                OT_RATE = OT_RATE + "," + "S" + (ctr + i3 + 1);
                                allow = allow + "," + "T" + (ctr + i3 + 1);
                                TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "U" + (ctr + i3 + 1);

                                CONVEYANCE_RATE = CONVEYANCE_RATE + "," + "V" + (ctr + i3 + 1);
                                CONVEYANCE_BASE_RATE = CONVEYANCE_BASE_RATE + "," + "W" + (ctr + i3 + 1);

                                hra1 = hra1 + "," + "X" + (ctr + i3 + 1);
                                s_allowance = s_allowance + "," + "Y" + (ctr + i3 + 1);
                                CGST = CGST + "," + "Z" + (ctr + i3 + 1);
                                SGST = SGST + "," + "AA" + (ctr + i3 + 1);
                                IGST = IGST + "," + "AB" + (ctr + i3 + 1);
                                TOTAL_GST = TOTAL_GST + "," + "AC" + (ctr + i3 + 1);
                                TOTAL_CTC = TOTAL_CTC + "," + "AD" + (ctr + i3 + 1);

                                PF = PF + "," + "AE" + (ctr + i3 + 1);
                                ESIC = ESIC + "," + "AF" + (ctr + i3 + 1);
                                uniform1 = uniform1 + "," + "AG" + (ctr + i3 + 1);
                                grp_insurance = grp_insurance + "," + "AH" + (ctr + i3 + 1);
                                medical_insurance = medical_insurance + "," + "AI" + (ctr + i3 + 1);
                                LWF = LWF + "," + "AJ" + (ctr + i3 + 1);
                                op_cost = op_cost + "," + "AK" + (ctr + i3 + 1);
                                ALLOWANCE = ALLOWANCE + "," + "AL" + (ctr + i3 + 1);
                                SUB_TOTAL_A = SUB_TOTAL_A + "," + "AM" + (ctr + i3 + 1);
                                OT_1_HR_AMOUNT = OT_1_HR_AMOUNT + "," + "AN" + (ctr + i3 + 1);
                                ESIC_ON_OT = ESIC_ON_OT + "," + "AO" + (ctr + i3 + 1);
                                OT_HOURS = OT_HOURS + "," + "AP" + (ctr + i3 + 1);
                                SUB_TOTAL_AMOUNT_B = SUB_TOTAL_AMOUNT_B + "," + "AQ" + (ctr + i3 + 1);
                                SUB_TOTAL_AB = SUB_TOTAL_AB + "," + "AR" + (ctr + i3 + 1);
                                RELIEVING_AMOUNT = RELIEVING_AMOUNT + "," + "AS" + (ctr + i3 + 1);
                                SUB_TOTAL_C = SUB_TOTAL_C + "," + "AT" + (ctr + i3 + 1);
                                UNIFORM = UNIFORM + "," + "AU" + (ctr + i3 + 1);
                                OPERATIONAL_COST = OPERATIONAL_COST + "," + "AV" + (ctr + i3 + 1);
                                OT_AMOUNT = OT_AMOUNT + "," + "AW" + (ctr + i3 + 1);
                                RATE = RATE + "," + "AX" + (ctr + i3 + 1);
                                SERVICE_CHARGE = SERVICE_CHARGE + "," + "AY" + (ctr + i3 + 1);
                                GROUP_INSURANCE = GROUP_INSURANCE + "," + "AZ" + (ctr + i3 + 1);
                                GRAND_TOTAL = GRAND_TOTAL + "," + "BA" + (ctr + i3 + 1);
                                GRAND_TOTAL1 = GRAND_TOTAL1 + "," + "BB" + (ctr + i3 + 1);
                                client_change = 1;

                                ctc1 = ctr + i3 + 1;

                            }

                            client = ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper();
                        }

                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["DUTYHRS"] + "</td><td>" + ds.Tables[0].Rows[ctr]["tot_days_present"] + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["basic"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["vda"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["emp_basic_vda"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bonus_rate"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["washing"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["travelling"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["education"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["allowances_esic"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["cca_billing"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["other_allow"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bonus_gross"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["leave_gross"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["gratuity_gross"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["hra"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["special_allowance"].ToString()), 2) + "</td><td>=ROUND(SUM(O" + (ctr + i3 + 2) + ":Y" + (ctr + i3 + 2) + ",M" + (ctr + i3 + 2) + "),2)</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bonus_after_gross"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["leave_after_gross"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["gratuity_after_gross"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["NH"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["pf"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["esic"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["uniform_ser"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing_ser"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["medical_insurance_amount"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["lwf"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["allowances_no_esic"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total_a"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["ot_pr_hr_rate"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["esi_on_ot_amount"].ToString()), 2) + "</td><td>" + ds.Tables[0].Rows[ctr]["ot_hours"] + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total_b"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total_ab"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["relieving_charg"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total_c"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["uniform_no_ser"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["operational_cost_no_ser"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["conveyance_amount"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total_c"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["conveyance_amount"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()), 2) + "</td></tr>");


                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            i3 = i3 + 2;


                            lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(J" + (ctc1 + set_start_row) + ":J" + (ctr + i3) + ")</td><td>=SUM(K" + (ctc1 + set_start_row) + ":K" + (ctr + i3) + ")</td><td>=SUM(L" + (ctc1 + set_start_row) + ":L" + (ctr + i3) + ")</td><td>=SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + ")</td><td>=ROUND(SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AB" + (ctc1 + set_start_row) + ":AB" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AC" + (ctc1 + set_start_row) + ":AC" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AD" + (ctc1 + set_start_row) + ":AD" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AE" + (ctc1 + set_start_row) + ":AE" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AF" + (ctc1 + set_start_row) + ":AF" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AG" + (ctc1 + set_start_row) + ":AG" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AH" + (ctc1 + set_start_row) + ":AH" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AI" + (ctc1 + set_start_row) + ":AI" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AJ" + (ctc1 + set_start_row) + ":AJ" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AK" + (ctc1 + set_start_row) + ":AK" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AL" + (ctc1 + set_start_row) + ":AL" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AM" + (ctc1 + set_start_row) + ":AM" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AN" + (ctc1 + set_start_row) + ":AN" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AO" + (ctc1 + set_start_row) + ":AO" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AP" + (ctc1 + set_start_row) + ":AP" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AQ" + (ctc1 + set_start_row) + ":AQ" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AR" + (ctc1 + set_start_row) + ":AR" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AS" + (ctc1 + set_start_row) + ":AS" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AT" + (ctc1 + set_start_row) + ":AT" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AU" + (ctc1 + set_start_row) + ":AU" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AV" + (ctc1 + set_start_row) + ":AV" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AW" + (ctc1 + set_start_row) + ":AW" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AX" + (ctc1 + set_start_row) + ":AX" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AY" + (ctc1 + set_start_row) + ":AY" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AZ" + (ctc1 + set_start_row) + ":AZ" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(BA" + (ctc1 + set_start_row) + ":BA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(BB" + (ctc1 + set_start_row) + ":BB" + (ctr + i3) + "),2)</td></tr>";

                            days_t = days_t + "," + "J" + (ctr + i3 + 1);
                            BASIC = BASIC + "," + "K" + (ctr + i3 + 1);
                            VDA = VDA + "," + "L" + (ctr + i3 + 1);
                            BASIC_VDA = BASIC_VDA + "," + "M" + (ctr + i3 + 1);
                            DUTY_HOURS = DUTY_HOURS + "," + "N" + (ctr + i3 + 1);
                            washing1 = washing1 + "," + "O" + (ctr + i3 + 1);
                            NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "P" + (ctr + i3 + 1);
                            BASE_AMOUNT = BASE_AMOUNT + "," + "Q" + (ctr + i3 + 1);
                            oth_allow = oth_allow + "," + "R" + (ctr + i3 + 1);
                            OT_RATE = OT_RATE + "," + "S" + (ctr + i3 + 1);
                            allow = allow + "," + "T" + (ctr + i3 + 1);
                            TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "U" + (ctr + i3 + 1);

                            CONVEYANCE_RATE = CONVEYANCE_RATE + "," + "V" + (ctr + i3 + 1);
                            CONVEYANCE_BASE_RATE = CONVEYANCE_BASE_RATE + "," + "W" + (ctr + i3 + 1);

                            hra1 = hra1 + "," + "X" + (ctr + i3 + 1);
                            s_allowance = s_allowance + "," + "Y" + (ctr + i3 + 1);
                            CGST = CGST + "," + "Z" + (ctr + i3 + 1);
                            SGST = SGST + "," + "AA" + (ctr + i3 + 1);
                            IGST = IGST + "," + "AB" + (ctr + i3 + 1);
                            TOTAL_GST = TOTAL_GST + "," + "AC" + (ctr + i3 + 1);
                            TOTAL_CTC = TOTAL_CTC + "," + "AD" + (ctr + i3 + 1);

                            PF = PF + "," + "AE" + (ctr + i3 + 1);
                            ESIC = ESIC + "," + "AF" + (ctr + i3 + 1);
                            uniform1 = uniform1 + "," + "AG" + (ctr + i3 + 1);
                            grp_insurance = grp_insurance + "," + "AH" + (ctr + i3 + 1);
                            medical_insurance = medical_insurance + "," + "AI" + (ctr + i3 + 1);
                            LWF = LWF + "," + "AJ" + (ctr + i3 + 1);
                            op_cost = op_cost + "," + "AK" + (ctr + i3 + 1);
                            ALLOWANCE = ALLOWANCE + "," + "AL" + (ctr + i3 + 1);
                            SUB_TOTAL_A = SUB_TOTAL_A + "," + "AM" + (ctr + i3 + 1);
                            OT_1_HR_AMOUNT = OT_1_HR_AMOUNT + "," + "AN" + (ctr + i3 + 1);
                            ESIC_ON_OT = ESIC_ON_OT + "," + "AO" + (ctr + i3 + 1);
                            OT_HOURS = OT_HOURS + "," + "AP" + (ctr + i3 + 1);
                            SUB_TOTAL_AMOUNT_B = SUB_TOTAL_AMOUNT_B + "," + "AQ" + (ctr + i3 + 1);
                            SUB_TOTAL_AB = SUB_TOTAL_AB + "," + "AR" + (ctr + i3 + 1);
                            RELIEVING_AMOUNT = RELIEVING_AMOUNT + "," + "AS" + (ctr + i3 + 1);
                            SUB_TOTAL_C = SUB_TOTAL_C + "," + "AT" + (ctr + i3 + 1);
                            UNIFORM = UNIFORM + "," + "AU" + (ctr + i3 + 1);
                            OPERATIONAL_COST = OPERATIONAL_COST + "," + "AV" + (ctr + i3 + 1);
                            OT_AMOUNT = OT_AMOUNT + "," + "AW" + (ctr + i3 + 1);
                            RATE = RATE + "," + "AX" + (ctr + i3 + 1);
                            SERVICE_CHARGE = SERVICE_CHARGE + "," + "AY" + (ctr + i3 + 1);
                            GROUP_INSURANCE = GROUP_INSURANCE + "," + "AZ" + (ctr + i3 + 1);
                            GRAND_TOTAL = GRAND_TOTAL + "," + "BA" + (ctr + i3 + 1);
                            GRAND_TOTAL1 = GRAND_TOTAL1 + "," + "BB" + (ctr + i3 + 1);


                            if (client_change == 1)
                            {
                                lc.Text = lc.Text + "<tr><b> <td align=center colspan=" + colsize + ">Grand Total</td><td>=ROUND(SUM(" + days_t + "),2)</td><td>=ROUND(SUM(" + BASIC + "),2)</td><td>=ROUND(SUM(" + VDA + "),2)</td><td>=ROUND(SUM(" + BASIC_VDA + "),2)</td><td>=ROUND(SUM(" + DUTY_HOURS + "),2)</td><td>=ROUND(SUM(" + washing1 + "),2)</td><td>=ROUND(SUM(" + NO_OF_PAID_DAYS + "),2)</td><td>=ROUND(SUM(" + BASE_AMOUNT + "),2)</td><td>=ROUND(SUM(" + oth_allow + "),2)</td><td>=ROUND(SUM(" + OT_RATE + "),2)</td><td>=ROUND(SUM(" + allow + "),2)</td><td>=ROUND(SUM(" + TOTAL_BASE_AMT_OT_AMT + "),2)</td><td>=ROUND(SUM(" + CONVEYANCE_RATE + "),2)</td><td>=ROUND(SUM(" + CONVEYANCE_BASE_RATE + "),2)</td><td>=ROUND(SUM(" + hra1 + "),2)</td><td>=ROUND(SUM(" + s_allowance + "),2)</td><td>=ROUND(SUM(" + CGST + "),2)</td><td>=ROUND(SUM(" + SGST + "),2)</td><td>=ROUND(SUM(" + IGST + "),2)</td><td>=ROUND(SUM(" + TOTAL_GST + "),2)</td><td>=ROUND(SUM(" + TOTAL_CTC + "),2)</td><td>=ROUND(SUM(" + PF + "),2)</td><td>=ROUND(SUM(" + ESIC + "),2)</td><td>=ROUND(SUM(" + uniform1 + "),2)</td><td>=ROUND(SUM(" + grp_insurance + "),2)</td><td>=ROUND(SUM(" + medical_insurance + "),2)</td><td>=ROUND(SUM(" + LWF + "),2)</td><td>=ROUND(SUM(" + op_cost + "),2)</td><td>=ROUND(SUM(" + ALLOWANCE + "),2)</td><td>=ROUND(SUM(" + SUB_TOTAL_A + "),2)</td><td>=ROUND(SUM(" + OT_1_HR_AMOUNT + "),2)</td><td>=ROUND(SUM(" + ESIC_ON_OT + "),2)</td><td>=ROUND(SUM(" + OT_HOURS + "),2)</td><td>=ROUND(SUM(" + SUB_TOTAL_AMOUNT_B + "),2)</td><td>=ROUND(SUM(" + SUB_TOTAL_AB + "),2)</td><td>=ROUND(SUM(" + RELIEVING_AMOUNT + "),2)</td><td>=ROUND(SUM(" + SUB_TOTAL_C + "),2)</td><td>=ROUND(SUM(" + UNIFORM + "),2)</td><td>=ROUND(SUM(" + OPERATIONAL_COST + "),2)</td><td>=ROUND(SUM(" + OT_AMOUNT + "),2)</td><td>=ROUND(SUM(" + RATE + "),2)</td><td>=ROUND(SUM(" + SERVICE_CHARGE + "),2)</td><td>=ROUND(SUM(" + GROUP_INSURANCE + "),2)</td><td>=ROUND(SUM(" + GRAND_TOTAL + "),2)</td><td>=ROUND(SUM(" + GRAND_TOTAL1 + "),2)</td></b></tr>";

                            }
                        }


                        header = "";
                        bodystr = "";
                    }
                    else if (i == 2 || i == 7)
                    {
                        string branch = "";
                        string opus_code = "";
                        string tot = "";
                        string tot_hrs = "";
                        string base_amount = "", tot_ctc = "", tot_gst = "";
                        int set_start_row = 1;
                        if (ds.Tables[0].Rows[ctr]["client"].ToString().Contains("HDFC"))
                        {
                            if (state_name != ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper())
                            {
                                if (state_name != "")
                                {
                                    //code here
                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=18>Total</td><td>" + (Math.Round(sub_total, 2) - Math.Round(sub_total1, 2)) + "</td><td>" + (total_emp_count - total_emp_count1) + "</td><td>" + (no_of_duties - no_of_duties1) + "</td><td>" + (paid_days - paid_days1) + "</td><td>" + (Math.Round(rate, 2) - Math.Round(rate1, 2)) + "</td><td>" + (ot_hrs - ot_hrs1) + "</td><td>" + (ot_rate - ot_rate1) + "</td><td>" + (ot_amount - ot_amount1) + "</td><td>" + (Math.Round((rate + ot_amount), 2) - Math.Round((rate1 + ot_amount1), 2)) + "</td><td>" + (Math.Round(service_charge, 2) - Math.Round(service_charge1, 2)) + "</td><td>" + (Math.Round(grand_tot, 2) - Math.Round(grand_tot1, 2)) + "</td><td>" + (cgst - cgst1) + "</td><td>" + (sgst - sgst1) + "</td><td>" + (igst - igst1) + "</td><td>" + (gst - gst1) + "</td><td>" + (Math.Ceiling(Math.Round(ctc, 2)) - Math.Ceiling(Math.Round(ctc1, 2))) + "</td></b></tr>";

                                    sub_total1 = sub_total;
                                    total_emp_count1 = total_emp_count;
                                    no_of_duties1 = no_of_duties;
                                    paid_days1 = paid_days;
                                    rate1 = rate;
                                    ot_hrs1 = ot_hrs;
                                    ot_rate1 = ot_rate;
                                    ot_amount1 = ot_amount;
                                    service_charge1 = service_charge;
                                    grand_tot1 = grand_tot;
                                    cgst1 = cgst;
                                    sgst1 = sgst;
                                    igst1 = igst;
                                    gst1 = gst;
                                    ctc1 = ctc;
                                    state_change = 1;
                                }
                                state_name = ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper();
                            }

                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zonal_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["branch_cost_centre_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["ihms"] + "</td><td>" + ds.Tables[0].Rows[ctr]["branch_cost_centre_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["material_area"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["location_type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_count"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_count1"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_per"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + Math.Round((double.Parse(ds.Tables[0].Rows[ctr]["sub_total_c"].ToString())), 2) + "</td><td>" + ds.Tables[0].Rows[ctr]["total_emp_count"] + "</td><td>" + ds.Tables[0].Rows[ctr]["no_of_duties"] + "</td><td>" + ds.Tables[0].Rows[ctr]["TOT_WORKING_DAYS"] + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()), 2) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_hours"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString())) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))) + double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + ((double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()))), 2) + "</td></tr>");

                            sub_total = sub_total + (double.Parse(ds.Tables[0].Rows[ctr]["sub_total_c"].ToString()));
                            total_emp_count = total_emp_count + (double.Parse(ds.Tables[0].Rows[ctr]["total_emp_count"].ToString()));
                            no_of_duties = no_of_duties + (double.Parse(ds.Tables[0].Rows[ctr]["no_of_duties"].ToString()));
                            paid_days = paid_days + (double.Parse(ds.Tables[0].Rows[ctr]["TOT_WORKING_DAYS"].ToString()));
                            rate = rate + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()));
                            ot_hrs = ot_hrs + double.Parse(ds.Tables[0].Rows[ctr]["ot_hours"].ToString());
                            ot_rate = ot_rate + double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString());
                            ot_amount = ot_amount + double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString());
                            service_charge = service_charge + (double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()));
                            grand_tot = grand_tot + ((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString())));
                            cgst = cgst + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()));
                            sgst = sgst + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()));
                            igst = igst + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()));
                            gst = gst + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()));
                            ctc = ctc + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))) + (double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString())) + double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()))), 2);
                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                if (state_change == 1)
                                {
                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=18>Total</td><td>" + (Math.Round(sub_total, 2) - Math.Round(sub_total1, 2)) + "</td><td>" + (total_emp_count - total_emp_count1) + "</td><td>" + (no_of_duties - no_of_duties1) + "</td><td>" + (paid_days - paid_days1) + "</td><td>" + (Math.Round(rate, 2) - Math.Round(rate1, 2)) + "</td><td>" + (ot_hrs - ot_hrs1) + "</td><td>" + (ot_rate - ot_rate1) + "</td><td>" + (ot_amount - ot_amount1) + "</td><td>" + (Math.Round((rate + ot_amount), 2) - Math.Round((rate1 + ot_amount1), 2)) + "</td><td>" + (Math.Round(service_charge, 2) - Math.Round(service_charge1, 2)) + "</td><td>" + (Math.Round(grand_tot, 2) - Math.Round(grand_tot1, 2)) + "</td><td>" + (cgst - cgst1) + "</td><td>" + (sgst - sgst1) + "</td><td>" + (igst - igst1) + "</td><td>" + (gst - gst1) + "</td><td>" + (Math.Ceiling(Math.Round(ctc, 2)) - Math.Ceiling(Math.Round(ctc1, 2))) + "</td></b></tr>";
                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=18>Total</td><td>" + Math.Round(sub_total, 2) + "</td><td>" + total_emp_count + "</td><td>" + no_of_duties + "</td><td>" + paid_days + "</td><td>" + Math.Round(rate, 2) + "</td><td>" + ot_hrs + "</td><td>" + ot_rate + "</td><td>" + ot_amount + "</td><td>" + Math.Round((rate + ot_amount), 2) + "</td><td>" + Math.Round(service_charge, 2) + "</td><td>" + Math.Round(grand_tot, 2) + "</td><td>" + cgst + "</td><td>" + sgst + "</td><td>" + igst + "</td><td>" + gst + "</td><td>" + (Math.Round(ctc, 2)) + "</td></b></tr>";
                                }
                                else
                                {
                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=18>Total</td><td>" + Math.Round(sub_total, 2) + "</td><td>" + total_emp_count + "</td><td>" + no_of_duties + "</td><td>" + paid_days + "</td><td>" + Math.Round(rate, 2) + "</td><td>" + ot_hrs + "</td><td>" + ot_rate + "</td><td>" + ot_amount + "</td><td>" + Math.Round((rate + ot_amount), 2) + "</td><td>" + Math.Round(service_charge, 2) + "</td><td>" + Math.Round(grand_tot, 2) + "</td><td>" + cgst + "</td><td>" + sgst + "</td><td>" + igst + "</td><td>" + gst + "</td><td>" + (Math.Round(ctc, 2)) + "</td></b></tr>";
                                }
                            }
                        }
                        //BAGICTMM
                        else if (ds.Tables[0].Rows[ctr]["client_code"].ToString().Contains("BAGICTM"))
                        {
                            int colsize = 12;

                            tot_hrs = "<td>=ROUND(SUM(M2:M" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            tot_ctc = "<td>=ROUND(SUM(X" + (ctr + i3 + set_start_row + 1) + ",AB" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            tot_gst = "<td>=ROUND(SUM(Y" + (ctr + i3 + set_start_row + 1) + ":AA" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            base_amount = "<td>=ROUND(N" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * O" + (ctr + i3 + set_start_row + 1) + ",2)</td>";

                            if (ds.Tables[0].Rows[ctr]["gst_applicable"].ToString() == "0")
                            {
                                ds.Tables[0].Rows[ctr]["IGST18"] = "0";
                                ds.Tables[0].Rows[ctr]["CGST9"] = "0";
                                ds.Tables[0].Rows[ctr]["SGST9"] = "0";
                            }

                            if (state_name != ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper())
                            {
                                if (state_name != "")
                                {
                                    //code here 
                                    i3 = i3 + 1;


                                    if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() == "0")
                                    {
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + ")</td><td>=ROUND(SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AB" + (ctc1 + set_start_row) + ":AB" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AC" + (ctc1 + set_start_row) + ":AC" + (ctr + i3) + "),2)</td></tr>";
                                        DUTY_HOURS = DUTY_HOURS + "," + "M" + (ctr + i3 + 1);
                                        RATE = RATE + "," + "N" + (ctr + i3 + 1);
                                        NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "O" + (ctr + i3 + 1);
                                        BASE_AMOUNT = BASE_AMOUNT + "," + "P" + (ctr + i3 + 1);
                                        OT_HOURS = OT_HOURS + "," + "Q" + (ctr + i3 + 1);
                                        OT_RATE = OT_RATE + "," + "R" + (ctr + i3 + 1);
                                        OT_AMOUNT = OT_AMOUNT + "," + "S" + (ctr + i3 + 1);
                                        TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "T" + (ctr + i3 + 1);

                                        CONVEYANCE_RATE = CONVEYANCE_RATE + "," + "U" + (ctr + i3 + 1);
                                        CONVEYANCE_BASE_RATE = CONVEYANCE_BASE_RATE + "," + "V" + (ctr + i3 + 1);

                                        SERVICE_CHARGE = SERVICE_CHARGE + "," + "W" + (ctr + i3 + 1);
                                        GRAND_TOTAL = GRAND_TOTAL + "," + "X" + (ctr + i3 + 1);
                                        CGST = CGST + "," + "Y" + (ctr + i3 + 1);
                                        SGST = SGST + "," + "Z" + (ctr + i3 + 1);
                                        IGST = IGST + "," + "AA" + (ctr + i3 + 1);
                                        TOTAL_GST = TOTAL_GST + "," + "AB" + (ctr + i3 + 1);
                                        TOTAL_CTC = TOTAL_CTC + "," + "AC" + (ctr + i3 + 1);
                                        state_change = 1;

                                        tot_hrs = "<td>=ROUND(SUM(M" + (set_start_row + 1) + ":M" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        tot_ctc = "<td>=ROUND(SUM(X" + (ctr + i3 + set_start_row + 1) + ",AB" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        tot_gst = "<td>=ROUND(SUM(Y" + (ctr + i3 + set_start_row + 1) + ":AA" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        base_amount = "<td>=ROUND(N" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * O" + (ctr + i3 + set_start_row + 1) + ",2)</td>";

                                    }
                                    else
                                    {
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=ROUND(SUM(L" + (ctc1 + set_start_row) + ":L" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + "),2)</td><td>=SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + ")</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td></tr>";

                                        DUTY_HOURS = DUTY_HOURS + "," + "N" + (ctr + i3 + 1);
                                        RATE = RATE + "," + "O" + (ctr + i3 + 1);
                                        NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "P" + (ctr + i3 + 1);
                                        BASE_AMOUNT = BASE_AMOUNT + "," + "Q" + (ctr + i3 + 1);
                                        OT_HOURS = OT_HOURS + "," + "R" + (ctr + i3 + 1);
                                        OT_RATE = OT_RATE + "," + "S" + (ctr + i3 + 1);
                                        OT_AMOUNT = OT_AMOUNT + "," + "T" + (ctr + i3 + 1);
                                        TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "U" + (ctr + i3 + 1);

                                        CONVEYANCE_RATE = CONVEYANCE_RATE + "," + "V" + (ctr + i3 + 1);
                                        CONVEYANCE_BASE_RATE = CONVEYANCE_BASE_RATE + "," + "W" + (ctr + i3 + 1);

                                        SERVICE_CHARGE = SERVICE_CHARGE + "," + "X" + (ctr + i3 + 1);
                                        GRAND_TOTAL = GRAND_TOTAL + "," + "Y" + (ctr + i3 + 1);
                                        CGST = CGST + "," + "Z" + (ctr + i3 + 1);
                                        SGST = SGST + "," + "AA" + (ctr + i3 + 1);
                                        IGST = IGST + "," + "AB" + (ctr + i3 + 1);
                                        TOTAL_GST = TOTAL_GST + "," + "AC" + (ctr + i3 + 1);
                                        TOTAL_CTC = TOTAL_CTC + "," + "AD" + (ctr + i3 + 1);
                                        state_change = 1;
                                        if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "0")
                                        {
                                            branch = "<td>" + ds.Tables[0].Rows[ctr]["branch_type"].ToString().ToUpper() + "</td>";
                                        }
                                        tot = "<td>=ROUND(SUM(AB" + (1 + set_start_row) + ":AB" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        base_amount = "<td>=ROUND(O" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * P" + (ctr + i3 + set_start_row + 1) + ",2)</td>";
                                        tot_ctc = "<td>=ROUND(SUM(W" + (ctr + i3 + set_start_row + 1) + ",AA" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        tot_gst = "<td>=ROUND(SUM(X" + (ctr + i3 + set_start_row + 1) + ":AB" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        colsize = 11;
                                    }
                                    ctc1 = ctr + i3 + 1;

                                }

                                state_name = ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper();
                            }

                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td>" + opus_code + "<td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td>" + branch + "<td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["hours"] + "</td><td>" + Math.Round((double.Parse(ds.Tables[0].Rows[ctr]["sub_total_c"].ToString())), 2) + "</td><td>" + ds.Tables[0].Rows[ctr]["tot_days_present"] + "</td>" + base_amount + "<td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_hours"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["conveyance_rate"].ToString()), 2) + "</td><td>" + Math.Round((double.Parse(ds.Tables[0].Rows[ctr]["conveyance_rate"].ToString()) + ((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["conveyance_rate"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td>" + tot_gst + "" + tot_ctc + "</tr>");


                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                i3 = i3 + 2;
                                //state total
                                if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() == "0" || ds.Tables[0].Rows[ctr]["branch_type"].ToString() == "")
                                {

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + ")</td><td>=ROUND(SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AB" + (ctc1 + set_start_row) + ":AB" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AC" + (ctc1 + set_start_row) + ":AC" + (ctr + i3) + "),2)</td></tr>";
                                    DUTY_HOURS = DUTY_HOURS + "," + "M" + (ctr + i3 + 1);
                                    RATE = RATE + "," + "N" + (ctr + i3 + 1);
                                    NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "O" + (ctr + i3 + 1);
                                    BASE_AMOUNT = BASE_AMOUNT + "," + "P" + (ctr + i3 + 1);
                                    OT_HOURS = OT_HOURS + "," + "Q" + (ctr + i3 + 1);
                                    OT_RATE = OT_RATE + "," + "R" + (ctr + i3 + 1);
                                    OT_AMOUNT = OT_AMOUNT + "," + "S" + (ctr + i3 + 1);
                                    TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "T" + (ctr + i3 + 1);

                                    CONVEYANCE_RATE = CONVEYANCE_RATE + "," + "U" + (ctr + i3 + 1);
                                    CONVEYANCE_BASE_RATE = CONVEYANCE_BASE_RATE + "," + "V" + (ctr + i3 + 1);

                                    SERVICE_CHARGE = SERVICE_CHARGE + "," + "W" + (ctr + i3 + 1);
                                    GRAND_TOTAL = GRAND_TOTAL + "," + "X" + (ctr + i3 + 1);
                                    CGST = CGST + "," + "Y" + (ctr + i3 + 1);
                                    SGST = SGST + "," + "Z" + (ctr + i3 + 1);
                                    IGST = IGST + "," + "AA" + (ctr + i3 + 1);
                                    TOTAL_GST = TOTAL_GST + "," + "AB" + (ctr + i3 + 1);
                                    TOTAL_CTC = TOTAL_CTC + "," + "AC" + (ctr + i3 + 1);



                                    if (state_change == 1)
                                    {
                                        lc.Text = lc.Text + "<tr><b> <td align=center colspan=" + colsize + ">Grand Total</td> <td>=ROUND(SUM(" + DUTY_HOURS + "),2)</td> <td>=ROUND(SUM(" + RATE + "),2)</td> <td>=ROUND(SUM(" + NO_OF_PAID_DAYS + "),2)</td> <td>=ROUND(SUM(" + BASE_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + OT_HOURS + "),2)</td> <td>=ROUND(SUM(" + OT_RATE + "),2)</td> <td>=ROUND(SUM(" + OT_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + TOTAL_BASE_AMT_OT_AMT + "),2)</td> <td>=ROUND(SUM(" + CONVEYANCE_RATE + "),2)</td>  <td>=ROUND(SUM(" + CONVEYANCE_BASE_RATE + "),2)</td> <td>=ROUND(SUM(" + SERVICE_CHARGE + "),2)</td> <td>=ROUND(SUM(" + GRAND_TOTAL + "),2)</td> <td>=ROUND(SUM(" + CGST + "),2)</td> <td>=ROUND(SUM(" + SGST + "),2)</td> <td>=ROUND(SUM(" + IGST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_GST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_CTC + "),2)</td> </b></tr>";

                                    }
                                }


                                //client total
                                else
                                {

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + ")</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AB" + (ctc1 + set_start_row) + ":AB" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AC" + (ctc1 + set_start_row) + ":AC" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AD" + (ctc1 + set_start_row) + ":AD" + (ctr + i3) + "),2)</td></tr>";


                                    DUTY_HOURS = DUTY_HOURS + "," + "N" + (ctr + i3 + 1);
                                    RATE = RATE + "," + "O" + (ctr + i3 + 1);
                                    NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "P" + (ctr + i3 + 1);
                                    BASE_AMOUNT = BASE_AMOUNT + "," + "Q" + (ctr + i3 + 1);
                                    OT_HOURS = OT_HOURS + "," + "R" + (ctr + i3 + 1);
                                    OT_RATE = OT_RATE + "," + "S" + (ctr + i3 + 1);
                                    OT_AMOUNT = OT_AMOUNT + "," + "T" + (ctr + i3 + 1);
                                    TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "U" + (ctr + i3 + 1);

                                    CONVEYANCE_RATE = CONVEYANCE_RATE + "," + "V" + (ctr + i3 + 1);
                                    CONVEYANCE_BASE_RATE = CONVEYANCE_BASE_RATE + "," + "W" + (ctr + i3 + 1);

                                    SERVICE_CHARGE = SERVICE_CHARGE + "," + "X" + (ctr + i3 + 1);
                                    GRAND_TOTAL = GRAND_TOTAL + "," + "Y" + (ctr + i3 + 1);
                                    CGST = CGST + "," + "Z" + (ctr + i3 + 1);
                                    SGST = SGST + "," + "AA" + (ctr + i3 + 1);
                                    IGST = IGST + "," + "AB" + (ctr + i3 + 1);
                                    TOTAL_GST = TOTAL_GST + "," + "AC" + (ctr + i3 + 1);
                                    TOTAL_CTC = TOTAL_CTC + "," + "AD" + (ctr + i3 + 1);
                                    if (state_change == 1)
                                    {
                                        lc.Text = lc.Text + "<tr><b> <td align=center colspan=" + colsize + ">Grand Total</td> <td>=ROUND(SUM(" + DUTY_HOURS + "),2)</td> <td>=ROUND(SUM(" + RATE + "),2)</td> <td>=ROUND(SUM(" + NO_OF_PAID_DAYS + "),2)</td> <td>=ROUND(SUM(" + BASE_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + OT_HOURS + "),2)</td> <td>=ROUND(SUM(" + OT_RATE + "),2)</td> <td>=ROUND(SUM(" + OT_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + TOTAL_BASE_AMT_OT_AMT + "),2)</td>  <td>=ROUND(SUM(" + CONVEYANCE_RATE + "),2)</td>  <td>=ROUND(SUM(" + CONVEYANCE_BASE_RATE + "),2)</td> <td>=ROUND(SUM(" + SERVICE_CHARGE + "),2)</td> <td>=ROUND(SUM(" + GRAND_TOTAL + "),2)</td> <td>=ROUND(SUM(" + CGST + "),2)</td> <td>=ROUND(SUM(" + SGST + "),2)</td> <td>=ROUND(SUM(" + IGST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_GST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_CTC + "),2)</td> </b></tr>";
                                    }
                                }

                            }


                        }
                        else if (ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BAG") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BG") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIK HK") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC SG") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC") && !ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("4"))
                        {
                            int colsize = 11;

                            //tot_hrs = "<td>=ROUND(SUM(K2:K" + (ctr + i3 + 2) + "),2)</td>";
                            tot_ctc = "<td>=ROUND(SUM(T" + (ctr + i3 + set_start_row + 1) + ",X" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            tot_gst = "<td>=ROUND(SUM(U" + (ctr + i3 + set_start_row + 1) + ":W" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            base_amount = "<td>=ROUND(L" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * M" + (ctr + i3 + set_start_row + 1) + ",2)</td>";

                            if (ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIK HK") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC SG") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC"))
                            {
                                opus_code = "<td>" + ds.Tables[0].Rows[ctr]["OPus_NO"].ToString().ToUpper() + "</td>";
                                tot = "<td>=ROUND(SUM(Z" + (1 + set_start_row) + ":Z" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                base_amount = "<td>=ROUND(M" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * N" + (ctr + i3 + set_start_row + 1) + ",2)</td>";
                                tot_ctc = "<td>=ROUND(SUM(U" + (ctr + i3 + set_start_row + 1) + ",Y" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                tot_gst = "<td>=ROUND(SUM(V" + (ctr + i3 + set_start_row + 1) + ":X" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                colsize = 12;
                            }


                            if (ds.Tables[0].Rows[ctr]["gst_applicable"].ToString() == "0")
                            {
                                ds.Tables[0].Rows[ctr]["IGST18"] = "0";
                                ds.Tables[0].Rows[ctr]["CGST9"] = "0";
                                ds.Tables[0].Rows[ctr]["SGST9"] = "0";
                            }

                            if (state_name != ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper())
                            {
                                if (state_name != "")
                                {
                                    //code here 
                                    i3 = i3 + 1;


                                    if (ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIK HK") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC SG") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC"))
                                    {
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + ")</td><td>=ROUND(SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td></tr>";
                                        //DUTY_HOURS = DUTY_HOURS + "," + "k" + (ctr + i3 + 1);
                                        RATE = RATE + "," + "M" + (ctr + i3 + 1);
                                        NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "N" + (ctr + i3 + 1);
                                        BASE_AMOUNT = BASE_AMOUNT + "," + "O" + (ctr + i3 + 1);
                                        OT_HOURS = OT_HOURS + "," + "P" + (ctr + i3 + 1);
                                        OT_RATE = OT_RATE + "," + "Q" + (ctr + i3 + 1);
                                        OT_AMOUNT = OT_AMOUNT + "," + "R" + (ctr + i3 + 1);
                                        TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "S" + (ctr + i3 + 1);
                                        SERVICE_CHARGE = SERVICE_CHARGE + "," + "T" + (ctr + i3 + 1);
                                        GRAND_TOTAL = GRAND_TOTAL + "," + "U" + (ctr + i3 + 1);
                                        CGST = CGST + "," + "V" + (ctr + i3 + 1);
                                        SGST = SGST + "," + "W" + (ctr + i3 + 1);
                                        IGST = IGST + "," + "X" + (ctr + i3 + 1);
                                        TOTAL_GST = TOTAL_GST + "," + "Y" + (ctr + i3 + 1);
                                        TOTAL_CTC = TOTAL_CTC + "," + "Z" + (ctr + i3 + 1);
                                        state_change = 1;

                                        //tot_hrs = "<td>=ROUND(SUM(K2:K" + (ctr + i3 + 2) + "),2)</td>";
                                        tot_ctc = "<td>=ROUND(SUM(U" + (ctr + i3 + set_start_row + 1) + ",Y" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        tot_gst = "<td>=ROUND(SUM(V" + (ctr + i3 + set_start_row + 1) + ":X" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        base_amount = "<td>=ROUND(M" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * N" + (ctr + i3 + set_start_row + 1) + ",2)</td>";

                                    }
                                    else
                                    {
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=ROUND(SUM(L" + (ctc1 + set_start_row) + ":L" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + "),2)</td><td>=SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + ")</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td></tr>";

                                        //DUTY_HOURS = DUTY_HOURS + "," + "L" + (ctr + i3 + 1);
                                        RATE = RATE + "," + "L" + (ctr + i3 + 1);
                                        NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "M" + (ctr + i3 + 1);
                                        BASE_AMOUNT = BASE_AMOUNT + "," + "N" + (ctr + i3 + 1);
                                        OT_HOURS = OT_HOURS + "," + "O" + (ctr + i3 + 1);
                                        OT_RATE = OT_RATE + "," + "P" + (ctr + i3 + 1);
                                        OT_AMOUNT = OT_AMOUNT + "," + "Q" + (ctr + i3 + 1);
                                        TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "R" + (ctr + i3 + 1);
                                        SERVICE_CHARGE = SERVICE_CHARGE + "," + "S" + (ctr + i3 + 1);
                                        GRAND_TOTAL = GRAND_TOTAL + "," + "T" + (ctr + i3 + 1);
                                        CGST = CGST + "," + "U" + (ctr + i3 + 1);
                                        SGST = SGST + "," + "V" + (ctr + i3 + 1);
                                        IGST = IGST + "," + "W" + (ctr + i3 + 1);
                                        TOTAL_GST = TOTAL_GST + "," + "X" + (ctr + i3 + 1);
                                        TOTAL_CTC = TOTAL_CTC + "," + "Y" + (ctr + i3 + 1);
                                        state_change = 1;

                                        //tot = "<td>=ROUND(SUM(Z2:Z" + (ctr + i3 + 2) + "),2)</td>";
                                        base_amount = "<td>=ROUND(L" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * M" + (ctr + i3 + set_start_row + 1) + ",2)</td>";
                                        //tot_ctc = "<td>=ROUND(SUM(U" + (ctr + i3 + 2) + ",Y" + (ctr + i3 + 2) + "),2)</td>";
                                        //tot_gst = "<td>=ROUND(SUM(V" + (ctr + i3 + 2) + ":X" + (ctr + i3 + 2) + "),2)</td>";

                                        tot_ctc = "<td>=ROUND(SUM(T" + (ctr + i3 + set_start_row + 1) + ",X" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        tot_gst = "<td>=ROUND(SUM(U" + (ctr + i3 + set_start_row + 1) + ":W" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        //colsize = 10;
                                    }
                                    ctc1 = ctr + i3 + 1;

                                }

                                state_name = ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper();
                            }

                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td>" + opus_code + "<td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td>" + branch + "<td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + Math.Round((double.Parse(ds.Tables[0].Rows[ctr]["sub_total_c"].ToString())), 2) + "</td><td>" + ds.Tables[0].Rows[ctr]["tot_days_present"] + "</td>" + base_amount + "<td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_hours"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td>" + tot_gst + "" + tot_ctc + "</tr>");


                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                i3 = i3 + 2;
                                //state total
                                if (ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIK HK") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC SG") || ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("BALIC"))
                                {

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + ")</td><td>=ROUND(SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td></tr>";

                                    RATE = RATE + "," + "M" + (ctr + i3 + 1);
                                    NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "N" + (ctr + i3 + 1);
                                    BASE_AMOUNT = BASE_AMOUNT + "," + "O" + (ctr + i3 + 1);
                                    OT_HOURS = OT_HOURS + "," + "P" + (ctr + i3 + 1);
                                    OT_RATE = OT_RATE + "," + "Q" + (ctr + i3 + 1);
                                    OT_AMOUNT = OT_AMOUNT + "," + "R" + (ctr + i3 + 1);
                                    TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "S" + (ctr + i3 + 1);
                                    SERVICE_CHARGE = SERVICE_CHARGE + "," + "T" + (ctr + i3 + 1);
                                    GRAND_TOTAL = GRAND_TOTAL + "," + "U" + (ctr + i3 + 1);
                                    CGST = CGST + "," + "V" + (ctr + i3 + 1);
                                    SGST = SGST + "," + "W" + (ctr + i3 + 1);
                                    IGST = IGST + "," + "X" + (ctr + i3 + 1);
                                    TOTAL_GST = TOTAL_GST + "," + "Y" + (ctr + i3 + 1);
                                    TOTAL_CTC = TOTAL_CTC + "," + "Z" + (ctr + i3 + 1);

                                    if (state_change == 1)
                                    {
                                        lc.Text = lc.Text + "<tr><b> <td align=center colspan=" + colsize + ">Total</td> <td>=ROUND(SUM(" + RATE + "),2)</td> <td>=ROUND(SUM(" + NO_OF_PAID_DAYS + "),2)</td> <td>=ROUND(SUM(" + BASE_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + OT_HOURS + "),2)</td> <td>=ROUND(SUM(" + OT_RATE + "),2)</td> <td>=ROUND(SUM(" + OT_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + TOTAL_BASE_AMT_OT_AMT + "),2)</td> <td>=ROUND(SUM(" + SERVICE_CHARGE + "),2)</td> <td>=ROUND(SUM(" + GRAND_TOTAL + "),2)</td> <td>=ROUND(SUM(" + CGST + "),2)</td> <td>=ROUND(SUM(" + SGST + "),2)</td> <td>=ROUND(SUM(" + IGST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_GST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_CTC + "),2)</td> </b></tr>";

                                    }
                                }

                                //client total
                                else
                                {

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=ROUND(SUM(L" + (ctc1 + set_start_row) + ":L" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + "),2)</td><td>=SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + ")</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td></tr>";


                                    RATE = RATE + "," + "L" + (ctr + i3 + 1);
                                    NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "M" + (ctr + i3 + 1);
                                    BASE_AMOUNT = BASE_AMOUNT + "," + "N" + (ctr + i3 + 1);
                                    OT_HOURS = OT_HOURS + "," + "O" + (ctr + i3 + 1);
                                    OT_RATE = OT_RATE + "," + "P" + (ctr + i3 + 1);
                                    OT_AMOUNT = OT_AMOUNT + "," + "Q" + (ctr + i3 + 1);
                                    TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "R" + (ctr + i3 + 1);
                                    SERVICE_CHARGE = SERVICE_CHARGE + "," + "S" + (ctr + i3 + 1);
                                    GRAND_TOTAL = GRAND_TOTAL + "," + "T" + (ctr + i3 + 1);
                                    CGST = CGST + "," + "U" + (ctr + i3 + 1);
                                    SGST = SGST + "," + "V" + (ctr + i3 + 1);
                                    IGST = IGST + "," + "W" + (ctr + i3 + 1);
                                    TOTAL_GST = TOTAL_GST + "," + "X" + (ctr + i3 + 1);
                                    TOTAL_CTC = TOTAL_CTC + "," + "Y" + (ctr + i3 + 1);
                                    if (state_change == 1)
                                    {
                                        lc.Text = lc.Text + "<tr><b> <td align=center colspan=" + colsize + ">Total</td> <td>=ROUND(SUM(" + DUTY_HOURS + "),2)</td> <td>=ROUND(SUM(" + RATE + "),2)</td> <td>=ROUND(SUM(" + NO_OF_PAID_DAYS + "),2)</td> <td>=ROUND(SUM(" + BASE_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + OT_HOURS + "),2)</td> <td>=ROUND(SUM(" + OT_RATE + "),2)</td> <td>=ROUND(SUM(" + OT_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + TOTAL_BASE_AMT_OT_AMT + "),2)</td> <td>=ROUND(SUM(" + SERVICE_CHARGE + "),2)</td> <td>=ROUND(SUM(" + GRAND_TOTAL + "),2)</td> <td>=ROUND(SUM(" + CGST + "),2)</td> <td>=ROUND(SUM(" + SGST + "),2)</td> <td>=ROUND(SUM(" + IGST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_GST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_CTC + "),2)</td> </b></tr>";
                                    }
                                }


                            }

                        }
                        else if (ds.Tables[0].Rows[ctr]["client_code"].ToString().Equals("4"))
                        {


                            int colsize = 13;

                            tot_hrs = "<td>=ROUND(SUM(M2:M" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            tot_ctc = "<td>=ROUND(SUM(V" + (ctr + i3 + set_start_row + 1) + ",Z" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            tot_gst = "<td>=ROUND(SUM(W" + (ctr + i3 + set_start_row + 1) + ":Y" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            base_amount = "<td>=ROUND(N" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * O" + (ctr + i3 + set_start_row + 1) + ",2)</td>";

                            if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "0" && ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "")
                            {
                                branch = "<td>" + ds.Tables[0].Rows[ctr]["branch_type"].ToString().ToUpper() + "</td>";
                                tot = "<td>=ROUND(SUM(AD2:AD" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                base_amount = "<td>=ROUND(Q" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * R" + (ctr + i3 + set_start_row + 1) + ",2)</td>";
                                tot_ctc = "<td>=ROUND(SUM(AA" + (ctr + i3 + set_start_row + 1) + ",AE" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                tot_gst = "<td>=ROUND(SUM(AB" + (ctr + i3 + set_start_row + 1) + ":AD" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                colsize = 14;
                            }

                            if (ds.Tables[0].Rows[ctr]["gst_applicable"].ToString() == "0")
                            {
                                ds.Tables[0].Rows[ctr]["IGST18"] = "0";
                                ds.Tables[0].Rows[ctr]["CGST9"] = "0";
                                ds.Tables[0].Rows[ctr]["SGST9"] = "0";
                            }

                            if (state_name != ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper())
                            {
                                if (state_name != "")
                                {
                                    //code here 
                                    i3 = i3 + 1;


                                    if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() == "0" && !ds.Tables[0].Rows[ctr]["client"].ToString().Contains("BAJAJ ALLIANZ LIFE INSURANCE COMPANY LIMITED"))
                                    {
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + ")</td><td>=ROUND(SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td></tr>";
                                        DUTY_HOURS = DUTY_HOURS + "," + "M" + (ctr + i3 + 1);
                                        RATE = RATE + "," + "N" + (ctr + i3 + 1);
                                        NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "O" + (ctr + i3 + 1);
                                        BASE_AMOUNT = BASE_AMOUNT + "," + "P" + (ctr + i3 + 1);
                                        OT_HOURS = OT_HOURS + "," + "Q" + (ctr + i3 + 1);
                                        OT_RATE = OT_RATE + "," + "R" + (ctr + i3 + 1);
                                        OT_AMOUNT = OT_AMOUNT + "," + "S" + (ctr + i3 + 1);
                                        TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "T" + (ctr + i3 + 1);
                                        SERVICE_CHARGE = SERVICE_CHARGE + "," + "U" + (ctr + i3 + 1);
                                        GRAND_TOTAL = GRAND_TOTAL + "," + "V" + (ctr + i3 + 1);
                                        CGST = CGST + "," + "W" + (ctr + i3 + 1);
                                        SGST = SGST + "," + "X" + (ctr + i3 + 1);
                                        IGST = IGST + "," + "Y" + (ctr + i3 + 1);
                                        TOTAL_GST = TOTAL_GST + "," + "Z" + (ctr + i3 + 1);
                                        TOTAL_CTC = TOTAL_CTC + "," + "AA" + (ctr + i3 + 1);
                                        state_change = 1;

                                        tot_ctc = "<td>=ROUND(SUM(V" + (ctr + i3 + set_start_row + 1) + ",Z" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        tot_gst = "<td>=ROUND(SUM(W" + (ctr + i3 + set_start_row + 1) + ":Y" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        base_amount = "<td>=ROUND(N" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * O" + (ctr + i3 + set_start_row + 1) + ",2)</td>";

                                    }
                                    else
                                    {
                                        //lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + ")</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AB" + (ctc1 + set_start_row) + ":AB" + (ctr + i3) + "),2)</td></tr>";

                                        //DUTY_HOURS = DUTY_HOURS + "," + "N" + (ctr + i3 + 1);
                                        //RATE = RATE + "," + "O" + (ctr + i3 + 1);
                                        //NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "P" + (ctr + i3 + 1);
                                        //BASE_AMOUNT = BASE_AMOUNT + "," + "Q" + (ctr + i3 + 1);
                                        //OT_HOURS = OT_HOURS + "," + "R" + (ctr + i3 + 1);
                                        //OT_RATE = OT_RATE + "," + "S" + (ctr + i3 + 1);
                                        //OT_AMOUNT = OT_AMOUNT + "," + "T" + (ctr + i3 + 1);
                                        //TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "U" + (ctr + i3 + 1);
                                        //SERVICE_CHARGE = SERVICE_CHARGE + "," + "V" + (ctr + i3 + 1);
                                        //GRAND_TOTAL = GRAND_TOTAL + "," + "W" + (ctr + i3 + 1);
                                        //CGST = CGST + "," + "X" + (ctr + i3 + 1);
                                        //SGST = SGST + "," + "Y" + (ctr + i3 + 1);
                                        //IGST = IGST + "," + "Z" + (ctr + i3 + 1);
                                        //TOTAL_GST = TOTAL_GST + "," + "AA" + (ctr + i3 + 1);
                                        //TOTAL_CTC = TOTAL_CTC + "," + "AB" + (ctr + i3 + 1);

                                        lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AB" + (ctc1 + set_start_row) + ":AB" + (ctr + i3) + "),2)</td><td>=SUM(AC" + (ctc1 + set_start_row) + ":AC" + (ctr + i3) + ")</td><td>=SUM(AD" + (ctc1 + set_start_row) + ":AD" + (ctr + i3) + ")</td><td>=SUM(AE" + (ctc1 + set_start_row) + ":AE" + (ctr + i3) + ")</td><td>=SUM(AF" + (ctc1 + set_start_row) + ":AF" + (ctr + i3) + ")</td><td>=SUM(AG" + (ctc1 + set_start_row) + ":AG" + (ctr + i3) + ")</td><td>=SUM(AH" + (ctc1 + set_start_row) + ":AH" + (ctr + i3) + ")</td></tr>";


                                        DUTY_HOURS = DUTY_HOURS + "," + "O" + (ctr + i3 + 1);
                                        RATE = RATE + "," + "Q" + (ctr + i3 + 1);
                                        NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "R" + (ctr + i3 + 1);
                                        BASE_AMOUNT = BASE_AMOUNT + "," + "S" + (ctr + i3 + 1);
                                        OT_HOURS = OT_HOURS + "," + "T" + (ctr + i3 + 1);
                                        OT_RATE = OT_RATE + "," + "U" + (ctr + i3 + 1);
                                        OT_AMOUNT = OT_AMOUNT + "," + "V" + (ctr + i3 + 1);
                                        TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "W" + (ctr + i3 + 1);
                                        SERVICE_CHARGE = SERVICE_CHARGE + "," + "X" + (ctr + i3 + 1);
                                        UNIFORM = UNIFORM + "," + "Y" + (ctr + i3 + 1);
                                        OPERATIONAL_COST = OPERATIONAL_COST + "," + "Z" + (ctr + i3 + 1);
                                        GRAND_TOTAL = GRAND_TOTAL + "," + "AA" + (ctr + i3 + 1);
                                        CGST = CGST + "," + "AB" + (ctr + i3 + 1);
                                        SGST = SGST + "," + "AC" + (ctr + i3 + 1);
                                        IGST = IGST + "," + "AD" + (ctr + i3 + 1);
                                        TOTAL_GST = TOTAL_GST + "," + "AE" + (ctr + i3 + 1);
                                        TOTAL_CTC = TOTAL_CTC + "," + "AF" + (ctr + i3 + 1);
                                        YEARLY_BONUS = YEARLY_BONUS + "," + "AG" + (ctr + i3 + 1);
                                        YEARLY_GRATUITY = YEARLY_GRATUITY + "," + "AH" + (ctr + i3 + 1);


                                        state_change = 1;
                                        //if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "0")
                                        //{
                                        //    branch = "<td>" + ds.Tables[0].Rows[ctr]["branch_type"].ToString().ToUpper() + "</td>";
                                        //}

                                        branch = "<td>" + ds.Tables[0].Rows[ctr]["branch_type"].ToString().ToUpper() + "</td>";
                                        tot = "<td>=ROUND(SUM(AD2:AD" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        base_amount = "<td>=ROUND(Q" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * R" + (ctr + i3 + set_start_row + 1) + ",2)</td>";
                                        tot_ctc = "<td>=ROUND(SUM(AA" + (ctr + i3 + set_start_row + 1) + ",AE" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        tot_gst = "<td>=ROUND(SUM(AB" + (ctr + i3 + set_start_row + 1) + ":AD" + (ctr + i3 + set_start_row + 1) + "),2)</td>";

                                        //tot = "<td>=ROUND(SUM(AB" + (1 + set_start_row) + ":AB" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        //base_amount = "<td>=ROUND(O" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * P" + (1 + ctr + i3 + set_start_row) + ",2)</td>";
                                        //tot_ctc = "<td>=ROUND(SUM(W" + (ctr + i3 + set_start_row + 1) + ",AA" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        //tot_gst = "<td>=ROUND(SUM(X" + (ctr + i3 + set_start_row + 1) + ":Z" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        //colsize = 14;
                                    }
                                    ctc1 = ctr + i3 + 1;

                                }

                                state_name = ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper();
                            }


                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td>" + opus_code + "<td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td></td>" + branch + "<td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["hours"] + "</td><td></td><td>" + Math.Round((double.Parse(ds.Tables[0].Rows[ctr]["sub_total_c"].ToString())), 2) + "</td><td>" + ds.Tables[0].Rows[ctr]["tot_days_present"] + "</td>" + base_amount + "<td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_hours"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td>" + tot_gst + "" + tot_ctc + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["yearly_bonus"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["yearly_gratuity"].ToString()), 2) + "</td></tr>");


                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                i3 = i3 + 2;
                                //state total
                                if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() == "0" || ds.Tables[0].Rows[ctr]["branch_type"].ToString() == "")
                                {

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + ")</td><td>=ROUND(SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td></tr>";
                                    DUTY_HOURS = DUTY_HOURS + "," + "M" + (ctr + i3 + 1);
                                    RATE = RATE + "," + "N" + (ctr + i3 + 1);
                                    NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "O" + (ctr + i3 + 1);
                                    BASE_AMOUNT = BASE_AMOUNT + "," + "P" + (ctr + i3 + 1);
                                    OT_HOURS = OT_HOURS + "," + "Q" + (ctr + i3 + 1);
                                    OT_RATE = OT_RATE + "," + "R" + (ctr + i3 + 1);
                                    OT_AMOUNT = OT_AMOUNT + "," + "S" + (ctr + i3 + 1);
                                    TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "T" + (ctr + i3 + 1);
                                    SERVICE_CHARGE = SERVICE_CHARGE + "," + "U" + (ctr + i3 + 1);
                                    GRAND_TOTAL = GRAND_TOTAL + "," + "V" + (ctr + i3 + 1);
                                    CGST = CGST + "," + "W" + (ctr + i3 + 1);
                                    SGST = SGST + "," + "X" + (ctr + i3 + 1);
                                    IGST = IGST + "," + "Y" + (ctr + i3 + 1);
                                    TOTAL_GST = TOTAL_GST + "," + "Z" + (ctr + i3 + 1);
                                    TOTAL_CTC = TOTAL_CTC + "," + "AA" + (ctr + i3 + 1);
                                    YEARLY_BONUS = YEARLY_BONUS + "," + "AG" + (ctr + i3 + 1);
                                    YEARLY_GRATUITY = YEARLY_GRATUITY + "," + "AH" + (ctr + i3 + 1);



                                    if (state_change == 1 && state_name_ddl.Equals("ALL"))
                                    {
                                        lc.Text = lc.Text + "<tr><b> <td align=center colspan=" + colsize + ">Grand Total</td> <td>=ROUND(SUM(" + DUTY_HOURS + "),2)</td><td></td><td>=ROUND(SUM(" + RATE + "),2)</td> <td>=ROUND(SUM(" + NO_OF_PAID_DAYS + "),2)</td> <td>=ROUND(SUM(" + BASE_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + OT_HOURS + "),2)</td> <td>=ROUND(SUM(" + OT_RATE + "),2)</td> <td>=ROUND(SUM(" + OT_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + TOTAL_BASE_AMT_OT_AMT + "),2)</td> <td>=ROUND(SUM(" + SERVICE_CHARGE + "),2)</td> <td>=ROUND(SUM(" + GRAND_TOTAL + "),2)</td> <td>=ROUND(SUM(" + CGST + "),2)</td> <td>=ROUND(SUM(" + SGST + "),2)</td> <td>=ROUND(SUM(" + IGST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_GST + "),2)</td><td>=ROUND(SUM(" + TOTAL_CTC + "),2)</td> <td>=ROUND(SUM(" + YEARLY_BONUS + "),2)</td><td>=ROUND(SUM(" + YEARLY_GRATUITY + "),2)</td></b></tr>";

                                    }
                                }


                                //client total
                                else
                                {

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AB" + (ctc1 + set_start_row) + ":AB" + (ctr + i3) + "),2)</td><td>=SUM(AC" + (ctc1 + set_start_row) + ":AC" + (ctr + i3) + ")</td><td>=SUM(AD" + (ctc1 + set_start_row) + ":AD" + (ctr + i3) + ")</td><td>=SUM(AE" + (ctc1 + set_start_row) + ":AE" + (ctr + i3) + ")</td><td>=SUM(AF" + (ctc1 + set_start_row) + ":AF" + (ctr + i3) + ")</td><td>=SUM(AG" + (ctc1 + set_start_row) + ":AG" + (ctr + i3) + ")</td><td>=SUM(AH" + (ctc1 + set_start_row) + ":AH" + (ctr + i3) + ")</td></tr>";


                                    DUTY_HOURS = DUTY_HOURS + "," + "O" + (ctr + i3 + 1);
                                    RATE = RATE + "," + "Q" + (ctr + i3 + 1);
                                    NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "R" + (ctr + i3 + 1);
                                    BASE_AMOUNT = BASE_AMOUNT + "," + "S" + (ctr + i3 + 1);
                                    OT_HOURS = OT_HOURS + "," + "T" + (ctr + i3 + 1);
                                    OT_RATE = OT_RATE + "," + "U" + (ctr + i3 + 1);
                                    OT_AMOUNT = OT_AMOUNT + "," + "V" + (ctr + i3 + 1);
                                    TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "W" + (ctr + i3 + 1);
                                    SERVICE_CHARGE = SERVICE_CHARGE + "," + "X" + (ctr + i3 + 1);
                                    UNIFORM = UNIFORM + "," + "Y" + (ctr + i3 + 1);
                                    OPERATIONAL_COST = OPERATIONAL_COST + "," + "Z" + (ctr + i3 + 1);
                                    GRAND_TOTAL = GRAND_TOTAL + "," + "AA" + (ctr + i3 + 1);
                                    CGST = CGST + "," + "AB" + (ctr + i3 + 1);
                                    SGST = SGST + "," + "AC" + (ctr + i3 + 1);
                                    IGST = IGST + "," + "AD" + (ctr + i3 + 1);
                                    TOTAL_GST = TOTAL_GST + "," + "AE" + (ctr + i3 + 1);
                                    TOTAL_CTC = TOTAL_CTC + "," + "AF" + (ctr + i3 + 1);
                                    YEARLY_BONUS = YEARLY_BONUS + "," + "AG" + (ctr + i3 + 1);
                                    YEARLY_GRATUITY = YEARLY_GRATUITY + "," + "AH" + (ctr + i3 + 1);

                                    if (state_change == 1 && state_name_ddl.Equals("ALL"))
                                    {
                                        lc.Text = lc.Text + "<tr><b> <td align=center colspan=" + colsize + ">Grand Total</td> <td>=ROUND(SUM(" + DUTY_HOURS + "),2)</td> <td></td><td>=ROUND(SUM(" + RATE + "),2)</td> <td>=ROUND(SUM(" + NO_OF_PAID_DAYS + "),2)</td> <td>=ROUND(SUM(" + BASE_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + OT_HOURS + "),2)</td> <td>=ROUND(SUM(" + OT_RATE + "),2)</td> <td>=ROUND(SUM(" + OT_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + TOTAL_BASE_AMT_OT_AMT + "),2)</td> <td>=ROUND(SUM(" + SERVICE_CHARGE + "),2)</td><td>=ROUND(SUM(" + UNIFORM + "),2)</td><td>=ROUND(SUM(" + OPERATIONAL_COST + "),2)</td> <td>=ROUND(SUM(" + GRAND_TOTAL + "),2)</td> <td>=ROUND(SUM(" + CGST + "),2)</td> <td>=ROUND(SUM(" + SGST + "),2)</td> <td>=ROUND(SUM(" + IGST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_GST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_CTC + "),2)</td><td>=ROUND(SUM(" + YEARLY_BONUS + "),2)</td><td>=ROUND(SUM(" + YEARLY_GRATUITY + "),2)</td> </b></tr>";
                                    }
                                }


                            }
                        }
                        else
                        {


                            int colsize = 12;

                            tot_hrs = "<td>=ROUND(SUM(M2:M" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            tot_ctc = "<td>=ROUND(SUM(V" + (ctr + i3 + set_start_row + 1) + ",Z" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            tot_gst = "<td>=ROUND(SUM(W" + (ctr + i3 + set_start_row + 1) + ":Y" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                            base_amount = "<td>=ROUND(N" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * O" + (ctr + i3 + set_start_row + 1) + ",2)</td>";

                            if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "0" && ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "")
                            {
                                branch = "<td>" + ds.Tables[0].Rows[ctr]["branch_type"].ToString().ToUpper() + "</td>";
                                tot = "<td>=ROUND(SUM(AB2:AB" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                base_amount = "<td>=ROUND(O" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * P" + (ctr + i3 + set_start_row + 1) + ",2)</td>";
                                tot_ctc = "<td>=ROUND(SUM(W" + (ctr + i3 + set_start_row + 1) + ",AA" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                tot_gst = "<td>=ROUND(SUM(X" + (ctr + i3 + set_start_row + 1) + ":Z" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                colsize = 13;
                            }

                            if (ds.Tables[0].Rows[ctr]["gst_applicable"].ToString() == "0")
                            {
                                ds.Tables[0].Rows[ctr]["IGST18"] = "0";
                                ds.Tables[0].Rows[ctr]["CGST9"] = "0";
                                ds.Tables[0].Rows[ctr]["SGST9"] = "0";
                            }

                            if (state_name != ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper())
                            {
                                if (state_name != "")
                                {
                                    //code here 
                                    i3 = i3 + 1;


                                    if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() == "0" && !ds.Tables[0].Rows[ctr]["client"].ToString().Contains("BAJAJ ALLIANZ LIFE INSURANCE COMPANY LIMITED"))
                                    {
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + ")</td><td>=ROUND(SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td></tr>";
                                        DUTY_HOURS = DUTY_HOURS + "," + "M" + (ctr + i3 + 1);
                                        RATE = RATE + "," + "N" + (ctr + i3 + 1);
                                        NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "O" + (ctr + i3 + 1);
                                        BASE_AMOUNT = BASE_AMOUNT + "," + "P" + (ctr + i3 + 1);
                                        OT_HOURS = OT_HOURS + "," + "Q" + (ctr + i3 + 1);
                                        OT_RATE = OT_RATE + "," + "R" + (ctr + i3 + 1);
                                        OT_AMOUNT = OT_AMOUNT + "," + "S" + (ctr + i3 + 1);
                                        TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "T" + (ctr + i3 + 1);
                                        SERVICE_CHARGE = SERVICE_CHARGE + "," + "U" + (ctr + i3 + 1);
                                        GRAND_TOTAL = GRAND_TOTAL + "," + "V" + (ctr + i3 + 1);
                                        CGST = CGST + "," + "W" + (ctr + i3 + 1);
                                        SGST = SGST + "," + "X" + (ctr + i3 + 1);
                                        IGST = IGST + "," + "Y" + (ctr + i3 + 1);
                                        TOTAL_GST = TOTAL_GST + "," + "Z" + (ctr + i3 + 1);
                                        TOTAL_CTC = TOTAL_CTC + "," + "AA" + (ctr + i3 + 1);
                                        state_change = 1;

                                        tot_ctc = "<td>=ROUND(SUM(V" + (ctr + i3 + set_start_row + 1) + ",Z" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        tot_gst = "<td>=ROUND(SUM(W" + (ctr + i3 + set_start_row + 1) + ":Y" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        base_amount = "<td>=ROUND(N" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * O" + (ctr + i3 + set_start_row + 1) + ",2)</td>";

                                    }
                                    else
                                    {
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + ")</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AB" + (ctc1 + set_start_row) + ":AB" + (ctr + i3) + "),2)</td></tr>";

                                        DUTY_HOURS = DUTY_HOURS + "," + "N" + (ctr + i3 + 1);
                                        RATE = RATE + "," + "O" + (ctr + i3 + 1);
                                        NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "P" + (ctr + i3 + 1);
                                        BASE_AMOUNT = BASE_AMOUNT + "," + "Q" + (ctr + i3 + 1);
                                        OT_HOURS = OT_HOURS + "," + "R" + (ctr + i3 + 1);
                                        OT_RATE = OT_RATE + "," + "S" + (ctr + i3 + 1);
                                        OT_AMOUNT = OT_AMOUNT + "," + "T" + (ctr + i3 + 1);
                                        TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "U" + (ctr + i3 + 1);
                                        SERVICE_CHARGE = SERVICE_CHARGE + "," + "V" + (ctr + i3 + 1);
                                        GRAND_TOTAL = GRAND_TOTAL + "," + "W" + (ctr + i3 + 1);
                                        CGST = CGST + "," + "X" + (ctr + i3 + 1);
                                        SGST = SGST + "," + "Y" + (ctr + i3 + 1);
                                        IGST = IGST + "," + "Z" + (ctr + i3 + 1);
                                        TOTAL_GST = TOTAL_GST + "," + "AA" + (ctr + i3 + 1);
                                        TOTAL_CTC = TOTAL_CTC + "," + "AB" + (ctr + i3 + 1);
                                        state_change = 1;
                                        if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "0")
                                        {
                                            branch = "<td>" + ds.Tables[0].Rows[ctr]["branch_type"].ToString().ToUpper() + "</td>";
                                        }
                                        tot = "<td>=ROUND(SUM(AB" + (1 + set_start_row) + ":AB" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        base_amount = "<td>=ROUND(O" + (ctr + i3 + set_start_row + 1) + "/" + ds.Tables[0].Rows[ctr]["month_days"].ToString() + " * P" + (1 + ctr + i3 + set_start_row) + ",2)</td>";
                                        tot_ctc = "<td>=ROUND(SUM(W" + (ctr + i3 + set_start_row + 1) + ",AA" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        tot_gst = "<td>=ROUND(SUM(X" + (ctr + i3 + set_start_row + 1) + ":Z" + (ctr + i3 + set_start_row + 1) + "),2)</td>";
                                        colsize = 13;
                                    }
                                    ctc1 = ctr + i3 + 1;

                                }

                                state_name = ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper();
                            }


                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td>" + opus_code + "<td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td>" + branch + "<td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["hours"] + "</td><td>" + Math.Round((double.Parse(ds.Tables[0].Rows[ctr]["sub_total_c"].ToString())), 2) + "</td><td>" + ds.Tables[0].Rows[ctr]["tot_days_present"] + "</td>" + base_amount + "<td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_hours"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td>" + tot_gst + "" + tot_ctc + "</tr>");


                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                i3 = i3 + 2;
                                //state total
                                if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() == "0" || ds.Tables[0].Rows[ctr]["branch_type"].ToString() == "")
                                {

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(M" + (ctc1 + set_start_row) + ":M" + (ctr + i3) + ")</td><td>=ROUND(SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td></tr>";
                                    DUTY_HOURS = DUTY_HOURS + "," + "M" + (ctr + i3 + 1);
                                    RATE = RATE + "," + "N" + (ctr + i3 + 1);
                                    NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "O" + (ctr + i3 + 1);
                                    BASE_AMOUNT = BASE_AMOUNT + "," + "P" + (ctr + i3 + 1);
                                    OT_HOURS = OT_HOURS + "," + "Q" + (ctr + i3 + 1);
                                    OT_RATE = OT_RATE + "," + "R" + (ctr + i3 + 1);
                                    OT_AMOUNT = OT_AMOUNT + "," + "S" + (ctr + i3 + 1);
                                    TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "T" + (ctr + i3 + 1);
                                    SERVICE_CHARGE = SERVICE_CHARGE + "," + "U" + (ctr + i3 + 1);
                                    GRAND_TOTAL = GRAND_TOTAL + "," + "V" + (ctr + i3 + 1);
                                    CGST = CGST + "," + "W" + (ctr + i3 + 1);
                                    SGST = SGST + "," + "X" + (ctr + i3 + 1);
                                    IGST = IGST + "," + "Y" + (ctr + i3 + 1);
                                    TOTAL_GST = TOTAL_GST + "," + "Z" + (ctr + i3 + 1);
                                    TOTAL_CTC = TOTAL_CTC + "," + "AA" + (ctr + i3 + 1);



                                    if (state_change == 1 && state_name_ddl.Equals("ALL"))
                                    {
                                        lc.Text = lc.Text + "<tr><b> <td align=center colspan=" + colsize + ">Grand Total</td> <td>=ROUND(SUM(" + DUTY_HOURS + "),2)</td> <td>=ROUND(SUM(" + RATE + "),2)</td> <td>=ROUND(SUM(" + NO_OF_PAID_DAYS + "),2)</td> <td>=ROUND(SUM(" + BASE_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + OT_HOURS + "),2)</td> <td>=ROUND(SUM(" + OT_RATE + "),2)</td> <td>=ROUND(SUM(" + OT_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + TOTAL_BASE_AMT_OT_AMT + "),2)</td> <td>=ROUND(SUM(" + SERVICE_CHARGE + "),2)</td> <td>=ROUND(SUM(" + GRAND_TOTAL + "),2)</td> <td>=ROUND(SUM(" + CGST + "),2)</td> <td>=ROUND(SUM(" + SGST + "),2)</td> <td>=ROUND(SUM(" + IGST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_GST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_CTC + "),2)</td> </b></tr>";

                                    }
                                }


                                //client total
                                else
                                {

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + colsize + ">Total</td><td>=SUM(N" + (ctc1 + set_start_row) + ":N" + (ctr + i3) + ")</td><td>=ROUND(SUM(O" + (ctc1 + set_start_row) + ":O" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(P" + (ctc1 + set_start_row) + ":P" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Q" + (ctc1 + set_start_row) + ":Q" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(R" + (ctc1 + set_start_row) + ":R" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(S" + (ctc1 + set_start_row) + ":S" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(T" + (ctc1 + set_start_row) + ":T" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(U" + (ctc1 + set_start_row) + ":U" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(V" + (ctc1 + set_start_row) + ":V" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(W" + (ctc1 + set_start_row) + ":W" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(X" + (ctc1 + set_start_row) + ":X" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Y" + (ctc1 + set_start_row) + ":Y" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(Z" + (ctc1 + set_start_row) + ":Z" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AA" + (ctc1 + set_start_row) + ":AA" + (ctr + i3) + "),2)</td><td>=ROUND(SUM(AB" + (ctc1 + set_start_row) + ":AB" + (ctr + i3) + "),2)</td></tr>";


                                    DUTY_HOURS = DUTY_HOURS + "," + "N" + (ctr + i3 + 1);
                                    RATE = RATE + "," + "O" + (ctr + i3 + 1);
                                    NO_OF_PAID_DAYS = NO_OF_PAID_DAYS + "," + "P" + (ctr + i3 + 1);
                                    BASE_AMOUNT = BASE_AMOUNT + "," + "Q" + (ctr + i3 + 1);
                                    OT_HOURS = OT_HOURS + "," + "R" + (ctr + i3 + 1);
                                    OT_RATE = OT_RATE + "," + "S" + (ctr + i3 + 1);
                                    OT_AMOUNT = OT_AMOUNT + "," + "T" + (ctr + i3 + 1);
                                    TOTAL_BASE_AMT_OT_AMT = TOTAL_BASE_AMT_OT_AMT + "," + "U" + (ctr + i3 + 1);
                                    SERVICE_CHARGE = SERVICE_CHARGE + "," + "V" + (ctr + i3 + 1);
                                    GRAND_TOTAL = GRAND_TOTAL + "," + "W" + (ctr + i3 + 1);
                                    CGST = CGST + "," + "X" + (ctr + i3 + 1);
                                    SGST = SGST + "," + "Y" + (ctr + i3 + 1);
                                    IGST = IGST + "," + "Z" + (ctr + i3 + 1);
                                    TOTAL_GST = TOTAL_GST + "," + "AA" + (ctr + i3 + 1);
                                    TOTAL_CTC = TOTAL_CTC + "," + "AB" + (ctr + i3 + 1);
                                    if (state_change == 1 && state_name_ddl.Equals("ALL"))
                                    {
                                        lc.Text = lc.Text + "<tr><b> <td align=center colspan=" + colsize + ">Grand Total</td> <td>=ROUND(SUM(" + DUTY_HOURS + "),2)</td> <td>=ROUND(SUM(" + RATE + "),2)</td> <td>=ROUND(SUM(" + NO_OF_PAID_DAYS + "),2)</td> <td>=ROUND(SUM(" + BASE_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + OT_HOURS + "),2)</td> <td>=ROUND(SUM(" + OT_RATE + "),2)</td> <td>=ROUND(SUM(" + OT_AMOUNT + "),2)</td> <td>=ROUND(SUM(" + TOTAL_BASE_AMT_OT_AMT + "),2)</td> <td>=ROUND(SUM(" + SERVICE_CHARGE + "),2)</td> <td>=ROUND(SUM(" + GRAND_TOTAL + "),2)</td> <td>=ROUND(SUM(" + CGST + "),2)</td> <td>=ROUND(SUM(" + SGST + "),2)</td> <td>=ROUND(SUM(" + IGST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_GST + "),2)</td> <td>=ROUND(SUM(" + TOTAL_CTC + "),2)</td> </b></tr>";
                                    }
                                }


                            }
                        }
                    }
                    #endregion
                    else if (i == 3 || i == 8)
                    {
                        string color = "";
                        bodystr = "";
                        int start_first_row = 3;
                        if (ds.Tables[0].Rows[ctr]["DAY01"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY01"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY02"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY02"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY03"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY03"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY04"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY04"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY05"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY05"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY06"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY06"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY07"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY07"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY08"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY08"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY09"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY09"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY10"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY10"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY11"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY11"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY12"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY12"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY13"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY13"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY14"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY14"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY15"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY15"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY16"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY16"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY17"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY17"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY18"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY18"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY19"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY19"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY20"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY20"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY21"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY21"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY22"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY22"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY23"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY23"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY24"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY24"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY25"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY25"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY26"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY26"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY27"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY27"] + "</td>";
                        if (ds.Tables[0].Rows[ctr]["DAY28"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY28"] + "</td>";

                        int days = int.Parse(ds.Tables[0].Rows[ctr]["total days"].ToString());
                        if (month_days > 0)
                        {
                            days = month_days;
                        }

                        if (days == 29)
                        {
                            if (ds.Tables[0].Rows[ctr]["DAY29"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY29"] + "</td>";
                            //bodystr = "<td>" + ds.Tables[0].Rows[ctr]["DAY29"] + "</td>"; 
                        }
                        else if (days == 30)
                        {
                            if (ds.Tables[0].Rows[ctr]["DAY29"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY29"] + "</td>";
                            if (ds.Tables[0].Rows[ctr]["DAY30"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY30"] + "</td>";

                            // bodystr = "<td>" + ds.Tables[0].Rows[ctr]["DAY29"] + "</td><td>" + ds.Tables[0].Rows[ctr]["DAY30"] + "</td>";
                        }
                        else if (days == 31)
                        {
                            if (ds.Tables[0].Rows[ctr]["DAY29"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY29"] + "</td>";
                            if (ds.Tables[0].Rows[ctr]["DAY30"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY30"] + "</td>";
                            if (ds.Tables[0].Rows[ctr]["DAY31"].ToString() == "A") { color = "red"; } else { color = "white"; } bodystr = bodystr + "<td bgcolor=" + color + ">" + ds.Tables[0].Rows[ctr]["DAY31"] + "</td>";

                            //  bodystr = "<td>" + ds.Tables[0].Rows[ctr]["DAY29"] + "</td><td>" + ds.Tables[0].Rows[ctr]["DAY30"] + "</td><td>" + ds.Tables[0].Rows[ctr]["DAY31"] + "</td>";
                        }
                        int count = bodystr.Split('A').Length - 1;
                        //string present_day1 = (days == 31 ? " = SUM(COUNTIF(J" + (ctr + start_first_row) + ":AN" + (ctr + start_first_row) + ",\"P\")+COUNTIF(J" + (ctr + start_first_row) + ":AN" + (ctr + start_first_row) + ",\"PH\")+COUNTIF(J" + (ctr + start_first_row) + ":AN" + (ctr + start_first_row) + ",\"HD\")/2)" : (days == 28) ? " = SUM(COUNTIF(J" + (ctr + start_first_row) + ":AK" + (ctr + start_first_row) + ",\"P\")+COUNTIF(J" + (ctr + start_first_row) + ":AK" + (ctr + start_first_row) + ",\"PH\")+COUNTIF(H" + (ctr + start_first_row) + ":AI" + (ctr + start_first_row) + ",\"HD\")/2)" : (days == 29) ? " = SUM(COUNTIF(H" + (ctr + start_first_row) + ":AJ" + (ctr + start_first_row) + ",\"P\")+COUNTIF(J" + (ctr + start_first_row) + ":AL" + (ctr + start_first_row) + ",\"PH\")+COUNTIF(J" + (ctr + start_first_row) + ":AL" + (ctr + start_first_row) + ",\"HD\")/2)" : "=SUM(COUNTIF(J" + (ctr + start_first_row) + ":AM" + (ctr + start_first_row) + ",\"P\")+COUNTIF(J" + (ctr + start_first_row) + ":AM" + (ctr + start_first_row) + ",\"PH\")+COUNTIF(J" + (ctr + start_first_row) + ":AM" + (ctr + start_first_row) + ",\"HD\")/2)");
                        //string absent_day1 = (days == 31 ? " = SUM(COUNTIF(J" + (ctr + start_first_row) + ":AN" + (ctr + start_first_row) + ",\"A\")+COUNTIF(J" + (ctr + start_first_row) + ":AN" + (ctr + start_first_row) + ",\"HD\")/2)" : (days == 28) ? " = SUM(COUNTIF(J" + (ctr + start_first_row) + ":AK" + (ctr + start_first_row) + ",\"A\")+COUNTIF(J" + (ctr + start_first_row) + ":AK" + (ctr + start_first_row) + ",\"HD\")/2)" : (days == 29) ? " = SUM(COUNTIF(J" + (ctr + start_first_row) + ":AJ" + (ctr + start_first_row) + ",\"A\")+COUNTIF(H" + (ctr + start_first_row) + ":AJ" + (ctr + start_first_row) + ",\"HD\")/2)" : "=SUM(COUNTIF(H" + (ctr + start_first_row) + ":AJ" + (ctr + start_first_row) + ",\"A\")+COUNTIF(J" + (ctr + start_first_row) + ":AM" + (ctr + start_first_row) + ",\"HD\")/2)");


                        if (start_cnt == 0)
                        {
                            start_cnt = ctr + start_first_row;
                            row_cnt = 0;
                        }
                        else
                        {
                            //start_cnt = end_cnt + 1;
                            row_cnt = row_cnt + 1;
                        }
                        string present_day1 = "", absent_day1 = "";
                        if (days == 31)
                        {
                            present_day1 = (days == 31 ? " = SUM(COUNTIF(J" + (start_cnt + row_cnt) + ":AN" + (start_cnt + row_cnt) + ",\"P\")+COUNTIF(J" + (start_cnt + row_cnt) + ":AN" + (start_cnt + row_cnt) + ",\"PH\")+COUNTIF(J" + (start_cnt + row_cnt) + ":AN" + (start_cnt + row_cnt) + ",\"HD\")/2)" : (days == 28) ? " = SUM(COUNTIF(J" + (start_cnt + row_cnt) + ":AK" + (start_cnt + row_cnt) + ",\"P\")+COUNTIF(J" + (start_cnt + row_cnt) + ":AK" + (start_cnt + row_cnt) + ",\"PH\")+COUNTIF(H" + (start_cnt + row_cnt) + ":AI" + (start_cnt + row_cnt) + ",\"HD\")/2)" : (days == 29) ? " = SUM(COUNTIF(H" + (start_cnt + row_cnt) + ":AJ" + (start_cnt + row_cnt) + ",\"P\")+COUNTIF(J" + (start_cnt + row_cnt) + ":AL" + (start_cnt + row_cnt) + ",\"PH\")+COUNTIF(J" + (start_cnt + row_cnt) + ":AL" + (start_cnt + row_cnt) + ",\"HD\")/2)" : "=SUM(COUNTIF(J" + (start_cnt + row_cnt) + ":AM" + (start_cnt + row_cnt) + ",\"P\")+COUNTIF(J" + (start_cnt + row_cnt) + ":AM" + (start_cnt + row_cnt) + ",\"PH\")+COUNTIF(J" + (start_cnt + row_cnt) + ":AM" + (start_cnt + row_cnt) + ",\"HD\")/2)");
                            absent_day1 = (days == 31 ? " = SUM(COUNTIF(J" + (start_cnt + row_cnt) + ":AN" + (start_cnt + row_cnt) + ",\"A\")+COUNTIF(J" + (start_cnt + row_cnt) + ":AN" + (start_cnt + row_cnt) + ",\"HD\")/2)" : (days == 28) ? " = SUM(COUNTIF(J" + (start_cnt + row_cnt) + ":AK" + (start_cnt + row_cnt) + ",\"A\")+COUNTIF(J" + (start_cnt + row_cnt) + ":AK" + (start_cnt + row_cnt) + ",\"HD\")/2)" : (days == 29) ? " = SUM(COUNTIF(J" + (start_cnt + row_cnt) + ":AJ" + (start_cnt + row_cnt) + ",\"A\")+COUNTIF(H" + (start_cnt + row_cnt) + ":AJ" + (start_cnt + row_cnt) + ",\"HD\")/2)" : "=SUM(COUNTIF(H" + (start_cnt + row_cnt) + ":AJ" + (start_cnt + row_cnt) + ",\"A\")+COUNTIF(J" + (start_cnt + row_cnt) + ":AM" + (start_cnt + row_cnt) + ",\"HD\")/2)");

                        }
                        else
                        {
                            present_day1 = (days == 31 ? " = SUM(COUNTIF(J" + (ctr + start_first_row) + ":AN" + (ctr + start_first_row) + ",\"P\")+COUNTIF(J" + (ctr + start_first_row) + ":AN" + (ctr + start_first_row) + ",\"PH\")+COUNTIF(J" + (ctr + start_first_row) + ":AN" + (ctr + start_first_row) + ",\"HD\")/2)" : (days == 28) ? " = SUM(COUNTIF(J" + (ctr + start_first_row) + ":AK" + (ctr + start_first_row) + ",\"P\")+COUNTIF(J" + (ctr + start_first_row) + ":AK" + (ctr + start_first_row) + ",\"PH\")+COUNTIF(H" + (ctr + start_first_row) + ":AI" + (ctr + start_first_row) + ",\"HD\")/2)" : (days == 29) ? " = SUM(COUNTIF(H" + (ctr + start_first_row) + ":AJ" + (ctr + start_first_row) + ",\"P\")+COUNTIF(J" + (ctr + start_first_row) + ":AL" + (ctr + start_first_row) + ",\"PH\")+COUNTIF(J" + (ctr + start_first_row) + ":AL" + (ctr + start_first_row) + ",\"HD\")/2)" : "=SUM(COUNTIF(J" + (ctr + start_first_row) + ":AM" + (ctr + start_first_row) + ",\"P\")+COUNTIF(J" + (ctr + start_first_row) + ":AM" + (ctr + start_first_row) + ",\"PH\")+COUNTIF(J" + (ctr + start_first_row) + ":AM" + (ctr + start_first_row) + ",\"HD\")/2)");
                            absent_day1 = (days == 31 ? " = SUM(COUNTIF(J" + (ctr + start_first_row) + ":AN" + (ctr + start_first_row) + ",\"A\")+COUNTIF(J" + (ctr + start_first_row) + ":AN" + (ctr + start_first_row) + ",\"HD\")/2)" : (days == 28) ? " = SUM(COUNTIF(J" + (ctr + start_first_row) + ":AK" + (ctr + start_first_row) + ",\"A\")+COUNTIF(J" + (ctr + start_first_row) + ":AK" + (ctr + start_first_row) + ",\"HD\")/2)" : (days == 29) ? " = SUM(COUNTIF(J" + (ctr + start_first_row) + ":AJ" + (ctr + start_first_row) + ",\"A\")+COUNTIF(H" + (ctr + start_first_row) + ":AJ" + (ctr + start_first_row) + ",\"HD\")/2)" : "=SUM(COUNTIF(H" + (ctr + start_first_row) + ":AJ" + (ctr + start_first_row) + ",\"A\")+COUNTIF(J" + (ctr + start_first_row) + ":AM" + (ctr + start_first_row) + ",\"HD\")/2)");

                        }


                        //int absent = Convert.ToInt32 (bodystr.Contains("A"));

                        if (state_name != ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper())
                        {
                            if (state_name != "")
                            {
                                end_cnt = start_cnt + row_cnt - 1;
                                int col_span = (days + 9);
                                if (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM")
                                {
                                    col_span = col_span + 1;
                                }

                                if (days == 31)
                                {
                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + col_span + ">Total</td><td colspan=1>=SUM(AO" + start_cnt + ":AO" + end_cnt + ")</td><td colspan=1>=SUM(AP" + start_cnt + ":AP" + end_cnt + ")</td><td colspan=1>=SUM(AQ" + start_cnt + ":AQ" + end_cnt + ")</td></b></tr>";
                                }
                                else
                                {
                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + col_span + ">Total</td><td colspan=1>" + present_days1 + "</td><td colspan=1>" + absent_days1 + "</td><td colspan=1>" + total_days1 + "</td></b></tr>";
                                }

                                //=SUM("AO" + start_cnt + ":AO" + end_cnt + ")
                                // start_cnt=0;
                                ctr1 = ctr1 + 1;
                                start_cnt = end_cnt + 2;
                                row_cnt = 0;
                                present_days1 = 0; absent_days = 0; total_days1 = 0;
                                if (days == 31)
                                {
                                    present_day1 = (days == 31 ? " = SUM(COUNTIF(J" + (start_cnt) + ":AN" + (start_cnt) + ",\"P\")+COUNTIF(J" + (start_cnt) + ":AN" + (start_cnt) + ",\"PH\")+COUNTIF(J" + (start_cnt) + ":AN" + (start_cnt) + ",\"HD\")/2)" : (days == 28) ? " = SUM(COUNTIF(J" + (start_cnt) + ":AK" + (start_cnt) + ",\"P\")+COUNTIF(J" + (start_cnt) + ":AK" + (start_cnt) + ",\"PH\")+COUNTIF(H" + (start_cnt) + ":AI" + (start_cnt) + ",\"HD\")/2)" : (days == 29) ? " = SUM(COUNTIF(H" + (start_cnt) + ":AJ" + (start_cnt) + ",\"P\")+COUNTIF(J" + (start_cnt) + ":AL" + (start_cnt) + ",\"PH\")+COUNTIF(J" + (start_cnt) + ":AL" + (start_cnt) + ",\"HD\")/2)" : "=SUM(COUNTIF(J" + (start_cnt) + ":AM" + (start_cnt) + ",\"P\")+COUNTIF(J" + (start_cnt) + ":AM" + (start_cnt) + ",\"PH\")+COUNTIF(J" + (start_cnt) + ":AM" + (start_cnt) + ",\"HD\")/2)");
                                    absent_day1 = (days == 31 ? " = SUM(COUNTIF(J" + (start_cnt) + ":AN" + (start_cnt) + ",\"A\")+COUNTIF(J" + (start_cnt) + ":AN" + (start_cnt) + ",\"HD\")/2)" : (days == 28) ? " = SUM(COUNTIF(J" + (start_cnt) + ":AK" + (start_cnt) + ",\"A\")+COUNTIF(J" + (start_cnt) + ":AK" + (start_cnt) + ",\"HD\")/2)" : (days == 29) ? " = SUM(COUNTIF(J" + (start_cnt) + ":AJ" + (start_cnt) + ",\"A\")+COUNTIF(H" + (start_cnt) + ":AJ" + (start_cnt) + ",\"HD\")/2)" : "=SUM(COUNTIF(H" + (start_cnt) + ":AJ" + (start_cnt) + ",\"A\")+COUNTIF(J" + (start_cnt) + ":AM" + (start_cnt) + ",\"HD\")/2)");
                                }

                                total_days1 = 0;
                                state_change = 1;
                            }
                            state_name = ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper();
                        }

                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td>" + (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM" ? "<td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td>" : "") + "<td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["ot_hours"].ToString().ToUpper() + "</td>" + bodystr + "<td>" + (month_days == 0 ? ds.Tables[0].Rows[ctr]["tot_days_present"] : present_day1) + "</td><td>" + (month_days == 0 ? count.ToString() : absent_day1) + "</td><td>" + (month_days == 0 ? ds.Tables[0].Rows[ctr]["total days"].ToString() : month_days.ToString()) + "</td>" + (type_cl == 1 || type_cl == 0 ? "<td>" + ds.Tables[0].Rows[ctr]["STATUS"].ToString() + "</td>" : "") + "</tr>");
                        if (month_days == 0)
                        {
                            present_days = present_days + double.Parse(ds.Tables[0].Rows[ctr]["tot_days_present"].ToString());
                            absent_days = absent_days + count;
                            total_days = total_days + double.Parse(ds.Tables[0].Rows[ctr]["total days"].ToString());

                            present_days1 = present_days1 + double.Parse(ds.Tables[0].Rows[ctr]["tot_days_present"].ToString());
                            absent_days1 = absent_days1 + count;
                            total_days1 = total_days1 + double.Parse(ds.Tables[0].Rows[ctr]["total days"].ToString());


                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                int col_span = (int.Parse(ds.Tables[0].Rows[ctr]["total days"].ToString()) + 9);
                                if (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "BAGICTM")
                                {
                                    col_span = col_span + 1;
                                }
                                lc.Text = lc.Text + "<tr><b><td align=center colspan=" + col_span + ">Total</td><td colspan=1>" + present_days1 + "</td><td colspan=1>" + absent_days1 + "</td><td colspan=1>" + total_days1 + "</td></b></tr>";
                                present_days1 = 0;
                                absent_days1 = 0;
                                total_days1 = 0;
                                if (state_name_ddl.Equals("ALL"))
                                {
                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=" + col_span + ">Grand Total</td><td colspan=1>" + present_days + "</td><td colspan=1>" + absent_days + "</td><td colspan=1>" + total_days + "</td></b></tr>";
                                }
                            }
                        }
                        bodystr = "";
                    }
                    #region
                    else if (i == 4)
                    {
                        if (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "UTKARSH")
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + invoice + "</td><td>" + bill_date + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_gst"].ToString().ToUpper() + "</td><td>INTERNATIONAL HOUSEKEEPING & MAINTENANCE SERVICES</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))) + double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + ((double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()))), 2) + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td></tr>");


                            grand_tot = grand_tot + ((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString())));
                            cgst = cgst + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()));
                            sgst = sgst + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()));
                            igst = igst + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()));

                            ctc = ctc + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))) + (double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString())) + double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["uniform"].ToString()))), 2);
                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                lc.Text = lc.Text + "<tr><b><td align=center colspan=8>Total</td><td>" + Math.Round(grand_tot, 2) + "</td><td>" + cgst + "</td><td>" + sgst + "</td><td>" + igst + "</td><td>" + Math.Ceiling(Math.Round(ctc, 2)) + "</td></b></tr>";
                            }
                        }
                        else if (ds.Tables[0].Rows[ctr]["client_code"].ToString() == "MAX")
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"].ToString().ToUpper() + "</td><td>" + int.Parse(ds.Tables[0].Rows[ctr]["emp_count"].ToString()) + "</td><td>" + int.Parse(ds.Tables[0].Rows[ctr]["Present_Days"].ToString()) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["grand_total"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()), 2) + "</td><td>" + invoice + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString() + "</td></tr>");

                        }
                    }
                    if (i == 5)
                    {
                        bodystr = "";
                        int start_first_row = 3;
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["actual_basic"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["actual_vda"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["emp_basic_vda"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bonus_rate"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["washing"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["travelling"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["education"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["allowances"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["cca_billing"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["other_allow"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bonus_gross"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["leave_gross"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["gratuity_gross"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["hra"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["special_allowance"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>=ROUND((I" + (ctr + start_first_row) + "+K" + (ctr + start_first_row) + "+L" + (ctr + start_first_row) + "+M" + (ctr + start_first_row) + "+N" + (ctr + start_first_row) + "+O" + (ctr + start_first_row) + "+P" + (ctr + start_first_row) + "+Q" + (ctr + start_first_row) + "+R" + (ctr + start_first_row) + "+S" + (ctr + start_first_row) + "+T" + (ctr + start_first_row) + "+U" + (ctr + start_first_row) + "),2)</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bonus_after_gross"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["leave_after_gross"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["gratuity_after_gross"].ToString()), 2) + "</td>";



                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["NH"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bill_pf"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["esic_amount"].ToString()), 2) + "</td>";


                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["uniform_gross"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["monthlwf"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["operational_gross"].ToString()), 2) + "</td>";


                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["allowances_after_gross"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>=ROUND(SUM(V" + (ctr + start_first_row) + ":AG" + (ctr + start_first_row) + "),2)</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["esi_on_ot_amount"].ToString()), 2) + "</td>";


                        bodystr = bodystr + "<td>" + ds.Tables[0].Rows[ctr]["hours"] + "</td>";

                        bodystr = bodystr + "<td>=ROUND(SUM(AI" + (ctr + start_first_row) + ":AJ" + (ctr + start_first_row) + "),2)</td>";

                        bodystr = bodystr + "<td>=ROUND(AH" + (ctr + start_first_row) + ",2)</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["relieving_charg"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>=ROUND(AM" + (ctr + start_first_row) + ",2)</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["uniform_after_gross"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["operational_after_gross"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>=ROUND(AK" + (ctr + start_first_row) + "*AL" + (ctr + start_first_row) + ",2)</td>";

                        bodystr = bodystr + "<td>=ROUND(AO" + (ctr + start_first_row) + ",2)</td>";

                        if (Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()), 2) == 0)
                        {
                            bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge_amount"].ToString()), 2) + "</td>";
                        }
                        else
                        {
                            bodystr = bodystr + "<td>=ROUND(AS" + (ctr + start_first_row) + "*" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bill_service_charge"].ToString()), 2) + ",2)/100</td>";

                        }


                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString()), 2) + "</td>";



                        bodystr = bodystr + "<td>=ROUND(SUM(AP" + (ctr + start_first_row) + ",AQ" + (ctr + start_first_row) + ",AS" + (ctr + start_first_row) + ",AT" + (ctr + start_first_row) + ",AU" + (ctr + start_first_row) + "),2)</td>";

                        lc = new LiteralControl("<tr>" + bodystr + "</tr>");

                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["billing_state"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["DUTYHRS"] + "</td>" + bodystr + "</tr>");


                        header = "";
                        bodystr = "";
                    }
                    //arrears
                    if (i == 6)
                    {
                        int start_first_row = 4;
                        bodystr = "";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["basic"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["vda"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["emp_basic_vda"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bonus_rate"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["washing"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["travelling"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["education"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["allowances_esic"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["cca_billing"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["other_allow"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bonus_gross"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["leave_gross"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["gratuity_gross"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["hra"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["special_allowance"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>=ROUND(SUM(L" + (ctr + start_first_row) + ":V" + (ctr + start_first_row) + ",J" + (ctr + start_first_row) + "),2)</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["bonus_after_gross"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["leave_after_gross"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["gratuity_after_gross"].ToString()), 2) + "</td>";



                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["NH"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["pf"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["esic"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["uniform_ser"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing_ser"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["lwf"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["operational_cost"].ToString()), 2) + "</td>";


                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["allowances_no_esic"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total_a"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["ot_pr_hr_rate"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["esi_on_ot_amount"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + ds.Tables[0].Rows[ctr]["ot_hours"] + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total_b"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total_ab"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["relieving_charg"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total_c"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["uniform_no_ser"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["operational_cost_no_ser"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total_c"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td>";

                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["group_insurance_billing"].ToString()), 2) + "</td>";
                        bodystr = bodystr + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()), 2) + "</td>";


                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["DUTYHRS"] + "</td><td>" + ds.Tables[0].Rows[ctr]["tot_days_present"] + "</td>" + bodystr + "<td>" + ds.Tables[0].Rows[ctr]["aa"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["year"].ToString().ToUpper() + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan=6>Total</td><td>=ROUND(SUM(G" + start_first_row + ":G" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(H" + start_first_row + ":H" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(I" + start_first_row + ":I" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(R" + start_first_row + ":R" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(S" + start_first_row + ":S" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(T" + start_first_row + ":T" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(U" + start_first_row + ":U" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(V" + start_first_row + ":V" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(W" + start_first_row + ":W" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(X" + start_first_row + ":X" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Y" + start_first_row + ":Y" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Z" + start_first_row + ":Z" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AA" + start_first_row + ":AA" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AB" + start_first_row + ":AB" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AC" + start_first_row + ":AC" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AD" + start_first_row + ":AD" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AE" + start_first_row + ":AE" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AF" + start_first_row + ":AF" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AG3:AG" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AH" + start_first_row + ":AH" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AI" + start_first_row + ":AI" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AJ" + start_first_row + ":AJ" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AK" + start_first_row + ":AK" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AL" + start_first_row + ":AL" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AM" + start_first_row + ":AM" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AN" + start_first_row + ":AN" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AO" + start_first_row + ":AO" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AP" + start_first_row + ":AP" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AQ" + start_first_row + ":AQ" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AR" + start_first_row + ":AR" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AS" + start_first_row + ":AS" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AT" + start_first_row + ":AT" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AU" + start_first_row + ":AU" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AV" + start_first_row + ":AV" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AW" + start_first_row + ":AW" + (ctr + start_first_row) + "),2)</td></b></tr>";
                        }
                        header = "";
                        bodystr = "";
                    }
                    else if (i == 9)
                    {

                        if (ds.Tables[0].Rows[ctr]["client"].ToString().Contains("HDFC"))
                        {

                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"] + "</td><td>" + ds.Tables[0].Rows[ctr]["zonal_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["branch_cost_centre_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"] + "</td><td></td><td>" + ds.Tables[0].Rows[ctr]["ihms"] + "</td><td>" + ds.Tables[0].Rows[ctr]["branch_cost_centre_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["material_area"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"] + "</td><td>" + ds.Tables[0].Rows[ctr]["location_type"] + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_count"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_count1"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_per"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["total_emp_count"] + "</td><td>" + ds.Tables[0].Rows[ctr]["tot_days_present"] + "</td><td>" + ds.Tables[0].Rows[ctr]["TOT_WORKING_DAYS"] + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_hours"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString())) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))) + double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())), 2) + "</td></tr>");

                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                lc.Text = lc.Text + "<tr><b><td align=center colspan=18>Total</td><td>=ROUND(SUM(S3:S" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(T3:T" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(U3:U" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(V3:V" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(W3:W" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(X3:X" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Y3:Y" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Z3:Z" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(AA3:AA" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(AB3:AB" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(AC3:AC" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(AD3:AD" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(AE3:AE" + (ctr + 3) + "),2)</td></b></tr>";

                            }
                        }
                        else
                        {
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["tot_days_present"] + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_hours"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString())) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["ot_amount"].ToString())) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString())) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))) + double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())), 2) + "</td></tr>");
                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                lc.Text = lc.Text + "<tr><b><td align=center colspan=12>Total</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(R3:R" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(S3:S" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(T3:T" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(U3:U" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(V3:V" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(W3:W" + (ctr + 3) + "),2)</td></b></tr>";

                            }

                        }

                    }

                    else if (i == 10)
                    {

                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["client"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"] + "</td><td>" + ds.Tables[0].Rows[ctr]["DUTYHRS"] + "</td><td>" + ds.Tables[0].Rows[ctr]["tot_days_present"] + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["esi_on_ot_amount"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["ot_rate"].ToString())) + double.Parse(ds.Tables[0].Rows[ctr]["esi_on_ot_amount"].ToString())), 2) + "</td><td>" + ds.Tables[0].Rows[ctr]["ot_hours"] + "</td><td>" + ds.Tables[0].Rows[ctr]["ot_amount"] + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())), 2) + "</td></tr>");

                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan=9>Total</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td></b></tr>";

                        }

                    }
                    else if (i == 11)
                    {
                        //string branch = "";


                        //if (ds.Tables[0].Rows[ctr]["branch_type"].ToString() != "0")
                        //{
                        //    branch = "<td>" + ds.Tables[0].Rows[ctr]["branch_type"].ToString().ToUpper() + "</td>";
                        //}


                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["help_req_number"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["utr_number"].ToString().ToUpper() + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td><td>" + ds.Tables[0].Rows[ctr]["payment_date"] + "</td></tr>");

                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 13>Total</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(R3:R" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(S3:S" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(T3:T" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(U3:U" + (ctr + 3) + "),2)</td></b></tr>";

                        }


                    }
                    else if (i == 12)
                    {

                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["utr_number"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["days"].ToString().ToUpper() + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td></tr>");

                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 12>Total</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(R3:R" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(S3:S" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(T3:T" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(U3:U" + (ctr + 3) + "),2)</td></b></tr>";

                        }

                    }
                    else if (i == 13)
                    {
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["grade_desc"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["ot_hours"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["STATUS"].ToString() + "</td></tr>");


                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 8>Total</td><td>=ROUND(SUM(I3:I" + (ctr + 3) + "),2)</td></b></tr>";

                        }

                    }
                    else if (i == 14)
                    {

                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["shift_days"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["shiftwise_rate"].ToString().ToUpper() + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td></tr>");

                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 11>Total</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(R3:R" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(S3:S" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(T3:T" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(U3:U" + (ctr + 3) + "),2)</td></b></tr>";

                        }

                    }
                    else if (i == 15)
                    {

                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["emp_name"].ToString().ToUpper() + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td></tr>");

                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 11>Total</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(R3:R" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(S3:S" + (ctr + 3) + "),2)</td></b></tr>";

                        }

                    }
                    else if (i == 16)
                    {

                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["txt_zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["zone"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_city"].ToString().ToUpper() + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString()))), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td><td>" + Math.Round(((double.Parse(ds.Tables[0].Rows[ctr]["Service_charge"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["Amount"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["CGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["SGST9"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["IGST18"].ToString()))), 2) + "</td></tr>");

                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 10>Total</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(P3:P" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(Q3:Q" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(R3:R" + (ctr + 3) + "),2)</td></b></tr>";

                        }

                    }
                    ctr++;
                    break;
                    #endregion

                case ListItemType.Footer:
                    lc = new LiteralControl("</table>");
                    ctr = 0;
                    break;
            }
            container.Controls.Add(lc);
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
        where = "comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'  and unit_code = '" + ddl_unitcode.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "'";
        if (ddl_billing_state.SelectedValue == "ALL")
        {
            where = "comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "'";
        }
        else if (ddl_unitcode.SelectedValue == "ALL")
        {
            where = "comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'  and unit_code in (Select unit_code from pay_unit_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state.SelectedValue + "') and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "'";
        }


        MySqlCommand cmd = new MySqlCommand("Select state_per from pay_billing_unit_rate_history where client_code = '" + ddl_client.SelectedValue + "' and comp_code = '" + Session["COMP_CODE"].ToString() + "' and unit_code = '" + ddl_unitcode.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' AND Emp_code IS NULL group by state_per", d_cg.con);
        d_cg.con.Open();
        MySqlDataReader dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            string invoice_no = d.getsinglestring("select auto_invoice_no from pay_billing_unit_rate_history where " + where + " and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' and state_per = '" + dr.GetValue(0).ToString() + "' and invoice_flag != 0 ");
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
    protected string get_selected_days(string ddl_start_date_common, string ddl_end_date_common)
    {
        int month = int.Parse(txt_month_year.Text.Substring(0, 2));
        int year = int.Parse(txt_month_year.Text.Substring(3));
        int monthdays = 0;
        monthdays = DateTime.DaysInMonth(year, month);
        string getdays = "";
        //int n = 1;
        if (ddl_start_date_common == "0" || ddl_end_date_common == "0")
        {
            return "";
        }
        for (int n = 1; monthdays >= n; n++)
        {
            if (int.Parse(ddl_start_date_common) <= n && int.Parse(ddl_end_date_common) >= n)
            {
                if (n < 10)
                {
                    getdays = getdays + "pay_attendance_muster.DAY" + "0" + n + " as 'DAY0" + n + "',";
                }
                else { getdays = getdays + "pay_attendance_muster.DAY" + n + " as 'DAY" + n + "',"; }
            }
            else
            {
                if (n < 10)
                {
                    getdays = getdays + "'A'" + " as 'DAY0" + n + "',";
                }
                else { getdays = getdays + "'A'" + " as 'DAY" + n + "',"; }
            }
        }
        return getdays;
    }
    #endregion

    #region FINANCECOPY Material
    protected void material(int i, string ddl_designation, string ddl_material_bill_type, string billing_type, string ddl_invoice_type)
    {
        try
        {
            string material_contract = "";
            string start_date_common = get_start_date();
            DateTimeFormatInfo mfi = new DateTimeFormatInfo();
            month_name = mfi.GetMonthName(int.Parse(txt_month_year.Text.Substring(0, 2))).ToString();
            month_name = month_name + " " + txt_month_year.Text.Substring(3).ToUpper();
            int month = int.Parse(txt_month_year.Text.Substring(0, 2));
            int year = int.Parse(txt_month_year.Text.Substring(3));


            string daterange = "concat(upper(DATE_FORMAT(str_to_date('" + year + "-" + month + "-01','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(LAST_DAY('" + txt_month_year.Text.Substring(3) + "-" + txt_month_year.Text.Substring(0, 2) + "-01'), '%d %b %Y'))) as fromtodate";
            if (start_date_common != "" && start_date_common != "1")
            {
                month = int.Parse(txt_month_year.Text.Substring(0, 2)) - 1;
                if (month == 0) { month = 12; year = year - 1; }
                daterange = "concat(upper(DATE_FORMAT(str_to_date('" + year + "-" + month + "-" + start_date_common + "','%Y-%m-%d'), '%d %b %Y')),' TO ',upper(DATE_FORMAT(str_to_date('" + txt_month_year.Text.Substring(3) + "-" + txt_month_year.Text.Substring(0, 2) + "-" + (int.Parse(start_date_common) - 1) + "','%Y-%m-%d'), '%d %b %Y'))) as fromtodate";


            }
            string invoice = null;
            string bill_date = null;
            string des_grade = ddl_designation;
            //if (ddl_material_bill_type=="2")//2-Tissue Bill
            //{
            //    des_grade = "Tissue";
            //}


            //invoice and bill date 
            string invoice_bill_date = bs.get_invoice_bill_date(Session["COMP_CODE"].ToString(), ddl_client.SelectedValue, ddl_billing_state.SelectedValue, ddl_unitcode.SelectedValue, ddl_invoice_type, des_grade, txt_month_year.Text, i, ddlregion.SelectedValue, billing_type);

            if (invoice_bill_date.Equals(""))
            {
                invoice = "";
                bill_date = "";
            }
            else
            {
                var invoice_bill = invoice_bill_date.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                invoice = invoice_bill[0].ToString();
                bill_date = invoice_bill[1].ToString();
            }

            string query = "";
            string where = "", where_clause = "", where_fix = "", where_emp = "", where_state = "";
            if (ddl_billing_state.SelectedValue.Equals("Maharashtra") && ddl_client.SelectedValue.Equals("BAGIC") && int.Parse("" + txt_month_year.Text.Substring(3) + "" + txt_month_year.Text.Substring(0, 2) + "") > 20204) { where_state = " and state='" + ddl_billing_state.SelectedValue + "' and billingwise_id = 5"; }
            //if (!ddl_billing_state.SelectedValue.Equals("ALL")) { where_state = " and state='" + ddl_billing_state.SelectedValue + "'"; }
            if (d.getsinglestring("select billingwise_id from pay_client_billing_details where  client_code = '" + ddl_client.SelectedValue + "' " + where_state).Equals("5"))
            {
                where_state = " and zone = '" + ddlregion.SelectedValue + "'";
                //material_where_state = " and pay_billing_unit_rate_history.zone = '" + ddlregion.SelectedValue + "'";
            }
            else
            { where_state = ""; }
            if (i == 1)
            {
                if (ddl_client.SelectedValue != "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' " + where_state + " and client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + ddl_unitcode.SelectedValue + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "'  ";
                }
                if (ddl_billing_state.SelectedValue == "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' " + where_state + " and client_code= '" + ddl_client.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' ";
                }
                else if (ddl_unitcode.SelectedValue == "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' " + where_state + " and client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' ";
                }
                if (ddl_client.SelectedValue == "ALL")
                {
                    where = "where comp_code='" + Session["comp_code"].ToString() + "' " + where_state + "  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' ";
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

                query = "SELECT CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date',client,COMP_STATE  as 'STATE', STATE_NAME , UNIT_NAME , EMP_NAME , grade_desc  as 'designation', IF( conveyance_applicable  = 1 AND  conveyance_type  = 2, ( Conveyance_PerKmRate  *  conveyance_km ),  conveyance_rate ) AS 'total', IF( conveyance_service_charge_per  <= 0,  conveyance_service_amount , (IF( conveyance_applicable  = 1 AND  conveyance_type  = 2, ( Conveyance_PerKmRate  *  conveyance_km ),  conveyance_rate ) *  conveyance_service_charge_per ) / 100) AS 'Service_Charge', SGST,  CGST, IGST, unit_code , conveyance_service_charge_per," + daterange + ",client_branch_code,Conveyance_PerKmRate,IF(conveyance_type = 1, (conveyance_rate / Conveyance_PerKmRate), conveyance_km) AS 'conveyance_km' ,conveyance_type FROM pay_billing_material_history " + where;

            }
            //material finance copy
            else if (i == 2)
            {
                if (ddl_material_bill_type == "2")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and type ='Material' and material_type='Tissue'  ";

                    query = " select comp_code,   auto_invoice_no,invoice_no,DATE_FORMAT(billing_date, '%d/%m/%Y') as billing_date, client_code,client,COMP_STATE, state_name,  unit_code, month, year, COMP_ADDRESS1, COMP_ADDRESS2, COMP_CITY, PF_REG_NO, COMPANY_PAN_NO, COMPANY_TAN_NO, COMPANY_CIN_NO, SERVICE_TAX_REG_NO, ESIC_REG_NO, unit_gst_no, COMPANY_NAME, UNIT_full_ADD1, UNIT_ADD2, EMP_TYPE, housekeeiing_sac_code, Security_sac_code, GRADE_CODE, material_contract, contract_type, cgst, sgst, igst,(cgst+sgst+igst) as total_gst, material_type, tissue_qty, tissue_rate, Round( (tissue_qty*tissue_rate) ,2) as amount,  Round(((tissue_qty*tissue_rate)+ cgst+sgst+igst) ,2) as billingamt from pay_billing_material_history where " + where;

                }
                else
                {
                    where_fix = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_material_history.unit_code = '" + ddl_unitcode.SelectedValue + "' AND pay_billing_material_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' AND pay_billing_material_history.Year = '" + txt_month_year.Text.Substring(3) + "' ";
                    where_clause = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and unit_code='" + ddl_unitcode.SelectedValue + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "'  ";
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and unit_code = '" + ddl_unitcode.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0  and material_contract != 0 AND grade_code in ('HK','HKSR','CT') GROUP BY unit_code,designation  ORDER BY STATE_NAME, UNIT_NAME ";
                    where_emp = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_material_history.unit_code = '" + ddl_unitcode.SelectedValue + "' and pay_billing_material_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0  and material_contract != 0 and grade_code = 'HK' AND pay_material_details.material_flag = '2' GROUP BY unit_code,pay_billing_material_history.emp_code,designation  ORDER BY STATE_NAME, UNIT_NAME ";
                    if (ddl_billing_state.SelectedValue == "ALL")
                    {
                        where_fix = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' AND pay_billing_material_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' AND pay_billing_material_history.Year = '" + txt_month_year.Text.Substring(3) + "' ";
                        where_clause = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' ";
                        where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0 and material_contract != 0 AND grade_code in ('HK','HKSR','CT') GROUP BY unit_code,designation ORDER BY STATE_NAME, UNIT_NAME ";
                        where_emp = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "'  and pay_billing_material_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0  and material_contract != 0 and grade_code = 'HK' AND pay_material_details.material_flag = '2' GROUP BY unit_code,pay_billing_material_history.emp_code,designation  ORDER BY STATE_NAME, UNIT_NAME ";
                    }
                    else if (ddl_unitcode.SelectedValue == "ALL")
                    {
                        where_fix = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "'  AND pay_billing_material_history.state_name = '" + ddl_billing_state.SelectedValue + "' AND pay_billing_material_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' AND pay_billing_material_history.Year = '" + txt_month_year.Text.Substring(3) + "' ";
                        where_clause = "where comp_code='" + Session["comp_code"].ToString() + "' and client_code= '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' ";
                        where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0 and material_contract != 0 AND grade_code in ('HK','HKSR','CT') GROUP BY unit_code,designation ORDER BY STATE_NAME, UNIT_NAME ";
                        where_emp = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_material_history.state_name = '" + ddl_billing_state.SelectedValue + "' and pay_billing_material_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0  and material_contract != 0 and grade_code = 'HK' AND pay_material_details.material_flag = '2' GROUP BY unit_code,pay_billing_material_history.emp_code,designation  ORDER BY STATE_NAME, UNIT_NAME ";
                    }


                    material_contract = d.getsinglestring("select max(material_contract) from pay_billing_material_history   " + where_clause + " limit  1").ToString();

                    if (material_contract.Equals("3"))//3-Fix Material
                    {
                        //query = "SELECT CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND pay_billing_material_history.month >= 4 AND pay_billing_material_history.year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE '' END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', material_contract, contract_type, COMP_STATE AS 'STATE', pay_billing_material_history.unit_code, pay_billing_material_history.fromtodate, pay_billing_material_history.STATE_NAME, pay_billing_material_history.CLIENT_CODE, UNIT_NAME, Client_branch_code, grade_desc AS 'designation', material_name, rate, quantity, ROUND(rate * quantity, 2) AS 'total', CASE WHEN pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0 THEN ROUND((((rate * quantity) * pay_material_billing_details.handling_percent) / 100), 2) WHEN pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_charges_amount > 0 THEN pay_billing_material_history.handling_charges_amount ELSE 0 END AS 'handling_charge', pay_material_billing_details.handling_percent, round(IF(gst_applicable = 1 AND LOCATE(COMP_STATE, STATE_NAME), IF(material_contract = 3, (((rate * quantity) + IF(material_contract = 3 AND pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0, (((rate * quantity) * pay_material_billing_details.handling_percent) / 100), pay_billing_material_history.handling_charges_amount)) * 9) / 100, 0), 0),2) AS 'SGST', round(IF(gst_applicable = 1 AND LOCATE(COMP_STATE, STATE_NAME), IF(material_contract = 3, (((rate * quantity) + IF(material_contract = 3 AND pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0, (((rate * quantity) * pay_material_billing_details.handling_percent) / 100), pay_billing_material_history.handling_charges_amount)) * 9) / 100, 0), 0),2) AS 'CGST', round(IF(gst_applicable = 1 AND LOCATE(COMP_STATE, STATE_NAME) != 1, IF(material_contract = 3, (((rate * quantity) + IF(material_contract = 3 AND pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0, (((rate * quantity) * pay_material_billing_details.handling_percent) / 100), pay_billing_material_history.handling_charges_amount)) * 18) / 100, 0), 0),2) AS 'IGST' from  pay_billing_material_history INNER JOIN pay_material_billing_details ON pay_billing_material_history.comp_code = pay_material_billing_details.comp_Code AND pay_billing_material_history.client_code = pay_material_billing_details.client_code AND pay_billing_material_history.state_name = pay_material_billing_details.state AND pay_billing_material_history.unit_code = pay_material_billing_details.unit_code1 AND pay_billing_material_history.month = pay_material_billing_details.month AND pay_billing_material_history.year = pay_material_billing_details.year WHERE " + where_fix + " AND pay_billing_material_history.tot_days_present > 0 AND pay_billing_material_history.material_contract = 3 AND grade_code = 'HK' GROUP BY pay_billing_material_history.unit_code, Id_material ORDER BY UNIT_NAME  ";
                        query = " SELECT CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND pay_billing_material_history.month >= 4 AND pay_billing_material_history.year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE '' END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', material_contract, contract_type, COMP_STATE AS 'STATE', pay_billing_material_history.unit_code, pay_billing_material_history.fromtodate, pay_billing_material_history.STATE_NAME, pay_billing_material_history.CLIENT_CODE, UNIT_NAME, Client_branch_code, grade_desc AS 'designation', material_name, rate, quantity, ROUND(rate * quantity, 2) AS 'total', CASE WHEN pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_percent > 0 THEN ROUND((((rate * quantity) * pay_material_billing_details.handling_percent) / 100), 2) WHEN pay_material_billing_details.handling_applicable = 1 AND pay_material_billing_details.handling_charges_amount > 0 THEN pay_billing_material_history.handling_charges_amount ELSE 0 END AS 'handling_charge', pay_material_billing_details.handling_percent, SGST, CGST, IGST from  pay_billing_material_history INNER JOIN pay_material_billing_details ON pay_billing_material_history.comp_code = pay_material_billing_details.comp_Code AND pay_billing_material_history.client_code = pay_material_billing_details.client_code AND pay_billing_material_history.state_name = pay_material_billing_details.state AND pay_billing_material_history.unit_code = pay_material_billing_details.unit_code1 AND pay_billing_material_history.month = pay_material_billing_details.month AND pay_billing_material_history.year = pay_material_billing_details.year WHERE " + where_fix + " AND pay_billing_material_history.tot_days_present > 0 AND pay_billing_material_history.material_contract = 3 AND grade_code = 'HK' GROUP BY pay_billing_material_history.unit_code, Id_material ORDER BY UNIT_NAME  ";
                    }
                    else if (material_contract.Equals("4"))//4-Employeewise
                    {
                        query = "SELECT material_contract, CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', contract_type, COMP_STATE AS 'STATE', pay_billing_material_history.unit_code, pay_billing_material_history.STATE_NAME, pay_billing_material_history.CLIENT_CODE, UNIT_NAME, Client_branch_code, grade_desc AS 'designation', pay_billing_material_history.handling_percent, material_area, IF(material_contract != 0, contract_amount, 0) AS 'rate', IF(material_contract != 0 AND contract_type = 2, ROUND(contract_amount * material_area, 2), ROUND(contract_amount, 2)) AS 'sub_total', IF(material_contract != 0 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) AS 'total', IF(pay_billing_material_history.handling_applicable = 1, IF(pay_billing_material_history.handling_charges_amount > 0, pay_billing_material_history.handling_charges_amount, (IF(material_contract != 0 AND contract_type = 2, ROUND((((contract_amount * material_area) + machine_rental_amount) * pay_billing_material_history.handling_percent) / 100, 2), (ROUND((contract_amount + machine_rental_amount), 2) * pay_billing_material_history.handling_percent) / 100))), 0) AS 'handling_charge', CGST, SGST, IGST, machine_rental_amount, machine_rental_applicable, fromtodate FROM pay_billing_material_history INNER JOIN pay_material_details ON pay_billing_material_history.comp_code = pay_material_details.comp_code AND pay_billing_material_history.client_code = pay_material_details.client_code AND pay_billing_material_history.emp_code = pay_material_details.emp_code AND pay_billing_material_history.month = pay_material_details.month AND pay_billing_material_history.year = pay_material_details.year WHERE " + where_emp;
                    }
                    else
                    {
                        query = "SELECT material_contract,CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no',CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date',contract_type,COMP_STATE AS 'STATE',unit_code,STATE_NAME,CLIENT_CODE,UNIT_NAME,Client_branch_code,grade_desc AS 'designation',handling_percent,material_area,IF(material_contract != 0, contract_amount, 0) AS 'rate',IF(material_contract != 0 AND contract_type = 2, ROUND(contract_amount * material_area, 2), ROUND(contract_amount, 2)) AS 'sub_total',IF(material_contract != 0 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) AS 'total',  IF(handling_applicable = 1, IF(handling_charges_amount > 0, handling_charges_amount, (IF(material_contract != 0 AND contract_type = 2, ROUND((((contract_amount * material_area) + machine_rental_amount) * handling_percent) / 100, 2), (ROUND((contract_amount+machine_rental_amount), 2) * handling_percent) / 100))), 0) AS 'handling_charge',CGST,SGST,IGST,machine_rental_amount,machine_rental_applicable,fromtodate FROM pay_billing_material_history WHERE " + where;
                        //query = "SELECT material_contract, CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND pay_billing_material_history.month >= 4 AND pay_billing_material_history.year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE '' END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', contract_type, COMP_STATE AS 'STATE', unit_code, STATE_NAME, CLIENT_CODE, UNIT_NAME, Client_branch_code, grade_desc AS 'designation', handling_percent, material_area, IF(material_contract != 0, contract_amount, 0) AS 'rate', IF(material_contract = 2, ROUND(contract_amount * material_area, 2), ROUND(contract_amount, 2)) AS 'sub_total', IF(material_contract = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) AS 'total', IF(handling_applicable = 1, (IF(material_contract = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0) AS 'handling_charge', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME), ((IF(material_contract = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 9) / 100, 0) AS 'SGST', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME), ((IF(material_contract = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 9) / 100, 0) AS 'CGST', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME) != 1, ((IF(material_contract = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 18) / 100, 0) AS 'IGST', machine_rental_amount, machine_rental_applicable, fromtodate FROM pay_billing_material_history WHERE " + where;
                        //  query = "SELECT material_contract,CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(invoice_no, '') WHEN invoice_flag != 0 AND pay_billing_material_history.month >= 4 AND pay_billing_material_history.year >= 2019 THEN IFNULL(auto_invoice_no, '') ELSE '' END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', contract_type,COMP_STATE AS 'STATE',unit_code, STATE_NAME, CLIENT_CODE, UNIT_NAME, Client_branch_code, grade_desc AS 'designation', handling_percent, material_area, IF(material_contract = 2, contract_amount, 0) AS 'rate', IF(material_contract = 2 AND contract_type = 2, ROUND(contract_amount * material_area, 2), ROUND(contract_amount, 2)) AS 'sub_total', IF(material_contract = 2 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) AS 'total', IF(handling_applicable = 2, (IF(material_contract = 1 AND contract_type = 2, ROUND(((contract_amount * material_area) / month_days), 2), (ROUND(contract_amount, 2) * handling_percent) / 100)), 0) AS 'handling_charge', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME), ((IF(material_contract = 2 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 1 AND contract_type = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 9) / 100, 0) AS 'SGST', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME), ((IF(material_contract = 2 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 1 AND contract_type = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 9) / 100, 0) AS 'CGST', IF(gst_applicable = 0 AND LOCATE(COMP_STATE, STATE_NAME) != 1, ((IF(material_contract = 2 AND contract_type = 2, ROUND(((contract_amount * material_area) + machine_rental_amount), 2), ROUND(contract_amount + machine_rental_amount, 2)) + IF(handling_applicable = 1, (IF(material_contract = 1 AND contract_type = 2, ROUND(((contract_amount * material_area)), 2), ROUND(contract_amount, 2)) * handling_percent) / 100, 0)) * 18) / 100, 0) AS 'IGST', machine_rental_amount, machine_rental_applicable, fromtodate FROM pay_billing_material_history WHERE " + where;
                    }
                }

            }
            //Deep Clean finance copy
            else if (i == 3)
            {
                if (ddl_client.SelectedValue != "ALL")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and unit_code = '" + ddl_unitcode.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0 AND dc_contract = 1  GROUP BY unit_code, designation  ORDER BY STATE_NAME, UNIT_NAME";
                }
                if (ddl_billing_state.SelectedValue == "ALL")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0 AND dc_contract = 1 GROUP BY unit_code, designation ORDER BY STATE_NAME, UNIT_NAME ";
                }
                else if (ddl_unitcode.SelectedValue == "ALL")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0 AND dc_contract = 1 GROUP BY unit_code, designation ORDER BY STATE_NAME, UNIT_NAME ";
                }
                if (ddl_client.SelectedValue == "ALL")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "'  and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0 AND dc_contract = 1  GROUP BY unit_code, designation  ORDER BY client,STATE_NAME, UNIT_NAME";
                }
                query = "SELECT  CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', COMP_STATE  AS 'STATE',client,  STATE_NAME ,  UNIT_NAME ,  Client_branch_code ,  grade_desc  AS 'designation',  dc_handling_percent ,  dc_rate , IF( dc_contract  = 1 AND  dc_type  = 2, ( dc_rate  *  dc_area ),  dc_rate ) AS 'total', IF( dc_handling_charge  = 1, (IF( dc_contract  = 1 AND  dc_type  = 2, ( dc_rate  *  dc_area ),  dc_rate ) *  dc_handling_percent ) / 100, 0) AS 'handling_charge', SGST,CGST, IGST FROM  pay_billing_material_history  WHERE " + where;
            }
            //Pest Control finance copy
            else if (i == 4)
            {
                where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and unit_code = '" + ddl_unitcode.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0 AND pc_contract = 1  GROUP BY unit_code, designation  ORDER BY STATE_NAME, UNIT_NAME ";
                if (ddl_billing_state.SelectedValue == "ALL")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0 AND pc_contract = 1 GROUP BY unit_code, designation ORDER BY STATE_NAME, UNIT_NAME ";
                }
                else if (ddl_unitcode.SelectedValue == "ALL")
                {
                    where = " comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and Year = '" + txt_month_year.Text.Substring(3) + "' and tot_days_present > 0 AND pc_contract = 1 GROUP BY unit_code, designation ORDER BY STATE_NAME, UNIT_NAME ";
                }
                query = "SELECT CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', COMP_STATE  AS 'STATE', STATE_NAME , UNIT_NAME , Client_branch_code , grade_desc  AS 'designation', pc_handling_percent , IF( pc_contract  = 1 AND  pc_type  = 2, ( pc_rate  *  pc_area ),  pc_rate ) AS 'total', IF( pc_handling_charge  = 1, (IF( pc_contract  = 1 AND  pc_type  = 2, ( pc_rate  *  pc_area ),  pc_rate ) *  pc_handling_percent ) / 100, 0) AS 'handling_charge', IF( gst_applicable  = 1 AND LOCATE( COMP_STATE ,  STATE_NAME ), ((IF( pc_contract  = 1 AND  pc_type  = 2, ( pc_rate  *  pc_area ),  pc_rate ) + IF( pc_handling_charge  = 1, (IF( pc_contract  = 1 AND  pc_type  = 2, ( pc_rate  *  pc_area ),  pc_rate ) *  pc_handling_percent ) / 100, 0)) * 9) / 100, 0) AS 'SGST', IF( gst_applicable  = 1 AND LOCATE( COMP_STATE ,  STATE_NAME ), ((IF( pc_contract  = 1 AND  pc_type  = 2, ( pc_rate  *  pc_area ),  pc_rate ) + IF( pc_handling_charge  = 1, (IF( pc_contract  = 1 AND  pc_type  = 2, ( pc_rate  *  pc_area ),  pc_rate ) *  pc_handling_percent ) / 100, 0)) * 9) / 100, 0) AS 'CGST', IF( gst_applicable  = 1 AND LOCATE( COMP_STATE ,  STATE_NAME ) != 1, ((IF( pc_contract  = 1 AND  pc_type  = 2, ( pc_rate  *  pc_area ),  pc_rate ) + IF( pc_handling_charge  = 1, (IF( pc_contract  = 1 AND  pc_type  = 2, ( pc_rate  *  pc_area ),  pc_rate ) *  pc_handling_percent ) / 100, 0)) * 18) / 100, 0) AS 'IGST',pc_rate,pc_area   FROM  pay_billing_material_history  WHERE  " + where;
            }
            // Conveyance
            else if (i == 5)
            {
                if (ddl_client.SelectedValue != "ALL")
                {
                    where = " pay_transaction.comp_code = '" + Session["comp_code"].ToString() + "' and pay_transaction.CUST_CODE = '" + ddl_client.SelectedValue + "' and pay_transaction.state = '" + ddl_billing_state.SelectedValue + "' and pay_transaction.branch_name = '" + ddl_unitcode.SelectedValue + "' and  pay_transaction.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_transaction.year = '" + txt_month_year.Text.Substring(3) + "' ";
                }

                if (ddl_unitcode.SelectedValue == "ALL")
                {
                    where = " pay_transaction.comp_code = '" + Session["comp_code"].ToString() + "' and pay_transaction.CUST_CODE = '" + ddl_client.SelectedValue + "' and pay_transaction.state = '" + ddl_billing_state.SelectedValue + "' and pay_transaction.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_transaction.year = '" + txt_month_year.Text.Substring(3) + "'";
                }
                if (ddl_client.SelectedValue == "ALL")
                {
                    where = " pay_transaction.comp_code = '" + Session["comp_code"].ToString() + "' and pay_transaction.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_transaction.year = '" + txt_month_year.Text.Substring(3) + "'";
                }
                query = "SELECT  CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no', CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date', client, pay_transaction.state, (SELECT UNIT_NAME FROM pay_unit_master WHERE unit_code = pay_transaction.branch_name AND comp_code = pay_transaction.COMP_CODE) AS 'BRANCH', (SELECT ITEM_NAME FROM pay_item_master WHERE ITEM_CODE = pay_transaction_details.ITEM_CODE AND comp_code = pay_transaction_details.COMP_CODE) AS 'ITEMS', RATE, QUANTITY, pay_transaction_details.DESIGNATION AS 'UNIT', AMOUNT,  pay_transaction_details.Vat AS 'GST %', (AMOUNT * pay_transaction_details.Vat) / 100 AS 'GST AMOUNT', (AMOUNT + (AMOUNT * pay_transaction_details.Vat) / 100) AS 'TOTAL' FROM pay_transaction INNER JOIN pay_transaction_details ON pay_transaction.DOC_NO = pay_transaction_details.DOC_NO AND pay_transaction.COMP_CODE = pay_transaction_details.COMP_CODE INNER JOIN pay_client_master on  pay_client_master.client_code = pay_transaction.CUST_CODE WHERE " + where;
            }
            // Driver Conveyance
            else if (i == 6)
            {
                if (ddl_client.SelectedValue != "ALL")
                {
                    where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_material_history.unit_code = '" + ddl_unitcode.SelectedValue + "' and pay_billing_material_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_month_year.Text.Substring(3) + "'  and pay_billing_material_history.conveyance_type = 100  ORDER BY state_name,unit_name,emp_name ";
                }
                if (ddl_billing_state.SelectedValue == "ALL")
                {
                    where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_material_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_month_year.Text.Substring(3) + "'  and pay_billing_material_history.conveyance_type = 100 ORDER BY STATE_NAME, UNIT_NAME  ";
                }
                else if (ddl_unitcode.SelectedValue == "ALL")
                {
                    where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' " + where_state + " and pay_billing_material_history.client_code = '" + ddl_client.SelectedValue + "' and pay_billing_material_history.state_name = '" + ddl_billing_state.SelectedValue + "' and pay_billing_material_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_month_year.Text.Substring(3) + "'  and pay_billing_material_history.conveyance_type = 100 ORDER BY state_name,unit_name,emp_name ";
                }
                if (ddl_client.SelectedValue == "ALL")
                {
                    where = " pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'  " + where_state + " and pay_billing_material_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.Year = '" + txt_month_year.Text.Substring(3) + "'  and pay_billing_material_history.conveyance_type = 100 ORDER BY state_name,unit_name,emp_name ";
                }
                query = " SELECT CASE WHEN invoice_flag != 0 AND pay_billing_material_history.month <= 3 AND pay_billing_material_history.year <= 2019 THEN IFNULL(auto_invoice_no, invoice_no) ELSE auto_invoice_no END AS 'bill_invoice_no',CASE WHEN invoice_flag != 0 THEN DATE_FORMAT(billing_date, '%d/%m/%Y') ELSE '' END AS 'billing_date'," + daterange + ", CLIENT,UNIT_NAME, Client_branch_code as 'unit_code', STATE_NAME, pay_billing_material_history.EMP_NAME, grade_desc AS 'designation', conv_food_allowance_rate, food_allowance_days, conv_food_allowance_rate,conv_outstation_allowance_rate,conv_outstation_food_allowance_rate,conv_night_halt_rate,(conv_food_allowance_rate * food_allowance_days) AS 'food_total', conv_outstation_allowance_rate, outstation_allowance_days, (conv_outstation_allowance_rate * outstation_allowance_days) AS 'out_total', conv_outstation_food_allowance_rate, outstation_food_allowance_days, (conv_outstation_food_allowance_rate * outstation_food_allowance_days) AS 'out_food_total', conv_night_halt_rate, night_halt_days, (conv_night_halt_rate * night_halt_days) AS 'night_total', km_rate, kms, (total_km) AS 'km_total', ((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) AS 'Subtotal_A', (((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) * 5 / 100) AS 'Service_Charge', (((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) + (((conv_food_allowance_rate * food_allowance_days) + (conv_outstation_allowance_rate * outstation_allowance_days) + (conv_outstation_food_allowance_rate * outstation_food_allowance_days) + (conv_night_halt_rate * night_halt_days) + (total_km)) * 5 / 100)) AS 'sub_total',SGST, CGST,IGST FROM pay_billing_material_history INNER JOIN pay_conveyance_amount_history ON pay_conveyance_amount_history.emp_code = pay_billing_material_history.emp_code AND pay_conveyance_amount_history.comp_code = pay_billing_material_history.comp_code  AND pay_conveyance_amount_history.month = '" + txt_month_year.Text.Substring(0, 2) + "' AND pay_conveyance_amount_history.year = '" + txt_month_year.Text.Substring(3) + "' and  pay_conveyance_amount_history.conveyance = 'driver_conveyance' INNER JOIN pay_billing_master ON pay_billing_master.billing_unit_code = pay_billing_material_history.unit_code AND pay_billing_master.comp_code = pay_billing_material_history.comp_code AND pay_billing_master.designation = pay_billing_material_history.GRADE_CODE  WHERE " + where;
            }
            //adp1 = new MySqlDataAdapter("SELECT (SELECT ITEM_NAME FROM pay_item_master WHERE ITEM_CODE = pay_transaction_details.ITEM_CODE AND comp_code = pay_transaction_details.COMP_CODE) AS 'ITEMS',RATE,QUANTITY,	DESIGNATION as 'UNIT', AMOUNT, (SELECT UNIT_NAME FROM pay_unit_master WHERE unit_code = pay_transaction.branch_name AND comp_code = pay_transaction.COMP_CODE) AS 'BRANCH', pay_transaction_details.Vat AS 'GST %', (AMOUNT * pay_transaction_details.Vat) / 100 AS 'GST AMOUNT',(AMOUNT +(AMOUNT * pay_transaction_details.Vat) / 100) as 'TOTAL' FROM pay_transaction INNER JOIN pay_transaction_details ON pay_transaction.DOC_NO = pay_transaction_details.DOC_NO AND pay_transaction.COMP_CODE = pay_transaction_details.COMP_CODE WHERE " + where, d1.con1);

            DataSet ds = new DataSet();
            MySqlDataAdapter adp1 = new MySqlDataAdapter(query, d1.con1);
            adp1.Fill(ds);


            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                if (i == 1)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=Conveyance_Finance_Copy_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");

                }
                else if (i == 2)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=Material_Finance_Copy_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                }
                else if (i == 3)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=Deep_Cleaning_Finance_Copy_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                }
                else if (i == 4)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=PestControl_Finance_Copy_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                }
                else if (i == 5)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=Material_Billing_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                }
                else if (i == 6)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=Driver_Conveyance_Finance_Copy_" + ddl_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                }
                if (ddl_client.SelectedValue == "RCPL" && i == 2) { invoice = ""; }
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                if (ddl_material_bill_type == "2")//2-Tissue bill
                {
                    Repeater1.HeaderTemplate = new MyTemplate_Tissue(ListItemType.Header, ds, i, invoice, bill_date, 1);
                    Repeater1.ItemTemplate = new MyTemplate_Tissue(ListItemType.Item, ds, i, invoice, bill_date, 1);
                    Repeater1.FooterTemplate = new MyTemplate_Tissue(ListItemType.Footer, null, i, invoice, bill_date, 1);
                }
                else
                {
                    Repeater1.HeaderTemplate = new MyTemplate1(ListItemType.Header, ds, i, invoice, bill_date, 1);
                    Repeater1.ItemTemplate = new MyTemplate1(ListItemType.Item, ds, i, invoice, bill_date, 1);
                    Repeater1.FooterTemplate = new MyTemplate1(ListItemType.Footer, null, i, invoice, bill_date, 1);
                }


                Repeater1.DataBind();

                System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                Repeater1.RenderControl(htmlWrite);

                if (ddl_client.SelectedValue == "RCPL" && i == 2)
                {
                    stringWrite = update_material_grp_companies(stringWrite, ds);
                }

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
    private StringWriter update_material_grp_companies(StringWriter stringwrite, DataSet ds)
    {

        string grp_comp = "", where = "", tr = "", td = "", branch_name = "", cell = "";
        double ctc = 0, percentage = 0;
        int row_count = ds.Tables[0].Rows.Count;

        cell = "Q" + (row_count + 4) + "";
        where = "comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'  and unit_code = '" + ddl_unitcode.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "'";
        if (ddl_billing_state.SelectedValue == "ALL")
        {
            where = "comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' ";
        }
        else if (ddl_unitcode.SelectedValue == "ALL")
        {
            where = "comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "'  and unit_code in (Select unit_code from pay_unit_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name = '" + ddl_billing_state.SelectedValue + "') and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "'";
        }

        MySqlCommand cmd = new MySqlCommand("Select state_per from pay_billing_material_history  where " + where + " AND Emp_code IS NULL GROUP BY state_per", d_cg.con);
        d_cg.con.Open();
        MySqlDataReader dr = cmd.ExecuteReader();
        //looop
        while (dr.Read())
        {
            string invoice_no = d.getsinglestring("select auto_invoice_no from pay_billing_material_history where " + where + " and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' and state_per = '" + dr.GetValue(0).ToString() + "' and invoice_flag != 0 ");
            d1.con1.Open();
            MySqlCommand cmd_cg = new MySqlCommand("Select  unit_code,state_per,Companyname_gst_no,gst_address,bill_amount from pay_billing_material_history where " + where + "  and state_per= '" + dr.GetValue(0).ToString() + "' GROUP BY state_per", d1.con1);
            MySqlDataReader dr_cg = cmd_cg.ExecuteReader();
            while (dr_cg.Read())
            {
                percentage = percentage + double.Parse(dr_cg.GetValue(4).ToString());
                td = td + "<td>" + dr_cg.GetValue(4).ToString() + "</td><td>= " + cell + " * " + dr_cg.GetValue(4).ToString() + "%</td>";
            }
            dr_cg.Dispose();
            d1.con1.Close();

            tr = tr + "<tr><th colspan=2>'" + invoice_no + "</th><th colspan=2>" + dr.GetValue(0).ToString() + "</th>" + td + "</tr>";
            td = "";
        }
        tr = tr + "<tr><th colspan=4 >Total</th><td>=Round(" + percentage + ",2)</td><td>=Round(" + cell + ",2)</td></tr>";
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
    public class MyTemplate_Tissue : ITemplate
    {

        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        int i, i3 = 1, ctr1, state_change = 0;
        static int ctr;
        string invoice = "";
        string bill_date = "";
        string km_per_rate = "";
        string km_per_rate_value = "", col1 = "", col2 = "", total = "BASE AMOUNT", state_name = "";
        double grand_tot = 0, cgst = 0, sgst = 0, igst = 0, total1 = 0, sub_total = 0, machine_rate_amt = 0, no_of_duties = 0, gst = 0, handl = 0, handltotal = 0, rate = 0, material_area = 0;
        double grand_tot1 = 0, cgst1 = 0, sgst1 = 0, igst1 = 0, total2 = 0, sub_total1 = 0, machine_rate_amt1 = 0, gst1 = 0, handl1 = 0, handltotal1 = 0, rate1 = 0, material_area1 = 0, grand_tot3 = 0;

        public MyTemplate_Tissue(ListItemType type, DataSet ds, int i, string invoice, string bill_date, int i3)
        {

            this.type = type;
            this.ds = ds;
            ctr = 0;
            this.i = i;
            this.invoice = invoice;
            this.bill_date = bill_date;
            this.i3 = i3;


        }
        public void InstantiateIn(Control container)
        {

            switch (type)
            {
                case ListItemType.Header:

                    if (i == 2)
                    {
                        lc = new LiteralControl("<table border=1><tr><th  colspan=16 bgcolor=yellow align=center >MATERIAL FINANCE COPY FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr></tr><tr><th>SR.NO.</th><th>CLIENT</th><th>BILL NO</th><th>BILL DATE</th><th>MONTH</th><th>YEAR</th><th>STATE</th><th>MATERIAL NAME</th><th>RATE</th><th> QTY</th><th>SUB TOTAL</th><th>CGST 9%</th><th>SGST 9%</th><th>IGST 18%</th><th>TOTAL GST</th><th>GRAND TOTAL</th></tr>");
                    }

                    break;
                case ListItemType.Item:
                    if (i == 2)
                    {
                        string material_name = "TISSUE & NAPKIN";
                        // comp_code, auto_invoice_no, invoice_no, billing_date, client_code, client, COMP_STATE,                                                                                                                                                                                                                                                                                                         state_name, unit_code, month, year,  contract_type,                                                                                                                                                                                                                                                                                                                                                                                                  cgst, sgst, igst, material_type, tissue_qty, tissue_rate, amount, billingamt
                        int start_first_row = 4;
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["client"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["month"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["year"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + material_name + "</td><td>" + ds.Tables[0].Rows[ctr]["tissue_rate"].ToString() + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["tissue_qty"].ToString()) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["amount"].ToString()) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString()) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString()) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["total_gst"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["billingamt"].ToString()), 2) + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            //  lc.Text = lc.Text + "<tr><b><td align=center colspan=9>Total</td><td>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O2:M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td></b></tr>";
                            lc.Text = lc.Text + "<tr><b><td align=center colspan = 9>Total</td><td>=ROUND(SUM(J4:J" + (ctr + 4) + "),2)</td><td>=ROUND(SUM(K4:K" + (ctr + 4) + "),2)</td><td>=ROUND(SUM(L4:L" + (ctr + 4) + "),2)</td><td>=ROUND(SUM(M4:M" + (ctr + 4) + "),2)</td><td>=ROUND(SUM(N4:N" + (ctr + 4) + "),2)</td><td>=ROUND(SUM(O4:O" + (ctr + 4) + "),2)</td><td>=ROUND(SUM(P4:P" + (ctr + 4) + "),2)</td></b></tr>";
                        }
                        //if (counter == 1)
                        //{
                        //    if (ds.Tables[0].Rows.Count == ctr + 1)
                        //    {
                        //        lc.Text = lc.Text + "<tr><b><td align=center colspan = 9>Total</td><td>=ROUND(SUM(J3:J" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(K3:K" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(L3:L" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(M3:M" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(N3:N" + (ctr + 3) + "),2)</td><td>=ROUND(SUM(O3:O" + (ctr + 3) + "),2)</td></b></tr>";
                        //    }
                        //}
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
    public class MyTemplate1 : ITemplate
    {

        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        int i, i3 = 1, ctr1, state_change = 0;
        static int ctr;
        string invoice = "";
        string bill_date = "";
        string km_per_rate = "";
        string km_per_rate_value = "", col1 = "", col2 = "", total = "BASE AMOUNT", state_name = "";
        double grand_tot = 0, cgst = 0, sgst = 0, igst = 0, total1 = 0, sub_total = 0, machine_rate_amt = 0, no_of_duties = 0, gst = 0, handl = 0, handltotal = 0, rate = 0, material_area = 0;
        double grand_tot1 = 0, cgst1 = 0, sgst1 = 0, igst1 = 0, total2 = 0, sub_total1 = 0, machine_rate_amt1 = 0, gst1 = 0, handl1 = 0, handltotal1 = 0, rate1 = 0, material_area1 = 0, grand_tot3 = 0;

        public MyTemplate1(ListItemType type, DataSet ds, int i, string invoice, string bill_date, int i3)
        {

            this.type = type;
            this.ds = ds;
            ctr = 0;
            this.i = i;
            this.invoice = invoice;
            this.bill_date = bill_date;
            this.i3 = i3;


        }
        public void InstantiateIn(Control container)
        {

            switch (type)
            {
                case ListItemType.Header:
                    if (i == 1)
                    {
                        int colsize = 18;
                        if (!ds.Tables[0].Rows[ctr]["conveyance_type"].ToString().Equals("3"))
                        {
                            km_per_rate = "<th>RATE PER KM.</th><th>TOTAL KM.</th>";
                            total = "SUB TOTAL";
                            colsize = 20;

                        }
                        lc = new LiteralControl("<table border=1><tr><th  colspan=" + colsize + " bgcolor=yellow align=center >CONVEYANCE FINANCE COPY FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr></tr><tr><th>SR.NO.</th><th>BILL NO</th><th>BILL DATE</th><th>BILLING PERIOD</th><th>CLIENT NAME<th>STATE</th><th>LOCATION</th><th>BRANCH CODE</th><th>EMPLOYEE NAME</th><th>DESIGNATION</th>" + km_per_rate + "<th>" + total + "</th><th>SERVICE CHARGE " + ds.Tables[0].Rows[ctr]["conveyance_service_charge_per"].ToString() + "%</th><th>TOTAL</th><th>CGST 9%</th><th>SGST 9%</th><th>IGST 18%</th><th>TOTAL GST</th><th>GRAND TOTAL</th></tr>");


                    }
                    else if (i == 2)
                    {


                        string handling = "";
                        int colspan = 16;
                        string sqr_fit_columns = "<th>RATE (PER SQ.FT.)</th><th>BRANCH AREA (SQ.FT.)</th>";
                        string machine_rental = "";
                        if (!ds.Tables[0].Rows[ctr]["CLIENT_CODE"].ToString().Equals("RBL"))
                        {
                            handling = "<th>HANDLING CHARGES(" + ds.Tables[0].Rows[ctr]["handling_percent"] + "%)</th><th>TOTAL</th>";

                            if (ds.Tables[0].Rows[ctr]["contract_type"].ToString().Equals("1") || ds.Tables[0].Rows[ctr]["contract_type"].ToString().Equals("4"))
                            {
                                sqr_fit_columns = "";
                                colspan = 16;
                            }
                            else
                            {
                                colspan = 18;
                            }
                        }
                        if (ds.Tables[0].Rows[ctr]["CLIENT_CODE"].ToString().Equals("RCPL") || ds.Tables[0].Rows[ctr]["machine_rental_applicable"].ToString().Equals("1"))
                        {
                            machine_rental = "<th>MACHINE RENTAL</th>";
                            colspan = 17;
                        }
                        if (ds.Tables[0].Rows[ctr]["material_contract"].ToString().Equals("3"))
                        {
                            lc = new LiteralControl("<table border=1><tr><th  colspan=" + colspan + " bgcolor=yellow align=center >MATERIAL FINANCE COPY FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr></tr><tr><th>SR.NO.</th><th>BILL NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>STATE</th><th>LOCATION</th><th>BRANCH CODE</th><th>MATERIAL NAME</th><th>RATE</th><th> QTY.</th><th>SUB TOTAL</th>" + handling + "<th>CGST 9%</th><th>SGST 9%</th><th>IGST 18%</th><th>TOTAL GST</th><th>GRAND TOTAL</th></tr>");
                        }
                        else
                        {
                            lc = new LiteralControl("<table border=1><tr><th  colspan=" + colspan + " bgcolor=yellow align=center >MATERIAL FINANCE COPY FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr></tr><tr><th>SR.NO.</th><th>BILL NO</th><th>BILL DATE</th><th>INVOICE PERIOD</th><th>STATE</th><th>LOCATION</th><th>BRANCH CODE</th>" + sqr_fit_columns + "<th>TOTAL MATERIAL COST</th>" + machine_rental + "<th>SUB TOTAL</th>" + handling + "<th>CGST 9%</th><th>SGST 9%</th><th>IGST 18%</th><th>TOTAL GST</th><th>GRAND TOTAL</th></tr>");
                            //if (ds.Tables[0].Rows.Count == ctr + 1)
                            //{
                            //    lc.Text = lc.Text + "<tr><b><td align=center colspan=10>Total</td><td>=ROUND(SUM(K2:K" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(L2:L" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(M2:M" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(N2:N" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(O2:O" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(P2:P" + (ctr + 2) + "),2)</td></b></tr>";
                            //}
                        }
                    }
                    else if (i == 3)
                    {
                        lc = new LiteralControl("<table border=1><tr><th  colspan=15 bgcolor=yellow align=center >DEEP CLEAN FINANCE COPY FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr></tr><tr><th>SR.NO.</th><th>BILL NO</th><th>BILL DATE</th><th>CLIENT NAME</th><th>STATE</th><th>LOCATION</th><th>BRANCH CODE</th><th>RATE</th><th>SERVICE CHARGE(" + ds.Tables[0].Rows[ctr]["dc_handling_percent"] + "%)</th><th>SUB TOTAL</th><th>CGST 9%</th><th>SGST 9%</th><th>IGST 18%</th><th>TOTAL GST</th><th>GRAND TOTAL</th></tr>");
                    }
                    else if (i == 4)
                    {
                        lc = new LiteralControl("<table border=1><tr><th  colspan=16 bgcolor=yellow align=center >PEST CONTROL FINANCE COPY FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr></tr><tr><th>SR.NO.</th><th>BILL NO</th><th>BILL DATE</th><th>STATE</th><th>LOCATION</th><th>BRANCH CODE</th><th>SQFT.AREA</th><th>PER SQFT.RATE</th><th>AMOUNT</th><th>HANDLING CHARGES(" + ds.Tables[0].Rows[ctr]["pc_handling_percent"] + "%)</th><th>SUB TOTAL</th><th>CGST 9%</th><th>SGST 9%</th><th>IGST 18%</th><th>TOTAL GST</th><th>GRAND TOTAL</th></tr>");
                    }
                    else if (i == 5)
                    {
                        lc = new LiteralControl("<TABLE BORDER=1><TR><TH colspan=2>CLIENT NAME</TH><TH colspan=2>STATE</TH><TH colspan=2>BRANCH NAME</TH><TH colspan=2>ITEM NAME</TH><TH colspan=2>ITEM RATE</TH><TH colspan=2>QUANTITY</TH><TH colspan=2>UNIT</TH><TH colspan=2>AMOUNT </TH><TH colspan=2>GST %</TH><TH colspan=2>GST AMOUNT</TH><TH colspan=2>TOTAL AMOUNT</TH></TR>");

                    }
                    else if (i == 6)
                    {
                        lc = new LiteralControl("<table border =1><tr ><th style=background-color:yellow  colspan= 33 align=center >DRIVER CONVEYANCE FINANCE COPY FOR " + reprint_invoice.month_name.ToUpper() + "</th></tr><tr style=font-weight:bold;text-align:center;><td rowspan=2 style=background-color:yellow>SR.NO</td><td style=background-color:yellow rowspan=2>BILL NO</td><td rowspan=2 style=background-color:yellow>BILL DATE</td><td style=background-color:yellow;white-space:nowrap;text-align:center; rowspan=2>BILLING PERIOD</td><td style=background-color:yellow rowspan=2>CLIENT NAME</td><td style=background-color:yellow rowspan=2>LOCATION</td><td style=background-color:yellow rowspan=2>BRANCH CODE</td><td style=background-color:yellow rowspan=2>STATE</td><td style=background-color:yellow rowspan=2>NAME OF THE DEPUTY</td><td style=background-color:yellow rowspan=2>DESIGNATION</td><td style=background-color:yellow colspan=3>FOOD ALLOWANCE</td><td style=background-color:yellow colspan=3>OUTSTATION ALLOWANCE/CONVEYANCE </td><td style=background-color:yellow colspan=3>OUTSTATION FOOD ALLOWANCE </td><td style=background-color:yellow colspan=3>NIGHT HALT</td><td style=background-color:yellow colspan=3>TOTAL KM AMOUNT</td><td style=background-color:yellow rowspan=2>SUB TOTAL (A) REGULAR + OT</td><td style=background-color:yellow rowspan=2>SERVICE CHARGE@5%</td><td style=background-color:yellow rowspan=2>SUB TOTAL</td><td style=background-color:yellow;text-align:center; colspan=3>GST</td><td style=background-color:yellow rowspan=2>TOTAL GST</td><td style=background-color:yellow rowspan=2>GRAND TOTAL</td></tr><tr><td style=background-color:pink;>RATE</td><td style=background-color:pink;>DAYS</td><td style=background-color:pink;>TOTAL</td><td style=background-color:skyblue;>RATE</td><td style=background-color:skyblue>DAYS</td><td style=background-color:skyblue>TOTAL</td><td style=background-color:pink;>RATE</td><td style=background-color:pink;>DAYS</td><td style=background-color:pink;>TOTAL</td><td style=background-color:skyblue>RATE</td><td style=background-color:skyblue>DAYS</td><td style=background-color:skyblue>TOTAL</td><td style=background-color:pink;>RATE</td><td style=background-color:pink;>TOTAL KM</td><td style=background-color:pink;>TOTAL</td><td style=background-color:yellow;>CGST 9%</td><td style=background-color:yellow;>SGST 9%</td><td style=background-color:yellow;>IGST 18%</td></tr>");

                    }
                    break;
                case ListItemType.Item:
                    if (i == 5)
                    {
                        lc = new LiteralControl("<tr><td colspan=2 >" + ds.Tables[0].Rows[ctr]["CLIENT_NAME"] + " </td><td colspan=2>" + ds.Tables[0].Rows[ctr]["state"] + "</td><td colspan=2>" + ds.Tables[0].Rows[ctr]["BRANCH"] + "</td><td colspan=2>" + ds.Tables[0].Rows[ctr]["ITEMS"] + " </td><td colspan=2>'" + ds.Tables[0].Rows[ctr]["RATE"] + "</td><td colspan=2>'" + ds.Tables[0].Rows[ctr]["QUANTITY"] + "</td><td colspan=2>" + ds.Tables[0].Rows[ctr]["UNIT"] + "</td><td colspan=2>" + ds.Tables[0].Rows[ctr]["AMOUNT"] + "</td><td colspan=2>" + ds.Tables[0].Rows[ctr]["GST %"] + "</td><td colspan=2>" + ds.Tables[0].Rows[ctr]["GST AMOUNT"] + "</td><td colspan=2>'" + ds.Tables[0].Rows[ctr]["TOTAL"] + "</td></tr>");
                    }
                    else if (i == 2)
                    {
                        int start_first_row = 4;
                        if (ds.Tables[0].Rows[ctr]["CLIENT_CODE"].ToString().Equals("RCPL"))
                        {
                            string handling = "";
                            string handling_tot = "";
                            string sqr_fit_columns = "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["rate"].ToString()), 2) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["material_area"].ToString()) + "</td>";
                            string grand_total = "<td>=ROUND(SUM(L" + (ctr + start_first_row) + ",P" + (ctr + start_first_row) + "),2)</td>";


                            if (!ds.Tables[0].Rows[ctr]["CLIENT_CODE"].ToString().Equals("RBL"))
                            {
                                handling = "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()), 2) + "</td>";
                                handling_tot = "<td>=ROUND(SUM(R" + start_first_row + ":R" + (ctr + start_first_row) + "),2)</td><td>= ROUND(SUM(S" + start_first_row + ": S" + (ctr + start_first_row) + "), 2) </td>";
                                grand_total = "<td>=ROUND(SUM(N" + (ctr + start_first_row) + ",R" + (ctr + start_first_row) + "),2)</td>";

                                if (ds.Tables[0].Rows[ctr]["contract_type"].ToString().Equals("1"))
                                {
                                    sqr_fit_columns = "";
                                    handling = "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()), 2) + "</td>";
                                    handling_tot = "<td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>= ROUND(SUM(P" + start_first_row + ": P" + (ctr + start_first_row) + "), 2) </td>";
                                    grand_total = "<td>=ROUND(SUM(L" + (ctr + start_first_row) + ",P" + (ctr + start_first_row) + "),2)</td>";

                                }

                            }
                            if (state_name != ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper())
                            {
                                if (state_name != "")
                                {

                                    i3 = i3 + 1;

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan= 7>Total</td><td>" + sub_total + "</td><td>" + machine_rate_amt + "</td><td>" + total1 + "</td><td>" + handl + "</td><td>" + handltotal + "</td><td>" + cgst + "</td><td>" + sgst + "</td><td>" + igst + "</td><td>" + gst + "</td><td>" + grand_tot + "</td></b></tr>";

                                    ctr1 = ctr + i3 + 1;
                                    state_change = 1;
                                    sub_total = 0; machine_rate_amt = 0; total1 = 0; cgst = 0; sgst = 0; handl = 0; handltotal = 0; igst = 0; gst = 0; grand_tot = 0; total1 = 0;

                                }

                            }
                            sub_total = sub_total + double.Parse(ds.Tables[0].Rows[ctr]["sub_total"].ToString());
                            machine_rate_amt = machine_rate_amt + double.Parse(ds.Tables[0].Rows[ctr]["machine_rental_amount"].ToString());
                            total1 = total1 + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString());
                            handl = handl + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString());
                            handltotal = handltotal + (double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()));
                            cgst = cgst + double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString());
                            sgst = sgst + double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString());
                            igst = igst + double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString());
                            gst = gst + ((double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString())));
                            grand_tot = grand_tot + ((double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString())) + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString())));

                            sub_total1 = sub_total + double.Parse(ds.Tables[0].Rows[ctr]["sub_total"].ToString());
                            machine_rate_amt1 = machine_rate_amt + double.Parse(ds.Tables[0].Rows[ctr]["machine_rental_amount"].ToString());
                            total2 = total1 + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString());
                            handl1 = handl + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString());
                            handltotal1 = handltotal + (double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()));
                            cgst1 = cgst + double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString());
                            sgst1 = sgst + double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString());
                            igst1 = igst + double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString());
                            gst1 = gst + ((double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString())));
                            grand_tot1 = grand_tot + ((double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString())) + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString())));

                            state_name = ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper();
                            lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString() + "</td>" + sqr_fit_columns + "<td>" + double.Parse(ds.Tables[0].Rows[ctr]["sub_total"].ToString()) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["machine_rental_amount"].ToString()) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()), 2) + "</td>" + handling + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>" + grand_tot + "</td></tr>");

                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                if (!ds.Tables[0].Rows[ctr]["CLIENT_CODE"].ToString().Equals("RBL"))
                                {

                                    if (ds.Tables[0].Rows[ctr]["contract_type"].ToString().Equals("1") || ds.Tables[0].Rows[ctr]["contract_type"].ToString().Equals("4"))
                                    {
                                        //lc.Text = lc.Text + "<tr><b><td align=center colspan=7>Total</td><td>=ROUND(SUM(H" + start_first_row + ":H" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(I" + start_first_row + ":I" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td></b></tr>";
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan= 7>Total</td><td>" + sub_total + "</td><td>" + machine_rate_amt + "</td><td>" + total1 + "</td><td>" + handl + "</td><td> " + handltotal + "</td><td>" + cgst + "</td><td>" + sgst + "</td><td>" + igst + "</td><td>" + gst + "</td><td>" + grand_tot + "</td></b></tr>";
                                        if (state_change == 1)
                                        {
                                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 7>Grand Total</td><td>" + sub_total1 + "</td><td>" + machine_rate_amt1 + "</td><td>" + total2 + "</td><td>" + handl1 + "</td><td> " + handltotal1 + "</td><td>" + cgst1 + "</td><td>" + sgst1 + "</td><td>" + igst1 + "</td><td>" + gst1 + "</td><td>" + grand_tot1 + "</td></b></tr>";
                                        }
                                    }
                                    else
                                    {

                                        lc.Text = lc.Text + "<tr><b><td align=center colspan=7>Total</td><td>=ROUND(SUM(H" + start_first_row + ":H" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(I" + start_first_row + ":I" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td>" + handling_tot + "</b></tr>";

                                    }
                                }
                                else
                                {

                                    lc.Text = lc.Text + "<tr><b><td align=center colspan=7>Total</td><td>=ROUND(SUM(H" + start_first_row + ":H" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(I" + start_first_row + ":I" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td>" + handling_tot + "</b></tr>";

                                }
                            }
                        }
                        else
                        {
                            string handling = "";
                            string handling_tot = "";
                            string sqr_fit_columns = "";
                            if (ds.Tables[0].Rows[ctr]["material_contract"].ToString().Equals("3"))
                            {
                                sqr_fit_columns = "";
                            }
                            else
                            {
                                sqr_fit_columns = "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["rate"].ToString()), 2) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["material_area"].ToString()) + "</td>";
                            }
                            string grand_total = "<td>=ROUND(SUM(K" + (ctr + start_first_row) + ",O" + (ctr + start_first_row) + "),2)</td>";

                            if (!ds.Tables[0].Rows[ctr]["CLIENT_CODE"].ToString().Equals("RBL"))
                            {

                                handling = "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()), 2) + "</td>";
                                handling_tot = "<td>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td><td>= ROUND(SUM(R" + start_first_row + ": R" + (ctr + start_first_row) + "), 2) </td>";
                                grand_total = "<td>=ROUND(SUM(M" + (ctr + start_first_row) + ",Q" + (ctr + start_first_row) + "),2)</td>";

                                if (ds.Tables[0].Rows[ctr]["contract_type"].ToString().Equals("1") || ds.Tables[0].Rows[ctr]["contract_type"].ToString().Equals("4"))
                                {
                                    sqr_fit_columns = "";
                                    handling = "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()), 2) + "</td>";
                                    handling_tot = "<td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>= ROUND(SUM(N" + start_first_row + ": N" + (ctr + start_first_row) + "), 2) </td>";
                                    grand_total = "<td>=ROUND(SUM(K" + (ctr + start_first_row) + ",O" + (ctr + start_first_row) + "),2)</td>";

                                }

                            }
                            string machine_rental = "";
                            if (ds.Tables[0].Rows[ctr]["machine_rental_applicable"].ToString().Equals("1"))
                            {
                                machine_rental = "<td>" + ds.Tables[0].Rows[ctr]["machine_rental_amount"].ToString() + "</td>";
                                grand_total = "<td>=ROUND(SUM(L" + (ctr + start_first_row) + ",P" + (ctr + start_first_row) + "),2)</td>";
                            }

                            if (state_name != ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper())
                            {
                                if (state_name != "")
                                {

                                    i3 = i3 + 1;


                                    if (ds.Tables[0].Rows[ctr]["machine_rental_applicable"].ToString().Equals("1"))
                                    {
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan= 7 bgcolor=Orange>Total</td><td bgcolor=Orange>" + sub_total + "</td><td bgcolor=Orange>" + total1 + "</td><td bgcolor=Orange>" + handl + "</td><td bgcolor=Orange>" + handltotal + "</td><td bgcolor=Orange>" + cgst + "</td><td bgcolor=Orange>" + sgst + "</td><td bgcolor=Orange>" + igst + "</td><td bgcolor=Orange>" + gst + "</td><td bgcolor=Orange>" + grand_tot3 + "</td></b></tr>";
                                    }
                                    else if (ds.Tables[0].Rows[ctr]["CLIENT_CODE"].ToString().Equals("RBL") || ds.Tables[0].Rows[ctr]["CLIENT_CODE"].ToString().Equals("TAIL"))
                                    {
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan= 7 bgcolor=Orange>Total</td><td bgcolor=Orange>" + rate + "</td><td bgcolor=Orange>" + material_area + "</td><td bgcolor=Orange>" + sub_total + "</td><td bgcolor=Orange>" + total1 + "</td><td bgcolor=Orange>" + handl + "</td><td bgcolor=Orange>" + handltotal + "</td><td bgcolor=Orange>" + cgst + "</td><td bgcolor=Orange>" + sgst + "</td><td bgcolor=Orange>" + igst + "</td><td bgcolor=Orange>" + gst + "</td><td bgcolor=Orange>" + grand_tot3 + "</td></b></tr>";
                                    }
                                    else
                                    {
                                        if (sqr_fit_columns == "")
                                        {
                                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 7 bgcolor=Orange>Total</td><td bgcolor=Orange>" + sub_total + "</td><td bgcolor=Orange>" + total1 + "</td><td bgcolor=Orange>" + handl + "</td><td bgcolor=Orange>" + handltotal + "</td><td bgcolor=Orange>" + cgst + "</td><td bgcolor=Orange>" + sgst + "</td><td bgcolor=Orange>" + igst + "</td><td bgcolor=Orange>" + gst + "</td><td bgcolor=Orange>" + grand_tot3 + "</td></b></tr>";
                                        }
                                        else
                                        {
                                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 7 bgcolor=Orange>Total</td><td  bgcolor=Orange>" + rate + "</td><td  bgcolor=Orange>" + material_area + "</td><td bgcolor=Orange>" + sub_total + "</td><td bgcolor=Orange>" + total1 + "</td><td bgcolor=Orange>" + handl + "</td><td bgcolor=Orange>" + handltotal + "</td><td bgcolor=Orange>" + cgst + "</td><td bgcolor=Orange>" + sgst + "</td><td bgcolor=Orange>" + igst + "</td><td bgcolor=Orange>" + gst + "</td><td bgcolor=Orange>" + grand_tot3 + "</td></b></tr>";
                                        }
                                    }

                                    ctr1 = ctr + i3 + 1;
                                    state_change = 1;
                                    sub_total = 0; machine_rate_amt = 0; total1 = 0; cgst = 0; sgst = 0; handl = 0; handltotal = 0; igst = 0; gst = 0; grand_tot = 0; total1 = 0; rate = 0; material_area = 0; grand_tot3 = 0;

                                }

                            }
                            state_name = ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper();

                            grand_tot = 0;

                            sub_total = sub_total + double.Parse(ds.Tables[0].Rows[ctr]["sub_total"].ToString());
                            rate = rate + double.Parse(ds.Tables[0].Rows[ctr]["rate"].ToString());
                            material_area = material_area + double.Parse(ds.Tables[0].Rows[ctr]["material_area"].ToString());
                            machine_rate_amt = machine_rate_amt + double.Parse(ds.Tables[0].Rows[ctr]["machine_rental_amount"].ToString());
                            total1 = total1 + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString());
                            handl = handl + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString());
                            handltotal = handltotal + (double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()));
                            cgst = cgst + double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString());
                            sgst = sgst + double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString());
                            igst = igst + double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString());
                            gst = gst + ((double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString())));
                            grand_tot = grand_tot + ((double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString())) + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString())));
                            grand_tot3 = grand_tot3 + grand_tot;

                            if (ds.Tables[0].Rows[ctr]["material_contract"].ToString().Equals("3"))
                            {
                                lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["material_name"] + "</td><td>" + ds.Tables[0].Rows[ctr]["rate"] + "</td><td>" + ds.Tables[0].Rows[ctr]["quantity"] + "</td>" + machine_rental + "<td>" + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + "</td>" + handling + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>" + grand_tot + "</td></tr>");
                            }
                            else
                            {
                                lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString() + "</td>" + sqr_fit_columns + "<td>" + double.Parse(ds.Tables[0].Rows[ctr]["sub_total"].ToString()) + "</td>" + machine_rental + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()), 2) + "</td>" + handling + "<td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>" + grand_tot + "</td></tr>");
                            }
                            rate1 = rate1 + double.Parse(ds.Tables[0].Rows[ctr]["rate"].ToString());
                            sub_total1 = sub_total1 + double.Parse(ds.Tables[0].Rows[ctr]["sub_total"].ToString());
                            material_area1 = material_area1 + double.Parse(ds.Tables[0].Rows[ctr]["material_area"].ToString());
                            machine_rate_amt1 = machine_rate_amt1 + double.Parse(ds.Tables[0].Rows[ctr]["machine_rental_amount"].ToString());
                            total2 = total2 + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString());
                            handl1 = handl1 + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString());
                            handltotal1 = handltotal1 + (double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()));
                            cgst1 = cgst1 + double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString());
                            sgst1 = sgst1 + double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString());
                            igst1 = igst1 + double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString());
                            gst1 = gst1 + ((double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString())));
                            grand_tot1 = grand_tot1 + ((double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString())) + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + (double.Parse(ds.Tables[0].Rows[ctr]["cgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["sgst"].ToString())) + (double.Parse(ds.Tables[0].Rows[ctr]["igst"].ToString())));


                            if (ds.Tables[0].Rows.Count == ctr + 1)
                            {
                                if (!ds.Tables[0].Rows[ctr]["CLIENT_CODE"].ToString().Equals("RBL"))
                                {

                                    if (ds.Tables[0].Rows[ctr]["contract_type"].ToString().Equals("1") || ds.Tables[0].Rows[ctr]["contract_type"].ToString().Equals("4"))
                                    {


                                        if (ds.Tables[0].Rows[ctr]["machine_rental_applicable"].ToString().Equals("1"))
                                        {
                                            //lc.Text = lc.Text + "<td>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td>";
                                            //lc.Text = lc.Text + "<td>" + machine_rate_amt + "</td>";
                                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 7>Total</td><td>" + sub_total + "</td><td>" + machine_rate_amt + "</td><td>" + total1 + "</td><td>" + handl + "</td><td> " + handltotal + "</td><td>" + cgst + "</td><td>" + sgst + "</td><td>" + igst + "</td><td>" + gst + "</td><td>" + grand_tot3 + "</td></b></tr>";
                                        }
                                        else
                                        {
                                            // lc.Text = lc.Text + "<tr><b><td align=center colspan=7>Total</td><td>=ROUND(SUM(H" + start_first_row + ":H" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(I" + start_first_row + ":I" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td>";
                                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 7 bgcolor=Orange>Total</td><td bgcolor=Orange>" + sub_total + "</td><td bgcolor=Orange>" + total1 + "</td><td bgcolor=Orange>" + handl + "</td><td bgcolor=Orange> " + handltotal + "</td><td bgcolor=Orange>" + cgst + "</td><td bgcolor=Orange>" + sgst + "</td><td bgcolor=Orange>" + igst + "</td><td bgcolor=Orange>" + gst + "</td><td bgcolor=Orange>" + grand_tot3 + "</td></b></tr>";

                                        }
                                        if (state_change == 1)
                                        {
                                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 7 bgcolor=Orange>Grand Total</td><td bgcolor=Orange>" + sub_total1 + "</td><td bgcolor=Orange>" + total2 + "</td><td bgcolor=Orange>" + handl1 + "</td><td bgcolor=Orange> " + handltotal1 + "</td><td bgcolor=Orange>" + cgst1 + "</td><td bgcolor=Orange>" + sgst1 + "</td><td bgcolor=Orange>" + igst1 + "</td><td bgcolor=Orange>" + gst1 + "</td><td bgcolor=Orange>" + grand_tot1 + "</td></b></tr>";

                                        }

                                        // lc.Text = lc.Text + "</b></tr>";
                                    }
                                    else if (ds.Tables[0].Rows[ctr]["material_contract"].ToString().Equals("3"))
                                    {
                                        lc.Text = lc.Text + "<tr><b><td align=center colspan=8>Total</td><td>=ROUND(SUM(I" + start_first_row + ":I" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td>";
                                        if (ds.Tables[0].Rows[ctr]["machine_rental_applicable"].ToString().Equals("1"))
                                        {
                                            lc.Text = lc.Text + "<td>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td>";
                                        }
                                        lc.Text = lc.Text + handling_tot + "</b></tr>";
                                    }
                                    else
                                    {

                                        if (state_change == 1)
                                        {
                                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 7  bgcolor=Orange>Total</td><td  bgcolor=Orange>" + rate + "</td><td  bgcolor=Orange>" + material_area + "</td><td  bgcolor=Orange>" + sub_total + "</td><td bgcolor=Orange>" + total1 + "</td><td  bgcolor=Orange>" + handl + "</td><td  bgcolor=Orange>" + handltotal + "</td><td  bgcolor=Orange>" + cgst + "</td><td  bgcolor=Orange>" + sgst + "</td><td  bgcolor=Orange>" + igst + "</td><td  bgcolor=Orange>" + gst + "</td><td  bgcolor=Orange>" + grand_tot3 + "</td></b></tr>";
                                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 7  bgcolor=Orange>Grand Total</td><td  bgcolor=Orange>" + rate1 + "</td><td  bgcolor=Orange>" + material_area1 + "</td><td  bgcolor=Orange>" + sub_total1 + "</td><td  bgcolor=Orange>" + total2 + "</td><td  bgcolor=Orange>" + handl1 + "</td><td  bgcolor=Orange> " + handltotal1 + "</td><td  bgcolor=Orange>" + cgst1 + "</td><td  bgcolor=Orange>" + sgst1 + "</td><td  bgcolor=Orange>" + igst1 + "</td><td  bgcolor=Orange>" + gst1 + "</td><td  bgcolor=Orange>" + grand_tot1 + "</td></b></tr>";

                                        }
                                        else
                                        {
                                            lc.Text = lc.Text + "<tr><b><td align=center colspan=7 bgcolor=Orange>Total</td><td bgcolor=Orange>=ROUND(SUM(H" + start_first_row + ":H" + (ctr + start_first_row) + "),2)</td><td bgcolor=Orange>=ROUND(SUM(I" + start_first_row + ":I" + (ctr + start_first_row) + "),2)</td><td bgcolor=Orange>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td bgcolor=Orange>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td bgcolor=Orange>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td bgcolor=Orange>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td bgcolor=Orange>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td bgcolor=Orange>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td bgcolor=Orange>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td>";
                                            if (ds.Tables[0].Rows[ctr]["machine_rental_applicable"].ToString().Equals("1"))
                                            {
                                                lc.Text = lc.Text + "<td bgcolor=Orange>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td>";
                                            }
                                            lc.Text = lc.Text + handling_tot + "</b></tr>";
                                        }
                                    }
                                }
                                else
                                {
                                    lc.Text = lc.Text + "<tr><b><td align=center colspan= 7 bgcolor=Orange>Total</td><td bgcolor=Orange>" + rate + "</td><td bgcolor=Orange>" + material_area + "</td><td bgcolor=Orange>" + sub_total + "</td><td bgcolor=Orange>" + total1 + "</td><td bgcolor=Orange>" + cgst + "</td><td bgcolor=Orange>" + sgst + "</td><td bgcolor=Orange>" + igst + "</td><td bgcolor=Orange>" + gst + "</td><td bgcolor=Orange>" + grand_tot3 + "</td></b></tr>";
                                    // lc.Text = lc.Text + "<tr><b><td align=center colspan=7>Total</td><td>=ROUND(SUM(H" + start_first_row + ":H" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(I" + start_first_row + ":I" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td>";
                                    if (ds.Tables[0].Rows[ctr]["machine_rental_applicable"].ToString().Equals("1"))
                                    {
                                        lc.Text = lc.Text + "<td bgcolor=Orange>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td>";
                                    }
                                    lc.Text = lc.Text + handling_tot + "</b></tr>";
                                }
                            }
                        }
                    }
                    else if (i == 3)
                    {
                        int start_first_row = 4;
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["CLIENT"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString() + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>=ROUND(SUM(J" + (ctr + start_first_row) + ",N" + (ctr + start_first_row) + "),2)</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan=7>Total</td><td>=ROUND(SUM(H" + start_first_row + ":H" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(I" + start_first_row + ":I" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M2:M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td></b></tr>";
                        }

                    }
                    else if (i == 4)
                    {
                        int start_first_row = 4;
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["billing_date"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Client_branch_code"].ToString() + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["pc_area"].ToString()) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["pc_rate"].ToString()) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["handling_charge"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>=ROUND(SUM(K" + (ctr + start_first_row) + ",O" + (ctr + start_first_row) + "),2)</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan=8 >Total</td><td>=ROUND(SUM(I" + start_first_row + ":I" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(J" + start_first_row + ":J" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(K2:K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td></b></tr>";
                        }

                    }
                    else if (i == 1)
                    {
                        int start_first_row = 4;
                        if (!ds.Tables[0].Rows[ctr]["conveyance_type"].ToString().Equals("3"))
                        {
                            km_per_rate_value = "<td>" + double.Parse(ds.Tables[0].Rows[ctr]["Conveyance_PerKmRate"].ToString()) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["conveyance_km"].ToString()) + "</td>";
                            col1 = "O"; col2 = "S";
                        }
                        else { col1 = "M"; col2 = "Q"; }
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["CLIENT"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["client_branch_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["EMP_NAME"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["designation"].ToString().ToUpper() + "</td>" + km_per_rate_value + "<td>" + double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + "</td><td>" + double.Parse(ds.Tables[0].Rows[ctr]["Service_Charge"].ToString()) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["total"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["Service_Charge"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>=ROUND(SUM(" + col1 + "" + (ctr + start_first_row) + "," + col2 + "" + (ctr + start_first_row) + "),2)</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            if (!ds.Tables[0].Rows[ctr]["conveyance_type"].ToString().Equals("3"))
                            {

                                lc.Text = lc.Text + "<tr><b><td align=center colspan=10>Total</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N2:N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(R" + start_first_row + ":R" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(S" + start_first_row + ":S" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(T" + start_first_row + ":T" + (ctr + start_first_row) + "),2)</td></b></tr>";
                            }
                            else
                            {
                                lc.Text = lc.Text + "<tr><b><td align=center colspan=10>Total</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(R" + start_first_row + ":R" + (ctr + start_first_row) + "),2)</td></tr>";
                                //lc.Text = lc.Text + "<tr><b><td align=center colspan=9>Total</td><td>=ROUND(SUM(H2:H" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(I2:I" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(J2:J" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(K2:K" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(L2:L" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(M2:M" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(N2:N" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(O2:O" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(P2:P" + (ctr + 2) + "),2)</td><td>=ROUND(SUM(Q2:Q" + (ctr + 2) + "),2)</td></b></tr>";
                            }
                        }
                    }
                    else if (i == 6)
                    {
                        int start_first_row = 4;
                        lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>'" + ds.Tables[0].Rows[ctr]["bill_invoice_no"].ToString() + "</td><td>'" + ds.Tables[0].Rows[ctr]["billing_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["fromtodate"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["CLIENT"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_name"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["unit_code"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["STATE_NAME"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["EMP_NAME"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["designation"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["conv_food_allowance_rate"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["food_allowance_days"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["food_total"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["conv_outstation_allowance_rate"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["outstation_allowance_days"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["out_total"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["conv_outstation_food_allowance_rate"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["outstation_food_allowance_days"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["out_food_total"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["conv_night_halt_rate"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["night_halt_days"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["night_total"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["km_rate"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["kms"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["km_total"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Subtotal_A"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["Service_Charge"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["sub_total"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["SGST"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["CGST"].ToString().ToUpper() + "</td><td>" + ds.Tables[0].Rows[ctr]["IGST"].ToString().ToUpper() + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td><td>" + Math.Round(double.Parse(ds.Tables[0].Rows[ctr]["sub_total"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["CGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["SGST"].ToString()) + double.Parse(ds.Tables[0].Rows[ctr]["IGST"].ToString()), 2) + "</td></tr>");
                        if (ds.Tables[0].Rows.Count == ctr + 1)
                        {
                            lc.Text = lc.Text + "<tr><b><td align=center colspan= 10>Total</td><td>=ROUND(SUM(K" + start_first_row + ":K" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(L" + start_first_row + ":L" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(M" + start_first_row + ":M" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(N" + start_first_row + ":N" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(O" + start_first_row + ":O" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(P" + start_first_row + ":P" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Q" + start_first_row + ":Q" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(R" + start_first_row + ":R" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(S" + start_first_row + ":S" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(T" + start_first_row + ":T" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(U" + start_first_row + ":U" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(V" + start_first_row + ":V" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(W" + start_first_row + ":W" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(X" + start_first_row + ":X" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Y" + start_first_row + ":Y" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(Z" + start_first_row + ":Z" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AA" + start_first_row + ":AA" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AB" + start_first_row + ":AB" + (ctr + start_first_row) + "),2)</td><td></td><td></td><td>=ROUND(SUM(AE" + start_first_row + ":AE" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AF" + start_first_row + ":AF" + (ctr + start_first_row) + "),2)</td><td>=ROUND(SUM(AG" + start_first_row + ":AG" + (ctr + start_first_row) + "),2)</td></b></tr>";
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
    #endregion
}