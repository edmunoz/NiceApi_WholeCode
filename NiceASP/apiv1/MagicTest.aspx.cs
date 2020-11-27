using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class apiv1_MagicTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from an external user

        Response.ContentType = "application/json";
        MagicTest_JsonOut jsonOut = new MagicTest_JsonOut()
        {
            Id = "13",
            Zapi = "zapi_41795937121",
            Text = "justTesting"
        };
        using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
        {
            using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, jsonOut);
                jsonWriter.Flush();
            }
        }
    }
}

internal class MagicTest_JsonOut
{
    public string Id { get; set; }
    public string Zapi { get; set; }
    public string Text { get; set; }
}

