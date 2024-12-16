using System.ComponentModel;
using NewLife.Configuration;

namespace NewLife.CacheServer;

/// <summary>设置</summary>
[Config(@"CacheServer")]
public class Setting : Config<Setting>
{
    /// <summary>调试开关。默认 false</summary>
    [Description("调试开关。默认 false")]
    public Boolean Debug { get; set; }

    /// <summary>端口。默认 1234</summary>
    [Description("端口。默认 1234")]
    public Int32 Port { get; set; } = 1234;

    /// <summary>缓存有效期。默认 24 * 3600</summary>
    [Description("缓存有效期。默认 24 * 3600")]
    public Int32 Expire { get; set; } = 24 * 3600;
}