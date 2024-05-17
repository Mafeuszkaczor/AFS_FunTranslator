using Microsoft.AspNetCore.Mvc;
using AFS.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using AFS.Models;
using System.Net;
using System.Text.RegularExpressions;

namespace AFS.Controllers
{
    public class TranslationController : Controller
    {
        private readonly IFunTranslationsService _funTranslationsService;

        public TranslationController(IFunTranslationsService funTranslationsService)
        {
            _funTranslationsService = funTranslationsService;
        }

        public IActionResult Index()
        {
            var viewModel = new TranslationViewModel
            {
                TranslationTypes = GetTranslationTypes()
            };

            return View(viewModel);
        }

        public List<string> GetTranslationTypes()
        {
            string url = "https://funtranslations.com/api/";
            WebClient client = new WebClient();
            string htmlContent = client.DownloadString(url);
            List<string> translationTypes = ExtractTranslationTypes(htmlContent);
            return translationTypes;
        }

        private List<string> ExtractTranslationTypes(string htmlContent)
        {
            List<string> translationTypes = new List<string>();

            Regex regex = new Regex(@"<li><a href=""/api/(.*?)""");
            MatchCollection matches = regex.Matches(htmlContent);

            foreach (Match match in matches)
            {
                string translationType = match.Groups[1].Value;
                translationTypes.Add(translationType);
            }

            return translationTypes;
        }

        [HttpPost]
        public async Task<IActionResult> Index(string text, string translationType)
        {
            List<string> translationTypes = GetTranslationTypes();
            var viewModel = new TranslationViewModel { TranslationTypes = translationTypes };

            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(translationType))
            {
                try
                {
                    var result = await _funTranslationsService.TranslateAsync(text, translationType);
                    var translatedText = DeserializeJSON(result);

                    if (!string.IsNullOrEmpty(translatedText))
                    {
                        viewModel = new TranslationViewModel
                        {
                            Text = text,
                            TranslationType = translationType,
                            TranslatedText = translatedText,
                            TranslationTypes = translationTypes
                        };

                        return View("Index", viewModel);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Translation failed. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"An error occurred during translation: {ex.Message}";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Text to translate and translation type are required.";
            }

            return View("Index", viewModel);
        }

        public string DeserializeJSON(string result)
        {
            try
            {
                dynamic jsonObject = JsonConvert.DeserializeObject(result);
                string translatedText = jsonObject.contents.translated;

                return translatedText;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
