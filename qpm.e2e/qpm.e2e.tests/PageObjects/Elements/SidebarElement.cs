using Microsoft.Playwright;

namespace qpm.e2e.tests.PageObjects.Elements
{
    public class SidebarElement
    {
        public async Task ChooseElementsInSideBar(IPage page, string name)
        {
            var sidebar = page.Locator("//div[@class='sidebar is-enabled']").Last;
            await sidebar.WaitForAsync(new() { State = WaitForSelectorState.Visible });

            await sidebar.Locator($"//span[text()='{name}']/..").ClickAsync();
            await sidebar.Locator("//button[text()='Save']").ClickAsync();
        }
    }
}
