using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamController : MonoBehaviour
{
	public Vector2 sensitivity = new Vector2(1f, 0.5f);
	public float speed = 10f;
	public float trackingSpeed = 10f;
	public float trackingRangeModifier = 1.5f;
	public float maxDist = 1000f;
	public LayerMask mask = 0;


	Transform following = null;
	float offset = 0f;
	float minOffset = 0f;
	Quaternion offsetRot;
	bool doRot = false;

	bool isRotating = false;
	
	bool isMoving = false;

	Camera cam = null;

	private void Awake() {
		cam = GetComponent<Camera>();
	}

	//multitap interaction
	public void TryLockOn(InputAction.CallbackContext context) {
		if (!context.performed)	return;

		//screen position
		Ray dir = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
		RaycastHit hit;
		if (Physics.Raycast(dir, out hit, maxDist, mask)) {
			//check if planet (add conditions later)
			SphereCollider sphere = hit.transform.GetComponent<SphereCollider>();
			if (sphere) {
				//travel to planet
				minOffset = sphere.radius + 1f;
				StartCoroutine(LockOn(hit.transform, sphere.radius * trackingRangeModifier));
				return;
			}
		}

		//stop if falling through
		following = null;
	}

	public void StopTracking(InputAction.CallbackContext context) {
		if (!context.started)	return;

		following = null;
	}

	public void RotateCamera(InputAction.CallbackContext context) {
		if (context.performed) {
			StartCoroutine(Rotate(Mouse.current));
		}
		if (context.canceled) {
			isRotating = false;
		}
	}

	public void MoveCamera(InputAction.CallbackContext context) {
		if (context.started) {
			StartCoroutine(Move(context.action));
		}
		if (context.canceled) {
			isMoving = false;
		}
	}

	static WaitForEndOfFrame eof = new WaitForEndOfFrame();

	IEnumerator LockOn(Transform target, float newOffset) {
		//check if something has already been locked on, if so, break out, let the other take care of it
		doRot = true;
		offset = newOffset;

		//calc the new offsetRot
		if (target) {
			offsetRot = Quaternion.LookRotation(target.position - transform.position);
		}

		if (following || target == null) {
			following = target;
			yield break;
		}
		following = target;

		//now move towards target while active
		while (following) {
			//do rotation only while doRot is true and not masnually rotating (which also resets doRot)
			if (doRot && !isRotating) {
				transform.rotation = Quaternion.RotateTowards(transform.rotation, offsetRot, 360f * Time.deltaTime);
				if (transform.rotation == offsetRot) {
					doRot = false;
				}
			}

			Vector3 targetPos = following.position + offsetRot * (Vector3.back * offset);
			//if too far, do some lerping at 1% per frame
			if (Vector3.Distance(transform.position, targetPos) > trackingSpeed) {
				transform.position = Vector3.Lerp(transform.position, targetPos, 0.01f);
			}
			else {
				transform.position = Vector3.MoveTowards(transform.position, targetPos, trackingSpeed * Time.deltaTime);
			}

			yield return eof;
		}
	}

	IEnumerator Rotate(Mouse cursor) {
		//disable autoRot
		isRotating = true;
		doRot = false;

		Vector3 rot = transform.rotation.eulerAngles;

		Vector2 pos = cursor.position.ReadValue();
		Cursor.lockState = CursorLockMode.Locked;

		while (isRotating) {
			
			if (cursor.delta.ReadValue() != Vector2.zero) {
				Vector2 change = cursor.delta.ReadValue() * sensitivity;
				rot.x = Mathf.Clamp(rot.x - change.y, -85f, 85f);
				rot.y += change.x;

				transform.rotation = Quaternion.Euler(rot.x, rot.y, 0f);
			}

			yield return eof;
		}

		Cursor.lockState = CursorLockMode.None;
		cursor.WarpCursorPosition(pos);
	}

	IEnumerator Move(InputAction action) {
		isMoving = true;
		Vector3 euler = offsetRot.eulerAngles;
		bool lockedOn = following;

		while (isMoving) {
			Vector3 change = action.ReadValue<Vector3>() * speed * Time.deltaTime;
			//is locked on
			if (following) {
				if (!lockedOn) {
					euler = offsetRot.eulerAngles;
					lockedOn = true;
				}

				euler.x = Mathf.Clamp(euler.x + change.z * 2f, -90f, 90f);
				euler.y -= change.x * 2f;
				offset = Mathf.Clamp(offset + change.y * 0.5f, minOffset, minOffset * trackingRangeModifier * 2f);
				offsetRot = Quaternion.Euler(euler);

				doRot = true;
			}
			//not locked on, just move
			else {
				transform.position += transform.rotation * change;
			}

			yield return eof;
		}
	}
}
