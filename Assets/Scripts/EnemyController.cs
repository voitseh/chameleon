using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    // Singelton
	public static EnemyController Instance { get; private set; }
	// Сериализованая переменная для связи с обьектом-шаблоном
	[SerializeField] 
	private GameObject[] enemyPrefabKind = new GameObject[10];
	//Лист созданых шаблонов врагов
	public static List<GameObject> enemyPrefabs = new List<GameObject> ();
	private GameObject _enemy;
	// Шаблоны инвентаря
	[SerializeField] 
	private GameObject[] inventoryPrefabKind = new GameObject[2];
	//Лист созданых шаблонов инвентаря
	public static List<GameObject> invPrefabs = new List<GameObject> ();
	private GameObject _inventory;
	// Флажок который позволяет или не позволяет создавать врагов
	private bool flag = true;

	public void Awake ()
	{   
		Instance = this;
	}

	private void Update ()
	{ 
		if (flag)
			StartCoroutine (CreateEnemy ());
	}

	private IEnumerator  CreateEnemy ()
	{ 
		// Порождаем нового врага.
		_enemy = Instantiate (enemyPrefabKind [Random.Range (0, 9)]) as GameObject; 
		_enemy.transform.position = new Vector3 (7.5f, Random.Range (-2.3f, 5.5f), -4);
		_enemy.SetActive (true);
		enemyPrefabs.Add (_enemy);
		flag = !flag;
		yield return new WaitForSeconds (Random.Range (0, 7));
		// Уничтожаем врага который улетел за пределы экрана
		for (int i = 0; i < enemyPrefabs.Count; i++) {
			if (enemyPrefabs [i].transform.position.x < -4.1 || enemyPrefabs [i].transform.position.y < -3.9) {
				Destroy (enemyPrefabs [i]);
				enemyPrefabs.RemoveAt (i); 
			}
		}
		flag = true;
	}

	// Создание инвентаря у противников(вызывается в скрипте ReactiveTarget) при уничтожении врага
	public void  CreateInventory (Transform pos)
	{
		int index = Random.Range (0, 15);

		if (index == 0 || index == 1)
			_inventory = Instantiate (inventoryPrefabKind [index]) as GameObject; 
		// Инвентарь появляется рядом с подбитым врагом
		if ((pos.position.x - 0.5f) != null && pos.position.y != null)
			_inventory.transform.position = new Vector3 (pos.position.x, pos.position.y - 0.5f, -2);
		_inventory.transform.GetComponent<Rigidbody2D> ().velocity = new Vector2 (-1, 2);
		_inventory.transform.GetComponent<Rigidbody2D> ().isKinematic = false;
		_inventory.SetActive (true);   
		invPrefabs.Add (_inventory);
		InvDestroy ();
	}

	// Этот метод прячет врагов и инвентарь со сцены когда появляются надписи "Player win" или "Game over"
	public static void EnemiesVisibility ()
	{
		for (int i = 0; i < enemyPrefabs.Count; i++) {
			Destroy (enemyPrefabs [i]);
			enemyPrefabs.RemoveAt (i); 
		}
		InvDestroy ();
	}
	// Уничтожаем инвентар который улетел за пределы экрана
	private static void InvDestroy(){
		for (int i = 0; i < invPrefabs.Count; i++) {
			if (invPrefabs [i].transform.position.y < -3.9) {
				invPrefabs.RemoveAt (i); 
			}
		}
	}
}
