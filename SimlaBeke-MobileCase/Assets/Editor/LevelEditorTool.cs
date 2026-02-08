using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LevelEditorTool
{
    static GameObject tilePrefab;

    static LevelEditorTool()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.type != EventType.MouseDown || e.button != 0)
            return;

        if (tilePrefab == null)
        {
            tilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Game/Prefabs/LevelEditor/Tile.prefab"
            );
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Vector3 worldPos = ray.origin;

        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.y);

        GameObject tile = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
        tile.transform.position = new Vector3(x, y, 0);

        EditorTile lt = tile.GetComponent<EditorTile>();
        lt.cell = new Vector2Int(x, y);

        e.Use();
    }
}