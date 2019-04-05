using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class Dealer : MonoBehaviour 
{
    public static Dealer instace;
    
	[SerializeField]
	private CardDeck _cardDeck;

    [SerializeField]
	private CardSlot _pickupCardSlot;		

	[SerializeField]
	private CardSlot _centerStackCardSlot;	

	[SerializeField]
	public CardSlot _discardStackCardSlot;

    [SerializeField]
    public CardSlot _drawStackCardSlot;

	[SerializeField]
	private CardSlot _rightHandCardSlot;

	[SerializeField]
	private CardSlot _leftHandCardSlot;

	[SerializeField]
	private CardSlot _currentCardSlot;	

	private const float CardStackDelay = .01f;
	
	/// <summary>
	/// Counter which keeps track current dealing movements in progress.
	/// </summary>
	public int DealInProgress { get; set; }

	private void Awake()
	{
        instace = this;
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
        ShuffleCoroutine();
        StartCoroutine(ShuffleCoroutine());        
	}

    /// <summary>
    /// Shuffle Coroutine.
    /// Moves all card to pickupCardSlot. Then shuffles them back
    /// to cardStackSlot.
    /// </summary>
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

        DealInProgress--;
    }

    public IEnumerator DealCards()
    {
        DealInProgress++;

        for (int i = 0; i < 10; i++)
        {
            PlayerHand.instance.CardSlotList[i].AddCard(_centerStackCardSlot.TopCard());
            yield return new WaitForSeconds(.05f);
            AIHand.instance.CardSlotList[i].AddCard(_centerStackCardSlot.TopCard());
            yield return new WaitForSeconds(.05f);
        }

        MoveCardSlotToCardSlot(_centerStackCardSlot, _drawStackCardSlot);
        yield return new WaitForSeconds(.3f);

        //UpdateCurrentTurn(Turn.PlayerDraw);

        DealInProgress--;
    }

    //public IEnumerator DrawCoroutine()
    //{
    //DealInProgress++;

    //if (_discardHoverStackCardSlot.AddCard(_prior4CardSlot.TopCard()))
    //{	
    //	yield return new WaitForSeconds(CardStackDelay);	
    //}	
    //if (_discardStackCardSlot.AddCard(_discardHoverStackCardSlot.TopCard()))
    //{
    //	yield return new WaitForSeconds(CardStackDelay);
    //}
    //if (_prior4CardSlot.AddCard(_prior3CardSlot.TopCard()))
    //{
    //	yield return new WaitForSeconds(CardStackDelay);
    //}
    //if (_prior3CardSlot.AddCard(_prior2CardSlot.TopCard()))
    //{
    //	yield return new WaitForSeconds(CardStackDelay);
    //}
    //if (_prior2CardSlot.AddCard(_prior1CardSlot.TopCard()))
    //{
    //	yield return new WaitForSeconds(CardStackDelay);
    //}
    //if (_prior1CardSlot.AddCard(_prior0CardSlot.TopCard()))
    //{
    //	yield return new WaitForSeconds(CardStackDelay);	
    //}
    //if (_prior0CardSlot.AddCard(_currentCardSlot.TopCard()))
    //{
    //yield return new WaitForSeconds(CardStackDelay);		
    //}		
    //_currentCardSlot.AddCard(_centerStackCardSlot.TopCard());	

    //int collectiveFaceValue = _prior0CardSlot.FaceValue();
    //collectiveFaceValue += _prior1CardSlot.FaceValue();
    //collectiveFaceValue += _prior2CardSlot.FaceValue();
    //collectiveFaceValue += _prior3CardSlot.FaceValue();
    //collectiveFaceValue += _prior4CardSlot.FaceValue();
    //collectiveFaceValue += _currentCardSlot.FaceValue();	
    //GameUIInstance.FaceValueText.text = collectiveFaceValue.ToString();

    //DealInProgress--;
    //}	
}
