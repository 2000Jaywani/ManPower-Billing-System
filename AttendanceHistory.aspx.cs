using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

public partial class AttendanceHistory : System.Web.UI.Page
{
    DAL d = new DAL();
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["comp_code"] == null || Session["comp_code"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }
        if (d.getaccess(Session["ROLE"].ToString(), "Attendance History", Session["COMP_CODE"].ToString()) == "I")
        {
            Response.Redirect("unauthorised_access.aspx");
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Attendance History", Session["COMP_CODE"].ToString()) == "R")
        {
            btn_emp.Visible = true;
            btn_admin.Visible = false;
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Attendance History", Session["COMP_CODE"].ToString()) == "U")
        {
            btn_emp.Visible = true;
            btn_admin.Visible = false;
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Attendance History", Session["COMP_CODE"].ToString()) == "C")
        {
            btn_emp.Visible = true;
            btn_admin.Visible = false;
        }
        if (d.getaccess(Session["ROLE"].ToString(), "Attendance History", Session["COMP_CODE"].ToString()) != "D")
        {
            
        }




        if (!IsPostBack)
        {
            //department
            MySqlCommand cmd = new MySqlCommand("select dept_code,dept_name from pay_department_master where comp_code = '" + Session["comp_code"].ToString() + "'", d.con1);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            ddl_department.DataSource = ds.Tables[0];
            ddl_department.DataValueField = "dept_code";
            ddl_department.DataTextField = "dept_name";
            ddl_department.DataBind();
            ddl_department.Items.Insert(0, new ListItem("--Select Department--", ""));


            //Grade
            cmd = new MySqlCommand("select grade_code,grade_desc from pay_grade_master where comp_code = '" + Session["comp_code"].ToString() + "'", d.con1);
            sda = new MySqlDataAdapter(cmd);
            ds = new DataSet();
            sda.Fill(ds);
            ddl_grade.DataSource = ds.Tables[0];
            ddl_grade.DataValueField = "grade_code";
            ddl_grade.DataTextField = "grade_desc";
            ddl_grade.DataBind();
            ddl_grade.Items.Insert(0, new ListItem("--Select Grade--", ""));

           // Unit Code Display
            ddl_unitcode_att.Items.Clear();
            MySqlCommand cmd_unitcode = new MySqlCommand("Select concat(UNIT_CODE,'-',UNIT_NAME) from pay_unit_master where comp_code='" + Session["comp_code"] + "' and branch_status = 0 ", d.con);
            d.conopen();
            MySqlDataReader dr_unitcode = cmd_unitcode.ExecuteReader();
            while (dr_unitcode.Read())
            {
                ddl_unitcode_att.Items.Add(dr_unitcode.GetValue(0).ToString());
            }
            dr_unitcode.Close();
            d.conclose();
            ddl_unitcode_att.Items.Insert(0, "Select Unit Code");

        }
    }
    protected void ddl_department_SelectedIndexChanged(object sender, EventArgs e)
    {
        MySqlCommand cmd = new MySqlCommand("select grade_code,grade_desc from pay_grade_master where comp_code = '" + Session["comp_code"].ToString() + "' and grade_code in (select distinct(grade_code) from pay_employee_master where dept_code= '" + ddl_department.SelectedValue + "' )", d.con1);
        MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
        DataSet ds = new DataSet();
        sda.Fill(ds);
        ddl_grade.DataSource = ds.Tables[0];
        ddl_grade.DataValueField = "grade_code";
        ddl_grade.DataTextField = "grade_desc";
        ddl_grade.DataBind();
        ddl_grade.Items.Insert(0, new ListItem("--Select Grade--", ""));

    }
    protected void ddl_grade_SelectedIndexChanged(object sender, EventArgs e)
    {
        MySqlCommand cmd = new MySqlCommand("select emp_code,emp_name from pay_employee_master where comp_code = '" + Session["comp_code"].ToString() + "' and emp_code in (select emp_code from pay_employee_master where grade_code= '" + ddl_grade.SelectedValue + "' )", d.con1);
        MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
        DataSet ds = new DataSet();
        sda.Fill(ds);
        ddl_empname.DataSource = ds.Tables[0];
        ddl_empname.DataValueField = "emp_code";
        ddl_empname.DataTextField = "emp_name";
        ddl_empname.DataBind();
        ddl_empname.Items.Insert(0, new ListItem("--Select Employee--", ""));

    }
    protected void btn_save_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        if (ddl_empname.SelectedIndex > 0)
        {
            MySqlCommand cmd = new MySqlCommand("select pay_employee_master.emp_name as emp_name, date_format(logdate,'%d/%m/%Y') as logdate, time(min(logdate)) as intime, time(max(logdate)) as outtime, TIMEDIFF(max(logdate),min(logdate)) as WH from device_logs LEFT join pay_employee_master on pay_employee_master.attendance_id = device_logs.userid where logdate between str_to_date('" + from_date.Text + "','%d/%m/%Y') and str_to_date('" + to_date.Text + "','%d/%m/%Y')  and userid in (select attendance_id from pay_employee_master where emp_code = '" + ddl_empname.SelectedValue + "') group by date_format(logdate,'%d/%m/%Y'), pay_employee_master.emp_code order by pay_employee_master.emp_code, date_format(logdate,'%d/%m/%Y')", d.con1);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            attendance_gridview.DataSource = null;
            attendance_gridview.DataBind();
            attendance_gridview.DataSource = ds.Tables[0];
            attendance_gridview.DataBind();
        }
        else if ((ddl_empname.SelectedIndex <= 0) && (ddl_grade.SelectedIndex > 0))
        {
            MySqlCommand cmd = new MySqlCommand("select pay_employee_master.emp_name as emp_name, date_format(logdate,'%d/%m/%Y') as logdate, time(min(logdate)) as intime, time(max(logdate)) as outtime, TIMEDIFF(max(logdate),min(logdate)) as WH from device_logs LEFT join pay_employee_master on pay_employee_master.attendance_id = device_logs.userid where logdate between str_to_date('" + from_date.Text + "','%d/%m/%Y') and str_to_date('" + to_date.Text + "','%d/%m/%Y')  and userid in (select attendance_id from pay_employee_master where grade_code = '" + ddl_grade.SelectedValue + "') group by date_format(logdate,'%d/%m/%Y'), pay_employee_master.emp_code order by pay_employee_master.emp_code, date_format(logdate,'%d/%m/%Y')", d.con1);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            attendance_gridview.DataSource = null;
            attendance_gridview.DataBind();
            attendance_gridview.DataSource = ds.Tables[0];
            attendance_gridview.DataBind();
        }
        else if ((ddl_empname.SelectedIndex <= 0) && (ddl_grade.SelectedIndex <= 0) && (ddl_department.SelectedIndex > 0))
        {
            MySqlCommand cmd = new MySqlCommand("select pay_employee_master.emp_name as emp_name, date_format(logdate,'%d/%m/%Y') as logdate, time(min(logdate)) as intime, time(max(logdate)) as outtime, TIMEDIFF(max(logdate),min(logdate)) as WH from device_logs LEFT join pay_employee_master on pay_employee_master.attendance_id = device_logs.userid where logdate between str_to_date('" + from_date.Text + "','%d/%m/%Y') and str_to_date('" + to_date.Text + "','%d/%m/%Y')  and userid in (select attendance_id from pay_employee_master where dept_code = '" + ddl_department.SelectedValue + "') group by date_format(logdate,'%d/%m/%Y'), pay_employee_master.emp_code order by pay_employee_master.emp_code, date_format(logdate,'%d/%m/%Y')", d.con1);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            attendance_gridview.DataSource = null;
            attendance_gridview.DataBind();
            attendance_gridview.DataSource = ds.Tables[0];
            attendance_gridview.DataBind();
        }
    }

    protected void btn_exceloutput_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        if (attendance_gridview.Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Attendance_excel_data.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                attendance_gridview.AllowPaging = false;
                foreach (TableCell cell in attendance_gridview.HeaderRow.Cells)
                {
                    cell.BackColor = attendance_gridview.HeaderStyle.BackColor;
                }
                foreach (GridViewRow row in attendance_gridview.Rows)
                {

                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = attendance_gridview.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = attendance_gridview.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }
                attendance_gridview.RenderControl(hw);
                //style to format numbers to string
                string style = @"<style> .textmode { } </style>";
                Response.Write(style);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
        /* Verifies that the control is rendered */
    }
    
    //protected void attendance_gridview_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    attendance_gridview.PageIndex = e.NewPageIndex;
    //    btn_save_Click(null, null);
    //}

    protected void btn_close_click(object sender, EventArgs e) {
        Response.Redirect("Home.aspx");
      
    }

    protected void btn_empployee_attendace(object sender, EventArgs e) 
    {

        String year = txt_from_att.Text.ToString();
       // String month = txt_from_att.Text.Substring(3, 2).ToString();
        String month = txtcurrentmonth.SelectedValue.ToString();

        string com_code = Session["comp_code"].ToString();
        d.con1.Open();
        string query = "";
        if (month == "0")
        {
            query = "SELECT pay_attendance_muster.comp_code, pay_attendance_muster.UNIT_CODE, pay_attendance_muster.EMP_CODE, pay_employee_master .EMP_NAME, pay_attendance_muster.DAY01, pay_attendance_muster.DAY02, pay_attendance_muster.DAY03, pay_attendance_muster.DAY04, pay_attendance_muster.DAY05, pay_attendance_muster.DAY06, pay_attendance_muster.DAY07, pay_attendance_muster.DAY08, pay_attendance_muster.DAY09, pay_attendance_muster.DAY10, pay_attendance_muster.DAY11, pay_attendance_muster.DAY12, pay_attendance_muster.DAY13, pay_attendance_muster.DAY14, pay_attendance_muster.DAY15, pay_attendance_muster.DAY16, pay_attendance_muster.DAY17, pay_attendance_muster.DAY18, pay_attendance_muster.DAY19, pay_attendance_muster.DAY20, pay_attendance_muster.DAY21, pay_attendance_muster.DAY22, pay_attendance_muster.DAY23, pay_attendance_muster.DAY24, pay_attendance_muster.DAY25, pay_attendance_muster.DAY26, pay_attendance_muster.DAY27, pay_attendance_muster.DAY28, pay_attendance_muster.DAY29, pay_attendance_muster.DAY30, pay_attendance_muster.DAY31, pay_attendance_muster.TOT_DAYS_PRESENT,pay_attendance_muster.TOT_DAYS_ABSENT, pay_attendance_muster.TOT_CL,pay_attendance_muster.TOT_PL,pay_attendance_muster.TOT_MATERNITY,pay_attendance_muster.TOT_PATERNITY,pay_attendance_muster.CL_BALANCE,pay_attendance_muster.PL_BALANCE,pay_attendance_muster.TOT_LEAVES, pay_attendance_muster.WEEKLY_OFF, pay_attendance_muster.HOLIDAYS, pay_attendance_muster.TOT_WORKING_DAYS, pay_attendance_muster.MONTH, pay_attendance_muster.YEAR FROM pay_attendance_muster,pay_employee_master   WHERE pay_attendance_muster .EMP_CODE =pay_employee_master .EMP_CODE  AND pay_attendance_muster .comp_code ='" + Session["comp_code"].ToString() + "'  AND pay_attendance_muster .UNIT_CODE ='" + ddl_unitcode_att.SelectedValue.ToString().Substring(0, 4) + "' and  year='" + year + "' and pay_employee_master .EMP_CODE='" + Session["LOGIN_ID"].ToString() + "' ORDER BY  pay_attendance_muster.MONTH";
        }
        else {
            query = "SELECT pay_attendance_muster.comp_code, pay_attendance_muster.UNIT_CODE, pay_attendance_muster.EMP_CODE, pay_employee_master .EMP_NAME, pay_attendance_muster.DAY01, pay_attendance_muster.DAY02, pay_attendance_muster.DAY03, pay_attendance_muster.DAY04, pay_attendance_muster.DAY05, pay_attendance_muster.DAY06, pay_attendance_muster.DAY07, pay_attendance_muster.DAY08, pay_attendance_muster.DAY09, pay_attendance_muster.DAY10, pay_attendance_muster.DAY11, pay_attendance_muster.DAY12, pay_attendance_muster.DAY13, pay_attendance_muster.DAY14, pay_attendance_muster.DAY15, pay_attendance_muster.DAY16, pay_attendance_muster.DAY17, pay_attendance_muster.DAY18, pay_attendance_muster.DAY19, pay_attendance_muster.DAY20, pay_attendance_muster.DAY21, pay_attendance_muster.DAY22, pay_attendance_muster.DAY23, pay_attendance_muster.DAY24, pay_attendance_muster.DAY25, pay_attendance_muster.DAY26, pay_attendance_muster.DAY27, pay_attendance_muster.DAY28, pay_attendance_muster.DAY29, pay_attendance_muster.DAY30, pay_attendance_muster.DAY31, pay_attendance_muster.TOT_DAYS_PRESENT,pay_attendance_muster.TOT_DAYS_ABSENT, pay_attendance_muster.TOT_CL,pay_attendance_muster.TOT_PL,pay_attendance_muster.TOT_MATERNITY,pay_attendance_muster.TOT_PATERNITY,pay_attendance_muster.CL_BALANCE,pay_attendance_muster.PL_BALANCE,pay_attendance_muster.TOT_LEAVES, pay_attendance_muster.WEEKLY_OFF, pay_attendance_muster.HOLIDAYS, pay_attendance_muster.TOT_WORKING_DAYS, pay_attendance_muster.MONTH, pay_attendance_muster.YEAR FROM pay_attendance_muster,pay_employee_master   WHERE pay_attendance_muster .EMP_CODE =pay_employee_master .EMP_CODE  AND pay_attendance_muster .comp_code ='" + Session["comp_code"].ToString() + "'  AND pay_attendance_muster .UNIT_CODE ='" + ddl_unitcode_att.SelectedValue.ToString().Substring(0, 4) + "' and month='" + month + "' and year='" + year + "' and pay_employee_master .EMP_CODE='" + Session["LOGIN_ID"].ToString() + "' ORDER BY  pay_attendance_muster.MONTH";
        }
        MySqlCommand cmd1 = new MySqlCommand(query,d.con1);
        DataSet ds1 = new DataSet();
        MySqlDataAdapter adp1 = new MySqlDataAdapter(cmd1);
        adp1.Fill(ds1);
        gv_attendance.DataSource = ds1.Tables[0];
        gv_attendance.DataBind();
        d.con1.Close();


    }

    protected void btn_attendace_all(object sender,EventArgs e) { 
    
    admin_panel.Visible = true;
    emp_panel.Visible = false;
    try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
    catch { }

    }

    protected void btn_attendace_employee(object sender, EventArgs e) {

        admin_panel.Visible = false;
        emp_panel.Visible = true;
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
    }

}