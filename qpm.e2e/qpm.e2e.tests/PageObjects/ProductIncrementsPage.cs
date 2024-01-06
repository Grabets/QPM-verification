using Microsoft.Playwright;
using System.Diagnostics;

namespace qpm.e2e.tests.PageObjects
{
    public class ProductIncrementsPage
    {
        public const string ResourceName = "product-increments";
        private IPage _page;

        public ProductIncrementsPage(IPage page)
        {
            _page = page;

            //TODO: use PAGE extensions class
            var cookiesContainer = page.Locator("//div[@class='cc-isolation-container']");
            if (cookiesContainer.CountAsync().Result > 0)
            {
                cookiesContainer.Locator("//button[text()='Accept all']").ClickAsync().Wait();
            }
        }

        public async Task CreatePI(string piTitle, string piDescription, string plannedDate)
        {
            await _page.Locator("//button[text()='Create product increment']").ClickAsync();

            //TODO: need to find more sophisticated way. Here should be some explisit wait.
            Task.Delay(3000).Wait();
            ILocator piItem = await new DocumentItemElement()
                .FillTitleAndDescription(_page, piTitle, piDescription);

            await ChooseDateInDatePicker(piItem, plannedDate);
        }

        public async Task NavigateToPage(string baseUrl)
        {
            //TODO: need parent class
            var url = baseUrl + ResourceName;
            await _page.GotoAsync(url);
            await WaitForElementsPresense();
        }

        private async Task WaitForElementsPresense()
        {
            while (true)
            {
                var stopwatch = Stopwatch.StartNew();

                if (stopwatch.ElapsedMilliseconds > TimeSpan.FromSeconds(10).Milliseconds ||
                    await _page.Locator(DocumentItemElement.TitleXPathLocator).CountAsync() == 2)
                {
                    stopwatch.Stop();
                    break;
                }
            }
        }

        public async Task<List<ProductIncrementItemElement>> GetPIItemElements()
        {
            await WaitForElementsPresense();

            var piItemLocators = await new DocumentItemElement().GetItemsOnPage(_page);
            var itemsCount = await piItemLocators.CountAsync();
            var itemElements = new List<ProductIncrementItemElement>();
            for (var i = 0; i < itemsCount; i++)
            {
                itemElements.Add(new ProductIncrementItemElement(piItemLocators.Nth(i)));
            }
            return itemElements;
        }

        private async Task ChooseDateInDatePicker(ILocator piItem, string plannedDate)
        {
            await piItem.Locator("//div[contains(@class,'DatePicker')]//input").ClickAsync();

            //TODO: We can have a problem with datepicker on the last day of the month.

            await _page.Locator("//div[contains(@class,'picker-calendar')]")
                .Locator($"//button[contains(@aria-label, '{plannedDate}')]")
                .ClickAsync();

            await piItem.Locator("//span[@title='Update']").ClickAsync();
        }





        public class ProductIncrementItemElement
        {
            //TODO: better to make with EpicItem as this xpath dublicates
            private const string EpicsParentXPathSelector = "//h3[text()='Epics']/ancestor::div[contains(@class,'document__branch')][1]";

            public string Title => new DocumentItemElement().GetDocumentItemTitle(_item).Result;
            public string Description => GetItemDescription().Result;
            public string PlannedDate => GetPlannedDate().Result;

            private ILocator _item;
            private ILocator _epicItem;


            public ProductIncrementItemElement(ILocator item)
            {
                _item = item;
            }

            public async Task Expand()
            {
                await new DocumentItemElement().ExpandItem(_item, ItemTypes.ProductIncrement);
            }

            public async Task Shrink()
            {
                await new DocumentItemElement().ShrinkItem(_item, ItemTypes.ProductIncrement);
            }

            private async Task<string> GetItemDescription()
            {
                return await new DocumentItemElement().GetDocumentItemDescription(_item, ItemTypes.ProductIncrement);
            }

            private async Task<string> GetPlannedDate()
            {
                return await _item.Locator("//div[@title='Planned Date']/..//div[@class='renderer']").First.InnerTextAsync();
            }

            /// <summary>
            /// Method expand epics block. Works only with one epics.
            /// </summary>
            /// <returns></returns>
            /// <exception cref="NullReferenceException"></exception>
            public async Task ExpandEpics()
            {
                var epics = _item.Locator(EpicsParentXPathSelector);
                var innerText = await epics.InnerTextAsync();
                if (!innerText.Contains("(1)")) 
                    throw new NullReferenceException("Epics not found");

                await epics.Locator("//span[contains(@style,'triangle-filled')]").ClickAsync();
                await epics.Locator("//span[contains(@style,'triangle-outline')]").WaitForAsync();

                await new DocumentItemElement().ExpandItem(epics, ItemTypes.Epic);

                _epicItem = epics.Locator("//div[@class='document__block']");
            }

            public string GetEpicItemTitle()
            {
                return _epicItem.Locator(DocumentItemElement.TitleXPathLocator).First.InnerTextAsync().Result;
            }

            public string GetEpicItemDescription()
            {
                return _epicItem.Locator(DocumentItemElement.TitleXPathLocator).Nth(1).InnerTextAsync().Result;
            }

        }
    }
}
