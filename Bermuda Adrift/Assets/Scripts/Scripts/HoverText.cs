using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HoverText : MonoBehaviour
{
    [SerializeField] GameObject descriptionBox;
    Vector2 position;
    bool tooFarRight = false;
    bool tooFarDown = false;
    private void OnEnable()
    {
        ButtonDescription.onMouseExitButton += disable;
        ButtonDescription.onDescriptionUpdate += updateDescription;

        GameManager.onRoundEnd += disable;
    }
    private void OnDisable()
    {
        ButtonDescription.onMouseExitButton -= disable;
        ButtonDescription.onDescriptionUpdate -= updateDescription;

        GameManager.onRoundEnd -= disable;
    }
    private void Start()
    {
        descriptionBox = Instantiate(descriptionBox, gameObject.transform);
        disable();
    }
    private void FixedUpdate()
    {
        if (Camera.main.WorldToViewportPoint(Input.mousePosition).x / 38 >= 0.95)    //Moves hover text to the left of the mouse if too close to the right edge of the screen
            tooFarRight = true;
        else
            tooFarRight = false;

        if (Camera.main.WorldToViewportPoint(Input.mousePosition).y / 38 <= 0.1)
            tooFarDown = true;
        else
            tooFarDown = false;



        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        if (tooFarRight)
            position.x = mouseWorldPosition.x - (25f * descriptionBox.transform.localScale.x);
        else
            position.x = mouseWorldPosition.x + (25f * descriptionBox.transform.localScale.x);

        if (tooFarDown)
            position.y = mouseWorldPosition.y + (26f * descriptionBox.transform.localScale.y);
        else
            position.y = mouseWorldPosition.y - (26f * descriptionBox.transform.localScale.y);

        //Debug.Log(position);

        descriptionBox.gameObject.transform.position = position;
    }
    private void updateDescription(string title, string description)
    {
        if (title.CompareTo("") == 0 && description.CompareTo("") == 0) return;

        enable();

        descriptionBox.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = title;
        descriptionBox.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = description;
    }
    public void enable() { descriptionBox.SetActive(true); }
    public void disable() { descriptionBox.SetActive(false); }
}
