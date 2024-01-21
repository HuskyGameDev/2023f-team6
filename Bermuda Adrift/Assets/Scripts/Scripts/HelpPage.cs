using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Block", menuName = "ScriptableObjects/HelpBlock")]
public class HelpPage : ScriptableObject
{
    [SerializeField] private HelpPage[] pageBlocks;
    [SerializeField] private GameObject prefab;

    [SerializeField] private float height;

    [SerializeField] private string[] contents;
    [SerializeField] private int goToPageNumber;
    [SerializeField] private Sprite image;



    public bool hasPageBlocks() { return pageBlocks.Length == 0; }
    public HelpPage[] getPageBlocks() { return pageBlocks; }
    public GameObject getPrefab() { return prefab; }
    public float getHeight() { return height; }
    public string[] getContents() { return contents; }
    public int getGoToPageNumber() { return goToPageNumber; }
    public Sprite getImage() { return image; }
}
