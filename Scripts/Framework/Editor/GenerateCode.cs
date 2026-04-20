using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace Framework.Editor
{
//Disable Specify StringComparison for clarity
#pragma warning disable CA1307
    public static class GenerateLayers
    {
        #region Constants
        private const string Header = "//Do not edit. This file is auto-generated.";
        #endregion

        #region Menu Items
        [MenuItem("Generate/Generate Layer Enum")]
        public static void GenerateLayerEnum()
        {
            StringBuilder content = new();
            content.AppendLine(Header);
            content.AppendLine();
            content.AppendLine("namespace Framework.Generated");
            content.AppendLine("{");
            content.AppendLine("    #region Enums");
            content.AppendLine("    public enum Layers");
            content.AppendLine("    {");
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (string.IsNullOrEmpty(layerName))
                    continue;


                string sanitized = layerName.Replace(" ", string.Empty);

                content.AppendLine("        " + sanitized + " = " + i + ",");
            }
            content.AppendLine("    }");
            content.AppendLine("    #endregion");
            content.AppendLine("}");
            ExportFile("Layers.cs", content.ToString());
        }

        [MenuItem("Generate/Generate Sorting Layer Enums")]
        public static void GenerateSortingLayerEnums()
        {
            StringBuilder enumIdBuilder = new();
            enumIdBuilder.AppendLine(Header);
            enumIdBuilder.AppendLine();
            enumIdBuilder.AppendLine("namespace Framework.Generated");
            enumIdBuilder.AppendLine("{");
            enumIdBuilder.AppendLine("    #region Enums");
            enumIdBuilder.AppendLine("    public enum SortingLayers");
            enumIdBuilder.AppendLine("    {");

            StringBuilder enumIndexBuilder = new();
            enumIndexBuilder.AppendLine(Header);
            enumIndexBuilder.AppendLine();
            enumIndexBuilder.AppendLine("namespace Framework.Generated");
            enumIndexBuilder.AppendLine("{");
            enumIndexBuilder.AppendLine("    #region Enums");
            enumIndexBuilder.AppendLine("    public enum SortingLayersIndex");
            enumIndexBuilder.AppendLine("    {");

            SortingLayer[] layers = SortingLayer.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                string layerName = layers[i].name;
                if (string.IsNullOrEmpty(layerName))
                    continue;

                string sanitized = layerName.Replace(" ", string.Empty);
                enumIdBuilder.AppendLine("        " + sanitized + " = " + layers[i].id + ",");
                enumIndexBuilder.AppendLine("        " + sanitized + " = " + i + ",");
            }

            enumIdBuilder.AppendLine("    }");
            enumIdBuilder.AppendLine("    #endregion");
            enumIdBuilder.AppendLine("}");
            ExportFile("SortingLayers.cs", enumIdBuilder.ToString());

            enumIndexBuilder.AppendLine("    }");
            enumIndexBuilder.AppendLine("#endregion");
            enumIndexBuilder.AppendLine("}");
            ExportFile("SortingLayersIndex.cs", enumIndexBuilder.ToString());
        }
        #endregion

        #region Private Methods
        private static void ExportFile(string fileName, string content)
        {
            try
            {
                string path = Application.dataPath + "/Scripts/Framework/Generated";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.WriteAllText(Path.Combine(path, fileName), content);
                Debug.Log(fileName + " generated.");
                AssetDatabase.Refresh();
            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
        }
        #endregion
    }
#pragma warning restore CA1307
}
