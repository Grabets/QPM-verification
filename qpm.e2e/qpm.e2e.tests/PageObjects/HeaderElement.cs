using Microsoft.Playwright;

namespace qpm.e2e.tests.PageObjects
{
    public class HeaderElement
    {
        private IPage Page { get; set; }

        public HeaderElement(IPage page)
        {
            Page = page;
        }

        public async Task ChooseFirstProduct()
        {
            await Page.Locator("xpath=//div[@class='products__activator']").ClickAsync();
            await Page.Locator("xpath=//div[@class='products__list']").ClickAsync();
        }

        public async Task ChooseFirstProject(string projectName)
        {
            await Page.Locator("xpath=//div[@class='projects__activator']").ClickAsync();
            await Page.Locator($"xpath=//div[@class='projects__list']//div[text()='{projectName}']").ClickAsync();
        }

        internal async Task<ProductIncrementsPage> PIButtonClick()
        {
            await Page.Locator("text=Product increments").ClickAsync();
            return new ProductIncrementsPage(Page);
        }
    }
}
