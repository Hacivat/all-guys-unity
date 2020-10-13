using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    void Start (){
        PlayerPrefs.SetInt("Volume", 1);
    }
    public void ToggleValue (Toggle change){
        PlayerPrefs.SetInt("Volume", change.isOn ? 1 : 0);
    }
}
