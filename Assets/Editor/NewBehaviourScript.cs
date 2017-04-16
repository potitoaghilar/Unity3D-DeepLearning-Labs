using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawGameObjectName(Transform transform, GizmoType gizmoType)
    {
        if (transform.tag == "neuron")
        {
            GUI.color = Color.yellow;
            Handles.Label(transform.position + new Vector3(0, 2, 0), GameObject.Find("Lab").GetComponent<Lab>().gController.getBrain(0).n_values[int.Parse(transform.gameObject.name)].ToString());
            GUI.color = Color.white;
        }
    }
}
