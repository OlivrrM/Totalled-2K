using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Newtonsoft.Json;
using System.Text;

//Used for all com.olivrm games since Jumpy Coupe

public class Utilities : MonoBehaviour
{
    public static int BoolToInt(bool boolean)
    {
        if (boolean) return 1;
        return 0;
    }
    public static int BoolToIntn(bool boolean)
    {
        if (boolean) return 1;
        return -1;
    }
    public static bool IntToBool(int integer)
    {
        if (integer == 1) return true;
        return false;
    }
    public static bool CoinFlip()
    {
        int dice = Random.Range(0, 2);
        return IntToBool(dice);
    }
    public static float CoinFlipn()
    {
        int dice = 0;
        while (true)
        {
            dice = Random.Range(-1, 2);
            if (dice != 0) break;
        }

        return dice;
    }
    public static Color Invisible(Color targetColor)
    {
        return new Color(targetColor.r, targetColor.g, targetColor.b, 0);
    }
    public static Color Visible(Color targetColor)
    {
        return new Color(targetColor.r, targetColor.g, targetColor.b, 1);
    }
    public static float DifferenceBetweenn(float a, float b)
    {
        float answer = 0;
        if (a > b) answer = -(a - b);
        else answer = b - a;
        return answer;
    }
    public static float RoundUpFloat(float n, int places)
    {
        return Mathf.Ceil(n * Mathf.Pow(10, places)) / Mathf.Pow(10, places);
    }

    public static int GetArrayWrappedIndex(int arrayLength, int targetIndex, int offset)
    {
        int offsetLeft = offset;
        int currentTargetIndex = targetIndex;
        int direction = 0;
        if (offset > 0) direction = 1;
        else if (offset < 0) direction = -1;
        else return targetIndex;
        for (int i = 0; i < Mathf.Abs(offset); i++)
        {
            currentTargetIndex += direction;
            if (currentTargetIndex < 0) currentTargetIndex = arrayLength - 1;
            else if (currentTargetIndex >= arrayLength) currentTargetIndex = 0;
            offsetLeft--;
        }
        return currentTargetIndex;
    }
    public static string TruncateString(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }
    public static Vector2 DisplayPosToViewportPos(Vector2 displayPos) //Currently only designed for mobile
    {
        Vector2 displaySize = new Vector2(Display.displays[GetCurrentDisplayNumber()].systemWidth, Display.displays[GetCurrentDisplayNumber()].systemHeight);
        return displayPos / displaySize;
    }
    public static Vector2 ViewportPosToCanvasPos(Vector2 viewportPos) //Only works when canvas is set to screen space and is scaling with screen size
    {
        return viewportPos * new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
    }
    public static int GetCurrentDisplayNumber()
    {
        if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
        { //Supported platforms
            List<DisplayInfo> displayLayout = new List<DisplayInfo>();
            Screen.GetDisplayLayout(displayLayout);
            return displayLayout.IndexOf(Screen.mainWindowDisplayInfo);
        }
        else
        { //Unsupported platform
            return 0; //Display.main ID
        }
    }
    public static string StringBetweenTwoStrings(string str, string FirstString, string LastString)
    {
        string FinalString;
        int Pos1 = str.IndexOf(FirstString) + FirstString.Length;
        int Pos2 = str.IndexOf(LastString);
        FinalString = str.Substring(Pos1, Pos2 - Pos1);
        return FinalString;
    }
    public static Vector3 V3All(float value) { return new Vector3(value, value, value); }
    public static Vector2 V2All(float value) { return new Vector2(value, value); }
    public static Color InvertHue(Color originalColor)
    {
        float ogAlpha = originalColor.a;
        Color.RGBToHSV(originalColor, out float h, out float s, out float v);
        h = (h + 0.5f) % 1.0f;
        Color invertedColor = Color.HSVToRGB(h, s, v);
        invertedColor.a = ogAlpha;
        return invertedColor;
    }
    public static Color DarkenColByPercent(Color originalColor, float percent)
    {
        float ogAlpha = originalColor.a;
        percent = Mathf.Clamp(percent, 0f, 100f);
        Color.RGBToHSV(originalColor, out float h, out float s, out float v);
        v = v * (1 - percent / 100f);
        Color darkenedColor = Color.HSVToRGB(h, s, v);
        darkenedColor.a = ogAlpha;
        return darkenedColor;
    }
    public static Color InvertCol(Color originalColor)
    {
        return new Color(-originalColor.r + 1, -originalColor.g + 1, -originalColor.b + 1, originalColor.a);
    }

