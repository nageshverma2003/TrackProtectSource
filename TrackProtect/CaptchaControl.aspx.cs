using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

public partial class CaptchaControl : System.Web.UI.Page
{
	const int WIDTH = 200;
	const int HEIGHT = 60;
	
    private Random rand = new Random();
   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {         
           CreateImage();           
        }
    }

    private void CreateImage()
    {
		Session["captcha.guid"] = Guid.NewGuid().ToString ("N");
        string code = GetRandomText(); 
        
        Bitmap bitmap = new Bitmap(WIDTH,HEIGHT,System.Drawing.Imaging.PixelFormat.Format32bppArgb);        

        Graphics g = Graphics.FromImage(bitmap); 
        Pen pen = new Pen(Color.DarkSlateGray); 
        Rectangle rect = new Rectangle(0,0,WIDTH,HEIGHT);

        SolidBrush background = new SolidBrush(Color.AntiqueWhite); 
        SolidBrush textcolor = new SolidBrush(Color.DarkSlateGray);
       
        int counter = 0; 
       
        g.DrawRectangle(pen, rect);
        g.FillRectangle(background, rect);

        for (int i = 0; i < code.Length; i++)
        {
            g.DrawString(code[i].ToString(), 
			             new Font("Verdana", 10 + rand.Next(6, 14)), 
			             textcolor, 
			             new PointF(10 + counter, 10));
            counter += 25;
        }

        DrawRandomLines(g); 

        bitmap.Save(Response.OutputStream,ImageFormat.Gif);

        g.Dispose();
        bitmap.Dispose();
        
    }

    private void DrawRandomLines(Graphics g)
    {
        SolidBrush linecolor = new SolidBrush(Color.DarkSlateGray);
		g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        for (int i = 0; i < 10; i++)
        {
            g.DrawLines(new Pen(linecolor, 1), GetRandomPoints());
        } 

    }

    private Point[] GetRandomPoints()
    {
        Point[] points = { 
			new Point(rand.Next(10, WIDTH-10), rand.Next(10, HEIGHT-10)), 
			new Point(rand.Next(10, WIDTH-10), rand.Next(10, HEIGHT-10)) 
		};
        return points; 
    }

    private string GetRandomText()
    {
		string guid = Session["captcha.guid"] as string;
        StringBuilder randomText = new StringBuilder();
        
        if (Session[guid] == null)
        {
            string alphabets = "abcdefghijklmnopqrstuvwxyz0123456789";
            
            Random r = new Random();
            for (int j = 0; j <= 6; j++)
                randomText.Append(alphabets[r.Next(alphabets.Length)]);

            Session[guid] = randomText.ToString(); 
        }

        return Session[guid] as String; 
    }

}
