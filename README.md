# Blazor SplitContainer [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.Blazor.SplitContainer.svg)](https://www.nuget.org/packages/Toolbelt.Blazor.SplitContainer/) [![Discord](https://img.shields.io/discord/798312431893348414?style=flat&logo=discord&logoColor=white&label=Blazor%20Community&labelColor=5865f2&color=gray)](https://discord.com/channels/798312431893348414/1202165955900473375)

## Summary

A Blazor component to create panes separated by a slidable splitter bar.

[![movie](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.SplitContainer/main/.assets/movie-001.gif)](https://jsakamoto.github.io/Toolbelt.Blazor.SplitContainer/)

- [Live Demo](https://jsakamoto.github.io/Toolbelt.Blazor.SplitContainer/)

> **Note**  
> I know the same feature component library, the ["BlazorSliders"](https://github.com/carlfranklin/BlazorSliders), already exists, but it was a bit unsmooth, particularly on Blazor Server apps. So I've decided to create another one on my hand.

## How to use

### Installation

1. Add package to your project like this.

```shell
dotnet add package Toolbelt.Blazor.SplitContainer
```

3. Open `Toolbelt.Blazor.Splitter.V2` namespace in `_Imports.razor` file.

```razor
@* This is "_Imports.razor" *@
...
@using Toolbelt.Blazor.Splitter.V2
```

4. Then you can use the `SplitContainer` component in your Blazor app.

```html
<SplitContainer @bind-FirstPaneSize="_PaneSize">

    <FirstPane>
        <h1>Hello</h1>
    </FirstPane>

    <SecondPane>
        <h1>World</h1>
    </SecondPane>

</SplitContainer>

@code {
    private int _PaneSize = 80;
}
```

#### NOTICE: Including CSS style sheet

This package assumes that the application uses Blazor's CSS isolation by default. Usually, this pre-requirement is appropriate. However, unfortunately, some Blazor projects scenario, such as those made by the "empty" project template, are not configured for CSS isolation. In this case, the CSS file of this package will never be loaded in the app, and the Splitter Container component will not work. To resolve this issue, you must include this package's CSS file by yourself.

Specifically, you should include the bundled CSS file for the project in the fallback HTML document file, like the following code,

```html
<!DOCTYPE html>
<html lang="en">
<head>
    ...
    <!-- 👇 Add this line. -->
    <link href="{ASSEMBLY NAME}.styles.css" rel="stylesheet" />
    ....
```

or include the CSS file for this package individually, like the following code.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    ...
    <!-- 👇 Add this line. -->
    <link href="_content/Toolbelt.Blazor.SplitContainer/Toolbelt.Blazor.SplitContainer.bundle.scp.css"
        rel="stylesheet" />
    ...
```

See also: https://learn.microsoft.com/aspnet/core/blazor/components/css-isolation

### Parameters

 Parameter Name       |  Type               | Description
----------------------|---------------------|--------------
TSize                 | `Type`                | The type of the size value of the panes. It can be `int`, `double`, etc. This is the type parameter for the `SplitContainer` component. You don't have to specify this type parameter explicitly as long as it can be inferred from the other parameters.
Id                    | `string?`             | Gets or sets an id string applied for the "id" attribute of the split container element.
Style                 | `string?`             | Gets or sets a CSS style string applied for the "style" attribute of the split container element.
Class                 | `string?`             | Gets or sets a CSS class string applied for the "class" attribute of the split container element.
FirstPane             | `RenderFragment`      | The left or top pane in the SplitContainer.
SecondPane            | `RenderFragment`      | The right or bottom pane in the SplitContainer.
Orientation           | `SplitterOrientation` | Determines if the spliter is vertical or horizontal. The default value is `SplitterOrientation.Vertical`.
UnitOfPaneSize        | `UnitOfPaneSize`        | Determines the unit of the pane size whether it is `Pixel` or `Percent`. The default value is `UnitOfPaneSize.Pixel`.
FirstPaneSize         | `TSize?`                | Determines distance of the splitter from the left or top edge.
FirstPaneMinSize      | `TSize?`                | Determines the minimum distance of the splitter from the left or the top edge of first pane.
SecondPaneSize        | `TSize?`                | Determines distance of the splitter from the right or bottom edge.
SecondPaneMinSize     | `TSize?`                | Determines the minimum distance of the splitter from the right or the bottom edge of second pane.
FirstPaneSizeChanged  | `EventCallback<TSize>`  | A callback that will be invoked when the size of the first pane is changed.
SecondPaneSizeChanged | `EventCallback<TSize>`  | A callback that will be invoked when the size of the second pane is changed.

> **Warning**  
> You can specify the pane size to only either the `FirstPaneSize` or the `SecondPaneSize` parameter. If you specify both the `FirstPaneSize` or the `SecondPaneSize` parameters, then the splitter won't work.

### Orientation

The `Orientation` parameter represents the "Splitter Bar" orientation that splits the `SplitContainer`'s client area into two panes. The following figures show the layout you will see with each `SplitterOrientation` enum value set to the `Orientation` parameter.

Vertical | Horizontal
---------|--------------
![Vertical](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.SplitContainer/main/.assets/fig.001.png) | ![Horizontal](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.SplitContainer/main/.assets/fig.002.png)


### Pane size

When you set the `FirstPaneSize` parameter, the first pane will be fixed size, and the second pane will be stretched to fill the remaining area of the `SplitContainer`.

![When the `FirstPaneSize` parameter was set](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.SplitContainer/main/.assets/fig.003.png)

On the other hand, when you set the `SecondPaneSize` parameter, the first pane will be stretched to fill the remaining area of the `SplitContainer`, and the second pane will be fixed size.

![When the `SecondPaneSize` parameter was set](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.SplitContainer/main/.assets/fig.004.png)

⚠️ **Warning**

I strongly recommend binding a pane size parameter to a field variable, like the following example.

```html
<SplitContainer @bind-FirstPaneSize="_PaneSize">
    ...
</SplitContainer>

@code {
    private int _PaneSize = 80;
}
```

When you set a pane size parameter with a literal value, like below, 

```html
<SplitContainer FirstPaneSize="80">
    ...
</SplitContainer>
```

the pane size might be reset unintendedly to that literal value you specified even after a user has operated to resize the pane.

### Save and restore the pane size

Please refer to the following example if you want to save and restore the pane size. The following example shows you how to save and restore the pane size into a web browser's local storage (The example uses the `Blazored.LocalStorage` NuGet package to access the web browser's local storage API).

```razor
@inject ILocalStorageService LocalStorage

<SplitContainer @bind-FirstPaneSize="_PaneSize"
                @bind-FirstPaneSize:after="OnPaneSizeChanged">
    ...
</SplitContainer>

@code {
  private int _PaneSize = 80;

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    if (firstRender)
    {
      var paneSizeStr = await this.LocalStorage.GetItemAsStringAsync("_PaneSize");
      if (int.TryParse(paneSizeStr, out var paneSize)) this._PaneSize = paneSize;
      this.StateHasChanged();
    }
  }

  private async Task OnPaneSizeChanged()
  {
    await this.LocalStorage.SetItemAsStringAsync("_PaneSize", _PaneSize.ToString());
  }
}
```

### Change the size of the splitter bar

The size of the splitter bar is defined by the CSS custom property named `--splitter-bar-size` scoped in the `split-container` CSS class (The default value is '4px'). So you can change the size of the splitter bar by overriding that CSS custom property value like the following example.

```css
::deep .split-container {
    --splitter-bar-size: 14px;
}
```


## Release Note

[Release notes](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.SplitContainer/main/RELEASE-NOTES.txt)

## License

[Mozilla Public License Version 2.0](https://raw.githubusercontent.com/jsakamoto/Toolbelt.Blazor.SplitContainer/main/LICENSE)
