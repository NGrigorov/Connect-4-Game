using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreenHideShow : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool _showHide = true;

    public void HideShow()
    {
        animator.SetBool("hidePannel", _showHide);
        _showHide = !_showHide;
    }
}
