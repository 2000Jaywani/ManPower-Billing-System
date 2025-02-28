using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Drawing;
using System.Globalization;

public partial class AttendanceRegister1 : System.Web.UI.Page
{
    DAL d5 = new DAL();
    DAL d = new DAL();
    DAL d1 = new DAL();
    AttendanceRegisterBAL arbl = new AttendanceRegisterBAL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (d.getaccess(Session["ROLE"].ToString(), "Attendance Sheet", Session["COMP_CODE"].ToString()) == "I")
        {
            Response.Redirect("unauthorised_access.aspx");
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Attendance Sheet", Session["COMP_CODE"].ToString()) == "R")
        {
            //btn_delete.Visible = false;
            //btn_save.Visible = false;
            //btn_add.Visible = false;
            //btn_print.Visible = false;
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Attendance Sheet", Session["COMP_CODE"].ToString()) == "U")
        {
            //btn_save.Visible = false;
            //btn_add.Visible = false;
            //btn_print.Visible = false;
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Attendance Sheet", Session["COMP_CODE"].ToString()) == "C")
        {
            //btn_delete.Visible = false;
            //btn_print.Visible = false;
        }



        if (Session["COMP_CODE"] == null || Session["COMP_CODE"].ToString() == "")
        {
            Response.Redirect("Home.aspx");
        }

        if (!IsPostBack)
        {
            //txt_date.Text = Session["system_curr_date"].ToString();
            //ddl_unitcode.Items.Clear();
            //System.Data.DataTable dt_item = new System.Data.DataTable();
            //MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select concat(UNIT_NAME,'-',state_name,'-',unit_city) as UNIT_NAME, unit_code from pay_unit_master where COMP_CODE='" + Session["COMP_CODE"] + "' and client_code = '" + Session["SalaryProcessClient"].ToString() + "' ORDER BY UNIT_CODE", d.con);
            //d.con.Open();
            //try
            //{
            //    cmd_item.Fill(dt_item);
            //    if (dt_item.Rows.Count > 0)
            //    {
            //        ddl_unitcode.DataSource = dt_item;
            //        ddl_unitcode.DataTextField = dt_item.Columns[0].ToString();
            //        ddl_unitcode.DataValueField = dt_item.Columns[1].ToString();
            //        ddl_unitcode.DataBind();
            //    }
            //    dt_item.Dispose();
            //    d.con.Close();
            //    ddl_unitcode.Items.Insert(0, "ALL");
            //}
            //catch (Exception ex) { throw ex; }
            //finally
            //{
            //    d.con.Close();
            //}
            //txt_date.Visible = false;
            //ddl_status.Visible = false;
            //btn_add.Visible = false;
            //btn_save.Visible = false;
            //btn_print.Visible = false;
            //lbl_date.Visible = false;
            //lbl_status.Visible = false;
            if (Session["SalarySteps"].ToString() == "1")
            {
                // Session["SalaryProcessMonth"].ToString() = "0";
                // Session["SalaryProcessYear"].ToString() = "0";
                // txt_month.Text = Session["SalaryProcessMonth"].ToString() + "/" + Session["SalaryProcessYear"].ToString();
                //ddl_unitcode.SelectedValue = Session["SalaryProcessUnit"].ToString();
                //rdbmonth.Checked = true;
                btn_process_Click(sender, e);
            }

        }
    }
    protected void btn_process_Click(object sender, EventArgs e)
    {
        d1.con1.Open();
        MySqlCommand cmd_hol;
        if (Session["SalaryProcessUnit"].ToString() == "ALL")
        {
            cmd_hol = new MySqlCommand("Select distinct month_end from pay_attendance_muster where month ='" + Session["SalaryProcessMonth"].ToString() + "' AND YEAR='" + Session["SalaryProcessYear"].ToString() + "' AND COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE in (select unit_code from pay_unit_master where client_code = '" + Session["SalaryProcessClient"].ToString() + "')", d1.con1);
        }
        else
        {
            cmd_hol = new MySqlCommand("Select distinct month_end from pay_attendance_muster where month ='" + Session["SalaryProcessMonth"].ToString() + "' AND YEAR='" + Session["SalaryProcessYear"].ToString() + "' AND COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE ='" + Session["SalaryProcessUnit"].ToString() + "'", d1.con1);
        }
        MySqlDataReader dr_hol = cmd_hol.ExecuteReader();
        while (dr_hol.Read())
        {
            if (dr_hol.GetValue(0).ToString() == "1")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Month End Process completed, You cannot make changes !!')", true);
                d1.con1.Close();
                return;
            }

        }
        dr_hol.Dispose();
        cmd_hol.Dispose();
        d1.con1.Close();

        string h_day = "";
       
        d.con1.Open();
        int res_holiday = 0;
        int res = 0;

        if (Session["SalaryProcessUnit"].ToString() == "ALL")
        {
            res = d.operation("INSERT INTO pay_attendance_muster (COMP_CODE,EMP_CODE,UNIT_CODE,MONTH,YEAR,TOT_WORKING_DAYS,WEEKLY_OFF,TOT_DAYS_PRESENT," + d.PrintSundays(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 1) + ") (SELECT pay_employee_master.COMP_CODE,pay_employee_master.EMP_CODE,pay_employee_master.UNIT_CODE,'" + Session["SalaryProcessMonth"].ToString() + "','" + Session["SalaryProcessYear"].ToString() + "'," + CountDay(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 1) + "," + CountDay(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 3) + "," + CountDay(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 1) + "," + d.PrintSundays(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 2) + "  FROM pay_employee_master WHERE (pay_employee_master.LEFT_REASON='' || pay_employee_master.LEFT_REASON is null) AND pay_employee_master.EMP_CODE NOT IN (SELECT EMP_CODE FROM pay_attendance_muster WHERE COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE in (select unit_code from pay_unit_master where client_code = '" + Session["SalaryProcessClient"].ToString() + "') AND pay_attendance_muster.MONTH = '" + Session["SalaryProcessMonth"].ToString() + "' AND pay_attendance_muster.YEAR='" + Session["SalaryProcessYear"].ToString() + "') AND pay_employee_master.COMP_CODE='" + Session["COMP_CODE"].ToString() + "'  and (pay_employee_master.LEFT_REASON='' || pay_employee_master.LEFT_REASON is null) AND  pay_employee_master.UNIT_CODE in (select unit_code from pay_unit_master where client_code = '" + Session["SalaryProcessClient"].ToString() + "'))");
        }
        else
        {
            res = d.operation("INSERT INTO pay_attendance_muster (COMP_CODE,EMP_CODE,UNIT_CODE,MONTH,YEAR,TOT_WORKING_DAYS,WEEKLY_OFF,TOT_DAYS_PRESENT," + d.PrintSundays(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 1) + ") (SELECT pay_employee_master.COMP_CODE,pay_employee_master.EMP_CODE,pay_employee_master.UNIT_CODE,'" + Session["SalaryProcessMonth"].ToString() + "','" + Session["SalaryProcessYear"].ToString() + "'," + CountDay(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 1) + "," + CountDay(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 3) + "," + CountDay(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 1) + "," + d.PrintSundays(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 2) + " FROM pay_employee_master WHERE (pay_employee_master.LEFT_REASON='' || pay_employee_master.LEFT_REASON is null) AND pay_employee_master.EMP_CODE NOT IN (SELECT EMP_CODE FROM pay_attendance_muster WHERE COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE='" + Session["SalaryProcessUnit"].ToString() + "' AND pay_attendance_muster.MONTH = '" + Session["SalaryProcessMonth"].ToString() + "' AND pay_attendance_muster.YEAR='" + Session["SalaryProcessYear"].ToString() + "') AND pay_employee_master.COMP_CODE='" + Session["COMP_CODE"].ToString() + "'  and (pay_employee_master.LEFT_REASON='' || pay_employee_master.LEFT_REASON is null) AND pay_employee_master.UNIT_CODE='" + Session["SalaryProcessUnit"].ToString() + "')");
        }

       

        //Added by vinod for Leave
        d1.con1.Open();
        MySqlCommand cmd_leave = new MySqlCommand("select emp_code, leave_type, day(From_date), No_OF_days, date_format(from_date,'%d/%m/%Y'), date_format(to_date,'%d/%m/%Y')  from pay_leave_transaction where from_date between str_to_date('01/" + Session["SalaryProcessMonth"].ToString() + "/" + Session["SalaryProcessYear"].ToString() + "', '%d/%m/%Y') and str_to_date('31/" + Session["SalaryProcessMonth"].ToString() + "/" + Session["SalaryProcessYear"].ToString() + "', '%d/%m/%Y') and leave_status = 'Approved'  and COMP_CODE='" + Session["COMP_CODE"].ToString() + "'", d1.con1);
        MySqlDataReader dr_leave = cmd_leave.ExecuteReader();
        while (dr_leave.Read())
        {
            DateTime FromYear = DateTime.ParseExact(dr_leave.GetValue(4).ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime ToYear = DateTime.ParseExact(dr_leave.GetValue(5).ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            double no_of_days = double.Parse(dr_leave.GetValue(3).ToString());
            no_of_days--;
            int day1 = int.Parse(dr_leave.GetValue(2).ToString());

            int leave = dr_leave.GetValue(1).ToString().IndexOf('-');
            string leave_abbr = dr_leave.GetValue(1).ToString().Substring(leave + 1);

            while (no_of_days >= 0)
            {
                if (day1 < 10)
                {
                    h_day = "DAY0" + day1;
                }
                else
                {
                    h_day = "DAY" + day1;
                }
                res_holiday = d.operation("UPDATE pay_attendance_muster  SET " + h_day + "='" + (leave_abbr == "HD" ? "HD" : leave_abbr) + "' WHERE EMP_CODE = '" + dr_leave.GetValue(0).ToString() + "' AND COMP_CODE = '" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE='" + Session["SalaryProcessUnit"].ToString() + "' AND MONTH = '" + Session["SalaryProcessMonth"].ToString() + "' AND YEAR='" + Session["SalaryProcessYear"].ToString() + "'");
                day1 = day1 + 1;
                no_of_days = no_of_days - 1;
            }
        }
        dr_leave.Close();
        cmd_leave.Dispose();
        d1.con1.Close();


        //updatesunday();
        //if (rdbmonth.Checked == true && rdbindividual.Checked == false)
        //{
        Panel1.Visible = true;
        //Panel2.Visible = false;
        //lblheader.Text = "Attendance of unit: " + ddl_unitcode.SelectedItem.Text;
        d1.con1.Open();
        //MySqlCommand cmd2 = new MySqlCommand("SELECT pay_attendance_muster .*,pay_employee_master .EMP_NAME FROM pay_attendance_muster,pay_employee_master   WHERE pay_attendance_muster .EMP_CODE =pay_employee_master .EMP_CODE  AND pay_attendance_muster .COMP_CODE ='" + Session["COMP_CODE"].ToString() + "' AND pay_attendance_muster .UNIT_CODE ='" + Session["SalaryProcessUnit"].ToString() + "' AND pay_attendance_muster.MONTH ='" + Session["SalaryProcessMonth"].ToString() + "'  AND pay_attendance_muster.YEAR ='" + Session["SalaryProcessYear"].ToString() + "' ORDER BY pay_employee_master .EMP_CODE", d1.con1);
        DataSet ds2 = new DataSet();
        //   MySqlDataAdapter adp2 = new MySqlDataAdapter("SELECT pay_attendance_muster .*,pay_employee_master .EMP_NAME , pay_leave_emp_balance.CL , pay_leave_emp_balance.PL FROM pay_attendance_muster,pay_employee_master,pay_leave_emp_balance  WHERE pay_attendance_muster.EMP_CODE = pay_employee_master .EMP_CODE AND pay_leave_emp_balance.emp_code = pay_attendance_muster.EMP_CODE  AND pay_attendance_muster .COMP_CODE ='" + Session["COMP_CODE"].ToString() + "' AND pay_attendance_muster .UNIT_CODE ='" + Session["SalaryProcessUnit"].ToString() + "' AND pay_attendance_muster.MONTH = '" + Session["SalaryProcessMonth"].ToString() + "' AND pay_attendance_muster.YEAR ='" + Session["SalaryProcessYear"].ToString() + "' ORDER BY pay_employee_master.EMP_CODE ", d1.con1);
        MySqlDataAdapter adp2;
        if (Session["SalaryProcessUnit"].ToString() == "ALL")
        {
            adp2 = new MySqlDataAdapter("SELECT pay_attendance_muster.*,pay_employee_master.EMP_NAME FROM pay_attendance_muster inner join pay_employee_master on pay_attendance_muster.EMP_CODE =pay_employee_master.EMP_CODE and pay_attendance_muster.COMP_CODE = pay_employee_master.COMP_CODE  and `pay_attendance_muster`.`UNIT_CODE` = `pay_employee_master`.`UNIT_CODE` WHERE pay_attendance_muster .COMP_CODE ='" + Session["COMP_CODE"].ToString() + "' AND pay_attendance_muster.UNIT_CODE in (select unit_code from pay_unit_master where client_code = '" + Session["SalaryProcessClient"].ToString() + "') AND pay_attendance_muster.MONTH ='" + Session["SalaryProcessMonth"].ToString() + "' AND pay_attendance_muster.YEAR ='" + Session["SalaryProcessYear"].ToString() + "' ORDER BY pay_employee_master.EMP_CODE ", d1.con1);
        }
        else
        {
            adp2 = new MySqlDataAdapter("SELECT pay_attendance_muster.*,pay_employee_master.EMP_NAME FROM pay_attendance_muster inner join pay_employee_master on pay_attendance_muster.EMP_CODE =pay_employee_master.EMP_CODE and pay_attendance_muster.COMP_CODE = pay_employee_master.COMP_CODE  and `pay_attendance_muster`.`UNIT_CODE` = `pay_employee_master`.`UNIT_CODE` WHERE pay_attendance_muster .COMP_CODE ='" + Session["COMP_CODE"].ToString() + "' AND pay_attendance_muster.UNIT_CODE ='" + Session["SalaryProcessUnit"].ToString() + "' AND pay_attendance_muster.MONTH ='" + Session["SalaryProcessMonth"].ToString() + "' AND pay_attendance_muster.YEAR ='" + Session["SalaryProcessYear"].ToString() + "' ORDER BY pay_employee_master.EMP_CODE ", d1.con1);
        }
        adp2.Fill(ds2);
        gv_attendance.DataSource = ds2.Tables[0];
        gv_attendance.DataBind();
        d1.con1.Close();



        if (ds2.Tables[0].Rows.Count > 0)
        {

            loadheaders(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()));
        }
        
        d.con1.Close();
        //btn_save.Visible = true;
    }
    private string chkday(string day)
    {
        if (day == "1") { return "01"; }
        else if (day == "2") { return "02"; }
        else if (day == "3") { return "03"; }
        else if (day == "4") { return "04"; }
        else if (day == "5") { return "05"; }
        else if (day == "6") { return "06"; }
        else if (day == "7") { return "07"; }
        else if (day == "8") { return "08"; }
        else if (day == "9") { return "09"; }
        return day;

    }
    private string chkmonth(string month)
    {
        if (month == "1") { return "01"; }
        else if (month == "2") { return "02"; }
        else if (month == "3") { return "03"; }
        else if (month == "4") { return "04"; }
        else if (month == "5") { return "05"; }
        else if (month == "6") { return "06"; }
        else if (month == "7") { return "07"; }
        else if (month == "8") { return "08"; }
        else if (month == "9") { return "09"; }

        return month;

    }
    protected void updatesunday()
    {
        int count = CountDay(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 2);

        for (int i = 1; i < count; i++)
        {
            string countnew = Convert.ToString(i);
           string countnew1= chkday(countnew);
           string month = ((Session["SalaryProcessMonth"].ToString()));
           string chkmonth1 = chkmonth(month);
             string year=(Session["SalaryProcessYear"].ToString());
        
           string date = string.Concat( countnew1 +  "/" +   chkmonth1 + "/"+ year);

           DateTime date1 = DateTime.ParseExact(date, "dd/MM/yyyy", null);
            string days = date1.DayOfWeek.ToString();

            string dateupdate = countnew1;
            if(days=="Sunday")
            {
                int res = d.operation("update pay_attendance_muster set DAY" + dateupdate.ToString() + "='W'  where   COMP_CODE = '" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE='" + Session["SalaryProcessUnit"].ToString() + "'  AND MONTH = '" + Session["SalaryProcessMonth"].ToString() + "' AND YEAR='" + Session["SalaryProcessYear"].ToString() + "' and DAY" + dateupdate.ToString() + "!='P'  ");
            }

        }



       //int TotalRows = gv_attendance.Rows.Count;

       //for (int i = 0; i < TotalRows; i++)
       //{
       //    System.Web.UI.WebControls.Label lbl_empcode = (System.Web.UI.WebControls.Label)gv_attendance.Rows[i].Cells[1].FindControl("lblempcode");
       //    string empcode = lbl_empcode.Text;

       //    d5.con.Open();
       //    MySqlCommand cmd = new MySqlCommand("select DAY01,DAY02,DAY03,DAY04,DAY05,DAY06,DAY07,DAY08,DAY09,DAY10,DAY11,DAY12,DAY13,DAY14,DAY15,DAY16,DAY17,DAY18,DAY19,DAY20,DAY21,DAY22,DAY23,DAY24,DAY25,DAY26,DAY27,DAY28,DAY29,DAY30,DAY31,EMP_CODE from pay_attendance_muster where EMP_CODE='" + empcode + "' AND  COMP_CODE = '" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE='" + Session["SalaryProcessUnit"].ToString() + "' AND MONTH = '" + Session["SalaryProcessMonth"].ToString() + "' AND YEAR='" + Session["SalaryProcessYear"].ToString() + "' ", d5.con);
       //    MySqlDataReader drcount1 = cmd.ExecuteReader();
       //    double pcount = 0;
       //    double acount = 0;
       //    double halfdaycount = 0;
       //    double leavescount = 0;
       //    double holidaycount = 0;
       //    double weeklyoffcount = 0;
       //    while (drcount1.Read())
       //    {
       //        string day1 = drcount1.GetValue(0).ToString();
       //        string day2 = drcount1.GetValue(1).ToString();
       //        string day3 = drcount1.GetValue(2).ToString();
       //        string day4 = drcount1.GetValue(3).ToString();
       //        string day5 = drcount1.GetValue(4).ToString();
       //        string day6 = drcount1.GetValue(5).ToString();
       //        string day7 = drcount1.GetValue(6).ToString();
       //        string day8 = drcount1.GetValue(7).ToString();
       //        string day9 = drcount1.GetValue(8).ToString();
       //        string day10 = drcount1.GetValue(9).ToString();
       //        string day11 = drcount1.GetValue(11).ToString();
       //        string day12 = drcount1.GetValue(11).ToString();
       //        string day13 = drcount1.GetValue(12).ToString();
       //        string day14 = drcount1.GetValue(13).ToString();
       //        string day15 = drcount1.GetValue(14).ToString();
       //        string day16 = drcount1.GetValue(15).ToString();
       //        string day17 = drcount1.GetValue(16).ToString();
       //        string day18 = drcount1.GetValue(17).ToString();
       //        string day19 = drcount1.GetValue(18).ToString();
       //        string day20 = drcount1.GetValue(19).ToString();
       //        string day21 = drcount1.GetValue(20).ToString();
       //        string day22 = drcount1.GetValue(21).ToString();
       //        string day23 = drcount1.GetValue(22).ToString();
       //        string day24 = drcount1.GetValue(23).ToString();
       //        string day25 = drcount1.GetValue(24).ToString();
       //        string day26 = drcount1.GetValue(25).ToString();
       //        string day27 = drcount1.GetValue(26).ToString();
       //        string day28 = drcount1.GetValue(27).ToString();
       //        string day29 = drcount1.GetValue(28).ToString();
       //        string day30 = drcount1.GetValue(29).ToString();
       //        string day31 = drcount1.GetValue(30).ToString();
       //        string emp_code = drcount1.GetValue(31).ToString();


       //        double cocount = 0;

       //        for (int j = 0; j <= 30; j++)
       //        {
       //            if (drcount1.GetValue(j).ToString() == "P")
       //            {
       //                pcount++;
       //            }

       //            if (drcount1.GetValue(j).ToString() == "A")
       //            {
       //                acount++;
       //            }
       //            if (drcount1.GetValue(j).ToString() == "HD")
       //            {
       //                halfdaycount++;
       //            }
       //            if (drcount1.GetValue(j).ToString() == "L")
       //            {
       //                leavescount++;
       //            }
       //            if (drcount1.GetValue(j).ToString() == "W")
       //            {
       //                weeklyoffcount++;
       //            }
       //            if (drcount1.GetValue(j).ToString() == "H")
       //            {
       //                holidaycount++;
       //            }
       //            if (drcount1.GetValue(j).ToString() == "CL")
       //            {
       //                leavescount++;
       //            }
       //            if (drcount1.GetValue(j).ToString() == "PL")
       //            {
       //                leavescount++;
       //            }
       //            if (drcount1.GetValue(j).ToString() == "EL")
       //            {
       //                leavescount++;
       //            }
       //            if (drcount1.GetValue(j).ToString() == "SL")
       //            {
       //                leavescount++;
       //            }

       //        }
       //        halfdaycount = halfdaycount / 2;
       //        pcount = halfdaycount + pcount;

       //        int month = Convert.ToInt32(Session["SalaryProcessMonth"].ToString());
       //        int year = Convert.ToInt32(Session["SalaryProcessYear"].ToString());
       //        int total_day_count = CountDay(month, year, 1);

       //        d.operation("Update pay_attendance_muster set  TOT_DAYS_PRESENT =" + pcount + ", TOT_DAYS_ABSENT =" + acount + ", TOT_HALF_DAYS =" + halfdaycount + ",TOT_LEAVES =" + leavescount + ", HOLIDAYS =" + holidaycount + " ,WEEKLY_OFF='" + weeklyoffcount + "',TOT_WORKING_DAYS='" + total_day_count + "'  where comp_code = '" + Session["COMP_CODE"].ToString() + "' and month = '" + Session["SalaryProcessMonth"].ToString() + "' and year = '" + Session["SalaryProcessYear"].ToString() + "' and emp_code = '" + emp_code + "'");
       //    }
       //    d5.con.Close();
       //}

    }
    
    double halfdays = 0;

    //protected void btn_add_Click(object sender, EventArgs e)
    //{

    //    //--------- For Present ---------------
    //    if (ddl_status.Text == "Present")
    //    {

    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "P";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "P";
    //            }
    //        }
    //    }
    //    //--------- For Absent ---------------
    //    else if (ddl_status.Text == "Absent")
    //    {
    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "A";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "A";
    //            }
    //        }
    //    }
    //    //--------- For Half Day ---------------
    //    else if (ddl_status.Text == "Half Day")
    //    {

    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "F";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "F";
    //            }
    //        }
    //    }

    //          //--------- For CLeaves ---------------
    //    else if (ddl_status.Text == "Cl Leave")
    //    {
    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "CL";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "CL";
    //            }
    //        }
    //    }

    //                  //--------- For pLeaves ---------------
    //    else if (ddl_status.Text == "Pl Leave")
    //    {
    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "PL";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "CL";
    //            }
    //        }
    //    }

    //    else if (ddl_status.Text == "Ml Leave")
    //    {
    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "ML";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "ML";
    //            }
    //        }
    //    }
    //    else if (ddl_status.Text == "Previlage Leave")
    //    {
    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "PH";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "PH";
    //            }
    //        }
    //    }

    //    //--------- For Leaves ---------------
    //    else if (ddl_status.Text == "Leave")
    //    {
    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "L";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "L";
    //            }
    //        }
    //    }
    //    //--------- For Half Day ---------------
    //    else if (ddl_status.Text == "Weekly Off")
    //    {
    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);

    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "W";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "W";
    //            }
    //        }
    //    }
    //    //--------- For Holiday ---------------
    //    else if (ddl_status.Text == "Holiday")
    //    {
    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "H";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "H";
    //            }
    //        }
    //    }
    //    // ------------Company Off------------
    //    else if (ddl_status.Text == "Company Off")
    //    {
    //        int split_length = txt_date.Text.ToString().IndexOf('/');
    //        string day = txt_date.Text.ToString().Substring(0, split_length);
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            for (int i = 0; i <= 33; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Text = "CO";
    //                    }
    //                }
    //            }
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            foreach (GridViewRow row in GridView1.Rows)
    //            {
    //                TextBox tempday = (TextBox)row.FindControl("txtday");
    //                tempday.Text = "CO";
    //            }
    //        }
    //    }

    //    if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //    {
    //        int TotalRows = gv_attendance.Rows.Count;

    //        for (int i = 0; i < TotalRows; i++)
    //        {
    //            int TotalCells = gv_attendance.Rows[i].Cells.Count;

    //            int pcount = 0;
    //            int acount = 0;
    //            double halfdaycount = 0;
    //            int leavescount = 0;
    //            int weeklyoffcount = 0;
    //            int holidaycount = 0;
    //            int clcount = 0;
    //            int plcount = 0;
    //            int mlcounty = 0;
    //            int privilagecount = 0;
    //            int cocount = 0;

    //            for (int j = 1; j <= TotalCells; j++)
    //            {
    //                if (j <= 9)
    //                {
    //                    string cntrlname = "txtday" + '0' + j.ToString();
    //                    TextBox txt_day1 = (TextBox)gv_attendance.Rows[i].Cells[j].FindControl(cntrlname);
    //                    if (txt_day1.Text == "P")
    //                    {
    //                        pcount++;
    //                    }
    //                    else if (txt_day1.Text == "A")
    //                    {
    //                        acount++;
    //                    }
    //                    else if (txt_day1.Text == "F")
    //                    {
    //                        halfdaycount++;
    //                    }
    //                    else if (txt_day1.Text == "L")
    //                    {
    //                        leavescount++;
    //                    }
    //                    else if (txt_day1.Text == "W")
    //                    {
    //                        weeklyoffcount++;
    //                    }
    //                    else if (txt_day1.Text == "H")
    //                    {
    //                        holidaycount++;
    //                    }
    //                    else if (txt_day1.Text == "CL")
    //                    {
    //                        leavescount++;
    //                    }
    //                    else if (txt_day1.Text == "PL")
    //                    {
    //                        leavescount++;
    //                    }
    //                    else if (txt_day1.Text == "ML")
    //                    {
    //                        leavescount++;
    //                    }
    //                    else if (txt_day1.Text == "PH")
    //                    {
    //                        leavescount++;
    //                    }
    //                    else if (txt_day1.Text == "CO")
    //                    {
    //                        cocount++;
    //                    }
    //                }
    //                else if (j <= 31)
    //                {
    //                    string cntrlname = "txtday" + j.ToString();
    //                    TextBox txt_day1 = (TextBox)gv_attendance.Rows[i].Cells[j].FindControl(cntrlname);
    //                    if (txt_day1.Text == "P")
    //                    {
    //                        pcount++;
    //                    }
    //                    else if (txt_day1.Text == "A")
    //                    {
    //                        acount++;
    //                    }
    //                    else if (txt_day1.Text == "F")
    //                    {
    //                        halfdaycount++;
    //                    }
    //                    else if (txt_day1.Text == "L")
    //                    {
    //                        leavescount++;
    //                    }
    //                    else if (txt_day1.Text == "W")
    //                    {
    //                        weeklyoffcount++;
    //                    }
    //                    else if (txt_day1.Text == "H")
    //                    {
    //                        holidaycount++;
    //                    }
    //                    else if (txt_day1.Text == "CL")
    //                    {
    //                        leavescount++;
    //                    }
    //                    else if (txt_day1.Text == "PL")
    //                    {
    //                        leavescount++;
    //                    }
    //                    else if (txt_day1.Text == "ML")
    //                    {
    //                        leavescount++;
    //                    }
    //                    else if (txt_day1.Text == "PH")
    //                    {
    //                        leavescount++;
    //                    }
    //                    else if (txt_day1.Text == "CO")
    //                    {
    //                        cocount++;
    //                    }

    //                }
    //            }
    //            //Label lbl_totpdays = (Label)gv_attendance.Rows[i].Cells[34].FindControl("lbl_totPdays");
    //            //Label lbl_totadays = (Label)gv_attendance.Rows[i].Cells[35].FindControl("lbl_totAdays");
    //            //Label lbl_totcleaves = (Label)gv_attendance.Rows[i].Cells[36].FindControl("lbl_totcLeaves");
    //            //Label lbl_totpleaves = (Label)gv_attendance.Rows[i].Cells[37].FindControl("lbl_totpLeaves");
    //            //Label lbl_totmleaves = (Label)gv_attendance.Rows[i].Cells[38].FindControl("lbl_totmLeaves");
    //            //Label lbl_totprevileaves = (Label)gv_attendance.Rows[i].Cells[39].FindControl("lbl_totprevilageLeaves");
    //            //Label lbl_totcleavesbal = (Label)gv_attendance.Rows[i].Cells[40].FindControl("lbl_totcbalLeaves");
    //            //Label lbl_totpleavesbal = (Label)gv_attendance.Rows[i].Cells[41].FindControl("lbl_totpbalLeaves");
    //            //Label lbl_totcballeaves = (Label)gv_attendance.Rows[i].Cells[42].FindControl("lbl_totcbalLeaves");
    //            //Label lbl_totpballeaves = (Label)gv_attendance.Rows[i].Cells[43].FindControl("lbl_totpbalLeaves");

    //            //Label lbl_totleaves = (Label)gv_attendance.Rows[i].Cells[44].FindControl("lbl_totLeaves");
    //            //Label lbl_totwoff = (Label)gv_attendance.Rows[i].Cells[45].FindControl("lbl_totWoff");
    //            //Label lbl_totholidays = (Label)gv_attendance.Rows[i].Cells[46].FindControl("lbl_totHolidays");
    //            //Label lbl_totwdays = (Label)gv_attendance.Rows[i].Cells[47].FindControl("lbl_totWdays");

    //            Label lbl_totpdays = (Label)gv_attendance.Rows[i].Cells[34].FindControl("lbl_totPdays");
    //            Label lbl_totadays = (Label)gv_attendance.Rows[i].Cells[35].FindControl("lbl_totAdays");

    //            Label lbl_totleaves = (Label)gv_attendance.Rows[i].Cells[36].FindControl("lbl_totLeaves");
    //            Label lbl_totwoff = (Label)gv_attendance.Rows[i].Cells[37].FindControl("lbl_totWoff");
    //            Label lbl_totholidays = (Label)gv_attendance.Rows[i].Cells[38].FindControl("lbl_totHolidays");
    //            Label lbl_company_off = (Label)gv_attendance.Rows[i].Cells[39].FindControl("lbl_co");
    //            Label lbl_totwdays = (Label)gv_attendance.Rows[i].Cells[40].FindControl("lbl_totWdays");

    //            halfdays = halfdaycount / 2;
    //            double presentdays = pcount + halfdays;
    //            lbl_totpdays.Text = presentdays.ToString();
    //            lbl_totadays.Text = acount.ToString();

    //            lbl_totleaves.Text = leavescount.ToString();
    //            lbl_totwoff.Text = weeklyoffcount.ToString();
    //            lbl_totholidays.Text = holidaycount.ToString();
    //            lbl_company_off.Text = cocount.ToString();
    //            double totwdays = presentdays + leavescount + clcount + plcount + mlcounty + privilagecount + weeklyoffcount + holidaycount + cocount;
    //            lbl_totwdays.Text = totwdays.ToString();
    //        }
    //    }
    //    else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //    {
    //    }
    //    btn_save.Visible = true;
    //}
    //protected void btn_save_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        if (rdbmonth.Checked == true && rdbindividual.Checked == false)
    //        {
    //            //int result = 0;
    //            int TotalRows = gv_attendance.Rows.Count;

    //            for (int i = 0; i < TotalRows; i++)
    //            {
    //                Label lbl_empcode = (Label)gv_attendance.Rows[i].Cells[1].FindControl("lblempcode");
    //                Label lbl_empname = (Label)gv_attendance.Rows[i].Cells[2].FindControl("lblempname");
    //                TextBox txt_day1 = (TextBox)gv_attendance.Rows[i].Cells[3].FindControl("txtday01");
    //                TextBox txt_day2 = (TextBox)gv_attendance.Rows[i].Cells[4].FindControl("txtday02");
    //                TextBox txt_day3 = (TextBox)gv_attendance.Rows[i].Cells[5].FindControl("txtday03");
    //                TextBox txt_day4 = (TextBox)gv_attendance.Rows[i].Cells[6].FindControl("txtday04");
    //                TextBox txt_day5 = (TextBox)gv_attendance.Rows[i].Cells[7].FindControl("txtday05");
    //                TextBox txt_day6 = (TextBox)gv_attendance.Rows[i].Cells[8].FindControl("txtday06");
    //                TextBox txt_day7 = (TextBox)gv_attendance.Rows[i].Cells[9].FindControl("txtday07");
    //                TextBox txt_day8 = (TextBox)gv_attendance.Rows[i].Cells[10].FindControl("txtday08");
    //                TextBox txt_day9 = (TextBox)gv_attendance.Rows[i].Cells[11].FindControl("txtday09");
    //                TextBox txt_day10 = (TextBox)gv_attendance.Rows[i].Cells[12].FindControl("txtday10");
    //                TextBox txt_day11 = (TextBox)gv_attendance.Rows[i].Cells[13].FindControl("txtday11");
    //                TextBox txt_day12 = (TextBox)gv_attendance.Rows[i].Cells[14].FindControl("txtday12");
    //                TextBox txt_day13 = (TextBox)gv_attendance.Rows[i].Cells[15].FindControl("txtday13");
    //                TextBox txt_day14 = (TextBox)gv_attendance.Rows[i].Cells[16].FindControl("txtday14");
    //                TextBox txt_day15 = (TextBox)gv_attendance.Rows[i].Cells[17].FindControl("txtday15");
    //                TextBox txt_day16 = (TextBox)gv_attendance.Rows[i].Cells[18].FindControl("txtday16");
    //                TextBox txt_day17 = (TextBox)gv_attendance.Rows[i].Cells[19].FindControl("txtday17");
    //                TextBox txt_day18 = (TextBox)gv_attendance.Rows[i].Cells[20].FindControl("txtday18");
    //                TextBox txt_day19 = (TextBox)gv_attendance.Rows[i].Cells[21].FindControl("txtday19");
    //                TextBox txt_day20 = (TextBox)gv_attendance.Rows[i].Cells[22].FindControl("txtday20");
    //                TextBox txt_day21 = (TextBox)gv_attendance.Rows[i].Cells[23].FindControl("txtday21");
    //                TextBox txt_day22 = (TextBox)gv_attendance.Rows[i].Cells[24].FindControl("txtday22");
    //                TextBox txt_day23 = (TextBox)gv_attendance.Rows[i].Cells[25].FindControl("txtday23");
    //                TextBox txt_day24 = (TextBox)gv_attendance.Rows[i].Cells[26].FindControl("txtday24");
    //                TextBox txt_day25 = (TextBox)gv_attendance.Rows[i].Cells[27].FindControl("txtday25");
    //                TextBox txt_day26 = (TextBox)gv_attendance.Rows[i].Cells[28].FindControl("txtday26");
    //                TextBox txt_day27 = (TextBox)gv_attendance.Rows[i].Cells[29].FindControl("txtday27");
    //                TextBox txt_day28 = (TextBox)gv_attendance.Rows[i].Cells[30].FindControl("txtday28");
    //                TextBox txt_day29 = (TextBox)gv_attendance.Rows[i].Cells[31].FindControl("txtday29");
    //                TextBox txt_day30 = (TextBox)gv_attendance.Rows[i].Cells[32].FindControl("txtday30");
    //                TextBox txt_day31 = (TextBox)gv_attendance.Rows[i].Cells[33].FindControl("txtday31");


    //                Label lbl_totpdays = (Label)gv_attendance.Rows[i].Cells[34].FindControl("lbl_totPdays");
    //                Label lbl_totadays = (Label)gv_attendance.Rows[i].Cells[35].FindControl("lbl_totAdays");


    //                Label lbl_totleaves = (Label)gv_attendance.Rows[i].Cells[36].FindControl("lbl_totLeaves");
    //                Label lbl_totwoff = (Label)gv_attendance.Rows[i].Cells[37].FindControl("lbl_totWoff");
    //                Label lbl_totholidays = (Label)gv_attendance.Rows[i].Cells[38].FindControl("lbl_totHolidays");
    //                Label lbl_company_off = (Label)gv_attendance.Rows[i].Cells[39].FindControl("lbl_co");
    //                Label lbl_totwdays = (Label)gv_attendance.Rows[i].Cells[40].FindControl("lbl_totWdays");

    //                if (lbl_totpdays.Text == "" || lbl_totpdays.Text == null)
    //                {
    //                    lbl_totpdays.Text = "0";
    //                }
    //                if (lbl_totadays.Text == "" || lbl_totadays.Text == null)
    //                {
    //                    lbl_totadays.Text = "0";
    //                }
    //                if (lbl_totleaves.Text == "" || lbl_totleaves.Text == null)
    //                {
    //                    lbl_totleaves.Text = "0";
    //                }
    //                if (lbl_totwoff.Text == "" || lbl_totwoff.Text == null)
    //                {
    //                    lbl_totwoff.Text = "0";
    //                }
    //                if (lbl_totholidays.Text == "" || lbl_totholidays.Text == null)
    //                {
    //                    lbl_totholidays.Text = "0";
    //                }
    //                if (lbl_company_off.Text == "" || lbl_company_off.Text == null)
    //                {
    //                    lbl_company_off.Text = "0";
    //                }
    //                if (lbl_totwdays.Text == "" || lbl_totwdays.Text == null)
    //                {
    //                    lbl_totwdays.Text = "0";
    //                }

    //                string month = txt_date.Text.Substring(3, 2);
    //                int year = Convert.ToInt32(txt_date.Text.Substring(6));
    //                string update;
    //                if (Session["SalaryProcessUnit"].ToString() == "ALL")
    //                {
    //                    update = "UPDATE pay_attendance_muster SET DAY01 ='" + txt_day1.Text + "', DAY02 ='" + txt_day2.Text + "', DAY03 ='" + txt_day3.Text + "', DAY04 ='" + txt_day4.Text + "', DAY05 ='" + txt_day5.Text + "', DAY06 ='" + txt_day6.Text + "', DAY07 ='" + txt_day7.Text + "', DAY08 ='" + txt_day8.Text + "', DAY09 ='" + txt_day9.Text + "', DAY10 ='" + txt_day10.Text + "', DAY11 ='" + txt_day11.Text + "', DAY12 ='" + txt_day12.Text + "', DAY13 ='" + txt_day13.Text + "', DAY14 ='" + txt_day14.Text + "', DAY15 ='" + txt_day15.Text + "', DAY16 ='" + txt_day16.Text + "', DAY17 ='" + txt_day17.Text + "', DAY18 ='" + txt_day18.Text + "', DAY19 ='" + txt_day19.Text + "', DAY20 ='" + txt_day20.Text + "', DAY21 ='" + txt_day21.Text + "', DAY22 ='" + txt_day22.Text + "', DAY23 ='" + txt_day23.Text + "', DAY24 ='" + txt_day24.Text + "', DAY25 ='" + txt_day25.Text + "', DAY26 ='" + txt_day26.Text + "', DAY27 ='" + txt_day27.Text + "', DAY28 ='" + txt_day28.Text + "', DAY29 ='" + txt_day29.Text + "', DAY30 ='" + txt_day30.Text + "', DAY31 ='" + txt_day31.Text + "', TOT_DAYS_PRESENT =" + Convert.ToDouble(lbl_totpdays.Text) + ", TOT_DAYS_ABSENT =" + Convert.ToDouble(lbl_totadays.Text) + ", TOT_HALF_DAYS =" + halfdays + ",TOT_LEAVES =" + Convert.ToDouble(lbl_totleaves.Text) + ", WEEKLY_OFF =" + Convert.ToDouble(lbl_totwoff.Text) + ", HOLIDAYS =" + Convert.ToDouble(lbl_totholidays.Text) + ", TOT_CO=" + Convert.ToDouble(lbl_company_off.Text) + ",TOT_WORKING_DAYS =" + Convert.ToDouble(lbl_totwdays.Text) + " Where MONTH= '" + Session["SalaryProcessMonth"].ToString() + "' AND YEAR= '" + Session["SalaryProcessYear"].ToString() + "' AND COMP_CODE = '" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE in (select unit_code from pay_unit_master where client_code = '" + Session["SalaryProcessClient"].ToString() + "') AND EMP_CODE='" + lbl_empcode.Text + "'";
    //                }
    //                else
    //                {
    //                    update = "UPDATE pay_attendance_muster SET DAY01 ='" + txt_day1.Text + "', DAY02 ='" + txt_day2.Text + "', DAY03 ='" + txt_day3.Text + "', DAY04 ='" + txt_day4.Text + "', DAY05 ='" + txt_day5.Text + "', DAY06 ='" + txt_day6.Text + "', DAY07 ='" + txt_day7.Text + "', DAY08 ='" + txt_day8.Text + "', DAY09 ='" + txt_day9.Text + "', DAY10 ='" + txt_day10.Text + "', DAY11 ='" + txt_day11.Text + "', DAY12 ='" + txt_day12.Text + "', DAY13 ='" + txt_day13.Text + "', DAY14 ='" + txt_day14.Text + "', DAY15 ='" + txt_day15.Text + "', DAY16 ='" + txt_day16.Text + "', DAY17 ='" + txt_day17.Text + "', DAY18 ='" + txt_day18.Text + "', DAY19 ='" + txt_day19.Text + "', DAY20 ='" + txt_day20.Text + "', DAY21 ='" + txt_day21.Text + "', DAY22 ='" + txt_day22.Text + "', DAY23 ='" + txt_day23.Text + "', DAY24 ='" + txt_day24.Text + "', DAY25 ='" + txt_day25.Text + "', DAY26 ='" + txt_day26.Text + "', DAY27 ='" + txt_day27.Text + "', DAY28 ='" + txt_day28.Text + "', DAY29 ='" + txt_day29.Text + "', DAY30 ='" + txt_day30.Text + "', DAY31 ='" + txt_day31.Text + "', TOT_DAYS_PRESENT =" + Convert.ToDouble(lbl_totpdays.Text) + ", TOT_DAYS_ABSENT =" + Convert.ToDouble(lbl_totadays.Text) + ", TOT_HALF_DAYS =" + halfdays + ",TOT_LEAVES =" + Convert.ToDouble(lbl_totleaves.Text) + ", WEEKLY_OFF =" + Convert.ToDouble(lbl_totwoff.Text) + ", HOLIDAYS =" + Convert.ToDouble(lbl_totholidays.Text) + ", TOT_CO=" + Convert.ToDouble(lbl_company_off.Text) + ",TOT_WORKING_DAYS =" + Convert.ToDouble(lbl_totwdays.Text) + " Where MONTH= '" + Session["SalaryProcessMonth"].ToString() + "' AND YEAR= '" + Session["SalaryProcessYear"].ToString() + "' AND COMP_CODE = '" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE='" + Session["SalaryProcessUnit"].ToString() + "' AND EMP_CODE='" + lbl_empcode.Text + "'";
    //                }
    //                MySqlCommand cmd_update = new MySqlCommand(update, d.con);
    //                d.conopen();
    //                cmd_update.ExecuteNonQuery();
    //                d.conclose();
    //            }
    //            int split_length = txt_date.Text.ToString().IndexOf('/');
    //            string day = txt_date.Text.ToString().Substring(0, split_length);
    //            for (int i = 0; i <= 31; i++)
    //            {
    //                if (gv_attendance.Columns[i].HeaderText == day.ToString())
    //                {
    //                    string cntrlname = "txtday" + day.ToString();
    //                    foreach (GridViewRow row in gv_attendance.Rows)
    //                    {
    //                        TextBox txtdays = (TextBox)row.FindControl(cntrlname);
    //                        txtdays.Enabled = false;
    //                        //txtdays.Visible = true;
    //                        //txtdays.Text = "P";
    //                    }
    //                }

    //            }

    //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Attendance Saved successfully!')", true);
    //            //d.reset(this);
    //        }
    //        else if (rdbmonth.Checked == false && rdbindividual.Checked == true)
    //        {
    //            int TotalRows = GridView1.Rows.Count;
    //            int split_length = txt_date.Text.ToString().IndexOf('/');
    //            string day = txt_date.Text.ToString().Substring(0, split_length);
    //            string month = txt_date.Text.Substring(3, 2);
    //            int year = Convert.ToInt32(txt_date.Text.Substring(6));
    //            for (int i = 0; i < TotalRows; i++)
    //            {
    //                Label lbl_empcode = (Label)GridView1.Rows[i].Cells[1].FindControl("lblempcode");
    //                Label lbl_empname = (Label)GridView1.Rows[i].Cells[2].FindControl("lblempname");
    //                TextBox tempday = (TextBox)GridView1.Rows[i].Cells[3].FindControl("txtday");
    //                string update = "";
    //                if (Session["SalaryProcessUnit"].ToString() == "ALL")
    //                {
    //                    update = "UPDATE pay_attendance_muster SET DAY" + day.ToString() + "='" + tempday.Text + "' WHERE month= '" + Session["SalaryProcessMonth"].ToString() + "' AND YEAR= '" + Session["SalaryProcessYear"].ToString() + "' AND COMP_CODE = '" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE in (select unit_code from pay_unit_master where client_code = '" + Session["SalaryProcessClient"].ToString() + "') AND EMP_CODE='" + lbl_empcode.Text + "'";
    //                }
    //                else
    //                {
    //                    update = "UPDATE pay_attendance_muster SET DAY" + day.ToString() + "='" + tempday.Text + "' WHERE month= '" + Session["SalaryProcessMonth"].ToString() + "' AND YEAR= '" + Session["SalaryProcessYear"].ToString() + "' AND COMP_CODE = '" + Session["COMP_CODE"].ToString() + "' AND UNIT_CODE='" + Session["SalaryProcessUnit"].ToString() + "' AND EMP_CODE='" + lbl_empcode.Text + "'";
    //                }
    //                MySqlCommand cmd_update = new MySqlCommand(update, d.con);
    //                d.conopen();
    //                cmd_update.ExecuteNonQuery();
    //                d.conclose();
    //            }
    //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Attendance for Saved successfully!')", true);
    //        }
    //        btn_process_Click(null, null);
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //}



    public override void VerifyRenderingInServerForm(Control control)
    {
        /* Verifies that the control is rendered */
    }
    //protected void btn_print_Click(object sender, EventArgs e)
    //{
    //    if (gv_attendance.Rows.Count > 0)
    //    {

    //        Response.Clear();
    //        Response.Buffer = true;
    //        Response.AddHeader("content-disposition", "attachment;filename=Attendance_Excel_data.xls");
    //        Response.Charset = "";
    //        Response.ContentType = "application/vnd.ms-excel";
    //        using (StringWriter sw = new StringWriter())
    //        {
    //            HtmlTextWriter hw = new HtmlTextWriter(sw);

    //            //To Export all pages
    //            gv_attendance.AllowPaging = false;
    //            // UnitBAL ubl1 = new UnitBAL();
    //            //DataSet ds = new DataSet();
    //            //ds = ubl1.UnitSelect();
    //            //UnitGridView.DataSource = ds.Tables["pay_unit_master"];

    //            //Session["ExlColCnt"] = GridView2.Columns.Count.ToString();
    //            foreach (TableCell cell in gv_attendance.HeaderRow.Cells)
    //            {
    //                cell.BackColor = gv_attendance.HeaderStyle.BackColor;
    //                //gv_attendance_RowCreated(sender, e);
    //            }
    //            foreach (GridViewRow row in gv_attendance.Rows)
    //            {

    //                foreach (TableCell cell in row.Cells)
    //                {
    //                    if (row.RowIndex % 2 == 0)
    //                    {
    //                        cell.BackColor = gv_attendance.AlternatingRowStyle.BackColor;
    //                    }
    //                    else
    //                    {
    //                        cell.BackColor = gv_attendance.RowStyle.BackColor;
    //                    }
    //                    cell.CssClass = "textmode";
    //                }
    //            }

    //            gv_attendance.RenderControl(hw);

    //            //style to format numbers to string
    //            string style = @"<style> .textmode { } </style>";
    //            Response.Write(style);
    //            Response.Output.Write(sw.ToString());
    //            Response.Flush();
    //            Response.End();
    //        }
    //    }
    //}
    protected void bntclose_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }


    //protected void gv_attendance_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {

    //        fill_cell(e, (TextBox)(e.Row.Cells[3].FindControl("txtday01") as TextBox), 3);
    //        fill_cell(e, (TextBox)(e.Row.Cells[4].FindControl("txtday02") as TextBox), 4);
    //        fill_cell(e, (TextBox)(e.Row.Cells[5].FindControl("txtday03") as TextBox), 5);
    //        fill_cell(e, (TextBox)(e.Row.Cells[6].FindControl("txtday04") as TextBox), 6);
    //        fill_cell(e, (TextBox)(e.Row.Cells[7].FindControl("txtday05") as TextBox), 7);
    //        fill_cell(e, (TextBox)(e.Row.Cells[8].FindControl("txtday06") as TextBox), 8);
    //        fill_cell(e, (TextBox)(e.Row.Cells[9].FindControl("txtday07") as TextBox), 9);
    //        fill_cell(e, (TextBox)(e.Row.Cells[10].FindControl("txtday08") as TextBox), 10);
    //        fill_cell(e, (TextBox)(e.Row.Cells[11].FindControl("txtday09") as TextBox), 11);
    //        fill_cell(e, (TextBox)(e.Row.Cells[12].FindControl("txtday10") as TextBox), 12);
    //        fill_cell(e, (TextBox)(e.Row.Cells[13].FindControl("txtday11") as TextBox), 13);
    //        fill_cell(e, (TextBox)(e.Row.Cells[14].FindControl("txtday12") as TextBox), 14);
    //        fill_cell(e, (TextBox)(e.Row.Cells[15].FindControl("txtday13") as TextBox), 15);
    //        fill_cell(e, (TextBox)(e.Row.Cells[16].FindControl("txtday14") as TextBox), 16);
    //        fill_cell(e, (TextBox)(e.Row.Cells[17].FindControl("txtday15") as TextBox), 17);
    //        fill_cell(e, (TextBox)(e.Row.Cells[18].FindControl("txtday16") as TextBox), 18);
    //        fill_cell(e, (TextBox)(e.Row.Cells[19].FindControl("txtday17") as TextBox), 19);
    //        fill_cell(e, (TextBox)(e.Row.Cells[20].FindControl("txtday18") as TextBox), 20);
    //        fill_cell(e, (TextBox)(e.Row.Cells[21].FindControl("txtday19") as TextBox), 21);
    //        fill_cell(e, (TextBox)(e.Row.Cells[22].FindControl("txtday20") as TextBox), 22);
    //        fill_cell(e, (TextBox)(e.Row.Cells[23].FindControl("txtday21") as TextBox), 23);
    //        fill_cell(e, (TextBox)(e.Row.Cells[24].FindControl("txtday22") as TextBox), 24);
    //        fill_cell(e, (TextBox)(e.Row.Cells[25].FindControl("txtday23") as TextBox), 25);
    //        fill_cell(e, (TextBox)(e.Row.Cells[26].FindControl("txtday24") as TextBox), 26);
    //        fill_cell(e, (TextBox)(e.Row.Cells[27].FindControl("txtday25") as TextBox), 27);
    //        fill_cell(e, (TextBox)(e.Row.Cells[28].FindControl("txtday26") as TextBox), 28);
    //        fill_cell(e, (TextBox)(e.Row.Cells[29].FindControl("txtday27") as TextBox), 29);
    //        fill_cell(e, (TextBox)(e.Row.Cells[30].FindControl("txtday28") as TextBox), 30);
    //        fill_cell(e, (TextBox)(e.Row.Cells[31].FindControl("txtday29") as TextBox), 31);
    //        fill_cell(e, (TextBox)(e.Row.Cells[32].FindControl("txtday30") as TextBox), 32);
    //        fill_cell(e, (TextBox)(e.Row.Cells[33].FindControl("txtday31") as TextBox), 33);



    //    }
    //}
    private void fill_cell(GridViewRowEventArgs e, DropDownList tb, int location)
    {

        if (tb.SelectedValue == "P")
        {
            tb.BackColor = Color.LimeGreen;
        }
        else if (tb.SelectedValue == "A")
        {
            tb.BackColor = Color.Red;
        }
        else if (tb.SelectedValue == "HD")
        {
            tb.BackColor = Color.Orange;
        }
        else if (tb.SelectedValue == "L")
        {
            tb.BackColor = Color.Yellow;
        }
        else if (tb.SelectedValue == "W")
        {
            tb.BackColor = Color.Violet;
        }
        else if (tb.SelectedValue == "H")
        {
            tb.BackColor = Color.Pink;
        }
        else if (tb.SelectedValue == "CL")
        {
            tb.BackColor = Color.Yellow;
        }
        else if (tb.SelectedValue == "PL")
        {
            tb.BackColor = Color.YellowGreen;
        }
        else if (tb.SelectedValue == "ML")
        {
            tb.BackColor = Color.Wheat;
        }
        else if (tb.SelectedValue == "PH")
        {
            tb.BackColor = Color.Aqua;
        }
        else if (tb.SelectedValue == "CO")
        {
            tb.BackColor = Color.Peru;
        }
    }

    //protected void gv_attendance_RowCreated(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.Header)
    //    {
    //        int columns = 41;

    //        //-----COMPANY HEADER -------------
    //        GridView HeaderGrid = (GridView)sender;
    //        GridViewRow HeaderGridRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
    //        TableCell HeaderCell = new TableCell();
    //        HeaderCell.Text = Session["COMP_NAME"].ToString();
    //        //            HeaderCell.ColumnSpan = 5;
    //        HeaderCell.ColumnSpan = 41;
    //        HeaderGridRow.Cells.Add(HeaderCell);
    //        gv_attendance.Controls[0].Controls.AddAt(0, HeaderGridRow);

    //        //------ UNIT HEADER -------------

    //        GridViewRow HeaderGridRow1 = new GridViewRow(1, 1, DataControlRowType.Header, DataControlRowState.Insert);
    //        TableCell HeaderCell1 = new TableCell();
    //       // HeaderCell1.Text = lbl_units.Text;
    //        //            HeaderCell.ColumnSpan = 5;
    //        HeaderCell1.ColumnSpan = 41;
    //        HeaderGridRow1.Cells.Add(HeaderCell1);
    //        gv_attendance.Controls[0].Controls.AddAt(1, HeaderGridRow1);

    //        //----- Attendance -----------------------
    //        GridViewRow HeaderGridRow2 = new GridViewRow(2, 2, DataControlRowType.Header, DataControlRowState.Insert);
    //        TableCell HeaderCell2 = new TableCell();
    //        //DateTime dt = Convert.ToDateTime(lbl_month_year.Text);
    //        //string month = dt.ToString("MMMM");
    //        //HeaderCell2.Text = "Attendance Register : " + month + " - " + lbl_month_year.Text.Substring(3);


    //        //                HeaderCell3.ColumnSpan = 5;
    //        HeaderCell2.ColumnSpan = 41;
    //        HeaderGridRow2.Cells.Add(HeaderCell2);
    //        gv_attendance.Controls[0].Controls.AddAt(2, HeaderGridRow2);

    //        HeaderGridRow.BackColor = ColorTranslator.FromHtml("#fff");
    //        HeaderGridRow.ForeColor = ColorTranslator.FromHtml("#000");

    //        HeaderGridRow1.BackColor = ColorTranslator.FromHtml("#fff");
    //        HeaderGridRow1.ForeColor = ColorTranslator.FromHtml("#000");

    //        HeaderGridRow2.BackColor = ColorTranslator.FromHtml("#fff");
    //        HeaderGridRow2.ForeColor = ColorTranslator.FromHtml("#000");

    //    }
    //}


    //protected void btn_Upload_Click(object sender, EventArgs e)
    //{

    //    if (FileUpload1.HasFile)
    //    {

    //        try
    //        {
    //            string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
    //            string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
    //            string FolderPath = ConfigurationManager.AppSettings["FolderPath"];
    //            string FilePath = Server.MapPath(FolderPath + FileName);
    //            FileUpload1.SaveAs(FilePath);
    //            btn_Import_Click(FilePath, Extension, "Yes");
    //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Attendance Added Successfully...')", true);


    //        }
    //        catch (Exception ee)
    //        {
    //            ee.Message.ToString();
    //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('System Error- Please Try agains....')", true);

    //        }

    //        finally
    //        {

    //        }
    //    }


    //}

    //public void btn_Import_Click(string FilePath, string Extension, string IsHDR)
    //{
    //    string conStr = "";
    //    switch (Extension)
    //    {
    //        case ".xls":
    //            conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
    //            break;
    //        case ".xlsx":
    //            conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
    //            break;
    //    }
    //    conStr = String.Format(conStr, FilePath, IsHDR);
    //    OleDbConnection connExcel = new OleDbConnection(conStr);
    //    OleDbCommand cmdExcel = new OleDbCommand();
    //    OleDbDataAdapter oda = new OleDbDataAdapter();
    //    System.Data.DataTable dt = new System.Data.DataTable();
    //    cmdExcel.Connection = connExcel;

    //    // Get The Name of First Sheet
    //    connExcel.Open();
    //    System.Data.DataTable dtExcelSchema;
    //    dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

    //    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
    //    connExcel.Close();

    //    //Read Data from First Sheet

    //    connExcel.Open();
    //    cmdExcel.CommandText = "SELECT * FROM [" + sheetName + "]";
    //    oda.SelectCommand = cmdExcel;
    //    oda.Fill(dt);
    //    connExcel.Close();

    //    //Push Datatable to database

    //    // string Mysql = "update  pay_attendance_muster set DAY01=@E,DAY02=@F,DAY03=@G,DAY04=@H,DAY05=@I,DAY06=@J,DAY07=@K,DAY08=@L,DAY09=@M,DAY10=@N,DAY11=@O,DAY12=@P,DAY13=@Q,DAY14=@R,DAY15=@S,DAY16=@T,DAY17=@U,DAY18=@V,DAY19=@W,DAY20=@X,DAY21=@Y,DAY22=@Z,DAY23=@AA,DAY24=@AB,DAY25=@AC,DAY26=@AD,DAY27=@AE,DAY28=@AF,DAY29=@AG,DAY30=@AH,DAY31=@AI,TOT_DAYS_PRESENT=@AJ,TOT_DAYS_ABSENT=@AK,TOT_CL=@AL,TOT_PL=@AM,TOT_MATERNITY=@AN,TOT_PATERNITY=@AO,TOT_LEAVES=@AP,WEEKLY_OFF=@AQ,HOLIDAYS=@AR,TOT_WORKING_DAYS=@AS where emp_code=@C and comp_code=@A and unit_code=@B and MONTH = @AT AND YEAR=@AU ";

    //    string Mysql = "update  pay_attendance_muster set DAY01=@H,DAY02=@I,DAY03=@J,DAY04=@K,DAY05=@L,DAY06=@M,DAY07=@N,DAY08=@O,DAY09=@P,DAY10=@Q,DAY11=@R,DAY12=@S,DAY13=@T,DAY14=@U,DAY15=@V,DAY16=@W,DAY17=@X,DAY18=@Y,DAY19=@Z,DAY20=@AA,DAY21=@AB,DAY22=@AC,DAY23=@AD,DAY24=@AE,DAY25=@AF,DAY26=@AG,DAY27=@AH,DAY28=@AI,DAY29=@AJ,DAY30=@AK,DAY31=@AL where emp_code=@C and comp_code=@A and unit_code=@B and MONTH = @AX AND YEAR=@AY";

    //    for (int i = 0; i <= 3; i++)
    //    {
    //        dt.Rows.RemoveAt(0);

    //    }
    //    dt.AcceptChanges();

    //    d.con.Open();
    //    foreach (DataRow r in dt.Rows)
    //    {
    //        string COMP_CODE = "", UNIT_CODE = "", EMP_CODE = "", gender = "", department = "", reportingto = "", DAY01 = "", DAY02 = "", DAY03 = "", DAY04 = "", DAY05 = "", DAY06 = "", DAY07 = "", DAY08 = "", DAY09 = "", DAY10 = "", DAY11 = "", DAY12 = "", DAY13 = "", DAY14 = "", DAY15 = "", DAY16 = "", DAY17 = "", DAY18 = "", DAY19 = "", DAY20 = "", DAY21 = "", DAY22 = "", DAY23 = "", DAY24 = "", DAY25 = "", DAY26 = "", DAY27 = "", DAY28 = "", DAY29 = "", DAY30 = "", DAY31 = "";
    //        double TOTAL_PRESENT_DAYS = 0, TOTAL_ABSENT_DAYS = 0, CASUAL_LEAVES = 0, PREVILEGE_LEAVES = 0, MATERNITY_LEAVE = 0,
    //            PATERNITY_LEAVE = 0, TOTAL_LEAVES = 0, WEEKLYOFF = 0, HOLIDAYS = 0, TOTAL_PAYABLE_DAYS = 0, COMPANY_OFF = 0;
    //        string MONTH = "", YEAR = ""; string TEMP_DAY = "";

    //        if (r[0].ToString() != "") { COMP_CODE = r[0].ToString(); }
    //        if (r[1].ToString() != "") { UNIT_CODE = r[1].ToString(); }
    //        if (r[2].ToString() != "") { EMP_CODE = r[2].ToString(); }
    //        if (r[7].ToString() != "") { DAY01 = r[7].ToString(); }
    //        if (r[8].ToString() != "") { DAY02 = r[8].ToString(); }
    //        if (r[9].ToString() != "") { DAY03 = r[9].ToString(); }
    //        if (r[10].ToString() != "") { DAY04 = r[10].ToString(); }
    //        if (r[11].ToString() != "") { DAY05 = r[11].ToString(); }
    //        if (r[12].ToString() != "") { DAY06 = r[12].ToString(); }
    //        if (r[13].ToString() != "") { DAY07 = r[13].ToString(); }
    //        if (r[14].ToString() != "") { DAY08 = r[14].ToString(); }
    //        if (r[15].ToString() != "") { DAY09 = r[15].ToString(); }
    //        if (r[16].ToString() != "") { DAY10 = r[16].ToString(); }
    //        if (r[17].ToString() != "") { DAY11 = r[17].ToString(); }
    //        if (r[18].ToString() != "") { DAY12 = r[18].ToString(); }
    //        if (r[19].ToString() != "") { DAY13 = r[19].ToString(); }
    //        if (r[20].ToString() != "") { DAY14 = r[20].ToString(); }
    //        if (r[21].ToString() != "") { DAY15 = r[21].ToString(); }
    //        if (r[22].ToString() != "") { DAY16 = r[22].ToString(); }
    //        if (r[23].ToString() != "") { DAY17 = r[23].ToString(); }
    //        if (r[24].ToString() != "") { DAY18 = r[24].ToString(); }
    //        if (r[25].ToString() != "") { DAY19 = r[25].ToString(); }
    //        if (r[26].ToString() != "") { DAY20 = r[26].ToString(); }
    //        if (r[27].ToString() != "") { DAY21 = r[27].ToString(); }
    //        if (r[28].ToString() != "") { DAY22 = r[28].ToString(); }
    //        if (r[29].ToString() != "") { DAY23 = r[29].ToString(); }
    //        if (r[30].ToString() != "") { DAY24 = r[30].ToString(); }
    //        if (r[31].ToString() != "") { DAY25 = r[31].ToString(); }
    //        if (r[32].ToString() != "") { DAY26 = r[32].ToString(); }
    //        if (r[33].ToString() != "") { DAY27 = r[33].ToString(); }
    //        if (r[34].ToString() != "") { DAY28 = r[34].ToString(); }
    //        if (r[35].ToString() != "") { DAY29 = r[35].ToString(); }
    //        if (r[36].ToString() != "") { DAY30 = r[36].ToString(); }
    //        if (r[37].ToString() != "") { DAY31 = r[37].ToString(); }
    //        if (r[38].ToString() != "") { TOTAL_PRESENT_DAYS = double.Parse(r[38].ToString()); }
    //        if (r[39].ToString() != "") { TOTAL_ABSENT_DAYS = double.Parse(r[39].ToString()); }
    //        if (r[40].ToString() != "") { CASUAL_LEAVES = double.Parse(r[40].ToString()); }
    //        if (r[41].ToString() != "") { PREVILEGE_LEAVES = double.Parse(r[41].ToString()); }
    //        if (r[42].ToString() != "") { MATERNITY_LEAVE = double.Parse(r[42].ToString()); }
    //        if (r[43].ToString() != "") { PATERNITY_LEAVE = double.Parse(r[43].ToString()); }
    //        if (r[44].ToString() != "") { TOTAL_LEAVES = double.Parse(r[44].ToString()); }
    //        if (r[45].ToString() != "") { WEEKLYOFF = double.Parse(r[45].ToString()); }
    //        if (r[46].ToString() != "") { HOLIDAYS = double.Parse(r[46].ToString()); }
    //        if (r[47].ToString() != "") { COMPANY_OFF = double.Parse(r[47].ToString()); }
    //        if (r[48].ToString() != "") { TOTAL_PAYABLE_DAYS = double.Parse(r[48].ToString()); }
    //        if (r[49].ToString() != "") { MONTH = r[49].ToString(); }
    //        if (r[50].ToString() != "") { YEAR = r[50].ToString(); }

    //        MySqlCommand cmd = d.con.CreateCommand();
    //        cmd.CommandText = Mysql;
    //        cmd.Parameters.AddWithValue("@A", COMP_CODE.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@B", UNIT_CODE.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@C", EMP_CODE.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@H", DAY01.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@I", DAY02.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@J", DAY03.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@K", DAY04.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@L", DAY05.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@M", DAY06.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@N", DAY07.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@O", DAY08.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@P", DAY09.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@Q", DAY10.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@R", DAY11.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@S", DAY12.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@T", DAY13.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@U", DAY14.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@V", DAY15.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@W", DAY16.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@X", DAY17.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@Y", DAY18.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@Z", DAY19.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AA", DAY20.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AB", DAY21.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AC", DAY22.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AD", DAY23.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AE", DAY24.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AF", DAY25.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AG", DAY26.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AH", DAY27.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AI", DAY28.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AJ", DAY29.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AK", DAY30.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AL", DAY31.Trim().ToUpper());
    //        cmd.Parameters.AddWithValue("@AM", TOTAL_PRESENT_DAYS);
    //        cmd.Parameters.AddWithValue("@AN", TOTAL_ABSENT_DAYS);
    //        cmd.Parameters.AddWithValue("@AO", CASUAL_LEAVES);
    //        cmd.Parameters.AddWithValue("@AP", PREVILEGE_LEAVES);
    //        cmd.Parameters.AddWithValue("@AQ", MATERNITY_LEAVE);
    //        cmd.Parameters.AddWithValue("@AR", PATERNITY_LEAVE);
    //        cmd.Parameters.AddWithValue("@AS", TOTAL_LEAVES);
    //        cmd.Parameters.AddWithValue("@AT", WEEKLYOFF);
    //        cmd.Parameters.AddWithValue("@AU", HOLIDAYS);
    //        cmd.Parameters.AddWithValue("@AV", COMPANY_OFF);
    //        cmd.Parameters.AddWithValue("@AW", TOTAL_PAYABLE_DAYS);
    //        cmd.Parameters.AddWithValue("@AX", MONTH);
    //        cmd.Parameters.AddWithValue("@AY", YEAR);

    //        //d.con.Open();
    //        cmd.ExecuteNonQuery();

    //    }
    //    d.con.Close();
    //    btn_process_Click(null, null);
    //}

    //protected void btn_Export_CheckedChanged(object sender, EventArgs e)
    //{

    //    Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
    //    Microsoft.Office.Interop.Excel.Workbook wb = xla.Workbooks.Add(Microsoft.Office.Interop.Excel.XlSheetType.xlWorksheet);
    //    Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)xla.ActiveSheet;
    //    xla.Columns.ColumnWidth = 20;

    //    //Microsoft.Office.Interop.Excel.Range rng12 = ws.get_Range(ws.Cells[1, 1], ws.Cells[500, 300]);
    //    //rng12.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;


    //    Microsoft.Office.Interop.Excel.Range rng = ws.get_Range("E1:F1");
    //    rng.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbDarkGreen;

    //    Microsoft.Office.Interop.Excel.Range formateRange2 = ws.get_Range("E1:F1");
    //    formateRange2.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

    //    //ws.Range["E1"].Text = "Large size";

    //    formateRange2.Font.Size = 20;
    //    //formateRange2.Font.IsBold = true;


    //    Microsoft.Office.Interop.Excel.Range rng1 = ws.get_Range("E2:G2");
    //    rng1.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbGreen;

    //    Microsoft.Office.Interop.Excel.Range formateRange1 = ws.get_Range("E2:G2");
    //    formateRange1.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
    //    formateRange1.Font.Size = 20;


    //    Microsoft.Office.Interop.Excel.Range rng4 = ws.get_Range("C3:L3");
    //    rng4.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbGreen;
    //    rng4.Font.Size = 15;

    //    Microsoft.Office.Interop.Excel.Range formateRange4 = ws.get_Range("C3:L3");
    //    formateRange4.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
    //    formateRange4.Font.Size = 15;

    //    Microsoft.Office.Interop.Excel.Range rng2 = ws.get_Range("A5:AY5");
    //    rng2.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbBlue;
    //    rng2.Font.Size = 15;

    //    Microsoft.Office.Interop.Excel.Range formateRange = ws.get_Range("A5:AY5");
    //    formateRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
    //    formateRange.Font.Size = 15;

    //    ws.Cells[1, 5] = Session["SalaryProcessUnit"].ToString();
    //    ws.Cells[2, 5] = "Employee Attendace_" + txt_month.Text.ToString();
    //    ws.Cells[3, 3] = "Note : P : Present, A : Absent, F : Half Day, CL : Casual Leave, PH : Paternity Leaves, ML : Maternity Leave , PL : Privilege Leave, W : Weekly Off, H : Holiday, CO : Company Off";

    //    ws.Cells[5, 1] = "COMPANY CODE";
    //    ws.Cells[5, 2] = "UNIT CODE";
    //    ws.Cells[5, 3] = "EMPLOYEE CODE";
    //    ws.Cells[5, 4] = "EMPLOYEE NAME";
    //    ws.Cells[5, 5] = "GENDER";
    //    ws.Cells[5, 6] = "DEPRTMENT";
    //    ws.Cells[5, 7] = "REPORTING TO";
    //    ws.Cells[5, 8] = "DAY01";
    //    ws.Cells[5, 9] = "DAY02";
    //    ws.Cells[5, 10] = "DAY03";
    //    ws.Cells[5, 11] = "DAY04";
    //    ws.Cells[5, 12] = "DAY05";
    //    ws.Cells[5, 13] = "DAY06";
    //    ws.Cells[5, 14] = "DAY07";
    //    ws.Cells[5, 15] = "DAY08";
    //    ws.Cells[5, 16] = "DAY09";
    //    ws.Cells[5, 17] = "DAY10";
    //    ws.Cells[5, 18] = "DAY11";
    //    ws.Cells[5, 19] = "DAY12";
    //    ws.Cells[5, 20] = "DAY13";
    //    ws.Cells[5, 21] = "DAY14";
    //    ws.Cells[5, 22] = "DAY15";
    //    ws.Cells[5, 23] = "DAY16";
    //    ws.Cells[5, 24] = "DAY17";
    //    ws.Cells[5, 25] = "DAY18";
    //    ws.Cells[5, 26] = "DAY19";
    //    ws.Cells[5, 27] = "DAY20";
    //    ws.Cells[5, 28] = "DAY21";
    //    ws.Cells[5, 29] = "DAY22";
    //    ws.Cells[5, 30] = "DAY23";
    //    ws.Cells[5, 31] = "DAY24";
    //    ws.Cells[5, 32] = "DAY25";
    //    ws.Cells[5, 33] = "DAY26";
    //    ws.Cells[5, 34] = "DAY27";
    //    ws.Cells[5, 35] = "DAY28";
    //    ws.Cells[5, 36] = "DAY29";
    //    ws.Cells[5, 37] = "DAY30";
    //    ws.Cells[5, 38] = "DAY31";
    //    ws.Cells[5, 39] = "TOTAL PRESENT DAYS";
    //    ws.Cells[5, 40] = "TOTAL ABSENT DAYS";
    //    ws.Cells[5, 41] = "CASUAL LEAVES";
    //    ws.Cells[5, 42] = "PREVILEGE LEAVES";
    //    ws.Cells[5, 43] = "MATERNITY LEAVE";
    //    ws.Cells[5, 44] = "PATERNITY LEAVE";
    //    ws.Cells[5, 45] = "TOTAL LEAVES";
    //    ws.Cells[5, 46] = "WEEKLYOFF";
    //    ws.Cells[5, 47] = "HOLIDAYS";
    //    ws.Cells[5, 48] = "COMPANY OFF";
    //    ws.Cells[5, 49] = "TOTAL PAYABLE DAYS";
    //    ws.Cells[5, 50] = "MONTH";
    //    ws.Cells[5, 51] = "YEAR";


    //    String year = txt_date.Text.Substring(6, 4).ToString();
    //    String month = txt_date.Text.Substring(3, 2).ToString();
    //    try
    //    {
    //        d.con1.Open();
    //        DataSet ds2 = new DataSet();
    //        // MySqlDataAdapter adp2 = new MySqlDataAdapter("SELECT pay_attendance_muster.COMP_CODE, pay_attendance_muster.UNIT_CODE, pay_attendance_muster.EMP_CODE, pay_employee_master .EMP_NAME, pay_attendance_muster.DAY01, pay_attendance_muster.DAY02, pay_attendance_muster.DAY03, pay_attendance_muster.DAY04, pay_attendance_muster.DAY05, pay_attendance_muster.DAY06, pay_attendance_muster.DAY07, pay_attendance_muster.DAY08, pay_attendance_muster.DAY09, pay_attendance_muster.DAY10, pay_attendance_muster.DAY11, pay_attendance_muster.DAY12, pay_attendance_muster.DAY13, pay_attendance_muster.DAY14, pay_attendance_muster.DAY15, pay_attendance_muster.DAY16, pay_attendance_muster.DAY17, pay_attendance_muster.DAY18, pay_attendance_muster.DAY19, pay_attendance_muster.DAY20, pay_attendance_muster.DAY21, pay_attendance_muster.DAY22, pay_attendance_muster.DAY23, pay_attendance_muster.DAY24, pay_attendance_muster.DAY25, pay_attendance_muster.DAY26, pay_attendance_muster.DAY27, pay_attendance_muster.DAY28, pay_attendance_muster.DAY29, pay_attendance_muster.DAY30, pay_attendance_muster.DAY31, pay_attendance_muster.TOT_DAYS_PRESENT,pay_attendance_muster.TOT_DAYS_ABSENT, pay_attendance_muster.TOT_CL,pay_attendance_muster.TOT_PL,pay_attendance_muster.TOT_MATERNITY,pay_attendance_muster.TOT_PATERNITY,pay_attendance_muster.TOT_LEAVES, pay_attendance_muster.WEEKLY_OFF, pay_attendance_muster.HOLIDAYS, pay_attendance_muster.TOT_CO,pay_attendance_muster.TOT_WORKING_DAYS, concat('''',pay_attendance_muster.MONTH) as Month, pay_attendance_muster.YEAR,(select emp_name from pay_employee_master a where a.emp_code = pay_employee_master.REPORTING_TO) As 'Reporting To' FROM pay_attendance_muster,pay_employee_master  WHERE pay_attendance_muster .EMP_CODE =pay_employee_master .EMP_CODE  AND pay_attendance_muster .COMP_CODE ='" + Session["COMP_CODE"].ToString() + "'  AND pay_attendance_muster .UNIT_CODE ='" + Session["SalaryProcessUnit"].ToString()ToString().Substring(0, 4) + "' and month='" + month + "' and year='" + year + "' ORDER BY  pay_employee_master .EMP_CODE", d.con1);
    //        MySqlDataAdapter adp2 = new MySqlDataAdapter("SELECT pay_attendance_muster.COMP_CODE, pay_attendance_muster.UNIT_CODE, pay_attendance_muster.EMP_CODE,pay_employee_master.EMP_NAME,case pay_employee_master.GENDER when 'M' then 'MALE' when 'F' then 'FEMALE' END as 'GENDER',pay_department_master.DEPT_NAME, (select emp_name from pay_employee_master a where a.emp_code = pay_employee_master.REPORTING_TO) As 'Reporting To' ,pay_attendance_muster.DAY01, pay_attendance_muster.DAY02,pay_attendance_muster.DAY03, pay_attendance_muster.DAY04, pay_attendance_muster.DAY05,pay_attendance_muster.DAY06, pay_attendance_muster.DAY07, pay_attendance_muster.DAY08, pay_attendance_muster.DAY09, pay_attendance_muster.DAY10, pay_attendance_muster.DAY11, pay_attendance_muster.DAY12, pay_attendance_muster.DAY13, pay_attendance_muster.DAY14, pay_attendance_muster.DAY15, pay_attendance_muster.DAY16, pay_attendance_muster.DAY17, pay_attendance_muster.DAY18, pay_attendance_muster.DAY19, pay_attendance_muster.DAY20, pay_attendance_muster.DAY21, pay_attendance_muster.DAY22, pay_attendance_muster.DAY23, pay_attendance_muster.DAY24, pay_attendance_muster.DAY25, pay_attendance_muster.DAY26, pay_attendance_muster.DAY27, pay_attendance_muster.DAY28, pay_attendance_muster.DAY29, pay_attendance_muster.DAY30, pay_attendance_muster.DAY31, pay_attendance_muster.TOT_DAYS_PRESENT,pay_attendance_muster.TOT_DAYS_ABSENT, pay_attendance_muster.TOT_CL,pay_attendance_muster.TOT_PL,pay_attendance_muster.TOT_MATERNITY,pay_attendance_muster.TOT_PATERNITY,pay_attendance_muster.TOT_LEAVES, pay_attendance_muster.WEEKLY_OFF, pay_attendance_muster.HOLIDAYS, pay_attendance_muster.TOT_CO,pay_attendance_muster.TOT_WORKING_DAYS, concat('''',pay_attendance_muster.MONTH) as Month, pay_attendance_muster.YEAR FROM pay_attendance_muster,pay_employee_master,pay_department_master  WHERE pay_attendance_muster .EMP_CODE =pay_employee_master .EMP_CODE  and pay_employee_master.DEPT_CODE= pay_department_master.DEPT_CODE  AND pay_attendance_muster .COMP_CODE ='" + Session["COMP_CODE"].ToString() + "'  AND pay_attendance_muster .UNIT_CODE ='" + Session["SalaryProcessUnit"].ToString() + "' and month='" + month + "' and year='" + year + "' ORDER BY  pay_employee_master .EMP_CODE", d.con1);
    //        System.Data.DataTable dt = new System.Data.DataTable();
    //        adp2.Fill(dt);
    //        int j = 6;

    //        foreach (System.Data.DataRow row in dt.Rows)
    //        {
    //            for (int i = 0; i < dt.Columns.Count; i++)
    //            {
    //                ws.Cells[j, i + 1] = row[i].ToString();

    //            }
    //            j++;

    //        }
    //        //ws.Range["A6"].Locked =false;
    //        //ws.get_Range("A1", "Q12").Style.Locked = true;
    //    }
    //    catch (Exception ee)
    //    {
    //        Response.Write(ee.Message);
    //    }
    //    finally
    //    {
    //        xla.Visible = true;
    //        d.con1.Close();
    //    }

    //}

    protected void gv_attendance_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_attendance.UseAccessibleHeader = false;
            gv_attendance.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch

    }


    protected void shiftcalendar_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView drv = e.Row.DataItem as DataRowView;
            DropDownList ddlCategories1 = e.Row.FindControl("DropDownList1") as DropDownList;
            DropDownList ddlCategories2 = e.Row.FindControl("DropDownList2") as DropDownList;
            DropDownList ddlCategories3 = e.Row.FindControl("DropDownList3") as DropDownList;
            DropDownList ddlCategories4 = e.Row.FindControl("DropDownList4") as DropDownList;
            DropDownList ddlCategories5 = e.Row.FindControl("DropDownList5") as DropDownList;
            DropDownList ddlCategories6 = e.Row.FindControl("DropDownList6") as DropDownList;
            DropDownList ddlCategories7 = e.Row.FindControl("DropDownList7") as DropDownList;
            DropDownList ddlCategories8 = e.Row.FindControl("DropDownList8") as DropDownList;
            DropDownList ddlCategories9 = e.Row.FindControl("DropDownList9") as DropDownList;
            DropDownList ddlCategories10 = e.Row.FindControl("DropDownList10") as DropDownList;
            DropDownList ddlCategories11 = e.Row.FindControl("DropDownList11") as DropDownList;
            DropDownList ddlCategories12 = e.Row.FindControl("DropDownList12") as DropDownList;
            DropDownList ddlCategories13 = e.Row.FindControl("DropDownList13") as DropDownList;
            DropDownList ddlCategories14 = e.Row.FindControl("DropDownList14") as DropDownList;
            DropDownList ddlCategories15 = e.Row.FindControl("DropDownList15") as DropDownList;
            DropDownList ddlCategories16 = e.Row.FindControl("DropDownList16") as DropDownList;
            DropDownList ddlCategories17 = e.Row.FindControl("DropDownList17") as DropDownList;
            DropDownList ddlCategories18 = e.Row.FindControl("DropDownList18") as DropDownList;
            DropDownList ddlCategories19 = e.Row.FindControl("DropDownList19") as DropDownList;
            DropDownList ddlCategories20 = e.Row.FindControl("DropDownList20") as DropDownList;
            DropDownList ddlCategories21 = e.Row.FindControl("DropDownList21") as DropDownList;
            DropDownList ddlCategories22 = e.Row.FindControl("DropDownList22") as DropDownList;
            DropDownList ddlCategories23 = e.Row.FindControl("DropDownList23") as DropDownList;
            DropDownList ddlCategories24 = e.Row.FindControl("DropDownList24") as DropDownList;
            DropDownList ddlCategories25 = e.Row.FindControl("DropDownList25") as DropDownList;
            DropDownList ddlCategories26 = e.Row.FindControl("DropDownList26") as DropDownList;
            DropDownList ddlCategories27 = e.Row.FindControl("DropDownList27") as DropDownList;
            DropDownList ddlCategories28 = e.Row.FindControl("DropDownList28") as DropDownList;
            DropDownList ddlCategories29 = e.Row.FindControl("DropDownList29") as DropDownList;
            DropDownList ddlCategories30 = e.Row.FindControl("DropDownList30") as DropDownList;
            DropDownList ddlCategories31 = e.Row.FindControl("DropDownList31") as DropDownList;

            //ddlCategories1.Items.Insert(0, new ListItem("Select Shift", "0"));
            if (ddlCategories1 != null)
            {
                ddlCategories1.SelectedValue = drv["DAY01"].ToString();
            }
            else { ddlCategories1.SelectedValue = "0"; }
            fill_cell(e, ddlCategories1, 41);
            if (ddlCategories2 != null)
            {
                ddlCategories2.SelectedValue = drv["DAY02"].ToString();
            }
            else { ddlCategories2.SelectedValue = "0"; } fill_cell(e, ddlCategories2, 42);
            if (ddlCategories3 != null)
            {
                ddlCategories3.SelectedValue = drv["DAY03"].ToString();
            }
            else { ddlCategories3.SelectedValue = "0"; } fill_cell(e, ddlCategories3, 43);
            if (ddlCategories4 != null)
            {
                ddlCategories4.SelectedValue = drv["DAY04"].ToString();
            }
            else { ddlCategories4.SelectedValue = "0"; } fill_cell(e, ddlCategories4, 44);
            if (ddlCategories5 != null)
            {
                ddlCategories5.SelectedValue = drv["DAY05"].ToString();
            }
            else { ddlCategories5.SelectedValue = "0"; } fill_cell(e, ddlCategories5, 45);
            if (ddlCategories6 != null)
            {
                ddlCategories6.SelectedValue = drv["DAY06"].ToString();
            }
            else { ddlCategories6.SelectedValue = "0"; } fill_cell(e, ddlCategories6, 46);
            if (ddlCategories7 != null)
            {
                ddlCategories7.SelectedValue = drv["DAY07"].ToString();
            }
            else { ddlCategories7.SelectedValue = "0"; } fill_cell(e, ddlCategories7, 47);
            if (ddlCategories8 != null)
            {
                ddlCategories8.SelectedValue = drv["DAY08"].ToString();
            }
            else { ddlCategories8.SelectedValue = "0"; } fill_cell(e, ddlCategories8, 48);
            if (ddlCategories9 != null)
            {
                ddlCategories9.SelectedValue = drv["DAY09"].ToString();
            }
            else { ddlCategories9.SelectedValue = "0"; } fill_cell(e, ddlCategories9, 49);
            if (ddlCategories10 != null)
            {
                ddlCategories10.SelectedValue = drv["DAY10"].ToString();
            }
            else { ddlCategories10.SelectedValue = "0"; } fill_cell(e, ddlCategories10, 50);
            if (ddlCategories11 != null)
            {
                ddlCategories11.SelectedValue = drv["DAY11"].ToString();
            }
            else { ddlCategories11.SelectedValue = "0"; } fill_cell(e, ddlCategories11, 51);
            if (ddlCategories2 != null)
            {
                ddlCategories12.SelectedValue = drv["DAY12"].ToString();
            }
            else { ddlCategories12.SelectedValue = "0"; } fill_cell(e, ddlCategories12, 52);
            if (ddlCategories13 != null)
            {
                ddlCategories13.SelectedValue = drv["DAY13"].ToString();
            }
            else { ddlCategories13.SelectedValue = "0"; } fill_cell(e, ddlCategories13, 53);
            if (ddlCategories14 != null)
            {
                ddlCategories14.SelectedValue = drv["DAY14"].ToString();
            }
            else { ddlCategories14.SelectedValue = "0"; } fill_cell(e, ddlCategories14, 54);
            if (ddlCategories15 != null)
            {
                ddlCategories15.SelectedValue = drv["DAY15"].ToString();
            }
            else { ddlCategories15.SelectedValue = "0"; } fill_cell(e, ddlCategories15, 55);
            if (ddlCategories16 != null)
            {
                ddlCategories16.SelectedValue = drv["DAY16"].ToString();
            }
            else { ddlCategories16.SelectedValue = "0"; } fill_cell(e, ddlCategories16, 56);
            if (ddlCategories17 != null)
            {
                ddlCategories17.SelectedValue = drv["DAY17"].ToString();
            }
            else { ddlCategories17.SelectedValue = "0"; } fill_cell(e, ddlCategories17, 57);
            if (ddlCategories18 != null)
            {
                ddlCategories18.SelectedValue = drv["DAY18"].ToString();
            }
            else { ddlCategories18.SelectedValue = "0"; } fill_cell(e, ddlCategories18, 58);
            if (ddlCategories19 != null)
            {
                ddlCategories19.SelectedValue = drv["DAY19"].ToString();
            }
            else { ddlCategories19.SelectedValue = "0"; } fill_cell(e, ddlCategories19, 59);
            if (ddlCategories20 != null)
            {
                ddlCategories20.SelectedValue = drv["DAY20"].ToString();
            }
            else { ddlCategories20.SelectedValue = "0"; } fill_cell(e, ddlCategories20, 60);
            if (ddlCategories21 != null)
            {
                ddlCategories21.SelectedValue = drv["DAY21"].ToString();
            }
            else { ddlCategories21.SelectedValue = "0"; } fill_cell(e, ddlCategories21, 61);
            if (ddlCategories22 != null)
            {
                ddlCategories22.SelectedValue = drv["DAY22"].ToString();
            }
            else { ddlCategories22.SelectedValue = "0"; } fill_cell(e, ddlCategories22, 62);
            if (ddlCategories23 != null)
            {
                ddlCategories23.SelectedValue = drv["DAY23"].ToString();
            }
            else { ddlCategories23.SelectedValue = "0"; } fill_cell(e, ddlCategories23, 63);
            if (ddlCategories24 != null)
            {
                ddlCategories24.SelectedValue = drv["DAY24"].ToString();
            }
            else { ddlCategories24.SelectedValue = "0"; } fill_cell(e, ddlCategories24, 64);
            if (ddlCategories25 != null)
            {
                ddlCategories25.SelectedValue = drv["DAY25"].ToString();
            }
            else { ddlCategories25.SelectedValue = "0"; } fill_cell(e, ddlCategories25, 65);
            if (ddlCategories26 != null)
            {
                ddlCategories26.SelectedValue = drv["DAY26"].ToString();
            }
            else { ddlCategories26.SelectedValue = "0"; } fill_cell(e, ddlCategories26, 66);
            if (ddlCategories27 != null)
            {
                ddlCategories27.SelectedValue = drv["DAY27"].ToString();
            }
            else { ddlCategories27.SelectedValue = "0"; } fill_cell(e, ddlCategories27, 67);
            if (ddlCategories28 != null)
            {
                ddlCategories28.SelectedValue = drv["DAY28"].ToString();
            }
            else { ddlCategories28.SelectedValue = "0"; } fill_cell(e, ddlCategories28, 68);
            if (ddlCategories29 != null)
            {
                ddlCategories29.SelectedValue = drv["DAY29"].ToString();
            }
            else { ddlCategories29.SelectedValue = "0"; } fill_cell(e, ddlCategories29, 69);
            if (ddlCategories30 != null)
            {
                ddlCategories30.SelectedValue = drv["DAY30"].ToString();
            }
            else { ddlCategories30.SelectedValue = "0"; } fill_cell(e, ddlCategories30, 70);
            if (ddlCategories31 != null)
            {
                ddlCategories31.SelectedValue = drv["DAY31"].ToString();
            }
            else { ddlCategories31.SelectedValue = "0"; } fill_cell(e, ddlCategories31, 71);

            //string temp = d1.getsinglestring("select OT from pay_client_master where client_code = '" + Session["SalaryProcessClient"].ToString() + "'");
            //if (temp == "0")
            //{
            //    TextBox txtothours = e.Row.FindControl("txt_ot_hours") as TextBox;
            //    txtothours.ReadOnly = true;
            //}
            //else
            //{
            //    TextBox txtothours = e.Row.FindControl("txt_ot_hours") as TextBox;
            //    txtothours.ReadOnly = false;
            //}
        }
        e.Row.Cells[1].Visible = false;
        e.Row.Cells[10].Visible = false;

        e.Row.Cells[11].Visible = false;
        e.Row.Cells[12].Visible = false;
        e.Row.Cells[13].Visible = false;
        e.Row.Cells[14].Visible = false;
        e.Row.Cells[15].Visible = false;
        e.Row.Cells[16].Visible = false;
        e.Row.Cells[17].Visible = false;
        e.Row.Cells[18].Visible = false;
        e.Row.Cells[19].Visible = false;
        e.Row.Cells[20].Visible = false;

        e.Row.Cells[21].Visible = false;
        e.Row.Cells[22].Visible = false;
        e.Row.Cells[23].Visible = false;
        e.Row.Cells[24].Visible = false;
        e.Row.Cells[25].Visible = false;
        e.Row.Cells[26].Visible = false;
        e.Row.Cells[27].Visible = false;
        e.Row.Cells[28].Visible = false;
        e.Row.Cells[29].Visible = false;
        e.Row.Cells[30].Visible = false;
        e.Row.Cells[31].Visible = false;
        e.Row.Cells[32].Visible = false;
        e.Row.Cells[33].Visible = false;
        e.Row.Cells[34].Visible = false;
        e.Row.Cells[35].Visible = false;
        e.Row.Cells[36].Visible = false;
        e.Row.Cells[37].Visible = false;
        e.Row.Cells[38].Visible = false;
        e.Row.Cells[39].Visible = false;
        e.Row.Cells[40].Visible = false;

        int days = DateTime.DaysInMonth(int.Parse(Session["SalaryProcessYear"].ToString()), int.Parse(Session["SalaryProcessMonth"].ToString()));

        if (days == 30)
        {
            e.Row.Cells[71].Visible = false;
        }
        else if (days == 29)
        {
            e.Row.Cells[70].Visible = false;
            e.Row.Cells[71].Visible = false;
        }
        else if (days == 28)
        {
            e.Row.Cells[60].Visible = false;
            e.Row.Cells[70].Visible = false;
            e.Row.Cells[71].Visible = false;
        }

        
    }
    private void loadheaders(int month, int year)
    {
        DateTime dt = new DateTime(year, month, 1);
        gv_attendance.HeaderRow.Cells[41].Text = "1 " + dt.AddDays(0).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[42].Text = "2 " + dt.AddDays(1).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[43].Text = "3 " + dt.AddDays(2).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[44].Text = "4 " + dt.AddDays(3).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[45].Text = "5 " + dt.AddDays(4).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[46].Text = "6 " + dt.AddDays(5).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[47].Text = "7 " + dt.AddDays(6).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[48].Text = "8 " + dt.AddDays(7).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[49].Text = "9 " + dt.AddDays(8).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[50].Text = "10 " + dt.AddDays(9).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[51].Text = "11 " + dt.AddDays(10).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[52].Text = "12 " + dt.AddDays(11).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[53].Text = "13 " + dt.AddDays(12).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[54].Text = "14 " + dt.AddDays(13).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[55].Text = "15 " + dt.AddDays(14).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[56].Text = "16 " + dt.AddDays(15).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[57].Text = "17 " + dt.AddDays(16).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[58].Text = "18 " + dt.AddDays(17).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[59].Text = "19 " + dt.AddDays(18).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[60].Text = "20 " + dt.AddDays(19).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[61].Text = "21 " + dt.AddDays(20).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[62].Text = "22 " + dt.AddDays(21).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[63].Text = "23 " + dt.AddDays(22).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[64].Text = "24 " + dt.AddDays(23).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[65].Text = "25 " + dt.AddDays(24).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[66].Text = "26 " + dt.AddDays(25).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[67].Text = "27 " + dt.AddDays(26).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[68].Text = "28 " + dt.AddDays(27).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[69].Text = "29 " + dt.AddDays(28).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[70].Text = "30 " + dt.AddDays(29).DayOfWeek.ToString().Substring(0, 3).ToUpper();
        gv_attendance.HeaderRow.Cells[71].Text = "31 " + dt.AddDays(30).DayOfWeek.ToString().Substring(0, 3).ToUpper();


    }

