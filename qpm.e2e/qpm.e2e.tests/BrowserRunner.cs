using Microsoft.Playwright;

namespace qpm.e2e.tests
{
    public class BrowserRunner : IDisposable
    {
        private IPlaywright? PlaywrightInstance;
        private IBrowser? Browser;
        private IBrowserContext? BrowserContext;

        public async Task<IPage> OpenInitPage(string url)
        {
            var launchOptions = new BrowserTypeLaunchOptions
            {
                //Headless = false,
                Args = new List<string> { "--start-maximized" },

            };

            PlaywrightInstance = await Playwright.CreateAsync();

            Browser = await PlaywrightInstance.Chromium.LaunchAsync(launchOptions);

            BrowserContext = await Browser.NewContextAsync(
                new BrowserNewContextOptions
                {

                    ViewportSize = new ViewportSize
                    {
                        Height = 1080,
                        Width = 1920
                    },
                    RecordVideoDir = "videos/",
                    RecordVideoSize = new RecordVideoSize() { Width = 1920, Height = 1080 },
                    
                });

            BrowserContext.SetDefaultNavigationTimeout(60000);

            var page = await BrowserContext.NewPageAsync();

            await page.GotoAsync(url);
            return page;
        }

        public void Dispose()
        {
            BrowserContext?.CloseAsync().Wait();
            Browser?.CloseAsync().Wait();
            PlaywrightInstance?.Dispose();
        }
    }
}
