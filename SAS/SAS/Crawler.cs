using System.Collections.Generic;
using System.Net;
using RestSharp;

namespace SAS
{
    public class Crawler
    {
        private string url = "https://classic.flysas.com/en/de/";
        public IList<RestResponseCookie> mainCookieList { get; set; }
        public void crawling()
        {
            RestClient client = new RestClient(url);
            client = addingHeaders(client);
            // first pageload
            GETrequest(client);
            // second pageload
            POSTrequest(client);
        }
        private void POSTrequest(RestClient client)
        {
            RestRequest request = new RestRequest("", Method.POST);
            client.AddDefaultHeader("Content-Type", "multipart/form-data; boundary=---------------------------101362052429613054672289599900");
            client.AddDefaultHeader("Origin", "https://classic.flysas.com");
            client.AddDefaultHeader("Referer", "https://classic.flysas.com/en/de/");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0";
            client.AddDefaultHeader("Connection", "keep-alive");
            
            foreach (RestResponseCookie cookie in mainCookieList)
            {
                request.AddCookie(cookie.Name, cookie.Value);
            }
          //  request.AddBody("");
            IRestResponse response = client.Execute(request);
        }
        public RestClient addingHeaders(RestClient client)
        {
            client.AddDefaultHeader("Host", "classic.flysas.com");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0";
            client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.AddDefaultHeader("Accept-Language", "en-GB,en;q=0.5");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.AddDefaultHeader("DNT", "1");
            client.ConnectionGroupName = "keep-alive";
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.FollowRedirects = false;
            return client;
        }
        private void GETrequest(RestClient client)
        {
            RestRequest request = new RestRequest("", Method.GET);
            IRestResponse response = client.Execute(request);
            collectingCookies(response);
        }
        private void collectingCookies(IRestResponse response)
        {
            mainCookieList = response.Cookies;
        }
    }
}
