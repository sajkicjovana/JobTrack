package pmf.imi.moodle;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.FindBy;
import org.openqa.selenium.support.ui.ExpectedConditions;

public class LoginPage extends BasePageModel {

    public static final String LOGIN_URL =
            BASE_URL + "/login";


    @FindBy(name = "email")
    private WebElement emailInput;


    @FindBy(name = "password")
    private WebElement passwordInput;


    @FindBy(css = ".submit-button")
    private WebElement signInButton;


    @FindBy(css = ".error-message")
    private WebElement errorMessage;


    public LoginPage(WebDriver driver) {
        super(driver);
    }


    public void open() {

        driver.get(LOGIN_URL);

        wait.until(
                ExpectedConditions
                        .visibilityOf(emailInput)
        );
    }


    public void login(
            String email,
            String password) {

        wait.until(
                ExpectedConditions
                        .visibilityOf(emailInput)
        );

        emailInput.clear();
        emailInput.sendKeys(email);

        passwordInput.clear();
        passwordInput.sendKeys(password);

        signInButton.click();
    }


    public boolean isErrorDisplayed() {

        return wait.until(
                ExpectedConditions
                        .visibilityOf(errorMessage)
        ).isDisplayed();
    }


    public String getErrorMessage() {

        return wait.until(
                ExpectedConditions
                        .visibilityOf(errorMessage)
        ).getText();
    }


    public void waitForSuccessfulLogin() {

        wait.until(driver ->
                !driver.getCurrentUrl()
                        .contains("/login")
        );
    }
}