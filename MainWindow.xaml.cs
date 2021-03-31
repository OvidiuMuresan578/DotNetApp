using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Net;

namespace WpfApp3
{
    struct SearchResult
    {
        public String jsonResult;
        public Dictionary<String, String> relevantHeaders;
    }


    public partial class MainWindow : Window
    {
        // In production, make sure you're pulling the subscription key from secured storage.
        const string subscriptionKey = "51a0c2489827455684b888f7f0173135";
        const string uriBase = "https://api.bing.microsoft.com/v7.0/news/search";
        string searchkeyWord = "Sp500";


        private static string path = @"C:\Users\ovidi\OneDrive\Desktop\NewsApiInfo.txt";

        public MainWindow()
        {
            

            InitializeComponent();
            




        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
             var res = String.Empty;
             SearchResult result = BingNewsSearch(KeyWordText.Text);
             var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(result.jsonResult);


            WriteToFile(JsonConvert.SerializeObject(jsonObj));

            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(ReadFromFile());

            DisplayPage dp = new DisplayPage();
            dp.DisplayData.Text = myDeserializedClass.value[0].name;// we need to proces data from classes in order to display it
            dp.Show();

        }

       

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        static SearchResult BingNewsSearch(string toSearch)
        {

            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(toSearch);
            //...

            WebRequest request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = subscriptionKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // Create the result object for return
            var searchResult = new SearchResult()
            {
                jsonResult = json,
                relevantHeaders = new Dictionary<String, String>()
            };

            // Extract Bing HTTP headers
            foreach (String header in response.Headers)
            {
                if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                    searchResult.relevantHeaders[header] = response.Headers[header];
            }
            return searchResult;
        }

        public static void WriteToFile(string content)
        {


            // This text is added only once to the file.
            try
            {
                File.Delete(path);
            }
            catch(Exception e)
            {
                MessageBox.Show("File not found");
            }


            if (!File.Exists(path))
            {
                // Create a file to write to.
                string createText = content + Environment.NewLine;
                File.WriteAllText(path, createText);
            }        
        }

        public static string ReadFromFile()
        {
            // Open the file to read from.
            string readText = File.ReadAllText(path);
            return readText;
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class QueryContext
        {
            public string originalQuery { get; set; }
            public bool adultIntent { get; set; }
        }

        public class Sort
        {
            public string name { get; set; }
            public string id { get; set; }
            public bool isSelected { get; set; }
            public string url { get; set; }
        }

        public class Thumbnail
        {
            public string contentUrl { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Image
        {
            public Thumbnail thumbnail { get; set; }
        }

        public class Provider
        {
            public string _type { get; set; }
            public string name { get; set; }
        }

        public class About
        {
            public string readLink { get; set; }
            public string name { get; set; }
        }

        public class Value
        {
            public string name { get; set; }
            public string url { get; set; }
            public Image image { get; set; }
            public string description { get; set; }
            public List<Provider> provider { get; set; }
            public DateTime datePublished { get; set; }
            public List<About> about { get; set; }
        }

        public class Root
        {
            public string _type { get; set; }
            public string readLink { get; set; }
            public QueryContext queryContext { get; set; }
            public int totalEstimatedMatches { get; set; }
            public List<Sort> sort { get; set; }
            public List<Value> value { get; set; }

            /* public override string tostring()
            {
                return $"name: {this.name}{environment.newline}" +
                    $"url: {this.url}{environment.newline}" +
                    $"datetime: {this.datepublished.tostring()}{environment.newline}";
            }*/
        }






        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }
    }
}
