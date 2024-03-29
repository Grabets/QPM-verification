﻿using Microsoft.Playwright;
using qpm.e2e.tests.PageObjects.Elements;

namespace qpm.e2e.tests.PageObjects
{
    public class StartPage
    {
        public string BaseUrl { get; set; }

        public StartPage(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public async Task<IPage> LoadStartPage(string userName, string password)
        {
            var browserRunner = new BrowserRunner();

            var startPage = await browserRunner.OpenInitPage(BaseUrl);

            await startPage.Locator("id=signInName").FillAsync(userName);
            await startPage.Locator("id=password").FillAsync(password);
            await startPage.Locator("id=next").ClickAsync();

            await startPage.WaitForLoadStateAsync();

            await startPage.Locator("text=Product increments").WaitForAsync();

            await Assertions.Expect(startPage).ToHaveURLAsync($"{BaseUrl}inbox");

            return startPage;
        }

        public async Task<HeaderElement> ChooseDefaultSettings(IPage page, string projectName = "Draft")
        {
            var userHeaderElement = new HeaderElement(page);
            await userHeaderElement.ChooseFirstProduct();
            await userHeaderElement.ChooseFirstProject(projectName);
            return userHeaderElement;
        }

    }
}
