using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlackButton : MonoBehaviour
{
    GameObject controller;
    void Start(){
        controller = GameObject.FindGameObjectWithTag("GameController");
    }
    void OnMouseUp() {
        controller.GetComponent<Game>().moveBlack();
    }

    void OnMouseEnter() {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    void OnMouseExit(){
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
    }
}
