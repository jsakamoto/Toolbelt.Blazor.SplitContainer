using Microsoft.JSInterop;

namespace Toolbelt.Blazor.Splitter.V2;

internal class JS
{
    private static bool CacheBustingEnabled() => Environment.GetEnvironmentVariable("TOOLBELT_BLAZOR_SPLITCONTAINER_JSCACHEBUSTING") != "0";

#if NET10_0_OR_GREATER
    internal static Task<IJSObjectReference> ImportScriptAsync(IJSRuntime jsRuntime)
    {
        var cacheBustingQueryAsync = CacheBustingEnabled() ?
            jsRuntime.GetValueAsync<bool>("navigator.onLine").AsTask().ContinueWith(static task => task.Result ? "?v=" + VersionInfo.VersionText : "") :
            Task.FromResult("");

        return cacheBustingQueryAsync
            .ContinueWith(task => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Toolbelt.Blazor.SplitContainer/script.min.js" + task.Result).AsTask())
            .Unwrap();
    }
#else
    internal static async Task<IJSObjectReference> ImportScriptAsync(IJSRuntime jsRuntime)
    {
        var scriptPath = "./_content/Toolbelt.Blazor.SplitContainer/script.min.js";
        var isOnLine = await jsRuntime.InvokeAsync<bool>("Toolbelt.Blazor.getProperty", "navigator.onLine");
        if (isOnLine) scriptPath += "?v=" + VersionInfo.VersionText;
        return await jsRuntime.InvokeAsync<IJSObjectReference>("import", scriptPath);
    }
#endif
}
