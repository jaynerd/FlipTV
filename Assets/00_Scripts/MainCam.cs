using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
	float shakeDuration = Settings.cameraShakeDuration;
	float shakeMagnitude = 0.3f;

	public IEnumerator Shake ()
	{
		Vector3 lastPosition = transform.position;
		float duration = shakeDuration;
		while (true) {
			yield return new WaitForSeconds (0.025f);
			transform.localPosition = lastPosition + Random.insideUnitSphere * shakeMagnitude;
			duration -= Time.deltaTime;
			if (duration < 0) {
				transform.localPosition = lastPosition;
				StopCoroutine ("Shake");
				break;
			}
		}
	}
}