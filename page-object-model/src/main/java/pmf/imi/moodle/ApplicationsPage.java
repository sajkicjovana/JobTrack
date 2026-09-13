package pmf.imi.moodle;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.FindBy;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.Select;

import java.util.List;
import java.util.stream.Collectors;

public class ApplicationsPage extends BasePageModel {

    public static final String APPLICATIONS_URL =
            BASE_URL + "/applications";


    @FindBy(css = ".page-header h1")
    private WebElement heading;


    @FindBy(css = ".add-button")
    private WebElement addApplicationButton;


    @FindBy(
            css =
            "input[placeholder='Search company or position...']"
    )
    private WebElement searchInput;


    @FindBy(css = ".application-filters select")
    private List<WebElement> filters;


    private static final By APPLICATION_FORM =
            By.id("applicationForm");


    private static final By NEW_APPLICATION_HEADING =
            By.xpath(
                    "//h2[normalize-space()='New Application']"
            );


    public ApplicationsPage(
            WebDriver driver) {

        super(driver);
    }


    public void open() {

        driver.get(APPLICATIONS_URL);

        wait.until(
                ExpectedConditions
                        .urlContains("/applications")
        );

        wait.until(
                ExpectedConditions
                        .visibilityOf(heading)
        );
    }


    public String getHeadingText() {

        return wait.until(
                ExpectedConditions
                        .visibilityOf(heading)
        ).getText();
    }


    public void openNewApplicationForm() {

        wait.until(
                ExpectedConditions
                        .elementToBeClickable(
                                addApplicationButton
                        )
        ).click();
    }


    public boolean isNewApplicationFormDisplayed() {

        return wait.until(
                ExpectedConditions
                        .visibilityOfElementLocated(
                                APPLICATION_FORM
                        )
        ).isDisplayed();
    }


    public String getNewApplicationHeading() {

        return wait.until(
                ExpectedConditions
                        .visibilityOfElementLocated(
                                NEW_APPLICATION_HEADING
                        )
        ).getText();
    }


    public List<String> getStatusFilterOptions() {

        WebElement statusFilter =
                filters.get(0);

        return new Select(statusFilter)
                .getOptions()
                .stream()
                .map(WebElement::getText)
                .map(String::trim)
                .collect(Collectors.toList());
    }


    public String getSelectedStatusValue() {

        WebElement statusFilter =
                filters.get(0);

        return new Select(statusFilter)
                .getFirstSelectedOption()
                .getAttribute("value");
    }


    public void enterSearchText(String text) {

        searchInput.clear();
        searchInput.sendKeys(text);
    }


    public String getSearchValue() {

        return searchInput.getAttribute(
                "value"
        );
    }
}