using Microsoft.Playwright;
using qpm.e2e.tests.PageObjects.Elements;

namespace qpm.e2e.tests.PageObjects
{
    public class SubsystemsPage
    {
        private IPage Page { get; set; }

        public SubsystemsPage(IPage page)
        {
            Page = page;
        }

        public static async Task<SubsystemsPage> Navigate(IPage page, string baseUrl)
        {
            var url = baseUrl + "subsystems";
            await page.GotoAsync(url);
            await page.Locator("//button[text()='Create subsystem']")
                .WaitForAsync(new() { State = WaitForSelectorState.Visible });

            //TODO: move to BasePage
            var cookiesContainer = page.Locator("//div[@class='cc-isolation-container']");
            if (cookiesContainer.CountAsync().Result > 0)
            {
                cookiesContainer.Locator("//button[text()='Accept all']").ClickAsync().Wait();
            }

            return new SubsystemsPage(page);
        }

        public async Task<SubsystemElement> CreateSubsystem(string subSystemName, string subSystemDescription)
        {
            await Page.Locator("//button[text()='Create subsystem']").ClickAsync();

            //TODO: need to find more sophisticated way. Here should be some explisit wait.
            Task.Delay(3000).Wait();
            ILocator subsystemsItem = await new DocumentItemElement()
                .FillTitleAndDescription(Page, subSystemName, subSystemDescription);

            return new SubsystemElement(Page, subsystemsItem);
        }
    }
}
