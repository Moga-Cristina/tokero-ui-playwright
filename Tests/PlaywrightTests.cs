using Microsoft.Playwright;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Diagnostics;
using System.IO;



public class PlaywrightTests : IAsyncLifetime
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private const string BaseUrl = "https://tokero.dev/en/";



    #region SETUP
    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false });
    }


    public async Task DisposeAsync()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    #endregion


    #region PERFORMANCE TESTS

    [Fact]
    public async Task HomePage_ShouldLoadWithin5Seconds()
    {
        var basePage = await BasePage.CreateAsync(_browser, BaseUrl);

        var stopwatch = Stopwatch.StartNew();

        await basePage.Page.GotoAsync(BaseUrl, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = 10000
        });

        stopwatch.Stop();

        Console.WriteLine($"Page load time: {stopwatch.ElapsedMilliseconds} ms");

        Assert.True(stopwatch.ElapsedMilliseconds < 5000, $"Page took too long to load: {stopwatch.ElapsedMilliseconds} ms");

        await basePage.CloseAsync(); // cleanup
    }

    #endregion


    #region FUNCTIONAL TESTS

    [Fact]
    public async Task ShouldContainTextInTitle()
    {
        var basePage = await BasePage.CreateAsync(_browser, BaseUrl);
        var title = await basePage.GetTitleAsync();

        Assert.Contains("TOKERO", title);

        await basePage.CloseAsync(); // cleanup
    }


    [Theory]
    [InlineData("chromium")]
    [InlineData("firefox")]
    [InlineData("webkit")]
    public async Task ShouldWorkInAllBrowsers(string browserName)
    {
        var basePage = await BasePage.CreateForBrowserAsync(browserName, BaseUrl);

        var title = await basePage.GetTitleAsync();
        Assert.Contains("TOKERO", title);

        await basePage.CloseAsync(); // Cleanup
    }


    [Theory]
    [InlineData("RO", "Creare cont")]
    [InlineData("EN", "Create account")]
    [InlineData("DE", "Konto erstellen")]
    public async Task LanguageSwitcher_ShouldChangeLanguage(string langCode, string expectedText)
    {
        var basePage = await BasePage.CreateAsync(_browser, BaseUrl);


        await LanguageHelper.SwitchLanguageAndAssertAsync(basePage.Page, langCode, expectedText);

        await basePage.CloseAsync(); // cleanup
    }


    [Fact]
    public async Task JoinSocialFi_ShouldShowCongratsMessage()
    {
        var basePage = await BasePage.CreateAsync(_browser, BaseUrl);

        // Click on the "SocialFi" section
        await basePage.Page.Locator("div .text-hover-purple-white").ClickAsync();

        // Wait for the "register waiting list" div to become visible (max 20s)
        await basePage.Page.WaitForSelectorAsync("div .border-smoke-white", new() { Timeout = 20000 });

        var registerWaitingListDiv = await basePage.Page.Locator("div .border-smoke-white").IsVisibleAsync();

        Assert.True(registerWaitingListDiv, "Register waiting list section not visible");

        // Fill and submit
        await basePage.Page.FillAsync("#Name", "Cris");
        await basePage.Page.FillAsync("#Email", TestDataGenerator.GenerateRandomEmail());
        await basePage.Page.ClickAsync(".mud-button-label");

        // Wait for success message
        await basePage.Page.WaitForSelectorAsync("h1.mt-5", new() { Timeout = 20000 });

        var h1Text = await basePage.Page.Locator("h1.mt-5").InnerTextAsync();

        Assert.Equal("Congratulations!", h1Text);

        await basePage.CloseAsync(); // cleanup
    }


    [Fact]
    public async Task FooterPolicyLinks_ShouldOpenAndContainExpectedText()
    {
        var basePage = await BasePage.CreateAsync(_browser, BaseUrl);

        // Relevant footer links 
        string[] keywords = { "Policies list", "Terms and conditions", "GDPR", "Privacy", "KYC", "Cookies" };

        // All links from the footer area
        var footerLinks = await basePage.Page.QuerySelectorAllAsync("footer a");

        foreach (var link in footerLinks)
        {
            var text = await link.InnerTextAsync();

            // Match links 
            if (keywords.Any(k => text.Contains(k)))
            {
                var href = await link.GetAttributeAsync("href");
                if (string.IsNullOrEmpty(href)) continue;

                var fullUrl = new Uri(new Uri("https://tokero.dev"), href).ToString();
                var newPage = await basePage.Context.NewPageAsync();

                await newPage.GotoAsync(fullUrl, new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 15000 });

                // Confirm the page is correct
                string[] validContents = {
                "TOKERO policies and rules", "Terms of Service", "RIGHTS OF DATA SUBJECTS",
                "Privacy Policy", "KYC and AML policy", "Cookies Policy"
            };

                // Find at least one of the expected texts on the page
                bool textFound = false;
                foreach (var expectedText in validContents)
                {
                    try
                    {
                        await newPage.WaitForSelectorAsync($"text={expectedText}", new() { Timeout = 15000 });
                        Console.WriteLine($"Found expected text: \"{expectedText}\"");
                        textFound = true;
                        break;
                    }
                    catch (PlaywrightException)
                    {
                        var screenshotDir = "Screenshots";
                        if (!Directory.Exists(screenshotDir))
                        {
                            Directory.CreateDirectory(screenshotDir);
                        }

                        await newPage.ScreenshotAsync(new()
                        {
                            Path = $"{screenshotDir}/policy_error_{DateTime.Now:yyyyMMdd_HHmmss}.png"
                        });
                        throw;
                    }
                }

                // Assert that at least one expected content string is visible
                Assert.True(textFound, $"None of the expected texts were found at {fullUrl}");
                Console.WriteLine($"Finished checking: {fullUrl}\n");

                await newPage.CloseAsync();
            }
        }
        await basePage.Context.CloseAsync(); // Cleanup 
    }

    #endregion
}