    private static float hue = 0.0f;
    private static float saturation = 1.0f;
    private static float value = 1.0f;

    public static Color GetRainbowColor(float speed)
    {
        hue += speed * Time.deltaTime;
        if (hue >= 1.0f) { hue = 0.0f; }
        return HSVToRGB(hue, saturation, value);
    }

    private static Color HSVToRGB(float hue, float saturation, float value)
    {
        int hi = Mathf.FloorToInt(hue * 6) % 6;
        float f = hue * 6 - Mathf.Floor(hue * 6);
        value *= 255.0f;
        int v = Mathf.RoundToInt(value);
        int p = Mathf.RoundToInt(value * (1 - saturation));
        int q = Mathf.RoundToInt(value * (1 - f * saturation));
        int t = Mathf.RoundToInt(value * (1 - (1 - f) * saturation));
        switch (hi)
        {
            case 0:
                return new Color32((byte)v, (byte)t, (byte)p, 255);
            case 1:
                return new Color32((byte)q, (byte)v, (byte)p, 255);
            case 2:
                return new Color32((byte)p, (byte)v, (byte)t, 255);
            case 3:
                return new Color32((byte)p, (byte)q, (byte)v, 255);
            case 4:
                return new Color32((byte)t, (byte)p, (byte)v, 255);
            case 5:
                return new Color32((byte)v, (byte)p, (byte)q, 255);
            default:
                throw new System.Exception("Invalid hue value");
        }
    }
    public static float RandomExclusiveListRange(float min, float max, List<float> excluding)
    {
        while (true)
        {
            float num = Random.Range(min, max);
            if (!excluding.Contains((int)System.Math.Round(num))) { return num; }
        }
    }
    /*
    public static float RandomExclusiveRange(float min, float max, float minExclusive, float maxExclusive)
    {
        while (true)
        {
            float num = Random.Range(min, max);
            if (num < minExclusive && num > maxExclusive) { return num; }
        }
    }
    */
    public static float Vector3Sum(Vector3 vector3)
    {
        return vector3.x + vector3.y + vector3.z;
    }
    public static float Vector3Interpolation(Vector3 A, Vector3 B, Vector3 pos)
    {
        float distanceAB = Vector3.Distance(A, B);
        float distanceAC = Vector3.Distance(A, pos);
        float interpolationValue = 1 - (distanceAC / distanceAB);
        interpolationValue = Mathf.Clamp01(interpolationValue);
        return interpolationValue;
    }
    public static float PerlinNoise3D(float x, float y, float z)
    {
        y += 1;
        z += 2;
        float xy = _perlin3DFixed(x, y);
        float xz = _perlin3DFixed(x, z);
        float yz = _perlin3DFixed(y, z);
        float yx = _perlin3DFixed(y, x);
        float zx = _perlin3DFixed(z, x);
        float zy = _perlin3DFixed(z, y);

        return xy * xz * yz * yx * zx * zy;
    }

