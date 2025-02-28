using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;

public partial class Travelling_Management : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();
    ArrayList arraylist1 = new ArrayList();
    ArrayList arraylist2 = new ArrayList();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["comp_code"] == null || Session["comp_code"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }		
        if (!IsPostBack)
        {
            lstLeft.Visible = true;
            load_ddls();
        }
    }
    private void load_ddls()
    {
        //clients ddl
        ddl_client.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', client_code from pay_client_master where comp_code='" + Session["comp_code"] + "' ORDER BY client_code", d.con);
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
    
        //Policy ddl
        ddlpolicies.Items.Clear();
         dt_item = new System.Data.DataTable();
         cmd_item = new MySqlDataAdapter("Select distinct(policy_name1) from pay_billing_master where comp_code='" + Session["comp_code"] + "' ORDER BY 1", d.con);
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddlpolicies.DataSource = dt_item;
                ddlpolicies.DataTextField = dt_item.Columns[0].ToString();
                ddlpolicies.DataValueField = dt_item.Columns[0].ToString();
                ddlpolicies.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
            ddlpolicies.Items.Insert(0, "Select");
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
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
        int li_select = 0;
        try
        {
            d.operation("insert into pay_billing_unit_policy (comp_code,unit_code) select comp_code,unit_code from pay_unit_master where comp_code = '" + Session["comp_code"].ToString() + "' and unit_code not in (select unit_code from pay_billing_unit_policy where comp_code = '" + Session["comp_code"].ToString() + "')");
            res = d.operation("update pay_billing_unit_policy set policy_name = null where comp_code = '" + Session["comp_code"].ToString() + "' and policy_name = '" + ddlpolicies.SelectedValue.ToString() + "'");

            foreach (ListItem li in lstRight.Items)
            {
                li_select++;
                res = d.operation("update pay_billing_unit_policy set policy_name = '" + ddlpolicies.SelectedValue + "' where comp_code = '" + Session["comp_code"].ToString() + "' and unit_code = '" + li.Value + "'");
            }
            if (res > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Branches added successfully to Policy.');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Branches adding failed to Policy.');", true);
            }
        }
        catch (Exception ee)
        {
            throw ee;
        }
        finally
        {
            ddlpolicies.SelectedIndex = 0;
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
                MySqlCommand cmd_1 = new MySqlCommand("select unit_code, CONCAT( (SELECT DISTINCT(STATE_CODE) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME from pay_unit_master where unit_code in (select unit_code from pay_billing_unit_policy where policy_name = '" + ddlpolicies.SelectedValue + "') and comp_code ='" + Session["comp_code"] + "'", d.con);
                MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
                DataSet ds1 = new DataSet();
                cad1.Fill(ds1);
                lstRight.DataSource = ds1.Tables[0];
                lstRight.DataValueField = "unit_code";
                lstRight.DataTextField = "unit_name";
                lstRight.DataBind();
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
    protected void ddl_client_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!ddlpolicies.SelectedValue.Equals("0"))
        {
            try
            {
                d.con.Open();
                MySqlCommand cmd_1 = new MySqlCommand("select unit_code, CONCAT( (SELECT DISTINCT(STATE_name) FROM pay_state_master WHERE STATE_NAME = pay_unit_master.STATE_NAME),'_',UNIT_CITY,'_',UNIT_ADD1,'_',UNIT_NAME) as UNIT_NAME from pay_unit_master where comp_code ='" + Session["comp_code"] + "' and client_code = '"+ddl_client.SelectedValue+"'", d.con);
                MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
                DataSet ds1 = new DataSet();
                cad1.Fill(ds1);
                lstLeft.DataSource = ds1.Tables[0];
                lstLeft.DataValueField = "unit_code";
                lstLeft.DataTextField = "unit_name";
                lstLeft.DataBind();
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
        ddlpolicies_SelectedIndexChanged(null,null);
    }
}