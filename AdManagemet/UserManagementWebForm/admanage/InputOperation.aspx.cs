using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UserManagementWebForm.admanage
{
    public partial class InputOperation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
               outArea.InnerText=("Success"+inText.Value);
            }
        }
    }
}