    static float _perlin3DFixed(float a, float b)
    {
        return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
    }
    public static Vector3 RandomRangeV3All(float min, float max)
    {
        return new Vector3(Random.RandomRange(min, max), Random.RandomRange(min, max), Random.RandomRange(min, max));
    }
    public static float NormalizedDistance(Vector3 a, Vector3 b, Vector3 current)
    {
        float totalDistance = Vector3.Distance(a, b);
        float distanceToCurrent = Vector3.Distance(a, current);
        float normalizedDistance = Mathf.Clamp01(distanceToCurrent / totalDistance);
        return normalizedDistance;
    }
    public static Vector3 RandomRangeVector3(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.RandomRange(min.x, max.x), Random.RandomRange(min.y, max.y), Random.RandomRange(min.z, max.z));
    }
    public class DictionaryInfo
    {
        public string Name { get; set; }
        public object Dictionary { get; set; }
        public System.Type KeyType { get; set; }
        public System.Type ValueType { get; set; }

        public DictionaryInfo(string name, object dictionary, System.Type keyType, System.Type valueType)
        {
            Name = name;
            Dictionary = dictionary;
            KeyType = keyType;
            ValueType = valueType;
        }
    }
    public static List<DictionaryInfo> GetDictionariesInClass(object obj)
    {
        var dictList = new List<DictionaryInfo>();
        var type = obj.GetType();

        // Get all properties of the given object
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.PropertyType.IsGenericType &&
                property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var value = property.GetValue(obj);
                if (value != null)
                {
                    var keyType = property.PropertyType.GetGenericArguments()[0];
                    var valueType = property.PropertyType.GetGenericArguments()[1];
                    dictList.Add(new DictionaryInfo(property.Name, value, keyType, valueType));
                }
            }
        }

        // Get all fields of the given object
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.FieldType.IsGenericType &&
                field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var value = field.GetValue(obj);
                if (value != null)
                {
                    var keyType = field.FieldType.GetGenericArguments()[0];
                    var valueType = field.FieldType.GetGenericArguments()[1];
                    dictList.Add(new DictionaryInfo(field.Name, value, keyType, valueType));
                }
            }
        }

        return dictList;
    }

    public static System.Type GetType(string TypeName)
    {
        try
        {
            // Try Type.GetType() first. This will work with types defined
            // by the Mono runtime, etc.
            var type = System.Type.GetType(TypeName);

            // If it worked, then we're done here
            if (type != null)
                return type;

            // Get the name of the assembly (Assumption is that we are using
            // fully-qualified type names)
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

            // Attempt to load the indicated Assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;

            // Ask that assembly to return the proper Type
            return assembly.GetType(TypeName);
        }
        catch (System.Exception e) { Debug.LogError("Utilities.GetType() return error catch: " + e.Message); return null; }
    }
    public static List<string> GetScenesInBuild()
    {
        List<string> scenes = new List<string>();
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            scenes.Add(sceneName);
        }
        return scenes;
    }
    public static Quaternion GetRotationTowards(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        return Quaternion.LookRotation(direction);
    }
    public static string CapitalizeFirstCharacter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        char firstChar = char.ToUpper(input[0]);
        string restOfString = input.Substring(1);

        return firstChar + restOfString;
    }
    public static Color SetColorAlpha(Color col, float alpha)
    {
        return new Color(col.r, col.g, col.b, alpha);
    }
    public static List<Transform> GetAllChildTransforms(Transform parent)
    {
        List<Transform> transforms = new List<Transform>();
        foreach (Transform child in parent){
            transforms.Add(child);
            transforms.AddRange(GetAllChildTransforms(child));
        }
        return transforms;
    }
    /*
    public static T DeepCopy<T>(T obj)
    {
        using (var ms = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;
            return (T)formatter.Deserialize(ms);
        }
    }
    */
    public static T DeepCopy<T>(T obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        return JsonConvert.DeserializeObject<T>(json);
    }
    public static bool AreObjectsEqual<T>(T obj1, T obj2)
    {
        if (obj1 == null || obj2 == null)
            return obj1 == null && obj2 == null;
        if (obj1.GetType() != obj2.GetType())
            return false;

        var type = typeof(T);
        foreach (var property in type.GetProperties())
        {
            var value1 = property.GetValue(obj1);
            var value2 = property.GetValue(obj2);

            if (!AreValuesEqual(value1, value2))
                return false;
        }
        foreach (var field in type.GetFields())
        {
            var value1 = field.GetValue(obj1);
            var value2 = field.GetValue(obj2);

            if (!AreValuesEqual(value1, value2))
                return false;
        }

        return true;
    }

    public static bool AreValuesEqual(object value1, object value2)
    {
        if (value1 == null || value2 == null)
            return value1 == null && value2 == null;
        if (value1 is System.Collections.IEnumerable enumerable1 && value2 is System.Collections.IEnumerable enumerable2)
        {
            var enum1 = enumerable1.Cast<object>().ToList();
            var enum2 = enumerable2.Cast<object>().ToList();

            if (enum1.Count != enum2.Count)
                return false;

            for (int i = 0; i < enum1.Count; i++)
            {
                if (!AreValuesEqual(enum1[i], enum2[i]))
                    return false;
            }

            return true;
        }
        if (value1.GetType().IsClass && value1.GetType() != typeof(string))
        {
            return AreObjectsEqual(value1, value2);
        }
        return object.Equals(value1, value2);
    }
    public static ScriptableRendererFeature FindUrpRenderFeature(UniversalRendererData urpData, string name)
    {
        for (int x = 0; x < urpData.rendererFeatures.Count; x++){
            if (urpData.rendererFeatures[x].name == name){
                return urpData.rendererFeatures[x];
            }
        }
        return null;
    }
    //This should not be used as a way to swap out Pipeline Render Data for quality settings, instead use 'Render Pipeline Asset' within each quality level
    public static void SetTargetPipelineRendererType(UniversalRenderPipelineAsset pipeline, int index)
    {
        System.Type typ = typeof(UniversalRenderPipelineAsset);
        FieldInfo type = typ.GetField("m_DefaultRendererIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        type.SetValue(pipeline, index);
    }
    public static Vector2 SysVec2ToUnityVec2(System.Numerics.Vector2 vector2){
        return new Vector2(vector2.X, vector2.Y);
    }
    public static Vector3 SysVec3ToUnityVec3(System.Numerics.Vector3 vector3){
        return new Vector3(vector3.X, vector3.Y,vector3.Z);
    }
    public static System.Numerics.Vector2 UnityVec2ToSysVec2(Vector2 vector2){
        return new System.Numerics.Vector2 { X = vector2.x, Y = vector2.y };
    }
    public static System.Numerics.Vector2 UnityVec3ToSysVec3(Vector2 vector2){
        return new System.Numerics.Vector2 { X = vector2.x, Y = vector2.y };
    }
    public static object ConvertStringToType(string input, System.Type targetType)
    {
        try
        {
            if (targetType == typeof(string)){
                return input;
            }
            else if (targetType.IsEnum){
                return System.Enum.Parse(targetType, input);
            }
            else if (targetType == typeof(System.Guid)){
                return System.Guid.Parse(input);
            }
            else if (targetType == typeof(System.DateTime)){
                return System.DateTime.Parse(input);
            }
            else if (targetType == typeof(System.TimeSpan)){
                return System.TimeSpan.Parse(input);
            }
            else{
                return System.Convert.ChangeType(input, targetType); //Convert.ChangeType can handle most types
            }
        }
        catch (System.Exception ex){
            return null;
        }
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < 0f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        if (angle > 180f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
    public static string GetSceneHierarchy()
    {
        StringBuilder hierarchy = new StringBuilder();
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++){
            bool isLastRoot = i == rootObjects.Length - 1;
            AppendGameObjectHierarchy(hierarchy, rootObjects[i], "", isLastRoot);
        }
        return hierarchy.ToString();
    }
    public static string GetSceneHierarchy(Scene scene)
    {
        StringBuilder hierarchy = new StringBuilder();
        GameObject[] rootObjects = scene.GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++)
        {
            bool isLastRoot = i == rootObjects.Length - 1;
            AppendGameObjectHierarchy(hierarchy, rootObjects[i], "", isLastRoot);
        }
        return hierarchy.ToString();
    }
    public static void AppendGameObjectHierarchy(StringBuilder hierarchy, GameObject obj, string prefix, bool isLast)
    {
        string currentPrefix = prefix + (isLast ? " └───" : " ├───");
        hierarchy.AppendLine(currentPrefix + obj.name);
        string newPrefix = prefix + (isLast ? "    " : " │   ");
        for (int i = 0; i < obj.transform.childCount; i++){
            bool isLastChild = i == obj.transform.childCount - 1;
            AppendGameObjectHierarchy(hierarchy, obj.transform.GetChild(i).gameObject, newPrefix, isLastChild);
        }
    }
    public static void RenameFile(string currentFilePath, string newFileName, out string newFilePath)
    {
        newFilePath = "null";
        if (!File.Exists(currentFilePath)){
            Debug.LogError("The specified file does not exist.");
            return;
        }
        string directory = Path.GetDirectoryName(currentFilePath);
        newFilePath = Path.Combine(directory, newFileName);
        try{
            File.Move(currentFilePath, newFilePath);
        }
        catch (IOException ex){
            Debug.LogError("An error occurred: " + ex.Message);
        }
    }
    public static bool DoesSceneExist(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneFileName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneFileName == sceneName){
                return true;
            }
        }
        return false;
    }
    public static string ToReadableByteArray(byte[] bytes)
    {
        return string.Join(", ", bytes);
    }
    public static void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1){
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    // SmoothDamp ensures a frame-rate independent interpolation
    public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Quaternion velocity, float smoothTime, float deltaTime)
    {
        if (deltaTime < Mathf.Epsilon) return current;

        float t = 1f - Mathf.Exp(-deltaTime / smoothTime);
        return Quaternion.Slerp(current, target, t);
    }
    public static float NextFloat(System.Random random, float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }
    public static void ApplyStructToMaterial<T>(T settings, Material mat) where T : struct
    {
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            string fieldName = field.Name;
            string shaderProperty = "_" + char.ToUpper(fieldName[0]) + fieldName.Substring(1);

            object value = field.GetValue(settings);

            if (value is float floatVal)
                mat.SetFloat(shaderProperty, floatVal);
            else if (value is int intVal)
                mat.SetInt(shaderProperty, intVal);
            else if (value is Color colorVal)
                mat.SetColor(shaderProperty, colorVal);
            else if (value is Vector4 vec4Val)
                mat.SetVector(shaderProperty, vec4Val);
            else if (value is Texture texVal)
                mat.SetTexture(shaderProperty, texVal);
            else if (value is Vector3 vec3Val)
                mat.SetVector(shaderProperty, vec3Val);
            else if (value is Vector2 vec2Val)
                mat.SetVector(shaderProperty, vec2Val);
            else
                Debug.LogWarning($"Unsupported field type '{field.FieldType}' for '{field.Name}'");
        }
    }
    public static Vector2 GetCursorDirection()
    {
        Vector2 mousePos = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float normalizedX = mousePos.x / screenWidth;
        float normalizedY = mousePos.y / screenHeight;
        float directionX = (normalizedX - 0.5f) * 2f;
        float directionY = (normalizedY - 0.5f) * 2f;

        return new Vector2(directionX, directionY);
    }
    public static void ResizeRenderTexture(RenderTexture renderTexture, int width, int height)
    {
        if (renderTexture)
        {
            renderTexture.Release();
            renderTexture.width = width;
            renderTexture.height = height;
            //renderTexture.Create();
        }
    }
    public static bool IsSceneLoaded(string sceneName) 
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.IsValid();
    }
    public static List<string> GetAllProgramClasses()
    {
        var results = new List<string>();
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            // Skip Unity and system assemblies for clarity
            /*
            if (assembly.FullName.StartsWith("Unity") ||
                assembly.FullName.StartsWith("System") ||
                assembly.FullName.StartsWith("mscorlib") ||
                assembly.FullName.StartsWith("Mono"))
                continue;
            */
            System.Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }

            foreach (var type in types)
            {
                if (type == null || !type.IsClass || type.Namespace == null)
                    continue;

                results.Add(type.FullName);
            }
        }

        results.Sort();
        return results;
    }
    public static string FormatBytes(long bytes){
        return $"{(bytes / (1024f * 1024f)):F2} MB";
    }
}

