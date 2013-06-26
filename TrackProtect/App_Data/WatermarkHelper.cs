using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
	public static class WatermarkHelper
	{
        public static void ApplyWaterMarkToTextBox(ref TextBox textBox, string waterMarkText, 
                                            string waterMarkStyle, string normalStyle)
        {
            textBox.Attributes.Add("OnFocus",
                                   "javascript:js_waterMark_Focus('" + textBox.ClientID + "', '" +
                                   waterMarkText.Replace("'", "\'") + "','" + waterMarkStyle + "', '" + normalStyle +
                                   "')");
            textBox.Attributes.Add("OnBlur",
                                   "javascript:js_waterMark_Blur('" + textBox.ClientID + "', '" +
                                   waterMarkText.Replace("'", "\'") + "','" + waterMarkStyle + "', '" + normalStyle +
                                   "')");
            textBox.Text = waterMarkText;
            textBox.CssClass = waterMarkStyle;
            if (!textBox.Page.ClientScript.IsClientScriptBlockRegistered("WaterMarkScript"))
            {
				const string javaScript = @"
<script language='javascript'>\r\n
  function js_waterMark_Focus(objname, waterMarkText, waterMarkStyle, normalStyle)\r\n
  {\r\n
      obj = document.getElementById(objname);\r\n
      if(obj.value == waterMarkText)\r\n
      {\r\n
          obj.value='""';\r\n
          obj.className = normalStyle\r\n
      }\r\n
  }\r\n
  function js_waterMark_Blur(objname, waterMarkText, waterMarkStyle, normalStyle)\r\n
  {\r\n
      obj = document.getElementById(objname);\r\n
      if(obj.value == '""')\r\n
      {\r\n
         obj.value=waterMarkText;\r\n
          obj.className = waterMarkStyle\r\n
      }\r\n
      else\r\n
      {\r\n
          obj.className = normalStyle\r\n
      }\r\n
  }\r\n
</script>\r\n
";
                textBox.Page.ClientScript.RegisterClientScriptBlock(textBox.Page.GetType(), "WaterMarkScript",
                                                                    javaScript, false);
            }
        }
	}
}
