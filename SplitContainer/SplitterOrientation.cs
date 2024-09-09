using System.ComponentModel;

namespace Toolbelt.Blazor.Splitter;

/// <summary>
/// Specifies the orientation of the spliter.
/// </summary>
[Obsolete("Use the one in the Toolbelt.Blazor.Splitter.V2 namespace instead."), EditorBrowsable(EditorBrowsableState.Never)]
public enum SplitterOrientation
{
    /// <summary>
    ///  The spliter is oriented horizontally.
    /// </summary>
    Horizontal,

    /// <summary>
    ///  The spliter is oriented vertically.
    /// </summary>
    Vertical
}
