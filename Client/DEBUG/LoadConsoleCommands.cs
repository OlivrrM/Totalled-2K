using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Totalled;
using UnityEditor;
/*
public class SafeContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        property.ShouldSerialize = instance =>
        {
            try
            {
                var value = property.ValueProvider.GetValue(instance);
                // Try to access the value to see if it throws
                return true;
            }
            catch
            {
                return false;
            }
        };

        return property;
    }
}
*/
public class SafeValueProvider : IValueProvider
{
    private readonly IValueProvider _innerProvider;

    public SafeValueProvider(IValueProvider innerProvider)
    {
        _innerProvider = innerProvider;
    }

    public object GetValue(object target)
    {
        try
        {
            return _innerProvider.GetValue(target);
        }
        catch
        {
            return "<b>???</b>"; // Replace problematic value with "???"
        }
    }

    public void SetValue(object target, object value)
    {
        _innerProvider.SetValue(target, value);
    }
}

// Custom ContractResolver that uses SafeValueProvider
public class SafeContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        property.ValueProvider = new SafeValueProvider(property.ValueProvider);

        return property;
    }
}
public class LoadConsoleCommands : MonoBehaviour
{
    public LayerMask getTargetLayermask;
    public static GameObject[] branchObjectsCache;
    public static Terminal terminal;

    JsonSerializerSettings UnityEngineTypeSerializerSettings;

    public static KeyValuePair<Component,FieldInfo> copiedPtr;

