using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    public TextMeshProUGUI peopleRaechedText = null;
    public TextMeshProUGUI paintTheWallText = null;
    public TextMeshProUGUI paintedTheWallText = null;
    public TextMeshProUGUI eliminatedText = null;
    public Button restartButton = null;
    public Slider paintWallPercentageSlider = null;
    public Slider volumeSlider = null;
    public static int finishedCount = 0;
    private int playerCount = 0;
    private int maxQualifiedPlayer = 0;

    private void Start() 
    {
        if (volumeSlider == null)
            return;

        AudioListener.volume = PlayerPrefs.GetInt("Volume");
        volumeSlider.value = AudioListener.volume;
        playerCount = Spawner.playerCount;
        maxQualifiedPlayer = (int)Mathf.Round((float)playerCount / 2);
    }
    public void Update () 
    {
        if (eliminatedText == null || peopleRaechedText == null)
            return;

        if (finishedCount == maxQualifiedPlayer) {
            eliminatedText.enabled = true;
            restartButton.gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        peopleRaechedText.text = finishedCount + " / " + maxQualifiedPlayer;
    }
    public void PlayGame () 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void RestartGame () 
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
        finishedCount = 0;
    }
    public void PauseGame () 
    {
        Time.timeScale = 0;
    }
    public void ResumeGame () 
    {
        Time.timeScale = 1;
    }
    public void QuitGame () 
    {
        Application.Quit();
    }
    public void ChangeValue (){
        AudioListener.volume = volumeSlider.value;
    }

}
