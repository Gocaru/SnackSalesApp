using AppSnacks.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppSnacks.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        public static readonly string BaseUrl = "https://qr82pv0n-7243.uks1.devtunnels.ms/";
        private readonly ILogger<ApiService> _logger;
        JsonSerializerOptions _serializerOptions;

        public ApiService(HttpClient httpClient,
                          ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }


        public async Task<ApiResponse<bool>> RegisterUserAsync(string name, string email, string phone, string password)
        {
            try
            {
                var register = new Register()
                {
                    Name = name,
                    Email = email,
                    Phone = phone,
                    Password = password
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Register", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                await Application.Current.MainPage.DisplayAlert("DEBUG",
                    $"Status: {response.StatusCode}\nContent:\n{responseContent}", "OK");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Register failed: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Register failed: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in Register: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }


        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login()
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("DEBUG", $"Erro:\nStatus: {response.StatusCode}\nContent:\n{errorContent}", "OK");

                    _logger.LogError($"Failed to send HTTP request : {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Failed to send HTTP request : {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                if (result == null)
                {
                    _logger.LogError("Failed to deserialize token.");
                    return new ApiResponse<bool> { ErrorMessage = "Invalid token format." };
                }

                Preferences.Set("accesstoken", result!.AccessToken);
                Preferences.Set("userid", (int)result.UserId!);
                Preferences.Set("username", result.UserName);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to log in : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> AddItemInCart(ShoppingCart shoppingCart)
        {
            try
            {
                var json = JsonSerializer.Serialize(shoppingCart, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/ShoppingCartItems", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error while sending HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error while sending HTTP request: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding item to the cart: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {

            var enderecoUrl = BaseUrl + uri;

            try
            {
                var result = await _httpClient.PostAsync(enderecoUrl, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending POST request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

        }

        public async Task<(bool Data, string? ErrorMessage)> UpdateCartItemQuantity(int productId, string action)
        {
            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var response = await PutRequest($"api/ShoppingCartItems?productId={productId}&action={action}", content);
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (false, errorMessage);
                    }
                    string generalErrorMessage = $"Request error: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (false, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"HTTP request error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"An unexpected error occurred: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
        }

        private async Task<HttpResponseMessage> PutRequest(string uri, HttpContent content)
        {
            var urlAddress = AppConfig.BaseUrl + uri;
            try
            {
                AddAuthorizationHeader();
                var result = await _httpClient.PutAsync(urlAddress, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending PUT request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }



        public async Task<(List<Category>? Categories, string? ErrorMessage)> GetCategories()
        {
            return await GetAsync<List<Category>>("api/categories");
        }

        /// <summary>
        /// Fetches a list of popular products.
        /// Calls the endpoint: GET /api/products/popular
        /// </summary>
        public async Task<(List<Product>? Products, string? ErrorMessage)> GetPopularProductsAsync()
        {
            string endpoint = "api/products/popular";
            return await GetAsync<List<Product>>(endpoint);
        }

        /// <summary>
        /// Fetches a list of best-selling products.
        /// Calls the endpoint: GET /api/products/bestsellers
        /// </summary>
        public async Task<(List<Product>? Products, string? ErrorMessage)> GetBestSellingProductsAsync()
        {
            string endpoint = "api/products/bestsellers";
            return await GetAsync<List<Product>>(endpoint);
        }

        /// <summary>
        /// Fetches a list of products for a specific category.
        /// Calls the endpoint: GET /api/products/category/{categoryId}
        /// </summary>
        public async Task<(List<Product>? Products, string? ErrorMessage)> GetProductsByCategoryAsync(int categoryId)
        {
            string endpoint = $"api/products/category/{categoryId}";
            return await GetAsync<List<Product>>(endpoint);
        }

        public async Task<(Product? ProductDetail, string? ErrorMessage)> GetProductDetail(int productId)
        {
            string endpoint = $"api/products/{productId}";
            return await GetAsync<Product>(endpoint);
        }


        public async Task<(List<ShoppingCartItem>? ShoppingCartItems, string? ErrorMessage)> GetShoppingCartItems(int userId)
        {
            var endpoint = $"api/ShoppingCartItems/{userId}";
            return await GetAsync<List<ShoppingCartItem>>(endpoint);
        }


        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Error in the request: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Error in the HTTP request: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"JSON deserialisation error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Unexpected error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }

        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

    }
}
