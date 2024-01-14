using Microsoft.Playwright;
using System.Diagnostics;

namespace qpm.e2e.tests.PageObjects
{
    public abstract class BasePage
    {
        protected IPage _page;

        protected BasePage(IPage page)
        {
            _page = page;
        }

        protected void AcceptCookies()
        {
            var cookiesContainer = _page.Locator("//div[@class='cc-isolation-container']");
            if (cookiesContainer.CountAsync().Result > 0)
            {
                cookiesContainer.Locator("//button[text()='Accept all']").ClickAsync().Wait();
            }
        }

        protected async Task WaitForElementsPresense(string locator, int elementsCount)
        {
            var stopwatch = Stopwatch.StartNew();
            while (true)
            {
                if (stopwatch.ElapsedMilliseconds > TimeSpan.FromSeconds(10).TotalMilliseconds |
                    await _page.Locator(locator).CountAsync() == elementsCount)
                {
                    stopwatch.Stop();
                    break;
                }
                await Task.Delay(300);
            }
        }
    }
}
