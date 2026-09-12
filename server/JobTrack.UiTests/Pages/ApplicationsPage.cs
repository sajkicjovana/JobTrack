using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace JobTrack.UiTests.Pages;

public class ApplicationsPage
{
    private const string BaseUrl =
        "http://localhost:4200";

    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;


    public ApplicationsPage(
        IWebDriver driver)
    {
        _driver = driver;

        _wait = new WebDriverWait(
            driver,
            TimeSpan.FromSeconds(10)
        );
    }


    public void Open()
    {
        _driver.Navigate().GoToUrl(
            $"{BaseUrl}/applications"
        );

        _wait.Until(driver =>
            driver.Url.Contains(
                "/applications"
            )
        );
    }


    public void OpenNewApplicationForm()
    {
        var addButton =
            _wait.Until(driver =>
                driver.FindElement(
                    By.CssSelector(
                        ".add-button"
                    )
                )
            );

        addButton.Click();
    }


    public bool IsNewApplicationFormDisplayed()
    {
        var form =
            _wait.Until(driver =>
                driver.FindElement(
                    By.Id(
                        "applicationForm"
                    )
                )
            );

        return form.Displayed;
    }


    public string GetFormHeading()
    {
        return _wait.Until(driver =>
            driver.FindElement(
                By.XPath(
                    "//h2[normalize-space()='New Application']"
                )
            )
        ).Text;
    }
}