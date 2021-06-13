using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTrigger : MonoBehaviour
{
    public static bool displayingText = false;
    public static RectTransform textBox;
    [TextArea(3, 10)]
    public string dialougue;

    private void Start()
    {
        textBox = GameObject.Find("TEXTBOX").GetComponent<RectTransform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player" && !displayingText)
        {
            //Player should stop moving
            StartCoroutine(textDisplay());
        }
    }

    IEnumerator textDisplay()
    {
        displayingText = true;
        Text textInput = textBox.GetChild(0).GetComponent<Text>();
        textInput.text = "";
        for (int i = 0; i < 25; i++)
        {
            textBox.anchoredPosition += Vector2.up * 6;
            yield return null;
        }

        TextGenerator textGen = new TextGenerator();
        TextGenerationSettings textGenSettings = new TextGenerationSettings();
        textGenSettings.font = textInput.font;
        textGenSettings.fontSize = textInput.fontSize;

        foreach (string str in dialougue.Split('\n')) 
        {
            string currWords = "";
            textInput.text = "";
            bool keyPress = false;
            foreach (string word in str.Split(' '))
            {
                currWords += word + " ";
                if (textGen.GetPreferredWidth(currWords, textGenSettings) > textBox.sizeDelta.x * .645)
                {
                    textInput.text += '\n';
                    currWords = "";
                }
                foreach (char c in word)
                {
                    textInput.text += c;
                    for (float i = 0; i < .025; i += Time.deltaTime)
                    {
                        if (Input.GetKeyDown(KeyCode.Return))
                            keyPress = true;
                        yield return null;
                    }
                    if (keyPress)
                        break;
                }
                textInput.text += " ";
                for (float i = 0; i < .025; i += Time.deltaTime)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                        keyPress = true;
                    yield return null;
                }
                if (keyPress)
                    break;
            }

            textInput.text = str;
            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
            yield return null;

        }
        for (int i = 0; i < 25; i++)
        {
            textBox.anchoredPosition -= Vector2.up * 6;
            yield return null;
        }
        displayingText = false;
        Destroy(gameObject);
    }
}
