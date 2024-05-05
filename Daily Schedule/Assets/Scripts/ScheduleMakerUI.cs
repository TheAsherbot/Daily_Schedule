using UnityEngine;
using UnityEngine.UIElements;

public class ScheduleMakerUI : MonoBehaviour
{

    private VisualElement root;


    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
    }

}
