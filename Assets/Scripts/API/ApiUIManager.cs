using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApiUIManager : MonoBehaviour
{
    [SerializeField]
    Vector2 logosCellSize;
    [SerializeField]
    Vector2 iconsCellSize;

    [SerializeField]
    TMP_InputField inputField;
    [SerializeField]
    Toggle toggle;
    [SerializeField]
    private GameObject logoImagePrefab;
    [SerializeField]
    private Transform content;
    private GridLayoutGroup contentGrid;

    GameObject[] images;
    private void Start() {
        ApiManager._instance.LoadingData += SetInputState;
        contentGrid = content.GetComponent<GridLayoutGroup>();
    }

    private void SetInputState(bool state) {
        inputField.interactable = !state;
        toggle.interactable = !state;
    }

    public void SetImageArray(Sprite[] imageArr, bool isLogos) {
        contentGrid.cellSize = (isLogos ? logosCellSize : iconsCellSize);
        if (images != null && images.Length > 0)
            foreach (GameObject logo in images)
                Destroy(logo);
        images = new GameObject[imageArr.Length];
        for (int i = 0; i < imageArr.Length; i++)
            images[i] = AddGame(imageArr[i]);
    }
    public GameObject AddGame(Sprite GameLogo) {
        GameObject imageLogo = Instantiate(logoImagePrefab, content);
        imageLogo.GetComponent<Image>().sprite = GameLogo;
        return imageLogo;
    }


}
