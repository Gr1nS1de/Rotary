using UnityEngine; 
using UnityEditor;

public class CustomAssetMaker 
{
    public static T CreateAsset<T>(string name) where T : ScriptableObject {
        T asset = ScriptableObject.CreateInstance(typeof(T)) as T;
        SerializeAsset(asset, name);
        Selection.activeObject = asset;  
        return asset;
    }
    
    public static void SerializeAsset(ScriptableObject asset, string name) {
        string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (selectedPath.Contains("Assets/")) {
            if (Selection.activeObject.GetType() == typeof(UnityEngine.Object)) {
                selectedPath+="/";
            }
            
            int dir = selectedPath.LastIndexOf('/');
            if (dir != -1) {
                selectedPath = selectedPath.Substring(0, dir + 1);
                Debug.Log(selectedPath);
            } else {
                selectedPath = "Assets/";   
            }
        } else {
            selectedPath = "Assets/";
        }
        AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(selectedPath + name + ".asset")); 
        AssetDatabase.SaveAssets();
    }
}
