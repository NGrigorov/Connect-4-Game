using UnityEngine;


/// <summary>
/// Class <c>GameOverScreenHideShow</c> used to manage the animator of the end game screen
/// </summary>
public class GameOverScreenHideShow : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool _showHide = true;

    /// <summary>
    /// Method <c>HideShow</c> changes the boolen of <c>animator</c> to show or hide the gameobject
    /// </summary>
    public void HideShow()
    {
        animator.SetBool("hidePannel", _showHide);
        _showHide = !_showHide;
    }
}
