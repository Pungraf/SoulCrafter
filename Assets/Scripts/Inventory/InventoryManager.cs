using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject inventoryItemPrefab;

    [SerializeField]
    private Item FakeItem;

    [SerializeField]
    private InputActionAsset InputActions;
    private InputActionMap UIActionMap;
    private InputAction Toggle;
    private InputAction FakeAction;

    /// ////////////////////////////////////////////////////
    public List<UI_InventorySlot> InventoryItems = new List<UI_InventorySlot>();

    private VisualElement m_Root;
    private VisualElement m_InventoryCointainer;
    private bool isInventoryVisible = false; 
    private VisualElement m_SlotContainer;

    private static bool m_IsDragging;
    private static UI_InventorySlot m_OriginalSlot;

    private static VisualElement m_GhostIcon;
    private static VisualElement m_DescriptionPanel;
    private static Label m_DescriptionPanel_Title;
    private static Label m_DescriptionPanel_Description;
    private static bool m_DesDescriptionPanelInOn;

    public VisualElement M_Root => m_Root;

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

        ///////////////////////////
        //Store the root from the UI Document component
        m_Root = GetComponent<UIDocument>().rootVisualElement;
        m_InventoryCointainer = m_Root.Q<VisualElement>("Container");
        //Search the root for the SlotContainer Visual Element
        m_SlotContainer = m_Root.Q<VisualElement>("SlotContainer");

        m_GhostIcon = m_Root.Query<VisualElement>("GhostIcon");
        m_GhostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        m_GhostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

        m_DescriptionPanel = m_Root.Query<VisualElement>("DetailsPanel");
        m_DescriptionPanel_Title = m_DescriptionPanel.Query<Label>("ItemNameLabel");
        m_DescriptionPanel_Description = m_DescriptionPanel.Query<Label>("DetailsLabel");
        //Create InventorySlots and add them as children to the SlotContainer
        for (int i = 0; i < 25; i++)
        {
            UI_InventorySlot item = new UI_InventorySlot();

            InventoryItems.Add(item);

            m_SlotContainer.Add(item);
        }
    }

    public void SetDescriptionPanel(MouseEnterEvent evt, string title, string description)
    {
        if(m_DesDescriptionPanelInOn) return;
        m_DescriptionPanel.style.visibility = Visibility.Visible;
        m_DescriptionPanel.style.left = evt.mousePosition.x;
        m_DescriptionPanel.style.top = evt.mousePosition.y;
        m_DescriptionPanel_Title.text = title;
        m_DescriptionPanel_Description.text = description;
        m_DesDescriptionPanelInOn = true;
    }

    public void HideDescriptionPanel()
    {
         m_DescriptionPanel.style.visibility = Visibility.Hidden;
         m_DesDescriptionPanelInOn = false;
    }

    public void StartDrag(Vector2 position, UI_InventorySlot originalSlot)
    {
        //Set tracking variables
        m_IsDragging = true;
        m_OriginalSlot = originalSlot;

        //Set the new position
        m_GhostIcon.style.top = position.y - m_GhostIcon.layout.height / 2;
        m_GhostIcon.style.left = position.x - m_GhostIcon.layout.width / 2;

        //Set the image
        m_GhostIcon.style.backgroundImage = originalSlot.Item.GetSprite().texture;

        //Flip the visibility on
        m_GhostIcon.style.visibility = Visibility.Visible;
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!m_IsDragging)
        {
            return;
        }

        //Set the new position
        m_GhostIcon.style.top = evt.position.y - m_GhostIcon.layout.height / 2;
        m_GhostIcon.style.left = evt.position.x - m_GhostIcon.layout.width / 2;

    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!m_IsDragging)
        {
            return;
        }

        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<UI_InventorySlot> slots = InventoryItems.Where(x =>
               x.worldBound.Overlaps(m_GhostIcon.worldBound));
        

        //Found at least one
        if (slots.Count() != 0)
        {
            UI_InventorySlot closestSlot = slots.OrderBy(x => Vector2.Distance
               (x.worldBound.position, m_GhostIcon.worldBound.position)).First();

            if(closestSlot != m_OriginalSlot)
            {
                if(closestSlot.Item != null)
                {
                    Item itemToSwitch = closestSlot.Item;
                    int countToSwitch = closestSlot.Count;
                    if (closestSlot.Item.ItemName == m_OriginalSlot.Item.ItemName && closestSlot.Item.IsStackable)
                    {
                        if(m_OriginalSlot.Count + closestSlot.Count > itemToSwitch.MaxStack)
                        {
                            m_OriginalSlot.Count = Mathf.Abs(m_OriginalSlot.Count - closestSlot.Count);
                            closestSlot.Count = itemToSwitch.MaxStack;
                        }
                        else
                        {
                            closestSlot.Count += m_OriginalSlot.Count;
                            m_OriginalSlot.RemoveItemFromSlot();
                        }
                    }
                    else
                    {
                        closestSlot.AddItemToSlot(m_OriginalSlot.Item, m_OriginalSlot.Count);
                        m_OriginalSlot.AddItemToSlot(itemToSwitch, countToSwitch);
                    }
                }
                else
                {
                    //Set the new inventory slot with the data
                    closestSlot.AddItemToSlot(m_OriginalSlot.Item, m_OriginalSlot.Count);
                    //Clear the original slot
                    m_OriginalSlot.RemoveItemFromSlot();
                }
            }
            else
            {
                m_OriginalSlot.SetImage();
            }
        }
        //Didn't find any (dragged off the window)
        else
        {
            m_OriginalSlot.SetImage();
        }
        //Clear dragging related visuals and data
        m_IsDragging = false;
        m_GhostIcon.style.visibility = Visibility.Hidden;

    }

    // Function to toggle the visibility of the inventory panel
    public void ToggleInventoryUI(bool isVisible)
    {
        if (m_Root != null)
        {
            isInventoryVisible = isVisible;
            m_InventoryCointainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    // Convenience method for toggling on and off
    public void ToggleInventoryUI()
    {
        ToggleInventoryUI(!isInventoryVisible);
    }

    private void HandleToggleActionPerformed(InputAction.CallbackContext Context)
    {
        //ToggleInventory();
        ToggleInventoryUI();
    }

    private void HandleFakeActionStarted(InputAction.CallbackContext Context)
    {
        AddItem(FakeItem);
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < InventoryItems.Count; i++)
        {
            UI_InventorySlot slot = InventoryItems[i];
            Item itemInSlot = slot.Item;

            if (itemInSlot == null || (itemInSlot.IsStackable && slot.Count < item.MaxStack && itemInSlot.ItemName == item.ItemName))
            {
                if(itemInSlot == null)
                {
                    slot.AddItemToSlot(item);
                    return true;
                }
                else
                {
                    slot.Count++;
                    return true;
                }
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

    