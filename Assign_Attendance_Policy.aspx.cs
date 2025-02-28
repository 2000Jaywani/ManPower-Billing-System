using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;

public partial class Assign_Attandance_Ploicy : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();
    DAL d2 = new DAL();
    ArrayList arraylist1 = new ArrayList();
    ArrayList arraylist2 = new ArrayList();
    int counter = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["COMP_CODE"] == null || Session["COMP_CODE"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }		
        if (!IsPostBack)
        {
            lstLeft.Visible = true;
            policy_fill();
        }
    }
    protected void update_listbox(string listarray1)
    {
        listarray1 = listarray1.Replace(",", ""); lstLeft.ClearSelection();

        for (int i = 0; i <= listarray1.Length - 1; i++)
        {
            lstLeft.Items[int.Parse(listarray1.Substring(i, 1))].Selected = true;
        }
    }
    private int getindex(string item1)
    {
        for (int i = 0; i < lstLeft.Items.Count; i++)
        {
            if (lstLeft.Items[i].ToString() == item1)
            {
                return i;
            }
        }
        return 0;
    }

    protected void checkemp_click(object sender, EventArgs e)
    {
        if (checkemp.Checked == true && CheckDepartment.Checked == false && ChecUnit.Checked == false)
        {
            try
            {
                d.con.Open();
                //vikas 08-01-19
                MySqlCommand cmd_1 = new MySqlCommand("SELECT (SELECT CASE `Employee_type` WHEN 'Reliever' THEN CONCAT(`emp_name`, '-', 'Reliever') ELSE `emp_name` END) AS 'EMP_NAME', emp_code FROM pay_employee_master INNER JOIN pay_unit_master ON pay_employee_master.COMP_CODE =  pay_unit_master.COMP_CODE AND pay_employee_master.unit_code =  pay_unit_master.unit_code where pay_unit_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND branch_status = 0 order by EMP_NAME", d.con);
                MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
                DataSet ds1 = new DataSet();
                cad1.Fill(ds1);
                counter = 1;
                lstLeft.DataSource = ds1.Tables[0];
                lstLeft.DataValueField = "emp_code";
                lstLeft.DataTextField = "EMP_NAME";
                lstLeft.DataBind();

                checklistcox.Items.Clear();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
              // lstLeft.Items.Clear();
                d.con.Close();
                checklistcox.Items.Clear();
            }
        }
        else
        {
            lstLeft.Items.Clear();
            //checkemp.Checked = true;
           // checklistcox.Items.Clear(); 
            counter = 0;
           
        }

    }
    protected void ChecUnit_click(object sender, EventArgs e)
    {
        if (ChecUnit.Checked == true && checkemp.Checked == false && CheckDepartment.Checked == false)
        {
           

            try
            {
                d.con.Open();
                checklistcox.Visible = true;
                MySqlCommand cmd_empcode = new MySqlCommand("select UNIT_NAME from pay_unit_master where comp_code = '" + Session["COMP_CODE"].ToString() + "' AND branch_status = 0 ", d.con);
                MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_empcode);
                DataSet ds1 = new DataSet();
                cad1.Fill(ds1);
                checklistcox.Visible = true;
                checklistcox.DataSource = ds1.Tables[0];
                checklistcox.DataValueField = "UNIT_NAME";
                checklistcox.DataTextField = "UNIT_NAME";
                checklistcox.DataBind();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                d.con.Close();
                lstLeft.Items.Clear();
            }
        }
        else
        {
          //  if (counter == 0)
          //  {
            //ChecUnit.Checked = false;
                checklistcox.Items.Clear();
                lstLeft.Items.Clear();
          //  }
        }
    }

    protected void checklistcox_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        string name = "";
        if (CheckDepartment.Checked == true)
        {
            if (checklistcox.SelectedItem != null)
            {

              
                try
                {
                    foreach (ListItem lst in checklistcox.Items)
                    {
                        if (lst.Selected == true)
                        {
                           // string abc = lst.Value.ToString();
                            if (name == "")
                            {
                                name = (lst.Value.ToString()) + ",";
                            }
                            else
                            {
                                name = name + (lst.Value.ToString()) + ",";
                            }
                        }
                    }

                    if (name.EndsWith(","))
                    {
                        name = name.Remove(name.Length - 1, 1);
                        name = name.Replace(",", "','");
                    }

                    d.con.Open();
                    MySqlCommand cmd_1 = new MySqlCommand("select (SELECT CASE `Employee_type` WHEN 'Reliever' THEN CONCAT(`emp_name`, '-', 'Reliever') ELSE `emp_name` END) AS 'EMP_NAME', emp_code from pay_employee_master,pay_client_master where pay_employee_master.comp_code ='" + Session["COMP_CODE"] + "' and pay_client_master.CLIENT_CODE in ('" + name + "') and  pay_employee_master.CLIENT_CODE= pay_client_master.CLIENT_CODE order by emp_name ", d.con);
                    MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
                    DataSet ds1 = new DataSet();
                    cad1.Fill(ds1);
                    lstLeft.DataSource = ds1.Tables[0];
                    lstLeft.DataValueField = "emp_code";
                    lstLeft.DataTextField = "emp_name";
                    lstLeft.DataBind();
                    lstLeft.Visible = true;
                    //d.con.Close();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); 
                }
                catch (Exception ex)
                { throw ex; }
                finally
                {
                   d.con.Close();
                }
            }
            else
            {
               lstLeft.Items.Clear();
            }
        }
        else if (ChecUnit.Checked == true)
        {
            if (checklistcox.SelectedItem != null)
            {
                try
                {
                    d.con.Open();
                    //vikas 08-01-19
                    MySqlCommand cmd_1 = new MySqlCommand("select (SELECT CASE `Employee_type` WHEN 'Reliever' THEN CONCAT(`emp_name`, '-', 'Reliever') ELSE `emp_name` END) AS 'EMP_NAME', emp_code from pay_employee_master,pay_unit_master where pay_employee_master.comp_code ='" + Session["COMP_CODE"] + "' and UNIT_NAME='" + checklistcox.SelectedItem.Text + "' and  pay_employee_master.UNIT_CODE= pay_unit_master.UNIT_CODE order by emp_name ", d.con);
                    MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
                    DataSet ds1 = new DataSet();
                    cad1.Fill(ds1);
                    lstLeft.DataSource = ds1.Tables[0];
                    lstLeft.DataValueField = "emp_code";
                    lstLeft.DataTextField = "emp_name";
                    lstLeft.DataBind();
                    lstLeft.Visible = true;
                }
                catch (Exception ex)
                { throw ex; }
                finally
                {
                    d.con.Close();
                }
            }
           // else { lstLeft.Items.Clear(); }
        }
    }


    protected void CheckDepartment_click(object sender, EventArgs e)
    {
        if (CheckDepartment.Checked == true && ChecUnit.Checked == false && checkemp.Checked == false)
        {
            try
            {
                string com_code = Session["COMP_CODE"].ToString();
                checklistcox.Visible = true;
                d.con.Open();
                MySqlCommand cmd1 = new MySqlCommand("SELECT CLIENT_NAME,CLIENT_CODE FROM pay_client_master WHERE COMP_CODE='" + Session["COMP_CODE"].ToString() + "'  ", d.con);
                DataSet ds1 = new DataSet();
                MySqlDataAdapter adp1 = new MySqlDataAdapter(cmd1);
                adp1.Fill(ds1);
                checklistcox.Visible = true;
                checklistcox.DataSource = ds1.Tables[0];
                checklistcox.DataValueField = "CLIENT_CODE";
                checklistcox.DataTextField = "CLIENT_NAME";
                checklistcox.DataBind();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                d.con.Close();
                lstLeft.Items.Clear();
            }
        }
        else
        {
            //if (counter == 0)
            //{
                //CheckDepartment.Checked = false;
                checklistcox.Items.Clear();
                lstLeft.Items.Clear();
           // }
        }
    }

    protected void allriht_click(object sender, EventArgs e)
    {
        while (lstRight.Items.Count != 0)
        {
            for (int i = 0; i < lstRight.Items.Count; i++)
            {
                lstLeft.Items.Add(lstRight.Items[i]);
                lstRight.Items.Remove(lstRight.Items[i]);
            }
        }
    }
    protected void brnallleft_Click(object sender, EventArgs e)
    {

        while (lstLeft.Items.Count != 0)
        {
            for (int i = 0; i < lstLeft.Items.Count; i++)
            {
                lstRight.Items.Add(lstLeft.Items[i]);
                lstLeft.Items.Remove(lstLeft.Items[i]);

                //lstRight.Items.Add(lstLeft.Items[i]);
                //lstRight.Items.Remove(lstLeft.Items[i]);
            }
        }


    }

    protected void btnleft_click(object sender, EventArgs e)
    {
        if (lstRight.SelectedIndex >= 0)
        {
            for (int i = 0; i < lstRight.Items.Count; i++)
            {
                if (lstRight.Items[i].Selected)
                {
                    if (!arraylist1.Contains(lstRight.Items[i]))
                    {
                        arraylist1.Add(lstRight.Items[i]);
                    }
                }
            }
            for (int i = 0; i < arraylist1.Count; i++)
            {
                if (!lstLeft.Items.Contains(((ListItem)arraylist1[i])))
                {
                    lstLeft.Items.Add(((ListItem)arraylist1[i]));
                }
                 lstRight.Items.Remove(((ListItem)arraylist1[i]));
            }
            lstLeft.SelectedIndex = -1;
        }

    }

    protected void btnright_click(object sender, EventArgs e)
    {
        if (lstLeft.SelectedIndex > 0 || lstLeft.SelectedIndex == 0)
        {
            for (int i = 0; i < lstLeft.Items.Count; i++)
            {
                if (lstLeft.Items[i].Selected)
                {
                    if (!arraylist2.Contains(lstLeft.Items[i]))
                    {
                        arraylist2.Add(lstLeft.Items[i]);
                    }
                }
            }
            for (int i = 0; i < arraylist2.Count; i++)
            {
                if (!lstRight.Items.Contains(((ListItem)arraylist2[i])))
                {
                    lstRight.Items.Add(((ListItem)arraylist2[i]));
                }
                lstLeft.Items.Remove(((ListItem)arraylist2[i]));
            }
            lstRight.SelectedIndex = -1;
        }
    }
    protected void btnclose_click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }

    private void update_notification()
    {
        d1.con1.Open();

        try
        {
            if (Application["policy_id"] == null || Application["policy_id"].ToString() == "")
            {
                Application["policy_id"] = Session["system_curr_date"].ToString();
               // MySqlCommand cmd1 = new MySqlCommand("SELECT id,txt_policy_name FROM pay_travel_policy_master  WHERE COMP_CODE=@COMP_CODE and UNIT_CODE=UNIT_CODE", d1.con1);
                MySqlCommand cmd1 = new MySqlCommand("select concat('Its ',EMP_CODE,'s birthday today - ', DATE_FORMAT(str_to_date('" + Application["policy_id"].ToString() + "','%d/%m'),'%d %b'))as noti from PAY_EMPLOYEE_MASTER WHERE date_format(BIRTH_DATE,'%d/%m') = substring('" + Application["policy_id"].ToString() + "',1,5)", d1.con1);
                MySqlDataReader dr1 = cmd1.ExecuteReader();
                if (dr1.Read())
                {
                    d1.operation("insert into pay_notification_master (notification,not_read,EMP_CODE,page_name) select  'Travel Polices for " + Session["USERNAME"].ToString() + " -Policy- " + ddlpolicies.SelectedItem.Text.ToString() + "','0',EMP_CODE,'Attandance_policy_master.aspx' from pay_assign_attandance_policy");
                }

            }
            else
            {
                //DateTime policy_id = DateTime.ParseExact(Application["policy_id"].ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime systemdate = DateTime.ParseExact(Session["system_curr_date"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //if (policy_id <= systemdate)
                //{
                //    for (int i = 1; policy_id != systemdate; i++)
                //    {
                        MySqlCommand cmd1 = new MySqlCommand("select concat('Its ',EMP_NAME,'s birthday today - ', DATE_FORMAT(str_to_date('" + Application["policy_id"].ToString() + "','%d/%m'),'%d %b'))as noti from PAY_EMPLOYEE_MASTER WHERE date_format(BIRTH_DATE,'%d/%m') = substring('" + Application["policy_id"].ToString() + "',1,5)", d1.con1);
                        MySqlDataReader dr1 = cmd1.ExecuteReader();
                        if (dr1.Read())
                        {
                            d1.operation("insert into pay_notification_master (notification,not_read,EMP_CODE,page_name) select  'Travel Polices for " + Session["USERNAME"].ToString() + " -Policy- " + ddlpolicies.SelectedItem.Text.ToString() + "','0',EMP_CODE,'Attandance_policy_master.aspx' from pay_assign_attandance_policy");
                        }
                        dr1.Close();
                        //policy_id = policy_id.AddDays(1);
                        //Application["policy_id"] = policy_id.ToString("dd/MM/yyyy");

                //    }
                //}

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            d1.con1.Close();
        }
        //insert into pay_notification_master (notification,not_read,EMP_CODE,page_name) select (select concat('Its ',EMP_NAME,' birthday today - ', DATE_FORMAT(NOW(),'%d %b %y')) from PAY_EMPLOYEE_MASTER WHERE str_to_date(BIRTH_DATE,'%d/%m/%Y') = date(now())),'0',EMP_CODE,'birthday.aspx' from PAY_EMPLOYEE_MASTER

    }

    protected void btnSubmit_click(object sender, EventArgs e)
    {
        int res = 0;
        int li_select = 0;
        try
        {
            update_notification();
         
           d.operation("insert into pay_assign_attandance_policy (comp_code,unit_code,emp_code,policy_id) select comp_code,unit_code,emp_code,0 from pay_employee_master where comp_code = '" + Session["COMP_CODE"].ToString() + "'  and emp_code not in ( select distinct emp_code from pay_assign_attandance_policy where comp_code = '" + Session["COMP_CODE"].ToString() + "' )");
           
           // res = d.operation("update pay_assign_attandance_policy set policy_id = 0 where comp_code = '" + Session["COMP_CODE"].ToString() + "'  and policy_id ='" + ddlpolicies.SelectedValue.ToString()+"'");
            foreach (ListItem li in lstRight.Items)
            {
                li_select++;
               // res = d.operation("update pay_assign_attandance_policy set policy_id = " + ddlpolicies.SelectedValue.ToString() + " where comp_code = '" + Session["COMP_CODE"].ToString() + "' and unit_code = '" + Session["UNIT_CODE"] + "' and emp_code = '" + li.Value + "'");
                res = d.operation("update pay_assign_attandance_policy set policy_id = '" + ddlpolicies.SelectedValue.ToString() + "' where comp_code = '" + Session["COMP_CODE"].ToString() + "' and emp_code = '" + li.Value + "' ");
            }
            if (res > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Employees added successfully to Policy.');", true);
                lstRight.ClearSelection();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Employees adding failed to Policy.');", true);
            }
        }
        catch (Exception ee)
        {
            throw ee;
        }
        finally
        {
           // ddlpolicies.SelectedValue = "0";
            lstRight.Items.Clear();
        }
    }
    protected void ddlpolicies_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!ddlpolicies.SelectedValue.Equals("0"))
        {
            try
            {
                d.con.Open();
                lstRight.Items.Clear();
             //   MySqlCommand cmd_1 = new MySqlCommand("select emp_name, emp_code from pay_employee_master where emp_code in (select emp_code from pay_assign_attandance_policy where Id = " + ddlpolicies.SelectedValue + ") and comp_code ='" + Session["COMP_CODE"] + "' and unit_code ='" + Session["UNIT_CODE"] + "' order by EMP_NAME", d.con);
              //vikas 08-01-19
                MySqlCommand cmd_1 = new MySqlCommand("select (SELECT CASE `Employee_type` WHEN 'Reliever' THEN CONCAT(`emp_name`, '-', 'Reliever') ELSE `emp_name` END) AS 'EMP_NAME', emp_code from pay_employee_master where emp_code in (select emp_code from pay_assign_attandance_policy where POLICY_ID = '" + ddlpolicies.SelectedValue + "') and comp_code ='" + Session["COMP_CODE"] + "' order by EMP_NAME", d.con);
                MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
                DataSet ds1 = new DataSet();
                cad1.Fill(ds1);
                lstRight.DataSource = ds1.Tables[0];
                lstRight.DataValueField = "emp_code";
                lstRight.DataTextField = "emp_name";
                lstRight.DataBind();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            {
                d.con.Close();
               
            }
        }
        else
        {
            lstRight.Items.Clear();
        }
    }

    protected void policy_fill()
    {
        ddlpolicies.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT Policy_Id,POLICY_NAME FROM attandance_police_master where COMP_CODE='" + Session["COMP_CODE"] + "'", d.con);
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddlpolicies.DataSource = dt_item;
                ddlpolicies.DataValueField = dt_item.Columns[0].ToString();
                ddlpolicies.DataTextField = dt_item.Columns[1].ToString();
                ddlpolicies.DataBind();
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            lstRight.Items.Clear();
            d.conclose();
        }


        ddlpolicies.Items.Insert(0, new ListItem("Select Policy", "Select Policy"));



    }
    protected void btnsalarycalculation_Click(object sender, EventArgs e)
    {
    //    int li_select = 0;
    //    foreach (ListItem li in lstRight.Items)
    //    {
    //        li_select++;
        try {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
        }
        catch { }

            btnsalarycalculation_Click();


       // }
    
    }

    protected void btnsalarycalculation_Click()
    {
        string employee_id = "";
        string policy_id = "";
        string policy_name = "";
        string working_hours = "";
        string roll = "0";
        string transaction = "0";
        string week_off = "";
        string Early_in = "";
        string Early_out = "";
        string Let_In = "";
        String Let_Out = "";
        string deduction_policy = "";
        string reminder = "";
        string short_hours = "";
        string timing = "";
        string limits_of_hours = "";
        string punch_realization = "";
        string period = "";
        string od = "";
        string minimun_ot_hours = "";
        string max_ot_hours = "";
        string Approval_req = "";
        string comman_off = "";
        string ot_rate = "";
        string general_ramark = "";
        string auto_shift = "";
        string intime = "";
        string outtime = "";
        string Attendances_intime = "";
        string Attendances_outtime = "";
        string Camera_intime = "";
        string Camera_outtime = "";
        string Attendances_intime_full = "";
        string Attendances_outtime_full = "";
        string Camera_intime_full = "";
        string Camera_outtime_full = "";
        // String currentdate1 = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

        d.operation("insert into pay_attendance_muster (comp_code,unit_code,emp_code ) select comp_code,unit_code,emp_code  from pay_employee_master where comp_code = '" + Session["COMP_CODE"].ToString() + "'  and emp_code not in ( select emp_code from pay_attendance_muster where comp_code = '" + Session["COMP_CODE"].ToString() + "' )");


        d2.con1.Open();
        MySqlCommand cmd21 = new MySqlCommand("select emp_code from pay_assign_attandance_policy where policy_Id= '" + ddlpolicies.SelectedValue + "'",d2.con1);
   
        MySqlDataReader dr21 = cmd21.ExecuteReader();
        while (dr21.Read())
        {
            string emp_code1 = dr21.GetValue(0).ToString();
                                                     MySqlCommand cmd1 = new MySqlCommand(" select EMP_CODE, Attendances_intime , Camera_intime  from pay_android_attendance_logs where Comp_Code='" + Session["COMP_CODE"].ToString() + "' and emp_code ='" + emp_code1 + "'", d.con1);
                                            d.con1.Open();
                                            MySqlDataReader dr1 = cmd1.ExecuteReader();
                                            while (dr1.Read())
                                            {
                                                string emp_code = dr1.GetValue(0).ToString();
                                                string attaance_in = dr1.GetValue(1).ToString();
                                                string camera_in = dr1.GetValue(2).ToString();

                                                MySqlCommand cmd = new MySqlCommand(" select pay_assign_attandance_policy.EMP_CODE, attandance_police_master.Policy_Id,POLICY_NAME,WORKING_HOURS,Roll ,Transation,Weak_Off ,Rarly_In ,EarlyOut ,Let_In ,Let_Out,Deduction_Policy,Reminders ,Short_Hours,Timing ,Limits_Of_Hours ,Punch_Regularization ,Period ,OD ,Minimum_Ot_urs,Max_Ot_Hours,Approval_Required,Comp_Off ,OT_Rate ,General_Remark,AUTO_SHIFT,SUBSTRING(Attendances_intime,12,5),SUBSTRING(Attendances_outtime,12,5),SUBSTRING(Camera_intime,12,5),SUBSTRING(Camera_outtime,12,5),Attendances_intime,Attendances_outtime,Camera_intime,Camera_outtime  FROM attandance_police_master inner join pay_android_attendance_logs on attandance_police_master.COMP_CODE= pay_android_attendance_logs.COMP_CODE inner join pay_assign_attandance_policy on  attandance_police_master.COMP_CODE=pay_assign_attandance_policy.COMP_CODE  AND pay_android_attendance_logs.EMP_CODE=pay_assign_attandance_policy.EMP_CODE  where pay_android_attendance_logs.COMP_CODE='" + Session["COMP_CODE"].ToString() + "' and attandance_police_master.Policy_Id='" + ddlpolicies.SelectedValue + "' AND pay_assign_attandance_policy.EMP_CODE ='" + emp_code + "' ", d.con);
                                                d.con.Open();
                                                MySqlDataReader dr = cmd.ExecuteReader();
                                                if (dr.Read())
                                                {
                                                    employee_id = dr.GetValue(0).ToString();
                                                    policy_id = dr.GetValue(1).ToString();
                                                    policy_name = dr.GetValue(2).ToString();
                                                    working_hours = dr.GetValue(3).ToString();
                                                    roll = dr.GetValue(4).ToString();
                                                    transaction = dr.GetValue(5).ToString();
                                                    week_off = dr.GetValue(6).ToString();
                                                    Early_in = dr.GetValue(7).ToString();
                                                    Early_out = dr.GetValue(8).ToString();
                                                    Let_In = dr.GetValue(9).ToString();
                                                    Let_Out = dr.GetValue(10).ToString();
                                                    deduction_policy = dr.GetValue(11).ToString();
                                                    reminder = dr.GetValue(12).ToString();
                                                    short_hours = dr.GetValue(13).ToString();
                                                    timing = dr.GetValue(14).ToString();
                                                    limits_of_hours = dr.GetValue(15).ToString();
                                                    punch_realization = dr.GetValue(16).ToString();
                                                    period = dr.GetValue(17).ToString();
                                                    od = dr.GetValue(18).ToString();
                                                    minimun_ot_hours = dr.GetValue(19).ToString();
                                                    max_ot_hours = dr.GetValue(20).ToString();
                                                    Approval_req = dr.GetValue(21).ToString();
                                                    comman_off = dr.GetValue(22).ToString();
                                                    ot_rate = dr.GetValue(23).ToString();
                                                    general_ramark = dr.GetValue(24).ToString();
                                                    auto_shift = dr.GetValue(25).ToString();
                                                    Attendances_intime = dr.GetValue(26).ToString();
                                                    Attendances_outtime = dr.GetValue(27).ToString();
                                                    Camera_intime = dr.GetValue(28).ToString();
                                                    Camera_outtime = dr.GetValue(29).ToString();
                                                    Attendances_intime_full = dr.GetValue(30).ToString();
                                                    Attendances_outtime_full = dr.GetValue(31).ToString();
                                                    Camera_intime_full = dr.GetValue(32).ToString();
                                                    Camera_outtime_full = dr.GetValue(33).ToString();

                                                    d2.con.Open();
                                                    MySqlCommand cmd2 = new MySqlCommand(" select IN_TIME,OUT_TIME  from attandance_inout_time_details where Policy_Id='" + ddlpolicies.SelectedValue + "' and Comp_Code='" + Session["COMP_CODE"].ToString() + "'", d2.con);
                                                   
                                                    MySqlDataReader dr2 = cmd2.ExecuteReader();

                                                    while (dr2.Read())
                                                    {
                                                        string intime1 = dr2.GetValue(0).ToString();
                                                        string outtime1 = dr2.GetValue(1).ToString();



                                                                                                                                                                if (Attendances_intime != "" && Attendances_outtime != "")
                                                                                                                                                                {
                                                                                                                                                                    string ii = intime1 + (".60");
                                                                                                                                                                    double dd = Convert.ToDouble(ii);
                                                                                                                                                                    string s1 = Attendances_intime.Replace(':', '.');
                                                                                                                                                                    string s2 = Attendances_outtime.Replace(':', '.');
                                                                                                                                                                    double dd2 = Convert.ToDouble(s1);
                                                                                                                                                                    double dd3 = Convert.ToDouble(s2);

                                                                                                                                                                    double diff_dd_dd2 = dd2 - (dd - 1);//for late in

                                                                                                                                                                    double diff_dd_dd3 = (dd - 1) - dd2;//for early in
                                                                                                                                                                    double diff_9hr = dd3 - dd2;

                                                                                                                                                                    string month = Attendances_intime_full.ToString().Substring(3, 2);
                                                                                                                                                                    string year = Attendances_intime_full.ToString().Substring(6, 4);
                                                                                                                                                                    string date = Attendances_intime_full.ToString().Substring(0, 2);

                                                                                                                                                                    if (diff_9hr >= Convert.ToDouble(working_hours))//for present
                                                                                                                                                                    {
                                                                                                                                                                        if (diff_dd_dd2 > Convert.ToDouble(Let_In) && diff_9hr >= Convert.ToDouble(working_hours))
                                                                                                                                                                        {
                                                                                                                                                                            int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='P'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                        }

                                                                                                                                                                        else


                                                                                                                                                                            if (diff_dd_dd3 > Convert.ToDouble(Early_in) && diff_9hr >= Convert.ToDouble(working_hours))
                                                                                                                                                                            {
                                                                                                                                                                                int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='P'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                            }

                                                                                                                                                                    }
                                                                                                                                                                    else//for half day
                                                                                                                                                                        if (diff_9hr >= Convert.ToDouble(short_hours))
                                                                                                                                                                        {
                                                                                                                                                                            if (diff_dd_dd2 > Convert.ToDouble(Let_In) && diff_9hr >= Convert.ToDouble(short_hours))
                                                                                                                                                                            {
                                                                                                                                                                                int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='H'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                                if (diff_dd_dd3 > Convert.ToDouble(Early_in) && diff_9hr >= Convert.ToDouble(short_hours))
                                                                                                                                                                                {
                                                                                                                                                                                    int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='H'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                                }

                                                                                                                                                                        }
                                                                                                                                                                        else//for absent
                                                                                                                                                                            if (diff_9hr < Convert.ToDouble(short_hours))
                                                                                                                                                                            {

                                                                                                                                                                                if (diff_dd_dd2 > Convert.ToDouble(Let_In) && diff_9hr < Convert.ToDouble(short_hours))
                                                                                                                                                                                {
                                                                                                                                                                                    int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='A'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                    if (diff_dd_dd3 > Convert.ToDouble(Let_In) && diff_9hr < Convert.ToDouble(working_hours))
                                                                                                                                                                                    {
                                                                                                                                                                                        int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='A'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                                    }
                                                                                                                                                                            }

                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                    if (Camera_intime != "" && Camera_outtime != "")
                                                                                                                                                                    {

                                                                                                                                                                        string ii = intime1 + (".60");
                                                                                                                                                                        double dd = Convert.ToDouble(ii);
                                                                                                                                                                        string s1 = Camera_intime.Replace(':', '.');
                                                                                                                                                                        string s2 = Camera_outtime.Replace(':', '.');
                                                                                                                                                                        double dd2 = Convert.ToDouble(s1);
                                                                                                                                                                        double dd3 = Convert.ToDouble(s2);

                                                                                                                                                                        double diff_dd_dd2 = dd2 - (dd - 1);//for late in

                                                                                                                                                                        double diff_dd_dd3 = (dd - 1) - dd2;//for early in
                                                                                                                                                                        double diff_9hr = dd3 - dd2;

                                                                                                                                                                        string month = Camera_intime_full.ToString().Substring(3, 2);
                                                                                                                                                                        string year = Camera_intime_full.ToString().Substring(6, 4);
                                                                                                                                                                        string date = Camera_intime_full.ToString().Substring(0, 2);


                                                                                                                                                                        if (diff_9hr >= Convert.ToDouble(working_hours))//for present
                                                                                                                                                                        {
                                                                                                                                                                            if (diff_dd_dd2 > Convert.ToDouble(Let_In) && diff_9hr >= Convert.ToDouble(working_hours))
                                                                                                                                                                            {
                                                                                                                                                                                int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='P'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                            }

                                                                                                                                                                            else


                                                                                                                                                                                if (diff_dd_dd3 > Convert.ToDouble(Early_in) && diff_9hr >= Convert.ToDouble(working_hours))
                                                                                                                                                                                {
                                                                                                                                                                                    int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='P'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                                }

                                                                                                                                                                        }
                                                                                                                                                                        else//for half day
                                                                                                                                                                            if (diff_9hr >= Convert.ToDouble(short_hours))
                                                                                                                                                                            {
                                                                                                                                                                                if (diff_dd_dd2 > Convert.ToDouble(Let_In) && diff_9hr >= Convert.ToDouble(short_hours))
                                                                                                                                                                                {
                                                                                                                                                                                    int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='H'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                    if (diff_dd_dd3 > Convert.ToDouble(Early_in) && diff_9hr >= Convert.ToDouble(short_hours))
                                                                                                                                                                                    {
                                                                                                                                                                                        int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='H'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                                    }

                                                                                                                                                                            }
                                                                                                                                                                            else//for absent
                                                                                                                                                                                if (diff_9hr < Convert.ToDouble(short_hours))
                                                                                                                                                                                {

                                                                                                                                                                                    if (diff_dd_dd2 > Convert.ToDouble(Let_In) && diff_9hr < Convert.ToDouble(short_hours))
                                                                                                                                                                                    {
                                                                                                                                                                                        int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='A'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                        if (diff_dd_dd3 > Convert.ToDouble(Let_In) && diff_9hr < Convert.ToDouble(working_hours))
                                                                                                                                                                                        {
                                                                                                                                                                                            int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='A'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");
                                                                                                                                                                                        }
                                                                                                                                                                                }
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                        if (Camera_intime == "" && Attendances_intime_full == "")
                                                                                                                                                                        {
                                                                                                                                                                            string month = Camera_intime_full.ToString().Substring(3, 2);
                                                                                                                                                                            string year = Camera_intime_full.ToString().Substring(6, 4);
                                                                                                                                                                            string date = Camera_intime_full.ToString().Substring(0, 2);

                                                                                                                                                                            int res = d.operation("update pay_attendance_muster set DAY" + date.ToString() + "='A'  where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND EMP_CODE='" + employee_id + "' AND MONTH='" + month + "' AND YEAR ='" + year + "'");

                                                                                                                                                                        }








                                                    }
                                                    dr2.Close();
                                                    d2.con.Close();


                                                }

                                                dr.Close();
                                                d.con.Close();
                                            }


                                            dr1.Close();
                                            d.con1.Close();


                                        }
        dr21.Close();
        d2.con1.Close();

    }
}