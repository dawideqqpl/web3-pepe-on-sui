using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OPS;
using OPS.AntiCheat;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Prefs;
using TMPro;
using UnityEngine.UI;

public class GameManager2 : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager2 _Instance;
    public TextMeshProUGUI[] holdsNeededTxt;
    public ProtectedInt16 holderLevel;
    public TextMeshProUGUI holderSummaryText;
    public ProtectedDouble yourTokens;
    public ProtectedInt16 currentChance;
    public TextMeshProUGUI hammersText;

    private void Awake()
    {
        _Instance = this;

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        hammersText.text = PlayFabManager._Instance.hammers.ToString();
        if (holderLevel == 1)
        {
            GetPrice._Instance.holderObjects[0].GetComponent<Image>().color = Color.green;
            currentChance = 10;

            holderSummaryText.text = "Holder Bonus: 0%";
        }
        if (holderLevel == 2)
        {
            currentChance = 15;
            GetPrice._Instance.holderObjects[1].GetComponent<Image>().color = Color.green;


            holderSummaryText.text = "Holder Bonus: 5%";
        }
        if (holderLevel == 3)
        {
            GetPrice._Instance.holderObjects[2].GetComponent<Image>().color = Color.green;


            currentChance = 25;
            holderSummaryText.text = "Holder Bonus: 15%";
        }
    }
}
