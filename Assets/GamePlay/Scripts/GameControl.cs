﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GO_Extensions;

public class GameControl : MonoBehaviour
{

	public static GameControl instance;

	public PiecePool piecePoolObj;
    public ColumnSwinging columnObj;
    public ScreenMoveUp screenMoveUpObj;
    public EllipticalOrbit slingObj;
    public ComboControl comboControlObj;
    public DoTweenControl doTweenObj;
    public ParticleControl particleObj;
    public MyCollisionControl mycolObj;
    public CCMStackAgent agentObj;
    public CBXDDAAgent saverAgentObj;
    public CBXAcademy academyObj;
    public Transform monitorObj;

    public Text scoreText;
    public Text missText;
    public GameObject gameOverText;
    public GameObject columnGameObj;

    public int populationScore = 0;
    public int stackedPieceNum = 0;
    public int missNum = 0;
    public Vector3 seaLevel;

    public enum GameStatus
    {
        GAME_READY = 0,  // Scene has been loaded, but game not start
        GAME_START, // Game start, but first piece not fallen
        GAME_RUNNING, // Game is running but not combo, the first piece has fallen
        GAME_COMBO, // Game is running and in combo phase
        GAME_OVER // Game is over.
    }
    public GameStatus gameStatus;

    public bool isGameRunning
    {
        get {return (gameStatus == GameStatus.GAME_RUNNING 
            || gameStatus == GameStatus.GAME_COMBO);}
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(!instance)
        {
        	instance = this;
        }else if(instance)
        {
        	Destroy(gameObject);
        }
    }

    void Start()
    {
        seaLevel = new Vector3(0, -10, 0);
        gameStatus = GameStatus.GAME_START;
    }

    void Update()
    {
        if(gameStatus == GameStatus.GAME_OVER 
            && (Input.GetKeyDown("space") || Input.touchCount > 0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            gameStatus = GameStatus.GAME_START; 
        }
    }

    public void OnPieceStacking()
    {
        PieceStacked();
    }

    public void AfterPieceStackingSuccessfully(bool isDeadCenter)
    {
        // CheckFirstPieceIfStacked();
        // Scored();
        // ScreenMoveUp();
        // SetColumnSwinging();
        // seaLevel.y++;

        if(isDeadCenter)
        {
            gameStatus = GameStatus.GAME_COMBO;
            // agentObj.ComputeReward();
            // agentObj.m_SetRewrd(1f);
            // agentObj.Done();
            // agentObj.isJustCalledDone = true;
            // Debug.Log("culmalitive reward = " + agentObj.GetReward().ToString(), gameObject);
            // comboControlObj.Combo();
            // columnObj.Set2ComboSwingingAmplitude();
            // particleObj.PlayComboPeriodAnim();
        }
        else
        {
            // agentObj.ComputeReward();
            // agentObj.m_SetRewrd(-1f);
            // agentObj.Done();
            // agentObj.isJustCalledDone = true;
            // Debug.Log("culmalitive reward = " + agentObj.GetReward().ToString(), gameObject);
        }
        // else if(gameStatus == GameStatus.GAME_COMBO)
        // {
        //     comboControlObj.AddComboNum();            
        // }
    }

    public void AfterPieceStackingFailed(int fallenSide)
    {
        // agentObj.ComputeReward();
        // agentObj.m_SetRewrd(-1f);
        // agentObj.Done();
        // agentObj.isJustCalledDone = true;
        // Debug.Log("culmalitive reward = " + agentObj.GetReward().ToString(), gameObject);
        
        // Missed();
        // CheckMissNum();
        // screenMoveUpObj.ShakeCamera();
    }

    void PieceStacked()
    {
    	// piecePoolObj.HookNewPiece();
    }

    void Scored()
    {
        stackedPieceNum++;
        populationScore++;
        scoreText.text = "Score:" + populationScore.ToString();
    }

    void Missed()
    {
        missNum++;
        missText.text = "Miss:" + missNum.ToString();
    }

    void CheckMissNum()
    {
        if(missNum > 2)
        {
            gameOverText.SetActive(true);
            if(gameStatus == GameStatus.GAME_COMBO)
            {
                comboControlObj.comboTimer.EndTiming();
                comboControlObj.comboScored();
            }
            gameStatus = GameStatus.GAME_OVER;
        }
    }

    void ScreenMoveUp()
    {
        screenMoveUpObj.MoveUp();
    }

    void CheckFirstPieceIfStacked()
    {
        if(gameStatus == GameStatus.GAME_START)
        {
            gameStatus = GameStatus.GAME_RUNNING;
        }
    }

    void SetColumnSwinging()
    {
        columnObj.SwingingCenterMoveUp();
        columnObj.AddAmplitudeRotate();
        columnObj.SetAmplitudeIncrementAndMax();
    }
















}
