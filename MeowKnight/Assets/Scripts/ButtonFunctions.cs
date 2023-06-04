using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ButtonFunctions : MonoBehaviour
{
	public void BackToMenu()
	{
		SceneManager.LoadScene("MainMenu");
		Time.timeScale = 1f;
	}

	public void PlayAgain()
	{
		SceneManager.LoadScene("Game");
		Time.timeScale = 1f;
	}
}
