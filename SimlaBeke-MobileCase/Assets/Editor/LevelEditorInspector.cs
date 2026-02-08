using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelEditorComponent))]
public class LevelEditorInspector : Editor
{
    LevelEditorComponent editor;

    void OnEnable()
    {
        editor = (LevelEditorComponent)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (editor.levelData == null)
            return;

        var level = editor.levelData;
        level.EnsureGrid();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level Grid", EditorStyles.boldLabel);

        for (int y = level.gridHeight - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < level.gridWidth; x++)
            {
                int index = y * level.gridWidth + x;
                TileData tile = level.tiles[index];

                GUI.backgroundColor = tile != null ? tile.tileColor : Color.black;

                if (GUILayout.Button(tile != null ? tile.tileId : "",
                        GUILayout.Width(40), GUILayout.Height(40)))
                {
                    level.tiles[index] = editor.selectedTile;
                    EditorUtility.SetDirty(level);
                }

                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}