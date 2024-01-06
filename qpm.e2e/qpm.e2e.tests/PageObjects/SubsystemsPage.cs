using Microsoft.Playwright;
using System.Net;

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

        public class SubsystemElement
        {
            private IPage Page { get; set; }
            private readonly ILocator SubsystemsItem;
            private ILocator CapabilityItem;
            private ILocator EpicItem;

            public SubsystemElement(IPage page, ILocator subsystemsItem)
            {
                Page = page;
                SubsystemsItem = subsystemsItem;
            }

            internal async Task CreateCapability(string capabilityName, string capabilityDescription)
            {
                await SubsystemsItem.Locator("//span[@title='Create new']").ClickAsync();
                var capabilityBlock = SubsystemsItem.Locator("//h3[text()='Capabilities']/ancestor::div[contains(@class,'document__branch')]");
                CapabilityItem = await new DocumentItemElement()
                .FillTitleAndDescription(capabilityBlock, capabilityName, capabilityDescription, ItemTypes.Capability);
            }

            internal async Task CreateEpic(string epicName, string epicDescription, string piName)
            {
                if (CapabilityItem == null)
                {
                    throw new NullReferenceException($"The {CapabilityItem} not initialized");
                }
                const string epicsParentSelector = "//h3[text()='Epics']/ancestor::div[contains(@class,'document__branch')][1]";

                await CapabilityItem.Locator(epicsParentSelector + "//span[@title='Create new']").ClickAsync();
                var epicsBlock = CapabilityItem.Locator(epicsParentSelector);
                EpicItem = await new DocumentItemElement()
                .FillTitleAndDescription(epicsBlock, epicName, epicDescription, ItemTypes.Epic);

                var piAssignButton = EpicItem.Locator("//div[@class='btns']/div[text()='Add Product Increment']");
                await piAssignButton.ClickAsync();

                //TODO: not good
                var sidebar = Page.Locator("//div[@class='sidebar is-enabled']").Last;
                await sidebar.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                await sidebar.Locator($"//span[text()='{piName}']/..").ClickAsync();
                await sidebar.Locator("//button[text()='Save']").ClickAsync();
            }

            internal void DeleteSubsystem()
            {
                var docItemElement = new DocumentItemElement();
                docItemElement.DeleteDocumentItems(EpicItem);
                Task.Delay(300).Wait();
                docItemElement.DeleteDocumentItems(CapabilityItem);
                Task.Delay(300).Wait();
                docItemElement.DeleteDocumentItems(SubsystemsItem);
            }

            internal async Task Shrink()
            {
                await new DocumentItemElement().ShrinkItem(SubsystemsItem, ItemTypes.Subsystem);
            }

            internal async void Expand()
            {
                await new DocumentItemElement().ExpandItem(SubsystemsItem, ItemTypes.Subsystem);
            }
        }


    }
}
