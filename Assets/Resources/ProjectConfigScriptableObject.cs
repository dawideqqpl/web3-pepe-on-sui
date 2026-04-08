using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectConfigData", menuName = "ScriptableObjects/ProjectConfigScriptableObject",
    order = 1)]
public class ProjectConfigScriptableObject : ScriptableObject
{
    private const string DefaultConfigResourcePath = "ProjectConfigData";
    private const string LocalOverrideResourcePath = "ProjectConfigDataLocal";

    [SerializeField] private string projectID;
    [SerializeField] private string chainID;
    [SerializeField] private string chain;
    [SerializeField] private string network;
    [SerializeField] private string symbol;
    [SerializeField] private string rpc;
    [SerializeField] private string playFabTitleId;
    [SerializeField] private string dexScreenerPairUrl;

    public string ProjectId
    {
        get => projectID;
        set => projectID = value;
    }

    public string ChainId
    {
        get => chainID;
        set => chainID = value;
    }

    public string Chain
    {
        get => chain;
        set => chain = value;
    }

    public string Network
    {
        get => network;
        set => network = value;
    }

    public string Symbol
    {
        get => symbol;
        set => symbol = value;
    }

    public string Rpc
    {
        get => rpc;
        set => rpc = value;
    }

    public string PlayFabTitleId
    {
        get => playFabTitleId;
        set => playFabTitleId = value;
    }

    public string DexScreenerPairUrl
    {
        get => dexScreenerPairUrl;
        set => dexScreenerPairUrl = value;
    }

    public static ProjectConfigScriptableObject LoadConfig()
    {
        ProjectConfigScriptableObject localOverride = Resources.Load<ProjectConfigScriptableObject>(LocalOverrideResourcePath);
        return localOverride != null
            ? localOverride
            : Resources.Load<ProjectConfigScriptableObject>(DefaultConfigResourcePath);
    }

    public static bool HasConfiguredValue(string value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               !value.Trim().StartsWith("REPLACE_ME_", StringComparison.OrdinalIgnoreCase);
    }
}
