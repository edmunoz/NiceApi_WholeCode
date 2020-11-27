using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;
using NiceApiLibrary_low;
using NiceASP;


public partial class Sql : System.Web.UI.Page
{
    IMyLog logInTest = MyLog.GetLogger("Sql");

    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from the Admin User (protected)
        NiceASP.SessionData.LoggedOnOrRedirectToRoot(Session, Response, true);
    }


    protected void Button1_Click(object sender, EventArgs e)
    {
        PlaceHolder1.Controls.Clear();

        object sqlRes;
        try
        {
            sqlRes = DSSwitch.sql().ProcessSql(SessionData.SessionsSystem_Get(Session), TextBox_Sql.Text);
        }
        catch (Exception ex)
        {
            sqlRes = ex.Exceptio2Table();
        }

        if ((sqlRes != null) && (sqlRes.GetType() == typeof(System.Data.DataTable)))
        {
            System.Data.DataTable r1 = (System.Data.DataTable)sqlRes;
            WebControlsTableResult r2 = r1.DataTable2WebControlsTable(20);

            PlaceHolder1.Controls.Add(r2.Label);
            PlaceHolder1.Controls.Add(r2.Table);
        }
        else if ((sqlRes != null) && (sqlRes.GetType() == typeof (Table)))
        {
            PlaceHolder1.Controls.Clear();
            Table sqlTable = (Table)sqlRes;
            PlaceHolder1.Controls.Add(sqlTable);
        }
    }

    private int GetCfg()
    {
        return 20;

    }
}