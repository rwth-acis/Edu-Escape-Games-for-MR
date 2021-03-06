﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// logical representation of a menu item
/// </summary>
[RequireComponent(typeof(Menu))]
[Serializable]
public class CustomMenuItem : MonoBehaviour
{
    /// <summary>
    /// the style of the menu item (which GameObject will be instantiated)
    /// </summary>
    public GameObject menuStyle;

    private GameObject origMenuStyle;
    /// <summary>
    /// list of child menu items
    /// </summary>
    public List<CustomMenuItem> subMenu;
    public bool overrideMenuDirection = false;
    public Direction subMenuDirection;
    [Tooltip("If enabled, the whole menu will be closed and the root menu will be displayed again")]
    public bool closeOnClick;
    [Tooltip("The menu item will be marked or unmarked when clicking on it")]
    public bool markOnClick;
    [Tooltip("Marks the menu item. Only used if markOnClick is true")]
    [SerializeField]
    private bool marked;

    /// <summary>
    /// visibility settings of the menu item
    /// </summary>
    public PlayerType visibleTo = PlayerType.ALL;

    private MenuStyleAdapter menuStyleAdapter;
    private GameObject containerInstance;
    private Menu parentMenu;
    private CustomMenuItem parentMenuItem;
    private bool subMenuOpened;
    private bool itemEnabled = true;

    [Tooltip("Functions which are called if the user clicks the menu entry")]
    public StringEvent onClickEvent;

    [SerializeField] // this makes the variable appear in the editor
    private Texture icon;
    [SerializeField] // this makes the variable appear in the editor
    private string text;
    [SerializeField]
    private string menuItemName;

    public string MenuItemName
    {
        get { return menuItemName; }
        set
        {
            if (parentMenu != null)
            {
                parentMenu.UpdateItemName(menuItemName, value);
            }
            menuItemName = value;
        }
    }


    public void Start()
    {
        InitialText = text;
        origMenuStyle = menuStyle;
        Text = text; // this is for automatic translation on startup
    }

    /// <summary>
    /// the icon of the menu item
    /// </summary>
    public Texture Icon
    {
        get
        { return icon; }
        set
        { menuStyleAdapter.UpdateIcon(value); icon = value; }
    }

    /// <summary>
    /// the text-content of the menu item
    /// </summary>
    public string Text
    {
        get { return text; }
        set
        {
            string localizedText = LocalizationManager.Instance.ResolveString(value);
            if (MenuSytleAdapter != null)
            {
                menuStyleAdapter.UpdateText(localizedText);
            }
            text = localizedText;
        }
    }

    public string InitialText
    {
        get; private set;
    }

    /// <summary>
    /// whether the item is currently enabled
    /// </summary>
    public bool ItemEnabled
    {
        get { return itemEnabled; }
        set
        {
            itemEnabled = value;
            if (menuStyleAdapter != null)
            {
                menuStyleAdapter.ItemEnabled = value;
            }
        }
    }

    public bool Marked
    {
        get { return marked; }
        set
        {
            marked = value;
            if (menuStyleAdapter != null)
            {
                menuStyleAdapter.Marked = value;
            }
        }
    }

    /// <summary>
    /// should be called if the menu item is created programmatically and not in the Unity editor
    /// </summary>
    public void Init(GameObject menuStyle, List<CustomMenuItem> subMenu, bool closeOnClick)
    {
        this.menuStyle = menuStyle;
        this.subMenu = subMenu;
        this.closeOnClick = closeOnClick;
        if (onClickEvent == null)
        {
            onClickEvent = new StringEvent();
        }
    }

    /// <summary>
    /// creates the menu item as a visible object in the 3D scene
    /// </summary>
    /// <param name="parentMenu">The menu to which the item belongs</param>
    /// <param name="parent">The parent menu item</param>
    public void Create(Menu parentMenu, CustomMenuItem parent)
    {
        this.parentMenuItem = parent;
        this.parentMenu = parentMenu;
        subMenuOpened = false;
        if (menuStyle == null)
        {
            menuStyle = parentMenu.defaultMenuStyle;
        }
        containerInstance = GameObject.Instantiate(menuStyle, parentMenu.transform);

        menuStyleAdapter = containerInstance.GetComponent<MenuStyleAdapter>();
        menuStyleAdapter.Initialize();
        menuStyleAdapter.RegisterForClickEvent(OnClick);
        menuStyleAdapter.UpdateText(text);
        menuStyleAdapter.UpdateIcon(icon);
        menuStyleAdapter.ItemEnabled = ItemEnabled;
        if (markOnClick)
        {
            menuStyleAdapter.Marked = marked;
        }

        if (onClickEvent == null)
        {
            onClickEvent = new StringEvent();
        }
    }

