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