    protected void shiftcalendar_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow row = ((Control)sender).NamingContainer as GridViewRow;
        if (row != null)
        {
            DropDownList ddl1 = (DropDownList)row.FindControl("DropDownList1");
            DropDownList ddl2 = (DropDownList)row.FindControl("DropDownList2");
            DropDownList ddl3 = (DropDownList)row.FindControl("DropDownList3");
            DropDownList ddl4 = (DropDownList)row.FindControl("DropDownList4");
            DropDownList ddl5 = (DropDownList)row.FindControl("DropDownList5");
            DropDownList ddl6 = (DropDownList)row.FindControl("DropDownList6");
            DropDownList ddl7 = (DropDownList)row.FindControl("DropDownList7");
            DropDownList ddl8 = (DropDownList)row.FindControl("DropDownList8");
            DropDownList ddl9 = (DropDownList)row.FindControl("DropDownList9");
            DropDownList ddl10 = (DropDownList)row.FindControl("DropDownList10");
            DropDownList ddl11 = (DropDownList)row.FindControl("DropDownList11");
            DropDownList ddl12 = (DropDownList)row.FindControl("DropDownList12");
            DropDownList ddl13 = (DropDownList)row.FindControl("DropDownList13");
            DropDownList ddl14 = (DropDownList)row.FindControl("DropDownList14");
            DropDownList ddl15 = (DropDownList)row.FindControl("DropDownList15");
            DropDownList ddl16 = (DropDownList)row.FindControl("DropDownList16");
            DropDownList ddl17 = (DropDownList)row.FindControl("DropDownList17");
            DropDownList ddl18 = (DropDownList)row.FindControl("DropDownList18");
            DropDownList ddl19 = (DropDownList)row.FindControl("DropDownList19");
            DropDownList ddl20 = (DropDownList)row.FindControl("DropDownList20");
            DropDownList ddl21 = (DropDownList)row.FindControl("DropDownList21");
            DropDownList ddl22 = (DropDownList)row.FindControl("DropDownList22");
            DropDownList ddl23 = (DropDownList)row.FindControl("DropDownList23");
            DropDownList ddl24 = (DropDownList)row.FindControl("DropDownList24");
            DropDownList ddl25 = (DropDownList)row.FindControl("DropDownList25");
            DropDownList ddl26 = (DropDownList)row.FindControl("DropDownList26");
            DropDownList ddl27 = (DropDownList)row.FindControl("DropDownList27");
            DropDownList ddl28 = (DropDownList)row.FindControl("DropDownList28");
            DropDownList ddl29 = (DropDownList)row.FindControl("DropDownList29");
            DropDownList ddl30 = (DropDownList)row.FindControl("DropDownList30");
            DropDownList ddl31 = (DropDownList)row.FindControl("DropDownList31");
            TextBox txt_ot_hour = (TextBox)row.FindControl("txt_ot_hours");

            double pcount = 0;
            double acount = 0;
            double halfdaycount = 0;
            double leavescount = 0;
            double holidaycount = 0;
            // double cocount = 0;

            for (int j = 1; j <= (CountDay(int.Parse(Session["SalaryProcessMonth"].ToString()), int.Parse(Session["SalaryProcessYear"].ToString()), 2)); j++)
            {
                string cntrlname = "DropDownList" + j.ToString();
                DropDownList txt_day1 = (DropDownList)row.FindControl(cntrlname);
                if (txt_day1.SelectedValue == "P")
                {
                    pcount++;
                }
                else if (txt_day1.SelectedValue == "A")
                {
                    acount++;
                }
                else if (txt_day1.SelectedValue == "HD")
                {
                    halfdaycount++;
                }
                else if (txt_day1.SelectedValue == "L")
                {
                    leavescount++;
                }
                //else if (txt_day1.SelectedValue == "W")
                //{
                //    acount++;
                //}
                else if (txt_day1.SelectedValue == "H")
                {
                    holidaycount++;
                }
                else if (txt_day1.SelectedValue == "CL")
                {
                    leavescount++;
                }
                else if (txt_day1.SelectedValue == "PL")
                {
                    leavescount++;
                }
                else if (txt_day1.SelectedValue == "ML")
                {
                    leavescount++;
                }
                else if (txt_day1.SelectedValue == "PH")
                {
                    //leavescount++;
                    pcount++;
                }
                //else if (txt_day1.SelectedValue == "CO")
                //{
                //    cocount++;
                //}
            }
            halfdaycount = halfdaycount / 2;
            pcount = halfdaycount + pcount;

            d.operation("Update pay_attendance_muster set DAY01='" + ddl1.SelectedValue + "', DAY02='" + ddl2.SelectedValue + "', DAY03='" + ddl3.SelectedValue + "', DAY04='" + ddl4.SelectedValue + "', DAY05='" + ddl5.SelectedValue + "', DAY06='" + ddl6.SelectedValue + "', DAY07='" + ddl7.SelectedValue + "', DAY08='" + ddl8.SelectedValue + "', DAY09='" + ddl9.SelectedValue + "', DAY10='" + ddl10.SelectedValue + "', DAY11='" + ddl11.SelectedValue + "', DAY12='" + ddl12.SelectedValue + "', DAY13='" + ddl13.SelectedValue + "', DAY14='" + ddl14.SelectedValue + "', DAY15='" + ddl15.SelectedValue + "', DAY16='" + ddl16.SelectedValue + "', DAY17='" + ddl17.SelectedValue + "', DAY18='" + ddl18.SelectedValue + "', DAY19='" + ddl19.SelectedValue + "', DAY20='" + ddl20.SelectedValue + "', DAY21='" + ddl21.SelectedValue + "', DAY22='" + ddl22.SelectedValue + "', DAY23='" + ddl23.SelectedValue + "', DAY24='" + ddl24.SelectedValue + "', DAY25='" + ddl25.SelectedValue + "', DAY26='" + ddl26.SelectedValue + "', DAY27='" + ddl27.SelectedValue + "', DAY28='" + ddl28.SelectedValue + "', DAY29='" + ddl29.SelectedValue + "', DAY30='" + ddl30.SelectedValue + "', DAY31='" + ddl31.SelectedValue + "', TOT_DAYS_PRESENT =" + pcount + ", TOT_DAYS_ABSENT =" + acount + ", TOT_HALF_DAYS =" + halfdaycount + ",TOT_LEAVES =" + leavescount + ", HOLIDAYS =" + holidaycount + ", ot_hours=" + (txt_ot_hour.Text.Trim() == "" ? "0" : txt_ot_hour.Text) + " where comp_code = '" + Session["COMP_CODE"].ToString() + "' and month = '" + Session["SalaryProcessMonth"].ToString() + "' and year = '" + Session["SalaryProcessYear"].ToString() + "' and emp_code = '" + Convert.ToString(gv_attendance.DataKeys[row.RowIndex].Value) + "'");
        }
    }

    int CountDay(int month, int year, int counter)
    {
        int NoOfSunday = 0;
        var firstDay = new DateTime(year, month, 1);

        var day29 = firstDay.AddDays(28);
        var day30 = firstDay.AddDays(29);
        var day31 = firstDay.AddDays(30);

        if ((day29.Month == month && day29.DayOfWeek == DayOfWeek.Sunday)
        || (day30.Month == month && day30.DayOfWeek == DayOfWeek.Sunday)
        || (day31.Month == month && day31.DayOfWeek == DayOfWeek.Sunday))
        {
            NoOfSunday = 5;
        }
        else
        {
            NoOfSunday = 4;
        }

        int NumOfDay = DateTime.DaysInMonth(year, month);
        if (counter == 2)
        {
            return NumOfDay ;
        }
        if (counter == 1)
        {
            return NumOfDay - NoOfSunday;
        }
        else
        { return NoOfSunday; }
    }

}