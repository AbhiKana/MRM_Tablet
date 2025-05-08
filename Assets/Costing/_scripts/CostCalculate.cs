using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CostCalculate : MonoBehaviour
{
    public static CostCalculate Costcal;
    // [Header("Rooms Scriptable")]
    public List<RoomType> _roomScriptableData;

    // Start is called before the first frame update
    public Transform GenerateBtn;
    public Transform Invoicepanel;
    public Transform CostingScroller;
    public GameObject CostingRoomPrefab;
    public GameObject TotalInvoicePrefab;
    public GameObject CostMarbleInfoPrefab;
    private int TotalAmount = 0;

    private void Start()
    {
        Invoicepanel.gameObject.SetActive(false);
        GenerateBtn.gameObject.SetActive(false);
    }
    void Awake()
    {
        if (Costcal != null)
        {
            Destroy(gameObject);
            return;
        }
        Costcal = this;
    }

    // On click of Generate Invoice button create room costing as per marbles added per room
    public void OnclickOfInvoice()
    {
        Invoicepanel.gameObject.SetActive(true);
        CreateCostingPerRoom();
    }

    // check per room if marbles added then only create it in that room
    public void CreateCostingPerRoom()
    {
        RemoveInvoiceObjects();
        //============================================
        for (int i = 0; i < RoomsManager.RoomInstance.RoomsAddedSequence.Count; i++)
        {
            TotalAmount = 0;
            int index = RoomsManager.RoomInstance.RoomsAddedSequence[i];
           // print("at index " + i);
            if (_roomScriptableData[index].DropSelectedMarbles.Count > 0)
            {
                // create room costing prefab
                GameObject _RoomsWithMarbleObject = Instantiate(CostingRoomPrefab, CostingScroller);
                int count = _roomScriptableData[index].DropSelectedMarbles.Count;
                _RoomsWithMarbleObject.GetComponent<RoomInvoice>().RoomnameIn.text = _roomScriptableData[index].RoomName;
               // print(" marble count " + count);
                for (int j = 0; j < count; j++)
                {
                    GameObject _marbleCostInfo = Instantiate(CostMarbleInfoPrefab, _RoomsWithMarbleObject.transform);
                    _marbleCostInfo.GetComponent<MarbleInformation>()._RawTexture.texture = _roomScriptableData[index].DropSelectedMarbles[j]._Texture;
                    _marbleCostInfo.GetComponent<MarbleInformation>().Marbleinfoid = _roomScriptableData[index].DropSelectedMarbles[j].id;
                    _marbleCostInfo.GetComponent<MarbleInformation>().MarbleNameInfo.text = _roomScriptableData[index].DropSelectedMarbles[j].marble_name;
                    _marbleCostInfo.GetComponent<MarbleInformation>().MarblePriceInfo.text = _roomScriptableData[index].DropSelectedMarbles[j].PriceMarble + " sq/ft";
                    _marbleCostInfo.GetComponent<MarbleInformation>().MarbleDimensionInfo.text = _roomScriptableData[index].DropSelectedMarbles[j].marbleDimension;
                    float priceFloat = float.Parse(_roomScriptableData[index].DropSelectedMarbles[j].PriceMarble);
                    int dprice = CalculatePrice(priceFloat, _roomScriptableData[index].DropSelectedMarbles[j].marbleDimension);
                    _marbleCostInfo.GetComponent<MarbleInformation>().MarbleAmountInfo.text = "Rs." + dprice;
                    // add total amount marbles
                    TotalAmount += dprice;
                }
            }
        }

        if (TotalAmount > 0)
        {
            // create total invoice prefab at bottom last to calculate final total
            GameObject _Invoicetotal = Instantiate(TotalInvoicePrefab, CostingScroller);

            _Invoicetotal.GetComponent<TotalCalculation>().Totalofmarble.text = TotalAmount + "";
            float tenPercent = TotalAmount * 0.1f; // 10% of the total for wastage
            _Invoicetotal.GetComponent<TotalCalculation>().Wastage.text = tenPercent + "";
            float twoPercent = TotalAmount * 0.02f; // 2% of the total for installation
            _Invoicetotal.GetComponent<TotalCalculation>().Installation.text = twoPercent + "";
            float fivePercent = TotalAmount * 0.05f; // 5% of the total for Transport
            _Invoicetotal.GetComponent<TotalCalculation>().Transport.text = fivePercent + "";
            float GstPercent = TotalAmount * 0.18f; // 18% of the total for GST
            _Invoicetotal.GetComponent<TotalCalculation>().TaxGST.text = GstPercent + "";
            // add all float values to make total
            int finalamt = TotalAmount + (int)(tenPercent + twoPercent + fivePercent + GstPercent);

            _Invoicetotal.GetComponent<TotalCalculation>().FinalAmount.text = finalamt + "";
        }
    }

    // to clear scriptable marbles list data
    public void OnDisable()
    {
        for (int j = 0; j < _roomScriptableData.Count; j++)
        {
            _roomScriptableData[j].DropSelectedMarbles.Clear();
        }
    }

    public void RemoveInvoiceObjects()
    {
        // remove instantiated invoice prefab if already created 
        // and create new everytime
        int tcount = CostingScroller.childCount;
       // print("tcount " + tcount);
        if (tcount > 0)
        {
            for (int i = 0; i < tcount; i++)
            {
                Destroy(CostingScroller.GetChild(i).gameObject);
            }
        }
    }

    // return integer value to calculate marble price 
    public int CalculatePrice(float pricePerSqft, string dimensionStr)
    {
        try
        {
            // Remove "ft" if present and trim whitespace
            string cleanedStr = dimensionStr.Replace("ft", "").Trim();

            // Split by 'x' character
            string[] dimensions = cleanedStr.Split('x');

            if (dimensions.Length != 2)
            {
                Debug.LogError("Invalid dimension format. Use format like '10x15 ft'");
                return 0;
            }

            // Parse dimensions to floats
            if (!float.TryParse(dimensions[0].Trim(), out float length) ||
                !float.TryParse(dimensions[1].Trim(), out float width))
            {
                Debug.LogError("Could not parse dimension values");
                return 0;
            }

            // Calculate area and total price
            float area = length * width;
            float totalPrice = area * pricePerSqft;

            // Round to nearest integer
            return Mathf.RoundToInt(totalPrice);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error calculating price: {e.Message}");
            return 0;
        }
    }
}

