using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {

	[HideInInspector]
	public string _type{ get; private set;}
	// Эти поля "смотрят" чтоб инвентарь был подобран лишь один раз
	[HideInInspector]
	public bool _clockTaken = true;
	[HideInInspector]
	public bool _heartTaken = true;

	// Use this for initialization
	private	void Start () {
		_type = gameObject.name;
	}

}

