
using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
#if !UNITY_EDITOR && UNITY_WEBGL
using System.Runtime.InteropServices;
#endif


public class ShareToTwitter : MonoBehaviour
{
    [SerializeField] private string _tweetMessage;
    private string _imgurClientId = "012c14702fea8cf";

#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string TweetFromUnity(string rawMessage);
#endif

    public void TweetWithScreenshot(string msg)
    {
        StartCoroutine(TweetWithScreenshotCo(msg));
    }

    public void ShareMessage(string msg)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        TweetFromUnity($"{msg}");
        return;
#endif
        Application.OpenURL($"https://twitter.com/intent/tweet?text={msg}");
    }

    private IEnumerator TweetWithScreenshotCo(string msg)
    {
        yield return new WaitForEndOfFrame();

        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();

        var wwwForm = new WWWForm();
        wwwForm.AddField("image", Convert.ToBase64String(tex.EncodeToJPG()));
        wwwForm.AddField("type", "base64");

        // Upload to Imgur
        UnityWebRequest www = UnityWebRequest.Post("https://api.imgur.com/3/image.xml", wwwForm);
        www.SetRequestHeader("AUTHORIZATION", "Client-ID " + _imgurClientId);

        yield return www.SendWebRequest();

        var uri = "";

        if (!www.isNetworkError)
        {
            try
            {
                XDocument xDoc = XDocument.Parse(www.downloadHandler.text);
                uri = xDoc.Element("data")?.Element("link")?.Value;

                // Remove Ext
                uri = uri?.Remove(uri.Length - 4, 4);
            }
            catch (Exception e)
            {
                uri = "";
                Debug.Log("Error:   " + e.Message);
            }
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        TweetFromUnity($"{msg}%0a{uri}");
        yield break;
#endif
        Application.OpenURL($"https://twitter.com/intent/tweet?text={msg}%0a{uri}");
        
    }
}

