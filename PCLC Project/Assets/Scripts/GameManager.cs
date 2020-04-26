using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

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
    public int cardStartID;

    //Card
    private string[,] cardList = new string[155,11];
    List<string> stringList = new List<string>();
    private int currentActiveCardRow;

    //Values
    private List<string> valuesList;

    //Card swipe states
    public enum ChoiceDirection { left, right };

    //CSV column headings
    public enum columnHeading { cardEnum = 0, cardID = 1, ActSection = 2, Main_body_text = 3, LeftOptionText = 4, LeftNextCardID = 5, LeftValue = 6, RightOptionText = 7, RightNextCardID = 8, RightValue = 9, Randomize = 10};

    void Start()
    {
        valuesList = new List<string>();

        readTextFile();
        currentActiveCardRow = cardStartID;
        LoadCardFromArray(currentActiveCardRow);

        //LoadCard(testCard);
    }

    public void readTextFile()
    {
        StreamReader input_Stream = new StreamReader("Assets/Cards/PCLC cards.csv");

        while (!input_Stream.EndOfStream)
        {
            string input_line = input_Stream.ReadLine();

            stringList.Add(input_line);
        }
        
        input_Stream.Close();

        //Debug.Log("stringList count = " + stringList.Count);

        //putting input_line -> cardList[,]
        for (int row = 0; row < stringList.Count; row++)
        {
            string[] string_columns = stringList[row].Split('|'); //This is char?
            for (int col = 0; col < string_columns.Length; col++)
            {
                cardList[row, col] = string_columns[col];
            }
        }

        //Debug.Log("Array complete.");
        //Debug.Log("Row 5, col 4 = " + cardList[4,3]);
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

        //Debug.Log("x = " + cardGameObject.transform.position.x);
        if (cardGameObject.transform.position.x > 0)//&& cardGameObject.transform.position.y < 0)
        {
            //ignore
            //int XAxisAlphaValue = (int)(Mathf.Min(cardGameObject.transform.position.x, 1) * 255);
            //int YAxisAlphaValue = (int)(Mathf.Min(cardGameObject.transform.position.y / 0.35f, 1) * 255);
            //byte finalAlphaValue = (byte)(Mathf.Max(XAxisAlphaValue, YAxisAlphaValue));
            //downText.faceColor = new Color32(0, 0, 0, finalAlphaValue);

            byte XAxisAlphaValue = (byte)(Mathf.Min(cardGameObject.transform.position.x, 1) * 255);
            downText.faceColor = new Color32(0, 0, 0, XAxisAlphaValue);

            //Debug.Log("x is postive");

            if (Input.GetMouseButtonUp(0))//if (!Input.GetMouseButton(0) && XAxisAlphaValue > 220)
            {
                //NextCard(ChoiceDirection.right); //old
                NextCardFromArray(ChoiceDirection.right);
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

            //Debug.Log("x is negative");

            byte XAxisAlphaValue = (byte)(Mathf.Min(-cardGameObject.transform.position.x, 1) * 255);
            upText.faceColor = new Color32(0, 0, 0, XAxisAlphaValue);

            if (Input.GetMouseButtonUp(0))//if (!Input.GetMouseButton(0) && XAxisAlphaValue > 220)
            {
                //NextCard(ChoiceDirection.left); //old
                NextCardFromArray(ChoiceDirection.left);
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
            //Debug.Log("Card not moved.");
            //cardSpriteRenderer.color = Color.white;
        }
    }

    public void LoadCardFromArray(int cardID)
    {
        if (cardID == 800)
        {
            //ENDING trigger
        }
        else
        {
            for (int row = 1; row < cardList.GetLength(0); row++)
            {
                if (cardID == int.Parse(cardList[row, (int)columnHeading.cardID])) //this is the right row for the cards! Now we need to load the card.
                {
                    currentActiveCardRow = row;

                    LeftUpQuote = cardList[row, (int)columnHeading.LeftOptionText];
                    RightDownQuote = cardList[row, (int)columnHeading.RightOptionText];
                    MainTextQuote = cardList[row, (int)columnHeading.Main_body_text];

                    //need to remove extra "" from start and end of string
                    if (MainTextQuote[0] == '"')
                    {
                        MainTextQuote = MainTextQuote.Trim('"');
                        MainTextQuote = "'" + MainTextQuote + "'";
                    }
                    if (LeftUpQuote[0] == '"')
                    {
                        LeftUpQuote = LeftUpQuote.Trim('"');
                        LeftUpQuote = "'" + LeftUpQuote + "'";
                    }
                    if (RightDownQuote[0] == '"')
                    {
                        RightDownQuote = RightDownQuote.Trim('"');
                        RightDownQuote = "'" + RightDownQuote + "'";
                    }

                    //CARD details update when cards load
                    upText.text = LeftUpQuote;
                    downText.text = RightDownQuote;
                    mainText.text = MainTextQuote;

                    break;
                }
            }
        }
        
        Debug.Log("Card has finished loading without quiting the function.");
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

    public void NextCardFromArray(ChoiceDirection choice)
    {
        Debug.Log("CardID : " + currentActiveCardRow + " swiped " + choice.ToString());
        
        if (choice == ChoiceDirection.left)
        {
            //Give specific value points
            if (cardList[currentActiveCardRow, (int)columnHeading.LeftValue] == "")
            {
                //do nothing if there is nothing here
            }
            else
            {
                valuesList.Add(cardList[currentActiveCardRow, (int)columnHeading.LeftValue]);

                Debug.Log("valuesList = " + string.Join("", new List<string>(valuesList).ConvertAll(i => i.ToString()).ToArray()));
            }

            //Check if the next card needs to be randomized
            if (cardList[currentActiveCardRow, (int)columnHeading.Randomize] == "TRUE")
            {
                //randomize
                string[] randomizedCol = cardList[currentActiveCardRow, (int)columnHeading.LeftNextCardID].Split(','); //This is char?
                Debug.Log("Choices #:" + randomizedCol.Length);
                LoadCardFromArray(int.Parse(randomizedCol[Random.Range(0, 2)]));
            }
            else //load card Left choice if NOT randomized
            {
                LoadCardFromArray(int.Parse(cardList[currentActiveCardRow, (int)columnHeading.LeftNextCardID]));
            }
        }
        else
        {
            //Check if the next card needs to be randomized
            if (cardList[currentActiveCardRow, (int)columnHeading.Randomize] == "TRUE")
            {
                //randomize
                string[] randomizedCol = cardList[currentActiveCardRow, (int)columnHeading.RightNextCardID].Split(','); //This is char?
                Debug.Log("Choices #:" + randomizedCol.Length);
                LoadCardFromArray(int.Parse(randomizedCol[Random.Range(0, 2)]));
            }
            else //load card Right choice if NOT randomized
            {
                LoadCardFromArray(int.Parse(cardList[currentActiveCardRow, (int)columnHeading.RightNextCardID]));
            }
        }
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

