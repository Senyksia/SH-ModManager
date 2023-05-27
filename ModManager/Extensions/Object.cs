using System.Linq;
using UnityEngine;

namespace ModManager.Extensions
{
    internal static class ObjectHelper
    {
        /// <summary>
        /// Instantiates a <see cref="GameObject"/> without instantiating its children.
        /// </summary>
        /// <param name="original">The GameObject to instantiate.</param>
        /// <param name="parent">The parent to attach the instantiated object to.</param>
        /// <returns>
        /// The instantiated clone.
        /// </returns>
        internal static GameObject InstantiateWithoutChildren(GameObject original, Transform parent = null)
        {
            Transform[] children = original.transform.Cast<Transform>().ToArray();

            foreach (Transform child in children) { child.SetParent(null, false); }
            GameObject instance = Object.Instantiate(original, parent);
            foreach (Transform child in children) { child.SetParent(original.transform, false); }

            return instance;
        }
    }
}
