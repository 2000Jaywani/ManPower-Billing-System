using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class android_atten_logs_history : System.Web.UI.Page
{
    DAL d = new DAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (!Session["Emp_code"].ToString().Equals(""))
            {
                load_sub_grid(Session["Emp_code"].ToString(), Session["client_code"].ToString(), Session["state_name"].ToString(), Session["unit_code"].ToString(), Session["start_date"].ToString(), Session["end_date"].ToString(), Session["Conuter"].ToString());
            }
        }
    }
    protected void load_sub_grid(string emp_code, string client_code,string state_name,string unit_code,string start_date,string end_date ,string counter)
    {
       
            d.con.Open();
            try
            {
                MySqlDataAdapter dscmd;
                if (counter == "1")
                {
                    dscmd = new MySqlDataAdapter("Select (select client_name from pay_client_master where client_code='" + client_code + "' and comp_code='" + Session["comp_code"].ToString() + "') as client_name , state_name,(SELECT DISTINCT(`pay_unit_master`.`unit_name`) FROM `pay_unit_master` WHERE `unit_code` = '" + unit_code + "' AND comp_code='" + Session["COMP_CODE"].ToString() + "')as unit_name,pay_android_attendance_logs.EMP_CODE,UNIT_LATITUDE,UNIT_LONGTUTDE,EMP_LATITUDE,EMP_LONGTUTDE,DISTANCES,ADDRESS,(SELECT CASE pay_employee_master.`Employee_type` WHEN 'Reliever' THEN CONCAT(pay_employee_master.`emp_name`, '-', 'Reliever') ELSE pay_employee_master.`emp_name` END) AS 'EMP_NAME',date_format(`Date_Time`,'%d/%m/%Y %h:%i:%s %p') as Date_Time,date_format(`attendances_intime`,'%d/%m/%Y %h:%i:%s %p') as attendances_intime, date_format(`attendances_outtime`,'%d/%m/%Y %h:%i:%s %p') as attendances_outtime ,date_format(`camera_intime`,'%d/%m/%Y %h:%i:%s %p') as camera_intime,date_format(`camera_outtime`,'%d/%m/%Y %h:%i:%s %p') as camera_outtime,Camera_intime_images,Camera_outtime_images,Attendances_intime_images,Attendances_outtime_images from pay_android_attendance_logs inner join pay_employee_master on pay_employee_master.`EMP_CODE`=pay_android_attendance_logs.emp_code    INNER JOIN `pay_unit_master` ON `pay_android_attendance_logs`.`comp_code` = `pay_unit_master`.`comp_code` AND `pay_android_attendance_logs`.`unit_code` = `pay_unit_master`.`unit_code` where pay_android_attendance_logs.UNIT_CODE = '" + unit_code + "' AND pay_android_attendance_logs.comp_code = '" + Session["comp_code"].ToString() + "' and  pay_android_attendance_logs.emp_code= '"+emp_code+"' and date_time between str_to_date('" + start_date + "','%d/%m/%Y') and str_to_date('" + end_date + " 23:59:59','%d/%m/%Y %H:%i:%s') order by date_time desc limit 200", d.con);
                    DataSet ds = new DataSet();
                    dscmd.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        GradeGridView.DataSource = ds;
                        GradeGridView.DataBind();

                    }
                    else
                    {
                        GradeGridView.DataSource = null;
                        GradeGridView.DataBind();
                    }
                }
                else if (counter == "2")
                {
                    dscmd = new MySqlDataAdapter("SELECT `pay_geolocation_address`.`id`,state_name,pay_geolocation_address.unit_code,pay_geolocation_address.client_code,( SELECT CASE pay_employee_master.`Employee_type` WHEN 'Reliever' THEN CONCAT(pay_employee_master.`emp_name`, '-', 'Reliever') ELSE pay_employee_master.`emp_name` END ) AS 'emp_code', `cur_address`, `cur_latitude`, `cur_longtitude`, `cur_date` FROM `pay_geolocation_address` INNER JOIN `pay_employee_master` ON `pay_geolocation_address`.`EMP_CODE` = `pay_employee_master`.`emp_code`  INNER JOIN `pay_unit_master` ON `pay_geolocation_address`.`comp_code` = `pay_unit_master`.`comp_code` AND `pay_geolocation_address`.`unit_code` = `pay_unit_master`.`unit_code`  WHERE `pay_geolocation_address`.`comp_code` = '" + Session["comp_code"].ToString() + "' and pay_geolocation_address.client_code='" + client_code + "' and pay_geolocation_address.unit_code='" + unit_code + "'  and pay_geolocation_address.`cur_date` BETWEEN   STR_TO_DATE('" + start_date + "', '%d/%m/%Y') and  STR_TO_DATE('" + end_date + " 23:59:59', '%d/%m/%Y %H:%i:%s')    ORDER BY pay_geolocation_address.`cur_date` DESC LIMIT 200", d.con);
                    DataSet ds = new DataSet();
                    dscmd.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        grd_current_location.DataSource = ds;
                        grd_current_location.DataBind();

                    }
                    else
                    {
                        grd_current_location.DataSource = null;
                        grd_current_location.DataBind();

                    }
                }
                d.con.Close();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                d.con.Close();
            }
        }

    protected void GradeGridView_PreRender(object sender, EventArgs e)
    {
        try
        {
            GradeGridView.UseAccessibleHeader = false;
            GradeGridView.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }

    }
    protected void GradeGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (DataRowView)e.Row.DataItem;
            string imageUrl, imageUrl2 = "";
            if (dr["Attendances_intime_images"].ToString() != "")
            {
                imageUrl = "~/Captures/attendance_images/" + dr["Attendances_intime_images"];
                (e.Row.FindControl("Camera_Image1") as System.Web.UI.WebControls.Image).ImageUrl = imageUrl;


            }
            if (dr["Attendances_outtime_images"].ToString() != "")
            {

                imageUrl2 = "~/Captures/attendance_images/" + dr["Attendances_outtime_images"];
                (e.Row.FindControl("Camera_Image2") as System.Web.UI.WebControls.Image).ImageUrl = imageUrl2;


            }
            if (dr["Camera_intime_images"].ToString() != "")
            {
                imageUrl = "~/Captures/attendance_images/" + dr["Attendances_intime_images"];
                (e.Row.FindControl("Camera_Image1") as System.Web.UI.WebControls.Image).ImageUrl = imageUrl;

            }
            if (dr["Camera_outtime_images"].ToString() != "")
            {
                imageUrl2 = "~/Captures/attendance_images/" + dr["Attendances_outtime_images"];
                (e.Row.FindControl("Camera_Image2") as System.Web.UI.WebControls.Image).ImageUrl = imageUrl2;

            }
        }

    }
    protected void grd_current_location_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grd_current_location, "Select$" + e.Row.RowIndex);

        }
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    DataRowView dr = (DataRowView)e.Row.DataItem;

        //    string latitude = dr["cur_latitude"].ToString();
        //    string longtitude = dr["cur_longtitude"].ToString();
        //    string address = dr["cur_address"].ToString();
        //    //(e.Row.FindControl("Camera_Image3") as Image).ImageUrl = imageUrl;
        //    Session["MAP_UNIT_CODE"] = "U003";
        //    Response.Redirect("location_map.aspx");

        //}
    }
    protected void Location_SelectedIndexChanged(object sender, EventArgs e)
    {
        // System.Web.UI.WebControls.Label lbl_EMP_code = (System.Web.UI.WebControls.Label)grd_current_location.SelectedRow.FindControl("id");
        string id_no = grd_current_location.SelectedRow.Cells[1].Text;

        //Session["UNIT_NO"] = ddlunitselect.SelectedValue.ToString();
        Session["MAP_ID"] = id_no;
        Session["MAP_ADDRESS"] = grd_current_location.SelectedRow.Cells[6].Text;
        Session["MAP_LONGITUDE"] = grd_current_location.SelectedRow.Cells[4].Text;
        Session["MAP_LATTITUDE"] = grd_current_location.SelectedRow.Cells[3].Text;
        Session["MAP_AREA"] = "100";


        Response.Redirect("location_map.aspx");

    }
    protected void grd_location_PreRender(object sender, EventArgs e)
    {
        try
        {
            grd_current_location.UseAccessibleHeader = false;
            grd_current_location.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
}
