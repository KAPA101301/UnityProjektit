using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class User
{
    public int id;
    public string username;
    public string password;
    public string firstname;
    public string lastname;
    public string created;
    public string lastseen;
    public int banned;
    public int isadmin;


    public static User CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<User>(jsonString);
    }

}



public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}



public class RestClient : MonoBehaviour
{
    private string baseurl = "http://localhost/bb/api";
    private bool loggedIn = false;
    private bool objsInited = false;

    public User[] users; // array representing copy of the database (as the backend server provides)


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Login());
        //StartCoroutine(PollThreads());
        StartCoroutine(PollUsers());
    }

    private string fixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }

    // perform one-time login at the beginning of a connection
    IEnumerator Login()
    {
        string name = "kassu";
        string pass = "kassu1";
        UnityWebRequest www = UnityWebRequest.Post(baseurl + "/login", "{ \"username\": \"" + name + "\", \"password\": \"" + pass + "\" }", "application/json");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            var text = www.downloadHandler.text;
            Debug.Log(baseurl + "/login");
            Debug.Log(www.error);
            Debug.Log(text);
        }
        else
        {
            Debug.Log("Login complete!");
            loggedIn = true;
            var text = www.downloadHandler.text;
            Debug.Log(text);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // perform asynchronnous polling of threads information every X seconds after login succesfull
    IEnumerator PollThreads()
    {
        while (!loggedIn) yield return new WaitForSeconds(10); // wait for login to happen

        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(baseurl + "/threads");
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                var text = www.downloadHandler.text;
                Debug.Log(baseurl + "/threads");
                Debug.Log(www.error);
                Debug.Log(text);
            }
            else
            {
                var text = www.downloadHandler.text;
                Debug.Log("threads download complete: " + text);
                loggedIn = true;
                // TODO: handle messages JSON somwhow
            }
            yield return new WaitForSeconds(5);
        }

    }

    // perform asynchronnous polling of users information every X seconds after login succesfull
    IEnumerator PollUsers()
    {

        while (!loggedIn) yield return new WaitForSeconds(5); // wait for login to happen

        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(baseurl + "/users");
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                var text = www.downloadHandler.text;
                Debug.Log(baseurl + "/users");
                Debug.Log(www.error);
                Debug.Log(text);
            }
            else
            {
                var text = www.downloadHandler.text;
                Debug.Log("users download complete: " + text);
                loggedIn = true;
                string jsonString = fixJson(text); // add Items: in front of the json array
                users = JsonHelper.FromJson<User>(jsonString); // convert json to User-array (public users) // overwrite data each update!
                // SEE :
                // https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity/36244111#36244111

                if (!objsInited)
                {
                    GameObject userSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Vector3 position = new Vector3(2.0f, 2.0f, 0.0f);
                    float gap = 2;
                    for (int i = 0; i < users.Length; i++)
                    {
                        GameObject newObject = (GameObject)Instantiate(userSphere, position, Quaternion.identity);
                        position.x += gap;
                        newObject.name = users[i].username;

                    }
                    objsInited = true;
                }
                else
                {
                    // TODO: only update users, e.g. add new user or update changed properties of existing one, need to compare existing ones
                }
            }
            yield return new WaitForSeconds(60); // users may not update very often
        }

    }
}
