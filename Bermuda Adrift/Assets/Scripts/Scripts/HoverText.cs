using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HoverText : MonoBehaviour
{
    [SerializeField] GameObject descriptionBox;
    Vector2 position;
    private void OnEnable()
    {
        ButtonDescription.onMouseExitButton += disable;
        ButtonDescription.onDescriptionUpdate += updateDescription;
    }
    private void Start()
    {
        descriptionBox = Instantiate(descriptionBox, gameObject.transform);
        disable();
    }
    private void FixedUpdate()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        position.x = mouseWorldPosition.x + (30f * descriptionBox.transform.localScale.x);
        position.y = mouseWorldPosition.y - (26f * descriptionBox.transform.localScale.y);
        descriptionBox.gameObject.transform.position = position;
    }
    private void updateDescription(string title, string description)
    {
        enable();
        descriptionBox.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = title;
        descriptionBox.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = description;
    }
    public void enable() { descriptionBox.SetActive(true); }
    public void disable() { descriptionBox.SetActive(false); }
}
