using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private GameObject logoImagePrefab;
    [SerializeField]
    private Transform Content;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetLogoArray(Sprite[] LogoArr)
    {
        for (int i = 0; i < LogoArr.Length; i++)
        {
            AddGame(LogoArr[i]);
        }
    }
    public void AddGame(Sprite GameLogo)
    {
        //logoImage.sprite = GameLogo;
        GameObject ImageLogo = Instantiate(logoImagePrefab, Content);
        ImageLogo.GetComponent<Image>().sprite = GameLogo;
        Debug.Log("Added Game Logo");
    }


}
