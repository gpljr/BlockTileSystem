using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonHighlight : MonoBehaviour {
	[SerializeField] Sprite highlightSprite;
	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Button>().onClick.AddListener(Highlight);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void Highlight()
	{
		print("highlight");
		gameObject.GetComponent<Button>().image.overrideSprite = highlightSprite;
	}
}
