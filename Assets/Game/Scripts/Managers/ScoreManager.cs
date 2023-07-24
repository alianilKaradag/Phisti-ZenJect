using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    public CharacterController GetWinner(List<CharacterController> users)
    {
        var maxScore = 0;
        var userWhoHasMostCard = users.OrderByDescending( x=> x.GainedCards.Count).FirstOrDefault();
        CharacterController winner = CharacterManager.Instance.Player;

        foreach (var user in users)
        {
            var userScore = 0;
            
            foreach (var item in user.GainedCards)
            {
                if (item.CardValue == CardValue.Ten && item.CardType == CardType.Diamond)
                {
                    userScore += 3;
                }

                if (item.CardValue == CardValue.Two && item.CardType == CardType.Club)
                {
                    userScore += 2;
                }

                if (item.CardValue == CardValue.Joker)
                {
                    userScore += 1;
                }

                if (item.CardValue == CardValue.One)
                {
                    userScore += 1;
                }
            }

            foreach (var item in user.Phistis)
            {
                if (item.CardValue == CardValue.Joker)
                {
                    userScore += 10;
                }
                else
                {
                    userScore += 5;
                }
            }

            if (user == userWhoHasMostCard)
            {
                userScore += 3;
            }

            if (userScore > maxScore)
            {
                maxScore = userScore;
                winner = user;
            }
        }

        return winner;
    }
}
