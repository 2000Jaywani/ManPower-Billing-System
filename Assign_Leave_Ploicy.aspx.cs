using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;

public partial class Assign_Leave_Ploicy : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();
    ArrayList arraylist1 = new ArrayList();
    ArrayList arraylist2 = new ArrayList();
    int counter = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["comp_code"] == null || Session["comp_code"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }		
        if (!IsPostBack)
        {
            lstLeft.Visible = true;
            client_list();
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
      
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                d.con.Open();
                MySqlCommand cmd_1 = new MySqlCommand("SELECT EMP_NAME, emp_code FROM pay_employee_master where Employee_type='Staff' and  comp_code = '" + Session["comp_code"].ToString() + "' and client_code='"+ddl_labour_client.SelectedValue+"' and unit_code='"+ddl_labour_branch.SelectedValue+"' and location='"+ddl_labour_state.SelectedValue+"' order by EMP_NAME", d.con);
                MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
                DataSet ds1 = new DataSet();
                cad1.Fill(ds1);
                counter = 1;
                lstLeft.DataSource = ds1.Tables[0];
                lstLeft.DataValueField = "emp_code";
                lstLeft.DataTextField = "EMP_NAME";
                lstLeft.DataBind();

                radiolistcox.Items.Clear();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
              // lstLeft.Items.Clear();
                d.con.Close();
                radiolistcox.Items.Clear();
            }
       // }
       

    }
    protected void ChecUnit_click(object sender, EventArgs e)
    {
        if (ChecUnit.Checked == true && checkemp.Checked == false && CheckDepartment.Checked == false)
        {

            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                d.con.Open();
                RadioButtonList1.Visible = true;
                MySqlCommand cmd_empcode = new MySqlCommand("select UNIT_NAME from pay_unit_master where client_code = '" + radiolistcox.Text + "' ", d.con);
                MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_empcode);
                DataSet ds1 = new DataSet();
                cad1.Fill(ds1);
                RadioButtonList1.Visible = true;
                RadioButtonList1.DataSource = ds1.Tables[0];
                RadioButtonList1.DataValueField = "UNIT_NAME";
                RadioButtonList1.DataTextField = "UNIT_NAME";
                RadioButtonList1.DataBind();

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
            
            radiolistcox.Items.Clear();
                lstLeft.Items.Clear();
          //  }
        }
    }

    protected void checklistcox_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        string name = "";
        if (CheckDepartment.Checked == true)
        {
            if (radiolistcox.SelectedItem != null)
            {

              
                try
                {
                    foreach (ListItem lst in radiolistcox.Items)
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
                    MySqlCommand cmd_1 = new MySqlCommand("select emp_name, emp_code from pay_employee_master,pay_client_master where pay_employee_master.comp_code ='" + Session["comp_code"] + "' and pay_client_master.CLIENT_CODE in ('" + name + "') and  pay_employee_master.CLIENT_CODE= pay_client_master.CLIENT_CODE order by emp_name ", d.con);
                    MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
                    DataSet ds1 = new DataSet();
                    cad1.Fill(ds1);
                    lstLeft.DataSource = ds1.Tables[0];
                    lstLeft.DataValueField = "emp_code";
                    lstLeft.DataTextField = "emp_name";
                    lstLeft.DataBind();
                    lstLeft.Visible = true;
                    //d.con.Close();
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
            if (radiolistcox.SelectedItem != null)
            {
                try
                {
                    d.con.Open();
                    MySqlCommand cmd_1 = new MySqlCommand("select emp_name, emp_code from pay_employee_master,pay_unit_master where pay_employee_master.comp_code ='" + Session["comp_code"] + "' and UNIT_NAME='" + radiolistcox.SelectedItem.Text + "' and  pay_employee_master.UNIT_CODE= pay_unit_master.UNIT_CODE order by emp_name ", d.con);
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

    public void client_list()
    {
        d.con1.Close();
   
        ddl_labour_client.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT DISTINCT pay_client_master.client_code, client_NAME FROM pay_client_master INNER JOIN pay_client_state_role_grade ON pay_client_master.COMP_CODE = pay_client_state_role_grade.COMP_CODE and pay_client_master.client_code = pay_client_state_role_grade.client_code WHERE pay_client_master.comp_code = '" + Session["COMP_CODE"].ToString() + "' AND pay_client_state_role_grade.emp_code IN (" + Session["REPORTING_EMP_SERIES"] + ")", d.con1);
        d.con1.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_labour_client.DataSource = dt_item;
                ddl_labour_client.DataTextField = dt_item.Columns[1].ToString();
                ddl_labour_client.DataValueField = dt_item.Columns[0].ToString();
                ddl_labour_client.DataBind();
            }
            dt_item.Dispose();
            d.con1.Close();
          
            ddl_labour_client.Items.Insert(0, "Select");

        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con1.Close();

        }

    }

    protected void ddl_client_labour_SelectedIndexChanged(object sender, EventArgs e)
    {

        ddl_labour_state.Items.Clear();
        if (ddl_labour_client.SelectedValue != "Select")
        {

            //State
            ddl_labour_state.Items.Clear();
            ddl_labour_branch.Items.Clear();
            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("select distinct(STATE_NAME) from pay_client_state_role_grade where comp_code='" + Session["COMP_CODE"].ToString() + "' and client_code = '" + ddl_labour_client.SelectedValue + "' and EMP_CODE in (" + Session["REPORTING_EMP_SERIES"].ToString() + ") ", d.con);

            d.con.Open();
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
           
                cmd_item.Fill(dt_item);
                if (dt_item.Rows.Count > 0)
                {
                    ddl_labour_state.DataSource = dt_item;
                    ddl_labour_state.DataTextField = dt_item.Columns[0].ToString();
                    ddl_labour_state.DataValueField = dt_item.Columns[0].ToString();
                    ddl_labour_state.DataBind();
                }
                dt_item.Dispose();
                d.con.Close();
                ddl_labour_state.Items.Insert(0, "Select");
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
                ddl_labour_branch.Items.Clear();

            }
        }


    }

    protected void ddl_state_labour_SelectedIndexChanged(object sender, EventArgs e)
    {
       
        ddl_labour_branch.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CONCAT((SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_NAME,'_',UNIT_ADD1) as UNIT_NAME, unit_code,flag from pay_unit_master where state_name = '" + ddl_labour_state.SelectedValue + "' and comp_code='" + Session["comp_code"] + "' and client_code = '" + ddl_labour_client.SelectedValue + "' AND UNIT_CODE in(select UNIT_CODE from pay_client_state_role_grade where  COMP_CODE='" + Session["COMP_CODE"].ToString() + "' AND  EMP_CODE in (" + Session["REPORTING_EMP_SERIES"].ToString() + ") AND client_code='" + ddl_labour_client.SelectedValue + "') AND branch_status = 0  ORDER BY 1", d.con);
        d.con.Open();
        try
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_labour_branch.DataSource = dt_item;
                ddl_labour_branch.DataTextField = dt_item.Columns[0].ToString();
                ddl_labour_branch.DataValueField = dt_item.Columns[1].ToString();
                ddl_labour_branch.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
            //vikas
            ddl_labour_branch.Items.Insert(0, "Select");
        }
        catch (Exception ex) { throw ex; }
        finally
        {

            d.con.Close();
          


        }
    }

    protected void CheckDepartment_click(object sender, EventArgs e)
    {
        if (CheckDepartment.Checked == true && ChecUnit.Checked == false && checkemp.Checked == false)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                string com_code = Session["comp_code"].ToString();
                radiolistcox.Visible = true;
                d.con.Open();
                MySqlCommand cmd1 = new MySqlCommand("SELECT CLIENT_NAME,CLIENT_CODE FROM pay_client_master WHERE comp_code='" + Session["comp_code"].ToString() + "'  ", d.con);
                DataSet ds1 = new DataSet();
                MySqlDataAdapter adp1 = new MySqlDataAdapter(cmd1);
                adp1.Fill(ds1);
                radiolistcox.Visible = true;
                radiolistcox.DataSource = ds1.Tables[0];
                radiolistcox.DataValueField = "CLIENT_CODE";
                radiolistcox.DataTextField = "CLIENT_NAME";
                radiolistcox.DataBind();

                ddl_labour_client.Visible = true;
                ddl_labour_client.DataSource = ds1.Tables[0];
                ddl_labour_client.DataValueField = "CLIENT_CODE";
                ddl_labour_client.DataTextField = "CLIENT_NAME";
                ddl_labour_client.DataBind();
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
            radiolistcox.Items.Clear();
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
               // MySqlCommand cmd1 = new MySqlCommand("SELECT id,txt_policy_name FROM pay_travel_policy_master  WHERE comp_code=@comp_code and UNIT_CODE=UNIT_CODE", d1.con1);
                MySqlCommand cmd1 = new MySqlCommand("select concat('Its ',EMP_CODE,'s birthday today - ', DATE_FORMAT(str_to_date('" + Application["policy_id"].ToString() + "','%d/%m'),'%d %b'))as noti from pay_employee_master WHERE date_format(BIRTH_DATE,'%d/%m') = substring('" + Application["policy_id"].ToString() + "',1,5)", d1.con1);
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
                        MySqlCommand cmd1 = new MySqlCommand("select concat('Its ',EMP_NAME,'s birthday today - ', DATE_FORMAT(str_to_date('" + Application["policy_id"].ToString() + "','%d/%m'),'%d %b'))as noti from pay_employee_master WHERE date_format(BIRTH_DATE,'%d/%m') = substring('" + Application["policy_id"].ToString() + "',1,5)", d1.con1);
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
        //insert into pay_notification_master (notification,not_read,EMP_CODE,page_name) select (select concat('Its ',EMP_NAME,' birthday today - ', DATE_FORMAT(NOW(),'%d %b %y')) from pay_employee_master WHERE str_to_date(BIRTH_DATE,'%d/%m/%Y') = date(now())),'0',EMP_CODE,'birthday.aspx' from pay_employee_master

    }

    protected void btnSubmit_click(object sender, EventArgs e)
    {
        int res = 0;
        int li_select = 0;
        try
        {
            update_notification();


            d.operation("insert into pay_assign_leave_policy (comp_code,unit_code,emp_code,policy_id,policy_name,client_code) select comp_code,unit_code,emp_code,0,'',client_code from pay_employee_master where comp_code = '" + Session["comp_code"].ToString() + "' AND `Employee_type`='Staff'  and emp_code not in ( select emp_code from pay_assign_leave_policy where comp_code = '" + Session["comp_code"].ToString() + "' )");
            

              foreach (ListItem li in lstRight.Items)
            {
                li_select++;

                res = d.operation("UPDATE pay_assign_leave_policy SET policy_id = '" + ddlpolicies.SelectedValue.ToString() + "',policy_name =( SELECT policy_name FROM pay_policy_master WHERE id =' " + ddlpolicies.SelectedValue.ToString() + "')   WHERE  emp_code = '" + li.Value + "' and client_code='"+ddl_labour_client.SelectedValue+"' and unit_code='" + ddl_labour_branch.SelectedValue + "'");

                 //   res = d.operation("UPDATE pay_assign_leave_policy SET policy_id = '" + ddlpolicies.SelectedValue.ToString() + "',policy_name =( SELECT policy_name FROM pay_policy_master WHERE id =' " + ddlpolicies.SelectedValue.ToString() + "')   WHERE  emp_code = '" + li.Value + "' and client_code=(select client_code from pay_client_master where client_name='" + radiolistcox.SelectedItem + "') and unit_code=(select unit_code from pay_unit_master where unit_name='" + RadioButtonList1.SelectedValue + "')");
            }
            if (res > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Employees added successfully to Policy.');", true);
                 
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
            ddlpolicies.SelectedValue="Select Policy";
            ddl_labour_client.SelectedValue = "Select";
            ddl_labour_state.SelectedValue = "Select";
            ddl_labour_branch.SelectedValue = "Select";
            checkemp.Checked = false;
            lstLeft.Items.Clear();
        }
    }
    protected void ddlpolicies_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!ddlpolicies.SelectedValue.Equals("0"))
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true);
                d.con.Open();
             //   MySqlCommand cmd_1 = new MySqlCommand("select emp_name, emp_code from pay_employee_master where emp_code in (select emp_code from pay_assign_attandance_policy where Id = " + ddlpolicies.SelectedValue + ") and comp_code ='" + Session["comp_code"] + "' and unit_code ='" + Session["UNIT_CODE"] + "' order by EMP_NAME", d.con);
                MySqlCommand cmd_1 = new MySqlCommand("select emp_name, emp_code from pay_employee_master where emp_code in (select emp_code from pay_assign_leave_policy where POLICY_ID = " + ddlpolicies.SelectedValue + ") and comp_code ='" + Session["comp_code"] + "' order by EMP_NAME", d.con);
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
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT id,POLICY_NAME FROM pay_policy_master where comp_code='" + Session["comp_code"] + "'", d.con);
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
            d.conclose();
        }


        ddlpolicies.Items.Insert(0, new ListItem("Select Policy", "Select Policy"));



    }

    protected void radiolistcox_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }

    protected void radiolistcox_SelectedIndexChanged_BRANCH(object sender, EventArgs e)
    {

        try
        {
            d.con.Open();
            MySqlCommand cmd_1 = new MySqlCommand("select emp_name, emp_code from pay_employee_master,pay_unit_master where pay_employee_master.comp_code ='" + Session["comp_code"] + "' and pay_unit_master.client_code='" + radiolistcox.SelectedValue + "' and UNIT_NAME='" + RadioButtonList1.SelectedItem.Text + "' and   pay_employee_master.UNIT_CODE= pay_unit_master.UNIT_CODE order by emp_name ", d.con);
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
}