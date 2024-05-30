using UnityEngine;


/*
 A static instance is like a singleton but instead of destroying any new instances,
it overrides the current instance. Usefull for resetting the state
 */
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/*
 Basic singleton: Destroy any new version created.
 */
public abstract class Singleton<T> : StaticInstance<T> where T: MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        base.Awake();
    }
}


/*
 Will survive scene load
 Perfect for system classes, persistent data, audio source between scenes...
 
 */
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}

