using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using static PriceTracker.Infrastructure.Constants.DataConstants;
using static PriceTracker.Infrastructure.Exceptions.ValidationMessages.StoreConstants;

namespace PriceTracker.Infrastructure.Data.SeedDatabase.Builders
{
	/// <summary>
	/// Store builder class used for data seeding and validation before data being imported in database 
	/// Enhanced with in-memory duplication tracking
	/// </summary>
	public class StoreBuilder : IBuilder<Store>
	{
		private readonly Store _store;
		private readonly IRepository<Store>? _storeRepository;
		private static readonly HashSet<string> _existingStoreNames = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Creates a new store with required data
		/// </summary>
		/// <param name="name">Store name</param>
		/// <param name="storeRepository">Optional repository for checking existing stores in database</param>
		/// <exception cref="ValidationException">Thrown when validation fails</exception>
		public StoreBuilder(string name, IRepository<Store>? storeRepository = null)
		{
			_storeRepository = storeRepository;

			try
			{
				ValidateStoreInputData(name);

				_store = new Store
				{
					Name = name,
					Prices = new List<Price>(),
					Expenses = new List<Expense>()
				};

				_existingStoreNames.Add(name);
			}
			catch (Exception ex) when (ex is not ValidationException)
			{
				throw new ValidationException($"Failed to create store: {ex.Message}");
			}
		}

		public Store Build() => _store;

		/// <summary>
		/// Clear tracking collections for new seeding session
		/// Call this before starting a new migration or seeding operation
		/// </summary>
		public static void ResetTracking()
		{
			_existingStoreNames.Clear();
		}

		/// <summary>
		/// Get count of currently tracked stores in this session
		/// </summary>
		public static int GetTrackedStoreCount()
		{
			return _existingStoreNames.Count;
		}

		/// <summary>
		/// Check if a store name is already tracked in current session
		/// </summary>
		/// <param name="name">Store name to check</param>
		/// <returns>True if already tracked, false otherwise</returns>
		public static bool IsNameTracked(string name)
		{
			return _existingStoreNames.Contains(name);
		}

		/// <summary>
		/// Get all tracked store names in current session
		/// </summary>
		/// <returns>Collection of tracked store names</returns>
		public static IEnumerable<string> GetTrackedNames()
		{
			return _existingStoreNames.AsEnumerable();
		}

		private void ValidateStoreInputData(string name)
		{
			// Name validations
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ValidationException(NameRequired);
			}

			if (name.Length < storeNameMinLength || name.Length > storeNameMaxLength)
			{
				throw new ValidationException(
					string.Format(InvalidNameLength,
						storeNameMinLength,
						storeNameMaxLength));
			}

			// Check for duplicate in current seed
			if (_existingStoreNames.Contains(name))
			{
				throw new ValidationException(
					string.Format(StoreAlreadyExists, name));
			}

			// Check for duplicate in database if repository is provided
			if (_storeRepository != null)
			{
				var storeExists = _storeRepository
					.AllReadOnly()
					.Any(s => s.Name.ToLower() == name.ToLower());

				if (storeExists)
				{
					throw new ValidationException(
						string.Format(StoreAlreadyExistsInDb, name));
				}
			}
		}
	}
}