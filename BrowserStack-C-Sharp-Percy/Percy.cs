using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;


namespace BrowserStackPercy
{
    public class Percy
    {
        // HTTP Client to communicate with Percy
        static RestClient client;

        // Selenium WebDriver we'll use for accessing the web pages to snapshot.
        private IWebDriver driver;

        // The JavaScript contained in percy-agent.js
        private String percyAgentJs;

        // Is the Percy Agent process running or not
        private Boolean percyIsRunning = true;

        public Percy(IWebDriver driver)
        {
            this.driver = driver;
            client = new RestClient("http://localhost:5338");
            percyAgentJs = loadPercyAgentJs();
        }

        private String loadPercyAgentJs()
        {
            IRestRequest restRequest = new RestRequest("percy-agent.js", DataFormat.None);
            IRestResponse restResponse = client.Get(restRequest);

            if (!restResponse.IsSuccessful)
            {
                Console.WriteLine("[percy] An error occured while retrieving percy-agent.js: " + restResponse.ErrorMessage);
                percyIsRunning = false;
                Console.WriteLine("[percy] Percy has been disabled");
                return null;
            }

            return restResponse.Content;
        }

        public void snapshot(String name)
        {
            snapshot(name, new int[0], 0, false, "");
        }

        public void snapshot(String name, int[] widths)
        {
            snapshot(name, widths, 0, false, "");
        }

        public void snapshot(String name, int[] widths, int minHeight)
        {
            snapshot(name, widths, minHeight, false, "");
        }

        public void snapshot(String name, int[] widths, int minHeight, Boolean enableJavaScript)
        {
            snapshot(name, widths, minHeight, enableJavaScript, "");
        }

        public void snapshot(String name, int[] widths, int minHeight, Boolean enableJavaScript, String percyCSS)
        {
            String domSnapshot = "";

            if (percyAgentJs == null) return;

            try
            {
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript(percyAgentJs);
                domSnapshot = (String)jse.ExecuteScript("var percyAgentClient = new PercyAgent({handleAgentCommunication: false}); return percyAgentClient.snapshot('not used');");

                postSnapshot(domSnapshot, name, widths, minHeight, driver.Url, enableJavaScript, percyCSS);
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("[percy] Something went wrong attempting to take a snapshot: " + e.GetBaseException());
            }
        }

        private void postSnapshot(String domSnapshot, String name, int[] widths, int minHeight, String url, Boolean enableJavaScript, String percyCSS)
        {
            if (!percyIsRunning)
            {
                return;
            }

            IRestRequest restRequest = new RestRequest("percy/snapshot", DataFormat.Json);
            restRequest.Method = Method.POST;

            var requestBody = new {
                url = url,
                name = name,
                domSnapshot = domSnapshot,
                clientInfo = "test clientInfo",
                enableJavaScript = enableJavaScript,
                environmentInfo = "test environmentInfo",
                //widths = widths.
                //minHeight = minHeight,
                //percyCSS = percyCSS
            };

            restRequest.AddJsonBody(requestBody);

            IRestResponse restResponse = client.Post(restRequest);

            if (!restResponse.IsSuccessful)
            {
                Console.WriteLine("percy] An error occured when sending the DOM to agent: " + restResponse.ErrorMessage);
                percyIsRunning = false;
                Console.WriteLine("[percy] Percy has been disabled");
                return;
            }

            // to debug 
            //Console.WriteLine(restResponse.Request.Body.Value.ToString());
            //Console.WriteLine(restResponse.Content);
        }
    }
}
