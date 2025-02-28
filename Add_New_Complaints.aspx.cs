using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;

public partial class Add_New_Complaints : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            add_catagroy();
            Travelling_Gridview1();
        }
    }

    protected void btn_save_Click(object sender, EventArgs e)
    {
        int res;

        res = d.operation("insert into pay_add_new_complaints(name,priority,comp_category)Values('" + txt_name.Text + "','" + dll_Priority.SelectedValue + "','" + ddl_complaint.SelectedValue + "')");
      if (res > 0)
      {
          ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record Added Successfully... !!');", true);
          Travelling_Gridview1();
      }
      else
      {
          ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record Added Faill.... !!');", true);
      }
      txt_name.Text = "";
      dll_Priority.SelectedValue = "Select";
      ddl_complaint.SelectedValue = "Select";

    }
    //protected void GradeGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    for (int i = 0; i < e.Row.Cells.Count; i++)
    //    {
    //        if (e.Row.Cells[i].Text == "&nbsp;")
    //        {
    //            e.Row.Cells[i].Text = "";
    //        }
    //    }
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {
    //        e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
    //        e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
    //        e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.Grid_gst_rate, "Select$" + e.Row.RowIndex);
    //    }
    //}
    //protected void GradeGridView_PreRender(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        Grid_gst_rate.UseAccessibleHeader = false;
    //        Grid_gst_rate.HeaderRow.TableSection = TableRowSection.TableHeader;
    //    }
    //    catch { }//vinod dont apply catch
    //}
    protected void lnkbtn_removeitem_Click(object sender, EventArgs e)
    {
        LinkButton lb = (LinkButton)sender;
        GridViewRow row = (GridViewRow)lb.NamingContainer;
        int rowID = row.RowIndex;
        if (ViewState["Travelling_grid"] != null)
        {
            DataTable dt = (DataTable)ViewState["Travelling_grid"];
            if (dt.Rows.Count >= 1)
            {
                if (row.RowIndex < dt.Rows.Count)
                {
                    d.operation("Delete  from pay_add_new_complaints where  Id = '" + dt.Rows[rowID][0] + "'");
                    dt.Rows.Remove(dt.Rows[rowID]);
                }
            }
            ViewState["Travelling_grid"] = dt;
            Grid_gst_rate.DataSource = dt;
            Grid_gst_rate.DataBind();

            Grid_gst_rate.DataSource = dt;
            //Grid_gst_rate.DataTextField = dt.Columns[0].ToString();
            //Grid_gst_rate.DataValueField = dt.Columns[0].ToString();
            Grid_gst_rate.DataBind();
     

        }
    }
    protected void Travelling_Gridview1()
    {

        try
        {
            d1.con.Open();
            MySqlCommand cmd_1 = new MySqlCommand("select ID, name,priority,comp_category from pay_add_new_complaints", d1.con);
            MySqlDataAdapter cad1 = new MySqlDataAdapter(cmd_1);
            DataTable DS1 = new DataTable();
            cad1.Fill(DS1);
            Grid_gst_rate.DataSource = DS1;
            ViewState["Travelling_grid"] = DS1;
            Grid_gst_rate.DataBind();

            cad1.Dispose();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            d1.con.Close();


        }
    }

    //protected void lnkbtn_removeitem_Click(object sender, EventArgs e)
    //{
    //    LinkButton lb = (LinkButton)sender;
    //    GridViewRow row = (GridViewRow)lb.NamingContainer;
    //    //int request_id = (int)Grid_gst_rate.DataKeys[row.RowIndex].Value;
    //    int rowID = row.RowIndex;
    //    if (ViewState["request_id"] != null)
    //    {
    //        DataTable dt = (DataTable)ViewState["dt"];
    //        if (dt.Rows.Count >= 1)
    //        {
    //            if (row.RowIndex < dt.Rows.Count)
    //            {
    //                dt.Rows.Remove(dt.Rows[rowID]);
    //            }
    //        }
    //        ViewState["ds"] = dt;
    //        Grid_gst_rate.DataSource = dt;
    //        Grid_gst_rate.DataBind();
    //    }
    //}
    protected void Grid_gst_rate_PreRender(object sender, EventArgs e)
    {
        try
        {
            Grid_gst_rate.UseAccessibleHeader = false;
            Grid_gst_rate.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
    protected void add_catagroy()
    {
        ddl_complaint.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT  category FROM `pay_complaint_category` ", d.con);
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_complaint.DataSource = dt_item;
                // ddl_category.DataTextField = dt_item.Columns[1].ToString();
                ddl_complaint.DataValueField = dt_item.Columns[0].ToString();
                ddl_complaint.DataBind();
            }
            dt_item.Dispose();

            d.con.Close();
            ddl_complaint.Items.Insert(0, "Select");
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
    }
}