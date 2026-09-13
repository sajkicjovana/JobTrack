package pmf.imi.moodle;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.support.PageFactory;
import org.openqa.selenium.support.ui.WebDriverWait;

import java.time.Duration;

public class BasePageModel {

    public static final String BASE_URL =
            "http://localhost:4200";

    protected WebDriver driver;
    protected WebDriverWait wait;

    public BasePageModel(WebDriver driver) {

        this.driver = driver;

        this.wait = new WebDriverWait(
                driver,
                Duration.ofSeconds(10)
        );

        PageFactory.initElements(
                driver,
                this
        );
    }
}