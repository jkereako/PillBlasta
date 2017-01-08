using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor: Editor {

  public override void OnInspectorGUI() {
    MapGenerator map = target as MapGenerator;

    // Generate the map when a value changes.
    if (DrawDefaultInspector()) {
      map.GenerateMap();
    }

    // Generate the map when the custom button "Generate Map" is pressed.
    else if (GUILayout.Button("Generate Map")) {
      map.GenerateMap();
    }
  }
}
