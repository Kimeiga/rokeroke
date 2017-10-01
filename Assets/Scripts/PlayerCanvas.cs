using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour {

    public static PlayerCanvas canvas;

    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI speedText;

    public GameObject panel1;
    public GameObject panel2;

    private void Awake()
    {
        if (canvas == null)
            canvas = this;
        else if (canvas != this)
            Destroy(gameObject);

    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetAmmo(int amount)
    {
        ammoText.text = amount.ToString() + "a";
    }
    public void SetHealth(float amount)
    {
        healthText.text = amount.ToString() + "h";
    }
    public void SetKills(int amount)
    {
        killsText.text = amount.ToString() + "k";
    }
    public void SetSpeed(float amount)
    {
        speedText.text = amount.ToString("F1") + " m / s";

    }

    public void EnableCanvas()
    {
        panel1.SetActive(true);
        panel2.SetActive(true);
    }

    public void DisableCanvas()
    {
        panel1.SetActive(false);
        panel2.SetActive(false);
    }
}
