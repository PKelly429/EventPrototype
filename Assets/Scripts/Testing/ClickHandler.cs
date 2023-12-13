using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    [SerializeField] private Camera camera;

    private RaycastHit hitInfo;
    
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        bool mouseDown = Input.GetMouseButtonDown(0);

        if (!mouseDown) return;
        if (Physics.Raycast(ray, out hitInfo))
        {
            var clickableObject = hitInfo.transform.GetComponent<ClickableObject>();
            if (clickableObject != null)
            {
                clickableObject.HandleClick();
            }
        }
    }
}
