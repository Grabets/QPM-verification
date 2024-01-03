using Microsoft.Playwright;
using NUnit.Framework;
using System.Globalization;

namespace qpm.e2e.tests
{
    [TestFixture]
    public class SanityTests
    {
        const string BaseUrl = "https://app.qpm.digital/";
        private IPage AdminPage { get; set; }
        private IPage UserPage { get; set; }


        [TearDown]
        public void TearDown()
        {
            //Add here browser runner dispose
            // https://temp-mail.org/


            // TODO: add remove for the PI here
            var deletePIButtons = AdminPage.Locator("xpath=//div[@class='document__item']//span[@title='Delete']");
            var deletePiButtonsList = deletePIButtons.AllAsync()
                .Result
                .ToList();
            deletePiButtonsList.ForEach(x => x.ClickAsync().Wait());
        }

        [Test]
        public async Task UsersIterationTest()
        {
            const string adminName = "hinogir860@vasteron.com";
            const string password = "6T76vuEjfgs9US5";
            const string userName = "jiyive1530@ubinert.com";

            var adminBrowser = new BrowserRunner();
            AdminPage = await LoadStartPage(adminName, password, adminBrowser);

            //var userBrowser = new BrowserRunner();
            //var userPage = await LoadStartPage(userName, password, userBrowser);

            // admins actions for products and project activation
            await AdminPage.Locator("xpath=//div[@class='products__activator']").ClickAsync();
            await AdminPage.Locator("xpath=//div[@class='products__list']").ClickAsync();
                  
            await AdminPage.Locator("xpath=//div[@class='projects__activator']").ClickAsync();
            await AdminPage.Locator("xpath=//div[@class='projects__list']//div[text()='Draft']").ClickAsync();

            //await adminPage.Locator("text=Create product increment").ClickAsync();

            // Create first PI
            await AdminPage.Locator("text=Product increments").ClickAsync();
            await CreatePI("First PI item", "This is first PI item description");

            // Create second PI
            //await CreatePI("Second PI item", "This is second PI item description");


        }



        private async Task CreatePI(string piTitle, string piDescription)
        {
            
            await AdminPage.Locator("//button[text()='Create product increment']").ClickAsync(new() { Delay = 300});

            var piItem = AdminPage.Locator("xpath=//div[@class='document__item']");//"xpath=//div[@style='margin-bottom: 8px;'][1]//div[@class='document__item']"
            await piItem.WaitForAsync(new() { State = WaitForSelectorState.Visible});
            //piItem.Locator("xpath=//button[@type='button']").ClickAsync();
            await FillActionsAsync(piItem, "xpath=//div[@data-bunit-item]", piTitle);
            await FillActionsAsync(piItem, "xpath=//div[@class='renderer' and @id]", piDescription);

            await piItem.Locator("xpath=//div[contains(@class,'DatePicker')]//input").ClickAsync();

            //TODO: We can have a problem with datepicker on the last day of the month.
            await AdminPage.Locator("xpath=//div[contains(@class,'picker-calendar')]").Locator($"xpath=//button[contains(@aria-label, '{DateTime.Today.AddDays(1).ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture)}')]").ClickAsync();

            await piItem.Locator("xpath=//span[@title='Update']").ClickAsync();
        }

        private async Task FillActionsAsync(ILocator piItem, string innerItemXPath, string textToFill)
        {
            await Task.Delay(500);
            var piItemName = piItem.Locator(innerItemXPath);
            await piItemName.DblClickAsync(new() { Delay = 300 });
            await piItemName.PressAsync("Control+a", new() { Delay = 50 });
            await piItemName.PressSequentiallyAsync(textToFill);
            await piItem.Locator("xpath=//span[@title='Update']").ClickAsync(new() { Delay = 300 });
            //await piItemName.PressAsync("Enter", new() { Delay = 300 });
            //await piItem.Locator("xpath=//span[@title='Update']").ClickAsync(new() { Delay = 300 });
        }

        private static async Task<IPage> LoadStartPage(string adminName, string password, BrowserRunner adminBrowser)
        {
            var adminPage = await adminBrowser.OpenInitPage(BaseUrl);

            await adminPage.Locator("id=signInName").FillAsync(adminName);
            await adminPage.Locator("id=password").FillAsync(password);
            await adminPage.Locator("id=next").ClickAsync();

            await adminPage.WaitForLoadStateAsync();

            await adminPage.Locator("text=Product increments").WaitForAsync();

            await Assertions.Expect(adminPage).ToHaveURLAsync($"{BaseUrl}inbox");

            return adminPage;
        }
    }
}
