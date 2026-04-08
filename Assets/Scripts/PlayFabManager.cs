using System;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Prefs;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using TMPro;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager _Instance;

    public ProtectedString playerId;
    public ProtectedInt32 hammers;
    public Transform leaderboardParent;
    public GameObject leaderboardPrefab;
    public ProtectedString displayName;
    public TextMeshProUGUI lardPriceText;
    public GameObject loadingObject;
    public ProtectedString code;
    public AudioSource nomnomnomSound;

    private void Awake()
    {
        _Instance = this;
    }

    void Start()
    {
        Login();
    }

    public void Login()
    {
        if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
        {
            GameManager.Instance.loadingObj.gameObject.SetActive(true);
        }

        ProjectConfigScriptableObject projectConfig = ProjectConfigScriptableObject.LoadConfig();
        string titleId = projectConfig != null && ProjectConfigScriptableObject.HasConfiguredValue(projectConfig.PlayFabTitleId)
            ? projectConfig.PlayFabTitleId
            : PlayFabSettings.staticSettings.TitleId;

        if (string.IsNullOrWhiteSpace(titleId))
        {
            Debug.LogError("PlayFab TitleId is not configured.");

            if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
            {
                GameManager.Instance.loadingObj.gameObject.SetActive(false);
            }

            return;
        }

        PlayFabSettings.staticSettings.TitleId = titleId;

        var request = new LoginWithCustomIDRequest
        {
            CustomId = ProtectedPlayerPrefs.GetString("Account"),
            TitleId = titleId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Błąd logowania: " + error.ErrorMessage);

        if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
        {
            GameManager.Instance.loadingObj.gameObject.SetActive(false);
        }
    }

    private void OnLoginSuccess(LoginResult result)
    {
        playerId = result.PlayFabId;

        if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
        {
            GameManager.Instance.loadingObj.gameObject.SetActive(false);
        }

        GetPlayerCurrency();
        GetLeaderboard();
        ExecuteCloudScriptGetGameData();
        GetTitleData();

        string playFabDisplayName = result.InfoResultPayload != null &&
                                    result.InfoResultPayload.PlayerProfile != null
            ? result.InfoResultPayload.PlayerProfile.DisplayName
            : null;

        if (!string.IsNullOrWhiteSpace(playFabDisplayName) && playFabDisplayName != "Null")
        {
            displayName = playFabDisplayName;
        }

        if (string.IsNullOrWhiteSpace(displayName.ToString()))
        {
            UpdateDisplayName();
        }
    }

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Wystąpił błąd w usłudze PlayFab: " + error.ErrorMessage);
    }

    public void GetPlayerCurrency()
    {
        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, OnGetUserInventorySuccess, OnPlayFabError);
    }

    private void OnGetUserInventorySuccess(GetUserInventoryResult result)
    {
        if (result.VirtualCurrency.TryGetValue("HA", out int currencyAmount))
        {
            hammers = currencyAmount;
        }
    }

    public void GetPlayerReadOnlyData()
    {
        PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest(), OnGetPlayerDataSuccess, OnGetPlayerDataFailure);
    }

    void OnGetPlayerDataSuccess(GetUserDataResult result)
    {
        if (result.Data == null)
        {
            return;
        }

        if (result.Data.TryGetValue("Lifes", out UserDataRecord lifesData))
        {
            if (lifesData.Value == "1")
            {
                GameManager.Instance.lifes = 1;
            }
            else if (lifesData.Value == "2")
            {
                GameManager.Instance.lifes = 2;
            }
        }

        if (result.Data.TryGetValue("Multiplier", out UserDataRecord multiplierData))
        {
            if (multiplierData.Value == "1")
            {
                GameManager.Instance.multiplier = 1;
            }
            else if (multiplierData.Value == "2")
            {
                GameManager.Instance.multiplier = 2;
            }
        }
    }

    void OnGetPlayerDataFailure(PlayFabError error)
    {
        Debug.LogError("Failed to get player data from PlayFab: " + error.ErrorMessage);
    }

    public void SendLeaderboard()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionParameter = new { Code = code.ToString() },
                FunctionName = "SetPoints",
                GeneratePlayStreamEvent = true,
            },
            cloudResult =>
            {
                if (cloudResult.FunctionResult != null && cloudResult.FunctionResult.ToString() == "Success")
                {
                    if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
                    {
                        GameManager.Instance.loadingObj.gameObject.SetActive(false);
                    }

                    GameManager.Instance.score = 0;
                    GameManager.Instance.bonusScore = 0;
                    GetPlayerCurrency();
                }
            },
            cloudError =>
            {
                Debug.LogError("CloudScript SetPoints failed.");
            });
    }

    public void BuyLife()
    {
        if (hammers >= 500)
        {
            if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
            {
                GameManager.Instance.loadingObj.gameObject.SetActive(true);
            }

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "BuyLife",
                    GeneratePlayStreamEvent = true,
                },
                cloudResult =>
                {
                    if (cloudResult.FunctionResult != null && cloudResult.FunctionResult.ToString() == "Success")
                    {
                        GetPlayerCurrency();

                        if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
                        {
                            GameManager.Instance.loadingObj.gameObject.SetActive(false);
                        }

                        ExecuteCloudScriptGetGameData();
                    }

                    if (cloudResult.FunctionResult != null && cloudResult.FunctionResult.ToString() == "Error")
                    {
                        if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
                        {
                            GameManager.Instance.loadingObj.gameObject.SetActive(false);
                        }
                    }
                },
                cloudError =>
                {
                    Debug.LogError("CloudScript BuyLife failed.");
                });
        }
    }

    public void BuyMultiplier()
    {
        if (hammers >= 500)
        {
            if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
            {
                GameManager.Instance.loadingObj.gameObject.SetActive(true);
            }

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "BuyMultiplier",
                    GeneratePlayStreamEvent = true,
                },
                cloudResult =>
                {
                    if (cloudResult.FunctionResult != null && cloudResult.FunctionResult.ToString() == "Success")
                    {
                        GetPlayerCurrency();

                        if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
                        {
                            GameManager.Instance.loadingObj.gameObject.SetActive(false);
                        }

                        ExecuteCloudScriptGetGameData();
                    }

                    if (cloudResult.FunctionResult != null && cloudResult.FunctionResult.ToString() == "Error")
                    {
                        if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
                        {
                            GameManager.Instance.loadingObj.gameObject.SetActive(false);
                        }
                    }
                },
                cloudError =>
                {
                    Debug.LogError("CloudScript BuyMultiplier failed.");
                });
        }
    }

    public void ExecuteCloudScriptGetGameData()
    {
        if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
        {
            GameManager.Instance.loadingObj.gameObject.SetActive(true);
        }

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "GetGameData",
                GeneratePlayStreamEvent = false,
            },
            cloudResult =>
            {
                JsonObject jsonResult = (JsonObject)cloudResult.FunctionResult;
                object valueMultiplier;
                object valueLifes;

                ProtectedInt16 valueMultiplierSafe = 0;
                ProtectedInt16 valueLifesSafe = 0;

                if (jsonResult.TryGetValue("Multiplier", out valueMultiplier))
                {
                    valueMultiplierSafe = Convert.ToInt16(valueMultiplier);
                }

                if (jsonResult.TryGetValue("Lifes", out valueLifes))
                {
                    valueLifesSafe = Convert.ToInt16(valueLifes);
                }

                if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
                {
                    GameManager.Instance.loadingObj.gameObject.SetActive(false);
                }

                GameManager.Instance.lifes = valueLifesSafe;
                GameManager.Instance.multiplier = valueMultiplierSafe;
            },
            cloudError =>
            {
                Debug.LogError("CloudScript GetGameData failed.");
            });
    }

    public void ExecuteCloudScriptSendPointsCode()
    {
        if (GameManager.Instance != null && GameManager.Instance.loadingObj != null)
        {
            GameManager.Instance.loadingObj.gameObject.SetActive(true);
        }

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionParameter = new { Value = GameManager.Instance.score.ToString(), Bonus = GameManager.Instance.bonusScore.ToString() },
                FunctionName = "SetPointsCode",
                GeneratePlayStreamEvent = true,
            },
            cloudResult =>
            {
                JsonObject jsonResult = (JsonObject)cloudResult.FunctionResult;
                object valueObject2;
                object valuePoints;
                object valueBonus;

                ProtectedString codeValue = "";
                ProtectedInt16 valuePointsSafe = 0;
                ProtectedInt16 valueBonusSafe = 0;

                if (jsonResult.TryGetValue("Code", out valueObject2))
                {
                    codeValue = Convert.ToString(valueObject2);
                }

                if (jsonResult.TryGetValue("Points", out valuePoints))
                {
                    valuePointsSafe = Convert.ToInt16(valuePoints);
                }

                if (jsonResult.TryGetValue("Bonus", out valueBonus))
                {
                    valueBonusSafe = Convert.ToInt16(valueBonus);
                }

                if (valuePointsSafe != GameManager.Instance.score || valueBonusSafe != GameManager.Instance.bonusScore)
                {
                    BanPlayer();
                }

                if (valuePointsSafe == GameManager.Instance.score && valueBonusSafe == GameManager.Instance.bonusScore)
                {
                    code = Math.Sqrt((double.Parse(codeValue.ToString()) / 3) + 1337).ToString();
                    SendLeaderboard();
                }
            },
            cloudError =>
            {
                Debug.LogError("CloudScript SetPointsCode failed.");
            });
    }

    public void BanPlayer()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "banPlayerFromGame",
                GeneratePlayStreamEvent = true,
            },
            cloudResult => { },
            cloudError =>
            {
                Debug.LogError("CloudScript banPlayerFromGame failed.");
            });
    }

    public void UpdateDisplayName()
    {
        string telegramName = ProtectedPlayerPrefs.GetString("Telename");

        if (string.IsNullOrWhiteSpace(telegramName))
        {
            return;
        }

        var request2 = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = telegramName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request2, OnUpdateDisplayNameSuccess, OnUpdateDisplayNameFailure);
    }

    private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        ProtectedPlayerPrefs.SetString("SavedWallets_" + ProtectedPlayerPrefs.GetString("Account"), "true");
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Points",
            StartPosition = 0,
            MaxResultsCount = 9
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error getting leaderboard data: " + error.ErrorMessage);
    }

    private void GetTitleData()
    {
        var request = new GetTitleDataRequest();
        PlayFabClientAPI.GetTitleData(request, OnGetTitleDataSuccess, OnGetTitleDataFailure);
    }

    private void OnGetTitleDataSuccess(GetTitleDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("LARD"))
        {
            if (loadingObject != null)
            {
                loadingObject.gameObject.SetActive(false);
            }

            string lardValue = result.Data["LARD"];
            lardPriceText.text = "$LARD Price: " + lardValue;
        }
    }

    private void OnGetTitleDataFailure(PlayFabError error)
    {
        Debug.LogError("Error fetching TitleData: " + error.GenerateErrorReport());
    }

    private void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (Transform item in leaderboardParent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(leaderboardPrefab, leaderboardParent);

            TextMeshProUGUI[] texts = newGo.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();

            if (playerId == item.PlayFabId)
            {
                texts[0].color = Color.yellow;
                texts[1].color = Color.yellow;
                texts[2].color = Color.yellow;
            }
        }
    }

    private void OnUpdateDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update display name: " + error.ErrorMessage);
    }
}
