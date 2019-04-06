using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public enum Suit { Spades, Hearts, Clubs, Diamonds };

public class CardDeck : MonoBehaviour 
{
	public GameObject _cardPrefab;	
	
	public readonly List<Card> CardList =  new List<Card>();

    //cardBundlePath = "cards" aka Cards folder under AssetBundles
    public void InstanatiateDeck(string cardBundlePath)
	{
		AssetBundle cardBundle = BundleSingleton.Instance.LoadBundle(DirectoryUtility.ExternalAssets() + cardBundlePath);
		string[] nameArray = cardBundle.GetAllAssetNames();

        shuffleArray(nameArray);

        //go thru the Cards folder, initialize each as a Card instance
        //then add to CardList	
        for (int i = 0; i < nameArray.Length; ++i)
		{
			GameObject cardInstance = (GameObject)Instantiate(_cardPrefab);
			Card card = cardInstance.GetComponent<Card>();
			card.gameObject.name = Path.GetFileNameWithoutExtension(nameArray[ i ]);
			card.TexturePath = nameArray[ i ];
			card.SourceAssetBundlePath = cardBundlePath;
			card.transform.position = new Vector3(0, 10, 0);
            StringToProperties(card);
			CardList.Add(card);
		}
	}

    //Function to shuffle the input array in random order
    void shuffleArray(string[] array)
    {
        for (int t = 0; t < array.Length; t++)
        {
            string tmp = array[t];
            int r = Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = tmp;
        }
    }

    private void StringToProperties(Card card)
	{
        string name = card.gameObject.name;

        if (name.Contains("clubs"))
        {
            card.CardSuit = Suit.Clubs;
        }
        else if (name.Contains("diamonds"))
        {
            card.CardSuit = Suit.Diamonds;
        }
        else if (name.Contains("hearts"))
        {
            card.CardSuit = Suit.Hearts;
        }
        else if (name.Contains("spades"))
        {
            card.CardSuit = Suit.Spades;
        }

        for (int i = 2; i < 11; ++i)
		{
			if (name.Contains(i.ToString()))
			{
                card.FaceValue = i;
                card.Rank = i;
                return;
			}
		}
        if (name.Contains("jack"))
        {
            card.FaceValue = 10;
            card.Rank = 11;
        }
        else if (name.Contains("queen")) 
        {
            card.FaceValue = 10;
            card.Rank = 12;
        }
        else if (name.Contains("king"))
        {
            card.FaceValue = 10;
            card.Rank = 13;
        }
		else if (name.Contains("ace"))
		{
			card.FaceValue = 11;
            card.Rank = 1;
        }
	}	
}
