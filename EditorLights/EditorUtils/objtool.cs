using System;
using BaldiLevelEditor;
using EditorLights;
using UnityEngine;

namespace EditorUtils
{
	//Automatic object script, no change needed.
	//Made by bigthinker!!!! not me. okay
	public class objtool : RotateAndPlacePrefab
	{
		public override Sprite editorSprite
		{
			get
			{
				Sprite result;
				try
				{
					Sprite sprite = Plugin.instance.assetMan.Get<Sprite>("OBJ_" + _object);
					bool flag = sprite == null;
					if (flag)
					{
						Debug.LogError("Error: Sprite for object '" + _object + "' is null! Falling back to 'Spr_null'.");
						result = Plugin.instance.assetMan.Get<Sprite>("null");
					}
					else
					{
						result = sprite;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in objtool.get_editorSprite: " + ex.Message + "\n" + ex.StackTrace);
					result = Plugin.instance.assetMan.Get<Sprite>("null");
				}
				return result;
			}
		}

		public objtool(string prefab) : base(prefab)
		{
			_object = prefab;
		}

		private string _object;
	}
}
