using Zenject;

public class OnACardPlayedSignal
{
    public Card Card;
    public CharacterController User;
    
    public OnACardPlayedSignal(Card card, CharacterController user)
    {
        Card = card;
        User = user;
    }
}
