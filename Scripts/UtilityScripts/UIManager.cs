#if PLANNER_DOMAIN_GENERATED
using System;
using System.Collections.Generic;
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.UI;
using DefenderNameSpace;
using UnityEngine;

//This Script also sets your inventory
public class UIManager : MonoBehaviour
{
    
    public Image UIBarFood;
    public Image UIBarWater;
    public Image UIBarHarvest;
    public GameObject FoodIcon;
    public GameObject WaterIcon;
    public GameObject HarvestIcon;
    public float InventoryOffset;

    Defender m_Defender;
    List<Image> m_Hunger = new List<Image>();
    List<Image> m_Thirst = new List<Image>();

    List<Image> m_Harvest = new List<Image>();

    Material m_hungerMat;
    Material m_thirstMat;

    Material m_HarvestMat;

    int m_NumFood;
    int m_MaxFood = 10;
    int m_NumWater;
    int m_MaxWater = 10;
    //for resources
    int m_NumResource;
    int m_MaxResources = 20; 
    static readonly int s_Amount = Shader.PropertyToID("_Amount");

    static ComponentType[] k_NeedType;
    static ComponentType[] k_InventoryType;
    static ComponentType[] k_PouchType;

    public void Awake()
    {
        m_Defender = GetComponent<Defender>();

        // Make instances of these, so the asset on disk doesn't get modified
        UIBarFood.material = Instantiate(UIBarFood.material);
        UIBarWater.material = Instantiate(UIBarWater.material);
        UIBarHarvest.material = Instantiate(UIBarHarvest.material);
        m_hungerMat = UIBarFood.material;
        m_thirstMat = UIBarWater.material;
        m_HarvestMat = UIBarHarvest.material;
        m_hungerMat.SetFloat(s_Amount, 1);
        m_thirstMat.SetFloat(s_Amount, 1);
        m_HarvestMat.SetFloat(s_Amount, 1);
        k_NeedType = new ComponentType[] { typeof(Need) };
        k_InventoryType = new ComponentType[] { typeof(Inventory) };
        k_PouchType = new ComponentType[] { typeof(Pouch) };
    }

    public void Start()
    {
        for (int i = m_MaxFood - 1 ; i >= 0; i--)
        {
            var food = Instantiate(FoodIcon, FoodIcon.transform.parent, true);
            food.transform.position = FoodIcon.transform.position;
            food.transform.localScale = new Vector3(1, 1, 1);

            var foodRect = food.GetComponent<RectTransform>();
            var position = foodRect.position;
            position.x += -1 * InventoryOffset * i;
            foodRect.position = position;
            var foodImage = food.GetComponent<Image>();
            foodImage.enabled = false;
            food.SetActive(true);

            m_Hunger.Add(foodImage);
        }

        for (int i = m_MaxWater - 1; i >= 0; i--)
        {
            var water = Instantiate(WaterIcon, WaterIcon.transform.parent, true);
            water.transform.position = WaterIcon.transform.position;
            water.transform.localScale = new Vector3(1, 1, 1);

            var waterRect = water.GetComponent<RectTransform>();
            var position = waterRect.position;
            position.x += -1 * InventoryOffset * i;
            waterRect.position = position;
            var waterImage = water.GetComponent<Image>();
            waterImage.enabled = false;
            water.SetActive(true);

            m_Thirst.Add(waterImage);
        }

        for (int i = m_MaxResources - 1; i >= 0; i--)
        {
            var wood = Instantiate(HarvestIcon, HarvestIcon.transform.parent, true);
            wood.transform.position = HarvestIcon.transform.position;
            wood.transform.localScale = new Vector3(1, 1, 1);

            var woodRect = wood.GetComponent<RectTransform>();
            var position = woodRect.position;
            position.x += -1 * InventoryOffset * i;
            woodRect.position = position;
            var woodImage = wood.GetComponent<Image>();
            woodImage.enabled = false;
            wood.SetActive(true);

            m_Harvest.Add(woodImage);
        }
    }

    public void Update()
    {
#if PLANNER_DOMAIN_GENERATED
        var state = m_Defender.GetCurrentState(false);

        SetNeeds(state);
        SetInventory(state);
        SetPouch(state);
#endif
    }
    void SetNeeds(StateData state)
    {
        var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.TempJob);
        foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, k_NeedType))
        {
            var need = state.GetTraitOnObjectAtIndex<Need>(domainObjectIndex);

            if (need.NeedType == NeedType.Hunger)
                m_hungerMat.SetFloat(s_Amount, 1 - need.Urgency / 200f);
            else if (need.NeedType == NeedType.Thirst)
                m_thirstMat.SetFloat(s_Amount, 1 - need.Urgency / 200f);
            else if (need.NeedType == NeedType.Harvest)
                m_HarvestMat.SetFloat(s_Amount, 1 - need.Urgency / 200f);
            
        }
        domainObjects.Dispose();
    }

    void SetInventory(StateData state)
    {
        var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.TempJob);
        foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, k_InventoryType))
        {
            var inventory = state.GetTraitOnObjectAtIndex<Inventory>(domainObjectIndex);

            if (inventory.SatisfiesNeed == NeedType.Hunger) // food
            {
                m_NumFood = (int)inventory.Amount;
                for (var i = 0; i < m_Hunger.Count; i++)
                {
                    m_Hunger[i].enabled = i >= m_MaxFood - m_NumFood;
                }
            }
            else if (inventory.SatisfiesNeed == NeedType.Thirst) // water
            {
                m_NumWater = (int)inventory.Amount;
                for (var i = 0; i < m_Thirst.Count; i++)
                {
                    m_Thirst[i].enabled = i >= m_MaxWater - m_NumWater;
                }
            }
        }
        domainObjects.Dispose();
    }

    void SetPouch(StateData state)
    {
        
        var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.TempJob);
        foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, k_PouchType))
        {
            var Pouch = state.GetTraitOnObjectAtIndex<Pouch>(domainObjectIndex);

            if (Pouch.HarvestType == HarvestableType.Wood) // food
            {
                m_NumResource = (int)Pouch.Amount;
                for (var i = 0; i < m_Harvest.Count; i++)
                {
                    m_Harvest[i].enabled = i >= m_MaxResources - m_NumResource;
                }
            }
           
        }
        domainObjects.Dispose();
    }
}
#endif
