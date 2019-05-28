using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
    public CardSlot ParentCardSlot { get; set; }

    public int FaceValue { get; set; } //Face cards:10; Ace:11
    public Suit CardSuit { get; set; } //Enum SUIT defined in CardDeck.cs
    public int Rank { get; set; } //Ace:1; Jack:11; Queen:12; King:13

    private float _positionDamp = .2f;
    private float _rotationDamp = .2f;

    //dir path to texture file (.png): AssetBundles/Cards
    public string TexturePath { get; set; }

    //dir path to AssetBundles/Cards
    public string SourceAssetBundlePath { get; set; }

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
    }
    private Transform _targetTransform;

    private void Update()
    {
        SmoothToTargetPositionRotation();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //    print("clicking on: " + name + "\n");
            //    print("ParentCardSlot: " + ParentCardSlot.name + "\n");

            //PLAYER DRAW
            if (Round.instance.CurrentTurn == Turn.PlayerDraw)
            {
                if (ParentCardSlot.name.IndexOf("DrawStackSlot", System.StringComparison.CurrentCulture) != -1 
                || ParentCardSlot.name.IndexOf("DiscardStackSlot", System.StringComparison.CurrentCulture) != -1)
                {
                    PlayerHand.instance.DrawCard(this);
                }
            }

            //PLAYER DISCARD
            else if (Round.instance.CurrentTurn == Turn.PlayerDiscard)
            {
                if (ParentCardSlot.name.IndexOf("PlayerCardSlot", System.StringComparison.CurrentCulture) != -1)
                {
                    PlayerHand.instance.DiscardCard(this, false);
                }
            }
        }
    }

    public void SetDamp(float newPositionDamp, float newRotationDamp)
    {
        _positionDamp = newPositionDamp;
        _rotationDamp = newRotationDamp;
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
        TestVisibility();
    }
    private Vector3 _smoothVelocity;
    private Vector4 _smoothRotationVelocity;

    private void TestVisibility()
    {
        float angle = Vector3.Angle(Camera.main.transform.forward, transform.forward);
        if (angle < 90)
        {
            FrontBecameVisible();
        }
        else
        {
            FrontBecameHidden();
        }
    }

    private void FrontBecameVisible()
    {
        AssetBundle cardBundle = BundleSingleton.Instance.LoadBundle(SourceAssetBundlePath);
        GetComponent<Renderer>().material.mainTexture = (Texture)cardBundle.LoadAsset(TexturePath);
    }

    private void FrontBecameHidden()
    {
        Resources.UnloadAsset(GetComponent<Renderer>().material.mainTexture);
        GetComponent<Renderer>().material.mainTexture = null;
    }
}

