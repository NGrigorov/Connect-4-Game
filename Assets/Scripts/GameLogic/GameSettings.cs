using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    public enum Difficulty 
    { 
        easy = 7,
        medium = 50,
        hard = 1000
    }
    public Difficulty difficulty = Difficulty.medium;

    public bool playVSAI = false;
    public bool aiFirst = true;

    public void LoadAIGame()
    {
        playVSAI = true;
    }

    public void LoadHumanGame()
    {
        playVSAI = false;
    }

    public void PlayFirst(bool value)
    {
        aiFirst = value;
    }

    public void SetDifficulty(float value)
    {
        switch (value)
        {
            case 0:
                difficulty = Difficulty.easy;
                break;
            case 1:
                difficulty = Difficulty.medium;
                break;
            case 2:
                difficulty = Difficulty.hard;
                break;
            default:
                difficulty = Difficulty.medium;
                break;
        }
    }

}
