using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CustomLODGroup.InutanEditor
{
    public static class LODGroupGUI
    {
        // Default colors for each LOD group....
        public static readonly Color[] kLODColors =
        {
            new Color(0.4831376f, 0.6211768f, 0.0219608f, 1.0f),
            new Color(0.2792160f, 0.4078432f, 0.5835296f, 1.0f),
            new Color(0.2070592f, 0.5333336f, 0.6556864f, 1.0f),
            new Color(0.5333336f, 0.1600000f, 0.0282352f, 1.0f),
            new Color(0.3827448f, 0.2886272f, 0.5239216f, 1.0f),
            new Color(0.8000000f, 0.4423528f, 0.0000000f, 1.0f),
            new Color(0.4486272f, 0.4078432f, 0.0501960f, 1.0f),
            new Color(0.7749016f, 0.6368624f, 0.0250984f, 1.0f)
        };

        public static readonly Color kCulledLODColor = new Color(.4f, 0f, 0f, 1f);

        public const int kSceneLabelHalfWidth = 100;
        public const int kSceneLabelHeight = 45;
        public const int kSceneHeaderOffset = 40;

        public const int kSliderBarTopMargin = 18;
        public const int kSliderBarHeight = 30;
        public const int kSliderBarBottomMargin = 16;

        public const int kRenderersButtonHeight = 60;
        public const int kButtonPadding = 2;
        public const int kDeleteButtonSize = 20;

        public const int kSelectedLODRangePadding = 3;

        public const int kRenderAreaForegroundPadding = 3;

        public class GUIStyles
        {
            public readonly GUIStyle m_LODSliderBG = "LODSliderBG";
            public readonly GUIStyle m_LODSliderRange = "LODSliderRange";
            public readonly GUIStyle m_LODSliderRangeSelected = "LODSliderRangeSelected";
            public readonly GUIStyle m_LODSliderText = "LODSliderText";
            public readonly GUIStyle m_LODSliderTextSelected = "LODSliderTextSelected";
            public readonly GUIStyle m_LODStandardButton = "Button";
            public readonly GUIStyle m_LODRendererButton = "LODRendererButton";
            public readonly GUIStyle m_LODRendererAddButton = "LODRendererAddButton";
            public readonly GUIStyle m_LODRendererRemove = "LODRendererRemove";
            public readonly GUIStyle m_LODBlackBox = "LODBlackBox";
            public readonly GUIStyle m_LODCameraLine = "LODCameraLine";

            public readonly GUIStyle m_LODSceneText = "LODSceneText";
            public readonly GUIStyle m_LODRenderersText = "LODRenderersText";
            public readonly GUIStyle m_LODLevelNotifyText = "LODLevelNotifyText";

            public readonly GUIContent m_IconRendererPlus = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add New Renderers");
            public readonly GUIContent m_IconRendererMinus = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove Renderer");
            public readonly GUIContent m_CameraIcon = EditorGUIUtility.IconContent("Camera Icon");

            public readonly GUIContent m_UploadToImporter = EditorGUIUtility.TrTextContent("Upload to Importer", "Upload the modified screen percentages to the model importer.");
            public readonly GUIContent m_UploadToImporterDisabled = EditorGUIUtility.TrTextContent("Upload to Importer", "Number of LOD's in the scene instance differ from the number of LOD's in the imported model.");
            public readonly GUIContent m_RecalculateBounds = EditorGUIUtility.TrTextContent("Recalculate Bounds", "Recalculate bounds to encapsulate all child renderers.");
            public readonly GUIContent m_RecalculateBoundsDisabled = EditorGUIUtility.TrTextContent("Recalculate Bounds", "Bounds are already up-to-date.");
            public readonly GUIContent m_LightmapScale = EditorGUIUtility.TrTextContent("Recalculate Lightmap Scale", "Set the lightmap scale to match the LOD percentages.");
            public readonly GUIContent m_RendersTitle = EditorGUIUtility.TrTextContent("Renderers");

            public readonly GUIContent m_AnimatedCrossFadeInvalidText = EditorGUIUtility.TrTextContent("Animated cross-fading is currently disabled. Please enable \"Animate Between Next LOD\" on either the current or the previous LOD.");
            public readonly GUIContent m_AnimatedCrossFadeInconsistentText = EditorGUIUtility.TrTextContent("Animated cross-fading is currently disabled. \"Animate Between Next LOD\" is enabled but the next LOD is not in Animated Cross Fade mode.");
            public readonly GUIContent m_AnimateBetweenPreviousLOD = EditorGUIUtility.TrTextContent("Animate Between Previous LOD", "Cross-fade animation plays when transits between this LOD and the previous (lower) LOD.");

            public readonly GUIContent m_LODObjectSizeLabel = EditorGUIUtility.TrTextContent("Object Size", "The Object size in local space. This is used to calculate the relative screen height for the object.");
            public readonly GUIContent m_ResetObjectSizeLabel = EditorGUIUtility.TrTextContent("Reset Object Size", "Resets the Object Size in Local Space to 1 and preserves LOD distances.");
            public readonly GUIContent m_LODDistancesInRelativeSizeLabel = EditorGUIUtility.TrTextContent("LOD Distances in Screen Relative Size");
            public readonly GUIContent m_LODSetToCameraLabel = EditorGUIUtility.TrTextContent("Set to Camera");

            public readonly GUIContent m_LODModeLabel = EditorGUIUtility.TrTextContent("LOD Mode", "LOD Mode.");
            public readonly GUIContent m_LODTransitionPercentageLabel = EditorGUIUtility.TrTextContent("Transition (% Screen Size)", "This value marks where LOD level transitions into a lower LOD level.");
            public static GUIContent m_TriangleCountLabel = EditorGUIUtility.TrTextContent("Triangles");
            public static GUIContent m_VertexCountLabel = EditorGUIUtility.TrTextContent("Vertices");

            public static GUIContent m_DistanceInMetersLabel = EditorGUIUtility.TrTextContent("-", "The displayed distance depends on the current Scene View camera settings and might be different in Game View.");

            public static GUIStyle m_InspectorTitlebarFlat;


            public static GUIContent m_BlueBorderTextureSelected = EditorGUIUtility.TrIconContent("AnimationRowOddSelected");
            public static GUIContent m_BlueBorderTextureNormal = EditorGUIUtility.TrIconContent("OL title act");

            public GUIStyles()
            {
                m_InspectorTitlebarFlat = new GUIStyle(EditorStyles.inspectorDefaultMargins);
                m_InspectorTitlebarFlat.focused.textColor = m_InspectorTitlebarFlat.normal.textColor;
            }

        }

        private static GUIStyles s_Styles;

        public static GUIStyles Styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new GUIStyles();
                }
                return s_Styles;
            }
        }

        public static float DelinearizeScreenPercentage(float percentage)
        {
            if (Mathf.Approximately(0.0f, percentage))
                return 0.0f;

            return Mathf.Sqrt(percentage);
        }

        public static float LinearizeScreenPercentage(float percentage)
        {
            return percentage * percentage;
        }

        public static Rect CalcLODButton(Rect totalRect, float percentage)
        {
            return new Rect(totalRect.x + (Mathf.Round(totalRect.width * (1.0f - percentage))) - 5, totalRect.y, 10, totalRect.height);
        }

        public static Rect GetCulledBox(Rect totalRect, float previousLODPercentage)
        {
            var r = CalcLODRange(totalRect, previousLODPercentage, 0.0f);
            r.height -= 2;
            r.width -= 1;
            r.center += new Vector2(0f, 1.0f);
            return r;
        }


        public class LODInfo
        {
            public Rect m_ButtonPosition;
            public Rect m_RangePosition;

            public LODInfo(int lodLevel, string name, float screenPercentage)
            {
                LODLevel = lodLevel;
                LODName = name;
                RawScreenPercent = screenPercentage;
            }

            public int LODLevel { get; private set; }
            public string LODName { get; private set; }
            public float RawScreenPercent { get; set; }

            public float ScreenPercent
            {
                get { return DelinearizeScreenPercentage(RawScreenPercent); }
                set { RawScreenPercent = LinearizeScreenPercentage(value); }
            }
        }

        public static List<LODInfo> CreateLODInfos(int numLODs, Rect area, Func<int, string> nameGen, Func<int, float> heightGen)
        {
            var lods = new List<LODInfo>();

            for (int i = 0; i < numLODs; ++i)
            {
                var lodInfo = new LODInfo(i, nameGen(i), heightGen(i));
                lodInfo.m_ButtonPosition = CalcLODButton(area, lodInfo.ScreenPercent);
                var previousPercentage = i == 0 ? 1.0f : lods[i - 1].ScreenPercent;
                lodInfo.m_RangePosition = CalcLODRange(area, previousPercentage, lodInfo.ScreenPercent);
                lods.Add(lodInfo);
            }

            return lods;
        }

        public static float GetCameraPercent(Vector2 position, Rect sliderRect)
        {
            var percentage = Mathf.Clamp(1.0f - (position.x - sliderRect.x) / sliderRect.width, 0.01f, 1.0f);
            percentage = LODGroupGUI.LinearizeScreenPercentage(percentage);
            return percentage;
        }

        public static void SetSelectedLODLevelPercentage(float newScreenPercentage, int lod, List<LODInfo> lods)
        {
            // Find the lower detail lod... clamp value to stop overlapping slider
            var minimum = 0.0f;
            var lowerLOD = lods.FirstOrDefault(x => x.LODLevel == lods[lod].LODLevel + 1);
            if (lowerLOD != null)
                minimum = lowerLOD.RawScreenPercent;

            // Find the higher detail lod... clamp value to stop overlapping slider
            var maximum = 1.0f;
            var higherLOD = lods.FirstOrDefault(x => x.LODLevel == lods[lod].LODLevel - 1);
            if (higherLOD != null)
                maximum = higherLOD.RawScreenPercent;

            maximum = Mathf.Clamp01(maximum);
            minimum = Mathf.Clamp01(minimum);

            // Set that value
            lods[lod].RawScreenPercent = Mathf.Clamp(newScreenPercentage, minimum, maximum);
        }

        public static void DrawLODSlider(Rect area, IList<LODInfo> lods, int selectedLevel)
        {
            Styles.m_LODSliderBG.Draw(area, GUIContent.none, false, false, false, false);
            for (int i = 0; i < lods.Count; i++)
            {
                var lod = lods[i];
                DrawLODRange(lod, i == 0 ? 1.0f : lods[i - 1].RawScreenPercent, i == selectedLevel);
                DrawLODButton(lod);
            }

            // Draw the last range (culled)
            DrawCulledRange(area, lods.Count > 0 ? lods[lods.Count - 1].RawScreenPercent : 1.0f);
        }

        private static Rect CalcLODRange(Rect totalRect, float startPercent, float endPercent)
        {
            var startX = Mathf.Round(totalRect.width * (1.0f - startPercent));
            var endX = Mathf.Round(totalRect.width * (1.0f - endPercent));

            return new Rect(totalRect.x + startX, totalRect.y, endX - startX, totalRect.height);
        }

        private static void DrawLODButton(LODInfo currentLOD)
        {
            // Make the lod button areas a horizonal resizer
            EditorGUIUtility.AddCursorRect(currentLOD.m_ButtonPosition, MouseCursor.ResizeHorizontal);
        }

        private static void DrawLODRange(LODInfo currentLOD, float previousLODPercentage, bool isSelected)
        {
            var tempColor = GUI.backgroundColor;
            var startPercentageString = string.Format("{0}\n{1:0}%", currentLOD.LODName, previousLODPercentage * 100);
            if (isSelected)
            {
                var foreground = currentLOD.m_RangePosition;
                foreground.width -= kSelectedLODRangePadding * 2;
                foreground.height -= kSelectedLODRangePadding * 2;
                foreground.center += new Vector2(kSelectedLODRangePadding, kSelectedLODRangePadding);
                Styles.m_LODSliderRangeSelected.Draw(currentLOD.m_RangePosition, GUIContent.none, false, false, false, false);
                GUI.backgroundColor = kLODColors[currentLOD.LODLevel];
                if (foreground.width > 0)
                    Styles.m_LODSliderRange.Draw(foreground, GUIContent.none, false, false, false, false);
                Styles.m_LODSliderText.Draw(currentLOD.m_RangePosition, startPercentageString, false, false, false, false);
            }
            else
            {
                GUI.backgroundColor = kLODColors[currentLOD.LODLevel];
                GUI.backgroundColor *= 0.6f;
                Styles.m_LODSliderRange.Draw(currentLOD.m_RangePosition, GUIContent.none, false, false, false, false);
                Styles.m_LODSliderText.Draw(currentLOD.m_RangePosition, startPercentageString, false, false, false, false);
            }
            GUI.backgroundColor = tempColor;
        }

        private static void DrawCulledRange(Rect totalRect, float previousLODPercentage)
        {
            if (Mathf.Approximately(previousLODPercentage, 0.0f)) return;

            var r = GetCulledBox(totalRect, DelinearizeScreenPercentage(previousLODPercentage));
            // Draw the range of a lod level on the slider
            var tempColor = GUI.color;
            GUI.color = kCulledLODColor;
            Styles.m_LODSliderRange.Draw(r, GUIContent.none, false, false, false, false);
            GUI.color = tempColor;

            // Draw some details for the current marker
            var startPercentageString = string.Format("Culled\n{0:0}%", previousLODPercentage * 100);
            Styles.m_LODSliderText.Draw(r, startPercentageString, false, false, false, false);
        }

        private static readonly int s_FoldoutHeaderHash = "FoldoutHeader".GetHashCode();

        internal static void DrawRoundedBoxAroundLODDFoldout(int lodGroupIndex, int activeLOD)
        {
            Texture borderTexture = lodGroupIndex == activeLOD ? GUIStyles.m_BlueBorderTextureSelected.image : GUIStyles.m_BlueBorderTextureNormal.image;


            // EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var rect = GUILayoutUtility.GetRect(GUIContent.none, LODGroupGUI.GUIStyles.m_InspectorTitlebarFlat);
            // EditorGUILayout.EndVertical();
            // GUILayout.BeginArea(rect);
            // var rect = EditorGUILayout.GetControlRect();
            // GUILayoutGroup g = GUILayoutUtility.BeginLayoutGroup(EditorStyles.helpBox, null, typeof(GUILayoutGroup));
            // g.isVertical = true;

            // GUIUtility.CheckOnGUI();
            if (Event.current.type == EventType.Repaint)
            {
                GUI.DrawTexture(rect, borderTexture, ScaleMode.StretchToFill, true, 1, Color.white, 1, 3);
            }

            // GUILayout.EndArea();
        }

        internal static bool FoldoutHeaderGroupInternal(Rect position, bool foldout, string label, Texture2D background, Color backgroundColor, string additionalLabel = "")
        {
            GUIStyle foldoutStyle = EditorStyles.foldout;//titlebarFoldout
            foldoutStyle.alignment = TextAnchor.MiddleLeft;

            var offset = 24;
            position.x += offset;

            // add some spaces so that we could draw a colored texture before the label
            label = $"             {label}";
            GUIContent content = new GUIContent(label);
            // Removing the default margin for inspectors
            if (EditorGUIUtility.hierarchyMode)
            {
                position.xMin -= EditorStyles.inspectorDefaultMargins.padding.left - EditorStyles.inspectorDefaultMargins.padding.right;
                position.xMax += EditorStyles.inspectorDefaultMargins.padding.right;
            }


            //额外信息
            var labelSize = GUI.skin.label.CalcSize(new GUIContent(additionalLabel));
            Rect menuRect = new Rect
            {
                x = position.xMax - foldoutStyle.padding.right - labelSize.x - offset,
                y = position.y + 2,
                size = labelSize
            };


            if (additionalLabel != null && Event.current.type == EventType.Repaint && labelSize.x < Screen.width * 0.8f)
            {
                GUI.Label(menuRect, additionalLabel);
                menuRect.x = 14 + offset;
                // menuRect.y += 1;
                menuRect.width = menuRect.height = 16;
                GUI.DrawTexture(menuRect, background, ScaleMode.ScaleToFit, true, 1, backgroundColor, 0, 2);
            }


            int id = GUIUtility.GetControlID(s_FoldoutHeaderHash, FocusType.Keyboard, position);
            if (Event.current.type == EventType.KeyDown && GUIUtility.keyboardControl == id)
            {
                KeyCode kc = Event.current.keyCode;
                if (kc == KeyCode.LeftArrow && foldout || (kc == KeyCode.RightArrow && foldout == false))
                {
                    foldout = !foldout;
                    Debug.LogError(111);
                    GUI.changed = true;
                    Event.current.Use();
                }
            }
            else
            {
                // foldout = EditorGUIInternal.DoToggleForward(position, id, foldout, content, foldoutStyle);
                position.x -= 10;
                foldout = EditorGUI.Toggle(position, "", foldout, foldoutStyle);
                EditorGUI.LabelField(position, content);
            }

            return foldout;
        }
    }
}
