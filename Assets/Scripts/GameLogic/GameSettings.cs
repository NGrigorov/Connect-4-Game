using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            DontDestroyOnLoad(gameObject);
        }
    }

    public enum Difficulty 
    { 
        easy = 14,
        medium = 150,
        hard = 1000
    }
    public Difficulty difficulty = Difficulty.medium;

    public bool playVSAI = false;
    public bool playFirst = false;

    public void LoadAIGame()
    {
        playVSAI = true;
    }

    public void LoadHumanGame()
    {
        playVSAI = false;
    }

    public void PlayFirst()
    {
        playFirst = true;
    }

    public void PlaySecond()
    {
        playFirst = false;
    }

    public void SetDifficulty(Difficulty diff)
    {
        difficulty = diff;
    }

}
