using Newtonsoft.Json;
using NiceApiLibrary_low;
using NiceApiLibrary;
using NiceASP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NiceApiLibrary.ASP_AppCode;


public partial class apiv1_Message : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // This comes from an external user

        Response.ContentType = "application/json";
        try
        {
            jsonIn jsonIn = null;
            jsonOut jsonOut = new jsonOut();
            IMyLog log = MyLog.GetLogger("JsonAPI");

            // read
            if (!Request.HttpMethod.Equals("POST"))
            {
                jsonOut.Result = "HttpMethod " + Request.HttpMethod + " not supported.";
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        JsonSerializer ser = new JsonSerializer();
                        ser.Serialize(jsonWriter, jsonOut, typeof(jsonOut));
                        jsonWriter.Flush();
                    }
                }
                return;
            }
            using (StreamReader streamReader = new StreamReader(Request.InputStream))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                {
                    JsonSerializer ser = new JsonSerializer();
                    jsonIn = (jsonIn)ser.Deserialize(jsonReader, typeof(jsonIn));
                }
            }

            // work
            jsonOut.Result = new APIActualSending(jsonIn.APIId.GetSystemInfoFromAPIId()).SendWhatsApp(
                jsonIn.APIId,
                jsonIn.APIMobile,
                jsonIn.Message,
                log);

            // response
            using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    JsonSerializer ser = new JsonSerializer();
                    ser.Serialize(jsonWriter, jsonOut, typeof(jsonOut));
                    jsonWriter.Flush();
                }
            }
        }
        catch (Exception ex)
        {
            jsonOut ExOut = new jsonOut();
            ExOut.Result = "Exception";
            using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    JsonSerializer ser = new JsonSerializer();
                    ser.Serialize(jsonWriter, ExOut);
                    jsonWriter.Flush();
                }
            }
        }
    }
}

internal class jsonIn
{
    public string APIId { get; set; }
    public string APIMobile { get; set; }
    public string Message { get; set; }
}

internal class jsonOut
{
    public string Result { get; set; }
}

