using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace TrackProtect
{
	public partial class Contact : System.Web.UI.Page
	{
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
			{
				using (TextReader reader = new StreamReader("Content/contact.inc"))
				{
					ContactLiteral.Text = reader.ReadToEnd();
				}
			}
		}
	}
}

