using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using NiceASP;
using NiceApiLibrary;
using NiceApiLibrary.ASP_AppCode;

public partial class Price : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This come from an anonymous browser
        Page.MetaKeywords = "Price, Send Whatsapp message, Free";
        Page.MetaDescription = "See all prices in details.";
        NiceASP.KeywordLoader.Load(this, NiceASP.KeywordLoader.Which.Price);

        StringBuilder sb = new StringBuilder();

        AddTable(sb, "46,204,64",
            UpdatePriceText.s_FreeAccountInfo.Title,
            "Register",
            "buttonRegister",
            UpdatePriceText.s_FreeAccountInfo.Info);

        AddTable(sb, "255, 220, 0",
            UpdatePriceText.s_PayAsYouSend.Title,
            "Upgrade",
            "buttonUpgradePayAsYouSend",
            UpdatePriceText.s_PayAsYouSend.Info);

        AddTable(sb, "255, 133, 27",
            UpdatePriceText.s_PayMonthlyDifPrice.Title,
            "Upgrade",
            "buttonUpgradePayMonthlyDifPrice",
            UpdatePriceText.s_PayMonthlyDifPrice.Info);

        AddTable(sb, "255, 65, 54",
            UpdatePriceText.s_SystemDuplication.Title,
            "Upgrade",
            "buttonUpgradeSystemDuplication",
            UpdatePriceText.s_SystemDuplication.Info);

        lit.Text = sb.ToString();

    }

    private static void AddTable(StringBuilder sb, string rgb,
        string title, string buttonText, string buttonId, string[] textList)
    {
        sb.AppendLine("<table border=\"1\" width=\"100%\">");
        sb.AppendLine("<tr>");
        sb.AppendLine("<td width=\"20%\" valign=\"center\" align=\"center\" style=\"background-color:rgba(" + rgb + ",1.0);\" >");
        sb.AppendLine("<p ><h3>" + title + "</h3></p>");
        sb.AppendLine("</td>");
        sb.AppendLine("<td rowspan=\"2\" valign=\"top\" style=\"background-color:rgba(" + rgb + ",0.7);\"  >");
        foreach (string text in textList)
        {
            sb.AppendLine("<p ><span><div>&nbsp;- " + text + "</div></span></p>");
        }
        sb.AppendLine("</td>");
        sb.AppendLine("</tr>");
        sb.AppendLine("<tr>");
        sb.AppendLine("<td width=\"20%\" valign=\"center\" align=\"center\" style=\"background-color:rgba(" + rgb + ",0.5);\" >");
        sb.AppendLine("<p ><input class=\"button\" type=\"button\" id=\"" + buttonId + "\" value=\"" + buttonText + "\"></p>");
        sb.AppendLine("</td>");
        sb.AppendLine("</tr>");
        sb.AppendLine("</table><br />");
    }
}

