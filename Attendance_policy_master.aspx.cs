using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

public partial class Attandance_policy_master : System.Web.UI.Page
{
    DAL d1 = new DAL();
    DAL d = new DAL();
    protected void Page_Load(object sender, EventArgs e)
    {
     
        if (!IsPostBack)
        {
            policy_fill();
            btn_submit.Visible = false;
            btndelete.Visible = false;

            Policy_code();
        }
    }

    protected void policy_fill()
    {
        ddl_Existing_policy_name.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT Policy_Id,POLICY_NAME FROM attandance_police_master where COMP_CODE='" + Session["COMP_CODE"].ToString() + "'", d.con);
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_Existing_policy_name.DataSource = dt_item;
                ddl_Existing_policy_name.DataValueField = dt_item.Columns[0].ToString();
                ddl_Existing_policy_name.DataTextField = dt_item.Columns[1].ToString();
                ddl_Existing_policy_name.DataBind();
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.conclose();
        }


        ddl_Existing_policy_name.Items.Insert(0, new ListItem("Select Policy", "Select Policy"));

        Policy_code();

    }
    protected void ddl_Existing_policy_name_SelectedIndexChanged(object sender, EventArgs e)
    {

        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        btnadd.Visible = true;
        btn_submit.Visible = true;
        if (ddl_Existing_policy_name.SelectedValue.Equals("Select Policy"))
        {
             text_clear();
            btn_submit.Visible = false;
            btndelete.Visible = false;
        }
        else
        {
            txt_policy_name1.Text = ddl_Existing_policy_name.SelectedItem.Text;

            d.con1.Open();
            try
            {
                //  MySqlCommand cmd = new MySqlCommand("select comp_code, '', txt_policy_name,date_format(txt_start_date,'%d/%m/%Y') as txt_start_date ,date_format(txt_end_date,'%d/%m/%Y') as txt_end_date,chkair,chkbus,chktraint1,chktraint2,chktraint3,chkcabtaxi,chkauto,chkownedvehicle,txtownedvehiclekms,chklocalconveyance,txt_localconveyancelimit,txt_approval_level,txt_approval_days_before,chk_auto_approval,txt_app_days_before_travel,chk_escalation_approval,txt_app_escalation_approcal,txt_app_escalation_level,txt_not_approved_emailid,chk_app_cancel_if_approved,chk_app_cancel_if_ticket_confirmed,txt_cancellation_days,txt_exception_case_approval_level,chk_hotel,chk_laundry,chk_expenses_allowed,txt_per_day_limit,chk_femail_upgrade,txt_female_percent,txt_claim_max_days,txt_late_claim_days,txt_claim_exception_case,chk_payment_air,chk_payment_bus,chk_payment_train,chk_payment_taxi,chk_payment_hotel,modified_time,now(),submit FROM pay_travel_policy_master where id=" + ddl_Existing_policy_name.SelectedValue, d.con1);
                MySqlCommand cmd = new MySqlCommand("select Policy_Id,POLICY_NAME,date_format(START_DATE,'%d/%m/%Y') as START_DATE  ,date_format(END_DATE,'%d/%m/%Y') as END_DATE,WORKING_HOURS,Roll,Transation,Weak_Off ,Rarly_In ,EarlyOut ,Let_In ,Let_Out,Deduction_Policy,Reminders ,Short_Hours,Timing ,Limits_Of_Hours ,Punch_Regularization ,Period ,OD ,Minimum_Ot_urs,Max_Ot_Hours,Approval_Required,Comp_Off ,OT_Rate ,General_Remark,AUTO_SHIFT,Gender FROM attandance_police_master where POLICY_NAME='" + ddl_Existing_policy_name.SelectedItem.Text + "' and comp_code='"+Session["comp_code"].ToString()+"'", d.con1);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txt_id.Text = dr.GetValue(0).ToString();
                    txt_policy_name1.Text = dr.GetValue(1).ToString();
                    txt_start_date.Text = dr.GetValue(2).ToString();
                    txt_end_date.Text = dr.GetValue(3).ToString();
                    txt_working_hours.Text= dr.GetValue(4).ToString();
                   ddl_roll.SelectedValue=dr.GetValue(5).ToString();
                     ddl_transaction.SelectedValue=dr.GetValue(6).ToString();
                     ddl_Weekoff.SelectedValue=dr.GetValue(7).ToString();
                     txt_Early_In.Text=dr.GetValue(8).ToString();
                     txt_Earlyout.Text=dr.GetValue(9).ToString();
                     txt_letin.Text =dr.GetValue(10).ToString();
                     txt_letout.Text=dr.GetValue(11).ToString();
                    txt_txtdeductionpolicy.Text=dr.GetValue(12).ToString();
                     ddl_reminder.SelectedValue=dr.GetValue(13).ToString();
                     txt_shorthours.Text=dr.GetValue(14).ToString();
                     txt_timing.Text=dr.GetValue(15).ToString();
                     txt_limitofhours.Text=dr.GetValue(16).ToString();
                     txt_punchrealization.Text =dr.GetValue(17).ToString();
                     txt_period.Text=dr.GetValue(18).ToString();
                     ddl_od.SelectedValue=dr.GetValue(19).ToString();
                     txt_min_othrs.Text =dr.GetValue(20).ToString();
                     txt_max_othours.Text=dr.GetValue(21).ToString();
                     ddl_approval.SelectedValue=dr.GetValue(22).ToString();
                    txt_commonoff.Text =dr.GetValue(23).ToString();
                     txt_otrate.Text =dr.GetValue(24).ToString();
                     txt_generalremark.Text =dr.GetValue(25).ToString();
                     ddl_gender.SelectedValue = dr.GetValue(27).ToString();
                    
                   

                    if (dr.GetValue(26).ToString() == "1")
                    {
                        chk_Autoshift.Checked = true;

                    }
                    else
                    {
                        chk_Autoshift.Checked = false;

                    }
                   
                }
            }






            catch (Exception ex) { throw ex; }
            finally { d.con1.Close(); }


            panel2.Visible = true;
            btndelete.Visible = true;
            try
            {
                d.con.Open();
                MySqlCommand cmd2 = new MySqlCommand("Select SUBMIT from attandance_police_master where COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND Policy_Id='" + ddl_Existing_policy_name.SelectedValue + "'", d.con);
                MySqlDataReader dr1 = cmd2.ExecuteReader();
                if (dr1.Read())
                {
                    if (dr1[0].Equals(0))
                    {
                        btnadd.Visible = true;
                        btn_submit.Visible = true;
                        btndelete.Visible = true;
                    }
                    else
                    {
                        btn_submit.Visible = true;
                        btnadd.Visible = true;
                        btndelete.Visible = true;
                    }
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                d.con.Close();
                btnadd.Text = "Update";
            }


            d1.con.Open();
            MySqlCommand cmd1 = new MySqlCommand("SELECT IN_TIME,OUT_TIME from attandance_inout_time_details WHERE Policy_Id ='" + txt_id.Text + "' AND COMP_CODE='" + Session["COMP_CODE"].ToString() + "'  ", d1.con);
                 MySqlDataReader dr11 = cmd1.ExecuteReader();
                 if (dr11.Read())
                 {
                     txt_intime.Text = dr11.GetValue(0).ToString();
                     txt_outtime.Text = dr11.GetValue(1).ToString();
                 }
                 d1.con.Close();
        }

    }
    protected void btnadd_Click(object sender, EventArgs e)
    {
        String flag1;
        if (chk_Autoshift.Checked == true)
        {
            flag1 = "1";
        }
        else
        {
            flag1 = "0";
        }
        int resins1 = 0;
        if (ddl_Existing_policy_name.SelectedIndex.ToString() == "0")
        {
            int i = d1.operation("insert into attandance_police_master (Policy_Id,COMP_CODE,UNIT_CODE ,POLICY_NAME,START_DATE ,END_DATE ,CREATED_DATE ,MODIFY_BY ,WORKING_HOURS,Roll ,Transation,Weak_Off ,Rarly_In ,EarlyOut ,Let_In ,Let_Out,Deduction_Policy,Reminders ,Short_Hours,Timing ,Limits_Of_Hours ,Punch_Regularization ,Period ,OD ,Minimum_Ot_urs,Max_Ot_Hours,Approval_Required,Comp_Off ,OT_Rate ,General_Remark,AUTO_SHIFT,SUBMIT,Gender) values ('"+txt_id.Text+"','" + Session["COMP_CODE"].ToString() + "','" + Session["UNIT_CODE"].ToString() + "','" + txt_policy_name1.Text + "', str_to_date('" + txt_start_date.Text + "','%d/%m/%Y'), case when " + txt_end_date.Text.Length + "=0 then null else str_to_date('" + txt_end_date.Text + "','%d/%m/%Y') END, now(),'" + Session["LOGIN_ID"].ToString() + "','" + txt_working_hours.Text + "','" + ddl_roll.SelectedValue + "','" + ddl_transaction.SelectedValue + "','" + ddl_Weekoff.SelectedValue + "','" + txt_Early_In.Text + "','" + txt_Earlyout.Text + "','" + txt_letin.Text + "','" + txt_letout.Text + "','" + txt_txtdeductionpolicy.Text + "','" + ddl_reminder.SelectedValue + "','" + txt_shorthours.Text + "','" + txt_timing.Text + "','" + txt_limitofhours.Text + "','" + txt_punchrealization.Text + "','" + txt_period.Text + "','" + ddl_od.SelectedValue + "','" + txt_min_othrs.Text + "','" + txt_max_othours.Text + "','" + ddl_approval.SelectedValue + "','" + txt_commonoff.Text + "','" + txt_otrate.Text + "','" + txt_generalremark.Text + "','" + flag1 + "','" + ddl_gender.SelectedValue + "','0')");
            int ii = d.operation("INSERT INTO attandance_inout_time_details(COMP_CODE,Policy_Id,IN_TIME,OUT_TIME,Create_by,Created_Date) VALUES('" + Session["COMP_CODE"].ToString() + "','" + txt_id.Text + "','" + txt_intime.Text + "','" + txt_outtime.Text + "','" + Session["LOGIN_ID"].ToString() + "',now())");
        

            
            if (i > 0 && ii>0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Travel Policy Saved Successfully...');", true);
            }

            policy_fill();
        }
        else
        { 
            d.con.Open();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                MySqlCommand cmd1 = new MySqlCommand("Select * from pay_travel_emp_policy Where policy_id in (Select Policy_Id from pay_travel_policy_master Where comp_code='"+Session["comp_code"].ToString()+"' and txt_policy_name ='" + ddl_Existing_policy_name.SelectedItem.ToString() + "')", d.con);
                MySqlDataReader dr1 = cmd1.ExecuteReader();
                if (dr1.Read())
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attendence Policy already assigned you cannot make any Changes......');", true);

                }
                else
                {
                 int i = d.operation("UPDATE attandance_police_master SET POLICY_NAME='" + txt_policy_name1.Text + "' ,START_DATE =str_to_date('" + txt_start_date.Text.ToString() + "','%d/%m/%Y'),END_DATE =case when " + txt_end_date.Text.Length + "=0 then null else str_to_date('" + txt_end_date.Text + "','%d/%m/%Y') END,CREATED_DATE= now(),MODIFY_BY='" + Session["LOGIN_ID"].ToString() + "',WORKING_HOURS='" + txt_working_hours.Text + "',Roll='" + ddl_roll.SelectedValue + "' ,Transation='" + ddl_transaction.SelectedValue + "',Weak_Off='" + ddl_Weekoff.SelectedValue + "' ,Rarly_In='" + txt_Early_In.Text + "' ,EarlyOut='" + txt_Earlyout.Text + "' ,Let_In='" + txt_letin.Text + "' ,Let_Out='" + txt_letout.Text + "',Deduction_Policy='" + txt_txtdeductionpolicy.Text + "',Reminders='" + ddl_reminder.SelectedValue + "' ,Short_Hours='" + txt_shorthours.Text + "',Timing='" + txt_timing.Text + "' ,Limits_Of_Hours='" + txt_limitofhours.Text + "' ,Punch_Regularization='" + txt_punchrealization.Text + "' ,Period='" + txt_period.Text + "' ,OD ='" + ddl_od.SelectedValue + "',Minimum_Ot_urs='" + txt_min_othrs.Text + "',Max_Ot_Hours='" + txt_max_othours.Text + "',Approval_Required='" + ddl_approval.SelectedValue + "',Comp_Off='" + txt_commonoff.Text + "' ,OT_Rate='" + txt_otrate.Text + "' ,General_Remark='" + txt_generalremark.Text + "',AUTO_SHIFT='" + flag1 + "',Gender='" + ddl_gender.SelectedValue + "',SUBMIT='0' WHERE comp_code='"+Session["comp_code"].ToString()+"' and  POLICY_NAME='" + ddl_Existing_policy_name.SelectedItem.Text + "'");
                    if (i > 0)
                    {
                       
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Attendence Policy Update Successfully...');", true);
                       // return;
                    }
                }
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
                
                policy_fill();
                Policy_code();
            }
        }
          text_clear();
        
    }

    protected void btn_submit_Click(object sender, EventArgs e)
    {
        String flag1;
        if (chk_Autoshift.Checked == true)
        {
            flag1 = "1";
        }
        else
        {
            flag1 = "0";
        }

        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            int i = d.operation("UPDATE attandance_police_master SET POLICY_NAME='" + txt_policy_name1.Text + "' ,START_DATE =str_to_date('" + txt_start_date.Text.ToString() + "','%d/%m/%Y'),END_DATE =case when " + txt_end_date.Text.Length + "=0 then null else str_to_date('" + txt_end_date.Text + "','%d/%m/%Y') END,CREATED_DATE= now(),MODIFY_BY='" + Session["LOGIN_ID"].ToString() + "',WORKING_HOURS='" + txt_working_hours.Text + "'Roll='" + ddl_roll.SelectedValue + "' ,Transation='" + ddl_transaction.SelectedValue + "',Weak_Off='" + ddl_Weekoff.SelectedValue + "' ,Rarly_In='" + txt_Early_In.Text + "' ,EarlyOut='" + txt_Earlyout.Text + "' ,Let_In='" + txt_letin.Text + "' ,Let_Out='" + txt_letout.Text + "',Deduction_Policy='" + txt_txtdeductionpolicy.Text + "',Reminders='" + ddl_reminder.SelectedValue + "' ,Short_Hours='" + txt_shorthours.Text + "',Timing='" + txt_timing.Text + "' ,Limits_Of_Hours='" + txt_limitofhours.Text + "' ,Punch_Regularization='" + txt_punchrealization.Text + "' ,Period='" + txt_period.Text + "' ,OD ='" + ddl_od.SelectedValue + "',Minimum_Ot_urs='" + txt_min_othrs.Text + "',Max_Ot_Hours='" + txt_max_othours.Text + "',Approval_Required='" + ddl_approval.SelectedValue + "',Comp_Off='" + txt_commonoff.Text + "' ,OT_Rate='" + txt_otrate.Text + "' ,General_Remark='" + txt_generalremark.Text + "',AUTO_SHIFT='" + flag1 + "',Gender='" + ddl_gender.SelectedValue + "',SUBMIT='1' WHERE comp_code='"+Session["comp_code"].ToString()+"' and Policy_Id=" +txt_id.Text);
            if (i > 0)
            {
               
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Travel Policy Submitted Successfully...');", true);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            policy_fill();
            text_clear();
            btnadd.Visible = false;
            Policy_code();
        }
        
    }

    protected void btndelete_Click(object sender, EventArgs e)
    { 
          try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                int res = 0;
                res = d.operation("DELETE FROM attandance_police_master WHERE Policy_Id= " + txt_id.Text);
                if (res > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record deleted successfully !!');", true);
                    text_clear();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record deletion failed !!');", true);
                }
            }
        
        catch (Exception ee)
        {
            throw ee;
        }
        finally
        {
            d.con.Close();
           text_clear();
            policy_fill();
            Policy_code();
        }
    }

    protected void text_clear()
    {
        txt_policy_name1.Text = "";
       txt_start_date.Text = "";
       txt_end_date.Text = "";
       txt_end_date.Text = "";
        txt_working_hours.Text = "";
        ddl_roll.SelectedValue = "0";
        ddl_transaction.SelectedValue = "0";
        ddl_Weekoff.SelectedValue = "";
        txt_Early_In.Text = "";
        txt_Earlyout.Text = "";
        txt_letin.Text = "";
        txt_letout.Text = "";
        txt_txtdeductionpolicy.Text = "";
        ddl_reminder.SelectedValue = "0";
        txt_shorthours.Text = "";
        txt_timing.Text = "";
        txt_limitofhours.Text = "";
        txt_punchrealization.Text = "";
        txt_period.Text = "";
        ddl_od.SelectedValue = "";
        txt_min_othrs.Text = "";
        txt_max_othours.Text = "";
        ddl_approval.SelectedValue = "0";
        txt_commonoff.Text = "";
        txt_otrate.Text = "";
        txt_generalremark.Text = "";
        ddl_gender.SelectedValue = "0";
        txt_intime.Text = "";
        txt_outtime.Text = "";


        //gv_itemslist.Visible = false;


       
        if (chk_Autoshift != null)
            chk_Autoshift.Checked = false;
    }
  
    
    protected void btncloselow_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }

    

    protected void gv_itemslist_RowDataBound(object sender, GridViewRowEventArgs e)
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
    }


    
    public void Policy_code()
    {
        d.con1.Open();
        try
        {
            MySqlCommand cmdmax = new MySqlCommand("SELECT MAX(CAST(SUBSTRING(Policy_Id, 2, 5) AS UNSIGNED))+1 FROM  attandance_police_master WHERE COMP_CODE='" + Session["COMP_CODE"].ToString() + "'", d.con1);
            MySqlDataReader drmax = cmdmax.ExecuteReader();
            if (drmax.Read())
            {
                string str = drmax.GetValue(0).ToString();
                if (str == "")
                {
                    txt_id.Text = "P001";
                }
                else
                {
                    int max_unitcode = int.Parse(drmax.GetValue(0).ToString());
                    if (max_unitcode < 10)
                    {
                        txt_id.Text = "P00" + max_unitcode;
                    }
                    else if (max_unitcode > 9 && max_unitcode < 100)
                    {
                        txt_id.Text = "P0" + max_unitcode;
                    }
                    else if (max_unitcode > 99 && max_unitcode < 1000)
                    {
                        txt_id.Text = "P" + max_unitcode;
                    }
                }
            }
            drmax.Close();
            d.con1.Close();
        }
        catch (Exception ex) { throw ex; }
        finally { d.con1.Close(); }
    }

}