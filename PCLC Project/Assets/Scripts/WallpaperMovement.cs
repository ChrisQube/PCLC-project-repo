using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WallpaperMovement : MonoBehaviour
{
    public GameObject wallpaperGameObject;
    public float fWallpaperMoveSpeed;

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        wallpaperGameObject.transform.position = new Vector2(mousePosition.x * fWallpaperMoveSpeed + 0.76f, mousePosition.y * fWallpaperMoveSpeed + 0.76f);

        //Debug.Log("x = " + mousePosition.x + "; y = " + mousePosition.y);
    }
}
