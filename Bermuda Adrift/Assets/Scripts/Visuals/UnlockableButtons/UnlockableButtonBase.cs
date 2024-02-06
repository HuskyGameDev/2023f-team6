using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class UnlockableButtonBase : MonoBehaviour
{
    protected Color32 myCol;
    [SerializeField] protected bool locked = true;
    [SerializeField] protected GameObject viewport;
    [SerializeField] protected GameObject descriptionField;
    protected Image iconImage;
    protected Image borderImage;
    protected Image backgroundImage;
    protected TextMeshProUGUI txt;
}
