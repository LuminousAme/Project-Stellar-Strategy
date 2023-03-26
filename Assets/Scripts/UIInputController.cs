using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputController : MonoBehaviour
{
	public PlayerInput input;

	public void DisableInput() {
		StartCoroutine(WaitForFrame());
	}

	IEnumerator WaitForFrame() {
		input.enabled = false;
		yield return new WaitForEndOfFrame();
		input.enabled = true;
	}
}
