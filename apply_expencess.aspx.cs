using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class apply_expencess : System.Web.UI.Page
{
    DAL d = new DAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["comp_code"] == null || Session["comp_code"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }
        if (d.getaccess(Session["ROLE"].ToString(), "Add Travel Plan Master", Session["COMP_CODE"].ToString()) == "I")
        {
            Response.Redirect("unauthorised_access.aspx");
        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Add Travel Plan Master", Session["COMP_CODE"].ToString()) == "R")
        {

        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Add Travel Plan Master", Session["COMP_CODE"].ToString()) == "U")
        {

        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Add Travel Plan Master", Session["COMP_CODE"].ToString()) == "C")
        {

        }
        else if (d.getaccess(Session["ROLE"].ToString(), "Add Travel Plan Master", Session["COMP_CODE"].ToString()) == "D")
        {

        }

        if (!IsPostBack)
        {
            load_gridview();

        }
    }

    protected void btn_close_Click(object sender, EventArgs e)
    {
        Session["EXPENSE_ID"] = "";
        Response.Redirect("Home.aspx");
    }

    protected void gv_expeness_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
        try
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                gv_expeness.ShowFooter = false;
                gv_expeness.FooterRow.Visible = false;
                
            }

            //if (e.Row.Cells[6].Text.Equals("Seeking Clarification") || e.Row.Cells[6].Text.Equals("Cancelled") || e.Row.Cells[6].Text.Equals("Draft"))
            //{
            //    ((LinkButton)e.Row.Cells[13].FindControl("lnkclaimexpense")).Visible = false;
            //}
            ////if (e.Row.Cells[6].Text.Equals("Submitted") || e.Row.Cells[6].Text.Equals("Approved") || e.Row.Cells[6].Text.Equals("NA") || e.Row.Cells[6].Text.Equals("Rejected"))
            ////{
            ////    ((LinkButton)e.Row.Cells[11].FindControl("lnkView")).Visible = false;
            ////}

            //if (e.Row.Cells[9].Text.Equals("Approved") || e.Row.Cells[9].Text.Equals("Submitted") || e.Row.Cells[6].Text.Equals("Rejected"))
            //{
            //    ((LinkButton)e.Row.Cells[13].FindControl("lnkclaimexpense")).Visible = false;
            //}
            //if (e.Row.Cells[9].Text.Equals("Draft"))
            //{
            //    ((LinkButton)e.Row.Cells[13].FindControl("lnkclaimexpense")).Visible = true;
            //}

        }
        catch (Exception ex) {  }
        e.Row.Cells[0].Visible = false;
    }

    private void load_gridview()
    {
        d.con1.Open();
        try
        {
            DataTable dt_id = new DataTable();
            MySqlCommand cmd = new MySqlCommand("SELECT b.expenses_id, (select  group_concat(distinct(travel_mode)) from apply_travel_plan a where a.expenses_id = b.expenses_id) as travel_mode, from_designation,(select to_designation from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as to_designation,date_format(from_date,'%d/%m/%Y') as from_date,(select date_format(to_date,'%d/%m/%Y') from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as to_date,b.Comments, b.expense_status,  (select IFNULL(sum(Amount),0) from pay_add_expenses a where a.expenses_id = b.expenses_id) as claim_amount,pay_add_expenses.status as Claim_Status,pay_add_expenses.comments AS 'Comments',date_format(pay_add_expenses.NowDate,'%d/%m/%Y') as NowDate, (select case when pay_add_expenses.status='Approved' then IFNULL(sum(app_amount),0) else 0 END as app_amount from pay_add_expenses a where a.expenses_id = b.expenses_id) as app_amount from apply_travel_plan b left outer join pay_add_expenses on b.Expenses_id = pay_add_expenses.Expenses_id WHERE b.emp_code = '" + Session["LOGIN_ID"] + "' and b.comp_code = '" + Session["comp_code"] + "' group by b.expenses_id order by b.id desc", d.con1);
            DataSet ds1 = new DataSet();
            MySqlDataAdapter adp1 = new MySqlDataAdapter(cmd);
            adp1.Fill(ds1);
            gv_expeness.DataSource = ds1.Tables[0];
            gv_expeness.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            d.con1.Close();
        }
    }


    protected void btn_add_new_exp_Click(object sender, EventArgs e)
    {
        Panel2.Visible = false;
        gv_expeness.Visible = false;
    }


    protected void lnkView_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        Session["EXPENSE_ID"] = grdrow.Cells[0].Text;
        if (travel_policy())
        {
            ModalPopupExtender1.Show();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Travel Policy not assigned / Expired. Please talk to your Manager.');", true);
     
        }
    }
    protected void lnkdelete_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        d.operation("delete from apply_travel_plan where expenses_id = '" + grdrow.Cells[0].Text + "'");
        d.operation("delete from pay_add_expenses where expenses_id = '" + grdrow.Cells[0].Text + "'");
        load_gridview();
    }
    protected void lnkclaimexpense_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        Session["EXPENSE_ID"] = grdrow.Cells[0].Text;
        Session["expfrom_date"] = grdrow.Cells[4].Text;
        Session["expTo_date"] = grdrow.Cells[5].Text;
        if (travel_policy())
        {
            mp1.Show();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Travel Policy not assigned / Expired. Please talk to your Manager.');", true);
        }
    }
    protected void Button4_Click(object sender, EventArgs e)
    {
        load_gridview();
    }

    protected void lnkaddtravelplan_Click(object sender, EventArgs e)
    {
        Session["EXPENSE_ID"] = "";
        if (travel_policy())
        {
            ModalPopupExtender1.Show();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Travel Policy not assigned / Expired. Please talk to your Manager.');", true);
        }
    }

    private bool travel_policy()
    {
        bool return_value = false;
        d.con.Open();
        try
        {
            MySqlCommand cmd_1 = new MySqlCommand("select chkair from pay_travel_policy_master where CURDATE() between txt_start_date and case when txt_end_date is null then now() else txt_end_date END and id = (select policy_id from pay_travel_emp_policy where emp_code = '" + Session["LOGIN_ID"].ToString() + "')", d.con);
            MySqlDataReader cad1 = cmd_1.ExecuteReader();
            if (cad1.HasRows)
            {
                return_value = true;
            }
            return return_value;
        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }
    }
    protected void lnkclaimexpense_Click1(object sender, EventArgs e)
    {
        Session["EXPENSE_ID"] = "";
        if (travel_policy())
        {
            mp1.Show();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Travel Policy not assigned / Expired. Please talk to your Manager.');", true);
        }

        Session["iddata"] = 1;
    }

    protected void gv_expeness_PreRender(object sender, EventArgs e)
    {
        try
        {
            gv_expeness.UseAccessibleHeader = false;
            gv_expeness.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }//vinod dont apply catch
        
    }
}