using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Cs : MonoBehaviour
{
    public TextMeshProUGUI fpsDisplay;

    private void Start()
    {
        StartCoroutine(Fps_update());
    }

    IEnumerator Fps_update()
    {
        while (true)
        {
			float fps = Mathf.Round(1 / Time.deltaTime);
			fpsDisplay.text = fps.ToString();
			yield return new WaitForSeconds(1);
		}
	}
}
