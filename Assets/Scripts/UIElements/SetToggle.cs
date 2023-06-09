using UnityEngine;
using UnityEngine.UI;

public class SetToggle : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Toggle>().isOn = GameSettings.Instance.aiFirst;
    }
}
