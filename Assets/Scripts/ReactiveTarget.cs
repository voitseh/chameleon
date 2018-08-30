using UnityEngine;
using System.Collections;

public class ReactiveTarget : MonoBehaviour
{
	
	// Здоровье врага
	[HideInInspector]
	public int curHealth;
	// Скорость движения врага
	private float speed = -1.00f;
	// Логическая переменная для слежения за состоянием врага.
	[HideInInspector]
	public bool _alive;
	// Для траектории движения мух после их подбития
	private float HoriInitialSpeed = 1;
	private float VertInitialSpeed = 2;

	private void Start ()
	{
		curHealth = 2;
		_alive = true; 
	}

	private void FixedUpdate ()
	{
		if (_alive) {
			transform.Translate (speed * Time.deltaTime, 0, 0); 
		} else {
			transform.GetComponentInChildren<ParticleSystem> ().Simulate (2f);
			transform.GetComponent<Animator> ().enabled = false;
		}
	}
	// Открытый метод, позволяющий внешнему коду воздействовать на «живое» состояние.
	public void SetAlive (bool alive)
	{ 
		_alive = alive;
	}

	private void OnTriggerEnter2D (Collider2D other)
	{ 
		Fireball fire = other.GetComponent<Fireball> ();
		// Попадение пули во врага
		if (fire != null) {
			curHealth--; 
			// Дважды стреляем по цели, за вторым разом жизни врага пропадают, от уничтожается и шлется сообщение
			if (curHealth == 1) {
				transform.GetChild (1).transform.GetComponent<Transform> ().localScale -= new Vector3 (0.5f, 0.3f, 40);
				transform.GetChild (1).transform.GetComponent<Transform> ().localPosition = new Vector3 (-0.25f, 0.37f, 0);
			} else {
				if (transform.GetChild (1).gameObject.activeSelf == true)
					transform.GetChild (1).gameObject.SetActive (false);
				// При попадании огня, цель делает параболическое движение по определенной траектории
				transform.GetComponent<Rigidbody2D> ().velocity = new Vector2 (HoriInitialSpeed, VertInitialSpeed);
				transform.GetComponent<Rigidbody2D> ().isKinematic = false;
				SetAlive (false);                   
				// К реакции на попадания добавлена рассылка сообщения.
				Messenger.Broadcast (GameEvent.ENEMY_HIT);
				// Этот метод создает на месте некоторых убитых мух инвентарь
				EnemyController.Instance.CreateInventory (transform); 
			}
			Destroy (fire.gameObject);
		}
	}
}


