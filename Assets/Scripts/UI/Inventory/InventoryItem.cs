using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static CodeMonkey.Utils.Button_UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    [SerializeField] private string imageName;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Item item;

    public bool ButtonIsActive;
    private Button_UI buttonUI;
    InterceptActionHandler interceptHandler;

    public Image image;

    private int count = 1;

    private void Start()
    {
        RefreshCount();
        buttonUI = GetComponent<Button_UI>();
        buttonUI.MouseRightClickFunc = () =>
        {
            UseItem();
        };
        interceptHandler = buttonUI.InterceptAction("MouseRightClickFunc", () =>
        {
            // Condition to allow or block MouseRightClickFunc
            return ButtonIsActive;
        });
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.parent.GetComponent<InventorySlot>().SetItemToSlot(null);
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }

    public Item Item
    {
        get { return item; }
        set { item = value; }
    }

    public void InitializeItem(Item item)
    {
        this.item = item;
        image.sprite = item.GetSprite();
    }


    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public Item GetItem()
    {
        return item;
    }

    public int Count
    {
        get => count;
        set
        {
            count = value;
            if (count < 0)
            {
                count = 0;
            }
            RefreshCount();
        }
    }

    private void UseItem()
    {
        if (Item is IUsable)
        {
            IUsable usableitem = (IUsable)Item;
            usableitem.Use();
            count--;
            if (count < 1)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Item is unusable");
        }
    }
}
