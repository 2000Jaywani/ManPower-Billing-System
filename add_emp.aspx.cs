using System;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Text;
using System.Security.Cryptography;

public partial class add_emp : System.Web.UI.Page
{
    DAL d = new DAL();
    DAL d1 = new DAL();

    protected int result = 0;//vikas
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {

            // client_code();//vikas
            btnnew_Click();
            ddl_fill_designation();
            stete_code();

        }

    }

    protected void stete_code()
    {
        try
        {

            System.Data.DataTable dt_item = new System.Data.DataTable();
            MySqlCommand cmd = new MySqlCommand("SELECT STATE_NAME from pay_unit_master where  comp_code = '" + Session["COMP_CODE"].ToString() + "' and CLIENT_CODE='" + Session["client_code"].ToString() + "' and UNIT_CODE='" + Session["unit_code_addemp"].ToString() + "'", d.con);
            d.con.Open();
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {

                txt_state.Text = dr.GetValue(0).ToString();
            }

        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();

        }

    }
    protected void ddl_fill_designation()
    {
        ddl_grade.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        //    MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT GRADE_CODE, CONCAT(GRADE_DESC,'-',Working_Hours) as GRADE_DESC FROM pay_designation_count  WHERE comp_code='" + Session["comp_code"].ToString() + "'", d.con);
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT DISTINCT(Select grade_code from pay_grade_master where grade_desc = pay_designation_count.designation and comp_code = '" + Session["COMP_CODE"].ToString() + "'),DESIGNATION from pay_designation_count WHERE comp_code ='" + Session["comp_code"].ToString() + "' and CLIENT_CODE='" + Session["client_code"].ToString() + "' ", d.con);

        d.con.Open();
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                ddl_grade.DataSource = dt_item;
                ddl_grade.DataTextField = dt_item.Columns[1].ToString();
                ddl_grade.DataValueField = dt_item.Columns[0].ToString();
                ddl_grade.DataBind();

            }
            dt_item.Dispose();
            d.con.Close();
            ddl_grade.Items.Insert(0, "Select");
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
            // rahul comment btnnew_Click();

        }
    }
    private Boolean chkcount()
    {
        if (ddl_employee_type.SelectedValue == "Reliever")
        {
            string Perment_emp_count = d.getsinglestring("Select sum(1) from pay_employee_master where CLIENT_CODE='" + Session["client_code"].ToString() + "' and comp_code='" + Session["comp_code"].ToString() + "' AND   UNIT_CODE='" + Session["unit_code_addemp"].ToString() + "' AND pay_employee_master.Employee_type = 'Permanent'   and GRADE_CODE ='" + ddl_grade.SelectedValue + "' and (pay_employee_master.LEFT_REASON='' || pay_employee_master.LEFT_REASON is null)");
            string reliver_emp_count = d.getsinglestring("Select sum(1) from pay_employee_master where CLIENT_CODE='" + Session["client_code"].ToString() + "' and comp_code='" + Session["comp_code"].ToString() + "' AND   UNIT_CODE='" + Session["unit_code_addemp"].ToString() + "' AND pay_employee_master.Employee_type = 'Reliever'   and GRADE_CODE ='" + ddl_grade.SelectedValue + "' and (pay_employee_master.LEFT_REASON='' || pay_employee_master.LEFT_REASON is null)");
            if (Perment_emp_count == "") { Perment_emp_count = "0"; }
            int count = Convert.ToInt32(Perment_emp_count) * 3;

            if (reliver_emp_count == "") { reliver_emp_count = "0"; }
            if (Convert.ToInt32(reliver_emp_count) >= (count))
            {
                return true;
            }
        }
        else
        {
            if (ddl_grade.SelectedItem.ToString() != "Select")
            {
                string unit_emp_count = d.getsinglestring("Select sum(COUNT) from pay_designation_count where CLIENT_CODE='" + Session["client_code"].ToString() + "' and comp_code='" + Session["comp_code"].ToString() + "' AND   UNIT_CODE='" + Session["unit_code_addemp"].ToString() + "' and HOURS = '" + txt_attendanceid.SelectedValue + "'  AND DESIGNATION = '" + ddl_grade.SelectedItem.ToString() + "' and branch_status='0'");
                if (unit_emp_count == "")
                {
                    return false;
                }
                string emp_count = d.getsinglestring("Select sum(1) from pay_employee_master where CLIENT_CODE='" + Session["client_code"].ToString() + "' and comp_code='" + Session["comp_code"].ToString() + "' AND   UNIT_CODE='" + Session["unit_code_addemp"].ToString() + "' AND pay_employee_master.Employee_type != 'Reliever'    and GRADE_CODE ='" + ddl_grade.SelectedValue + "' and (pay_employee_master.LEFT_REASON='' || pay_employee_master.LEFT_REASON is null)");
                try
                {


                    emp_count = emp_count == "" ? "0" : emp_count;

                    if (int.Parse(unit_emp_count) < int.Parse(emp_count) + 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception ex) { throw ex; }
                finally { d.con.Close(); }
            }
        }
        return false;
    }
    protected void btn_save_Click(object sender, EventArgs e)
    {

        if (chkcount())
        {
            if (ddl_employee_type.SelectedValue == "Reliever")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Add Atlist One Pernament Employee!!.');", true);
                return;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Take Approval To Change or to Next Employee!!.');", true);
                return;
            }
        }
        try
        {
            int age = 0;
            DateTime current_date = DateTime.Now;
            DateTime birth = DateTime.ParseExact(txt_birthdate.Text.ToString(), "dd/MM/yyyy", null);
            age = current_date.Year - birth.Year;
            if (current_date < birth.AddYears(age))
            {
                age--;

            }
            if (age < 18 || age > 55)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please check Date of Birth !!! Employee should be 18 to 55 Years old');", true);
                txt_birthdate.Text = "";
                txt_birthdate.Focus();
                return;
                //   txt_birthdate.Text = "";
            }
            else
            {
                btnnew_Click(); // rahul 

                //vikas add 17/05/2019
                string joining_date = d.getsinglestring("select DATE_FORMAT(unit_start_date,'%d/%m/%Y') from pay_designation_count where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + Session["client_code"].ToString() + "' and STATE='" + Session["state_new_emp"].ToString() + "' and DESIGNATION=(select GRADE_DESC from pay_grade_master where GRADE_CODE = '" + ddl_grade.SelectedValue + "' and comp_code='" + Session["comp_code"].ToString() + "') and unit_code='" + Session["unit_code_addemp"].ToString() + "' limit 1 ");
                string end_date = d.getsinglestring("select DATE_FORMAT(unit_end_date,'%d/%m/%Y') from pay_designation_count where comp_code='" + Session["comp_code"].ToString() + "' and client_code='" + Session["client_code"].ToString() + "' and STATE='" + Session["state_new_emp"].ToString() + "' and DESIGNATION=(select GRADE_DESC from pay_grade_master where GRADE_CODE = '" + ddl_grade.SelectedValue + "' and comp_code='" + Session["comp_code"].ToString() + "') and unit_code='" + Session["unit_code_addemp"].ToString() + "' limit 1 ");
                try
                {

                    if (joining_date != "" && end_date != "")
                    {
                        if (DateTime.ParseExact(txt_joining_date.Text, "dd/MM/yyyy", null) < DateTime.ParseExact(joining_date, "dd/MM/yyyy", null))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Joining Date Cannot be less than " + joining_date + ".');", true);
                            txt_joining_date.Focus();

                            return;
                        }
                        if (DateTime.ParseExact(txt_joining_date.Text, "dd/MM/yyyy", null) > DateTime.ParseExact(end_date, "dd/MM/yyyy", null))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Joining Date Cannot be Grater than " + end_date + ".');", true);
                            txt_joining_date.Focus();

                            return;
                        }

                    }

                }
                catch (Exception ex) { throw ex; }
                finally { }
                //vikas end 
                string val = txt_joining_date.Text.ToString();
                result = d.operation("INSERT INTO pay_employee_master(comp_code,client_code,UNIT_CODE,Employee_type,EMP_CODE,EMP_NAME,GRADE_CODE,JOINING_DATE,GENDER,attendance_id,EMP_BLOOD_GROUP,EMP_CURRENT_CITY,EMP_CURRENT_STATE,EMP_PERM_CITY,LOCATION_CITY,EMP_PERM_STATE,NFD_CODE,BIRTH_DATE,LOCATION,client_wise_state,EMP_NATIONALITY) VALUES('" + Session["COMP_CODE"].ToString() + "','" + Session["client_code"].ToString() + "','" + Session["unit_code_addemp"].ToString() + "','" + ddl_employee_type.SelectedValue + "','" + txt_emp_code.Text + "','" + txt_emp_name.Text + "','" + ddl_grade.SelectedValue + "',str_to_date('" + txt_joining_date.Text + "','%d/%m/%Y'),'" + ddl_gender.SelectedValue + "','" + txt_attendanceid.SelectedValue + "','Select','Select','" + Session["state_new_emp"].ToString() + "','Select',(select  unit_city from pay_unit_master where unit_code='" + Session["unit_code_addemp"].ToString() + "' and comp_code='" + Session["COMP_CODE"].ToString() + "'),'Select','Select',str_to_date('" + txt_birthdate.Text + "','%d/%m/%Y') ,'" + Session["state_new_emp"].ToString() + "','" + Session["state_new_emp"].ToString() + "','INDIAN')");

                upolad_photo();


                if (result > 0)
                {

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Employee Added Successfully !!!');", true);
                    employee_leave();
                    MySqlCommand cmdmax = new MySqlCommand();
                    d1.con.Open();
                    string comp_code_dob = Session["comp_code"].ToString();

                    if (comp_code_dob == "C01")
                    {
                        cmdmax = new MySqlCommand("SELECT ifnull(MAX(SUBSTRING(Id_as_per_dob, 8, 6)),0) FROM  pay_employee_master WHERE comp_code='" + Session["comp_code"].ToString() + "'", d1.con);

                    }
                    else if (comp_code_dob == "C02")
                    {
                        cmdmax = new MySqlCommand("SELECT ifnull(MAX(SUBSTRING(Id_as_per_dob, 8, 6)),0) FROM  pay_employee_master WHERE comp_code='" + Session["comp_code"].ToString() + "'", d1.con);

                    }
                    MySqlDataReader drmax = cmdmax.ExecuteReader();
                    if (drmax.Read())
                    {
                        int rownum = int.Parse(drmax.GetValue(0).ToString());
                        int rownum1 = int.Parse(drmax.GetValue(0).ToString());
                        rownum = rownum + 1;
                        if (comp_code_dob == "C02")
                        {
                            d.operation("update pay_employee_master set Id_as_per_dob = (case when " + rownum1 + " < 9 then concat('IHMS-','M0000','" + rownum + "') when  " + rownum1 + " < 99 then concat('IHMS-','M000','" + rownum + "') when " + rownum1 + "<999 then concat('IHMS-','M00','" + rownum + "') when " + rownum1 + "<9999 then concat('IHMS-','M0','" + rownum + "') end) where comp_code='C02' and emp_code='" + txt_emp_code.Text + "' order by joining_date ");
                        }
                        else if (comp_code_dob == "C01")
                        {
                            d.operation("update pay_employee_master set Id_as_per_dob = (case when " + rownum1 + " < 9 then concat('IH&MS-','I0000','" + rownum + "') when  " + rownum1 + " < 99 then concat('IH&MS-','I000','" + rownum + "') when " + rownum1 + "<999 then concat('IH&MS-','I00','" + rownum + "') when " + rownum1 + "<9999 then concat('IH&MS-','I0','" + rownum + "') end) where comp_code='C01' and emp_code='" + txt_emp_code.Text + "' order by joining_date ");
                        }
                    }
                    drmax.Close();
                    d1.con.Close();
                }

                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Employee Adding Failed !!!');", true);
                }
                text_clera();

            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            // databind(null, null);
            // ddl_clear(null, null);
            d.con.Close();
            btnnew_Click();//12/11
        }
    }

    protected void upolad_photo()
    {
        if (txt_emp_code.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertmessage", "javascript:alert('Please select Employee first !!!')", true);
        }
        else if (txt_emp_code.Text != "")
        {
            string cnt = "0";
            d.con.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT count(*) FROM pay_images_master WHERE emp_code = '" + txt_emp_code.Text + "' AND  comp_code='" + Session["comp_code"].ToString() + "'", d.con);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    cnt = dr.GetValue(0).ToString();
                }
            }
            catch (Exception ex) { throw ex; }
            finally { d.con.Close(); }
            if (cnt == "0")
            {
                d.operation("INSERT INTO pay_images_master (EMP_CODE, emp_name, comp_code,MODIF_DATE) values ('" + txt_emp_code.Text + "','" + txt_emp_name.Text + "', '" + Session["comp_code"].ToString() + "',curdate())");
            }
        }
    }
    protected void btnnew_Click()
    {
        d1.con.Open();
        try
        {
            MySqlCommand cmdmax = new MySqlCommand("SELECT MAX(CAST(SUBSTRING(EMP_CODE, 2, 5) AS UNSIGNED))+1 FROM  pay_images_master WHERE comp_code='" + Session["comp_code"].ToString() + "'", d1.con);
            MySqlDataReader drmax = cmdmax.ExecuteReader();
            if (!drmax.HasRows)
            {

            }
            else if (drmax.Read())
            {
                d1.con1.Open();
                try
                {
                    MySqlCommand cmdinit = new MySqlCommand("SELECT EMP_SERIES_INIT FROM pay_company_master WHERE comp_code='" + Session["comp_code"].ToString() + "'", d1.con1);
                    MySqlDataReader drinit = cmdinit.ExecuteReader();
                    if (!drinit.HasRows)
                    {
                    }
                    else if (drinit.Read())
                    {
                        string stinit = drinit.GetValue(0).ToString();
                        string initcode = "";
                        if (stinit != "")
                        {
                            initcode = stinit.ToString();
                            string str = drmax.GetValue(0).ToString();
                            if (str == "")
                            {
                                txt_emp_code.Text = initcode.ToString() + "00001";
                            }
                            else
                            {
                                int max_empcode = int.Parse(drmax.GetValue(0).ToString());
                                if (max_empcode < 10)
                                {
                                    txt_emp_code.Text = initcode.ToString() + "0000" + max_empcode;
                                }
                                else if (max_empcode >= 10 && max_empcode < 100)
                                {
                                    txt_emp_code.Text = initcode.ToString() + "000" + max_empcode;
                                }
                                else if (max_empcode >= 100 && max_empcode < 1000)
                                {
                                    txt_emp_code.Text = initcode.ToString() + "00" + max_empcode;
                                }
                                else if (max_empcode >= 1000 && max_empcode < 10000)
                                {
                                    txt_emp_code.Text = initcode.ToString() + "0" + max_empcode;
                                }
                                else if (max_empcode == 10000)
                                {
                                    txt_emp_code.Text = initcode.ToString() + "" + max_empcode;
                                }
                            }
                        }
                    }
                    drinit.Close();
                }
                catch (Exception ex) { throw ex; }
                finally
                {
                    d1.con1.Close();
                }
            }
            drmax.Close();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d1.con.Close();

        }

    }
    protected void ddl_grade_SelectedIndexChanged(object sender, EventArgs e)
    {
        string grade = "";
        if (Session["client_code"].ToString().Equals("Staff") || Session["client_code"].ToString().Equals("IHMS")) { }

        else
        {
            grade = d.getsinglestring("select designation from pay_billing_master where billing_unit_code= '" + Session["unit_code_addemp"].ToString() + "' and designation='" + ddl_grade.SelectedValue + "' and comp_code='" + Session["COMP_CODE"].ToString() + "'");
            if (grade.Equals(""))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please create branch Policy......');", true);

                ddl_grade.SelectedIndex = 0;
                return;
            }
        }
        txt_attendanceid.Items.Clear();
        System.Data.DataTable dt_item = new System.Data.DataTable();
        MySqlDataAdapter cmd_item = new MySqlDataAdapter("SELECT distinct(hours) from pay_designation_count where unit_code ='" + Session["unit_code_addemp"].ToString() + "' and comp_code = '" + Session["COMP_CODE"].ToString() + "' and client_code = '" + Session["client_code"].ToString() + "' and designation = (Select grade_desc from pay_grade_master where grade_code = '" + ddl_grade.SelectedValue + "' and COMP_CODE = '" + Session["COMP_CODE"].ToString() + "')", d.con);
        try
        {
            cmd_item.Fill(dt_item);
            if (dt_item.Rows.Count > 0)
            {
                txt_attendanceid.DataSource = dt_item;
                txt_attendanceid.DataValueField = dt_item.Columns[0].ToString();
                txt_attendanceid.DataTextField = dt_item.Columns[0].ToString();
                txt_attendanceid.DataBind();

            }
            d.con.Close();
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            d.con.Close();
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

    public void employee_leave()
    {
        //int result = 0;
        //result = d.operation("INSERT INTO pay_leave_emp_balance(comp_code,unit_code,EMP_CODE,last_update_date,create_user,create_date,leave_name,abbreviation,gender,max_no_of_leave,balance_leave) select '" + Session["comp_code"].ToString() + "', '" + ddl_unitcode.SelectedValue.ToString().Substring(0, 4) + "','" + txt_eecode.Text + "',now(),'" + Session["LOGIN_ID"].ToString() + "',now(),leave_name,abbreviation,gender,max_no_of_leave,max_no_of_leave from pay_leave_master where comp_code='" + Session["comp_code"].ToString() + "' and gender in ('" + ddl_gender.SelectedValue + "','B') ");
        d.operation("INSERT INTO pay_user_master(LOGIN_ID,USER_NAME,USER_PASSWORD,flag,create_user, create_date, password_changed_date,first_login,comp_code) VALUES('" + txt_emp_code.Text + "','" + txt_emp_name.Text + "','" + GetSha256FromString(txt_birthdate.Text) + "','A','" + Session["USERID"].ToString() + "',now(),now(),'0','" + Session["comp_code"].ToString() + "')");
    }
    protected void text_clera()
    {
        //txt_emp_code.Text = "";
        txt_emp_name.Text = "";
        txt_joining_date.Text = "";
        // txt_left_date.Text = "";
        // txt_email_id.Text = "";
        txt_birthdate.Text = "";
        ddl_employee_type.SelectedValue = "0";
        ddl_grade.SelectedValue = "Select";

        ddl_gender.SelectedValue = "0";
        // txt_attendanceid.Text = "Select";
        //ddl_relation.SelectedIndex = 0;

    }

    //protected void btn_close_Click(object sender, EventArgs e)
    //{
    //    //ClientScript.RegisterStartupScript(typeof(Page), "emp_sample.aspx", "window.close(emp_sample.aspx);", true);
    //    Response.Redirect("emp_sample.aspx");

    //}
}