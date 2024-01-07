using Microsoft.Playwright;

namespace qpm.e2e.tests.PageObjects.Elements
{
    public class DocumentItemElement
    {
        public const string TitleXPathLocator = "//div[contains(@data-bunit-item,'content-editable')]";
        public const string DescriptionXPathLocator = "//div[@class='renderer' and @id]";
        private const string DeleteButtonXPathLocator = "//div[@class='document__item']//span[@title='Delete']";
        private const string IconXPathLocatorTemplate = "//span[contains(@style,'{0}')]/../..";

        public ILocator GetItemsOnPage(IPage page)
        {
            var piItems = page.Locator("//div[@class='document__block']");
            return piItems;
        }

        public async Task<ILocator> FillTitleAndDescription(IPage page, string piTitle, string piDescription)
        {
            var piItem = GetItemsOnPage(page).First;
            await piItem.WaitForAsync(new() { State = WaitForSelectorState.Visible });

            return await FillTitleAndDescription(piItem, piTitle, piDescription, ItemTypes.ProductIncrement);
        }

        public async Task<ILocator> FillTitleAndDescription(ILocator item, string title, string description, ItemTypes itemType)
        {
            await FillActionsAsync(item, TitleXPathLocator, title);
            await ExpandDescription(item, itemType);

            await FillActionsAsync(item, DescriptionXPathLocator, description);
            return item;
        }

        public async Task<string> GetDocumentItemTitle(ILocator item)
        {
            return await item.Locator(TitleXPathLocator).InnerTextAsync();
        }

        public async Task<string> GetDocumentItemDescription(ILocator item, ItemTypes itemType)
        {
            await ExpandDescription(item, itemType);

            return await item.Locator(DescriptionXPathLocator).First.InnerTextAsync();
        }

        public async Task ExpandItem(ILocator item, ItemTypes itemType)
        {
            switch (itemType)
            {
                case ItemTypes.ProductIncrement:
                    {
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "circle-filled")).ClickAsync();
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "circle-outline")).WaitForAsync();
                        break;
                    }
                case ItemTypes.Epic:
                    {
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "triangle-filled")).ClickAsync();
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "triangle-outline")).WaitForAsync();
                        break;
                    }
                case ItemTypes.Capability:
                    {
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "capability-item-filled")).ClickAsync();
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "capability-item-outline")).WaitForAsync();
                        break;
                    }
                case ItemTypes.Subsystem:
                    {
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "subsystem-item-filled")).ClickAsync();
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "subsystem-item-outline")).WaitForAsync();
                        break;
                    }
                case ItemTypes.None:
                    throw new NotImplementedException($"Expanding for {itemType} is not implemented");
            }
        }

        public async Task ShrinkItem(ILocator item, ItemTypes itemType)
        {
            switch (itemType)
            {
                case ItemTypes.ProductIncrement:
                    {
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "circle-outline")).ClickAsync();
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "circle-filled")).WaitForAsync();
                        break;
                    }

                case ItemTypes.Subsystem:
                    {
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "subsystem-item-outline")).ClickAsync();
                        await item.Locator(string.Format(IconXPathLocatorTemplate, "subsystem-item-filled")).WaitForAsync();
                        break;
                    }
                case ItemTypes.Epic:
                case ItemTypes.None:
                case ItemTypes.Capability:
                    throw new NotImplementedException($"Shrinking for {itemType} is not implemented");
            }
        }

        public void DeleteDocumentItems(IPage page)
        {
            var deletePIButtons = page.Locator(DeleteButtonXPathLocator);
            DeleteButtonClick(deletePIButtons);
        }

        public void DeleteDocumentItems(ILocator locator)
        {
            var deletePIButtons = locator.Locator(DeleteButtonXPathLocator);
            DeleteButtonClick(deletePIButtons);
        }

        private async Task FillActionsAsync(ILocator piItem, string innerItemXPath, string textToFill)
        {
            var piItemName = piItem.Locator(innerItemXPath).First;

            await piItemName.DblClickAsync(new() { Delay = 300 });
            await piItemName.PressAsync("Control+a", new() { Delay = 100 });
            await piItemName.PressSequentiallyAsync(textToFill);
            await piItemName.PressAsync("Enter", new() { Delay = 300 });
        }

        private static void DeleteButtonClick(ILocator deletePIButtons)
        {
            var deleteButtonsList = deletePIButtons.AllAsync()
                            .Result
                            .ToList();
            deleteButtonsList.ForEach(x => x.ClickAsync().Wait());
        }

        private async Task ExpandDescription(ILocator item, ItemTypes itemType)
        {
            if (await item.Locator(DescriptionXPathLocator).CountAsync() == 0)
            {
                await ExpandItem(item, itemType);
            }
        }
    }
}
