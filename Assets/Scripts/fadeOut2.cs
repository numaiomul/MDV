using UnityEngine;
using System.Collections;

public class fadeOut2 : MonoBehaviour {


	public void FadeMe () {
		StartCoroutine (DoFade());
	}

	IEnumerator DoFade () {
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		while (canvasGroup.alpha>0) {
			canvasGroup.alpha -= Time.deltaTime / 1f;
			yield return null;
		}
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		yield return null;
	}
}