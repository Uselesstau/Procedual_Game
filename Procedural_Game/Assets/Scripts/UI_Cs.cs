using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Cs : MonoBehaviour
{
    public TextMeshProUGUI fps_display;

    private void Start()
    {
        StartCoroutine(Fps_update());
    }

	void Update()
    {
        

    }

    IEnumerator Fps_update()
    {
        while (true)
        {
			float fps = 1 / Time.deltaTime;
			fps_display.text = fps.ToString();
			yield return new WaitForSeconds(1);
		}
	}
}
