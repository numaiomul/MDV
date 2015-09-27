using UnityEngine;
using System.Collections;

public class fadeIn2 : MonoBehaviour {

	public void FadeMeIn () {
		StartCoroutine (DoFade());
	}
	
	IEnumerator DoFade () {
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		while (canvasGroup.alpha<1) {
			canvasGroup.alpha += Time.deltaTime / 1f;
			yield return null;
		}
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		yield return null;
	}
}