using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace RecipeCardGenerator.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetRecipeCardData(string recipeName)
        {
            string apiKey = "3b4365a9ca034564a8a8c79414c304b8";
            string mask = "ellipseMask";
            string backgroundImage = "none";
            string backgroundColor = "ffffff";
            string fontColor = "333333";

            int? recipeId = await GetRecipeIdFromName(apiKey, recipeName);

            if (recipeId.HasValue)
            {
                string recipeCardData = await GetRecipeCard(apiKey, recipeId.Value, mask, backgroundImage, backgroundColor, fontColor);
                return Content(recipeCardData, "application/json");
            }
            else
            {
                return Content($"Recipe '{recipeName}' not found.", "text/plain", System.Text.Encoding.UTF8);
            }
        }

        private async Task<int?> GetRecipeIdFromName(string apiKey, string recipeName)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://api.spoonacular.com/recipes/");
                var response = await httpClient.GetAsync($"search?apiKey={apiKey}&query={recipeName}&number=1");
                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();
                JObject jsonData = JObject.Parse(responseData);

                var results = jsonData["results"];
                if (results.HasValues)
                {
                    int recipeId = (int)results[0]["id"];
                    return recipeId;
                }

                return null;
            }
        }

        private async Task<string> GetRecipeCard(string apiKey, int recipeId, string mask, string backgroundImage, string backgroundColor, string fontColor)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://api.spoonacular.com/recipes/");
                var response = await httpClient.GetAsync($"{recipeId}/card?apiKey={apiKey}&mask={mask}&backgroundImage={backgroundImage}&backgroundColor={backgroundColor}&fontColor={fontColor}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
