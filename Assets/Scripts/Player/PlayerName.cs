using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerName : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;

    public void SetName(string name)
    {
        nameText.text = name;
    }
}
