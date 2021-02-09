using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputField;
    [SerializeField]
    Toggle toggle;
    [SerializeField]
    private GameObject logoImagePrefab;
    [SerializeField]
    private Transform Content;

    GameObject[] logos;
    private void Start() {
        AppManager._instance.LoadingData += SetInputState;
    }

    private void SetInputState(bool state) {
        inputField.interactable = !state;
        toggle.interactable = !state;
    }

    public void GetImageArray(Sprite[] logoArr) {
        if (logos != null && logos.Length > 0)
            foreach (GameObject logo in logos)
                Destroy(logo);
        logos = new GameObject[logoArr.Length];
        for (int i = 0; i < logoArr.Length; i++)
            logos[i] = AddGame(logoArr[i]);
    }
    public GameObject AddGame(Sprite GameLogo) {
        GameObject imageLogo = Instantiate(logoImagePrefab, Content);
        imageLogo.GetComponent<Image>().sprite = GameLogo;
        Debug.Log("Added Game Logo");
        return imageLogo;
    }


}
