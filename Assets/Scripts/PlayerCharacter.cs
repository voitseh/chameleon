using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerCharacter : MonoBehaviour
{

	[SerializeField] 
	private GameObject fireballPrefab;
	private GameObject _fireball;
	// Спрайты для анимации головы хамелеона
	[SerializeField]
	private GameObject[] _chameleonHeads = new GameObject[6];
	[SerializeField]
	private Camera _camera;
	[SerializeField]
	private Texture2D cursor;
	// Объявляем переменные, задающие область сцены, где можна ловить врагов.
	private float minimumX = 155.0f;
	private float maximumX = 800.0f;
	private float minimumY = 70.0f;
	private float maximumY = 600.0f;
	// Хамелеон идет
	public static bool walk;

	private void Start ()
	{
		// В начале включена анимация головы хамелеона при хотьбе
		for (int i = 0; i < _chameleonHeads.Length; i++) {
			if (i == 0 || i == 4)
				_chameleonHeads [i].SetActive (true);
			else
				_chameleonHeads [i].SetActive (false);
		}
	}
	// Под этим углом можна стрелять
	private float Angle (Vector3 end)
	{
		float angle = Mathf.Atan2 (end.y - 70, end.x - 155) * 180 / Mathf.PI;
		return angle;
	}

	private void Update ()
	{                                   
		if (walk)
			ChameleonWalks ();
		// Стреляем только при условиях что не нажата пауза и курсор не находится над кнопкой 
		if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject () && UIController.paused % 2 == 0 &&
		    Scaler.labelVisibility) {                            
			// Реакция на нажатие кнопки мыши.
			Vector3 point = Input.mousePosition; 
			point.z = 10; 
			float	px = Mathf.Clamp (point.x, minimumX, maximumX);
			float	py = Mathf.Clamp (point.y, minimumY, maximumY);
			// Угол,необходим для того чтоб проигрывать нужную анимацию головы хамелеона
			float angle = Angle (point); 
			// Анимация головы хамелеона при нажатии левой кнопки мыши(выстрел)
			StartCoroutine (Shot (angle));
			if (point.x == px && point.y == py) {
				// Создаем огненый шар 
				_fireball = Instantiate (fireballPrefab) as GameObject;
				// К реакции на выстрел добавлена рассылка сообщения.
				Messenger.Broadcast (GameEvent.FIRE); 
				// Поместим огненный шар перед хамелеоном и нацелим в направлении курсора.
				_fireball.transform.position = new Vector3 (-3.8f, -2.5f, -2);  
				_fireball.transform.Rotate (0, 0, angle);
			}
		}
	}

	// Реакция на столкновение хамелеона с врагом
	private void OnTriggerEnter2D (Collider2D other)
	{ 
		ReactiveTarget player = other.GetComponent<ReactiveTarget> (); 
		Inventory inventory = other.GetComponent<Inventory> ();
		// Если враг живой столкнулся а не мертвый упал сверху
		if (player != null && player._alive == true) {
			walk = false;
			ChameleonFall ();
			// Шлем сообщение UIController-у о падении чтобы отнять одну жизнь в хамелеона
			Messenger.Broadcast (GameEvent.CHAMELEON_FALL);
		}
		if (inventory != null) {
			if (inventory._type == "BlinkClock(Clone)" && inventory._clockTaken == true) {
				Messenger<string>.Broadcast (GameEvent.RECEIVED_CLOCK_INV, "BlinkClock(Clone)");                  
				inventory._clockTaken = false;
			}
			if (inventory._type == "BlinkHeart(Clone)" && inventory._heartTaken == true)
				Messenger<string>.Broadcast (GameEvent.RECEIVED_CLOCK_INV, "BlinkHeart(Clone)");
			inventory._heartTaken = false;
		}
	}

	// Хамелеон идет
	public 	void ChameleonWalks ()
	{
		_chameleonHeads [0].SetActive (true);
		_chameleonHeads [4].SetActive (true);
	}

	// Хамелеон падает
	public	void ChameleonFall ()
	{
		for (int i = 0; i < _chameleonHeads.Length; i++) {
			if (i == 5) {
				_chameleonHeads [i].SetActive (true);
				StartCoroutine (Fall ());
			} else
				_chameleonHeads [i].SetActive (false);
		}

	}

	// После падения анимация падения хамелеона не проигрывается
	IEnumerator Fall ()
	{
		yield return new WaitForSeconds (1);
		_chameleonHeads [5].transform.GetComponent<Animator> ().enabled = false;
		_chameleonHeads [5].SetActive (false);
		_chameleonHeads [5].transform.GetComponent<Animator> ().enabled = true;  
		if (Scaler.labelVisibility)
			walk = true;
	}

	IEnumerator Shot (float angle)
	{
		walk = false;
		if (angle > 0 && angle <= 30) {
			_chameleonHeads [0].SetActive (false);
			_chameleonHeads [1].SetActive (true);
		}
		if (angle > 30 && angle <= 60) {
			_chameleonHeads [0].SetActive (false);
			_chameleonHeads [2].SetActive (true);
		}
		if (angle > 60 && angle < 90) {
			_chameleonHeads [0].SetActive (false);
			_chameleonHeads [3].SetActive (true);
		}
		yield return new WaitForSeconds (0.2f); 
		for (int i = 0; i < _chameleonHeads.Length; i++) {
			if (i == 0 || i == 4)
				_chameleonHeads [i].SetActive (true);
			else
				_chameleonHeads [i].SetActive (false);
		}
	}
	// Визуальный индикатор для точки прицеливания
	private	void OnGUI ()
	{
		Vector2 MP = Input.mousePosition;
		MP.y = Screen.height - MP.y;
		MP.x -= 31;
		MP.y -= 31;
		GUI.DrawTexture (new Rect (MP.x, MP.y, 60, 60), cursor); 
	}
}
