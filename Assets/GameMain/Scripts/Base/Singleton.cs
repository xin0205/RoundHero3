using UnityEngine;

namespace RoundHero
{
    public abstract class Singleton<T> where T : class,new()
    {
        private readonly static object lockObj = new object();
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new T();
                        }
                    }
                }
                return instance;
            }
        }
    }
    
    public class TMonoSingleton<T> : MonoBehaviour
        where T : Component
    {
        protected static T _instance;
        public static T Instance
        {
            get{
                if(_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null){
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name ;
                        obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
    }

}