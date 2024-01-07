using Microsoft.Playwright;
using qpm.e2e.tests.PageObjects.Elements;

namespace qpm.e2e.tests.PageObjects
{
    public class SubsystemsPage : BasePage
    {
        public SubsystemsPage(IPage page) : base(page)
        {
            AcceptCookies();
        }

        public static async Task<SubsystemsPage> Navigate(IPage page, string baseUrl)
        {
            var url = baseUrl + "subsystems";
            await page.GotoAsync(url);
            await page.Locator("//button[text()='Create subsystem']")
                .WaitForAsync(new() { State = WaitForSelectorState.Visible });

            return new SubsystemsPage(page);
        }

        public async Task<SubsystemElement> CreateSubsystem(string subSystemName, string subSystemDescription)
        {
            await _page.Locator("//button[text()='Create subsystem']").ClickAsync();

            Task.Delay(3000).Wait(); //TODO: need to find more sophisticated way. Here should be some explicit wait.
            ILocator subsystemsItem = await new DocumentItemElement()
                .FillTitleAndDescription(_page, subSystemName, subSystemDescription);

            return new SubsystemElement(subsystemsItem);
        }
    }
}
