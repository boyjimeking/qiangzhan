using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExportAssetBundles {
    [MenuItem("Assets/Build AssetBundle From Selection - Track dependencies")]
    static void ExportResource()
    {
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if( path.Length != 0 )
        {
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets
                , BuildTarget.StandaloneWindows);
            Selection.objects = selection;
        }
    }

    [MenuItem("Assets/Build AssetBundle From Scene - Track dependencies")]
    static void ExportScene()
    {
//         string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
//         if (path.Length != 0)
//         {
// //             Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
//             BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets
//                 , BuildTarget.StandaloneWindows);
//             Selection.objects = selection;
//             string[] levels = new string[1];
//             levels[0] = "Assets/Scene/sence_tijiao.unity";
//             BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows, BuildOptions.BuildAdditionalStreamedScenes);
//        }
        
    }
}
