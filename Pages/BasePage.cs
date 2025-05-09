using Microsoft.Playwright;
using System.Threading.Tasks;



public class BasePage
{
    private readonly IBrowser _browser;
    private readonly IBrowserContext _context;
    private readonly IPage _page;


    private BasePage(IBrowser browser, IBrowserContext context, IPage page)
    {
        _browser = browser;
        _context = context;
        _page = page;
    }


    public static async Task<BasePage> CreateForBrowserAsync(string browserName, string url, bool headless = false)
    {
        var (browser, context, page) = await BrowserHelper.LaunchBrowserAsync(browserName, headless);
        await page.GotoAsync(url);
        await BrowserHelper.AcceptCookiesIfVisibleAsync(page);

        return new BasePage(browser, context, page);
    }


    public static async Task<BasePage> CreateAsync(IBrowser browser, string url)
    {
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync(url);
        await BrowserHelper.AcceptCookiesIfVisibleAsync(page);

        return new BasePage(browser, context, page);
    }


    public async Task<string> GetTitleAsync()
        => await _page.TitleAsync();

    public IPage Page => _page;
    public IBrowserContext Context => _context;
    public IBrowser Browser => _browser;


    public async Task CloseAsync()
    {
        await _context.CloseAsync();
        await _browser.CloseAsync();
    }
}
