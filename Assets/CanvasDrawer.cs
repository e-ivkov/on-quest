using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CanvasDrawer : MonoBehaviour
{
    private bool _drawing = false;

    private Vector2 _prev;
    
    // Use this for initialization
    void Start()
    {
        GetComponent<Renderer>().material.mainTexture = new Texture2D(128, 128);
    }

    // Update is called once per frame
    void Update()
    {
        // Only when we press the mouse
        if (!Input.GetMouseButton(0))
        {
            _drawing = false;
            return;
        }

        // Only if we hit something, do we continue
        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            return;
        }

        if (!hit.collider.CompareTag("canvas")) return;

        // Just in case, also make sure the collider also has a renderer
        // material and texture. Also we should ignore primitive colliders.
        Renderer rend = hit.transform.GetComponent<Renderer>();

        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null ||
            rend.sharedMaterial.mainTexture == null || meshCollider == null)
        {
            return;
        }

        // Now draw a pixel where we hit the object
        Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord2;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;
        Vector2 v = new Vector2(Mathf.FloorToInt(pixelUV.x), Mathf.FloorToInt(pixelUV.y));

        if (_drawing)
        {
            DrawLine(tex, _prev, v, Color.black);
        }
        else
        {
            _drawing = true;
        }

        _prev = v;

        //tex.SetPixel(Mathf.FloorToInt(pixelUV.x), Mathf.FloorToInt(pixelUV.y), Color.black);

        tex.Apply();
    }
    
    public static void DrawLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col)
    {
        Vector2 t = p1;
        float frac = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
        float ctr = 0;
     
        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y) {
            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            tex.SetPixel((int)t.x, (int)t.y, col);
        }
    }

    private void OnDestroy()
    {
        var tex2d = (Texture2D) GetComponent<Renderer>().material.mainTexture;
        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", tex2d.EncodeToPNG());
    }
}