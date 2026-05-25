using System.Globalization;
using Toolbelt.Blazor.Splitter.V2;

namespace SplitContainer.Test.V2;

public class SplitContainerTest
{
    [Test]
    public void GetPaneStyle_NoSize_Test()
    {
        // Given
        var splitContainer = new SplitContainer<int>()
        {
            Orientation = SplitterOrientation.Horizontal,
            UnitOfPaneSize = UnitOfPaneSize.Percent,
        };

        // When
        var styleText = splitContainer.GetPaneStyle(null, null);

        // Then
        styleText.Is("flex:1;");
    }

    [Test]
    public void GetPaneStyle_PixelSize_Test()
    {
        // Given
        var splitContainer = new SplitContainer<int>()
        {
            Orientation = SplitterOrientation.Vertical,
            UnitOfPaneSize = UnitOfPaneSize.Pixel,
        };

        // When
        var styleText = splitContainer.GetPaneStyle(128, null);

        // Then
        styleText.Is("width:128px;");
    }

    [Test]
    public void GetPaneStyle_PersentSize_Test()
    {
        // Given
        var splitContainer = new SplitContainer<int>()
        {
            Orientation = SplitterOrientation.Horizontal,
            UnitOfPaneSize = UnitOfPaneSize.Percent,
        };

        // When
        var styleText = splitContainer.GetPaneStyle(34, 10);

        // Then
        styleText.Is("min-height:calc(10% - calc(var(--splitter-bar-size) / 2));height:calc(34% - calc(var(--splitter-bar-size) / 2));");
    }

    [Test]
    public void GetPaneStyle_PersentMinSize_Test()
    {
        // Given
        var splitContainer = new SplitContainer<double>()
        {
            Orientation = SplitterOrientation.Vertical,
            UnitOfPaneSize = UnitOfPaneSize.Percent,
        };

        // When
        var styleText = splitContainer.GetPaneStyle(null, 8.5);

        // Then
        styleText.Is("min-width:calc(8.5% - calc(var(--splitter-bar-size) / 2));flex:1;");
    }
}

public class SplitContainerCultureTest
{
    private CultureInfo _OriginalCulture = null!;
    private CultureInfo _OriginalUICulture = null!;

    [SetUp]
    public void SetUp()
    {
        this._OriginalCulture = CultureInfo.CurrentCulture;
        this._OriginalUICulture = CultureInfo.CurrentUICulture;
    }

    [TearDown]
    public void TearDown()
    {
        CultureInfo.CurrentCulture = this._OriginalCulture;
        CultureInfo.CurrentUICulture = this._OriginalUICulture;
    }

    // The browser's CSS calc() only accepts '.' as the decimal separator,
    // so the style must never contain ',' regardless of the OS / app culture.
    [TestCase("pl-PL")]
    [TestCase("de-DE")]
    [TestCase("fr-FR")]
    [TestCase("ru-RU")]
    [TestCase("nl-NL")]
    public void GetPaneStyle_PercentSize_UsesInvariantDecimalSeparator(string cultureName)
    {
        CultureInfo.CurrentCulture = new CultureInfo(cultureName);
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;

        var splitContainer = new SplitContainer<double>
        {
            Orientation = SplitterOrientation.Horizontal,
            UnitOfPaneSize = UnitOfPaneSize.Percent,
        };

        var styleText = splitContainer.GetPaneStyle(47.059, null);

        styleText.Is("height:calc(47.059% - calc(var(--splitter-bar-size) / 2));");
        styleText.Contains(",").IsFalse();
    }

    [TestCase("pl-PL")]
    [TestCase("de-DE")]
    public void GetPaneStyle_PercentMinSize_UsesInvariantDecimalSeparator(string cultureName)
    {
        CultureInfo.CurrentCulture = new CultureInfo(cultureName);
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;

        var splitContainer = new SplitContainer<double>
        {
            Orientation = SplitterOrientation.Vertical,
            UnitOfPaneSize = UnitOfPaneSize.Percent,
        };

        var styleText = splitContainer.GetPaneStyle(null, 8.5);

        styleText.Is("min-width:calc(8.5% - calc(var(--splitter-bar-size) / 2));flex:1;");
        styleText.Contains(",").IsFalse();
    }

    // Pixel path is also culture-sensitive for fractional TSize.
    // Latent today (most callers pass int), but the contract is "valid CSS".
    [TestCase("pl-PL")]
    [TestCase("de-DE")]
    public void GetPaneStyle_PixelSize_FractionalDouble_UsesInvariantDecimalSeparator(string cultureName)
    {
        CultureInfo.CurrentCulture = new CultureInfo(cultureName);
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;

        var splitContainer = new SplitContainer<double>
        {
            Orientation = SplitterOrientation.Vertical,
            UnitOfPaneSize = UnitOfPaneSize.Pixel,
        };

        var styleText = splitContainer.GetPaneStyle(627.5, null);

        styleText.Is("width:627.5px;");
        styleText.Contains(",").IsFalse();
    }
}
