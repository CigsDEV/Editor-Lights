using System;
using System.IO;
using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Registers;
using PlusLevelLoader;
using UnityEngine;

namespace EditorLights
{
    [BepInPlugin(Plugin.Guid, Plugin.Name, Plugin.Version)]
    [BepInDependency("mtm101.rulerp.baldiplus.leveleditor", 1)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi", 1)]
    [BepInProcess("BALDI.exe")]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Guid = "cigs.bbplus.editorlights";
        public const string Name = "editor lights extension";
        public const string Version = "0.8.X";

        public static Plugin instance;

        public AssetManager assetMan;
        public string CustomColorsPath;

        string _modPath;

        void Awake()
        {
            instance = this;
            assetMan = new AssetManager();

            _modPath = AssetLoader.GetModPath(this);
            CustomColorsPath = Path.Combine(_modPath, "customcolors.bdh");

            HarmonyExtensions.PatchAllConditionals(new Harmony(Guid));

            AddSpriteFolderToAssetMan("", 40f, _modPath, "EditorUI");
            AddSpriteFolderToAssetMan("", 40f, _modPath, "CustomColors");

            LoadingEvents.RegisterOnAssetsLoaded(Info, PostLoad, true);
        }

        //Adds our shit to the editor
        void PostLoad()
        {
            AddScriptToEditor<FixLighting>("fixlights", "OBJ_fixlights", true, new Vector3(0f, 4f, 0f));

            AddColoredLightToEditor("redlight", "red", "OBJ_redlight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("orangelight", "orange", "OBJ_orangelight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("yellowlight", "yellow", "OBJ_yellowlight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("greenlight", "green", "OBJ_greenlight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("limelight", "lime", "OBJ_limelight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("cyanlight", "cyan", "OBJ_cyanlight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("bluelight", "blue", "OBJ_bluelight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("purplelight", "purple", "OBJ_purplelight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("magentalight", "magenta", "OBJ_magentalight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("pinklight", "pink", "OBJ_pinklight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("whitelight", "white", "OBJ_whitelight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("graylight", "gray", "OBJ_graylight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("blacklight", "black", "OBJ_blacklight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("brownlight", "brown", "OBJ_brownlight", new Vector3(0f, 4f, 0f));
            AddColoredLightToEditor("goldlight", "gold", "OBJ_goldlight", new Vector3(0f, 4f, 0f));
            //! do NOT add custom colors here, just use the .txt
            LoadCustomColors(); //<-- see
        }

        //Loads the colors from the .bdh file
        void LoadCustomColors()
        {
            if (!File.Exists(CustomColorsPath))
            {
                Debug.LogWarning($"Custom colors file not found at {CustomColorsPath}");
                return;
            }

            var lines = File.ReadAllLines(CustomColorsPath);

            foreach (var ln in lines)
            {
                if (string.IsNullOrWhiteSpace(ln) || ln.StartsWith("[")) continue;

                var parts = ln.Split('|');
                if (parts.Length < 2)
                {
                    Debug.LogWarning($"Invalid line in custom colors file: {ln}");
                    continue;
                }

                var name = parts[0].Trim('\'', ' ').ToLowerInvariant();
                var hex = parts[1].Trim('\'', ' ');

                if (!ColorUtility.TryParseHtmlString("#" + hex, out _))
                {
                    Debug.LogWarning($"Failed to parse color '{name}' with hex '{hex}'.");
                    continue;
                }

                var spriteKey = "OBJ_" + name + "light";

                //backup check
                if (assetMan.Get<Sprite>(spriteKey) == null)
                {
                    Debug.LogWarning($"Sprite was null for '{name}', using default 'OBJ_customcolorlight'.");
                    spriteKey = "OBJ_customcolor";
                }
                
                AddColoredLightToEditor(name + "light", name, spriteKey, new Vector3(0f, 4f, 0f));
            }
        }

        //Folder to assets
        void AddSpriteFolderToAssetMan(string prefix, float ppu, params string[] pathSegs)
        {
            var dir = Path.Combine(pathSegs);
            if (!Directory.Exists(dir))
            {
                Debug.LogWarning($"Sprite folder missing: {dir}");
                return;
            }

            var files = Directory.GetFiles(dir);
            foreach (var f in files)
            {
                var tex = AssetLoader.TextureFromFile(f);
                if (tex == null) { Debug.LogWarning($"Failed texture load: {f}"); continue; }

                var spr = AssetLoader.SpriteFromTexture2D(tex, ppu);
                var key = prefix + Path.GetFileNameWithoutExtension(f);
                assetMan.Add<Sprite>(key, spr);
            }
        }

        //Adds a script in general to the editor
        void AddScriptToEditor<T>(string name, string visualSprite, bool isDirection, Vector3 offset) where T : Component
        {
            var go = new GameObject(visualSprite);
            go.transform.position += offset;
            go.transform.localScale = Vector3.one;

            var srObj = new GameObject(visualSprite + "_Sprite");
            srObj.transform.parent = go.transform;
            srObj.transform.localPosition = Vector3.zero;
            srObj.transform.localScale = new Vector3(5f, 5f, 5f);

            var sr = srObj.AddComponent<SpriteRenderer>();
            sr.sprite = assetMan.Get<Sprite>(visualSprite);

            var bc = go.AddComponent<BoxCollider>();
            bc.isTrigger = true;

            Extensions.ConvertToPrefab(go, true);
            BaldiLevelEditorPlugin.editorObjects.Add(
                EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>(name, go, offset, true)
            );

            var runtime = Instantiate(go);
            runtime.name = visualSprite;
            Destroy(runtime.GetComponentInChildren<SpriteRenderer>());
            runtime.AddComponent<T>();

            var bc2 = runtime.AddComponent<BoxCollider>();
            bc2.isTrigger = true;

            Extensions.ConvertToPrefab(runtime, true);
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add(name, runtime);
        }

        //Adds it as a valid tool
        void AddColoredLightToEditor(string name, string colorKey, string visualSprite, Vector3 offset)
        {
            Debug.Log($"{name} {colorKey} {visualSprite}");

            if (!visualSprite.Contains("OBJ_"))
                visualSprite = "OBJ_" + visualSprite;

            var go = new GameObject(name);
            go.transform.position += offset;
            go.transform.localScale = Vector3.one;

            var srObj = new GameObject(visualSprite + "_Sprite");
            srObj.transform.parent = go.transform;
            srObj.transform.localPosition = Vector3.zero;
            srObj.transform.localScale = new Vector3(5f, 5f, 5f);

            var sr = srObj.AddComponent<SpriteRenderer>();
            var spr = assetMan.Get<Sprite>(visualSprite);
            if (spr == null)
                Debug.LogWarning($"Sprite not found for '{visualSprite}'");
            else
                sr.sprite = spr;

            var bc = go.AddComponent<BoxCollider>();
            bc.isTrigger = true;

            Extensions.ConvertToPrefab(go, true);
            BaldiLevelEditorPlugin.editorObjects.Add(
                EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>(name, go, offset, true)
            );

            var runtime = Instantiate(go);
            runtime.name = name;

            var child = runtime.transform.Find(visualSprite + "_Sprite");
            if (child != null) Destroy(child.gameObject);

            var cl = runtime.AddComponent<ColoredLight>();
            cl.Color = colorKey; // parsed at runtime via ColorLookup.TryGet

            Extensions.ConvertToPrefab(runtime, true);
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add(name, runtime);
        }
    }
}
