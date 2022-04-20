using System;
using System.Net;
using PuppeteerSharp;

namespace RemoteWebDriverDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            Console.WriteLine("Hello World!");

            //BasicUsage();
            RemoteUsage();

            Console.ReadLine();

        }

        /// <summary>
        /// basic usage
        /// </summary>
        static async void BasicUsage()
        {
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                DefaultViewport = null,
                Headless = true
            });
            var page = await browser.NewPageAsync();
            await page.GoToAsync("http://www.google.com");
            await page.ScreenshotAsync(@"screenshot.png");

            Console.WriteLine("done!");
        }

        /// <summary>
        /// Connect to a remote browser
        /// </summary>
        static async void RemoteUsage()
        {
            // chrome.exe --remote-debugging-port=9222 --user-data-dir="e:\\cache" --no-default-browser-check --proxy-server="ip:port"
            /*
             * other args:
             * --remote-debugging-address="ip"
             * --no-first-run
             * --no-sandbox
             * --no-zygote
             * --disable-setuid-sandbox
             * --disable-gpu
             * --disable-dev-shm-usage
             * --single-process
             * 
             */

            var options = new ConnectOptions()
            {
                DefaultViewport = null,
                BrowserURL = "http://127.0.0.1:9222"
                //BrowserURL = "http://192.168.4.180:9222"
            };


            var url = "https://www.google.com/";
            using (var browser = await PuppeteerSharp.Puppeteer.ConnectAsync(options))
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url);
                    var searchElm = await page.XPathAsync("//input[@name='q']");
                    await searchElm[0].TypeAsync("hello world");
                    await page.ScreenshotAsync(@"screenshot-remote.png");
                }
                browser.Disconnect();
            }
            Console.WriteLine("done!");
        }
    }
}
