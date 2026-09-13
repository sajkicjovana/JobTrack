package pmf.imi.moodle;

import io.github.bonigarcia.wdm.WebDriverManager;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;

import org.testng.annotations.AfterMethod;
import org.testng.annotations.BeforeMethod;
import org.testng.annotations.Test;

import java.util.List;

import static org.testng.Assert.*;

public class ApplicationsPageTest {

    private static final String USER_EMAIL =
            "ana@test.com";

    private static final String USER_PASSWORD =
            "Test123!";


    private WebDriver driver;

    private ApplicationsPage
            applicationsPage;


    @BeforeMethod
    public void beforeMethod() {

        WebDriverManager
                .chromedriver()
                .setup();

        driver =
                new ChromeDriver();

        driver.manage()
                .window()
                .maximize();


        LoginPage loginPage =
                new LoginPage(driver);

        loginPage.open();

        loginPage.login(
                USER_EMAIL,
                USER_PASSWORD
        );

        loginPage
                .waitForSuccessfulLogin();


        applicationsPage =
                new ApplicationsPage(
                        driver
                );

        applicationsPage.open();
    }


    @AfterMethod
    public void afterMethod() {

        if (driver != null) {
            driver.quit();
        }
    }


    @Test
    public void testApplicationsPageHeadingAndUrl() {

        assertTrue(
                driver
                        .getCurrentUrl()
                        .contains("/applications")
        );

        assertEquals(
                applicationsPage
                        .getHeadingText(),
                "Job Applications"
        );
    }


    @Test
    public void testNewApplicationFormOpens() {

        applicationsPage
                .openNewApplicationForm();

        assertTrue(
                applicationsPage
                        .isNewApplicationFormDisplayed()
        );

        assertEquals(
                applicationsPage
                        .getNewApplicationHeading(),
                "New Application"
        );
    }


    @Test
    public void testStatusFilterOptions() {

        assertEquals(
                applicationsPage
                        .getStatusFilterOptions(),

                List.of(
                        "All statuses",
                        "Saved",
                        "Applied",
                        "HR Interview",
                        "Technical Interview",
                        "Offer",
                        "Rejected",
                        "No Response"
                )
        );

        assertEquals(
                applicationsPage
                        .getSelectedStatusValue(),
                ""
        );
    }


    @Test
    public void testSearchInputAcceptsText() {

        applicationsPage
                .enterSearchText(
                        "Microsoft"
                );

        assertEquals(
                applicationsPage
                        .getSearchValue(),
                "Microsoft"
        );
    }
}