using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;
using qpm.e2e.tests.PageObjects;
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
            //if (AdminPage.Url == BaseUrl + ProductIncrementsPage.ResourceName)
            //{
            //    new DocumentItemElement().DeleteDocumentItems(AdminPage);
            //}




            // TODO: add remove for the PI here

        }

        [Test]
        public async Task UsersIterationTest()
        {
            const string adminName = "hinogir860@vasteron.com";
            const string password = "6T76vuEjfgs9US5";
            const string userName = "tidaver777@talmetry.com";

            var adminBrowser = new BrowserRunner();
            AdminPage = await LoadStartPage(adminName, password, adminBrowser, false);

            // admins actions for products and project activation
            var headerElement = new HeaderElement(AdminPage);
            await headerElement.ChooseFirstProduct();
            await headerElement.ChooseFirstProject(projectName: "Draft");


            // Create first PI
            var piPage = await headerElement.PIButtonClick();
            const string firstPiTitle = "First PI item";
            var plannedDateTime = DateTime.Today.AddDays(1);
            string tomorrowDateInput = plannedDateTime.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);
            const string firstPiDescription = "This is first PI item description";
            await piPage.CreatePI(firstPiTitle, firstPiDescription, tomorrowDateInput);
            const string secondPiTitle = "Second PI item";
            const string secondPiDescription = "This is second PI item description";
            await piPage.CreatePI(secondPiTitle, secondPiDescription, tomorrowDateInput);

            //Move to the Subsystems page in order to create Epics
            var subsystemPage = await SubsystemsPage.Navigate(AdminPage, BaseUrl);

            const string secondSubsystemTitle = "Second subsystem";
            const string secondSubsystemDescription = "Second subsystem description";
            var secondSubsystemItem = await subsystemPage.CreateSubsystem(secondSubsystemTitle, secondSubsystemDescription);
            await secondSubsystemItem.CreateCapability("Second capability", "Second capability description");

            const string secondEpicTitle = "Second Epic";
            const string secondEpicDescription = "Second Epic description";
            await secondSubsystemItem.CreateEpic(secondEpicTitle, secondEpicDescription, secondPiTitle);
            
            await secondSubsystemItem.Shrink();

            const string firstSubsystemTitle = "First subsystem";
            const string firstSubsystemDescription = "First subsystem description";
            var subSystemItem = await subsystemPage.CreateSubsystem(firstSubsystemTitle, firstSubsystemDescription);
            await subSystemItem.CreateCapability("First capability", "First capability description");

            const string firstEpicTitle = "First Epic";
            const string firstEpicDescription = "First Epic description";
            await subSystemItem.CreateEpic(firstEpicTitle, firstEpicDescription, firstPiTitle);

            

            


            var userBrowser = new BrowserRunner();
            var userPage = await LoadStartPage(userName, password, userBrowser, false);

            var userHeaderElement = new HeaderElement(userPage);
            await userHeaderElement.ChooseFirstProduct();
            await userHeaderElement.ChooseFirstProject(projectName: "Draft");
            
            var userPIPage = await userHeaderElement.PIButtonClick();


            var piItemElements = await userPIPage.GetPIItemElements();
            var firstPiElement = piItemElements.First();
            firstPiElement.Title.Should().Be(firstPiTitle);
            await firstPiElement.Expand();
            firstPiElement.Description.Should().Be(firstPiDescription);
            string expectedPlannedDate = plannedDateTime.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);

            firstPiElement.PlannedDate.Should().Be(expectedPlannedDate);

            await firstPiElement.ExpandEpics();
            firstPiElement.GetEpicItemTitle().Should().Contain(firstEpicTitle);
            firstPiElement.GetEpicItemDescription().Should().Be(firstEpicDescription);

            await firstPiElement.Shrink();

            var secondPiElement = piItemElements.Last();
            secondPiElement.Title.Should().Be(secondPiTitle);
            await secondPiElement.Expand();
            secondPiElement.Description.Should().Be(secondPiDescription);
            secondPiElement.PlannedDate.Should().Be(expectedPlannedDate);

            await secondPiElement.ExpandEpics();
            secondPiElement.GetEpicItemTitle().Should().Contain(secondEpicTitle);
            secondPiElement.GetEpicItemDescription().Should().Be(secondEpicDescription);
            userBrowser.Dispose();


            subSystemItem.DeleteSubsystem();
            secondSubsystemItem.Expand();
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            secondSubsystemItem.DeleteSubsystem();
            
            

            await piPage.NavigateToPage(BaseUrl);
            new DocumentItemElement().DeleteDocumentItems(AdminPage);

        }





        //TODO: move it to the StartPage object
        private static async Task<IPage> LoadStartPage(string userName, string password, BrowserRunner browserRunner, bool headlessMode = true)
        {
            var adminPage = await browserRunner.OpenInitPage(BaseUrl, headlessMode);

            await adminPage.Locator("id=signInName").FillAsync(userName);
            await adminPage.Locator("id=password").FillAsync(password);
            await adminPage.Locator("id=next").ClickAsync();

            await adminPage.WaitForLoadStateAsync();

            await adminPage.Locator("text=Product increments").WaitForAsync();

            await Assertions.Expect(adminPage).ToHaveURLAsync($"{BaseUrl}inbox");

            return adminPage;
        }
    }
}
