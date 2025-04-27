namespace BotSharp.Plugin.ESP32.Models;

/// <summary>
/// 设备表
/// </summary>
/// <author>Joey</author>
public class SysDevice : SysRole
{
    private string deviceId;
    private string sessionId;
    private int modelId;
    private int sttId;

    /// <summary>
    /// 设备名称
    /// </summary>
    private string deviceName;

    /// <summary>
    /// 设备状态
    /// </summary>
    private string state;

    /// <summary>
    /// 设备对话次数
    /// </summary>
    private int? totalMessage;

    /// <summary>
    /// 验证码
    /// </summary>
    private string code;

    /// <summary>
    /// 音频文件
    /// </summary>
    private string audioPath;

    /// <summary>
    /// 最后在线时间
    /// </summary>
    private string lastLogin;

    /// <summary>
    /// WiFi名称
    /// </summary>
    private string wifiName;

    /// <summary>
    /// IP
    /// </summary>
    private string ip;

    /// <summary>
    /// 芯片型号
    /// </summary>
    private string chipModelName;

    /// <summary>
    /// 固件版本
    /// </summary>
    private string version;

    public int ModelId
    {
        get { return modelId; }
        set { modelId = value; }
    }

    public int SttId
    {
        get { return sttId; }
        set { sttId = value; }
    }

    public string DeviceId
    {
        get { return deviceId; }
        set { deviceId = value; }
    }

    public string SessionId
    {
        get { return sessionId; }
        set { sessionId = value; }
    }

    public string DeviceName
    {
        get { return deviceName; }
        set { deviceName = value; }
    }

    public string State
    {
        get { return state; }
        set { state = value; }
    }

    public int? TotalMessage
    {
        get { return totalMessage; }
        set { totalMessage = value; }
    }

    public string Code
    {
        get { return code; }
        set { code = value; }
    }

    public string AudioPath
    {
        get { return audioPath; }
        set { audioPath = value; }
    }

    public string LastLogin
    {
        get { return lastLogin; }
        set { lastLogin = value; }
    }

    public string WifiName
    {
        get { return wifiName; }
        set { wifiName = value; }
    }

    public string Ip
    {
        get { return ip; }
        set { ip = value; }
    }

    public string ChipModelName
    {
        get { return chipModelName; }
        set { chipModelName = value; }
    }

    public string Version
    {
        get { return version; }
        set { version = value; }
    }

    public override string ToString()
    {
        return $"SysDevice [deviceId={deviceId}, sessionId={sessionId}, modelId={modelId}, sttId={sttId}, " +
               $"deviceName={deviceName}, state={state}, totalMessage={totalMessage}, code={code}, " +
               $"audioPath={audioPath}, lastLogin={lastLogin}, wifiName={wifiName}, ip={ip}, " +
               $"chipModelName={chipModelName}, version={version}]";
    }
}