using System.IO;
using System.Text;
using System.Net;

namespace GoFoodBeverage.Payment.MoMo
{
    public class MoMoPaymentResponse
    {
        public bool Success { get; set; }

        public string Data { get; set; }
    }

    /// <summary>
    /// Request config provided by momo
    /// </summary>
    public class MoMoPaymentRequest
    {
        public MoMoPaymentRequest() { }

        public static MoMoPaymentResponse SendPaymentRequest(string endpoint, string postJsonString)
        {
            var response = new MoMoPaymentResponse();
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(endpoint);
                var postData = postJsonString;
                var data = Encoding.UTF8.GetBytes(postData);
                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/json";
                httpWReq.ContentLength = data.Length;
                httpWReq.ReadWriteTimeout = 30000;
                httpWReq.Timeout = 15000;
                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWReq.GetResponse();
                string jsonresponse = string.Empty;
                using (var reader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    string temp = null;
                    while ((temp = reader.ReadLine()) != null)
                    {
                        jsonresponse += temp;
                    }
                }

                response.Success = true;
                response.Data = jsonresponse;
            }
            catch (WebException e)
            {
                response.Data = e.Message;
            }

            return response;
        }
    }
}
