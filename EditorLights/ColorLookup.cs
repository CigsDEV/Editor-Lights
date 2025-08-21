using System.Collections.Generic;
using UnityEngine;

public static class ColorLookup
{
    //new as of current commit, i just hated having this in the same script as ColoredLight
    public static readonly Dictionary<string, Color> colorMap =
        new Dictionary<string, Color>(16, System.StringComparer.OrdinalIgnoreCase)
        {
            ["red"] = Color.red,
            ["orange"] = new Color32(255, 128, 0, 255),
            ["green"] = Color.green,
            ["blue"] = Color.blue,
            ["yellow"] = Color.yellow,
            ["purple"] = new Color32(128, 0, 128, 255),
            ["cyan"] = Color.cyan,
            ["magenta"] = Color.magenta,
            ["pink"] = new Color32(255, 191, 204, 255),
            ["lime"] = new Color32(191, 255, 0, 255),
            ["brown"] = new Color32(166, 41, 41, 255),
            ["black"] = Color.black,
            ["white"] = Color.white,
            ["gray"] = Color.gray,
            ["gold"] = new Color32(255, 214, 0, 255),
        };

    public static bool TryGet(string s, out Color c)
    {
        if (string.IsNullOrEmpty(s)) { c = default; return false; }
        return ColorUtility.TryParseHtmlString(s, out c);
    }
}
