using UnityEngine;

public interface IUserInfoDataGenerator
{
    public UserInfoData Generate(NPCRandomValues randomValues, SaloonType saloonType);
}

public class RandomUserInfoGenerator
{
    public UserInfoData Generate(NPCRandomValues randomValues, SaloonType saloonType)
    {
        switch (saloonType)
        {
            case SaloonType.NewBies:
                return new NewBiesUserInfoDataGenerator().Generate(randomValues, saloonType);
            case SaloonType.Rookies:
                return new RookiesUserInfoDataGenerator().Generate(randomValues, saloonType);
            case SaloonType.Nobles:
                return new NoblesUserInfoDataGenerator().Generate(randomValues, saloonType);
        }

        return null;
    }
    
}

public class NewBiesUserInfoDataGenerator : IUserInfoDataGenerator
{
    public UserInfoData Generate(NPCRandomValues randomValues, SaloonType saloonType)
    {
        var randomName = randomValues.Names[Random.Range(0, randomValues.Names.Count)];
        var randomWinAmount = Random.Range(randomValues.NewBie_WinAmountRange.x, randomValues.NewBie_WinAmountRange.y);
        var randomLostAmount = Random.Range(randomValues.NewBie_LostAmountRange.x, randomValues.NewBie_LostAmountRange.y);
        var randomMoney = Random.Range(randomValues.NewBie_MoneyAmountRange.x,randomValues.NewBie_MoneyAmountRange.y);
        
        var data = new UserInfoData(randomName, randomWinAmount, randomLostAmount, randomMoney);
        return data;
    }
}
public class RookiesUserInfoDataGenerator : IUserInfoDataGenerator
{
    public UserInfoData Generate(NPCRandomValues randomValues, SaloonType saloonType)
    {
        var randomName = randomValues.Names[Random.Range(0, randomValues.Names.Count)];
        var randomWinAmount = Random.Range(randomValues.Rookie_WinAmountRange.x, randomValues.Rookie_WinAmountRange.y);
        var randomLostAmount = Random.Range(randomValues.Rookie_LostAmountRange.x, randomValues.Rookie_LostAmountRange.y);
        var randomMoney = Random.Range(randomValues.Rookie_MoneyAmountRange.x,randomValues.Rookie_MoneyAmountRange.y);
        
        var data = new UserInfoData(randomName, randomWinAmount, randomLostAmount, randomMoney);
        return data;
    }
}

public class NoblesUserInfoDataGenerator : IUserInfoDataGenerator
{
    public UserInfoData Generate(NPCRandomValues randomValues, SaloonType saloonType)
    {
        var randomName = randomValues.Names[Random.Range(0, randomValues.Names.Count)];
        var randomWinAmount = Random.Range(randomValues.Noble_WinAmountRange.x, randomValues.Noble_WinAmountRange.y);
        var randomLostAmount = Random.Range(randomValues.Noble_LostAmountRange.x, randomValues.Noble_LostAmountRange.y);
        var randomMoney = Random.Range(randomValues.Noble_MoneyAmountRange.x,randomValues.Noble_MoneyAmountRange.y);
        
        var data = new UserInfoData(randomName, randomWinAmount, randomLostAmount, randomMoney);
        return data;
    }
}

