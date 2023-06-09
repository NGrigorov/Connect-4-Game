using UnityEngine;
/// <summary>
/// Static Class <c>GameSettings</c> holds the game settings for when the opponent is AI. It persists through scenes.
/// </summary>
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

    /// <summary>
    /// Enum <c>Difficulty</c> used to set the difficulty of the AI
    /// </summary>
    public enum Difficulty 
    { 
        easy = 7,
        medium = 50,
        hard = 1000
    }
    public Difficulty difficulty = Difficulty.medium;

    public bool playVSAI = false;
    public bool aiFirst = true;

    /// <summary>
    /// Method <c>LoadAIGame</c> prepares the game to played vs AI opponent
    /// </summary>
    public void LoadAIGame()
    {
        playVSAI = true;
    }

    /// <summary>
    /// Method <c>LoadHumanGame</c> prepares the game to played vs Human opponent
    /// </summary>
    public void LoadHumanGame()
    {
        playVSAI = false;
    }

    /// <summary>
    /// Method <c>PlayFirst</c> makes the AI play first. More difficult AI
    /// </summary>
    public void PlayFirst(bool value)
    {
        aiFirst = value;
    }

    /// <summary>
    /// Method <c>SetDifficulty</c> sets the AI difficulty from a given value between 0 and 2
    /// </summary>
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
