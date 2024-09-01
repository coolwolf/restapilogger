public sealed class RestApiLoggerConf
{
    public int EventId { get; set; }
    public string LogLevels { get; set; }="None";
    public string ApiHost {get; set; }="";
    public string ApiUrl {get; set; }="/";
    public string ApiMethod {get; set; }="GET";
    public string AuthType { get; set; }= "None";
    public string AuthUser { get; set; }="";
    public string AuthPass { get; set; }="";
}