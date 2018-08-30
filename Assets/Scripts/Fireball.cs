using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{
	
	// Скорость пули
	public float speed = 25.0f;

	private void Update ()
	{
		transform.Translate (Vector2.right * speed * Time.deltaTime);
		if (transform.position.x > 6.5f || transform.position.y > 6.0f)
			Destroy (this.gameObject);
	}

	private void OnTriggerEnter2D (Collider2D other)
	{ 
		Inventory inv = other.transform.GetComponent<Inventory> ();
		if (inv != null) {
			if (inv._type == "BlinkClock(Clone)" && inv._clockTaken == true) { 
				Messenger<string>.Broadcast (GameEvent.RECEIVED_CLOCK_INV, "BlinkClock(Clone)");  
				inv.gameObject.SetActive (false);
				inv._clockTaken = false;
			}
			if (inv._type == "BlinkHeart(Clone)" && inv._heartTaken == true)
				Messenger<string>.Broadcast (GameEvent.RECEIVED_CLOCK_INV, "BlinkHeart(Clone)");
			inv.gameObject.SetActive (false);
			inv._heartTaken = false;
		} 
	}
}
