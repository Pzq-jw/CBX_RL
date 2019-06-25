using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboControl : MonoBehaviour
{
    
	public Slider comboSlider;
    public Timer comboTimer;
    public Text comboNumText;
    public Text comboScoreText;
    
    public int comboNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        comboTimer = Timer.createTimer("ComboTimer");       
    }

    public void Combo()
    {
        AddComboNum();
        comboSlider.gameObject.SetActive(true);
        if(comboTimer.isTiming)
        {
            comboTimer.RestartTimerForCombo();
        }
        else
            comboTimer.startTiming(5, OnComboTimingComplete, OnComboTimingProcess, true, false, false);
    }

    public void AddComboNum()
    {
        comboNum++;
        comboNumText.text = "X " + comboNum.ToString();
    }

    void OnComboTimingComplete()
    {
        comboSlider.gameObject.SetActive(false);
        GameControl.instance.gameStatus = GameControl.GameStatus.GAME_RUNNING;
        comboScored();
        GameControl.instance.particleObj.StopComboPeriodAnim();
        comboNum = 0;
    }

    void OnComboTimingProcess(float p)
    {
        comboSlider.value = 1 - p;
    }

    public void comboScored()
    {
        comboScoreText.gameObject.SetActive(true);
        int comboScore = comboNum * 3;
        comboScoreText.text = "+ " + comboScore.ToString(); 
        GameControl.instance.populationScore += comboScore;
        GameControl.instance.scoreText.text = "Score:" + GameControl.instance.populationScore.ToString();
        Invoke("delayInactiveComboScoreText", 1);
    }

    void delayInactiveComboScoreText()
    {
        comboScoreText.gameObject.SetActive(false);
    }
}
