using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;

public class StickerImportSettings : AssetPostprocessor
{
    // const
    private const string StickerAssetPath = "Assets/Content/GameScene/StickerPacks/Textures/new";
    private const string PacksAssetPath = "Assets/Content/GameScene/StickerPacks/Textures/packs";

    // public

    // protected

    // private

    // properties

    #region Unity Methods
    void OnPreprocessTexture()
    {
        TextureImporter importer = assetImporter as TextureImporter;

        // Stickers.
        if (importer.assetPath.StartsWith(StickerAssetPath))
        {
            ApplyStickerImportSettings(importer);
        }
        // Packs.
        else if (importer.assetPath.StartsWith(PacksAssetPath))
        {
            ApplyPacksImportSettings(importer);
        }
    }
    #endregion

    #region Public Methods
    [UnityEditor.MenuItem("Game/Import Settings/Apply Stickers Import Settings")]
    public static void ApplyStickerImportSettings()
    {
        var assetPaths = AssetDatabase.GetAllAssetPaths()
            .Where(path => path.StartsWith(StickerAssetPath) &&
                !Directory.Exists(path))  // Exclude directories
            .ToArray();

        foreach (var assetPath in assetPaths)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            ApplyStickerImportSettings(importer, true);
        }
    }

    [UnityEditor.MenuItem("Game/Import Settings/Apply Packs Import Settings")]
    public static void ApplyPacksImportSettings()
    {
        var assetPaths = AssetDatabase.GetAllAssetPaths()
            .Where(path => path.StartsWith(PacksAssetPath) &&
                !Directory.Exists(path))  // Exclude directories
            .ToArray();

        foreach (var assetPath in assetPaths)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            ApplyPacksImportSettings(importer, true);
        }
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private static void ApplyStickerImportSettings(TextureImporter importer, bool forceImport = false)
    {
        if (importer == null)
        {
            return;
        }

        Object asset = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
        if (importer.importSettingsMissing || forceImport)
        {
            bool isDirty = false;

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                isDirty = true;
            }
            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                isDirty = true;
            }
            if (importer.spritePixelsPerUnit != 2)
            {
                importer.spritePixelsPerUnit = 2;
                isDirty = true;
            }
            if (!importer.isReadable)
            {
                importer.isReadable = true;
                isDirty = true;
            }
            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                isDirty = true;
            }

            if (importer.importSettingsMissing || isDirty)
            {
                Debug.Log($"Apply sticker import settings: {importer.assetPath}");
            }

            if (asset != null && isDirty)
            {
                importer.SaveAndReimport();
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            }
        }
    }

    private static void ApplyPacksImportSettings(TextureImporter importer, bool forceImport = false)
    {
        if (importer == null)
        {
            return;
        }

        Object asset = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
        if (importer.importSettingsMissing || forceImport)
        {
            bool isDirty = false;

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                isDirty = true;
            }
            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                isDirty = true;
            }
            if (importer.spritePixelsPerUnit != 2)
            {
                importer.spritePixelsPerUnit = 2;
                isDirty = true;
            }
            if (importer.isReadable)
            {
                importer.isReadable = false;
                isDirty = true;
            }
            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                isDirty = true;
            }

            if (importer.importSettingsMissing || isDirty)
            {
                Debug.Log($"Apply pack import settings: {importer.assetPath}");
            }

            if (asset != null && isDirty)
            {
                importer.SaveAndReimport();
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            }
        }
    }
    #endregion
}
