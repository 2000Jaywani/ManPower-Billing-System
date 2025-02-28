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
        try {
            if (e.Row.Cells[7].Text.Equals("Approved"))
            {
                ((LinkButton)e.Row.Cells[10].FindControl("lnkView")).Visible = false;
            }
            else if (e.Row.Cells[7].Text.Equals("Rejected"))
            {
                ((LinkButton)e.Row.Cells[10].FindControl("lnkView")).Visible = false;
            }      
        }
        catch (Exception ex) {
        }

        e.Row.Cells[0].Visible = false;
    }

    private void load_gridview()
    {
        d.con1.Open();
        try
        {
            DataTable dt_id = new DataTable();
         //   MySqlCommand cmd = new MySqlCommand("select b.emp_code, (select emp_name from pay_employee_master c where c.emp_code=b.emp_code) as emp_name, b.expenses_id,travel_mode,from_designation,(select to_designation from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as to_designation, date_format(from_date,'%d/%m/%Y') as from_date,date_format(to_date,'%d/%m/%Y') as to_date, adv_amount, Add_Description,expense_status,modified_by,b.Comments from apply_travel_plan b  where expense_status !='Draft'   and b.emp_code in (select emp_code from pay_employee_master where reporting_to = '" + Session["LOGIN_ID"].ToString() + "') and b.comp_code = '" + Session["comp_code"].ToString() + "'  group by expenses_id order by b.id desc", d.con1);
            // MySqlCommand cmd = new MySqlCommand("SELECT b.emp_code, (SELECT emp_name FROM pay_employee_master c WHERE c.emp_code = b.emp_code) AS 'emp_name', b.expenses_id, travel_mode, from_designation, (SELECT to_designation FROM apply_travel_plan a WHERE a.expenses_id = b.expenses_id ORDER BY id DESC LIMIT 1) AS 'to_designation', DATE_FORMAT(from_date, '%d/%m/%Y') AS 'from_date', DATE_FORMAT(to_date, '%d/%m/%Y') AS 'to_date', adv_amount, Add_Description, CASE WHEN expense_status = 'Pending' THEN (CASE WHEN approval_emp LIKE '%" + Session["LOGIN_ID"].ToString() + "%' THEN 'Submitted' WHEN approved_emp LIKE '%" + Session["LOGIN_ID"].ToString() + "%' THEN 'Approved' END) ELSE expense_status END AS 'expense_status', modified_by, b.Comments FROM apply_travel_plan b WHERE expense_status != 'Draft' AND (LEFT(approval_emp, 6) = '" + Session["LOGIN_ID"].ToString() + "' OR approved_emp LIKE '%" + Session["LOGIN_ID"].ToString() + "%') GROUP BY expenses_id order by id desc", d.con1);
           // MySqlCommand cmd = new MySqlCommand("SELECT b.emp_code, (SELECT emp_name FROM pay_employee_master c WHERE c.emp_code = b.emp_code) AS 'emp_name', b.expenses_id, travel_mode, from_designation, (SELECT to_designation FROM apply_travel_plan a WHERE a.expenses_id = b.expenses_id ORDER BY id DESC LIMIT 1) AS 'to_designation', DATE_FORMAT(from_date, '%d/%m/%Y') AS 'from_date', (select date_format(to_date,'%d/%m/%Y') from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as to_date, (select IFNULL(sum(app_amount),0) as adv_amount from pay_add_expenses a where a.expenses_id = b.expenses_id) as adv_amount, Add_Description, CASE WHEN expense_status = 'Pending' THEN (CASE WHEN approval_emp LIKE '%E00006%' THEN 'Submitted' WHEN approved_emp LIKE '%E00006%' THEN 'Approved' END) ELSE expense_status END AS 'expense_status', modified_by, b.Comments FROM apply_travel_plan b WHERE expense_status != 'Draft' AND (LEFT(approval_emp, 6) = '" + Session["LOGIN_ID"].ToString() + "' OR approved_emp LIKE '%" + Session["LOGIN_ID"].ToString() + "%') GROUP BY expenses_id and  comp_code='"+Session["comp_code"].ToString()+"' order by id desc", d.con1);
            MySqlCommand cmd = new MySqlCommand("SELECT b.emp_code, (SELECT emp_name FROM pay_employee_master c WHERE c.emp_code = b.emp_code) AS 'emp_name', b.expenses_id,CASE WHEN `city_type` = 1 THEN 'Inside City' WHEN `city_type` = 2 THEN 'Outside City' END AS 'City_type', (SELECT GROUP_CONCAT(DISTINCT (`travel_mode`)) FROM `apply_travel_plan` a WHERE `a`.`expenses_id` = `b`.`expenses_id`) AS 'travel_mode',type, from_designation, (SELECT to_designation FROM apply_travel_plan a WHERE a.expenses_id = b.expenses_id ORDER BY id DESC LIMIT 1) AS 'to_designation', DATE_FORMAT(from_date, '%d/%m/%Y') AS 'from_date', (select date_format(to_date,'%d/%m/%Y') from apply_travel_plan a where a.expenses_id = b.expenses_id order by id desc limit 1) as to_date, (select IFNULL(sum(Amount),0) as adv_amount from pay_add_expenses a where a.expenses_id = b.expenses_id) as adv_amount, Add_Description, CASE WHEN expense_status = 'Pending' THEN (CASE WHEN approval_emp LIKE '%" + Session["LOGIN_ID"].ToString() + "%' THEN 'Submitted' WHEN approved_emp LIKE '%" + Session["LOGIN_ID"].ToString() + "%' THEN 'Approved' END) ELSE expense_status END AS 'expense_status', modified_by, b.Comments FROM apply_travel_plan b WHERE expense_status != 'Draft' and comp_code='" + Session["comp_code"].ToString() + "' and `approval_emp` is not null GROUP BY expenses_id  order by id desc", d.con1); //and approval_emp LIKE '%" + Session["LOGIN_ID"].ToString() + "%' 
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
        ModalPopupExtender1.Show();
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