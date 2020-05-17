using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject cardGameObject;
    public SpriteRenderer cardSpriteRenderer;

    public ResourceManager cardResourceManager;

    public CardController mainCardController;

    public GameObject wallpaperGameObject;

    [Header("Tweaking variables")]
    public float fMovingSpeed;
    public float fSwivleSpeed;
    Vector3 pos;
    public float fWallpaperMoveSpeed;

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

    [Header("Pending Card")]
    public bool isSubstituting = false;
    public float fRotatingSpeed;
    public Vector3 cardRotation;
    public Vector3 currentRotation; 
    public Vector3 initialRotation;

    //Card
    private string[,] cardList = new string[155,11];
    List<string> stringList = new List<string>();
    public int currentActiveCardRow;

    //Values
    private List<string> valuesList;

    //Card swipe states
    public enum ChoiceDirection { left, right };

    //CSV column headings
    public enum columnHeading { cardEnum = 0, cardID = 1, ActSection = 2, Main_body_text = 3, LeftOptionText = 4, LeftNextCardID = 5, LeftValue = 6, RightOptionText = 7, RightNextCardID = 8, RightValue = 9, Randomize = 10};

    [Header("Main Menu")]
    public float fBaseTextSize;
    public TextMeshPro titleText;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    private bool playedSound;
    //public ParticleSystem particles;
    //public GameObject particleGameObject;

    void Start()
    {
        valuesList = new List<string>();

        //readTextFile();
        LoadStringLines();
        currentActiveCardRow = cardStartID;
        LoadCardFromArray(currentActiveCardRow);

        //LoadCard(testCard);
    }

    private void LoadStringLines()
    {
        stringList.Add("card enum|cardID|Act/section|Main body test|LeftOption|LeftNextCardID|LeftValue|RightOption|RightNextCardID|RightValue|Random");
        stringList.Add("0|0|0|You can choose decisions by swiping left or right. Try swiping left!|Continue!|1||The other left!|||FALSE");
        stringList.Add("1|1|0|Nice! Now try swiping right!|This is still left!|||Continue!|2||FALSE");
        stringList.Add("2|2|0|Great work! This game is about your choices and values. Choose wisely!|Let's begin!|100||Wait... start the tutorial again!|0||FALSE");
        stringList.Add("3|100|1|In this game you will be making key decisions which shapes the outcomes of various characters.|Continue|101||Continue|101||FALSE");
        stringList.Add("4|101|1|It is late 2004. It is a calm Friday evening and you are tending to your baby son.|Continue|102||Continue|102||FALSE");
        stringList.Add("5|102|1|You currently also 24 weeks into your pregnancy, when contractions begin spontaneously.|Continue|103||Continue|103||FALSE");
        stringList.Add("6|103|1|You are rushed to hospital and another baby boy was born weighing 675g with a head circumference of 22cm.|Continue|104||Continue|104||FALSE");
        stringList.Add("7|104|1|He was born vaginally but in breech position and was unresponsive.|Continue|105||Continue|105||FALSE");
        stringList.Add("8|105|1|His APGAR was 2 for the first 5 minutes of life, and went to 3 at 10 minutes.|Continue|106||Continue|106||FALSE");
        stringList.Add("9|106|1|He is rushed to NICU. A decision was made to intubate the boy.|Continue|107||Continue|107||FALSE");
        stringList.Add("10|107|1|Despite the organized chaos of the delivery suite. Things have now settled and are quite.|I want to find my child.|108|Selflessness|I will go after a brief rest|108|Health|FALSE");
        stringList.Add("11|108|1|As you walk into the bustling NICU environment, you were greeted by staff.|I want to be with my child.|109|Relationships|I shouldn't create trouble for the staff.|109|Patience|FALSE");
        stringList.Add("12|109|1|'You must be the parents of Travis. He is just over here.'|Go find Travis.|110||Go find Travis.|110||FALSE");
        stringList.Add("13|110|1|Little Travis has been intubated and has a umbilical vein catheter.|'What have you done to my baby?!'|111|Justice|'How long will he need to be in here for?'|112|Trust|FALSE");
        stringList.Add("14|111|1|'We are doing everything we can to keep your baby healthy by providing oxygen and nutrients.'|'I see... How long will he need to be in here for?'|112||'I see... How long will he need to be in here for?'|112||FALSE");
        stringList.Add("15|112|1|'We...are not sure... it is too early to tell.'|'I see...'|201,219,227||'I see...'|201,219,227||TRUE");
        stringList.Add("16|201|2|A week has passed and the medical team has growing concerns of Travis's vision.|Continue|202||Continue|202||FALSE");
        stringList.Add("17|202|2|'We would like have Travis reviewed in Christchurch.'|Go to Christchurch|204|Health|I need to prepare my commitments here first.|203|Finances|FALSE");
        stringList.Add("18|203|2|'Unfortuantely, Christchurch is the closest place which has the equipment to investigate Travis.'|Okay|204||Okay|204||FALSE");
        stringList.Add("19|204|2|After a good 5 hour drive to Christchurch you arrive to the hospital.|Continue|205||Continue|205||FALSE");
        stringList.Add("20|205|2|Christchurch hospital feels even more busy.|Enquire about an eye appointment.|207|Courage|Best not distrub the staff.|206|Patience|FALSE");
        stringList.Add("21|206|2|No one seems to have noticed your presence.|Contiue waiting|206|Patience|Take the courage to ask.|207|Courage|FALSE");
        stringList.Add("22|207|2|You have been told to wait for your appointment.|Take a seat|208||Take a seat|208||FALSE");
        stringList.Add("23|208|2|The hospital continues to bustle and everything feels overwhelmingly new.|Continue to wait for your appointment.|210|Patience|Talk a walk to get some air|209|Health|FALSE");
        stringList.Add("24|209|2|You take some time to check in with yourself, at least the fresh air is relaxing.|Go back|210||Go back|210||FALSE");
        stringList.Add("25|210|2|Finally, your turn for a diagnosis. It feels like forever by now.|Continue|211||Continue|211||FALSE");
        stringList.Add("26|211|2|Within 10 minutes, the doctor informs you of the diagnosis.|Continue|212||Continue|212||FALSE");
        stringList.Add("27|212|2|'This is a typical presentation of bilateral retinopathy of prematurity.'|Continue|213||Continue|213||FALSE");
        stringList.Add("28|213|2|The doctor explains that this may cause Travis to be blind in one or both eyes.|Continue|215||Take a deep breath|214||FALSE");
        stringList.Add("29|214|2|You feel cold air enter your body and leave as swiftly as it came.|Continue|215||Continue|215||FALSE");
        stringList.Add("30|215|2|Complete with the appointment, you are unsure whether to drive back the same day.|Drive back tonight|216|Selflessness|Stay a night in Christchurch|217|Health|FALSE");
        stringList.Add("31|216|2|The sun hasn't yet set. You decide to push through the news, only 5 hours to get home.|Drive home|218||Drive home|218||FALSE");
        stringList.Add("32|217|2|It has been a long day already with little gain. Best leave tomorrow morning.|Rest|218||Rest|218||FALSE");
        stringList.Add("33|218|2|Arriving back home. You long for a good rest.|Rest|301||Rest|301||FALSE");
        stringList.Add("34|219|2|You notice that your baby's skin has a yellow-ish tint.|Inform someone|220|Courage|Probably just lighting|221|Patience|FALSE");
        stringList.Add("35|220|2|A medical staff agrees that Travis has jaundice.|Continue|222||Continue|222||FALSE");
        stringList.Add("36|221|2|After a few hours, a nurse informs you she has noticed that Travis is looking jaundiced.|Continue|222||Continue|222||FALSE");
        stringList.Add("37|222|2|Travis is treated with phototherapy. Which is successful.|Continue|223||Continue|223||FALSE");
        stringList.Add("38|223|2|You also wondered when Travis is able to start to be fed by orally as he is currently using an NG tube.|Ask somebody|224|Courage|They will tell me when I need to know|226|Trust|FALSE");
        stringList.Add("39|224|2|Wanting to feel more prepared you ask someone.|Continue|225||Continue|225||FALSE");
        stringList.Add("40|225|2|They apologise as they are unsure, they estimate around one month.|Continue|226||Continue|226||FALSE");
        stringList.Add("41|226|2|After around one month, Travis is still fed through an NG tube.|Continue|301||Continue|301||FALSE");
        stringList.Add("42|227|2|Travis has now been in NICU for 2 weeks.|Continue|228||Continue|228||FALSE");
        stringList.Add("43|228|2|You have become somewhat familiar with a few of the families in NICU.|We can support each other|229|Trust|Good to make new friends|229|Relationships|FALSE");
        stringList.Add("44|229|2|This week, one of the families are very distressed|Ask if they are okay|230|Relationships|Give them space|236|-|FALSE");
        stringList.Add("45|230|2|Their baby currently has an infection in the blood and have been given antibiotics.|Continue|231||Continue|231||FALSE");
        stringList.Add("46|231|2|Medical staff have told them that there is a chance of death, but this is low.|Continue|232||Continue|232||FALSE");
        stringList.Add("47|232|2|Despite this they are still quite worried. Maybe you can give some advice?|Provide reassurance|235|Justice|Provide empathy|233|Relationships|FALSE");
        stringList.Add("48|233|2|You know this is their first child after trying for 2 years.|Continue|234||Continue|234||FALSE");
        stringList.Add("49|234|2|But you cannot provide any reassurance, only to take things a step at a time.|Continue|236||Continue|236||FALSE");
        stringList.Add("50|235|2|They thank you for your reassurance. You know this is their first child after trying for 2 years.|Continue|236||Continue|236||FALSE");
        stringList.Add("51|236|2|Another week as passed and you don't see the family at NICU.|Continue|237||Continue|237||FALSE");
        stringList.Add("52|237|2|You later hear through the grape vines that their child had died due to infection.|Continue|301||Continue|301||FALSE");
        stringList.Add("53|301|3|It is around midnight when you get a ring from hospital staff.|Continue|302||Continue|302||FALSE");
        stringList.Add("54|302|3|'We currently suspect that Travis has an infection in the blood.'|Go to hospital immediately.|305|Health|Wait until morning|303|Patience|FALSE");
        stringList.Add("55|303|3|There's lots to organize especially with another boy at home. Best to leave in the morning.|Continue|304||Continue|304||FALSE");
        stringList.Add("56|304|3|You couldn't get a good sleep after that call, but you are ready to go to hospital now.|Continue|306||Continue|306||FALSE");
        stringList.Add("57|305|3|It's now 4am in the morning. Completely exhausted by the recent events you try to rest.|Continue|306||Continue|306||FALSE");
        stringList.Add("58|306|3|The medical team approaches you with a grim look. You see one of the nurses get quite emotional.|Continue|307||Continue|307||FALSE");
        stringList.Add("59|307|3|'Travis has a severe infection that is spreading in the blood.'|Continue|308||Continue|308||FALSE");
        stringList.Add("60|308|3|'There is a high chance that Travis will not survive.'|Continue|309||Continue|309||FALSE");
        stringList.Add("61|309|3|Feeling exhausted, this felt like insult to injury.|'No, he is going to live.'|310|Courage|Don't reply to their comment.|310|Health|FALSE");
        stringList.Add("62|310|3|The staff members back off, 'We will be here to support you if you need.'|Continue|311||Continue|311||FALSE");
        stringList.Add("63|311|3|Now the following day, Travis's vital signs had recovered a little from hours before.|Continue|312,313,314||Continue|312,313,314||TRUE");
        stringList.Add("64|312|3|You return home and get some time to reflect. Which do you agree with more?|'Need to go with the flow.'|400|Patience|'Can learn from positives and negatives.'|400|Selflessness|FALSE");
        stringList.Add("65|313|3|You return home and get some time to reflect. Which do you agree with more?|'Honesty is key.'|400|Trust|'Follow your gut.'|400|Selflessness|FALSE");
        stringList.Add("66|314|3|You return home and get some time to reflect. Which do you agree with more?|'Treat others how you want to be treated.'|400|Justice|'Don't be afraid to speak your mind.'|400|Courage|FALSE");
        stringList.Add("67|400|4|Five years later...|Already?!|401||Continue|401||FALSE");
        stringList.Add("68|401|4|Travis continues to use the NG tube for feeding. But it keeps coming out, which is very unpleasant.|Continue|402||Continue|402||FALSE");
        stringList.Add("69|402|4|A decision was made to use a PEG tube.|Continue|403||Continue|403||FALSE");
        stringList.Add("70|403|4|When you mention this to the GP and paediatrician, they are not too sure about this.|Continue|404||Continue|404||FALSE");
        stringList.Add("71|404|4|They suspect this is due to the reliance on the PEG tube and Travis is not used to eating orally.|Continue|405||Continue|405||FALSE");
        stringList.Add("72|405|4|They asked if you will be willing to include a SLT and psychologist.|Let's give it a go|408|Trust|Enquire more about it|406|-|FALSE");
        stringList.Add("73|406|4|It is good to eerie on side of caution with bringing people in.|Continue|407||Continue|407||FALSE");
        stringList.Add("74|407|4|However, the admit that this may require further expertise to help with.|Okay|408|Courage|Fine|408|Health|FALSE");
        stringList.Add("75|408|4|Now with 2 more professionals on the team, time seems a little more filled than before.|Continue|409||Continue|409||FALSE");
        stringList.Add("76|409|4|There is slow but sure improvement from month to month.|One can only hope|410|Health|I will trust in them|410|Trust|FALSE");
        stringList.Add("77|410|4|They are not certain that the oral aversion will be completely gone.|Continue|411||Continue|411||FALSE");
        stringList.Add("78|411|4|But, they will do their best in helping Travis|Any support is good|500,511,533|Trust|Stop if not helping|500,511,533|Justice|TRUE");
        stringList.Add("79|500|5|You recall one of the tests completed by the paediatrician called WISC-V|Continue|501||Continue|501||FALSE");
        stringList.Add("80|501|5|Travis had scored less than 0.1 percentile.|Continue|502||Continue|502||FALSE");
        stringList.Add("81|502|5|This means, compared to 1000 same-aged peers, 999 would score higher than Travis|Continue|503||Continue|503||FALSE");
        stringList.Add("82|503|5|It has now come time to choose a school for Travis. What values will guide your decision?|Go with the flow|504|Patience|Independence for Travis|504|Justice|FALSE");
        stringList.Add("83|504|5|You have narrowed it down to 2 schools.|Continue|505||Continue|505||FALSE");
        stringList.Add("84|505|5|Choose the attributes of the schools you would like.|Dependability|506|Relationships|Open-mindedness|506|Trust|FALSE");
        stringList.Add("85|506|5|Choose the attributes of the schools you would like.|Honesty|507|Justice|Teamwork|507|Relationships|FALSE");
        stringList.Add("86|507|5|Choose the attributes of the schools you would like.|Innovation|508|Courage|Passion|508|Selflessness|FALSE");
        stringList.Add("87|508|5|Choose the attributes of the schools you would like.|Respect|509|Relationships|Optimism|509|Courage|FALSE");
        stringList.Add("88|509|5|Choose the attributes of the schools you would like.|Service to others|510|Selflessness|Education|510|Justice|FALSE");
        stringList.Add("89|510|5|It was not easy to choose the school. But you are satisfied with your choice.|Continue|601||Continue|601||FALSE");
        stringList.Add("90|511|5|After school one day, Calvin comes home very excited. He has just been invited to a birthday party!|Continue|512||Continue|512||FALSE");
        stringList.Add("91|512|5|You are excited too. But, there is another issue you are considering.|Continue|513||Continue|513||FALSE");
        stringList.Add("92|513|5|Travis has been having chest infections very freuqnetly.|Continue|514||Continue|514||FALSE");
        stringList.Add("93|514|5|You are worried that this could increase the risk of more exacerbations of infections.|Continue|515||Continue|515||FALSE");
        stringList.Add("94|515|5|Calvin stares at you with excited eyes and asks you, 'Can I go!?'|Calvin should have some fun|516|Justice|Best not|522|Health|FALSE");
        stringList.Add("95|516|5|Almost exploding with joy, Calvin leaps into the air with his invitation.|Continue|517||Continue|517||FALSE");
        stringList.Add("96|517|5|After the party in the weekend, the only difference you notice with Calvin was a small cake stain on his cheek|'Where's my cake?'|518|Relationships|Wipe off the cake from his face|519|-|FALSE");
        stringList.Add("97|518|5|Calvin smiles and points at his tummy.|Continue|519||Continue|519||FALSE");
        stringList.Add("98|519|5|About 2 days later, Travis starts to have mild coughing fits.|Take Travis to the hospital|521|Courage|Just wait and see|520|Patience|FALSE");
        stringList.Add("99|520|5|A few hours after that, they are getting much worse and Travis is finding breathing more difficult.|Take Travis to the hospital|521||Take Travis to the hospital|521||FALSE");
        stringList.Add("100|521|5|You swiftly take Travis to the hospital where he was admitted and discharged a few days later.|Continue|601||Continue|601||FALSE");
        stringList.Add("101|522|5|The glimmer of hope in Calvin fades and you see him nod.|Try to reason with Calvin|523|Justice|Try do something creative|524|Selflessness|FALSE");
        stringList.Add("102|523|5|Calvin nods in understanding, but does not reply. It wasn't an easy decision for you to make either.|Continue|526||Continue|526||FALSE");
        stringList.Add("103|524|5|You tell Calvin, 'That is because we will be having a party at home!'|Continue|525|Selflessness|Continue|525|Selflessness|FALSE");
        stringList.Add("104|525|5|Still not back to full excitement, you know he was still looking forward to the other party.|Continue|526||Continue|526||FALSE");
        stringList.Add("105|526|5|A while later, Calvin wanted to pick up some social team sports like rugby.|Continue|527||Continue|527||FALSE");
        stringList.Add("106|527|5|All his friends are playing it! He wanted to give it a go himself too.|Best not either, too many people there|528|Health|Okay, it is only fair|531|Justice|FALSE");
        stringList.Add("107|528|5|Calvin starts to get very upset|Calm him down|530|Patience|Ground him to his room|529|-|FALSE");
        stringList.Add("108|529|5|It is not an easy decision for you either; unpleased, you tell him to cool down in his room.|Continue|601||Continue|601||FALSE");
        stringList.Add("109|530|5|You once again do your best to explain to Calvin why and explain that it is also not an easy decison for you.|Continue|601||Continue|601||FALSE");
        stringList.Add("110|531|5|On saturday morning, you take him over to the local park where many families have gathered.|Continue|532||Continue|532||FALSE");
        stringList.Add("111|532|5|Calvin is having a fun time on the field and you have a relaxing time talking with other parents.|Continue|519||Continue|519||FALSE");
        stringList.Add("112|533|5|At school, there is an up and coming 2 day camp. |Continue|534||Continue|534||FALSE");
        stringList.Add("113|534|5|You contemplate whether or not it would be safe for Travis to attend the camp.|Continue|535||Continue|535||FALSE");
        stringList.Add("114|535|5|You worry about chest infection and his ability to keep up with the rest of his peers.|Continue|536||Continue|536||FALSE");
        stringList.Add("115|536|5|Will you allow Travis to go to the school camp?|He should go|538|Courage|No, don't go|537|Health|FALSE");
        stringList.Add("116|537|5|Travis doesn't seem too phased with this decision.|Continue|601||Continue|601||FALSE");
        stringList.Add("117|538|5|You think that it is more important he not miss out on opportunities and it will build independence|Continue|539||Continue|539||FALSE");
        stringList.Add("118|539|5|Travis asks if you can come to camp with him.|Go with him to camp|540|Selflessness|I have other commitments at home|541|Relationships|FALSE");
        stringList.Add("119|540|5|You go along to camp with him and he seemed to enjoy various activities which were offered.|Continue|541||Continue|541||FALSE");
        stringList.Add("120|541|5|Upon returning home, he was slightly unsettled at the start, but did enjoy himself and built up some confidence|Continue|519||Continue|519||FALSE");
        stringList.Add("121|601|6|You feel, in order to support Travis and the teacher at school, a teacher aid would be helpful.|Continue|602||Continue|602||FALSE");
        stringList.Add("122|602|6|You are introduced to the new teacher aid which seems like a very nice person.|Continue|603||Continue|603||FALSE");
        stringList.Add("123|603|6|After a couple of days, Travis seems to be doing fine at school.|Continue|604||Continue|604||FALSE");
        stringList.Add("124|604|6|However, the teacher aid has growing concerns of Travis not orally feeding.|Explain the situation|605|Patience|Tell her not to worry|605|Trust|FALSE");
        stringList.Add("125|605|6|Even after explaining that there are professionals working with Travis to improve this. She does not buy it.|Remove her from the support network|607|Courage|It is not her expertise to worry about|606|Justice|FALSE");
        stringList.Add("126|606|6|She feels uncomfortable to provide further help and thus removes herself from support network.|Continue|701||Continue|701||FALSE");
        stringList.Add("127|607|6|You appreciate her help, but this is an ongoing journey and you feel everyone needs to be on the same page.|Continue|701||Continue|701||FALSE");
        stringList.Add("128|608|6|After a few more years, Travis is still mainly using his PEG tube to feed.|Continue|609||Continue|609||FALSE");
        stringList.Add("129|609|6|You have heard of a camp which may be able to help Travis to improve his oral feeding.|Continue|610||Continue|610||FALSE");
        stringList.Add("130|610|6|The only problem is that it is a few hours away by drive.|Let's try something new|614|Trust|It seems too good to be true|611|Patience|FALSE");
        stringList.Add("131|611|6|After discussing this idea with a couple of people, they all think that this may be a good opportunity.|Okay, let's give it a go|614|Health|I still don't want him to go|612|Patience|FALSE");
        stringList.Add("132|612|6|Despite pressure from other people, you decide against not sending him.|Continue|613||Continue|613||FALSE");
        stringList.Add("133|613|6|You wonder, if you had missed an opportunity or not.|Continue|701||Continue|701||FALSE");
        stringList.Add("134|614|6|You arrive to the camp with Travis and they inform you  that they will take care of the rest.|Continue|615||Continue|615||FALSE");
        stringList.Add("135|615|6|A week later you pop back to check in on him. They tell you they have lost him in the morning!|Go find Travis|617|Selflessness|Call the police|616|Justice|FALSE");
        stringList.Add("136|616|6|You immediately dial 111 and register to find a missing person.|Continue|617||Continue|617||FALSE");
        stringList.Add("137|617|6|You move into your car and begin searching around the premises to see if you can find Travis.|Continue|618||Continue|618||FALSE");
        stringList.Add("138|618|6|Off in the distance you see a child with a slim body and you promptly stop the car. It is Travis.|Go check up on him|620|Health|Give him a hug|619|Relationships|FALSE");
        stringList.Add("139|619|6|You hold him tightly and tell him that this will not happen again|Continue|621||Continue|621||FALSE");
        stringList.Add("140|620|6|To your amazement, Travis is in one piece. |Continue|621||Continue|621||FALSE");
        stringList.Add("141|621|6|You take him home. You cannot even imagine what had happen for him to run off.|Continue|622||Continue|622||FALSE");
        stringList.Add("142|622|6|As well, tt seems that the camp had only made his want to orally feed worse.|Continue|701||Continue|701||FALSE");
        stringList.Add("143|701|7|A few more years later Amber is born and makes a lovely addition to the family.|Continue|702||Continue|702||FALSE");
        stringList.Add("144|702|7|The kids are growing up to be very close to each other.|Continue|703||Continue|703||FALSE");
        stringList.Add("145|703|7|After many various attemps and ideas working with the SLT and psychologist.|Continue|704||Continue|704||FALSE");
        stringList.Add("146|704|7|Travis finally was able to feed orally and have his PEG tube removed in 2016.|Continue|705||Continue|705||FALSE");
        stringList.Add("147|705|7|In summary,|Continue|706||Continue|706||FALSE");
        stringList.Add("148|706|7|Sharon is still the showrunner of the house.|Continue|707||Continue|707||FALSE");
        stringList.Add("149|707|7|Andrew feels lucky he has had an understanding work environment (and loves his Toyota very much).|Continue|708||Continue|708||FALSE");
        stringList.Add("150|708|7|Calvin has been working and enjoys pranking each other.|Continue|709||Continue|709||FALSE");
        stringList.Add("151|709|7|Amber has been doing her homework with Travis and enjoying Roblox together!|Continue|710||Continue|710||FALSE");
        stringList.Add("152|710|7|Travis has been enjoying his school life and is growing through his social experiences.|Ending|800||Ending|800||FALSE");
        stringList.Add("153|800|8|End|End|0||End|0||FALSE");

        //putting input_line -> cardList[,]
        for (int row = 0; row < stringList.Count; row++)
        {
            string[] string_columns = stringList[row].Split('|'); //This is char?
            for (int col = 0; col < string_columns.Length; col++)
            {
                cardList[row, col] = string_columns[col];
            }
        }

        stringList.Clear();
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
        //Wallpaper movement
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        wallpaperGameObject.transform.position = new Vector2(mousePosition.x * fWallpaperMoveSpeed, mousePosition.y * fWallpaperMoveSpeed + 1.33f);

        //Movement
        if (Input.GetMouseButton(0) && mainCardController.isMouseOver && !isSubstituting)
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
            cardGameObject.transform.eulerAngles = new Vector3(0, 0, cardGameObject.transform.position.x * fSwivleSpeed);
       }
       else if (isSubstituting)
        {
            cardGameObject.transform.position = new Vector3(0, 0, 0);
            //begin rotating back of card and front card
            cardGameObject.transform.eulerAngles = Vector3.MoveTowards(cardGameObject.transform.rotation.eulerAngles, cardRotation, fRotatingSpeed);

            //Debug.Log("Rotation = " + cardGameObject.transform.eulerAngles.y);
            if (cardGameObject.transform.eulerAngles.y < 90f)
            {
                //show text
                mainText.faceColor = new Color32(mainText.faceColor.r, mainText.faceColor.g, mainText.faceColor.b, 255);
                //Debug.Log("Show text");
            }
            else //hide text
            {
                mainText.faceColor = new Color32(mainText.faceColor.r, mainText.faceColor.g, mainText.faceColor.b, 0);
                //Debug.Log("Hidden text");
            }
            if (!playedSound)
            {
                playedSound = true;
                audioSource.volume = 0.25f;
                audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                audioSource.Play();
                //Debug.Log(audioSource.pitch);
            }

        }
        else //Only reset if not substituting
        {
            cardGameObject.transform.position = Vector2.MoveTowards(cardGameObject.transform.position, new Vector2(0, 0), fMovingSpeed);
            cardGameObject.transform.eulerAngles = new Vector3(0, 0, cardGameObject.transform.position.x * fSwivleSpeed);
        }
        //display.text = "x = " + cardGameObject.transform.position.x.ToString() + "; y = " + cardGameObject.transform.position.y.ToString(); //For debug


        if (cardGameObject.transform.eulerAngles == cardRotation)
        {
            isSubstituting = false;
            playedSound = false;
            //Debug.Log("Finished substituting");
        }
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
            currentActiveCardRow = 800;
            //ENDING trigger
            //Debug.Log("Calculate score...");

            //string[] testValues = { "bird", "cat", "bird", "dog", "bird", "man", "frog", "cat" };
            //valuesList = new List<string>(testValues);

            string[] Top3Values = new string[3];
            Top3Values = CalculateTop3Values(valuesList);

            //Debug.Log("Top values: " + Top3Values[0] + ", " + Top3Values[1] + ", " + Top3Values[2]);

            mainText.text = "Congratulations! You have completed the game. Your top values were: " + Top3Values[0] + ", " + Top3Values[1] + " and " + Top3Values[2];
            upText.text = "Back to Main Menu";
            downText.text = "Play again";
        }
        else if (cardID == 1000) // Main menu
        {
            currentActiveCardRow = 1000;
            titleText.text = "Core";
            mainText.text = "Swipe <- to start\n\nSwipe -> Credits";
            upText.text = "Let's go!";
            downText.text = "Thank you!";
        }
        else if (cardID == 1001) //Options menu
        {
            currentActiveCardRow = 1001;
            titleText.text = "Credits";
            mainText.text = "Thank you to the Sydney family!\n\nMade by\nChris Wang";
            upText.text = "<- Back";
            downText.text = "Back ->";
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

        //Debug.Log("Card has finished loading without quiting the function.");

        //reset position of card

        //intialize rotating card
        isSubstituting = true;
        cardGameObject.transform.eulerAngles = initialRotation;
    }

    private string[] CalculateTop3Values(List<string> valuesList)
    {
        var result = new Dictionary<string, int>();

        foreach (string value in valuesList)
        {
            if (result.TryGetValue(value, out int count))
            {
                // Increase existing value.
                result[value] = count + 1;
            }
            else
            {
                // New value, set to 1.
                result.Add(value, 1);
            }
        }

        //var sorted = from pair in result
        //             orderby pair.Value descending
        //             select pair;

        //LINQ
        var top3 = result.OrderByDescending(pair => pair.Value).Take(3);

        List<string> listStrTop3 = new List<string>();

        // Display all results in order.
        foreach (var pair in top3)
        {
            listStrTop3.Add(pair.Key);
        }

        //returns string[3]
        return listStrTop3.ToArray();
        
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

        //Debug.Log("Card: " + currentCard.cardName + "has finished loading.");

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
        ////particle system
        //if (choice == ChoiceDirection.left)
        //{
        //    particleGameObject.transform.position = new Vector2(-1.5f, 0f);
        //    particles.Play();
        //}
        //else
        //{
        //    particleGameObject.transform.position = new Vector2(1.5f, 0f);
        //    particles.Play();
        //}
        

        //Debug.Log("CardID : " + currentActiveCardRow + " swiped " + choice.ToString());
        if (currentActiveCardRow == 800) //ending
        {
            if (choice == ChoiceDirection.left) //left Swipe
            {
                //Return to main menu
                ResetGameVariables();
                currentActiveCardRow = 1000;
                LoadCardFromArray(1000);
            }
            else
            {
                //Start again
                ResetGameVariables();
                currentActiveCardRow = 101; //Skip the tutorial
                LoadCardFromArray(currentActiveCardRow);
            }
        }
        else if (currentActiveCardRow == 1000)
        {
            if (choice == ChoiceDirection.left) //left Swipe
            {
                //Start game
                LoadCardFromArray(0);
            }
            else
            {
                //Options
                LoadCardFromArray(1001);
            }
        }
        else if (currentActiveCardRow == 1001)
        {
            LoadCardFromArray(1000);
        }
        else
        {
            if (choice == ChoiceDirection.left) //left Swipe
            {
                //Give specific Left value points
                if (cardList[currentActiveCardRow, (int)columnHeading.LeftValue] == "")
                {
                    //do nothing if there is nothing here
                }
                else
                {
                    valuesList.Add(cardList[currentActiveCardRow, (int)columnHeading.LeftValue]);

                    //Debug.Log("valuesList = " + string.Join("", new List<string>(valuesList).ConvertAll(i => i.ToString()).ToArray()));
                }

                //Check if the next card needs to be randomized
                if (cardList[currentActiveCardRow, (int)columnHeading.LeftNextCardID] == "")
                {
                    //do nothing e.g. in tutorial, we don't want to reload (flip) the same card 
                }
                else if (cardList[currentActiveCardRow, (int)columnHeading.Randomize] == "TRUE")
                {
                    //randomize
                    string[] randomizedCol = cardList[currentActiveCardRow, (int)columnHeading.LeftNextCardID].Split(','); //This is char?
                                                                                                                           //Debug.Log("Random Choice # of Paths:" + randomizedCol.Length);
                    LoadCardFromArray(int.Parse(randomizedCol[UnityEngine.Random.Range(0, 2)]));
                }
                else //load card Left choice if NOT randomized
                {
                    LoadCardFromArray(int.Parse(cardList[currentActiveCardRow, (int)columnHeading.LeftNextCardID]));
                }
            }
            else //Right Swipe
            {
                //Give specific Right value points 
                if (cardList[currentActiveCardRow, (int)columnHeading.RightValue] == "")
                {
                    //do nothing if there is nothing here
                }
                else
                {
                    valuesList.Add(cardList[currentActiveCardRow, (int)columnHeading.RightValue]);

                    //Debug.Log("valuesList = " + string.Join("", new List<string>(valuesList).ConvertAll(i => i.ToString()).ToArray()));
                }

                //Check if the next card needs to be randomized
                if (cardList[currentActiveCardRow, (int)columnHeading.RightNextCardID] == "")
                {
                    //do nothing e.g. in tutorial, we don't want to reload (flip) the same card 
                }
                else if (cardList[currentActiveCardRow, (int)columnHeading.Randomize] == "TRUE")
                {
                    //randomize
                    string[] randomizedCol = cardList[currentActiveCardRow, (int)columnHeading.RightNextCardID].Split(','); //This is char?
                                                                                                                            //Debug.Log("Random Choice # of Paths:" + randomizedCol.Length);
                    LoadCardFromArray(int.Parse(randomizedCol[UnityEngine.Random.Range(0, 2)]));
                }
                else //load card Right choice if NOT randomized
                {
                    LoadCardFromArray(int.Parse(cardList[currentActiveCardRow, (int)columnHeading.RightNextCardID]));
                }
            }
        }
    }

    private void ResetGameVariables()
    {
        valuesList = new List<string>();
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

