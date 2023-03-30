// using System.Collections;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities.Editor;
// using UnityEditor;
// using UnityEngine;

// namespace CustomLODGroup
// {
//     public class LODGroupToolWindow : OdinEditorWindow
//     {
//         // [MenuItem(ToolsDefine.SystemRoot + "LODGroup")]
//         static void OpenWindow()
//         {
//             var window = GetWindow<LODGroupToolWindow>();
//             window.Show();
//         }

//         protected override void OnGUI()
//         {
//             base.OnGUI();

//             EditorGUILayout.LabelField($"总LODGroup数量:{LODGroupManager.Instance.totalCount}");
//         }

//     }
// }
