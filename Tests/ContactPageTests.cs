using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;



public class ContactPageTests : IAsyncLifetime
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private const string BaseUrl = "https://tokero.dev/en/contact/";


    #region SETUP

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false });
    }
    public async Task DisposeAsync()
    {
        await _browser.CloseAsync();
    }

    #endregion


    #region NEGATIVE TESTS

    [Fact]
    public async Task ShouldShowError_WhenInputIsLessThan20Characters()
    {
        var basePage = await BasePage.CreateAsync(_browser, BaseUrl);
        var messageInput = basePage.Page.Locator("#contact-form-message");
        var errorLocator = basePage.Page.Locator("#contact-form-message-error");

        // Fill 19 caracters
        await messageInput.FillAsync("1234567890123456789");
        await basePage.Page.WaitForTimeoutAsync(5000);

        // Verify if the error message is visible
        var isErrorVisible = await errorLocator.IsVisibleAsync();
        Assert.True(isErrorVisible, "Expected error message not shown for input < 20 chars.");

        await basePage.CloseAsync(); // cleanup
    }

    #endregion


    #region POSITIVE TESTS

    [Fact]
    public async Task ShouldHideError_WhenInputIsExactly20Characters()
    {
        var basePage = await BasePage.CreateAsync(_browser, BaseUrl);
        var messageInput = basePage.Page.Locator("#contact-form-message");
        var errorLocator = basePage.Page.Locator("#contact-form-message-error");

        // Fill 20 caracters
        await messageInput.FillAsync("12345678901234567890");
        await basePage.Page.WaitForTimeoutAsync(5000);

        // Verify if the error message is not visible
        var isErrorVisible = await errorLocator.IsVisibleAsync();
        Assert.False(isErrorVisible, "Error message still visible after valid 20-character input.");

        await basePage.CloseAsync(); // cleanup
    }

    #endregion
}