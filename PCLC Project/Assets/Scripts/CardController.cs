using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public Card card;
    public BoxCollider2D thisCardBoxCollider2D;
    public bool isMouseOver;

    private void Start()
    {
        thisCardBoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
    }

    private void OnMouseOver()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;   
    }

}

public enum CardSprite
{
    Daisy,
    Kirby,
    Link,
    Peach
}