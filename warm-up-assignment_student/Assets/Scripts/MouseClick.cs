using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseClick : MonoBehaviour
{
    public Vector3 clickPosition;
    [SerializeField]
    private List<Vector3> clickedPlaces = new List<Vector3>();
    public UnityEvent<Vector3> onClick;

    void Update()
    {
        // Get the mouse click position in world space 
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
            {
                Vector3 clickWorldPosition = hitInfo.point;
                Debug.Log(clickWorldPosition);

                if (!clickedPlaces.Contains(clickWorldPosition) && clickWorldPosition != clickPosition)
                {
                    clickedPlaces.Add(clickWorldPosition);
                    clickPosition = clickWorldPosition;
                    Debug.DrawRay(clickPosition, mouseRay.origin, Color.yellow, 1);
                    DebugExtension.DebugWireSphere(clickPosition, 1, 1);
                    onClick.Invoke(clickPosition);
                }
            }
        }

    }
}
