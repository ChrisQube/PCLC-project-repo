using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject cardGameObject;
    public SpriteRenderer cardSpriteRenderer;

    public CardController mainCardController;

    public float fMovingSpeed;
    Vector3 pos;
    public TextMeshProUGUI display;

    public float f_xDistanceToTrigger;
    public float f_yDistanceToTrigger;

    void Start()
    {
        
    }

    void Update()
    {
       //Movement
       if (Input.GetMouseButton(0) && mainCardController.isMouseOver)
       {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cardGameObject.transform.position = pos;
       }
       else
        {
            cardGameObject.transform.position = Vector2.MoveTowards(cardGameObject.transform.position, new Vector2(0, 0), fMovingSpeed);
        }
        display.text = "x = " + cardGameObject.transform.position.x.ToString() + "; y = " + cardGameObject.transform.position.y.ToString();

        ////Checking sides
        //Right Side + Up
        if (cardGameObject.transform.position.x > f_xDistanceToTrigger && cardGameObject.transform.position.y < -f_yDistanceToTrigger)
        {
            
            //display.text = "x = " + cardGameObject.transform.position.x.ToString() + "; y = " + cardGameObject.transform.position.y.ToString();
            cardSpriteRenderer.color = Color.green;

            if (!Input.GetMouseButton(0))
            {
                //mainCardController.InduceRight();
            }
        }
        //Left Side + Down
        else if (cardGameObject.transform.position.x < -f_xDistanceToTrigger && cardGameObject.transform.position.y > f_yDistanceToTrigger) 
        {

            //display.text = "x = " + cardGameObject.transform.position.x.ToString() + "; y = " + cardGameObject.transform.position.y.ToString();
            cardSpriteRenderer.color = Color.red;

            if (!Input.GetMouseButton(0))
            {
                //mainCardController.InduceLeft();
            }
        }
        else
        {
            cardSpriteRenderer.color = Color.white;
        }
    }
}
