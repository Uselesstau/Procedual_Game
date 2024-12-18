using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    public TMP_InputField input_seed;
	public TMP_InputField input_complexity;
	public int seed;
	public int complexity;
	private void Awake()
	{
        DontDestroyOnLoad(gameObject);
	}
	public void OnPress()
    {
		if (input_seed.text != "")
        {
			try
            {
                seed = int.Parse(input_seed.text);
			}
            catch
            {
				seed = Random.Range(0, 100000);
			}
        }
        else
        {
			seed = Random.Range(0, 100000);
		}
		if (input_complexity.text != "")
		{
			try
			{
				complexity = int.Parse(input_complexity.text);
			}
			catch
			{
				complexity = 5;
			}
		}
		else
		{
			complexity = 5;
		}
		SceneManager.LoadScene("MainGame");
    }
}
