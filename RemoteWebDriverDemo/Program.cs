using System;
using System.Collections.Generic;
using System.Net;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.Recaptcha;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha;
using PuppeteerSharp;

namespace RemoteWebDriverDemo
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
     * --lang="en_US"
     * --user-agent="Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4892.0 Safari/537.36"
     * --window-position=x,y
     * --window-size=w,h
     * 
     */

    internal class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            Console.WriteLine("Hello World!");

            //BasicUsage();
            RemoteUsage();

            //RecaptchaUsage();


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
                Headless = false
            });
            var page = await browser.NewPageAsync();
            //await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>() { { "DNT","1"} });
            await page.SetRequestInterceptionAsync(true);
            await page.GoToAsync("https://whoer.net/");
            await page.ScreenshotAsync(@"screenshot.png");

            Console.WriteLine("done!");
        }

        /// <summary>
        /// Connect to a remote browser
        /// </summary>
        static async void RemoteUsage()
        {
            // launch chrome on windows via chrome.exe --remote-debugging-port=9222
            // and go to http://localhost:9222/json/version


            var options = new ConnectOptions()
            {
                DefaultViewport = null, // fullscreen
                IgnoreHTTPSErrors = true,
                BrowserURL = "http://127.0.0.1:9222"
                //BrowserURL = "http://192.168.4.180:9222"
            };


            var url = "https://www.whoer.net/";
            //var url = "https://www.google.com/";
            using (var browser = await Puppeteer.ConnectAsync(options))
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url);
                    //var searchElm = await page.XPathAsync("//input[@name='q']");
                    //await searchElm[0].TypeAsync("hello world");
                    await page.ScreenshotAsync(@"screenshot-remote.png");
                }
                await browser.CloseAsync();
                browser.Disconnect();
            }
            Console.WriteLine("done!");
        }

        static async void RecaptchaUsage()
        {
            // Initialize recaptcha plugin with AntiCaptchaProvider
            var recaptchaPlugin = new RecaptchaPlugin(new TwoCaptcha("2captcha token"));
            var browser = await new PuppeteerExtra().Use(recaptchaPlugin).ConnectAsync(new ConnectOptions()
            {
                BrowserURL = "http://127.0.0.1:9222"
            });

            var page = await browser.NewPageAsync();
            await page.GoToAsync("https://patrickhlauke.github.io/recaptcha/");
            // Solves captcha in page!
            await recaptchaPlugin.SolveCaptchaAsync(page);

            Console.WriteLine("done");
        }


    }
}
