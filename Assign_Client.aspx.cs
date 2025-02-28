using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;

public partial class Assign_Client : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();
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
            //try
            //{
                d.con1.Open();
                try
                {
                    MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter("SELECT client_code, CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name' FROM pay_client_master where comp_code = '" + Session["COMP_CODE"].ToString() + "'", d.con1);
                    DataSet DS = new DataSet();
                    MySqlDataAdapter.Fill(DS);
                    ddl_client.DataSource = DS;
                    ddl_client.DataValueField = "client_code";
                    ddl_client.DataTextField = "client_NAME";
                    ddl_client.DataBind();
                }
                catch (Exception ex) { throw ex; }
                finally
                {
                    d.con1.Close();
                    ddl_client.Items.Insert(0, new ListItem("Select", "Select"));
                }

                //d.con.Open();
                //MySqlCommand cmd_1 = new MySqlCommand("SELECT EMP_NAME, EMP_CODE FROM pay_employee_master where comp_code = '" + Session["COMP_CODE"].ToString() + "' order by EMP_NAME", d.con);
                //MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
                //DataSet ds1 = new DataSet();
                //cad1.Fill(ds1);
                //ddl_employee.DataSource = ds1.Tables[0];
                //ddl_employee.DataValueField = "EMP_CODE";
                //ddl_employee.DataTextField = "EMP_NAME";
            //    //ddl_employee.DataBind();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    ddl_employee.Items.Insert(0, new ListItem("Select"));
            //    d.con.Close();
            //}

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

    //protected void checkemp_click(object sender, EventArgs e)
    //{
    //    if (rdb_client.Checked == true)
    //    {
    //        try
    //        {
    //            d.con.Open();
    //            MySqlCommand cmd_1 = new MySqlCommand("SELECT EMP_NAME, emp_code FROM pay_employee_master where comp_code = '" + Session["COMP_CODE"].ToString() + "' order by EMP_NAME", d.con);
    //            MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
    //            DataSet ds1 = new DataSet();
    //            cad1.Fill(ds1);
    //            counter = 1;
    //            lstLeft.DataSource = ds1.Tables[0];
    //            lstLeft.DataValueField = "emp_code";
    //            lstLeft.DataTextField = "EMP_NAME";
    //            lstLeft.DataBind();

    //            checklistcox.Items.Clear();
                
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //          // lstLeft.Items.Clear();
    //            d.con.Close();
    //            checklistcox.Items.Clear();
    //        }
    //    }
    //    else
    //    {
    //        lstLeft.Items.Clear();
    //        //checkemp.Checked = true;
    //       // checklistcox.Items.Clear(); 
    //        counter = 0;
           
    //    }

    //}
    //protected void ChecUnit_click(object sender, EventArgs e)
    //{
    //    if (ChecUnit.Checked == true && checkemp.Checked == false && CheckDepartment.Checked == false)
    //    {
           

    //        try
    //        {
    //            d.con.Open();
    //            checklistcox.Visible = true;
    //            MySqlCommand cmd_empcode = new MySqlCommand("select UNIT_NAME from pay_unit_master where comp_code = '" + Session["COMP_CODE"].ToString() + "' ", d.con);
    //            MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_empcode);
    //            DataSet ds1 = new DataSet();
    //            cad1.Fill(ds1);
    //            checklistcox.Visible = true;
    //            checklistcox.DataSource = ds1.Tables[0];
    //            checklistcox.DataValueField = "UNIT_NAME";
    //            checklistcox.DataTextField = "UNIT_NAME";
    //            checklistcox.DataBind();

    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            d.con.Close();
    //            lstLeft.Items.Clear();
    //        }
    //    }
    //    else
    //    {
    //      //  if (counter == 0)
    //      //  {
    //        //ChecUnit.Checked = false;
    //            checklistcox.Items.Clear();
    //            lstLeft.Items.Clear();
    //      //  }
    //    }
    //}

    //protected void checklistcox_OnSelectedIndexChanged(object sender, EventArgs e)
    //{
    //    string name = "";
    //    if (CheckDepartment.Checked == true)
    //    {
    //        if (checklistcox.SelectedItem != null)
    //        {

              
    //            try
    //            {
    //                foreach (ListItem lst in checklistcox.Items)
    //                {
    //                    if (lst.Selected == true)
    //                    {
    //                       // string abc = lst.Value.ToString();
    //                        if (name == "")
    //                        {
    //                            name = (lst.Value.ToString()) + ",";
    //                        }
    //                        else
    //                        {
    //                            name = name + (lst.Value.ToString()) + ",";
    //                        }
    //                    }
    //                }

    //                if (name.EndsWith(","))
    //                {
    //                    name = name.Remove(name.Length - 1, 1);
    //                    name = name.Replace(",", "','");
    //                }

    //                d.con.Open();
    //                MySqlCommand cmd_1 = new MySqlCommand("select emp_name, emp_code from pay_employee_master,pay_client_master where pay_employee_master.comp_code ='" + Session["COMP_CODE"] + "' and pay_client_master.CLIENT_CODE in ('" + name + "') and  pay_employee_master.CLIENT_CODE= pay_client_master.CLIENT_CODE order by emp_name ", d.con);
    //                MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
    //                DataSet ds1 = new DataSet();
    //                cad1.Fill(ds1);
    //                lstLeft.DataSource = ds1.Tables[0];
    //                lstLeft.DataValueField = "emp_code";
    //                lstLeft.DataTextField = "emp_name";
    //                lstLeft.DataBind();
    //                lstLeft.Visible = true;
    //                //d.con.Close();
    //            }
    //            catch (Exception ex)
    //            { throw ex; }
    //            finally
    //            {
    //               d.con.Close();
    //            }
    //        }
    //        else
    //        {
    //           lstLeft.Items.Clear();
    //        }
    //    }
    //    else if (ChecUnit.Checked == true)
    //    {
    //        if (checklistcox.SelectedItem != null)
    //        {
    //            try
    //            {
    //                d.con.Open();
    //                MySqlCommand cmd_1 = new MySqlCommand("select emp_name, emp_code from pay_employee_master,pay_unit_master where pay_employee_master.comp_code ='" + Session["COMP_CODE"] + "' and UNIT_NAME='" + checklistcox.SelectedItem.Text + "' and  pay_employee_master.UNIT_CODE= pay_unit_master.UNIT_CODE order by emp_name ", d.con);
    //                MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
    //                DataSet ds1 = new DataSet();
    //                cad1.Fill(ds1);
    //                lstLeft.DataSource = ds1.Tables[0];
    //                lstLeft.DataValueField = "emp_code";
    //                lstLeft.DataTextField = "emp_name";
    //                lstLeft.DataBind();
    //                lstLeft.Visible = true;
    //            }
    //            catch (Exception ex)
    //            { throw ex; }
    //            finally
    //            {
    //                d.con.Close();
    //            }
    //        }
    //       // else { lstLeft.Items.Clear(); }
    //    }
    //}


    //protected void CheckDepartment_click(object sender, EventArgs e)
    //{
    //    if (CheckDepartment.Checked == true && ChecUnit.Checked == false && checkemp.Checked == false)
    //    {
    //        try
    //        {
    //            string com_code = Session["COMP_CODE"].ToString();
    //            checklistcox.Visible = true;
    //            d.con.Open();
    //            MySqlCommand cmd1 = new MySqlCommand("SELECT CLIENT_NAME,CLIENT_CODE FROM pay_client_master WHERE COMP_CODE='" + Session["COMP_CODE"].ToString() + "'  ", d.con);
    //            DataSet ds1 = new DataSet();
    //            MySqlDataAdapter adp1 = new MySqlDataAdapter(cmd1);
    //            adp1.Fill(ds1);
    //            checklistcox.Visible = true;
    //            checklistcox.DataSource = ds1.Tables[0];
    //            checklistcox.DataValueField = "CLIENT_CODE";
    //            checklistcox.DataTextField = "CLIENT_NAME";
    //            checklistcox.DataBind();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            d.con.Close();
    //            lstLeft.Items.Clear();
    //        }
    //    }
    //    else
    //    {
    //        //if (counter == 0)
    //        //{
    //            //CheckDepartment.Checked = false;
    //            checklistcox.Items.Clear();
    //            lstLeft.Items.Clear();
    //       // }
    //    }
    //}

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

    protected void btnSubmit_click(object sender, EventArgs e)
    {
        int res = 0;
        //int li_select = 0;
        try
        {
            if (ddl_client.SelectedIndex >= 0)
            {
                for (int i = 0; i < ddl_client.Items.Count; i++)
                {
                    if (ddl_client.Items[i].Selected)
                    {
                        if (lst_branch.SelectedIndex >= 0)
                        {
                            for (int j = 0; j < lst_branch.Items.Count; j++)
                            {
                                if (lst_branch.Items[j].Selected)
                                {
                                    foreach (ListItem li in lstRight.Items)
                                    {
                                        res = d.operation("INSERT INTO PAY_ASSIGN_CLIENT (COMP_CODE,CLIENT_CODE,UNIT_CODE,EMP_CODE) VALUES('" + Session["COMP_CODE"].ToString() + "','" + ddl_client.Items[i].Value + "','" + lst_branch.Items[j].Value + "','" + li.Value + "')");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (res > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Client Assign Successfully !!!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Client Assign Failed');", true);
            }
        }
        catch (Exception ee)
        {
            throw ee;
        }
        finally
        {
           
            lstRight.Items.Clear();
        }
    }
  
    protected void rdb_branch_list_CheckedChanged(object sender, EventArgs e)
    {
        string name = "";
        if (rdb_branch_list.Checked == true)
        {
            foreach (ListItem lst in ddl_client.Items)
            {
                if (lst.Selected == true)
                {
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
        }
        if (name.EndsWith(","))
        {
            name = name.Remove(name.Length - 1, 1);
            name = name.Replace(",", "','");
        }
        try
        {
            d.con.Open();

            MySqlCommand cmd_empcode = new MySqlCommand("select UNIT_CODE,UNIT_NAME from pay_unit_master where comp_code = '" + Session["COMP_CODE"].ToString() + "' and CLIENT_CODE in ('" + name + "')  ", d.con);
            MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_empcode);
            DataSet ds1 = new DataSet();
            cad1.Fill(ds1);
            if (ds1.Tables[0].Rows.Count > 0)
            {
                lst_branch.DataSource = ds1.Tables[0];
                lst_branch.DataValueField = "UNIT_CODE";
                lst_branch.DataTextField = "UNIT_NAME";
                lst_branch.DataBind();

            }
            else {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Branch For This Client !!!');", true);
            }
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
    protected void rdb_employee_CheckedChanged(object sender, EventArgs e)
    {
        string name = "";
        if (rdb_employee.Checked == true)
        {
            try
            {

                foreach (ListItem lst in lst_branch.Items)
                {
                    if (lst.Selected == true)
                    {
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
                MySqlCommand cmd_1 = new MySqlCommand("SELECT EMP_NAME, emp_code FROM pay_employee_master where comp_code = '" + Session["COMP_CODE"].ToString() + "' and UNIT_CODE in ('" + name + "') order by EMP_NAME", d.con);
                MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
                DataSet ds1 = new DataSet();
                cad1.Fill(ds1);
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    lstLeft.DataSource = ds1.Tables[0];
                    lstLeft.DataValueField = "emp_code";
                    lstLeft.DataTextField = "EMP_NAME";
                    lstLeft.DataBind();

                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Employee For This Branch !!!');", true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // lstLeft.Items.Clear();
                d.con.Close();
                //checklistcox.Items.Clear();
            }
        }
        else
        {
            lstLeft.Items.Clear();
            //checkemp.Checked = true;
            // checklistcox.Items.Clear(); 
            //counter = 0;

        }
    }
}    