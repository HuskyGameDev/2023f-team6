using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HelpBook : MonoBehaviour
{
    [SerializeField] private HelpPage allPages;
    private HelpPage currentPage;

    [SerializeField] private float leftPageX;
    [SerializeField] private float startY;
    [SerializeField] private float rightPageX;
    [SerializeField] private float yFloor;

    private Vector3 Origin;

    private void Start()
    {
        Origin = new Vector3(leftPageX, startY);
        goToPage(1);
    }

    public void goToPage(int pageNumber)
    {
        resetPages();

        Origin.y = startY;

        if (pageNumber <= 0)
            pageNumber = 1;
        if (pageNumber > allPages.getPageBlocks().Length)
            pageNumber = allPages.getPageBlocks().Length;

        currentPage = allPages.getPageBlocks()[pageNumber - 1];

        foreach (HelpPage block in currentPage.getPageBlocks())
        {
            loadBlock(pageNumber, block);
        }

        //Once the given page is loaded, load the other page that would show up
        if (pageNumber % 2 == 0)
            pageNumber--;   //Right page was given, so load left
        else
            pageNumber++;   //Left page was given, so load right

        if (pageNumber > allPages.getPageBlocks().Length)    //Could end on a left page, so just don't load anything for the right page
            return;

        //Shouldn't be able to go to negative numbers/page 0 because the check at the start would set anything that low to 1, and then that would be recognized as the left page, so it'd load the right page next, not the one before

        currentPage = allPages.getPageBlocks()[pageNumber - 1];
        Origin.y = startY;

        foreach (HelpPage block in currentPage.getPageBlocks())
        {
            loadBlock(pageNumber, block);
        }
    }

    public void loadBlock(int pageNumber, HelpPage block)
    {
        string prefabName = block.getPrefab().name;

        if (pageNumber % 2 == 1)
            Origin.x = leftPageX;
        else
            Origin.x = rightPageX;

        if (Origin.y - block.getHeight() <= yFloor)
        {
            Debug.Log(prefabName + " goes too far down");
            return;
        }

        if (prefabName.CompareTo("TableOfContents") == 0)
            loadTableOfContents(block);

        else if (prefabName.CompareTo("EmptySpace") == 0)
            loadEmptySpace(block.getHeight());

        else if (prefabName.CompareTo("TitleTextBox") == 0)
            loadTitleText(block);

        else if (prefabName.CompareTo("BigTextbox") == 0)
            loadTextbox(block);

        else if (prefabName.CompareTo("NextPage") == 0 || prefabName.CompareTo("PrevPage") == 0)
            loadNextPageButton(block, pageNumber);

        else if (prefabName[2..].CompareTo("PicDescription") == 0)
            loadPicDescription(block);

        else
            loadPrefabRaw(block);
    }

    private void loadEmptySpace(float height)
    {
        Origin.y -= height;
    }
    private void loadTableOfContents(HelpPage block)
    {
        GameObject mainCanvas;
        Origin.y -= block.getHeight() / 2f;

        mainCanvas = Instantiate(block.getPrefab(), transform);
        mainCanvas.GetComponent<RectTransform>().anchoredPosition = Origin;

        mainCanvas.GetComponent<Button>().onClick.AddListener(() => goToPage(block.getGoToPageNumber()));
        mainCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = block.getContents()[0];
        mainCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = block.getGoToPageNumber().ToString();

        Origin.y -= block.getHeight() / 2f;
    }
    private void loadTitleText(HelpPage block)
    {
        GameObject mainCanvas;
        Origin.y -= block.getHeight() / 2f;

        mainCanvas = Instantiate(block.getPrefab(), transform);
        mainCanvas.GetComponent<RectTransform>().anchoredPosition = Origin;

        mainCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = block.getContents()[0];

        Origin.y -= block.getHeight() / 2f;
    }
    private void loadTextbox(HelpPage block)
    {
        GameObject mainCanvas;
        Origin.y -= block.getHeight() / 2f;

        mainCanvas = Instantiate(block.getPrefab(), transform);
        mainCanvas.GetComponent<RectTransform>().anchoredPosition = Origin;

        mainCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = block.getContents()[0];

        Origin.y -= block.getHeight() / 2f;
    }
    private void loadPicDescription(HelpPage block)
    {
        GameObject mainCanvas;
        Origin.y -= block.getHeight() / 2f;

        mainCanvas = Instantiate(block.getPrefab(), transform);
        mainCanvas.GetComponent<RectTransform>().anchoredPosition = Origin;

        mainCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = block.getContents()[0];
        mainCanvas.transform.GetChild(1).GetComponent<Image>().sprite = block.getImage();

        Origin.y -= block.getHeight() / 2f;
    }
    private void loadNextPageButton(HelpPage block, int pgNum)
    {
        int destination;
        if (pgNum % 2 == 1) //if setting up the left page, assume it's going back a page
            destination = pgNum - 1;
        else
            destination = pgNum + 1;    //Otherwise assume going to the next page


        GameObject mainCanvas;
        Origin.y -= block.getHeight() / 2f;

        mainCanvas = Instantiate(block.getPrefab(), transform);
        mainCanvas.GetComponent<RectTransform>().anchoredPosition = Origin;

        mainCanvas.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => goToPage(destination));
        mainCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = pgNum.ToString();

        Origin.y -= block.getHeight() / 2f;
    }
    private void loadPrefabRaw(HelpPage block)
    {
        GameObject mainCanvas;
        Origin.y -= block.getHeight() / 2f;

        mainCanvas = Instantiate(block.getPrefab(), transform);
        mainCanvas.GetComponent<RectTransform>().anchoredPosition = Origin;

        Origin.y -= block.getHeight() / 2f;
    }

    public void resetPages()
    {
        for (int i = 3; i < transform.childCount; i++)  //Constants are the book itself, the close button, and book grid
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
