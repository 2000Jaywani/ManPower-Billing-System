using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Backup_Page : System.Web.UI.Page
{
    DAL d1 = new DAL();
    DAL d = new DAL();
   
    string MySql = "";
    string BackupData = "";
  
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

         //  for_reset_gv_search.Visible = false; // gv visibility 15-05-2021 komal
            client_data(); // client select 15-05-2021 komal
        
        }


        if (Session["comp_code"] == null || Session["comp_code"].ToString() == "")
        {
            Response.Redirect("Login_Page.aspx");
        }		
        if (d1.getaccess(Session["ROLE"].ToString(), "System Backup",Session["COMP_CODE"].ToString()) == "I")
        {
            Response.Redirect("unauthorised_access.aspx");
        }
        else if (d1.getaccess(Session["ROLE"].ToString(), "System Backup",Session["COMP_CODE"].ToString()) == "R")
        {
            btn_backup.Visible = false;
            //btn_edit.Visible = false;
            //btn_add.Visible = false;
            //btnexporttoexcelgrade.Visible = false;
        }
        else if (d1.getaccess(Session["ROLE"].ToString(), "System Backup",Session["COMP_CODE"].ToString()) == "U")
        {
            btn_backup.Visible = false;
            //btn_add.Visible = false;
            //btnexporttoexcelgrade.Visible = false;
        }
        else if (d1.getaccess(Session["ROLE"].ToString(), "System Backup", Session["COMP_CODE"].ToString()) == "C")
        {
            btn_backup.Visible = false;
            //btnexporttoexcelgrade.Visible = false;
        }
       // d1.con1.Close();
    }

    protected void btn_BackUp_Click(object sender, EventArgs e) 
    {
        try
        {
            var shell = PowerShell.Create();

            MySql = Server.MapPath("~\\Images\\backup.zip");
            string db_backUp = Server.MapPath("~\\EMP_Images\\celtpayroll.sql");

            BackupData = "mysqldump -u " + txt_user_id.Text + " -p " + txt_password.Text + " celtpayroll > " + db_backUp + "";
            // BackupData = "mysqldump -u root -proot celtpay > " + MySql + "";
            shell.Commands.AddScript(BackupData);
            shell.Invoke();
            if (File.Exists(MySql))
            {
                File.Delete(MySql);
            }
           //// ZipFile.CreateFromDirectory(Server.MapPath("~\\EMP_Images"), MySql);


            Download_File(MySql);
            File.Delete(MySql);
            string backup = "BackUp Done Successfully!";
            d1.logs(backup);
        }
        catch(Exception error_backup)
        {
            d1.logs(error_backup.Message);
        }

     //   MessageBox.Show("Successfully Database Backup Completed");
    }

    private void Download_File(string FilePath)
    {
        Response.ContentType = ContentType;
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
        Response.WriteFile(FilePath);
        Response.End();
    }

    protected void btn_PasswordGenerat_Click(object sender, EventArgs e)
    {

        string left_date = d1.getsinglestring("select left_date from pay_employee_master where emp_code='" + txt_emp_login_id.Text.ToString() + "'");

        if (!left_date.Equals(""))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Employee already left for system !!!')", true);
           // return;
        }
        else {
            string newPassword = GetSha256FromString(txtPassword.Text.ToString());

            txt_generatesha256.Text = newPassword;
            txt_orignalpassword.Text = txtPassword.Text.ToString();

            int result = d1.operation("update pay_user_master set user_password='" + newPassword + "',flag='A',password_changed_date = date(now()) where Login_id='" + txt_emp_login_id.Text.ToString() + "'");

            txtPassword.Text = "";
            txt_emp_login_id.Text = "";
            txt_orignalpassword.Text = "";
            txt_generatesha256.Text = "";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Employee password change successfully !!!')", true);

        }

        
    }

    public static string GetSha256FromString(string strData)
    {
        var message = Encoding.ASCII.GetBytes(strData);
        SHA256Managed hashString = new SHA256Managed();
        string hex = "";

        var hashValue = hashString.ComputeHash(message);
        foreach (byte x in hashValue)
        {
            hex += String.Format("{0:x2}", x);
        }
        return hex;
    }

    protected void btn_Close_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home.aspx");
    }


    

    // password reset code komal 15-05-2021


    protected void client_data() 
    {
        ddl_client.Items.Clear();

         System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlDataAdapter cmd_item = new MySqlDataAdapter("Select CASE WHEN  client_code  = 'BALIK HK' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BALIC SG' THEN CONCAT( client_name , ' SG') WHEN  client_code  = 'BAG' THEN CONCAT( client_name , ' HK') WHEN  client_code  = 'BG' THEN CONCAT( client_name , ' SG') ELSE  client_name  END AS 'client_name', client_code from pay_client_master where comp_code='" + Session["comp_code"] + "' and client_active_close='0' ORDER BY client_code", d.con);
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
            finally { d.con.Close(); }
    
    }

    protected void reset_pass_gv()
    {
        gv_reset.Visible = true;

        try
        {

            System.Data.DataTable dt = new System.Data.DataTable();
            gv_pass_reset.DataSource = null;
            gv_pass_reset.DataBind();

            d.con.Open();
            MySqlCommand cmd = null;
            cmd = new MySqlCommand("select  ROLE,LOGIN_ID,USER_NAME from pay_user_master where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and (ROLE = 'zone' || ROLE = 'HO' || ROLE = 'Region' || ROLE = 'UNIT' || ROLE = 'cluster' || LOGIN_ID like 'RM%' ) ", d.con);
            MySqlDataAdapter dt_item = new MySqlDataAdapter(cmd);
            dt_item.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                gv_pass_reset.DataSource = dt;
                gv_pass_reset.DataBind();
            }
            d.con.Close();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            d.con.Close();
        }
    }


    //pass reset code komal
    protected void ddl_client_SelectedIndexChanged(object sender, EventArgs e)
    {
       // for_reset_gv_search.Visible = true;
        reset_pass_gv();
    }
    protected void btn_reset_Click(object sender, EventArgs e)
    {
        try 
        {
            d.con.Open();
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
            string id = grdrow.Cells[3].Text;


            TextBox txt_new_pass = (TextBox)grdrow.FindControl("txt_new_pass");
            string new_pass_txt = (txt_new_pass.Text);

            TextBox txt_con_pass = (TextBox)grdrow.FindControl("txt_conf_pass");
            string pass_txt = (txt_con_pass.Text);


            if (new_pass_txt == "")
            { ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter New Password Password... !!!');", true); return; }


            if (pass_txt == "") 
            { ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Enter Conform Password... !!!');", true); return; }


            string login_id = grdrow.Cells[3].Text;

            int res = d.operation("update pay_user_master set USER_PASSWORD = '" + GetSha256FromString(pass_txt) + "' where comp_code = '" + Session["comp_code"].ToString() + "' and client_code = '" + ddl_client.SelectedValue + "' and LOGIN_ID = '" + login_id + "' ");

    if (res > 0)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Password Reset Successfully... !!!');", true);
    }
    else
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert(' Password Reset Failed... !!!');", true);
    }
        
        
        }
        catch (Exception ex) { throw ex; }
        finally{}

    }
    protected void gv_pass_reset_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        for (int i = 0; i < e.Row.Cells.Count; i++)
        {
            if (e.Row.Cells[i].Text == "&nbsp;")
            {
                e.Row.Cells[i].Text = "";
            }
        }
    }
    protected void gv_pass_reset_PreRender(object sender, EventArgs e)
    {

        try
        {
            gv_pass_reset.UseAccessibleHeader = false;
            gv_pass_reset.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
        catch { }
    }
   
}
