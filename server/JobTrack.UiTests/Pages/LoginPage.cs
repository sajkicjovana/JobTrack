using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace JobTrack.UiTests.Pages;

public class LoginPage
{
    private const string BaseUrl =
        "http://localhost:4200";

    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;


    public LoginPage(IWebDriver driver)
    {
        _driver = driver;

        _wait = new WebDriverWait(
            driver,
            TimeSpan.FromSeconds(10)
        );
    }


    private IWebElement EmailInput =>
        _wait.Until(driver =>
            driver.FindElement(
                By.Name("email")
            )
        );


    private IWebElement PasswordInput =>
        _driver.FindElement(
            By.Name("password")
        );


    private IWebElement SignInButton =>
        _driver.FindElement(
            By.CssSelector(
                ".submit-button"
            )
        );


    public void Open()
    {
        _driver.Navigate().GoToUrl(
            $"{BaseUrl}/login"
        );
    }


    public void Login(
        string email,
        string password)
    {
        EmailInput.Clear();
        EmailInput.SendKeys(email);

        PasswordInput.Clear();
        PasswordInput.SendKeys(password);

        SignInButton.Click();
    }


    public bool IsErrorDisplayed()
    {
        var error =
            _wait.Until(driver =>
                driver.FindElement(
                    By.CssSelector(
                        ".error-message"
                    )
                )
            );

        return error.Displayed;
    }


    public string GetErrorMessage()
    {
        return _wait.Until(driver =>
            driver.FindElement(
                By.CssSelector(
                    ".error-message"
                )
            )
        ).Text;
    }


    public void WaitForSuccessfulLogin()
    {
        _wait.Until(driver =>
            !driver.Url.Contains(
                "/login"
            )
        );
    }
}