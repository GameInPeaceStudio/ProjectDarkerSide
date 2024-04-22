using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PopInGames
{
    public static class Utility
    {
        public static T FindComponentInHierarchyTopDown<T>(Transform obj)
        {
            T comp = obj.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }

            foreach (Transform i in obj)
            {
                comp = FindComponentInHierarchyTopDown<T>(i);
                if (comp != null)
                {
                    return comp;
                }
            }

            return default;
        }

        public static T FindComponentInHierarchyBottomUp<T>(Transform obj, int overflowBreak = 1000)
        {
            T comp = obj.GetComponent<T>();
            overflowBreak--;
            if (overflowBreak < 0)
            {
                return default;
            }
            if (comp != null && !comp.Equals(null))
            {
                return comp;
            }
            if (obj.parent == null)
            {
                return default;
            }
            return FindComponentInHierarchyBottomUp<T>(obj.parent, overflowBreak);
        }

        public static List<T> FindComponentsInHierarchyTopDown<T>(Transform obj)
        {
            List<T> list = new List<T>();
            FindComponentsInHierarchyTopDown_Helper(obj, list);
            return list;
        }
        static void FindComponentsInHierarchyTopDown_Helper<T>(Transform obj, List<T> list)
        {
            if (list == null)
            {
                list = new List<T>();
            }

            T comp = obj.GetComponent<T>();
            if (comp != null)
            {
                list.Add(comp);
            }

            foreach (Transform i in obj)
            {
                FindComponentsInHierarchyTopDown_Helper<T>(i, list);
            }
        }

        public static T FindComponentInScene<T>()
        {
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                GameObject[] rootObjects = SceneManager.GetSceneAt(j).GetRootGameObjects();
                foreach (var i in rootObjects)
                {
                    T comp = FindComponentInHierarchyTopDown<T>(i.transform);
                    if (comp != null)
                    {
                        return comp;
                    }
                }
            }
            return default;
        }

        public static List<T> FindComponentsInScene<T>()
        {
            List<T> list = new List<T>();

            if (Application.isPlaying)
            {
                GameObject temp = new GameObject();
                Object.DontDestroyOnLoad(temp);
                UnityEngine.SceneManagement.Scene dontDestroyOnLoad = temp.scene;
                GameObject[] ddolRootObjects = dontDestroyOnLoad.GetRootGameObjects();
                for (int i = 0; i < ddolRootObjects.Length; i++)
                {
                    List<T> foundList = FindComponentsInHierarchyTopDown<T>(ddolRootObjects[i].transform);
                    list.AddRange(foundList);
                }
                Object.Destroy(temp);
            }

            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                GameObject[] rootObjects = SceneManager.GetSceneAt(j).GetRootGameObjects();
                for(int i=0;i<rootObjects.Length;i++)
                {
                    List<T> foundList = FindComponentsInHierarchyTopDown<T>(rootObjects[i].transform);
                    list.AddRange(foundList);
                }
            }
            return list;
        }
    }
}
