using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class HeartBeat : MonoBehaviour
{

	private Image heartImage;

	private float _currentScale = InitScale;
	private const float TargetScale = 0.6f;
	private const float InitScale = 0.5f;
	private const int FramesCount = 100;
	private const float AnimationTimeSeconds = 0.5f;
	private float _deltaTime = AnimationTimeSeconds / FramesCount;
	private float _dx = (TargetScale - InitScale) / FramesCount;
	private bool _upScale = true;

	// Объявляем, какой метод отвечает на события HEAT_COLOR(цвет сердца изменяется при потери жизни)
	private void Awake ()
	{
		Messenger.AddListener (GameEvent.HEART_COLOR, WhiteHeart); 
	}

	// При разрушении объекта удаляем подписчика, чтобы избежать ошибок.
	private void OnDestroy ()
	{
		Messenger.RemoveListener (GameEvent.HEART_COLOR, WhiteHeart); 
	}

	private IEnumerator Breath ()
	{
		while (true) {
			while (_upScale) {
				_currentScale += _dx;
				if (_currentScale > TargetScale) {
					_upScale = false;
					_currentScale = TargetScale;
				}
				transform.localScale = Vector3.one * _currentScale;
				yield return new WaitForSeconds (_deltaTime);
			}

			while (!_upScale) {
				_currentScale -= _dx;
				if (_currentScale < InitScale) {
					_upScale = true;
					_currentScale = InitScale;
				}
				transform.localScale = Vector3.one * _currentScale;
				yield return new WaitForSeconds (_deltaTime);
			}
		}
	}

	private void Start ()
	{
		StartCoroutine (Breath ());
	}

	public void WhiteHeart ()
	{
		StartCoroutine (Wait ());
	}

	IEnumerator Wait ()
	{
		heartImage =	transform.GetComponent<Image> ();
		heartImage.color = new Color (0, 235, 235, 255);
		yield return new WaitForSeconds (1);
		heartImage.color = new Color (235, 235, 235, 255);
	}
}