    public void Refresh()
    {
        Vector3 position = Position;
        Destroy();
        Create(parentMenu, parentMenuItem);
        containerInstance.transform.localPosition = position;
    }

    /// <summary>
    /// destroys the gameobject representation of the menu item
    /// </summary>
    public void Destroy()
    {
        if (containerInstance != null)
        {
            GameObject.Destroy(containerInstance);
        }
    }

    /// <summary>
    /// called if the menu item is cliced
    /// </summary>
    public void OnClick()
    {
        Debug.Log("clicked " + text + " (" + menuItemName + ")");
        if (ItemEnabled)
        {
            // invoke the defined action
            onClickEvent.Invoke(menuItemName);

            if (markOnClick)
            {
                if (parentMenu.markOnlyOne)
                {
                    parentMenu.MarkOne(this);
                }
                else
                {
                    Marked = !Marked;
                }
            }

            // reset the menu on click if closeOnClick is enabled
            if (closeOnClick)
            {
                parentMenu.ResetMenu();
                return;
            }

            // also spawn the sub menu if it exists
            if (subMenu.Count > 0 && !subMenuOpened)
            {
                menuStyle = (GameObject)Resources.Load("SimpleMenuItem Parent Icon");
                Refresh();
                //InstantiateSubMenus();
                Direction dir = parentMenu.alignment;
                if (overrideMenuDirection)
                {
                    dir = subMenuDirection;
                }
                parentMenu.InstantiateMenu(GameObjectInstance.transform.localPosition, menuStyleAdapter.Size, subMenu, this, true, dir);
                if (parentMenuItem != null)
                {
                    parentMenu.HideSiblings(this, parentMenuItem.subMenu);
                    parentMenuItem.Hide(); // this is for hiding parents of parents
                }
                else
                {
                    parentMenu.HideSiblings(this, parentMenu.rootMenu);
                }
                subMenuOpened = true;
            }
            else if (subMenu.Count > 0)
            {
                menuStyle = origMenuStyle;
                Refresh();
                // destroy the sub menu
                DestroySubmenus();
                // if parentMenuItem is null => it is the root
                if (parentMenuItem != null)
                {
                    parentMenu.ShowSiblings(parentMenuItem.subMenu);
                    parentMenuItem.Show();
                }
                else
                {
                    parentMenu.ShowSiblings(parentMenu.rootMenu);
                }
                subMenuOpened = false;
            }
        }
    }

    /// <summary>
    /// destroys the gameobjects of all child menu items
    /// </summary>
    private void DestroySubmenus()
    {
        foreach (CustomMenuItem child in subMenu)
        {
            child.Destroy();
        }
    }

    public void Hide()
    {
        if (GameObjectInstance != null)
        {
            GameObjectInstance.SetActive(false);
        }
    }

    public void Show()
    {
        if (GameObjectInstance != null)
        {
            GameObjectInstance.SetActive(true);
            MenuSytleAdapter.UpdateContainerColor();
        }
    }

    [System.Obsolete("InstantiateSubMenus is obsolete, please use parentMenu.InstantiateMenu instead")]
    private void InstantiateSubMenus()
    {
        Vector3 instantiatePosition = GameObjectInstance.transform.localPosition;
        instantiatePosition.y -= menuStyleAdapter.Size.y + parentMenu.padding;

        // instantiate the menu
        for (int i = 0; i < subMenu.Count; i++)
        {
            subMenu[i].Create(this.parentMenu, this);
            if (i > 0)
            {
                // set the correct position
                if (parentMenu.alignment == Direction.HORIZONTAL)
                {
                    // get to the middle between the previous and the current item
                    instantiatePosition.x += (subMenu[i - 1].MenuSytleAdapter.Size.x + parentMenu.padding) / 2;
                    // get to the center of the current item
                    instantiatePosition.x += (subMenu[i].MenuSytleAdapter.Size.x + parentMenu.padding) / 2;
                }
                else
                {
                    // get to the middle between the previous and the current item
                    instantiatePosition.y -= (subMenu[i - 1].MenuSytleAdapter.Size.y + parentMenu.padding) / 2;
                    // get to the center of the current item
                    instantiatePosition.y -= (subMenu[i].MenuSytleAdapter.Size.y + parentMenu.padding) / 2;
                }
            }
            subMenu[i].Position = instantiatePosition;
        }
    }

    /// <summary>
    /// the menuStyleAdapter which is attached to this menu
    /// </summary>
    public MenuStyleAdapter MenuSytleAdapter { get { return menuStyleAdapter; } }

    /// <summary>
    /// the position of the gameobject-representation
    /// </summary>
    public Vector3 Position { get { return containerInstance.transform.localPosition; } set { containerInstance.transform.localPosition = value; } }

    /// <summary>
    /// the gameobject representation
    /// </summary>
    public GameObject GameObjectInstance { get { return containerInstance; } }
}
