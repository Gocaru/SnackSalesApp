using AppSnacks.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSnacks.Services
{
    public class FavouritesService
    {
        private readonly SQLiteAsyncConnection _database;

        public FavouritesService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "favoritos.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<FavouriteProduct>().Wait();
        }

        public async Task<FavouriteProduct> ReadAsync(int id)
        {
            try
            {
                return await _database.Table<FavouriteProduct>().Where(p => p.ProductId == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FavouriteProduct>> ReadAllAsync()
        {
            try
            {
                return await _database.Table<FavouriteProduct>().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateAsync(FavouriteProduct favouriteProduct)
        {
            try
            {
                await _database.InsertAsync(favouriteProduct);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(FavouriteProduct favouriteProduct)
        {
            try
            {
                await _database.DeleteAsync(favouriteProduct);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
