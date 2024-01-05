using Microsoft.Playwright;
using NUnit.Framework;
using qpm.e2e.tests.PageObjects;

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
            if (AdminPage.Url == BaseUrl + ProductIncrementsPage.ResourceName)
            {
                new DocumentItemElement().DeleteDocumentItems(AdminPage);
            }




            // TODO: add remove for the PI here

        }

        [Test]
        public async Task UsersIterationTest()
        {
            const string adminName = "hinogir860@vasteron.com";
            const string password = "6T76vuEjfgs9US5";
            const string userName = "jiyive1530@ubinert.com";

            var adminBrowser = new BrowserRunner();
            AdminPage = await LoadStartPage(adminName, password, adminBrowser);

            //TODO: assept cookies

            //var userBrowser = new BrowserRunner();
            //var userPage = await LoadStartPage(userName, password, userBrowser);

            // admins actions for products and project activation
            var headerElement = new HeaderElement(AdminPage);
            await headerElement.ChooseFirstProduct();
            await headerElement.ChooseFirstProject(projectName: "Draft");


            // Create first PI
            var piPage = await headerElement.PIButtonClick();
            const string firstPiTitle = "First PI item";
            await piPage.CreatePI(firstPiTitle, "This is first PI item description");
            await piPage.CreatePI("Second PI item", "This is second PI item description");

            //TODO: Here need to check the information about PI
            

            //Move to the Subsystems page in order to create Epics
            var subsystemPage = await SubsystemsPage.Navigate(AdminPage, BaseUrl);

            var subSystemItem = await subsystemPage.CreateSubsystem("First subsystem", "First subsystem description");
            await subSystemItem.CreateCapability("First capability", "First capability description");

            await subSystemItem.CreateEpic("First Epic", "First Epic description", firstPiTitle);

            
            
            
            
            
            
            //TODO: too quick
            subSystemItem.DeleteSubsystem();

            await piPage.NavigateToPage(BaseUrl);
            new DocumentItemElement().DeleteDocumentItems(AdminPage);

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