    public GameObject freecam; // This should be accessed by code
    GameObject currentFreecam;
    private void Awake()
    {
        Cache.references = gameObject.GetComponent<References>();
        terminal = gameObject.GetComponent<Terminal>();
        UnityEngineTypeSerializerSettings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new SafeContractResolver()
        };
    }
    RaycastHit GetTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(Cache.vcam.transform.position, Cache.vcam.transform.forward, out hit, 999999f, getTargetLayermask)) { }
        return hit;
    }
    GameObject GetName(string name)
    {
        return GameObject.Find(name.Replace('-', ' '));
    }

    [TerminalCommand("font", "font [size] - Changes the size of console font to given value\n")]
    public string Font(int fontSize)
    {
        TerminalGUI.terminalFontSize = fontSize;
        string returnString = "Set terminal GUI font size to " + fontSize.ToString();
        ReloadTerminalGUI();
        return returnString;
    }
    [TerminalCommand("reloadlegacyconsolegui", "Reloads console GUI\n")]
    public string ReloadTerminalGUI()
    {
        string returnString = "Attempting to reload terminal GUI. . .";
        try
        {
            terminal.ReloadGUI();
            returnString = "\nReloaded terminal GUI";
        }
        catch
        {
            returnString = "\nFailed to reloaded terminal GUI";
        }
        return returnString;
    }
    [TerminalCommand("references", "references [list || category] - Lists identifiers for target category. 'list' will list all categories\n")]
    [ICC.IccCommand("references","Debug", "references [list || category] - Lists identifiers for target category. 'list' will list all categories\n")]
    public string Reference(string category)
    {
        var dictionaries = new List<Utilities.DictionaryInfo>();
        try
        {
            dictionaries = Utilities.GetDictionariesInClass(Cache.references);
        }
        catch
        {
            return "Internal error fetching reference dictionaries";
        }

        string output = "";
        int itemsFound = 0;
        bool dictFound = false;
        switch (category)
        {
            case "list":
                output = "List of each reference category:\n";
                foreach (var dict in dictionaries)
                {
                    output += $"{dict.Name}\n";
                }
                return output;

            default:
                foreach (var dict in dictionaries)
                {
                    if (dict.Name.Equals(category))
                    {
                        System.Type dictType = typeof(Dictionary<,>).MakeGenericType(dict.KeyType, dict.ValueType);
                        var dictionary = dict.Dictionary;
                        if (dictionary != null && dictionary.GetType().IsGenericType && (dictionary.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                        {
                            dictFound = true;
                            output += "List of each identifier in " + category + ":\n";
                            var enumeratorMethod = dictionary.GetType().GetMethod("GetEnumerator");
                            var enumerator = (System.Collections.IEnumerator)enumeratorMethod.Invoke(dictionary, null);
                            while (enumerator.MoveNext())
                            {
                                var current = enumerator.Current;
                                var key = current.GetType().GetProperty("Key").GetValue(current, null);
                                var value = current.GetType().GetProperty("Value").GetValue(current, null);
                                output += $"{key}\n";
                                itemsFound++;
                            }
                            break;
                        }
                        else
                        {
                            return $"Reference category '{category}' is not readable";
                        }
                    }
                }
                break;
        }
        if (dictFound)
        {
            if (itemsFound > 0)
            {
                return output;
            }
            else
            {
                return $"Reference category '{category}' contained no items";
            }
        }
        else { return $"Unknown reference category '{category}'"; }
    }
    public object[] GetInvokePassingParameters(string[] parameterStrings, ParameterInfo[] methodParameters, string[] specifiedCasts)
    {
        object[] passingParameters = new object[parameterStrings.Length];
        for (int i = 0; i < parameterStrings.Length; i++)
        {
            switch (methodParameters[i].ParameterType.Name)
            {
                case "String":
                    passingParameters[i] = parameterStrings[i].Substring(1, parameterStrings[i].Length - 2);
                    break;
                case "Int":
                    passingParameters[i] = int.Parse(parameterStrings[i].Substring(1, parameterStrings[i].Length - 2));
                    break;
                case "Int16":
                    passingParameters[i] = System.Int16.Parse(parameterStrings[i].Substring(1, parameterStrings[i].Length - 2));
                    break;
                case "Int32":
                    passingParameters[i] = System.Int32.Parse(parameterStrings[i].Substring(1, parameterStrings[i].Length - 2));
                    break;
                case "Int64":
                    passingParameters[i] = System.Int64.Parse(parameterStrings[i].Substring(1, parameterStrings[i].Length - 2));
                    break;
                case "Single":
                    passingParameters[i] = float.Parse(parameterStrings[i].Substring(1, parameterStrings[i].Length - 2));
                    break;
                case "Boolean":
                    passingParameters[i] = bool.Parse(parameterStrings[i].Substring(1, parameterStrings[i].Length - 2));
                    break;
                case "Object":
                    if (specifiedCasts[i] != string.Empty)
                    {
                        passingParameters[i] = System.Convert.ChangeType(parameterStrings[i].Substring(1, parameterStrings[i].Length - 2), System.Type.GetType(specifiedCasts[i]));
                    }
                    else
                    {
                        passingParameters[i] = parameterStrings[i].Substring(1, parameterStrings[i].Length - 2);
                    }
                    break;
                default:
                    passingParameters[i] = JsonConvert.DeserializeObject(parameterStrings[i], specifiedCasts[i] != string.Empty ? System.Type.GetType(specifiedCasts[i]) : methodParameters[i].ParameterType);
                    //passingParameters[i] = JsonUtility.FromJson(parameterStrings[i], specifiedCasts[i] != string.Empty ? System.Type.GetType(specifiedCasts[i]) : methodParameters[i].ParameterType);
                    break;
            }
        }
        return passingParameters;
    }
    [TerminalCommand("invoke", "invoke [name || target || this || null] [component] [method] [arguments || null] - Invokes target method within target component within target gameobject with given parameters. Parameters must be in JSON format. Primitive types must also be enclosed with curly brackets\n")]
    [ICC.IccCommand("invoke","Debug", "invoke [name || target || this || null] [component] [method] [arguments || null] - Invokes target method within target component within target gameobject with given parameters. Parameters must be in JSON format. Primitive types must also be enclosed with curly brackets\n")]
    public string InvokeMethod(string target, string component, string method, string parameters)
    {
        GameObject targetGO;
        switch (target)
        {
            case "target":
                targetGO = GetTarget().transform.gameObject;
                break;
            case "this":
                targetGO = Cache.surfCharacter.gameObject;
                break;
            default:
                targetGO = GetName(target);
                break;
        }
        if (targetGO == null) { return $"Gameobject '{target}' does not exist within current instance or is disabled"; }
        System.Type componentType = Utilities.GetType(component);
        if (componentType != null)
        {
            Component targetComponent = targetGO.GetComponent(componentType);
            if (targetComponent != null)
            {
                List<MethodInfo> methodInfos = targetComponent.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList<MethodInfo>();
                int FORCEBREAK = 20;
                System.Type baseType = componentType.BaseType;
                while (true)
                {
                    if (baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                    {
                        MethodInfo[] baseTypeMethodInfos = baseType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        for (int x = 0; x < baseTypeMethodInfos.Length; x++)
                        {
                            bool notOverrideDuplicate = true;
                            for (int y = 0; y < methodInfos.Count; y++)
                            {
                                if (methodInfos[y].Name == baseTypeMethodInfos[x].Name)
                                {
                                    notOverrideDuplicate = false;
                                    break;
                                }
                            }
                            if (notOverrideDuplicate) { methodInfos.Add(baseTypeMethodInfos[x]); }
                        }
                        baseType = baseType.BaseType;
                    }
                    else { break; }


                    FORCEBREAK--;
                    if (FORCEBREAK < 0) { break; }
                }

                MethodInfo methodInfo = null;
                for (int z = 0; z < methodInfos.Count; z++)
                {
                    if (methodInfos[z].Name == method) { methodInfo = methodInfos[z]; }
                }
                //MethodInfo methodInfo = componentType.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                if (methodInfo != null)
                {
                    try
                    {
                        if (parameters == "null") { methodInfo.Invoke(methodInfo.IsStatic ? null : targetComponent, null); }
                        else
                        {
                            string pattern = @"(\(([^)]+)\))?(\{(?:[^{}]|(?<open>\{)|(?<-open>\}))*\}(?(open)(?!)))";
                            MatchCollection matches = Regex.Matches(parameters, pattern);
                            string[] parameterStrings = new string[matches.Count];
                            string[] specifiedCasts = new string[matches.Count];
                            for (int i = 0; i < matches.Count; i++)
                            {
                                var match = matches[i].Groups[3].Value;
                                var bracketMatch = matches[i].Groups[2].Value;
                                parameterStrings[i] = match.Trim();
                                specifiedCasts[i] = bracketMatch.Trim();
                            }


                            ParameterInfo[] methodParameters = methodInfo.GetParameters();
                            object[] passingParameters = GetInvokePassingParameters(parameterStrings, methodParameters, specifiedCasts);

                            methodInfo.Invoke(methodInfo.IsStatic ? null : targetComponent, passingParameters);
                            return $"Invoked method '{method}' within component '{component}' within gameobject '{targetGO.name}'";
                        }
                    }
                    catch (System.Exception e) { return "Error invoking method: " + e.Message + "\n<b>" + e.StackTrace + "</b>"; }
                    return $"Invoked method '{method}' within component '{component}' within gameobject '{targetGO.name}'";
                }
                else { return $"Unknown method '{method}' within '{component}'"; }
            }
            else { return $"Target component does not derive from Component base class"; }
        }
        else { return $"Component '{component}' does not exist within '{targetGO.name}' or is disabled"; }
    }
    [TerminalCommand("set", "set [name || target || this] [component] [variable] [value] - Sets target variable within target component within target gameobject to given value\n")]
    [ICC.IccCommand("set","Debug", "set [name || target || this] [component] [variable] [value] - Sets target variable within target component within target gameobject to given value\n")]
    public string SetVariableValue(string target, string component, string variable, string value)
    {
        GameObject targetGO;
        switch (target)
        {
            case "target":
                targetGO = GetTarget().transform.gameObject;
                break;
            case "this":
                targetGO = Cache.surfCharacter.gameObject;
                break;
            default:
                targetGO = GetName(target);
                break;
        }
        if (targetGO == null) { return $"Gameobject '{target}' does not exist within current instance or is disabled"; }
        System.Type componentType = Utilities.GetType(component);
        if (componentType != null)
        {
            FieldInfo fieldInfo = componentType.GetField(variable, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

            int FORCEBREAK = 20;
            System.Type baseType = componentType.BaseType;
            if (fieldInfo == null)
            {
                while (true)
                {
                    if (baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                    {
                        FieldInfo[] baseTypeFieldInfos = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        for (int x = 0; x < baseTypeFieldInfos.Length; x++)
                        {
                            if (variable == baseTypeFieldInfos[x].Name)
                            {
                                fieldInfo = baseTypeFieldInfos[x];
                                break;
                            }
                        }
                        baseType = baseType.BaseType;
                    }
                    else { break; }


                    FORCEBREAK--;
                    if (FORCEBREAK < 0) { break; }
                }
            }

            if (fieldInfo == null) { return $"Unknown variable '{variable}' within component '{component}'"; }
            Component targetComponent = targetGO.GetComponent(componentType);
            if (targetComponent != null)
            {
                object setValue = null;
                if (value != null)
                {
                    System.Type fieldType = fieldInfo.FieldType;
                    switch (fieldType)
                    {
                        case System.Type when fieldType == typeof(string):
                            setValue = value;
                            break;
                        case System.Type when fieldType == typeof(int):
                            int outInt = 0;
                            if (int.TryParse(value, out outInt)) { setValue = outInt; }
                            else { return $"Value '{value}' failed to cast as type '{fieldType.Name}'"; }
                            break;
                        case System.Type when fieldType == typeof(System.Int16):
                            System.Int16 outInt16 = 0;
                            if (System.Int16.TryParse(value, out outInt16)) { setValue = outInt16; }
                            else { return $"Value '{value}' failed to cast as type '{fieldType.Name}'"; }
                            break;
                        case System.Type when fieldType == typeof(System.Int32):
                            System.Int32 outInt32 = 0;
                            if (System.Int32.TryParse(value, out outInt32)) { setValue = outInt32; }
                            else { return $"Value '{value}' failed to cast as type '{fieldType.Name}'"; }
                            break;
                        case System.Type when fieldType == typeof(System.Int64):
                            System.Int64 outInt64 = 0;
                            if (System.Int64.TryParse(value, out outInt64)) { setValue = outInt64; }
                            else { return $"Value '{value}' failed to cast as type '{fieldType.Name}'"; }
                            break;
                        case System.Type when fieldType == typeof(float):
                            float outFloat = 0f;
                            if (float.TryParse(value, out outFloat)) { setValue = outFloat; }
                            else { return $"Value '{value}' failed to cast as type '{fieldType.Name}'"; }
                            break;
                        case System.Type when fieldType == typeof(bool):
                            bool outBool = false;
                            if (bool.TryParse(value, out outBool)) { setValue = outBool; }
                            else { return $"Value '{value}' failed to cast as type '{fieldType.Name}'"; }
                            break;
                        case System.Type when fieldType == typeof(object):
                            setValue = value;
                            break;
                        default:
                            setValue = JsonConvert.DeserializeObject(value, fieldType);
                            //setValue = JsonUtility.FromJson(value, fieldType);
                            break;
                    }
                    fieldInfo.SetValue(targetComponent, setValue);
                    return $"Set variable '{fieldInfo.Name}' within component '{targetComponent}' within gameobject '{targetGO.name}'";
                }
                else { fieldInfo.SetValue(targetComponent, null); return $"Set variable '{fieldInfo.Name}' within component '{targetComponent}' within gameobject '{targetGO.name}' to '{value}'"; }
            }
            else { return $"Target component does not derive from Component base class"; }
        }
        else { return $"Component '{component}' does not exist within '{targetGO.name}' or is disabled"; }
    }
    public class ComponentFieldInfo
    {
        FieldInfo fieldInfo;
        System.Type type;
    }
    [TerminalCommand("get", "get [name || target || this] [component] [variable] - Returns value of target variable within target component within target gameobject\n")]
    [ICC.IccCommand("get","Debug", "get [name || target || this] [component] [variable] - Returns value of target variable within target component within target gameobject\n")]
    public string GetVariableValue(string target, string component, string variable)
    {
        GameObject targetGO;
        switch (target)
        {
            case "target":
                targetGO = GetTarget().transform.gameObject;
                break;
            case "this":
                targetGO = Cache.surfCharacter.gameObject;
                break;
            default:
                targetGO = GetName(target);
                break;
        }
        if (targetGO == null) { return $"Gameobject '{target}' does not exist within current instance or is disabled"; }
        System.Type componentType = Utilities.GetType(component);
        if (componentType != null)
        {
            try
            {
                FieldInfo fieldInfo = componentType.GetField(variable, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                Component targetComponent = targetGO.GetComponent(componentType);

                int FORCEBREAK = 20;
                System.Type baseType = componentType.BaseType;
                if (fieldInfo == null)
                {
                    while (true)
                    {
                        if (baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                        {
                            FieldInfo[] baseTypeFieldInfos = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                            for (int x = 0; x < baseTypeFieldInfos.Length; x++)
                            {
                                if (variable == baseTypeFieldInfos[x].Name)
                                {
                                    fieldInfo = baseTypeFieldInfos[x];
                                    break;
                                }
                            }
                            baseType = baseType.BaseType;
                        }
                        else { break; }


                        FORCEBREAK--;
                        if (FORCEBREAK < 0) { break; }
                    }
                }

                if (fieldInfo == null) { return $"Unknown variable '{variable}' within component '{component}'"; }
                if (targetComponent != null)
                {
                    try ///Big issue with serializing certain UnityEngine fields which can crash client and editor instantly.
                    {
                        if (fieldInfo.FieldType == typeof(int) || fieldInfo.FieldType == typeof(float) || fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType == typeof(bool)) { return $"<{fieldInfo.FieldType}> {fieldInfo.GetValue(targetComponent)}"; }
                        else { return $"<{fieldInfo.FieldType}> { JsonConvert.SerializeObject(fieldInfo.GetValue(targetComponent), UnityEngineTypeSerializerSettings) }"; }//JsonUtility.ToJson(fieldInfo.GetValue(targetComponent))}"; }
                    }
                    catch (System.Exception e)
                    {
                        return $"Error serializing desired object: {e.Message}";
                    }
                }
                else { return $"Target component does not derive from Component base class"; }
            }
            catch (System.Exception e)
            {
                return $"Error handling target variable: {e.Message}";
            }
        }
        else { return $"Component '{component}' does not exist within '{targetGO.name}' or is disabled"; }
    }
    [TerminalCommand("vars", "vars [name || target || this] [component] - Lists all variables within target component within target gameobject\n")]
    [ICC.IccCommand("vars", "Debug", "vars [name || target || this] [component] - Lists all variables within target component within target gameobject\n")]
    public string GetComponentVariables(string target, string component)
    {
        GameObject targetGO;
        switch (target)
        {
            case "target":
                targetGO = GetTarget().transform.gameObject;
                break;
            case "this":
                targetGO = Cache.surfCharacter.gameObject;
                break;
            default:
                targetGO = GetName(target);
                break;
        }
        if (targetGO == null) { return $"Gameobject '{target}' does not exist within current instance or is disabled"; }
        System.Type componentType = Utilities.GetType(component);
        if (componentType != null)
        {
            List<FieldInfo> fieldInfos = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList<FieldInfo>();
            int FORCEBREAK = 20;
            System.Type baseType = componentType.BaseType;
            while (true)
            {
                if (baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                {
                    FieldInfo[] baseTypeFieldInfos = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    for (int x = 0; x < baseTypeFieldInfos.Length; x++)
                    {
                        bool notOverrideDuplicate = true;
                        for (int y = 0; y < fieldInfos.Count; y++)
                        {
                            if (fieldInfos[y].Name == baseTypeFieldInfos[x].Name)
                            {
                                notOverrideDuplicate = false;
                                break;
                            }
                        }
                        if (notOverrideDuplicate) { fieldInfos.Add(baseTypeFieldInfos[x]); }
                    }
                    baseType = baseType.BaseType;
                }
                else { break; }


                FORCEBREAK--;
                if (FORCEBREAK < 0) { break; }
            }
            string returnString = "";
            for (int i = 0; i < fieldInfos.Count; i++) {
                returnString += $"<{fieldInfos[i].FieldType.Name}> {fieldInfos[i].Name}\n";
                //returnString += $"{(fieldInfos[i].DeclaringType!=componentType?$"({fieldInfos[i].DeclaringType}) ":"")}<{fieldInfos[i].FieldType.Name}> {fieldInfos[i].Name}\n";
            }
            return returnString;
        }
        else { return $"Component '{component}' does not exist within '{targetGO.name}' or is disabled"; }
    }


    [TerminalCommand("give", "give [type] [identifier * quantity] - Gives given item of given type. Specify quantity instead of an identifier if type is stackable\n")]
    [ICC.IccCommand("give","Debug", "give [type] [identifier * quantity] - Gives given item of given type. Specify quantity instead of an identifier if type is stackable\n")]
    public string Give(string type, string identifier)
    {
        switch (type)
        {
            case "item":
                if (Cache.references.items.ContainsKey(identifier))
                {
                    GameObject.Find("Inventory").GetComponent<Inventory>().PickupItem(Cache.references.items[identifier]);
                    return "Received item '" + identifier + "'";
                }
                else
                {
                    if (identifier == "all")
                    {
                        Inventory inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
                        foreach (KeyValuePair<string, GameObject> item in Cache.references.items){
                            inventory.PickupItem(item.Value);
                        }
                    }
                    return "Unknown identifier '" + identifier + "'";
                }
                break;
            case "ammo":
                int quantity = 0;
                try { quantity = int.Parse(identifier); }
                catch { return "Ammo is stackable. Specify quantity instead of an identifier"; }
                Cache.ammo.ChangeAmmo(quantity);
                return "Received " + quantity.ToString() + " ammo";
            case "grenade":
                int quantity1 = 0;
                try { quantity1 = int.Parse(identifier); }
                catch { return "Grenade is stackable. Specify quantity instead of an identifier"; }
                Cache.grenadeManager.PickupGrenade(quantity1, true);
                if (quantity1 > 1) { return "Received " + quantity1.ToString() + " grenades"; }
                else { return "Received grenade"; }
            default:
                return "Unknown type '" + type + "'";
        }
        return "null";
    }
    [TerminalCommand("kill", "kill [target || name || this] - Kills target entity\n")]
    [ICC.IccCommand("kill", "Debug", "kill [target || name || this] - Kills target entity\n")]
    public string Kill(string target)
    {
        switch (target)
        {
            case "this":
                Health playerHealth = GameObject.Find("Health").GetComponent<Health>();
                playerHealth.Damage(new Damage { amount = playerHealth.maxHealth, type = Totalled.DamageType.Unknown });
                return "Committed suicide";
            case "target":
                RaycastHit hit = GetTarget();
                if (hit.transform != null)
                {
                    IDamageable damageable = hit.transform.gameObject.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        Damage damage = new Damage();
                        damage.amount = 9999999f;
                        damage.type = Totalled.DamageType.Unknown;
                        damageable.Damage(damage);
                        return "Killed '" + hit.transform.name + "'";
                    }
                    else
                    {
                        return "'" + hit.transform.name + "' is not damageable";
                    }
                }
                return "No target cast";
            default:
                GameObject targetGO = GetName(target);
                if (targetGO != null)
                {
                    IDamageable damageable = GameObject.Find(target).GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        Damage damage = new Damage();
                        damage.amount = 9999999f;
                        damage.type = Totalled.DamageType.Unknown;
                        damageable.Damage(damage);
                        return "Killed '" + target + "'";
                    }
                    else
                    {
                        return "'" + target + "' is not damageable";
                    }
                }
                else
                {
                    return $"Gameobject '{target}' does not exist within current instance or is disabled";
                }
                return "null";
        }
    }
    [TerminalCommand("damage", "damage [target || name || this] [amount] - Damages target entity with given amount of damage\n")]
    [ICC.IccCommand("damage", "Debug", "damage [target || name || this] [amount] - Damages target entity with given amount of damage\n")]
    public string Damage(string target, string amount)
    {
        float amountf = 0f;
        if (float.TryParse(amount, out amountf))
        {
            switch (target)
            {
                case "this":
                    Health playerHealth = GameObject.Find("Health").GetComponent<Health>();
                    playerHealth.Damage(new Damage { amount = amountf, type = Totalled.DamageType.Unknown });
                    return $"Damaged Richard Lincoln for {amountf} health";
                case "target":
                    RaycastHit hit = GetTarget();
                    if (hit.transform != null)
                    {
                        IDamageable damageable = hit.transform.gameObject.GetComponent<IDamageable>();
                        if (damageable != null)
                        {
                            Damage damage = new Damage();
                            damage.amount = amountf;
                            damage.type = Totalled.DamageType.Unknown;
                            damageable.Damage(damage);
                            return "Damaged '" + hit.transform.name + "' for "+amountf;
                        }
                        else
                        {
                            return "'" + hit.transform.name + "' is not damageable";
                        }
                    }
                    return "No target cast";
                default:
                    GameObject targetGO = GetName(target);
                    if (targetGO != null)
                    {
                        IDamageable damageable = GameObject.Find(target).GetComponent<IDamageable>();
                        if (damageable != null)
                        {
                            Damage damage = new Damage();
                            damage.amount = amountf;
                            damage.type = Totalled.DamageType.Unknown;
                            damageable.Damage(damage);
                            return "Damaged '" + target + "' for "+amountf;
                        }
                        else
                        {
                            return "'" + target + "' is not damageable";
                        }
                    }
                    else
                    {
                        return $"Gameobject '{target}' does not exist within current instance or is disabled";
                    }
                    return "null";
            }
        }
        else
        {
            return "Amount needs to be a number";
        }
    }
    [TerminalCommand("heal", "heal [target || name || this] [amount] - Heals target entity for given amount of health\n")]
    [ICC.IccCommand("heal", "Debug", "heal [target || name || this] [amount] - Heals target entity for given amount of health\n")]
    public string Heal(string target, string amount)
    {
        float amountf = 0f;
        if (float.TryParse(amount, out amountf))
        {
            switch (target)
            {
                case "this":
                    Health playerHealth = GameObject.Find("Health").GetComponent<Health>();
                    playerHealth.Heal(new Heal { amount = amountf, type = Totalled.HealType.Consumable });
                    return $"Healed Richard Lincoln for {amountf} health";
                case "target":
                    RaycastHit hit = GetTarget();
                    if (hit.transform != null)
                    {
                        IDamageable damageable = hit.transform.gameObject.GetComponent<IDamageable>();
                        if (damageable != null)
                        {
                            Damage damage = new Damage();
                            damage.amount = -amountf;
                            damage.type = Totalled.DamageType.Unknown;
                            damageable.Damage(damage);
                            return "Damaged '" + hit.transform.name + "' for " + amountf;
                        }
                        else
                        {
                            return "'" + hit.transform.name + "' is not damageable";
                        }
                    }
                    return "No target cast";
                default:
                    GameObject targetGO = GetName(target);
                    if (targetGO != null)
                    {
                        IDamageable damageable = GameObject.Find(target).GetComponent<IDamageable>();
                        if (damageable != null)
                        {
                            Damage damage = new Damage();
                            damage.amount = -amountf;
                            damage.type = Totalled.DamageType.Unknown;
                            damageable.Damage(damage);
                            return "Damaged '" + target + "' for " + amountf;
                        }
                        else
                        {
                            return "'" + target + "' is not damageable";
                        }
                    }
                    else
                    {
                        return $"Gameobject '{target}' does not exist within current instance or is disabled";
                    }
                    return "null";
            }
        }
        else
        {
            return "Amount needs to be a number";
        }
    }
    [TerminalCommand("spawn", "spawn [identifier] [x,y,z || this || target] - Spawns given entity at target location\n")]
    [ICC.IccCommand("spawn", "Debug", "spawn [identifier] [x,y,z || this || target] - Spawns given entity at target location\n")]
    public string Spawn(string identifier, string position)
    {
        GameObject spawnedObject;
        if (Cache.references.spawnables.ContainsKey(identifier))
        {
            spawnedObject = Cache.references.spawnables[identifier];
        }
        else if (Cache.references.doodads.ContainsKey(identifier))
        {
            spawnedObject = Cache.references.doodads[identifier];
        }
        else
        {
            spawnedObject = GetName(identifier);
            if (spawnedObject == null) { return "Unknown identifier '" + identifier + "'"; }
        }

        switch (position)
        {
            case "this":
                Instantiate(spawnedObject, Cache.surfCharacter.transform.position, Quaternion.identity);
                return "Spawned '" + identifier + "' at " + Cache.surfCharacter.transform.position;
            case "target":
                RaycastHit hit = GetTarget();
                if (hit.transform != null)
                {
                    Instantiate(spawnedObject, hit.point, Quaternion.identity);
                    return "Spawned '" + identifier + "' at " + hit.point;
                }
                else
                {
                    return "No target cast";
                }
            default:
                if (position.Contains(","))
                {
                    string[] coordinates = position.Split(',');
                    Vector3 targetPos = Vector3.zero;
                    try { targetPos = new Vector3(float.Parse(coordinates[0]), float.Parse(coordinates[1]), float.Parse(coordinates[2])); }
                    catch { return "Given coordinates are not of numeric values"; }
                    Instantiate(spawnedObject, targetPos, Quaternion.identity);
                    return "Spawned '" + identifier + "' at " + targetPos;
                }
                else { return "Invalid location method"; }
        }
        return null;
    }
    [TerminalCommand("unpack","unpack [packageName] - Unpacks given package\n")]
    [ICC.IccCommand("unpack", "Debug", "unpack [packageName] - Unpacks given package\n")]
    public string Unpack(string packageName)
    {
        if (!packageName.Contains(".zip")) { packageName += ".zip"; }
        Cache.packageLoader.LoadPackage(packageName);
        return $"Attempted to unpack '{packageName}'";
    }
    [TerminalCommand("unloadpackagecontent", "unloadpackagecontent [packageName] - Unloads package content\n")]
    [ICC.IccCommand("unloadpackagecontent", "Debug", "unloadpackagecontent [packageName] - Unloads package content\n")]
    public string UnloadPackageContent(string packageName)
    {
        if (packageName.Contains(".zip")) { packageName.Remove(packageName.Length - 4); }
        Cache.packageLoader.UnloadPackageContent(packageName);
        return $"Attempted to unload package '{packageName}'";
    }
    [TerminalCommand("reloadcommandsfromdisk","Reloads all .t2kc files from Run folder\n")]
    [ICC.IccCommand("reloadcommandsfromdisk", "Debug", "Reloads all .t2kc files from Run folder\n")]
    public string ReloadCommandsFromDisk()
    {
        Cache.runCommandOnLevel.LoadCommands();
        return "Reloaded commands from disk";
    }

    [TerminalCommand("resources", "resources [load || unload] [resourceName] - Loads or unloads given resource from external resources\n")]
    [ICC.IccCommand("resources", "Debug", "resources [load || unload] [resourceName] - Loads or unloads given resource from external resources\n")]
    public string Resources(string action,string resourceName)
    {
        switch (action)
        {
            case "load":
                return ExternalResourcesManager.LoadAssetBundle(resourceName, resourceName);
            case "unload":
                return ExternalResourcesManager.UnloadAssetBundle(resourceName);
            default:
                return $"Unknown action {action}";
        }
    }
    //[TerminalCommand("buildassetbundle","buildassetbundle [exportDirectory] - Builds assetbundle")]
    [TerminalCommand("loadprefab","loadprefab [resourceName] [prefabName] - Loads given prefab from given loaded resource\n")]
    [ICC.IccCommand("loadprefab", "Debug", "loadprefab [resourceName] [prefabName] - Loads given prefab from given loaded resource\n")]
    public string LoadPrefab(string resourceName, string prefabName)
    {
        return ExternalResourcesManager.LoadPrefabAsset(resourceName, prefabName);
    }
    [TerminalCommand("unloadprefab", "unloadprefab [prefabName] - Unloads given prefab\n")]
    [ICC.IccCommand("unloadprefab", "Debug", "unloadprefab [prefabName] - Unloads given prefab\n")]
    public string UnloadPrefab(string prefabName)
    {
        return ExternalResourcesManager.UnloadPrefabAsset(prefabName);
    }
    [TerminalCommand("initialize", "initialize [gamesystem] - Initializes given gamesystem\n")]
    [ICC.IccCommand("initialize", "Debug", "initialize [gamesystem] - Initializes given gamesystem\n")]
    public string Initialize(string gamesystem)
    {
        if (Cache.references.gamesystems.ContainsKey(gamesystem))
        {
            Instantiate(Cache.references.gamesystems[gamesystem]);
            return $"Initialized {gamesystem}";
        }
        else
        {
            return $"Unknown gamesystem {gamesystem}";
        }
    }
    [TerminalCommand("instantiate", "instantiate [identifier] [x,y,z || this || target] [name] [parent] - Spawns given entity at target location with specified name, as a child of target parent\n")]
    [ICC.IccCommand("instantiate", "Debug", "instantiate [identifier] [x,y,z || this || target] [name] [parent] - Spawns given entity at target location with specified name, as a child of target parent\n")]
    public string Instantiate(string identifier, string position, string name, string parent)
    {
        GameObject spawnedObject;
        if (Cache.references.spawnables.ContainsKey(identifier))
        {
            spawnedObject = Cache.references.spawnables[identifier];
        }
        else if (Cache.references.doodads.ContainsKey(identifier))
        {
            spawnedObject = Cache.references.doodads[identifier];
        }
        else if (Cache.references.externallyLoadedPrefabs.ContainsKey(identifier))
        {
            spawnedObject = Cache.references.externallyLoadedPrefabs[identifier];
        }
        else
        {
            spawnedObject = GetName(identifier);
            if (spawnedObject == null) { return "Unknown identifier '" + identifier + "'"; }
        }

        Transform parentTranform;
        switch (parent)
        {
            case "target":
                parentTranform = GetTarget().transform;
                break;
            case "this":
                parentTranform = Cache.surfCharacter.transform;
                break;
            case "null":
                parentTranform = null;
                break;
            default:
                parentTranform = GetName(parent).transform;
                break;
        }
        GameObject instantiatedGO;
        switch (position)
        {
            case "this":
                instantiatedGO = Instantiate(spawnedObject, Cache.surfCharacter.transform.position, Quaternion.identity, parentTranform);
                instantiatedGO.name = name;
                return "Spawned '" + identifier + "' with name '" + name + "' at " + Cache.surfCharacter.transform.position;
            case "target":
                RaycastHit hit = GetTarget();
                if (hit.transform != null)
                {
                    instantiatedGO = Instantiate(spawnedObject, hit.point, Quaternion.identity, parentTranform);
                    instantiatedGO.name = name;
                    return "Instantiated '" + identifier + "' with name '" + name + "' at " + hit.point;
                }
                else
                {
                    return "No target cast";
                }
            default:
                if (position.Contains(","))
                {
                    string[] coordinates = position.Split(',');
                    Vector3 targetPos = Vector3.zero;
                    try { targetPos = new Vector3(float.Parse(coordinates[0]), float.Parse(coordinates[1]), float.Parse(coordinates[2])); }
                    catch { return "Given coordinates are not of numeric values"; }
                    instantiatedGO = Instantiate(spawnedObject, targetPos, Quaternion.identity, parentTranform);
                    instantiatedGO.name = name;
                    return "Spawned '" + identifier + "' with name '" + name + "' at " + targetPos;
                }
                else { return "Invalid location method"; }
        }
        return null;
    }
    [TerminalCommand("duplicate", "duplicate [name || target] [x,y,z || this || target || identity] - Duplicates given gameobject at target location\n")]
    [ICC.IccCommand("duplicate", "Debug", "duplicate [name || target] [x,y,z || this || target || identity] - Duplicates given gameobject at target location\n")]
    public string Duplicate(string identifier, string position)
    {
        GameObject spawnedObject;
        if (identifier == "target")
        {
            spawnedObject = GetTarget().transform.gameObject;
            if (spawnedObject == null) { return "No target cast"; }
        }
        else
        {
            spawnedObject = GetName(identifier);
            if (spawnedObject == null) { return $"Gameobject '{identifier}' does not exist within current instance or is disabled"; }
        }

        switch (position)
        {
            case "this":
                Instantiate(spawnedObject, Cache.surfCharacter.transform.position, Quaternion.identity);
                return "Spawned '" + identifier + "' at " + Cache.surfCharacter.transform.position;
            case "target":
                RaycastHit hit = GetTarget();
                if (hit.transform != null)
                {
                    Instantiate(spawnedObject, hit.point, Quaternion.identity);
                    return "Spawned '" + identifier + "' at " + hit.point;
                }
                else
                {
                    return "No target cast";
                }
            case "identity":
                Instantiate(spawnedObject);
                return "Spawned '" + identifier + "' at " + spawnedObject.transform.position;
            default:
                if (position.Contains(","))
                {
                    string[] coordinates = position.Split(',');
                    Vector3 targetPos = Vector3.zero;
                    try { targetPos = new Vector3(float.Parse(coordinates[0]), float.Parse(coordinates[1]), float.Parse(coordinates[2])); }
                    catch { return "Given coordinates are not of numeric values"; }
                    Instantiate(spawnedObject, targetPos, Quaternion.identity);
                    return "Spawned '" + identifier + "' at " + targetPos;
                }
                else { return "Invalid location method"; }
        }
        return null;
    }
    [TerminalCommand("destroy", "destroy [target || name || this] - Destroys target gameobject\n")]
    [ICC.IccCommand("destroy", "Debug", "destroy [target || name || this] - Destroys target gameobject\n")]
    public string Destroy(string target)
    {
        switch (target)
        {
            case "target":
                RaycastHit hit = GetTarget();
                if (hit.transform != null)
                {
                    Destroy(hit.transform.gameObject);
                    return "Destroyed target gameobject";
                }
                else
                {
                    return "No target cast";
                }
                break;
            case "this":
                Destroy(Cache.surfCharacter.gameObject);
                return "Destroyed Richard Lincoln";
                break;
            default:
                GameObject targetGO = GetName(target);
                if (targetGO != null)
                {
                    Destroy(targetGO);
                    return "Destroyed '" + target + "'";
                }
                else
                {
                    return $"Gameobject '{target}' does not exist within current instance or is disabled";
                }
                break;
        }
    }
    [TerminalCommand("showspeed", "showspeed - Enables/disables speed counter\n")]
    [ICC.IccCommand("showspeed", "Debug", "showspeed - Enables/disables speed counter\n")]
    public string ShowSpeed()
    {
        Cache.speedCounter.gameObject.SetActive(!Cache.speedCounter.gameObject.active);
        return (Cache.speedCounter.gameObject.active ? "Enabled " : "Disabled ") + "speed counter";
    }
    [TerminalCommand("showspread", "showspread - Enables/disables spread counter\n")]
    [ICC.IccCommand("showspread", "Debug", "showspread - Enables/disables spread counter\n")]
    public string ShowSpread()
    {
        Cache.spreadCounter.gameObject.SetActive(!Cache.spreadCounter.gameObject.active);
        return (Cache.spreadCounter.gameObject.active ? "Enabled " : "Disabled ") + "spread counter";
    }
    [TerminalCommand("name", "name - Names target gameobject\n")]
    [ICC.IccCommand("name", "Debug", "name - Names target gameobject\n")]
    public string Name()
    {
        RaycastHit hit = GetTarget();
        if (hit.transform != null)
        {
            return $"'{hit.transform.gameObject.name}'";
        }
        else
        {
            return "No target cast";
        }
    }
    [TerminalCommand("rename", "rename [name || target] [name] - Renames target gameobject\n")]
    [ICC.IccCommand("rename", "Debug", "rename [name || target] [name] - Renames target gameobject\n")]
    public string Rename(string target, string name)
    {
        GameObject targetGO;
        switch (target)
        {
            case "target":
                targetGO = GetTarget().transform.gameObject;
                break;
            default:
                targetGO = GetName(target);
                break;
        }
        string oldName = targetGO.name;
        if (targetGO == null) { return $"Gameobject '{target}' does not exist within current instance or is disabled"; }
        else { targetGO.name = name; return $"Renamed '{oldName}' to '{name}'"; }
    }
    [TerminalCommand("branch", "branch [name || target || this || instance] - Returns names of each child within target gameobject or within currently loaded instance\n")]
    [ICC.IccCommand("branch", "Debug", "branch [name || target || this || instance] - Returns names of each child within target gameobject or within currently loaded instance\n")]
    public string Branch(string name)
    {
        string returnString = "";
        if (name == "target")
        {
            RaycastHit hit = GetTarget();
            if (hit.transform != null)
            {
                Transform target = hit.transform;
                if (target.childCount == 0) { return "'" + name + "' does not contain any children"; }
                else { branchObjectsCache = new GameObject[target.childCount]; }
                for (int i = 0; i < target.childCount; i++)
                {
                    returnString += i + ") " + target.GetChild(i).gameObject.name;
                    if (target.GetChild(i).childCount > 0) { returnString += " {" + target.GetChild(i).childCount + "}"; }
                    returnString += "\n";
                    branchObjectsCache[i] = target.GetChild(i).gameObject;
                }
            }
            else { return "No target cast"; }
        }
        else if (name == "this")
        {
            Transform target = Cache.surfCharacter.transform;
            if (target.childCount == 0) { return "'" + name + "' does not contain any children"; }
            else { branchObjectsCache = new GameObject[target.childCount]; }
            for (int i = 0; i < target.childCount; i++)
            {
                returnString += i + ") " + target.GetChild(i).gameObject.name;
                if (target.GetChild(i).childCount > 0) { returnString += " {" + target.GetChild(i).childCount + "}"; }
                returnString += "\n";
                branchObjectsCache[i] = target.GetChild(i).gameObject;
            }
        }
        else if (name != "instance")
        {
            int idCheck = 0;
            Transform target = null;
            try
            {
                idCheck = int.Parse(name);
                if (idCheck <= branchObjectsCache.Length)
                {
                    target = branchObjectsCache[idCheck].transform;
                }
                else
                {
                    return "Invalid branch ID\nIf target name is a single number, start name with '~'";
                }
            }
            catch
            {
                name = name.Replace("-", " ");
                name = name.Replace("~", "");
                try { target = GetName(name).transform; }
                catch { return "Gameobject '" + name + "' does not exist within current instance or is disabled"; }
                if (target == null) { return "Gameobject '" + name + "' does not exist within current instance or is disabled"; }
            }
            if (target.childCount == 0) { return "'" + name + "' does not contain any children"; }
            else { branchObjectsCache = new GameObject[target.childCount]; }
            for (int i = 0; i < target.childCount; i++)
            {
                returnString += i + ") " + target.GetChild(i).gameObject.name;
                if (target.GetChild(i).childCount > 0) { returnString += " {" + target.GetChild(i).childCount + "}"; }
                returnString += "\n";
                branchObjectsCache[i] = target.GetChild(i).gameObject;
            }
        }
        else
        {
            GameObject[] objects = SceneManager.GetActiveScene().GetRootGameObjects();
            if (objects.Length == 0) { return "Current instance does not contain any children. Where are you?"; }
            else { branchObjectsCache = new GameObject[objects.Length]; }
            for (int i = 0; i < objects.Length; i++)
            {
                returnString += i + ") " + objects[i].name;
                if (objects[i].transform.childCount > 0) { returnString += " {" + objects[i].transform.childCount + "}"; }
                returnString += "\n";
                branchObjectsCache[i] = objects[i];
            }
        }
        return returnString;
    }
    [TerminalCommand("tree","tree [instanceName] - Returns a tree of given instance hierarchy\n")]
    [ICC.IccCommand("tree", "Debug", "tree [instanceName] - Returns a tree of given instance hierarchy\n")]
    public string Tree(string instanceName)
    {
        /*
        if (instanceName == "all"){
            string printString = "";
            for (int i = 0; i < SceneManager.loadedSceneCount; i++){
                printString += $"\n<b>{SceneManager.GetSceneByBuildIndex(i).name}</b>\n{Utilities.GetSceneHierarchy(SceneManager.GetSceneByBuildIndex(i))}\n";
                return printString;
            }
            return "No instances loaded";
        }*/
        Scene givenScene = SceneManager.GetSceneByName(instanceName);
        if (instanceName == "this") { givenScene = SceneManager.GetActiveScene(); }
        if (givenScene == null) { return $"'{instanceName}' is not a currently loaded instance"; }
        return $"\n<b>{givenScene.name}</b>\n{Utilities.GetSceneHierarchy(givenScene)}";
    }
    [TerminalCommand("transform", "transform [name || target] [position || rotation || scale] [set || move] [x] [y] [z] - Either sets or moves the position,rotation or scale of target gameobject with given values\n")]
    [ICC.IccCommand("transform", "Debug", "transform [name || target] [position || rotation || scale] [set || move] [x] [y] [z] - Either sets or moves the position,rotation or scale of target gameobject with given values\n")]
    public string Transform(string name, string value, string action, float x, float y, float z)
    {
        name = name.Replace("-", " ");
        Transform target = null;
        if (name == "target")
        {
            RaycastHit hit = GetTarget();
            if (hit.transform != null) { target = hit.transform; }
            else { return "No target cast"; }
        }
        else
        {
            try { target = GetName(name).transform; }
            catch { return "Gameobject '" + name + "' does not exist within current instance or is disabled"; }
        }
        if (target == null) { return "Gameobject '" + name + "' does not exist within current instance or is disabled"; }
        switch (value)
        {
            case "position":
                switch (action)
                {
                    case "set":
                        target.position = new Vector3(x, y, z);
                        break;
                    case "move":
                        target.position += new Vector3(x, y, z);
                        break;
                    default:
                        return "'" + action + "' is not a valid action";
                }
                break;
            case "rotation":
                switch (action)
                {
                    case "set":
                        target.eulerAngles = new Vector3(x, y, z);
                        break;
                    case "move":
                        target.eulerAngles += new Vector3(x, y, z);
                        break;
                    default:
                        return "'" + action + "' is not a valid action";
                }
                break;
            case "scale":
                switch (action)
                {
                    case "set":
                        target.localScale = new Vector3(x, y, z);
                        break;
                    case "move":
                        target.localScale += new Vector3(x, y, z);
                        break;
                    default:
                        return "'" + action + "' is not a valid action";
                }
                break;
            default:
                return "'" + value + "' is not a value within Transform class";
        }
        if (action == "move") { return string.Format("Changed {0} of '{1}' by ({2},{3},{4})", value, name, x, y, z); }
        else { return string.Format("Set {0} of '{1}' to ({2},{3},{4})", value, name, x, y, z); }
    }
    [TerminalCommand("component", "component [add || remove || enable || disable] [componentName] [name || target || this] - Adds, removes, enables or disables target component from target gameobject\n")]
    [ICC.IccCommand("component", "Debug", "component [add || remove || enable || disable] [componentName] [name || target || this] - Adds, removes, enables or disables target component from target gameobject\n")]
    public string Component(string action, string component, string target)
    {
        Transform targetGO;
        switch (target)
        {
            case "target":
                targetGO = GetTarget().transform;
                break;
            case "this":
                targetGO = Cache.surfCharacter.transform;
                break;
            default:
                targetGO = GetName(target).transform;
                break;
        }
        if (targetGO == null) { return $"Gameobject '{target}' does not exist within current instance or is disabled"; }
        switch (action)
        {
            case "add":
                System.Type componentType = Utilities.GetType(component);
                if (componentType != null && (componentType.IsSubclassOf(typeof(Component)) || componentType.IsSubclassOf(typeof(MonoBehaviour)))) {
                    targetGO.gameObject.AddComponent(componentType);
                    return $"Added component '{component}' to '{targetGO.name}'";
                }
                else {
                    return $"Unknown component '{component}'";
                }
                break;
            case "remove":
                if (true)
                {
                    System.Type type = Utilities.GetType(component);
                    if (type == null) { return $"'{component}' does not exist within '{targetGO.name}'"; }
                    Component targetComponent = targetGO.GetComponent(type);
                    if (targetComponent == null) { return $"'{component}' does not exist within '{targetGO.name}'"; }
                    Destroy(targetComponent);
                    return $"Removed '{component}' from '{targetGO.name}'";
                }
                break;
            case "enable":
                if (true)
                {
                    System.Type type = Utilities.GetType(component);
                    if (type == null) { return $"'{component}' does not exist within '{targetGO.name}'"; }
                    Component targetComponent = targetGO.GetComponent(type);
                    if (targetComponent == null) { return $"'{component}' does not exist within '{targetGO.name}'"; }
                    var enabledProperty = type.GetProperty("enabled");
                    if (enabledProperty != null && enabledProperty.PropertyType == typeof(bool))
                    {
                        enabledProperty.SetValue(targetComponent, true);
                        return $"Enabled component '{component}' within '{targetGO.name}'";
                    }
                    else { return $"'{component}' cannot be enabled"; }
                }
            case "disable":
                if (true)
                {
                    System.Type type = Utilities.GetType(component);
                    if (type == null) { return $"'{component}' does not exist within '{targetGO.name}'"; }
                    Component targetComponent = targetGO.GetComponent(type);
                    if (targetComponent == null) { return $"'{component}' does not exist within '{targetGO.name}'"; }
                    var enabledProperty = type.GetProperty("enabled");
                    if (enabledProperty != null && enabledProperty.PropertyType == typeof(bool))
                    {
                        enabledProperty.SetValue(targetComponent, false);
                        return $"Disable component '{component}' within '{targetGO.name}'";
                    }
                    else { return $"'{component}' cannot be disabled"; }
                }
        }

        return "";
    }

    [TerminalCommand("parent", "parent [name || target] [name || target || this] - Sets target gameobjects parent to target gameobject\n")]
    [ICC.IccCommand("parent", "Debug", "parent [name || target] [name || target || this] - Sets target gameobjects parent to target gameobject\n")]
    public string SetParent(string target, string targetParent)
    {
        Transform targetGO;
        switch (target)
        {
            case "target":
                targetGO = GetTarget().transform;
                break;
            case "this":
                targetGO = Cache.surfCharacter.transform;
                break;
            default:
                targetGO = GetName(target).transform;
                break;
        }
        if (targetGO == null) { return $"Gameobject '{target}' does not exist within current instance or is disabled"; }
        Transform parentGO;
        switch (targetParent)
        {
            case "target":
                parentGO = GetTarget().transform;
                break;
            case "this":
                parentGO = Cache.surfCharacter.transform;
                break;
            default:
                parentGO = GetName(target).transform;
                break;
        }
        //if (parentGO == null) { return $"Gameobject '{targetParent}' does not exist within current instance or is disabled"; }
        targetGO.parent = parentGO;
        if (parentGO == null) { return $"'{targetGO.name}' no longer has a parent"; }
        else { return $"'{targetGO.name}' is now a child of '{parentGO.name}'"; }
    }

    [TerminalCommand("info", "info [name || target || this] [position || rotation || scale || components] - Returns info on target gameobjects target info\n")]
    [ICC.IccCommand("info", "Debug", "info [name || target || this] [position || rotation || scale || components] - Returns info on target gameobjects target info\n")]
    public string GameObjectInfo(string name, string value)
    {
        name = name.Replace("-", " ");
        Transform target = null;
        if (name == "target")
        {
            RaycastHit hit = GetTarget();
            if (hit.transform != null) { target = hit.transform; }
            else { return "No target cast"; }
        }
        else if (name == "this") { target = Cache.surfCharacter.transform; }
        else
        {
            try { target = GetName(name).transform; }
            catch { return "Gameobject '" + target.name + "' does not exist within current instance or is disabled"; }
        }
        if (target == null) { return "Gameobject '" + target.name + "' does not exist within current instance or is disabled"; }
        switch (value)
        {
            case "position":
                return "The " + value + " of '" + target.name + "' is " + target.position;
            case "rotation":
                return "The " + value + " of '" + target.name + "' is " + target.eulerAngles;
            case "scale":
                return "The " + value + " of '" + target.name + "' is " + target.localScale;
            case "components":
                string text = "";
                int index = -1;
                foreach (var component in target.gameObject.GetComponents(typeof(Component)))
                {
                    index++;
                    text += index + ") " + component.GetType().ToString() +$"{(component is Behaviour?(((Behaviour)component).enabled?"":" (disabled)"):"")}\n";
                }
                return text;
            default:
                return "Value '" + value + "' is not a valid option";
        }
    }
    [TerminalCommand("methodinfo", "methodinfo [name || target || this] [component] - Returns a list of methods within target gameobjects target component\n")]
    [ICC.IccCommand("methodinfo", "Debug", "methodinfo [name || target || this] [component] - Returns a list of methods within target gameobjects target component\n")]
    public string MethodInfo(string target, string component)
    {
        GameObject targetGO;
        switch (target)
        {
            case "target":
                targetGO = GetTarget().transform.gameObject;
                break;
            case "this":
                targetGO = Cache.surfCharacter.gameObject;
                break;
            default:
                targetGO = GetName(target);
                break;
        }
        if (targetGO == null) { return $"Gameobject '{target}' does not exist within current instance or is disabled"; }
        //System.Type componentType = System.Type.GetType(component);
        System.Type componentType = Utilities.GetType(component);
        if (componentType != null)
        {
            Component targetComponent = targetGO.GetComponent(componentType);
            if (targetComponent != null)
            {
                List<MethodInfo> methodInfos = targetComponent.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList<MethodInfo>();
                int FORCEBREAK = 20;
                System.Type baseType = componentType.BaseType;
                while (true)
                {
                    if (baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                    {
                        MethodInfo[] baseTypeMethodInfos = baseType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        for (int x = 0; x < baseTypeMethodInfos.Length; x++) {
                            bool notOverrideDuplicate = true;
                            for (int y = 0; y < methodInfos.Count; y++) {
                                if (methodInfos[y].Name == baseTypeMethodInfos[x].Name) {
                                    notOverrideDuplicate = false;
                                    break;
                                }
                            }
                            if (notOverrideDuplicate) { methodInfos.Add(baseTypeMethodInfos[x]); }
                        }
                        baseType = baseType.BaseType;
                    }
                    else { break; }


                    FORCEBREAK--;
                    if (FORCEBREAK < 0) { break; }
                }
                if (methodInfos.Count == 0) { return $"Component '{component}' has no obtainable methods"; }
                string returnString = "";
                int count = 1;
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    returnString += count.ToString() + ") ";
                    count++;
                    returnString += ((methodInfo.IsPublic ? "public " : "private ") + (methodInfo.IsStatic ? "static " : "") + (nameof(methodInfo.ReturnType) == "ReturnType" ? "void" : methodInfo.ReturnType.Name) + " " + methodInfo.Name);
                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                    returnString += "(";
                    if (parameterInfos.Length > 0) {
                        for (int i = 0; i < parameterInfos.Length; i++) {
                            returnString += parameterInfos[i].ParameterType.Name + " " + parameterInfos[i].Name + (i != parameterInfos.Length - 1 ? ", " : "");
                        }
                    }
                    returnString += ")\n";
                }
                return returnString;
            }
            else { return $"Target component does not derive from Component base class"; }
        }
        else { return $"Component '{component}' does not exist within '{targetGO.name}' or is disabled"; }
    }
    [TerminalCommand("face", "face [name || target] [towards || away] [name || target || this] - Faces target gameobject towards or away from target gameobject\n")]
    [ICC.IccCommand("face", "Debug", "face [name || target] [towards || away] [name || target || this] - Faces target gameobject towards or away from target gameobject\n")]
    public string Face(string target, string direction, string toFace)
    {
        GameObject targetGO;
        switch (target)
        {
            case "target":
                targetGO = GetTarget().transform.gameObject;
                break;
            case "this":
                targetGO = Cache.surfCharacter.gameObject;
                break;
            default:
                targetGO = GetName(target);
                break;
        }
        if (targetGO == null) { return $"Gameobject '{target}' does not exist within current instance or is disabled"; }
        GameObject toFaceGO;
        switch (toFace)
        {
            case "target":
                toFaceGO = GetTarget().transform.gameObject;
                break;
            case "this":
                toFaceGO = Cache.surfCharacter.gameObject;
                break;
            default:
                toFaceGO = GetName(toFace);
                break;
        }
        if (toFaceGO == null) { return $"Gameobject '{toFace}' does not exist within current instance or is disabled"; }
        targetGO.transform.LookAt(toFaceGO.transform);
        if (direction == "away") { targetGO.transform.eulerAngles = -targetGO.transform.eulerAngles; }
        return $"Faced '{targetGO.name}' {((direction == "away") ? "away from" : "towards")} '{toFaceGO.name}'";
    }
    [TerminalCommand("bind", "bind [+keycode || -keycode || keycode] [action || <<command>>] - Binds given key to given action or command. Use '+' symbol before keycode to define KeyDown, '-' for KeyUp, no symbol for hold. For reference to each action identifier, use command 'references binds'. Binding a command string requires command to be encapsulated within '<< >>'. Command string can include multiple commands strung together with ';'\n")]
    [ICC.IccCommand("bind", "Debug", "bind [+keycode || -keycode || keycode] [action || <<command>>] - Binds given key to given action or command. Use '+' symbol before keycode to define KeyDown, '-' for KeyUp, no symbol for hold. For reference to each action identifier, use command 'references binds'. Binding a command string requires command to be encapsulated within '<< >>'. Command string can include multiple commands strung together with ';'\n")]
    public string Bind(string keycode, string command)
    {
        if (command[0] != '<')
        {
            KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), (keycode[0] == '+' || keycode[0] == '-') ? keycode.Remove(0, 1) : keycode, true);
            switch (keycode[0])
            {
                case '+':
                    InputManager.inputBinds.BindKey(keyCode, command,KeybindType.Down);
                    break;
                case '-':
                    InputManager.inputBinds.BindKey(keyCode, command, KeybindType.Up);
                    break;
                default:
                    InputManager.inputBinds.BindKey(keyCode, command, KeybindType.Key);
                    break;
            }
            InputManager.inputBinds.SaveBinds();
            return $"Bound {keycode} to {command}";
        }
        InputManager.inputBinds.SaveBinds();
        return "";
    }
    [TerminalCommand("clearbinds", "clearbinds - Resets all current and saved binds to default\n")]
    [ICC.IccCommand("clearbinds", "Debug", "clearbinds - Resets all current and saved binds to default\n")]
    public string Clearbinds()
    {
        InputManager.inputBinds.ResetAllBinds();
        return "Reset all binds";
    }
    [TerminalCommand("goto", "goto - Teleports to target\n")]
    [ICC.IccCommand("goto", "Debug", "goto - Teleports to target\n")]
    public string Goto()
    {
        RaycastHit hit = GetTarget();
        if (hit.transform != null) { Cache.moveData.Origin = hit.point; }
        else { return "No target cast"; }
        return $"Teleported to {hit.point}";
    }
    [TerminalCommand("tp", "tp [x] [y] [z] - Teleports to given position\n")]
    [ICC.IccCommand("tp", "Debug", "tp [x] [y] [z] - Teleports to given position\n")]
    public string Tp(float x, float y, float z)
    {
        Vector3 targetPos = new Vector3(x, y, z);
        Cache.surfCharacter.MoveData.Origin = targetPos;
        Cache.surfCharacter.MoveData.Velocity = Vector3.zero;
        return $"Teleported to {targetPos}";
    }
    [TerminalCommand("ptr", "ptr [copy || paste || null] [name || target || this] [component] [variable] - Copies, pastes or sets target variable to null\n")]
    [ICC.IccCommand("ptr", "Debug", "ptr [copy || paste || null] [name || target || this] [component] [variable] - Copies, pastes or sets target variable to null\n")]
    public string Pointer(string action, string target, string component, string variable)
    {
        GameObject go;
        switch (target)
        {
            case "target":
                go = GetTarget().transform.gameObject;
                break;
            case "this":
                go = Cache.surfCharacter.gameObject;
                break;
            default:
                go = GetName(target);
                break;
        }
        if (go == null) { return $"Gameobject '{target}' does not exist within current instance or is disabled"; }

        switch (action)
        {
            case "null":
                if (true)
                {
                    System.Type componentType = Utilities.GetType(component);
                    FieldInfo fieldInfo = componentType.GetField(variable, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    Component targetComponent = go.GetComponent(componentType);
                    int FORCEBREAK = 20;
                    System.Type baseType = componentType.BaseType;
                    if (fieldInfo == null)
                    {
                        while (true)
                        {
                            if (baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                            {
                                FieldInfo[] baseTypeFieldInfos = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                                for (int x = 0; x < baseTypeFieldInfos.Length; x++)
                                {
                                    if (variable == baseTypeFieldInfos[x].Name)
                                    {
                                        fieldInfo = baseTypeFieldInfos[x];
                                        break;
                                    }
                                }
                                baseType = baseType.BaseType;
                            }
                            else { break; }


                            FORCEBREAK--;
                            if (FORCEBREAK < 0) { break; }
                        }
                    }
                    if (fieldInfo == null) { return $"Unknown variable '{variable}' within component '{component}'"; }
                    fieldInfo.SetValue(targetComponent, null);
                    return $"Set variable '{variable}' within component '{component}' within gameobject '{go.name}' to null";
                }
            case "copy":
                if (true)
                {
                    System.Type componentType = Utilities.GetType(component);
                    FieldInfo fieldInfo = componentType.GetField(variable, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    Component targetComponent = go.GetComponent(componentType);
                    int FORCEBREAK = 20;
                    System.Type baseType = componentType.BaseType;
                    if (fieldInfo == null)
                    {
                        while (true)
                        {
                            if (baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                            {
                                FieldInfo[] baseTypeFieldInfos = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                                for (int x = 0; x < baseTypeFieldInfos.Length; x++)
                                {
                                    if (variable == baseTypeFieldInfos[x].Name)
                                    {
                                        fieldInfo = baseTypeFieldInfos[x];
                                        break;
                                    }
                                }
                                baseType = baseType.BaseType;
                            }
                            else { break; }


                            FORCEBREAK--;
                            if (FORCEBREAK < 0) { break; }
                        }
                    }
                    if (fieldInfo == null) { return $"Unknown variable '{variable}' within component '{component}'"; }
                    copiedPtr = new KeyValuePair<Component, FieldInfo>(targetComponent, fieldInfo);
                    return $"Copied '{fieldInfo.Name}'";
                }
            case "paste":
                /*
                Component targetComponenttt = go.GetComponent(component);
                System.Type componentTypeee = Utilities.GetType(component);
                System.Type baseTypeee = componentTypeee.BaseType;
                FieldInfo fieldInfooo = baseTypeee.GetField(variable, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                fieldInfooo.SetValue(targetComponenttt,copiedPtr);
                return $"Pasted '{copiedPtr.Name}' to '{variable}' within component '{component}' within gameobject '{go.transform.name}'";
                */
                if (true)
                {
                    if (copiedPtr.Value == null) { return "No pointer copied"; }
                    System.Type componentType = Utilities.GetType(component);
                    FieldInfo fieldInfo = componentType.GetField(variable, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    Component targetComponent = go.GetComponent(componentType);
                    int FORCEBREAK = 20;
                    System.Type baseType = componentType.BaseType;
                    if (fieldInfo == null)
                    {
                        while (true)
                        {
                            if (baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                            {
                                FieldInfo[] baseTypeFieldInfos = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                                for (int x = 0; x < baseTypeFieldInfos.Length; x++)
                                {
                                    if (variable == baseTypeFieldInfos[x].Name)
                                    {
                                        fieldInfo = baseTypeFieldInfos[x];
                                        break;
                                    }
                                }
                                baseType = baseType.BaseType;
                            }
                            else { break; }


                            FORCEBREAK--;
                            if (FORCEBREAK < 0) { break; }
                        }
                    }
                    if (fieldInfo == null) { return $"Unknown variable '{variable}' within component '{component}'"; }
                    fieldInfo.SetValue(targetComponent,copiedPtr.Value.GetValue(copiedPtr.Key));
                    return $"Pasted '{copiedPtr.Value.Name}' to '{variable}' within component '{component}' within gameobject '{go.transform.name}'";
                }
            default:
                return $"'{action}' is not a valid option";
        }

        //System.Type componentType = Utilities.GetType(component);
      
        /*
        int FORCEBREAK = 20;
        if (fieldInfo == null)
        {
            while (true)
            {
                if (baseType != typeof(MonoBehaviour) && baseType != typeof(Component))
                {
                    FieldInfo[] baseTypeFieldInfos = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    for (int x = 0; x < baseTypeFieldInfos.Length; x++)
                    {
                        if (variable == baseTypeFieldInfos[x].Name)
                        {
                            fieldInfo = baseTypeFieldInfos[x];
                            break;
                        }
                    }
                    baseType = baseType.BaseType;
                }
                else { break; }


                FORCEBREAK--;
                if (FORCEBREAK < 0) { break; }
            }
        }
        */
        return null;
    }
    [TerminalCommand("shoot", "shoot [projectile || cast] [speed] [damage] [impactForce] [distance] - Shoots a projectile or cast with given speed, damage, distance and impact force. Speed value will have no effect on casts\n")]
    [ICC.IccCommand("shoot", "Debug", "shoot [projectile || cast] [speed] [damage] [impactForce] [distance] - Shoots a projectile or cast with given speed, damage, distance and impact force. Speed value will have no effect on casts\n")]
    public string Shoot(string type,string speed,string damage, string impactForce, string distance)
    {
        if (type == "projectile")
        {
            GameObject projectileGO = Instantiate(Cache.references.spawnables["Projectile"], Cache.vcam.transform.position, Cache.vcam.transform.rotation);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            if (float.TryParse(speed, out projectile.speed))
            {
                if (float.TryParse(damage, out projectile.damage))
                {
                    if (float.TryParse(impactForce, out projectile.impactForce))
                    {
                        if (float.TryParse(distance, out projectile.lifetime))
                        {
                            return "Shot projectile";
                        }
                    }
                }
            }
            return "Given value needs to be a number";
        }
        else if (type == "cast")
        {
            float range = 0f;
            if (float.TryParse(distance, out range))
            {
                RaycastHit hit;
                if (Physics.Raycast(Cache.vcam.transform.position, Cache.vcam.transform.forward, out hit, range, Cache.references.bulletLayerMask))
                {
                    Firearm firearm = new Firearm();
                    if (float.TryParse(damage, out firearm.damage))
                    {
                        if (float.TryParse(impactForce, out firearm.impactForce))
                        {
                            ImpactManager.Hit(firearm, hit);
                            if (hit.transform != null) { return $"Cast shot and hit '{hit.transform.name}' at '{hit.point}'"; }
                            return $"Cast shot with nothing returned";
                        }
                    }
                    return "Given value needs to be a number";
                }
                return $"Cast shot with nothing returned";
            }
            return "Given value needs to be a number";
        }
        else { return $"'{type}' is not a valid type of shot"; }
    }
    [TerminalCommand("light", "light - Sets alight target gameobject\n")]
    [ICC.IccCommand("light", "Debug", "light - Sets alight target gameobject\n")]
    public string Light()
    {
        RaycastHit hit;
        if (Physics.Raycast(Cache.vcam.transform.position, Cache.vcam.transform.forward, out hit, 999999f, Cache.references.flammableLayerMask))
        {
            Flammable flammable = hit.transform.GetComponent<Flammable>();
            if (flammable != null)
            {
                flammable.Light();
                return $"Lit '{flammable.gameObject.name}'";
            }
            else
            {
                SpreadableFire spreadableFire = hit.transform.GetComponent<SpreadableFire>();
                if (spreadableFire != null)
                {
                    spreadableFire.Light();
                    return $"Lit '{spreadableFire.gameObject.name}'";
                }
            }
            return $"Target is not flammable! ('{hit.transform.name}'";
        }
        return $"No flammable gameobject found within cast";
    }

    [TerminalCommand("volume", "volume [amount] - Sets volume to given amount between 0-1\n")]
    [ICC.IccCommand("volume", "Debug", "volume [amount] - Sets volume to given amount between 0-1\n")]
    public string Volume(string amount)
    {
        float amountf = 0f;
        if (float.TryParse(amount,out amountf))
        {
            if (Cache.audioVolumeManager.values.ContainsKey("CONSOLE_VOLUME")){
                Cache.audioVolumeManager.UpdateValue("CONSOLE_VOLUME", amountf);
            }
            else{
                Cache.audioVolumeManager.AddValue("CONSOLE_VOLUME", amountf);
            }
            //AudioListener.volume = Mathf.Clamp(amountf,0f,1f);
            return $"Set master volume to {amountf}";
        }
        else
        {
            return "Amount needs to be a value between 0-1";
        }
    }
    [TerminalCommand("timescale", "timescale [amount] - Sets time scale to given amount\n")]
    [ICC.IccCommand("timescale", "Debug", "timescale [amount] - Sets time scale to given amount\n")]
    public string TimeScale(string amount)
    {
        float amountf = 0f;
        if (float.TryParse(amount, out amountf))
        {
            if (TimeScaleManager.values.ContainsKey("CONSOLE_TIMESCALE"))
            {
                TimeScaleManager.UpdateValue("CONSOLE_TIMESCALE", amountf);
            }
            else
            {
                TimeScaleManager.AddValue("CONSOLE_TIMESCALE", amountf);
            }
            //AudioListener.volume = Mathf.Clamp(amountf,0f,1f);
            return $"Set time scale to {amountf}";
        }
        else
        {
            return "Amount needs to be a number";
        }
    }
    [TerminalCommand("fov", "fov [amount] - Sets fov to given amount\n")]
    [ICC.IccCommand("fov", "Debug", "fov [amount] - Sets fov to given amount\n")]
    public string Fov(string amount)
    {
        float amountf = 0f;
        if (amount == "get") { return Cache.fovManager.defaultFov.ToString(); }
        else if (float.TryParse(amount, out amountf))
        {
            Cache.fovManager.defaultFov = amountf;
            return $"Set fov to {amountf}";
        }
        else
        {
            return "Amount needs to be an integer";
        }
    }
    [TerminalCommand("mousesens", "mousesens [amount] - Sets mouse sensitivity to given amount\n")]
    [ICC.IccCommand("mousesens", "Debug", "mousesens [amount] - Sets mouse sensitivity to given amount\n")]
    public string MouseSens(string amount)
    {
        float amountf = 0f;
        if (float.TryParse(amount, out amountf))
        {
            Vector2 amountv2 = new Vector2(amountf,amountf);
            if (Cache.mouseSensitivityManager.values.ContainsKey("CONSOLE")){
                Cache.mouseSensitivityManager.UpdateValue("CONSOLE", amountv2);
            }
            else{
                Cache.mouseSensitivityManager.AddValue("CONSOLE", amountv2);
            }
            return $"Set mouse sensitivity to {amountf}";
        }
        else if (amount == "get")
        {
            float sens = 1f;
            if (Cache.mouseSensitivityManager.values.ContainsKey("CONSOLE")){
                sens = ((Vector2)Cache.mouseSensitivityManager.values["CONSOLE"]).x;
            }
            else{
                Cache.mouseSensitivityManager.AddValue("CONSOLE", Vector2.one);
                sens = ((Vector2)Cache.mouseSensitivityManager.values["CONSOLE"]).x;
            }
            return $"Current mouse sensitivity: {sens}";
        }
        else
        {
            return "Amount needs to be a number";
        }
    }
    [TerminalCommand("clearbind", "bind [+keycode || -keycode || keycode]  - Clears any binds from given key\n")]
    [ICC.IccCommand("clearbind", "Debug", "bind [+keycode || -keycode || keycode]  - Clears any binds from given key\n")]
    public string ClearBind(string keycode)
    {
        KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), (keycode[0] == '+' || keycode[0] == '-') ? keycode.Remove(0, 1) : keycode, true);
        switch (keycode[0])
        {
            case '+':
                InputManager.inputBinds.ClearBoundKey(new Keybind { keycode = keyCode,keybindType = KeybindType.Down});
                break;
            case '-':
                InputManager.inputBinds.ClearBoundKey(new Keybind { keycode = keyCode, keybindType = KeybindType.Up });
                break;
            default:
                InputManager.inputBinds.ClearBoundKey(new Keybind { keycode = keyCode, keybindType = KeybindType.Key });
                break;
        }
        InputManager.inputBinds.SaveBinds();
        return $"Cleared bind {keycode}";
    }
    [TerminalCommand("renderfeature", "renderfeature [enable/disable/toggle] [identifier] - Enables/disables given render feature\n")]
    [ICC.IccCommand("renderfeature", "Debug", "renderfeature [enable/disable/toggle] [identifier] - Enables/disables given render feature\n")]
    public string RenderFeature(string setState, string identifier)
    {
        if (Cache.references.renderFeatures.ContainsKey(identifier))
        {
            if (setState=="enable"|| setState == "disable"|| setState == "toggle")
            {
                MethodInfo methodInfo = Cache.references.renderFeatures[identifier].GetType().GetMethod(Utilities.CapitalizeFirstCharacter(setState));
                methodInfo.Invoke(Cache.references.renderFeatures[identifier],null);
                return $"{setState}d {identifier}";
            }
            return $"'{setState}' is not a valid action";
        }
        return $"Unknown identifier '{identifier}'";
    }
    [TerminalCommand("showagroradius", "showagroradius - Enables/disables agro radius visual\n")]
    [ICC.IccCommand("showagroradius", "Debug", "showagroradius - Enables/disables agro radius visual\n")]
    public string ShowAgroRadius()
    {
        Robot.debugShowAgroRadius = !Robot.debugShowAgroRadius;
        Robot[] enemies = FindObjectsOfType<Robot>();
        foreach (Robot robot in enemies){
            robot.UpdateShowDebugAgroRadius();
        }
        TransformTurret[] turrets = FindObjectsOfType<TransformTurret>();
        foreach (TransformTurret turret in turrets){
            turret.UpdateShowDebugAgroRadius();
        }
        return $"{(Robot.debugShowAgroRadius ? "Enabled" : "Disabled")} agro radius visual";
    }
    [TerminalCommand("setqualitylevel", "setqualitylevel [level || get] - Gets graphics quality level or sets quality level to given value\n")]
    [ICC.IccCommand("setqualitylevel", "Debug", "setqualitylevel [level || get] - Gets graphics quality level or sets quality level to given value\n")]
    public string SetQualityLevel(string setLevel)
    {
        if (setLevel == "get")
        {
            return $"Current quality level: {QualitySettings.GetQualityLevel()}";
        }
        else
        {
            int level = 0;
            if (int.TryParse(setLevel, out level))
            {
                QualitySettings.SetQualityLevel(level);
                return $"Set quality level to {QualitySettings.GetQualityLevel()}";
            }
            else
            {
                return $"Given level needs to be an int";
            }
        }
    }
    /*
    [TerminalCommand("quality", "quality [level || get] - Gets quality level or sets quality level to given value\n")]
    public string Quality(string setLevel)
    {
        if (setLevel == "get")
        {
            return $"Current quality level: {ConfigSave.config.quality}";
        }
        else
        {
            if (setLevel == "0" || setLevel == "Low")
            {
                Cache.configSettings.SetQuality(0);
                return "Set quality level to 1";
            }
            else if (setLevel == "1" || setLevel == "Medium")
            {
                Cache.configSettings.SetQuality(1);
                return "Set quality level to 1";
            }
            else if (setLevel == "2" || setLevel == "High")
            {
                Cache.configSettings.SetQuality(2);
                return "Set quality level to 2";
            }
            else
            {
                return "Unknown quality level";
            }
        }
    }
    */
    [TerminalCommand("god", "god - Enables/disables god mode\n")]
    [ICC.IccCommand("god", "Debug", "god - Enables/disables god mode\n")]
    public string God()
    {
        Cache.health.god = !Cache.health.god;
        return $"{(Cache.health.god ? "Enabled" : "Disabled")} god mode";
    }
    [TerminalCommand("ghost", "ghost - Enables/disables ability to agro robots\n")]
    [ICC.IccCommand("ghost", "Debug", "ghost - Enables/disables ability to agro robots\n")]
    public string Ghost()
    {
        Robot.ghost = !Robot.ghost;
        return $"{(Robot.ghost ? "Enabled" : "Disabled")} ghost mode";
    }
    [TerminalCommand("capfps", "capfps [amount] - Caps frame rate to given amount. Set to 0 to uncap\n")]
    [ICC.IccCommand("capfps", "Debug", "capfps [amount] - Caps frame rate to given amount. Set to 0 to uncap\n")]
    public string Capfps(string amount)
    {
        int fps = 0;
        if (int.TryParse(amount,out fps)){
            Application.targetFrameRate = fps;
            return $"Set target frame rate to {fps}";
        }
        else { return "Given amount needs to be an int"; }
    }

    [TerminalCommand("instance", "instance [name] - Loads target instance. All other loaded instances will be unloaded\n")]
    [ICC.IccCommand("instance", "Debug", "instance [name] - Loads target instance. All other loaded instances will be unloaded\n")]
    public string Instance(string instanceName)
    {
        if (instanceName == "this") { instanceName = SceneManager.GetActiveScene().name; }
        Cache.instanceManagement.LoadInstance(instanceName);
        return "Loading instance '"+instanceName+"'";
    }
    [TerminalCommand("asyncinstance", "asyncinstance [load/unload] [name] - Loads or unloads target instance asynchronously ontop of this instance\n")]
    [ICC.IccCommand("asyncinstance", "Debug", "asyncinstance [load/unload] [name] - Loads or unloads target instance asynchronously ontop of this instance\n")]
    public string AsyncInstance(string action, string instanceName)
    {
        if (instanceName == "this") { instanceName = SceneManager.GetActiveScene().name; }
        if (action == "load") { Cache.instanceManagement.AsyncLoadInstanceAdditive(instanceName); }
        else if (action == "unload") { SceneManager.UnloadSceneAsync(instanceName); }
        else { return $"'{action}' is not a valid action"; }
        return $"{(action=="unload"?"Unl":"L")}oading instance '" + instanceName + "'";
    }
    /* Cant be bothered to add support for a loading method that has no use
    [TerminalCommand("directinstance", "directinstance [load/unload] [name] - Loads or unloads target instance ontop of this instance. Unloading directly is redundant and will be done asynchronously\n")]
    public string DirectInstance(string action, string instanceName)
    {
        if (instanceName == "this") { instanceName = SceneManager.GetActiveScene().name; }
        if (action == "load") { SceneManager.LoadScene(instanceName, LoadSceneMode.Additive); }
        else if (action == "unload") { SceneManager.UnloadSceneAsync(instanceName); }
        else { return $"'{action}' is not a valid action"; }
        return $"{(action == "unload" ? "Unl" : "L")}oading instance '" + instanceName + "'";
    }
    */
    [TerminalCommand("instances", "instances - Lists all instances within build\n")]
    [ICC.IccCommand("instances", "Debug", "instances - Lists all instances within build\n")]
    public string Instances()
    {
        List<string> scenes = Utilities.GetScenesInBuild();
        string returnString = "";
        for (int i = 0; i < scenes.Count; i++){
            returnString += $"{i}) {scenes[i]}\n";
        }
        return returnString;
    }
    [TerminalCommand("fixedcullgroups", "fixedcullgroups [enable/disable] - Enabled/disables fixed cull groups\n")]
    [ICC.IccCommand("fixedcullgroups", "Debug", "fixedcullgroups [enable/disable] - Enabled/disables fixed cull groups\n")]
    public string FixedCullGroups(string action)
    {
        if (action == "disable"||action == "0"){
            foreach (CullGroup cullGroup in CullGroup.cullGroupsLoaded){
                cullGroup.Uncull();
            }
            CullGroup.disabled = true;
            return "Disabled fixed cull groups. All cull groups have been unculled";
        }
        else if (action == "enable" || action == "1"){
            CullGroup.disabled = false;
            return "Enabled fixed cull groups";
        }
        else{
            return $"'{action}' is not a valid action";
        }

    }
    [TerminalCommand("memoryusage", "Returns amount of memory being used on this process\n")]
    [ICC.IccCommand("memoryusage", "Debug", "Returns amount of memory being used on this process\n")]
    public string MemoryUsage()
    {
        System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
        long privateMemory = currentProcess.PrivateMemorySize64;
        return Utilities.FormatBytes(privateMemory);
    }
    [TerminalCommand("gibs", "gibs [clear || get] - Clears all currently active gibs or returns current amount of active gibs\n")]
    [ICC.IccCommand("gibs", "Debug", "gibs [clear || get] - Clears all currently active gibs or returns current amount of active gibs\n")]
    public string Gibs(string action)
    {
        if (action == "clear"){
            GibManager.Clear();
            return "Cleared all currently active gibs";
        }
        else if (action == "get"){
            return GibManager.activeGibs.Count.ToString();
        }
        return $"Unknown action '{action}'";
    }
    [TerminalCommand("crouch16", "Enables/disables crouch16 mechanics. Regular crouch mechanics will be replaced when active\n")]
    [ICC.IccCommand("crouch16", "Debug", "Enables/disables crouch16 mechanics. Regular crouch mechanics will be replaced when active\n")]
    public string ToggleCrouch16()
    {
        Crouch16.active = !Crouch16.active;
        Cache.crouch16.enabled = Crouch16.active;
        Cache.crouch.enableCrouching = !Crouch16.active;
        return $"{(Crouch16.active ? "Enabled" : "Disabled")} crouch16 mechanics";
    }
    [TerminalCommand("resetrespawnsequence", "Resets current respawn sequence\n")]
    [ICC.IccCommand("resetrespawnsequence", "Debug", "Resets current respawn sequence\n")]
    public string ResetRespawnSequence()
    {
        if (Cache.resetSequenceOnRespawn != null) {
            Cache.resetSequenceOnRespawn.ResetSequence();
            return "Reset current respawn sequence";
        }
        else { return "This instance does not support sequence resets"; }
    }
    [TerminalCommand("fog", "Toggles volumetric fog\n")]
    [ICC.IccCommand("fog", "Debug", "Toggles volumetric fog\n")]
    public string Fog()
    {
        Cache.fogManager.Toggle();
        return $"{(FogManager.active ? "Enabled" : "Disable")} volumetric fog";
    }
    [TerminalCommand("foliage", "Toggles terrain foliage\n")]
    [ICC.IccCommand("foliage", "Debug", "Toggles terrain foliage\n")]
    public string Foliage()
    {
        Terrain terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        if (terrain != null){
            terrain.drawTreesAndFoliage = !terrain.drawTreesAndFoliage;
            return $"{(terrain.drawTreesAndFoliage ? "Enabled" : "Disabled")} rendering of foliage";
        }
        else{
            return "No terrain was found within current instance";
        }
    }
    [TerminalCommand("enginefog", "Toggles engine fog\n")]
    [ICC.IccCommand("enginefog", "Debug", "Toggles engine fog\n")]
    public string EngineFog()
    {
        RenderSettings.fog = !RenderSettings.fog;
        return $"{(RenderSettings.fog ? "Enabled" : "Disable")} engine fog";
    }
    [TerminalCommand("map", "map [export || import] [mapname] - Exports currently loaded instance content as a map with given name or imports given map from maps folder\n")]
    [ICC.IccCommand("map", "Debug", "map [export || import] [mapname] - Exports currently loaded instance content as a map with given name or imports given map from maps folder\n")]
    public string Map(string action, string mapName)
    {
        if (action == "export")
        {
            string exportLocation = "";
            MapManager.SerializeMap(mapName,out exportLocation);
            return $"Exported {mapName}.t2km to {exportLocation}";
        }
        else { return "null"; }
    }
    [TerminalCommand("config", "config [save || load || reset || print] - Saves loaded config to disc, loads saved config from disc, resets all config or outputs currently loaded config data\n")]
    [ICC.IccCommand("config", "Debug", "config [save || load || reset || print] - Saves loaded config to disc, loads saved config from disc, resets all config or outputs currently loaded config data\n")]
    public string Config(string action)
    {
        switch (action)
        {
            case "save":
                ConfigSave.Save();
                return $"Saved config to {ConfigSave.dir}";
            case "load":
                ConfigSave.Load();
                return $"Loaded config from {ConfigSave.dir}";
            case "reset":
                ConfigSave.ResetAll();
                return $"Reset all currently saved and loaded config. Currently instanced interface will need to be reset";
            case "print":
                return $"Currently loaded config data:\n{JsonConvert.SerializeObject(ConfigSave.config, new JsonSerializerSettings { Formatting = Formatting.Indented})}";
                break;
            default:
                return $"Unknown action '{action}'";
        }
    }
    [TerminalCommand("setconfig","setconfig [setting] [value] - Sets target setting to given value\n")]
    [ICC.IccCommand("setconfig", "Debug", "setconfig [setting] [value] - Sets target setting to given value\n")]
    public string SetConfig(string setting, string value)
    {
        FieldInfo[] fieldInfos = ConfigSave.config.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        bool foundSetting = false;
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            if (fieldInfos[i].Name == setting)
            {
                foundSetting = true;
                string method = fieldInfos[i].Name;
                method = char.ToUpper(method[0]) + method.Substring(1);
                method = "Set" + method;
                MethodInfo methodInfo = Cache.configSettings.GetType().GetMethod(method);
                object convertedType = Utilities.ConvertStringToType(value, methodInfo.GetParameters()[0].ParameterType);
                methodInfo.Invoke(Cache.configSettings, new object[] { convertedType });
                ConfigSave.config = ConfigSave.edittingConfig;
                ConfigSave.Save();
                return $"Set {setting} to {value}";
            }
        }
        if (!foundSetting) { return $"Unknown setting '{setting}'"; }
        else { return "null"; }
    }
    [TerminalCommand("showclipbrushes","Enables/disables rendering of clip and trigger brushes")]
    [ICC.IccCommand("showclipbrushes", "Debug", "Enables/disables rendering of clip and trigger brushes")]
    public string ShowClipBrushes()
    {
        ClipBrushes.Toggle();
        return $"Rendering of clipbrushes {(ClipBrushes.active ? "enabled" : "disabled")}";
    }
    [TerminalCommand("freecam","Enables/disables freecam")]
    [ICC.IccCommand("freecam", "Debug", "Enables/disables freecam")]
    public string Freecam()
    {
        if (currentFreecam == null){
            currentFreecam = Instantiate(freecam, Camera.main.transform.position, Camera.main.transform.rotation);
            InputManager.enabled = false;
            return "Enabled freecam";
        }
        else{
            Destroy(currentFreecam);
            currentFreecam = null;
            InputManager.enabled = true;
            return "Disabled freecam";
        }
    }
    [TerminalCommand("exitworldintrosequence", "Force exits world into sequence")]
    [ICC.IccCommand("exitworldintrosequence", "Debug", "Force exits world into sequence")]
    public string ExitWorldIntroSequence()
    {
        WorldIntroSequenceManager worldIntroSequenceManager = GameObject.Find("IntroSequenceEvent").GetComponent<WorldIntroSequenceManager>();
        if (worldIntroSequenceManager == null) { return "Could not find intro sequence event manager"; }
        worldIntroSequenceManager.Exit();
        return "Exited world intro sequence";
    }
    [TerminalCommand("getallprogramclasses", "Lists every class in program")]
    [ICC.IccCommand("getallprogramclasses", "Debug", "Lists every class in program")]
    public string GetAllProgramClasses()
    {
        var classes = Utilities.GetAllProgramClasses();
        string output = "";
        foreach (var c in classes){
            output += c + "\n";
        }
        return output;
    }
    [TerminalCommand("connect", "connect [ip] - Connects to server with given ip\n")]
    [ICC.IccCommand("connect", "Debug", "connect [ip] - Connects to server with given ip\n")]
    public string Connect(string ip)
    {
        string serverIP;
        int serverPort;
        try{
            serverIP = ip.Substring(0, ip.IndexOf(':'));
            serverPort = int.Parse(ip.Substring(serverIP.Length+1));
        }
        catch{
            return "Incorrect format of ip. Please format as: x.x.x.x:xxxx";
        }
        Cache.client.ConnectToServer(serverIP, serverPort);
        return "";
        //return $"Attempted to connect to {serverIP}:{serverPort.ToString()}";
    }
    [TerminalCommand("sendpacket", "sendpacket [packet] - Sends packet to currently connected server\n")]
    [ICC.IccCommand("sendpacket", "Debug", "sendpacket [packet] - Sends packet to currently connected server\n")]
    public string SendPacket(string packet)
    {
        if (Client.online)
        {
            Cache.client.SendMessageToServer(packet);
            return "";
            //return "Attempted to send message to server";
        }
        else
        {
            return "Currently not connected to a server";
        }
    }
    [TerminalCommand("outputrawincomingtraffic", "outputrawincomingtraffic - Outputs the raw array of bytes being received from connected server\n")]
    [ICC.IccCommand("outputrawincomingtraffic", "Debug", "outputrawincomingtraffic - Outputs the raw array of bytes being received from connected server\n")]
    public string OutputRawIncomingTraffic()
    {
        Cache.onlineProxyIn.OutputIncomingStream = !Cache.onlineProxyIn.OutputIncomingStream;
        return $"{(Cache.onlineProxyIn.OutputIncomingStream ? "Now outputting" : "No longer outputting")} received data from server";
    }
    [TerminalCommand("outputrawoutgoingtraffic", "outputrawoutgoingtraffic - Outputs the raw array of bytes being sent to connected server\n")]
    [ICC.IccCommand("outputrawoutgoingtraffic", "Debug", "outputrawoutgoingtraffic - Outputs the raw array of bytes being sent to connected server\n")]
    public string OutputRawOutgoingTraffic()
    {
        Cache.client.OutputOutgoingStream = !Cache.client.OutputOutgoingStream;
        return $"{(Cache.client.OutputOutgoingStream ? "Now outputting" : "No longer outputting")} data being sent to server";
    }
    [TerminalCommand("dc", "disconnects from currently connected server\n")]
    [ICC.IccCommand("dc", "Debug", "disconnects from currently connected server\n")]
    public string Dc()
    {
        if (Client.online)
        {
            Cache.client.Disconnect();
            return "";
            //return "Attempted to send disconnect from server. . .";
        }
        else
        {
            return "Currently not connected to a server";
        }
    }
    [TerminalCommand("disconnect", "disconnects from currently connected server\n")]
    [ICC.IccCommand("disconnect", "Debug", "disconnects from currently connected server\n")]
    public string Disconnect()
    {
        if (Client.online)
        {
            Cache.client.Disconnect();
            return "";
            //return "Attempted to send disconnect from server. . .";
        }
        else
        {
            return "Currently not connected to a server";
        }
    }
    [TerminalCommand("screenshot", "Takes a screenshot of the current frame and saves to the screenshots folder\n")]
    [ICC.IccCommand("screenshot", "Debug", "Takes a screenshot of the current frame and saves to the screenshots folder\n")]
    public string Screenshot()
    {
        ScreenshotManager.SaveScreenshot();
        return "Screenshot taken";
    }
    [TerminalCommand("collectgarbage", "Forces a garbage collection to occur\n")]
    [ICC.IccCommand("collectgarbage", "Debug", "Forces a garbage collection to occur\n")]
    public string CollectGarbage()
    {
        System.GC.Collect();
        return "Took out the trash";
    }
    [TerminalCommand("deviceinfo", "Returns info on the system the game is running on\n")]
    [ICC.IccCommand("deviceinfo", "Debug", "Returns info on the system the game is running on\n")]
    public string DeviceInfo()
    {
        string returnString = "";
        returnString += "Device model: " + SystemInfo.deviceModel;
        returnString += "\nDevice type: " + SystemInfo.deviceType;
        returnString += "\nDevice battery status: " + SystemInfo.batteryStatus;
        returnString += "\nDevice operating system: " + SystemInfo.operatingSystem;
        returnString += "\nDevice operating system family: " + SystemInfo.operatingSystemFamily;
        returnString += "\nDevice system memory size: " + SystemInfo.systemMemorySize;
        return returnString;
    }
    [TerminalCommand("screeninfo", "Returns info on the screen being used to run the application\n")]
    [ICC.IccCommand("screeninfo", "Debug", "Returns info on the screen being used to run the application\n")]
    public string ScreenInfo()
    {
        string returnString = "";
        returnString += "\nDisplay systemResolution: x" + Display.displays[Utilities.GetCurrentDisplayNumber()].systemWidth + " y" + Display.displays[Utilities.GetCurrentDisplayNumber()].systemHeight;
        returnString += "\nDisplay renderingResolution: x" + Display.displays[Utilities.GetCurrentDisplayNumber()].renderingWidth + " y" + Display.displays[Utilities.GetCurrentDisplayNumber()].renderingHeight;
        returnString += "\nScreen.resolution: x" + Screen.width + " y" + Screen.height;
        returnString += "\nScreen.currentResolution.resolution: x" + Screen.currentResolution.width + " y" + Screen.currentResolution.height;
        return returnString;
    }
    [ICC.IccCommand("enablelegacyconsole","Legacy","Enables legacy client console")]
    public string EnableLegacyConsole()
    {
        terminal.enabled = true;
        return "Enabled legacy console";
    }
    [ICC.IccCommand("disablelegacyconsole", "Legacy", "Disables legacy client console")]
    public string DisableLegacyConsole()
    {
        terminal.enabled = false;
        return "Disabled legacy console";
    }
    [TerminalCommand("print", "print [string] - Prints string to console\n")]
    [ICC.IccCommand("print", "Debug", "print [string] - Prints string to console\n")]
    public string Print(string str)
    {
        return str;
    }
    [TerminalCommand("exit","Exits application\n")]
    [ICC.IccCommand("exit", "Debug", "Exits application\n")]
    public string Exit()
    {
        if (Application.isEditor) { return "Cannot quit application when in editor"; }
        else
        {
            Application.Quit();
            return "Quitting application. . .";
        }
    }
}
