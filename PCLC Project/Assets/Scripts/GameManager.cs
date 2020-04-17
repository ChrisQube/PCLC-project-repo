using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject cardGameObject;
    public SpriteRenderer cardSpriteRenderer;

    public ResourceManager cardResourceManager;

    public CardController mainCardController;

    [Header("Tweaking variables")]
    public float fMovingSpeed;
    Vector3 pos;

    [Header("UI")]
    public TextMeshProUGUI display; //for debugging
    public TextMeshPro characterName; //CAN BE DELETED TOO
    public TextMeshPro mainText;
    public TextMeshPro upText;
    public TextMeshPro downText;

    [Header("Card Margins")]
    public float f_xDistanceToMargin;
    public float f_yDistanceToMargin;
    public float f_xDistanceToTrigger;
    
    [Header("Card variables")]
    private string LeftUpQuote;
    private string RightDownQuote;
    private string MainTextQuote;
    public Card currentCard;
    public Card testCard;

    //Card swipe states
    public enum ChoiceDirection { left, right };

    void Start()
    {
        LoadCard(testCard);
    }

    private float SmoothCardYPosition(float originalY)
    {
        if (originalY > 0)
        {
            //y=-a^{0.652+x}+0.35
            return -Mathf.Pow(0.2f, (0.652f + originalY)) + 0.35f;
        }
        else
        {
            //y=a^{0.456+x}-0.35
            return Mathf.Pow(0.2f, -(-0.456f + originalY)) - 0.35f;
        }
    }

    void Update()
    {
       //Movement
       if (Input.GetMouseButton(0) && mainCardController.isMouseOver)
       {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //if (pos.y > 0.35)
            //{
            //    pos.y = 0.35f;
            //}
            //else if(pos.y < -0.35)
            //{
            //    pos.y = -0.35f;
            //}
            pos.y = SmoothCardYPosition(pos.y);
            cardGameObject.transform.position = pos;
       }
       else
        {
            cardGameObject.transform.position = Vector2.MoveTowards(cardGameObject.transform.position, new Vector2(0, 0), fMovingSpeed);
        }
        display.text = "x = " + cardGameObject.transform.position.x.ToString() + "; y = " + cardGameObject.transform.position.y.ToString();


        //CARD details update every frame (can make more efficient by only updating when cards load)
        //upText.text = LeftUpQuote;
        //downText.text = RightDownQuote;
        //mainText.text = MainTextQuote;
        
        ////Checking sides
        //Right Side +/- Down
        if (cardGameObject.transform.position.x > 0)//&& cardGameObject.transform.position.y < 0)
        {
            //ignore
            //int XAxisAlphaValue = (int)(Mathf.Min(cardGameObject.transform.position.x, 1) * 255);
            //int YAxisAlphaValue = (int)(Mathf.Min(cardGameObject.transform.position.y / 0.35f, 1) * 255);
            //byte finalAlphaValue = (byte)(Mathf.Max(XAxisAlphaValue, YAxisAlphaValue));
            //downText.faceColor = new Color32(0, 0, 0, finalAlphaValue);

            byte XAxisAlphaValue = (byte)(Mathf.Min(cardGameObject.transform.position.x, 1) * 255);
            downText.faceColor = new Color32(0, 0, 0, XAxisAlphaValue);

            if (Input.GetMouseButtonUp(0))//if (!Input.GetMouseButton(0) && XAxisAlphaValue > 220)
            {
                NextCard(ChoiceDirection.right);
                //Debug.Log("Right");
            }

            //Debug.Log("a = " + Mathf.Min(cardGameObject.transform.position.x, 1));
            //Debug.Log("alpha = " + XAxisAlphaValue);
            //display.text = "x = " + cardGameObject.transform.position.x.ToString() + "; y = " + cardGameObject.transform.position.y.ToString();
            //cardSpriteRenderer.color = Color.green;

            //mainText.text = XAxisAlphaValue.ToString();
        }
        //Left Side +/- Up
        else if (cardGameObject.transform.position.x < 0)// && cardGameObject.transform.position.y > 0) 
        {
            //ignore
            //int XAxisAlphaValue = (int)(Mathf.Min(-cardGameObject.transform.position.x, 1) * 255); //finds the Alpha value based of position X
            //int YAxisAlphaValue = (int)(Mathf.Min(-cardGameObject.transform.position.y / 0.35f, 1) * 255); //finds the alpha value based of position Y
            //byte finalAlphaValue = (byte)(Mathf.Max(XAxisAlphaValue, YAxisAlphaValue)); //compares between X and Y for maximum opacity
            //upText.faceColor = new Color32(0, 0, 0, finalAlphaValue);

            byte XAxisAlphaValue = (byte)(Mathf.Min(-cardGameObject.transform.position.x, 1) * 255);
            upText.faceColor = new Color32(0, 0, 0, XAxisAlphaValue);

            if (Input.GetMouseButtonUp(0))//if (!Input.GetMouseButton(0) && XAxisAlphaValue > 220)
            {
                NextCard(ChoiceDirection.left);
                //Debug.Log("Left");
            }

            //downText.alpha = Mathf.Min(-cardGameObject.transform.position.x, 1);
            //Debug.Log("a = " + Mathf.Min(-cardGameObject.transform.position.x, 1));
            //display.text = "x = " + cardGameObject.transform.position.x.ToString() + "; y = " + cardGameObject.transform.position.y.ToString();
            //cardSpriteRenderer.color = Color.red;

            //mainText.text = XAxisAlphaValue.ToString();
        }
        else
        {
            upText.faceColor = new Color32(0, 0, 0, 50);
            downText.faceColor = new Color32(0, 0, 0, 50);
            //cardSpriteRenderer.color = Color.white;
        }
    }

    public void LoadCard(Card card)
    {
        currentCard = card;
        cardSpriteRenderer.sprite = cardResourceManager.sprites[(int)card.sprite];
        LeftUpQuote = card.LeftUpQuote;
        RightDownQuote = card.RightDownQuote;
        MainTextQuote = card.MainText;
        //CARD details update when cards load
        upText.text = LeftUpQuote;
        downText.text = RightDownQuote;
        mainText.text = MainTextQuote;

        Debug.Log("Card: " + currentCard.cardName + "has finished loading.");

    }

    public void NewCard()
    {
        //Needs randomizer
        //Needs an integer of count, i.e. if the game only has 14 cards, then needs to know which "act/section" the game is in to choose from a different set of cards.
        //Needs randomizer when the "act/section" is known.
            //When specific cardcounts come out; then some cards are "must sees/major life events/decisions"
    }

    public void NextCard(ChoiceDirection choice)
    {
        Debug.Log("CardID " + currentCard.cardName + " swiped " + choice.ToString());

        switch (currentCard.cardID)
        {
            case 0: //Tutorial 1
                if (choice == ChoiceDirection.left)
                { LoadCard(cardResourceManager.cards[1]); }
                else
                {
                    //do nothing
                }
                break;
            case 1: //Tutorial 2
                if (choice == ChoiceDirection.left)
                {
                    //do nothing
                }
                else
                {
                    LoadCard(cardResourceManager.cards[2]);
                }
                break;
            case 2: //Tutorial 2
                if (choice == ChoiceDirection.left)
                {
                    //START GAME
                }
                else
                {
                    //restart tutorial
                    LoadCard(cardResourceManager.cards[0]);
                }
                break;
        }
    }

}

