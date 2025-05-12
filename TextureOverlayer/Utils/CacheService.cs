using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Dalamud.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TextureOverlayer.Textures;

namespace TextureOverlayer.Utils;

/// <summary>
/// This is going to host file hashes for already saved files to compare against when adding new layers.
/// This will resolve SHA256 hashes to filenames during the loading phase either during the startup OR layer add phases.
/// </summary>
public class CacheService
{

    private Dictionary<Blake3.Hash, String> _rawCache = new Dictionary<Blake3.Hash, String>();
    private String _cacheDir = Service.penumbraApi.GetModDirectory().ToString() + "\\TextureOverlayer\\Raw\\";
    private String _randChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public CacheService()
    {
        LoadCache();
    }

    private void LoadCache()
    {
        if (File.Exists(_cacheDir + "\\_cache.json"))
        {
            try
            {
                using StreamReader reader = new(_cacheDir + "\\_cache.json");
                string json = reader.ReadToEnd();
                _rawCache = JsonConvert.DeserializeObject<Dictionary<Blake3.Hash, String>>(json);
            }
            catch (Exception e)
            {

            }
        }
    }

    public void WriteCache()
    {
        var json = JsonConvert.SerializeObject(_rawCache, Formatting.Indented);
        FilesystemUtil.WriteAllTextSafe(_cacheDir + "\\_cache.json", json);
    }
    

    public Blake3.Hash TryGetCache(Texture texture, string _path)
    {
        var hash = Blake3.Hasher.Hash(File.ReadAllBytes(_path));
        if (hash != null && _rawCache.ContainsKey(hash))
        {
            texture.Load(Service.TextureManager,_cacheDir + _rawCache[hash]);
            return hash;
        }
        else
        {
            var _oldSplit = _path.Split('\\').Last();
            var _newFileName = _oldSplit.Split('.').First() +"-" + GenerateAppend() + "." + _oldSplit.Split('.').Last();
            File.Copy(_path, _cacheDir + _newFileName );
            _rawCache.Add(hash, _newFileName);
            texture.Load(Service.TextureManager,_cacheDir + _newFileName);
            WriteCache();
            return hash;

        }

    }

    private String GenerateAppend()
    {
        var rand = new Random();
        var randnum = rand.Next();
        String append = string.Empty;
        
        append += _randChars[randnum%36];
        randnum /= 36;
        append += _randChars[randnum%36];
        randnum /= 36;
        append += _randChars[randnum%36];
        return append;
    }
}
