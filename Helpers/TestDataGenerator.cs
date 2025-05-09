using System;



public static class TestDataGenerator
{
    public static string GenerateRandomEmail()
    {
        var guid = Guid.NewGuid().ToString().Substring(0, 8);
        return $"testuser_{guid}@example.com";
    }
}