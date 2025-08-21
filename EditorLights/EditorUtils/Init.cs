using System;
using System.Collections.Generic;
using BaldiLevelEditor;
using HarmonyLib;
using UnityEngine;

namespace EditorUtils
{
	[HarmonyPatch(typeof(PlusLevelEditor), "Initialize")]
	internal class Init
	{
		//Add our tools to the editor, this is probably what you're looking for.
		[HarmonyPrefix]
		private static void Prefix(PlusLevelEditor __instance)
		{
			ToolCategory toolCategory = __instance.toolCats.Find((ToolCategory x) => x.name == "connectables"); //sdiybt
			toolCategory.tools.Add(new objtool("fixlights"));
			toolCategory.tools.Add(new objtool("flickerlights")); //mtm please in the v2 of the editor make it way easier to add custom objects. pleas.e please do not make us do this stupid ass system
			toolCategory.tools.Add(new objtool("redlight"));
			toolCategory.tools.Add(new objtool("orangelight"));
			toolCategory.tools.Add(new objtool("yellowlight"));
			toolCategory.tools.Add(new objtool("greenlight"));
			toolCategory.tools.Add(new objtool("limelight"));
			toolCategory.tools.Add(new objtool("cyanlight"));
			toolCategory.tools.Add(new objtool("bluelight"));
			toolCategory.tools.Add(new objtool("purplelight"));
			toolCategory.tools.Add(new objtool("magentalight"));
			toolCategory.tools.Add(new objtool("pinklight"));
			toolCategory.tools.Add(new objtool("whitelight"));
			toolCategory.tools.Add(new objtool("graylight"));
			toolCategory.tools.Add(new objtool("blacklight"));
			toolCategory.tools.Add(new objtool("brownlight"));
			toolCategory.tools.Add(new objtool("goldlight"));

			List<string> defaultColors = Init.GetDefaultColors();
            
			foreach (var k in ColorLookup.colorMap.Keys)
            {
                var t = Init.RemoveSpaces(k).ToLowerInvariant();
                if (!Init.IsDefaultColor(t))
                {
                    var n = t + "light";
                    toolCategory.tools.Add(new objtool(n));
                    Debug.Log(n);
                }
            }

        }

        private static List<string> GetDefaultColors()
		{
			return new List<string>
			{
				"red",
				"orange",
				"yellow",
				"green",
				"lime",
				"cyan",
				"blue",
				"purple",
				"magenta",
				"pink",
				"white",
				"gray",
				"black",
				"brown",
				"gold"
			};
		}

		private static bool IsDefaultColor(string colorName)
		{
			return Init.GetDefaultColors().Contains(colorName);
		}

		private static string RemoveSpaces(string colorName)
		{
			return colorName.Replace(" ", "");
		}
	}
}
