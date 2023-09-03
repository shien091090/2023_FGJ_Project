using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;

namespace SNShien.Common.DataTools
{
    public static class ReflectionManager
    {
        private static readonly List<Assembly> AssemblyStorageList = new List<Assembly>();
        private static readonly List<Type> AssemblyStorageSourceTypeList = new List<Type>();

        public static bool HaveAssemblyStorageSource(Type findType)
        {
            return AssemblyStorageSourceTypeList.Contains(findType);
        }

        public static void AddAssemblyStorage(string assemblyName)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            if (assembly != null && AssemblyStorageList.Contains(assembly) == false)
                AssemblyStorageList.Add(assembly);
        }

        public static void AddAssemblyStorage(params Type[] types)
        {
            foreach (Type t in types)
            {
                if (AssemblyStorageSourceTypeList.Contains(t) == false)
                    AssemblyStorageSourceTypeList.Add(t);

                try
                {
                    Assembly assembly = t.Assembly;

                    if (!AssemblyStorageList.Contains(assembly))
                        AssemblyStorageList.Add(assembly);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public static Type[] GetAllTypes()
        {
            if (AssemblyStorageList.Count == 0)
                AddAssemblyStorage("Assembly-CSharp");

            List<Type> resultTypes = new List<Type>();
            foreach (Assembly assembly in AssemblyStorageList)
            {
                resultTypes.AddRange(assembly.GetTypes().ToList());
            }

            resultTypes = resultTypes.Distinct().ToList();
            return resultTypes.ToArray();
        }

        public static Type[] GetInheritedTypes<T>()
        {
            List<Type> resultTypeList = new List<Type>();
            foreach (Assembly assembly in AssemblyStorageList)
            {
                List<Type> searchTypeList = assembly
                    .GetTypes()
                    .ToList();

                List<Type> matchTypeList = searchTypeList.Where(x => x.ImplementsOrInherits(typeof(T))).ToList();
                resultTypeList.AddRange(matchTypeList);
            }

            return resultTypeList.Distinct().ToArray();
        }

        public static Type[] PartialMatchSearchTypes(string searchName, int searchResultMax = 10)
        {
            List<Type> filterResult = new List<Type>();
            if (AssemblyStorageList.Count == 0)
                return filterResult.ToArray();

            List<Type> fitResult = new List<Type>();
            List<Type> similarResult = new List<Type>();
            foreach (Assembly assembly in AssemblyStorageList)
            {
                List<Type> similarTypes = assembly.GetTypes()
                    .Where(x => x.IsSubclassOf(typeof(MonoBehaviour)))
                    .Where(x => x.Name.Contains(searchName))
                    .ToList();

                if (similarTypes.Count <= 0)
                    continue;

                FillUpTypeList(similarResult, similarTypes, searchResultMax);

                List<Type> fitTypes = similarTypes
                    .Where(x => x.Name == searchName)
                    .ToList();

                if (fitTypes.Count > 0)
                    FillUpTypeList(fitResult, fitTypes, searchResultMax);
            }

            FillUpTypeList(filterResult, fitResult, searchResultMax);
            FillUpTypeList(filterResult, similarResult, searchResultMax);

            return filterResult.ToArray();
        }

        private static void FillUpTypeList(List<Type> target, List<Type> source, int maxLength)
        {
            target ??= new List<Type>();

            if (source == null || source.Count <= 0)
                return;

            foreach (Type type in source)
            {
                if (!target.Contains(type))
                    target.Add(type);

                if (target.Count >= maxLength)
                    break;
            }
        }
    }
}