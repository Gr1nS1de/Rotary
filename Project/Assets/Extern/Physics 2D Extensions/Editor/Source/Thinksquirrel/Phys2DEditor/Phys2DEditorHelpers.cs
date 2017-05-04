//
// P2DEditorHelpers.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using System.IO;
using Thinksquirrel.Phys2D;
using Thinksquirrel.Phys2DEditor;
using UnityEditor;
using UnityEngine;

namespace Thinksquirrel.Phys2DEditor
{
    static class Phys2DEditorHelpers
    {
        // TODO - indents could be done a bit better, this won't support deep indents
        public static bool DrawDefaultInspector(SerializedObject serializedObject, params string[] propertiesToSkip)
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            var iterator = serializedObject.FindProperty("m_Script");
            bool enterChildren = true;
            int childCount = -1;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.name != "m_BreakForce" && iterator.name != "m_BreakTorque")
                {
                    bool skip = false;

                    for(int i = 0; i < propertiesToSkip.Length; ++i)
                    {
                        if (iterator.name == propertiesToSkip[i])
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip)
                        continue;

                    int count = 0;

                    try
                    {
                        count = iterator.Copy().CountInProperty() - 1;
                        enterChildren = EditorGUILayout.PropertyField(iterator, enterChildren);
                    }
                    catch
                    {
                        continue;
                    }


                    // At a parent object, indent
                    if (count > 0)
                    {
                        childCount = count;
                        EditorGUI.indentLevel++;
                    }

                    // Went through all child objects, remove indent
                    if (childCount == 0)
                    {
                        childCount = -1;
                        EditorGUI.indentLevel--;
                    }
                    else if (childCount > 0)
                    {
                        childCount--;
                    }
                }
            }

            var breakForceProperty = serializedObject.FindProperty("m_BreakForce");
            var breakTorqueProperty = serializedObject.FindProperty("m_BreakTorque");

            if (breakForceProperty != null && !System.Array.Exists(propertiesToSkip, s => s == "m_BreakForce"))
                EditorGUILayout.PropertyField(breakForceProperty, false);
            if (breakTorqueProperty != null  && !System.Array.Exists(propertiesToSkip, s => s == "m_BreakTorque"))
                EditorGUILayout.PropertyField(breakTorqueProperty, false);

            serializedObject.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        public static int DrawBitMaskField(Rect position, int mask, System.Type type, GUIContent label)
        {
            var itemNames = System.Enum.GetNames(type);
            var itemValues = System.Enum.GetValues(type) as int[];

            int val = mask;
            int maskVal = 0;
            for (int i = 0; i < itemValues.Length; i++)
            {
                if (itemValues [i] != 0)
                {
                    if ((val & itemValues [i]) == itemValues [i])
                        maskVal |= 1 << i;
                }
                else
                if (val == 0)
                    maskVal |= 1 << i;
            }
            int newMaskVal = EditorGUI.MaskField(position, label, maskVal, itemNames);
            int changes = maskVal ^ newMaskVal;

            for (int i = 0; i < itemValues.Length; i++)
            {
                if ((changes & (1 << i)) != 0)            // has this list item changed?
                {
                    if ((newMaskVal & (1 << i)) != 0)     // has it been set?
                    {
                        if (itemValues [i] == 0)           // special case: if "0" is set, just set the val to 0
                        {
                            val = 0;
                            break;
                        }
                        val |= itemValues [i];
                    }
                    else                                  // it has been reset
                    {
                        val &= ~itemValues [i];
                    }
                }
            }
            return val;
        }
    }

    [CustomPropertyDrawer(typeof(BitMaskAttribute))]
    class EnumBitMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
        {
            var typeAttr = attribute as BitMaskAttribute;
            // Add the actual int value behind the field name
            if (prop.hasMultipleDifferentValues)
            {
                label.text += "*";
            }

            bool changed = GUI.changed;
            GUI.changed = false;

            int v = Phys2DEditorHelpers.DrawBitMaskField(position, prop.intValue, typeAttr.propertyType, label);

            if (GUI.changed)
            {
                prop.intValue = v;
            }

            GUI.changed = changed;
        }
    }
}
#region Helper class for JavaScript installation
[InitializeOnLoad]
static class JavaScriptInstaller
{
    static JavaScriptInstaller()
    {
        if (EditorPrefs.GetBool("P2D.InstallForJavaScript", false))
            EditorApplication.update += DoJavaScriptInstaller;
    }

    static void DoJavaScriptInstaller()
    {
        EditorApplication.update -= DoJavaScriptInstaller;
        EditorPrefs.DeleteKey("P2D.InstallForJavaScript");

        bool isWindows = Application.platform == RuntimePlatform.WindowsEditor;
        string dataPath, plugins, pluginsP2D, pluginsP2DRel, P2DMainRel, error;

        // Get paths
        dataPath = isWindows ? Application.dataPath.Replace("/", "\\") : Application.dataPath;
        plugins = Path.Combine(dataPath, "Plugins");
        pluginsP2D = Path.Combine(plugins, "Physics 2D Extensions");
        P2DMainRel = "Assets/Physics 2D Extensions/_Main/";
        pluginsP2DRel = "Assets/Plugins/Physics 2D Extensions/";

        // Delete any old Physics 2D Extensions DLL files in the plugins folder
        if (Directory.Exists(pluginsP2D))
        {
            bool del = false;

            foreach(string file in Directory.GetFiles(pluginsP2D, "P2D.Runtime*", SearchOption.AllDirectories))
            {
                del = true;
                File.Delete(file);
            }

            if (del)
                AssetDatabase.Refresh();
        }

        // Check to see if the plugins folder exists
        if (!Directory.Exists(plugins))
        {
            AssetDatabase.CreateFolder("Assets", "Plugins");
        }

        // Check to see if the Physics 2D Extensions folder exists under the plugins folder
        if (!Directory.Exists(pluginsP2D))
        {
            AssetDatabase.CreateFolder("Assets/Plugins", "Physics 2D Extensions");
        }

        error = AssetDatabase.MoveAsset(P2DMainRel + "P2D.Runtime.dll", pluginsP2DRel + "P2D.Runtime.dll");

        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError("Unable to move P2D.Runtime.dll: " + error);
        }

        /*
        error = AssetDatabase.MoveAsset(P2DMainRel + "P2D.Runtime.xml", pluginsP2DRel + "P2D.Runtime.xml");

        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError("Unable to move P2D.Runtime.xml: " + error);
        }
        */

        AssetDatabase.Refresh();
    }
}
#endregion
