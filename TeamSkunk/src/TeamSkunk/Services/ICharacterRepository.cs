using System;
using System.Collections.Generic;
using TeamSkunk.Models;

namespace TeamSkunk.Services
{
    public interface ICharacterRepository : IDisposable
    {
        IList<Character> GetCharacters();
        Character GetCharacterByID(int characterId);
        void InsertCharacter(Character character);
        void DeleteCharacter(int characterId);
        void UpdateCharacter(Character character);
        void Save();
    }
}