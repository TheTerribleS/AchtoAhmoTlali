﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> characterItems = new List<Item>();
    public ItemDatabase itemDatabase;
    public InventoryUI inventoryUI;

    bool doOnce = false;

    private void Update()
    {

    }

    public void GiveItem(int id)
    {
        Item itemToAdd = itemDatabase.GetItem(id);
        characterItems.Add(itemToAdd);
        inventoryUI.AddNewItem(itemToAdd);
    }

    public void GiveItem(string itemType)
    {
        Item itemToAdd = itemDatabase.GetItem(itemType);
        characterItems.Add(itemToAdd);
        inventoryUI.AddNewItem(itemToAdd);
    }

    public Item CheckForItem(int id0)
    {
        return characterItems.Find(item => item.id == id0);
    }

    public Item CheckForItem(string typeOfMaterial)
    {
        return characterItems.Find(item => item.resourceName == typeOfMaterial);
    }

    public void RemoveItem (int id)
    {
        Item itemToRemove = CheckForItem(id);
        if (itemToRemove != null)
        {
            characterItems.Remove(itemToRemove);
            inventoryUI.RemoveItem(itemToRemove);
        }
    }

    public void RemoveItem(string itemType)
    {
        Item itemToRemove = CheckForItem(itemType);
        if (itemToRemove != null)
        {
            characterItems.Remove(itemToRemove);
            inventoryUI.RemoveItem(itemToRemove);
        }
    }

}
