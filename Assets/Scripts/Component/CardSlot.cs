using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardSlot : MonoBehaviour 
{
	public List<Card> CardList = new List<Card>();

    //card under table?
	public bool _inverseStack;

	[Range(0.05f, 0.3f)]
	public float _positionDamp = .2f;

	[Range(0.05f, 0.3f)]
	public float _rotationDamp = .2f;

    public Transform TargetTransform
    {
        get
        {
            if (_targetTransform == null)
            {
                _targetTransform = new GameObject(this.name + "Target").GetComponent<Transform>();
                _targetTransform.position = transform.position;
                _targetTransform.forward = transform.forward;
            }
            return _targetTransform;
        }
        set
        {
            _targetTransform = value;
        }

    }
    private Transform _targetTransform;

    private void Awake()
	{
        _targetTransform = transform;
        GetComponent<MeshRenderer>().enabled = false;
	}

    private void Update()
    {
        SmoothToTargetPositionRotation();
    }

    public int FaceValue()
	{
		int collectiveFaceValue = 0;
		for (int i = 0; i < CardList.Count; ++i)
		{
			collectiveFaceValue += CardList[ i ].FaceValue;
		}
		return collectiveFaceValue;
	}
    
	public Card TopCard()
	{
		if (CardList.Count > 0)
		{
			return CardList[ CardList.Count - 1 ];
		}
		else
		{
			return null;
		}	
	}

    public Card GetCard(int index)
    {
        if (CardList.Count > index)
        {
            return CardList[index];
        }
        else
        {
            return null;
        }
    }

    public int GetSize()
    {
        return CardList.Count;
    }



    public Card BottomCard()
	{
		if (CardList.Count > 0)
		{
			return CardList[ 0 ];
		}
		else
		{
			return null;
		}			
	}
	
	public bool AddCard(Card card)
	{
		if (card != null)
		{
			if (card.ParentCardSlot != null)
			{
				card.ParentCardSlot.RemoveCard(card);
			}
			card.ParentCardSlot = this;
			CardList.Add(card);
            card.TargetTransform.rotation = transform.rotation;
			card.TargetTransform.Rotate(card.TargetTransform.forward, Random.Range(-.4f, .4f), Space.Self);
			float cardHeight = card.GetComponent<BoxCollider>().size.z;
			card.TargetTransform.position = transform.position;
			if (_inverseStack)
			{
				card.TargetTransform.Translate(new Vector3(0, 0, CardList.Count * (float)cardHeight) * -1f, Space.Self);
			}
			else
			{
				card.TargetTransform.Translate(new Vector3(0, 0, CardList.Count * (float)cardHeight), Space.Self);
			}
			card.SetDamp(_positionDamp, _rotationDamp);
			return true;
		}
		else
		{
			return false;
		}
	}

	public void RemoveCard(Card card)
	{
		card.ParentCardSlot = null;
		CardList.Remove(card);
	}

    public void RemoveAllCards()
    {
        while (TopCard() != null)
        {
            RemoveCard(TopCard());
        }
    }


    private void SmoothToTargetPositionRotation()
    {
        if (TargetTransform.position != transform.position || TargetTransform.eulerAngles != transform.eulerAngles)
        {
            SmoothToPointAndDirection(TargetTransform.position, _positionDamp, TargetTransform.rotation, _rotationDamp);
        }
    }


    private void SmoothToPointAndDirection(Vector3 point, float moveSmooth, Quaternion rotation, float rotSmooth)
    {
        transform.position = Vector3.SmoothDamp(transform.position, point, ref _smoothVelocity, moveSmooth);
        Quaternion newRotation;
        newRotation.x = Mathf.SmoothDamp(transform.rotation.x, rotation.x, ref _smoothRotationVelocity.x, rotSmooth);
        newRotation.y = Mathf.SmoothDamp(transform.rotation.y, rotation.y, ref _smoothRotationVelocity.y, rotSmooth);
        newRotation.z = Mathf.SmoothDamp(transform.rotation.z, rotation.z, ref _smoothRotationVelocity.z, rotSmooth);
        newRotation.w = Mathf.SmoothDamp(transform.rotation.w, rotation.w, ref _smoothRotationVelocity.w, rotSmooth);
        transform.rotation = newRotation;
    }
    private Vector3 _smoothVelocity;
    private Vector4 _smoothRotationVelocity;

}
