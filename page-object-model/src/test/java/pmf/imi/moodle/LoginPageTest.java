package pmf.imi.moodle;

import io.github.bonigarcia.wdm.WebDriverManager;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;

import org.testng.annotations.AfterMethod;
import org.testng.annotations.BeforeMethod;
import org.testng.annotations.Test;

import static org.testng.Assert.*;

public class LoginPageTest {

    private static final String USER_EMAIL =
            "ana@test.com";

    private static final String USER_PASSWORD =
            "Test123!";


    private WebDriver driver;
    private LoginPage loginPage;


    @BeforeMethod
    public void beforeMethod() {

        WebDriverManager
                .chromedriver()
                .setup();

        driver = new ChromeDriver();

        driver.manage()
                .window()
                .maximize();

        loginPage =
                new LoginPage(driver);

        loginPage.open();
    }


    @AfterMethod
    public void afterMethod() {

        if (driver != null) {
            driver.quit();
        }
    }


    @Test
    public void testLoginPageUrl() {

        assertTrue(
                driver
                        .getCurrentUrl()
                        .contains("/login")
        );
    }


    @Test
    public void testInvalidLoginShowsError() {

        loginPage.login(
                "invalid@test.com",
                "WrongPassword123!"
        );

        assertTrue(
                loginPage.isErrorDisplayed()
        );

        assertFalse(
                loginPage
                        .getErrorMessage()
                        .isEmpty()
        );

        assertTrue(
                driver
                        .getCurrentUrl()
                        .contains("/login")
        );
    }


    @Test
    public void testSuccessfulLogin() {

        loginPage.login(
                USER_EMAIL,
                USER_PASSWORD
        );

        loginPage.waitForSuccessfulLogin();

        assertFalse(
                driver
                        .getCurrentUrl()
                        .contains("/login")
        );
    }
}