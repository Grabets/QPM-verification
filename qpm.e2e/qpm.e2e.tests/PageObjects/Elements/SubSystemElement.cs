using Microsoft.Playwright;

namespace qpm.e2e.tests.PageObjects.Elements
{
    public class SubsystemElement
    {
        private readonly ILocator SubsystemsItem;
        private ILocator CapabilityItem;
        private ILocator EpicItem;

        public SubsystemElement(ILocator subsystemsItem)
        {
            SubsystemsItem = subsystemsItem;
        }

        public async Task CreateCapability(string capabilityName, string capabilityDescription)
        {
            await SubsystemsItem.Locator("//span[@title='Create new']").ClickAsync();
            var capabilityBlock = SubsystemsItem.Locator("//h3[text()='Capabilities']/ancestor::div[contains(@class,'document__branch')]");
            CapabilityItem = await new DocumentItemElement()
            .FillTitleAndDescription(capabilityBlock, capabilityName, capabilityDescription, ItemTypes.Capability);
        }

        public async Task CreateEpic(string epicName, string epicDescription)
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
        }

        public void DeleteSubsystem()
        {
            var docItemElement = new DocumentItemElement();
            docItemElement.DeleteDocumentItems(EpicItem);
            Task.Delay(300).Wait(); // TODO: Need to find some explicit wait for item deletion
            docItemElement.DeleteDocumentItems(CapabilityItem);
            Task.Delay(300).Wait(); // TODO: Need to find some explicit wait for item deletion
            docItemElement.DeleteDocumentItems(SubsystemsItem);
        }

        public async Task Shrink()
        {
            await new DocumentItemElement().ShrinkItem(SubsystemsItem, ItemTypes.Subsystem);
        }

        public async void Expand()
        {
            await new DocumentItemElement().ExpandItem(SubsystemsItem, ItemTypes.Subsystem);
        }
    }
}
