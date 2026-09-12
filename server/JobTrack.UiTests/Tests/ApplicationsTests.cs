using JobTrack.UiTests.Pages;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JobTrack.UiTests.Tests;

public class ApplicationsTests : IDisposable
{
    private readonly IWebDriver _driver;


    public ApplicationsTests()
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
    public void LoggedInUser_CanOpenNewApplicationForm()
    {
        var email =
            Environment.GetEnvironmentVariable(
                "JOBTRACK_TEST_EMAIL"
            );

        var password =
            Environment.GetEnvironmentVariable(
                "JOBTRACK_TEST_PASSWORD"
            );


        Assert.False(
            string.IsNullOrWhiteSpace(email),
            "JOBTRACK_TEST_EMAIL is not set."
        );

        Assert.False(
            string.IsNullOrWhiteSpace(password),
            "JOBTRACK_TEST_PASSWORD is not set."
        );


        var loginPage =
            new LoginPage(_driver);


        loginPage.Open();

        loginPage.Login(
            email!,
            password!
        );

        loginPage.WaitForSuccessfulLogin();


        var applicationsPage =
            new ApplicationsPage(
                _driver
            );


        applicationsPage.Open();

        applicationsPage
            .OpenNewApplicationForm();


        Assert.True(
            applicationsPage
                .IsNewApplicationFormDisplayed()
        );

        Assert.Equal(
            "New Application",
            applicationsPage
                .GetFormHeading()
        );
    }


    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}