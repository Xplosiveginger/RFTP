using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));

                if (FindObjectsOfType(typeof(T)).Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                                   " - there should never be more than 1 singleton!" +
                                   " Reopening the scene might fix it.");
                    return _instance;
                }
            }

            return _instance;

        }
    }

}
