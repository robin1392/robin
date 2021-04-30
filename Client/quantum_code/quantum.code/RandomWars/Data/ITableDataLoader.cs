using System.Collections.Generic;

public interface ITableLoader<K, V>
{
    bool Run(string sourcePath, string fileName, string targetPath, ref Dictionary<K, V> outValues);
}