using Microsoft.Playwright;
using qpm.e2e.tests.PageObjects.Elements;

namespace qpm.e2e.tests.PageObjects
{
    public class ProductIncrementsPage : BasePage
    {
        public const string ResourceName = "product-increments";

        public ProductIncrementsPage(IPage page) : base(page)
        {
            AcceptCookies();
        }

        public async Task CreatePI(string piTitle, string piDescription, string plannedDate)
        {
            await _page.Locator("//button[text()='Create product increment']").ClickAsync();

            Task.Delay(3000).Wait(); //TODO: Should be some explicit wait.
            ILocator piItem = await new DocumentItemElement()
                .FillTitleAndDescription(_page, piTitle, piDescription);

            await ChooseDateInDatePicker(piItem, plannedDate);
        }

        public async Task NavigateToPage(string baseUrl)
        {
            var url = baseUrl + ResourceName;
            await _page.GotoAsync(url);
            await WaitForElementsPresense(DocumentItemElement.TitleXPathLocator, elementsCount: 2);
        }

        public async Task<List<ProductIncrementItemElement>> GetPIItemElements()
        {
            await WaitForElementsPresense(DocumentItemElement.TitleXPathLocator, elementsCount: 2);

            var piItemLocators = new DocumentItemElement().GetItemsOnPage(_page);
            var itemsCount = await piItemLocators.CountAsync();
            var itemElements = new List<ProductIncrementItemElement>();
            for (var i = 0; i < itemsCount; i++)
            {
                itemElements.Add(new ProductIncrementItemElement(piItemLocators.Nth(i)));
            }
            return itemElements;
        }

        private async Task ChooseDateInDatePicker(ILocator piItem, string plannedDate)
        {
            await piItem.Locator("//div[contains(@class,'DatePicker')]//input").ClickAsync();

            //TODO: We can have a problem with datepicker on the last day of the month.
            await _page.Locator("//div[contains(@class,'picker-calendar')]")
                .Locator($"//button[contains(@aria-label, '{plannedDate}')]")
                .ClickAsync();

            await piItem.Locator("//span[@title='Update']").ClickAsync();
        }
    }
}
