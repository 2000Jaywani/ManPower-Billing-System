using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

public partial class advance_policy : System.Web.UI.Page
{
    DAL d = new DAL();

    protected int result = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["comp_code"] == null || Session["comp_code"].ToString() == "")
        {
            Response.Redirect("Home.aspx");
        }
        databind(null, null);


        if (!IsPostBack)
        {
            vendor_name();
            grv_vendor1();
            ddl_client.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', client_code from pay_client_master where comp_code='" + Session["comp_code"] + "' AND  client_code in(select distinct(client_code) from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  pay_client_state_role_grade.emp_code IN (" + Session["REPORTING_EMP_SERIES"].ToString() + ")) ORDER BY client_code", d.con);
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
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }

            btn_Delet.Visible = false;
            //  btn_Update.Visible = false;
        }
        //gridView();
    }
    protected void databind(object sender, EventArgs e)
    {
        DataSet ds = new DataSet();
        // ds = d.getData("SELECT  distinct policy_id,  pay_client_master.client_name, pay_advance_policy.state_name, (SELECT UNIT_NAME FROM pay_unit_master WHERE unit_code = pay_advance_policy.branch_code AND comp_code = pay_advance_policy.COMP_CODE) AS 'branch_code', user_type, (SELECT CASE  Employee_type  WHEN 'Reliever' THEN CONCAT( emp_name , '-', 'Reliever') ELSE  emp_name  END AS 'EMP_NAME' FROM  pay_employee_master  WHERE  comp_code  =  pay_advance_policy . COMP_CODE  AND  EMP_CODE  =  pay_advance_policy . pay_to_emp_code ) AS 'emp_name', advance_service, amount,date, comment, pay_advance_policy.emp_code FROM pay_advance_policy inner join pay_client_state_role_grade on pay_client_state_role_grade.client_code = pay_advance_policy.client_code and pay_client_state_role_grade.state_name = pay_advance_policy.state_name and pay_client_state_role_grade.unit_code = pay_advance_policy.branch_code and  pay_client_state_role_grade.emp_code in (" + Session["REPORTING_EMP_SERIES"].ToString() + ") INNER JOIN pay_unit_master ON pay_unit_master.client_code = pay_advance_policy.client_code  AND pay_unit_master.unit_code = pay_advance_policy.branch_code inner join pay_client_master on pay_advance_policy.COMP_CODE=pay_client_master.comp_code and pay_advance_policy.client_code=pay_client_master.client_code WHERE pay_unit_master.branch_status = 0 group by  date ,pay_to_emp_code,amount   ");
        ds = d.getData("SELECT DISTINCT policy_id, pay_client_master.client_name, pay_advance_policy.state_name, UNIT_NAME AS 'branch_code', user_type, (SELECT CASE Employee_type WHEN 'Reliever' THEN CONCAT(emp_name, '-', 'Reliever') ELSE emp_name END AS 'EMP_NAME' FROM pay_employee_master WHERE comp_code = pay_advance_policy.COMP_CODE AND EMP_CODE = pay_advance_policy.pay_to_emp_code) AS 'emp_name', advance_service, amount, date, comment, pay_advance_policy.emp_code FROM pay_advance_policy INNER JOIN pay_client_master ON pay_advance_policy.COMP_CODE = pay_client_master.comp_code AND pay_advance_policy.client_code = pay_client_master.client_code INNER JOIN pay_unit_master ON  pay_unit_master.comp_code = pay_advance_policy.comp_code  AND pay_unit_master.client_code = pay_advance_policy.client_code  AND pay_unit_master.unit_code = pay_advance_policy.branch_code WHERE pay_advance_policy.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.branch_status = 0 GROUP BY date, pay_to_emp_code, amount");
        gv_client_bill.DataSource = ds.Tables[0];
        gv_client_bill.DataBind();

    }
    protected void GradeGridView_RowDataBound(object sender, GridViewRowEventArgs e)
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
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.gv_client_bill, "Select$" + e.Row.RowIndex);
        }
        e.Row.Cells[8].Visible = false;
        e.Row.Cells[0].Visible = false;
    }
    protected void onSelected_IndexChange(object sender, EventArgs e)
    {

        btn_add.Visible = false;
        System.Web.UI.WebControls.Label lbl_GRADE_CODE = (System.Web.UI.WebControls.Label)gv_client_bill.SelectedRow.FindControl("lbl_GRADE_CODE");
        string l_GRADE_CODE = lbl_GRADE_CODE.Text;
        d.con1.Open();
        try
        {
            MySqlCommand cmd = new MySqlCommand("SELECT COMP_CODE,client_code,state_name,branch_code,user_type,emp_code,advance_service,amount,date,comment,payment_mode,installation,pay_to_emp_code,remaing_advance  from pay_advance_policy where policy_id='" + l_GRADE_CODE + "'", d.con1);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                //policy_id.Text = dr.GetValue(0).ToString();
                ddl_client.SelectedValue = dr.GetValue(1).ToString();
                ddl_client_SelectedIndexChanged(null, null);
                ddl_state.SelectedValue = dr.GetValue(2).ToString();
                ddl_state_SelectedIndexChanged(null, null);
                ddl_user_type.SelectedValue = dr.GetValue(4).ToString();
                ddl_unit.SelectedValue = dr.GetValue(3).ToString();
                ddl_branch_SelectedIndexChanged(null, null);
                if (ddl_user_type.SelectedValue == "Field_Officer")
                {
                    ddl_emp_name.SelectedValue = dr.GetValue(5).ToString();
                }
                ddl_advance_serves.SelectedValue = dr.GetValue(6).ToString();
                txt_amount.Text = dr.GetValue(7).ToString();
                date_picker.Text = dr.GetValue(8).ToString();
                txt_comment.Text = dr.GetValue(9).ToString();
                ddl_payment_mode.SelectedValue = dr.GetValue(10).ToString();
                txt_installment.Text = dr.GetValue(11).ToString();
                ddl_advance_serves_SelectedIndexChanged(null, null);
                ddl_emp_name1.SelectedValue = dr.GetValue(12).ToString();
                txt_remaing.Text = dr.GetValue(13).ToString();
            }
            d.con.Dispose();
            d.con.Close();
        }
        catch (Exception ex)
        { throw ex; }
        finally
        {
            d.con.Close();

            btn_Delet.Visible = true;
            //  btn_Update.Visible = true;
        }


    }
    protected void btn_update_policy(object sender, EventArgs e)
    {
        System.Web.UI.WebControls.Label lbl_GRADE_CODE = (System.Web.UI.WebControls.Label)gv_client_bill.SelectedRow.FindControl("lbl_GRADE_CODE");
        string l_GRADE_CODE = lbl_GRADE_CODE.Text;
        try
        {
            //if (ddl_advance_serves.SelectedValue == "Self Advance")
            //{
            //    string advance = d.getsinglestring("Select EMP_ADVANCE_PAYMENT from pay_employee_master where comp_code='" + Session["COMP_CODE"].ToString() + "' and EMP_CODE='" + ddl_emp_name.SelectedValue + "'");               
            //    d.operation("Update pay_employee_master Set EMP_ADVANCE_PAYMENT='" + (double.Parse(txt_amount.Text) + double.Parse(advance)) + "' where comp_code='" + Session["COMP_CODE"].ToString() + "' and EMP_CODE='" + ddl_emp_name.SelectedValue + "'");
            //}
            result = d.operation("UPDATE pay_advance_policy SET user_type= '" + ddl_user_type.Text + "', advance_service='" + ddl_advance_serves.Text + "',amount='" + txt_amount.Text + "',date = str_to_date('" + date_picker.Text + "','%d/%m/%Y'),comment='" + txt_comment.Text + "',pay_to_emp_code='" + ddl_emp_name1.SelectedValue + "',remaing_advance='" + txt_remaing.Text + "' WHERE policy_id='" + l_GRADE_CODE + "'");
            if (result > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Policy Updated Succsefully');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "focus", "l_GRADE_CODE.focus();", true);
            }

            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Policy NOT updated !!');", true);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            databind(null, null);
            ddl_clear(null, null);
            d.con.Close();
            btn_Delet.Visible = false;
            //  btn_Update.Visible = false;
            btn_add.Visible = true;
        }
    }
    public void gridView()
    {
        DataSet ds = new DataSet();
        string daterange = "concat(upper(DATE_FORMAT(str_to_date('" + date_picker.Text.Substring(5) + "-" + date_picker.Text.Substring(3, 2) + "-01','%Y-%m-%d'), '%D %b %Y')),' TO ',upper(DATE_FORMAT(LAST_DAY('" + date_picker.Text.Substring(5) + "-" + date_picker.Text.Substring(3, 2) + "-01'), '%D %b %Y'))) as fromtodate";
        d.con.Open();
        MySqlDataAdapter adp = new MySqlDataAdapter("SELECT client, SUM((Total + pf + esic)) AS 'Amount', SUM((((Total + pf + esic) * bill_service_charge) / 100)) AS 'Service_charge', SUM(CASE WHEN company_state = state_name THEN ROUND(((((Total + pf + esic + operational_cost + uniform) + (((Total + pf + esic) * bill_service_charge) / 100)) * 9) / 100), 2) ELSE 0 END) AS 'CGST9', SUM( CASE WHEN company_state != state_name THEN ROUND(((((Total + pf + esic + operational_cost + uniform) + (((Total + pf + esic) * bill_service_charge) / 100)) * 18) / 100), 2) ELSE 0 END) AS 'IGST18', SUM(CASE WHEN company_state = state_name THEN ROUND(((((Total + pf + esic + operational_cost + uniform) + (((Total + pf + esic) * bill_service_charge) / 100)) * 9) / 100), 2) ELSE 0 END) AS 'SGST9', " + daterange + ", SUM(sub_total_c) as 'total' FROM (SELECT client, company_state,unit_name ,state_name, unit_city, client_branch_code, emp_name, grade_desc, emp_basic_vda, hra, bonus_gross, leave_gross, gratuity_gross, washing, travelling, education, allowances, cca_billing, other_allow, (emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross+ hrs_12_ot) AS 'gross', bonus_after_gross, leave_after_gross, gratuity_after_gross, (((emp_basic_vda) / 100) * pf_percent) AS 'pf', (((emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross+ hrs_12_ot) / 100) * esic_percent) AS 'esic', hrs_12_ot, (((hrs_12_ot) * esic_percent) / 100) AS 'esic_ot', lwf, CASE WHEN bill_ser_uniform = 1 THEN 0 ELSE uniform END AS 'uniform',relieving_charg, CASE WHEN bill_ser_operations = 1 THEN 0 ELSE operational_cost END AS 'operational_cost', tot_days_present, (emp_basic_vda + hra + bonus_gross + leave_gross + washing + travelling + education + allowances + cca_billing + other_allow + gratuity_gross + bonus_after_gross + leave_after_gross + gratuity_after_gross + lwf + CASE WHEN bill_ser_uniform = 0 THEN 0 ELSE uniform END + relieving_charg + CASE WHEN bill_ser_operations = 0 THEN 0 ELSE operational_cost END + NH+ hrs_12_ot) AS 'Total', bill_service_charge, NH,hours,sub_total_c,bill_ser_uniform,bill_ser_operations FROM (SELECT (SELECT client_name FROM pay_client_master WHERE client_code = pay_unit_master.client_code AND comp_code = '" + Session["COMP_CODE"].ToString() + "') AS 'client', pay_company_master.state as 'company_state',pay_unit_master.unit_name, pay_unit_master.state_name, pay_unit_master.unit_city, pay_unit_master.client_branch_code, pay_employee_master.emp_name, pay_grade_master.grade_desc, (((pay_billing_master_history.basic + pay_billing_master_history.vda) / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'emp_basic_vda', ((pay_billing_unit_rate.hra / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'hra', CASE WHEN bonus_taxable = '1' THEN ((pay_billing_unit_rate.bonus_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'bonus_gross', CASE WHEN bonus_taxable = '0' THEN ((pay_billing_unit_rate.bonus_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'bonus_after_gross', CASE WHEN leave_taxable = '1' THEN ((pay_billing_unit_rate.leave_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'leave_gross', CASE WHEN leave_taxable = '0' THEN ((pay_billing_unit_rate.leave_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'leave_after_gross', CASE WHEN gratuity_taxable = '1' THEN ((pay_billing_unit_rate.grauity_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'gratuity_gross', CASE WHEN gratuity_taxable = '0' THEN ((pay_billing_unit_rate.grauity_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'gratuity_after_gross', ((pay_billing_unit_rate.washing / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'washing', ((pay_billing_unit_rate.traveling / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'travelling', ((pay_billing_unit_rate.education / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'education', ((pay_billing_unit_rate.national_holiday_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'NH', ((pay_billing_unit_rate.allowances / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'allowances', CASE WHEN pay_employee_master.cca = 0 THEN ((pay_billing_unit_rate.cca / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE pay_employee_master.cca END AS 'cca_billing', CASE WHEN pay_employee_master.special_allow = 0 THEN ((pay_billing_master_history.other_allow / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE pay_employee_master.special_allow END AS 'other_allow', CASE WHEN pay_billing_master_history.ot_policy_billing ='1' THEN ((pay_billing_master_history.ot_amount_billing / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) ELSE 0 END AS 'hrs_12_ot', pay_billing_master_history.bill_esic_percent AS 'esic_percent', pay_billing_master_history.bill_pf_percent AS 'pf_percent', ((pay_billing_unit_rate.lwf / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'lwf', ((pay_billing_unit_rate.uniform / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'uniform', ((pay_billing_unit_rate.relieving_amount / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'relieving_charg', ((pay_billing_unit_rate.operational_cost / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present) AS 'operational_cost', pay_attendance_muster.tot_days_present, ROUND(((pay_billing_unit_rate.sub_total_c / pay_billing_unit_rate.month_days) * pay_attendance_muster.tot_days_present), 2) AS 'baseamount', bill_service_charge,	pay_billing_master_history.hours,pay_billing_unit_rate.sub_total_c,	pay_billing_master_history.bill_ser_operations,	pay_billing_master_history.bill_ser_uniform FROM pay_employee_master INNER JOIN pay_attendance_muster ON pay_attendance_muster.emp_code = pay_employee_master.emp_code AND pay_attendance_muster.comp_code = pay_employee_master.comp_code INNER JOIN pay_unit_master ON pay_attendance_muster.unit_code = pay_unit_master.unit_code AND pay_attendance_muster.comp_code = pay_unit_master.comp_code INNER JOIN pay_billing_unit_rate ON pay_attendance_muster.unit_code = pay_billing_unit_rate.unit_code AND pay_attendance_muster.month = pay_billing_unit_rate.month AND pay_attendance_muster.year = pay_billing_unit_rate.year INNER JOIN pay_billing_master_history ON pay_billing_master_history.comp_code = pay_billing_unit_rate.comp_code AND  pay_billing_master_history.comp_code = pay_employee_master.comp_code AND pay_billing_master_history.billing_client_code = pay_billing_unit_rate.client_code AND pay_billing_master_history.billing_unit_code = pay_billing_unit_rate.unit_code AND pay_billing_master_history.month = pay_billing_unit_rate.month AND pay_billing_master_history.year = pay_billing_unit_rate.year AND pay_employee_master.grade_code = pay_billing_master_history.designation AND pay_billing_master_history.designation = pay_billing_unit_rate.designation AND pay_billing_master_history.hours = pay_billing_unit_rate.hours INNER JOIN pay_company_master ON pay_employee_master.comp_code = pay_company_master.comp_code INNER JOIN pay_grade_master ON pay_billing_master_history.comp_code = pay_grade_master.comp_code AND pay_billing_master_history.designation = pay_grade_master.GRADE_CODE WHERE pay_company_master.comp_code = '" + Session["comp_code"].ToString() + "' and pay_unit_master.client_code = '" + ddl_client.SelectedValue + "' and pay_unit_master.state_name = '" + ddl_state.SelectedValue + "' and pay_attendance_muster.month = '" + date_picker.Text.Substring(3, 2) + "' and pay_attendance_muster.Year = '" + date_picker.Text.Substring(5) + "' and pay_attendance_muster.tot_days_present > 0) AS billing_table) as Final_billing group by client", d.con);
        adp.Fill(ds);
        gv_client_bill.DataSource = ds.Tables[0];
        gv_client_bill.DataBind();
        d.con.Close();
    }
    protected void btn_add_Click(object sender, EventArgs e)
    {
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            // cumment  date 26/09/2019
            //string val = date_picker.Text.ToString();
            // string advance = d.getsinglestring("Select EMP_ADVANCE_PAYMENT from pay_employee_master where comp_code='" + Session["COMP_CODE"].ToString() + "' and EMP_CODE='" + ddl_emp_name.SelectedValue + "'");
            // result = d.operation("INSERT INTO pay_advance_policy(COMP_CODE,client_code,state_name,branch_code,user_type,emp_name,advance_service,amount,date,comment) VALUES('" + Session["COMP_CODE"].ToString() + "','" + ddl_client.SelectedValue + "','" + ddl_state.SelectedValue + "','" + ddl_unit.SelectedValue + "','" + ddl_user_type.SelectedValue + "','" + ddl_emp_name.SelectedValue + "','" + ddl_advance_serves.SelectedValue + "','" + txt_amount.Text + "','"+ date_picker.Text+ "','" + txt_comment.Text + "');");
            //if (ddl_advance_serves.SelectedValue=="Self Advance")
            // {
            //if (checkemp())
            //{
            //      result = d.operation("INSERT INTO pay_advance_policy(COMP_CODE,client_code,state_name,branch_code,user_type,emp_code,advance_service,amount,date,comment,status,payment_mode,installation,pay_to_emp_code,remaing_advance) VALUES('" + Session["COMP_CODE"].ToString() + "','" + ddl_client.SelectedValue + "','" + ddl_state.SelectedValue + "','" + ddl_unit.SelectedValue + "','" + ddl_user_type.SelectedValue + "','" + ddl_emp_name.SelectedValue + "','" + ddl_advance_serves.SelectedValue + "','" + txt_amount.Text + "', str_to_date('" + date_picker.Text + "','%d/%m/%Y') ,'" + txt_comment.Text + "','ADVANCE','" + ddl_payment_mode.SelectedValue + "','" + txt_installment.Text + "','" + ddl_emp_name1.SelectedValue + "','" + txt_remaing.Text+ "');");
            //}
            //else 
            //{
            //    d.operation("Update pay_advance_policy Set amount='" + txt_amount.Text + "' where comp_code='" + Session["COMP_CODE"].ToString() + "' and EMP_CODE='" + ddl_emp_name.SelectedValue + "' and advance_service='" + ddl_advance_serves.SelectedValue + "'");
            //}
            //d.operation("Update pay_employee_master Set EMP_ADVANCE_PAYMENT='" + (double.Parse(txt_amount.Text) + double.Parse(advance)) + "' , advance_payment_mode = '" + ddl_payment_mode.SelectedValue + "' , Installment='"+ txt_installment.Text +"' where comp_code='" + Session["COMP_CODE"].ToString() + "' and EMP_CODE='" + ddl_emp_name.SelectedValue + "'");
            //   if (result > 0)
            //  {
            //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Policy created succsefully  !!!');", true);
            //    }
            //    else
            //    {
            //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Policy  NOT Created !!!');", true);
            //    }
            //}
            //else if (ddl_advance_serves.SelectedValue != "Self Advance")
            //{
            //    if (checkemp())
            //    {
            //        result = d.operation("INSERT INTO pay_advance_policy(COMP_CODE,client_code,state_name,branch_code,user_type,emp_code,advance_service,amount,date,comment,status,payment_mode,installation,pay_to_emp_code,remaing_advance) VALUES('" + Session["COMP_CODE"].ToString() + "','" + ddl_client.SelectedValue + "','" + ddl_state.SelectedValue + "','" + ddl_unit.SelectedValue + "','" + ddl_user_type.SelectedValue + "','" + ddl_emp_name.SelectedValue + "','" + ddl_advance_serves.SelectedValue + "','" + txt_amount.Text + "', str_to_date('" + date_picker.Text + "','%d/%m/%Y') ,'" + txt_comment.Text + "','ADVANCE','" + ddl_payment_mode.SelectedValue + "','" + txt_installment.Text + "','" + ddl_emp_name1.SelectedValue + "','" + txt_remaing.Text + "');");
            //    }
            //    else
            //    {
            //        d.operation("Update pay_advance_policy Set amount='" + txt_amount.Text + "' where comp_code='" + Session["COMP_CODE"].ToString() + "' and EMP_CODE='" + ddl_emp_name.SelectedValue + "' and advance_service='" + ddl_advance_serves.SelectedValue + "'");
            //    }
            //    if (result > 0)
            //    {
            //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Policy created succsefully  !!!');", true);
            //    }
            //    else
            //    {
            //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Policy  NOT Created !!!');", true);
            //    }
            //}
            //else
            //{
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Advance is already given to "+ ddl_emp_name.SelectedItem.Text +" !!!');", true);
            //}

            int instalment = int.Parse(txt_installment.Text);
            int month = int.Parse(date_picker.Text.Substring(3, 2));
            int year = int.Parse(date_picker.Text.Substring(6));
            double deduction_amount = 0;
            if (month == 13) { month = 1; year = year + 1; }
            if (ddl_advance_serves.SelectedValue == "Self Advance")
            {
                if (ddl_payment_mode.SelectedValue == "1")
                {
                    string check = d.getsinglestring("SELECT GROUP_CONCAT( month , '/',  year ) FROM  pay_advance_policy  WHERE  pay_to_emp_code  = '" + ddl_emp_name1.SelectedValue + "' AND  pay_flag  = '1' AND month = (SELECT MAX( month ) FROM  pay_advance_policy  WHERE  pay_to_emp_code  = '" + ddl_emp_name1.SelectedValue + "' AND  pay_flag  = '1')");

                    if (check != "")
                    {
                        month = int.Parse(check.Substring(0, 1).ToString());
                        year = int.Parse(check.Substring(2).ToString());
                        if (month == 12)
                        {
                            year++;
                            month = 01;
                        }
                        else
                        {
                            month++;
                        }
                    }
                    for (int i = 1; i <= instalment; i++)
                    {

                        deduction_amount = double.Parse(txt_amount.Text) / instalment;
                        result = d.operation("INSERT INTO pay_advance_policy(COMP_CODE,client_code,state_name,branch_code,user_type,emp_code,advance_service,amount,date,comment,status,payment_mode,installation,pay_to_emp_code,remaing_advance,month,year,deduction_amount) VALUES('" + Session["COMP_CODE"].ToString() + "','" + ddl_client.SelectedValue + "','" + ddl_state.SelectedValue + "','" + ddl_unit.SelectedValue + "','" + ddl_user_type.SelectedValue + "','" + ddl_emp_name.SelectedValue + "','" + ddl_advance_serves.SelectedValue + "','" + txt_amount.Text + "', '" + date_picker.Text + "' ,'" + txt_comment.Text + "','Self ADVANCE','" + ddl_payment_mode.SelectedValue + "','" + txt_installment.Text + "','" + ddl_emp_name1.SelectedValue + "','" + txt_remaing.Text + "'," + month + "," + year + "," + deduction_amount + ");");
                        if (month == 12)
                        {
                            year++;
                            month = 01;
                        }
                        else
                        {
                            month++;
                        }
                    }

                }
                else
                {
                    deduction_amount = double.Parse(txt_amount.Text);
                    result = d.operation("INSERT INTO pay_advance_policy(COMP_CODE,client_code,state_name,branch_code,user_type,emp_code,advance_service,amount,date,comment,status,payment_mode,installation,pay_to_emp_code,remaing_advance,month,year,deduction_amount) VALUES('" + Session["COMP_CODE"].ToString() + "','" + ddl_client.SelectedValue + "','" + ddl_state.SelectedValue + "','" + ddl_unit.SelectedValue + "','" + ddl_user_type.SelectedValue + "','" + ddl_emp_name.SelectedValue + "','" + ddl_advance_serves.SelectedValue + "','" + txt_amount.Text + "', '" + date_picker.Text + "' ,'" + txt_comment.Text + "','Self Advance','" + ddl_payment_mode.SelectedValue + "','" + txt_installment.Text + "','" + ddl_emp_name1.SelectedValue + "','" + txt_remaing.Text + "'," + month + "," + year + "," + deduction_amount + ");");
                }
            }
            else if (ddl_advance_serves.SelectedValue == "Reliver Payment")
            {
                deduction_amount = double.Parse(txt_amount.Text);
                result = d.operation("INSERT INTO pay_advance_policy(COMP_CODE,client_code,state_name,branch_code,user_type,emp_code,advance_service,amount,date,comment,status,payment_mode,installation,pay_to_emp_code,remaing_advance,month,year,deduction_amount) VALUES('" + Session["COMP_CODE"].ToString() + "','" + ddl_client.SelectedValue + "','" + ddl_state.SelectedValue + "','" + ddl_unit.SelectedValue + "','" + ddl_user_type.SelectedValue + "','" + ddl_emp_name.SelectedValue + "','" + ddl_advance_serves.SelectedValue + "','" + txt_amount.Text + "', '" + date_picker.Text + "','" + txt_comment.Text + "','Reliver Advance','" + ddl_payment_mode.SelectedValue + "','" + txt_installment.Text + "','" + ddl_emp_name1.SelectedValue + "','" + txt_remaing.Text + "'," + month + "," + year + "," + deduction_amount + ");");

            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            databind(null, null);
            ddl_clear(null, null);
            d.con.Close();
        }
    }
    protected void ddl_client_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_client.SelectedValue != "Select")
        {
            //State
            ddl_state.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("select distinct(STATE_NAME) from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name in(select state_name from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  pay_client_state_role_grade.emp_code IN (" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_client.SelectedValue + "') order by 1", d.con);
            d.con.Open();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
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
                ddl_state.Items.Insert(0, "Select");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
        else
        {
        }

    }
    protected void ddl_state_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddl_unit.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME, unit_code from pay_unit_master where comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_client.SelectedValue + "' and state_name='" + ddl_state.SelectedValue + "'  and UNIT_CODE  in(select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  pay_client_state_role_grade.emp_code IN (" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_client.SelectedValue + "' AND state_name='" + ddl_state.SelectedValue + "') AND branch_status = 0 ORDER BY UNIT_CODE", d.con);
        d.con.Open();
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_unit.DataSource = dt_item;
                ddl_unit.DataTextField = dt_item.Columns[0].ToString();
                ddl_unit.DataValueField = dt_item.Columns[1].ToString();
                ddl_unit.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
            ddl_unit.Items.Insert(0, "Select");
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
    protected void ddl_branch_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_user_type.SelectedValue == "Employee")
        {
            emp_hide.Visible = false;
            //try
            //{                
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            //    System.Data.DataTable dt_item = new System.Data.DataTable();
            //    MySqlDataAdapter cmd_item = new MySqlDataAdapter("select (SELECT CASE Employee_type WHEN 'Reliever' THEN CONCAT(emp_name, '-', 'Reliever') ELSE emp_name END) AS 'EMP_NAME',emp_code from pay_employee_master where unit_code='" + ddl_unit.SelectedValue + "' and comp_code='" + Session["comp_code"].ToString() + "' ORDER BY EMP_CODE", d.con);          
            //    cmd_item.Fill(dt_item);
            //if (dt_item.Rows.Count > 0)
            //{
            //    ddl_emp_name.DataSource = dt_item;
            //    ddl_emp_name.DataTextField = dt_item.Columns[0].ToString();
            //    ddl_emp_name.DataValueField = dt_item.Columns[1].ToString();
            //    ddl_emp_name.DataBind();
            //}
            //    dt_item.Dispose();
            //    d.con.Close();
            //}
            //catch (Exception ex) { throw ex; }
            //finally
            //{
            //    d.con.Close();
            //}
        }
        if (ddl_user_type.SelectedValue == "Field_Officer")
        {
            emp_hide.Visible = true;
            try
            {
                ddl_emp_name.Items.Clear();
                System.Data.DataTable dt_item = new System.Data.DataTable();
                //reliver change
                MySqlDataAdapter cmd_item = new MySqlDataAdapter("select field_officer_name,emp_code from pay_op_management where unit_code='" + ddl_unit.SelectedValue + "' and comp_code='" + Session["comp_code"].ToString() + "' ", d.con);
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_emp_name.DataSource = dt_item;
                    ddl_emp_name.DataTextField = dt_item.Columns[0].ToString();
                    ddl_emp_name.DataValueField = dt_item.Columns[1].ToString();
                    ddl_emp_name.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
        if (ddl_user_type.SelectedValue == "Vendor")
        {
            emp_hide.Visible = true;
            try
            {
                ddl_emp_name.Items.Clear();
                System.Data.DataTable dt_item = new System.Data.DataTable();
                MySqlDataAdapter cmd_item = new MySqlDataAdapter("select VEND_NAME from pay_vendor_master where  comp_code='" + Session["comp_code"].ToString() + "'", d.con);
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_emp_name.DataSource = dt_item;
                    ddl_emp_name.DataValueField = dt_item.Columns[0].ToString();
                    ddl_emp_name.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
    }
    protected void ddl_clear(object sender, EventArgs e)
    {
        try
        {
            ddl_client.SelectedValue = "Select";
            ddl_emp_name.Items.Clear();
            ddl_state.Items.Clear();
            ddl_unit.Items.Clear();
            ddl_user_type.SelectedValue = "Select";
            ddl_emp_name.Items.Clear();
            ddl_advance_serves.SelectedValue = "Select";
            txt_amount.Text = "";
            txt_comment.Text = "";
            date_picker.Text = "";
            ddl_payment_mode.SelectedValue = "0";
            txt_installment.Text = "0";
            txt_remaing.Text = "0";
            ddl_emp_name1.SelectedValue = "Select";
        }
        catch (Exception ex) { throw ex; }
        finally
        {
        }
    }
    protected void ddl_delete_policy(object sender, EventArgs e)
    {

        // System.Web.UI.WebControls.Label lbl_GRADE_CODE = (System.Web.UI.WebControls.Label)gv_client_bill.SelectedRow.FindControl("lbl_GRADE_CODE");
        //string l_GRADE_CODE = lbl_GRADE_CODE.Text;
        try
        {
            string check = d.getsinglestring("select * from pay_advance_policy where client_code='" + ddl_client.SelectedValue + "' and branch_code='" + ddl_unit.SelectedValue + "' and  pay_to_emp_code ='" + ddl_emp_name1.SelectedValue + "' and  date ='" + date_picker.Text + "' and pay_flag='1'");
            if (check == "")
            {
                result = d.operation("DELETE FROM pay_advance_policy WHERE client_code='" + ddl_client.SelectedValue + "' and branch_code='" + ddl_unit.SelectedValue + "' and  pay_to_emp_code ='" + ddl_emp_name1.SelectedValue + "' and  date ='" + date_picker.Text + "' and amount='" + txt_amount.Text + "'");
                if (result > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Policy Deleted Succsefully !!');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Policy NOT Deleted !!');", true);
            }
            //result = d.operation("DELETE FROM pay_advance_policy WHERE policy_id='"+l_GRADE_CODE+"'");
            //  if (result > 0)
            //      {
            //         ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Policy Deleted Succsefully !!');", true);
            //      }
            //else  {
            //         ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Policy NOT Deleted !!');", true);
            //      }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            // btn_Update.Visible = false;
            btn_add.Visible = true;
            databind(null, null);
            ddl_clear(null, null);
            d.con.Close();
        }
    }
    protected void btnclose_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }
    protected void gv_client_bill_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_client_bill.UseAccessibleHeader = false;
            gv_client_bill.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
    }
    private bool checkemp()
    {
        foreach (GridViewRow row in gv_client_bill.Rows)
        {
            if (row.Cells[8].Text == ddl_emp_name.SelectedValue && row.Cells[4].Text == ddl_advance_serves.SelectedValue)
            {
                return false;
            }
        }
        return true;
    }
    protected void ddl_advance_serves_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddl_emp_name1.Items.Clear();
        if (ddl_advance_serves.SelectedValue == "Reliver Payment")
        {
            try
            {
                //ddl_emp_name.Items.Clear();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                System.Data.DataTable dt_item = new System.Data.DataTable();

                MySqlDataAdapter cmd_item = new MySqlDataAdapter("select (SELECT CASE Employee_type WHEN 'Reliever' THEN CONCAT(emp_name, '-', 'Reliever') ELSE emp_name END) AS 'EMP_NAME',emp_code from pay_employee_master where unit_code='" + ddl_unit.SelectedValue + "' and comp_code='" + Session["comp_code"].ToString() + "' and Employee_type='Reliever' and (left_date is null or left_date ='') ORDER BY EMP_CODE", d.con);

                cmd_item.Fill(dt_item);

                if (dt_item.Rows.Count > 0)
                {
                    ddl_emp_name1.DataSource = dt_item;
                    ddl_emp_name1.DataTextField = dt_item.Columns[0].ToString();
                    ddl_emp_name1.DataValueField = dt_item.Columns[1].ToString();
                    ddl_emp_name1.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_emp_name1.Items.Insert(0, "Select");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
        else
        {
            try
            {
                //ddl_emp_name.Items.Clear();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                System.Data.DataTable dt_item = new System.Data.DataTable();

                MySqlDataAdapter cmd_item = new MySqlDataAdapter("select (SELECT CASE Employee_type WHEN 'Reliever' THEN CONCAT(emp_name, '-', 'Reliever') ELSE emp_name END) AS 'EMP_NAME',emp_code from pay_employee_master where unit_code='" + ddl_unit.SelectedValue + "' and comp_code='" + Session["comp_code"].ToString() + "' and Employee_type !='Reliever' and ( left_date is null or left_date ='') ORDER BY EMP_CODE", d.con);

                cmd_item.Fill(dt_item);

                if (dt_item.Rows.Count > 0)
                {
                    ddl_emp_name1.DataSource = dt_item;
                    ddl_emp_name1.DataTextField = dt_item.Columns[0].ToString();
                    ddl_emp_name1.DataValueField = dt_item.Columns[1].ToString();
                    ddl_emp_name1.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_emp_name1.Items.Insert(0, "Select");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }
    }
    protected void btn_advreport_Click(object sender, EventArgs e)
    {
        try
        {
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT  client_name AS 'client_code',pay_unit_master.unit_name AS 'branch_code',pay_pro_master.emp_name as 'pay_to_emp_code',amount,remaing_advance,CONCAT(pay_pro_master.month, '/', pay_pro_master.year) AS 'month_year',emp_advance FROM  pay_advance_policy LEFT JOIN pay_pro_master ON pay_advance_policy.pay_to_emp_code = pay_pro_master.emp_code INNER JOIN pay_client_master ON pay_advance_policy.comp_code = pay_client_master.comp_code AND pay_advance_policy.client_code = pay_client_master.client_code INNER JOIN pay_unit_master ON pay_advance_policy.comp_code = pay_unit_master.comp_code AND pay_advance_policy.branch_code = pay_unit_master.unit_code WHERE pay_advance_policy.comp_code = '" + Session["comp_code"].ToString() + "' AND pay_to_emp_code = '" + ddl_emp_name1.SelectedValue + "'   and emp_advance!='0'", d.con);
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                gv_empadvance.DataSource = dt_item;
                gv_empadvance.DataBind();
            }
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
    protected void vendor_name()
    {
        try
        {
            ddl_vendor.Items.Clear();
            DataTable dt = new DataTable();
            d.con.Open();
            MySqlDataAdapter dr = new MySqlDataAdapter("select  VEND_ID ,concat( VEND_ID ,' - ', VEND_NAME ) from pay_vendor_master where comp_code='" + Session["comp_code"].ToString() + "' and  ACTIVE_STATUS ='A' group by VEND_ID ", d.con);
            dr.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ddl_vendor.DataSource = dt;
                ddl_vendor.DataTextField = dt.Columns[1].ToString();
                ddl_vendor.DataValueField = dt.Columns[0].ToString();
                ddl_vendor.DataBind();
            }
            ddl_vendor.Items.Insert(0, "Select");
            d.con.Close();
            dt.Dispose();
        }
        catch (Exception ex)
        {
        }
        finally
        {
            d.con.Close();
        }
    }
    protected void ddl_vendor_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        try
        {
            hidtab.Value = "1";
            ddl_po.Items.Clear();
            d.con.Open();
            DataTable dt = new DataTable();
            MySqlDataAdapter dr = new MySqlDataAdapter("select po_no from pay_transaction_po where comp_code='" + Session["comp_code"].ToString() + "' and CUST_NAME='" + ddl_vendor.SelectedItem.Text + "' group by po_no", d.con);
            dr.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ddl_po.DataSource = dt;
                ddl_po.DataTextField = dt.Columns[0].ToString();
                ddl_po.DataValueField = dt.Columns[0].ToString();
                ddl_po.DataBind();
            }
            ddl_po.Items.Insert(0, "Select");
            dt.Dispose();
            d.con.Close();
        }
        catch (Exception ex)
        { }
        finally { d.con.Close(); }
    }
    protected void ddl_po_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            hidtab.Value = "1";
            ddl_invoice.Items.Clear();
            d.con.Open();
            DataTable dt = new DataTable();
            MySqlDataAdapter dr = new MySqlDataAdapter("select distinct DOC_NO from pay_transactionp where comp_code='" + Session["comp_code"].ToString() + "' and  CUST_CODE ='" + ddl_vendor.SelectedValue + "' and  pur_order_no ='" + ddl_po.SelectedValue + "'", d.con);
            dr.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ddl_invoice.DataSource = dt;
                ddl_invoice.DataTextField = dt.Columns[0].ToString();
                ddl_invoice.DataValueField = dt.Columns[0].ToString();
                ddl_invoice.DataBind();
            }
            ddl_invoice.Items.Insert(0, "Select");
            dt.Dispose();
            d.con.Close();
        }
        catch (Exception ex)
        {
        }
        finally
        {
            d.con.Close();
        }

    }
    protected void btn_vendor_save_Click(object sender, EventArgs e)
    {

        hidtab.Value = "1";
        int result = 0;
        result = d.operation("insert into pay_vendor_advance(comp_code,vendor_code,vendor_name,po_no,invoice_no,amount,date,description,payment_mode,cheque_no,cheque_received_by,utr_no,month,year)value('" + Session["comp_code"].ToString() + "','" + ddl_vendor.SelectedValue + "','" + ddl_vendor.SelectedItem.Text + "','" + ddl_po.SelectedValue + "','" + ddl_invoice.SelectedValue + "','" + txt_vamount.Text + "','" + txt_vdate.Text + "','" + txt_des.Text + "','" + ddl_pay_mod.SelectedValue + "','" + txt_cheq_no.Text + "','" + txt_cheq_rby.Text + "','" + txt_utr_no.Text + "','" + txt_vdate.Text.Substring(3, 2) + "','" + txt_vdate.Text.Substring(6) + "')");
        if (result > 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Vendor Advance Save Succsefully');", true);
            clear();
            grv_vendor1();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Vendor Advance Not Save Succsefully');", true);
        }
    }
    protected void clear()
    {
        ddl_vendor.SelectedValue = "Select";
        ddl_po.SelectedValue = "Select";
        ddl_invoice.SelectedValue = "Select";
        txt_vamount.Text = "0";
        txt_vdate.Text = "";
        txt_des.Text = "";
        ddl_pay_mod.SelectedValue = "0";
        txt_cheq_no.Text = "";
        txt_cheq_rby.Text = "";
        txt_utr_no.Text = "";
        txt_id.Text = "";

    }
    protected void grv_vendor1()
    {
        try
        {
            grv_vendor.DataSource = null;
            grv_vendor.DataBind();
            d.con.Open();
            DataTable dt = new DataTable();
            MySqlDataAdapter dr = new MySqlDataAdapter("select  id,vendor_name,po_no,invoice_no,amount,date from pay_vendor_advance ", d.con);
            dr.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                grv_vendor.DataSource = dt;
                grv_vendor.DataBind();
            }
            d.con.Close();
        }
        catch (Exception ex) { throw; }
        finally { d.con.Close(); }
    }
    protected void grv_vendor_RowDataBound(object sender, GridViewRowEventArgs e)
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
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grv_vendor, "Select$" + e.Row.RowIndex);
        }
        e.Row.Cells[0].Visible = false;
    }
    protected void grv_vendor_SelectedIndexChanged(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        btn_vendor_save.Visible = false;
        try
        {
            d.con1.Open();
            DataTable dt = new DataTable();
            //   System.Web.UI.WebControls.Label lbl_GRADE_CODE = (System.Web.UI.WebControls.Label)gv_client_bill.SelectedRow.FindControl("lbl_GRADE_CODE");
            string id = grv_vendor.SelectedRow.Cells[0].Text;

            MySqlCommand cmd = new MySqlCommand("select  vendor_code,po_no,invoice_no,amount,date,description,payment_mode,cheque_no,cheque_received_by,utr_no,id  from pay_vendor_advance where id='" + id + "'", d.con1);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                ddl_vendor.SelectedValue = dr.GetValue(0).ToString();
                ddl_vendor_SelectedIndexChanged(null, null);
                ddl_po.SelectedValue = dr.GetValue(1).ToString();
                ddl_po_SelectedIndexChanged(null, null);
                ddl_invoice.SelectedValue = dr.GetValue(2).ToString();
                txt_vamount.Text = dr.GetValue(3).ToString();
                txt_vdate.Text = dr.GetValue(4).ToString();
                txt_des.Text = dr.GetValue(5).ToString();
                ddl_pay_mod.SelectedValue = dr.GetValue(6).ToString();
                txt_cheq_no.Text = dr.GetValue(7).ToString();
                txt_cheq_rby.Text = dr.GetValue(8).ToString();
                txt_utr_no.Text = dr.GetValue(9).ToString();
                txt_id.Text = dr.GetValue(10).ToString();
            }
            d.con1.Close();
        }
        catch (Exception ex) { throw; }
        finally
        {
            d.con1.Close();
        }
    }
    protected void btn_vendor_delete_Click(object sender, EventArgs e)
    {
        hidtab.Value = "1";
        int result = 0;
        string amount = d.getsinglestring("select  pay_flag  from pay_vendor_advance  where id='" + txt_id.Text + "'");
        if (amount == "1")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Vendor Advance already deduct cant delete record');", true);
            return;
        }
        result = d.operation("delete from pay_vendor_advance where id='" + txt_id.Text + "'");
        if (result > 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Vendor Advance Delete Succsefully');", true);
            clear();
            grv_vendor1();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Vendor Advance Not Delete Succsefully');", true);
        }
    }
    protected void btn_upload_Click(object sender, EventArgs e)
    {
        string fileExt = System.IO.Path.GetExtension(FileUpload1.FileName);

            string fname = null;

            string csvPath = "";
            string FilePath = "";
            if (FileUpload1.HasFile)
            {
                try
                {
                        string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                        string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                        string FolderPath = "~/unit_upload/";
                        FilePath = Server.MapPath(FolderPath + FileName);
                        if (Extension == ".csv")
                        {
                           // csvPath = "~/unit_upload/";
                            //csvPath = Server.MapPath(csvPath + FileName);
                            FileUpload1.SaveAs(FilePath);
                            btn_Import_Click(FilePath, Extension, "Yes", FileName, fname);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('File Uploaded Successfully...');", true);
                            File.Delete(FilePath);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please upload a valid csv file.');", true);
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

    public void btn_Import_Click(string FilePath, string Extension, string IsHDR, string filename, string fname)
    {

        FileUpload1.SaveAs(FilePath);

        //Create a DataTable.  
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[17] 
            { 
                new DataColumn("SR NO", typeof(string)),
        new DataColumn("Client Name", typeof(string)),  
        new DataColumn("State Name", typeof(string)),  
        new DataColumn("Unit code", typeof(string)),  
        new DataColumn("User Type",typeof(string)),
        new DataColumn("Advanced For Service Type",typeof(string)),
        new DataColumn("Employee Code",typeof(string)),
        new DataColumn("Deduction Amount",typeof(string)),
        new DataColumn("Date",typeof(string)),
        new DataColumn("Payment Mode",typeof(string)),
          new DataColumn("Comment",typeof(string)),
            new DataColumn("status",typeof(string)),
              new DataColumn("Installation",typeof(string)),
                new DataColumn("Remaining advanced",typeof(string)),
                  new DataColumn("month",typeof(string)),
                    new DataColumn("year",typeof(string)),
                      new DataColumn("deduction Amount",typeof(string))
      
           
        });

        //Read the contents of CSV file.  
        string csvData = File.ReadAllText(FilePath);

        string csvdata1 = csvData;
        //Execute a loop over the rows.  
        foreach (string row in csvData.Split('\n'))
        {
            //csvdata1 = csvdata1.Replace(row + "\n", "");
            //csvdata1 = csvdata1.Replace(row + "\r", "");
            if (!string.IsNullOrEmpty(row))
            {
                dt.Rows.Add();
                int i = 0;
                string row1 = row.Replace("\r", "");
                //Execute a loop over the columns.  
                foreach (string cell in row1.Split(','))
                {

                    dt.Rows[dt.Rows.Count - 1][i] = cell;
                    i++;
                }
            }
        }


        DataTable table2 = new DataTable("unit");

        table2.Columns.Add("ID");
        table2.Columns.Add("client_code");
        table2.Columns.Add("state_name");
        table2.Columns.Add("branch_code");
        table2.Columns.Add("comment");
        try
        {
            foreach (DataRow r in dt.Rows)
            {
                int res = 0;

                if (r[0].ToString().Trim() != "" && r[1].ToString().Trim() != "" && r[2].ToString().Trim() != "" && r[3].ToString().Trim() != "" && r[4].ToString().Trim() != "" && r[5].ToString().Trim() != "" && r[6].ToString().Trim() != "" && r[7].ToString().Trim() != "" && r[8].ToString().Trim() != "" && r[9].ToString().Trim() != "" && r[10].ToString().Trim() != "" && r[11].ToString().Trim() != "" && r[12].ToString().Trim() != "" && r[13].ToString().Trim() != "")
                {
                    if (r[1].ToString().Trim() != "Client Name")
                    {
                        string client_code = "", state_name = "", branch_code = "", emp_code = "";
                        client_code = d.getsinglestring("SELECT DISTINCT (client_code) FROM pay_client_master where comp_code= '" + Session["COMP_CODE"].ToString() + "' and client_name='" + r[1].ToString() + "'");
                        if (client_code != "")
                        {
                            state_name = d.getsinglestring("SELECT DISTINCT (state) FROM pay_designation_count where comp_code= '" + Session["COMP_CODE"].ToString() + "' and client_code='" + client_code + "' and state= '" + r[2].ToString().Trim() + "'");
                            if (state_name != "")
                            {
                                branch_code = d.getsinglestring("SELECT DISTINCT (unit_code) FROM pay_unit_master where comp_code= '" + Session["COMP_CODE"].ToString() + "' and client_code='" + client_code + "' and state_name= '" + state_name + "'  and UNIT_NAME='" + r[3].ToString() + "'");
                                if (branch_code != "")
                                {
                                    emp_code = d.getsinglestring("SELECT DISTINCT (emp_code) FROM pay_employee_master where comp_code= '" + Session["COMP_CODE"].ToString() + "' and client_code='" + client_code + "' and client_wise_state= '" + state_name + "'  and EMP_CODE='" + r[6].ToString() + "'");
                                    if (emp_code != "")
                                    {
                                        if (r[4].ToString().Trim() == "Employee" || r[4].ToString().Trim() == "Field_Officer")
                                        {
                                            if (r[5].ToString().Trim() == "Self Advance" || r[5].ToString().Trim() == "Reliver Payment")
                                            {

                                                if (r[8].ToString().Trim().All(char.IsDigit))
                                                {
                                                    if (r[9].ToString().Trim().All(char.IsDigit))
                                                    {
                                                        if (r[10].ToString().Trim() == "Full Payment")
                                                        {
                                                            if (r[12].ToString().Trim().All(char.IsDigit))
                                                            {
                                                                if (r[13].ToString().Trim().All(char.IsDigit))
                                                                {
                                                                    string check_same_record = d.getsinglestring("SELECT * FROM pay_advance_policy where client_code='" + client_code + "' and state_name='" + state_name + "' and branch_code='" + branch_code + "' and pay_to_emp_code='" + emp_code + "' and month='" + r[12].ToString() + "' and year='" + r[13].ToString() + "'");
                                                                    if (check_same_record == "")
                                                                    {
                                                                        string payment_mode = "" + r[10].ToString() + "";
                                                                        string mode = "0";
                                                                        if (payment_mode == "Full Payment")
                                                                        {
                                                                            mode = "2";
                                                                        }
                                                                        result = d.operation("INSERT INTO pay_advance_policy(comp_code,client_code,state_name,branch_code,pay_to_emp_code,user_type,advance_service,amount,date,payment_mode,comment,month,year,installation,remaing_advance,deduction_amount,pay_flag,status) VALUES('" + Session["comp_code"].ToString() + "','" + client_code + "','" + state_name + "','" + branch_code + "','" + emp_code + "','" + r[4].ToString() + "','" + r[5].ToString() + "','" + r[8].ToString() + "','" + r[9].ToString() + "'," + mode + ",'" + r[11].ToString() + "','" + r[12].ToString() + "','" + r[13].ToString() + "','0','0','" + r[8].ToString() + "','0','" + r[5].ToString() + "');");
                                                                    }
                                                                    else
                                                                    { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "Same record Already In List"); }
                                                                }
                                                                else
                                                                { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "Year Is Not In Correct Formate"); }
                                                            }
                                                            else
                                                            { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "Month Is Not In Correct Formate"); }
                                                        }
                                                        else
                                                        { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "Payment Mode is not correct"); }
                                                    }
                                                    else
                                                    { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "Date is Not in Correct Format"); }
                                                }
                                                else { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "Amount Is Not in Numeric Number"); }
                                            }
                                            else { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "Advanced type Is Not in Correct formate"); }
                                        }
                                        else { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "User Type Is Not Correct"); }
                                    }
                                    else { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "Employee Code Not Correct"); }
                                }
                                else { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "Branch Name Not Correct"); }
                            }
                            else { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "State Name Not Correct"); }
                        }
                        else { table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), "Client Name Not Correct"); }
                    }
                }
                else
                {
                    table2.Rows.Add(r[0].ToString(), r[1].ToString(), r[2].ToString(), "Fill All Details");
                }
            }
        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }
        try
        {
            if (table2.Rows.Count > 0)
            {
                DataSet ds = new DataSet("unit");
                ds.Tables.Add(table2);
                send_file(ds);
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('File Uploaded Successfully !!!');", true);
        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }

    }
    private void send_file(DataSet ds)
    {
        if (ds.Tables[0].Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Advanc_payment_upload_issues.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Repeater Repeater1 = new Repeater();
                Repeater1.DataSource = ds;
                Repeater1.HeaderTemplate = new MyTemplate(ListItemType.Header, ds);
                Repeater1.ItemTemplate = new MyTemplate(ListItemType.Item, ds);
                Repeater1.FooterTemplate = new MyTemplate(ListItemType.Footer, null);
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
        public MyTemplate(ListItemType type, DataSet ds)
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

                    lc = new LiteralControl("<table border=1><tr><th>SR. NO.</th><th> CLIENT NAME </th><th> STATE NAME</th><th>BRANCH NAME</th><th>COMMENT</th>");

                    break;
                case ListItemType.Item:

                    lc = new LiteralControl("<tr><td>" + (ctr + 1) + "</td><td>" + ds.Tables[0].Rows[ctr]["client_code"] + " </td><td>" + ds.Tables[0].Rows[ctr]["State_name"] + " </td><td>" + ds.Tables[0].Rows[ctr]["branch_code"] + " </td><td>" + ds.Tables[0].Rows[ctr]["comment"] + " </td>");

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
