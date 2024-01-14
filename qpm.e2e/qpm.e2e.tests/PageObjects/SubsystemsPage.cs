using Microsoft.Playwright;
using qpm.e2e.tests.PageObjects.Elements;

namespace qpm.e2e.tests.PageObjects
{
    public class SubsystemsPage : BasePage
    {
        private const string CreateSubsystemButtonLocator = "//button[text()='Create subsystem']";

        public SubsystemsPage(IPage page) : base(page)
        {
            AcceptCookies();
        }

        public static async Task<SubsystemsPage> Navigate(IPage page, string baseUrl)
        {
            var url = baseUrl + "subsystems";
            await page.GotoAsync(url);
            await page.Locator(CreateSubsystemButtonLocator)
                .WaitForAsync(new() { State = WaitForSelectorState.Visible });

            return new SubsystemsPage(page);
        }

        public async Task<SubsystemElement> CreateSubsystem(string subSystemName, string subSystemDescription)
        {
            await _page.Locator(CreateSubsystemButtonLocator).ClickAsync();

            await Task.Delay(TimeSpan.FromSeconds(3)); //TODO: need to find more sophisticated way. Here should be some explicit wait.
            ILocator subsystemsItem = await new DocumentItemElement()
                .FillTitleAndDescription(_page, subSystemName, subSystemDescription);

            return new SubsystemElement(subsystemsItem);
        }

        public List<SubsystemElement> GetSubsystemElementsOnPage()
        {
            var subsystemsLocator = new DocumentItemElement().GetItemsOnPage(_page);

            var subsystemsList = subsystemsLocator.AllAsync()
                           .Result
                           .Select(x => new SubsystemElement(x))
                           .ToList();
            return subsystemsList;
        }
    }
}
