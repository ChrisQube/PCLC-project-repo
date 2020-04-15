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

    public TextMeshProUGUI display; //for debugging
    public TextMeshPro mainText;
    public TextMeshPro upText;
    public TextMeshPro downText;

    public float f_xDistanceToMargin;
    public float f_yDistanceToMargin;

    public float f_xDistanceToTrigger;

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
        //Right Side +/- Down
        if (cardGameObject.transform.position.x > 0)//f_xDistanceToMargin) // && cardGameObject.transform.position.y < -f_yDistanceToTrigger)
        {
            byte alphaValue = (byte)(Mathf.Min(cardGameObject.transform.position.x, 1) * 255);
            downText.faceColor = new Color32(0, 0, 0, alphaValue);
            //Debug.Log("a = " + Mathf.Min(cardGameObject.transform.position.x, 1));
            Debug.Log("alpha = " + alphaValue);
            //display.text = "x = " + cardGameObject.transform.position.x.ToString() + "; y = " + cardGameObject.transform.position.y.ToString();
            //cardSpriteRenderer.color = Color.green;

            mainText.text = alphaValue.ToString();
        }
        //Left Side +/- Up
        else if (cardGameObject.transform.position.x < 0)//-f_xDistanceToMargin) // && cardGameObject.transform.position.y > f_yDistanceToTrigger) 
        {

            byte alphaValue = (byte)(Mathf.Min(-cardGameObject.transform.position.x, 1) * 255);
            upText.faceColor = new Color32(0, 0, 0, alphaValue);
            //downText.alpha = Mathf.Min(-cardGameObject.transform.position.x, 1);
            //Debug.Log("a = " + Mathf.Min(-cardGameObject.transform.position.x, 1));
            //display.text = "x = " + cardGameObject.transform.position.x.ToString() + "; y = " + cardGameObject.transform.position.y.ToString();
            //cardSpriteRenderer.color = Color.red;

            mainText.text = alphaValue.ToString();
        }
        else
        {
            upText.faceColor = new Color32(0, 0, 0, 50);
            downText.faceColor = new Color32(0, 0, 0, 50);
            //cardSpriteRenderer.color = Color.white;
        }
    }

    private void OnMouseUp()
    {
        if (!Input.GetMouseButton(0) && cardGameObject.transform.position.x > f_xDistanceToTrigger)
        {
            //mainCardController.InduceRight();
            Debug.Log("Right");
        }
        else if(!Input.GetMouseButton(0) && cardGameObject.transform.position.x < -f_xDistanceToTrigger)
        {
            //mainCardController.InduceLeft();
            Debug.Log("Left");
        }
    }

}
