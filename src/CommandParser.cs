namespace codecrafters_redis;

public class CommandParser
{
    private string _hostDir = null;
    private string _hostFileName = null;
    
    public CommandParser(string[] args)
    {
        GetHostInfo(args);
    }

    private void GetHostInfo(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--dir" && i + 1 < args.Length)
            {
                _hostDir = args[i + 1];
            }
            
            else if (args[i] == "--dbfilename" && i + 1 < args.Length)
            {
                _hostFileName = args[i + 1];
            }
        }
    }

    public string GetHostDir()
    {
        return _hostDir;
    }

    public string GetHostFileName()
    {
        return _hostFileName;
    }

}