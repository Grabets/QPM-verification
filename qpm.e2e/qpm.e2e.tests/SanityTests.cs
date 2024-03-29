﻿using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Internal;
using qpm.e2e.tests.PageObjects;
using qpm.e2e.tests.PageObjects.Elements;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text;

namespace qpm.e2e.tests
{
    [TestFixture]
    public class SanityTests
    {
        private const string BaseUrl = "https://app.qpm.digital/";
        private const string AdminName = "hinogir860@vasteron.com";
        private const string UserName = "tidaver777@talmetry.com";
        private const string Password = "NlQ3NnZ1RWpmZ3M5VVM1";

        private const string FirstPiTitle = "First PI item";
        private const string FirstPiDescription = "This is first PI item description";
        private const string SecondPiTitle = "Second PI item";
        private const string SecondPiDescription = "This is second PI item description";
        private static DateTime PiPlannedDateTime = DateTime.Today.AddDays(1);

        private const string FirstSubsystemTitle = "First subsystem";
        private const string FirstSubsystemDescription = "First subsystem description";
        private const string SecondSubsystemTitle = "Second subsystem";
        private const string SecondSubsystemDescription = "Second subsystem description";
        private const string FirstCapabilityName = "First capability";
        private const string FirstCapabilityDescription = "First capability description";
        private const string SecondCapabilityName = "Second capability";
        private const string SecondCapabilityDescription = "Second capability description";
        private const string FirstEpicTitle = "First Epic";
        private const string FirstEpicDescription = "First Epic description";
        private const string SecondEpicTitle = "Second Epic";
        private const string SecondEpicDescription = "Second Epic description";

        private IPage _adminPage;
        private ProductIncrementsPage _piPage;
        private SubsystemElement? _subSystemItem;
        private SubsystemElement? _secondSubSystemItem;
        private string _decodedPassword;

        [SetUp]
        public async Task SetUp()
        {
            var startAdminPage = new StartPage(BaseUrl);

            byte[] data = Convert.FromBase64String(Password);
            _decodedPassword = Encoding.UTF8.GetString(data);

            var startAdminPageTask = startAdminPage.LoadStartPage(AdminName, _decodedPassword);

            _adminPage = startAdminPageTask.Result;
            var adminHeaderElement = startAdminPage.ChooseDefaultSettings(_adminPage).Result;

            await CleanSubsystemsIfExist();
            await CleanProductIncrementsIfExist();

            _piPage = await adminHeaderElement.PIButtonClick();
        }

        [TearDown]
        public async Task TearDown()
        {
            await CleanSubsystemsIfExist();

            await CleanProductIncrementsIfExist();
        }

        [Test]
        public async Task AdminCreatesPIsAndEpics_UserVerifiesData()
        {
            // Arrange
            var startUserPage = new StartPage(BaseUrl);
            var startUserPageTask = startUserPage.LoadStartPage(UserName, _decodedPassword);

            //Act
                //// Going to create two unique product increments as administator 
            string tomorrowDateInput = PiPlannedDateTime.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);
            await _piPage.CreatePI(FirstPiTitle, FirstPiDescription, tomorrowDateInput);
            await _piPage.CreatePI(SecondPiTitle, SecondPiDescription, tomorrowDateInput);

                //// Going to create two unique subsystems with filled capability and epics as administrator
            var subsystemPage = await SubsystemsPage.Navigate(_adminPage, BaseUrl);

            _secondSubSystemItem = await subsystemPage.CreateSubsystem(SecondSubsystemTitle, SecondSubsystemDescription);
            await _secondSubSystemItem.CreateCapability(SecondCapabilityName, SecondCapabilityDescription);
            await _secondSubSystemItem.CreateEpic(SecondEpicTitle, SecondEpicDescription);
            var sidebarElement = new SidebarElement();
            await sidebarElement.ChooseElementsInSideBar(_adminPage, SecondPiTitle);

            await _secondSubSystemItem.Shrink();

            _subSystemItem = await subsystemPage.CreateSubsystem(FirstSubsystemTitle, FirstSubsystemDescription);
            await _subSystemItem.CreateCapability(FirstCapabilityName, FirstCapabilityDescription);
            await _subSystemItem.CreateEpic(FirstEpicTitle, FirstEpicDescription);
            await sidebarElement.ChooseElementsInSideBar(_adminPage, FirstPiTitle);

                //// Going to open product increment page as user
            var userPage = await startUserPageTask;
            var userHeaderElement = await startUserPage.ChooseDefaultSettings(userPage);

            var userPIPage = await userHeaderElement.PIButtonClick();

            string expectedPlannedDate = PiPlannedDateTime.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);

            var piItemElements = await userPIPage.GetPIItemElements();
            
            //Assert
                //// Going to assert first product increment with epic data
            var firstPiElement = piItemElements.First();
            using (new AssertionScope())
            {
                firstPiElement.Title.Should().Be(FirstPiTitle);
                await firstPiElement.Expand();
                firstPiElement.Description.Should().Be(FirstPiDescription);
                firstPiElement.PlannedDate.Should().Be(expectedPlannedDate);
                await firstPiElement.ExpandEpics();
                firstPiElement.GetEpicItemTitle().Should().Contain(FirstEpicTitle);
                firstPiElement.GetEpicItemDescription().Should().Be(FirstEpicDescription);
            }
            await firstPiElement.Shrink();

                //// Going to assert second product increment with epic data
            var secondPiElement = piItemElements.Last();
            using (new AssertionScope())
            {
                secondPiElement.Title.Should().Be(SecondPiTitle);
                await secondPiElement.Expand();
                secondPiElement.Description.Should().Be(SecondPiDescription);
                secondPiElement.PlannedDate.Should().Be(expectedPlannedDate);
                await secondPiElement.ExpandEpics();
                secondPiElement.GetEpicItemTitle().Should().Contain(SecondEpicTitle);
                secondPiElement.GetEpicItemDescription().Should().Be(SecondEpicDescription);
            }
        }

        private async Task CleanSubsystemsIfExist()
        {
            var subsystemPage = await SubsystemsPage.Navigate(_adminPage, BaseUrl);


            while (subsystemPage.GetSubsystemElementsOnPage().Count() > 0 )
            {
                var subSystemItems = subsystemPage.GetSubsystemElementsOnPage();
                var firstSubsystem = subSystemItems.FirstOrDefault();
                if (firstSubsystem != null)
                {
                    await firstSubsystem.SafelyExpandAll();
                    await firstSubsystem.DeleteSubsystem();
                }
            }
        }

        private async Task CleanProductIncrementsIfExist()
        {
            var piPage = new ProductIncrementsPage(_adminPage);
            await piPage.NavigateToPage(BaseUrl);
            await piPage.DeletePIs();
        }
    }
}
