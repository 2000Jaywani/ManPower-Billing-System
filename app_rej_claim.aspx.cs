using System;
using System.Data;
using MySql.Data.MySqlClient;
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
            if (e.Row.Cells[6].Text.Equals("Approved") || e.Row.Cells[6].Text.Equals("Rejected"))
            {
                ((LinkButton)e.Row.Cells[11].FindControl("lnkView")).Visible = false;
            }
        }
        catch (Exception ex) { }

        e.Row.Cells[0].Visible = false;
    }

    private void load_gridview()
    {
        d.con1.Open();
        try
        {
            DataTable dt_id = new DataTable();
            MySqlCommand cmd = new MySqlCommand("SELECT b.expenses_id,  pay_add_expenses.type,(SELECT GROUP_CONCAT(DISTINCT (`travel_type`)) FROM `pay_add_expenses` a WHERE `a`.`expenses_id` = `b`.`expenses_id`) AS 'travel_type',CASE WHEN pay_add_expenses.`city_type` = 1 THEN 'Inside city' WHEN pay_add_expenses.`city_type` = 2 THEN 'Outside city' END AS 'city_type', DATE_FORMAT(`pay_add_expenses`.`date`, '%d/%m/%Y') as 'date',(select  group_concat(distinct(travel_mode)) from apply_travel_plan a where a.expenses_id = b.expenses_id) as travel_mode,from_designation,(select to_designation from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as to_designation,date_format(from_date,'%d/%m/%Y') as from_date,(select date_format(to_date,'%d/%m/%Y') from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as to_date, (select Add_Description from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as Add_Description,(select status from pay_add_expenses a  where a.Expenses_id = b.expenses_id order by id desc limit 1) as status, b.modified_by,(select Comments from pay_add_expenses a  where a.Expenses_id = b.expenses_id order by id desc limit 1) as Comments, (select  sum(Amount) from pay_add_expenses a  where a.expenses_id = b.expenses_id) as claim_amount, (select  sum(app_amount) from pay_add_expenses a where a.expenses_id = b.expenses_id) as app_amount from apply_travel_plan b inner join pay_add_expenses on b.Expenses_id = pay_add_expenses.Expenses_id ,pay_employee_master	WHERE b.emp_code in (select emp_code from pay_employee_master a where a.REPORTING_TO = '" + Session["LOGIN_ID"].ToString() + "') and pay_add_expenses.status not in ('Draft') group by b.expenses_id order by b.id desc  ", d.con1);
           // MySqlCommand cmd = new MySqlCommand("SELECT b.expenses_id, (SELECT (`type`) FROM `pay_add_expenses` a WHERE `a`.`expenses_id` = `b`.`expenses_id`) AS 'type',(SELECT (`travel_type`) FROM `pay_add_expenses` a WHERE `a`.`expenses_id` = `b`.`expenses_id`) AS 'travel_type',(SELECT  case when `city_type`=1 then 'Inside city' when city_type=2 then 'Outside city' end as 'city_type' FROM `pay_add_expenses` a WHERE `a`.`expenses_id` = `b`.`expenses_id`) AS 'city_type',(select  group_concat(distinct(travel_mode)) from apply_travel_plan a where a.expenses_id = b.expenses_id) as travel_mode,from_designation,(select to_designation from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as to_designation,date_format(from_date,'%d/%m/%Y') as from_date,(select date_format(to_date,'%d/%m/%Y') from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as to_date, (select Add_Description from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as Add_Description,(select status from pay_add_expenses a  where a.Expenses_id = b.expenses_id order by id desc limit 1) as status, b.modified_by,(select Comments from pay_add_expenses a  where a.Expenses_id = b.expenses_id order by id desc limit 1) as Comments, (select  sum(Amount) from pay_add_expenses a  where a.expenses_id = b.expenses_id) as claim_amount, (select  sum(app_amount) from pay_add_expenses a where a.expenses_id = b.expenses_id) as app_amount from apply_travel_plan b inner join pay_add_expenses on b.Expenses_id = pay_add_expenses.Expenses_id ,pay_employee_master	WHERE b.emp_code in (select emp_code from pay_employee_master a where a.REPORTING_TO = '" + Session["LOGIN_ID"].ToString() + "') and pay_add_expenses.status not in ('Draft') group by b.expenses_id order by b.id desc  ", d.con1);
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
        mp1.Show();
    }
    //protected void lnkdelete_Click(object sender, EventArgs e)
    //{
    //    GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
    //    d.operation("delete from apply_travel_plan where expenses_id = '" + grdrow.Cells[0].Text + "'");
    //    load_gridview();
    //}
    //protected void lnkclaimexpense_Click(object sender, EventArgs e)
    //{
    //    GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
    //    Session["EXPENSE_ID"] = grdrow.Cells[0].Text;
    //    mp1.Show();
    //}
    protected void Button4_Click(object sender, EventArgs e)
    {
        load_gridview();
    }
    
    //protected void lnkaddtravelplan_Click(object sender, EventArgs e)
    //{
    //    Session["EXPENSE_ID"] = "";
    //    ModalPopupExtender1.Show();
    //}
    //protected void lnkclaimexpense_Click1(object sender, EventArgs e)
    //{
    //    Session["EXPENSE_ID"] = "";
    //    mp1.Show();
    //}
}