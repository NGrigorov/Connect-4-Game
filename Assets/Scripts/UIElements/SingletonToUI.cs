using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>SingletonToUI</c> finds the Singleton to be used by UI elements
/// </summary>
public class SingletonToUI : MonoBehaviour
{
    public void LoadAIGame()
    {
        GameSettings.Instance.playVSAI = true;
    }

    public void LoadHumanGame()
    {
        GameSettings.Instance.playVSAI = false;
    }

    public void PlayFirst(Toggle value)
    {
        GameSettings.Instance.PlayFirst(value.isOn);
    }

    public void SetDifficulty(Slider value)
    {
        GameSettings.Instance.SetDifficulty(value.value);
    }
}
