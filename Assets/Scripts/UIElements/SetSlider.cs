using UnityEngine;
using UnityEngine.UI;
using static GameSettings;

public class SetSlider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Difficulty difficulty = GameSettings.Instance.difficulty;
        switch (difficulty)
        {
            case Difficulty.easy:
                GetComponent<Slider>().value = 0;
                break;
            case Difficulty.medium:
                GetComponent<Slider>().value = 1;
                break;
            case Difficulty.hard:
                GetComponent<Slider>().value = 2;
                break;
            default:
                GetComponent<Slider>().value = 1;
                break;
        }
        
    }
}
