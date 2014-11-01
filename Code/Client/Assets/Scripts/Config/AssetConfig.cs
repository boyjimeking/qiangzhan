using System;

/// <summary>
/// 资源相对路径配置
/// </summary>
public static class AssetConfig
{
    public static string AssetSuffix = ".ab";//for assetbundle

    public static string CharacterPath
    {
        get
        {
            return "Character/";
        }
    }
    //----------------------------------------------
    public static string ParticleConfig = "ParticleData.config";
    public static string ParticlePath
    {
        get
        {
            return "Particle/";
        }
    }
    public static string ParticleRes
    {
        get
        {
            return ParticlePath + "res/";
        }
    }
    public static string ParticlePrefab
    {
        get
        {
            return ParticlePath + "prefab/";
        }
    }

    public static string CustomBonesConfig = "CustomBone.xml";
    public static string ModelPath
    {
        get
        {
            return "Model/";
        }
    }

    public static string ModelPrefabPath
    {
        get
        {
            return "Model/Prefab/";
        }
    }
    public static string WeaponPath
    {
        get
        {
            return "Weapon/";
        }
    }
    public static string RolightPath
    {
        get
        {
            return "Scene/Dynamiclight.ab";
        }
    }
    public static string CustomMaterialPath
    {
        get
        {
            return "Model/materials.ab";
        }
    }
}

