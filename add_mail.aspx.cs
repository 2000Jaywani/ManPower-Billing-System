using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class add_mail : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL gv_con = new DAL();

    string client_code;
    string state;
    protected void Page_Load(object sender, EventArgs e)
    {
        client_code = Session["add_email_client"].ToString();
        state = Session["add_email_state"].ToString();


        if (!IsPostBack)
        {
            client_code = Session["add_email_client"].ToString();
            state = Session["add_email_state"].ToString();

            fill_gv();
            btndelete.Visible = false;
            btn_update.Visible = false;
            ddl_state_SelectedIndexChanged(null, null);

        }

    }

    protected void fill_gv()
    {
        client_code = Session["add_email_client"].ToString();
        state = Session["add_email_state"].ToString();
        try
        {
            d.con.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT    distinct(`pay_branch_mail_details`.`Id`),    pay_unit_master.`unit_name` AS 'unit_code',  `head_name`,  `head_email_id`,  `head_type`,cc_emailid  FROM  `pay_branch_mail_details`  INNER JOIN `pay_client_master` ON `pay_branch_mail_details`.`client_code` = `pay_client_master`.`client_code` AND `pay_branch_mail_details`.`comp_code` = `pay_client_master`.`comp_code`  INNER JOIN `pay_unit_master` ON `pay_branch_mail_details`.`unit_code` = `pay_unit_master`.`unit_code` AND `pay_branch_mail_details`.`comp_code` = `pay_unit_master`.`comp_code`  INNER JOIN `pay_client_state_role_grade` ON `pay_branch_mail_details`.`client_code` = `pay_unit_master`.`client_code`  WHERE  `pay_branch_mail_details`.`comp_code` = '" + Session["COMP_CODE"].ToString() + "' AND (pay_client_state_role_grade.emp_code IN (" + Session["REPORTING_EMP_SERIES"].ToString() + ") OR pay_client_state_role_grade.emp_code = '" + Session["LOGIN_ID"].ToString() + "') and pay_branch_mail_details.client_code='" + client_code + "' and pay_branch_mail_details.state='" + state + "'and branch_status=0 ", d.con);
            DataSet ds = new DataSet();
            //   cmd.Fill(ds);
            MySqlDataAdapter adp_vend_gv = new MySqlDataAdapter(cmd);
            adp_vend_gv.Fill(ds);
            AddMailGridView.DataSource = ds;
            AddMailGridView.DataBind();
            d.con.Close();
        }
        catch (Exception ex) { throw ex; }
        finally { d.con.Close(); }


    }
    protected void AddMailGridView_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            gv_con.con.Open();
            string id = AddMailGridView.SelectedRow.Cells[0].Text;
            MySqlCommand cmd = new MySqlCommand("select Id,unit_code,head_name,head_email_id,head_type,mobileno,cc_emailid from pay_branch_mail_details where Id='" + id + "' ", gv_con.con);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                txt_id.Text = dr.GetValue(0).ToString();


                ddl_unitcode.SelectedValue = dr.GetValue(1).ToString();
                txt_head.Text = dr.GetValue(2).ToString();
                txt_email.Text = dr.GetValue(3).ToString();
                ddl_head_type.SelectedValue = dr.GetValue(4).ToString();
                txt_mobileno.Text = dr.GetValue(5).ToString();
                txt_cc_emailid.Text = dr.GetValue(6).ToString();
            }
            dr.Close();
            gv_con.con.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

            gv_con.con.Close();
            btndelete.Visible = true;
            btn_update.Visible = true;
            btn_save.Visible = false;
        }
    }

    protected void btnupdate_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        int result = d.operation("update   pay_branch_mail_details set CLIENT_CODE='" + client_code + "',unit_code='" + ddl_unitcode.Text + "',head_type='" + ddl_head_type.SelectedValue + "',head_name='" + txt_head.Text + "',head_email_id='" + txt_email.Text + "',state='" + state + "',mobileno='" + txt_mobileno.Text + "',cc_emailid='" + txt_cc_emailid.Text + "'  WHERE comp_code='" + Session["comp_code"].ToString() + "' AND Id = '" + txt_id.Text + "'");
        if (ddl_head_type.SelectedValue == "Operation_Head")
        {
            d.operation("update pay_unit_master set `OperationHead_Name`='" + txt_head.Text + "',`OperationHead_Mobileno`='" + txt_mobileno.Text + "',`OperationHead_EmailId`='" + txt_email.Text + "' where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "' and unit_code='" + ddl_unitcode.SelectedValue + "' and `STATE_NAME`='" + state + "'");

        }
        if (ddl_head_type.SelectedValue == "Location_Head")
        {
            d.operation("update pay_unit_master set `LocationHead_Name`='" + txt_head.Text + "',`LocationHead_mobileno`='" + txt_mobileno.Text + "',`LocationHead_Emailid`='" + txt_email.Text + "' where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "'and unit_code='" + ddl_unitcode.SelectedValue + "' and `STATE_NAME`='" + state + "'");

        }
        if (ddl_head_type.SelectedValue == "Finance_Head")
        {
            d.operation("update pay_unit_master set `FinanceHead_Name`='" + txt_head.Text + "',`FinanceHead_Mobileno`='" + txt_mobileno.Text + "',`FinanceHead_EmailId`='" + txt_email.Text + "' where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "' and unit_code='" + ddl_unitcode.SelectedValue + "' and `STATE_NAME`='" + state + "'");

        }
        if (ddl_head_type.SelectedValue == "Admin_Head")
        {
            d.operation("update pay_unit_master set `adminhead_name`='" + txt_head.Text + "',`adminhead_mobile`='" + txt_mobileno.Text + "',`adminhead_email`='" + txt_email.Text + "' where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "'and unit_code='" + ddl_unitcode.SelectedValue + "' and `STATE_NAME`='" + state + "'");

        }
        if (ddl_head_type.SelectedValue == "Other_Head")
        {
            d.operation("update pay_unit_master set `OtherHead_Name`='" + txt_head.Text + "',`OtherHead_Monileno`='" + txt_mobileno.Text + "',`OtherHead_Emailid`='" + txt_email.Text + "' where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "'and unit_code='" + ddl_unitcode.SelectedValue + "' and `STATE_NAME`='" + state + "'");

        }

        if (result > 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record Update successfully!!');", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record Update failed!!');", true);
        }
        fill_gv();
        txt_clear();
        btn_save.Visible = true;
        btn_update.Visible = false;
        btndelete.Visible = false;
    }

    protected void btndelete_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        int result = d.operation("DELETE FROM pay_branch_mail_details WHERE comp_code='" + Session["comp_code"].ToString() + "' AND Id = '" + txt_id.Text + "'");
        if (result > 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record Deleted successfully!!');", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record Deleted failed!!');", true);
        }
        fill_gv();
        txt_clear();
        btn_save.Visible = true;
        btn_update.Visible = false;
        btndelete.Visible = false;
    }
    protected void DesignationGridView_RowDataBound(object sender, GridViewRowEventArgs e)
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
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.AddMailGridView, "Select$" + e.Row.RowIndex);
        }

        e.Row.Cells[0].Visible = false;
    }
    protected void txt_clear()
    {

        ddl_unitcode.SelectedValue = "Select";
        ddl_head_type.SelectedValue = "Select";

        txt_head.Text = "";
        txt_email.Text = "";
        txt_mobileno.Text = "";
        txt_cc_emailid.Text = "";


    }


    protected void ddl_state_SelectedIndexChanged(object sender, EventArgs e)
    {

        ddl_unitcode.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT  distinct(pay_unit_master.`unit_code`),CONCAT((SELECT DISTINCT (pay_state_master.`STATE_CODE`) FROM `pay_state_master` WHERE pay_state_master.`STATE_NAME` = `pay_unit_master`.`STATE_NAME`), '_', `UNIT_CITY`, '_', `UNIT_ADD1`, '_', `UNIT_NAME`) AS 'UNIT_NAME'  FROM  `pay_unit_master`  INNER JOIN `pay_client_state_role_grade` ON `pay_client_state_role_grade`.`COMP_CODE` = `pay_unit_master`.`COMP_CODE` AND `pay_client_state_role_grade`.`client_code` = `pay_unit_master`.`client_code` AND `pay_client_state_role_grade`.`state_name` = `pay_unit_master`.`state_name`  And pay_client_state_role_grade.unit_code = pay_unit_master.unit_code  WHERE  `pay_client_state_role_grade`.`comp_code` = '" + Session["COMP_CODE"].ToString() + "' AND `pay_client_state_role_grade`.`client_code` = '" + client_code + "' AND `pay_client_state_role_grade`.`state_name` = '" + state + "' AND (pay_client_state_role_grade.emp_code IN (" + Session["REPORTING_EMP_SERIES"].ToString() + ") OR pay_client_state_role_grade.emp_code = '" + Session["LOGIN_ID"].ToString() + "') and branch_status = 0 ORDER BY pay_client_state_role_grade.`state_name`", d.con);
        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_unitcode.DataSource = dt_item;
                ddl_unitcode.DataTextField = dt_item.Columns[1].ToString();
                ddl_unitcode.DataValueField = dt_item.Columns[0].ToString();
                ddl_unitcode.DataBind();
            }
            dt_item.Dispose();
            d.con.Close();
            ddl_unitcode.Items.Insert(0, "Select");
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
        }
        // }

    }

    protected void btn_save_Click(object sender, EventArgs e)
    {
        try { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CallMyFunction", "unblock()", true); }
        catch { }
        int i = d.operation("INSERT INTO pay_branch_mail_details (COMP_CODE,CLIENT_CODE,unit_code,head_type,head_name,head_email_id,state,mobileno,cc_emailid)VALUES('" + Session["COMP_CODE"].ToString() + "','" + client_code + "','" + ddl_unitcode.Text + "','" + ddl_head_type.SelectedValue + "','" + txt_head.Text + "','" + txt_email.Text + "','" + state + "','" + txt_mobileno.Text + "','" + txt_cc_emailid.Text + "')");
        if (ddl_head_type.SelectedValue == "Operation_Head")
        {
            d.operation("update pay_unit_master set `OperationHead_Name`='" + txt_head.Text + "',`OperationHead_Mobileno`='" + txt_mobileno.Text + "',`OperationHead_EmailId`='" + txt_email.Text + "' where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "' and unit_code='" + ddl_unitcode.SelectedValue + "' and `STATE_NAME`='" + state + "'");

        }
        if (ddl_head_type.SelectedValue == "Location_Head")
        {
            d.operation("update pay_unit_master set `LocationHead_Name`='" + txt_head.Text + "',`LocationHead_mobileno`='" + txt_mobileno.Text + "',`LocationHead_Emailid`='" + txt_email.Text + "' where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "'and unit_code='" + ddl_unitcode.SelectedValue + "' and `STATE_NAME`='" + state + "'");

        }
        if (ddl_head_type.SelectedValue == "Finance_Head")
        {
            d.operation("update pay_unit_master set `FinanceHead_Name`='" + txt_head.Text + "',`FinanceHead_Mobileno`='" + txt_mobileno.Text + "',`FinanceHead_EmailId`='" + txt_email.Text + "' where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "' and unit_code='" + ddl_unitcode.SelectedValue + "' and `STATE_NAME`='" + state + "'");

        }
        if (ddl_head_type.SelectedValue == "Admin_Head")
        {
            d.operation("update pay_unit_master set `adminhead_name`='" + txt_head.Text + "',`adminhead_mobile`='" + txt_mobileno.Text + "',`adminhead_email`='" + txt_email.Text + "' where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "'and unit_code='" + ddl_unitcode.SelectedValue + "' and `STATE_NAME`='" + state + "'");

        }
        if (ddl_head_type.SelectedValue == "Other_Head")
        {
            d.operation("update pay_unit_master set `OtherHead_Name`='" + txt_head.Text + "',`OtherHead_Monileno`='" + txt_mobileno.Text + "',`OtherHead_Emailid`='" + txt_email.Text + "' where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + client_code + "'and unit_code='" + ddl_unitcode.SelectedValue + "' and `STATE_NAME`='" + state + "'");

        }
        if (i > 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record added successfully!!')", true);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record added Fail!!')", true);
        }
        fill_gv();
        txt_clear();
    }
    protected void AddMailGridView_PreRender(object sender, EventArgs e)
    {
        try
        {
            AddMailGridView.UseAccessibleHeader = false;
            AddMailGridView.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
}