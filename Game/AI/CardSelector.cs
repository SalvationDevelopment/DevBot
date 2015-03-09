using System.Collections.Generic;
using DevBot.Game.Enums;

namespace DevBot.Game.AI
{
    public class CardSelector
    {
        private enum SelectType
        {
            Card,
            Cards,
            Id,
            Ids,
            Location
        }

        private SelectType m_type;
        private ClientCard m_card;
        private IList<ClientCard> m_cards;
        private int m_id;
        private IList<int> m_ids;
        private CardLocation m_location;

        public CardSelector(ClientCard card)
        {
            m_type = SelectType.Card;
            m_card = card;
        }

        public CardSelector(IList<ClientCard> cards)
        {
            m_type = SelectType.Cards;
            m_cards = cards;
        }

        public CardSelector(int cardId)
        {
            m_type = SelectType.Id;
            m_id = cardId;
        }

        public CardSelector(IList<int> ids)
        {
            m_type = SelectType.Ids;
            m_ids = ids;
        }

        public CardSelector(CardLocation location)
        {
            m_type = SelectType.Location;
            m_location = location;
        }

        public IList<ClientCard> Select(IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> result = new List<ClientCard>();

            switch (m_type)
            {
                case SelectType.Card:
                    if (cards.Contains(m_card))
                        result.Add(m_card);
                    break;
                case SelectType.Cards:
                    foreach (ClientCard card in m_cards)
                        if (cards.Contains(card))
                            result.Add(card);
                    break;
                case SelectType.Id:
                    foreach (ClientCard card in cards)
                        if (card.Id == m_id)
                            result.Add(card);
                    break;
                case SelectType.Ids:
                    foreach (int id in m_ids)
                        foreach (ClientCard card in cards)
                            if (card.Id == id)
                                result.Add(card);
                    break;
                case SelectType.Location:
                    foreach (ClientCard card in cards)
                        if (card.Location == m_location)
                            result.Add(card);
                    break;
            }

            if (result.Count < min)
            {
                foreach (ClientCard card in cards)
                {
                    if (!result.Contains(card))
                        result.Add(card);
                    if (result.Count >= min)
                        break;
                }
            }

            while (result.Count > max)
                result.RemoveAt(result.Count - 1);

            return result;
        }
    }
}