using Microsoft.Playwright;
using System.Globalization;

namespace qpm.e2e.tests.PageObjects
{
    public class ProductIncrementsPage
    {
        public const string ResourceName = "product-increments";
        private IPage Page { get; set; }
        
        public ProductIncrementsPage(IPage page)
        {
            Page = page;
        }

        public async Task CreatePI(string piTitle, string piDescription)
        {
            await Page.Locator("//button[text()='Create product increment']").ClickAsync();

            //TODO: need to find more sophisticated way. Here should be some explisit wait.
            Task.Delay(3000).Wait();
            ILocator piItem = await new DocumentItemElement().FillTitleAndDescription(Page, piTitle, piDescription);

            await ChooseDateInDatePicker(piItem);
        }

        public async Task NavigateToPage(string baseUrl)
        {
            //TODO: need parent
            var url = baseUrl + ResourceName;
            await Page.GotoAsync(url);
        }

        private async Task ChooseDateInDatePicker(ILocator piItem)
        {
            await piItem.Locator("xpath=//div[contains(@class,'DatePicker')]//input").ClickAsync();

            //TODO: We can have a problem with datepicker on the last day of the month.
            string tomorrowDate = DateTime.Today.AddDays(1).ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);

            await Page.Locator("xpath=//div[contains(@class,'picker-calendar')]")
                .Locator($"xpath=//button[contains(@aria-label, '{tomorrowDate}')]")
                .ClickAsync();

            await piItem.Locator("xpath=//span[@title='Update']").ClickAsync();
        }
    }
}
