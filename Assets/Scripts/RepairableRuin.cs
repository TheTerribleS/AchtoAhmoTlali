﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Events;


public class RepairableRuin : MonoBehaviour
{
    public bool haveIBeenRepaired = false,
                hasNotChangedColorsYet = true,
                isPlayerNearMe = false,
                canIBeRepaired = false,
                amIAPhase2Ruin = false,
                amIBeingPickedUp = false,
                doIHavePriority = false,
                hasPhase2Begun = false;

    public List<Item> itemsINeed = new List<Item>();
    public ItemDatabase itemDatabase;
    public InventoryUI inventoryUI;

    public string historyWhenNotRepaired;
    public string historyWhenRepaired;
    public string material0, material1;

    public string optionsMessageToExport = "",
                  destroyOptionString = "";



    public float priorityChangeFactor = 1;

    Light RuinLight;

    Color newColorOfLight;
    new/*, because af a stupid warning, */ ParticleSystem particleSystem;

    private void Start()
    {
        RuinLight = gameObject.GetComponentInChildren<Light>();

        if (!amIAPhase2Ruin)
        {
            particleSystem = GetComponentInChildren<ParticleSystem>();

            particleSystem.Stop();

            PyramidControler.PlayerIsNearThePyramid += ActivateParticles;
            PyramidControler.PlayerIsNotNearThePyramid += DeactivateParticles;
        }

        Item item0ToAdd = itemDatabase.GetItem(materialName:material0);
        Item item1ToAdd = itemDatabase.GetItem(materialName:material1);
        itemsINeed.Add(item0ToAdd);
        itemsINeed.Add(item1ToAdd);
    }

    public void Update()
    {
        if (hasNotChangedColorsYet)
        {
            if (haveIBeenRepaired)
                newColorOfLight = Color.red;
            
            else if (amIBeingPickedUp)
                newColorOfLight = Color.yellow;
           
            else
                newColorOfLight = Color.green;

            hasNotChangedColorsYet = false;
            
            RuinLight.color = newColorOfLight;
        }

        if (isPlayerNearMe)
        {
            if (RuinLight.intensity <= 6)
                RuinLight.intensity += ( Time.deltaTime * 4 ) ;
            
            if (amIAPhase2Ruin) 
                RuinLight.intensity += priorityChangeFactor / 10;
        }
        else
            RuinLight.intensity -= Time.deltaTime ;
        

    }

    private void OnTriggerEnter(Collider other) /*HERE IS THE OPTIONS ALTERATION SCRIPT*/
    {
        bool isItem0Aviable = false, isItem1Aviable = false ;

        string Phase2RuinOption = " | L Click - Pick up";
        
        if (other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            isPlayerNearMe = true;
            if (RuinLight.intensity < 0)
                RuinLight.intensity = 0;

            if (!amIAPhase2Ruin) destroyOptionString = "F - Destroy | ";
            else destroyOptionString = ""; 

            /*(if I havent been repaired AND ( (I'm not a phase2ruin) OR (I am a Phase2Ruin AND phase2 is active) )*/
            if (!haveIBeenRepaired && ( (!amIAPhase2Ruin) || (amIAPhase2Ruin && hasPhase2Begun) )/*<- End of &&*/ )/*<- End of IF*/
            {
                for (int i = 0; i < inventoryUI.ItemsUI.Count; i++)
                {
                    //if the item in the inventory slot of the player is equal to one of the required materials of the ruin, increase counter
                    if (inventoryUI.ItemsUI[i].item == itemsINeed[0] && !isItem0Aviable)
                        isItem0Aviable = true;

                    else if (inventoryUI.ItemsUI[i].item == itemsINeed[1] && !isItem1Aviable)
                        isItem1Aviable = true;

                }

                if (isItem0Aviable == true && isItem1Aviable == true)
                {
                    canIBeRepaired = true;

                    if (itemsINeed[0].resourceName == itemsINeed[1].resourceName)
                        optionsMessageToExport = destroyOptionString + "E - Repair with 2 " + itemsINeed[0].resourceName;
                    else
                        optionsMessageToExport = destroyOptionString + "E - Repair with " + itemsINeed[0].resourceName + " and " + itemsINeed[1].resourceName;

                }
                else
                {
                    canIBeRepaired = false;

                    if (isItem0Aviable == isItem1Aviable)
                    {
                        if (itemsINeed[0].resourceName == itemsINeed[1].resourceName)
                            optionsMessageToExport = destroyOptionString + "You're missing 2 " + itemsINeed[0].resourceName + " items";

                        else
                            optionsMessageToExport = destroyOptionString + "You're missing " + itemsINeed[0].resourceName + " and " + itemsINeed[1].resourceName;
                    }
                    else
                    {
                        if (!isItem0Aviable)
                            optionsMessageToExport = destroyOptionString + "You're missing " + itemsINeed[0].resourceName;

                        else if (!isItem1Aviable)
                            optionsMessageToExport = destroyOptionString + "You're missing " + itemsINeed[1].resourceName;

                    }
                }

                if (amIAPhase2Ruin)
                    optionsMessageToExport += Phase2RuinOption;
            }
            
            else if (amIAPhase2Ruin && !hasPhase2Begun)
                optionsMessageToExport = " \"Interact the other ruins first...\"\nThat's what I read";
            
            else if (haveIBeenRepaired)
                optionsMessageToExport = "I think it says \"Thanks for fixing me\"... Besides the other thing";
        }
    }

    private void OnTriggerStay(Collider other)
    {
      
        
        //if I'm colliding with the player AND I haven't been repaired
        if (other.gameObject.GetComponent<PlayerMovement>() != null && !haveIBeenRepaired)
        {
            if (!amIAPhase2Ruin || (amIAPhase2Ruin && !amIBeingPickedUp && doIHavePriority) )
            {
                if (Input.GetKeyDown(KeyCode.E) && canIBeRepaired) //decides to repair it
                {
                    haveIBeenRepaired = true;
                    hasNotChangedColorsYet = true;
                    PlayerMovement.aRuinGotRepaired = true;
                    other.GetComponent<Inventory>().RemoveItem(itemType: itemsINeed[0].resourceName);
                    other.GetComponent<Inventory>().RemoveItem(itemType: itemsINeed[1].resourceName);

                    OnTriggerEnter(other);

                }
                else if (Input.GetKeyDown(KeyCode.F) && !amIAPhase2Ruin) //decides to destroy it AND I'm not a Phase 2 ruin
                {
                    PlayerMovement.aRuinHasBeenDestroyed = true;
                    gameObject.SetActive(false);

                    RandomGameItemCreator.GenerateRandomMaterial(transform.position.x, transform.position.y, transform.position.z);
                }
            }
            else if (amIBeingPickedUp)
            {
                hasNotChangedColorsYet = true;
            }
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>() != null)
            isPlayerNearMe = false;
    }

    public string ExportStoryText()
    {
        if (haveIBeenRepaired)
        {
            return historyWhenRepaired;
        }   
        else
        {
            return historyWhenNotRepaired;
        }
            
    }

    public void ActivateParticles()
    {
        if (!haveIBeenRepaired && !amIAPhase2Ruin)
            particleSystem.Play();
    }

    public void DeactivateParticles()
    {
        if (!amIAPhase2Ruin)
            particleSystem.Stop();
    }

}