using Microsoft.Playwright;

namespace qpm.e2e.tests.PageObjects.Elements
{
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
