using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;

    RaycastHit hitInfo;

    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;

    public Texture2D target, attack;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector3(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector3(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClicked.Invoke(hitInfo.point);
            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked.Invoke(hitInfo.collider.gameObject);
            }
        }
    }
}
