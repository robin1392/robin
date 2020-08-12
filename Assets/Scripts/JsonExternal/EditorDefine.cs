using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum DataToolMenu
{
    DT_OpenExcel,       //
    DT_ExportBinary,
    DT_ExportBinary_ALL,
    DT_ExportCode,              //
    DT_TableLoadRefresh,
    DT_CopyData,
    DT_CopyCode, 
    DT_CopyAll,     // bat파일 실행으로 한번에 복사
    DT_Export_JSON,
    DT_ExcelPath,
    DT_TEST_JSON,
    DT_S3_UPLOAD,
    DT_S3_UPLOAD_START,
    
    DT_MAX,
}


[Serializable]
public struct FileCheckSum
{
    public string FileName;
    public long FileSize;
    public string FileMD5;
}

public enum FileUpLoadState
{
    Upload_Ready,
    Upload_Sending,
}


// 게임 서비스 상태 -- 데이터를 아마존에서 다운로드 경로 결정하기 위한
public enum E_GameServiceMode
{
    Mode_Dev,
    Mode_Live,
    Mode_MAX,
}

// json data 를 로딩할때 어디서 로딩할지 결정하기 위해
public enum E_GamePlayMode
{
    PlayMode_Dev,
    PlayMode_Online,
    PlayMode_MAX,
}

public class EditorDefine
{

    public static bool g_isUploadComplete = false;
    
    public static string g_exceptionString = "~";
    
    #region path define

    // json class base
    public static string g_edit_txtClassBase = "/Editor/TextBase/JsonClassBase.txt";
    public static string g_edit_txtManagerBase = "/Editor/TextBase/ClassManager.txt";
    
    
    public static string g_edit_pathJsonClass = "/Scripts/JSONData";
    public static string g_edit_name_Manager = "/JsonDataManager.cs";

    public static string g_patchJsonFileName = "/DataCheckList.bin";

    public static string g_pathClientResourcesData = "/Resources/JsonData/";
    #endregion
    
    
    #region class make string define

    public static string g_edit_string_ClassName = "[CLASSNAME]";
    public static string g_edit_string_KeyType = "[KEYTYPE]";
    public static string g_edit_string_KeyName = "[KEYNAME]";
    public static string g_edit_string_VarData = "[VARDATA]";
    public static string g_edit_string_VarDef = "[VARDEFINE]";

    public static string g_edit_mgr_VarDef = "[MGRVARDEF]";
    public static string g_edit_mgr_VarMem = "[MGRVARMEM]";
    public static string g_edit_mgr_VarNull = "[MGRVARNULL]";
    public static string g_edit_mgr_VarCount = "[VARCLASSCOUNT]";
    

    public static string g_edit_string_var_data =
        "\t\t\tinfo.{0} = ({1})JsonDataParse.GetParseData(info.{0}.GetType(), jsonData[i][\"{0}\"].ToString());\n";

    public static string g_edit_arrayvarType = "List<{0}>";

    public static string g_edit_varDefine = "\tpublic {1} {0} = ";
    public static string e_edit_varArrayDefine = "\tpublic {1} {0} = new {1}();\n";
    
    
    
    // manager define
    public static string g_edit_Manager_Count = "\tpublic int loadMaxCount = {0};";
    public static string g_edit_Manager_Var = "\tpublic {0} data{0} = null;\n";
    public static string g_edit_Manager_mem = "\t\tdata{0} = new {0}(filePath , callBack);\n";
    public static string g_edit_Manager_null = "\t\tdata{0}.DestroyData();\n\t\tdata{0} = null;\n";

    #endregion
}
