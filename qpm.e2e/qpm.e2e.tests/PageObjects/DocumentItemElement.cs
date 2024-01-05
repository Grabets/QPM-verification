using Microsoft.Playwright;

namespace qpm.e2e.tests.PageObjects
{
    public class DocumentItemElement
    {
        public async Task<ILocator> FillTitleAndDescription(IPage page, string piTitle, string piDescription)
        {
            var piItem = page.Locator("//div[@class='document__block']").First;
            await piItem.WaitForAsync(new() { State = WaitForSelectorState.Visible });

            return await FillTitleAndDescription(piItem, piTitle, piDescription);
        }

        public async Task<ILocator> FillTitleAndDescription(ILocator item, string title, string description)
        {
            const string titleXPathLocator = "//div[contains(@data-bunit-item,'content-editable')]";
            const string descriptionXPathLocator = "//div[@class='renderer' and @id]";

            await FillActionsAsync(item, titleXPathLocator, title);

            // TODO: make it with switch for item type
            if (await item.Locator(descriptionXPathLocator).CountAsync() == 0)
            {
                await item.Locator("//span[contains(@style,'circle-filled')]/../..").ClickAsync();
            }

            await FillActionsAsync(item, descriptionXPathLocator, description);
            return item;
        }

        private async Task FillActionsAsync(ILocator piItem, string innerItemXPath, string textToFill)
        {
            var piItemName = piItem.Locator(innerItemXPath).First;

            await piItemName.DblClickAsync(new() { Delay = 300 });
            await piItemName.PressAsync("Control+a", new() { Delay = 50 });
            await piItemName.PressSequentiallyAsync(textToFill);

            await piItemName.PressAsync("Enter", new() { Delay = 300 });
        }

        public void DeleteDocumentItems(IPage page)
        {
            var deletePIButtons = page.Locator("//div[@class='document__item']//span[@title='Delete']");
            DeleteButtonClick(deletePIButtons);
        }

        public void DeleteDocumentItems(ILocator locator)
        {
            var deletePIButtons = locator.Locator("//div[@class='document__item']//span[@title='Delete']");
            DeleteButtonClick(deletePIButtons);
        }

        private static void DeleteButtonClick(ILocator deletePIButtons)
        {
            var deleteButtonsList = deletePIButtons.AllAsync()
                            .Result
                            .ToList();
            deleteButtonsList.ForEach(x => x.ClickAsync().Wait());
        }
    }

    public enum ItemTypes
    {
        None = 0,
        ProductIncrement,
        Subsystem,
        Capability,
        Epic,
    }
}
