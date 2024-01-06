using Microsoft.Playwright;

namespace qpm.e2e.tests
{
    public class BrowserRunner
    {
        private IPlaywright? PlaywrightInstance;
        private IBrowser? Browser;

        public async Task<IPage> OpenInitPage(string url, bool headlessMode)
        {
            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = headlessMode,
                Args = new List<string> { "--start-maximized" },
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

        public void Dispose()
        {
            Browser?.DisposeAsync();
            PlaywrightInstance?.Dispose();
        }
    }
}
