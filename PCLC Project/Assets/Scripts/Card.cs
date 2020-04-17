using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Card : ScriptableObject
{
    public int cardID;
    public string cardName;
    public CardSprite sprite;
    public string MainText;
    public string LeftUpQuote;
    public string RightDownQuote;

    //public enum ChoiceDirection { left, right };

    //public void Left()
    //{
    //    Debug.Log(cardName + " swiped left.");
    //    NextCard(ChoiceDirection.left);
    //}
    //public void Right()
    //{
    //    Debug.Log(cardName + " swiped right.");
    //    NextCard(ChoiceDirection.right);
    //}

    //public void NextCard(ChoiceDirection choice)
    //{
    //    switch (cardID)
    //    {
    //        case 0:
    //            if (choice == ChoiceDirection.left)
    //            {

    //            }
    //            else
    //            {

    //            }
    //            break;
    //        case 1:
    //            if (choice == ChoiceDirection.left)
    //            {

    //            }
    //            else
    //            {

    //            }
    //            break;
    //    }
    //}
}