using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI chargeText;
    public void UpdateChargeText(float chargePercent)
    {
        chargeText.text = chargePercent.ToString();
    }
}
