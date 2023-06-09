using UnityEngine;
using UnityEngine.UI;

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
