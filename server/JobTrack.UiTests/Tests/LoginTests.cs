using JobTrack.UiTests.Pages;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JobTrack.UiTests.Tests;

public class LoginTests : IDisposable
{
    private readonly IWebDriver _driver;


    public LoginTests()
    {
        var options =
            new ChromeOptions();

        options.AddArgument(
            "--start-maximized"
        );

        _driver =
            new ChromeDriver(options);
    }


    [Fact]
    public void Login_WithInvalidCredentials_ShowsError()
    {
        var loginPage =
            new LoginPage(_driver);


        loginPage.Open();

        loginPage.Login(
            "invalid-user@example.com",
            "WrongPassword123!"
        );


        Assert.True(
            loginPage.IsErrorDisplayed()
        );

        Assert.False(
            string.IsNullOrWhiteSpace(
                loginPage.GetErrorMessage()
            )
        );

        Assert.Contains(
            "/login",
            _driver.Url
        );
    }


    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}