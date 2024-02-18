using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class VideoSettings : MonoBehaviour
{
    [SerializeField] GameObject optionPrefab;

    [Header("Resolution Options")]
    [SerializeField] GameObject ResolutionContent;
    [SerializeField] TextMeshProUGUI ResolutionText;
    Resolution[] allResolutions;
    List<MyResolution> currentResolutions;
    
    int numberOfResolutions = 0;
    int currentResolutionIndex = 0;

    [Header("Aspect Ratio Options")]
    [SerializeField] GameObject AspectContent;
    [SerializeField] TextMeshProUGUI AspectText;
    float[] aspectRatios = { 4f / 3f, 16f / 9f, 16f / 10f };
    int currentAspectIndex = 0;

    void Start()
    {
        Camera.main.aspect = 16f / 9f;

        currentResolutions = new List<MyResolution>();

        InitializeAspectRatios();
        InitializeResolutions();

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            // Add Player_Prefs
        }
    }
    #region Resolutions

    public void InitializeResolutions()
    {
        allResolutions = Screen.resolutions;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            GameObject newOption;
            MyResolution newResolution = new MyResolution();

            if (Camera.main.aspect == 4f / 3f)
            {
                if ((float)(allResolutions[i].width / (float)allResolutions[i].height) == (4f / 3f))
                {
                    string option = allResolutions[i].ToString();

                    newOption = Instantiate(optionPrefab, ResolutionContent.transform);

                    ResolutionText.text = option;

                    if (allResolutions[i].width == Screen.currentResolution.width && allResolutions[i].height == Screen.currentResolution.height)
                    {
                        currentResolutionIndex = i;
                        newOption.GetComponent<Button>().Select();
                    }
                    newResolution.width = allResolutions[i].width;
                    newResolution.height = allResolutions[i].height;
                    newResolution.hz = allResolutions[i].refreshRateRatio;
                    currentResolutions.Add(newResolution);
                    numberOfResolutions++;
                }
            }

            else if (Camera.main.aspect == 16f / 9f)
            {
                if ((float)(allResolutions[i].width / (float)allResolutions[i].height) == (16f / 9f))
                {
                    string option = allResolutions[i].ToString();

                    newOption = Instantiate(optionPrefab, ResolutionContent.transform);

                    ResolutionText.text = option;

                    if (allResolutions[i].width == Screen.currentResolution.width && allResolutions[i].height == Screen.currentResolution.height)
                    {
                        currentResolutionIndex = i;
                        newOption.GetComponent<Button>().Select();
                    }

                    newResolution.width = allResolutions[i].width;
                    newResolution.height = allResolutions[i].height;
                    newResolution.hz = allResolutions[i].refreshRateRatio;
                    currentResolutions.Add(newResolution);
                    numberOfResolutions++;
                }
            }

            else if (Camera.main.aspect == 16f / 10f)
            {
                if ((float)(allResolutions[i].width / (float)allResolutions[i].height) == (16f / 10f))
                {
                    string option = allResolutions[i].ToString();

                    newOption = Instantiate(optionPrefab, ResolutionContent.transform);

                    ResolutionText.text = option;

                    if (allResolutions[i].width == Screen.currentResolution.width && allResolutions[i].height == Screen.currentResolution.height)
                    {
                        currentResolutionIndex = i;
                        newOption.GetComponent<Button>().Select();
                    }

                    newResolution.width = allResolutions[i].width;
                    newResolution.height = allResolutions[i].height;
                    newResolution.hz = allResolutions[i].refreshRateRatio;
                    currentResolutions.Add(newResolution);
                    numberOfResolutions++;
                }
            }
        }
    }

    public void DeleteResolutions()
    {
        for (int i = numberOfResolutions - 1; i >= 0; i--)
        {
            Destroy(ResolutionContent.transform.GetChild(i).gameObject);
            currentResolutions.Clear();
        }

        numberOfResolutions = 0;
    }
    public void SetResolution(int resolutionIndex)
    {
        MyResolution resolution = currentResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        string option = resolution.ToString();

        ResolutionText.text = option;

        foreach (MyResolution r in currentResolutions)
        {
            if (r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height)
            {
                ResolutionContent.transform.GetChild(resolutionIndex).GetComponent<Button>().Select();
            }
        }
    }

    public void DecreaseResolutionIndex()
    {
        currentResolutionIndex--;
        if (currentResolutionIndex < 0)
            currentResolutionIndex = numberOfResolutions - 1;
        SetResolution(currentResolutionIndex % numberOfResolutions);
    }

    public void IncreaseResolutionIndex()
    {
        currentResolutionIndex++;
        if (currentResolutionIndex > numberOfResolutions - 1)
            currentResolutionIndex = 0;
        SetResolution(currentResolutionIndex);
    }
    #endregion

    #region Aspect Ratios
    public void InitializeAspectRatios()
    {
        for (int i = 0; i < aspectRatios.Length; i++)
        {
            GameObject newOption = Instantiate(optionPrefab, AspectContent.transform);

            if (aspectRatios[i] == Camera.main.aspect)
            {
                currentAspectIndex = i;
                newOption.GetComponent<Button>().Select();
            }
        }
    }

    public void SetAspectRatio(int aspectRatioIndex)
    {
        float ratio = aspectRatios[aspectRatioIndex];
        Camera.main.aspect = ratio;

        if (ratio == 4f / 3f)
            AspectText.text = "4:3";
        else if (ratio == 16f / 9f)
            AspectText.text = "16:9";
        else if (ratio == 16f / 10f)
            AspectText.text = "16:10";
    }

    public void DecreaseAspectRatioIndex()
    {
        currentAspectIndex--;
        if (currentAspectIndex < 0)
            currentAspectIndex = aspectRatios.Length - 1;
        SetAspectRatio(currentAspectIndex);
        DeleteResolutions();
        InitializeResolutions();
    }

    public void IncreaseAspectRatioIndex()
    {
        currentAspectIndex++;
        if (currentAspectIndex > aspectRatios.Length - 1)
            currentAspectIndex = 0;
        SetAspectRatio(currentAspectIndex);
        DeleteResolutions();
        InitializeResolutions();
    }

    #endregion

    public void FullscreenOn()
    {
        Screen.fullScreen = true;
    }

    public void FullscreenOff()
    {
        Screen.fullScreen = false;
    }
}
