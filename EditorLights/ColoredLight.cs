using System;
using System.Collections.Generic;
using UnityEngine;

public class ColoredLight : MonoBehaviour 
{
	private void Awake()
	{
		//! Check if the string is empty before sending out
		if (string.IsNullOrEmpty(this.Color))
		{
			this.Color = "white";
			Debug.LogWarning("Color string was null, defaulting to white. [probably the init]");
		}

		//Sets the gameobject name
		base.name = "Light_" + this.Color;
		this.lightColor = this.DetermineLightColor(this.Color);
		//Debugging
		Debug.Log(string.Format("Set light color: {0}", this.lightColor));
	}

	//Auto determine
	private Color DetermineLightColor(string colorName)
	{
		Color color;
		ColorLookup.TryGet(colorName.ToLower(), out color);
        return (color == default(Color)) ? UnityEngine.Color.white : color;
	}

	public string Color;

	public Color lightColor = UnityEngine.Color.white;

	//! Outdated dictionary
	/*
	public static Dictionary<string, Color> colorMap = new Dictionary<string, Color>
	{
		{
			"red",
			UnityEngine.Color.red
		},
		{
			"orange",
			new Color(1f, 0.5f, 0f)
		},
		{
			"green",
			UnityEngine.Color.green
		},
		{
			"blue",
			UnityEngine.Color.blue
		},
		{
			"yellow",
			UnityEngine.Color.yellow
		},
		{
			"purple",
			new Color(0.5f, 0f, 0.5f)
		},
		{
			"cyan",
			UnityEngine.Color.cyan
		},
		{
			"magenta",
			UnityEngine.Color.magenta
		},
		{
			"pink",
			new Color(1f, 0.75f, 0.8f)
		},
		{
			"lime",
			new Color(0.75f, 1f, 0f)
		},
		{
			"brown",
			new Color(0.65f, 0.16f, 0.16f)
		},
		{
			"black",
			UnityEngine.Color.black
		},
		{
			"white",
			UnityEngine.Color.white
		},
		{
			"gray",
			UnityEngine.Color.gray
		},
		{
			"gold",
			new Color(1f, 0.84f, 0f)
		}
	};
	*/
}
