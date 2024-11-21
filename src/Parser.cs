using System.ComponentModel;

namespace codecrafters_redis;

public class AstroParser(Dictionary<string, (string, long?)> store, string hostDir, string hostFileName)
{
    private string StoreValue(string key, string value)
    {
        store[key] = (value, null);
        return "+OK\r\n";
    }

    private string StoreValueWithExpiry(string key, string value, string expiryTime)
    {
        // convert the string to milliseconds
        long expirationTimeInMilliseconds = long.Parse(expiryTime);
        long currentTimeInMilliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long timeToExpireInMilliseconds = currentTimeInMilliseconds + expirationTimeInMilliseconds;
        
        store[key] = (value, timeToExpireInMilliseconds);
        return "+OK\r\n";
    }

    private string GetValue(string key)
    {
        if (store.TryGetValue(key, out var valueWithExpiry))
        {
            var (value, expirationTime) = valueWithExpiry;

            if (expirationTime.HasValue && DateTimeOffset.Now.ToUnixTimeMilliseconds() > expirationTime.Value)
            {
                return "$-1\r\n";
            }

            return $"${value.Length}\r\n{value}\r\n";
        }
        return "$-1\r\n";
    }
    
    
    public string ParseCommand(string command)
    {
        int minValidNumberOfCommandParts = 2;
        string[] commandParts = command.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

        if (commandParts[0].StartsWith("*"))
        {
            int arrayLength = int.Parse(commandParts[0].Substring(1));

            if (arrayLength > 0 && commandParts.Length >= minValidNumberOfCommandParts)
            {
                string commandToExecute = commandParts[2].ToUpper();

                return commandToExecute switch
                {
                    "PING" => "+PONG\r\n",
                    "ECHO" when commandParts.Length == 5 => $"${commandParts[4].Length}\r\n{commandParts[4]}\r\n",
                    "SET" when commandParts.Length == 7 => StoreValue(commandParts[4], commandParts[6]),
                    "SET" when commandParts.Length == 11 && commandParts[8].ToUpper() == "PX" => StoreValueWithExpiry(commandParts[4], commandParts[6], commandParts[10]),
                    "GET" when commandParts.Length >= 4 => GetValue(commandParts[4]),
                    "CONFIG" when commandParts.Length >= 5 && commandParts[4] == "GET" && commandParts[6] == "dir" => $"*2\r\n${commandParts[6].Length}\r\n{commandParts[6]}\r\n${hostDir.Length}\r\n{hostDir}\r\n",
                    "CONFIG" when commandParts.Length >= 5 && commandParts[4] == "GET" && commandParts[6] == "dbfilename" => $"*2\r\n${commandParts[6].Length}\r\n{commandParts[6]}\r\n${hostFileName.Length}\r\n{hostFileName}\r\n",
                    _ => "-ERR Unknown command\r\n"
                };
            }
        }
        return "-ERR Invalid Request\r\n";
    }
    
}