using UnityEngine;
using System.Collections;

public class Blink : MonoBehaviour
{
	private	void Start ()
	{
		StartCoroutine (DoBlinks (7f, 0.2f));
	}

	public	IEnumerator DoBlinks (float duration, float blinkTime)
	{
		while (duration > 0f) {
			duration -= Time.deltaTime;
			// Тумблер визуализатор
			transform.GetComponent<Renderer> ().enabled = !transform.GetComponent<Renderer> ().enabled;
			// Подождать немного
			yield return new WaitForSeconds (blinkTime);
		}
		transform.GetComponent<Renderer> ().enabled = true;
	}
}
