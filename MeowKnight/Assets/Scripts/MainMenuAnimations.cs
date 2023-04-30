using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAnimations : MonoBehaviour
{
    public Animator mainMenuAnimator;

    public void StartPressed()
    {
        mainMenuAnimator.SetBool("startPressed", true);
    } 
}
