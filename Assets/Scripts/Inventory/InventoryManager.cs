using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject InventoryGO;
    [SerializeField] private InventorySlot[] inventorySlots;
    [SerializeField] private GameObject inventoryItemPrefab;

    [SerializeField]
    private Item FakeItem;

    [SerializeField]
    private InputActionAsset InputActions;
    private InputActionMap UIActionMap;
    private InputAction Toggle;
    private InputAction FakeAction;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more then one  InventoryManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        UIActionMap = InputActions.FindActionMap("UI");
        Toggle = InputActions.FindAction("ToggleInventory");
        Toggle.performed += HandleToggleActionPerformed;
        Toggle.Enable();
        FakeAction = InputActions.FindAction("FakeAction");
        FakeAction.started += HandleFakeActionStarted;
        FakeAction.Enable();
        UIActionMap.Enable();
        InputActions.Enable();
    }

    private void HandleToggleActionPerformed(InputAction.CallbackContext Context)
    {
        ToggleInventory();
    }

    private void HandleFakeActionStarted(InputAction.CallbackContext Context)
    {
        AddItem(FakeItem);
    }

    public void ToggleInventory()
    {
        InventoryGO.SetActive(!InventoryGO.activeSelf);
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null &&
                itemInSlot.GetItem() == item &&
                itemInSlot.GetItem().IsStackable() &&
                (itemInSlot.Count < itemInSlot.GetItem().MaxStack))
            {
                itemInSlot.Count++;
                itemInSlot.RefreshCount();
                return true;
            }
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    public void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGO = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>();
        slot.SetItemToSlot(inventoryItem);
        inventoryItem.InitializeItem(item);
    }

    public void SpawnNewItem(Item item, InventorySlot slot, int count)
    {
        GameObject newItemGO = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>();
        slot.SetItemToSlot(inventoryItem);
        inventoryItem.Count = count;
        inventoryItem.InitializeItem(item);
    }
}

    