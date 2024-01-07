using Microsoft.Playwright;

namespace qpm.e2e.tests.PageObjects.Elements
{
    public class ProductIncrementItemElement
    {
        //TODO: better to make with EpicItem as this xpath dublicates
        private const string EpicsParentXPathSelector = "//h3[text()='Epics']/ancestor::div[contains(@class,'document__branch')][1]";

        public string Title => new DocumentItemElement().GetDocumentItemTitle(_item).Result;
        public string Description => GetItemDescription().Result;
        public string PlannedDate => GetPlannedDate().Result;

        private ILocator _item;
        private ILocator _epicItem;

        public ProductIncrementItemElement(ILocator item)
        {
            _item = item;
        }

        public async Task Expand()
        {
            await new DocumentItemElement().ExpandItem(_item, ItemTypes.ProductIncrement);
        }

        public async Task Shrink()
        {
            await new DocumentItemElement().ShrinkItem(_item, ItemTypes.ProductIncrement);
        }

        private async Task<string> GetItemDescription()
        {
            return await new DocumentItemElement().GetDocumentItemDescription(_item, ItemTypes.ProductIncrement);
        }

        private async Task<string> GetPlannedDate()
        {
            return await _item.Locator("//div[@title='Planned Date']/..//div[@class='renderer']").First.InnerTextAsync();
        }

        /// <summary>
        /// Method expand epics block. Works only with one epics.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task ExpandEpics()
        {
            var epics = _item.Locator(EpicsParentXPathSelector);
            var innerText = await epics.InnerTextAsync();
            if (!innerText.Contains("(1)"))
                throw new NullReferenceException("Epics not found");

            await epics.Locator("//span[contains(@style,'triangle-filled')]").ClickAsync();
            await epics.Locator("//span[contains(@style,'triangle-outline')]").WaitForAsync();

            await new DocumentItemElement().ExpandItem(epics, ItemTypes.Epic);

            _epicItem = epics.Locator("//div[@class='document__block']");
        }

        public string GetEpicItemTitle()
        {
            return _epicItem.Locator(DocumentItemElement.TitleXPathLocator).First.InnerTextAsync().Result;
        }

        public string GetEpicItemDescription()
        {
            return _epicItem.Locator(DocumentItemElement.TitleXPathLocator).Nth(1).InnerTextAsync().Result;
        }
    }
}
