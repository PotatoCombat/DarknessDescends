﻿using UnityEngine;

namespace GDG
{
using static GameManager;
public class GameplayController : MonoBehaviour
{
    public GameLogicManager gameLogic;
    public DropManager dropManager;

    bool isGameRunning = false;
    float timeSinceGameStart = 0.0f;
    SaveData saveData;

    void OnEnable()
    {
        EventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    void OnDisable()
    {
        EventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.previousGameState == GameState.Pregame &&
            e.currentGameState == GameState.Running)
        {
            StartGame();
        }

        if (e.previousGameState == GameState.Paused &&
            e.currentGameState != GameState.Paused)
        {
            TogglePauseGame(false);
        }
        else if (e.previousGameState != GameState.Paused &&
            e.currentGameState == GameState.Paused)
        {
            TogglePauseGame(true);
        }
    }

    void StartGame()
    {
        saveData = SaveData.LoadSaveData();
        if(saveData != null)
        {
            UpdateStateBasedOnSaveData(saveData);
        }
        timeSinceGameStart = 0.0f;
        isGameRunning = true;
    }

    void TogglePauseGame(bool toggle)
    {
        UIManager.Instance.TogglePauseMenu(toggle);
    }

    void Update()
    {
        if (isGameRunning)
        {
            float timeElapsedFromPreviousFrame = Time.deltaTime;
            timeSinceGameStart += timeElapsedFromPreviousFrame;
            UIManager.Instance.UpdateTime(timeSinceGameStart);
        }
    }

    void OnEnemyDeath(IEnemy enemy)
    {
        gameLogic.OnEnemyDeath(enemy);
        dropManager.DropReward(enemy);
    }

    void OnHeroDeath(IHero hero)
    {
        GameOver();
    }

    void GameOver()
    {
        RecordSavedData();
        SoundManager.Instance.PlaySound(SoundManager.Instance.gameover);
        UIManager.Instance.DisplayEndGame();
        isGameRunning = false;
    }

    void RecordSavedData()
    {
        saveData = SaveData.RecordHighscoreIntoSave(gameLogic.highScore);
    }

    void UpdateStateBasedOnSaveData(SaveData incomingSaveData)
    {
        gameLogic.highScore = incomingSaveData.highScore;
    }
}
}
