using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

public class PackageLoader : MonoBehaviour
{
    string packageFolder;
    string packageTempFolder;
    string commandsFolder;
    public string assetBundlesFolder;

    string lastUnpackedPackageName;
    string lastUnpackedPackageNameWithoutExtension;
    private void Awake()
    {
        Cache.packageLoader = this;

        packageFolder = @"Packages";
        packageTempFolder = @"Packages\temp";
        commandsFolder = @"Run";
        assetBundlesFolder = @"Totalled 2k_Data\StreamingAssets";

        if (!Directory.Exists(packageFolder)) { Directory.CreateDirectory(packageFolder); }
        if (!Directory.Exists(commandsFolder)) { Directory.CreateDirectory(commandsFolder); }
        if (!Directory.Exists(assetBundlesFolder)) { Directory.CreateDirectory(assetBundlesFolder); }

        PackagesMeta packagesMeta = null;
        if (File.Exists(packageFolder + @"\packages.meta"))
        {
            string contents = File.ReadAllText(packageFolder + @"\packages.meta");
            try { packagesMeta = JsonConvert.DeserializeObject<PackagesMeta>(contents); }
            catch(System.Exception e) 
            {
                Debug.LogError($"Error deserializing packages.meta: {e.Message}");
                File.Delete(packageFolder + @"\packages.meta");
                File.Create(packageFolder + @"\packages.meta");
            }
        }
        else
        {
            File.Create(packageFolder + @"\packages.meta");
            packagesMeta = new PackagesMeta();
            File.WriteAllText(packageFolder + @"\packages.meta", JsonConvert.SerializeObject(packagesMeta));
        }

        List<string> justLoadedPackages = new List<string>();

        foreach (string filePath in Directory.GetFiles(packageFolder)){
            string fileName = Path.GetFileName(filePath);
            string fileExtension = Path.GetExtension(filePath).ToLower();
            if (fileExtension == ".zip") { LoadPackage(fileName); justLoadedPackages.Add(lastUnpackedPackageNameWithoutExtension); }
        }

        for (int i = 0; i < packagesMeta.PreviouslyLoadedPackages.Count; i++)
        {
            if (!justLoadedPackages.Contains(packagesMeta.PreviouslyLoadedPackages[i])){
                //Cache.terminal.Print("FOUND DELETED PACKAGE! ("+ packagesMeta.PreviouslyLoadedPackages[i]+")");
                UnloadPackageContent(packagesMeta.PreviouslyLoadedPackages[i]);
            }
        }

        packagesMeta.PreviouslyLoadedPackages.Clear();
        for (int i = 0; i < justLoadedPackages.Count; i++){
            packagesMeta.PreviouslyLoadedPackages.Add(justLoadedPackages[i]);
        }

        File.WriteAllText(packageFolder + @"\packages.meta", JsonConvert.SerializeObject(packagesMeta));
    }
    public void UnloadPackageContent(string packageName)
    {
        UnloadPackageAssetsFromFolder(packageName, commandsFolder);
        UnloadPackageAssetsFromFolder(packageName, assetBundlesFolder);
    }
    void UnloadPackageAssetsFromFolder(string packageName, string folderDirectory)
    {
        foreach (string filePath in Directory.GetFiles(folderDirectory)){
            string fileName = Path.GetFileName(filePath);
            //Cache.terminal.Print("CHECKING NAME-- " + fileName.Substring(0, packageName.Length) +"   OVER-- "+packageName);
            if (fileName.Substring(0,packageName.Length) == packageName){
                File.Delete(filePath);
                Cache.terminal.Print($"Unloaded {fileName}");
            }
        }
    }
    public void LoadPackage(string name)
    {
        lastUnpackedPackageName = name;
        lastUnpackedPackageNameWithoutExtension = name.Substring(0, name.Length - 4);
        ExtractZipToTempFolder(packageFolder+@"\"+name);
        ExtractPackageAssets();
    }
    void ExtractZipToTempFolder(string zipFilePath)
    {
        if (!Directory.Exists(packageFolder)) { Directory.CreateDirectory(packageFolder); }

        if (Directory.Exists(packageTempFolder)){
            Directory.Delete(packageTempFolder, true);
        }
        Directory.CreateDirectory(packageTempFolder);

        ZipFile.ExtractToDirectory(zipFilePath, packageTempFolder);
    }
    void ExtractPackageAssets()
    {
        foreach (string filePath in Directory.GetFiles(packageTempFolder)){
            string fileName = Path.GetFileName(filePath);
            string newFilePath = "null";
            Utilities.RenameFile(filePath, $"{lastUnpackedPackageNameWithoutExtension}-{fileName}", out newFilePath);
            fileName = Path.GetFileName(newFilePath);
            string fileExtension = Path.GetExtension(newFilePath).ToLower();

            if (fileExtension == ".t2kc"){
                if (!File.Exists(Path.Combine(commandsFolder, fileName))){
                    File.Move(newFilePath, Path.Combine(commandsFolder, fileName));
                    Cache.terminal.Print($"Unpacked {fileName} from {lastUnpackedPackageName}");
                }
                else {
                    File.Copy(newFilePath, Path.Combine(commandsFolder, fileName),true);
                    Cache.terminal.Print($"Unpacked and overwrote {fileName} from {lastUnpackedPackageName}");
                }
            }
            else if (fileExtension == ""|| fileExtension == ".manifest"){
                if (!File.Exists(Path.Combine(assetBundlesFolder, fileName))){
                    File.Move(newFilePath, Path.Combine(assetBundlesFolder, fileName));
                    Cache.terminal.Print($"Unpacked {fileName} from {lastUnpackedPackageName}");
                }
                else{
                    File.Copy(newFilePath, Path.Combine(assetBundlesFolder, fileName), true);
                    Cache.terminal.Print($"Unpacked and overwrote {fileName} from {lastUnpackedPackageName}");
                }
            }
            else { Cache.terminal.Print($"Unknown asset '{fileName}' from {lastUnpackedPackageName}"); }
        }
    }
}
