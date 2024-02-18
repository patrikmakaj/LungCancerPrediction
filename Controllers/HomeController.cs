using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace YourWebApp.Controllers
{
    public class HomeController : Controller
    {
        // Default Index method for handling the default view
        public IActionResult Index()
        {
            return View();
        }

        // New method for invoking the request and displaying the result
        [HttpPost]
        public async Task<IActionResult> InvokeAndDisplayResult(string gender, int age, bool smoking, bool yellowFingers, bool anxiety,
            bool peerPressure, bool chronicDisease, bool fatigue, bool allergy, bool wheezing, bool alcoholConsuming, bool coughing,
            bool shortnessOfBreath, bool swallowingDifficulty, bool chestPain)
        {
            try
            {
                // Check if age is within the specified range
                if (age < 1 || age > 100)
                {
                    ViewBag.PredictionMessage = "Please enter an age between 1 and 100.";
                    return View("Index");
                }

                // Convert boolean values to 1 or 2
                var smokingValue = smoking ? 2 : 1;
                var yellowFingersValue = yellowFingers ? 2 : 1;
                var anxietyValue = anxiety ? 2 : 1;
                var peerPressureValue = peerPressure ? 2 : 1;
                var chronicDiseaseValue = chronicDisease ? 2 : 1;
                var fatigueValue = fatigue ? 2 : 1;
                var allergyValue = allergy ? 2 : 1;
                var wheezingValue = wheezing ? 2 : 1;
                var alcoholConsumingValue = alcoholConsuming ? 2 : 1;
                var coughingValue = coughing ? 2 : 1;
                var shortnessOfBreathValue = shortnessOfBreath ? 2 : 1;
                var swallowingDifficultyValue = swallowingDifficulty ? 2 : 1;
                var chestPainValue = chestPain ? 2 : 1;

                // Prepare the request body based on the form data
                var requestBody = $@"{{
                    ""Inputs"": {{
                        ""input1"": [{{
                            ""GENDER"": ""{gender}"",
                            ""AGE"": {age},
                            ""SMOKING"": {smokingValue},
                            ""YELLOW_FINGERS"": {yellowFingersValue},
                            ""ANXIETY"": {anxietyValue},
                            ""PEER_PRESSURE"": {peerPressureValue},
                            ""CHRONIC DISEASE"": {chronicDiseaseValue},
                            ""FATIGUE"": {fatigueValue},
                            ""ALLERGY"": {allergyValue},
                            ""WHEEZING"": {wheezingValue},
                            ""ALCOHOL CONSUMING"": {alcoholConsumingValue},
                            ""COUGHING"": {coughingValue},
                            ""SHORTNESS OF BREATH"": {shortnessOfBreathValue},
                            ""SWALLOWING DIFFICULTY"": {swallowingDifficultyValue},
                            ""CHEST PAIN"": {chestPainValue}
                        }}]
                    }},
                    ""GlobalParameters"": {{}}
                }}";

                var result = await InvokeRequestResponseService(requestBody);

                // Parse the result and create a user-friendly message
                var prediction = ParsePrediction(result);
                ViewBag.PredictionMessage = prediction;
            }
            catch (Exception ex)
            {
                // Set an error message if an exception occurs
                ViewBag.PredictionMessage = $"Error: {ex.Message}";
            }

            return View("Index");
        }

        private string ParsePrediction(string result)
        {
            // Parse the JSON result to get the prediction values
            var predictionResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
            var lungCancerPrediction = predictionResult.Results.WebServiceOutput0[0].LungCancerPrediction;
            var probability = predictionResult.Results.WebServiceOutput0[0].Probability;

            // Create a user-friendly message based on the prediction
            return lungCancerPrediction == true
                ? $"The data suggests that the patient has cancer with a probability of {probability:P2}."
                : $"The data suggests that the patient does not have cancer with a probability of {probability:P2}.";
        }

        private async Task<string> InvokeRequestResponseService(string requestBody)
        {
            var handler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
            };

            using (var client = new HttpClient(handler))
            {
                const string apiKey = "M1megEBwiuJEOQUZk4pFY1ztkU2dwm4x";

                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new Exception("A key should be provided to invoke the endpoint");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("http://baa86df8-73d3-40af-b535-72882867e90c.westeurope.azurecontainer.io/score");
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"The request failed with status code: {response.StatusCode}\n{responseContent}");
                }
            }
        }

        // New method to handle access to the Privacy page
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
