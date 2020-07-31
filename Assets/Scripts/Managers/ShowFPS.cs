using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowFPS : MonoBehaviour {

	public Text text;
	public float deltaTime;

	private void Start() {
		text.color = Color.green;
	}

	void Update () {
		if(Time.timeScale > 0)
		{
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			float fps = 1.0f / deltaTime;
			text.text = Mathf.Ceil(fps).ToString();
		}
	}
}
