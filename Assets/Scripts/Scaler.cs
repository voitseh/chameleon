
// Данный скрипт ответственный за эффекты, связаные с появлением и исчезновением надписей, сообщающих об выиграше или проиграше

using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour
{

	[SerializeField]
	private	GameObject _buttonNEW;

	public float maxSize;
	public float growFactor;
	public float waitTime;

	// Флажок,регулирующий умненьшение размеров
	public static  bool flag = false;
	// Если на екране надписи-запрещается стрелять
	public static  bool labelVisibility = true;

	public  void PlayerWin ()
	{
		labelVisibility = false;
		// Прячем мух
		EnemyController.EnemiesVisibility ();
		gameObject.SetActive (true);
		StartCoroutine (Scale ()); 
		transform.localScale = new Vector3 (0, 0, 0);
	}

	public void GameOver ()
	{
		labelVisibility = false;
		EnemyController.EnemiesVisibility ();
		gameObject.SetActive (true);
		StartCoroutine (Scale ());
		transform.localScale = new Vector3 (0, 0, 0);
	}

	IEnumerator Scale ()
	{
		float timer = 0;

		while (true) {
			// Мы масштабируем все оси, они будут иметь одинаковые значения, 
			// потому мы можем работать с float вместо сравнения поплавков
			while (maxSize > transform.localScale.x) {
				timer += Time.deltaTime;
				transform.localScale += new Vector3 (1, 1, 1) * Time.deltaTime * growFactor;
				yield return null;
			}
			// Скинуть таймер
			yield return new WaitForSeconds (waitTime);
			//*********************************************************************************
			if (flag == false) {
				Time.timeScale = 0;
				EnemyController.EnemiesVisibility ();
			}
			_buttonNEW.SetActive (true);
			//*********************************************************************************
			if (flag == true) {
				_buttonNEW.SetActive (false);
				
				timer = 0;  
				while (1 < transform.localScale.x) {
					timer += Time.deltaTime;
					transform.localScale -= new Vector3 (1, 1, 1) * Time.deltaTime * growFactor;
					yield return null;
				}
				//**********************************************************************************
				timer = 0;

				gameObject.SetActive (false);
				labelVisibility = true;
				flag = false;
				//**********************************************************************************
				yield return new WaitForSeconds (waitTime);
			}
		}
	}
}


