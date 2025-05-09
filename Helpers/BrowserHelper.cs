using Microsoft.Playwright;
using System.Threading.Tasks;
using System;



public static class BrowserHelper
{
    public static async Task<(IBrowserContext context, IPage page)> CreatePageAndGotoAsync(IBrowser browser, string url)
    {
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync(url);
        return (context, page);
    }


    public static async Task AcceptCookiesIfVisibleAsync(IPage page)
    {
        var acceptBtn = page.Locator("text=Accept all cookies");
        if (await acceptBtn.IsVisibleAsync())
        {
            await acceptBtn.ClickAsync();
        }
    }


    public static async Task<(IBrowser browser, IBrowserContext context, IPage page)> LaunchBrowserAsync(string browserName, bool headless = false)
    {
        var playwright = await Playwright.CreateAsync();

        IBrowser browser = browserName switch
        {
            "chromium" => await playwright.Chromium.LaunchAsync(new() { Headless = headless }),
            "firefox" => await playwright.Firefox.LaunchAsync(new() { Headless = headless }),
            "webkit" => await playwright.Webkit.LaunchAsync(new() { Headless = headless }),
            _ => throw new ArgumentException("Unsupported browser", nameof(browserName))
        };

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        return (browser, context, page);
    }
}
