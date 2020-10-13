using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintableWallController : MonoBehaviour
{
    [SerializeField] GameObject Player = null;
    [SerializeField] GameObject GameManager = null;
    [SerializeField] int brushSize = 30;
    [SerializeField] Color drawColor = new Color();
    public bool finished = false;
    private Color32[] colors;
    private MeshRenderer meshRend;
    private MeshCollider meshCollider; 
    private Texture2D mainTex;
    private int pixelsPainted;
    private const int ACCEPTABLE_PERC = 96;
    private void Start() {
        meshRend = GetComponent<MeshRenderer>();
        mainTex = meshRend.material.mainTexture as Texture2D;
        meshCollider = GetComponent<MeshCollider>() as MeshCollider; 
        colors = mainTex.GetPixels32();
    }
    private void Update()
    {
        if(Input.GetMouseButton(0) && Player.GetComponent<PlayerController>().canPaint && !finished) {
            GameManager.GetComponent<UIController>().paintWallPercentageSlider.gameObject.SetActive(true);
            PaintTheWall();
            CalculatePercentage();
        }
    }

    //After all brush pixels color changed from instantiated texture, used the SetPixels32 and applied
    private void PaintTheWall () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            if (hit.transform.tag == "Painting Wall") {
                //Clean wall texture can also be preferred.
                Texture2D newTex = (Texture2D)GameObject.Instantiate(mainTex);         
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= newTex.width;
                pixelUV.y *= newTex.height;

                //Brush coords
                int startX = Mathf.Clamp((int)pixelUV.x - (int)brushSize, 0, newTex.width - 1);
                int endX = Mathf.Clamp((int)pixelUV.x + (int)brushSize, 0, newTex.width - 1);
                int startY = Mathf.Clamp((int)pixelUV.y - (int)brushSize, 0, newTex.height - 1);
                int endY = Mathf.Clamp((int)pixelUV.y + (int)brushSize, 0, newTex.height - 1);

                //Changed colors between brush coords
                for (int line = startY; line < endY; line++)
                {
                    int totalPreviousLinePixels = line * newTex.width;
                    for (int pix = startX; pix < endX; pix++)
                    {
                        if (colors[totalPreviousLinePixels + pix] != drawColor) 
                            pixelsPainted++;
                        
                        colors[totalPreviousLinePixels + pix] = drawColor;
                    }
                }

                //Changed and applied cloned texture
                newTex.SetPixels32(colors);
                newTex.Apply();

                //Assinged new Texture to main texture
                meshRend.material.mainTexture = newTex;
            }
        }
    }

    private void CalculatePercentage() {
        float percentage = (pixelsPainted / (float)colors.Length) * 100;
        GameManager.GetComponent<UIController>().paintWallPercentageSlider.value = percentage;

        if (percentage > ACCEPTABLE_PERC) {
            GameManager.GetComponent<UIController>().paintWallPercentageSlider.value = 100;
            GameManager.GetComponent<UIController>().paintedTheWallText.enabled = true;
            GameManager.GetComponent<UIController>().restartButton.gameObject.SetActive(true);
            PaintAllofTheWall();
            Time.timeScale = 0;
            finished = true;
        }
    }

    private void PaintAllofTheWall() {
        Texture2D newTex = (Texture2D)GameObject.Instantiate(mainTex);         

        for (int pix = 0; pix < colors.Length; pix++)
        {
            if (colors[pix] != drawColor) 
                colors[pix] = drawColor;
        }
        newTex.SetPixels32(colors);
        newTex.Apply();
        meshRend.material.mainTexture = newTex;
    }

    // Using SetPixel to every brush pixel
    // private void PaintTheWall() {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;
    //     if(Physics.Raycast(ray, out hit)) {
    //         if (hit.transform.tag == "Painting Wall") {
    //             MeshRenderer rend = GetComponent<MeshRenderer>();
    //             MeshCollider meshCollider = GetComponent<MeshCollider>() as MeshCollider; 

    //             if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null) 
    //                 return;

    //             //Clean wall texture can also be preferred.
    //             Texture2D newTex = (Texture2D)GameObject.Instantiate(rend.material.mainTexture);              
    //             Vector2 pixelUV = hit.textureCoord;
    //             pixelUV.x *= newTex.width;
    //             pixelUV.y *= newTex.height;

    //             //brush
    //             int counter = -brushSize / 2;
    //             for (int i = 1; i < brushSize + 1; i++)
    //             {
    //                 for (int j = 1; j < brushSize + 1; j++)
    //                 {
    //                     newTex.SetPixel((int)pixelUV.x + counter + j, (int)pixelUV.y + counter + i, drawColor);
    //                 }
    //             }

    //             newTex.Apply();
    //             rend.material.mainTexture = newTex;
    //         }
    //     }
    // }


    // The plane gameObject as brush instantiate method
    // private void PaintTheWall() {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;
    //     if(Physics.Raycast(ray, out hit)) {
    //         if (hit.transform.tag == "Painting Wall") {
    //             GameObject instantiatedBrush = Instantiate(brush, hit.point + Vector3.left * .1f, Quaternion.Euler(-90, 0, 0), transform); 
    //             instantiatedBrush.transform.localScale *= brushSize;
    //         }
    //     }
    // }
}
