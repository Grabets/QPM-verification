using Microsoft.Playwright;

namespace qpm.e2e.tests.PageObjects.Elements
{
    public class SubsystemElement
    {
        private const string CapabilityBlockSelector = "//h3[text()='Capabilities']/ancestor::div[contains(@class,'document__branch')]";
        private const string EpicsBlockSelector = "//h3[text()='Epics']/ancestor::div[contains(@class,'document__branch')][1]";
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
            var capabilityBlock = SubsystemsItem.Locator(CapabilityBlockSelector);
            CapabilityItem = await new DocumentItemElement()
            .FillTitleAndDescription(capabilityBlock, capabilityName, capabilityDescription, ItemTypes.Capability);
        }

        public async Task CreateEpic(string epicName, string epicDescription)
        {
            if (CapabilityItem == null)
            {
                throw new NullReferenceException($"The {CapabilityItem} not initialized");
            }
            
            await CapabilityItem.Locator(EpicsBlockSelector + "//span[@title='Create new']").ClickAsync();
            var epicsBlock = CapabilityItem.Locator(EpicsBlockSelector);
            EpicItem = await new DocumentItemElement()
            .FillTitleAndDescription(epicsBlock, epicName, epicDescription, ItemTypes.Epic);

            var piAssignButton = EpicItem.Locator("//div[@class='btns']//span[text()='Add Product Increment']");
            await piAssignButton.ClickAsync();
        }

        public async Task DeleteSubsystem()
        {
            var docItemElement = new DocumentItemElement();
            await docItemElement.DeleteDocumentItems(EpicItem);
            Task.Delay(500).Wait(); // TODO: Need to find some explicit wait for item deletion
            await docItemElement.DeleteDocumentItems(CapabilityItem);
            Task.Delay(500).Wait(); // TODO: Need to find some explicit wait for item deletion
            await docItemElement.DeleteDocumentItems(SubsystemsItem);
        }

        public async Task Shrink()
        {
            await new DocumentItemElement().ShrinkItem(SubsystemsItem, ItemTypes.Subsystem);
        }

        public async Task Expand()
        {
            await new DocumentItemElement().ExpandItem(SubsystemsItem, ItemTypes.Subsystem);
        }

        public async Task SafelyExpandAll()
        {
            try
            {
                await new DocumentItemElement().ExpandItem(SubsystemsItem, ItemTypes.Subsystem);

                CapabilityItem = SubsystemsItem.Locator(CapabilityBlockSelector);
                await new DocumentItemElement().ExpandItem(CapabilityItem, ItemTypes.Capability);

                var capabilityInnerItem = CapabilityItem.Locator(DocumentItemElement.DocumentBlockXPathLocator);
                await new DocumentItemElement().ExpandItem(capabilityInnerItem, ItemTypes.Capability);

                EpicItem = CapabilityItem.Locator(EpicsBlockSelector);
                await new DocumentItemElement().ExpandItem(CapabilityItem, ItemTypes.Epic);

                var epicInnerItem = CapabilityItem.Locator(DocumentItemElement.DocumentBlockXPathLocator);
                await new DocumentItemElement().ExpandItem(epicInnerItem, ItemTypes.Epic);
            }
            catch (Exception)
            {
                // TODO: Here we need to add Logging with Error level to print this issue.
            }
        }
    }
}
