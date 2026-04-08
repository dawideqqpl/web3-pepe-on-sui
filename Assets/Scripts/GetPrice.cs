using System.Collections;
using System.Globalization;
using OPS.AntiCheat.Field;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GetPrice : MonoBehaviour
{
    private const string DefaultDexScreenerUrl = "https://api.dexscreener.com/latest/dex/pairs/base/0xec436cd521af0cb47d00e0ec2dcd4fcfe7587b08";

    public static GetPrice _Instance;
    public ProtectedString priceUsd;
    public ProtectedDouble holderLevel1;
    public ProtectedDouble holderLevel2;
    public GameObject[] holderObjects;

    [System.Serializable]
    public class PairData
    {
        public PairInfo pair;
    }

    [System.Serializable]
    public class PairInfo
    {
        public string priceUsd;
    }

    public TextMeshProUGUI currentPriceText;

    private void Awake()
    {
        _Instance = this;
    }

    void Start()
    {
        GameManager.Instance.loadingObj.gameObject.SetActive(true);
        StartCoroutine(GetPriceData());
    }

    IEnumerator GetPriceData()
    {
        ProjectConfigScriptableObject projectConfig = ProjectConfigScriptableObject.LoadConfig();
        string url = projectConfig != null && ProjectConfigScriptableObject.HasConfiguredValue(projectConfig.DexScreenerPairUrl)
            ? projectConfig.DexScreenerPairUrl
            : DefaultDexScreenerUrl;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error get data: " + webRequest.error);
                yield break;
            }

            PairData pairData = JsonUtility.FromJson<PairData>(webRequest.downloadHandler.text);
            if (pairData == null || pairData.pair == null || !float.TryParse(pairData.pair.priceUsd, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedPrice) || parsedPrice <= 0f)
            {
                Debug.LogError("Error get data: invalid price payload.");
                yield break;
            }

            priceUsd = pairData.pair.priceUsd;
            currentPriceText.text = "Current price: <b>" + priceUsd + "$";

            holderLevel1 = 100 / parsedPrice;
            holderLevel2 = 1000 / parsedPrice;

            if ((holderLevel1 / 1000) >= 1000)
            {
                GameManager2._Instance.holdsNeededTxt[0].text = "< " + (holderLevel1 / 1000000).ToString("F1") + "M";
            }
            else
            {
                GameManager2._Instance.holdsNeededTxt[0].text = "< " + (holderLevel1 / 1000).ToString("F0") + "K";
            }

            if ((holderLevel2 / 1000) >= 1000)
            {
                GameManager2._Instance.holdsNeededTxt[1].text = (holderLevel1 / 1000).ToString("F0") + "K - " + (holderLevel2 / 1000000).ToString("F1") + "M";
                GameManager2._Instance.holdsNeededTxt[2].text = ">= " + (holderLevel2 / 1000000).ToString("F1") + "M";
            }
            else
            {
                GameManager2._Instance.holdsNeededTxt[1].text = (holderLevel1 / 1000).ToString("F0") + "K - " + (holderLevel2 / 1000).ToString("F0") + "K";
                GameManager2._Instance.holdsNeededTxt[2].text = ">= " + (holderLevel2 / 1000).ToString("F0") + "K";
            }

            CustomCallExample._Instance.GetBalance();
        }
    }
}
