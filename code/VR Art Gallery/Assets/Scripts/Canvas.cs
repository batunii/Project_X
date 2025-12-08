using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Canvas : MonoBehaviour
{
    public string imageUrl = "https://b40055af-b1ef-4764-a834-4514c762316a.client-api.unity3dusercontent.com/client_api/v1/environments/5e763078-e928-4102-bf17-44493fa27eb0/buckets/c3fe5c96-b9ee-4103-8f78-d1030d0a03f9/entries/1d7142c0-d590-49d0-baaf-5a24d5f29045/versions/1358a2c3-66e5-419a-bdcb-420920f41aed/content/";
    public Renderer targetRenderer; // Assign the plane's renderer in Inspector

    void Start()
    {
        StartCoroutine(LoadImageFromCloud());
    }

    IEnumerator LoadImageFromCloud()
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                targetRenderer.material.mainTexture = texture;
                targetRenderer.material.color = Color.white;
                targetRenderer.material.SetFloat("_Metallic", 1f);
            }
            else
            {
                Debug.LogError("Failed to load image: " + uwr.error);
            }
        }
    }
}