using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TeamSkunk.Data;
using TeamSkunk.Models;

namespace TeamSkunk.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext context;
        private ApplicationDbContext Context
        {
            get
            {
                if (context == null || disposed || context.IsDisposed)
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    context = new ApplicationDbContext(optionsBuilder.Options);
                }

                return context;
            }
        }

        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }

        public UnitOfWork()
        {
            if (HttpContextExtension.Current.Items["Context"] == null)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                HttpContextExtension.Current.Items["Context"] = new ApplicationDbContext(optionsBuilder.Options);
            }

            this.context = (ApplicationDbContext)HttpContextExtension.Current.Items["Context"];
            if (context.IsDisposed)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                context = new ApplicationDbContext(optionsBuilder.Options);
            }
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed == false)
            {
                if (disposing == true)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (ValidationException e) 
            {
                // We tried to save a bad entity - Let's make a better exception message so we can fix this shit.

                // Get all the validation error messages.
                var validationErrors = e.ValidationResult.ErrorMessage;

                // Shove all the validation error messages together.
                var combinedValidationErrorMessage = string.Join("; ", validationErrors);

                // Concatentate the base error message with our validation error messages.
                var exceptionMessage = string.Concat(e.Message, "  The validation errors are: ", combinedValidationErrorMessage);

                // Throw the exception back out with the new error message.
                throw new ValidationException(exceptionMessage, e);
            }
        }
        public IGenericRepository<Guild> Guild
        {
            get
            {
                return new Repository<Guild>(Context);
            }
        }
        public IGenericRepository<Member> Member
        {
            get
            {
                return new Repository<Member>(Context);
            }
        }
        public IGenericRepository<Person> Person
        {
            get
            {
                return new Repository<Person>(Context);
            }
        }
        public IGenericRepository<Character> Character
        {
            get
            {
                return new Repository<Character>(Context);
            }
        }
    }

    public interface IUnitOfWork : IDisposable
    {
        void Save();

        IGenericRepository<Guild> Guild { get; }
        IGenericRepository<Member> Member { get; }
        IGenericRepository<Person> Person { get; }
        IGenericRepository<Character> Character { get; }
    }
}
