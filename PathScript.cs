using UnityEngine;

public class PathScript : MonoBehaviour
{
    // setting up color and size options for drawing our path gizmos in the scene view
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private float gizmoRadius = 0.2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // drawing a small sphere at each child checkpoint position so i can see the path layout
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentChild = transform.GetChild(i);
            Gizmos.DrawWireSphere(currentChild.position, gizmoRadius);

            // drawing lines to connect the current checkpoint to the next one in order
            if (i < transform.childCount - 1)
            {
                Transform nextChild = transform.GetChild(i + 1);
                Gizmos.DrawLine(currentChild.position, nextChild.position);
            }
        }
    }
}