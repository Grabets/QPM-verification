using Microsoft.Playwright;

namespace qpm.e2e.tests
{
    public class BrowserRunner
    {
        public IPlaywright? PlaywrightInstance { get; set; }
        public IBrowser? Browser { get; set; }

        public async Task<IPage> OpenInitPage(string url)
        {
            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = false,
                Args = new List<string> { "--start-maximized" },
                //SlowMo = 50,
                
            };

            PlaywrightInstance = await Playwright.CreateAsync();

            Browser = await PlaywrightInstance.Chromium.LaunchAsync(launchOptions);

            var context = await Browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    ViewportSize = ViewportSize.NoViewport
                });

            var page = await context.NewPageAsync();

            await page.GotoAsync(url);
            return page;
        }
    }
}
