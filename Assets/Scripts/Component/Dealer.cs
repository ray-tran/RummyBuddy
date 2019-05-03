using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class Dealer : MonoBehaviour 
{
    public static Dealer instance;

    public CardDeck _cardDeck;
    public CardSlot _pickupCardSlot;
    public CardSlot _centerStackCardSlot;	
	public CardSlot _discardStackCardSlot;
    public CardSlot _drawStackCardSlot; //Need to flip this top card. transfer to discard pile - _discardStackCardSlot 
    public CardSlot _rightHandCardSlot;
    public CardSlot _leftHandCardSlot;
    public CardSlot _currentCardSlot;
    
    public const float CardStackDelay = .01f;
	
	/// Counter which keeps track current dealing movements in progress.
	public int DealInProgress { get; set; }

	private void Awake()
	{
        instance = this;
		_cardDeck.InstanatiateDeck("cards");

        StartCoroutine(CardDropShuffleDeal(0, _cardDeck.CardList.Count, _centerStackCardSlot));
	}
    
    //Move all cards in a slot to another slot
    private void MoveCardSlotToCardSlot(CardSlot sourceCardSlot, CardSlot targetCardSlot) 
	{
		Card card;
		while ((card = sourceCardSlot.TopCard()) != null)    
		{
			targetCardSlot.AddCard(card);
		}
	}
	
    //Beginning animation where everycard falls from the sky to the center stack
    //Drops, Shuffles and Deals the cards.
	private IEnumerator CardDropShuffleDeal(int start, int end, CardSlot cardSlot) 
	{

        DealInProgress++;

        for (int i = start; i < end; ++i)
		{
			cardSlot.AddCard(_cardDeck.CardList[i]);
            

            yield return new WaitForSeconds(CardStackDelay);
		}
        DealInProgress--;

        PlayerHand.instance.CardSlotList[9].TargetTransform.position = new Vector3(PlayerHand.instance.CardSlotList[9].TargetTransform.position.x, 0.098f, PlayerHand.instance.CardSlotList[9].TargetTransform.position.z);
        AIHand.instance.CardSlotList[4].TargetTransform.position = new Vector3(AIHand.instance.CardSlotList[4].TargetTransform.position.x, 0.035f, AIHand.instance.CardSlotList[4].TargetTransform.position.z);


        StartCoroutine(ShuffleCoroutine());        
	}

    /// Shuffle Coroutine.
    /// Moves all card to pickupCardSlot. Then shuffles them back
    /// to cardStackSlot.
    public IEnumerator ShuffleCoroutine()
	{
        DealInProgress++;
        foreach (CardSlot slot in AIHand.instance.CardSlotList)
        {
            MoveCardSlotToCardSlot(slot, _centerStackCardSlot);
        }

        foreach (CardSlot slot in PlayerHand.instance.CardSlotList)
        {
            MoveCardSlotToCardSlot(slot, _centerStackCardSlot);
        }

        MoveCardSlotToCardSlot(_discardStackCardSlot, _centerStackCardSlot);
        MoveCardSlotToCardSlot(_drawStackCardSlot, _centerStackCardSlot);
        MoveCardSlotToCardSlot(_currentCardSlot, _centerStackCardSlot);
        yield return new WaitForSeconds(.5f);
        MoveCardSlotToCardSlot(_centerStackCardSlot, _pickupCardSlot);
        yield return new WaitForSeconds(.5f);	
		int halfLength = _cardDeck.CardList.Count / 2;
		for (int i = 0; i < halfLength; ++i)
		{
			_leftHandCardSlot.AddCard(_pickupCardSlot.TopCard());
		}
		yield return new WaitForSeconds(.2f);	
		for (int i = 0; i < halfLength; ++i)
		{
			_rightHandCardSlot.AddCard(_pickupCardSlot.TopCard());
		}
		yield return new WaitForSeconds(.2f);	
		for (int i = 0; i < _cardDeck.CardList.Count; ++i)
		{
			if (i % 2 == 0)
			{
				_centerStackCardSlot.AddCard(_rightHandCardSlot.TopCard());
			}
			else
			{
				_centerStackCardSlot.AddCard(_leftHandCardSlot.TopCard());
			}
			yield return new WaitForSeconds(CardStackDelay);
		}
        yield return new WaitForSeconds(.3f);

        StartCoroutine(DealCards());
    }

    public IEnumerator DealCards()
    {
        System.Random random = new System.Random();

        for (int i = 0; i < 10; i++)
        {
            int randomIndex = random.Next(0, _centerStackCardSlot.GetSize());
            _centerStackCardSlot.AddCard(_centerStackCardSlot.GetCard(randomIndex));
            PlayerHand.instance.CardSlotList[i].AddCard(_centerStackCardSlot.TopCard());
            yield return new WaitForSeconds(.05f);
            randomIndex = random.Next(0, _centerStackCardSlot.GetSize());
            _centerStackCardSlot.AddCard(_centerStackCardSlot.GetCard(randomIndex));
            AIHand.instance.CardSlotList[i].AddCard(_centerStackCardSlot.TopCard());
            yield return new WaitForSeconds(.05f);
        }

        MoveCardSlotToCardSlot(_centerStackCardSlot, _drawStackCardSlot);
        yield return new WaitForSeconds(.3f);

        //Do a card flip after shuffle and dealing cards
        Card _cardTopFlip = _drawStackCardSlot.TopCard();
        _discardStackCardSlot.AddCard(_cardTopFlip);

        if (Match.instance.RoundCount == 0)
        {
            Match.instance.InitializeMatch();
        }
        else
        {
            Round.instance.InitializeRound();
        }

        DealInProgress--;
    }
}