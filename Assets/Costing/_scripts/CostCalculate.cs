using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System;

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
    public ScrollRect Costscrollrect;

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
        Costscrollrect.verticalNormalizedPosition = 1;
        RemoveInvoiceObjects();
        InvoiceManager._invoiceMananger.MarbleInvoiceData = new MarbleInvoice();
        //============================================
        for (int i = 0; i < RoomsManager.RoomInstance.RoomsAddedSequence.Count; i++)
        {
            int index = RoomsManager.RoomInstance.RoomsAddedSequence[i];
            // print("at index " + i);
            if (_roomScriptableData[index].DropSelectedMarbles.Count > 0)
            {
                // create room costing prefab
                GameObject _RoomsWithMarbleObject = Instantiate(CostingRoomPrefab, CostingScroller);
                int count = _roomScriptableData[index].DropSelectedMarbles.Count;
                _RoomsWithMarbleObject.GetComponent<RoomInvoice>().RoomnameIn.text = _roomScriptableData[index].RoomName;
                //======== add room data to json==========================
                Room room1 = new Room();
                room1.id = _roomScriptableData[index].RoomId;
                room1.name = _roomScriptableData[index].RoomName;
                room1.dimension = _roomScriptableData[index].Roomdimension;
                
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
                    // Add Marbles data to each room =============================

                    room1.marbles.Add(new Marble()
                    {
                        id = _roomScriptableData[index].DropSelectedMarbles[j].id,
                        marble_name = _roomScriptableData[index].DropSelectedMarbles[j].marble_name,
                        dimension = _roomScriptableData[index].DropSelectedMarbles[j].marbleDimension,
                        price = priceFloat,
                        imgurl = _roomScriptableData[index].DropSelectedMarbles[j].imgUrl,
                        amount = dprice
                    }); ;
                }
              
                //add room to marble invoce data
                InvoiceManager._invoiceMananger.MarbleInvoiceData.rooms.Add(room1);
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

            // add price data to json marble invoice class to send data to CMS
            InvoiceManager._invoiceMananger.MarbleInvoiceData.price_summary.subtotal = TotalAmount;
            InvoiceManager._invoiceMananger.MarbleInvoiceData.price_summary.wastage_fee = tenPercent;
            InvoiceManager._invoiceMananger.MarbleInvoiceData.price_summary.installation = twoPercent;
            InvoiceManager._invoiceMananger.MarbleInvoiceData.price_summary.transport = fivePercent;
            InvoiceManager._invoiceMananger.MarbleInvoiceData.price_summary.tax = GstPercent;
            InvoiceManager._invoiceMananger.MarbleInvoiceData.price_summary.total = finalamt;

            //add userid
            //InvoiceManager._invoiceMananger.MarbleInvoiceData.user_id = 1;
        }

        // print json raw data for cms
        string jsonPayload = JsonUtility.ToJson(InvoiceManager._invoiceMananger.MarbleInvoiceData, prettyPrint: true);
        Debug.Log(jsonPayload);
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
           TotalAmount = 0;
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
            if (string.IsNullOrWhiteSpace(dimensionStr))
            {
                Debug.LogError("Dimension string is empty");
                return 0;
            }

            // Remove whitespace and "ft" (optional)
            string cleanedStr = Regex.Replace(dimensionStr, @"[\sftFT]", "");

            // Split by 'x' (case-insensitive)
            string[] parts = cleanedStr.Split(new[] { 'x', 'X' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                Debug.LogError($"Invalid format: '{dimensionStr}'. Expected format like '1x1' or '12.5x10'");
                return 0;
            }

            // SIMPLE PARSING (no CultureInfo or NumberStyles)
            if (!float.TryParse(parts[0], out float length) ||
                !float.TryParse(parts[1], out float width))
            {
                Debug.LogError($"Could not parse numbers in: '{dimensionStr}'");
                return 0;
            }

            // Validate positive dimensions
            if (length <= 0 || width <= 0)
            {
                Debug.LogError("Dimensions must be positive numbers");
                return 0;
            }

            // Calculate price
            float area = length * width;
            float totalPrice = area * pricePerSqft;

            return Mathf.RoundToInt(totalPrice);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error calculating price: {e.Message}");
            return 0;
        }
    }
}

