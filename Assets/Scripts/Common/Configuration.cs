using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONOBJECT;

public class Configuration
{
    // 게임 서비스 상태 -- 개발 모드
    public E_GameServiceMode eGameServiceMode = E_GameServiceMode.Mode_Dev;
    
    
    // 게임 플레이 데이터 다운모드 상태
    public E_GamePlayMode eGamePlayMode = E_GamePlayMode.PlayMode_Dev;
    
    
    public static Configuration GetConfigFile()
    {
        var config = Resources.Load<TextAsset>("Configuration");

        if (config == null)
        {
            SaveConfigFile(new Configuration());
            config = Resources.Load<TextAsset>("Configuration");
        }
        
        return JsonHelper.Deserialize<Configuration>(config.text);
    }
    
    public static void SaveConfigFile(Configuration configuration)
    {
        var str = JsonHelper.ToJson<Configuration>(configuration);
        System.IO.File.WriteAllText(Application.dataPath + "/Resources/Configuration.json", str, System.Text.Encoding.UTF8);
    }
    
    
    
    
}
