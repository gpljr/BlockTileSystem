using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelProgressionImage : MonoBehaviour {
	[SerializeField] Sprite[] sprite = new Sprite[16];




	void OnEnable () {
        Events.g.AddListener<LevelLoadedEvent>(LevelLoaded);
    }
    void OnDisable () {
        Events.g.RemoveListener<LevelLoadedEvent>(LevelLoaded);
    }
    void LevelLoaded (LevelLoadedEvent e) {

        	switchImage(sprite[e.iLevel-1]);

    }
    void switchImage(Sprite sprite)
    {
    	gameObject.GetComponent<Image>().sprite=sprite;
    }
}
