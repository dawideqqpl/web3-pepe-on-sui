using System.Text.RegularExpressions;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Prefs;
using UnityEngine;

public class TelegramConnect : MonoBehaviour
{
    public ProtectedString platform;
    public ProtectedString dataString;
    public ProtectedBool isTesting;

    void Start()
    {
        StartClicked();
    }

    public void StartClicked()
    {
        PlayFabManager._Instance.loadingObject.gameObject.SetActive(true);

        if (isTesting == false)
        {
            if (IsMobilePlatform(platform))
            {
                ProtectedPlayerPrefs.SetString("Account", dataString);
                ProtectedPlayerPrefs.SetBool("Platform", true);
                ProtectedPlayerPrefs.SetString("PlatformType", platform);
            }
            else
            {
                ProtectedPlayerPrefs.SetString("Account", dataString);
                ProtectedPlayerPrefs.SetString("PlatformType", platform);
                ProtectedPlayerPrefs.SetBool("Platform", false);
            }
        }
        else
        {
            ProtectedPlayerPrefs.SetString("Account", "test");
            ProtectedPlayerPrefs.SetBool("Platform", false);
        }

        PlayFabManager._Instance.Login();
    }

    public void getTelegramFirstLast(string data1)
    {
        string cleanValue = TrimWrappedValue(data1);
        ProtectedPlayerPrefs.SetString("Telename", EmojiRemover.RemoveEmojis(cleanValue));
    }

    public void getTelegramData(string data)
    {
        dataString = data;
    }

    public void getTelegramPlatform(string data)
    {
        platform = TrimWrappedValue(data);
    }

    private static bool IsMobilePlatform(string platformValue)
    {
        return platformValue == "android" || platformValue == "ios" || platformValue == "android_x";
    }

    private static string TrimWrappedValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (value.Length >= 2)
        {
            return value.Substring(1, value.Length - 2);
        }

        return value;
    }
}

public class EmojiRemover
{
    public static string RemoveEmojis(string input)
    {
        var regex = new Regex(@"[\p{Cs}]");
        return regex.Replace(input, "");
    }
}
