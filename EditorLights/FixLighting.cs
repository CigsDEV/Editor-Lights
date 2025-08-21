using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixLighting : MonoBehaviour
{
    private EnvironmentController ec;

    private PlayerManager pm;

    public static List<GameObject> LightsInGame;

    public static Cell[,] cellsInLevel;

    private Dictionary<IntVector2, List<GameObject>> lightsByPosition;

    private const int MinimumLights = 1;

    private const int MaxAttempts = 5;

    public void Awake()
	{
		//You need the AssemblyC# for refrence.
		bool flag = Singleton<CoreGameManager>.Instance == null;
		if (flag)
		{
			Debug.Log("Init");
		}
		else
		{
			base.StartCoroutine(InitializeLighting());
		}
	}
	
	//does as it says
	private IEnumerator InitializeLighting()
	{
		//wait till ec and pm aren't null
		while (ec == null || pm == null)
		{
			ec = UnityEngine.Object.FindObjectOfType<EnvironmentController>();
			pm = UnityEngine.Object.FindObjectOfType<PlayerManager>(); 
			yield return null;
		}

		//current attempt
		int attempt = 0;

		do
		{
			FindLightsInScene();
			int num = attempt;
			attempt = num + 1;
			bool flag = FixLighting.LightsInGame.Count >= 1;
			if (flag)
			{
				break;
			}
		}
		
		//max attempts is 5
		while (attempt < 5);

		bool flag2 = FixLighting.LightsInGame.Count < 1;
		
		if (flag2)
		{
			Debug.LogWarning(string.Format("Only found {0} lights after {1} attempts.", FixLighting.LightsInGame.Count, attempt));
		}

		//set cells
		FixLighting.cellsInLevel = ec.cells;

		//populate and check
		PopulateLightGrid();
		CheckCellsForLightsUsingGrid();
		
		yield break;
	}

	//this just gets every light within the game, if you want to add more light objects
	//look into having your object's name with "light" in it, it's that simple and is auto registered.
	private void FindLightsInScene()
	{
		FixLighting.LightsInGame = new List<GameObject>();
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
		foreach (GameObject gameObject in array)
		{
			bool flag = gameObject.name.ToLower().Contains("room") && gameObject.name.ToLower().Contains("_");
			if (flag)
			{
				Transform transform = gameObject.transform.Find("RoomObjects");
				bool flag2 = transform != null;
				if (flag2)
				{
					foreach (object obj in transform)
					{
						Transform transform2 = (Transform)obj;
						bool flag3 = transform2.name.ToLower().Contains("light") && !transform2.name.ToLower().Contains("lights");
						if (flag3)
						{
							gameObject.transform.parent = null;
							FixLighting.LightsInGame.Add(transform2.gameObject);
						}
					}
				}
			}
		}
	}

    //populates the light grid grouped by their rounded X/Z world grid cell.
    private void PopulateLightGrid()
    {
        var lights = FixLighting.LightsInGame;
        var map = new Dictionary<IntVector2, List<GameObject>>();

        if (lights == null || lights.Count == 0)
        {
            lightsByPosition = map;
            return;
        }

        foreach (var go in lights)
        {
            Vector3 worldPos = go.transform.position;
            var cellKey = new IntVector2(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));

            if (!map.TryGetValue(cellKey, out var cellList))
            {
                cellList = new List<GameObject>();
                map[cellKey] = cellList;
            }

            cellList.Add(go);
        }

        lightsByPosition = map;
    }


   
    // For each grid cell, attaches any lights mapped at that cell and updates lighting state.
    private void CheckCellsForLightsUsingGrid()
    {
        if (FixLighting.cellsInLevel == null)
        {
            Debug.LogError("cellsInLevel is null!");
            return;
        }
        if (lightsByPosition == null)
        {
            Debug.LogError("lightsByPosition is null!");
            return;
        }
        if (ec == null)
        {
            Debug.LogError("EnvironmentController (ec) is null!");
            return;
        }
        if (ec.lights == null)
        {
            Debug.LogError("ec.lights is null!");
            return;
        }
        if (ec.lightsToFlicker == null)
        {
            Debug.LogError("ec.lightsToFlicker is null!");
            return;
        }
        if (Singleton<CoreGameManager>.Instance == null)
        {
            Debug.LogError("CoreGameManager instance is null!");
            return;
        }

        ec.standardDarkLevel = Color.black; //todo: yeah yeah customizeable i guess fuck you
        ec.InitializeLighting();

        int w = FixLighting.cellsInLevel.GetLength(0);
        int h = FixLighting.cellsInLevel.GetLength(1);
        int added = 0;

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                var cell = FixLighting.cellsInLevel[i, j];
                if (cell == null)
                {
                    Debug.LogError($"Cell at position ({i}, {j}) is null."); //never possible but yknow, just in case
                    continue;
                }

                var tt = cell.TileTransform;
                if (tt == null) continue;

                var lp = tt.localPosition;
                var key = new IntVector2(Mathf.RoundToInt(lp.x), Mathf.RoundToInt(lp.z));

                if (!lightsByPosition.TryGetValue(key, out var lst)) continue;
                if (lst == null)
                {
                    Debug.LogError($"lightsByPosition at {key} is null.");
                    continue;
                }

                foreach (var go in lst)
                {
                    if (go == null)
                    {
                        Debug.LogError($"Light at position {key} is null.");
                        continue;
                    }

                    var wp = go.transform.position;
                    var lightCell = new IntVector2(Mathf.RoundToInt(wp.x), Mathf.RoundToInt(wp.z));

                    if (cell.room == null)
                    {
                        Debug.LogError($"Room is null for cell at position ({i}, {j}).");
                        continue;
                    }
                    if (cell.room.standardLightCells == null)
                    {
                        Debug.LogError($"standardLightCells is null for room at position ({i}, {j}).");
                        continue;
                    }

                    added++;
                    cell.SetLight(true);
                    cell.room.standardLightCells.Add(lightCell);
                    cell.lightStrength = 8; //todo: make this modifiable per level instead?
                    cell.lightColor = SetLightColor(go);

                    ec.lights.Add(cell);
                    ec.lightsToFlicker.Add(cell); //not used but still nessacary!

                    ec.GenerateLight(cell, cell.lightColor, cell.lightStrength);
                    Singleton<CoreGameManager>.Instance.UpdateLighting(cell.lightColor, lightCell);
                }
            }
        }

        ec.lightMode = 0; //todo: maybe make this change depending on user settings
        Singleton<CoreGameManager>.Instance.UpdateLightMap();
    }


    //sets the light color, really hacky honestly
    private Color SetLightColor(GameObject light)
	{
		ColoredLight component = light.GetComponent<ColoredLight>();
		return (component != null) ? component.lightColor : Color.white;
	}

	
}
