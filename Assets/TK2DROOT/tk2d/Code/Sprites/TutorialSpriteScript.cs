using UnityEngine;
using System.Collections;

public class TutorialSpriteScript : MonoBehaviour {
    tk2dSprite sprite;

    void Start() {
        sprite = GetComponent<tk2dSprite>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            sprite.color = Color.red;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            sprite.color = Color.white;
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            sprite.scale = new Vector3(2, 2, 2);
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            sprite.SetSprite("crate");
        }
		if (Input.GetKeyDown(KeyCode.Z)) {
            int spriteId = sprite.GetSpriteIdByName("Rock");
			sprite.SetSprite(spriteId);
        }	
    }
}