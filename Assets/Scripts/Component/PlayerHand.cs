using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHand : MonoBehaviour
{
    public bool silent = false;
    public int MatrixValue;
    public static PlayerHand instance;
    protected int InstanceType; //0: PlayerHand, 1: AIHand

    public List<Card> Deadwoods;
    public int DeadwoodPoints;

    public List<List<Card>> AllMelds;
    public List<List<Card>> OptimalMelds;

    public List<Card> SpadesList;
    public List<Card> ClubsList;
    public List<Card> DiamondsList;
    public List<Card> HeartsList;

    public List<Card> CardsInHand;

    public CardSlot CardSlot0;
    public CardSlot CardSlot1;
    public CardSlot CardSlot2;
    public CardSlot CardSlot3;
    public CardSlot CardSlot4;
    public CardSlot CardSlot5;
    public CardSlot CardSlot6;
    public CardSlot CardSlot7;
    public CardSlot CardSlot8;
    public CardSlot CardSlot9;
    public CardSlot CardSlot10;

    public readonly List<CardSlot> CardSlotList = new List<CardSlot>();

    public void Awake()
    {
        MatrixValue = 0;
        instance = this;
        InstanceType = 0;
        EmptyHand();
        CardSlotList.Add(CardSlot0);
        CardSlotList.Add(CardSlot1);
        CardSlotList.Add(CardSlot2);
        CardSlotList.Add(CardSlot3);
        CardSlotList.Add(CardSlot4);
        CardSlotList.Add(CardSlot5);
        CardSlotList.Add(CardSlot6);
        CardSlotList.Add(CardSlot7);
        CardSlotList.Add(CardSlot8);
        CardSlotList.Add(CardSlot9);
        CardSlotList.Add(CardSlot10);
        if (!silent)
        {
            InitializeCardSlots();
        }
        //Debug.Log("Check y :" + CardSlotList[9].TargetTransform.position.y);
    }

    public void EmptyHand()
    {
        CardsInHand = new List<Card>();
        SpadesList = new List<Card>();
        ClubsList = new List<Card>();
        DiamondsList = new List<Card>();
        HeartsList = new List<Card>();
        AllMelds = new List<List<Card>>();
        OptimalMelds = new List<List<Card>>();
        Deadwoods = new List<Card>();
    }

    //Called at start of round
    public void InitializeHand()
    {
        EmptyHand();
        foreach (CardSlot slot in CardSlotList)
        {
            if (slot.TopCard() != null)
            {
                Card card = slot.TopCard();
                CardsInHand.Add(card);
                AddToSuitList(card);
            }
        }
        ScanHand(true);
    }

    //Scan the hand to determine what are deadwoods, runs, sets and other data
    public void ScanHand(bool UI)
    {
        AllMelds = new List<List<Card>>();
        OptimalMelds = new List<List<Card>>();
        Deadwoods = new List<Card>();
        SortCardList(CardsInHand);

        ScanForSets();
        ScanForRuns(SpadesList);
        ScanForRuns(ClubsList);
        ScanForRuns(DiamondsList);
        ScanForRuns(HeartsList);

        DecideOptimalMelds();

        CalculateDeadwoods();

        if (UI)
        {
            SortHandUI();
            GameUI.instance.UpdateDWUI();
        }
    }

    //Function that scan the hand and put any possible sets
    //(3+ of the same rank) into AllMelds list
    private void ScanForSets()
    {
        List<Card> set = new List<Card>();
        int curSetRank = CardsInHand[0].Rank;
        int curSetCount = 1;
        int j = 0;
        int i = 1;
        for (; i< CardsInHand.Count; i++)
        {
            if (CardsInHand[i].Rank == curSetRank)
            {
                curSetCount++;
            }
            else
            {
                if (curSetCount == 3)
                {
                    while (j < i)
                    {
                        set.Add(CardsInHand[j]);
                        j++;
                    }
                    AllMelds.Add(set);
                    set = new List<Card>();
                }
                else if (curSetCount == 4)
                {
                    AddAllSetsFromFourCards(j);
                    //add all combination of 3 cards, and 4
                }
                curSetCount = 1;
                curSetRank = CardsInHand[i].Rank;
                j = i;
            }
        }
        if (curSetCount == 3)
        {
            while (j < i)
            {
                set.Add(CardsInHand[j]);
                j++;
            }
            AllMelds.Add(set);
            set = new List<Card>();
        }
        else if (curSetCount == 4)
        {
            //add all combination of 3 cards, and 4
            AddAllSetsFromFourCards(j);
        }
    }

    private void AddAllSetsFromFourCards(int j)
    {
        List<Card> set = new List<Card>();
        set.Add(CardsInHand[j]); set.Add(CardsInHand[j+1]); set.Add(CardsInHand[j+2]);
        AllMelds.Add(set);

        set = new List<Card>();
        set.Add(CardsInHand[j]); set.Add(CardsInHand[j+1]); set.Add(CardsInHand[j+3]);
        AllMelds.Add(set);

        set = new List<Card>();
        set.Add(CardsInHand[j]); set.Add(CardsInHand[j+2]); set.Add(CardsInHand[j+3]);
        AllMelds.Add(set);

        set = new List<Card>();
        set.Add(CardsInHand[j+1]); set.Add(CardsInHand[j+2]); set.Add(CardsInHand[j+3]);
        AllMelds.Add(set);

        set = new List<Card>();
        set.Add(CardsInHand[j]); set.Add(CardsInHand[j+1]); set.Add(CardsInHand[j+2]); set.Add(CardsInHand[j+3]);
        AllMelds.Add(set);
    }

    //Function that scan the hand and put any possible runs
    //(sequence of 3+ card of same suit) into AllMelds list
    private void ScanForRuns(List<Card> suitList)
    {
        //No need to scan if the suit list only contains 2 cards (have to have 3 minimum for a run)
        if (suitList.Count > 2)
        {
            List<Card> run = new List<Card>();
            int prevCardRank = suitList[0].Rank;
            int curRunCount = 1;
            int j = 0;
            int i = 1;
            for (; i < suitList.Count; i++)
            {
                if (suitList[i].Rank == prevCardRank + 1)
                {
                    curRunCount++;
                    prevCardRank = suitList[i].Rank;
                }
                else
                {
                    if (curRunCount > 2)
                    {

                        AddRunsToAllMelds(suitList, j, i);
                    }
                    curRunCount = 1;
                    prevCardRank = suitList[i].Rank;
                    j = i;
                }
            }
            if (curRunCount > 2)
            {
                AddRunsToAllMelds(suitList, j, i);
            }
        }
    }

    //From j to i-1
    private void AddRunsToAllMelds(List<Card> suitList, int j, int i)
    {
        List<List<Card>> allNewRuns = new List<List<Card>>();
        List<Card> run = new List<Card>();

        for (int index = j; index < j + 3; index++)
        {
            run.Add(suitList[index]);
        }
        allNewRuns.Add(run);
        if (i - j > 3)
        {
            for (int index = j + 3; index < i; index++)
            {
                int runsCountToCopy = index - j - 2;
                List<List<Card>> temp = new List<List<Card>>();
                temp.AddRange(allNewRuns.GetRange(allNewRuns.Count-runsCountToCopy, runsCountToCopy));
                foreach (List<Card> meld in temp)
                {
                    List<Card> newMeld = MeldListCopy(meld);
                    newMeld.Add(suitList[index]);
                    allNewRuns.Add(newMeld);
                }
                run = new List<Card>();
                for (int start = index -2 ; start <= index; start++)
                {
                    run.Add(suitList[start]);
                }

                allNewRuns.Add(run);
            }
        }
        AllMelds.AddRange(allNewRuns);
    }

    private List<Card> MeldListCopy(List<Card> meld)
    {
        List<Card> copiedMeld = new List<Card>();
        foreach (Card c in meld)
        {
            copiedMeld.Add(c);
        }
        return copiedMeld;
    }

    //Function to use the melds in AllMelds and decide the optimal subset of AllMelds:
    //meaning no overlap in cards (each card appear only once)
    //and have the highest total face value (consequentially lowest total deadwood value)
    private void DecideOptimalMelds()
    {
        Dictionary<Card, int> dict = new Dictionary<Card, int>();
        foreach (List<Card> meld in AllMelds)
        {
            foreach (Card c in meld)
            {
                if (!dict.ContainsKey(c))
                {
                    dict.Add(c, 1);
                }
                else
                {
                    dict[c] += 1;
                }
            }

        }

        int i = 0;
        while (i < AllMelds.Count)
        {
            bool noOverlap = true;
            foreach(Card c in AllMelds[i])
            {
                if (dict[c] > 1)
                {
                    noOverlap = false;
                    break;
                }
            }

            if (noOverlap)
            {
                OptimalMelds.Add(AllMelds[i]);
                AllMelds.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        //At this point, AllMelds only contains melds with overlap

        //Debug.Log("Overlapped Melds");
        //Log2DList(AllMelds);

        //Debug.Log("Unique Melds");
        //Log2DList(OptimalMelds);

        GetMaxSetOfPowerSet();
    }

    //Return the set of melds that have the highest value
    private void GetMaxSetOfPowerSet()
    {
        List<List<List<Card>>> powerset = new List<List<List<Card>>>();
        MaxSetOfPowerSetHelper(0, powerset);
        //Debug.Log("Powerset");
        //Log3DList(powerset);

        int maxVal = 0;
        List<List<Card>> curOptimalSet = new List<List<Card>>();
        foreach (List<List<Card>> setOfMelds in powerset)
        {
            int curSetVal = GetSetOfMeldsFaceValue(setOfMelds);
            if (curSetVal > maxVal)
            {
                maxVal = curSetVal;
                curOptimalSet = setOfMelds;
            }
        }

        OptimalMelds.AddRange(curOptimalSet);

        OptimalMelds.Sort((a, b) =>
        {
            int firstCompare = a[0].Rank.CompareTo(b[0].Rank);
            return firstCompare != 0 ? firstCompare : a[0].CardSuit.CompareTo(b[0].CardSuit);
        });

    }

    private void MaxSetOfPowerSetHelper(int index, List<List<List<Card>>> powerset)
    {
        //Base case
        //Add empty
        if (index == AllMelds.Count)
        {
            List<List<Card>> empty = new List<List<Card>>();
            powerset.Add(empty);
            return;
        }

        //Build
        MaxSetOfPowerSetHelper(index + 1, powerset);
        List<Card> currentMeld = AllMelds[index];
        List<List<List<Card>>> moreSets = new List<List<List<Card>>>();
        foreach (List<List<Card>> inner in powerset)
        {
            if (!HasOverlap(inner, currentMeld))
            {
                List<List<Card>> temp = new List<List<Card>>();
                temp.AddRange(inner);
                temp.Add(currentMeld);
                moreSets.Add(temp);
            }
        }
        powerset.AddRange(moreSets);
    }

    private int GetSetOfMeldsFaceValue(List<List<Card>> setOfMelds)
    {
        int totalFaceValue = 0;
        foreach (List<Card> meld in setOfMelds)
        {
            totalFaceValue += GetFaceValueOfList(meld);
        }
        return totalFaceValue;
    }

    private int GetFaceValueOfList(List<Card> list)
    {
        int totalFaceValue = 0;
        foreach (Card c in list)
        {
            totalFaceValue += c.FaceValue;
        }
        return totalFaceValue;
    }

    //Return whether listOfMelds has a card that is in meld
    private bool HasOverlap(List<List<Card>> listOfMelds, List<Card> meld)
    {
        HashSet<Card> set = new HashSet<Card>();
        foreach (Card c in meld)
        {
            set.Add(c);
        }

        foreach(List<Card> currentMeld in listOfMelds)
        {
            foreach(Card c in currentMeld)
            {
                if (set.Contains(c))
                    return true;
            }
        }
        return false;
    }

    //CardsInHand - OptimalMelds = Deadwoods
    private void CalculateDeadwoods()
    {
        Deadwoods.AddRange(CardsInHand);
        foreach(List<Card> meld in OptimalMelds)
        {
            foreach (Card c in meld)
            {
                Deadwoods.Remove(c);
            }
        }
        SortCardList(Deadwoods);
        DeadwoodPoints = GetFaceValueOfList(Deadwoods);
    }

    private void SortCardList(List<Card> cardList)
    {
        cardList.Sort((a, b) =>
        {
            int firstCompare = a.Rank.CompareTo(b.Rank);
            return firstCompare != 0 ? firstCompare : a.CardSuit.CompareTo(b.CardSuit);
        });
    }

    //Function to display the hand in the UI according to the order of CardsInHand list
    private void SortHandUI()
    {
        int index = 0;
        FitCardSlots();

        //Melds on the left
        foreach (List<Card> meld in OptimalMelds)
        {
            foreach(Card c in meld)
            {
                if (InstanceType == 0)
                {
                    c.gameObject.transform.Find("Glow").gameObject.SetActive(true);
                    c.gameObject.GetComponent<Card>().TurnDark();
                }
                CardSlotList[index].AddCard(c);
                index++;
            }
        }

        //Deadwoods on the right
        foreach(Card c in Deadwoods)
        {
            c.gameObject.transform.Find("Glow").gameObject.SetActive(false);
            CardSlotList[index].AddCard(c);
            index++;
        }
    }

    protected void InitializeCardSlots()
    {
        float y = CardSlotList[0].TargetTransform.position.y;
        float gap;
        if (InstanceType == 0)
            gap = .005f;
        else
            gap = 0.003f;
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            y += gap;
            CardSlotList[i].TargetTransform.position = new Vector3(CardSlotList[i].TargetTransform.position.x, y, CardSlotList[i].TargetTransform.position.z);
        }

    }

    private void FitCardSlots()
    {

        int slotListLen = CardSlotList.Count;
        float leftPoint = CardSlotList[0].transform.position.x;
        float rightPoint = CardSlotList[slotListLen - 1].transform.position.x;

        float delta = rightPoint - leftPoint;

        int lastIndex = CardsInHand.Count - 1;

        int gaps = lastIndex;

        float gapFromOneItemToTheNextOne = delta / gaps;

        for (int i = 0; i <= lastIndex; i++)
        {

            CardSlotList[i].TargetTransform.position = new Vector3(leftPoint + (i * gapFromOneItemToTheNextOne), CardSlotList[i].TargetTransform.position.y, CardSlotList[i].TargetTransform.position.z);

        }
    }

    public void DrawCard(Card newCard, bool UI)
    {
        if (!silent)
        {
            //If player draws from discard pile, then we can record this info in the matrix
            if (MatrixValue == 0 && newCard.ParentCardSlot.name.IndexOf("DiscardStackSlot", System.StringComparison.CurrentCulture) != -1)
                GameObject.Find("AIHand").GetComponent<AIHand>().PutCardInGameState(GameObject.Find("AIHand").GetComponent<AIHand>().KnownGameState, newCard, MatrixValue);
            else if (MatrixValue == 1)
                GameObject.Find("AIHand").GetComponent<AIHand>().PutCardInGameState(GameObject.Find("AIHand").GetComponent<AIHand>().KnownGameState, newCard, MatrixValue);
        }

        CardsInHand.Add(newCard);
        AddToSuitList(newCard);

        ScanHand(UI);

        if (UI && Round.instance.CurrentTurn == Turn.PlayerDraw)
        {
            Round.instance.UpdateTurn(Turn.PlayerDiscard);
            int legalEndRoundMove = CheckLegalEndRoundMove();
            if (legalEndRoundMove != -1)
            {
                GameUI.instance.DisplayEndRoundMoveButton(legalEndRoundMove);
            }
        }
    }

    //bool endRound: pass in true only if this is the last discard automatic
    //that happens after calling gin or knock
    public void DiscardCard(Card card, bool UI, bool endRound)
    {
        if (UI)
        {
            Round.instance.DiscardPile.AddCard(card);
            card.gameObject.transform.Find("Glow").gameObject.SetActive(false);
        }

        if (!silent)
            GameObject.Find("AIHand").GetComponent<AIHand>().PutCardInGameState(GameObject.Find("AIHand").GetComponent<AIHand>().KnownGameState, card, 2);
        CardsInHand.Remove(card);
        Deadwoods.Remove(card);
        SpadesList.Remove(card);
        ClubsList.Remove(card);
        DiamondsList.Remove(card);
        HeartsList.Remove(card);
        foreach (List<Card> l in AllMelds)
        {
            l.Remove(card);
        }
        foreach (List<Card> l in OptimalMelds)
        {
            l.Remove(card);
        }

        ScanHand(UI);

        if (UI && !endRound)
        {
            if (Round.instance.CurrentTurn == Turn.PlayerDiscard)
            {
                GameUI.instance.DisableEndRoundMoveButton();
                Round.instance.UpdateTurn(Turn.AIDraw);
                GameObject.Find("AIHand").GetComponent<AIHand>().AIExecuteTurn();

                //AIHand.instance.AIExecuteTurn();
            }
            else
            {
                Round.instance.UpdateTurn(Turn.PlayerDraw);
            }
        }
    }

    private void AddToSuitList(Card card)
    {
        switch (card.CardSuit)
        {
            case Card.Suit.Clubs:
                ClubsList.Add(card);
                SortCardList(ClubsList);
                break;
            case Card.Suit.Diamonds:
                DiamondsList.Add(card);
                SortCardList(DiamondsList);
                break;
            case Card.Suit.Hearts:
                HeartsList.Add(card);
                SortCardList(HeartsList);
                break;
            case Card.Suit.Spades:
                SpadesList.Add(card);
                SortCardList(SpadesList);
                break;
        }

    }

    //End round call type:
    //0: knock
    //1: gin
    //2: big gin
    //-1: no legal move
    public int CheckLegalEndRoundMove()
    {
        //BIG GIN legal
        if (Deadwoods.Count == 0)
        {
            return 2;
        }

        //GIN leagl
        else if (Deadwoods.Count == 1)
        {
            return 1;
        }

        //KNOCK legal
        else if ((DeadwoodPoints - Deadwoods[Deadwoods.Count-1].FaceValue) < 10)
        {
            return 0;
        }

        return -1;
    }

    private void DisplayLegalEndRoundButton()
    {

    }


    public void Gin()
    {
        DiscardCard(Deadwoods[0], true, true);
        Round.instance.CalculateAndUpdateScore(1, InstanceType);
    }

    public void BigGin()
    {
        Round.instance.CalculateAndUpdateScore(2, InstanceType);
    }

    public void Knock()
    {
        DiscardCard(Deadwoods[Deadwoods.Count - 1], true, true);
        Round.instance.CalculateAndUpdateScore(0, InstanceType);
    }

    private void Log3DList(List<List<List<Card>>> list)
    {
        foreach (List<List<Card>> inner in list)
        {
            Log2DList(inner);
            Debug.Log("---");
        }
    }

    private void Log2DList(List<List<Card>> list)
    {
        foreach (List<Card> inner in list)
        {
            Log1DList(inner);
        }
    }
    private void Log1DList(List<Card> list)
    {
        string line = "";
        foreach (Card card in list)
        {
            line += card.name + " ";
        }
        Debug.Log(line);

    }

}
