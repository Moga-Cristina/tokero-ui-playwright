# UI Automation – Playwright + .NET

Repository created for Tokero UI Automated flows using **Playwright** and **xUnit**.

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) or later
- [Playwright](https://playwright.dev/dotnet/docs/intro) CLI installed via:

```bash
dotnet add package Microsoft.Playwright
dotnet add package Microsoft.Playwright.Xunit
dotnet tool install --global Microsoft.Playwright.CLI
playwright install
dotnet build
```

_(Optional but recommended)_: Install the **.NET Core Test Explorer** extension in VS Code for a more visual test experience. It allows you to run, filter, and debug tests directly from the Test Explorer panel.  
[https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer]
[ID] : formulahendry.dotnet-test-explorer

## Run Tests

```bash
dotnet test
```

## Project Structure

Folder/File Purpose

`PlaywrightTests.cs` - Main test suite  
`Helpers/BrowserHelper` - Cross-browser + context utilities  
`Helpers/LanguageHelper` - Language switching assertions  
`Helpers/TestDataGenerator.cs` - Email/random helpers  
`BasePage.cs` - Shared wrapper for context and page
`Screenshots/` - Captured screenshots on failure (created automatically if fail)

## Test Types Covered

- Functional testing (language switching, form validation, footer links)
- Performance testing (homepage loads within 5s)
- Multi-browser testing (Chromium, Firefox, WebKit)

## Documentation

See the detailed report `TestReport.docx` under docs folder.

## Author

Cristina Moga (mogacristina74@yahoo.com)

## Date

09.05.2025
