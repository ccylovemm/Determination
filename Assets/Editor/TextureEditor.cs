 using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

public class TextureEditor : Editor
{
	[MenuItem("TextureEditor/TextureForPC")]
	static void TextureForPC()
	{
		foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
		{
			if (o.GetType() != typeof(Texture2D)) continue;
			Texture2D texture = (Texture2D)o;
			string path = AssetDatabase.GetAssetPath(texture);
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			textureImporter.textureType = TextureImporterType.Advanced;
            textureImporter.generateCubemap = TextureImporterGenerateCubemap.None;
            textureImporter.wrapMode = TextureWrapMode.Clamp;
			textureImporter.isReadable = false;
			textureImporter.mipmapEnabled = false;
			textureImporter.npotScale = TextureImporterNPOTScale.None;
			textureImporter.textureFormat = TextureImporterFormat.RGBA32;
			AssetDatabase.ImportAsset(path);
		}
	}
     
    [MenuItem("TextureEditor/TextureForToSmaller")]
    static void TextureForToSmaller()
    {
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (o.GetType() != typeof(Texture2D)) continue;
            Texture2D texture = (Texture2D)o;
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = TextureImporterType.Advanced;
            textureImporter.generateCubemap = TextureImporterGenerateCubemap.None;
            textureImporter.wrapMode = TextureWrapMode.Clamp;
            textureImporter.isReadable = false;
            textureImporter.mipmapEnabled = false;
            textureImporter.npotScale = TextureImporterNPOTScale.ToSmaller;
            textureImporter.textureFormat = TextureImporterFormat.ETC2_RGBA8;
            AssetDatabase.ImportAsset(path);
        }
    }

}