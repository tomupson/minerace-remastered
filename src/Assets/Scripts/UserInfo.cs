using System;

[Serializable]
public class UserInfo
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public float Experience { get; set; }
    public int HighScore { get; set; }
}