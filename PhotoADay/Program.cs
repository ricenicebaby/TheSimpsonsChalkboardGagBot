using System;
using System.IO;
using System.Threading.Tasks;
// Twitter API
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
// Web scraping
using HtmlAgilityPack;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

namespace PhotoADay
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string APIKey = "{}";
            string APISecret = "{}";
            string accessToken = "{}";
            string accessSecret = "{}";

            Random randNum = new Random();
            List<string> chalkboardQuotes = await GetHtmlAsync();

            byte[] imageBytes = File.ReadAllBytes("psyduck.gif");
            TwitterClient userClient = new TwitterClient(APIKey, APISecret, accessToken, accessSecret);
            IMedia imageIMedia = await userClient.Upload.UploadTweetImageAsync(imageBytes);

            ITweet tweetWithImage = await userClient.Tweets.PublishTweetAsync(
                new PublishTweetParameters(chalkboardQuotes[randNum.Next(chalkboardQuotes.Count - chalkboardQuotes.Count/3)]));
        }

        private static async Task<List<string>> GetHtmlAsync()
        {
            string url = "https://simpsons.fandom.com/wiki/List_of_chalkboard_gags";

            HttpClient client = new HttpClient();
            var html = await client.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var quotesHtml = htmlDocument.DocumentNode.Descendants("table") // On the site, there are tables for each season. This grabs all tables, aka seasons
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("wikitable")).ToList();

            var seasonsTableList = new List<HtmlNode>();
            var quotesSet = new List<HtmlNode>();

            var quotesSet2 = new List<HtmlNode>();
            var quotesList = new List<string>();

            for (int i = 0; i < quotesHtml.Count; i++)
            {
                seasonsTableList = quotesHtml[i].Descendants("tbody").ToList(); // Get a new season

                quotesSet = seasonsTableList[0].Descendants("tr")
                                .Where(node => node.GetAttributeValue("style", "")
                                .Contains("background-color")).ToList();

                for (int j = 0; j < quotesSet.Count; j++)
                {
                    quotesSet2.Add(quotesSet[j].Descendants("b").FirstOrDefault());
                }
            }

            foreach (var quote in quotesSet2)
            {
                if(quote != null)
                {
                    quotesList.Add(quote.InnerText);
                }
            }

            return quotesList;
        }

    }
}
