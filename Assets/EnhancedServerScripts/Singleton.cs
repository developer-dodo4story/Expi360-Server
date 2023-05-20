using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Class implementing Singleton pattern, to be inherited from by other classes
    /// </summary>
    /// <typeparam name="T">
    /// Type of the class inheriting from Singleton
    /// </typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Variable used to access Singleton and its methods
        /// </summary>
        public static T instance;
        protected virtual void Awake()
        {
            if (instance == null)
                instance = (T)FindObjectOfType(typeof(T));
            else
                Destroy(gameObject);

            //DontDestroyOnLoad(gameObject);
        }
    }
}