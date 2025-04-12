using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MRM_Details : MonoBehaviour
{
    public TextMeshProUGUI MarbleName, MarbleDescription;
    public TextMeshProUGUI MarbleDimension, MarbleMaterial, MarbleFinish;
    public RawImage CircleImage, TopMarbleImage;
    public RawImage[] BgImages;

    public Toggle isSelected;
}
