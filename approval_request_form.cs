using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class approval_request_form : System.Web.UI.Page
{
    DAL d = new DAL();

    protected void Page_Load(object sender, EventArgs e)
    {

        // btns visibility for dispatch tab 06-02-2021 komal
        for_material_gv_search.Visible = false;
        for_dublicate_gv_search.Visible = false;
        for_invoice_gv_search.Visible = false;
        dispatch_date_panel.Visible = false;
        upload_data.Visible = false;

        //btn_approve_for_material.Visible = false;
        //btn_hold_material.Visible = false;
        //btn_reject_material.Visible = false;
        //btn_download_report.Visible = false;

        btn_visibility.Visible = false;

        // btns visibility for dispatch tab 06-02-2021 komal end
    }
  

    protected void client_attendace() 
    {
       

        System.Data.DataTable dt_id_gv = new System.Data.DataTable();
        MySqlDataAdapter cmd_id_gv = new MySqlDataAdapter("select id,date_format(attendance_date,'%d/%m/%Y') as 'attendance_date',date_format(approval_date_on,'%d/%m/%Y') as 'approval_date_on',comment_box ,case when client_request_for = '1' then 'Attendance' when client_request_for= '2' then 'Leave' end as 'client_request_for', case when status = '0' then 'Pending' when status = '1' then 'Approve' when status = '3' then 'Rejected' end as 'status',emp_code,emp_name,reject_reason as 'reject_reason' from pay_client_request_form where comp_code = '" + Session["comp_code"].ToString() + "' and client_request_for = '" + ddl_client_request.SelectedValue + "' ", d.con);


        cmd_id_gv.Fill(dt_id_gv);
        //appro_emp_legal = "0";
        if (dt_id_gv.Rows.Count > 0)
        {
            //ViewState["appro_emp_legal"] = dt_id_gv.Rows.Count.ToString();
            //appro_emp_legal = ViewState["appro_emp_legal"].ToString();

            gv_attendance.DataSource = dt_id_gv;
            gv_attendance.DataBind();


        }
        dt_id_gv.Dispose();
    
    
    }


    protected void client_leave()
    {
        System.Data.DataTable dt_id_gv = new System.Data.DataTable();
        MySqlDataAdapter cmd_id_gv = new MySqlDataAdapter("select id,date_format(approval_date_on,'%d/%m/%Y') as 'approval_date_on',case when client_request_for = '1' then 'Attendance' when client_request_for= '2' then 'Leave' end as 'client_request_for', date_format(leave_from_date,'%d/%m/%Y') as 'leave_from_date',date_format(leave_to_date,'%d/%m/%Y') as 'leave_to_date',comment_box,case when status = '0' then 'Pending' when status = '1' then 'Approve' when status = '3' then 'Rejected' end as 'status',reject_reason as 'reject_reason',emp_code,emp_name from pay_client_request_form where comp_code = '" + Session["comp_code"].ToString() + "' and client_request_for = '" + ddl_client_request.SelectedValue + "' ", d.con);


        cmd_id_gv.Fill(dt_id_gv);
        //appro_emp_legal = "0";
        if (dt_id_gv.Rows.Count > 0)
        {
            //ViewState["appro_emp_legal"] = dt_id_gv.Rows.Count.ToString();
            //appro_emp_legal = ViewState["appro_emp_legal"].ToString();

            gv_Leave.DataSource = dt_id_gv;
            gv_Leave.DataBind();


        }
        dt_id_gv.Dispose();


    }
    protected void ddl_client_request_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_client_request.SelectedValue == "1") // for client attendance
        {
            gv_Leave.DataSource = null;
            gv_Leave.DataBind();

            client_attendace();
        }
        else if (ddl_client_request.SelectedValue == "2") // for client leave
       
        {

            gv_attendance.DataSource = null;
            gv_attendance.DataBind();

            client_leave();
        }

        
        //else
        //    if (ddl_client_request.SelectedValue == "2") // for client leave
        //    {

        //        gv_attendance.DataSource = null;
        //        gv_attendance.DataBind();


        //        Panel_leave.Visible = true;
        //        client_leave();
        //    }


    }
    protected void btncloseloewe_Click(object sender, EventArgs e)
    {

    }
    protected void gv_attendance_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Text == "&nbsp;")
                {
                    e.Row.Cells[i].Text = "";
                }
            }
        }


        e.Row.Cells[1].Visible = false;
        e.Row.Cells[4].Visible = false;
    }
    protected void gv_attendance_PreRender(object sender, EventArgs e)
    {
        try
        {
            // UnitGridView.UseAccessibleHeader = false;
            gv_attendance.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }
    protected void gv_Leave_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Text == "&nbsp;")
                {
                    e.Row.Cells[i].Text = "";
                }
            }
        }

        e.Row.Cells[1].Visible = false;
        e.Row.Cells[4].Visible = false;
    }
    protected void gv_Leave_PreRender(object sender, EventArgs e)
    {
        try
        {
            // UnitGridView.UseAccessibleHeader = false;
            gv_Leave.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }
    //protected void gv_attendance_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        d.con.Open();
    //        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
    //        string id = grdrow.Cells[1].Text;
    //      //  MySqlCommand cmd = new MySqlCommand("select invoice_number,date_format(dispatch_date,'%d/%m/%Y') as dispatch_date,date_format(receiving_date,'%d/%m/%Y') as receiving_date,pod_number,shipping_address from pay_bill_invoices where Id='" + id + "' ", d.con);

    //        MySqlCommand cmd = new MySqlCommand("select client_request_for,`attendance_date`,comment_box where Id='" + id + "' ", d.con);
    //        MySqlDataReader dr = cmd.ExecuteReader();
    //        if (dr.Read())
    //        {
    //              ddl_client_request.SelectedValue = dr.GetValue(0).ToString();
    //              txt_date_request.Text = dr.GetValue(1).ToString();
    //              txt_comment.Text = dr.GetValue(2).ToString();
    //        //    txt_bill_rtn_date.Text = dr.GetValue(1).ToString();
    //        //    txt_receiv_date.Text = dr.GetValue(2).ToString();
    //        //    txt_pod_number.Text = dr.GetValue(3).ToString();
    //        //    txt_bill_reason.Text = dr.GetValue(4).ToString();
    //        }
    //      //  btn_bill_save.Visible = true;
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


    protected void btn_approve_Click(object sender, EventArgs e)
    {
        string inlist = "";

        if (ddl_client_request.SelectedValue == "1")
        {

            foreach (GridViewRow gvrow in gv_attendance.Rows)
            {

                // string emp_code = (string)gv_checklist_uniform.DataKeys[gvrow.RowIndex].Value;
                string id = gv_attendance.Rows[gvrow.RowIndex].Cells[1].Text;

                var checkbox = gvrow.FindControl("chk_client") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    string attendance_date = gv_attendance.Rows[gvrow.RowIndex].Cells[7].Text;
                    string emp_code = gv_attendance.Rows[gvrow.RowIndex].Cells[4].Text;
                    inlist = "" + id + "";

                    string date = ""; string month = ""; string year = "";

                    date = attendance_date.Substring(0, 2);
                    month = attendance_date.Substring(3,2);
                    year = attendance_date.Substring(6);

                int result = 0;

                string aa = "CONCAT(DAY" + date + ")";
                aa = aa.Replace(aa, "DAY" + date + "");

                string weekend = d.getsinglestring("select " + aa + " from pay_attendance_muster where comp_code = '" + Session["comp_code"].ToString() + "' and emp_code = '" + emp_code + "' and month = '" + month + "' and year = '" + year + "' ");

                if (weekend != "W")
                {


                    d.operation("update pay_attendance_muster set " + aa + " ='P' where comp_code = '" + Session["comp_code"].ToString() + "' and  emp_code = '" + emp_code + "' and month = '" + month + "' and year = '" + year + "'");

                }


                result = d.operation("UPDATE pay_client_request_form SET status = '1',approval_date_on = now() WHERE comp_code = '" + Session["comp_code"].ToString() + "' and id = '" + inlist + "' ");
               
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Approve Successfully !!!')", true);
                
                }
            }

            client_attendace();
        }
        else
      
            if (ddl_client_request.SelectedValue == "2")
             {
                 foreach (GridViewRow gvrow in gv_Leave.Rows)
                 {

                     // string emp_code = (string)gv_checklist_uniform.DataKeys[gvrow.RowIndex].Value;
                     string id = gv_Leave.Rows[gvrow.RowIndex].Cells[1].Text;

                     var checkbox = gvrow.FindControl("chk_client1") as System.Web.UI.WebControls.CheckBox;
                     if (checkbox.Checked == true)
                     {
                         inlist = "" + id + "";

                         string from_date_leave = gv_Leave.Rows[gvrow.RowIndex].Cells[7].Text;
                         string to_date_leave = gv_Leave.Rows[gvrow.RowIndex].Cells[8].Text;
                         string emp_code = gv_Leave.Rows[gvrow.RowIndex].Cells[4].Text;

                         string from_date = null; string to_date = null; string leave_year = null; string leave_month = "";

                         from_date = from_date_leave.Substring(0, 2); //"DATE_FORMAT(" + att_date + ", '%d')";
                         to_date = to_date_leave.Substring(0, 2);
                         leave_month = from_date_leave.Substring(3, 2); //"DATE_FORMAT(" + att_date + ", '%m')";
                         leave_year = from_date_leave.Substring(6);

                         string leave_date_fun = d.getsinglestring("SELECT GROUP_CONCAT(`id`) FROM `pay_leave_dummy_table` WHERE `id` BETWEEN " + from_date + " AND " + to_date + " ");



                         string[] invoice_ship_add = leave_date_fun.Split(',');


                         foreach (object obj in invoice_ship_add)
                         {
                             string object_day = "" + obj + ""; string bb = null;

                             if (object_day == "1")
                             {
                                 bb = "CONCAT(DAY0" + obj + ")";
                                 bb = bb.Replace(bb, "DAY0" + obj + "");
                             }
                             else

                                 if (object_day == "2")
                                 {
                                     bb = "CONCAT(DAY0" + obj + ")";
                                     bb = bb.Replace(bb, "DAY0" + obj + "");
                                 }
                                 else

                                     if (object_day == "3")
                                     {
                                         bb = "CONCAT(DAY0" + obj + ")";
                                         bb = bb.Replace(bb, "DAY0" + obj + "");
                                     }
                                     else
                                         if (object_day == "4")
                                         {
                                             bb = "CONCAT(DAY0" + obj + ")";
                                             bb = bb.Replace(bb, "DAY0" + obj + "");
                                         }
                                         else
                                             if (object_day == "5")
                                             {
                                                 bb = "CONCAT(DAY0" + obj + ")";
                                                 bb = bb.Replace(bb, "DAY0" + obj + "");
                                             }
                                             else
                                                 if (object_day == "6")
                                                 {
                                                     bb = "CONCAT(DAY0" + obj + ")";
                                                     bb = bb.Replace(bb, "DAY0" + obj + "");
                                                 }
                                                 else
                                                     if (object_day == "7")
                                                     {
                                                         bb = "CONCAT(DAY0" + obj + ")";
                                                         bb = bb.Replace(bb, "DAY0" + obj + "");
                                                     }
                                                     else
                                                         if (object_day == "8")
                                                         {
                                                             bb = "CONCAT(DAY0" + obj + ")";
                                                             bb = bb.Replace(bb, "DAY0" + obj + "");
                                                         }
                                                         else
                                                             if (object_day == "9")
                                                             {
                                                                 bb = "CONCAT(DAY0" + obj + ")";
                                                                 bb = bb.Replace(bb, "DAY0" + obj + "");
                                                             }
                                                             else
                                                             {

                                                                 bb = "CONCAT(DAY" + obj + ")";
                                                                 bb = bb.Replace(bb, "DAY" + obj + "");
                                                             }

                             string weekend = d.getsinglestring("select " + bb + " from pay_attendance_muster where comp_code = '" + Session["comp_code"].ToString() + "' and emp_code = '" + emp_code + "' and month = '" + leave_month + "' and year = '" + leave_year + "' ");

                             if (weekend != "W")
                             {

                                 d.operation("update pay_attendance_muster set " + bb + " ='L' where comp_code = '" + Session["comp_code"].ToString() + "' and emp_code = '" + emp_code + "' and month = '" + leave_month + "' and year = '" + leave_year + "'");
                             }

                         }




                         int result = 0;
                         result = d.operation("UPDATE pay_client_request_form SET status = '1',approval_date_on = now() WHERE comp_code = '" + Session["comp_code"].ToString() + "' and id = '" + inlist + "' ");

                         ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Approve Successfully !!!')", true);
                     }
                 }


                 client_leave();
             }

    }
    protected void btn_reject_Click(object sender, EventArgs e)
    {
        string inlist = "";

        string reject_reason="";

        if (ddl_client_request.SelectedValue == "1")
        {

            foreach (GridViewRow gvrow in gv_attendance.Rows)
            {

                // string emp_code = (string)gv_checklist_uniform.DataKeys[gvrow.RowIndex].Value;
                string id = gv_attendance.Rows[gvrow.RowIndex].Cells[1].Text;

                var checkbox = gvrow.FindControl("chk_client") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {


                    inlist = "" + id + "";

                    TextBox txt_reject_reason = (TextBox)gvrow.FindControl("txt_reject_reason_attendance");
                    reject_reason = (txt_reject_reason.Text);


                    if (reject_reason == "")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Reject Reason')", true);

                        return;


                    }



                    int result = 0;

                    if (reject_reason != "")
                    {
                        result = d.operation("UPDATE pay_client_request_form SET status = '3' , `reject_reason`= '" + reject_reason + "' WHERE comp_code = '" + Session["comp_code"].ToString() + "' and id = '" + inlist + "' ");

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Rejected Successfully !!!')", true);
                    }
                }

            }

            client_attendace();
        }
        else

            if (ddl_client_request.SelectedValue == "2")
            {

                foreach (GridViewRow gvrow in gv_Leave.Rows)
                {

                    // string emp_code = (string)gv_checklist_uniform.DataKeys[gvrow.RowIndex].Value;
                    string id = gv_Leave.Rows[gvrow.RowIndex].Cells[1].Text;

                    var checkbox = gvrow.FindControl("chk_client1") as System.Web.UI.WebControls.CheckBox;
                    if (checkbox.Checked == true)
                    {


                        inlist = "" + id + "";

                        TextBox txt_reject_reason = (TextBox)gvrow.FindControl("txt_reject_reason_leave");
                        reject_reason = (txt_reject_reason.Text);


                        if (reject_reason == "")
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Reject Reason')", true);

                            return;


                        }



                        int result = 0;

                        if (reject_reason != "")
                        {
                            result = d.operation("UPDATE pay_client_request_form SET status = '3', `reject_reason`= '" + reject_reason + "' WHERE comp_code = '" + Session["comp_code"].ToString() + "' and id = '" + inlist + "' ");

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Rejected Successfully !!!')", true);
                        }
                    }
                }


                client_leave();

            }


    }
    protected void btn_show_req_Click(object sender, EventArgs e)
    {
        hidtab.Value = "1";

        dispatch_date_panel.Visible = true;

        upload_data.Visible = true;
        //btn_approve_for_material.Visible = true;
        //btn_hold_material.Visible = true;
        //btn_reject_material.Visible = true;
        //btn_download_report.Visible=true;

        btn_visibility.Visible = true;

        gv_material_dispatch.DataSource = null;
        gv_material_dispatch.DataBind();

        gv_invoice_dispatch.DataSource = null;
        gv_invoice_dispatch.DataBind();

        gv_dublicate_id_card.DataSource = null;
        gv_dublicate_id_card.DataBind();


        if (ddl_type.SelectedValue == "1")
        {
            for_material_gv_search.Visible = true;
            material_data_select();
        }
        else
            if (ddl_type.SelectedValue == "2")
            {
                for_invoice_gv_search.Visible = true;
                invoice_data_select();
            }
            else
                if (ddl_type.SelectedValue == "3")
                {
                    for_dublicate_gv_search.Visible = true;
                    dublicate_id_data_select();

                }


    }


    protected void material_data_select()
    {
        for_material_gv_search.Visible = true;
        System.Data.DataTable dt_id_gv = new System.Data.DataTable();
        MySqlDataAdapter cmd_id_gv = new MySqlDataAdapter("SELECT `pay_client_request_dispatch`.`id`, client_re_upload,case when `material_type` = '1' then 'Material' when material_type = '2' then 'Invoice' when material_type = '3' then 'Duplicate ID-Card' end as 'material_type' ,case when  `dispatch_through` = '1'then 'By Courier' when dispatch_through = '2' then 'By Hand' when dispatch_through = '3' then 'By Postal' end as 'dispatch_through' , case when approve_record = '1' then 'New' when approve_record = '2' then 'Dispatch' when approve_record = '3' then 'Hold' when approve_record = '4' then 'Rejected'  end as 'approve_record' ,`pay_client_request_dispatch`.`client_code`,approve_dispatch_date, `client_name`, pay_client_request_dispatch.`state_name`,pay_client_request_dispatch.`unit_code`, `unit_name`,  pay_client_request_dispatch.`emp_code`, `emp_name`, `material_dispatch_date`, `material_uniform_size`, `material_uniform_set`, `material_shoes_size`, `material_shoes_set`, `material_Receiver_name`, `material_shipping_address`,material_hold_reason,requested_by FROM `pay_client_request_dispatch` INNER JOIN `pay_client_master` ON `pay_client_request_dispatch`.`comp_code` = `pay_client_master`.`comp_code` AND `pay_client_request_dispatch`.`client_code` = `pay_client_master`.`client_code`  INNER JOIN `pay_unit_master` ON `pay_client_request_dispatch`.`comp_code` = `pay_unit_master`.`comp_code` AND `pay_client_request_dispatch`.`unit_code` = `pay_unit_master`.`unit_code` WHERE `pay_client_request_dispatch`.`comp_code` = '" + Session["comp_code"].ToString() + "' and `material_type`= '" + ddl_type.SelectedValue + "' and `approve_record`!='0' and material_dispatch_date  BETWEEN '" + txt_from_date.Text + "' AND '" + txt_to_date.Text + "' ", d.con);
       

        cmd_id_gv.Fill(dt_id_gv);

        if (dt_id_gv.Rows.Count > 0)
        {

            gv_material_dispatch.DataSource = dt_id_gv;
            gv_material_dispatch.DataBind();


        }
        dt_id_gv.Dispose();



    }

    protected void invoice_data_select()
    {
        for_invoice_gv_search.Visible = true;
        System.Data.DataTable dt_id_gv = new System.Data.DataTable();
        MySqlDataAdapter cmd_id_gv = new MySqlDataAdapter("SELECT `pay_client_request_dispatch`.`id`,client_re_upload, case when `material_type` = '1' then 'Material' when material_type = '2' then 'Invoice' when material_type = '3' then 'Duplicate ID-Card' end as 'material_type' ,case when  `dispatch_through` = '1'then 'By Courier' when dispatch_through = '2' then 'By Hand' when dispatch_through = '3' then 'By Postal' end as 'dispatch_through' , case when approve_record = '1' then 'New' when approve_record = '2' then 'Dispatch' when approve_record = '3' then 'Hold' when approve_record = '4' then 'Rejected' end as 'approve_record', `pay_client_request_dispatch`.`client_code`, `client_name`, pay_client_request_dispatch.`state_name`,pay_client_request_dispatch.`unit_code`,approve_dispatch_date, invoice_branch_name,material_dispatch_date, `material_shipping_address`,pay_client_request_dispatch.month_year,material_person_name,material_hold_reason,requested_by FROM `pay_client_request_dispatch` INNER JOIN `pay_client_master` ON `pay_client_request_dispatch`.`comp_code` = `pay_client_master`.`comp_code` AND `pay_client_request_dispatch`.`client_code` = `pay_client_master`.`client_code`  WHERE `pay_client_request_dispatch`.`comp_code` = '" + Session["comp_code"].ToString() + "' and `material_type`= '" + ddl_type.SelectedValue + "' and `approve_record`!='0' and material_dispatch_date  BETWEEN '" + txt_from_date.Text + "' AND '" + txt_to_date.Text + "' ", d.con);
       

        cmd_id_gv.Fill(dt_id_gv);
        //appro_emp_legal = "0";
        if (dt_id_gv.Rows.Count > 0)
        {
            gv_invoice_dispatch.DataSource = dt_id_gv;
            gv_invoice_dispatch.DataBind();


        }
        dt_id_gv.Dispose();



    }

    protected void dublicate_id_data_select()
    {
        for_dublicate_gv_search.Visible = true;

        System.Data.DataTable dt_id_gv = new System.Data.DataTable();
        MySqlDataAdapter cmd_id_gv = new MySqlDataAdapter("SELECT `pay_client_request_dispatch`.`id`,client_re_upload, approve_dispatch_date,case when `material_type` = '1' then 'Material' when material_type = '2' then 'Invoice' when material_type = '3' then 'Duplicate ID-Card' end as 'material_type' ,case when  `dispatch_through` = '1'then 'By Courier' when dispatch_through = '2' then 'By Hand' when dispatch_through = '3' then 'By Postal' end as 'dispatch_through' ,case when approve_record = '1' then 'New' when approve_record = '2' then 'Dispatch' when approve_record = '3' then 'Hold' when approve_record = '4' then 'Rejected' end as 'approve_record', `pay_client_request_dispatch`.`client_code`, `client_name`, pay_client_request_dispatch.`state_name`,pay_client_request_dispatch.`unit_code`, `unit_name`,  pay_client_request_dispatch.`emp_code`, `emp_name`, `material_dispatch_date`,  `material_Receiver_name`, `material_shipping_address`,dublicate_id_card,material_person_name,material_hold_reason,requested_by,approve_record FROM `pay_client_request_dispatch` INNER JOIN `pay_client_master` ON `pay_client_request_dispatch`.`comp_code` = `pay_client_master`.`comp_code` AND `pay_client_request_dispatch`.`client_code` = `pay_client_master`.`client_code`  INNER JOIN `pay_unit_master` ON `pay_client_request_dispatch`.`comp_code` = `pay_unit_master`.`comp_code` AND `pay_client_request_dispatch`.`unit_code` = `pay_unit_master`.`unit_code` WHERE `pay_client_request_dispatch`.`comp_code` = '" + Session["comp_code"].ToString() + "' and `material_type`= '" + ddl_type.SelectedValue + "' and `approve_record`!='0' and material_dispatch_date  BETWEEN '" + txt_from_date.Text + "' AND '" + txt_to_date.Text + "' ", d.con);


        cmd_id_gv.Fill(dt_id_gv);

        if (dt_id_gv.Rows.Count > 0)
        {

            gv_dublicate_id_card.DataSource = dt_id_gv;
            gv_dublicate_id_card.DataBind();


        }
        dt_id_gv.Dispose();


    }

    
    protected void btn_approve_for_material_Click(object sender, EventArgs e)
    {
        dispatch_date_panel.Visible = true;
      
        if(ddl_type.SelectedValue=="1")
        {
            for_material_gv_search.Visible = true;
            material_approve_code();
        
        }

        if (ddl_type.SelectedValue == "2")
        {
            invoice_approve_code();

        }

        if (ddl_type.SelectedValue == "3")
        {
            dublicate_approve_code();

        }
        btn_visibility.Visible = true;   
    }

    // for material Hold data
    protected void material_hold_code() 
    {
        // for material approve
        if (ddl_type.SelectedValue == "1")
        {


            foreach (GridViewRow gvrow in gv_material_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";
                string material_hold = "";

                var checkbox = gvrow.FindControl("chk_record_material") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;
                    emp_code = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[10].Text;
                    emp_name = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[11].Text;

                    TextBox txt_material = (TextBox)gvrow.FindControl("txt_hold_material");
                    material_hold = (txt_material.Text);


                    if (material_hold == "")
                    {

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Hold Reason .')", true);

                        return;

                    }


                }
            }



            foreach (GridViewRow gvrow in gv_material_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";

                var checkbox = gvrow.FindControl("chk_record_material") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;
                    emp_code = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[10].Text;
                    emp_name = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[11].Text;
                    string material_hold = "";

                    TextBox txt_material = (TextBox)gvrow.FindControl("txt_hold_material");
                    material_hold = (txt_material.Text);


                    //if (material_hold == "")
                    //{

                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Hold Reason .')", true);

                    //    return;

                    //}

                    int result = 0;
                    if (material_hold != "")
                    {
                        result = d.operation("UPDATE pay_client_request_dispatch SET `approve_record` = '3', material_hold_reason= '" + material_hold + "' WHERE comp_code = '" + Session["comp_code"].ToString() + "' and emp_code = '" + emp_code + "'  AND id = '" + id + "'  ");
                        if (result > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record On Hold Successfully !!!')", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Not Hold Successfully !!!')", true);
                        }
                    
                    }
                }

                

               
            }

           material_data_select();

        }
    
    }


    // invoice hold data
    protected void invoice_hold_code()
    {
        // for material approve
        if (ddl_type.SelectedValue == "2")
        {

            foreach (GridViewRow gvrow in gv_invoice_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";
                string invoice_reject = "";

                var checkbox = gvrow.FindControl("chk_record_invoice") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;

                    int result = 0;


                    TextBox txt_invoice = (TextBox)gvrow.FindControl("txt_invoice_reason");
                    invoice_reject = (txt_invoice.Text);


                    if (invoice_reject == "")
                    {

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Reject Reason .')", true);

                        return;

                    }
                }
            }

            foreach (GridViewRow gvrow in gv_invoice_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";

                var checkbox = gvrow.FindControl("chk_record_invoice") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;
                   
                    string invoice_hold = "";

                    TextBox txt_reason = (TextBox)gvrow.FindControl("txt_invoice_reason");
                    invoice_hold = (txt_reason.Text);


                    //if (invoice_hold == "")
                    //{

                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Hold Reason')", true);

                    //    return;

                    //}

                    int result = 0;
                    if (invoice_hold != "")
                    {
                        result = d.operation("UPDATE pay_client_request_dispatch SET `approve_record` = '3',material_hold_reason= '" + invoice_hold + "' WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND id = '" + id + "'  ");

                        if (result > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record On Hold Successfully !!!')", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Not Hold Successfully !!!')", true);
                        }

                    }
                }




            }

            invoice_data_select();

        }

    }

    // dublicate hold data
    protected void dublicate_hold_code()
    {
        // for material approve
        if (ddl_type.SelectedValue == "3")
        {




            foreach (GridViewRow gvrow in gv_dublicate_id_card.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";

                var checkbox = gvrow.FindControl("chk_record_dublicate") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[2].Text;

                    string dublicate_hold = "";

                    TextBox txt_reason = (TextBox)gvrow.FindControl("txt_dublicate_reason");
                    dublicate_hold = (txt_reason.Text);


                    if (dublicate_hold == "")
                    {

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Hold Reason')", true);

                        return;

                    }
                }
            }


            foreach (GridViewRow gvrow in gv_dublicate_id_card.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";

                var checkbox = gvrow.FindControl("chk_record_dublicate") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[2].Text;

                    string dublicate_hold = "";

                    TextBox txt_reason = (TextBox)gvrow.FindControl("txt_dublicate_reason");
                    dublicate_hold = (txt_reason.Text);


                    //if (dublicate_hold == "")
                    //{

                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Hold Reason')", true);

                    //    return;

                    //}

                    int result = 0;
                    if (dublicate_hold != "")
                    {
                        result = d.operation("UPDATE pay_client_request_dispatch SET `approve_record` = '3',material_hold_reason= '" + dublicate_hold + "' WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND id = '" + id + "'  ");

                        if (result > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record On Hold Successfully !!!')", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Not Hold Successfully !!!')", true);
                        }

                    }
                }




            }
            dublicate_id_data_select();
        }

    }




    // for material approve data
    protected void material_approve_code()
    {
        // for material approve
        if (ddl_type.SelectedValue == "1")
        {


            foreach (GridViewRow gvrow in gv_material_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";

                var checkbox = gvrow.FindControl("chk_record_material") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;
                    emp_code = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[10].Text;
                    emp_name = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[11].Text;

                    string client_upload = d.getsinglestring("select client_re_upload from pay_client_request_dispatch where  comp_code = '" + Session["comp_code"].ToString() + "' and id = '" + id + "'");

                    if (client_upload == "")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Upload File First !!');", true);
                        material_data_select();
                        return;
                    }


                    int result = 0;

                    result = d.operation("UPDATE pay_client_request_dispatch SET `approve_record` = '2',approve_dispatch_date= '" + txt_dispatch_date.Text + "',material_hold_reason= '' WHERE comp_code = '" + Session["comp_code"].ToString() + "' and emp_code = '" + emp_code + "'  AND id = '" + id + "'  ");

                        if (result > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Approve Successfully !!!')", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Not Approve Successfully !!!')", true);
                        }

                   
                }




            }

            material_data_select();
        }

    }


    // invoice approve data
    protected void invoice_approve_code()
    {
        // for material approve
        if (ddl_type.SelectedValue == "2")
        {


            foreach (GridViewRow gvrow in gv_invoice_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";

                var checkbox = gvrow.FindControl("chk_record_invoice") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;


                    string client_upload = d.getsinglestring("select client_re_upload from pay_client_request_dispatch where  comp_code = '" + Session["comp_code"].ToString() + "' and id = '" + id + "'");

                    if (client_upload == "")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Upload File First !!');", true);
                        invoice_data_select();
                        return;
                    }


                    int result = 0;

                    result = d.operation("UPDATE pay_client_request_dispatch SET `approve_record` = '2',approve_dispatch_date= '" + txt_dispatch_date.Text + "',material_hold_reason= '' WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND id = '" + id + "'  ");

                        if (result > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Approve Successfully !!!')", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Not Approve Successfully !!!')", true);
                        }

                    
                }

            }
            invoice_data_select();
        }

    }

    // dublicate approve data
    protected void dublicate_approve_code()
    {
        // for material approve
        if (ddl_type.SelectedValue == "3")
        {


            foreach (GridViewRow gvrow in gv_dublicate_id_card.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";

                var checkbox = gvrow.FindControl("chk_record_dublicate") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[2].Text;


                    string client_upload = d.getsinglestring("select client_re_upload from pay_client_request_dispatch where  comp_code = '" + Session["comp_code"].ToString() + "' and id = '" + id + "'");

                    if (client_upload == "")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Upload File First !!');", true);
                        dublicate_id_data_select();
                        return;
                    }

                    string dublicate_hold = "";

                   
                    int result = 0;

                    result = d.operation("UPDATE pay_client_request_dispatch SET `approve_record` = '2',approve_dispatch_date= '" + txt_dispatch_date.Text + "',material_hold_reason= '' WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND id = '" + id + "'  ");
                        if (result > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Approve Successfully !!!')", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Not Approve Successfully !!!')", true);
                        }
                   
                }




            }
            dublicate_id_data_select();
        }

    }



    protected void btn_hold_material_Click(object sender, EventArgs e)
    {
        btn_visibility.Visible = true;
        dispatch_date_panel.Visible = true;
        
         if(ddl_type.SelectedValue=="1")
        {
            material_hold_code();
        
        }

        if (ddl_type.SelectedValue == "2")
        {
            invoice_hold_code();

        }

        if (ddl_type.SelectedValue == "3")
        {
            dublicate_hold_code();

        }


     }
    protected void gv_invoice_dispatch_SelectedIndexChanged(object sender, EventArgs e)
    {
       
    }
    protected void gv_material_dispatch_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Text == "&nbsp;")
                {
                    e.Row.Cells[i].Text = "";
                }
            }
        }


        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (DataRowView)e.Row.DataItem;
            if (dr["client_re_upload"].ToString() == "")
            {
                //LinkButton lb1 = e.Row.FindControl("unit_name") as LinkButton;
                //lb1.Visible = false;
                e.Row.Cells[23].Text = "Not Download";

            }

            
        }

        try
        {
            GridViewRow material_Gridview = e.Row;
            if (material_Gridview.Cells[22].Text.Equals("Dispatch"))
            {
                e.Row.BackColor = System.Drawing.Color.Green;
            }
            if (material_Gridview.Cells[22].Text.Equals("Hold"))
            {
                e.Row.BackColor = System.Drawing.Color.Yellow;
            }
            if (material_Gridview.Cells[22].Text.Equals("Rejected"))
            {
                e.Row.BackColor = System.Drawing.Color.Red;
            }
            if (material_Gridview.Cells[22].Text.Equals("Approve"))
            {
                e.Row.BackColor = System.Drawing.Color.Orange;
            }

        }
        catch (Exception ex)
        { }


        e.Row.Cells[2].Visible = false;
        e.Row.Cells[5].Visible = false;
        e.Row.Cells[8].Visible = false;
        e.Row.Cells[10].Visible = false;
    }
    protected void gv_invoice_dispatch_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Text == "&nbsp;")
                {
                    e.Row.Cells[i].Text = "";
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (DataRowView)e.Row.DataItem;
            if (dr["client_re_upload"].ToString() == "")
            {
                //LinkButton lb1 = e.Row.FindControl("unit_name") as LinkButton;
                //lb1.Visible = false;
                e.Row.Cells[18].Text = "Not Download";

            }


        }



        try
        {
            GridViewRow invoice_Gridview = e.Row;
            if (invoice_Gridview.Cells[17].Text.Equals("Dispatch"))
            {
                e.Row.BackColor = System.Drawing.Color.Green;
            }
            if (invoice_Gridview.Cells[17].Text.Equals("Hold"))
            {
                e.Row.BackColor = System.Drawing.Color.Yellow;
            }
            if (invoice_Gridview.Cells[17].Text.Equals("Rejected"))
            {
                e.Row.BackColor = System.Drawing.Color.Red;
            }
            if (invoice_Gridview.Cells[17].Text.Equals("Approve"))
            {
                e.Row.BackColor = System.Drawing.Color.Orange;
            }

        }
        catch (Exception ex)
        { }


        e.Row.Cells[2].Visible = false;
        e.Row.Cells[5].Visible = false;
        e.Row.Cells[8].Visible = false;

    }
    protected void gv_dublicate_id_card_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Text == "&nbsp;")
                {
                    e.Row.Cells[i].Text = "";
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (DataRowView)e.Row.DataItem;
            if (dr["client_re_upload"].ToString() == "")
            {
                //LinkButton lb1 = e.Row.FindControl("unit_name") as LinkButton;
                //lb1.Visible = false;
                e.Row.Cells[20].Text = "Not Download";

            }


        }


        try
        {
            GridViewRow dublicate_Gridview = e.Row;
            if (dublicate_Gridview.Cells[19].Text.Equals("Dispatch"))
            {
                e.Row.BackColor = System.Drawing.Color.Green;
            }
            if (dublicate_Gridview.Cells[19].Text.Equals("Hold"))
            {
                e.Row.BackColor = System.Drawing.Color.Yellow;
            }
            if (dublicate_Gridview.Cells[19].Text.Equals("Rejected"))
            {
                e.Row.BackColor = System.Drawing.Color.Red;
            }
            if (dublicate_Gridview.Cells[19].Text.Equals("Approve"))
            {
                e.Row.BackColor = System.Drawing.Color.Orange;
            }

        }
        catch (Exception ex)
        { }

        e.Row.Cells[2].Visible = false;
        e.Row.Cells[5].Visible = false;
        e.Row.Cells[8].Visible = false;
        e.Row.Cells[10].Visible = false;
    }
    protected void btn_reject_material_Click(object sender, EventArgs e)
    {
        btn_visibility.Visible = true;
        dispatch_date_panel.Visible = true;

        if (ddl_type.SelectedValue == "1")
        {
            material_rejected_code();

        }

        if (ddl_type.SelectedValue == "2")
        {
            invoice_rejected_code();

        }

        if (ddl_type.SelectedValue == "3")
        {
            dublicate_rejected_code();

        }
    }


    // for material rejected data
    protected void material_rejected_code()
    {
        // for material approve
        if (ddl_type.SelectedValue == "1")
        {

            foreach (GridViewRow gvrow in gv_material_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";
                string material_hold = "";

                var checkbox = gvrow.FindControl("chk_record_material") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;
                    emp_code = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[10].Text;
                    emp_name = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[11].Text;

                    TextBox txt_material = (TextBox)gvrow.FindControl("txt_hold_material");
                    material_hold = (txt_material.Text);


                    if (material_hold == "")
                    {

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Reject Reason .')", true);

                        return;

                    }


                }
            }






            foreach (GridViewRow gvrow in gv_material_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";
                string material_hold = "";

                var checkbox = gvrow.FindControl("chk_record_material") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;
                    emp_code = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[10].Text;
                    emp_name = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[11].Text;

                    TextBox txt_material = (TextBox)gvrow.FindControl("txt_hold_material");
                    material_hold = (txt_material.Text);


                    //if (material_hold == "")
                    //{

                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Reject Reason .')", true);

                    //    return;

                    //}

                    int result = 0;

                    result = d.operation("UPDATE pay_client_request_dispatch SET `approve_record` = '4',material_hold_reason= '" + material_hold + "' WHERE comp_code = '" + Session["comp_code"].ToString() + "' and emp_code = '" + emp_code + "'  AND id = '" + id + "'  ");

                    if (result > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Rejected Successfully !!!')", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Not Rejected Successfully !!!')", true);
                    }


                }




            }

            material_data_select();
        }

    }


    // invoice rejected data
    protected void invoice_rejected_code()
    {
        // for material approve
        if (ddl_type.SelectedValue == "2")
        {


            foreach (GridViewRow gvrow in gv_invoice_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";
                string invoice_reject = "";

                var checkbox = gvrow.FindControl("chk_record_invoice") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;

                    int result = 0;


                    TextBox txt_invoice = (TextBox)gvrow.FindControl("txt_invoice_reason");
                    invoice_reject = (txt_invoice.Text);


                    if (invoice_reject == "")
                    {

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Reject Reason .')", true);

                        return;

                    }
                }
            }


            foreach (GridViewRow gvrow in gv_invoice_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";
                string invoice_reject = "";

                var checkbox = gvrow.FindControl("chk_record_invoice") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;

                    int result = 0;


                TextBox txt_invoice = (TextBox)gvrow.FindControl("txt_invoice_reason");
                 invoice_reject = (txt_invoice.Text);


                 //   if (invoice_reject == "")
                 //   {

                 //       ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Reject Reason .')", true);

                 //       return;

                 //   }

                    result = d.operation("UPDATE pay_client_request_dispatch SET `approve_record` = '4',material_hold_reason= '"+invoice_reject+"' WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND id = '" + id + "'  ");

                    if (result > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Rejected Successfully !!!')", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Not Rejected Successfully !!!')", true);
                    }


                }

            }
            invoice_data_select();
        }

    }

    // dublicate rejected data
    protected void dublicate_rejected_code()
    {
        // for material approve
        if (ddl_type.SelectedValue == "3")
        {


            foreach (GridViewRow gvrow in gv_dublicate_id_card.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";
                string dublicate_reject = "";

            var checkbox = gvrow.FindControl("chk_record_dublicate") as System.Web.UI.WebControls.CheckBox;
            if (checkbox.Checked == true)
            {
                id = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[2].Text;



                TextBox txt_dublicate = (TextBox)gvrow.FindControl("txt_dublicate_reason");
                dublicate_reject = (txt_dublicate.Text);


                if (dublicate_reject == "")
                {

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Reject Reason .')", true);

                    return;

                }
            }


        }


            foreach (GridViewRow gvrow in gv_dublicate_id_card.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";
                string dublicate_reject = "";


                var checkbox = gvrow.FindControl("chk_record_dublicate") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[2].Text;

                    TextBox txt_dublicate = (TextBox)gvrow.FindControl("txt_dublicate_reason");
                   
                    dublicate_reject = (txt_dublicate.Text);


                    //if (dublicate_reject == "")
                    //{

                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Reject Reason .')", true);

                    //   return;

                    //}



                    int result = 0;

                    result = d.operation("UPDATE pay_client_request_dispatch SET `approve_record` = '4',material_hold_reason= '" + dublicate_reject + "' WHERE comp_code = '" + Session["comp_code"].ToString() + "' AND id = '" + id + "'  ");
                    if (result > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Rejected Successfully !!!')", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert(' Record Not Rejected Successfully !!!')", true);
                    }

                }




            }
            dublicate_id_data_select();
        }

    }

    protected void btn_download_report_Click(object sender, EventArgs e)
    {
        dispatch_date_panel.Visible = true;
        try
        {

            if (ddl_type.SelectedValue == "1")
            {

                string material_list = ""; string emp_code = "";
                foreach (GridViewRow gvrow in gv_material_dispatch.Rows)
                {
                    string emp_code_update = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[10].Text;
                    //string Record_no = (string)gv_material_dispatch.DataKeys[gvrow.RowIndex].Value;
                    emp_code = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[10].Text;

                    var checkbox = gvrow.FindControl("chk_record_material") as CheckBox;


                    if (checkbox.Checked == true)
                    {

                        material_list = material_list + "'" + emp_code + "',";



                     


                        d.operation("update pay_client_request_dispatch set download_report_date = now() where comp_code = '" + Session["comp_code"] + "' and emp_code = '" + emp_code_update + "'  ");
                    }
                    else
                    {
                        emp_code = "";
                    }
                }

                if (material_list.Length > 0)
                {
                    material_list = material_list.Substring(0, material_list.Length - 1);
                }



                ReportDocument crystalReport = new ReportDocument();

                MySqlDataAdapter cmd_item = null;
                System.Data.DataTable dt = new System.Data.DataTable();

                cmd_item = new MySqlDataAdapter("select  DATE_FORMAT(download_report_date, '%d-%m-%Y') AS 'month',case when `material_type` = '1' then 'Material' when material_type = '2' then 'Invoice' when material_type = '3' then 'Duplicate ID-Card' end as 'COMP_CODE' ,case when  `dispatch_through` = '1'then 'By Courier' when dispatch_through = '2' then 'By Hand' when dispatch_through = '3' then 'By Postal' end as 'UNIT_ADD1',material_uniform_size as 'ADDRESS1',material_uniform_set AS 'UNIT_ADD2',material_id_card AS 'ADDRESS2',client_name as 'client_code',pay_client_request_dispatch.state_name as 'STATE',unit_name as 'CITY',emp_name AS 'COMPANY_NAME',material_shipping_address AS 'UNIT_CITY',material_dispatch_date AS 'other' from pay_client_request_dispatch  inner join pay_client_master on  pay_client_request_dispatch.comp_code = pay_client_master.comp_code and  pay_client_request_dispatch.client_code = pay_client_master.client_code  inner join pay_unit_master on  pay_client_request_dispatch.comp_code = pay_unit_master.comp_code and  pay_client_request_dispatch.unit_code = pay_unit_master.unit_code where pay_client_request_dispatch.comp_code = '" + Session["comp_code"].ToString() + "' and emp_code in (" + material_list + ") and material_type = '" + ddl_type.SelectedValue + "' and `approve_record`='2' ", d.con);

                d.con.Open();
                try
                {
                    cmd_item.Fill(dt);
                }

                catch (Exception ex) { throw ex; }

                crystalReport.Load(Server.MapPath("~/dispatch_material.rpt"));
                crystalReport.SetDataSource(dt);
                crystalReport.Refresh();
                crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, "approval_dispatch_copy");
            }
            else
                if (ddl_type.SelectedValue == "2")
                {
                    report_download_invoice();
                }
            else
                if(ddl_type.SelectedValue=="3")
                {
                    report_download_dublicate_id();
                }
               



        }
        catch (Exception ex) { throw ex; }
        finally{}
    }

    protected void report_download_invoice()
    {

        if (ddl_type.SelectedValue == "2")
        {


            string material_list = ""; string unit_code = "";
            foreach (GridViewRow gvrow in gv_invoice_dispatch.Rows)
            {
                string unit_code_update = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[8].Text;
             
                unit_code = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[8].Text;

                var checkbox = gvrow.FindControl("chk_record_invoice") as CheckBox;


                if (checkbox.Checked == true)
                {

                    material_list = material_list + "'" + unit_code + "',";



                  



                    d.operation("update pay_client_request_dispatch set download_report_date = now() where comp_code = '" + Session["comp_code"] + "' and unit_code = '" + unit_code_update + "'  ");
                }
                else
                {
                    unit_code = "";
                }
            }

            if (material_list.Length > 0)
            {
                material_list = material_list.Substring(0, material_list.Length - 1);
            }

            ReportDocument crystalReport = new ReportDocument();

            MySqlDataAdapter cmd_item = null;
            System.Data.DataTable dt = new System.Data.DataTable();

            cmd_item = new MySqlDataAdapter("select  DATE_FORMAT(download_report_date, '%d-%m-%Y') AS 'month',case when `material_type` = '1' then 'Material' when material_type = '2' then 'Invoice' when material_type = '3' then 'Duplicate ID-Card' end as 'COMP_CODE' ,case when  `dispatch_through` = '1'then 'By Courier' when dispatch_through = '2' then 'By Hand' when dispatch_through = '3' then 'By Postal' end as 'UNIT_ADD1',client_name as 'client_code',pay_client_request_dispatch.state_name as 'STATE',unit_name as 'CITY',material_shipping_address AS 'UNIT_CITY',material_dispatch_date AS 'other',material_person_name as 'COMPANY_NAME' from pay_client_request_dispatch  inner join pay_client_master on  pay_client_request_dispatch.comp_code = pay_client_master.comp_code and  pay_client_request_dispatch.client_code = pay_client_master.client_code  inner join pay_unit_master on  pay_client_request_dispatch.comp_code = pay_unit_master.comp_code and  pay_client_request_dispatch.unit_code = pay_unit_master.unit_code where pay_client_request_dispatch.comp_code = '" + Session["comp_code"].ToString() + "'and material_type = '"+ddl_type.SelectedValue+"' and pay_client_request_dispatch.unit_code in (" + material_list + ") and `approve_record`='2'", d.con);

            d.con.Open();
            try
            {
                cmd_item.Fill(dt);
            }

            catch (Exception ex) { throw ex; }

            crystalReport.Load(Server.MapPath("~/dispatch_invoice.rpt"));
            crystalReport.SetDataSource(dt);
            crystalReport.Refresh();
            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, "invoice_download_report");

        }



    }



    protected void report_download_dublicate_id() 
    {
    
        if(ddl_type.SelectedValue=="3")
        {


            string material_list = ""; string emp_code = "";
            foreach (GridViewRow gvrow in gv_dublicate_id_card.Rows)
            {
                string emp_code_update = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[10].Text;
                //string Record_no = (string)gv_material_dispatch.DataKeys[gvrow.RowIndex].Value;
                emp_code = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[10].Text;

                var checkbox = gvrow.FindControl("chk_record_dublicate") as CheckBox;


                if (checkbox.Checked == true)
                {

                    material_list = material_list + "'" + emp_code + "',";



                    


                    d.operation("update pay_client_request_dispatch set download_report_date = now() where comp_code = '" + Session["comp_code"] + "' and emp_code = '" + emp_code_update + "'  ");
                }
                else
                {
                    emp_code = "";
                }
            }

            if (material_list.Length > 0)
            {
                material_list = material_list.Substring(0, material_list.Length - 1);
            }


            ReportDocument crystalReport = new ReportDocument();

            MySqlDataAdapter cmd_item = null;
            System.Data.DataTable dt = new System.Data.DataTable();

            cmd_item = new MySqlDataAdapter("select  DATE_FORMAT(download_report_date, '%d-%m-%Y') AS 'month',case when `material_type` = '1' then 'Material' when material_type = '2' then 'Invoice' when material_type = '3' then 'Duplicate ID-Card' end as 'COMP_CODE' ,case when  `dispatch_through` = '1'then 'By Courier' when dispatch_through = '2' then 'By Hand' when dispatch_through = '3' then 'By Postal' end as 'UNIT_ADD1',client_name as 'client_code',pay_client_request_dispatch.state_name as 'STATE',unit_name as 'CITY',emp_name AS 'COMPANY_NAME',material_shipping_address AS 'UNIT_CITY',material_dispatch_date AS 'other' from pay_client_request_dispatch  inner join pay_client_master on  pay_client_request_dispatch.comp_code = pay_client_master.comp_code and  pay_client_request_dispatch.client_code = pay_client_master.client_code  inner join pay_unit_master on  pay_client_request_dispatch.comp_code = pay_unit_master.comp_code and  pay_client_request_dispatch.unit_code = pay_unit_master.unit_code where pay_client_request_dispatch.comp_code = '" + Session["comp_code"].ToString() + "' and material_type = '"+ddl_type.SelectedValue+"' and emp_code in (" + material_list + ") and `approve_record` ='2' ", d.con);

            d.con.Open();
            try
            {
                cmd_item.Fill(dt);
            }

            catch (Exception ex) { throw ex; }

            crystalReport.Load(Server.MapPath("~/dispatch_dublicate_id.rpt"));
            crystalReport.SetDataSource(dt);
            crystalReport.Refresh();
            crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, this.Response, false, "dublicate_id_download");
        
        }
    
    
    
    }

    protected void gv_material_dispatch_PreRender(object sender, EventArgs e)
    {
        try
        {
            // UnitGridView.UseAccessibleHeader = false;
            gv_material_dispatch.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }


    // upload code 22-02-2021 

    protected void btn_approval_upload_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        hidtab.Value = "1";

        btn_visibility.Visible = true;
        if (ddl_type.SelectedValue == "1")
        {

            if (client_request_upload.HasFile)
            {
                material_upload_data();  // for material 
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Please Select File')", true);
            }

        }

        else
            if (ddl_type.SelectedValue == "2")
            {
                if (client_request_upload.HasFile)
                {
                    invoice_upload_data();  // for material 
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Please Select File')", true);
                }
            
            }
        if (ddl_type.SelectedValue == "3") 
        {
            if (client_request_upload.HasFile)
            {
                dublicate_upload_data();  // for material 
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Please Select File')", true);
            }
        
        
        }

        dispatch_date_panel.Visible = true;
    }

    // for material upload
    protected void material_upload_data()
    {
        try
        {
            d.con.Open();
            string fileExt = "";
            string bill_upload1 = "";

            foreach (GridViewRow gvrow in gv_material_dispatch.Rows)
            {


                string id = "";
                string emp_code = "";
                string emp_name = "";
                string material_type = "";

                string client_code = "";
                string state_name = ""; string unit_code = "";

                var checkbox = gvrow.FindControl("chk_record_material") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;
                    emp_code = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[10].Text;
                    emp_name = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[11].Text;
                    client_code = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[5].Text;
                    state_name = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[7].Text;
                    unit_code = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[8].Text;
                    material_type = gv_material_dispatch.Rows[gvrow.RowIndex].Cells[3].Text;

                    //con_bill_upload as bill_upload
                    fileExt = System.IO.Path.GetExtension(client_request_upload.FileName);
                    bill_upload1 = Path.GetFileName(client_request_upload.PostedFile.FileName);

                    string fname = null;
                    if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".ZIP")
                    {
                        string fileName = bill_upload1;
                        client_request_upload.PostedFile.SaveAs(Server.MapPath("~/client_request_upload/") + fileName);
                        if (ddl_type.SelectedValue == "1")
                        {
                            fname = Session["COMP_CODE"].ToString() + "_" + client_code + "_" + state_name + "_" + emp_name + "_" + material_type +"_"+ id + "client_request_form" + fileExt;
                        }
                        
                        File.Copy(Server.MapPath("~/client_request_upload/") + fileName, Server.MapPath("~/client_request_upload/") + fname, true);
                        File.Delete(Server.MapPath("~/client_request_upload/") + fileName);

                        int result = 0;

                        if (ddl_type.SelectedValue == "1" )
                        {

                             result = d.operation("update pay_client_request_dispatch set client_re_upload = '" + fname + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + client_code + "' and state_name = '" + state_name + "' and unit_code = '" + unit_code + "' and id = '"+id+"'");
                        }
                        //else
                        //    if (ddl_type.SelectedValue == "3")
                        //    {

                        //        //    result = d.operation("update pay_client_request_dispatch set client_re_upload = '" + fname + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + ddl_client_invoice.SelectedValue + "' and state_name = '" + ddl_state_invoice.SelectedValue + "' and unit_code = '" + ddl_branch_invoice.SelectedValue + "' and material_type = '" + ddl_type.SelectedValue + "'  and dispatch_through = '" + ddl_dispatch_through.SelectedValue + "' and material_dispatch_date = '" + txt_material_dispatch_date.Text + "'");
                        //    }


                        if (result > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Files uploaded Successfully... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Files uploading Failed... !!!');", true);
                        }


                    }

                }
            }

            material_data_select();

        }
        catch (Exception ex) { throw ex; }
        finally { }

    }


    // for invoice upload 
    protected void invoice_upload_data()
    {
        try
        {
            d.con.Open();
            string fileExt = "";
            string bill_upload1 = "";

            foreach (GridViewRow gvrow in gv_invoice_dispatch.Rows)
            {


                string id = "";
                string material_type = "";
                string emp_name = "";

                string client_code = "";
                string state_name = ""; string unit_code = "";

                var checkbox = gvrow.FindControl("chk_record_invoice") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[2].Text;
                    client_code = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[5].Text;
                    state_name = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[7].Text;
                    unit_code = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[8].Text;
                    material_type = gv_invoice_dispatch.Rows[gvrow.RowIndex].Cells[3].Text;


                    //con_bill_upload as bill_upload
                    fileExt = System.IO.Path.GetExtension(client_request_upload.FileName);
                    bill_upload1 = Path.GetFileName(client_request_upload.PostedFile.FileName);

                    string fname = null;
                    if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".ZIP")
                    {
                        string fileName = bill_upload1;
                        client_request_upload.PostedFile.SaveAs(Server.MapPath("~/client_request_upload/") + fileName);
                        if (ddl_type.SelectedValue == "2")
                        {
                            fname = Session["COMP_CODE"].ToString() + "_" + client_code + "_" + state_name + "_" + unit_code + "_" + material_type +"_"+ id + "client_request_form" + fileExt;
                        }

                        File.Copy(Server.MapPath("~/client_request_upload/") + fileName, Server.MapPath("~/client_request_upload/") + fname, true);
                        File.Delete(Server.MapPath("~/client_request_upload/") + fileName);

                        int result = 0;

                        if (ddl_type.SelectedValue == "2")
                        {

                            result = d.operation("update pay_client_request_dispatch set client_re_upload = '" + fname + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + client_code + "' and state_name = '" + state_name + "' and unit_code = '" + unit_code + "' and id = '" + id + "'");
                        }
                        
                        if (result > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Files uploaded Successfully... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Files uploading Failed... !!!');", true);
                        }


                    }



                }
            }

            invoice_data_select();

        }
        catch (Exception ex) { throw ex; }
        finally { }

    }


    // for dublicate upload 
    protected void dublicate_upload_data()
    {
        try
        {
            d.con.Open();
            string fileExt = "";
            string bill_upload1 = "";

            foreach (GridViewRow gvrow in gv_dublicate_id_card.Rows)
            {


                string id = "";
                string material_type = "";
                string emp_name = "";

                string client_code = "";
                string state_name = ""; string unit_code = "";

                var checkbox = gvrow.FindControl("chk_record_dublicate") as System.Web.UI.WebControls.CheckBox;
                if (checkbox.Checked == true)
                {
                    id = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[2].Text;
                    client_code = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[5].Text;
                    state_name = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[7].Text;
                    unit_code = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[8].Text;
                    material_type = gv_dublicate_id_card.Rows[gvrow.RowIndex].Cells[3].Text;


                    //con_bill_upload as bill_upload
                    fileExt = System.IO.Path.GetExtension(client_request_upload.FileName);
                    bill_upload1 = Path.GetFileName(client_request_upload.PostedFile.FileName);

                    string fname = null;
                    if (fileExt.ToUpper() == ".JPG" || fileExt.ToUpper() == ".PNG" || fileExt.ToUpper() == ".PDF" || fileExt.ToUpper() == ".JPEG" || fileExt.ToUpper() == ".ZIP")
                    {
                        string fileName = bill_upload1;
                        client_request_upload.PostedFile.SaveAs(Server.MapPath("~/client_request_upload/") + fileName);
                        if (ddl_type.SelectedValue == "3")
                        {
                            fname = Session["COMP_CODE"].ToString() + "_" + client_code + "_" + state_name + "_" + unit_code + "_" + material_type + "_" + id + "client_request_form" + fileExt;
                        }

                        File.Copy(Server.MapPath("~/client_request_upload/") + fileName, Server.MapPath("~/client_request_upload/") + fname, true);
                        File.Delete(Server.MapPath("~/client_request_upload/") + fileName);

                        int result = 0;

                        if (ddl_type.SelectedValue == "3")
                        {

                            result = d.operation("update pay_client_request_dispatch set client_re_upload = '" + fname + "' where comp_code = '" + Session["comp_code"].ToString() + "' and  client_code = '" + client_code + "' and state_name = '" + state_name + "' and unit_code = '" + unit_code + "' and id = '" + id + "'");
                        }

                        if (result > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Files uploaded Successfully... !!!');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Files uploading Failed... !!!');", true);
                        }


                    }



                }
            }

            dublicate_id_data_select();

        }
        catch (Exception ex) { throw ex; }
        finally { }

    }

    protected void lnk_material_download_Command(object sender, CommandEventArgs e)
    {
        string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
        GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow1.Cells[2].Text;


        //  ViewState["id"] = gv_material.SelectedRow.Cells[1].Text;
        string data = d.getsinglestring(" select client_re_upload from pay_client_request_dispatch where id = '" + id + "'  ");
        string filename = data;
        
        if (filename != "")
        {
            downloadfile(filename);
           
        }

        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attachment File Cannot Be Uploaded !!!')", true);
        }
    }

    protected void lnk_invoice_download_Command(object sender, CommandEventArgs e)
    {
        string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
        GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow1.Cells[2].Text;


        //  ViewState["id"] = gv_material.SelectedRow.Cells[1].Text;
        string data = d.getsinglestring(" select client_re_upload from pay_client_request_dispatch where id = '" + id + "'  ");
        string filename = data;

        if (filename != "")
        {
            downloadfile(filename);

        }

        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attachment File Cannot Be Uploaded !!!')", true);
        }

    }

    protected void lnk_dublicate_download_Command(object sender, CommandEventArgs e)
    {
        string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
        GridViewRow grdrow1 = (GridViewRow)((LinkButton)sender).NamingContainer;
        string id = grdrow1.Cells[2].Text;


        //  ViewState["id"] = gv_material.SelectedRow.Cells[1].Text;
        string data = d.getsinglestring(" select client_re_upload from pay_client_request_dispatch where id = '" + id + "'  ");
        string filename = data;

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

        //I03526_25.jpg
        //I03118_25
        try
        {
            var result = filename.Substring(filename.Length - 4);
            if (result.Contains("jpeg"))
            {
                result = ".jpeg";
            }

            string path2 = Server.MapPath("~\\client_request_upload\\" + filename);
            //  string unitName = stamp_copy + "-Attendance" + result;
            Response.Clear();
            Response.ContentType = "Application/pdf/jpg/jpeg/png/zip";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename);
            Response.TransmitFile("~\\client_request_upload\\" + filename);
            Response.WriteFile(path2);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Response.End();
            Response.Close();


        }
        catch (Exception ex) { }
    }

    
}