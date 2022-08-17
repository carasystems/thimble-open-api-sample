using System;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Net.Http;

class MainClass {
  public static string byteToString(byte[] buff)
  {
    string str = "";
    for (int i = 0; i < buff.Length; i++)
    {
        str += buff[i].ToString("X2");
    }
    return (str);
  }
  
  private static string sign(string appKey, string secret, string timestamp, string url, string reqData)
  {
    Console.WriteLine("reqData: " + reqData);

    string data = appKey + url + timestamp + reqData;
    System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
    byte[] keyByte = encoding.GetBytes(secret);
    byte[] messageBytes = encoding.GetBytes(data);
    HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
    byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);

    return byteToString(hashmessage);
  }

  public static async Task Main(string[] args)
  {
    string baseUri = "https://open-api-test.thimble.com"; // sandbox uri
    string appKey = "<Your-App-Key-Here>";
    string secret = "<Your-App-Secret-Here>";
    string timestamp = "1660225739"; // Epoch Unix Time Stamp
    string url = "/insurance-plans"; // The request url, contains query string. Say a request to "https://open-api.thimble.com/activities?q=1", the 'url' would be /activities?q=1.
    string reqData = "{\"activity_id\":\"ACT-00037\",\"zipcode\":\"07013\"}"; // The request data, convert it into a string

    /* do get
      string signGetRequest = sign(appKey, secret, timestamp, url , "").ToLower();
      Console.WriteLine("sign: " + signGetRequest);
      using  (var httpClient = new HttpClient())
        {
          httpClient.BaseAddress = new Uri(baseUri);
          httpClient.DefaultRequestHeaders.Add("appkey", appKey);
          httpClient.DefaultRequestHeaders.Add("x-timestamp", timestamp);
          httpClient.DefaultRequestHeaders.Add("x-signature", signGetRequest);
          var response = await httpClient.GetAsync(url);
          var responseString = await response.Content.ReadAsStringAsync();
          Console.WriteLine("Receive data: " + responseString); ;
        }
    */

    // do post
    string signPostRequest = sign(appKey, secret, timestamp, url, reqData).ToLower();
    Console.WriteLine("hash: " + signPostRequest); 
    using  (var httpClient = new HttpClient())
    {
      httpClient.BaseAddress = new Uri(baseUri);
      httpClient.DefaultRequestHeaders.Add("appkey", appKey);
      httpClient.DefaultRequestHeaders.Add("x-timestamp", timestamp);
      httpClient.DefaultRequestHeaders.Add("x-signature", signPostRequest);
      var requestData = new StringContent(reqData, Encoding.UTF8, "application/json");
      var response = await httpClient.PostAsync(url, requestData);
      var responseString = await response.Content.ReadAsStringAsync();
      Console.WriteLine("Receive data: " + responseString);
    }
  }
}
