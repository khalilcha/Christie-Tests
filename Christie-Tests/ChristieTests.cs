using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace ChristieTests
{
    [TestFixture]
    public class TestBrowseByUsed
    {
        private IWebDriver driver;
        private string homeURL;

        [SetUp]
        public void SetupTest()
        {
            homeURL = "https://christieclearanceprojectors.com/collections/clearance-projectors";
            driver = new ChromeDriver();

        }

        [Test]
        public void Test()
        {
            
            driver.Navigate().GoToUrl(homeURL);

            List<string> allProjectors = TestHelpers.GetProjectors(driver);
            List<string> expectedUsed = (from p in allProjectors where p.Contains("Used") select p).ToList<string>();

            SelectElement browseBy =  new SelectElement(driver.FindElement(By.Id("BrowseBy")));
            browseBy.SelectByText("Used");

            List<string> actualUsed = TestHelpers.GetProjectors(driver);

            CollectionAssert.AreEqual(expectedUsed, actualUsed);
        }

        [TearDown]
        public void TearDownTest()
        {
            driver.Quit();
        }
    }

    [TestFixture]
    public class TestSortPriceAscending
    {
        private IWebDriver driver;
        private string homeURL;

        [SetUp]
        public void SetupTest()
        {
            homeURL = "https://christieclearanceprojectors.com/collections/clearance-projectors";
            driver = new ChromeDriver();

        }

        [Test]
        public void Test()
        {

            driver.Navigate().GoToUrl(homeURL);
            List<double> expectedPrices = TestHelpers.GetPrices(driver);
            expectedPrices.Sort();

            SelectElement sortBy = new SelectElement(driver.FindElement(By.Id("SortBy")));
            sortBy.SelectByValue("price-ascending");
            List<double> actualPrices = TestHelpers.GetPrices(driver);

            CollectionAssert.AreEqual(expectedPrices, actualPrices);
        }

        [TearDown]
        public void TearDownTest()
        {
            driver.Quit();
        }
    }

    [TestFixture]
    public class TestProjectorClick
    {
        private IWebDriver driver;
        private string homeURL;

        [SetUp]
        public void SetupTest()
        {
            homeURL = "https://christieclearanceprojectors.com/collections/clearance-projectors";
            driver = new ChromeDriver();

        }

        [Test]
        public void Test()
        {
            driver.Navigate().GoToUrl(homeURL);

            int nextPageClicks = 0;
            ReadOnlyCollection<IWebElement> pages = null;
            try 
            {
                pages = driver.FindElement(By.ClassName("pagination-custom")).FindElements(By.TagName("a"));
                nextPageClicks = int.Parse(pages[pages.Count - 2].Text) - 1;
            }
            catch (NoSuchElementException)
            {
                // This just means there was only 1 page, don't need to do anything
            }

            for (int i = 0; i < nextPageClicks || (nextPageClicks == 0 && i == 0); i++)
            {
                foreach(IWebElement e in driver.FindElement(By.ClassName("grid-link__container")).FindElements(By.ClassName("grid__item")))
                {
                    Assert.AreEqual(e.FindElement(By.TagName("a")), e.FindElement(By.ClassName("grid-link")));
                }

                for (int j = 0; j < driver.FindElements(By.ClassName("grid-link")).Count; j++)
                {
                    string gridPage = driver.Url;

                    string priceExpected = TestHelpers.PriceStringParse(driver.FindElements(By.ClassName("grid-link__meta"))[j].Text);
                    string projectorExpected = driver.FindElements(By.ClassName("grid-link__title"))[j].Text;

                    IWebElement e = driver.FindElements(By.ClassName("grid-link"))[j]; 
                    e.Click();

                    string priceActual = TestHelpers.PriceStringParse(driver.FindElement(By.Id("ProductPrice")).Text);
                    string projectorActual = driver.FindElement(By.ClassName("product-single__title")).Text;

                    Assert.AreEqual(priceExpected, priceActual);
                    Assert.AreEqual(projectorExpected, projectorActual);

                    driver.Navigate().GoToUrl(gridPage);                          
                }

                if(nextPageClicks != 0)
                {
                    //Stale element reference exception is fixed by below line, the .Click in the loop changes DOM
                    pages = driver.FindElement(By.ClassName("pagination-custom")).FindElements(By.TagName("a"));
                    pages[pages.Count - 1].Click(); //This clicks "arrow" button for the next page
                }
            }
        }

        [TearDown]
        public void TearDownTest()
        {
            driver.Quit();
        }

    }


    static class TestHelpers
    {
        public static List<string> GetProjectors(IWebDriver d)
        {
            string startingUrl = d.Url;

            List<string> projectors = IterateGrid(d, "grid-link__title");

            d.Navigate().GoToUrl(startingUrl);
            return projectors;
        }

        public static List<double> GetPrices(IWebDriver d)
        {
            string startingUrl = d.Url;

            List<string> priceStrings = IterateGrid(d, "grid-link__meta");
            for (int i = 0; i < priceStrings.Count; i++)
            {
                priceStrings[i] = PriceStringParse(priceStrings[i]);
            }

            d.Navigate().GoToUrl(startingUrl);
            return priceStrings.Select(x => double.Parse(x)).ToList();
        }

        public static string PriceStringParse(string s)
        {
            const string salePrice = "Sale price\r\n";
            const string regularPrice = "Regular price\r\n";

            if (s.Contains(salePrice))
            {
                int index = s.IndexOf(salePrice) + salePrice.Length;
                s = s.Substring(index);
            }
            else if (s.Contains(regularPrice))
            {
                int index = s.IndexOf(regularPrice) + regularPrice.Length;
                s = s.Substring(index);
            }
            s = s.Replace(",", "");
            s = s.Replace("$", "");

            return s;
        }

        private static List<string> IterateGrid(IWebDriver d, string className)
        {
            //Grab items from first page
            List<string> dataStrings = new List<string>();
            dataStrings.AddRange(from e in d.FindElements(By.ClassName(className)) select e.Text);

            try
            {
                ReadOnlyCollection<IWebElement> pages = d.FindElement(By.ClassName("pagination-custom")).FindElements(By.TagName("a"));
                int clicks = int.Parse(pages[pages.Count - 2].Text) - 1;
                for (int i = 0; i < clicks; i++)
                {
                    //Stale element reference exception is fixed by below line, the .Click in the loop changes DOM
                    pages = d.FindElement(By.ClassName("pagination-custom")).FindElements(By.TagName("a"));
                    pages[pages.Count - 1].Click(); //This clicks "arrow" button for the next page
                    dataStrings.AddRange(from e in d.FindElements(By.ClassName(className)) select e.Text);
                }
            }
            catch(NoSuchElementException)
            {
                // This just means there was only 1 page, don't need to do anything
            }                       

            return dataStrings;
        }
    }
}
