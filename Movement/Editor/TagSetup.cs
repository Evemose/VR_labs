using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor utility to automatically add required tags to the project
/// This will run automatically when Unity compiles scripts
/// </summary>
[InitializeOnLoad]
public static class TagSetup
{
    private static readonly string[] RequiredTags =
    {
        "Ground",
        "Collectible",
        "Obstacle",
        "Player"
    };

    static TagSetup()
    {
        AddRequiredTags();
    }

    [MenuItem("Tools/Lab2/Add Required Tags")]
    public static void AddRequiredTags()
    {
        var tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        var tagsProp = tagManager.FindProperty("tags");

        var addedCount = 0;

        foreach (var tag in RequiredTags)
        {
            var found = false;
            for (var i = 0; i < tagsProp.arraySize; i++)
            {
                var t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tag))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = tag;
                addedCount++;
            }
        }

        tagManager.ApplyModifiedProperties();

        Debug.Log(addedCount > 0
            ? $"[Lab2 TagSetup] Added {addedCount} required tags to the project."
            : "[Lab2 TagSetup] All required tags already exist.");
    }
}