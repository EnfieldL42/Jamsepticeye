using UnityEditor;
using UnityEngine.UIElements;

namespace TinyGiantStudio.BetterInspector
{
    public static class StyleSheetsManager
    {
        static StyleSheet animatedFoldoutStyleSheet;

        const string AnimatedFoldoutStyleSheetFileLocation =
            "Assets/Plugins/Tiny Giant Studio/Better Inspector/Common Scripts/Editor/StyleSheets/Foldout/Foldout Animation.uss";

        const string AnimatedFoldoutStyleSheetGuid = "920070771e2f6c747b78ea05534a8a79";


        static StyleSheet foldoutStyleSheet1;

        const string FileLocationFoldoutStyle1 =
            "Assets/Plugins/Tiny Giant Studio/Better Inspector/Common Scripts/Editor/StyleSheets/Foldout/Foldout Style 1.uss";

        const string GuidForFoldoutStyle1 = "46a1e9f07dfc8da49a73be794970c7b2";


        static StyleSheet foldoutStyleSheet2;

        const string FileLocationFoldoutStyle2 =
            "Assets/Plugins/Tiny Giant Studio/Better Inspector/Common Scripts/Editor/StyleSheets/Foldout/Foldout Style 2.uss";

        const string GuidForFoldoutStyle2 = "fa4dbc50196807549b696fe088efb36f";


        static StyleSheet foldoutStyleSheet3;

        const string FileLocationFoldoutStyle3 =
            "Assets/Plugins/Tiny Giant Studio/Better Inspector/Common Scripts/Editor/StyleSheets/Foldout/Foldout Style 3.uss";

        const string GuidForFoldoutStyle3 = "778d4ba8ba5039540a0a6445f96542ac";


        static StyleSheet buttonStyleSheet1;

        const string FileLocationButtonStyleSheet1 =
            "Assets/Plugins/Tiny Giant Studio/Better Inspector/Common Scripts/Editor/StyleSheets/Button/Button Style 1.uss";

        const string GuidForButtonStyleSheet1 = "59803e0920eb13142ab503bd9be870fe";

        static StyleSheet buttonStyleSheet2;

        const string FileLocationButtonStyleSheet2 =
            "Assets/Plugins/Tiny Giant Studio/Better Inspector/Common Scripts/Editor/StyleSheets/Button/Button Style 2.uss";

        const string GuidForButtonStyleSheet2 = "2d050522d26d0fc449b98c46647dd990";
        
        static StyleSheet buttonStyleSheet3;

        const string FileLocationButtonStyleSheet3 =
            "Assets/Plugins/Tiny Giant Studio/Better Inspector/Common Scripts/Editor/StyleSheets/Button/Button Style 3.uss";

        const string GuidForButtonStyleSheet3 = "ebf57d3250622714996e19c09ba25c12";


        public static void UpdateStyleSheet(VisualElement root)
        {
            animatedFoldoutStyleSheet = GetStyleSheet(animatedFoldoutStyleSheet, AnimatedFoldoutStyleSheetFileLocation,
                AnimatedFoldoutStyleSheetGuid);

            if (animatedFoldoutStyleSheet != null)
                if (BetterInspectorEditorSettings.instance.useAnimatedFoldoutStyleSheet)
                    root.styleSheets.Add(animatedFoldoutStyleSheet);

            UpdateFoldoutStyle(root);
            UpdateButtonStyle(root);
        }

        static void UpdateFoldoutStyle(VisualElement root)
        {
            foldoutStyleSheet1 = GetStyleSheet(foldoutStyleSheet1, FileLocationFoldoutStyle1, GuidForFoldoutStyle1);
            foldoutStyleSheet2 = GetStyleSheet(foldoutStyleSheet2, FileLocationFoldoutStyle2, GuidForFoldoutStyle2);
            foldoutStyleSheet3 = GetStyleSheet(foldoutStyleSheet3, FileLocationFoldoutStyle3, GuidForFoldoutStyle3);

            if (!foldoutStyleSheet1 || !foldoutStyleSheet2 || !foldoutStyleSheet3) return;

            switch (BetterInspectorEditorSettings.instance.selectedFoldoutStyle)
            {
                case 1:
                    root.styleSheets.Add(foldoutStyleSheet1);
                    root.styleSheets.Remove(foldoutStyleSheet2);
                    root.styleSheets.Remove(foldoutStyleSheet3);
                    break;
                case 2:
                    root.styleSheets.Remove(foldoutStyleSheet1);
                    root.styleSheets.Add(foldoutStyleSheet2);
                    root.styleSheets.Remove(foldoutStyleSheet3);
                    break;
                case 3:
                    root.styleSheets.Remove(foldoutStyleSheet1);
                    root.styleSheets.Remove(foldoutStyleSheet2);
                    root.styleSheets.Add(foldoutStyleSheet3);
                    break;
            }
        }

        static void UpdateButtonStyle(VisualElement root)
        {
            buttonStyleSheet1 =
                GetStyleSheet(buttonStyleSheet1, FileLocationButtonStyleSheet1, GuidForButtonStyleSheet1);
            buttonStyleSheet2 =
                GetStyleSheet(buttonStyleSheet2, FileLocationButtonStyleSheet2, GuidForButtonStyleSheet2);
            buttonStyleSheet3 =
                GetStyleSheet(buttonStyleSheet3, FileLocationButtonStyleSheet3, GuidForButtonStyleSheet3);
            // Debug.Log(AssetDatabase.GUIDFromAssetPath(FileLocationButtonStyleSheet3));

            if (!buttonStyleSheet1 || !buttonStyleSheet2 || !buttonStyleSheet3) return;

            switch (BetterInspectorEditorSettings.instance.selectedButtonStyle)
            {
                case 1:
                    root.styleSheets.Add(buttonStyleSheet1);
                    root.styleSheets.Remove(buttonStyleSheet2);
                    root.styleSheets.Remove(buttonStyleSheet3);
                    break;
                case 2:
                    root.styleSheets.Remove(buttonStyleSheet1);
                    root.styleSheets.Add(buttonStyleSheet2);
                    root.styleSheets.Remove(buttonStyleSheet3);
                    break;
                case 3:
                    root.styleSheets.Remove(buttonStyleSheet1);
                    root.styleSheets.Remove(buttonStyleSheet2);
                    root.styleSheets.Add(buttonStyleSheet3);
                    break;
            }
        }


        /// <summary>
        /// If the style sheet isn't loaded yet, loads it from the given location.
        /// If it isn't found at the location, load it using GUID 
        /// </summary>
        static StyleSheet GetStyleSheet(StyleSheet currentStyleSheet, string location, string guid)
        {
            if (currentStyleSheet) return currentStyleSheet;

            currentStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(location);
            if (currentStyleSheet) return currentStyleSheet;

            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return string.IsNullOrEmpty(assetPath) ? null : AssetDatabase.LoadAssetAtPath<StyleSheet>(assetPath);
        }
    }
}