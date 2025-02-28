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

public partial class Return_material : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();
    DAL d_sum = new DAL();
    DAL d2 = new DAL();
    DAL d3 = new DAL();
    DAL d4 = new DAL();
    DAL d5 = new DAL();
    DAL d6 = new DAL();
    DAL d7 = new DAL();
    // DAL d8 = new DAL();
    //DAL d9 = new DAL();
    DAL d10 = new DAL();
    DAL d11 = new DAL();
    // DAL d12 = new DAL();
    DAL d34 = new DAL();
    double doubleoinword1;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["COMP_CODE"] == null || Session["COMP_CODE"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }
        if (!IsPostBack)
        {
            txt_month_year.Text = d.getCurrentMonthYear();
            client_code();
            Panel6.Visible = false;
            Panel_invoice.Visible = false;
            btn_Save.Visible = false;
            Panel_receiver_name.Visible = false;
            Panel_du_receiver_date.Visible = false;
            Panel_dub_receiver_name.Visible = false;
            Panel_id.Visible = false;
            Panel_id_set.Visible = false;
            btn_update.Visible = false;
            report_panel.Visible = false;
            desigpanel.Visible = false;
            pod_number.Visible = false;
            Panel_du_pod.Visible = false;
            dispatch_date_sec.Visible = false;
            Sec_Panel_receiver_name.Visible = false;
            Sec_Panel_receiving_date.Visible = false;
            sec_pod_number.Visible = false;
            Panel_du_pod_3.Visible = false;
            Panel_dis_third.Visible = false;
            Panel_du_receiver_date_3.Visible = false;
            Panel_dub_receiver_name3.Visible = false;
            Panel_receiving_date.Visible = false;
            Panel_receiver_name.Visible = false;
            Panel_month.Visible = false;
            Print_Report.Visible = false;
            btn_uniform_shoes.Visible = false;
            second_btn_uniform_shoes.Visible = false;
            Panel_shipping.Visible = false;
            Panel_inv_no.Visible = false;
            Panel_grand_total.Visible = false;
            //load_grdview();
            Panel_inv_type.Visible = false;
            // load_inventary_gidview();
            Panel_dis_sec.Visible = false;
            // courier_gridview();
            //uniform_gridview();
            //gridview_invoice();
            btn_bill_save.Visible = false;
            ddl_sendmail_client.Items.Clear();
            ddl_client_name.Items.Clear();
            ddl_courier_client_name.Items.Clear();

            ddl_uniform_entry_client.Items.Clear();
            ddl_uniform_state_id.Items.Clear();
            ddl_uniform_branch.Items.Clear();
            ddl_uniform_emp_name.Items.Clear();


            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select client_name, client_code from pay_client_master where comp_code='" + Session["comp_code"] + "'  AND  client_code in(select distinct(client_code) from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE in (" + Session["REPORTING_EMP_SERIES"].ToString() + ")) and client_active_close=0 ORDER BY client_code", d.con);
            d.con.Open();
            try
            {
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_sendmail_client.DataSource = dt_item;
                    ddl_sendmail_client.DataTextField = dt_item.Columns[0].ToString();
                    ddl_sendmail_client.DataValueField = dt_item.Columns[1].ToString();
                    ddl_sendmail_client.DataBind();


                    // for dispatch billing 28-04-2020 komal
                    ddl_client_name.DataSource = dt_item;
                    ddl_client_name.DataTextField = dt_item.Columns[0].ToString();
                    ddl_client_name.DataValueField = dt_item.Columns[1].ToString();
                    ddl_client_name.DataBind();

                    //ddl_clientname1.DataSource = dt_item;
                    //ddl_clientname1.DataTextField = dt_item.Columns[0].ToString();
                    //ddl_clientname1.DataValueField = dt_item.Columns[1].ToString();
                    //ddl_clientname1.DataBind();
                    //// end


                    // for courier tab
                    ddl_courier_client_name.DataSource = dt_item;
                    ddl_courier_client_name.DataTextField = dt_item.Columns[0].ToString();
                    ddl_courier_client_name.DataValueField = dt_item.Columns[1].ToString();
                    ddl_courier_client_name.DataBind();
                    // end


                    ddl_uniform_entry_client.DataSource = dt_item;
                    ddl_uniform_entry_client.DataTextField = dt_item.Columns[0].ToString();
                    ddl_uniform_entry_client.DataValueField = dt_item.Columns[1].ToString();
                    ddl_uniform_entry_client.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_sendmail_client.Items.Insert(0, "Select");
                ddl_state.Items.Insert(0, "ALL");
                ddl_sendmail_unit.Items.Insert(0, "ALL");
                ddl_client_name.Items.Insert(0, "Select");
                // ddl_clientname1.Items.Insert(0, "Select");
                ddl_state_name.Items.Insert(0, "Select");
                ddl_branch_name.Items.Insert(0, "ALL");
                ddl_emp_name.Items.Insert(0, "Select");

                ddl_courier_client_name.Items.Insert(0, "Select");
                ddl_courier_state_name.Items.Insert(0, "Select");

                ddl_uniform_entry_client.Items.Insert(0, "Select");
                ddl_uniform_state_id.Items.Insert(0, "Select");
                ddl_uniform_state_id.Items.Insert(1, "ALL");
                ddl_uniform_branch.Items.Insert(0, "Select");
                ddl_uniform_emp_name.Items.Insert(0, "Select");
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
            ddl_unitcode.Items.Clear();
            ddl_billing_state.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("select distinct(state) from pay_transaction where comp_code='" + Session["comp_code"] + "' and CUST_CODE = '" + ddl_client.SelectedValue + "' AND  state  in (select distinct(state_name) from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "'  AND client_code='" + ddl_client.SelectedValue + "') order by 1", d.con);
            d.con.Open();
            try
            {
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
                ddl_billing_state.Items.Insert(0, "Select");
                ddl_uniform_state_id.Items.Insert(0, "Select");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
        else
        {
            ddl_unitcode.Items.Clear();
        }
    }
    protected void ddl_client_SelectedIndexChanged1(object sender, EventArgs e)
    {
        ddl_unitcode.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select DISTINCT(CONCAT((SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_NAME,'_',UNIT_ADD1)) as UNIT_NAME, pay_transaction.branch_name,flag from pay_transaction inner join pay_unit_master on pay_transaction.branch_name=pay_unit_master.unit_code and pay_transaction.comp_code=pay_unit_master.comp_code  where pay_transaction.comp_code='" + Session["comp_code"] + "' and pay_transaction.CUST_CODE = '" + ddl_client.SelectedValue + "' AND UNIT_CODE in(select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "'  AND client_code='" + ddl_client.SelectedValue + "') and state='" + ddl_billing_state.SelectedValue + "'  ORDER BY state", d.con);
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
            ddl_unitcode.Items.Insert(0, "Select");

        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    protected void fill_gridview(object sender, EventArgs e)
    {
        gv_fill();
    }

    protected void validation_num(object sender, EventArgs e)
    {

        foreach (GridViewRow gvrow in ItemGridView.Rows)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("select COALESCE(sum(return_qty), 0) AS return_qty from pay_return_material_details where comp_code='" + Session["comp_code"].ToString() + "' and item_code='" + gvrow.Cells[1].Text + "' and invoice_no='" + txt_invoice_num.SelectedValue + "'", d.con);
                d.con.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    string return_qty = dr.GetValue(0).ToString();

                    if (((System.Web.UI.WebControls.TextBox)gvrow.FindControl("txt_returnqty")).Text != "")
                    {
                        int return_qty1 = int.Parse(return_qty) + int.Parse(((System.Web.UI.WebControls.TextBox)gvrow.FindControl("txt_returnqty")).Text);
                        // if (int.Parse(gvrow.Cells[4].Text) < int.Parse(((System.Web.UI.WebControls.TextBox)gvrow.FindControl("txt_returnqty")).Text))
                        if (int.Parse(gvrow.Cells[4].Text) < (return_qty1))
                        {
                            ((System.Web.UI.WebControls.TextBox)gvrow.FindControl("txt_returnqty")).Text = return_qty;
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Return Quantity Should not be greater than Quantity!')", true);
                            btn_Save.Visible = false;
                        }
                        else
                        {
                            btn_Save.Visible = true;
                        }
                    }
                }
                dr.Close();
                d.con.Close();
            }
            catch { }
        }
    }
    //protected void validation_num(object sender, EventArgs e)
    //{

    //    foreach (GridViewRow gvrow in ItemGridView.Rows)
    //    {
    //        try
    //        {
    //            MySqlCommand cmd = new MySqlCommand("select sum(return_qty) from pay_return_material_details where comp_code='"+Session["comp_code"].ToString()+"' and item_code='" + gvrow.Cells[1].Text + "'", d.con);
    //            d.con.Open();
    //            MySqlDataReader dr = cmd.ExecuteReader();
    //            if (dr.Read())
    //            {
    //                string return_qty = dr.GetValue(0).ToString();

    //                if (((System.Web.UI.WebControls.TextBox)gvrow.FindControl("txt_returnqty")).Text != "")
    //                {
    //                    int return_qty1 = int.Parse(return_qty)+int.Parse(((System.Web.UI.WebControls.TextBox)gvrow.FindControl("txt_returnqty")).Text);
    //                   // if (int.Parse(gvrow.Cells[4].Text) < int.Parse(((System.Web.UI.WebControls.TextBox)gvrow.FindControl("txt_returnqty")).Text))
    //                    if (int.Parse(gvrow.Cells[4].Text) < (return_qty1))
    //                    {
    //                        ((System.Web.UI.WebControls.TextBox)gvrow.FindControl("txt_returnqty")).Text = return_qty;
    //                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Return Quantity Should not be greater than Quantity!')", true);
    //                    }
    //                }
    //            }
    //            dr.Close();
    //            d.con.Close();
    //        }
    //        catch{}
    //    }
    //}


    protected void client_code()
    {
        ddl_client.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        //MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select client_name, client_code from pay_client_master where comp_code='" + Session["comp_code"] + "' AND  client_code in(select distinct(client_code) from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE='" + Session["LOGIN_ID"].ToString() + "') ORDER BY client_code", d.con);
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select DISTINCT(CUST_CODE), CUST_NAME from pay_transaction where pay_transaction.comp_code='" + Session["comp_code"] + "' ", d.con);
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_client.DataSource = dt_item;
                ddl_client.DataTextField = dt_item.Columns[1].ToString();
                ddl_client.DataValueField = dt_item.Columns[0].ToString();
                ddl_client.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
            ddl_client.Items.Insert(0, "Select");
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }

    }

    protected void gv_fill()
    {
        System.Data.DataTable dt_item = new System.Data.DataTable();

        MySqlDataAdapter ads1 = new MySqlDataAdapter("select invoice_no from pay_return_material_details where comp_code='" + Session["comp_code"].ToString() + "' and  invoice_no='" + txt_invoice_num.SelectedValue + "'", d.con);
        d.con.Open();
        try
        {
            ads1.Fill(dt_item);
            if (dt_item.Rows.Count == 0)
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("select item_type as 'item_type',ITEM_CODE,PARTICULAR,DESCRIPTION,QUANTITY,0 as Return_Material,POD_NUM from pay_transaction_details where DOC_NO='" + txt_invoice_num.SelectedValue + "' and comp_code='" + Session["comp_code"].ToString() + "' ", d.con);

                DataSet ds = new DataSet();
                adp.Fill(ds);
                ItemGridView.DataSource = ds.Tables[0];
                ItemGridView.DataBind();

            }
            else
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT item_type, ITEM_CODE,item_name AS 'PARTICULAR',DESCRIPTION, QUANTIRY as 'QUANTITY',sum( return_qty) AS 'Return_Material', POD_NUM FROM pay_return_material_details where invoice_no='" + txt_invoice_num.SelectedValue + "' and comp_code='" + Session["comp_code"].ToString() + "' group by item_type ", d.con);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                ItemGridView.DataSource = ds.Tables[0];
                ItemGridView.DataBind();
            }
        }
        catch (Exception ex)
        { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    protected void btn_close_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }
    protected void dispatch_bill_clear()
    {
        ddl_material_contract.SelectedValue = "Select";
        ddl_client_name.SelectedValue = "Select";
        ddl_state_name.SelectedValue = "Select";
        ddl_branch_name.SelectedValue = "ALL";
        txt_invoice_num.SelectedValue = "Select";
        ddl_emp_name.SelectedValue = "Select";
        txt_bill_rtn_date.Text = "";
        ddl_designation.Items.Insert(0, "Select");


    }
    protected void clear()
    {
        ddl_client.SelectedValue = "Select";
        ddl_billing_state.SelectedValue = "Select";
        ddl_unitcode.SelectedValue = "Select";
        txt_invoice_num.SelectedValue = "Select";
        ddl_product.SelectedValue = "";
        txt_start_date.Text = "";
        txt_return_accepted_by.Text = "";
        txt_reason.Text = "";
        ItemGridView.DataSource = null;
        ItemGridView.DataBind();
    }
    protected void btn_save_Click(object sender, EventArgs e)
    {
        try
        {

            //System.Data.DataTable dt_item = new System.Data.DataTable();
            //MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select invoice_no from pay_return_material_master where invoice_no='" + txt_invoice_num.SelectedValue + "'  ", d.con);
            //d.con.Open();
            //cmd_item.Fill(dt_item);
            //if (dt_item.Rows.Count > 0)
            //{

            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Material Already Returned...');", true);
            //}
            //else
            //  {

            int res;
            res = d.operation("insert into pay_return_material_master(client_name,state,branch_name,material_type,invoice_no,return_date,accepted_by,reason)values('" + ddl_client.SelectedValue + "','" + ddl_billing_state.SelectedValue + "','" + ddl_unitcode.SelectedValue + "','" + ddl_product.SelectedValue + "','" + txt_invoice_num.SelectedValue + "','" + txt_start_date.Text + "','" + txt_return_accepted_by.Text + "','" + txt_reason.Text + "')");

            foreach (GridViewRow row in ItemGridView.Rows)
            {
                string item_code = row.Cells[1].Text;
                string item_type1 = row.Cells[0].Text;
                string lbl_item_name1 = row.Cells[2].Text;
                string lbl_item_description1 = row.Cells[3].Text;
                string lbl_item_qty1 = row.Cells[4].Text;

                TextBox txt_returnqty = (TextBox)row.FindControl("txt_returnqty");
                string return_qty = (txt_returnqty.Text);

                TextBox txt_opd_no = (TextBox)row.FindControl("txt_opd_no");
                string txt_opd_no1 = (txt_opd_no.Text);

                d.operation("insert into pay_return_material_details(item_code,item_name,item_type,description,quantiry,invoice_no,comp_code,POD_NUM,return_qty)values('" + item_code.ToString() + "','" + lbl_item_name1.ToString() + "','" + item_type1.ToString() + "','" + lbl_item_description1.ToString() + "','" + lbl_item_qty1.ToString() + "','" + txt_invoice_num.SelectedValue + "','" + Session["comp_code"].ToString() + "','" + txt_opd_no1 + "','" + return_qty + "')");
                if (res > 0)
                {
                    try
                    {
                        //  item_result = item_result + insert_result;
                        string sum_inward = (" select sum(quantiry) from pay_return_material_details  where Item_Code='" + item_code + "' AND COMP_CODE = '" + Session["COMP_CODE"] + "' AND invoice_no='" + txt_invoice_num.SelectedValue + "' ");
                        MySqlCommand cmd_sum = new MySqlCommand(sum_inward, d_sum.con1);
                        d_sum.con1.Open();
                        MySqlDataReader dr_sum_inward = cmd_sum.ExecuteReader();
                        if (dr_sum_inward.Read())
                        {
                            doubleoinword1 = Convert.ToDouble(return_qty);
                            // doubleoinword1 = Convert.ToDouble(dr_sum_inward.GetValue(0).ToString());
                            //  int updaterecord = d.operation("update pay_item_master SET inward ='" + doubleoinword + "' where Item_Code='" + item_code + "'");
                        }
                        dr_sum_inward.Close();
                        cmd_sum.Dispose();
                        d_sum.con1.Close();
                        string query1 = (" select inward,outward,Stock from pay_item_master  where Item_Code='" + item_code + "' AND COMP_CODE = '" + Session["COMP_CODE"] + "' ");

                        MySqlCommand cmd1 = new MySqlCommand(query1, d.con);
                        d.con.Open();
                        MySqlDataReader sda = cmd1.ExecuteReader();

                        if (sda.Read())
                        {
                            double doubleoinword = Convert.ToDouble(sda.GetValue(0).ToString());
                            double doubleoutword = Convert.ToDouble(sda.GetValue(1).ToString());
                            double doublestock = Convert.ToDouble(sda.GetValue(2).ToString());
                            doublestock = doubleoinword - doubleoutword;
                            int updaterecord = d.operation("update pay_item_master SET inward ='" + doubleoinword + "',outward='" + (doubleoutword - doubleoinword1) + "' ,Stock= '" + (doublestock + doubleoinword1) + "'  where Item_Code='" + item_code + "'");
                        }
                    }

                    catch { }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Material Return Successfully...');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Material Return Fail...');", true);
                }
            }
            // }
            d.con.Close();
        }
        catch (Exception ex)
        { throw ex; }
        finally
        {
            d.con.Close();
            clear();
        }
    }

    protected void fill_invoice_no(object sender, EventArgs e)
    {
        try
        {
            d.con.Open();
            MySqlDataAdapter adp = new MySqlDataAdapter("select DOC_NO from pay_transaction where comp_code='" + Session["comp_code"].ToString() + "' and CUST_CODE='" + ddl_client.SelectedValue + "' and branch_name='" + ddl_unitcode.SelectedValue + "' and state='" + ddl_billing_state.SelectedValue + "'", d.con);
            DataTable ds = new DataTable();
            adp.Fill(ds);
            if (ds.Rows.Count > 0)
            {
                txt_invoice_num.DataSource = ds;
                txt_invoice_num.DataTextField = ds.Columns[0].ToString();
                txt_invoice_num.DataValueField = ds.Columns[0].ToString();
                txt_invoice_num.DataBind();
            }
            adp.Dispose();
        }
        catch (Exception ex)
        { throw ex; }
        finally
        {
            d.con.Close();
            txt_invoice_num.Items.Insert(0, "Select");
        }


    }
    protected void ItemGridView_PreRender(object sender, EventArgs e)
    {
        try
        {
            ItemGridView.UseAccessibleHeader = false;
            ItemGridView.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
    private void load_grdview()
    {
        MySqlDataAdapter MySqlDataAdapter1 = new MySqlDataAdapter("SELECT id,(select client_name from pay_client_master where pay_client_master.client_code = pay_bill_invoices.client_code) as client_code, state_name,(select unit_name from pay_unit_master where pay_unit_master.unit_code = pay_bill_invoices.unit_code and pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "') as unit_code,month,year,bill_type,invoice_number,date_format(dispatch_date,'%d/%m/%Y') as dispatch_date,date_format(receiving_date,'%d/%m/%Y') as receiving_date,pod_number,concat('~/approved_bills/',shipping_file) as Value FROM pay_bill_invoices where comp_Code = '" + Session["COMP_CODE"].ToString() + "'", d.con1);
        DataSet DS1 = new DataSet();
        try
        {
            d.con1.Open();
            MySqlDataAdapter1.Fill(DS1);
            gv_bill_material.DataSource = DS1;
            gv_bill_material.DataBind();
            d.con1.Close();
        }
        catch (Exception ex) { throw ex; }
        finally { d.con1.Close(); }
    }
    private void load_inventary_gidview()
    {

        MySqlDataAdapter MySqlDataAdapter1 = new MySqlDataAdapter("SELECT pay_document_details.ID, pay_client_master.client_name, EMP_CURRENT_STATE AS 'state_name', (SELECT unit_name FROM pay_unit_master WHERE pay_unit_master.comp_code = pay_document_details.comp_code AND pay_unit_master.unit_code = pay_document_details.unit_code) AS 'unit_name', emp_name, document_type, DATE_FORMAT(dispatch_date, '%d/%m/%Y') AS 'dispatch_date', DATE_FORMAT(receiving_date, '%d/%m/%Y') AS 'receiving_date', pod_number, CONCAT('~/approved_bills/', shipping_file) AS 'filename' FROM pay_document_details INNER JOIN pay_employee_master ON pay_document_details.comp_code = pay_employee_master.comp_code AND pay_document_details.emp_code = pay_employee_master.emp_code INNER JOIN pay_client_master ON pay_document_details.comp_code = pay_client_master.comp_code AND pay_document_details.client_code = pay_client_master.client_code WHERE client_active_close = '0' AND pay_client_master.comp_Code = '" + Session["COMP_CODE"].ToString() + "' ", d.con1);
        DataSet DS1 = new DataSet();
        try
        {
            d.con1.Open();
            MySqlDataAdapter1.Fill(DS1);
            grd_inv_material.DataSource = DS1;
            grd_inv_material.DataBind();
            d.con1.Close();
        }
        catch (Exception ex) { throw ex; }
        finally { d.con1.Close(); }
    }
    protected void gv_bill_material_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_bill_material.UseAccessibleHeader = false;
            gv_bill_material.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }
    //protected void gv_bill_material_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        d.con.Open();
    //        string id = gv_bill_material.SelectedRow.Cells[1].Text;
    //        MySqlCommand cmd = new MySqlCommand("select invoice_number,date_format(dispatch_date,'%d/%m/%Y') as dispatch_date,date_format(receiving_date,'%d/%m/%Y') as receiving_date,pod_number,shipping_address from pay_bill_invoices where Id='" + id + "' ", d.con);
    //        MySqlDataReader dr = cmd.ExecuteReader();
    //        if (dr.Read())
    //        {
    //            lbl_invoice_num.Text = dr.GetValue(0).ToString();
    //            txt_bill_rtn_date.Text = dr.GetValue(1).ToString();
    //            txt_receiv_date.Text = dr.GetValue(2).ToString();
    //            txt_pod_number.Text = dr.GetValue(3).ToString();
    //            txt_bill_reason.Text = dr.GetValue(4).ToString();
    //        }
    //        btn_bill_save.Visible = true;
    //        dr.Close();
    //        d.con.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        d.con.Close();
    //    }

    //}
    protected void gv_bill_material_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
        //    e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
        //    e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.gv_bill_material, "Select$" + e.Row.RowIndex);
        //}
        e.Row.Cells[5].Text = get_month(e.Row.Cells[5].Text);
        e.Row.Cells[1].Visible = false;
    }
    protected void btn_bill_save_Click(object sender, EventArgs e)
    {
        if (check_date(txt_bill_rtn_date.Text, txt_receiv_date.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Receiving date should be greater than dispatch date!!!')", true);
            return;
        }
        hidtab.Value = "1";
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);

            if (txt_report_receiving.HasFile)
            {
                string fileExt = System.IO.Path.GetExtension(txt_report_receiving.FileName);
                if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG")
                {
                    string fileName = Path.GetFileName(txt_report_receiving.PostedFile.FileName);
                    txt_report_receiving.PostedFile.SaveAs(Server.MapPath("~/approved_bills/") + fileName);

                    File.Copy(Server.MapPath("~/approved_bills/") + fileName, Server.MapPath("~/approved_bills/") + Session["COMP_CODE"].ToString() + "_" + lbl_invoice_num.Text.Replace("-", "_") + fileExt, true);
                    File.Delete(Server.MapPath("~/approved_bills/") + fileName);

                    d.operation("update pay_bill_invoices set dispatch_date=str_to_date('" + txt_bill_rtn_date.Text + "','%d/%m/%Y'),receiving_date=str_to_date('" + txt_receiv_date.Text + "','%d/%m/%Y'),pod_number='" + txt_pod_number.Text + "',shipping_address='" + txt_bill_reason.Text + "', updated_by='" + Session["LOGIN_ID"].ToString() + "', updated_date=now(), shipping_file = '" + Session["COMP_CODE"].ToString() + "_" + lbl_invoice_num.Text.Replace("-", "_") + fileExt + "' where invoice_number = '" + lbl_invoice_num.Text + "'");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please select only JPG, PNG and PDF Files !!!')", true);
                    return;
                }
            }
            else
            {
                d.operation("update pay_bill_invoices set dispatch_date=str_to_date('" + txt_bill_rtn_date.Text + "','%d/%m/%Y'),receiving_date=str_to_date('" + txt_receiv_date.Text + "','%d/%m/%Y'),pod_number='" + txt_pod_number.Text + "',shipping_address='" + txt_bill_reason.Text + "', updated_by='" + Session["LOGIN_ID"].ToString() + "', updated_date=now() where invoice_number = '" + lbl_invoice_num.Text + "'");
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Data updated Successfully !!!')", true);
            load_grdview();
            btn_bill_save.Visible = false;
            lbl_invoice_num.Text = "";
            txt_bill_rtn_date.Text = "";
            txt_receiv_date.Text = "";
            txt_pod_number.Text = "";
            txt_bill_reason.Text = "";
        }
        catch (Exception ex) { throw ex; }
        finally { }
    }
    private string get_month(string month)
    {
        if (month == "1") { return "January"; }
        if (month == "2") { return "February"; }
        if (month == "3") { return "March"; }
        if (month == "4") { return "April"; }
        if (month == "5") { return "May"; }
        if (month == "6") { return "June"; }
        if (month == "7") { return "July"; }
        if (month == "8") { return "August"; }
        if (month == "9") { return "September"; }
        if (month == "10") { return "October"; }
        if (month == "11") { return "November"; }
        if (month == "12") { return "December"; }
        return month;
    }
    protected void DownloadFile(object sender, EventArgs e)
    {
        try
        {
            string filePath = (sender as LinkButton).CommandArgument;
            Response.ContentType = ContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            Response.WriteFile(filePath);
            Response.End();
        }
        catch { }
    }
    protected void lnkedit_Click(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        try
        {
            d.con.Open();
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
            string id = grdrow.Cells[1].Text;
            MySqlCommand cmd = new MySqlCommand("select invoice_number,date_format(dispatch_date,'%d/%m/%Y') as dispatch_date,date_format(receiving_date,'%d/%m/%Y') as receiving_date,pod_number,shipping_address from pay_bill_invoices where Id='" + id + "' ", d.con);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                lbl_invoice_num.Text = dr.GetValue(0).ToString();
                txt_bill_rtn_date.Text = dr.GetValue(1).ToString();
                txt_receiv_date.Text = dr.GetValue(2).ToString();
                txt_pod_number.Text = dr.GetValue(3).ToString();
                txt_bill_reason.Text = dr.GetValue(4).ToString();
            }
            btn_bill_save.Visible = true;
            dr.Close();
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

    protected void lnkinv_edit_Click(object sender, EventArgs e)
    {
        hidtab.Value = "2";
        try
        {
            d.con.Open();
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
            string id = grdrow.Cells[1].Text;
            ViewState["inv_id"] = id;
            MySqlCommand cmd = new MySqlCommand("SELECT emp_name, DATE_FORMAT(dispatch_date, '%d/%m/%Y') AS 'dispatch_date', DATE_FORMAT(receiving_date, '%d/%m/%Y') AS 'receiving_date', pod_number, IF((shipping_address IS NULL || shipping_address = ''), UNIT_ADD2, shipping_address) AS 'shipping_address', document_type FROM pay_document_details INNER JOIN pay_employee_master ON pay_document_details.comp_code = pay_employee_master.comp_code AND pay_document_details.emp_code = pay_employee_master.emp_code INNER JOIN pay_unit_master ON pay_document_details.comp_code = pay_unit_master.comp_code AND pay_document_details.unit_code = pay_unit_master.unit_code WHERE pay_document_details.id = '" + id + "' ", d.con);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                lbl_employee.Text = dr.GetValue(0).ToString();
                txt_inv_dispath_date.Text = dr.GetValue(1).ToString();
                txt_inv_receiving_date.Text = dr.GetValue(2).ToString();
                txt_inv_pod_no.Text = dr.GetValue(3).ToString();
                txt_inv_shipping_add.Text = dr.GetValue(4).ToString();
                lbl_document.Text = dr.GetValue(5).ToString();
            }
            btn_bill_save.Visible = true;
            dr.Close();
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
    protected void btn_inv_save_Click(object sender, EventArgs e)
    {
        if (check_date(txt_inv_dispath_date.Text, txt_inv_receiving_date.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Receiving date should be greater than dispatch date!!!')", true);
            return;
        }
        hidtab.Value = "2";
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            if (fup_inv_upload.HasFile)
            {
                string fileExt = System.IO.Path.GetExtension(fup_inv_upload.FileName);
                if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG")
                {
                    string fname = d.getsinglestring("select concat(emp_code,'-',document_type) from pay_document_details where id = '" + ViewState["inv_id"].ToString() + "'");
                    string fileName = Path.GetFileName(fup_inv_upload.PostedFile.FileName);
                    fup_inv_upload.PostedFile.SaveAs(Server.MapPath("~/approved_bills/") + fileName);

                    File.Copy(Server.MapPath("~/approved_bills/") + fileName, Server.MapPath("~/approved_bills/") + fname + fileExt, true);
                    File.Delete(Server.MapPath("~/approved_bills/") + fileName);

                    d.operation("update pay_document_details set dispatch_date=str_to_date('" + txt_inv_dispath_date.Text + "','%d/%m/%Y'),receiving_date=str_to_date('" + txt_inv_receiving_date.Text + "','%d/%m/%Y'),pod_number='" + txt_inv_pod_no.Text + "',shipping_address='" + txt_inv_shipping_add.Text + "', updated_by='" + Session["LOGIN_ID"].ToString() + "', updated_date=now(), shipping_file = '" + fname + fileExt + "' where ID = '" + ViewState["inv_id"].ToString() + "'");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please select only JPG, PNG and PDF Files !!!')", true);
                    return;
                }
            }
            else
            {
                d.operation("update pay_document_details set dispatch_date=str_to_date('" + txt_inv_dispath_date.Text + "','%d/%m/%Y'),receiving_date=str_to_date('" + txt_inv_receiving_date.Text + "','%d/%m/%Y'),pod_number='" + txt_inv_pod_no.Text + "',shipping_address='" + txt_inv_shipping_add.Text + "', updated_by='" + Session["LOGIN_ID"].ToString() + "', updated_date=now() where ID = '" + ViewState["inv_id"].ToString() + "'");
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Data updated Successfully !!!')", true);
            load_inventary_gidview();

            lbl_employee.Text = "";
            txt_inv_dispath_date.Text = "";
            txt_inv_receiving_date.Text = "";
            txt_inv_pod_no.Text = "";
            txt_inv_shipping_add.Text = "";
            lbl_document.Text = "";
        }
        catch (Exception ex) { throw ex; }
        finally { }
    }
    protected void grd_inv_material_RowDataBound(object sender, GridViewRowEventArgs e)
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
    protected void grd_inv_material_PreRender(object sender, EventArgs e)
    {
        try
        {
            grd_inv_material.UseAccessibleHeader = false;
            grd_inv_material.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }

    public bool check_date(string dispatching_date, string resiving_date)
    {

        try
        {
            //DateTime dispatch_date = DateTime.Parse(dispatching_date);
            //DateTime resive_date = DateTime.Parse(resiving_date);
            DateTime dispatch_date = DateTime.ParseExact(dispatching_date, "dd/MM/yyyy", null);
            DateTime resive_date = DateTime.ParseExact(resiving_date, "dd/MM/yyyy", null);

            if (dispatch_date.Date > resive_date.Date)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        { return false; }

    }
    // for second dispatch 

    public bool second_dispatch(string first_dispatch_date, string second_dispatch_date)
    {

        try
        {

            DateTime first_dispatch = DateTime.ParseExact(first_dispatch_date, "dd/MM/yyyy", null);
            DateTime second_dispatch = DateTime.ParseExact(second_dispatch_date, "dd/MM/yyyy", null);

            if (first_dispatch.Date > second_dispatch.Date)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        { return false; }


    }

    // for second resiving date

    public bool second_resiving(string second_dispatch_date, string second_resiving_date)
    {

        try
        {

            DateTime second_dispatch_1 = DateTime.ParseExact(second_dispatch_date, "dd/MM/yyyy", null);
            DateTime second_resiving = DateTime.ParseExact(second_resiving_date, "dd/MM/yyyy", null);

            if (second_dispatch_1.Date > second_resiving.Date)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        { return false; }


    }





    protected void ddl_client_SelectedIndexChanged2(object sender, EventArgs e)
    {
        hidtab.Value = "3";
        if (ddl_sendmail_client.SelectedValue != "ALL")
        {

            d1.con1.Open();
            ddl_state.Items.Clear();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter("SELECT distinct state FROM pay_designation_count where CLIENT_CODE = '" + ddl_sendmail_client.SelectedValue + "' and state in (select state_name from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE in(" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_sendmail_client.SelectedValue + "')  ORDER BY STATE", d1.con1);
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


            ddl_sendmail_unit.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_sendmail_client.SelectedValue + "' AND state_name ='" + ddl_state.SelectedValue + "' and UNIT_CODE in(select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE='" + Session["LOGIN_ID"].ToString() + "' AND client_code='" + ddl_sendmail_client.SelectedValue + "' AND state_name='" + ddl_state.SelectedValue + "') ORDER BY UNIT_CODE", d.con);
            d.con.Open();
            try
            {
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_sendmail_unit.DataSource = dt_item;
                    ddl_sendmail_unit.DataTextField = dt_item.Columns[0].ToString();
                    ddl_sendmail_unit.DataValueField = dt_item.Columns[1].ToString();
                    ddl_sendmail_unit.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_sendmail_unit.Items.Insert(0, "ALL");
                ddl_sendmail_unit.SelectedIndex = 0;
                ddl_state_SelectedIndexChanged(null, null);
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
        hidtab.Value = "3";
        if (ddl_sendmail_client.SelectedValue != "ALL")
        {
            ddl_sendmail_unit.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_sendmail_client.SelectedValue + "' and pay_unit_master.state_name = '" + ddl_state.SelectedValue + "' and  pay_unit_master.UNIT_CODE  in ( select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE in(" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_sendmail_client.SelectedValue + "' AND state_name='" + ddl_state.SelectedValue + "')   ORDER BY pay_unit_master.state_name", d.con);
            d.con.Open();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_sendmail_unit.DataSource = dt_item;
                    ddl_sendmail_unit.DataTextField = dt_item.Columns[0].ToString();
                    ddl_sendmail_unit.DataValueField = dt_item.Columns[1].ToString();
                    ddl_sendmail_unit.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_sendmail_unit.Items.Insert(0, "ALL");
                ddl_sendmail_unit.SelectedIndex = 0;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
    }
    protected void btn_send_email_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        hidtab.Value = "3";
        create_letter_pdf("", 1);
    }
    private void create_letter_pdf(string unit_code, int counter1)
    {
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);

            string where = "pay_unit_master.client_code = '" + ddl_sendmail_client.SelectedValue + "' and pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "'";
            if (unit_code == "")
            {
                if (ddl_state.SelectedValue != "ALL")
                {
                    where = where + " and pay_unit_master.state_name = '" + ddl_state.SelectedValue + "'";
                }
                if (ddl_sendmail_unit.SelectedValue != "ALL")
                {
                    where = where + " and pay_unit_master.unit_code = '" + ddl_sendmail_unit.SelectedValue + "'";
                }
            }
            else
            {
                where = "comp_code = '" + Session["COMP_CODE"].ToString() + "' and unit_code = '" + unit_code + "'";
            }
            d.con1.Open();
            MySqlCommand cmd = new MySqlCommand("select pay_unit_master.comp_code, pay_unit_master.unit_code,pay_unit_master.client_code,pay_unit_master.state_name from pay_dispatch_billing inner join pay_unit_master on pay_dispatch_billing.unit_code = pay_unit_master.unit_code and pay_dispatch_billing.comp_code = pay_unit_master.comp_code and pay_dispatch_billing.client_code = pay_unit_master.client_code INNER JOIN pay_document_details ON pay_dispatch_billing.unit_code = pay_document_details.unit_code AND pay_dispatch_billing.comp_code = pay_document_details.comp_code AND pay_dispatch_billing.client_code = pay_document_details.client_code where pay_document_details.email_sent=0 and pay_dispatch_billing.pod_no is not null and pay_unit_master.branch_status = 0 and " + where + " group by pay_unit_master.comp_code, pay_unit_master.unit_code ", d.con1);
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ReportDocument crystalReport = new ReportDocument();
                string query = null;

                crystalReport.Load(Server.MapPath("~/uniform.rpt"));
                string companyimagepath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Images\\") + d.getsinglestring("SELECT comp_logo from pay_company_master where comp_code = '" + dr.GetValue(0).ToString() + "'");
                //  crystalReport.DataDefinition.FormulaFields["company_image_path"].Text = @"'" + companyimagepath + "'";
                query = "SELECT company_name, pay_unit_master.unit_code, emp_name, GRADE_DESC AS 'grade_desc',  concat( pay_company_master.ADDRESS1 ,',', pay_company_master.CITY,',', pay_company_master.STATE) as   address1   , CONCAT(client_name, ',', pay_dispatch_billing.shipping_address) AS 'unit_add2', document_type AS 'unit_name', no_of_set AS 'headerpath', size AS 'leftfooterpath',pay_dispatch_billing.pod_no AS 'rightfooterpath', DATE_FORMAT(pay_dispatch_billing.dispatch_date, '%d/%m/%Y') AS 'stappath' FROM pay_employee_master  INNER JOIN pay_dispatch_billing ON pay_employee_master.comp_code = pay_dispatch_billing.comp_code AND pay_employee_master.emp_code = pay_dispatch_billing.emp_code INNER JOIN pay_document_details ON pay_employee_master.comp_code = pay_document_details.comp_code AND pay_employee_master.emp_code = pay_document_details.emp_code INNER JOIN pay_grade_master ON pay_grade_master.comp_code = pay_employee_master.comp_code AND pay_grade_master.GRADE_CODE = pay_employee_master.GRADE_CODE INNER JOIN pay_client_master ON pay_client_master.comp_code = pay_document_details.comp_code AND pay_client_master.client_code = pay_document_details.client_code INNER JOIN pay_company_master ON pay_company_master.comp_code = pay_document_details.comp_code INNER JOIN pay_unit_master ON pay_document_details.unit_code = pay_unit_master.unit_code AND pay_document_details.comp_code = pay_unit_master.comp_code where email_sent=0 and pay_dispatch_billing.pod_no is not null and pay_unit_master.comp_code = '" + dr.GetValue(0).ToString() + "' and pay_unit_master.unit_code='" + dr.GetValue(1).ToString() + "'and pay_unit_master.client_code = '" + dr.GetValue(2).ToString() + "' and pay_unit_master.state_name = '" + dr.GetValue(3).ToString() + "' GROUP BY emp_name,document_type  order by emp_name";

                System.Data.DataTable dt = new System.Data.DataTable();
                MySqlCommand cmd1 = new MySqlCommand(query);
                MySqlDataReader sda = null;
                cmd1.Connection = d.con;
                d.con.Open();
                sda = cmd1.ExecuteReader();
                dt.Load(sda);
                d.con.Close();
                string body = "";
                if (dt.Rows.Count > 0)
                {
                    int i = 0;
                    body = "<B><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2>Hi, <p>Please find the attached Inventory details for the following employees. </FONT></FONT></FONT></B><p><Table border =1><tr><th><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2>SR. No.</FONT></FONT></FONT></th><th><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2>EMPLOYEE NAME</FONT></FONT></FONT></th><th><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2>DESIGNATION</FONT></FONT></FONT></th></tr>";
                    foreach (DataRow row in dt.Rows)
                    {
                        body = body + "<tr><td><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2>" + ++i + "</FONT></FONT></FONT></td><td><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2>" + row["EMP_NAME"].ToString() + "</FONT></FONT></FONT></td><td><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2>" + row["grade_desc"].ToString() + "</FONT></FONT></FONT></td></tr>";
                    }
                    body = body + "</Table> <p>";
                    crystalReport.Refresh();
                    crystalReport.SetDataSource(dt);
                    crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Downloads\\") + "Inventory.pdf");
                    crystalReport.Close();
                    crystalReport.Clone();
                    crystalReport.Dispose();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    try
                    {
                        d.con.Open();
                        MySqlCommand cmdnew = new MySqlCommand("SET SESSION group_concat_max_len = 100000;select cast(group_concat(distinct head_email_id) as char), head_name, client_code,comp_code,state from pay_branch_mail_details where comp_code = '" + dr.GetValue(0).ToString() + "' and unit_code = '" + dr.GetValue(1).ToString() + "'", d.con);
                        MySqlDataReader drnew = cmdnew.ExecuteReader();
                        System.Data.DataTable DataTable1 = new System.Data.DataTable();
                        DataTable1.Load(drnew);
                        d.con.Close();
                        if (!IsEmptyGrid(DataTable1))
                        {
                            foreach (DataRow row in DataTable1.Rows)
                            {
                                mail_send(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), 3, "IH&MS - Inventory Confirmation", ddl_state.SelectedValue, dr.GetValue(1).ToString(), counter1, body);
                            }
                        }
                    }
                    catch (Exception ex) { throw ex; }
                    finally { d.con.Close(); }
                }
            }
            d.con1.Close();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
            d.con1.Close();
            File.Delete(System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Downloads\\") + "Inventory.pdf");
        }
    }
    private Boolean IsEmptyGrid(DataTable datatable)
    {
        for (int i = 0; i < datatable.Rows.Count; i++)
        {
            for (int j = 0; j < datatable.Columns.Count; j++)
            {
                if (!string.IsNullOrEmpty(datatable.Rows[i][j].ToString()))
                    return false;
            }
        }
        return true;
    }
    protected void btn_emails_not_sent_Click(object sender, EventArgs e)
    {
        hidtab.Value = "3";
        string where = "pay_unit_master.client_code = '" + ddl_sendmail_client.SelectedValue + "'";
        if (ddl_state.SelectedValue != "ALL") { where = where + " and pay_unit_master.state_name = '" + ddl_state.SelectedValue + "'"; }
        if (ddl_sendmail_unit.SelectedValue != "ALL") { where = where + " and pay_unit_master.unit_code = '" + ddl_sendmail_unit.SelectedValue + "'"; }

        MySqlDataAdapter MySqlDataAdapter1 = new MySqlDataAdapter("SELECT (SELECT client_name FROM pay_client_master WHERE pay_client_master.client_code = pay_employee_master.client_code ) as 'client_code', pay_unit_master.state_name, unit_name, emp_name ,case when email_sent = '0' then 'NO' else 'YES' end AS 'email_sent',DATE_FORMAT( email_sent_date , '%d/%m/%Y') AS 'joining_letter_sent_date',document_type FROM pay_employee_master  INNER JOIN pay_dispatch_billing ON pay_employee_master.comp_code = pay_dispatch_billing.comp_code AND pay_employee_master.emp_code = pay_dispatch_billing.emp_code inner join pay_unit_master on pay_unit_master.unit_code = pay_employee_master.unit_code and pay_unit_master.comp_code = pay_employee_master.comp_code left join pay_document_details on pay_employee_master.emp_code=pay_document_details.emp_code and  pay_employee_master.comp_code=pay_document_details.comp_code and pay_employee_master.unit_code = pay_document_details.unit_code WHERE " + where + " and left_date is null and pay_employee_master.legal_flag=2  and pod_no is not null order by 1,2,3,4", d.con1);
        try
        {
            DataTable DS1 = new DataTable();
            d.con1.Open();
            MySqlDataAdapter1.Fill(DS1);
            gv_itemslist.DataSource = DS1;
            gv_itemslist.DataBind();
            d.con1.Close();
        }
        catch (Exception ex) { throw ex; }
        finally { d.con1.Close(); }


    }
    protected void gv_itemslist_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
    }
    protected void gv_itemslist_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_itemslist.UseAccessibleHeader = false;
            gv_itemslist.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }
    protected void mail_send(string head_email_id, string head_email_name, string client_name, string comp_code, int counter, string subject, string state_name, string unit_code, int counter1, string body1)
    {
        List<string> list1 = new List<string>();
        string from_emailid = "", password = "";
        try
        {
            d.con.Open();
            MySqlCommand cmd = new MySqlCommand("select email_id,password from pay_client_master where client_code = '" + client_name + "' ", d.con);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                from_emailid = dr.GetValue(0).ToString();
                password = dr.GetValue(1).ToString();
            }
            dr.Close();
            d.con.Close();
            if (!(from_emailid == "") || !(password == ""))
            {
                string body = body1;
                string name = d.getsinglestring("select group_concat( Field4 ,'<br />', Field5 ,'<br />Mobile - ', Field6 , '<br />Immediate Manager - Chaitali Nilawar(manager@ihmsindia.com)</FONT></FONT></FONT></B>') as 'ss' from pay_zone_master where type='client_Email' and  Field1 = 'Operation' and client_code='" + client_name + "' and comp_code='" + Session["comp_code"].ToString() + "'");
                body = body + "<B><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2><br />Thanks & Regards,<br />" + name + "";

                //if (client_name == "BALIC")
                //{
                //    body = body + "<B><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2><br />Thanks & Regards,<br />Santosh Ghurade<br />Admin and OPS<br />Mobile - 9325431471<br />Immediate Manager - Jayati Roy(jayatiroy@ihmsindia.com)</FONT></FONT></FONT></B>";
                //}
                //else if (client_name == "BAGIC")
                //{
                //    body = body + "<B><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2><br />Thanks & Regards,<br />Samiksha<br />Admin and OPS<br />Mobile - 9067159872<br />Immediate Manager - Jayati Roy(jayatiroy@ihmsindia.com)</FONT></FONT></FONT></B>";
                //}
                //else if (client_name == "MAX" || client_name == "AEG" || client_name == "5" || client_name == "7" || client_name == "8" || client_name == "ICICI HK" || client_name == "ESFB" || client_name == "TBZ")
                //{
                //    body = body + "<B><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2><br />Thanks & Regards,<br />SNEHAL GHADGE<br />Admin and OPS<br />Mobile - 8308925811<br />Immediate Manager - Jayati Roy(jayatiroy@ihmsindia.com)</FONT></FONT></FONT></B>";
                //}
                //else if (client_name == "RLIC HK" || client_name == "RCFL" || client_name == "RCPL")
                //{
                //    body = body + "<B><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2><br />Thanks & Regards,<br />CHAITALI<br />Admin and OPS<br />Mobile - 8805814003<br />Immediate Manager - Jayati Roy(jayatiroy@ihmsindia.com)</FONT></FONT></FONT></B>";
                //}
                //else if (client_name == "SUD" || client_name == "UTKARSH" || client_name == "HDFC" || client_name == "TAVISKA" || client_name == "SUN" || client_name == "DAF" || client_name == "TBML" || client_name == "BRLI")
                //{
                //    body = body + "<B><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2><br />Thanks & Regards,<br />SNEHAL GHADGE<br />Admin and OPS<br />Mobile - 8308925811<br />Immediate Manager - Jayati Roy(jayatiroy@ihmsindia.com)</FONT></FONT></FONT></B>";
                //}
                //else if (client_name == "4" || client_name == "RBL")
                //{
                //    body = body + "<B><FONT COLOR=\"#17365d\"><FONT FACE=\"Verdana, serif\"><FONT SIZE=2><br />Thanks & Regards,<br />RAHUL<br />Admin and OPS<br />Mobile - 7057919614<br />Immediate Manager - Jayati Roy(jayatiroy@ihmsindia.com)</FONT></FONT></FONT></B>";
                //}
                using (MailMessage mailMessage = new MailMessage())
                {
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    mailMessage.From = new MailAddress(from_emailid);

                    if (head_email_id != "")
                    {

                        mailMessage.To.Add(head_email_id);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.Attachments.Add(new Attachment(System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/."), "Downloads\\") + "Inventory.pdf"));

                        mailMessage.IsBodyHtml = true;
                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(from_emailid, password);
                        SmtpServer.EnableSsl = true;
                        try
                        {
                            SmtpServer.Send(mailMessage);
                            d.operation("update pay_document_details set email_sent =1, email_sent_date = now() where comp_code = '" + Session["COMP_CODE"].ToString() + "' and unit_code = '" + unit_code + "'");
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Sent successfully!!');", true);
                        }
                        catch
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Error in Sending Email !!');", true);

                        }
                        Thread.Sleep(500);
                    }
                }
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
    protected void ddl_client_name_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        gv_invoice.DataSource = null;
        gv_invoice.DataBind();
        gv_dublicate_id_card.DataSource = null;
        gv_dublicate_id_card.DataBind();
        gv_material.DataSource = null;
        gv_material.DataBind();

        if (ddl_client_name.SelectedValue != "Select")
        {

            d1.con1.Open();
            ddl_state_name.Items.Clear();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);


                if (ddl_Receiver_type.SelectedValue == "2")
                {
                    MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter("select distinct(STATE_NAME) as state from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('01/" + txt_month_year.Text + "', '%d/%m/%Y')) order by 1", d1.con1);
                    DataSet DS = new DataSet();
                    MySqlDataAdapter.Fill(DS);
                    ddl_state_name.DataSource = DS;
                    ddl_state_name.DataBind();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                    ddl_state_name.Items.Insert(0, "Select");
                    ddl_state_name.Items.Insert(1, "ALL");
                    region();
                }
                else
                {
                    MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter("select distinct(STATE_NAME) as state from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client_name.SelectedValue + "'AND (branch_close_date IS NULL || branch_close_date = '') order by 1", d1.con1);
                    DataSet DS = new DataSet();
                    MySqlDataAdapter.Fill(DS);
                    ddl_state_name.DataSource = DS;
                    ddl_state_name.DataBind();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                    ddl_state_name.Items.Insert(0, "Select");


                }

            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d1.con1.Close();
            }

            //Material
            if (ddl_Receiver_type.SelectedValue == "1")
            {

                //Panel_month.Visible = false;
                //Panel_inv_type.Visible = false;

                uniform_gridview();

            }
            if (ddl_Receiver_type.SelectedValue == "2")
            {
                //Panel_month.Visible = true;
                //Panel_inv_no.Visible = false;
                //Panel_inv_date.Visible = false;
                //Panel_shipping.Visible = false;
                //Panel_inv_type.Visible = true;
                //btn_uniform_shoes.Visible = false;

                gridview_invoice();

            }
            if (ddl_Receiver_type.SelectedValue == "3")
            {

                //Panel_inv_no.Visible = false;
                //Panel_inv_date.Visible = false;
                //Panel_shipping.Visible = false;
                //btn_uniform_shoes.Visible = false;
                //Panel_month.Visible = false;
                //btn_dispatch_save.Visible = false;

                gridview_dublicate_id();


            }


        }

    }
    protected void shipping_state()
    {
        d10.con.Open();
        try
        {

            string state_add = null;

            if (ddl_state_name.SelectedValue == "ALL")
            {
                if (ddl_inv_details.SelectedValue == "1")
                {
                    state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_unit_rate_history  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'  ");
                    //state_add = "'" + state_add + "'";
                    //state_add = state_add.Replace(",", "','");

                }
                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {
                    state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_material_history  where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'  ");

                }

                // changes by komal 21-05-2020

                else if (ddl_inv_details.SelectedValue == "5")
                {
                    state_add = d.get_group_concat("select group_concat(distinct(pay_billing_material_history.STATE_NAME)) from pay_billing_material_history  INNER JOIN pay_billing_unit_rate_history ON pay_billing_material_history.comp_code = pay_billing_unit_rate_history.comp_code AND pay_billing_material_history.client_code = pay_billing_unit_rate_history.client_code AND pay_billing_material_history.unit_code = pay_billing_unit_rate_history.unit_code where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'  ");

                }


                state_add = "'" + state_add + "'";
                state_add = state_add.Replace(",", "','");

            }
            else if (ddl_state_name.SelectedValue != "ALL")
            {

                Session["STATE_CODE"] = ddl_state_name.SelectedValue;
                state_add = "'" + Session["STATE_CODE"].ToString() + "'";

            }

            string all_statewise = null;
            if (ddl_inv_details.SelectedValue == "5")
            {
                all_statewise = d.getsinglestring("SELECT group_concat(billing_id) FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "' AND state = '" + ddl_state_name.SelectedValue + "'");

                all_statewise = all_statewise.Replace(",", "','");
            }
            else
            {

                all_statewise = "" + ddl_inv_details.SelectedValue + "";
            }

            string ship_billing = d.getsinglestring("SELECT DISTINCT billing_name,billing_wise FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "' and state IN(" + state_add + ") and billing_id in('" + all_statewise + "')");
            string ship_wise = d.getsinglestring("SELECT DISTINCT billing_wise FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "'and state IN(" + state_add + ")  and billing_id in('" + all_statewise + "')");

            MySqlCommand unit_address = new MySqlCommand("select distinct invoice_shipping_address from pay_client_billing_details where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and state IN(" + state_add + ") and billing_name = '" + ship_billing + "' and billing_wise = '" + ship_wise + "' and billing_id in('" + all_statewise + "') ", d10.con);
            MySqlDataReader unit_reader = unit_address.ExecuteReader();

            if (unit_reader.Read())
            {
                txt_bill_reason.Text = unit_reader.GetValue(0).ToString();
            }
            unit_reader.Close();

        }
        catch (Exception ex) { throw ex; }
        finally { d10.con.Close(); }

    }


    protected void inv_flag()
    {
        try
        {
            d11.con.Open();
            //  string invoice_flag = null;

            string billing_wise = d.getsinglestring("SELECT DISTINCT billing_wise FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "'");


            // string state_add = d.get_group_concat("SELECT group_concat(state_name) FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "' AND billing_wise = '" + billing_wise + "' AND state IN (SELECT distinct state_name FROM pay_unit_master WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date IS NULL || branch_close_date = '' || branch_close_date >= STR_TO_DATE('01/" + txt_month_year.Text + "', '%d/%m/%Y'))) ");
            string state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('01/" + txt_month_year.Text + "', '%d/%m/%Y')) order by 1");
            state_add = state_add.Replace(",", "','");
            string region = "";
            if (ddl_client_name.SelectedValue == "DHFL" || (ddl_client_name.SelectedValue == "BAGIC" && ddl_state_name.SelectedValue == "Maharashtra" && int.Parse("" + txt_month_year.Text.Substring(3) + "" + txt_month_year.Text.Substring(0, 2) + "") > 20204))
            { region = "AND Zone = '" + ddlregion.SelectedValue + "'"; }


            if (ddl_state_name.SelectedValue != "ALL")
            {
                if (ddl_state_name.SelectedValue != "Select")
                {

                    MySqlCommand invoice_cmd1 = null;
                    string group_data1 = "";
                    DataTable dt = new DataTable();

                    if (ddl_inv_details.SelectedValue == "1")
                    {
                        invoice_cmd1 = new MySqlCommand("SELECT distinct state_name  FROM pay_billing_unit_rate_history  WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "'  and state_name = '" + ddl_state_name.SelectedValue + "' and invoice_flag !='2' " + region + "  AND auto_invoice_no IS NOT NULL", d11.con);
                    }
                    else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                    {
                        invoice_cmd1 = new MySqlCommand("SELECT distinct state_name  FROM pay_billing_material_history  WHERE pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' AND pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "'  and state_name = '" + ddl_state_name.SelectedValue + "' and invoice_flag !='2' AND auto_invoice_no IS NOT NULL", d11.con);

                    }
                    // change by komal 21-05-2020
                    else if (ddl_inv_details.SelectedValue == "5")
                    {
                        invoice_cmd1 = new MySqlCommand("SELECT distinct pay_billing_material_history.state_name  FROM pay_billing_material_history INNER JOIN pay_billing_unit_rate_history ON pay_billing_material_history.comp_code = pay_billing_unit_rate_history.comp_code AND pay_billing_material_history.client_code = pay_billing_unit_rate_history.client_code AND pay_billing_material_history.unit_code = pay_billing_unit_rate_history.unit_code  WHERE pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'   and pay_billing_material_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.year='" + txt_month_year.Text.Substring(3) + "'  and pay_billing_material_history.state_name = '" + ddl_state_name.SelectedValue + "' and pay_billing_material_history.invoice_flag !='2' AND pay_billing_material_history.auto_invoice_no IS NOT NULL", d11.con);

                    }

                    MySqlDataReader dt_item1 = invoice_cmd1.ExecuteReader();
                    while (dt_item1.Read())
                    {

                        group_data1 = group_data1 + dt_item1.GetValue(0).ToString() + ",";


                    }

                    if (group_data1 != "")
                    {

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('This State Not Approve By Final Athority : " + group_data1 + "');", true);
                        ddl_state_name.SelectedValue = "Select";
                        return;

                    }

                }


            }
            else if (ddl_state_name.SelectedValue == "ALL")
            {

                string group_data = "";
                MySqlCommand invoice_cmd = null;
                DataTable dt = new DataTable();

                if (ddl_inv_details.SelectedValue == "1")
                {

                    invoice_cmd = new MySqlCommand("SELECT distinct state_name  FROM pay_billing_unit_rate_history  WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "'  and state_name in('" + state_add + "') " + region + " and invoice_flag !='2' ", d11.con);
                }
                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {

                    invoice_cmd = new MySqlCommand("SELECT distinct state_name FROM pay_billing_material_history  WHERE pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "'  AND pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "'  and state_name in('" + state_add + "') and invoice_flag !='2' ", d11.con);
                }
                // change by komal 21-05-2020
                else if (ddl_inv_details.SelectedValue == "5")
                {
                    invoice_cmd = new MySqlCommand("SELECT distinct pay_billing_material_history.state_name  FROM pay_billing_material_history INNER JOIN pay_billing_unit_rate_history ON pay_billing_material_history.comp_code = pay_billing_unit_rate_history.comp_code AND pay_billing_material_history.client_code = pay_billing_unit_rate_history.client_code AND pay_billing_material_history.unit_code = pay_billing_unit_rate_history.unit_code  WHERE pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'   and pay_billing_material_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.year='" + txt_month_year.Text.Substring(3) + "'  and state_name in('" + state_add + "') and pay_billing_material_history.invoice_flag !='2' AND pay_billing_material_history.auto_invoice_no IS NOT NULL", d11.con);

                }

                MySqlDataReader dt_item = invoice_cmd.ExecuteReader();
                while (dt_item.Read())
                {

                    group_data = group_data + dt_item.GetValue(0).ToString() + ",";


                }

                if (group_data != "")
                {

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('This State Not Approve By MD : " + group_data + "');", true);
                    ddl_state_name.SelectedValue = "Select";
                    return;

                }

            }

        }
        catch (Exception ex) { throw ex; }
        finally { d11.con.Close(); }


    }

    protected void final_state_bill1()
    {

        try
        {

            d34.con.Open();
            MySqlCommand final_state_bill = null;
            string final = null;
            string region = "";
            if (ddl_client_name.SelectedValue == "DHFL" || (ddl_client_name.SelectedValue == "BAGIC" && ddl_state_name.SelectedValue == "Maharashtra" && int.Parse("" + txt_month_year.Text.Substring(3) + "" + txt_month_year.Text.Substring(0, 2) + "") > 20204))
            { region = "AND Zone = '" + ddlregion.SelectedValue + "'"; }

            if (ddl_state_name.SelectedValue == "ALL")
            {


                if (ddl_inv_details.SelectedValue == "1")
                {
                    final_state_bill = new MySqlCommand("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and branch_status='0' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('31/01/" + txt_month_year.Text + "', '%d/%m/%Y')) and STATE_NAME not in(select distinct state_name from pay_billing_unit_rate_history where comp_code = '" + Session["comp_code"].ToString() + "'and client_code = '" + ddl_client_name.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' " + region + " and invoice_flag = '2') " + region + " order by 1", d34.con);

                }

                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {
                    //final_state_bill = d.getsinglestring("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('31/01/" + txt_month_year.Text + "', '%d/%m/%Y')) and STATE_NAME not in(select distinct state_name from pay_billing_material_history where comp_code = '" + Session["comp_code"].ToString() + "'and client_code = '" + ddl_client_name.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2')order by 1");
                    final_state_bill = new MySqlCommand("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and branch_status='0' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('31/01/" + txt_month_year.Text + "', '%d/%m/%Y')) and STATE_NAME not in(select distinct state_name from pay_billing_material_history where comp_code = '" + Session["comp_code"].ToString() + "'and client_code = '" + ddl_client_name.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2')order by 1", d34.con);
                }
                else if (ddl_inv_details.SelectedValue == "5")
                {
                    //final_state_bill = d.getsinglestring("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('31/01/" + txt_month_year.Text + "', '%d/%m/%Y')) and STATE_NAME not in(select distinct state_name from pay_billing_material_history where comp_code = '" + Session["comp_code"].ToString() + "'and client_code = '" + ddl_client_name.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2')order by 1");
                    final_state_bill = new MySqlCommand("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and branch_status='0' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('31/01/" + txt_month_year.Text + "', '%d/%m/%Y')) and STATE_NAME not in(select distinct pay_billing_material_history.state_name from pay_billing_material_history  INNER JOIN pay_billing_unit_rate_history ON pay_billing_material_history.comp_code = pay_billing_unit_rate_history.comp_code AND pay_billing_material_history.client_code = pay_billing_unit_rate_history.client_code AND pay_billing_material_history.unit_code = pay_billing_unit_rate_history.unit_code where pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'  and pay_billing_material_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.year='" + txt_month_year.Text.Substring(3) + "' and pay_billing_material_history.invoice_flag = '2')order by 1", d34.con);
                }

                //  final_state_bill = final_state_bill.Replace(",", "','");
                MySqlDataReader dt_item1 = final_state_bill.ExecuteReader();
                while (dt_item1.Read())
                {

                    final = final + dt_item1.GetValue(0).ToString() + "";

                }

                if (final != "")
                {

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Final The Bill For this State " + final + "');", true);
                    ddl_state_name.SelectedValue = "Select";
                    return;
                }

            }
            if (ddl_state_name.SelectedValue != "ALL")
            {


                if (ddl_inv_details.SelectedValue == "1")
                {
                    final_state_bill = new MySqlCommand("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and branch_status='0' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('31/01/" + txt_month_year.Text + "', '%d/%m/%Y')) AND STATE_NAME = '" + ddl_state_name.SelectedValue + "' and STATE_NAME not in(select distinct state_name from pay_billing_unit_rate_history where comp_code = '" + Session["comp_code"].ToString() + "'and client_code = '" + ddl_client_name.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' " + region + " and invoice_flag = '2')order by 1", d34.con);

                }

                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {
                    //final_state_bill = d.getsinglestring("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('31/01/" + txt_month_year.Text + "', '%d/%m/%Y')) and STATE_NAME not in(select distinct state_name from pay_billing_material_history where comp_code = '" + Session["comp_code"].ToString() + "'and client_code = '" + ddl_client_name.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2')order by 1");
                    final_state_bill = new MySqlCommand("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and branch_status='0' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('31/01/" + txt_month_year.Text + "', '%d/%m/%Y')) AND STATE_NAME = '" + ddl_state_name.SelectedValue + "' and STATE_NAME not in(select distinct state_name from pay_billing_material_history where comp_code = '" + Session["comp_code"].ToString() + "'and client_code = '" + ddl_client_name.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2')order by 1", d34.con);
                }
                else if (ddl_inv_details.SelectedValue == "5")
                {
                    //final_state_bill = d.getsinglestring("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('31/01/" + txt_month_year.Text + "', '%d/%m/%Y')) and STATE_NAME not in(select distinct state_name from pay_billing_material_history where comp_code = '" + Session["comp_code"].ToString() + "'and client_code = '" + ddl_client_name.SelectedValue + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2')order by 1");
                    final_state_bill = new MySqlCommand("select group_concat(distinct(STATE_NAME)) from pay_unit_master where comp_code='" + Session["comp_code"].ToString() + "' and branch_status='0' and client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date is null ||branch_close_date  >= STR_TO_DATE('31/01/" + txt_month_year.Text + "', '%d/%m/%Y')) AND STATE_NAME = '" + ddl_state_name.SelectedValue + "' and STATE_NAME not in(select distinct pay_billing_material_history.state_name from pay_billing_material_history  INNER JOIN pay_billing_unit_rate_history ON pay_billing_material_history.comp_code = pay_billing_unit_rate_history.comp_code AND pay_billing_material_history.client_code = pay_billing_unit_rate_history.client_code AND pay_billing_material_history.unit_code = pay_billing_unit_rate_history.unit_code where pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'  and pay_billing_material_history.month='" + txt_month_year.Text.Substring(0, 2) + "' and pay_billing_material_history.year='" + txt_month_year.Text.Substring(3) + "' and pay_billing_material_history.invoice_flag = '2')order by 1", d34.con);
                }


                //  final_state_bill = final_state_bill.Replace(",", "','");
                MySqlDataReader dt_item_final = final_state_bill.ExecuteReader();
                while (dt_item_final.Read())
                {

                    final = final + dt_item_final.GetValue(0).ToString();

                }

                if (final != "")
                {

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Final The Bill For this State " + final + "');", true);
                    ddl_state_name.SelectedValue = "Select";
                    return;
                }


            }


        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d34.con.Close();
        }



    }


    protected void ddl_state_name_SelectedIndexChanged(object sender, EventArgs e)
    {


        hidtab.Value = "1";

        if (ddl_Receiver_type.SelectedValue == "2")
        {
            if (ddl_state_name.SelectedValue != "Select")
            {
                final_state_bill1();
            }
        }

        string region = "";
        if (ddl_Receiver_type.SelectedValue == "2")
        {

            if (ddl_client_name.SelectedValue == "DHFL" || (ddl_client_name.SelectedValue == "BAGIC" && ddl_state_name.SelectedValue == "Maharashtra" && int.Parse("" + txt_month_year.Text.Substring(3) + "" + txt_month_year.Text.Substring(0, 2) + "") > 20204))
            { region = "AND Zone = '" + ddlregion.SelectedValue + "'"; }

        }

        ddl_branch_name.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();

        MySqlDataAdapter cmd_item = null;

        if (ddl_state_name.SelectedValue != "Select" && ddl_Receiver_type.SelectedValue == "2")
        {
            inv_flag();
        }
        if (ddl_state_name.SelectedValue != "ALL")
        {

            cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client_name.SelectedValue + "' and pay_unit_master.state_name = '" + ddl_state_name.SelectedValue + "' and  pay_unit_master.UNIT_CODE  in ( select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND (branch_close_date is null || branch_close_date = '' ||branch_close_date  >= STR_TO_DATE('01/" + txt_month_year.Text + "', '%d/%m/%Y')) AND  EMP_CODE in(" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_client_name.SelectedValue + "' AND state_name='" + ddl_state_name.SelectedValue + "') " + region + " and branch_status='0' ORDER BY pay_unit_master.UNIT_NAME ASC", d.con);

        }

        else if (ddl_state_name.SelectedValue == "ALL")
        {

            string billing_wise = d.getsinglestring("SELECT DISTINCT billing_wise FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "'");
            string state_add = d.get_group_concat("SELECT group_concat(state) FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "' AND billing_wise = '" + billing_wise + "' AND state IN (SELECT distinct state_name FROM pay_unit_master WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "' AND (branch_close_date IS NULL || branch_close_date = '' || branch_close_date >= STR_TO_DATE('01/" + txt_month_year.Text + "', '%d/%m/%Y'))) ");
            state_add = state_add.Replace(",", "','");


            cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client_name.SelectedValue + "' and pay_unit_master.state_name IN ( '" + state_add + "') and  pay_unit_master.UNIT_CODE  in ( select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND (branch_close_date is null || branch_close_date = '' ||branch_close_date  >= STR_TO_DATE('01/" + txt_month_year.Text + "', '%d/%m/%Y')) AND  EMP_CODE in(" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_client_name.SelectedValue + "' AND state_name in ('" + state_add + "'))  " + region + " and branch_status='0' ORDER BY pay_unit_master.UNIT_NAME ASC", d.con);


        }

        d.con.Open();


        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_branch_name.DataSource = dt_item;
                ddl_branch_name.DataTextField = dt_item.Columns[0].ToString();
                ddl_branch_name.DataValueField = dt_item.Columns[1].ToString();
                ddl_branch_name.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();

            ddl_branch_name.Items.Insert(0, "Select");
            ddl_branch_name.Items.Insert(1, "ALL");
            ddl_branch_name.SelectedIndex = 0;

            if (ddl_state_name.SelectedValue != "Select")
            {

                shipping_state();
            }

            if (ddl_state_name.SelectedValue == "ALL")
            {

                Panel_shipping.Visible = false;
            }



            if (txt_month_year.Text != "" && ddl_Receiver_type.SelectedValue != "1")
            {

                billing_statewise();

            }

            //28-04-2020 komal changes
            // dispatch_resiver();
            //end

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        }


        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }



    }


    protected void billing_statewise()
    {
        string unit_code1 = null;
        string state_add = null;
        try
        {
            string all_statewise = null;
            if (ddl_inv_details.SelectedValue == "5")
            {
                all_statewise = d.getsinglestring("SELECT group_concat(billing_id) FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "' AND state = '" + ddl_state_name.SelectedValue + "'");

                all_statewise = all_statewise.Replace(",", "','");
            }
            else
            {

                all_statewise = "" + ddl_inv_details.SelectedValue + "";
            }


            string state = d.getsinglestring("SELECT DISTINCT billing_wise FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "' and billing_id in('" + all_statewise + "')");

            if (state == "Statewise")
            {

                if (ddl_state_name.SelectedValue == "ALL")
                {

                    if (ddl_inv_details.SelectedValue == "1")
                    {

                        state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_unit_rate_history  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "' and invoice_flag = '2' ");

                    }
                    else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                    {

                        state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_material_history  where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "' and invoice_flag = '2' ");

                    }
                    state_add = "'" + state_add + "'";
                    state_add = state_add.Replace(",", "','");

                }
                else if (ddl_state_name.SelectedValue != "ALL" && ddl_state_name.SelectedValue != "Select")
                {

                    Session["STATE_CODE"] = ddl_state_name.SelectedValue;
                    state_add = "'" + Session["STATE_CODE"].ToString() + "'";

                }

                if (ddl_state_name.SelectedValue != "Select")
                {
                    if (ddl_inv_details.SelectedValue == "1")
                    {
                        unit_code1 = d.get_group_concat("select group_concat(distinct(unit_code)) from pay_billing_unit_rate_history  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_unit_rate_history.STATE_NAME IN( " + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' ");
                    }
                    else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                    {

                        unit_code1 = d.get_group_concat("select group_concat(distinct(unit_code)) from pay_billing_material_history  where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' and pay_billing_material_history.STATE_NAME IN( " + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' ");

                    }
                    unit_code1 = "'" + unit_code1 + "'";
                    unit_code1 = unit_code1.Replace(",", "','");
                }

                //old query 18-01-2020
                //MySqlCommand ds_uniform = new MySqlCommand("SELECT auto_invoice_no, date_format(billing_date,'%d/%m/%Y'), ROUND(SUM(sub_total_c), 2) AS 'TOTAL'  FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_unit_rate_history.unit_code IN ('" + unit_code1 + "') and pay_billing_unit_rate_history.STATE_NAME = '" + ddl_state_name.SelectedValue +"'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' group by auto_invoice_no ", d.con);

                if (ddl_state_name.SelectedValue != "Select")
                {
                    // string auto_inv_not = d.getsinglestring("");

                    string auto_invoice_noo = d.getsinglestring("select group_concat(invoice_no) from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' AND pay_dispatch_billing.month = '" + txt_month_year.Text + "' and client_code = '" + ddl_client_name.SelectedValue + "'");
                    auto_invoice_noo = "'" + auto_invoice_noo + "'";
                    auto_invoice_noo = auto_invoice_noo.Replace(",", "','");
                    MySqlCommand ds_uniform = null;

                    if (ddl_inv_details.SelectedValue == "1")
                    {
                        ds_uniform = new MySqlCommand("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_unit_rate_history.unit_code IN (" + unit_code1 + ") and pay_billing_unit_rate_history.STATE_NAME IN (" + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' and auto_invoice_no not in (" + auto_invoice_noo + ") AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t", d.con);
                    }

                    else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                    {

                        ds_uniform = new MySqlCommand("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_material_history WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_material_history.unit_code IN (" + unit_code1 + ") and pay_billing_material_history.STATE_NAME IN (" + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' and auto_invoice_no not in (" + auto_invoice_noo + ") and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t", d.con);

                    }
                    else if (ddl_inv_details.SelectedValue == "5")
                    {

                        // for manpower komal 21-05-2020   
                        string unit_code_manpower = d.get_group_concat("select group_concat(distinct(unit_code)) from pay_billing_unit_rate_history  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_unit_rate_history.STATE_NAME IN( " + state_add + ") and invoice_flag = '2' ");
                        unit_code_manpower = unit_code_manpower.Replace(",", "','");

                        string manpower = d.getsinglestring("SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_unit_rate_history.unit_code IN ('" + unit_code_manpower + "') and pay_billing_unit_rate_history.STATE_NAME IN (" + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' and auto_invoice_no not in (" + auto_invoice_noo + ") AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");


                        // for man_con_dip komal 21-05-2020

                        string unit_code_man_con_dip = d.get_group_concat("select group_concat(distinct(unit_code)) from pay_billing_material_history  where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_material_history.STATE_NAME IN( " + state_add + ") and invoice_flag = '2' ");
                        unit_code_man_con_dip = unit_code_man_con_dip.Replace(",", "','");

                        string man_con_dip = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_material_history WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_material_history.unit_code IN ('" + unit_code_man_con_dip + "') and pay_billing_material_history.STATE_NAME IN (" + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' and auto_invoice_no not in (" + auto_invoice_noo + ") AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");

                        string all_billing = "" + manpower + "," + man_con_dip + "";
                        //all_billing = all_billing.Replace("", " ");

                        lbl_invoice_num.Text = " " + all_billing + " ";

                    }



                    d.con.Open();

                    if (ddl_inv_details.SelectedValue != "5")
                    {
                        MySqlDataReader dr_uniform = ds_uniform.ExecuteReader();

                        if (dr_uniform.Read())
                        {
                            lbl_invoice_num.Text = dr_uniform.GetValue(0).ToString();

                        }
                    }

                }
                Panel_inv_date.Visible = false;

            }
            else if (state == "Statewisedesignation")
            {

                Panel_invoice.Visible = true;

            }
        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }


    }

    protected void billing_statewisedesignation()
    {


        try
        {
            string state_add = null;

            string state1 = d.getsinglestring("SELECT DISTINCT billing_wise FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "'");

            if (state1 == "Statewisedesignation")
            {
                if (ddl_state_name.SelectedValue == "ALL")
                {
                    state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_unit_rate_history  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "' and invoice_flag = '2' ");
                    state_add = "'" + state_add + "'";
                    state_add = state_add.Replace(",", "','");
                }
                else if (ddl_state_name.SelectedValue != "ALL" && ddl_state_name.SelectedValue != "Select")
                {

                    Session["STATE_CODE"] = ddl_state_name.SelectedValue;
                    state_add = "'" + Session["STATE_CODE"].ToString() + "'";

                }



                // string unit_code = d.get_group_concat("select  group_concat(distinct(unit_code)) from pay_billing_unit_rate_history  INNER JOIN pay_grade_master ON pay_billing_unit_rate_history.comp_code = pay_grade_master.comp_code AND pay_billing_unit_rate_history.grade_desc = pay_grade_master.GRADE_DESC  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_unit_rate_history.STATE_NAME = '" + ddl_state_name.SelectedValue + "'  AND pay_billing_unit_rate_history.grade_desc = (SELECT grade_desc FROM pay_grade_master WHERE grade_code = '" + ddl_designation.SelectedValue + "' AND comp_code = '" + Session["comp_code"].ToString() + "' GROUP BY grade_desc) ");
                string unit_code = d.get_group_concat("select  group_concat(distinct(unit_code)) from pay_billing_unit_rate_history  INNER JOIN pay_grade_master ON pay_billing_unit_rate_history.comp_code = pay_grade_master.comp_code AND pay_billing_unit_rate_history.grade_desc = pay_grade_master.GRADE_DESC  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.STATE_NAME in (" + state_add + ") and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "' ");
                unit_code = unit_code.Replace(",", "','");

                MySqlCommand ds_uniform = null;
                if (ddl_designation.SelectedValue != "ALL")
                {
                    ds_uniform = new MySqlCommand("SELECT auto_invoice_no, date_format(billing_date,'%d/%m/%Y'), ROUND(SUM(sub_total_c), 2) AS 'TOTAL' FROM pay_billing_unit_rate_history   INNER JOIN pay_grade_master ON pay_billing_unit_rate_history.comp_code = pay_grade_master.comp_code AND pay_billing_unit_rate_history.grade_desc = pay_grade_master.GRADE_DESC WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_unit_rate_history.grade_desc = (select grade_desc from pay_grade_master where grade_code='" + ddl_designation.SelectedValue + "' and comp_code='" + Session["comp_code"].ToString() + "' group by grade_desc) and pay_billing_unit_rate_history.unit_code IN ( '" + unit_code + "' ) AND state_name = '" + ddl_state_name.SelectedValue + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' group by auto_invoice_no ", d.con);
                }
                else if (ddl_designation.SelectedValue == "ALL")
                {

                    string design_all = d.getsinglestring("SELECT group_concat(distinct (pay_grade_master.grade_code))FROM pay_grade_master INNER JOIN pay_billing_unit_rate_history ON pay_grade_master.comp_code = pay_billing_unit_rate_history.comp_code AND pay_grade_master.grade_desc = pay_billing_unit_rate_history.grade_desc WHERE pay_grade_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'  and pay_billing_unit_rate_history.unit_code IN ( '" + unit_code + "' ) AND state_name IN (" + state_add + ") and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "'");
                    design_all = design_all.Replace(",", "','");

                    string grade_code = d.getsinglestring("SELECT group_concat(grade_desc) FROM pay_grade_master WHERE grade_code IN ('" + design_all + "') AND comp_code = '" + Session["comp_code"].ToString() + "' ");
                    grade_code = grade_code.Replace(",", "','");


                    ds_uniform = new MySqlCommand("SELECT  group_concat(distinct(auto_invoice_no)) FROM pay_billing_unit_rate_history   INNER JOIN pay_grade_master ON pay_billing_unit_rate_history.comp_code = pay_grade_master.comp_code AND pay_billing_unit_rate_history.grade_desc = pay_grade_master.GRADE_DESC WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_unit_rate_history.grade_desc IN ('" + grade_code + "') and pay_billing_unit_rate_history.unit_code IN ( '" + unit_code + "' ) AND state_name IN (" + state_add + ") and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' ", d.con);

                }

                d.con.Open();
                MySqlDataReader dr_uniform = ds_uniform.ExecuteReader();

                if (dr_uniform.Read())
                {
                    lbl_invoice_num.Text = dr_uniform.GetValue(0).ToString();
                    //txt_invoice_date.Text = dr_uniform.GetValue(1).ToString();
                    // txt_grand_total.Text = dr_uniform.GetValue(2).ToString();
                }




            }
        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }

    }


    protected void billing_branchwise()
    {

        try
        {
            d.con.Open();
            string branch = d.getsinglestring("SELECT DISTINCT billing_wise FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "'");

            if (branch == "Branchwise")
            {

                MySqlCommand ds_uniform = new MySqlCommand("SELECT auto_invoice_no, date_format(billing_date,'%d/%m/%Y'), Amount  FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND state_name = '" + ddl_state_name.SelectedValue + "' and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and invoice_flag = '1' group by auto_invoice_no ", d.con);

                MySqlDataReader dr_uniform = ds_uniform.ExecuteReader();

                if (dr_uniform.Read())
                {
                    lbl_invoice_num.Text = dr_uniform.GetValue(0).ToString();
                    txt_invoice_date.Text = dr_uniform.GetValue(1).ToString();
                    txt_grand_total.Text = dr_uniform.GetValue(2).ToString();
                }




            }
        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }


    }







    protected void ddl_courier_client_name_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "4";
        if (ddl_courier_client_name.SelectedValue != "ALL")
        {

            d1.con1.Open();
            ddl_courier_state_name.Items.Clear();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter("SELECT distinct state FROM pay_designation_count where CLIENT_CODE = '" + ddl_courier_client_name.SelectedValue + "' and state in (select state_name from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE in(" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_courier_client_name.SelectedValue + "')  ORDER BY STATE", d1.con1);
                DataSet DS = new DataSet();
                MySqlDataAdapter.Fill(DS);
                ddl_courier_state_name.DataSource = DS;
                ddl_courier_state_name.DataBind();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                ddl_courier_state_name.Items.Insert(0, "Select");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d1.con1.Close();
            }


            ddl_courier_branch_name.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_courier_client_name.SelectedValue + "' AND state_name ='" + ddl_courier_state_name.SelectedValue + "' and UNIT_CODE in(select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE='" + Session["LOGIN_ID"].ToString() + "' AND client_code='" + ddl_courier_client_name.SelectedValue + "' AND state_name='" + ddl_courier_state_name.SelectedValue + "') ORDER BY UNIT_CODE", d.con);
            d.con.Open();
            try
            {
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_courier_branch_name.DataSource = dt_item;
                    ddl_courier_branch_name.DataTextField = dt_item.Columns[0].ToString();
                    ddl_courier_branch_name.DataValueField = dt_item.Columns[1].ToString();
                    ddl_courier_branch_name.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_courier_branch_name.Items.Insert(0, "ALL");
                ddl_courier_branch_name.SelectedIndex = 0;
                ddl_courier_state_name_SelectedIndexChanged(null, null);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }

        }



    }
    protected void ddl_courier_state_name_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "4";
        if (ddl_courier_client_name.SelectedValue != "ALL")
        {
            ddl_courier_branch_name.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_courier_client_name.SelectedValue + "' and pay_unit_master.state_name = '" + ddl_courier_state_name.SelectedValue + "' and  pay_unit_master.UNIT_CODE  in ( select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE in(" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_courier_client_name.SelectedValue + "' AND state_name='" + ddl_courier_state_name.SelectedValue + "')   ORDER BY pay_unit_master.state_name", d.con);
            d.con.Open();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_courier_branch_name.DataSource = dt_item;
                    ddl_courier_branch_name.DataTextField = dt_item.Columns[0].ToString();
                    ddl_courier_branch_name.DataValueField = dt_item.Columns[1].ToString();
                    ddl_courier_branch_name.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_courier_branch_name.Items.Insert(0, "Select");
                // ddl_sendmail_unit.SelectedIndex = 0;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
    }

    //komal 4-07-19 
    protected void btn_save_courier_Click(object sender, EventArgs e)
    {
        hidtab.Value = "4";
        try
        {
            //string id = (" select id from pay_courier_tracking where '" + Session["COMP_CODE"] + "','" + ddl_courier_branch_name.SelectedValue + "','" + ddl_courier_client_name.SelectedValue + "','" + ddl_courier_state_name.SelectedValue + "','" + ddl_courier_category.SelectedValue + "','" + courier_weight.Text + "','" + courier_packet.Text + "','" + courier_pod.Text + "','" + ddl_courier_deliver.SelectedValue + "','" + courier_date.Text + "','" + courier_address.Text + "' ");
            // ViewState["id"] = grd_courier.SelectedRow.Cells[2].Text;
            // d.operation("delete from pay_courier_tracking where '"+ ViewState["id"].ToString()+"'");
            int courier = d.operation("insert into pay_courier_tracking (comp_code,unit_code,client_code,state_courier,category,weight,packet,pod_no,deliver,date,address,units)value('" + Session["COMP_CODE"] + "','" + ddl_courier_branch_name.SelectedValue + "','" + ddl_courier_client_name.SelectedValue + "','" + ddl_courier_state_name.SelectedValue + "','" + ddl_courier_category.SelectedValue + "','" + courier_weight.Text + "','" + courier_packet.Text + "','" + courier_pod.Text + "','" + ddl_courier_deliver.SelectedValue + "','" + courier_date.Text + "','" + courier_address.Text + "','" + ddl_courier_unit.SelectedValue + "')");

            if (courier > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Courier Added Successfully... !!!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Courier Added Failed... !!!');", true);
            }


            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        }
        catch (Exception ex) { throw ex; }
        finally
        {

            courier_gridview();
            ddl_courier_category.SelectedValue = "Select";
            courier_date.Text = "";
            ddl_courier_client_name.SelectedValue = "Select";
            ddl_courier_state_name.SelectedValue = "Select";
            ddl_courier_branch_name.SelectedValue = "Select";
            courier_address.Text = "";
            courier_weight.Text = "";
            courier_packet.Text = "";
            courier_pod.Text = "";
            ddl_courier_deliver.SelectedValue = "No";
            ddl_courier_category.SelectedValue = "Select";



        }

    }


    protected void courier_gridview()
    {

        try
        {

            MySqlDataAdapter ds_courier = new MySqlDataAdapter("SELECT CASE category  WHEN 1 THEN 'Uniform'   WHEN 2 THEN 'Shoes' WHEN 3 THEN 'Id Card' WHEN 4 THEN 'Documents'  ELSE '' END AS 'category', pay_courier_tracking.id, state_courier,  weight,  packet,  pod_no,  date,  address,  client_name,  unit_name,  CASE deliver WHEN 0 THEN 'No' WHEN 1 THEN 'Yes' ELSE '' END AS 'deliver',CASE units WHEN 0 THEN 'Kg' WHEN 1 THEN 'Gram' ELSE '' END AS 'units'FROM pay_courier_tracking INNER JOIN pay_client_master ON pay_courier_tracking.client_code = pay_client_master.client_code AND pay_courier_tracking.comp_code = pay_client_master.comp_code INNER JOIN pay_unit_master ON pay_courier_tracking.unit_code = pay_unit_master.unit_code AND pay_courier_tracking.comp_code = pay_unit_master.comp_code  ", d.con);
            d.con.Open();
            DataTable courier_data = new DataTable();
            ds_courier.Fill(courier_data);
            if (courier_data.Rows.Count > 0)
            {

                grd_courier.DataSource = courier_data;
                grd_courier.DataBind();

            }

        }
        catch (Exception ex) { throw ex; }

        finally { d.con.Close(); }
    }


    protected void btn_update_courier_Click(object sender, EventArgs e)
    {
        hidtab.Value = "4";
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            ViewState["id"] = grd_courier.SelectedRow.Cells[1].Text;

            int res = 0;
            res = d.operation("UPDATE  pay_courier_tracking SET client_code = '" + ddl_courier_client_name.SelectedValue + "', unit_code = '" + ddl_courier_branch_name.SelectedValue + "', state_courier = '" + ddl_courier_state_name.SelectedValue + "', weight = '" + courier_weight.Text + "', packet = '" + courier_packet.Text + "', pod_no = '" + courier_pod.Text + "', date = '" + courier_date.Text + "', address = '" + courier_address.Text + "' where id = " + ViewState["id"].ToString());
            if (res > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Courier  Updated Successfully');", true);
            }
            courier_gridview();




        }
        catch (Exception ex) { throw ex; }
        finally { }




    }
    protected void btn_delete_courier_Click(object sender, EventArgs e)
    {

        hidtab.Value = "4";
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            ViewState["id"] = grd_courier.SelectedRow.Cells[1].Text;

            int res = 0;
            res = d.operation("delete from pay_courier_tracking  where id = " + ViewState["id"].ToString());
            if (res > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Courier  Deleted Successfully');", true);
            }
            courier_gridview();
        }

        catch (Exception ex) { throw ex; }
        finally { }

    }
    protected void grd_courier_RowDataBound(object sender, GridViewRowEventArgs e)
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
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grd_courier, "Select$" + e.Row.RowIndex);

        }


        e.Row.Cells[1].Visible = false;

    }
    protected void grd_courier_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["id"] = grd_courier.SelectedRow.Cells[1].Text;
        d2.con.Open();
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);

            MySqlCommand cmd_courier = new MySqlCommand("select category,client_code,unit_code,state_courier,weight,units,packet,pod_no,deliver,date,address from pay_courier_tracking where id = '" + ViewState["id"].ToString() + "' ", d2.con);
            MySqlDataReader dr_courier = cmd_courier.ExecuteReader();

            if (dr_courier.Read())
            {
                ddl_courier_category.SelectedValue = dr_courier.GetValue(0).ToString();
                ddl_courier_client_name.SelectedValue = dr_courier.GetValue(1).ToString();
                ddl_courier_client_name_SelectedIndexChanged(null, null);
                ddl_courier_state_name.SelectedValue = dr_courier.GetValue(3).ToString();
                ddl_courier_state_name_SelectedIndexChanged(null, null);
                ddl_courier_branch_name.SelectedValue = dr_courier.GetValue(2).ToString();
                courier_weight.Text = dr_courier.GetValue(4).ToString();
                ddl_courier_unit.Text = dr_courier.GetValue(5).ToString();
                courier_packet.Text = dr_courier.GetValue(6).ToString();
                courier_pod.Text = dr_courier.GetValue(7).ToString();
                ddl_courier_deliver.SelectedValue = dr_courier.GetValue(8).ToString();
                courier_date.Text = dr_courier.GetValue(9).ToString();
                courier_address.Text = dr_courier.GetValue(10).ToString();

            }
        }
        catch (Exception ex) { throw ex; }
        finally { d2.con.Close(); }

    }
    protected void grd_courier_PreRender(object sender, EventArgs e)
    {
        try
        {
            grd_courier.UseAccessibleHeader = false;
            grd_courier.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
    protected void ddl_courier_branch_name_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "4";
        courier_gridview();
    }
    protected void btn_dispatch_save_Click(object sender, EventArgs e)
    {
        string date = "";
        DateTime current_date = new DateTime();
        DateTime nextYear = new DateTime();
        DateTime Date1 = new DateTime();
        DateTime dis_date_new = new DateTime();
        DateTime nextYear1 = new DateTime();
        #region
        try
        {
            if (ddl_Receiver_type.SelectedValue == "2")
            {

                if (check1() == "1")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Same Invoice Number ... !!!');", true);

                }
                else
                {
                    // 27-03-2020 komal
                    // for billig date 
                    //  date_format(billing_date,'%d/%m/%Y') as billing_date
                    string dispatch_date_billing = d.getsinglestring("select distinct date_format(billing_date,'%d/%m/%Y') as 'billing_date' from pay_billing_unit_rate_history where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and state_name = '" + ddl_state_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "'");

                    if (dispatch_date_billing != "")
                    {

                        if (check_dispatch_date(txt_bill_rtn_date.Text, dispatch_date_billing))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Invoice Dispatch Date should be greater than Billing Date!!!')", true);
                            return;
                        }

                    }



                    string state_add = null;
                    if (ddl_state_name.SelectedValue == "ALL")
                    {

                        if (ddl_inv_details.SelectedValue == "1")
                        {

                            state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_unit_rate_history  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "' and invoice_flag = '2' ");
                        }
                        else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                        {

                            state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_material_history  where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' and invoice_flag = '2' ");

                        }
                        state_add = "'" + state_add + "'";
                        state_add = state_add.Replace(",", "','");

                    }
                    else if (ddl_state_name.SelectedValue != "ALL" && ddl_state_name.SelectedValue != "Select")
                    {

                        Session["STATE_CODE"] = ddl_state_name.SelectedValue;
                        state_add = "'" + Session["STATE_CODE"].ToString() + "'";

                    }

                    string final_bill = null;

                    if (ddl_inv_details.SelectedValue == "1")
                    {
                        final_bill = d.getsinglestring("select group_concat(auto_invoice_no) from pay_billing_unit_rate_history where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and state_name in (" + state_add + ") and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "'");
                    }
                    else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                    {
                        final_bill = d.getsinglestring("select group_concat(auto_invoice_no) from pay_billing_material_history where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' and state_name in (" + state_add + ") and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "'");
                    }

                    if (final_bill == "")
                    {

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Please First Final The Bill... !!!');", true);
                        return;
                    }


                    int dispatch = d.operation(" insert into pay_dispatch_billing(comp_code,client_code,unit_code,state_dispatch,dispatch_by,invoice_no,dispatch_date,receiving_date,shipping_address,invoice_date,receiver_name_invoice,grand_total,emp_code,designation,receiver_type,month,invoice_type,region_name)value ('" + Session["comp_code"].ToString() + "','" + ddl_client_name.SelectedValue + "','" + ddl_branch_name.SelectedValue + "','" + ddl_state_name.SelectedValue + "','" + ddl_material_contract.SelectedValue + "' , '" + lbl_invoice_num.Text + "' ,'" + txt_bill_rtn_date.Text + "', '" + txt_receiv_date.Text + "' ,'" + txt_bill_reason.Text + "','" + txt_invoice_date.Text + "','" + txt_receiver_name.Text + "','" + txt_grand_total.Text + "','" + ddl_emp_name.SelectedValue + "','" + ddl_designation.SelectedValue + "','" + ddl_Receiver_type.SelectedValue + "','" + txt_month_year.Text + "','" + ddl_inv_details.SelectedValue + "','" + ddlregion.SelectedValue + "')");
                    // int dispatch = d.operation("insert into pay_dispatch_billing(comp_code,client_code,unit_code,state_dispatch,dispatch_by,invoice_no,dispatch_date,receiving_date,shipping_address,invoice_date,receiver_name_invoice,grand_total,emp_code,designation,receiver_type,month)value ('" + Session["comp_code"].ToString() + "','" + ddl_client_name.SelectedValue + "','" + ddl_branch_name.SelectedValue + "','" + ddl_state_name.SelectedValue + "','" + ddl_material_contract.SelectedValue + "' , '" + lbl_invoice_num.Text + "' ,str_to_date('" + txt_bill_rtn_date.Text + "','%d/%m/%Y') , '" + txt_receiv_date.Text + "' ,'" + txt_bill_reason.Text + "','" + txt_invoice_date.Text + "','" + txt_receiver_name.Text + "','" + txt_grand_total.Text + "','" + ddl_emp_name.SelectedValue + "','" + ddl_designation.SelectedValue + "','" + ddl_Receiver_type.SelectedValue + "','" + txt_month_year.Text + "')");
                    if (dispatch > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Added Successfully... !!!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Added Failed... !!!');", true);
                    }
                    //uniform_gridview();
                    //gridview_invoice();
                    //  emp_Name_material();
                    dispatch_resiver();
                    // clear_text();

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                }
            }
            else if (ddl_Receiver_type.SelectedValue == "1")
            {
                //  date_format(start_date,'%d/%m/%Y') as 'start_date'
                string issue_date = d.getsinglestring("select distinct date_format(start_date,'%d/%m/%Y') as 'start_date' from pay_document_details where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "'  and unit_code = '" + ddl_branch_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and year_complete = 0 and start_date like '" + txt_bill_rtn_date.Text.Substring(6) + "%' ");

                if (issue_date != "")
                {

                    if (check_dispatch_usi(txt_bill_rtn_date.Text, issue_date))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Uniform,Shoes,Id Card Dispatch Date Should be greater than Issue Date!!!')", true);
                        return;
                    }

                }

                //string dispatch_from_ut = d.getsinglestring("select emp_code from pay_document_details where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "'  and unit_code = '" + ddl_branch_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and year_complete = 0 and No_of_set != 0 ");


                //if (dispatch_from_ut == "")
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' First Add Number of Set From Uniform Tracking ... !!!');", true);
                //    return;
                //}

                if (emp_Name_material() == "1")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Same Employee Added ... !!!');", true);
                    return;
                }
        #endregion
                try
                {
                    current_date = DateTime.ParseExact(txt_bill_rtn_date.Text.ToString(), "dd/MM/yyyy", null);
                    nextYear = current_date.AddYears(1);
                }
                catch { }
                try
                {
                     date = nextYear.ToString("dd/MM/yyyy", null);
                    Date1 = nextYear.Date;
                }
                catch { }
                string dispatch_date = d.getsinglestring("select max(pay_dispatch_billing.dispatch_date) from pay_dispatch_billing INNER JOIN pay_document_details ON pay_dispatch_billing.comp_code = pay_document_details.comp_code AND pay_dispatch_billing.emp_code = pay_document_details.emp_code where pay_dispatch_billing.comp_code = '" + Session["COMP_CODE"].ToString() + "' and  pay_dispatch_billing.client_code = '" + ddl_client_name.SelectedValue + "' and pay_dispatch_billing.unit_code = '" + ddl_branch_name.SelectedValue + "' and pay_dispatch_billing.emp_code = '" + ddl_emp_name.SelectedValue + "' and pay_dispatch_billing.receiver_type = '" + ddl_Receiver_type.SelectedValue + "' and year_complete = 0  AND dis_year_complete = 0 GROUP BY document_type ");


                if (dispatch_date != "")
                {
                    try
                    {
                        dis_date_new = DateTime.ParseExact(dispatch_date.ToString(), "dd/MM/yyyy", null);
                        nextYear1 = dis_date_new.AddYears(1);
                    }
                    catch { }

                    //  DateTime Date2 = Convert.ToDateTime(dispatch_date);

                    if (nextYear1 <= current_date)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Are you sure You want to send the Uniform!!!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Uniform Can Not dispatch for One Year!!!');", true);
                        return;
                    }
                }
                int dispatch1 = d.operation(" insert into pay_dispatch_billing(comp_code,client_code,unit_code,state_dispatch,dispatch_by,dispatch_date,receiving_date,shipping_address,receiver_name_invoice,emp_code,designation,receiver_type)value ('" + Session["comp_code"].ToString() + "','" + ddl_client_name.SelectedValue + "','" + ddl_branch_name.SelectedValue + "','" + ddl_state_name.SelectedValue + "','" + ddl_material_contract.SelectedValue + "'  ,'" + txt_bill_rtn_date.Text + "', '" + txt_receiv_date.Text + "','" + txt_bill_reason.Text + "','" + txt_receiver_name.Text + "','" + ddl_emp_name.SelectedValue + "','" + ddl_designation.SelectedValue + "','" + ddl_Receiver_type.SelectedValue + "')");
                //    int res = d.operation("UPDATE pay_document_details SET dispatch_date = '" + txt_bill_rtn_date.Text + "'  where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "'and emp_code = '" + ddl_emp_name.SelectedValue + "' ");
                if (dispatch1 > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Added Successfully... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Added Failed... !!!');", true);
                }

                dispatch_resiver();
                // clear_text();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);

            }
        }

        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }

    }

    protected void dublicate_id_clear()
    {

        ddl_material_contract.SelectedValue = "Select";
        ddl_client_name.SelectedValue = "Select";
        ddl_state_name.SelectedValue = "Select";
        ddl_branch_name.SelectedValue = "ALL";
        ddl_emp_name.SelectedValue = "Select";
        txt_receiver_name.Text = "";
        txt_receiv_date.Text = "";
        txt_bill_rtn_date.Text = "";
        txt_pod_number.Text = "";
        txt_bill_reason.Text = "";
        desigpanel.Visible = false;


    }


    protected void clear_text()
    {
        ddl_Receiver_type.SelectedValue = "0";
        ddl_material_contract.SelectedValue = "Select";
        ddl_client_name.SelectedValue = "Select";
        ddl_state_name.SelectedValue = "Select";
        ddl_branch_name.SelectedValue = "ALL";
        ddl_emp_name.SelectedValue = "Select";
        txt_receiver_name.Text = "";
        txt_receiv_date.Text = "";
        txt_month_year.Text = "";
        txt_bill_rtn_date.Text = "";
        txt_pod_number.Text = "";
        txt_bill_reason.Text = "";
        txt_sec_dispatch_date.Text = "";
        txt_sec_pod.Text = "";
        txt_sec_dispatch_date.Text = "";
        lbl_invoice_num.Text = "";
        desigpanel.Visible = false;


    }


    protected void uniform_gridview()
    {

        try
        {
            MySqlDataAdapter uniform = null;
            uniform = new MySqlDataAdapter("SELECT pay_document_details.uniform_no_flag, ifnull((SELECT  remaining_no_set FROM pay_document_details WHERE pay_document_details.emp_code = pay_dispatch_billing.emp_code AND remaining_no_set != 0 group by pay_dispatch_billing.emp_code) ,0) AS 'remaining_no_set',pay_dispatch_billing.id, client_name,  state_dispatch,  ifnull(pay_unit_master.unit_name,pay_dispatch_billing.unit_code) as unit_code, ifnull(pay_employee_master.emp_name,pay_dispatch_billing.emp_code) as emp_code, IFNULL(pay_unit_master.unit_code, pay_dispatch_billing.unit_code) AS unit_code1, IFNULL(pay_employee_master.emp_code, pay_dispatch_billing.emp_code) AS emp_code1, pay_dispatch_billing.designation, CASE dispatch_by   WHEN '0' THEN 'By Curier'  WHEN '1' THEN 'By Postal' WHEN '2' THEN 'By Hand' ELSE ''  END AS 'dispatch_by',  CASE receiver_type WHEN '1' THEN 'Material' ELSE '' END AS 'receiver_type',pay_dispatch_billing.dispatch_date AS 'dispatch_date' , pay_dispatch_billing.receiving_date,receiver_name_invoice, pod_no,pay_dispatch_billing.shipping_address,stamp_copy,sec_pod_no,sec_dispatch_date,remaining_uniform,sec_receiver_name,sec_receiving_date,second_stamp_copy,pay_dispatch_billing.emp_code FROM pay_dispatch_billing  inner JOIN pay_document_details ON pay_dispatch_billing.comp_code = pay_document_details.comp_code  AND pay_dispatch_billing.emp_code = pay_document_details.emp_code INNER JOIN pay_client_master ON pay_dispatch_billing.comp_code = pay_client_master.comp_code AND pay_dispatch_billing.client_code = pay_client_master.client_code    left outer join pay_unit_master on pay_dispatch_billing.comp_code = pay_unit_master.comp_code AND pay_dispatch_billing.unit_code = pay_unit_master.unit_code  left outer join pay_employee_master on pay_dispatch_billing.emp_code = pay_employee_master.emp_code WHERE  pay_dispatch_billing.comp_code = '" + Session["comp_code"].ToString() + "' and pay_dispatch_billing.CLIENT_CODE = '" + ddl_client_name.SelectedValue + "' AND receiver_type = '" + ddl_Receiver_type.SelectedValue + "' and ( year_complete = 0 and dis_year_complete = 0) group by pay_dispatch_billing.emp_code  ", d.con);


            d4.con.Open();
            DataTable dt_uniform = new DataTable();
            uniform.Fill(dt_uniform);
            if (dt_uniform.Rows.Count > 0)
            {

                gv_material.DataSource = dt_uniform;
                gv_material.DataBind();
                Panel6.Visible = true;

            }
        }
        catch (Exception ex) { throw ex; }
        finally { d4.con.Close(); }

    }

    protected void gridview_dublicate_id()
    {
        try
        {
            MySqlDataAdapter id_dublicate = null;

            // string current_date = d.getsinglestring("select field2,field3,field4 from pay_id_card_resend where");


            // id_dublicate = new MySqlDataAdapter("select client_code,unit_code ,emp_code from pay_id_card_resend where comp_code = '"+Session["comp_code"].ToString()+"'  ", d4.con);
            id_dublicate = new MySqlDataAdapter("SELECT  pay_id_card_resend.Id,pay_client_master.client_name, ifnull(pay_unit_master.unit_name,pay_id_card_resend.unit_code) as unit_code, ifnull(pay_employee_master.emp_name,pay_id_card_resend.emp_code) as emp_code,id_no_set,pay_id_card_resend.state_name, pay_dispatch_billing.designation,dispatch_date_du,receiving_date_du,du_receiving_name,du_pod1,shipping_address,du_stamp_copy,CASE du_dispatch_by  WHEN '0' THEN 'By Curier'  WHEN '1' THEN 'By Postal'  WHEN '2' THEN 'By Hand'  ELSE '' END AS 'du_dispatch_by',id_no_set,dispatch_date_du2,receiving_date_du2,du_receiving_name2,du_pod_no2,du_stamp_copy2,CASE du_dispatch_by2  WHEN '0' THEN 'By Curier'  WHEN '1' THEN 'By Postal'  WHEN '2' THEN 'By Hand'  ELSE '' END AS 'du_dispatch_by2',dispatch_date_du3,receiving_date_du3,du_receiving_name3,du_pod_no3,du_stamp_copy3,CASE du_dispatch_by3  WHEN '0' THEN 'By Curier'  WHEN '1' THEN 'By Postal'  WHEN '2' THEN 'By Hand'  ELSE '' END AS 'du_dispatch_by3' FROM pay_id_card_resend  INNER JOIN pay_dispatch_billing ON pay_id_card_resend.comp_code = pay_dispatch_billing.comp_code AND pay_id_card_resend.client_code = pay_dispatch_billing.client_code AND pay_id_card_resend.emp_code = pay_dispatch_billing.emp_code  INNER JOIN pay_client_master ON pay_id_card_resend.comp_code = pay_client_master.comp_code AND pay_id_card_resend.client_code = pay_client_master.client_code  INNER JOIN pay_unit_master ON pay_id_card_resend.comp_code = pay_unit_master.comp_code AND pay_id_card_resend.unit_code = pay_unit_master.unit_code  INNER JOIN pay_employee_master ON pay_id_card_resend.emp_code = pay_employee_master.emp_code WHERE pay_id_card_resend.comp_code = '" + Session["comp_code"].ToString() + "' group by pay_id_card_resend.emp_code ", d4.con);


            d4.con.Open();
            DataTable dt_id = new DataTable();
            id_dublicate.Fill(dt_id);
            if (dt_id.Rows.Count > 0)
            {

                gv_dublicate_id_card.DataSource = dt_id;
                gv_dublicate_id_card.DataBind();


            }
        }
        catch (Exception ex) { throw ex; }
        finally { d4.con.Close(); }

    }


    protected void gridview_invoice()
    {

        try
        {
            string region = "", region_name = "";
            if (ddl_client_name.SelectedValue == "DHFL" || (ddl_client_name.SelectedValue == "BAGIC" && ddl_state_name.SelectedValue == "Maharashtra" && int.Parse("" + txt_month_year.Text.Substring(3) + "" + txt_month_year.Text.Substring(0, 2) + "") > 20204))
            {
                region = "AND Zone = '" + ddlregion.SelectedValue + "'";
                region_name = "AND region_name = '" + ddlregion.SelectedValue + "'";
            }

            d6.con.Open();
            MySqlDataAdapter cd_invoice = new MySqlDataAdapter("SELECT  pay_dispatch_billing.id, client_name, state_dispatch, IFNULL(pay_unit_master.unit_name, pay_dispatch_billing.unit_code) AS 'unit_code', invoice_no, invoice_date, grand_total, CASE dispatch_by WHEN '0' THEN 'By Curier' WHEN '1' THEN 'By Postal' WHEN '2' THEN 'By Hand' ELSE '' END AS dispatch_by, CASE receiver_type WHEN '2' THEN 'Invoice' ELSE '' END AS receiver_type, dispatch_date, receiving_date, receiver_name_invoice, pod_no, shipping_address, stamp_copy, invoice_stamp_copy, month, CASE invoice_type WHEN '1' THEN 'Manpower' WHEN '2' THEN 'Material' WHEN '3' THEN 'Conveyance' WHEN '4' THEN 'DeepClean' ELSE '' END AS invoice_type FROM pay_dispatch_billing INNER JOIN pay_client_master ON pay_dispatch_billing.comp_code = pay_client_master.comp_code AND pay_dispatch_billing.client_code = pay_client_master.client_code LEFT OUTER JOIN pay_unit_master ON pay_dispatch_billing.comp_code = pay_unit_master.comp_code AND pay_dispatch_billing.client_code = pay_unit_master.client_code AND pay_dispatch_billing.region_name = pay_unit_master.ZONE AND pay_dispatch_billing.unit_code = pay_unit_master.unit_code    WHERE pay_dispatch_billing.comp_code = '" + Session["comp_code"].ToString() + "' and  pay_dispatch_billing.client_code = '" + ddl_client_name.SelectedValue + "' and invoice_no is not null ", d6.con);
            DataTable dt_invoice = new DataTable();
            cd_invoice.Fill(dt_invoice);
            if (dt_invoice.Rows.Count > 0)
            {

                gv_invoice.DataSource = dt_invoice;
                gv_invoice.DataBind();

            }

        }
        catch (Exception ex) { throw ex; }
        finally { d6.con.Close(); }



    }

    protected void Print_Report_Click(object sender, EventArgs e)
    {

        try
        {
            ReportDocument crystalReport = new ReportDocument();

            MySqlDataAdapter cmd_item = null;
            System.Data.DataTable dt = new System.Data.DataTable();

            string state_add = null;
            string region = "", region_name = "";
            if (ddl_client_name.SelectedValue == "DHFL" || (ddl_client_name.SelectedValue == "BAGIC" && ddl_state_name.SelectedValue == "Maharashtra" && int.Parse("" + txt_month_year.Text.Substring(3) + "" + txt_month_year.Text.Substring(0, 2) + "") > 20204))
            {
                // exten_region = "" + ddlregion.SelectedValue + "_";
                region = "AND ZONE ='" + ddlregion.SelectedValue + "'";
                //  region_name = "AND region_name = '" + ddlregion.SelectedValue + "'";
            }

            if (ddl_state_name.SelectedValue == "ALL")
            {
                if (ddl_inv_details.SelectedValue == "1")
                {
                    state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_unit_rate_history  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'  " + region);
                }
                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {

                    state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_material_history  where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'  and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'  ");
                }
                else if (ddl_inv_details.SelectedValue == "5")
                {

                    state_add = d.get_group_concat("select group_concat(distinct(pay_billing_material_history.STATE_NAME)) from pay_billing_material_history  INNER JOIN pay_billing_unit_rate_history ON pay_billing_material_history.comp_code = pay_billing_unit_rate_history.comp_code AND pay_billing_material_history.client_code = pay_billing_unit_rate_history.client_code AND pay_billing_material_history.unit_code = pay_billing_unit_rate_history.unit_code  where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'   and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'  ");
                }


                state_add = "'" + state_add + "'";
                state_add = state_add.Replace(",", "','");

            }
            else if (ddl_state_name.SelectedValue != "ALL")
            {

                Session["STATE_CODE"] = ddl_state_name.SelectedValue;
                state_add = "'" + Session["STATE_CODE"].ToString() + "'";
                // state_add = state_add.Replace(",", "','");

            }

            string all_statewise = null;
            if (ddl_inv_details.SelectedValue == "5")
            {
                all_statewise = d.getsinglestring("SELECT group_concat(billing_id) FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "' AND state = '" + ddl_state_name.SelectedValue + "'");

                all_statewise = all_statewise.Replace(",", "','");
            }
            else
            {

                all_statewise = "" + ddl_inv_details.SelectedValue + "";
            }

            string ship_billing = d.getsinglestring("SELECT group_concat(DISTINCT(billing_name)) FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' and billing_id in('" + all_statewise + "') AND client_code = '" + ddl_client_name.SelectedValue + "' and state IN(" + state_add + ")");
            ship_billing = "'" + ship_billing + "'";
            ship_billing = ship_billing.Replace(",", "','");



            string ship_wise = d.getsinglestring("SELECT group_concat(DISTINCT(billing_wise)) FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' and billing_id in('" + all_statewise + "') AND client_code = '" + ddl_client_name.SelectedValue + "'and state IN(" + state_add + ")");
            ship_wise = "'" + ship_wise + "'";
            ship_wise = ship_wise.Replace(",", "','");

            string shipping_add = null;
            if (ddl_inv_details.SelectedValue == "5")
            {

                shipping_add = d.getsinglestring("SELECT  GROUP_CONCAT(distinct(invoice_shipping_address)) FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' and billing_id in ('1','2','3','4') AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_client_billing_details.billing_name in(" + ship_billing + ")  AND billing_wise in(" + ship_wise + ") and state IN(" + state_add + ")");
            }
            else
            {

                shipping_add = d.getsinglestring("SELECT  GROUP_CONCAT(distinct(invoice_shipping_address)) FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' and billing_id = '" + ddl_inv_details.SelectedValue + "' AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_client_billing_details.billing_name in(" + ship_billing + ")  AND billing_wise in(" + ship_wise + ") and state IN(" + state_add + ")");
            }
            string[] invoice_ship_add = shipping_add.Split(',');

            string auto_invoice_no1 = null;
            foreach (object obj in invoice_ship_add)
            {
                if (ddl_inv_details.SelectedValue == "1")
                {

                    auto_invoice_no1 = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_unit_rate_history INNER JOIN pay_client_billing_details ON pay_billing_unit_rate_history.comp_code = pay_client_billing_details.comp_code AND pay_billing_unit_rate_history.client_code = pay_client_billing_details.client_code AND pay_billing_unit_rate_history.state_name = pay_client_billing_details.state WHERE  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' and state_name in(" + state_add + ")  AND invoice_shipping_address = '" + obj + "'and billing_id = '" + ddl_inv_details.SelectedValue + "' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");

                }

                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {

                    auto_invoice_no1 = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_material_history INNER JOIN pay_client_billing_details ON pay_billing_material_history.comp_code = pay_client_billing_details.comp_code AND pay_billing_material_history.client_code = pay_client_billing_details.client_code AND pay_billing_material_history.state_name = pay_client_billing_details.state WHERE  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'  and type = '" + ddl_inv_details.SelectedItem + "'  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' and state_name in(" + state_add + ")  AND invoice_shipping_address = '" + obj + "'and pay_client_billing_details.billing_id = '" + ddl_inv_details.SelectedValue + "' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");

                }

                else if (ddl_inv_details.SelectedValue == "5")
                {
                    //string auto_invoice_noo = d.getsinglestring("select group_concat(invoice_no) from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' AND pay_dispatch_billing.month = '" + txt_month_year.Text + "' and client_code = '" + ddl_client_name.SelectedValue + "'");
                    //auto_invoice_noo = "'" + auto_invoice_noo + "'";
                    //auto_invoice_noo = auto_invoice_noo.Replace(",", "','");

                    //// for manpower komal 21-05-2020   
                    //string unit_code_manpower = d.get_group_concat("select group_concat(distinct(unit_code)) from pay_billing_unit_rate_history  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_unit_rate_history.STATE_NAME IN( " + state_add + ") and invoice_flag = '2' ");
                    //unit_code_manpower = unit_code_manpower.Replace(",", "','");

                    //  string manpower = d.getsinglestring("SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_unit_rate_history.unit_code IN ('" + unit_code_manpower + "') and pay_billing_unit_rate_history.STATE_NAME IN (" + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' and auto_invoice_no not in (" + auto_invoice_noo+ ") AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");
                    string manpower = d.getsinglestring(" SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_unit_rate_history INNER JOIN pay_client_billing_details ON pay_billing_unit_rate_history.comp_code = pay_client_billing_details.comp_code AND pay_billing_unit_rate_history.client_code = pay_client_billing_details.client_code AND pay_billing_unit_rate_history.state_name = pay_client_billing_details.state WHERE  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' and state_name in(" + state_add + ")  AND invoice_shipping_address = '" + obj + "'and billing_id = '1' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");

                    //// for man_con_dip komal 21-05-2020

                    //string unit_code_man_con_dip = d.get_group_concat("select group_concat(distinct(unit_code)) from pay_billing_material_history  where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_material_history.STATE_NAME IN( " + state_add + ") and invoice_flag = '2' ");
                    //unit_code_man_con_dip = unit_code_man_con_dip.Replace(",", "','");

                    string man_con_dip = d.getsinglestring(" SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_material_history INNER JOIN pay_client_billing_details ON pay_billing_material_history.comp_code = pay_client_billing_details.comp_code AND pay_billing_material_history.client_code = pay_client_billing_details.client_code AND pay_billing_material_history.state_name = pay_client_billing_details.state WHERE  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' and state_name in(" + state_add + ")  AND invoice_shipping_address = '" + obj + "' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");

                    string all_billing = "" + manpower + "," + man_con_dip + "";

                    auto_invoice_no1 = "" + all_billing + "";

                    //all_billing = all_billing.Replace("", " ");



                }

                if (ddl_inv_details.SelectedValue == "1")
                {
                    cmd_item = new MySqlDataAdapter("SELECT distinct client_name AS 'client_code',  (SELECT GROUP_CONCAT(state_name) FROM (SELECT state_name FROM pay_billing_unit_rate_history INNER JOIN pay_client_billing_details ON pay_billing_unit_rate_history.comp_code = pay_client_billing_details.comp_code AND pay_billing_unit_rate_history.client_code = pay_client_billing_details.client_code AND pay_billing_unit_rate_history.state_name = pay_client_billing_details.state WHERE  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2'  AND invoice_shipping_address = '" + obj + "' and state_name in(" + state_add + ") AND auto_invoice_no IS NOT NULL group by state_name ) as a) AS 'STATE',  pay_client_billing_details.invoice_shipping_address AS 'UNIT_ADD1',case dispatch_by  WHEN '0' THEN 'By Curier' WHEN '1' THEN 'By Postal' WHEN '2' THEN 'By Hand'  ELSE '' END AS 'ADDRESS1', receiver_name_invoice AS 'ADDRESS2',receiving_date AS 'UNIT_ADD2', (SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_unit_rate_history INNER JOIN pay_client_billing_details ON pay_billing_unit_rate_history.comp_code = pay_client_billing_details.comp_code AND pay_billing_unit_rate_history.client_code = pay_client_billing_details.client_code AND pay_billing_unit_rate_history.state_name = pay_client_billing_details.state WHERE  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2'  AND invoice_shipping_address = '" + obj + "' and state_name in(" + state_add + ") AND auto_invoice_no IS NOT NULL  and pay_client_billing_details.billing_name in(" + ship_billing + ") and pay_client_billing_details.billing_wise in (" + ship_wise + ") group by auto_invoice_no ) as t) AS 'COMP_CODE' FROM  pay_dispatch_billing  LEFT OUTER JOIN pay_unit_master ON pay_dispatch_billing.unit_code = pay_unit_master.unit_code AND pay_dispatch_billing.comp_code = pay_unit_master.comp_code INNER JOIN pay_client_master ON pay_dispatch_billing.comp_code = pay_client_master.comp_code AND pay_dispatch_billing.client_code = pay_client_master.client_code  INNER JOIN pay_client_billing_details ON pay_dispatch_billing.comp_code = pay_client_billing_details.comp_code AND pay_dispatch_billing.client_code = pay_client_billing_details.client_code  INNER JOIN pay_billing_unit_rate_history ON pay_dispatch_billing.comp_code = pay_billing_unit_rate_history.comp_code AND pay_dispatch_billing.client_code = pay_billing_unit_rate_history.client_code WHERE   pay_client_master.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_client_master.client_code = '" + ddl_client_name.SelectedValue + "'  AND state_dispatch = '" + ddl_state_name.SelectedValue + "' and pay_client_billing_details.billing_name in(" + ship_billing + ") and pay_client_billing_details.billing_wise in (" + ship_wise + ") and invoice_shipping_address = '" + obj + "' AND receiver_name_invoice = '" + txt_receiver_name.Text + "'  AND pay_dispatch_billing.month = '" + txt_month_year.Text + "'", d.con);//AND client_code in(select distinct(client_code) from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE='" + Session["LOGIN_ID"].ToString() + "')

                }
                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {

                    cmd_item = new MySqlDataAdapter("SELECT distinct client_name AS 'client_code',  (SELECT GROUP_CONCAT(state_name) FROM (SELECT state_name FROM pay_billing_material_history INNER JOIN pay_client_billing_details ON pay_billing_material_history.comp_code = pay_client_billing_details.comp_code AND pay_billing_material_history.client_code = pay_client_billing_details.client_code AND pay_billing_material_history.state_name = pay_client_billing_details.state WHERE  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2'  AND invoice_shipping_address = '" + obj + "' and state_name in(" + state_add + ") AND auto_invoice_no IS NOT NULL group by state_name ) as a) AS 'STATE',  pay_client_billing_details.invoice_shipping_address AS 'UNIT_ADD1',case dispatch_by  WHEN '0' THEN 'By Curier' WHEN '1' THEN 'By Postal' WHEN '2' THEN 'By Hand'  ELSE '' END AS 'ADDRESS1', receiver_name_invoice AS 'ADDRESS2',receiving_date AS 'UNIT_ADD2', (SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_material_history INNER JOIN pay_client_billing_details ON pay_billing_material_history.comp_code = pay_client_billing_details.comp_code AND pay_billing_material_history.client_code = pay_client_billing_details.client_code AND pay_billing_material_history.state_name = pay_client_billing_details.state WHERE  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2'  AND invoice_shipping_address = '" + obj + "' and state_name in(" + state_add + ") and type = '" + ddl_inv_details.SelectedItem + "' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t) AS 'COMP_CODE' FROM  pay_dispatch_billing  LEFT OUTER JOIN pay_unit_master ON pay_dispatch_billing.unit_code = pay_unit_master.unit_code AND pay_dispatch_billing.comp_code = pay_unit_master.comp_code INNER JOIN pay_client_master ON pay_dispatch_billing.comp_code = pay_client_master.comp_code AND pay_dispatch_billing.client_code = pay_client_master.client_code  INNER JOIN pay_client_billing_details ON pay_dispatch_billing.comp_code = pay_client_billing_details.comp_code AND pay_dispatch_billing.client_code = pay_client_billing_details.client_code  INNER JOIN pay_billing_material_history ON pay_dispatch_billing.comp_code = pay_billing_material_history.comp_code AND pay_dispatch_billing.client_code = pay_billing_material_history.client_code WHERE   pay_client_master.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_client_master.client_code = '" + ddl_client_name.SelectedValue + "'  AND state_dispatch = '" + ddl_state_name.SelectedValue + "' and pay_client_billing_details.billing_name in(" + ship_billing + ") and pay_client_billing_details.billing_wise in (" + ship_wise + ") and invoice_shipping_address = '" + obj + "' AND receiver_name_invoice = '" + txt_receiver_name.Text + "'  AND pay_dispatch_billing.month = '" + txt_month_year.Text + "'", d.con);//AND client_code in(select distinct(client_code) from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE='" + Session["LOGIN_ID"].ToString() + "')

                }
                else if (ddl_inv_details.SelectedValue == "5")
                {

                    cmd_item = new MySqlDataAdapter("SELECT distinct client_name AS 'client_code',  (SELECT GROUP_CONCAT(state_name) FROM (SELECT state_name FROM pay_billing_material_history INNER JOIN pay_client_billing_details ON pay_billing_material_history.comp_code = pay_client_billing_details.comp_code AND pay_billing_material_history.client_code = pay_client_billing_details.client_code AND pay_billing_material_history.state_name = pay_client_billing_details.state WHERE  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'   and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2'  AND invoice_shipping_address = '" + obj + "' and state_name in(" + state_add + ") AND auto_invoice_no IS NOT NULL group by state_name ) as a) AS 'STATE',  pay_client_billing_details.invoice_shipping_address AS 'UNIT_ADD1',case dispatch_by  WHEN '0' THEN 'By Curier' WHEN '1' THEN 'By Postal' WHEN '2' THEN 'By Hand'  ELSE '' END AS 'ADDRESS1', receiver_name_invoice AS 'ADDRESS2',receiving_date AS 'UNIT_ADD2', ('" + auto_invoice_no1 + "') AS 'COMP_CODE' FROM  pay_dispatch_billing  LEFT OUTER JOIN pay_unit_master ON pay_dispatch_billing.unit_code = pay_unit_master.unit_code AND pay_dispatch_billing.comp_code = pay_unit_master.comp_code INNER JOIN pay_client_master ON pay_dispatch_billing.comp_code = pay_client_master.comp_code AND pay_dispatch_billing.client_code = pay_client_master.client_code  INNER JOIN pay_client_billing_details ON pay_dispatch_billing.comp_code = pay_client_billing_details.comp_code AND pay_dispatch_billing.client_code = pay_client_billing_details.client_code  INNER JOIN pay_billing_material_history ON pay_dispatch_billing.comp_code = pay_billing_material_history.comp_code AND pay_dispatch_billing.client_code = pay_billing_material_history.client_code WHERE   pay_client_master.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_client_master.client_code = '" + ddl_client_name.SelectedValue + "'  AND state_dispatch = '" + ddl_state_name.SelectedValue + "' and pay_client_billing_details.billing_name in(" + ship_billing + ") and pay_client_billing_details.billing_wise in (" + ship_wise + ") and invoice_shipping_address = '" + obj + "' AND receiver_name_invoice = '" + txt_receiver_name.Text + "'  AND pay_dispatch_billing.month = '" + txt_month_year.Text + "'", d.con);//AND client_code in(select distinct(client_code) from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE='" + Session["LOGIN_ID"].ToString() + "')

                }

                d.con.Open();
                try
                {
                    cmd_item.Fill(dt);
                }

                catch (Exception ex) { throw ex; }
            }
            crystalReport.Load(Server.MapPath("~/invoice_copy.rpt"));
            crystalReport.SetDataSource(dt);
            crystalReport.Refresh();
            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, "invoice_copy");

            //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, "TaxInvoice");
            //uniform_gridview();
            //gridview_invoice();
            dispatch_resiver();
        }

        catch (Exception ee)
        {
            throw ee;
        }
        finally
        {
            d.con.Close();

        }
    }


    protected void ddl_branch_name_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        try
        {
            d.con.Open();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            MySqlDataAdapter cmd_emp = null;
            if (ddl_branch_name.SelectedValue == "ALL")
            {

                cmd_emp = new MySqlDataAdapter("select emp_name,emp_code from pay_employee_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_code ='" + ddl_client_name.SelectedValue + "' and client_wise_state = '" + ddl_state_name.SelectedValue + "'  ", d.con);//and legal_flag = '2' 
            }
            else
            {
                cmd_emp = new MySqlDataAdapter("SELECT emp_name,emp_code FROM  pay_employee_master WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "' AND unit_code = '" + ddl_branch_name.SelectedValue + "'  and client_wise_state = '" + ddl_state_name.SelectedValue + "'   ", d.con);//and legal_flag = '2'
                //   cmd_emp = new MySqlDataAdapter("select emp_name,emp_code from pay_employee_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_code ='" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and client_wise_state = '" + ddl_state_name.SelectedValue + "' and legal_flag = '2'  AND (left_date = '' OR left_date IS NULL) ", d.con);

            }
            DataTable dt_emp = new DataTable();
            cmd_emp.Fill(dt_emp);
            if (dt_emp.Rows.Count > 0)
            {

                ddl_emp_name.DataSource = dt_emp;
                ddl_emp_name.DataTextField = dt_emp.Columns[0].ToString();
                ddl_emp_name.DataValueField = dt_emp.Columns[1].ToString();
                ddl_emp_name.DataBind();
                ddl_emp_name.Items.Insert(0, "Select");
                ddl_emp_name.Items.Insert(1, "ALL");
            }

            if (txt_month_year.Text != "" && ddl_Receiver_type.SelectedValue != "1")
            {

                //   billing_branchwise();

            }

            if (ddl_Receiver_type.SelectedValue == "1")
            {

                //string shipping_material = d.getsinglestring("select distinct UNIT_ADD2 from pay_unit_master where comp_code = '"+Session[].ToString()+"' and client_code = '"+ddl_client_name.SelectedValue+"' and STATE_NAME = '"+ddl_state_name.SelectedValue+"' and unit_code = '"+ddl_branch_name.SelectedValue+"'");


                MySqlCommand shipping_material = new MySqlCommand("select distinct UNIT_ADD2 from pay_unit_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and STATE_NAME = '" + ddl_state_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "'", d.con);
                MySqlDataReader shipping = shipping_material.ExecuteReader();

                if (shipping.Read())
                {
                    txt_bill_reason.Text = shipping.GetValue(0).ToString();
                }
                shipping.Close();

            }

            // 28-04-2020 komal changes
            // dispatch_resiver();
            //end
        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }


    }

    protected void ddl_emp_name_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        try
        {

            d7.con.Open();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            MySqlDataAdapter cmd_desg = null;


            string unit_code = d.get_group_concat("select  group_concat(distinct(emp_code)) from pay_employee_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and client_wise_state = '" + ddl_state_name.SelectedValue + "' ");

            string unit_code1 = d.get_group_concat("select  group_concat(distinct(emp_code)) from pay_employee_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and client_wise_state = '" + ddl_state_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' ");

            unit_code = unit_code.Replace(",", "','");
            unit_code1 = unit_code.Replace(",", "','");

            if (ddl_emp_name.SelectedValue == "ALL" && ddl_branch_name.SelectedValue == "ALL")
            {
                cmd_desg = new MySqlDataAdapter(" SELECT  GRADE_DESC FROM pay_employee_master INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND pay_employee_master.grade_code = pay_grade_master.grade_code WHERE pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_employee_master.client_code = '" + ddl_client_name.SelectedValue + "' AND client_wise_state = '" + ddl_state_name.SelectedValue + "' AND pay_employee_master.emp_code IN ('" + unit_code + "')  group by GRADE_DESC  ", d.con);

            }
            else if (ddl_emp_name.SelectedValue == "ALL" && ddl_branch_name.SelectedValue != "ALL")
            {

                cmd_desg = new MySqlDataAdapter(" SELECT  GRADE_DESC FROM pay_employee_master INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND pay_employee_master.grade_code = pay_grade_master.grade_code WHERE pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_employee_master.client_code = '" + ddl_client_name.SelectedValue + "' AND client_wise_state = '" + ddl_state_name.SelectedValue + "' AND pay_employee_master.emp_code IN ('" + unit_code1 + "')    group by GRADE_DESC  ", d.con);

            }
            else
            {

                cmd_desg = new MySqlDataAdapter(" SELECT  GRADE_DESC FROM pay_employee_master INNER JOIN pay_grade_master ON pay_employee_master.comp_code = pay_grade_master.comp_code AND pay_employee_master.grade_code = pay_grade_master.grade_code WHERE pay_employee_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_employee_master.client_code = '" + ddl_client_name.SelectedValue + "' AND client_wise_state = '" + ddl_state_name.SelectedValue + "' AND pay_employee_master.emp_code = '" + ddl_emp_name.SelectedValue + "'and unit_code = '" + ddl_branch_name.SelectedValue + "' ", d.con);

            }
            DataTable dt_desig = new DataTable();
            cmd_desg.Fill(dt_desig);
            if (dt_desig.Rows.Count > 0)
            {

                ddl_designation.DataSource = dt_desig;
                //ddl_designation.DataTextField = dt_desig.Columns[0].ToString();
                ddl_designation.DataValueField = dt_desig.Columns[0].ToString();
                ddl_designation.DataBind();

            }
            desigpanel.Visible = true;
            //gridview_invoice();
            //uniform_gridview();

            // 28-04-2020 komal changes
            // dispatch_resiver();
            //end


        }

        catch (Exception ex) { throw ex; }

        finally { d7.con.Close(); }




    }

    protected void btn_uniform_shoes_Click(object sender, EventArgs e)
    {
        try
        {

            ReportDocument crystalReport = new ReportDocument();
            System.Data.DataTable dt1 = new System.Data.DataTable();


            MySqlDataAdapter cmd_uniform_shoes = new MySqlDataAdapter(" SELECT client_name AS 'COMP_CODE', IFNULL(pay_unit_master.unit_name, pay_dispatch_billing.unit_code) AS 'CITY', state_dispatch AS 'STATE', IFNULL(pay_employee_master.emp_name, pay_dispatch_billing.emp_code) AS 'ADDRESS1', receiver_name_invoice AS 'ADDRESS2', pay_dispatch_billing.designation, (SELECT size FROM pay_document_details WHERE pay_document_details.emp_code = pay_employee_master.emp_code AND comp_code = '" + Session["comp_code"].ToString() + "' AND document_type = 'Uniform' AND year_complete = 0) AS 'UNIT_ADD1', (SELECT size FROM pay_document_details WHERE pay_document_details.emp_code = pay_employee_master.emp_code AND comp_code = pay_unit_master.comp_code AND document_type = 'Shoes' AND year_complete = 0) AS 'UNIT_ADD2', IF(((SELECT ID FROM pay_document_details WHERE pay_document_details.emp_code = pay_employee_master.emp_code AND comp_code = '" + Session["comp_code"].ToString() + "' AND document_type = 'ID_Card' AND year_complete = 0) != ''), 'YES', 'NO') AS 'UNIT_CITY' FROM pay_dispatch_billing INNER JOIN pay_document_details ON pay_dispatch_billing.comp_code = pay_document_details.comp_code AND pay_dispatch_billing.client_code = pay_document_details.client_code AND pay_dispatch_billing.emp_code = pay_document_details.emp_code AND pay_dispatch_billing.unit_code = pay_document_details.unit_code INNER JOIN pay_client_master ON pay_dispatch_billing.comp_code = pay_client_master.comp_code AND pay_dispatch_billing.client_code = pay_client_master.client_code LEFT OUTER JOIN pay_unit_master ON pay_dispatch_billing.unit_code = pay_unit_master.unit_code AND pay_dispatch_billing.comp_code = pay_unit_master.comp_code LEFT OUTER JOIN pay_employee_master ON pay_dispatch_billing.emp_code = pay_employee_master.emp_code AND pay_dispatch_billing.comp_code = pay_employee_master.comp_code  WHERE  pay_client_master.client_code = '" + ddl_client_name.SelectedValue + "' AND pay_unit_master.unit_code = '" + ddl_branch_name.SelectedValue + "'  AND pay_dispatch_billing.emp_code = pay_employee_master.emp_code AND pay_unit_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_dispatch_billing.emp_code = '" + ddl_emp_name.SelectedValue + "' AND year_complete = 0 AND dis_year_complete = 0 GROUP BY receiver_name_invoice ", d.con);
            d.con.Open();
            try
            {
                cmd_uniform_shoes.Fill(dt1);

                //MySqlDataReader sda = null;



                // sda = cmd1.ExecuteReader();
                //dt.Load(sda);
                //sda.Close();

            }
            catch (Exception ex) { throw ex; }
            crystalReport.Load(Server.MapPath("~/receiving_copy.rpt"));
            crystalReport.SetDataSource(dt1);
            crystalReport.Refresh();
            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, "receiving_copy");
            //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, "TaxInvoice");


        }

        catch (Exception ee)
        {
            throw ee;
        }
        finally
        {
            dispatch_resiver();

        }

    }
    protected void gv_material_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        string emp_Code = "", unit_code = "";

        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                try
                {
                    unit_code = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "unit_code1"));

                }
                catch { }
                try
                {
                    emp_Code = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "emp_code1"));
                }
                catch { }
            }
            if (d.getsinglestring("select branch_status from pay_unit_master where unit_code='" + unit_code + "'") == "1")
            {
                LinkButton edit = e.Row.FindControl("link_uniform") as LinkButton;
                edit.Visible = false;
            }
            else
            {
                LinkButton edit = e.Row.FindControl("link_uniform") as LinkButton;
                edit.Visible = true;
            }
            string left_date = d.getsinglestring("select ifnull(LEFT_DATE,0) as left_date from pay_employee_master where emp_code='" + emp_Code + "'");
            if (left_date != "0")
            {
                LinkButton edit = e.Row.FindControl("link_uniform") as LinkButton;
                edit.Visible = false;
            }
            else
            {
                LinkButton edit = e.Row.FindControl("link_uniform") as LinkButton;
                edit.Visible = true;
            }
        }
        catch { }


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
            if (dr["stamp_copy"].ToString() == "")
            {
                //LinkButton lb1 = e.Row.FindControl("unit_name") as LinkButton;
                //lb1.Visible = false;
                e.Row.Cells[14].Text = "Dispatched and POD Remaining";

            }

            // for second dispatched
            if (dr["second_stamp_copy"].ToString() == "")
            {

                e.Row.Cells[22].Text = "Second Dispatched and POD Remaining";
            }
        }

        e.Row.Cells[1].Visible = false;
        e.Row.Cells[15].Visible = false;
        e.Row.Cells[16].Visible = false;
        e.Row.Cells[23].Visible = false;




    }


    protected void btn_update_Click(object sender, EventArgs e)
    {
        if (ddl_Receiver_type.SelectedValue == "1")
        {
            // if (txt_report_receiving.HasFile)
            //{
            update_stamp_copy();


            //}
        }
        if (ddl_Receiver_type.SelectedValue == "2")
        {

            invoice_stamp_copy();
        }

        if (ddl_Receiver_type.SelectedValue == "3")
        {

            update_dublicate_id();


            //GridViewRow row = (GridViewRow)(((LinkButton)sender)).NamingContainer;
            //string emp_id = row.Cells[1].Text;
            string dublicate_id_no = "" + txt_id_set.Text + "";
            int remaining_du_id_set = Convert.ToInt16(dublicate_id_no) - 1;



            if (dublicate_id_no != "0" && txt_bill_rtn_date.Text != "")
            {
                string disp_date_du1 = d.getsinglestring("select dispatch_date_du from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "'  and dublicate_id_card = 'YES' ");



                if (disp_date_du1 == txt_bill_rtn_date.Text)
                {

                    if (txt_bill_rtn_date.Text != "" && txt_pod_number.Text != "" && ddl_material_contract.SelectedValue == "0")
                    {

                        int res = d.operation("UPDATE pay_id_card_resend SET id_no_set='" + remaining_du_id_set + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");
                        if (res > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                        }

                    }

                    if (txt_bill_rtn_date.Text != "" && ddl_material_contract.SelectedValue == "1")
                    {
                        int res = d.operation("UPDATE pay_id_card_resend SET id_no_set='" + remaining_du_id_set + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");
                        if (res > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                        }


                    }

                    if (txt_bill_rtn_date.Text != "" && ddl_material_contract.SelectedValue == "2" && txt_receiver_name.Text != "" && txt_receiv_date.Text != "")
                    {
                        int res = d.operation("UPDATE pay_id_card_resend SET id_no_set='" + remaining_du_id_set + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");
                        if (res > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                        }

                    }

                }
                if (ddl_material_contract.SelectedValue == "2")
                {

                    Panel_receiver_name.Visible = false;
                    Panel_receiving_date.Visible = false;
                }

                dublicate_id_clear();

            }

            //////////// for second dublicate id card


            if (dublicate_id_no != "0" && txt_dub_dispatch_date.Text != "")
            {
                string disp_date_du2 = d.getsinglestring("select dispatch_date_du2 from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "'  and dublicate_id_card = 'YES' ");



                if (disp_date_du2 == txt_dub_dispatch_date.Text)
                {

                    if (txt_dub_dispatch_date.Text != "" && txt_sec_du_pod.Text != "" && ddl_material_contract.SelectedValue == "0")
                    {

                        int res = d.operation("UPDATE pay_id_card_resend SET id_no_set='" + remaining_du_id_set + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");
                        if (res > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                        }

                    }

                    if (txt_dub_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "1")
                    {
                        int res = d.operation("UPDATE pay_id_card_resend SET id_no_set='" + remaining_du_id_set + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");
                        if (res > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                        }


                    }

                    if (txt_dub_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "2" && txt_dub_receiver_name.Text != "" && txt_du_receiver_date.Text != "")
                    {
                        int res = d.operation("UPDATE pay_id_card_resend SET id_no_set='" + remaining_du_id_set + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");
                        if (res > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                        }

                    }

                }
                dublicate_id_clear();
                Panel_dis_sec.Visible = false;
                Panel_dispatch.Visible = true;
            }




            //////////////////////// third dublicate id card

            if (dublicate_id_no != "0" && txt_dub_dispatch_date3.Text != "")
            {
                string disp_date_du3 = d.getsinglestring("select dispatch_date_du3 from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "'  and dublicate_id_card = 'YES' ");



                if (disp_date_du3 == txt_dub_dispatch_date3.Text)
                {

                    if (txt_dub_dispatch_date3.Text != "" && txt_third_du_pod.Text != "" && ddl_material_contract.SelectedValue == "0")
                    {

                        int res = d.operation("UPDATE pay_id_card_resend SET id_no_set='" + remaining_du_id_set + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");
                        if (res > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                        }

                    }

                    if (txt_dub_dispatch_date3.Text != "" && ddl_material_contract.SelectedValue == "1")
                    {
                        int res = d.operation("UPDATE pay_id_card_resend SET id_no_set='" + remaining_du_id_set + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");
                        if (res > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                        }


                    }

                    if (txt_dub_dispatch_date3.Text != "" && ddl_material_contract.SelectedValue == "2" && txt_dub_receiver_name_third.Text != "" && txt_du_receiver_date_third.Text != "")
                    {
                        int res = d.operation("UPDATE pay_id_card_resend SET id_no_set='" + remaining_du_id_set + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");
                        if (res > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                        }

                    }

                }
                dublicate_id_clear();
                Panel_dis_third.Visible = false;
                Panel_dispatch.Visible = true;
            }


            gridview_dublicate_id();



            ///////////////////////////////////////
        }


        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }


    // for uniform shoes id issues date to dispatch date 27-03-2020 komal

    public bool check_dispatch_usi(string dispatch_date, string issue_date)
    {

        try
        {
            //DateTime dispatch_date = DateTime.Parse(dispatching_date);
            //DateTime resive_date = DateTime.Parse(resiving_date);
            DateTime dispatch_date1 = DateTime.ParseExact(dispatch_date, "dd/MM/yyyy", null);
            DateTime issue_date1 = DateTime.ParseExact(issue_date, "dd/MM/yyyy", null);

            if (issue_date1.Date > dispatch_date1.Date)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        { return false; }

    }




    // for material dispatch date

    public bool check_dispatch_date(string material_dis_date, string dispatch_billig)
    {

        try
        {
            //DateTime dispatch_date = DateTime.Parse(dispatching_date);
            //DateTime resive_date = DateTime.Parse(resiving_date);
            DateTime material_dispatch_date = DateTime.ParseExact(material_dis_date, "dd/MM/yyyy", null);
            DateTime billig_dispatch_date = DateTime.ParseExact(dispatch_billig, "dd/MM/yyyy", null);

            if (billig_dispatch_date.Date > material_dispatch_date.Date)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        { return false; }

    }




    // for dublicate dublicate dispatch date

    public bool check_date_dublicate(string second_dispatch_date, string first_dispatch_date)
    {

        try
        {
            //DateTime dispatch_date = DateTime.Parse(dispatching_date);
            //DateTime resive_date = DateTime.Parse(resiving_date);
            DateTime sec_dispatch_date = DateTime.ParseExact(second_dispatch_date, "dd/MM/yyyy", null);
            DateTime firs_dispatch_date = DateTime.ParseExact(first_dispatch_date, "dd/MM/yyyy", null);

            if (firs_dispatch_date.Date > sec_dispatch_date.Date)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        { return false; }

    }



    public bool check_date_issueing(string dispatch_date, string issue_date)
    {

        try
        {
            //DateTime dispatch_date = DateTime.Parse(dispatching_date);
            //DateTime resive_date = DateTime.Parse(resiving_date);
            DateTime dispatch_date1 = DateTime.ParseExact(dispatch_date, "dd/MM/yyyy", null);
            DateTime issue_date1 = DateTime.ParseExact(issue_date, "dd/MM/yyyy", null);

            if (issue_date1.Date > dispatch_date1.Date)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        { return false; }

    }

    public bool check_sec_date_dublicate(string receiving_date, string second_dispatch_date)
    {

        try
        {
            //DateTime dispatch_date = DateTime.Parse(dispatching_date);
            //DateTime resive_date = DateTime.Parse(resiving_date);
            DateTime receiving_date1 = DateTime.ParseExact(receiving_date, "dd/MM/yyyy", null);
            DateTime sec_dispatch_date = DateTime.ParseExact(second_dispatch_date, "dd/MM/yyyy", null);

            if (receiving_date1.Date > sec_dispatch_date.Date)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        { return false; }

    }

    protected void update_dublicate_id()
    {


        // for dispatch date
        //GridViewRow row = (GridViewRow)(((LinkButton)sender)).NamingContainer;
        //  string emp_id = row.Cells[1].Text;

        string issueing_date = d.getsinglestring("select from_date from pay_id_card_resend where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and state_name = '" + ddl_state_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' ");

        // for first du id dispatch date 06-05-2020
        if (txt_bill_rtn_date.Text != "")
        {

            if (check_date_issueing(txt_bill_rtn_date.Text, issueing_date))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Dublicate id dispatch date should be greater than issueing date !!!')", true);
                return;
            }

        }

        if (txt_dub_dispatch_date.Text != "")
        {

            if (check_date_issueing(txt_dub_dispatch_date.Text, issueing_date))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Dublicate id dispatch date should be greater than issueing date !!!')", true);
                return;
            }

        }

        if (txt_dub_dispatch_date3.Text != "")
        {

            if (check_date_issueing(txt_dub_dispatch_date3.Text, issueing_date))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Dublicate id dispatch date should be greater than issueing date !!!')", true);
                return;
            }

        }




        //if (ddl_material_contract.SelectedValue == "0" && txt_pod_number.Text == "" && txt_bill_rtn_date.Text != "")
        //{
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Please Enter Dublicate Id POD Number... !!!');", true);
        //    return;

        //}

        // for dublicate receiving date

        if (txt_receiv_date.Text != "" && ddl_material_contract.SelectedValue == "2" && txt_bill_rtn_date.Text != "")
        {

            if (check_date(txt_bill_rtn_date.Text, txt_receiv_date.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('First  Receiving date should be greater than dispatch date!!!')", true);
                return;
            }


        }

        //for dublicate second dispatch date with by curier

        if (txt_dub_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "0")
        {

            string disp_date_du1 = d.getsinglestring("select dispatch_date_du from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "'  and dublicate_id_card = 'YES' ");

            if (check_date_dublicate(txt_dub_dispatch_date.Text, disp_date_du1))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('First  Second Dispatch Date should be greater than First dispatch date!!!')", true);
                return;
            }


        }


        //for dublicate second dispatch date with by hand

        if (txt_receiv_date.Text != "" && txt_dub_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "2")
        {

            if (check_sec_date_dublicate(txt_dub_dispatch_date.Text, txt_receiv_date.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('  Second Dispatch Date should be greater than First dispatch date!!!')", true);
                return;
            }


        }

        // for dublicate third dispatch date  with by curier

        if (txt_dub_dispatch_date3.Text != "" && ddl_material_contract.SelectedValue == "0")
        {

            string disp_date_du2 = d.getsinglestring("select dispatch_date_du2 from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "'  and dublicate_id_card = 'YES' ");

            if (disp_date_du2 != "")
            {

                if (check_date_dublicate(txt_dub_dispatch_date3.Text, disp_date_du2))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Third Dispatch Date should be greater than Second dispatch date!!!')", true);
                    return;
                }

            }


        }

        //for dublicate third dispatch date with by hand


        if (txt_du_receiver_date_third.Text != "" && txt_dub_dispatch_date3.Text != "" && ddl_material_contract.SelectedValue == "2")
        {

            if (check_sec_date_dublicate(txt_dub_dispatch_date3.Text, txt_du_receiver_date_third.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('  Second Dispatch Date should be greater than First dispatch date!!!')", true);
                return;
            }


        }

        ////////////////////////////////
        if (txt_bill_rtn_date.Text != "")
        {


            if (txt_bill_rtn_date.Text != "" && txt_pod_number.Text == "")
            {

                int res = d.operation("UPDATE pay_dispatch_billing SET dispatch_date_du = '" + txt_bill_rtn_date.Text + "',dublicate_id_card ='YES', du_dispatch_by='" + ddl_material_contract.SelectedValue + "',dublicate_id='" + txt_id.Text + "',receiver_type_du='" + ddl_Receiver_type.SelectedValue + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "'  ");
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                }

            }

            if (txt_bill_rtn_date.Text != "" && txt_pod_number.Text != "" && ddl_material_contract.SelectedValue == "0")
            {

                int res = d.operation("UPDATE pay_dispatch_billing SET du_pod1='" + txt_pod_number.Text + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "' and dispatch_date_du='" + txt_bill_rtn_date.Text + "' and dublicate_id_card = 'YES'  ");
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                }

            }


            if (txt_bill_rtn_date.Text != "" && ddl_material_contract.SelectedValue == "2" && txt_receiver_name.Text != "" && txt_receiv_date.Text != "")
            {
                // update dublicate id  receiver name and date
                int res = d.operation("UPDATE pay_dispatch_billing SET du_receiving_name = '" + txt_receiver_name.Text + "',receiving_date_du = '" + txt_receiv_date.Text + "'  where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "' and dispatch_date_du='" + txt_bill_rtn_date.Text + "'  and dublicate_id_card = 'YES'  ");
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                }

            }

            // update dublicate id stamp copy 
            string fileExt = System.IO.Path.GetExtension(txt_report_receiving.FileName);

            string fname = null;

            string abc = txt_bill_rtn_date.Text.Replace("/", "_");

            if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".ZIP")
            {
                string dublicate_stamp_copy = d.getsinglestring("select du_stamp_copy from  pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date_du = '" + txt_bill_rtn_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "'  and dublicate_id_card = 'YES'");

                if (dublicate_stamp_copy == "")
                {
                    string fileName = Path.GetFileName(txt_report_receiving.PostedFile.FileName);
                    txt_report_receiving.PostedFile.SaveAs(Server.MapPath("~/dublicate_id_card/") + fileName);
                    fname = ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + "_" + abc + fileExt;
                    File.Copy(Server.MapPath("~/dublicate_id_card/") + fileName, Server.MapPath("~/dublicate_id_card/") + fname, true);
                    File.Delete(Server.MapPath("~/dublicate_id_card/") + fileName);

                    int res1 = d.operation("UPDATE pay_dispatch_billing SET du_stamp_copy = '" + ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + "_" + abc + fileExt + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date_du = '" + txt_bill_rtn_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "'  and dublicate_id_card = 'YES'  ");
                    if (res1 > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Successfully Updated... !!!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Updation Failed... !!!');", true);
                    }

                }
            }

        }

        // for second dublicate dispatch date
        if (txt_dub_dispatch_date.Text != "")
        {

            if (txt_dub_dispatch_date.Text != "" && txt_sec_du_pod.Text == "")
            {

                int res = d.operation("UPDATE pay_dispatch_billing SET dispatch_date_du2  = '" + txt_dub_dispatch_date.Text + "', du_dispatch_by2='" + ddl_material_contract.SelectedValue + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "'  ");
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                }

            }

            if (txt_dub_dispatch_date.Text != "" && txt_sec_du_pod.Text != "" && ddl_material_contract.SelectedValue == "0")
            {

                int res = d.operation("UPDATE pay_dispatch_billing SET du_pod_no2 ='" + txt_sec_du_pod.Text + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "' and dispatch_date_du2='" + txt_dub_dispatch_date.Text + "' and dublicate_id_card = 'YES'  ");
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                }

            }


            if (txt_dub_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "2" && txt_dub_receiver_name.Text != "" && txt_du_receiver_date.Text != "")
            {
                // update dublicate id  receiver name and date
                int res = d.operation("UPDATE pay_dispatch_billing SET du_receiving_name2 = '" + txt_dub_receiver_name.Text + "',receiving_date_du2 = '" + txt_du_receiver_date.Text + "'  where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "' and dispatch_date_du2 ='" + txt_dub_dispatch_date.Text + "'  and dublicate_id_card = 'YES'  ");
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                }

            }

            // update dublicate id stamp copy 
            string fileExt = System.IO.Path.GetExtension(txt_report_receiving.FileName);

            string fname = null;

            string abc = txt_bill_rtn_date.Text.Replace("/", "_");

            if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".ZIP")
            {
                string dublicate_stamp_copy = d.getsinglestring("select du_stamp_copy2 from  pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date_du2 = '" + txt_dub_dispatch_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "'  and dublicate_id_card = 'YES'");

                if (dublicate_stamp_copy == "")
                {
                    string fileName = Path.GetFileName(txt_report_receiving.PostedFile.FileName);
                    txt_report_receiving.PostedFile.SaveAs(Server.MapPath("~/dublicate_id_card/") + fileName);
                    fname = ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + "_" + abc + fileExt;
                    File.Copy(Server.MapPath("~/dublicate_id_card/") + fileName, Server.MapPath("~/dublicate_id_card/") + fname, true);
                    File.Delete(Server.MapPath("~/dublicate_id_card/") + fileName);

                    int res1 = d.operation("UPDATE pay_dispatch_billing SET du_stamp_copy2 = '" + ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + "_" + abc + fileExt + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date_du2 = '" + txt_dub_dispatch_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "'  and dublicate_id_card = 'YES'  ");
                    if (res1 > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Successfully Updated... !!!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Updation Failed... !!!');", true);
                    }

                }
            }





        }


        // third dublicate dispatch date 

        if (txt_dub_dispatch_date3.Text != "")
        {

            if (txt_dub_dispatch_date3.Text != "" && txt_third_du_pod.Text == "")
            {

                int res = d.operation("UPDATE pay_dispatch_billing SET dispatch_date_du3  = '" + txt_dub_dispatch_date3.Text + "', du_dispatch_by3='" + ddl_material_contract.SelectedValue + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "'  ");
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                }

            }

            if (txt_dub_dispatch_date3.Text != "" && txt_third_du_pod.Text != "" && ddl_material_contract.SelectedValue == "0")
            {

                int res = d.operation("UPDATE pay_dispatch_billing SET du_pod_no3 ='" + txt_sec_du_pod.Text + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "' and dispatch_date_du2='" + txt_dub_dispatch_date.Text + "' and dublicate_id_card = 'YES'  ");
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                }

            }


            if (txt_dub_dispatch_date3.Text != "" && ddl_material_contract.SelectedValue == "2" && txt_dub_receiver_name_third.Text != "" && txt_du_receiver_date_third.Text != "")
            {
                // update dublicate id  receiver name and date
                int res = d.operation("UPDATE pay_dispatch_billing SET du_receiving_name3 = '" + txt_dub_receiver_name_third.Text + "',receiving_date_du3 = '" + txt_du_receiver_date_third.Text + "'  where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' and shipping_address = '" + txt_bill_reason.Text + "' and dispatch_date_du2 ='" + txt_dub_dispatch_date.Text + "'  and dublicate_id_card = 'YES'  ");
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dublicate ID Dispatch Updation Failed... !!!');", true);
                }

            }

            // update dublicate id stamp copy 
            string fileExt = System.IO.Path.GetExtension(txt_report_receiving.FileName);

            string fname = null;

            string abc = txt_bill_rtn_date.Text.Replace("/", "_");

            if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".ZIP")
            {
                string dublicate_stamp_copy = d.getsinglestring("select du_stamp_copy2 from  pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date_du2 = '" + txt_dub_dispatch_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "'  and dublicate_id_card = 'YES'");

                if (dublicate_stamp_copy == "")
                {
                    string fileName = Path.GetFileName(txt_report_receiving.PostedFile.FileName);
                    txt_report_receiving.PostedFile.SaveAs(Server.MapPath("~/dublicate_id_card/") + fileName);
                    fname = ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + "_" + abc + fileExt;
                    File.Copy(Server.MapPath("~/dublicate_id_card/") + fileName, Server.MapPath("~/dublicate_id_card/") + fname, true);
                    File.Delete(Server.MapPath("~/dublicate_id_card/") + fileName);

                    int res1 = d.operation("UPDATE pay_dispatch_billing SET du_stamp_copy3 = '" + ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + "_" + abc + fileExt + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date_du2 = '" + txt_dub_dispatch_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "'  and dublicate_id_card = 'YES'  ");
                    if (res1 > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Successfully Updated... !!!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Updation Failed... !!!');", true);
                    }

                }
            }





        }




    }



    protected void update_stamp_copy()
    {

        if (ddl_material_contract.SelectedValue == "0" && txt_pod_number.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Please Enter First POD Number... !!!');", true);
            return;


        }
        if (txt_sec_dispatch_date.Text != "")
        {

            string No_of_set = d.getsinglestring("select remaining_no_set from pay_document_details where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "'  and unit_code = '" + ddl_branch_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and year_complete = 0 and document_type = 'Uniform' ");
            if (No_of_set != "0")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' First Add Number of Set From Uniform Tracking ... !!!');", true);
                return;
            }
            if (No_of_set == "0")
            {
                string uni_No_of_set = d.getsinglestring("select No_of_set from pay_document_details where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "'  and unit_code = '" + ddl_branch_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and year_complete = 0 and document_type = 'Uniform' ");
                string admin_No_of_set = d.getsinglestring("select (admin_no_of_set + remaining_no_set) from pay_document_details where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "'  and unit_code = '" + ddl_branch_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and year_complete = 0 and document_type = 'Uniform' ");

                if (uni_No_of_set == admin_No_of_set)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' First Add Number of Set From Uniform Tracking ... !!!');", true);
                    return;
                }

            }
        }
        if (ddl_material_contract.SelectedValue == "0" && txt_sec_pod.Text == "" && txt_sec_dispatch_date.Text != "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Please Enter Second POD Number... !!!');", true);
            return;

        }


        string fileExt = System.IO.Path.GetExtension(txt_report_receiving.FileName);


        string fname = null;

        string abc = txt_bill_rtn_date.Text.Replace("/", "_");

        // for first resiving date
        if (txt_receiv_date.Text != "")
        {
            if (check_date(txt_bill_rtn_date.Text, txt_receiv_date.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('First  Receiving date should be greater than dispatch date!!!')", true);
                return;
            }
        }

        // for second dispatch date
        if (txt_sec_dispatch_date.Text != "")
        {

            if (txt_sec_dispatch_date.Text != "" && txt_bill_rtn_date.Text != "" && ddl_material_contract.SelectedValue == "0")
            {

                if (second_dispatch(txt_bill_rtn_date.Text, txt_sec_dispatch_date.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Second Dispatch Date should be greater than First Dispatch date!!!')", true);
                    return;
                }
            }

            if (txt_sec_dispatch_date.Text != "" && txt_receiv_date.Text != "" && ddl_material_contract.SelectedValue == "2")
            {

                if (second_dispatch(txt_bill_rtn_date.Text, txt_sec_dispatch_date.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Second Dispatch Date should be greater than First Receiving date!!!')", true);
                    return;
                }
            }
        }
        // for second resiving date

        if (txt_sec_dispatch_date.Text != "" && txt_sec_rece_date.Text != "" && ddl_material_contract.SelectedValue == "2")
        {
            if (second_resiving(txt_sec_dispatch_date.Text, txt_sec_rece_date.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Second Receiving date should be greater than Second dispatch date!!!')", true);
                return;
            }
        }





        if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".ZIP")
        {
            string first_stamp_copy = d.getsinglestring("select stamp_copy from  pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "' AND dis_year_complete = 0");

            if (first_stamp_copy == "")
            {
                string fileName = Path.GetFileName(txt_report_receiving.PostedFile.FileName);
                txt_report_receiving.PostedFile.SaveAs(Server.MapPath("~/approved_attendance_images/") + fileName);
                fname = ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + "_" + abc + fileExt;
                File.Copy(Server.MapPath("~/approved_attendance_images/") + fileName, Server.MapPath("~/approved_attendance_images/") + fname, true);
                File.Delete(Server.MapPath("~/approved_attendance_images/") + fileName);

                int res = d.operation("UPDATE pay_dispatch_billing SET stamp_copy = '" + ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + "_" + abc + fileExt + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "' AND dis_year_complete = 0 ");
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Updation Failed... !!!');", true);
                }

            }

            // for second stamp copy
            if (first_stamp_copy != "" && txt_sec_dispatch_date.Text != "" && txt_sec_rece_name.Text != "" && txt_sec_rece_date.Text != "" && ddl_material_contract.SelectedValue == "2")
            {
                string fileName = Path.GetFileName(txt_report_receiving.PostedFile.FileName);
                txt_report_receiving.PostedFile.SaveAs(Server.MapPath("~/Return_material_images/") + fileName);
                fname = ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + fileExt;
                File.Copy(Server.MapPath("~/Return_material_images/") + fileName, Server.MapPath("~/Return_material_images/") + fname, true);
                File.Delete(Server.MapPath("~/Return_material_images/") + fileName);


                int res2 = d.operation("UPDATE pay_dispatch_billing SET second_stamp_copy = '" + ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + fileExt + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "' and sec_dispatch_date='" + txt_sec_dispatch_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "' AND dis_year_complete = 0 ");
                if (res2 > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Second Dispatched Bill Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Second Dispatched Copy Updation Failed... !!!');", true);
                }


            }


            // for by curier update sec stamp copy

            if (first_stamp_copy != "" && txt_sec_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "0")
            {
                string fileName = Path.GetFileName(txt_report_receiving.PostedFile.FileName);
                txt_report_receiving.PostedFile.SaveAs(Server.MapPath("~/Return_material_images/") + fileName);
                fname = ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + fileExt;
                File.Copy(Server.MapPath("~/Return_material_images/") + fileName, Server.MapPath("~/Return_material_images/") + fname, true);
                File.Delete(Server.MapPath("~/Return_material_images/") + fileName);


                //int res2 = d.operation("UPDATE pay_dispatch_billing SET second_stamp_copy = '" + ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + fileExt + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "' and sec_dispatch_date='" + txt_sec_dispatch_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "' AND dis_year_complete = 0 ");and sec_dispatch_date is null
                int res2 = d.operation("UPDATE pay_dispatch_billing SET second_stamp_copy = '" + ddl_client_name.SelectedValue + "_" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + fileExt + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and designation = '" + ddl_designation.SelectedValue + "' AND dis_year_complete = 0 ");
                if (res2 > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Second Dispatched Bill Successfully Updated... !!!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Second Dispatched Copy Updation Failed... !!!');", true);
                }


            }




            ////////////////

        }
        // for first resiving name and date pod

        if (((txt_receiver_name.Text != "" && txt_receiv_date.Text != "") || txt_pod_number.Text != "") && txt_sec_rece_name.Text == "" && txt_sec_rece_date.Text == "")
        {
            int res2 = d.operation("UPDATE pay_dispatch_billing SET pod_no = '" + txt_pod_number.Text + "', receiving_date = '" + txt_receiv_date.Text + "', receiver_name_invoice = '" + txt_receiver_name.Text + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "' AND dis_year_complete = 0 ");
            if (res2 > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Dispatch Bill Successfully Updated... !!!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Dispatch Bill Updation Failed... !!!');", true);
            }




        }
        // for second dispatch date by curier

        if (txt_sec_dispatch_date.Text != "")
        {

            int sec_dispatch = d.operation("UPDATE pay_dispatch_billing SET sec_dispatch_date='" + txt_sec_dispatch_date.Text + "'  where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "' AND dis_year_complete = 0 ");
            if (sec_dispatch > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Second Dispatch Date Updated  !!!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Second Dispatch Date Does not Updated ... !!!');", true);
            }

        }






        //sec pod update
        if (txt_sec_pod.Text != "" && txt_sec_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "0")
        {

            int second_record = d.operation("UPDATE pay_dispatch_billing SET sec_pod_no = '" + txt_sec_pod.Text + "',sec_receiving_date = '" + txt_sec_rece_date.Text + "', sec_receiver_name = '" + txt_sec_rece_name.Text + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "' AND dis_year_complete = 0 ");
            if (second_record > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Second Pod Successfully Updated... !!!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Second Pod Updation Failed... !!!');", true);
            }

        }



        // resiving date and name

        if (txt_sec_rece_date.Text != "" && txt_sec_rece_name.Text != "" && txt_sec_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "2")
        {

            int second_record = d.operation("UPDATE pay_dispatch_billing SET sec_receiving_date = '" + txt_sec_rece_date.Text + "', sec_receiver_name = '" + txt_sec_rece_name.Text + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "' and designation = '" + ddl_designation.SelectedValue + "' AND dis_year_complete = 0 ");
            if (second_record > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Second Pod Successfully Updated... !!!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Second Pod Updation Failed.... !!!');", true);
            }

        }



        //////////////////

        Panel_receiver_name.Visible = false;
        Panel_receiving_date.Visible = false;
        Sec_Panel_receiver_name.Visible = false;
        Sec_Panel_receiving_date.Visible = false;
        pod_number.Visible = false;
        btn_update.Visible = false;
        btn_uniform_shoes.Visible = false;
        btn_bill_save.Visible = true;

        dispatch_resiver();
        //clear_text();

    }

    protected void invoice_stamp_copy()
    {
        string region = "", region_name = "", exten_region = "";
        if (ddl_client_name.SelectedValue == "DHFL" || (ddl_client_name.SelectedValue == "BAGIC" && ddl_state_name.SelectedValue == "Maharashtra" && int.Parse("" + txt_month_year.Text.Substring(3) + "" + txt_month_year.Text.Substring(0, 2) + "") > 20204))
        {
            exten_region = "" + ddlregion.SelectedValue + "_";
            region = "AND ZONE ='" + ddlregion.SelectedValue + "'";
            region_name = "AND region_name = '" + ddlregion.SelectedValue + "'";
        }


        string dispatch_date_billing = d.getsinglestring("select distinct date_format(billing_date,'%d/%m/%Y') as 'billing_date' from pay_billing_unit_rate_history where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and state_name = '" + ddl_state_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and month = '" + txt_month_year.Text.Substring(0, 2) + "' and year = '" + txt_month_year.Text.Substring(3) + "' " + region + "");

        if (dispatch_date_billing != "")
        {

            if (check_dispatch_date(txt_bill_rtn_date.Text, dispatch_date_billing))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Invoice Dispatch Date should be greater than Billing Date!!!')", true);
                return;
            }

            int res = d.operation("UPDATE pay_dispatch_billing SET dispatch_date ='" + txt_bill_rtn_date.Text + "'   where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and month = '" + txt_month_year.Text + "'and invoice_no= '" + lbl_invoice_num.Text + "' and dispatch_by= '" + ddl_material_contract.SelectedValue + "'" + region_name);
            if (res > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Record Successfully Updated... !!!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Stamp Copy Updation Failed... !!!');", true);
            }


        }





        if (ddl_material_contract.SelectedValue == "0" && txt_pod_number.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Please Enter First POD Number... !!!');", true);
            return;

        }

        // for first resiving date
        if (ddl_material_contract.SelectedValue == "2")
        {
            if (check_date(txt_bill_rtn_date.Text, txt_receiv_date.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('First  Receiving date should be greater than dispatch date!!!')", true);
                return;
            }
        }


        //string date = "select dispatch_date from pay_dispatch_billing where comp_code = '" + Session["C01"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "'";
        string abc = txt_bill_rtn_date.Text.Replace("/", "_");

        string fileExt = System.IO.Path.GetExtension(txt_report_receiving.FileName);
        string fname = null;
        if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".ZIP")
        {
            string fileName = Path.GetFileName(txt_report_receiving.PostedFile.FileName);
            txt_report_receiving.PostedFile.SaveAs(Server.MapPath("~/approved_attendance_images/") + fileName);
            fname = ddl_client_name.SelectedValue + "_" + exten_region + "" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + "_" + abc + fileExt;
            File.Copy(Server.MapPath("~/approved_attendance_images/") + fileName, Server.MapPath("~/approved_attendance_images/") + fname, true);
            File.Delete(Server.MapPath("~/approved_attendance_images/") + fileName);

            int res = d.operation("UPDATE pay_dispatch_billing SET invoice_stamp_copy = '" + ddl_client_name.SelectedValue + "_" + exten_region + "" + ddl_state_name.SelectedValue + "_" + ddl_branch_name.SelectedValue + "_" + abc + fileExt + "'  where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "'   and invoice_no = '" + lbl_invoice_num.Text + "'   and month = '" + txt_month_year.Text + "' " + region_name);
            if (res > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Record Successfully Updated... !!!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Stamp Copy Updation Failed... !!!');", true);
            }


        }

        if ((txt_receiver_name.Text != "" && txt_receiv_date.Text != "") || txt_pod_number.Text != "")
        {

            int pod_no = d.operation("UPDATE pay_dispatch_billing SET pod_no = '" + txt_pod_number.Text + "' ,receiving_date = '" + txt_receiv_date.Text + "' , receiver_name_invoice = '" + txt_receiver_name.Text + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch='" + ddl_state_name.SelectedValue + "' and  dispatch_date = '" + txt_bill_rtn_date.Text + "' and invoice_no = '" + lbl_invoice_num.Text + "'  and invoice_date = '" + txt_invoice_date.Text + "' and grand_total = '" + txt_grand_total.Text + "' and month = '" + txt_month_year.Text + "'  " + region_name);

            if (pod_no > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' POD Number Successfully Updated... !!!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' POD Number Updation Failed... !!!');", true);
            }
        }
        dispatch_resiver();
        //uniform_gridview();
        //gridview_invoice();
        clear_text();


    }

    protected void downloadfile(string filename)
    {

        //I03526_25.jpg
        //I03118_25
        try
        {
            var result = filename.Substring(filename.Length - 4);
            if (result.Contains("jpeg"))
            {
                result = ".jpeg";
            }

            string path2 = Server.MapPath("~\\approved_attendance_images\\" + filename);
            //  string unitName = stamp_copy + "-Attendance" + result;
            Response.Clear();
            Response.ContentType = "Application/pdf/jpg/jpeg/png/zip";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename);
            Response.TransmitFile("~\\approved_attendance_images\\" + filename);
            Response.WriteFile(path2);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Response.End();
            Response.Close();


        }
        catch (Exception ex) { }
    }

    protected void downloadfile_second(string filename_second)
    {

        //I03526_25.jpg
        //I03118_25
        try
        {
            var result = filename_second.Substring(filename_second.Length - 4);
            if (result.Contains("jpeg"))
            {
                result = ".jpeg";
            }

            string path2 = Server.MapPath("~\\Return_material_images\\" + filename_second);
            //  string unitName = stamp_copy + "-Attendance" + result;
            Response.Clear();
            Response.ContentType = "Application/pdf/jpg/jpeg/png/zip";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename_second);
            Response.TransmitFile("~\\Return_material_images\\" + filename_second);
            Response.WriteFile(path2);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Response.End();
            Response.Close();
        }
        catch (Exception ex) { }
    }

    // for first dublicate id card
    protected void downloadfile_dub_id(string filename_du)
    {
        try
        {
            var result = filename_du.Substring(filename_du.Length - 4);
            if (result.Contains("jpeg"))
            {
                result = ".jpeg";
            }

            string path2 = Server.MapPath("~\\dublicate_id_card\\" + filename_du);
            //  string unitName = stamp_copy + "-Attendance" + result;
            Response.Clear();
            Response.ContentType = "Application/pdf/jpg/jpeg/png/zip";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename_du);
            Response.TransmitFile("~\\dublicate_id_card\\" + filename_du);
            Response.WriteFile(path2);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Response.End();
            Response.Close();
        }
        catch (Exception ex) { }


    }

    // for third dublicate id card

    protected void downloadfile_dub_id3(string filename_du3)
    {
        try
        {
            var result = filename_du3.Substring(filename_du3.Length - 4);
            if (result.Contains("jpeg"))
            {
                result = ".jpeg";
            }

            string path2 = Server.MapPath("~\\dublicate_id_card\\" + filename_du3);
            //  string unitName = stamp_copy + "-Attendance" + result;
            Response.Clear();
            Response.ContentType = "Application/pdf/jpg/jpeg/png/zip";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename_du3);
            Response.TransmitFile("~\\dublicate_id_card\\" + filename_du3);
            Response.WriteFile(path2);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Response.End();
            Response.Close();
        }
        catch (Exception ex) { }


    }


    // for second dublicate id card

    protected void downloadfile_dub_id2(string filename_du2)
    {
        try
        {
            var result = filename_du2.Substring(filename_du2.Length - 4);
            if (result.Contains("jpeg"))
            {
                result = ".jpeg";
            }

            string path2 = Server.MapPath("~\\dublicate_id_card\\" + filename_du2);
            //  string unitName = stamp_copy + "-Attendance" + result;
            Response.Clear();
            Response.ContentType = "Application/pdf/jpg/jpeg/png/zip";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename_du2);
            Response.TransmitFile("~\\dublicate_id_card\\" + filename_du2);
            Response.WriteFile(path2);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Response.End();
            Response.Close();
        }
        catch (Exception ex) { }


    }


    protected void lnk_uniform_Command(object sender, CommandEventArgs e)
    {
        string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
        GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow1.Cells[1].Text;


        //  ViewState["id"] = gv_material.SelectedRow.Cells[1].Text;
        string data = d.getsinglestring(" select stamp_copy from pay_dispatch_billing where id = '" + id + "'  ");
        string filename = data;
        //string stamp_copy = commandArgs[1];

        //string filename = e.CommandArgument.ToString();
        ////string unit_name = gv_approve_attendace.SelectedRow.Cells[2].ToString();
        if (filename != "")
        {
            downloadfile(filename);
            //gridview_invoice();
            //uniform_gridview();
            dispatch_resiver();
        }

        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attachment File Cannot Be Uploaded !!!')", true);
        }



    }
    protected string check1()
    {
        // d12.con.Open();
        string count = "";
        /////////////
        string state_add = null;
        string unit_code1 = null;
        string state_all = null;

        if (ddl_state_name.SelectedValue == "ALL")
        {
            if (ddl_inv_details.SelectedValue == "1")
            {

                state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_unit_rate_history  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "' and invoice_flag = '2' ");
            }
            else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
            {

                state_add = d.get_group_concat("select group_concat(distinct(STATE_NAME)) from pay_billing_material_history  where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "' and invoice_flag = '2' ");

            }

            state_add = "'" + state_add + "'";
            state_add = state_add.Replace(",", "','");

        }
        else if (ddl_state_name.SelectedValue != "ALL" && ddl_state_name.SelectedValue != "Select")
        {

            Session["STATE_CODE"] = ddl_state_name.SelectedValue;
            state_add = "'" + Session["STATE_CODE"].ToString() + "'";

        }

        if (ddl_state_name.SelectedValue != "Select")
        {
            if (ddl_inv_details.SelectedValue == "1")
            {
                unit_code1 = d.get_group_concat("select group_concat(distinct(unit_code)) from pay_billing_unit_rate_history  where  pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_unit_rate_history.STATE_NAME IN( " + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' ");
            }
            else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
            {
                unit_code1 = d.get_group_concat("select group_concat(distinct(unit_code)) from pay_billing_material_history  where  pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' and pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_material_history.STATE_NAME IN( " + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' ");
            }


            unit_code1 = "'" + unit_code1 + "'";
            unit_code1 = unit_code1.Replace(",", "','");
        }
        // for statewise or statedesignationwise
        string same_invoice = null;
        string state1 = d.getsinglestring("SELECT DISTINCT billing_wise FROM pay_client_billing_details WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND client_code = '" + ddl_client_name.SelectedValue + "'");

        if (state1 == "Statewise")
        {

            if (ddl_state_name.SelectedValue != "ALL")
            {
                if (ddl_inv_details.SelectedValue == "1")
                {

                    same_invoice = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_unit_rate_history.unit_code IN (" + unit_code1 + ") and pay_billing_unit_rate_history.STATE_NAME IN (" + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");

                }
                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {

                    same_invoice = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_material_history WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "' AND pay_billing_material_history.unit_code IN (" + unit_code1 + ") and pay_billing_material_history.STATE_NAME IN (" + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");

                }

            }
            else if (ddl_state_name.SelectedValue == "ALL")
            {
                string auto_invoice_noo = d.getsinglestring("select group_concat(invoice_no) from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' AND pay_dispatch_billing.month = '" + txt_month_year.Text + "' and client_code = '" + ddl_client_name.SelectedValue + "'");
                state_all = d.getsinglestring("select group_concat(state_dispatch) from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' AND pay_dispatch_billing.month = '" + txt_month_year.Text + "' and client_code = '" + ddl_client_name.SelectedValue + "'");

                auto_invoice_noo = "'" + auto_invoice_noo + "'";
                auto_invoice_noo = auto_invoice_noo.Replace(",", "','");

                if (ddl_inv_details.SelectedValue == "1")
                {

                    same_invoice = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_unit_rate_history WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_unit_rate_history.unit_code IN (" + unit_code1 + ") and pay_billing_unit_rate_history.STATE_NAME IN (" + state_add + ")and auto_invoice_no not in (" + auto_invoice_noo + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");
                }

                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {

                    same_invoice = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_material_history WHERE comp_code = '" + Session["comp_code"].ToString() + "' and pay_billing_material_history.type = '" + ddl_inv_details.SelectedItem + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_material_history.unit_code IN (" + unit_code1 + ") and pay_billing_material_history.STATE_NAME IN (" + state_add + ")and auto_invoice_no not in (" + auto_invoice_noo + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");
                }

            }
        }
        else if (state1 == "Statewisedesignation")
        {
            string design_all = d.getsinglestring("SELECT group_concat(distinct (pay_grade_master.grade_code))FROM pay_grade_master INNER JOIN pay_billing_unit_rate_history ON pay_grade_master.comp_code = pay_billing_unit_rate_history.comp_code AND pay_grade_master.grade_desc = pay_billing_unit_rate_history.grade_desc WHERE pay_grade_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'  and pay_billing_unit_rate_history.unit_code IN ( " + unit_code1 + ") AND state_name IN (" + state_add + ") and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "'");
            design_all = design_all.Replace(",", "','");

            string grade_code = d.getsinglestring("SELECT group_concat(grade_desc) FROM pay_grade_master WHERE grade_code IN ('" + design_all + "') AND comp_code = '" + Session["comp_code"].ToString() + "' ");
            grade_code = grade_code.Replace(",", "','");


            if (ddl_state_name.SelectedValue != "ALL")
            {
                if (ddl_inv_details.SelectedValue == "1")
                {
                    same_invoice = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_unit_rate_history INNER JOIN pay_grade_master ON pay_billing_unit_rate_history.comp_code = pay_grade_master.comp_code AND pay_billing_unit_rate_history.grade_desc = pay_grade_master.GRADE_DESC WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_unit_rate_history.unit_code IN (" + unit_code1 + ") and pay_billing_unit_rate_history.STATE_NAME IN (" + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' AND pay_billing_unit_rate_history.grade_desc IN ('" + grade_code + "')  and invoice_flag = '2' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");
                }
                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {
                    same_invoice = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_material_history INNER JOIN pay_grade_master ON pay_billing_material_history.comp_code = pay_grade_master.comp_code AND pay_billing_material_history.grade_desc = pay_grade_master.GRADE_DESC WHERE pay_billing_material_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_material_history.client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_material_history.unit_code IN (" + unit_code1 + ") and pay_billing_unit_rate_history.STATE_NAME IN (" + state_add + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' AND pay_billing_unit_rate_history.grade_desc IN ('" + grade_code + "')  and invoice_flag = '2' AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");
                }

            }
            else if (ddl_state_name.SelectedValue == "ALL")
            {
                string auto_invoice_noo = d.getsinglestring("select group_concat(invoice_no) from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' AND pay_dispatch_billing.month = '" + txt_month_year.Text + "' and client_code = '" + ddl_client_name.SelectedValue + "'");
                state_all = d.getsinglestring("select group_concat(state_dispatch) from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' AND pay_dispatch_billing.month = '" + txt_month_year.Text + "' and client_code = '" + ddl_client_name.SelectedValue + "'");

                auto_invoice_noo = "'" + auto_invoice_noo + "'";
                auto_invoice_noo = auto_invoice_noo.Replace(",", "','");

                if (ddl_inv_details.SelectedValue == "1")
                {
                    same_invoice = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_unit_rate_history INNER JOIN pay_grade_master ON pay_billing_unit_rate_history.comp_code = pay_grade_master.comp_code AND pay_billing_unit_rate_history.grade_desc = pay_grade_master.GRADE_DESC WHERE pay_billing_unit_rate_history.comp_code = '" + Session["comp_code"].ToString() + "'  AND pay_billing_unit_rate_history.client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_unit_rate_history.unit_code IN (" + unit_code1 + ") and pay_billing_unit_rate_history.STATE_NAME IN (" + state_add + ")and auto_invoice_no not in (" + auto_invoice_noo + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' AND pay_billing_unit_rate_history.grade_desc IN ('" + grade_code + "') AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");
                }
                else if (ddl_inv_details.SelectedValue == "2" || ddl_inv_details.SelectedValue == "3" || ddl_inv_details.SelectedValue == "4")
                {
                    same_invoice = d.getsinglestring("SET SESSION group_concat_max_len = 100000; SELECT GROUP_CONCAT(auto_invoice_no) FROM (SELECT auto_invoice_no FROM pay_billing_material_history INNER JOIN pay_grade_master ON pay_billing_material_history.comp_code = pay_grade_master.comp_code AND pay_billing_material_history.grade_desc = pay_grade_master.GRADE_DESC WHERE comp_code = '" + Session["comp_code"].ToString() + "'  AND client_code = '" + ddl_client_name.SelectedValue + "'  AND pay_billing_material_history.unit_code IN (" + unit_code1 + ") and pay_billing_material_history.STATE_NAME IN (" + state_add + ")and auto_invoice_no not in (" + auto_invoice_noo + ")  and month='" + txt_month_year.Text.Substring(0, 2) + "' and year='" + txt_month_year.Text.Substring(3) + "' and invoice_flag = '2' AND pay_billing_material_history.grade_desc IN ('" + grade_code + "') AND auto_invoice_no IS NOT NULL group by auto_invoice_no ) as t");
                }
            }
        }

        /////////////////
        string dublicate_invoice = null;
        dublicate_invoice = d.getsinglestring(" select group_concat(invoice_no) from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' AND pay_dispatch_billing.month = '" + txt_month_year.Text + "' and state_dispatch IN (" + state_add + ") and client_code = '" + ddl_client_name.SelectedValue + "' ");

        if (dublicate_invoice != "")
        {
            string[] dublicate_invoice1 = dublicate_invoice.Split(',');

            foreach (object obj in dublicate_invoice1)
            {

                string[] same_invoice_no = same_invoice.Split(',');

                foreach (object obj1 in same_invoice_no)
                {
                    string aaa = "'" + obj + "'";

                    string bbb = "'" + obj1 + "'";


                    //if (lbl_invoice_num.Text != "" && aaa !="")
                    //{

                    if (aaa == bbb)
                    {
                        //if (ddl_state_name.SelectedValue == state_all)
                        // {

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' this Invoice Number All ready Added... !!!');", true);
                        count = "1";
                        return count;
                        // }
                    }

                    // }

                }
            }

            if (ddl_state_name.SelectedValue == "ALL")
            {
                string[] state_all1 = state_all.Split(',');

                foreach (object obj2 in state_all1)
                {

                    string state = "'" + obj2 + "'";
                    string ddl_state = "'" + ddl_state_name.SelectedValue + "'";

                    if (ddl_state == state)
                    {

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' this Invoice Number Allready Added... !!!');", true);
                        count = "1";
                        return count;
                    }
                }
            }
        }


        count = "2";
        return count;
    }

    protected string emp_Name_material()
    {
        try
        {
            string emp = "";
            d.con.Open();

            string month_name = d.getsinglestring(" SELECT dispatch_date,emp_code FROM  pay_dispatch_billing WHERE comp_code = '" + Session["comp_code"].ToString() + "'AND client_code = '" + ddl_client_name.SelectedValue + "'AND state_dispatch = '" + ddl_state_name.SelectedValue + "' AND unit_code = '" + ddl_branch_name.SelectedValue + "' AND dispatch_by = '" + ddl_material_contract.SelectedValue + "' and receiver_type= '" + ddl_Receiver_type.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "' and dispatch_date = '" + txt_bill_rtn_date.Text + "'");

            if (month_name == txt_bill_rtn_date.Text)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' This Year Employee ALL Ready Added ... !!!');", true);
                emp = "1";
                return emp;
            }
            emp = "2";
            return emp;
        }


        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }
    }



    protected void link_uniform_Click(object sender, EventArgs e)
    {
        try
        {
            d5.con.Open();
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
            string id = grdrow.Cells[1].Text;

            //ViewState["id"] = gv_material.SelectedRow.Cells[1].Text;
            MySqlCommand ds_uniform = new MySqlCommand(" select dispatch_by,client_code,state_dispatch,unit_code,emp_code,designation,receiver_name_invoice,receiving_date,dispatch_date,pod_no,shipping_address,receiver_type,sec_dispatch_date,sec_receiver_name,sec_receiving_date,sec_pod_no from pay_dispatch_billing where Id = '" + id + "'", d5.con);



            MySqlDataReader dr_uniform = ds_uniform.ExecuteReader();

            txt_sec_dispatch_date.Text = "";
            if (dr_uniform.Read())
            {
                try
                {

                    ddl_Receiver_type.SelectedValue = dr_uniform.GetValue(11).ToString();
                    ddl_material_contract.SelectedValue = dr_uniform.GetValue(0).ToString();
                    ddl_client_name.SelectedValue = dr_uniform.GetValue(1).ToString();
                    ddl_client_name_SelectedIndexChanged(null, null);
                    ddl_state_name.SelectedValue = dr_uniform.GetValue(2).ToString();
                    ddl_state_name_SelectedIndexChanged(null, null);

                }
                catch { }
                try
                {
                    string unit = dr_uniform.GetValue(3).ToString();
                    ddl_branch_name.SelectedValue = unit;
                    ddl_branch_name_SelectedIndexChanged(null, null);

                }
                catch { }
                try
                {
                    string emp = dr_uniform.GetValue(4).ToString();
                    ddl_emp_name.SelectedValue = emp;
                    ddl_emp_name_SelectedIndexChanged(null, null);

                }
                catch { }
                try
                {
                    string desi = dr_uniform.GetValue(5).ToString();
                    ddl_designation.SelectedValue = desi;
                }
                catch { }
                try
                {

                    txt_receiver_name.Text = dr_uniform.GetValue(6).ToString();
                    txt_receiv_date.Text = dr_uniform.GetValue(7).ToString();
                    txt_bill_rtn_date.Text = dr_uniform.GetValue(8).ToString();
                    txt_pod_number.Text = dr_uniform.GetValue(9).ToString();
                    txt_bill_reason.Text = dr_uniform.GetValue(10).ToString();
                    txt_sec_dispatch_date.Text = dr_uniform.GetValue(12).ToString();
                    txt_sec_rece_name.Text = dr_uniform.GetValue(13).ToString();
                    txt_sec_rece_date.Text = dr_uniform.GetValue(14).ToString();
                    txt_sec_pod.Text = dr_uniform.GetValue(15).ToString();

                }
                catch { }
            }
            dr_uniform.Dispose();
            d5.con.Close();


            btn_update.Visible = true;

            report_panel.Visible = true;
            btn_dispatch_save.Visible = false;
            btn_bill_save.Visible = false;
            //for first dispatch date
            if (txt_receiver_name.Text == "" || txt_receiv_date.Text == "")
            {
                Panel_receiver_name.Visible = true;
                Panel_receiving_date.Visible = true;
                pod_number.Visible = true;
            }
            //for second dispatch date
            if ((txt_receiver_name.Text != "" && txt_sec_rece_name.Text == "") && (txt_receiv_date.Text != "" && txt_sec_rece_date.Text == ""))
            {
                Sec_Panel_receiver_name.Visible = true;
                Sec_Panel_receiving_date.Visible = true;
                dispatch_date_sec.Visible = true;
                sec_pod_number.Visible = true;
                Panel_dispatch.Visible = false;
                pod_number.Visible = false;
            }
            if (txt_receiver_name.Text != "" || txt_receiv_date.Text != "")
            {
                btn_uniform_shoes.Visible = true;
                second_btn_uniform_shoes.Visible = false;
            }
            if (txt_sec_rece_name.Text != "" || txt_sec_rece_date.Text != "")
            {
                btn_uniform_shoes.Visible = false;
                second_btn_uniform_shoes.Visible = true;
            }


            //21-12-19
            GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
            string dispatch_date1 = grdrow1.Cells[17].Text;
            string remaning_set1 = grdrow1.Cells[18].Text;
            string uniform_no_flag = grdrow1.Cells[23].Text;

            //if (uniform_no_flag == "1" && remaning_set1 == "0" && txt_bill_rtn_date.Text != "")
            //{ dispatch_date_sec.Visible = true; }


            //if (uniform_no_flag == "1" && remaning_set1 == "0" && txt_receiver_name.Text != "" && txt_receiv_date.Text != "" && ddl_material_contract.SelectedValue == "2")
            //{
            //    dispatch_date_sec.Visible = true;

            //}

            if (dispatch_date1 != "" && ddl_material_contract.SelectedValue == "2")
            {

                if (remaning_set1 == "0")
                {

                    Sec_Panel_receiver_name.Visible = true;
                    Sec_Panel_receiving_date.Visible = true;
                    dispatch_date_sec.Visible = true;

                }


            }

            //if (ddl_material_contract.SelectedValue == "0" && dispatch_date1 != "")
            //{
            //    sec_pod_number.Visible = true;

            //}
            //else { sec_pod_number.Visible = false; }


            // for second receiver name, date and pod no
            //foreach (GridViewRow gvrow in gv_material.Rows)
            // {
            //     string dispatch_date = gv_material.Rows[gvrow.RowIndex].Cells[17].Text;

            //     string remaning_set = gv_material.Rows[gvrow.RowIndex].Cells[18].Text;
            //     if (dispatch_date!="")
            //     {
            //     if (remaning_set != "" || remaning_set == "1") 
            //     {
            //     Sec_Panel_receiver_name.Visible = true;
            //     Sec_Panel_receiving_date.Visible = true;
            //     sec_pod_number.Visible = true;
            //     dispatch_date_sec.Visible = true;

            //    }
            //    }
            //}


            //if (txt_receiver_name.Text != "")
            //{
            //    btn_uniform_shoes.Visible = true;
            //}
            //else { btn_uniform_shoes.Visible = false; }


            // for second 
            //if (txt_sec_rece_name.Text != "")
            //{
            //    second_btn_uniform_shoes.Visible = true;
            //}
            //else { second_btn_uniform_shoes.Visible = false; }


            //  dispatch_resiver();


            //   if (ddl_material_contract.SelectedValue == "0")
            //{
            //    pod_number.Visible = true;
            //}
            //else { pod_number.Visible = false; }


        }
        catch (Exception ex) { throw ex; }
        finally
        {
            // dispatch_resiver();


            d5.con.Close();
        }


        // for second dispatch date
        //try
        //{
        //    d8.con.Open();

        //    GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        //    string id1 = grdrow1.Cells[1].Text;
        //    // for second dispatch date 
        //    string sec_dispatch_date1 = d.getsinglestring("select sec_dispatch_date from pay_dispatch_billing where id = '" + id1 + "'");

        //    if (sec_dispatch_date1 != "")
        //    {

        //        MySqlCommand ds_dispatch = new MySqlCommand(" select sec_dispatch_date from pay_dispatch_billing where id = '" + id1 + "'", d8.con);
        //        MySqlDataReader dr_dispatch = ds_dispatch.ExecuteReader();

        //        if (dr_dispatch.Read())
        //        {

        //            txt_sec_dispatch_date.Text = dr_dispatch.GetValue(0).ToString();

        //        }
        //        ds_dispatch.Dispose();

        //    }

        //}
        //catch (Exception ex) { throw ex; }
        //finally { d8.con.Close(); }


        // for receiver name and receiver date

        //try
        //{
        //    d8.con.Open();


        //    GridViewRow grdrow2 = (GridViewRow)((LinkButton)sender).NamingContainer;
        //    string id1 = grdrow2.Cells[1].Text;
        //    // for second dispatch date 
        //    string sec_receiver = d.getsinglestring("select sec_receiver_name,sec_receiving_date from pay_dispatch_billing where id = '" + id1 + "'");

        //    if (sec_receiver != "")
        //    {

        //        MySqlCommand ds_receiver = new MySqlCommand(" select sec_receiver_name,sec_receiving_date,sec_pod_no from pay_dispatch_billing where id = '" + id1 + "'", d8.con);
        //        MySqlDataReader dr_receiver = ds_receiver.ExecuteReader();

        //        if (dr_receiver.Read())
        //        {

        //            txt_sec_rece_name.Text = dr_receiver.GetValue(0).ToString();
        //            txt_sec_rece_date.Text = dr_receiver.GetValue(1).ToString();
        //            txt_sec_pod.Text = dr_receiver.GetValue(2).ToString();

        //        }
        //        ds_receiver.Dispose();


        //    }


        //}
        //catch (Exception ex) { throw ex; }
        //finally { d8.con.Close(); }






    }
    protected void link_invoice_Click(object sender, EventArgs e)
    {
        try
        {
            d3.con.Open();
            GridViewRow grd_invoce = (GridViewRow)((LinkButton)sender).NamingContainer;
            string id = grd_invoce.Cells[1].Text;

            //ViewState["id"] = gv_material.SelectedRow.Cells[1].Text;
            MySqlCommand ds_invoice = new MySqlCommand(" select dispatch_by,client_code,state_dispatch,unit_code,receiver_name_invoice,receiving_date,dispatch_date,pod_no,shipping_address,invoice_no,invoice_date,grand_total,receiver_type , month,invoice_type,region_name from pay_dispatch_billing where id = '" + id + "'", d3.con);

            MySqlDataReader dr_invoice = ds_invoice.ExecuteReader();

            if (dr_invoice.Read())
            {
                ddl_Receiver_type.SelectedValue = dr_invoice.GetValue(12).ToString();
                ddl_material_contract.SelectedValue = dr_invoice.GetValue(0).ToString();
                ddl_inv_details.SelectedValue = dr_invoice.GetValue(14).ToString();
                txt_month_year.Text = dr_invoice.GetValue(13).ToString();

                ddl_client_name.SelectedValue = dr_invoice.GetValue(1).ToString();
                if (dr_invoice.GetValue(15).ToString() != "" && dr_invoice.GetValue(15).ToString() != "Select")
                {
                    ddlregion.SelectedValue = dr_invoice.GetValue(15).ToString();
                    ddl_region_SelectedIndexChanged(null, null);
                }

                ddl_client_name_SelectedIndexChanged(null, null);



                ddl_state_name.SelectedValue = dr_invoice.GetValue(2).ToString();
                ddl_state_name_SelectedIndexChanged(null, null);

                ddl_branch_name.SelectedValue = dr_invoice.GetValue(3).ToString();
                ddl_branch_name_SelectedIndexChanged(null, null);
                txt_receiver_name.Text = dr_invoice.GetValue(4).ToString();
                txt_receiv_date.Text = dr_invoice.GetValue(5).ToString();
                txt_bill_rtn_date.Text = dr_invoice.GetValue(6).ToString();
                txt_pod_number.Text = dr_invoice.GetValue(7).ToString();
                txt_bill_reason.Text = dr_invoice.GetValue(8).ToString();
                lbl_invoice_num.Text = dr_invoice.GetValue(9).ToString();
                txt_invoice_date.Text = dr_invoice.GetValue(10).ToString();
                txt_grand_total.Text = dr_invoice.GetValue(11).ToString();

            }

            btn_update.Visible = true;
            btn_dispatch_save.Visible = false;
            report_panel.Visible = true;
            //pod_number.Visible = true;
            Panel_receiver_name.Visible = false;
            Panel_receiving_date.Visible = false;

            if (ddl_material_contract.SelectedValue == "0" || ddl_material_contract.SelectedValue == "1")
            {
                Print_Report.Visible = true;
            }
            else if (ddl_material_contract.SelectedValue == "2")
            {

                Print_Report.Visible = true;
            }
            else { Print_Report.Visible = false; }

            // 28-04-2020 komal changes
            // dispatch_resiver();
            //end

            if (ddl_material_contract.SelectedValue == "0")
            {
                pod_number.Visible = true;
            }
            else { pod_number.Visible = false; }


            if (ddl_material_contract.SelectedValue == "2")
            {
                Panel_receiver_name.Visible = true;
                Panel_receiving_date.Visible = true;

            }
            else
            {
                Panel_receiver_name.Visible = false;
                Panel_receiving_date.Visible = false;
            }


        }
        catch (Exception ex) { throw ex; }
        finally { d3.con.Close(); }

    }

    protected void lnk_invoice_Command1(object sender, CommandEventArgs e)
    {

        string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
        string value = commandArgs[0];

        if (value != "")
        {
            downloadfile(value);
        }

        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attachment File Cannot Be Uploaded !!!')", true);
        }


    }
    protected void gv_invoice_RowDataBound(object sender, GridViewRowEventArgs e)
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
            DataRowView dr1 = (DataRowView)e.Row.DataItem;
            if (dr1["invoice_stamp_copy"].ToString() == "")
            {
                //LinkButton lb1 = e.Row.FindControl("unit_name") as LinkButton;
                //lb1.Visible = false;
                e.Row.Cells[15].Text = "Dispatched";

            }
        }

        e.Row.Cells[1].Visible = false;
        e.Row.Cells[16].Visible = false;
        e.Row.Cells[17].Visible = false;
        //e.Row.Cells[18].Visible = false;
        e.Row.Cells[8].Visible = false;
        e.Row.Cells[7].Visible = false;
        e.Row.Cells[13].Visible = false;


    }
    protected void ddl_invoice_type_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_invoice_type.SelectedValue == "1")
        {
            ddl_designation.Items.Clear();
            desigpanel.Visible = false;
        }


        else if (ddl_invoice_type.SelectedValue == "2")
        {
            if (txt_month_year.Text != "")
            {
                ddl_designation.Items.Clear();
                desigpanel.Visible = true; int i = 0; string temp = "";

                if (ddl_branch_name.SelectedValue == "ALL")
                {
                    temp = d1.getsinglestring("select group_concat(distinct(designation)) from pay_billing_unit_rate where client_code='" + ddl_client_name.SelectedValue + "'  and year='" + txt_month_year.Text.Substring(3) + "'and month='" + txt_month_year.Text.Substring(0, 2) + "' and unit_code in (select unit_code from pay_unit_master where comp_code='" + Session["COMP_CODE"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and state_name = '" + ddl_state_name.SelectedValue + "')");
                }
                else
                {
                    temp = d1.getsinglestring("select group_concat(distinct(designation)) from pay_billing_unit_rate where client_code='" + ddl_client_name.SelectedValue + "'  and year='" + txt_month_year.Text.Substring(3) + "'and month='" + txt_month_year.Text.Substring(0, 2) + "'");
                }

                var designationlist = temp.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string designation in designationlist)
                {
                    ddl_designation.Items.Insert(i++, designation);
                }
                ddl_designation.Items.Insert(0, "Select");
                ddl_designation.Items.Insert(1, "ALL");
            }
            else
            { ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select Month and try again.');", true); }
        }




    }
    protected void ddl_designation_SelectedIndexChanged(object sender, EventArgs e)
    {


        billing_statewisedesignation();
    }
    protected void ddl_Receiver_type_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        try
        {
            gv_invoice.DataSource = null;
            gv_invoice.DataBind();
            gv_dublicate_id_card.DataSource = null;
            gv_dublicate_id_card.DataBind();
            gv_material.DataSource = null;
            gv_material.DataBind();

            if (ddl_Receiver_type.SelectedValue == "1")
            {
                Panel_month.Visible = false;
                Panel_inv_type.Visible = false;
            }
            else if (ddl_Receiver_type.SelectedValue == "2")
            {
                Panel_month.Visible = true;
                Panel_inv_no.Visible = false;
                Panel_inv_date.Visible = false;
                Panel_shipping.Visible = false;
                Panel_inv_type.Visible = true;
                btn_uniform_shoes.Visible = false;

            }
            else if (ddl_Receiver_type.SelectedValue == "3")
            {
                Panel_inv_type.Visible = false;
                Panel_month.Visible = false;
            }
            dispatch_bill_clear();
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    protected void dispatch_resiver()
    {
        if (ddl_Receiver_type.SelectedValue == "1")
        {
            uniform_gridview();
            Panel_month.Visible = false;
            gv_invoice.DataSource = null;
            gv_invoice.DataBind();



        }

        if (ddl_Receiver_type.SelectedValue == "2")
        {
            Panel_month.Visible = true;
            gridview_invoice();
            pod_number.Visible = false;
            sec_pod_number.Visible = false;
            gv_material.DataSource = null;
            gv_material.DataBind();


        }

        if (ddl_Receiver_type.SelectedValue == "3")
        {

            gridview_dublicate_id();

            Panel_month.Visible = false;
            gv_invoice.DataSource = null;
            gv_invoice.DataBind();

            gv_material.DataSource = null;
            gv_material.DataBind();
        }



    }
    //protected void ddl_clientname1_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    dispatch_resiver();
    //    try
    //    {
    //        if (ddl_Receiver_type.SelectedValue == "2")
    //        {
    //            gv_invoice.DataSource = null;
    //            gv_invoice.DataBind();

    //            d6.con.Open();
    //            MySqlDataAdapter cd_invoice = new MySqlDataAdapter("SELECT pay_dispatch_billing.id, client_name, state_dispatch, IFNULL(pay_unit_master.unit_name, pay_dispatch_billing.unit_code) AS 'unit_code', invoice_no,  invoice_date,grand_total, CASE dispatch_by WHEN '0' THEN 'By Curier'  WHEN '1' THEN 'By Postal' WHEN '2' THEN 'By Hand'  ELSE '' END AS dispatch_by, CASE receiver_type  WHEN '2' THEN 'Invoice'  ELSE '' END AS receiver_type , dispatch_date,  receiving_date,  receiver_name_invoice,  pod_no,  shipping_address , stamp_copy,invoice_stamp_copy, month,sec_dispatch_date ,invoice_type FROM  pay_dispatch_billing  LEFT OUTER JOIN pay_unit_master ON pay_dispatch_billing.comp_code = pay_unit_master.comp_code AND pay_dispatch_billing.unit_code = pay_unit_master.unit_code AND pay_dispatch_billing.client_code = pay_unit_master.client_code  INNER JOIN pay_client_master ON pay_dispatch_billing.comp_code = pay_client_master.comp_code AND pay_dispatch_billing.client_code = pay_client_master.client_code    WHERE pay_dispatch_billing.comp_code = '" + Session["comp_code"].ToString() + "' and pay_client_master.client_code = '" + ddl_clientname1.SelectedValue + "' ", d6.con);
    //            DataTable dt_invoice = new DataTable();
    //            cd_invoice.Fill(dt_invoice);
    //            if (dt_invoice.Rows.Count > 0)
    //            {

    //                gv_invoice.DataSource = dt_invoice;
    //                gv_invoice.DataBind();


    //            }

    //        }

    //        if (ddl_Receiver_type.SelectedValue == "1")
    //        {
    //           uniform_gridview();   
    //          //  material_client();

    //        }


    //        if (ddl_Receiver_type.SelectedValue == "3")
    //        {
    //            gv_dublicate_id_card.DataSource = null;
    //            gv_dublicate_id_card.DataBind();

    //            d6.con.Open();
    //            MySqlDataAdapter cd_id = new MySqlDataAdapter("SELECT  pay_id_card_resend.Id,pay_client_master.client_name, ifnull(pay_unit_master.unit_name,pay_id_card_resend.unit_code) as unit_code, ifnull(pay_employee_master.emp_name,pay_id_card_resend.emp_code) as emp_code,id_no_set,pay_id_card_resend.state_name, pay_dispatch_billing.designation,dispatch_date_du,receiving_date_du,du_receiving_name,du_pod1,shipping_address,du_stamp_copy,CASE du_dispatch_by  WHEN '0' THEN 'By Curier'  WHEN '1' THEN 'By Postal'  WHEN '2' THEN 'By Hand'  ELSE '' END AS 'du_dispatch_by',id_no_set,dispatch_date_du2,receiving_date_du2,du_receiving_name2,du_pod_no2,du_stamp_copy2,CASE du_dispatch_by2  WHEN '0' THEN 'By Curier'  WHEN '1' THEN 'By Postal'  WHEN '2' THEN 'By Hand'  ELSE '' END AS 'du_dispatch_by2' FROM pay_id_card_resend  INNER JOIN pay_dispatch_billing ON pay_id_card_resend.comp_code = pay_dispatch_billing.comp_code AND pay_id_card_resend.client_code = pay_dispatch_billing.client_code AND pay_id_card_resend.emp_code = pay_dispatch_billing.emp_code  INNER JOIN pay_client_master ON pay_id_card_resend.comp_code = pay_client_master.comp_code AND pay_id_card_resend.client_code = pay_client_master.client_code  INNER JOIN pay_unit_master ON pay_id_card_resend.comp_code = pay_unit_master.comp_code AND pay_id_card_resend.unit_code = pay_unit_master.unit_code  INNER JOIN pay_employee_master ON pay_id_card_resend.emp_code = pay_employee_master.emp_code WHERE pay_id_card_resend.comp_code = '" + Session["comp_code"].ToString() + "' and pay_client_master.client_code = '" + ddl_clientname1.SelectedValue + "' group by pay_id_card_resend.emp_code  ", d6.con);
    //            DataTable dt_du_id = new DataTable();
    //            cd_id.Fill(dt_du_id);
    //            if (dt_du_id.Rows.Count > 0)
    //            {

    //                gv_dublicate_id_card.DataSource = dt_du_id;
    //                gv_dublicate_id_card.DataBind();


    //            }

    //        }


    //    }

    //    catch (Exception ex) { throw ex; }
    //    finally
    //    {
    //        d6.con.Close();

    //        //material_client();

    //    }

    //}

    //protected void material_client()
    //{
    //    gv_material.DataSource = null;
    //    gv_material.DataBind();
    //    try
    //    {
    //        d5.con.Open();
    //        MySqlDataAdapter cd_invoice = new MySqlDataAdapter("SELECT distinct pay_dispatch_billing.id, client_name,  state_dispatch,  ifnull(pay_unit_master.unit_name,pay_dispatch_billing.unit_code) as unit_code, ifnull(pay_employee_master.emp_name,pay_dispatch_billing.emp_code) as emp_code, pay_dispatch_billing.designation, CASE dispatch_by   WHEN '0' THEN 'By Curier'  WHEN '1' THEN 'By Postal' WHEN '2' THEN 'By Hand' ELSE ''  END AS 'dispatch_by',  CASE receiver_type WHEN '1' THEN 'Material' ELSE '' END AS 'receiver_type',pay_dispatch_billing.dispatch_date, pay_dispatch_billing.receiving_date,receiver_name_invoice, pod_no,pay_dispatch_billing.shipping_address,stamp_copy,sec_dispatch_date,'sec_receiver_name' ,'sec_receiving_date' , pay_dispatch_billing.remaining_no_set,'sec_pod_no',second_stamp_copy ,'uniform_no_flag',invoice_type   FROM pay_dispatch_billing   INNER JOIN pay_client_master ON pay_dispatch_billing.comp_code = pay_client_master.comp_code AND pay_dispatch_billing.client_code = pay_client_master.client_code    left outer join pay_unit_master on pay_dispatch_billing.comp_code = pay_unit_master.comp_code AND pay_dispatch_billing.unit_code = pay_unit_master.unit_code  left outer join pay_employee_master on pay_dispatch_billing.emp_code = pay_employee_master.emp_code INNER JOIN pay_document_details ON pay_dispatch_billing.comp_code = pay_document_details.comp_code AND pay_dispatch_billing.client_code = pay_document_details.client_code WHERE  pay_dispatch_billing.comp_code = '" + Session["comp_code"].ToString() + "' AND receiver_type = '" + ddl_Receiver_type.SelectedValue + "' and pay_dispatch_billing.client_code = '" + ddl_clientname1.SelectedValue + "'   group by state_dispatch ", d5.con);
    //        DataTable dt_invoice = new DataTable();
    //        cd_invoice.Fill(dt_invoice);
    //        if (dt_invoice.Rows.Count > 0)
    //        {

    //            gv_material.DataSource = dt_invoice;
    //            gv_material.DataBind();
    //            //ddl_clientname1.Items.Insert(0, "Select");
    //        }

    //        dispatch_resiver();
    //    }
    //    catch (Exception ex) { throw ex; }
    //    finally { d5.con.Close(); }




    //}
    protected void gv_invoice_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_invoice.UseAccessibleHeader = false;
            gv_invoice.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }
    protected void lnk_uniform_second_Command(object sender, CommandEventArgs e)
    {
        string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
        GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow1.Cells[1].Text;
        string data1 = d.getsinglestring(" select second_stamp_copy from pay_dispatch_billing where id = '" + id + "'  ");
        string filename_second = data1;

        if (filename_second != "")
        {
            downloadfile_second(filename_second);

            dispatch_resiver();
        }

        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Second Attachment File Cannot Be Uploaded !!!')", true);
        }

    }
    protected void second_btn_uniform_shoes_Click(object sender, EventArgs e)
    {
        try
        {
            ReportDocument crystalReport = new ReportDocument();
            System.Data.DataTable dt2 = new System.Data.DataTable();


            MySqlDataAdapter second_uniform = new MySqlDataAdapter(" SELECT client_name AS 'COMP_CODE', IFNULL(pay_unit_master.unit_name, pay_dispatch_billing.unit_code) AS 'CITY', state_dispatch AS 'STATE', IFNULL(pay_employee_master.emp_name, pay_dispatch_billing.emp_code) AS 'ADDRESS1', sec_receiver_name AS 'ADDRESS2', pay_dispatch_billing.designation, (SELECT size FROM pay_document_details WHERE pay_document_details.emp_code = pay_employee_master.emp_code AND comp_code = '" + Session["comp_code"].ToString() + "' AND document_type = 'Uniform' AND year_complete = 0) AS 'UNIT_ADD1', (SELECT size FROM pay_document_details WHERE pay_document_details.emp_code = pay_employee_master.emp_code AND comp_code = pay_unit_master.comp_code AND document_type = 'Shoes' AND year_complete = 0) AS 'UNIT_ADD2', IF(((SELECT ID FROM pay_document_details WHERE pay_document_details.emp_code = pay_employee_master.emp_code AND comp_code = '" + Session["comp_code"].ToString() + "' AND document_type = 'ID_Card' AND year_complete = 0) != ''), 'YES', 'NO') AS 'UNIT_CITY' FROM pay_dispatch_billing INNER JOIN pay_document_details ON pay_dispatch_billing.comp_code = pay_document_details.comp_code AND pay_dispatch_billing.client_code = pay_document_details.client_code AND pay_dispatch_billing.emp_code = pay_document_details.emp_code AND pay_dispatch_billing.unit_code = pay_document_details.unit_code INNER JOIN pay_client_master ON pay_dispatch_billing.comp_code = pay_client_master.comp_code AND pay_dispatch_billing.client_code = pay_client_master.client_code LEFT OUTER JOIN pay_unit_master ON pay_dispatch_billing.unit_code = pay_unit_master.unit_code AND pay_dispatch_billing.comp_code = pay_unit_master.comp_code LEFT OUTER JOIN pay_employee_master ON pay_dispatch_billing.emp_code = pay_employee_master.emp_code AND pay_dispatch_billing.comp_code = pay_employee_master.comp_code  WHERE  pay_client_master.client_code = '" + ddl_client_name.SelectedValue + "' AND pay_unit_master.unit_code = '" + ddl_branch_name.SelectedValue + "'  AND pay_dispatch_billing.emp_code = pay_employee_master.emp_code AND pay_unit_master.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_dispatch_billing.emp_code = '" + ddl_emp_name.SelectedValue + "' AND year_complete = 0 AND dis_year_complete = 0 GROUP BY receiver_name_invoice ", d.con);
            d.con.Open();
            try
            {
                second_uniform.Fill(dt2);


            }
            catch (Exception ex) { throw ex; }
            crystalReport.Load(Server.MapPath("~/second_receiving_copy.rpt"));
            crystalReport.SetDataSource(dt2);
            crystalReport.Refresh();
            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, "second_receiving_copy");
            //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, "TaxInvoice");
        }
        catch (Exception ee)
        {
            throw ee;
        }
        finally
        {
            dispatch_resiver();

        }
    }

    protected void gv_dublicate_id_card_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
        e.Row.Cells[1].Visible = false;
        e.Row.Cells[15].Visible = false;
        e.Row.Cells[16].Visible = false;
        e.Row.Cells[24].Visible = false;




    }
    protected void link_id_dublicate_Click(object sender, EventArgs e)
    {

        try
        {
            d5.con.Open();
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
            string id = grdrow.Cells[1].Text;

            //ViewState["id"] = gv_material.SelectedRow.Cells[1].Text;
            MySqlCommand ds_id_card = new MySqlCommand(" SELECT  pay_id_card_resend . id , pay_id_card_resend . client_code , pay_id_card_resend . state_name ,  pay_id_card_resend . unit_code , pay_id_card_resend . emp_code ,   pay_dispatch_billing . designation , du_receiving_name , receiving_date_du , dispatch_date_du , du_pod1 , shipping_address,du_dispatch_by,pay_id_card_resend.id_no_set  FROM   pay_id_card_resend  INNER JOIN  pay_dispatch_billing  ON  pay_id_card_resend . comp_code  =  pay_dispatch_billing . comp_code  AND  pay_id_card_resend . client_code  =  pay_dispatch_billing . client_code  AND  pay_id_card_resend . emp_code  =  pay_dispatch_billing . emp_code  where pay_id_card_resend.Id = '" + id + "'", d5.con);



            MySqlDataReader dr_id = ds_id_card.ExecuteReader();


            if (dr_id.Read())
            {
                //ddl_Receiver_type.SelectedValue = dr_id.GetValue(11).ToString();
                txt_id.Text = dr_id.GetValue(0).ToString();

                ddl_client_name.SelectedValue = dr_id.GetValue(1).ToString();
                ddl_client_name_SelectedIndexChanged(null, null);
                ddl_state_name.SelectedValue = dr_id.GetValue(2).ToString();
                ddl_state_name_SelectedIndexChanged(null, null);
                ddl_branch_name.SelectedValue = dr_id.GetValue(3).ToString();
                ddl_branch_name_SelectedIndexChanged(null, null);
                ddl_emp_name.SelectedValue = dr_id.GetValue(4).ToString();
                ddl_emp_name_SelectedIndexChanged(null, null);
                ddl_designation.SelectedValue = dr_id.GetValue(5).ToString();
                txt_receiver_name.Text = dr_id.GetValue(6).ToString();
                txt_receiv_date.Text = dr_id.GetValue(7).ToString();
                txt_bill_rtn_date.Text = dr_id.GetValue(8).ToString();
                txt_pod_number.Text = dr_id.GetValue(9).ToString();
                txt_bill_reason.Text = dr_id.GetValue(10).ToString();

                txt_id_set.Text = dr_id.GetValue(12).ToString();

                if (txt_bill_rtn_date.Text != "")
                {
                    ddl_material_contract.SelectedValue = dr_id.GetValue(11).ToString();

                }

            }
            dr_id.Dispose();
            d5.con.Close();

            string original_id_dispatched = d.getsinglestring("select dispatch_date from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch = '" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");

            if (txt_id_set.Text != "0" && original_id_dispatched != "")
            {
                btn_update.Visible = true;
            }
            else if (txt_id_set.Text == "0" && original_id_dispatched == "")
            { btn_update.Visible = false; }


            // validation for dublicate id genration
            if (ddl_material_contract.SelectedValue == "Select") { btn_update.Visible = true; }

            string current_date1 = d.getsinglestring("select field2 from pay_id_card_resend where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name = '" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");

            if (current_date1 == "") { btn_update.Visible = false; }

            // validation for original id card dispatch date not null if dublicate genrate

            string original_dispatch = d.getsinglestring("select dispatch_date from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch = '" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");

            if (original_dispatch == "")
            {


                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('First Dispatch Original Id Card !!!')", true);
                btn_update.Visible = false;
            }

            report_panel.Visible = false;
            btn_dispatch_save.Visible = false;
            btn_bill_save.Visible = false;


            if (txt_bill_rtn_date.Text != "")
            {

                if (ddl_material_contract.SelectedValue == "2")
                {
                    Panel_receiving_date.Visible = true;
                    Panel_receiver_name.Visible = true;
                    report_panel.Visible = true;
                }

                if (ddl_material_contract.SelectedValue == "0")
                {
                    pod_number.Visible = true;
                    report_panel.Visible = true;
                    // Print_Report.Visible = true;

                }

                if (ddl_material_contract.SelectedValue == "1")
                {

                    report_panel.Visible = false;

                }

            }

        }
        catch (Exception ex) { throw ex; }
        finally { d5.con.Close(); }

    }


    // for second dublicate id card
    protected void link_id_dublicate2_Click(object sender, EventArgs e)
    {

        Panel_dis_sec.Visible = true;
        Panel_dispatch.Visible = false;



        //Panel_du_pod.Visible = true;

        try
        {
            d5.con.Open();
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
            string id = grdrow.Cells[1].Text;

            //ViewState["id"] = gv_material.SelectedRow.Cells[1].Text;
            MySqlCommand ds_id_card = new MySqlCommand(" SELECT  pay_id_card_resend . id , pay_id_card_resend . client_code , pay_id_card_resend . state_name ,  pay_id_card_resend . unit_code , pay_id_card_resend . emp_code , pay_dispatch_billing . designation , du_receiving_name2 , receiving_date_du2 , dispatch_date_du2 , du_pod_no2, shipping_address,du_dispatch_by2,pay_id_card_resend.id_no_set  FROM   pay_id_card_resend  INNER JOIN  pay_dispatch_billing  ON  pay_id_card_resend . comp_code  =  pay_dispatch_billing . comp_code  AND  pay_id_card_resend . client_code  =  pay_dispatch_billing . client_code  AND  pay_id_card_resend . emp_code  =  pay_dispatch_billing . emp_code  where pay_id_card_resend.Id = '" + id + "'", d5.con);



            MySqlDataReader dr_id = ds_id_card.ExecuteReader();


            if (dr_id.Read())
            {
                //ddl_Receiver_type.SelectedValue = dr_id.GetValue(11).ToString();
                txt_id.Text = dr_id.GetValue(0).ToString();

                ddl_client_name.SelectedValue = dr_id.GetValue(1).ToString();
                ddl_client_name_SelectedIndexChanged(null, null);
                ddl_state_name.SelectedValue = dr_id.GetValue(2).ToString();
                ddl_state_name_SelectedIndexChanged(null, null);
                ddl_branch_name.SelectedValue = dr_id.GetValue(3).ToString();
                ddl_branch_name_SelectedIndexChanged(null, null);
                ddl_emp_name.SelectedValue = dr_id.GetValue(4).ToString();
                ddl_emp_name_SelectedIndexChanged(null, null);
                ddl_designation.SelectedValue = dr_id.GetValue(5).ToString();
                txt_dub_receiver_name.Text = dr_id.GetValue(6).ToString();
                txt_du_receiver_date.Text = dr_id.GetValue(7).ToString();
                txt_dub_dispatch_date.Text = dr_id.GetValue(8).ToString();
                txt_sec_du_pod.Text = dr_id.GetValue(9).ToString();
                txt_bill_reason.Text = dr_id.GetValue(10).ToString();

                txt_id_set.Text = dr_id.GetValue(12).ToString();

                if (txt_dub_dispatch_date.Text != "")
                {
                    ddl_material_contract.SelectedValue = dr_id.GetValue(11).ToString();

                }

                if (txt_dub_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "0")
                {
                    Panel_du_pod.Visible = true;
                    report_panel.Visible = true;

                }

                if (txt_dub_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "2")
                {
                    Panel_du_receiver_date.Visible = true;
                    Panel_dub_receiver_name.Visible = true;
                    report_panel.Visible = true;

                }

                if (txt_dub_dispatch_date.Text != "" && ddl_material_contract.SelectedValue == "1")
                { report_panel.Visible = true; }


            }
            dr_id.Dispose();
            d5.con.Close();

            // 05-05-2020 komal changes
            string first_du_id_dispatched = d.getsinglestring("select dispatch_date_du from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch = '" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");


            if (txt_id_set.Text != "0" && first_du_id_dispatched != "")
            {

                btn_update.Visible = true;

            }
            else if (txt_id_set.Text == "0" && first_du_id_dispatched == "")
            { btn_update.Visible = false; }

            if (ddl_material_contract.SelectedValue == "Select") { btn_update.Visible = true; }


            string current_date2 = d.getsinglestring("select field3 from pay_id_card_resend where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name = '" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");

            if (current_date2 == "") { btn_update.Visible = false; }


            // validation for original id card not dispatched 


            string original_dispatch1 = d.getsinglestring("select dispatch_date from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch = '" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");

            if (original_dispatch1 == "")
            {


                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('First Dispatch Original Id Card !!!')", true);
                btn_update.Visible = false;
            }


            //report_panel.Visible = false;
            btn_dispatch_save.Visible = false;
            btn_bill_save.Visible = false;


        }
        catch (Exception ex) { throw ex; }
        finally { d5.con.Close(); }

    }


    protected void lnk_dub_id_Command(object sender, CommandEventArgs e)
    {
        string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
        GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow1.Cells[1].Text;


        //  ViewState["id"] = gv_material.SelectedRow.[1].Text;
        string data = d.getsinglestring(" select du_stamp_copy from pay_dispatch_billing where dublicate_id = '" + id + "'  ");
        string filename_du = data;
        //string stamp_copy = commandArgs[1];

        //string filename = e.CommandArgument.ToString();
        ////string unit_name = gv_approve_attendace.SelectedRow.Cells[2].ToString();
        if (filename_du != "")
        {
            downloadfile_dub_id(filename_du);
            //gridview_invoice();
            //uniform_gridview();
            dispatch_resiver();
        }

        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attachment File Cannot Be Uploaded !!!')", true);
        }

    }

    protected void lnk_dub_id2_Command(object sender, CommandEventArgs e)
    {
        string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
        GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow1.Cells[1].Text;


        //  ViewState["id"] = gv_material.SelectedRow.[1].Text;
        string data = d.getsinglestring(" select du_stamp_copy2 from pay_dispatch_billing where dublicate_id = '" + id + "'  ");
        string filename_du2 = data;
        //string stamp_copy = commandArgs[1];

        //string filename = e.CommandArgument.ToString();
        ////string unit_name = gv_approve_attendace.SelectedRow.Cells[2].ToString();
        if (filename_du2 != "")
        {
            downloadfile_dub_id2(filename_du2);
            //gridview_invoice();
            //uniform_gridview();
            dispatch_resiver();
        }

        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attachment File Cannot Be Uploaded !!!')", true);
        }


    }
    protected void link_id_dublicate3_Click(object sender, EventArgs e)
    {



        Panel_dis_third.Visible = true;
        Panel_dispatch.Visible = false;
        Panel_dis_sec.Visible = false;



        //Panel_du_pod.Visible = true;

        try
        {
            d5.con.Open();
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
            string id = grdrow.Cells[1].Text;

            //ViewState["id"] = gv_material.SelectedRow.Cells[1].Text;
            MySqlCommand ds_id_card = new MySqlCommand(" SELECT  pay_id_card_resend . id , pay_id_card_resend . client_code , pay_id_card_resend . state_name ,  pay_id_card_resend . unit_code , pay_id_card_resend . emp_code , pay_dispatch_billing . designation , du_receiving_name3 , receiving_date_du3 , dispatch_date_du3 , du_pod_no3, shipping_address,du_dispatch_by3, pay_id_card_resend.id_no_set  FROM   pay_id_card_resend  INNER JOIN  pay_dispatch_billing  ON  pay_id_card_resend . comp_code  =  pay_dispatch_billing . comp_code  AND  pay_id_card_resend . client_code  =  pay_dispatch_billing . client_code  AND  pay_id_card_resend . emp_code  =  pay_dispatch_billing . emp_code  where pay_id_card_resend.Id = '" + id + "'", d5.con);



            MySqlDataReader dr_id = ds_id_card.ExecuteReader();


            if (dr_id.Read())
            {
                //ddl_Receiver_type.SelectedValue = dr_id.GetValue(11).ToString();
                txt_id.Text = dr_id.GetValue(0).ToString();

                ddl_client_name.SelectedValue = dr_id.GetValue(1).ToString();
                ddl_client_name_SelectedIndexChanged(null, null);
                ddl_state_name.SelectedValue = dr_id.GetValue(2).ToString();
                ddl_state_name_SelectedIndexChanged(null, null);
                ddl_branch_name.SelectedValue = dr_id.GetValue(3).ToString();
                ddl_branch_name_SelectedIndexChanged(null, null);
                ddl_emp_name.SelectedValue = dr_id.GetValue(4).ToString();
                ddl_emp_name_SelectedIndexChanged(null, null);
                ddl_designation.SelectedValue = dr_id.GetValue(5).ToString();
                txt_dub_receiver_name_third.Text = dr_id.GetValue(6).ToString();
                txt_du_receiver_date_third.Text = dr_id.GetValue(7).ToString();
                txt_dub_dispatch_date3.Text = dr_id.GetValue(8).ToString();
                txt_third_du_pod.Text = dr_id.GetValue(9).ToString();
                txt_bill_reason.Text = dr_id.GetValue(10).ToString();

                txt_id_set.Text = dr_id.GetValue(12).ToString();

                if (txt_dub_dispatch_date3.Text != "")
                {
                    ddl_material_contract.SelectedValue = dr_id.GetValue(11).ToString();

                }




            }
            dr_id.Dispose();
            d5.con.Close();

            // 05-05-2020 komal changes
            string second_du_id_dispatched = d.getsinglestring("select dispatch_date_du2 from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch = '" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");

            if (txt_dub_dispatch_date3.Text != "" && ddl_material_contract.SelectedValue == "0")
            {
                Panel_du_pod_3.Visible = true;
                report_panel.Visible = true;

            }

            if (txt_dub_dispatch_date3.Text != "" && ddl_material_contract.SelectedValue == "2")
            {
                Panel_du_receiver_date_3.Visible = true;
                Panel_dub_receiver_name3.Visible = true;
                report_panel.Visible = true;

            }

            if (second_du_id_dispatched != "" && ddl_material_contract.SelectedValue == "1")
            { report_panel.Visible = true; }

            if (txt_id_set.Text != "0" && second_du_id_dispatched != "")
            {

                btn_update.Visible = true;

            }
            else if (txt_id_set.Text == "0" && txt_dub_dispatch_date3.Text == "")
            { btn_update.Visible = false; }

            if (ddl_material_contract.SelectedValue == "Select") { btn_update.Visible = true; }


            string current_date3 = d.getsinglestring("select field4 from pay_id_card_resend where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_name = '" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");

            if (current_date3 == "") { btn_update.Visible = false; }


            // validation for original id card not dispatched 


            string original_dispatch2 = d.getsinglestring("select dispatch_date from pay_dispatch_billing where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client_name.SelectedValue + "' and unit_code = '" + ddl_branch_name.SelectedValue + "' and state_dispatch = '" + ddl_state_name.SelectedValue + "' and emp_code = '" + ddl_emp_name.SelectedValue + "'");

            if (original_dispatch2 == "")
            {


                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('First Dispatch Original Id Card !!!')", true);
                btn_update.Visible = false;
            }


            //report_panel.Visible = false;
            btn_dispatch_save.Visible = false;
            btn_bill_save.Visible = false;


        }
        catch (Exception ex) { throw ex; }
        finally { d5.con.Close(); }
    }
    protected void lnk_dub_id3_Command(object sender, CommandEventArgs e)
    {
        string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
        GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow1.Cells[1].Text;


        //  ViewState["id"] = gv_material.SelectedRow.[1].Text;
        string data = d.getsinglestring(" select du_stamp_copy3 from pay_dispatch_billing where dublicate_id = '" + id + "'  ");
        string filename_du3 = data;
        //string stamp_copy = commandArgs[1];

        //string filename = e.CommandArgument.ToString();
        ////string unit_name = gv_approve_attendace.SelectedRow.Cells[2].ToString();
        if (filename_du3 != "")
        {
            downloadfile_dub_id2(filename_du3);
            //gridview_invoice();
            //uniform_gridview();
            dispatch_resiver();
        }

        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attachment File Cannot Be Uploaded !!!')", true);
        }

    }

    protected void region()
    {
        if (ddl_client_name.SelectedValue != "Select")
        {
            ddlregion.Items.Clear();
            System.Data.DataTable dt_item2 = new System.Data.DataTable();

            MySqlDataAdapter cmd_item2 = new MySqlDataAdapter("SELECT DISTINCT  pay_zone_master.region FROM pay_client_billing_details INNER JOIN pay_zone_master  ON  pay_client_billing_details . comp_code  =  pay_zone_master . comp_code  AND  pay_client_billing_details . client_code  =  pay_zone_master . client_code  WHERE  pay_client_billing_details . client_code  = '" + ddl_client_name.SelectedValue + "' and type = 'Region' ", d.con);
            //MySqlDataAdapter cmd_item1 = new MySqlDataAdapter(" Select DISTINCT region from pay_zone_master where comp_code ='" + Session["comp_code"].ToString() + "' and client_code='" + ddl_client.SelectedValue + "' ", d.con);

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
                //  ddl_state_SelectedIndexChanged(null, null);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }

        }
    }

    protected void ddl_region_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlregion.SelectedValue != "Select")
        {
            ddl_billing_state.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = null;
            if (ddlregion.SelectedValue != "ALL")
            {
                cmd_item = new MySqlDataAdapter("SELECT DISTINCT (STATE_NAME) as state FROM pay_unit_master WHERE comp_code = '" + Session["comp_code"] + "' AND client_code = '" + ddl_client_name.SelectedValue + "' AND ZONE = '" + ddlregion.SelectedValue + "' ORDER BY 1", d.con);
            }
            else
            {
                cmd_item = new MySqlDataAdapter("SELECT DISTINCT (STATE_NAME) as state FROM pay_unit_master WHERE comp_code = '" + Session["comp_code"] + "' AND client_code = '" + ddl_client_name.SelectedValue + "' ORDER BY 1", d.con);
            }
            d.con.Open();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_state_name.DataSource = dt_item;
                    ddl_state_name.DataTextField = dt_item.Columns[0].ToString();
                    ddl_state_name.DataValueField = dt_item.Columns[0].ToString();
                    ddl_state_name.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_state_name.Items.Insert(0, "Select");
                ddl_state_name.Items.Insert(1, "ALL");
                ddl_branch_name.Items.Clear();
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

    //uniform new tab
    protected void ddl_uniform_emp_type_SelectedIndexChanged1(object sender, EventArgs e)
    {

    }
    protected void ddl_uniform_entry_client_SelectedIndexChanged(object sender, EventArgs e)
    {

        hidtab.Value = "5";
        try
        {

            if (ddl_uniform_entry_client.SelectedValue != "Select")
            {
                ddl_uniform_state_id.Items.Clear();
                ddl_uniform_branch.Items.Clear();
                ddl_uniform_emp_name.Items.Clear();
                System.Data.DataTable dt_item = new System.Data.DataTable();
                MySqlDataAdapter cmd_item = new MySqlDataAdapter("select distinct(STATE_NAME) from pay_client_state_role_grade where comp_code='" + Session["COMP_CODE"].ToString() + "' and client_code = '" + ddl_uniform_entry_client.SelectedValue + "' and EMP_CODE in (" + Session["REPORTING_EMP_SERIES"].ToString() + ") ", d.con);
                d.con.Open();
                try
                {
                    cmd_item.Fill(dt_item);
                    if (dt_item.Rows.Count > 0)
                    {
                        ddl_uniform_state_id.DataSource = dt_item;
                        ddl_uniform_state_id.DataTextField = dt_item.Columns[0].ToString();
                        ddl_uniform_state_id.DataValueField = dt_item.Columns[0].ToString();
                        ddl_uniform_state_id.DataBind();
                    }
                    dt_item.Dispose();
                    d.con.Close();
                    ddl_uniform_state_id.Items.Insert(0, "Select");
                    ddl_uniform_state_id.Items.Insert(1, "ALL");
                    ddl_uniform_branch.Items.Insert(0, "Select");
                    ddl_uniform_emp_name.Items.Insert(0, "Select");
                }
                catch (Exception ex) { throw ex; }
                finally
                {
                    d.con.Close();

                }
            }

        }
        catch (Exception)
        {

            throw;
        }

    }
    protected void ddl_uniform_state_id_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_uniform_state_id.SelectedValue != "Select")
        {
            ddl_uniform_branch.Items.Clear();
            ddl_uniform_emp_name.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = null;
            if (ddl_uniform_entry_client.SelectedValue == "DHFL" || (ddl_uniform_entry_client.SelectedValue == "BAGIC" && ddl_uniform_state_id.SelectedValue == "Maharashtra"))
            {
                if (ddl_uniform_state_id.SelectedValue != "ALL")
                {
                    cmd_item = new MySqlDataAdapter("Select UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_uniform_entry_client.SelectedValue + "' and state_name='" + ddl_uniform_state_id.SelectedValue + "'  ORDER BY 1", d.con);
                }
                else
                {
                    cmd_item = new MySqlDataAdapter("Select UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_uniform_entry_client.SelectedValue + "'  ORDER BY 1", d.con);
                }
            }
            else
            {

                if (ddl_uniform_state_id.SelectedValue != "ALL")
                {
                    cmd_item = new MySqlDataAdapter("Select UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_uniform_entry_client.SelectedValue + "' and state_name='" + ddl_uniform_state_id.SelectedValue + "'  ORDER BY 1", d.con);
                }
                else
                {
                    cmd_item = new MySqlDataAdapter("SELECT UNIT_NAME,  unit_code  FROM  pay_unit_master  WHERE  comp_code  = '" + Session["comp_code"] + "' AND  client_code  = '" + ddl_uniform_entry_client.SelectedValue + "' ORDER BY 1", d.con);
                }
            }
            d.con.Open();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_uniform_branch.DataSource = dt_item;
                    ddl_uniform_branch.DataTextField = dt_item.Columns[0].ToString();
                    ddl_uniform_branch.DataValueField = dt_item.Columns[1].ToString();
                    ddl_uniform_branch.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_uniform_branch.Items.Insert(0, "Select");
                ddl_uniform_branch.Items.Insert(1, "ALL");
                ddl_uniform_emp_name.Items.Insert(0, "Select");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }

        }

    }
    protected void ddl_uniform_branch_SelectedIndexChanged(object sender, EventArgs e)
    {

        try
        {

            if (ddl_uniform_branch.SelectedValue != "Select" && ddl_uniform_emp_type.SelectedValue != "0")
            {
                string emp_type = " and Employee_type='Permanent' ", where = "";

                //if (ddl_uniform_emp_type.SelectedValue == "2")
                //{
                //    emp_type = "and LEFT_DATE is null and Employee_type='Permanent'";
                //}
                //else
                //{
                //    emp_type = "and LEFT_DATE is not null and Employee_type='Permanent'";
                //}
                if (ddl_uniform_branch.SelectedValue != "Select" && ddl_uniform_branch.SelectedValue != "ALL" && ddl_uniform_branch.SelectedValue != "")
                {
                    where = " and UNIT_CODE = '" + ddl_uniform_branch.SelectedValue + "' " + emp_type + " ";
                }

                ddl_uniform_emp_name.Items.Clear();
                System.Data.DataTable dt_item = new System.Data.DataTable();
                MySqlDataAdapter cmd_item = new MySqlDataAdapter("select distinct(EMP_CODE),EMP_NAME from pay_employee_master where COMP_CODE='" + Session["comp_code"].ToString() + "'and client_code='" + ddl_uniform_entry_client.SelectedValue + "' " + where + " ", d.con);
                d.con.Open();
                try
                {
                    cmd_item.Fill(dt_item);
                    if (dt_item.Rows.Count > 0)
                    {
                        ddl_uniform_emp_name.DataSource = dt_item;
                        ddl_uniform_emp_name.DataTextField = dt_item.Columns[1].ToString();
                        ddl_uniform_emp_name.DataValueField = dt_item.Columns[0].ToString();
                        ddl_uniform_emp_name.DataBind();
                    }
                    dt_item.Dispose();
                    d.con.Close();
                    ddl_uniform_emp_name.Items.Insert(0, "Select");
                    ddl_uniform_emp_name.Items.Insert(1, "ALL");
                }
                catch (Exception ex) { throw ex; }
                finally
                {
                    d.con.Close();

                }
            }

        }
        catch (Exception)
        {

            throw;
        }
    }
    protected void ddl_uniform_emp_name_SelectedIndexChanged(object sender, EventArgs e)
    {
        gv_uniform_entry_bind();

    }
    //END

    //Start
    protected void gv_uniform_entry_bind()
    {

        hidtab.Value = "5";
        try
        {
            gv_uniform_entry.DataSource = null;
            gv_uniform_entry.DataBind();
            d.con.Open();

            string where = "", emp_type = "";

            if (ddl_uniform_emp_type.SelectedValue == "")
            {
                emp_type = "and employee_type='" + ddl_uniform_emp_type.SelectedItem.Text + "'";
            }
            else
            {
                emp_type = "and employee_type='" + ddl_uniform_emp_type.SelectedItem.Text + "'";
            }

            if (ddl_uniform_entry_client.SelectedValue != "Select")
            {
                where = "and pay_dispatch_billing.CLIENT_CODE='" + ddl_uniform_entry_client.SelectedValue + "'";

                if (ddl_uniform_state_id.SelectedValue != "Select" && ddl_uniform_state_id.SelectedValue != "ALL")
                {
                    where = "and pay_dispatch_billing.CLIENT_CODE='" + ddl_uniform_entry_client.SelectedValue + "'and state_dispatch='" + ddl_uniform_state_id.SelectedValue + "'";

                    if (ddl_uniform_branch.SelectedValue != "Select" && ddl_uniform_branch.SelectedValue != "ALL" && ddl_uniform_branch.SelectedValue != "")
                    {
                        where = "and pay_dispatch_billing.CLIENT_CODE='" + ddl_uniform_entry_client.SelectedValue + "'and state_dispatch='" + ddl_uniform_state_id.SelectedValue + "' and pay_dispatch_billing.unit_code='" + ddl_uniform_branch.SelectedValue + "'";

                        if (ddl_uniform_emp_name.SelectedValue != "Select" && ddl_uniform_emp_name.SelectedValue != "ALL" && ddl_uniform_emp_name.SelectedValue != "")
                        {
                            where = "and pay_dispatch_billing.CLIENT_CODE='" + ddl_uniform_entry_client.SelectedValue + "'and state_dispatch='" + ddl_uniform_state_id.SelectedValue + "' and pay_dispatch_billing.unit_code='" + ddl_uniform_branch.SelectedValue + "' and pay_dispatch_billing.emp_code='" + ddl_uniform_emp_name.SelectedValue + "'";
                        }
                    }
                }
            }

            // MySqlDataAdapter adp_grid = new MySqlDataAdapter("SELECT id, comp_code, client_code, client_name, state_name, unit_code, branch_name, emp_code, emp_name, employee_type, dispatch_by, DATE_FORMAT(dispatch_date, '%d/%m/%Y') AS 'dispatch_date',DATE_FORMAT(delivery_date, '%d/%m/%Y') AS 'delivery_date', No_of_set, uploaded_by, uploaded_date, Attatchment, Pod_no, Remark FROM uniform_entry_details where comp_code='" + Session["comp_code"].ToString() + "'" + emp_type + " " + where + "", d.con);
            MySqlDataAdapter adp_grid = new MySqlDataAdapter("SELECT pay_dispatch_billing.employee_type, pay_dispatch_billing.No_of_set,pay_dispatch_billing.Remark,pay_document_details.uniform_no_flag, ifnull((SELECT  remaining_no_set FROM pay_document_details WHERE pay_document_details.emp_code = pay_dispatch_billing.emp_code AND remaining_no_set != 0 group by pay_dispatch_billing.emp_code) ,0) AS 'remaining_no_set',pay_dispatch_billing.id, client_name,  state_dispatch,  ifnull(pay_unit_master.unit_name,pay_dispatch_billing.unit_code) as unit_code, ifnull(pay_employee_master.emp_name,pay_dispatch_billing.emp_code) as emp_code, IFNULL(pay_unit_master.unit_code, pay_dispatch_billing.unit_code) AS unit_code1, IFNULL(pay_employee_master.emp_code, pay_dispatch_billing.emp_code) AS emp_code1, pay_dispatch_billing.designation, CASE dispatch_by   WHEN '0' THEN 'By Curier'  WHEN '1' THEN 'By Postal' WHEN '2' THEN 'By Hand' ELSE ''  END AS 'dispatch_by',  CASE receiver_type WHEN '1' THEN 'Material' ELSE '' END AS 'receiver_type',pay_dispatch_billing.dispatch_date AS 'dispatch_date' , pay_dispatch_billing.receiving_date,receiver_name_invoice, pod_no,pay_dispatch_billing.shipping_address,stamp_copy,sec_pod_no,sec_dispatch_date,remaining_uniform,sec_receiver_name,sec_receiving_date,second_stamp_copy,pay_dispatch_billing.emp_code FROM pay_dispatch_billing  inner JOIN pay_document_details ON pay_dispatch_billing.comp_code = pay_document_details.comp_code  AND pay_dispatch_billing.emp_code = pay_document_details.emp_code INNER JOIN pay_client_master ON pay_dispatch_billing.comp_code = pay_client_master.comp_code AND pay_dispatch_billing.client_code = pay_client_master.client_code    left outer join pay_unit_master on pay_dispatch_billing.comp_code = pay_unit_master.comp_code AND pay_dispatch_billing.unit_code = pay_unit_master.unit_code  left outer join pay_employee_master on pay_dispatch_billing.emp_code = pay_employee_master.emp_code WHERE  pay_dispatch_billing.comp_code = '" + Session["comp_code"].ToString() + "' " + where + " and ( year_complete = 0 and dis_year_complete = 0) group by pay_dispatch_billing.emp_code", d.con);

            DataSet ds = new DataSet();
            adp_grid.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                gv_uniform_entry.DataSource = ds;
                gv_uniform_entry.DataBind();
                d.con.Close();
                div_gv_uniform.Visible = true;
            }
            else
            {
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Matching Records Found.');", true);
                div_gv_uniform.Visible = false;
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
    protected void gv_uniform_entry_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_uniform_entry.UseAccessibleHeader = false;
            gv_uniform_entry.HeaderRow.TableSection = TableRowSection.TableHeader;
            div_gv_uniform.Visible = gv_uniform_entry.Rows.Count > 0;
        }
        catch { }
    }
    protected void gv_uniform_entry_RowDataBound(object sender, GridViewRowEventArgs e)
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
            if (dr["second_stamp_copy"].ToString() == "")
            {
                LinkButton edit = e.Row.FindControl("uniform_entry_download") as LinkButton;
                edit.Visible = false;

            }
            else
            {
                LinkButton edit = e.Row.FindControl("uniform_entry_download") as LinkButton;
                edit.Visible = true;
            }
        }

        e.Row.Cells[0].Visible = false;
    }
    protected void uniform_submit_btn_Click(object sender, EventArgs e)
    {
        int k = 0;
        hidtab.Value = "5";
        try
        {
            string fileName = "", file = "", ids = "", date = "", attachment_file = "", attachment_file_name = "", update_attachment_file = "", same_date_pod_no = "", not_same_date_pod_no = "", pod_no = "";
            ids = d.getsinglestring("select max(Id) as 'id' from pay_dispatch_billing");
            if (ids == "")
            {
                ids = "1";
            }
            else
            {

            }
            date = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");
            List<string> temp_list = new List<string>();
            try
            {


                same_date_pod_no = d.getsinglestring("select pod_no from pay_dispatch_billing where client_code='" + ddl_uniform_entry_client.SelectedValue + "' and unit_code='" + ddl_uniform_branch.SelectedValue + "' and Pod_no='" + pod_uniform_no.Text + "' and dispatch_date=str_to_date('" + Disp_uniform_date.Text + "','%d/%m/%Y')");
                not_same_date_pod_no = d.getsinglestring("select pod_no from pay_dispatch_billing where client_code!='" + ddl_uniform_entry_client.SelectedValue + "' and unit_code!='" + ddl_uniform_branch.SelectedValue + "' and Pod_no='" + pod_uniform_no.Text + "' and dispatch_date!=str_to_date('" + Disp_uniform_date.Text + "','%d/%m/%Y')");
                pod_no = d.getsinglestring("select pod_no from pay_dispatch_billing where Pod_no='" + pod_uniform_no.Text + "'");

            }
            catch { }

            if (same_date_pod_no != "" || not_same_date_pod_no == "" || pod_no == "")
            {
                #region
                if (FileUpload_uniform_entry.HasFile)
                {
                    try
                    {

                        string fileExt = System.IO.Path.GetExtension(FileUpload_uniform_entry.FileName);
                        fileName = Path.GetFileName(FileUpload_uniform_entry.PostedFile.FileName);
                        FileUpload_uniform_entry.PostedFile.SaveAs(Server.MapPath("~/approved_attendance_images/") + fileName);
                        file = ids + "_" + pod_uniform_no.Text + "_" + ddl_uniform_emp_name.SelectedValue + "_" + date + fileExt;
                        File.Copy(Server.MapPath("~/approved_attendance_images/") + fileName, Server.MapPath("~/approved_attendance_images/") + file, true);
                        File.Delete(Server.MapPath("~/approved_attendance_images/") + fileName);
                        string uploaded_file = Path.GetFullPath(Server.MapPath("~/approved_attendance_images/") + file);
                        if (file != "")
                        {
                            attachment_file = ", second_stamp_copy";
                            attachment_file_name = ",'" + file + "'";
                            update_attachment_file = "second_stamp_copy='" + file + "',";

                        }
                    }
                    catch { }
                }

                if (uniform_submit_btn.Text == "Submit")
                {
                    try
                    {

                        k = d.operation("Insert pay_dispatch_billing(comp_code, client_code, state_dispatch,unit_code, emp_code, employee_type, dispatch_by, dispatch_date, receiving_date " + attachment_file + ",pod_no, No_of_set, Remark, uploaded_by, uploaded_date)values('" + Session["comp_code"].ToString() + "','" + ddl_uniform_entry_client.SelectedValue + "','" + ddl_uniform_state_id.SelectedItem + "','" + ddl_uniform_branch.SelectedValue + "','" + ddl_uniform_emp_name.SelectedValue + "','" + ddl_uniform_emp_type.SelectedItem + "','" + ddl_dispatch_d1.SelectedValue + "',str_to_date('" + Disp_uniform_date.Text + "','%d/%m/%Y'),str_to_date('" + deliver_uniform_dt.Text + "','%d/%m/%Y') " + attachment_file_name + ",'" + pod_uniform_no.Text.Trim() + "','" + set_uniform_no.Text.Trim() + "','" + uniform_remark.Text + "','" + Session["LOGIN_ID"].ToString() + "',now())");

                        if (k > 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record inserted successfully..!!')", true);
                            gv_uniform_entry_bind();
                            clr_txt();
                            return;
                        }
                    }
                    catch { }
                }
                else if (uniform_submit_btn.Text == "Update")
                {
                    try
                    {
                        string id = "";
                        id = ViewState["uniform_id"].ToString();
                        int u = 0;

                        u = d.operation("update pay_dispatch_billing set " + update_attachment_file + " dispatch_by='" + ddl_dispatch_d1.SelectedValue + "', dispatch_date=str_to_date('" + Disp_uniform_date.Text + "','%d/%m/%Y'), receiving_date=str_to_date('" + deliver_uniform_dt.Text + "','%d/%m/%Y'), No_of_set='" + set_uniform_no.Text.Trim() + "',pod_no ='" + pod_uniform_no.Text.Trim() + "',Remark ='" + uniform_remark.Text + "' where id='" + id + "'");//str_to_date('" + Disp_uniform_date.Text + "','%d/%m/%Y'), delivery_date=str_to_date('" + deliver_uniform_dt.Text + "','%d/%m/%Y'),

                        if (u > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record updated successfully ...!!!')", true);
                            gv_uniform_entry_bind();
                            clr_txt();
                            return;
                        }

                    }
                    catch
                    { }
                }
                #endregion
            }
            else
            {

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Same POD no is already exists...!!!')", true);
                return;

            }

        }
        catch { }
    }
    protected void uniform_entry_download_Command(object sender, CommandEventArgs e)
    {
        string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
        GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow1.Cells[0].Text;
        string data = d.getsinglestring(" select second_stamp_copy from pay_dispatch_billing where id = '" + id + "'  ");
        string filename = data;
        if (filename != "")
        {
            downloadfile_uniform(filename);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attachment file not found !!!')", true);
        }
    }
    protected void uniform_entry_edit_Click(object sender, EventArgs e)
    {
        uniform_submit_btn.Text = "Update";
        string id = "";

        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        id = grdrow.Cells[0].Text;
        ViewState["uniform_id"] = id;

        MySqlCommand cmd2 = new MySqlCommand("SELECT Id, comp_code, client_code, unit_code, state_dispatch, dispatch_by, invoice_no,date_format(dispatch_date,'%d/%m/%Y') as 'dispatch_date',  date_format(receiving_date,'%d/%m/%Y') as 'receiving_date', pod_no, shipping_address, invoice_name, invoice_date, receiver_name_invoice, grand_total, emp_code, designation, stamp_copy, second_stamp_copy, invoice_stamp_copy, Dispatched, receiver_type, month, sec_pod_no, sec_dispatch_date, remaining_uniform, sec_receiver_name, sec_receiving_date, remaining_no_set, uniform_no_flag, invoice_type, dublicate_id_card, dispatch_date_du, first_dublicate_card, receiving_date_du, du_pod1, du_stamp_copy_first, du_receiving_name, du_stamp_copy, du_dispatch_by, dublicate_id, dispatch_date_du2, receiving_date_du2, du_receiving_name2, du_pod_no2, du_stamp_copy2, du_dispatch_by2, receiver_type_du, dispatch_date_du3, receiving_date_du3, du_receiving_name3, du_pod_no3, du_stamp_copy3, du_dispatch_by3, dis_year_complete, region_name, employee_type, no_of_set, Remark, uploaded_by, uploaded_date FROM pay_dispatch_billing where Id='" + id + "'", d.con);

        d.con.Open();
        try
        {
            MySqlDataReader dr2 = cmd2.ExecuteReader();
            if (dr2.Read())
            {
                ddl_uniform_emp_type.SelectedItem.Text = dr2["employee_type"].ToString();
                ddl_uniform_entry_client.SelectedValue = dr2["client_code"].ToString();
                ddl_uniform_state_id.SelectedValue = dr2["state_dispatch"].ToString();
                ddl_uniform_branch.SelectedValue = dr2["unit_code"].ToString();
                ddl_uniform_emp_name.SelectedValue = dr2["emp_code"].ToString();
                ddl_dispatch_d1.SelectedValue = dr2["dispatch_by"].ToString();
                Disp_uniform_date.Text = dr2["dispatch_date"].ToString();
                deliver_uniform_dt.Text = dr2["receiving_date"].ToString();
                set_uniform_no.Text = dr2["No_of_set"].ToString();
                pod_uniform_no.Text = dr2["pod_no"].ToString();
                uniform_remark.Text = dr2["Remark"].ToString();
            }
            dr2.Dispose();
        }
        catch { }
    }
    protected void downloadfile_uniform(string filename)
    {

        try
        {
            foreach (GridViewRow gvrow in gv_uniform_entry.Rows)
            {
                LinkButton download = gvrow.FindControl("uniform_entry_download") as LinkButton;
                ScriptManager.GetCurrent(this).RegisterPostBackControl(download);
                string path = Server.MapPath("~\\approved_attendance_images\\" + filename);
                bool code = File.Exists(path);
                if (code == true)
                {
                    Response.Clear();
                    Response.ContentType = "Application/pdf/jpg/jpeg/png/zip";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(path));
                    Response.TransmitFile("~\\approved_attendance_images\\" + filename);
                    Response.WriteFile(path);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    Response.End();
                    Response.Close();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('File not Found!!.');", true);
                }
            }
        }
        catch (Exception ex) { throw ex; }
    }
    protected void clr_txt()
    {
        uniform_submit_btn.Text = "Submit";

        //ddl_uniform_emp_type.SelectedValue = "0";
        //ddl_uniform_entry_client.SelectedValue = "Select";
        //ddl_uniform_state_id.SelectedValue = "Select";
        //ddl_uniform_branch.SelectedValue = "Select";
        //ddl_uniform_emp_name.SelectedValue = "Select";
        ddl_dispatch_d1.SelectedValue = "Select";
        Disp_uniform_date.Text = "";
        deliver_uniform_dt.Text = "";
        pod_uniform_no.Text = "";
        set_uniform_no.Text = "";
        uniform_remark.Text = "";
    }

    protected void btn_pod_detail_Click(object sender, EventArgs e)
    {

        try
        {
            string where = "";

            if (ddl_uniform_entry_client.SelectedValue != "Select" && ddl_uniform_entry_client.SelectedValue != "ALL")
            {
                where = " and b.client_code ='" + ddl_uniform_entry_client.SelectedValue + "'";

                if (ddl_uniform_state_id.SelectedValue != "Select" && ddl_uniform_state_id.SelectedValue != "ALL")
                {
                    where = "and b.client_code ='" + ddl_uniform_entry_client.SelectedValue + "' and state_dispatch ='" + ddl_uniform_state_id.SelectedItem + "' ";

                    if (ddl_uniform_branch.SelectedValue != "Select" && ddl_uniform_branch.SelectedValue != "ALL")
                    {
                        where = "and b.client_code ='" + ddl_uniform_entry_client.SelectedValue + "' and state_dispatch ='" + ddl_uniform_state_id.SelectedItem + "'  and b.unit_code='" + ddl_uniform_branch.SelectedValue + "'";

                        if (ddl_uniform_branch.SelectedValue != "Select" && ddl_uniform_branch.SelectedValue != "ALL")
                        {
                            where = "and b.client_code ='" + ddl_uniform_entry_client.SelectedValue + "' and state_dispatch ='" + ddl_uniform_state_id.SelectedItem + "'  and b.unit_code='" + ddl_uniform_branch.SelectedValue + "' and b.emp_code ='" + ddl_uniform_emp_name.SelectedValue + "'";
                        }
                    }

                }
            }

            string sql = "SELECT c.CLIENT_NAME, b.client_code, no_of_set, case when dispatch_by=0  then 'By Courier' when dispatch_by=1 then 'By Postal' when dispatch_by=2 then 'By Hand' end as 'dispatch_by', state_dispatch, u.UNIT_NAME, b.unit_code, remark, pod_no, dispatch_date, receiving_date,  e.EMP_NAME,  b.employee_type  FROM    pay_dispatch_billing b        INNER JOIN    pay_client_master c ON b.COMP_CODE = c.COMP_CODE        AND b.CLIENT_CODE = c.CLIENT_CODE        INNER JOIN    pay_unit_master u ON b.COMP_CODE = u.COMP_CODE        AND b.CLIENT_CODE = u.CLIENT_CODE        AND b.unit_code = u.unit_code         INNER JOIN    pay_employee_master e ON b.COMP_CODE = e.COMP_CODE        AND b.CLIENT_CODE = e.CLIENT_CODE        AND b.unit_code = e.unit_code        AND b.emp_code = e.emp_code  where b.COMP_CODE='" + Session["COMP_CODE"].ToString() + "' " + where + "";

            DataSet ds = new DataSet();
            MySqlDataAdapter dscmd = new MySqlDataAdapter(sql, d.con);
            dscmd.SelectCommand.CommandTimeout = 200;
            dscmd.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=POD Details " + ddl_uniform_entry_client.SelectedItem.Text.Replace(" ", "_").Replace(",", "_").Replace(".", "_") + ".xls");
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate_Pod_No(ListItemType.Header, ds);
                Repeater1.ItemTemplate = new MyTemplate_Pod_No(ListItemType.Item, ds);
                Repeater1.FooterTemplate = new MyTemplate_Pod_No(ListItemType.Footer, ds);
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
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Matching Record Found.');", true);

            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('File Not Download...!!');", true);
            throw ex;
        }
        finally
        {
            d.con.Close();

        }

    }

    public class MyTemplate_Pod_No : ITemplate
    {
        ListItemType type;
        LiteralControl lc;
        DataSet ds;
        static int ctr;

        public MyTemplate_Pod_No(ListItemType type, DataSet ds)
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
                    lc = new LiteralControl("<table border=1 width=100%><tr><th bgcolor=yellow colspan=12>POD DETAILS</th></tr><tr><th>SR NO.</th><th>CLIENT NAME</th><th>STATE NAME</th><th>BRANCH NAME</th><th>EMPLOYEE TYPE</th><th>EMPLOYEE NAME</th><th>DISPATCH BY</th><th>DISPATCH DATE</th><th>DELIVERY DATE</th><th>POD NO</th><th>NO OF SET</th><th>REMARK</th></tr>");
                    break;
                case ListItemType.Item:
                    lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["CLIENT_NAME"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["state_dispatch"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["UNIT_NAME"] + "</td><td>" + ds.Tables[0].Rows[ctr]["employee_type"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["EMP_NAME"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["dispatch_by"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["dispatch_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["receiving_date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["pod_no"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["no_of_set"].ToString() + "</td><td>" + ds.Tables[0].Rows[ctr]["remark"].ToString() + "</td></tr>");
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