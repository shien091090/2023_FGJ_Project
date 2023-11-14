using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace SCGLobby
{
    public class ActivityJsonParser
    {
        public bool TryDeserializeObject<T>(string json, out T obj)
        {
            T result = default;
            bool isParseSuccess = true;

            if (string.IsNullOrEmpty(json))
            {
                obj = result;
                return false;
            }

            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Debug.Log($"[ActivityJsonParser] DeserializeObject Failed, Log = {e}");
                isParseSuccess = false;
            }

            obj = result;
            return isParseSuccess;
        }

        public bool TrySerializeObject<T>(T obj, out string json)
        {
            if (obj == null)
            {
                json = string.Empty;
                return false;
            }

            try
            {
                json = JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                json = string.Empty;
                Debug.Log($"[ActivityJsonParser] Json SerializeObject Failed, Log = {e}");
                return false;
            }

            return true;
        }

        public bool TryParseMultipleLayerJObject(string json, out JObject jObj, params string[] fieldKeys)
        {
            jObj = null;

            if (string.IsNullOrEmpty(json) || fieldKeys == null || fieldKeys.Length == 0)
            {
                Debug.Log($"[ActivityJsonParser] Parse JObject Failed : Input Is Empty");
                return false;
            }

            try
            {
                jObj = JObject.Parse(json);
                foreach (string fieldKey in fieldKeys)
                {
                    if (jObj[fieldKey] == null)
                    {
                        Debug.Log($"[ActivityJsonParser] Parse JObject Failed : Not Conatin fieldKey '{fieldKey}', json = {json}");
                        return false;
                    }

                    jObj = JObject.Parse(jObj[fieldKey].ToString());
                }
            }
            catch (Exception e)
            {
                Debug.Log($"[ActivityJsonParser] Parse JObject Failed, Log = {e}");
                return false;
            }

            return jObj != null;
        }
    }
}