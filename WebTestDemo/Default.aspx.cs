using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Vic.Data.DataAccess dataAccess = new Vic.Data.DataAccess("Data Source=orcl20.83;User ID=tzgjspdb;Password=tzgjspdb123;", Vic.Data.DbProviderType.OracleClient);

            string ex = "";
            bool b = dataAccess.CheckConn(out ex);

            DataTable dt = dataAccess.QueryTable("select * from CX_RZ");
            this.GridView1.DataSource = dt;
            this.GridView1.DataBind();

        }
    }
}
