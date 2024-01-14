using Microsoft.Playwright;

namespace qpm.e2e.tests.PageObjects.Elements
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
            await Page.Locator("//div[@class='products__activator']").ClickAsync();
            await Page.Locator("//div[@class='products__list']").ClickAsync();
        }

        public async Task ChooseFirstProject(string projectName)
        {
            await Page.Locator("//div[@class='projects__activator']").ClickAsync();
            await Page.Locator($"//div[@class='projects__list']//div[text()='{projectName}']").ClickAsync();
        }

        public async Task<ProductIncrementsPage> PIButtonClick()
        {
            await Page.Locator("text=Product increments").First.ClickAsync();
            return new ProductIncrementsPage(Page);
        }
    }
}
