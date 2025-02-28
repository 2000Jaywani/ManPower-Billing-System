using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Approval_reject_reason : System.Web.UI.Page
{
    DAL d = new DAL();
    double a = 0, c = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["COMP_CODE"] == null || Session["COMP_CODE"].ToString() == "")
        {
            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "window.close();", true);
        }

        if (!IsPostBack)
        {
            if (!Session["Approval_Id"].ToString().Equals(""))
            {
                load_sub_grid(Session["Approval_Id"].ToString());
            }
            else
            {
                SearchGridView.DataSource = null;
                SearchGridView.DataBind();
            }
        }
    }
   
   

   



    private void load_sub_grid(string request_Id_edit)
    {
        try
        {
            d.con.Open();
            DataSet ds = new DataSet();
            // MySqlDataAdapter dr = new MySqlDataAdapter("select Id,services,pay_service_master.location,priority,pay_service_master.status,additional_comment,EMP_NAME as forword_to,documents from  pay_service_master inner join pay_employee_master on pay_service_master.comp_code=pay_employee_master.comp_code and  pay_service_master.forword_to=pay_employee_master.emp_code   where pay_service_master.comp_code='" + Session["comp_code"].ToString() + "' and pay_service_master.unit_code='" + Session["unit_code"].ToString() + "' order by Id  ", d.con);
            MySqlDataAdapter dr = new MySqlDataAdapter("SELECT ID,COMP_CODE,COMPANY_NAME,CITY,STATE FROM pay_company_master_approval where id = '"+request_Id_edit+"'", d.con);
            dr.Fill(ds);
            SearchGridView.DataSource = ds.Tables[0];
            SearchGridView.DataBind();
            d.con.Close();
            SearchGridView.Visible = true;
        }
        catch (Exception ex)
        { throw ex; }
        finally
        {
            d.con.Close();

        }
    }


    protected void btn_reject_Click(object sender, EventArgs e)
    {
       try
       {
           d.operation("Insert into approval_status (comp_code,status,comment,created_by) values((select comp_code from pay_company_master_approval where id = '"+Session["Approval_Id"].ToString()+"'),'Rejected', '"+ txt_comments.Text + "','"+Session["LOGIN_ID"].ToString()+"')");
           d.operation("delete from pay_company_master_approval WHERE id=" + Session["Approval_Id"].ToString());
        }
        catch (Exception ee)
        {
            throw ee;
        }
        finally
        {
            btn_reject.Visible = false;
            load_sub_grid(Session["Approval_Id"].ToString());
        }

    }
  
    protected void SearchGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }

        //e.Row.Cells[1].Visible = false;
        //e.Row.Cells[2].Visible = false;
    }
}