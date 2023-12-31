using Microsoft.Playwright;
using NUnit.Framework;

namespace qpm.e2e.tests
{
    [TestFixture]
    public class SanityTests
    {
        const string BaseUrl = "https://app.qpm.digital/";


        [TearDown]
        public void TearDown()
        {
            // https://temp-mail.org/
        }

        [Test]
        public async Task UsersIterationTest()
        {
            const string adminName = "hinogir860@vasteron.com";
            const string password = "6T76vuEjfgs9US5";
            const string userName = "jiyive1530@ubinert.com";

            var adminBrowser = new BrowserRunner();
            var adminPage = await LoadStartPage(adminName, password, adminBrowser);

            var userBrowser = new BrowserRunner();
            var userPage = await LoadStartPage(userName, password, userBrowser);
            await adminPage.Locator("text=Product increments").ClickAsync();
            await userPage.Locator("text=Product increments").ClickAsync();
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
