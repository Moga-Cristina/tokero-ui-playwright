using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;



public static class LanguageHelper
{
    public static async Task SwitchLanguageAndAssertAsync(IPage page, string langCode, string expectedText)
    {
        await page.Locator("button.dropdown-toggle").First.ClickAsync();
        await page.Locator($".languageSwitcher_btnLabel__JjcN5:has-text('{langCode}')").ClickAsync();
        await page.WaitForSelectorAsync($"text={expectedText}", new() { Timeout = 5000 });

        var isVisible = await page.Locator($"text={expectedText}").First.IsVisibleAsync();
        Assert.True(isVisible, $"Expected text '{expectedText}' was not found after switching to {langCode}.");
    }
}
