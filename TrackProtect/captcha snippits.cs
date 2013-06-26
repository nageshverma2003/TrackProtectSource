<asp:Image ID="imgCaptcha" ImageUrl="Captcha.ashx" runat="server" />

-------------------------------

private void SetCaptchaText()
{
	const char base[] = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz".ToCharArray();
	Random random = new Random();
	StringBuilder captcha = new StringBuilder();
	for (int i = 0; i < 6; i++)
	{
		int idx = random.Next(0, base.Length - 1);
		captcha.Append(base[idx]);
	}
	Session["Captch"] = captcha.ToString();
}

---------------------------------

if (Session["Captcha"].ToString() == txtCaptcha.Text.Trim())
	Response.Redirect("Failed.aspx");
else
	Response.Redirect("Success.aspx");

---------------------------------
<httpHandlers>
  <add verb="GET" path="ImageBasedBotDetector.ashx" 
       type="Marss.Web.UI.Controls.ImageBasedBotDetector, Marss.Web"/>
</httpHandlers>