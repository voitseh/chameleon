using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
	
	// Расстояние между цыфрами
	private const float DIGITS_DISTANSE = 0.4f;
	private const string SPRITE_TAG_TIME = "DIGIT_SPRITE_TIME";
	private const string SPRITE_TAG_LIFE = "DIGIT_SPRITE_LIFE";
	private const string SPRITE_TAG_SCORE = "DIGIT_SPRITE_SCORE";
	// Пул набоев
	private const int MAX_WEAPONS_POOL = 100;
	// Количество жизней на старте
	private const int MAX_PLAYER_LIFES = 5;
	// Максимальное время
	private const float TIME_ON_START = 100;
	// Количество врагов на старте
	private const int MAX_ENEMIES = 25;
	private static float _curWeaponCount;
	[SerializeField] 
	private Slider weapon;
	// Время игры
	[HideInInspector]
	public static float _curTime;
	// Актуальное количество жизней
	private int _curLife;
	// Количество врагов которых осталось уничтожить
	private int _curScore;
	// Условие нажатия паузы
	public static int paused = 1;
	[SerializeField]
	private GameObject PlayOrPauseButton;
	// Спрайты, меняющиеся на кнопке
	[SerializeField]
	private Sprite Play;
	[SerializeField]
	private Sprite Pause;
	[SerializeField]
	private	GameObject _winLabel;
	[SerializeField]
	private	GameObject _gameOverLabel;
	[SerializeField]
	private	GameObject _buttonNEW;

	// Объявляем, какие методы отвечают на события.
	private	void Awake ()
	{
		Messenger.AddListener (GameEvent.ENEMY_HIT, OnEnemyHit); 
		Messenger.AddListener (GameEvent.FIRE, WeaponCount); 
		Messenger.AddListener (GameEvent.CHAMELEON_FALL, OnChameleonFall); 
		Messenger<string>.AddListener (GameEvent.RECEIVED_CLOCK_INV, OnInventoryTaken); 
		Messenger<string>.AddListener (GameEvent.RECEIVED_HEART_INV, OnInventoryTaken); 
	}
	// При разрушении объекта удаляем подписчика, чтобы избежать ошибок.
	private	void OnDestroy ()
	{
		Messenger.RemoveListener (GameEvent.ENEMY_HIT, OnEnemyHit); 
		Messenger.RemoveListener (GameEvent.FIRE, WeaponCount); 
		Messenger.RemoveListener (GameEvent.CHAMELEON_FALL, OnChameleonFall);
		Messenger<string>.RemoveListener (GameEvent.RECEIVED_CLOCK_INV, OnInventoryTaken); 
		Messenger<string>.RemoveListener (GameEvent.RECEIVED_HEART_INV, OnInventoryTaken); 
	}

	private	void Start ()
	{ 
		_curTime = TIME_ON_START;
		_curWeaponCount = MAX_WEAPONS_POOL;
		_curLife = MAX_PLAYER_LIFES;
		_curScore = MAX_ENEMIES;
	}
	// Метод ответственный количество огняных шаров у хамелеона
	private void WeaponCount ()
	{
		_curWeaponCount--;
		if (_curWeaponCount == 0) {
			_curLife--;
			Messenger.Broadcast (GameEvent.HEART_COLOR);
			ShowLife ();
			if (_curLife != 0)
				StartCoroutine (Wait ());
			else
				_gameOverLabel.transform.GetComponent<Scaler> ().GameOver ();
		} else
			weapon.value = _curWeaponCount;
	}

	// Задержка времени перед пополнением арсенала оружия
	IEnumerator Wait ()
	{
		yield return new WaitForSeconds (1);
		_curWeaponCount = MAX_WEAPONS_POOL;
	}

	// Если подобран инвентарь
	private void OnInventoryTaken (string type)
	{
		if (type == "BlinkClock(Clone)")
			_curTime += 10;
		if (type == "BlinkHeart(Clone)") {
			_curLife++;
			ShowLife ();
		}
	}

	// Уменьшение переменной score на 1 в ответ на данное событие(попадание у врага).
	private void OnEnemyHit ()
	{
		_curScore--;
		// Убито всех врагов
		if (_curScore == 0)
			_winLabel.transform.GetComponent<Scaler> ().PlayerWin ();
	}
	// Если хамелеона "зацепила муха",он падает и теряет одну жизнь
	public void OnChameleonFall ()
	{
		_curLife--;
		Messenger.Broadcast (GameEvent.HEART_COLOR);
		ShowLife ();
		if (_curLife == 0)
			_gameOverLabel.transform.GetComponent<Scaler> ().GameOver ();
	}

	private	int ratioX, ratioY;

	private	void Update ()
	{                                                  
		
		if (paused % 2 == 0) {
			
			_curTime -= Time.deltaTime;

			ShowScore (_curScore);
			ShowTime ();
		} else
			Time.timeScale = 0;
	}
	// Блок методов,отображающих числовые значения на панеле UI-елементов
	//***********************************************************************************************
	// Функция для отображения время игры в виде спрайтов
	public void ShowTime ()
	{
		// Удаляет предидущие значения
		GameObject[] oldDigitsTime = GameObject.FindGameObjectsWithTag (SPRITE_TAG_TIME);
		foreach (var oldDigit in oldDigitsTime) {
			Destroy (oldDigit);
		}
		_curTime = Mathf.Clamp (_curTime, 0, 100);
		string	digitStr = _curTime.ToString ("000");
		if (digitStr == "001")
			StartCoroutine (TimeOver ());
		for (int i = 0; i < digitStr.Length; i++) {   
			Vector3 pos = new Vector3 (DIGITS_DISTANSE * i - 5, 5.1f, -2);

			GameObject _digit = Instantiate (GameObject.Find (digitStr.Substring (i, 1)), pos, Quaternion.identity) as GameObject;
			// Назначает тэг обьекту для последующего удаления
			_digit.tag = SPRITE_TAG_TIME;
		}
	}

	// Функция для отображения ЖИЗНЕЙ в виде спрайтов
	private void ShowLife ()
	{   
		GameObject oldDigitLife = GameObject.FindGameObjectWithTag (SPRITE_TAG_LIFE);
		Destroy (oldDigitLife);
		_curLife = Mathf.Clamp (_curLife, 0, 5);
		string	digitStr = _curLife.ToString ();
		Vector3 pos = new Vector3 (-4.7f, 4f, -2);
		GameObject _digit = Instantiate (GameObject.Find (digitStr.Substring (0, 1)), pos, Quaternion.identity) as GameObject;
		_digit.tag = SPRITE_TAG_LIFE;
	}

	// Функция для отображения счета игры в виде спрайтов
	public void ShowScore (int _curScore)
	{
		// Удаляет предидущие значения
		GameObject[] oldDigitScore = GameObject.FindGameObjectsWithTag (SPRITE_TAG_SCORE);
		foreach (var oldDigit in oldDigitScore) {
			Destroy (oldDigit);
		}
		_curScore = Mathf.Clamp (_curScore, 0, 25);
		string	digitStr = _curScore.ToString ("000");
		for (int i = 0; i < digitStr.Length; i++) {   
			Vector3 pos = new Vector3 (DIGITS_DISTANSE * i - 5.2f, 2.1f, -2);
			GameObject _digit = Instantiate (GameObject.Find ("_" + digitStr.Substring (i, 1)), pos, Quaternion.identity) as GameObject;
			// Назначает тэг обьекту для последующего удаления
			_digit.tag = SPRITE_TAG_SCORE;
		}
	}

	IEnumerator TimeOver ()
	{ 
		_gameOverLabel.transform.GetComponent<Scaler> ().GameOver ();
		yield return new WaitForSeconds (5);
	}
	//***************************************************************************************
	// Метод, вызываемый кнопкой паузы.
	public void OnPlayPause ()
	{
		// Ети кнопки не должны срабатывать если на экране есть кнопка "NEW"
		if (!_buttonNEW.activeSelf) {
			ShowLife ();
			paused++;
			if (paused % 2 != 0) {
				Time.timeScale = 0;
				PlayOrPauseButton.transform.GetComponent<Image> ().sprite = Play;
			} else {
				Time.timeScale = 1;
				PlayOrPauseButton.transform.GetComponent<Image> ().sprite = Pause; 
			}
		}
	}
	// Метод, вызываемый кнопкой выхода
	public void OnExit ()
	{
		Application.Quit ();
	}
	// Метод, вызываемый кнопкой "new"
	public void OnNEW ()
	{ 
		EnemyController.EnemiesVisibility ();
		PlayerCharacter.walk = true;
		Scaler.flag = true;
		Time.timeScale = 1;
		_curTime = TIME_ON_START;
		_curWeaponCount = MAX_WEAPONS_POOL;
		WeaponCount ();
		_curLife = MAX_PLAYER_LIFES;
		ShowLife ();
		_curScore = MAX_ENEMIES;
	}
}