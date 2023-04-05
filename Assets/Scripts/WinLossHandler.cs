using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLossHandler : MonoBehaviour
{
	public UnitSelection selectionManager;
	public GameObject HUD;
	public GameObject winStuff;
	public GameObject loseStuff;
    void Start()
    {
        MatchManager.instance.playerWon += Win;
        MatchManager.instance.playerLost += Lose;

		winStuff.SetActive(false);
		loseStuff.SetActive(false);
		gameObject.SetActive(false);
    }

	private void OnDestroy() {
		if (gameObject.activeInHierarchy || MatchManager.instance == null)	return;

		MatchManager.instance.playerWon -= Win;
		MatchManager.instance.playerLost -= Lose;
	}

	void DisableStuff() {
		MatchManager.instance.playerWon -= Win;
		MatchManager.instance.playerLost -= Lose;

		selectionManager.enabled = false;
		HUD.SetActive(false);
		gameObject.SetActive(true);

		StartCoroutine(SlowTime(1f));
	}

	IEnumerator SlowTime(float duration) {
		float counter = duration;
		while (counter > 0) {
			Time.timeScale = counter / duration;

			counter += Time.unscaledDeltaTime;
			yield return null;
		}
		Time.timeScale = 0f;
	}

	void Win() {
		DisableStuff();
		winStuff.SetActive(true);
	}

	void Lose() {
		DisableStuff();
		loseStuff.SetActive(true);
	}
}
