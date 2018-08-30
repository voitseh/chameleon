using UnityEngine;
using System.Collections;

public class VerticalMoving : MonoBehaviour
{

	// Обьект, который нужно переместить
	public GameObject movingObject;

	// Стартовая позиция
	[HideInInspector]
	public Vector3 startPos;
	// Конечная позицыя
	private Vector3 endPos;
	// Расстояние
	public float distance = 30f;
	// Время потраченое на движение от начала до конца
	public float lerpTime = 3f;
	// Время будет добавлено, пока она не настпнет lerpTime и произойдут определенные условия
	private float currentLerpTime = 0;
	// Запретить пользователю спам перемещение объекта и вызывает ошибку
	private bool keyHit = false;

	private void Update ()
	{
		// Процедура перемещения объекта
		if (keyHit == false) { 
			// Стартовая позицыя будет оновлена
			startPos = movingObject.transform.position;
			// Конечная позицыя будет оновлена
			endPos = movingObject.transform.position + Vector3.up * distance;
			keyHit = true;
		}
		if (keyHit == true) {
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime >= lerpTime) {
				currentLerpTime = lerpTime;
				keyHit = false;
			}
			float Perc = currentLerpTime / lerpTime;
			// Объект будет двигаться от начала до конца
			movingObject.transform.position = Vector3.Lerp (startPos, endPos, Perc);
		}
		if (keyHit == false) {
			// Установите currentLerpTime равным 0, так что процесс может быть перезапущен снова
			currentLerpTime = 0;
		}
	}
}
