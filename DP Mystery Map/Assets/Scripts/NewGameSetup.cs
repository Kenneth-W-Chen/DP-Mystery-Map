using System.Collections;
using System.Collections.Generic;
using PlayerInfo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameSetup : MonoBehaviour
{
    /// <summary>
    /// Image for the fade-in transition
    /// </summary>
    public Image overlay;

    public GameObject content;

    public TMP_Dropdown majorDropdown;

    public float transitionSpeed = 0.1f;
    public float targetTime = 3f;

    private static readonly Color Transparent = new Color(0, 0, 0, 0);
    
    // Start is called before the first frame update
    void Start()
    {
        content.SetActive(false);
        overlay.color = Transparent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        StartCoroutine(NewGameCoroutine());
    }

    public void StartGame()
    {
        Player.resetValues();
        Player.major = majorDropdown.value switch
        {
            0 => Major.ComputerEngineering,
            1 => Major.ComputerScience,
            2 => Major.ElectricalEngineering,
            3 => Major.MechanicalEngineering,
            _ => Player.major
        };
        //todo: set up new game 
        Debug.Log(Player.major);
    }

    private IEnumerator NewGameCoroutine()
    {
        // Play image transition
        overlay.color = Transparent;
        var targetColor = Color.black;
        var time = 0f;
        while (overlay.color != targetColor)
        {
            time += Time.deltaTime * transitionSpeed;
            /*i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / time*speed));*/
            overlay.color = Color.Lerp(overlay.color, targetColor, time/targetTime);
            yield return null;
        }
        
        // Set major prompt content to active
        content.SetActive(true);
    }
    
    private IEnumerator FadeImageColor(UnityEngine.UI.Image image, Color targetColor, float speed)
    {
        var initialColor = image.color;
        var time = 0f;
        while (image.color != targetColor)
        {
            //check in case the object gets destroyed for some reason
            //consider removing?
            if (!image)
            {
                yield break;
            }

            time += Time.deltaTime * speed;
            /*i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / time*speed));*/
            image.color = Color.Lerp(initialColor, targetColor, time);
            yield return null;
        }
    }
    
    private IEnumerator FadeImageColor(GameObject gameObject, Color targetColor, float speed/*, bool toVisible = false*/)
    {
        if (gameObject.TryGetComponent(out Image i))
        {
            /*if (!toVisible)
            {
                if (gameObject.TryGetComponent(out Button button))
                    button.interactable = false;
                i.raycastTarget = false;
            }*/
            
            var initialColor = i.color;
            var time = 0f;
            while (i.color != targetColor)
            {
                //check in case the object gets destroyed for some reason
                if (!i)
                {
                    yield break;
                }

                time += Time.deltaTime * speed;
                /*i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / time*speed));*/
                i.color = Color.Lerp(initialColor, targetColor, time);
                yield return null;
            }

            /*if (toVisible)
            {
                if (gameObject.TryGetComponent(out Button button))
                    button.interactable = true;
                i.raycastTarget = true;
            }*/
        }
    }
    
    private IEnumerator FadeTextColor(GameObject gameObject, Color targetColor, float speed)
    {
        Text t = gameObject.GetComponent<Text>();
        var initialColor = t.color;
        var time = 0f;
        while (t.color != targetColor)
        {
            //check in case the object gets destroyed for some reason
            if (!t)
            {
                yield break;
            }

            time += Time.deltaTime * speed;
            /*i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / time*speed));*/
            t.color = Color.Lerp(initialColor, targetColor, time);
            yield return null;
        }
    }
}
