using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using TeamSkunk.Models;
using TeamSkunk.Data;
using TeamSkunk.ViewModels.CharacterViewModels;
using Microsoft.EntityFrameworkCore;

namespace TeamSkunk.Services
{
    public class CharacterRepository : ICharacterRepository, IDisposable
    {
        private ApplicationDbContext context;

        public CharacterRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IList<Character> GetCharacters()
        {
            return context.Characters.ToList();
        }

        public Character GetCharacterByID(int id)
        {
            return context.Characters.FirstOrDefault(m => m.CharacterId == id);
        }

        public void InsertCharacter(Character character)
        {
            context.Characters.Add(character);
        }

        public void DeleteCharacter(int characterId)
        {
            Character character = context.Characters.FirstOrDefault(m => m.CharacterId == characterId);
            context.Characters.Remove(character);
        }

        public void UpdateCharacter(Character character)
        {
            context.Entry(character).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}