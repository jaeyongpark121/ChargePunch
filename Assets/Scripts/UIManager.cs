using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private PlayerController playerConroller1;
    [SerializeField] private PlayerController playerConroller2;

    [SerializeField] private TextMeshProUGUI chargeText1;
    [SerializeField] private TextMeshProUGUI chargeText2;

    [SerializeField] private TextMeshProUGUI stateText1;
    [SerializeField] private TextMeshProUGUI stateText2;
    public void UpdateUIText<T>(T value, TextMeshProUGUI chargeText)
    {
        chargeText.text = value.ToString();
    }

    private void Update()
    {
        UpdateUIText(playerConroller1.ChargeAmount, chargeText1);
        UpdateUIText(playerConroller2.ChargeAmount, chargeText2);
        UpdateUIText(playerConroller1.CurrentState, stateText1);
        UpdateUIText(playerConroller2.CurrentState, stateText2);
    }
}
