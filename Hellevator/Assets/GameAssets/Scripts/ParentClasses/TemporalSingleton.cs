using UnityEngine;
using System.Collections;

namespace Utils
{ 
    public class TemporalSingleton<T> : MonoBehaviour	where T : Component
    {
	    public static T Instance 
	    {
		    get 
		    {
			    if (_instance == null) 
			    {
				    _instance = FindObjectOfType<T> ();
				    if (_instance == null) 
				    {
					    GameObject obj = new GameObject ();
					    //obj.hideFlags = HideFlags.HideAndDontSave;
					    _instance = obj.AddComponent<T> ();
				    }
			    }
			    return _instance;
		    }
	    }
	
	    public virtual void Awake ()
	    {
		    if (_instance == null) 
		    {
			    _instance = this as T;
		    } 
		    else 
		    {
			    Destroy (gameObject);
		    }
	    }

        private static T _instance;
    }
}