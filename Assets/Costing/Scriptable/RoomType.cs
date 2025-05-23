using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RoomType", order = 1)]
public class RoomType : ScriptableObject
{
    public int RoomId;
    public string RoomName;
    //public Transform DimensionObject;   
    public string Roomdimension;
    public Sprite BigRoomSprite;
    public List<MarbleDetails> DropSelectedMarbles = new();
}

[System.Serializable]
public class MarbleDetails{
    public int id;
    public string marble_name;
    public string imgUrl;
    public Texture _Texture;
    public string marbleDimension;
    public string PriceMarble;

    public MarbleDetails DeepClone()
    {
        return new MarbleDetails
        {
            id = this.id,
            marble_name = this.marble_name,
            imgUrl = this.imgUrl,
            _Texture = this._Texture, // Textures are reference types - consider copying if needed
            marbleDimension = this.marbleDimension,
            PriceMarble = this.PriceMarble
        };
    }
}
