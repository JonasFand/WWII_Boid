using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private Camera _camera;
    public LayerMask mask;
    public LayerMask entityMask;
    public List<Movement> Selected;
    private Vector3 point1;
    private Vector3 point2;
    private Vector3 checkPoint;
    [SerializeField] private BoxCollider box;

    // Start is called before the first frame update
    void Start()
    {
        point1 = Vector3.zero;
        point2 = Vector3.zero;
        checkPoint = Vector3.zero;
        Selected = new List<Movement>();
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("we here" );
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hitinfo, mask);
            Debug.Log(hitinfo.collider);
            if (hitinfo.collider)
            {
                var a = hitinfo.collider.GetComponent<Entity>();
                if (Selected.Contains(a))
                {
                    Selected.Remove(a);
                }
                else
                {
                    Selected.Add(a);
                }
            }
        }*/
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var entity in Selected)
            {
                entity.IsSelected(false);
            }
            Selected.Clear();
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hitinfo,200, mask);
            point1 = hitinfo.point;
        }

        if (Input.GetMouseButtonUp(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hitinfo, 200, mask);
            point2 = hitinfo.point;
        }

        if (point1 != Vector3.zero && point2 != Vector3.zero)
        {
            box.transform.rotation = _camera.transform.rotation;
            box.transform.position = (point1 + point2) / 2;
            var boxSize = box.size;
            boxSize.x = Mathf.Abs(box.transform.position.x - point1.x)*2;
            boxSize.y = Mathf.Abs(box.transform.position.z - point1.z)*2;
            boxSize.z = 200;
            box.size = boxSize;
            point1 = Vector3.zero;
            point2 = Vector3.zero;

            foreach (var collider in Physics.OverlapBox(box.transform.position, box.size*2, box.transform.rotation,entityMask))
            {
                Selected.Add(collider.GetComponent<Movement>());
            }
            box.size = Vector3.zero;
        }

        foreach (var entity in Selected)
        {
            entity.IsSelected(true);
        }

        if (Input.GetMouseButtonDown(1))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hitinfo,200, mask);
            checkPoint = hitinfo.point;
            foreach (var entity in Selected)
            {
                entity.CheckPoint = checkPoint;
            }
            checkPoint = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (Selected.Count>0)
        {
            foreach (var entity in Selected)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(entity.transform.position,2);
            }
        }
        Gizmos.matrix = Matrix4x4.TRS(box.transform.position, box.transform.rotation, box.transform.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, box.size);
        
    }
}
