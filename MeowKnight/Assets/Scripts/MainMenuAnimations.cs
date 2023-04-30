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

	public void CreditsPressed()
	{
        transform.Rotate(0, 180, 0);
        mainMenuAnimator.SetBool("creditsPressed", true);
	}

	public void QuitPressed()
    {
        mainMenuAnimator.SetBool("quitPressed", true);
	}
}
