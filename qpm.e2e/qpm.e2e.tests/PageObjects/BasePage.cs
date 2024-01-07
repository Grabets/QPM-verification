using Microsoft.Playwright;

namespace qpm.e2e.tests.PageObjects
{
    public abstract class BasePage
    {
        protected IPage _page;

        public BasePage(IPage page)
        {
            _page = page;
        }
    }
}
